using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Threading;
using System.Data;
using System.Globalization;
using System.Net;
using NetworkControl;
using ATL.Common;
using ATL.Core;
using ATL.Engine;
using ATL.PLCVariableValueService;
using System.Text.RegularExpressions;
using System.Net.NetworkInformation;
using Newtonsoft.Json;

namespace ATL.MES
{
    public partial class InterfaceClient
    {
        private static InterfaceClient current;
        public static InterfaceClient Current
        {
            get
            {
                return current;
            }
            set
            {
                if (current == null)
                {
                    current = value;
                }
            }
        }
        public static object LockLstReceivedJasonObj = new object();
        public static List<string> A053FolderPath = new List<string>();
        //public static List<A047.RequestInfo> lstRequestInfo = new List<A047.RequestInfo>();

        public static List<A047RequestInfo> lstA047RequestFromMES = new List<A047RequestInfo>();
        public static object LocklstA047RequestFromMESObj = new object();
        public static List<A047RequestInfo> lstA047RequestFromFile = new List<A047RequestInfo>();
        
        public static List<backserver> BackServerPara = new List<backserver>();
        public static ClientConnectionManager Client;
        public static ClientConnectionManager back_Client;
        
        private static string localIpAdr;

        private static int localUdpSendPortNo;
        private static int localUdpRecvPortNo;
        private static int localTcpPortNo;
        private static string serverIpAdr;
        private static string _serverIpAdr;
        private static int serverUdpPortNo;

        //private static int backupLocalUdpSendPortNo;
        //private static int backupLocalUdpRecvPortNo;
        //private static int backupLocalTcpPortNo;
        //private static string backupServerIpAdr;
        //private static int backupServerUdpPortNo;

        public static uint receiveTimeOut = 5000;//ms
        public static bool InterfaceClientWorking = false;
        public static bool AllowWork = true;
        /// <summary>
        /// DeviceRegisterResult等于0表示A001接口暂未注册，等于1表示注册成功，其他值表示注册失败。
        /// </summary>
        public static int DeviceRegisterResult = 0;
        /// <summary>
        /// 启用备用服务器的时候需要重新注册
        /// </summary>
        public static bool allDeviceRegistered = false;
        public static bool isFirstRegister = true;
        private static bool mesConnected = false;
        private static bool mesConnectError = false;
        private static bool heartBeatError = false;
        public static bool MESconnected
        {
            get
            {
                return mesConnected;
            }
            set
            {
                mesConnected = value;
                if (mesConnected && !mesConnectError && !heartBeatError)
                {
                    Station.Station.Current.MESState = "OK";
                }
                else
                {
                    Station.Station.Current.MESState = "NG";
                }
            }
        }
        public static bool MESconnectError
        {
            get
            {
                return mesConnectError;
            }
            set
            {
                mesConnectError = value;
                if (mesConnected && !mesConnectError && !heartBeatError)
                {
                    Station.Station.Current.MESState = "OK";
                }
                else
                {
                    Station.Station.Current.MESState = "NG";
                }
            }
        }
        public static bool HeartBeatError
        {
            get
            {
                return heartBeatError;
            }
            set
            {
                heartBeatError = value;
                if (mesConnected && !mesConnectError && !heartBeatError)
                {
                    Station.Station.Current.MESState = "OK";
                }
                else
                {
                    Station.Station.Current.MESState = "NG";
                }
            }
        }
        public static bool StartUploadBuffer = false;
        /// <summary>
        /// 超时仍未收到相应Guid的信息，则退出，不再等待。单位为ms
        /// </summary>
        public static uint ReceiveTimeOut = 5000;
        public static uint receivedMsgUnhandledTimeOut = 60000;
        /// <summary>
        /// 对超时未处理的已收数据清除
        /// </summary>
        public static uint ReceivedMsgUnhandledTimeOut
        {
            get
            {
                return receivedMsgUnhandledTimeOut;
            }
            set
            {
                if (value - 5000 > receiveTimeOut)
                {
                    receivedMsgUnhandledTimeOut = value;
                }
            }
        }

        /// <summary>
        /// 获取本机所有ip地址
        /// </summary>
        /// <param name="netType">"InterNetwork":ipv4地址，"InterNetworkV6":ipv6地址</param>
        /// <returns>ip地址集合</returns>
        public static List<string> GetLocalIpAddress(string netType)
        {
            string hostName = Dns.GetHostName();                    //获取主机名称  
            IPAddress[] addresses = Dns.GetHostAddresses(hostName); //解析主机IP地址  

            List<string> IPList = new List<string>();
            if (string.IsNullOrEmpty(netType))
            {
                for (int i = 0; i < addresses.Length; i++)
                {
                    IPList.Add(addresses[i].ToString());
                }
            }
            else
            {
                //AddressFamily.InterNetwork表示此IP为IPv4,
                //AddressFamily.InterNetworkV6表示此地址为IPv6类型
                for (int i = 0; i < addresses.Length; i++)
                {
                    if (addresses[i].AddressFamily.ToString() == netType)
                    {
                        IPList.Add(addresses[i].ToString());
                    }
                }
            }
            return IPList;
        }

        public InterfaceClient()
        {
            ThreadPool.SetMinThreads(1000, 1000);
            ThreadPool.QueueUserWorkItem(doWork);
            ThreadPool.QueueUserWorkItem(MESheartBeat);
            //ThreadPool.QueueUserWorkItem(PlcHeartBeat);

            DAL.me.GetBackServerPara();
            Thread thread = new Thread(new ThreadStart(StartMain));
            thread.Start();

            #region 自动获取本机IP，
            ATL.Common.GetComputerIP get = new GetComputerIP();
            string _localAddress = get.GetComputerIPFunction();
            if(Station.Station.Current.PCName == "NMRD-I94974-N")  //我的调试笔记本
                _localAddress = string.Empty;
            if (string.IsNullOrEmpty(_localAddress))
            {
                List<string> ipv4_ips = GetLocalIpAddress("InterNetwork");

                localIpAdr = UserDefineVariableInfo.DicVariables["localIpAdr"].ToString();
                string[] localIpAdrArray = localIpAdr.Split('.');
                if (localIpAdrArray.Count() < 3)
                {
                    //string msg = "localIpAdr IP配置错误";
                    string msg = string.Empty;
                    if (ATL.Common.StringResources.IsDefaultLanguage)
                    {
                        msg = "localIpAdr IP配置错误";
                    }
                    else
                    {
                        msg = "Localipadr IP configuration error ";
                    }
                    MessageBox.Show(msg);
                    LogInDB.Error(msg);
                }
                else
                {
                    foreach (var v in ipv4_ips)
                    {
                        string[] ip = v.Split('.');
                        if (ip.Count() == 4)
                        {
                            if (ip[0] == localIpAdrArray[0] && ip[1] == localIpAdrArray[1] /*&& ip[2] == localIpAdrArray[2]*/)
                            {
                                localIpAdr = v;
                                break;
                            }
                        }
                    }
                }
            }
            else
            {
                localIpAdr = _localAddress;
            }
            #endregion

            localUdpSendPortNo = (int)float.Parse(UserDefineVariableInfo.DicVariables["localUdpSendPortNo"].ToString());
            localUdpRecvPortNo = (int)float.Parse(UserDefineVariableInfo.DicVariables["localUdpRecvPortNo"].ToString());
            localTcpPortNo = (int)float.Parse(UserDefineVariableInfo.DicVariables["localTcpPortNo"].ToString());
            serverIpAdr = UserDefineVariableInfo.DicVariables["serverIpAdr"].ToString();
            serverUdpPortNo = (int)float.Parse(UserDefineVariableInfo.DicVariables["serverUdpPortNo"].ToString());

            initInterfaceClient();
        }

        /// <summary>
        /// 上位机软件和mes重连请求间隔为10S，不能一直不停的向服务器发送连接请求
        /// </summary>
        public void initInterfaceClient()
        {
            try
            {
                _serverIpAdr = "主服务器IP:" + serverIpAdr;
                Client = new ClientConnectionManager(localIpAdr,
                                                       localUdpSendPortNo,
                                                       localUdpRecvPortNo,
                                                       localTcpPortNo,
                                                       serverIpAdr,
                                                       serverUdpPortNo,
                                                       ClientConnect,
                                                       RecvData,
                                                       Disconnect,
                                                       ConnectError);
                Thread.Sleep(50);
                if (Client.ConnectionRequest() == 0) //等于0表示连接成功
                {
                    byte[] data = Encoding.Default.GetBytes(GetMesHeartJason());
                    Thread.Sleep(1000);
                    if (Client.SendData("A005", data, 500) != 0)
                    {
                        //发送失败
                    }
                }
            }
            catch (Exception ex)
            {
                MESconnected = false;
                main_server = false;
            }
            is_reconnect = true;
            Thread t = new Thread(new ThreadStart(newThreadStartBackup));
            t.Start();
        }

        private void newThreadStartBackup()
        {
            //设置超时时间10秒，若未执行ClientConnect回调函数，则切换备用
            Thread.Sleep(10000);
            if (!MESconnected)
            {
                string msg = string.Empty;
                if (ATL.Common.StringResources.IsDefaultLanguage)
                {
                    msg = "切换备用服务器";
                }
                else
                {
                    msg = "Switch standby server";
                }
                LogInDB.Info(msg);
                RecordConnErrorInfo(msg);
                StartBackup();
            }
        }

        private static object StartBackupObj = new object();
        public bool is_reconnect = false;//防止刚开启就重连服务器
        public static bool main_server = false;//主服务器连接状态

        private object obj1 = new object();

        private string GetMesHeartJason()
        {
            A005.Root root = new A005.Root();
            string error = string.Empty;

            root.Header = new MES.A005.Header();
            root.Header.EQCode = Station.Station.Current.EquipmentID;
            root.Header.FunctionID = "A005";
            root.Header.PCName = Station.Station.Current.PCName;
            root.Header.RequestTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff");
            root.Header.SessionID = Guid.NewGuid().ToString();
            root.Header.SoftName = Station.Station.Current.SoftName;
            root.RequestInfo = new A005.RequestInfo();
            return root.ToJSON();
        }

        /// <summary>
        /// 启用备用服务器
        /// </summary>
        private void StartBackup()
        {
            foreach (var v in DeivceEquipmentidPlcInfo.lstDeivceEquipmentidPlcInfos)
            {
                lock (DeivceEquipmentidPlcInfo.LockObj)
                {
                    v.DeviceRegisterResult = 0;
                }
            }
            string msg = string.Empty;
            if (ATL.Common.StringResources.IsDefaultLanguage)
            {
                msg = "尝试切换备用服务器";
            }
            else
            {
                msg = "Try to switch the standby server ";
            }
            LogInDB.Info(msg);
            RecordConnErrorInfo(msg);
            Thread.Sleep(10000);
            allDeviceRegistered = false;
            isFirstRegister = true;
            heartBeatError = false;
            lock (StartBackupObj)
            {
                if (BackServerPara.Count != 0)
                {
                    foreach (var v in BackServerPara)
                    {
                        if (main_server)
                            break;
                        if (Client != null)
                        {
                            MESconnected = false;
                            Client.Close();
                            Client = null;
                        }
                        lock (obj1)
                        {
                            if (!main_server)
                            {
                                try
                                {
                                    heartBeatError = false;
                                    _serverIpAdr = "备用服务器IP:" + v.backupServerIpAdr;
                                    back_Client = new ClientConnectionManager(localIpAdr.ToString(),
                                                                         v.backupLocalUdpSendPortNo,
                                                                         v.backupLocalUdpRecvPortNo,
                                                                         v.backupLocalTcpPortNo,
                                                                         v.backupServerIpAdr,
                                                                         v.backupServerUdpPortNo,
                                                         BackupClientConnect,
                                                         RecvData,
                                                         BackupDisconnect,
                                                         BackupConnectError);
                                    if (back_Client.ConnectionRequest() == 0)
                                    {
                                        byte[] data = Encoding.Default.GetBytes(GetMesHeartJason());
                                        Thread.Sleep(1000);
                                        if (back_Client.SendData("A005", data, 500) != 0)
                                        {
                                            //发送失败
                                        }
                                        else
                                        {
                                            if (Client != null)
                                            {
                                                Client.Close();
                                                Client = null;
                                                Client = back_Client;
                                            }
                                            else
                                            {
                                                Client = back_Client;
                                            }
                                            //LogInDB.Info("切换备用服务器" + v.ID.ToString() + "号成功");
                                            if (ATL.Common.StringResources.IsDefaultLanguage)
                                            {
                                                LogInDB.Info("切换备用服务器" + v.ID.ToString() + "号成功");
                                            }
                                            else
                                            {
                                                LogInDB.Info("Switch the standby server" + v.ID.ToString() + "succeeded ");
                                            }
                                            StartUploadBuffer = true;
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        if (back_Client != null)
                                        {
                                            back_Client.Close();
                                            back_Client = null;
                                        }
                                        msg = string.Empty;
                                        if (ATL.Common.StringResources.IsDefaultLanguage)
                                        {
                                            msg = "连接备用服务器" + v.ID.ToString() + "号失败";
                                        }
                                        else
                                        {
                                            msg = "Connecting to the standby server " + v.ID.ToString() + "failed ";
                                        }
                                        LogInDB.Info(msg);
                                        RecordConnErrorInfo(msg);
                                    }
                                }
                                catch (Exception e)
                                {
                                    if (back_Client != null)
                                    {
                                        back_Client.Close();
                                        back_Client = null;
                                    }
                                    msg = string.Empty;
                                    if (ATL.Common.StringResources.IsDefaultLanguage)
                                    {
                                        msg = "连接备用服务器" + v.ID.ToString() + "号失败";
                                    }
                                    else
                                    {
                                        msg = "Connecting to the standby server " + v.ID.ToString() + "failed ";
                                    }
                                    LogInDB.Info(msg);
                                    RecordConnErrorInfo(msg);
                                }
                            }
                            else
                                break;
                        }
                        Thread.Sleep(10000);
                    }
                }
            }
        }

        /// <summary>
        /// 主机断开后需要不断尝试连接的主机的方法
        /// </summary>
        private void StartMain()
        {
            ClientConnectionManager mainClientRetry = null;
            while (!ATL.Core.Core.SoftClosing)
            {
                Thread.Sleep(10000);  //上位机软件和mes重连请求间隔为10S，不能一直不停的向服务器发送连接请求
                if ((!main_server || !MESconnected || Client == null || Current == null) && is_reconnect)
                {
                    if (mainClientRetry != null)
                    {
                        mainClientRetry.Close();
                        mainClientRetry = null;
                    }
                    if (Client != null)
                    {
                        Client.Close();
                        Client = null;
                    }
                    lock (obj1)
                    {
                        try
                        {
                            _serverIpAdr = "主服务器IP:" + serverIpAdr;
                            mainClientRetry = new ClientConnectionManager(localIpAdr,
                                                                           localUdpSendPortNo,
                                                                           localUdpRecvPortNo,
                                                                           localTcpPortNo,
                                                                           serverIpAdr,
                                                                           serverUdpPortNo,
                                                                           ClientConnect,
                                                                           RecvData,
                                                                           Disconnect,
                                                                           ConnectError);
                            if (mainClientRetry.ConnectionRequest() == 0)
                            {
                                byte[] data = Encoding.Default.GetBytes(GetMesHeartJason());
                                Thread.Sleep(1000);
                                if (mainClientRetry.SendData("A005", data, 500) != 0)
                                {
                                    //发送失败
                                    if (mainClientRetry != null)
                                    {
                                        mainClientRetry.Close();
                                        mainClientRetry = null;
                                    }
                                }
                                else
                                {
                                    foreach (var v in DeivceEquipmentidPlcInfo.lstDeivceEquipmentidPlcInfos)
                                    {
                                        lock (DeivceEquipmentidPlcInfo.LockObj)
                                        {
                                            v.DeviceRegisterResult = 0;
                                        }
                                    }
                                    //LogInDB.Info("主服务器重连接成功！");
                                    if (ATL.Common.StringResources.IsDefaultLanguage)
                                    {
                                        LogInDB.Info("主服务器重连接成功！");
                                    }
                                    else
                                    {
                                        LogInDB.Info("The primary server was reconnected successfully！");
                                    }
                                    if (Client != null)
                                    {
                                        Client.Close();
                                        Client = null;
                                    }
                                    Client = mainClientRetry;
                                    if (back_Client != null)
                                    {
                                        back_Client.Close();
                                        back_Client = null;
                                    }
                                    allDeviceRegistered = false;
                                    isFirstRegister = true;
                                    StartUploadBuffer = true;
                                    main_server = true;
                                }
                            }
                            else
                            {
                                //连接主服务器失败
                                if (mainClientRetry != null)
                                {
                                    mainClientRetry.Close();
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            //连接主服务器失败
                            //Console.WriteLine("尝试连接主服务失败！继续尝试！");
                            if (ATL.Common.StringResources.IsDefaultLanguage)
                            {
                                Console.WriteLine("尝试连接主服务失败！继续尝试！");
                            }
                            else
                            {
                                Console.WriteLine("Attempt to connect to main service failed! Keep trying！");
                            }
                            if (mainClientRetry != null)
                            {
                                mainClientRetry.Close();
                            }
                        }
                    }
                }
            }
        }
        static InterfaceClient()
        {
            Station.Station.Current.PCName = Environment.MachineName;
        }
        static Engine.PLC plc = new Engine.PLC();
        private static void doWork(object state)
        {
            StartUploadBuffer = true;
            try
            {
                WRSerializable.DeserializeFromFile(ref lstA047RequestFromFile, System.IO.Directory.GetCurrentDirectory() + "\\lstA047RequestFromMES.dat");
                WRSerializable.DeserializeFromFile(ref lstA047RequestFromMES, System.IO.Directory.GetCurrentDirectory() + "\\lstA047RequestFromMES.dat");
            }
            catch(Exception ex){
                //LogInDB.Error($"反序列化[lstA047RequestFromFile]failure：{ex.ToString()}");
                if (ATL.Common.StringResources.IsDefaultLanguage)
                {
                    LogInDB.Error($"反序列化[lstA047RequestFromFile]failure：{ex.ToString()}");
                }
                else
                {
                    LogInDB.Error($"Failed to deserialize [lstA047RequestFromFile] ：{ex.ToString()}");
                }
            }
            Thread.Sleep(2000);
            
            DateTime lastLongTimeSendDeviceStatus = DateTime.Now.AddSeconds(-700);
            
            DateTime lastWriteControlCode = DateTime.Now.AddSeconds(-700);
            DateTime lastDownLoadSpartExpectedLifetime = DateTime.Now.AddSeconds(-700);
            DateTime lastUploadAlertInfo = DateTime.Now.AddSeconds(-700);
            DateTime lastNeedDownloadModeltime = DateTime.Now.AddSeconds(-700);

            DateTime lastSpartLifeTimeSendToMes = DateTime.Now.AddMinutes(-31);
            DateTime lastUploadChangeMonitorValueToMes = DateTime.Now.AddSeconds(-11);
            DateTime lastInsertOfflineDataInfoToDatabase = DateTime.Now;
            DateTime lastHmiPermissionRequest = DateTime.Now;

            DateTime lastShortTimeSendDeviceStatus = DateTime.Now;
            DateTime lastTimeQueryLampState = DateTime.Now.AddDays(-1) ;
            DateTime lastUploadVersionInfo = DateTime.Now.AddSeconds(-600);
            bool totalPLCState = true;
            while (!ATL.Core.Core.SoftClosing)
            {
                Thread.Sleep(100);
                try
                {
                    InterfaceClientWorking = false;
                    if (!_allowWork()) continue;
                    InterfaceClientWorking = true;


                    if (PlcConfigurationInfo.lstPlcConfiguration.Where(x => x.Enabled == 1).ToList().Count > 0)
                    {
                        totalPLCState = true;
                        foreach (var v in PlcConfigurationInfo.lstPlcConfiguration.Where(x => x.Enabled == 1))
                        {
                            totalPLCState = totalPLCState && v.PLC.IsConnect;
                        }
                        if(!totalPLCState) continue;
                    }
                    
                    #region A023上传版本号
                    try
                    {
                        if ((DateTime.Now - lastUploadVersionInfo).TotalSeconds > 600)
                        {
                            UserVariableValueService Service = new UserVariableValueService();
                            Service.WriteSoftVersion(DeivceEquipmentidPlcInfo.lstDeivceEquipmentidPlcInfos);

                            lastUploadVersionInfo = DateTime.Now;
                            A024.Root root = Current.A023();
                            if (root != null)
                            {
                                if (root.ResponseInfo.Result != null && root.ResponseInfo.Result.ToUpper() == "OK")
                                {
                                    //LogInDB.Info("上传版本号成功");
                                    if (ATL.Common.StringResources.IsDefaultLanguage)
                                    {
                                        LogInDB.Info("上传版本号成功");
                                    }
                                    else
                                    {
                                        LogInDB.Info("Upload version number successfully");
                                    }
                                }
                                else
                                {
                                    //LogInDB.Info("上传版本号失败");
                                    if (ATL.Common.StringResources.IsDefaultLanguage)
                                    {
                                        LogInDB.Info("上传版本号失败");
                                    }
                                    else
                                    {
                                        LogInDB.Info("Failed to upload version number");
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Thread.Sleep(3000);
                        //LogInDB.Error("A023上传版本号:" + ex.ToString());
                        if (ATL.Common.StringResources.IsDefaultLanguage)
                        {
                            LogInDB.Error("A023上传版本号:" + ex.ToString());
                        }
                        else
                        {
                            LogInDB.Error("A023 upload version number:" + ex.ToString());
                        }
                    }

                    #endregion

                    #region 更新易损件可使用寿命到PLC里
                    try
                    {
                        if ((DateTime.Now - lastDownLoadSpartExpectedLifetime).TotalSeconds > 10)
                        {
                            lastDownLoadSpartExpectedLifetime = DateTime.Now;
                            foreach (var vv in DeivceEquipmentidPlcInfo.lstDeivceEquipmentidPlcInfos)
                            {
                                List<DeviceSpartConfigInfo> lst = DeviceSpartConfigInfo.lstDeviceSpartConfigInfos.Where(x => x.NeedDownLoadSpartExpectedLifetime && x.EquipmentID == vv.EquipmentID).ToList();
                                if (lst != null && lst.Count > 0)
                                {
                                    A022.Root root = Current.A021_ReplaceSpart(lst);
                                    if (root != null && root.ResponseInfo.Result.ToUpper() == "OK")  //上传易损件当前已使用寿命为0成功后，才更新设备里当前易损件信息
                                    {
                                        //LogInDB.Error($"更换易损件，上传新的易损件当前已使用寿命以及预期寿命成功");
                                        if (ATL.Common.StringResources.IsDefaultLanguage)
                                        {
                                            LogInDB.Error($"更换易损件，上传新的易损件当前已使用寿命以及预期寿命成功");
                                        }
                                        else
                                        {
                                            LogInDB.Error($"Replace the wearing parts and upload the current service life and expected life of the new vulnerable parts successfully ");
                                        }
                                        UserVariableValueService Service = new UserVariableValueService();
                                        Service.DownLoadSpart(vv, lst);
                                    }
                                    else
                                    {
                                        //LogInDB.Info($"更新易损件可使用寿命到{root.Header.EQCode} PLC里失败！！！！");
                                        if (ATL.Common.StringResources.IsDefaultLanguage)
                                        {
                                            LogInDB.Info($"更新易损件可使用寿命到{root.Header.EQCode} PLC里失败！！！！");
                                        }
                                        else
                                        {
                                            LogInDB.Info($"Update the service life of wearing parts to{ root.Header.EQCode } PLC failed ！！！！");
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        //LogInDB.Error("更新易损件可使用寿命到PLC里:" + ex.ToString());
                        if (ATL.Common.StringResources.IsDefaultLanguage)
                        {
                            LogInDB.Error("更新易损件可使用寿命到PLC里:" + ex.ToString());
                        }
                        else
                        {
                            LogInDB.Error("Update the service life of vulnerable parts to PLC :" + ex.ToString());
                        }
                        Thread.Sleep(3000);
                    }
                    #endregion

                    #region 下载Model到PLC里

                    try
                    {
                        if ((DateTime.Now - lastNeedDownloadModeltime).TotalSeconds > 10)
                        {
                            lastNeedDownloadModeltime = DateTime.Now;
                            foreach (var v in DeivceEquipmentidPlcInfo.lstDeivceEquipmentidPlcInfos.Where(x => x.NeedDownloadModel))
                            {
                                UserVariableValueService Service = new UserVariableValueService();
                                Service.DownloadModel(v);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        //LogInDB.Error("下载Model到PLC里:" + ex.ToString());
                        if (ATL.Common.StringResources.IsDefaultLanguage)
                        {
                            LogInDB.Error("下载Model到PLC里:" + ex.ToString());
                        }
                        else
                        {
                            LogInDB.Error("Download model to PLC:" + ex.ToString());
                        }
                        Thread.Sleep(3000);
                    }

                    #endregion

                    #region A019发送设备状态

                    try
                    {
                        if ((DateTime.Now - lastShortTimeSendDeviceStatus).TotalSeconds > 3)
                        {
                            lastShortTimeSendDeviceStatus = DateTime.Now;
                            foreach (var v in DeivceEquipmentidPlcInfo.lstDeivceEquipmentidPlcInfos)
                            {
                                UserVariableValueService Service = new UserVariableValueService();
                                Service.GetStatus(v);

                                if (v.NeedUploadParentEQStateCode)
                                {
                                    v.NeedUploadParentEQStateCode = false;
                                    Current.A019(v.EquipmentID);
                                    if (ATL.Common.StringResources.IsDefaultLanguage)
                                    {
                                        LogInDB.Info($"A019发送设备状态 {v.EquipmentID}");
                                    }
                                    else
                                    {
                                        LogInDB.Info($"A019 sending device status:{v.EquipmentID}");
                                    }
                                }
                                Thread.Sleep(10);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        //LogInDB.Error("A019发送设备状态:" + ex.ToString());
                        if (ATL.Common.StringResources.IsDefaultLanguage)
                        {
                            LogInDB.Error("A019发送设备状态:" + ex.ToString());
                        }
                        else
                        {
                            LogInDB.Error("A019 sending device status:" + ex.ToString());
                        }
                        Thread.Sleep(3000);
                    }
                    #endregion

                    #region A100发送设备三色灯状态

                    try
                    {
                        if ((DateTime.Now - lastTimeQueryLampState).TotalSeconds > 3)
                        {
                            lastTimeQueryLampState = DateTime.Now;
                            int interval = 15;
                            if(UserDefineVariableInfo.DicVariables._dict.ContainsKey("A100Interval")
                                && int.TryParse(UserDefineVariableInfo.DicVariables["A100Interval"].ToString(), out interval)
                                && interval > 3)
                            {
                                foreach (var v in DeivceEquipmentidPlcInfo.lstDeivceEquipmentidPlcInfos)
                                {
                                    v.LampState = plc.ReadByVariableName(v.LampStateAddress);
                                    if (v.LastLampState != v.LampState)
                                    {
                                        v.LastLampState = v.LampState;
                                        v.LastLampTime = DateTime.Now;
                                        v.HasSendA099 = false;
                                    }
                                    if(!v.HasSendA099 && (DateTime.Now - v.LastLampTime).TotalSeconds > interval)
                                    {
                                        v.HasSendA099 = true;
                                        Current.A099(v.LampState, interval.ToString(), v.EquipmentID);
                                    }
                                }
                            }
                            else
                            {
                                if (ATL.Common.StringResources.IsDefaultLanguage)
                                {
                                    LogInDB.Error("user_define_variable 表里需要正确配置变量:A100Interval");
                                }
                                else
                                {
                                    LogInDB.Error("You need to config variableName 'A100Interval' into table user_define_variable correctly");
                                }
                            }
                        }
                    }
                    catch(Exception ex)
                    {
                        if (ATL.Common.StringResources.IsDefaultLanguage)
                        {
                            LogInDB.Error("A099发送设备三色灯状态:" + ex.ToString());
                        }
                        else
                        {
                            LogInDB.Error("A099 sending 3_color_Lamp_Status:" + ex.ToString());
                        }
                        Thread.Sleep(3000);
                    }
                    #endregion

                    #region A021发送当前易损件信息给MES
                    try
                    {
                        if ((DateTime.Now - lastSpartLifeTimeSendToMes).TotalMinutes > 30) //上位机软件每30分钟发送一次当前所有关键配件的寿命数据到mes
                        {
                            lastSpartLifeTimeSendToMes = DateTime.Now;
                            foreach (var v in DeivceEquipmentidPlcInfo.lstDeivceEquipmentidPlcInfos)
                            {
                                A022.Root root = Current.A021(v.EquipmentID);
                                if (root != null && root.ResponseInfo.Result.ToUpper() == "OK")
                                {
                                    //LogInDB.Info($"A021发送当前易损件信息给MES {root.Header.EQCode}");
                                    if (ATL.Common.StringResources.IsDefaultLanguage)
                                    {
                                        LogInDB.Info($"A021发送当前易损件信息给MES {root.Header.EQCode}");
                                    }
                                    else
                                    {
                                        LogInDB.Info($"A021 send current vulnerable parts information to MES : {root.Header.EQCode}");
                                    }
                                }
                                Thread.Sleep(10);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        //LogInDB.Error("A021发送当前易损件信息给MES:" + ex.ToString());
                        if (ATL.Common.StringResources.IsDefaultLanguage)
                        {
                            LogInDB.Error("A021发送当前易损件信息给MES:" + ex.ToString());
                        }
                        else
                        {
                            LogInDB.Error("A021 send current vulnerable parts information to MES:" + ex.ToString());
                        }
                        Thread.Sleep(3000);
                    }
                    #endregion

                    #region A025上传报警信息
                    try
                    {
                        if ((DateTime.Now - lastUploadAlertInfo).TotalSeconds > 3)
                        {
                            lastUploadAlertInfo = DateTime.Now;
                            foreach (var v in DeivceEquipmentidPlcInfo.lstDeivceEquipmentidPlcInfos)
                            {
                                A026.Root root = Current.A025(v.EquipmentID);
                                Thread.Sleep(10);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        //LogInDB.Error("A025上传报警信息:" + ex.ToString());
                        if (ATL.Common.StringResources.IsDefaultLanguage)
                        {
                            LogInDB.Error("A025上传报警信息:" + ex.ToString());
                        }
                        else
                        {
                            LogInDB.Error("A025 upload alarm information : " + ex.ToString());
                        }
                        Thread.Sleep(3000);
                    }
                    #endregion

                    #region A045上传重要参数
                    try
                    {
                        if ((DateTime.Now - lastUploadChangeMonitorValueToMes).TotalSeconds > 3)
                        {
                            lastUploadChangeMonitorValueToMes = DateTime.Now;
                            foreach (var v in DeivceEquipmentidPlcInfo.lstDeivceEquipmentidPlcInfos)
                            {
                                A046.Root root = Current.A045(v.EquipmentID);
                                Thread.Sleep(10);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        //LogInDB.Error("A045上传重要参数:" + ex.ToString());
                        if (ATL.Common.StringResources.IsDefaultLanguage)
                        {
                            LogInDB.Error("A045上传重要参数:" + ex.ToString());
                        }
                        else
                        {
                            LogInDB.Error("A045 upload important parameters :" + ex.ToString());
                        }
                        Thread.Sleep(3000);
                    }
                    #endregion

                    #region PLC触发上位机查询MES权限、下发A007\A011接口的Count信息给PLC

                    if ((DateTime.Now - lastHmiPermissionRequest).TotalSeconds > 2)
                    {
                        lastHmiPermissionRequest = DateTime.Now;
                        foreach (var v in DeivceEquipmentidPlcInfo.lstDeivceEquipmentidPlcInfos)
                        {
                            bool HmiPermissionRequest = plc.ReadByVariableName(v.HmiPermissionRequestAddress) == "1";
                            if (HmiPermissionRequest)
                            {
                                string PlcAccount = plc.ReadByVariableName(v.AccountAddress);
                                string PlcCode = plc.ReadByVariableName(v.CodeAddress);
                                HmiLogin(PlcAccount, PlcCode, v.UserLevelAddress, v.HmiPermissionRequestAddress);
                            }
                            //A026.Root root = Current.A025(v.EquipmentID);
                            Thread.Sleep(10);
                            DateTime dt = v.LastMesUpdateCountDateTime;

                            if (dt > v.LastUpdateCountToPlcDateTime)
                            {
                                UserVariableValueService Service = new UserVariableValueService();
                                Service.WriteProductModeAndCount(v, dt);
                            }
                        }
                    }

                    #endregion
                    

                }
                catch (Exception ex)
                {
                    LogInDB.Error(ex.ToString());
                }
            }
        }

        public static bool _allowWork()
        {
            return (Client != null && MESconnected && !HeartBeatError && allDeviceRegistered && ATL.Core.Core.CheckedOK);
        }

        private static void MESheartBeat(object state)
        {
            int heartBeatError = 0;
            DateTime lastMesHeartBeatTime = DateTime.Now;
            DateTime LastMesHeartBeatOKTime = DateTime.Now;
            DateTime LastPingMesServer = DateTime.Now;

            DateTime lastDeviceRegisterTime = DateTime.Now.AddSeconds(-3);
            
            while (!ATL.Core.Core.SoftClosing)
            {
                Thread.Sleep(1000);

                #region 注册设备
                if ((DateTime.Now - lastDeviceRegisterTime).TotalSeconds > 3 && MESconnected && !allDeviceRegistered && Client != null && Current != null)
                {
                    lastDeviceRegisterTime = DateTime.Now;
                    try
                    {
                        lock (DeivceEquipmentidPlcInfo.LockObj)
                        {
                            foreach (var v in DeivceEquipmentidPlcInfo.lstDeivceEquipmentidPlcInfos.Where(x => x.DeviceRegisterResult == 0))
                            {
                                if (isFirstRegister)
                                {
                                    isFirstRegister = false;
                                    Thread.Sleep(2000);
                                }
                                A002.Root root = Current.A001(v.EquipmentID);
                                if (root != null)
                                {
                                    if (root.ResponseInfo != null && root.ResponseInfo.Result != null && root.ResponseInfo.Result.ToUpper() == "OK")
                                    {
                                        v.DeviceRegisterResult = 1;

                                        MESmsg msg = new MESmsg();
                                        msg.FunctionID = root.Header.FunctionID;
                                        //msg.translation = $"{DateTime.Now.ToString("MM-dd HH:mm:ss")} 设备ID:{v.EquipmentID}注册成功";
                                        if (ATL.Common.StringResources.IsDefaultLanguage)
                                        {
                                            msg.translation = $"{DateTime.Now.ToString("MM-dd HH:mm:ss")} 设备ID:{v.EquipmentID}注册成功";
                                        }
                                        else
                                        {
                                            msg.translation = $"{DateTime.Now.ToString("MM-dd HH:mm:ss")} Equipment ID:{v.EquipmentID} registrated succeed";
                                        }
                                        LogInDB.Info(msg.translation);
                                    }
                                    else
                                    {
                                        //LogInDB.Error($"A001接口注册失败, ErrorCode: {root.Header.ErrorCode}, ErrorMsg: {root.Header.ErrorMsg}");
                                        if (ATL.Common.StringResources.IsDefaultLanguage)
                                        {
                                            LogInDB.Error($"A001接口注册失败, ErrorCode: {root.Header.ErrorCode}, ErrorMsg: {root.Header.ErrorMsg}");
                                        }
                                        else
                                        {
                                            LogInDB.Error($"A001 interface registration failed , ErrorCode: {root.Header.ErrorCode}, ErrorMsg: {root.Header.ErrorMsg}");
                                        }
                                        lock (DeivceEquipmentidPlcInfo.LockObj)
                                        {
                                            v.DeviceRegisterResult = 2;
                                        }
                                    }
                                }
                            }

                            if (ATL.Core.Core.CheckedOK && MESconnected && Client != null 
                                && DeivceEquipmentidPlcInfo.lstDeivceEquipmentidPlcInfos.Count > 0 
                                && !DeivceEquipmentidPlcInfo.lstDeivceEquipmentidPlcInfos.Exists(x => x.DeviceRegisterResult == 0))
                                allDeviceRegistered = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        //LogInDB.Error("MESheartBeat线程执行异常：" + ex.ToString());
                        if (ATL.Common.StringResources.IsDefaultLanguage)
                        {
                            LogInDB.Error("MESheartBeat线程执行异常：" + ex.ToString());
                        }
                        else
                        {
                            LogInDB.Error("MESheartBeat thread acted abnormal ：" + ex.ToString());
                        }
                        Thread.Sleep(60000);
                    }
                }
                #endregion

                #region MES心跳

                if ((DateTime.Now - lastMesHeartBeatTime).TotalSeconds > 10 && MESconnected && Client != null && Current != null)
                {
                    string msg = string.Empty;
                    try
                    {
                        lastMesHeartBeatTime = DateTime.Now;

                        A006.Root root = Current.A005();
                        if (root != null && root.ResponseInfo != null && root.ResponseInfo.Result != null && root.ResponseInfo.Result.ToUpper() == "OK")
                        {
                            LastMesHeartBeatOKTime = DateTime.Now;
                            HeartBeatError = false;
                            heartBeatError = 0;
                        }
                        else
                            heartBeatError++;
                        if (heartBeatError >= 3)
                        {
                            main_server = false;
                            HeartBeatError = true;
                            heartBeatError = 0;
                            if (ATL.Common.StringResources.IsDefaultLanguage)
                            {
                                msg = "切换备用服务器";
                            }
                            else
                            {
                                msg = "Switch standby server";
                            }
                            LogInDB.Info(msg);
                            current.RecordConnErrorInfo(msg);
                            Current.StartBackup();
                        }
                    }
                    catch (Exception ex)
                    {
                        //LogInDB.Error("MES心跳执行异常：" + ex.ToString());
                        if (ATL.Common.StringResources.IsDefaultLanguage)
                        {
                            msg = "MES心跳执行异常：" + ex.ToString();
                        }
                        else
                        {
                            msg = "MES heartbeat acted abnormal ：" + ex.ToString();
                        }
                        LogInDB.Info(msg);
                        current.RecordConnErrorInfo(msg);
                        Thread.Sleep(60000);
                    }
                }
                #endregion

                #region Ping MES服务器
                if ((DateTime.Now - LastPingMesServer).TotalSeconds > 5)
                {
                    string msg = string.Empty;
                    try
                    {
                        LastPingMesServer = DateTime.Now;
                        Ping ping = new Ping();
                        PingReply reply = ping.Send(_serverIpAdr.Split(':')[1]);
                        if (reply.Status != IPStatus.Success)
                        {
                            if (ATL.Common.StringResources.IsDefaultLanguage)
                            {
                                msg = $"本机IP：{localIpAdr} ping {_serverIpAdr} 失败！！";
                            }
                            else
                            {
                                msg = $"local IP：{localIpAdr} ping {_serverIpAdr} failure！！";
                            }
                            LogInDB.Info(msg);
                            current.RecordConnErrorInfo(msg);
                        }
                        else
                        {
                            if(reply.RoundtripTime > 50)
                            {
                                if (ATL.Common.StringResources.IsDefaultLanguage)
                                {
                                    msg = $"本机IP：{localIpAdr} ping {_serverIpAdr} 时间{reply.RoundtripTime}ms";
                                }
                                else
                                {
                                    msg = $"local IP：{localIpAdr} ping {_serverIpAdr} {reply.RoundtripTime}ms";
                                }
                                LogInDB.Info(msg);
                                current.RecordConnErrorInfo(msg);
                            }
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                }
                #endregion
                   
            }
        }

        private static void PlcHeartBeat(object state)
        {
            DateTime lastPLCHeatBeatTime = DateTime.Now.AddSeconds(-3);
            Thread.Sleep(2000);
            bool totalPLCState = true;
            while (!ATL.Core.Core.SoftClosing)
            {
                Thread.Sleep(1000);
                
                if(!ATL.Core.Core.CheckedOK) continue;
                totalPLCState = true;
                
                foreach (var v in PlcConfigurationInfo.lstPlcConfiguration.Where(x => x.Enabled == 1))
                {
                    totalPLCState = totalPLCState && v.PLC.IsConnect;
                }
                try
                {
                    if ((!totalPLCState || ID_Device.lstDevices.Count == 0) && PlcConfigurationInfo.lstPlcConfiguration.Where(x => x.Enabled == 1).ToList().Count > 0)
                        continue;
                }
                catch (Exception ex)
                {

                }
                #region MES连接状态、MesReplyOK、控机ControlCode、心跳给PLC。为了保证通讯恢复正常后也能够正常写入控机指令，所以每隔几秒钟写一次。
                try
                {
                    if (ATL.Core.Core.CheckedOK && (DateTime.Now - lastPLCHeatBeatTime).TotalSeconds > 3 && allDeviceRegistered)
                    {
                        lastPLCHeatBeatTime = DateTime.Now;
                        UserVariableValueService Service = new UserVariableValueService();
                        Service.WriteStatusToPLC();
                    }
                }
                catch (Exception ex)
                {
                    LogInDB.Error(ex.ToString());
                    Thread.Sleep(3000);
                }
                #endregion
            }
        }

        private static void HmiLogin(string plcAccount, string plcCode, string UserLevelAddress, string HmiPermissionRequestAddress)
        {
            if (!UserDefineVariableInfo.DicVariables._dict.ContainsKey(UserLevelAddress) || !UserDefineVariableInfo.DicVariables._dict.ContainsKey(HmiPermissionRequestAddress)) return;
            Engine.PLC plc = new Engine.PLC();
            try
            {
                ATL.MES.A040.Root root = Current.A039(plcAccount, plcCode);
                if (root != null)
                {
                    //TODO:zxh 从PLC读回来的账号密码只有前两位，导致Mes返回的权限有问题，暂时这样处理
                    //if (root.ResponseInfo.UserLevel == "")
                    //{
                    //    root.ResponseInfo.UserLevel = "Administrator";
                    //}
                    RoleInfo role = RoleInfo.lstRoleInfos.Where(x => x.MesUserLevel == root.ResponseInfo.UserLevel).FirstOrDefault();
                    //LogInDB.Info($"HMI登录账号:{plcAccount} 登陆MES成功，权限为:{root.ResponseInfo.UserLevel}");
                    if (ATL.Common.StringResources.IsDefaultLanguage)
                    {
                        LogInDB.Info($"HMI登录账号:{plcAccount} 登陆MES成功，权限为:{root.ResponseInfo.UserLevel}");
                    }
                    else
                    {
                        LogInDB.Info($"HMI login account :{plcAccount}login to MES successfully，the permission is :{root.ResponseInfo.UserLevel}");
                    }
                    if (role != null)
                    {
                        if (!plc.WriteByVariableName(UserLevelAddress, role.UserLevelPLCValue))
                        {
                            //string msg = $"通信失败，没有写入权限到PLC";
                            string msg = string.Empty;
                            if (ATL.Common.StringResources.IsDefaultLanguage)
                            {
                                msg = $"通信失败，没有写入权限到PLC";
                            }
                            else
                            {
                                msg = $"Communication failure, no write permission to PLC ";
                            }
                            LogInDB.Info(msg);
                            goto skip;
                        }
                    }
                    else
                    {
                        //string msg = $"MES返回的权限:{root.ResponseInfo.UserLevel}，不在本地数据库表配置里面";
                        string msg = string.Empty;
                        if (ATL.Common.StringResources.IsDefaultLanguage)
                        {
                            msg = $"MES返回的权限:{root.ResponseInfo.UserLevel}，不在本地数据库表配置里面";
                        }
                        else
                        {
                            msg = $"The permissions returned by MES :{root.ResponseInfo.UserLevel}，It is not in the local configuration table of database";
                        }
                        //MessageBox.Show(msg);
                        LogInDB.Info(msg);
                        plc.WriteByVariableName(UserLevelAddress, 0);
                    }    
                }
                else
                {
                    UserDefineVariableInfo.DicVariables[UserLevelAddress] = 0;
                    plc.WriteByVariableName(UserLevelAddress, 0);
                    //string msg = $"账号:{plcAccount} 登陆MES失败";
                    string msg = string.Empty;
                    if (ATL.Common.StringResources.IsDefaultLanguage)
                    {
                        msg = $"账号:{plcAccount} 登陆MES失败";
                    }
                    else
                    {
                        msg = $"Account :{plcAccount} that failed to login MES ";
                    }
                    MessageBox.Show(msg);
                    LogInDB.Info(msg);
                }
                UserDefineVariableInfo u = UserDefineVariableInfo.lstUserDefineVariables.Where(x => x.VariableName == HmiPermissionRequestAddress).FirstOrDefault();
                if (plc.WriteByVariableName(u.VariableName, 0))
                {
                    UserDefineVariableInfo.DicVariables[HmiPermissionRequestAddress] = 0;
                    plc.WriteByVariableName(HmiPermissionRequestAddress, 0);
                    Thread.Sleep(1000);
                }
                skip:
                int jj;
            }
            catch (Exception ex)
            {
                LogInDB.Error(ex.ToString());
                Thread.Sleep(3000);
            }
        }

        ~InterfaceClient()
        {
            Close();
        }

        public static void Close()
        {
            try
            {
                if (Client != null)
                {
                    MESconnected = false;
                    Client.Close();
                    Client = null;
                }
            }
            catch
            {

            }
        }

        private void Disconnect()
        {
            main_server = false;
            MESconnected = false;
            string msg = string.Empty;
            if (ATL.Common.StringResources.IsDefaultLanguage)
            {
                msg = "MES 连接断开";
            }
            else
            {
                msg = "MES disconnected ";
            }
            LogInDB.Info(msg);
            RecordConnErrorInfo(msg);
            if (!ATL.Core.Core.SoftClosing)
            {
                //LogInDB.Info("切换备用服务器");
                if (ATL.Common.StringResources.IsDefaultLanguage)
                {
                    msg = "切换备用服务器";
                }
                else
                {
                    msg = "Switch standby server ";
                }
                LogInDB.Info(msg);
                RecordConnErrorInfo(msg);
                StartBackup();
            }
        }

        private void BackupDisconnect()
        {
            MESconnected = false;
            string msg = string.Empty;
            if (ATL.Common.StringResources.IsDefaultLanguage)
            {
                msg = "MES备用服务器 连接断开";
            }
            else
            {
                msg = "MES standby server disconnected ";
            }
            LogInDB.Info(msg);
            RecordConnErrorInfo(msg);
            if (Client != null)
            {
                Client.Close();
                Client = null;
            }
        }

        private void ConnectError(ClientConnectionManager.ConnectionErrorStat eStat)
        {
            string msg = string.Empty;
            if (eStat == ClientConnectionManager.ConnectionErrorStat.UdpSocketError)
            {
                //LogInDB.Error("UDP Socket Error! MES主服务器连接失败");
                if (ATL.Common.StringResources.IsDefaultLanguage)
                {
                    msg = "UDP Socket Error! MES主服务器连接失败";
                }
                else
                {
                    msg = "UDP Socket Error! MES master connection failure ";
                }
                LogInDB.Info(msg);
                RecordConnErrorInfo(msg);
            }
            else if (eStat == ClientConnectionManager.ConnectionErrorStat.TcpSocketError)
            {
                //LogInDB.Error("TCP Socket Error! MES主服务器连接失败");
                if (ATL.Common.StringResources.IsDefaultLanguage)
                {
                    msg = "TCP Socket Error! MES主服务器连接失败";
                }
                else
                {
                    msg = "TCP Socket Error! MES master connection failure";
                }
                LogInDB.Info(msg);
                RecordConnErrorInfo(msg);
            }
            MESconnected = false;
        }

        private void BackupConnectError(ClientConnectionManager.ConnectionErrorStat eStat)
        {
            if (eStat == ClientConnectionManager.ConnectionErrorStat.UdpSocketError)
            {
                string msg = string.Empty;
                if (ATL.Common.StringResources.IsDefaultLanguage)
                {
                    msg = "UDP Socket Error! MES备用服务器连接失败";
                }
                else
                {
                    msg = "UDP Socket Error! MES standby server connection failed ";
                }
                LogInDB.Info(msg);
                RecordConnErrorInfo(msg);
            }
            else if (eStat == ClientConnectionManager.ConnectionErrorStat.TcpSocketError)
            {
                string msg = string.Empty;
                if (ATL.Common.StringResources.IsDefaultLanguage)
                {
                    msg = "TCP Socket Error! MES备用服务器连接失败";
                }
                else
                {
                    msg = "TCP Socket Error! MES standby server connection failed";
                }
                LogInDB.Info(msg);
                RecordConnErrorInfo(msg);
            }
            MESconnected = false;
        }

        private void ClientConnect()
        {
            //LogInDB.Info("Connect MES主服务器 Success!");
            if (ATL.Common.StringResources.IsDefaultLanguage)
            {
                LogInDB.Info("Connect MES主服务器 Success!");
            }
            else
            {
                LogInDB.Info("Connect MES master server succeed!");
            }
            MESconnected = true;
            main_server = true;
        }

        private void BackupClientConnect()
        {
            //LogInDB.Info("Connect MES备用服务器 Success!");
            if (ATL.Common.StringResources.IsDefaultLanguage)
            {
                LogInDB.Info("Connect MES备用服务器 Success!");
            }
            else
            {
                LogInDB.Info("Connect MES standby server succeed!");
            }
            MESconnected = true;
        }
        #region 接口缓存
        RecvdData Test_Buffer = new RecvdData();
        RecvdData A002_Buffer = new RecvdData();
        RecvdData A006_Buffer = new RecvdData();
        RecvdData A014_Buffer1;
        RecvdData A014_Buffer2;
        RecvdData A014_Buffer3;
        RecvdData A014_Buffer4;
        RecvdData A016_Buffer1;
        RecvdData A016_Buffer2;
        RecvdData A016_Buffer3;
        RecvdData A016_Buffer4;
        RecvdData A020_Buffer = new RecvdData();
        RecvdData A022_Buffer = new RecvdData();
        RecvdData A024_Buffer = new RecvdData();
        RecvdData A026_Buffer = new RecvdData();
        RecvdData A030_Buffer1;
        RecvdData A030_Buffer2;
        RecvdData A030_Buffer3;
        RecvdData A030_Buffer4;
        RecvdData A032_Buffer = new RecvdData();
        RecvdData A034_Buffer = new RecvdData();
        RecvdData A036_Buffer = new RecvdData();
        RecvdData A038_Buffer = new RecvdData();
        RecvdData A040_Buffer = new RecvdData();
        RecvdData A042_Buffer = new RecvdData();
        RecvdData A044_Buffer = new RecvdData();
        RecvdData A046_Buffer = new RecvdData();
        RecvdData A050_Buffer = new RecvdData();
        RecvdData A052_Buffer = new RecvdData();
        RecvdData A056_Buffer = new RecvdData();
        RecvdData A063_Buffer = new RecvdData();
        RecvdData A065_Buffer = new RecvdData();
        RecvdData A068_Buffer = new RecvdData();
        RecvdData A070_Buffer = new RecvdData();
        RecvdData A072_Buffer = new RecvdData();
        RecvdData A074_Buffer = new RecvdData();
        RecvdData A076_Buffer = new RecvdData();
        RecvdData A078_Buffer = new RecvdData();
        RecvdData A080_Buffer = new RecvdData();
        RecvdData A082_Buffer = new RecvdData();
        RecvdData A084_Buffer = new RecvdData();
        RecvdData A086_Buffer = new RecvdData();
        RecvdData A088_Buffer = new RecvdData();
        RecvdData A090_Buffer = new RecvdData();
        RecvdData A092_Buffer = new RecvdData();
        RecvdData A094_Buffer = new RecvdData();
        RecvdData A096_Buffer = new RecvdData();
        RecvdData A098_Buffer = new RecvdData();
        RecvdData A100_Buffer = new RecvdData();


        #endregion
        
        private object lockA014 = new object();
        private object lockA016 = new object();
        private object lockA030 = new object();

        private void funcName(object obj)
        {
            RecvdData recvdData = (RecvdData)obj;
            try
            {
                switch (recvdData.cmd)
                {
                    case "A002":
                        A002_Buffer = recvdData;
                        break;
                    case "A006":
                        A006_Buffer = recvdData;
                        break;
                    case "A014":
                        {
                            lock (lockA014)
                            {
                                if (A014_Buffer1 == null)
                                {
                                    A014_Buffer1 = recvdData;
                                }
                                else if (A014_Buffer2 == null)
                                {
                                    A014_Buffer2 = recvdData;
                                }
                                else if (A014_Buffer3 == null)
                                {
                                    A014_Buffer3 = recvdData;
                                }
                                else if (A014_Buffer4 == null)
                                {
                                    A014_Buffer4 = recvdData;
                                }
                                else
                                {
                                    try
                                    {
                                        DateTime dt1 = DateTime.ParseExact(A014_Buffer1.receivedTime, "yyyy-MM-dd HH:mm:ss fff", System.Globalization.CultureInfo.CurrentCulture);
                                        DateTime dt2 = DateTime.ParseExact(A014_Buffer2.receivedTime, "yyyy-MM-dd HH:mm:ss fff", System.Globalization.CultureInfo.CurrentCulture);
                                        DateTime dt3 = DateTime.ParseExact(A014_Buffer3.receivedTime, "yyyy-MM-dd HH:mm:ss fff", System.Globalization.CultureInfo.CurrentCulture);
                                        DateTime dt4 = DateTime.ParseExact(A014_Buffer4.receivedTime, "yyyy-MM-dd HH:mm:ss fff", System.Globalization.CultureInfo.CurrentCulture);
                                        if (dt1 < dt2 && dt1 < dt3 && dt1 < dt4)
                                        {
                                            A014_Buffer1 = null;
                                            A014_Buffer1 = recvdData;
                                            dt1 = DateTime.ParseExact(A014_Buffer1.receivedTime, "yyyy-MM-dd HH:mm:ss fff", System.Globalization.CultureInfo.CurrentCulture);
                                        }
                                        else if (dt2 < dt1 && dt2 < dt3 && dt2 < dt4)
                                        {
                                            A014_Buffer2 = null;
                                            A014_Buffer2 = recvdData;
                                            dt2 = DateTime.ParseExact(A014_Buffer2.receivedTime, "yyyy-MM-dd HH:mm:ss fff", System.Globalization.CultureInfo.CurrentCulture);
                                        }
                                        else if (dt3 < dt1 && dt3 < dt2 && dt3 < dt4)
                                        {
                                            A014_Buffer3 = null;
                                            A014_Buffer3 = recvdData;
                                            dt3 = DateTime.ParseExact(A014_Buffer3.receivedTime, "yyyy-MM-dd HH:mm:ss fff", System.Globalization.CultureInfo.CurrentCulture);
                                        }
                                        else if (dt4 < dt1 && dt4 < dt2 && dt4 < dt3)
                                        {
                                            A014_Buffer4 = null;
                                            A014_Buffer4 = recvdData;
                                            dt4 = DateTime.ParseExact(A014_Buffer4.receivedTime, "yyyy-MM-dd HH:mm:ss fff", System.Globalization.CultureInfo.CurrentCulture);
                                        }
                                        //清空超时1min未处理的缓存
                                        if ((DateTime.Now - dt1).TotalSeconds > 60)
                                        {
                                            A014_Buffer1 = null;
                                        }
                                        if ((DateTime.Now - dt2).TotalSeconds > 60)
                                        {
                                            A014_Buffer2 = null;
                                        }
                                        if ((DateTime.Now - dt3).TotalSeconds > 60)
                                        {
                                            A014_Buffer3 = null;
                                        }
                                        if ((DateTime.Now - dt4).TotalSeconds > 60)
                                        {
                                            A014_Buffer4 = null;
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        LogInDB.Error(ex.ToString());
                                    }
                                }
                            }
                        }
                        break;
                    case "A016":
                        lock (lockA016)
                        {
                            if (A016_Buffer1 == null)
                            {
                                A016_Buffer1 = recvdData;
                            }
                            else if (A016_Buffer2 == null)
                            {
                                A016_Buffer2 = recvdData;
                            }
                            else if (A016_Buffer3 == null)
                            {
                                A016_Buffer3 = recvdData;
                            }
                            else if (A016_Buffer4 == null)
                            {
                                A016_Buffer4 = recvdData;
                            }
                            else
                            {
                                try
                                {
                                    DateTime dt1 = DateTime.ParseExact(A016_Buffer1.receivedTime, "yyyy-MM-dd HH:mm:ss fff", System.Globalization.CultureInfo.CurrentCulture);
                                    DateTime dt2 = DateTime.ParseExact(A016_Buffer2.receivedTime, "yyyy-MM-dd HH:mm:ss fff", System.Globalization.CultureInfo.CurrentCulture);
                                    DateTime dt3 = DateTime.ParseExact(A016_Buffer3.receivedTime, "yyyy-MM-dd HH:mm:ss fff", System.Globalization.CultureInfo.CurrentCulture);
                                    DateTime dt4 = DateTime.ParseExact(A016_Buffer4.receivedTime, "yyyy-MM-dd HH:mm:ss fff", System.Globalization.CultureInfo.CurrentCulture);
                                    if (dt1 < dt2 && dt1 < dt3 && dt1 < dt4)
                                    {
                                        A016_Buffer1 = null;
                                        A016_Buffer1 = recvdData;
                                        dt1 = DateTime.ParseExact(A016_Buffer1.receivedTime, "yyyy-MM-dd HH:mm:ss fff", System.Globalization.CultureInfo.CurrentCulture);
                                    }
                                    else if (dt2 < dt1 && dt2 < dt3 && dt2 < dt4)
                                    {
                                        A016_Buffer2 = null;
                                        A016_Buffer2 = recvdData;
                                        dt2 = DateTime.ParseExact(A016_Buffer2.receivedTime, "yyyy-MM-dd HH:mm:ss fff", System.Globalization.CultureInfo.CurrentCulture);
                                    }
                                    else if (dt3 < dt1 && dt3 < dt2 && dt3 < dt4)
                                    {
                                        A016_Buffer3 = null;
                                        A016_Buffer3 = recvdData;
                                        dt3 = DateTime.ParseExact(A016_Buffer3.receivedTime, "yyyy-MM-dd HH:mm:ss fff", System.Globalization.CultureInfo.CurrentCulture);
                                    }
                                    else if (dt4 < dt1 && dt4 < dt2 && dt4 < dt3)
                                    {
                                        A016_Buffer4 = null;
                                        A016_Buffer4 = recvdData;
                                        dt4 = DateTime.ParseExact(A016_Buffer4.receivedTime, "yyyy-MM-dd HH:mm:ss fff", System.Globalization.CultureInfo.CurrentCulture);
                                    }
                                    //清空超时1min未处理的缓存
                                    if ((DateTime.Now - dt1).TotalSeconds > 60)
                                    {
                                        A016_Buffer1 = null;
                                    }
                                    if ((DateTime.Now - dt2).TotalSeconds > 60)
                                    {
                                        A016_Buffer2 = null;
                                    }
                                    if ((DateTime.Now - dt3).TotalSeconds > 60)
                                    {
                                        A016_Buffer3 = null;
                                    }
                                    if ((DateTime.Now - dt4).TotalSeconds > 60)
                                    {
                                        A016_Buffer4 = null;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    LogInDB.Error(ex.ToString());
                                }
                            }
                        }
                        break;
                    case "A020":
                        A020_Buffer = recvdData;
                        break;
                    case "A022":
                        A022_Buffer = recvdData;
                        break;
                    case "A024":
                        A024_Buffer = recvdData;
                        break;
                    case "A026":
                        A026_Buffer = recvdData;
                        break;
                    case "A030":
                        lock (lockA030)
                        {
                            if (A030_Buffer1 == null)
                            {
                                A030_Buffer1 = recvdData;
                            }
                            else if (A030_Buffer2 == null)
                            {
                                A030_Buffer2 = recvdData;
                            }
                            else if (A030_Buffer3 == null)
                            {
                                A030_Buffer3 = recvdData;
                            }
                            else if (A030_Buffer4 == null)
                            {
                                A030_Buffer4 = recvdData;
                            }
                            else
                            {
                                try
                                {
                                    DateTime dt1 = DateTime.ParseExact(A030_Buffer1.receivedTime, "yyyy-MM-dd HH:mm:ss fff", System.Globalization.CultureInfo.CurrentCulture);
                                    DateTime dt2 = DateTime.ParseExact(A030_Buffer2.receivedTime, "yyyy-MM-dd HH:mm:ss fff", System.Globalization.CultureInfo.CurrentCulture);
                                    DateTime dt3 = DateTime.ParseExact(A030_Buffer3.receivedTime, "yyyy-MM-dd HH:mm:ss fff", System.Globalization.CultureInfo.CurrentCulture);
                                    DateTime dt4 = DateTime.ParseExact(A030_Buffer4.receivedTime, "yyyy-MM-dd HH:mm:ss fff", System.Globalization.CultureInfo.CurrentCulture);
                                    if (dt1 < dt2 && dt1 < dt3 && dt1 < dt4)
                                    {
                                        A030_Buffer1 = null;
                                        A030_Buffer1 = recvdData;
                                        dt1 = DateTime.ParseExact(A030_Buffer1.receivedTime, "yyyy-MM-dd HH:mm:ss fff", System.Globalization.CultureInfo.CurrentCulture);
                                    }
                                    else if (dt2 < dt1 && dt2 < dt3 && dt2 < dt4)
                                    {
                                        A030_Buffer2 = null;
                                        A030_Buffer2 = recvdData;
                                        dt2 = DateTime.ParseExact(A030_Buffer2.receivedTime, "yyyy-MM-dd HH:mm:ss fff", System.Globalization.CultureInfo.CurrentCulture);
                                    }
                                    else if (dt3 < dt1 && dt3 < dt2 && dt3 < dt4)
                                    {
                                        A030_Buffer3 = null;
                                        A030_Buffer3 = recvdData;
                                        dt3 = DateTime.ParseExact(A030_Buffer3.receivedTime, "yyyy-MM-dd HH:mm:ss fff", System.Globalization.CultureInfo.CurrentCulture);
                                    }
                                    else if (dt4 < dt1 && dt4 < dt2 && dt4 < dt3)
                                    {
                                        A030_Buffer4 = null;
                                        A030_Buffer4 = recvdData;
                                        dt4 = DateTime.ParseExact(A030_Buffer4.receivedTime, "yyyy-MM-dd HH:mm:ss fff", System.Globalization.CultureInfo.CurrentCulture);
                                    }
                                    //清空超时1min未处理的缓存
                                    if ((DateTime.Now - dt1).TotalSeconds > 60)
                                    {
                                        A030_Buffer1 = null;
                                    }
                                    if ((DateTime.Now - dt2).TotalSeconds > 60)
                                    {
                                        A030_Buffer2 = null;
                                    }
                                    if ((DateTime.Now - dt3).TotalSeconds > 60)
                                    {
                                        A030_Buffer3 = null;
                                    }
                                    if ((DateTime.Now - dt4).TotalSeconds > 60)
                                    {
                                        A030_Buffer4 = null;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    LogInDB.Error(ex.ToString());
                                }
                            }
                        }
                        break;
                    case "A032":
                        A032_Buffer = recvdData;
                        break;
                    case "A034":
                        A034_Buffer = recvdData;
                        break;
                    case "A036":
                        A036_Buffer = recvdData;
                        break;
                    case "A038":
                        A038_Buffer = recvdData;
                        break;
                    case "A040":
                        A040_Buffer = recvdData;
                        break;
                    case "A042":
                        A042_Buffer = recvdData;
                        break;
                    case "A044":
                        A044_Buffer = recvdData;
                        break;
                    case "A046":
                        A046_Buffer = recvdData;
                        break;
                    case "A050":
                        A050_Buffer = recvdData;
                        break;
                    case "A052":
                        A052_Buffer = recvdData;
                        break;
                    case "A056":
                        A056_Buffer = recvdData;
                        break;
                    case "A063":
                        A063_Buffer = recvdData;
                        break;
                    case "A065":
                        A065_Buffer = recvdData;
                        break;
                    case "A068":
                        A068_Buffer = recvdData;
                        break;
                    case "A070":
                        A070_Buffer = recvdData;
                        break;
                    case "A072":
                        A072_Buffer = recvdData;
                        break;
                    case "A074":
                        A074_Buffer = recvdData;
                        break;
                    case "A076":
                        A076_Buffer = recvdData;
                        break;
                    case "A078":
                        A078_Buffer = recvdData;
                        break;
                    case "A080":
                        A080_Buffer = recvdData;
                        break;
                    case "A082":
                        A082_Buffer = recvdData;
                        break;
                    case "A084":
                        A084_Buffer = recvdData;
                        break;
                    case "A086":
                        A086_Buffer = recvdData;
                        break;
                    case "A088":
                        A088_Buffer = recvdData;
                        break;
                    case "A090":
                        A090_Buffer = recvdData;
                        break;
                    case "A092":
                        A092_Buffer = recvdData;
                        break;
                    case "A094":
                        A094_Buffer = recvdData;
                        break;
                    case "A096":
                        A096_Buffer = recvdData;
                        break;
                    case "A098":
                        A098_Buffer = recvdData;
                        break;
                    case "A100":
                        A100_Buffer = recvdData;
                        break;
                    default:
                        break;
                }

                if (recvdData.cmd.Contains("A007"))//MES发送初始化开机指令,内容主要包括设备关键部件及设备运行标准参数
                {
                    #region A007
                    AnalyseA007Jason(recvdData);
                    #endregion
                }
                else if (recvdData.cmd.Contains("A011"))//发送设备控制指令请求
                {
                    #region A011
                    A011.Root root = JsonNewtonsoft.FromJSON<A011.Root>(recvdData.jason);
                    if (root != null)
                    {
                        MESmsg msg = new MESmsg();
                        msg.FunctionID = root.Header.FunctionID;
                        //msg.translation = $"{DateTime.Now.ToString("MM-dd HH:mm:ss")} MES推送给设备ID{root.Header.EQCode}如下{msg.FunctionID}接口的信息:";
                        if (ATL.Common.StringResources.IsDefaultLanguage)
                        {
                            msg.translation = $"{DateTime.Now.ToString("MM-dd HH:mm:ss")} MES推送给设备ID{root.Header.EQCode}如下{msg.FunctionID}接口的信息:";
                        }
                        else
                        {
                            msg.translation = $"{DateTime.Now.ToString("MM-dd HH:mm:ss")} MES push to device ID{ root.Header.EQCode }as follows:{ msg.FunctionID }interface information :";
                        }

                        DAL.me.InsertMesInterfaceLog("A011", root.Header.SessionID,
                            DateTime.ParseExact(root.Header.RequestTime, "yyyy-MM-dd HH:mm:ss fff", System.Globalization.CultureInfo.CurrentCulture).ToString("yyyy-MM-dd HH:mm:ss.fff"),
                            null, recvdData.jason, "");
                        DeivceEquipmentidPlcInfo deivceEquipmentidPlcInfo = DeivceEquipmentidPlcInfo.lstDeivceEquipmentidPlcInfos.Where(x => x.EquipmentID == root.Header.EQCode).FirstOrDefault();
                        if (deivceEquipmentidPlcInfo != null)
                        {
                            lock (DeivceEquipmentidPlcInfo.LockObj)
                            {
                                deivceEquipmentidPlcInfo.ControlCode = root.RequestInfo.ControlCode != null ? root.RequestInfo.ControlCode.ToUpper() : string.Empty;
                                deivceEquipmentidPlcInfo.ProductMode = "0";
                                deivceEquipmentidPlcInfo.StateCode = root.RequestInfo.StateCode;
                                deivceEquipmentidPlcInfo.StateDesc = root.RequestInfo.StateDesc;
                            }
                            deivceEquipmentidPlcInfo.LastMesUpdateCountDateTime = DateTime.Now;
                            //LogInDB.Info($"MES通过A011接口发送当前模式为正常运行模式，StateCode：{root.RequestInfo.StateCode}");
                            if (ATL.Common.StringResources.IsDefaultLanguage)
                            {
                                LogInDB.Info($"MES通过A011接口发送当前模式为正常运行模式，StateCode：{root.RequestInfo.StateCode}");
                            }
                            else
                            {
                                LogInDB.Info($"MES sends the current mode to normal operation mode through A011 interface ，StateCode：{root.RequestInfo.StateCode}");
                            }
                        }
                        msg.translation += Environment.NewLine + $"指令:{root.RequestInfo.ControlCode}, 状态:{root.RequestInfo.StateCode},描述:{root.RequestInfo.StateDesc}";

                        ATL.Station.Station.Current.AddMESmsg($"时间:{root.Header.RequestTime}：MES控机指令：   EQCode:{root.Header.EQCode}, StateCode:{root.RequestInfo.StateCode}, StateDesc:{root.RequestInfo.StateDesc}");
                        Thread t = new Thread(new ParameterizedThreadStart(A012));
                        t.Start(root);
                        LogInDB.Info(msg.translation);
                    }
                    else
                    {
                        int indexGuidBegin = recvdData.jason.IndexOf("SessionID") + 12;
                        string s = recvdData.jason.Substring(indexGuidBegin);
                        int indexGuidEnd = s.IndexOf("\"");
                        string guid = s.Substring(0, indexGuidEnd);
                        string FunctionID = recvdData.cmd;
                        DAL.me.InsertMesInterfaceLog("A011", guid, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), recvdData.jason, "Jason转对象失败");
                    }
                    #endregion
                }
                else if (recvdData.cmd.Contains("A017"))//MES主动采集设备数据
                {
                    #region A017
                    A017.Root root = JsonNewtonsoft.FromJSON<A017.Root>(recvdData.jason);
                    if (root != null)
                    {
                        Thread t = new Thread(new ParameterizedThreadStart(A018));
                        t.Start(root);
                        MESmsg msg = new MESmsg();
                        msg.FunctionID = root.Header.FunctionID;
                        //msg.translation = $"{DateTime.Now.ToString("MM-dd HH:mm:ss")} MES推送给设备ID{root.Header.EQCode}如下{msg.FunctionID}接口的信息:";
                        if (ATL.Common.StringResources.IsDefaultLanguage)
                        {
                            msg.translation = $"{DateTime.Now.ToString("MM-dd HH:mm:ss")} MES推送给设备ID{root.Header.EQCode}如下{msg.FunctionID}接口的信息:";
                        }
                        else
                        {
                            msg.translation = $"{DateTime.Now.ToString("MM-dd HH:mm:ss")} MES push to device ID{ root.Header.EQCode }as follows:{ msg.FunctionID }interface information :";
                        }

                        DAL.me.InsertMesInterfaceLog("A017", root.Header.SessionID,
                            DateTime.ParseExact(root.Header.RequestTime, "yyyy-MM-dd HH:mm:ss fff", System.Globalization.CultureInfo.CurrentCulture).ToString("yyyy-MM-dd HH:mm:ss.fff"),
                            null, recvdData.jason, "");
                        if (root.RequestInfo != null && root.RequestInfo.ParamInfo != null && root.RequestInfo.ParamInfo.Count > 0)
                        {
                            foreach (var v in root.RequestInfo.ParamInfo)
                            {
                                if (ATL.Common.StringResources.IsDefaultLanguage)
                                {
                                    msg.translation += Environment.NewLine + $"MES需要采集参数ID:{v.ParamID},参数名称:{v.ParamDesc}";
                                }
                                else
                                {
                                    msg.translation += Environment.NewLine + $"MES need collect parameter ID:{v.ParamID}, parameter name:{v.ParamDesc}";
                                }

                            }
                        }
                        LogInDB.Info(msg.translation);
                    }
                    else
                    {
                        int indexGuidBegin = recvdData.jason.IndexOf("SessionID") + 12;
                        string s = recvdData.jason.Substring(indexGuidBegin);
                        int indexGuidEnd = s.IndexOf("\"");
                        string guid = s.Substring(0, indexGuidEnd);
                        string FunctionID = recvdData.cmd;
                        DAL.me.InsertMesInterfaceLog("A017", guid, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), recvdData.jason, "Jason转对象失败");
                    }
                    #endregion
                }
                else if (recvdData.cmd.Contains("A047"))//MES下发物料信息请求
                {
                    #region A047
                    A047.Root root = JsonNewtonsoft.FromJSON<A047.Root>(recvdData.jason);
                    if (root != null)
                    {
                        Thread t = new Thread(new ParameterizedThreadStart(newThreadA048));
                        t.Start(root);
                        
                        MESmsg msg = new MESmsg();
                        msg.FunctionID = root.Header.FunctionID;
                        //msg.translation = $"{DateTime.Now.ToString("MM-dd HH:mm:ss")} MES推送给设备ID{root.Header.EQCode}如下{msg.FunctionID}接口的信息:";
                        if (ATL.Common.StringResources.IsDefaultLanguage)
                        {
                            msg.translation = $"{DateTime.Now.ToString("MM-dd HH:mm:ss")} MES推送给设备ID{root.Header.EQCode}如下{msg.FunctionID}接口的信息:";
                        }
                        else
                        {
                            msg.translation = $"{DateTime.Now.ToString("MM-dd HH:mm:ss")} MES push to device ID{ root.Header.EQCode }as follows:{ msg.FunctionID }interface information :";
                        }
                        DAL.me.InsertMesInterfaceLog("A047", root.Header.SessionID,
                            DateTime.ParseExact(root.Header.RequestTime, "yyyy-MM-dd HH:mm:ss fff", System.Globalization.CultureInfo.CurrentCulture).ToString("yyyy-MM-dd HH:mm:ss.fff"),
                            null, recvdData.jason, "");
                        A047RequestInfo a047RequestInfo;
                        lock (LocklstA047RequestFromMESObj)
                        {
                            a047RequestInfo = lstA047RequestFromMES.Where(x => x.EquipmentID == root.Header.EQCode).FirstOrDefault();
                            if (a047RequestInfo == null)
                            {
                                a047RequestInfo = new A047RequestInfo();
                                a047RequestInfo.EquipmentID = root.Header.EQCode;
                                a047RequestInfo.UserInfo = new A047UserInfo();
                                lstA047RequestFromMES.Add(a047RequestInfo);
                            }
                        }
                        //a047RequestInfo.ModelInfo = string.Empty;
                        a047RequestInfo.MaterialInfo = null;

                        if (root.RequestInfo != null)
                        {
                            if (root.RequestInfo.UserInfo != null)
                            {
                                if (ATL.Common.StringResources.IsDefaultLanguage)
                                {
                                    msg.translation += Environment.NewLine + $"MES下发物料信息,用户ID:{root.RequestInfo.UserInfo.UserID},用户名称:{root.RequestInfo.UserInfo.UserName},用户权限:{root.RequestInfo.UserInfo.UserLevel}";
                                }
                                else
                                {
                                    msg.translation += Environment.NewLine + $"MES issued material shortage information, user ID:{root.RequestInfo.UserInfo.UserID},user name:{root.RequestInfo.UserInfo.UserName}, user privilege:{root.RequestInfo.UserInfo.UserLevel}";
                                }
                            }
                            if (ATL.Common.StringResources.IsDefaultLanguage)
                            {
                                msg.translation += Environment.NewLine + $"MES下发物料的产品型号:{root.RequestInfo.ModelInfo}";
                            }
                            else
                            {
                                msg.translation += Environment.NewLine + $"MES issued material model:{root.RequestInfo.ModelInfo}";
                            }


                            //a047RequestInfo.ModelInfo = root.RequestInfo.ModelInfo;
                            a047RequestInfo.MaterialInfo = root.RequestInfo.MaterialInfo;
                            lock (LocklstA047RequestFromMESObj)
                            {
                                try
                                {
                                    WRSerializable.SerializeToFile(lstA047RequestFromMES, System.IO.Directory.GetCurrentDirectory() + "\\lstA047RequestFromMES.dat");
                                }
                                catch (Exception ex)
                                {
                                    //LogInDB.Error($"序列化[lstA047RequestFromMES]failure：{ex.ToString()}");
                                    if (ATL.Common.StringResources.IsDefaultLanguage)
                                    {
                                        LogInDB.Error($"序列化[lstA047RequestFromMES]failure：{ex.ToString()}");
                                    }
                                    else
                                    {
                                        LogInDB.Error($"Failed to serialize [lsta047requestfrommes]：{ex.ToString()}");
                                    }
                                }
                            }
                            if (root.RequestInfo.MaterialInfo != null && root.RequestInfo.MaterialInfo.Count > 0)
                            {
                                foreach (var v in root.RequestInfo.MaterialInfo)
                                {
                                    if (ATL.Common.StringResources.IsDefaultLanguage)
                                    {
                                        msg.translation += Environment.NewLine + $"MES下发物料ID:{v.MaterialID},名称:{v.MaterialName},数量:{v.MaterialQuality},标签号:{v.LabelNumber},UoM:{v.UoM}";
                                    }
                                    else
                                    {
                                        msg.translation += Environment.NewLine + $"MES issued material ID:{v.MaterialID},name:{v.MaterialName},count:{v.MaterialQuality},tag:{v.LabelNumber},UoM:{v.UoM}";
                                    }
                                }
                            }
                        }
                        LogInDB.Info(msg.translation);
                    }
                    else
                    {
                        int indexGuidBegin = recvdData.jason.IndexOf("SessionID") + 12;
                        string s = recvdData.jason.Substring(indexGuidBegin);
                        int indexGuidEnd = s.IndexOf("\"");
                        string guid = s.Substring(0, indexGuidEnd);
                        string FunctionID = recvdData.cmd;
                        DAL.me.InsertMesInterfaceLog("A047", guid, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), recvdData.jason, "Jason转对象失败");
                    }
                    #endregion
                }
                else if (recvdData.cmd.Contains("A053"))//MES向ACT发送路径
                {
                    #region A053
                    A053.Root root = JsonNewtonsoft.FromJSON<A053.Root>(recvdData.jason);
                    if (root != null)
                    {
                        A053FolderPath.Add(root.RequestInfo.FolderPath);
                        Thread t = new Thread(new ParameterizedThreadStart(newThreadA054));
                        t.Start(root);
                        MESmsg msg = new MESmsg();
                        msg.FunctionID = root.Header.FunctionID;
                        //msg.translation = $"{DateTime.Now.ToString("MM-dd HH:mm:ss")} MES推送给设备ID{root.Header.EQCode}如下{msg.FunctionID}接口的信息:";
                        //msg.translation += Environment.NewLine + $"MES向ACT发送路径:{root.RequestInfo.FolderPath}";
                        if (ATL.Common.StringResources.IsDefaultLanguage)
                        {
                            msg.translation = $"{DateTime.Now.ToString("MM-dd HH:mm:ss")} MES推送给设备ID{root.Header.EQCode}如下{msg.FunctionID}接口的信息:";
                            msg.translation += Environment.NewLine + $"MES向ACT发送路径:{root.RequestInfo.FolderPath}";
                        }
                        else
                        {
                            msg.translation = $"{DateTime.Now.ToString("MM-dd HH:mm:ss")} MES push to device ID{ root.Header.EQCode }as follows:{ msg.FunctionID }interface information :";
                            msg.translation += Environment.NewLine + $"MES send path to ACT:{root.RequestInfo.FolderPath}";
                        }
                        LogInDB.Info(msg.translation);
                        DAL.me.InsertMesInterfaceLog("A053", root.Header.SessionID,
                            DateTime.ParseExact(root.Header.RequestTime, "yyyy-MM-dd HH:mm:ss fff", System.Globalization.CultureInfo.CurrentCulture).ToString("yyyy-MM-dd HH:mm:ss.fff"),
                            null, recvdData.jason, "");
                    }
                    else
                    {
                        int indexGuidBegin = recvdData.jason.IndexOf("SessionID") + 12;
                        string s = recvdData.jason.Substring(indexGuidBegin);
                        int indexGuidEnd = s.IndexOf("\"");
                        string guid = s.Substring(0, indexGuidEnd);
                        string FunctionID = recvdData.cmd;
                        DAL.me.InsertMesInterfaceLog("A053", guid, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), recvdData.jason, "Jason转对象失败");
                    }
                    #endregion
                }
                else if (recvdData.cmd.Contains("A060"))//发送设备地址配置请求
                {
                    #region A060
                    A060.Root root = JsonNewtonsoft.FromJSON<A060.Root>(recvdData.jason);
                    if (root != null)
                    {
                        foreach (var deivceEquipmentidPlcInfo in DeivceEquipmentidPlcInfo.lstDeivceEquipmentidPlcInfos)
                        {
                            lock (DeivceEquipmentidPlcInfo.LockObj)
                            {
                                deivceEquipmentidPlcInfo.ControlCode = "STOP";
                                if (ATL.Common.StringResources.IsDefaultLanguage)
                                {
                                    deivceEquipmentidPlcInfo.StateDesc = "MES下发A060配置，停机运行";
                                }
                                else
                                {
                                    deivceEquipmentidPlcInfo.StateDesc = "MES issued A060 config，stop running";
                                }

                            }
                        }
                        ATL.Core.Core.CheckedOK = false;
                        ATL.Core.PLC.Communicating = false;
                        Thread t = new Thread(new ParameterizedThreadStart(A061));
                        t.Start(root);
                        MESmsg msg = new MESmsg();
                        msg.FunctionID = root.Header.FunctionID;
                        //msg.translation = $"{DateTime.Now.ToString("MM-dd HH:mm:ss")} MES推送给设备ID{root.Header.EQCode}如下{msg.FunctionID}接口的地址配置信息. 请核对参数是否OK后再重新启动生产";
                        if (ATL.Common.StringResources.IsDefaultLanguage)
                        {
                            msg.translation = $"{DateTime.Now.ToString("MM-dd HH:mm:ss")} MES推送给设备ID{root.Header.EQCode}如下{msg.FunctionID}接口的地址配置信息. 请核对参数是否OK后再重新启动生产";
                        }
                        else
                        {
                            msg.translation = $"{DateTime.Now.ToString("MM-dd HH:mm:ss")} MES push to device ID{ root.Header.EQCode }as follows:{ msg.FunctionID }address configuration information of the interface . Please check whether the parameters are OK before restarting production ";
                        }
                        LogInDB.Info(msg.translation);

                        DAL.me.InsertMesInterfaceLog("A060", root.Header.SessionID,
                            DateTime.ParseExact(root.Header.RequestTime, "yyyy-MM-dd HH:mm:ss fff", System.Globalization.CultureInfo.CurrentCulture).ToString("yyyy-MM-dd HH:mm:ss.fff"),
                            null, recvdData.jason, "");
                        Thread.Sleep(2000);
                        foreach (var v in root.RequestInfo)
                        {
                            if (v.ParamType == "InputOutput")
                            {
                                List<DeviceInputOutputConfigInfo> lstDeviceInputOutputConfigInfos = new List<DeviceInputOutputConfigInfo>();
                                foreach (var d in v.Param)
                                {
                                    DeviceInputOutputConfigInfo info = new DeviceInputOutputConfigInfo();
                                    info.EquipmentID = d.EquipmentID;
                                    info.ParamName = d.ParamName;
                                    info.UploadParamID = d.UploadParamID;
                                    info.Type = d.Type;
                                    info.SettingValueAddr = d.SettingValueAddr;
                                    info.UpperLimitValueAddr = d.UpperLimitValueAddr;
                                    info.LowerLimitValueAddr = d.LowerLimitValueAddr;
                                    info.LimitControl = d.LimitControl;
                                    info.InputChangeMonitorAddr = d.InputChangeMonitorAddr;
                                    info.ActualValueAddr = d.ActualValueAddr;
                                    info.BycellOutputAddr = d.BycellOutputAddr;
                                    info.ParamValueRatio = d.ParamValueRatio;

                                    lstDeviceInputOutputConfigInfos.Add(info);
                                }
                                DAL.me.UpdateDeviceInputOutputConfigTable(lstDeviceInputOutputConfigInfos);
                            }
                            else if (v.ParamType == "Alert")
                            {
                                List<DeviceAlertConfigInfo> lstDeviceAlertConfigInfos = new List<DeviceAlertConfigInfo>();
                                foreach (var d in v.Param)
                                {
                                    DeviceAlertConfigInfo info = new DeviceAlertConfigInfo();
                                    info.AlarmLevel = d.AlertLevel;
                                    info.AlarmBitAddr = d.AlertBitAddr;
                                    info.EquipmentID = d.EquipmentID;
                                    info.AlarmContent = d.ParamName;
                                    info.UploadParamID = d.UploadParamID;

                                    lstDeviceAlertConfigInfos.Add(info);
                                }
                                DAL.me.UpdateAlarmRuleTable(lstDeviceAlertConfigInfos);
                            }
                            else if (v.ParamType == "Spart")
                            {
                                List<DeviceSpartConfigInfo> lstDeviceSpartConfigInfos = new List<DeviceSpartConfigInfo>();
                                foreach (var d in v.Param)
                                {
                                    DeviceSpartConfigInfo info = new DeviceSpartConfigInfo();
                                    info.EquipmentID = d.EquipmentID;
                                    info.UploadParamID = d.UploadParamID;
                                    info.ParamName = d.ParamName;
                                    info.Type = d.Type;
                                    info.MaxSettingValueAddr = d.SettingValueAddr;
                                    info.ActualValueAddr = d.ActualValueAddr;
                                    info.ParamValueRatio = d.ParamValueRatio;
                                    info.SpartExpectedLifetime = DeviceSpartConfigInfo.lstDeviceSpartConfigInfos.Where(x => x.EquipmentID == d.EquipmentID && x.UploadParamID == d.UploadParamID).FirstOrDefault() != null ? DeviceSpartConfigInfo.lstDeviceSpartConfigInfos.Where(x => x.EquipmentID == d.EquipmentID && x.UploadParamID == d.UploadParamID).FirstOrDefault().SpartExpectedLifetime : 0;
                                    lstDeviceSpartConfigInfos.Add(info);
                                }
                                DAL.me.UpdateDeviceSpartConfigTable(lstDeviceSpartConfigInfos);
                            }
                        }
                    }
                    else
                    {
                        int indexGuidBegin = recvdData.jason.IndexOf("SessionID") + 12;
                        string s = recvdData.jason.Substring(indexGuidBegin);
                        int indexGuidEnd = s.IndexOf("\"");
                        string guid = s.Substring(0, indexGuidEnd);
                        string FunctionID = recvdData.cmd;
                        DAL.me.InsertMesInterfaceLog("A060", guid, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), recvdData.jason, "Jason转对象失败");
                    }
                    #endregion
                }
                else if (recvdData.jason.Contains(@"""FunctionID"":") && !recvdData.cmd.Contains("A006"))
                {
                    #region else
                    RecordErrorInfo(recvdData.jason);
                    int indexGuidBegin = recvdData.jason.IndexOf("SessionID") + 12;
                    string s = recvdData.jason.Substring(indexGuidBegin);
                    int indexGuidEnd = s.IndexOf("\"");
                    string guid = s.Substring(0, indexGuidEnd);
                    string FunctionID = recvdData.cmd;
                    DAL.me.InsertMesInterfaceLog(FunctionID, guid, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), recvdData.jason, "");
                    #endregion
                }

                //ClearExpiredMsg();
                if (TestGuid != string.Empty)
                {
                    TestMes.Root root = JsonNewtonsoft.FromJSON<TestMes.Root>(recvdData.jason);
                    if (root.Header.SessionID == TestGuid)
                        Test_Buffer = recvdData;
                }
            }
            catch (Exception ex)
            {
                LogInDB.Error(ex.ToString());
            }
        }
        
        private void newThreadA048(object obj)
        {
            A047.Root root = (A047.Root)obj;
            A048(root);
            A047EventArgs arg = new A047EventArgs();
            arg.root = root;
            if (A047event != null)
            {
                A047event(this, arg);
            }
        }

        private void newThreadA054(object obj)
        {
            A053.Root root = (A053.Root)obj;
            A054(root);
            A053EventArgs arg = new A053EventArgs();
            arg.root = root;
            if (A053event != null)
            {
                A053event(this, arg);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dstIp"></param>
        /// <param name="srcIp"></param>
        /// <param name="sendTime"></param>
        /// <param name="cmd"></param>
        /// <param name="data"></param>
        private void RecvData(string dstIp, string srcIp, string sendTime, string cmd, byte[] data)
        {
            RecvdData recvdData = new RecvdData()
            {
                cmd = cmd,
                dstIp = dstIp,
                sendTime = DateTime.Now.ToString("yyyy-MM-dd") + " " + sendTime,
                srcIp = srcIp,
                jason = Encoding.UTF8.GetString(data)
            };
            Thread t = new Thread(new ParameterizedThreadStart(funcName));
            t.Start(recvdData);
        }
        public class A007EventArgs : EventArgs
        {
            public A007.Root root { get; set; }
        }
        public static event EventHandler<A007EventArgs> A007event;
        private void AnalyseA007Jason(RecvdData recvdData)
        {
            A007.Root root = JsonNewtonsoft.FromJSON<A007.Root>(recvdData.jason);
            if (root != null && root.RequestInfo != null)
            {
                MESmsg msg = new MESmsg();
                msg.FunctionID = root.Header.FunctionID;
                //msg.translation = $"{DateTime.Now.ToString("MM-dd HH:mm:ss")} MES推送给设备ID{root.Header.EQCode}如下{msg.FunctionID}接口的信息:";
                if (ATL.Common.StringResources.IsDefaultLanguage)
                {
                    msg.translation = $"{DateTime.Now.ToString("MM-dd HH:mm:ss")} MES推送给设备ID{root.Header.EQCode}如下{msg.FunctionID}接口的信息:";
                }
                else
                {
                    msg.translation = $"{DateTime.Now.ToString("MM-dd HH:mm:ss")} MES push to device ID{ root.Header.EQCode }as follows:{ msg.FunctionID }interface information :";
                }

                DAL.me.InsertMesInterfaceLog("A007", root.Header.SessionID,
                    DateTime.ParseExact(root.Header.RequestTime, "yyyy-MM-dd HH:mm:ss fff", System.Globalization.CultureInfo.CurrentCulture).ToString("yyyy-MM-dd HH:mm:ss.fff"),
                    null, recvdData.jason, "");
                ATL.Core.Core.A007Jason = recvdData.jason;
                ATL.Station.Station.Current.AddMESmsg(recvdData.jason);
                Thread t = new Thread(new ParameterizedThreadStart(A008));
                t.Start(root);
                DeivceEquipmentidPlcInfo deivceEquipmentidPlcInfo = DeivceEquipmentidPlcInfo.lstDeivceEquipmentidPlcInfos.Where(x => x.EquipmentID == root.Header.EQCode).FirstOrDefault();
                //判断 下发input还是易损件(by controlcode)
                if (deivceEquipmentidPlcInfo != null)
                {

                    PlcDAL.ThisObject.Update_deivce_equipmentid_plc_A007JASON(root.Header.EQCode, recvdData.jason);

                    deivceEquipmentidPlcInfo.A007Jason = recvdData.jason;
                    if (root.RequestInfo.CmdInfo.ControlCode == string.Empty)
                    {
                        #region 易损件
                        if (root.RequestInfo.SpartInfo != null && root.RequestInfo.SpartInfo.Count > 0)
                        {
                            List<A007.SpartInfoItem> lstSpartInfoItem = root.RequestInfo.SpartInfo;
                            foreach (var v in root.RequestInfo.SpartInfo)
                            {
                                if (v.ChangeFlag.ToUpper() == "TRUE")
                                {
                                    DeviceSpartConfigInfo deviceSpartConfigInfo = DeviceSpartConfigInfo.lstDeviceSpartConfigInfos.Where(x => x.EquipmentID == root.Header.EQCode
                                && x.UploadParamID == v.SpartID).FirstOrDefault();
                                    if (deviceSpartConfigInfo != null)
                                    {
                                        float f, usedlife;
                                        if (float.TryParse(v.SpartExpectedLifetime, out f) && float.TryParse(v.UsedLife, out usedlife))
                                        {
                                            lock (DeviceSpartConfigInfo.LockObj)
                                            {
                                                deviceSpartConfigInfo.SpartExpectedLifetime = (int)f;
                                                deviceSpartConfigInfo.MesSettingUsedLife = usedlife;
                                            }

                                            UserVariableValueService Service = new UserVariableValueService();
                                            deviceSpartConfigInfo.UsedLife = Service.GetDeviceSpartUsedLife(deviceSpartConfigInfo);
                                            PlcDAL.ThisObject.InsertQuickwearparthistoryinfo(deviceSpartConfigInfo, UserInfo.UserName);
                                            PlcDAL.ThisObject.Update_device_spart_config(deviceSpartConfigInfo, UserInfo.UserName);
                                        }
                                        else
                                        {
                                            //LogInDB.Error($"MES下发的A007接口易损件寿命{v.ToJSON()}不是数字，解析失败");
                                            if (ATL.Common.StringResources.IsDefaultLanguage)
                                            {
                                                LogInDB.Error($"MES下发的A007接口易损件寿命{v.ToJSON()}不是数字，解析失败");
                                            }
                                            else
                                            {
                                                LogInDB.Error($"Life {v.ToJSON()} of vulnerable parts of A007 interface issued by MES is not digital, and analysis failed ");
                                            }
                                        }
                                    }
                                    else
                                    {
                                        //LogInDB.Error(v.ToJSON() + "  解析和下发失败");
                                        if (ATL.Common.StringResources.IsDefaultLanguage)
                                        {
                                            LogInDB.Error(v.ToJSON() + "  解析和下发失败");
                                        }
                                        else
                                        {
                                            LogInDB.Error(v.ToJSON() + "  Parsing and distribution failed ");
                                        }
                                    }
                                    //msg.translation += Environment.NewLine + $"当前需要更换的易损件ID:{v.SpartID},易损件名称:{v.SpartName},预期寿命:{v.SpartExpectedLifetime}";
                                    if (ATL.Common.StringResources.IsDefaultLanguage)
                                    {
                                        msg.translation += Environment.NewLine + $"当前需要更换的易损件ID:{v.SpartID},易损件名称:{v.SpartName},预期寿命:{v.SpartExpectedLifetime}";
                                    }
                                    else
                                    {
                                        msg.translation += Environment.NewLine + $"Vulnerable parts ID to be replaced at present :{v.SpartID},Vulnerable parts name:{v.SpartName},Vulnerable parts expected life:{v.SpartExpectedLifetime}";
                                    }
                                }
                                else
                                {
                                    //msg.translation += Environment.NewLine + $"当前正在使用的易损件ID:{v.SpartID},易损件名称:{v.SpartName},预期寿命:{v.SpartExpectedLifetime}";
                                    if (ATL.Common.StringResources.IsDefaultLanguage)
                                    {
                                        msg.translation += Environment.NewLine + $"当前正在使用的易损件ID:{v.SpartID},易损件名称:{v.SpartName},预期寿命:{v.SpartExpectedLifetime}";
                                    }
                                    else
                                    {
                                        msg.translation += Environment.NewLine + $"Vulnerable parts ID in use currently:{v.SpartID},Vulnerable parts name:{v.SpartName},Vulnerable parts expected life:{v.SpartExpectedLifetime}";
                                    }
                                }
                            }
                        }
                        else
                            //LogInDB.Error("MES下发的A007接口中，controlcode为字符串的空，为下发易损件模式，但是未包含易损件信息" + recvdData.jason);
                            if (ATL.Common.StringResources.IsDefaultLanguage)
                            {
                                LogInDB.Error("MES下发的A007接口中，controlcode为字符串的空，为下发易损件模式，但是未包含易损件信息" + recvdData.jason);
                            }
                            else
                            {
                                LogInDB.Error("In A007 interface issued by MES, controlcode is empty of string, which is mode of distributing vulnerable parts, but it does not contain information of vulnerable parts " + recvdData.jason);
                            }
                        #endregion 易损件
                    }
                    else
                    {
                        #region Input下发

                        if (root.RequestInfo.ParameterInfo != null && root.RequestInfo.ParameterInfo.Count > 0)
                        {
                            deivceEquipmentidPlcInfo.RecvA007 = true;
                            A007EventArgs arg = new A007EventArgs();
                            arg.root = root;
                            if (A007event != null)
                            {
                                A007event(this, arg);
                            }
                            foreach (var v in root.RequestInfo.ParameterInfo)
                            {
                                //msg.translation += Environment.NewLine + $"MES下发参数ID:{v.ParamID},参数名称:{v.Description},上限:{v.UpperLimitValue},下限:{v.LowerLimitValue},标准值:{v.StandardValue}";
                                if (ATL.Common.StringResources.IsDefaultLanguage)
                                {
                                    msg.translation += Environment.NewLine + $"MES下发参数ID:{v.ParamID},参数名称:{v.Description},上限:{v.UpperLimitValue},下限:{v.LowerLimitValue},标准值:{v.StandardValue}";
                                }
                                else
                                {
                                    msg.translation += Environment.NewLine + $"MES issued parameter ID :{v.ParamID},Parameter name :{v.Description},upper limit :{v.UpperLimitValue},lower limit :{v.LowerLimitValue},Standard value :{v.StandardValue}";
                                }
                            }
                            lock (DeivceEquipmentidPlcInfo.LockObj)
                            {
                                deivceEquipmentidPlcInfo.ControlCode = "STOP";
                                deivceEquipmentidPlcInfo.ProductMode = "1";
                                deivceEquipmentidPlcInfo.StateCode = "Param not ready";
                                deivceEquipmentidPlcInfo.StateDesc = "machine did not received para";
                            }
                            deivceEquipmentidPlcInfo.LastMesUpdateCountDateTime = DateTime.Now;
                            //写入控机给PLC
                            UserVariableValueService Service = new UserVariableValueService();
                            Service.WriteStatusToPLC();
                            //msg.translation += Environment.NewLine + $"指令:{deivceEquipmentidPlcInfo.ControlCode}, 状态:{deivceEquipmentidPlcInfo.StateCode},描述:{deivceEquipmentidPlcInfo.StateDesc}";
                            if (ATL.Common.StringResources.IsDefaultLanguage)
                            {
                                msg.translation += Environment.NewLine + $"指令:{deivceEquipmentidPlcInfo.ControlCode}, 状态:{deivceEquipmentidPlcInfo.StateCode},描述:{deivceEquipmentidPlcInfo.StateDesc}";
                            }
                            else
                            {
                                msg.translation += Environment.NewLine + $"instructions:{deivceEquipmentidPlcInfo.ControlCode}, state:{deivceEquipmentidPlcInfo.StateCode},describe:{deivceEquipmentidPlcInfo.StateDesc}";
                            }
                        }
                        else
                        {
                            lock (DeivceEquipmentidPlcInfo.LockObj)
                            {
                                deivceEquipmentidPlcInfo.ControlCode = root.RequestInfo.CmdInfo.ControlCode.ToUpper();
                                deivceEquipmentidPlcInfo.StateCode = root.RequestInfo.CmdInfo.StateCode;
                                deivceEquipmentidPlcInfo.StateDesc = root.RequestInfo.CmdInfo.StateDesc;
                            }
                            UserVariableValueService Service = new UserVariableValueService();
                            Service.WriteStatusToPLC();
                            //msg.translation += Environment.NewLine + $"指令:{deivceEquipmentidPlcInfo.ControlCode}, 状态:{deivceEquipmentidPlcInfo.StateCode},描述:{deivceEquipmentidPlcInfo.StateDesc}";
                            if (ATL.Common.StringResources.IsDefaultLanguage)
                            {
                                msg.translation += Environment.NewLine + $"指令:{deivceEquipmentidPlcInfo.ControlCode}, 状态:{deivceEquipmentidPlcInfo.StateCode},描述:{deivceEquipmentidPlcInfo.StateDesc}";
                            }
                            else
                            {
                                msg.translation += Environment.NewLine + $"instructions:{deivceEquipmentidPlcInfo.ControlCode}, state:{deivceEquipmentidPlcInfo.StateCode},describe:{deivceEquipmentidPlcInfo.StateDesc}";
                            }
                        }
                        #endregion Input下发
                    }
                }
                else
                    //LogInDB.Error($"配置错误：MES下发的A007接口中的设备ID与配置的设备ID不同");
                    if (ATL.Common.StringResources.IsDefaultLanguage)
                {
                    LogInDB.Error($"配置错误：MES下发的A007接口中的设备ID与配置的设备ID不同");
                }
                else
                {
                    LogInDB.Error($"Configuration error: the device ID in A007 interface issued by MES is different from the configured device ID ");
                }

                #region Count
                if (deivceEquipmentidPlcInfo != null)
                {
                    if (root.RequestInfo.CmdInfo.ControlCode.ToUpper() == "RUN")
                    {
                        if (!string.IsNullOrEmpty(root.RequestInfo.Count))
                        {
                            float count;
                            if (float.TryParse(root.RequestInfo.Count, out count))
                            {
                                lock (DeivceEquipmentidPlcInfo.LockObj)
                                {
                                    deivceEquipmentidPlcInfo.Count = (int)count;
                                    deivceEquipmentidPlcInfo.ProductMode = "1";
                                }
                                deivceEquipmentidPlcInfo.LastMesUpdateCountDateTime = DateTime.Now;
                                //LogInDB.Info($"MES通过A007接口发送首件数量：{count},当前模式为首件模式");
                                if (ATL.Common.StringResources.IsDefaultLanguage)
                                {
                                    LogInDB.Info($"MES通过A007接口发送首件数量：{count},当前模式为首件模式");
                                }
                                else
                                {
                                    LogInDB.Info($"MES sends the first article quantity {count} through A007 interface, the current mode is the first article mode ");
                                }
                            }
                            else
                                 //LogInDB.Error("MES下发的controlcode为RUN，但是首件数量格式不正确——"+ root.RequestInfo.Count);
                                 if (ATL.Common.StringResources.IsDefaultLanguage)
                            {
                                LogInDB.Error("MES下发的controlcode为RUN，但是首件数量格式不正确——" + root.RequestInfo.Count);
                            }
                            else
                            {
                                LogInDB.Error("The control code issued by MES is RUN, but the format of the first piece quantity is incorrect ——" + root.RequestInfo.Count);
                            }
                        }
                        else
                        {
                            //LogInDB.Error("MES下发的controlcode为RUN，但是首件数量为空");
                            if (ATL.Common.StringResources.IsDefaultLanguage)
                            {
                                LogInDB.Error("MES下发的controlcode为RUN，但是首件数量为空");
                            }
                            else
                            {
                                LogInDB.Error("The control code issued by MES is RUN, but the quantity of the first piece is empty ");
                            }
                        }
                    }
                }
                #endregion

                A047RequestInfo a047RequestInfo = null;

                #region UserInfo
                if (root.RequestInfo.UserInfo != null)  //
                {
                    UserInfo.UserID = root.RequestInfo.UserInfo.UserID;
                    UserInfo.UserLevel = root.RequestInfo.UserInfo.UserLevel;
                    UserInfo.UserName = root.RequestInfo.UserInfo.UserName;
                    lock (LocklstA047RequestFromMESObj)
                    {
                        a047RequestInfo = lstA047RequestFromMES.Where(x => x.EquipmentID == root.Header.EQCode).FirstOrDefault();
                    }
                    if (a047RequestInfo == null)
                    {
                        a047RequestInfo = new A047RequestInfo();
                        a047RequestInfo.EquipmentID = root.Header.EQCode;
                        a047RequestInfo.UserInfo = JsonNewtonsoft.FromJSON<A047UserInfo>(root.RequestInfo.UserInfo.ToJSON());
                        lock (LocklstA047RequestFromMESObj)
                        {
                            lstA047RequestFromMES.Add(a047RequestInfo);
                        }
                    }
                    else
                    {
                        a047RequestInfo.UserInfo = JsonNewtonsoft.FromJSON<A047UserInfo>(root.RequestInfo.UserInfo.ToJSON());
                    }
                    //msg.translation += Environment.NewLine + $"用户ID:{root.RequestInfo.UserInfo.UserID},用户等级:{root.RequestInfo.UserInfo.UserLevel}";
                    if (ATL.Common.StringResources.IsDefaultLanguage)
                    {
                        msg.translation += Environment.NewLine + $"用户ID:{root.RequestInfo.UserInfo.UserID},用户等级:{root.RequestInfo.UserInfo.UserLevel}";
                    }
                    else
                    {
                        msg.translation += Environment.NewLine + $"User ID:{root.RequestInfo.UserInfo.UserID},User level:{root.RequestInfo.UserInfo.UserLevel}";
                    }
                }
                #endregion

                #region ModelInfo
                if (deivceEquipmentidPlcInfo != null)
                {
                    lock (DeivceEquipmentidPlcInfo.LockObj)
                    {
                        deivceEquipmentidPlcInfo.ModelInfo = root.RequestInfo.ModelInfo;
                        deivceEquipmentidPlcInfo.NeedDownloadModel = true;
                    }
                    if (a047RequestInfo == null)
                    {
                        a047RequestInfo = new A047RequestInfo();
                        a047RequestInfo.EquipmentID = root.Header.EQCode;
                        a047RequestInfo.ModelInfo = root.RequestInfo.ModelInfo;
                        lock (LocklstA047RequestFromMESObj)
                        {
                            lstA047RequestFromMES.Add(a047RequestInfo);
                        }
                    }
                    else
                    {
                        a047RequestInfo.ModelInfo = root.RequestInfo.ModelInfo;
                    }
                }
                lock (LocklstA047RequestFromMESObj)
                {
                    try
                    {
                        WRSerializable.SerializeToFile(lstA047RequestFromMES, System.IO.Directory.GetCurrentDirectory() + "\\lstA047RequestFromMES.dat");
                    }
                    catch(Exception ex)
                    {
                        //LogInDB.Error($"序列化[lstA047RequestFromMES]failure：{ex.ToString()}");
                        if (ATL.Common.StringResources.IsDefaultLanguage)
                        {
                            LogInDB.Error($"序列化[lstA047RequestFromMES]failure：{ex.ToString()}");
                        }
                        else
                        {
                            LogInDB.Error($"Failed to serialize [lstA047RequestFromMES] ：{ex.ToString()}");
                        }
                    }
                }
                //msg.translation += Environment.NewLine + $"当前生产型号:{root.RequestInfo.ModelInfo}";
                if (ATL.Common.StringResources.IsDefaultLanguage)
                {
                    msg.translation += Environment.NewLine + $"当前生产型号:{root.RequestInfo.ModelInfo}";
                }
                else
                {
                    msg.translation += Environment.NewLine + $"Current production model :{root.RequestInfo.ModelInfo}";
                }
                #endregion ModelInfo

                #region 班次
                if (root.RequestInfo.ResourceInfo != null)
                {
                    if (deivceEquipmentidPlcInfo != null)
                        lock (DeivceEquipmentidPlcInfo.LockObj)
                        {
                            deivceEquipmentidPlcInfo.ResourceShift = root.RequestInfo.ResourceInfo.ResourceShift;
                        }
                    //msg.translation += Environment.NewLine + $"当前资源编号:{root.RequestInfo.ResourceInfo.ResourceID},当前班次:{root.RequestInfo.ResourceInfo.ResourceShift}";
                    if (ATL.Common.StringResources.IsDefaultLanguage)
                    {
                        msg.translation += Environment.NewLine + $"当前资源编号:{root.RequestInfo.ResourceInfo.ResourceID},当前班次:{root.RequestInfo.ResourceInfo.ResourceShift}";
                    }
                    else
                    {
                        msg.translation += Environment.NewLine + $"Current resource number :{root.RequestInfo.ResourceInfo.ResourceID},Current shift :{root.RequestInfo.ResourceInfo.ResourceShift}";
                    }
                }
                #endregion

                LogInDB.Info(msg.translation);
            }
            else
            {
                int indexGuidBegin = recvdData.jason.IndexOf("SessionID") + 12;
                string s = recvdData.jason.Substring(indexGuidBegin);
                int indexGuidEnd = s.IndexOf("\"");
                string guid = s.Substring(0, indexGuidEnd);
                string FunctionID = recvdData.cmd;
                DAL.me.InsertMesInterfaceLog("A007", guid, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), recvdData.jason, "Jason转对象失败");
            }
        }

        //private void DownloadLimitOnly(A007.Root root)
        //{
        //    bool WriteSuccess = true;
        //    #region 先对所有的LimitControl以Int16数据类型写入数字10
        //    DeviceControlCodeInfo deviceControlCodeInfo = DeviceControlCodeInfo.lstDeviceControlCodeInfos.Where(x => x.ControlCode == "SingleInputLimitControl" && x.StateCode == "Disable").FirstOrDefault();
        //    int DisableSingleInputLimitControl = 10;
        //    if (deviceControlCodeInfo != null)
        //    {
        //        DisableSingleInputLimitControl = deviceControlCodeInfo.plcValue;
        //    }
        //    deviceControlCodeInfo = DeviceControlCodeInfo.lstDeviceControlCodeInfos.Where(x => x.ControlCode == "SingleInputLimitControl" && x.StateCode == "Enable").FirstOrDefault();
        //    int EnableSingleInputLimitControl = 0;
        //    if (deviceControlCodeInfo != null)
        //    {
        //        EnableSingleInputLimitControl = deviceControlCodeInfo.plcValue;
        //    }
        //    foreach (var v in DeviceInputOutputConfigInfo.lstDeviceInputOutputConfigInfos.Where(x => x.EquipmentID == root.Header.EQCode && !string.IsNullOrEmpty(x.LimitControl)).ToList())
        //    {
        //        UserVariableValueService Service = new UserVariableValueService();
        //        WriteSuccess = Service.WriteLimitControl(v, DisableSingleInputLimitControl);
        //    }
        //    #endregion 先对所有的LimitControl以Int16数据类型写入数字10
        //    foreach (var msg in root.RequestInfo.ParameterInfo)
        //    {
        //        var deviceInputOutputConfigs = DeviceInputOutputConfigInfo.lstDeviceInputOutputConfigInfos.Where(x => x.SendParamID == msg.ParamID && x.EquipmentID == root.Header.EQCode).ToList();
        //        if (deviceInputOutputConfigs == null || deviceInputOutputConfigs.Count == 0)
        //        {
        //            continue;
        //        }
        //        foreach (var deviceInputOutputConfig in deviceInputOutputConfigs)
        //        {
        //            if (deviceInputOutputConfig.InputVariableTypeID == 2)
        //            {
        //                ID_Device device = ID_Device.lstDevices.Where(x => x.id == deviceInputOutputConfig.deivceEquipmentidPlcInfo.PLCid.Value).FirstOrDefault();
        //                if (device != null)
        //                {
        //                    if (!device.WriteByAddress(deviceInputOutputConfig.LowerLimitValueAddr, deviceInputOutputConfig.Type, (float.Parse(deviceInputOutputConfig.PreparedLowerLimitValueFromMES) / float.Parse(deviceInputOutputConfig.ParamValueRatio)).ToString())
        //                            || !device.WriteByAddress(v.UpperLimitValueAddr, v.Type, (float.Parse(v.PreparedUpperLimitValueFromMES) / float.Parse(v.ParamValueRatio)).ToString())
        //                            || !device.WriteByAddress(v.SettingValueAddr, v.Type, (float.Parse(v.PreparedStandardValueFromMES) / float.Parse(v.ParamValueRatio)).ToString())
        //                            )
        //                    {
        //                        string msg = "Input参数写入失败";
        //                        MessageBox.Show(msg);
        //                        LogInDB.Error(msg);
        //                        WriteSuccess = false;
        //                        break;
        //                    }
        //                    else
        //                    {
        //                        if (!device.WriteByAddress(v.LimitControl, "Int16", EnableSingleInputLimitControl))
        //                        {
        //                            string msg = "成功下载Input参数后，将Input启用控制参数写入PLC失败";
        //                            MessageBox.Show(msg);
        //                            LogInDB.Error(msg);
        //                            WriteSuccess = false;
        //                            break;
        //                        }
        //                    }
        //                }
        //                else
        //                {
        //                    LogInDB.Error($"代码bug:2020.02.29.14.31");
        //                }
        //            }
        //        }
        //    }
        //}

        //private static DateTime lastClearReceivedJason = DateTime.Now;
        ///// <summary>
        ///// 清除过期消息
        ///// </summary>
        //private static void ClearExpiredMsg()
        //{
        //    if((DateTime.Now - lastClearReceivedJason).TotalSeconds > 60)
        //    {
        //        lastClearReceivedJason = DateTime.Now;
        //        lock (LockLstReceivedJasonObj)
        //        {
        //            if (lstReceivedJason.Count() > 100)
        //            {
        //                if ((DateTime.Now - DateTime.ParseExact(lstReceivedJason[0].receivedTime, "yyyy-MM-dd HH:mm:ss fff", CultureInfo.InvariantCulture)).TotalMilliseconds > ReceivedMsgUnhandledTimeOut)
        //                {
        //                    lstReceivedJason.RemoveAt(0);
        //                }
        //            }
        //        }
        //    }
        //}
        
        /// <summary>
        /// 记录错误信息
        /// </summary>
        private void RecordErrorInfo(string jason)
        {
            string functionid= SubstringSingelLine(jason, "FunctionID");
            string errorcode = SubstringSingelLine(jason, "ErrorCode");
            string errormsg= SubstringSingelLine(jason, "ErrorMsg");
           if(!string.IsNullOrEmpty(errorcode) || (!string.IsNullOrEmpty(errormsg) && errormsg.ToUpper() != "NULL"))
            {
                MESmsg msg = new MESmsg();
                msg.FunctionID = functionid;
                msg.translation = $"{errorcode}   {errormsg}";
                //LogInDB.Info("MES返回的errmsg："+functionid + "---- " + msg.translation);
                if (ATL.Common.StringResources.IsDefaultLanguage)
                {
                    LogInDB.Info("MES返回的errmsg：" + functionid + "---- " + msg.translation);
                }
                else
                {
                    LogInDB.Info("MES returned the errmsg  ：" + functionid + "---- " + msg.translation);
                }
                msg.translation += DateTime.Now.ToString("MM-dd HH:mm:ss");
                if(!string.IsNullOrEmpty(errormsg))
                    LogHelper.Error(jason);
                MESmsg.Add(msg);
            }
        }

        private void RecordConnErrorInfo(string connMsg)
        {
            MESmsg msg = new MESmsg();
            msg.FunctionID = "";
            msg.translation = connMsg;
            msg.translation += DateTime.Now.ToString("MM-dd HH:mm:ss");
            MESmsg.Add(msg);
        }


        /// <summary>
        /// 截取字符串中开始和结束字符串中间的字符串
        /// </summary>
        /// <param name="source">源字符串</param>
        /// <param name="startStr">开始字符串</param>
        /// <param name="endStr">结束字符串</param>
        /// <returns>中间字符串</returns>
        private string SubstringSingelLine(string source, string startStr, string endStr = "\",")
        {
            Regex rg = new Regex("(?<=(\"" + startStr + "\":\"))[.\\s\\S]*?(?=(" + endStr + "))", RegexOptions.Singleline);
            Match match = rg.Match(source);
            string res = match.Value;
            return res;
        }

        private static object ClientObj = new object();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmd">比如A001</param>
        /// <param name="jason">要发送的JASON数据</param>
        /// <returns></returns>
        private static bool Send(string cmd, string jason, out string errorMSG)
        {
            errorMSG = string.Empty;
            byte[] data = Encoding.UTF8.GetBytes(jason);
            try
            {
                bool res = true;
                if (Client.SendData(cmd, data, 500) != 0)
                {
                    errorMSG = "Send Error!";
                    res = false;
                }
                return res;
            }
            catch (ArgumentNullException)
            {
                errorMSG = "Data Error!";
                return false;
            }
            catch (ArgumentOutOfRangeException)
            {
                errorMSG = "Command Error!";
                return false;
            }
            catch (Exception ex)
            {
                errorMSG = ex.Message;
                return false;
            }
        }

        /// <summary>字符串高低位交换</summary>
        public string UpLowReverse(string origin)
        {
            string result = "";
            for (int i = 0; i < origin.Length / 2; i++)
            {
                string val = origin.Substring(i * 2, 2);
                result += "" + val[1] + val[0];
            }
            if (origin.Length % 2 != 0)
            {
                result += origin.Substring(origin.Length - 1, 1);
            }
            return result;
        }
        
        #region 接口

        private void MesErrorStopPLC(string EQCode)
        {
            //LogInDB.Info("MES返回信息超时");
            if (ATL.Common.StringResources.IsDefaultLanguage)
            {
                LogInDB.Info("给PLC：MES返回信息超时");
            }
            else
            {
                LogInDB.Info("To PLC: MES return information timeout");
            }
            /////////////////超时，写入PLC，停机
            DeivceEquipmentidPlcInfo deivceEquipmentidPlcInfo = DeivceEquipmentidPlcInfo.lstDeivceEquipmentidPlcInfos.Where(x => x.EquipmentID == EQCode).FirstOrDefault();
            if (deivceEquipmentidPlcInfo != null)
            {
                lock (DeivceEquipmentidPlcInfo.LockObj)
                {
                    deivceEquipmentidPlcInfo.MesReplyOK = false;
                }
            }
            else if (deivceEquipmentidPlcInfo == null)
            {
                //LogInDB.Error($"配置错误, MES返回的root.Header.EQCode:{EQCode}有误,控机失败");
                if (ATL.Common.StringResources.IsDefaultLanguage)
                {
                    LogInDB.Error($"配置错误, MES返回的root.Header.EQCode:{EQCode}有误,控机失败");
                }
                else
                {
                    LogInDB.Error($"Configuration error, MES returned root.Header.EQCode :{EQCode} error, controller failed ");
                }
                return;
            }
        }

        string TestGuid = string.Empty;
        string TestFunctionID= string.Empty;
        public string InterfaceTest(string FunctionID, string Jason, out bool ret)
        {
            TestGuid = string.Empty;
            TestFunctionID = string.Empty;
            ret = false;
            if (Client == null || !MESconnected || HeartBeatError) return "MES连接失败";
            if (!Jason.Contains(@"""FunctionID""" + ":" + @"""" + FunctionID + @""""))
            {
                //string ss = $"Jason数据有误！不包含当前测试的FunctionID:{FunctionID}";
                string ss = string.Empty;
                if (ATL.Common.StringResources.IsDefaultLanguage)
                {
                    ss = $"Jason数据有误！不包含当前测试的FunctionID:{FunctionID}";
                }
                else
                {
                    ss = $"Jason's data is wrong! Does not contain the FunctionID:{FunctionID} of the current test ";
                }
                TestGuid = string.Empty;
                return ss;
            }
            string error, guid;
            try
            {
                int indexGuidBegin = Jason.IndexOf("SessionID") + 12;
                string s = Jason.Substring(indexGuidBegin);
                int indexGuidEnd = s.IndexOf("\"");

                if (indexGuidEnd < 32)
                {
                    //string ss = $"GUID长度计算错误，请核对JASON字符串格式";
                    string ss = string.Empty;
                    if (ATL.Common.StringResources.IsDefaultLanguage)
                    {
                        ss = $"GUID长度计算错误，请核对JASON字符串格式";
                    }
                    else
                    {
                        ss = $"Error in calculating the length of the GUID, please check the JASON string format ";
                    }
                    TestGuid = string.Empty;
                    return ss;
                }
                if (indexGuidBegin < 0 || Jason.Length < indexGuidBegin)
                {
                    //string ss = $"Jason数据长度和格式有误！";
                    string ss = string.Empty;
                    if (ATL.Common.StringResources.IsDefaultLanguage)
                    {
                        ss = $"Jason数据长度和格式有误！";
                    }
                    else
                    {
                        ss = $"Jason data length and format error ！";
                    }
                    TestGuid = string.Empty;
                    return ss;
                }
                guid = s.Substring(0, indexGuidEnd);
                TestGuid = guid;
                TestFunctionID = FunctionID;
            }
            catch (Exception ex)
            {
                //string ss = $"Jason数据有误:{ex.ToString()}";
                string ss = string.Empty;
                if (ATL.Common.StringResources.IsDefaultLanguage)
                {
                    ss = $"Jason数据有误:{ex.ToString()}";
                }
                else
                {
                    ss = $"Jason's data is wrong:{ex.ToString()}";
                }
                TestGuid = string.Empty;
                return ss;
            }
            Test_Buffer = new RecvdData();
            DateTime start = DateTime.Now;
            if (Send(FunctionID, Jason, out error))
            {
                DAL.me.InsertMesInterfaceLog(FunctionID, TestGuid, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), null, "离线Jason: " + Jason, error);
                while (true)
                {
                    Thread.Sleep(100);
                    if ((DateTime.Now - start).TotalMilliseconds > ReceiveTimeOut)
                    {
                        //string ss = $"MES response timeout";
                        string ss = string.Empty;
                        if (ATL.Common.StringResources.IsDefaultLanguage)
                        {
                            ss = $"发送缓存数据失败，MES响应超时";
                        }
                        else
                        {
                            ss = $"Send Buffer failed: MES response timeout ";
                        }
                        TestGuid = string.Empty;
                        return ss;
                    }
                    if (Test_Buffer.jason != null)
                    {
                        string msg = Test_Buffer.jason;
                        TestGuid = string.Empty;
                        ret = true;
                        return msg;
                     }
                }
            }
            else
            {
                string msg = $"InterfaceTest Error:{error}";
                return msg;
            }
        }

        /// <summary>
        /// 设备上位机通过设备ID向MES请求设备代码
        /// </summary>
        /// <param name="error"></param>
        /// <returns>MES根据设备ID查询该设备是否注册，成功结果返回OK，否则结果返回NG</returns>
        public A002.Root A001(string EquipmentID = "")
        {
            if (Client == null || !MESconnected || HeartBeatError)
                return null;
            int tryTimes = 0;
            A001.Root root = new A001.Root();
            string error = string.Empty;
            root.Header = new MES.A001.Header();
            root.Header.EQCode = string.IsNullOrEmpty(EquipmentID) ? Station.Station.Current.EquipmentID : EquipmentID;
            root.Header.FunctionID = "A001";
            root.Header.PCName = Station.Station.Current.PCName;
            root.Header.RequestTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff");
            root.Header.SessionID = Guid.NewGuid().ToString();
            root.Header.SoftName = Station.Station.Current.SoftName;

            root.RequestInfo = new A001.RequestInfo();
            root.RequestInfo.EQCode = Station.Station.Current.EquipmentID;
            root.RequestInfo.PCName = Station.Station.Current.PCName;

            string jason = root.ToJSON();
            OfflineDataInfo offlineDataInfo = new OfflineDataInfo();
            offlineDataInfo.Guid = root.Header.SessionID;
            offlineDataInfo.Data = jason;
            offlineDataInfo.FunctionID = root.Header.FunctionID;
            A002_Buffer = new RecvdData();
            retry:
            tryTimes++;
            DateTime start = DateTime.Now;
            if (Send("A001", offlineDataInfo.Data, out error))
            {
                DAL.me.InsertMesInterfaceLog("A001", root.Header.SessionID,
                    DateTime.ParseExact(root.Header.RequestTime, "yyyy-MM-dd HH:mm:ss fff", System.Globalization.CultureInfo.CurrentCulture).ToString("yyyy-MM-dd HH:mm:ss.fff"),
                    null, jason, error);
                while (true)
                {
                    Thread.Sleep(100);
                    if ((DateTime.Now - start).TotalMilliseconds > ReceiveTimeOut)
                    {
                        if (!HeartBeatError && tryTimes < 3)
                            goto retry;
                        if (ATL.Common.StringResources.IsDefaultLanguage)
                        {
                            error = "MES响应A001超时";
                        }
                        else
                        {
                            error = "MES response A001 timeout";
                        }
                        LogInDB.Error(error);
                        DAL.me.UpdateMesInterfaceLogErrorMsg("A001", root.Header.SessionID, error);
                        return null;
                    }
                    try
                    {
                        if (A002_Buffer.jason != null&& A002_Buffer.jason.Contains(offlineDataInfo.Guid))
                        {
                            A002.Root _root = JsonNewtonsoft.FromJSON<A002.Root>(A002_Buffer.jason);
                            return _root;
                        }
                    }
                    catch (Exception ex)
                    {
                        if (ATL.Common.StringResources.IsDefaultLanguage)
                        {
                            error = "解析A002 Jason失败：" + ex.Message;
                        }
                        else
                        {
                            error = "Analysis Jaoson-A002 failure：" + ex.Message;
                        }
                        LogInDB.Error(error);
                        DAL.me.UpdateMesInterfaceLogErrorMsg("A002", root.Header.SessionID, error);
                        return null;
                    }
                }
            }
            else
            {
                LogInDB.Error(error);
                DAL.me.InsertMesInterfaceLog("A001", root.Header.SessionID,
                    DateTime.ParseExact(root.Header.RequestTime, "yyyy-MM-dd HH:mm:ss fff", System.Globalization.CultureInfo.CurrentCulture).ToString("yyyy-MM-dd HH:mm:ss.fff"),
                     null, jason, error);
                return null;
            }
        }

        /// <summary>
        /// 定时向服务端发送心跳包请求
        /// </summary>
        /// <param name="error"></param>
        /// <returns>响应客户端心跳包请求</returns>
        public A006.Root A005()
        {
            A005.Root root = new A005.Root();
            string error = string.Empty;

            root.Header = new MES.A005.Header();
            root.Header.EQCode = Station.Station.Current.EquipmentID;
            root.Header.FunctionID = "A005";
            root.Header.PCName = Station.Station.Current.PCName;
            root.Header.RequestTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff");
            root.Header.SessionID = Guid.NewGuid().ToString();
            root.Header.SoftName = Station.Station.Current.SoftName;

            root.RequestInfo = new A005.RequestInfo();

            string jason = root.ToJSON();
            OfflineDataInfo offlineDataInfo = new OfflineDataInfo();
            offlineDataInfo.Guid = root.Header.SessionID;
            offlineDataInfo.Data = jason;
            offlineDataInfo.FunctionID = root.Header.FunctionID;
            DateTime start = DateTime.Now;
            A006_Buffer = new RecvdData();
            if (Send("A005", offlineDataInfo.Data, out error))
            {
                while (true)
                {
                    Thread.Sleep(100);
                    if ((DateTime.Now - start).TotalMilliseconds > ReceiveTimeOut)
                    {
                        error = "MES response timeout";
                        LogInDB.Error(error);
                        string msg = string.Empty;
                        if (ATL.Common.StringResources.IsDefaultLanguage)
                        {
                            msg = $"发送心跳失败:{error}";
                        }
                        else
                        {
                            msg = $"Sending heartbeat failed :{error}";
                        }
                        LogInDB.Info(msg);
                        RecordConnErrorInfo(msg);
                        return null;
                    }
                    try
                    {
                        if (A006_Buffer.jason != null&&A006_Buffer.jason.Contains(offlineDataInfo.Guid))
                        {
                            A006.Root _root = JsonNewtonsoft.FromJSON<A006.Root>(A006_Buffer.jason);
                            //lstReceivedJason.Remove(A006_Buffer);
                            return _root;
                        }
                    }
                    catch (Exception ex)
                    {
                        error = "Analysis Jaoson-A005failure：" + ex.ToString();
                        string msg = string.Empty;
                        if (ATL.Common.StringResources.IsDefaultLanguage)
                        {
                            msg = $"发送心跳失败:{error}";
                        }
                        else
                        {
                            msg = $"Sending heartbeat failed :{error}";
                        }
                        LogInDB.Info(msg);
                        RecordConnErrorInfo(msg);
                        return null;
                    }
                }
            }
            else
            {
                string msg = string.Empty;
                if (ATL.Common.StringResources.IsDefaultLanguage)
                {
                    msg = $"发送心跳失败:{error}";
                }
                else
                {
                    msg = $"Sending heartbeat failed :{error}";
                }
                LogInDB.Info(msg);
                RecordConnErrorInfo(msg);
                return null;
            }
        }

        /// <summary>
        /// MES发送初始化开机指令,内容主要包括设备关键部件及设备运行标准参数；之后响应
        /// </summary>
        /// <param name="msg"></param>
        public void A008(object obj)
        {
            A007.Root msg = (A007.Root)obj;
            if (Client == null || !MESconnected || HeartBeatError) return;
            A008.Root root = new A008.Root();
            string error = string.Empty;

            root.Header = new A008.Header();
            root.Header.EQCode = msg.Header.EQCode;  //也是EquipmentID
            root.Header.FunctionID = "A008";
            root.Header.PCName = Station.Station.Current.PCName;
            root.Header.RequestTime = msg.Header.RequestTime;
            root.Header.SessionID = msg.Header.SessionID;
            root.Header.SoftName = Station.Station.Current.SoftName;
            root.Header.ErrorCode = "00";
            root.Header.ErrorMsg = "Null";
            root.Header.IsSuccess = "True";
            root.Header.ResponseTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff");
            root.ResponseInfo = new A008.ResponseInfo();
            root.ResponseInfo.Result = "OK";
            string jason = root.ToJSON();
            DAL.me.InsertMesInterfaceLog("A008", msg.Header.SessionID, null, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), jason, "");
            Send("A008", jason, out error);
        }

        /// <summary>
        /// 设备接收MES控制指令响应后响应
        /// </summary>
        /// <param name="msg"></param>
        public void A012(object state)
        {
            A011.Root msg = (A011.Root)state;
            if (Client == null || !MESconnected || HeartBeatError) return;
            A012.Root root = new A012.Root();
            string error = string.Empty;

            root.Header = new A012.Header();
            root.Header.EQCode = msg.Header.EQCode;
            root.Header.FunctionID = "A012";
            root.Header.PCName = Station.Station.Current.PCName;
            root.Header.RequestTime = msg.Header.RequestTime;
            root.Header.SessionID = msg.Header.SessionID;
            root.Header.SoftName = Station.Station.Current.SoftName;
            root.Header.ErrorCode = "00";
            root.Header.ErrorMsg = "Null";
            root.Header.IsSuccess = "True";
            root.Header.ResponseTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff");
            root.ResponseInfo = new A012.ResponseInfo();
            root.ResponseInfo.Result = "OK";
            string jason = root.ToJSON();
            DAL.me.InsertMesInterfaceLog("A012", msg.Header.SessionID, null, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), jason, "");
            Send("A012", jason, out error);
        }
        
        /// <summary>
        /// 发送设备生成的数据，内容主要包括原料、半成品或成品的工艺参数
        /// </summary>
        public A014.Root A013(string Type, List<A013._ProductsItem> lst, string EquipmentID = "")
        {
            DateTime startTime = DateTime.Now;
            int tryTimes = 0;
            A013.Root root = new A013.Root();
            string error = string.Empty;
            root.Header = new MES.A013.Header();
            root.Header.EQCode = string.IsNullOrEmpty(EquipmentID) ? Station.Station.Current.EquipmentID : EquipmentID;
            root.Header.FunctionID = "A013";
            root.Header.PCName = Station.Station.Current.PCName;
            root.Header.RequestTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff");
            root.Header.SessionID = Guid.NewGuid().ToString();
            root.Header.SoftName = Station.Station.Current.SoftName;

            root.RequestInfo = new A013.RequestInfo();
            root.RequestInfo.Type = Type;
            root.RequestInfo.Products = new List<MES.A013.ProductsItem>();
            List<string> lstBarcode = new List<string>();
            List<DeviceInputOutputConfigInfo> lstDeviceOutputConfig = DeviceInputOutputConfigInfo.lstDeviceInputOutputConfigInfos.Where(x => !string.IsNullOrEmpty(x.BycellOutputAddr)).ToList();
            foreach (var Item in lst)
            {
                A013.ProductsItem ProductsItem = new A013.ProductsItem();
                ProductsItem.ProductSN = Item.ProductSN;
                ProductsItem.Station = Item.Station;
                ProductsItem.Pass = Item.Pass;
                ProductsItem.ChildEquCode = Item.ChildEquCode;
                lstBarcode.Add(Item.ProductSN);
                ProductsItem.OutputParam = new List<A013.OutputParamItem>();
                foreach (var v in lstDeviceOutputConfig)
                {
                    int BycellOutputAddrLength = v.BycellOutputAddr.Split(',').Count();
                    if (BycellOutputAddrLength == lst.Count || BycellOutputAddrLength == 1)
                    {
                        foreach (var addr in v.BycellOutputAddr.Split(','))
                        {
                            A013.OutputParamItem OutputParamItem = new A013.OutputParamItem();
                            OutputParamItem.ParamID = v.UploadParamID;
                            OutputParamItem.SpecParamID = v.SendParamID;
                            OutputParamItem.ParamDesc = v.ParamName;

                            UserVariableValueService Service = new UserVariableValueService();
                            OutputParamItem.ParamValue = Service.GetBycellOutputValue(v, addr);
                            OutputParamItem.Result = "UN";
                            ProductsItem.OutputParam.Add(OutputParamItem);
                        }
                    }
                }
                root.RequestInfo.Products.Add(ProductsItem);
            }

            string jason = root.ToJSON();
            OfflineDataInfo offlineDataInfo = new OfflineDataInfo();
            offlineDataInfo.Guid = root.Header.SessionID;
            offlineDataInfo.Data = jason;
            offlineDataInfo.FunctionID = root.Header.FunctionID;
            if (!_allowWork())
            {
                if (ATL.Common.StringResources.IsDefaultLanguage)
                {
                    LogInDB.Error("PC和MES通信中断");
                }
                else
                {
                    LogInDB.Error("PC soft MES interaction terminated，probable cause：pc soft start and config check failed、MES communication error");
                }
                DAL.me.InsertToMESofflineBuffer(root.Header.EQCode, "A013", root.Header.SessionID, jason, lstBarcode);
                LogInDB.Info($"Excuted A013 spends {(int)(DateTime.Now - startTime).TotalMilliseconds} ms");
                return null;
            }
            //A014_Buffer = new RecvdData();
            retry:
            tryTimes++;
            DateTime start = DateTime.Now;
            if (Send("A013", offlineDataInfo.Data, out error))
            {
                DAL.me.InsertMesInterfaceLog("A013", root.Header.SessionID,
                    DateTime.ParseExact(root.Header.RequestTime, "yyyy-MM-dd HH:mm:ss fff", System.Globalization.CultureInfo.CurrentCulture).ToString("yyyy-MM-dd HH:mm:ss.fff"),
                     null, jason, error);
                while (true)
                {
                    Thread.Sleep(1);
                    if ((DateTime.Now - start).TotalMilliseconds > ReceiveTimeOut)
                    {
                        if (!HeartBeatError && tryTimes < 3)
                            goto retry;
                        if (ATL.Common.StringResources.IsDefaultLanguage)
                        {
                            error = "MES响应A013超时";
                        }
                        else
                        {
                            error = "MES response A013 timeout";
                        }
                        LogInDB.Error(error);
                        DAL.me.UpdateMesInterfaceLogErrorMsg("A013", root.Header.SessionID, error);
                        DAL.me.InsertToMESofflineBuffer(root.Header.EQCode, "A013", root.Header.SessionID, jason, lstBarcode);
                        MesErrorStopPLC(root.Header.EQCode);
                        LogInDB.Info($"Excuted A013 spends {(int)(DateTime.Now - startTime).TotalMilliseconds} ms");
                        return null;
                    }
                    try
                    {
                        if (A014_Buffer1 != null && A014_Buffer1.jason.Contains(offlineDataInfo.Guid))
                        {
                            A014.Root _root = JsonNewtonsoft.FromJSON<A014.Root>(A014_Buffer1.jason);
                            A014_Buffer1 = null;
                            LogInDB.Info($"Excuted A013 spends {(int)(DateTime.Now - startTime).TotalMilliseconds} ms");
                            return _root;
                        }
                        else if (A014_Buffer2 != null && A014_Buffer2.jason.Contains(offlineDataInfo.Guid))
                        {
                            A014.Root _root = JsonNewtonsoft.FromJSON<A014.Root>(A014_Buffer2.jason);
                            A014_Buffer2 = null;
                            LogInDB.Info($"Excuted A013 spends {(int)(DateTime.Now - startTime).TotalMilliseconds} ms");
                            return _root;
                        }
                        else if (A014_Buffer3 != null && A014_Buffer3.jason.Contains(offlineDataInfo.Guid))
                        {
                            A014.Root _root = JsonNewtonsoft.FromJSON<A014.Root>(A014_Buffer3.jason);
                            A014_Buffer3 = null;
                            LogInDB.Info($"Excuted A013 spends {(int)(DateTime.Now - startTime).TotalMilliseconds} ms");
                            return _root;
                        }
                        else if (A014_Buffer4 != null && A014_Buffer4.jason.Contains(offlineDataInfo.Guid))
                        {
                            A014.Root _root = JsonNewtonsoft.FromJSON<A014.Root>(A014_Buffer4.jason);
                            A014_Buffer4 = null;
                            LogInDB.Info($"Excuted A013 spends {(int)(DateTime.Now - startTime).TotalMilliseconds} ms");
                            return _root;
                        }
                    }
                    catch (Exception ex)
                    {
                        if (ATL.Common.StringResources.IsDefaultLanguage)
                        {
                            error = "解析 A014 Jason失败：" + ex.Message;
                        }
                        else
                        {
                            error = "Analysis Jaoson-A014 failure：" + ex.Message;
                        }
                        LogInDB.Error(error);
                        DAL.me.UpdateMesInterfaceLogErrorMsg("A014", root.Header.SessionID, error);
                        LogInDB.Info($"Excuted A013 spends {(int)(DateTime.Now - startTime).TotalMilliseconds} ms");
                        return null;
                    }
                }
            }
            else
            {
                LogInDB.Error(error);
                DAL.me.InsertToMESofflineBuffer(root.Header.EQCode, "A013", root.Header.SessionID, jason, lstBarcode);
                DAL.me.InsertMesInterfaceLog("A013", root.Header.SessionID,
                    DateTime.ParseExact(root.Header.RequestTime, "yyyy-MM-dd HH:mm:ss fff", System.Globalization.CultureInfo.CurrentCulture).ToString("yyyy-MM-dd HH:mm:ss.fff"),
                     null, jason, error);
                LogInDB.Info($"Excuted A013 spends {(int)(DateTime.Now - startTime).TotalMilliseconds} ms");
                return null;
            }
        }

        public A014.Root A013(string Type, List<A013.ProductsItem> lst, string EquipmentID = "")
        {
            DateTime startTime = DateTime.Now;
            int tryTimes = 0;
            A013.Root root = new A013.Root();
            string error = string.Empty;

            root.Header = new MES.A013.Header();
            root.Header.EQCode = string.IsNullOrEmpty(EquipmentID) ? Station.Station.Current.EquipmentID : EquipmentID;
            root.Header.FunctionID = "A013";
            root.Header.PCName = Station.Station.Current.PCName;
            root.Header.RequestTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff");
            root.Header.SessionID = Guid.NewGuid().ToString();
            root.Header.SoftName = Station.Station.Current.SoftName;

            root.RequestInfo = new A013.RequestInfo();
            root.RequestInfo.Type = Type;
            root.RequestInfo.Products = new List<MES.A013.ProductsItem>();
            List<string> lstBarcode = new List<string>();
            foreach (var Item in lst)
            {
                A013.ProductsItem ProductsItem = new A013.ProductsItem();
                ProductsItem.ProductSN = Item.ProductSN;
                lstBarcode.Add(Item.ProductSN);
                ProductsItem.Station = Item.Station;
                ProductsItem.Pass = Item.Pass;
                ProductsItem.ChildEquCode = Item.ChildEquCode;

                ProductsItem.OutputParam = Item.OutputParam;
                root.RequestInfo.Products.Add(ProductsItem);
            }

            string jason = root.ToJSON();
            OfflineDataInfo offlineDataInfo = new OfflineDataInfo();
            offlineDataInfo.Guid = root.Header.SessionID;
            offlineDataInfo.Data = jason;
            offlineDataInfo.FunctionID = root.Header.FunctionID;
            if (!_allowWork())
            {
                if (ATL.Common.StringResources.IsDefaultLanguage)
                {
                    LogInDB.Error("PC和MES通信中断");
                }
                else
                {
                    LogInDB.Error("PC soft MES interaction terminated，probable cause：pc soft start and config check failed、MES communication error");
                }
                DAL.me.InsertToMESofflineBuffer(root.Header.EQCode, "A013", root.Header.SessionID, jason, lstBarcode);
                LogInDB.Info($"Excuted A013 spends {(int)(DateTime.Now - startTime).TotalMilliseconds} ms");
                return null;
            }
            //A014_Buffer = new RecvdData();
            retry:
            tryTimes++;
            DateTime start = DateTime.Now;
            if (Send("A013", offlineDataInfo.Data, out error))
            {
                DAL.me.InsertMesInterfaceLog("A013", root.Header.SessionID,
                    DateTime.ParseExact(root.Header.RequestTime, "yyyy-MM-dd HH:mm:ss fff", System.Globalization.CultureInfo.CurrentCulture).ToString("yyyy-MM-dd HH:mm:ss.fff"),
                     null, jason, error);
                while (true)
                {
                    Thread.Sleep(1);
                    if ((DateTime.Now - start).TotalMilliseconds > ReceiveTimeOut)
                    {
                        if (!HeartBeatError && tryTimes < 3)
                            goto retry;
                        if (ATL.Common.StringResources.IsDefaultLanguage)
                        {
                            error = "MES响应A013超时";
                        }
                        else
                        {
                            error = "MES response A013 timeout";
                        }
                        LogInDB.Error(error);
                        DAL.me.UpdateMesInterfaceLogErrorMsg("A013", root.Header.SessionID, error);
                        DAL.me.InsertToMESofflineBuffer(root.Header.EQCode, "A013", root.Header.SessionID, jason, lstBarcode);
                        MesErrorStopPLC(root.Header.EQCode);
                        LogInDB.Info($"Excuted A013 spends {(int)(DateTime.Now - startTime).TotalMilliseconds} ms");
                        return null;
                    }
                    try
                    {
                        if (A014_Buffer1 != null && A014_Buffer1.jason.Contains(offlineDataInfo.Guid))
                        {
                            A014.Root _root = JsonNewtonsoft.FromJSON<A014.Root>(A014_Buffer1.jason);
                            A014_Buffer1 = null;
                            LogInDB.Info($"Excuted A013 spends {(int)(DateTime.Now - startTime).TotalMilliseconds} ms");
                            return _root;
                        }
                        else if (A014_Buffer2 != null && A014_Buffer2.jason.Contains(offlineDataInfo.Guid))
                        {
                            A014.Root _root = JsonNewtonsoft.FromJSON<A014.Root>(A014_Buffer2.jason);
                            A014_Buffer2 = null;
                            LogInDB.Info($"Excuted A013 spends {(int)(DateTime.Now - startTime).TotalMilliseconds} ms");
                            return _root;
                        }
                        else if (A014_Buffer3 != null && A014_Buffer3.jason.Contains(offlineDataInfo.Guid))
                        {
                            A014.Root _root = JsonNewtonsoft.FromJSON<A014.Root>(A014_Buffer3.jason);
                            A014_Buffer3 = null;
                            LogInDB.Info($"Excuted A013 spends {(int)(DateTime.Now - startTime).TotalMilliseconds} ms");
                            return _root;
                        }
                        else if (A014_Buffer4 != null && A014_Buffer4.jason.Contains(offlineDataInfo.Guid))
                        {
                            A014.Root _root = JsonNewtonsoft.FromJSON<A014.Root>(A014_Buffer4.jason);
                            A014_Buffer4 = null;
                            LogInDB.Info($"Excuted A013 spends {(int)(DateTime.Now - startTime).TotalMilliseconds} ms");
                            return _root;
                        }
                    }
                    catch (Exception ex)
                    {
                        if (ATL.Common.StringResources.IsDefaultLanguage)
                        {
                            error = "解析 A014 Jason失败：" + ex.Message;
                        }
                        else
                        {
                            error = "Analysis Jaoson-A014 failure：" + ex.Message;
                        }
                        LogInDB.Error(error);
                        DAL.me.UpdateMesInterfaceLogErrorMsg("A014", root.Header.SessionID, error);
                        LogInDB.Info($"Excuted A013 spends {(int)(DateTime.Now - startTime).TotalMilliseconds} ms");
                        return null;
                    }
                }
            }
            else
            {
                LogInDB.Error(error);
                DAL.me.InsertToMESofflineBuffer(root.Header.EQCode, "A013", root.Header.SessionID, jason, lstBarcode);
                DAL.me.InsertMesInterfaceLog("A013", root.Header.SessionID,
                    DateTime.ParseExact(root.Header.RequestTime, "yyyy-MM-dd HH:mm:ss fff", System.Globalization.CultureInfo.CurrentCulture).ToString("yyyy-MM-dd HH:mm:ss.fff"),
                     null, jason, error);
                LogInDB.Info($"Excuted A013 spends {(int)(DateTime.Now - startTime).TotalMilliseconds} ms");
                return null;
            }

        }

        public A014_MTW20.Root A013_MTW20(string Type, List<A013_MTW20._ProductsItem> lst, string EquipmentID = "")
        {
            DateTime startTime = DateTime.Now;
            int tryTimes = 0;
            A013_MTW20.Root root = new A013_MTW20.Root();
            string error = string.Empty;
            root.Header = new MES.A013_MTW20.Header();
            root.Header.EQCode = string.IsNullOrEmpty(EquipmentID) ? Station.Station.Current.EquipmentID : EquipmentID;
            root.Header.FunctionID = "A013";
            root.Header.PCName = Station.Station.Current.PCName;
            root.Header.RequestTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff");
            root.Header.SessionID = Guid.NewGuid().ToString();
            root.Header.SoftName = Station.Station.Current.SoftName;

            root.RequestInfo = new A013_MTW20.RequestInfo();
            root.RequestInfo.Type = Type;
            root.RequestInfo.Products = new List<MES.A013_MTW20.ProductsItem>();
            List<string> lstBarcode = new List<string>();
            List<DeviceInputOutputConfigInfo> lstDeviceOutputConfig = DeviceInputOutputConfigInfo.lstDeviceInputOutputConfigInfos.Where(x => !string.IsNullOrEmpty(x.BycellOutputAddr)).ToList();
            foreach (var Item in lst)
            {
                A013_MTW20.ProductsItem ProductsItem = new A013_MTW20.ProductsItem();
                ProductsItem.ProductSN = Item.ProductSN;
                ProductsItem.Station = Item.Station;
                ProductsItem.Pass = Item.Pass;
                lstBarcode.Add(Item.ProductSN);
                ProductsItem.OutputParam = new List<A013_MTW20.OutputParamItem>();
                foreach (var v in lstDeviceOutputConfig)
                {
                    int BycellOutputAddrLength = v.BycellOutputAddr.Split(',').Count();
                    if (BycellOutputAddrLength == lst.Count || BycellOutputAddrLength == 1)
                    {
                        foreach (var addr in v.BycellOutputAddr.Split(','))
                        {
                            A013_MTW20.OutputParamItem OutputParamItem = new A013_MTW20.OutputParamItem();
                            OutputParamItem.ParamID = v.UploadParamID;
                            OutputParamItem.ParamDesc = v.ParamName;

                            UserVariableValueService Service = new UserVariableValueService();
                            OutputParamItem.ParamValue = Service.GetBycellOutputValue(v, addr);
                            OutputParamItem.Result = "UN";
                            ProductsItem.OutputParam.Add(OutputParamItem);
                        }
                    }
                }
                root.RequestInfo.Products.Add(ProductsItem);
            }

            string jason = root.ToJSON();
            OfflineDataInfo offlineDataInfo = new OfflineDataInfo();
            offlineDataInfo.Guid = root.Header.SessionID;
            offlineDataInfo.Data = jason;
            offlineDataInfo.FunctionID = root.Header.FunctionID;
            if (!_allowWork())
            {
                if (ATL.Common.StringResources.IsDefaultLanguage)
                {
                    LogInDB.Error("PC和MES通信中断");
                }
                else
                {
                    LogInDB.Error("PC soft MES interaction terminated，probable cause：pc soft start and config check failed、MES communication error");
                }
                DAL.me.InsertToMESofflineBuffer(root.Header.EQCode, "A013", root.Header.SessionID, jason, lstBarcode);
                LogInDB.Info($"Excuted A013 spends {(int)(DateTime.Now - startTime).TotalMilliseconds} ms");
                return null;
            }
            //A014_Buffer = new RecvdData();
            retry:
            tryTimes++;
            DateTime start = DateTime.Now;
            if (Send("A013", offlineDataInfo.Data, out error))
            {
                DAL.me.InsertMesInterfaceLog("A013", root.Header.SessionID,
                    DateTime.ParseExact(root.Header.RequestTime, "yyyy-MM-dd HH:mm:ss fff", System.Globalization.CultureInfo.CurrentCulture).ToString("yyyy-MM-dd HH:mm:ss.fff"),
                     null, jason, error);
                while (true)
                {
                    Thread.Sleep(1);
                    if ((DateTime.Now - start).TotalMilliseconds > ReceiveTimeOut)
                    {
                        if (!HeartBeatError && tryTimes < 3)
                            goto retry;
                        
                        if (ATL.Common.StringResources.IsDefaultLanguage)
                        {
                            error = "MES响应A013超时";
                        }
                        else
                        {
                            error = "MES response A013 timeout";
                        }
                        LogInDB.Error(error);
                        DAL.me.UpdateMesInterfaceLogErrorMsg("A013", root.Header.SessionID, error);
                        DAL.me.InsertToMESofflineBuffer(root.Header.EQCode, "A013", root.Header.SessionID, jason, lstBarcode);
                        MesErrorStopPLC(root.Header.EQCode);
                        LogInDB.Info($"Excuted A013 spends {(int)(DateTime.Now - startTime).TotalMilliseconds} ms");
                        return null;
                    }
                    try
                    {
                        if (A014_Buffer1 != null && A014_Buffer1.jason.Contains(offlineDataInfo.Guid))
                        {
                            A014_MTW20.Root _root = JsonNewtonsoft.FromJSON<A014_MTW20.Root>(A014_Buffer1.jason);
                            A014_Buffer1 = null;
                            LogInDB.Info($"Excuted A013 spends {(int)(DateTime.Now - startTime).TotalMilliseconds} ms");
                            return _root;
                        }
                        else if (A014_Buffer2 != null && A014_Buffer2.jason.Contains(offlineDataInfo.Guid))
                        {
                            A014_MTW20.Root _root = JsonNewtonsoft.FromJSON<A014_MTW20.Root>(A014_Buffer2.jason);
                            A014_Buffer2 = null;
                            LogInDB.Info($"Excuted A013 spends {(int)(DateTime.Now - startTime).TotalMilliseconds} ms");
                            return _root;
                        }
                        else if (A014_Buffer3 != null && A014_Buffer3.jason.Contains(offlineDataInfo.Guid))
                        {
                            A014_MTW20.Root _root = JsonNewtonsoft.FromJSON<A014_MTW20.Root>(A014_Buffer3.jason);
                            A014_Buffer3 = null;
                            LogInDB.Info($"Excuted A013 spends {(int)(DateTime.Now - startTime).TotalMilliseconds} ms");
                            return _root;
                        }
                        else if (A014_Buffer4 != null && A014_Buffer4.jason.Contains(offlineDataInfo.Guid))
                        {
                            A014_MTW20.Root _root = JsonNewtonsoft.FromJSON<A014_MTW20.Root>(A014_Buffer4.jason);
                            A014_Buffer4 = null;
                            LogInDB.Info($"Excuted A013 spends {(int)(DateTime.Now - startTime).TotalMilliseconds} ms");
                            return _root;
                        }
                    }
                    catch (Exception ex)
                    {
                        if (ATL.Common.StringResources.IsDefaultLanguage)
                        {
                            error = "解析 A014 Jason失败：" + ex.Message;
                        }
                        else
                        {
                            error = "Analysis Jaoson-A014 failure：" + ex.Message;
                        }
                        LogInDB.Error(error);
                        DAL.me.UpdateMesInterfaceLogErrorMsg("A014", root.Header.SessionID, error);
                        LogInDB.Info($"Excuted A013 spends {(int)(DateTime.Now - startTime).TotalMilliseconds} ms");
                        return null;
                    }
                }
            }
            else
            {
                LogInDB.Error(error);
                DAL.me.InsertToMESofflineBuffer(root.Header.EQCode, "A013", root.Header.SessionID, jason, lstBarcode);
                DAL.me.InsertMesInterfaceLog("A013", root.Header.SessionID,
                    DateTime.ParseExact(root.Header.RequestTime, "yyyy-MM-dd HH:mm:ss fff", System.Globalization.CultureInfo.CurrentCulture).ToString("yyyy-MM-dd HH:mm:ss.fff"),
                     null, jason, error);
                LogInDB.Info($"Excuted A013 spends {(DateTime.Now - startTime).TotalMilliseconds} ms");
                return null;
            }

        }

        public A014_MTW20.Root A013_MTW20(string Type, List<A013_MTW20.ProductsItem> lst, string EquipmentID = "", bool SkipTransitionValidation = false)
        {
            DateTime startTime = DateTime.Now;
            int tryTimes = 0;
            A013_MTW20.Root root = new A013_MTW20.Root();
            string error = string.Empty;

            root.Header = new MES.A013_MTW20.Header();
            root.Header.EQCode = string.IsNullOrEmpty(EquipmentID) ? Station.Station.Current.EquipmentID : EquipmentID;
            root.Header.FunctionID = "A013";
            root.Header.PCName = Station.Station.Current.PCName;
            root.Header.RequestTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff");
            root.Header.SessionID = Guid.NewGuid().ToString();
            root.Header.SoftName = Station.Station.Current.SoftName;

            root.RequestInfo = new A013_MTW20.RequestInfo();
            root.RequestInfo.Type = Type;
            root.RequestInfo.SkipTransitionValidation = SkipTransitionValidation;
            root.RequestInfo.Products = new List<MES.A013_MTW20.ProductsItem>();
                   
            List<string> lstBarcode = new List<string>();
            foreach (var Item in lst)
            {
                A013_MTW20.ProductsItem ProductsItem = new A013_MTW20.ProductsItem();
                ProductsItem.ProductSN = Item.ProductSN;
                lstBarcode.Add(Item.ProductSN);
                ProductsItem.Station = Item.Station;
                ProductsItem.Pass = Item.Pass;

                ProductsItem.OutputParam = Item.OutputParam;
                root.RequestInfo.Products.Add(ProductsItem);
            }

            string jason = root.ToJSON();
            OfflineDataInfo offlineDataInfo = new OfflineDataInfo();
            offlineDataInfo.Guid = root.Header.SessionID;
            offlineDataInfo.Data = jason;
            offlineDataInfo.FunctionID = root.Header.FunctionID;
            if (!_allowWork())
            {
                if (ATL.Common.StringResources.IsDefaultLanguage)
                {
                    LogInDB.Error("PC和MES通信中断");
                }
                else
                {
                    LogInDB.Error("PC soft MES interaction terminated，probable cause：pc soft start and config check failed、MES communication error");
                }
                DAL.me.InsertToMESofflineBuffer(root.Header.EQCode, "A013", root.Header.SessionID, jason, lstBarcode);
                LogInDB.Info($"Excuted A013 spends {(DateTime.Now - startTime).TotalMilliseconds} ms");
                return null;
            }
            //A014_Buffer = new RecvdData();
            retry:
            tryTimes++;
            DateTime start = DateTime.Now;
            if (Send("A013", offlineDataInfo.Data, out error))
            {
                DAL.me.InsertMesInterfaceLog("A013", root.Header.SessionID,
                    DateTime.ParseExact(root.Header.RequestTime, "yyyy-MM-dd HH:mm:ss fff", System.Globalization.CultureInfo.CurrentCulture).ToString("yyyy-MM-dd HH:mm:ss.fff"),
                     null, jason, error);
                while (true)
                {
                    Thread.Sleep(1);
                    if ((DateTime.Now - start).TotalMilliseconds > ReceiveTimeOut)
                    {
                        if (!HeartBeatError && tryTimes < 3)
                            goto retry;
                        if (ATL.Common.StringResources.IsDefaultLanguage)
                        {
                            error = "MES响应A013超时";
                        }
                        else
                        {
                            error = "MES response A013 timeout";
                        }
                        LogInDB.Error(error);
                        DAL.me.UpdateMesInterfaceLogErrorMsg("A013", root.Header.SessionID, error);
                        DAL.me.InsertToMESofflineBuffer(root.Header.EQCode, "A013", root.Header.SessionID, jason, lstBarcode);
                        MesErrorStopPLC(root.Header.EQCode);
                        LogInDB.Info($"Excuted A013 spends {(DateTime.Now - startTime).TotalMilliseconds} ms");
                        return null;
                    }
                    try
                    {
                        if (A014_Buffer1 != null && A014_Buffer1.jason.Contains(offlineDataInfo.Guid))
                        {
                            A014_MTW20.Root _root = JsonNewtonsoft.FromJSON<A014_MTW20.Root>(A014_Buffer1.jason);
                            A014_Buffer1 = null;
                            LogInDB.Info($"Excuted A013 spends {(DateTime.Now - startTime).TotalMilliseconds} ms");
                            return _root;
                        }
                        else if (A014_Buffer2 != null && A014_Buffer2.jason.Contains(offlineDataInfo.Guid))
                        {
                            A014_MTW20.Root _root = JsonNewtonsoft.FromJSON<A014_MTW20.Root>(A014_Buffer2.jason);
                            A014_Buffer2 = null;
                            LogInDB.Info($"Excuted A013 spends {(DateTime.Now - startTime).TotalMilliseconds} ms");
                            return _root;
                        }
                        else if (A014_Buffer3 != null && A014_Buffer3.jason.Contains(offlineDataInfo.Guid))
                        {
                            A014_MTW20.Root _root = JsonNewtonsoft.FromJSON<A014_MTW20.Root>(A014_Buffer3.jason);
                            A014_Buffer3 = null;
                            LogInDB.Info($"Excuted A013 spends {(DateTime.Now - startTime).TotalMilliseconds} ms");
                            return _root;
                        }
                        else if (A014_Buffer4 != null && A014_Buffer4.jason.Contains(offlineDataInfo.Guid))
                        {
                            A014_MTW20.Root _root = JsonNewtonsoft.FromJSON<A014_MTW20.Root>(A014_Buffer4.jason);
                            A014_Buffer4 = null;
                            LogInDB.Info($"Excuted A013 spends {(DateTime.Now - startTime).TotalMilliseconds} ms");
                            return _root;
                        }
                    }
                    catch (Exception ex)
                    {
                        if (ATL.Common.StringResources.IsDefaultLanguage)
                        {
                            error = "解析 A014 Jason失败：" + ex.Message;
                        }
                        else
                        {
                            error = "Analysis Jaoson-A014 failure：" + ex.Message;
                        }
                        LogInDB.Error(error);
                        DAL.me.UpdateMesInterfaceLogErrorMsg("A014", root.Header.SessionID, error);
                        LogInDB.Info($"Excuted A013 spends {(DateTime.Now - startTime).TotalMilliseconds} ms");
                        return null;
                    }
                }
            }
            else
            {
                LogInDB.Error(error);
                DAL.me.InsertToMESofflineBuffer(root.Header.EQCode, "A013", root.Header.SessionID, jason, lstBarcode);
                DAL.me.InsertMesInterfaceLog("A013", root.Header.SessionID,
                    DateTime.ParseExact(root.Header.RequestTime, "yyyy-MM-dd HH:mm:ss fff", System.Globalization.CultureInfo.CurrentCulture).ToString("yyyy-MM-dd HH:mm:ss.fff"),
                     null, jason, error);
                LogInDB.Info($"Excuted A013 spends {(DateTime.Now - startTime).TotalMilliseconds} ms");
                return null;
            }
        }

        public A014.Root A013Offline(string productSN, string jason)
        {
            DateTime startTime = DateTime.Now;
            int tryTimes = 0;
            string error = string.Empty;
            A013.Root root = JsonNewtonsoft.FromJSON<A013.Root>(jason);

            OfflineDataInfo offlineDataInfo = new OfflineDataInfo();
            offlineDataInfo.Guid = root.Header.SessionID;
            offlineDataInfo.Data = jason;
            offlineDataInfo.FunctionID = root.Header.FunctionID;
            
            retry:
            tryTimes++;
            DateTime start = DateTime.Now;
            if (Send("A013", jason, out error))
            {
                DAL.me.InsertMesInterfaceLog("A013", root.Header.SessionID,
                    DateTime.ParseExact(root.Header.RequestTime, "yyyy-MM-dd HH:mm:ss fff", System.Globalization.CultureInfo.CurrentCulture).ToString("yyyy-MM-dd HH:mm:ss.fff"),
                     null, jason, error);
                while (true)
                {
                    Thread.Sleep(1);
                    if ((DateTime.Now - start).TotalMilliseconds > ReceiveTimeOut)
                    {
                        if (!HeartBeatError && tryTimes <= 3)
                            goto retry;
                        error = "MES返回超时";
                        LogInDB.Error(error);
                        DAL.me.UpdateMesInterfaceLogErrorMsg("A013", root.Header.SessionID, error);
                        return null;
                    }
                    try
                    {
                        if (A014_Buffer1 != null && A014_Buffer1.jason.Contains(offlineDataInfo.Guid))
                        {
                            A014.Root _root = JsonNewtonsoft.FromJSON<A014.Root>(A014_Buffer1.jason);
                            A014_Buffer1 = null;
                            LogInDB.Info($"Excuted A013 spends {(DateTime.Now - startTime).TotalMilliseconds} ms");
                            return _root;
                        }
                        else if (A014_Buffer2 != null && A014_Buffer2.jason.Contains(offlineDataInfo.Guid))
                        {
                            A014.Root _root = JsonNewtonsoft.FromJSON<A014.Root>(A014_Buffer2.jason);
                            A014_Buffer2 = null;
                            LogInDB.Info($"Excuted A013 spends {(DateTime.Now - startTime).TotalMilliseconds} ms");
                            return _root;
                        }
                        else if (A014_Buffer3 != null && A014_Buffer3.jason.Contains(offlineDataInfo.Guid))
                        {
                            A014.Root _root = JsonNewtonsoft.FromJSON<A014.Root>(A014_Buffer3.jason);
                            A014_Buffer3 = null;
                            LogInDB.Info($"Excuted A013 spends {(DateTime.Now - startTime).TotalMilliseconds} ms");
                            return _root;
                        }
                        else if (A014_Buffer4 != null && A014_Buffer4.jason.Contains(offlineDataInfo.Guid))
                        {
                            A014.Root _root = JsonNewtonsoft.FromJSON<A014.Root>(A014_Buffer4.jason);
                            A014_Buffer4 = null;
                            LogInDB.Info($"Excuted A013 spends {(DateTime.Now - startTime).TotalMilliseconds} ms");
                            return _root;
                        }
                    }
                    catch (Exception ex)
                    {
                        error = "解析Jaoson-A014失败：" + ex.ToString();
                        LogInDB.Error(error);
                        DAL.me.UpdateMesInterfaceLogErrorMsg("A014", root.Header.SessionID, error);
                        return null;
                    }
                }
            }
            else
            {
                LogInDB.Error(error);
                DAL.me.InsertMesInterfaceLog("A013", root.Header.SessionID,
                    DateTime.ParseExact(root.Header.RequestTime, "yyyy-MM-dd HH:mm:ss fff", System.Globalization.CultureInfo.CurrentCulture).ToString("yyyy-MM-dd HH:mm:ss.fff"),
                     null, jason, error);
                return null;
            }
        }

        public A014_minicell.Root A013(string Type, string ProductSN, string Pass, string station, List<string> OutputParamItems,string operationMark, out string jsonData)
        {
            DateTime startTime = DateTime.Now;
            int tryTimes = 0;
            A013_minicell.Root root = new A013_minicell.Root();
            string error = string.Empty;

            root.Header = new MES.A013_minicell.Header();
            root.Header.EQCode = Station.Station.Current.EquipmentID;
            root.Header.FunctionID = "A013";
            root.Header.PCName = Station.Station.Current.PCName;
            root.Header.RequestTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff");
            root.Header.SessionID = Guid.NewGuid().ToString();
            root.Header.SoftName = Station.Station.Current.SoftName;
            
            root.RequestInfo = new A013_minicell.RequestInfo();
            root.RequestInfo.Type = Type;
            root.RequestInfo.SkipTransitionValidation = "False";
            root.RequestInfo.OperationMark = operationMark;
            root.RequestInfo.Products = new List<MES.A013_minicell.ProductsItem>();

            ATL.MES.A013_minicell.ProductsItem ProductsItem = new ATL.MES.A013_minicell.ProductsItem();
            ProductsItem.ProductSN = ProductSN;
            ProductsItem.Pass = Pass;
            ProductsItem.Station = station;
            ProductsItem.OutputParam = new List<ATL.MES.A013_minicell.OutputParamItem>();

            foreach (string json in OutputParamItems)
            {
                ATL.MES.A013_minicell.OutputParamItem OutputParamItem = JsonConvert.DeserializeObject<ATL.MES.A013_minicell.OutputParamItem>(json);
                ProductsItem.OutputParam.Add(OutputParamItem);
            }
            root.RequestInfo.Products.Add(ProductsItem);

            string jason = root.ToJSON();
            OfflineDataInfo offlineDataInfo = new OfflineDataInfo();
            offlineDataInfo.Guid = root.Header.SessionID;
            offlineDataInfo.Data = jason;
            offlineDataInfo.FunctionID = root.Header.FunctionID;

            jsonData = jason;

            retry:
            tryTimes++;
            DateTime start = DateTime.Now;
            if (Send("A013", offlineDataInfo.Data, out error))
            {
                DAL.me.InsertMesInterfaceLog("A013", root.Header.SessionID,
                    DateTime.ParseExact(root.Header.RequestTime, "yyyy-MM-dd HH:mm:ss fff", System.Globalization.CultureInfo.CurrentCulture).ToString("yyyy-MM-dd HH:mm:ss.fff"),
                     null, jason, error);
                while (true)
                {
                    Thread.Sleep(1);
                    if ((DateTime.Now - start).TotalMilliseconds > ReceiveTimeOut)
                    {
                        if (!HeartBeatError && tryTimes <= 3)
                            goto retry;
                        error = "MES返回超时";
                        LogInDB.Error(error);
                        DAL.me.UpdateMesInterfaceLogErrorMsg("A013", root.Header.SessionID, error);
                        return null;
                    }
                    try
                    {
                        if (A014_Buffer1 != null && A014_Buffer1.jason.Contains(offlineDataInfo.Guid))
                        {
                            A014_minicell.Root _root = JsonNewtonsoft.FromJSON<A014_minicell.Root>(A014_Buffer1.jason);
                            A014_Buffer1 = null;
                            LogInDB.Info($"Excuted A013 spends {(int)(DateTime.Now - startTime).TotalMilliseconds} ms");
                            return _root;
                        }
                        else if (A014_Buffer2 != null && A014_Buffer2.jason.Contains(offlineDataInfo.Guid))
                        {
                            A014_minicell.Root _root = JsonNewtonsoft.FromJSON<A014_minicell.Root>(A014_Buffer2.jason);
                            A014_Buffer2 = null;
                            LogInDB.Info($"Excuted A013 spends {(int)(DateTime.Now - startTime).TotalMilliseconds} ms");
                            return _root;
                        }
                        else if (A014_Buffer3 != null && A014_Buffer3.jason.Contains(offlineDataInfo.Guid))
                        {
                            A014_minicell.Root _root = JsonNewtonsoft.FromJSON<A014_minicell.Root>(A014_Buffer3.jason);
                            A014_Buffer3 = null;
                            LogInDB.Info($"Excuted A013 spends {(int)(DateTime.Now - startTime).TotalMilliseconds} ms");
                            return _root;
                        }
                        else if (A014_Buffer4 != null && A014_Buffer4.jason.Contains(offlineDataInfo.Guid))
                        {
                            A014_minicell.Root _root = JsonNewtonsoft.FromJSON<A014_minicell.Root>(A014_Buffer4.jason);
                            A014_Buffer4 = null;
                            LogInDB.Info($"Excuted A013 spends {(int)(DateTime.Now - startTime).TotalMilliseconds} ms");
                            return _root;
                        }
                    }
                    catch (Exception ex)
                    {
                        error = "解析Jaoson-A014失败：" + ex.ToString();
                        LogInDB.Error(error);
                        DAL.me.UpdateMesInterfaceLogErrorMsg("A014", root.Header.SessionID, error);
                        return null;
                    }
                }
            }
            else
            {
                LogInDB.Error(error);
                DAL.me.InsertMesInterfaceLog("A013", root.Header.SessionID,
                    DateTime.ParseExact(root.Header.RequestTime, "yyyy-MM-dd HH:mm:ss fff", System.Globalization.CultureInfo.CurrentCulture).ToString("yyyy-MM-dd HH:mm:ss.fff"),
                     null, jason, error);
                return null;
            }
        }

        public void A013Recheck(string Type, string ProductSN, string Pass, List<string> OutputParamItems, out string jsonData)
        {
            A013_minicell.Root root = new A013_minicell.Root();

            root.Header = new MES.A013_minicell.Header();
            root.Header.EQCode = Station.Station.Current.EquipmentID;
            root.Header.FunctionID = "A013";
            root.Header.PCName = Station.Station.Current.PCName;
            root.Header.RequestTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff");
            root.Header.SessionID = Guid.NewGuid().ToString();
            root.Header.SoftName = Station.Station.Current.SoftName;

            root.RequestInfo = new A013_minicell.RequestInfo();
            root.RequestInfo.Type = Type;
            root.RequestInfo.SkipTransitionValidation = "True";
            root.RequestInfo.Products = new List<MES.A013_minicell.ProductsItem>();

            ATL.MES.A013_minicell.ProductsItem ProductsItem = new ATL.MES.A013_minicell.ProductsItem();
            ProductsItem.ProductSN = ProductSN;
            ProductsItem.Pass = Pass;
            ProductsItem.Station = "1";
            ProductsItem.EmployeeNo = "PRD";
            ProductsItem.EmployeeNo1 = "";
            ProductsItem.OutputParam = new List<ATL.MES.A013_minicell.OutputParamItem>();

            foreach (string json in OutputParamItems)
            {
                ATL.MES.A013_minicell.OutputParamItem OutputParamItem = JsonConvert.DeserializeObject<ATL.MES.A013_minicell.OutputParamItem>(json);
                ProductsItem.OutputParam.Add(OutputParamItem);
            }
            root.RequestInfo.Products.Add(ProductsItem);

            jsonData = root.ToJSON();
        }

        //private object A015LokObj = new object();
        /// <summary>
        /// 发送上料相关数据
        /// </summary>
        /// <param name="MaterialID"></param>
        /// <param name="HighOrLowFlag"></param>
        /// <param name="error"></param>
        /// <returns>MES接收上料相关数据做验证、返回验证结果</returns>
        public A016.Root A015(string MaterialID, string Type, string Operation = "", string HighOrLowFlag = "", string EquipmentID = "")
        {
            DateTime startTime = DateTime.Now;
            int tryTimes = 0;
            A015.Root root = new A015.Root();
            string error = string.Empty;

            root.Header = new MES.A015.Header();
            root.Header.EQCode = string.IsNullOrEmpty(EquipmentID) ? Station.Station.Current.EquipmentID : EquipmentID;
            root.Header.FunctionID = "A015";
            root.Header.PCName = Station.Station.Current.PCName;
            root.Header.RequestTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff");
            root.Header.SessionID = Guid.NewGuid().ToString();
            root.Header.SoftName = Station.Station.Current.SoftName;

            root.RequestInfo = new A015.RequestInfo();
            root.RequestInfo.MaterialID = MaterialID;
            root.RequestInfo.HighOrLowFlag = HighOrLowFlag;
            root.RequestInfo.Type = Type;
            root.RequestInfo.Operation = Operation;

            string jason = root.ToJSON();
            OfflineDataInfo offlineDataInfo = new OfflineDataInfo();
            offlineDataInfo.Guid = root.Header.SessionID;
            offlineDataInfo.Data = jason;
            offlineDataInfo.FunctionID = root.Header.FunctionID;
            if (!_allowWork())
            {
                if (ATL.Common.StringResources.IsDefaultLanguage)
                {
                    LogInDB.Error("PC和MES通信中断");
                }
                else
                {
                    LogInDB.Error("PC soft MES interaction terminated，probable cause：pc soft start and config check failed、MES communication error");
                }                //DAL.me.InsertToMESofflineBuffer(root.Header.EQCode, "A015", root.Header.SessionID, jason);
                LogInDB.Info($"Excuted A015 spends {(DateTime.Now - startTime).TotalMilliseconds} ms");
                return null;
            }
            //A016_Buffer = new RecvdData();
            retry:
            tryTimes++;
            DateTime start = DateTime.Now;
            if (Send("A015", offlineDataInfo.Data, out error))
            {
                DAL.me.InsertMesInterfaceLog("A015", root.Header.SessionID,
                    DateTime.ParseExact(root.Header.RequestTime, "yyyy-MM-dd HH:mm:ss fff", System.Globalization.CultureInfo.CurrentCulture).ToString("yyyy-MM-dd HH:mm:ss.fff"),
                     null, jason, error);
                while (true)
                {
                    Thread.Sleep(1);
                    try
                    {
                        if (A016_Buffer1 != null && A016_Buffer1.jason.Contains(offlineDataInfo.Guid))
                        {
                            A016.Root _root = JsonNewtonsoft.FromJSON<A016.Root>(A016_Buffer1.jason);
                            A016_Buffer1 = null;
                            LogInDB.Info($"Excuted A015 spends {(DateTime.Now - startTime).TotalMilliseconds} ms");
                            return _root;
                        }
                        else if (A016_Buffer2 != null && A016_Buffer2.jason.Contains(offlineDataInfo.Guid))
                        {
                            A016.Root _root = JsonNewtonsoft.FromJSON<A016.Root>(A016_Buffer2.jason);
                            A016_Buffer2 = null;
                            LogInDB.Info($"Excuted A015 spends {(DateTime.Now - startTime).TotalMilliseconds} ms");
                            return _root;
                        }
                        else if (A016_Buffer3 != null && A016_Buffer3.jason.Contains(offlineDataInfo.Guid))
                        {
                            A016.Root _root = JsonNewtonsoft.FromJSON<A016.Root>(A016_Buffer3.jason);
                            A016_Buffer3 = null;
                            LogInDB.Info($"Excuted A015 spends {(DateTime.Now - startTime).TotalMilliseconds} ms");
                            return _root;
                        }
                        else if (A016_Buffer4 != null && A016_Buffer4.jason.Contains(offlineDataInfo.Guid))
                        {
                            A016.Root _root = JsonNewtonsoft.FromJSON<A016.Root>(A016_Buffer4.jason);
                            A016_Buffer4 = null;
                            LogInDB.Info($"Excuted A015 spends {(DateTime.Now - startTime).TotalMilliseconds} ms");
                            return _root;
                        }
                    }
                    catch (Exception ex)
                    {
                        if (ATL.Common.StringResources.IsDefaultLanguage)
                        {
                            error = "解析 A016 Jason失败：" + ex.Message;
                        }
                        else
                        {
                            error = "Analysis Jaoson-A016 failure：" + ex.Message;
                        }
                        LogInDB.Error(error);
                        DAL.me.UpdateMesInterfaceLogErrorMsg("A016", root.Header.SessionID, error);
                        LogInDB.Info($"Excuted A015 spends {(DateTime.Now - startTime).TotalMilliseconds} ms");
                        return null;
                    }
                    if ((DateTime.Now - start).TotalMilliseconds > ReceiveTimeOut)
                    {
                        if (!HeartBeatError && tryTimes < 3)
                            goto retry;
                        if (ATL.Common.StringResources.IsDefaultLanguage)
                        {
                            error = "MES响应A015超时";
                        }
                        else
                        {
                            error = "MES response A015 timeout";
                        }
                        LogInDB.Error(error);
                        DAL.me.UpdateMesInterfaceLogErrorMsg("A015", root.Header.SessionID, error);
                        //DAL.me.InsertToMESofflineBuffer(root.Header.EQCode, "A015", root.Header.SessionID, jason);
                        MesErrorStopPLC(root.Header.EQCode);
                        LogInDB.Info($"0 {(DateTime.Now - startTime).TotalMilliseconds} ms");
                        return null;
                    }
                }
            }
            else
            {
                LogInDB.Error(error);
                //DAL.me.InsertToMESofflineBuffer(root.Header.EQCode, "A015", root.Header.SessionID, jason, new List<string>() { MaterialID });
                DAL.me.InsertMesInterfaceLog("A015", root.Header.SessionID,
                    DateTime.ParseExact(root.Header.RequestTime, "yyyy-MM-dd HH:mm:ss fff", System.Globalization.CultureInfo.CurrentCulture).ToString("yyyy-MM-dd HH:mm:ss.fff"),
                     null, jason, error);
                LogInDB.Info($"Excuted A015 spends {(DateTime.Now - startTime).TotalMilliseconds} ms");
                return null;
            }
        }

        /// <summary>
        /// MES发送采集设备运行参数指令，用于点检；之后设备响应
        /// </summary>
        /// <param name="msg"></param>
        public void A018(object obj)
        {
            A017.Root msg = (A017.Root)obj;
            if (!_allowWork())
            {
                if (ATL.Common.StringResources.IsDefaultLanguage)
                {
                    LogInDB.Error("PC和MES通信中断");
                }
                else
                {
                    LogInDB.Error("PC soft MES interaction terminated，probable cause：pc soft start and config check failed、MES communication error");
                }
                return;
            }
            A018.Root root = new A018.Root();
            string error = string.Empty;

            root.Header = new A018.Header();
            root.Header.EQCode = msg.Header.EQCode;
            root.Header.FunctionID = "A018";
            root.Header.PCName = Station.Station.Current.PCName;
            root.Header.RequestTime = msg.Header.RequestTime;
            root.Header.SessionID = msg.Header.SessionID;
            root.Header.SoftName = Station.Station.Current.SoftName;
            root.Header.ErrorCode = "00";
            root.Header.ErrorMsg = "Null";
            root.Header.FunctionID = "A018";
            root.Header.IsSuccess = "True";
            root.Header.ResponseTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff");
            root.ResponseInfo = new A018.ResponseInfo();
            root.ResponseInfo.ParamInfo = new List<A018.ParamInfoItem>();

            List<DeviceInputOutputConfigInfo> lstDeviceOutputConfig = DeviceInputOutputConfigInfo.lstDeviceInputOutputConfigInfos.Where(x => (!string.IsNullOrEmpty(x.ActualValueAddr) || !string.IsNullOrEmpty(x.SettingValueAddr))
            && (!string.IsNullOrEmpty(x.UploadParamID) || !string.IsNullOrEmpty(x.SendParamID))).ToList();
            foreach (var v in msg.RequestInfo.ParamInfo)
            {
                DeviceInputOutputConfigInfo deviceInputOutputConfigInfo = lstDeviceOutputConfig.Where(x => x.UploadParamID == v.ParamID && x.EquipmentID == msg.Header.EQCode).FirstOrDefault();
                if (deviceInputOutputConfigInfo == null)  //MES下发的ID可能是OutputID,也可能是InputID
                    deviceInputOutputConfigInfo = lstDeviceOutputConfig.Where(x => x.SendParamID == v.ParamID && x.EquipmentID == msg.Header.EQCode).FirstOrDefault();
                if (deviceInputOutputConfigInfo == null)
                    continue;

                A018.ParamInfoItem item = new MES.A018.ParamInfoItem();
                item.ParamID = v.ParamID;
                item.ParamDesc = deviceInputOutputConfigInfo.ParamName;
                UserVariableValueService service = new UserVariableValueService();
                item.ParamValue = service.GetActualValue(deviceInputOutputConfigInfo);

                root.ResponseInfo.ParamInfo.Add(item);
            }

            string jason = root.ToJSON();
            OfflineDataInfo offlineDataInfo = new OfflineDataInfo();
            offlineDataInfo.Guid = root.Header.SessionID;
            offlineDataInfo.Data = jason;
            offlineDataInfo.FunctionID = root.Header.FunctionID;


            DateTime start = DateTime.Now;
            DAL.me.InsertMesInterfaceLog("A018", offlineDataInfo.Guid, null, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), jason, "");
            if (!Send("A018", offlineDataInfo.Data, out error))
                LogInDB.Error($"A018:{error}");
        }

        /// <summary>
        /// 发送设备状态数据
        /// </summary>
        /// <param name="EquipmentID"></param>
        /// <returns></returns>
        public A020.Root A019(string EquipmentID = "")
        {
            if (!_allowWork())
            {
                if (ATL.Common.StringResources.IsDefaultLanguage)
                {
                    LogInDB.Error("PC和MES通信中断");
                }
                else
                {
                    LogInDB.Error("PC soft MES interaction terminated，probable cause：pc soft start and config check failed、MES communication error");
                }
                return null;
            }
            int tryTimes = 0;
            A019.Root root = new A019.Root();
            string error = string.Empty;

            root.Header = new MES.A019.Header();
            root.Header.EQCode = string.IsNullOrEmpty(EquipmentID) ? Station.Station.Current.EquipmentID : EquipmentID;
            root.Header.FunctionID = "A019";
            root.Header.PCName = Station.Station.Current.PCName;
            root.Header.RequestTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff");
            root.Header.SessionID = Guid.NewGuid().ToString();
            root.Header.SoftName = Station.Station.Current.SoftName;

            root.RequestInfo = new A019.RequestInfo();

            DeivceEquipmentidPlcInfo deivceEquipmentidPlcInfo = DeivceEquipmentidPlcInfo.lstDeivceEquipmentidPlcInfos.Where(x => x.EquipmentID == root.Header.EQCode).FirstOrDefault();
            if (deivceEquipmentidPlcInfo == null)
                return null;
            
            root.RequestInfo.ParentEQStateCode = deivceEquipmentidPlcInfo.ParentEQStateCode;
            root.RequestInfo.AndonState = deivceEquipmentidPlcInfo.andonStateValue;
            Engine.PLC plc = new Engine.PLC();
            root.RequestInfo.Quantity = plc.ReadByVariableName(deivceEquipmentidPlcInfo.QuantityAddress);
            root.RequestInfo.ChildEQ = new List<MES.A019.ChildEQItem>();
            {
                string MiniCMStateValue = plc.ReadByVariableName(deivceEquipmentidPlcInfo.MiniCMAddress);
                DeviceStatusCode code = DeviceStatusCode.lstDeviceStatusCode.Where(x => x.PlcValue.ToString() == MiniCMStateValue
                    && x.Key == "MiniCMState").FirstOrDefault();
                if (code != null)
                {
                    deivceEquipmentidPlcInfo.MiniCMState = code.Status;
                    root.RequestInfo.MiniCMState = code.Status;
                }
                else
                {
                    deivceEquipmentidPlcInfo.MiniCMState = "0";
                    root.RequestInfo.MiniCMState = "0";
                }
            }
            foreach (var v in DeviceChildEqcodeInfo.lstDeviceChildEqcodeInfos)
            {
                A019.ChildEQItem item = new MES.A019.ChildEQItem();
                item.ChildEQCode = v.ChildEQCode;
                DeviceStatusCode code = DeviceStatusCode.lstDeviceStatusCode.Where(x => x.PlcValue.ToString() == deivceEquipmentidPlcInfo.andonStateValue
                    && x.Key == "ChildEQState").FirstOrDefault();
                if (code != null)
                    v.ChildEQState = code.Status;
                else
                    v.ChildEQState = string.Empty;

                item.ChildEQState = v.ChildEQState;
                root.RequestInfo.ChildEQ.Add(item);
            }

            string jason = root.ToJSON();
            OfflineDataInfo offlineDataInfo = new OfflineDataInfo();
            offlineDataInfo.Guid = root.Header.SessionID;
            offlineDataInfo.Data = jason;
            offlineDataInfo.FunctionID = root.Header.FunctionID;

            A020_Buffer = new RecvdData();
            retry:
            tryTimes++;
            DateTime start = DateTime.Now;
            if (Send("A019", offlineDataInfo.Data, out error))
            {
                DAL.me.InsertMesInterfaceLog("A019", root.Header.SessionID,
                    DateTime.ParseExact(root.Header.RequestTime, "yyyy-MM-dd HH:mm:ss fff", System.Globalization.CultureInfo.CurrentCulture).ToString("yyyy-MM-dd HH:mm:ss.fff"),
                     null, jason, error);
                while (true)
                {
                    Thread.Sleep(1);
                    if ((DateTime.Now - start).TotalMilliseconds > ReceiveTimeOut)
                    {
                        if (!HeartBeatError && tryTimes < 3)
                            goto retry;
                        if (ATL.Common.StringResources.IsDefaultLanguage)
                        {
                            error = "MES响应A019超时";
                        }
                        else
                        {
                            error = "MES response A019 timeout";
                        }
                        LogInDB.Error(error);
                        DAL.me.UpdateMesInterfaceLogErrorMsg("A019", root.Header.SessionID, error);
                        return null;
                    }
                    try
                    {
                        if (A020_Buffer.jason != null && A020_Buffer.jason.Contains(offlineDataInfo.Guid))
                        {
                            A020.Root _root = JsonNewtonsoft.FromJSON<A020.Root>(A020_Buffer.jason);
                            return _root;
                        }
                    }
                    catch (Exception ex)
                    {
                        if (ATL.Common.StringResources.IsDefaultLanguage)
                        {
                            error = "解析 A020 Jason失败：" + ex.Message;
                        }
                        else
                        {
                            error = "Analysis Jaoson-A020 failure：" + ex.Message;
                        }
                        LogInDB.Error(error);
                        DAL.me.UpdateMesInterfaceLogErrorMsg("A020", root.Header.SessionID, error);
                        return null;
                    }
                }
            }
            else
            {
                LogInDB.Error(error);
                DAL.me.InsertMesInterfaceLog("A019", root.Header.SessionID,
                    DateTime.ParseExact(root.Header.RequestTime, "yyyy-MM-dd HH:mm:ss fff", System.Globalization.CultureInfo.CurrentCulture).ToString("yyyy-MM-dd HH:mm:ss.fff"),
                     null, jason, error);
                return null;
            }
        }

        /// <summary>
        /// 设备向MES发送设备关键件寿命数据
        /// </summary>
        /// <param name="error"></param>
        /// <returns>MES返回已收到设备关键件寿命数据响应</returns>
        public A022.Root A021(string EquipmentID = "")
        {
            if (!_allowWork())
            {
                if (ATL.Common.StringResources.IsDefaultLanguage)
                {
                    LogInDB.Error("PC和MES通信中断");
                }
                else
                {
                    LogInDB.Error("PC soft MES interaction terminated，probable cause：pc soft start and config check failed、MES communication error");
                }
                return null;
            }
            int tryTimes = 0;
            A021.Root root = new A021.Root();
            string error = string.Empty;

            root.Header = new MES.A021.Header();
            root.Header.EQCode = string.IsNullOrEmpty(EquipmentID) ? Station.Station.Current.EquipmentID : EquipmentID;
            root.Header.FunctionID = "A021";
            root.Header.PCName = Station.Station.Current.PCName;
            root.Header.RequestTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff");
            root.Header.SessionID = Guid.NewGuid().ToString();
            root.Header.SoftName = Station.Station.Current.SoftName;

            root.RequestInfo = new A021.RequestInfo();
            root.RequestInfo.SpartLifeTimeInfo = new List<A021.SpartLifeTimeInfoItem>();

            foreach (var v in DeviceSpartConfigInfo.lstDeviceSpartConfigInfos.Where(x => x.EquipmentID == root.Header.EQCode).ToList())
            {
                A021.SpartLifeTimeInfoItem Item = new A021.SpartLifeTimeInfoItem();
                Item.SpartID = v.UploadParamID;
                Item.SpartName = v.ParamName;

                UserVariableValueService u = new UserVariableValueService();
                string LifeTime = u.GetDeviceSpartUsedLife(v);
                float lifetime;
                if (string.IsNullOrEmpty(LifeTime) || !float.TryParse(LifeTime, out lifetime))
                    return null;
                Item.UseLifetime = LifeTime;
                if ((lifetime > v.PercentWarning * v.SpartExpectedLifetime) && !v.Warned)
                {
                    MESmsg msg = new MESmsg();
                    msg.FunctionID = "A021";
                    //msg.translation = $"易损件{v.ParamName}即将寿命到期";
                    if (ATL.Common.StringResources.IsDefaultLanguage)
                    {
                        msg.translation = $"易损件{v.ParamName}即将寿命到期";
                    }
                    else
                    {
                        msg.translation = $"Vulnerable parts {v.ParamName} is about to expire ";
                    }
                    LogInDB.Info(msg.translation);
                    MESmsg.Add(msg);
                    //MessageBox.Show($"易损件{v.ParamName}即将寿命到期");
                    if (ATL.Common.StringResources.IsDefaultLanguage)
                    {
                        MessageBox.Show($"易损件{v.ParamName}即将寿命到期");
                    }
                    else
                    {
                        MessageBox.Show($"Vulnerable parts {v.ParamName} is about to expire");
                    }

                    v.Warned = true;
                }
                root.RequestInfo.SpartLifeTimeInfo.Add(Item);
            }

            string jason = root.ToJSON();
            OfflineDataInfo offlineDataInfo = new OfflineDataInfo();
            offlineDataInfo.Guid = root.Header.SessionID;
            offlineDataInfo.Data = jason;
            offlineDataInfo.FunctionID = root.Header.FunctionID;
            A022_Buffer = new RecvdData();
            retry:
            tryTimes++;
            DateTime start = DateTime.Now;
            if (Send("A021", offlineDataInfo.Data, out error))
            {
                DAL.me.InsertMesInterfaceLog("A021", root.Header.SessionID,
                    DateTime.ParseExact(root.Header.RequestTime, "yyyy-MM-dd HH:mm:ss fff", System.Globalization.CultureInfo.CurrentCulture).ToString("yyyy-MM-dd HH:mm:ss.fff"),
                     null, jason, error);
                while (true)
                {
                    Thread.Sleep(1);
                    if ((DateTime.Now - start).TotalMilliseconds > ReceiveTimeOut)
                    {
                        if (!HeartBeatError && tryTimes < 3)
                            goto retry;
                        if (ATL.Common.StringResources.IsDefaultLanguage)
                        {
                            error = "MES响应A021超时";
                        }
                        else
                        {
                            error = "MES response A021 timeout";
                        }
                        LogInDB.Error(error);
                        DAL.me.UpdateMesInterfaceLogErrorMsg("A021", root.Header.SessionID, error);
                        return null;
                    }
                    try
                    {
                        if (A022_Buffer.jason != null && A022_Buffer.jason.Contains(offlineDataInfo.Guid))
                        {
                            A022.Root _root = JsonNewtonsoft.FromJSON<A022.Root>(A022_Buffer.jason);
                            return _root;
                        }
                    }
                    catch (Exception ex)
                    {
                        if (ATL.Common.StringResources.IsDefaultLanguage)
                        {
                            error = "解析 A022 Jason失败：" + ex.Message;
                        }
                        else
                        {
                            error = "Analysis Jaoson-A022 failure：" + ex.Message;
                        }
                        LogInDB.Error(error);
                        DAL.me.UpdateMesInterfaceLogErrorMsg("A022", root.Header.SessionID, error);
                        return null;
                    }
                }
            }
            else
            {
                LogInDB.Error(error);
                DAL.me.InsertMesInterfaceLog("A021", root.Header.SessionID,
                    DateTime.ParseExact(root.Header.RequestTime, "yyyy-MM-dd HH:mm:ss fff", System.Globalization.CultureInfo.CurrentCulture).ToString("yyyy-MM-dd HH:mm:ss.fff"),
                     null, jason, error);
                return null;
            }
        }

        /// <summary>
        /// 更换易损件，下发易损件信息给设备之前，先将易损件当前寿命为零上传给MES。
        /// </summary>
        /// <param name="error"></param>
        /// <returns>MES返回已收到设备关键件寿命数据响应</returns>
        public A022.Root A021_ReplaceSpart(DeviceSpartConfigInfo deviceSpartConfigInfo)
        {
            if (!_allowWork())
            {
                if (ATL.Common.StringResources.IsDefaultLanguage)
                {
                    LogInDB.Error("PC和MES通信中断");
                }
                else
                {
                    LogInDB.Error("PC soft MES interaction terminated，probable cause：pc soft start and config check failed、MES communication error");
                }
                return null;
            }
            int tryTimes = 0;
            A021.Root root = new A021.Root();
            string error = string.Empty;

            root.Header = new MES.A021.Header();
            root.Header.EQCode = deviceSpartConfigInfo.EquipmentID;
            root.Header.FunctionID = "A021";
            root.Header.PCName = Station.Station.Current.PCName;
            root.Header.RequestTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff");
            root.Header.SessionID = Guid.NewGuid().ToString();
            root.Header.SoftName = Station.Station.Current.SoftName;

            root.RequestInfo = new A021.RequestInfo();
            root.RequestInfo.SpartLifeTimeInfo = new List<A021.SpartLifeTimeInfoItem>();

            DeviceSpartConfigInfo d = DeviceSpartConfigInfo.lstDeviceSpartConfigInfos.Where(x => x.EquipmentID == root.Header.EQCode && x.UploadParamID == deviceSpartConfigInfo.UploadParamID).FirstOrDefault();
            if (d == null)
            {
                //MessageBox.Show("当前要更换的零部件信息有误");
                if (ATL.Common.StringResources.IsDefaultLanguage)
                {
                    MessageBox.Show("当前要更换的零部件信息有误");
                }
                else
                {
                    MessageBox.Show("The information of the parts to be replaced is incorrect ");
                }
                return null;
            }

            A021.SpartLifeTimeInfoItem Item = new A021.SpartLifeTimeInfoItem();
            Item.SpartID = d.UploadParamID;
            Item.SpartName = d.ParamName;
            Item.UseLifetime = "0";
            root.RequestInfo.SpartLifeTimeInfo.Add(Item);

            string jason = root.ToJSON();
            OfflineDataInfo offlineDataInfo = new OfflineDataInfo();
            offlineDataInfo.Guid = root.Header.SessionID;
            offlineDataInfo.Data = jason;
            offlineDataInfo.FunctionID = root.Header.FunctionID;
            A022_Buffer = new RecvdData();
            retry:
            tryTimes++;
            DateTime start = DateTime.Now;
            if (Send("A021", offlineDataInfo.Data, out error))
            {
                DAL.me.InsertMesInterfaceLog("A021", root.Header.SessionID,
                    DateTime.ParseExact(root.Header.RequestTime, "yyyy-MM-dd HH:mm:ss fff", System.Globalization.CultureInfo.CurrentCulture).ToString("yyyy-MM-dd HH:mm:ss.fff"),
                     null, jason, error);
                while (true)
                {
                    Thread.Sleep(1);
                    if ((DateTime.Now - start).TotalMilliseconds > ReceiveTimeOut)
                    {
                        if (!HeartBeatError && tryTimes < 3)
                            goto retry;
                        if (ATL.Common.StringResources.IsDefaultLanguage)
                        {
                            error = "MES响应A021超时";
                        }
                        else
                        {
                            error = "MES response A021 timeout";
                        }
                        LogInDB.Error(error);
                        DAL.me.UpdateMesInterfaceLogErrorMsg("A021", root.Header.SessionID, error);
                        return null;
                    }
                    try
                    {
                        if (A022_Buffer.jason != null && A022_Buffer.jason.Contains(offlineDataInfo.Guid))
                        {
                            A022.Root _root = JsonNewtonsoft.FromJSON<A022.Root>(A022_Buffer.jason);
                            return _root;
                        }
                    }
                    catch (Exception ex)
                    {
                        if (ATL.Common.StringResources.IsDefaultLanguage)
                        {
                            error = "解析 A022 Jason失败：" + ex.Message;
                        }
                        else
                        {
                            error = "Analysis Jaoson-A022 failure：" + ex.Message;
                        }
                        LogInDB.Error(error);
                        DAL.me.UpdateMesInterfaceLogErrorMsg("A022", root.Header.SessionID, error);
                        return null;
                    }
                }
            }
            else
            {
                LogInDB.Error(error);
                DAL.me.InsertMesInterfaceLog("A021", root.Header.SessionID,
                    DateTime.ParseExact(root.Header.RequestTime, "yyyy-MM-dd HH:mm:ss fff", System.Globalization.CultureInfo.CurrentCulture).ToString("yyyy-MM-dd HH:mm:ss.fff"),
                     null, jason, error);
                return null;
            }
        }

        /// <summary>
        /// 更换易损件，下发易损件信息给设备之前，先将易损件当前寿命为零上传给MES。
        /// </summary>
        /// <param name="error"></param>
        /// <returns>MES返回已收到设备关键件寿命数据响应</returns>
        public A022.Root A021_ReplaceSpart(List<DeviceSpartConfigInfo> lst)
        {
            if (!_allowWork())
            {
                if (ATL.Common.StringResources.IsDefaultLanguage)
                {
                    LogInDB.Error("PC和MES通信中断");
                }
                else
                {
                    LogInDB.Error("PC soft MES interaction terminated，probable cause：pc soft start and config check failed、MES communication error");
                }
                return null;
            }
            int tryTimes = 0;
            A021.Root root = new A021.Root();
            string error = string.Empty;

            root.Header = new MES.A021.Header();
            root.Header.EQCode = lst[0].EquipmentID;
            root.Header.FunctionID = "A021";
            root.Header.PCName = Station.Station.Current.PCName;
            root.Header.RequestTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff");
            root.Header.SessionID = Guid.NewGuid().ToString();
            root.Header.SoftName = Station.Station.Current.SoftName;

            root.RequestInfo = new A021.RequestInfo();
            root.RequestInfo.SpartLifeTimeInfo = new List<A021.SpartLifeTimeInfoItem>();

            foreach (var v in DeviceSpartConfigInfo.lstDeviceSpartConfigInfos.Where(x => x.EquipmentID == root.Header.EQCode && x.NeedDownLoadSpartExpectedLifetime).ToList())
            {
                A021.SpartLifeTimeInfoItem Item = new A021.SpartLifeTimeInfoItem();
                Item.SpartID = v.UploadParamID;
                Item.SpartName = v.ParamName;
                Item.UseLifetime = v.MesSettingUsedLife.ToString(); 
                root.RequestInfo.SpartLifeTimeInfo.Add(Item);
            }

            string jason = root.ToJSON();
            OfflineDataInfo offlineDataInfo = new OfflineDataInfo();
            offlineDataInfo.Guid = root.Header.SessionID;
            offlineDataInfo.Data = jason;
            offlineDataInfo.FunctionID = root.Header.FunctionID;
            A022_Buffer = new RecvdData();
            retry:
            tryTimes++;
            DateTime start = DateTime.Now;
            if (Send("A021", offlineDataInfo.Data, out error))
            {
                DAL.me.InsertMesInterfaceLog("A021", root.Header.SessionID,
                    DateTime.ParseExact(root.Header.RequestTime, "yyyy-MM-dd HH:mm:ss fff", System.Globalization.CultureInfo.CurrentCulture).ToString("yyyy-MM-dd HH:mm:ss.fff"),
                     null, jason, error);
                while (true)
                {
                    Thread.Sleep(1);
                    if ((DateTime.Now - start).TotalMilliseconds > ReceiveTimeOut)
                    {
                        if (!HeartBeatError && tryTimes < 3)
                            goto retry;
                        if (ATL.Common.StringResources.IsDefaultLanguage)
                        {
                            error = "MES响应A021超时";
                        }
                        else
                        {
                            error = "MES response A021 timeout";
                        }
                        LogInDB.Error(error);
                        DAL.me.UpdateMesInterfaceLogErrorMsg("A021", root.Header.SessionID, error);
                        return null;
                    }
                    try
                    {
                        if (A022_Buffer.jason != null && A022_Buffer.jason.Contains(offlineDataInfo.Guid))
                        {
                            A022.Root _root = JsonNewtonsoft.FromJSON<A022.Root>(A022_Buffer.jason);
                            return _root;
                        }
                    }
                    catch (Exception ex)
                    {
                        if (ATL.Common.StringResources.IsDefaultLanguage)
                        {
                            error = "解析 A022 Jason失败：" + ex.Message;
                        }
                        else
                        {
                            error = "Analysis Jaoson-A022 failure：" + ex.Message;
                        }
                        LogInDB.Error(error);
                        DAL.me.UpdateMesInterfaceLogErrorMsg("A022", root.Header.SessionID, error);
                        return null;
                    }
                }
            }
            else
            {
                LogInDB.Error(error);
                DAL.me.InsertMesInterfaceLog("A021", root.Header.SessionID,
                    DateTime.ParseExact(root.Header.RequestTime, "yyyy-MM-dd HH:mm:ss fff", System.Globalization.CultureInfo.CurrentCulture).ToString("yyyy-MM-dd HH:mm:ss.fff"),
                     null, jason, error);
                return null;
            }
        }

        /// <summary>
        /// 发送版本号
        /// </summary>
        /// <param name="SoftVersion"></param>
        /// <param name="PlcVersion"></param>
        /// <param name="HmiVersion"></param>
        /// <param name="EquipmentID"></param>
        /// <returns></returns>
        public A024.Root A023(string EquipmentID = "")
        {
            if (!_allowWork())
            {
                if (ATL.Common.StringResources.IsDefaultLanguage)
                {
                    LogInDB.Error("PC和MES通信中断");
                }
                else
                {
                    LogInDB.Error("PC soft MES interaction terminated，probable cause：pc soft start and config check failed、MES communication error");
                }
                return null;
            }
            int tryTimes = 0;
            A023.Root root = new A023.Root();
            string error = string.Empty;

            root.Header = new MES.A023.Header();
            root.Header.EQCode = string.IsNullOrEmpty(EquipmentID) ? Station.Station.Current.EquipmentID : EquipmentID;
            root.Header.FunctionID = "A023";
            root.Header.PCName = Station.Station.Current.PCName;
            root.Header.RequestTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff");
            root.Header.SessionID = Guid.NewGuid().ToString();
            root.Header.SoftName = Station.Station.Current.SoftName;

            root.RequestInfo = new A023.RequestInfo();
            root.RequestInfo.ResourceRecordInfo = new List<MES.A023.ResourceRecordInfoItem>();


            string PlcVersion = plc.ReadByVariableName("PLCversion");
            string HmiVersion = plc.ReadByVariableName("HMIversion");
            string SoftVersion = Station.Station.Current.LabVersion;
            if (UserDefineVariableInfo.DicVariables._dict.ContainsKey("PCsoft001"))
            {
                
            }
            for (int i = 0; i < 3; i++)
            {
                A023.ResourceRecordInfoItem item = new A023.ResourceRecordInfoItem();
                if (i == 0)
                {
                    item.ResourceID = root.Header.EQCode;
                    item.ProgramName = "PCsoft001";
                    item.ProgramVension = SoftVersion;
                    root.RequestInfo.ResourceRecordInfo.Add(item);
                }
                else if (i == 1 && !string.IsNullOrEmpty(PlcVersion) && ID_Device.lstDevices.Count > 0)
                {
                    item.ResourceID = root.Header.EQCode;
                    item.ProgramName = "PLC001";
                    //TODO:从PLC读出来的版本号高低位顺序反的，做一下处理 zxh 2021-04-14
                    item.ProgramVension = UpLowReverse(PlcVersion);
                    root.RequestInfo.ResourceRecordInfo.Add(item);
                }
                else if (i == 2 && !string.IsNullOrEmpty(HmiVersion) && ID_Device.lstDevices.Count > 0)
                {
                    item.ResourceID = root.Header.EQCode;
                    item.ProgramName = "HMI001";
                    //TODO:从PLC读出来的版本号高低位顺序反的，做一下处理 zxh 2021-04-14
                    item.ProgramVension = UpLowReverse(HmiVersion);
                    root.RequestInfo.ResourceRecordInfo.Add(item);
                }
            }
            if (UserDefineVariableInfo.DicVariables._dict.ContainsKey("PCsoftMES"))
            {
                A023.ResourceRecordInfoItem item = new A023.ResourceRecordInfoItem();
                item.ResourceID = root.Header.EQCode;
                item.ProgramName = "PCsoftMES";
                item.ProgramVension = UserDefineVariableInfo.DicVariables["PCsoftMES"].ToString();
                root.RequestInfo.ResourceRecordInfo.Add(item);
            }
            if (UserDefineVariableInfo.DicVariables._dict.ContainsKey("RobotVersion"))
            {
                A023.ResourceRecordInfoItem item = new A023.ResourceRecordInfoItem();
                item.ResourceID = root.Header.EQCode;
                item.ProgramName = "Robot001";
                //TODO:从PLC读出来的版本号高低位顺序反的，做一下处理 zxh 2021-04-14
                string version = plc.ReadByVariableName("RobotVersion");
                item.ProgramVension = UpLowReverse(version); //UserDefineVariableInfo.DicVariables["RobotVersion"].ToString();
                root.RequestInfo.ResourceRecordInfo.Add(item);
            }
            if (UserDefineVariableInfo.DicVariables._dict.ContainsKey("Robot2Version"))
            {
                A023.ResourceRecordInfoItem item = new A023.ResourceRecordInfoItem();
                item.ResourceID = root.Header.EQCode;
                item.ProgramName = "Robot002";
                //TODO:从PLC读出来的版本号高低位顺序反的，做一下处理 zxh 2021-04-14
                string version = plc.ReadByVariableName("Robot2Version");
                item.ProgramVension = UpLowReverse(version); //UserDefineVariableInfo.DicVariables["Robot2Version"].ToString();
                root.RequestInfo.ResourceRecordInfo.Add(item);
            }
            if (UserDefineVariableInfo.DicVariables._dict.ContainsKey("Robot3Version"))
            {
                A023.ResourceRecordInfoItem item = new A023.ResourceRecordInfoItem();
                item.ResourceID = root.Header.EQCode;
                item.ProgramName = "Robot003";
                //TODO:从PLC读出来的版本号高低位顺序反的，做一下处理 zxh 2021-04-14
                string version = plc.ReadByVariableName("Robot3Version");
                item.ProgramVension = UpLowReverse(version); //UserDefineVariableInfo.DicVariables["Robot3Version"].ToString();
                root.RequestInfo.ResourceRecordInfo.Add(item);
            }
            if (UserDefineVariableInfo.DicVariables._dict.ContainsKey("Robot4Version"))
            {
                A023.ResourceRecordInfoItem item = new A023.ResourceRecordInfoItem();
                item.ResourceID = root.Header.EQCode;
                item.ProgramName = "Robot004";
                //TODO:从PLC读出来的版本号高低位顺序反的，做一下处理 zxh 2021-04-14
                string version = plc.ReadByVariableName("Robot4Version");
                item.ProgramVension = UpLowReverse(version); //UserDefineVariableInfo.DicVariables["Robot4Version"].ToString();
                root.RequestInfo.ResourceRecordInfo.Add(item);
            }
            if (UserDefineVariableInfo.DicVariables._dict.ContainsKey("Robot5Version"))
            {
                A023.ResourceRecordInfoItem item = new A023.ResourceRecordInfoItem();
                item.ResourceID = root.Header.EQCode;
                item.ProgramName = "Robot005";
                item.ProgramVension = UserDefineVariableInfo.DicVariables["Robot5Version"].ToString();
                root.RequestInfo.ResourceRecordInfo.Add(item);
            }
            if (UserDefineVariableInfo.DicVariables._dict.ContainsKey("Robot6Version"))
            {
                A023.ResourceRecordInfoItem item = new A023.ResourceRecordInfoItem();
                item.ResourceID = root.Header.EQCode;
                item.ProgramName = "Robot006";
                item.ProgramVension = UserDefineVariableInfo.DicVariables["Robot6Version"].ToString();
                root.RequestInfo.ResourceRecordInfo.Add(item);
            }
            if (UserDefineVariableInfo.DicVariables._dict.ContainsKey("Robot7Version"))
            {
                A023.ResourceRecordInfoItem item = new A023.ResourceRecordInfoItem();
                item.ResourceID = root.Header.EQCode;
                item.ProgramName = "Robot007";
                item.ProgramVension = UserDefineVariableInfo.DicVariables["Robot7Version"].ToString();
                root.RequestInfo.ResourceRecordInfo.Add(item);
            }
            if (UserDefineVariableInfo.DicVariables._dict.ContainsKey("Robot8Version"))
            {
                A023.ResourceRecordInfoItem item = new A023.ResourceRecordInfoItem();
                item.ResourceID = root.Header.EQCode;
                item.ProgramName = "Robot008";
                item.ProgramVension = UserDefineVariableInfo.DicVariables["Robot8Version"].ToString();
                root.RequestInfo.ResourceRecordInfo.Add(item);
            }
            if (UserDefineVariableInfo.DicVariables._dict.ContainsKey("Robot9Version"))
            {
                A023.ResourceRecordInfoItem item = new A023.ResourceRecordInfoItem();
                item.ResourceID = root.Header.EQCode;
                item.ProgramName = "Robot009";
                item.ProgramVension = UserDefineVariableInfo.DicVariables["Robot9Version"].ToString();
                root.RequestInfo.ResourceRecordInfo.Add(item);
            }
            if (UserDefineVariableInfo.DicVariables._dict.ContainsKey("Robot00A"))
            {
                A023.ResourceRecordInfoItem item = new A023.ResourceRecordInfoItem();
                item.ResourceID = root.Header.EQCode;
                item.ProgramName = "Robot00A";
                item.ProgramVension = UserDefineVariableInfo.DicVariables["Robot00A"].ToString();
                root.RequestInfo.ResourceRecordInfo.Add(item);
            }
            if (UserDefineVariableInfo.DicVariables._dict.ContainsKey("Robot00B"))
            {
                A023.ResourceRecordInfoItem item = new A023.ResourceRecordInfoItem();
                item.ResourceID = root.Header.EQCode;
                item.ProgramName = "Robot00B";
                item.ProgramVension = UserDefineVariableInfo.DicVariables["Robot00B"].ToString();
                root.RequestInfo.ResourceRecordInfo.Add(item);
            }
            if (UserDefineVariableInfo.DicVariables._dict.ContainsKey("Robot00C"))
            {
                A023.ResourceRecordInfoItem item = new A023.ResourceRecordInfoItem();
                item.ResourceID = root.Header.EQCode;
                item.ProgramName = "Robot00C";
                item.ProgramVension = UserDefineVariableInfo.DicVariables["Robot00C"].ToString();
                root.RequestInfo.ResourceRecordInfo.Add(item);
            }
            if (UserDefineVariableInfo.DicVariables._dict.ContainsKey("PLC002"))
            {
                A023.ResourceRecordInfoItem item = new A023.ResourceRecordInfoItem();
                item.ResourceID = root.Header.EQCode;
                item.ProgramName = "PLC002";
                item.ProgramVension = UserDefineVariableInfo.DicVariables["PLC002"].ToString();
                root.RequestInfo.ResourceRecordInfo.Add(item);
            }
            if (UserDefineVariableInfo.DicVariables._dict.ContainsKey("PLC003"))
            {
                A023.ResourceRecordInfoItem item = new A023.ResourceRecordInfoItem();
                item.ResourceID = root.Header.EQCode;
                item.ProgramName = "PLC003";
                item.ProgramVension = UserDefineVariableInfo.DicVariables["PLC003"].ToString();
                root.RequestInfo.ResourceRecordInfo.Add(item);
            }
            if (UserDefineVariableInfo.DicVariables._dict.ContainsKey("PLC004"))
            {
                A023.ResourceRecordInfoItem item = new A023.ResourceRecordInfoItem();
                item.ResourceID = root.Header.EQCode;
                item.ProgramName = "PLC004";
                item.ProgramVension = UserDefineVariableInfo.DicVariables["PLC004"].ToString();
                root.RequestInfo.ResourceRecordInfo.Add(item);
            }
            if (UserDefineVariableInfo.DicVariables._dict.ContainsKey("PLC005"))
            {
                A023.ResourceRecordInfoItem item = new A023.ResourceRecordInfoItem();
                item.ResourceID = root.Header.EQCode;
                item.ProgramName = "PLC005";
                item.ProgramVension = UserDefineVariableInfo.DicVariables["PLC005"].ToString();
                root.RequestInfo.ResourceRecordInfo.Add(item);
            }
            if (UserDefineVariableInfo.DicVariables._dict.ContainsKey("PLC006"))
            {
                A023.ResourceRecordInfoItem item = new A023.ResourceRecordInfoItem();
                item.ResourceID = root.Header.EQCode;
                item.ProgramName = "PLC006";
                item.ProgramVension = UserDefineVariableInfo.DicVariables["PLC006"].ToString();
                root.RequestInfo.ResourceRecordInfo.Add(item);
            }
            if (UserDefineVariableInfo.DicVariables._dict.ContainsKey("PLC007"))
            {
                A023.ResourceRecordInfoItem item = new A023.ResourceRecordInfoItem();
                item.ResourceID = root.Header.EQCode;
                item.ProgramName = "PLC007";
                item.ProgramVension = UserDefineVariableInfo.DicVariables["PLC007"].ToString();
                root.RequestInfo.ResourceRecordInfo.Add(item);
            }
            if (UserDefineVariableInfo.DicVariables._dict.ContainsKey("PLC008"))
            {
                A023.ResourceRecordInfoItem item = new A023.ResourceRecordInfoItem();
                item.ResourceID = root.Header.EQCode;
                item.ProgramName = "PLC008";
                item.ProgramVension = UserDefineVariableInfo.DicVariables["PLC008"].ToString();
                root.RequestInfo.ResourceRecordInfo.Add(item);
            }
            if (UserDefineVariableInfo.DicVariables._dict.ContainsKey("PLC009"))
            {
                A023.ResourceRecordInfoItem item = new A023.ResourceRecordInfoItem();
                item.ResourceID = root.Header.EQCode;
                item.ProgramName = "PLC009";
                item.ProgramVension = UserDefineVariableInfo.DicVariables["PLC009"].ToString();
                root.RequestInfo.ResourceRecordInfo.Add(item);
            }
            if (UserDefineVariableInfo.DicVariables._dict.ContainsKey("HMI002"))
            {
                A023.ResourceRecordInfoItem item = new A023.ResourceRecordInfoItem();
                item.ResourceID = root.Header.EQCode;
                item.ProgramName = "HMI002";
                item.ProgramVension = UserDefineVariableInfo.DicVariables["HMI002"].ToString();
                root.RequestInfo.ResourceRecordInfo.Add(item);
            }
            if (UserDefineVariableInfo.DicVariables._dict.ContainsKey("HMI003"))
            {
                A023.ResourceRecordInfoItem item = new A023.ResourceRecordInfoItem();
                item.ResourceID = root.Header.EQCode;
                item.ProgramName = "HMI003";
                item.ProgramVension = UserDefineVariableInfo.DicVariables["HMI003"].ToString();
                root.RequestInfo.ResourceRecordInfo.Add(item);
            }
            if (UserDefineVariableInfo.DicVariables._dict.ContainsKey("HMI004"))
            {
                A023.ResourceRecordInfoItem item = new A023.ResourceRecordInfoItem();
                item.ResourceID = root.Header.EQCode;
                item.ProgramName = "HMI004";
                item.ProgramVension = UserDefineVariableInfo.DicVariables["HMI004"].ToString();
                root.RequestInfo.ResourceRecordInfo.Add(item);
            }
            if (UserDefineVariableInfo.DicVariables._dict.ContainsKey("HMI005"))
            {
                A023.ResourceRecordInfoItem item = new A023.ResourceRecordInfoItem();
                item.ResourceID = root.Header.EQCode;
                item.ProgramName = "HMI005";
                item.ProgramVension = UserDefineVariableInfo.DicVariables["HMI005"].ToString();
                root.RequestInfo.ResourceRecordInfo.Add(item);
            }
            if (UserDefineVariableInfo.DicVariables._dict.ContainsKey("HMI006"))
            {
                A023.ResourceRecordInfoItem item = new A023.ResourceRecordInfoItem();
                item.ResourceID = root.Header.EQCode;
                item.ProgramName = "HMI006";
                item.ProgramVension = UserDefineVariableInfo.DicVariables["HMI006"].ToString();
                root.RequestInfo.ResourceRecordInfo.Add(item);
            }
            if (UserDefineVariableInfo.DicVariables._dict.ContainsKey("HMI007"))
            {
                A023.ResourceRecordInfoItem item = new A023.ResourceRecordInfoItem();
                item.ResourceID = root.Header.EQCode;
                item.ProgramName = "HMI007";
                item.ProgramVension = UserDefineVariableInfo.DicVariables["HMI007"].ToString();
                root.RequestInfo.ResourceRecordInfo.Add(item);
            }
            if (UserDefineVariableInfo.DicVariables._dict.ContainsKey("HMI008"))
            {
                A023.ResourceRecordInfoItem item = new A023.ResourceRecordInfoItem();
                item.ResourceID = root.Header.EQCode;
                item.ProgramName = "HMI008";
                item.ProgramVension = UserDefineVariableInfo.DicVariables["HMI008"].ToString();
                root.RequestInfo.ResourceRecordInfo.Add(item);
            }
            if (UserDefineVariableInfo.DicVariables._dict.ContainsKey("HMI009"))
            {
                A023.ResourceRecordInfoItem item = new A023.ResourceRecordInfoItem();
                item.ResourceID = root.Header.EQCode;
                item.ProgramName = "HMI009";
                item.ProgramVension = UserDefineVariableInfo.DicVariables["HMI009"].ToString();
                root.RequestInfo.ResourceRecordInfo.Add(item);
            }
            if (UserDefineVariableInfo.DicVariables._dict.ContainsKey("HMI010"))
            {
                A023.ResourceRecordInfoItem item = new A023.ResourceRecordInfoItem();
                item.ResourceID = root.Header.EQCode;
                item.ProgramName = "HMI010";
                item.ProgramVension = UserDefineVariableInfo.DicVariables["HMI010"].ToString();
                root.RequestInfo.ResourceRecordInfo.Add(item);
            }
            if (UserDefineVariableInfo.DicVariables._dict.ContainsKey("CCDVersion"))
            {
                A023.ResourceRecordInfoItem item = new A023.ResourceRecordInfoItem();
                item.ResourceID = root.Header.EQCode;
                item.ProgramName = "CCD001";
                item.ProgramVension = UserDefineVariableInfo.DicVariables["CCDVersion"].ToString();
                root.RequestInfo.ResourceRecordInfo.Add(item);
            }
            if (UserDefineVariableInfo.DicVariables._dict.ContainsKey("CCD2Version"))
            {
                A023.ResourceRecordInfoItem item = new A023.ResourceRecordInfoItem();
                item.ResourceID = root.Header.EQCode;
                item.ProgramName = "CCD002";
                item.ProgramVension = UserDefineVariableInfo.DicVariables["CCD2Version"].ToString();
                root.RequestInfo.ResourceRecordInfo.Add(item);
            }
            if (UserDefineVariableInfo.DicVariables._dict.ContainsKey("CCD3Version"))
            {
                A023.ResourceRecordInfoItem item = new A023.ResourceRecordInfoItem();
                item.ResourceID = root.Header.EQCode;
                item.ProgramName = "CCD003";
                item.ProgramVension = UserDefineVariableInfo.DicVariables["CCD3Version"].ToString();
                root.RequestInfo.ResourceRecordInfo.Add(item);
            }
            if (UserDefineVariableInfo.DicVariables._dict.ContainsKey("CCD4Version"))
            {
                A023.ResourceRecordInfoItem item = new A023.ResourceRecordInfoItem();
                item.ResourceID = root.Header.EQCode;
                item.ProgramName = "CCD004";
                item.ProgramVension = UserDefineVariableInfo.DicVariables["CCD4Version"].ToString();
                root.RequestInfo.ResourceRecordInfo.Add(item);
            }
            if (UserDefineVariableInfo.DicVariables._dict.ContainsKey("CCD5Version"))
            {
                A023.ResourceRecordInfoItem item = new A023.ResourceRecordInfoItem();
                item.ResourceID = root.Header.EQCode;
                item.ProgramName = "CCD005";
                item.ProgramVension = UserDefineVariableInfo.DicVariables["CCD5Version"].ToString();
                root.RequestInfo.ResourceRecordInfo.Add(item);
            }
            if (UserDefineVariableInfo.DicVariables._dict.ContainsKey("CCD6Version"))
            {
                A023.ResourceRecordInfoItem item = new A023.ResourceRecordInfoItem();
                item.ResourceID = root.Header.EQCode;
                item.ProgramName = "CCD006";
                item.ProgramVension = UserDefineVariableInfo.DicVariables["CCD6Version"].ToString();
                root.RequestInfo.ResourceRecordInfo.Add(item);
            }
            if (UserDefineVariableInfo.DicVariables._dict.ContainsKey("CCD7Version"))
            {
                A023.ResourceRecordInfoItem item = new A023.ResourceRecordInfoItem();
                item.ResourceID = root.Header.EQCode;
                item.ProgramName = "CCD007";
                item.ProgramVension = UserDefineVariableInfo.DicVariables["CCD7Version"].ToString();
                root.RequestInfo.ResourceRecordInfo.Add(item);
            }
            if (UserDefineVariableInfo.DicVariables._dict.ContainsKey("CCD8Version"))
            {
                A023.ResourceRecordInfoItem item = new A023.ResourceRecordInfoItem();
                item.ResourceID = root.Header.EQCode;
                item.ProgramName = "CCD008";
                item.ProgramVension = UserDefineVariableInfo.DicVariables["CCD8Version"].ToString();
                root.RequestInfo.ResourceRecordInfo.Add(item);
            }
            if (UserDefineVariableInfo.DicVariables._dict.ContainsKey("CCD9Version"))
            {
                A023.ResourceRecordInfoItem item = new A023.ResourceRecordInfoItem();
                item.ResourceID = root.Header.EQCode;
                item.ProgramName = "CCD009";
                item.ProgramVension = UserDefineVariableInfo.DicVariables["CCD9Version"].ToString();
                root.RequestInfo.ResourceRecordInfo.Add(item);
            }
            if (UserDefineVariableInfo.DicVariables._dict.ContainsKey("CCD10Version"))
            {
                A023.ResourceRecordInfoItem item = new A023.ResourceRecordInfoItem();
                item.ResourceID = root.Header.EQCode;
                item.ProgramName = "CCD010";
                item.ProgramVension = UserDefineVariableInfo.DicVariables["CCD10Version"].ToString();
                root.RequestInfo.ResourceRecordInfo.Add(item);
            }
            if (UserDefineVariableInfo.DicVariables._dict.ContainsKey("PCsoft002"))
            {
                A023.ResourceRecordInfoItem item = new A023.ResourceRecordInfoItem();
                item.ResourceID = root.Header.EQCode;
                item.ProgramName = "PCsoft002";
                item.ProgramVension = UserDefineVariableInfo.DicVariables["PCsoft002"].ToString();
                root.RequestInfo.ResourceRecordInfo.Add(item);
            }
            if (UserDefineVariableInfo.DicVariables._dict.ContainsKey("PCsoft003"))
            {
                A023.ResourceRecordInfoItem item = new A023.ResourceRecordInfoItem();
                item.ResourceID = root.Header.EQCode;
                item.ProgramName = "PCsoft003";
                item.ProgramVension = UserDefineVariableInfo.DicVariables["PCsoft003"].ToString();
                root.RequestInfo.ResourceRecordInfo.Add(item);
            }
            if (UserDefineVariableInfo.DicVariables._dict.ContainsKey("PCsoft004"))
            {
                A023.ResourceRecordInfoItem item = new A023.ResourceRecordInfoItem();
                item.ResourceID = root.Header.EQCode;
                item.ProgramName = "PCsoft004";
                item.ProgramVension = UserDefineVariableInfo.DicVariables["PCsoft004"].ToString();
                root.RequestInfo.ResourceRecordInfo.Add(item);
            }
            if (UserDefineVariableInfo.DicVariables._dict.ContainsKey("PCsoft005"))
            {
                A023.ResourceRecordInfoItem item = new A023.ResourceRecordInfoItem();
                item.ResourceID = root.Header.EQCode;
                item.ProgramName = "PCsoft005";
                item.ProgramVension = UserDefineVariableInfo.DicVariables["PCsoft005"].ToString();
                root.RequestInfo.ResourceRecordInfo.Add(item);
            }
            string jason = root.ToJSON();
            OfflineDataInfo offlineDataInfo = new OfflineDataInfo();
            offlineDataInfo.Guid = root.Header.SessionID;
            offlineDataInfo.Data = jason;
            offlineDataInfo.FunctionID = root.Header.FunctionID;
            A024_Buffer = new RecvdData();
            retry:
            tryTimes++;
            DateTime start = DateTime.Now;
            if (Send("A023", offlineDataInfo.Data, out error))
            {
                DAL.me.InsertMesInterfaceLog("A023", root.Header.SessionID,
                    DateTime.ParseExact(root.Header.RequestTime, "yyyy-MM-dd HH:mm:ss fff", System.Globalization.CultureInfo.CurrentCulture).ToString("yyyy-MM-dd HH:mm:ss.fff"),
                     null, jason, error);
                while (true)
                {
                    Thread.Sleep(1);
                    if ((DateTime.Now - start).TotalMilliseconds > ReceiveTimeOut)
                    {
                        if (!HeartBeatError && tryTimes < 3)
                            goto retry;
                        if (ATL.Common.StringResources.IsDefaultLanguage)
                        {
                            error = "MES响应A023超时";
                        }
                        else
                        {
                            error = "MES response A023 timeout";
                        }
                        LogInDB.Error(error);
                        DAL.me.UpdateMesInterfaceLogErrorMsg("A023", root.Header.SessionID, error);
                        return null;
                    }
                    try
                    {
                        if (A024_Buffer.jason != null && A024_Buffer.jason.Contains(offlineDataInfo.Guid))
                        {
                            A024.Root _root = JsonNewtonsoft.FromJSON<A024.Root>(A024_Buffer.jason);
                            return _root;
                        }
                    }
                    catch (Exception ex)
                    {
                        if (ATL.Common.StringResources.IsDefaultLanguage)
                        {
                            error = "解析 A024 Jason失败：" + ex.Message;
                        }
                        else
                        {
                            error = "Analysis Jaoson-A024 failure：" + ex.Message;
                        }
                        LogInDB.Error(error);
                        DAL.me.UpdateMesInterfaceLogErrorMsg("A024", root.Header.SessionID, error);
                        return null;
                    }
                }
            }
            else
            {
                LogInDB.Error(error);
                DAL.me.InsertMesInterfaceLog("A023", root.Header.SessionID,
                    DateTime.ParseExact(root.Header.RequestTime, "yyyy-MM-dd HH:mm:ss fff", System.Globalization.CultureInfo.CurrentCulture).ToString("yyyy-MM-dd HH:mm:ss.fff"),
                     null, jason, error);
                return null;
            }
        }

        public A024.Root A023(A023.RequestInfo RequestInfo, string EquipmentID = "")
        {
            if (!_allowWork())
            {
                if (ATL.Common.StringResources.IsDefaultLanguage)
                {
                    LogInDB.Error("PC和MES通信中断");
                }
                else
                {
                    LogInDB.Error("PC soft MES interaction terminated，probable cause：pc soft start and config check failed、MES communication error");
                }
                return null;
            }
            int tryTimes = 0;
            A023.Root root = new A023.Root();
            string error = string.Empty;

            root.Header = new MES.A023.Header();
            root.Header.EQCode = string.IsNullOrEmpty(EquipmentID) ? Station.Station.Current.EquipmentID : EquipmentID;
            root.Header.FunctionID = "A023";
            root.Header.PCName = Station.Station.Current.PCName;
            root.Header.RequestTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff");
            root.Header.SessionID = Guid.NewGuid().ToString();
            root.Header.SoftName = Station.Station.Current.SoftName;

            root.RequestInfo = RequestInfo;

            string jason = root.ToJSON();
            OfflineDataInfo offlineDataInfo = new OfflineDataInfo();
            offlineDataInfo.Guid = root.Header.SessionID;
            offlineDataInfo.Data = jason;
            offlineDataInfo.FunctionID = root.Header.FunctionID;
            A024_Buffer = new RecvdData();
            retry:
            tryTimes++;
            DateTime start = DateTime.Now;
            if (Send("A023", offlineDataInfo.Data, out error))
            {
                DAL.me.InsertMesInterfaceLog("A023", root.Header.SessionID,
                    DateTime.ParseExact(root.Header.RequestTime, "yyyy-MM-dd HH:mm:ss fff", System.Globalization.CultureInfo.CurrentCulture).ToString("yyyy-MM-dd HH:mm:ss.fff"),
                     null, jason, error);
                while (true)
                {
                    Thread.Sleep(1);
                    if ((DateTime.Now - start).TotalMilliseconds > ReceiveTimeOut)
                    {
                        if (!HeartBeatError && tryTimes < 3)
                            goto retry;
                        if (ATL.Common.StringResources.IsDefaultLanguage)
                        {
                            error = "MES响应A023超时";
                        }
                        else
                        {
                            error = "MES response A023 timeout";
                        }
                        LogInDB.Error(error);
                        DAL.me.UpdateMesInterfaceLogErrorMsg("A023", root.Header.SessionID, error);
                        return null;
                    }
                    try
                    {
                        if (A024_Buffer.jason != null && A024_Buffer.jason.Contains(offlineDataInfo.Guid))
                        {
                            A024.Root _root = JsonNewtonsoft.FromJSON<A024.Root>(A024_Buffer.jason);
                            return _root;
                        }
                    }
                    catch (Exception ex)
                    {
                        if (ATL.Common.StringResources.IsDefaultLanguage)
                        {
                            error = "解析 A024 Jason失败：" + ex.Message;
                        }
                        else
                        {
                            error = "Analysis Jaoson-A024 failure：" + ex.Message;
                        }
                        LogInDB.Error(error);
                        DAL.me.UpdateMesInterfaceLogErrorMsg("A024", root.Header.SessionID, error);
                        return null;
                    }
                }
            }
            else
            {
                LogInDB.Error(error);
                DAL.me.InsertMesInterfaceLog("A023", root.Header.SessionID,
                    DateTime.ParseExact(root.Header.RequestTime, "yyyy-MM-dd HH:mm:ss fff", System.Globalization.CultureInfo.CurrentCulture).ToString("yyyy-MM-dd HH:mm:ss.fff"),
                     null, jason, error);
                return null;
            }
        }


        /// <summary>
        /// 设备向MES发送设备报警数据
        /// </summary>
        /// <param name="EquipmentID"></param>
        /// <returns></returns>
        public A026.Root A025(string EquipmentID = "")
        {
            if (!_allowWork())
            {
                if (ATL.Common.StringResources.IsDefaultLanguage)
                {
                    LogInDB.Error("PC和MES通信中断");
                }
                else
                {
                    LogInDB.Error("PC soft MES interaction terminated，probable cause：pc soft start and config check failed、MES communication error");
                }
                return null;
            }
            int tryTimes = 0;
            A025.Root root = new A025.Root();
            string error = string.Empty;
            root.Header = new MES.A025.Header();
            root.Header.EQCode = string.IsNullOrEmpty(EquipmentID) ? Station.Station.Current.EquipmentID : EquipmentID;
            root.Header.FunctionID = "A025";
            root.Header.PCName = Station.Station.Current.PCName;
            root.Header.RequestTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff");
            root.Header.SessionID = Guid.NewGuid().ToString();
            root.Header.SoftName = Station.Station.Current.SoftName;

            root.RequestInfo = new A025.RequestInfo();
            root.RequestInfo.ResourceAlertInfo = new List<A025.ResourceAlertInfoItem>();

            lock (DeviceAlertConfigInfo.alarmObj)
            {
                List<DeviceAlertConfigInfo> lst = DeviceAlertConfigInfo.lstAlarming.Where(x => x.EquipmentID == root.Header.EQCode).ToList();
                if (lst.Count == 0)
                    return null;
                foreach (var v in lst)
                {
                    A025.ResourceAlertInfoItem item = new MES.A025.ResourceAlertInfoItem();
                    item.AlertCode = v.UploadParamID;
                    item.AlertLevel = v.AlarmLevel.ToString();
                    item.AlertName = v.AlarmContent;
                    root.RequestInfo.ResourceAlertInfo.Add(item);
                }
                DeviceAlertConfigInfo.lstAlarming.RemoveAll(x => x.EquipmentID == root.Header.EQCode);
            }

            string jason = root.ToJSON();
            OfflineDataInfo offlineDataInfo = new OfflineDataInfo();
            offlineDataInfo.Guid = root.Header.SessionID;
            offlineDataInfo.Data = jason;
            offlineDataInfo.FunctionID = root.Header.FunctionID;
            A026_Buffer = new RecvdData();
            retry:
            tryTimes++;
            DateTime start = DateTime.Now;
            if (Send("A025", offlineDataInfo.Data, out error))
            {
                DAL.me.InsertMesInterfaceLog("A025", root.Header.SessionID,
                    DateTime.ParseExact(root.Header.RequestTime, "yyyy-MM-dd HH:mm:ss fff", System.Globalization.CultureInfo.CurrentCulture).ToString("yyyy-MM-dd HH:mm:ss.fff"),
                     null, jason, error);
                while (true)
                {
                    Thread.Sleep(1);
                    if ((DateTime.Now - start).TotalMilliseconds > ReceiveTimeOut)
                    {
                        if (!HeartBeatError && tryTimes < 3)
                            goto retry;
                        if (ATL.Common.StringResources.IsDefaultLanguage)
                        {
                            error = "MES响应A025超时";
                        }
                        else
                        {
                            error = "MES response A025 timeout";
                        }
                        LogInDB.Error(error);
                        DAL.me.UpdateMesInterfaceLogErrorMsg("A025", root.Header.SessionID, error);
                        return null;
                    }
                    try
                    {
                        if (A026_Buffer.jason != null && A026_Buffer.jason.Contains(offlineDataInfo.Guid))
                        {
                            A026.Root _root = JsonNewtonsoft.FromJSON<A026.Root>(A026_Buffer.jason);
                            return _root;
                        }
                    }
                    catch (Exception ex)
                    {
                        if (ATL.Common.StringResources.IsDefaultLanguage)
                        {
                            error = "解析 A026 Jason失败：" + ex.Message;
                        }
                        else
                        {
                            error = "Analysis Jaoson-A026 failure：" + ex.Message;
                        }
                        LogInDB.Error(error);
                        DAL.me.UpdateMesInterfaceLogErrorMsg("A026", root.Header.SessionID, error);
                        return null;
                    }
                }
            }
            else
            {
                LogInDB.Error(error);
                DAL.me.InsertMesInterfaceLog("A025", root.Header.SessionID,
                    DateTime.ParseExact(root.Header.RequestTime, "yyyy-MM-dd HH:mm:ss fff", System.Globalization.CultureInfo.CurrentCulture).ToString("yyyy-MM-dd HH:mm:ss.fff"),
                     null, jason, error);
                return null;
            }
        }


        /// <summary>
        /// 设备向MES发送单个Tab号和电芯号
        /// </summary>
        /// <param name="Tab"></param>
        /// <param name="Cell"></param>
        /// <param name="error"></param>
        /// <returns>MES将Tab和电芯号绑定，并返回绑定结果</returns>
        public A030.Root A029(string Tab, string Cell, string EquipmentID = "")
        {
            DateTime startTime = DateTime.Now;
            int tryTimes = 0;
            A029.Root root = new A029.Root();
            string error = string.Empty;

            root.Header = new A029.Header();
            root.Header.EQCode = string.IsNullOrEmpty(EquipmentID) ? Station.Station.Current.EquipmentID : EquipmentID;
            root.Header.FunctionID = "A029";
            root.Header.PCName = Station.Station.Current.PCName;
            root.Header.RequestTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff");
            root.Header.SessionID = Guid.NewGuid().ToString();
            root.Header.SoftName = Station.Station.Current.SoftName;

            root.RequestInfo = new A029.RequestInfo();
            root.RequestInfo.Tab = Tab;
            root.RequestInfo.Cell = Cell;

            string jason = root.ToJSON();
            OfflineDataInfo offlineDataInfo = new OfflineDataInfo();
            offlineDataInfo.Guid = root.Header.SessionID;
            offlineDataInfo.Data = jason;
            offlineDataInfo.FunctionID = root.Header.FunctionID;
            if (!_allowWork())
            {
                if (ATL.Common.StringResources.IsDefaultLanguage)
                {
                    LogInDB.Error("PC和MES通信中断");
                }
                else
                {
                    LogInDB.Error("PC soft MES interaction terminated，probable cause：pc soft start and config check failed、MES communication error");
                }                //DAL.me.InsertToMESofflineBuffer(root.Header.EQCode, "A029", root.Header.SessionID, jason);
                LogInDB.Info($"Excuted A029 spends {(DateTime.Now - startTime).TotalMilliseconds} ms");
                return null;
            }
            //A030_Buffer = new RecvdData();
            retry:
            tryTimes++;
            DateTime start = DateTime.Now;
            if (Send("A029", offlineDataInfo.Data, out error))
            {
                DAL.me.InsertMesInterfaceLog("A029", root.Header.SessionID,
                    DateTime.ParseExact(root.Header.RequestTime, "yyyy-MM-dd HH:mm:ss fff", System.Globalization.CultureInfo.CurrentCulture).ToString("yyyy-MM-dd HH:mm:ss.fff"),
                     null, jason, error);
                while (true)
                {
                    Thread.Sleep(1);
                    if ((DateTime.Now - start).TotalMilliseconds > ReceiveTimeOut)
                    {
                        if (!HeartBeatError && tryTimes < 3)
                            goto retry;
                        if (ATL.Common.StringResources.IsDefaultLanguage)
                        {
                            error = "MES响应A029超时";
                        }
                        else
                        {
                            error = "MES response A029 timeout";
                        }
                        LogInDB.Error(error);
                        DAL.me.UpdateMesInterfaceLogErrorMsg("A029", root.Header.SessionID, error);
                        //DAL.me.InsertToMESofflineBuffer(root.Header.EQCode, "A029", root.Header.SessionID, jason);
                        MesErrorStopPLC(root.Header.EQCode);
                        LogInDB.Info($"Excuted A029 spends {(DateTime.Now - startTime).TotalMilliseconds} ms");
                        return null;
                    }
                    try
                    {
                        if (A030_Buffer1 != null && A030_Buffer1.jason.Contains(offlineDataInfo.Guid))
                        {
                            A030.Root _root = JsonNewtonsoft.FromJSON<A030.Root>(A030_Buffer1.jason);
                            A030_Buffer1 = null;
                            LogInDB.Info($"Excuted A029 spends {(DateTime.Now - startTime).TotalMilliseconds} ms");
                            return _root;
                        }
                        else if (A030_Buffer2 != null && A030_Buffer2.jason.Contains(offlineDataInfo.Guid))
                        {
                            A030.Root _root = JsonNewtonsoft.FromJSON<A030.Root>(A030_Buffer2.jason);
                            A030_Buffer2 = null;
                            LogInDB.Info($"Excuted A029 spends {(DateTime.Now - startTime).TotalMilliseconds} ms");
                            return _root;
                        }
                        else if (A030_Buffer3 != null && A030_Buffer3.jason.Contains(offlineDataInfo.Guid))
                        {
                            A030.Root _root = JsonNewtonsoft.FromJSON<A030.Root>(A030_Buffer3.jason);
                            A030_Buffer3 = null;
                            LogInDB.Info($"Excuted A029 spends {(DateTime.Now - startTime).TotalMilliseconds} ms");
                            return _root;
                        }
                        else if (A030_Buffer4 != null && A030_Buffer4.jason.Contains(offlineDataInfo.Guid))
                        {
                            A030.Root _root = JsonNewtonsoft.FromJSON<A030.Root>(A030_Buffer4.jason);
                            A030_Buffer4 = null;
                            LogInDB.Info($"Excuted A029 spends {(DateTime.Now - startTime).TotalMilliseconds} ms");
                            return _root;
                        }
                    }
                    catch (Exception ex)
                    {
                        if (ATL.Common.StringResources.IsDefaultLanguage)
                        {
                            error = "解析 A030 Jason失败：" + ex.Message;
                        }
                        else
                        {
                            error = "Analysis Jaoson-A030 failure：" + ex.Message;
                        }
                        LogInDB.Error(error);
                        DAL.me.UpdateMesInterfaceLogErrorMsg("A030", root.Header.SessionID, error);
                        LogInDB.Info($"Excuted A029 spends {(DateTime.Now - startTime).TotalMilliseconds} ms");
                        return null;
                    }
                }
            }
            else
            {
                LogInDB.Error(error);
                //DAL.me.InsertToMESofflineBuffer(root.Header.EQCode, "A029", root.Header.SessionID, jason);
                DAL.me.InsertMesInterfaceLog("A029", root.Header.SessionID,
                    DateTime.ParseExact(root.Header.RequestTime, "yyyy-MM-dd HH:mm:ss fff", System.Globalization.CultureInfo.CurrentCulture).ToString("yyyy-MM-dd HH:mm:ss.fff"),
                     null, jason, error);
                LogInDB.Info($"Excuted A029 spends {(DateTime.Now - startTime).TotalMilliseconds} ms");
                return null;
            }
        }
        public A030.Root A029_MTW20(string Tab, string Cell,string NewSerialno, string OldSerialno, List<A029_MTW20.linkserialno> LinkSerialno, string EquipmentID = "")
        {
            DateTime startTime = DateTime.Now;
            int tryTimes = 0;
            A029_MTW20.Root root = new A029_MTW20.Root();
            string error = string.Empty;

            root.Header = new A029_MTW20.Header();
            root.Header.EQCode = string.IsNullOrEmpty(EquipmentID) ? Station.Station.Current.EquipmentID : EquipmentID;
            root.Header.FunctionID = "A029";
            root.Header.PCName = Station.Station.Current.PCName;
            root.Header.RequestTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff");
            root.Header.SessionID = Guid.NewGuid().ToString();
            root.Header.SoftName = Station.Station.Current.SoftName;

            root.RequestInfo = new A029_MTW20.RequestInfo();
            root.RequestInfo.Tab = Tab;
            root.RequestInfo.Cell = Cell;
            root.RequestInfo.NewSerialno  = NewSerialno;
            root.RequestInfo.OldSerialno = OldSerialno;
            root.RequestInfo.Linkserialno = LinkSerialno;

            string jason = root.ToJSON();
            OfflineDataInfo offlineDataInfo = new OfflineDataInfo();
            offlineDataInfo.Guid = root.Header.SessionID;
            offlineDataInfo.Data = jason;
            offlineDataInfo.FunctionID = root.Header.FunctionID;
            if (!_allowWork())
            {
                if (ATL.Common.StringResources.IsDefaultLanguage)
                {
                    LogInDB.Error("PC和MES通信中断");
                }
                else
                {
                    LogInDB.Error("PC soft MES interaction terminated，probable cause：pc soft start and config check failed、MES communication error");
                }                //DAL.me.InsertToMESofflineBuffer(root.Header.EQCode, "A029", root.Header.SessionID, jason);
                LogInDB.Info($"Excuted A029 spends {(DateTime.Now - startTime).TotalMilliseconds} ms");
                return null;
            }
            //A030_Buffer = new RecvdData();
            retry:
            tryTimes++;
            DateTime start = DateTime.Now;
            if (Send("A029", offlineDataInfo.Data, out error))
            {
                DAL.me.InsertMesInterfaceLog("A029", root.Header.SessionID,
                    DateTime.ParseExact(root.Header.RequestTime, "yyyy-MM-dd HH:mm:ss fff", System.Globalization.CultureInfo.CurrentCulture).ToString("yyyy-MM-dd HH:mm:ss.fff"),
                     null, jason, error);
                while (true)
                {
                    Thread.Sleep(1);
                    if ((DateTime.Now - start).TotalMilliseconds > ReceiveTimeOut)
                    {
                        if (!HeartBeatError && tryTimes < 3)
                            goto retry;

                        if (ATL.Common.StringResources.IsDefaultLanguage)
                        {
                            error = "MES响应A029超时";
                        }
                        else
                        {
                            error = "MES response A029 timeout";
                        }
                        LogInDB.Error(error);
                        DAL.me.UpdateMesInterfaceLogErrorMsg("A029", root.Header.SessionID, error);
                        //DAL.me.InsertToMESofflineBuffer(root.Header.EQCode, "A029", root.Header.SessionID, jason);
                        MesErrorStopPLC(root.Header.EQCode);
                        LogInDB.Info($"Excuted A029 spends {(DateTime.Now - startTime).TotalMilliseconds} ms");
                        return null;
                    }
                    try
                    {
                        if (A030_Buffer1 != null && A030_Buffer1.jason.Contains(offlineDataInfo.Guid))
                        {
                            A030.Root _root = JsonNewtonsoft.FromJSON<A030.Root>(A030_Buffer1.jason);
                            A030_Buffer1 = null;
                            LogInDB.Info($"Excuted A029 spends {(DateTime.Now - startTime).TotalMilliseconds} ms");
                            return _root;
                        }
                        else if (A030_Buffer2 != null && A030_Buffer2.jason.Contains(offlineDataInfo.Guid))
                        {
                            A030.Root _root = JsonNewtonsoft.FromJSON<A030.Root>(A030_Buffer2.jason);
                            A030_Buffer2 = null;
                            LogInDB.Info($"Excuted A029 spends {(DateTime.Now - startTime).TotalMilliseconds} ms");
                            return _root;
                        }
                        else if (A030_Buffer3 != null && A030_Buffer3.jason.Contains(offlineDataInfo.Guid))
                        {
                            A030.Root _root = JsonNewtonsoft.FromJSON<A030.Root>(A030_Buffer3.jason);
                            A030_Buffer3 = null;
                            LogInDB.Info($"Excuted A029 spends {(DateTime.Now - startTime).TotalMilliseconds} ms");
                            return _root;
                        }
                        else if (A030_Buffer4 != null && A030_Buffer4.jason.Contains(offlineDataInfo.Guid))
                        {
                            A030.Root _root = JsonNewtonsoft.FromJSON<A030.Root>(A030_Buffer4.jason);
                            A030_Buffer4 = null;
                            LogInDB.Info($"Excuted A029 spends {(DateTime.Now - startTime).TotalMilliseconds} ms");
                            return _root;
                        }
                    }
                    catch (Exception ex)
                    {
                        if (ATL.Common.StringResources.IsDefaultLanguage)
                        {
                            error = "解析 A030 Jason失败：" + ex.Message;
                        }
                        else
                        {
                            error = "Analysis Jaoson-A030 failure：" + ex.Message;
                        }
                        LogInDB.Error(error);
                        DAL.me.UpdateMesInterfaceLogErrorMsg("A030", root.Header.SessionID, error);
                        LogInDB.Info($"Excuted A029 spends {(DateTime.Now - startTime).TotalMilliseconds} ms");
                        return null;
                    }
                }
            }
            else
            {
                LogInDB.Error(error);
                //DAL.me.InsertToMESofflineBuffer(root.Header.EQCode, "A029", root.Header.SessionID, jason);
                DAL.me.InsertMesInterfaceLog("A029", root.Header.SessionID,
                    DateTime.ParseExact(root.Header.RequestTime, "yyyy-MM-dd HH:mm:ss fff", System.Globalization.CultureInfo.CurrentCulture).ToString("yyyy-MM-dd HH:mm:ss.fff"),
                     null, jason, error);
                LogInDB.Info($"Excuted A029 spends {(DateTime.Now - startTime).TotalMilliseconds} ms");
                return null;
            }
        }

        public class A031EventArgs : EventArgs
        {
            public A032.Root root { get; set; }
            public string error { get; set; }
        }

        private object A031LokObj = new object();
        public static event EventHandler<A031EventArgs> A031event;
        /// <summary>
        /// 设备向MES发送弹夹号及对应各个电芯号
        /// </summary>
        /// <param name="Carrier">弹夹</param>
        /// <param name="lstCell">电芯</param>
        /// <param name="error"></param>
        /// <returns>MES将弹夹和各个电芯绑定，并返回绑定结果</returns>
        public A032.Root A031(string Carrier, List<string> lstCell, string EquipmentID = "")
        {
            DateTime startTime = DateTime.Now;
            int tryTimes = 0;
            A031.Root root = new A031.Root();
            string error = string.Empty;

            root.Header = new MES.A031.Header();
            root.Header.EQCode = string.IsNullOrEmpty(EquipmentID) ? Station.Station.Current.EquipmentID : EquipmentID;
            root.Header.FunctionID = "A031";
            root.Header.PCName = Station.Station.Current.PCName;
            root.Header.RequestTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff");
            root.Header.SessionID = Guid.NewGuid().ToString();
            root.Header.SoftName = Station.Station.Current.SoftName;

            root.RequestInfo = new A031.RequestInfo();
            root.RequestInfo.Carrier = Carrier;
            root.RequestInfo.Cell = lstCell;

            string jason = root.ToJSON();
            OfflineDataInfo offlineDataInfo = new OfflineDataInfo();
            offlineDataInfo.Guid = root.Header.SessionID;
            offlineDataInfo.Data = jason;
            offlineDataInfo.FunctionID = root.Header.FunctionID;
            if (!_allowWork())
            {
                if (ATL.Common.StringResources.IsDefaultLanguage)
                {
                    LogInDB.Error("PC和MES通信中断");
                }
                else
                {
                    LogInDB.Error("PC soft MES interaction terminated，probable cause：pc soft start and config check failed、MES communication error");
                }                //DAL.me.InsertToMESofflineBuffer(root.Header.EQCode, "A031", root.Header.SessionID, jason);
                LogInDB.Info($"Excuted A031 spends {(DateTime.Now - startTime).TotalMilliseconds} ms");
                return null;
            }
            A032_Buffer = new RecvdData();
            retry:
            tryTimes++;
            lock(A031LokObj)
            {
                if (Send("A031", offlineDataInfo.Data, out error))
                {
                    DateTime start = DateTime.Now;
                    DAL.me.InsertMesInterfaceLog("A031", root.Header.SessionID,
                        DateTime.ParseExact(root.Header.RequestTime, "yyyy-MM-dd HH:mm:ss fff", System.Globalization.CultureInfo.CurrentCulture).ToString("yyyy-MM-dd HH:mm:ss.fff"),
                         null, jason, error);
                    while (true)
                    {
                        Thread.Sleep(1);
                        if ((DateTime.Now - start).TotalMilliseconds > ReceiveTimeOut)
                        {
                            if (!HeartBeatError && tryTimes < 3)
                                goto retry;

                            if (ATL.Common.StringResources.IsDefaultLanguage)
                            {
                                error = "MES响应A031超时";
                            }
                            else
                            {
                                error = "MES response A031 timeout";
                            }
                            LogInDB.Error(error);
                            DAL.me.UpdateMesInterfaceLogErrorMsg("A031", root.Header.SessionID, error);
                            //DAL.me.InsertToMESofflineBuffer(root.Header.EQCode, "A031", root.Header.SessionID, jason);
                            A031EventArgs arg = new A031EventArgs();
                            arg.error = error;
                            arg.root = null;
                            if (A031event != null)
                            {
                                A031event(this, arg);
                            }
                            MesErrorStopPLC(root.Header.EQCode);
                            LogInDB.Info($"Excuted A031 spends {(DateTime.Now - startTime).TotalMilliseconds} ms");
                            return null;
                        }
                        try
                        {
                            if (A032_Buffer.jason != null && A032_Buffer.jason.Contains(offlineDataInfo.Guid))
                            {
                                A032.Root _root = JsonNewtonsoft.FromJSON<A032.Root>(A032_Buffer.jason);
                                A031EventArgs arg = new A031EventArgs();
                                arg.root = _root;
                                if (A031event != null)
                                {
                                    A031event(this, arg);
                                }
                                LogInDB.Info($"Excuted A031 spends {(DateTime.Now - startTime).TotalMilliseconds} ms");
                                return _root;
                            }
                        }
                        catch (Exception ex)
                        {
                            if (ATL.Common.StringResources.IsDefaultLanguage)
                            {
                                error = "解析 A032 Jason失败：" + ex.Message;
                            }
                            else
                            {
                                error = "Analysis Jaoson-A032 failure：" + ex.Message;
                            }
                            LogInDB.Error(error);
                            DAL.me.UpdateMesInterfaceLogErrorMsg("A032", root.Header.SessionID, error);
                            A031EventArgs arg = new A031EventArgs();
                            arg.error = error;
                            arg.root = null;
                            if (A031event != null)
                            {
                                A031event(this, arg);
                            }
                            LogInDB.Info($"Excuted A031 spends {(DateTime.Now - startTime).TotalMilliseconds} ms");
                            return null;
                        }
                    }
                }
                else
                {
                    LogInDB.Error(error);
                    //DAL.me.InsertToMESofflineBuffer(root.Header.EQCode, "A031", root.Header.SessionID, jason);
                    DAL.me.InsertMesInterfaceLog("A031", root.Header.SessionID,
                        DateTime.ParseExact(root.Header.RequestTime, "yyyy-MM-dd HH:mm:ss fff", System.Globalization.CultureInfo.CurrentCulture).ToString("yyyy-MM-dd HH:mm:ss.fff"),
                         null, jason, error);
                    A031EventArgs arg = new A031EventArgs();
                    arg.error = error;
                    arg.root = null;
                    if (A031event != null)
                    {
                        A031event(this, arg);
                    }
                    LogInDB.Info($"Excuted A031 spends {(DateTime.Now - startTime).TotalMilliseconds} ms");
                    return null;
                }
            }
        }

        private object A033LokObj = new object();
        /// <summary>
        /// ACT的OUTPUT数据接口中发送电芯序列号与电芯码绑定、文件基本信息、电芯output数据
        /// </summary>
        /// <param name="RequestInfo"></param>
        /// <param name="error"></param>
        /// <returns>MES收到ACT的output数据,反馈电芯结果和不良代码</returns>
        public A034.Root A033(A033.RequestInfo RequestInfo, string EquipmentID = "")
        {
            DateTime startTime = DateTime.Now;
            int tryTimes = 0;
            A033.Root root = new A033.Root();
            string error = string.Empty;

            root.Header = new MES.A033.Header();
            root.Header.EQCode = string.IsNullOrEmpty(EquipmentID) ? Station.Station.Current.EquipmentID : EquipmentID;
            root.Header.FunctionID = "A033";
            root.Header.PCName = Station.Station.Current.PCName;
            root.Header.RequestTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff");
            root.Header.SessionID = Guid.NewGuid().ToString();
            root.Header.SoftName = Station.Station.Current.SoftName;
            root.RequestInfo = RequestInfo;
            List<string> lstBarcode = new List<string>();
            foreach (var v in RequestInfo.cells)
            {
                lstBarcode.Add(v.CELL_NAME);
            }
            string jason = root.ToJSON();
            OfflineDataInfo offlineDataInfo = new OfflineDataInfo();
            offlineDataInfo.Guid = root.Header.SessionID;
            offlineDataInfo.Data = jason;
            offlineDataInfo.FunctionID = root.Header.FunctionID;
            if (!_allowWork())
            {
                if (ATL.Common.StringResources.IsDefaultLanguage)
                {
                    LogInDB.Error("PC和MES通信中断");
                }
                else
                {
                    LogInDB.Error("PC soft MES interaction terminated，probable cause：pc soft start and config check failed、MES communication error");
                }
                DAL.me.InsertToMESofflineBuffer(root.Header.EQCode, "A033", root.Header.SessionID, jason, lstBarcode);
                LogInDB.Info($"Excuted A033 spends {(DateTime.Now - startTime).TotalMilliseconds} ms");
                return null;
            }
            A034_Buffer = new RecvdData();
            retry:
            tryTimes++;
            DateTime start = DateTime.Now;
            lock(A033LokObj)
            {

                if (Send("A033", offlineDataInfo.Data, out error))
                {
                    DAL.me.InsertMesInterfaceLog("A033", root.Header.SessionID,
                        DateTime.ParseExact(root.Header.RequestTime, "yyyy-MM-dd HH:mm:ss fff", System.Globalization.CultureInfo.CurrentCulture).ToString("yyyy-MM-dd HH:mm:ss.fff"),
                         null, jason, error);
                    while (true)
                    {
                        Thread.Sleep(1);
                        if ((DateTime.Now - start).TotalMilliseconds > ReceiveTimeOut)
                        {
                            if (!HeartBeatError && tryTimes < 3)
                                goto retry;

                            if (ATL.Common.StringResources.IsDefaultLanguage)
                            {
                                error = "MES响应A033超时";
                            }
                            else
                            {
                                error = "MES response A033 timeout";
                            }
                            LogInDB.Error(error);
                            DAL.me.UpdateMesInterfaceLogErrorMsg("A033", root.Header.SessionID, error);
                            DAL.me.InsertToMESofflineBuffer(root.Header.EQCode, "A033", root.Header.SessionID, jason, lstBarcode);
                            MesErrorStopPLC(root.Header.EQCode);
                            LogInDB.Info($"Excuted A033 spends {(DateTime.Now - startTime).TotalMilliseconds} ms");
                            return null;
                        }
                        try
                        {
                            if (A034_Buffer.jason != null && A034_Buffer.jason.Contains(offlineDataInfo.Guid))
                            {
                                A034.Root _root = JsonNewtonsoft.FromJSON<A034.Root>(A034_Buffer.jason);
                                LogInDB.Info($"Excuted A033 spends {(DateTime.Now - startTime).TotalMilliseconds} ms");
                                return _root;
                            }
                        }
                        catch (Exception ex)
                        {
                            if (ATL.Common.StringResources.IsDefaultLanguage)
                            {
                                error = "解析 A034 Jason失败：" + ex.Message;
                            }
                            else
                            {
                                error = "Analysis Jaoson-A034 failure：" + ex.Message;
                            }
                            LogInDB.Error(error);
                            DAL.me.UpdateMesInterfaceLogErrorMsg("A034", root.Header.SessionID, error);
                            LogInDB.Info($"Excuted A033 spends {(DateTime.Now - startTime).TotalMilliseconds} ms");
                            return null;
                        }
                    }
                }
                else
                {
                    LogInDB.Error(error);
                    DAL.me.InsertToMESofflineBuffer(root.Header.EQCode, "A033", root.Header.SessionID, jason, lstBarcode);
                    DAL.me.InsertMesInterfaceLog("A033", root.Header.SessionID,
                        DateTime.ParseExact(root.Header.RequestTime, "yyyy-MM-dd HH:mm:ss fff", System.Globalization.CultureInfo.CurrentCulture).ToString("yyyy-MM-dd HH:mm:ss.fff"),
                         null, jason, error);
                    LogInDB.Info($"Excuted A033 spends {(DateTime.Now - startTime).TotalMilliseconds} ms");
                    return null;
                }
            }
        }

        private object A035LokObj = new object();
        /// <summary>
        /// 涂布、冷压、分条上传Output数据
        /// </summary>
        /// <param name="Model"></param>
        /// <param name="ProductSN"></param>
        /// <param name="DataType">ProcessData/ProductionData</param>
        /// <param name="EquType"></param>
        /// <param name="lst"></param>
        /// <param name="EquipmentID"></param>
        /// <returns></returns>
        public A036.Root A035(A035.RequestInfo requestInfo, string EquipmentID = "")
        {
            int tryTimes = 0;
            A035.Root root = new A035.Root();
            string error = string.Empty;

            root.Header = new MES.A035.Header();
            root.Header.EQCode = string.IsNullOrEmpty(EquipmentID) ? Station.Station.Current.EquipmentID : EquipmentID;
            root.Header.FunctionID = "A035";
            root.Header.PCName = Station.Station.Current.PCName;
            root.Header.RequestTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff");
            root.Header.SessionID = Guid.NewGuid().ToString();
            root.Header.SoftName = Station.Station.Current.SoftName;

            root.RequestInfo = requestInfo;

            string jason = root.ToJSON();
            OfflineDataInfo offlineDataInfo = new OfflineDataInfo();
            offlineDataInfo.Guid = root.Header.SessionID;
            offlineDataInfo.Data = jason;
            offlineDataInfo.FunctionID = root.Header.FunctionID;
            if (!_allowWork())
            {
                if (ATL.Common.StringResources.IsDefaultLanguage)
                {
                    LogInDB.Error("PC和MES通信中断");
                }
                else
                {
                    LogInDB.Error("PC soft MES interaction terminated，probable cause：pc soft start and config check failed、MES communication error");
                }                //DAL.me.InsertToMESofflineBuffer(root.Header.EQCode, "A035", root.Header.SessionID, jason);
                return null;
            }
            A036_Buffer = new RecvdData();
            retry:
            tryTimes++;
            lock(A035LokObj)
            {
                if (Send("A035", offlineDataInfo.Data, out error))
                {
                    DateTime start = DateTime.Now;
                    DAL.me.InsertMesInterfaceLog("A035", root.Header.SessionID,
                        DateTime.ParseExact(root.Header.RequestTime, "yyyy-MM-dd HH:mm:ss fff", System.Globalization.CultureInfo.CurrentCulture).ToString("yyyy-MM-dd HH:mm:ss.fff"),
                         null, jason, error);
                    while (true)
                    {
                        Thread.Sleep(100);
                        if ((DateTime.Now - start).TotalMilliseconds > 15000)
                        {
                            if (!HeartBeatError && tryTimes < 3)
                                goto retry;

                            if (ATL.Common.StringResources.IsDefaultLanguage)
                            {
                                error = "MES响应A035超时";
                            }
                            else
                            {
                                error = "MES response A035 timeout";
                            }
                            LogInDB.Error(error);
                            DAL.me.UpdateMesInterfaceLogErrorMsg("A035", root.Header.SessionID, error);
                            //DAL.me.InsertToMESofflineBuffer(root.Header.EQCode, "A035", root.Header.SessionID, jason);
                            MesErrorStopPLC(root.Header.EQCode);
                            return null;
                        }
                        try
                        {
                            if (A036_Buffer.jason != null && A036_Buffer.jason.Contains(offlineDataInfo.Guid))
                            {
                                A036.Root _root = JsonNewtonsoft.FromJSON<A036.Root>(A036_Buffer.jason);
                                return _root;
                            }
                        }
                        catch (Exception ex)
                        {
                            if (ATL.Common.StringResources.IsDefaultLanguage)
                            {
                                error = "解析 A036 Jason失败：" + ex.Message;
                            }
                            else
                            {
                                error = "Analysis Jaoson-A036 failure：" + ex.Message;
                            }
                            LogInDB.Error(error);
                            DAL.me.UpdateMesInterfaceLogErrorMsg("A036", root.Header.SessionID, error);
                            A031EventArgs arg = new A031EventArgs();
                            arg.error = error;
                            arg.root = null;
                            return null;
                        }
                    }
                }
                else
                {
                    LogInDB.Error(error);
                    //DAL.me.InsertToMESofflineBuffer(root.Header.EQCode, "A035", root.Header.SessionID, jason);
                    DAL.me.InsertMesInterfaceLog("A036", root.Header.SessionID,
                        DateTime.ParseExact(root.Header.RequestTime, "yyyy-MM-dd HH:mm:ss fff", System.Globalization.CultureInfo.CurrentCulture).ToString("yyyy-MM-dd HH:mm:ss.fff"),
                         null, jason, error);
                    A031EventArgs arg = new A031EventArgs();
                    arg.error = error;
                    arg.root = null;
                    return null;
                }
            }
        }

        private object A037LokObj = new object();
        /// <summary>
        /// 设备向MES发送工产品信息请求
        /// </summary>
        /// <param name="ProductID"></param>
        /// <param name="error"></param>
        /// <returns>MES收到返回产品信息结果</returns>
        public A038.Root A037(string OperationCode,string ReworkFlag,List<A037.SerialNoInfoItem> SerialNoInfo, string EquipmentID = "")
        {
            if (!_allowWork())
            {
                if (ATL.Common.StringResources.IsDefaultLanguage)
                {
                    LogInDB.Error("PC和MES通信中断");
                }
                else
                {
                    LogInDB.Error("PC soft MES interaction terminated，probable cause：pc soft start and config check failed、MES communication error");
                }
                return null;
            }
            int tryTimes = 0;
            A037.Root root = new A037.Root();
            string error = string.Empty;

            root.Header = new A037.Header();
            root.Header.EQCode = string.IsNullOrEmpty(EquipmentID) ? Station.Station.Current.EquipmentID : EquipmentID;
            root.Header.FunctionID = "A037";
            root.Header.PCName = Station.Station.Current.PCName;
            root.Header.RequestTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff");
            root.Header.SessionID = Guid.NewGuid().ToString();
            root.Header.SoftName = Station.Station.Current.SoftName;

            root.RequestInfo = new A037.RequestInfo();
            root.RequestInfo.OperationCode = OperationCode;
            root.RequestInfo.ReworkFlag = ReworkFlag;
            root.RequestInfo.SerialNoInfo = SerialNoInfo;

            string jason = root.ToJSON();
            OfflineDataInfo offlineDataInfo = new OfflineDataInfo();
            offlineDataInfo.Guid = root.Header.SessionID;
            offlineDataInfo.Data = jason;
            offlineDataInfo.FunctionID = root.Header.FunctionID;
            A038_Buffer = new RecvdData();
            retry:
            tryTimes++;
            DateTime start = DateTime.Now;
            lock(A037LokObj)
            {
                if (Send("A037", offlineDataInfo.Data, out error))
                {
                    DAL.me.InsertMesInterfaceLog("A037", root.Header.SessionID,
                        DateTime.ParseExact(root.Header.RequestTime, "yyyy-MM-dd HH:mm:ss fff", System.Globalization.CultureInfo.CurrentCulture).ToString("yyyy-MM-dd HH:mm:ss.fff"),
                         null, jason, error);
                    while (true)
                    {
                        Thread.Sleep(1);
                        if ((DateTime.Now - start).TotalMilliseconds > ReceiveTimeOut)
                        {
                            if (!HeartBeatError && tryTimes < 3)
                                goto retry;

                            if (ATL.Common.StringResources.IsDefaultLanguage)
                            {
                                error = "MES响应A037超时";
                            }
                            else
                            {
                                error = "MES response A037 timeout";
                            }
                            LogInDB.Error(error);
                            DAL.me.UpdateMesInterfaceLogErrorMsg("A037", root.Header.SessionID, error);
                            return null;
                        }
                        try
                        {
                            if (A038_Buffer.jason != null && A038_Buffer.jason.Contains(offlineDataInfo.Guid))
                            {
                                A038.Root _root = JsonNewtonsoft.FromJSON<A038.Root>(A038_Buffer.jason);
                                return _root;
                            }
                        }
                        catch (Exception ex)
                        {
                            if (ATL.Common.StringResources.IsDefaultLanguage)
                            {
                                error = "解析 A038 Jason失败：" + ex.Message;
                            }
                            else
                            {
                                error = "Analysis Jaoson-A038 failure：" + ex.Message;
                            }
                            LogInDB.Error(error);
                            DAL.me.UpdateMesInterfaceLogErrorMsg("A038", root.Header.SessionID, error);
                            return null;
                        }
                    }
                }
                else
                {
                    LogInDB.Error(error);
                    DAL.me.InsertMesInterfaceLog("A037", root.Header.SessionID,
                        DateTime.ParseExact(root.Header.RequestTime, "yyyy-MM-dd HH:mm:ss fff", System.Globalization.CultureInfo.CurrentCulture).ToString("yyyy-MM-dd HH:mm:ss.fff"),
                         null, jason, error);
                    return null;
                }
            }
        }

        public delegate void A039LoginEvent(ATL.MES.A040.ResponseInfo info);
        public static A039LoginEvent a039LoginEvent;
        private object A039LokObj = new object();
        /// <summary>
        /// 设备向MES发送用户信息请求
        /// </summary>
        /// <param name="UserID"></param>
        /// <param name="UserPassword"></param>
        /// <param name="error"></param>
        /// <returns>MES收到返回用户信息结果</returns>
        public A040.Root A039(string UserID, string UserPassword,string TYPE = "1", string EquipmentID = "")
        {
            if (Client == null || !MESconnected || HeartBeatError || !allDeviceRegistered) return null;
            int tryTimes = 0;
            A039.Root root = new A039.Root();
            string error = string.Empty;

            root.Header = new A039.Header();
            root.Header.EQCode = string.IsNullOrEmpty(EquipmentID) ? Station.Station.Current.EquipmentID : EquipmentID;
            root.Header.FunctionID = "A039";
            root.Header.PCName = Station.Station.Current.PCName;
            root.Header.RequestTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff");
            root.Header.SessionID = Guid.NewGuid().ToString();
            root.Header.SoftName = Station.Station.Current.SoftName;

            root.RequestInfo = new A039.RequestInfo();
            root.RequestInfo.UserID = UserID;
            root.RequestInfo.UserPassword = UserPassword;
            root.RequestInfo.TYPE = TYPE;

            string jason = root.ToJSON();
            OfflineDataInfo offlineDataInfo = new OfflineDataInfo();
            offlineDataInfo.Guid = root.Header.SessionID;
            offlineDataInfo.Data = jason;
            offlineDataInfo.FunctionID = root.Header.FunctionID;
            A040_Buffer = new RecvdData();
            retry:
            tryTimes++;
            DateTime start = DateTime.Now;
            lock(A039LokObj)
            {
                if (Send("A039", offlineDataInfo.Data, out error))
                {
                    DAL.me.InsertMesInterfaceLog("A039", root.Header.SessionID,
                        DateTime.ParseExact(root.Header.RequestTime, "yyyy-MM-dd HH:mm:ss fff", System.Globalization.CultureInfo.CurrentCulture).ToString("yyyy-MM-dd HH:mm:ss.fff"),
                         null, jason, error);
                    while (true)
                    {
                        Thread.Sleep(1);
                        if ((DateTime.Now - start).TotalMilliseconds > ReceiveTimeOut)
                        {
                            if (!HeartBeatError && tryTimes < 3)
                                goto retry;

                            if (ATL.Common.StringResources.IsDefaultLanguage)
                            {
                                error = "MES响应A039超时";
                            }
                            else
                            {
                                error = "MES response A039 timeout";
                            }
                            LogInDB.Error(error);
                            DAL.me.UpdateMesInterfaceLogErrorMsg("A039", root.Header.SessionID, error);
                            return null;
                        }
                        try
                        {
                            if (A040_Buffer.jason != null && A040_Buffer.jason.Contains(offlineDataInfo.Guid))
                            {
                                A040.Root _root = JsonNewtonsoft.FromJSON<A040.Root>(A040_Buffer.jason);
                                try
                                {
                                    if(a039LoginEvent != null)
                                    {
                                        a039LoginEvent(_root.ResponseInfo);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    LogInDB.Error(ex.ToString());
                                }
                                return _root;
                            }
                        }
                        catch (Exception ex)
                        {
                            if (ATL.Common.StringResources.IsDefaultLanguage)
                            {
                                error = "解析 A040 Jason失败：" + ex.Message;
                            }
                            else
                            {
                                error = "Analysis Jaoson-A040 failure：" + ex.Message;
                            }
                            LogInDB.Error(error);
                            DAL.me.UpdateMesInterfaceLogErrorMsg("A040", root.Header.SessionID, error);
                            return null;
                        }
                    }
                }
                else
                {
                    LogInDB.Error(error);
                    DAL.me.InsertMesInterfaceLog("A039", root.Header.SessionID,
                        DateTime.ParseExact(root.Header.RequestTime, "yyyy-MM-dd HH:mm:ss fff", System.Globalization.CultureInfo.CurrentCulture).ToString("yyyy-MM-dd HH:mm:ss.fff"),
                         null, jason, error);
                    return null;
                }
            }
        }

        public A040.Root A039_New(string UserID, string UserPassword, string Type, string EquipmentID = "")
        {
            if (Client == null || !MESconnected || HeartBeatError || !allDeviceRegistered) return null;
            int tryTimes = 0;
            A039.Root root = new A039.Root();
            string error = string.Empty;

            root.Header = new A039.Header();
            root.Header.EQCode = string.IsNullOrEmpty(EquipmentID) ? Station.Station.Current.EquipmentID : EquipmentID;
            root.Header.FunctionID = "A039";
            root.Header.PCName = Station.Station.Current.PCName;
            root.Header.RequestTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff");
            root.Header.SessionID = Guid.NewGuid().ToString();
            root.Header.SoftName = Station.Station.Current.SoftName;

            root.RequestInfo = new A039.RequestInfo();
            root.RequestInfo.UserID = UserID;
            root.RequestInfo.UserPassword = UserPassword;
            root.RequestInfo.TYPE = Type;

            string jason = root.ToJSON();
            OfflineDataInfo offlineDataInfo = new OfflineDataInfo();
            offlineDataInfo.Guid = root.Header.SessionID;
            offlineDataInfo.Data = jason;
            offlineDataInfo.FunctionID = root.Header.FunctionID;
            A040_Buffer = new RecvdData();
            retry:
            tryTimes++;
            DateTime start = DateTime.Now;
            if (Send("A039", offlineDataInfo.Data, out error))
            {
                DAL.me.InsertMesInterfaceLog("A039", root.Header.SessionID,
                    DateTime.ParseExact(root.Header.RequestTime, "yyyy-MM-dd HH:mm:ss fff", System.Globalization.CultureInfo.CurrentCulture).ToString("yyyy-MM-dd HH:mm:ss.fff"),
                     null, jason, error);
                while (true)
                {
                    Thread.Sleep(0);
                    if ((DateTime.Now - start).TotalMilliseconds > ReceiveTimeOut)
                    {
                        if (!HeartBeatError && tryTimes <= 3)
                            goto retry;
                        error = "MES返回超时";
                        LogInDB.Error(error);
                        DAL.me.UpdateMesInterfaceLogErrorMsg("A039", root.Header.SessionID, error);
                        return null;
                    }
                    try
                    {
                        if (A040_Buffer.jason != null && A040_Buffer.jason.Contains(offlineDataInfo.Guid))
                        {
                            A040.Root _root = JsonNewtonsoft.FromJSON<A040.Root>(A040_Buffer.jason);
                            return _root;
                        }
                    }
                    catch (Exception ex)
                    {
                        error = "解析Jaoson-A040失败：" + ex.ToString();
                        LogInDB.Error(error);
                        DAL.me.UpdateMesInterfaceLogErrorMsg("A040", root.Header.SessionID, error);
                        return null;
                    }
                }
            }
            else
            {
                LogInDB.Error(error);
                DAL.me.InsertMesInterfaceLog("A039", root.Header.SessionID,
                    DateTime.ParseExact(root.Header.RequestTime, "yyyy-MM-dd HH:mm:ss fff", System.Globalization.CultureInfo.CurrentCulture).ToString("yyyy-MM-dd HH:mm:ss.fff"),
                     null, jason, error);
                return null;
            }
        }
        private object A041LokObj = new object();
        /// <summary>
        /// 设备向MES提供固定共享文件夹地址传送文件，传送完成向MES发送文件已生成请求
        /// </summary>
        /// <param name="FileTransmis"></param>
        /// <param name="FolderPath"></param>
        /// <param name="error"></param>
        /// <returns>MES收到文件生成请求，去对应文件夹下解析文件数据解析完成数据再写回文件响应位置，在将文件返回给设备上位机对应目录下</returns>
        public A042.Root A041(string FileTransmis, string FolderPath, string EquipmentID = "")
        {
            if (!_allowWork())
            {
                if (ATL.Common.StringResources.IsDefaultLanguage)
                {
                    LogInDB.Error("PC和MES通信中断");
                }
                else
                {
                    LogInDB.Error("PC soft MES interaction terminated，probable cause：pc soft start and config check failed、MES communication error");
                }
                return null;
            }
            int tryTimes = 0;
            A041.Root root = new A041.Root();
            string error = string.Empty;

            root.Header = new A041.Header();
            root.Header.EQCode = string.IsNullOrEmpty(EquipmentID) ? Station.Station.Current.EquipmentID : EquipmentID;
            root.Header.FunctionID = "A041";
            root.Header.PCName = Station.Station.Current.PCName;
            root.Header.RequestTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff");
            root.Header.SessionID = Guid.NewGuid().ToString();
            root.Header.SoftName = Station.Station.Current.SoftName;

            root.RequestInfo = new A041.RequestInfo();
            root.RequestInfo.FileTransmis = FileTransmis;
            root.RequestInfo.FolderPath = FolderPath;

            string jason = root.ToJSON();
            OfflineDataInfo offlineDataInfo = new OfflineDataInfo();
            offlineDataInfo.Guid = root.Header.SessionID;
            offlineDataInfo.Data = jason;
            offlineDataInfo.FunctionID = root.Header.FunctionID;
            A042_Buffer = new RecvdData();
            retry:
            tryTimes++;
            DateTime start = DateTime.Now;
            lock(A041LokObj)
            {
                if (Send("A041", offlineDataInfo.Data, out error))
                {
                    DAL.me.InsertMesInterfaceLog("A041", root.Header.SessionID,
                        DateTime.ParseExact(root.Header.RequestTime, "yyyy-MM-dd HH:mm:ss fff", System.Globalization.CultureInfo.CurrentCulture).ToString("yyyy-MM-dd HH:mm:ss.fff"),
                         null, jason, error);
                    while (true)
                    {
                        Thread.Sleep(1);
                        if ((DateTime.Now - start).TotalMilliseconds > ReceiveTimeOut)
                        {
                            if (!HeartBeatError && tryTimes < 3)
                                goto retry;

                            if (ATL.Common.StringResources.IsDefaultLanguage)
                            {
                                error = "MES响应A041超时";
                            }
                            else
                            {
                                error = "MES response A041 timeout";
                            }
                            LogInDB.Error(error);
                            DAL.me.UpdateMesInterfaceLogErrorMsg("A041", root.Header.SessionID, error);
                            return null;
                        }
                        try
                        {
                            if (A042_Buffer.jason != null && A042_Buffer.jason.Contains(offlineDataInfo.Guid))
                            {
                                A042.Root _root = JsonNewtonsoft.FromJSON<A042.Root>(A042_Buffer.jason);
                                return _root;
                            }
                        }
                        catch (Exception ex)
                        {
                            if (ATL.Common.StringResources.IsDefaultLanguage)
                            {
                                error = "解析 A042 Jason失败：" + ex.Message;
                            }
                            else
                            {
                                error = "Analysis Jaoson-A042 failure：" + ex.Message;
                            }
                            LogInDB.Error(error);
                            DAL.me.UpdateMesInterfaceLogErrorMsg("A042", root.Header.SessionID, error);
                            return null;
                        }
                    }
                }
                else
                {
                    LogInDB.Error(error);
                    DAL.me.InsertMesInterfaceLog("A041", root.Header.SessionID,
                        DateTime.ParseExact(root.Header.RequestTime, "yyyy-MM-dd HH:mm:ss fff", System.Globalization.CultureInfo.CurrentCulture).ToString("yyyy-MM-dd HH:mm:ss.fff"),
                         null, jason, error);
                    return null;
                }
            }
        }

        private object A043LokObj = new object();
        /// <summary>
        /// 设备向MES提供固定共享文件夹地址传送图片，传送完成向MES发送图片已生成请求
        /// </summary>
        /// <param name="PicTransmis"></param>
        /// <param name="FolderPath"></param>
        /// <param name="error"></param>
        /// <returns>MES收到图片生成请求，去对应文件夹下解析图片数据,解析完成数据，向设备返回对应电芯结果</returns>
        public A044.Root A043(string PicTransmis, string FolderPath, string EquipmentID = "")
        {
            if (!_allowWork())
            {
                if (ATL.Common.StringResources.IsDefaultLanguage)
                {
                    LogInDB.Error("PC和MES通信中断");
                }
                else
                {
                    LogInDB.Error("PC soft MES interaction terminated，probable cause：pc soft start and config check failed、MES communication error");
                }
                return null;
            }
            int tryTimes = 0;
            A043.Root root = new A043.Root();
            string error = string.Empty;

            root.Header = new A043.Header();
            root.Header.EQCode = string.IsNullOrEmpty(EquipmentID) ? Station.Station.Current.EquipmentID : EquipmentID;
            root.Header.FunctionID = "A043";
            root.Header.PCName = Station.Station.Current.PCName;
            root.Header.RequestTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff");
            root.Header.SessionID = Guid.NewGuid().ToString();
            root.Header.SoftName = Station.Station.Current.SoftName;

            root.RequestInfo = new A043.RequestInfo();
            root.RequestInfo.PicTransmis = PicTransmis;
            root.RequestInfo.FolderPath = FolderPath;

            string jason = root.ToJSON();
            OfflineDataInfo offlineDataInfo = new OfflineDataInfo();
            offlineDataInfo.Guid = root.Header.SessionID;
            offlineDataInfo.Data = jason;
            offlineDataInfo.FunctionID = root.Header.FunctionID;
            A044_Buffer = new RecvdData();
            retry:
            tryTimes++;
            DateTime start = DateTime.Now;
            lock(A043LokObj)
            {
                if (Send("A043", offlineDataInfo.Data, out error))
                {
                    DAL.me.InsertMesInterfaceLog("A043", root.Header.SessionID,
                        DateTime.ParseExact(root.Header.RequestTime, "yyyy-MM-dd HH:mm:ss fff", System.Globalization.CultureInfo.CurrentCulture).ToString("yyyy-MM-dd HH:mm:ss.fff"),
                         null, jason, error);
                    while (true)
                    {
                        Thread.Sleep(1);
                        if ((DateTime.Now - start).TotalMilliseconds > ReceiveTimeOut)
                        {
                            if (!HeartBeatError && tryTimes < 3)
                                goto retry;

                            if (ATL.Common.StringResources.IsDefaultLanguage)
                            {
                                error = "MES响应A043超时";
                            }
                            else
                            {
                                error = "MES response A043 timeout";
                            }
                            LogInDB.Error(error);
                            DAL.me.UpdateMesInterfaceLogErrorMsg("A043", root.Header.SessionID, error);
                            return null;
                        }
                        try
                        {
                            if (A044_Buffer.jason != null && A044_Buffer.jason.Contains(offlineDataInfo.Guid))
                            {
                                A044.Root _root = JsonNewtonsoft.FromJSON<A044.Root>(A044_Buffer.jason);
                                return _root;
                            }
                        }
                        catch (Exception ex)
                        {
                            if (ATL.Common.StringResources.IsDefaultLanguage)
                            {
                                error = "解析 A044 Jason失败：" + ex.Message;
                            }
                            else
                            {
                                error = "Analysis Jaoson-A044 failure：" + ex.Message;
                            }
                            LogInDB.Error(error);
                            DAL.me.UpdateMesInterfaceLogErrorMsg("A044", root.Header.SessionID, error);
                            return null;
                        }
                    }
                }
                else
                {
                    LogInDB.Error(error);
                    DAL.me.InsertMesInterfaceLog("A043", root.Header.SessionID,
                        DateTime.ParseExact(root.Header.RequestTime, "yyyy-MM-dd HH:mm:ss fff", System.Globalization.CultureInfo.CurrentCulture).ToString("yyyy-MM-dd HH:mm:ss.fff"),
                         null, jason, error);
                    return null;
                }
            }
        }

        /// <summary>
        /// 设备运行标准参数人工更改后需要向MES递交参数
        /// </summary>
        /// <returns></returns>
        public A046.Root A045(string EquipmentID = "")
        {
            int tryTimes = 0;
            A045.Root root = new A045.Root();
            string error = string.Empty;

            root.Header = new MES.A045.Header();
            root.Header.EQCode = string.IsNullOrEmpty(EquipmentID) ? Station.Station.Current.EquipmentID : EquipmentID;
            root.Header.FunctionID = "A045";
            root.Header.PCName = Station.Station.Current.PCName;
            root.Header.RequestTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff");
            root.Header.SessionID = Guid.NewGuid().ToString();
            root.Header.SoftName = Station.Station.Current.SoftName;

            List<A045.EquParamItem> lstChangeMonitorValue = new List<A045.EquParamItem>();
            lock (DeviceInputOutputConfigInfo.ChangeMonitorValueObj)
            {
                List<EquParamItem> lstEquParamItem = ATL.Core.EquParamItem.lstEquParamItem.Where(x => x.EquipmentID == root.Header.EQCode).ToList();
                if (lstEquParamItem.Count == 0)
                {
                    return null;
                }

                for (int i = 0; i < lstEquParamItem.Count; i++)
                {
                    A045.EquParamItem item = new MES.A045.EquParamItem();
                    item.NewParamValue = lstEquParamItem[i].NewParamValue;
                    item.OldParamValue = lstEquParamItem[i].OldParamValue;
                    item.ParamDesc = lstEquParamItem[i].ParamDesc;
                    item.ParamID = lstEquParamItem[i].ParamID;
                    lstChangeMonitorValue.Add(item);
                }
                ATL.Core.EquParamItem.lstEquParamItem.RemoveAll(x => x.EquipmentID == root.Header.EQCode);
            }

            root.RequestInfo = new MES.A045.RequestInfo();
            root.RequestInfo.EquParam = lstChangeMonitorValue;
            root.RequestInfo.UserInfo = new MES.A045.UserInfo();
            root.RequestInfo.UserInfo.UserID = UserInfo.UserID;
            root.RequestInfo.UserInfo.UserLevel = UserInfo.UserLevel;
            root.RequestInfo.UserInfo.UserName = UserInfo.UserName;

            string jason = root.ToJSON();
            OfflineDataInfo offlineDataInfo = new OfflineDataInfo();
            offlineDataInfo.Guid = root.Header.SessionID;
            offlineDataInfo.Data = jason;
            offlineDataInfo.FunctionID = root.Header.FunctionID;
            if (!_allowWork())
            {
                if (ATL.Common.StringResources.IsDefaultLanguage)
                {
                    LogInDB.Error("PC和MES通信中断");
                }
                else
                {
                    LogInDB.Error("PC soft MES interaction terminated，probable cause：pc soft start and config check failed、MES communication error");
                }                //DAL.me.InsertToMESofflineBuffer(root.Header.EQCode, "A045", root.Header.SessionID, jason);
                return null;
            }
            A046_Buffer = new RecvdData();
            retry:
            tryTimes++;
            DateTime start = DateTime.Now;
            if (Send("A045", offlineDataInfo.Data, out error))
            {
                DAL.me.InsertMesInterfaceLog("A045", root.Header.SessionID,
                    DateTime.ParseExact(root.Header.RequestTime, "yyyy-MM-dd HH:mm:ss fff", System.Globalization.CultureInfo.CurrentCulture).ToString("yyyy-MM-dd HH:mm:ss.fff"),
                     null, jason, error);
                while (true)
                {
                    Thread.Sleep(1);
                    if ((DateTime.Now - start).TotalMilliseconds > ReceiveTimeOut)
                    {
                        if (!HeartBeatError && tryTimes < 3)
                            goto retry;

                        if (ATL.Common.StringResources.IsDefaultLanguage)
                        {
                            error = "MES响应A045超时";
                        }
                        else
                        {
                            error = "MES response A045 timeout";
                        }
                        LogInDB.Error(error);
                        DAL.me.UpdateMesInterfaceLogErrorMsg("A045", root.Header.SessionID, error);
                        //DAL.me.InsertToMESofflineBuffer(root.Header.EQCode, "A045", root.Header.SessionID, jason);
                        return null;
                    }
                    try
                    {
                        if (A046_Buffer.jason != null && A046_Buffer.jason.Contains(offlineDataInfo.Guid))
                        {
                            A046.Root _root = JsonNewtonsoft.FromJSON<A046.Root>(A046_Buffer.jason);
                            return _root;
                        }
                    }
                    catch (Exception ex)
                    {
                        if (ATL.Common.StringResources.IsDefaultLanguage)
                        {
                            error = "解析 A046 Jason失败：" + ex.Message;
                        }
                        else
                        {
                            error = "Analysis Jaoson-A046 failure：" + ex.Message;
                        }
                        LogInDB.Error(error);
                        DAL.me.UpdateMesInterfaceLogErrorMsg("A046", root.Header.SessionID, error);
                        return null;
                    }
                }
            }
            else
            {
                LogInDB.Error(error);
                //DAL.me.InsertToMESofflineBuffer(root.Header.EQCode, "A045", root.Header.SessionID, jason);
                DAL.me.InsertMesInterfaceLog("A045", root.Header.SessionID,
                    DateTime.ParseExact(root.Header.RequestTime, "yyyy-MM-dd HH:mm:ss fff", System.Globalization.CultureInfo.CurrentCulture).ToString("yyyy-MM-dd HH:mm:ss.fff"),
                     null, jason, error);
                return null;
            }
        }

        /// <summary>
        /// 上物料或更换物料时将物料信息发送至设备；之后设备进行响应
        /// </summary>
        /// <param name="msg"></param>
        public void A048(object obj)
        {
            A047.Root msg = (A047.Root)obj;
            if (Client == null || !MESconnected || HeartBeatError) return;
            A048.Root root = new A048.Root();
            string error = string.Empty;

            root.Header = new A048.Header();
            root.Header.EQCode = msg.Header.EQCode;
            root.Header.FunctionID = "A048";
            root.Header.PCName = Station.Station.Current.PCName;
            root.Header.RequestTime = msg.Header.RequestTime;
            root.Header.SessionID = msg.Header.SessionID;
            root.Header.SoftName = Station.Station.Current.SoftName;
            root.Header.ErrorCode = "00";
            root.Header.ErrorMsg = "Null";
            root.Header.IsSuccess = "True";
            root.Header.ResponseTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff");
            root.ResponseInfo = new A048.ResponseInfo();
            root.ResponseInfo.Result = "OK";
            string jason = root.ToJSON();
            DAL.me.InsertMesInterfaceLog("A048", msg.Header.SessionID, null, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), jason, "");
            Send("A048", jason, out error);
        }

        public class A047EventArgs : EventArgs
        {
            public A047.Root root { get; set; }
        }
        public static event EventHandler<A047EventArgs> A047event;

        /// <summary>
        /// 设备在MES宕机后，进行清尾生产时产生的电芯码及物料号进行绑定
        /// </summary>
        /// <param name="RequestInfo"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public A050.Root A049(List<string> lstBarcode, string EquipmentID = "")
        {
            if (!_allowWork())
            {
                if (ATL.Common.StringResources.IsDefaultLanguage)
                {
                    LogInDB.Error("PC和MES通信中断");
                }
                else
                {
                    LogInDB.Error("PC soft MES interaction terminated，probable cause：pc soft start and config check failed、MES communication error");
                }
                return null;
            }
            int tryTimes = 0;
            A049.Root root = new A049.Root();
            string error = string.Empty;

            root.Header = new MES.A049.Header();
            root.Header.EQCode = string.IsNullOrEmpty(EquipmentID) ? Station.Station.Current.EquipmentID : EquipmentID;
            root.Header.FunctionID = "A049";
            root.Header.PCName = Station.Station.Current.PCName;
            root.Header.RequestTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff");
            root.Header.SessionID = Guid.NewGuid().ToString();
            root.Header.SoftName = Station.Station.Current.SoftName;

            A049.RequestInfo RequestInfo = new MES.A049.RequestInfo();
            RequestInfo.UserInfo = new MES.A049.UserInfo();
            A047RequestInfo a047RequestInfo = lstA047RequestFromFile.Where(x => x.EquipmentID == root.Header.EQCode).FirstOrDefault();
            if (a047RequestInfo != null)
            {
                if (a047RequestInfo.UserInfo != null)
                {
                    RequestInfo.UserInfo.UserID = a047RequestInfo.UserInfo.UserID;
                    RequestInfo.UserInfo.UserLevel = a047RequestInfo.UserInfo.UserLevel;
                    RequestInfo.UserInfo.UserName = a047RequestInfo.UserInfo.UserName;
                }
                RequestInfo.ModelInfo = a047RequestInfo.ModelInfo;
                RequestInfo.MaterialInfo = new List<MES.A049.MaterialInfoItem>();
                if(a047RequestInfo.MaterialInfo != null && a047RequestInfo.MaterialInfo.Count > 0)
                {
                    foreach (var v in a047RequestInfo.MaterialInfo)
                    {
                        A049.MaterialInfoItem item = new MES.A049.MaterialInfoItem()
                        {
                            LabelNumber = v.LabelNumber,
                            MaterialID = v.MaterialID,
                            MaterialName = v.MaterialName,
                            MaterialQuantity = v.MaterialQuality,
                            UoM = v.UoM
                        };
                        RequestInfo.MaterialInfo.Add(item);
                    }
                }
            }
            else
            {
                lock (LocklstA047RequestFromMESObj)
                {
                    a047RequestInfo = lstA047RequestFromMES.Where(x => x.EquipmentID == root.Header.EQCode).FirstOrDefault();
                }
                if (a047RequestInfo != null)
                {
                    if (a047RequestInfo.UserInfo != null)
                    {
                        RequestInfo.UserInfo = JsonNewtonsoft.FromJSON<MES.A049.UserInfo>(a047RequestInfo.UserInfo.ToJSON());
                    }
                    RequestInfo.ModelInfo = a047RequestInfo.ModelInfo;
                    RequestInfo.MaterialInfo = new List<MES.A049.MaterialInfoItem>();
                    if (a047RequestInfo.MaterialInfo != null && a047RequestInfo.MaterialInfo.Count > 0)
                    {
                        foreach (var v in a047RequestInfo.MaterialInfo)
                        {
                            A049.MaterialInfoItem item = new MES.A049.MaterialInfoItem()
                            {
                                LabelNumber = v.LabelNumber,
                                MaterialID = v.MaterialID,
                                MaterialName = v.MaterialName,
                                MaterialQuantity = v.MaterialQuality,
                                UoM = v.UoM
                            };
                            RequestInfo.MaterialInfo.Add(item);
                        }
                    }
                }
            }
            RequestInfo.ProductSN = lstBarcode;
            root.RequestInfo = RequestInfo;

            string jason = root.ToJSON();
            OfflineDataInfo offlineDataInfo = new OfflineDataInfo();
            offlineDataInfo.Guid = root.Header.SessionID;
            offlineDataInfo.Data = jason;
            offlineDataInfo.FunctionID = root.Header.FunctionID;
            A050_Buffer = new RecvdData();
            retry:
            tryTimes++;
            DateTime start = DateTime.Now;
            if (Send("A049", offlineDataInfo.Data, out error))
            {
                DAL.me.InsertMesInterfaceLog("A049", root.Header.SessionID,
                    DateTime.ParseExact(root.Header.RequestTime, "yyyy-MM-dd HH:mm:ss fff", System.Globalization.CultureInfo.CurrentCulture).ToString("yyyy-MM-dd HH:mm:ss.fff"),
                     null, jason, error);
                while (true)
                {
                    Thread.Sleep(1);
                    if ((DateTime.Now - start).TotalMilliseconds > ReceiveTimeOut)
                    {
                        if (!HeartBeatError && tryTimes < 3)
                            goto retry;

                        if (ATL.Common.StringResources.IsDefaultLanguage)
                        {
                            error = "MES响应A049超时";
                        }
                        else
                        {
                            error = "MES response A049 timeout";
                        }
                        LogInDB.Error(error);
                        DAL.me.UpdateMesInterfaceLogErrorMsg("A049", root.Header.SessionID, error);
                        return null;
                    }
                    try
                    {
                        if (A050_Buffer.jason != null && A050_Buffer.jason.Contains(offlineDataInfo.Guid))
                        {
                            A050.Root _root = JsonNewtonsoft.FromJSON<A050.Root>(A050_Buffer.jason);
                            return _root;
                        }
                    }
                    catch (Exception ex)
                    {
                        if (ATL.Common.StringResources.IsDefaultLanguage)
                        {
                            error = "解析 A050 Jason失败：" + ex.Message;
                        }
                        else
                        {
                            error = "Analysis Jaoson-A050 failure：" + ex.Message;
                        }
                        LogInDB.Error(error);
                        DAL.me.UpdateMesInterfaceLogErrorMsg("A050", root.Header.SessionID, error);
                        return null;
                    }
                }
            }
            else
            {
                LogInDB.Error(error);
                DAL.me.InsertMesInterfaceLog("A049", root.Header.SessionID,
                    DateTime.ParseExact(root.Header.RequestTime, "yyyy-MM-dd HH:mm:ss fff", System.Globalization.CultureInfo.CurrentCulture).ToString("yyyy-MM-dd HH:mm:ss.fff"),
                     null, jason, error);
                return null;
            }
        }

        private object A051LokObj = new object();
        /// <summary>
        /// ACT上传整炉电芯号
        /// </summary>
        /// <param name="lstCell"></param>
        /// <param name="error"></param>
        /// <returns>MES通过第一个电芯号，根据高低组属性、正常或者复测，获取响应的测试流程路径，并反馈给设备</returns>
        public A052.Root A051(List<string> lstCell, string EquipmentID = "")
        {
            int tryTimes = 0;
            A051.Root root = new A051.Root();
            string error = string.Empty;

            root.Header = new MES.A051.Header();
            root.Header.EQCode = string.IsNullOrEmpty(EquipmentID) ? Station.Station.Current.EquipmentID : EquipmentID;
            root.Header.FunctionID = "A051";
            root.Header.PCName = Station.Station.Current.PCName;
            root.Header.RequestTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff");
            root.Header.SessionID = Guid.NewGuid().ToString();
            root.Header.SoftName = Station.Station.Current.SoftName;

            root.RequestInfo = new A051.RequestInfo();
            root.RequestInfo.Cell = lstCell;

            string jason = root.ToJSON();
            OfflineDataInfo offlineDataInfo = new OfflineDataInfo();
            offlineDataInfo.Guid = root.Header.SessionID;
            offlineDataInfo.Data = jason;
            offlineDataInfo.FunctionID = root.Header.FunctionID;
            if (!_allowWork())
            {
                if (ATL.Common.StringResources.IsDefaultLanguage)
                {
                    LogInDB.Error("PC和MES通信中断");
                }
                else
                {
                    LogInDB.Error("PC soft MES interaction terminated，probable cause：pc soft start and config check failed、MES communication error");
                }                //DAL.me.InsertToMESofflineBuffer(root.Header.EQCode, "A051", root.Header.SessionID, jason);
                return null;
            }
            A052_Buffer = new RecvdData();
            retry:
            tryTimes++;
            DateTime start = DateTime.Now;
            lock(A051LokObj)
            {
                if (Send("A051", offlineDataInfo.Data, out error))
                {
                    DAL.me.InsertMesInterfaceLog("A051", root.Header.SessionID,
                        DateTime.ParseExact(root.Header.RequestTime, "yyyy-MM-dd HH:mm:ss fff", System.Globalization.CultureInfo.CurrentCulture).ToString("yyyy-MM-dd HH:mm:ss.fff"),
                         null, jason, error);
                    while (true)
                    {
                        Thread.Sleep(1);
                        if ((DateTime.Now - start).TotalMilliseconds > ReceiveTimeOut)
                        {
                            if (!HeartBeatError && tryTimes < 3)
                                goto retry;

                            if (ATL.Common.StringResources.IsDefaultLanguage)
                            {
                                error = "MES响应A051超时";
                            }
                            else
                            {
                                error = "MES response A051 timeout";
                            }
                            LogInDB.Error(error);
                            DAL.me.UpdateMesInterfaceLogErrorMsg("A051", root.Header.SessionID, error);
                            //DAL.me.InsertToMESofflineBuffer(root.Header.EQCode, "A051", root.Header.SessionID, jason);
                            MesErrorStopPLC(root.Header.EQCode);
                            return null;
                        }
                        try
                        {
                            if (A052_Buffer.jason != null && A052_Buffer.jason.Contains(offlineDataInfo.Guid))
                            {
                                A052.Root _root = JsonNewtonsoft.FromJSON<A052.Root>(A052_Buffer.jason);
                                return _root;
                            }
                        }
                        catch (Exception ex)
                        {
                            if (ATL.Common.StringResources.IsDefaultLanguage)
                            {
                                error = "解析 A052 Jason失败：" + ex.Message;
                            }
                            else
                            {
                                error = "Analysis Jaoson-A052 failure：" + ex.Message;
                            }
                            LogInDB.Error(error);
                            DAL.me.UpdateMesInterfaceLogErrorMsg("A052", root.Header.SessionID, error);
                            return null;
                        }
                    }
                }
                else
                {
                    LogInDB.Error(error);
                    //DAL.me.InsertToMESofflineBuffer(root.Header.EQCode, "A051", root.Header.SessionID, jason);
                    DAL.me.InsertMesInterfaceLog("A051", root.Header.SessionID,
                        DateTime.ParseExact(root.Header.RequestTime, "yyyy-MM-dd HH:mm:ss fff", System.Globalization.CultureInfo.CurrentCulture).ToString("yyyy-MM-dd HH:mm:ss.fff"),
                         null, jason, error);
                    return null;
                }
            }
        }

        /// <summary>
        /// ACT反馈
        /// </summary>
        /// <param name="msg"></param>
        public void A054(A053.Root msg)
        {
            if (Client == null || !MESconnected || HeartBeatError) return;
            A054.Root root = new A054.Root();
            string error = string.Empty;

            root.Header = new A054.Header();
            root.Header.EQCode = msg.Header.EQCode;
            root.Header.FunctionID = "A054";
            root.Header.PCName = Station.Station.Current.PCName;
            root.Header.RequestTime = msg.Header.RequestTime;
            root.Header.SessionID = msg.Header.SessionID;
            root.Header.SoftName = Station.Station.Current.SoftName;
            root.Header.ErrorCode = "00";
            root.Header.ErrorMsg = "Null";
            root.Header.IsSuccess = "True";
            root.Header.ResponseTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff");
            root.ResponseInfo = new A054.ResponseInfo();
            root.ResponseInfo.Result = "OK";
            string jason = root.ToJSON();
            DAL.me.InsertMesInterfaceLog("A054", msg.Header.SessionID, null, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), jason, "");
            Send("A054", jason, out error);
        }
        public class A053EventArgs : EventArgs
        {
            public A053.Root root { get; set; }
        }
        public static event EventHandler<A053EventArgs> A053event;

        private object A055LokObj = new object();
        /// <summary>
        /// 因Freebaking与真空baking无法获取电芯码，故使用此结构的OUTPUT数据
        /// </summary>
        /// <param name="lst"></param>
        /// <param name="EquipmentID"></param>
        /// <returns></returns>
        public A056.Root A055(List<A055.ParamInfoItem> lst, string EquipmentID = "")
        {
            DateTime startTime = DateTime.Now;
            int tryTimes = 0;
            A055.Root root = new A055.Root();
            string error = string.Empty;

            root.Header = new A055.Header();
            root.Header.EQCode = string.IsNullOrEmpty(EquipmentID) ? Station.Station.Current.EquipmentID : EquipmentID;
            root.Header.FunctionID = "A055";
            root.Header.PCName = Station.Station.Current.PCName;
            root.Header.RequestTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff");
            root.Header.SessionID = Guid.NewGuid().ToString();
            root.Header.SoftName = Station.Station.Current.SoftName;

            root.RequestInfo = new A055.RequestInfo();
            root.RequestInfo.ParamInfo = lst;

            string jason = root.ToJSON();
            OfflineDataInfo offlineDataInfo = new OfflineDataInfo();
            offlineDataInfo.Guid = root.Header.SessionID;
            offlineDataInfo.Data = jason;
            offlineDataInfo.FunctionID = root.Header.FunctionID;
            if (!_allowWork())
            {
                if (ATL.Common.StringResources.IsDefaultLanguage)
                {
                    LogInDB.Error("PC和MES通信中断");
                }
                else
                {
                    LogInDB.Error("PC soft MES interaction terminated，probable cause：pc soft start and config check failed、MES communication error");
                }                //DAL.me.InsertToMESofflineBuffer(root.Header.EQCode, "A055", root.Header.SessionID, jason);
                return null;
            }
            A056_Buffer = new RecvdData();
            retry:
            tryTimes++;
            DateTime start = DateTime.Now;
            lock(A055LokObj)
            {
                if (Send("A055", offlineDataInfo.Data, out error))
                {
                    DAL.me.InsertMesInterfaceLog("A055", root.Header.SessionID,
                        DateTime.ParseExact(root.Header.RequestTime, "yyyy-MM-dd HH:mm:ss fff", System.Globalization.CultureInfo.CurrentCulture).ToString("yyyy-MM-dd HH:mm:ss.fff"),
                         null, jason, error);
                    while (true)
                    {
                        Thread.Sleep(1);
                        if ((DateTime.Now - start).TotalMilliseconds > ReceiveTimeOut)
                        {
                            if (!HeartBeatError && tryTimes < 3)
                                goto retry;

                            if (ATL.Common.StringResources.IsDefaultLanguage)
                            {
                                error = "MES响应A055超时";
                            }
                            else
                            {
                                error = "MES response A055 timeout";
                            }
                            LogInDB.Error(error);
                            DAL.me.UpdateMesInterfaceLogErrorMsg("A055", root.Header.SessionID, error);
                            //DAL.me.InsertToMESofflineBuffer(root.Header.EQCode, "A055", root.Header.SessionID, jason);
                            MesErrorStopPLC(root.Header.EQCode);
                            return null;
                        }
                        try
                        {
                            if (A056_Buffer.jason != null && A056_Buffer.jason.Contains(offlineDataInfo.Guid))
                            {
                                A056.Root _root = JsonNewtonsoft.FromJSON<A056.Root>(A056_Buffer.jason);
                                return _root;
                            }
                        }
                        catch (Exception ex)
                        {
                            if (ATL.Common.StringResources.IsDefaultLanguage)
                            {
                                error = "解析 A056 Jason失败：" + ex.Message;
                            }
                            else
                            {
                                error = "Analysis Jaoson-A056 failure：" + ex.Message;
                            }
                            LogInDB.Error(error);
                            DAL.me.UpdateMesInterfaceLogErrorMsg("A056", root.Header.SessionID, error);
                            return null;
                        }
                    }
                }
                else
                {
                    LogInDB.Error(error);
                    //DAL.me.InsertToMESofflineBuffer(root.Header.EQCode, "A055", root.Header.SessionID, jason);
                    DAL.me.InsertMesInterfaceLog("A055", root.Header.SessionID,
                        DateTime.ParseExact(root.Header.RequestTime, "yyyy-MM-dd HH:mm:ss fff", System.Globalization.CultureInfo.CurrentCulture).ToString("yyyy-MM-dd HH:mm:ss.fff"),
                         null, jason, error);
                    return null;
                }
            }
        }

        public A056.Root A055_MTW20(string ChildEquipment, List<A055_MTW20.ParamInfoItem> lst, string EquipmentID = "")
        {
            DateTime startTime = DateTime.Now;
            int tryTimes = 0;
            A055_MTW20.Root root = new A055_MTW20.Root();
            string error = string.Empty;

            root.Header = new A055_MTW20.Header();
            root.Header.EQCode = string.IsNullOrEmpty(EquipmentID) ? Station.Station.Current.EquipmentID : EquipmentID;
            root.Header.FunctionID = "A055";
            root.Header.PCName = Station.Station.Current.PCName;
            root.Header.RequestTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff");
            root.Header.SessionID = Guid.NewGuid().ToString();
            root.Header.SoftName = Station.Station.Current.SoftName;

            root.RequestInfo = new A055_MTW20.RequestInfo();
            root.RequestInfo.ChildEquipment = ChildEquipment;
            root.RequestInfo.ParamInfo = lst;

            string jason = root.ToJSON();
            OfflineDataInfo offlineDataInfo = new OfflineDataInfo();
            offlineDataInfo.Guid = root.Header.SessionID;
            offlineDataInfo.Data = jason;
            offlineDataInfo.FunctionID = root.Header.FunctionID;
            if (!_allowWork())
            {
                if (ATL.Common.StringResources.IsDefaultLanguage)
                {
                    LogInDB.Error("PC和MES通信中断");
                }
                else
                {
                    LogInDB.Error("PC soft MES interaction terminated，probable cause：pc soft start and config check failed、MES communication error");
                }                //DAL.me.InsertToMESofflineBuffer(root.Header.EQCode, "A055", root.Header.SessionID, jason);
                return null;
            }
            A056_Buffer = new RecvdData();
            retry:
            tryTimes++;
            DateTime start = DateTime.Now;
            lock(A055LokObj)
            {
                if (Send("A055", offlineDataInfo.Data, out error))
                {
                    DAL.me.InsertMesInterfaceLog("A055", root.Header.SessionID,
                        DateTime.ParseExact(root.Header.RequestTime, "yyyy-MM-dd HH:mm:ss fff", System.Globalization.CultureInfo.CurrentCulture).ToString("yyyy-MM-dd HH:mm:ss.fff"),
                         null, jason, error);
                    while (true)
                    {
                        Thread.Sleep(1);
                        if ((DateTime.Now - start).TotalMilliseconds > ReceiveTimeOut)
                        {
                            if (!HeartBeatError && tryTimes < 3)
                                goto retry;
                            if (ATL.Common.StringResources.IsDefaultLanguage)
                            {
                                error = "MES响应A055超时";
                            }
                            else
                            {
                                error = "MES response A055 timeout";
                            }
                            LogInDB.Error(error);
                            DAL.me.UpdateMesInterfaceLogErrorMsg("A055", root.Header.SessionID, error);
                            //DAL.me.InsertToMESofflineBuffer(root.Header.EQCode, "A055", root.Header.SessionID, jason);
                            MesErrorStopPLC(root.Header.EQCode);
                            return null;
                        }
                        try
                        {
                            if (A056_Buffer.jason != null && A056_Buffer.jason.Contains(offlineDataInfo.Guid))
                            {
                                A056.Root _root = JsonNewtonsoft.FromJSON<A056.Root>(A056_Buffer.jason);
                                return _root;
                            }
                        }
                        catch (Exception ex)
                        {
                            if (ATL.Common.StringResources.IsDefaultLanguage)
                            {
                                error = "解析 A056 Jason失败：" + ex.Message;
                            }
                            else
                            {
                                error = "Analysis Jaoson-A056 failure：" + ex.Message;
                            }
                            LogInDB.Error(error);
                            DAL.me.UpdateMesInterfaceLogErrorMsg("A056", root.Header.SessionID, error);
                            return null;
                        }
                    }
                }
                else
                {
                    LogInDB.Error(error);
                    //DAL.me.InsertToMESofflineBuffer(root.Header.EQCode, "A055", root.Header.SessionID, jason);
                    DAL.me.InsertMesInterfaceLog("A055", root.Header.SessionID,
                        DateTime.ParseExact(root.Header.RequestTime, "yyyy-MM-dd HH:mm:ss fff", System.Globalization.CultureInfo.CurrentCulture).ToString("yyyy-MM-dd HH:mm:ss.fff"),
                         null, jason, error);
                    return null;
                }
            }
        }

        /// <summary>
        /// MES下发地址配置
        /// </summary>
        /// <param name="msg"></param>
        public void A061(object obj)
        {
            A060.Root msg = (A060.Root)obj;
            if (Client == null || !MESconnected || HeartBeatError) return;
            A048.Root root = new A048.Root();
            string error = string.Empty;

            root.Header = new A048.Header();
            root.Header.EQCode = msg.Header.EQCode;
            root.Header.FunctionID = "A061";
            root.Header.PCName = Station.Station.Current.PCName;
            root.Header.RequestTime = msg.Header.RequestTime;
            root.Header.SessionID = msg.Header.SessionID;
            root.Header.SoftName = Station.Station.Current.SoftName;
            root.Header.ErrorCode = "00";
            root.Header.ErrorMsg = "Null";
            root.Header.IsSuccess = "True";
            root.Header.ResponseTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff");
            root.ResponseInfo = new A048.ResponseInfo();
            root.ResponseInfo.Result = "OK";
            string jason = root.ToJSON();
            DAL.me.InsertMesInterfaceLog("A061", msg.Header.SessionID, null, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), jason, "");
            Send("A061", jason, out error);
        }
        public A063.Root A062(List<A062.MaterialInfoItem> lst, string EquipmentID = "")
        {
            int tryTimes = 0;
            A062.Root root = new A062.Root();
            string error = string.Empty;

            root.Header = new MES.A062.Header();
            root.Header.EQCode = string.IsNullOrEmpty(EquipmentID) ? Station.Station.Current.EquipmentID : EquipmentID;
            root.Header.FunctionID = "A062";
            root.Header.PCName = Station.Station.Current.PCName;
            root.Header.RequestTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff");
            root.Header.SessionID = Guid.NewGuid().ToString();
            root.Header.SoftName = Station.Station.Current.SoftName;

            root.RequestInfo = new A062.RequestInfo();
            root.RequestInfo.MaterialInfo = lst;

            string jason = root.ToJSON();
            OfflineDataInfo offlineDataInfo = new OfflineDataInfo();
            offlineDataInfo.Guid = root.Header.SessionID;
            offlineDataInfo.Data = jason;
            offlineDataInfo.FunctionID = root.Header.FunctionID;
            if (!_allowWork())
            {
                if (ATL.Common.StringResources.IsDefaultLanguage)
                {
                    LogInDB.Error("PC和MES通信中断");
                }
                else
                {
                    LogInDB.Error("PC soft MES interaction terminated，probable cause：pc soft start and config check failed、MES communication error");
                }                //DAL.me.InsertToMESofflineBuffer(root.Header.EQCode, "A062", root.Header.SessionID, jason);
                return null;
            }
            A063_Buffer = new RecvdData();
            retry:
            tryTimes++;
            DateTime start = DateTime.Now;
            if (Send("A062", offlineDataInfo.Data, out error))
            {
                DAL.me.InsertMesInterfaceLog("A062", root.Header.SessionID,
                    DateTime.ParseExact(root.Header.RequestTime, "yyyy-MM-dd HH:mm:ss fff", System.Globalization.CultureInfo.CurrentCulture).ToString("yyyy-MM-dd HH:mm:ss.fff"),
                     null, jason, error);
                while (true)
                {
                    Thread.Sleep(1);
                    if ((DateTime.Now - start).TotalMilliseconds > ReceiveTimeOut)
                    {
                        if (!HeartBeatError && tryTimes < 3)
                            goto retry;
                        if (ATL.Common.StringResources.IsDefaultLanguage)
                        {
                            error = "MES响应A061超时";
                        }
                        else
                        {
                            error = "MES response A061 timeout";
                        }
                        LogInDB.Error(error);
                        DAL.me.UpdateMesInterfaceLogErrorMsg("A062", root.Header.SessionID, error);
                        //DAL.me.InsertToMESofflineBuffer(root.Header.EQCode, "A062", root.Header.SessionID, jason);
                        MesErrorStopPLC(root.Header.EQCode);
                        return null;
                    }
                    try
                    {
                        if (A063_Buffer.jason != null && A063_Buffer.jason.Contains(offlineDataInfo.Guid))
                        {
                            A063.Root _root = JsonNewtonsoft.FromJSON<A063.Root>(A063_Buffer.jason);
                            return _root;
                        }
                    }
                    catch (Exception ex)
                    {
                        if (ATL.Common.StringResources.IsDefaultLanguage)
                        {
                            error = "解析 A063 Jason失败：" + ex.Message;
                        }
                        else
                        {
                            error = "Analysis Jaoson-A063 failure：" + ex.Message;
                        }
                        LogInDB.Error(error);
                        DAL.me.UpdateMesInterfaceLogErrorMsg("A063", root.Header.SessionID, error);
                        return null;
                    }
                }
            }
            else
            {
                LogInDB.Error(error);
                //DAL.me.InsertToMESofflineBuffer(root.Header.EQCode, "A062", root.Header.SessionID, jason);
                DAL.me.InsertMesInterfaceLog("A062", root.Header.SessionID,
                    DateTime.ParseExact(root.Header.RequestTime, "yyyy-MM-dd HH:mm:ss fff", System.Globalization.CultureInfo.CurrentCulture).ToString("yyyy-MM-dd HH:mm:ss.fff"),
                     null, jason, error);
                return null;
            }
        }

        private object A064LokObj = new object();
        public A065.Root A064(string Type,List<A064.ProductsItem> lst, string EquipmentID = "")
        {
            int tryTimes = 0;
            A064.Root root = new A064.Root();
            string error = string.Empty;

            root.Header = new MES.A064.Header();
            root.Header.EQCode = string.IsNullOrEmpty(EquipmentID) ? Station.Station.Current.EquipmentID : EquipmentID;
            root.Header.FunctionID = "A064";
            root.Header.PCName = Station.Station.Current.PCName;
            root.Header.RequestTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff");
            root.Header.SessionID = Guid.NewGuid().ToString();
            root.Header.SoftName = Station.Station.Current.SoftName;

            root.RequestInfo = new A064.RequestInfo();
            root.RequestInfo.Type = Type;
            root.RequestInfo.Products = new List<MES.A064.ProductsItem>();
            List<string> lstBarcode = new List<string>();
            foreach (var Item in lst)
            {
                A064.ProductsItem ProductsItem = new A064.ProductsItem();
                ProductsItem.ProductSN1 = Item.ProductSN1;
                lstBarcode.Add(Item.ProductSN1);
                ProductsItem.ProductSN2 = Item.ProductSN2;
                ProductsItem.Station = Item.Station;
                ProductsItem.Pass = Item.Pass;
                ProductsItem.ChildEquCode = Item.ChildEquCode;

                ProductsItem.OutputParam = Item.OutputParam;
                root.RequestInfo.Products.Add(ProductsItem);
            }
            string jason = root.ToJSON();
            OfflineDataInfo offlineDataInfo = new OfflineDataInfo();
            offlineDataInfo.Guid = root.Header.SessionID;
            offlineDataInfo.Data = jason;
            offlineDataInfo.FunctionID = root.Header.FunctionID;
            if (!_allowWork())
            {
                if (ATL.Common.StringResources.IsDefaultLanguage)
                {
                    LogInDB.Error("PC和MES通信中断");
                }
                else
                {
                    LogInDB.Error("PC soft MES interaction terminated，probable cause：pc soft start and config check failed、MES communication error");
                }                //DAL.me.InsertToMESofflineBuffer(root.Header.EQCode, "A064", root.Header.SessionID, jason, lstBarcode);
                return null;
            }
            A065_Buffer = new RecvdData();
            retry:
            tryTimes++;
            DateTime start = DateTime.Now;
            lock(A064LokObj)
            {

                if (Send("A064", offlineDataInfo.Data, out error))
                {
                    DAL.me.InsertMesInterfaceLog("A064", root.Header.SessionID,
                        DateTime.ParseExact(root.Header.RequestTime, "yyyy-MM-dd HH:mm:ss fff", System.Globalization.CultureInfo.CurrentCulture).ToString("yyyy-MM-dd HH:mm:ss.fff"),
                         null, jason, error);
                    while (true)
                    {
                        Thread.Sleep(1);
                        if ((DateTime.Now - start).TotalMilliseconds > ReceiveTimeOut)
                        {
                            if (!HeartBeatError && tryTimes < 3)
                                goto retry;
                            if (ATL.Common.StringResources.IsDefaultLanguage)
                            {
                                error = "MES响应A063超时";
                            }
                            else
                            {
                                error = "MES response A063 timeout";
                            }
                            LogInDB.Error(error);
                            DAL.me.UpdateMesInterfaceLogErrorMsg("A064", root.Header.SessionID, error);
                            //DAL.me.InsertToMESofflineBuffer(root.Header.EQCode, "A064", root.Header.SessionID, jason, lstBarcode);
                            MesErrorStopPLC(root.Header.EQCode);
                            return null;
                        }
                        try
                        {
                            if (A065_Buffer.jason != null && A065_Buffer.jason.Contains(offlineDataInfo.Guid))
                            {
                                A065.Root _root = JsonNewtonsoft.FromJSON<A065.Root>(A065_Buffer.jason);
                                return _root;
                            }
                        }
                        catch (Exception ex)
                        {
                            if (ATL.Common.StringResources.IsDefaultLanguage)
                            {
                                error = "解析 A065 Jason失败：" + ex.Message;
                            }
                            else
                            {
                                error = "Analysis Jaoson-A065 failure：" + ex.Message;
                            }
                            LogInDB.Error(error);
                            DAL.me.UpdateMesInterfaceLogErrorMsg("A065", root.Header.SessionID, error);
                            return null;
                        }
                    }
                }
                else
                {
                    LogInDB.Error(error);
                    //DAL.me.InsertToMESofflineBuffer(root.Header.EQCode, "A064", root.Header.SessionID, jason, lstBarcode);
                    DAL.me.InsertMesInterfaceLog("A064", root.Header.SessionID,
                        DateTime.ParseExact(root.Header.RequestTime, "yyyy-MM-dd HH:mm:ss fff", System.Globalization.CultureInfo.CurrentCulture).ToString("yyyy-MM-dd HH:mm:ss.fff"),
                         null, jason, error);
                    return null;
                }
            }
        }

        private object A067LokObj = new object();
        public A068.Root A067(string Type, List<A067.ProductsItem> Products, string EquipmentID = "")
        {
            int tryTimes = 0;
            A067.Root root = new A067.Root();
            string error = string.Empty;

            root.Header = new MES.A067.Header();
            root.Header.EQCode = string.IsNullOrEmpty(EquipmentID) ? Station.Station.Current.EquipmentID : EquipmentID;
            root.Header.FunctionID = "A067";
            root.Header.PCName = Station.Station.Current.PCName;
            root.Header.RequestTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff");
            root.Header.SessionID = Guid.NewGuid().ToString();
            root.Header.SoftName = Station.Station.Current.SoftName;

            root.RequestInfo = new A067.RequestInfo();
            root.RequestInfo.Type = Type;
            root.RequestInfo.Products = Products;

            string jason = root.ToJSON();
            OfflineDataInfo offlineDataInfo = new OfflineDataInfo();
            offlineDataInfo.Guid = root.Header.SessionID;
            offlineDataInfo.Data = jason;
            offlineDataInfo.FunctionID = root.Header.FunctionID;
            List<string> lstBarcode = new List<string>();
            foreach (var item in Products)
            {
                lstBarcode.Add(item.ProductSN1);
            }

            if (!_allowWork())
            {
                if (ATL.Common.StringResources.IsDefaultLanguage)
                {
                    LogInDB.Error("PC和MES通信中断");
                }
                else
                {
                    LogInDB.Error("PC soft MES interaction terminated，probable cause：pc soft start and config check failed、MES communication error");
                }                //DAL.me.InsertToMESofflineBuffer(root.Header.EQCode, "A067", root.Header.SessionID, jason, lstBarcode);
                return null;
            }
            A068_Buffer = new RecvdData();
            retry:
            tryTimes++;
            DateTime start = DateTime.Now;
            lock(A067LokObj)
            {
                if (Send("A067", offlineDataInfo.Data, out error))
                {
                    DAL.me.InsertMesInterfaceLog("A067", root.Header.SessionID,
                        DateTime.ParseExact(root.Header.RequestTime, "yyyy-MM-dd HH:mm:ss fff", System.Globalization.CultureInfo.CurrentCulture).ToString("yyyy-MM-dd HH:mm:ss.fff"),
                         null, jason, error);
                    while (true)
                    {
                        Thread.Sleep(1);
                        if ((DateTime.Now - start).TotalMilliseconds > ReceiveTimeOut)
                        {
                            if (!HeartBeatError && tryTimes < 3)
                                goto retry;
                            if (ATL.Common.StringResources.IsDefaultLanguage)
                            {
                                error = "MES响应A067超时";
                            }
                            else
                            {
                                error = "MES response A067 timeout";
                            }
                            LogInDB.Error(error);
                            DAL.me.UpdateMesInterfaceLogErrorMsg("A067", root.Header.SessionID, error);
                            //DAL.me.InsertToMESofflineBuffer(root.Header.EQCode, "A067", root.Header.SessionID, jason, lstBarcode);
                            MesErrorStopPLC(root.Header.EQCode);
                            return null;
                        }
                        try
                        {
                            if (A068_Buffer.jason != null && A068_Buffer.jason.Contains(offlineDataInfo.Guid))
                            {
                                A068.Root _root = JsonNewtonsoft.FromJSON<A068.Root>(A068_Buffer.jason);
                                return _root;
                            }
                        }
                        catch (Exception ex)
                        {
                            if (ATL.Common.StringResources.IsDefaultLanguage)
                            {
                                error = "解析 A068 Jason失败：" + ex.Message;
                            }
                            else
                            {
                                error = "Analysis Jaoson-A068 failure：" + ex.Message;
                            }
                            LogInDB.Error(error);
                            DAL.me.UpdateMesInterfaceLogErrorMsg("A068", root.Header.SessionID, error);
                            return null;
                        }
                    }
                }
                else
                {
                    LogInDB.Error(error);
                    //DAL.me.InsertToMESofflineBuffer(root.Header.EQCode, "A067", root.Header.SessionID, jason, lstBarcode);
                    DAL.me.InsertMesInterfaceLog("A067", root.Header.SessionID,
                        DateTime.ParseExact(root.Header.RequestTime, "yyyy-MM-dd HH:mm:ss fff", System.Globalization.CultureInfo.CurrentCulture).ToString("yyyy-MM-dd HH:mm:ss.fff"),
                         null, jason, error);
                    return null;
                }
            }
        }

        private object A069LokObj = new object();
        public A070.Root A069(string Type,string HoldCode, List<string> Products, string EquipmentID = "")
        {
            int tryTimes = 0;
            A069.Root root = new A069.Root();
            string error = string.Empty;

            root.Header = new MES.A069.Header();
            root.Header.EQCode = string.IsNullOrEmpty(EquipmentID) ? Station.Station.Current.EquipmentID : EquipmentID;
            root.Header.FunctionID = "A069";
            root.Header.PCName = Station.Station.Current.PCName;
            root.Header.RequestTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff");
            root.Header.SessionID = Guid.NewGuid().ToString();
            root.Header.SoftName = Station.Station.Current.SoftName;

            root.RequestInfo = new A069.RequestInfo();
            root.RequestInfo.Type = Type;
            root.RequestInfo.HoldCode = HoldCode;
            root.RequestInfo.ProductsSN1 = Products;

            string jason = root.ToJSON();
            OfflineDataInfo offlineDataInfo = new OfflineDataInfo();
            offlineDataInfo.Guid = root.Header.SessionID;
            offlineDataInfo.Data = jason;
            offlineDataInfo.FunctionID = root.Header.FunctionID;
            List<string> lstBarcode = new List<string>();
            foreach (var item in Products)
            {
                lstBarcode.Add(item);
            }

            if (!_allowWork())
            {
                if (ATL.Common.StringResources.IsDefaultLanguage)
                {
                    LogInDB.Error("PC和MES通信中断");
                }
                else
                {
                    LogInDB.Error("PC soft MES interaction terminated，probable cause：pc soft start and config check failed、MES communication error");
                }                //DAL.me.InsertToMESofflineBuffer(root.Header.EQCode, "A069", root.Header.SessionID, jason, lstBarcode);
                return null;
            }
            A070_Buffer = new RecvdData();
            retry:
            tryTimes++;
            DateTime start = DateTime.Now;
            lock(A069LokObj)
            {
                if (Send("A069", offlineDataInfo.Data, out error))
                {
                    DAL.me.InsertMesInterfaceLog("A069", root.Header.SessionID,
                        DateTime.ParseExact(root.Header.RequestTime, "yyyy-MM-dd HH:mm:ss fff", System.Globalization.CultureInfo.CurrentCulture).ToString("yyyy-MM-dd HH:mm:ss.fff"),
                         null, jason, error);
                    while (true)
                    {
                        Thread.Sleep(1);
                        if ((DateTime.Now - start).TotalMilliseconds > ReceiveTimeOut)
                        {
                            if (!HeartBeatError && tryTimes < 3)
                                goto retry;
                            if (ATL.Common.StringResources.IsDefaultLanguage)
                            {
                                error = "MES响应A069超时";
                            }
                            else
                            {
                                error = "MES response A069 timeout";
                            }
                            LogInDB.Error(error);
                            DAL.me.UpdateMesInterfaceLogErrorMsg("A069", root.Header.SessionID, error);
                            //DAL.me.InsertToMESofflineBuffer(root.Header.EQCode, "A069", root.Header.SessionID, jason, lstBarcode);
                            MesErrorStopPLC(root.Header.EQCode);
                            return null;
                        }
                        try
                        {
                            if (A070_Buffer.jason != null && A070_Buffer.jason.Contains(offlineDataInfo.Guid))
                            {
                                A070.Root _root = JsonNewtonsoft.FromJSON<A070.Root>(A070_Buffer.jason);
                                return _root;
                            }
                        }
                        catch (Exception ex)
                        {
                            if (ATL.Common.StringResources.IsDefaultLanguage)
                            {
                                error = "解析 A070 Jason失败：" + ex.Message;
                            }
                            else
                            {
                                error = "Analysis Jaoson-A070 failure：" + ex.Message;
                            }
                            LogInDB.Error(error);
                            DAL.me.UpdateMesInterfaceLogErrorMsg("A070", root.Header.SessionID, error);
                            return null;
                        }
                    }
                }
                else
                {
                    LogInDB.Error(error);
                    //DAL.me.InsertToMESofflineBuffer(root.Header.EQCode, "A069", root.Header.SessionID, jason, lstBarcode);
                    DAL.me.InsertMesInterfaceLog("A069", root.Header.SessionID,
                        DateTime.ParseExact(root.Header.RequestTime, "yyyy-MM-dd HH:mm:ss fff", System.Globalization.CultureInfo.CurrentCulture).ToString("yyyy-MM-dd HH:mm:ss.fff"),
                         null, jason, error);
                    return null;
                }
            }
        }

        private object A071LokObj = new object();
        public A072.Root A071(string Type, List<A071.ProductsItem> Products, string EquipmentID = "")
        {
            int tryTimes = 0;
            A071.Root root = new A071.Root();
            string error = string.Empty;

            root.Header = new MES.A071.Header();
            root.Header.EQCode = string.IsNullOrEmpty(EquipmentID) ? Station.Station.Current.EquipmentID : EquipmentID;
            root.Header.FunctionID = "A071";
            root.Header.PCName = Station.Station.Current.PCName;
            root.Header.RequestTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff");
            root.Header.SessionID = Guid.NewGuid().ToString();
            root.Header.SoftName = Station.Station.Current.SoftName;

            root.RequestInfo = new A071.RequestInfo();
            root.RequestInfo.Type = Type;
            root.RequestInfo.Products = Products;

            string jason = root.ToJSON();
            OfflineDataInfo offlineDataInfo = new OfflineDataInfo();
            offlineDataInfo.Guid = root.Header.SessionID;
            offlineDataInfo.Data = jason;
            offlineDataInfo.FunctionID = root.Header.FunctionID;
            List<string> lstBarcode = new List<string>();
            foreach (var item in Products)
            {
                lstBarcode.Add(item.ProductSN1);
            }

            if (!_allowWork())
            {
                if (ATL.Common.StringResources.IsDefaultLanguage)
                {
                    LogInDB.Error("PC和MES通信中断");
                }
                else
                {
                    LogInDB.Error("PC soft MES interaction terminated，probable cause：pc soft start and config check failed、MES communication error");
                }                //DAL.me.InsertToMESofflineBuffer(root.Header.EQCode, "A071", root.Header.SessionID, jason, lstBarcode);
                return null;
            }
            A072_Buffer = new RecvdData();
            retry:
            tryTimes++;
            DateTime start = DateTime.Now;
            lock(A071LokObj)
            {
                if (Send("A071", offlineDataInfo.Data, out error))
                {
                    DAL.me.InsertMesInterfaceLog("A071", root.Header.SessionID,
                        DateTime.ParseExact(root.Header.RequestTime, "yyyy-MM-dd HH:mm:ss fff", System.Globalization.CultureInfo.CurrentCulture).ToString("yyyy-MM-dd HH:mm:ss.fff"),
                         null, jason, error);
                    while (true)
                    {
                        Thread.Sleep(1);
                        if ((DateTime.Now - start).TotalMilliseconds > ReceiveTimeOut)
                        {
                            if (!HeartBeatError && tryTimes < 3)
                                goto retry;
                            if (ATL.Common.StringResources.IsDefaultLanguage)
                            {
                                error = "MES响应A071超时";
                            }
                            else
                            {
                                error = "MES response A071 timeout";
                            }
                            LogInDB.Error(error);
                            DAL.me.UpdateMesInterfaceLogErrorMsg("A071", root.Header.SessionID, error);
                            //DAL.me.InsertToMESofflineBuffer(root.Header.EQCode, "A071", root.Header.SessionID, jason, lstBarcode);
                            MesErrorStopPLC(root.Header.EQCode);
                            return null;
                        }
                        try
                        {
                            if (A072_Buffer.jason != null && A072_Buffer.jason.Contains(offlineDataInfo.Guid))
                            {
                                A072.Root _root = JsonNewtonsoft.FromJSON<A072.Root>(A072_Buffer.jason);
                                return _root;
                            }
                        }
                        catch (Exception ex)
                        {
                            if (ATL.Common.StringResources.IsDefaultLanguage)
                            {
                                error = "解析 A072 Jason失败：" + ex.Message;
                            }
                            else
                            {
                                error = "Analysis Jaoson-A072 failure：" + ex.Message;
                            }
                            LogInDB.Error(error);
                            DAL.me.UpdateMesInterfaceLogErrorMsg("A072", root.Header.SessionID, error);
                            return null;
                        }
                    }
                }
                else
                {
                    LogInDB.Error(error);
                    //DAL.me.InsertToMESofflineBuffer(root.Header.EQCode, "A071", root.Header.SessionID, jason, lstBarcode);
                    DAL.me.InsertMesInterfaceLog("A071", root.Header.SessionID,
                        DateTime.ParseExact(root.Header.RequestTime, "yyyy-MM-dd HH:mm:ss fff", System.Globalization.CultureInfo.CurrentCulture).ToString("yyyy-MM-dd HH:mm:ss.fff"),
                         null, jason, error);
                    return null;
                }
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Type"></param>
        /// <param name="Products"></param>
        /// <param name="Holdcode">holdcode字段，即可以输入holdcode字段，也可以没有这字段，如果没有这个字段，就和之前一样，如果有，就是ATL_WIP_SERIAL_NO里头，
        /// 电芯的holdcode和指令发给mes的holdcode相同(假设holdcode是HA016，那么只有电芯holdcode也是HA016，区分大小写，才解，否则不解)的才自动解，否则不解。</param>
        /// <param name="EquipmentID"></param>
        /// <returns></returns>
        public A072.Root A071WithHoldcode(string Type, List<A071_Holdcode.ProductsItem> Products, string Holdcode, string EquipmentID = "")
        {
            int tryTimes = 0;
            A071_Holdcode.Root root = new A071_Holdcode.Root();
            string error = string.Empty;

            root.Header = new MES.A071_Holdcode.Header();
            root.Header.EQCode = string.IsNullOrEmpty(EquipmentID) ? Station.Station.Current.EquipmentID : EquipmentID;
            root.Header.FunctionID = "A071";
            root.Header.PCName = Station.Station.Current.PCName;
            root.Header.RequestTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff");
            root.Header.SessionID = Guid.NewGuid().ToString();
            root.Header.SoftName = Station.Station.Current.SoftName;

            root.RequestInfo = new A071_Holdcode.RequestInfo();
            root.RequestInfo.Type = Type;
            root.RequestInfo.Products = Products;
            root.RequestInfo.Holdcode = Holdcode;
            string jason = root.ToJSON();
            OfflineDataInfo offlineDataInfo = new OfflineDataInfo();
            offlineDataInfo.Guid = root.Header.SessionID;
            offlineDataInfo.Data = jason;
            offlineDataInfo.FunctionID = root.Header.FunctionID;
            List<string> lstBarcode = new List<string>();
            foreach (var item in Products)
            {
                lstBarcode.Add(item.ProductSN1);
            }

            if (!_allowWork())
            {
                if (ATL.Common.StringResources.IsDefaultLanguage)
                {
                    LogInDB.Error("PC和MES通信中断");
                }
                else
                {
                    LogInDB.Error("PC soft MES interaction terminated，probable cause：pc soft start and config check failed、MES communication error");
                }                //DAL.me.InsertToMESofflineBuffer(root.Header.EQCode, "A071", root.Header.SessionID, jason, lstBarcode);
                return null;
            }
            A072_Buffer = new RecvdData();
            retry:
            tryTimes++;
            DateTime start = DateTime.Now;
            lock (A071LokObj)
            {
                if (Send("A071", offlineDataInfo.Data, out error))
                {
                    DAL.me.InsertMesInterfaceLog("A071", root.Header.SessionID,
                        DateTime.ParseExact(root.Header.RequestTime, "yyyy-MM-dd HH:mm:ss fff", System.Globalization.CultureInfo.CurrentCulture).ToString("yyyy-MM-dd HH:mm:ss.fff"),
                         null, jason, error);
                    while (true)
                    {
                        Thread.Sleep(1);
                        if ((DateTime.Now - start).TotalMilliseconds > ReceiveTimeOut)
                        {
                            if (!HeartBeatError && tryTimes < 3)
                                goto retry;
                            if (ATL.Common.StringResources.IsDefaultLanguage)
                            {
                                error = "MES响应A071超时";
                            }
                            else
                            {
                                error = "MES response A071 timeout";
                            }
                            LogInDB.Error(error);
                            DAL.me.UpdateMesInterfaceLogErrorMsg("A071", root.Header.SessionID, error);
                            //DAL.me.InsertToMESofflineBuffer(root.Header.EQCode, "A071", root.Header.SessionID, jason, lstBarcode);
                            MesErrorStopPLC(root.Header.EQCode);
                            return null;
                        }
                        try
                        {
                            if (A072_Buffer.jason != null && A072_Buffer.jason.Contains(offlineDataInfo.Guid))
                            {
                                A072.Root _root = JsonNewtonsoft.FromJSON<A072.Root>(A072_Buffer.jason);
                                return _root;
                            }
                        }
                        catch (Exception ex)
                        {
                            if (ATL.Common.StringResources.IsDefaultLanguage)
                            {
                                error = "解析 A072 Jason失败：" + ex.Message;
                            }
                            else
                            {
                                error = "Analysis Jaoson-A072 failure：" + ex.Message;
                            }
                            LogInDB.Error(error);
                            DAL.me.UpdateMesInterfaceLogErrorMsg("A072", root.Header.SessionID, error);
                            return null;
                        }
                    }
                }
                else
                {
                    LogInDB.Error(error);
                    //DAL.me.InsertToMESofflineBuffer(root.Header.EQCode, "A071", root.Header.SessionID, jason, lstBarcode);
                    DAL.me.InsertMesInterfaceLog("A071", root.Header.SessionID,
                        DateTime.ParseExact(root.Header.RequestTime, "yyyy-MM-dd HH:mm:ss fff", System.Globalization.CultureInfo.CurrentCulture).ToString("yyyy-MM-dd HH:mm:ss.fff"),
                         null, jason, error);
                    return null;
                }
            }
        }
        
        private object A073LokObj = new object();
        public A074.Root A073(string Carrier, string EquipmentID = "")
        {
            int tryTimes = 0;
            A073.Root root = new A073.Root();
            string error = string.Empty;

            root.Header = new MES.A073.Header();
            root.Header.EQCode = string.IsNullOrEmpty(EquipmentID) ? Station.Station.Current.EquipmentID : EquipmentID;
            root.Header.FunctionID = "A073";
            root.Header.PCName = Station.Station.Current.PCName;
            root.Header.RequestTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff");
            root.Header.SessionID = Guid.NewGuid().ToString();
            root.Header.SoftName = Station.Station.Current.SoftName;

            root.RequestInfo = new A073.RequestInfo();
            root.RequestInfo.Container = Carrier;

            string jason = root.ToJSON();
            OfflineDataInfo offlineDataInfo = new OfflineDataInfo();
            offlineDataInfo.Guid = root.Header.SessionID;
            offlineDataInfo.Data = jason;
            offlineDataInfo.FunctionID = root.Header.FunctionID;

            if (!_allowWork())
            {
                if (ATL.Common.StringResources.IsDefaultLanguage)
                {
                    LogInDB.Error("PC和MES通信中断");
                }
                else
                {
                    LogInDB.Error("PC soft MES interaction terminated，probable cause：pc soft start and config check failed、MES communication error");
                }                //DAL.me.InsertToMESofflineBuffer(root.Header.EQCode, "A073", root.Header.SessionID, jason);
                return null;
            }
            A074_Buffer = new RecvdData();
            retry:
            tryTimes++;
            DateTime start = DateTime.Now;
            lock(A073LokObj)
            {
                if (Send("A073", offlineDataInfo.Data, out error))
                {
                    DAL.me.InsertMesInterfaceLog("A073", root.Header.SessionID,
                        DateTime.ParseExact(root.Header.RequestTime, "yyyy-MM-dd HH:mm:ss fff", System.Globalization.CultureInfo.CurrentCulture).ToString("yyyy-MM-dd HH:mm:ss.fff"),
                         null, jason, error);
                    while (true)
                    {
                        Thread.Sleep(1);
                        if ((DateTime.Now - start).TotalMilliseconds > ReceiveTimeOut)
                        {
                            if (!HeartBeatError && tryTimes < 3)
                                goto retry;
                            if (ATL.Common.StringResources.IsDefaultLanguage)
                            {
                                error = "MES响应A073超时";
                            }
                            else
                            {
                                error = "MES response A073 timeout";
                            }
                            LogInDB.Error(error);
                            DAL.me.UpdateMesInterfaceLogErrorMsg("A073", root.Header.SessionID, error);
                            //DAL.me.InsertToMESofflineBuffer(root.Header.EQCode, "A073", root.Header.SessionID, jason);
                            MesErrorStopPLC(root.Header.EQCode);
                            return null;
                        }
                        try
                        {
                            if (A074_Buffer.jason != null && A074_Buffer.jason.Contains(offlineDataInfo.Guid))
                            {
                                A074.Root _root = JsonNewtonsoft.FromJSON<A074.Root>(A074_Buffer.jason);
                                return _root;
                            }
                        }
                        catch (Exception ex)
                        {
                            if (ATL.Common.StringResources.IsDefaultLanguage)
                            {
                                error = "解析 A074 Jason失败：" + ex.Message;
                            }
                            else
                            {
                                error = "Analysis Jaoson-A074 failure：" + ex.Message;
                            }
                            LogInDB.Error(error);
                            DAL.me.UpdateMesInterfaceLogErrorMsg("A074", root.Header.SessionID, error);
                            return null;
                        }
                    }
                }
                else
                {
                    LogInDB.Error(error);
                    //DAL.me.InsertToMESofflineBuffer(root.Header.EQCode, "A073", root.Header.SessionID, jason);
                    DAL.me.InsertMesInterfaceLog("A073", root.Header.SessionID,
                        DateTime.ParseExact(root.Header.RequestTime, "yyyy-MM-dd HH:mm:ss fff", System.Globalization.CultureInfo.CurrentCulture).ToString("yyyy-MM-dd HH:mm:ss.fff"),
                         null, jason, error);
                    return null;
                }
            }
        }

        private object A075LokObj = new object();
        public A076.Root A075(string Carrier,List<A075.Cell> Cells, string EquipmentID = "")
        {
            int tryTimes = 0;
            A075.Root root = new A075.Root();
            string error = string.Empty;

            root.Header = new MES.A075.Header();
            root.Header.EQCode = string.IsNullOrEmpty(EquipmentID) ? Station.Station.Current.EquipmentID : EquipmentID;
            root.Header.FunctionID = "A075";
            root.Header.PCName = Station.Station.Current.PCName;
            root.Header.RequestTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff");
            root.Header.SessionID = Guid.NewGuid().ToString();
            root.Header.SoftName = Station.Station.Current.SoftName;

            root.RequestInfo = new A075.RequestInfo();
            root.RequestInfo.Carrier = Carrier;
            root.RequestInfo.Cells = Cells;

            string jason = root.ToJSON();
            OfflineDataInfo offlineDataInfo = new OfflineDataInfo();
            offlineDataInfo.Guid = root.Header.SessionID;
            offlineDataInfo.Data = jason;
            offlineDataInfo.FunctionID = root.Header.FunctionID;

            if (!_allowWork())
            {
                if (ATL.Common.StringResources.IsDefaultLanguage)
                {
                    LogInDB.Error("PC和MES通信中断");
                }
                else
                {
                    LogInDB.Error("PC soft MES interaction terminated，probable cause：pc soft start and config check failed、MES communication error");
                }                //DAL.me.InsertToMESofflineBuffer(root.Header.EQCode, "A075", root.Header.SessionID, jason);
                return null;
            }
            A076_Buffer = new RecvdData();
            retry:
            tryTimes++;
            DateTime start = DateTime.Now;
            lock(A075LokObj)
            {
                if (Send("A075", offlineDataInfo.Data, out error))
                {
                    DAL.me.InsertMesInterfaceLog("A075", root.Header.SessionID,
                        DateTime.ParseExact(root.Header.RequestTime, "yyyy-MM-dd HH:mm:ss fff", System.Globalization.CultureInfo.CurrentCulture).ToString("yyyy-MM-dd HH:mm:ss.fff"),
                         null, jason, error);
                    while (true)
                    {
                        Thread.Sleep(1);
                        if ((DateTime.Now - start).TotalMilliseconds > ReceiveTimeOut)
                        {
                            if (!HeartBeatError && tryTimes < 3)
                                goto retry;
                            if (ATL.Common.StringResources.IsDefaultLanguage)
                            {
                                error = "MES响应A075超时";
                            }
                            else
                            {
                                error = "MES response A075 timeout";
                            }
                            LogInDB.Error(error);
                            DAL.me.UpdateMesInterfaceLogErrorMsg("A075", root.Header.SessionID, error);
                            //DAL.me.InsertToMESofflineBuffer(root.Header.EQCode, "A075", root.Header.SessionID, jason);
                            MesErrorStopPLC(root.Header.EQCode);
                            return null;
                        }
                        try
                        {
                            if (A076_Buffer.jason != null && A076_Buffer.jason.Contains(offlineDataInfo.Guid))
                            {
                                A076.Root _root = JsonNewtonsoft.FromJSON<A076.Root>(A076_Buffer.jason);
                                return _root;
                            }
                        }
                        catch (Exception ex)
                        {
                            if (ATL.Common.StringResources.IsDefaultLanguage)
                            {
                                error = "解析 A076 Jason失败：" + ex.Message;
                            }
                            else
                            {
                                error = "Analysis Jaoson-A076 failure：" + ex.Message;
                            }
                            LogInDB.Error(error);
                            DAL.me.UpdateMesInterfaceLogErrorMsg("A076", root.Header.SessionID, error);
                            return null;
                        }
                    }
                }
                else
                {
                    LogInDB.Error(error);
                    //DAL.me.InsertToMESofflineBuffer(root.Header.EQCode, "A075", root.Header.SessionID, jason);
                    DAL.me.InsertMesInterfaceLog("A075", root.Header.SessionID,
                        DateTime.ParseExact(root.Header.RequestTime, "yyyy-MM-dd HH:mm:ss fff", System.Globalization.CultureInfo.CurrentCulture).ToString("yyyy-MM-dd HH:mm:ss.fff"),
                         null, jason, error);
                    return null;
                }
            }
        }

        private object A077LokObj = new object();
        public A078.Root A077(string Type,string OperationMark, List<A077.Containers> Containers, string EquipmentID = "")
        {
            DateTime startTime = DateTime.Now;
            int tryTimes = 0;
            A077.Root root = new A077.Root();
            string error = string.Empty;

            root.Header = new MES.A077.Header();
            root.Header.EQCode = string.IsNullOrEmpty(EquipmentID) ? Station.Station.Current.EquipmentID : EquipmentID;
            root.Header.FunctionID = "A077";
            root.Header.PCName = Station.Station.Current.PCName;
            root.Header.RequestTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff");
            root.Header.SessionID = Guid.NewGuid().ToString();
            root.Header.SoftName = Station.Station.Current.SoftName;

            root.RequestInfo = new A077.RequestInfo();
            root.RequestInfo.Type = Type;
            root.RequestInfo.OperationMark = OperationMark;
            root.RequestInfo.Containers = Containers;

            string jason = root.ToJSON();
            OfflineDataInfo offlineDataInfo = new OfflineDataInfo();
            offlineDataInfo.Guid = root.Header.SessionID;
            offlineDataInfo.Data = jason;
            offlineDataInfo.FunctionID = root.Header.FunctionID;

            if (!_allowWork())
            {
                if (ATL.Common.StringResources.IsDefaultLanguage)
                {
                    LogInDB.Error("PC和MES通信中断");
                }
                else
                {
                    LogInDB.Error("PC soft MES interaction terminated，probable cause：pc soft start and config check failed、MES communication error");
                }
                DAL.me.InsertToMESofflineBuffer(root.Header.EQCode, "A077", root.Header.SessionID, jason);
                return null;
            }
            A078_Buffer = new RecvdData();
            retry:
            tryTimes++;
            DateTime start = DateTime.Now;
            lock(A077LokObj)
            {

                if (Send("A077", offlineDataInfo.Data, out error))
                {
                    DAL.me.InsertMesInterfaceLog("A077", root.Header.SessionID,
                        DateTime.ParseExact(root.Header.RequestTime, "yyyy-MM-dd HH:mm:ss fff", System.Globalization.CultureInfo.CurrentCulture).ToString("yyyy-MM-dd HH:mm:ss.fff"),
                         null, jason, error);
                    while (true)
                    {
                        Thread.Sleep(1);
                        if ((DateTime.Now - start).TotalMilliseconds > ReceiveTimeOut)
                        {
                            if (!HeartBeatError && tryTimes < 3)
                                goto retry;
                            if (ATL.Common.StringResources.IsDefaultLanguage)
                            {
                                error = "MES响应A077超时";
                            }
                            else
                            {
                                error = "MES response A077 timeout";
                            }
                            LogInDB.Error(error);
                            DAL.me.UpdateMesInterfaceLogErrorMsg("A077", root.Header.SessionID, error);
                            DAL.me.InsertToMESofflineBuffer(root.Header.EQCode, "A077", root.Header.SessionID, jason);
                            MesErrorStopPLC(root.Header.EQCode);
                            return null;
                        }
                        try
                        {
                            if (A078_Buffer.jason != null && A078_Buffer.jason.Contains(offlineDataInfo.Guid))
                            {
                                A078.Root _root = JsonNewtonsoft.FromJSON<A078.Root>(A078_Buffer.jason);
                                return _root;
                            }
                        }
                        catch (Exception ex)
                        {
                            if (ATL.Common.StringResources.IsDefaultLanguage)
                            {
                                error = "解析 A078 Jason失败：" + ex.Message;
                            }
                            else
                            {
                                error = "Analysis Jaoson-A078 failure：" + ex.Message;
                            }
                            LogInDB.Error(error);
                            DAL.me.UpdateMesInterfaceLogErrorMsg("A078", root.Header.SessionID, error);
                            return null;
                        }
                    }
                }
                else
                {
                    LogInDB.Error(error);
                    DAL.me.InsertToMESofflineBuffer(root.Header.EQCode, "A077", root.Header.SessionID, jason);
                    DAL.me.InsertMesInterfaceLog("A077", root.Header.SessionID,
                        DateTime.ParseExact(root.Header.RequestTime, "yyyy-MM-dd HH:mm:ss fff", System.Globalization.CultureInfo.CurrentCulture).ToString("yyyy-MM-dd HH:mm:ss.fff"),
                         null, jason, error);
                    return null;
                }
            }
        }

        private object A079LokObj = new object();
        public A080.Root A079(string Container, bool ISBLINGDING, string EquipmentID = "")
        {
            DateTime startTime = DateTime.Now;
            int tryTimes = 0;
            A079.Root root = new A079.Root();
            string error = string.Empty;

            root.Header = new MES.A079.Header();
            root.Header.EQCode = string.IsNullOrEmpty(EquipmentID) ? Station.Station.Current.EquipmentID : EquipmentID;
            root.Header.FunctionID = "A079";
            root.Header.PCName = Station.Station.Current.PCName;
            root.Header.RequestTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff");
            root.Header.SessionID = Guid.NewGuid().ToString();
            root.Header.SoftName = Station.Station.Current.SoftName;

            root.RequestInfo = new A079.RequestInfo();
            root.RequestInfo.Container = Container;
            root.RequestInfo.ISBLINGDING = ISBLINGDING?"True":"False";

            string jason = root.ToJSON();
            OfflineDataInfo offlineDataInfo = new OfflineDataInfo();
            offlineDataInfo.Guid = root.Header.SessionID;
            offlineDataInfo.Data = jason;
            offlineDataInfo.FunctionID = root.Header.FunctionID;

            if (!_allowWork())
            {
                if (ATL.Common.StringResources.IsDefaultLanguage)
                {
                    LogInDB.Error("PC和MES通信中断");
                }
                else
                {
                    LogInDB.Error("PC soft MES interaction terminated，probable cause：pc soft start and config check failed、MES communication error");
                }                //DAL.me.InsertToMESofflineBuffer(root.Header.EQCode, "A079", root.Header.SessionID, jason);
                return null;
            }
            A080_Buffer = new RecvdData();
            retry:
            tryTimes++;
            DateTime start = DateTime.Now;
            lock(A079LokObj)
            {
                if (Send("A079", offlineDataInfo.Data, out error))
                {
                    DAL.me.InsertMesInterfaceLog("A079", root.Header.SessionID,
                        DateTime.ParseExact(root.Header.RequestTime, "yyyy-MM-dd HH:mm:ss fff", System.Globalization.CultureInfo.CurrentCulture).ToString("yyyy-MM-dd HH:mm:ss.fff"),
                         null, jason, error);
                    while (true)
                    {
                        Thread.Sleep(1);
                        if ((DateTime.Now - start).TotalMilliseconds > ReceiveTimeOut)
                        {
                            if (!HeartBeatError && tryTimes < 3)
                                goto retry;
                            if (ATL.Common.StringResources.IsDefaultLanguage)
                            {
                                error = "MES响应A079超时";
                            }
                            else
                            {
                                error = "MES response A079 timeout";
                            }
                            LogInDB.Error(error);
                            DAL.me.UpdateMesInterfaceLogErrorMsg("A079", root.Header.SessionID, error);
                            //DAL.me.InsertToMESofflineBuffer(root.Header.EQCode, "A079", root.Header.SessionID, jason);
                            MesErrorStopPLC(root.Header.EQCode);
                            return null;
                        }
                        try
                        {
                            if (A080_Buffer.jason != null && A080_Buffer.jason.Contains(offlineDataInfo.Guid))
                            {
                                A080.Root _root = JsonNewtonsoft.FromJSON<A080.Root>(A080_Buffer.jason);
                                return _root;
                            }
                        }
                        catch (Exception ex)
                        {
                            if (ATL.Common.StringResources.IsDefaultLanguage)
                            {
                                error = "解析 A080 Jason失败：" + ex.Message;
                            }
                            else
                            {
                                error = "Analysis Jaoson-A080 failure：" + ex.Message;
                            }
                            LogInDB.Error(error);
                            DAL.me.UpdateMesInterfaceLogErrorMsg("A080", root.Header.SessionID, error);
                            return null;
                        }
                    }
                }
                else
                {
                    LogInDB.Error(error);
                    //DAL.me.InsertToMESofflineBuffer(root.Header.EQCode, "A079", root.Header.SessionID, jason);
                    DAL.me.InsertMesInterfaceLog("A079", root.Header.SessionID,
                        DateTime.ParseExact(root.Header.RequestTime, "yyyy-MM-dd HH:mm:ss fff", System.Globalization.CultureInfo.CurrentCulture).ToString("yyyy-MM-dd HH:mm:ss.fff"),
                         null, jason, error);
                    return null;
                }
            }
        }

        public A082.Root A081(string machine, string is_export, string EquipmentID = "")
        {
            int tryTimes = 0;
            A081.Root root = new A081.Root();
            string error = string.Empty;

            root.Header = new MES.A081.Header();
            root.Header.EQCode = string.IsNullOrEmpty(EquipmentID) ? Station.Station.Current.EquipmentID : EquipmentID;
            root.Header.FunctionID = "A081";
            root.Header.PCName = Station.Station.Current.PCName;
            root.Header.RequestTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff");
            root.Header.SessionID = Guid.NewGuid().ToString();
            root.Header.SoftName = Station.Station.Current.SoftName;

            root.RequestInfo = new A081.RequestInfo();
            root.RequestInfo.machine = machine;
            root.RequestInfo.is_export = is_export;

            string jason = root.ToJSON();
            OfflineDataInfo offlineDataInfo = new OfflineDataInfo();
            offlineDataInfo.Guid = root.Header.SessionID;
            offlineDataInfo.Data = jason;
            offlineDataInfo.FunctionID = root.Header.FunctionID;

            if (!_allowWork())
            {
                if (ATL.Common.StringResources.IsDefaultLanguage)
                {
                    LogInDB.Error("PC和MES通信中断");
                }
                else
                {
                    LogInDB.Error("PC soft MES interaction terminated，probable cause：pc soft start and config check failed、MES communication error");
                }
                //DAL.me.InsertToMESofflineBuffer(root.Header.EQCode, "A081", root.Header.SessionID, jason);
                return null;
            }
            A082_Buffer = new RecvdData();
            retry:
            tryTimes++;
            DateTime start = DateTime.Now;
            if (Send("A081", offlineDataInfo.Data, out error))
            {
                DAL.me.InsertMesInterfaceLog("A081", root.Header.SessionID,
                    DateTime.ParseExact(root.Header.RequestTime, "yyyy-MM-dd HH:mm:ss fff", System.Globalization.CultureInfo.CurrentCulture).ToString("yyyy-MM-dd HH:mm:ss.fff"),
                     null, jason, error);
                while (true)
                {
                    Thread.Sleep(1);
                    if ((DateTime.Now - start).TotalMilliseconds > ReceiveTimeOut)
                    {
                        if (!HeartBeatError && tryTimes < 3)
                            goto retry;
                        if (ATL.Common.StringResources.IsDefaultLanguage)
                        {
                            error = "MES响应A081超时";
                        }
                        else
                        {
                            error = "MES response A081 timeout";
                        }
                        LogInDB.Error(error);
                        DAL.me.UpdateMesInterfaceLogErrorMsg("A081", root.Header.SessionID, error);
                        //DAL.me.InsertToMESofflineBuffer(root.Header.EQCode, "A081", root.Header.SessionID, jason);
                        MesErrorStopPLC(root.Header.EQCode);
                        return null;
                    }
                    try
                    {
                        if (A082_Buffer.jason != null && A082_Buffer.jason.Contains(offlineDataInfo.Guid))
                        {
                            A082.Root _root = JsonNewtonsoft.FromJSON<A082.Root>(A082_Buffer.jason);
                            return _root;
                        }
                    }
                    catch (Exception ex)
                    {
                        if (ATL.Common.StringResources.IsDefaultLanguage)
                        {
                            error = "解析 A082 Jason失败：" + ex.Message;
                        }
                        else
                        {
                            error = "Analysis Jaoson-A082 failure：" + ex.Message;
                        }
                        LogInDB.Error(error);
                        DAL.me.UpdateMesInterfaceLogErrorMsg("A082", root.Header.SessionID, error);
                        return null;
                    }
                }
            }
            else
            {
                LogInDB.Error(error);
                //DAL.me.InsertToMESofflineBuffer(root.Header.EQCode, "A081", root.Header.SessionID, jason);
                DAL.me.InsertMesInterfaceLog("A081", root.Header.SessionID,
                    DateTime.ParseExact(root.Header.RequestTime, "yyyy-MM-dd HH:mm:ss fff", System.Globalization.CultureInfo.CurrentCulture).ToString("yyyy-MM-dd HH:mm:ss.fff"),
                     null, jason, error);
                return null;
            }
        }

        public A084.Root A083(string oldcontainer, string newcontainer, string lotType, string qty, string Operation, string EquipmentID = "")
        {
            int tryTimes = 0;
            A083.Root root = new A083.Root();
            string error = string.Empty;

            root.Header = new A083.Header();
            root.Header.EQCode = string.IsNullOrEmpty(EquipmentID) ? Station.Station.Current.EquipmentID : EquipmentID;
            root.Header.FunctionID = "A083";
            root.Header.PCName = Station.Station.Current.PCName;
            root.Header.RequestTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff");
            root.Header.SessionID = Guid.NewGuid().ToString();
            root.Header.SoftName = Station.Station.Current.SoftName;

            root.RequestInfo = new A083.RequestInfo();
            root.RequestInfo.oldcontainer = oldcontainer;
            root.RequestInfo.newcontainer = newcontainer;
            root.RequestInfo.lotType = lotType;
            root.RequestInfo.qty = qty;
            root.RequestInfo.Operation = Operation;

            string jason = root.ToJSON();
            OfflineDataInfo offlineDataInfo = new OfflineDataInfo();
            offlineDataInfo.Guid = root.Header.SessionID;
            offlineDataInfo.Data = jason;
            offlineDataInfo.FunctionID = root.Header.FunctionID;
            if (!_allowWork())
            {
                if (ATL.Common.StringResources.IsDefaultLanguage)
                {
                    LogInDB.Error("PC和MES通信中断");
                }
                else
                {
                    LogInDB.Error("PC soft MES interaction terminated，probable cause：pc soft start and config check failed、MES communication error");
                }
                //DAL.me.InsertToMESofflineBuffer(root.Header.EQCode, "A083", root.Header.SessionID, jason);
                return null;
            }
            A084_Buffer = new RecvdData();
            retry:
            tryTimes++;
            DateTime start = DateTime.Now;
            if (Send("A083", offlineDataInfo.Data, out error))
            {
                DAL.me.InsertMesInterfaceLog("A083", root.Header.SessionID,
                    DateTime.ParseExact(root.Header.RequestTime, "yyyy-MM-dd HH:mm:ss fff", System.Globalization.CultureInfo.CurrentCulture).ToString("yyyy-MM-dd HH:mm:ss.fff"),
                     null, jason, error);
                while (true)
                {
                    Thread.Sleep(1);
                    if ((DateTime.Now - start).TotalMilliseconds > ReceiveTimeOut)
                    {
                        if (!HeartBeatError && tryTimes < 3)
                            goto retry;
                        if (ATL.Common.StringResources.IsDefaultLanguage)
                        {
                            error = "MES响应A083超时";
                        }
                        else
                        {
                            error = "MES response A083 timeout";
                        }
                        LogInDB.Error(error);
                        DAL.me.UpdateMesInterfaceLogErrorMsg("A083", root.Header.SessionID, error);
                        //DAL.me.InsertToMESofflineBuffer(root.Header.EQCode, "A083", root.Header.SessionID, jason);
                        MesErrorStopPLC(root.Header.EQCode);
                        return null;
                    }
                    try
                    {
                        if (A084_Buffer.jason != null && A084_Buffer.jason.Contains(offlineDataInfo.Guid))
                        {
                            A084.Root _root = JsonNewtonsoft.FromJSON<A084.Root>(A084_Buffer.jason);
                            return _root;
                        }
                    }
                    catch (Exception ex)
                    {
                        if (ATL.Common.StringResources.IsDefaultLanguage)
                        {
                            error = "解析 A084 Jason失败：" + ex.Message;
                        }
                        else
                        {
                            error = "Analysis Jaoson-A084 failure：" + ex.Message;
                        }
                        LogInDB.Error(error);
                        DAL.me.UpdateMesInterfaceLogErrorMsg("A084", root.Header.SessionID, error);
                        return null;
                    }
                }
            }
            else
            {
                LogInDB.Error(error);
                //DAL.me.InsertToMESofflineBuffer(root.Header.EQCode, "A083", root.Header.SessionID, jason);
                DAL.me.InsertMesInterfaceLog("A083", root.Header.SessionID,
                    DateTime.ParseExact(root.Header.RequestTime, "yyyy-MM-dd HH:mm:ss fff", System.Globalization.CultureInfo.CurrentCulture).ToString("yyyy-MM-dd HH:mm:ss.fff"),
                     null, jason, error);
                return null;
            }
        }

        public A086.Root A085(List<string> SERIALNOS, List<string> DEFECTCODES, string OPERATION, string employeeNo, string EquipmentID = "")
        {
            int tryTimes = 0;
            A085.Root root = new A085.Root();
            string error = string.Empty;

            root.Header = new A085.Header();
            root.Header.EQCode = string.IsNullOrEmpty(EquipmentID) ? Station.Station.Current.EquipmentID : EquipmentID;
            root.Header.FunctionID = "A085";
            root.Header.PCName = Station.Station.Current.PCName;
            root.Header.RequestTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff");
            root.Header.SessionID = Guid.NewGuid().ToString();
            root.Header.SoftName = Station.Station.Current.SoftName;

            root.RequestInfo = new A085.RequestInfo();
            root.RequestInfo.SERIALNOS = SERIALNOS;
            root.RequestInfo.DEFECTCODES = DEFECTCODES;
            root.RequestInfo.OPERATION = OPERATION;
            root.RequestInfo.employeeNo = employeeNo;

            string jason = root.ToJSON();
            OfflineDataInfo offlineDataInfo = new OfflineDataInfo();
            offlineDataInfo.Guid = root.Header.SessionID;
            offlineDataInfo.Data = jason;
            offlineDataInfo.FunctionID = root.Header.FunctionID;
            if (!AllowWork)
            {
                string s = "PC soft MES interaction terminated，probable cause：pc soft start and config check failed、MES communication error";
                LogInDB.Error(s);
                //DAL.me.InsertToMESofflineBuffer(root.Header.EQCode, "A085", root.Header.SessionID, jason);
                return null;
            }
            A086_Buffer = new RecvdData();
            retry:
            tryTimes++;
            DateTime start = DateTime.Now;
            if (Send("A085", offlineDataInfo.Data, out error))
            {
                DAL.me.InsertMesInterfaceLog("A085", root.Header.SessionID,
                    DateTime.ParseExact(root.Header.RequestTime, "yyyy-MM-dd HH:mm:ss fff", System.Globalization.CultureInfo.CurrentCulture).ToString("yyyy-MM-dd HH:mm:ss.fff"),
                     null, jason, error);
                while (true)
                {
                    Thread.Sleep(1);
                    if ((DateTime.Now - start).TotalMilliseconds > ReceiveTimeOut)
                    {
                        if (!HeartBeatError && tryTimes < 3)
                            goto retry;
                        if (ATL.Common.StringResources.IsDefaultLanguage)
                        {
                            error = "MES响应A085超时";
                        }
                        else
                        {
                            error = "MES response A085 timeout";
                        }
                        LogInDB.Error(error);
                        DAL.me.UpdateMesInterfaceLogErrorMsg("A085", root.Header.SessionID, error);
                        //DAL.me.InsertToMESofflineBuffer(root.Header.EQCode, "A085", root.Header.SessionID, jason);
                        MesErrorStopPLC(root.Header.EQCode);
                        return null;
                    }
                    try
                    {
                        if (A086_Buffer.jason != null && A086_Buffer.jason.Contains(offlineDataInfo.Guid))
                        {
                            A086.Root _root = JsonNewtonsoft.FromJSON<A086.Root>(A086_Buffer.jason);
                            return _root;
                        }
                    }
                    catch (Exception ex)
                    {
                        if (ATL.Common.StringResources.IsDefaultLanguage)
                        {
                            error = "解析 A086 Jason失败：" + ex.Message;
                        }
                        else
                        {
                            error = "Analysis Jaoson-A086 failure：" + ex.Message;
                        }
                        LogInDB.Error(error);
                        DAL.me.UpdateMesInterfaceLogErrorMsg("A086", root.Header.SessionID, error);
                        return null;
                    }
                }
            }
            else
            {
                LogInDB.Error(error);
                //DAL.me.InsertToMESofflineBuffer(root.Header.EQCode, "A085", root.Header.SessionID, jason);
                DAL.me.InsertMesInterfaceLog("A085", root.Header.SessionID,
                    DateTime.ParseExact(root.Header.RequestTime, "yyyy-MM-dd HH:mm:ss fff", System.Globalization.CultureInfo.CurrentCulture).ToString("yyyy-MM-dd HH:mm:ss.fff"),
                     null, jason, error);
                return null;
            }
        }

        public A086.Root A085(List<string> SERIALNOS, List<string> DEFECTCODES, string OPERATION, string EquipmentID = "")
        {
            int tryTimes = 0;
            A085.Root root = new A085.Root();
            string error = string.Empty;

            root.Header = new A085.Header();
            root.Header.EQCode = string.IsNullOrEmpty(EquipmentID) ? Station.Station.Current.EquipmentID : EquipmentID;
            root.Header.FunctionID = "A085";
            root.Header.PCName = Station.Station.Current.PCName;
            root.Header.RequestTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff");
            root.Header.SessionID = Guid.NewGuid().ToString();
            root.Header.SoftName = Station.Station.Current.SoftName;

            root.RequestInfo = new A085.RequestInfo();
            root.RequestInfo.SERIALNOS = SERIALNOS;
            root.RequestInfo.DEFECTCODES = DEFECTCODES;
            root.RequestInfo.OPERATION = OPERATION;
            root.RequestInfo.employeeNo = ATL.MES.UserInfo.UserID;

            string jason = root.ToJSON();
            OfflineDataInfo offlineDataInfo = new OfflineDataInfo();
            offlineDataInfo.Guid = root.Header.SessionID;
            offlineDataInfo.Data = jason;
            offlineDataInfo.FunctionID = root.Header.FunctionID;
            if (!AllowWork)
            {
                string s = "PC soft MES interaction terminated，probable cause：pc soft start and config check failed、MES communication error";
                LogInDB.Error(s);
                //DAL.me.InsertToMESofflineBuffer(root.Header.EQCode, "A085", root.Header.SessionID, jason);
                return null;
            }
            A086_Buffer = new RecvdData();
            retry:
            tryTimes++;
            DateTime start = DateTime.Now;
            if (Send("A085", offlineDataInfo.Data, out error))
            {
                DAL.me.InsertMesInterfaceLog("A085", root.Header.SessionID,
                    DateTime.ParseExact(root.Header.RequestTime, "yyyy-MM-dd HH:mm:ss fff", System.Globalization.CultureInfo.CurrentCulture).ToString("yyyy-MM-dd HH:mm:ss.fff"),
                     null, jason, error);
                while (true)
                {
                    Thread.Sleep(1);
                    if ((DateTime.Now - start).TotalMilliseconds > ReceiveTimeOut)
                    {
                        if (!HeartBeatError && tryTimes < 3)
                            goto retry;
                        if (ATL.Common.StringResources.IsDefaultLanguage)
                        {
                            error = "MES响应A085超时";
                        }
                        else
                        {
                            error = "MES response A085 timeout";
                        }
                        LogInDB.Error(error);
                        DAL.me.UpdateMesInterfaceLogErrorMsg("A085", root.Header.SessionID, error);
                        //DAL.me.InsertToMESofflineBuffer(root.Header.EQCode, "A085", root.Header.SessionID, jason);
                        MesErrorStopPLC(root.Header.EQCode);
                        return null;
                    }
                    try
                    {
                        if (A086_Buffer.jason != null && A086_Buffer.jason.Contains(offlineDataInfo.Guid))
                        {
                            A086.Root _root = JsonNewtonsoft.FromJSON<A086.Root>(A086_Buffer.jason);
                            return _root;
                        }
                    }
                    catch (Exception ex)
                    {
                        if (ATL.Common.StringResources.IsDefaultLanguage)
                        {
                            error = "解析 A086 Jason失败：" + ex.Message;
                        }
                        else
                        {
                            error = "Analysis Jaoson-A086 failure：" + ex.Message;
                        }
                        LogInDB.Error(error);
                        DAL.me.UpdateMesInterfaceLogErrorMsg("A086", root.Header.SessionID, error);
                        return null;
                    }
                }
            }
            else
            {
                LogInDB.Error(error);
                //DAL.me.InsertToMESofflineBuffer(root.Header.EQCode, "A085", root.Header.SessionID, jason);
                DAL.me.InsertMesInterfaceLog("A085", root.Header.SessionID,
                    DateTime.ParseExact(root.Header.RequestTime, "yyyy-MM-dd HH:mm:ss fff", System.Globalization.CultureInfo.CurrentCulture).ToString("yyyy-MM-dd HH:mm:ss.fff"),
                     null, jason, error);
                return null;
            }
        }


        public A086.Root A085(List<string> SERIALNOS, List<string> DEFECTCODES, string OPERATION = "G080F")
        {
            int tryTimes = 0;
            A085.Root root = new A085.Root();
            string error = string.Empty;

            root.Header = new MES.A085.Header();
            root.Header.EQCode = Station.Station.Current.EquipmentID;
            root.Header.FunctionID = "A085";
            root.Header.PCName = Station.Station.Current.PCName;
            root.Header.RequestTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff");
            root.Header.SessionID = Guid.NewGuid().ToString();
            root.Header.SoftName = Station.Station.Current.SoftName;
            //ATL.UI.Core.AppContext.Current.UserId.ToString()
            root.RequestInfo = new A085.RequestInfo();
            root.RequestInfo.SERIALNOS = SERIALNOS;
            root.RequestInfo.DEFECTCODES = DEFECTCODES;
            root.RequestInfo.employeeNo = ATL.MES.UserInfo.UserID;
            root.RequestInfo.OPERATION = OPERATION;

            string jason = root.ToJSON();
            OfflineDataInfo offlineDataInfo = new OfflineDataInfo();
            offlineDataInfo.Guid = root.Header.SessionID;
            offlineDataInfo.Data = jason;
            offlineDataInfo.FunctionID = root.Header.FunctionID;

            if (Client == null || !MESconnected || HeartBeatError || !allDeviceRegistered || !AllowWork)
            {
                if (!AllowWork)
                {
                    string s = "请在代码逻辑里添加 InterfaceClient.AllowWork == true 的判断";
                    MessageBox.Show(s);
                    LogInDB.Error(s);
                    return null;
                }
                DAL.me.InsertToMESofflineBuffer(root.Header.EQCode, "A085", root.Header.SessionID, jason);
                return null;
            }
            A086_Buffer = new RecvdData();
            retry:
            tryTimes++;
            DateTime start = DateTime.Now;
            if (Send("A085", offlineDataInfo.Data, out error))
            {
                DAL.me.InsertMesInterfaceLog("A085", root.Header.SessionID,
                    DateTime.ParseExact(root.Header.RequestTime, "yyyy-MM-dd HH:mm:ss fff", System.Globalization.CultureInfo.CurrentCulture).ToString("yyyy-MM-dd HH:mm:ss.fff"),
                     null, jason, error);
                while (true)
                {
                    Thread.Sleep(0);
                    if ((DateTime.Now - start).TotalMilliseconds > ReceiveTimeOut)
                    {
                        if (!HeartBeatError && tryTimes <= 3)
                            goto retry;
                        error = "MES返回超时";
                        LogInDB.Error(error);
                        DAL.me.UpdateMesInterfaceLogErrorMsg("A085", root.Header.SessionID, error);
                        DAL.me.InsertToMESofflineBuffer(root.Header.EQCode, "A085", root.Header.SessionID, jason);
                        MesErrorStopPLC(root.Header.EQCode);
                        return null;
                    }
                    try
                    {
                        if (A086_Buffer.jason != null && A086_Buffer.jason.Contains(offlineDataInfo.Guid))
                        {
                            A086.Root _root = JsonNewtonsoft.FromJSON<A086.Root>(A086_Buffer.jason);
                            return _root;
                        }
                    }
                    catch (Exception ex)
                    {
                        error = "解析Jaoson-A086失败：" + ex.ToString();
                        LogInDB.Error(error);
                        DAL.me.UpdateMesInterfaceLogErrorMsg("A086", root.Header.SessionID, error);
                        return null;
                    }
                }
            }
            else
            {
                LogInDB.Error(error);
                DAL.me.InsertToMESofflineBuffer(root.Header.EQCode, "A085", root.Header.SessionID, jason);
                DAL.me.InsertMesInterfaceLog("A085", root.Header.SessionID,
                    DateTime.ParseExact(root.Header.RequestTime, "yyyy-MM-dd HH:mm:ss fff", System.Globalization.CultureInfo.CurrentCulture).ToString("yyyy-MM-dd HH:mm:ss.fff"),
                     null, jason, error);
                return null;
            }
        }

        public A088.Root A087(string LotNo, string Quantity,  string EquipmentID = "")
        {
            int tryTimes = 0;
            A087.Root root = new A087.Root();
            string error = string.Empty;

            root.Header = new A087.Header();
            root.Header.EQCode = string.IsNullOrEmpty(EquipmentID) ? Station.Station.Current.EquipmentID : EquipmentID;
            root.Header.FunctionID = "A087";
            root.Header.PCName = Station.Station.Current.PCName;
            root.Header.RequestTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff");
            root.Header.SessionID = Guid.NewGuid().ToString();
            root.Header.SoftName = Station.Station.Current.SoftName;

            root.RequestInfo = new A087.RequestInfo();
            root.RequestInfo.LotNo = LotNo;
            root.RequestInfo.Quantity = Quantity;

            string jason = root.ToJSON();
            OfflineDataInfo offlineDataInfo = new OfflineDataInfo();
            offlineDataInfo.Guid = root.Header.SessionID;
            offlineDataInfo.Data = jason;
            offlineDataInfo.FunctionID = root.Header.FunctionID;
            if (!AllowWork)
            {
                string s = "PC soft MES interaction terminated，probable cause：pc soft start and config check failed、MES communication error";
                LogInDB.Error(s);
                //DAL.me.InsertToMESofflineBuffer(root.Header.EQCode, "A087", root.Header.SessionID, jason);
                return null;
            }
            A088_Buffer = new RecvdData();
            retry:
            tryTimes++;
            DateTime start = DateTime.Now;
            if (Send("A087", offlineDataInfo.Data, out error))
            {
                DAL.me.InsertMesInterfaceLog("A087", root.Header.SessionID,
                    DateTime.ParseExact(root.Header.RequestTime, "yyyy-MM-dd HH:mm:ss fff", System.Globalization.CultureInfo.CurrentCulture).ToString("yyyy-MM-dd HH:mm:ss.fff"),
                     null, jason, error);
                while (true)
                {
                    Thread.Sleep(1);
                    if ((DateTime.Now - start).TotalMilliseconds > ReceiveTimeOut)
                    {
                        if (!HeartBeatError && tryTimes < 3)
                            goto retry;
                        if (ATL.Common.StringResources.IsDefaultLanguage)
                        {
                            error = "MES响应A087超时";
                        }
                        else
                        {
                            error = "MES response A087 timeout";
                        }
                        LogInDB.Error(error);
                        DAL.me.UpdateMesInterfaceLogErrorMsg("A087", root.Header.SessionID, error);
                        //DAL.me.InsertToMESofflineBuffer(root.Header.EQCode, "A087", root.Header.SessionID, jason);
                        MesErrorStopPLC(root.Header.EQCode);
                        return null;
                    }
                    try
                    {
                        if (A088_Buffer.jason != null && A088_Buffer.jason.Contains(offlineDataInfo.Guid))
                        {
                            A088.Root _root = JsonNewtonsoft.FromJSON<A088.Root>(A088_Buffer.jason);
                            return _root;
                        }
                    }
                    catch (Exception ex)
                    {
                        if (ATL.Common.StringResources.IsDefaultLanguage)
                        {
                            error = "解析 A088 Jason失败：" + ex.Message;
                        }
                        else
                        {
                            error = "Analysis Jaoson-A088 failure：" + ex.Message;
                        }
                        LogInDB.Error(error);
                        DAL.me.UpdateMesInterfaceLogErrorMsg("A088", root.Header.SessionID, error);
                        return null;
                    }
                }
            }
            else
            {
                LogInDB.Error(error);
                //DAL.me.InsertToMESofflineBuffer(root.Header.EQCode, "A087", root.Header.SessionID, jason);
                DAL.me.InsertMesInterfaceLog("A087", root.Header.SessionID,
                    DateTime.ParseExact(root.Header.RequestTime, "yyyy-MM-dd HH:mm:ss fff", System.Globalization.CultureInfo.CurrentCulture).ToString("yyyy-MM-dd HH:mm:ss.fff"),
                     null, jason, error);
                return null;
            }
        }

        public A090.Root A089(string CHARACTERISTIC, string GROUPCODE, string OPERATION, List<string> SERIALNOS, string EquipmentID = "")
        {
            int tryTimes = 0;
            A089.Root root = new A089.Root();
            string error = string.Empty;

            root.Header = new A089.Header();
            root.Header.EQCode = string.IsNullOrEmpty(EquipmentID) ? Station.Station.Current.EquipmentID : EquipmentID;
            root.Header.FunctionID = "A089";
            root.Header.PCName = Station.Station.Current.PCName;
            root.Header.RequestTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff");
            root.Header.SessionID = Guid.NewGuid().ToString();
            root.Header.SoftName = Station.Station.Current.SoftName;

            root.RequestInfo = new A089.RequestInfo();
            root.RequestInfo.CHARACTERISTIC = CHARACTERISTIC;
            root.RequestInfo.GROUPCODE = GROUPCODE;
            root.RequestInfo.OPERATION = OPERATION;
            root.RequestInfo.SERIALNOS = SERIALNOS;

            string jason = root.ToJSON();
            OfflineDataInfo offlineDataInfo = new OfflineDataInfo();
            offlineDataInfo.Guid = root.Header.SessionID;
            offlineDataInfo.Data = jason;
            offlineDataInfo.FunctionID = root.Header.FunctionID;
            if (!AllowWork)
            {
                string s = "PC soft MES interaction terminated，probable cause：pc soft start and config check failed、MES communication error";
                LogInDB.Error(s);
                //DAL.me.InsertToMESofflineBuffer(root.Header.EQCode, "A089", root.Header.SessionID, jason);
                return null;
            }
            A090_Buffer = new RecvdData();
            retry:
            tryTimes++;
            DateTime start = DateTime.Now;
            if (Send("A089", offlineDataInfo.Data, out error))
            {
                DAL.me.InsertMesInterfaceLog("A089", root.Header.SessionID,
                    DateTime.ParseExact(root.Header.RequestTime, "yyyy-MM-dd HH:mm:ss fff", System.Globalization.CultureInfo.CurrentCulture).ToString("yyyy-MM-dd HH:mm:ss.fff"),
                     null, jason, error);
                while (true)
                {
                    Thread.Sleep(1);
                    if ((DateTime.Now - start).TotalMilliseconds > ReceiveTimeOut)
                    {
                        if (!HeartBeatError && tryTimes < 3)
                            goto retry;
                        if (ATL.Common.StringResources.IsDefaultLanguage)
                        {
                            error = "MES响应A089超时";
                        }
                        else
                        {
                            error = "MES response A089 timeout";
                        }
                        LogInDB.Error(error);
                        DAL.me.UpdateMesInterfaceLogErrorMsg("A089", root.Header.SessionID, error);
                        //DAL.me.InsertToMESofflineBuffer(root.Header.EQCode, "A089", root.Header.SessionID, jason);
                        MesErrorStopPLC(root.Header.EQCode);
                        return null;
                    }
                    try
                    {
                        if (A090_Buffer.jason != null && A090_Buffer.jason.Contains(offlineDataInfo.Guid))
                        {
                            A090.Root _root = JsonNewtonsoft.FromJSON<A090.Root>(A090_Buffer.jason);
                            return _root;
                        }
                    }
                    catch (Exception ex)
                    {
                        if (ATL.Common.StringResources.IsDefaultLanguage)
                        {
                            error = "解析 A090 Jason失败：" + ex.Message;
                        }
                        else
                        {
                            error = "Analysis Jaoson-A090 failure：" + ex.Message;
                        }
                        LogInDB.Error(error);
                        DAL.me.UpdateMesInterfaceLogErrorMsg("A090", root.Header.SessionID, error);
                        return null;
                    }
                }
            }
            else
            {
                LogInDB.Error(error);
                //DAL.me.InsertToMESofflineBuffer(root.Header.EQCode, "A089", root.Header.SessionID, jason);
                DAL.me.InsertMesInterfaceLog("A089", root.Header.SessionID,
                    DateTime.ParseExact(root.Header.RequestTime, "yyyy-MM-dd HH:mm:ss fff", System.Globalization.CultureInfo.CurrentCulture).ToString("yyyy-MM-dd HH:mm:ss.fff"),
                     null, jason, error);
                return null;
            }
        }

        public A092.Root A091(A091.RequestInfo RequestInfo, string EquipmentID = "")
        {
            int tryTimes = 0;
            A091.Root root = new A091.Root();
            string error = string.Empty;

            root.Header = new A091.Header();
            root.Header.EQCode = string.IsNullOrEmpty(EquipmentID) ? Station.Station.Current.EquipmentID : EquipmentID;
            root.Header.FunctionID = "A091";
            root.Header.PCName = Station.Station.Current.PCName;
            root.Header.RequestTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff");
            root.Header.SessionID = Guid.NewGuid().ToString();
            root.Header.SoftName = Station.Station.Current.SoftName;

            root.RequestInfo = RequestInfo;

            string jason = root.ToJSON();
            OfflineDataInfo offlineDataInfo = new OfflineDataInfo();
            offlineDataInfo.Guid = root.Header.SessionID;
            offlineDataInfo.Data = jason;
            offlineDataInfo.FunctionID = root.Header.FunctionID;
            if (!AllowWork)
            {
                string s = "PC soft MES interaction terminated，probable cause：pc soft start and config check failed、MES communication error";
                LogInDB.Error(s);
                //DAL.me.InsertToMESofflineBuffer(root.Header.EQCode, "A091", root.Header.SessionID, jason);
                return null;
            }
            A092_Buffer = new RecvdData();
            retry:
            tryTimes++;
            DateTime start = DateTime.Now;
            if (Send("A091", offlineDataInfo.Data, out error))
            {
                DAL.me.InsertMesInterfaceLog("A091", root.Header.SessionID,
                    DateTime.ParseExact(root.Header.RequestTime, "yyyy-MM-dd HH:mm:ss fff", System.Globalization.CultureInfo.CurrentCulture).ToString("yyyy-MM-dd HH:mm:ss.fff"),
                     null, jason, error);
                while (true)
                {
                    Thread.Sleep(1);
                    if ((DateTime.Now - start).TotalMilliseconds > ReceiveTimeOut)
                    {
                        if (!HeartBeatError && tryTimes < 3)
                            goto retry;
                        if (ATL.Common.StringResources.IsDefaultLanguage)
                        {
                            error = "MES响应A091超时";
                        }
                        else
                        {
                            error = "MES response A091 timeout";
                        }
                        LogInDB.Error(error);
                        DAL.me.UpdateMesInterfaceLogErrorMsg("A091", root.Header.SessionID, error);
                        //DAL.me.InsertToMESofflineBuffer(root.Header.EQCode, "A091", root.Header.SessionID, jason);
                        MesErrorStopPLC(root.Header.EQCode);
                        return null;
                    }
                    try
                    {
                        if (A092_Buffer.jason != null && A092_Buffer.jason.Contains(offlineDataInfo.Guid))
                        {
                            A092.Root _root = JsonNewtonsoft.FromJSON<A092.Root>(A092_Buffer.jason);
                            return _root;
                        }
                    }
                    catch (Exception ex)
                    {
                        if (ATL.Common.StringResources.IsDefaultLanguage)
                        {
                            error = "解析 A092 Jason失败：" + ex.Message;
                        }
                        else
                        {
                            error = "Analysis Jaoson-A092 failure：" + ex.Message;
                        }
                        LogInDB.Error(error);
                        DAL.me.UpdateMesInterfaceLogErrorMsg("A092", root.Header.SessionID, error);
                        return null;
                    }
                }
            }
            else
            {
                LogInDB.Error(error);
                //DAL.me.InsertToMESofflineBuffer(root.Header.EQCode, "A091", root.Header.SessionID, jason);
                DAL.me.InsertMesInterfaceLog("A091", root.Header.SessionID,
                    DateTime.ParseExact(root.Header.RequestTime, "yyyy-MM-dd HH:mm:ss fff", System.Globalization.CultureInfo.CurrentCulture).ToString("yyyy-MM-dd HH:mm:ss.fff"),
                     null, jason, error);
                return null;
            }
        }

        public A094.Root A093(A093.RequestInfo RequestInfo, string EquipmentID = "")
        {
            int tryTimes = 0;
            A093.Root root = new A093.Root();
            string error = string.Empty;

            root.Header = new A093.Header();
            root.Header.EQCode = string.IsNullOrEmpty(EquipmentID) ? Station.Station.Current.EquipmentID : EquipmentID;
            root.Header.FunctionID = "A093";
            root.Header.PCName = Station.Station.Current.PCName;
            root.Header.RequestTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff");
            root.Header.SessionID = Guid.NewGuid().ToString();
            root.Header.SoftName = Station.Station.Current.SoftName;

            root.RequestInfo = RequestInfo;

            string jason = root.ToJSON();
            OfflineDataInfo offlineDataInfo = new OfflineDataInfo();
            offlineDataInfo.Guid = root.Header.SessionID;
            offlineDataInfo.Data = jason;
            offlineDataInfo.FunctionID = root.Header.FunctionID;
            if (!AllowWork)
            {
                string s = "PC soft MES interaction terminated，probable cause：pc soft start and config check failed、MES communication error";
                LogInDB.Error(s);
                //DAL.me.InsertToMESofflineBuffer(root.Header.EQCode, "A093", root.Header.SessionID, jason);
                return null;
            }
            A094_Buffer = new RecvdData();
            retry:
            tryTimes++;
            DateTime start = DateTime.Now;
            if (Send("A093", offlineDataInfo.Data, out error))
            {
                DAL.me.InsertMesInterfaceLog("A093", root.Header.SessionID,
                    DateTime.ParseExact(root.Header.RequestTime, "yyyy-MM-dd HH:mm:ss fff", System.Globalization.CultureInfo.CurrentCulture).ToString("yyyy-MM-dd HH:mm:ss.fff"),
                     null, jason, error);
                while (true)
                {
                    Thread.Sleep(1);
                    if ((DateTime.Now - start).TotalMilliseconds > ReceiveTimeOut)
                    {
                        if (!HeartBeatError && tryTimes < 3)
                            goto retry;
                        if (ATL.Common.StringResources.IsDefaultLanguage)
                        {
                            error = "MES响应A093超时";
                        }
                        else
                        {
                            error = "MES response A093 timeout";
                        }
                        LogInDB.Error(error);
                        DAL.me.UpdateMesInterfaceLogErrorMsg("A093", root.Header.SessionID, error);
                        //DAL.me.InsertToMESofflineBuffer(root.Header.EQCode, "A093", root.Header.SessionID, jason);
                        MesErrorStopPLC(root.Header.EQCode);
                        return null;
                    }
                    try
                    {
                        if (A094_Buffer.jason != null && A094_Buffer.jason.Contains(offlineDataInfo.Guid))
                        {
                            A094.Root _root = JsonNewtonsoft.FromJSON<A094.Root>(A094_Buffer.jason);
                            return _root;
                        }
                    }
                    catch (Exception ex)
                    {
                        if (ATL.Common.StringResources.IsDefaultLanguage)
                        {
                            error = "解析 A094 Jason失败：" + ex.Message;
                        }
                        else
                        {
                            error = "Analysis Jaoson-A094 failure：" + ex.Message;
                        }
                        LogInDB.Error(error);
                        DAL.me.UpdateMesInterfaceLogErrorMsg("A094", root.Header.SessionID, error);
                        return null;
                    }
                }
            }
            else
            {
                LogInDB.Error(error);
                //DAL.me.InsertToMESofflineBuffer(root.Header.EQCode, "A093", root.Header.SessionID, jason);
                DAL.me.InsertMesInterfaceLog("A093", root.Header.SessionID,
                    DateTime.ParseExact(root.Header.RequestTime, "yyyy-MM-dd HH:mm:ss fff", System.Globalization.CultureInfo.CurrentCulture).ToString("yyyy-MM-dd HH:mm:ss.fff"),
                     null, jason, error);
                return null;
            }
        }

        public A096.Root A095(A095.RequestInfo RequestInfo, string EquipmentID = "")
        {
            int tryTimes = 0;
            A095.Root root = new A095.Root();
            string error = string.Empty;

            root.Header = new A095.Header();
            root.Header.EQCode = string.IsNullOrEmpty(EquipmentID) ? Station.Station.Current.EquipmentID : EquipmentID;
            root.Header.FunctionID = "A095";
            root.Header.PCName = Station.Station.Current.PCName;
            root.Header.RequestTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff");
            root.Header.SessionID = Guid.NewGuid().ToString();
            root.Header.SoftName = Station.Station.Current.SoftName;

            root.RequestInfo = RequestInfo;

            string jason = root.ToJSON();
            OfflineDataInfo offlineDataInfo = new OfflineDataInfo();
            offlineDataInfo.Guid = root.Header.SessionID;
            offlineDataInfo.Data = jason;
            offlineDataInfo.FunctionID = root.Header.FunctionID;
            if (!AllowWork)
            {
                string s = "PC soft MES interaction terminated，probable cause：pc soft start and config check failed、MES communication error";
                LogInDB.Error(s);
                //DAL.me.InsertToMESofflineBuffer(root.Header.EQCode, "A095", root.Header.SessionID, jason);
                return null;
            }
            A096_Buffer = new RecvdData();
            retry:
            tryTimes++;
            DateTime start = DateTime.Now;
            if (Send("A095", offlineDataInfo.Data, out error))
            {
                DAL.me.InsertMesInterfaceLog("A095", root.Header.SessionID,
                    DateTime.ParseExact(root.Header.RequestTime, "yyyy-MM-dd HH:mm:ss fff", System.Globalization.CultureInfo.CurrentCulture).ToString("yyyy-MM-dd HH:mm:ss.fff"),
                     null, jason, error);
                while (true)
                {
                    Thread.Sleep(1);
                    if ((DateTime.Now - start).TotalMilliseconds > ReceiveTimeOut)
                    {
                        if (!HeartBeatError && tryTimes < 3)
                            goto retry;
                        if (ATL.Common.StringResources.IsDefaultLanguage)
                        {
                            error = "MES响应A095超时";
                        }
                        else
                        {
                            error = "MES response A095 timeout";
                        }
                        LogInDB.Error(error);
                        DAL.me.UpdateMesInterfaceLogErrorMsg("A095", root.Header.SessionID, error);
                        //DAL.me.InsertToMESofflineBuffer(root.Header.EQCode, "A095", root.Header.SessionID, jason);
                        MesErrorStopPLC(root.Header.EQCode);
                        return null;
                    }
                    try
                    {
                        if (A096_Buffer.jason != null && A096_Buffer.jason.Contains(offlineDataInfo.Guid))
                        {
                            A096.Root _root = JsonNewtonsoft.FromJSON<A096.Root>(A096_Buffer.jason);
                            return _root;
                        }
                    }
                    catch (Exception ex)
                    {
                        if (ATL.Common.StringResources.IsDefaultLanguage)
                        {
                            error = "解析 A096 Jason失败：" + ex.Message;
                        }
                        else
                        {
                            error = "Analysis Jaoson-A096 failure：" + ex.Message;
                        }
                        LogInDB.Error(error);
                        DAL.me.UpdateMesInterfaceLogErrorMsg("A096", root.Header.SessionID, error);
                        return null;
                    }
                }
            }
            else
            {
                LogInDB.Error(error);
                //DAL.me.InsertToMESofflineBuffer(root.Header.EQCode, "A095", root.Header.SessionID, jason);
                DAL.me.InsertMesInterfaceLog("A095", root.Header.SessionID,
                    DateTime.ParseExact(root.Header.RequestTime, "yyyy-MM-dd HH:mm:ss fff", System.Globalization.CultureInfo.CurrentCulture).ToString("yyyy-MM-dd HH:mm:ss.fff"),
                     null, jason, error);
                return null;
            }
        }

        public A098.Root A097(A097.RequestInfo RequestInfo, string EquipmentID = "")
        {
            int tryTimes = 0;
            A097.Root root = new A097.Root();
            string error = string.Empty;

            root.Header = new A097.Header();
            root.Header.EQCode = string.IsNullOrEmpty(EquipmentID) ? Station.Station.Current.EquipmentID : EquipmentID;
            root.Header.FunctionID = "A097";
            root.Header.PCName = Station.Station.Current.PCName;
            root.Header.RequestTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff");
            root.Header.SessionID = Guid.NewGuid().ToString();
            root.Header.SoftName = Station.Station.Current.SoftName;

            root.RequestInfo = RequestInfo;

            string jason = root.ToJSON();
            OfflineDataInfo offlineDataInfo = new OfflineDataInfo();
            offlineDataInfo.Guid = root.Header.SessionID;
            offlineDataInfo.Data = jason;
            offlineDataInfo.FunctionID = root.Header.FunctionID;
            if (!AllowWork)
            {
                string s = "PC soft MES interaction terminated，probable cause：pc soft start and config check failed、MES communication error";
                LogInDB.Error(s);
                //DAL.me.InsertToMESofflineBuffer(root.Header.EQCode, "A097", root.Header.SessionID, jason);
                return null;
            }
            A098_Buffer = new RecvdData();
            retry:
            tryTimes++;
            DateTime start = DateTime.Now;
            if (Send("A097", offlineDataInfo.Data, out error))
            {
                DAL.me.InsertMesInterfaceLog("A097", root.Header.SessionID,
                    DateTime.ParseExact(root.Header.RequestTime, "yyyy-MM-dd HH:mm:ss fff", System.Globalization.CultureInfo.CurrentCulture).ToString("yyyy-MM-dd HH:mm:ss.fff"),
                     null, jason, error);
                while (true)
                {
                    Thread.Sleep(1);
                    if ((DateTime.Now - start).TotalMilliseconds > ReceiveTimeOut)
                    {
                        if (!HeartBeatError && tryTimes < 3)
                            goto retry;
                        if (ATL.Common.StringResources.IsDefaultLanguage)
                        {
                            error = "MES响应A097超时";
                        }
                        else
                        {
                            error = "MES response A097 timeout";
                        }
                        LogInDB.Error(error);
                        DAL.me.UpdateMesInterfaceLogErrorMsg("A097", root.Header.SessionID, error);
                        //DAL.me.InsertToMESofflineBuffer(root.Header.EQCode, "A097", root.Header.SessionID, jason);
                        MesErrorStopPLC(root.Header.EQCode);
                        return null;
                    }
                    try
                    {
                        if (A098_Buffer.jason != null && A098_Buffer.jason.Contains(offlineDataInfo.Guid))
                        {
                            A098.Root _root = JsonNewtonsoft.FromJSON<A098.Root>(A098_Buffer.jason);
                            return _root;
                        }
                    }
                    catch (Exception ex)
                    {
                        if (ATL.Common.StringResources.IsDefaultLanguage)
                        {
                            error = "解析 A098 Jason失败：" + ex.Message;
                        }
                        else
                        {
                            error = "Analysis Jaoson-A098 failure：" + ex.Message;
                        }
                        LogInDB.Error(error);
                        DAL.me.UpdateMesInterfaceLogErrorMsg("A098", root.Header.SessionID, error);
                        return null;
                    }
                }
            }
            else
            {
                LogInDB.Error(error);
                //DAL.me.InsertToMESofflineBuffer(root.Header.EQCode, "A097", root.Header.SessionID, jason);
                DAL.me.InsertMesInterfaceLog("A097", root.Header.SessionID,
                    DateTime.ParseExact(root.Header.RequestTime, "yyyy-MM-dd HH:mm:ss fff", System.Globalization.CultureInfo.CurrentCulture).ToString("yyyy-MM-dd HH:mm:ss.fff"),
                     null, jason, error);
                return null;
            }
        }

        public A100.Root A099(string LampState, string Interval, string EquipmentID = "")
        {
            int tryTimes = 0;
            A099.Root root = new A099.Root();
            string error = string.Empty;

            root.Header = new A099.Header();
            root.Header.EQCode = string.IsNullOrEmpty(EquipmentID) ? Station.Station.Current.EquipmentID : EquipmentID;
            root.Header.FunctionID = "A099";
            root.Header.PCName = Station.Station.Current.PCName;
            root.Header.RequestTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff");
            root.Header.SessionID = Guid.NewGuid().ToString();
            root.Header.SoftName = Station.Station.Current.SoftName;

            root.RequestInfo = new MES.A099.RequestInfo();
            root.RequestInfo.LampState = LampState;
            root.RequestInfo.Interval = Interval;
            string jason = root.ToJSON();
            OfflineDataInfo offlineDataInfo = new OfflineDataInfo();
            offlineDataInfo.Guid = root.Header.SessionID;
            offlineDataInfo.Data = jason;
            offlineDataInfo.FunctionID = root.Header.FunctionID;
            if (!AllowWork)
            {
                string s = "PC soft MES interaction terminated，probable cause：pc soft start and config check failed、MES communication error";
                LogInDB.Error(s);
                //DAL.me.InsertToMESofflineBuffer(root.Header.EQCode, "A097", root.Header.SessionID, jason);
                return null;
            }
            A100_Buffer = new RecvdData();
            retry:
            tryTimes++;
            DateTime start = DateTime.Now;
            if (Send("A099", offlineDataInfo.Data, out error))
            {
                DAL.me.InsertMesInterfaceLog("A099", root.Header.SessionID,
                    DateTime.ParseExact(root.Header.RequestTime, "yyyy-MM-dd HH:mm:ss fff", System.Globalization.CultureInfo.CurrentCulture).ToString("yyyy-MM-dd HH:mm:ss.fff"),
                     null, jason, error);
                while (true)
                {
                    Thread.Sleep(1);
                    if ((DateTime.Now - start).TotalMilliseconds > ReceiveTimeOut)
                    {
                        if (!HeartBeatError && tryTimes < 3)
                            goto retry;
                        if (ATL.Common.StringResources.IsDefaultLanguage)
                        {
                            error = "MES响应A099超时";
                        }
                        else
                        {
                            error = "MES response A099 timeout";
                        }
                        LogInDB.Error(error);
                        DAL.me.UpdateMesInterfaceLogErrorMsg("A099", root.Header.SessionID, error);
                        //DAL.me.InsertToMESofflineBuffer(root.Header.EQCode, "A099", root.Header.SessionID, jason);
                        MesErrorStopPLC(root.Header.EQCode);
                        return null;
                    }
                    try
                    {
                        if (A100_Buffer.jason != null && A100_Buffer.jason.Contains(offlineDataInfo.Guid))
                        {
                            A100.Root _root = JsonNewtonsoft.FromJSON<A100.Root>(A100_Buffer.jason);
                            return _root;
                        }
                    }
                    catch (Exception ex)
                    {
                        if (ATL.Common.StringResources.IsDefaultLanguage)
                        {
                            error = "解析 A0100 Jason失败：" + ex.Message;
                        }
                        else
                        {
                            error = "Analysis Jaoson-A0100 failure：" + ex.Message;
                        }
                        LogInDB.Error(error);
                        DAL.me.UpdateMesInterfaceLogErrorMsg("A100", root.Header.SessionID, error);
                        return null;
                    }
                }
            }
            else
            {
                LogInDB.Error(error);
                //DAL.me.InsertToMESofflineBuffer(root.Header.EQCode, "A099", root.Header.SessionID, jason);
                DAL.me.InsertMesInterfaceLog("A099", root.Header.SessionID,
                    DateTime.ParseExact(root.Header.RequestTime, "yyyy-MM-dd HH:mm:ss fff", System.Globalization.CultureInfo.CurrentCulture).ToString("yyyy-MM-dd HH:mm:ss.fff"),
                     null, jason, error);
                return null;
            }
        }

        #endregion

        public class RecvdData
        {
            public string dstIp;
            public string srcIp;
            public string sendTime;
            public string cmd;
            public string jason;
            public string receivedTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff");
            
        }
    }
}
