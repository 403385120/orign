using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using Shuyz.Framework.Mvvm;
using ATL.Common;

namespace XRayClient.BatteryCheckManager
{
    /// <summary>
    /// 电池X-Ray检测
    /// </summary>
    public class BatteryCheckManage : ObservableObject
    {

        private readonly string _tableName = "@zy_battery_check@";
        private readonly string _insertSql = "INSERT INTO {0}(BatteryBarCode,CreateTime,UserID,DetectionMode,FinalResult,ResultPath,StartTiem,EndTime, " +
                                            " A1_PhotographTime,A1_PhotographResult,A1_OriginalImagePath,A1_ResultImagePath,A1_Status,A1_Voltage,A1_ElectricCurrent,A1_UsageTime,A1_DetectionLayer,A1_MarkingResult,A1_Min,A1_Max,A1_Distance,A1_Angle,A1_LayerNum, " +
                                            " A2_PhotographTime,A2_PhotographResult,A2_OriginalImagePath,A2_ResultImagePath,A2_Status,A2_Voltage,A2_ElectricCurrent,A2_UsageTime,A2_DetectionLayer,A2_MarkingResult,A2_Min,A2_Max,A2_Distance,A2_Angle,A2_LayerNum, " +
                                            " A3_PhotographTime,A3_PhotographResult,A3_OriginalImagePath,A3_ResultImagePath,A3_Status,A3_Voltage,A3_ElectricCurrent,A3_UsageTime,A3_DetectionLayer,A3_MarkingResult,A3_Min,A3_Max,A3_Distance,A3_Angle,A3_LayerNum, " +
                                            " A4_PhotographTime,A4_PhotographResult,A4_OriginalImagePath,A4_ResultImagePath,A4_Status,A4_Voltage,A4_ElectricCurrent,A4_UsageTime,A4_DetectionLayer,A4_MarkingResult,A4_Min,A4_Max,A4_Distance,A4_Angle,A4_LayerNum, ResultCode, DetectionExtMode, STFImagePath" +
                                            " ) VALUES({1},{2},{3},{4},{5},{6},{7},{8}," +
                                            "{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19},{20},{21},{22},{23}," +
                                            "{24},{25},{26},{27},{28},{29},{30},{31},{32},{33},{34},{35},{36},{37},{38},"  +
                                            "{39},{40},{41},{42},{43},{44},{45},{46},{47},{48},{49},{50},{51},{52},{53}," +
                                            "{54},{55},{56},{57},{58},{59},{60},{61},{62},{63},{64},{65},{66},{67},{68},{69}, {70}, {71});";  //68字段
        private readonly string _selectSql = "SELECT RecordID,BatteryBarCode,CreateTime,UserID,DetectionMode,FinalResult,ResultPath,StartTiem,EndTime,A1_PhotographTime,A1_PhotographResult,A1_OriginalImagePath,A1_ResultImagePath,A1_Status,A1_Voltage,A1_ElectricCurrent,A1_UsageTime,A1_DetectionLayer,A1_MarkingResult,A1_Min,A1_Max,A1_Distance,A1_Angle,A1_LayerNum, "+
                                            " A2_PhotographTime,A2_PhotographResult,A2_OriginalImagePath,A2_ResultImagePath,A2_Status,A2_Voltage,A2_ElectricCurrent,A2_UsageTime,A2_DetectionLayer,A2_MarkingResult,A2_Min,A2_Max,A2_Distance,A2_Angle,A2_LayerNum, " +
                                            " A3_PhotographTime,A3_PhotographResult,A3_OriginalImagePath,A3_ResultImagePath,A3_Status,A3_Voltage,A3_ElectricCurrent,A3_UsageTime,A3_DetectionLayer,A3_MarkingResult,A3_Min,A3_Max,A3_Distance,A3_Angle,A3_LayerNum, " +
                                            " A4_PhotographTime,A4_PhotographResult,A4_OriginalImagePath,A4_ResultImagePath,A4_Status,A4_Voltage,A4_ElectricCurrent,A4_UsageTime,A4_DetectionLayer,A4_MarkingResult,A4_Min,A4_Max,A4_Distance,A4_Angle,A4_LayerNum, ResultCode,DetectionExtMode,IFNULL(STFImagePath,'')," +
                                            " IFNULL(RecheckTime, CAST('1988-12-25 00:00:00' AS DATETIME)), IFNULL(RecheckUserID,''),IFNULL(FQATime, CAST('1988-12-25 00:00:00' AS DATETIME)),IFNULL(FQAUser, ''), "+
                                            " RecheckState FROM {0} ORDER BY RecordID Desc LIMIT 50;";
        private readonly string _cleanSQL = @"DELETE FROM {0} WHERE CreateTime < NOW() - INTERVAL {1} DAY";

        private readonly string _quickFindSQl = @"SELECT 
                                            BatteryBarCode, CreateTime, FinalResult, ResultCode
                                        FROM
                                            {0}
                                        WHERE
                                            DetectionMode = 0
                                                AND BatteryBarCode = {1}
                                                AND DetectionExtMode = 0
                                                AND CreateTime > NOW() - INTERVAL 10 DAY";

        public static BaseFacade baseFacade = new BaseFacade();
        private List<BatteryCheck> _batteryCheckList = new List<BatteryCheck>();
        private List<BatteryCheck> BatteryCheckList
        {
            get { return this._batteryCheckList; }
            set
            {
                this._batteryCheckList = value;
                RaisePropertyChanged("BatteryCheckList");
                //RaisePropertyChanged("PushRecordList_PushOnly");
                //RaisePropertyChanged("PushRecordList_UploadOnly");
            }
        }

        public List<BatteryCheck> _queryList = new List<BatteryCheck>();
        public List<BatteryCheck> QueryList
        {
            get { return this._queryList; }
            set
            {
                this._queryList = value;
                RaisePropertyChanged("QueryList");
            }
        }

        public void SetDbConfig()
        {
        }

        /// <summary>
        /// 快速查找10天内的某个条码
        /// </summary>
        /// <param name="barCode"></param>
        /// <param name="resultBat"></param>
        /// <returns></returns>
        public bool QuickFind(string barCode, ref BatteryCheck resultBat)
        {


            return true;
        }

        /// <summary>
        /// 添加电池检测
        /// </summary>
        /// <param name="PushRecord"></param>
        /// <returns></returns>
        public bool AddBatteryCheck(BatteryCheck batteryCheck)
        {
            string sql = "INSERT INTO production_data(`ProductSN`,`quality`,`NGreason`,`ProductDate`,`Operator`,`ResultPath`,`MesImagePath`,`CheckMode`,`Thickness`," +
                "`A1_PhotographResult`,`A1_OriginalImagePath`,`A1_ResultImagePath`,`A2_PhotographResult`,`A2_OriginalImagePath`,`A2_ResultImagePath`," +
                "`A3_PhotographResult`,`A3_OriginalImagePath`,`A3_ResultImagePath`,`A4_PhotographResult`,`A4_OriginalImagePath`,`A4_ResultImagePath`," +
                "`A1_Min`,`A1_Max`,`A2_Min`,`A2_Max`,`A3_Min`,`A3_Max`,`A4_Min`,`A4_Max`," +
                "`A1_Distance1`,`A1_Distance2`,`A1_Distance3`,`A1_Distance4`,`A1_Distance5`,`A1_Distance6`,`A1_Distance7`," +
                "`A1_Distance8`,`A1_Distance9`,`A1_Distance10`,`A1_Distance11`,`A1_Distance12`,`A1_Distance13`,`A1_Distance14`,`A1_Distance15`," +
                "`A1_Distance16`,`A1_Distance17`,`A1_Distance18`,`A1_Distance19`,`A1_Distance20`,`A1_Distance21`,`A1_Distance22`," +
                "`A1_Distance23`,`A1_Distance24`,`A1_Distance25`,`A1_Distance26`,`A1_Distance27`,`A1_Distance28`,`A1_Distance29`,`A1_Distance30`," +
                "`A1_Angle1`,`A1_Angle2`,`A1_Angle3`,`A1_Angle4`,`A1_Angle5`,`A1_Angle6`,`A1_Angle7`," +
                "`A1_Angle8`,`A1_Angle9`,`A1_Angle10`,`A1_Angle11`,`A1_Angle12`,`A1_Angle13`,`A1_Angle14`,`A1_Angle15`," +
                "`A1_Angle16`,`A1_Angle17`,`A1_Angle18`,`A1_Angle19`,`A1_Angle20`,`A1_Angle21`,`A1_Angle22`," +
                "`A1_Angle23`,`A1_Angle24`,`A1_Angle25`,`A1_Angle26`,`A1_Angle27`,`A1_Angle28`,`A1_Angle29`,`A1_Angle30`," +
                "`A2_Distance1`,`A2_Distance2`,`A2_Distance3`,`A2_Distance4`,`A2_Distance5`,`A2_Distance6`,`A2_Distance7`," +
                "`A2_Distance8`,`A2_Distance9`,`A2_Distance10`,`A2_Distance11`,`A2_Distance12`,`A2_Distance13`,`A2_Distance14`,`A2_Distance15`," +
                "`A2_Distance16`,`A2_Distance17`,`A2_Distance18`,`A2_Distance19`,`A2_Distance20`,`A2_Distance21`,`A2_Distance22`," +
                "`A2_Distance23`,`A2_Distance24`,`A2_Distance25`,`A2_Distance26`,`A2_Distance27`,`A2_Distance28`,`A2_Distance29`,`A2_Distance30`," +
                "`A2_Angle1`,`A2_Angle2`,`A2_Angle3`,`A2_Angle4`,`A2_Angle5`,`A2_Angle6`,`A2_Angle7`," +
                "`A2_Angle8`,`A2_Angle9`,`A2_Angle10`,`A2_Angle11`,`A2_Angle12`,`A2_Angle13`,`A2_Angle14`,`A2_Angle15`," +
                "`A2_Angle16`,`A2_Angle17`,`A2_Angle18`,`A2_Angle19`,`A2_Angle20`,`A2_Angle21`,`A2_Angle22`," +
                "`A2_Angle23`,`A2_Angle24`,`A2_Angle25`,`A2_Angle26`,`A2_Angle27`,`A2_Angle28`,`A2_Angle29`,`A2_Angle30`," +
                "`A3_Distance1`,`A3_Distance2`,`A3_Distance3`,`A3_Distance4`,`A3_Distance5`,`A3_Distance6`,`A3_Distance7`," +
                "`A3_Distance8`,`A3_Distance9`,`A3_Distance10`,`A3_Distance11`,`A3_Distance12`,`A3_Distance13`,`A3_Distance14`,`A3_Distance15`," +
                "`A3_Distance16`,`A3_Distance17`,`A3_Distance18`,`A3_Distance19`,`A3_Distance20`,`A3_Distance21`,`A3_Distance22`," +
                "`A3_Distance23`,`A3_Distance24`,`A3_Distance25`,`A3_Distance26`,`A3_Distance27`,`A3_Distance28`,`A3_Distance29`,`A3_Distance30`," +
                "`A3_Angle1`,`A3_Angle2`,`A3_Angle3`,`A3_Angle4`,`A3_Angle5`,`A3_Angle6`,`A3_Angle7`," +
                "`A3_Angle8`,`A3_Angle9`,`A3_Angle10`,`A3_Angle11`,`A3_Angle12`,`A3_Angle13`,`A3_Angle14`,`A3_Angle15`," +
                "`A3_Angle16`,`A3_Angle17`,`A3_Angle18`,`A3_Angle19`,`A3_Angle20`,`A3_Angle21`,`A3_Angle22`," +
                "`A3_Angle23`,`A3_Angle24`,`A3_Angle25`,`A3_Angle26`,`A3_Angle27`,`A3_Angle28`,`A3_Angle29`,`A3_Angle30`," +
                "`A4_Distance1`,`A4_Distance2`,`A4_Distance3`,`A4_Distance4`,`A4_Distance5`,`A4_Distance6`,`A4_Distance7`," +
                "`A4_Distance8`,`A4_Distance9`,`A4_Distance10`,`A4_Distance11`,`A4_Distance12`,`A4_Distance13`,`A4_Distance14`,`A4_Distance15`," +
                "`A4_Distance16`,`A4_Distance17`,`A4_Distance18`,`A4_Distance19`,`A4_Distance20`,`A4_Distance21`,`A4_Distance22`," +
                "`A4_Distance23`,`A4_Distance24`,`A4_Distance25`,`A4_Distance26`,`A4_Distance27`,`A4_Distance28`,`A4_Distance29`,`A4_Distance30`," +
                "`A4_Angle1`,`A4_Angle2`,`A4_Angle3`,`A4_Angle4`,`A4_Angle5`,`A4_Angle6`,`A4_Angle7`," +
                "`A4_Angle8`,`A4_Angle9`,`A4_Angle10`,`A4_Angle11`,`A4_Angle12`,`A4_Angle13`,`A4_Angle14`,`A4_Angle15`," +
                "`A4_Angle16`,`A4_Angle17`,`A4_Angle18`,`A4_Angle19`,`A4_Angle20`,`A4_Angle21`,`A4_Angle22`," +
                "`A4_Angle23`,`A4_Angle24`,`A4_Angle25`,`A4_Angle26`,`A4_Angle27`,`A4_Angle28`,`A4_Angle29`,`A4_Angle30`" +
                $" ) VALUES('{batteryCheck.ProductSN}','{batteryCheck.Quality}','{batteryCheck.NGreason}','{batteryCheck.ProductDate}','{batteryCheck.Operator}','{batteryCheck.ResultPath}','{batteryCheck.MesImagePath}','{batteryCheck.CheckMode}','{batteryCheck.Thickness}'," +
                $"'{batteryCheck.A1_PhotographResult}','{batteryCheck.A1_OriginalImagePath}','{batteryCheck.A1_ResultImagePath}','{batteryCheck.A2_PhotographResult}','{batteryCheck.A2_OriginalImagePath}','{batteryCheck.A2_ResultImagePath}'," +
                $"'{batteryCheck.A3_PhotographResult}','{batteryCheck.A3_OriginalImagePath}','{batteryCheck.A3_ResultImagePath}','{batteryCheck.A4_PhotographResult}','{batteryCheck.A4_OriginalImagePath}','{batteryCheck.A4_ResultImagePath}'," +
                $"'{batteryCheck.A1_Min}','{batteryCheck.A1_Max}','{batteryCheck.A2_Min}','{batteryCheck.A2_Max}','{batteryCheck.A3_Min}','{batteryCheck.A3_Max}','{batteryCheck.A4_Min}','{batteryCheck.A4_Max}'," +
                $"'{batteryCheck.A1_Distance1}','{batteryCheck.A1_Distance2}','{batteryCheck.A1_Distance3}','{batteryCheck.A1_Distance4}','{batteryCheck.A1_Distance5}','{batteryCheck.A1_Distance6}','{batteryCheck.A1_Distance7}'," +
                $"'{batteryCheck.A1_Distance8}','{batteryCheck.A1_Distance9}','{batteryCheck.A1_Distance10}','{batteryCheck.A1_Distance11}','{batteryCheck.A1_Distance12}','{batteryCheck.A1_Distance13}','{batteryCheck.A1_Distance14}','{batteryCheck.A1_Distance15}'," +
                $"'{batteryCheck.A1_Distance16}','{batteryCheck.A1_Distance17}','{batteryCheck.A1_Distance18}','{batteryCheck.A1_Distance19}','{batteryCheck.A1_Distance20}','{batteryCheck.A1_Distance21}','{batteryCheck.A1_Distance22}'," +
                $"'{batteryCheck.A1_Distance23}','{batteryCheck.A1_Distance24}','{batteryCheck.A1_Distance25}','{batteryCheck.A1_Distance26}','{batteryCheck.A1_Distance27}','{batteryCheck.A1_Distance28}','{batteryCheck.A1_Distance29}','{batteryCheck.A1_Distance30}'," +
                $"'{batteryCheck.A1_Angle1}','{batteryCheck.A1_Angle2}','{batteryCheck.A1_Angle3}','{batteryCheck.A1_Angle4}','{batteryCheck.A1_Angle5}','{batteryCheck.A1_Angle6}','{batteryCheck.A1_Angle7}'," +
                $"'{batteryCheck.A1_Angle8}','{batteryCheck.A1_Angle9}','{batteryCheck.A1_Angle10}','{batteryCheck.A1_Angle11}','{batteryCheck.A1_Angle12}','{batteryCheck.A1_Angle13}','{batteryCheck.A1_Angle14}','{batteryCheck.A1_Angle15}'," +
                $"'{batteryCheck.A1_Angle16}','{batteryCheck.A1_Angle17}','{batteryCheck.A1_Angle18}','{batteryCheck.A1_Angle19}','{batteryCheck.A1_Angle20}','{batteryCheck.A1_Angle21}','{batteryCheck.A1_Angle22}'," +
                $"'{batteryCheck.A1_Angle23}','{batteryCheck.A1_Angle24}','{batteryCheck.A1_Angle25}','{batteryCheck.A1_Angle26}','{batteryCheck.A1_Angle27}','{batteryCheck.A1_Angle28}','{batteryCheck.A1_Angle29}','{batteryCheck.A1_Angle30}'," +
                $"'{batteryCheck.A2_Distance1}','{batteryCheck.A2_Distance2}','{batteryCheck.A2_Distance3}','{batteryCheck.A2_Distance4}','{batteryCheck.A2_Distance5}','{batteryCheck.A2_Distance6}','{batteryCheck.A2_Distance7}'," +
                $"'{batteryCheck.A2_Distance8}','{batteryCheck.A2_Distance9}','{batteryCheck.A2_Distance10}','{batteryCheck.A2_Distance11}','{batteryCheck.A2_Distance12}','{batteryCheck.A2_Distance13}','{batteryCheck.A2_Distance14}','{batteryCheck.A2_Distance15}'," +
                $"'{batteryCheck.A2_Distance16}','{batteryCheck.A2_Distance17}','{batteryCheck.A2_Distance18}','{batteryCheck.A2_Distance19}','{batteryCheck.A2_Distance20}','{batteryCheck.A2_Distance21}','{batteryCheck.A2_Distance22}'," +
                $"'{batteryCheck.A2_Distance23}','{batteryCheck.A2_Distance24}','{batteryCheck.A2_Distance25}','{batteryCheck.A2_Distance26}','{batteryCheck.A2_Distance27}','{batteryCheck.A2_Distance28}','{batteryCheck.A2_Distance29}','{batteryCheck.A2_Distance30}'," +
                $"'{batteryCheck.A2_Angle1}','{batteryCheck.A2_Angle2}','{batteryCheck.A2_Angle3}','{batteryCheck.A2_Angle4}','{batteryCheck.A2_Angle5}','{batteryCheck.A2_Angle6}','{batteryCheck.A2_Angle7}'," +
                $"'{batteryCheck.A2_Angle8}','{batteryCheck.A2_Angle9}','{batteryCheck.A2_Angle10}','{batteryCheck.A2_Angle11}','{batteryCheck.A2_Angle12}','{batteryCheck.A2_Angle13}','{batteryCheck.A2_Angle14}','{batteryCheck.A2_Angle15}'," +
                $"'{batteryCheck.A2_Angle16}','{batteryCheck.A2_Angle17}','{batteryCheck.A2_Angle18}','{batteryCheck.A2_Angle19}','{batteryCheck.A2_Angle20}','{batteryCheck.A2_Angle21}','{batteryCheck.A2_Angle22}'," +
                $"'{batteryCheck.A2_Angle23}','{batteryCheck.A2_Angle24}','{batteryCheck.A2_Angle25}','{batteryCheck.A2_Angle26}','{batteryCheck.A2_Angle27}','{batteryCheck.A2_Angle28}','{batteryCheck.A2_Angle29}','{batteryCheck.A2_Angle30}'," +
                $"'{batteryCheck.A3_Distance1}','{batteryCheck.A3_Distance2}','{batteryCheck.A3_Distance3}','{batteryCheck.A3_Distance4}','{batteryCheck.A3_Distance5}','{batteryCheck.A3_Distance6}','{batteryCheck.A3_Distance7}'," +
                $"'{batteryCheck.A3_Distance8}','{batteryCheck.A3_Distance9}','{batteryCheck.A3_Distance10}','{batteryCheck.A3_Distance11}','{batteryCheck.A3_Distance12}','{batteryCheck.A3_Distance13}','{batteryCheck.A3_Distance14}','{batteryCheck.A3_Distance15}'," +
                $"'{batteryCheck.A3_Distance16}','{batteryCheck.A3_Distance17}','{batteryCheck.A3_Distance18}','{batteryCheck.A3_Distance19}','{batteryCheck.A3_Distance20}','{batteryCheck.A3_Distance21}','{batteryCheck.A3_Distance22}'," +
                $"'{batteryCheck.A3_Distance23}','{batteryCheck.A3_Distance24}','{batteryCheck.A3_Distance25}','{batteryCheck.A3_Distance26}','{batteryCheck.A3_Distance27}','{batteryCheck.A3_Distance28}','{batteryCheck.A3_Distance29}','{batteryCheck.A3_Distance30}'," +
                $"'{batteryCheck.A3_Angle1}','{batteryCheck.A3_Angle2}','{batteryCheck.A3_Angle3}','{batteryCheck.A3_Angle4}','{batteryCheck.A3_Angle5}','{batteryCheck.A3_Angle6}','{batteryCheck.A3_Angle7}'," +
                $"'{batteryCheck.A3_Angle8}','{batteryCheck.A3_Angle9}','{batteryCheck.A3_Angle10}','{batteryCheck.A3_Angle11}','{batteryCheck.A3_Angle12}','{batteryCheck.A3_Angle13}','{batteryCheck.A3_Angle14}','{batteryCheck.A3_Angle15}'," +
                $"'{batteryCheck.A3_Angle16}','{batteryCheck.A3_Angle17}','{batteryCheck.A3_Angle18}','{batteryCheck.A3_Angle19}','{batteryCheck.A3_Angle20}','{batteryCheck.A3_Angle21}','{batteryCheck.A3_Angle22}'," +
                $"'{batteryCheck.A3_Angle23}','{batteryCheck.A3_Angle24}','{batteryCheck.A3_Angle25}','{batteryCheck.A3_Angle26}','{batteryCheck.A3_Angle27}','{batteryCheck.A3_Angle28}','{batteryCheck.A3_Angle29}','{batteryCheck.A3_Angle30}'," +
                $"'{batteryCheck.A4_Distance1}','{batteryCheck.A4_Distance2}','{batteryCheck.A4_Distance3}','{batteryCheck.A4_Distance4}','{batteryCheck.A4_Distance5}','{batteryCheck.A4_Distance6}','{batteryCheck.A4_Distance7}'," +
                $"'{batteryCheck.A4_Distance8}','{batteryCheck.A4_Distance9}','{batteryCheck.A4_Distance10}','{batteryCheck.A4_Distance11}','{batteryCheck.A4_Distance12}','{batteryCheck.A4_Distance13}','{batteryCheck.A4_Distance14}','{batteryCheck.A4_Distance15}'," +
                $"'{batteryCheck.A4_Distance16}','{batteryCheck.A4_Distance17}','{batteryCheck.A4_Distance18}','{batteryCheck.A4_Distance19}','{batteryCheck.A4_Distance20}','{batteryCheck.A4_Distance21}','{batteryCheck.A4_Distance22}'," +
                $"'{batteryCheck.A4_Distance23}','{batteryCheck.A4_Distance24}','{batteryCheck.A4_Distance25}','{batteryCheck.A4_Distance26}','{batteryCheck.A4_Distance27}','{batteryCheck.A4_Distance28}','{batteryCheck.A4_Distance29}','{batteryCheck.A4_Distance30}'," +
                $"'{batteryCheck.A4_Angle1}','{batteryCheck.A4_Angle2}','{batteryCheck.A4_Angle3}','{batteryCheck.A4_Angle4}','{batteryCheck.A4_Angle5}','{batteryCheck.A4_Angle6}','{batteryCheck.A4_Angle7}'," +
                $"'{batteryCheck.A4_Angle8}','{batteryCheck.A4_Angle9}','{batteryCheck.A4_Angle10}','{batteryCheck.A4_Angle11}','{batteryCheck.A4_Angle12}','{batteryCheck.A4_Angle13}','{batteryCheck.A4_Angle14}','{batteryCheck.A4_Angle15}'," +
                $"'{batteryCheck.A4_Angle16}','{batteryCheck.A4_Angle17}','{batteryCheck.A4_Angle18}','{batteryCheck.A4_Angle19}','{batteryCheck.A4_Angle20}','{batteryCheck.A4_Angle21}','{batteryCheck.A4_Angle22}'," +
                $"'{batteryCheck.A4_Angle23}','{batteryCheck.A4_Angle24}','{batteryCheck.A4_Angle25}','{batteryCheck.A4_Angle26}','{batteryCheck.A4_Angle27}','{batteryCheck.A4_Angle28}','{batteryCheck.A4_Angle29}','{batteryCheck.A4_Angle30}');";

sql = sql.Replace("\\","\\\\");

            bool success = false;
            string exceptionMsg = string.Empty;
            try
            {
                baseFacade.equipDB.ExecuteNonQuery(CommandType.Text, sql);
                success = true;
            }
            catch (Exception _ex)
            {
                exceptionMsg = _ex.ToString();
            }
            if (!success) LogHelper.Error(exceptionMsg);

            return true;
        }

        /// <summary>
        /// 刷新电池检测信息
        /// 兼供查询
        /// </summary>
        /// <returns></returns>
        public bool RefreshBatteryCheckList(DateTime startTime, DateTime endTime, string barCode, bool isQuery = false)
        {


            return true;
        }

        public bool OutResult(string filename)
        {
            List<string> plist = new List<string>();

            if (CheckRecordExportConfig.Instance.NeedSaveRecordID)
            {
                plist.Add("RecordID");
            }
            if (CheckRecordExportConfig.Instance.NeedSaveBatteryBarCode)
            {
                plist.Add("BatteryBarCode");
            }
            if (CheckRecordExportConfig.Instance.NeedSaveCreateTime)
            {
                plist.Add("CreateTime");
            }
            if (CheckRecordExportConfig.Instance.NeedSaveUserID)
            {
                plist.Add("UserID");
            }
            if (CheckRecordExportConfig.Instance.NeedSaveDetectionMode)
            {
                plist.Add("DetectionMode");
            }
            if (CheckRecordExportConfig.Instance.NeedSaveDetectExtMode)
            {
                plist.Add("DetectExtMode");
            }
            if (CheckRecordExportConfig.Instance.NeedSaveFinalResult)
            {
                plist.Add("FinalResult");
            }
            if (CheckRecordExportConfig.Instance.NeedSaveResultCode)
            {
                plist.Add("ResultCode");
            }
            if (CheckRecordExportConfig.Instance.NeedSaveResultPath)
            {
                plist.Add("ResultPath");
            }
            if (CheckRecordExportConfig.Instance.NeedSaveStartTime)
            {
                plist.Add("StartTime");
            }
            if (CheckRecordExportConfig.Instance.NeedSaveEndTime)
            {
                plist.Add("EndTime");
            }
            

            for (int i = 1; i < 5; i++)
            {
                if (CheckRecordExportConfig.Instance.NeedSavePhotographTime)
                {
                    plist.Add("A"+i.ToString()+"_PhotographTime");
                }
                if (CheckRecordExportConfig.Instance.NeedSavePhotographResult)
                {
                    plist.Add("A" + i.ToString() + "_PhotographResult");
                }
                if (CheckRecordExportConfig.Instance.NeedSaveOriginalImagePath)
                {
                    plist.Add("A" + i.ToString() + "_OriginalImagePath");
                }
                if (CheckRecordExportConfig.Instance.NeedSaveResultImagePath)
                {
                    plist.Add("A" + i.ToString() + "_ResultImagePath");
                }
                if (CheckRecordExportConfig.Instance.NeedSaveStatus)
                {
                    plist.Add("A" + i.ToString() + "_Status");
                }
                if (CheckRecordExportConfig.Instance.NeedSaveVoltage)
                {
                    plist.Add("A" + i.ToString() + "_Voltage");
                }
                if (CheckRecordExportConfig.Instance.NeedSaveElectricCurrent)
                {
                    plist.Add("A" + i.ToString() + "_ElectricCurrent");
                }
                if (CheckRecordExportConfig.Instance.NeedSaveUsageTime)
                {
                    plist.Add("A" + i.ToString() + "_UsageTime");
                }
                if (CheckRecordExportConfig.Instance.NeedSaveDetectionLayer)
                {
                    plist.Add("A" + i.ToString() + "_DetectionLayer");
                }
                if (CheckRecordExportConfig.Instance.NeedSaveMarkingResult)
                {
                    plist.Add("A" + i.ToString() + "_MarkingResult");
                }
                if (CheckRecordExportConfig.Instance.NeedSaveMin)
                {
                    plist.Add("A" + i.ToString() + "_Min");
                }
                if (CheckRecordExportConfig.Instance.NeedSaveMax)
                {
                    plist.Add("A" + i.ToString() + "_Max");
                }
                if (CheckRecordExportConfig.Instance.NeedSaveDistance)
                {
                    string ldtemp = "A"+i.ToString()+"_Distance_";
                    for (int n = 0; n < 35; n++)   //15改成35
                    {
                        plist.Add(ldtemp + (n + 1).ToString());
                    }
                }
                if (CheckRecordExportConfig.Instance.NeedSaveAngle)
                {
                    string ldtemp2 = "A"+i.ToString()+"_Angle_";
                    for (int n = 0; n < 35; n++)  //15改成35
                    {
                        plist.Add(ldtemp2 + (n + 1).ToString());
                    }
                }
                //if (CheckRecordExportConfig.Instance.NeedSaveLayerNum)
                //{
                //    plist.Add("A" + i.ToString() + "_LayerNum");
                //}
            }

            if (CheckRecordExportConfig.Instance.NeedSaveRecheckState)
            {
                plist.Add("RecheckState");
            }
            if (CheckRecordExportConfig.Instance.NeedSaveRecheckTime)
            {
                plist.Add("RecheckTime");
            }
            if (CheckRecordExportConfig.Instance.NeedSaveRecheckUserID)
            {
                plist.Add("RecheckUserID");
            }
            if (CheckRecordExportConfig.Instance.NeedSaveFAQTime)
            {
                plist.Add("FQATime");
            }
            if (CheckRecordExportConfig.Instance.NeedSaveFQAUser)
            {
                plist.Add("FQAUser");
            }                           

            try
            {
                System.IO.FileStream fs = new System.IO.FileStream(filename, System.IO.FileMode.Create, System.IO.FileAccess.Write);
                System.IO.StreamWriter sw = new System.IO.StreamWriter(fs, System.Text.Encoding.UTF8);

                sw.WriteLine(WriteTitleToFile(plist));
                plist.Clear();

                for (int n = 0; n < QueryList.Count; n++)
                {
                    plist.Clear();
                    if (CheckRecordExportConfig.Instance.NeedSaveRecordID)
                    {
                        plist.Add(QueryList[n].Iden.ToString());
                    }

                    if (CheckRecordExportConfig.Instance.NeedSaveBatteryBarCode)
                    {
                        plist.Add(QueryList[n].ProductSN.ToString());
                    }
                    if (CheckRecordExportConfig.Instance.NeedSaveCreateTime)
                    {
                        plist.Add(QueryList[n].ProductDate.ToString("yyyy-MM-dd HH:mm:ss"));
                    }
                    if (CheckRecordExportConfig.Instance.NeedSaveUserID)
                    {
                        plist.Add(QueryList[n].Operator.ToString());
                    }
                    if (CheckRecordExportConfig.Instance.NeedSaveFinalResult)
                    {
                        plist.Add(QueryList[n].Quality.ToString());
                    }
                    if (CheckRecordExportConfig.Instance.NeedSaveResultCode)
                    {
                        plist.Add(QueryList[n].NGreason.ToString());
                    }
                    if (CheckRecordExportConfig.Instance.NeedSaveResultPath)
                    {
                        plist.Add(QueryList[n].ResultPath.ToString());
                    }
                    if (CheckRecordExportConfig.Instance.NeedSaveStartTime)
                    {
                        plist.Add(QueryList[n].OutTime.ToString("yyyy-MM-dd HH:mm:ss"));
                    }

                    for (int i = 1; i < 5; i++)
                    {
                        if (CheckRecordExportConfig.Instance.NeedSavePhotographResult)
                        {
                            plist.Add(GetPropValue(QueryList[n], "A" + i.ToString() + "_PhotographResult").ToString());
                        }
                        if (CheckRecordExportConfig.Instance.NeedSaveOriginalImagePath)
                        {
                            plist.Add(GetPropValue(QueryList[n], "A" + i.ToString() + "_OriginalImagePath").ToString());                          
                        }
                        if (CheckRecordExportConfig.Instance.NeedSaveResultImagePath)
                        {
                            plist.Add(GetPropValue(QueryList[n], "A" + i.ToString() + "_ResultImagePath").ToString());
                        }
                        if (CheckRecordExportConfig.Instance.NeedSaveMin)
                        {
                            plist.Add(((float)GetPropValue(QueryList[n], "A" + i.ToString() + "_Min")).ToString("N3"));
                        }
                        if (CheckRecordExportConfig.Instance.NeedSaveMax)
                        {
                            plist.Add(((float)GetPropValue(QueryList[n], "A" + i.ToString() + "_Max")).ToString("N3"));
                        }
                        if (CheckRecordExportConfig.Instance.NeedSaveDistance)
                        {
                            string[] ldtemplist = (GetPropValue(QueryList[n], "A" + i.ToString() + "_Distance").ToString()).Split('#');
                               
                            for (int m = 0; m < ldtemplist.Count(); m++)
                            {
                                plist.Add(ldtemplist[m]);
                            }

                            //add by fjy 2018.08.24
                            //int length = 15 - ldtemplist.Count();   //15层电池数据
                            int length = 35 - ldtemplist.Count();     //35层电池数据

                            for (int m = 0; m < length; m++)
                            {
                                plist.Add("0");
                            }
                        }
                        if (CheckRecordExportConfig.Instance.NeedSaveAngle)
                        {
                            string[] ldtemplistangle = (GetPropValue(QueryList[n], "A" + i.ToString() + "_Angle").ToString()).Split('#');

                            for (int m = 0; m < ldtemplistangle.Count(); m++)
                            {
                                plist.Add(ldtemplistangle[m]);
                            }

                            //add by fjy 2018.08.24
                            // int length = 15 - ldtemplistangle.Count();  //15层电池数据
                            int length = 35 - ldtemplistangle.Count();   //35层电池数据

                            for (int m = 0; m < length; m++)
                            {
                                plist.Add("0");
                            }
                        }
                    }

                    if (CheckRecordExportConfig.Instance.NeedSaveRecheckState)
                    {
                        if (string.IsNullOrEmpty(QueryList[n].RecheckState.ToString()))
                            plist.Add(" - ");
                        else
                            plist.Add(QueryList[n].RecheckState.ToString());
                    }
                    if (CheckRecordExportConfig.Instance.NeedSaveRecheckTime)
                    {
                        if (QueryList[n].RecheckTime.ToString("yyyy-MM-dd") == "1988-12-25")
                            plist.Add(" - ");
                        else
                            plist.Add(QueryList[n].RecheckTime.ToString("yyyy-MM-dd HH:mm:ss"));
                    }
                    if (CheckRecordExportConfig.Instance.NeedSaveRecheckUserID)
                    {
                        if (string.IsNullOrEmpty(QueryList[n].RecheckUserID.ToString()))
                            plist.Add(" - ");
                        else
                            plist.Add(QueryList[n].RecheckUserID.ToString());
                    }
                    if (CheckRecordExportConfig.Instance.NeedSaveFAQTime)
                    {
                        if (QueryList[n].FQATime.ToString("yyyy-MM-dd") == "1988-12-25")
                            plist.Add(" - ");
                        else
                            plist.Add(QueryList[n].FQATime.ToString("yyyy-MM-dd HH:mm:ss"));
                    }
                    if (CheckRecordExportConfig.Instance.NeedSaveFQAUser)
                    {
                        if (string.IsNullOrEmpty(QueryList[n].FQAUser.ToString()))
                            plist.Add(" - ");
                        else
                            plist.Add(QueryList[n].FQAUser.ToString());
                    }
                 
                    sw.WriteLine(WriteTitleToFile(plist));
                }

                sw.Close();
                fs.Close();
            }
            catch(Exception e)
            {
                throw e;
            }
            

            return true;
        }

        static public string WriteTitleToFile(List<string> filedata)
        {
            string ldtempwrite = string.Empty;
            for(int n = 0; n < filedata.Count; n++)
            {
                ldtempwrite = ldtempwrite + filedata[n];
                ldtempwrite = ldtempwrite + ",";
            }

            return ldtempwrite;
        }
        // Get property value from string using reflection
        static object GetPropValue(object obj, string propName)
        {
            return obj.GetType().GetProperty(propName).GetValue(obj, null);
        }
        /// <summary>
        /// 初始化，建表
        /// </summary>
        public void Init()
        {
            bool result = this.CheckAndCreateTable();

            this.RefreshBatteryCheckList(DateTime.Now, DateTime.Now, string.Empty, false);
        }

        public void UnInit()
        {
            // do nothing
        }

        public bool CheckAndCreateTable(bool isNew = false)
        {

            return true;
        }

        //BatteryCheckManage Data Clear
        private void Clean()
        {
        }

    }
}
