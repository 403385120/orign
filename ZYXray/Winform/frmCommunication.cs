using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using Esquel.BaseManager;
using Ivi.Visa.Interop;
using SuperCom.DataCommunication;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZY.BLL;
using ZY.DAL;
using ZY.Logging;
using ZY.MitutoyoReader;
using ZY.Model;
using ZY.Systems;
using ZYXray.ViewModels;

namespace ZYXray.Winform
{
    public partial class frmCommunication : Form
    {
        public static frmCommunication Current;
        public frmCommunication()
        {
            InitializeComponent();
        }

        DictionaryRefDAL dictionaryRefDAL = new DictionaryRefDAL();
        CommunicationDAL commfDAL = new CommunicationDAL();
        private void frmCommunication_Load(object sender, EventArgs e)
        {

            //string filters = $" {Common.SqlIsNullKey}RefIsUse = 1";
            //string errMsg = string.Empty;
            //var listRef = dictionaryRefDAL.GetList(filters, ref errMsg);

            string filters = "";
            string errMsg = string.Empty;
            var listcomm = commfDAL.GetList(filters, ref errMsg);
            var listdic = listcomm as List<Communication>;

            Common.lstComm = listdic;

            string ScanBarcodeIPAdress = FilesManager.GetPrivateProfileString(Application.StartupPath + "\\HardwareConfig.ini", "CodeReaderConfig", "ScanBarcodeIPAdress", "192.168.100.3");//扫码1 IP
            int ScanBarcodePort = int.Parse(FilesManager.GetPrivateProfileString(Application.StartupPath + "\\HardwareConfig.ini", "CodeReaderConfig", "ScanBarcodePort", "9004"));//扫码1 IP

            var comm = Common.lstComm.FirstOrDefault(o => o.Name.Equals("上料扫码"));
            if (comm == null)
            {
                comm = new Communication();
            }
            comm.Factory = "ATL";
            comm.Code = "socket";
            comm.Name = "上料扫码";
            comm.ComPort = "";
            comm.Ip = ScanBarcodeIPAdress;
            comm.IpPort = ScanBarcodePort;
            comm.CommunicationTypeName = "基恩士";
            comm.SendCommand1 = "LON";
            comm.SendCommand2 = "LOFF";
            comm.ScanCount = 2;
            comm.ScanInterval = 50;
            comm.IsUse = true;
            comm.IsSocket = true;
            comm.IsSocketTcp = true;
            comm.SocketTimeOut = 2000;
            comm.IsClose = true;
            comm.ReadDelay = 1;
            comm.IsLine = true;

            string errmsg = string.Empty;
            if (!commfDAL.IsExists(" Name ='上料扫码'", ref errmsg))
            {
                commfDAL.Add(comm, ref errmsg);
            }
            else
            {
                commfDAL.Update(comm, ref errmsg);
            }


            string ScanBarcodeIPAdress2 = FilesManager.GetPrivateProfileString(Application.StartupPath + "\\HardwareConfig.ini", "CodeReaderConfig", "ScanBarcodeIPAdress2", "192.168.250.101"); //扫码2 IP
            int ScanBarcodePort2 = int.Parse(FilesManager.GetPrivateProfileString(Application.StartupPath + "\\HardwareConfig.ini", "CodeReaderConfig", "ScanBarcodePort", "9004")); //扫码2 IP
            comm = Common.lstComm.FirstOrDefault(o => o.Name.Equals("下料扫码"));
            if (comm == null)
            {
                comm = new Communication();
            }
            comm.Factory = "ATL";
            comm.Code = "socket";
            comm.Name = "下料扫码";
            comm.ComPort = "";
            comm.Ip = ScanBarcodeIPAdress2;
            comm.IpPort = ScanBarcodePort2;
            comm.CommunicationTypeName = "基恩士";
            comm.SendCommand1 = "LON";
            comm.SendCommand2 = "LOFF";
            comm.ScanCount = 2;
            comm.ScanInterval = 50;
            comm.IsUse = true;
            comm.IsSocket = true;
            comm.IsSocketTcp = true;
            comm.SocketTimeOut = 2000;
            comm.IsClose = true;
            comm.ReadDelay = 1;
            comm.IsLine = true;
            if (!commfDAL.IsExists(" Name ='下料扫码'", ref errmsg))
            {
                commfDAL.Add(comm, ref errmsg);
            }
            else
            {
                commfDAL.Update(comm, ref errmsg);
            }



            string DimensionIPAdress = FilesManager.GetPrivateProfileString(Application.StartupPath + "\\HardwareConfig.ini", "CodeReaderConfig", "DimensionIPAdress", "127.0.0.1");//尺寸测量 IP
            int DimensionPort = int.Parse(FilesManager.GetPrivateProfileString(Application.StartupPath + "\\HardwareConfig.ini", "CodeReaderConfig", "DimensionPort", "6600"));//尺寸测量  端口号

            comm = Common.lstComm.FirstOrDefault(o => o.Name.Equals("尺寸测量"));
            if (comm == null)
            {
                comm = new Communication();
            }
            comm.Factory = "ATL";
            comm.Code = "socket";
            comm.Name = "尺寸测量";
            comm.ComPort = "";
            comm.Ip = DimensionIPAdress;
            comm.IpPort = DimensionPort;
            if (!commfDAL.IsExists(" Name ='尺寸测量'", ref errmsg))
            {
                commfDAL.Add(comm, ref errmsg);
            }
            else
            {
                commfDAL.Update(comm, ref errmsg);
            }

            string ThicknessCOM = FilesManager.GetPrivateProfileString(Application.StartupPath + "\\HardwareConfig.ini", "MitutoyoConfig", "PortName", "COM12");//测厚1  COM
            int ThicknessBaud = int.Parse(FilesManager.GetPrivateProfileString(Application.StartupPath + "\\HardwareConfig.ini", "MitutoyoConfig", "BaudRate", "2400"));//测厚1  波特率
            string ThicknessData = FilesManager.GetPrivateProfileString(Application.StartupPath + "\\HardwareConfig.ini", "MitutoyoConfig", "DataBits", "8");//测厚1  数据位
            string ThicknessStop = FilesManager.GetPrivateProfileString(Application.StartupPath + "\\HardwareConfig.ini", "MitutoyoConfig", "StopBits", "1");//测厚1  停止位
            string ThicknessParity = FilesManager.GetPrivateProfileString(Application.StartupPath + "\\HardwareConfig.ini", "MitutoyoConfig", "Parity", "0");//测厚1  校验值
            string MitutoyoReaderTypes = FilesManager.GetPrivateProfileString(Application.StartupPath + "\\HardwareConfig.ini", "MitutoyoConfig", "MitutoyoReaderTypes", "MitutoyoSerial");//测厚1  型号

            comm = Common.lstComm.FirstOrDefault(o => o.Name.Equals("厚度测试1"));
            if (comm == null)
            {
                comm = new Communication();
            }
            comm.Factory = "ATL";
            comm.Code = "串口";
            comm.Name = "厚度测试1";
            comm.ComPort = ThicknessCOM;
            comm.ComBaudRate = ThicknessBaud;
            comm.ComStopBits = ThicknessStop;
            comm.ComParity = ThicknessParity;
            comm.CommunicationTypeName = MitutoyoReaderTypes;
            if (!commfDAL.IsExists(" Name ='厚度测试1'", ref errmsg))
            {
                commfDAL.Add(comm, ref errmsg);
            }
            else
            {
                commfDAL.Update(comm, ref errmsg);
            }

            string ThicknessCOM2 = FilesManager.GetPrivateProfileString(Application.StartupPath + "\\HardwareConfig.ini", "MitutoyoConfig2", "PortName", "COM12");//测厚2  COM
            int ThicknessBaud2 = int.Parse(FilesManager.GetPrivateProfileString(Application.StartupPath + "\\HardwareConfig.ini", "MitutoyoConfig2", "BaudRate", "2400"));//测厚2  波特率
            string ThicknessData2 = FilesManager.GetPrivateProfileString(Application.StartupPath + "\\HardwareConfig.ini", "MitutoyoConfig2", "DataBits", "8");//测厚2  数据位
            string ThicknessStop2 = FilesManager.GetPrivateProfileString(Application.StartupPath + "\\HardwareConfig.ini", "MitutoyoConfig2", "StopBits", "1");//测厚2  停止位
            string ThicknessParity2 = FilesManager.GetPrivateProfileString(Application.StartupPath + "\\HardwareConfig.ini", "MitutoyoConfig2", "Parity", "0");//测厚2  校验值
            string MitutoyoReaderTypes2 = FilesManager.GetPrivateProfileString(Application.StartupPath + "\\HardwareConfig.ini", "MitutoyoConfig2", "MitutoyoReaderTypes", "MitutoyoSerial");//测厚2  型号

            comm = Common.lstComm.FirstOrDefault(o => o.Name.Equals("厚度测试2"));
            if (comm == null)
            {
                comm = new Communication();
            }
            comm.Factory = "ATL";
            comm.Code = "串口";
            comm.Name = "厚度测试2";
            comm.ComPort = ThicknessCOM2;
            comm.ComBaudRate = ThicknessBaud2;
            comm.ComStopBits = ThicknessStop2;
            comm.ComParity = ThicknessParity2;
            comm.CommunicationTypeName = MitutoyoReaderTypes2;
            if (!commfDAL.IsExists(" Name ='厚度测试2'", ref errmsg))
            {
                commfDAL.Add(comm, ref errmsg);
            }
            else
            {
                commfDAL.Update(comm, ref errmsg);
            }
            string ThicknessCOM3 = FilesManager.GetPrivateProfileString(Application.StartupPath + "\\HardwareConfig.ini", "MitutoyoConfig3", "PortName", "COM12");//测厚3  COM
            int ThicknessBaud3 = int.Parse(FilesManager.GetPrivateProfileString(Application.StartupPath + "\\HardwareConfig.ini", "MitutoyoConfig3", "BaudRate", "2400"));//测厚3  波特率
            string ThicknessData3 = FilesManager.GetPrivateProfileString(Application.StartupPath + "\\HardwareConfig.ini", "MitutoyoConfig3", "DataBits", "8");//测厚3  数据位
            string ThicknessStop3 = FilesManager.GetPrivateProfileString(Application.StartupPath + "\\HardwareConfig.ini", "MitutoyoConfig3", "StopBits", "1");//测厚3  停止位
            string ThicknessParity3 = FilesManager.GetPrivateProfileString(Application.StartupPath + "\\HardwareConfig.ini", "MitutoyoConfig3", "Parity", "0");//测厚3  校验值
            string MitutoyoReaderTypes3 = FilesManager.GetPrivateProfileString(Application.StartupPath + "\\HardwareConfig.ini", "MitutoyoConfig3", "MitutoyoReaderTypes", "MitutoyoSerial");//测厚3  型号
            comm = Common.lstComm.FirstOrDefault(o => o.Name.Equals("厚度测试3"));
            if (comm == null)
            {
                comm = new Communication();
            }
            comm.Factory = "ATL";
            comm.Code = "串口";
            comm.Name = "厚度测试3";
            comm.ComPort = ThicknessCOM3;
            comm.ComBaudRate = ThicknessBaud3;
            comm.ComStopBits = ThicknessStop3;
            comm.ComParity = ThicknessParity3;
            comm.CommunicationTypeName = MitutoyoReaderTypes3;
            if (!commfDAL.IsExists(" Name ='厚度测试3'", ref errmsg))
            {
                commfDAL.Add(comm, ref errmsg);
            }
            else
            {
                commfDAL.Update(comm, ref errmsg);
            }


            string ThicknessCOM4 = FilesManager.GetPrivateProfileString(Application.StartupPath + "\\HardwareConfig.ini", "MitutoyoConfig4", "PortName", "COM12");//测厚4  COM
            int ThicknessBaud4 = int.Parse(FilesManager.GetPrivateProfileString(Application.StartupPath + "\\HardwareConfig.ini", "MitutoyoConfig4", "BaudRate", "2400"));//测厚4  波特率
            string ThicknessData4 = FilesManager.GetPrivateProfileString(Application.StartupPath + "\\HardwareConfig.ini", "MitutoyoConfig4", "DataBits", "8");//测厚4  数据位
            string ThicknessStop4 = FilesManager.GetPrivateProfileString(Application.StartupPath + "\\HardwareConfig.ini", "MitutoyoConfig4", "StopBits", "1");//测厚4  停止位
            string ThicknessParity4 = FilesManager.GetPrivateProfileString(Application.StartupPath + "\\HardwareConfig.ini", "MitutoyoConfig4", "Parity", "0");//测厚4  校验值
            string MitutoyoReaderTypes4 = FilesManager.GetPrivateProfileString(Application.StartupPath + "\\HardwareConfig.ini", "MitutoyoConfig4", "MitutoyoReaderTypes", "MitutoyoSerial");//测厚4  型号
            comm = Common.lstComm.FirstOrDefault(o => o.Name.Equals("厚度测试4"));
            if (comm == null)
            {
                comm = new Communication();
            }
            comm.Factory = "ATL";
            comm.Code = "串口";
            comm.Name = "厚度测试4";
            comm.ComPort = ThicknessCOM4;
            comm.ComBaudRate = ThicknessBaud4;
            comm.ComStopBits = ThicknessStop4;
            comm.ComParity = ThicknessParity4;
            comm.CommunicationTypeName = MitutoyoReaderTypes4;
            if (!commfDAL.IsExists(" Name ='厚度测试4'", ref errmsg))
            {
                commfDAL.Add(comm, ref errmsg);
            }
            else
            {
                commfDAL.Update(comm, ref errmsg);
            }

            string BT3562COM = FilesManager.GetPrivateProfileString(Application.StartupPath + "\\HardwareConfig.ini", "BT3562Config", "PortName", "COM7");//
            int BT3562BaudRate = int.Parse(FilesManager.GetPrivateProfileString(Application.StartupPath + "\\HardwareConfig.ini", "BT3562Config", "BaudRate", "9600"));//
            string BT3562DataBits = FilesManager.GetPrivateProfileString(Application.StartupPath + "\\HardwareConfig.ini", "BT3562Config", "DataBits", "8");//
            string BT3562StopBits = FilesManager.GetPrivateProfileString(Application.StartupPath + "\\HardwareConfig.ini", "BT3562Config", "StopBits", "1");//
            string BT3562Parity = FilesManager.GetPrivateProfileString(Application.StartupPath + "\\HardwareConfig.ini", "BT3562Config", "Parity", "0");//
            comm = Common.lstComm.FirstOrDefault(o => o.Name.Equals("3562"));
            if (comm == null)
            {
                comm = new Communication();
            }
            comm.Factory = "ATL";
            comm.Code = "串口";
            comm.Name = "3562";
            comm.ComPort = BT3562COM;
            comm.ComBaudRate = BT3562BaudRate;
            comm.ComStopBits = BT3562StopBits;
            comm.ComParity = BT3562Parity;
            comm.CommunicationTypeName = "3562";
            if (!commfDAL.IsExists(" Name ='3562'", ref errmsg))
            {
                commfDAL.Add(comm, ref errmsg);
            }
            else
            {
                commfDAL.Update(comm, ref errmsg);
            }

            string E5CCCOM = FilesManager.GetPrivateProfileString(Application.StartupPath + "\\HardwareConfig.ini", "E5CCConfig", "PortName", "COM13");//
            int E5CCBaudRate = int.Parse(FilesManager.GetPrivateProfileString(Application.StartupPath + "\\HardwareConfig.ini", "E5CCConfig", "BaudRate", "9600"));//
            string E5CCDataBits = FilesManager.GetPrivateProfileString(Application.StartupPath + "\\HardwareConfig.ini", "E5CCConfig", "DataBits", "8");//
            string E5CCStopBits = FilesManager.GetPrivateProfileString(Application.StartupPath + "\\HardwareConfig.ini", "E5CCConfig", "StopBits", "1");//
            string E5CCParity = FilesManager.GetPrivateProfileString(Application.StartupPath + "\\HardwareConfig.ini", "E5CCConfig", "Parity", "2");//
            comm = Common.lstComm.FirstOrDefault(o => o.Name.Equals("E5环境温度"));
            if (comm == null)
            {
                comm = new Communication();
            }
            comm.Factory = "ATL";
            comm.Code = "串口";
            comm.Name = "E5环境温度";
            comm.ComPort = E5CCCOM;
            comm.ComBaudRate = E5CCBaudRate;
            comm.ComStopBits = E5CCStopBits;
            comm.ComParity = E5CCParity;
            comm.CommunicationTypeName = "E5环境温度";
            if (!commfDAL.IsExists(" Name ='E5环境温度'", ref errmsg))
            {
                commfDAL.Add(comm, ref errmsg);
            }
            else
            {
                commfDAL.Update(comm, ref errmsg);
            }



            string Ip34461A = FilesManager.GetPrivateProfileString(Application.StartupPath + "\\HardwareConfig.ini", "Ip34461AConfig", "Ip34461A", "192.168.106.2");//
            comm = Common.lstComm.FirstOrDefault(o => o.Name.Equals("34461"));
            if (comm == null)
            {
                comm = new Communication();
            }
            comm.Factory = "ATL";
            comm.Code = "socket";
            comm.Name = "34461";
            comm.ComPort = "";
            comm.Ip = Ip34461A;
            comm.IpPort = 0;
            comm.CommunicationTypeName = "34461";
            if (!commfDAL.IsExists(" Name ='34461'", ref errmsg))
            {
                commfDAL.Add(comm, ref errmsg);
            }
            else
            {
                commfDAL.Update(comm, ref errmsg);
            }

            string IpToprie = FilesManager.GetPrivateProfileString(Application.StartupPath + "\\HardwareConfig.ini", "IpToprieConfig", "IpToprie", "192.168.150.80");//
            int PortToprie = int.Parse(FilesManager.GetPrivateProfileString(Application.StartupPath + "\\HardwareConfig.ini", "IpToprieConfig", "PortToprie", "3000"));//

            comm = Common.lstComm.FirstOrDefault(o => o.Name.Equals("Toprie"));
            if (comm == null)
            {
                comm = new Communication();
            }
            comm.Factory = "ATL";
            comm.Code = "socket";
            comm.Name = "Toprie";
            comm.ComPort = "COM1";
            comm.Ip = IpToprie;
            comm.IpPort = PortToprie;
            comm.CommunicationTypeName = "Toprie";
            if (!commfDAL.IsExists(" Name ='Toprie'", ref errmsg))
            {
                commfDAL.Add(comm, ref errmsg);
            }
            string IpToprie2 = FilesManager.GetPrivateProfileString(Application.StartupPath + "\\HardwareConfig.ini", "IpToprieConfig", "IpToprie2", "127.0.0.1");//
            int PortToprie2 = int.Parse(FilesManager.GetPrivateProfileString(Application.StartupPath + "\\HardwareConfig.ini", "IpToprieConfig", "PortToprie2", "3000"));//
            comm = Common.lstComm.FirstOrDefault(o => o.Name.Equals("Toprie2"));
            if (comm == null)
            {
                comm = new Communication();
            }
            comm.Factory = "ATL";
            comm.Code = "socket";
            comm.Name = "Toprie2";
            comm.ComPort = "COM1";
            comm.Ip = IpToprie2;
            comm.IpPort = PortToprie2;
            comm.CommunicationTypeName = "Toprie2";
            if (!commfDAL.IsExists(" Name ='Toprie2'", ref errmsg))
            {
                commfDAL.Add(comm, ref errmsg);
            }
            else
            {
                commfDAL.Update(comm, ref errmsg);
            }
            string LR8450 = FilesManager.GetPrivateProfileString(Application.StartupPath + "\\HardwareConfig.ini", "LR8401Config", "IpLR8401", "192.168.107.2");//
            comm = Common.lstComm.FirstOrDefault(o => o.Name.Equals("LR8450"));
            if (comm == null)
            {
                comm = new Communication();
            }
            comm.Factory = "ATL";
            comm.Code = "socket";
            comm.Name = "LR8450";
            comm.ComPort = "";
            comm.Ip = LR8450;
            comm.IpPort = 0;
            comm.CommunicationTypeName = "LR8450";
            if (!commfDAL.IsExists(" Name ='LR8450'", ref errmsg))
            {
                commfDAL.Add(comm, ref errmsg);
            }
            else
            {
                commfDAL.Update(comm, ref errmsg);
            }


            string MI3C1COM = FilesManager.GetPrivateProfileString(Application.StartupPath + "\\HardwareConfig.ini", "MI3Config1", "PortName", "COM14");//
            int MI3C1BaudRate = int.Parse(FilesManager.GetPrivateProfileString(Application.StartupPath + "\\HardwareConfig.ini", "MI3Config1", "BaudRate", "9600"));//
            string MI3C1DataBits = FilesManager.GetPrivateProfileString(Application.StartupPath + "\\HardwareConfig.ini", "MI3Config1", "DataBits", "8");//
            string MI3C1StopBits = FilesManager.GetPrivateProfileString(Application.StartupPath + "\\HardwareConfig.ini", "MI3Config1", "StopBits", "1");//
            string MI3C1Parity = FilesManager.GetPrivateProfileString(Application.StartupPath + "\\HardwareConfig.ini", "MI3Config1", "Parity", "0");//
            comm = Common.lstComm.FirstOrDefault(o => o.Name.Equals("MI3C1测温仪"));
            if (comm == null)
            {
                comm = new Communication();
            }
            comm.Factory = "ATL";
            comm.Code = "串口";
            comm.Name = "MI3C1测温仪";
            comm.ComPort = MI3C1COM;
            comm.ComBaudRate = MI3C1BaudRate;
            comm.ComStopBits = MI3C1StopBits;
            comm.ComParity = MI3C1Parity;
            comm.CommunicationTypeName = "MI3C1测温仪";
            if (!commfDAL.IsExists(" Name ='MI3C1测温仪'", ref errmsg))
            {
                commfDAL.Add(comm, ref errmsg);
            }
            else
            {
                commfDAL.Update(comm, ref errmsg);
            }

            string MI3C2 = FilesManager.GetPrivateProfileString(Application.StartupPath + "\\HardwareConfig.ini", "MI3Config2", "IpLR8401", "192.168.107.2");//
            int MI3C2BaudRate = int.Parse(FilesManager.GetPrivateProfileString(Application.StartupPath + "\\HardwareConfig.ini", "MI3Config2", "BaudRate", "9600"));//
            string MI3C2DataBits = FilesManager.GetPrivateProfileString(Application.StartupPath + "\\HardwareConfig.ini", "MI3Config2", "DataBits", "8");//
            string MI3C2StopBits = FilesManager.GetPrivateProfileString(Application.StartupPath + "\\HardwareConfig.ini", "MI3Config2", "StopBits", "1");//
            string MI3C2Parity = FilesManager.GetPrivateProfileString(Application.StartupPath + "\\HardwareConfig.ini", "MI3Config2", "Parity", "0");//
            comm = Common.lstComm.FirstOrDefault(o => o.Name.Equals("MI3C2测温仪"));
            if (comm == null)
            {
                comm = new Communication();
            }
            comm.Factory = "ATL";
            comm.Code = "串口";
            comm.Name = "MI3C2测温仪";
            comm.ComPort = MI3C2;
            comm.ComBaudRate = MI3C2BaudRate;
            comm.ComStopBits = MI3C2DataBits;
            comm.ComParity = MI3C2Parity;
            comm.CommunicationTypeName = "MI3C2测温仪";
            if (!commfDAL.IsExists(" Name ='MI3C2测温仪'", ref errmsg))
            {
                commfDAL.Add(comm, ref errmsg);
            }
            else
            {
                commfDAL.Update(comm, ref errmsg);
            }
            string MI3CC1 = FilesManager.GetPrivateProfileString(Application.StartupPath + "\\HardwareConfig.ini", "MI3CConfig1", "IpLR8401", "192.168.107.2");//
            int MI3CC1BaudRate = int.Parse(FilesManager.GetPrivateProfileString(Application.StartupPath + "\\HardwareConfig.ini", "MI3CConfig1", "BaudRate", "9600"));//
            string MI3CC1DataBits = FilesManager.GetPrivateProfileString(Application.StartupPath + "\\HardwareConfig.ini", "MI3CConfig1", "DataBits", "8");//
            string MI3CC1StopBits = FilesManager.GetPrivateProfileString(Application.StartupPath + "\\HardwareConfig.ini", "MI3CConfig1", "StopBits", "1");//
            string MI3CC1Parity = FilesManager.GetPrivateProfileString(Application.StartupPath + "\\HardwareConfig.ini", "MI3CConfig1", "Parity", "0");//
            comm = Common.lstComm.FirstOrDefault(o => o.Name.Equals("MI3CC1测温仪"));
            if (comm == null)
            {
                comm = new Communication();
            }
            comm.Factory = "ATL";
            comm.Code = "串口";
            comm.Name = "MI3CC1测温仪";
            comm.ComPort = MI3CC1;
            comm.ComBaudRate = MI3CC1BaudRate;
            comm.ComStopBits = MI3CC1StopBits;
            comm.ComParity = MI3CC1Parity;
            comm.CommunicationTypeName = "MI3CC1测温仪";
            if (!commfDAL.IsExists(" Name ='MI3CC1测温仪'", ref errmsg))
            {
                commfDAL.Add(comm, ref errmsg);
            }
            else
            {
                commfDAL.Update(comm, ref errmsg);
            }
            string MI3CC2 = FilesManager.GetPrivateProfileString(Application.StartupPath + "\\HardwareConfig.ini", "MI3CConfig2", "IpLR8401", "192.168.107.2");//
            int MI3CC2BaudRate = int.Parse(FilesManager.GetPrivateProfileString(Application.StartupPath + "\\HardwareConfig.ini", "MI3CConfig2", "BaudRate", "9600"));//
            string MI3CC2DataBits = FilesManager.GetPrivateProfileString(Application.StartupPath + "\\HardwareConfig.ini", "MI3CConfig2", "DataBits", "8");//
            string MI3CC2StopBits = FilesManager.GetPrivateProfileString(Application.StartupPath + "\\HardwareConfig.ini", "MI3CConfig2", "StopBits", "1");//
            string MI3CC2Parity = FilesManager.GetPrivateProfileString(Application.StartupPath + "\\HardwareConfig.ini", "MI3CConfig2", "Parity", "0");//

            comm = Common.lstComm.FirstOrDefault(o => o.Name.Equals("MI3CC2测温仪"));
            if (comm == null)
            {
                comm = new Communication();
            }
            comm.Factory = "ATL";
            comm.Code = "串口";
            comm.Name = "MI3CC2测温仪";
            comm.ComPort = MI3CC2;
            comm.ComBaudRate = MI3CC2BaudRate;
            comm.ComStopBits = MI3CC2StopBits;
            comm.ComParity = MI3CC2Parity;
            comm.CommunicationTypeName = "MI3CC2测温仪";
            if (!commfDAL.IsExists(" Name ='MI3CC2测温仪'", ref errmsg))
            {
                commfDAL.Add(comm, ref errmsg);
            }
            else
            {
                commfDAL.Update(comm, ref errmsg);
            }


            WaitDialogForm waitFrm = new WaitDialogForm("正在初始化数据...", "系统提示");
            try
            {

                //if (Common.IsDynamicColumn)//动态创建列头
                //{
                //    PermissionBLL.CreateXtraGrid(gridView5, Convert.ToString(this.Tag), ref errMsg, gridControl5);
                //    if (!string.IsNullOrEmpty(errMsg))
                //    {
                //        frmTip.MessageBox(errMsg, Common.GetMessage("STRING_SYSTEM_PROMPT"));
                //        return;
                //    }
                //}


                //gridView5.ActiveFilterString = $"Factory = 'ATL' And IsUse = True";

                gridColumn1.SortMode = DevExpress.XtraGrid.ColumnSortMode.Custom;
                gridColumn1.SortOrder = DevExpress.Data.ColumnSortOrder.Ascending;

                SetPortNameValues(comboBoxEdit1);
                SetParityValues(comboBoxEdit2);
                SetStopBitValues(comboBoxEdit3);

                lstSocketTcpType = Common.EnumExtentionsToList<Common.EnumSocketTcpType>();
                lookUpEdit1.Properties.DataSource = lstSocketTcpType;
                lookUpEdit1.Properties.ValueMember = "EnumValue";
                lookUpEdit1.Properties.DisplayMember = "Desction";

                lookUpEdit1.Properties.Columns.Add(new LookUpColumnInfo("Desction", 120, "通讯名称"));  //通讯名称
                lookUpEdit1.Properties.Columns.Add(new LookUpColumnInfo("EnumName", 180, "通讯编码")); //通讯编码
                lookUpEdit1.Properties.Columns.Add(new LookUpColumnInfo("EnumValue", 60, "通讯值")); //通讯值
                lookUpEdit1.Properties.PopupWidth = 450;

                lstTestingBrand = Common.EnumExtentionsToList<Common.EnumInstrumentBrand>();
                lookUpEdit3.Properties.DataSource = lstTestingBrand;
                lookUpEdit3.Properties.ValueMember = "EnumValue";
                lookUpEdit3.Properties.DisplayMember = "Desction";

                lookUpEdit3.Properties.Columns.Add(new LookUpColumnInfo("Desction", 150, "通讯名称"));//通讯名称
                lookUpEdit3.Properties.Columns.Add(new LookUpColumnInfo("EnumName", 150, "通讯编码")); //通讯编码
                lookUpEdit3.Properties.Columns.Add(new LookUpColumnInfo("EnumValue", 60, "通讯值"));//通讯值
                lookUpEdit3.Properties.PopupWidth = 400;



                lsCommunicationType = Common.EnumExtentionsToList<Common.EnumCommunicationType>();
                lookUpEdit4.Properties.DataSource = lsCommunicationType;
                lookUpEdit4.Properties.ValueMember = "EnumValue";
                lookUpEdit4.Properties.DisplayMember = "Desction";

                lookUpEdit4.Properties.Columns.Add(new LookUpColumnInfo("Desction", 120, "通讯名称")); //通讯名称
                lookUpEdit4.Properties.Columns.Add(new LookUpColumnInfo("EnumName", 180, "通讯编码"));//通讯编码
                lookUpEdit4.Properties.Columns.Add(new LookUpColumnInfo("EnumValue", 60, "通讯值")); //通讯值

                lookUpEdit4.Properties.PopupWidth = 450;



                var lsEnumEncoding = Common.EnumExtentionsToList<Common.EnumEncoding>();
                lookUpEdit5.Properties.DataSource = lsEnumEncoding;
                lookUpEdit5.Properties.ValueMember = "EnumValue";
                lookUpEdit5.Properties.DisplayMember = "Desction";

                lookUpEdit5.Properties.Columns.Add(new LookUpColumnInfo("Desction", 120, "通讯名称")); //通讯名称
                lookUpEdit5.Properties.Columns.Add(new LookUpColumnInfo("EnumName", 180, "通讯编码")); //通讯编码
                lookUpEdit5.Properties.Columns.Add(new LookUpColumnInfo("EnumValue", 60, "通讯值"));//通讯值

                lookUpEdit5.Properties.PopupWidth = 450;


                DataRefresh();


                //string filters = string.Empty;

                //filters = $"RefKey = 'Communication' and {Common.SqlIsNullKey}RefIsUse = 1";

                //var listRef = dictionaryRefDAL.GetList(filters, ref errMsg);

                //DataSourceLookUpEdit(luSendCommand1, listRef, "SendCommand1");
                //DataSourceLookUpEdit(luSendCommand2, listRef, "SendCommand2");
                //DataSourceLookUpEdit(luSendCommand3, listRef, "SendCommand3");
                //DataSourceLookUpEdit(luSendCommand4, listRef, "SendCommand4");
                //DataSourceLookUpEdit(luSendCommand5, listRef, "SendCommand5");
                //DataSourceLookUpEdit(luSendCommand6, listRef, "SendCommand6");
                //DataSourceLookUpEdit(luSendCommand7, listRef, "SendCommand7");
                //DataSourceLookUpEdit(luSendCommand8, listRef, "SendCommand8");

            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(ex.ToString());
            }
            finally
            {
                waitFrm.Close();
            }
        }












        private List<ZY.Systems.Enums.EnumberEntity> lstSocketTcpType = new List<ZY.Systems.Enums.EnumberEntity>();
        private List<ZY.Systems.Enums.EnumberEntity> lstTestingBrand = new List<ZY.Systems.Enums.EnumberEntity>();
        private List<ZY.Systems.Enums.EnumberEntity> lsCommunicationType = new List<ZY.Systems.Enums.EnumberEntity>();
        private bool isAdd = false;
        string ipBefore = string.Empty;
        int rowIndex = 0;
        private void bbiModify_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

            int row = gridView5.FocusedRowHandle;
            if (row >= 0)
            {
                rowIndex = row;
                try
                {
                    isAdd = false;
                    textEdit3.Tag = gridView5.GetRowCellValue(row, "Iden");
                    lookUpEdit2.EditValue = gridView5.GetRowCellValue(row, "StrCode");
                    lookUpEdit2.Tag = gridView5.GetRowCellValue(row, "Code");

                    textEdit3.EditValue = gridView5.GetRowCellValue(row, "Name");
                    textEdit5.EditValue = gridView5.GetRowCellValue(row, "Ip");
                    ipBefore = textEdit5.Text;
                    spinEdit2.Value = string.IsNullOrEmpty(Convert.ToString(gridView5.GetRowCellValue(row, "IpPort"))) ? 0 : Convert.ToInt32(gridView5.GetRowCellValue(row, "IpPort"));
                    spinEdit3.Value = string.IsNullOrEmpty(Convert.ToString(gridView5.GetRowCellValue(row, "IpLocalPort"))) ? 0 : Convert.ToInt32(gridView5.GetRowCellValue(row, "IpLocalPort"));
                    spinEdit1.Value = string.IsNullOrEmpty(Convert.ToString(gridView5.GetRowCellValue(row, "IpLocalPortNum"))) ? 0 : Convert.ToInt32(gridView5.GetRowCellValue(row, "IpLocalPortNum"));

                    spinEdit5.EditValue = string.IsNullOrEmpty(Convert.ToString(gridView5.GetRowCellValue(row, "ScanCount"))) ? 0 : Convert.ToInt32(gridView5.GetRowCellValue(row, "ScanCount"));
                    spinEdit4.EditValue = string.IsNullOrEmpty(Convert.ToString(gridView5.GetRowCellValue(row, "ScanInterval"))) ? 0 : Convert.ToInt32(gridView5.GetRowCellValue(row, "ScanInterval"));
                    spinEdit6.EditValue = string.IsNullOrEmpty(Convert.ToString(gridView5.GetRowCellValue(row, "ReadDelay"))) ? 1 : Convert.ToInt32(gridView5.GetRowCellValue(row, "ReadDelay"));

                    cbeNXConnType.EditValue = gridView5.GetRowCellValue(row, "NXConnType");


                    comboBoxEdit1.EditValue = gridView5.GetRowCellValue(row, "ComPort");

                    comboBoxEdit5.EditValue = gridView5.GetRowCellValue(row, "ComBaudRate");
                    comboBoxEdit2.EditValue = gridView5.GetRowCellValue(row, "ComParity");
                    comboBoxEdit3.EditValue = gridView5.GetRowCellValue(row, "ComStopBits");
                    comboBoxEdit4.EditValue = gridView5.GetRowCellValue(row, "ComDataBits");

                    luSendCommand1.EditValue = gridView5.GetRowCellValue(row, "SendCommand1");
                    luSendCommand2.EditValue = gridView5.GetRowCellValue(row, "SendCommand2");
                    luSendCommand3.EditValue = gridView5.GetRowCellValue(row, "SendCommand3");
                    luSendCommand4.EditValue = gridView5.GetRowCellValue(row, "SendCommand4");
                    luSendCommand5.EditValue = gridView5.GetRowCellValue(row, "SendCommand5");
                    luSendCommand6.EditValue = gridView5.GetRowCellValue(row, "SendCommand6");
                    luSendCommand7.EditValue = gridView5.GetRowCellValue(row, "SendCommand7");
                    luSendCommand8.EditValue = gridView5.GetRowCellValue(row, "SendCommand8");

                    luSendCommand1_remark.EditValue = gridView5.GetRowCellValue(row, "SendCommand1Remark");
                    luSendCommand2_remark.EditValue = gridView5.GetRowCellValue(row, "SendCommand2Remark");
                    luSendCommand3_remark.EditValue = gridView5.GetRowCellValue(row, "SendCommand3Remark");
                    luSendCommand4_remark.EditValue = gridView5.GetRowCellValue(row, "SendCommand4Remark");
                    luSendCommand5_remark.EditValue = gridView5.GetRowCellValue(row, "SendCommand5Remark");
                    luSendCommand6_remark.EditValue = gridView5.GetRowCellValue(row, "SendCommand6Remark");
                    luSendCommand7_remark.EditValue = gridView5.GetRowCellValue(row, "SendCommand7Remark");
                    luSendCommand8_remark.EditValue = gridView5.GetRowCellValue(row, "SendCommand8Remark");


                    checkEdit2.Checked = Convert.ToBoolean(gridView5.GetRowCellValue(row, "IsEnter"));
                    checkEdit1.Checked = Convert.ToBoolean(gridView5.GetRowCellValue(row, "IsLine"));
                    checkEdit3.Checked = Convert.ToBoolean(gridView5.GetRowCellValue(row, "IsByte"));
                    checkEdit4.Checked = Convert.ToBoolean(gridView5.GetRowCellValue(row, "IsHex"));
                    checkEdit5.Checked = Convert.ToBoolean(gridView5.GetRowCellValue(row, "IsCRC"));
                    chkIsClose.Checked = Convert.ToBoolean(gridView5.GetRowCellValue(row, "IsClose"));

                    checkEdit8.Checked = Convert.ToBoolean(gridView5.GetRowCellValue(row, "IsSocketTcp"));
                    checkEdit7.Checked = Convert.ToBoolean(gridView5.GetRowCellValue(row, "IsSocket"));

                    chkIsCheck.Checked = Convert.ToBoolean(gridView5.GetRowCellValue(row, "IsCheck"));
                    chkIsLock.Checked = Convert.ToBoolean(gridView5.GetRowCellValue(row, "IsLock"));


                    chkIsLoading.Checked = Convert.ToBoolean(gridView5.GetRowCellValue(row, "IsLoading"));
                    chkIsUnLoading.Checked = Convert.ToBoolean(gridView5.GetRowCellValue(row, "IsUnLoading"));


                    chkSFC.Checked = Convert.ToBoolean(gridView5.GetRowCellValue(row, "IsFCS"));

                    chkIsLog.Checked = Convert.ToBoolean(gridView5.GetRowCellValue(row, "IsWriteLog"));
                    checkEdit9.Checked = Convert.ToBoolean(gridView5.GetRowCellValue(row, "IsClearCacheBeforeRead"));



                    checkEdit6.Checked = Convert.ToBoolean(gridView5.GetRowCellValue(row, "IsUse"));

                    seSocketTimeOut.Text = Convert.ToString(gridView5.GetRowCellValue(row, "SocketTimeOut"));
                    textEdit8.Text = Convert.ToString(gridView5.GetRowCellValue(row, "Factory"));
                    textEdit8.Text = string.IsNullOrEmpty(textEdit8.Text) ? "ATL" : textEdit8.Text;

                    var machineID = gridView5.GetRowCellValue(row, "MachineID");
                    if (string.IsNullOrEmpty(Convert.ToString(machineID)) || machineID.Equals("0"))
                    {
                        if (ccboMachine.Properties.Items.Count > 0)
                            machineID = Convert.ToString(ccboMachine.Properties.Items[0].Value);
                    }
                    ccboMachine.SetEditValue(machineID);


                    var socketTcpID = gridView5.GetRowCellValue(row, "SocketTcpType");


                    lookUpEdit1.EditValue = string.IsNullOrEmpty(Convert.ToString(socketTcpID)) ? 0 : Convert.ToInt32(socketTcpID);

                    var TestingBrandID = gridView5.GetRowCellValue(row, "InstrumentBrand");

                    lookUpEdit3.EditValue = string.IsNullOrEmpty(Convert.ToString(TestingBrandID)) ? 0 : Convert.ToInt32(TestingBrandID);

                    var CommunicationTypeID = gridView5.GetRowCellValue(row, "CommunicationType");


                    lookUpEdit4.EditValue = string.IsNullOrEmpty(Convert.ToString(CommunicationTypeID)) ? 0 : Convert.ToInt32(CommunicationTypeID);




                    checkEdit6.Checked = Convert.ToBoolean(gridView5.GetRowCellValue(row, "IsUse"));

                    seClearTime.EditValue = string.IsNullOrEmpty(Convert.ToString(gridView5.GetRowCellValue(row, "ClearDelayTime"))) ? 100 : Convert.ToInt32(gridView5.GetRowCellValue(row, "ClearDelayTime"));
                    seCommBfTime.EditValue = string.IsNullOrEmpty(Convert.ToString(gridView5.GetRowCellValue(row, "CommDelayBfTime"))) ? 1 : Convert.ToInt32(gridView5.GetRowCellValue(row, "CommDelayBfTime"));
                    seCommIngTime.EditValue = string.IsNullOrEmpty(Convert.ToString(gridView5.GetRowCellValue(row, "CommDelayIngTime"))) ? 1 : Convert.ToInt32(gridView5.GetRowCellValue(row, "CommDelayIngTime"));
                    seCommTime1.EditValue = string.IsNullOrEmpty(Convert.ToString(gridView5.GetRowCellValue(row, "CommDelayTime1"))) ? 1 : Convert.ToInt32(gridView5.GetRowCellValue(row, "CommDelayTime1"));
                    seDeviedVal.EditValue = string.IsNullOrEmpty(Convert.ToString(gridView5.GetRowCellValue(row, "DeviedVal"))) ? 1 : Convert.ToInt32(gridView5.GetRowCellValue(row, "DeviedVal"));

                    seByteLen.EditValue = string.IsNullOrEmpty(Convert.ToString(gridView5.GetRowCellValue(row, "EndByteLen"))) ? 0 : Convert.ToInt32(gridView5.GetRowCellValue(row, "EndByteLen"));
                    seStrLen.EditValue = string.IsNullOrEmpty(Convert.ToString(gridView5.GetRowCellValue(row, "EndStrLen"))) ? 0 : Convert.ToInt32(gridView5.GetRowCellValue(row, "EndStrLen"));

                    checkEdit10.Checked = Convert.ToBoolean(gridView5.GetRowCellValue(row, "IsRecPlc"));
                    seBufferSize.Value = Convert.ToInt32(gridView5.GetRowCellValue(row, "BufferSize") ?? "1");

                    var EncodingId = gridView5.GetRowCellValue(row, "Encoding");

                    lookUpEdit5.EditValue = string.IsNullOrEmpty(Convert.ToString(EncodingId)) ? 1 : Convert.ToInt32(EncodingId);

                    SetEnabled(true);

                }
                catch (Exception ex)
                {
                    XtraMessageBox.Show(ex.ToString());

                }
            }
            else
            {
                XtraMessageBox.Show("请选择需要修改的数据");
            }
        }
        private void SetEnabled(bool iFalg)
        {
            bbiCancel.Enabled = iFalg;
            bbiSave.Enabled = iFalg;
            bbiRefresh.Enabled = !iFalg;
            bbiAddNew.Enabled = !iFalg;
            bbiModify.Enabled = !iFalg;
            bbiDelete.Enabled = !iFalg;
            bbiInsert.Enabled = !iFalg;
            panel1.Visible = iFalg;
            gridControl5.Enabled = !iFalg;

        }
        private void bbiInsert_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

        }

        private void bbiSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //try
            //{
            var model = GetModel();
            commcation = model;
            string errMsg = string.Empty;
            //if ((Common.EnumCommunicationType)model.CommunicationType.GetValueOrDefault(0) == Common.EnumCommunicationType.SocketTcpServer
            //    || (Common.EnumCommunicationType)model.CommunicationType.GetValueOrDefault(0) == Common.EnumCommunicationType.SocketUDPServer)
            //{

            //    var listIp = OSClass.GetLocalListIp();
            //    if (!listIp.Exists(o => o.Equals(model.Ip)))
            //    {
            //        XtraMessageBox.Show($"服务器设置的IP[{model.Ip}]在本地网络中不存在,请检查Ip设置是否正确!");
            //        return;
            //    }
            //}
            if (isAdd)
            {
                CommunicationBLL.Add(model, ref errMsg);
            }
            else
            {
                //if (!string.IsNullOrEmpty(ipBefore)) {
                //    string ipBeforeSection = ipBefore.Split(new char[] { '.' })[2];

                //    if (!string.IsNullOrEmpty(textEdit5.Text)) {
                //        string ipAfterSection = textEdit5.Text.Split(new char[] { '.' })[2];
                //        if (ipBeforeSection != ipAfterSection) {
                //            if (XtraMessageBox.Show(Common.GetMessage($"STRING_FRMCOMMUNICATION_CHANGEIPQUESTION", ipAfterSection), Common.GetMessage("STRING_SYSTEM_PROMPT"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes) {
                //                CommunicationBLL.UpdateIpSection(ipBeforeSection, ipAfterSection, ref errMsg);
                //            } else {
                //                BLL.CommunicationBLL.Update(model, ref errMsg);
                //            }
                //        } else
                //            BLL.CommunicationBLL.Update(model, ref errMsg);
                //    } else {
                //        BLL.CommunicationBLL.Update(model, ref errMsg);
                //    }
                //} else {
                CommunicationBLL.Update(model, ref errMsg);
                //  }
            }


            //    if (string.IsNullOrEmpty(errMsg))
            //    {
            //        if (Common.lstComm != null)
            //        {
            //            var m = Common.lstComm.FirstOrDefault(o => o.Code.Equals(model.Code));
            //            if (m != null)
            //            {
            //                m.IsWriteLog = model.IsWriteLog;
            //                m.IsSocketTcp = model.IsSocketTcp;
            //                m.IsRecPlc = model.IsRecPlc;
            //            }
            //        }

            //        if (Common.lstTcp != null)
            //        {
            //            var tcp = Common.lstTcp.FirstOrDefault(o => o.keyTcp == (Common.EnumSocketTcpType)model.SocketTcpType.GetValueOrDefault(0)
            //                                                     && o.commType == (Common.EnumCommunicationType)model.CommunicationType
            //                                                     && o.Name == model.Name);

            //            if (tcp != null)
            //            {
            //                switch ((Common.EnumCommunicationType)model.CommunicationType)
            //                {
            //                    case Common.EnumCommunicationType.NXCIP:
            //                        if (tcp.socketNxCip != null)
            //                        {
            //                            tcp.socketNxCip.IsWriteLog = model.IsWriteLog;
            //                            tcp.socketNxCip.IsRecPlc = model.IsRecPlc.GetValueOrDefault(false);
            //                        }
            //                        break;
            //                    case Common.EnumCommunicationType.ModbusTCP:
            //                        if (tcp.socketModbus != null)
            //                        {
            //                            tcp.socketModbus.IsWriteLog = model.IsWriteLog;
            //                            tcp.socketModbus.IsRecPlc = model.IsRecPlc.GetValueOrDefault(false);

            //                        }
            //                        break;
            //                    case Common.EnumCommunicationType.Twincat:
            //                        if (tcp.socketAds != null)
            //                        {
            //                            tcp.socketAds.IsWriteLog = model.IsWriteLog;
            //                            tcp.socketAds.IsRecPlc = model.IsRecPlc.GetValueOrDefault(false);
            //                        }
            //                        break;
            //                    //case Common.CommunicationType.SiemensS7:
            //                    //    if (tcp. != null)
            //                    //        tcp.socketTcp.IsWriteLog = model.IsWriteLog;
            //                    //    break;
            //                    default:
            //                        if (tcp.socketTcp != null)
            //                        {
            //                            tcp.socketTcp.IsWriteLog = model.IsWriteLog;
            //                            tcp.socketTcp.IsSocketTcp = model.IsSocketTcp;
            //                            tcp.socketTcp.IsRecPlc = model.IsRecPlc.GetValueOrDefault(false);
            //                        }
            //                        break;
            //                }
            //            }
            //        }
            DataRefresh();
            SetEnabled(false);
            gridView5.FocusedRowHandle = rowIndex;
            XtraMessageBox.Show("保存成功!");
            //    }
            //    else
            //    {
            //        XtraMessageBox.Show(errMsg);
            //    }
            //}
            //catch (Exception ex)
            //{

            //    XtraMessageBox.Show(ex.ToString());

            //}
        }

        private void DataRefresh()
        {
            string errMsg = string.Empty;
            isAdd = false;
            SetEnabled(false);
            var list = CommunicationBLL.GetList($"Factory='ATL' Or Factory Is null", "Code", ref errMsg);
            if (!string.IsNullOrEmpty(errMsg))
            {
                XtraMessageBox.Show(errMsg);
                return;
            }

            foreach (var m in list)
            {
                m.StrCode = m.Code + "-" + m.Name;

                //   m.SocketTcpTypeName = ((Common.EnumSocketTcpType)m.SocketTcpType.GetValueOrDefault(0)).ToString();

                var _modelSocketTcpTypeName = lstSocketTcpType.FirstOrDefault(o => o.EnumValue.Equals(m.SocketTcpType.GetValueOrDefault(0)));
                if (_modelSocketTcpTypeName != null)
                {
                    m.SocketTcpTypeName = _modelSocketTcpTypeName.Desction;
                }

                //m.CommunicationTypeName = ((Common.EnumCommunicationType)m.CommunicationType.GetValueOrDefault(0)).ToString();
                var _modelCommunicationType = lsCommunicationType.FirstOrDefault(o => o.EnumValue.Equals(m.InstrumentBrand));

                if (_modelCommunicationType != null)
                {
                    m.CommunicationTypeName = _modelCommunicationType.Desction;
                }
                var _modelBrand = lstTestingBrand.FirstOrDefault(o => o.EnumValue.Equals(m.InstrumentBrand));
                if (_modelBrand != null)
                {
                    m.InstrumentBrandName = _modelBrand.Desction;
                }
            }
            lookUpEdit2.Properties.DataSource = list;
            lookUpEdit2.Properties.ValueMember = "StrCode";
            lookUpEdit2.Properties.DisplayMember = "Code";

            lookUpEdit2.Properties.Columns.Clear();
            lookUpEdit2.Properties.Columns.Add(new LookUpColumnInfo("Iden", 120, "Iden"));
            lookUpEdit2.Properties.Columns.Add(new LookUpColumnInfo("Code", 120, "编码"));    //编码
            lookUpEdit2.Properties.Columns.Add(new LookUpColumnInfo("StrCode", 120, "编码")); //编码
            lookUpEdit2.Properties.Columns.Add(new LookUpColumnInfo("Name", 180, "名称")); //名称
            lookUpEdit2.Properties.Columns[0].Visible = false;
            lookUpEdit2.Properties.Columns[2].Visible = false;
            lookUpEdit2.Properties.PopupWidth = 450;

            gridControl5.DataSource = list;

            //var _listData = list.ToList().FindAll(o => !string.IsNullOrEmpty(o.Factory) && o.Factory.ToUpper().Equals("ATL"));
            //foreach (Communication _model in _listData)
            //{
            //    var Row = new DictionaryRef();
            //    if (!string.IsNullOrEmpty(_model.SendCommand1))
            //    {
            //        Row.RefKey = "Communication";
            //        Row.RefCode = "SendCommand1";
            //        Row.RefCode2 = _model.Code;
            //        Row.RefValue = _model.SendCommand1;
            //        Row.RefValue2 = _model.Name;
            //        Row.RefRemark = "PLC通讯配置";//PLC通讯配置;
            //        Row.RefIsUse = true;
            //        var isExi = DictionaryRefBLL.IsExists($"RefCode='SendCommand1' and  RefValue='{_model.SendCommand1}'", ref errMsg);
            //        if (!isExi)
            //            DictionaryRefBLL.Add(Row, ref errMsg);
            //    }

            //    if (!string.IsNullOrEmpty(_model.SendCommand2))
            //    {
            //        Row = new DictionaryRef();
            //        Row.RefKey = "Communication";
            //        Row.RefCode = "SendCommand2";
            //        Row.RefCode2 = _model.Code;
            //        Row.RefValue = _model.SendCommand2;
            //        Row.RefValue2 = _model.Name;
            //        Row.RefRemark = "PLC通讯配置";//PLC通讯配置;
            //        Row.RefIsUse = true;
            //        var isExi = DictionaryRefBLL.IsExists($"RefCode='SendCommand2' and  RefValue='{_model.SendCommand2}'", ref errMsg);
            //        if (!isExi)
            //            DictionaryRefBLL.Add(Row, ref errMsg);
            //    }

            //    if (!string.IsNullOrEmpty(_model.SendCommand3))
            //    {
            //        Row = new DictionaryRef();
            //        Row.RefKey = "Communication";
            //        Row.RefCode = "SendCommand3";
            //        Row.RefCode2 = _model.Code;
            //        Row.RefValue = _model.SendCommand3;
            //        Row.RefRemark = "PLC通讯配置";//PLC通讯配置;
            //        Row.RefValue2 = _model.Name;
            //        Row.RefIsUse = true;
            //        var isExi = DictionaryRefBLL.IsExists($"RefCode='SendCommand3' and  RefValue='{_model.SendCommand3}'", ref errMsg);
            //        if (!isExi)
            //            DictionaryRefBLL.Add(Row, ref errMsg);
            //    }

            //    if (!string.IsNullOrEmpty(_model.SendCommand4))
            //    {
            //        Row = new DictionaryRef();
            //        Row.RefKey = "Communication";
            //        Row.RefCode = "SendCommand4";
            //        Row.RefCode2 = _model.Code;
            //        Row.RefValue = _model.SendCommand4;
            //        Row.RefRemark = "PLC通讯配置";//PLC通讯配置;
            //        Row.RefValue2 = _model.Name;
            //        Row.RefIsUse = true;
            //        var isExi = DictionaryRefBLL.IsExists($"RefCode='SendCommand4' and   RefValue='{_model.SendCommand4}'", ref errMsg);
            //        if (!isExi)
            //            DictionaryRefBLL.Add(Row, ref errMsg);
            //    }
            //    if (!string.IsNullOrEmpty(_model.SendCommand5))
            //    {
            //        Row = new DictionaryRef();
            //        Row.RefKey = "Communication";
            //        Row.RefCode = "SendCommand5";
            //        Row.RefCode2 = _model.Code;
            //        Row.RefValue = _model.SendCommand5;
            //        Row.RefRemark = "PLC通讯配置";//PLC通讯配置;
            //        Row.RefValue2 = _model.Name;
            //        Row.RefIsUse = true;
            //        var isExi = DictionaryRefBLL.IsExists($"RefCode='SendCommand5' and  RefValue='{_model.SendCommand5}'", ref errMsg);
            //        if (!isExi)
            //            DictionaryRefBLL.Add(Row, ref errMsg);
            //    }
            //    if (!string.IsNullOrEmpty(_model.SendCommand6))
            //    {
            //        Row = new DictionaryRef();
            //        Row.RefKey = "Communication";
            //        Row.RefCode = "SendCommand6";
            //        Row.RefCode2 = _model.Code;
            //        Row.RefRemark = "PLC通讯配置";//PLC通讯配置;
            //        Row.RefValue = _model.SendCommand6;
            //        Row.RefValue2 = _model.Name;
            //        Row.RefIsUse = true;
            //        var isExi = DictionaryRefBLL.IsExists($"RefCode='SendCommand6' and  RefValue='{_model.SendCommand6}'", ref errMsg);
            //        if (!isExi)
            //            DictionaryRefBLL.Add(Row, ref errMsg);
            //    }
            //    if (!string.IsNullOrEmpty(_model.SendCommand7))
            //    {
            //        Row = new DictionaryRef();
            //        Row.RefKey = "Communication";
            //        Row.RefCode = "SendCommand7";
            //        Row.RefCode2 = _model.Code;
            //        Row.RefValue = _model.SendCommand7;
            //        Row.RefRemark = "PLC通讯配置";//PLC通讯配置;
            //        Row.RefValue2 = _model.Name;
            //        Row.RefIsUse = true;
            //        var isExi = DictionaryRefBLL.IsExists($"RefCode='SendCommand7' and RefValue='{_model.SendCommand7}'", ref errMsg);
            //        if (!isExi)
            //            DictionaryRefBLL.Add(Row, ref errMsg);
            //    }
            //    if (!string.IsNullOrEmpty(_model.SendCommand8))
            //    {
            //        Row = new DictionaryRef();
            //        Row.RefKey = "Communication";
            //        Row.RefCode = "SendCommand8";
            //        Row.RefCode2 = _model.Code;
            //        Row.RefRemark = "PLC通讯配置";//PLC通讯配置;
            //        Row.RefValue = _model.SendCommand8;
            //        Row.RefValue2 = _model.Name;
            //        Row.RefIsUse = true;
            //        var isExi = DictionaryRefBLL.IsExists($"RefCode='SendCommand8' and RefValue='{_model.SendCommand8}'", ref errMsg);
            //        if (!isExi)
            //            DictionaryRefBLL.Add(Row, ref errMsg);
            //    }
            //    if (!string.IsNullOrEmpty(_model.SendCommand9))
            //    {
            //        Row = new DictionaryRef();
            //        Row.RefKey = "Communication";
            //        Row.RefCode = "SendCommand9";
            //        Row.RefCode2 = _model.Code;
            //        Row.RefRemark = "PLC通讯配置";//PLC通讯配置;
            //        Row.RefValue = _model.SendCommand9;
            //        Row.RefValue2 = _model.Name;
            //        Row.RefIsUse = true;
            //        var isExi = DictionaryRefBLL.IsExists($"RefCode='SendCommand8' and RefValue='{_model.SendCommand9}'", ref errMsg);
            //        if (!isExi)
            //            DictionaryRefBLL.Add(Row, ref errMsg);
            //    }
            //    if (!string.IsNullOrEmpty(_model.SendCommand10))
            //    {
            //        Row = new DictionaryRef();
            //        Row.RefKey = "Communication";
            //        Row.RefCode = "SendCommand10";
            //        Row.RefCode2 = _model.Code;
            //        Row.RefRemark = "PLC通讯配置";//PLC通讯配置;
            //        Row.RefValue = _model.SendCommand10;
            //        Row.RefValue2 = _model.Name;
            //        Row.RefIsUse = true;
            //        var isExi = DictionaryRefBLL.IsExists($"RefCode='SendCommand10' and RefValue='{_model.SendCommand10}'", ref errMsg);
            //        if (!isExi)
            //            DictionaryRefBLL.Add(Row, ref errMsg);
            //    }
            //    if (!string.IsNullOrEmpty(_model.SendCommand11))
            //    {
            //        Row = new DictionaryRef();
            //        Row.RefKey = "Communication";
            //        Row.RefCode = "SendCommand11";
            //        Row.RefCode2 = _model.Code;
            //        Row.RefRemark = "PLC通讯配置";//PLC通讯配置;
            //        Row.RefValue = _model.SendCommand11;
            //        Row.RefValue2 = _model.Name;
            //        Row.RefIsUse = true;
            //        var isExi = DictionaryRefBLL.IsExists($"RefCode='SendCommand11' and RefValue='{_model.SendCommand11}'", ref errMsg);
            //        if (!isExi)
            //            DictionaryRefBLL.Add(Row, ref errMsg);
            //    }
            //    if (!string.IsNullOrEmpty(_model.SendCommand12))
            //    {
            //        Row = new DictionaryRef();
            //        Row.RefKey = "Communication";
            //        Row.RefCode = "SendCommand12";
            //        Row.RefCode2 = _model.Code;
            //        Row.RefRemark = "PLC通讯配置";//PLC通讯配置;
            //        Row.RefValue = _model.SendCommand12;
            //        Row.RefValue2 = _model.Name;
            //        Row.RefIsUse = true;
            //        var isExi = DictionaryRefBLL.IsExists($"RefCode='SendCommand12' and RefValue='{_model.SendCommand12}'", ref errMsg);
            //        if (!isExi)
            //            DictionaryRefBLL.Add(Row, ref errMsg);
            //    }
            //}


        }
        private Communication GetModel()
        {
            Communication model = new Communication();
            model.NXConnType = cbeNXConnType.Text;
            model.Iden = textEdit3.Tag == null ? -1 : Convert.ToInt32(textEdit3.Tag);
            model.Factory = "ATL";
            model.Code = Convert.ToString(lookUpEdit2.Tag);
            model.Name = textEdit3.Text.Trim();
            model.Ip = textEdit5.Text.Trim();
            model.IpPort = Convert.ToInt32(spinEdit2.Value);
            model.IpLocalPort = Convert.ToInt32(spinEdit3.Value);
            model.IpLocalPortNum = Convert.ToInt32(spinEdit1.Value);
            model.IsClose = chkIsClose.Checked;
            model.ComPort = comboBoxEdit1.Text;
            if (!string.IsNullOrEmpty(comboBoxEdit5.Text))
                model.ComBaudRate = Convert.ToInt32(comboBoxEdit5.Text);
            model.ComParity = comboBoxEdit2.Text;
            model.ComStopBits = comboBoxEdit3.Text;
            if (!string.IsNullOrEmpty(comboBoxEdit4.Text))
                model.ComDataBits = Convert.ToInt32(comboBoxEdit4.Text);

            model.SocketTcpType = Convert.ToInt32(lookUpEdit1.EditValue);
            model.InstrumentBrand = Convert.ToInt32(lookUpEdit3.EditValue);


            model.SendCommand1 = luSendCommand1.Text;
            model.SendCommand2 = luSendCommand2.Text;
            model.SendCommand3 = luSendCommand3.Text;
            model.SendCommand4 = luSendCommand4.Text;

            model.SendCommand5 = luSendCommand5.Text;
            model.SendCommand6 = luSendCommand6.Text;
            model.SendCommand7 = luSendCommand7.Text;
            model.SendCommand8 = luSendCommand8.Text;
            model.BufferSize = Convert.ToInt32(seBufferSize.Value);
            model.IsUse = checkEdit6.Checked;
            model.IsEnter = checkEdit2.Checked;
            model.IsLine = checkEdit1.Checked;
            model.IsByte = checkEdit3.Checked;
            model.IsHex = checkEdit4.Checked;
            model.IsCRC = checkEdit5.Checked;
            model.IsCheck = chkIsCheck.Checked;
            model.IsSocketTcp = checkEdit8.Checked;
            model.IsRecPlc = checkEdit10.Checked;
            model.IsSocket = checkEdit7.Checked;
            model.IsLock = chkIsLock.Checked;

            model.IsFCS = chkSFC.Checked;
            model.ScanCount = Convert.ToInt32(spinEdit5.Value);
            model.ScanInterval = Convert.ToInt32(spinEdit4.Value);
            model.SocketTimeOut = Convert.ToInt32(seSocketTimeOut.Value);
            model.Factory = textEdit8.Text;
            model.ReadDelay = Convert.ToInt32(spinEdit6.Value);

            model.IsClearCacheBeforeRead = checkEdit9.Checked;

            model.CommunicationType = Convert.ToInt32(lookUpEdit4.EditValue);

            model.IsLoading = chkIsLoading.Checked;
            model.IsUnLoading = chkIsUnLoading.Checked;
            if (!string.IsNullOrEmpty(ccboMachine.Text))
                model.MachineID = Convert.ToInt32(ccboMachine.EditValue);
            else
            {
                if (ccboMachine.Properties.Items.Count > 0)
                    model.MachineID = Convert.ToInt32(ccboMachine.Properties.Items[0].Value);
            }
            model.IsWriteLog = chkIsLog.Checked;


            model.Encoding = Convert.ToInt32(lookUpEdit5.EditValue);
            model.ClearDelayTime = Convert.ToInt32(seClearTime.Value);
            model.CommDelayBfTime = Convert.ToInt32(seCommBfTime.Value);
            model.CommDelayIngTime = Convert.ToInt32(seCommIngTime.Value);
            model.CommDelayTime1 = Convert.ToInt32(seCommTime1.Value);
            model.DeviedVal = Convert.ToInt32(seDeviedVal.Value);

            //updCmd.Append(",DeviedVal2=@DeviedVal2");

            model.EndByteLen = Convert.ToInt32(seByteLen.Value);
            model.EndStrLen = Convert.ToInt32(seStrLen.Value);
            model.SendCommand1Remark = luSendCommand1_remark.Text;
            model.SendCommand2Remark = luSendCommand2_remark.Text;
            model.SendCommand3Remark = luSendCommand3_remark.Text;
            model.SendCommand4Remark = luSendCommand4_remark.Text;
            model.SendCommand5Remark = luSendCommand5_remark.Text;
            model.SendCommand6Remark = luSendCommand6_remark.Text;
            model.SendCommand7Remark = luSendCommand7_remark.Text;
            model.SendCommand8Remark = luSendCommand8_remark.Text;


            return model;
        }

        public void SetPortNameValues(object obj)
        {

            string[] ports = SerialPort.GetPortNames();
            Array.Sort(ports);
            foreach (string str in ports)
            {
                ((ComboBoxEdit)obj).Properties.Items.Add(str);
            }
        }
        public void SetStopBitValues(object obj)
        {
            foreach (string str in Enum.GetNames(typeof(StopBits)))
            {
                ((ComboBoxEdit)obj).Properties.Items.Add(str);
            }
        }

        public void SetParityValues(object obj)
        {
            foreach (string str in Enum.GetNames(typeof(Parity)))
            {
                ((ComboBoxEdit)obj).Properties.Items.Add(str);
            }
        }

        private void DataSourceLookUpEdit(LookUpEdit lu, IList<DictionaryRef> listRef, string refCode)
        {
            var listData = listRef.ToList().FindAll(o => o.RefCode.Equals(refCode));
            lu.Properties.DataSource = listData;
            lu.Properties.ValueMember = "RefValue";
            lu.Properties.DisplayMember = "RefValue";
            lu.Properties.Columns.Clear();


            lu.Properties.Columns.Add(new LookUpColumnInfo("RefValue", "通讯指令")); //通讯指令
            lu.Properties.Columns.Add(new LookUpColumnInfo("RefCode2", "通讯类型"));//通讯类型
            lu.Properties.Columns.Add(new LookUpColumnInfo("RefValue2", "通讯品牌"));//通讯品牌
            lu.Properties.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFitResizePopup;
        }

        private void bbiCancel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DataRefresh();
            gridView5.FocusedRowHandle = rowIndex;
        }

        private void bbiRefresh_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

        }

        private string _ip;

        private FormattedIO488 myDmm;

        public void TCP34461A(string ip)
        {
            try
            {
                _ip = ip;
                string resourceName = "TCPIP0::" + _ip;
                ResourceManager resourceManager = (ResourceManager)Activator.CreateInstance(Marshal.GetTypeFromCLSID(new Guid("DB8CBF1C-D6D3-11D4-AA51-00A024EE30BD")));
                myDmm = (FormattedIO488)Activator.CreateInstance(Marshal.GetTypeFromCLSID(new Guid("DB8CBF1D-D6D3-11D4-AA51-00A024EE30BD")));
                myDmm.IO = (IMessage)resourceManager.Open(resourceName, AccessMode.NO_LOCK, 500, "");
                myDmm.IO.Timeout = 3000;
                myDmm.IO.Clear();
            }
            catch (Exception arg)
            {
                LoggingIF.Log("电压表初始化异常：" + arg, LogLevels.Error, "TCP34461A");
            }
        }

        public string Get34461AVoltage()
        {
            try
            {
                double num = 0.0;
                int num3;
                for (int num2 = 0; num2 < 3; num2 = num3 + 1)
                {
                    myDmm.WriteString("READ?", true);
                    string text = myDmm.ReadString();
                    text = text.Replace("\n", "");
                    num = Math.Abs(Convert.ToDouble(text));
                    if (num > 2.0 && num < 5.0)
                    {
                        break;
                    }
                    num3 = num2;
                }
                return num.ToString();
            }
            catch (Exception arg)
            {
                LoggingIF.Log("电压表异常：" + arg, LogLevels.Error, "Get34461AVoltage");
                return "电压表通讯异常";
            }
        }
        private void barButtonItem8_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

            switch (commcation.Code)
            {
                case "socket":
                    switch (commcation.Name)
                    {
                        case "LR8450":
                            break;
                        case "尺寸测量":
                            break;
                        case "34461":
                            TCP34461A(commcation.Ip);
                            barEditItem4.EditValue = Get34461AVoltage().ToString();
                            break;
                        case "上料扫码":
                        case "下料扫码":

                            #region 上料扫码\下料扫码
                            SocketTCPInterface tcp = CreateSocket(commcation);
                            string errmsg = string.Empty;
                            if (!tcp.IsClose && !tcp.Connected)
                            {
                                bool isOpen = InitSocketConnect(tcp, ref errmsg);
                                if (!isOpen)
                                {
                                    barEditItem4.EditValue = errmsg;
                                }
                            }
                            string errMsg = string.Empty;
                            var inputData = $"{tcp.SendCommand1}{tcp.EndString}";
                            var inputDataClear = $"{tcp.SendCommand2.ToUpper()}{tcp.EndString}";
                            barEditItem4.EditValue = tcp.SendData(inputData, ref errMsg, true, tcp.CommDelayIngTime, tcp.IsWriteLog);
                            for (int i = 0; i < 1; i++)
                            {
                                if (errMsg.Contains("TimedOut"))
                                {
                                    barEditItem4.EditValue = tcp.SendData(inputData, ref errMsg, true, tcp.CommDelayIngTime, tcp.IsWriteLog);
                                    if (i >= 0)
                                    {
                                        barEditItem4.EditValue = tcp.SendData(inputDataClear, ref errMsg, true, tcp.CommDelayIngTime, tcp.IsWriteLog);
                                    }

                                }
                                else
                                {
                                    if (!string.IsNullOrEmpty(errMsg))
                                    {
                                        barEditItem4.EditValue = errMsg;
                                    }
                                    break;
                                }
                            }
                            #endregion
                            break;

                        default:
                            break;
                    }
                    break;
                case "串口":
                    #region 串口通讯
                    //if (!Common.IsShortSocket)
                    //{
                    //    MessageBox.Show("串口通讯请改为短连接进行测试");
                    //    return;
                    //}
                    switch (commcation.Name)
                    {
                        case "厚度测试1":
                        case "厚度测试2":
                        case "厚度测试3":
                        case "厚度测试4":
                            int No = int.Parse(commcation.Name.Substring(4,1));
                            MitutoyoReaderIF.UnInit();
                
                            string ldstring = string.Empty;
                            var res = MitutoyoReaderIF.ReOpen(commcation.ComPort, No);
                            Thread.Sleep(1000);
                            if (false == res)
                            {
                                barEditItem4.EditValue = "COM打开失败!";
                                break;
                            }
                            else
                            {
                                MitutoyoReaderIF.ReadThickness(ref ldstring, No);
                            }
                           

                            var split = ldstring.Split('+');
                            if (split.Length > 1)
                            {
                                barEditItem4.EditValue = split[1];
                            }
                            else {
                                barEditItem4.EditValue = ldstring;
                            }




                            break;
                        case "3562":

                            //Communication _m = new Communication();
                            //_m.ComPort = comboBoxEdit1.Text;
                            //_m.ComBaudRate = int.Parse(comboBoxEdit5.Text);
                            //_m.ComDataBits = int.Parse(comboBoxEdit4.Text);
                            //_m.ComParity = comboBoxEdit2.Text;
                            //_m.ComStopBits = comboBoxEdit3.Text;
                            //_m.SocketTimeOut = 1500;
                         
                            var serial = CreateSerial(commcation);
                            var endString = "\r";//结束符
                            RichTextBox rtx = new RichTextBox();
                            string errMsg = string.Empty;
                            var inputData = string.Format("{0}{1}", serial.SendCommand1, serial.EndString);
                            serial.IsClose = true;
                            var rec = serial.SendData(ref errMsg, inputData, true, serial.CommDelayIngTime, serial.ReadDelay);
                            
                            serial.Close();
                            barEditItem4.EditValue = rec;
                            if (!string.IsNullOrEmpty(errMsg))
                            {
                                barStaticItem2.Caption = errMsg;
                                barStaticItem2.ItemAppearance.Normal.BackColor = Color.Red;
                                XtraMessageBox.Show(errMsg);
                                return;
                            }

                            //recData = Convert.ToString(rec);
                            break;

                        default:
                            break;
                    }
                    break;
                    #endregion

            }
        }

        public static bool InitSocketConnect(SocketTCPInterface socket, ref string errmsg)
        {
            var isOpen = false;
            int localPort = socket.LocalPort;
            string errMsg = string.Empty;
            for (int i = 0; i <= socket.LocalPortMaxNum; i++)
            {
                if (!socket.IsClose && socket.LocalPort > 0)
                {
                    localPort = socket.LocalPort + i;
                    isOpen = socket.InitSocket(ref errMsg, localPort);
                    isOpen = socket.OpenConnect();
                }
                else
                {
                    isOpen = socket.InitSocket(ref errMsg);
                }
                if (isOpen)
                {

                    errmsg = string.Format("IP:{0},网络重新连接成功,", socket.Ip);
                    break;
                }
                else
                {
                    errmsg = string.Format("IP:{0},网络重新连接失败,错误信息:{1}", socket.Ip, errMsg);
                }
            }
            return isOpen;
        }
        public static SerialPortInterface CreateSerial(Communication _m)
        {
            Encoding encoding = Encoding.ASCII;

            SerialPortInterface serial = new SerialPortInterface(encoding, _m.ComPort, _m.ComBaudRate.GetValueOrDefault(9600), _m.ComDataBits.GetValueOrDefault()
                     , _m.ComParity, _m.ComStopBits, _m.SocketTimeOut.GetValueOrDefault(3000), _m.IsClose.GetValueOrDefault(true));
            serial.BufferSize = _m.BufferSize.GetValueOrDefault(1);

            serial.EndString = string.Empty;
            if (_m.IsEnter.GetValueOrDefault(false))
            {
                serial.EndString = Common.LINE;
            }
            if (_m.IsLine.GetValueOrDefault(false))
            {
                if (!serial.EndString.Contains(Common.ENTER))
                    serial.EndString += Common.ENTER;
            }
            serial.IsLine = _m.IsLine.GetValueOrDefault(false);
            serial.IsEnter = _m.IsEnter.GetValueOrDefault(false);
            serial.ScanCount = _m.ScanCount.GetValueOrDefault(2);
            serial.ScanTime = _m.ScanInterval.GetValueOrDefault(500);
            serial.IsClose = _m.IsClose.GetValueOrDefault(true);
            serial.SendCommand1 = _m.SendCommand1;
            serial.SendCommand1Remark = _m.SendCommand1Remark;
            serial.SendCommand2 = _m.SendCommand2;
            serial.SendCommand2Remark = _m.SendCommand2Remark;
            serial.SendCommand3 = _m.SendCommand3;
            serial.SendCommand3Remark = _m.SendCommand3Remark;
            serial.SendCommand4 = _m.SendCommand4;
            serial.SendCommand4Remark = _m.SendCommand4Remark;
            serial.SendCommand5 = _m.SendCommand5;
            serial.SendCommand5Remark = _m.SendCommand5Remark;
            serial.SendCommand6 = _m.SendCommand6;
            serial.SendCommand6Remark = _m.SendCommand6Remark;
            serial.SendCommand7 = _m.SendCommand7;
            serial.SendCommand7Remark = _m.SendCommand7Remark;
            serial.SendCommand8 = _m.SendCommand8;
            serial.SendCommand8Remark = _m.SendCommand8Remark;
            serial.SendCommand9 = _m.SendCommand9;
            serial.SendCommand9Remark = _m.SendCommand9Remark;
            serial.SendCommand10 = _m.SendCommand10;
            serial.SendCommand10Remark = _m.SendCommand10Remark;
            serial.SendCommand11 = _m.SendCommand11;
            serial.SendCommand11Remark = _m.SendCommand11Remark;
            serial.SendCommand12 = _m.SendCommand12;
            serial.SendCommand12Remark = _m.SendCommand12Remark;
            if (_m.IsLine.GetValueOrDefault(true))
            {
                serial.EndString = Common.LINE;
            }
            if (_m.IsEnter.GetValueOrDefault(false))
            {
                if (!serial.EndString.Contains(Common.ENTER))
                    serial.EndString += Common.ENTER;
            }
            serial.ReadDelay = _m.ReadDelay.GetValueOrDefault(1);

            serial.IsLoading = _m.IsLoading.GetValueOrDefault(false);
            serial.IsUnLoading = _m.IsUnLoading.GetValueOrDefault(false);
            serial.InstrumentBrand = _m.InstrumentBrand.GetValueOrDefault(0);
            serial.IsHex = _m.IsHex.GetValueOrDefault(false);

            serial.MachineID = _m.MachineID.GetValueOrDefault(0);
            serial.IsWriteLog = _m.IsWriteLog;
            serial.ReceiveTimeout = _m.SocketTimeOut.GetValueOrDefault(1000);
            serial.WriteTimeout = _m.SocketTimeOut.GetValueOrDefault(1000);
            serial.IsSocketTcp = _m.IsSocketTcp.GetValueOrDefault(false);

            serial.ClearDelayTime = _m.ClearDelayTime.GetValueOrDefault(1) <= 10 ? 300 : _m.ClearDelayTime.GetValueOrDefault(1);
            serial.CommDelayBfTime = _m.CommDelayBfTime.GetValueOrDefault(1);
            serial.CommDelayIngTime = _m.CommDelayIngTime.GetValueOrDefault(1);
            serial.CommDelayTime1 = _m.CommDelayTime1.GetValueOrDefault(1);
            serial.CommDelayTime2 = _m.CommDelayTime2.GetValueOrDefault(1);
            serial.DeviedVal = _m.DeviedVal.GetValueOrDefault(1);
            serial.DeviedVal2 = _m.DeviedVal2.GetValueOrDefault(1);
            serial.DeviedVal3 = _m.DeviedVal3.GetValueOrDefault(1);
            serial.EndStrLen = _m.EndStrLen.GetValueOrDefault(1);
            serial.EndByteLen = _m.EndByteLen.GetValueOrDefault(1);

            serial.IsRecPlc = _m.IsRecPlc.GetValueOrDefault(false);


            return serial;
        }
        public SocketTCPInterface CreateSocket(Communication _m)
        {
            Encoding encoding = Encoding.ASCII;

            SocketTCPInterface tcpPlc = new SocketTCPInterface(encoding, _m.Ip, _m.IpPort.GetValueOrDefault(9004), _m.IpLocalPort.GetValueOrDefault(-1));
            tcpPlc.BufferSize = 1;
            tcpPlc.SocketType = SocketType.Stream;
            tcpPlc.ProtocolType = ProtocolType.Tcp;
            tcpPlc.SocketTimeout = _m.SocketTimeOut.GetValueOrDefault(1000);
            //tcpPlc.Ip = textEdit5.Text;
            tcpPlc.Ip = _m.Ip;
            tcpPlc.IsCRC = _m.IsCRC.GetValueOrDefault(false);
            tcpPlc.IsFCS = _m.IsFCS.GetValueOrDefault(false);
            tcpPlc.ScanCount = _m.ScanCount.GetValueOrDefault(3);
            tcpPlc.ScanTime = _m.ScanInterval.GetValueOrDefault(100);
            //_m.IsClose = chkIsClose.Checked;
            tcpPlc.IsClose = _m.IsClose.GetValueOrDefault(true);

            tcpPlc.LocalPort = _m.IpLocalPort.GetValueOrDefault(0);
            tcpPlc.LocalPortMaxNum = _m.IpLocalPortNum.GetValueOrDefault(1);

            //tcpPlc.Port = int.Parse(spinEdit2.Text);
            tcpPlc.Port = _m.IpPort.GetValueOrDefault(9004);
            tcpPlc.ReceiveTimeout = _m.SocketTimeOut.GetValueOrDefault(1000);
            tcpPlc.SendTimeout = _m.SocketTimeOut.GetValueOrDefault(1000);
            tcpPlc.IsLock = _m.IsLock.GetValueOrDefault(false);
            tcpPlc.EndString = string.Empty;
            //_m.IsLine = checkEdit1.Checked;

            if (_m.IsLine.GetValueOrDefault(true))
            {
                tcpPlc.EndString = Common.LINE;
            }
            //_m.IsEnter = checkEdit2.Checked;
            if (_m.IsEnter.GetValueOrDefault(false))
            {
                if (!tcpPlc.EndString.Contains(Common.ENTER))
                    tcpPlc.EndString += Common.ENTER;
            }
            tcpPlc.IsLine = _m.IsLine.GetValueOrDefault(false);

            tcpPlc.IsEnter = _m.IsEnter.GetValueOrDefault(false);
            tcpPlc.IsHex = _m.IsHex.GetValueOrDefault(false);
            //tcpPlc.SendCommand1 = _m.SendData;
            //tcpPlc.CloseCommand = _m.SendErrorData;
            //tcpPlc.SendCommand2 = _m.SendData1;
            //tcpPlc.SendCommand2 = _m.SendErrorData1;
            //tcpPlc.CommandCount = _m.ScanCount.GetValueOrDefault(3);

            //tcpPlc.SendCommand1 = luSendCommand1.Text;

            //tcpPlc.SendCommand2 = luSendCommand2.Text;
            tcpPlc.SendCommand1 = _m.SendCommand1;
            tcpPlc.SendCommand1Remark = _m.SendCommand1Remark;
            tcpPlc.SendCommand2 = _m.SendCommand2;
            tcpPlc.SendCommand2Remark = _m.SendCommand2Remark;
            tcpPlc.SendCommand3 = _m.SendCommand3;
            tcpPlc.SendCommand3Remark = _m.SendCommand3Remark;
            tcpPlc.SendCommand4 = _m.SendCommand4;
            tcpPlc.SendCommand4Remark = _m.SendCommand4Remark;
            tcpPlc.ReadDelay = _m.ReadDelay.GetValueOrDefault(1);
            tcpPlc.InstrumentBrand = _m.InstrumentBrand.GetValueOrDefault(0);
            tcpPlc.SendCommand5 = _m.SendCommand5;
            tcpPlc.SendCommand5Remark = _m.SendCommand5Remark;
            tcpPlc.SendCommand6 = _m.SendCommand6;
            tcpPlc.SendCommand6Remark = _m.SendCommand6Remark;
            tcpPlc.SendCommand7 = _m.SendCommand7;
            tcpPlc.SendCommand7Remark = _m.SendCommand7Remark;
            tcpPlc.SendCommand8 = _m.SendCommand8;
            tcpPlc.SendCommand8Remark = _m.SendCommand8Remark;
            tcpPlc.SendCommand9 = _m.SendCommand9;
            tcpPlc.SendCommand9Remark = _m.SendCommand9Remark;
            tcpPlc.SendCommand10 = _m.SendCommand10;
            tcpPlc.SendCommand10Remark = _m.SendCommand10Remark;
            tcpPlc.SendCommand11 = _m.SendCommand11;
            tcpPlc.SendCommand11Remark = _m.SendCommand11Remark;
            tcpPlc.SendCommand12 = _m.SendCommand12;
            tcpPlc.SendCommand12Remark = _m.SendCommand12Remark;
            tcpPlc.IsLoading = _m.IsLoading.GetValueOrDefault(false);
            tcpPlc.IsUnLoading = _m.IsUnLoading.GetValueOrDefault(false);
            tcpPlc.CommandCount = _m.ScanCount.GetValueOrDefault(2);
            tcpPlc.MachineID = _m.MachineID.GetValueOrDefault(0);
            tcpPlc.IsWriteLog = _m.IsWriteLog;

            tcpPlc.IsSocketTcp = _m.IsSocketTcp;

            tcpPlc.ClearDelayTime = _m.ClearDelayTime.GetValueOrDefault(1) <= 10 ? 300 : _m.ClearDelayTime.GetValueOrDefault(1);
            tcpPlc.CommDelayBfTime = _m.CommDelayBfTime.GetValueOrDefault(1);
            tcpPlc.CommDelayIngTime = _m.CommDelayIngTime.GetValueOrDefault(1);
            tcpPlc.CommDelayTime1 = _m.CommDelayTime1.GetValueOrDefault(1);
            tcpPlc.CommDelayTime2 = _m.CommDelayTime2.GetValueOrDefault(1);
            tcpPlc.DeviedVal = _m.DeviedVal.GetValueOrDefault(1);
            tcpPlc.DeviedVal2 = _m.DeviedVal2.GetValueOrDefault(1);
            tcpPlc.DeviedVal3 = _m.DeviedVal3.GetValueOrDefault(1);
            tcpPlc.EndStrLen = _m.EndStrLen.GetValueOrDefault(0);
            tcpPlc.EndByteLen = _m.EndByteLen.GetValueOrDefault(0);
            tcpPlc.IsRecPlc = _m.IsRecPlc.GetValueOrDefault(false);


            return tcpPlc;
        }
        Communication commcation = new Communication();
        private void gridView5_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {

            int row = gridView5.FocusedRowHandle;
            if (row >= 0)
            {
                commcation = gridView5.GetFocusedRow() as Communication;
            }

            else
            {
                XtraMessageBox.Show("请选择需要修改的数据");
            }
        }
    }
}
