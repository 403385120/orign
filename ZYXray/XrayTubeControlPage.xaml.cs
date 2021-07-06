using ATL.UI.Controls;
using ATL.UI.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Threading;
using ATL.Engine;
using ATL.Core;
using ATL.Station;
using ATL.Common;

using ATL.MES;
using System.Threading.Tasks;
using ZY.Motion;
using System.Windows.Controls;
using ZY.XRayTube;
using Shuyz.Framework.Mvvm;
using System.Windows.Input;
using ZYXray.Models;
using ZY.BarCodeReader;
using ZY.MitutoyoReader;
using XRayClient.Core;
using System.Net.Sockets;
using System.Net;
using System.Text;
using ZYXray.ViewModels;
using XRayClient.VisionSysWrapper;
using System.IO;


namespace ZYXray
{
    /// <summary>
    /// XrayTubeControlPage.xaml 的交互逻辑
    /// </summary>
    public partial class XrayTubeControlPage : BasePage, IComponentConnector
    {
        private static ATL.Engine.PLC plc = new ATL.Engine.PLC();
        public XrayTubeControlPage()
        {
            InitializeComponent();

            DataContext = View.Current;
        }

        public class View : ViewModelBase
        {
            public static View Current = new View();

            public XRayTubeStatus XRayTube1Stauts
            {
                get { return XRayTubeIF.XRayTube1Stauts; }
            }
            public XRayTubeStatus XRayTube2Status
            {
                get { return XRayTubeIF.XRayTube2Status; }
            }
            public Statistics MyStatistics
            {
                get { return BotIF.MyCheckStatus.MyStatistics; }
            }

            public ICommand ToggleXRayTube1
            {
                get
                {
                    return new RelayCommand(new Action(delegate
                    {
                        if (XRayTubeIF.XRayTube1Stauts.ShouldXrayOn)
                        {
                            XRayTubeIF.CloseXray(ETubePosition.Position1);
                            XRayTubeIF.CloseXray(ETubePosition.Position2);
                            plc.WriteByVariableName("XRayTubeOnState", 0);
                        }
                        else
                        {
                            XRayTubeIF.OpenXray(ETubePosition.Position1);
                            XRayTubeIF.OpenXray(ETubePosition.Position2);
                        }
                    }), delegate
                    {
                        return true;
                    });
                }
            }

            public ICommand ClearStat
            {
                get
                {
                    return new RelayCommand(new Action(delegate
                    {
                        this.MyStatistics.Reset();
                    }), delegate
                    {
                        return true;
                    });
                }
            }
        }

        private void btnScanBarcode_Click(object sender, RoutedEventArgs e)
        {
            string revMeg = "";
            try
            {

                SocketTCPInterface tcp = MotionControlVm.CreateSocket(HardwareConfig.Instance.ScanBarcodeIPAdress, HardwareConfig.Instance.ScanBarcodePort);
                string errmsg = string.Empty;
                if (!tcp.IsClose && !tcp.Connected)
                {
                    bool isOpen = MotionControlVm.InitSocketConnect(tcp, ref errmsg);
                    if (!isOpen)
                    {
                        ScanTestCode.Text = errmsg;
                    }
                }
                string errMsg = string.Empty;
                var inputData = $"{tcp.SendCommand1}{tcp.EndString}";
                var inputDataClear = $"{tcp.SendCommand2.ToUpper()}{tcp.EndString}";
                ScanTestCode.Text = tcp.SendData(inputData, ref errMsg, true, tcp.CommDelayIngTime, tcp.IsWriteLog);
                for (int i = 0; i < 1; i++)
                {
                    if (errMsg.Contains("TimedOut"))
                    {
                        ScanTestCode.Text = tcp.SendData(inputData, ref errMsg, true, tcp.CommDelayIngTime, tcp.IsWriteLog);
                        if (i>=0) {
                            ScanTestCode.Text =tcp.SendData(inputDataClear, ref errMsg, true, tcp.CommDelayIngTime, tcp.IsWriteLog);
                        }
                       
                    }
                    else {
                        if (!string.IsNullOrEmpty(errMsg)) {
                            ScanTestCode.Text = errMsg;
                        }
                        break;
                    }
                }
               
                //for (int i = 0; i < 3; i++)
                //{
                //    ScanTestCode.Text = tcp.SendData(inputDataClear, ref errMsg, true, tcp.CommDelayIngTime, tcp.IsWriteLog);
                //}
                //CodeReaderIF.ClientSendMsg("LON\r", 1);
                //Thread.Sleep(200);
                //CodeReaderIF.ClientSendMsg("LOFF\r", 1);
                //ScanTestCode.Text = CodeReaderIF.ClientReceiveMsg(1);
            }
            catch (Exception ex)
            {
                MessageBox.Show(revMeg + ex.Message);
            }
        }
        private void btnScanBarcode_Click2(object sender, RoutedEventArgs e)
        {
            string revMeg = "";
            try
            {
                SocketTCPInterface tcp = MotionControlVm.CreateSocket(HardwareConfig.Instance.ScanBarcodeIPAdress2, HardwareConfig.Instance.ScanBarcodePort2);
                string errmsg = string.Empty;
                if (!tcp.IsClose && !tcp.Connected)
                {
                    bool isOpen = MotionControlVm.InitSocketConnect(tcp, ref errmsg);
                    if (!isOpen)
                    {
                        ScanTestCode2.Text = errmsg;
                    }
                }
                string errMsg = string.Empty;
                var inputData = $"{tcp.SendCommand1}{tcp.EndString}";
                var inputDataClear = $"{tcp.SendCommand2.ToUpper()}{tcp.EndString}";
                ScanTestCode2.Text = tcp.SendData(inputData, ref errMsg, true, tcp.CommDelayIngTime, tcp.IsWriteLog);
                for (int i = 0; i < 1; i++)
                {
                    if (errMsg.Contains("TimedOut"))
                    {
                        ScanTestCode2.Text = tcp.SendData(inputData, ref errMsg, true, tcp.CommDelayIngTime, tcp.IsWriteLog);
                        if (i >=0)
                        {
                            ScanTestCode2.Text = tcp.SendData(inputDataClear, ref errMsg, true, tcp.CommDelayIngTime, tcp.IsWriteLog);
                        }

                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(errMsg))
                        {
                            ScanTestCode2.Text = errMsg;
                        }
                        break;
                    }
                }

                //CodeReaderIF.ClientSendMsg("LON\r", 3);
                //Thread.Sleep(200);
                //CodeReaderIF.ClientSendMsg("LOFF\r", 3);
                //ScanTestCode2.Text = CodeReaderIF.ClientReceiveMsg(3);
            }
            catch (Exception ex)
            {
                MessageBox.Show(revMeg + ex.Message);
            }
        }

        private void btnThicknessMeasure_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ARawThickness.Text = "";
                AThickness.Text = "";

                bool res = true;
                string ldstring = string.Empty;
                double thickness = 0;
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
                    thickness = double.Parse(strthickness);
                }
                ARawThickness.Text = (thickness).ToString();
                AThickness.Text = ldstring;
                SaveResultToFile_Thickness(Convert.ToSingle(thickness), 1);
            }
            catch (Exception ex)
            {
                ARawThickness.Text = ex.Message;
                AThickness.Text = "";
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            DateTime d1 = Convert.ToDateTime(FQAtime.Text);
            string Bar = FQABarcode.Text;
            List<Inquire> inquires = new List<Inquire>();
            int id = 5;
            switch (this.combobox1.SelectedIndex)
            {
                case 0: id = 5; break;
                case 1: id = 6; break;
            }

            Task.Run(() =>
            {
                if (Bar != "")
                {
                    inquires = ZYXray.ViewModels.MakeUp.MakeUpData(d1, id, Bar);
                }
                else
                {
                    inquires = ZYXray.ViewModels.MakeUp.MakeUpData(d1, id);
                }
                foreach (Inquire inquire in inquires)
                {
                    ATL.MES.A014.Root _root = ATL.MES.InterfaceClient.Current.A013Offline(inquire.Barcode, inquire.data);
                    //更新数据库状态
                    ZYXray.ViewModels.MakeUp.UpdateSQL(inquire, id);
                    Thread.Sleep(10);
                }
                MessageBox.Show("补传复判数据完成！已上传" + inquires.Count + "条数据！");
            });
        }

        private void btnDimension_Click(object sender, RoutedEventArgs e)
        {
            string revMeg = "";
            try
            {
                CodeReaderIF.ClientSendMsg("Run_ACQ2,abc\r", 2);
            }
            catch (Exception ex)
            {
                CodeReaderIF.Init_ClientConnet(HardwareConfig.Instance.DimensionIPAdress,
                    HardwareConfig.Instance.DimensionPort, 2); //尺寸测量连接
                CodeReaderIF.ClientSendMsg("Run_ACQ2,\r", 2);
                //MessageBox.Show(revMeg + ex.Message);
            }
            Thread.Sleep(500);
            DimensionTestCode.Text = CodeReaderIF.ClientReceiveMsg(2);
        }

        private void btnBThicknessMeasure_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                BRawThickness.Text = "";
                BThickness.Text = "";

                bool res = true;
                string ldstring = string.Empty;
                double thickness = 0;
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
                    thickness = double.Parse(strthickness);
                }
                BRawThickness.Text = (thickness).ToString();
                BThickness.Text = ldstring;
                SaveResultToFile_Thickness(Convert.ToSingle(thickness), 2);
            }
            catch (Exception ex)
            {
                BRawThickness.Text = ex.Message;
                BThickness.Text = "";
            }
        }
        private void btnCThicknessMeasure_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CRawThickness.Text = "";
                CThickness.Text = "";

                bool res = true;
                string ldstring = string.Empty;
                double thickness = 0;
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
                    thickness = double.Parse(strthickness);
                }
                CRawThickness.Text = (thickness).ToString();
                CThickness.Text = ldstring;
                SaveResultToFile_Thickness(Convert.ToSingle(thickness), 3);
            }
            catch (Exception ex)
            {
                CRawThickness.Text = ex.Message;
                CThickness.Text = "";
            }
        }
        private void btnDThicknessMeasure_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DRawThickness.Text = "";
                DThickness.Text = "";

                bool res = true;
                string ldstring = string.Empty;
                double thickness = 0;
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
                    thickness = double.Parse(strthickness);
                }
                DRawThickness.Text = (thickness).ToString();
                DThickness.Text = ldstring;
                SaveResultToFile_Thickness(Convert.ToSingle(thickness), 4);
            }
            catch (Exception ex)
            {
                DRawThickness.Text = ex.Message;
                DThickness.Text = "";
            }
        }

        private void btnBT3562_Click(object sender, RoutedEventArgs e)
        {
            txtBT3562.Text = MotionControlVm.Bt3562.GetRisitance().ToString();
            //int position = 0;
            //if (plc.ReadByVariableName("TestMode") == "7")
            //{
            //    if (plc.ReadByVariableName("ResistanceSignal1") == "1")
            //    {
            //        plc.WriteByVariableName("ResistanceComplete1", 1);
            //        position = 1;
            //    }
            //    if (plc.ReadByVariableName("ResistanceSignal2") == "1")
            //    {
            //        plc.WriteByVariableName("ResistanceComplete2", 1);
            //        position = 2;
            //    }
            //    string result = "NG";
            //    if (Convert.ToDouble(txtBT3562.Text) > 10 && Convert.ToDouble(txtBT3562.Text) < 25)
            //    {
            //        result = "OK";
            //    }
            //    SaveResultToFile_OCV(txtBT3562.Text, result, "内阻", position);

            //}
        }
        private void btn34461A_Click(object sender, RoutedEventArgs e)
        {
            txt34461A.Text = MotionControlVm.Tcp34461A.Get34461AVoltage().ToString();
            //int position = 0;
            //if (plc.ReadByVariableName("TestMode") == "7")
            //{
            //    if (plc.ReadByVariableName("VoltageSignal1") == "1")
            //    {
            //        plc.WriteByVariableName("VoltageComplete1", 1);
            //        position = 1;
            //    }
            //    if (plc.ReadByVariableName("VoltageSignal2") == "1")
            //    {
            //        plc.WriteByVariableName("VoltageComplete2", 1);
            //        position = 2;
            //    }
            //    string result = "NG";
            //    if (Convert.ToDouble(txt34461A.Text) > 3 && Convert.ToDouble(txt34461A.Text) < 4)
            //    {
            //        result = "OK";
            //    }
            //    SaveResultToFile_OCV(txt34461A.Text, result, "电压", position);

            //}
        }
        private void btnE5CC_Click(object sender, RoutedEventArgs e)
        {
            txtE5CC.Text = MotionControlVm.E5cc.GetE5CCTemperatrue().ToString();
        }
        private void btnToprie_Click(object sender, RoutedEventArgs e)
        {
            txtMI3_1.Text = MotionControlVm.Mi3_1.GetRaytekMI3Temperatrue().ToString();
        }
        private void btnToprie2_Click(object sender, RoutedEventArgs e)
        {
            txtMI3_2.Text = MotionControlVm.Mi3_2.GetRaytekMI3Temperatrue().ToString();
        }
        private void btnLR8401_Click(object sender, RoutedEventArgs e)
        {
            MotionControlVm.Lr8450.StartTest();
            Thread.Sleep(2000);
            //txtLR8401.Text = MotionControlVm.Lr8450.GetData(1);
            string strData1 = MotionControlVm.Lr8450.GetData(1);
            string strData2 = MotionControlVm.Lr8450.GetData(3);
            string strData3 = MotionControlVm.Lr8450.GetData(5);
            string strData4 = MotionControlVm.Lr8450.GetData(7);
            txtLR8401.Text = strData1;
            BatterySeat bt = new BatterySeat();
            bt.IV = strData1;
            bt.Sn = "1";
            MotionControlVm.Instance.SaveIVDataToFile(bt);
            bt.IV = strData2;
            bt.Sn = "2";
            MotionControlVm.Instance.SaveIVDataToFile(bt);
            bt.IV = strData3;
            bt.Sn = "3";
            MotionControlVm.Instance.SaveIVDataToFile(bt);
            bt.IV = strData4;
            bt.Sn = "4";
            MotionControlVm.Instance.SaveIVDataToFile(bt);
            bt.Destroy();
        }

        public void SaveResultToFile_OCV(string value, string result, string type, int position)
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
            };


        }

        private void SaveResultToFile_Thickness(float thickness, int position)
        {
            string filePath = "D:\\测量数据\\手动数据\\PPG标块点检\\" + ATL.Core.DeivceEquipmentidPlcInfo.lstDeivceEquipmentidPlcInfos[0].ModelInfo;
            string fileName = filePath + "\\PPG_" + DateTime.Now.ToString("yyyyMMdd") + ".csv";

            if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);

            string line = "";
            byte[] myByte = System.Text.Encoding.UTF8.GetBytes(line);

            if (!File.Exists(fileName))
            {
                line = "时间,厚度值,工位\r\n";
                myByte = System.Text.Encoding.UTF8.GetBytes(line);
                using (FileStream fsWrite = new FileStream(fileName, FileMode.Create))
                {
                    byte[] bs = { (byte)0xEF, (byte)0xBB, (byte)0xBF };
                    fsWrite.Write(bs, 0, bs.Length);
                    fsWrite.Write(myByte, 0, myByte.Length);
                };
            }

            string time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            line = time + "," + (CheckParamsConfig.Instance.CaliValThickness + thickness).ToString("N3") + "," + position;
            line += "\r\n";

            myByte = System.Text.Encoding.UTF8.GetBytes(line);
            using (FileStream fsWrite = new FileStream(fileName, FileMode.Append))
            {
                fsWrite.Write(myByte, 0, myByte.Length);
            };


        }

    }
}
