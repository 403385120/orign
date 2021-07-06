using System;
using System.Collections.Generic;
using ATL.UI.Controls;
using System.Windows.Markup;
using System.Linq;
using XRayClient.Core;
using Shuyz.Framework.Mvvm;
using System.Windows.Input;
using System.Threading;
using ZY.Logging;
using System.Windows;
using System.IO;
using ATL.Common;
using System.Data;
using System.Windows.Controls;
using ZY.BarCodeReader;
using ZYXray.Models;
using ZYXray.Utils;
using ZYXray.ViewModels;
using ZY.Systems;

namespace ZYXray
{
    /// <summary>
    /// DimensionCheckPage.xaml 的交互逻辑
    /// </summary>
    public partial class DimensionCheckPage : BasePage, IComponentConnector
    {
        private static ATL.Engine.PLC plc = new ATL.Engine.PLC();
        private static int measuretimes = 0;
        public static BaseFacade baseFacade = new BaseFacade();
        private string sendMsr = "Run_ACQA,\r";
        string mode = "";
        float _length = 0;
        float _width = 0;
        float _leftLug = 0;
        float _rightLug = 0;
        float _allLength = 0;
        float _left1Glue = 0;
        float _left2Glue = 0;
        float _right1Glue = 0;
        float _right2Glue = 0;
        float _leftLugLen = 0;
        float _rightLugLen = 0;
        string checkresult = "NG";
        bool stop = false;

        private CncButton MyCncButton
        {
            get { return CncButton.Instance; }
        }
        public CheckParamsConfig MyCheckParamsConfig
        {
            get { return CheckParamsConfig.Instance; }
        }
        public DimensionCheckPage()
        {
            InitializeComponent();
            DataContext = this;

            SelectCaliType_ComboBox();
        }

        private static List<DimensionCheckData> thickData = new List<DimensionCheckData>();

        public IEnumerable<ECalibrationBlockModel> BindableBlockModels
        {
            get
            {
                return Enum.GetValues(typeof(ECalibrationBlockModel))
                    .Cast<ECalibrationBlockModel>();
            }
        }
        public ECalibrationBlockModel BlockModel
        {
            get
            {
                return (ECalibrationBlockModel)CheckParamsConfig.Instance.DimensionCalibrationMode;
            }
            set
            {
                CheckParamsConfig.Instance.DimensionCalibrationMode = (int)value;
                if (CheckParamsConfig.Instance.DimensionCalibrationMode == 0)
                    sendMsr = "Run_ACQC,\r";
                else if (CheckParamsConfig.Instance.DimensionCalibrationMode == 1)
                    sendMsr = "Run_ACQB,\r";
                else
                    sendMsr = "Run_ACQA,\r";
            }
        }

        public List<DimensionCheckData> ThickData
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


        public ICommand StartCheck
        {
            get
            {
                return new RelayCommand(new Action(delegate
                {
                    if (CheckParamsConfig.Instance.DimensionCalibrationMode == 0)
                        sendMsr = "Run_ACQC,\r";
                    else if (CheckParamsConfig.Instance.DimensionCalibrationMode == 1)
                        sendMsr = "Run_ACQB,\r";
                    else
                        sendMsr = "Run_ACQA,\r";

                    if (MyCncButton.IsStartBtnEnable == false)
                    {
                        MessageBox.Show("请停止自动测试！");
                        return;
                    }
                    System.Windows.Forms.DialogResult dr1 = System.Windows.Forms.MessageBox.Show("是否开始尺寸点检？", "提示", System.Windows.Forms.MessageBoxButtons.YesNo);
                    if (dr1 != System.Windows.Forms.DialogResult.Yes)
                    {
                        return;
                    }
                    plc.WriteByVariableName("SoftwareOnState", 1);
                    plc.WriteByVariableName("SoftwareHeartbeatPackage", 1);
                    PromptWindow win = new PromptWindow();
                    win.Message = "请开启机器开始点检";
                    win.Topmost = true;
                    DoMeasure(win);
                    win.Closed += new EventHandler(PromptWindow_Closed);
                    //new Thread(() =>
                    //{

                    //}).Start();
                    //win.ShowDialog();
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

                //string status = "0";
                //int waitcount = 0;
                //while (status != "1" && status != "2")
                //{
                //    status = plc.ReadByVariableName("DimensionMeasureSignal");
                //    waitcount++;
                //    Thread.Sleep(100);
                //    if (waitcount > 50)
                //    {
                //        waitcount = 0;
                //        LoggingIF.Log("等待PLC触发尺寸测量信号超过5秒", LogLevels.Info, "DoMeasure");
                //    }
                //}
                //this.Dispatcher.BeginInvoke((Action)delegate ()
                //{
                //    pw.Message = "机器正在点检中，请耐心等候...";
                //});


                float min_length = 0;
                float max_length = 0;
                float min_width = 0;
                float max_width = 0;
                float min_leftLug = 0;
                float max_leftLug = 0;
                float min_rightLug = 0;
                float max_rightLug = 0;
                float min_allLength = 0;
                float max_allLength = 0;
                float min_left1Glue = 0;
                float max_left1Glue = 0;
                float min_left2Glue = 0;
                float max_left2Glue = 0;
                float min_right1Glue = 0;
                float max_right1Glue = 0;
                float min_right2Glue = 0;
                float max_right2Glue = 0;
                float min_leftLugLen = 0;
                float max_leftLugLen = 0;
                float min_rightLugLen = 0;
                float max_rightLugLen = 0;

                string DimensionStr = string.Empty;

                try
                {
                    CodeReaderIF.ClientSendMsg(sendMsr, 2);
                }
                catch (Exception ex)
                {
                    CodeReaderIF.Init_ClientConnet(HardwareConfig.Instance.DimensionIPAdress, HardwareConfig.Instance.DimensionPort, 2);//尺寸测量连接
                    CodeReaderIF.ClientSendMsg(sendMsr, 2);
                }
                //Thread.Sleep(800);
                //DimensionStr = CodeReaderIF.ClientReceiveMsg(2);
                Thread.Sleep(1500);
                DimensionStr = CodeReaderIF.ClientReceiveMsg(2);
                //DimensionStr.Replace("OKOK", "OK");
                //DimensionStr.Replace("OKNG", "OK");
                //DimensionStr.Replace("NGNG", "OK");
                if (DimensionStr.Contains("OK"))
                {
                    string[] str_result = DimensionStr.Substring(5).Split(',');
                    _length = float.Parse(str_result[0]);
                    _width = float.Parse(str_result[1]);
                    _leftLug = float.Parse(str_result[2]);
                    _rightLug = float.Parse(str_result[3]);
                    _allLength = float.Parse(str_result[4]);
                    _left1Glue = float.Parse(str_result[5]);
                    _left2Glue = float.Parse(str_result[6]);
                    _right1Glue = float.Parse(str_result[7]);
                    _right2Glue = float.Parse(str_result[8]);
                    _leftLugLen = float.Parse(str_result[9]);
                    _rightLugLen = float.Parse(str_result[10]);

                    string[] str_set = null;
                    if (CheckParamsConfig.Instance.DimensionCalibrationMode == 0)
                    {
                        mode = "小标块";
                        str_set = CheckParamsConfig.Instance.BatLengthS.Split('-');
                        min_length = float.Parse(str_set[0]);
                        max_length = float.Parse(str_set[1]);
                        str_set = CheckParamsConfig.Instance.BatWidthS.Split('-');
                        min_width = float.Parse(str_set[0]);
                        max_width = float.Parse(str_set[1]);
                        str_set = CheckParamsConfig.Instance.LeftLugS.Split('-');
                        min_leftLug = float.Parse(str_set[0]);
                        max_leftLug = float.Parse(str_set[1]);
                        str_set = CheckParamsConfig.Instance.RightLugS.Split('-');
                        min_rightLug = float.Parse(str_set[0]);
                        max_rightLug = float.Parse(str_set[1]);
                        str_set = CheckParamsConfig.Instance.AllLengthS.Split('-');
                        min_allLength = float.Parse(str_set[0]);
                        max_allLength = float.Parse(str_set[1]);
                        str_set = CheckParamsConfig.Instance.Left1GlueS.Split('-');
                        min_left1Glue = float.Parse(str_set[0]);
                        max_left1Glue = float.Parse(str_set[1]);
                        str_set = CheckParamsConfig.Instance.Left2GlueS.Split('-');
                        min_left2Glue = float.Parse(str_set[0]);
                        max_left2Glue = float.Parse(str_set[1]);
                        str_set = CheckParamsConfig.Instance.Right1GlueS.Split('-');
                        min_right1Glue = float.Parse(str_set[0]);
                        max_right1Glue = float.Parse(str_set[1]);
                        str_set = CheckParamsConfig.Instance.Right2GlueS.Split('-');
                        min_right2Glue = float.Parse(str_set[0]);
                        max_right2Glue = float.Parse(str_set[1]);
                        str_set = CheckParamsConfig.Instance.LeftLugLenS.Split('-');
                        min_leftLugLen = float.Parse(str_set[0]);
                        max_leftLugLen = float.Parse(str_set[1]);
                        str_set = CheckParamsConfig.Instance.RightLugLenS.Split('-');
                        min_rightLugLen = float.Parse(str_set[0]);
                        max_rightLugLen = float.Parse(str_set[1]);

                    }
                    else if (CheckParamsConfig.Instance.DimensionCalibrationMode == 1)
                    {
                        mode = "中标块";
                        str_set = CheckParamsConfig.Instance.BatLengthM.Split('-');
                        min_length = float.Parse(str_set[0]);
                        max_length = float.Parse(str_set[1]);
                        str_set = CheckParamsConfig.Instance.BatWidthM.Split('-');
                        min_width = float.Parse(str_set[0]);
                        max_width = float.Parse(str_set[1]);
                        str_set = CheckParamsConfig.Instance.LeftLugM.Split('-');
                        min_leftLug = float.Parse(str_set[0]);
                        max_leftLug = float.Parse(str_set[1]);
                        str_set = CheckParamsConfig.Instance.RightLugM.Split('-');
                        min_rightLug = float.Parse(str_set[0]);
                        max_rightLug = float.Parse(str_set[1]);
                        str_set = CheckParamsConfig.Instance.AllLengthM.Split('-');
                        min_allLength = float.Parse(str_set[0]);
                        max_allLength = float.Parse(str_set[1]);
                        str_set = CheckParamsConfig.Instance.Left1GlueM.Split('-');
                        min_left1Glue = float.Parse(str_set[0]);
                        max_left1Glue = float.Parse(str_set[1]);
                        str_set = CheckParamsConfig.Instance.Left2GlueM.Split('-');
                        min_left2Glue = float.Parse(str_set[0]);
                        max_left2Glue = float.Parse(str_set[1]);
                        str_set = CheckParamsConfig.Instance.Right1GlueM.Split('-');
                        min_right1Glue = float.Parse(str_set[0]);
                        max_right1Glue = float.Parse(str_set[1]);
                        str_set = CheckParamsConfig.Instance.Right2GlueM.Split('-');
                        min_right2Glue = float.Parse(str_set[0]);
                        max_right2Glue = float.Parse(str_set[1]);
                        str_set = CheckParamsConfig.Instance.LeftLugLenM.Split('-');
                        min_leftLugLen = float.Parse(str_set[0]);
                        max_leftLugLen = float.Parse(str_set[1]);
                        str_set = CheckParamsConfig.Instance.RightLugLenM.Split('-');
                        min_rightLugLen = float.Parse(str_set[0]);
                        max_rightLugLen = float.Parse(str_set[1]);
                    }
                    else
                    {
                        mode = "大标块";
                        str_set = CheckParamsConfig.Instance.BatLengthB.Split('-');
                        min_length = float.Parse(str_set[0]);
                        max_length = float.Parse(str_set[1]);
                        str_set = CheckParamsConfig.Instance.BatWidthB.Split('-');
                        min_width = float.Parse(str_set[0]);
                        max_width = float.Parse(str_set[1]);
                        str_set = CheckParamsConfig.Instance.LeftLugB.Split('-');
                        min_leftLug = float.Parse(str_set[0]);
                        max_leftLug = float.Parse(str_set[1]);
                        str_set = CheckParamsConfig.Instance.RightLugB.Split('-');
                        min_rightLug = float.Parse(str_set[0]);
                        max_rightLug = float.Parse(str_set[1]);
                        str_set = CheckParamsConfig.Instance.AllLengthB.Split('-');
                        min_allLength = float.Parse(str_set[0]);
                        max_allLength = float.Parse(str_set[1]);
                        str_set = CheckParamsConfig.Instance.Left1GlueB.Split('-');
                        min_left1Glue = float.Parse(str_set[0]);
                        max_left1Glue = float.Parse(str_set[1]);
                        str_set = CheckParamsConfig.Instance.Left2GlueB.Split('-');
                        min_left2Glue = float.Parse(str_set[0]);
                        max_left2Glue = float.Parse(str_set[1]);
                        str_set = CheckParamsConfig.Instance.Right1GlueB.Split('-');
                        min_right1Glue = float.Parse(str_set[0]);
                        max_right1Glue = float.Parse(str_set[1]);
                        str_set = CheckParamsConfig.Instance.Right2GlueB.Split('-');
                        min_right2Glue = float.Parse(str_set[0]);
                        max_right2Glue = float.Parse(str_set[1]);
                        str_set = CheckParamsConfig.Instance.LeftLugLenB.Split('-');
                        min_leftLugLen = float.Parse(str_set[0]);
                        max_leftLugLen = float.Parse(str_set[1]);
                        str_set = CheckParamsConfig.Instance.RightLugLenB.Split('-');
                        min_rightLugLen = float.Parse(str_set[0]);
                        max_rightLugLen = float.Parse(str_set[1]);
                    }

                    if (_length >= min_length
                    && _length <= max_length
                    && _width >= min_width
                    && _width <= max_width
                    && _leftLug >= min_leftLug
                    && _leftLug <= max_leftLug
                    && _rightLug >= min_rightLug
                    && _rightLug <= max_rightLug
                    && _allLength >= min_allLength
                    && _allLength <= max_allLength
                    && _left1Glue >= min_left1Glue
                    && _left1Glue <= max_left1Glue
                    && _left2Glue >= min_left2Glue
                    && _left2Glue <= max_left2Glue
                    && _right1Glue >= min_right1Glue
                    && _right1Glue <= max_right1Glue
                    && _right2Glue >= min_right2Glue
                    && _right2Glue <= max_right2Glue
                    && _leftLugLen >= min_leftLugLen
                    && _leftLugLen <= max_leftLugLen
                    && _rightLugLen >= min_rightLugLen
                    && _rightLugLen <= max_rightLugLen)
                    {
                        checkresult = "OK";
                    }
                    else
                    {
                        checkresult = "NG";
                    }

                }
                else
                {
                    LoggingIF.Log(string.Format("解析字串失败: {0}", DimensionStr), LogLevels.Info, "DoMeasure");
                }

                thickData.Add(new DimensionCheckData
                {
                    index = measuretimes,
                    time = DateTime.Now,
                    model = mode,
                    batLength = _length,
                    batWidth = _width,
                    leftLug = _leftLug,
                    rightLug = _rightLug,
                    allBatLength = _allLength,
                    left1WhiteGlue = _left1Glue,
                    left2WhiteGlue = _left2Glue,
                    right1WhiteGlue = _right1Glue,
                    right2WhiteGlue = _right2Glue,
                    leftLugLen = _leftLugLen,
                    rightLugLen = _rightLugLen,
                    result = checkresult
                });
                SaveResultToFile_Dimension(measuretimes, mode);
                if (i == 2)
                {
                    if (MotionControlVm.Instance.CheckMDI())
                    {
                        CheckParamsConfig.Instance.MdiCheckTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        CheckParamsSettingsVm.Instance.MyCheckParamsConfig.Write();
                        MessageBox.Show("尺寸点检通过");
                    }
                }

                //plc.WriteByVariableName("BatteryDimensionMeasureComplete", 1);

                //status = "1";
                //waitcount = 0;
                //while (status != "0")
                //{
                //    status = plc.ReadByVariableName("DimensionMeasureSignal");
                //    waitcount++;
                //    Thread.Sleep(100);
                //    if (waitcount > 50)
                //    {
                //        waitcount = 0;
                //        LoggingIF.Log("等待PLC关闭测厚触发信号超过5秒", LogLevels.Info, "DoMeasure");
                //    }
                //}
                //plc.WriteByVariableName("BatteryDimensionMeasureComplete", 0);

            }

            this.Dispatcher.BeginInvoke((Action)delegate ()
            {
                datalist.ItemsSource = null;
                datalist.ItemsSource = ThickData;
            });

            //this.Dispatcher.BeginInvoke((Action)delegate ()
            //{
            //    pw.Close();
            //});
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            plc.WriteByVariableName("ThicknessMeasureSignal", 2);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            plc.WriteByVariableName("ThicknessMeasureSignal", 0);
        }

        public void SaveResultToFile_Dimension(int num, string model)
        {
            string filePath = "D:\\测量数据\\点检数据\\尺寸测量标块点检\\" + ATL.Core.DeivceEquipmentidPlcInfo.lstDeivceEquipmentidPlcInfos[0].ModelInfo;
            string fileName = filePath + "\\FQI_" + DateTime.Now.ToString("yyyyMMdd") + ".csv";

            if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);

            string line = "";
            byte[] myByte = System.Text.Encoding.UTF8.GetBytes(line);

            if (!File.Exists(fileName))
            {
                line = "编号,点检时间,标定块,电池长度,电池宽度,左极耳边距,右极耳边距,电池总长度,左1小白胶,左2小白胶,右1小白胶,右2小白胶,左极耳长,右极耳长,结果\r\n";
                myByte = System.Text.Encoding.UTF8.GetBytes(line);
                using (FileStream fsWrite = new FileStream(fileName, FileMode.Create))
                {
                    byte[] bs = { (byte)0xEF, (byte)0xBB, (byte)0xBF };
                    fsWrite.Write(bs, 0, bs.Length);
                    fsWrite.Write(myByte, 0, myByte.Length);
                };
            }

            string time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            line = num + "," + time + "," + model + "," + _length + "," + _width + "," + _leftLug + "," + _rightLug + "," + _allLength + "," + _left1Glue + "," + _left2Glue + "," + _right1Glue + "," + _right2Glue + "," + _leftLugLen + "," + _rightLugLen + "," + checkresult;
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
                        if (!(item as TextBox).Text.Contains('-') && (item as TextBox).Name != "textBox_type")
                        {
                            MessageBox.Show("最小值与最大值之间请用 ‘ - ’ 隔开！");
                            return;
                        }
                    }
                }
                string sql = "select t.cali_type from cali_block_data_dimension t where t.cali_type='" + textBox_type.Text + "'";
                //sql = "select t.cali_type from cali_block_data t where t.cali_type=''";
                DataSet ds = baseFacade.equipDB.ExecuteDataSet(CommandType.Text, sql);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    sql = "update cali_block_data_dimension t set t.s_length='" + textBox_s_length.Text + "',t.s_width='" + textBox_s_width.Text + "',"
                        + "t.s_left_lug='" + textBox_s_left_lug.Text + "',t.s_right_lug='" + textBox_s_right_lug.Text + "',t.s_all_length='" + textBox_s_all.Text + "',"
                        + "t.s_left_1='" + textBox_s_left1.Text + "',t.s_left_2='" + textBox_s_left2.Text + "',t.s_right_1='" + textBox_s_right1.Text + "',t.s_right_2='" + textBox_s_right2.Text + "',"
                        + "t.s_left_lug_len='" + textBox_s_left_lug_len.Text + "',t.s_right_lug_len='" + textBox_s_right_lug_len.Text + "',"
                        + "t.m_length='" + textBox_m_length.Text + "',t.m_width='" + textBox_m_width.Text + "',"
                        + "t.m_left_lug='" + textBox_m_left_lug.Text + "',t.m_right_lug='" + textBox_m_right_lug.Text + "',t.m_all_length='" + textBox_m_all.Text + "',"
                        + "t.m_left_1='" + textBox_m_left1.Text + "',t.m_left_2='" + textBox_m_left2.Text + "',t.m_right_1='" + textBox_m_right1.Text + "',t.m_right_2='" + textBox_m_right2.Text + "',"
                        + "t.m_left_lug_len='" + textBox_m_left_lug_len.Text + "',t.m_right_lug_len='" + textBox_m_right_lug_len.Text + "',"
                        + "t.b_length='" + textBox_b_length.Text + "',t.b_width='" + textBox_b_width.Text + "',"
                        + "t.b_left_lug='" + textBox_b_left_lug.Text + "',t.b_right_lug='" + textBox_b_right_lug.Text + "',t.b_all_length='" + textBox_b_all.Text + "',"
                        + "t.b_left_1='" + textBox_b_left1.Text + "',t.b_left_2='" + textBox_b_left2.Text + "',t.b_right_1='" + textBox_b_right1.Text + "',t.b_right_2='" + textBox_b_right2.Text + "',"
                        + "t.b_left_lug_len='" + textBox_b_left_lug_len.Text + "',t.b_right_lug_len='" + textBox_b_right_lug_len.Text + "'"
                        + " where t.cali_type='" + textBox_type.Text + "' ";
                    baseFacade.equipDB.ExecuteNonQuery(CommandType.Text, sql);
                }
                else
                {
                    sql = "insert into cali_block_data_dimension  (cali_type,s_length,s_width,s_left_lug,s_right_lug,s_all_length,s_left_1,s_left_2,s_right_1,s_right_2,s_left_lug_len,s_right_lug_len," +
                        "m_length,m_width,m_left_lug,m_right_lug,m_all_length,m_left_1,m_left_2,m_right_1,m_right_2,m_left_lug_len,m_right_lug_len," +
                        "b_length,b_width,b_left_lug,b_right_lug,b_all_length,b_left_1,b_left_2,b_right_1,b_right_2,b_left_lug_len,b_right_lug_len)" +
                        "values ('" + textBox_type.Text + "','" + textBox_s_length.Text + "','" + textBox_s_width.Text + "','" + textBox_s_left_lug.Text + "','" + textBox_s_right_lug.Text + "'," +
                        "'" + textBox_s_all.Text + "','" + textBox_s_left1.Text + "','" + textBox_s_left2.Text + "','" + textBox_s_right1.Text + "','" + textBox_s_right2.Text + "'," +
                        "'" + textBox_s_left_lug_len.Text + "','" + textBox_s_right_lug_len.Text + "'," +
                        "'" + textBox_m_length.Text + "','" + textBox_m_width.Text + "','" + textBox_m_left_lug.Text + "','" + textBox_m_right_lug.Text + "'," +
                        "'" + textBox_m_all.Text + "','" + textBox_m_left1.Text + "','" + textBox_m_left2.Text + "','" + textBox_m_right1.Text + "','" + textBox_m_right2.Text + "'," +
                        "'" + textBox_m_left_lug_len.Text + "','" + textBox_m_right_lug_len.Text + "'," +
                        "'" + textBox_b_length.Text + "','" + textBox_b_width.Text + "','" + textBox_b_left_lug.Text + "','" + textBox_b_right_lug.Text + "'," +
                        "'" + textBox_b_all.Text + "','" + textBox_b_left1.Text + "','" + textBox_b_left2.Text + "','" + textBox_b_right1.Text + "','" + textBox_b_right2.Text + "'," +
                        "'" + textBox_b_left_lug_len.Text + "','" + textBox_b_right_lug_len.Text + "')";
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
            MyCheckParamsConfig.IsEnabled = false;
        }

        /// <summary>
        /// 调用
        /// </summary>
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            if (comboBox.Text != "")
            {
                string sql = "select * from cali_block_data_dimension t where t.cali_type='" + comboBox.Text + "'";
                DataSet ds = baseFacade.equipDB.ExecuteDataSet(CommandType.Text, sql);
                string s = ds.Tables[0].Rows[0]["cali_type"].ToString();
                textBox_type.Text = MyCheckParamsConfig.CurrentModel = s;

                textBox_s_length.Text = MyCheckParamsConfig.BatLengthS = ds.Tables[0].Rows[0]["s_length"].ToString();
                textBox_s_width.Text = MyCheckParamsConfig.BatWidthS = ds.Tables[0].Rows[0]["s_width"].ToString();
                textBox_s_left_lug.Text = MyCheckParamsConfig.LeftLugS = ds.Tables[0].Rows[0]["s_left_lug"].ToString();
                textBox_s_right_lug.Text = MyCheckParamsConfig.RightLugS = ds.Tables[0].Rows[0]["s_right_lug"].ToString();
                textBox_s_all.Text = MyCheckParamsConfig.AllLengthS = ds.Tables[0].Rows[0]["s_all_length"].ToString();
                textBox_s_left1.Text = MyCheckParamsConfig.Left1GlueS = ds.Tables[0].Rows[0]["s_left_1"].ToString();
                textBox_s_left2.Text = MyCheckParamsConfig.Left2GlueS = ds.Tables[0].Rows[0]["s_left_2"].ToString();
                textBox_s_right1.Text = MyCheckParamsConfig.Right1GlueS = ds.Tables[0].Rows[0]["s_right_1"].ToString();
                textBox_s_right2.Text = MyCheckParamsConfig.Right2GlueS = ds.Tables[0].Rows[0]["s_right_2"].ToString();
                textBox_s_left_lug_len.Text = MyCheckParamsConfig.LeftLugLenS = ds.Tables[0].Rows[0]["s_left_lug_len"].ToString();
                textBox_s_right_lug_len.Text = MyCheckParamsConfig.RightLugLenS = ds.Tables[0].Rows[0]["s_right_lug_len"].ToString();

                textBox_m_length.Text = MyCheckParamsConfig.BatLengthM = ds.Tables[0].Rows[0]["m_length"].ToString();
                textBox_m_width.Text = MyCheckParamsConfig.BatWidthM = ds.Tables[0].Rows[0]["m_width"].ToString();
                textBox_m_left_lug.Text = MyCheckParamsConfig.LeftLugM = ds.Tables[0].Rows[0]["m_left_lug"].ToString();
                textBox_m_right_lug.Text = MyCheckParamsConfig.RightLugM = ds.Tables[0].Rows[0]["m_right_lug"].ToString();
                textBox_m_all.Text = MyCheckParamsConfig.AllLengthM = ds.Tables[0].Rows[0]["m_all_length"].ToString();
                textBox_m_left1.Text = MyCheckParamsConfig.Left1GlueM = ds.Tables[0].Rows[0]["m_left_1"].ToString();
                textBox_m_left2.Text = MyCheckParamsConfig.Left2GlueM = ds.Tables[0].Rows[0]["m_left_2"].ToString();
                textBox_m_right1.Text = MyCheckParamsConfig.Right1GlueM = ds.Tables[0].Rows[0]["m_right_1"].ToString();
                textBox_m_right2.Text = MyCheckParamsConfig.Right2GlueM = ds.Tables[0].Rows[0]["m_right_2"].ToString();
                textBox_m_left_lug_len.Text = MyCheckParamsConfig.LeftLugLenM = ds.Tables[0].Rows[0]["m_left_lug_len"].ToString();
                textBox_m_right_lug_len.Text = MyCheckParamsConfig.RightLugLenM = ds.Tables[0].Rows[0]["m_right_lug_len"].ToString();

                textBox_b_length.Text = MyCheckParamsConfig.BatLengthB = ds.Tables[0].Rows[0]["b_length"].ToString();
                textBox_b_width.Text = MyCheckParamsConfig.BatWidthB = ds.Tables[0].Rows[0]["b_width"].ToString();
                textBox_b_left_lug.Text = MyCheckParamsConfig.LeftLugB = ds.Tables[0].Rows[0]["b_left_lug"].ToString();
                textBox_b_right_lug.Text = MyCheckParamsConfig.RightLugB = ds.Tables[0].Rows[0]["b_right_lug"].ToString();
                textBox_b_all.Text = MyCheckParamsConfig.AllLengthB = ds.Tables[0].Rows[0]["b_all_length"].ToString();
                textBox_b_left1.Text = MyCheckParamsConfig.Left1GlueB = ds.Tables[0].Rows[0]["b_left_1"].ToString();
                textBox_b_left2.Text = MyCheckParamsConfig.Left2GlueB = ds.Tables[0].Rows[0]["b_left_2"].ToString();
                textBox_b_right1.Text = MyCheckParamsConfig.Right1GlueB = ds.Tables[0].Rows[0]["b_right_1"].ToString();
                textBox_b_right2.Text = MyCheckParamsConfig.Right2GlueB = ds.Tables[0].Rows[0]["b_right_2"].ToString();
                textBox_b_left_lug_len.Text = MyCheckParamsConfig.LeftLugLenB = ds.Tables[0].Rows[0]["b_left_lug_len"].ToString();
                textBox_b_right_lug_len.Text = MyCheckParamsConfig.RightLugLenB = ds.Tables[0].Rows[0]["b_right_lug_len"].ToString();

                MyCheckParamsConfig.WriteDimensionParams();
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
                string sql = "delete from cali_block_data_dimension  where cali_type='" + comboBox.Text + "'";
                baseFacade.equipDB.ExecuteNonQuery(CommandType.Text, sql);
                SelectCaliType_ComboBox();

                if (model == MyCheckParamsConfig.CurrentModel)
                {
                    MyCheckParamsConfig.CurrentModel = "";
                    MyCheckParamsConfig.BatLengthS = "";
                    MyCheckParamsConfig.BatWidthS = "";
                    MyCheckParamsConfig.LeftLugS = "";
                    MyCheckParamsConfig.RightLugS = "";
                    MyCheckParamsConfig.AllLengthS = "";
                    MyCheckParamsConfig.Left1GlueS = "";
                    MyCheckParamsConfig.Left2GlueS = "";
                    MyCheckParamsConfig.Right1GlueS = "";
                    MyCheckParamsConfig.Right2GlueS = "";
                    MyCheckParamsConfig.LeftLugLenS = "";
                    MyCheckParamsConfig.RightLugLenS = "";

                    MyCheckParamsConfig.BatLengthM = "";
                    MyCheckParamsConfig.BatWidthM = "";
                    MyCheckParamsConfig.LeftLugM = "";
                    MyCheckParamsConfig.RightLugM = "";
                    MyCheckParamsConfig.AllLengthM = "";
                    MyCheckParamsConfig.Left1GlueM = "";
                    MyCheckParamsConfig.Left2GlueM = "";
                    MyCheckParamsConfig.Right1GlueM = "";
                    MyCheckParamsConfig.Right2GlueM = "";
                    MyCheckParamsConfig.LeftLugLenM = "";
                    MyCheckParamsConfig.RightLugLenM = "";

                    MyCheckParamsConfig.BatLengthB = "";
                    MyCheckParamsConfig.BatWidthB = "";
                    MyCheckParamsConfig.LeftLugB = "";
                    MyCheckParamsConfig.RightLugB = "";
                    MyCheckParamsConfig.AllLengthB = "";
                    MyCheckParamsConfig.Left1GlueB = "";
                    MyCheckParamsConfig.Left2GlueB = "";
                    MyCheckParamsConfig.Right1GlueB = "";
                    MyCheckParamsConfig.Right2GlueB = "";
                    MyCheckParamsConfig.LeftLugLenB = "";
                    MyCheckParamsConfig.RightLugLenB = "";
                    MyCheckParamsConfig.WriteDimensionParams();
                }
            }
        }
        /// <summary>
        /// 标块类型绑定ComboBox
        /// </summary>
        private void SelectCaliType_ComboBox()
        {
            string sql = "select t.ID,t.cali_type from cali_block_data_dimension t";
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


        private void button1_Click_2(object sender, RoutedEventArgs e)
        {
            #region +-0.02


            double updateData = Common.FQIRange;
            // 小标快
            if (textBox_s_left_lug_len.Text == "0" || textBox_s_left_lug_len.Text == "999" || textBox_s_left_lug_len.Text == "")
            {
                textBox_s_left_lug_len.Text = "0-999";//左极耳长
            }
            else
            {
                double data = 0;
                if (double.TryParse(textBox_s_left_lug_len.Text.Trim(), out data))
                {
                    double data1 = data - updateData;
                    double data2 = data + updateData;
                    MyCheckParamsConfig.AllLengthS = textBox_s_left_lug_len.Text = data1 + "-" + data2;
                }
            }


            if (textBox_s_right_lug_len.Text == "0" || textBox_s_right_lug_len.Text == "999" || textBox_s_right_lug_len.Text == "")
            {
                textBox_s_right_lug_len.Text = "0-999";//右极耳长
            }
            else
            {
                double data = 0;
                if (double.TryParse(textBox_s_right_lug_len.Text.Trim(), out data))
                {
                    double data1 = data - updateData;
                    double data2 = data + updateData;
                    MyCheckParamsConfig.RightLugLenS = textBox_s_right_lug_len.Text = data1 + "-" + data2;
                }
            }

            if (textBox_s_length.Text == "0" || textBox_s_length.Text == "999" || textBox_s_length.Text == "")
            {
                textBox_s_length.Text = "0-999";//主体长度
            }
            else
            {
                double data = 0;
                if (double.TryParse(textBox_s_length.Text.Trim(), out data))
                {
                    double data1 = data - updateData;
                    double data2 = data + updateData;
                    MyCheckParamsConfig.BatLengthS = textBox_s_length.Text = data1 + "-" + data2;
                }
            }
            if (textBox_s_width.Text == "0" || textBox_s_width.Text == "999" || textBox_s_width.Text == "")
            {
                textBox_s_width.Text = "0-999";//主体宽度
            }
            else
            {
                double data = 0;
                if (double.TryParse(textBox_s_width.Text.Trim(), out data))
                {
                    double data1 = data - updateData;
                    double data2 = data + updateData;
                    MyCheckParamsConfig.BatWidthS = textBox_s_width.Text = data1 + "-" + data2;
                }
            }
            if (textBox_s_all.Text == "0" || textBox_s_all.Text == "999" || textBox_s_all.Text == "")
            {
                textBox_s_all.Text = "0-999";//总长度
            }
            else
            {
                double data = 0;
                if (double.TryParse(textBox_s_all.Text.Trim(), out data))
                {
                    double data1 = data - updateData;
                    double data2 = data + updateData;
                    MyCheckParamsConfig.AllLengthS = textBox_s_all.Text = data1 + "-" + data2;
                }
            }
            if (textBox_s_left_lug.Text == "0" || textBox_s_left_lug.Text == "999" || textBox_s_left_lug.Text == "")
            {
                textBox_s_left_lug.Text = "0-999";//左极耳边距
            }
            else
            {
                double data = 0;
                if (double.TryParse(textBox_s_left_lug.Text.Trim(), out data))
                {
                    double data1 = data - updateData;
                    double data2 = data + updateData;
                    MyCheckParamsConfig.LeftLugS = textBox_s_left_lug.Text = data1 + "-" + data2;
                }
            }
            if (textBox_s_right_lug.Text == "0" || textBox_s_right_lug.Text == "999" || textBox_s_right_lug.Text == "")
            {
                textBox_s_right_lug.Text = "0-999";//右极耳边距
            }
            else
            {
                double data = 0;
                if (double.TryParse(textBox_s_right_lug.Text.Trim(), out data))
                {
                    double data1 = data - updateData;
                    double data2 = data + updateData;
                    MyCheckParamsConfig.RightLugS = textBox_s_right_lug.Text = data1 + "-" + data2;
                }
            }
            if (textBox_s_left1.Text == "0" || textBox_s_left1.Text == "999" || textBox_s_left1.Text == "")
            {
                textBox_s_left1.Text = "0-999";//左1小白胶
            }
            else
            {
                double data = 0;
                if (double.TryParse(textBox_s_left1.Text.Trim(), out data))
                {
                    double data1 = data - updateData;
                    double data2 = data + updateData;
                    MyCheckParamsConfig.Left1GlueS = textBox_s_left1.Text = data1 + "-" + data2;
                }
            }
            if (textBox_s_left2.Text == "0" || textBox_s_left2.Text == "999" || textBox_s_left2.Text == "")
            {
                textBox_s_left2.Text = "0-999";//左2小白胶
            }
            else
            {
                double data = 0;
                if (double.TryParse(textBox_s_left2.Text.Trim(), out data))
                {
                    double data1 = data - updateData;
                    double data2 = data + updateData;
                    MyCheckParamsConfig.Left2GlueS = textBox_s_left2.Text = data1 + "-" + data2;
                }
            }
            if (textBox_s_right1.Text == "0" || textBox_s_right1.Text == "999" || textBox_s_right1.Text == "")
            {
                textBox_s_right1.Text = "0-999";//右1小白胶
            }
            else
            {
                double data = 0;
                if (double.TryParse(textBox_s_right1.Text.Trim(), out data))
                {
                    double data1 = data - updateData;
                    double data2 = data + updateData;
                    MyCheckParamsConfig.Right1GlueS = textBox_s_right1.Text = data1 + "-" + data2;
                }
            }
            if (textBox_s_right2.Text == "0" || textBox_s_right2.Text == "999" || textBox_s_right2.Text == "")
            {
                textBox_s_right2.Text = "0-999";//右2小白胶
            }
            else
            {
                double data = 0;
                if (double.TryParse(textBox_s_right2.Text.Trim(), out data))
                {
                    double data1 = data - updateData;
                    double data2 = data + updateData;
                    MyCheckParamsConfig.Right2GlueS = textBox_s_right2.Text = data1 + "-" + data2;
                }
            }

            //中标块
            if (textBox_m_left_lug_len.Text == "0" || textBox_m_left_lug_len.Text == "999" || textBox_m_left_lug_len.Text == "")
            {
                textBox_m_left_lug_len.Text = "0-999";//左极耳长
            }
            else
            {
                double data = 0;
                if (double.TryParse(textBox_m_left_lug_len.Text.Trim(), out data))
                {
                    double data1 = data - updateData;
                    double data2 = data + updateData;
                    MyCheckParamsConfig.LeftLugLenM = textBox_m_left_lug_len.Text = data1 + "-" + data2;
                }
            }
            if (textBox_m_right_lug_len.Text == "0" || textBox_m_right_lug_len.Text == "999" || textBox_m_right_lug_len.Text == "")
            {
                textBox_m_right_lug_len.Text = "0-999";//右极耳长
            }
            else
            {
                double data = 0;
                if (double.TryParse(textBox_m_right_lug_len.Text.Trim(), out data))
                {
                    double data1 = data - updateData;
                    double data2 = data + updateData;
                    MyCheckParamsConfig.RightLugLenM = textBox_m_right_lug_len.Text = data1 + "-" + data2;
                }
            }
            if (textBox_m_length.Text == "0" || textBox_m_length.Text == "999" || textBox_m_length.Text == "")
            {
                textBox_m_length.Text = "0-999";//主体长
            }
            else
            {
                double data = 0;
                if (double.TryParse(textBox_m_length.Text.Trim(), out data))
                {
                    double data1 = data - updateData;
                    double data2 = data + updateData;
                    MyCheckParamsConfig.BatLengthM = textBox_m_length.Text = data1 + "-" + data2;
                }
            }
            if (textBox_m_width.Text == "0" || textBox_m_width.Text == "999" || textBox_m_width.Text == "")
            {
                textBox_m_width.Text = "0-999";//主体宽
            }
            else
            {
                double data = 0;
                if (double.TryParse(textBox_m_width.Text.Trim(), out data))
                {
                    double data1 = data - updateData;
                    double data2 = data + updateData;
                    MyCheckParamsConfig.BatWidthM = textBox_m_width.Text = data1 + "-" + data2;
                }
            }
            if (textBox_m_all.Text == "0" || textBox_m_all.Text == "999" || textBox_m_all.Text == "")
            {
                textBox_m_all.Text = "0-999";//总长度
            }
            else
            {
                double data = 0;
                if (double.TryParse(textBox_m_all.Text.Trim(), out data))
                {
                    double data1 = data - updateData;
                    double data2 = data + updateData;
                    MyCheckParamsConfig.AllLengthM = textBox_m_all.Text = data1 + "-" + data2;
                }
            }
            if (textBox_m_left_lug.Text == "0" || textBox_m_left_lug.Text == "999" || textBox_m_left_lug.Text == "")
            {
                textBox_m_left_lug.Text = "0-999";//左极耳边距
            }
            else
            {
                double data = 0;
                if (double.TryParse(textBox_m_left_lug.Text.Trim(), out data))
                {
                    double data1 = data - updateData;
                    double data2 = data + updateData;
                    MyCheckParamsConfig.LeftLugM = textBox_m_left_lug.Text = data1 + "-" + data2;
                }
            }
            if (textBox_m_right_lug.Text == "0" || textBox_m_right_lug.Text == "999" || textBox_m_right_lug.Text == "")
            {
                textBox_m_right_lug.Text = "0-999";//右极耳边距
            }
            else
            {
                double data = 0;
                if (double.TryParse(textBox_m_right_lug.Text.Trim(), out data))
                {
                    double data1 = data - updateData;
                    double data2 = data + updateData;
                    MyCheckParamsConfig.RightLugM = textBox_m_right_lug.Text = data1 + "-" + data2;
                }
            }
            if (textBox_m_left1.Text == "0" || textBox_m_left1.Text == "999" || textBox_m_left1.Text == "")
            {
                textBox_m_left1.Text = "0-999";//左1小白胶
            }
            else
            {
                double data = 0;
                if (double.TryParse(textBox_m_left1.Text.Trim(), out data))
                {
                    double data1 = data - updateData;
                    double data2 = data + updateData;
                    MyCheckParamsConfig.Left1GlueM = textBox_m_left1.Text = data1 + "-" + data2;
                }
            }
            if (textBox_m_left2.Text == "0" || textBox_m_left2.Text == "999" || textBox_m_left2.Text == "")
            {
                textBox_m_left2.Text = "0-999";//左2小白胶
            }
            else
            {
                double data = 0;
                if (double.TryParse(textBox_m_left1.Text.Trim(), out data))
                {
                    double data1 = data - updateData;
                    double data2 = data + updateData;
                    MyCheckParamsConfig.Left2GlueM = textBox_m_left1.Text = data1 + "-" + data2;
                }
            }
            if (textBox_m_right1.Text == "0" || textBox_m_right1.Text == "999" || textBox_m_right1.Text == "")
            {
                textBox_m_right1.Text = "0-999";//右1小白胶
            }
            else
            {
                double data = 0;
                if (double.TryParse(textBox_m_right1.Text.Trim(), out data))
                {
                    double data1 = data - updateData;
                    double data2 = data + updateData;
                    MyCheckParamsConfig.Right1GlueM = textBox_m_right1.Text = data1 + "-" + data2;
                }
            }
            if (textBox_m_right2.Text == "0" || textBox_m_right2.Text == "999" || textBox_m_right2.Text == "")
            {
                textBox_m_right2.Text = "0-999";//右2小白胶
            }
            else
            {
                double data = 0;
                if (double.TryParse(textBox_m_right2.Text.Trim(), out data))
                {
                    double data1 = data - updateData;
                    double data2 = data + updateData;
                    MyCheckParamsConfig.Right2GlueM = textBox_m_right2.Text = data1 + "-" + data2;
                }
            }

            //大标快
            if (textBox_b_left_lug_len.Text == "0" || textBox_b_left_lug_len.Text == "999" || textBox_b_left_lug_len.Text == "")
            {
                textBox_b_left_lug_len.Text = "0-999";//左极耳长
            }
            else
            {
                double data = 0;
                if (double.TryParse(textBox_b_left_lug_len.Text.Trim(), out data))
                {
                    double data1 = data - updateData;
                    double data2 = data + updateData;
                    MyCheckParamsConfig.LeftLugLenB = textBox_b_left_lug_len.Text = data1 + "-" + data2;
                }
            }
            if (textBox_b_right_lug_len.Text == "0" || textBox_b_right_lug_len.Text == "999" || textBox_b_right_lug_len.Text == "")
            {
                textBox_b_right_lug_len.Text = "0-999";//右极耳长
            }
            else
            {
                double data = 0;
                if (double.TryParse(textBox_b_right_lug_len.Text.Trim(), out data))
                {
                    double data1 = data - updateData;
                    double data2 = data + updateData;
                    MyCheckParamsConfig.RightLugLenB = textBox_b_right_lug_len.Text = data1 + "-" + data2;
                }
            }
            if (textBox_b_length.Text == "0" || textBox_b_length.Text == "999" || textBox_b_length.Text == "")
            {
                textBox_b_length.Text = "0-999";//主体长
            }
            else
            {
                double data = 0;
                if (double.TryParse(textBox_b_length.Text.Trim(), out data))
                {
                    double data1 = data - updateData;
                    double data2 = data + updateData;
                    MyCheckParamsConfig.BatLengthB = textBox_b_length.Text = data1 + "-" + data2;
                }
            }
            if (textBox_b_width.Text == "0" || textBox_b_width.Text == "999" || textBox_b_width.Text == "")
            {
                textBox_b_width.Text = "0-999";//主体宽
            }
            else
            {
                double data = 0;
                if (double.TryParse(textBox_b_width.Text.Trim(), out data))
                {
                    double data1 = data - updateData;
                    double data2 = data + updateData;
                    MyCheckParamsConfig.BatWidthB = textBox_b_width.Text = data1 + "-" + data2;
                }
            }
            if (textBox_b_all.Text == "0" || textBox_b_all.Text == "999" || textBox_b_all.Text == "")
            {
                textBox_b_all.Text = "0-999";//总长度
            }
            else
            {
                double data = 0;
                if (double.TryParse(textBox_b_all.Text.Trim(), out data))
                {
                    double data1 = data - updateData;
                    double data2 = data + updateData;
                    MyCheckParamsConfig.AllLengthB = textBox_b_all.Text = data1 + "-" + data2;
                }
            }
            if (textBox_b_left_lug.Text == "0" || textBox_b_left_lug.Text == "999" || textBox_b_left_lug.Text == "")
            {
                textBox_b_left_lug.Text = "0-999";//左极耳边距
            }
            else
            {
                double data = 0;
                if (double.TryParse(textBox_b_left_lug.Text.Trim(), out data))
                {
                    double data1 = data - updateData;
                    double data2 = data + updateData;
                    MyCheckParamsConfig.LeftLugB = textBox_b_left_lug.Text = data1 + "-" + data2;
                }
            }
            if (textBox_b_right_lug.Text == "0" || textBox_b_right_lug.Text == "999" || textBox_b_right_lug.Text == "")
            {
                textBox_b_right_lug.Text = "0-999";//右极耳边距
            }
            else
            {
                double data = 0;
                if (double.TryParse(textBox_b_right_lug.Text.Trim(), out data))
                {
                    double data1 = data - updateData;
                    double data2 = data + updateData;
                    MyCheckParamsConfig.RightLugB = textBox_b_right_lug.Text = data1 + "-" + data2;
                }
            }
            if (textBox_b_left1.Text == "0" || textBox_b_left1.Text == "999" || textBox_b_left1.Text == "")
            {
                textBox_b_left1.Text = "0-999";//左1小白胶
            }
            else
            {
                double data = 0;
                if (double.TryParse(textBox_b_left1.Text.Trim(), out data))
                {
                    double data1 = data - updateData;
                    double data2 = data + updateData;
                    MyCheckParamsConfig.Left1GlueB = textBox_b_left1.Text = data1 + "-" + data2;
                }
            }
            if (textBox_b_left2.Text == "0" || textBox_b_left2.Text == "999" || textBox_b_left2.Text == "")
            {
                textBox_b_left2.Text = "0-999";//左2小白胶
            }
            else
            {
                double data = 0;
                if (double.TryParse(textBox_b_left2.Text.Trim(), out data))
                {
                    double data1 = data - updateData;
                    double data2 = data + updateData;
                    MyCheckParamsConfig.Left2GlueB = textBox_b_left2.Text = data1 + "-" + data2;
                }
            }
            if (textBox_b_right1.Text == "0" || textBox_b_right1.Text == "999" || textBox_b_right1.Text == "")
            {
                textBox_b_right1.Text = "0-999";//右1小白胶
            }
            else
            {
                double data = 0;
                if (double.TryParse(textBox_b_right1.Text.Trim(), out data))
                {
                    double data1 = data - updateData;
                    double data2 = data + updateData;
                    MyCheckParamsConfig.Right1GlueB = textBox_b_right1.Text = data1 + "-" + data2;
                }
            }
            if (textBox_b_right2.Text == "0" || textBox_b_right2.Text == "999" || textBox_b_right2.Text == "")
            {
                textBox_b_right2.Text = "0-999";//右2小白胶
            }
            else
            {
                double data = 0;
                if (double.TryParse(textBox_b_right2.Text.Trim(), out data))
                {
                    double data1 = data - updateData;
                    double data2 = data + updateData;
                    MyCheckParamsConfig.Right2GlueB = textBox_b_right2.Text = data1 + "-" + data2;
                }
            }
            #endregion
        }
    }
}
