using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Input;
using Shuyz.Framework.Mvvm;
using ZY.Logging;
using XRayClient.VisionSysWrapper;
using XRayClient.Core;
using XRayClient.Core.Converters;
using ZY.BarCodeReader;
using ZYXray.Models;
using ATL.MES;
using ATL.Core;
using XRayClient.BatteryCheckManager;
using System.IO;
using ZY.MitutoyoReader;
using ZY.XRayTube;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Net.Sockets;
using MessageBox = System.Windows.Forms.MessageBox;
using System.IO.Ports;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using Instrument;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Forms.VisualStyles;
using ZY.Vision;
using System.Configuration;

namespace ZYXray.ViewModels
{
    public class MotionControlVm : ObservableObject
    {
        private static MotionControlVm _instance = new MotionControlVm();
        private static ATL.Engine.PLC plc = new ATL.Engine.PLC();
        private static Bis bis = new Bis();
        StressStatePage stressPage = null;

        public static BT3562 Bt3562;//内阻仪器
        public static E5CC E5cc;//环境温度仪器
        public static TCP34461A Tcp34461A;//电压仪器
        public static LR8450 Lr8450;//IV仪器
        public static RaytekMI3 Mi3_1;//红外测温仪1
        public static RaytekMI3 Mi3_2;//红外测温仪1
        public static ShangLingVision vision;

        private static Queue<string> queueGRRBarcode = new Queue<string>();//电池条码堆，GRR模式用
        private static Queue<string> queueGRRBarMdiCode = new Queue<string>();
        private static bool isScanGRRFinish = true;
        private static bool isOCVGRRFinish = true;
        private static bool isMDIAndPPGGRRFinish = true;
        private static bool isIVGRRFinish = true;
        private static bool isXray1GRRFinish = true;
        private static bool isXRAYGRRMDIFinish = true;

        private static bool isPPGFinished = true;

        private Dictionary<int, string> dicBarcode1 = new Dictionary<int, string>();
        private Dictionary<int, string> dicBarcode2 = new Dictionary<int, string>();

        //每一个工位建一个堆，电池先进先出
        private static Queue<BatterySeat> queueScanStation1 = new Queue<BatterySeat>();
        private static Queue<BatterySeat> queueScanStation2 = new Queue<BatterySeat>();
        private static Queue<BatterySeat> queueIVStation1 = new Queue<BatterySeat>();
        private static Queue<BatterySeat> queueIVStation2 = new Queue<BatterySeat>();
        private static Queue<BatterySeat> queueOCVStation1 = new Queue<BatterySeat>();
        private static Queue<BatterySeat> queueOCVStation2 = new Queue<BatterySeat>();
        private static Queue<BatterySeat> queueMDIStation1 = new Queue<BatterySeat>();
        private static Queue<BatterySeat> queueMDIStation2 = new Queue<BatterySeat>();
        private static Queue<BatterySeat> queueThickness = new Queue<BatterySeat>();
        private static Queue<BatterySeat> queueXRAY1Station = new Queue<BatterySeat>();
        private static Queue<BatterySeat> queueXRAY2Station = new Queue<BatterySeat>();

        private static Queue<BatterySeat> queueNoPPGOfMDIProduct = new Queue<BatterySeat>();
        private static int scanGrrTime = 0;
        private static bool isOpenBis = false;
        public static string OperaterId = "123";
        public static string Model = "";
        private static string machinNo = CheckParamsConfig.Instance.PcName;
        private static string oldMDIData = "";
        private static bool isCCDBusy = false;
        private static bool isABCell = false;//是否AB面电池
        private static bool isXrayThreadOpen = false;

        //private static string machinNo = UserDefineVariableInfo.DicVariables["AssetsNO"].ToString();///<summary>保存测试图片</summary>

        //TODO: 保存图片配置项  ZhangKF 2021-3-16
        static readonly string SAVE_TEST_IMAGE = ConfigurationManager.AppSettings["SaveTestImage"];

        public static MotionControlVm Instance
        {
            get { return _instance; }
        }

        public CncButton MyCncButton
        {
            get { return CncButton.Instance; }
        }

        public PlcSignalState MyPlcSignalState
        {
            get { return PlcSignalState.Instance; }
        }

        public CameraSetingsVm MyCameraSetting
        {
            get { return CameraSetingsVm.Instance; }
        }

        public string CCDData
        {
            get { return _ccdData; }
            set
            {
                _ccdData = value;
                RaisePropertyChanged("CCDData");
            }
        }

        public int loglines = 0;
        private bool _isReset = false;
        private bool _isScanBarcode = false;
        private bool _isGetACorner = false;
        private bool _isGetBCorner = false;
        private bool _isThicknessMsr = false;
        private bool _isDimension = false;
        private bool _isGetNGPostion = false;
        private static string _logData = string.Empty;
        private string _ccdData = "";
        private int _reworkMode = 0;

        public static int lengthArray = 30;

        private static ICheckLogicExt _extRunEmpty = new CheckLogicExtRunEmpty();
        private static ICheckLogicExt _extSTF = new CheckLogicExtSTF();
        private static BatterySeat _resultSeat1 = new BatterySeat();
        private static CheckStatus _checkStatus = BotIF.MyCheckStatus;
        private static WorkingSeats _workingSeats = new WorkingSeats();
        private static CameraParams _camParam1 = new CameraParams();
        private static CameraParams _camParam2 = new CameraParams();
        private static ImageSaveConfig _imageSaveConfig = new ImageSaveConfig();
        private static bool _AutoRunning = false;
        private TestCodeManager testCodeManager = null;

        private static List<string> lastCheckOutBarcode = new List<string>();
        private static int checkOutTimes = 0;

        public static int m_uBarcodeIndex = 0;
        public static int m_uThicknessIndex = 0;
        public static int m_uDimensionIndex = 0;
        public static int m_uOCVIndex = 0;
        public static int m_uCameraOneIndexA = 0;
        public static int m_uCameraOneIndexB = 0;
        public static int m_uImageTestOneIndexA = 0;
        public static int m_uImageTestOneIndexB = 0;
        public static int m_uCameraOneIndexC = 0;
        public static int m_uCameraOneIndexD = 0;
        public static int m_uImageTestOneIndexC = 0;
        public static int m_uImageTestOneIndexD = 0;

        public static int m_uNGIndex = 0;

        public static bool m_bAlgoCorner3Finished = false;
        public static bool m_bAlgoCorner1Finished = false;
        public static bool m_bAlgoCorner2Finished = false;
        public static bool m_bAlgoCorner4Finished = false;

        public static int _XRayTubeIdleTime = 0;
        //public static bool 
        public static string _algoInitFinHint = "算法初始化中";
        string testMode = string.Empty;//
        bool _grrXRayResult = true;
        bool _grrThicknessResult = true;
        bool _isBatInfoError = false;//电池信息是否发生错乱

        private List<float> listPPGdata1 = new List<float>();
        private List<float> listPPGdata2 = new List<float>();
        private List<float> listPPGdata3 = new List<float>();
        private List<float> listPPGdata4 = new List<float>();


        //int layerAC = int.Parse(UserDefineVariableInfo.DicVariables["LayerACNum"].ToString());
        //int layerBD = int.Parse(UserDefineVariableInfo.DicVariables["LayerBDNum"].ToString());
        private bool isBarcodeBinding1Finished = true;
        private bool isOcv1Finished = true;
        private int oldTempBarcode = 0;
        private int algoMode = 3;//算法模式,3为TIFF算法，4为传统算法
        private bool canDoIV = false;
        private bool canDoOCV = false;
        private bool canDoMDI = false;
        private bool canDoPPG = false;
        private bool canDoXRAY = false;

        public bool IsReset
        {
            get
            {
                return this._isReset;
            }
            set
            {
                this._isReset = value;
                RaisePropertyChanged("IsReset");
            }
        }
        public bool IsScanBarcode
        {
            get
            {
                return this._isScanBarcode;
            }
            set
            {
                this._isScanBarcode = value;
                RaisePropertyChanged("IsScanBarcode");
            }
        }
        public bool IsGetACorner
        {
            get
            {
                return this._isGetACorner;
            }
            set
            {
                this._isGetACorner = value;
                RaisePropertyChanged("IsGetACorner");
            }
        }
        public bool IsGetBCorner
        {
            get
            {
                return this._isGetBCorner;
            }
            set
            {
                this._isGetBCorner = value;
                RaisePropertyChanged("IsGetBCorner");
            }
        }
        public bool IsThicknessMsr
        {
            get
            {
                return this._isThicknessMsr;
            }
            set
            {
                this._isThicknessMsr = value;
                RaisePropertyChanged("IsThicknessMsr");
            }
        }

        public bool IsDimensionMsr
        {
            get
            {
                return this._isDimension;
            }
            set
            {
                this._isDimension = value;
                RaisePropertyChanged("IsDimensionMsr");
            }
        }
        public bool IsGetNGPostion
        {
            get
            {
                return this._isGetNGPostion;
            }
            set
            {
                this._isGetNGPostion = value;
                RaisePropertyChanged("IsGetNGPostion");
            }
        }

        public string AlgoInitFinHint
        {
            get
            {
                return _algoInitFinHint;
            }
            set
            {
                _algoInitFinHint = value;
                RaisePropertyChanged("AlgoInitFinHint");
            }
        }

        public int XRayTubeIdleTime
        {
            get
            {
                return HardwareConfig.Instance.XRayTubeIdleTime;
            }
            set
            {
                HardwareConfig.Instance.XRayTubeIdleTime = value;
                RaisePropertyChanged("XRayTubeIdleTime");
            }
        }

        public string OCVMode
        {
            get { return CheckParamsConfig.Instance.OCVMode; }
            set
            {
                CheckParamsConfig.Instance.OCVMode = value;
                RaisePropertyChanged("OCVMode");
                CheckParamsConfig.Instance.WriteGeneralParams();
            }
        }

        public string LogData
        {
            get
            {
                return _logData;
            }
            set
            {
                loglines = loglines + 1;
                if (loglines > 1000)
                {
                    loglines = 0;
                    _logData = "";
                }
                else
                {
                    _logData = _logData + "[" + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff") + "]  " + value + "\r";
                }

                RaisePropertyChanged("LogData");
            }
        }

        /// <summary>
        /// 工艺路线
        /// 0: OB - XRay - FQI
        /// 1: OC - XRay - FQI
        /// 2: XRay - FQI
        /// 3: FQI
        /// 4: 离线生产
        /// </summary>
        public int ReworkMode
        {
            get { return _reworkMode; }

            set
            {
                _reworkMode = value;
                RaisePropertyChanged("ReworkMode");
            }
        }

        public MotionControlVm()
        {
            //MainWindow.Current.mainPage.Navigate(new DataViewPage());//加载数据表格界面
            //MainWindow.Current.mainPage.Navigate(new ChannelCheckPage());//加载通道点检界面

            testCodeManager = new TestCodeManager();
            testCodeManager.RefreshTestCodeList();
            if (!Directory.Exists("D:\\Test"))
            {
                Directory.CreateDirectory("D:\\Test");
            }
            //XRayTubeStateThread();
            //IsXRayTubeCloseThread();
            AlgoInitFinHint_Thread();
            SoftWareResetThread();
            HeartBeatThread();

            Bt3562 = new BT3562();
            E5cc = new E5CC();
            Mi3_1 = new RaytekMI3();
            Mi3_2 = new RaytekMI3();
            Lr8450 = new LR8450();
            vision = new ShangLingVision();

            Bt3562.InitialBT3562(HardwareConfig.Instance.BT3562SerialPortConfig.PortName, HardwareConfig.Instance.BT3562SerialPortConfig.BaudRate, HardwareConfig.Instance.BT3562SerialPortConfig.DataBits, (Parity)HardwareConfig.Instance.BT3562SerialPortConfig.Parity, (StopBits)HardwareConfig.Instance.BT3562SerialPortConfig.StopBits);
            E5cc.InitialE5CC(HardwareConfig.Instance.E5CCSerialPortConfig.PortName, HardwareConfig.Instance.E5CCSerialPortConfig.BaudRate, HardwareConfig.Instance.E5CCSerialPortConfig.DataBits, (Parity)HardwareConfig.Instance.E5CCSerialPortConfig.Parity, (StopBits)HardwareConfig.Instance.E5CCSerialPortConfig.StopBits);
            Tcp34461A = new TCP34461A(HardwareConfig.Instance.Ip34461A);
            Mi3_1.InitialRaytekMI3(HardwareConfig.Instance.MI3SerialPortConfig1.PortName,
                HardwareConfig.Instance.MI3SerialPortConfig1.BaudRate,
                HardwareConfig.Instance.MI3SerialPortConfig1.DataBits,
                (Parity)HardwareConfig.Instance.MI3SerialPortConfig1.Parity,
                (StopBits)HardwareConfig.Instance.MI3SerialPortConfig1.StopBits);
            Mi3_2.InitialRaytekMI3(HardwareConfig.Instance.MI3SerialPortConfig2.PortName,
                HardwareConfig.Instance.MI3SerialPortConfig2.BaudRate,
                HardwareConfig.Instance.MI3SerialPortConfig2.DataBits,
                (Parity)HardwareConfig.Instance.MI3SerialPortConfig2.Parity,
                (StopBits)HardwareConfig.Instance.MI3SerialPortConfig2.StopBits);
            Lr8450.InitialLR8450(HardwareConfig.Instance.IpLR8401, HardwareConfig.Instance.PortLR8401);
            Lr8450.Initial();
            vision.InitialShangLingVision(HardwareConfig.Instance.DimensionIPAdress, HardwareConfig.Instance.DimensionPort);
            //ThreadPool.QueueUserWorkItem(state => Task_Shot());


        }

        public static bool Init()
        {
            MotionControlVm.Instance.LoadWorkParams(CheckParamsConfig.Instance, HardwareConfig.Instance.CameraParams1, HardwareConfig.Instance.CameraParams2);
            BatteryCheckIF.MyTestCodeManager.Init();

            return true;
        }


        private static object lockLog = new object();
        public void WriteLog(string content, LogLevels level = LogLevels.Info, string tag = "")
        {
            lock (lockLog)
            {
                LoggingIF.Log(content, level, tag);
                LogData = content;
                //LogInDB.Error(content);
            }
        }
        public void LoadWorkParams(CheckParamsConfig checkParams, CameraParams camParam1, CameraParams camParam2)
        {
            _camParam1 = camParam1;
            _camParam2 = camParam2;

            _imageSaveConfig = checkParams.MyImageSaveConfig;
            _checkStatus.CheckMode = checkParams.CheckMode;

            _workingSeats._seat1.CheckMode = ECheckModes.FourSides;

            checkParams.MyInspectParams.CopyTo(ref _workingSeats._seat1.Corner1.InspectParams);
            _workingSeats._seat1.Corner1.InspectParams.iCorner = 1;
            _workingSeats._seat1.Corner1.InspectParams.total_layer = checkParams.TotalLayer;
            _workingSeats._seat1.Corner1.InspectParams.detected_rect.width = checkParams.RectWidth;
            _workingSeats._seat1.Corner1.InspectParams.detected_rect.height = checkParams.RectHeight;
            _workingSeats._seat1.Corner1.InspectParams.max_length = checkParams.MaxLengthTail;
            _workingSeats._seat1.Corner1.InspectParams.min_length = checkParams.MinLengthTail;

            checkParams.MyInspectParams.CopyTo(ref _workingSeats._seat1.Corner2.InspectParams);
            _workingSeats._seat1.Corner2.InspectParams.iCorner = 2;
            _workingSeats._seat1.Corner2.InspectParams.total_layer = checkParams.TotalLayersBD;
            _workingSeats._seat1.Corner2.InspectParams.detected_rect.width = checkParams.RectWidthBD;
            _workingSeats._seat1.Corner2.InspectParams.detected_rect.height = checkParams.RectHeightBD;
            _workingSeats._seat1.Corner2.InspectParams.max_length = checkParams.MaxLengthTail;
            _workingSeats._seat1.Corner2.InspectParams.min_length = checkParams.MinLengthTail;

            checkParams.MyInspectParams.CopyTo(ref _workingSeats._seat1.Corner3.InspectParams);
            _workingSeats._seat1.Corner3.InspectParams.iCorner = 3;
            _workingSeats._seat1.Corner3.InspectParams.total_layer = checkParams.TotalLayersBD;
            _workingSeats._seat1.Corner3.InspectParams.detected_rect.width = checkParams.RectWidthBD;
            _workingSeats._seat1.Corner3.InspectParams.detected_rect.height = checkParams.RectHeightBD;
            _workingSeats._seat1.Corner3.InspectParams.max_length = checkParams.MaxLengthHead;
            _workingSeats._seat1.Corner3.InspectParams.min_length = checkParams.MinLengthHead;

            checkParams.MyInspectParams.CopyTo(ref _workingSeats._seat1.Corner4.InspectParams);
            _workingSeats._seat1.Corner4.InspectParams.iCorner = 4;
            _workingSeats._seat1.Corner4.InspectParams.total_layer = checkParams.TotalLayer;
            _workingSeats._seat1.Corner4.InspectParams.detected_rect.width = checkParams.RectWidth;
            _workingSeats._seat1.Corner4.InspectParams.detected_rect.height = checkParams.RectHeight;
            _workingSeats._seat1.Corner4.InspectParams.max_length = checkParams.MaxLengthHead;
            _workingSeats._seat1.Corner4.InspectParams.min_length = checkParams.MinLengthHead;


            //
            _workingSeats._seat2.CheckMode = ECheckModes.FourSides;

            checkParams.MyInspectParams.CopyTo(ref _workingSeats._seat2.Corner1.InspectParams);
            _workingSeats._seat2.Corner1.InspectParams.iCorner = 1;
            _workingSeats._seat2.Corner1.InspectParams.total_layer = checkParams.TotalLayersBD;
            _workingSeats._seat2.Corner1.InspectParams.detected_rect.width = checkParams.RectWidthBD;
            _workingSeats._seat2.Corner1.InspectParams.detected_rect.height = checkParams.RectHeightBD;
            _workingSeats._seat2.Corner1.InspectParams.max_length = checkParams.MaxLengthTail;
            _workingSeats._seat2.Corner1.InspectParams.min_length = checkParams.MinLengthTail;

            checkParams.MyInspectParams.CopyTo(ref _workingSeats._seat2.Corner2.InspectParams);
            _workingSeats._seat2.Corner2.InspectParams.iCorner = 2;
            _workingSeats._seat2.Corner2.InspectParams.total_layer = checkParams.TotalLayer;
            _workingSeats._seat2.Corner2.InspectParams.detected_rect.width = checkParams.RectWidth;
            _workingSeats._seat2.Corner2.InspectParams.detected_rect.height = checkParams.RectHeight;
            _workingSeats._seat2.Corner2.InspectParams.max_length = checkParams.MaxLengthTail;
            _workingSeats._seat2.Corner2.InspectParams.min_length = checkParams.MinLengthTail;

            checkParams.MyInspectParams.CopyTo(ref _workingSeats._seat2.Corner3.InspectParams);
            _workingSeats._seat2.Corner3.InspectParams.iCorner = 3;
            _workingSeats._seat2.Corner3.InspectParams.total_layer = checkParams.TotalLayer;
            _workingSeats._seat2.Corner3.InspectParams.detected_rect.width = checkParams.RectWidth;
            _workingSeats._seat2.Corner3.InspectParams.detected_rect.height = checkParams.RectHeight;
            _workingSeats._seat2.Corner3.InspectParams.max_length = checkParams.MaxLengthHead;
            _workingSeats._seat2.Corner3.InspectParams.min_length = checkParams.MinLengthHead;

            checkParams.MyInspectParams.CopyTo(ref _workingSeats._seat2.Corner4.InspectParams);
            _workingSeats._seat2.Corner4.InspectParams.iCorner = 4;
            _workingSeats._seat2.Corner4.InspectParams.total_layer = checkParams.TotalLayersBD;
            _workingSeats._seat2.Corner4.InspectParams.detected_rect.width = checkParams.RectWidthBD;
            _workingSeats._seat2.Corner4.InspectParams.detected_rect.height = checkParams.RectHeightBD;
            _workingSeats._seat2.Corner4.InspectParams.max_length = checkParams.MaxLengthHead;
            _workingSeats._seat2.Corner4.InspectParams.min_length = checkParams.MinLengthHead;

            _workingSeats._seat2.Corner1.InspectParams.pixel_to_mm = _camParam1.pixelRatio;
            _workingSeats._seat2.Corner2.InspectParams.pixel_to_mm = _camParam1.pixelRatio;

            _workingSeats._seat2.Corner3.InspectParams.pixel_to_mm = _camParam2.pixelRatio;
            _workingSeats._seat2.Corner4.InspectParams.pixel_to_mm = _camParam2.pixelRatio;
            //


            //_workingSeats._seat1.Corner1.InspectParams.max_length = float.Parse(UserDefineVariableInfo.DicVariables["TestSpec_up"].ToString());
            //_workingSeats._seat1.Corner1.InspectParams.min_length = float.Parse(UserDefineVariableInfo.DicVariables["TestSpec_down"].ToString());
            //_workingSeats._seat1.Corner2.InspectParams.max_length = float.Parse(UserDefineVariableInfo.DicVariables["TestSpec_up"].ToString());
            //_workingSeats._seat1.Corner2.InspectParams.min_length = float.Parse(UserDefineVariableInfo.DicVariables["TestSpec_down"].ToString());
            //_workingSeats._seat1.Corner3.InspectParams.max_length = float.Parse(UserDefineVariableInfo.DicVariables["TestSpec_up"].ToString());
            //_workingSeats._seat1.Corner3.InspectParams.min_length = float.Parse(UserDefineVariableInfo.DicVariables["TestSpec_down"].ToString());
            //_workingSeats._seat1.Corner4.InspectParams.max_length = float.Parse(UserDefineVariableInfo.DicVariables["TestSpec_up"].ToString());
            //_workingSeats._seat1.Corner4.InspectParams.min_length = float.Parse(UserDefineVariableInfo.DicVariables["TestSpec_down"].ToString());
            //_workingSeats._seat1.Corner1.InspectParams.total_layer = int.Parse(UserDefineVariableInfo.DicVariables["LayerACNum"].ToString());
            //_workingSeats._seat1.Corner4.InspectParams.total_layer = int.Parse(UserDefineVariableInfo.DicVariables["LayerACNum"].ToString());
            //_workingSeats._seat1.Corner2.InspectParams.total_layer = int.Parse(UserDefineVariableInfo.DicVariables["LayerBDNum"].ToString());
            //_workingSeats._seat1.Corner3.InspectParams.total_layer = int.Parse(UserDefineVariableInfo.DicVariables["LayerBDNum"].ToString());
            //checkParams.MaxThickness = float.Parse(UserDefineVariableInfo.DicVariables["Thickness_up"].ToString());
            //checkParams.MinThickness = float.Parse(UserDefineVariableInfo.DicVariables["Thickness_down"].ToString());

            _workingSeats._seat1.Corner1.InspectParams.pixel_to_mm = _camParam1.pixelRatio;
            _workingSeats._seat1.Corner2.InspectParams.pixel_to_mm = _camParam1.pixelRatio;

            _workingSeats._seat1.Corner3.InspectParams.pixel_to_mm = _camParam2.pixelRatio;
            _workingSeats._seat1.Corner4.InspectParams.pixel_to_mm = _camParam2.pixelRatio;

            //CheckParamsConfig.Instance.MaxLengthHead = float.Parse(UserDefineVariableInfo.DicVariables["TestSpec_up"].ToString());
            //CheckParamsConfig.Instance.MinLengthHead = float.Parse(UserDefineVariableInfo.DicVariables["TestSpec_down"].ToString());
            //CheckParamsConfig.Instance.MaxLengthTail = float.Parse(UserDefineVariableInfo.DicVariables["TestSpec_up"].ToString());
            //CheckParamsConfig.Instance.MinLengthTail = float.Parse(UserDefineVariableInfo.DicVariables["TestSpec_down"].ToString());
            //CheckParamsConfig.Instance.TotalLayer = int.Parse(UserDefineVariableInfo.DicVariables["LayerACNum"].ToString());
            //CheckParamsConfig.Instance.TotalLayersBD = int.Parse(UserDefineVariableInfo.DicVariables["LayerBDNum"].ToString());
            //CheckParamsConfig.Instance.MaxThickness = float.Parse(UserDefineVariableInfo.DicVariables["Thickness_up"].ToString());
            //CheckParamsConfig.Instance.MinThickness = float.Parse(UserDefineVariableInfo.DicVariables["Thickness_down"].ToString());
            CheckParamsConfig.Instance.Write();
        }

        public ICommand StartTest
        {
            get
            {
                return new RelayCommand(new Action(delegate
                {
                    string version = GetVersion("ATL_MES.exe");
                    UserDefineVariableInfo.DicVariables["LabVersion"] = version;
                    UserDefineVariableInfo.DicVariables["AlgoVersion"] = plc.ReadByVariableName("AlgoVersionLocal");
                    UserDefineVariableInfo.DicVariables["CCDVersion"] = "3.02.6";

                    plc.WriteByVariableName("SoftwareOnState", 1);

                    if (Int16.Parse(UserDefineVariableInfo.DicVariables["AlgoInitFinished"].ToString()) != 1)
                    {
                        System.Windows.MessageBox.Show("算法未初始化完成!", "提示", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.None,
                            System.Windows.MessageBoxResult.OK, System.Windows.MessageBoxOptions.ServiceNotification);
                        return;
                    }

                    string workroute = string.Empty;
                    if (ReworkMode == 0)
                        workroute = "OB - XRay - FQI";
                    else if (ReworkMode == 1)
                        workroute = "OC - XRay - FQI";
                    else if (ReworkMode == 2)
                        workroute = "XRay - FQI";
                    else if (ReworkMode == 3)
                        workroute = "FQI";
                    else if (ReworkMode == 4)
                        workroute = "离线生产";
                    if (System.Windows.Forms.DialogResult.No == MessageBox.Show("当前工艺路线为：" + workroute + "，是否继续？", "提示", System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.None,
                        System.Windows.Forms.MessageBoxDefaultButton.Button1, System.Windows.Forms.MessageBoxOptions.ServiceNotification))
                    {
                        return;
                    }

                    if (!XRayTubeIF.XRayTube1Stauts.ShouldXrayOn)//光管未打开
                    {
                        System.Windows.Forms.DialogResult dr =
                        System.Windows.Forms.MessageBox.Show("光管未打开，是否继续启动？", "提示", System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.None,
                        System.Windows.Forms.MessageBoxDefaultButton.Button1, System.Windows.Forms.MessageBoxOptions.ServiceNotification);
                        if (dr == System.Windows.Forms.DialogResult.No)
                            return;
                    }

                    string user = WinAPI.GetCurrentUser();
                    user = user.ToUpper();
                    if (user.EndsWith("\\ND-PE"))
                    {
                        System.Windows.MessageBox.Show("当前账号为ND-PE, 不允许生产, 请注销后用产线生产账号登入!", "提示", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.None,
                            System.Windows.MessageBoxResult.OK, System.Windows.MessageBoxOptions.ServiceNotification);
                        return;
                    }

                    //预警
                    int xrayusetime = Int16.Parse(UserDefineVariableInfo.DicVariables["GG00_curr"].ToString());
                    int xrayexpecttime = Int16.Parse(UserDefineVariableInfo.DicVariables["GG00_expect"].ToString());
                    int xrayexpecttime2 = Int16.Parse(UserDefineVariableInfo.DicVariables["ZQQ0_expect"].ToString());

                    if (xrayusetime > xrayexpecttime || xrayusetime > xrayexpecttime2)
                    {
                        System.Windows.MessageBox.Show(string.Format("光管使用时间为 {0} 小时 超出寿命预警时间 {1} 小时, 请停机找ME更换光管! ", xrayusetime, xrayexpecttime), "提示", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.None,
                            System.Windows.MessageBoxResult.OK, System.Windows.MessageBoxOptions.ServiceNotification);
                        return;
                    }

                    //检查算法版本是否match
                    bool ret = CheckAlgoVersion();
                    if (ret == false)
                    {
                        System.Windows.MessageBox.Show("当前算法版本与MES下发的算法版本不一致，不允许开机!", "提示", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.None,
                            System.Windows.MessageBoxResult.OK, System.Windows.MessageBoxOptions.ServiceNotification);
                        return;
                    }

                    //是否有开机指令
                    if (DeivceEquipmentidPlcInfo.lstDeivceEquipmentidPlcInfos[0].ControlCode == null || DeivceEquipmentidPlcInfo.lstDeivceEquipmentidPlcInfos[0].ControlCode.ToUpper() != "RUN")
                    {
                        if (ReworkMode != 4)
                        {
                            System.Windows.MessageBox.Show("未收到MES开机指令，不允许开机!", "提示", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.None,
                            System.Windows.MessageBoxResult.OK, System.Windows.MessageBoxOptions.ServiceNotification);
                            return;
                        }
                    }

                    if (MyCameraSetting.IsVideoOn == true)
                    {
                        System.Windows.MessageBox.Show("请先关闭相机标定页面的视频!", "提示", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.None,
                            System.Windows.MessageBoxResult.OK, System.Windows.MessageBoxOptions.ServiceNotification);
                        return;
                    }
                    //先发离线 output
                    //BatteryCheckIF.MyProductionDataOffline.RefreshProductionDataList();
                    //if (BatteryCheckIF.MyProductionDataOffline.ProductionDataList.Count > 0)
                    //{
                    //    foreach (ProductionDataXray productData in BatteryCheckIF.MyProductionDataOffline.ProductionDataList)
                    //    {
                    //        ATL.MES.A014.Root _root = InterfaceClient.Current.A013Offline(productData.ProductSN, productData.JsonData);

                    //        if (_root != null)
                    //        {
                    //            BatteryCheckIF.MyProductionDataOffline.RemoveProductionData(productData.ProductSN);
                    //        }
                    //    }
                    //}

                    testCodeManager.RefreshTestCodeList();

                    WriteLog("启动检测", LogLevels.Info, "StartTest");

                    plc.WriteByVariableName("BatteryScanning1OK", 0);
                    plc.WriteByVariableName("BatteryScanning1NG", 0);
                    plc.WriteByVariableName("BatteryScanning2OK", 0);
                    plc.WriteByVariableName("BatteryScanning2NG", 0);
                    plc.WriteByVariableName("IVComplete", 0);
                    plc.WriteByVariableName("ResistanceComplete1", 0);
                    plc.WriteByVariableName("ResistanceComplete2", 0);
                    plc.WriteByVariableName("VoltageComplete1", 0);
                    plc.WriteByVariableName("VoltageComplete2", 0);
                    plc.WriteByVariableName("MDIComplete1", 0);
                    plc.WriteByVariableName("MDIComplete2", 0);
                    plc.WriteByVariableName("ThicknessComplete1", 0);
                    plc.WriteByVariableName("ThicknessComplete2", 0);
                    plc.WriteByVariableName("ThicknessComplete3", 0);
                    plc.WriteByVariableName("ThicknessComplete4", 0);
                    plc.WriteByVariableName("Battery1CornerPhotoComplete", 0);
                    plc.WriteByVariableName("Battery2CornerPhotoComplete", 0);
                    plc.WriteByVariableName("Battery3CornerPhotoComplete", 0);
                    plc.WriteByVariableName("Battery4CornerPhotoComplete", 0);
                    plc.WriteByVariableName("ResultOK", 0);
                    plc.WriteByVariableName("IVNG", 0);
                    plc.WriteByVariableName("IVNoConduction", 0);
                    plc.WriteByVariableName("OtherNG", 0);
                    plc.WriteByVariableName("VoltageNG", 0);
                    plc.WriteByVariableName("ResistanceNG", 0);
                    plc.WriteByVariableName("KNG", 0);
                    plc.WriteByVariableName("MDING", 0);
                    plc.WriteByVariableName("ThicknessNG", 0);
                    plc.WriteByVariableName("XRAYNG", 0);

                    MyCncButton.IsStartBtnEnable = false;
                    _AutoRunning = true;


                    Init();

                    WriteLog("当前工艺路线为：" + workroute, LogLevels.Info, "StartTest");

                    MainControlThread();
                    plc.WriteByVariableName("ParentEQState", 22);

                    //string plcVersion = plc.ReadByVariableName("PLCversion");
                    string hmiVersion = plc.ReadByVariableName("HMIversion");
                    string softVersion = UserDefineVariableInfo.DicVariables["SoftVersion"].ToString();
                    //HardwareConfig.Instance.SaveSoftWareVersionConfig(plcVersion, hmiVersion, softVersion);
                    //UserDefineVariableInfo.DicVariables["ParentEQState"] = "Run";
                    //ATL.MES.A020.Root root = InterfaceClient.Current.A019();

                    MyCncButton.IsStopBtnEnable = true;

                    if (_AutoRunning == true)
                    {
                        ThreadPool.QueueUserWorkItem(state => Task_ScanBarcode1());
                        ThreadPool.QueueUserWorkItem(state => Task_ScanBarcode2());
                        ThreadPool.QueueUserWorkItem(state => Task_BarcodeBinding1());
                        ThreadPool.QueueUserWorkItem(state => Task_BarcodeBinding2());
                        ThreadPool.QueueUserWorkItem(state => Task_IV());
                        ThreadPool.QueueUserWorkItem(state => Task_OCV1());
                        ThreadPool.QueueUserWorkItem(state => Task_OCV2());
                        ThreadPool.QueueUserWorkItem(state => Task_FQI1());
                        ThreadPool.QueueUserWorkItem(state => Task_FQI2());
                        ThreadPool.QueueUserWorkItem(state => Task_Thickness1());
                        ThreadPool.QueueUserWorkItem(state => Task_Thickness2());
                        ThreadPool.QueueUserWorkItem(state => Task_XrayShot1());
                        ThreadPool.QueueUserWorkItem(state => Task_XrayShot2());
                        ThreadPool.QueueUserWorkItem(state => Task_Sorting());
                        ThreadPool.QueueUserWorkItem(state => Task_RemoveProduct());
                        ThreadPool.QueueUserWorkItem(state => Task_GrrMdiPosition());
                    }
                    if (isXrayThreadOpen == false)
                    {
                        ThreadPool.QueueUserWorkItem(state => XrayCheckScan());
                        ThreadPool.QueueUserWorkItem(state => XrayCheck());
                        ThreadPool.QueueUserWorkItem(state => XrayCheckOnFQI());
                        ThreadPool.QueueUserWorkItem(state => Task_NGUnload());

                        isXrayThreadOpen = true;
                    }

                    plc.WriteByVariableName("PCVersion", UserDefineVariableInfo.DicVariables["LabVersion"]);
                    plc.WriteByVariableName("CCDVersion", "V3.1.1.1");
                    plc.WriteByVariableName("AlgoVersion", UserDefineVariableInfo.DicVariables["AlgoVersionLocal"]);


                }), delegate
                {
                    return true;
                });
            }
        }

        public ICommand StopTest
        {
            get
            {
                return new RelayCommand(new Action(delegate
                {
                    _AutoRunning = false;
                    Thread.Sleep(50);

                    plc.WriteByVariableName("ParentEQState", 11);
                    //UserDefineVariableInfo.DicVariables["ParentEQState"] = "Stop";
                    //ATL.MES.A020.Root root = InterfaceClient.Current.A019();

                    //SoftWareReset();
                    MyCncButton.IsStopBtnEnable = false;
                    MyCncButton.IsStartBtnEnable = true;
                }), delegate
                {
                    return true;
                });
            }
        }

        public ICommand CleanLog
        {
            get
            {
                return new RelayCommand(new Action(delegate
                {
                    loglines = 1001;
                    LogData = "";
                }), delegate
                {
                    return true;
                });
            }
        }

        public void HeartBeatThread()
        {
            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;

                while (!ATL.Core.Core.SoftClosing)
                {
                    plc.WriteByVariableName("SoftwareOnState", 1);
                    if (ATL.Station.Station.Current.MESState.Contains("OK"))
                    {
                        plc.WriteByVariableName("MESstatusToPLC", 1);//MES在线心跳                     
                    }
                    else
                    {
                        plc.WriteByVariableName("MESstatusToPLC", 0);
                    }

                    if (MyPlcSignalState.HeartBeat)
                    {
                        Thread.Sleep(500);
                        plc.WriteByVariableName("SoftwareHeartbeatPackage", 0);
                    }
                    else
                    {
                        Thread.Sleep(500);
                        plc.WriteByVariableName("SoftwareHeartbeatPackage", 1);
                    }

                    if (XRayTubeIF.XRayTube1Stauts.ShouldXrayOn)
                    {
                        plc.WriteByVariableName("XRayTubeOnState", 1);
                    }
                    else
                    {
                        plc.WriteByVariableName("XRayTubeOnState", 0);
                    }

                    if (plc.ReadByVariableName("SoftwareHeartbeatPackage") == "1")
                    {
                        PlcSignalState.Instance.HeartBeat = true;
                    }
                    else
                    {
                        PlcSignalState.Instance.HeartBeat = false;
                    }

                    if (plc.ReadByVariableName("XRayTubeOnState") == "1")
                    {
                        PlcSignalState.Instance.XRayON = true;
                    }
                    else
                    {
                        PlcSignalState.Instance.XRayON = false;
                    }

                    if (plc.ReadByVariableName("SoftwareOnState") == "1")
                    {
                        PlcSignalState.Instance.SoftwareOnState = true;
                    }
                    else
                    {
                        PlcSignalState.Instance.SoftwareOnState = false;
                    }

                    ///////////////////////////////////////////////////////////
                    if (plc.ReadByVariableName("SoftwareReset") == "1")
                    {
                        PlcSignalState.Instance.SoftwareReset = true;
                    }
                    else
                    {
                        PlcSignalState.Instance.SoftwareReset = false;
                    }

                    if (plc.ReadByVariableName("ScanBarcodeSignal1") == "1")
                    {
                        PlcSignalState.Instance.ScanBarcodeSignal1 = true;
                    }
                    else
                    {
                        PlcSignalState.Instance.ScanBarcodeSignal1 = false;
                    }
                    if (plc.ReadByVariableName("ScanBarcodeSignal2") == "1")
                    {
                        PlcSignalState.Instance.ScanBarcodeSignal2 = true;
                    }
                    else
                    {
                        PlcSignalState.Instance.ScanBarcodeSignal2 = false;
                    }
                    if (plc.ReadByVariableName("IVTestSignal1") == "1")
                    {
                        PlcSignalState.Instance.IVTestSignal1 = true;
                    }
                    else
                    {
                        PlcSignalState.Instance.IVTestSignal1 = false;
                    }
                    if (plc.ReadByVariableName("IVTestSignal2") == "1")
                    {
                        PlcSignalState.Instance.IVTestSignal2 = true;
                    }
                    else
                    {
                        PlcSignalState.Instance.IVTestSignal2 = false;
                    }
                    if (plc.ReadByVariableName("IVTestSignal3") == "1")
                    {
                        PlcSignalState.Instance.IVTestSignal3 = true;
                    }
                    else
                    {
                        PlcSignalState.Instance.IVTestSignal3 = false;
                    }
                    if (plc.ReadByVariableName("IVTestSignal4") == "1")
                    {
                        PlcSignalState.Instance.IVTestSignal4 = true;
                    }
                    else
                    {
                        PlcSignalState.Instance.IVTestSignal4 = false;
                    }
                    if (plc.ReadByVariableName("ResistanceSignal1") == "1")
                    {
                        PlcSignalState.Instance.ResistanceSignal1 = true;
                    }
                    else
                    {
                        PlcSignalState.Instance.ResistanceSignal1 = false;
                    }
                    if (plc.ReadByVariableName("ResistanceSignal2") == "1")
                    {
                        PlcSignalState.Instance.ResistanceSignal2 = true;
                    }
                    else
                    {
                        PlcSignalState.Instance.ResistanceSignal2 = false;
                    }
                    if (plc.ReadByVariableName("VoltageSignal1") == "1")
                    {
                        PlcSignalState.Instance.VoltageSignal1 = true;
                    }
                    else
                    {
                        PlcSignalState.Instance.VoltageSignal1 = false;
                    }
                    if (plc.ReadByVariableName("VoltageSignal2") == "1")
                    {
                        PlcSignalState.Instance.VoltageSignal2 = true;
                    }
                    else
                    {
                        PlcSignalState.Instance.VoltageSignal2 = false;
                    }
                    if (plc.ReadByVariableName("MDISignal1") == "1")
                    {
                        PlcSignalState.Instance.MDISignal1 = true;
                    }
                    else
                    {
                        PlcSignalState.Instance.MDISignal1 = false;
                    }
                    if (plc.ReadByVariableName("MDISignal2") == "1")
                    {
                        PlcSignalState.Instance.MDISignal2 = true;
                    }
                    else
                    {
                        PlcSignalState.Instance.MDISignal2 = false;
                    }
                    if (plc.ReadByVariableName("MDISignal3") == "1")
                    {
                        PlcSignalState.Instance.MDISignal3 = true;
                    }
                    else
                    {
                        PlcSignalState.Instance.MDISignal3 = false;
                    }
                    if (plc.ReadByVariableName("MDISignal4") == "1")
                    {
                        PlcSignalState.Instance.MDISignal4 = true;
                    }
                    else
                    {
                        PlcSignalState.Instance.MDISignal4 = false;
                    }
                    if (plc.ReadByVariableName("ThicknessMeasureSignal1") == "1")
                    {
                        PlcSignalState.Instance.ThicknessMeasureSignal1 = true;
                    }
                    else
                    {
                        PlcSignalState.Instance.ThicknessMeasureSignal1 = false;
                    }
                    if (plc.ReadByVariableName("ThicknessMeasureSignal2") == "1")
                    {
                        PlcSignalState.Instance.ThicknessMeasureSignal2 = true;
                    }
                    else
                    {
                        PlcSignalState.Instance.ThicknessMeasureSignal2 = false;
                    }
                    if (plc.ReadByVariableName("ThicknessMeasureSignal3") == "1")
                    {
                        PlcSignalState.Instance.ThicknessMeasureSignal3 = true;
                    }
                    else
                    {
                        PlcSignalState.Instance.ThicknessMeasureSignal3 = false;
                    }
                    if (plc.ReadByVariableName("ThicknessMeasureSignal4") == "1")
                    {
                        PlcSignalState.Instance.ThicknessMeasureSignal4 = true;
                    }
                    else
                    {
                        PlcSignalState.Instance.ThicknessMeasureSignal4 = false;
                    }
                    if (plc.ReadByVariableName("Battery1CornerPhotoSignal") == "1")
                    {
                        PlcSignalState.Instance.Battery1CornerPhotoSignal = true;
                    }
                    else
                    {
                        PlcSignalState.Instance.Battery1CornerPhotoSignal = false;
                    }
                    if (plc.ReadByVariableName("Battery2CornerPhotoSignal") == "1")
                    {
                        PlcSignalState.Instance.Battery2CornerPhotoSignal = true;
                    }
                    else
                    {
                        PlcSignalState.Instance.Battery2CornerPhotoSignal = false;
                    }
                    if (plc.ReadByVariableName("Battery3CornerPhotoSignal") == "1")
                    {
                        PlcSignalState.Instance.Battery3CornerPhotoSignal = true;
                    }
                    else
                    {
                        PlcSignalState.Instance.Battery3CornerPhotoSignal = false;
                    }
                    if (plc.ReadByVariableName("Battery4CornerPhotoSignal") == "1")
                    {
                        PlcSignalState.Instance.Battery4CornerPhotoSignal = true;
                    }
                    else
                    {
                        PlcSignalState.Instance.Battery4CornerPhotoSignal = false;
                    }
                    if (plc.ReadByVariableName("PhotoResultSignal") == "1")
                    {
                        PlcSignalState.Instance.PhotoResultSignal = true;
                    }
                    else
                    {
                        PlcSignalState.Instance.PhotoResultSignal = false;
                    }
                    if (plc.ReadByVariableName("NeedleNGSignal1") == "1")
                    {
                        PlcSignalState.Instance.NeedleNGSignal1 = true;
                    }
                    else
                    {
                        PlcSignalState.Instance.NeedleNGSignal1 = false;
                    }
                    if (plc.ReadByVariableName("NeedleNGSignal2") == "1")
                    {
                        PlcSignalState.Instance.NeedleNGSignal2 = true;
                    }
                    else
                    {
                        PlcSignalState.Instance.NeedleNGSignal2 = false;
                    }
                    if (plc.ReadByVariableName("NeedleNGSignal3") == "1")
                    {
                        PlcSignalState.Instance.NeedleNGSignal3 = true;
                    }
                    else
                    {
                        PlcSignalState.Instance.NeedleNGSignal3 = false;
                    }
                    if (plc.ReadByVariableName("NeedleNGSignal4") == "1")
                    {
                        PlcSignalState.Instance.NeedleNGSignal4 = true;
                    }
                    else
                    {
                        PlcSignalState.Instance.NeedleNGSignal4 = false;
                    }
                    if (plc.ReadByVariableName("KnifeNGSignal1") == "1")
                    {
                        PlcSignalState.Instance.KnifeNGSignal1 = true;
                    }
                    else
                    {
                        PlcSignalState.Instance.KnifeNGSignal1 = false;
                    }
                    if (plc.ReadByVariableName("KnifeNGSignal2") == "1")
                    {
                        PlcSignalState.Instance.KnifeNGSignal2 = true;
                    }
                    else
                    {
                        PlcSignalState.Instance.KnifeNGSignal2 = false;
                    }
                    if (plc.ReadByVariableName("KnifeNGSignal3") == "1")
                    {
                        PlcSignalState.Instance.KnifeNGSignal3 = true;
                    }
                    else
                    {
                        PlcSignalState.Instance.KnifeNGSignal3 = false;
                    }
                    if (plc.ReadByVariableName("KnifeNGSignal4") == "1")
                    {
                        PlcSignalState.Instance.KnifeNGSignal4 = true;
                    }
                    else
                    {
                        PlcSignalState.Instance.KnifeNGSignal4 = false;
                    }
                    if (plc.ReadByVariableName("IVPlatform1") == "1")
                    {
                        PlcSignalState.Instance.IVPlatform1 = true;
                    }
                    else
                    {
                        PlcSignalState.Instance.IVPlatform1 = false;
                    }
                    if (plc.ReadByVariableName("IVPlatform2") == "1")
                    {
                        PlcSignalState.Instance.IVPlatform2 = true;
                    }
                    else
                    {
                        PlcSignalState.Instance.IVPlatform2 = false;
                    }
                    if (plc.ReadByVariableName("BarcodebindingSignal1") == "1")
                    {
                        PlcSignalState.Instance.BarcodebindingSignal1 = true;
                    }
                    else
                    {
                        PlcSignalState.Instance.BarcodebindingSignal1 = false;
                    }
                    if (plc.ReadByVariableName("BarcodebindingSignal2") == "1")
                    {
                        PlcSignalState.Instance.BarcodebindingSignal2 = true;
                    }
                    else
                    {
                        PlcSignalState.Instance.BarcodebindingSignal2 = false;
                    }

                    ///////////////////////////////////////////////////////////
                    if (plc.ReadByVariableName("SoftResetComplete") == "1")
                    {
                        PlcSignalState.Instance.SoftResetComplete = true;
                    }
                    else
                    {
                        PlcSignalState.Instance.SoftResetComplete = false;
                    }

                    if (plc.ReadByVariableName("BatteryScanning1OK") == "1")
                    {
                        PlcSignalState.Instance.BatteryScanning1OK = true;
                    }
                    else
                    {
                        PlcSignalState.Instance.BatteryScanning1OK = false;
                    }
                    if (plc.ReadByVariableName("BatteryScanning1NG") == "1")
                    {
                        PlcSignalState.Instance.BatteryScanning1NG = true;
                    }
                    else
                    {
                        PlcSignalState.Instance.BatteryScanning1NG = false;
                    }
                    if (plc.ReadByVariableName("BatteryScanning2OK") == "1")
                    {
                        PlcSignalState.Instance.BatteryScanning2OK = true;
                    }
                    else
                    {
                        PlcSignalState.Instance.BatteryScanning2OK = false;
                    }
                    if (plc.ReadByVariableName("BatteryScanning2NG") == "1")
                    {
                        PlcSignalState.Instance.BatteryScanning2NG = true;
                    }
                    else
                    {
                        PlcSignalState.Instance.BatteryScanning2NG = false;
                    }
                    if (plc.ReadByVariableName("IVComplete") == "1")
                    {
                        PlcSignalState.Instance.IVComplete = true;
                    }
                    else
                    {
                        PlcSignalState.Instance.IVComplete = false;
                    }
                    if (plc.ReadByVariableName("ResistanceComplete1") == "1")
                    {
                        PlcSignalState.Instance.ResistanceComplete1 = true;
                    }
                    else
                    {
                        PlcSignalState.Instance.ResistanceComplete1 = false;
                    }
                    if (plc.ReadByVariableName("ResistanceComplete2") == "1")
                    {
                        PlcSignalState.Instance.ResistanceComplete2 = true;
                    }
                    else
                    {
                        PlcSignalState.Instance.ResistanceComplete2 = false;
                    }
                    if (plc.ReadByVariableName("VoltageComplete1") == "1")
                    {
                        PlcSignalState.Instance.VoltageComplete1 = true;
                    }
                    else
                    {
                        PlcSignalState.Instance.VoltageComplete1 = false;
                    }
                    if (plc.ReadByVariableName("VoltageComplete2") == "1")
                    {
                        PlcSignalState.Instance.VoltageComplete2 = true;
                    }
                    else
                    {
                        PlcSignalState.Instance.VoltageComplete2 = false;
                    }
                    if (plc.ReadByVariableName("MDIComplete1") == "1")
                    {
                        PlcSignalState.Instance.MDIComplete1 = true;
                    }
                    else
                    {
                        PlcSignalState.Instance.MDIComplete1 = false;
                    }
                    if (plc.ReadByVariableName("MDIComplete2") == "1")
                    {
                        PlcSignalState.Instance.MDIComplete2 = true;
                    }
                    else
                    {
                        PlcSignalState.Instance.MDIComplete2 = false;
                    }
                    if (plc.ReadByVariableName("ThicknessComplete1") == "1")
                    {
                        PlcSignalState.Instance.ThicknessComplete1 = true;
                    }
                    else
                    {
                        PlcSignalState.Instance.ThicknessComplete1 = false;
                    }
                    if (plc.ReadByVariableName("ThicknessComplete2") == "1")
                    {
                        PlcSignalState.Instance.ThicknessComplete2 = true;
                    }
                    else
                    {
                        PlcSignalState.Instance.ThicknessComplete2 = false;
                    }
                    if (plc.ReadByVariableName("ThicknessComplete3") == "1")
                    {
                        PlcSignalState.Instance.ThicknessComplete3 = true;
                    }
                    else
                    {
                        PlcSignalState.Instance.ThicknessComplete3 = false;
                    }
                    if (plc.ReadByVariableName("ThicknessComplete4") == "1")
                    {
                        PlcSignalState.Instance.ThicknessComplete4 = true;
                    }
                    else
                    {
                        PlcSignalState.Instance.ThicknessComplete4 = false;
                    }
                    if (plc.ReadByVariableName("Battery1CornerPhotoComplete") == "1")
                    {
                        PlcSignalState.Instance.Battery1CornerPhotoComplete = true;
                    }
                    else
                    {
                        PlcSignalState.Instance.Battery1CornerPhotoComplete = false;
                    }
                    if (plc.ReadByVariableName("Battery2CornerPhotoComplete") == "1")
                    {
                        PlcSignalState.Instance.Battery2CornerPhotoComplete = true;
                    }
                    else
                    {
                        PlcSignalState.Instance.Battery2CornerPhotoComplete = false;
                    }
                    if (plc.ReadByVariableName("Battery3CornerPhotoComplete") == "1")
                    {
                        PlcSignalState.Instance.Battery3CornerPhotoComplete = true;
                    }
                    else
                    {
                        PlcSignalState.Instance.Battery3CornerPhotoComplete = false;
                    }
                    if (plc.ReadByVariableName("Battery4CornerPhotoComplete") == "1")
                    {
                        PlcSignalState.Instance.Battery4CornerPhotoComplete = true;
                    }
                    else
                    {
                        PlcSignalState.Instance.Battery4CornerPhotoComplete = false;
                    }
                    if (plc.ReadByVariableName("ResultOK") == "1")
                    {
                        PlcSignalState.Instance.ResultOK = true;
                    }
                    else
                    {
                        PlcSignalState.Instance.ResultOK = false;
                    }
                    if (plc.ReadByVariableName("IVNG") == "1")
                    {
                        PlcSignalState.Instance.IVNG = true;
                    }
                    else
                    {
                        PlcSignalState.Instance.IVNG = false;
                    }
                    if (plc.ReadByVariableName("IVNoConduction") == "1")
                    {
                        PlcSignalState.Instance.IVNoConduction = true;
                    }
                    else
                    {
                        PlcSignalState.Instance.IVNoConduction = false;
                    }
                    if (plc.ReadByVariableName("OtherNG") == "1")
                    {
                        PlcSignalState.Instance.OtherNG = true;
                    }
                    else
                    {
                        PlcSignalState.Instance.OtherNG = false;
                    }
                    if (plc.ReadByVariableName("VoltageNG") == "1")
                    {
                        PlcSignalState.Instance.VoltageNG = true;
                    }
                    else
                    {
                        PlcSignalState.Instance.VoltageNG = false;
                    }
                    if (plc.ReadByVariableName("ResistanceNG") == "1")
                    {
                        PlcSignalState.Instance.ResistanceNG = true;
                    }
                    else
                    {
                        PlcSignalState.Instance.ResistanceNG = false;
                    }
                    if (plc.ReadByVariableName("KNG") == "1")
                    {
                        PlcSignalState.Instance.KNG = true;
                    }
                    else
                    {
                        PlcSignalState.Instance.KNG = false;
                    }
                    if (plc.ReadByVariableName("MDING") == "1")
                    {
                        PlcSignalState.Instance.MDING = true;
                    }
                    else
                    {
                        PlcSignalState.Instance.MDING = false;
                    }
                    if (plc.ReadByVariableName("ThicknessNG") == "1")
                    {
                        PlcSignalState.Instance.ThicknessNG = true;
                    }
                    else
                    {
                        PlcSignalState.Instance.ThicknessNG = false;
                    }
                    if (plc.ReadByVariableName("XRAYNG") == "1")
                    {
                        PlcSignalState.Instance.XRAYNG = true;
                    }
                    else
                    {
                        PlcSignalState.Instance.XRAYNG = false;
                    }
                    if (plc.ReadByVariableName("BarcodeBindingComplete2") == "1")
                    {
                        PlcSignalState.Instance.BarcodeBindingComplete2 = true;
                    }
                    else
                    {
                        PlcSignalState.Instance.BarcodeBindingComplete2 = false;
                    }
                    if (plc.ReadByVariableName("BarcodeBindingComplete1") == "1")
                    {
                        PlcSignalState.Instance.BarcodeBindingComplete1 = true;
                    }
                    else
                    {
                        PlcSignalState.Instance.BarcodeBindingComplete1 = false;
                    }

                    //给plc写日期
                    plc.WriteByVariableName("Year", DateTime.Now.Year);
                    plc.WriteByVariableName("Month", DateTime.Now.Month);
                    plc.WriteByVariableName("Day", DateTime.Now.Day);
                    plc.WriteByVariableName("Hour", DateTime.Now.Hour);
                    plc.WriteByVariableName("Minutes", DateTime.Now.Minute);
                    plc.WriteByVariableName("Seconds", DateTime.Now.Second);

                    ///////////////////////////////////////////////////////////

                    Thread.Sleep(10);
                }
            }).Start();
        }

        public void MainControlThread()
        {
            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                try
                {
                    int tickCount = 0;
                    while (_AutoRunning == true)
                    {
                        Thread.Sleep(10);
                        if (plc.ReadByVariableName("TestMode") == "2" || plc.ReadByVariableName("TestMode") == "3" || plc.ReadByVariableName("TestMode") == "4" || plc.ReadByVariableName("TestMode") == "5")
                        {
                            if (/*plc.ReadByVariableName("ScanBarcodeSignal1") == "1" &&*/ plc.ReadByVariableName("ScanBarcodeSignal2") == "1")
                            {
                                Thread.Sleep(100);
                                if (isScanGRRFinish == true)
                                {
                                    isScanGRRFinish = false;
                                    if (plc.ReadByVariableName("SingleScanerMode") == "1")
                                    {
                                        ThreadPool.QueueUserWorkItem(state => SingleScanBarcodeGRR());
                                    }
                                    else
                                    {
                                        ThreadPool.QueueUserWorkItem(state => ScanBarcodeGRR());
                                    }
                                }
                            }
                        }
                        if (plc.ReadByVariableName("TestMode") == "2" && isIVGRRFinish == true)
                        {
                            if (plc.ReadByVariableName("IVTestSignal1") == "1" || plc.ReadByVariableName("IVTestSignal2") == "1" || plc.ReadByVariableName("IVTestSignal3") == "1" || plc.ReadByVariableName("IVTestSignal4") == "1")
                            {
                                Thread.Sleep(200);
                                isIVGRRFinish = false;
                                ThreadPool.QueueUserWorkItem(state => IVGRR());
                            }
                        }
                        if (plc.ReadByVariableName("TestMode") == "3")
                        {
                            if (isOCVGRRFinish == true && plc.ReadByVariableName("VoltageSignal2") == "1")
                            {
                                Thread.Sleep(100);
                                isOCVGRRFinish = false;
                                ThreadPool.QueueUserWorkItem(state => OCVGRR());
                            }
                        }
                        if (plc.ReadByVariableName("TestMode") == "4")
                        {
                            if (isMDIAndPPGGRRFinish == true && plc.ReadByVariableName("MDISignal2") == "1")
                            {
                                Thread.Sleep(100);
                                isMDIAndPPGGRRFinish = false;
                                ThreadPool.QueueUserWorkItem(state => MDIAndPPGGRR());
                            }
                        }

                        if (plc.ReadByVariableName("TestMode") == "5")
                        {
                            if (isXray1GRRFinish == true)
                            {
                                Thread.Sleep(100);
                                if (plc.ReadByVariableName("Battery1CornerPhotoSignal") == "1")
                                {
                                    isXray1GRRFinish = false;
                                    ThreadPool.QueueUserWorkItem(state => XRAYGRR());
                                }
                            }
                            if (isXRAYGRRMDIFinish == true)
                            {
                                Thread.Sleep(100);
                                if (plc.ReadByVariableName("MDISignal2") == "1")
                                {
                                    isXRAYGRRMDIFinish = false;
                                    //if ((queueGRRBarMdiCode.Count() >= 4) && isScanGRRFinish == true)
                                    ThreadPool.QueueUserWorkItem(state => XRAYGRRFQI());
                                }
                            }

                        }

                        if (ReworkMode != 4)//isOpenBis == true
                        {
                            if (CheckCanDo(CheckParamsConfig.Instance.IvCheckTime))
                            {
                                canDoIV = true;
                            }
                            else
                            {
                                canDoIV = false;
                            }
                            if (CheckCanDo(CheckParamsConfig.Instance.OcvCheckTime))
                            {
                                canDoOCV = true;
                            }
                            else
                            {
                                canDoOCV = false;
                            }
                            if (CheckCanDo(CheckParamsConfig.Instance.MdiCheckTime))
                            {
                                canDoMDI = true;
                            }
                            else
                            {
                                canDoMDI = false;
                            }
                            if (CheckCanDo(CheckParamsConfig.Instance.PpgCheckTime))
                            {
                                canDoPPG = true;
                            }
                            else
                            {
                                canDoPPG = false;
                            }
                            if (CheckCanDo(CheckParamsConfig.Instance.XrayCheckTime))
                            {
                                canDoXRAY = true;
                            }
                            else
                            {
                                canDoXRAY = false;
                            }
                        }
                        else
                        {
                            canDoIV = true;
                            canDoOCV = true;
                            canDoMDI = true;
                            canDoPPG = true;
                            canDoXRAY = true;
                        }
                        if (CheckParamsConfig.Instance.IsTiffMode == false)
                        {
                            algoMode = 4;
                        }
                        if (ATL.Station.Station.Current.MESState.Contains("OK"))
                        {
                            plc.WriteByVariableName("MesReply", 10);
                        }
                        else
                        {
                            plc.WriteByVariableName("MesReply", 20);
                        }

                    }
                    WriteLog("退出主控线程", LogLevels.Info, "MainControlThread");

                }
                catch (System.Exception ex)
                {
                    WriteLog(string.Format("进入主控线程异常: {0}", ex.Message.ToString()), LogLevels.Info, "MainControlThread");
                }

            }).Start();
        }

        public void SoftWareResetThread()//软件复位线程，把软件复位从主线程移出来，确保停止状态下也能收到复位信号
        {
            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                while (true)
                {
                    if (plc.ReadByVariableName("SoftwareReset") == "1" || plc.ReadByVariableName("SoftwareReset") == "2")
                    {
                        PlcSignalState.Instance.SoftwareReset = true;
                        SoftWareReset();
                    }
                    else
                    {
                        PlcSignalState.Instance.SoftwareReset = false;
                    }
                }
            }).Start();
        }

        public void SoftWareReset()
        {
            WriteLog("收到PLC复位信号", LogLevels.Info, "SoftWareReset");
            plc.WriteByVariableName("SoftResetComplete", 0);

            _isBatInfoError = false;
            isScanGRRFinish = true;
            isOCVGRRFinish = true;
            isMDIAndPPGGRRFinish = true;
            isIVGRRFinish = true;
            isXray1GRRFinish = true;
            isXRAYGRRMDIFinish = true;
            isPPGFinished = true;
            scanGrrTime = 0;
            listOCVGRRBarcode.Clear();

            testCodeManager.RefreshTestCodeList();

            dicBarcode1.Clear();
            dicBarcode2.Clear();
            queueGRRBarcode.Clear();
            oldTempBarcode = 0;

            while (queueScanStation1.Count > 0)
            {
                queueScanStation1.Dequeue().Destroy();
            }
            while (queueScanStation2.Count > 0)
            {
                queueScanStation2.Dequeue().Destroy();
            }
            while (queueIVStation1.Count > 0)
            {
                queueIVStation1.Dequeue().Destroy();
            }
            while (queueIVStation2.Count > 0)
            {
                queueIVStation2.Dequeue().Destroy();
            }
            while (queueOCVStation1.Count > 0)
            {
                queueOCVStation1.Dequeue().Destroy();
            }
            while (queueOCVStation2.Count > 0)
            {
                queueOCVStation2.Dequeue().Destroy();
            }
            while (queueMDIStation1.Count > 0)
            {
                queueMDIStation1.Dequeue().Destroy();
            }
            while (queueMDIStation2.Count > 0)
            {
                queueMDIStation2.Dequeue().Destroy();
            }
            while (queueThickness.Count > 0)
            {
                queueThickness.Dequeue().Destroy();
            }
            while (queueXRAY1Station.Count > 0)
            {
                queueXRAY1Station.Dequeue().Destroy();
            }
            while (queueXRAY2Station.Count > 0)
            {
                queueXRAY2Station.Dequeue().Destroy();
            }
            while (queueNoPPGOfMDIProduct.Count > 0)
            {
                queueNoPPGOfMDIProduct.Dequeue().Destroy();
            }
            queueGRRBarMdiCode.Clear();

            m_bAlgoCorner1Finished = false;
            m_bAlgoCorner2Finished = false;
            m_bAlgoCorner3Finished = false;
            m_bAlgoCorner4Finished = false;
            isCCDBusy = false;

            //plc.WriteByVariableName("XRayTubeOnState", 0);
            plc.WriteByVariableName("BatteryScanning1OK", 0);
            plc.WriteByVariableName("BatteryScanning1NG", 0);
            plc.WriteByVariableName("BatteryScanning2OK", 0);
            plc.WriteByVariableName("BatteryScanning2NG", 0);
            plc.WriteByVariableName("IVComplete", 0);
            plc.WriteByVariableName("ResistanceComplete1", 0);
            plc.WriteByVariableName("ResistanceComplete2", 0);
            plc.WriteByVariableName("VoltageComplete1", 0);
            plc.WriteByVariableName("VoltageComplete2", 0);
            plc.WriteByVariableName("MDIComplete1", 0);
            plc.WriteByVariableName("MDIComplete2", 0);
            plc.WriteByVariableName("ThicknessComplete1", 0);
            plc.WriteByVariableName("ThicknessComplete2", 0);
            plc.WriteByVariableName("ThicknessComplete3", 0);
            plc.WriteByVariableName("ThicknessComplete4", 0);
            plc.WriteByVariableName("Battery1CornerPhotoComplete", 0);
            plc.WriteByVariableName("Battery2CornerPhotoComplete", 0);
            plc.WriteByVariableName("Battery3CornerPhotoComplete", 0);
            plc.WriteByVariableName("Battery4CornerPhotoComplete", 0);
            plc.WriteByVariableName("ResultOK", 0);
            plc.WriteByVariableName("IVNG", 0);
            plc.WriteByVariableName("IVNoConduction", 0);
            plc.WriteByVariableName("OtherNG", 0);
            plc.WriteByVariableName("VoltageNG", 0);
            plc.WriteByVariableName("ResistanceNG", 0);
            plc.WriteByVariableName("KNG", 0);
            plc.WriteByVariableName("MDING", 0);
            plc.WriteByVariableName("ThicknessNG", 0);
            plc.WriteByVariableName("XRAYNG", 0);
            plc.WriteByVariableName("IVConductionComplete", 0);

            Thread.Sleep(500);

            plc.WriteByVariableName("SoftResetComplete", 1);
            string status = "1";
            int waitcount = 0;
            do
            {
                status = plc.ReadByVariableName("SoftwareReset");
                waitcount++;
                Thread.Sleep(50);
                if (waitcount > 100)
                {
                    waitcount = 0;
                    WriteLog(string.Format("第{0}次 等待PLC关闭软件复位触发信号超过5秒", m_uBarcodeIndex), LogLevels.Info, "SoftWareReset");
                }
            }
            while (status != "0");
            plc.WriteByVariableName("SoftResetComplete", 0);

            WriteLog("软件复位完成", LogLevels.Info, "SoftWareReset");
        }

        /// <summary>
        /// 通道1扫码
        /// </summary>
        private void Task_ScanBarcode1()
        {
            //string marking = bis.GetMarking("CE7049C10F56");
            //WriteLog("BIS返回 " + marking, LogLevels.Info, "MotionControlVm");

            //string strOut;
            //string testType = "IV";
            //BatterySeat bt = new BatterySeat();
            //bt.IvData = Convert.ToSingle(0.3);
            //bt.Sn = "BQ7052B014FD";
            //int result = -2;
            ////IV
            //result = bis.TransferDataExTest(bt.Sn, testType, machinNo, "0", bt.IvData, "0", out strOut);
            //WriteLog("BIS返回 " + result + ":" + strOut, LogLevels.Info, "MotionControlVm");

            ////OCV
            //bt.Sn = "AS4102F630DF";
            //machinNo = "A799";
            //bt.Resistance = Convert.ToSingle(16.98);
            //bt.Voltage = Convert.ToSingle(3.800822);
            //bt.Temperature = Convert.ToSingle(25.1);
            //bt.EnvirementTemperature = Convert.ToSingle(25.9);
            //int outResult1;
            //int outResult2;
            //bool ocvResult = bis.AutoTransfData("OB", bt.Sn, bt.Voltage, bt.Resistance, bt.EnvirementTemperature, "1-1",
            //    "none", bt.Voltage, bt.Resistance, bt.EnvirementTemperature,
            //    "1-2", machinNo, out outResult1, out outResult2, bt.Temperature.ToString(), bt.Temperature.ToString(), out strOut);
            //WriteLog("BIS返回 " + outResult1 + ":" + strOut, LogLevels.Info, "MotionControlVm");

            ////MDI
            //string flag = "False";
            //bt.BatLength = 100;
            //bt.BatWidth = 40;
            //bt.LeftLugMargin = 30;
            //bt.RightLugMargin = 20;
            //bt.LeftLugLength = 19;
            //bt.RightLugLength = 18;
            //bt.AllBatLength = 17;
            //bt.Left1WhiteGlue = 16;
            //bt.Left2WhiteGlue = 15;
            //bt.Right1WhiteGlue = 14;
            //bt.Right2WhiteGlue = 13;

            //bt.Thickness = 15;

            //result = bis.BIS_TransfMylarData_New(bt.Sn, bt.Thickness.ToString(), ToString(), bt.BatLength.ToString(), bt.BatWidth.ToString(),
            //    "", bt.LeftLugMargin.ToString(), bt.RightLugMargin.ToString(), "", machinNo, "", "", "", "", "", "OK", "", "", "", "", "", bt.Thickness.ToString(),
            //    "", bt.Left1WhiteGlue.ToString(), bt.Left2WhiteGlue.ToString(), bt.Right1WhiteGlue.ToString(), bt.Right2WhiteGlue.ToString(), bt.RightLugLength.ToString(), bt.LeftLugLength.ToString(), "", "", OperaterId, "", bt.AllBatLength.ToString(), out strOut);
            //WriteLog("BIS返回 " + result + ":" + strOut, LogLevels.Info, "MotionControlVm");

            //XRAY
            //result = bis.BIS_TransfXRayDataNew(bt.Sn, OperaterId, "P89984", "0",
            //    "1.02_1.13_1.23_1.12",
            //    "0.82_0.75_.098_0.95", "NG", "-7", "", "",
            //    _workingSeats._seat1.Corner1.InspectParams.min_length.ToString(),
            //    _workingSeats._seat1.Corner1.InspectParams.max_length.ToString(), "", out strOut);
            //WriteLog("BIS返回 " + result + ":" + strOut, LogLevels.Info, "MotionControlVm");

            //bool bisResult = bis.BISXRayPicUpload(bt.Sn, @"D:\XRayPic\Test\20210106\1\NG\NG_20210106010155154_AY8051G30DE4_5_test.jpg", "NG_20210106010155154_AY8051G30DE4_5_test.jpg", "P89984", "0", OperaterId, out strOut);
            //WriteLog("BIS返回 " + bisResult + ":" + strOut, LogLevels.Info, "MotionControlVm");

            int index = -1;
            string strStep = "";
            int tickCount = 0;
            string barcode = string.Empty;
            plc.WriteByVariableName("GetTempBarcodeComplete1", 0);
            plc.WriteByVariableName("BatteryScanning1OK", 0);
            plc.WriteByVariableName("BatteryScanning1NG", 0);
            WriteLog("通道1扫码线程开启", LogLevels.Info, "Task_ScanBarcode1");


            while (true)
            {
                Thread.Sleep(10);
                if (_AutoRunning == false)
                {
                    WriteLog("通道1扫码线程退出", LogLevels.Info, "Task_ScanBarcode1");
                    return;
                }
                if (plc.ReadByVariableName("SoftwareReset") == "1")
                {
                    MotionEnum.EnumScanBarcode1 = MotionEnum.ScanBarcode1.工位1触发扫码;
                    index = -1;
                }
                //if (plc.ReadByVariableName("IVGRRSignal") == "1" || plc.ReadByVariableName("OCVGRRSignal") == "1" ||
                //    plc.ReadByVariableName("MDIGRRSignal") == "1")
                //{
                //    continue;
                //}
                strStep = Enum.GetName(typeof(MotionEnum.ScanBarcode1), MotionEnum.EnumScanBarcode1);
                switch (MotionEnum.EnumScanBarcode1)
                {
                    case MotionEnum.ScanBarcode1.工位1触发扫码:

                        if (plc.ReadByVariableName("ScanBarcodeSignal1") == "1" && plc.ReadByVariableName("TestMode") == "1")
                        {
                            tickCount = Environment.TickCount;
                            index += 2;
                            Thread.Sleep(HardwareConfig.Instance.ScanBarcodeDelay);
                            barcode = string.Empty;
                            CodeReaderIF.ClientSendMsg("LON\r", 1);
                            Thread.Sleep(100);
                            CodeReaderIF.ClientSendMsg("LOFF\r", 1);
                            barcode = CodeReaderIF.ClientReceiveMsg(1);

                            WriteLog(string.Format("第{0}次 {1} 条码为 {2}", index, strStep, barcode), LogLevels.Info, "Task_ScanBarcode1");

                            if (barcode != string.Empty && !barcode.Contains("ERROR"))
                            {
                                //MI号校验
                                if (CheckParamsConfig.Instance.IsCheckMI == true)
                                {
                                    if (CheckParamsConfig.Instance.Mi != barcode.Substring(0, 3))
                                    {
                                        System.Windows.MessageBox.Show("MI号与设置的不一致!", "提示",
                                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information, System.Windows.MessageBoxResult.OK, System.Windows.MessageBoxOptions.ServiceNotification);
                                        WriteLog(string.Format("第{0}次 {1} 条码为 {2} MI号与设置的不一致", index, strStep, barcode), LogLevels.Warn, "Task_ScanBarcode1");
                                        plc.WriteByVariableName("BatteryScanning1NG", 1);
                                        MotionEnum.EnumScanBarcode1 = MotionEnum.ScanBarcode1.工位1扫码完成;
                                        continue;
                                    }
                                }
                                //周次校验
                                if (CheckParamsConfig.Instance.IsCheckWeekCount == true)
                                {
                                    if (CheckParamsConfig.Instance.WeekCounts != Convert.ToInt32(barcode.Substring(4, 2)))
                                    {
                                        System.Windows.MessageBox.Show("周次与设置的不一致!", "提示",
                                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information, System.Windows.MessageBoxResult.OK, System.Windows.MessageBoxOptions.ServiceNotification);
                                        WriteLog(string.Format("第{0}次 {1} 条码为 {2} 周次与设置的不一致", index, strStep, barcode), LogLevels.Warn, "Task_ScanBarcode1");
                                        plc.WriteByVariableName("BatteryScanning1NG", 1);
                                        MotionEnum.EnumScanBarcode1 = MotionEnum.ScanBarcode1.工位1扫码完成;
                                        continue;
                                    }
                                }
                                plc.WriteByVariableName("BatteryScanning1OK", 1);
                                MotionEnum.EnumScanBarcode1 = MotionEnum.ScanBarcode1.工位1获取条码代号;
                            }
                            else
                            {
                                BotIF.MyCheckStatus.MyStatistics.ScanNG++;
                                plc.WriteByVariableName("BatteryScanning1NG", 1);
                                MotionEnum.EnumScanBarcode1 = MotionEnum.ScanBarcode1.工位1扫码完成;
                            }
                            BotIF.MyCheckStatus.MyStatistics.ScanTotal++;

                        }

                        break;

                    case MotionEnum.ScanBarcode1.工位1获取条码代号:

                        if (plc.ReadByVariableName("GetTempBarodeSignal1") == "1")
                        {
                            if (plc.ReadByVariableName("ScannerTempBarcode1") != "0")
                            {
                                WriteLog(string.Format("第{0}次 {1} 条码 {2} 条码代号 {3}", index, strStep, barcode, plc.ReadByVariableName("ScannerTempBarcode1")), LogLevels.Info, "Task_ScanBarcode1");
                                //barcode = barcode.Replace("+", "");
                                //barcode = barcode + "_" + plc.ReadByVariableName("ScannerTempBarcode1");
                                if (!dicBarcode1.ContainsKey(Convert.ToInt32(plc.ReadByVariableName("ScannerTempBarcode1"))))
                                {
                                    dicBarcode1.Add(Convert.ToInt32(plc.ReadByVariableName("ScannerTempBarcode1")), barcode);
                                }
                                else
                                {
                                    dicBarcode1[Convert.ToInt32(plc.ReadByVariableName("ScannerTempBarcode1"))] = barcode;
                                }
                                plc.WriteByVariableName("GetTempBarcodeComplete1", 1);
                                MotionEnum.EnumScanBarcode1 = MotionEnum.ScanBarcode1.工位1获取条码代号完成;
                            }
                        }
                        break;

                    case MotionEnum.ScanBarcode1.工位1获取条码代号完成:

                        if (plc.ReadByVariableName("GetTempBarodeSignal1") == "0")
                        {
                            plc.WriteByVariableName("GetTempBarcodeComplete1", 0);
                            MotionEnum.EnumScanBarcode1 = MotionEnum.ScanBarcode1.工位1扫码完成;
                        }
                        break;

                    case MotionEnum.ScanBarcode1.工位1扫码完成:

                        if (plc.ReadByVariableName("ScanBarcodeSignal1") == "0")
                        {
                            WriteLog(string.Format("第{0}次 {1} ,共耗时 {2}", index, strStep, Environment.TickCount - tickCount), LogLevels.Info, "Task_ScanBarcode1");
                            plc.WriteByVariableName("BatteryScanning1OK", 0);
                            plc.WriteByVariableName("BatteryScanning1NG", 0);
                            if (barcode == string.Empty || barcode.Contains("ERROR"))
                            {
                                index -= 2;
                            }
                            MotionEnum.EnumScanBarcode1 = MotionEnum.ScanBarcode1.工位1触发扫码;
                        }
                        break;
                }
            }

        }

        /// <summary>
        /// 通道2扫码
        /// </summary>
        private void Task_ScanBarcode2()
        {
            int index = 0;
            string strStep = "";
            int tickCount = 0;
            string barcode = string.Empty;
            plc.WriteByVariableName("GetTempBarcodeComplete2", 0);
            plc.WriteByVariableName("BatteryScanning2OK", 0);
            plc.WriteByVariableName("BatteryScanning2NG", 0);
            WriteLog("通道2扫码线程开启", LogLevels.Info, "Task_ScanBarcode2");

            while (true)
            {
                Thread.Sleep(10);
                if (_AutoRunning == false)
                {
                    WriteLog("通道2扫码线程退出", LogLevels.Info, "Task_ScanBarcode2");
                    return;
                }
                if (plc.ReadByVariableName("SoftwareReset") == "1")
                {
                    MotionEnum.EnumScanBarcode2 = MotionEnum.ScanBarcode2.工位2触发扫码;
                    index = 0;
                }
                strStep = Enum.GetName(typeof(MotionEnum.ScanBarcode2), MotionEnum.EnumScanBarcode2);
                switch (MotionEnum.EnumScanBarcode2)
                {
                    case MotionEnum.ScanBarcode2.工位2触发扫码:

                        if (plc.ReadByVariableName("ScanBarcodeSignal2") == "1" && plc.ReadByVariableName("TestMode") == "1")
                        {
                            tickCount = Environment.TickCount;
                            if (plc.ReadByVariableName("SingleScanerMode") == "1")
                            {
                                index += 1;
                            }
                            else
                            {
                                index += 2;
                            }
                            Thread.Sleep(HardwareConfig.Instance.ScanBarcodeDelay);
                            barcode = string.Empty;
                            CodeReaderIF.ClientSendMsg("LON\r", 3);
                            Thread.Sleep(100);
                            CodeReaderIF.ClientSendMsg("LOFF\r", 3);
                            barcode = CodeReaderIF.ClientReceiveMsg(3);

                            WriteLog(string.Format("第{0}次 {1} 条码为 {2}", index, strStep, barcode), LogLevels.Info, "Task_ScanBarcode2");
                            if (barcode != string.Empty && !barcode.Contains("ERROR"))
                            {
                                //MI号校验
                                if (CheckParamsConfig.Instance.IsCheckMI == true)
                                {
                                    if (CheckParamsConfig.Instance.Mi != barcode.Substring(0, 3))
                                    {
                                        System.Windows.MessageBox.Show("MI号与设置的不一致!");
                                        WriteLog(string.Format("第{0}次 {1} 条码为 {2} MI号与设置的不一致", index, strStep, barcode), LogLevels.Warn, "Task_ScanBarcode2");
                                        plc.WriteByVariableName("BatteryScanning2NG", 1);
                                        MotionEnum.EnumScanBarcode2 = MotionEnum.ScanBarcode2.工位2扫码完成;
                                        continue;
                                    }
                                }
                                //周次校验
                                if (CheckParamsConfig.Instance.IsCheckWeekCount == true)
                                {
                                    if (CheckParamsConfig.Instance.WeekCounts != Convert.ToInt32(barcode.Substring(4, 2)))
                                    {
                                        System.Windows.MessageBox.Show("周次与设置的不一致!", "提示",
                                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information, System.Windows.MessageBoxResult.OK, System.Windows.MessageBoxOptions.ServiceNotification);
                                        WriteLog(string.Format("第{0}次 {1} 条码为 {2} 周次与设置的不一致", index, strStep, barcode), LogLevels.Warn, "Task_ScanBarcode2");
                                        plc.WriteByVariableName("BatteryScanning2NG", 1);
                                        MotionEnum.EnumScanBarcode2 = MotionEnum.ScanBarcode2.工位2扫码完成;
                                        continue;
                                    }
                                }
                                plc.WriteByVariableName("BatteryScanning2OK", 1);
                                MotionEnum.EnumScanBarcode2 = MotionEnum.ScanBarcode2.工位2获取条码代号;
                            }
                            else
                            {
                                BotIF.MyCheckStatus.MyStatistics.ScanNG++;
                                plc.WriteByVariableName("BatteryScanning2NG", 1);
                                MotionEnum.EnumScanBarcode2 = MotionEnum.ScanBarcode2.工位2扫码完成;
                            }
                            BotIF.MyCheckStatus.MyStatistics.ScanTotal++;
                        }

                        break;

                    case MotionEnum.ScanBarcode2.工位2获取条码代号:

                        if (plc.ReadByVariableName("GetTempBarodeSignal2") == "1")
                        {
                            if (plc.ReadByVariableName("ScannerTempBarcode2") != "0")
                            {
                                if (plc.ReadByVariableName("SingleScanerMode") == "1")
                                {
                                    if (oldTempBarcode == Convert.ToInt32(plc.ReadByVariableName("ScannerTempBarcode2")))
                                    {
                                        WriteLog(string.Format("第{0}次 {1} PLC条码代号 {2} 未更新", index, strStep, plc.ReadByVariableName("ScannerTempBarcode2")), LogLevels.Warn, "Task_ScanBarcode2");
                                        Thread.Sleep(500);
                                        continue;
                                    }
                                }
                                //barcode = barcode + "_" + plc.ReadByVariableName("ScannerTempBarcode2");
                                WriteLog(string.Format("第{0}次 {1} 条码 {2} 条码代号 {3}", index, strStep, barcode, plc.ReadByVariableName("ScannerTempBarcode2")), LogLevels.Info, "Task_ScanBarcode2");
                                if (!dicBarcode2.ContainsKey(Convert.ToInt32(plc.ReadByVariableName("ScannerTempBarcode2"))))
                                {
                                    dicBarcode2.Add(Convert.ToInt32(plc.ReadByVariableName("ScannerTempBarcode2")), barcode);
                                }
                                else
                                {
                                    dicBarcode2[Convert.ToInt32(plc.ReadByVariableName("ScannerTempBarcode2"))] = barcode;
                                }
                                plc.WriteByVariableName("GetTempBarcodeComplete2", 1);
                                oldTempBarcode = Convert.ToInt32(plc.ReadByVariableName("ScannerTempBarcode2"));
                                MotionEnum.EnumScanBarcode2 = MotionEnum.ScanBarcode2.工位2获取条码代号完成;
                            }
                        }
                        break;

                    case MotionEnum.ScanBarcode2.工位2获取条码代号完成:

                        if (plc.ReadByVariableName("GetTempBarodeSignal2") == "0")
                        {
                            plc.WriteByVariableName("GetTempBarcodeComplete2", 0);
                            MotionEnum.EnumScanBarcode2 = MotionEnum.ScanBarcode2.工位2扫码完成;
                        }
                        break;

                    case MotionEnum.ScanBarcode2.工位2扫码完成:

                        if (plc.ReadByVariableName("ScanBarcodeSignal2") == "0")
                        {
                            WriteLog(string.Format("第{0}次 {1} ,共耗时 {2}", index, strStep, Environment.TickCount - tickCount), LogLevels.Info, "Task_ScanBarcode2");
                            plc.WriteByVariableName("BatteryScanning2OK", 0);
                            plc.WriteByVariableName("BatteryScanning2NG", 0);
                            if (barcode == string.Empty || barcode.Contains("ERROR"))
                            {
                                if (plc.ReadByVariableName("SingleScanerMode") == "1")
                                {
                                    index -= 1;
                                }
                                else
                                {
                                    index -= 2;
                                }
                            }
                            MotionEnum.EnumScanBarcode2 = MotionEnum.ScanBarcode2.工位2触发扫码;
                        }
                        break;
                }
            }
        }

        private void Task_BarcodeBinding1()
        {
            string strStep = "";
            BatterySeat bt = null;
            int tickCount = 0;
            plc.WriteByVariableName("BarcodeBindingComplete1", 0);
            WriteLog("通道1条码绑定线程开启", LogLevels.Info, "Task_BarcodeBinding1");

            int index = -1;
            while (true)
            {
                Thread.Sleep(10);
                if (_AutoRunning == false)
                {
                    WriteLog("通道1条码绑定线程退出", LogLevels.Info, "Task_BarcodeBinding1");
                    return;
                }
                if (plc.ReadByVariableName("SoftwareReset") == "1")
                {
                    MotionEnum.EnumBarcodeBinding1 = MotionEnum.BarcodeBinding1.触发条码绑定;
                    index = -1;
                }

                strStep = Enum.GetName(typeof(MotionEnum.BarcodeBinding1), MotionEnum.EnumBarcodeBinding1);

                switch (MotionEnum.EnumBarcodeBinding1)
                {
                    case MotionEnum.BarcodeBinding1.触发条码绑定:

                        if (plc.ReadByVariableName("BarcodebindingSignal1") == "1" && plc.ReadByVariableName("TestMode") == "1")
                        {
                            isBarcodeBinding1Finished = false;
                            Thread.Sleep(300);
                            tickCount = Environment.TickCount;

                            if (plc.ReadByVariableName("SingleScanerMode") == "1")//单扫码枪模式
                            {
                                index += 1;
                                int tempBarcode = Convert.ToInt32(plc.ReadByVariableName("BindingTempBarcode1"));
                                if (dicBarcode2.ContainsKey(tempBarcode))
                                {
                                    WriteLog(string.Format("第{0}次 {1} ", index, strStep), LogLevels.Info, "Task_BarcodeBinding1");
                                    bt = new BatterySeat();
                                    if (CheckParamsConfig.Instance.IsCheckABCell == false)
                                    {
                                        _workingSeats._seat1.CopyTo(ref bt);
                                    }
                                    else
                                    {
                                        string marking = bis.GetMarking(dicBarcode2[tempBarcode]);
                                        WriteLog("BIS返回 " + marking, LogLevels.Info, "Task_BarcodeBinding1");
                                        if (!marking.Contains(CheckParamsConfig.Instance.MarkingOfACell))
                                        {
                                            //B
                                            _workingSeats._seat2.CopyTo(ref bt);
                                            bt.CellType = "B";
                                        }
                                        else
                                        {
                                            //A
                                            _workingSeats._seat1.CopyTo(ref bt);
                                            bt.CellType = "A";
                                        }
                                    }


                                    bt.TempBarcode = tempBarcode;
                                    bt.Sn = dicBarcode2[tempBarcode];
                                    if (bt.Sn.Contains("+"))
                                    {
                                        bt.LengthBarcode = bt.Sn;
                                        //bt.Sn = bt.Sn.Substring(bt.Sn.Length - 12, 12);//bis上传图片的命名必须要12位条码
                                    }
                                    else
                                    {
                                        bt.LengthBarcode = bt.Sn;
                                    }
                                    WriteLog(string.Format("第{0}次工位一条码绑定 {1} <==> {2} ", index, tempBarcode, bt.Sn), LogLevels.Info, "Task_BarcodeBinding1");
                                    if (isOpenBis == true)
                                    {
                                        bt.Marking = bis.GetMarking(bt.Sn);
                                    }
                                    foreach (var temp in testCodeManager.CodeList)
                                    {
                                        if (bt.Sn == temp.BarCode || bt.LengthBarcode == temp.BarCode)
                                        {
                                            bt.MasterType = temp.Remarks;
                                            bt.CheckExtension = ECheckExtensions.Local;
                                            break;
                                        }
                                    }

                                    EnterQueue(bt, "BarcodeBinding1");
                                    dicBarcode2.Remove(tempBarcode);
                                    plc.WriteByVariableName("BarcodeBindingComplete1", 1);
                                    MotionEnum.EnumBarcodeBinding1 = MotionEnum.BarcodeBinding1.条码绑定完成;
                                }
                                else
                                {
                                    WriteLog(string.Format("第{0}次 {1} 找不到条码代号 {2}", index, strStep, tempBarcode), LogLevels.Info, "Task_BarcodeBinding1");
                                    Thread.Sleep(3000);
                                    index -= 1;
                                }
                            }
                            else
                            {
                                index += 2;
                                int tempBarcode = Convert.ToInt32(plc.ReadByVariableName("BindingTempBarcode1"));
                                if (dicBarcode1.ContainsKey(tempBarcode))
                                {
                                    WriteLog(string.Format("第{0}次 {1} ", index, strStep), LogLevels.Info, "Task_BarcodeBinding1");
                                    bt = new BatterySeat();

                                    if (CheckParamsConfig.Instance.IsCheckABCell == false)
                                    {
                                        _workingSeats._seat1.CopyTo(ref bt);
                                    }
                                    else
                                    {
                                        string marking = bis.GetMarking(dicBarcode2[tempBarcode]);
                                        //if (!marking.Contains("GRFTWF"))
                                        if (!marking.Contains(CheckParamsConfig.Instance.MarkingOfACell))
                                        {
                                            //B
                                            _workingSeats._seat2.CopyTo(ref bt);
                                            bt.CellType = "B";
                                        }
                                        else
                                        {
                                            //A
                                            _workingSeats._seat1.CopyTo(ref bt);
                                            bt.CellType = "A";
                                        }
                                    }
                                    bt.TempBarcode = tempBarcode;
                                    bt.Sn = dicBarcode1[tempBarcode];
                                    if (bt.Sn.Contains("+"))
                                    {
                                        bt.LengthBarcode = bt.Sn;
                                        //bt.Sn = bt.Sn.Substring(bt.Sn.Length - 12, 12);//bis上传图片的命名必须要12位条码
                                    }
                                    else
                                    {
                                        bt.LengthBarcode = bt.Sn;
                                    }
                                    WriteLog(string.Format("第{0}次工位一条码绑定 {1} <==> {2} ", index, tempBarcode, bt.Sn), LogLevels.Info, "Task_BarcodeBinding1");
                                    if (isOpenBis == true && CheckParamsConfig.Instance.IsCheckMarking == true)
                                    {
                                        string strOut;
                                        bis.SetMarking(bt.Sn, CheckParamsConfig.Instance.SetMarking, out strOut);
                                        WriteLog(string.Format("条码 {0} 的设置marking为 {1} ,Bis返回 {2} ", bt.Sn, CheckParamsConfig.Instance.SetMarking, strOut), LogLevels.Info, "Task_BarcodeBinding1");
                                    }
                                    if (isOpenBis == true)
                                    {
                                        bt.Marking = bis.GetMarking(bt.Sn);
                                    }
                                    foreach (var temp in testCodeManager.CodeList)
                                    {
                                        if (bt.Sn == temp.BarCode || bt.LengthBarcode == temp.BarCode)
                                        {
                                            bt.MasterType = temp.Remarks;
                                            bt.CheckExtension = ECheckExtensions.Local;
                                            break;
                                        }
                                    }

                                    EnterQueue(bt, "BarcodeBinding1");
                                    dicBarcode1.Remove(tempBarcode);
                                    plc.WriteByVariableName("BarcodeBindingComplete1", 1);
                                    MotionEnum.EnumBarcodeBinding1 = MotionEnum.BarcodeBinding1.条码绑定完成;
                                }
                                else
                                {
                                    WriteLog(string.Format("第{0}次 {1} 找不到条码代号 {2}", index, strStep, tempBarcode), LogLevels.Info, "Task_BarcodeBinding1");
                                    Thread.Sleep(3000);
                                    index -= 2;
                                }
                            }

                        }
                        break;

                    case MotionEnum.BarcodeBinding1.条码绑定完成:

                        if (plc.ReadByVariableName("BarcodebindingSignal1") == "0")
                        {
                            isBarcodeBinding1Finished = true;
                            WriteLog(string.Format("第{0}次 {1} ,共耗时 {2}", index, strStep, Environment.TickCount - tickCount), LogLevels.Info, "Task_ScanBarcode1");
                            plc.WriteByVariableName("BarcodeBindingComplete1", 0);
                            MotionEnum.EnumBarcodeBinding1 = MotionEnum.BarcodeBinding1.触发条码绑定;
                        }

                        break;
                }
            }
        }

        private void Task_BarcodeBinding2()
        {
            string strStep = "";
            BatterySeat bt = null;
            int tickCount = 0;
            int index = 0;
            plc.WriteByVariableName("BarcodeBindingComplete2", 0);
            WriteLog("通道2条码绑定线程开启", LogLevels.Info, "Task_BarcodeBinding2");

            while (true)
            {
                Thread.Sleep(10);
                if (_AutoRunning == false)
                {
                    WriteLog("通道2条码绑定线程退出", LogLevels.Info, "Task_BarcodeBinding2");
                    return;
                }
                if (plc.ReadByVariableName("SoftwareReset") == "1")
                {
                    MotionEnum.EnumBarcodeBinding2 = MotionEnum.BarcodeBinding2.触发条码绑定;
                    index = 0;
                }

                strStep = Enum.GetName(typeof(MotionEnum.BarcodeBinding2), MotionEnum.EnumBarcodeBinding2);

                switch (MotionEnum.EnumBarcodeBinding2)
                {
                    case MotionEnum.BarcodeBinding2.触发条码绑定:

                        if (plc.ReadByVariableName("BarcodebindingSignal2") == "1" && plc.ReadByVariableName("TestMode") == "1")
                        {
                            Thread.Sleep(300);
                            tickCount = Environment.TickCount;
                            index += 2;
                            int tempBarcode = Convert.ToInt32(plc.ReadByVariableName("BindingTempBarcode2"));
                            if (dicBarcode2.ContainsKey(tempBarcode))
                            {
                                WriteLog(string.Format("第{0}次 {1} ", index, strStep), LogLevels.Info, "Task_BarcodeBinding2");
                                bt = new BatterySeat();
                                if (CheckParamsConfig.Instance.IsCheckABCell == false)
                                {
                                    _workingSeats._seat1.CopyTo(ref bt);
                                }
                                else
                                {
                                    string marking = bis.GetMarking(dicBarcode2[tempBarcode]);
                                    WriteLog("BIS返回 " + marking, LogLevels.Info, "Task_BarcodeBinding2");
                                    if (!marking.Contains(CheckParamsConfig.Instance.MarkingOfACell))
                                    {
                                        //B
                                        _workingSeats._seat2.CopyTo(ref bt);
                                        bt.CellType = "B";
                                    }
                                    else
                                    {
                                        //A
                                        _workingSeats._seat1.CopyTo(ref bt);
                                        bt.CellType = "A";
                                    }
                                }

                                bt.TempBarcode = tempBarcode;
                                bt.Sn = dicBarcode2[tempBarcode];
                                if (bt.Sn.Contains("+"))
                                {
                                    bt.LengthBarcode = bt.Sn;
                                    //bt.Sn = bt.Sn.Substring(bt.Sn.Length - 12, 12);//bis上传图片的命名必须要12位条码
                                }
                                else
                                {
                                    bt.LengthBarcode = bt.Sn;
                                }

                                WriteLog(string.Format("第{0}次工位二条码绑定 {1} <==> {2} ", index, tempBarcode, bt.Sn), LogLevels.Info, "Task_BarcodeBinding2");
                                if (isOpenBis == true && CheckParamsConfig.Instance.SetMarking != "")
                                {
                                    string strOut;
                                    bis.SetMarking(bt.Sn, CheckParamsConfig.Instance.SetMarking, out strOut);
                                    WriteLog(string.Format("条码 {0} 的设置marking为 {1} ,Bis返回 {2} ", bt.Sn, CheckParamsConfig.Instance.SetMarking, strOut), LogLevels.Info, "Task_BarcodeBinding2");
                                }

                                if (isOpenBis == true)
                                {
                                    bt.Marking = bis.GetMarking(bt.Sn);
                                }
                                dicBarcode2.Remove(tempBarcode);
                                plc.WriteByVariableName("BarcodeBindingComplete2", 1);
                                foreach (var temp in testCodeManager.CodeList)
                                {
                                    if (bt.Sn == temp.BarCode || bt.LengthBarcode == temp.BarCode)
                                    {
                                        bt.MasterType = temp.Remarks;
                                        bt.CheckExtension = ECheckExtensions.Local;
                                        break;
                                    }
                                }
                                MotionEnum.EnumBarcodeBinding2 = MotionEnum.BarcodeBinding2.条码绑定完成;
                            }
                            else
                            {
                                WriteLog(string.Format("第{0}次 {1} 找不到条码代号 {2}", index, strStep, tempBarcode), LogLevels.Info, "Task_BarcodeBinding2");
                                Thread.Sleep(3000);
                                index -= 2;
                            }
                        }
                        break;

                    case MotionEnum.BarcodeBinding2.条码绑定完成:

                        if (plc.ReadByVariableName("BarcodebindingSignal2") == "0" && isBarcodeBinding1Finished == true)
                        {
                            EnterQueue(bt, "BarcodeBinding2");
                            WriteLog(string.Format("第{0}次 {1} ,共耗时 {2}", index, strStep, Environment.TickCount - tickCount), LogLevels.Info, "Task_ScanBarcode2");
                            plc.WriteByVariableName("BarcodeBindingComplete2", 0);
                            MotionEnum.EnumBarcodeBinding2 = MotionEnum.BarcodeBinding2.触发条码绑定;
                        }

                        break;
                }
            }
        }

        /// <summary>
        /// 平板1拍照四、三角
        /// </summary>
        private void Task_XrayShot1()
        {
            string strStep = "";
            BatterySeat bt = null;
            int index = 0;
            int tickCount = 0;
            WriteLog("平板1拍照四、三线程开启", LogLevels.Info, "Task_XrayShot1");
            plc.WriteByVariableName("Battery1CornerPhotoComplete", 0);
            plc.WriteByVariableName("Battery2CornerPhotoComplete", 0);

            while (true)
            {
                Thread.Sleep(10);
                if (_AutoRunning == false)
                {
                    WriteLog("相机1拍照一二角线程退出", LogLevels.Info, "Task_XrayShot1");
                    return;
                }
                if (plc.ReadByVariableName("SoftwareReset") == "1")
                {
                    MotionEnum.EnumXrayShot1 = MotionEnum.XRAYShot1.触发角一拍照;
                    index = 0;
                }

                strStep = Enum.GetName(typeof(MotionEnum.XRAYShot1), MotionEnum.EnumXrayShot1);

                switch (MotionEnum.EnumXrayShot1)
                {
                    case MotionEnum.XRAYShot1.触发角一拍照:

                        //拍照1实际拍电池的四号角
                        if (plc.ReadByVariableName("Battery1CornerPhotoSignal") == "1" && plc.ReadByVariableName("TestMode") == "1")
                        {
                            if (MyCameraSetting.IsVideoOn == true)
                            {
                                System.Windows.MessageBox.Show("请先关闭相机页面的视频模式!", "提示", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.None,
                            System.Windows.MessageBoxResult.OK, System.Windows.MessageBoxOptions.ServiceNotification);
                                Thread.Sleep(5000);
                                continue;
                            }

                            if (queueThickness.Count == 0)
                            {
                                WriteLog("XRAY1工位无料", LogLevels.Warn, "Task_XrayShot1");
                                Thread.Sleep(1000);
                                continue;
                            }
                            if (queueThickness.Peek().MasterType == "")
                            {
                                if (canDoXRAY == false)
                                {
                                    System.Windows.MessageBox.Show("XRAY未点检完成，不能启动XRAY功能", "提示", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.None,
                            System.Windows.MessageBoxResult.OK, System.Windows.MessageBoxOptions.ServiceNotification);
                                    Thread.Sleep(1000);
                                    continue;
                                }
                            }

                            index = queueThickness.Peek().TempBarcode;
                            int tempBarcode = 0;
                            tempBarcode = Convert.ToInt32(plc.ReadByVariableName("XRAYTempBarcode1"));
                            //if (isOpenBis == true && !CheckXray())
                            //{
                            //    if (bt.MasterType == "")
                            //    {
                            //        WriteLog("XRAY未点检，不能开机做XRAY！", LogLevels.Warn, "Task_XrayShot1");
                            //        System.Windows.MessageBox.Show("XRAY未点检，不能开机做XRAY!");
                            //        Thread.Sleep(5000);
                            //        continue;
                            //    }
                            //}
                            if (queueThickness.Peek().TempBarcode != tempBarcode)
                            {
                                WriteLog(string.Format("XRAY1工位条码代号 PC {0} != PLC {1}！", queueThickness.Peek().TempBarcode, tempBarcode), LogLevels.Warn, "Task_XrayShot1");
                                Thread.Sleep(500);
                                continue;
                            }

                            tickCount = Environment.TickCount;
                            WriteLog(string.Format("第{0}次 {1} ", index, strStep), LogLevels.Info, "Task_XrayShot1");
                            bt = queueThickness.Dequeue();
                            MotionEnum.EnumXrayShot1 = MotionEnum.XRAYShot1.角一拍照;
                        }
                        break;

                    case MotionEnum.XRAYShot1.角一拍照:

                        if (XRayTubeIF.XRayTube1Stauts.ActualVoltage == HardwareConfig.Instance.XRayConfig1.PresetVoltage && XRayTubeIF.XRayTube1Stauts.ActualCurrent == HardwareConfig.Instance.XRayConfig1.PresetCurrent)
                        {

                            Thread.Sleep(HardwareConfig.Instance.CameraShotDelay4);

                            //TODO: 替换旧版相机采图  ZhangKF 2021-3-16
                            //ZYImageStruct img4 = bt.Corner4.ShotImage;
                            try
                            {
                                //int k = 0;
                                //int shotTime = Environment.TickCount;
                                //int iRet4 = VisionSysWrapperIF.ShotAndAverage(ECamTypes.Camera1, _camParam1.PinValue, ref img4, 4, out k);
                                //bt.Corner4.ImgNO = k;
                                //WriteLog(string.Format("第{0}次 角一采图 保存图片, 16位图编号：{1} {2} 耗时 {3} 毫秒", index, bt.Corner4.ImgNO, k, Environment.TickCount - shotTime), LogLevels.Info, "Task_XrayShot1");
                                //string picname = "D:\\Test\\4_" + bt.Sn + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + index + ".jpg";
                                //img4.Save(picname, 1, bt.Corner4.ImgNO);

                                //TODO: 替换旧版相机采图  ZhangKF 2021-3-16
                                var result = CameraHelper.CaptureOneImage(0);
                                //if (iRet4 == -1)
                                if (!result.Result)
                                {
                                    bt.Corner4.IsShotOK = false;
                                    WriteLog(string.Format("第{0}次 角一采图失败", index), LogLevels.Info, "Task_XrayShot1");
                                }
                                else
                                {
                                    bt.Corner4.ShotImage = result.Bitmap.ToTransfor(Consts.ImageTypes.Sixteen);
                                    bt.Corner4.IsShotOK = true;
                                    //TODO: 增加保存原图 ZhangKF 2020-12-30
                                    //if (SAVE_TEST_IMAGE == "1")
                                    //{
                                    //string picname = "D:\\Test\\4_" + bt.Sn + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".JPG";
                                    //capResult.Bitmap.SaveToJPG(picname);
                                    //}
                                }
                                m_bAlgoCorner4Finished = false;
                                ThreadPool.QueueUserWorkItem(state => TestCorner4(ref bt, index));
                                plc.WriteByVariableName("Battery1CornerPhotoComplete", 1);
                                MotionEnum.EnumXrayShot1 = MotionEnum.XRAYShot1.角一拍照完成;
                            }
                            catch (Exception ex)
                            {
                                WriteLog(string.Format("第{0}次 角一采图失败 {1}", index, ex), LogLevels.Error, "Task_XrayShot1");
                                Thread.Sleep(3000);
                                continue;
                            }
                        }
                        break;

                    case MotionEnum.XRAYShot1.角一拍照完成:

                        if (plc.ReadByVariableName("Battery1CornerPhotoSignal") == "0")
                        {
                            //WriteLog(string.Format("第{0}次 {1} ", index, strStep), LogLevels.Info, "Task_XrayShot1");
                            WriteLog(string.Format("第{0}次 {1} 耗时 {2} 毫秒", index, strStep, Environment.TickCount - tickCount), LogLevels.Info, "Task_XrayShot1");
                            plc.WriteByVariableName("Battery1CornerPhotoComplete", 0);
                            MotionEnum.EnumXrayShot1 = MotionEnum.XRAYShot1.触发角二拍照;
                        }
                        break;

                    case MotionEnum.XRAYShot1.触发角二拍照:

                        if (plc.ReadByVariableName("Battery2CornerPhotoSignal") == "1")
                        {
                            if (XRayTubeIF.XRayTube1Stauts.ActualVoltage == HardwareConfig.Instance.XRayConfig1.PresetVoltage && XRayTubeIF.XRayTube1Stauts.ActualCurrent == HardwareConfig.Instance.XRayConfig1.PresetCurrent)
                            {
                                if (MyCameraSetting.IsVideoOn == true)
                                {
                                    System.Windows.MessageBox.Show("请先关闭相机页面的视频模式!", "提示", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.None,
                            System.Windows.MessageBoxResult.OK, System.Windows.MessageBoxOptions.ServiceNotification);
                                    Thread.Sleep(5000);
                                    continue;
                                }

                                tickCount = Environment.TickCount;
                                WriteLog(string.Format("第{0}次 {1} ", index, strStep), LogLevels.Info, "Task_XrayShot1");
                                Thread.Sleep(HardwareConfig.Instance.CameraShotDelay3);

                                //ZYImageStruct img3 = bt.Corner3.ShotImage;
                                //TODO: 替换旧版相机采图  ZhangKF 2021-3-16                            
                                //int shotTime = Environment.TickCount;
                                //int k = 0;
                                //int iRet3 = VisionSysWrapperIF.ShotAndAverage(ECamTypes.Camera1, _camParam1.PinValue, ref img3, 4, out k);//最后一个参数：4 jpg 3 tiff
                                //bt.Corner3.ImgNO = k;
                                //WriteLog(string.Format("第{0}次 角二采图 保存图片, 16位图编号：{1} {2} 耗时 {3} 毫秒", index, bt.Corner3.ImgNO, k, Environment.TickCount - shotTime), LogLevels.Info, "Task_XrayShot1");
                                //string picname = "D:\\Test\\3_" + bt.Sn + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + index + ".jpg";
                                //img3.Save(picname, 1, bt.Corner3.ImgNO);

                                //TODO: 替换旧版相机采图  ZhangKF 2021-3-16
                                var result = CameraHelper.CaptureOneImage(0);
                                //if (iRet3 == -1)
                                if (!result.Result)
                                {
                                    bt.Corner3.IsShotOK = false;
                                    WriteLog(string.Format("第{0}次 角二采图失败", index), LogLevels.Info, "Task_XrayShot1");
                                }
                                else
                                {
                                    bt.Corner3.ShotImage = result.Bitmap.ToTransfor(Consts.ImageTypes.Sixteen);
                                    bt.Corner3.IsShotOK = true;
                                }
                                m_bAlgoCorner3Finished = false;
                                ThreadPool.QueueUserWorkItem(state => TestCorner3(ref bt, index));
                                plc.WriteByVariableName("Battery2CornerPhotoComplete", 1);
                                queueXRAY1Station.Enqueue(bt);
                                MotionEnum.EnumXrayShot1 = MotionEnum.XRAYShot1.角二拍照完成;
                            }
                        }

                        break;

                    case MotionEnum.XRAYShot1.角二拍照完成:

                        if (plc.ReadByVariableName("Battery2CornerPhotoSignal") == "0" && m_bAlgoCorner4Finished && m_bAlgoCorner3Finished)
                        {
                            WriteLog(string.Format("第{0}次 {1} 耗时 {2} 毫秒", index, strStep, Environment.TickCount - tickCount), LogLevels.Info, "Task_XrayShot1");
                            plc.WriteByVariableName("Battery2CornerPhotoComplete", 0);
                            MotionEnum.EnumXrayShot1 = MotionEnum.XRAYShot1.触发角一拍照;
                        }
                        break;
                }
            }
        }
        bool isKValueNG = false;

        /// <summary>
        /// 平板2拍照二、一角
        /// </summary>
        private void Task_XrayShot2()
        {
            string strStep = "";
            BatterySeat bt = null;
            int index = 0;
            int tickCount = 0;
            string bisBarcode = "";
            bool isMasterCell = false;//是否为master电池
            WriteLog("平板2拍照二、一角线程开启", LogLevels.Info, "Task_XrayShot2");
            plc.WriteByVariableName("Battery3CornerPhotoComplete", 0);
            plc.WriteByVariableName("Battery4CornerPhotoComplete", 0);

            string xRayMachinNo = UserDefineVariableInfo.DicVariables["AssetsNO"].ToString();

            while (true)
            {
                Thread.Sleep(10);
                if (_AutoRunning == false)
                {
                    WriteLog("相机二拍照三四角线程退出", LogLevels.Info, "Task_XrayShot2");
                    return;
                }
                if (plc.ReadByVariableName("SoftwareReset") == "1")
                {
                    MotionEnum.EnumXrayShot2 = MotionEnum.XRAYShot2.触发角三拍照;
                    index = 0;
                }

                strStep = Enum.GetName(typeof(MotionEnum.XRAYShot2), MotionEnum.EnumXrayShot2);

                switch (MotionEnum.EnumXrayShot2)
                {
                    case MotionEnum.XRAYShot2.触发角三拍照:

                        if (plc.ReadByVariableName("Battery3CornerPhotoSignal") == "1" && plc.ReadByVariableName("TestMode") == "1")
                        {
                            if (MyCameraSetting.IsVideoOn == true)
                            {
                                System.Windows.MessageBox.Show("请先关闭相机页面的视频模式!", "提示", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.None,
                            System.Windows.MessageBoxResult.OK, System.Windows.MessageBoxOptions.ServiceNotification);
                                Thread.Sleep(5000);
                                continue;
                            }

                            if (queueXRAY1Station.Count == 0)
                            {
                                WriteLog("XRAY2工位无料", LogLevels.Warn, "Task_XrayShot2");
                                Thread.Sleep(1000);
                                continue;
                            }

                            int tempBarcode = 0;
                            tempBarcode = Convert.ToInt32(plc.ReadByVariableName("XRAYTempBarcode2"));
                            index = queueXRAY1Station.Peek().TempBarcode;
                            if (queueXRAY1Station.Peek().TempBarcode != tempBarcode)
                            {
                                WriteLog(string.Format("XRAY2工位条码代号 PC {0} != PLC {1}！", queueXRAY1Station.Peek().TempBarcode, tempBarcode), LogLevels.Warn, "Task_XrayShot2");
                                Thread.Sleep(500);
                                continue;
                            }

                            tickCount = Environment.TickCount;
                            WriteLog(string.Format("第{0}次 {1} ", index, strStep), LogLevels.Info, "Task_XrayShot2");
                            bt = queueXRAY1Station.Dequeue();
                            MotionEnum.EnumXrayShot2 = MotionEnum.XRAYShot2.角三拍照;
                        }
                        break;

                    case MotionEnum.XRAYShot2.角三拍照:

                        if (XRayTubeIF.XRayTube2Status.ActualVoltage == HardwareConfig.Instance.XRayConfig2.PresetVoltage && XRayTubeIF.XRayTube2Status.ActualCurrent == HardwareConfig.Instance.XRayConfig2.PresetCurrent)
                        {
                            try
                            {
                                Thread.Sleep(HardwareConfig.Instance.CameraShotDelay2);

                                //TOOD:替换旧版相机采图  ZhangKF 2021-3-16
                                //ZYImageStruct img1 = bt.Corner2.ShotImage;
                                //int shotTime = Environment.TickCount;
                                //int k = 0;
                                //int iRet1 = VisionSysWrapperIF.ShotAndAverage(ECamTypes.Camera2, _camParam2.PinValue, ref img1, 4, out k);
                                //bt.Corner2.ImgNO = k;
                                //WriteLog(string.Format("第{0}次 角三采图 保存图片, 16位图编号：{1} {2} 耗时 {3} 毫秒", index, bt.Corner2.ImgNO, k, Environment.TickCount - shotTime), LogLevels.Info, "Task_XrayShot2");
                                //string picname = "D:\\Test\\2_" + bt.Sn + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + index + ".jpg";
                                //img1.Save(picname, 1, bt.Corner2.ImgNO);
                                //if (iRet1 == -1)

                                //TOOD:替换旧版相机采图  ZhangKF 2021-3-16
                                var result = CameraHelper.CaptureOneImage(1);
                                if (!result.Result)
                                {
                                    bt.Corner2.IsShotOK = false;
                                    WriteLog(string.Format("第{0}次 角三采图失败", index), LogLevels.Info, "Task_XrayShot2");
                                }
                                else
                                {
                                    bt.Corner2.ShotImage = result.Bitmap.ToTransfor(Consts.ImageTypes.Sixteen);
                                    bt.Corner2.IsShotOK = true;
                                }
                                m_bAlgoCorner2Finished = false;
                                ThreadPool.QueueUserWorkItem(state => TestCorner2(ref bt, index));
                                plc.WriteByVariableName("Battery3CornerPhotoComplete", 1);
                                MotionEnum.EnumXrayShot2 = MotionEnum.XRAYShot2.角三拍照完成;
                            }
                            catch (Exception ex)
                            {
                                WriteLog(string.Format("第{0}次 角三采图失败 {1}", index, ex), LogLevels.Error, "Task_XrayShot2");
                                Thread.Sleep(3000);
                                continue;
                            }
                        }
                        break;

                    case MotionEnum.XRAYShot2.角三拍照完成:

                        if (plc.ReadByVariableName("Battery3CornerPhotoSignal") == "0")
                        {
                            //WriteLog(string.Format("第{0}次 {1} ", index, strStep), LogLevels.Info, "Task_XrayShot2");
                            WriteLog(string.Format("第{0}次 {1} 耗时 {2} 毫秒", index, strStep, Environment.TickCount - tickCount), LogLevels.Info, "Task_XrayShot2");
                            plc.WriteByVariableName("Battery3CornerPhotoComplete", 0);
                            MotionEnum.EnumXrayShot2 = MotionEnum.XRAYShot2.触发角四拍照;
                        }
                        break;

                    case MotionEnum.XRAYShot2.触发角四拍照:

                        if (plc.ReadByVariableName("Battery4CornerPhotoSignal") == "1")
                        {
                            if (XRayTubeIF.XRayTube2Status.ActualVoltage == HardwareConfig.Instance.XRayConfig2.PresetVoltage && XRayTubeIF.XRayTube2Status.ActualCurrent == HardwareConfig.Instance.XRayConfig2.PresetCurrent)
                            {
                                if (MyCameraSetting.IsVideoOn == true)
                                {
                                    System.Windows.MessageBox.Show("请先关闭相机页面的视频模式!", "提示", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.None,
                            System.Windows.MessageBoxResult.OK, System.Windows.MessageBoxOptions.ServiceNotification);
                                    Thread.Sleep(5000);
                                    continue;
                                }

                                tickCount = Environment.TickCount;
                                WriteLog(string.Format("第{0}次 {1} ", index, strStep), LogLevels.Info, "Task_XrayShot2");
                                Thread.Sleep(HardwareConfig.Instance.CameraShotDelay1);

                                //TODO: 替换旧版相机采图  ZhangKF 2021-3-16
                                //ZYImageStruct img2 = bt.Corner1.ShotImage;
                                //int shotTime = Environment.TickCount;
                                //int k = 0;
                                //int iRet2 = VisionSysWrapperIF.ShotAndAverage(ECamTypes.Camera2, _camParam2.PinValue, ref img2, 4, out k);
                                //bt.Corner1.ImgNO = k;
                                //WriteLog(string.Format("第{0}次 角四采图 保存图片, 16位图编号：{1} {2} {3} 毫秒", index, bt.Corner1.ImgNO, 0, Environment.TickCount - shotTime), LogLevels.Info, "Task_XrayShot2");
                                //string picname = "D:\\Test\\1_" + bt.Sn + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + index + ".jpg";
                                //img2.Save(picname, 1, bt.Corner1.ImgNO);

                                var result = CameraHelper.CaptureOneImage(1);
                                //if (iRet2 == -1)
                                if (!result.Result)
                                {
                                    bt.Corner1.IsShotOK = false;
                                    WriteLog(string.Format("第{0}次 角四采图失败", index), LogLevels.Info, "Task_XrayShot2");
                                }
                                else
                                {
                                    //TODO: 替换旧版相机采图  ZhangKF 2021-3-16
                                    bt.Corner1.ShotImage = result.Bitmap.ToTransfor(Consts.ImageTypes.Sixteen);
                                    bt.Corner1.IsShotOK = true;
                                }
                                m_bAlgoCorner1Finished = false;
                                ThreadPool.QueueUserWorkItem(state => TestCorner1(ref bt, index));
                                plc.WriteByVariableName("Battery4CornerPhotoComplete", 1);

                                #region 先处理一点数据
                                //master电池不上传
                                isMasterCell = false;
                                foreach (var temp in testCodeManager.CodeList)
                                {
                                    if (bt.Sn == temp.BarCode || bt.LengthBarcode == temp.BarCode)
                                    {
                                        isMasterCell = true;
                                        bt.CheckExtension = ECheckExtensions.Local;//本地模式不保存数据到数据库
                                    }
                                }
                                if (ReworkMode == 4)//isOpenBis == false
                                {
                                    bt.CheckExtension = ECheckExtensions.Local;//本地模式不保存数据到数据库
                                }

                                //上传OCV、MDI数据
                                if ((ReworkMode == 0 || ReworkMode == 1) && string.IsNullOrWhiteSpace(bt.MasterType) && plc.ReadByVariableName("NOOCV") == "0")
                                {
                                    UpLoadData(ref bt, "OCV");
                                }
                                #endregion

                                MotionEnum.EnumXrayShot2 = MotionEnum.XRAYShot2.处理XRAY数据;
                            }
                        }
                        break;

                    case MotionEnum.XRAYShot2.处理XRAY数据:

                        if (m_bAlgoCorner1Finished && m_bAlgoCorner2Finished)
                        {
                            WriteLog(string.Format("第{0}次 {1} ", index, strStep), LogLevels.Info, "Task_XrayShot2");

                            #region 处理XRAY数据

                            //ThreadPool.QueueUserWorkItem(state => DataHandle(ref bt, index));

                            if (isOpenBis == true)
                            {
                                if (CheckParamsConfig.Instance.IsCheckMarking == true)
                                {
                                    string[] arrMarking = CheckParamsConfig.Instance.MarkingCurrent.Split(',');
                                    for (int i = 0; i < arrMarking.Length; i++)
                                    {
                                        if (bt.Marking.Contains(arrMarking[i]))
                                        {
                                            bt.NgItem += "Marking拦截";
                                            bt.IsMarkingCell = true;
                                            WriteLog(string.Format("第{0}次 {1} 标记为Marking电池 {2}", index, bt.Sn, arrMarking[i]), LogLevels.Info, "Task_XrayShot2");
                                            break;
                                        }
                                    }
                                }
                                if (CheckParamsConfig.Instance.IsSetMarking == true)
                                {
                                    string strOutBis;
                                    bis.SetMarking(bt.Sn, CheckParamsConfig.Instance.SetMarking, out strOutBis);
                                    WriteLog(string.Format("条码 {0} 的设置marking为 {1} ,Bis返回 {2} ", bt.Sn, CheckParamsConfig.Instance.SetMarking, strOutBis), LogLevels.Info, "Task_XrayShot2");
                                }
                                if (bt.Sn.Contains("+"))
                                {
                                    bisBarcode = bt.Sn.Substring(bt.Sn.Length - 12, 12);
                                }
                                else
                                {
                                    bisBarcode = bt.Sn;
                                }
                            }

                            ////上传OCV、MDI数据
                            //if (plc.ReadByVariableName("NOOCV") == "0" && (ReworkMode == 0 || ReworkMode == 1))
                            //{
                            //    UpLoadData(ref bt, "OCV");
                            //}

                            string strOut;
                            string testType = "IV";
                            int result = -2;
                            if (plc.ReadByVariableName("NOXRAY") == "0")
                            {
                                bt.UpdateResult();
                                //bt.FinalResult = true;
                                WriteLog(string.Format("第{0}次 Final result is {1} {2}-{3}: {4} {5} {6} {7} ",
                                    index,
                                    bt.Sn,
                                    bt.FinalResult,
                                    bt.ResultCode,
                                    bt.Corner1.InspectResults.resultDataMin.iResult,
                                    bt.Corner2.InspectResults.resultDataMin.iResult,
                                    bt.Corner3.InspectResults.resultDataMin.iResult,
                                    bt.Corner4.InspectResults.resultDataMin.iResult), LogLevels.Info, "Task_XrayShot2");

                                WriteLog(string.Format("第{0}次 合成结果图", index), LogLevels.Info, "Task_XrayShot2");
                                try
                                {
                                    AlgoWrapper.Instance.GetResultImage(ref bt);
                                }
                                catch (Exception ex)
                                {
                                    WriteLog(string.Format("第{0}次 合成结果图异常", index), LogLevels.Warn, "Task_XrayShot2");
                                }
                                WriteLog(string.Format("第{0}次 合成结果图完成", index), LogLevels.Info, "Task_XrayShot2");
                                ResultRelay.Instance.CheckResultHandlers(bt);//刷新监控状态页面

                                //位置错误或黑白图
                                if ((bt.ResultCode == EResultCodes.Unknow || bt.ResultCode == EResultCodes.AlgoErr) && plc.ReadByVariableName("NOXRAY") == "0")
                                {
                                    try
                                    {
                                        string path = "D:\\位置错误黑白图\\" + DateTime.Now.ToString("yyyyMM") + "\\" + DateTime.Now.Day + "\\" + DateTime.Now.Hour;
                                        string fileAllName = path + "\\" + bt.Sn + DateTime.Now.ToString("yyyyMMddHHmmss") + ".jpg";
                                        if (!Directory.Exists(path))
                                        {
                                            Directory.CreateDirectory(path);
                                        }

                                        //TODO: 替换旧版采图 ZhangKF 2021-3-16
                                        //bt.Corner1.ShotImage.Save(path + "\\" + bt.Sn + DateTime.Now.ToString("yyyyMMddHHmmss") + "_1.jpg", 1, bt.Corner1.ImgNO);
                                        //bt.Corner2.ShotImage.Save(path + "\\" + bt.Sn + DateTime.Now.ToString("yyyyMMddHHmmss") + "_2.jpg", 1, bt.Corner2.ImgNO);
                                        //bt.Corner3.ShotImage.Save(path + "\\" + bt.Sn + DateTime.Now.ToString("yyyyMMddHHmmss") + "_3.jpg", 1, bt.Corner3.ImgNO);
                                        //bt.Corner4.ShotImage.Save(path + "\\" + bt.Sn + DateTime.Now.ToString("yyyyMMddHHmmss") + "_4.jpg", 1, bt.Corner4.ImgNO);
                                        bt.Corner1.ShotImage.Save(path + "\\" + bt.Sn + DateTime.Now.ToString("yyyyMMddHHmmss") + "_1.jpg", Consts.ImageTypes.Sixteen);
                                        bt.Corner2.ShotImage.Save(path + "\\" + bt.Sn + DateTime.Now.ToString("yyyyMMddHHmmss") + "_2.jpg", Consts.ImageTypes.Sixteen);
                                        bt.Corner3.ShotImage.Save(path + "\\" + bt.Sn + DateTime.Now.ToString("yyyyMMddHHmmss") + "_3.jpg", Consts.ImageTypes.Sixteen);
                                        bt.Corner4.ShotImage.Save(path + "\\" + bt.Sn + DateTime.Now.ToString("yyyyMMddHHmmss") + "_4.jpg", Consts.ImageTypes.Sixteen);
                                        bt.ResultImage.Save(fileAllName);

                                    }
                                    catch (Exception ex)
                                    {
                                        WriteLog(string.Format("第{0}次 保存黑白图异常 {1}", index, ex), LogLevels.Error, "Task_XrayShot2");
                                    }
                                    bt.CheckExtension = ECheckExtensions.Local;
                                    bt.NgItem += "XRAY位置错误黑白图/";
                                }

                                try
                                {
                                    bool isErr = false;
                                    if (bt.ResultCode == EResultCodes.AlgoErr || bt.ResultCode == EResultCodes.Unknow)
                                    {
                                        isErr = true;
                                    }
                                    else
                                    {
                                        if (isABCell == false)
                                        {
                                            SaveXRayResultToFile(bt, CheckParamsConfig.Instance.TotalLayer, CheckParamsConfig.Instance.TotalLayersBD, 0, isErr);
                                        }
                                        else
                                        {
                                            if (bt.CellType == "B")
                                            {
                                                SaveXRayResultToFile(bt, CheckParamsConfig.Instance.TotalLayersBD, CheckParamsConfig.Instance.TotalLayer, 0, isErr);
                                            }
                                            else
                                            {
                                                SaveXRayResultToFile(bt, CheckParamsConfig.Instance.TotalLayer, CheckParamsConfig.Instance.TotalLayersBD, 0, isErr);
                                            }
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    WriteLog(string.Format("在第{0}次 保存XRay测量数据出错: {1}", index, ex.Message), LogLevels.Error, "Task_XrayShot2");
                                }

                                if (bt.FinalResult == false && bt.Sn != "ERROR" && bt.Sn != string.Empty && bt.ResultCode != EResultCodes.AlgoErr && bt.ResultCode != EResultCodes.Unknow/* && plc.ReadByVariableName("NOXRAY") == "0"*/)
                                {
                                    WriteLog(string.Format("第{0}次 检测NG, 数据缓存待人工复判后再上传", index), LogLevels.Info, "Task_XrayShot2");
                                    List<string> outpuParamItems = ConertersUtils.BatterySeatToBatteryTestData(bt, 2, CheckParamsConfig.Instance.TotalLayer, CheckParamsConfig.Instance.TotalLayersBD, true);
                                    string json = string.Empty;
                                    InterfaceClient.Current.A013Recheck("Normal", bt.Sn, "OK", outpuParamItems, out json);

                                    ProductionDataXray productData = new ProductionDataXray();
                                    productData.ProductSN = bt.Sn;
                                    productData.JsonData = json;

                                    productData.KeyValue = bt.Corner1.InspectParams.min_length.ToString() + "_" + bt.Corner1.InspectParams.max_length.ToString();

                                    BatteryCheckIF.MyProductionDataRecheck.AddProductionData(productData);
                                    bt.NgItem += "XRAYNG/";
                                }
                                if (isMasterCell == false/* && plc.ReadByVariableName("NOXRAY") == "0"*/)
                                {
                                    if (ReworkMode == 0 || ReworkMode == 1)
                                    {
                                        if (bt.MesOCVResult)
                                        {
                                            //上传XRAY数据
                                            UpLoadData(ref bt, "XRAY");
                                        }
                                    }
                                    else if (ReworkMode == 2)
                                    {
                                        UpLoadData(ref bt, "XRAY");
                                    }
                                }
                                _extRunEmpty.OnCheckFinished(bt, _imageSaveConfig, ref _checkStatus);//XRayClient.Core\CheckLogic\Extensions\CheckLogicExtRunEmpty.cs


                                if (bt.Sn != "ERROR"
                                    && bt.Sn != string.Empty
                                    && bt.ResultCode != EResultCodes.Unknow
                                    && bt.ResultCode != EResultCodes.AlgoErr && isMasterCell == false && plc.ReadByVariableName("NOXRAY") == "0") //算法位置错误或黑白图不上传mes
                                {
                                    #region   A085接口,给电池标返工标记
                                    //if (CheckParamsConfig.Instance.IsSamplingMode == true)
                                    //{
                                    //    List<string> SERIALNOS = new List<string>();
                                    //    SERIALNOS.Add(bt.Sn);
                                    //    List<string> DEFECTCODES = new List<string>();
                                    //    DEFECTCODES.Add("XRAYCJ");
                                    //    ATL.MES.A086.Root root_86 = InterfaceClient.Current.A085(SERIALNOS, DEFECTCODES);

                                    //    if (root_86 == null)
                                    //    {
                                    //        bt.Sn = "ERROR";
                                    //        WriteLog(string.Format("第{0}次 A085异常", index), LogLevels.Info, "GetNGPostionB");
                                    //    }
                                    //    else
                                    //    {
                                    //        //_resultSeatArray[m_uBarcodeIndex % lengthArray].StfResult = (root_86.ResponseInfo.Result == "OK");
                                    //        WriteLog(string.Format("第{0}次 A085结果为 {1}", index, root_86.ResponseInfo.Result), LogLevels.Info, "GetNGPostionB");
                                    //    }
                                    //}
                                    #endregion
                                    #region A013接口
                                    //DateTime startTime1 = DateTime.Now;
                                    //List<string> outpuParamItems = new List<string>();
                                    //if (CheckParamsConfig.Instance.IsCheckABCell == false)
                                    //{
                                    //    outpuParamItems = ConertersUtils.BatterySeatToBatteryTestData(bt, 2, CheckParamsConfig.Instance.TotalLayer, CheckParamsConfig.Instance.TotalLayersBD);
                                    //}
                                    //else
                                    //{
                                    //    if (bt.CellType == "A")
                                    //    {
                                    //        outpuParamItems = ConertersUtils.BatterySeatToBatteryTestData(bt, 2, CheckParamsConfig.Instance.TotalLayer, CheckParamsConfig.Instance.TotalLayersBD);
                                    //    }
                                    //    else
                                    //    {
                                    //        outpuParamItems = ConertersUtils.BatterySeatToBatteryTestData(bt, 2, CheckParamsConfig.Instance.TotalLayersBD, CheckParamsConfig.Instance.TotalLayer);
                                    //    }
                                    //}
                                    //string Pass = bt.FinalResult ? "OK" : "NG";
                                    //string json = string.Empty;
                                    //ATL.MES.A014_minicell.Root root_14 = ATL.MES.InterfaceClient.Current.A013("Normal", bt.LengthBarcode, Pass, outpuParamItems, out json);
                                    //if (root_14 == null)
                                    //{
                                    //    WriteLog(string.Format("第{0}次 上传MES失败(A013接口)，离线缓存", index), LogLevels.Info, "Task_XrayShot2");
                                    //    bt.MesResult = false;
                                    //    ProductionDataXray productData = new ProductionDataXray();
                                    //    productData.ProductSN = bt.LengthBarcode;
                                    //    productData.JsonData = json;
                                    //    BatteryCheckIF.MyProductionDataOffline.AddProductionData(productData);
                                    //}
                                    //else
                                    //{
                                    //    DateTime endTime1 = DateTime.Now;
                                    //    WriteLog(string.Format("第{0}次 上传MES成功(A013接口), 耗时 = {1} 毫秒,返回值：{2}", index, (endTime1 - startTime1).TotalMilliseconds, root_14.ResponseInfo.Products[0].Pass), LogLevels.Info, "Task_Sorting");
                                    //    bt.MesResult = root_14.ResponseInfo.Products[0].Pass == "OK" ? true : false;
                                    //}
                                    #endregion

                                    #region Bis代码
                                    if (isOpenBis == true && isMasterCell == false)
                                    {
                                        //XRAY
                                        //-2：位置错误黑白图，-5褶皱，-6缺层，-7长度NG，-8：角度NG
                                        string xRayOKOrNG = "OK";
                                        string ngType = "";
                                        int ngCode = 0;
                                        if (bt.FinalResult == false)
                                        {
                                            xRayOKOrNG = "NG";
                                            if (bt.Corner1.InspectResults.resultDataMin.iResult != 1 && bt.Corner1.InspectResults.resultDataMin.iResult != -2)
                                            {
                                                ngCode = bt.Corner1.InspectResults.resultDataMin.iResult;
                                            }
                                            if (bt.Corner2.InspectResults.resultDataMin.iResult != 1 && bt.Corner2.InspectResults.resultDataMin.iResult != -2)
                                            {
                                                ngCode = bt.Corner2.InspectResults.resultDataMin.iResult;
                                            }
                                            if (bt.Corner3.InspectResults.resultDataMin.iResult != 1 && bt.Corner3.InspectResults.resultDataMin.iResult != -2)
                                            {
                                                ngCode = bt.Corner3.InspectResults.resultDataMin.iResult;
                                            }
                                            if (bt.Corner4.InspectResults.resultDataMin.iResult != 1 && bt.Corner4.InspectResults.resultDataMin.iResult != -2)
                                            {
                                                ngCode = bt.Corner4.InspectResults.resultDataMin.iResult;
                                            }

                                            switch (ngCode)
                                            {
                                                case -5:
                                                    ngType = "Anode Broken";
                                                    break;
                                                case -6:
                                                    ngType = "Lost Layer";
                                                    break;
                                                case -7:
                                                    ngType = "OH_NG";
                                                    break;
                                                case -8:
                                                    ngType = "Andle NG";
                                                    break;
                                                default:
                                                    ngType = "NG";
                                                    break;
                                            }
                                        }
                                        string max = bt.Corner1.InspectResults.resultDataMin.fMaxDis.ToString("0.##") + "_" + bt.Corner2.InspectResults.resultDataMin.fMaxDis.ToString("0.##") + "_" + bt.Corner3.InspectResults.resultDataMin.fMaxDis.ToString("0.##") + "_" + bt.Corner4.InspectResults.resultDataMin.fMaxDis.ToString("0.##");
                                        string min = bt.Corner1.InspectResults.resultDataMin.fMinDis.ToString("0.##") + "_" + bt.Corner2.InspectResults.resultDataMin.fMinDis.ToString("0.##") + "_" + bt.Corner3.InspectResults.resultDataMin.fMinDis.ToString("0.##") + "_" + bt.Corner4.InspectResults.resultDataMin.fMinDis.ToString("0.##");
                                        float minLength = Math.Min(CheckParamsConfig.Instance.MinLengthTail, CheckParamsConfig.Instance.MinLengthHead);
                                        float maxLength = Math.Max(CheckParamsConfig.Instance.MaxLengthTail, CheckParamsConfig.Instance.MaxLengthHead);

                                        if (bt.FinalResult == false)
                                        {
                                            result = bis.BIS_TransfXRayDataNew(bisBarcode, OperaterId, xRayMachinNo, "0", max.ToString(), min.ToString(), xRayOKOrNG, ngType, ngCode.ToString(), "", minLength.ToString(), maxLength.ToString(), "", out strOut);
                                        }
                                        else
                                        {
                                            result = bis.BIS_TransfXRayDataNew(bisBarcode, OperaterId, xRayMachinNo, "0", max.ToString(), min.ToString(), xRayOKOrNG, "", "", "", minLength.ToString(), maxLength.ToString(), "", out strOut);
                                        }
                                        WriteLog("XRAYBIS数据上传返回 " + result + ":" + strOut, LogLevels.Info, "Task_XrayShot2");

                                        if (result == 3 || result == 4 || result == 6 || result == 9)
                                        {
                                            bt.MesResult = false;
                                        }
                                        while (!File.Exists(bt.ResultImgSavePath + "\\" + bt.ResultImgFileName))
                                        {
                                            Thread.Sleep(50);
                                        }
                                        bool imgResult = bis.BISXRayPicUpload(bt.Sn, bt.ResultImgSavePath + "\\" + bt.ResultImgFileName, bt.ResultImgFileName, xRayMachinNo, "0", OperaterId, out strOut);
                                        WriteLog("路径： " + bt.ResultImgSavePath + ":" + bt.ResultImgFileName, LogLevels.Info, "Task_XrayShot2");
                                        WriteLog("XRAYBIS图片上传返回 " + imgResult + ":" + strOut, LogLevels.Info, "Task_XrayShot2");
                                        if (imgResult == false)
                                        {
                                            bt.MesResult = false;
                                        }
                                    }
                                    #endregion
                                }

                                try
                                {
                                    _extSTF.OnSaveImages(bt, _imageSaveConfig);//XRayClient.Core\CheckLogic\Extensions\CheckLogicExtSTF.cs
                                }
                                catch (Exception ex)
                                {
                                    System.Windows.MessageBox.Show("磁盘已满，无法继续保存图片，请清理再启动!", "提示", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.None,
                        System.Windows.MessageBoxResult.OK, System.Windows.MessageBoxOptions.ServiceNotification);
                                    bt.MesResult = false;
                                }

                            }
                            if (isMasterCell == false && plc.ReadByVariableName("NOXRAY") == "0" && CheckParamsConfig.Instance.IsNoUpLoadMdiAndPPGData == false)
                            {
                                if (bt.MesOCVResult && bt.MesXRAYResult && (ReworkMode == 0 || ReworkMode == 1))
                                {
                                    UpLoadData(ref bt, "FQI");
                                }
                                else if (bt.MesXRAYResult && ReworkMode == 2)
                                {
                                    UpLoadData(ref bt, "FQI");
                                }
                                else if (ReworkMode == 3)
                                {
                                    UpLoadData(ref bt, "FQI");
                                }
                            }

                            if (bt.MesOCVResult == false || bt.MesMDIResult == false || bt.MesXRAYResult == false)
                            {
                                bt.MesResult = false;
                            }
                            else
                            {
                                bt.MesResult = true;
                            }

                            #region Bis代码
                            if (isOpenBis == true && isMasterCell == false)
                            {
                                //IV
                                if (plc.ReadByVariableName("NOIV") == "0")
                                {
                                    if (bt.KnifeNoConduction == false && bt.NeedleNoConduction == false)//导通
                                    {
                                        result = bis.TransferDataExTest(bisBarcode, testType, machinNo, "0", bt.IvData, "0", out strOut);
                                        WriteLog("IVBIS返回 " + result + ":" + strOut, LogLevels.Info, "Task_XrayShot2");
                                        if (result != 0)
                                        {
                                            //bt.MesResult = false;
                                            bt.IVResult = false;
                                        }
                                    }
                                }

                                //OCV
                                if (plc.ReadByVariableName("NOOCV") == "0")
                                {
                                    testType = OCVMode.ToString().Substring(OCVMode.ToString().Length - 2, 2);
                                    int outResult1;
                                    int outResult2;
                                    bool ocvResult = bis.AutoTransfData(testType, bisBarcode, bt.Voltage, bt.Resistance, bt.Temperature, "1-1", "none", bt.Voltage, bt.Resistance, bt.Temperature,
                                        "1-2", machinNo, out outResult1, out outResult2, bt.EnvirementTemperature.ToString(), bt.EnvirementTemperature.ToString(), out strOut);
                                    WriteLog(testType + " BIS返回 " + outResult1 + ":" + strOut, LogLevels.Info, "Task_XrayShot2");

                                    if (ocvResult == false)
                                    {
                                        bt.OCVResult = false;
                                        if (outResult1 == 1)
                                        {
                                            bt.VoltageResult = false;
                                        }
                                        else if (outResult1 == 2)
                                        {
                                            isKValueNG = true;
                                            bt.NgItem += "K值NG/";
                                        }
                                        else if (outResult1 == 3)
                                        {
                                            bt.ResistanceResult = false;
                                        }
                                        else if (outResult1 == 4)
                                        {
                                            bt.NgItem += "超时或缺项/";
                                        }
                                        else
                                        {
                                            bt.MesResult = false;
                                            bt.NgItem += "OCVBis未知NG/";
                                        }
                                    }
                                    else
                                    {
                                        bt.OCVResult = true;
                                    }
                                    bt.OcvRemark = outResult1 + "-" + strOut;
                                    SaveOCVDataToFileBySetp(bt);
                                }

                                if (CheckParamsConfig.Instance.IsNoUpLoadMdiAndPPGData == false)
                                {
                                    string flag = "NG";
                                    if (bt.DimensionResult == true && bt.ThicknessResult == true)
                                    {
                                        flag = "OK";
                                    }
                                    if (CheckParamsConfig.Instance.IsAlOnLeft == true)//Al极耳在左
                                    {
                                        result = bis.BIS_TransfMylarData_New(bisBarcode, bt.Thickness.ToString(), bt.BatLength.ToString(), bt.BatWidth.ToString(), "None",
                                               "None", bt.RightLugMargin.ToString(), bt.LeftLugMargin.ToString(), "None", xRayMachinNo, bt.LeftLugLength.ToString(), bt.RightLugLength.ToString(), "-999.999", "-999.999", "None", flag, "None", "None", "None", "None", "None", "None",
                                               "", bt.Right1WhiteGlue.ToString(), bt.Right2WhiteGlue.ToString(), bt.Left1WhiteGlue.ToString(), bt.Left2WhiteGlue.ToString(), "", "", "", "", OperaterId, "", bt.AllBatLength.ToString(), out strOut);

                                    }
                                    else
                                    {
                                        result = bis.BIS_TransfMylarData_New(bisBarcode, bt.Thickness.ToString(), bt.BatLength.ToString(), bt.BatWidth.ToString(), "None",
                                                "None", bt.LeftLugMargin.ToString(), bt.RightLugMargin.ToString(), "None", xRayMachinNo, bt.RightLugLength.ToString(), bt.LeftLugLength.ToString(), "-999.999", "-999.999", "None", flag, "None", "None", "None", "None", "None", "None",
                                                "", bt.Left1WhiteGlue.ToString(), bt.Left2WhiteGlue.ToString(), bt.Right1WhiteGlue.ToString(), bt.Right2WhiteGlue.ToString(), "", "", "", "", OperaterId, "", bt.AllBatLength.ToString(), out strOut);

                                    }

                                    WriteLog("MDIBIS返回 " + result + ":" + strOut, LogLevels.Info, "Task_XrayShot2");
                                    if (result != 1)
                                    {
                                        //bt.MesResult = false;
                                        bt.DimensionResult = false;
                                    }
                                }
                            }
                            #endregion


                            #endregion

                            queueXRAY2Station.Enqueue(bt);
                            //plc.WriteByVariableName("Battery4CornerPhotoComplete", 1);
                            MotionEnum.EnumXrayShot2 = MotionEnum.XRAYShot2.角四拍照完成;

                        }
                        break;

                    case MotionEnum.XRAYShot2.角四拍照完成:

                        if (plc.ReadByVariableName("Battery4CornerPhotoSignal") == "0")
                        {
                            WriteLog(string.Format("第{0}次 {1} ,共耗时：{2} 毫秒", index, strStep, Environment.TickCount - tickCount), LogLevels.Info, "Task_XrayShot2");
                            plc.WriteByVariableName("Battery4CornerPhotoComplete", 0);
                            MotionEnum.EnumXrayShot2 = MotionEnum.XRAYShot2.触发角三拍照;

                        }
                        break;

                }
            }
        }

        private void DataHandle(ref BatterySeat bt, int index)
        {
            string bisBarcode = "";
            bool isMasterCell = false;//是否为master电池

            int tickCount = Environment.TickCount;
            WriteLog(string.Format("第{0}次 进入数据处理线程", index), LogLevels.Info, "DataHandle");
            if (isOpenBis == true)
            {
                if (CheckParamsConfig.Instance.IsCheckMarking == true)
                {
                    string[] arrMarking = CheckParamsConfig.Instance.MarkingCurrent.Split(',');
                    for (int i = 0; i < arrMarking.Length; i++)
                    {
                        if (bt.Marking.Contains(arrMarking[i]))
                        {
                            bt.NgItem += "Marking拦截";
                            bt.IsMarkingCell = true;
                            WriteLog(string.Format("第{0}次 {1} 标记为Marking电池 {2}", index, bt.Sn, arrMarking[i]), LogLevels.Info, "DataHandle");
                            break;
                        }
                    }
                }
                if (CheckParamsConfig.Instance.IsSetMarking == true)
                {
                    string strOutBis;
                    bis.SetMarking(bt.Sn, CheckParamsConfig.Instance.SetMarking, out strOutBis);
                    WriteLog(string.Format("条码 {0} 的设置marking为 {1} ,Bis返回 {2} ", bt.Sn, CheckParamsConfig.Instance.SetMarking, strOutBis), LogLevels.Info, "DataHandle");
                }
                if (bt.Sn.Contains("+"))
                {
                    bisBarcode = bt.Sn.Substring(bt.Sn.Length - 12, 12);
                }
                else
                {
                    bisBarcode = bt.Sn;
                }
            }

            //上传OCV、NDI数据
            if (plc.ReadByVariableName("NOOCV") == "0" && (ReworkMode == 0 || ReworkMode == 1))
            {
                UpLoadData(ref bt, "OCV");
            }

            string strOut;
            string testType = "IV";
            int result = -2;
            if (plc.ReadByVariableName("NOXRAY") == "0")
            {
                bt.UpdateResult();
                //bt.FinalResult = true;
                WriteLog(string.Format("第{0}次 Final result is {1} {2}-{3}: {4} {5} {6} {7} ",
                    index,
                    bt.Sn,
                    bt.FinalResult,
                    bt.ResultCode,
                    bt.Corner1.InspectResults.resultDataMin.iResult,
                    bt.Corner2.InspectResults.resultDataMin.iResult,
                    bt.Corner3.InspectResults.resultDataMin.iResult,
                    bt.Corner4.InspectResults.resultDataMin.iResult), LogLevels.Info, "DataHandle");

                WriteLog(string.Format("第{0}次 合成结果图", index), LogLevels.Info, "DataHandle");
                try
                {
                    AlgoWrapper.Instance.GetResultImage(ref bt);
                }
                catch (Exception ex)
                {
                    WriteLog(string.Format("第{0}次 合成结果图异常", index), LogLevels.Warn, "DataHandle");
                }
                WriteLog(string.Format("第{0}次 合成结果图完成", index), LogLevels.Info, "DataHandle");
                ResultRelay.Instance.CheckResultHandlers(bt);//刷新监控状态页面

                //master电池不上传
                isMasterCell = false;
                foreach (var temp in testCodeManager.CodeList)
                {
                    if (bt.Sn == temp.BarCode || bt.LengthBarcode == temp.BarCode)
                    {
                        isMasterCell = true;
                        bt.CheckExtension = ECheckExtensions.Local;//本地模式不保存数据到数据库
                    }
                }
                if (ReworkMode == 4)//isOpenBis == false
                {
                    bt.CheckExtension = ECheckExtensions.Local;//本地模式不保存数据到数据库
                }

                //位置错误或黑白图
                if ((bt.ResultCode == EResultCodes.Unknow || bt.ResultCode == EResultCodes.AlgoErr) && plc.ReadByVariableName("NOXRAY") == "0")
                {
                    try
                    {
                        string path = "D:\\位置错误黑白图\\" + DateTime.Now.ToString("yyyyMM") + "\\" + DateTime.Now.Day + "\\" + DateTime.Now.Hour;
                        string fileAllName = path + "\\" + bt.Sn + DateTime.Now.ToString("yyyyMMddHHmmss") + ".jpg";
                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }
                        //TODO: 替换旧版采图 ZhangKF 2021-3-16
                        //bt.Corner1.ShotImage.Save(path + "\\" + bt.Sn + DateTime.Now.ToString("yyyyMMddHHmmss") + "_1.jpg", 1, bt.Corner1.ImgNO);
                        //bt.Corner2.ShotImage.Save(path + "\\" + bt.Sn + DateTime.Now.ToString("yyyyMMddHHmmss") + "_2.jpg", 1, bt.Corner2.ImgNO);
                        //bt.Corner3.ShotImage.Save(path + "\\" + bt.Sn + DateTime.Now.ToString("yyyyMMddHHmmss") + "_3.jpg", 1, bt.Corner3.ImgNO);
                        //bt.Corner4.ShotImage.Save(path + "\\" + bt.Sn + DateTime.Now.ToString("yyyyMMddHHmmss") + "_4.jpg", 1, bt.Corner4.ImgNO);
                        bt.Corner1.ShotImage.Save(path + "\\" + bt.Sn + DateTime.Now.ToString("yyyyMMddHHmmss") + "_1.jpg", Consts.ImageTypes.Sixteen);
                        bt.Corner2.ShotImage.Save(path + "\\" + bt.Sn + DateTime.Now.ToString("yyyyMMddHHmmss") + "_2.jpg", Consts.ImageTypes.Sixteen);
                        bt.Corner3.ShotImage.Save(path + "\\" + bt.Sn + DateTime.Now.ToString("yyyyMMddHHmmss") + "_3.jpg", Consts.ImageTypes.Sixteen);
                        bt.Corner4.ShotImage.Save(path + "\\" + bt.Sn + DateTime.Now.ToString("yyyyMMddHHmmss") + "_4.jpg", Consts.ImageTypes.Sixteen);
                        bt.ResultImage.Save(fileAllName);
                    }
                    catch (Exception ex)
                    {
                        WriteLog(string.Format("第{0}次 保存黑白图异常 {1}", index, ex), LogLevels.Error, "DataHandle");
                    }
                    bt.CheckExtension = ECheckExtensions.Local;
                    bt.NgItem += "XRAY位置错误黑白图/";
                }

                try
                {
                    bool isErr = false;
                    if (bt.ResultCode == EResultCodes.AlgoErr || bt.ResultCode == EResultCodes.Unknow)
                    {
                        isErr = true;
                    }
                    else
                    {
                        if (isABCell == false)
                        {
                            SaveXRayResultToFile(bt, CheckParamsConfig.Instance.TotalLayer, CheckParamsConfig.Instance.TotalLayersBD, 0, isErr);
                        }
                        else
                        {
                            if (bt.CellType == "B")
                            {
                                SaveXRayResultToFile(bt, CheckParamsConfig.Instance.TotalLayersBD, CheckParamsConfig.Instance.TotalLayer, 0, isErr);
                            }
                            else
                            {
                                SaveXRayResultToFile(bt, CheckParamsConfig.Instance.TotalLayer, CheckParamsConfig.Instance.TotalLayersBD, 0, isErr);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    WriteLog(string.Format("在第{0}次 保存XRay测量数据出错: {1}", index, ex.Message), LogLevels.Error, "DataHandle");
                }

                if (bt.FinalResult == false && bt.Sn != "ERROR" && bt.Sn != string.Empty && bt.ResultCode != EResultCodes.AlgoErr && bt.ResultCode != EResultCodes.Unknow && plc.ReadByVariableName("NOXRAY") == "0")
                {
                    WriteLog(string.Format("第{0}次 检测NG, 数据缓存待人工复判后再上传", index), LogLevels.Info, "Task_Sorting");
                    List<string> outpuParamItems = ConertersUtils.BatterySeatToBatteryTestData(bt, 2, CheckParamsConfig.Instance.TotalLayer, CheckParamsConfig.Instance.TotalLayersBD, true);
                    string json = string.Empty;
                    InterfaceClient.Current.A013Recheck("Normal", bt.Sn, "OK", outpuParamItems, out json);

                    ProductionDataXray productData = new ProductionDataXray();
                    productData.ProductSN = bt.Sn;
                    productData.JsonData = json;

                    productData.KeyValue = bt.Corner1.InspectParams.min_length.ToString() + "_" + bt.Corner1.InspectParams.max_length.ToString();

                    BatteryCheckIF.MyProductionDataRecheck.AddProductionData(productData);
                    bt.NgItem += "XRAYNG/";
                }
                if (plc.ReadByVariableName("NOXRAY") == "0")
                {
                    if (ReworkMode == 0 || ReworkMode == 1)
                    {
                        if (bt.MesOCVResult)
                        {
                            //上传XRAY数据
                            UpLoadData(ref bt, "XRAY");
                        }
                    }
                    else if (ReworkMode == 2)
                    {
                        UpLoadData(ref bt, "XRAY");
                    }
                }
                _extRunEmpty.OnCheckFinished(bt, _imageSaveConfig, ref _checkStatus);//XRayClient.Core\CheckLogic\Extensions\CheckLogicExtRunEmpty.cs


                if (bt.Sn != "ERROR"
                    && bt.Sn != string.Empty
                    && bt.ResultCode != EResultCodes.Unknow
                    && bt.ResultCode != EResultCodes.AlgoErr && isMasterCell == false && plc.ReadByVariableName("NOXRAY") == "0") //算法位置错误或黑白图不上传mes
                {
                    #region   A085接口,给电池标返工标记
                    //if (CheckParamsConfig.Instance.IsSamplingMode == true)
                    //{
                    //    List<string> SERIALNOS = new List<string>();
                    //    SERIALNOS.Add(bt.Sn);
                    //    List<string> DEFECTCODES = new List<string>();
                    //    DEFECTCODES.Add("XRAYCJ");
                    //    ATL.MES.A086.Root root_86 = InterfaceClient.Current.A085(SERIALNOS, DEFECTCODES);

                    //    if (root_86 == null)
                    //    {
                    //        bt.Sn = "ERROR";
                    //        WriteLog(string.Format("第{0}次 A085异常", index), LogLevels.Info, "GetNGPostionB");
                    //    }
                    //    else
                    //    {
                    //        //_resultSeatArray[m_uBarcodeIndex % lengthArray].StfResult = (root_86.ResponseInfo.Result == "OK");
                    //        WriteLog(string.Format("第{0}次 A085结果为 {1}", index, root_86.ResponseInfo.Result), LogLevels.Info, "GetNGPostionB");
                    //    }
                    //}
                    #endregion
                    #region A013接口
                    //DateTime startTime1 = DateTime.Now;
                    //List<string> outpuParamItems = new List<string>();
                    //if (CheckParamsConfig.Instance.IsCheckABCell == false)
                    //{
                    //    outpuParamItems = ConertersUtils.BatterySeatToBatteryTestData(bt, 2, CheckParamsConfig.Instance.TotalLayer, CheckParamsConfig.Instance.TotalLayersBD);
                    //}
                    //else
                    //{
                    //    if (bt.CellType == "A")
                    //    {
                    //        outpuParamItems = ConertersUtils.BatterySeatToBatteryTestData(bt, 2, CheckParamsConfig.Instance.TotalLayer, CheckParamsConfig.Instance.TotalLayersBD);
                    //    }
                    //    else
                    //    {
                    //        outpuParamItems = ConertersUtils.BatterySeatToBatteryTestData(bt, 2, CheckParamsConfig.Instance.TotalLayersBD, CheckParamsConfig.Instance.TotalLayer);
                    //    }
                    //}
                    //string Pass = bt.FinalResult ? "OK" : "NG";
                    //string json = string.Empty;
                    //ATL.MES.A014_minicell.Root root_14 = ATL.MES.InterfaceClient.Current.A013("Normal", bt.LengthBarcode, Pass, outpuParamItems, out json);
                    //if (root_14 == null)
                    //{
                    //    WriteLog(string.Format("第{0}次 上传MES失败(A013接口)，离线缓存", index), LogLevels.Info, "Task_Sorting");
                    //    bt.MesResult = false;
                    //    ProductionDataXray productData = new ProductionDataXray();
                    //    productData.ProductSN = bt.LengthBarcode;
                    //    productData.JsonData = json;
                    //    BatteryCheckIF.MyProductionDataOffline.AddProductionData(productData);
                    //}
                    //else
                    //{
                    //    DateTime endTime1 = DateTime.Now;
                    //    WriteLog(string.Format("第{0}次 上传MES成功(A013接口), 耗时 = {1} 毫秒,返回值：{2}", index, (endTime1 - startTime1).TotalMilliseconds, root_14.ResponseInfo.Products[0].Pass), LogLevels.Info, "Task_Sorting");
                    //    bt.MesResult = root_14.ResponseInfo.Products[0].Pass == "OK" ? true : false;
                    //}
                    #endregion

                    #region Bis代码
                    //if (isOpenBis == true && isMasterCell == false)
                    //{
                    //    //XRAY
                    //    //-2：位置错误黑白图，-5褶皱，-6缺层，-7长度NG，-8：角度NG
                    //    string xRayOKOrNG = "OK";
                    //    string ngType = "";
                    //    int ngCode = 0;
                    //    if (bt.FinalResult == false)
                    //    {
                    //        xRayOKOrNG = "NG";
                    //        if (bt.Corner1.InspectResults.resultDataMin.iResult != 1 && bt.Corner1.InspectResults.resultDataMin.iResult != -2)
                    //        {
                    //            ngCode = bt.Corner1.InspectResults.resultDataMin.iResult;
                    //        }
                    //        if (bt.Corner2.InspectResults.resultDataMin.iResult != 1 && bt.Corner2.InspectResults.resultDataMin.iResult != -2)
                    //        {
                    //            ngCode = bt.Corner2.InspectResults.resultDataMin.iResult;
                    //        }
                    //        if (bt.Corner3.InspectResults.resultDataMin.iResult != 1 && bt.Corner3.InspectResults.resultDataMin.iResult != -2)
                    //        {
                    //            ngCode = bt.Corner3.InspectResults.resultDataMin.iResult;
                    //        }
                    //        if (bt.Corner4.InspectResults.resultDataMin.iResult != 1 && bt.Corner4.InspectResults.resultDataMin.iResult != -2)
                    //        {
                    //            ngCode = bt.Corner4.InspectResults.resultDataMin.iResult;
                    //        }

                    //        switch (ngCode)
                    //        {
                    //            case -5:
                    //                ngType = "Anode Broken";
                    //                break;
                    //            case -6:
                    //                ngType = "Lost Layer";
                    //                break;
                    //            case -7:
                    //                ngType = "OH_NG";
                    //                break;
                    //            case -8:
                    //                ngType = "Andle NG";
                    //                break;
                    //            default:
                    //                ngType = "NG";
                    //                break;
                    //        }
                    //    }
                    //    string max = bt.Corner1.InspectResults.resultDataMin.fMaxDis.ToString("0.##") + "_" + bt.Corner2.InspectResults.resultDataMin.fMaxDis.ToString("0.##") + "_" + bt.Corner3.InspectResults.resultDataMin.fMaxDis.ToString("0.##") + "_" + bt.Corner4.InspectResults.resultDataMin.fMaxDis.ToString("0.##");
                    //    string min = bt.Corner1.InspectResults.resultDataMin.fMinDis.ToString("0.##") + "_" + bt.Corner2.InspectResults.resultDataMin.fMinDis.ToString("0.##") + "_" + bt.Corner3.InspectResults.resultDataMin.fMinDis.ToString("0.##") + "_" + bt.Corner4.InspectResults.resultDataMin.fMinDis.ToString("0.##");
                    //    float minLength = Math.Min(CheckParamsConfig.Instance.MinLengthTail, CheckParamsConfig.Instance.MinLengthHead);
                    //    float maxLength = Math.Max(CheckParamsConfig.Instance.MaxLengthTail, CheckParamsConfig.Instance.MaxLengthHead);

                    //    if (bt.FinalResult == false)
                    //    {
                    //        result = bis.BIS_TransfXRayDataNew(bisBarcode, OperaterId, xRayMachinNo, "0", max.ToString(), min.ToString(), xRayOKOrNG, ngType, ngCode.ToString(), "", minLength.ToString(), maxLength.ToString(), "", out strOut);
                    //    }
                    //    else
                    //    {
                    //        result = bis.BIS_TransfXRayDataNew(bisBarcode, OperaterId, xRayMachinNo, "0", max.ToString(), min.ToString(), xRayOKOrNG, "", "", "", minLength.ToString(), maxLength.ToString(), "", out strOut);
                    //    }
                    //    WriteLog("XRAYBIS数据上传返回 " + result + ":" + strOut, LogLevels.Info, "Task_Sorting");

                    //    if (result == 3 || result == 4 || result == 6 || result == 9)
                    //    {
                    //        bt.MesResult = false;
                    //    }
                    //    while (!File.Exists(bt.ResultImgSavePath + "\\" + bt.ResultImgFileName))
                    //    {
                    //        Thread.Sleep(50);
                    //    }
                    //    bool imgResult = bis.BISXRayPicUpload(bt.Sn, bt.ResultImgSavePath + "\\" + bt.ResultImgFileName, bt.ResultImgFileName, xRayMachinNo, "0", OperaterId, out strOut);
                    //    WriteLog("路径： " + bt.ResultImgSavePath + ":" + bt.ResultImgFileName, LogLevels.Info, "Task_Sorting");
                    //    WriteLog("XRAYBIS图片上传返回 " + imgResult + ":" + strOut, LogLevels.Info, "Task_Sorting");
                    //    if (imgResult == false)
                    //    {
                    //        bt.MesResult = false;
                    //    }
                    //} 
                    #endregion
                }

                try
                {
                    _extSTF.OnSaveImages(bt, _imageSaveConfig);//XRayClient.Core\CheckLogic\Extensions\CheckLogicExtSTF.cs
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show("磁盘已满，无法继续保存图片，请清理再启动!", "提示", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.None,
                            System.Windows.MessageBoxResult.OK, System.Windows.MessageBoxOptions.ServiceNotification);
                    bt.MesResult = false;
                }

            }
            if (plc.ReadByVariableName("NOXRAY") == "0" && CheckParamsConfig.Instance.IsNoUpLoadMdiAndPPGData == false)
            {
                if (bt.MesOCVResult && bt.MesXRAYResult && (ReworkMode == 0 || ReworkMode == 1))
                {
                    UpLoadData(ref bt, "FQI");
                }
                else if (bt.MesXRAYResult && ReworkMode == 2)
                {
                    UpLoadData(ref bt, "FQI");
                }
                else if (ReworkMode == 3)
                {
                    UpLoadData(ref bt, "FQI");
                }
            }

            if (bt.MesOCVResult == false || bt.MesMDIResult == false || bt.MesXRAYResult == false)
            {
                bt.MesResult = false;
            }
            else
            {
                bt.MesResult = true;
            }

            #region Bis代码
            //if (isOpenBis == true && isMasterCell == false)
            //{
            //    //IV
            //    if (plc.ReadByVariableName("NOIV") == "0")
            //    {
            //        if (bt.KnifeNoConduction == false && bt.NeedleNoConduction == false)//导通
            //        {
            //            result = bis.TransferDataExTest(bisBarcode, testType, machinNo, "0", bt.IvData, "0", out strOut);
            //            WriteLog("IVBIS返回 " + result + ":" + strOut, LogLevels.Info, "Task_Sorting");
            //            if (result != 0)
            //            {
            //                //bt.MesResult = false;
            //                bt.IVResult = false;
            //            }
            //        }
            //    }

            //    //OCV
            //    if (plc.ReadByVariableName("NOOCV") == "0")
            //    {
            //        testType = OCVMode.ToString().Substring(OCVMode.ToString().Length - 2, 2);
            //        int outResult1;
            //        int outResult2;
            //        bool ocvResult = bis.AutoTransfData(testType, bisBarcode, bt.Voltage, bt.Resistance, bt.Temperature, "1-1", "none", bt.Voltage, bt.Resistance, bt.Temperature,
            //            "1-2", machinNo, out outResult1, out outResult2, bt.EnvirementTemperature.ToString(), bt.EnvirementTemperature.ToString(), out strOut);
            //        WriteLog(testType + " BIS返回 " + outResult1 + ":" + strOut, LogLevels.Info, "Task_Sorting");

            //        if (ocvResult == false)
            //        {
            //            bt.OCVResult = false;
            //            if (outResult1 == 1)
            //            {
            //                bt.VoltageResult = false;
            //            }
            //            else if (outResult1 == 2)
            //            {
            //                isKValueNG = true;
            //                bt.NgItem += "K值NG/";
            //            }
            //            else if (outResult1 == 3)
            //            {
            //                bt.ResistanceResult = false;
            //            }
            //            else if (outResult1 == 4)
            //            {
            //                bt.NgItem += "超时或缺项/";
            //            }
            //            else
            //            {
            //                bt.MesResult = false;
            //                bt.NgItem += "OCVBis未知NG/";
            //            }
            //        }
            //        else
            //        {
            //            bt.OCVResult = true;
            //        }
            //        bt.OcvRemark = outResult1 + "-" + strOut;
            //        SaveOCVDataToFileBySetp(bt);
            //    }

            //    if (CheckParamsConfig.Instance.IsNoUpLoadMdiAndPPGData == false)
            //    {
            //        string flag = "NG";
            //        if (bt.DimensionResult == true && bt.ThicknessResult == true)
            //        {
            //            flag = "OK";
            //        }
            //        if (CheckParamsConfig.Instance.IsAlOnLeft == true)//Al极耳在左
            //        {
            //            result = bis.BIS_TransfMylarData_New(bisBarcode, bt.Thickness.ToString(), bt.BatLength.ToString(), bt.BatWidth.ToString(), "None",
            //                   "None", bt.RightLugMargin.ToString(), bt.LeftLugMargin.ToString(), "None", xRayMachinNo, bt.LeftLugLength.ToString(), bt.RightLugLength.ToString(), "-999.999", "-999.999", "None", flag, "None", "None", "None", "None", "None", "None",
            //                   "", bt.Right1WhiteGlue.ToString(), bt.Right2WhiteGlue.ToString(), bt.Left1WhiteGlue.ToString(), bt.Left2WhiteGlue.ToString(), "", "", "", "", OperaterId, "", bt.AllBatLength.ToString(), out strOut);

            //        }
            //        else
            //        {
            //            result = bis.BIS_TransfMylarData_New(bisBarcode, bt.Thickness.ToString(), bt.BatLength.ToString(), bt.BatWidth.ToString(), "None",
            //                    "None", bt.LeftLugMargin.ToString(), bt.RightLugMargin.ToString(), "None", xRayMachinNo, bt.RightLugLength.ToString(), bt.LeftLugLength.ToString(), "-999.999", "-999.999", "None", flag, "None", "None", "None", "None", "None", "None",
            //                    "", bt.Left1WhiteGlue.ToString(), bt.Left2WhiteGlue.ToString(), bt.Right1WhiteGlue.ToString(), bt.Right2WhiteGlue.ToString(), "", "", "", "", OperaterId, "", bt.AllBatLength.ToString(), out strOut);

            //        }

            //        WriteLog("MDIBIS返回 " + result + ":" + strOut, LogLevels.Info, "Task_Sorting");
            //        if (result != 1)
            //        {
            //            //bt.MesResult = false;
            //            bt.DimensionResult = false;
            //        }
            //    }
            //} 
            #endregion

            bt.DataFinish = true;
            queueXRAY2Station.Enqueue(bt);
            WriteLog(string.Format("第{0}次 数据处理耗时 {1} 毫秒", index, Environment.TickCount - tickCount), LogLevels.Info, "DataHandle");
        }

        private void Task_Sorting()
        {
            //bool isKValueNG = false;
            string strStep = "";
            BatterySeat bt = null;
            int tickCount = 0;
            int index = 0;
            string bisBarcode = "";
            int xrayCheckCount = 0;
            int ivCheckCount = 0;

            plc.WriteByVariableName("ResultOK", 0);
            plc.WriteByVariableName("IVNG", 0);
            plc.WriteByVariableName("IVNoConduction", 0);
            plc.WriteByVariableName("OtherNG", 0);
            plc.WriteByVariableName("VoltageNG", 0);
            plc.WriteByVariableName("ResistanceNG", 0);
            plc.WriteByVariableName("KNG", 0);
            plc.WriteByVariableName("MDING", 0);
            plc.WriteByVariableName("ThicknessNG", 0);
            plc.WriteByVariableName("XRAYNG", 0);
            MotionEnum.EnumSorting = MotionEnum.Sorting.触发分拣;
            bool isMasterCell = false;//是否为master电池
            string xRayMachinNo = UserDefineVariableInfo.DicVariables["AssetsNO"].ToString();

            while (true)
            {
                Thread.Sleep(10);
                if (_AutoRunning == false)
                {
                    WriteLog("分拣线程退出", LogLevels.Info, "Task_Sorting");
                    return;
                }
                if (plc.ReadByVariableName("SoftwareReset") == "1")
                {
                    MotionEnum.EnumSorting = MotionEnum.Sorting.触发分拣;
                    index = 0;
                }
                strStep = Enum.GetName(typeof(MotionEnum.Sorting), MotionEnum.EnumSorting);

                switch (MotionEnum.EnumSorting)
                {
                    case MotionEnum.Sorting.触发分拣:

                        if (plc.ReadByVariableName("PhotoResultSignal") == "1" && plc.ReadByVariableName("TestMode") == "1")
                        {
                            string strOut;
                            string testType = "IV";
                            int result = -2;
                            tickCount = Environment.TickCount;
                            if (queueXRAY2Station.Count == 0)
                            {
                                WriteLog("分拣工位无料", LogLevels.Warn, "Task_Sorting");
                                Thread.Sleep(1000);
                                continue;
                            }
                            //if (queueXRAY2Station.Peek().DataFinish == false)
                            //{
                            //    WriteLog(string.Format("第{0}次 等待数据处理线程完成", index), LogLevels.Info, "Task_Sorting");
                            //    continue;
                            //}
                            bt = queueXRAY2Station.Dequeue();
                            index = bt.TempBarcode;
                            WriteLog(string.Format("第{0}次 {1} ", index, strStep), LogLevels.Info, "Task_Sorting");

                            if (bt.MasterType != "")
                            {
                                if (bt.MasterType.ToUpper().Contains("IV"))
                                {
                                    if (bt.IVResult == true)
                                    {
                                        ivCheckCount = 0;
                                        plc.WriteByVariableName("ResultOK", 1);
                                        WriteLog(string.Format("第{0}次 发送PLC分拣 OK", index), LogLevels.Info, "Task_Sorting");
                                    }
                                    else
                                    {
                                        ivCheckCount++;
                                        if (bt.KnifeNoConduction == true || bt.NeedleNoConduction == true)
                                        {
                                            plc.WriteByVariableName("IVNoConduction", 1);
                                            WriteLog(string.Format("第{0}次 IV 不导通 发送PLC分拣 未导通NG", index), LogLevels.Info, "Task_Sorting");
                                        }
                                        else
                                        {
                                            plc.WriteByVariableName("IVNG", 1);
                                            WriteLog(string.Format("第{0}次 发送PLC分拣 IVNG", index), LogLevels.Info, "Task_Sorting");
                                        }
                                        if (ivCheckCount == 4)
                                        {
                                            WriteLog(string.Format("第{0}次 IV点检通过", index), LogLevels.Info, "Task_Sorting");
                                            //MessageBox.Show("IV点检通过");
                                            CheckParamsConfig.Instance.IvCheckTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                            CheckParamsSettingsVm.Instance.MyCheckParamsConfig.Write();
                                            ivCheckCount = 0;
                                        }
                                    }
                                }
                                else if (bt.MasterType.ToUpper().Contains("XRAY"))
                                {
                                    bt.UpdateResult();
                                    WriteLog(string.Format("第{0}次 Final result is {1} {2}-{3}: {4} {5} {6} {7} ",
                                        index,
                                        bt.Sn,
                                        bt.FinalResult,
                                        bt.ResultCode,
                                        bt.Corner1.InspectResults.resultDataMin.iResult,
                                        bt.Corner2.InspectResults.resultDataMin.iResult,
                                        bt.Corner3.InspectResults.resultDataMin.iResult,
                                        bt.Corner4.InspectResults.resultDataMin.iResult), LogLevels.Info, "Task_Sorting");
                                    AlgoWrapper.Instance.GetResultImage(ref bt);
                                    ResultRelay.Instance.CheckResultHandlers(bt); //刷新监控状态页面
                                    _extRunEmpty.OnCheckFinished(bt, _imageSaveConfig, ref _checkStatus);
                                    //XRayClient.Core\CheckLogic\Extensions\CheckLogicExtRunEmpty.cs
                                    _extSTF.OnSaveImages(bt, _imageSaveConfig);
                                    //XRayClient.Core\CheckLogic\Extensions\CheckLogicExtSTF.cs

                                    if (bt.FinalResult == true)
                                    {
                                        xrayCheckCount = 0;
                                        plc.WriteByVariableName("ResultOK", 1);
                                        WriteLog(string.Format("第{0}次 发送PLC分拣 OK", index), LogLevels.Info,
                                            "Task_Sorting");
                                    }
                                    else if (bt.ResultCode == EResultCodes.AlgoFail)
                                    {
                                        bt.NgItem += "XRAYNG/";
                                        xrayCheckCount++;
                                        plc.WriteByVariableName("XRAYNG", 1);
                                        WriteLog(string.Format("第{0}次 发送PLC分拣 XRAYNG", index), LogLevels.Info,
                                            "Task_Sorting");
                                        if (xrayCheckCount == CheckParamsConfig.Instance.MyStartupTestConfig.TestNGNum)
                                        {
                                            CheckParamsConfig.Instance.MyStartupTestConfig.lastTestTime = DateTime.Now;
                                            CheckParamsConfig.Instance.SaveStartupConfig();
                                            xrayCheckCount = 0;
                                            WriteLog(string.Format("第{0}次 XRAY点检通过", index), LogLevels.Info,
                                                "Task_Sorting");
                                            //MessageBox.Show("XRAY点检通过");
                                            CheckParamsConfig.Instance.XrayCheckTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                            CheckParamsSettingsVm.Instance.MyCheckParamsConfig.Write();
                                        }
                                    }
                                    else
                                    {
                                        bt.NgItem += "XRAY位置错误黑白图/";
                                        xrayCheckCount = 0;
                                        plc.WriteByVariableName("OtherNG", 1);
                                        WriteLog(string.Format("第{0}次 发送PLC分拣 其他NG", index), LogLevels.Info, "Task_Sorting");
                                    }
                                }
                                else
                                {
                                    plc.WriteByVariableName("OtherNG", 1);
                                    WriteLog(string.Format("第{0}次 未知Master类型 发送PLC分拣 其他NG", index), LogLevels.Info, "Task_Sorting");
                                }
                                MotionEnum.EnumSorting = MotionEnum.Sorting.分拣完成;
                                continue;
                            }

                            #region 原处理数据(空)

                            #endregion

                            if (plc.ReadByVariableName("NOOCV") == "0" && bt.OCVResult == false)
                            {
                                if (bt.VoltageResult == false)
                                {
                                    plc.WriteByVariableName("VoltageNG", 1);
                                    WriteLog(string.Format("第{0}次 发送PLC分拣 电压NG", index), LogLevels.Info, "Task_Sorting");
                                }
                                if (bt.ResistanceResult == false)
                                {
                                    plc.WriteByVariableName("ResistanceNG", 1);
                                    WriteLog(string.Format("第{0}次 发送PLC分拣 内阻NG", index), LogLevels.Info, "Task_Sorting");
                                }
                                else if (isKValueNG == true)
                                {
                                    plc.WriteByVariableName("KNG", 1);
                                    WriteLog(string.Format("第{0}次 发送PLC分拣 K值NG", index), LogLevels.Info, "Task_Sorting");
                                }
                                else
                                {
                                    //温度NG当其他NG处理
                                    plc.WriteByVariableName("OtherNG", 1);
                                    BotIF.MyCheckStatus.MyStatistics.OtherNG++;
                                    WriteLog(string.Format("第{0}次 发送PLC分拣 其他NG", index), LogLevels.Info, "Task_Sorting");
                                }
                            }
                            else if (bt.FinalResult == false && bt.Sn != "ERROR" && bt.Sn != string.Empty && bt.ResultCode != EResultCodes.AlgoErr && bt.ResultCode != EResultCodes.Unknow && plc.ReadByVariableName("NOXRAY") == "0")
                            {
                                plc.WriteByVariableName("XRAYNG", 1);
                                BotIF.MyCheckStatus.MyStatistics.XRayNG++;
                                WriteLog(string.Format("第{0}次 发送PLC分拣 XRAYNG", index), LogLevels.Info, "Task_Sorting");
                            }
                            else if (plc.ReadByVariableName("NOMDI") == "0" && bt.DimensionResult == false && CheckParamsConfig.Instance.IsNoUpLoadMdiAndPPGData == false)
                            {
                                plc.WriteByVariableName("MDING", 1);
                                WriteLog(string.Format("第{0}次 发送PLC分拣 FQING", index), LogLevels.Info, "Task_Sorting");
                            }
                            else if (bt.IsMarkingCell == true)
                            {
                                plc.WriteByVariableName("OtherNG", 1);
                                BotIF.MyCheckStatus.MyStatistics.OtherNG++;
                                WriteLog(string.Format("第{0}次 {1} 是marking拦截电池，发送PLC分拣 其他NG", index, bt.Sn), LogLevels.Info, "Task_Sorting");
                            }
                            else if ((bt.KnifeNoConduction == true || bt.NeedleNoConduction == true) && plc.ReadByVariableName("NOIV") == "0")//不导通
                            {
                                plc.WriteByVariableName("IVNoConduction", 1);
                                //plc.WriteByVariableName("OtherNG", 1);
                                BotIF.MyCheckStatus.MyStatistics.OtherNG++;
                                WriteLog(string.Format("第{0}次 IV 不导通 发送PLC分拣 未导通NG", index), LogLevels.Info, "Task_Sorting");
                            }
                            else if ((bt.ResultCode == EResultCodes.Unknow || bt.ResultCode == EResultCodes.AlgoErr) && plc.ReadByVariableName("NOXRAY") == "0")
                            {
                                plc.WriteByVariableName("OtherNG", 1);
                                BotIF.MyCheckStatus.MyStatistics.OtherNG++;
                                WriteLog(string.Format("第{0}次 发送PLC分拣 其他NG", index), LogLevels.Info, "Task_Sorting");
                            }
                            else if (plc.ReadByVariableName("NOIV") == "0" && bt.IVResult == false)
                            {
                                plc.WriteByVariableName("IVNG", 1);
                                WriteLog(string.Format("第{0}次 发送PLC分拣 IVNG", index), LogLevels.Info, "Task_Sorting");
                            }
                            else if (plc.ReadByVariableName("NOPPG") == "0" && bt.ThicknessResult == false && CheckParamsConfig.Instance.IsNoUpLoadMdiAndPPGData == false)
                            {
                                plc.WriteByVariableName("ThicknessNG", 1);
                                WriteLog(string.Format("第{0}次 发送PLC分拣 PPGNG", index), LogLevels.Info, "Task_Sorting");
                            }
                            else if (bt.MesResult == false)
                            {
                                plc.WriteByVariableName("OtherNG", 1);
                                BotIF.MyCheckStatus.MyStatistics.MesNG++;
                                WriteLog(string.Format("第{0}次 MESNG,发送PLC分拣 其他NG", index), LogLevels.Info, "Task_Sorting");
                            }
                            else
                            {
                                BotIF.MyCheckStatus.MyStatistics.OKNum++;
                                plc.WriteByVariableName("ResultOK", 1);
                                WriteLog(string.Format("第{0}次 发送PLC分拣 OK", index), LogLevels.Info, "Task_Sorting");
                            }

                            if (bt.IsMarkingCell == false)
                            {
                                if (plc.ReadByVariableName("NOOCV") == "0" && bt.OCVResult == false)
                                {
                                    BotIF.MyCheckStatus.MyStatistics.OCVNG++;
                                }
                                if (CheckParamsConfig.Instance.IsNoUpLoadMdiAndPPGData == false)
                                {
                                    if (plc.ReadByVariableName("NOMDI") == "0" && bt.DimensionResult == false)
                                    {
                                        BotIF.MyCheckStatus.MyStatistics.DimensionNG++;
                                    }
                                    if (plc.ReadByVariableName("NOPPG") == "0" && bt.ThicknessResult == false)
                                    {
                                        BotIF.MyCheckStatus.MyStatistics.ThincknessNG++;
                                    }
                                }
                                if (plc.ReadByVariableName("NOIV") == "0" && bt.IVResult == false && bt.KnifeNoConduction == false && bt.NeedleNoConduction == false)
                                {
                                    BotIF.MyCheckStatus.MyStatistics.IVNG++;
                                }
                            }

                            BotIF.MyCheckStatus.MyStatistics.TotalNum++;
                            BotIF.MyCheckStatus.MyStatistics.Save();
                            MotionEnum.EnumSorting = MotionEnum.Sorting.分拣完成;

                            //plc.WriteByVariableName("ResultOK", 1);//强制OK排除

                        }
                        break;

                    case MotionEnum.Sorting.分拣完成:

                        if (plc.ReadByVariableName("PhotoResultSignal") == "0")
                        {
                            WriteLog(string.Format("第{0}次 {1} ,共耗时：{2} ms", index, strStep, Environment.TickCount - tickCount), LogLevels.Info, "Task_Sorting");
                            plc.WriteByVariableName("ResultOK", 0);
                            plc.WriteByVariableName("IVNG", 0);
                            plc.WriteByVariableName("IVNoConduction", 0);
                            plc.WriteByVariableName("OtherNG", 0);
                            plc.WriteByVariableName("VoltageNG", 0);
                            plc.WriteByVariableName("ResistanceNG", 0);
                            plc.WriteByVariableName("KNG", 0);
                            plc.WriteByVariableName("MDING", 0);
                            plc.WriteByVariableName("ThicknessNG", 0);
                            plc.WriteByVariableName("XRAYNG", 0);
                            DataViewPage.Instance.UpdateListView(bt);
                            bt.Destroy();
                            MotionEnum.EnumSorting = MotionEnum.Sorting.触发分拣;
                        }

                        break;
                }
            }
        }

        private void Task_IV()
        {
            string strStep = "";
            BatterySeat bt1 = null;
            BatterySeat bt2 = null;
            BatterySeat bt3 = null;
            BatterySeat bt4 = null;
            int index = 0;
            int tickCount = 0;
            string strData1 = "";
            string strData2 = "";
            string strData3 = "";
            string strData4 = "";
            WriteLog("IV测试线程开启", LogLevels.Info, "Task_IV");
            plc.WriteByVariableName("IVComplete", 0);
            MotionEnum.EnumIV = MotionEnum.IV.触发IV测试;

            while (true)
            {
                Thread.Sleep(10);
                if (_AutoRunning == false)
                {
                    WriteLog("IV测试线程退出", LogLevels.Info, "Task_IV");
                    return;
                }
                if (plc.ReadByVariableName("SoftwareReset") == "1")
                {
                    MotionEnum.EnumIV = MotionEnum.IV.触发IV测试;
                    index = 0;
                }

                strStep = Enum.GetName(typeof(MotionEnum.IV), MotionEnum.EnumIV);

                switch (MotionEnum.EnumIV)
                {
                    case MotionEnum.IV.触发IV测试:

                        if ((plc.ReadByVariableName("IVTestSignal1") == "1" || plc.ReadByVariableName("IVTestSignal2") == "1" || plc.ReadByVariableName("IVTestSignal3") == "1" || plc.ReadByVariableName("IVTestSignal4") == "1") && plc.ReadByVariableName("TestMode") == "1")
                        {
                            bt1 = null;
                            bt2 = null;
                            bt3 = null;
                            bt4 = null;
                            tickCount = Environment.TickCount;
                            WriteLog(string.Format("第{0}次 {1} ", index, strStep), LogLevels.Info, "Task_IV");
                            MotionEnum.EnumIV = MotionEnum.IV.判断工位一是否有料;
                        }

                        break;

                    case MotionEnum.IV.判断工位一是否有料:

                        if (plc.ReadByVariableName("IVTestSignal1") == "1")
                        {
                            if (queueScanStation1.Count == 0)
                            {
                                WriteLog(string.Format("第{0}次 {1} 结果是无料但PLC给了触发信号", index, strStep), LogLevels.Warn, "Task_IV");
                                Thread.Sleep(3000);
                            }
                            else
                            {
                                if (queueScanStation1.Peek().MasterType == "")
                                {
                                    if (canDoIV == false)
                                    {
                                        System.Windows.MessageBox.Show("IV未点检完成，不能启动IV功能", "提示",
                                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information, System.Windows.MessageBoxResult.OK, System.Windows.MessageBoxOptions.ServiceNotification);
                                        Thread.Sleep(3000);
                                        continue;
                                    }
                                }

                                bt1 = queueScanStation1.Dequeue();
                                index = bt1.TempBarcode;
                                //bt1.Cannel = 1;
                                bt1.IVChannel = 1;
                                MotionEnum.EnumIV = MotionEnum.IV.判断工位二是否有料;
                            }
                        }
                        else
                        {
                            MotionEnum.EnumIV = MotionEnum.IV.判断工位二是否有料;
                        }

                        break;

                    case MotionEnum.IV.判断工位二是否有料:

                        if (plc.ReadByVariableName("IVTestSignal2") == "1")
                        {
                            if (queueScanStation2.Count == 0)
                            {
                                WriteLog(string.Format("第{0}次 {1} 结果是无料但PLC给了触发信号", index, strStep), LogLevels.Warn, "Task_IV");
                                Thread.Sleep(3000);
                            }
                            else
                            {
                                if (queueScanStation2.Peek().MasterType == "")
                                {
                                    if (canDoIV == false)
                                    {
                                        System.Windows.MessageBox.Show("IV未点检完成，不能启动IV功能", "提示",
                                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information, System.Windows.MessageBoxResult.OK, System.Windows.MessageBoxOptions.ServiceNotification);
                                        Thread.Sleep(3000);
                                        continue;
                                    }
                                }

                                bt2 = queueScanStation2.Dequeue();
                                index = bt2.TempBarcode;
                                //bt2.Cannel = 2;
                                bt2.IVChannel = 2;
                                MotionEnum.EnumIV = MotionEnum.IV.判断工位三是否有料;
                            }
                        }
                        else
                        {
                            MotionEnum.EnumIV = MotionEnum.IV.判断工位三是否有料;
                        }

                        break;

                    case MotionEnum.IV.判断工位三是否有料:

                        if (plc.ReadByVariableName("IVTestSignal3") == "1")
                        {
                            if (queueScanStation1.Count == 0)
                            {
                                WriteLog(string.Format("第{0}次 {1} 结果是无料但PLC给了触发信号", index, strStep), LogLevels.Warn, "Task_IV");
                                Thread.Sleep(3000);
                            }
                            else
                            {
                                if (queueScanStation1.Peek().MasterType == "")
                                {
                                    if (canDoIV == false)
                                    {
                                        System.Windows.MessageBox.Show("IV未点检完成，不能启动IV功能", "提示",
                                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information, System.Windows.MessageBoxResult.OK, System.Windows.MessageBoxOptions.ServiceNotification);
                                        Thread.Sleep(3000);
                                        continue;
                                    }
                                }

                                bt3 = queueScanStation1.Dequeue();
                                index = bt3.TempBarcode;
                                //bt3.Cannel = 3;
                                bt3.IVChannel = 3;
                                MotionEnum.EnumIV = MotionEnum.IV.判断工位四是否有料;
                            }
                        }
                        else
                        {
                            MotionEnum.EnumIV = MotionEnum.IV.判断工位四是否有料;
                        }

                        break;

                    case MotionEnum.IV.判断工位四是否有料:

                        if (plc.ReadByVariableName("IVTestSignal4") == "1")
                        {
                            if (queueScanStation2.Count == 0)
                            {
                                WriteLog(string.Format("第{0}次 {1} 结果是无料但PLC给了触发信号", index, strStep), LogLevels.Warn, "Task_IV");
                                Thread.Sleep(3000);
                            }
                            else
                            {
                                if (queueScanStation2.Peek().MasterType == "")
                                {
                                    if (canDoIV == false)
                                    {
                                        System.Windows.MessageBox.Show("IV未点检完成，不能启动IV功能", "提示",
                                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information, System.Windows.MessageBoxResult.OK, System.Windows.MessageBoxOptions.ServiceNotification);
                                        Thread.Sleep(3000);
                                        continue;
                                    }
                                }

                                bt4 = queueScanStation2.Dequeue();
                                index = bt4.TempBarcode;
                                //bt4.Cannel = 4;
                                bt4.IVChannel = 4;
                                MotionEnum.EnumIV = MotionEnum.IV.获取IV数据;
                            }
                        }
                        else
                        {
                            MotionEnum.EnumIV = MotionEnum.IV.获取IV数据;
                        }

                        break;

                    case MotionEnum.IV.获取IV数据:

                        //获取IV数据
                        Lr8450.StartTest();
                        Thread.Sleep(CheckParamsConfig.Instance.IvTestTime);
                        //Thread.Sleep(1200);
                        strData1 = Lr8450.GetData(CheckParamsConfig.Instance.IvStation1Channel);
                        strData2 = Lr8450.GetData(CheckParamsConfig.Instance.IvStation2Channel);
                        strData3 = Lr8450.GetData(CheckParamsConfig.Instance.IvStation3Channel);
                        strData4 = Lr8450.GetData(CheckParamsConfig.Instance.IvStation4Channel);
                        WriteLog(string.Format("第{0}次 {1} ,数据：{2} | {3} | {4} | {5}", index, strStep, strData1, strData2, strData3, strData4), LogLevels.Info, "Task_IV");
                        Thread.Sleep(10);
                        strData1 = Lr8450.GetData(CheckParamsConfig.Instance.IvStation1Channel);
                        strData2 = Lr8450.GetData(CheckParamsConfig.Instance.IvStation2Channel);
                        strData3 = Lr8450.GetData(CheckParamsConfig.Instance.IvStation3Channel);
                        strData4 = Lr8450.GetData(CheckParamsConfig.Instance.IvStation4Channel);
                        WriteLog(string.Format("第{0}次 {1} ,数据：{2} | {3} | {4} | {5}", index, strStep, strData1, strData2, strData3, strData4), LogLevels.Info, "Task_IV");

                        plc.WriteByVariableName("IVComplete", 1);
                        MotionEnum.EnumIV = MotionEnum.IV.获取数据完成;

                        break;

                    case MotionEnum.IV.获取数据完成:

                        if (plc.ReadByVariableName("IVTestSignal1") == "0" &&
                            plc.ReadByVariableName("IVTestSignal2") == "0" &&
                            plc.ReadByVariableName("IVTestSignal3") == "0" &&
                            plc.ReadByVariableName("IVTestSignal4") == "0")
                        {
                            plc.WriteByVariableName("IVComplete", 0);
                            MotionEnum.EnumIV = MotionEnum.IV.处理IV数据;
                        }
                        break;

                    case MotionEnum.IV.处理IV数据:

                        if (plc.ReadByVariableName("NeedleOKSignal1") == "1" || plc.ReadByVariableName("KnifeOKSignal1") == "1" || plc.ReadByVariableName("NeedleNGSignal1") == "1" || plc.ReadByVariableName("KnifeNGSignal1") == "1"
                            || plc.ReadByVariableName("NeedleOKSignal2") == "1" || plc.ReadByVariableName("KnifeOKSignal2") == "1" || plc.ReadByVariableName("NeedleNGSignal2") == "1" || plc.ReadByVariableName("KnifeNGSignal2") == "1"
                            || plc.ReadByVariableName("NeedleOKSignal3") == "1" || plc.ReadByVariableName("KnifeOKSignal3") == "1" || plc.ReadByVariableName("NeedleNGSignal3") == "1" || plc.ReadByVariableName("KnifeNGSignal3") == "1"
                            || plc.ReadByVariableName("NeedleOKSignal4") == "1" || plc.ReadByVariableName("KnifeOKSignal4") == "1" || plc.ReadByVariableName("NeedleNGSignal4") == "1" || plc.ReadByVariableName("KnifeNGSignal4") == "1")
                        {
                            string[] strArr = strData1.Split(',');
                            float[] datas1 = new float[6];
                            for (int i = 0; i < datas1.Length; i++)
                            {
                                try
                                {
                                    datas1[i] = Convert.ToSingle(strArr[i]);
                                }
                                catch (Exception ex)
                                {
                                    datas1[i] = 999;
                                }
                            }
                            strArr = strData2.Split(',');
                            float[] datas2 = new float[6];
                            for (int i = 0; i < datas2.Length; i++)
                            {
                                try
                                {
                                    datas2[i] = Convert.ToSingle(strArr[i]);
                                }
                                catch (Exception ex)
                                {
                                    datas2[i] = 999;
                                }
                            }
                            strArr = strData3.Split(',');
                            float[] datas3 = new float[6];
                            for (int i = 0; i < datas3.Length; i++)
                            {
                                try
                                {
                                    datas3[i] = Convert.ToSingle(strArr[i]);
                                }
                                catch (Exception ex)
                                {
                                    datas3[i] = 999;
                                }
                            }
                            strArr = strData4.Split(',');
                            float[] datas4 = new float[6];
                            for (int i = 0; i < datas4.Length; i++)
                            {
                                try
                                {
                                    datas4[i] = Convert.ToSingle(strArr[i]);
                                }
                                catch (Exception ex)
                                {
                                    datas4[i] = 999;
                                }
                            }

                            int result = 0;
                            if (bt1 != null)
                            {
                                Thread.Sleep(100);//PLC地址有时候更新慢，延时减少报警
                                while (_AutoRunning == true)
                                {
                                    if (plc.ReadByVariableName("NeedleOKSignal1") == "0" && plc.ReadByVariableName("KnifeOKSignal1") == "0" && plc.ReadByVariableName("NeedleNGSignal1") == "0" && plc.ReadByVariableName("KnifeNGSignal1") == "0")
                                    {
                                        System.Windows.MessageBox.Show("工位1有IV测试信号PLC却没有给导通信号，逻辑错误!", "提示", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.None,
                            System.Windows.MessageBoxResult.OK, System.Windows.MessageBoxOptions.ServiceNotification);
                                        Thread.Sleep(2000);
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                                WriteLog(string.Format("第{0}次IV工位1数据 {1} ", index, strData1), LogLevels.Info, "Task_IV");
                                if (plc.ReadByVariableName("IVPlatform1") == "1")
                                {
                                    if (bt1.TempBarcode != Convert.ToInt32(plc.ReadByVariableName("IVPlatform1TempBarcode1")))
                                    {
                                        WriteLog(string.Format("IV平台1工位1条码代号 PC {0} != PLC {1}！", bt1.TempBarcode, plc.ReadByVariableName("IVPlatform1TempBarcode1")), LogLevels.Warn, "Task_IV");
                                        Thread.Sleep(2000);
                                        continue;
                                    }
                                }
                                else if (plc.ReadByVariableName("IVPlatform2") == "1")
                                {
                                    if (bt1.TempBarcode != Convert.ToInt32(plc.ReadByVariableName("IVPlatform2TempBarcode1")))
                                    {
                                        WriteLog(string.Format("IV平台2工位1条码代号 PC {0} != PLC {1}！", bt1.TempBarcode, plc.ReadByVariableName("IVPlatform1TempBarcode1")), LogLevels.Warn, "Task_IV");
                                        Thread.Sleep(2000);
                                        continue;
                                    }
                                }
                                if (plc.ReadByVariableName("NeedleOKSignal1") == "1" && plc.ReadByVariableName("KnifeOKSignal1") == "1")
                                {
                                    WriteLog(string.Format("第{0}次 工位1 导通", index), LogLevels.Info, "Task_IV");
                                    bt1.IVPLCConduction = "导通";
                                    result = HandleIV(true, datas1, ref bt1);
                                }
                                else if (plc.ReadByVariableName("NeedleNGSignal1") == "1" || plc.ReadByVariableName("KnifeNGSignal1") == "1")
                                {
                                    if (plc.ReadByVariableName("NeedleNGSignal1") == "1")
                                    {
                                        WriteLog(string.Format("第{0}次 工位1 探针不导通", index), LogLevels.Info, "Task_IV");
                                        bt1.IVPLCConduction += "探针不通/";
                                    }
                                    if (plc.ReadByVariableName("KnifeNGSignal1") == "1")
                                    {
                                        WriteLog(string.Format("第{0}次 工位1 刀不导通", index), LogLevels.Info, "Task_IV");
                                        bt1.IVPLCConduction += "刀不通";
                                    }
                                    if (plc.ReadByVariableName("NeedleNGSignal1") == "1")
                                    {
                                        bt1.NeedleNoConduction = true;
                                        result = HandleIV(false, datas1, ref bt1);
                                    }
                                    else
                                    {
                                        bt1.KnifeNoConduction = true;
                                        result = HandleIV(false, datas1, ref bt1);
                                    }
                                }
                                bt1.IvData = Math.Max(datas1[4], datas1[5]);
                                if (result == 0 && bt1.IvData >= CheckParamsConfig.Instance.MinIV && bt1.IvData <= CheckParamsConfig.Instance.MaxIV)
                                {
                                    bt1.IVResult = true;
                                }
                                else
                                {
                                    bt1.IVResult = false;
                                }
                                if (CheckIVRoundIsClose(strData1, 1) == false)
                                {
                                    string errmsg = "IV通道1连续3次测得数据为0，请检查!";
                                    System.Windows.MessageBox.Show(errmsg, "提示",
                                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information, System.Windows.MessageBoxResult.OK, System.Windows.MessageBoxOptions.ServiceNotification);
                                    WriteLog(errmsg, LogLevels.Warn, "Task_IV");
                                }
                            }
                            if (bt2 != null)
                            {
                                while (_AutoRunning == true)
                                {
                                    if (plc.ReadByVariableName("NeedleOKSignal2") == "0" && plc.ReadByVariableName("KnifeOKSignal2") == "0" && plc.ReadByVariableName("NeedleNGSignal2") == "0" && plc.ReadByVariableName("KnifeNGSignal2") == "0")
                                    {
                                        string errmsg = "工位2有IV测试信号PLC却没有给导通信号，逻辑错误!";
                                        System.Windows.MessageBox.Show(errmsg, "提示", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.None,
                            System.Windows.MessageBoxResult.OK, System.Windows.MessageBoxOptions.ServiceNotification);
                                        WriteLog(errmsg, LogLevels.Warn, "Task_IV");
                                        Thread.Sleep(2000);
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }

                                WriteLog(string.Format("第{0}次IV工位2数据 {1} ", index, strData2), LogLevels.Info, "Task_IV");
                                if (plc.ReadByVariableName("IVPlatform1") == "1")
                                {
                                    if (bt2.TempBarcode != Convert.ToInt32(plc.ReadByVariableName("IVPlatform1TempBarcode2")))
                                    {
                                        WriteLog(string.Format("IV平台1工位2条码代号 PC {0} != PLC {1}！", bt2.TempBarcode, plc.ReadByVariableName("IVPlatform1TempBarcode2")), LogLevels.Warn, "Task_IV");
                                        Thread.Sleep(2000);
                                        continue;
                                    }
                                }
                                else if (plc.ReadByVariableName("IVPlatform2") == "1")
                                {
                                    if (bt2.TempBarcode != Convert.ToInt32(plc.ReadByVariableName("IVPlatform2TempBarcode2")))
                                    {
                                        WriteLog(string.Format("IV平台2工位2条码代号 PC {0} != PLC {1}！", bt2.TempBarcode, plc.ReadByVariableName("IVPlatform2TempBarcode2")), LogLevels.Warn, "Task_IV");
                                        Thread.Sleep(2000);
                                        continue;
                                    }
                                }
                                if (plc.ReadByVariableName("NeedleOKSignal2") == "1" && plc.ReadByVariableName("KnifeOKSignal2") == "1")
                                {
                                    WriteLog(string.Format("第{0}次 工位2 导通", index), LogLevels.Info, "Task_IV");
                                    bt2.IVPLCConduction = "导通";
                                    result = HandleIV(true, datas2, ref bt2);
                                }
                                else if (plc.ReadByVariableName("NeedleNGSignal2") == "1" || plc.ReadByVariableName("KnifeNGSignal2") == "1")
                                {
                                    if (plc.ReadByVariableName("NeedleNGSignal2") == "1")
                                    {
                                        WriteLog(string.Format("第{0}次 工位2 探针不导通", index), LogLevels.Info, "Task_IV");
                                        bt2.IVPLCConduction += "探针不通/";
                                    }
                                    if (plc.ReadByVariableName("KnifeNGSignal2") == "1")
                                    {
                                        WriteLog(string.Format("第{0}次 工位2 刀不导通", index), LogLevels.Info, "Task_IV");
                                        bt2.IVPLCConduction += "刀不通";
                                    }

                                    if (plc.ReadByVariableName("NeedleNGSignal2") == "1")
                                    {
                                        bt2.NeedleNoConduction = true;
                                        result = HandleIV(false, datas2, ref bt2);
                                    }
                                    else
                                    {
                                        bt2.KnifeNoConduction = true;
                                        result = HandleIV(false, datas2, ref bt2);
                                    }
                                }
                                bt2.IvData = Math.Max(datas2[4], datas2[5]);
                                if (result == 0 && bt2.IvData >= CheckParamsConfig.Instance.MinIV && bt2.IvData <= CheckParamsConfig.Instance.MaxIV)
                                {
                                    bt2.IVResult = true;
                                }
                                else
                                {
                                    bt2.IVResult = false;
                                }
                                if (CheckIVRoundIsClose(strData2, 3) == false)
                                {
                                    string errmsg = "IV通道3连续3次测得数据为0，请检查!";
                                    System.Windows.MessageBox.Show(errmsg, "提示",
                                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information, System.Windows.MessageBoxResult.OK, System.Windows.MessageBoxOptions.ServiceNotification);
                                    WriteLog(errmsg, LogLevels.Warn, "Task_IV");
                                }

                            }
                            if (bt3 != null)
                            {
                                while (_AutoRunning == true)
                                {
                                    if (plc.ReadByVariableName("NeedleOKSignal3") == "0" && plc.ReadByVariableName("KnifeOKSignal3") == "0" && plc.ReadByVariableName("NeedleNGSignal3") == "0" && plc.ReadByVariableName("KnifeNGSignal3") == "0")
                                    {
                                        System.Windows.MessageBox.Show("工位3有IV测试信号PLC却没有给导通信号，逻辑错误!", "提示", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.None,
                            System.Windows.MessageBoxResult.OK, System.Windows.MessageBoxOptions.ServiceNotification);
                                        Thread.Sleep(2000);
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }

                                WriteLog(string.Format("第{0}次IV工位3数据 {1} ", index, strData3), LogLevels.Info, "Task_IV");
                                if (plc.ReadByVariableName("IVPlatform1") == "1")
                                {
                                    if (bt3.TempBarcode != Convert.ToInt32(plc.ReadByVariableName("IVPlatform1TempBarcode3")))
                                    {
                                        WriteLog(string.Format("IV平台1工位3条码代号 PC {0} != PLC {1}！", bt3.TempBarcode, plc.ReadByVariableName("IVPlatform1TempBarcode3")), LogLevels.Warn, "Task_IV");
                                        Thread.Sleep(2000);
                                        continue;
                                    }
                                }
                                else if (plc.ReadByVariableName("IVPlatform2") == "1")
                                {
                                    if (bt3.TempBarcode != Convert.ToInt32(plc.ReadByVariableName("IVPlatform2TempBarcode3")))
                                    {
                                        WriteLog(string.Format("IV平台2工位3条码代号 PC {0} != PLC {1}！", bt3.TempBarcode, plc.ReadByVariableName("IVPlatform2TempBarcode3")), LogLevels.Warn, "Task_IV");
                                        Thread.Sleep(2000);
                                        continue;
                                    }
                                }

                                if (plc.ReadByVariableName("NeedleOKSignal3") == "1" && plc.ReadByVariableName("KnifeOKSignal3") == "1")
                                {
                                    WriteLog(string.Format("第{0}次 工位3 导通", index), LogLevels.Info, "Task_IV");
                                    bt3.IVPLCConduction = "导通";
                                    result = HandleIV(true, datas3, ref bt3);
                                }
                                else if (plc.ReadByVariableName("NeedleNGSignal3") == "1" || plc.ReadByVariableName("KnifeNGSignal3") == "1")
                                {
                                    if (plc.ReadByVariableName("NeedleNGSignal3") == "1")
                                    {
                                        WriteLog(string.Format("第{0}次 工位3 探针不导通", index), LogLevels.Info, "Task_IV");
                                        bt3.IVPLCConduction += "探针不通/";
                                    }
                                    if (plc.ReadByVariableName("KnifeNGSignal3") == "1")
                                    {
                                        WriteLog(string.Format("第{0}次 工位3 刀不导通", index), LogLevels.Info, "Task_IV");
                                        bt3.IVPLCConduction += "刀不通";
                                    }
                                    if (plc.ReadByVariableName("NeedleNGSignal3") == "1")
                                    {
                                        bt3.NeedleNoConduction = true;
                                        result = HandleIV(false, datas3, ref bt3);
                                    }
                                    else
                                    {
                                        bt3.KnifeNoConduction = true;
                                        result = HandleIV(false, datas3, ref bt3);
                                    }
                                }
                                bt3.IvData = Math.Max(datas3[4], datas3[5]);
                                if (result == 0 && bt3.IvData >= CheckParamsConfig.Instance.MinIV && bt3.IvData <= CheckParamsConfig.Instance.MaxIV)
                                {
                                    bt3.IVResult = true;
                                }
                                else
                                {
                                    bt3.IVResult = false;
                                }
                                if (CheckIVRoundIsClose(strData3, 5) == false)
                                {
                                    System.Windows.MessageBox.Show("IV通道5连续3次测得数据为0，请检查!", "提示",
                                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information, System.Windows.MessageBoxResult.OK, System.Windows.MessageBoxOptions.ServiceNotification);
                                }

                            }
                            if (bt4 != null)
                            {
                                while (_AutoRunning == true)
                                {
                                    if (plc.ReadByVariableName("NeedleOKSignal4") == "0" && plc.ReadByVariableName("KnifeOKSignal4") == "0" && plc.ReadByVariableName("NeedleNGSignal4") == "0" && plc.ReadByVariableName("KnifeNGSignal4") == "0")
                                    {
                                        System.Windows.MessageBox.Show("工位4有IV测试信号PLC却没有给导通信号，逻辑错误!");
                                        Thread.Sleep(2000);
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }

                                WriteLog(string.Format("第{0}次IV工位4数据 {1} ", index, strData4), LogLevels.Info, "Task_IV");
                                if (plc.ReadByVariableName("IVPlatform1") == "1")
                                {
                                    if (bt4.TempBarcode != Convert.ToInt32(plc.ReadByVariableName("IVPlatform1TempBarcode4")))
                                    {
                                        WriteLog(string.Format("IV工位4条码代号 PC {0} != PLC {1}！", bt4.TempBarcode, plc.ReadByVariableName("IVPlatform1TempBarcode4")), LogLevels.Warn, "Task_IV");
                                        Thread.Sleep(2000);
                                        continue;
                                    }
                                }
                                else if (plc.ReadByVariableName("IVPlatform2") == "1")
                                {
                                    if (bt4.TempBarcode != Convert.ToInt32(plc.ReadByVariableName("IVPlatform2TempBarcode4")))
                                    {
                                        WriteLog(string.Format("IV工位4条码代号 PC {0} != PLC {1}！", bt4.TempBarcode, plc.ReadByVariableName("IVPlatform2TempBarcode4")), LogLevels.Warn, "Task_IV");
                                        Thread.Sleep(2000);
                                        continue;
                                    }
                                }
                                if (plc.ReadByVariableName("NeedleOKSignal4") == "1" && plc.ReadByVariableName("KnifeOKSignal4") == "1")
                                {
                                    WriteLog(string.Format("第{0}次 工位4 导通", index), LogLevels.Info, "Task_IV");
                                    bt4.IVPLCConduction = "导通";
                                    result = HandleIV(true, datas4, ref bt4);
                                }
                                else if (plc.ReadByVariableName("NeedleNGSignal4") == "1" || plc.ReadByVariableName("KnifeNGSignal4") == "1")
                                {
                                    if (plc.ReadByVariableName("NeedleNGSignal4") == "1")
                                    {
                                        WriteLog(string.Format("第{0}次 工位4 探针不导通", index), LogLevels.Info, "Task_IV");
                                        bt4.IVPLCConduction += "探针不通/";
                                    }
                                    if (plc.ReadByVariableName("KnifeNGSignal4") == "1")
                                    {
                                        WriteLog(string.Format("第{0}次 工位4 刀不导通", index), LogLevels.Info, "Task_IV");
                                        bt4.IVPLCConduction += "刀不通";
                                    }
                                    if (plc.ReadByVariableName("NeedleNGSignal4") == "1")
                                    {
                                        bt4.NeedleNoConduction = true;
                                        result = HandleIV(false, datas4, ref bt4);
                                    }
                                    else
                                    {
                                        bt4.KnifeNoConduction = true;
                                        result = HandleIV(false, datas4, ref bt4);
                                    }
                                }
                                bt4.IvData = Math.Max(datas4[4], datas4[5]);
                                if (result == 0 && bt4.IvData >= CheckParamsConfig.Instance.MinIV && bt4.IvData <= CheckParamsConfig.Instance.MaxIV)
                                {
                                    bt4.IVResult = true;
                                }
                                else
                                {
                                    bt4.IVResult = false;
                                }
                                if (CheckIVRoundIsClose(strData4, 7) == false)
                                {
                                    System.Windows.MessageBox.Show("IV通道7连续3次测得数据为0，请检查!", "提示",
                                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information, System.Windows.MessageBoxResult.OK, System.Windows.MessageBoxOptions.ServiceNotification);
                                }

                            }

                            plc.WriteByVariableName("IVConductionComplete", 1);
                            MotionEnum.EnumIV = MotionEnum.IV.IV测试完成;
                        }

                        break;

                    case MotionEnum.IV.IV测试完成:

                        if (plc.ReadByVariableName("NeedleOKSignal1") == "0" && plc.ReadByVariableName("KnifeOKSignal1") == "0" && plc.ReadByVariableName("NeedleNGSignal1") == "0" && plc.ReadByVariableName("KnifeNGSignal1") == "0"
                            && plc.ReadByVariableName("NeedleOKSignal2") == "0" && plc.ReadByVariableName("KnifeOKSignal2") == "0" && plc.ReadByVariableName("NeedleNGSignal2") == "0" && plc.ReadByVariableName("KnifeNGSignal2") == "0"
                            && plc.ReadByVariableName("NeedleOKSignal3") == "0" && plc.ReadByVariableName("KnifeOKSignal3") == "0" && plc.ReadByVariableName("NeedleNGSignal3") == "0" && plc.ReadByVariableName("KnifeNGSignal3") == "0"
                            && plc.ReadByVariableName("NeedleOKSignal4") == "0" && plc.ReadByVariableName("KnifeOKSignal4") == "0" && plc.ReadByVariableName("NeedleNGSignal4") == "0" && plc.ReadByVariableName("KnifeNGSignal4") == "0")
                        {
                            WriteLog(string.Format("第{0}次 {1} ,共耗时：{2} ms", index, strStep, Environment.TickCount - tickCount), LogLevels.Info, "Task_IV");
                            if (bt1 != null)
                            {
                                SaveIVDataToFile(bt1);
                                EnterQueue(bt1, "IV1");
                            }
                            if (bt2 != null)
                            {
                                SaveIVDataToFile(bt2);
                                EnterQueue(bt2, "IV2");
                            }
                            if (bt3 != null)
                            {
                                SaveIVDataToFile(bt3);
                                EnterQueue(bt3, "IV3");
                            }
                            if (bt4 != null)
                            {
                                SaveIVDataToFile(bt4);
                                EnterQueue(bt4, "IV4");
                            }
                            plc.WriteByVariableName("IVConductionComplete", 0);
                            MotionEnum.EnumIV = MotionEnum.IV.触发IV测试;
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// 处理IV数据
        /// </summary>
        /// <param name="isClose"></param>
        /// <param name="arr"></param>
        /// <param name="bt"></param>
        /// <returns></returns>
        private int HandleIV(bool isClose, float[] arr, ref BatterySeat bt)
        {
            string data = "";
            for (int i = 0; i < arr.Length; i++)
            {
                data += arr[i] + ",";
            }
            data = data.Remove(data.Length - 1, 1);
            bt.IV = data;

            float max = CheckParamsConfig.Instance.MaxIV;//IV上限
            float min = CheckParamsConfig.Instance.MinIV;//IV下限
            float source = CheckParamsConfig.Instance.Source;//初始值
            float range = CheckParamsConfig.Instance.Range;//跳变值
            float ex1 = CheckParamsConfig.Instance.ExceptionData1;//异常数据1
            float ex2 = CheckParamsConfig.Instance.ExceptionData2;//异常数据2
            float max16 = arr.Max();
            float max14 = arr.Take(4).Max();
            float max56 = arr.Skip(4).Max();
            bt.IvData = max56;
            if (isClose == false)
            {
                if (max16 < source || (max14 - max56) > range)
                {
                    bt.NgItem += "IV不导通/";
                    return 2;//不导通
                }
            }
            bt.KnifeNoConduction = false;
            bt.NeedleNoConduction = false;

            if (max16 <= ex1)
            {
                bt.IVRemark = "异常数据1";
            }
            else if (max14 - max56 >= ex2)
            {
                bt.IVRemark = "异常数据2";
            }
            else if (max56 >= max || max56 <= min)
            {
                bt.IVRemark = "超规格";
            }

            if (max56 >= max || max56 <= min || max16 <= ex1 || (max14 - max56) >= ex2)
            {
                bt.NgItem += "IVNG/";
                bt.IVResult = false;
                return 1;//IVNG
            }
            bt.IVResult = true;
            return 0;//IVOK
        }

        private static int ocvNGCounts1 = 0;
        private void Task_OCV1()
        {
            string strStep = "";
            BatterySeat bt1 = null;
            int index = 0;
            int tickCount = 0;
            WriteLog("通道1OCV测试线程开启", LogLevels.Info, "Task_OCV1");
            plc.WriteByVariableName("ResistanceComplete1", 0);
            plc.WriteByVariableName("VoltageComplete1", 0);
            MotionEnum.EnumOCV1 = MotionEnum.OCV1.工位1触发内阻测试;

            while (true)
            {
                Thread.Sleep(10);
                if (_AutoRunning == false)
                {
                    WriteLog("通道1OCV测试线程退出", LogLevels.Info, "Task_OCV1");
                    return;
                }
                if (plc.ReadByVariableName("SoftwareReset") == "1")
                {
                    MotionEnum.EnumOCV1 = MotionEnum.OCV1.工位1触发内阻测试;
                    index = 0;
                }
                strStep = Enum.GetName(typeof(MotionEnum.OCV1), MotionEnum.EnumOCV1);
                switch (MotionEnum.EnumOCV1)
                {
                    case MotionEnum.OCV1.工位1触发内阻测试:

                        if (plc.ReadByVariableName("ResistanceSignal1") == "1" && plc.ReadByVariableName("TestMode") == "1")
                        {
                            isOcv1Finished = false;
                            try
                            {
                                if (queueIVStation1.Count == 0)
                                {
                                    WriteLog("OCV工位1无料", LogLevels.Warn, "OCVTempBarcode1");
                                    Thread.Sleep(3000);
                                    continue;
                                }

                                if (queueIVStation1.Peek().TempBarcode != Convert.ToInt32(plc.ReadByVariableName("OCVTempBarcode1")))
                                {
                                    WriteLog(string.Format("OCV工位1条码代号 PC {0} != PLC {1}！", queueIVStation1.Peek().TempBarcode, plc.ReadByVariableName("OCVTempBarcode1")), LogLevels.Warn, "Task_OCV1");
                                    Thread.Sleep(2000);
                                    continue;
                                }
                                if (queueIVStation1.Peek().MasterType == "")
                                {
                                    if (canDoOCV == false)
                                    {
                                        System.Windows.MessageBox.Show("OCV未点检完成，不能启动OCV功能", "提示", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.None,
                            System.Windows.MessageBoxResult.OK, System.Windows.MessageBoxOptions.ServiceNotification);
                                        Thread.Sleep(3000);
                                        continue;
                                    }
                                }

                                tickCount = Environment.TickCount;
                                bt1 = queueIVStation1.Dequeue();
                                //bt1.Cannel = 1;
                                bt1.OCVChannel = 1;
                                WriteLog(string.Format("第{0}次 {1} ", bt1.TempBarcode, strStep), LogLevels.Info, "Task_OCV1");

                                double res = 0;
                                if (CheckParamsConfig.Instance.IsNoResistance == true)
                                {
                                    res = CheckParamsConfig.Instance.ResistanceFixedValue;
                                }
                                else
                                {
                                    res = Bt3562.GetRisitance();
                                }
                                bt1.Resistance = Convert.ToSingle(res) + CheckParamsConfig.Instance.ResistanceCoefficient;
                                if (bt1.Resistance >= CheckParamsConfig.Instance.MinResistance &&
                                    bt1.Resistance <= CheckParamsConfig.Instance.MaxResistance)
                                {
                                    bt1.ResistanceResult = true;
                                }
                                else
                                {
                                    bt1.NgItem += "内阻NG/";
                                    bt1.ResistanceResult = false;
                                }
                                plc.WriteByVariableName("ResistanceComplete1", 1);

                                MotionEnum.EnumOCV1 = MotionEnum.OCV1.工位1内阻测试完成;
                            }
                            catch (Exception ex)
                            {
                                WriteLog(string.Format("第{0}次 {1} {2}", bt1.TempBarcode, strStep, ex), LogLevels.Error, "Task_OCV1");
                            }
                        }

                        break;

                    case MotionEnum.OCV1.工位1内阻测试完成:

                        if (plc.ReadByVariableName("ResistanceSignal1") == "0")
                        {
                            WriteLog(string.Format("第{0}次 {1} ", bt1.TempBarcode, strStep), LogLevels.Info, "Task_OCV1");
                            plc.WriteByVariableName("ResistanceComplete1", 0);
                            MotionEnum.EnumOCV1 = MotionEnum.OCV1.工位1触发OCV电压测试;
                        }
                        break;

                    case MotionEnum.OCV1.工位1触发OCV电压测试:

                        if (plc.ReadByVariableName("VoltageSignal1") == "1")
                        {
                            WriteLog(string.Format("第{0}次 {1} ", bt1.TempBarcode, strStep), LogLevels.Info, "Task_OCV1");
                            double Vol = Tcp34461A.Get34461AVoltage();
                            bt1.Voltage = Convert.ToSingle(Vol);
                            if (bt1.Voltage >= CheckParamsConfig.Instance.MinVoltage &&
                                bt1.Voltage <= CheckParamsConfig.Instance.MaxVoltage)
                            {
                                bt1.VoltageResult = true;
                            }
                            else
                            {
                                bt1.NgItem += "电压NG/";
                                bt1.VoltageResult = false;
                            }

                            if (CheckParamsConfig.Instance.IsNoTemperature == true)
                            {
                                bt1.Temperature = CheckParamsConfig.Instance.TemperatureFixedValue + CheckParamsConfig.Instance.TemperatureCoefficient;
                            }
                            else
                            {
                                bt1.Temperature = Convert.ToSingle(Mi3_1.GetRaytekMI3Temperatrue()) + CheckParamsConfig.Instance.TemperatureCoefficient;
                            }
                            double environmentTemperatrue = E5cc.GetE5CCTemperatrue();
                            bt1.EnvirementTemperature = Convert.ToSingle(environmentTemperatrue);
                            if (bt1.Temperature >= CheckParamsConfig.Instance.MinTemperature && bt1.Temperature <= CheckParamsConfig.Instance.MaxTemperature
                                && Math.Abs(bt1.EnvirementTemperature - bt1.Temperature) <= CheckParamsConfig.Instance.RangeOfTemperatrue)//电池表面温度和环境温度差不能超出设定值
                            {
                                bt1.TemperatureResult = true;
                            }
                            else
                            {
                                bt1.NgItem += "温度NG/";
                                bt1.TemperatureResult = false;
                            }

                            if (bt1.VoltageResult == true && bt1.ResistanceResult == true && bt1.TemperatureResult == true)
                            {
                                bt1.OCVResult = true;
                                ocvNGCounts1 = 0;
                            }
                            else
                            {
                                bt1.OCVResult = false;
                                ocvNGCounts1++;
                                if (ocvNGCounts1 >= CheckParamsConfig.Instance.OCVWarmingCounts)
                                {
                                    System.Windows.MessageBox.Show("OCV工位1 NG个数连续超过 " + ocvNGCounts1 + " 个！！！", "提示",
                                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information, System.Windows.MessageBoxResult.OK, System.Windows.MessageBoxOptions.ServiceNotification);
                                    ocvNGCounts1 = 0;
                                }
                            }
                            try
                            {
                                SaveOCVDataToFile(bt1);
                            }
                            catch (Exception ex)
                            {
                                WriteLog("OCV1保存数据失败", LogLevels.Warn, "Task_OCV1");
                            }
                            plc.WriteByVariableName("VoltageComplete1", 1);
                            EnterQueue(bt1, "OCV1");
                            WriteLog(string.Format("第{0}次 {1} OCV测量电压为 {2} , 内阻为{3} , 电池表面温度为{4} , 环境温度为{5} | 电压结果{6} 内阻结果{7} 温度结果{8} =>{9}",
                                bt1.TempBarcode, bt1.Sn, bt1.Voltage, bt1.Resistance, bt1.Temperature, environmentTemperatrue, bt1.VoltageResult, bt1.ResistanceResult, bt1.TemperatureResult, bt1.OCVResult), LogLevels.Info, "Task_OCV1");
                            MotionEnum.EnumOCV1 = MotionEnum.OCV1.工位1OCV电压测试完成;

                        }
                        break;

                    case MotionEnum.OCV1.工位1OCV电压测试完成:

                        if (plc.ReadByVariableName("VoltageSignal1") == "0")
                        {
                            isOcv1Finished = true;
                            WriteLog(string.Format("第{0}次 {1} ,共耗时：{2} ms", bt1.TempBarcode, strStep, Environment.TickCount - tickCount), LogLevels.Info, "Task_OCV1");
                            plc.WriteByVariableName("VoltageComplete1", 0);
                            MotionEnum.EnumOCV1 = MotionEnum.OCV1.工位1触发内阻测试;
                        }
                        break;
                }
            }
        }

        private static int ocvNGCounts2 = 0;
        private void Task_OCV2()
        {
            string strStep = "";
            BatterySeat bt2 = null;
            int index = 0;
            int tickCount = 0;
            WriteLog("通道2OCV测试线程开启", LogLevels.Info, "Task_OCV2");
            plc.WriteByVariableName("ResistanceComplete2", 0);
            plc.WriteByVariableName("VoltageComplete2", 0);
            MotionEnum.EnumOCV2 = MotionEnum.OCV2.工位2触发OCV电压测试;

            while (true)
            {
                Thread.Sleep(10);
                if (_AutoRunning == false)
                {
                    WriteLog("通道2OCV测试线程退出", LogLevels.Info, "Task_OCV2");
                    return;
                }
                if (plc.ReadByVariableName("SoftwareReset") == "1")
                {
                    MotionEnum.EnumOCV2 = MotionEnum.OCV2.工位2触发OCV电压测试;
                    index = 0;
                }
                strStep = Enum.GetName(typeof(MotionEnum.OCV2), MotionEnum.EnumOCV2);
                switch (MotionEnum.EnumOCV2)
                {
                    case MotionEnum.OCV2.工位2触发OCV电压测试:

                        if (plc.ReadByVariableName("VoltageSignal2") == "1" && plc.ReadByVariableName("TestMode") == "1")
                        {
                            try
                            {
                                if (queueIVStation2.Count == 0)
                                {
                                    WriteLog("OCV工位2无料", LogLevels.Warn, "OCVTempBarcode2");
                                    Thread.Sleep(3000);
                                    continue;
                                }

                                if (queueIVStation2.Peek().TempBarcode != Convert.ToInt32(plc.ReadByVariableName("OCVTempBarcode2")))
                                {
                                    WriteLog(string.Format("OCV工位2条码代号 PC {0} != PLC {1}！", queueIVStation2.Peek().TempBarcode, plc.ReadByVariableName("OCVTempBarcode1")), LogLevels.Warn, "Task_OCV2");
                                    Thread.Sleep(2000);
                                    continue;
                                }
                                if (queueIVStation2.Peek().MasterType == "")
                                {
                                    if (canDoOCV == false)
                                    {
                                        System.Windows.MessageBox.Show("OCV未点检完成，不能启动OCV功能", "提示", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.None,
                            System.Windows.MessageBoxResult.OK, System.Windows.MessageBoxOptions.ServiceNotification);
                                        Thread.Sleep(3000);
                                        continue;
                                    }
                                }

                                tickCount = Environment.TickCount;
                                bt2 = queueIVStation2.Dequeue();
                                //bt2.Cannel = 2;
                                bt2.OCVChannel = 2;
                                WriteLog(string.Format("第{0}次 {1} ", bt2.TempBarcode, strStep), LogLevels.Info, "Task_OCV2");

                                double Vol = Tcp34461A.Get34461AVoltage();
                                bt2.Voltage = Convert.ToSingle(Vol);
                                if (bt2.Voltage >= CheckParamsConfig.Instance.MinVoltage &&
                                    bt2.Voltage <= CheckParamsConfig.Instance.MaxVoltage)
                                {
                                    bt2.VoltageResult = true;
                                }
                                else
                                {
                                    bt2.NgItem += "电压NG/";
                                    bt2.VoltageResult = false;
                                }
                                plc.WriteByVariableName("VoltageComplete2", 1);

                                if (CheckParamsConfig.Instance.IsNoTemperature == true)
                                {
                                    bt2.Temperature = CheckParamsConfig.Instance.TemperatureFixedValue + CheckParamsConfig.Instance.TemperatureCoefficient2;
                                }
                                else
                                {
                                    bt2.Temperature = Convert.ToSingle(Mi3_2.GetRaytekMI3Temperatrue()) + CheckParamsConfig.Instance.TemperatureCoefficient2;
                                }
                                double environmentTemperatrue = E5cc.GetE5CCTemperatrue();
                                bt2.EnvirementTemperature = Convert.ToSingle(environmentTemperatrue);
                                if (bt2.Temperature >= CheckParamsConfig.Instance.MinTemperature && bt2.Temperature <= CheckParamsConfig.Instance.MaxTemperature
                                    && Math.Abs(bt2.EnvirementTemperature - bt2.Temperature) <= CheckParamsConfig.Instance.RangeOfTemperatrue)//电池表面温度和环境温度差不能超出设定值
                                {
                                    bt2.TemperatureResult = true;
                                }
                                else
                                {
                                    bt2.NgItem += "温度NG/";
                                    bt2.TemperatureResult = false;
                                }

                                MotionEnum.EnumOCV2 = MotionEnum.OCV2.工位2OCV电压测试完成;
                            }
                            catch (Exception ex)
                            {
                                WriteLog(string.Format("第{0}次 {1} {2}", bt2.TempBarcode, strStep, ex), LogLevels.Error, "Task_OCV2");
                            }

                        }

                        break;

                    case MotionEnum.OCV2.工位2OCV电压测试完成:

                        if (plc.ReadByVariableName("VoltageSignal2") == "0")
                        {
                            WriteLog(string.Format("第{0}次 {1} ", bt2.TempBarcode, strStep), LogLevels.Info, "Task_OCV2");
                            plc.WriteByVariableName("VoltageComplete2", 0);
                            MotionEnum.EnumOCV2 = MotionEnum.OCV2.工位2触发内阻测试;
                        }
                        break;

                    case MotionEnum.OCV2.工位2触发内阻测试:

                        if (plc.ReadByVariableName("ResistanceSignal2") == "1")
                        {
                            WriteLog(string.Format("第{0}次 {1} ", bt2.TempBarcode, strStep), LogLevels.Info, "Task_OCV2");
                            double res = 0;
                            if (CheckParamsConfig.Instance.IsNoResistance == true)
                            {
                                res = CheckParamsConfig.Instance.ResistanceFixedValue;
                            }
                            else
                            {
                                res = Bt3562.GetRisitance();
                            }
                            bt2.Resistance = Convert.ToSingle(res) + CheckParamsConfig.Instance.ResistanceCoefficient;
                            if (bt2.Resistance >= CheckParamsConfig.Instance.MinResistance &&
                                bt2.Resistance <= CheckParamsConfig.Instance.MaxResistance)
                            {
                                bt2.ResistanceResult = true;
                            }
                            else
                            {
                                bt2.NgItem += "内阻NG/";
                                bt2.ResistanceResult = false;
                            }
                            if (bt2.VoltageResult == true && bt2.ResistanceResult == true && bt2.TemperatureResult == true)
                            {
                                bt2.OCVResult = true;
                                ocvNGCounts2 = 0;
                            }
                            else
                            {
                                bt2.OCVResult = false;
                                ocvNGCounts2++;
                                if (ocvNGCounts2 >= CheckParamsConfig.Instance.OCVWarmingCounts)
                                {
                                    System.Windows.MessageBox.Show("OCV工位2 NG个数连续超过 " + ocvNGCounts2 + " 个！！！", "提示",
                                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information, System.Windows.MessageBoxResult.OK, System.Windows.MessageBoxOptions.ServiceNotification);
                                    ocvNGCounts2 = 0;
                                }
                            }
                            try
                            {
                                SaveOCVDataToFile(bt2);
                            }
                            catch (Exception ex)
                            {
                                WriteLog("OCV2保存数据失败", LogLevels.Warn, "Task_OCV2");
                            }
                            plc.WriteByVariableName("ResistanceComplete2", 1);
                            WriteLog(string.Format("第{0}次 {1} OCV测量电压为 {2} , 内阻为{3} , 电池表面温度为{4} , 环境温度为{5} | 电压结果{6} 内阻结果{7} 温度结果{8} =>{9}",
                                bt2.TempBarcode, bt2.Sn, bt2.Voltage, bt2.Resistance, bt2.Temperature, bt2.EnvirementTemperature, bt2.VoltageResult, bt2.ResistanceResult, bt2.TemperatureResult, bt2.OCVResult), LogLevels.Info, "Task_OCV2");

                            MotionEnum.EnumOCV2 = MotionEnum.OCV2.工位2内阻测试完成;
                        }

                        break;

                    case MotionEnum.OCV2.工位2内阻测试完成:

                        if (plc.ReadByVariableName("ResistanceSignal2") == "0" && isOcv1Finished == true)
                        {
                            EnterQueue(bt2, "OCV2");
                            WriteLog(string.Format("第{0}次 {1} ,共耗时：{2} ms", bt2.TempBarcode, strStep, Environment.TickCount - tickCount), LogLevels.Info, "Task_OCV2");
                            plc.WriteByVariableName("ResistanceComplete2", 0);
                            MotionEnum.EnumOCV2 = MotionEnum.OCV2.工位2触发OCV电压测试;
                        }

                        break;
                }
            }
        }

        private static int mdiNGCounts = 0;
        private static bool isMDIDataFinished = true;
        private void Task_FQI1()
        {
            string strStep = "";
            BatterySeat bt = null;
            int index = 0;
            int tickCount = 0;
            WriteLog("通道1FQI测试线程开启", LogLevels.Info, "Task_FQI1");
            plc.WriteByVariableName("MDIComplete1", 0);
            MotionEnum.EnumMDI1 = MotionEnum.MDI1.工位1触发FQI检测;

            while (true)
            {
                Thread.Sleep(10);
                if (_AutoRunning == false)
                {
                    WriteLog("通道1FQI测试线程退出", LogLevels.Info, "Task_FQI1");
                    return;
                }
                if (plc.ReadByVariableName("SoftwareReset") == "1")
                {
                    MotionEnum.EnumMDI1 = MotionEnum.MDI1.工位1触发FQI检测;
                    index = 0;
                }
                strStep = Enum.GetName(typeof(MotionEnum.MDI1), MotionEnum.EnumMDI1);

                switch (MotionEnum.EnumMDI1)
                {
                    case MotionEnum.MDI1.工位1触发FQI检测:

                        if (plc.ReadByVariableName("MDISignal1") == "1" && (plc.ReadByVariableName("TestMode") == "1") && isCCDBusy == false)
                        {
                            if (queueOCVStation1.Count == 0)
                            {
                                WriteLog("FQI平台1工位1无料", LogLevels.Warn, "Task_FQI1");
                                Thread.Sleep(3000);
                                continue;
                            }

                            if (queueOCVStation1.Peek().TempBarcode != Convert.ToInt32(plc.ReadByVariableName("MDIPlatform1TempBarcode1")))
                            {
                                WriteLog(string.Format("FQI平台1工位1条码代号 PC {0} != PLC {1}！", queueOCVStation1.Peek().TempBarcode, plc.ReadByVariableName("MDIPlatform1TempBarcode1")), LogLevels.Warn, "Task_FQI1");
                                Thread.Sleep(2000);
                                continue;
                            }
                            tickCount = Environment.TickCount;
                            bt = queueOCVStation1.Dequeue();
                            isCCDBusy = true;
                            //bt.Cannel = 1;
                            bt.MDIChannel = 1;
                            WriteLog(string.Format("第{0}次 {1} ", bt.TempBarcode, strStep), LogLevels.Info, "Task_FQI1");
                            if (!GetMdiData(ref bt, bt.TempBarcode, 1))
                            {
                                WriteLog(string.Format("第{0}次 {1} 获取尺寸数据异常", bt.TempBarcode, strStep), LogLevels.Warn, "Task_FQI1");
                                plc.WriteByVariableName("MDIComplete1", 1);

                                //Thread.Sleep(3000);
                            }
                            if (plc.ReadByVariableName("MDIPlatForm1CellCount") == "2" && plc.ReadByVariableName("NOPPG") == "1")
                            {
                                EnterQueue(bt, "MDI1");
                                //BatterySeat bt2 = new BatterySeat();
                                //MDIPlatform1Cell2.CopyTo(ref bt2);
                                //EnterQueue(bt2, "MDI2");

                                EnterQueue(queueNoPPGOfMDIProduct.Dequeue(), "MDI2");
                            }
                            else
                            {
                                EnterQueue(bt, "MDI1");
                            }

                            MotionEnum.EnumMDI1 = MotionEnum.MDI1.工位1FQI完成;
                        }
                        else if (plc.ReadByVariableName("MDISignal3") == "1" && (plc.ReadByVariableName("TestMode") == "1") && isCCDBusy == false)
                        {
                            if (queueOCVStation1.Count == 0)
                            {
                                WriteLog("FQI平台2工位1无料", LogLevels.Warn, "Task_FQI1");
                                Thread.Sleep(3000);
                                continue;
                            }

                            if (queueOCVStation1.Peek().TempBarcode != Convert.ToInt32(plc.ReadByVariableName("MDIPlatform2TempBarcode1")))
                            {
                                WriteLog(string.Format("FQI平台2工位1条码代号 PC {0} != PLC {1}！", queueOCVStation1.Peek().TempBarcode, plc.ReadByVariableName("MDIPlatform2TempBarcode1")), LogLevels.Warn, "Task_FQI1");
                                Thread.Sleep(2000);
                                continue;
                            }
                            tickCount = Environment.TickCount;
                            bt = queueOCVStation1.Dequeue();
                            isCCDBusy = true;
                            //bt.Cannel = 3;
                            bt.MDIChannel = 3;
                            WriteLog(string.Format("第{0}次 {1} ", bt.TempBarcode, strStep), LogLevels.Info, "Task_FQI1");
                            if (!GetMdiData(ref bt, bt.TempBarcode, 3))
                            {
                                WriteLog(string.Format("第{0}次 {1} 获取尺寸数据异常", bt.TempBarcode, strStep), LogLevels.Warn, "Task_FQI1");
                                plc.WriteByVariableName("MDIComplete1", 1);

                                //Thread.Sleep(3000);
                            }
                            EnterQueue(bt, "MDI1");
                            MotionEnum.EnumMDI1 = MotionEnum.MDI1.工位1FQI完成;
                        }

                        break;

                    case MotionEnum.MDI1.工位1FQI完成:

                        if (plc.ReadByVariableName("MDISignal1") == "0" && plc.ReadByVariableName("MDISignal3") == "0")
                        {
                            WriteLog(string.Format("第{0}次 {1} ", bt.TempBarcode, strStep), LogLevels.Info, "Task_FQI1");
                            plc.WriteByVariableName("MDIComplete1", 0);
                            isCCDBusy = false;
                            MotionEnum.EnumMDI1 = MotionEnum.MDI1.工位1触发FQI检测;
                        }

                        break;
                }
            }
        }

        private void Task_FQI2()
        {
            string strStep = "";
            BatterySeat bt = null;
            int index = 0;
            int tickCount = 0;
            WriteLog("通道2FQI测试线程开启", LogLevels.Info, "Task_FQI2");
            plc.WriteByVariableName("MDIComplete2", 0);
            MotionEnum.EnumMDI2 = MotionEnum.MDI2.工位2触发FQI检测;

            while (true)
            {
                Thread.Sleep(10);
                if (_AutoRunning == false)
                {
                    WriteLog("通道2FQI测试线程退出", LogLevels.Info, "Task_FQI2");
                    return;
                }
                if (plc.ReadByVariableName("SoftwareReset") == "1")
                {
                    MotionEnum.EnumMDI2 = MotionEnum.MDI2.工位2触发FQI检测;
                    index = 0;
                }
                strStep = Enum.GetName(typeof(MotionEnum.MDI2), MotionEnum.EnumMDI2);

                switch (MotionEnum.EnumMDI2)
                {
                    case MotionEnum.MDI2.工位2触发FQI检测:

                        if (plc.ReadByVariableName("MDISignal2") == "1" && (plc.ReadByVariableName("TestMode") == "1") && isCCDBusy == false)
                        {
                            if (plc.ReadByVariableName("MDIPlatForm1CellCount") == "0")
                            {
                                WriteLog("FQI平台1电池数量是 0 ", LogLevels.Warn, "Task_FQI2");
                                Thread.Sleep(3000);
                                continue;
                            }
                            if (queueOCVStation2.Count == 0)
                            {
                                WriteLog("FQI平台1工位2无料", LogLevels.Warn, "Task_FQI2");
                                Thread.Sleep(3000);
                                continue;
                            }

                            if (queueOCVStation2.Peek().TempBarcode != Convert.ToInt32(plc.ReadByVariableName("MDIPlatform1TempBarcode2")))
                            {
                                WriteLog(string.Format("FQI平台1工位2条码代号 PC {0} != PLC {1}！", queueOCVStation2.Peek().TempBarcode, plc.ReadByVariableName("MDIPlatform1TempBarcode2")), LogLevels.Warn, "Task_FQI2");
                                Thread.Sleep(2000);
                                continue;
                            }
                            tickCount = Environment.TickCount;
                            bt = queueOCVStation2.Dequeue();
                            isCCDBusy = true;
                            //bt.Cannel = 2;
                            bt.MDIChannel = 2;
                            WriteLog(string.Format("第{0}次 {1} ", bt.TempBarcode, strStep), LogLevels.Info, "Task_FQI2");
                            if (!GetMdiData(ref bt, bt.TempBarcode, 2))
                            {
                                WriteLog(string.Format("第{0}次 {1} 获取尺寸数据异常", bt.TempBarcode, strStep), LogLevels.Warn, "Task_FQI1");
                                plc.WriteByVariableName("MDIComplete2", 1);

                                //Thread.Sleep(3000);
                            }
                            if (plc.ReadByVariableName("MDIPlatForm1CellCount") == "2" && plc.ReadByVariableName("NOPPG") == "1")
                            {
                                queueNoPPGOfMDIProduct.Enqueue(bt);
                                //bt.CopyTo(ref MDIPlatform1Cell2);
                            }
                            else
                            {
                                EnterQueue(bt, "MDI2");
                            }
                            MotionEnum.EnumMDI2 = MotionEnum.MDI2.工位2FQI完成;
                        }
                        else if (plc.ReadByVariableName("MDISignal4") == "1" && (plc.ReadByVariableName("TestMode") == "1") && isCCDBusy == false)
                        {
                            if (queueOCVStation2.Count == 0)
                            {
                                WriteLog("FQI平台2工位2无料", LogLevels.Warn, "Task_FQI2");
                                Thread.Sleep(3000);
                                continue;
                            }

                            if (queueOCVStation2.Peek().TempBarcode != Convert.ToInt32(plc.ReadByVariableName("MDIPlatform2TempBarcode2")))
                            {
                                WriteLog(string.Format("FQI平台2工位2条码代号 PC {0} != PLC {1}！", queueOCVStation2.Peek().TempBarcode, plc.ReadByVariableName("MDIPlatform2TempBarcode2")), LogLevels.Warn, "Task_FQI2");
                                Thread.Sleep(2000);
                                continue;
                            }
                            tickCount = Environment.TickCount;
                            bt = queueOCVStation2.Dequeue();
                            isCCDBusy = true;
                            //bt.Cannel = 4;
                            bt.MDIChannel = 4;
                            WriteLog(string.Format("第{0}次 {1} ", bt.TempBarcode, strStep), LogLevels.Info, "Task_FQI2");
                            if (!GetMdiData(ref bt, bt.TempBarcode, 4))
                            {
                                WriteLog(string.Format("第{0}次 {1} 获取尺寸数据异常", bt.TempBarcode, strStep), LogLevels.Warn, "Task_FQI2");
                                plc.WriteByVariableName("MDIComplete2", 1);

                                //Thread.Sleep(3000);
                            }
                            EnterQueue(bt, "MDI2");
                            MotionEnum.EnumMDI2 = MotionEnum.MDI2.工位2FQI完成;
                        }

                        break;

                    case MotionEnum.MDI2.工位2FQI完成:

                        if (plc.ReadByVariableName("MDISignal2") == "0" && plc.ReadByVariableName("MDISignal4") == "0")
                        {
                            WriteLog(string.Format("第{0}次 {1} ", bt.TempBarcode, strStep), LogLevels.Info, "Task_FQI2");
                            plc.WriteByVariableName("MDIComplete2", 0);
                            isCCDBusy = false;
                            MotionEnum.EnumMDI2 = MotionEnum.MDI2.工位2触发FQI检测;
                        }

                        break;
                }
            }
        }

        private static int ppgNGCounts1 = 0;
        private static int ppgNGCounts2 = 0;
        private static int ppgNGCounts3 = 0;
        private static int ppgNGCounts4 = 0;

        private void Task_Thickness1()
        {
            string strStep = "";
            BatterySeat bt = null;
            int index = 0;
            int tickCount = 0;
            WriteLog("通道1测厚测试线程开启", LogLevels.Info, "Task_Thickness1");
            plc.WriteByVariableName("ThicknessComplete1", 0);
            plc.WriteByVariableName("ThicknessComplete3", 0);
            MotionEnum.EnumThickness1 = MotionEnum.Thickness1.工位1触发测厚;

            while (true)
            {
                Thread.Sleep(10);
                if (_AutoRunning == false)
                {
                    WriteLog("通道1测厚测试线程退出", LogLevels.Info, "Task_Thickness1");
                    return;
                }
                if (plc.ReadByVariableName("SoftwareReset") == "1")
                {
                    MotionEnum.EnumThickness1 = MotionEnum.Thickness1.工位1触发测厚;
                    index = 0;
                }
                strStep = Enum.GetName(typeof(MotionEnum.Thickness1), MotionEnum.EnumThickness1);

                switch (MotionEnum.EnumThickness1)
                {
                    case MotionEnum.Thickness1.工位1触发测厚:

                        if (plc.ReadByVariableName("ThicknessMeasureSignal1") == "1" && plc.ReadByVariableName("TestMode") == "1")
                        {
                            isPPGFinished = false;
                            if (queueMDIStation1.Count == 0)
                            {
                                WriteLog("平台1测厚工位1无料", LogLevels.Warn, "Task_Thickness1");
                                Thread.Sleep(3000);
                                continue;
                            }

                            if (queueMDIStation1.Peek().TempBarcode != Convert.ToInt32(plc.ReadByVariableName("MDIPlatform1TempBarcode1")))
                            {
                                WriteLog(string.Format("平台1测厚工位1条码代号 PC {0} != PLC {1}！", queueMDIStation1.Peek().TempBarcode, plc.ReadByVariableName("MDIPlatform1TempBarcode1")), LogLevels.Warn, "Task_Thickness1");
                                Thread.Sleep(2000);
                                continue;
                            }
                            if (queueMDIStation1.Peek().MasterType == "")
                            {
                                if (canDoPPG == false)
                                {
                                    System.Windows.MessageBox.Show("测厚未点检完成，不能启动测厚功能", "提示", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.None,
                            System.Windows.MessageBoxResult.OK, System.Windows.MessageBoxOptions.ServiceNotification);
                                    Thread.Sleep(3000);
                                    continue;
                                }
                            }

                            tickCount = Environment.TickCount;
                            bt = queueMDIStation1.Dequeue();
                            //bt.Cannel = 1;
                            bt.PPGChannel = 1;
                            WriteLog(string.Format("第{0}次 {1} ", bt.TempBarcode, strStep), LogLevels.Info, "Task_Thickness1");
                            GetTickness(bt.TempBarcode, ref bt, 1);
                            EnterQueue(bt, "Thickness1");
                            if (bt.ThicknessResult == true)
                            {
                                ppgNGCounts1 = 0;
                            }
                            else
                            {
                                ppgNGCounts1++;
                                if (ppgNGCounts1 >= CheckParamsConfig.Instance.PPGWarmingCounts)
                                {
                                    string errmsg = "测厚工位1 连续NG " + ppgNGCounts1 + " 个！！！";
                                    //System.Windows.MessageBox.Show("测厚工位1 连续NG " + ppgNGCounts1 + " 个！！！", "提示",
                                    //System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information, System.Windows.MessageBoxResult.OK, System.Windows.MessageBoxOptions.ServiceNotification);

                                    stressPage = new StressStatePage(errmsg, CheckParamsConfig.Instance.WarmWaitTime);
                                    stressPage.Closed += new EventHandler(stressPage_Closed);
                                    stressPage.ShowDialog();

                                    WriteLog(errmsg, LogLevels.Warn, "Task_Thickness1");

                                    ppgNGCounts1 = 0;
                                }
                            }
                            //EnterQueue(bt, "Thickness1");
                            plc.WriteByVariableName("ThicknessComplete1", 1);
                            MotionEnum.EnumThickness1 = MotionEnum.Thickness1.工位1测厚完成;
                        }
                        else if (plc.ReadByVariableName("ThicknessMeasureSignal3") == "1" && plc.ReadByVariableName("TestMode") == "1")
                        {
                            isPPGFinished = false;
                            if (queueMDIStation1.Count == 0)
                            {
                                WriteLog("平台2测厚工位1无料", LogLevels.Warn, "Task_Thickness1");
                                Thread.Sleep(3000);
                                continue;
                            }

                            if (queueMDIStation1.Peek().TempBarcode != Convert.ToInt32(plc.ReadByVariableName("MDIPlatform2TempBarcode1")))
                            {
                                WriteLog(string.Format("平台2测厚工位1条码代号 PC {0} != PLC {1}！", queueMDIStation1.Peek().TempBarcode, plc.ReadByVariableName("MDIPlatform2TempBarcode1")), LogLevels.Warn, "Task_Thickness1");
                                Thread.Sleep(2000);
                                continue;
                            }
                            tickCount = Environment.TickCount;
                            bt = queueMDIStation1.Dequeue();
                            //bt.Cannel = 3;
                            bt.PPGChannel = 3;
                            WriteLog(string.Format("第{0}次 {1} ", bt.TempBarcode, strStep), LogLevels.Info, "Task_Thickness1");
                            float thickness1 = 0;
                            GetTickness(bt.TempBarcode, ref bt, 3);
                            EnterQueue(bt, "Thickness1");
                            if (bt.ThicknessResult == true)
                            {
                                ppgNGCounts3 = 0;
                            }
                            else
                            {
                                ppgNGCounts3++;
                                if (ppgNGCounts3 >= CheckParamsConfig.Instance.PPGWarmingCounts)
                                {
                                    //System.Windows.MessageBox.Show("测厚工位3 连续NG " + ppgNGCounts3 + " 个！！！", "提示",
                                    //System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information, System.Windows.MessageBoxResult.OK, System.Windows.MessageBoxOptions.ServiceNotification);

                                    string errmsg = "测厚工位3 连续NG " + ppgNGCounts3 + " 个！！！";
                                    stressPage = new StressStatePage(errmsg, CheckParamsConfig.Instance.WarmWaitTime);
                                    stressPage.Closed += new EventHandler(stressPage_Closed);
                                    stressPage.ShowDialog();

                                    WriteLog(errmsg, LogLevels.Warn, "Task_Thickness1");
                                    ppgNGCounts3 = 0;
                                }
                            }

                            //EnterQueue(bt, "Thickness1");
                            plc.WriteByVariableName("ThicknessComplete3", 1);
                            MotionEnum.EnumThickness1 = MotionEnum.Thickness1.工位1测厚完成;
                        }
                        break;

                    case MotionEnum.Thickness1.工位1测厚完成:

                        if (plc.ReadByVariableName("ThicknessMeasureSignal1") == "0" && plc.ReadByVariableName("ThicknessMeasureSignal3") == "0")
                        {
                            plc.WriteByVariableName("ThicknessComplete1", 0);
                            plc.WriteByVariableName("ThicknessComplete3", 0);
                            WriteLog(string.Format("第{0}次 {1} ", bt.TempBarcode, strStep), LogLevels.Info, "Task_Thickness1");
                            WriteLog(string.Format("{0} ", strStep), LogLevels.Info, "Task_Thickness1");
                            isPPGFinished = true;
                            MotionEnum.EnumThickness1 = MotionEnum.Thickness1.工位1触发测厚;
                        }
                        break;
                }
            }
        }

        private void stressPage_Closed(object sender, EventArgs e)
        {
            stressPage = null;
        }

        private void Task_Thickness2()
        {
            string strStep = "";
            BatterySeat bt = null;
            int index = 0;
            int tickCount = 0;
            WriteLog("通道2测厚测试线程开启", LogLevels.Info, "Task_Thickness2");
            plc.WriteByVariableName("ThicknessComplete2", 0);
            plc.WriteByVariableName("ThicknessComplete4", 0);
            MotionEnum.EnumThickness2 = MotionEnum.Thickness2.工位2触发测厚;

            while (true)
            {
                Thread.Sleep(10);
                if (_AutoRunning == false)
                {
                    WriteLog("通道2测厚测试线程退出", LogLevels.Info, "Task_Thickness2");
                    return;
                }
                if (plc.ReadByVariableName("SoftwareReset") == "1")
                {
                    MotionEnum.EnumThickness2 = MotionEnum.Thickness2.工位2触发测厚;
                    index = 0;
                }
                strStep = Enum.GetName(typeof(MotionEnum.Thickness2), MotionEnum.EnumThickness2);

                switch (MotionEnum.EnumThickness2)
                {
                    case MotionEnum.Thickness2.工位2触发测厚:

                        if (plc.ReadByVariableName("ThicknessMeasureSignal2") == "1" && plc.ReadByVariableName("TestMode") == "1")
                        {
                            if (queueMDIStation2.Count == 0)
                            {
                                WriteLog("平台1测厚工位2无料", LogLevels.Warn, "Task_Thickness2");
                                Thread.Sleep(3000);
                                continue;
                            }

                            if (queueMDIStation2.Peek().TempBarcode != Convert.ToInt32(plc.ReadByVariableName("MDIPlatform1TempBarcode2")))
                            {
                                WriteLog(string.Format("平台1测厚工位2条码代号 PC {0} != PLC {1}！", queueMDIStation2.Peek().TempBarcode, plc.ReadByVariableName("MDIPlatform1TempBarcode2")), LogLevels.Warn, "Task_Thickness2");
                                Thread.Sleep(2000);
                                continue;
                            }
                            if (queueMDIStation2.Peek().MasterType == "")
                            {
                                if (canDoPPG == false)
                                {
                                    System.Windows.MessageBox.Show("测厚未点检完成，不能启动测厚功能", "提示", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.None,
                            System.Windows.MessageBoxResult.OK, System.Windows.MessageBoxOptions.ServiceNotification);
                                    Thread.Sleep(3000);
                                    continue;
                                }
                            }

                            tickCount = Environment.TickCount;
                            bt = queueMDIStation2.Dequeue();
                            //bt.Cannel = 2;
                            bt.PPGChannel = 2;
                            WriteLog(string.Format("第{0}次 {1} ", bt.TempBarcode, strStep), LogLevels.Info, "Task_Thickness2");
                            float thickness1 = 0;
                            GetTickness(bt.TempBarcode, ref bt, 2);
                            if (bt.ThicknessResult == true)
                            {
                                ppgNGCounts2 = 0;
                            }
                            else
                            {
                                ppgNGCounts2++;
                                if (ppgNGCounts2 >= CheckParamsConfig.Instance.PPGWarmingCounts)
                                {
                                    string errmsg = "测厚工位2 连续NG " + ppgNGCounts2 + " 个！！！";
                                    //System.Windows.MessageBox.Show("测厚工位2 连续NG " + ppgNGCounts2 + " 个！！！", "提示",
                                    //System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information, System.Windows.MessageBoxResult.OK, System.Windows.MessageBoxOptions.ServiceNotification);

                                    stressPage = new StressStatePage(errmsg, CheckParamsConfig.Instance.WarmWaitTime);
                                    stressPage.Closed += new EventHandler(stressPage_Closed);
                                    stressPage.ShowDialog();

                                    WriteLog(errmsg, LogLevels.Warn, "Task_Thickness2");
                                    ppgNGCounts2 = 0;
                                }
                            }

                            plc.WriteByVariableName("ThicknessComplete2", 1);
                            MotionEnum.EnumThickness2 = MotionEnum.Thickness2.工位2测厚完成;
                        }
                        else if (plc.ReadByVariableName("ThicknessMeasureSignal4") == "1" && plc.ReadByVariableName("TestMode") == "1")
                        {
                            if (queueMDIStation2.Count == 0)
                            {
                                WriteLog("平台2测厚工位2无料", LogLevels.Warn, "Task_Thickness2");
                                Thread.Sleep(3000);
                                continue;
                            }

                            if (queueMDIStation2.Peek().TempBarcode != Convert.ToInt32(plc.ReadByVariableName("MDIPlatform2TempBarcode2")))
                            {
                                WriteLog(string.Format("平台2测厚工位2条码代号 PC {0} != PLC {1}！", queueMDIStation2.Peek().TempBarcode, plc.ReadByVariableName("MDIPlatform2TempBarcode2")), LogLevels.Warn, "Task_Thickness2");
                                Thread.Sleep(2000);
                                continue;
                            }
                            tickCount = Environment.TickCount;
                            bt = queueMDIStation2.Dequeue();
                            //bt.Cannel = 4;
                            bt.PPGChannel = 4;
                            WriteLog(string.Format("第{0}次 {1} ", bt.TempBarcode, strStep), LogLevels.Info, "Task_Thickness2");
                            float thickness1 = 0;
                            GetTickness(bt.TempBarcode, ref bt, 4);
                            if (bt.ThicknessResult == true)
                            {
                                ppgNGCounts4 = 0;
                            }
                            else
                            {
                                ppgNGCounts4++;
                                if (ppgNGCounts4 >= CheckParamsConfig.Instance.PPGWarmingCounts)
                                {
                                    string errmsg = "测厚工位4 连续NG " + ppgNGCounts4 + " 个！！！";
                                    //System.Windows.MessageBox.Show("测厚工位4 连续NG " + ppgNGCounts4 + " 个！！！", "提示",
                                    //System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information, System.Windows.MessageBoxResult.OK, System.Windows.MessageBoxOptions.ServiceNotification);

                                    stressPage = new StressStatePage(errmsg, CheckParamsConfig.Instance.WarmWaitTime);
                                    stressPage.Closed += new EventHandler(stressPage_Closed);
                                    stressPage.ShowDialog();

                                    WriteLog(errmsg, LogLevels.Warn, "Task_Thickness2");
                                    ppgNGCounts4 = 0;
                                }
                            }

                            plc.WriteByVariableName("ThicknessComplete4", 1);
                            MotionEnum.EnumThickness2 = MotionEnum.Thickness2.工位2测厚完成;
                        }
                        break;

                    case MotionEnum.Thickness2.工位2测厚完成:

                        //后面的工位变为单通道，必须要等待工位1先完成
                        if (isPPGFinished == true && plc.ReadByVariableName("ThicknessMeasureSignal1") == "0" && plc.ReadByVariableName("ThicknessMeasureSignal3") == "0")
                        {
                            Thread.Sleep(200);
                            if (plc.ReadByVariableName("ThicknessMeasureSignal2") == "0" && plc.ReadByVariableName("ThicknessMeasureSignal4") == "0")
                            {
                                WriteLog(string.Format("第{0}次 {1} ", bt.TempBarcode, strStep), LogLevels.Info, "Task_Thickness2");
                                EnterQueue(bt, "Thickness2");
                                plc.WriteByVariableName("ThicknessComplete2", 0);
                                plc.WriteByVariableName("ThicknessComplete4", 0);
                                WriteLog(string.Format("{0} ", strStep), LogLevels.Info, "Task_Thickness2");
                                MotionEnum.EnumThickness2 = MotionEnum.Thickness2.工位2触发测厚;
                            }
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// 移除某个电池
        /// </summary>
        private void Task_RemoveProduct()
        {
            plc.WriteByVariableName("RemoveBatteryComplete", 0);
            WriteLog("移除单个电池线程开启", LogLevels.Info, "Task_RemoveProduct");

            while (true)
            {
                Thread.Sleep(10);
                if (_AutoRunning == false)
                {
                    return;
                }
                if (plc.ReadByVariableName("SoftwareReset") == "1")
                {
                    MotionEnum.EnumRemoveProduct = MotionEnum.RemoveProduct.触发移除电池;
                }

                switch (MotionEnum.EnumRemoveProduct)
                {
                    case MotionEnum.RemoveProduct.触发移除电池:

                        if (plc.ReadByVariableName("RemoveBatterySignal") == "1")
                        {
                            Thread.Sleep(100);
                            int tempBarcode = Convert.ToInt32(plc.ReadByVariableName("RemoveBatteryTempBarcode"));
                            if (RemoveProduct(tempBarcode, ref queueScanStation1) || RemoveProduct(tempBarcode, ref queueScanStation2) ||
                                RemoveProduct(tempBarcode, ref queueIVStation1) || RemoveProduct(tempBarcode, ref queueIVStation2) ||
                                RemoveProduct(tempBarcode, ref queueOCVStation1) || RemoveProduct(tempBarcode, ref queueOCVStation2)
                                || RemoveProduct(tempBarcode, ref queueMDIStation1) || RemoveProduct(tempBarcode, ref queueMDIStation2) ||
                                RemoveProduct(tempBarcode, ref queueThickness) || RemoveProduct(tempBarcode, ref queueXRAY1Station) ||
                                RemoveProduct(tempBarcode, ref queueXRAY2Station) || RemoveScanBarcode(tempBarcode, ref dicBarcode1) ||
                                RemoveScanBarcode(tempBarcode, ref dicBarcode2) || RemoveProduct(tempBarcode, ref queueNoPPGOfMDIProduct))
                            {
                                Thread.Sleep(1500);
                                //plc.WriteByVariableName("RemoveBatteryComplete", 1);
                                MotionEnum.EnumRemoveProduct = MotionEnum.RemoveProduct.移除电池完成;
                            }
                            else
                            {
                                MotionEnum.EnumRemoveProduct = MotionEnum.RemoveProduct.移除电池完成;
                                WriteLog("没有找到条码代号：" + tempBarcode + " 的电池！移除电池失败！", LogLevels.Info, "Task_RemoveProduct");
                                Thread.Sleep(1500);

                            }
                        }

                        break;

                    case MotionEnum.RemoveProduct.移除电池完成:

                        plc.WriteByVariableName("RemoveBatteryComplete", 1);
                        if (plc.ReadByVariableName("RemoveBatterySignal") == "0")
                        {
                            plc.WriteByVariableName("RemoveBatteryComplete", 0);
                            MotionEnum.EnumRemoveProduct = MotionEnum.RemoveProduct.触发移除电池;
                        }
                        break;
                }
            }
        }


        private bool GetMdiData(ref BatterySeat bt, int time, int position, bool isGrr = false)
        {
            try
            {
                int tickCount = Environment.TickCount;
                bt.MDIChannel = position;
                string DimensionStr = string.Empty;
                string cmd = "";
                string remark = "";
                if (plc.ReadByVariableName("SendTempBarcodeToCCD") == "1")
                {
                    cmd = "Run_ACQ2," + bt.Sn + "," + position + "," + bt.TempBarcode + "\r";
                }
                else
                {
                    cmd = "Run_ACQ2," + bt.Sn + "," + position + "\r";
                }
                //if (isGrr == true && plc.ReadByVariableName("TestMode") == "4")
                //{
                //    cmd = "Run_ACQ2," + bt.Sn + "\r";
                //}

                try
                {
                    //CodeReaderIF.ClientSendMsg(cmd, 2);
                    bool ok = vision.SendCommand(cmd);
                    WriteLog(string.Format("第{0}次 请求尺寸测量数据指令 {1}, 通讯结果{2}", time, cmd, ok), LogLevels.Info, "GetFQIData");
                    if (!ok)
                    {
                        WriteLog(string.Format("第{0}次 尺寸测量重连", time), LogLevels.Warn, "GetFQIData");
                        //CodeReaderIF.Init_ClientConnet(HardwareConfig.Instance.DimensionIPAdress, HardwareConfig.Instance.DimensionPort, 2);//尺寸测量连接
                        vision.InitialShangLingVision(HardwareConfig.Instance.DimensionIPAdress, HardwareConfig.Instance.DimensionPort);
                        Thread.Sleep(500);
                        //CodeReaderIF.ClientSendMsg(cmd, 2);
                        vision.SendCommand(cmd);
                    }
                }
                catch (Exception ex)
                {
                    WriteLog(string.Format("第{0}次 尺寸测量重连", time), LogLevels.Warn, "GetFQIData");
                    //CodeReaderIF.Init_ClientConnet(HardwareConfig.Instance.DimensionIPAdress, HardwareConfig.Instance.DimensionPort, 2);//尺寸测量连接
                    vision.InitialShangLingVision(HardwareConfig.Instance.DimensionIPAdress, HardwareConfig.Instance.DimensionPort);
                    Thread.Sleep(500);
                    //CodeReaderIF.ClientSendMsg(cmd, 2);
                    vision.SendCommand(cmd);
                }
                WriteLog(string.Format("第{0}次 等待尺寸测量返回数据", time), LogLevels.Info, "GetFQIData");
                Thread.Sleep(350);

                //断线时发送成功，接收可能异常
                try
                {
                    //DimensionStr = CodeReaderIF.ClientReceiveMsg(2);
                    DimensionStr = vision.ReceiveData();
                }
                catch (Exception ex)
                {
                    WriteLog(string.Format("第{0}次 尺寸测量重连", time), LogLevels.Warn, "GetFQIData");
                    //CodeReaderIF.Init_ClientConnet(HardwareConfig.Instance.DimensionIPAdress, HardwareConfig.Instance.DimensionPort, 2);//尺寸测量连接
                    vision.InitialShangLingVision(HardwareConfig.Instance.DimensionIPAdress, HardwareConfig.Instance.DimensionPort);
                    Thread.Sleep(500);
                    //CodeReaderIF.ClientSendMsg(cmd, 2);
                    vision.SendCommand(cmd);
                    Thread.Sleep(350);
                    //DimensionStr = CodeReaderIF.ClientReceiveMsg(2);
                    DimensionStr = vision.ReceiveData();
                }

                while (DimensionStr != "OK")
                {
                    try
                    {
                        if (_AutoRunning == false) { return false; }
                        //CodeReaderIF.ClientSendMsg(cmd, 2);
                        bool ok = vision.SendCommand(cmd);
                        WriteLog(string.Format("第{0}次 请求尺寸测量数据指令 {1}, 通讯结果{2}", time, cmd, ok), LogLevels.Info, "GetFQIData");
                        if (!ok)
                        {
                            WriteLog(string.Format("第{0}次 尺寸测量重连", time), LogLevels.Warn, "GetFQIData");
                            //CodeReaderIF.Init_ClientConnet(HardwareConfig.Instance.DimensionIPAdress, HardwareConfig.Instance.DimensionPort, 2);//尺寸测量连接
                            vision.InitialShangLingVision(HardwareConfig.Instance.DimensionIPAdress, HardwareConfig.Instance.DimensionPort);
                            Thread.Sleep(350);
                            //CodeReaderIF.ClientSendMsg(cmd, 2);
                            vision.SendCommand(cmd);
                        }
                    }
                    catch (Exception ex)
                    {
                        WriteLog(string.Format("第{0}次 尺寸测量重连", time), LogLevels.Warn, "GetFQIData");
                        //CodeReaderIF.Init_ClientConnet(HardwareConfig.Instance.DimensionIPAdress, HardwareConfig.Instance.DimensionPort, 2);//尺寸测量连接
                        vision.InitialShangLingVision(HardwareConfig.Instance.DimensionIPAdress, HardwareConfig.Instance.DimensionPort);
                        Thread.Sleep(500);
                        //CodeReaderIF.ClientSendMsg(cmd, 2);
                        vision.SendCommand(cmd);
                    }
                    WriteLog(string.Format("第{0}次 等待尺寸测量返回数据", time), LogLevels.Info, "GetFQIData");
                    Thread.Sleep(350);

                    //断线时发送成功，接收可能异常
                    try
                    {
                        DimensionStr = vision.ReceiveData();
                    }
                    catch (Exception ex)
                    {
                        WriteLog(string.Format("第{0}次 尺寸测量重连", time), LogLevels.Warn, "GetFQIData");
                        //CodeReaderIF.Init_ClientConnet(HardwareConfig.Instance.DimensionIPAdress, HardwareConfig.Instance.DimensionPort, 2);//尺寸测量连接
                        vision.InitialShangLingVision(HardwareConfig.Instance.DimensionIPAdress, HardwareConfig.Instance.DimensionPort);
                        Thread.Sleep(500);
                        //CodeReaderIF.ClientSendMsg(cmd, 2);
                        vision.SendCommand(cmd);
                        Thread.Sleep(350);
                        //DimensionStr = CodeReaderIF.ClientReceiveMsg(2);
                        DimensionStr = vision.ReceiveData();
                    }
                }

                WriteLog(string.Format("第{0}次 尺寸测量返回信号 {1} 耗时 {2} 毫秒", time, DimensionStr, Environment.TickCount - tickCount), LogLevels.Info, "GetFQIData");
                tickCount = Environment.TickCount;
                if (isGrr == false)
                {
                    if (bt.MDIChannel == 1 || bt.MDIChannel == 3)
                    {
                        plc.WriteByVariableName("MDIComplete1", 1);
                    }
                    else if (bt.MDIChannel == 2 || bt.MDIChannel == 4)
                    {
                        plc.WriteByVariableName("MDIComplete2", 1);
                    }
                }
                Thread.Sleep(100);
                //DimensionStr = CodeReaderIF.ClientReceiveMsg(2);
                DimensionStr = vision.ReceiveData();
                WriteLog(string.Format("第{0}次 {1} 尺寸测量结果为 {2} 耗时 {3} 毫秒", time, bt.Sn, DimensionStr, Environment.TickCount - tickCount), LogLevels.Info, "GetFQIData");

                if (DimensionStr == oldMDIData)
                {
                    string errmsg = "尺寸数据跟上次的一模一样，请确认！";
                    MessageBox.Show(errmsg, "提示", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information,
                        System.Windows.Forms.MessageBoxDefaultButton.Button1, System.Windows.Forms.MessageBoxOptions.ServiceNotification);
                    WriteLog(errmsg, LogLevels.Warn, "GetFQIData");
                }
                oldMDIData = DimensionStr;

                //DimensionStr = "ACQ3,OK,58.344,31.661,7.419,19.157,12.867,13.049"; 
                //NG,87.019,64.540,15.096,44.096,0.000,10.218,1.502,0.069,0.073,0.063
                if (DimensionStr.Contains("OK,") || DimensionStr.Contains("NG,"))
                {
                    //string[] str = DimensionStr.Substring(8).Split(',');
                    string[] str = DimensionStr.Substring(3).Split(',');
                    bt.BatLength = float.Parse(str[0]);
                    bt.BatWidth = float.Parse(str[1]);
                    bt.LeftLugMargin = float.Parse(str[2]);
                    bt.RightLugMargin = float.Parse(str[3]);
                    bt.LeftLugLength = float.Parse(str[4]);
                    bt.RightLugLength = float.Parse(str[5]);

                    bt.AllBatLength = float.Parse(str[6]);
                    bt.Left1WhiteGlue = float.Parse(str[7]);
                    bt.Left2WhiteGlue = float.Parse(str[8]);
                    bt.Right1WhiteGlue = float.Parse(str[9]);
                    bt.Right2WhiteGlue = float.Parse(str[10]);

                    if (bt.BatLength >= CheckParamsConfig.Instance.MinBatLength
                    && bt.BatLength <= CheckParamsConfig.Instance.MaxBatLength
                    && bt.BatWidth >= CheckParamsConfig.Instance.MinBatWidth
                    && bt.BatWidth <= CheckParamsConfig.Instance.MaxBatWidth
                    && bt.LeftLugMargin >= CheckParamsConfig.Instance.MinLeftLugMargin
                    && bt.LeftLugMargin <= CheckParamsConfig.Instance.MaxLeftLugMargin
                    && bt.RightLugMargin >= CheckParamsConfig.Instance.MinRightLugMargin
                    && bt.RightLugMargin <= CheckParamsConfig.Instance.MaxRightLugMargin
                    && bt.LeftLugLength >= CheckParamsConfig.Instance.MinLeftLugLength
                    && bt.LeftLugLength <= CheckParamsConfig.Instance.MaxLeftLugLength
                    && bt.RightLugLength >= CheckParamsConfig.Instance.MinRightLugLength
                    && bt.RightLugLength <= CheckParamsConfig.Instance.MaxRightLugLength
                    && bt.AllBatLength >= CheckParamsConfig.Instance.MinAllBatLength
                    && bt.AllBatLength <= CheckParamsConfig.Instance.MaxAllBatLength
                    && bt.Left1WhiteGlue >= CheckParamsConfig.Instance.MinLeft1WhiteGlue
                    && bt.Left1WhiteGlue <= CheckParamsConfig.Instance.MaxLeft1WhiteGlue
                    && bt.Left2WhiteGlue >= CheckParamsConfig.Instance.MinLeft2WhiteGlue
                    && bt.Left2WhiteGlue <= CheckParamsConfig.Instance.MaxLeft2WhiteGlue
                    && bt.Right1WhiteGlue >= CheckParamsConfig.Instance.MinRight1WhiteGlue
                    && bt.Right1WhiteGlue <= CheckParamsConfig.Instance.MaxRight1WhiteGlue
                    && bt.Right2WhiteGlue >= CheckParamsConfig.Instance.MinRight2WhiteGlue
                    && bt.Right2WhiteGlue <= CheckParamsConfig.Instance.MaxRight2WhiteGlue)
                    {
                        bt.DimensionResult = true;
                        mdiNGCounts = 0;
                    }
                    else
                    {
                        remark += "尺寸NG(";
                        if (bt.AllBatLength < CheckParamsConfig.Instance.MinAllBatLength || bt.AllBatLength > CheckParamsConfig.Instance.MaxAllBatLength)
                        {
                            remark += "总长度NG;";
                        }
                        if (bt.BatLength < CheckParamsConfig.Instance.MinBatLength || bt.BatLength > CheckParamsConfig.Instance.MaxBatLength)
                        {
                            remark += "主体长度NG;";
                        }
                        if (bt.BatWidth < CheckParamsConfig.Instance.MinBatWidth || bt.BatWidth > CheckParamsConfig.Instance.MaxBatWidth)
                        {
                            remark += "主体宽度NG;";
                        }
                        if (bt.LeftLugMargin < CheckParamsConfig.Instance.MinLeftLugMargin || bt.LeftLugMargin > CheckParamsConfig.Instance.MaxLeftLugMargin)
                        {
                            remark += "左极耳边距NG;";
                        }
                        if (bt.RightLugMargin < CheckParamsConfig.Instance.MinRightLugMargin || bt.RightLugMargin > CheckParamsConfig.Instance.MaxRightLugMargin)
                        {
                            remark += "右极耳边距NG;";
                        }
                        if (bt.LeftLugLength < CheckParamsConfig.Instance.MinLeftLugLength || bt.LeftLugLength > CheckParamsConfig.Instance.MaxLeftLugLength)
                        {
                            remark += "左极耳长度NG;";
                        }
                        if (bt.RightLugLength < CheckParamsConfig.Instance.MinRightLugLength || bt.RightLugLength > CheckParamsConfig.Instance.MaxRightLugLength)
                        {
                            remark += "右极耳长度NG;";
                        }
                        if (bt.Left1WhiteGlue < CheckParamsConfig.Instance.MinLeft1WhiteGlue || bt.Left1WhiteGlue > CheckParamsConfig.Instance.MaxLeft1WhiteGlue)
                        {
                            remark += "左1小白胶NG;";
                        }
                        if (bt.Left2WhiteGlue < CheckParamsConfig.Instance.MinLeft2WhiteGlue || bt.Left2WhiteGlue > CheckParamsConfig.Instance.MaxLeft2WhiteGlue)
                        {
                            remark += "左2小白胶NG;";
                        }
                        if (bt.Right1WhiteGlue < CheckParamsConfig.Instance.MinRight1WhiteGlue || bt.Right1WhiteGlue > CheckParamsConfig.Instance.MaxRight1WhiteGlue)
                        {
                            remark += "右1小白胶NG;";
                        }
                        if (bt.Right2WhiteGlue < CheckParamsConfig.Instance.MinRight2WhiteGlue || bt.Right2WhiteGlue > CheckParamsConfig.Instance.MaxRight2WhiteGlue)
                        {
                            remark += "右2小白胶NG;";
                        }
                        remark += ")";
                        bt.NgItem += remark + "/";
                        bt.DimensionResult = false;
                        if (isGrr == false)
                        {
                            mdiNGCounts++;
                            if (mdiNGCounts >= CheckParamsConfig.Instance.MDIWarmingCounts)
                            {
                                string errmsg = "FQI 连续NG " + mdiNGCounts + " 个！！！";
                                System.Windows.MessageBox.Show(errmsg, "提示", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning,
                                    System.Windows.MessageBoxResult.OK, System.Windows.MessageBoxOptions.ServiceNotification);
                                WriteLog(errmsg, LogLevels.Warn, "GetFQIData");
                                mdiNGCounts = 0;
                            }
                        }
                    }

                    WriteLog(string.Format("第{0}次 尺寸测量 电池长度为{1}，宽度为{2}，左极耳边距为{3}，右极耳边距为{4}，左极耳边长度为{5}，右极耳长度为{6}，电池总长度为{7}，左1小白胶为{8}，左2小白胶为{9}，右1小白胶为{10}，右2小白胶为{11}，结果为{12}，备注为{13}",
                        time,
                        bt.BatLength,
                        bt.BatWidth,
                        bt.LeftLugMargin,
                        bt.RightLugMargin,
                        bt.LeftLugLength,
                        bt.RightLugLength,
                        bt.AllBatLength,
                        bt.Left1WhiteGlue,
                        bt.Left2WhiteGlue,
                        bt.Right1WhiteGlue,
                        bt.Right2WhiteGlue,
                        bt.DimensionResult,
                        remark
                        ), LogLevels.Info, "GetFQIData");
                }
                else
                {
                    bt.NgItem += "尺寸NG/";
                    WriteLog(string.Format("第{0}次 尺寸测量失败: {1}", time, DimensionStr), LogLevels.Warn, "GetFQIData");
                }
                try
                {
                    //plc.ReadByVariableName("NOMDI") == "1" 仅仅为了纠偏，不保存数据
                    if (plc.ReadByVariableName("NOMDI") == "0")
                    {
                        if (isGrr == true)
                        {
                            SaveDimensionDataToFile(bt, remark, true);
                        }
                        else
                        {
                            SaveDimensionDataToFile(bt, remark);
                        }
                    }
                }
                catch (Exception ex)
                {
                    WriteLog(string.Format("在第{0}次 保存尺寸测量数据出错: {1}", time, ex.Message.ToString()), LogLevels.Warn, "GetFQIData");
                }
                return true;
            }
            catch (Exception ex)
            {
                WriteLog(string.Format("在第{0}次 尺寸测量出错: {1}", time, ex.Message.ToString()), LogLevels.Warn, "GetFQIData");
                return false;
            }
        }

        /// <summary>
        /// 获取测厚数据
        /// </summary>
        /// <param name="index">次数</param>
        /// <param name="bt">电池对象</param>
        /// <param name="position">仪器编号</param>
        private void GetTickness(int index, ref BatterySeat bt, int position)
        {
            float thicknessValue;
            string thickness = "";
            try
            {
                MitutoyoReaderIF.ReadThickness(ref thickness, position);
                if (thickness.Contains("01A"))
                {
                    thickness = thickness.Substring(thickness.LastIndexOf("A") + 1);
                    thicknessValue = float.Parse(thickness);
                    thicknessValue = thicknessValue + CheckParamsConfig.Instance.CaliValThickness;
                    thicknessValue = (thicknessValue * CheckParamsConfig.Instance.ThicknessKValue + CheckParamsConfig.Instance.ThicknessBValue) * CheckParamsConfig.Instance.CellKValue + CheckParamsConfig.Instance.CellBValue;
                    thicknessValue = (float)Math.Round(thicknessValue, 3);
                    bt.Thickness = thicknessValue;
                    if (thicknessValue > CheckParamsConfig.Instance.MaxThickness ||
                        thicknessValue < CheckParamsConfig.Instance.MinThickness)
                    {
                        bt.ThicknessResult = false;
                        bt.NgItem += "厚度NG/";
                    }
                    else
                    {
                        bt.ThicknessResult = true;
                        switch (position)
                        {
                            case 1:
                                listPPGdata1.Add(bt.Thickness);
                                if (listPPGdata1.Count > 90)
                                {
                                    listPPGdata1.RemoveAt(0);
                                }
                                break;
                            case 2:
                                listPPGdata2.Add(bt.Thickness);
                                if (listPPGdata2.Count > 90)
                                {
                                    listPPGdata2.RemoveAt(0);
                                }
                                break;
                            case 3:
                                listPPGdata3.Add(bt.Thickness);
                                if (listPPGdata3.Count > 90)
                                {
                                    listPPGdata3.RemoveAt(0);
                                }
                                break;
                            case 4:
                                listPPGdata4.Add(bt.Thickness);
                                if (listPPGdata4.Count > 90)
                                {
                                    listPPGdata4.RemoveAt(0);
                                }
                                break;
                        }
                        if (listPPGdata1.Count >= 90 && listPPGdata2.Count >= 90 && listPPGdata3.Count >= 90 && listPPGdata4.Count >= 90)
                        {
                            float[] avg = new float[4]
                            {
                                listPPGdata1.Average(),
                                listPPGdata2.Average(),
                                listPPGdata3.Average(),
                                listPPGdata4.Average()
                            };

                            if (avg.Max() - avg.Min() > CheckParamsConfig.Instance.StationRange)
                            {
                                string errmsg = "4个测厚平台各自的平均值极差为：" + (avg.Max() - avg.Min()) + " 超过" + CheckParamsConfig.Instance.StationRange + "mm"
                                    + " 工位1：" + listPPGdata1.Average() + " 工位2：" + listPPGdata2.Average() + " 工位3：" + listPPGdata3.Average() + "工位4：" + listPPGdata4.Average();
                                System.Windows.MessageBox.Show(errmsg, "提示", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information, System.Windows.MessageBoxResult.OK, System.Windows.MessageBoxOptions.ServiceNotification);
                                WriteLog(string.Format("第{0}次 {1}", index, errmsg), LogLevels.Warn, "GetTickness");
                                listPPGdata1.Clear();
                                listPPGdata2.Clear();
                                listPPGdata3.Clear();
                                listPPGdata4.Clear();
                            }
                            else
                            {
                                float min = CheckParamsConfig.Instance.StationWarmingAverage - CheckParamsConfig.Instance.StationWarmingTolerance;
                                float max = CheckParamsConfig.Instance.StationWarmingAverage + CheckParamsConfig.Instance.StationWarmingTolerance;

                                if (listPPGdata1.Average() < min || listPPGdata1.Average() > max)
                                {
                                    string errmsg = "工位1厚度均值为：" + listPPGdata1.Average() + "mm,不在 " + min + " -- " + max + " 范围内";
                                    System.Windows.MessageBox.Show(errmsg, "提示", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information,
                                        System.Windows.MessageBoxResult.OK, System.Windows.MessageBoxOptions.ServiceNotification);
                                    WriteLog(errmsg, LogLevels.Warn, "GetTickness");
                                    listPPGdata1.Clear();
                                }
                                if (listPPGdata2.Average() < min || listPPGdata1.Average() > max)
                                {
                                    string errmsg = "工位1厚度均值为：" + listPPGdata2.Average() + "mm,不在 " + min + " -- " + max + " 范围内";
                                    System.Windows.MessageBox.Show(errmsg, "提示", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information,
                                        System.Windows.MessageBoxResult.OK, System.Windows.MessageBoxOptions.ServiceNotification);
                                    WriteLog(errmsg, LogLevels.Warn, "GetTickness");
                                    listPPGdata2.Clear();
                                }
                                if (listPPGdata3.Average() < min || listPPGdata1.Average() > max)
                                {
                                    string errmsg = "工位1厚度均值为：" + listPPGdata3.Average() + "mm,不在 " + min + " -- " + max + " 范围内";
                                    System.Windows.MessageBox.Show(errmsg, "提示", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information,
                                        System.Windows.MessageBoxResult.OK, System.Windows.MessageBoxOptions.ServiceNotification);
                                    WriteLog(errmsg, LogLevels.Warn, "GetTickness");
                                    listPPGdata3.Clear();
                                }
                                if (listPPGdata4.Average() < min || listPPGdata1.Average() > max)
                                {
                                    string errmsg = "工位1厚度均值为：" + listPPGdata4.Average() + "mm,不在 " + min + " -- " + max + " 范围内";
                                    System.Windows.MessageBox.Show(errmsg, "提示", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information,
                                        System.Windows.MessageBoxResult.OK, System.Windows.MessageBoxOptions.ServiceNotification);
                                    WriteLog(errmsg, LogLevels.Warn, "GetTickness");
                                    listPPGdata4.Clear();
                                }
                            }

                        }

                    }
                    WriteLog(string.Format("第{0}次 测厚原数据为: {1} ，补偿后数据为: {2}", index, thickness, thicknessValue), LogLevels.Info, "GetTickness");
                    try
                    {
                        SaveThicknessDataToFile(bt);
                    }
                    catch (Exception ex)
                    {
                        WriteLog(string.Format("第{0}次 保存测厚数据异常: {1}", index, ex), LogLevels.Error, "GetTickness");
                    }
                }
                else
                {
                    bt.NgItem += "厚度NG/";
                    WriteLog(string.Format("第{0}次 解析字串失败: {1}", index, thickness), LogLevels.Warn, "GetTickness");
                }
            }
            catch (Exception ex)
            {
                WriteLog(string.Format("第{0}次 测厚异常: {1}", index, ex), LogLevels.Error, "GetTickness");
            }
        }



        /// <summary>
        ///  开班点检，每天开班点检，必须连续三个
        /// </summary>
        /// <param name="barcode"></param>
        /// <param name="result"></param>
        /// <param name="msg"></param>
        /// <returns> 返回0成功，非0失败</returns>
        public int CheckOutEveryDay(string barcode, int result, ref string msg)
        {
            int iRet = -1;
            bool bNeedCheckOut = true;
            string strResult = string.Empty;

            if (barcode.Length != 12 && barcode.Length != 30)
            {
                return 0;
            }

            //比较日期
            DateTime time = DateTime.Now;
            string strCurrentTime = time.ToString("yyyyMMdd");
            int iCurrentTime = int.Parse(strCurrentTime);
            int iYearCurrent = time.Year;
            int iMonthCurrent = time.Month;
            int iDayCurrent = time.Day;
            int iHourCurrent = time.Hour;
            int iMinuteCurrent = time.Minute;
            int iMonthDay = DateTime.DaysInMonth(iYearCurrent, iMonthCurrent - 1);//获取当前月有多少天

            int iLastCheckOutTime = int.Parse(_checkStatus.MyStartupTestConfig.LastCheckOutTime.ToString("yyyyMMdd"));
            int iLastCheckOutHour = _checkStatus.MyStartupTestConfig.LastCheckOutHour;
            int iYearLast = iLastCheckOutTime / 10000;
            int lTemp = iLastCheckOutTime % 10000;
            int iMonthLast = lTemp / 100;
            int iDayLast = lTemp % 100;

            //白班
            if (iHourCurrent >= 7 && iHourCurrent < 19)//7:00-18:59:59
            {
                if (iCurrentTime == iLastCheckOutTime && iLastCheckOutHour >= 7)//有当天7点以后点检记录则不点检
                {
                    bNeedCheckOut = false;//不需要点检
                }
            }
            else //夜班 19:00-隔日6:59:59
            {
                if (iHourCurrent < 7)//7点之前
                {
                    if (1 == iDayCurrent) //月初1号处理
                    {
                        if (iDayLast == iMonthDay && iLastCheckOutHour >= 18)//月底晚班24点之前点检
                        {
                            bNeedCheckOut = false;//不需要点检
                        }

                        if (iLastCheckOutHour < 7 && iDayLast == 1)//1号已经点检
                        {
                            bNeedCheckOut = false;//不需要点检
                        }

                    }
                    else //2日开始计算
                    {
                        if (iYearCurrent == iYearLast && iMonthCurrent == iMonthLast)//上次点检同年同月
                        {
                            if (iDayCurrent == iDayLast)//上次点检同日
                            {
                                bNeedCheckOut = false;//不需要点检
                            }
                            else
                            {
                                if (iDayCurrent - iDayLast == 1 && iLastCheckOutHour >= 18)//次日
                                {
                                    bNeedCheckOut = false;//不需要点检
                                }
                            }

                        }
                    }
                }
                else if (iHourCurrent >= 19)//19点之后
                {
                    if (iCurrentTime == iLastCheckOutTime && iLastCheckOutHour >= 19)
                    {
                        bNeedCheckOut = false;
                    }
                }
            }

            //如果今天点检过了返回true
            if (!bNeedCheckOut)
            {
                iRet = 0;
            }
            else
            {
                //如果还没有点检，判断来料是否为点检料，并记录
                bool bIsCheckOutBarcode = false;

                for (int index = 0; index < BatteryCheckIF.MyTestCodeManager.CodeList.Count; ++index)
                {
                    if (barcode == BatteryCheckIF.MyTestCodeManager.CodeList[index].BarCode)
                    {
                        bIsCheckOutBarcode = true;
                        break;
                    }
                }

                if (bIsCheckOutBarcode)
                {
                    if (1 == result)
                    {
                        strResult = "OK";
                    }
                    else
                    {
                        strResult = "NG";
                    }

                    WriteLog($"条码：{barcode}, 结果： {strResult}", LogLevels.Info, "CheckOutEveryDay");

                    if (1 == result)
                    {
                        checkOutTimes = 0;
                        msg = "点检电芯结果为OK，点检失败。";
                        iRet = 1;
                    }
                    else
                    {
                        if (lastCheckOutBarcode.Contains(barcode))
                        {
                            checkOutTimes = 0;
                            msg = "重复点检电芯条码，点检失败。";
                            iRet = 2;
                        }
                        else
                        {
                            //点检OK，计数加一
                            iRet = 0;
                            checkOutTimes++;
                            lastCheckOutBarcode.Add(barcode);

                            if (3 == checkOutTimes)
                            {
                                _checkStatus.MyStartupTestConfig.LastCheckOutTime = DateTime.Now;
                                _checkStatus.MyStartupTestConfig.LastCheckOutHour = DateTime.Now.Hour;
                                CheckParamsConfig.Instance.SaveStartupConfig();

                                lastCheckOutBarcode.Clear();
                            }
                        }
                    }
                }
                else
                {
                    if ((7 == iHourCurrent || (iHourCurrent == 8 && iMinuteCurrent < 30)) || (19 == iHourCurrent || (iHourCurrent == 20 && iMinuteCurrent < 30)))
                    {
                        //早晚班七点到八点半之前未点检不报警
                        iRet = 0;
                    }
                    else
                    {
                        msg = "需要点检，不是点检电芯或者电芯条码不在点检条码库中。";
                        checkOutTimes = 0;
                        iRet = 3;
                    }
                }
            }

            return iRet;
        }

        public bool CheckAlgoVersion()
        {
            bool ret = false;

            string localAlgoVersion = "";
            GetFileVersion("zy_Xray_inspection.dll", ref localAlgoVersion);

            string inputAlgoVersion = UserDefineVariableInfo.DicVariables["AlgoVersionInput"].ToString();
            if (inputAlgoVersion == localAlgoVersion)
            {
                ret = true;
            }
            else
            {
                //VisionSysWrapperIF.UnInitAlgo();
                //string algoFilePath = UserDefineVariableInfo.DicVariables["AlgoFilePath"].ToString();

                //System.IO.File.Copy(algoFilePath, "zy_Xray_inspection.dll", true);

                //VisionSysWrapperIF.InitAlgo();
            }

            return ret;
        }

        public void GetFileVersion(string file, ref string version)
        {
            System.IO.FileInfo fileInfo = null;
            try
            {
                fileInfo = new System.IO.FileInfo(file);
            }
            catch (Exception e)
            {
                WriteLog(e.Message.ToString(), LogLevels.Info, "GetFileVersion");
                return;
            }

            if (fileInfo != null && fileInfo.Exists)
            {
                System.Diagnostics.FileVersionInfo info = System.Diagnostics.FileVersionInfo.GetVersionInfo(file);
                int majorp = info.ProductMajorPart;
                int minorp = info.ProductMinorPart;
                int buildp = info.ProductBuildPart;
                int privatep = info.ProductPrivatePart;
                version = String.Format("V{0}.{1}.{2}.{3}", majorp, minorp, buildp, privatep);

                UserDefineVariableInfo.DicVariables["AlgoVersionLocal"] = version;
            }
        }

        public string GetVersion(string file)
        {
            string version = "1.0.0.0";
            System.IO.FileInfo fileInfo = null;
            try
            {
                fileInfo = new System.IO.FileInfo(file);
            }
            catch (Exception e)
            {
                WriteLog(e.Message.ToString(), LogLevels.Info, "GetFileVersion");
                return version;
            }

            if (fileInfo != null && fileInfo.Exists)
            {
                System.Diagnostics.FileVersionInfo info = System.Diagnostics.FileVersionInfo.GetVersionInfo(file);
                int majorp = info.ProductMajorPart;
                int minorp = info.ProductMinorPart;
                int buildp = info.ProductBuildPart;
                int privatep = info.ProductPrivatePart;
                version = String.Format("V{0}.{1}.{2}.{3}", majorp, minorp, buildp, privatep);
            }
            return version;
        }

        public void SaveXRayResultToFile(BatterySeat seat, int layerAC, int layerBD, int inspectMode, bool is_err, bool isGrr = false)
        {
            string modeInfo = "Default";
            if (ATL.Core.DeivceEquipmentidPlcInfo.lstDeivceEquipmentidPlcInfos[0].ModelInfo != "")
            {
                modeInfo = ATL.Core.DeivceEquipmentidPlcInfo.lstDeivceEquipmentidPlcInfos[0].ModelInfo;
            }
            string filePath = "D:\\测量数据\\生产数据\\X-Ray测量数据\\" + modeInfo;
            if (isGrr == true)
            {
                filePath = "D:\\测量数据\\生产数据\\X-Ray测量数据\\" + modeInfo + "\\GRR";
            }
            string fileName = filePath + "\\X-Ray_" + DateTime.Now.ToString("yyyyMMdd") + ".csv";

            if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);


            string line = "";
            byte[] myByte = System.Text.Encoding.UTF8.GetBytes(line);

            if (!File.Exists(fileName))
            {
                line = "条码,时间,OK/NG,角号,算法结果,最小值,最大值,L_1,L_2,L_3,L_4,L_5,L_6,L_7,L_8,L_9,L_10,L_11,L_12,L_13,L_14,L_15,L_16,L_17,L_18,L_19,L_20,L_21,L_22,L_23,L_24,L_25,L_26,L_27,L_28,L_29,L_30," +
                    "ANG_1,ANG_2,ANG_3,ANG_4,ANG_5,ANG_6,ANG_7,ANG_8,ANG_9,ANG_10,ANG_11,ANG_12,ANG_13,ANG_14,ANG_15,ANG_16,ANG_17,ANG_18,ANG_19,ANG_20,ANG_21,ANG_22,ANG_23,ANG_24,ANG_25,ANG_26,ANG_27,ANG_28,ANG_29,ANG_30\r\n";
                myByte = System.Text.Encoding.UTF8.GetBytes(line);
                using (FileStream fsWrite = new FileStream(fileName, FileMode.Create))
                {
                    byte[] bs = { (byte)0xEF, (byte)0xBB, (byte)0xBF };
                    fsWrite.Write(bs, 0, bs.Length);
                    fsWrite.Write(myByte, 0, myByte.Length);
                };
            }

            string time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string okng = seat.FinalResult == true ? "OK" : "NG";
            string Sn = string.Empty;
            if (is_err == true)
            {
                Sn = "ERRORCODE";//ResultCode为 黑白图或位置错误或未知错误时，写入CSV文件的条码记为ERRORCODE
                seat.Corner1.InspectResults.ResetArray();
                seat.Corner2.InspectResults.ResetArray();
                seat.Corner3.InspectResults.ResetArray();
                seat.Corner4.InspectResults.ResetArray();
                //seat.Corner1.Reset();
                //seat.Corner2.Reset();
                //seat.Corner3.Reset();
                //seat.Corner4.Reset();

            }
            else
            {
                if (seat.LengthBarcode.Length > 12)
                {
                    Sn = seat.LengthBarcode;
                }
                else
                {
                    Sn = seat.Sn;
                }
            }

            if (inspectMode == 0 || inspectMode == 2)
            {
                line = Sn + "," + time + "," + okng + "," + "3" + "," + seat.Corner3.InspectResults.resultDataMin.iResult.ToString() + "," + seat.Corner3.InspectResults.resultDataMin.fMinDis.ToString() + "," + seat.Corner3.InspectResults.resultDataMin.fMaxDis.ToString();

                for (int i = 0; i < layerBD; i++)
                {
                    line += ",";
                    line += seat.Corner3.InspectResults.vecDis[i].ToString();
                }

                for (int i = 0; i < 30 - layerBD; i++)
                {
                    line += ",";
                    line += " ";
                }

                for (int i = 0; i < layerBD; i++)
                {
                    line += ",";
                    line += seat.Corner3.InspectResults.vecAngles[i].ToString();
                }

                for (int i = 0; i < 30 - layerBD; i++)
                {
                    line += ",";
                    line += " ";
                }

                line += "\r\n";

                line += Sn + "," + time + "," + okng + "," + "1" + "," + seat.Corner1.InspectResults.resultDataMin.iResult.ToString() + "," + seat.Corner1.InspectResults.resultDataMin.fMinDis.ToString() + "," + seat.Corner1.InspectResults.resultDataMin.fMaxDis.ToString();

                for (int i = 0; i < layerAC; i++)
                {
                    line += ",";
                    line += seat.Corner1.InspectResults.vecDis[i].ToString();
                }

                for (int i = 0; i < 30 - layerAC; i++)
                {
                    line += ",";
                    line += " ";
                }

                for (int i = 0; i < layerAC; i++)
                {
                    line += ",";
                    line += seat.Corner1.InspectResults.vecAngles[i].ToString();
                }

                for (int i = 0; i < 30 - layerAC; i++)
                {
                    line += ",";
                    line += " ";
                }

                line += "\r\n";

                myByte = System.Text.Encoding.UTF8.GetBytes(line);
                using (FileStream fsWrite = new FileStream(fileName, FileMode.Append))
                {
                    fsWrite.Write(myByte, 0, myByte.Length);
                };
            }

            if (inspectMode == 1 || inspectMode == 0)
            {
                line = Sn + "," + time + "," + okng + "," + "2" + "," + seat.Corner2.InspectResults.resultDataMin.iResult.ToString() + "," + seat.Corner2.InspectResults.resultDataMin.fMinDis.ToString() + "," + seat.Corner2.InspectResults.resultDataMin.fMaxDis.ToString();

                for (int i = 0; i < layerBD; i++)
                {
                    line += ",";
                    line += seat.Corner2.InspectResults.vecDis[i].ToString();
                }

                for (int i = 0; i < 30 - layerBD; i++)
                {
                    line += ",";
                    line += " ";
                }

                for (int i = 0; i < layerBD; i++)
                {
                    line += ",";
                    line += seat.Corner2.InspectResults.vecAngles[i].ToString();
                }

                for (int i = 0; i < 30 - layerBD; i++)
                {
                    line += ",";
                    line += " ";
                }

                line += "\r\n";

                line += Sn + "," + time + "," + okng + "," + "4" + "," + seat.Corner4.InspectResults.resultDataMin.iResult.ToString() + "," + seat.Corner4.InspectResults.resultDataMin.fMinDis.ToString() + "," + seat.Corner4.InspectResults.resultDataMin.fMaxDis.ToString();

                for (int i = 0; i < layerAC; i++)
                {
                    line += ",";
                    line += seat.Corner4.InspectResults.vecDis[i].ToString();
                }

                for (int i = 0; i < 30 - layerAC; i++)
                {
                    line += ",";
                    line += " ";
                }

                for (int i = 0; i < layerAC; i++)
                {
                    line += ",";
                    line += seat.Corner4.InspectResults.vecAngles[i].ToString();
                }

                for (int i = 0; i < 30 - layerAC; i++)
                {
                    line += ",";
                    line += " ";
                }

                line += "\r\n";

                myByte = System.Text.Encoding.UTF8.GetBytes(line);
                using (FileStream fsWrite = new FileStream(fileName, FileMode.Append))
                {
                    fsWrite.Write(myByte, 0, myByte.Length);
                };
            }
        }

        public void SaveThicknessDataToFile(BatterySeat seat, bool isGrr = false)
        {
            string modeInfo = "Default";
            if (ATL.Core.DeivceEquipmentidPlcInfo.lstDeivceEquipmentidPlcInfos[0].ModelInfo != "")
            {
                modeInfo = ATL.Core.DeivceEquipmentidPlcInfo.lstDeivceEquipmentidPlcInfos[0].ModelInfo;
            }

            string filePath = "D:\\测量数据\\生产数据\\PPG测量数据\\" + modeInfo;
            string fileName = filePath + "\\PPG_" + DateTime.Now.ToString("yyyyMMdd") + ".csv";

            if (isGrr == true)
            {
                filePath = "D:\\测量数据\\生产数据\\PPG测量数据\\" + modeInfo + "\\GRR";
                fileName = filePath + "\\PPG_" + DateTime.Now.ToString("yyyyMMdd") + ".csv";
            }
            if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);

            string line = "";
            byte[] myByte = System.Text.Encoding.UTF8.GetBytes(line);

            if (!File.Exists(fileName))
            {
                line = "条码,时间,OK/NG,厚度值,通道号\r\n";
                myByte = System.Text.Encoding.UTF8.GetBytes(line);
                using (FileStream fsWrite = new FileStream(fileName, FileMode.Create))
                {
                    byte[] bs = { (byte)0xEF, (byte)0xBB, (byte)0xBF };
                    fsWrite.Write(bs, 0, bs.Length);
                    fsWrite.Write(myByte, 0, myByte.Length);
                };
            }

            string time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string okng = seat.ThicknessResult == true ? "OK" : "NG";
            string barcode = "";
            if (seat.LengthBarcode.Length > 12)
            {
                barcode = seat.LengthBarcode;
            }
            else
            {
                barcode = seat.Sn;
            }

            line = barcode + "," + time + "," + okng + "," + seat.Thickness + "," + seat.PPGChannel + "\r\n";

            myByte = System.Text.Encoding.UTF8.GetBytes(line);
            using (FileStream fsWrite = new FileStream(fileName, FileMode.Append))
            {
                fsWrite.Write(myByte, 0, myByte.Length);
            }
        }

        public void SaveDimensionDataToFile(BatterySeat seat, string remark, bool isGrr = false)
        {
            string modeInfo = "Default";
            if (ATL.Core.DeivceEquipmentidPlcInfo.lstDeivceEquipmentidPlcInfos[0].ModelInfo != "")
            {
                modeInfo = ATL.Core.DeivceEquipmentidPlcInfo.lstDeivceEquipmentidPlcInfos[0].ModelInfo;
            }

            string filePath = "D:\\测量数据\\生产数据\\尺寸测量数据\\" + modeInfo;
            string fileName = filePath + "\\尺寸测量 _" + DateTime.Now.ToString("yyyyMMdd") + ".csv";

            if (isGrr == true)
            {
                filePath = "D:\\测量数据\\生产数据\\尺寸测量数据\\" + modeInfo + "\\GRR";
                fileName = filePath + "\\尺寸测量 _" + DateTime.Now.ToString("yyyyMMdd") + ".csv";
            }
            if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);

            string line = "";
            byte[] myByte = System.Text.Encoding.UTF8.GetBytes(line);

            if (!File.Exists(fileName))
            {
                line = "条码,时间,OK/NG,电池长度,电池宽度,左极耳边距,右极耳边距,左极耳长度,右极耳长度,电池总长度,左1小白胶,左2小白胶,右1小白胶,右2小白胶,通道号,备注\r\n";
                myByte = System.Text.Encoding.UTF8.GetBytes(line);
                using (FileStream fsWrite = new FileStream(fileName, FileMode.Create))
                {
                    byte[] bs = { (byte)0xEF, (byte)0xBB, (byte)0xBF };
                    fsWrite.Write(bs, 0, bs.Length);
                    fsWrite.Write(myByte, 0, myByte.Length);
                };
            }


            string time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string okng = seat.DimensionResult == true ? "OK" : "NG";
            string barcode = "";
            if (seat.LengthBarcode.Length > 12)
            {
                barcode = seat.LengthBarcode;
            }
            else
            {
                barcode = seat.Sn;
            }

            line = barcode + "," + time + "," + okng + "," + seat.BatLength + "," + seat.BatWidth + "," + seat.LeftLugMargin + ","
                + seat.RightLugMargin + "," + seat.LeftLugLength + ","
                + seat.RightLugLength + "," + seat.AllBatLength + "," + seat.Left1WhiteGlue + "," + seat.Left2WhiteGlue + "," + seat.Right1WhiteGlue + "," + seat.Right2WhiteGlue + "," + seat.MDIChannel + "," + remark + "\r\n";

            myByte = System.Text.Encoding.UTF8.GetBytes(line);
            using (FileStream fsWrite = new FileStream(fileName, FileMode.Append))
            {
                fsWrite.Write(myByte, 0, myByte.Length);
            };
        }

        public void SaveOCVDataToFile(BatterySeat seat, bool isGrr = false)
        {
            string modeInfo = "Default";
            if (ATL.Core.DeivceEquipmentidPlcInfo.lstDeivceEquipmentidPlcInfos[0].ModelInfo != "")
            {
                modeInfo = ATL.Core.DeivceEquipmentidPlcInfo.lstDeivceEquipmentidPlcInfos[0].ModelInfo;
            }

            string filePath = "D:\\测量数据\\生产数据\\OCV数据\\" + modeInfo;
            string fileName = filePath + "\\OCV _" + DateTime.Now.ToString("yyyyMMdd") + ".csv";

            if (isGrr == true)
            {
                filePath = "D:\\测量数据\\生产数据\\OCV数据\\" + modeInfo + "\\GRR";
                fileName = filePath + "\\OCV _" + DateTime.Now.ToString("yyyyMMdd") + ".csv";
            }
            if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);

            string line = "";
            byte[] myByte = System.Text.Encoding.UTF8.GetBytes(line);

            if (!File.Exists(fileName))
            {
                line = "条码,时间,OK/NG,电池内阻,电池电压,电池温度,电池K值,环境温度,通道号\r\n";
                myByte = System.Text.Encoding.UTF8.GetBytes(line);
                using (FileStream fsWrite = new FileStream(fileName, FileMode.Create))
                {
                    byte[] bs = { (byte)0xEF, (byte)0xBB, (byte)0xBF };
                    fsWrite.Write(bs, 0, bs.Length);
                    fsWrite.Write(myByte, 0, myByte.Length);
                };
            }

            string time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string barcode = "";
            if (seat.LengthBarcode.Length > 12)
            {
                barcode = seat.LengthBarcode;
            }
            else
            {
                barcode = seat.Sn;
            }

            line = barcode + "," + time + "," + (seat.OCVResult == true ? "OK" : "NG") + "," + seat.Resistance + "," + seat.Voltage + "," + seat.Temperature + "," + seat.K_Value + "," + seat.EnvirementTemperature + "," + seat.OCVChannel + "\r\n";

            myByte = System.Text.Encoding.UTF8.GetBytes(line);
            using (FileStream fsWrite = new FileStream(fileName, FileMode.Append))
            {
                fsWrite.Write(myByte, 0, myByte.Length);
            };
        }

        public void SaveOCVDataToFileBySetp(BatterySeat seat, bool isGrr = false)
        {
            string testType = "";
            if (OCVMode != "")
            {
                testType = OCVMode.Substring(OCVMode.ToString().Length - 2, 2);
            }
            else
            {
                testType = "O1";
            }
            string filePath = "D:\\测量数据\\生产数据\\O几数据\\" + testType;
            string fileName = filePath + "\\" + DateTime.Now.ToString("yyyyMMdd") + ".csv";

            if (isGrr == true)
            {
                filePath = "D:\\测量数据\\生产数据\\OCV数据\\" + testType + "\\GRR";
                fileName = filePath + "\\" + DateTime.Now.ToString("yyyyMMdd") + ".csv";
            }
            if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);

            string line = "";
            byte[] myByte = System.Text.Encoding.UTF8.GetBytes(line);

            if (!File.Exists(fileName))
            {
                line = "barcode,vol,res,BatteryTemp,pos,result,remark,time\r\n";
                myByte = System.Text.Encoding.UTF8.GetBytes(line);
                using (FileStream fsWrite = new FileStream(fileName, FileMode.Create))
                {
                    byte[] bs = { (byte)0xEF, (byte)0xBB, (byte)0xBF };
                    fsWrite.Write(bs, 0, bs.Length);
                    fsWrite.Write(myByte, 0, myByte.Length);
                };
            }

            string time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string barcode = "";
            if (seat.LengthBarcode.Length > 12)
            {
                barcode = seat.LengthBarcode;
            }
            else
            {
                barcode = seat.Sn;
            }

            line = barcode + "," + seat.Voltage + "," + seat.Resistance + "," + seat.Temperature + "," + seat.OCVChannel + "," + (seat.OCVResult == true ? "PASS" : "NG") + "," + seat.OcvRemark + "," + time + "\r\n";

            myByte = System.Text.Encoding.UTF8.GetBytes(line);
            using (FileStream fsWrite = new FileStream(fileName, FileMode.Append))
            {
                fsWrite.Write(myByte, 0, myByte.Length);
            };
        }

        public void SaveIVDataToFile(BatterySeat seat, bool isGrr = false)
        {
            try
            {
                string filePath = "D:\\测量数据\\生产数据\\IV数据\\" + ATL.Core.DeivceEquipmentidPlcInfo.lstDeivceEquipmentidPlcInfos[0].ModelInfo;
                string fileName = filePath + "\\IV _" + DateTime.Now.ToString("yyyyMMdd") + ".csv";

                if (isGrr == true)
                {
                    filePath = "D:\\测量数据\\生产数据\\IV数据\\" + ATL.Core.DeivceEquipmentidPlcInfo.lstDeivceEquipmentidPlcInfos[0].ModelInfo + "\\GRR";
                    fileName = filePath + "\\IV _" + DateTime.Now.ToString("yyyyMMdd") + ".csv";
                }
                if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);

                string line = "";
                byte[] myByte = System.Text.Encoding.UTF8.GetBytes(line);

                if (!File.Exists(fileName))
                {
                    line = "条码,时间,OK/NG,IV值1,IV值2,IV值3,IV值4,IV值5,IV值6,IV值,通道号,PLC导通信息,PC判断导通信息,备注\r\n";
                    myByte = System.Text.Encoding.UTF8.GetBytes(line);
                    using (FileStream fsWrite = new FileStream(fileName, FileMode.Create))
                    {
                        byte[] bs = { (byte)0xEF, (byte)0xBB, (byte)0xBF };
                        fsWrite.Write(bs, 0, bs.Length);
                        fsWrite.Write(myByte, 0, myByte.Length);
                    };
                }

                string time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                string pCConduction = "未导通";
                if (seat.KnifeNoConduction == false && seat.NeedleNoConduction == false)
                {
                    pCConduction = "导通";
                }
                line = seat.Sn + "," + time + "," + (seat.IVResult == true ? "OK" : "NG") + "," + seat.IV + "," + seat.IvData + "," + seat.IVChannel + "," + seat.IVPLCConduction + "," + pCConduction + "," + seat.IVRemark + "\r\n";

                myByte = System.Text.Encoding.UTF8.GetBytes(line);
                using (FileStream fsWrite = new FileStream(fileName, FileMode.Append))
                {
                    fsWrite.Write(myByte, 0, myByte.Length);
                }
            }
            catch (Exception ex)
            {
                WriteLog(string.Format("保存IV测量数据出错 {0}", ex.Message.ToString()), LogLevels.Error, "SaveIVDataToFile");
            }
        }

        DateTime XRayTubeStartTime = DateTime.Now;
        public bool _bool = true;

        /// <summary>
        /// 监控光管，确定光管开启时间
        /// </summary>
        //public void XRayTubeStateThread()
        //{
        //    new Thread(() =>
        //    {
        //        Thread.CurrentThread.IsBackground = true;
        //        while (true)
        //        {
        //            if (XRayTubeIF.XRayTube1Stauts.ShouldXrayOn && _bool)
        //            {
        //                XRayTubeStartTime = DateTime.Now;
        //                _bool = false;
        //                //WriteLog(string.Format("XRayTubeStartTime: {0}", XRayTubeStartTime.ToString()), LogLevels.Info, "IsXRayTubeCloseThread");
        //            }
        //            Thread.Sleep(250);

        //            if (!XRayTubeIF.XRayTube1Stauts.ShouldXrayOn)
        //            {
        //                _bool = true;
        //            }
        //            Thread.Sleep(250);
        //        }
        //    }).Start();
        //}


        /// <summary>
        /// 光管闲置超过规定时间自动关闭
        /// </summary>
        public void IsXRayTubeCloseThread()
        {
            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                while (true)
                {

                    if (XRayTubeIF.XRayTube1Stauts.ShouldXrayOn
                    && (DateTime.Now - XRayTubeStartTime).TotalMinutes > XRayTubeIdleTime)
                    {
                        XRayTubeIF.CloseXray(ETubePosition.Position1);
                        XRayTubeIF.CloseXray(ETubePosition.Position2);
                        XRayTubeStartTime = DateTime.Now;
                        _bool = true;

                        //关闭光管同时停止程序
                        //StopTest.Execute(null);
                    }

                    Thread.Sleep(1000);
                }
            }).Start();
        }

        //算法初始化完成提示
        public void AlgoInitFinHint_Thread()
        {
            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                while (true)
                {
                    if (Int16.Parse(UserDefineVariableInfo.DicVariables["AlgoInitFinished"].ToString()) == 1)
                    {
                        AlgoInitFinHint = "算法初始化完成";
                        break;
                    }
                    Thread.Sleep(500);
                }
            }).Start();
        }

        [DllImport("kernel32.dll", EntryPoint = "SetProcessWorkingSetSize")]
        public static extern int SetProcessWorkingSetSize(IntPtr process, int minSize, int maxSize);
        /// <summary> 
        /// 强制释放内存 把物理内存移到虚拟内存，降低电脑性能，造成不可预知问题，慎用
        /// </summary> 
        public static void ClearMemory()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                SetProcessWorkingSetSize(System.Diagnostics.Process.GetCurrentProcess().Handle, -1, -1);
            }
        }

        private void TestCorner3(ref BatterySeat bt, int time)
        {
            try
            {
                int tickCount = Environment.TickCount;
                WriteLog(string.Format("第{0}次 进入 算法角3线程,是 {1} 面电池", time, bt.CellType), LogLevels.Info, "TestCorner3");
                if (bt.CellType == "A")
                {
                    _workingSeats._seat1.Corner3.InspectParams.strBarcode = bt.Sn + "_" + bt.CellType;
                    //WriteLog(string.Format("{0}", _workingSeats._seat1.Corner3.ToString()), LogLevels.Info, "TestCorner3");
                    AlgoWrapper.Instance.ImageProcessInspect(ref _workingSeats._seat1.Corner3.InspectParams, ref bt.Corner3.ShotImage, ref bt.Corner3.InspectResults, algoMode, bt.Corner3.ImgNO);
                }
                else if (bt.CellType == "B")
                {
                    _workingSeats._seat2.Corner3.InspectParams.strBarcode = bt.Sn + "_" + bt.CellType;
                    AlgoWrapper.Instance.ImageProcessInspect(ref _workingSeats._seat2.Corner3.InspectParams, ref bt.Corner3.ShotImage, ref bt.Corner3.InspectResults, algoMode, bt.Corner3.ImgNO);
                }
                else
                {
                    if (CheckParamsConfig.Instance.CellType == "")
                    {
                        _workingSeats._seat1.Corner3.InspectParams.strBarcode = bt.Sn;
                    }
                    else
                    {
                        _workingSeats._seat1.Corner3.InspectParams.strBarcode = bt.Sn + "_" + CheckParamsConfig.Instance.CellType;
                    }
                    //WriteLog(string.Format("{0}", _workingSeats._seat1.Corner3.ToString()), LogLevels.Info, "TestCorner3");
                    AlgoWrapper.Instance.ImageProcessInspect(ref _workingSeats._seat1.Corner3.InspectParams, ref bt.Corner3.ShotImage, ref bt.Corner3.InspectResults, algoMode, bt.Corner3.ImgNO);
                }
                WriteLog(string.Format("第{0}次 3角 算法检测结果为 {1} 耗时 {2} 毫秒", time, bt.Corner3.InspectResults.resultDataMin.iResult, Environment.TickCount - tickCount), LogLevels.Info, "TestCorner3");

            }
            catch (Exception ex)
            {
                string picname = "D:\\Test\\3_" + bt.Sn + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + "算法报错" + ".jpg";
                bt.Corner3.ShotImage.Save(picname, Consts.ImageTypes.Sixteen);

                WriteLog(string.Format("第{0}次 进入角3算法线程异常: {1}", time, ex.Message.ToString()), LogLevels.Error, "TestCorner3");
                if (ex.Message.ToString().Contains("外部组件发生异常"))
                {
                    MessageBox.Show("算法外部组件发生异常,请重启软件！", "提示", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Warning,
                        System.Windows.Forms.MessageBoxDefaultButton.Button1, System.Windows.Forms.MessageBoxOptions.ServiceNotification);
                    StopTest.Execute(null);
                }
            }
            m_bAlgoCorner3Finished = true;
        }

        private void TestCorner4(ref BatterySeat bt, int time)
        {
            try
            {
                int tickCount = Environment.TickCount;
                WriteLog(string.Format("第{0}次 进入 算法角4线程,是 {1} 面电池", time, bt.CellType), LogLevels.Info, "TestCorner4");
                if (bt.CellType == "A")
                {
                    _workingSeats._seat1.Corner4.InspectParams.strBarcode = bt.Sn + "_" + bt.CellType;
                    //WriteLog(string.Format("{0}", _workingSeats._seat1.Corner4.ToString()), LogLevels.Info, "TestCorner4");
                    AlgoWrapper.Instance.ImageProcessInspect(ref _workingSeats._seat1.Corner4.InspectParams, ref bt.Corner4.ShotImage, ref bt.Corner4.InspectResults, algoMode, bt.Corner4.ImgNO);
                }
                else if (bt.CellType == "B")
                {
                    _workingSeats._seat2.Corner4.InspectParams.strBarcode = bt.Sn + "_" + bt.CellType;
                    AlgoWrapper.Instance.ImageProcessInspect(ref _workingSeats._seat2.Corner4.InspectParams, ref bt.Corner4.ShotImage, ref bt.Corner4.InspectResults, algoMode, bt.Corner4.ImgNO);
                }
                else
                {
                    if (CheckParamsConfig.Instance.CellType == "")
                    {
                        _workingSeats._seat1.Corner4.InspectParams.strBarcode = bt.Sn;
                    }
                    else
                    {
                        _workingSeats._seat1.Corner4.InspectParams.strBarcode = bt.Sn + "_" + CheckParamsConfig.Instance.CellType;
                    }
                    //WriteLog(string.Format("{0}", _workingSeats._seat1.Corner4.ToString()), LogLevels.Info, "TestCorner4");
                    AlgoWrapper.Instance.ImageProcessInspect(ref _workingSeats._seat1.Corner4.InspectParams, ref bt.Corner4.ShotImage, ref bt.Corner4.InspectResults, algoMode, bt.Corner4.ImgNO);
                }
                WriteLog(string.Format("第{0}次 4角 算法检测结果为 {1} 耗时 {2} 毫秒", time, bt.Corner4.InspectResults.resultDataMin.iResult, Environment.TickCount - tickCount), LogLevels.Info, "TestCorner4");

            }
            catch (Exception ex)
            {
                string picname = "D:\\Test\\4_" + bt.Sn + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + "算法报错" + ".jpg";
                bt.Corner4.ShotImage.Save(picname, Consts.ImageTypes.Sixteen);

                WriteLog(string.Format("第{0}次 进入角4算法线程异常: {1}", time, ex.Message.ToString()), LogLevels.Error, "TestCorner4");
                if (ex.Message.ToString().Contains("外部组件发生异常"))
                {
                    MessageBox.Show("算法外部组件发生异常,请重启软件！", "提示", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Warning,
                        System.Windows.Forms.MessageBoxDefaultButton.Button1, System.Windows.Forms.MessageBoxOptions.ServiceNotification);
                    StopTest.Execute(null);
                }
            }
            m_bAlgoCorner4Finished = true;
        }

        private void TestCorner1(ref BatterySeat bt, int time)
        {
            try
            {
                int tickCount = Environment.TickCount;
                WriteLog(string.Format("第{0}次 进入 算法角1线程,是 {1} 面电池", time, bt.CellType), LogLevels.Info, "TestCorner1");

                if (bt.CellType == "A")
                {
                    _workingSeats._seat1.Corner1.InspectParams.strBarcode = bt.Sn + "_" + bt.CellType;
                    //WriteLog(string.Format("{0}", _workingSeats._seat1.Corner1.ToString()), LogLevels.Info, "TestCorner1");
                    AlgoWrapper.Instance.ImageProcessInspect(ref _workingSeats._seat1.Corner1.InspectParams, ref bt.Corner1.ShotImage, ref bt.Corner1.InspectResults, algoMode, bt.Corner1.ImgNO);
                }
                else if (bt.CellType == "B")
                {
                    _workingSeats._seat2.Corner1.InspectParams.strBarcode = bt.Sn + "_" + bt.CellType;
                    AlgoWrapper.Instance.ImageProcessInspect(ref _workingSeats._seat2.Corner1.InspectParams, ref bt.Corner1.ShotImage, ref bt.Corner1.InspectResults, algoMode, bt.Corner1.ImgNO);
                }
                else
                {
                    if (CheckParamsConfig.Instance.CellType == "")
                    {
                        _workingSeats._seat1.Corner1.InspectParams.strBarcode = bt.Sn;
                    }
                    else
                    {
                        _workingSeats._seat1.Corner1.InspectParams.strBarcode = bt.Sn + "_" + CheckParamsConfig.Instance.CellType;
                    }
                    //WriteLog(string.Format("{0}", _workingSeats._seat1.Corner1.ToString()), LogLevels.Info, "TestCorner1");
                    AlgoWrapper.Instance.ImageProcessInspect(ref _workingSeats._seat1.Corner1.InspectParams, ref bt.Corner1.ShotImage, ref bt.Corner1.InspectResults, algoMode, bt.Corner1.ImgNO);
                }
                WriteLog(string.Format("第{0}次 1角 算法检测结果为 {1} 耗时 {2} 毫秒", time, bt.Corner1.InspectResults.resultDataMin.iResult, Environment.TickCount - tickCount), LogLevels.Info, "TestCorner1");

            }
            catch (Exception ex)
            {
                string picname = "D:\\Test\\1_" + bt.Sn + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + "算法报错" + ".jpg";
                bt.Corner1.ShotImage.Save(picname, Consts.ImageTypes.Sixteen);

                WriteLog(string.Format("第{0}次 进入角1算法线程异常: {1}", time, ex.Message.ToString()), LogLevels.Error, "TestCorner1");
                if (ex.Message.ToString().Contains("外部组件发生异常"))
                {
                    MessageBox.Show("算法外部组件发生异常,请重启软件！", "提示", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Warning,
                        System.Windows.Forms.MessageBoxDefaultButton.Button1, System.Windows.Forms.MessageBoxOptions.ServiceNotification);
                    StopTest.Execute(null);
                }
            }
            m_bAlgoCorner1Finished = true;
        }

        private void TestCorner2(ref BatterySeat bt, int time)
        {
            try
            {
                int tickCount = Environment.TickCount;
                WriteLog(string.Format("第{0}次 进入 算法角2线程,是 {1} 面电池", time, bt.CellType), LogLevels.Info, "TestCorner2");
                if (bt.CellType == "A")
                {
                    _workingSeats._seat1.Corner2.InspectParams.strBarcode = bt.Sn + "_" + bt.CellType;
                    //WriteLog(string.Format("{0}", _workingSeats._seat1.Corner2.ToString()), LogLevels.Info, "TestCorner2");
                    AlgoWrapper.Instance.ImageProcessInspect(ref _workingSeats._seat1.Corner2.InspectParams, ref bt.Corner2.ShotImage, ref bt.Corner2.InspectResults, algoMode, bt.Corner2.ImgNO);
                }
                else if (bt.CellType == "B")
                {
                    _workingSeats._seat2.Corner2.InspectParams.strBarcode = bt.Sn + "_" + bt.CellType;
                    AlgoWrapper.Instance.ImageProcessInspect(ref _workingSeats._seat2.Corner2.InspectParams, ref bt.Corner2.ShotImage, ref bt.Corner2.InspectResults, algoMode, bt.Corner2.ImgNO);
                }
                else
                {
                    if (CheckParamsConfig.Instance.CellType == "")
                    {
                        _workingSeats._seat1.Corner2.InspectParams.strBarcode = bt.Sn;
                    }
                    else
                    {
                        _workingSeats._seat1.Corner2.InspectParams.strBarcode = bt.Sn + "_" + CheckParamsConfig.Instance.CellType;
                    }
                    //WriteLog(string.Format("{0}", _workingSeats._seat1.Corner2.ToString()), LogLevels.Info, "TestCorner2");
                    AlgoWrapper.Instance.ImageProcessInspect(ref _workingSeats._seat1.Corner2.InspectParams, ref bt.Corner2.ShotImage, ref bt.Corner2.InspectResults, algoMode, bt.Corner2.ImgNO);
                }

                WriteLog(string.Format("第{0}次 2角 算法检测结果为 {1} 耗时 {2} 毫秒", time, bt.Corner2.InspectResults.resultDataMin.iResult, Environment.TickCount - tickCount), LogLevels.Info, "TestCorner2");

            }
            catch (Exception ex)
            {
                string picname = "D:\\Test\\2_" + bt.Sn + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + "算法报错" + ".jpg";
                bt.Corner2.ShotImage.Save(picname, Consts.ImageTypes.Sixteen);

                WriteLog(string.Format("第{0}次 进入角2算法线程异常: {1}", time, ex.Message.ToString()), LogLevels.Error, "TestCorner2");
                if (ex.Message.ToString().Contains("外部组件发生异常"))
                {
                    MessageBox.Show("算法外部组件发生异常,请重启软件！", "提示", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Warning,
                        System.Windows.Forms.MessageBoxDefaultButton.Button1, System.Windows.Forms.MessageBoxOptions.ServiceNotification);
                    StopTest.Execute(null);
                }
            }
            m_bAlgoCorner2Finished = true;
        }

        private static List<string> listOCVGRRBarcode = new List<string>();

        private void ScanBarcodeGRR()
        {
            WriteLog("扫码GGR线程开启", LogLevels.Info, "ScanBarcodeGRR");
            MotionEnum.EnumScanGrr = MotionEnum.ScanGRR.获取条码;
            while (true)
            {
                Thread.Sleep(10);
                if (plc.ReadByVariableName("SoftwareReset") == "1")
                {
                    //PLC复位
                    isScanGRRFinish = true;
                    return;
                }

                switch (MotionEnum.EnumScanGrr)
                {
                    case MotionEnum.ScanGRR.获取条码:

                        if (plc.ReadByVariableName("ScanBarcodeSignal1") == "1" && plc.ReadByVariableName("ScanBarcodeSignal2") == "1")
                        {
                            scanGrrTime += 2;
                            //IV做GRR时会抓回来重复扫码，这时候不扫码直接给完成信号
                            if (plc.ReadByVariableName("TestMode") == "2")
                            {
                                if (scanGrrTime != 2 && scanGrrTime != 4 && scanGrrTime != 50 && scanGrrTime != 52 &&
                                    scanGrrTime != 98 && scanGrrTime != 100 && scanGrrTime != 146 && scanGrrTime != 148)
                                {
                                    WriteLog(string.Format("第{0}次IVGRR不扫码", scanGrrTime), LogLevels.Info, "ScanBarcodeGRR");
                                    plc.WriteByVariableName("BatteryScanning1OK", 1);
                                    plc.WriteByVariableName("BatteryScanning2OK", 1);
                                    MotionEnum.EnumScanGrr = MotionEnum.ScanGRR.扫码完成;
                                    continue;
                                }
                            }
                            if (plc.ReadByVariableName("TestMode") == "3" && scanGrrTime > 12)//OCVGRR模式只扫前面12个码
                            {
                                WriteLog(string.Format("第{0}次OCVGRR不扫码", scanGrrTime), LogLevels.Info, "ScanBarcodeGRR");
                                plc.WriteByVariableName("BatteryScanning1OK", 1);
                                plc.WriteByVariableName("BatteryScanning2OK", 1);
                                MotionEnum.EnumScanGrr = MotionEnum.ScanGRR.扫码完成;
                                continue;
                            }
                            string barcode = string.Empty;

                            CodeReaderIF.ClientSendMsg("LON\r", 1);
                            Thread.Sleep(200);
                            CodeReaderIF.ClientSendMsg("LOFF\r", 1);
                            barcode = CodeReaderIF.ClientReceiveMsg(1);
                            if (plc.ReadByVariableName("TestMode") == "3")
                            {
                                listOCVGRRBarcode.Add(barcode + "_" + (scanGrrTime - 1));
                            }
                            else
                            {
                                queueGRRBarcode.Enqueue(barcode + "_" + (scanGrrTime - 1));
                                queueGRRBarMdiCode.Enqueue(barcode);
                            }
                            WriteLog(string.Format("第{0}次 {1} 条码为{2}", scanGrrTime - 1, Enum.GetName(typeof(MotionEnum.ScanGRR), MotionEnum.EnumScanGrr), barcode), LogLevels.Info, "ScanBarcodeGRR");

                            barcode = "";
                            CodeReaderIF.ClientSendMsg("LON\r", 3);
                            Thread.Sleep(200);
                            CodeReaderIF.ClientSendMsg("LOFF\r", 3);
                            barcode = CodeReaderIF.ClientReceiveMsg(3);
                            if (plc.ReadByVariableName("TestMode") == "3")
                            {
                                listOCVGRRBarcode.Add(barcode + "_" + scanGrrTime);
                            }
                            else
                            {
                                queueGRRBarcode.Enqueue(barcode + "_" + scanGrrTime);
                                queueGRRBarMdiCode.Enqueue(barcode);
                            }

                            WriteLog(string.Format("第{0}次 {1} 条码为{2}", scanGrrTime, Enum.GetName(typeof(MotionEnum.ScanGRR), MotionEnum.EnumScanGrr), barcode), LogLevels.Info, "ScanBarcodeGRR");
                            plc.WriteByVariableName("BatteryScanning1OK", 1);
                            plc.WriteByVariableName("BatteryScanning2OK", 1);
                            MotionEnum.EnumScanGrr = MotionEnum.ScanGRR.扫码完成;
                        }

                        break;

                    case MotionEnum.ScanGRR.扫码完成:

                        if (plc.ReadByVariableName("ScanBarcodeSignal1") == "0" && plc.ReadByVariableName("ScanBarcodeSignal2") == "0")
                        {
                            WriteLog(string.Format("第{0}次 {1}", scanGrrTime, Enum.GetName(typeof(MotionEnum.ScanGRR), MotionEnum.EnumScanGrr)), LogLevels.Info, "ScanBarcodeGRR");
                            if (plc.ReadByVariableName("TestMode") == "3" && scanGrrTime == 12) //OCVGRR模式只有12个码，后面拼接凑够12*7个码，共96个
                            {
                                List<string> listBarcode = new List<string>();
                                listBarcode = listOCVGRRBarcode.ToList();
                                for (int i = 0; i < 7; i++)
                                {
                                    listOCVGRRBarcode.AddRange(listBarcode);
                                }
                            }
                            plc.WriteByVariableName("BatteryScanning1OK", 0);
                            plc.WriteByVariableName("BatteryScanning2OK", 0);
                            plc.WriteByVariableName("BatteryScanning1NG", 0);
                            plc.WriteByVariableName("BatteryScanning2NG", 0);
                            isScanGRRFinish = true;
                            MotionEnum.EnumScanGrr = MotionEnum.ScanGRR.获取条码;
                            return;
                        }
                        break;
                }
            }

        }

        private void SingleScanBarcodeGRR()
        {
            WriteLog("单扫码枪扫码GGR线程开启", LogLevels.Info, "SingleScanBarcodeGRR");
            MotionEnum.EnumScanGRRSingleScaner = MotionEnum.ScanGRRSingleScaner.获取条码1;
            while (true)
            {
                Thread.Sleep(10);
                if (plc.ReadByVariableName("SoftwareReset") == "1")
                {
                    //PLC复位
                    isScanGRRFinish = true;
                    return;
                }

                switch (MotionEnum.EnumScanGRRSingleScaner)
                {
                    case MotionEnum.ScanGRRSingleScaner.获取条码1:

                        if (plc.ReadByVariableName("ScanBarcodeSignal2") == "1")
                        {
                            scanGrrTime += 2;

                            //if (plc.ReadByVariableName("TestMode") == "2")//IV做GRR时会抓回来重复扫码，这时候不扫码直接给完成信号
                            //{
                            //    if (scanGrrTime != 2 && scanGrrTime != 4 && scanGrrTime != 50 && scanGrrTime != 52 &&
                            //        scanGrrTime != 98 && scanGrrTime != 100 && scanGrrTime != 146 && scanGrrTime != 148)
                            //    {
                            //        WriteLog(string.Format("第{0}次IVGRR不扫码", (scanGrrTime - 1)), LogLevels.Info, "SingleScanBarcodeGRR");
                            //        plc.WriteByVariableName("BatteryScanning2OK", 1);
                            //        MotionEnum.EnumScanGRRSingleScaner = MotionEnum.ScanGRRSingleScaner.扫码完成1;
                            //        continue;
                            //    }
                            //}
                            //if (plc.ReadByVariableName("TestMode") == "3" && scanGrrTime > 12)//OCVGRR模式只扫前面12个码
                            //{
                            //    WriteLog(string.Format("第{0}次OCVGRR不扫码", (scanGrrTime - 1)), LogLevels.Info, "SingleScanBarcodeGRR");
                            //    plc.WriteByVariableName("BatteryScanning2OK", 1);
                            //    MotionEnum.EnumScanGRRSingleScaner = MotionEnum.ScanGRRSingleScaner.扫码完成1;
                            //    continue;
                            //}

                            string barcode = "";
                            CodeReaderIF.ClientSendMsg("LON\r", 3);
                            Thread.Sleep(200);
                            CodeReaderIF.ClientSendMsg("LOFF\r", 3);
                            barcode = CodeReaderIF.ClientReceiveMsg(3);
                            if (plc.ReadByVariableName("TestMode") == "3")
                            {
                                listOCVGRRBarcode.Add(barcode + "_" + (scanGrrTime - 1));
                            }
                            else
                            {
                                queueGRRBarcode.Enqueue(barcode + "_" + (scanGrrTime - 1));
                                queueGRRBarMdiCode.Enqueue(barcode);
                            }

                            WriteLog(string.Format("第{0}次 {1} 条码为{2}", scanGrrTime - 1, Enum.GetName(typeof(MotionEnum.ScanGRRSingleScaner), MotionEnum.EnumScanGRRSingleScaner), barcode), LogLevels.Info, "SingleScanBarcodeGRR");
                            plc.WriteByVariableName("BatteryScanning2OK", 1);
                            MotionEnum.EnumScanGRRSingleScaner = MotionEnum.ScanGRRSingleScaner.扫码完成1;
                        }

                        break;

                    case MotionEnum.ScanGRRSingleScaner.扫码完成1:

                        if (plc.ReadByVariableName("ScanBarcodeSignal2") == "0")
                        {
                            WriteLog(string.Format("第{0}次 {1}", scanGrrTime - 1, Enum.GetName(typeof(MotionEnum.ScanGRRSingleScaner), MotionEnum.EnumScanGRRSingleScaner)), LogLevels.Info, "SingleScanBarcodeGRR");
                            plc.WriteByVariableName("BatteryScanning2OK", 0);
                            plc.WriteByVariableName("BatteryScanning2NG", 0);
                            MotionEnum.EnumScanGRRSingleScaner = MotionEnum.ScanGRRSingleScaner.获取条码2;
                        }
                        break;

                    case MotionEnum.ScanGRRSingleScaner.获取条码2:

                        if (plc.ReadByVariableName("ScanBarcodeSignal2") == "1")
                        {

                            //if (plc.ReadByVariableName("TestMode") == "2")//IV做GRR时会抓回来重复扫码，这时候不扫码直接给完成信号
                            //{
                            //    if (scanGrrTime != 2 && scanGrrTime != 4 && scanGrrTime != 50 && scanGrrTime != 52 &&
                            //        scanGrrTime != 98 && scanGrrTime != 100 && scanGrrTime != 146 && scanGrrTime != 148)
                            //    {
                            //        WriteLog(string.Format("第{0}次IVGRR不扫码", scanGrrTime), LogLevels.Info, "SingleScanBarcodeGRR");
                            //        plc.WriteByVariableName("BatteryScanning2OK", 1);
                            //        MotionEnum.EnumScanGRRSingleScaner = MotionEnum.ScanGRRSingleScaner.扫码完成2;
                            //        continue;
                            //    }
                            //}
                            //if (plc.ReadByVariableName("TestMode") == "3" && scanGrrTime > 12)//OCVGRR模式只扫前面12个码
                            //{
                            //    WriteLog(string.Format("第{0}次OCVGRR不扫码", scanGrrTime), LogLevels.Info, "SingleScanBarcodeGRR");
                            //    plc.WriteByVariableName("BatteryScanning2OK", 1);
                            //    MotionEnum.EnumScanGRRSingleScaner = MotionEnum.ScanGRRSingleScaner.扫码完成2;
                            //    continue;
                            //}

                            string barcode = "";
                            CodeReaderIF.ClientSendMsg("LON\r", 3);
                            Thread.Sleep(200);
                            CodeReaderIF.ClientSendMsg("LOFF\r", 3);
                            barcode = CodeReaderIF.ClientReceiveMsg(3);
                            if (plc.ReadByVariableName("TestMode") == "3")
                            {
                                listOCVGRRBarcode.Add(barcode + "_" + scanGrrTime);
                            }
                            else
                            {
                                queueGRRBarcode.Enqueue(barcode + "_" + scanGrrTime);
                                queueGRRBarMdiCode.Enqueue(barcode);
                            }

                            WriteLog(string.Format("第{0}次 {1} 条码为{2}", scanGrrTime, Enum.GetName(typeof(MotionEnum.ScanGRRSingleScaner), MotionEnum.EnumScanGRRSingleScaner), barcode), LogLevels.Info, "SingleScanBarcodeGRR");
                            plc.WriteByVariableName("BatteryScanning2OK", 1);
                            MotionEnum.EnumScanGRRSingleScaner = MotionEnum.ScanGRRSingleScaner.扫码完成2;

                        }
                        break;

                    case MotionEnum.ScanGRRSingleScaner.扫码完成2:

                        if (plc.ReadByVariableName("ScanBarcodeSignal2") == "0")
                        {
                            WriteLog(string.Format("第{0}次 {1}", scanGrrTime, Enum.GetName(typeof(MotionEnum.ScanGRRSingleScaner), MotionEnum.EnumScanGRRSingleScaner)), LogLevels.Info, "SingleScanBarcodeGRR");
                            if (plc.ReadByVariableName("TestMode") == "3" && scanGrrTime == 12) //OCVGRR模式只有12个码，后面拼接凑够12*7个码，共96个
                            {
                                List<string> listBarcode = new List<string>();
                                listBarcode = listOCVGRRBarcode.ToList();
                                for (int i = 0; i < 7; i++)
                                {
                                    listOCVGRRBarcode.AddRange(listBarcode);
                                }
                            }
                            plc.WriteByVariableName("BatteryScanning2OK", 0);
                            plc.WriteByVariableName("BatteryScanning2NG", 0);
                            isScanGRRFinish = true;
                            MotionEnum.EnumScanGRRSingleScaner = MotionEnum.ScanGRRSingleScaner.获取条码1;
                            return;
                        }
                        break;

                }
            }

        }

        /// <summary>
        /// IV和OCV做完GRR清料定位
        /// </summary>
        private void Task_GrrMdiPosition()
        {
            int step = 0;
            int position = 0;
            while (true)
            {
                Thread.Sleep(50);
                if (plc.ReadByVariableName("SoftwareReset") == "1")
                {
                    step = 0;
                }
                if (_AutoRunning == false)
                {
                    return;
                }

                switch (step)
                {
                    case 0:

                        if (plc.ReadByVariableName("TestMode") == "2" || plc.ReadByVariableName("TestMode") == "3")
                        {
                            if (plc.ReadByVariableName("MDISignal1") == "1" || plc.ReadByVariableName("MDISignal2") == "1" || plc.ReadByVariableName("MDISignal3") == "1" || plc.ReadByVariableName("MDISignal4") == "1")
                            {
                                if (plc.ReadByVariableName("MDISignal1") == "1")
                                {
                                    position = 1;
                                }
                                else if (plc.ReadByVariableName("MDISignal2") == "1")
                                {
                                    position = 2;
                                }
                                else if (plc.ReadByVariableName("MDISignal3") == "1")
                                {
                                    position = 3;
                                }
                                else if (plc.ReadByVariableName("MDISignal4") == "1")
                                {
                                    position = 4;
                                }

                                BatterySeat bt = new BatterySeat();
                                GetMdiData(ref bt, 0, position, true);
                                step = 1;
                                if (position == 1)
                                {
                                    plc.WriteByVariableName("MDIComplete1", 1);
                                }
                                else if (position == 2)
                                {
                                    plc.WriteByVariableName("MDIComplete2", 1);
                                }
                                else if (position == 3)
                                {
                                    plc.WriteByVariableName("MDIComplete1", 1);
                                }
                                else if (position == 4)
                                {
                                    plc.WriteByVariableName("MDIComplete2", 1);
                                }
                            }
                        }

                        break;

                    case 1:

                        if (plc.ReadByVariableName("MDISignal1") == "0" && plc.ReadByVariableName("MDISignal2") == "0" && plc.ReadByVariableName("MDISignal3") == "0" && plc.ReadByVariableName("MDISignal4") == "0")
                        {
                            plc.WriteByVariableName("MDIComplete1", 0);
                            plc.WriteByVariableName("MDIComplete2", 0);
                            step = 0;
                            position = 0;
                        }
                        break;

                }

            }
        }

        private void MDIAndPPGGRR()
        {
            int testCount = 0;//次数，一般一组电池要重复测3次
            int testGroup = 0;//组数，这里交换3次电池位置，共测4组
            int testPPGCount = 0;//次数，一般一组电池要重复测3次
            int testPPGGroup = 0;//组数，这里交换3次电池位置，共测4组

            bool isOneGroupFinish = false;
            string step = "";
            string stepPPG = "";

            WriteLog("FQI和PPGGRR线程开启", LogLevels.Info, "FQIAndPPGGRR");
            MotionEnum.EnumMdiAndPpggrr = MotionEnum.MDIAndPPGGRR.准备拍照;
            MotionEnum.EnumPpgGrr = MotionEnum.PPGGRR.准备测厚;

            BatterySeat bt1 = new BatterySeat();
            BatterySeat bt2 = new BatterySeat();
            BatterySeat bt3 = new BatterySeat();
            BatterySeat bt4 = new BatterySeat();
            BatterySeat ppgbt1 = new BatterySeat();
            BatterySeat ppgbt2 = new BatterySeat();
            BatterySeat ppgbt3 = new BatterySeat();
            BatterySeat ppgbt4 = new BatterySeat();

            string barcode1 = queueGRRBarcode.Dequeue();
            string barcode2 = queueGRRBarcode.Dequeue();
            string barcode3 = queueGRRBarcode.Dequeue();
            string barcode4 = queueGRRBarcode.Dequeue();

            bt1.Sn = barcode1;
            bt2.Sn = barcode2;
            bt3.Sn = barcode3;
            bt4.Sn = barcode4;

            ppgbt1.Sn = barcode1;
            ppgbt2.Sn = barcode2;
            ppgbt3.Sn = barcode3;
            ppgbt4.Sn = barcode4;

            bool isReverce = false;
            bool isPPGReverce = false;

            while (true)
            {
                step = Enum.GetName(typeof(MotionEnum.MDIAndPPGGRR), MotionEnum.EnumMdiAndPpggrr);
                stepPPG = Enum.GetName(typeof(MotionEnum.PPGGRR), MotionEnum.EnumPpgGrr);

                if (plc.ReadByVariableName("SoftwareReset") == "1" || _AutoRunning == false)
                {
                    bt1.Destroy();
                    bt2.Destroy();
                    bt3.Destroy();
                    bt4.Destroy();
                    isMDIAndPPGGRRFinish = true;
                    return;
                }

                switch (MotionEnum.EnumMdiAndPpggrr)
                {
                    case MotionEnum.MDIAndPPGGRR.准备拍照:

                        testCount++;
                        if (testGroup % 4 == 1)
                        {
                            bt1.Sn = barcode2;
                            bt2.Sn = barcode1;
                            bt3.Sn = barcode4;
                            bt4.Sn = barcode3;
                        }
                        else if (testGroup % 4 == 2)
                        {
                            bt1.Sn = barcode4;
                            bt2.Sn = barcode3;
                            bt3.Sn = barcode2;
                            bt4.Sn = barcode1;
                        }
                        else if (testGroup % 4 == 3)
                        {
                            bt1.Sn = barcode3;
                            bt2.Sn = barcode4;
                            bt3.Sn = barcode1;
                            bt4.Sn = barcode2;
                        }
                        bt1.MDIChannel = 1;
                        bt2.MDIChannel = 2;
                        bt3.MDIChannel = 3;
                        bt4.MDIChannel = 4;
                        WriteLog(string.Format("第{0}次 {1} ", testCount, step), LogLevels.Info, "FQIAndPPGGRR");

                        if (isReverce == true)
                        {
                            MotionEnum.EnumMdiAndPpggrr = MotionEnum.MDIAndPPGGRR.工位3触发拍照;
                        }
                        else
                        {
                            MotionEnum.EnumMdiAndPpggrr = MotionEnum.MDIAndPPGGRR.工位2触发拍照;
                        }
                        break;

                    case MotionEnum.MDIAndPPGGRR.工位2触发拍照:

                        if (plc.ReadByVariableName("MDISignal2") == "1")
                        {
                            GetMdiData(ref bt2, testCount, 2, true);
                            plc.WriteByVariableName("MDIComplete2", 1);
                            MotionEnum.EnumMdiAndPpggrr = MotionEnum.MDIAndPPGGRR.工位2拍照完成;
                        }
                        break;

                    case MotionEnum.MDIAndPPGGRR.工位2拍照完成:

                        if (plc.ReadByVariableName("MDISignal2") == "0")
                        {
                            WriteLog(string.Format("第{0}次 {1} ", testCount, step), LogLevels.Info, "FQIAndPPGGRR");
                            plc.WriteByVariableName("MDIComplete2", 0);
                            MotionEnum.EnumMdiAndPpggrr = MotionEnum.MDIAndPPGGRR.工位1触发拍照;

                        }
                        break;

                    case MotionEnum.MDIAndPPGGRR.工位1触发拍照:

                        if (plc.ReadByVariableName("MDISignal1") == "1")
                        {
                            WriteLog(string.Format("第{0}次 {1} ", testCount, step), LogLevels.Info, "FQIAndPPGGRR");
                            GetMdiData(ref bt1, testCount, 1, true);
                            plc.WriteByVariableName("MDIComplete1", 1);
                            MotionEnum.EnumMdiAndPpggrr = MotionEnum.MDIAndPPGGRR.工位1拍照完成;
                        }

                        break;

                    case MotionEnum.MDIAndPPGGRR.工位1拍照完成:

                        if (plc.ReadByVariableName("MDISignal1") == "0")
                        {
                            WriteLog(string.Format("第{0}次 {1} ", testCount, step), LogLevels.Info, "FQIAndPPGGRR");
                            plc.WriteByVariableName("MDIComplete1", 0);
                            if (isReverce == true)
                            {
                                MotionEnum.EnumMdiAndPpggrr = MotionEnum.MDIAndPPGGRR.一组FQIGGR完成;
                            }
                            else
                            {
                                MotionEnum.EnumMdiAndPpggrr = MotionEnum.MDIAndPPGGRR.工位3触发拍照;
                            }
                        }
                        break;

                    case MotionEnum.MDIAndPPGGRR.工位3触发拍照:

                        if (plc.ReadByVariableName("MDISignal3") == "1")
                        {
                            WriteLog(string.Format("第{0}次 {1} ", testCount, step), LogLevels.Info, "FQIAndPPGGRR");
                            GetMdiData(ref bt3, testCount, 3, true);
                            plc.WriteByVariableName("MDIComplete1", 1);
                            MotionEnum.EnumMdiAndPpggrr = MotionEnum.MDIAndPPGGRR.工位3拍照完成;
                        }

                        break;

                    case MotionEnum.MDIAndPPGGRR.工位3拍照完成:

                        if (plc.ReadByVariableName("MDISignal3") == "0")
                        {
                            WriteLog(string.Format("第{0}次 {1} ", testCount, step), LogLevels.Info, "FQIAndPPGGRR");
                            plc.WriteByVariableName("MDIComplete1", 0);
                            MotionEnum.EnumMdiAndPpggrr = MotionEnum.MDIAndPPGGRR.工位4触发拍照;
                        }
                        break;

                    case MotionEnum.MDIAndPPGGRR.工位4触发拍照:

                        if (plc.ReadByVariableName("MDISignal4") == "1")
                        {
                            WriteLog(string.Format("第{0}次 {1} ", testCount, step), LogLevels.Info, "FQIAndPPGGRR");
                            GetMdiData(ref bt4, testCount, 4, true);
                            plc.WriteByVariableName("MDIComplete2", 1);
                            MotionEnum.EnumMdiAndPpggrr = MotionEnum.MDIAndPPGGRR.工位4拍照完成;
                        }
                        break;

                    case MotionEnum.MDIAndPPGGRR.工位4拍照完成:

                        if (plc.ReadByVariableName("MDISignal4") == "0")
                        {
                            WriteLog(string.Format("第{0}次 {1} ", testCount, step), LogLevels.Info, "FQIAndPPGGRR");
                            plc.WriteByVariableName("MDIComplete2", 0);
                            if (isReverce == true)
                            {
                                MotionEnum.EnumMdiAndPpggrr = MotionEnum.MDIAndPPGGRR.工位2触发拍照;
                            }
                            else
                            {
                                MotionEnum.EnumMdiAndPpggrr = MotionEnum.MDIAndPPGGRR.一组FQIGGR完成;
                            }
                        }
                        break;

                    case MotionEnum.MDIAndPPGGRR.一组FQIGGR完成:

                        if (testCount % 3 == 0)
                        {
                            testGroup++;
                        }
                        if (testCount == 6)
                        {
                            isReverce = true;
                        }
                        else
                        {
                            isReverce = false;
                        }
                        if (testGroup == 4)
                        {
                            //bt1.Destroy();
                            //bt2.Destroy();
                            //bt3.Destroy();
                            //bt4.Destroy();
                            //WriteLog("MDI和PPGGRR线程退出", LogLevels.Info, "MDIAndPPGGRR");
                            //isMDIAndPPGGRRFinish = true;
                            //return;
                        }

                        MotionEnum.EnumMdiAndPpggrr = MotionEnum.MDIAndPPGGRR.准备拍照;
                        break;
                }

                switch (MotionEnum.EnumPpgGrr)
                {
                    case MotionEnum.PPGGRR.准备测厚:

                        testPPGCount++;
                        WriteLog(string.Format("第{0}次 {1} ", testPPGCount, stepPPG), LogLevels.Info, "FQIAndPPGGRR");

                        if (testPPGGroup % 4 == 1)
                        {
                            ppgbt1.Sn = barcode2;
                            ppgbt2.Sn = barcode1;
                            ppgbt3.Sn = barcode4;
                            ppgbt4.Sn = barcode3;
                        }
                        else if (testPPGGroup % 4 == 2)
                        {
                            ppgbt1.Sn = barcode4;
                            ppgbt2.Sn = barcode3;
                            ppgbt3.Sn = barcode2;
                            ppgbt4.Sn = barcode1;
                        }
                        else if (testPPGGroup % 4 == 3)
                        {
                            ppgbt1.Sn = barcode3;
                            ppgbt2.Sn = barcode4;
                            ppgbt3.Sn = barcode1;
                            ppgbt4.Sn = barcode2;
                        }
                        ppgbt1.PPGChannel = 1;
                        ppgbt2.PPGChannel = 2;
                        ppgbt3.PPGChannel = 3;
                        ppgbt4.PPGChannel = 4;

                        if (isPPGReverce == true)
                        {
                            MotionEnum.EnumPpgGrr = MotionEnum.PPGGRR.工位3触发测厚;
                        }
                        else
                        {
                            MotionEnum.EnumPpgGrr = MotionEnum.PPGGRR.工位1触发测厚;
                        }

                        break;

                    case MotionEnum.PPGGRR.工位1触发测厚:

                        if (plc.ReadByVariableName("ThicknessMeasureSignal1") == "1")
                        {
                            ppgbt1.PPGChannel = 1;
                            WriteLog(string.Format("第{0}次 {1} ", testPPGCount, stepPPG), LogLevels.Info, "FQIAndPPGGRR");
                            GetTickness(testPPGCount, ref ppgbt1, 1);
                            plc.WriteByVariableName("ThicknessComplete1", 1);
                            try
                            {
                                SaveThicknessDataToFile(ppgbt1, true);
                            }
                            catch (Exception ex)
                            {
                                WriteLog(string.Format("第{0}次保存测厚数据异常 ", testPPGCount), LogLevels.Warn, "FQIAndPPGGRR");
                            }
                            MotionEnum.EnumPpgGrr = MotionEnum.PPGGRR.工位1测厚完成;
                        }
                        break;

                    case MotionEnum.PPGGRR.工位1测厚完成:

                        if (plc.ReadByVariableName("ThicknessMeasureSignal1") == "0")
                        {
                            WriteLog(string.Format("第{0}次 {1} ", testPPGCount, stepPPG), LogLevels.Info, "FQIAndPPGGRR");
                            plc.WriteByVariableName("ThicknessComplete1", 0);
                            MotionEnum.EnumPpgGrr = MotionEnum.PPGGRR.工位2触发测厚;
                        }
                        break;

                    case MotionEnum.PPGGRR.工位2触发测厚:

                        if (plc.ReadByVariableName("ThicknessMeasureSignal2") == "1")
                        {
                            ppgbt2.PPGChannel = 2;
                            WriteLog(string.Format("第{0}次 {1} ", testPPGCount, stepPPG), LogLevels.Info, "FQIAndPPGGRR");
                            GetTickness(testPPGCount, ref ppgbt2, 2);
                            plc.WriteByVariableName("ThicknessComplete2", 1);
                            try
                            {
                                SaveThicknessDataToFile(ppgbt2, true);
                            }
                            catch (Exception ex)
                            {
                                WriteLog(string.Format("第{0}次保存测厚数据异常 ", testPPGCount), LogLevels.Warn, "FQIAndPPGGRR");
                            }
                            MotionEnum.EnumPpgGrr = MotionEnum.PPGGRR.工位2测厚完成;
                        }
                        break;

                    case MotionEnum.PPGGRR.工位2测厚完成:

                        if (plc.ReadByVariableName("ThicknessMeasureSignal2") == "0")
                        {
                            WriteLog(string.Format("第{0}次 {1} ", testPPGCount, stepPPG), LogLevels.Info, "FQIAndPPGGRR");
                            plc.WriteByVariableName("ThicknessComplete2", 0);
                            if (isPPGReverce == true)
                            {
                                MotionEnum.EnumPpgGrr = MotionEnum.PPGGRR.一组PPGGGR完成;
                            }
                            else
                            {
                                MotionEnum.EnumPpgGrr = MotionEnum.PPGGRR.工位3触发测厚;
                            }
                        }

                        break;

                    case MotionEnum.PPGGRR.工位3触发测厚:

                        if (plc.ReadByVariableName("ThicknessMeasureSignal3") == "1")
                        {
                            ppgbt3.PPGChannel = 3;
                            WriteLog(string.Format("第{0}次 {1} ", testPPGCount, stepPPG), LogLevels.Info, "FQIAndPPGGRR");
                            GetTickness(testPPGCount, ref ppgbt3, 3);
                            plc.WriteByVariableName("ThicknessComplete3", 1);
                            try
                            {
                                SaveThicknessDataToFile(ppgbt3, true);
                            }
                            catch (Exception ex)
                            {
                                WriteLog(string.Format("第{0}次保存测厚数据异常 ", testPPGCount), LogLevels.Warn, "FQIAndPPGGRR");
                            }
                            MotionEnum.EnumPpgGrr = MotionEnum.PPGGRR.工位3测厚完成;
                        }
                        break;

                    case MotionEnum.PPGGRR.工位3测厚完成:

                        if (plc.ReadByVariableName("ThicknessMeasureSignal3") == "0")
                        {
                            WriteLog(string.Format("第{0}次 {1} ", testPPGCount, stepPPG), LogLevels.Info, "FQIAndPPGGRR");
                            plc.WriteByVariableName("ThicknessComplete3", 0);
                            MotionEnum.EnumPpgGrr = MotionEnum.PPGGRR.工位4触发测厚;
                        }
                        break;

                    case MotionEnum.PPGGRR.工位4触发测厚:

                        if (plc.ReadByVariableName("ThicknessMeasureSignal4") == "1")
                        {
                            ppgbt4.PPGChannel = 4;
                            WriteLog(string.Format("第{0}次 {1} ", testPPGCount, stepPPG), LogLevels.Info, "FQIAndPPGGRR");
                            GetTickness(testPPGCount, ref ppgbt4, 4);
                            plc.WriteByVariableName("ThicknessComplete4", 1);
                            try
                            {
                                SaveThicknessDataToFile(ppgbt4, true);
                            }
                            catch (Exception ex)
                            {
                                WriteLog(string.Format("第{0}次保存测厚数据异常 ", testPPGCount), LogLevels.Warn, "FQIAndPPGGRR");
                            }
                            MotionEnum.EnumPpgGrr = MotionEnum.PPGGRR.工位4测厚完成;
                        }
                        break;

                    case MotionEnum.PPGGRR.工位4测厚完成:

                        if (plc.ReadByVariableName("ThicknessMeasureSignal4") == "0")
                        {
                            WriteLog(string.Format("第{0}次 {1} ", testPPGCount, stepPPG), LogLevels.Info, "FQIAndPPGGRR");
                            plc.WriteByVariableName("ThicknessComplete4", 0);
                            if (isPPGReverce == true)
                            {
                                MotionEnum.EnumPpgGrr = MotionEnum.PPGGRR.工位1触发测厚;
                            }
                            else
                            {
                                MotionEnum.EnumPpgGrr = MotionEnum.PPGGRR.一组PPGGGR完成;
                            }
                        }
                        break;

                    case MotionEnum.PPGGRR.一组PPGGGR完成:

                        if (testPPGCount % 3 == 0)
                        {
                            testPPGGroup++;
                        }
                        if (testPPGCount == 6)
                        {
                            //isPPGReverce = true;
                        }
                        else
                        {
                            isPPGReverce = false;
                        }
                        if (testPPGGroup == 4)
                        {
                            bt1.Destroy();
                            bt2.Destroy();
                            bt3.Destroy();
                            bt4.Destroy();

                            ppgbt1.Destroy();
                            ppgbt2.Destroy();
                            ppgbt3.Destroy();
                            ppgbt4.Destroy();
                            WriteLog("FQI和PPGGRR线程退出", LogLevels.Info, "FQIAndPPGGRR");
                            isMDIAndPPGGRRFinish = true;
                            return;
                        }
                        MotionEnum.EnumPpgGrr = MotionEnum.PPGGRR.准备测厚;

                        break;

                }
            }
        }

        private void HandIVGRRdata(ref BatterySeat bt, string NeedleOKSignal, string KnifeOKSignal, string NeedleNGSignal, string KnifeNGSignal)
        {
            string[] strArr = bt.IV.Split(',');
            float[] datas = new float[6];
            for (int i = 0; i < datas.Length; i++)
            {
                try
                {
                    datas[i] = Convert.ToSingle(strArr[i]);
                }
                catch (Exception ex)
                {
                    datas[i] = 999;
                }
            }
            int result = 0;
            if (NeedleOKSignal == "1" && KnifeOKSignal == "1")
            {
                result = HandleIV(true, datas, ref bt);
            }
            else if (NeedleNGSignal == "1" || KnifeNGSignal == "1")
            {
                if (NeedleNGSignal == "1")
                {
                    bt.NeedleNoConduction = true;
                    result = HandleIV(false, datas, ref bt);
                    if (result == 2)
                    {
                        bt.NeedleNoConduction = false;
                    }
                }
                else
                {
                    bt.KnifeNoConduction = true;
                    result = HandleIV(false, datas, ref bt);
                    if (result == 2)
                    {
                        bt.KnifeNoConduction = false;
                    }
                }
            }
            bt.IvData = Math.Max(datas[4], datas[5]);

        }

        public void IVGRR()
        {
            //1组: 1、2、3、4
            //2组: 2、1、4、3
            //3组: 4、3、2、1
            //4组: 3、4、1、2
            //每组重复三次
            int time = 1;
            MotionEnum.EnumIVGRR = MotionEnum.IVGRR.获取IV数据;
            BatterySeat bt1 = new BatterySeat();
            BatterySeat bt2 = new BatterySeat();
            BatterySeat bt3 = new BatterySeat();
            BatterySeat bt4 = new BatterySeat();
            bt1.Sn = queueGRRBarcode.Dequeue();
            bt2.Sn = queueGRRBarcode.Dequeue();
            bt3.Sn = queueGRRBarcode.Dequeue();
            bt4.Sn = queueGRRBarcode.Dequeue();
            bt1.LengthBarcode = bt1.Sn;
            bt2.LengthBarcode = bt2.Sn;
            bt3.LengthBarcode = bt3.Sn;
            bt4.LengthBarcode = bt4.Sn;

            int testGroup = 0;//组数，这里交换3次电池位置，共测4组
            int testCount = 0;//次数，一般一组电池要重复测3次
            bool isOneGroupFinish = false;
            string step = "";
            WriteLog("IVGRR线程开启", LogLevels.Info, "IVGRR");
            string strData1 = "";
            string strData2 = "";
            string strData3 = "";
            string strData4 = "";

            while (true)
            {
                Thread.Sleep(10);
                if (plc.ReadByVariableName("SoftwareReset") == "1" || _AutoRunning == false)
                {
                    isIVGRRFinish = true;
                    return;
                }
                if (testGroup == 4)
                {
                    bt1.Destroy();
                    bt2.Destroy();
                    bt3.Destroy();
                    bt4.Destroy();
                    isIVGRRFinish = true;
                    return;
                }
                isOneGroupFinish = false;
                while (isOneGroupFinish == false)
                {
                    step = Enum.GetName(typeof(MotionEnum.IVGRR), MotionEnum.EnumIVGRR);
                    Thread.Sleep(10);
                    if (plc.ReadByVariableName("SoftwareReset") == "1")
                    {
                        bt1.Destroy();
                        bt2.Destroy();
                        bt3.Destroy();
                        bt4.Destroy();
                        isIVGRRFinish = true;
                        return;
                    }
                    switch (MotionEnum.EnumIVGRR)
                    {
                        case MotionEnum.IVGRR.获取IV数据:

                            if (plc.ReadByVariableName("IVTestSignal1") == "1" || plc.ReadByVariableName("IVTestSignal2") == "1" || plc.ReadByVariableName("IVTestSignal3") == "1" || plc.ReadByVariableName("IVTestSignal4") == "1")
                            {
                                testCount++;
                                WriteLog(string.Format("第{0}次 {1} ", time, step), LogLevels.Info, "IVGRR");
                                //获取IV数据
                                Lr8450.StartTest();
                                Thread.Sleep(1400);
                                strData1 = Lr8450.GetData(1);
                                strData2 = Lr8450.GetData(3);
                                strData3 = Lr8450.GetData(5);
                                strData4 = Lr8450.GetData(7);

                                WriteLog(string.Format("第{0}次IV数据： {1} | {2} | {3} | {4}", time, strData1, strData2, strData3, strData4), LogLevels.Info, "IVGRR");


                                plc.WriteByVariableName("IVComplete", 1);
                                MotionEnum.EnumIVGRR = MotionEnum.IVGRR.获取数据完成;
                            }

                            break;

                        case MotionEnum.IVGRR.获取数据完成:

                            if (plc.ReadByVariableName("IVTestSignal1") == "0" &&
                                plc.ReadByVariableName("IVTestSignal2") == "0" &&
                                plc.ReadByVariableName("IVTestSignal3") == "0" &&
                                plc.ReadByVariableName("IVTestSignal4") == "0")
                            {
                                WriteLog(string.Format("第{0}次 {1} ", time, step), LogLevels.Info, "IVGRR");
                                plc.WriteByVariableName("IVComplete", 0);
                                MotionEnum.EnumIVGRR = MotionEnum.IVGRR.处理数据;
                            }
                            break;

                        case MotionEnum.IVGRR.处理数据:

                            if (plc.ReadByVariableName("NeedleOKSignal1") == "1" ||
                                plc.ReadByVariableName("KnifeOKSignal1") == "1" ||
                                plc.ReadByVariableName("NeedleNGSignal1") == "1" ||
                                plc.ReadByVariableName("KnifeNGSignal1") == "1"
                                || plc.ReadByVariableName("NeedleOKSignal2") == "1" ||
                                plc.ReadByVariableName("KnifeOKSignal2") == "1" ||
                                plc.ReadByVariableName("NeedleNGSignal2") == "1" ||
                                plc.ReadByVariableName("KnifeNGSignal2") == "1"
                                || plc.ReadByVariableName("NeedleOKSignal3") == "1" ||
                                plc.ReadByVariableName("KnifeOKSignal3") == "1" ||
                                plc.ReadByVariableName("NeedleNGSignal3") == "1" ||
                                plc.ReadByVariableName("KnifeNGSignal3") == "1"
                                || plc.ReadByVariableName("NeedleOKSignal4") == "1" ||
                                plc.ReadByVariableName("KnifeOKSignal4") == "1" ||
                                plc.ReadByVariableName("NeedleNGSignal4") == "1" ||
                                plc.ReadByVariableName("KnifeNGSignal4") == "1")
                            {
                                WriteLog(string.Format("第{0}次 {1} ", time, step), LogLevels.Info, "IVGRR");
                                if (testGroup % 4 == 0)
                                {
                                    bt1.IV = strData1;
                                    bt2.IV = strData2;
                                    bt3.IV = strData3;
                                    bt4.IV = strData4;
                                    HandIVGRRdata(ref bt1, plc.ReadByVariableName("NeedleOKSignal1"), plc.ReadByVariableName("KnifeOKSignal1"), plc.ReadByVariableName("NeedleNGSignal1"), plc.ReadByVariableName("KnifeNGSignal1"));
                                    HandIVGRRdata(ref bt2, plc.ReadByVariableName("NeedleOKSignal2"), plc.ReadByVariableName("KnifeOKSignal2"), plc.ReadByVariableName("NeedleNGSignal2"), plc.ReadByVariableName("KnifeNGSignal2"));
                                    HandIVGRRdata(ref bt3, plc.ReadByVariableName("NeedleOKSignal3"), plc.ReadByVariableName("KnifeOKSignal3"), plc.ReadByVariableName("NeedleNGSignal3"), plc.ReadByVariableName("KnifeNGSignal3"));
                                    HandIVGRRdata(ref bt4, plc.ReadByVariableName("NeedleOKSignal4"), plc.ReadByVariableName("KnifeOKSignal4"), plc.ReadByVariableName("NeedleNGSignal4"), plc.ReadByVariableName("KnifeNGSignal4"));
                                    bt1.IVChannel = 1;
                                    bt2.IVChannel = 2;
                                    bt3.IVChannel = 3;
                                    bt4.IVChannel = 4;

                                }
                                else if (testGroup % 4 == 1)
                                {
                                    bt2.IV = strData1;
                                    bt1.IV = strData2;
                                    bt4.IV = strData3;
                                    bt3.IV = strData4;
                                    bt2.IVChannel = 1;
                                    bt1.IVChannel = 2;
                                    bt4.IVChannel = 3;
                                    bt3.IVChannel = 4;

                                    HandIVGRRdata(ref bt2, plc.ReadByVariableName("NeedleOKSignal1"), plc.ReadByVariableName("KnifeOKSignal1"), plc.ReadByVariableName("NeedleNGSignal1"), plc.ReadByVariableName("KnifeNGSignal1"));
                                    HandIVGRRdata(ref bt1, plc.ReadByVariableName("NeedleOKSignal2"), plc.ReadByVariableName("KnifeOKSignal2"), plc.ReadByVariableName("NeedleNGSignal2"), plc.ReadByVariableName("KnifeNGSignal2"));
                                    HandIVGRRdata(ref bt4, plc.ReadByVariableName("NeedleOKSignal3"), plc.ReadByVariableName("KnifeOKSignal3"), plc.ReadByVariableName("NeedleNGSignal3"), plc.ReadByVariableName("KnifeNGSignal3"));
                                    HandIVGRRdata(ref bt3, plc.ReadByVariableName("NeedleOKSignal4"), plc.ReadByVariableName("KnifeOKSignal4"), plc.ReadByVariableName("NeedleNGSignal4"), plc.ReadByVariableName("KnifeNGSignal4"));
                                }
                                else if (testGroup % 4 == 2)
                                {
                                    bt4.IV = strData1;
                                    bt3.IV = strData2;
                                    bt2.IV = strData3;
                                    bt1.IV = strData4;
                                    bt4.IVChannel = 1;
                                    bt3.IVChannel = 2;
                                    bt2.IVChannel = 3;
                                    bt1.IVChannel = 4;

                                    HandIVGRRdata(ref bt4, plc.ReadByVariableName("NeedleOKSignal1"), plc.ReadByVariableName("KnifeOKSignal1"), plc.ReadByVariableName("NeedleNGSignal1"), plc.ReadByVariableName("KnifeNGSignal1"));
                                    HandIVGRRdata(ref bt3, plc.ReadByVariableName("NeedleOKSignal2"), plc.ReadByVariableName("KnifeOKSignal2"), plc.ReadByVariableName("NeedleNGSignal2"), plc.ReadByVariableName("KnifeNGSignal2"));
                                    HandIVGRRdata(ref bt2, plc.ReadByVariableName("NeedleOKSignal3"), plc.ReadByVariableName("KnifeOKSignal3"), plc.ReadByVariableName("NeedleNGSignal3"), plc.ReadByVariableName("KnifeNGSignal3"));
                                    HandIVGRRdata(ref bt1, plc.ReadByVariableName("NeedleOKSignal4"), plc.ReadByVariableName("KnifeOKSignal4"), plc.ReadByVariableName("NeedleNGSignal4"), plc.ReadByVariableName("KnifeNGSignal4"));
                                }
                                else if (testGroup % 4 == 3)
                                {
                                    bt3.IV = strData1;
                                    bt4.IV = strData2;
                                    bt1.IV = strData3;
                                    bt2.IV = strData4;
                                    bt3.IVChannel = 1;
                                    bt4.IVChannel = 2;
                                    bt1.IVChannel = 3;
                                    bt2.IVChannel = 4;

                                    HandIVGRRdata(ref bt3, plc.ReadByVariableName("NeedleOKSignal1"), plc.ReadByVariableName("KnifeOKSignal1"), plc.ReadByVariableName("NeedleNGSignal1"), plc.ReadByVariableName("KnifeNGSignal1"));
                                    HandIVGRRdata(ref bt4, plc.ReadByVariableName("NeedleOKSignal2"), plc.ReadByVariableName("KnifeOKSignal2"), plc.ReadByVariableName("NeedleNGSignal2"), plc.ReadByVariableName("KnifeNGSignal2"));
                                    HandIVGRRdata(ref bt1, plc.ReadByVariableName("NeedleOKSignal3"), plc.ReadByVariableName("KnifeOKSignal3"), plc.ReadByVariableName("NeedleNGSignal3"), plc.ReadByVariableName("KnifeNGSignal3"));
                                    HandIVGRRdata(ref bt2, plc.ReadByVariableName("NeedleOKSignal4"), plc.ReadByVariableName("KnifeOKSignal4"), plc.ReadByVariableName("NeedleNGSignal4"), plc.ReadByVariableName("KnifeNGSignal4"));
                                }

                                SaveIVDataToFile(bt1, true);
                                SaveIVDataToFile(bt2, true);
                                SaveIVDataToFile(bt3, true);
                                SaveIVDataToFile(bt4, true);
                                plc.WriteByVariableName("IVConductionComplete", 1);
                                MotionEnum.EnumIVGRR = MotionEnum.IVGRR.IVGRR完成;

                            }
                            break;

                        case MotionEnum.IVGRR.IVGRR完成:


                            if (plc.ReadByVariableName("NeedleOKSignal1") == "0" && plc.ReadByVariableName("KnifeOKSignal1") == "0" && plc.ReadByVariableName("NeedleNGSignal1") == "0" && plc.ReadByVariableName("KnifeNGSignal1") == "0"
                                && plc.ReadByVariableName("NeedleOKSignal2") == "0" && plc.ReadByVariableName("KnifeOKSignal2") == "0" && plc.ReadByVariableName("NeedleNGSignal2") == "0" && plc.ReadByVariableName("KnifeNGSignal2") == "0"
                                && plc.ReadByVariableName("NeedleOKSignal3") == "0" && plc.ReadByVariableName("KnifeOKSignal3") == "0" && plc.ReadByVariableName("NeedleNGSignal3") == "0" && plc.ReadByVariableName("KnifeNGSignal3") == "0"
                                && plc.ReadByVariableName("NeedleOKSignal4") == "0" && plc.ReadByVariableName("KnifeOKSignal4") == "0" && plc.ReadByVariableName("NeedleNGSignal4") == "0" && plc.ReadByVariableName("KnifeNGSignal4") == "0")
                            {
                                WriteLog(string.Format("第{0}次 {1} ", time, step), LogLevels.Info, "IVGRR");
                                bt1.IV = "";
                                bt2.IV = "";
                                bt3.IV = "";
                                bt4.IV = "";
                                plc.WriteByVariableName("IVConductionComplete", 0);
                                if (testCount == 3)
                                {
                                    testCount = 0;
                                    testGroup++;
                                    bt1.Destroy();
                                    bt2.Destroy();
                                    bt3.Destroy();
                                    bt4.Destroy();
                                    isOneGroupFinish = true;
                                }
                                else
                                {
                                    time++;
                                    MotionEnum.EnumIVGRR = MotionEnum.IVGRR.获取IV数据;
                                }
                            }
                            break;
                    }
                }
                WriteLog("IVGRR线程退出", LogLevels.Info, "IVGRR");
            }
        }
        /// <summary>
        /// XRay GRR 需要 FQI 开启
        /// </summary>
        public void XRAYGRRFQI()
        {
            int index = 0;
            BatterySeat bt1 = new BatterySeat();
            BatterySeat bt2 = new BatterySeat();
            BatterySeat bt3 = new BatterySeat();
            BatterySeat bt4 = new BatterySeat();
            //string[] tt;
            if (isScanGRRFinish == true && queueGRRBarMdiCode.Count() >= 4)
            {
                //tt = new string[queueGRRBarcode.Count()];
                //queueGRRBarcode.CopyTo(tt,0);
                //Queue<string> tmp = new Queue<string>();
                //for (int i = 0; i < tt.Length; i++)
                //{
                //    tmp.Enqueue(tt[i]);
                //}
                string barcode1 = queueGRRBarMdiCode.Dequeue();
                string barcode2 = queueGRRBarMdiCode.Dequeue();
                string barcode3 = queueGRRBarMdiCode.Dequeue();
                string barcode4 = queueGRRBarMdiCode.Dequeue();
                bt1.Sn = barcode1;
                bt2.Sn = barcode2;
                bt3.Sn = barcode3;
                bt4.Sn = barcode4;
            }

            MotionEnum.EnumXraygrrMdi = MotionEnum.XRARYGRRMDI.工位2触发FQI拍照;

            while (true)
            {
                if (plc.ReadByVariableName("SoftwareReset") == "1" || _AutoRunning == false)
                {
                    bt1.Destroy();
                    bt2.Destroy();
                    bt3.Destroy();
                    bt4.Destroy();
                    isXRAYGRRMDIFinish = true;
                    MotionEnum.EnumXraygrrMdi = MotionEnum.XRARYGRRMDI.工位2触发FQI拍照;
                    return;
                }

                string step = Enum.GetName(typeof(MotionEnum.XRARYGRRMDI), MotionEnum.EnumXraygrrMdi);
                switch (MotionEnum.EnumXraygrrMdi)
                {
                    case MotionEnum.XRARYGRRMDI.工位2触发FQI拍照:
                        if (plc.ReadByVariableName("MDISignal2") == "1")
                        {
                            index++;
                            WriteLog(string.Format("第{0}次 {1} ", index, step), LogLevels.Info, "XRAYGRRFQI");
                            GetMdiData(ref bt2, index, 2, true);
                            plc.WriteByVariableName("MDIComplete2", 1);
                            MotionEnum.EnumXraygrrMdi = MotionEnum.XRARYGRRMDI.工位2拍照完成;
                        }
                        break;

                    case MotionEnum.XRARYGRRMDI.工位2拍照完成:

                        if (plc.ReadByVariableName("MDISignal2") == "0")
                        {
                            WriteLog(string.Format("第{0}次 {1} ", index, step), LogLevels.Info, "XRAYGRRFQI");
                            plc.WriteByVariableName("MDIComplete2", 0);
                            MotionEnum.EnumXraygrrMdi = MotionEnum.XRARYGRRMDI.工位1触发FQI拍照;
                        }
                        break;

                    case MotionEnum.XRARYGRRMDI.工位1触发FQI拍照:

                        if (plc.ReadByVariableName("MDISignal1") == "1")
                        {
                            index++;
                            WriteLog(string.Format("第{0}次 {1} ", index, step), LogLevels.Info, "XRAYGRRFQI");
                            GetMdiData(ref bt1, index, 1, true);
                            plc.WriteByVariableName("MDIComplete1", 1);
                            MotionEnum.EnumXraygrrMdi = MotionEnum.XRARYGRRMDI.工位1拍照完成;
                        }
                        break;

                    case MotionEnum.XRARYGRRMDI.工位1拍照完成:

                        if (plc.ReadByVariableName("MDISignal1") == "0")
                        {
                            WriteLog(string.Format("第{0}次 {1} ", index, step), LogLevels.Info, "XRAYGRRFQI");
                            plc.WriteByVariableName("MDIComplete1", 0);
                            MotionEnum.EnumXraygrrMdi = MotionEnum.XRARYGRRMDI.工位3触发FQI拍照;
                        }
                        break;

                    case MotionEnum.XRARYGRRMDI.工位3触发FQI拍照:

                        if (plc.ReadByVariableName("MDISignal3") == "1")
                        {
                            index++;
                            WriteLog(string.Format("第{0}次 {1} ", index, step), LogLevels.Info, "XRAYGRRFQI");
                            GetMdiData(ref bt3, index, 3, true);
                            plc.WriteByVariableName("MDIComplete1", 1);
                            MotionEnum.EnumXraygrrMdi = MotionEnum.XRARYGRRMDI.工位3拍照完成;
                        }
                        break;

                    case MotionEnum.XRARYGRRMDI.工位3拍照完成:

                        if (plc.ReadByVariableName("MDISignal3") == "0")
                        {
                            WriteLog(string.Format("第{0}次 {1} ", index, step), LogLevels.Info, "XRAYGRRFQI");
                            plc.WriteByVariableName("MDIComplete1", 0);
                            MotionEnum.EnumXraygrrMdi = MotionEnum.XRARYGRRMDI.工位4触发FQI拍照;
                        }
                        break;

                    case MotionEnum.XRARYGRRMDI.工位4触发FQI拍照:

                        if (plc.ReadByVariableName("MDISignal4") == "1")
                        {
                            index++;
                            WriteLog(string.Format("第{0}次 {1} ", index, step), LogLevels.Info, "XRAYGRRFQI");
                            GetMdiData(ref bt4, index, 4, true);
                            plc.WriteByVariableName("MDIComplete2", 1);
                            MotionEnum.EnumXraygrrMdi = MotionEnum.XRARYGRRMDI.工位4拍照完成;
                        }
                        break;

                    case MotionEnum.XRARYGRRMDI.工位4拍照完成:

                        if (plc.ReadByVariableName("MDISignal4") == "0")
                        {
                            WriteLog(string.Format("第{0}次 {1} ", index, step), LogLevels.Info, "XRAYGRRFQI");
                            plc.WriteByVariableName("MDIComplete2", 0);
                            MotionEnum.EnumXraygrrMdi = MotionEnum.XRARYGRRMDI.工位2触发FQI拍照;
                            if (index == 4)
                            {
                                bt1.Destroy();
                                bt2.Destroy();
                                bt3.Destroy();
                                bt4.Destroy();
                                index = 0;
                                isXRAYGRRMDIFinish = true;
                                return;
                            }
                        }
                        break;
                }

            }

        }
        private void XRAYGRR()
        {
            //一个电池连续测试12次
            WriteLog("进入XRAYGRR线程", LogLevels.Info, "XRAYGRR");
            string strStep = "";
            BatterySeat bt = new BatterySeat();
            _workingSeats._seat1.CopyTo(ref bt);
            if (queueGRRBarcode.Count() > 0)
            {
                bt.Sn = queueGRRBarcode.Dequeue();
                bt.LengthBarcode = bt.Sn;
            }
            int index = 0;
            MotionEnum.EnumXraygrr = MotionEnum.XRAYGRR.触发角1拍照;
            while (true)
            {
                Thread.Sleep(10);
                if (_AutoRunning == false)
                {
                    return;
                }
                if (plc.ReadByVariableName("SoftwareReset") == "1")
                {
                    isXray1GRRFinish = true;
                    return;
                }
                strStep = Enum.GetName(typeof(MotionEnum.XRAYGRR), MotionEnum.EnumXraygrr);

                switch (MotionEnum.EnumXraygrr)
                {
                    case MotionEnum.XRAYGRR.触发角1拍照:
                        if (plc.ReadByVariableName("Battery1CornerPhotoSignal") == "1")
                        {
                            bt.CheckExtension = ECheckExtensions.Local;
                            index++;
                            WriteLog(string.Format("第{0}次 {1} ", index, strStep), LogLevels.Info, "XRAYGRR");
                            Thread.Sleep(HardwareConfig.Instance.CameraShotDelay4);
                            //TODO: 替换旧版相机采图  ZhangKF
                            //ZYImageStruct img1 = bt.Corner4.ShotImage;
                            try
                            {
                                //int iRet = -2;
                                //int k = 0;
                                if (!string.IsNullOrEmpty(bt.Sn))
                                {
                                    //iRet = VisionSysWrapperIF.ShotAndAverage(ECamTypes.Camera1, _camParam1.PinValue, ref img1, 4, out k);
                                    var result = CameraHelper.CaptureOneImage(0);
                                    if (result.Result)
                                    {
                                        bt.Corner4.ShotImage = result.Bitmap.ToTransfor(Consts.ImageTypes.Sixteen);
                                    }
                                    else
                                    {
                                        WriteLog(string.Format("第{0}次 角一采图 保存图片失败：{1}", index), LogLevels.Info, "XRAYGRR");
                                    }

                                    bt.Corner4.IsShotOK = result.Result;
                                }
                                //bt.Corner4.ImgNO = k;
                                WriteLog(string.Format("第{0}次 角一采图 保存图片, 16位图编号：{1}", index), LogLevels.Info, "XRAYGRR");
                                string picname = "D:\\Test\\4_" + bt.Sn + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + index + ".jpg";
                            }
                            catch (Exception ex)
                            {
                                bt.Corner4.IsShotOK = false;
                                WriteLog(string.Format("第{0}次 角一采图失败,iRet = {1}", index, -1), LogLevels.Info, "XRAYGRR");
                            }
                            bt.Corner4.IsShotOK = true;
                            TestCorner4(ref bt, index);
                            plc.WriteByVariableName("Battery1CornerPhotoComplete", 1);
                            MotionEnum.EnumXraygrr = MotionEnum.XRAYGRR.角1拍照完成;
                        }

                        break;

                    case MotionEnum.XRAYGRR.角1拍照完成:

                        if (plc.ReadByVariableName("Battery1CornerPhotoSignal") == "0")
                        {
                            WriteLog(string.Format("第{0}次 {1} ", index, strStep), LogLevels.Info, "XRAYGRR");
                            plc.WriteByVariableName("Battery1CornerPhotoComplete", 0);
                            MotionEnum.EnumXraygrr = MotionEnum.XRAYGRR.触发角2拍照;
                        }

                        break;

                    case MotionEnum.XRAYGRR.触发角2拍照:

                        if (plc.ReadByVariableName("Battery2CornerPhotoSignal") == "1")
                        {
                            WriteLog(string.Format("第{0}次 {1} ", index, strStep), LogLevels.Info, "XRAYGRR");
                            Thread.Sleep(HardwareConfig.Instance.CameraShotDelay3);
                            //TODO: 替换旧版采图 ZhangKF 2021-3-16
                            //ZYImageStruct img2 = bt.Corner3.ShotImage;
                            try
                            {
                                int iRet2 = -2;
                                int k = 0;
                                if (!string.IsNullOrEmpty(bt.Sn))
                                {
                                    //iRet2 = VisionSysWrapperIF.ShotAndAverage(ECamTypes.Camera1, _camParam1.PinValue, ref img2, 4, out k);
                                    var result = CameraHelper.CaptureOneImage(0);
                                    if (result.Result)
                                    {
                                        bt.Corner3.ShotImage = result.Bitmap.ToTransfor(Consts.ImageTypes.Sixteen);
                                    }
                                    else
                                    {
                                        WriteLog(string.Format("第{0}次 角二采图 保存图片失败：{1}", index), LogLevels.Info, "XRAYGRR");
                                    }

                                    bt.Corner3.IsShotOK = result.Result;
                                }
                                //bt.Corner3.ImgNO = k;
                                WriteLog(string.Format("第{0}次 角二采图 保存图片, 16位图编号：{1} {2}", index, bt.Corner3.ImgNO, k), LogLevels.Info, "XRAYGRR");
                                string picname = "D:\\Test\\3_" + bt.Sn + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + index + ".jpg";
                            }
                            catch (Exception ex)
                            {
                                bt.Corner3.IsShotOK = false;
                                WriteLog(string.Format("第{0}次 角二采图失败,iRet2 = {1}", index, -1), LogLevels.Info, "XRAYGRR");
                            }

                            bt.Corner3.IsShotOK = true;
                            TestCorner3(ref bt, index);
                            plc.WriteByVariableName("Battery2CornerPhotoComplete", 1);

                            MotionEnum.EnumXraygrr = MotionEnum.XRAYGRR.角2拍照完成;
                        }

                        break;

                    case MotionEnum.XRAYGRR.角2拍照完成:

                        if (plc.ReadByVariableName("Battery2CornerPhotoSignal") == "0")
                        {
                            WriteLog(string.Format("第{0}次 {1} ", index, strStep), LogLevels.Info, "XRAYGRR");
                            plc.WriteByVariableName("Battery2CornerPhotoComplete", 0);
                            MotionEnum.EnumXraygrr = MotionEnum.XRAYGRR.触发角3拍照;
                        }

                        break;

                    case MotionEnum.XRAYGRR.触发角3拍照:

                        if (plc.ReadByVariableName("Battery3CornerPhotoSignal") == "1")
                        {
                            WriteLog(string.Format("第{0}次 {1} ", index, strStep), LogLevels.Info, "XRAYGRR");
                            Thread.Sleep(HardwareConfig.Instance.CameraShotDelay1);
                            //TODO: 替换旧版采图 ZhangKF 2021-3-16
                            //ZYImageStruct img3 = bt.Corner2.ShotImage;
                            try
                            {
                                int iRet3 = -2;
                                int k = 0;
                                if (!string.IsNullOrEmpty(bt.Sn))
                                {
                                    //iRet3 = VisionSysWrapperIF.ShotAndAverage(ECamTypes.Camera2, _camParam2.PinValue, ref img3, 4, out k);
                                    var result = CameraHelper.CaptureOneImage(1);
                                    if (result.Result)
                                    {
                                        bt.Corner2.ShotImage = result.Bitmap.ToTransfor(Consts.ImageTypes.Sixteen);
                                    }
                                    else
                                    {
                                        WriteLog(string.Format("第{0}次 角三采图 保存图片失败：{1}", index), LogLevels.Info, "XRAYGRR");
                                    }
                                    bt.Corner2.IsShotOK = result.Result;
                                }
                                //bt.Corner2.ImgNO = k;
                                WriteLog(string.Format("第{0}次 角三采图 保存图片, 16位图编号：{1} {2}", index, bt.Corner2.ImgNO, k), LogLevels.Info, "XRAYGRR");
                                string picname = "D:\\Test\\2_" + bt.Sn + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + index + ".jpg";
                            }
                            catch (Exception ex)
                            {
                                bt.Corner2.IsShotOK = false;
                                WriteLog(string.Format("第{0}次 角三采图失败,iRet3 = {1}", index, -1), LogLevels.Info, "XRAYGRR");
                            }

                            bt.Corner2.IsShotOK = true;
                            TestCorner2(ref bt, index);
                            plc.WriteByVariableName("Battery3CornerPhotoComplete", 1);
                            MotionEnum.EnumXraygrr = MotionEnum.XRAYGRR.角3拍照完成;
                        }

                        break;

                    case MotionEnum.XRAYGRR.角3拍照完成:

                        if (plc.ReadByVariableName("Battery3CornerPhotoSignal") == "0")
                        {
                            WriteLog(string.Format("第{0}次 {1} ", index, strStep), LogLevels.Info, "XRAYGRR");
                            plc.WriteByVariableName("Battery3CornerPhotoComplete", 0);
                            MotionEnum.EnumXraygrr = MotionEnum.XRAYGRR.触发角4拍照;
                        }

                        break;

                    case MotionEnum.XRAYGRR.触发角4拍照:

                        if (plc.ReadByVariableName("Battery4CornerPhotoSignal") == "1")
                        {
                            WriteLog(string.Format("第{0}次 {1} ", index, strStep), LogLevels.Info, "XRAYGRR");
                            Thread.Sleep(HardwareConfig.Instance.CameraShotDelay2);
                            //TODO: 替换旧版采图 ZhangKF 2021-3-16
                            //ZYImageStruct img4 = bt.Corner1.ShotImage;
                            try
                            {
                                //int iRet4 = -2;
                                int k = 0;
                                if (!string.IsNullOrEmpty(bt.Sn))
                                {
                                    //iRet4 = VisionSysWrapperIF.ShotAndAverage(ECamTypes.Camera2, _camParam2.PinValue, ref img4, 4, out k);
                                    var result = CameraHelper.CaptureOneImage(1);
                                    if (result.Result)
                                    {
                                        bt.Corner1.ShotImage = result.Bitmap.ToTransfor(Consts.ImageTypes.Sixteen);
                                    }
                                    else
                                    {
                                        WriteLog(string.Format("第{0}次 角四采图 保存图片失败：{1}", index), LogLevels.Info, "XRAYGRR");
                                    }
                                    bt.Corner1.IsShotOK = result.Result;
                                }
                                //bt.Corner1.ImgNO = k;
                                WriteLog(string.Format("第{0}次 角四采图 保存图片, 16位图编号：{1} {2}", index, bt.Corner1.ImgNO, k), LogLevels.Info, "XRAYGRR");
                                string picname = "D:\\Test\\1_" + bt.Sn + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + index + ".jpg";
                                //img4.Save(picname, 1, bt.Corner1.ImgNO);
                            }
                            catch (Exception ex)
                            {
                                bt.Corner1.IsShotOK = false;
                                WriteLog(string.Format("第{0}次 角四采图失败,iRet4 = {1}", index, -1), LogLevels.Info, "XRAYGRR");
                            }
                            bt.Corner1.IsShotOK = true;
                            TestCorner1(ref bt, index);
                            plc.WriteByVariableName("Battery4CornerPhotoComplete", 1);
                            MotionEnum.EnumXraygrr = MotionEnum.XRAYGRR.角4拍照完成;
                        }
                        break;

                    case MotionEnum.XRAYGRR.角4拍照完成:

                        if (plc.ReadByVariableName("Battery4CornerPhotoSignal") == "0")
                        {
                            WriteLog(string.Format("第{0}次 {1} ", index, strStep), LogLevels.Info, "XRAYGRR");
                            plc.WriteByVariableName("Battery4CornerPhotoComplete", 0);
                            MotionEnum.EnumXraygrr = MotionEnum.XRAYGRR.触发结果发送;
                        }

                        break;

                    case MotionEnum.XRAYGRR.触发结果发送:
                        {
                            WriteLog(string.Format("第{0}次 {1} ", index, strStep), LogLevels.Info, "XRAYGRR");
                            bt.UpdateResult();
                            WriteLog(string.Format("第{0}次 Final result is {1} {2}-{3}: {4} {5} {6} {7} ",
                                index,
                                bt.Sn,
                                bt.FinalResult,
                                bt.ResultCode,
                                bt.Corner1.InspectResults.resultDataMin.iResult,
                                bt.Corner2.InspectResults.resultDataMin.iResult,
                                bt.Corner3.InspectResults.resultDataMin.iResult,
                                bt.Corner4.InspectResults.resultDataMin.iResult), LogLevels.Info, "XRAYGRR");

                            WriteLog(string.Format("第{0}次 合成结果图", index), LogLevels.Info, "XRAYGRR");
                            AlgoWrapper.Instance.GetResultImage(ref bt);
                            WriteLog(string.Format("第{0}次 合成结果图完成", index), LogLevels.Info, "XRAYGRR");
                            ResultRelay.Instance.CheckResultHandlers(bt);//刷新监控状态页面
                            _extRunEmpty.OnCheckFinished(bt, _imageSaveConfig, ref _checkStatus);//XRayClient.Core\CheckLogic\Extensions\CheckLogicExtRunEmpty.cs
                            WriteLog(string.Format("第{0}次 保存图片", index), LogLevels.Info, "XRAYGRR");
                            _extSTF.OnSaveImages(bt, _imageSaveConfig);//XRayClient.Core\CheckLogic\Extensions\CheckLogicExtSTF.cs

                            try
                            {
                                bool isErr = false;
                                //if (bt.ResultCode == EResultCodes.AlgoErr || bt.ResultCode == EResultCodes.Unknow)
                                //{
                                //    isErr = true;
                                //}
                                //else
                                //{
                                SaveXRayResultToFile(bt, CheckParamsConfig.Instance.TotalLayer, CheckParamsConfig.Instance.TotalLayersBD, 0, isErr, true);
                                //}
                            }
                            catch (Exception ex)
                            {
                                WriteLog(string.Format("在第{0}次 保存XRay测量数据出错: {1}", index, ex.Message), LogLevels.Error, "XRAYGRR");
                            }
                            plc.WriteByVariableName("ResultOK", 1);
                            MotionEnum.EnumXraygrr = MotionEnum.XRAYGRR.发送结果完成;
                        }

                        break;

                    case MotionEnum.XRAYGRR.发送结果完成:
                        {
                            WriteLog(string.Format("第{0}次 {1} ", index, strStep), LogLevels.Info, "XRAYGRR");
                            plc.WriteByVariableName("ResultOK", 0);
                            MotionEnum.EnumXraygrr = MotionEnum.XRAYGRR.触发角1拍照;
                            //if (index == 12)
                            if (index == 3)
                            {
                                index = 0;
                                WriteLog("退出XRAYGRR线程", LogLevels.Info, "XRAYGRR");
                                bt.Destroy();
                                isXray1GRRFinish = true;
                                return;
                            }
                        }
                        break;
                }
            }
        }

        private void OCVGRR()
        {
            //一次性上12个料
            //第一次：1 2 3 4 5 6 7 8 9 10 11 12
            //第二次：1 2 3 4 5 6 7 8 9 10 11 12
            //第三次：1 2 3 4 5 6 7 8 9 10 11 12
            //第四次：1 2 3 4 5 6 7 8 9 10 11 12
            //第五次：2 1 4 3 6 5 8 7 10 9 12 11
            //第六次：2 1 4 3 6 5 8 7 10 9 12 11
            //第七次：2 1 4 3 6 5 8 7 10 9 12 11
            //第八次：2 1 4 3 6 5 8 7 10 9 12 11

            string strStep = "";
            BatterySeat bt1 = new BatterySeat();
            BatterySeat bt2 = new BatterySeat();
            int index = 0;
            int tickCount = 0;
            string barcode1 = "";
            string barcode2 = "";
            MotionEnum.EnumOCVGRR = MotionEnum.OCVGRR.工位2触发电压测试;

            while (true)
            {
                Thread.Sleep(10);
                if (_AutoRunning == false)
                {
                    return;
                }
                if (plc.ReadByVariableName("SoftwareReset") == "1")
                {
                    MotionEnum.EnumOCVGRR = MotionEnum.OCVGRR.工位2触发电压测试;
                    return;
                }

                strStep = Enum.GetName(typeof(MotionEnum.OCVGRR), MotionEnum.EnumOCVGRR);

                switch (MotionEnum.EnumOCVGRR)
                {
                    case MotionEnum.OCVGRR.工位2触发电压测试:

                        if (plc.ReadByVariableName("VoltageSignal2") == "1")
                        {
                            index++;
                            barcode1 = listOCVGRRBarcode[index * 2 - 2];
                            barcode2 = listOCVGRRBarcode[index * 2 - 1];

                            if (index > 24)
                            {
                                bt1.Sn = barcode2;
                                bt2.Sn = barcode1;
                            }
                            else
                            {
                                bt1.Sn = barcode1;
                                bt2.Sn = barcode2;
                            }
                            bt1.OCVChannel = 1;
                            bt2.OCVChannel = 2;

                            tickCount = Environment.TickCount;
                            WriteLog(string.Format("第{0}次 {1} {2}", index, strStep, bt2.Sn), LogLevels.Info, "OCVGRR");
                            double Vol = Tcp34461A.Get34461AVoltage();
                            bt2.Voltage = Convert.ToSingle(Vol);
                            if (bt2.Voltage >= CheckParamsConfig.Instance.MinVoltage &&
                                bt2.Voltage <= CheckParamsConfig.Instance.MaxVoltage)
                            {
                                bt2.VoltageResult = true;
                            }
                            else
                            {
                                bt2.VoltageResult = false;
                            }
                            plc.WriteByVariableName("VoltageComplete2", 1);

                            MotionEnum.EnumOCVGRR = MotionEnum.OCVGRR.工位1触发内阻测试;
                        }
                        break;

                    case MotionEnum.OCVGRR.工位1触发内阻测试:

                        if (plc.ReadByVariableName("ResistanceSignal1") == "1")
                        {
                            WriteLog(string.Format("第{0}次 {1} {2}", index, strStep, bt1.Sn), LogLevels.Info, "OCVGRR");
                            double res = Bt3562.GetRisitance();
                            bt1.Resistance = Convert.ToSingle(res);
                            if (bt1.Resistance >= CheckParamsConfig.Instance.MinResistance &&
                                bt1.Resistance <= CheckParamsConfig.Instance.MaxResistance)
                            {
                                bt1.ResistanceResult = true;
                            }
                            else
                            {
                                bt1.ResistanceResult = false;
                            }

                            //电池表面温度
                            bt1.Temperature = Convert.ToSingle(Mi3_1.GetRaytekMI3Temperatrue());
                            if (bt1.Temperature >= CheckParamsConfig.Instance.MinTemperature &&
                                bt1.Temperature <= CheckParamsConfig.Instance.MaxTemperature)
                            {
                                bt1.TemperatureResult = true;
                            }
                            else
                            {
                                bt1.TemperatureResult = false;
                            }

                            double environmentTemperatrue = E5cc.GetE5CCTemperatrue();
                            bt1.EnvirementTemperature = Convert.ToSingle(environmentTemperatrue);
                            plc.WriteByVariableName("ResistanceComplete1", 1);

                            MotionEnum.EnumOCVGRR = MotionEnum.OCVGRR.工位2触发内阻测试;
                        }
                        break;

                    case MotionEnum.OCVGRR.工位2触发内阻测试:

                        if (plc.ReadByVariableName("VoltageSignal2") == "0" &&
                            plc.ReadByVariableName("ResistanceSignal1") == "0")
                        {
                            plc.WriteByVariableName("VoltageComplete2", 0);
                            plc.WriteByVariableName("ResistanceComplete1", 0);

                            if (plc.ReadByVariableName("ResistanceSignal2") == "1")
                            {
                                WriteLog(string.Format("第{0}次 {1} {2}", index, strStep, bt2.Sn), LogLevels.Info, "OCVGRR");

                                double res = Bt3562.GetRisitance();
                                bt2.Resistance = Convert.ToSingle(res);
                                if (bt2.Resistance >= CheckParamsConfig.Instance.MinResistance &&
                                    bt2.Resistance <= CheckParamsConfig.Instance.MaxResistance)
                                {
                                    bt2.ResistanceResult = true;
                                }
                                else
                                {
                                    bt2.ResistanceResult = false;
                                }

                                //电池表面温度
                                bt2.Temperature = Convert.ToSingle(Mi3_2.GetRaytekMI3Temperatrue());
                                if (bt2.Temperature >= CheckParamsConfig.Instance.MinTemperature &&
                                    bt2.Temperature <= CheckParamsConfig.Instance.MaxTemperature)
                                {
                                    bt2.TemperatureResult = true;
                                }
                                else
                                {
                                    bt2.TemperatureResult = false;
                                }

                                double environmentTemperatrue = E5cc.GetE5CCTemperatrue();
                                bt2.EnvirementTemperature = Convert.ToSingle(environmentTemperatrue);
                                plc.WriteByVariableName("ResistanceComplete2", 1);
                                WriteLog(string.Format("第{0}次 {1} OCV测量电压为 {2} , 内阻为{3} , 电池表面温度为{4} , 环境温度为{5}", index, bt2.Sn, bt2.Voltage, res, bt2.Temperature, environmentTemperatrue, bt2.Sn), LogLevels.Info, "OCVGRR");
                                MotionEnum.EnumOCVGRR = MotionEnum.OCVGRR.工位1触发电压测试;

                            }
                        }
                        break;

                    case MotionEnum.OCVGRR.工位1触发电压测试:

                        if (plc.ReadByVariableName("VoltageSignal1") == "1")
                        {
                            WriteLog(string.Format("第{0}次 {1} {2}", index, strStep, bt1.Sn), LogLevels.Info, "OCVGRR");
                            double Vol = Tcp34461A.Get34461AVoltage();
                            bt1.Voltage = Convert.ToSingle(Vol);
                            if (bt1.Voltage >= CheckParamsConfig.Instance.MinVoltage &&
                                bt1.Voltage <= CheckParamsConfig.Instance.MaxVoltage)
                            {
                                bt1.VoltageResult = true;
                            }
                            else
                            {
                                bt1.VoltageResult = false;
                            }
                            plc.WriteByVariableName("VoltageComplete1", 1);

                            try
                            {
                                if (index <= 6 || (index > 24 && index <= 30))
                                {

                                }
                                else
                                {
                                    SaveOCVDataToFile(bt1, true);
                                    SaveOCVDataToFile(bt2, true);
                                }

                            }
                            catch (Exception ex)
                            {
                                WriteLog("OCVGRR保存数据异常！", LogLevels.Warn, "OCVGRR");
                            }
                            WriteLog(string.Format("第{0}次 {1} OCV测量电压为 {2} , 内阻为{3} , 电池表面温度为{4} , 环境温度为{5} {6}", index, bt1.Sn, bt1.Voltage, bt1.Resistance, bt1.Temperature, bt1.EnvirementTemperature, bt1.Sn), LogLevels.Info, "OCVGRR");

                            MotionEnum.EnumOCVGRR = MotionEnum.OCVGRR.OCV测试完成;
                        }
                        break;

                    case MotionEnum.OCVGRR.OCV测试完成:

                        if (plc.ReadByVariableName("ResistanceSignal2") == "0" && plc.ReadByVariableName("VoltageSignal1") == "0")
                        {
                            WriteLog(string.Format("第{0}次 {1} ,共耗时：{2} ms", index, strStep, Environment.TickCount - tickCount), LogLevels.Info, "OCVGRR");
                            plc.WriteByVariableName("ResistanceComplete2", 0);
                            plc.WriteByVariableName("VoltageComplete1", 0);
                            if (index == 48)
                            {
                                queueGRRBarcode.Clear();
                                bt1.Destroy();
                                bt2.Destroy();
                                WriteLog("OCVGRR结束，退出线程", LogLevels.Info, "OCVGRR");
                                isOCVGRRFinish = true;
                                return;
                            }
                            MotionEnum.EnumOCVGRR = MotionEnum.OCVGRR.工位2触发电压测试;
                        }

                        break;
                }
            }
        }


        /// <summary>
        /// 根据条码代号移除电池
        /// </summary>
        /// <param name="tempBarcode">条码代号</param>
        /// <param name="queueData">电池队列</param>
        /// <returns>true：找到电池并移除；false：队列中没有该电池</returns>
        private bool RemoveProduct(int tempBarcode, ref Queue<BatterySeat> queueData)
        {
            Queue<BatterySeat> queueTemp = new Queue<BatterySeat>();
            bool isProductExists = false;//电池是否在队列中
            foreach (BatterySeat bt in queueData)
            {
                if (bt.TempBarcode == tempBarcode)
                {
                    isProductExists = true;
                    break;
                }
            }
            if (isProductExists == true)
            {
                while (queueData.Count != 0)
                {
                    BatterySeat bt = queueData.Dequeue();
                    if (bt.TempBarcode == tempBarcode)
                    {
                        bt.Destroy();
                        WriteLog("移除条码代号：" + tempBarcode + " 的电池成功", LogLevels.Info, "RemoveProduct");
                    }
                    else
                    {
                        queueTemp.Enqueue(bt);
                    }
                }
                queueData = queueTemp;
            }
            return isProductExists;
        }

        private bool RemoveScanBarcode(int tempBarcode, ref Dictionary<int, string> dic)
        {
            if (dic.ContainsKey(tempBarcode))
            {
                dic.Remove(tempBarcode);
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 手动XRAYGRR
        /// </summary>
        //private void Task_Shot()
        //{
        //    int step1 = 0;
        //    int step2 = 0;
        //    int step3 = 0;
        //    int step4 = 0;

        //    while (true)
        //    {
        //        Thread.Sleep(10);
        //        if (plc.ReadByVariableName("TestMode") == "5")
        //        {
        //            switch (step1)
        //            {
        //                case 0:
        //                    if (plc.ReadByVariableName("Battery1CornerPhotoSignal") == "1")
        //                    {
        //                        WriteLog("触发拍照1", LogLevels.Info, "Task_Shot");
        //                        _workingSeats._seat1.Sn = XRayTubeIdleTime.ToString();
        //                        _workingSeats._seat1.CheckExtension = ECheckExtensions.Local;
        //                        ZYImageStruct img1 = _workingSeats._seat1.Corner4.ShotImage;
        //                        int k = 0;
        //                        int iRet1 = VisionSysWrapperIF.ShotAndAverage(ECamTypes.Camera1, _camParam1.PinValue, ref img1, 4, out k);
        //                        _workingSeats._seat1.Corner4.ImgNO = k;
        //                        string picname = "D:\\Test\\4_" + DateTime.Now.ToString("yyyyMMdd") + ".jpg";
        //                        img1.Save(picname, 1, _workingSeats._seat1.Corner4.ImgNO);
        //                        TestCorner4(ref _workingSeats._seat1, 1);
        //                        plc.WriteByVariableName("Battery1CornerPhotoComplete", 1);
        //                        step1 = 1;
        //                    }
        //                    break;

        //                case 1:
        //                    if (plc.ReadByVariableName("Battery1CornerPhotoSignal") == "0")
        //                    {
        //                        WriteLog("触发拍照1完成", LogLevels.Info, "Task_Shot");
        //                        plc.WriteByVariableName("Battery1CornerPhotoComplete", 0);
        //                        step1 = 0;
        //                    }
        //                    break;
        //            }

        //            switch (step2)
        //            {
        //                case 0:
        //                    if (plc.ReadByVariableName("Battery2CornerPhotoSignal") == "1")
        //                    {
        //                        WriteLog("触发拍照2", LogLevels.Info, "Task_Shot");
        //                        ZYImageStruct img2 = _workingSeats._seat1.Corner3.ShotImage;
        //                        int k = 0;
        //                        int iRet2 = VisionSysWrapperIF.ShotAndAverage(ECamTypes.Camera1, _camParam1.PinValue, ref img2, 4, out k);
        //                        _workingSeats._seat1.Corner3.ImgNO = k;
        //                        string picname = "D:\\Test\\3_" + DateTime.Now.ToString("yyyyMMdd") + ".jpg";
        //                        img2.Save(picname, 1, _workingSeats._seat1.Corner3.ImgNO);
        //                        TestCorner3(ref _workingSeats._seat1, 1);
        //                        plc.WriteByVariableName("Battery2CornerPhotoComplete", 1);
        //                        step2 = 1;
        //                    }
        //                    break;

        //                case 1:
        //                    if (plc.ReadByVariableName("Battery2CornerPhotoSignal") == "0")
        //                    {
        //                        WriteLog("触发拍照2完成", LogLevels.Info, "Task_Shot");
        //                        plc.WriteByVariableName("Battery2CornerPhotoComplete", 0);
        //                        step2 = 0;
        //                    }
        //                    break;
        //            }

        //            switch (step3)
        //            {
        //                case 0:
        //                    if (plc.ReadByVariableName("Battery3CornerPhotoSignal") == "1")
        //                    {
        //                        WriteLog("触发拍照3", LogLevels.Info, "Task_Shot");
        //                        ZYImageStruct img3 = _workingSeats._seat1.Corner2.ShotImage;
        //                        int k = 0;
        //                        int iRet3 = VisionSysWrapperIF.ShotAndAverage(ECamTypes.Camera2, _camParam2.PinValue, ref img3, 4, out k);
        //                        _workingSeats._seat1.Corner2.ImgNO = k;
        //                        string picname = "D:\\Test\\1_" + DateTime.Now.ToString("yyyyMMdd") + ".jpg";
        //                        img3.Save(picname, 1, _workingSeats._seat1.Corner2.ImgNO);
        //                        TestCorner2(ref _workingSeats._seat1, 1);
        //                        plc.WriteByVariableName("Battery3CornerPhotoComplete", 1);
        //                        step3 = 1;
        //                    }
        //                    break;

        //                case 1:
        //                    if (plc.ReadByVariableName("Battery3CornerPhotoSignal") == "0")
        //                    {
        //                        WriteLog("触发拍照3完成", LogLevels.Info, "Task_Shot");
        //                        plc.WriteByVariableName("Battery3CornerPhotoComplete", 0);
        //                        step3 = 0;
        //                    }
        //                    break;
        //            }

        //            switch (step4)
        //            {
        //                case 0:
        //                    if (plc.ReadByVariableName("Battery4CornerPhotoSignal") == "1")
        //                    {
        //                        WriteLog("触发拍照4", LogLevels.Info, "Task_Shot");
        //                        ZYImageStruct img4 = _workingSeats._seat1.Corner1.ShotImage;
        //                        int k = 0;
        //                        int iRet4 = VisionSysWrapperIF.ShotAndAverage(ECamTypes.Camera2, _camParam2.PinValue, ref img4, 4, out k);
        //                        _workingSeats._seat1.Corner1.ImgNO = k;
        //                        string picname = "D:\\Test\\1_" + DateTime.Now.ToString("yyyyMMdd") + ".jpg";
        //                        img4.Save(picname, 1, _workingSeats._seat1.Corner1.ImgNO);
        //                        TestCorner1(ref _workingSeats._seat1, 1);
        //                        _workingSeats._seat1.UpdateResult();
        //                        WriteLog(string.Format("第{0}次 合成结果图", 1), LogLevels.Info, "Task_XrayShot2");
        //                        AlgoWrapper.Instance.GetResultImage(ref _workingSeats._seat1);
        //                        WriteLog(string.Format("第{0}次 合成结果图完成", 1), LogLevels.Info, "Task_XrayShot2");
        //                        ResultRelay.Instance.CheckResultHandlers(_workingSeats._seat1);//刷新监控状态页面
        //                        _workingSeats._seat1.MesResult = true;
        //                        SaveXRayResultToFile(_workingSeats._seat1, CheckParamsConfig.Instance.TotalLayer, CheckParamsConfig.Instance.TotalLayersBD, 0, false);
        //                        _extRunEmpty.OnCheckFinished(_workingSeats._seat1, _imageSaveConfig, ref _checkStatus);//XRayClient.Core\CheckLogic\Extensions\CheckLogicExtRunEmpty.cs
        //                        WriteLog(string.Format("第{0}次 保存图片", 1), LogLevels.Info, "Task_Sorting");
        //                        _extSTF.OnSaveImages(_workingSeats._seat1, _imageSaveConfig);//XRayClient.Core\CheckLogic\Extensions\CheckLogicExtSTF.cs

        //                        plc.WriteByVariableName("Battery4CornerPhotoComplete", 1);
        //                        step4 = 1;
        //                    }
        //                    break;

        //                case 1:
        //                    if (plc.ReadByVariableName("Battery4CornerPhotoSignal") == "0")
        //                    {
        //                        WriteLog("触发拍照4完成", LogLevels.Info, "Task_Shot");
        //                        plc.WriteByVariableName("Battery4CornerPhotoComplete", 0);
        //                        step4 = 0;
        //                    }
        //                    break;
        //            }

        //        }

        //    }
        //}

        private void EnterQueue1(BatterySeat bt, string currentStation)
        {
            switch (currentStation)
            {
                case "BarcodeBinding1":
                    if (plc.ReadByVariableName("NOPPG") == "1" && plc.ReadByVariableName("NOMDI") == "1" && plc.ReadByVariableName("NOOCV") == "1" && plc.ReadByVariableName("NOIV") == "1")
                    {
                        queueThickness.Enqueue(bt);
                    }
                    else if (plc.ReadByVariableName("NOMDI") == "1" && plc.ReadByVariableName("NOOCV") == "1" && plc.ReadByVariableName("NOIV") == "1")
                    {
                        queueMDIStation1.Enqueue(bt);
                    }
                    else if (plc.ReadByVariableName("NOOCV") == "1" && plc.ReadByVariableName("NOIV") == "1")
                    {
                        queueOCVStation1.Enqueue(bt);
                    }
                    else if (plc.ReadByVariableName("NOIV") == "1")
                    {
                        queueIVStation1.Enqueue(bt);
                    }
                    else
                    {
                        queueScanStation1.Enqueue(bt);
                    }
                    break;

                case "BarcodeBinding2":
                    if (plc.ReadByVariableName("NOPPG") == "1" && plc.ReadByVariableName("NOMDI") == "1" && plc.ReadByVariableName("NOOCV") == "1" && plc.ReadByVariableName("NOIV") == "1")
                    {
                        queueThickness.Enqueue(bt);
                    }
                    else if (plc.ReadByVariableName("NOMDI") == "1" && plc.ReadByVariableName("NOOCV") == "1" && plc.ReadByVariableName("NOIV") == "1")
                    {
                        queueMDIStation2.Enqueue(bt);
                    }
                    else if (plc.ReadByVariableName("NOOCV") == "1" && plc.ReadByVariableName("NOIV") == "1")
                    {
                        queueOCVStation2.Enqueue(bt);
                    }
                    else if (plc.ReadByVariableName("NOIV") == "1")
                    {
                        queueIVStation2.Enqueue(bt);
                    }
                    else
                    {
                        queueScanStation2.Enqueue(bt);
                    }
                    break;

                case "IV1":
                case "IV3":
                    if (plc.ReadByVariableName("NOXRAY") == "1" && plc.ReadByVariableName("NOPPG") == "1" && plc.ReadByVariableName("NOMDI") == "1" && plc.ReadByVariableName("NOOCV") == "1")
                    {
                        queueXRAY2Station.Enqueue(bt);
                    }
                    else if (plc.ReadByVariableName("NOPPG") == "1" && plc.ReadByVariableName("NOMDI") == "1" && plc.ReadByVariableName("NOOCV") == "1")
                    {
                        queueThickness.Enqueue(bt);
                    }
                    else if (plc.ReadByVariableName("NOMDI") == "1" && plc.ReadByVariableName("NOOCV") == "1")
                    {
                        queueMDIStation1.Enqueue(bt);
                    }
                    else if (plc.ReadByVariableName("NOOCV") == "1")
                    {
                        queueOCVStation1.Enqueue(bt);
                    }
                    else
                    {
                        queueIVStation1.Enqueue(bt);
                    }
                    break;

                case "IV2":
                case "IV4":
                    if (plc.ReadByVariableName("NOXRAY") == "1" && plc.ReadByVariableName("NOPPG") == "1" && plc.ReadByVariableName("NOMDI") == "1" && plc.ReadByVariableName("NOOCV") == "1")
                    {
                        queueXRAY2Station.Enqueue(bt);
                    }
                    else if (plc.ReadByVariableName("NOPPG") == "1" && plc.ReadByVariableName("NOMDI") == "1" && plc.ReadByVariableName("NOOCV") == "1")
                    {
                        queueThickness.Enqueue(bt);
                    }
                    else if (plc.ReadByVariableName("NOMDI") == "1" && plc.ReadByVariableName("NOOCV") == "1")
                    {
                        queueMDIStation2.Enqueue(bt);
                    }
                    else if (plc.ReadByVariableName("NOOCV") == "1")
                    {
                        queueOCVStation2.Enqueue(bt);
                    }
                    else
                    {
                        queueIVStation2.Enqueue(bt);
                    }
                    break;

                case "OCV1":

                    if (plc.ReadByVariableName("NOXRAY") == "1" && plc.ReadByVariableName("NOPPG") == "1" && plc.ReadByVariableName("NOMDI") == "1")
                    {
                        queueXRAY2Station.Enqueue(bt);
                    }
                    else if (plc.ReadByVariableName("NOPPG") == "1" && plc.ReadByVariableName("NOMDI") == "1")
                    {
                        queueThickness.Enqueue(bt);
                    }
                    else if (plc.ReadByVariableName("NOMDI") == "1")
                    {
                        queueMDIStation1.Enqueue(bt);
                    }
                    else
                    {
                        queueOCVStation1.Enqueue(bt);
                    }
                    break;

                case "OCV2":

                    if (plc.ReadByVariableName("NOXRAY") == "1" && plc.ReadByVariableName("NOPPG") == "1" && plc.ReadByVariableName("NOMDI") == "1")
                    {
                        queueXRAY2Station.Enqueue(bt);
                    }
                    else if (plc.ReadByVariableName("NOPPG") == "1" && plc.ReadByVariableName("NOMDI") == "1")
                    {
                        queueThickness.Enqueue(bt);
                    }
                    else if (plc.ReadByVariableName("NOMDI") == "1")
                    {
                        queueMDIStation2.Enqueue(bt);
                    }
                    else
                    {
                        queueOCVStation2.Enqueue(bt);
                    }
                    break;

                case "MDI1":

                    if (plc.ReadByVariableName("NOXRAY") == "1" && plc.ReadByVariableName("NOPPG") == "1")
                    {
                        queueXRAY2Station.Enqueue(bt);
                    }
                    else if (plc.ReadByVariableName("NOPPG") == "1")
                    {
                        queueThickness.Enqueue(bt);
                    }
                    else
                    {
                        queueMDIStation1.Enqueue(bt);
                    }
                    break;

                case "MDI2":

                    if (plc.ReadByVariableName("NOXRAY") == "1" && plc.ReadByVariableName("NOPPG") == "1")
                    {
                        queueXRAY2Station.Enqueue(bt);
                    }
                    else if (plc.ReadByVariableName("NOPPG") == "1")
                    {
                        queueThickness.Enqueue(bt);
                    }
                    else
                    {
                        queueMDIStation2.Enqueue(bt);
                    }
                    break;

                case "Thickness1":
                case "Thickness2":

                    if (plc.ReadByVariableName("NOXRAY") == "1")
                    {
                        queueXRAY2Station.Enqueue(bt);
                    }
                    else
                    {
                        queueThickness.Enqueue(bt);
                    }
                    break;

            }
        }


        private static object objLockEnter = new object();
        private void EnterQueue(BatterySeat bt, string currentStation)
        {
            lock (objLockEnter)
            {
                //此处NDI动作不会屏蔽，所以必做MDI动作
                switch (currentStation)
                {
                    case "BarcodeBinding1":
                        if (plc.ReadByVariableName("NOOCV") == "1" && plc.ReadByVariableName("NOIV") == "1")
                        {
                            queueOCVStation1.Enqueue(bt);
                        }
                        else if (plc.ReadByVariableName("NOIV") == "1")
                        {
                            queueIVStation1.Enqueue(bt);
                        }
                        else
                        {
                            queueScanStation1.Enqueue(bt);
                        }
                        break;

                    case "BarcodeBinding2":
                        if (plc.ReadByVariableName("NOOCV") == "1" && plc.ReadByVariableName("NOIV") == "1")
                        {
                            queueOCVStation2.Enqueue(bt);
                        }
                        else if (plc.ReadByVariableName("NOIV") == "1")
                        {
                            queueIVStation2.Enqueue(bt);
                        }
                        else
                        {
                            queueScanStation2.Enqueue(bt);
                        }
                        break;

                    case "IV1":
                    case "IV3":
                        if (plc.ReadByVariableName("NOOCV") == "1")
                        {
                            queueOCVStation1.Enqueue(bt);
                        }
                        else
                        {
                            queueIVStation1.Enqueue(bt);
                        }
                        break;

                    case "IV2":
                    case "IV4":
                        if (plc.ReadByVariableName("NOOCV") == "1")
                        {
                            queueOCVStation2.Enqueue(bt);
                        }
                        else
                        {
                            queueIVStation2.Enqueue(bt);
                        }
                        break;

                    case "OCV1":

                        queueOCVStation1.Enqueue(bt);
                        break;

                    case "OCV2":

                        queueOCVStation2.Enqueue(bt);
                        break;

                    case "MDI1":

                        if (plc.ReadByVariableName("NOXRAY") == "1" && plc.ReadByVariableName("NOPPG") == "1")
                        {
                            queueXRAY2Station.Enqueue(bt);
                        }
                        else if (plc.ReadByVariableName("NOPPG") == "1")
                        {
                            queueThickness.Enqueue(bt);
                        }
                        else
                        {
                            queueMDIStation1.Enqueue(bt);
                        }
                        break;

                    case "MDI2":

                        if (plc.ReadByVariableName("NOXRAY") == "1" && plc.ReadByVariableName("NOPPG") == "1")
                        {
                            queueXRAY2Station.Enqueue(bt);
                        }
                        else if (plc.ReadByVariableName("NOPPG") == "1")
                        {
                            queueThickness.Enqueue(bt);
                        }
                        else
                        {
                            queueMDIStation2.Enqueue(bt);
                        }
                        break;

                    case "Thickness1":
                    case "Thickness2":

                        if (plc.ReadByVariableName("NOXRAY") == "1")
                        {
                            queueXRAY2Station.Enqueue(bt);
                        }
                        else
                        {
                            queueThickness.Enqueue(bt);
                        }
                        break;

                }
            }
        }


        public bool CheckMDI()
        {
            try
            {
                DateTime dayWorkStart = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd") + " " + UserDefineVariableInfo.DicVariables["dayShiftStartTime"]);
                DateTime dayWorkEnd = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd") + " " + UserDefineVariableInfo.DicVariables["nightShiftStartTime"]);

                bool isChecked = false;
                string filePath = "";
                string sourceData = "";
                int checkCount = 3;
                List<Check> list = new List<Check>();
                //尺寸
                if (DateTime.Now >= dayWorkStart && DateTime.Now <= dayWorkEnd)
                {
                    //白班
                    filePath = "D:\\测量数据\\点检数据\\尺寸测量标块点检\\" + ATL.Core.DeivceEquipmentidPlcInfo.lstDeivceEquipmentidPlcInfos[0].ModelInfo + "\\FQI_" + DateTime.Now.ToString("yyyyMMdd") + ".csv";
                }
                else
                {
                    if (DateTime.Now < dayWorkStart)
                    {
                        //第二天的夜班还没下班
                        filePath = "D:\\测量数据\\点检数据\\尺寸测量标块点检\\" + ATL.Core.DeivceEquipmentidPlcInfo.lstDeivceEquipmentidPlcInfos[0].ModelInfo + "\\FQI_" + DateTime.Now.ToString("yyyyMMdd") + ".csv";
                        if (!File.Exists(filePath))
                        {
                            filePath = "D:\\测量数据\\点检数据\\尺寸测量标块点检\\" + ATL.Core.DeivceEquipmentidPlcInfo.lstDeivceEquipmentidPlcInfos[0].ModelInfo + "\\FQI_" + DateTime.Now.AddDays(-1).ToString("yyyyMMdd") + ".csv";
                        }
                    }
                    else
                    {
                        filePath = "D:\\测量数据\\点检数据\\尺寸测量标块点检\\" + ATL.Core.DeivceEquipmentidPlcInfo.lstDeivceEquipmentidPlcInfos[0].ModelInfo + "\\FQI_" + DateTime.Now.ToString("yyyyMMdd") + ".csv";
                    }
                }

                if (File.Exists(filePath))
                {
                    using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                    {
                        byte[] buffer = new byte[1024 * 1024 * 5];
                        int readCount = fs.Read(buffer, 0, buffer.Length);
                        sourceData = Encoding.UTF8.GetString(buffer, 0, readCount);
                        sourceData = sourceData.Replace("\r\n", "!");
                    }
                }
                if (sourceData.Contains("!"))
                {
                    string[] lines = sourceData.Split('!');
                    for (int i = 1; i < lines.Length; i++)
                    {
                        //"编号,点检时间,标定块,电池长度,电池宽度,左极耳边距,右极耳边距,电池总长度,左1小白胶,左2小白胶,右1小白胶,右2小白胶,左极耳长,右极耳长,结果\r\n";
                        if (lines[i].Contains(","))
                        {
                            string[] arr = lines[i].Split(',');
                            Check check = new Check();
                            check.CheckTime = arr[1];
                            check.Modle = arr[2];
                            check.Result = arr[arr.Length - 1];
                            list.Add(check);
                        }
                    }

                    int smallOKCount = 0;
                    int midleOKCount = 0;
                    int bigOKCount = 0;
                    if (list.Count > 0)
                    {
                        for (int i = 0; i < list.Count; i++)
                        {
                            if (DateTime.Now >= dayWorkStart && DateTime.Now <= dayWorkEnd)//白班
                            {
                                if (Convert.ToDateTime(list[i].CheckTime) >= dayWorkStart &&
                                    Convert.ToDateTime(list[i].CheckTime) <= dayWorkEnd)
                                {
                                    if (list[i].Modle.Contains("小") && list[i].Result == "OK")
                                    {
                                        smallOKCount++;
                                    }
                                    if (list[i].Modle.Contains("中") && list[i].Result == "OK")
                                    {
                                        midleOKCount++;
                                    }
                                    if (list[i].Modle.Contains("大") && list[i].Result == "OK")
                                    {
                                        bigOKCount++;
                                    }
                                }
                            }
                            else//夜班
                            {
                                if (DateTime.Now < dayWorkStart) //第二天的夜班还没下班
                                {
                                    if (Convert.ToDateTime(list[i].CheckTime) > dayWorkEnd.AddDays(-1))
                                    {
                                        if (list[i].Modle.Contains("小") && list[i].Result == "OK")
                                        {
                                            smallOKCount++;
                                        }
                                        if (list[i].Modle.Contains("中") && list[i].Result == "OK")
                                        {
                                            midleOKCount++;
                                        }
                                        if (list[i].Modle.Contains("大") && list[i].Result == "OK")
                                        {
                                            bigOKCount++;
                                        }
                                    }
                                }
                                if (DateTime.Now > dayWorkEnd) //当天夜班
                                {
                                    if (Convert.ToDateTime(list[i].CheckTime) > dayWorkEnd)
                                    {
                                        if (list[i].Modle.Contains("小") && list[i].Result == "OK")
                                        {
                                            smallOKCount++;
                                        }
                                        if (list[i].Modle.Contains("中") && list[i].Result == "OK")
                                        {
                                            midleOKCount++;
                                        }
                                        if (list[i].Modle.Contains("大") && list[i].Result == "OK")
                                        {
                                            bigOKCount++;
                                        }
                                    }
                                }

                            }
                        }

                        if (smallOKCount >= checkCount && midleOKCount >= checkCount && bigOKCount >= checkCount)
                        {
                            isChecked = true;
                        }
                        else
                        {
                            isChecked = false;
                        }
                    }
                }

                return isChecked;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "提示", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Warning,
                        System.Windows.Forms.MessageBoxDefaultButton.Button1, System.Windows.Forms.MessageBoxOptions.ServiceNotification);
                return false;
            }
        }

        public bool CheckPPG()
        {

            try
            {
                DateTime dayWorkStart = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd") + " " + UserDefineVariableInfo.DicVariables["dayShiftStartTime"]);
                DateTime dayWorkEnd = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd") + " " + UserDefineVariableInfo.DicVariables["nightShiftStartTime"]);

                bool isChecked = false;
                string filePath = "";
                string sourceData = "";
                int checkCount = 3;
                List<Check> list = new List<Check>();
                //尺寸
                if (DateTime.Now >= dayWorkStart && DateTime.Now <= dayWorkEnd)
                {
                    //白班
                    filePath = "D:\\测量数据\\点检数据\\PPG标块点检\\" + ATL.Core.DeivceEquipmentidPlcInfo.lstDeivceEquipmentidPlcInfos[0].ModelInfo + "\\PPG_" + DateTime.Now.ToString("yyyyMMdd") + ".csv";
                }
                else
                {
                    if (DateTime.Now < dayWorkStart)
                    {
                        //第二天的夜班还没下班
                        filePath = "D:\\测量数据\\点检数据\\PPG标块点检\\" + ATL.Core.DeivceEquipmentidPlcInfo.lstDeivceEquipmentidPlcInfos[0].ModelInfo + "\\PPG_" + DateTime.Now.ToString("yyyyMMdd") + ".csv";
                        if (!File.Exists(filePath))
                        {
                            filePath = "D:\\测量数据\\点检数据\\PPG标块点检\\" + ATL.Core.DeivceEquipmentidPlcInfo.lstDeivceEquipmentidPlcInfos[0].ModelInfo + "\\PPG_" + DateTime.Now.AddDays(-1).ToString("yyyyMMdd") + ".csv";
                        }
                    }
                    else
                    {
                        filePath = "D:\\测量数据\\点检数据\\PPG标块点检\\" + ATL.Core.DeivceEquipmentidPlcInfo.lstDeivceEquipmentidPlcInfos[0].ModelInfo + "\\PPG_" + DateTime.Now.ToString("yyyyMMdd") + ".csv";
                    }
                }

                if (File.Exists(filePath))
                {
                    using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                    {
                        byte[] buffer = new byte[1024 * 1024 * 5];
                        int readCount = fs.Read(buffer, 0, buffer.Length);
                        sourceData = Encoding.UTF8.GetString(buffer, 0, readCount);
                        sourceData = sourceData.Replace("\r\n", "!");
                    }
                }
                if (sourceData.Contains("!"))
                {
                    string[] lines = sourceData.Split('!');
                    for (int i = 1; i < lines.Length; i++)
                    {
                        //"编号,点检时间,标定块,工位,厚度值,结果\r\n";
                        if (lines[i].Contains(","))
                        {
                            string[] arr = lines[i].Split(',');
                            Check check = new Check();
                            check.CheckTime = arr[1];
                            check.Modle = arr[2];
                            check.Result = arr[arr.Length - 1];
                            check.Poisition = arr[3];
                            list.Add(check);
                        }
                    }

                    int smallOKCount1 = 0;
                    int midleOKCount1 = 0;
                    int bigOKCount1 = 0;
                    int smallOKCount2 = 0;
                    int midleOKCount2 = 0;
                    int bigOKCount2 = 0;
                    int smallOKCount3 = 0;
                    int midleOKCount3 = 0;
                    int bigOKCount3 = 0;

                    if (list.Count > 0)
                    {
                        for (int i = 0; i < list.Count; i++)
                        {
                            if (DateTime.Now >= dayWorkStart && DateTime.Now <= dayWorkEnd) //白班
                            {
                                if (Convert.ToDateTime(list[i].CheckTime) >= dayWorkStart &&
                                    Convert.ToDateTime(list[i].CheckTime) <= dayWorkEnd)
                                {
                                    if (list[i].Result == "OK" && list[i].Poisition == "1")
                                    {
                                        if (list[i].Modle.Contains("小"))
                                        {
                                            smallOKCount1++;
                                        }
                                        if (list[i].Modle.Contains("中"))
                                        {
                                            midleOKCount1++;
                                        }
                                        if (list[i].Modle.Contains("大"))
                                        {
                                            bigOKCount1++;
                                        }
                                    }

                                    if (list[i].Result == "OK" && list[i].Poisition == "2")
                                    {
                                        if (list[i].Modle.Contains("小"))
                                        {
                                            smallOKCount2++;
                                        }
                                        if (list[i].Modle.Contains("中"))
                                        {
                                            midleOKCount2++;
                                        }
                                        if (list[i].Modle.Contains("大"))
                                        {
                                            bigOKCount2++;
                                        }
                                    }

                                    if (list[i].Result == "OK" && list[i].Poisition == "3")
                                    {
                                        if (list[i].Modle.Contains("小"))
                                        {
                                            smallOKCount3++;
                                        }
                                        if (list[i].Modle.Contains("中"))
                                        {
                                            midleOKCount3++;
                                        }
                                        if (list[i].Modle.Contains("大"))
                                        {
                                            bigOKCount3++;
                                        }
                                    }

                                }
                            }
                            else //夜班
                            {
                                if (DateTime.Now < dayWorkStart) //第二天的夜班还没下班
                                {
                                    if (Convert.ToDateTime(list[i].CheckTime) > dayWorkEnd.AddDays(-1))
                                    {
                                        if (list[i].Result == "OK" && list[i].Poisition == "1")
                                        {
                                            if (list[i].Modle.Contains("小"))
                                            {
                                                smallOKCount1++;
                                            }
                                            if (list[i].Modle.Contains("中"))
                                            {
                                                midleOKCount1++;
                                            }
                                            if (list[i].Modle.Contains("大"))
                                            {
                                                bigOKCount1++;
                                            }
                                        }

                                        if (list[i].Result == "OK" && list[i].Poisition == "2")
                                        {
                                            if (list[i].Modle.Contains("小"))
                                            {
                                                smallOKCount2++;
                                            }
                                            if (list[i].Modle.Contains("中"))
                                            {
                                                midleOKCount2++;
                                            }
                                            if (list[i].Modle.Contains("大"))
                                            {
                                                bigOKCount2++;
                                            }
                                        }

                                        if (list[i].Result == "OK" && list[i].Poisition == "3")
                                        {
                                            if (list[i].Modle.Contains("小"))
                                            {
                                                smallOKCount3++;
                                            }
                                            if (list[i].Modle.Contains("中"))
                                            {
                                                midleOKCount3++;
                                            }
                                            if (list[i].Modle.Contains("大"))
                                            {
                                                bigOKCount3++;
                                            }
                                        }
                                    }
                                }
                                if (DateTime.Now > dayWorkEnd) //当天夜班
                                {
                                    if (Convert.ToDateTime(list[i].CheckTime) > dayWorkEnd)
                                    {
                                        if (list[i].Result == "OK" && list[i].Poisition == "1")
                                        {
                                            if (list[i].Modle.Contains("小"))
                                            {
                                                smallOKCount1++;
                                            }
                                            if (list[i].Modle.Contains("中"))
                                            {
                                                midleOKCount1++;
                                            }
                                            if (list[i].Modle.Contains("大"))
                                            {
                                                bigOKCount1++;
                                            }
                                        }

                                        if (list[i].Result == "OK" && list[i].Poisition == "2")
                                        {
                                            if (list[i].Modle.Contains("小"))
                                            {
                                                smallOKCount2++;
                                            }
                                            if (list[i].Modle.Contains("中"))
                                            {
                                                midleOKCount2++;
                                            }
                                            if (list[i].Modle.Contains("大"))
                                            {
                                                bigOKCount2++;
                                            }
                                        }

                                        if (list[i].Result == "OK" && list[i].Poisition == "3")
                                        {
                                            if (list[i].Modle.Contains("小"))
                                            {
                                                smallOKCount3++;
                                            }
                                            if (list[i].Modle.Contains("中"))
                                            {
                                                midleOKCount3++;
                                            }
                                            if (list[i].Modle.Contains("大"))
                                            {
                                                bigOKCount3++;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        if (smallOKCount1 >= checkCount && midleOKCount1 >= checkCount && bigOKCount1 >= checkCount && smallOKCount2 >= checkCount && midleOKCount2 >= checkCount && bigOKCount2 >= checkCount
                                && smallOKCount3 >= checkCount && midleOKCount3 >= checkCount && bigOKCount3 >= checkCount)
                        {
                            isChecked = true;
                        }
                        else
                        {
                            isChecked = false;
                        }
                    }
                }

                return isChecked;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "提示", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Warning,
                        System.Windows.Forms.MessageBoxDefaultButton.Button1, System.Windows.Forms.MessageBoxOptions.ServiceNotification);
                return false;
            }
        }

        public bool CheckOCV()
        {
            try
            {
                DateTime dayWorkStart = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd") + " " + UserDefineVariableInfo.DicVariables["dayShiftStartTime"]);
                DateTime dayWorkEnd = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd") + " " + UserDefineVariableInfo.DicVariables["nightShiftStartTime"]);

                bool isChecked = false;
                string filePath = "";
                string sourceData = "";
                int checkCount = 1;
                List<Check> list = new List<Check>();

                if (DateTime.Now >= dayWorkStart && DateTime.Now <= dayWorkEnd)
                {
                    //白班
                    filePath = "D:\\测量数据\\点检数据\\OCV标块点检\\" + ATL.Core.DeivceEquipmentidPlcInfo.lstDeivceEquipmentidPlcInfos[0].ModelInfo + "\\OCV_" + DateTime.Now.ToString("yyyyMMdd") + ".csv";
                }
                else
                {
                    if (DateTime.Now < dayWorkStart)
                    {
                        //第二天的夜班还没下班
                        filePath = "D:\\测量数据\\点检数据\\OCV标块点检\\" + ATL.Core.DeivceEquipmentidPlcInfo.lstDeivceEquipmentidPlcInfos[0].ModelInfo + "\\OCV_" + DateTime.Now.ToString("yyyyMMdd") + ".csv";
                        if (!File.Exists(filePath))
                        {
                            filePath = "D:\\测量数据\\点检数据\\OCV标块点检\\" + ATL.Core.DeivceEquipmentidPlcInfo.lstDeivceEquipmentidPlcInfos[0].ModelInfo + "\\OCV_" + DateTime.Now.AddDays(-1).ToString("yyyyMMdd") + ".csv";
                        }
                    }
                    else
                    {
                        filePath = "D:\\测量数据\\点检数据\\OCV标块点检\\" + ATL.Core.DeivceEquipmentidPlcInfo.lstDeivceEquipmentidPlcInfos[0].ModelInfo + "\\OCV_" + DateTime.Now.ToString("yyyyMMdd") + ".csv";
                    }
                }

                if (File.Exists(filePath))
                {
                    using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                    {
                        byte[] buffer = new byte[1024 * 1024 * 5];
                        int readCount = fs.Read(buffer, 0, buffer.Length);
                        sourceData = Encoding.UTF8.GetString(buffer, 0, readCount);
                        sourceData = sourceData.Replace("\r\n", "!");
                    }
                }
                if (sourceData.Contains("!"))
                {
                    string[] lines = sourceData.Split('!');
                    for (int i = 1; i < lines.Length; i++)
                    {
                        //"点检时间,值,结果,类型,工位\r\n";
                        if (lines[i].Contains(","))
                        {
                            string[] arr = lines[i].Split(',');
                            Check check = new Check();
                            check.CheckTime = arr[0];
                            check.Result = arr[2];
                            check.Modle = arr[3];
                            check.Poisition = arr[arr.Length - 1];
                            list.Add(check);
                        }
                    }

                    int volOKCount1 = 0;
                    int volOKCount2 = 0;
                    int resOKCount1 = 0;
                    int resOKCount2 = 0;

                    if (list.Count > 0)
                    {
                        for (int i = 0; i < list.Count; i++)
                        {
                            if (DateTime.Now >= dayWorkStart && DateTime.Now <= dayWorkEnd) //白班
                            {
                                if (Convert.ToDateTime(list[i].CheckTime) >= dayWorkStart &&
                                    Convert.ToDateTime(list[i].CheckTime) <= dayWorkEnd)
                                {
                                    if (list[i].Modle.Contains("电压") && list[i].Result == "OK" && list[i].Poisition == "1")
                                    {
                                        volOKCount1++;
                                    }
                                    if (list[i].Modle.Contains("电压") && list[i].Result == "OK" && list[i].Poisition == "2")
                                    {
                                        volOKCount2++;
                                    }
                                    if (list[i].Modle.Contains("内阻") && list[i].Result == "OK" && list[i].Poisition == "1")
                                    {
                                        resOKCount1++;
                                    }
                                    if (list[i].Modle.Contains("内阻") && list[i].Result == "OK" && list[i].Poisition == "2")
                                    {
                                        resOKCount2++;
                                    }
                                }
                            }
                            else//夜班
                            {
                                if (DateTime.Now < dayWorkStart) //第二天的夜班还没下班
                                {
                                    if (Convert.ToDateTime(list[i].CheckTime) > dayWorkEnd.AddDays(-1))
                                    {
                                        if (list[i].Modle.Contains("电压") && list[i].Result == "OK" && list[i].Poisition == "1")
                                        {
                                            volOKCount1++;
                                        }
                                        if (list[i].Modle.Contains("电压") && list[i].Result == "OK" && list[i].Poisition == "2")
                                        {
                                            volOKCount2++;
                                        }
                                        if (list[i].Modle.Contains("内阻") && list[i].Result == "OK" && list[i].Poisition == "1")
                                        {
                                            resOKCount1++;
                                        }
                                        if (list[i].Modle.Contains("内阻") && list[i].Result == "OK" && list[i].Poisition == "2")
                                        {
                                            resOKCount2++;
                                        }
                                    }
                                }
                                if (DateTime.Now > dayWorkEnd) //当天夜班
                                {
                                    if (Convert.ToDateTime(list[i].CheckTime) > dayWorkEnd)
                                    {
                                        if (list[i].Modle.Contains("电压") && list[i].Result == "OK" && list[i].Poisition == "1")
                                        {
                                            volOKCount1++;
                                        }
                                        if (list[i].Modle.Contains("电压") && list[i].Result == "OK" && list[i].Poisition == "2")
                                        {
                                            volOKCount2++;
                                        }
                                        if (list[i].Modle.Contains("内阻") && list[i].Result == "OK" && list[i].Poisition == "1")
                                        {
                                            resOKCount1++;
                                        }
                                        if (list[i].Modle.Contains("内阻") && list[i].Result == "OK" && list[i].Poisition == "2")
                                        {
                                            resOKCount2++;
                                        }
                                    }
                                }

                            }
                        }

                        if (volOKCount1 >= checkCount && volOKCount2 >= checkCount && resOKCount1 >= checkCount && resOKCount2 >= checkCount)
                        {
                            isChecked = true;
                        }
                        else
                        {
                            isChecked = false;
                        }
                    }
                }

                return isChecked;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "提示", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Warning,
                        System.Windows.Forms.MessageBoxDefaultButton.Button1, System.Windows.Forms.MessageBoxOptions.ServiceNotification);
                return false;
            }
        }

        public bool CheckCanDo(string checkTime)
        {
            try
            {
                //DateTime dtStart1 = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd") + " 07:30:00");
                //DateTime dtStart2 = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd") + " 19:30:00");
                //DateTime dtEnd1 = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd") + " 08:30:00");
                //DateTime dtEnd2 = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd") + " 20:30:00");
                //return true;

                if (XRayTubeIdleTime == 9999)
                {
                    return true;
                }

                DateTime lastCheckTime = Convert.ToDateTime(checkTime);
                DateTime dayWorkStart = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd") + " " + UserDefineVariableInfo.DicVariables["dayShiftStartTime"]);
                DateTime dayWorkEnd = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd") + " " + UserDefineVariableInfo.DicVariables["nightShiftStartTime"]);

                //早晚七点半到八点半可以直接开机
                if ((DateTime.Now > dayWorkStart && DateTime.Now < dayWorkStart.AddHours(1)) || (DateTime.Now > dayWorkEnd && DateTime.Now < dayWorkEnd.AddHours(1)))
                {
                    return true;
                }

                if (DateTime.Now >= dayWorkStart && DateTime.Now <= dayWorkEnd) //白班
                {
                    if (lastCheckTime >= dayWorkStart && lastCheckTime <= dayWorkEnd)
                    {
                        return true;
                    }
                }
                else
                {
                    if (DateTime.Now < dayWorkStart)//第二天的夜班还没下班
                    {
                        if (lastCheckTime > dayWorkEnd.AddDays(-1))
                        {
                            return true;
                        }
                    }
                    if (DateTime.Now > dayWorkEnd)//当天夜班
                    {
                        if (lastCheckTime > dayWorkEnd)
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "提示", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Warning,
                        System.Windows.Forms.MessageBoxDefaultButton.Button1, System.Windows.Forms.MessageBoxOptions.ServiceNotification);
                return false;
            }
        }

        private static Queue<string> queueXrayCheckBarcode = new Queue<string>();
        /// <summary>
        /// Xray点检扫码
        /// </summary>
        private void XrayCheckScan()
        {
            int scanStep = 0;
            string barcode = "";
            bool isMaster = false;
            queueXrayCheckBarcode.Clear();
            while (true)
            {
                Thread.Sleep(10);
                if (plc.ReadByVariableName("SoftwareReset") == "1")
                {
                    scanStep = 0;
                    queueXrayCheckBarcode.Clear();
                }
                if (plc.ReadByVariableName("TestMode") == "9")
                {
                    if (plc.ReadByVariableName("SingleScanerMode") != "1")
                    {
                        switch (scanStep)
                        {
                            case 0:

                                if (plc.ReadByVariableName("ScanBarcodeSignal1") == "1")
                                {
                                    WriteLog("扫码枪1开始扫码", LogLevels.Info, "XrayCheckScan");
                                    Thread.Sleep(HardwareConfig.Instance.ScanBarcodeDelay);
                                    barcode = "";
                                    isMaster = false;
                                    CodeReaderIF.ClientSendMsg("LON\r", 1);
                                    Thread.Sleep(200);
                                    CodeReaderIF.ClientSendMsg("LOFF\r", 1);
                                    barcode = CodeReaderIF.ClientReceiveMsg(1);
                                    WriteLog("条码：" + barcode, LogLevels.Info, "XrayCheckScan");
                                    foreach (var temp in testCodeManager.CodeList)
                                    {
                                        if (barcode == temp.BarCode)
                                        {
                                            isMaster = true;
                                            break;
                                        }
                                    }

                                    if (isMaster == true)
                                    {
                                        plc.WriteByVariableName("BatteryScanning1OK", 1);
                                        queueXrayCheckBarcode.Enqueue(barcode);
                                    }
                                    else
                                    {
                                        WriteLog("条码：" + barcode + " 不是Master", LogLevels.Info, "XrayCheckScan");
                                        plc.WriteByVariableName("BatteryScanning1NG", 1);
                                    }
                                    Thread.Sleep(1000);
                                    plc.WriteByVariableName("GetTempBarcodeComplete1", 1);
                                    scanStep = 1;
                                }
                                break;

                            case 1:

                                if (plc.ReadByVariableName("ScanBarcodeSignal1") == "0" && plc.ReadByVariableName("GetTempBarodeSignal1") == "0")
                                {
                                    WriteLog("扫码枪1扫码完成", LogLevels.Info, "XrayCheckScan");
                                    plc.WriteByVariableName("BatteryScanning1OK", 0);
                                    plc.WriteByVariableName("BatteryScanning1NG", 0);
                                    plc.WriteByVariableName("GetTempBarcodeComplete1", 0);
                                    scanStep = 2;
                                }
                                break;

                            case 2:

                                if (plc.ReadByVariableName("ScanBarcodeSignal2") == "1")
                                {
                                    WriteLog("扫码枪2开始扫码", LogLevels.Info, "XrayCheckScan");
                                    Thread.Sleep(HardwareConfig.Instance.ScanBarcodeDelay);
                                    barcode = "";
                                    isMaster = false;
                                    CodeReaderIF.ClientSendMsg("LON\r", 3);
                                    Thread.Sleep(200);
                                    CodeReaderIF.ClientSendMsg("LOFF\r", 3);
                                    barcode = CodeReaderIF.ClientReceiveMsg(3);
                                    WriteLog("条码：" + barcode, LogLevels.Info, "XrayCheckScan");
                                    foreach (var temp in testCodeManager.CodeList)
                                    {
                                        if (barcode == temp.BarCode)
                                        {
                                            isMaster = true;
                                            break;
                                        }
                                    }

                                    if (isMaster == true)
                                    {
                                        plc.WriteByVariableName("BatteryScanning2OK", 1);
                                        queueXrayCheckBarcode.Enqueue(barcode);
                                    }
                                    else
                                    {
                                        WriteLog("条码：" + barcode + " 不是Master", LogLevels.Info, "XrayCheckScan");
                                        plc.WriteByVariableName("BatteryScanning2NG", 1);
                                    }
                                    Thread.Sleep(1000);
                                    plc.WriteByVariableName("GetTempBarcodeComplete2", 1);
                                    scanStep = 3;
                                }
                                break;

                            case 3:

                                if (plc.ReadByVariableName("ScanBarcodeSignal2") == "0" && plc.ReadByVariableName("GetTempBarodeSignal2") == "0")
                                {
                                    WriteLog("扫码枪2扫码完成", LogLevels.Info, "XrayCheckScan");
                                    plc.WriteByVariableName("BatteryScanning2OK", 0);
                                    plc.WriteByVariableName("BatteryScanning2NG", 0);
                                    plc.WriteByVariableName("GetTempBarcodeComplete2", 0);
                                    scanStep = 0;
                                }
                                break;


                        }
                    }
                    else
                    {
                        switch (scanStep)
                        {
                            case 0:

                                if (plc.ReadByVariableName("ScanBarcodeSignal2") == "1")
                                {
                                    WriteLog("扫码枪2开始扫码", LogLevels.Info, "XrayCheckScan");
                                    Thread.Sleep(HardwareConfig.Instance.ScanBarcodeDelay);
                                    barcode = "";
                                    isMaster = false;
                                    CodeReaderIF.ClientSendMsg("LON\r", 3);
                                    Thread.Sleep(200);
                                    CodeReaderIF.ClientSendMsg("LOFF\r", 3);
                                    barcode = CodeReaderIF.ClientReceiveMsg(3);
                                    WriteLog("条码：" + barcode, LogLevels.Info, "XrayCheckScan");
                                    foreach (var temp in testCodeManager.CodeList)
                                    {
                                        if (barcode == temp.BarCode)
                                        {
                                            isMaster = true;
                                            break;
                                        }
                                    }

                                    if (isMaster == true)
                                    {
                                        plc.WriteByVariableName("BatteryScanning2OK", 1);
                                        queueXrayCheckBarcode.Enqueue(barcode);
                                    }
                                    else
                                    {
                                        WriteLog("条码：" + barcode + " 不是Master", LogLevels.Info, "XrayCheckScan");
                                        plc.WriteByVariableName("BatteryScanning2NG", 1);
                                    }
                                    Thread.Sleep(1000);
                                    plc.WriteByVariableName("GetTempBarcodeComplete2", 1);
                                    scanStep = 1;
                                }
                                break;

                            case 1:

                                if (plc.ReadByVariableName("ScanBarcodeSignal2") == "0" && plc.ReadByVariableName("GetTempBarodeSignal2") == "0")
                                {
                                    WriteLog("扫码枪2扫码完成", LogLevels.Info, "XrayCheckScan");
                                    plc.WriteByVariableName("BatteryScanning2OK", 0);
                                    plc.WriteByVariableName("BatteryScanning2NG", 0);
                                    plc.WriteByVariableName("GetTempBarcodeComplete2", 0);
                                    scanStep = 0;
                                }
                                break;
                        }

                    }
                }

            }
        }

        /// <summary>
        /// XRAY点检
        /// </summary>
        private void XrayCheck()
        {
            int xrayStep = 0;
            int index = 0;
            int ngCount = 0;
            BatterySeat bt = new BatterySeat();
            while (true)
            {
                Thread.Sleep(10);
                if (plc.ReadByVariableName("SoftwareReset") == "1")
                {
                    index = 0;
                    ngCount = 0;
                    xrayStep = 0;
                }

                if (plc.ReadByVariableName("TestMode") == "9")
                {
                    switch (xrayStep)
                    {
                        case 0:

                            if (plc.ReadByVariableName("Battery1CornerPhotoSignal") == "1")
                            {
                                bt = new BatterySeat();
                                _workingSeats._seat1.CopyTo(ref bt);
                                bt.Sn = queueXrayCheckBarcode.Dequeue();
                                bt.CheckExtension = ECheckExtensions.Local;//本地模式不保存数据到数据库
                                Thread.Sleep(HardwareConfig.Instance.CameraShotDelay4);

                                //TODO: 替换旧版采图 ZhangKF 2021-3-16
                                //ZYImageStruct img4 = bt.Corner4.ShotImage;
                                try
                                {
                                    index++;
                                    int k = 0;
                                    int shotTime = Environment.TickCount;

                                    //TODO: 替换旧版采图 ZhangKF 2021-3-16
                                    //int iRet4 = VisionSysWrapperIF.ShotAndAverage(ECamTypes.Camera1, _camParam1.PinValue, ref img4, 4, out k);
                                    //bt.Corner4.ImgNO = k;
                                    var result = CameraHelper.CaptureOneImage(0);

                                    WriteLog(string.Format("第{0}次 角一采图 保存图片, 16位图编号：{1} {2} 耗时 {3} 毫秒", index, bt.Corner4.ImgNO, k, Environment.TickCount - shotTime), LogLevels.Info, "XrayCheckPage");
                                    string picname = "D:\\Test\\4_" + bt.Sn + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + index + ".jpg";
                                    //img4.Save(picname, 1, bt.Corner4.ImgNO);
                                    //if (iRet4 == -1)
                                    if (!result.Result)
                                    {
                                        bt.Corner4.IsShotOK = false;
                                        WriteLog(string.Format("第{0}次 角一采图失败", index), LogLevels.Info, "XrayCheckPage");
                                    }
                                    else
                                    {
                                        bt.Corner4.ShotImage = result.Bitmap.ToTransfor(Consts.ImageTypes.Sixteen);
                                        bt.Corner4.IsShotOK = true;
                                    }
                                    m_bAlgoCorner4Finished = false;
                                    TestCorner4(ref bt, index);
                                    plc.WriteByVariableName("Battery1CornerPhotoComplete", 1);
                                    xrayStep = 1;
                                }
                                catch (Exception ex)
                                {
                                    WriteLog(string.Format("第{0}次 角一采图失败 {1}", index, ex), LogLevels.Error, "XrayCheckPage");
                                    Thread.Sleep(3000);
                                    continue;
                                }
                            }

                            break;

                        case 1:

                            if (plc.ReadByVariableName("Battery1CornerPhotoSignal") == "0")
                            {
                                WriteLog(string.Format("第{0}次 角一拍照完成 ", index), LogLevels.Info, "Task_XrayShot1");
                                plc.WriteByVariableName("Battery1CornerPhotoComplete", 0);
                                xrayStep = 2;
                            }
                            break;

                        case 2:

                            if (plc.ReadByVariableName("Battery2CornerPhotoSignal") == "1")
                            {
                                Thread.Sleep(HardwareConfig.Instance.CameraShotDelay3);

                                //TODO: 替换旧版采图 ZhangKF 2021-3-16
                                //ZYImageStruct img3 = bt.Corner3.ShotImage;
                                int shotTime = Environment.TickCount;
                                int k = 0;
                                //int iRet3 = VisionSysWrapperIF.ShotAndAverage(ECamTypes.Camera1, _camParam1.PinValue, ref img3, 4, out k);//最后一个参数：4 jpg 3 tiff
                                //bt.Corner3.ImgNO = k;
                                WriteLog(string.Format("第{0}次 角二采图 保存图片, 16位图编号：{1} {2} 耗时 {3} 毫秒", index, bt.Corner3.ImgNO, k, Environment.TickCount - shotTime), LogLevels.Info, "Task_XrayShot1");
                                string picname = "D:\\Test\\3_" + bt.Sn + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + index + ".jpg";
                                //img3.Save(picname, 1, bt.Corner3.ImgNO);
                                //if (iRet3 == -1)
                                var result = CameraHelper.CaptureOneImage(0);
                                if (!result.Result)
                                {
                                    bt.Corner3.IsShotOK = false;
                                    WriteLog(string.Format("第{0}次 角二采图失败,iRet2", index), LogLevels.Info, "Task_XrayShot1");
                                }
                                else
                                {
                                    bt.Corner3.ShotImage = result.Bitmap.ToTransfor(Consts.ImageTypes.Sixteen);
                                    bt.Corner3.IsShotOK = true;
                                }
                                m_bAlgoCorner3Finished = false;
                                TestCorner3(ref bt, index);
                                plc.WriteByVariableName("Battery2CornerPhotoComplete", 1);
                                xrayStep = 3;
                            }
                            break;

                        case 3:

                            if (plc.ReadByVariableName("Battery2CornerPhotoSignal") == "0")
                            {
                                WriteLog(string.Format("第{0}次 角二拍照完成 ", index), LogLevels.Info, "Task_XrayShot1");
                                plc.WriteByVariableName("Battery2CornerPhotoComplete", 0);
                                xrayStep = 4;
                            }
                            break;

                        case 4:

                            if (plc.ReadByVariableName("Battery3CornerPhotoSignal") == "1")
                            {
                                Thread.Sleep(HardwareConfig.Instance.CameraShotDelay1);

                                //TODO: 替换旧版采图 ZhangKF 2021-3-16
                                //ZYImageStruct img1 = bt.Corner2.ShotImage;
                                int shotTime = Environment.TickCount;
                                //int k = 0;
                                //int iRet1 = VisionSysWrapperIF.ShotAndAverage(ECamTypes.Camera2, _camParam2.PinValue, ref img1, 4, out k);
                                //bt.Corner2.ImgNO = k;
                                WriteLog(string.Format("第{0}次 角三采图 保存图片, 16位图编号：{1} {2} 耗时 {3} 毫秒", index, bt.Corner2.ImgNO, 0, Environment.TickCount - shotTime), LogLevels.Info, "Task_XrayShot2");
                                string picname = "D:\\Test\\2_" + bt.Sn + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + index + ".jpg";
                                //img1.Save(picname, 1, bt.Corner2.ImgNO);
                                var result = CameraHelper.CaptureOneImage(1);
                                //if (iRet1 == -1)
                                if (result.Result)
                                {
                                    bt.Corner2.IsShotOK = false;
                                    WriteLog(string.Format("第{0}次 角三采图失败", index), LogLevels.Info, "Task_XrayShot2");
                                }
                                else
                                {
                                    bt.Corner2.ShotImage = result.Bitmap.ToTransfor(Consts.ImageTypes.Sixteen);
                                    bt.Corner2.IsShotOK = true;
                                }
                                m_bAlgoCorner2Finished = false;
                                TestCorner2(ref bt, index);
                                plc.WriteByVariableName("Battery3CornerPhotoComplete", 1);
                                xrayStep = 5;
                            }
                            break;

                        case 5:

                            if (plc.ReadByVariableName("Battery3CornerPhotoSignal") == "0")
                            {
                                WriteLog(string.Format("第{0}次 角三拍照完成 ", index), LogLevels.Info, "Task_XrayShot1");
                                plc.WriteByVariableName("Battery3CornerPhotoComplete", 0);
                                xrayStep = 6;
                            }
                            break;

                        case 6:

                            if (plc.ReadByVariableName("Battery4CornerPhotoSignal") == "1")
                            {
                                Thread.Sleep(HardwareConfig.Instance.CameraShotDelay4);
                                //TODO: 替换旧版采图 ZhangKF 2021-3-16
                                //ZYImageStruct img2 = bt.Corner1.ShotImage;
                                int shotTime = Environment.TickCount;
                                //int k = 0;
                                //int iRet2 = VisionSysWrapperIF.ShotAndAverage(ECamTypes.Camera2, _camParam2.PinValue, ref img2, 4, out k);
                                //bt.Corner1.ImgNO = k;
                                WriteLog(string.Format("第{0}次 角四采图 保存图片, 16位图编号：{1} {2} {3} 毫秒", index, bt.Corner1.ImgNO, 0, Environment.TickCount - shotTime), LogLevels.Info, "Task_XrayShot2");
                                string picname = "D:\\Test\\1_" + bt.Sn + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + index + ".jpg";
                                //img2.Save(picname, 1, bt.Corner1.ImgNO);
                                var result = CameraHelper.CaptureOneImage(1);
                                //if (iRet2 == -1)
                                if (result.Result)
                                {
                                    bt.Corner1.IsShotOK = false;
                                    WriteLog(string.Format("第{0}次 角四采图失败", index), LogLevels.Info, "Task_XrayShot2");
                                }
                                else
                                {
                                    bt.Corner1.ShotImage = result.Bitmap.ToTransfor(Consts.ImageTypes.Sixteen);
                                    bt.Corner1.IsShotOK = true;
                                }
                                m_bAlgoCorner1Finished = false;
                                TestCorner1(ref bt, index);
                                xrayStep = 7;
                            }
                            break;

                        case 7:

                            if (plc.ReadByVariableName("Battery4CornerPhotoSignal") == "0")
                            {
                                WriteLog(string.Format("第{0}次 角四拍照完成 ", index), LogLevels.Info, "Task_XrayShot1");
                                plc.WriteByVariableName("Battery4CornerPhotoComplete", 0);
                                xrayStep = 8;
                            }
                            break;

                        case 8:

                            if (plc.ReadByVariableName("PhotoResultSignal") == "1")
                            {
                                bt.UpdateResult();
                                WriteLog(string.Format("第{0}次 Final result is {1} {2}-{3}: {4} {5} {6} {7} ",
                                    index,
                                    bt.Sn,
                                    bt.FinalResult,
                                    bt.ResultCode,
                                    bt.Corner1.InspectResults.resultDataMin.iResult,
                                    bt.Corner2.InspectResults.resultDataMin.iResult,
                                    bt.Corner3.InspectResults.resultDataMin.iResult,
                                    bt.Corner4.InspectResults.resultDataMin.iResult), LogLevels.Info, "Task_Sorting");

                                WriteLog(string.Format("第{0}次 合成结果图", index), LogLevels.Info, "Task_Sorting");
                                try
                                {
                                    AlgoWrapper.Instance.GetResultImage(ref bt);
                                }
                                catch (Exception ex)
                                {
                                    WriteLog(string.Format("第{0}次 合成结果图异常", index), LogLevels.Warn, "Task_Sorting");
                                }
                                WriteLog(string.Format("第{0}次 合成结果图完成", index), LogLevels.Info, "Task_Sorting");
                                ResultRelay.Instance.CheckResultHandlers(bt);//刷新监控状态页面
                                //位置错误或黑白图
                                if ((bt.ResultCode == EResultCodes.Unknow || bt.ResultCode == EResultCodes.AlgoErr))
                                {
                                    try
                                    {
                                        string path = "D:\\位置错误黑白图\\" + DateTime.Now.ToString("yyyyMM") + "\\" + DateTime.Now.Day + "\\" + DateTime.Now.Hour;
                                        string fileAllName = path + "\\" + bt.Sn + DateTime.Now.ToString("yyyyMMddHHmmss") + ".jpg";
                                        if (!Directory.Exists(path))
                                        {
                                            Directory.CreateDirectory(path);
                                            bt.ResultImage.Save(fileAllName);
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        WriteLog(string.Format("第{0}次 保存黑白图异常 {1}", index, ex), LogLevels.Error, "Task_Sorting");
                                    }
                                }
                                _extRunEmpty.OnCheckFinished(bt, _imageSaveConfig, ref _checkStatus);//XRayClient.Core\CheckLogic\Extensions\CheckLogicExtRunEmpty.cs
                                try
                                {
                                    _extSTF.OnSaveImages(bt, _imageSaveConfig);//XRayClient.Core\CheckLogic\Extensions\CheckLogicExtSTF.cs
                                }
                                catch (Exception ex)
                                {
                                    System.Windows.MessageBox.Show("磁盘已满，无法继续保存图片，请清理再启动!");
                                    bt.MesResult = false;
                                }

                                if (bt.ResultCode == EResultCodes.OK)
                                {
                                    ngCount = 0;
                                    plc.WriteByVariableName("ResultOK", 1);
                                    WriteLog(string.Format("第{0}次 发送PLC分拣 OK", index), LogLevels.Info, "Task_Sorting");
                                }
                                else
                                {
                                    ngCount++;
                                    plc.WriteByVariableName("XRAYNG", 1);
                                    WriteLog(string.Format("第{0}次 发送PLC分拣 XRAYNG", index), LogLevels.Info, "Task_Sorting");
                                }
                                if (ngCount == CheckParamsConfig.Instance.MyStartupTestConfig.TestNGNum)
                                {
                                    CheckParamsConfig.Instance.MyStartupTestConfig.lastTestTime = DateTime.Now;
                                    CheckParamsConfig.Instance.SaveStartupConfig();
                                }
                                bt.Destroy();
                                if (index == CheckParamsConfig.Instance.MyStartupTestConfig.TestNGNum)
                                {
                                    MessageBox.Show("XRAY连续" + CheckParamsConfig.Instance.MyStartupTestConfig.TestNGNum + "个master电池NG，点检成功", "提示", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Warning,
                        System.Windows.Forms.MessageBoxDefaultButton.Button1, System.Windows.Forms.MessageBoxOptions.ServiceNotification);
                                    ngCount = 0;
                                }
                                xrayStep = 0;

                            }
                            break;
                    }
                }

            }
        }

        /// <summary>
        /// XRAY点检过程中的FQI定位
        /// </summary>
        private void XrayCheckOnFQI()
        {
            int step1 = 0;
            int step2 = 0;
            int step3 = 0;
            int step4 = 0;

            while (true)
            {
                Thread.Sleep(10);
                if (plc.ReadByVariableName("SoftwareReset") == "1")
                {
                    step1 = 0;
                    step2 = 0;
                    step3 = 0;
                    step4 = 0;
                }
                if (plc.ReadByVariableName("TestMode") == "9")
                {
                    switch (step1)
                    {
                        case 0:

                            if (plc.ReadByVariableName("MDISignal1") == "1")
                            {
                                string cmd = "Run_ACQ2,abcdefg,1\r";
                                try
                                {
                                    //CodeReaderIF.ClientSendMsg(cmd, 2);
                                    vision.SendCommand(cmd);
                                }
                                catch (Exception ex)
                                {
                                    //CodeReaderIF.Init_ClientConnet(HardwareConfig.Instance.DimensionIPAdress, HardwareConfig.Instance.DimensionPort, 2);//尺寸测量连接
                                    vision.InitialShangLingVision(HardwareConfig.Instance.DimensionIPAdress, HardwareConfig.Instance.DimensionPort);
                                    Thread.Sleep(500);
                                    //CodeReaderIF.ClientSendMsg(cmd, 2);
                                    vision.SendCommand(cmd);
                                }
                                WriteLog("工位1触发视觉拍照", LogLevels.Info, "XrayCheckOnFQI");
                                Thread.Sleep(500);
                                step1 = 1;
                            }
                            break;

                        case 1:

                            if (vision.ReceiveData() == "OK")
                            {
                                Thread.Sleep(500);
                                vision.ReceiveData();
                                plc.WriteByVariableName("MDIComplete1", 1);
                                step1 = 2;
                            }
                            else
                            {
                                step1 = 1;
                                WriteLog("工位1视觉拍照超时", LogLevels.Warn, "XrayCheckOnFQI");
                            }
                            break;

                        case 2:

                            if (plc.ReadByVariableName("MDISignal1") == "0")
                            {
                                plc.WriteByVariableName("MDIComplete1", 0);
                                WriteLog("工位1视觉拍照完成", LogLevels.Info, "XrayCheckOnFQI");
                                step1 = 0;
                            }
                            break;
                    }

                    switch (step2)
                    {
                        case 0:

                            if (plc.ReadByVariableName("MDISignal2") == "1")
                            {
                                string cmd = "Run_ACQ2,abcdefg,2\r";
                                try
                                {
                                    vision.SendCommand(cmd);
                                }
                                catch (Exception ex)
                                {
                                    //CodeReaderIF.Init_ClientConnet(HardwareConfig.Instance.DimensionIPAdress, HardwareConfig.Instance.DimensionPort, 2);//尺寸测量连接
                                    vision.InitialShangLingVision(HardwareConfig.Instance.DimensionIPAdress, HardwareConfig.Instance.DimensionPort);
                                    Thread.Sleep(500);
                                    vision.SendCommand(cmd);
                                }
                                WriteLog("工位2触发视觉拍照", LogLevels.Info, "XrayCheckOnFQI");
                                Thread.Sleep(500);
                                step2 = 1;
                            }
                            break;

                        case 1:

                            if (vision.ReceiveData() == "OK")
                            {
                                Thread.Sleep(500);
                                vision.ReceiveData();
                                plc.WriteByVariableName("MDIComplete2", 1);
                                step2 = 2;
                            }
                            else
                            {
                                step2 = 1;
                                WriteLog("工位2视觉拍照超时", LogLevels.Warn, "XrayCheckOnFQI");
                            }
                            break;

                        case 2:

                            if (plc.ReadByVariableName("MDISignal2") == "0")
                            {
                                plc.WriteByVariableName("MDIComplete2", 0);
                                WriteLog("工位2视觉拍照完成", LogLevels.Info, "XrayCheckOnFQI");
                                step2 = 0;
                            }
                            break;
                    }

                    switch (step3)
                    {
                        case 0:

                            if (plc.ReadByVariableName("MDISignal3") == "1")
                            {
                                string cmd = "Run_ACQ2,abcdefg,3\r";
                                try
                                {
                                    vision.SendCommand(cmd);
                                }
                                catch (Exception ex)
                                {
                                    //CodeReaderIF.Init_ClientConnet(HardwareConfig.Instance.DimensionIPAdress, HardwareConfig.Instance.DimensionPort, 2);//尺寸测量连接
                                    vision.InitialShangLingVision(HardwareConfig.Instance.DimensionIPAdress, HardwareConfig.Instance.DimensionPort);
                                    Thread.Sleep(500);
                                    vision.SendCommand(cmd);
                                }
                                WriteLog("工位1触发视觉拍照", LogLevels.Info, "XrayCheckOnFQI");
                                Thread.Sleep(500);
                                step3 = 1;
                            }
                            break;

                        case 1:

                            if (vision.ReceiveData() == "OK")
                            {
                                Thread.Sleep(500);
                                vision.ReceiveData();
                                plc.WriteByVariableName("MDIComplete1", 1);
                                step3 = 2;
                            }
                            else
                            {
                                step3 = 1;
                                WriteLog("工位3视觉拍照超时", LogLevels.Warn, "XrayCheckOnFQI");
                            }
                            break;

                        case 2:

                            if (plc.ReadByVariableName("MDISignal3") == "0")
                            {
                                plc.WriteByVariableName("MDIComplete1", 0);
                                WriteLog("工位3视觉拍照完成", LogLevels.Info, "XrayCheckOnFQI");
                                step3 = 0;
                            }
                            break;
                    }

                    switch (step4)
                    {
                        case 0:

                            if (plc.ReadByVariableName("MDISignal4") == "1")
                            {
                                string cmd = "Run_ACQ2,abcdefg,4\r";
                                try
                                {
                                    vision.SendCommand(cmd);
                                }
                                catch (Exception ex)
                                {
                                    //CodeReaderIF.Init_ClientConnet(HardwareConfig.Instance.DimensionIPAdress, HardwareConfig.Instance.DimensionPort, 2);//尺寸测量连接
                                    vision.InitialShangLingVision(HardwareConfig.Instance.DimensionIPAdress, HardwareConfig.Instance.DimensionPort);
                                    Thread.Sleep(500);
                                    vision.SendCommand(cmd);
                                }
                                WriteLog("工位4触发视觉拍照", LogLevels.Info, "XrayCheckOnFQI");
                                Thread.Sleep(500);
                                step4 = 1;
                            }
                            break;

                        case 1:

                            if (vision.ReceiveData() == "OK")
                            {
                                Thread.Sleep(500);
                                vision.ReceiveData();
                                plc.WriteByVariableName("MDIComplete2", 1);
                                step4 = 2;
                            }
                            else
                            {
                                step4 = 1;
                                WriteLog("工位4视觉拍照超时", LogLevels.Warn, "XrayCheckOnFQI");
                            }
                            break;

                        case 2:

                            if (plc.ReadByVariableName("MDISignal4") == "0")
                            {
                                plc.WriteByVariableName("MDIComplete2", 0);
                                WriteLog("工位4视觉拍照完成", LogLevels.Info, "XrayCheckOnFQI");
                                step4 = 0;
                            }
                            break;


                    }
                }
            }
        }

        private int[] ivChannelNGCount = new int[17] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };//索引1到16代表IV表16个通道的测不到数据的次数

        /// <summary>
        /// 检测IV测试线路是否接通,连续三次数据都为0判线路问题
        /// </summary>
        /// <param name="strData"></param>
        /// <returns></returns>
        private bool CheckIVRoundIsClose(string strData, int channel)
        {
            if (strData.Contains(","))
            {
                bool isIVRoundIsClose = true;
                string[] arr = strData.Split(',');
                for (int i = 0; i < arr.Length; i++)
                {
                    //线路不通时数值全为0
                    if (Math.Abs(Math.Round(Convert.ToDouble(arr[i]), 2)) < 0.01)
                    {
                        isIVRoundIsClose = false;
                    }
                    else
                    {
                        isIVRoundIsClose = true;
                        break;
                    }
                }
                if (isIVRoundIsClose == false)
                {
                    ivChannelNGCount[channel]++;
                }
                else
                {
                    ivChannelNGCount[channel] = 0;
                }
            }
            if (ivChannelNGCount[channel] >= 3)
            {
                ivChannelNGCount[channel] = 0;
                return false;
            }
            return true;
        }

        /// <summary>
        /// 有触发信号直接回完成信号，全部料当NG排出，一般在通信异常时使用
        /// </summary>
        private void Task_NGUnload()
        {
            int stepScan1 = 0;
            int stepScan2 = 0;
            int stepBinding1 = 0;
            int stepBinding2 = 0;
            int stepIv = 0;
            int stepOcv1 = 0;
            int stepOcv2 = 0;
            int stepMdi1 = 0;
            int stepMdi2 = 0;
            int stepMdi3 = 0;
            int stepMdi4 = 0;
            int stepPpg1 = 0;
            int stepPpg2 = 0;
            int stepPpg3 = 0;
            int stepPpg4 = 0;
            int stepXray1 = 0;
            int stepXray2 = 0;
            int stepSorting = 0;
            BatterySeat bt = new BatterySeat();
            bt.Sn = "abc";
            bt.TempBarcode = 102;

            while (true)
            {
                Thread.Sleep(100);
                if (plc.ReadByVariableName("SoftwareReset") == "1")
                {
                    stepScan1 = 0;
                    stepScan2 = 0;
                    stepBinding1 = 0;
                    stepBinding2 = 0;
                    stepIv = 0;
                    stepOcv1 = 0;
                    stepOcv2 = 0;
                    stepMdi1 = 0;
                    stepMdi2 = 0;
                    stepMdi3 = 0;
                    stepMdi4 = 0;
                    stepPpg1 = 0;
                    stepPpg2 = 0;
                    stepPpg3 = 0;
                    stepPpg4 = 0;
                    stepXray1 = 0;
                    stepXray2 = 0;
                    stepSorting = 0;
                    bt.TempBarcode = 102;
                }
                if (plc.ReadByVariableName("TestMode") == "11")
                {
                    if (bt.TempBarcode == 150)
                    {
                        bt.TempBarcode = 102;
                    }
                    switch (stepScan1)
                    {
                        case 0:

                            if (plc.ReadByVariableName("ScanBarcodeSignal1") == "1")
                            {
                                plc.WriteByVariableName("BatteryScanning1OK", 1);
                                stepScan1 = 1;
                            }
                            break;

                        case 1:

                            if (plc.ReadByVariableName("GetTempBarodeSignal1") == "1")
                            {
                                plc.WriteByVariableName("GetTempBarcodeComplete1", 1);
                                stepScan1 = 2;
                            }
                            break;

                        case 2:

                            if (plc.ReadByVariableName("ScanBarcodeSignal1") == "0" && plc.ReadByVariableName("GetTempBarodeSignal1") == "0")
                            {
                                plc.WriteByVariableName("BatteryScanning1OK", 0);
                                plc.WriteByVariableName("GetTempBarcodeComplete1", 0);
                                stepScan1 = 0;
                            }
                            break;
                    }

                    switch (stepScan2)
                    {
                        case 0:

                            if (plc.ReadByVariableName("ScanBarcodeSignal2") == "1")
                            {
                                plc.WriteByVariableName("BatteryScanning2OK", 1);
                                stepScan2 = 1;
                            }
                            break;

                        case 1:

                            if (plc.ReadByVariableName("GetTempBarodeSignal2") == "1")
                            {
                                plc.WriteByVariableName("GetTempBarcodeComplete2", 1);
                                stepScan2 = 2;
                            }
                            break;

                        case 2:

                            if (plc.ReadByVariableName("ScanBarcodeSignal2") == "0" && plc.ReadByVariableName("GetTempBarodeSignal2") == "0")
                            {
                                plc.WriteByVariableName("BatteryScanning2OK", 0);
                                plc.WriteByVariableName("GetTempBarcodeComplete2", 0);
                                stepScan2 = 0;
                            }
                            break;
                    }

                    switch (stepBinding1)
                    {
                        case 0:

                            if (plc.ReadByVariableName("BarcodebindingSignal1") == "1")
                            {
                                plc.WriteByVariableName("BarcodeBindingComplete1", 1);
                                stepBinding1 = 1;
                            }
                            break;

                        case 1:

                            if (plc.ReadByVariableName("BarcodebindingSignal1") == "0")
                            {
                                plc.WriteByVariableName("BarcodeBindingComplete1", 0);
                                stepBinding1 = 0;
                            }
                            break;
                    }

                    switch (stepBinding2)
                    {
                        case 0:

                            if (plc.ReadByVariableName("BarcodebindingSignal2") == "1")
                            {
                                plc.WriteByVariableName("BarcodeBindingComplete2", 1);
                                stepBinding2 = 1;
                            }
                            break;

                        case 1:

                            if (plc.ReadByVariableName("BarcodebindingSignal2") == "0")
                            {
                                plc.WriteByVariableName("BarcodeBindingComplete2", 0);
                                stepBinding2 = 0;
                            }
                            break;
                    }

                    switch (stepIv)
                    {
                        case 0:

                            if (plc.ReadByVariableName("IVTestSignal1") == "1" || plc.ReadByVariableName("IVTestSignal2") == "1" || plc.ReadByVariableName("IVTestSignal3") == "1" || plc.ReadByVariableName("IVTestSignal4") == "1")
                            {
                                plc.WriteByVariableName("IVComplete", 1);
                                stepIv = 1;
                            }
                            break;
                        case 1:
                            if (plc.ReadByVariableName("IVTestSignal1") == "0" && plc.ReadByVariableName("IVTestSignal2") == "0" && plc.ReadByVariableName("IVTestSignal3") == "0" && plc.ReadByVariableName("IVTestSignal4") == "0")
                            {
                                plc.WriteByVariableName("IVComplete", 0);
                                stepIv = 2;
                            }
                            break;
                        case 2:
                            if (plc.ReadByVariableName("NeedleOKSignal1") == "1" ||
                                plc.ReadByVariableName("KnifeOKSignal1") == "1" ||
                                plc.ReadByVariableName("NeedleNGSignal1") == "1" ||
                                plc.ReadByVariableName("KnifeNGSignal1") == "1"
                                || plc.ReadByVariableName("NeedleOKSignal2") == "1" ||
                                plc.ReadByVariableName("KnifeOKSignal2") == "1" ||
                                plc.ReadByVariableName("NeedleNGSignal2") == "1" ||
                                plc.ReadByVariableName("KnifeNGSignal2") == "1"
                                || plc.ReadByVariableName("NeedleOKSignal3") == "1" ||
                                plc.ReadByVariableName("KnifeOKSignal3") == "1" ||
                                plc.ReadByVariableName("NeedleNGSignal3") == "1" ||
                                plc.ReadByVariableName("KnifeNGSignal3") == "1"
                                || plc.ReadByVariableName("NeedleOKSignal4") == "1" ||
                                plc.ReadByVariableName("KnifeOKSignal4") == "1" ||
                                plc.ReadByVariableName("NeedleNGSignal4") == "1" ||
                                plc.ReadByVariableName("KnifeNGSignal4") == "1")
                            {
                                plc.WriteByVariableName("IVConductionComplete", 1);
                                stepIv = 3;
                            }
                            break;

                        case 3:
                            if (plc.ReadByVariableName("NeedleOKSignal1") == "0" &&
                                plc.ReadByVariableName("KnifeOKSignal1") == "0" &&
                                plc.ReadByVariableName("NeedleNGSignal1") == "0" &&
                                plc.ReadByVariableName("KnifeNGSignal1") == "0"
                                && plc.ReadByVariableName("NeedleOKSignal2") == "0" &&
                                plc.ReadByVariableName("KnifeOKSignal2") == "0" &&
                                plc.ReadByVariableName("NeedleNGSignal2") == "0" &&
                                plc.ReadByVariableName("KnifeNGSignal2") == "0"
                                && plc.ReadByVariableName("NeedleOKSignal3") == "0" &&
                                plc.ReadByVariableName("KnifeOKSignal3") == "0" &&
                                plc.ReadByVariableName("NeedleNGSignal3") == "0" &&
                                plc.ReadByVariableName("KnifeNGSignal3") == "0"
                                && plc.ReadByVariableName("NeedleOKSignal4") == "0" &&
                                plc.ReadByVariableName("KnifeOKSignal4") == "0" &&
                                plc.ReadByVariableName("NeedleNGSignal4") == "0" &&
                                plc.ReadByVariableName("KnifeNGSignal4") == "0")
                            {
                                plc.WriteByVariableName("IVConductionComplete", 0);
                                stepIv = 0;
                            }
                            break;
                    }

                    switch (stepOcv1)
                    {
                        case 0:
                            if (plc.ReadByVariableName("ResistanceSignal1") == "1")
                            {
                                plc.WriteByVariableName("ResistanceComplete1", 1);
                                stepOcv1 = 1;
                            }
                            break;
                        case 1:
                            if (plc.ReadByVariableName("ResistanceSignal1") == "0")
                            {
                                plc.WriteByVariableName("ResistanceComplete1", 0);
                                stepOcv1 = 2;
                            }
                            break;
                        case 2:
                            if (plc.ReadByVariableName("VoltageSignal1") == "1")
                            {
                                plc.WriteByVariableName("VoltageComplete1", 1);
                                stepOcv1 = 3;
                            }
                            break;
                        case 3:
                            if (plc.ReadByVariableName("VoltageSignal1") == "0")
                            {
                                plc.WriteByVariableName("VoltageComplete1", 0);
                                stepOcv1 = 0;
                            }
                            break;
                    }

                    switch (stepOcv2)
                    {
                        case 0:
                            if (plc.ReadByVariableName("VoltageSignal2") == "1")
                            {
                                plc.WriteByVariableName("VoltageComplete2", 1);
                                stepOcv2 = 1;
                            }
                            break;
                        case 1:
                            if (plc.ReadByVariableName("VoltageSignal2") == "0")
                            {
                                plc.WriteByVariableName("VoltageComplete2", 0);
                                stepOcv2 = 2;
                            }
                            break;

                        case 2:
                            if (plc.ReadByVariableName("ResistanceSignal2") == "1")
                            {
                                plc.WriteByVariableName("ResistanceComplete2", 1);
                                stepOcv2 = 3;
                            }
                            break;
                        case 3:
                            if (plc.ReadByVariableName("ResistanceSignal2") == "0")
                            {
                                plc.WriteByVariableName("ResistanceComplete2", 0);
                                stepOcv2 = 0;
                            }
                            break;
                    }

                    switch (stepMdi1)
                    {
                        case 0:
                            if (plc.ReadByVariableName("MDISignal1") == "1")
                            {
                                if (!GetMdiData(ref bt, bt.TempBarcode, 1))
                                {
                                    plc.WriteByVariableName("MDIComplete1", 1);
                                }
                                bt.TempBarcode++;
                                stepMdi1 = 1;
                            }
                            break;
                        case 1:
                            if (plc.ReadByVariableName("MDISignal1") == "0")
                            {
                                plc.WriteByVariableName("MDIComplete1", 0);
                                stepMdi1 = 0;
                            }
                            break;
                    }

                    switch (stepMdi2)
                    {
                        case 0:
                            if (plc.ReadByVariableName("MDISignal2") == "1")
                            {
                                if (!GetMdiData(ref bt, bt.TempBarcode, 2))
                                {
                                    plc.WriteByVariableName("MDIComplete2", 1);
                                }
                                bt.TempBarcode++;
                                stepMdi2 = 1;
                            }
                            break;
                        case 1:
                            if (plc.ReadByVariableName("MDISignal2") == "0")
                            {
                                plc.WriteByVariableName("MDIComplete2", 0);
                                stepMdi2 = 0;
                            }
                            break;
                    }

                    switch (stepMdi3)
                    {
                        case 0:
                            if (plc.ReadByVariableName("MDISignal3") == "1")
                            {
                                if (!GetMdiData(ref bt, bt.TempBarcode, 3))
                                {
                                    plc.WriteByVariableName("MDIComplete1", 1);
                                }
                                bt.TempBarcode++;
                                stepMdi3 = 1;
                            }
                            break;
                        case 1:
                            if (plc.ReadByVariableName("MDISignal3") == "0")
                            {
                                plc.WriteByVariableName("MDIComplete1", 0);
                                stepMdi3 = 0;
                            }
                            break;
                    }

                    switch (stepMdi4)
                    {
                        case 0:
                            if (plc.ReadByVariableName("MDISignal4") == "1")
                            {
                                if (!GetMdiData(ref bt, bt.TempBarcode, 4))
                                {
                                    plc.WriteByVariableName("MDIComplete2", 1);
                                }
                                bt.TempBarcode++;
                                stepMdi4 = 1;
                            }
                            break;
                        case 1:
                            if (plc.ReadByVariableName("MDISignal4") == "0")
                            {
                                plc.WriteByVariableName("MDIComplete2", 0);
                                stepMdi4 = 0;
                            }
                            break;
                    }

                    switch (stepPpg1)
                    {
                        case 0:
                            if (plc.ReadByVariableName("ThicknessMeasureSignal1") == "1")
                            {
                                plc.WriteByVariableName("ThicknessComplete1", 1);
                                stepPpg1 = 1;
                            }
                            break;
                        case 1:
                            if (plc.ReadByVariableName("ThicknessMeasureSignal1") == "0")
                            {
                                plc.WriteByVariableName("ThicknessComplete1", 0);
                                stepPpg1 = 0;
                            }
                            break;
                    }

                    switch (stepPpg2)
                    {
                        case 0:
                            if (plc.ReadByVariableName("ThicknessMeasureSignal2") == "1")
                            {
                                plc.WriteByVariableName("ThicknessComplete2", 1);
                                stepPpg2 = 1;
                            }
                            break;
                        case 1:
                            if (plc.ReadByVariableName("ThicknessMeasureSignal2") == "0")
                            {
                                plc.WriteByVariableName("ThicknessComplete2", 0);
                                stepPpg2 = 0;
                            }
                            break;
                    }

                    switch (stepPpg3)
                    {
                        case 0:
                            if (plc.ReadByVariableName("ThicknessMeasureSignal3") == "1")
                            {
                                plc.WriteByVariableName("ThicknessComplete3", 1);
                                stepPpg3 = 1;
                            }
                            break;
                        case 1:
                            if (plc.ReadByVariableName("ThicknessMeasureSignal3") == "0")
                            {
                                plc.WriteByVariableName("ThicknessComplete3", 0);
                                stepPpg3 = 0;
                            }
                            break;
                    }

                    switch (stepPpg4)
                    {
                        case 0:
                            if (plc.ReadByVariableName("ThicknessMeasureSignal4") == "1")
                            {
                                plc.WriteByVariableName("ThicknessComplete4", 1);
                                stepPpg4 = 1;
                            }
                            break;
                        case 1:
                            if (plc.ReadByVariableName("ThicknessMeasureSignal4") == "0")
                            {
                                plc.WriteByVariableName("ThicknessComplete4", 0);
                                stepPpg4 = 0;
                            }
                            break;
                    }

                    switch (stepXray1)
                    {
                        case 0:
                            if (plc.ReadByVariableName("Battery1CornerPhotoSignal") == "1")
                            {
                                plc.WriteByVariableName("Battery1CornerPhotoComplete", 1);
                                stepXray1 = 1;
                            }
                            break;
                        case 1:
                            if (plc.ReadByVariableName("Battery1CornerPhotoSignal") == "0")
                            {
                                plc.WriteByVariableName("Battery1CornerPhotoComplete", 0);
                                stepXray1 = 2;
                            }
                            break;

                        case 2:
                            if (plc.ReadByVariableName("Battery2CornerPhotoSignal") == "1")
                            {
                                plc.WriteByVariableName("Battery2CornerPhotoComplete", 1);
                                stepXray1 = 3;
                            }
                            break;
                        case 3:
                            if (plc.ReadByVariableName("Battery2CornerPhotoSignal") == "0")
                            {
                                plc.WriteByVariableName("Battery2CornerPhotoComplete", 0);
                                stepXray1 = 0;
                            }
                            break;
                    }

                    switch (stepXray2)
                    {
                        case 0:
                            if (plc.ReadByVariableName("Battery3CornerPhotoSignal") == "1")
                            {
                                plc.WriteByVariableName("Battery3CornerPhotoComplete", 1);
                                stepXray2 = 1;
                            }
                            break;
                        case 1:
                            if (plc.ReadByVariableName("Battery3CornerPhotoSignal") == "0")
                            {
                                plc.WriteByVariableName("Battery3CornerPhotoComplete", 0);
                                stepXray2 = 2;
                            }
                            break;

                        case 2:
                            if (plc.ReadByVariableName("Battery4CornerPhotoSignal") == "1")
                            {
                                plc.WriteByVariableName("Battery4CornerPhotoComplete", 1);
                                stepXray2 = 3;
                            }
                            break;
                        case 3:
                            if (plc.ReadByVariableName("Battery4CornerPhotoSignal") == "0")
                            {
                                plc.WriteByVariableName("Battery4CornerPhotoComplete", 0);
                                stepXray2 = 0;
                            }
                            break;
                    }

                    switch (stepSorting)
                    {
                        case 0:
                            if (plc.ReadByVariableName("PhotoResultSignal") == "1")
                            {
                                plc.WriteByVariableName("OtherNG", 1);
                                stepSorting = 1;
                            }
                            break;
                        case 1:
                            if (plc.ReadByVariableName("PhotoResultSignal") == "0")
                            {
                                plc.WriteByVariableName("OtherNG", 0);
                                stepSorting = 0;
                            }
                            break;
                    }
                }
            }

        }

        private void UpLoadData(ref BatterySeat bt, string name, bool isRecheck = false)
        {
            #region A013接口
            DateTime startTime1 = DateTime.Now;
            bool headerResult = false;
            bool mesResult = false;
            List<string> outpuParamItems = new List<string>();
            if (CheckParamsConfig.Instance.IsCheckABCell == false)
            {
                outpuParamItems = ConertersUtils.BatterySeatToBatteryTestDataByName(bt, 2, CheckParamsConfig.Instance.TotalLayer, CheckParamsConfig.Instance.TotalLayersBD, name, isRecheck);
            }
            else
            {
                if (bt.CellType == "A")
                {
                    outpuParamItems = ConertersUtils.BatterySeatToBatteryTestDataByName(bt, 2, CheckParamsConfig.Instance.TotalLayer, CheckParamsConfig.Instance.TotalLayersBD, name, isRecheck);
                }
                else
                {
                    outpuParamItems = ConertersUtils.BatterySeatToBatteryTestDataByName(bt, 2, CheckParamsConfig.Instance.TotalLayersBD, CheckParamsConfig.Instance.TotalLayer, name, isRecheck);
                }
            }
            string Pass = "NG";
            string station = "1";
            if (name == "OCV")
            {
                station = bt.OCVChannel.ToString();
                if (bt.OCVResult == true)
                {
                    Pass = "OK";
                }
            }
            if (name == "FQI")
            {
                station = bt.MDIChannel.ToString();
                if (bt.DimensionResult == true)
                {
                    Pass = "OK";
                }
            }
            if (name == "XRAY")
            {
                if (bt.FinalResult == true)
                {
                    Pass = "OK";
                }
            }
            string json = string.Empty;
            ATL.MES.A014_minicell.Root root_14 = ATL.MES.InterfaceClient.Current.A013("Normal", bt.LengthBarcode, Pass, station, outpuParamItems, name, out json);
            if (root_14 == null)
            {
                WriteLog(string.Format("第{0}次 上传MES失败(A013接口)，离线缓存", bt.TempBarcode), LogLevels.Warn, "UpLoadData");
                ProductionDataXray productData = new ProductionDataXray();
                productData.ProductSN = bt.LengthBarcode;
                productData.JsonData = json;
                BatteryCheckIF.MyProductionDataOffline.AddProductionData(productData);
            }
            else
            {
                DateTime endTime1 = DateTime.Now;
                headerResult = root_14.Header.IsSuccess.ToUpper() == "TRUE";
                WriteLog(string.Format("第{0}次 上传 {1} MES成功(A013接口), 耗时 = {2} 毫秒,返回值：{3} ,errmsg= {4}", bt.TempBarcode, name, (endTime1 - startTime1).TotalMilliseconds, headerResult, root_14.Header.ErrorMsg), LogLevels.Info, "UpLoadData");
                mesResult = headerResult && root_14.ResponseInfo.Products[0].Pass == "OK" ? true : false;

                if (name == "OCV")
                {
                    bt.MesOCVResult = mesResult;
                    bt.OCVResult = mesResult;
                    if (mesResult == false)
                    {
                        if (headerResult)
                        {
                            bt.VoltageResult = true;
                            bt.ResistanceResult = true;
                            if (root_14.ResponseInfo.Products[0].Param.Count > 3)
                            {
                                foreach (var param in root_14.ResponseInfo.Products[0].Param)
                                {
                                    if (param.ParamID == "241")
                                    {
                                        bt.VoltageResult = param.Result == "OK";
                                    }
                                    else if (param.ParamID == "323")
                                    {
                                        bt.ResistanceResult = param.Result == "OK";
                                    }
                                    else if (param.ParamID == "248")
                                    {
                                        bt.KValueResult = param.Result == "OK";
                                    }
                                }
                            }
                            //bt.VoltageResult = root_14.ResponseInfo.Products[0].Param[1].ParamID == "241" && root_14.ResponseInfo.Products[0].Param[1].Result == "OK";
                            //bt.ResistanceResult = root_14.ResponseInfo.Products[0].Param[2].ParamID == "323" && root_14.ResponseInfo.Products[0].Param[2].Result == "OK";
                            //温度报警暂时不管
                        }

                        if (root_14.Header.ErrorMsg == "ReworkSkipStation" || root_14.Header.ErrorMsg == "NotBackToSpecifiedOperation")
                        {
                            WriteLog(string.Format("第{0}次 OCV上传MES NG 但errmsg=ReworkSkipStation或NotBackToSpecifiedOperation，改判OK", bt.TempBarcode), LogLevels.Info, "UpLoadData");
                            bt.MesOCVResult = true;
                        }
                    }
                    //else
                    //{
                    //    bt.MesOCVResult = true;
                    //}
                }
                else if (name == "FQI")
                {
                    bt.MesMDIResult = mesResult;
                    bt.DimensionResult = mesResult;
                    if (headerResult)
                    {
                        bt.ThicknessResult = true;
                        bt.DimensionResult = true;
                        if (root_14.ResponseInfo.Products[0].Param.Count > 9)
                        {
                            foreach (var param in root_14.ResponseInfo.Products[0].Param)
                            {
                                if (param.ParamID == "3301")
                                {
                                    bt.ThicknessResult = param.Result == "OK";
                                }
                                else if (param.Result == "NG")
                                {
                                    bt.DimensionResult = false;
                                    break;
                                }
                            }
                        }
                    }
                }
                else if (name == "XRAY")
                {
                    bt.MesXRAYResult = mesResult;
                    bt.FinalResult = mesResult;
                    if (mesResult == false)
                    {
                        if (root_14.Header.ErrorMsg == "ReworkSkipStation")
                        {
                            WriteLog(string.Format("第{0}次 XRAY上传MES NG 但errmsg=ReworkSkipStation，改判OK", bt.TempBarcode), LogLevels.Info, "UpLoadData");
                            bt.MesXRAYResult = true;
                        }
                    }
                    //else
                    //{
                    //    bt.MesXRAYResult = true;
                    //}
                }
            }
            if (name == "XRAY")
            {
                bt.StfResult = true; //为true才会上传图片到MES
            }
            #endregion

        }
    }
}

