using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Shuyz.Framework.Mvvm;
using ZYXray.ViewModels;
using XRayClient.Core;
using System.Windows;
using System.Windows.Input;

namespace ZYXray
{
    public class ChannelCheckVm : ObservableObject
    {
        //private static ChannelCheckVm _instance = new ChannelCheckVm();
        private static ATL.Engine.PLC plc = new ATL.Engine.PLC();
        private string _voltage1;
        private string _voltage2;
        private string _resistance1;
        private string _resistance2;
        private string _voltageResult1;
        private string _voltageResult2;
        private string _resistanceResult1;
        private string _resistanceResult2;

        private string _iV1;
        private string _iV2;
        private string _iV3;
        private string _iV4;
        private string _iVResult1;
        private string _iVResult2;
        private string _iVResult3;
        private string _iVResult4;

        //public static ChannelCheckVm Instance
        //{
        //    get { return _instance; }
        //}

        public string Voltage1
        {
            get { return _voltage1; }
            set
            {
                _voltage1 = value;
                RaisePropertyChanged("Voltage1");
            }
        }

        public string Voltage2
        {
            get { return _voltage2; }
            set
            {
                _voltage2 = value;
                RaisePropertyChanged("Voltage2");
            }
        }

        public string Resistance1
        {
            get { return _resistance1; }
            set
            {
                _resistance1 = value;
                RaisePropertyChanged("Resistance1");
            }
        }

        public string Resistance2
        {
            get { return _resistance2; }
            set
            {
                _resistance2 = value;
                RaisePropertyChanged("Resistance2");
            }
        }

        public string VoltageResult1
        {
            get { return _voltageResult1; }
            set
            {
                _voltageResult1 = value;
                RaisePropertyChanged("VoltageResult1");
            }
        }

        public string VoltageResult2
        {
            get { return _voltageResult2; }
            set
            {
                _voltageResult2 = value;
                RaisePropertyChanged("VoltageResult2");
            }
        }

        public string ResistanceResult1
        {
            get { return _resistanceResult1; }
            set
            {
                _resistanceResult1 = value;
                RaisePropertyChanged("ResistanceResult1");
            }
        }

        public string ResistanceResult2
        {
            get { return _resistanceResult2; }
            set
            {
                _resistanceResult2 = value;
                RaisePropertyChanged("ResistanceResult2");
            }
        }

        public string IV1
        {
            get { return _iV1; }
            set
            {
                _iV1 = value;
                RaisePropertyChanged("IV1");
            }
        }

        public string IV2
        {
            get { return _iV2; }
            set
            {
                _iV2 = value;
                RaisePropertyChanged("IV2");
            }
        }

        public string IV3
        {
            get { return _iV3; }
            set
            {
                _iV3 = value;
                RaisePropertyChanged("IV3");
            }
        }

        public string IV4
        {
            get { return _iV4; }
            set
            {
                _iV4 = value;
                RaisePropertyChanged("IV4");
            }
        }

        public string IVResult1
        {
            get { return _iVResult1; }
            set
            {
                _iV1 = value;
                RaisePropertyChanged("IVResult1");
            }
        }

        public string IVResult2
        {
            get { return _iVResult2; }
            set
            {
                _iV2 = value;
                RaisePropertyChanged("IVResult2");
            }
        }

        public string IVResult3
        {
            get { return _iVResult3; }
            set
            {
                _iV3 = value;
                RaisePropertyChanged("IVResult3");
            }
        }

        public string IVResult4
        {
            get { return _iVResult4; }
            set
            {
                _iV4 = value;
                RaisePropertyChanged("IVResult4");
            }
        }

        public ICommand ClearData
        {
            get
            {
                return new RelayCommand(new Action(delegate
                {
                    using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
                    {
                        Voltage1 = "";
                        Voltage2 = "";
                        VoltageResult1 = "";
                        VoltageResult2 = "";
                        Resistance1 = "";
                        Resistance2 = "";
                        ResistanceResult1 = "";
                        ResistanceResult2 = "";
                    }
                }), delegate
                {
                    return true;
                });
            }
        }

        public ChannelCheckVm()
        {
            ThreadPool.QueueUserWorkItem(state => CheckIVAndOCV());
        }

        private enum CheckVoltage1
        {
            触发工位1电压测试,
            工位1电压测试完成
        }
        private static CheckVoltage1 EnumCheckVoltage1 = 0;
        private enum CheckVoltage2
        {
            触发工位2电压测试,
            工位2电压测试完成
        }
        private static CheckVoltage2 EnumCheckVoltage2 = 0;

        private enum CheckResistance1
        {
            触发工位1内阻测试,
            工位1内阻测试完成
        }
        private static CheckResistance1 EnumCheckResistance1 = 0;
        private enum CheckResistance2
        {
            触发工位2内阻测试,
            工位2内阻测试完成
        }
        private static CheckResistance2 EnumCheckResistance2 = 0;

        private enum CheckIV1
        {
            触发工位1IV测试,
            工位1IV测试完成
        }
        private static CheckIV1 EnumCheckIV1 = 0;
        private enum CheckIV2
        {
            触发工位2IV测试,
            工位2IV测试完成
        }
        private static CheckIV2 EnumCheckIV2 = 0;
        private enum CheckIV3
        {
            触发工位3IV测试,
            工位3IV测试完成
        }
        private static CheckIV3 EnumCheckIV3 = 0;
        private enum CheckIV4
        {
            触发工位4IV测试,
            工位4IV测试完成
        }
        private static CheckIV4 EnumCheckIV4 = 0;
        private static bool isThreadOpen=false;

        private void CheckIVAndOCV()
        {
            if (isThreadOpen == true)
            {
                return;
            }
            isThreadOpen = true;
            while (true)
            {
                Thread.Sleep(50);

                if (plc.ReadByVariableName("TestMode") == "7")
                {
                    switch (EnumCheckVoltage1)
                    {
                        case CheckVoltage1.触发工位1电压测试:

                            if (plc.ReadByVariableName("VoltageSignal1") == "1")
                            {
                                Thread.Sleep(500);
                                Voltage1 = MotionControlVm.Tcp34461A.Get34461AVoltage().ToString();
                                if (Convert.ToDouble(Voltage1) > 3)
                                {
                                    VoltageResult1 = "OK";
                                }
                                else
                                {
                                    VoltageResult1 = "NG";
                                }
                                SaveResultToFile_OCV(Voltage1, VoltageResult1, "电压", 1);
                                if (MotionControlVm.Instance.CheckOCV())
                                {
                                    CheckParamsConfig.Instance.OcvCheckTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                    CheckParamsSettingsVm.Instance.MyCheckParamsConfig.Write();
                                    MessageBox.Show("OCV点检通过");
                                }

                                plc.WriteByVariableName("VoltageComplete1", 1);
                                EnumCheckVoltage1 = CheckVoltage1.工位1电压测试完成;
                            }
                            break;

                        case CheckVoltage1.工位1电压测试完成:

                            if (plc.ReadByVariableName("VoltageSignal1") == "0")
                            {
                                plc.WriteByVariableName("VoltageComplete1", 0);
                                EnumCheckVoltage1 = CheckVoltage1.触发工位1电压测试;
                            }
                            break;
                    }

                    switch (EnumCheckVoltage2)
                    {
                        case CheckVoltage2.触发工位2电压测试:

                            if (plc.ReadByVariableName("VoltageSignal2") == "1")
                            {
                                Thread.Sleep(500);
                                Voltage2 = MotionControlVm.Tcp34461A.Get34461AVoltage().ToString();
                                if (Convert.ToDouble(Voltage2) > 3)
                                {
                                    VoltageResult2 = "OK";
                                }
                                else
                                {
                                    VoltageResult2 = "NG";
                                }
                                plc.WriteByVariableName("VoltageComplete2", 1);
                                SaveResultToFile_OCV(Voltage2, VoltageResult2, "电压", 2);
                                EnumCheckVoltage2 = CheckVoltage2.工位2电压测试完成;
                            }
                            break;

                        case CheckVoltage2.工位2电压测试完成:

                            if (plc.ReadByVariableName("VoltageSignal2") == "0")
                            {
                                plc.WriteByVariableName("VoltageComplete2", 0);
                                EnumCheckVoltage2 = CheckVoltage2.触发工位2电压测试;
                            }
                            break;
                    }

                    switch (EnumCheckResistance1)
                    {
                        case CheckResistance1.触发工位1内阻测试:

                            if (plc.ReadByVariableName("ResistanceSignal1") == "1")
                            {
                                Thread.Sleep(500);
                                Resistance1 = MotionControlVm.Bt3562.GetRisitance().ToString();
                                if (Convert.ToDouble(Resistance1) < 40)
                                {
                                    ResistanceResult1 = "OK";
                                }
                                else
                                {
                                    ResistanceResult1 = "NG";
                                }
                                plc.WriteByVariableName("ResistanceComplete1", 1);
                                SaveResultToFile_OCV(Resistance1, ResistanceResult1, "内阻", 1);
                                EnumCheckResistance1 = CheckResistance1.工位1内阻测试完成;
                            }
                            break;

                        case CheckResistance1.工位1内阻测试完成:

                            if (plc.ReadByVariableName("ResistanceSignal1") == "0")
                            {
                                plc.WriteByVariableName("ResistanceComplete1", 0);
                                EnumCheckResistance1 = CheckResistance1.触发工位1内阻测试;
                            }
                            break;
                    }

                    switch (EnumCheckResistance2)
                    {
                        case CheckResistance2.触发工位2内阻测试:

                            if (plc.ReadByVariableName("ResistanceSignal2") == "1")
                            {
                                Thread.Sleep(500);
                                Resistance2 = MotionControlVm.Bt3562.GetRisitance().ToString();
                                if (Convert.ToDouble(Resistance2) < 40)
                                {
                                    ResistanceResult2 = "OK";
                                }
                                else
                                {
                                    ResistanceResult2 = "NG";
                                }
                                plc.WriteByVariableName("ResistanceComplete2", 1);
                                SaveResultToFile_OCV(Resistance2, ResistanceResult2, "内阻", 2);
                                if (MotionControlVm.Instance.CheckOCV())
                                {
                                    CheckParamsConfig.Instance.OcvCheckTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                    CheckParamsSettingsVm.Instance.MyCheckParamsConfig.Write();
                                    MessageBox.Show("OCV点检通过");
                                }

                                EnumCheckResistance2 = CheckResistance2.工位2内阻测试完成;
                            }
                            break;

                        case CheckResistance2.工位2内阻测试完成:

                            if (plc.ReadByVariableName("ResistanceSignal2") == "0")
                            {
                                plc.WriteByVariableName("ResistanceComplete2", 0);
                                EnumCheckResistance2 = CheckResistance2.触发工位2内阻测试;
                            }
                            break;
                    }
                }


                if (plc.ReadByVariableName("TestMode") == "6")
                {
                    switch (EnumCheckIV1)
                    {
                        case CheckIV1.触发工位1IV测试:

                            if (plc.ReadByVariableName("IVTestSignal1") == "1")
                            {
                                MotionControlVm.Lr8450.StartTest();
                                Thread.Sleep(2000);
                                IV1 = MotionControlVm.Lr8450.GetData(1);
                                string[] arr = IV1.Split(',');
                                IVResult1 = "OK";
                                for (int i = 0; i < arr.Length; i++)
                                {
                                    if (Convert.ToDouble(arr[i]) < 3)
                                    {
                                        IVResult1 = "NG";
                                    }
                                }
                                //IV1 = Math.Max(Convert.ToDouble(arr[arr.Length - 1]), Convert.ToDouble(arr[arr.Length - 2])).ToString();
                                plc.WriteByVariableName("IVComplete", 1);
                                EnumCheckIV1 = CheckIV1.工位1IV测试完成;
                            }
                            break;

                        case CheckIV1.工位1IV测试完成:

                            if (plc.ReadByVariableName("IVTestSignal1") == "0")
                            {
                                plc.WriteByVariableName("IVComplete", 0);
                                EnumCheckIV1 = CheckIV1.工位1IV测试完成;
                            }
                            break;
                    }

                    switch (EnumCheckIV2)
                    {
                        case CheckIV2.触发工位2IV测试:

                            if (plc.ReadByVariableName("IVTestSignal2") == "1")
                            {
                                MotionControlVm.Lr8450.StartTest();
                                Thread.Sleep(2000);
                                IV2 = MotionControlVm.Lr8450.GetData(3);
                                string[] arr = IV2.Split(',');
                                IVResult2 = "OK";
                                for (int i = 0; i < arr.Length; i++)
                                {
                                    if (Convert.ToDouble(arr[i]) < 3)
                                    {
                                        IVResult2 = "NG";
                                    }
                                }
                                //IV2 = Math.Max(Convert.ToDouble(arr[arr.Length - 1]), Convert.ToDouble(arr[arr.Length - 2])).ToString();
                                plc.WriteByVariableName("IVComplete", 1);
                                EnumCheckIV2 = CheckIV2.工位2IV测试完成;
                            }
                            break;

                        case CheckIV2.工位2IV测试完成:

                            if (plc.ReadByVariableName("IVTestSignal2") == "0")
                            {
                                plc.WriteByVariableName("IVComplete", 0);
                                EnumCheckIV2 = CheckIV2.工位2IV测试完成;
                            }
                            break;
                    }

                    switch (EnumCheckIV3)
                    {
                        case CheckIV3.触发工位3IV测试:

                            if (plc.ReadByVariableName("IVTestSignal3") == "1")
                            {
                                MotionControlVm.Lr8450.StartTest();
                                Thread.Sleep(2000);
                                IV3 = MotionControlVm.Lr8450.GetData(5);
                                string[] arr = IV3.Split(',');
                                IVResult3 = "OK";
                                for (int i = 0; i < arr.Length; i++)
                                {
                                    if (Convert.ToDouble(arr[i]) < 3)
                                    {
                                        IVResult3 = "NG";
                                    }
                                }
                                //IV3 = Math.Max(Convert.ToDouble(arr[arr.Length - 1]), Convert.ToDouble(arr[arr.Length - 2])).ToString();
                                plc.WriteByVariableName("IVComplete", 1);
                                EnumCheckIV3 = CheckIV3.工位3IV测试完成;
                            }
                            break;

                        case CheckIV3.工位3IV测试完成:

                            if (plc.ReadByVariableName("IVTestSignal3") == "0")
                            {
                                plc.WriteByVariableName("IVComplete", 0);
                                EnumCheckIV3 = CheckIV3.工位3IV测试完成;
                            }
                            break;
                    }

                    switch (EnumCheckIV4)
                    {
                        case CheckIV4.触发工位4IV测试:

                            if (plc.ReadByVariableName("IVTestSignal4") == "1")
                            {
                                MotionControlVm.Lr8450.StartTest();
                                Thread.Sleep(2000);
                                IV4 = MotionControlVm.Lr8450.GetData(7);
                                string[] arr = IV4.Split(',');
                                IVResult4 = "OK";
                                for (int i = 0; i < arr.Length; i++)
                                {
                                    if (Convert.ToDouble(arr[i]) < 3)
                                    {
                                        IVResult4 = "NG";
                                    }
                                }
                                //IV4 = Math.Max(Convert.ToDouble(arr[arr.Length - 1]), Convert.ToDouble(arr[arr.Length - 2])).ToString();
                                plc.WriteByVariableName("IVComplete", 1);
                                EnumCheckIV4 = CheckIV4.工位4IV测试完成;
                            }
                            break;

                        case CheckIV4.工位4IV测试完成:

                            if (plc.ReadByVariableName("IVTestSignal4") == "0")
                            {
                                plc.WriteByVariableName("IVComplete", 0);
                                EnumCheckIV4 = CheckIV4.工位4IV测试完成;
                            }
                            break;
                    }
                }

            }
        }


        private static object objLockOcv = new object();
        public void SaveResultToFile_OCV(string value, string result, string type, int position)
        {
            lock (objLockOcv)
            {
                string filePath = "D:\\测量数据\\点检数据\\OCV标块点检\\" + ATL.Core.DeivceEquipmentidPlcInfo.lstDeivceEquipmentidPlcInfos[0].ModelInfo;
                string fileName = filePath + "\\OCV_" + DateTime.Now.ToString("yyyyMMdd") + ".csv";

                if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);

                string line = "";
                byte[] myByte = System.Text.Encoding.UTF8.GetBytes(line);

                if (!File.Exists(fileName))
                {
                    line = "点检时间,值,结果,类型,工位\r\n";
                    myByte = System.Text.Encoding.UTF8.GetBytes(line);
                    using (FileStream fsWrite = new FileStream(fileName, FileMode.Create))
                    {
                        byte[] bs = { (byte)0xEF, (byte)0xBB, (byte)0xBF };
                        fsWrite.Write(bs, 0, bs.Length);
                        fsWrite.Write(myByte, 0, myByte.Length);
                    };
                }

                string time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                line = time + "," + value + "," + result + "," + type + "," + position;
                line += "\r\n";

                myByte = System.Text.Encoding.UTF8.GetBytes(line);
                using (FileStream fsWrite = new FileStream(fileName, FileMode.Append))
                {
                    fsWrite.Write(myByte, 0, myByte.Length);
                }
            }
        }

    }
}
