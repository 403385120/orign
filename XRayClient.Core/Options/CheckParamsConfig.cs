using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XRayClient.VisionSysWrapper;
using Shuyz.Framework.Mvvm;
using System.Runtime.InteropServices;
using System.IO;

namespace XRayClient.Core
{
    /// <summary>
    /// 检测参数定义
    /// </summary>
    public class CheckParamsConfig : ObservableObject
    {
        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key,
                                                   string val, string filePath);
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def,
                                                          StringBuilder retVal, int size, string filePath);
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def,
                                                          byte[] retVal, int size, string filePath);

        private readonly string _configFile = Path.Combine(Environment.CurrentDirectory, "CheckParamsConfig.ini");

        private static CheckParamsConfig _instance = new CheckParamsConfig();
        public static CheckParamsConfig Instance
        {
            get { return _instance; }
        }

        private bool _isEnabled = false;
        private string _oCVMode = "O1";//OCV类型
        private string _factoryCode = "";//工厂代码
        private int _ocvWarmingCounts = 3;
        private int _mdiWarmingCounts = 3;
        private int _ppgWarmingCounts = 3;
        private int _warmWaitTime = 60;
        private int _stationRangeOutTimes = 5;
        private float _rangeOfTemperatrue = 5;//环境温度与电池表面温度差
        private string _markingBase = "";//Marking库
        private string _markingCurrent = "";//当前使用的Marking
        private string _ivCheckTime = "";//IV点检完成时间
        private string _ocvCheckTime = "";//OCV点检完成时间
        private string _mdiCheckTime = "";//MDI点检完成时间
        private string _ppgCheckTime = "";//PPG点检完成时间
        private string _xrayCheckTime = "";//XRAY点检完成时间
        private string _needleCheckTime = "";//针规点检完成时间
        private string _linearCheckTime = "";//电芯线性点检完成时间

        private string _productMode = "";
        private string _pcName = "";
        private string _cellType = "";
        private bool _isTiffMode = true;//是否TIFF图模式

        private ECheckModes _checkMode = ECheckModes.Diagonal_1_2;             // 检测模式

        private InspectParams _inspectParams = new InspectParams(1);        // 检测算法参数
        private ImageSaveConfig _imageSaveConfig = new ImageSaveConfig();   // 图片保存参数
        private StartupTestConfig _startTestConfig = new StartupTestConfig();// 点检参数
        private string _miDataDir = @"\\nd-app01\\XRaySpec\\MISpec.csv";
        private string _reModelDir = @"\\atlbattery.com\nd-fqi\MISpec-FQI正常规格";
        private string _productNO = "T96";
        private bool isGrrMode = false;
        private bool isSamplingMode = false;
        private bool _isCheckABCell = false;
        private string _markingOfACell = "";
        private bool isNoUploadMES = false;
        private float max_thickness = 0;
        private float min_thickness = 0;
        private float calival_thickness = 0;
        private float thicknessKValue = 1;
        private float thicknessBValue = 0;
        private float _cellKValue = 0;//厚度相关补偿值
        private float _cellBValue = 0;//厚度相关补偿值

        private float _stationRange = 0;//工位的数据极差
        private int _stationRangeNum = 90;//工位的极差检测个数
        private float _stationWarmingAverage = 0;//工位数据均值报警值
        private float _stationWarmingTolerance = 0;//工位的数据均值上限

        //厚度点检
        private int thicknessCalibrationModeA = 0;
        private int thicknessCalibrationModeB = 0;
        private int thicknessCalibrationModeC = 0;
        private int thicknessCalibrationModeD = 0;
        private float max_thickness_s = 0;
        private float min_thickness_s = 0;
        private float max_thickness_m = 0;
        private float min_thickness_m = 0;
        private float max_thickness_b = 0;
        private float min_thickness_b = 0;
        private string currentModel = string.Empty;
        private float linearOKNum = 0;
        private float linearNGNum = 0;
        private float linearRange = 0.015f;
        private float linearNeedOK = 3;

        //尺寸点检
        private int dimensionCalibrationMode = 0;
        private string bat_length_s = string.Empty;
        private string bat_width_s = string.Empty;
        private string left_lug_s = string.Empty;
        private string right_lug_s = string.Empty;
        private string all_length_s = string.Empty;
        private string left_1_glue_s = string.Empty;
        private string left_2_glue_s = string.Empty;
        private string right_1_glue_s = string.Empty;
        private string right_2_glue_s = string.Empty;
        private string left_lug_len_s = string.Empty;
        private string right_lug_len_s = string.Empty;
        private string bat_length_m = string.Empty;
        private string bat_width_m = string.Empty;
        private string left_lug_m = string.Empty;
        private string right_lug_m = string.Empty;
        private string all_length_m = string.Empty;
        private string left_1_glue_m = string.Empty;
        private string left_2_glue_m = string.Empty;
        private string right_1_glue_m = string.Empty;
        private string right_2_glue_m = string.Empty;
        private string left_lug_len_m = string.Empty;
        private string right_lug_len_m = string.Empty;
        private string bat_length_b = string.Empty;
        private string bat_width_b = string.Empty;
        private string left_lug_b = string.Empty;
        private string right_lug_b = string.Empty;
        private string all_length_b = string.Empty;
        private string left_1_glue_b = string.Empty;
        private string left_2_glue_b = string.Empty;
        private string right_1_glue_b = string.Empty;
        private string right_2_glue_b = string.Empty;
        private string left_lug_len_b = string.Empty;
        private string right_lug_len_b = string.Empty;

        public float MinMidLugMargin { get; set; }//中间极耳边距最小值
        public float MaxMidLugMargin { get; set; }//中间极耳边距最大值
        public float MinMid1WhiteGlue { get; set; }//中间1小白胶最小值
        public float MaxMid1WhiteGlue { get; set; }//中间1小白胶最大值
        public float MinMid2WhiteGlue { get; set; }//中间2小白胶最小值
        public float MaxMid2WhiteGlue { get; set; }//中间2小白胶最大值
        public float MinMidLugLength { get; set; }//中间极耳长度最小值
        public float MaxMidLugLength { get; set; }//中间极耳长度最大值
        public float CellKValue2 { get; set; }//相关性K值2
        public float CellKValue3 { get; set; }//相关性K值3
        public float CellKValue4 { get; set; }//相关性K值4
        public float CellBValue2 { get; set; }//相关性B值2
        public float CellBValue3 { get; set; }//相关性B值3
        public float CellBValue4 { get; set; }//相关性B值4

        private float minBatLength = 0;
        private float maxBatLength = 0;
        private float minBatWidth = 0;
        private float maxBatWidth = 0;
        private float minLeftLugMargin = 0;
        private float maxLeftLugMargin = 0;
        private float minRightLugMargin = 0;
        private float maxRightLugMargin = 0;
        private float minLeftLugLength = 0;
        private float maxLeftLugLength = 0;
        private float minRightLugLength = 0;
        private float maxRightLugLength = 0;
        private float minAllBatLength = 0;
        private float maxAllBatLength = 0;
        private float minLeft1WhiteGlue = 0;
        private float maxLeft1WhiteGlue = 0;
        private float minLeft2WhiteGlue = 0;
        private float maxLeft2WhiteGlue = 0;
        private float minRight1WhiteGlue = 0;
        private float maxRight1WhiteGlue = 0;
        private float minRight2WhiteGlue = 0;
        private float maxRight2WhiteGlue = 0;
        private float minLeft1WhiteGlueMin = 0;
        private float maxLeft1WhiteGlueMin = 0;
        private float minLeft2WhiteGlueMin = 0;
        private float maxLeft2WhiteGlueMin = 0;
        private float minRight1WhiteGlueMin = 0;
        private float maxRight1WhiteGlueMin = 0;
        private float minRight2WhiteGlueMin = 0;
        private float maxRight2WhiteGlueMin = 0;
        private bool _isNoUpLoadMdiAndPPGData = false;//是否不上传测厚和尺寸数据
        private bool _isAlOnLeft = false;//是否铝极耳在左边

        //OCV
        private float minResistance = 0;//内阻
        private float maxResistance = 0;
        private float minVoltage = 0;//电压
        private float maxVoltage = 0;
        private float minOCV_k = 0;//K值
        private float maxOCV_k = 0;
        private float minTemperature = 0;//温度
        private float maxTemperature = 0;
        private float thicknessAvgDiff = 0;//A、B测厚模块的均值差上限
        private float temperatureCoefficient = 0;//工位1温度补偿系数
        private float temperatureCoefficient2 = 0;//工位2温度补偿系数
        private float _voltageCoefficient = 0;//电压补偿值
        private float _resistanceCoefficient = 0;//内阻补偿值
        private float _resistanceFixedValue = 0;//内阻固定值
        private float _temperatureFixedValue = 0;//温度固定值
        private bool _isNoResistance = false;//是否不测内阻
        private bool _isNoTemperature = false;//是否不测温度
        private bool _isCheckMarking = false;//是否Marking拦截
        private bool _isCheckWeekCount = false;//是否周次拦截
        private bool _isSetMarking = false;//是否打Marking
        private bool _isCheckMI = false;//是否校验MI号


        private string _mi = "";
        private int _weekCounts = 1;
        private string _marking = "";
        private string _setMarking = "";

        //IV
        private float minIV = 0;//IV最大值
        private float maxIV = 0;//IV最小值
        private float source = 0;//初始值
        private float range = 0;//跳变值
        private float exceptionData1 = 0;//异常值1
        private float exceptionData2 = 0;//异常值2
        private int _ivTestTime = 1200;//IV测试时间，一般工艺要求写死，不能改
        private int _ivStation1Channel = 1;//IV工位1对应仪器通道号
        private int _ivStation2Channel = 3;//IV工位1对应仪器通道号
        private int _ivStation3Channel = 5;//IV工位1对应仪器通道号
        private int _ivStation4Channel = 7;//IV工位1对应仪器通道号

        public bool IsEnabled
        {
            get { return _isEnabled; }
            set
            {
                _isEnabled = value;
                RaisePropertyChanged("IsEnabled");
            }
        }

        public string IvCheckTime
        {
            get { return this._ivCheckTime; }
            set
            {
                this._ivCheckTime = value;
                RaisePropertyChanged("IvCheckTime");
            }
        }

        public string OcvCheckTime
        {
            get { return this._ocvCheckTime; }
            set
            {
                this._ocvCheckTime = value;
                RaisePropertyChanged("OcvCheckTime");
            }
        }

        public string MdiCheckTime
        {
            get { return this._mdiCheckTime; }
            set
            {
                this._mdiCheckTime = value;
                RaisePropertyChanged("MdiCheckTime");
            }
        }

        public string PpgCheckTime
        {
            get { return this._ppgCheckTime; }
            set
            {
                this._ppgCheckTime = value;
                RaisePropertyChanged("PpgCheckTime");
            }
        }

        public string XrayCheckTime
        {
            get { return this._xrayCheckTime; }
            set
            {
                this._xrayCheckTime = value;
                RaisePropertyChanged("XrayCheckTime");
            }
        }

        public string NeedleCheckTime
        {
            get { return this._needleCheckTime; }
            set
            {
                this._needleCheckTime = value;
                RaisePropertyChanged("NeedleCheckTime");
            }
        }



        public string OCVMode
        {
            get { return this._oCVMode; }
            set
            {
                this._oCVMode = value;
                RaisePropertyChanged("OCVMode");
            }
        }

        public string MarkingCurrent
        {
            get { return this._markingCurrent; }
            set
            {
                this._markingCurrent = value;
                RaisePropertyChanged("MarkingCurrent");
            }
        }

        public string MarkingBase
        {
            get { return this._markingBase; }
            set
            {
                this._markingBase = value;
                RaisePropertyChanged("MarkingBase");
            }
        }

        public string FactoryCode
        {
            get { return this._factoryCode; }
            set
            {
                this._factoryCode = value;
                RaisePropertyChanged("FactoryCode");
            }
        }

        public string PcName
        {
            get { return this._pcName; }
            set
            {
                this._pcName = value;
                RaisePropertyChanged("PcName");
            }
        }

        public string CellType
        {
            get { return this._cellType; }
            set
            {
                this._cellType = value;
                RaisePropertyChanged("CellType");
            }
        }

        public bool IsTiffMode
        {
            get { return this._isTiffMode; }
            set
            {
                this._isTiffMode = value;
                RaisePropertyChanged("IsTiffMode");
            }
        }

        public int OCVWarmingCounts
        {
            get { return this._ocvWarmingCounts; }
            set
            {
                this._ocvWarmingCounts = value;
                RaisePropertyChanged("OCVWarmingCounts");
            }
        }

        public float RangeOfTemperatrue
        {
            get { return this._rangeOfTemperatrue; }
            set
            {
                this._rangeOfTemperatrue = value;
                RaisePropertyChanged("RangeOfTemperatrue");
            }
        }

        public int MDIWarmingCounts
        {
            get { return this._mdiWarmingCounts; }
            set
            {
                this._mdiWarmingCounts = value;
                RaisePropertyChanged("MDIWarmingCounts");
            }
        }

        public int PPGWarmingCounts
        {
            get { return this._ppgWarmingCounts; }
            set
            {
                this._ppgWarmingCounts = value;
                RaisePropertyChanged("PPGWarmingCounts");
            }
        }

        public int WarmWaitTime
        {
            get { return this._warmWaitTime; }

            set
            {
                this._warmWaitTime = value;
                RaisePropertyChanged("WarmWaitTime");
            }
        }

        public int StationRangeOutTimes
        {
            get { return this._stationRangeOutTimes; }

            set
            {
                this._stationRangeOutTimes = value;
                RaisePropertyChanged("StationRangeOutTimes");
            }
        }

        public string ProductMode
        {
            get { return this._productMode; }
            set
            {
                this._productMode = value;
                RaisePropertyChanged("ProductMode");
            }
        }

        public string ProductNO
        {
            get { return this._productNO; }
            set
            {
                this._productNO = value;
                RaisePropertyChanged("ProductNO");
            }
        }

        public string MIDataDir
        {
            get { return this._miDataDir; }
            set
            {
                this._miDataDir = value;
                RaisePropertyChanged("MiDataDir");
            }
        }

        public string ReModelDir
        {
            get { return this._reModelDir; }

            set
            {
                this._reModelDir = value;
                RaisePropertyChanged("ReModelDir");
            }
        }

        public ECheckModes CheckMode
        {
            get { return this._checkMode; }
            set
            {
                this._checkMode = value;
                RaisePropertyChanged("CheckMode");
            }
        }

        public IEnumerable<ECheckModes> BindableCheckModes
        {
            get
            {
                return Enum.GetValues(typeof(ECheckModes))
                    .Cast<ECheckModes>();
            }
        }

        public ImageSaveConfig MyImageSaveConfig
        {
            get { return this._imageSaveConfig; }
            set
            {
                this._imageSaveConfig = value;
                RaisePropertyChanged("MyImageSaveConfig");
            }
        }

        public StartupTestConfig MyStartupTestConfig
        {
            get { return this._startTestConfig; }
            set
            {
                this._startTestConfig = value;
                RaisePropertyChanged("MyStartupTestConfig");
            }
        }

        public InspectParams MyInspectParams
        {
            get { return this._inspectParams; }
            set
            {
                this._inspectParams = value;
                RaisePropertyChanged("MyInspectParams");
            }
        }

        #region - InspectParams -

        // 检测算法结构体无法直接绑定，下面是Wrapper
        public int iLine
        {
            get { return this._inspectParams.iLine; }
            set
            {
                this._inspectParams.iLine = value;
                RaisePropertyChanged("iLine");
            }
        }

        // iCorner

        public int TotalLayer
        {
            get { return this._inspectParams.total_layer; }
            set
            {
                this._inspectParams.total_layer = value;
                RaisePropertyChanged("TotalLayer");
            }
        }

        public float PixelToMM
        {
            get { return this._inspectParams.pixel_to_mm; }
            set
            {
                this._inspectParams.pixel_to_mm = value;
                RaisePropertyChanged("PixelToMM");
            }
        }

        public float MinLengthHead
        {
            get { return this.min_length_head; }
            set
            {
                this.min_length_head = value;
                RaisePropertyChanged("MinLengthHead");
            }
        }

        public float MaxLengthHead
        {
            get { return this.max_length_head; }
            set
            {
                this.max_length_head = value;
                RaisePropertyChanged("MaxLengthHead");
            }
        }

        public float MinLengthTail
        {
            get { return this.min_length_tail; }
            set
            {
                this.min_length_tail = value;
                RaisePropertyChanged("MinLengthTail");
            }
        }

        public float MaxLengthTail
        {
            get { return this.max_length_tail; }
            set
            {
                this.max_length_tail = value;
                RaisePropertyChanged("MaxLengthTail");
            }
        }
        public float MaxAngleThresh
        {
            get { return this._inspectParams.max_angle_thresh; }
            set
            {
                this._inspectParams.max_angle_thresh = value;
                RaisePropertyChanged("MaxAngleThresh");
            }
        }

        public bool IsDrawLine
        {
            get { return this._inspectParams.isDrawLine; }
            set
            {
                this._inspectParams.isDrawLine = value;
                RaisePropertyChanged("IsDrawLine");
            }
        }

        public bool IsShowData
        {
            get { return this._inspectParams.isShowData; }
            set
            {
                this._inspectParams.isShowData = value;
                RaisePropertyChanged("IsShowData");
            }
        }

        public bool IsShowAngle
        {
            get { return this._inspectParams.isShowAngle; }
            set
            {
                this._inspectParams.isShowAngle = value;
                RaisePropertyChanged("IsShowAngle");
            }
        }

        public bool IsDetectAngle
        {
            get { return this._inspectParams.isDetectAngle; }
            set
            {
                this._inspectParams.isDetectAngle = value;
                RaisePropertyChanged("IsDetectAngle");
            }
        }

        public float RectX
        {
            get { return this._inspectParams.detected_rect.x; }
            set
            {
                this._inspectParams.detected_rect.x = value;
                RaisePropertyChanged("RectX");
            }
        }

        public float RectY
        {
            get { return this._inspectParams.detected_rect.y; }
            set
            {
                this._inspectParams.detected_rect.y = value;
                RaisePropertyChanged("RectY");
            }
        }

        public float RectWidth
        {
            get { return this._inspectParams.detected_rect.width; }
            set
            {
                this._inspectParams.detected_rect.width = value;
                RaisePropertyChanged("RectWidth");
            }
        }

        public float RectHeight
        {
            get { return this._inspectParams.detected_rect.height; }
            set
            {
                this._inspectParams.detected_rect.height = value;
                RaisePropertyChanged("RectHeight");
            }
        }

        // 电池的两边的下面几个参数是不同的，需要两组参数
        private int _totalLayersBD = 5;
        private float _rectWidthBD = 300;
        private float _rectHeightBD = 200;
        public float max_length_head;
        public float min_length_head;
        public float max_length_tail;
        public float min_length_tail;
        public bool IsGRRMode
        {
            get { return isGrrMode; }
            set
            {
                isGrrMode = value;
            }
        }
        public bool IsSamplingMode
        {
            get { return isSamplingMode; }
            set
            {
                isSamplingMode = value;
                RaisePropertyChanged("IsSamplingMode");
            }
        }

        public bool IsCheckABCell
        {
            get { return _isCheckABCell; }
            set
            {
                _isCheckABCell = value;
                RaisePropertyChanged("IsCheckABCell");
            }
        }

        public string MarkingOfACell
        {
            get { return _markingOfACell; }
            set
            {
                _markingOfACell = value;
                RaisePropertyChanged("MarkingOfACell");
            }
        }

        public bool IsNoUploadMES
        {
            get { return isNoUploadMES; }
            set
            {
                isNoUploadMES = value;
                RaisePropertyChanged("IsNoUploadMES");
            }
        }

        public int TotalLayersBD
        {
            get { return this._totalLayersBD; }
            set
            {
                this._totalLayersBD = value;
                RaisePropertyChanged("TotalLayersBD");
            }
        }

        public float RectWidthBD
        {
            get { return this._rectWidthBD; }
            set
            {
                this._rectWidthBD = value;
                RaisePropertyChanged("RectWidthBD");
            }
        }

        public float RectHeightBD
        {
            get { return this._rectHeightBD; }
            set
            {
                this._rectHeightBD = value;
                RaisePropertyChanged("RectHeightBD");
            }
        }

        public float MaxThickness
        {
            get { return this.max_thickness; }
            set
            {
                this.max_thickness = value;
                RaisePropertyChanged("MaxThickness");
            }
        }

        public float MinThickness
        {
            get { return this.min_thickness; }
            set
            {
                this.min_thickness = value;
                RaisePropertyChanged("MinThickness");
            }
        }

        public float CaliValThickness
        {
            get { return this.calival_thickness; }
            set
            {
                this.calival_thickness = value;
                RaisePropertyChanged("CaliValThickness");
            }
        }
        public float ThicknessKValue
        {
            get { return this.thicknessKValue; }
            set
            {
                this.thicknessKValue = value;
                RaisePropertyChanged("ThicknessKValue");
            }
        }
        public float ThicknessBValue
        {
            get { return this.thicknessBValue; }
            set
            {
                this.thicknessBValue = value;
                RaisePropertyChanged("ThicknessBValue");
            }
        }

        public float CellKValue
        {
            get { return this._cellKValue; }
            set
            {
                this._cellKValue = value;
                RaisePropertyChanged("CellKValue");
            }
        }

        public float CellBValue
        {
            get { return this._cellBValue; }
            set
            {
                this._cellBValue = value;
                RaisePropertyChanged("CellBValue");
            }
        }

        public float StationRange
        {
            get { return this._stationRange; }
            set
            {
                this._stationRange = value;
                RaisePropertyChanged("StationRange");
            }
        }

        public int StationRangeNum
        {
            get { return _stationRangeNum; }

            set
            {
                this._stationRangeNum = value;
                RaisePropertyChanged("StationRangeNum");
            }
        }

        public float StationWarmingAverage
        {
            get { return this._stationWarmingAverage; }
            set
            {
                this._stationWarmingAverage = value;
                RaisePropertyChanged("StationWarmingAverage");
            }
        }

        public float StationWarmingTolerance
        {
            get { return this._stationWarmingTolerance; }
            set
            {
                this._stationWarmingTolerance = value;
                RaisePropertyChanged("StationWarmingTolerance");
            }
        }

        public int ThicknessCalibrationModeA
        {
            get { return this.thicknessCalibrationModeA; }
            set
            {
                this.thicknessCalibrationModeA = value;
                RaisePropertyChanged("ThicknessCalibrationModeA");
            }
        }

        public int ThicknessCalibrationModeB
        {
            get { return this.thicknessCalibrationModeB; }
            set
            {
                this.thicknessCalibrationModeB = value;
                RaisePropertyChanged("ThicknessCalibrationModeB");
            }
        }

        public int ThicknessCalibrationModeC
        {
            get { return this.thicknessCalibrationModeC; }
            set
            {
                this.thicknessCalibrationModeC = value;
                RaisePropertyChanged("ThicknessCalibrationModeC");
            }
        }

        public int ThicknessCalibrationModeD
        {
            get { return this.thicknessCalibrationModeD; }
            set
            {
                this.thicknessCalibrationModeD = value;
                RaisePropertyChanged("ThicknessCalibrationModeD");
            }
        }

        public string CurrentModel
        {
            get { return this.currentModel; }
            set
            {
                this.currentModel = value;
                RaisePropertyChanged("CurrentModel");
            }
        }

        //TODO: 厚度线性点检属性 2021-4-5
        #region 线性点检属性
        ///<summary>点检通过的次数</summary>
        public float LinearOKNum
        {
            get { return this.linearOKNum; }

            set
            {
                this.linearOKNum = value;
                RaisePropertyChanged("LinearOKNum");
            }
        }

        ///<summary>点检不通过的次数</summary>
        public float LinearNGNum
        {
            get { return this.linearNGNum; }

            set
            {
                this.linearNGNum = value;
                RaisePropertyChanged("LinearNGNum");
            }
        }

        ///<summary>极差允许范围</summary>
        public float LinearRange
        {
            get { return this.linearRange; }

            set
            {
                this.linearRange = value;
                RaisePropertyChanged("LinearRange");
            }
        }

        ///<summary>强制要求点检通过的次数</summary>
        public float LinearNeedOK
        {
            get { return this.linearNeedOK; }

            set
            {
                this.linearNeedOK = value;
                RaisePropertyChanged("LinearNeedOK");
            }
        }

        /// <summary>电芯线性点检完成时间</summary>
        public string LinearCheckTime
        {
            get { return this._linearCheckTime; }

            set
            {
                this._linearCheckTime = value;
                RaisePropertyChanged("LinearCheckTime");
            }
        }
        #endregion

        public float MaxThicknessS
        {
            get { return this.max_thickness_s; }
            set
            {
                this.max_thickness_s = value;
                RaisePropertyChanged("MaxThicknessS");
            }
        }

        public float MinThicknessS
        {
            get { return this.min_thickness_s; }
            set
            {
                this.min_thickness_s = value;
                RaisePropertyChanged("MinThicknessS");
            }
        }

        public float MaxThicknessM
        {
            get { return this.max_thickness_m; }
            set
            {
                this.max_thickness_m = value;
                RaisePropertyChanged("MaxThicknessM");
            }
        }

        public float MinThicknessM
        {
            get { return this.min_thickness_m; }
            set
            {
                this.min_thickness_m = value;
                RaisePropertyChanged("MinThicknessM");
            }
        }

        public float MaxThicknessB
        {
            get { return this.max_thickness_b; }
            set
            {
                this.max_thickness_b = value;
                RaisePropertyChanged("MaxThicknessB");
            }
        }

        public float MinThicknessB
        {
            get { return this.min_thickness_b; }
            set
            {
                this.min_thickness_b = value;
                RaisePropertyChanged("MinThicknessB");
            }
        }

        #region 尺寸点检
        public int DimensionCalibrationMode
        {
            get { return this.dimensionCalibrationMode; }
            set
            {
                this.dimensionCalibrationMode = value;
                RaisePropertyChanged("DimensionCalibrationMode");
            }
        }
        public string BatLengthS
        {
            get { return this.bat_length_s; }
            set
            {
                this.bat_length_s = value;
                RaisePropertyChanged("BatLengthS");
            }
        }
        public string BatWidthS
        {
            get { return this.bat_width_s; }
            set
            {
                this.bat_width_s = value;
                RaisePropertyChanged("BatWidthS");
            }
        }
        public string LeftLugS
        {
            get { return this.left_lug_s; }
            set
            {
                this.left_lug_s = value;
                RaisePropertyChanged("LeftLugS");
            }
        }
        public string RightLugS
        {
            get { return this.right_lug_s; }
            set
            {
                this.right_lug_s = value;
                RaisePropertyChanged("RightLugS");
            }
        }
        public string AllLengthS
        {
            get { return this.all_length_s; }
            set
            {
                this.all_length_s = value;
                RaisePropertyChanged("AllLengthS");
            }
        }
        public string Left1GlueS
        {
            get { return this.left_1_glue_s; }
            set
            {
                this.left_1_glue_s = value;
                RaisePropertyChanged("Left1GlueS");
            }
        }
        public string Left2GlueS
        {
            get { return this.left_2_glue_s; }
            set
            {
                this.left_2_glue_s = value;
                RaisePropertyChanged("Left2GlueS");
            }
        }
        public string Right1GlueS
        {
            get { return this.right_1_glue_s; }
            set
            {
                this.right_1_glue_s = value;
                RaisePropertyChanged("Right1GlueS");
            }
        }
        public string Right2GlueS
        {
            get { return this.right_2_glue_s; }
            set
            {
                this.right_2_glue_s = value;
                RaisePropertyChanged("Right2GlueS");
            }
        }
        public string LeftLugLenS
        {
            get { return this.left_lug_len_s; }
            set
            {
                this.left_lug_len_s = value;
                RaisePropertyChanged("LeftLugLenS");
            }
        }
        public string RightLugLenS
        {
            get { return this.right_lug_len_s; }
            set
            {
                this.right_lug_len_s = value;
                RaisePropertyChanged("RightLugLenS");
            }
        }

        public string BatLengthM
        {
            get { return this.bat_length_m; }
            set
            {
                this.bat_length_m = value;
                RaisePropertyChanged("BatLengthM");
            }
        }
        public string BatWidthM
        {
            get { return this.bat_width_m; }
            set
            {
                this.bat_width_m = value;
                RaisePropertyChanged("BatWidthM");
            }
        }
        public string LeftLugM
        {
            get { return this.left_lug_m; }
            set
            {
                this.left_lug_m = value;
                RaisePropertyChanged("LeftLugM");
            }
        }
        public string RightLugM
        {
            get { return this.right_lug_m; }
            set
            {
                this.right_lug_m = value;
                RaisePropertyChanged("RightLugM");
            }
        }
        public string AllLengthM
        {
            get { return this.all_length_m; }
            set
            {
                this.all_length_m = value;
                RaisePropertyChanged("AllLengthM");
            }
        }
        public string Left1GlueM
        {
            get { return this.left_1_glue_m; }
            set
            {
                this.left_1_glue_m = value;
                RaisePropertyChanged("Left1GlueM");
            }
        }
        public string Left2GlueM
        {
            get { return this.left_2_glue_m; }
            set
            {
                this.left_2_glue_m = value;
                RaisePropertyChanged("Left2GlueM");
            }
        }
        public string Right1GlueM
        {
            get { return this.right_1_glue_m; }
            set
            {
                this.right_1_glue_m = value;
                RaisePropertyChanged("Right1GlueM");
            }
        }
        public string Right2GlueM
        {
            get { return this.right_2_glue_m; }
            set
            {
                this.right_2_glue_m = value;
                RaisePropertyChanged("Right2GlueM");
            }
        }
        public string LeftLugLenM
        {
            get { return this.left_lug_len_m; }
            set
            {
                this.left_lug_len_m = value;
                RaisePropertyChanged("LeftLugLenM");
            }
        }
        public string RightLugLenM
        {
            get { return this.right_lug_len_m; }
            set
            {
                this.right_lug_len_m = value;
                RaisePropertyChanged("RightLugLenM");
            }
        }

        public string BatLengthB
        {
            get { return this.bat_length_b; }
            set
            {
                this.bat_length_b = value;
                RaisePropertyChanged("BatLengthB");
            }
        }
        public string BatWidthB
        {
            get { return this.bat_width_b; }
            set
            {
                this.bat_width_b = value;
                RaisePropertyChanged("BatWidthB");
            }
        }
        public string LeftLugB
        {
            get { return this.left_lug_b; }
            set
            {
                this.left_lug_b = value;
                RaisePropertyChanged("LeftLugB");
            }
        }
        public string RightLugB
        {
            get { return this.right_lug_b; }
            set
            {
                this.right_lug_b = value;
                RaisePropertyChanged("RightLugB");
            }
        }
        public string AllLengthB
        {
            get { return this.all_length_b; }
            set
            {
                this.all_length_b = value;
                RaisePropertyChanged("AllLengthB");
            }
        }
        public string Left1GlueB
        {
            get { return this.left_1_glue_b; }
            set
            {
                this.left_1_glue_b = value;
                RaisePropertyChanged("Left1GlueB");
            }
        }
        public string Left2GlueB
        {
            get { return this.left_2_glue_b; }
            set
            {
                this.left_2_glue_b = value;
                RaisePropertyChanged("Left2GlueB");
            }
        }
        public string Right1GlueB
        {
            get { return this.right_1_glue_b; }
            set
            {
                this.right_1_glue_b = value;
                RaisePropertyChanged("Right1GlueB");
            }
        }
        public string Right2GlueB
        {
            get { return this.right_2_glue_b; }
            set
            {
                this.right_2_glue_b = value;
                RaisePropertyChanged("Right2GlueB");
            }
        }
        public string LeftLugLenB
        {
            get { return this.left_lug_len_b; }
            set
            {
                this.left_lug_len_b = value;
                RaisePropertyChanged("LeftLugLenB");
            }
        }
        public string RightLugLenB
        {
            get { return this.right_lug_len_b; }
            set
            {
                this.right_lug_len_b = value;
                RaisePropertyChanged("RightLugLenB");
            }
        }

        #endregion

        public float MinBatLength
        {
            get { return this.minBatLength; }
            set
            {
                this.minBatLength = value;
                RaisePropertyChanged("MinBatLength");
            }
        }

        public float MaxBatLength
        {
            get { return this.maxBatLength; }
            set
            {
                this.maxBatLength = value;
                RaisePropertyChanged("MaxBatLength");
            }
        }

        public float MinBatWidth
        {
            get { return this.minBatWidth; }
            set
            {
                this.minBatWidth = value;
                RaisePropertyChanged("MinBatWidth");
            }
        }

        public float MaxBatWidth
        {
            get { return this.maxBatWidth; }
            set
            {
                this.maxBatWidth = value;
                RaisePropertyChanged("MaxBatWidth");
            }
        }

        public float MinLeftLugMargin
        {
            get { return this.minLeftLugMargin; }
            set
            {
                this.minLeftLugMargin = value;
                RaisePropertyChanged("MinLeftLugMargin");
            }
        }

        public float MaxLeftLugMargin
        {
            get { return this.maxLeftLugMargin; }
            set
            {
                this.maxLeftLugMargin = value;
                RaisePropertyChanged("MaxLeftLugMargin");
            }
        }

        public float MinRightLugMargin
        {
            get { return this.minRightLugMargin; }
            set
            {
                this.minRightLugMargin = value;
                RaisePropertyChanged("MinRightLugMargin");
            }
        }

        public float MaxRightLugMargin
        {
            get { return this.maxRightLugMargin; }
            set
            {
                this.maxRightLugMargin = value;
                RaisePropertyChanged("MaxRightLugMargin");
            }
        }

        public float MinLeftLugLength
        {
            get { return this.minLeftLugLength; }
            set
            {
                this.minLeftLugLength = value;
                RaisePropertyChanged("MinLeftLugLength");
            }
        }

        public float MaxLeftLugLength
        {
            get { return this.maxLeftLugLength; }
            set
            {
                this.maxLeftLugLength = value;
                RaisePropertyChanged("MaxLeftLugLength");
            }
        }

        public float MinRightLugLength
        {
            get { return this.minRightLugLength; }
            set
            {
                this.minRightLugLength = value;
                RaisePropertyChanged("MinRightLugLength");
            }
        }

        public float MaxRightLugLength
        {
            get { return this.maxRightLugLength; }
            set
            {
                this.maxRightLugLength = value;
                RaisePropertyChanged("MaxRightLugLength");
            }
        }

        public float MinAllBatLength
        {
            get { return this.minAllBatLength; }
            set
            {
                this.minAllBatLength = value;
                RaisePropertyChanged("MinAllBatLength");
            }
        }

        public float MaxAllBatLength
        {
            get { return this.maxAllBatLength; }
            set
            {
                this.maxAllBatLength = value;
                RaisePropertyChanged("MaxAllBatLength");
            }
        }

        public float MinLeft1WhiteGlue
        {
            get { return this.minLeft1WhiteGlue; }
            set
            {
                this.minLeft1WhiteGlue = value;
                RaisePropertyChanged("MinLeft1WhiteGlue");
            }
        }

        public float MinLeft1WhiteGlueMin
        {
            get { return this.minLeft1WhiteGlueMin; }
            set
            {
                this.minLeft1WhiteGlueMin = value;
                RaisePropertyChanged("MinLeft1WhiteGlueMin");
            }
        }

        public float MaxLeft1WhiteGlue
        {
            get { return this.maxLeft1WhiteGlue; }
            set
            {
                this.maxLeft1WhiteGlue = value;
                RaisePropertyChanged("MaxLeft1WhiteGlue");
            }
        }

        public float MaxLeft1WhiteGlueMin
        {
            get { return this.maxLeft1WhiteGlueMin; }
            set
            {
                this.maxLeft1WhiteGlueMin = value;
                RaisePropertyChanged("MaxLeft1WhiteGlueMin");
            }
        }

        public float MinLeft2WhiteGlue
        {
            get { return this.minLeft2WhiteGlue; }
            set
            {
                this.minLeft2WhiteGlue = value;
                RaisePropertyChanged("MinLeft2WhiteGlue");
            }
        }

        public float MinLeft2WhiteGlueMin
        {
            get { return this.minLeft2WhiteGlueMin; }
            set
            {
                this.minLeft2WhiteGlueMin = value;
                RaisePropertyChanged("MinLeft2WhiteGlueMin");
            }
        }

        public float MaxLeft2WhiteGlue
        {
            get { return this.maxLeft2WhiteGlue; }
            set
            {
                this.maxLeft2WhiteGlue = value;
                RaisePropertyChanged("MaxLeft2WhiteGlue");
            }
        }

        public float MaxLeft2WhiteGlueMin
        {
            get { return this.maxLeft2WhiteGlueMin; }
            set
            {
                this.maxLeft2WhiteGlueMin = value;
                RaisePropertyChanged("MaxLeft2WhiteGlueMin");
            }
        }

        public float MinRight1WhiteGlue
        {
            get { return this.minRight1WhiteGlue; }
            set
            {
                this.minRight1WhiteGlue = value;
                RaisePropertyChanged("MinRight1WhiteGlue");
            }
        }

        public float MinRight1WhiteGlueMin
        {
            get { return this.minRight1WhiteGlueMin; }
            set
            {
                this.minRight1WhiteGlueMin = value;
                RaisePropertyChanged("MinRight1WhiteGlueMin");
            }
        }

        public float MaxRight1WhiteGlue
        {
            get { return this.maxRight1WhiteGlue; }
            set
            {
                this.maxRight1WhiteGlue = value;
                RaisePropertyChanged("MaxRight1WhiteGlue");
            }
        }

        public float MaxRight1WhiteGlueMin
        {
            get { return this.maxRight1WhiteGlueMin; }
            set
            {
                this.maxRight1WhiteGlueMin = value;
                RaisePropertyChanged("MaxRight1WhiteGlueMin");
            }
        }

        public float MinRight2WhiteGlue
        {
            get { return this.minRight2WhiteGlue; }
            set
            {
                this.minRight2WhiteGlue = value;
                RaisePropertyChanged("MinRight2WhiteGlue");
            }
        }

        public float MinRight2WhiteGlueMin
        {
            get { return this.minRight2WhiteGlueMin; }
            set
            {
                this.minRight2WhiteGlueMin = value;
                RaisePropertyChanged("MinRight2WhiteGlueMin");
            }
        }

        public float MaxRight2WhiteGlue
        {
            get { return this.maxRight2WhiteGlue; }
            set
            {
                this.maxRight2WhiteGlue = value;
                RaisePropertyChanged("MaxRight2WhiteGlue");
            }
        }
        public float MaxRight2WhiteGlueMin
        {
            get { return this.maxRight2WhiteGlueMin; }
            set
            {
                this.maxRight2WhiteGlueMin = value;
                RaisePropertyChanged("MaxRight2WhiteGlueMin");
            }
        }

        public bool IsNoUpLoadMdiAndPPGData
        {
            get { return this._isNoUpLoadMdiAndPPGData; }
            set
            {
                this._isNoUpLoadMdiAndPPGData = value;
                RaisePropertyChanged("IsNoUpLoadMdiAndPPGData");
            }
        }

        public bool IsAlOnLeft
        {
            get { return this._isAlOnLeft; }
            set
            {
                this._isAlOnLeft = value;
                RaisePropertyChanged("IsAlOnLeft");
            }
        }

        public float MaxResistance
        {
            get { return this.maxResistance; }
            set
            {
                this.maxResistance = value;
                RaisePropertyChanged("MaxResistance");
            }
        }

        public float MinResistance
        {
            get { return this.minResistance; }
            set
            {
                this.minResistance = value;
                RaisePropertyChanged("MinResistance");
            }
        }

        public float MinVoltage
        {
            get { return this.minVoltage; }
            set
            {
                this.minVoltage = value;
                RaisePropertyChanged("MinVoltage");
            }
        }

        public float MaxVoltage
        {
            get { return this.maxVoltage; }
            set
            {
                this.maxVoltage = value;
                RaisePropertyChanged("MaxVoltage");
            }
        }

        public float MinOCV_k
        {
            get { return this.minOCV_k; }
            set
            {
                this.minOCV_k = value;
                RaisePropertyChanged("MinOCV_k");
            }
        }

        public float MaxOCV_k
        {
            get { return this.maxOCV_k; }
            set
            {
                this.maxOCV_k = value;
                RaisePropertyChanged("MaxOCV_k");
            }
        }

        public float TemperatureCoefficient
        {
            get { return this.temperatureCoefficient; }
            set
            {
                this.temperatureCoefficient = value;
                RaisePropertyChanged("TemperatureCoefficient");
            }
        }

        public float TemperatureCoefficient2
        {
            get { return this.temperatureCoefficient2; }
            set
            {
                this.temperatureCoefficient2 = value;
                RaisePropertyChanged("TemperatureCoefficient2");
            }
        }

        public float VoltageCoefficient
        {
            get { return this._voltageCoefficient; }
            set
            {
                this._voltageCoefficient = value;
                RaisePropertyChanged("VoltageCoefficient");
            }
        }

        public float ResistanceCoefficient
        {
            get { return this._resistanceCoefficient; }
            set
            {
                this._resistanceCoefficient = value;
                RaisePropertyChanged("ResistanceCoefficient");
            }
        }

        public float ResistanceFixedValue
        {
            get { return this._resistanceFixedValue; }
            set
            {
                this._resistanceFixedValue = value;
                RaisePropertyChanged("ResistanceFixedValue");
            }
        }

        public float TemperatureFixedValue
        {
            get { return this._temperatureFixedValue; }
            set
            {
                this._temperatureFixedValue = value;
                RaisePropertyChanged("TemperatureFixedValue");
            }
        }

        public bool IsNoResistance
        {
            get { return this._isNoResistance; }
            set
            {
                this._isNoResistance = value;
                RaisePropertyChanged("IsNoResistance");
            }
        }

        public bool IsNoTemperature
        {
            get { return this._isNoTemperature; }
            set
            {
                this._isNoTemperature = value;
                RaisePropertyChanged("IsNoTemperature");
            }
        }

        public bool IsCheckMarking
        {
            get { return this._isCheckMarking; }
            set
            {
                this._isCheckMarking = value;
                RaisePropertyChanged("IsCheckMarking");
            }
        }

        public bool IsCheckWeekCount
        {
            get { return this._isCheckWeekCount; }
            set
            {
                this._isCheckWeekCount = value;
                RaisePropertyChanged("IsCheckWeekCount");
            }
        }

        public bool IsSetMarking
        {
            get { return this._isSetMarking; }
            set
            {
                this._isSetMarking = value;
                RaisePropertyChanged("IsSetMarking");
            }
        }

        public bool IsCheckMI
        {
            get { return this._isCheckMI; }
            set
            {
                this._isCheckMI = value;
                RaisePropertyChanged("IsCheckMI");
            }
        }

        public float MinTemperature
        {
            get { return this.minTemperature; }
            set
            {
                this.minTemperature = value;
                RaisePropertyChanged("MinTemperature");
            }
        }

        public float MaxTemperature
        {
            get { return this.maxTemperature; }
            set
            {
                this.maxTemperature = value;
                RaisePropertyChanged("MaxTemperature");
            }
        }

        public string Mi
        {
            get { return this._mi; }
            set
            {
                this._mi = value;
                RaisePropertyChanged("Mi");
            }
        }

        public string Marking
        {
            get { return this._marking; }
            set
            {
                this._marking = value;
                RaisePropertyChanged("Marking");
            }
        }

        public string SetMarking
        {
            get { return this._setMarking; }
            set
            {
                this._setMarking = value;
                RaisePropertyChanged("SetMarking");
            }
        }

        public int WeekCounts
        {
            get { return this._weekCounts; }
            set
            {
                this._weekCounts = value;
                RaisePropertyChanged("WeekCounts");
            }
        }

        public float ThicknessAvgDiff
        {
            get { return this.thicknessAvgDiff; }
            set
            {
                this.thicknessAvgDiff = value;
                RaisePropertyChanged("ThicknessAvgDiff");
            }
        }

        public float MinIV
        {
            get { return this.minIV; }
            set
            {
                this.minIV = value;
                RaisePropertyChanged("MinIV");
            }
        }
        public float MaxIV
        {
            get { return this.maxIV; }
            set
            {
                this.maxIV = value;
                RaisePropertyChanged("MaxIV");
            }
        }
        public float Source
        {
            get { return this.source; }
            set
            {
                this.source = value;
                RaisePropertyChanged("Source");
            }
        }
        public float Range
        {
            get { return this.range; }
            set
            {
                this.range = value;
                RaisePropertyChanged("Range");
            }
        }
        public float ExceptionData1
        {
            get { return this.exceptionData1; }
            set
            {
                this.exceptionData1 = value;
                RaisePropertyChanged("ExceptionData1");
            }
        }
        public float ExceptionData2
        {
            get { return this.exceptionData2; }
            set
            {
                this.exceptionData2 = value;
                RaisePropertyChanged("ExceptionData2");
            }
        }

        public int IvTestTime
        {
            get { return this._ivTestTime; }
            set
            {
                this._ivTestTime = value;
                RaisePropertyChanged("IvTestTime");
            }
        }

        public int IvStation1Channel
        {
            get { return this._ivStation1Channel; }
            set
            {
                this._ivStation1Channel = value;
                RaisePropertyChanged("IvStation1Channel");
            }
        }

        public int IvStation2Channel
        {
            get { return this._ivStation2Channel; }
            set
            {
                this._ivStation2Channel = value;
                RaisePropertyChanged("IvStation2Channel");
            }
        }

        public int IvStation3Channel
        {
            get { return this._ivStation3Channel; }
            set
            {
                this._ivStation3Channel = value;
                RaisePropertyChanged("IvStation3Channel");
            }
        }

        public int IvStation4Channel
        {
            get { return this._ivStation4Channel; }
            set
            {
                this._ivStation4Channel = value;
                RaisePropertyChanged("IvStation4Channel");
            }
        }

        #endregion

        private CheckParamsConfig()
        {
            if (!File.Exists(this._configFile))
            {
                this.Write();
            }

            this.Read();
        }

        private void Read()
        {
            this.ReadGeneralParams();
            this.ReadInspectParams();
            this.ReadImageSaveConfig();
            this.ReadStartupConfig();
            this.ReadThicknessParams();
            this.ReadDimensionParams();
        }

        public void Write()
        {
            this.WriteGeneralParams();
            this.WriteInspectParams();
            this.WriteImageSaveConfig();
            this.SaveStartupConfig();
            this.WriteThicknessParams();
        }

        #region - General -

        private void ReadGeneralParams()
        {
            StringBuilder builder = new StringBuilder(1024);

            GetPrivateProfileString("General", "CheckMode", "FourSides", builder, 1024, this._configFile);
            this.CheckMode = (ECheckModes)Enum.Parse(typeof(ECheckModes), builder.ToString());

            GetPrivateProfileString("General", "MiDataDir", @"\\nd-app01\\XRaySpec\\MISpec.csv", builder, 1024, this._configFile);
            this._miDataDir = builder.ToString();

            //GetPrivateProfileString("General", "ReModelDir", @"\\nd-app01\\XRaySpec", builder, 1024, this._configFile);
            //this._reModelDir = builder.ToString();
            byte[] buffer = new byte[1024];
            int bufLen = GetPrivateProfileString("General", "ReModelDir", @"\\atlbattery.com\nd-fqi\MISpec-FQI正常规格", buffer, 1024, this._configFile);
            this._reModelDir = Encoding.GetEncoding(Encoding.Default.CodePage).GetString(buffer, 0, bufLen);

            GetPrivateProfileString("General", "OCVMode", "O1", builder, 1024, this._configFile);
            this.OCVMode = builder.ToString();

            GetPrivateProfileString("General", "FactoryCode", "", builder, 1024, this._configFile);
            this.FactoryCode = builder.ToString();

            GetPrivateProfileString("General", "OCVWarmingCounts", "3", builder, 1024, this._configFile);
            this.OCVWarmingCounts = Convert.ToInt32(builder.ToString());

            GetPrivateProfileString("General", "RangeOfTemperatrue", "5", builder, 1024, this._configFile);
            this.RangeOfTemperatrue = Convert.ToInt32(builder.ToString());

            GetPrivateProfileString("General", "MDIWarmingCounts", "3", builder, 1024, this._configFile);
            this.MDIWarmingCounts = Convert.ToInt32(builder.ToString());

            GetPrivateProfileString("General", "PPGWarmingCounts", "3", builder, 1024, this._configFile);
            this.PPGWarmingCounts = Convert.ToInt32(builder.ToString());

            GetPrivateProfileString("General", "WarmWaitTime", "60", builder, 1024, this._configFile);
            this.WarmWaitTime = Convert.ToInt32(builder.ToString());

            GetPrivateProfileString("General", "StationRangeOutTimes", "5", builder, 1024, this._configFile);
            this.StationRangeOutTimes = Convert.ToInt32(builder.ToString());

            GetPrivateProfileString("General", "ProductMode", "123", builder, 1024, this._configFile);
            this.ProductMode = builder.ToString();

            GetPrivateProfileString("General", "PcName", "A795", builder, 1024, this._configFile);
            this.PcName = builder.ToString();

            GetPrivateProfileString("General", "CellType", "", builder, 1024, this._configFile);
            this.CellType = builder.ToString();

            GetPrivateProfileString("General", "IsCheckABCell", "False", builder, 1024, this._configFile);
            this.IsCheckABCell = bool.Parse(builder.ToString());

            GetPrivateProfileString("General", "MarkingOfACell", "GRFTWF", builder, 1024, this._configFile);
            this.MarkingOfACell = builder.ToString();

            GetPrivateProfileString("General", "MarkingBase", "", builder, 1024, this._configFile);
            this.MarkingBase = builder.ToString();

            GetPrivateProfileString("General", "MarkingCurrent", "", builder, 1024, this._configFile);
            this.MarkingCurrent = builder.ToString();

            GetPrivateProfileString("General", "IsTiffMode", "True", builder, 1024, this._configFile);
            this.IsTiffMode = bool.Parse(builder.ToString());

            GetPrivateProfileString("General", "IvCheckTime", "2021-01-01 08:00:00", builder, 1024, this._configFile);
            this.IvCheckTime = builder.ToString();

            GetPrivateProfileString("General", "OcvCheckTime", "2021-01-01 08:00:00", builder, 1024, this._configFile);
            this.OcvCheckTime = builder.ToString();

            GetPrivateProfileString("General", "MdiCheckTime", "2021-01-01 08:00:00", builder, 1024, this._configFile);
            this.MdiCheckTime = builder.ToString();

            GetPrivateProfileString("General", "PpgCheckTime", "2021-01-01 08:00:00", builder, 1024, this._configFile);
            this.PpgCheckTime = builder.ToString();

            GetPrivateProfileString("General", "XrayCheckTime", "2021-01-01 08:00:00", builder, 1024, this._configFile);
            this.XrayCheckTime = builder.ToString();

            GetPrivateProfileString("General", "NeedleCheckTime", "2021-01-01 08:00:00", builder, 1024, this._configFile);
            this.NeedleCheckTime = builder.ToString();

            GetPrivateProfileString("General", "LinearCheckTime", "2021-01-01 08:00:00", builder, 1024, this._configFile);
            this.LinearCheckTime = builder.ToString();
        }

        public void WriteGeneralParams()
        {
            WritePrivateProfileString("General", "CheckMode", this.CheckMode.ToString(), this._configFile);

            WritePrivateProfileString("General", "MiDataDir", this._miDataDir, this._configFile);

            WritePrivateProfileString("General", "ReModelDir", this._reModelDir, this._configFile);

            WritePrivateProfileString("General", "OCVMode", this.OCVMode, this._configFile);

            WritePrivateProfileString("General", "FactoryCode", this.FactoryCode, this._configFile);

            WritePrivateProfileString("General", "OCVWarmingCounts", this.OCVWarmingCounts.ToString(), this._configFile);

            WritePrivateProfileString("General", "RangeOfTemperatrue", this.RangeOfTemperatrue.ToString(), this._configFile);

            WritePrivateProfileString("General", "MDIWarmingCounts", this.MDIWarmingCounts.ToString(), this._configFile);

            WritePrivateProfileString("General", "PPGWarmingCounts", this.PPGWarmingCounts.ToString(), this._configFile);

            WritePrivateProfileString("General", "WarmWaitTime", this.WarmWaitTime.ToString(), this._configFile);

            WritePrivateProfileString("General", "StationRangeOutTimes", this.StationRangeOutTimes.ToString(), this._configFile);

            WritePrivateProfileString("General", "ProductMode", this.ProductMode, this._configFile);

            WritePrivateProfileString("General", "PcName", this.PcName, this._configFile);

            WritePrivateProfileString("General", "CellType", this.CellType.ToString(), this._configFile);

            WritePrivateProfileString("General", "IsCheckABCell", this.IsCheckABCell.ToString(), this._configFile);

            WritePrivateProfileString("General", "MarkingOfACell", this.MarkingOfACell.ToString(), this._configFile);

            WritePrivateProfileString("General", "MarkingBase", this.MarkingBase, this._configFile);

            WritePrivateProfileString("General", "MarkingCurrent", this.MarkingCurrent, this._configFile);

            WritePrivateProfileString("General", "IsTiffMode", this.IsTiffMode.ToString(), this._configFile);

            WritePrivateProfileString("General", "IvCheckTime", this.IvCheckTime, this._configFile);

            WritePrivateProfileString("General", "OcvCheckTime", this.OcvCheckTime, this._configFile);

            WritePrivateProfileString("General", "MdiCheckTime", this.MdiCheckTime, this._configFile);

            WritePrivateProfileString("General", "PpgCheckTime", this.PpgCheckTime, this._configFile);

            WritePrivateProfileString("General", "XrayCheckTime", this.XrayCheckTime, this._configFile);

            WritePrivateProfileString("General", "NeedleCheckTime", this.NeedleCheckTime, this._configFile);

            WritePrivateProfileString("General", "LinearCheckTime", this.LinearCheckTime, this._configFile);
        }

        #endregion

        #region - InspectParams W/R -

        //检测参数配置文件
        private void ReadInspectParams()
        {
            StringBuilder builder = new StringBuilder(1024);

            GetPrivateProfileString("InspectParamsConfig", "ProductNO", "M90", builder, 1024, this._configFile);
            this._productNO = builder.ToString();

            GetPrivateProfileString("InspectParamsConfig", "iLine", @"1", builder, 1024, this._configFile);
            this._inspectParams.iLine = int.Parse(builder.ToString());

            GetPrivateProfileString("InspectParamsConfig", "iCorner", @"1", builder, 1024, this._configFile);
            this._inspectParams.iCorner = int.Parse(builder.ToString());

            GetPrivateProfileString("InspectParamsConfig", "total_layer", @"5", builder, 1024, this._configFile);
            this._inspectParams.total_layer = int.Parse(builder.ToString());

            GetPrivateProfileString("InspectParamsConfig", "pixel_to_mm", @"0.01", builder, 1024, this._configFile);
            this._inspectParams.pixel_to_mm = float.Parse(builder.ToString());

            GetPrivateProfileString("InspectParamsConfig", "min_length_head", @"0.1", builder, 1024, this._configFile);
            this.min_length_head = float.Parse(builder.ToString());

            GetPrivateProfileString("InspectParamsConfig", "max_length_head", @"1.4", builder, 1024, this._configFile);
            this.max_length_head = float.Parse(builder.ToString());

            GetPrivateProfileString("InspectParamsConfig", "min_length_tail", @"0.1", builder, 1024, this._configFile);
            this.min_length_tail = float.Parse(builder.ToString());

            GetPrivateProfileString("InspectParamsConfig", "max_length_tail", @"1.4", builder, 1024, this._configFile);
            this.max_length_tail = float.Parse(builder.ToString());

            GetPrivateProfileString("InspectParamsConfig", "max_angle_thresh", @"5", builder, 1024, this._configFile);
            this._inspectParams.max_angle_thresh = int.Parse(builder.ToString());

            GetPrivateProfileString("InspectParamsConfig", "isDrawLine", @"true", builder, 1024, this._configFile);
            this._inspectParams.isDrawLine = bool.Parse(builder.ToString());

            GetPrivateProfileString("InspectParamsConfig", "isShowData", @"true", builder, 1024, this._configFile);
            this._inspectParams.isShowData = bool.Parse(builder.ToString());

            GetPrivateProfileString("InspectParamsConfig", "isShowAngle", @"true", builder, 1024, this._configFile);
            this._inspectParams.isShowAngle = bool.Parse(builder.ToString());

            GetPrivateProfileString("InspectParamsConfig", "isDetectAngle", @"true", builder, 1024, this._configFile);
            this._inspectParams.isDetectAngle = bool.Parse(builder.ToString());

            GetPrivateProfileString("InspectParamsConfig", "x", @"0", builder, 1024, this._configFile);
            this._inspectParams.detected_rect.x = int.Parse(builder.ToString());

            GetPrivateProfileString("InspectParamsConfig", "y", @"0", builder, 1024, this._configFile);
            this._inspectParams.detected_rect.y = int.Parse(builder.ToString());

            GetPrivateProfileString("InspectParamsConfig", "width", @"90", builder, 1024, this._configFile);
            this._inspectParams.detected_rect.width = int.Parse(builder.ToString());

            GetPrivateProfileString("InspectParamsConfig", "height", @"200", builder, 1024, this._configFile);
            this._inspectParams.detected_rect.height = int.Parse(builder.ToString());

            GetPrivateProfileString("InspectParamsConfig", "total_layer_bd", @"5", builder, 1024, this._configFile);
            this.TotalLayersBD = int.Parse(builder.ToString());

            GetPrivateProfileString("InspectParamsConfig", "width_bd", @"90", builder, 1024, this._configFile);
            this.RectWidthBD = int.Parse(builder.ToString());

            GetPrivateProfileString("InspectParamsConfig", "height_bd", @"200", builder, 1024, this._configFile);
            this.RectHeightBD = int.Parse(builder.ToString());

            GetPrivateProfileString("InspectParamsConfig", "isSamplingMode", @"true", builder, 1024, this._configFile);
            this.IsSamplingMode = bool.Parse(builder.ToString());

            GetPrivateProfileString("InspectParamsConfig", "isNoUploadMES", @"true", builder, 1024, this._configFile);
            this.IsNoUploadMES = bool.Parse(builder.ToString());

            GetPrivateProfileString("InspectParamsConfig", "max_thickness", @"90", builder, 1024, this._configFile);
            this.MaxThickness = float.Parse(builder.ToString());

            GetPrivateProfileString("InspectParamsConfig", "min_thickness", @"1.4", builder, 1024, this._configFile);
            this.MinThickness = float.Parse(builder.ToString());

            GetPrivateProfileString("InspectParamsConfig", "calival_thickness", @"90", builder, 1024, this._configFile);
            this.CaliValThickness = float.Parse(builder.ToString());

            GetPrivateProfileString("InspectParamsConfig", "k_thickness", @"90", builder, 1024, this._configFile);
            this.ThicknessKValue = float.Parse(builder.ToString());

            GetPrivateProfileString("InspectParamsConfig", "b_thickness", @"90", builder, 1024, this._configFile);
            this.ThicknessBValue = float.Parse(builder.ToString());

            GetPrivateProfileString("InspectParamsConfig", "CellKValue", @"1", builder, 1024, this._configFile);
            this.CellKValue = float.Parse(builder.ToString());

            GetPrivateProfileString("InspectParamsConfig", "CellBValue", @"0", builder, 1024, this._configFile);
            this.CellBValue = float.Parse(builder.ToString());

            GetPrivateProfileString("InspectParamsConfig", "StationRange", @"0.014", builder, 1024, this._configFile);
            this.StationRange = float.Parse(builder.ToString());

            GetPrivateProfileString("InspectParamsConfig", "StationRangeNum", @"90", builder, 1024, this._configFile);
            this.StationRangeNum = int.Parse(builder.ToString());

            GetPrivateProfileString("InspectParamsConfig", "StationWarmingAverage", @"4.98", builder, 1024, this._configFile);
            this.StationWarmingAverage = float.Parse(builder.ToString());

            GetPrivateProfileString("InspectParamsConfig", "StationWarmingTolerance", @"0.2", builder, 1024, this._configFile);
            this.StationWarmingTolerance = float.Parse(builder.ToString());

            GetPrivateProfileString("InspectParamsConfig", "MinBatLength", @"0", builder, 1024, this._configFile);
            this.MinBatLength = float.Parse(builder.ToString());

            GetPrivateProfileString("InspectParamsConfig", "MaxBatLength", @"0", builder, 1024, this._configFile);
            this.MaxBatLength = float.Parse(builder.ToString());

            GetPrivateProfileString("InspectParamsConfig", "MinBatWidth", @"0", builder, 1024, this._configFile);
            this.MinBatWidth = float.Parse(builder.ToString());

            GetPrivateProfileString("InspectParamsConfig", "MaxBatWidth", @"0", builder, 1024, this._configFile);
            this.MaxBatWidth = float.Parse(builder.ToString());

            GetPrivateProfileString("InspectParamsConfig", "MinLeftLugMargin", @"0", builder, 1024, this._configFile);
            this.MinLeftLugMargin = float.Parse(builder.ToString());

            GetPrivateProfileString("InspectParamsConfig", "MaxLeftLugMargin", @"0", builder, 1024, this._configFile);
            this.MaxLeftLugMargin = float.Parse(builder.ToString());

            GetPrivateProfileString("InspectParamsConfig", "MinRightLugMargin", @"0", builder, 1024, this._configFile);
            this.MinRightLugMargin = float.Parse(builder.ToString());

            GetPrivateProfileString("InspectParamsConfig", "MaxRightLugMargin", @"0", builder, 1024, this._configFile);
            this.MaxRightLugMargin = float.Parse(builder.ToString());

            GetPrivateProfileString("InspectParamsConfig", "MinLeftLugLength", @"0", builder, 1024, this._configFile);
            this.MinLeftLugLength = float.Parse(builder.ToString());

            GetPrivateProfileString("InspectParamsConfig", "MaxLeftLugLength", @"0", builder, 1024, this._configFile);
            this.MaxLeftLugLength = float.Parse(builder.ToString());

            GetPrivateProfileString("InspectParamsConfig", "MinRightLugLength", @"0", builder, 1024, this._configFile);
            this.MinRightLugLength = float.Parse(builder.ToString());

            GetPrivateProfileString("InspectParamsConfig", "MaxRightLugLength", @"0", builder, 1024, this._configFile);
            this.MaxRightLugLength = float.Parse(builder.ToString());

            GetPrivateProfileString("InspectParamsConfig", "MinAllBatLength", @"0", builder, 1024, this._configFile);
            this.MinAllBatLength = float.Parse(builder.ToString());

            GetPrivateProfileString("InspectParamsConfig", "MaxAllBatLength", @"0", builder, 1024, this._configFile);
            this.MaxAllBatLength = float.Parse(builder.ToString());

            GetPrivateProfileString("InspectParamsConfig", "MinLeft1WhiteGlue", @"0", builder, 1024, this._configFile);
            this.MinLeft1WhiteGlue = float.Parse(builder.ToString());

            GetPrivateProfileString("InspectParamsConfig", "MaxLeft1WhiteGlue", @"0", builder, 1024, this._configFile);
            this.MaxLeft1WhiteGlue = float.Parse(builder.ToString());

            GetPrivateProfileString("InspectParamsConfig", "MinLeft2WhiteGlue", @"0", builder, 1024, this._configFile);
            this.MinLeft2WhiteGlue = float.Parse(builder.ToString());

            GetPrivateProfileString("InspectParamsConfig", "MaxLeft2WhiteGlue", @"0", builder, 1024, this._configFile);
            this.MaxLeft2WhiteGlue = float.Parse(builder.ToString());

            GetPrivateProfileString("InspectParamsConfig", "MinRight1WhiteGlue", @"0", builder, 1024, this._configFile);
            this.MinRight1WhiteGlue = float.Parse(builder.ToString());

            GetPrivateProfileString("InspectParamsConfig", "MaxRight1WhiteGlue", @"0", builder, 1024, this._configFile);
            this.MaxRight1WhiteGlue = float.Parse(builder.ToString());

            GetPrivateProfileString("InspectParamsConfig", "MinRight2WhiteGlue", @"0", builder, 1024, this._configFile);
            this.MinRight2WhiteGlue = float.Parse(builder.ToString());

            GetPrivateProfileString("InspectParamsConfig", "MaxRight2WhiteGlue", @"0", builder, 1024, this._configFile);
            this.MaxRight2WhiteGlue = float.Parse(builder.ToString());
            GetPrivateProfileString("InspectParamsConfig", "MinLeft1WhiteGlueMin", @"0", builder, 1024, this._configFile);
            this.MinLeft1WhiteGlueMin = float.Parse(builder.ToString());

            GetPrivateProfileString("InspectParamsConfig", "MaxLeft1WhiteGlueMin", @"0", builder, 1024, this._configFile);
            this.MaxLeft1WhiteGlueMin = float.Parse(builder.ToString());

            GetPrivateProfileString("InspectParamsConfig", "MinLeft2WhiteGlueMin", @"0", builder, 1024, this._configFile);
            this.MinLeft2WhiteGlueMin = float.Parse(builder.ToString());

            GetPrivateProfileString("InspectParamsConfig", "MaxLeft2WhiteGlueMin", @"0", builder, 1024, this._configFile);
            this.MaxLeft2WhiteGlueMin = float.Parse(builder.ToString());

            GetPrivateProfileString("InspectParamsConfig", "MinRight1WhiteGlueMin", @"0", builder, 1024, this._configFile);
            this.MinRight1WhiteGlueMin = float.Parse(builder.ToString());

            GetPrivateProfileString("InspectParamsConfig", "MaxRight1WhiteGlueMin", @"0", builder, 1024, this._configFile);
            this.MaxRight1WhiteGlueMin = float.Parse(builder.ToString());

            GetPrivateProfileString("InspectParamsConfig", "MinRight2WhiteGlueMin", @"0", builder, 1024, this._configFile);
            this.MinRight2WhiteGlueMin = float.Parse(builder.ToString());

            GetPrivateProfileString("InspectParamsConfig", "MaxRight2WhiteGlueMin", @"0", builder, 1024, this._configFile);
            this.MaxRight2WhiteGlueMin = float.Parse(builder.ToString());

            GetPrivateProfileString("InspectParamsConfig", "IsNoUpLoadMdiAndPPGData", @"False", builder, 1024, this._configFile);
            this.IsNoUpLoadMdiAndPPGData = bool.Parse(builder.ToString());

            GetPrivateProfileString("InspectParamsConfig", "IsAlOnLeft", @"True", builder, 1024, this._configFile);
            this.IsAlOnLeft = bool.Parse(builder.ToString());

            GetPrivateProfileString("InspectParamsConfig", "MinResistance", @"0", builder, 1024, this._configFile);
            this.MinResistance = float.Parse(builder.ToString());

            GetPrivateProfileString("InspectParamsConfig", "MaxResistance", @"0", builder, 1024, this._configFile);
            this.MaxResistance = float.Parse(builder.ToString());

            GetPrivateProfileString("InspectParamsConfig", "MinVoltage", @"0", builder, 1024, this._configFile);
            this.MinVoltage = float.Parse(builder.ToString());

            GetPrivateProfileString("InspectParamsConfig", "MaxVoltage", @"0", builder, 1024, this._configFile);
            this.MaxVoltage = float.Parse(builder.ToString());

            GetPrivateProfileString("InspectParamsConfig", "MinOCV_k", @"0", builder, 1024, this._configFile);
            this.MinOCV_k = float.Parse(builder.ToString());

            GetPrivateProfileString("InspectParamsConfig", "MaxOCV_k", @"0", builder, 1024, this._configFile);
            this.MaxOCV_k = float.Parse(builder.ToString());

            GetPrivateProfileString("InspectParamsConfig", "MinTemperature", @"0", builder, 1024, this._configFile);
            this.MinTemperature = float.Parse(builder.ToString());

            GetPrivateProfileString("InspectParamsConfig", "MaxTemperature", @"0", builder, 1024, this._configFile);
            this.MaxTemperature = float.Parse(builder.ToString());

            GetPrivateProfileString("InspectParamsConfig", "TemperatureCoefficient", @"0", builder, 1024, this._configFile);
            this.TemperatureCoefficient = float.Parse(builder.ToString());

            GetPrivateProfileString("InspectParamsConfig", "TemperatureCoefficient2", @"0", builder, 1024, this._configFile);
            this.TemperatureCoefficient2 = float.Parse(builder.ToString());

            GetPrivateProfileString("InspectParamsConfig", "VoltageCoefficient", @"0", builder, 1024, this._configFile);
            this.VoltageCoefficient = float.Parse(builder.ToString());

            GetPrivateProfileString("InspectParamsConfig", "ResistanceCoefficient", @"0", builder, 1024, this._configFile);
            this.ResistanceCoefficient = float.Parse(builder.ToString());

            GetPrivateProfileString("InspectParamsConfig", "ResistanceFixedValue", @"0", builder, 1024, this._configFile);
            this.ResistanceFixedValue = float.Parse(builder.ToString());

            GetPrivateProfileString("InspectParamsConfig", "TemperatureFixedValue", @"0", builder, 1024, this._configFile);
            this.TemperatureFixedValue = float.Parse(builder.ToString());

            GetPrivateProfileString("InspectParamsConfig", "Mi", @"ABC", builder, 1024, this._configFile);
            this.Mi = builder.ToString();

            GetPrivateProfileString("InspectParamsConfig", "WeekCounts", @"1", builder, 1024, this._configFile);
            this.WeekCounts = int.Parse(builder.ToString());

            GetPrivateProfileString("InspectParamsConfig", "Marking", @"", builder, 1024, this._configFile);
            this.Marking = builder.ToString();

            GetPrivateProfileString("InspectParamsConfig", "SetMarking", @"", builder, 1024, this._configFile);
            this.SetMarking = builder.ToString();

            GetPrivateProfileString("InspectParamsConfig", "ThicknessAvgDiff", @"10", builder, 1024, this._configFile);
            this.ThicknessAvgDiff = float.Parse(builder.ToString());

            GetPrivateProfileString("InspectParamsConfig", "IsNoResistance", @"false", builder, 1024, this._configFile);
            this.IsNoResistance = bool.Parse(builder.ToString());

            GetPrivateProfileString("InspectParamsConfig", "IsNoTemperature", @"false", builder, 1024, this._configFile);
            this.IsNoTemperature = bool.Parse(builder.ToString());

            GetPrivateProfileString("InspectParamsConfig", "IsCheckMarking", @"false", builder, 1024, this._configFile);
            this.IsCheckMarking = bool.Parse(builder.ToString());

            GetPrivateProfileString("InspectParamsConfig", "IsCheckWeekCount", @"false", builder, 1024, this._configFile);
            this.IsCheckWeekCount = bool.Parse(builder.ToString());

            GetPrivateProfileString("InspectParamsConfig", "IsSetMarking", @"false", builder, 1024, this._configFile);
            this.IsSetMarking = bool.Parse(builder.ToString());

            GetPrivateProfileString("InspectParamsConfig", "IsCheckMI", @"false", builder, 1024, this._configFile);
            this.IsCheckMI = bool.Parse(builder.ToString());

            GetPrivateProfileString("InspectParamsConfig", "MinIV", @"-0.3", builder, 1024, this._configFile);
            this.MinIV = float.Parse(builder.ToString());

            GetPrivateProfileString("InspectParamsConfig", "MaxIV", @"0.95", builder, 1024, this._configFile);
            this.MaxIV = float.Parse(builder.ToString());

            GetPrivateProfileString("InspectParamsConfig", "Source", @"0.3", builder, 1024, this._configFile);
            this.Source = float.Parse(builder.ToString());

            GetPrivateProfileString("InspectParamsConfig", "Range", @"0.45", builder, 1024, this._configFile);
            this.Range = float.Parse(builder.ToString());

            GetPrivateProfileString("InspectParamsConfig", "ExceptionData1", @"-0.1", builder, 1024, this._configFile);
            this.ExceptionData1 = float.Parse(builder.ToString());

            GetPrivateProfileString("InspectParamsConfig", "ExceptionData2", @"0.95", builder, 1024, this._configFile);
            this.ExceptionData2 = float.Parse(builder.ToString());

            GetPrivateProfileString("InspectParamsConfig", "IvTestTime", @"1200", builder, 1024, this._configFile);
            this.IvTestTime = int.Parse(builder.ToString());

            GetPrivateProfileString("InspectParamsConfig", "IvStation1Channel", @"1", builder, 1024, this._configFile);
            this.IvStation1Channel = int.Parse(builder.ToString());

            GetPrivateProfileString("InspectParamsConfig", "IvStation2Channel", @"3", builder, 1024, this._configFile);
            this.IvStation2Channel = int.Parse(builder.ToString());

            GetPrivateProfileString("InspectParamsConfig", "IvStation3Channel", @"5", builder, 1024, this._configFile);
            this.IvStation3Channel = int.Parse(builder.ToString());

            GetPrivateProfileString("InspectParamsConfig", "IvStation4Channel", @"7", builder, 1024, this._configFile);
            this.IvStation4Channel = int.Parse(builder.ToString());

        }


        /// <summary>
        /// 厚度点检配置文件
        /// </summary>
        private void ReadThicknessParams()
        {
            StringBuilder builder = new StringBuilder(1024);

            GetPrivateProfileString("ThicknessParamsConfig", "currentModel", @"90", builder, 1024, this._configFile);
            this.CurrentModel = builder.ToString();

            GetPrivateProfileString("ThicknessParamsConfig", "max_thickness_s", @"90", builder, 1024, this._configFile);
            this.MaxThicknessS = float.Parse(builder.ToString());

            GetPrivateProfileString("ThicknessParamsConfig", "min_thickness_s", @"1.4", builder, 1024, this._configFile);
            this.MinThicknessS = float.Parse(builder.ToString());

            GetPrivateProfileString("ThicknessParamsConfig", "max_thickness_m", @"90", builder, 1024, this._configFile);
            this.MaxThicknessM = float.Parse(builder.ToString());

            GetPrivateProfileString("ThicknessParamsConfig", "min_thickness_m", @"1.4", builder, 1024, this._configFile);
            this.MinThicknessM = float.Parse(builder.ToString());

            GetPrivateProfileString("ThicknessParamsConfig", "max_thickness_b", @"90", builder, 1024, this._configFile);
            this.MaxThicknessB = float.Parse(builder.ToString());

            GetPrivateProfileString("ThicknessParamsConfig", "min_thickness_b", @"1.4", builder, 1024, this._configFile);
            this.MinThicknessB = float.Parse(builder.ToString());

            GetPrivateProfileString("ThicknessParamsConfig", "thicknessCalibrationModeA", @"0", builder, 1024, this._configFile);
            this.ThicknessCalibrationModeA = int.Parse(builder.ToString());

            GetPrivateProfileString("ThicknessParamsConfig", "thicknessCalibrationModeB", @"0", builder, 1024, this._configFile);
            this.ThicknessCalibrationModeB = int.Parse(builder.ToString());

            GetPrivateProfileString("ThicknessParamsConfig", "thicknessCalibrationModeC", @"0", builder, 1024, this._configFile);
            this.ThicknessCalibrationModeC = int.Parse(builder.ToString());

            GetPrivateProfileString("ThicknessParamsConfig", "thicknessCalibrationModeD", @"0", builder, 1024, this._configFile);
            this.ThicknessCalibrationModeD = int.Parse(builder.ToString());

            GetPrivateProfileString("ThicknessParamsConfig", "LinearNeedOK", "3", builder, 1024, this._configFile);
            this.LinearNeedOK = float.Parse(builder.ToString());

            //GetPrivateProfileString("ThicknessParamsConfig", "LinearNGNum", "0", builder, 1024, this._configFile);
            //this.LinearNGNum = float.Parse(builder.ToString());

            //GetPrivateProfileString("ThicknessParamsConfig", "LinearOKNum", "0", builder, 1024, this._configFile);
            //this.LinearOKNum = float.Parse(builder.ToString());

            GetPrivateProfileString("ThicknessParamsConfig", "LinearRange", "0.015", builder, 1024, this._configFile);
            this.LinearRange = float.Parse(builder.ToString());

        }

        private void ReadDimensionParams()
        {
            StringBuilder builder = new StringBuilder(1024);

            GetPrivateProfileString("DimensionParamsConfig", "bat_length_s", @"90", builder, 1024, this._configFile);
            this.BatLengthS = builder.ToString();
            GetPrivateProfileString("DimensionParamsConfig", "bat_width_s", @"90", builder, 1024, this._configFile);
            this.BatWidthS = builder.ToString();
            GetPrivateProfileString("DimensionParamsConfig", "left_lug_s", @"90", builder, 1024, this._configFile);
            this.LeftLugS = builder.ToString();
            GetPrivateProfileString("DimensionParamsConfig", "right_lug_s", @"90", builder, 1024, this._configFile);
            this.RightLugS = builder.ToString();
            GetPrivateProfileString("DimensionParamsConfig", "all_length_s", @"90", builder, 1024, this._configFile);
            this.AllLengthS = builder.ToString();
            GetPrivateProfileString("DimensionParamsConfig", "left_1_glue_s", @"90", builder, 1024, this._configFile);
            this.Left1GlueS = builder.ToString();
            GetPrivateProfileString("DimensionParamsConfig", "left_2_glue_s", @"90", builder, 1024, this._configFile);
            this.Left2GlueS = builder.ToString();
            GetPrivateProfileString("DimensionParamsConfig", "right_1_glue_s", @"90", builder, 1024, this._configFile);
            this.Right1GlueS = builder.ToString();
            GetPrivateProfileString("DimensionParamsConfig", "right_2_glue_s", @"90", builder, 1024, this._configFile);
            this.Right2GlueS = builder.ToString();
            GetPrivateProfileString("DimensionParamsConfig", "left_lug_len_s", @"90", builder, 1024, this._configFile);
            this.LeftLugLenS = builder.ToString();
            GetPrivateProfileString("DimensionParamsConfig", "right_lug_len_s", @"90", builder, 1024, this._configFile);
            this.RightLugLenS = builder.ToString();

            GetPrivateProfileString("DimensionParamsConfig", "bat_length_m", @"90", builder, 1024, this._configFile);
            this.BatLengthM = builder.ToString();
            GetPrivateProfileString("DimensionParamsConfig", "bat_width_m", @"90", builder, 1024, this._configFile);
            this.BatWidthM = builder.ToString();
            GetPrivateProfileString("DimensionParamsConfig", "left_lug_m", @"90", builder, 1024, this._configFile);
            this.LeftLugM = builder.ToString();
            GetPrivateProfileString("DimensionParamsConfig", "right_lug_m", @"90", builder, 1024, this._configFile);
            this.RightLugM = builder.ToString();
            GetPrivateProfileString("DimensionParamsConfig", "all_length_m", @"90", builder, 1024, this._configFile);
            this.AllLengthM = builder.ToString();
            GetPrivateProfileString("DimensionParamsConfig", "left_1_glue_m", @"90", builder, 1024, this._configFile);
            this.Left1GlueM = builder.ToString();
            GetPrivateProfileString("DimensionParamsConfig", "left_2_glue_m", @"90", builder, 1024, this._configFile);
            this.Left2GlueM = builder.ToString();
            GetPrivateProfileString("DimensionParamsConfig", "right_1_glue_m", @"90", builder, 1024, this._configFile);
            this.Right1GlueM = builder.ToString();
            GetPrivateProfileString("DimensionParamsConfig", "right_2_glue_m", @"90", builder, 1024, this._configFile);
            this.Right2GlueM = builder.ToString();
            GetPrivateProfileString("DimensionParamsConfig", "left_lug_len_m", @"90", builder, 1024, this._configFile);
            this.LeftLugLenM = builder.ToString();
            GetPrivateProfileString("DimensionParamsConfig", "right_lug_len_m", @"90", builder, 1024, this._configFile);
            this.RightLugLenM = builder.ToString();

            GetPrivateProfileString("DimensionParamsConfig", "bat_length_b", @"90", builder, 1024, this._configFile);
            this.BatLengthB = builder.ToString();
            GetPrivateProfileString("DimensionParamsConfig", "bat_width_b", @"90", builder, 1024, this._configFile);
            this.BatWidthB = builder.ToString();
            GetPrivateProfileString("DimensionParamsConfig", "left_lug_b", @"90", builder, 1024, this._configFile);
            this.LeftLugB = builder.ToString();
            GetPrivateProfileString("DimensionParamsConfig", "right_lug_b", @"90", builder, 1024, this._configFile);
            this.RightLugB = builder.ToString();
            GetPrivateProfileString("DimensionParamsConfig", "all_length_b", @"90", builder, 1024, this._configFile);
            this.AllLengthB = builder.ToString();
            GetPrivateProfileString("DimensionParamsConfig", "left_1_glue_b", @"90", builder, 1024, this._configFile);
            this.Left1GlueB = builder.ToString();
            GetPrivateProfileString("DimensionParamsConfig", "left_2_glue_b", @"90", builder, 1024, this._configFile);
            this.Left2GlueB = builder.ToString();
            GetPrivateProfileString("DimensionParamsConfig", "right_1_glue_b", @"90", builder, 1024, this._configFile);
            this.Right1GlueB = builder.ToString();
            GetPrivateProfileString("DimensionParamsConfig", "right_2_glue_b", @"90", builder, 1024, this._configFile);
            this.Right2GlueB = builder.ToString();
            GetPrivateProfileString("DimensionParamsConfig", "left_lug_len_b", @"90", builder, 1024, this._configFile);
            this.LeftLugLenB = builder.ToString();
            GetPrivateProfileString("DimensionParamsConfig", "right_lug_len_b", @"90", builder, 1024, this._configFile);
            this.RightLugLenB = builder.ToString();

        }

        private void WriteInspectParams()
        {
            WritePrivateProfileString("InspectParamsConfig", "ProductNO", this._productNO.ToString(), this._configFile);
            WritePrivateProfileString("InspectParamsConfig", "iLine", this._inspectParams.iLine.ToString(), this._configFile);
            WritePrivateProfileString("InspectParamsConfig", "iCorner", this._inspectParams.iCorner.ToString(), this._configFile);
            WritePrivateProfileString("InspectParamsConfig", "total_layer", this._inspectParams.total_layer.ToString(), this._configFile);
            WritePrivateProfileString("InspectParamsConfig", "pixel_to_mm", this._inspectParams.pixel_to_mm.ToString(), this._configFile);
            WritePrivateProfileString("InspectParamsConfig", "min_length_head", this.min_length_head.ToString(), this._configFile);
            WritePrivateProfileString("InspectParamsConfig", "max_length_head", this.max_length_head.ToString(), this._configFile);
            WritePrivateProfileString("InspectParamsConfig", "min_length_tail", this.min_length_tail.ToString(), this._configFile);
            WritePrivateProfileString("InspectParamsConfig", "max_length_tail", this.max_length_tail.ToString(), this._configFile);
            WritePrivateProfileString("InspectParamsConfig", "max_angle_thresh", this._inspectParams.max_angle_thresh.ToString(), this._configFile);

            WritePrivateProfileString("InspectParamsConfig", "isDrawLine", this._inspectParams.isDrawLine.ToString(), this._configFile);
            WritePrivateProfileString("InspectParamsConfig", "isShowData", this._inspectParams.isShowData.ToString(), this._configFile);
            WritePrivateProfileString("InspectParamsConfig", "isShowAngle", this._inspectParams.isShowAngle.ToString(), this._configFile);
            WritePrivateProfileString("InspectParamsConfig", "isDetectAngle", this._inspectParams.isDetectAngle.ToString(), this._configFile);
            WritePrivateProfileString("InspectParamsConfig", "x", this._inspectParams.detected_rect.x.ToString(), this._configFile);
            WritePrivateProfileString("InspectParamsConfig", "y", this._inspectParams.detected_rect.y.ToString(), this._configFile);
            WritePrivateProfileString("InspectParamsConfig", "width", this._inspectParams.detected_rect.width.ToString(), this._configFile);
            WritePrivateProfileString("InspectParamsConfig", "height", this._inspectParams.detected_rect.height.ToString(), this._configFile);

            WritePrivateProfileString("InspectParamsConfig", "total_layer_bd", this.TotalLayersBD.ToString(), this._configFile);
            WritePrivateProfileString("InspectParamsConfig", "width_bd", this.RectWidthBD.ToString(), this._configFile);
            WritePrivateProfileString("InspectParamsConfig", "height_bd", this.RectHeightBD.ToString(), this._configFile);

            WritePrivateProfileString("InspectParamsConfig", "isSamplingMode", this.IsSamplingMode.ToString(), this._configFile);
            WritePrivateProfileString("InspectParamsConfig", "isNoUploadMES", this.IsNoUploadMES.ToString(), this._configFile);
            WritePrivateProfileString("InspectParamsConfig", "max_thickness", this.MaxThickness.ToString(), this._configFile);
            WritePrivateProfileString("InspectParamsConfig", "min_thickness", this.MinThickness.ToString(), this._configFile);
            WritePrivateProfileString("InspectParamsConfig", "calival_thickness", this.CaliValThickness.ToString(), this._configFile);
            WritePrivateProfileString("InspectParamsConfig", "k_thickness", this.ThicknessKValue.ToString(), this._configFile);
            WritePrivateProfileString("InspectParamsConfig", "b_thickness", this.ThicknessBValue.ToString(), this._configFile);
            WritePrivateProfileString("InspectParamsConfig", "CellKValue", this.CellKValue.ToString(), this._configFile);
            WritePrivateProfileString("InspectParamsConfig", "CellBValue", this.CellBValue.ToString(), this._configFile);

            WritePrivateProfileString("InspectParamsConfig", "StationRange", this.StationRange.ToString(), this._configFile);
            WritePrivateProfileString("InspectParamsConfig", "StationRangeNum", this.StationRangeNum.ToString(), this._configFile);
            WritePrivateProfileString("InspectParamsConfig", "StationWarmingAverage", this.StationWarmingAverage.ToString(), this._configFile);
            WritePrivateProfileString("InspectParamsConfig", "StationWarmingTolerance", this.StationWarmingTolerance.ToString(), this._configFile);

            WritePrivateProfileString("InspectParamsConfig", "MinBatLength", this.MinBatLength.ToString(), this._configFile);
            WritePrivateProfileString("InspectParamsConfig", "MaxBatLength", this.MaxBatLength.ToString(), this._configFile);
            WritePrivateProfileString("InspectParamsConfig", "MinBatWidth", this.MinBatWidth.ToString(), this._configFile);
            WritePrivateProfileString("InspectParamsConfig", "MaxBatWidth", this.MaxBatWidth.ToString(), this._configFile);
            WritePrivateProfileString("InspectParamsConfig", "MinLeftLugMargin", this.MinLeftLugMargin.ToString(), this._configFile);
            WritePrivateProfileString("InspectParamsConfig", "MaxLeftLugMargin", this.MaxLeftLugMargin.ToString(), this._configFile);
            WritePrivateProfileString("InspectParamsConfig", "MinRightLugMargin", this.MinRightLugMargin.ToString(), this._configFile);
            WritePrivateProfileString("InspectParamsConfig", "MaxRightLugMargin", this.MaxRightLugMargin.ToString(), this._configFile);
            WritePrivateProfileString("InspectParamsConfig", "MinLeftLugLength", this.MinLeftLugLength.ToString(), this._configFile);
            WritePrivateProfileString("InspectParamsConfig", "MaxLeftLugLength", this.MaxLeftLugLength.ToString(), this._configFile);
            WritePrivateProfileString("InspectParamsConfig", "MinRightLugLength", this.MinRightLugLength.ToString(), this._configFile);
            WritePrivateProfileString("InspectParamsConfig", "MaxRightLugLength", this.MaxRightLugLength.ToString(), this._configFile);
            WritePrivateProfileString("InspectParamsConfig", "MinAllBatLength", this.MinAllBatLength.ToString(), this._configFile);
            WritePrivateProfileString("InspectParamsConfig", "MaxAllBatLength", this.MaxAllBatLength.ToString(), this._configFile);
            WritePrivateProfileString("InspectParamsConfig", "MinLeft1WhiteGlue", this.MinLeft1WhiteGlue.ToString(), this._configFile);
            WritePrivateProfileString("InspectParamsConfig", "MaxLeft1WhiteGlue", this.MaxLeft1WhiteGlue.ToString(), this._configFile);
            WritePrivateProfileString("InspectParamsConfig", "MinLeft2WhiteGlue", this.MinLeft2WhiteGlue.ToString(), this._configFile);
            WritePrivateProfileString("InspectParamsConfig", "MaxLeft2WhiteGlue", this.MaxLeft2WhiteGlue.ToString(), this._configFile);
            WritePrivateProfileString("InspectParamsConfig", "MinRight1WhiteGlue", this.MinRight1WhiteGlue.ToString(), this._configFile);
            WritePrivateProfileString("InspectParamsConfig", "MaxRight1WhiteGlue", this.MaxRight1WhiteGlue.ToString(), this._configFile);
            WritePrivateProfileString("InspectParamsConfig", "MinRight2WhiteGlue", this.MinRight2WhiteGlue.ToString(), this._configFile);
            WritePrivateProfileString("InspectParamsConfig", "MaxRight2WhiteGlue", this.MaxRight2WhiteGlue.ToString(), this._configFile);
            WritePrivateProfileString("InspectParamsConfig", "MinLeft1WhiteGlueMin", this.MinLeft1WhiteGlueMin.ToString(), this._configFile);
            WritePrivateProfileString("InspectParamsConfig", "MaxLeft1WhiteGlueMin", this.MaxLeft1WhiteGlueMin.ToString(), this._configFile);
            WritePrivateProfileString("InspectParamsConfig", "MinLeft2WhiteGlueMin", this.MinLeft2WhiteGlueMin.ToString(), this._configFile);
            WritePrivateProfileString("InspectParamsConfig", "MaxLeft2WhiteGlueMin", this.MaxLeft2WhiteGlueMin.ToString(), this._configFile);
            WritePrivateProfileString("InspectParamsConfig", "MinRight1WhiteGlueMin", this.MinRight1WhiteGlueMin.ToString(), this._configFile);
            WritePrivateProfileString("InspectParamsConfig", "MaxRight1WhiteGlueMin", this.MaxRight1WhiteGlueMin.ToString(), this._configFile);
            WritePrivateProfileString("InspectParamsConfig", "MinRight2WhiteGlueMin", this.MinRight2WhiteGlueMin.ToString(), this._configFile);
            WritePrivateProfileString("InspectParamsConfig", "MaxRight2WhiteGlueMin", this.MaxRight2WhiteGlueMin.ToString(), this._configFile);
            WritePrivateProfileString("InspectParamsConfig", "IsNoUpLoadMdiAndPPGData", this.IsNoUpLoadMdiAndPPGData.ToString(), this._configFile);
            WritePrivateProfileString("InspectParamsConfig", "IsAlOnLeft", this.IsAlOnLeft.ToString(), this._configFile);

            WritePrivateProfileString("InspectParamsConfig", "MinResistance", this.MinResistance.ToString(), this._configFile);
            WritePrivateProfileString("InspectParamsConfig", "MaxResistance", this.MaxResistance.ToString(), this._configFile);
            WritePrivateProfileString("InspectParamsConfig", "MinVoltage", this.MinVoltage.ToString(), this._configFile);
            WritePrivateProfileString("InspectParamsConfig", "MaxVoltage", this.MaxVoltage.ToString(), this._configFile);
            WritePrivateProfileString("InspectParamsConfig", "MinOCV_k", this.MinOCV_k.ToString(), this._configFile);
            WritePrivateProfileString("InspectParamsConfig", "MaxOCV_k", this.MaxOCV_k.ToString(), this._configFile);
            WritePrivateProfileString("InspectParamsConfig", "MinTemperature", this.MinTemperature.ToString(), this._configFile);
            WritePrivateProfileString("InspectParamsConfig", "MaxTemperature", this.MaxTemperature.ToString(), this._configFile);
            WritePrivateProfileString("InspectParamsConfig", "TemperatureCoefficient", this.TemperatureCoefficient.ToString(), this._configFile);
            WritePrivateProfileString("InspectParamsConfig", "TemperatureCoefficient2", this.TemperatureCoefficient2.ToString(), this._configFile);
            WritePrivateProfileString("InspectParamsConfig", "VoltageCoefficient", this.VoltageCoefficient.ToString(), this._configFile);
            WritePrivateProfileString("InspectParamsConfig", "ResistanceCoefficient", this.ResistanceCoefficient.ToString(), this._configFile);
            WritePrivateProfileString("InspectParamsConfig", "ResistanceFixedValue", this.ResistanceFixedValue.ToString(), this._configFile);
            WritePrivateProfileString("InspectParamsConfig", "TemperatureFixedValue", this.TemperatureFixedValue.ToString(), this._configFile);

            WritePrivateProfileString("InspectParamsConfig", "ThicknessAvgDiff", this.ThicknessAvgDiff.ToString(), this._configFile);

            WritePrivateProfileString("InspectParamsConfig", "MinIV", this.MinIV.ToString(), this._configFile);
            WritePrivateProfileString("InspectParamsConfig", "MaxIV", this.MaxIV.ToString(), this._configFile);
            WritePrivateProfileString("InspectParamsConfig", "Source", this.Source.ToString(), this._configFile);
            WritePrivateProfileString("InspectParamsConfig", "Range", this.Range.ToString(), this._configFile);
            WritePrivateProfileString("InspectParamsConfig", "ExceptionData1", this.ExceptionData1.ToString(), this._configFile);
            WritePrivateProfileString("InspectParamsConfig", "ExceptionData2", this.ExceptionData2.ToString(), this._configFile);
            WritePrivateProfileString("InspectParamsConfig", "IvTestTime", this.IvTestTime.ToString(), this._configFile);
            WritePrivateProfileString("InspectParamsConfig", "IvStation1Channel", this.IvStation1Channel.ToString(), this._configFile);
            WritePrivateProfileString("InspectParamsConfig", "IvStation2Channel", this.IvStation2Channel.ToString(), this._configFile);
            WritePrivateProfileString("InspectParamsConfig", "IvStation3Channel", this.IvStation3Channel.ToString(), this._configFile);
            WritePrivateProfileString("InspectParamsConfig", "IvStation4Channel", this.IvStation4Channel.ToString(), this._configFile);

            WritePrivateProfileString("InspectParamsConfig", "Mi", this.Mi.ToString(), this._configFile);

            WritePrivateProfileString("InspectParamsConfig", "WeekCounts", this.WeekCounts.ToString(), this._configFile);

            WritePrivateProfileString("InspectParamsConfig", "Marking", this.Marking.ToString(), this._configFile);

            WritePrivateProfileString("InspectParamsConfig", "SetMarking", this.SetMarking.ToString(), this._configFile);

            WritePrivateProfileString("InspectParamsConfig", "IsCheckMI", this.IsCheckMI.ToString(), this._configFile);

            WritePrivateProfileString("InspectParamsConfig", "IsCheckWeekCount", this.IsCheckWeekCount.ToString(), this._configFile);

            WritePrivateProfileString("InspectParamsConfig", "IsCheckMarking", this.IsCheckMarking.ToString(), this._configFile);

            WritePrivateProfileString("InspectParamsConfig", "IsSetMarking", this.IsSetMarking.ToString(), this._configFile);

            WritePrivateProfileString("InspectParamsConfig", "IsNoResistance", this.IsNoResistance.ToString(), this._configFile);

            WritePrivateProfileString("InspectParamsConfig", "IsNoTemperature", this.IsNoTemperature.ToString(), this._configFile);

        }

        public void WriteThicknessParams()
        {
            WritePrivateProfileString("ThicknessParamsConfig", "currentModel", this.CurrentModel, this._configFile);
            WritePrivateProfileString("ThicknessParamsConfig", "max_thickness_s", this.MaxThicknessS.ToString(), this._configFile);
            WritePrivateProfileString("ThicknessParamsConfig", "min_thickness_s", this.MinThicknessS.ToString(), this._configFile);
            WritePrivateProfileString("ThicknessParamsConfig", "max_thickness_m", this.MaxThicknessM.ToString(), this._configFile);
            WritePrivateProfileString("ThicknessParamsConfig", "min_thickness_m", this.MinThicknessM.ToString(), this._configFile);
            WritePrivateProfileString("ThicknessParamsConfig", "max_thickness_b", this.MaxThicknessB.ToString(), this._configFile);
            WritePrivateProfileString("ThicknessParamsConfig", "min_thickness_b", this.MinThicknessB.ToString(), this._configFile);
            WritePrivateProfileString("ThicknessParamsConfig", "thicknessCalibrationModeA", this.ThicknessCalibrationModeA.ToString(), this._configFile);
            WritePrivateProfileString("ThicknessParamsConfig", "thicknessCalibrationModeB", this.ThicknessCalibrationModeB.ToString(), this._configFile);
            WritePrivateProfileString("ThicknessParamsConfig", "thicknessCalibrationModeC", this.ThicknessCalibrationModeC.ToString(), this._configFile);
            WritePrivateProfileString("ThicknessParamsConfig", "thicknessCalibrationModeD", this.ThicknessCalibrationModeD.ToString(), this._configFile);
            WritePrivateProfileString("ThicknessParamsConfig", "LinearRange", this.LinearRange.ToString(), this._configFile);
            WritePrivateProfileString("ThicknessParamsConfig", "LinearNeedOK", this.LinearNeedOK.ToString(), this._configFile);
            //WritePrivateProfileString("ThicknessParamsConfig", "LinearNGNum", this.LinearNGNum.ToString(), this._configFile);
            //WritePrivateProfileString("ThicknessParamsConfig", "LinearOKNum", this.LinearOKNum.ToString(), this._configFile);
        }

        public void WriteDimensionParams()
        {
            WritePrivateProfileString("DimensionParamsConfig", "bat_length_s", this.BatLengthS, this._configFile);
            WritePrivateProfileString("DimensionParamsConfig", "bat_width_s", this.BatWidthS, this._configFile);
            WritePrivateProfileString("DimensionParamsConfig", "left_lug_s", this.LeftLugS, this._configFile);
            WritePrivateProfileString("DimensionParamsConfig", "right_lug_s", this.RightLugS, this._configFile);
            WritePrivateProfileString("DimensionParamsConfig", "all_length_s", this.AllLengthS, this._configFile);
            WritePrivateProfileString("DimensionParamsConfig", "left_1_glue_s", this.Left1GlueS, this._configFile);
            WritePrivateProfileString("DimensionParamsConfig", "left_2_glue_s", this.Left2GlueS, this._configFile);
            WritePrivateProfileString("DimensionParamsConfig", "right_1_glue_s", this.Right1GlueS, this._configFile);
            WritePrivateProfileString("DimensionParamsConfig", "right_2_glue_s", this.Right2GlueS, this._configFile);
            WritePrivateProfileString("DimensionParamsConfig", "left_lug_len_s", this.LeftLugLenS, this._configFile);
            WritePrivateProfileString("DimensionParamsConfig", "right_lug_len_s", this.RightLugLenS, this._configFile);

            WritePrivateProfileString("DimensionParamsConfig", "bat_length_m", this.BatLengthM, this._configFile);
            WritePrivateProfileString("DimensionParamsConfig", "bat_width_m", this.BatWidthM, this._configFile);
            WritePrivateProfileString("DimensionParamsConfig", "left_lug_m", this.LeftLugM, this._configFile);
            WritePrivateProfileString("DimensionParamsConfig", "right_lug_m", this.RightLugM, this._configFile);
            WritePrivateProfileString("DimensionParamsConfig", "all_length_m", this.AllLengthM, this._configFile);
            WritePrivateProfileString("DimensionParamsConfig", "left_1_glue_m", this.Left1GlueM, this._configFile);
            WritePrivateProfileString("DimensionParamsConfig", "left_2_glue_m", this.Left2GlueM, this._configFile);
            WritePrivateProfileString("DimensionParamsConfig", "right_1_glue_m", this.Right1GlueM, this._configFile);
            WritePrivateProfileString("DimensionParamsConfig", "right_2_glue_m", this.Right2GlueM, this._configFile);
            WritePrivateProfileString("DimensionParamsConfig", "left_lug_len_m", this.LeftLugLenM, this._configFile);
            WritePrivateProfileString("DimensionParamsConfig", "right_lug_len_m", this.RightLugLenM, this._configFile);

            WritePrivateProfileString("DimensionParamsConfig", "bat_length_b", this.BatLengthB, this._configFile);
            WritePrivateProfileString("DimensionParamsConfig", "bat_width_b", this.BatWidthB, this._configFile);
            WritePrivateProfileString("DimensionParamsConfig", "left_lug_b", this.LeftLugB, this._configFile);
            WritePrivateProfileString("DimensionParamsConfig", "right_lug_b", this.RightLugB, this._configFile);
            WritePrivateProfileString("DimensionParamsConfig", "all_length_b", this.AllLengthB, this._configFile);
            WritePrivateProfileString("DimensionParamsConfig", "left_1_glue_b", this.Left1GlueB, this._configFile);
            WritePrivateProfileString("DimensionParamsConfig", "left_2_glue_b", this.Left2GlueB, this._configFile);
            WritePrivateProfileString("DimensionParamsConfig", "right_1_glue_b", this.Right1GlueB, this._configFile);
            WritePrivateProfileString("DimensionParamsConfig", "right_2_glue_b", this.Right2GlueB, this._configFile);
            WritePrivateProfileString("DimensionParamsConfig", "left_lug_len_b", this.LeftLugLenB, this._configFile);
            WritePrivateProfileString("DimensionParamsConfig", "right_lug_len_b", this.RightLugLenB, this._configFile);
        }
        #endregion

        #region - Image SaveConfig -

        private void ReadImageSaveConfig()
        {
            StringBuilder builder = new StringBuilder(1024);

            GetPrivateProfileString("ImageSaveConfig", "_saveOrigOKImage", @"true", builder, 1024, this._configFile);
            this._imageSaveConfig.SaveOrigOKImage = bool.Parse(builder.ToString());

            GetPrivateProfileString("ImageSaveConfig", "_saveOrigNGImage", @"true", builder, 1024, this._configFile);
            this._imageSaveConfig.SaveOrigNGImage = bool.Parse(builder.ToString());

            GetPrivateProfileString("ImageSaveConfig", "_saveTestOKImage", @"true", builder, 1024, this._configFile);
            this._imageSaveConfig.SaveTestOKImage = bool.Parse(builder.ToString());

            GetPrivateProfileString("ImageSaveConfig", "_saveTestNGImage", @"true", builder, 1024, this._configFile);
            this._imageSaveConfig.SaveTestNGImage = bool.Parse(builder.ToString());

            GetPrivateProfileString("ImageSaveConfig", "_imageFormat", @"BMP", builder, 1024, this._configFile);
            this._imageSaveConfig.ImageFormat = (EImageFormats)Enum.Parse(typeof(EImageFormats), builder.ToString());

            GetPrivateProfileString("ImageSaveConfig", "_diskThreshold", @"50", builder, 1024, this._configFile);
            this._imageSaveConfig.DiskThreshold = int.Parse(builder.ToString());

            GetPrivateProfileString("ImageSaveConfig", "_saveOrigPath", @"D:\\XRayPic\\Orig", builder, 1024, this._configFile);
            this._imageSaveConfig.SaveOrigPath = builder.ToString();

            GetPrivateProfileString("ImageSaveConfig", "_saveTestPath", @"D:\\XRayPic\\Test", builder, 1024, this._configFile);
            this._imageSaveConfig.SaveTestPath = builder.ToString();

            GetPrivateProfileString("ImageSaveConfig", "UseSTFPath", @"true", builder, 1024, this._configFile);
            this._imageSaveConfig.UseDynamicTestPath = bool.Parse(builder.ToString());

            GetPrivateProfileString("ImageSaveConfig", "SaveStartupTestImage", @"true", builder, 1024, this._configFile);
            this._imageSaveConfig.SaveStartupTestImage = bool.Parse(builder.ToString());

            GetPrivateProfileString("ImageSaveConfig", "StartupTestPath", @"D:\\XRayPic\\Startup", builder, 1024, this._configFile);
            this._imageSaveConfig.StartupTestPath = builder.ToString();
        }

        private void WriteImageSaveConfig()
        {
            WritePrivateProfileString("ImageSaveConfig", "_saveOrigOKImage", this._imageSaveConfig.SaveOrigOKImage.ToString(), this._configFile);
            WritePrivateProfileString("ImageSaveConfig", "_saveOrigNGImage", this._imageSaveConfig.SaveOrigNGImage.ToString(), this._configFile);
            WritePrivateProfileString("ImageSaveConfig", "_saveTestOKImage", this._imageSaveConfig.SaveTestOKImage.ToString(), this._configFile);
            WritePrivateProfileString("ImageSaveConfig", "_saveTestNGImage", this._imageSaveConfig.SaveTestNGImage.ToString(), this._configFile);
            WritePrivateProfileString("ImageSaveConfig", "_imageFormat", this._imageSaveConfig.ImageFormat.ToString(), this._configFile);
            WritePrivateProfileString("ImageSaveConfig", "_diskThreshold", this._imageSaveConfig.DiskThreshold.ToString(), this._configFile);
            WritePrivateProfileString("ImageSaveConfig", "_saveOrigPath", this._imageSaveConfig.SaveOrigPath.ToString(), this._configFile);
            WritePrivateProfileString("ImageSaveConfig", "_saveTestPath", this._imageSaveConfig.SaveTestPath.ToString(), this._configFile);
            WritePrivateProfileString("ImageSaveConfig", "UseSTFPath", this._imageSaveConfig.UseDynamicTestPath.ToString(), this._configFile);
            WritePrivateProfileString("ImageSaveConfig", "SaveStartupTestImage", this._imageSaveConfig.SaveStartupTestImage.ToString(), this._configFile);
            WritePrivateProfileString("ImageSaveConfig", "StartupTestPath", this._imageSaveConfig.StartupTestPath, this._configFile);
        }

        #endregion

        #region - Startup Config -
        private void ReadStartupConfig()
        {
            StringBuilder builder = new StringBuilder(1024);

            GetPrivateProfileString("StartupTestConfig", "lastTestTime", DateTime.MinValue.ToString("yyyy-MM-dd HH:mm:ss"), builder, 1024, this._configFile);
            this.MyStartupTestConfig.lastTestTime = DateTime.Parse(builder.ToString());

            GetPrivateProfileString("StartupTestConfig", "lastTestBy", @"nobody", builder, 1024, this._configFile);
            this.MyStartupTestConfig.lastTestBy = builder.ToString();

            GetPrivateProfileString("StartupTestConfig", "testInterval", @"12", builder, 1024, this._configFile);
            this.MyStartupTestConfig.testInterval = float.Parse(builder.ToString());

            GetPrivateProfileString("StartupTestConfig", "TestNGNum", @"3", builder, 1024, this._configFile);
            this.MyStartupTestConfig.TestNGNum = int.Parse(builder.ToString());

            GetPrivateProfileString("StartupTestConfig", "LastCheckOutTime", DateTime.MinValue.ToString("yyyyMMdd"), builder, 1024, this._configFile);
            this.MyStartupTestConfig.LastCheckOutTime = DateTime.ParseExact(builder.ToString(), "yyyyMMdd", System.Globalization.CultureInfo.CurrentCulture);

            GetPrivateProfileString("StartupTestConfig", "LastCheckOutHour", @"7", builder, 1024, this._configFile);
            this.MyStartupTestConfig.LastCheckOutHour = int.Parse(builder.ToString());

            GetPrivateProfileString("StartupTestConfig", "LastPinCheckTime", DateTime.MinValue.ToString("yyyyMMdd"), builder, 1024, this._configFile);
            this.MyStartupTestConfig.LastPinCheckTime = DateTime.ParseExact(builder.ToString(), "yyyyMMdd", System.Globalization.CultureInfo.CurrentCulture);

            GetPrivateProfileString("StartupTestConfig", "LastPinCheckHour", @"7", builder, 1024, this._configFile);
            this.MyStartupTestConfig.LastPinCheckHour = int.Parse(builder.ToString());
        }

        public void SaveStartupConfig()
        {
            WritePrivateProfileString("StartupTestConfig", "lastTestTime", this.MyStartupTestConfig.lastTestTime.ToString("yyyy-MM-dd HH:mm:ss"), this._configFile);
            WritePrivateProfileString("StartupTestConfig", "lastTestBy", this.MyStartupTestConfig.lastTestBy, this._configFile);
            WritePrivateProfileString("StartupTestConfig", "testInterval", this.MyStartupTestConfig.testInterval.ToString(), this._configFile);
            WritePrivateProfileString("StartupTestConfig", "TestNGNum", this.MyStartupTestConfig.TestNGNum.ToString(), this._configFile);
            WritePrivateProfileString("StartupTestConfig", "LastCheckOutTime", this.MyStartupTestConfig.LastCheckOutTime.ToString("yyyyMMdd"), this._configFile);
            WritePrivateProfileString("StartupTestConfig", "LastCheckOutHour", this.MyStartupTestConfig.LastCheckOutHour.ToString(), this._configFile);
            WritePrivateProfileString("StartupTestConfig", "LastPinCheckTime", this.MyStartupTestConfig.LastPinCheckTime.ToString("yyyyMMdd"), this._configFile);
            WritePrivateProfileString("StartupTestConfig", "LastPinCheckHour", this.MyStartupTestConfig.LastPinCheckHour.ToString(), this._configFile);
        }
        #endregion

    }
}
