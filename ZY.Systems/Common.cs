using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using XRayClient.Core;
using XRayClient.VisionSysWrapper;
using ZY.Model;

namespace ZY.Systems
{
    public partial class Common
    {
        [AttributeUsage(AttributeTargets.Field)]
        public class EnumDisplayAttribute : Attribute
        {
            public EnumDisplayAttribute(string displayStr, params object[] args)
            {
                Display = displayStr;
            }
            public string Display
            {
                get;
                private set;
            }
        }
        public static string DBConType = "mysql";
        public static string LogUploadMqtt = GetAssemblyPath + @"Log\UploadMqtt\Bug\" + DateTime.Now.Year + @"\" + DateTime.Now.Month + @"\";
        public static string LogUpload = GetAssemblyPath + @"Log\Upload\Bug\" + DateTime.Now.Year + @"\" + DateTime.Now.Month + @"\";
        public static string LogUploadError = GetAssemblyPath + @"Log\Upload\Error\" + DateTime.Now.Year + @"\" + DateTime.Now.Month + @"\";
        public static string LogError = GetAssemblyPath + @"Log\Error\" + DateTime.Now.Year + @"\" + DateTime.Now.Month + @"\";
        public static string LogBug = GetAssemblyPath + @"Log\Bug\" + DateTime.Now.Year + @"\" + DateTime.Now.Month + @"\";
        public static string LogAgv = GetAssemblyPath + @"Log\Agv\" + DateTime.Now.Year + @"\" + DateTime.Now.Month + @"\";
        #region 产品型号
        /// <summary>
        /// 电解液过期时间
        /// </summary>
        public static int FixtureCount { get; set; }

        /// <summary>
        /// 后称先扫码后称重（勾为先扫码）
        /// </summary>
        public static bool IsFristScanBarCodeAfter { get; set; }
        /// <summary>
        /// 采集数据状态
        /// </summary>
        public static bool IsCollection { get; set; }


        /// <summary>
        /// PLC计数必须大于0
        /// </summary>
        public static bool IsPlcCountZero { get; set; }
        public static string ProductNo { set; get; }
        /// <summary>
        /// 前称最小重量
        /// </summary>
        public static double BeforeMinWeight { set; get; } = -1;
        /// <summary>
        /// 前称最大重量
        /// </summary>
        public static double BeforMaxWeight { get; set; } = -1;
        /// <summary>
        /// 注液(抽液)最小范围
        /// </summary>
        public static double Inject_LSL { get; set; } = -1;
        /// <summary>
        /// 注液(抽液)最大范围
        /// </summary>
        public static double Inject_USL { get; set; } = -1;
        /// <summary>
        /// 是否实时显示温度曲线
        /// </summary>
        public static bool IsRealTimeChart { get; set; }
        /// <summary>
        /// 二次注液量下限
        /// </summary>
        public static double InjSecond_LSL { get; set; } = -1;
        /// <summary>
        /// 二次注液量上限
        /// </summary>
        public static double InjSecond_USL { get; set; } = -1;
        /// <summary>
        /// 条码总长度
        /// </summary>
        public static int BarCodeLen { get; set; }
        /// <summary>
        /// 条码2总长度
        /// </summary>
        public static int BarCodeLen2 { get; set; }
        /// <summary>
        /// 条码3总长度
        /// </summary>
        public static int BarCodeLen3 { get; set; }
        /// <summary>
        /// 条码前缀起始位数
        /// </summary>
        public static int PrefixStartLen { get; set; } = 0;
        /// <summary>
        /// 条码前缀位数
        /// </summary>
        public static int PrefixLen { get; set; }
        /// <summary>
        /// 条码前缀位数2
        /// </summary>
        public static int PrefixLen2 { get; set; }
        /// <summary>
        /// 条码前缀位数3
        /// </summary>
        public static int PrefixLen3 { get; set; }
        /// <summary>
        /// 条码前缀
        /// </summary>
        public static string PrefixData { get; set; }
        /// <summary>
        /// 条码前缀2
        /// </summary>
        public static string PrefixData2 { get; set; }
        /// <summary>
        /// 条码前缀3
        /// </summary>
        public static string PrefixData3 { get; set; }

        /// <summary>
        /// 测厚下限
        /// </summary>
        public static double Thickness_LSL { get; set; }
        /// <summary>
        /// 测厚上限
        /// </summary>
        public static double Thickness_USL { get; set; }

        /// <summary>
        /// 测厚2下限
        /// </summary>
        public static double Thickness2_LSL { get; set; }
        /// <summary>
        /// 测厚2上限
        /// </summary>
        public static double Thickness2_USL { get; set; }

        /// <summary>
        /// 测厚3下限
        /// </summary>
        public static double Thickness3_LSL { get; set; }
        /// <summary>
        /// 测厚3上限
        /// </summary>
        public static double Thickness3_USL { get; set; }

        /// <summary>
        /// 测厚4下限
        /// </summary>
        public static double Thickness4_LSL { get; set; }
        /// <summary>
        /// 测厚4上限
        /// </summary>
        public static double Thickness4_USL { get; set; }

        /// <summary>
        /// 测厚4下限
        /// </summary>
        public static double Thickness5_LSL { get; set; }
        /// <summary>
        /// 测厚4上限
        /// </summary>
        public static double Thickness5_USL { get; set; }

        /// <summary>
        /// 后称上限
        /// </summary>
        public static double AfterWeight_USL { get; set; }
        /// <summary>
        /// 后称下限
        /// </summary>
        public static double AfterWeight_LSL { get; set; }


        /// <summary>
        /// 测厚压力下限
        /// </summary>
        public static double ThicknessPressure_LSL { get; set; }
        /// <summary>
        /// 测厚压力上限
        /// </summary>
        public static double ThicknessPressure_USL { get; set; }

        /// <summary>
        /// 当天流水号
        /// </summary>
        public static long SerialNumber { get; set; }
        ///// <summary>
        ///// 针头数
        ///// </summary>
        //public static int InjectionNeedleNo { get; set; }
        /// <summary>
        /// 针头总数
        /// </summary>
        public static int TotalInjectNeedleNum { get; set; }
        /// <summary>
        /// 杯体总数
        /// </summary>
        public static int TurntableNeedleNum { get; set; }
        /// <summary>
        /// 夹具总数
        /// </summary>
        public static int TotalInjectFixtureNum { get; set; }
        ///// <summary>
        ///// 夹具号
        ///// </summary>
        //public static int FixtureNo { get; set; }
        //public static int FixtureNo2 { get; set; }
        /// <summary>
        /// 表格序号 
        /// </summary>
        public static int No { get; set; }
        /// <summary>
        /// 前称当前行号
        /// </summary>
        public static int RowNoBefore { get; set; }
        /// <summary>
        /// 后称当前行号
        /// </summary>
        public static int RowNoAfter { get; set; }

        /// <summary>
        /// 前注液采集间隔时间
        /// </summary>
        public static int IntervalBefrore { get; set; }
        /// <summary>
        /// 后注液采集间隔时间 
        /// </summary>
        public static int IntervalAfter { get; set; }

        public static int IntervalBarcode { get; set; }

        public static bool AutoBarCode { get; set; }

        /// 如果未启用自动扫描条码时，将使用以下条码号+流水号
        /// </summary>
        public static string DefaultBarCode { get; set; }
        /// <summary>
        /// 0：正常,1:返封印,2:调机
        /// </summary>
        public static string ProductNoType { get; set; }
        #endregion


        #region
        /// <summary>
        /// 前称为NG是否保存数据,
        /// </summary>
        public static bool IsBeforeNGNotSave { get; set; }
        /// <summary>
        /// 线程间隔循环等待时间(ms)
        /// </summary>
        public static int ThreadClearInterval { get; set; }


        /// <summary>
        /// 线程夹具间隔循环等待时间(ms)
        /// </summary>
        public static int ThreadFixtureInterval { get; set; }


        /// <summary>
        /// 线程间隔循环等待时间(ms)
        /// </summary>
        public static int ThreadInterval { get; set; }
        /// <summary>
        /// 读取PLC /写PLC信号 90000信号间隔时间(ms)
        /// </summary>
        public static int PLCReaderSignalInterval { get; set; }



        /// <summary>
        /// 前称循环次数
        /// </summary>
        public static int BeforeCount
        {
            get; set;
        }
        /// <summary>
        /// 前称间隔时间
        /// </summary>
        public static int BeforeTime
        {
            get; set;
        }
        /// <summary>
        /// 条码循环次数
        /// </summary>
        public static int BarCodeCount
        {
            get; set;
        }
        /// <summary>
        /// 条码间隔时间
        /// </summary>
        public static int BarCodeTime
        {
            get; set;
        }
        /// <summary>
        /// 电池重量范围值
        /// </summary>
        public static double WeighWeight { get; set; }
        /// <summary>
        /// 后称循环次数
        /// </summary>
        public static int AfterCount
        {
            get; set;
        }
        /// <summary>
        /// 后称间隔时间
        /// </summary>
        public static int AfterTime
        {
            get; set;
        }
        /// <summary>
        /// 注液最小重量
        /// </summary>
        public static double Inject_Min_Weight { get; set; }
        /// <summary>
        /// 注液最大重量
        /// </summary>
        public static double Inject_Max_Weight { get; set; }
        /// <summary>
        /// 清零后检测范围最小值
        /// </summary>
        public static double Clear_Zero_MinScope { get; set; }
        /// <summary>
        /// 清零后检测范围最大值
        /// </summary>
        public static double Clear_Zero_MaxScope { get; set; }

        //#region  一出一
        ///// <summary>
        ///// 前称称重发送指令
        ///// </summary>
        //public static string BeforeSendData { get; set; }
        ///// <summary>
        ///// 前称称重清零发送指令
        ///// </summary>
        //public static string BeforeCloseSendData { get; set; }
        ///// <summary>
        ///// 后称称重发送指令
        ///// </summary>
        //public static string AfterSendData { get; set; }
        ///// <summary>
        ///// 后称称重清零发送指令
        ///// </summary>
        //public static string AfterCloseSendData { get; set; }
        ///// <summary>
        ///// 扫码枪发送指令
        ///// </summary>
        //public static string BarcodeSendData { get; set; }
        ///// <summary>
        ///// 扫码枪发送关闭指令
        ///// </summary>
        //public static string BarcodeSendCloseData { get; set; }

        ///// <summary>
        ///// 下料扫码枪发送指令
        ///// </summary>
        //public static string BarcodeUnloadingSendData { get; set; }
        ///// <summary>
        ///// 下料扫码枪发送关闭指令
        ///// </summary>
        //public static string BarcodeUnloadingSendCloseData { get; set; }



        ///// <summary>
        ///// 料盒弹夹检查发送指令
        ///// </summary>
        //public static string MaterialBoxCheckSendData { get; set; }
        ///// <summary>
        ///// 料盒弹夹发送指令
        ///// </summary>
        //public static string MaterialBoxSendData { get; set; }
        ///// <summary>
        ///// 电解液发送指令
        ///// </summary>
        //public static string ElectrolyteSendData { get; set; }

        ///// <summary>
        ///// 泵的发送指令
        ///// </summary>
        //public static string PumpSendData { get; set; }
        ///// <summary>
        ///// 泵的接受指令
        ///// </summary>
        //public static string PumpReceiveSendData { get; set; }

        ///// <summary>
        ///// 泵的发送指令2
        ///// </summary>
        //public static string PumpSendData2 { get; set; }
        ///// <summary>
        ///// 泵的接受指令2
        ///// </summary>
        //public static string PumpReceiveSendData2 { get; set; }


        //#endregion

        //#region  一出二
        ///// <summary>
        ///// 前称称重发送指令
        ///// </summary>
        //public static string Before2SendData { get; set; }
        ///// <summary>
        ///// 前称称重清零发送指令
        ///// </summary>
        //public static string BeforeClose2SendData { get; set; }
        ///// <summary>
        ///// 后称称重发送指令
        ///// </summary>
        //public static string After2SendData { get; set; }
        ///// <summary>
        ///// 后称称重清零发送指令
        ///// </summary>
        //public static string AfterClose2SendData { get; set; }
        ///// <summary>
        ///// 扫码枪发送指令
        ///// </summary>
        //public static string Barcode2SendData { get; set; }
        ///// <summary>
        ///// 扫码枪发送关闭指令
        ///// </summary>
        //public static string Barcode2SendCloseData { get; set; }

        ///// <summary>
        ///// 下料扫码枪发送指令
        ///// </summary>
        //public static string BarcodeUnloading2SendData { get; set; }
        ///// <summary>
        ///// 下料扫码枪发送关闭指令
        ///// </summary>
        //public static string BarcodeUnloading2SendCloseData { get; set; }


        ///// <summary>
        ///// 料盒弹夹检查发送指令
        ///// </summary>
        //public static string MaterialBoxCheck2SendData { get; set; }
        ///// <summary>
        ///// 料盒弹夹发送指令
        ///// </summary>
        //public static string MaterialBox2SendData { get; set; }
        ///// <summary>
        ///// 电解液发送指令
        ///// </summary>
        //public static string Electrolyte2SendData { get; set; }

        ///// <summary>
        ///// 泵的发送指令
        ///// </summary>
        //public static string Pump2SendData { get; set; }
        ///// <summary>
        ///// 泵的接受指令
        ///// </summary>
        //public static string Pump2ReceiveSendData { get; set; }

        ///// <summary>
        ///// 泵的发送指令2
        ///// </summary>
        //public static string Pump2SendData2 { get; set; }
        ///// <summary>
        ///// 泵的接受指令2
        ///// </summary>
        //public static string Pump2ReceiveSendData2 { get; set; }


        //#endregion 一出二

        /// <summary>
        /// 条码周次截取前开始
        /// </summary>
        public static int WeekStartCut { get; set; }
        /// <summary>
        /// 条码周次截取前结束
        /// </summary>
        public static int WeekEndCut { get; set; }
        /// <summary>
        /// 生产表格显示总行数
        /// </summary>
        public static int GridRowCount { get; set; }

        public static int CheckUploadDataDay { get; set; }
        private static bool isLoadBarcodeScan = false;

        private static bool isUnLoadBarcodeScan = false;

        /// <summary>


        /// <summary>
        /// 后前称先扫码后称重（勾为先扫码）
        /// </summary>
        public static bool IsFirstScanBarCodeBefore { get; set; }
        /// <summary>
        /// 是否开始条码周次验证
        /// </summary>
        public static bool IsWeek { get; set; }
        /// <summary>
        /// 周次报警是否发送NG
        /// </summary>
        public static bool IsWeekNG { get; set; }
        /// <summary>
        /// 是否关闭端口
        /// </summary>
        public static bool IsCloseSerialPort { get; set; }
        /// <summary>
        /// 是否托盘扫码
        /// </summary>
        public static bool IsLoadingPalletBarCodeScan { get; set; }
        /// <summary>
        /// 是否料盒弹夹扫码2
        /// </summary>
        public static bool IsLoadingMaterialBoxScan2 { get; set; }

        /// <summary>
        /// 是否料盒弹夹扫码验证
        /// </summary>
        public static bool IsMaterialBoxScanCheck { get; set; }
        /// <summary>
        /// 是否料盒弹夹扫码2验证
        /// </summary>
        public static bool IsMaterialBoxScanCheck2 { get; set; }

        /// <summary>
        /// 是否电解液验证
        /// </summary>
        public static bool IsElectrolyteScanCheck { get; set; }
        /// <summary>
        /// 是否电解液2验证
        /// </summary>
        public static bool IsElectrolyteScanCheck2 { get; set; }

        /// <summary>
        /// 下料扫码是否按条码与计数匹配
        /// </summary>
        public static bool IsUnScanInBarCodeAndCount { get; set; }

        /// <summary>
        /// 注液NG
        /// </summary>
        public static Color InjectWeighColorNG { get; set; }
        /// <summary>
        /// 注液OK
        /// </summary>
        public static Color InjectWeighColorOK { get; set; }

        #endregion

        /// <summary>
        /// 写信号的MR90100二进制数组
        /// </summary>
        public static List<char> ComPortSignalWriter100Byte { get; set; }
        /// <summary>
        /// 写信号的MR90400二进制数组
        /// </summary>
        public static List<char> ComPortSignalWriter400Byte { get; set; }

        /// <summary>
        /// 读信号的90000二进制数组
        /// </summary>
        public static List<char> ComPortSignalReaderByte { get; set; }

        #region MR90400
        /// <summary>
        /// 扫描电池条码断电记忆MR90400
        /// </summary>
        public const int PLC_90400_SHUTEDOWN_BARCODE = 0;
        /// <summary>
        /// 前称断电记忆完成MR90401
        /// </summary>
        public const int PLC_90401_SHUTEDOWN_BEFORE = 1;
        /// <summary>
        /// 后称断电记忆完成MR90402
        /// </summary>
        public const int PLC_90402_SHUTEDOWN_AFTER = 2;
        /// <summary>
        /// 更换电解液完成MR90403
        /// </summary>
        public const int PLC_90403_CHANGE_ELECTROLYTE = 3;
        /// <summary>
        /// 前称通讯异常MR90404
        /// </summary>
        public const int PLC_90404_BEFORE_COM_EXCEPTION = 4;
        /// <summary>
        /// 软件就绪
        /// </summary>
        public const int PLC_90405_SOFT_START = 5;
        /// <summary>
        /// 料盒扫描好品MR90406
        /// </summary>
        public const int PLC_90406_MATERIALBOX_OK = 6;
        /// <summary>
        /// 料盒未backing信号MR90407
        /// </summary>
        public const int PLC_90407_MATERIALBOX_NG = 7;
        /// <summary>
        /// 1号针连续NG信号MR90410
        /// </summary>
        public const int PLC_90410_INJECION_NEEDLE_SERIES_NG_1 = 10;
        /// <summary>
        /// 2号针连续NG信号MR90411
        /// </summary>
        public const int PLC_90411_INJECION_NEEDLE_SERIES_NG_2 = 11;
        /// <summary>
        /// 3号针连续NG信号MR90412
        /// </summary>
        public const int PLC_90412_INJECION_NEEDLE_SERIES_NG_3 = 12;
        /// <summary>
        /// 4号针连续NG信号MR90413
        /// </summary>
        public const int PLC_90413_INJECION_NEEDLE_SERIES_NG_4 = 13;
        /// <summary>
        /// 重码报警信号MR90414
        /// </summary>
        public const int PLC_90414_BARCODE_REPEAT = 14;
        /// <summary>
        /// 后称通讯异常MR90415
        /// </summary>
        public const int PLC_90415_AFTER_COM_EXCEPTION = 15;

        public const int PLC_90408_NOT_8 = 8;
        public const int PLC_90409_NOT_9 = 9;

        #endregion

        #region MR90100地址+902000

        /// <summary>
        /// 前称OK信号 90100
        /// </summary>
        public const int PLC_90100_BEFORE_OK = 0;
        /// <summary>
        /// 前称NG信号 90101
        /// </summary>
        public const int PLC_90101_BEFORE_NG = 1;
        /// <summary>
        /// 前称清零信号
        /// </summary>
        public const int PLC_90102_BEFORE_FINSH_ZERO = 2;
        /// <summary>
        /// 后称OK信号
        /// </summary>
        public const int PLC_90103_AFTER_OK = 3;
        /// <summary>
        /// 后称NG信号
        /// </summary>
        public const int PLC_90104_AFTER_NG = 4;
        /// <summary>
        /// 后称清零信号
        /// </summary>
        public const int PLC_90105_AFTER_FINSH_ZERO = 5;
        /// <summary>
        /// 未吸开未注液
        /// </summary>
        public const int PLC_90106_NOT_OPEN = 6;
        /// <summary>
        /// 4注液针头NG
        /// </summary>
        public const int PLC_90107_INJECION_NEEDLE_NG_4 = 7;
        /// <summary>
        /// 条码OK
        /// </summary>
        public const int PLC_90108_BARCODE_OK = 8;
        /// <summary>
        /// 条码NG
        /// </summary>
        public const int PLC_90109_BARCODE_NG = 9;
        /// <summary>
        ///  条码周次报警信号
        /// </summary>
        public const int PLC_90110_BARCODE_ALARM = 10;
        /// <summary>
        /// 条码前几位不匹配
        /// </summary>
        public const int PLC_90111_BARCODE_MIS_MATHCH = 11;
        /// <summary>
        /// 前称连续多个电池信号NG
        /// </summary>
        public const int PLC_90112_BEFORE_SERIES_NG = 12;
        /// <summary>
        /// 电解液验证 OK=0/NG=1
        /// </summary>
        public const int PLC_90113_ELECTROLYTE_CHECK = 13;
        /// <summary>
        /// 1注液针头NG
        /// </summary>
        public const int PLC_90114_INJECION_NEEDLE_NG_1 = 14;
        /// <summary>
        /// 2注液针头NG
        /// </summary>
        public const int PLC_90115_INJECION_NEEDLE_NG_2 = 15;
        /// <summary>
        /// 3注液针头NG
        /// </summary>
        public const int PLC_90200_INJECION_NEEDLE_NG_3 = 16;
        #endregion

        #region PLC TO PC 信号 

        /// <summary>
        /// 前称信号 90000
        /// </summary>
        public const int PLC_90000_BEFORE = 0;
        /// <summary>
        /// 前称清零信号 90001
        /// </summary>
        public const int PLC_90001_BEFORE_ZERO = 1;

        /// <summary>
        /// 后称信号 90002
        /// </summary>
        public const int PLC_90002_AFTER = 2;
        /// <summary>
        /// 后称清零信号 90003
        /// </summary>
        public const int PLC_90003_AFTER_ZERO = 3;
        /// <summary>
        /// 料盒条码信号  90004
        /// </summary>
        public const int PLC_90004_MATERIALBOX = 4;
        /// <summary>
        /// 条码信号  90005
        /// </summary>
        public const int PLC_90005_BARCODE = 5;
        /// <summary>
        /// 电解液信号  90006
        /// </summary>
        public const int PLC_90006_EIECTROLYTE_CHECK = 6;
        /// <summary>
        /// 结束生产信号  90007
        /// </summary>
        public const int PLC_90007_FINISH = 7;
        /// <summary>
        /// 返封印信号 90011
        /// </summary>
        public const int PLC_90011_SEAL = 11;

        /// <summary>
        /// 判断未吸开（未注液）
        /// </summary>
        public const int PLC_90012_NOT_OPEN = 12;

        /// <summary>
        /// 夹具信号1 900201 （ 封头压力 PumpPressure  ,前封口温度 BeforeTemp 后封口温度AfterTemp） 
        /// </summary>
        public const int PLC_90201_FIXTURE1 = 17;
        /// <summary>
        /// 夹具信号2 900202（ 封头压力 PumpPressure  ,前封口温度 BeforeTemp 后封口温度AfterTemp） 
        /// </summary>
        public const int PLC_90202_FIXTURE2 = 18;
        /// <summary>
        /// 夹具信号3 900203（ 封头压力 PumpPressure  ,前封口温度 BeforeTemp 后封口温度AfterTemp） 
        /// </summary>
        public const int PLC_90203_FIXTURE3 = 19;
        /// <summary>
        /// 夹具信号4 900204 （ 封头压力 PumpPressure  ,前封口温度 BeforeTemp 后封口温度AfterTemp） 
        /// </summary>
        public const int PLC_90204_FIXTURE4 = 20;
        /// <summary>
        /// 称一
        /// </summary>
        public static int InjectCompensation1_1;

        /// <summary>
        /// 称二
        /// </summary>
        public static int InjectCompensation1_2;
        /// <summary>
        /// 下料托盘扫码
        /// </summary>
        public static bool IsUnLoadingPalletBarCodeScan { get; set; }
        /// <summary>
        /// 下料托盘扫码2
        /// </summary>
        public static bool IsUnLoadingMaterialBoxScan2 { get; set; }

        private static bool isBeforeWeighWeight = false;


        private static bool isAfterWeighWeight = false;

        private static bool isTabVerify = false;
        /// <summary>
        /// Tab重码校验
        /// </summary>
        public static bool IsTabVerify
        {
            get { return isTabVerify; }
            set
            {
                isTabVerify = value;
            }
        }
        private static bool isMesNgInfo = false;
        /// <summary>
        /// MES反馈NG详情
        /// </summary>

        public static bool IsMesNgInfo
        {
            get { return isMesNgInfo; }
            set
            {
                isMesNgInfo = value;
            }
        }

        private static bool isEditPLCArg = false;
        /// <summary>
        /// 是否编辑下料原因
        /// </summary>
        public static bool IsEditPLCArg
        {
            get { return isEditPLCArg; }
            set
            {
                isEditPLCArg = value;
            }
        }
        #endregion

        /*
    
            //PLC地址 
            /// <summary>
            /// 前称重完成信号
            /// </summary>
            public const int PLC_ADDRESS_BEFORE_FINSH = 0;
            /// <summary>
            /// 后称重完成信号
            /// </summary>
            public const int PLC_ADDRESS_AFTER_FINSH = 1;

            /// <summary>
            /// 前称清零信号
            /// </summary>
            public const int PLC_ADDRESS_BEFORE_FINSH_ZERO = 2;
            /// <summary>
            /// 后称清零信号
            /// </summary>
            public const int PLC_ADDRESS_AFTER_FINSH_ZERO = 3;

            /// <summary>
            /// 软件就绪
            /// </summary>
            public const int PLC_ADDRESS_SOFT_START = 4;
            /// <summary>
            /// 后称NG信号
            /// </summary>
            public const int PLC_ADDRESS_AFTER_NG = 5;
            /// <summary>
            /// 后称OK信号
            /// </summary>
            public const int PLC_ADDRESS_AFTER_OK = 6;

            /// <summary>
            /// 条码OK信号
            /// </summary>
            public const int PLC_ADDRESS_BARCODE_OK = 7;
            /// <summary>
            /// 条码NG信号
            /// </summary>
            public const int PLC_ADDRESS_BARCODE_NG = 8;

            /// <summary>
            /// i没有注液信号
            /// </summary>
            public const int PLC_ADDRESS_INJECTED_NOT_SIGNAL = 9;

            /// <summary>
            /// 允许取电池信号
            /// </summary>
            public const int PLC_ADDRESS_ALLOW_BARCODE_SIGNAL = 10;
            /// <summary>
            /// PC给PLC扫描信号
            /// </summary>
            public const int PLC_ADDRESS_PC_TO_PLC = 11;

            /// <summary>
            /// 前称NG信号
            /// </summary>
            public const int PLC_ADDRESS_BEFORE_NG = 12;

            /// <summary>
            /// 前称OK信号
            /// </summary>
            public const int PLC_ADDRESS_BEFORE_OK = 13; 
         
            public const int PLC_ADDRESS_14 = 14;

            public const int PLC_ADDRESS_15 = 15;
            /// <summary>
            /// 前称信号
            /// </summary>
            public const int PLC_ADDRESS_SIGNAL_BEFORE = 0;
            /// <summary>
            /// 后称信号
            /// </summary>
            public const int PLC_ADDRESS_SIGNAL_AFTER = 1;

            /// <summary>
            /// 前称清零信号
            /// </summary>
            public const int PLC_ADDRESS_SIGNAL_BEFORE_ZERO = 2;
            /// <summary>
            /// 后称清零信号
            /// </summary>
            public const int PLC_ADDRESS_SIGNAL_AFTER_ZERO = 3;
            /// <summary>
            /// 条码信号 
            /// </summary>
            public const int PLC_ADDRESS_SIGNAL_BARCODE = 4;

            /// <summary>
            /// 条码信号备份 允许扫描条码
            /// </summary>
            public const int PLC_ADDRESS_SIGNAL_BARCODE_SCAN = 5;
         */

        /// <summary>
        /// 是否启用注液泵补尝值
        /// </summary>
        public static bool IsInjectCompensation { get; set; }
        /// <summary>
        /// 是否启用注液泵2补尝值
        /// </summary>
        public static bool IsInjectCompensation2 { get; set; }

        /// <summary>
        /// 注液量补偿值收集-个数
        /// </summary>
        public static int InjectCompensationCount { get; set; }
        /// <summary>
        /// 注液量补偿值常量
        /// </summary>
        public static int NormalInjectCompensation { get; set; }
        /// <summary>
        /// 前称称量后电池重量补偿值
        /// </summary>
        public static double BeforeWeighCompensate { get; set; }
        /// <summary>
        /// 是否允许前称称量后电池重量补偿值
        /// </summary>
        public static bool IsBeforeWeighCompensate { get; set; }
        /// <summary>
        /// 设备类型(一出几)称个数
        /// </summary>
        public static int WeighCount { get; set; }
        /// <summary>
        /// 是否截取条码
        /// </summary>
        public static bool IsBarCodeCut { get; set; }
        /// <summary>
        /// 条码第几位开始
        /// </summary>
        public static int BarCodeStartCut { get; set; }

        /// <summary>
        /// 条码截取位数
        /// </summary>
        public static int BarCodeEndCut { get; set; }
        public static int RowNoAfter2 { get; set; }
        //public static int InjectionNeedleNo2 { get; set; }
        /// <summary>
        /// 针头显示方式
        /// </summary>
        public static int NeedleDisplay { get; set; }
        /// <summary>
        /// 注液参数是否实时获取(压力,温度
        /// </summary>
        public static bool IsActualInjectArg { get; set; }
        /// <summary>
        /// 泵1R针头
        /// </summary>
        public static int NeedleRPump { get; set; }
        /// <summary>
        /// 泵1L针头
        /// </summary>
        public static int NeedleLPump { get; set; }

        /// <summary>
        /// 泵2R针头
        /// </summary>
        public static int NeedleRPump2 { get; set; }
        /// <summary>
        /// 泵2L针头
        /// </summary>
        public static int NeedleLPump2 { get; set; }
        /// <summary>
        /// 是否启用上料托盘1
        /// </summary>
        public static bool IsUsePalletBarCode_Loading { get; set; }
        /// <summary>
        /// 是否启用上料托盘2
        /// </summary>
        public static bool IsUsePalletBarCode_Loading2 { get; set; }

        public static bool IsUseSendCheck { get; set; }

        /// <summary>
        /// 是否只接收料盒数据
        /// </summary>
        public static bool IsReceiveMaterialBox { get; set; }
        /// <summary>
        /// 是否启用将条码编号写入到PLC数据
        /// </summary>
        public static bool IsCheckBarcodeWriter { get; set; }
        /// <summary>
        /// 是否启用将前称编号写入到PLC数据
        /// </summary>
        public static bool IsCheckBeforeWriter { get; set; }
        /// <summary>
        /// 是否启用将后称编号写入到PLC数据
        /// </summary>
        public static bool IsCheckAfterWriter { get; set; }
        /// <summary>
        /// 是否启用将软件编号写入到PLC数据,用于下料时查找序号
        /// </summary>
        public static bool IsCheckPLCWriter { get; set; }
        /// <summary>
        /// 泵1的L针头
        /// </summary>
        public static string InjectCompensationLNeedle { get; set; }
        /// <summary>
        /// 泵1的R针头
        /// </summary>
        public static string InjectCompensationRNeedle { get; set; }
        /// <summary>
        /// 泵2的L针头
        /// </summary>
        public static string InjectCompensationLNeedle2 { get; set; }
        /// <summary>
        /// 泵2的R针头
        /// </summary>
        public static string InjectCompensationRNeedle2 { get; set; }
        /// <summary>
        /// 工厂区域
        /// </summary>
        public static string FactoryNo { get; set; }

        /// <summary>
        /// 上料是否共享扫码信号
        /// </summary>
        public static bool IsLoadingShareScan { get; set; }
        /// <summary>
        /// 下料是否共享扫码信号
        /// </summary>
        public static bool IsUnLoadingShareScan { get; set; }
        /// <summary>
        /// 是否允许PLC数据与软件数据错位
        /// </summary>
        public static bool IsAllowDataMismatch { get; set; }
        /// <summary>
        /// 扫码后扫描枪是否关闭连接 （包含所有通讯对象都进行关闭）
        /// </summary>
        public static bool IsCommunicationClose { get; set; }
        /// <summary>
        /// 称重后（称）是否关闭连接(作废)
        /// </summary>
        public static bool IsWeightClose { get; set; }
        /// <summary>
        /// 允许发送NG信号否
        /// </summary>
        public static bool IsSendNG { get; set; }
        /// <summary>
        /// 系统出错是否允许发送NG
        /// </summary>
        public static bool IsErrorSendNG { get; set; }

        /// <summary>
        /// 是否监控报警信息
        /// </summary>
        public static bool IsAlarm { get; set; }

        /// <summary>
        /// 上料扫码是否共享同一个信号
        /// </summary>
        public static bool IsLoadingScanShareSignal { get; set; }
        /// <summary>
        /// 下料扫码是否共享同一个信号
        /// </summary>
        public static bool IsUnLoadingScanShareSignal { get; set; }
        /// <summary>
        /// 前称是否共享同一个信号
        /// </summary>
        public static bool IsBeforeWieghShareSignal { get; set; }
        /// <summary>
        /// 后称是否共享同一个信号
        /// </summary>
        public static bool IsAfterWieghShareSignal { get; set; }
        /// <summary>
        /// 是否允许发送注液量给注液泵
        /// </summary>
        public static bool IsSendPumpInjectWeigh { get; set; }

        /// <summary>
        /// 是否允许对称进行点点检验
        /// </summary>
        public static bool IsConPointCheck { get; set; }
        /// <summary>
        /// 自动检点
        /// </summary>
        public static bool IsAutoConPointCheck { get; set; }
        /// <summary>
        /// 上次点点检验时间
        /// </summary>
        public static DateTime ShiftPointCheckTime { get; set; }
        /// <summary>
        /// 点检提醒时间间隔（分钟）
        /// </summary>
        public static int StratPointTime { get; set; }
        /// <summary>
        /// 点点检验是否成功
        /// </summary>
        public static bool IsShiftPointCheck { get; set; }
        /// <summary>
        /// 点检提醒次数
        /// </summary>
        public static int PointNum { get; set; }
        /// <summary>
        /// 点检超时是否停机
        /// </summary>
        public static bool IsPointTimeOutStop { get; set; }

        public static string ShiftBegin { get; set; }

        public static DateTime ShiftDate { get; set; }

        /// <summary>
        /// 点点检验砝码重量
        /// </summary>
        public static double WeightsNum { get; set; }

        /// <summary>
        /// 点点检验重量误差最小值
        /// </summary>
        public static double WeightsMin { get; set; }

        /// <summary>
        /// 点点检验重量误差最大值
        /// </summary>
        public static double WeightsMax { get; set; }
        /// <summary>
        /// 是否启用第二个PLC
        /// </summary>
        public static bool IsPLC2Connect { get; set; }

        /// <summary>
        /// 是否启用检验上料数据库条码ID
        /// </summary>
        public static bool IsCheckBarCode { get; set; }
        /// <summary>
        /// 是否启用检验上料PLC计数
        /// </summary>
        public static bool IsCheckPLCCount { get; set; }
        /// <summary>
        /// 是否前称扫码共用信号
        /// </summary>
        public static bool IsBeforeWieghScanShareSignal { get; set; }
        /// <summary>
        /// 是否后称扫码共用信号
        /// </summary>
        public static bool IsAfterWieghScanShareSignal { get; set; }
        /// <summary>
        /// 是否按天进行开机检验设备 0,不检验，1按天，2,实时
        /// </summary>
        public static int RuningCheckByDay { get; set; }
        /// <summary>
        /// 是否动态创建列头
        /// </summary>
        public static bool IsDynamicColumn { get; set; }

        /// <summary>
        /// 是否生成典线图
        /// </summary>
        public static bool IsChart { get; set; }

        /// <summary>
        /// 是否需要选择停机报警原因
        /// </summary>
        public static bool IsSelectStopReason { get; set; }

        /// <summary>
        /// 是否启用及时报警同步
        /// </summary>
        public static bool IsSameTimeAlarmHandle { get; set; }

        /// <summary>
        /// 是否启用信号同步发送()
        /// </summary>
       // public static bool IsSyncSignal { get; set; }

        /// <summary>
        /// 称的小数位数
        /// </summary>
        public static string ConDataformat { get; set; }
        /// <summary>
        /// 后称重量验证(最小值)
        /// </summary>
        public static double AfterWeighReadMin { get; set; }

        /// <summary>
        /// 后称重量验证(最大值)
        /// </summary>
        public static double AfterWeighReadMax { get; set; }

        /// <summary>
        /// 是否实时上传设备状态
        /// </summary>
        public static bool chkIsDeviceUpload { get; set; }

        /// <summary>
        /// 是否设备数据采集时上传设备状态
        /// </summary>
        public static bool chkIsCollectDeviceUpload { get; set; }

        /// <summary>
        /// PLC结束生产退出软件
        /// </summary>
        public static bool IsEndSoftExit { get; set; }

        public static bool IsImportCSV { get; set; }
        /// <summary>
        /// 接口调用失败当NG处理
        /// </summary>
        public static bool IsUploadErrorByNG { get; set; }
        /// <summary>
        /// 上传状态
        /// </summary>
        public static bool IsDataUpload { get; set; }

        /// <summary>
        /// 上料扫码是否启用条码重复校验
        /// </summary>
        public static bool isScanRepetitionVerify { get; set; }

        /// <summary>
        /// 条码重复验证个数
        /// </summary>
        public static int ScanRepetitionVerifyNum { get; set; }

        /// <summary>
        /// 是否共享读PLC计数连接
        /// </summary>
        public static bool IsShareConnectReadPlcNo { get; set; }

        /// <summary>
        /// 需要上传数据接口
        /// </summary>
        public static List<Enums.EnumberEntity> lstUploadInterface { get; set; }
        public static List<Enums.EnumberEntity> lstUploadInterfaceTip { get; set; }

        /// <summary>
        /// NG时，是否上传中控
        /// </summary>
        public static bool IsNGUpload { get; set; }

        /// <summary>
        /// 开机是否先清零
        /// </summary>
        public static bool IsRuningZero { get; set; }
        /// <summary>
        /// 是否检查未上传数据
        /// </summary>
        public static bool IsCheckUploadData { get; set; }

        /// <summary>
        /// 早班开始时间
        /// </summary>
        public static string MStartTime { get; set; }

        /// <summary>
        /// 原图路径
        /// </summary>
        public static string OrigImageFile { get; set; }
        /// <summary>
        /// 结果图路径
        /// </summary>
        public static string ResultImageFile { get; set; }

        /// <summary>
        /// 删除图片小时
        /// </summary>
        public static int DelImageHour { get; set; }

        /// <summary>
        /// 删除图片分钟
        /// </summary>
        public static int DelImageMinute { get; set; }

        /// <summary>
        /// 结果图保存天数
        /// </summary>
        public static int ResultSaveDay { get; set; }

        /// <summary>
        /// 分拣启用后扫码
        /// </summary>
        public static bool SortingScan { get; set; }
        /// <summary>
        /// 条码前缀校验
        /// </summary>
        public static bool IsCheckBarcodeLenth { get; set; }
        /// <summary>
        /// 二次码上传
        /// </summary>
        public static bool IsCheckTwoBarcode { get; set; }

        /// <summary>
        /// MES参数下发
        /// </summary>
        public static bool IsProductType { get; set; }

        
        /// <summary>
        /// FQI MI路径
        /// </summary>
        public static string FQI_MI_Path { get; set; }
        /// <summary>
        /// FQI 标快差值
        /// </summary>
        public static double FQIRange { get; set; }
        
        /// <summary>
        /// XRAY MI路径
        /// </summary>
        public static string XRAY_MI_Path { get; set; }
        /// <summary>
        /// 原图保存天数
        /// </summary>
        public static int OrigSaveDay { get; set; }

        /// <summary>
        /// 仪器短连接
        /// </summary>
        public static bool IsShortSocket { get; set; }
        
        /// <summary>
        /// 早班结束时间
        /// </summary>
        public static string MEndTime { get; set; }
        /// <summary>
        /// 晚班开始时间
        /// </summary>
        public static string EStartTime { get; set; }
        /// <summary>
        /// 晚班结束时间
        /// </summary>
        public static string EEndTime { get; set; }

        /// <summary>
        /// 换班后一分钟内退出软件
        /// </summary>
        public static bool IsChangeShiftsExit { get; set; }

        /// <summary>
        /// 送检提示时间
        /// </summary>
        public static string InspectWarnTime { get; set; }

        /// <summary>
        /// 是否允许送检提示
        /// </summary>
        public static bool isInspectWarn { get; set; }

        /// <summary>
        /// Excel保存间隔分钟
        /// </summary>
        public static int SaveExcelTimes { get; set; }
        /// <summary>
        /// 是否生成EXCEL文件
        /// </summary>
        public static bool IsSaveExcelTimes { get; set; }
        /// <summary>
        /// 出错是否停机
        /// </summary>
        public static bool IsErrorStop { get; set; }

        /// <summary>
        /// 上传数据给多个服务器
        /// </summary>
        public static bool IsUploadMultiple { get; set; }
        /// <summary>
        /// 显示实时采集数据否
        /// </summary>

        public static bool IsRealTimeCollectData { get; set; }

        /// <summary>
        /// 保存EXCEL数据类型,0,所有，1 经过后称数据,2,后称OK数据
        /// </summary>
        public static int ExportDataType { get; set; }

        /// <summary>
        /// 结束时导出所有EXECL数据
        /// </summary>
        public static bool EndExportAllData { get; set; }
        /// <summary>
        /// 是否启用PPM
        /// </summary>
        public static bool IsPpm { get; set; }
        /// <summary>
        /// 是否启用CPK
        /// </summary>
        public static bool IsCPK { get; set; }
        /// <summary>
        /// 是否启用GRR
        /// </summary>
        public static bool IsGRR { get; set; }
        /// <summary>
        /// 是否控制GRR-NG
        /// </summary>
        public static bool IsGRRNG { get; set; }
        /// <summary>
        ///CPK下限
        /// </summary>
        public static double CPKAlarmMin { get; set; }
        /// <summary>
        /// CPK上限
        /// </summary>
        public static double CPKAlarmMax { get; set; }
        /// <summary>
        /// 间隔CPK个数
        /// </summary>
        public static int CPKCount { get; set; }
        /// <summary>
        /// 员工号长度
        /// </summary>
        public static int LenStaffID { get; set; }
        /// <summary>
        /// 电解液型号长度
        /// </summary>
        public static int LenElectrolyte { get; set; }
        /// <summary>
        /// 是否启用条码长度判断
        /// </summary>
        public static bool IsBarCodeLen { get; set; }
        /// <summary>
        /// 是否加载缓存
        /// </summary>
        public static bool IsLoadCache { get; set; }
        /// <summary>
        /// 加载缓存数据天数
        /// </summary>
        public static int LoadCacheDay { get; set; }
        /// <summary>
        /// 是否只启用后段
        /// </summary>
        public static bool IsAfter { get; set; }
        /// <summary>
        ///注液差值计算
        /// </summary>
        public static bool InjectionDifference { get; set; }
        /// <summary>
        /// 前段数据写入后段否
        /// </summary>
        public static bool IsBefore { get; set; }

        /// <summary>
        /// 前称NG时，数据是否保存至下料表中
        /// </summary>
        public static bool IsBeforeNGSaveData { get; set; }

        /// <summary>
        /// 下料时是否单独获取上料数据
        /// </summary>
        public static bool IsGetLoadData { get; set; }


        /// <summary>
        /// 结束生产是否清除缓存
        /// </summary>
        public static bool IsStopClearData { get; set; }
        /// <summary>
        /// 倍福PLC数组变量下标起始位
        /// </summary>
        public static int TWINCATPLC_Index { get; set; }
        /// <summary>
        /// 倍福PLC2数组变量下标起始位
        /// </summary>
        public static int TWINCATPLC2_Index { get; set; }

        /// <summary>
        /// 是否为Degas后段
        /// </summary>
        public static bool IsDegasAfter { get; set; }
        /// <summary>
        /// 上料扫码时,数据处理完成
        /// </summary>
        public static bool IsScanCodeUnLoading { get; set; }
        /// <summary>
        /// 下料扫码时,数据处理完成
        /// </summary>
        public static bool IsUnScanCodeUnLoading { get; set; }
        /// <summary>
        /// PLC参数通讯时数据保存至下料否
        /// </summary>
        public static bool IsPLCArgSaveUnLoading { get; set; }

        /// <summary>
        /// 是否使用单信号传送PLC
        /// </summary>
        public static bool IsSignalTwincat { get; set; }

        /// <summary>
        /// 是否单独生成数据模型
        /// </summary>
        public static bool IsCreateModel { get; set; }

        /// <summary>
        /// 上传报警次数
        /// </summary>
        public static int uploadAlarmNum { get; set; }
        /// <summary>
        /// 连续NG个数时报警
        /// </summary>
        public static int ContNgAlarmNum { get; set; }
        /// <summary>
        /// 注液量上限设置
        /// </summary>
        public static int seInjection_Upper { get; set; }
        /// <summary>
        /// excel文件保存位置
        /// </summary>
        public static string saveExcelPath { get; set; }

        /// <summary>
        /// 是否空运行
        /// </summary>
        public static bool IsEmptyRuning { get; set; }

        /// <summary>
        /// 保存TXT日志文件路径
        /// </summary>
        public static string LogFileName { get; set; }

        /// <summary>
        /// PPM 显示下限
        /// </summary>
        public static int PpmScaleRangeMin { get; set; }
        /// <summary>
        /// PPM 显示间隔范围
        /// </summary>
        public static int PpmScaleRangeSpan
        {
            get; set;
        }
        /// <summary>
        /// 显示日志行数
        /// </summary>
        public static int LogCount { get; set; }

        /// <summary>
        /// 是否启用SCI
        /// </summary>
        public static bool IsUseSci { get; set; }
        /// <summary>
        /// SCI文件路径
        /// </summary>
        public static string SciPathFile { get; set; }

        /// <summary>
        /// 是否启用首件扫码
        /// </summary>
        public static bool IsUseFirstPart { get; set; }

        /// <summary>
        /// 测厚NG数据判断
        /// </summary>
        public static int CheckThicknessNGNum { get; set; }

        /// <summary>
        /// 保有量公式
        /// </summary>
        public static int RetetionFormula { get; set; }
        /// <summary>
        /// 变量注液公式
        /// </summary>
        public static int VarInjFormula { get; set; }

        /// <summary>
        /// 开机屏蔽电池个数(注液闭环控制)
        /// </summary>
        public static int StartMachineConut { get; set; }

        /// <summary>
        /// 工位屏蔽电池个数(注液闭环控制)
        /// </summary>
        public static int StationConut { get; set; }

        public static bool IsTcpClose { get; set; }

        /// <summary>
        /// 后称重补偿
        /// </summary>
        public static double AfterWeighCompensation { get; set; }

        /// <summary>
        /// 焊机结果反馈PLC否
        /// </summary>
        public static bool IsWeldResult { get; set; }

        /// <summary>
        /// GRR测试人数
        /// </summary>
        public static int GRROperator { get; set; } = 3;
        /// <summary>
        /// 启用能量曲线图否
        /// </summary>
        public static bool IsWeldEnergy { get; set; }

        public static int WeldEnergy_LSL { get; set; }
        public static int WeldEnergy_Num { get; set; }

        public static int WeldPower_LSL { get; set; }
        public static int WeldPower_Num { get; set; }

        public static int WeldWeight_LSL { get; set; }
        public static int WeldWeight_Num { get; set; }

        /// <summary>
        /// 是否采用GRR模板填写
        /// </summary>
        public static bool IsGRRTemplate { get; set; }


        /// <summary>
        /// 与欧姆龙通讯反馈节点号
        /// </summary>
        public static string OmronNodeAlarmIp { get; set; }

        /// <summary>
        /// 与欧姆龙通讯反馈节点号
        /// </summary>
        public static string OmronNodeIp { get; set; }
        /// <summary>
        /// 与欧姆龙通讯的客户端IP
        /// </summary>
        public static int OmronLocalIp { get; set; }
        /// <summary>
        /// 欧姆龙PLC-ip
        /// </summary>
        public static int OmronPlcIp { get; set; }
        /// <summary>
        /// 间隔参数上传时间
        /// </summary>
        public static int UploadArgInterval { get; set; }

        /// <summary>
        /// 测厚标定值
        /// </summary>
        public static double ThicknessCalibration { get; set; }

        /// <summary>
        /// 测厚补偿值
        /// </summary>
        public static double ThicknessCompensate { get; set; }

        /// <summary>
        /// 正极测厚度NG个数
        /// </summary>
        public static double ThicknessNum1 { get; set; }

        /// <summary>
        /// 负极测厚度NG个数
        /// </summary>
        public static double ThicknessNum2 { get; set; }

        /// <summary>
        /// 底封测厚度NG个数
        /// </summary>
        public static double ThicknessNum3 { get; set; }

        /// <summary>
        /// 变量注液补偿上限
        /// </summary>
        public static double VarInjCompensationUSL { get; set; }
        /// <summary>
        /// 变量注液补偿下限
        /// </summary>
        public static double VarInjCompensationLSL { get; set; }
        /// <summary>
        /// 变量注液数量
        /// </summary>
        public static int VarInjectNumber { get; set; }
        /// <summary>
        /// 是否启用闭环注液
        /// </summary>
        public static bool IsVarInject { get; set; }
        /// <summary>
        /// 测厚仪是否一对多的电池
        /// </summary>
        public static bool IsThicknessOneToMore { get; set; }
        /// <summary>
        /// 上传失败是否反馈PLC否
        /// </summary>
        public static bool IsUploadNgPlc { get; set; }
        /// <summary>
        /// 硬盘最小百分比
        /// </summary>
        public static double MinMemory { get; set; }
        /// <summary>
        /// 4测厚线性
        /// </summary>
        public static bool FourThickness { get; set; }


        /// <summary>
        /// 是否周次抽检
        /// </summary>
        public static bool IsWeekCheck { get; set; }


        /// <summary>
        /// 抽检周次
        /// </summary>
        public static int? WeekCheck { get; set; }

        /// <summary>
        /// 抽检个数
        /// </summary>
        public static int? WeekCheckCount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public static string SqlIsNullKey { get; set; }
        /// <summary>
        /// SQL锁关键字
        /// </summary>
        public static string SqlLockKey { get; set; }


        public static string SqlDBKey { get; set; }
        /// <summary>
        /// 数据日期关键字1.datediff,2.TIMESTAMPDIFF
        /// </summary>
        public static string SqlDateKey { get; set; }
        /// <summary>
        /// Cast整型关键字1.int,2.SIGNED
        /// </summary>
        public static string SqlCastIntKey { get; set; }

        /// <summary>
        /// Cast字符串1.varchar,2.char(N)
        /// </summary>
        public static string SqlCastStringKey { get; set; }
        /// <summary>
        ///  from DUAL 
        /// </summary>
        public static string SqlFromKey { get; set; }


        //{ get; set; }
        /// <summary>
        /// 扫码启用是否一对多
        /// </summary>
        public static bool IsScanBarCodeOneToMore { get; set; }

        /// <summary>
        /// LED描述起始截取
        /// </summary>
        public static int LEDStartIndex { get; set; }
        /// <summary>
        /// LED描述终止截取
        /// </summary>
        public static int LEDEndIndex { get; set; }

        /// <summary>
        /// 补液针头数
        /// </summary>
        public static int TotalRepairNeedle { get; set; }

        /// <summary>
        /// 保液量下限
        /// </summary>
        public static double InjectRetention_LSL { get; set; }
        /// <summary>
        /// 保液量上限
        /// </summary>
        public static double InjectRetention_USL { get; set; }

        /// <summary>
        /// 设备类型
        /// </summary>
        public static int MachineType { get; set; }
        private static bool isFixtureScan = false;


        /// <summary>
        /// 允许点检时间(分钟)
        /// </summary>
        public static int PointStartScope { get; set; }
        /// <summary>
        /// 日志保存月数
        /// </summary>
        public static int LogSaveMonth { get; set; }

        /// <summary>
        /// 结束生产导出数据
        /// </summary>
        public static bool IsStopImportCSV { get; set; }

        /// <summary>
        /// 注液补液公式
        /// </summary>
        public static int RepirInjFormula { get; set; }
        /// <summary>
        /// 测厚方案
        /// </summary>
        public static int ThinessProgramme { get; set; }

        /// <summary>
        /// 注液量计算公式
        /// </summary>
        public static int InjVolumeFormula { get; set; }
        /// <summary>
        /// 注液结果计算公式
        /// </summary>
        public static int InjResultFormula { get; set; }
        /// <summary>
        /// 电阻上限
        /// </summary>
        public static double? RI_USL { get; set; }
        /// <summary>
        /// 电阻下限
        /// </summary>
        public static double? RI_LSL { get; set; }

        /// <summary>
        /// 电压上限
        /// </summary>
        public static double Volt_USL { get; set; }
        /// <summary>
        /// 电压下限
        /// </summary>
        public static double Volt_LSL { get; set; }

        /// <summary>
        /// 保存文件格式
        /// </summary>
        public static string SaveSuffix { get; set; }

        /// <summary>
        ///  接收心跳次数失败停机
        /// </summary>
        public static int HeartbeatReceiveStopNum { get; set; }

        /// <summary>
        /// 是否生成CSV表头
        /// </summary>
        public static bool IsCSVHeader { get; set; }
        /// <summary>
        /// 是否从数据库中校验条码
        /// </summary>
        public static bool IsDbVerifyBarCode { get; set; }
        /// <summary>
        /// 导出CSV数据
        /// </summary>
        public static bool IsDayCSV { get; set; }

        /// <summary>
        /// 保存文件分隔符
        /// </summary>
        public static string SaveSplit { get; set; }

        /// <summary>
        /// 是否启用模板验证
        /// </summary>
        public static bool IsTemplate { get; set; }
        /// <summary>
        /// 数据上传次数
        /// </summary>
        public static int contUpLoadNum { get; set; }

        /// <summary>
        /// 是否启用MES
        /// </summary>
        public static bool IsMes { get; set; }

        private static bool isTesting = false;

        private static bool isWeld = false;

        private static bool isThickness = false;

        /// <summary>
        /// 启用是否保存第一次前称重数据
        /// </summary>

        public static bool IsSaveFristBefore { get; set; }

        /// <summary>
        /// 扫码NG不排NG
        /// </summary>

        public static bool ScanNoNG { get; set; }
        /// <summary>
        /// 复判新版本
        /// </summary>
        public static bool NewCheck { get; set; }
        /// <summary>
        ///隐藏未扫码未下料
        /// </summary>
        
        public static bool EnableUnLoading { get; set; }
        /// <summary>
        /// 是否启用清零数据与下一次重量比较
        /// </summary>
        public static bool IsWeighZeroData { get; set; }
        /// <summary>
        /// 清零个数
        /// </summary>
        public static int ClearWeighZeroDataNum { get; set; }

        /// <summary>
        /// 是否启用电子称缓存
        /// </summary>
        public static bool IsCacheWeigh { get; set; }
        /// <summary>
        /// 缓存数量
        /// </summary>
        public static int CacheWeighNum { get; set; }
        /// <summary>
        /// 下料扫码与后称重二合一否
        /// </summary>
        public static bool IsUnScanWeighMerge { get; set; }
        /// <summary>
        /// 下料扫码失败按计数匹配
        /// </summary>
        public static bool IsUnScanErrorByCount { get; set; }

        /// <summary>
        /// 反喷码反HI-POT
        /// </summary>
        public static bool IsRepertCodingHiPot { get; set; }

        /// <summary>
        /// HI-POT 测试仪点检是否开始
        /// </summary>
        public static bool IsHiPotTestingCheck { get; set; }

        /// <summary>
        /// 反Hi-Pot
        /// </summary>
        public static bool IsRepertHiPot { get; set; }

        /// <summary>
        /// 注液泵类型
        /// </summary>
        public static int InjectPumpType { get; set; }
        /// <summary>
        /// 注液泵小数位数
        /// </summary>
        public static int InjectPumpPointNum { get; set; }
        /// <summary>
        /// 欧姆龙PLC类型
        /// </summary>
        public static string OmronType { get; set; }

        /// <summary>
        /// 进站前称上限
        /// </summary>
        public static double BeforeUsL { get; set; }
        /// <summary>
        /// 进站前称下限
        /// </summary>
        public static double BeforeLsL { get; set; }

        /// <summary>
        /// 是否显示CPK曲线名称
        /// </summary>
        public static bool IsCPKLegends { get; set; }
        /// <summary>
        /// 异步校验数据否
        /// </summary>
        public static bool IsUploadAsync { get; set; }
        /// <summary>
        /// 中控校验的Token
        /// </summary>
        public static string Token { get; set; }

        /// <summary>
        /// 条码只能包含字母与数字
        /// </summary>
        public static bool IsBarCodeABCAndNum { get; set; }

        /// <summary>
        /// 判断数据已下料完成否
        /// </summary>
        public static bool IsUnLoadingUpdate { get; set; }
        /// <summary>
        /// 已下料完成更新时NG否
        /// </summary>
        public static bool IsUnLoadingUpdateNG { get; set; }
        /// <summary>
        /// 判断PLC计数是否重复
        /// </summary>
        public static bool IsPlcNoRepeat { get; set; }
        /// <summary>
        /// PLC计数重复NG否
        /// </summary>
        public static bool IsPlcNoRepeatNG { get; set; }

        /// <summary>
        /// 上料扫码与前称重二合一否
        /// </summary>
        public static bool IsScanWeighMerge { get; set; }
        /// <summary>
        /// 电子称实时读取数据否
        /// </summary>
        public static bool IsWeighRealtime { get; set; }

        /// <summary>
        /// 电子称实时有效个数
        /// </summary>
        public static int WeighRealtimeCount { get; set; }
        /// <summary>
        /// 电子称实时保存个数
        /// </summary>
        public static int WeighRealtimeTotalCount { get; set; }
        /// <summary>
        /// 网络检查IP延时
        /// </summary>
        public static int NetworkSleep { get; set; }

        /// <summary>
        /// 注液均值上限
        /// </summary>
        public static double InjAvg_USL { get; set; }
        /// <summary>
        /// 注液均值下限
        /// </summary>
        public static double InjAvg_LSL { get; set; }

        /// <summary>
        /// 是否备份数据
        /// </summary>
        public static bool IsBackupDataBase { get; set; }

        /// <summary>
        /// 备份近几个月的数据
        /// </summary>
        public static int BackupDataBaseMonths { get; set; }
        /// <summary>
        /// 备份文件路径
        /// </summary>
        public static string BackupDataBasePath { get; set; }
        /// <summary>
        /// 是否从数据库中获取导出数据
        /// </summary>
        public static bool IsExportByDb { get; set; }
        /// <summary>
        /// 是否从数据库中获取上传数据
        /// </summary>
        public static bool IsUploadByDb { get; set; }
        /// <summary>
        /// 注液量上限
        /// </summary>
        public static double InjVolume_USL { get; set; }
        /// <summary>
        /// 注液量下限
        /// </summary>
        public static double InjVolume_LSL { get; set; }
        /// <summary>
        /// 导出数据单个文件总行数
        /// </summary>
        public static int SaveExportTotalRow { get; set; }

        /// <summary>
        /// 是否实时检测网络IP 
        /// </summary>
        public static bool IsNetworkRealtime { get; set; }
        /// <summary>
        /// 注液闭环按工位
        /// </summary>
        public static bool IsVarInjectStation { get; set; }
        /// <summary>
        /// 是否启用方法锁
        /// </summary>
        public static bool IsMethodLock { get; set; }
        /// <summary>
        /// 连续NG是否结束生产
        /// </summary>
        public static bool IsContiuneNgStop { get; set; }

        /// <summary>
        /// 是否检测产品型号
        /// </summary>
        public static bool IsJudgeProModel { get; set; }
        /// <summary>
        /// 是否控制菜单权限
        /// </summary>
        public static bool IsRightMenu { get; set; }
        /// <summary>
        /// 几分钟未操作电脑
        /// </summary>
        public static int MenuRightContorlTimer { get; set; }
        /// <summary>
        /// 是否实时导出
        /// </summary>
        public static bool IsScvRealtime { get; set; }
        /// <summary>
        /// 选择开机验证方式 mfg/mes
        /// </summary>
        public static string VerifyVlaue { get; set; }

        public static string CipSessionDefault = "00000000";

        private static bool isDistinct = false;

        /// <summary>
        /// IV点检时间
        /// </summary>
        public string IvCheckTime { get; set; }

        /// <summary>
        /// OCV点检时间
        /// </summary>

        public string OcvCheckTime { get; set; }
        /// <summary>
        /// MDI点检时间
        /// </summary>

        public string MdiCheckTime { get; set; }
        /// <summary>
        /// PPG点检时间
        /// </summary>

        public string PpgCheckTime { get; set; }
        /// <summary>
        ///Xray点检时间
        /// </summary>

        public string XrayCheckTime { get; set; }
        public string NeedleCheckTime { get; set; }
        /// <summary>
        /// OCV模式
        /// </summary>
        public string OCVMode { get; set; }
        /// <summary>
        /// Marking管理
        /// </summary>
        public string MarkingCurrent { get; set; }

        /// <summary>
        /// Marking
        /// </summary>
        public string MarkingBase { get; set; }
        /// <summary>
        /// 工厂管理
        /// </summary>
        public string FactoryCode { get; set; }
        /// <summary>
        /// pc名称
        /// </summary>

        public string PcName { get; set; }
        /// <summary>
        /// 指定电池类别
        /// </summary>
        public string CellType { get; set; }
        /// <summary>
        ///TIFF算法
        /// </summary>
        public bool IsTiffMode { get; set; }
        /// <summary>
        /// OCV连续报警
        /// </summary>
        public int OCVWarmingCounts { get; set; }
        /// <summary>
        /// 电池温度和环境温度差
        /// </summary>
        public float RangeOfTemperatrue { get; set; }
        /// <summary>
        /// MDI连续NG报警
        /// </summary>
        public int MDIWarmingCounts { get; set; }
        /// <summary>
        /// 厚度连续NG报警
        /// </summary>
        public int PPGWarmingCounts { get; set; }
        /// <summary>
        /// 产品类型
        /// </summary>
        public string ProductMode { get; set; }
        /// <summary>
        /// 产品编号
        /// </summary>
        public string ProductNO { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string MIDataDir { get; set; }


        public string ReModelDir { get; set; }

        public string BisUri { get; set; }
        public ECheckModes CheckMode { get; set; }
        public IEnumerable<ECheckModes> BindableCheckModes { get; set; }


        public ImageSaveConfig MyImageSaveConfig { get; set; }

        public StartupTestConfig MyStartupTestConfig { get; set; }
        public InspectParams MyInspectParams { get; set; }

        // 检测算法结构体无法直接绑定，下面是Wrapper

        public int iLine { get; set; }
        public int TotalLayer { get; set; }
        public float PixelToMM { get; set; }
        /// <summary>
        ///     //头部最小值
        /// </summary>
        public float MinLengthHead { get; set; }
        /// <summary>
        ///     //头部最大值
        /// </summary>
        public float MaxLengthHead { get; set; }
        /// <summary>
        /// 尾部最小值
        /// </summary>
        public float MinLengthTail { get; set; }
        /// <summary>
        /// 尾部最大值
        /// </summary>
        public float MaxLengthTail { get; set; }
        /// <summary>
        /// 最大角度阈值
        /// </summary>
        public float MaxAngleThresh { get; set; }
        /// <summary>
        /// 显示检测线
        /// </summary>
        public bool IsDrawLine { get; set; }
        /// <summary>
        /// 显示检测数据
        /// </summary>
        public bool IsShowData { get; set; }
        /// <summary>
        /// 显示检测角度
        /// </summary>
        public bool IsShowAngle { get; set; }
        /// <summary>
        /// 需要测试角度
        /// </summary>
        public bool IsDetectAngle { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public float RectX { get; set; }

        public float RectY { get; set; }
        /// <summary>
        /// AC角设置-Width
        /// </summary>
        public float RectWidth { get; set; }
        /// <summary>
        /// AC角设置-Height
        /// </summary>
        public float RectHeight { get; set; }

        public bool IsGRRMode { get; set; }

        public bool IsSamplingMode { get; set; }
        public bool IsCheckABCell { get; set; }
        public string MarkingOfACell { get; set; }

        public string MarkingOfABSide { get; set; }
        public bool IsNoUploadMES { get; set; }
        /// <summary>
        ///  //BD角设置-电池层数
        /// </summary>
        public int TotalLayersBD { get; set; }
        /// <summary>
        /// BD角设置-Width
        /// </summary>
        public float RectWidthBD { get; set; }
        /// <summary>
        /// BD角设置-Height
        /// </summary>
        public float RectHeightBD { get; set; }
        /// <summary>
        /// 厚度设置-最大值
        /// </summary>
        public float MaxThickness { get; set; }
        /// <summary>
        /// 厚度设置-最小值
        /// </summary>
        public float MinThickness { get; set; }
        /// <summary>
        /// 厚度设置-标定值
        /// </summary>
        public float CaliValThickness { get; set; }
        /// <summary>
        /// 厚度设置-K值
        /// </summary>
        public float ThicknessKValue { get; set; }
        /// <summary>
        ///厚度设置-B值
        /// </summary>
        public float ThicknessBValue { get; set; }
        /// <summary>
        /// 电池K值
        /// </summary>
        public float CellKValue { get; set; }
        /// <summary>
        /// 电池B值
        /// </summary>
        public float CellBValue { get; set; }
        /// <summary>
        /// 各工位测厚极差
        /// </summary>
        public float StationRange { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int StationRangeNum { get; set; }
        /// <summary>
        /// 工位均值报警
        /// </summary>
        public float StationWarmingAverage { get; set; }
        /// <summary>
        /// 工位均值报警公差
        /// </summary>
        public float StationWarmingTolerance { get; set; }

        public int ThicknessCalibrationModeA { get; set; }

        public int ThicknessCalibrationModeB { get; set; }

        public int ThicknessCalibrationModeC { get; set; }

        public int ThicknessCalibrationModeD { get; set; }

        public string CurrentModel { get; set; }
        /// <summary>
        /// 小标定块-最大值
        /// </summary>
        public float MaxThicknessS { get; set; }
        /// <summary>
        /// 小标定块-最小值
        /// </summary>
        public float MinThicknessS { get; set; }
        /// <summary>
        /// 中标定块-最大值
        /// </summary>
        public float MaxThicknessM { get; set; }
        /// <summary>
        /// 中标定块-最大值
        /// </summary>
        public float MinThicknessM { get; set; }
        /// <summary>
        /// 大标定块-最大值
        /// </summary>
        public float MaxThicknessB { get; set; }
        /// <summary>
        /// 大标定块-最小值
        /// </summary>
        public float MinThicknessB { get; set; }
        /// <summary>
        /// 点检通过的次数
        /// </summary>
        public float LinearOKNum { get; set; }
        /// <summary>
        /// 点检不通过的次数
        /// </summary>
        public float LinearNGNum { get; set; }
        /// <summary>
        /// 极差允许范围
        /// </summary>
        public float LinearRange { get; set; }
        /// <summary>
        /// 强制要求点检通过的次数
        /// </summary>
        public float LinearNeedOK { get; set; }
        /// <summary>
        /// 电芯线性点检完成时间
        /// </summary>
        public string LinearCheckTime { get; set; }
        /// <summary>
        /// 当前点检对应的班次开始时间
        /// </summary>
        public string LinearCheckStartTime { get; set; }
        /// <summary>
        /// 当前点检对应的班次结束时间
        /// </summary>
        public string LinearCheckEndTime { get; set; }
        public int DimensionCalibrationMode { get; set; }
        public string BatLengthS { get; set; }
        public string BatWidthS { get; set; }
        public string LeftLugS { get; set; }
        public string RightLugS { get; set; }
        public string AllLengthS { get; set; }
        public string Left1GlueS { get; set; }
        public string Left2GlueS { get; set; }
        public string Right1GlueS { get; set; }
        public string Right2GlueS { get; set; }
        public string LeftLugLenS { get; set; }
        public string MidLugS { get; set; }
        public string MidLugLenS { get; set; }
        public string Mid1GlueS { get; set; }
        public string Mid2GlueS { get; set; }
        public string MidLugM { get; set; }
        public string MidLugLenM { get; set; }
        public string Mid1GlueM { get; set; }
        public string Mid2GlueM { get; set; }
        public string MidLugB { get; set; }
        public string MidLugLenB { get; set; }
        public string Mid1GlueB { get; set; }
        public string Mid2GlueB { get; set; }
        public string RightLugLenS{ get; set; }
        public string BatLengthM { get; set; }
        public string BatWidthM { get; set; }
        public string LeftLugM { get; set; }
        public string RightLugM { get; set; }
        public string AllLengthM { get; set; }
        public string Left1GlueM { get; set; }
        public string Left2GlueM { get; set; }
        public string Right1GlueM { get; set; }
        public string Right2GlueM { get; set; }
        public string LeftLugLenM { get; set; }
        public string RightLugLenM { get; set; }
        public string BatLengthB { get; set; }
        public string BatWidthB { get; set; }
        public string LeftLugB { get; set; }
        public string RightLugB { get; set; }
        public string AllLengthB { get; set; }
        public string Left1GlueB { get; set; }
        public string Left2GlueB { get; set; }
        public string Right1GlueB { get; set; }
        public string Right2GlueB { get; set; }
        public string LeftLugLenB { get; set; }
        public string RightLugLenB { get; set; }
        /// <summary>
        /// 电池主体长度最小值
        /// </summary>
        public float MinBatLength { get; set; }
        /// <summary>
        /// 电池主体长度最大值
        /// </summary>
        public float MaxBatLength { get; set; }
        /// <summary>
        /// 电池主体宽度最小值
        /// </summary>
        public float MinBatWidth { get; set; }
        /// <summary>
        /// 电池主体宽度最大值
        /// </summary>
        public float MaxBatWidth{ get; set; }
        /// <summary>
        /// 电池左极耳边距最小值
        /// </summary>
        public float MinLeftLugMargin { get; set; }
        /// <summary>
        ///     //电池左极耳边距最大值
        /// </summary>
        public float MaxLeftLugMargin { get; set; }
        /// <summary>
        /// 电池中间极耳边距最小值
        /// </summary>
        public float MinMidLugMargin { get; set; }
        /// <summary>
        /// 电池中间极耳边距最大值
        /// </summary>
        public float MaxMidLugMargin { get; set; }
        /// <summary>
        /// 电池右极耳边距最小值
        /// </summary>
        public float MinRightLugMargin { get; set; }
        /// <summary>
        /// 电池右极耳边距最大值
        /// </summary>
        public float MaxRightLugMargin { get; set; }
        /// <summary>
        /// 左极耳长度最小值
        /// </summary>
        public float MinLeftLugLength { get; set; }
        /// <summary>
        /// 左极耳长度最大值
        /// </summary>
        public float MaxLeftLugLength { get; set; }
        /// <summary>
        /// 电池中间极耳长度最小值
        /// </summary>
        public float MinMidLugLength { get; set; }
        /// <summary>
        /// 电池中间极耳边距最大值
        /// </summary>
        public float MaxMidLugLength { get; set; }
        /// <summary>
        /// 右极耳长度最小值
        /// </summary>
        public float MinRightLugLength { get; set; }
        /// <summary>
        /// 右极耳长度最大值
        /// </summary>
        public float MaxRightLugLength { get; set; }
        /// <summary>
        /// 电池总长度最小值
        /// </summary>
        public float MinAllBatLength{ get; set; }
        /// <summary>
        /// 电池总长度最大值
        /// </summary>
        public float MaxAllBatLength { get; set; }
        /// <summary>
        /// 电池左1小白胶最小值
        /// </summary>
        public float MinLeft1WhiteGlue { get; set; }
        public float MinLeft1WhiteGlueMin { get; set; }
        public float MaxLeft1WhiteGlue { get; set; }
        public float MaxLeft1WhiteGlueMin { get; set; }
        /// <summary>
        /// 电池左2小白胶最小值
        /// </summary>
        public float MinLeft2WhiteGlue { get; set; }
        public float MinLeft2WhiteGlueMin { get; set; }
        /// <summary>
        /// 电池左2小白胶最大值
        /// </summary>
        public float MaxLeft2WhiteGlue { get; set; }
        public float MaxLeft2WhiteGlueMin { get; set; }
        /// <summary>
        /// 电池中间1小白胶最小值
        /// </summary>
        public float MinMid1WhiteGlue { get; set; }
        /// <summary>
        /// 电池中间1小白胶最大值
        /// </summary>
        public float MaxMid1WhiteGlue { get; set; }
        /// <summary>
        /// 电池中间2小白胶最小值
        /// </summary>
        public float MinMid2WhiteGlue { get; set; }
        /// <summary>
        /// 电池中间2小白胶最大值
        /// </summary>
        public float MaxMid2WhiteGlue { get; set; }
        /// <summary>
        /// 电池右1小白胶最小值
        /// </summary>
        public float MinRight1WhiteGlue { get; set; }
        public float MinRight1WhiteGlueMin { get; set; }
        /// <summary>
        /// 电池右1小白胶最大值
        /// </summary>
        public float MaxRight1WhiteGlue { get; set; }
        public float MaxRight1WhiteGlueMin { get; set; }
        /// <summary>
        /// 电池右2小白胶最小值
        /// </summary>
        public float MinRight2WhiteGlue { get; set; }
        public float MinRight2WhiteGlueMin { get; set; }
        /// <summary>
        /// 电池右2小白胶最大值
        /// </summary>
        public float MaxRight2WhiteGlue { get; set; }
        public float MaxRight2WhiteGlueMin { get; set; }
        /// <summary>
        /// 不上传尺寸测厚
        /// </summary>
        public bool IsNoUpLoadMdiAndPPGData { get; set; }
        /// <summary>
        /// 铝极耳在左
        /// </summary>
        public bool IsAlOnLeft { get; set; }
        /// <summary>
        /// 是否为3极耳电芯
        /// </summary>
        public bool IsThreeTab { get; set; }
        /// <summary>
        /// 电池内阻最大值
        /// </summary>
        public float MaxResistance { get; set; }
        /// <summary>
        /// 电池内阻最小值
        /// </summary>
        public float MinResistance { get; set; }
        /// <summary>
        /// 电池电压最小值
        /// </summary>
        public float MinVoltage { get; set; }
        /// <summary>
        /// 电池电压最大值
        /// </summary>
        public float MaxVoltage { get; set; }
        public float MinOCV_k { get; set; }
        public float MaxOCV_k { get; set; }
        /// <summary>
        /// 工位1温度补偿
        /// </summary>
        public float TemperatureCoefficient { get; set; }
        /// <summary>
        /// 工位2温度补偿
        /// </summary>
        public float TemperatureCoefficient2 { get; set; }
        /// <summary>
        /// 电压补偿
        /// </summary>
        public float VoltageCoefficient { get; set; }
        /// <summary>
        /// 内阻补偿
        /// </summary>
        public float ResistanceCoefficient { get; set; }
        /// <summary>
        /// 不测内阻时内阻设定值
        /// </summary>
        public float ResistanceFixedValue { get; set; }
        /// <summary>
        /// 不测温度时温度设定值
        /// </summary>
        public float TemperatureFixedValue { get; set; }
        /// <summary>
        /// 不测内阻
        /// </summary>
        public bool IsNoResistance { get; set; }
        /// <summary>
        /// 不测温度
        /// </summary>
        public bool IsNoTemperature { get; set; }
        /// <summary>
        /// Marking管理-启用
        /// </summary>
        public bool IsCheckMarking { get; set; }
        /// <summary>
        /// //周次校验-启用
        /// </summary>
        public bool IsCheckWeekCount { get; set; }
        /// <summary>
        /// 标记Marking-启用
        /// </summary>
        public bool IsSetMarking { get; set; }
        /// <summary>
        /// MI号校验-启用
        /// </summary>
        public bool IsCheckMI { get; set; }
        /// <summary>
        /// 电池温度最小值
        /// </summary>
        public float MinTemperature { get; set; }
        /// <summary>
        /// 电池温度最大值
        /// </summary>
        public float MaxTemperature { get; set; }
        /// <summary>
        /// MI号校验
        /// </summary>
        public string Mi { get; set; }
        public string Marking { get; set; }
        /// <summary>
        /// 标记Marking
        /// </summary>
        public string SetMarking { get; set; }
        /// <summary>
        /// 周次校验
        /// </summary>
        public int WeekCounts { get; set; }
        public float ThicknessAvgDiff { get; set; }
        /// <summary>
        ///IV下限
        /// </summary>
        public float MinIV { get; set; }
        /// <summary>
        /// IV上限
        /// </summary>
        public float MaxIV { get; set; }
        /// <summary>
        /// IV初始值
        /// </summary>
        public float Source { get; set; }
        /// <summary>
        /// IV跳变值
        /// </summary>
        public float Range { get; set; }
        /// <summary>
        /// IV异常值1
        /// </summary>
        public float ExceptionData1 { get; set; }
        /// <summary>
        /// IV异常值2
        /// </summary>
        public float ExceptionData2 { get; set; }
        /// <summary>
        /// IV测试时间
        /// </summary>
        public int IvTestTime { get; set; }
        /// <summary>
        /// 工位1对应通道
        /// </summary>
        public int IvStation1Channel { get; set; }
        /// <summary>
        /// 工位2对应通道
        /// </summary>
        public int IvStation2Channel { get; set; }
        /// <summary>
        /// 工位3对应通道
        /// </summary>
        public int IvStation3Channel { get; set; }
        /// <summary>
        /// 工位4对应通道
        /// </summary>
        public int IvStation4Channel { get; set; }


















/// <summary>
/// PLC下料原因
/// </summary>
public static List<DictionaryRef> PLCArgRemark = null;

        public static string ApplicationStartupPath { get; set; } = Application.StartupPath;


        #region 获取更多的异常信息

        public static string GetExceptionInfo(Exception ex)
        {
            var sb = new StringBuilder();
            BuilderExceptionContent(ex, sb);
            return sb.ToString();
        }


        /// <summary>
        /// 获取更多的异常信息 (递归获取内部异常消息和堆栈)
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="builder"></param>
        public static string BuilderExceptionContent(Exception ex, StringBuilder builder = null, int level = 0)
        {
            if (ex == null)
            {
                throw new ArgumentNullException("参数不能为空: ex");
            }
            if (builder == null)
            {
                builder = new StringBuilder();
                builder.AppendLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            }
            if (level > 0)
            {
                builder.Append($"内部异常({level}):");
            }
            builder.AppendLine(ex.Message);
            builder.AppendLine(ex.StackTrace);

            if (ex.InnerException == null)
            {
                builder.AppendLine();
                return builder.ToString();
            }
            level++;

            return BuilderExceptionContent(ex.InnerException, builder, level);

        }
        #endregion


        public static int LangIndex { get; set; }
        public static string GetAssemblyPath
        {
            get
            {
                string _CodeBase = System.Reflection.Assembly.GetExecutingAssembly().CodeBase;

                _CodeBase = _CodeBase.Substring(8, _CodeBase.Length - 8);    // 8是file:// 的长度  

                string[] arrSection = _CodeBase.Split(new char[] { '/' });

                string _FolderPath = "";
                for (int i = 0; i < arrSection.Length - 1; i++)
                {
                    _FolderPath += arrSection[i] + @"\";
                }
                return _FolderPath;
            }
        }

        /// <summary>
        /// 获取默认连接串
        /// </summary>
        public static string ConnectionString
        {
            get
            {
                ConnectionStringSettings cm = ConfigurationManager.ConnectionStrings["mysql"];
                string conStr = cm.ConnectionString;
                //conStr = DESEncrypt.Decrypt(conStr);
                return conStr;
            }
        }
        public static string GetConnectionString(string section)
        {

            string serverIP = ConfigurationManager.AppSettings["ServerIP"];

            return serverIP;

        }
        public static string UnLoadConnectionString
        {
            get
            {
                string serverIP = ConfigurationManager.AppSettings["UnLoadServerIP"];

                return serverIP;
            }
        }

        /// <summary>
        /// 获取指定连接串
        /// </summary>
        /// <param name="uploadConnectionStringName"></param>
        /// <returns></returns>
        public static string ConnectionStringUpload
        {
            get
            {

                string conStr = ConfigurationManager.ConnectionStrings[ConfigurationManager.AppSettings["UploadConnectionStringName"]].ConnectionString;
                return conStr;
            }
        }

        /// <summary>
        /// 扩展方法，获得枚举的Description
        /// </summary>
        /// <param name="value">枚举值</param>
        /// <param name="nameInstead">当枚举值没有定义DescriptionAttribute，是否使用枚举名代替，默认是使用</param>
        /// <returns>枚举的Description</returns>
        public static string GetDescription(Enum value, Boolean nameInstead = true)
        {
            try
            {
                Type type = value.GetType();
                string name = Enum.GetName(type, value);
                if (name == null)
                {
                    return null;
                }

                System.Reflection.FieldInfo field = type.GetField(name);
                DescriptionAttribute attribute = System.Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;

                if (attribute == null && nameInstead == true)
                {
                    return name;
                }
                return attribute?.Description;
            }
            catch
            {
                return string.Empty;
            }
        }
        public delegate void SetStatusToPLC(bool isRes, string enabledSignal, string disabledSignal);
        public static event SetStatusToPLC ChangeStatusToPLC;
        private static bool autoUploadData = false;



        /// <summary>
        /// 是否上传
        /// </summary>
        public static bool IsUpload = false;
        /// <summary>
        /// 上传错误是否显示错误
        /// </summary>
        public static bool IsDispayUploadError { get; set; }

        /// <summary>
        /// 是否显示PLC日志
        /// </summary>
        public static bool IsPLCWriteLog { get; set; }

        public static bool NotAutoRunCollect { get; set; }
        /// <summary>
        /// 检查测试仪当前状态
        /// </summary>
        public static int TesterStateNum { get; set; }
        /// <summary>
        /// 读取计数延时时间
        /// </summary>
        public static int SleepPlcNumer { get; set; } = 200;

        /// <summary>
        /// 消息序号
        /// </summary>
        public static long MsgNO { get; set; }

        /// <summary>
        /// 是否共用同一方法通讯
        /// </summary>
        public static bool IsShareCommunication { get; set; }

        public static string GetConfig(List<UploadArgConfig> mArg, string argName)
        {

            var m = mArg.FirstOrDefault(o => o.ArgCode.Equals(argName));
            if (m != null)
            {
                return m.ArgVal;
            }
            return string.Empty;

        }
        #region 日置  HIOKI_ST552x
        public const string NoComp = "NOCOMP";
        #endregion
        #region 中茂测试仪-11210   系列测试结果
        /// <summary>
        /// 0  Total PASS
        /// </summary>
        public const string TotalPASS = "PASS";
        /// <summary>
        /// 1  Discharge  Fail
        /// </summary>
        public const string Discharge = "Discharge  Fail";
        /// <summary>
        /// 2 Charge Fail 
        /// </summary>
        public const string Charge = "Charge Fail ";
        /// <summary>
        /// 3  Contact Check Fail
        /// </summary>
        public const string Contact = "Contact Check Fail";
        /// <summary>
        /// 4  LC Low/IR High 
        /// </summary>
        public const string LCLow = "LC Low/IR High";
        /// <summary>
        /// 5 LC High/IR Low
        /// </summary>
        public const string LCHigh = "LC High/IR Low";
        /// <summary>
        /// 6  LC Over Range
        /// </summary>
        public const string LCOver = "LC Over Range";
        /// <summary>
        /// 7   Auto Range Fail
        /// </summary>
        public const string AutoRange = "Auto Range Fail";
        /// <summary>
        /// 8   FD Fail
        /// </summary>
        public const string FDFail = "FD Fail";
        /// <summary>
        /// 9  TriggerStop
        /// </summary>
        public const string TriggerStop = "TriggerStop";
        #endregion
        #region 中茂1907X 系列测试结果

        //电压电阻测试结果
        /// <summary>
        /// Stop
        /// </summary>
        public const string Stop = "Stop";
        /// <summary>
        /// User
        /// </summary>
        public const string User = "User INTERRUPT";
        /// <summary>
        /// CNT
        /// </summary>
        public const string CNT = "CNT";
        /// <summary>
        /// TestING
        /// </summary>
        public const string TestING = "TestING";
        /// <summary>
        /// SKIPPED
        /// </summary>
        public const string SKIPPED = "SKIPPED";
        /// <summary>
        /// GFITRIPPED
        /// </summary>
        public const string GFITRIPPED = "GFITRIPPED";

        /// <summary>
        /// SLAVEFAIL
        /// </summary>
        public const string SLAVEFAIL = "SLAVE FAIL";
        /// <summary>
        /// GsSHORTFail
        /// </summary>
        public const string GsSHORTFail = "Gs/SHORT FAIL";

        //电容测试结果
        /// <summary>
        /// SHORTFail
        /// </summary>
        public const string SHORTFail = "SHORT FAIL";
        /// <summary>
        /// OPENFail
        /// </summary>
        public const string OPENFail = "OPEN FAIL";
        /// <summary>
        /// I/O FAIL
        /// </summary>
        public const string IOFail = "I/O FAIL";
        /// <summary>
        /// VOLT FAIL
        /// </summary>
        public const string VOLTFail = "VOLT FAIL";
        /// <summary>
        /// CURR FAIL
        /// </summary>
        public const string CURRFail = "CURR FAIL";


        #endregion
        #region 华士捷喷码机-F550-50SI系列 系列测试结果

        public const string CheckError = "校验值错误";

        public const string InstructionError = "指令错误";

        public const string CommunicationError = "喷码通讯未启用";
        #endregion

        #region 称重处理结果
        /// <summary>
        /// Pass
        /// </summary>
        public const string PASS = "Pass";
        /// <summary>
        /// Fail
        /// </summary>
        public const string FAIL = "Fail";
        /// <summary>
        /// OK
        /// </summary>
        public const string OK = "OK";
        /// <summary>
        /// NG
        /// </summary>
        public const string NG = "NG";
        /// <summary>
        /// 没有数据
        /// </summary>
        public const string NoData = "NoData";
        /// <summary>
        /// 数据错误
        /// </summary>
        public const string ErrorData = "ErrorData";
        /// <summary>
        /// 未知
        /// </summary>
        public const string Unknown = "--";

        public const string ErrorScanBarCode = "ERROR";
        /// <summary>
        /// 回车符
        /// </summary>
        public const string ENTER = "\n";

        /// <summary>
        /// 换行符
        /// </summary>
        public const string LINE = "\r";


        #endregion
        /// <summary>
        /// 测试仪器状态
        /// </summary>
        public class TesterState
        {
            public class Chorme
            {
                /// <summary>
                /// 完成
                /// </summary>
                public const string DONE = "IDLE";
                /// <summary>
                /// 放电\充电中
                /// </summary>
                public const string CHARGEING = "CHG";
                /// <summary>
                /// 延时中
                /// </summary>
                public const string DWELLING = "DWELL";
                /// <summary>
                /// 测试中
                /// </summary>
                public const string TESTING = "TEST";
                /// <summary>
                /// 测试中
                /// </summary>
                public const string DCHG = "DCHG";
            }
            public class HIOKI
            {
                /// <summary>
                /// 完成
                /// </summary>
                public const string DONE = "0";
                /// <summary>
                /// 测试中
                /// </summary>
                public const string TESTING = "1";
                /// <summary>
                /// 放电\充电中
                /// </summary>
                public const string CHARGEING = "2";

            }
        }
        /// <summary>
        /// 0，关机；1，开机；2，运行；3，待机；4，维护；5，故障
        /// </summary>
        public class DeviceState
        {

            /// <summary>
            /// 设备停机状态
            /// </summary>
            public static int Stop = 21;
            /// <summary>
            /// 设备开机状态
            /// </summary>
            public static int Open = 10;


            /// <summary>
            /// 风帆停机状态
            /// </summary>
            public static int Stop_FF = 0;

            /// <summary>
            /// 风帆开机状态
            /// </summary>
            public static int Open_FF = 1;

            /// <summary>
            /// 运行
            /// </summary>
            public static int Run = 2;
            /// <summary>
            /// 待机
            /// </summary>
            public static int Wait = 3;
            /// <summary>
            /// 维护
            /// </summary>
            public static int Maintain = 4;
            /// <summary>
            /// 故障
            /// </summary>
            public static int Fault = 5;
        }
        /// <summary>
        /// 上传接口编译器
        /// </summary>
        public static CompilerResults UploadCR { get; set; }
        /// <summary>
        /// PLC计数对象
        /// </summary>
        public static IList<Communication> lstComm { get; set; }
        /// <summary>
        /// 连接对象
        /// </summary>
        public static List<SocketTcp> lstTcp { get; set; }

        public static IList<ParametricDataArray> lstDataArray { get; set; }

        /// <summary>
        /// 上传反馈NG错误代码
        /// </summary>
        public static List<DictionaryRef> LstUploadNgCode { get; set; }
        /// <summary>
        /// 上料扫码
        /// </summary>
        public static bool IsScan { get; set; }
        /// <summary>
        /// 下料扫码二合一
        /// </summary>
        public static bool IsUnLoadScanAllInOne { get; set; }

        /// <summary>
        /// 通讯协议
        /// </summary>
        public enum EnumCommunicationType
        {
            /// <summary>
            /// SocketTCP
            /// </summary>
            //[Description("SocketTCP")]
            [EnumDisplay("SocketTCP")]
            SocketTCP = 0,
            /// <summary>
            /// SocketUDP
            /// </summary>
            //[Description("SocketUDP")]
            [EnumDisplay("SocketUDP")]
            SocketUDP = 1,
            /// <summary>
            /// 倍福通讯
            /// </summary>
            //[Description("倍福通讯")]
            [EnumDisplay("倍福通讯")]
            Twincat = 2,
            /// <summary>
            /// 西门子S7
            /// </summary>
            //[Description("西门子S7")]
            [EnumDisplay("西门子S7")]
            SiemensS7 = 3,

            /// <summary>
            /// ModbusTCP
            /// </summary>
            //[Description("ModbusTCP")]
            [EnumDisplay("ModbusTCP")]
            ModbusTCP = 4,

            ///// <summary>
            ///// 欧姆龙Fins-Tcp
            ///// </summary>
            //[Description("欧姆龙Fins-Tcp")]
            //OmronFinsTcp = 5,

            ///// <summary>
            ///// 欧姆龙Fins-Udp
            ///// </summary>
            //[Description("欧姆龙Fins-Udp")]
            //OmronFinsUdp = 6,

            ///// <summary>
            ///// 三菱-Mc
            ///// </summary>
            //[Description("三菱-Mc")]
            //MelsecMc = 7,

            /// <summary>
            /// 串口通讯
            /// </summary>
            //[Description("串口通讯")]
            [EnumDisplay("串口通讯")]
            serialPort = 8,
            /// <summary>
            /// 欧姆龙NXCIP协议
            /// </summary>
            //[Description("欧姆龙NXCIP协议")]
            [EnumDisplay("欧姆龙NXCIP协议")]
            NXCIP = 9,

            /// <summary>
            /// UDPServer
            /// </summary>
            //[Description("UDP服务器")]
            [EnumDisplay("UDP服务器")]
            SocketUDPServer = 10,

            /// <summary>
            /// TcpServer
            /// </summary>
            //[Description("TCP服务器")]
            [EnumDisplay("TCP服务器")]
            SocketTcpServer = 11,
            /// <summary>
            /// 欧姆龙CipNet协议
            /// </summary>
            //[Description("欧姆龙CipNet协议")]
            [EnumDisplay("欧姆龙CipNet协议")]
            OmronCipNet = 12,
        }


        public enum EnumSocketTcpType
        {
            /// <summary>
            /// 读PLC通讯
            /// </summary>
            //[Description("PLC通讯1")]
            [EnumDisplay("PLC通讯1", 1)]
            PLCReader = 0,
            /// <summary>
            /// 上料扫码1
            /// </summary>
            //[Description("上料扫码1")]
            [EnumDisplay("上料扫码1", 1)]
            LoadingScanBarCode1 = 1,
            /// <summary>
            /// 上料扫码2
            /// </summary>
            //[Description("上料扫码2")]
            [EnumDisplay("上料扫码2", 2)]
            LoadingScanBarCode2 = 2,
            /// <summary>
            /// 前称称重1
            /// </summary>
            //[Description("前称称重1")]
            [EnumDisplay("前称称重1", 1)]
            LoadingWeigh1 = 3,
            /// <summary>
            /// 前称称重2
            /// </summary>
            //[Description("前称称重2")]
            [EnumDisplay("前称称重2", 2)]
            LoadingWeigh2 = 4,

        }

        public static List<Enums.EnumberEntity> EnumExtentionsToList<T>()
        {
            List<Enums.EnumberEntity> list = new List<Enums.EnumberEntity>();

            foreach (var e in Enum.GetValues(typeof(T)))
            {
                Enums.EnumberEntity m = new Enums.EnumberEntity();
                object[] objArr = e.GetType().GetField(e.ToString()).GetCustomAttributes(typeof(EnumDisplayAttribute), true);
                if (objArr != null && objArr.Length > 0)
                {
                    EnumDisplayAttribute da = objArr[0] as EnumDisplayAttribute;
                    m.Desction = da.Display;
                }
                m.EnumValue = Convert.ToInt32(e);
                m.EnumName = e.ToString();
                list.Add(m);
            }
            return list;
        }


        #region 通讯仪器品牌
        /// <summary>
        /// 通讯仪器品牌
        /// </summary>
        public enum EnumInstrumentBrand
        {
            /// <summary>
            /// 无设备
            /// </summary>
            [EnumDisplay("无设备")]
            None = 0,
            /// <summary>
            /// 必能信焊机-L20系列
            /// </summary>
            //[Description("必能信焊机-L20系列")]
            [EnumDisplay("必能信焊机-L20系列")]
            Branson_L20 = 1,
            /// <summary>
            /// 骄成焊机ESx系列
            /// </summary>
            //[Description("骄成焊机-ESx系列")]
            [EnumDisplay("骄成焊机ESx系列")]
            SBT_ESx = 2,
            /// <summary>
            /// 科普焊机20KHz/40KHz系列
            /// </summary>
            //[Description("科普焊机20KHz/40KHz系列")]
            [EnumDisplay("科普焊机20KHz")]
            KEPU_20_40KHz = 3,
            /// <summary>
            /// 索尼克斯焊机-MSC系列
            /// </summary>
            //[Description("索尼克斯焊机-MSCx系列")]
            [EnumDisplay("索尼克斯焊机")]
            SONICS_MSCx = 4,
            /// <summary>
            /// 泰索迡克-TCS系列
            /// </summary>
            //[Description("泰索迡克-TCSX系列")]
            [EnumDisplay("泰索迡克-TCSX系列")]
            TELSONIC_TCSx = 5,
            /// <summary>
            /// 中茂测试仪-11210
            /// </summary>
            //[Description("中茂测试仪-11210")]
            [EnumDisplay("中茂测试仪-11210")]
            Chroma_11210 = 6,
            /// <summary>
            /// 日置测试仪-ST356x系列(电阻测量\电压测量)
            /// </summary>
            //[Description("日置测试仪-ST356x系列")]
            [EnumDisplay(@"日置测试仪-ST356x系列(电阻测量\电压测量)")]
            HIOKI_ST356x = 7,
            /// <summary>
            /// 锐捷测试仪-RJ69xF
            /// </summary>
            //[Description("锐捷测试仪-RJ69xF系列")]
            [EnumDisplay("锐捷测试仪-RJ69xF")]
            Ruijie_RJ69x = 8,
            /// <summary>
            /// 基恩士测厚仪-DL-EN系列
            /// </summary>
            //[Description("基恩士测厚仪-DL-EN系列")]
            [EnumDisplay("基恩士测厚仪-DL-EN系列")]
            Keyence_DL_EN = 9,
            /// <summary>
            /// 中茂测试仪-1907X系列
            /// </summary>
            //[Description("中茂测试仪-1907x系列")]
            [EnumDisplay("中茂测试仪-1907x系列")]
            Chroma_1907x = 10,

            /// <summary>
            /// 科普瑞测厚压力-CPR系列
            /// </summary>
            //[Description("科普瑞测厚压力-CPRx系列")]
            [EnumDisplay("科普瑞测厚压力-CPR系列")]
            Cooperate_CPRx = 11,

            /// <summary>
            /// 日置测试仪-ST552x系列
            /// </summary>
            //[Description("日置测试仪-ST552x系列")]
            [EnumDisplay("日置测试仪-ST552x系列")]
            HIOKI_ST552x = 12,
            /// <summary>
            /// 泰索迡克-M5系列
            /// </summary>
            //[Description("泰索迡克-M5系列")]
            [EnumDisplay("泰索迡克-M5系列")]
            TELSONIC_M5x = 13,
            /// <summary>
            /// 锐捷测试仪-RJ69xR系列
            /// </summary>
            //[Description("锐捷测试仪-RJ69xR系列")]
            [EnumDisplay("锐捷测试仪-RJ69xR系列")]
            Ruijie_RJ69xR = 14,

            /// <summary>
            /// 基恩士测厚仪-GT2-7x 系列
            /// </summary>
            //[Description("基恩士测厚仪-GT2-7x系列")]
            [EnumDisplay("基恩士测厚仪-GT2-7x 系列")]
            Keyence_GT2_7x = 15,

            /// <summary>
            /// 基恩士测厚仪-GT2 系列
            /// </summary>
            //[Description("基恩士测厚仪-GT2系列")]
            [EnumDisplay("基恩士测厚仪-GT2系列")]
            Keyence_GT2 = 16,
            /// <summary>
            /// 基恩士扫码枪-SR系列
            /// </summary>
            //[Description("基恩士扫码枪-SR系列")]
            [EnumDisplay("基恩士扫码枪-SR系列")]
            Keyence_SRx = 17,
            /// <summary>
            /// AND电子称
            /// </summary>
            //[Description("AND电子称")]
            [EnumDisplay("AND电子称")]
            AND = 18,

            /// <summary>
            /// 赛多利斯电子称
            /// </summary>
            //[Description("赛多利斯电子称")]
            [EnumDisplay("赛多利斯电子称")]
            BSA = 19,

            /// <summary>
            /// 霍尼韦尔扫码枪
            /// </summary>
            //[Description("霍尼韦尔扫码枪")]
            [EnumDisplay("霍尼韦尔扫码枪")]
            Honeywell_3310x = 20,
            /// <summary>
            /// RFID-OMRONV680读卡器
            /// </summary>
            //[Description("RFID欧姆龙-V680x")]
            [EnumDisplay("RFID欧姆龙-V680")]
            RFID_OMRONV680 = 21,
            /// <summary>
            /// RFID-CKURx读卡器
            /// </summary>
            //[Description("RFID晨控-URx")]
            [EnumDisplay("RFID晨控-URx")]
            RFID_CKURx = 22,

            /// <summary>
            /// 鸿伟电子称
            /// </summary>
            //[Description("鸿伟斯电子称")]
            [EnumDisplay("鸿伟斯电子称")]
            HVC = 23,

            /// <summary>
            /// AND_HSH电子称
            /// </summary>
            //[Description("AND_HSH电子称")]
            [EnumDisplay("AND_HSH电子称")]
            AND_HSH = 24,
            /// <summary>
            /// 深发源焊机
            /// </summary>
            //[Description("深发源焊机")]
            [EnumDisplay("深发源焊机")]
            SHENFAYUAN = 25,
            /// <summary>
            /// 多路温控
            /// </summary>
            //[Description("多路温控")]
            [EnumDisplay("多路温控")]
            HelionTemp = 26,
            /// <summary>
            /// 长陆电子称-TR700-D0C
            /// </summary>
            //[Description("长陆电子称-TR700")]
            [EnumDisplay("长陆电子称-TR700-D0C")]
            LongTec_TR700 = 27,

            /// <summary>
            /// 锐捷测试仪-RJ6903x系列
            /// </summary>
            //[Description("锐捷测试仪-RJ6903x系列")]
            [EnumDisplay("锐捷测试仪-RJ6903x系列")]
            Ruijie_RJ6903x = 29,

            /// <summary>
            /// 明治距离传感器
            /// </summary>
            //[Description("明治距离传感器-MLD22-X系列")]
            [EnumDisplay("明治距离传感器")]
            MLD22_X = 30,
            /// <summary>
            /// 华士捷喷码机-F550-50SI系列
            /// </summary>
            //[Description("华士捷喷码机-F550-50SI系列")]
            [EnumDisplay("华士捷喷码机-F550-50SI系列")]
            Fastjet_F550 = 31,

            /// <summary>
            /// 优霸注液泵
            /// </summary>
            //[Description("优霸注液泵")]
            [EnumDisplay("优霸注液泵")]
            InjectPump_UBA = 32,

            /// <summary>
            /// 飞升注液泵
            /// </summary>
            //[Description("飞升注液泵")]
            [EnumDisplay("飞升注液泵")]
            InjectPump_Ascend = 33,
            /// <summary>
            /// 岛津电子称
            /// </summary>
            //[Description("岛津电子称")]
            [EnumDisplay("岛津电子称")]
            SHIMADZU = 34,
            /// <summary>
            /// 中茂测试仪
            /// </summary>
            //[Description("中茂测试仪-11200")]
            [EnumDisplay("中茂测试仪-11200")]
            Chroma_11200 = 35,

            /// <summary>
            /// 易威奇注液泵
            /// </summary>
            //[Description("易威奇注液泵")]
            [EnumDisplay("易威奇注液泵")]
            InjectPump_IWAKI = 36,
        }

        #endregion

        #region 数据编码
        public enum EnumEncoding
        {
            /// <summary>
            /// 默认编码
            /// </summary>
            //[Description("Default")]
            [EnumDisplay("默认编码")]
            Default = 0,
            /// <summary>
            /// ASCII编码
            /// </summary>
            [EnumDisplay("ASCII编码")]
            ASCII = 1,
            /// <summary>
            /// UTF8编码
            /// </summary>
            [EnumDisplay("UTF8编码")]
            UTF8 = 2,
            /// <summary>
            /// Unicode编码
            /// </summary>
            [EnumDisplay("Unicode编码")]
            Unicode = 3
        }
        #endregion
    }
}
