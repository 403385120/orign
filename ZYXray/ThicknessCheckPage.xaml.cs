using ATL.UI.Controls;
using System.Windows.Markup;
using ZYXray.Models;
using ZYXray.ViewModels;
using System.Collections.Generic;
using ZYXray.Utils;
using System;
using System.Linq;
using XRayClient.Core;
using Shuyz.Framework.Mvvm;
using System.Windows.Input;
using System.Threading;
using ZY.Logging;
using ZY.MitutoyoReader;
using System.Windows;
using System.IO;
using ATL.Common;
using System.Data;
using System.Windows.Controls;
using ATL.Core;

namespace ZYXray
{
    /// <summary>
    /// ThicknessCheck.xaml 的交互逻辑
    /// </summary>
    public partial class ThicknessCheckPage : BasePage, IComponentConnector
    {
        private static ATL.Engine.PLC plc = new ATL.Engine.PLC();
        private static int measuretimes = 0;
        public static BaseFacade baseFacade = new BaseFacade();
        private bool stop = false;
        float[] linearRange = new float[] { 999, 999, 999, 999 };

        private CncButton MyCncButton
        {
            get { return CncButton.Instance; }
        }
        public CheckParamsConfig MyCheckParamsConfig
        {
            get { return CheckParamsConfig.Instance; }
        }

        public ThicknessCheckPage()
        {
            InitializeComponent();

            DataContext = this;

            SelectCaliType_ComboBox();


        }

        private static List<ThicknessCheckData> thickData = new List<ThicknessCheckData>();

        public IEnumerable<ECalibrationBlockModel> BindableBlockModels
        {
            get
            {
                return Enum.GetValues(typeof(ECalibrationBlockModel))
                    .Cast<ECalibrationBlockModel>();
            }
        }
        public ECalibrationBlockModel BlockModelA
        {
            get
            {
                return (ECalibrationBlockModel)CheckParamsConfig.Instance.ThicknessCalibrationModeA;
            }
            set
            {
                CheckParamsConfig.Instance.ThicknessCalibrationModeA = (int)value;
            }
        }

        public ECalibrationBlockModel BlockModelB
        {
            get
            {
                return (ECalibrationBlockModel)CheckParamsConfig.Instance.ThicknessCalibrationModeB;
            }
            set
            {
                CheckParamsConfig.Instance.ThicknessCalibrationModeB = (int)value;
            }
        }

        public ECalibrationBlockModel BlockModelC
        {
            get
            {
                return (ECalibrationBlockModel)CheckParamsConfig.Instance.ThicknessCalibrationModeC;
            }
            set
            {
                CheckParamsConfig.Instance.ThicknessCalibrationModeC = (int)value;
            }
        }

        public ECalibrationBlockModel BlockModelD
        {
            get
            {
                return (ECalibrationBlockModel)CheckParamsConfig.Instance.ThicknessCalibrationModeD;
            }
            set
            {
                CheckParamsConfig.Instance.ThicknessCalibrationModeD = (int)value;
            }
        }

        public List<ThicknessCheckData> ThickData
        {
            get
            {
                return thickData;
            }
            set
            {
                thickData = value;
            }
        }

        public float MinThickness1
        {
            get { return CheckParamsConfig.Instance.MinThicknessS; }
        }
        public float MaxThickness1
        {
            get { return CheckParamsConfig.Instance.MaxThicknessS; }
        }
        public float MinThickness2
        {
            get { return CheckParamsConfig.Instance.MinThicknessM; }
        }
        public float MaxThickness2
        {
            get { return CheckParamsConfig.Instance.MaxThicknessM; }
        }
        public float MinThickness3
        {
            get { return CheckParamsConfig.Instance.MinThicknessB; }
        }
        public float MaxThickness3
        {
            get { return CheckParamsConfig.Instance.MaxThicknessB; }
        }

        public ICommand StartCheck
        {
            get
            {
                return new RelayCommand(new Action(delegate
                {
                    measuretimes = 0;
                    if (MyCncButton.IsStartBtnEnable == false)
                    {
                        MessageBox.Show("请停止自动测试！");
                        return;
                    }
                    if (plc.ReadByVariableName("TestMode") != "10")
                    {
                        MessageBox.Show("PLC不在测厚点检模式，不能点检");
                        return;
                    }
                    System.Windows.Forms.DialogResult dr1 = System.Windows.Forms.MessageBox.Show("是否开始平台1厚度点检？", "提示", System.Windows.Forms.MessageBoxButtons.YesNo);
                    if (dr1 != System.Windows.Forms.DialogResult.Yes)
                    {
                        return;
                    }
                    //plc.WriteByVariableName("SoftwareOnState", 1);
                    //plc.WriteByVariableName("SoftwareHeartbeatPackage", 1);
                    this.stop = false;
                    PromptWindow win = new PromptWindow();
                    win.Message = "请开启机器开始平台1点检";
                    win.Topmost = true;
                    win.Closed += new EventHandler(PromptWindow_Closed);
                    new Thread(() =>
                    {
                        DoMeasure(win);
                    }).Start();
                    win.ShowDialog();
                }), delegate
                {
                    return true;
                });
            }
        }

        private void PromptWindow_Closed(object sender, EventArgs e)
        {
            this.stop = true;
        }

        public void DoMeasure(PromptWindow pw)
        {
            for (int i = 0; i < 3; i++)
            {
                measuretimes++;

                string status = "0";
                int waitcount = 0;
                while (status != "1")
                {
                    status = plc.ReadByVariableName("ThicknessMeasureSignal1");
                    waitcount++;
                    Thread.Sleep(100);
                    if (waitcount > 50)
                    {
                        waitcount = 0;
                        LoggingIF.Log("等待PLC触发工位1测厚信号超过5秒", LogLevels.Info, "DoMeasure");
                    }
                    if (this.stop) { break; }
                }
                this.Dispatcher.BeginInvoke((Action)delegate ()
                {
                    pw.Message = "机器正在点检中，请耐心等候...";
                });

                string mode = "";
                float maxlimit = 0;
                float minlimit = 0;
                string checkresult = "NG";


                string thickness = string.Empty;
                float thicknessvalue = 0;
                int time = 0;

                #region A测厚

                while (!thickness.Contains("01A"))
                {
                    if (time >= 20 || this.stop)//10秒读不出厚度就跳出循环
                        break;
                    MitutoyoReaderIF.ReadThickness(ref thickness, 1);
                    Thread.Sleep(500);
                    time++;
                }

                LoggingIF.Log(string.Format("工位1测厚结果为 {0} ", thickness), LogLevels.Info, "DoMeasure");

                if (thickness.Contains("01A"))
                {
                    thickness = thickness.Substring(thickness.LastIndexOf("A") + 1);
                    thicknessvalue = float.Parse(thickness);
                    float thicknessvaluetmp = thicknessvalue;

                    thicknessvalue = thicknessvalue + CheckParamsConfig.Instance.CaliValThickness;
                    thicknessvalue = thicknessvalue * CheckParamsConfig.Instance.ThicknessKValue + CheckParamsConfig.Instance.ThicknessBValue;

                    LoggingIF.Log(string.Format("厚度为( {0} + {1} ) * {2} + {3}= {4} mm", thicknessvaluetmp, CheckParamsConfig.Instance.CaliValThickness, CheckParamsConfig.Instance.ThicknessKValue, CheckParamsConfig.Instance.ThicknessBValue, thicknessvalue), LogLevels.Info, "DoMeasure");
                }
                else
                {
                    LoggingIF.Log(string.Format("解析字串失败: {0}", thickness), LogLevels.Info, "DoMeasure");
                }

                if (CheckParamsConfig.Instance.ThicknessCalibrationModeA == 0)
                {
                    mode = "小标块";
                    maxlimit = CheckParamsConfig.Instance.MaxThicknessS;
                    minlimit = CheckParamsConfig.Instance.MinThicknessS;
                }
                else if (CheckParamsConfig.Instance.ThicknessCalibrationModeA == 1)
                {
                    mode = "中标块";
                    maxlimit = CheckParamsConfig.Instance.MaxThicknessM;
                    minlimit = CheckParamsConfig.Instance.MinThicknessM;
                }
                else
                {
                    mode = "大标块";
                    maxlimit = CheckParamsConfig.Instance.MaxThicknessB;
                    minlimit = CheckParamsConfig.Instance.MinThicknessB;
                }

                checkresult = "NG";
                if (thicknessvalue <= maxlimit && thicknessvalue >= minlimit)
                {
                    checkresult = "OK";
                }
                thickData.Add(new ThicknessCheckData { index = measuretimes, module = "1", time = DateTime.Now, model = mode, value = double.Parse(thicknessvalue.ToString()), result = checkresult });
                SaveResultToFile_Thickness(measuretimes, mode, thicknessvalue, checkresult, "1");

                plc.WriteByVariableName("ThicknessComplete1", 1);
                //plc.WriteByVariableName("PPGResult1", 1);

                status = "1";
                waitcount = 0;
                while (status != "0")
                {
                    if (this.stop) { break; }
                    status = plc.ReadByVariableName("ThicknessMeasureSignal1");
                    waitcount++;
                    Thread.Sleep(100);
                    if (waitcount > 50)
                    {
                        waitcount = 0;
                        LoggingIF.Log("等待PLC关闭工位1测厚触发信号超过5秒", LogLevels.Info, "DoMeasure");
                    }
                }
                plc.WriteByVariableName("ThicknessComplete1", 0);

                //plc.WriteByVariableName("PPGResult1", 0);
                #endregion

                Thread.Sleep(100);

                thickness = string.Empty;
                thicknessvalue = 0;
                time = 0;
                #region B测厚
                status = "0";
                waitcount = 0;
                while (status != "1")
                {
                    if (this.stop) { break; }
                    status = plc.ReadByVariableName("ThicknessMeasureSignal2");
                    waitcount++;
                    Thread.Sleep(100);
                    if (waitcount > 50)
                    {
                        waitcount = 0;
                        LoggingIF.Log("等待PLC触发工位2测厚信号超过5秒", LogLevels.Info, "DoMeasure");
                    }
                }
                while (!thickness.Contains("01A"))
                {
                    if (time >= 20 || this.stop)//10秒读不出厚度就跳出循环
                        break;
                    MitutoyoReaderIF.ReadThickness(ref thickness, 2);
                    Thread.Sleep(500);
                    time++;
                }

                LoggingIF.Log(string.Format("工位2测厚结果为 {0} ", thickness), LogLevels.Info, "DoMeasure");

                if (thickness.Contains("01A"))
                {
                    thickness = thickness.Substring(thickness.LastIndexOf("A") + 1);
                    thicknessvalue = float.Parse(thickness);
                    float thicknessvaluetmp = thicknessvalue;

                    thicknessvalue = thicknessvalue + CheckParamsConfig.Instance.CaliValThickness;
                    thicknessvalue = thicknessvalue * CheckParamsConfig.Instance.ThicknessKValue + CheckParamsConfig.Instance.ThicknessBValue;

                    LoggingIF.Log(string.Format("厚度为( {0} + {1} ) * {2} + {3}= {4} mm", thicknessvaluetmp, CheckParamsConfig.Instance.CaliValThickness, CheckParamsConfig.Instance.ThicknessKValue, CheckParamsConfig.Instance.ThicknessBValue, thicknessvalue), LogLevels.Info, "DoMeasure");
                }
                else
                {
                    LoggingIF.Log(string.Format("解析字串失败: {0}", thickness), LogLevels.Info, "DoMeasure");
                }

                if (CheckParamsConfig.Instance.ThicknessCalibrationModeB == 0)
                {
                    mode = "小标块";
                    maxlimit = CheckParamsConfig.Instance.MaxThicknessS;
                    minlimit = CheckParamsConfig.Instance.MinThicknessS;
                }
                else if (CheckParamsConfig.Instance.ThicknessCalibrationModeB == 1)
                {
                    mode = "中标块";
                    maxlimit = CheckParamsConfig.Instance.MaxThicknessM;
                    minlimit = CheckParamsConfig.Instance.MinThicknessM;
                }
                else
                {
                    mode = "大标块";
                    maxlimit = CheckParamsConfig.Instance.MaxThicknessB;
                    minlimit = CheckParamsConfig.Instance.MinThicknessB;
                }

                checkresult = "NG";
                if (thicknessvalue <= maxlimit && thicknessvalue >= minlimit)
                {
                    checkresult = "OK";
                }
                thickData.Add(new ThicknessCheckData { index = measuretimes, module = "2", time = DateTime.Now, model = mode, value = double.Parse(thicknessvalue.ToString()), result = checkresult });
                SaveResultToFile_Thickness(measuretimes, mode, thicknessvalue, checkresult, "2");
                if (i == 2)
                {
                    if (MotionControlVm.Instance.CheckPPG())
                    {
                        CheckParamsConfig.Instance.PpgCheckTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        CheckParamsSettingsVm.Instance.MyCheckParamsConfig.Write();
                        MessageBox.Show("厚度点检通过");
                    }
                }

                #endregion

                plc.WriteByVariableName("ThicknessComplete2", 1);
                //plc.WriteByVariableName("PPGResult2", 1);

                status = "1";
                waitcount = 0;
                while (status != "0")
                {
                    if (this.stop) { break; }
                    status = plc.ReadByVariableName("ThicknessMeasureSignal2");
                    waitcount++;
                    Thread.Sleep(100);
                    if (waitcount > 50)
                    {
                        waitcount = 0;
                        LoggingIF.Log("等待PLC关闭工位2测厚触发信号超过5秒", LogLevels.Info, "DoMeasure");
                    }
                }
                plc.WriteByVariableName("ThicknessComplete2", 0);
                if (this.stop) { break; }
                //plc.WriteByVariableName("PPGResult2", 0);

            }

            this.Dispatcher.BeginInvoke((Action)delegate ()
            {
                datalist.ItemsSource = null;
                datalist.ItemsSource = ThickData;
            });

            this.Dispatcher.BeginInvoke((Action)delegate ()
            {
                pw.Close();
            });
            LoggingIF.Log("已成功退出厚度点检模式", LogLevels.Info, "DoMeasure");
        }

        public ICommand StartCheck2
        {
            get
            {
                return new RelayCommand(new Action(delegate
                {
                    measuretimes = 0;
                    if (MyCncButton.IsStartBtnEnable == false)
                    {
                        MessageBox.Show("请停止自动测试！");
                        return;
                    }
                    if (plc.ReadByVariableName("TestMode") != "10")
                    {
                        MessageBox.Show("PLC不在测厚点检模式，不能点检");
                        return;
                    }

                    System.Windows.Forms.DialogResult dr1 = System.Windows.Forms.MessageBox.Show("是否开始平台2厚度点检？", "提示", System.Windows.Forms.MessageBoxButtons.YesNo);
                    if (dr1 != System.Windows.Forms.DialogResult.Yes)
                    {
                        return;
                    }
                    this.stop = false;
                    PromptWindow win = new PromptWindow();
                    win.Message = "请开启机器开始平台2点检";
                    win.Topmost = true;
                    win.Closed += new EventHandler(PromptWindow_Closed);
                    new Thread(() =>
                    {
                        DoMeasure2(win);
                    }).Start();
                    win.ShowDialog();
                }), delegate
                {
                    return true;
                });
            }
        }

        public void DoMeasure2(PromptWindow pw)
        {
            for (int i = 0; i < 3; i++)
            {
                measuretimes++;

                string status = "0";
                int waitcount = 0;
                while (status != "1")
                {
                    if (this.stop) { break; }
                    status = plc.ReadByVariableName("ThicknessMeasureSignal3");
                    waitcount++;
                    Thread.Sleep(100);
                    if (waitcount > 50)
                    {
                        waitcount = 0;
                        LoggingIF.Log("等待PLC触发工位3测厚信号超过5秒", LogLevels.Info, "DoMeasure2");
                    }
                }
                this.Dispatcher.BeginInvoke((Action)delegate ()
                {
                    pw.Message = "机器正在点检中，请耐心等候...";
                });

                string mode = "";
                float maxlimit = 0;
                float minlimit = 0;
                string checkresult = "NG";


                string thickness = string.Empty;
                float thicknessvalue = 0;
                int time = 0;

                #region 3测厚

                while (!thickness.Contains("01A"))
                {
                    if (time >= 20 || this.stop)//10秒读不出厚度就跳出循环
                        break;
                    MitutoyoReaderIF.ReadThickness(ref thickness, 3);
                    Thread.Sleep(500);
                    time++;
                }

                LoggingIF.Log(string.Format("工位3测厚结果为 {0} ", thickness), LogLevels.Info, "DoMeasure2");

                if (thickness.Contains("01A"))
                {
                    thickness = thickness.Substring(thickness.LastIndexOf("A") + 1);
                    thicknessvalue = float.Parse(thickness);
                    float thicknessvaluetmp = thicknessvalue;

                    thicknessvalue = thicknessvalue + CheckParamsConfig.Instance.CaliValThickness;
                    thicknessvalue = thicknessvalue * CheckParamsConfig.Instance.ThicknessKValue + CheckParamsConfig.Instance.ThicknessBValue;

                    LoggingIF.Log(string.Format("厚度为( {0} + {1} ) * {2} + {3}= {4} mm", thicknessvaluetmp, CheckParamsConfig.Instance.CaliValThickness, CheckParamsConfig.Instance.ThicknessKValue, CheckParamsConfig.Instance.ThicknessBValue, thicknessvalue), LogLevels.Info, "DoMeasure");
                }
                else
                {
                    LoggingIF.Log(string.Format("解析字串失败: {0}", thickness), LogLevels.Info, "DoMeasure2");
                }

                if (CheckParamsConfig.Instance.ThicknessCalibrationModeC == 0)
                {
                    mode = "小标块";
                    maxlimit = CheckParamsConfig.Instance.MaxThicknessS;
                    minlimit = CheckParamsConfig.Instance.MinThicknessS;
                }
                else if (CheckParamsConfig.Instance.ThicknessCalibrationModeC == 1)
                {
                    mode = "中标块";
                    maxlimit = CheckParamsConfig.Instance.MaxThicknessM;
                    minlimit = CheckParamsConfig.Instance.MinThicknessM;
                }
                else
                {
                    mode = "大标块";
                    maxlimit = CheckParamsConfig.Instance.MaxThicknessB;
                    minlimit = CheckParamsConfig.Instance.MinThicknessB;
                }

                checkresult = "NG";
                if (thicknessvalue <= maxlimit && thicknessvalue >= minlimit)
                {
                    checkresult = "OK";
                }
                thickData.Add(new ThicknessCheckData { index = measuretimes, module = "3", time = DateTime.Now, model = mode, value = double.Parse(thicknessvalue.ToString()), result = checkresult });
                SaveResultToFile_Thickness(measuretimes, mode, thicknessvalue, checkresult, "3");

                plc.WriteByVariableName("ThicknessComplete3", 1);
                //plc.WriteByVariableName("PPGResult1", 1);

                status = "1";
                waitcount = 0;
                while (status != "0")
                {
                    if (this.stop) { break; }
                    status = plc.ReadByVariableName("ThicknessMeasureSignal3");
                    waitcount++;
                    Thread.Sleep(100);
                    if (waitcount > 50)
                    {
                        waitcount = 0;
                        LoggingIF.Log("等待PLC关闭工位3测厚触发信号超过5秒", LogLevels.Info, "DoMeasure2");
                    }
                }
                plc.WriteByVariableName("ThicknessComplete3", 0);
                //plc.WriteByVariableName("PPGResult1", 0);
                #endregion

                Thread.Sleep(100);

                thickness = string.Empty;
                thicknessvalue = 0;
                time = 0;
                #region 4测厚
                status = "0";
                waitcount = 0;
                while (status != "1")
                {
                    if (this.stop) { break; }
                    status = plc.ReadByVariableName("ThicknessMeasureSignal4");
                    waitcount++;
                    Thread.Sleep(100);
                    if (waitcount > 50)
                    {
                        waitcount = 0;
                        LoggingIF.Log("等待PLC触发工位4测厚信号超过5秒", LogLevels.Info, "DoMeasure2");
                    }
                }
                while (!thickness.Contains("01A"))
                {
                    if (time >= 20 || this.stop)//10秒读不出厚度就跳出循环
                        break;
                    MitutoyoReaderIF.ReadThickness(ref thickness, 4);
                    Thread.Sleep(500);
                    time++;
                }

                LoggingIF.Log(string.Format("工位4测厚结果为 {0} ", thickness), LogLevels.Info, "DoMeasure2");

                if (thickness.Contains("01A"))
                {
                    thickness = thickness.Substring(thickness.LastIndexOf("A") + 1);
                    thicknessvalue = float.Parse(thickness);
                    float thicknessvaluetmp = thicknessvalue;

                    thicknessvalue = thicknessvalue + CheckParamsConfig.Instance.CaliValThickness;
                    thicknessvalue = thicknessvalue * CheckParamsConfig.Instance.ThicknessKValue + CheckParamsConfig.Instance.ThicknessBValue;

                    LoggingIF.Log(string.Format("厚度为( {0} + {1} ) * {2} + {3}= {4} mm", thicknessvaluetmp, CheckParamsConfig.Instance.CaliValThickness, CheckParamsConfig.Instance.ThicknessKValue, CheckParamsConfig.Instance.ThicknessBValue, thicknessvalue), LogLevels.Info, "DoMeasure");
                }
                else
                {
                    LoggingIF.Log(string.Format("解析字串失败: {0}", thickness), LogLevels.Info, "DoMeasure2");
                }

                if (CheckParamsConfig.Instance.ThicknessCalibrationModeD == 0)
                {
                    mode = "小标块";
                    maxlimit = CheckParamsConfig.Instance.MaxThicknessS;
                    minlimit = CheckParamsConfig.Instance.MinThicknessS;
                }
                else if (CheckParamsConfig.Instance.ThicknessCalibrationModeD == 1)
                {
                    mode = "中标块";
                    maxlimit = CheckParamsConfig.Instance.MaxThicknessM;
                    minlimit = CheckParamsConfig.Instance.MinThicknessM;
                }
                else
                {
                    mode = "大标块";
                    maxlimit = CheckParamsConfig.Instance.MaxThicknessB;
                    minlimit = CheckParamsConfig.Instance.MinThicknessB;
                }

                checkresult = "NG";
                if (thicknessvalue <= maxlimit && thicknessvalue >= minlimit)
                {
                    checkresult = "OK";
                }
                thickData.Add(new ThicknessCheckData { index = measuretimes, module = "4", time = DateTime.Now, model = mode, value = double.Parse(thicknessvalue.ToString()), result = checkresult });
                SaveResultToFile_Thickness(measuretimes, mode, thicknessvalue, checkresult, "4");
                if (i == 2)
                {
                    if (MotionControlVm.Instance.CheckPPG())
                    {
                        CheckParamsConfig.Instance.PpgCheckTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        CheckParamsSettingsVm.Instance.MyCheckParamsConfig.Write();
                        MessageBox.Show("厚度点检通过");
                    }
                }

                #endregion

                plc.WriteByVariableName("ThicknessComplete4", 1);
                //plc.WriteByVariableName("PPGResult2", 1);

                status = "1";
                waitcount = 0;
                while (status != "0")
                {
                    if (this.stop) { break; }
                    status = plc.ReadByVariableName("ThicknessMeasureSignal4");
                    waitcount++;
                    Thread.Sleep(100);
                    if (waitcount > 50)
                    {
                        waitcount = 0;
                        LoggingIF.Log("等待PLC关闭工位4测厚触发信号超过5秒", LogLevels.Info, "DoMeasure2");
                    }
                }
                plc.WriteByVariableName("ThicknessComplete4", 0);
                //plc.WriteByVariableName("PPGResult2", 0);

            }

            this.Dispatcher.BeginInvoke((Action)delegate ()
            {
                datalist.ItemsSource = null;
                datalist.ItemsSource = ThickData;
            });

            this.Dispatcher.BeginInvoke((Action)delegate ()
            {
                pw.Close();
            });
            LoggingIF.Log("已成功退出厚度点检模式", LogLevels.Info, "DoMeasure2");
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            plc.WriteByVariableName("ThicknessMeasureSignal", 2);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            plc.WriteByVariableName("ThicknessMeasureSignal", 0);
        }

        public void SaveResultToFile_Thickness(int num, string model, float thickness, string result, string module)
        {
            string filePath = "D:\\测量数据\\点检数据\\PPG标块点检\\" + ATL.Core.DeivceEquipmentidPlcInfo.lstDeivceEquipmentidPlcInfos[0].ModelInfo;
            string fileName = filePath + "\\PPG_" + DateTime.Now.ToString("yyyyMMdd") + ".csv";

            if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);

            string line = "";
            byte[] myByte = System.Text.Encoding.UTF8.GetBytes(line);

            if (!File.Exists(fileName))
            {
                line = "编号,点检时间,标定块,工位,厚度值,结果\r\n";
                myByte = System.Text.Encoding.UTF8.GetBytes(line);
                using (FileStream fsWrite = new FileStream(fileName, FileMode.Create))
                {
                    byte[] bs = { (byte)0xEF, (byte)0xBB, (byte)0xBF };
                    fsWrite.Write(bs, 0, bs.Length);
                    fsWrite.Write(myByte, 0, myByte.Length);
                };
            }

            string time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            line = num + "," + time + "," + model + "," + module + "," + thickness.ToString("N3") + "," + result;
            line += "\r\n";

            myByte = System.Text.Encoding.UTF8.GetBytes(line);
            using (FileStream fsWrite = new FileStream(fileName, FileMode.Append))
            {
                fsWrite.Write(myByte, 0, myByte.Length);
            };


        }

        /// <summary>
        /// 保存
        /// </summary>
        private void button_Click_2(object sender, RoutedEventArgs e)
        {
            try
            {
                var temp = Grid1.Children;
                foreach (UIElement item in temp)
                {
                    if (item is TextBox)
                    {
                        if ((item as TextBox).Text.Trim() == "")
                        {
                            MessageBox.Show("请填写完整信息！");
                            return;
                        }
                    }
                }
                string sql = "select t.cali_type from cali_block_data_thickness t where t.cali_type='" + textBox_type.Text + "'";
                //sql = "select t.cali_type from cali_block_data t where t.cali_type=''";
                DataSet ds = baseFacade.equipDB.ExecuteDataSet(CommandType.Text, sql);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    sql = "update cali_block_data_thickness t set t.s_min_thickness='" + textBox_s_min.Text + "',t.s_max_thickness='" + textBox_s_max.Text + "',"
                        + "t.m_min_thickness='" + textBox_m_min.Text + "',t.m_max_thickness='" + textBox_m_max.Text + "',"
                        + "t.b_min_thickness='" + textBox_b_min.Text + "',t.b_max_thickness='" + textBox_b_max.Text + "' where t.cali_type='" + textBox_type.Text + "' ";
                    baseFacade.equipDB.ExecuteNonQuery(CommandType.Text, sql);
                }
                else
                {
                    sql = "insert into cali_block_data_thickness  (cali_type,s_min_thickness,s_max_thickness,m_min_thickness,m_max_thickness,b_min_thickness,b_max_thickness) " +
                        "values ('" + textBox_type.Text + "'," + textBox_s_min.Text + "," + textBox_s_max.Text + "," + textBox_m_min.Text + "," + textBox_m_max.Text + "," +
                        "" + textBox_b_min.Text + "," + textBox_b_max.Text + ")";
                    baseFacade.equipDB.ExecuteNonQuery(CommandType.Text, sql);
                }
                MessageBox.Show("保存成功！");
                SelectCaliType_ComboBox();
                foreach (UIElement item in Grid1.Children)
                {
                    if (item is TextBox)
                    {
                        TextBox tb = item as TextBox;
                        tb.Clear();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("保存失败：" + ex.Message);
            }

        }

        /// <summary>
        /// 调用
        /// </summary>
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            if (comboBox.Text != "")
            {
                string sql = "select * from cali_block_data_thickness t where t.cali_type='" + comboBox.Text + "'";
                DataSet ds = baseFacade.equipDB.ExecuteDataSet(CommandType.Text, sql);
                string s = ds.Tables[0].Rows[0]["cali_type"].ToString();
                MyCheckParamsConfig.CurrentModel = s;
                MyCheckParamsConfig.MinThicknessS = Convert.ToSingle(ds.Tables[0].Rows[0]["s_min_thickness"].ToString());
                MyCheckParamsConfig.MaxThicknessS = Convert.ToSingle(ds.Tables[0].Rows[0]["s_max_thickness"].ToString());
                MyCheckParamsConfig.MinThicknessM = Convert.ToSingle(ds.Tables[0].Rows[0]["m_min_thickness"].ToString());
                MyCheckParamsConfig.MaxThicknessM = Convert.ToSingle(ds.Tables[0].Rows[0]["m_max_thickness"].ToString());
                MyCheckParamsConfig.MinThicknessB = Convert.ToSingle(ds.Tables[0].Rows[0]["b_min_thickness"].ToString());
                MyCheckParamsConfig.MaxThicknessB = Convert.ToSingle(ds.Tables[0].Rows[0]["b_max_thickness"].ToString());

                textBox_type.Text = s;
                textBox_s_min.Text = ds.Tables[0].Rows[0]["s_min_thickness"].ToString();
                textBox_s_max.Text = ds.Tables[0].Rows[0]["s_max_thickness"].ToString();
                textBox_m_min.Text = ds.Tables[0].Rows[0]["m_min_thickness"].ToString();
                textBox_m_max.Text = ds.Tables[0].Rows[0]["m_max_thickness"].ToString();
                textBox_b_min.Text = ds.Tables[0].Rows[0]["b_min_thickness"].ToString();
                textBox_b_max.Text = ds.Tables[0].Rows[0]["b_max_thickness"].ToString();

                MyCheckParamsConfig.WriteThicknessParams();
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        private void button2_Click(object sender, RoutedEventArgs e)
        {
            if (comboBox.Text != "")
            {
                string model = comboBox.Text;
                if (!MyCheckParamsConfig.IsEnabled)
                {
                    Login login = new Login();
                    login.ShowDialog();
                    if (login.IsPermission == false)
                        return;
                    MyCheckParamsConfig.IsEnabled = true;
                }
                if (MessageBoxResult.No == System.Windows.MessageBox.Show(model + " 确定删除吗？", "提示", MessageBoxButton.YesNo,
                    MessageBoxImage.Information, MessageBoxResult.No, MessageBoxOptions.ServiceNotification))
                {
                    return;
                }
                string sql = "delete from cali_block_data_thickness  where cali_type='" + comboBox.Text + "'";
                baseFacade.equipDB.ExecuteNonQuery(CommandType.Text, sql);
                SelectCaliType_ComboBox();

                if (model == MyCheckParamsConfig.CurrentModel)
                {
                    MyCheckParamsConfig.CurrentModel = "";
                    MyCheckParamsConfig.MinThicknessS = 0;
                    MyCheckParamsConfig.MaxThicknessS = 0;
                    MyCheckParamsConfig.MinThicknessM = 0;
                    MyCheckParamsConfig.MaxThicknessM = 0;
                    MyCheckParamsConfig.MinThicknessB = 0;
                    MyCheckParamsConfig.MaxThicknessB = 0;
                    MyCheckParamsConfig.WriteThicknessParams();
                }
            }
        }

        /// <summary>
        /// 标块类型绑定ComboBox
        /// </summary>
        private void SelectCaliType_ComboBox()
        {
            string sql = "select t.ID,t.cali_type from cali_block_data_thickness t";
            DataSet ds = baseFacade.equipDB.ExecuteDataSet(CommandType.Text, sql);

            comboBox.ItemsSource = null;
            comboBox.DisplayMemberPath = null;
            comboBox.DisplayMemberPath = "cali_type";
            comboBox.ItemsSource = ds.Tables[0].DefaultView;
            comboBox.Text = MyCheckParamsConfig.CurrentModel;
        }

        private void comboBox_Drop(object sender, EventArgs e)
        {
            SelectCaliType_ComboBox();
        }

        ///<summary>测厚工位2点检</summary>
        private void btnGetPPG2_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                txtPPGChannel2.Text = "";

                bool res = true;
                string ldstring = string.Empty;
                res = MitutoyoReaderIF.ReOpen(HardwareConfig.Instance.MitutoyoConfig2.SerialConfig.PortName, 2);
                if (false == res)
                {
                    ldstring = "COM打开失败!";
                }
                else
                {
                    MitutoyoReaderIF.ReadThickness(ref ldstring, 2);
                }

                if (ldstring.Contains("01A"))
                {
                    string strthickness = ldstring.Substring(ldstring.LastIndexOf("A") + 1);
                    linearRange[1] = float.Parse(strthickness);
                }
                txtPPGChannel2.Text = (linearRange[1]).ToString();
            }
            catch (Exception ex)
            {
                txtPPGChannel2.Text = ex.Message;
                linearRange[1] = 999;
            }
        }

        ///<summary>测厚工位1点检</summary>
        private void btnGetPPG1_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                txtPPGChannel1.Text = "";

                bool res = true;
                string ldstring = string.Empty;
                res = MitutoyoReaderIF.ReOpen(HardwareConfig.Instance.MitutoyoConfig.SerialConfig.PortName, 1);
                if (false == res)
                {
                    ldstring = "COM打开失败!";
                }
                else
                {
                    MitutoyoReaderIF.ReadThickness(ref ldstring, 1);
                }

                if (ldstring.Contains("01A"))
                {
                    string strthickness = ldstring.Substring(ldstring.LastIndexOf("A") + 1);
                    linearRange[0] = float.Parse(strthickness);
                }
                txtPPGChannel1.Text = (linearRange[0]).ToString();
            }
            catch (Exception ex)
            {
                txtPPGChannel1.Text = ex.Message;
                linearRange[0] = 999;
            }
        }

        ///<summary>测厚工位4点检</summary>
        private void btnGetPPG4_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                txtPPGChannel4.Text = "";

                bool res = true;
                string ldstring = string.Empty;
                res = MitutoyoReaderIF.ReOpen(HardwareConfig.Instance.MitutoyoConfig4.SerialConfig.PortName, 4);
                if (false == res)
                {
                    ldstring = "COM打开失败!";
                }
                else
                {
                    MitutoyoReaderIF.ReadThickness(ref ldstring, 4);
                }

                if (ldstring.Contains("01A"))
                {
                    string strthickness = ldstring.Substring(ldstring.LastIndexOf("A") + 1);
                    linearRange[3] = float.Parse(strthickness);
                }
                txtPPGChannel4.Text = (linearRange[3]).ToString();
            }
            catch (Exception ex)
            {
                txtPPGChannel4.Text = ex.Message;
                linearRange[3] = 999;
            }
        }

        ///<summary>测厚工位3点检</summary>
        private void btnGetPPG3_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                txtPPGChannel3.Text = "";

                bool res = true;
                string ldstring = string.Empty;
                res = MitutoyoReaderIF.ReOpen(HardwareConfig.Instance.MitutoyoConfig3.SerialConfig.PortName, 3);
                if (false == res)
                {
                    ldstring = "COM打开失败!";
                }
                else
                {
                    MitutoyoReaderIF.ReadThickness(ref ldstring, 3);
                }

                if (ldstring.Contains("01A"))
                {
                    string strthickness = ldstring.Substring(ldstring.LastIndexOf("A") + 1);
                    linearRange[2] = float.Parse(strthickness);
                }
                txtPPGChannel3.Text = (linearRange[2]).ToString();
            }
            catch (Exception ex)
            {
                txtPPGChannel3.Text = ex.Message;
                linearRange[2] = 999;
            }
        }

        ///<summary>测厚检查、验证、保存结果</summary>
        private void btnGetRange_Click(object sender, RoutedEventArgs e)
        {
            if (linearRange.Contains(999))
            {
                MessageBox.Show("请重新读取四个工位厚度值再开始计算！");
                if (linearRange[0] == 999)
                    txtPPGChannel1.Text = "";
                if (linearRange[1] == 999)
                    txtPPGChannel2.Text = "";
                if (linearRange[2] == 999)
                    txtPPGChannel3.Text = "";
                if (linearRange[3] == 999)
                    txtPPGChannel4.Text = "";
                return;
            }

            string result = "";
            float range = linearRange.Max() - linearRange.Min();
            txtRange.Text = range.ToString();
            if (range <= MyCheckParamsConfig.LinearRange)
            {
                result = "OK";

                MyCheckParamsConfig.LinearOKNum++;
                if (MyCheckParamsConfig.LinearOKNum >= MyCheckParamsConfig.LinearNeedOK)
                {
                    MessageBox.Show("电芯线性点检通过！");
                    MyCheckParamsConfig.LinearCheckTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    MyCheckParamsConfig.LinearOKNum = 0;
                    MyCheckParamsConfig.LinearNGNum = 0;
                    MyCheckParamsConfig.Write();
                }
            }
            else
            {
                result = "NG";
                MyCheckParamsConfig.LinearNGNum++;
            }

            SaveResultToFile_Linear(range.ToString(), result);

            for (int i = 0; i < linearRange.Length; i++)
            {
                linearRange[i] = 999;
            }
            MyCheckParamsConfig.IsEnabled = false;
        }

        ///<summary>保存线性点检数据到Excel</summary>
        public void SaveResultToFile_Linear(string range, string result)
        {
            string filePath = "D:\\测量数据\\点检数据\\电芯线性点检\\" + ATL.Core.DeivceEquipmentidPlcInfo.lstDeivceEquipmentidPlcInfos[0].ModelInfo;
            string fileName = filePath + "\\PPG_" + DateTime.Now.ToString("yyyyMMdd") + ".csv";

            if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);

            try
            {
                string line = "";
                byte[] myByte = System.Text.Encoding.UTF8.GetBytes(line);

                if (!File.Exists(fileName))
                {
                    line = "时间,工位1,工位2,工位3,工位4,极差值,结果\r\n";
                    myByte = System.Text.Encoding.UTF8.GetBytes(line);
                    using (FileStream fsWrite = new FileStream(fileName, FileMode.Create))
                    {
                        byte[] bs = { (byte)0xEF, (byte)0xBB, (byte)0xBF };
                        fsWrite.Write(bs, 0, bs.Length);
                        fsWrite.Write(myByte, 0, myByte.Length);
                        fsWrite.Close();
                    };
                }

                string time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                line = time + "," + linearRange[0] + "," + linearRange[1] + "," + linearRange[2] + "," + linearRange[3] + "," + range + "," + result;
                line += "\r\n";

                myByte = System.Text.Encoding.UTF8.GetBytes(line);
                using (FileStream fsWrite = new FileStream(fileName, FileMode.Append))
                {
                    fsWrite.Write(myByte, 0, myByte.Length);
                    fsWrite.Close();
                };
            }
            catch (Exception ex)
            {
                MessageBox.Show("保存电芯线性点检数据失败: " + ex.Message);

                //写失败日志
                LoggingIF.Log("保存电芯线性点检数据失败:" + ex.Message);
                LoggingIF.Log("保存电芯线性点检数据失败:" + ex.StackTrace);
            }
        }
    }
}
