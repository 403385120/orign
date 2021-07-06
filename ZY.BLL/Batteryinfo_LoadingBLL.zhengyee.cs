using Dapper;
using Esquel.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZY.Model;
using ZY.Systems;

namespace ZY.BLL
{
    /// <summary>
    /// BatteryDataBLL    
    /// </summary>
    public partial class Batteryinfo_LoadingBLL
    {

        public static List<BatteryData> GetTotalBatteryDataLoad(string fitler, ref string errMsg, int pageSize = 1000, int pageIndex = 0, bool isHistory = false, string hisDb = "")
        {
            errMsg = string.Empty;
            StringBuilder selSql = new StringBuilder();
            selSql.AppendFormat(@"  SELECT  
               a.Iden,a.MachineNo,a.BatteryNo,a.ProductType,a.Barcode,a.Flag,a.CreateDate,a.Horn,a.Algorithm,a.Min,a.Max,a.L_1,a.L_2,a.L_3,a.L_4
               ,a.L_5,a.L_6,a.L_7,a.L_8,a.L_9,a.L_10,a.L_11,a.L_12,a.L_13,a.L_14,a.L_15,a.L_16,a.L_17,a.L_18,a.L_19,a.L_20,a.L_21,a.L_22,a.L_23
               ,a.L_24,a.L_25,a.L_26,a.L_27,a.L_28,a.L_29,a.L_30,a.ANG_1,a.ANG_2,a.ANG_3,a.ANG_4,a.ANG_5,a.ANG_6,a.ANG_7,a.ANG_8,a.ANG_9,a.ANG_10
               ,a.ANG_11,a.ANG_12,a.ANG_13,a.ANG_14,a.ANG_15,a.ANG_16,a.ANG_17,a.ANG_18,a.ANG_19,a.ANG_20,a.ANG_21,a.ANG_22,a.ANG_23,a.ANG_24
               ,a.ANG_25,a.ANG_26,a.ANG_27,a.ANG_28,a.ANG_29,a.ANG_30,a.isUpload,a.UploadReason,a.UploadDate,a.EmployeeID 
                FROM Batteryinfo_Loading a {0}
                WHERE {1}", Common.SqlLockKey, fitler);

            selSql.Append(" Order by a.Iden Desc ");

            try
            {
                var helper = DBHelperFactory.CreateHelperFactory(Common.ConnectionString);
                var list = helper.SelectReader<BatteryData>(selSql.ToString());

                return list.ToList();

            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
            }
            return new List<BatteryData>();
        }

        ///// <summary>
        ///// 绑盘数据查询
        ///// </summary>
        ///// <param name="fitler"></param>
        ///// <param name="pageSize"></param>
        ///// <param name="pageIndex"></param>
        ///// <param name="errMsg"></param>
        ///// <returns></returns>
        //public static IList<BatteryData> GetPalletData(string fitler, int pageSize, int pageIndex, ref string errMsg)
        //{
        //    errMsg = string.Empty;
        //    string selCmd = string.Empty;

        //    selCmd = string.Format(@"
        //                      SELECT DISTINCT UnPalletBarCode as UnPalletBarCode_UnLoading ,ProductLineNO ,EquipmentNo
        //                           , UnPalletDate_UnLoading=UnPalletDate, IsUpload_Pallet, UploadDate_Pallet,UploadNum_Pallet, UploadLog_Pallet
        //                      FROM BatteryInfo_UnLoading  {0}
        //                     WHERE {1}(UnPalletBarCode,'')<>'' and {2} Order by UnPalletDate DESC"
        //                 , Common.SqlLockKey, Common.SqlIsNullKey, fitler);

        //    try
        //    {
        //        DapperHelper dhelper = new DapperHelper();
        //        using (var con = dhelper.SQLConnection(Common.ConnectionString))
        //        {
        //            var list = con.Query<BatteryData>(selCmd.ToString()).ToList();

        //            foreach (BatteryData _m in list)
        //            {
        //                selCmd = $@"
        //                             select BarCode,PalletNo FROM BatteryInfo_UnLoading {Common.SqlLockKey}
        //                             where UnPalletBarCode='{_m.UnPalletBarCode_UnLoading}' and {Common.SqlConvertKey("UnPalletDate", 2)}=
        //                             '{_m.UnPalletDate_UnLoading.GetValueOrDefault(DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss")}'
        //                             and {Common.SqlIsNullKey}(IsUpload_Pallet,0)={(Convert.ToBoolean(_m.IsUpload_Pallet) ? 1 : 0)}";

        //                var lstBarCode = con.Query<BatteryData>(selCmd).ToList();
        //                _m.lstCellsInfo = lstBarCode;
        //            }
        //            return list;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        errMsg = ex.Message;
        //    }
        //    return new List<BatteryData>();
        //}


        public static List<BatteryData> GetTotalBatteryData(string fitler, ref string errMsg, int pageSize = 1000, int pageIndex = 0, bool isHistory = false, string hisDb = "", bool isMulti = false, bool isPage = false)
        {
            errMsg = string.Empty;
            StringBuilder selSql = new StringBuilder();
            selSql.AppendFormat(@"  SELECT  
                            a.Iden,a.MachineNo,a.BatteryNo,a.ProductType,a.Barcode,a.Flag,a.CreateDate,a.Horn,a.Algorithm,a.Min,a.Max,a.L_1,a.L_2,a.L_3,a.L_4
               ,a.L_5,a.L_6,a.L_7,a.L_8,a.L_9,a.L_10,a.L_11,a.L_12,a.L_13,a.L_14,a.L_15,a.L_16,a.L_17,a.L_18,a.L_19,a.L_20,a.L_21,a.L_22,a.L_23
               ,a.L_24,a.L_25,a.L_26,a.L_27,a.L_28,a.L_29,a.L_30,a.ANG_1,a.ANG_2,a.ANG_3,a.ANG_4,a.ANG_5,a.ANG_6,a.ANG_7,a.ANG_8,a.ANG_9,a.ANG_10
               ,a.ANG_11,a.ANG_12,a.ANG_13,a.ANG_14,a.ANG_15,a.ANG_16,a.ANG_17,a.ANG_18,a.ANG_19,a.ANG_20,a.ANG_21,a.ANG_22,a.ANG_23,a.ANG_24
               ,a.ANG_25,a.ANG_26,a.ANG_27,a.ANG_28,a.ANG_29,a.ANG_30,a.isUpload,a.UploadReason,a.UploadDate,a.EmployeeID 
                FROM Batteryinfo_Loading a {0}
                WHERE {1}", Common.SqlLockKey, fitler);

            if (isHistory)
            {
                foreach (string strDb in hisDb.Split(','))
                {
                    if (!string.IsNullOrEmpty(strDb))
                    {
                        selSql.AppendFormat(@" union all select 
                             a.Iden,a.MachineNo,a.BatteryNo,a.ProductType,a.Barcode,a.Flag,a.CreateDate,a.Horn,a.Algorithm,a.Min,a.Max,a.L_1,a.L_2,a.L_3,a.L_4
               ,a.L_5,a.L_6,a.L_7,a.L_8,a.L_9,a.L_10,a.L_11,a.L_12,a.L_13,a.L_14,a.L_15,a.L_16,a.L_17,a.L_18,a.L_19,a.L_20,a.L_21,a.L_22,a.L_23
               ,a.L_24,a.L_25,a.L_26,a.L_27,a.L_28,a.L_29,a.L_30,a.ANG_1,a.ANG_2,a.ANG_3,a.ANG_4,a.ANG_5,a.ANG_6,a.ANG_7,a.ANG_8,a.ANG_9,a.ANG_10
               ,a.ANG_11,a.ANG_12,a.ANG_13,a.ANG_14,a.ANG_15,a.ANG_16,a.ANG_17,a.ANG_18,a.ANG_19,a.ANG_20,a.ANG_21,a.ANG_22,a.ANG_23,a.ANG_24
               ,a.ANG_25,a.ANG_26,a.ANG_27,a.ANG_28,a.ANG_29,a.ANG_30,a.isUpload,a.UploadReason,a.UploadDate,a.EmployeeID 
                       FROM {0}.{1}Batteryinfo_Loading a {2}
                       WHERE {3} ", strDb, Common.SqlDBKey, Common.SqlLockKey, fitler);
                    }
                }
            }

            StringBuilder selCmd = new StringBuilder();


            selSql.Append(" Order by Iden Desc ");

            selCmd.Append(selSql);
            if (pageSize > 0)
            {
                selCmd.AppendFormat(" LIMIT {0},{1}", pageSize * pageIndex, pageSize);
            }
            if (!isPage)
            {
                StringBuilder selCount = new StringBuilder();


                selCount.AppendFormat(@" SELECT cast(COUNT(1) as Decimal(12,0)) AS TotalCount FROM Batteryinfo_Loading a {0} ", Common.SqlLockKey);
                if (isMulti)
                {
                    selCount.AppendFormat(@"  left join BatteryInfo_UnLoading b {0} ON a.Iden = b.Iden_Loading ", Common.SqlLockKey);
                }
                selCount.AppendFormat(@"  WHERE {0} ", fitler);

                if (isHistory)
                {
                    foreach (string strDb in hisDb.Split(','))
                    {
                        if (!string.IsNullOrEmpty(strDb))
                        {
                            selCount.AppendFormat(" union all SELECT cast(COUNT(1) as Decimal(12,0)) AS TotalCount FROM {0}.{1}Batteryinfo_Loading a {2} "
                                                    , Common.SqlDBKey, Common.SqlLockKey, strDb);
                            if (isMulti)
                            {
                                selCount.AppendFormat(@" left join {0}.{1}BatteryInfo_UnLoading b {2} ON a.Iden = b.Iden_Loading ", Common.SqlDBKey, Common.SqlLockKey, strDb);
                            }

                            selCount.AppendFormat(@"  WHERE {0} ", fitler);

                        }
                    }
                    selCmd.AppendFormat(";select cast(SUM(TotalCount) as Decimal(12,0)) AS TotalCount from ( {0} ) a", selCount.ToString());

                }
                else
                {
                    selCmd.AppendFormat(";{0}", selCount.ToString());
                }
            }
            try
            {
                var helper = DBHelperFactory.CreateHelperFactory(Common.ConnectionString);
                var list = helper.SelectReader<BatteryData>(selCmd.ToString());

                return list.ToList();

            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
            }
            return new List<BatteryData>();
        }

        public static List<BatteryData> GetDegasUnLoadingData(string filter, ref string errMsg, int pageSize = 1000, int pageIndex = 0, bool isHistory = false, string hisDb = "")
        {
            errMsg = string.Empty;
            List<BatteryData> list = new List<BatteryData>();
            try
            {
                filter = string.IsNullOrEmpty(filter) ? " 1=1 " : filter;
                StringBuilder selCmd = new StringBuilder();
                selCmd.Append($"SELECT ");

                selCmd.AppendFormat(@"  
                    
                           b.Iden, b.Iden_Loading,b.CreateDate as CreateDate_UnLoading,b.BatteryNo as BatteryNo_UnLoading,b.Barcode as Barcode_UnLoading,
                           b.Define1,b.Define2,b.Define3,b.Define4,b.Define5,b.Define6,b.Define7,b.Define8,b.Define9,b.Define10,b.Define11,
                           b.Define12,b.Define13,b.Define14,b.Define15,b.Define16,b.Define17,b.Define18,b.Define19,b.Define20,b.Define21,b.Define22,
                           b.Define23,b.Define24,b.Define25,b.Define26,b.Define27,b.Define28,b.Define29,b.Define30,b.Define31,b.Define32,b.Define33,
                           b.Define34,b.Define35,b.Define36,b.Define37,b.Define38,b.Define39,b.Define40,b.Define41,b.Define42,b.Define43,b.Define44,
                           b.Define45,b.Define46,b.Define47,b.Define48,b.Define49,b.Define50,b.Define51,b.Define52,b.Define53,b.Define54,b.Define55,
                           b.Define56,b.Define57,b.Define58,b.Define59,b.Define60,b.Define61,b.Define62,b.Define63,b.Define64,b.Define65,b.Define66,
                           b.Define67,b.Define68,b.Define69,b.Define70,b.Define71,b.Define72,b.Define73,b.Define74,b.Define75,b.Define76,b.Define77,
                           b.Define78,b.Define79,b.Define80,b.Define1_Reason,b.Define2_Reason,b.Define3_Reason,b.Define4_Reason,b.Define5_Reason,
                           b.Define6_Reason,b.Define7_Reason,b.Define8_Reason,b.Define9_Reason,b.Define10_Reason

                FROM BatteryInfo_UnLoading b {0}
                  LEFT JOIN Batteryinfo_Loading a {1} ON a.BarCode = b.BarCode AND a.Iden = b.Iden_Loading
                WHERE {2} ", Common.SqlLockKey, Common.SqlLockKey, filter);


                if (isHistory)
                {
                    foreach (string strDb in hisDb.Split(','))
                    {
                        if (!string.IsNullOrEmpty(strDb))
                        {
                            selCmd.Append($" union all SELECT ");
                            selCmd.AppendFormat(@"        b.Iden, b.Iden_Loading,b.CreateDate as CreateDate_UnLoading,b.BatteryNo as BatteryNo_UnLoading,b.Barcode as Barcode_UnLoading,
                           b.Define1,b.Define2,b.Define3,b.Define4,b.Define5,b.Define6,b.Define7,b.Define8,b.Define9,b.Define10,b.Define11,
                           b.Define12,b.Define13,b.Define14,b.Define15,b.Define16,b.Define17,b.Define18,b.Define19,b.Define20,b.Define21,b.Define22,
                           b.Define23,b.Define24,b.Define25,b.Define26,b.Define27,b.Define28,b.Define29,b.Define30,b.Define31,b.Define32,b.Define33,
                           b.Define34,b.Define35,b.Define36,b.Define37,b.Define38,b.Define39,b.Define40,b.Define41,b.Define42,b.Define43,b.Define44,
                           b.Define45,b.Define46,b.Define47,b.Define48,b.Define49,b.Define50,b.Define51,b.Define52,b.Define53,b.Define54,b.Define55,
                           b.Define56,b.Define57,b.Define58,b.Define59,b.Define60,b.Define61,b.Define62,b.Define63,b.Define64,b.Define65,b.Define66,
                           b.Define67,b.Define68,b.Define69,b.Define70,b.Define71,b.Define72,b.Define73,b.Define74,b.Define75,b.Define76,b.Define77,
                           b.Define78,b.Define79,b.Define80,b.Define1_Reason,b.Define2_Reason,b.Define3_Reason,b.Define4_Reason,b.Define5_Reason,
                           b.Define6_Reason,b.Define7_Reason,b.Define8_Reason,b.Define9_Reason,b.Define10_Reason

                        FROM {0}.{1}BatteryInfo_UnLoading b {2} WHERE {3}", strDb, Common.SqlDBKey, Common.SqlLockKey, filter);

                        }
                    }
                }
                StringBuilder selSql = new StringBuilder();
                selCmd.Append(" Order by Iden Desc ");
                selCmd.Append(selSql);
                if (pageSize > 0)
                {
                    selCmd.AppendFormat(" LIMIT {0},{1}", pageSize * pageIndex, pageSize);
                }

                StringBuilder selCount = new StringBuilder();
                selCount.AppendFormat(@" SELECT COUNT(1) AS TotalCount FROM BatteryInfo_UnLoading b {0} WHERE {1} ", Common.SqlLockKey, filter);

                if (isHistory)
                {
                    foreach (string strDb in hisDb.Split(','))
                    {
                        if (!string.IsNullOrEmpty(strDb))
                        {
                            selCount.AppendFormat(" union all SELECT COUNT(1) AS TotalCount FROM {0}.{1}BatteryInfo_UnLoading b {2} WHERE {3}"
                                                    , strDb, Common.SqlDBKey, Common.SqlLockKey, filter);
                        }
                    }
                }
                selSql.AppendFormat(" )select SUM(TotalCount) AS TotalCount from ( {0} ) a", selCount.ToString());

                //  selSql.AppendFormat(";select SUM(TotalCount) AS TotalCount from ( {0} ) b", selCount.ToString());

                DapperHelper dhelper = new DapperHelper();
                using (var con = dhelper.SQLConnection(Common.ConnectionString))
                {

                    list = con.Query<BatteryData>(selSql.ToString()).ToList();
                }
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
            }
            return list;
        }
        /// <summary>
        /// 根据条码获取上料表数据，给单机下料生产时保存上料数据使用，之前返回数据MODEL为 BatteryData，由于下料表数据再次生成，如使用该MODEL，将会将之前的数据都清空，因为该查询获取不到下料表数据 ,此次改成 Batteryinfo_Loading by ljx
        /// </summary>
        /// <param name="fitler"></param>
        /// <param name="errMsg"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="isHistory"></param>
        /// <param name="hisDb"></param>
        /// <returns></returns>
        public static List<Batteryinfo_Loading> GetDeagsLoadingData(string fitler, ref string errMsg, int pageSize = 1000, int pageIndex = 0, bool isHistory = false, string hisDb = "")
        {
            if (string.IsNullOrEmpty(fitler))
                fitler = " 1=1";
            List<Batteryinfo_Loading> list = new List<Batteryinfo_Loading>();
            StringBuilder selSql = new StringBuilder();
            try
            {
                //  string selCmd = $@" select top 1 a.Iden, a.TranfIden, a.CreateDate, a.EquipmentNo, a.No, a.ProductType, a.ProductLineNo, a.Shift, a.EndProductNo, a.LoadPalletBarCode , a.LoadPalletDate, a.BarCode, a.BarCodeDate, a.ScanCodePass, a.ScanDefectCode, a.InjectWeight, a.InjectFlag, a.CavityPressure, a.CavityDuration, a.CavityNo, a.CavityUseCount, a.FineSealingNo, a.FineSealingUseCount, a.FineSealingTopSetTemp, a.FineSealingBottomSetTemp , a.FineSealingDuration, a.FineSealingPressure, a.FineSealingTopTemp, a.FineSealingBottomTemp, a.CutterLeftUseCount, a.CutterRightUseCount , a.BeforeWeighCode, a.BeforeWeight, a.BeforeFlag, a.BeforeDefectCode, a.Weigh_LSL, a.Weigh_USL, a.IsUnLoading, a.UnLoadingDate , a.UnLoadingReason, a.CreateUser, a.IsDelete, a.DeleteDate, a.DeleteUser, a.IsUpload, a.UploadDate, a.UploadState, a.UploadNum, a.UploadLog, a.Remark, a.CreateTime, a.CavityLeftTemp, a.CavityRightTemp, a.CavityLeftSetTemp, a.CavityRightSetTemp, a.CavityVacuumTemp , a.RollingHUseNum, a.RollingHSpeed, a.RollingHPressure, a.RollingVUseNum, a.RollingVSpeed, a.RollingVPressure, a.ThicknessPressure, a.ThicknessTemp, a.ThicknessDuration, a.ThicknessTest, a.ThicknessSetTest, a.ThicknessFlag, a.ThicknessPressure1, a.ThicknessTemp1 , a.ThicknessDuration1, a.ThicknessTest1, a.ThicknessSetTest1, a.ThicknessFlag1, a.ThicknessPressure2, a.ThicknessTemp2, a.ThicknessDuration2, a.ThicknessTest2, a.ThicknessSetTest2, a.ThicknessFlag2, a.ThicknessPressure3, a.ThicknessTemp3, a.ThicknessDuration3, a.ThicknessTest3, a.ThicknessSetTest3, a.ThicknessFlag3, a.ThicknessPressure4, a.ThicknessTemp4, a.ThicknessDuration4, a.ThicknessTest4, a.ThicknessSetTest4, a.ThicknessFlag4, a.ThicknessPressure5, a.ThicknessTemp5 , a.ThicknessDuration5, a.ThicknessTest5, a.ThicknessSetTest5, a.ThicknessFlag5, a.ThicknessTest1_1, a.ThicknessTest1_2 , a.ThicknessTest1_3, a.ThicknessTest2_1, a.ThicknessTest2_2, a.ThicknessTest2_3, a.ThicknessTest3_1, a.ThicknessTest3_2 , a.ThicknessTest3_3, a.ThicknessTest4_1, a.ThicknessTest4_2, a.ThicknessTest4_3, a.ThicknessTest5_1, a.ThicknessTest5_2, a.ThicknessTest5_3, a.ThicknessTest_1, a.ThicknessTest_2, a.ThicknessTest_3,a.CavitySealDuration from Batteryinfo_Loading (NOLOCK) where Iden Not In (select Iden_Loading from BatteryInfo_UnLoading (NOLOCK)) and {fitler}  Order by Iden Desc";
                selSql.AppendFormat(@" select 
                         a.Iden,a.MachineNo,a.BatteryNo,a.ProductType,a.Barcode,a.Flag,a.CreateDate,a.Horn,a.Algorithm,a.Min,a.Max,a.L_1,a.L_2,a.L_3,a.L_4
               ,a.L_5,a.L_6,a.L_7,a.L_8,a.L_9,a.L_10,a.L_11,a.L_12,a.L_13,a.L_14,a.L_15,a.L_16,a.L_17,a.L_18,a.L_19,a.L_20,a.L_21,a.L_22,a.L_23
               ,a.L_24,a.L_25,a.L_26,a.L_27,a.L_28,a.L_29,a.L_30,a.ANG_1,a.ANG_2,a.ANG_3,a.ANG_4,a.ANG_5,a.ANG_6,a.ANG_7,a.ANG_8,a.ANG_9,a.ANG_10
               ,a.ANG_11,a.ANG_12,a.ANG_13,a.ANG_14,a.ANG_15,a.ANG_16,a.ANG_17,a.ANG_18,a.ANG_19,a.ANG_20,a.ANG_21,a.ANG_22,a.ANG_23,a.ANG_24
               ,a.ANG_25,a.ANG_26,a.ANG_27,a.ANG_28,a.ANG_29,a.ANG_30,a.isUpload,a.UploadReason,a.UploadDate,a.EmployeeID 
                from Batteryinfo_Loading {0} a where {1} ", Common.SqlLockKey, fitler);

                if (isHistory)
                {
                    foreach (string strDb in hisDb.Split(','))
                    {
                        if (!string.IsNullOrEmpty(strDb))
                        {
                            selSql.Append($" union all  ");
                            selSql.AppendFormat(@" select 
                                 a.Iden,a.MachineNo,a.BatteryNo,a.ProductType,a.Barcode,a.Flag,a.CreateDate,a.Horn,a.Algorithm,a.Min,a.Max,a.L_1,a.L_2,a.L_3,a.L_4
               ,a.L_5,a.L_6,a.L_7,a.L_8,a.L_9,a.L_10,a.L_11,a.L_12,a.L_13,a.L_14,a.L_15,a.L_16,a.L_17,a.L_18,a.L_19,a.L_20,a.L_21,a.L_22,a.L_23
               ,a.L_24,a.L_25,a.L_26,a.L_27,a.L_28,a.L_29,a.L_30,a.ANG_1,a.ANG_2,a.ANG_3,a.ANG_4,a.ANG_5,a.ANG_6,a.ANG_7,a.ANG_8,a.ANG_9,a.ANG_10
               ,a.ANG_11,a.ANG_12,a.ANG_13,a.ANG_14,a.ANG_15,a.ANG_16,a.ANG_17,a.ANG_18,a.ANG_19,a.ANG_20,a.ANG_21,a.ANG_22,a.ANG_23,a.ANG_24
               ,a.ANG_25,a.ANG_26,a.ANG_27,a.ANG_28,a.ANG_29,a.ANG_30,a.isUpload,a.UploadReason,a.UploadDate,a.EmployeeID 

                            from {0].{1}Batteryinfo_Loading {2} a where {3} ", strDb, Common.SqlDBKey, Common.SqlLockKey, fitler);
                        }
                    }
                }

                StringBuilder selCmd = new StringBuilder();
                selSql.Append(" Order by Iden Desc ");
                selCmd.Append(selSql);
                if (pageSize > 0)
                {
                    selCmd.AppendFormat(" LIMIT {0},{1}", pageSize * pageIndex, pageSize);
                    //selCmd.AppendFormat(" where Iden >=(select Iden from a order by Iden desc LIMIT {0},1) LIMIT {1};", pageSize * pageIndex, pageSize);
                }



                StringBuilder selCount = new StringBuilder();
                selCount.AppendFormat(@" SELECT COUNT(1) AS TotalCount FROM Batteryinfo_Loading a {0} WHERE {1} ", Common.SqlLockKey, fitler);

                if (isHistory)
                {
                    foreach (string strDb in hisDb.Split(','))
                    {
                        if (!string.IsNullOrEmpty(strDb))
                        {
                            selCount.AppendFormat(" union all SELECT COUNT(1) AS TotalCount FROM {0}.{1}Batteryinfo_Loading a {2} WHERE {3}"
                                        , strDb, Common.SqlDBKey, Common.SqlLockKey, fitler);
                        }
                    }
                }
                //  selSql.AppendFormat(";select SUM(TotalCount) AS TotalCount from ( {0} ) a", selCount.ToString());

                selSql.AppendFormat(";select cast(COUNT(1) as Decimal(12,0)) AS TotalCount from ( {0} ) a", selCount.ToString());
                DapperHelper dhelper = new DapperHelper();
                using (var con = dhelper.SQLConnection(Common.ConnectionString))
                {
                    list = con.Query<Batteryinfo_Loading>(selCmd.ToString()).ToList();
                }
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
            }

            return list;
        }


        public static BatteryData GetDeagsLoadingUploadData(string fitler, ref string errMsg, int pageSize = 1000, int pageIndex = 0, bool isHistory = false, string hisDb = "")
        {
            if (string.IsNullOrEmpty(fitler))
                fitler = " 1=1";
            BatteryData model = new BatteryData();
            StringBuilder selSql = new StringBuilder();
            try
            {
                //  string selCmd = $@" select top 1 a.Iden, a.TranfIden, a.CreateDate, a.EquipmentNo, a.No, a.ProductType, a.ProductLineNo, a.Shift, a.EndProductNo, a.LoadPalletBarCode , a.LoadPalletDate, a.BarCode, a.BarCodeDate, a.ScanCodePass, a.ScanDefectCode, a.InjectWeight, a.InjectFlag, a.CavityPressure, a.CavityDuration, a.CavityNo, a.CavityUseCount, a.FineSealingNo, a.FineSealingUseCount, a.FineSealingTopSetTemp, a.FineSealingBottomSetTemp , a.FineSealingDuration, a.FineSealingPressure, a.FineSealingTopTemp, a.FineSealingBottomTemp, a.CutterLeftUseCount, a.CutterRightUseCount , a.BeforeWeighCode, a.BeforeWeight, a.BeforeFlag, a.BeforeDefectCode, a.Weigh_LSL, a.Weigh_USL, a.IsUnLoading, a.UnLoadingDate , a.UnLoadingReason, a.CreateUser, a.IsDelete, a.DeleteDate, a.DeleteUser, a.IsUpload, a.UploadDate, a.UploadState, a.UploadNum, a.UploadLog, a.Remark, a.CreateTime, a.CavityLeftTemp, a.CavityRightTemp, a.CavityLeftSetTemp, a.CavityRightSetTemp, a.CavityVacuumTemp , a.RollingHUseNum, a.RollingHSpeed, a.RollingHPressure, a.RollingVUseNum, a.RollingVSpeed, a.RollingVPressure, a.ThicknessPressure, a.ThicknessTemp, a.ThicknessDuration, a.ThicknessTest, a.ThicknessSetTest, a.ThicknessFlag, a.ThicknessPressure1, a.ThicknessTemp1 , a.ThicknessDuration1, a.ThicknessTest1, a.ThicknessSetTest1, a.ThicknessFlag1, a.ThicknessPressure2, a.ThicknessTemp2, a.ThicknessDuration2, a.ThicknessTest2, a.ThicknessSetTest2, a.ThicknessFlag2, a.ThicknessPressure3, a.ThicknessTemp3, a.ThicknessDuration3, a.ThicknessTest3, a.ThicknessSetTest3, a.ThicknessFlag3, a.ThicknessPressure4, a.ThicknessTemp4, a.ThicknessDuration4, a.ThicknessTest4, a.ThicknessSetTest4, a.ThicknessFlag4, a.ThicknessPressure5, a.ThicknessTemp5 , a.ThicknessDuration5, a.ThicknessTest5, a.ThicknessSetTest5, a.ThicknessFlag5, a.ThicknessTest1_1, a.ThicknessTest1_2 , a.ThicknessTest1_3, a.ThicknessTest2_1, a.ThicknessTest2_2, a.ThicknessTest2_3, a.ThicknessTest3_1, a.ThicknessTest3_2 , a.ThicknessTest3_3, a.ThicknessTest4_1, a.ThicknessTest4_2, a.ThicknessTest4_3, a.ThicknessTest5_1, a.ThicknessTest5_2, a.ThicknessTest5_3, a.ThicknessTest_1, a.ThicknessTest_2, a.ThicknessTest_3,a.CavitySealDuration from Batteryinfo_Loading (NOLOCK) where Iden Not In (select Iden_Loading from BatteryInfo_UnLoading (NOLOCK)) and {fitler}  Order by Iden Desc";
                selSql.AppendFormat(@" select 

                    a.Iden,a.MachineNo,a.BatteryNo,a.ProductType,a.Barcode,a.Flag,a.CreateDate,a.Horn,a.Algorithm,a.Min,a.Max,a.L_1,a.L_2,a.L_3,a.L_4
               ,a.L_5,a.L_6,a.L_7,a.L_8,a.L_9,a.L_10,a.L_11,a.L_12,a.L_13,a.L_14,a.L_15,a.L_16,a.L_17,a.L_18,a.L_19,a.L_20,a.L_21,a.L_22,a.L_23
               ,a.L_24,a.L_25,a.L_26,a.L_27,a.L_28,a.L_29,a.L_30,a.ANG_1,a.ANG_2,a.ANG_3,a.ANG_4,a.ANG_5,a.ANG_6,a.ANG_7,a.ANG_8,a.ANG_9,a.ANG_10
               ,a.ANG_11,a.ANG_12,a.ANG_13,a.ANG_14,a.ANG_15,a.ANG_16,a.ANG_17,a.ANG_18,a.ANG_19,a.ANG_20,a.ANG_21,a.ANG_22,a.ANG_23,a.ANG_24
               ,a.ANG_25,a.ANG_26,a.ANG_27,a.ANG_28,a.ANG_29,a.ANG_30,a.isUpload,a.UploadReason,a.UploadDate,a.EmployeeID 

                from Batteryinfo_Loading {0} a where {1} ", Common.SqlLockKey, fitler);

                if (isHistory)
                {
                    foreach (string strDb in hisDb.Split(','))
                    {
                        if (!string.IsNullOrEmpty(strDb))
                        {
                            selSql.Append($" union all  ");
                            selSql.AppendFormat(@" select 
                             a.Iden,a.MachineNo,a.BatteryNo,a.ProductType,a.Barcode,a.Flag,a.CreateDate,a.Horn,a.Algorithm,a.Min,a.Max,a.L_1,a.L_2,a.L_3,a.L_4
               ,a.L_5,a.L_6,a.L_7,a.L_8,a.L_9,a.L_10,a.L_11,a.L_12,a.L_13,a.L_14,a.L_15,a.L_16,a.L_17,a.L_18,a.L_19,a.L_20,a.L_21,a.L_22,a.L_23
               ,a.L_24,a.L_25,a.L_26,a.L_27,a.L_28,a.L_29,a.L_30,a.ANG_1,a.ANG_2,a.ANG_3,a.ANG_4,a.ANG_5,a.ANG_6,a.ANG_7,a.ANG_8,a.ANG_9,a.ANG_10
               ,a.ANG_11,a.ANG_12,a.ANG_13,a.ANG_14,a.ANG_15,a.ANG_16,a.ANG_17,a.ANG_18,a.ANG_19,a.ANG_20,a.ANG_21,a.ANG_22,a.ANG_23,a.ANG_24
               ,a.ANG_25,a.ANG_26,a.ANG_27,a.ANG_28,a.ANG_29,a.ANG_30,a.isUpload,a.UploadReason,a.UploadDate,a.EmployeeID 
                            from {0].{1}Batteryinfo_Loading {2} a where {3} ", strDb, Common.SqlDBKey, Common.SqlLockKey, fitler);
                        }
                    }
                }

                StringBuilder selCmd = new StringBuilder();

                selSql.Append(" Order by Iden Desc ");
                selCmd.Append(selSql);
                if (pageSize > 0)
                    selCmd.AppendFormat(" LIMIT {0},{1}", pageSize * pageIndex, pageSize);


                StringBuilder selCount = new StringBuilder();
                selCount.AppendFormat(@" SELECT COUNT(1) AS TotalCount FROM Batteryinfo_Loading a {0} WHERE {1} ", Common.SqlLockKey, fitler);

                if (isHistory)
                {
                    foreach (string strDb in hisDb.Split(','))
                    {
                        if (!string.IsNullOrEmpty(strDb))
                        {
                            selCount.AppendFormat(" union all SELECT COUNT(1) AS TotalCount FROM {0}.{1}Batteryinfo_Loading a {2} WHERE {3}"
                                        , strDb, Common.SqlDBKey, Common.SqlLockKey, fitler);
                        }
                    }
                }
                //  selSql.AppendFormat(";select SUM(TotalCount) AS TotalCount from ( {0} ) a", selCount.ToString());

                selSql.AppendFormat(";select SUM(TotalCount) AS TotalCount from ( {0} ) a", selCount.ToString());
                DapperHelper dhelper = new DapperHelper();
                using (var con = dhelper.SQLConnection(Common.ConnectionString))
                {
                    var list = con.Query<BatteryData>(selCmd.ToString()).ToList();
                    if (list.Count > 0)
                    {
                        model = list[0];
                    }
                }
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
            }

            return model;
        }


        public static long Add(BatteryData model, ref string errMsg)
        {
            errMsg = string.Empty;
            StringBuilder insCmd = new StringBuilder();

            insCmd.AppendFormat("insert into {0}", "Batteryinfo_Loading");
            insCmd.Append("( MachineNo,BatteryNo,ProductType,Barcode,Flag,CreateDate,Horn,Algorithm,Min,Max,L_1,L_2,L_3,L_4, L_5, L_6, L_7, L_8, L_9, L_10, L_11, L_12, L_13, L_14, L_15, L_16, L_17, L_18, L_19, L_20, L_21, L_22, L_23, L_24, L_25, L_26, L_27, L_28, L_29, L_30, ANG_1, ANG_2, ANG_3, ANG_4, ANG_5, ANG_6, ANG_7, ANG_8, ANG_9, ANG_10, ANG_11, ANG_12, ANG_13, ANG_14, ANG_15, ANG_16, ANG_17, ANG_18, ANG_19, ANG_20, ANG_21, ANG_22, ANG_23, ANG_24 , ANG_25, ANG_26, ANG_27, ANG_28, ANG_29, ANG_30, isUpload, UploadReason, UploadDate, EmployeeID)");
            insCmd.Append("VALUES(@MachineNo,@BatteryNo,@ProductType,@Barcode,@Flag,@CreateDate,@Horn,@Algorithm,@Min,@Max,@L_1,@L_2,@L_3,@L_4,@L_5,@L_6,@L_7,@L_8,@L_9,@L_10,@L_11,@L_12,@L_13,@L_14,@L_15,@L_16,@L_17,@L_18,@L_19,@L_20,@L_21,@L_22,@L_23,@L_24,@L_25,@L_26,@L_27,@L_28,@L_29,@L_30,@ANG_1,@ANG_2,@ANG_3,@ANG_4,@ANG_5,@ANG_6,@ANG_7,@ANG_8,@ANG_9,@ANG_10,@ANG_11,@ANG_12,@ANG_13,@ANG_14,@ANG_15,@ANG_16,@ANG_17,@ANG_18,@ANG_19,@ANG_20,@ANG_21,@ANG_22,@ANG_23,@ANG_24 ,@ANG_25,@ANG_26,@ANG_27,@ANG_28,@ANG_29,@ANG_30,@isUpload,@UploadReason,@UploadDate,@EmployeeID)");
            insCmd.Append(";select @@IDENTITY");
            var helper = DBHelperFactory.CreateHelperFactory(Common.ConnectionString);

            try
            {

                object obj = helper.ExecuteScalar(insCmd.ToString(), true, model);


                try
                {
                    return Convert.ToInt32(obj);

                }
                catch (Exception ex)
                {
                    errMsg = $"写入上料数据库出错:{ex.Message}";
                }
                return Convert.ToInt64(obj);

            }
            catch (Exception ex)
            {
                errMsg = ex.Message.Replace("'", string.Empty);
            }
            return -1;
        }


        public static void UpdateScanData(BatteryData model, ref string errMsg, bool isUnLoading = false, bool isUnBarCode = false)
        {
            errMsg = string.Empty;
            StringBuilder insCmd = new StringBuilder();

            insCmd.AppendFormat(@"Update {0} set Barcode=@Barcode,BarcodeFlag=@BarcodeFlag "
                                            , "BatteryInfo_Loading");

            if (isUnBarCode)
 
            insCmd.Append(" where Iden = @Iden");
            DapperHelper dhelper = new DapperHelper();

            try
            {
                using (var con = dhelper.SQLConnection(Common.ConnectionString))
                {
                    con.Execute(insCmd.ToString(), model);

                }
            }
            catch (Exception ex)
            {
                errMsg = ex.Message.Replace("'", string.Empty);
            }
            finally
            {

            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <param name="errMsg"></param>
        /// <param name="isUnLoading"></param>
        /// <param name="isLoseFlag">是否更新抽液量数据</param>
        public static void UpdateAfterScanData(BatteryData model, ref string errMsg, bool isUnLoading = false, bool isLoseFlag = false, bool isUpdateUnLoading = false)
        {
            errMsg = string.Empty;
            StringBuilder insCmd = new StringBuilder();
            insCmd.AppendFormat(@"Update {0} set UnBarCode=@UnBarCode_UnLoading,UnBarCodeDate=@UnLoadingDate_UnLoading,UnScanCodePass=@UnScanCodePass_UnLoading", "BatteryInfo_UnLoading");
            //b.ThicknessPressure as ThicknessPressure_UnLoading,
            if (isUpdateUnLoading)
            {
                insCmd.Append(@",Iden_Loading=@Iden_Loading, ThicknessPressure=@ThicknessPressure_UnLoading, ThicknessTemp=@ThicknessTemp, ThicknessDuration=@ThicknessDuration
                                , ThicknessVal=@ThicknessVal, ThicknessSetVal=@ThicknessSetVal, ThicknessFlag=@ThicknessFlag
                                , TabRIDuration=@TabRIDuration, TabRISetTest=@TabRISetTest, TabRITest=@TabRITest, TabRIFlag=@TabRIFlag
                                , TabVIDuration=@TabVIDuration, TabVISetTest=@TabVISetTest, TabVITest=@TabVITest, TabVIFlag=@TabVIFlag, TabIADuration=@TabIADuration
                                , TabIASetTest=@TabIASetTest, TabIATest=@TabIATest, TabIAFlag=@TabIAFlag
                                , AiTabIAResistorTest=@AiTabIAResistorTest, AiTabIAVoltageTest=@AiTabIAVoltageTest,AiTabIADuration=@AiTabIADuration,AiTabIAFlag=@AiTabIAFlag
                                , NiTabIAResistorTest=@NiTabIAResistorTest, NiTabIAVoltageTest=@NiTabIAVoltageTest,NiTabIADuration=@NiTabIADuration,NiTabIAFlag=@NiTabIAFlag ");

            }

            if (isUnLoading)
            {
                insCmd.Append(@",IsUnLoading=@IsUnLoading_UnLoading,UnLoadingDate=@UnLoadingDate_UnLoading,UnLoadingReason=@UnLoadingReason_UnLoading ");
            }

            if (isLoseFlag)
            {
                insCmd.Append(@",LoseInjFlag=@LoseInjFlag,LoseInjectWeigh=@LoseInjectWeigh,InjectRetention=@InjectRetention,InjectRetentionFlag=@InjectRetentionFlag");
            }

            insCmd.Append(" where Iden = @Iden_UnLoading");

            DapperHelper dhelper = new DapperHelper();

            try
            {
                using (var con = dhelper.SQLConnection(Common.ConnectionString))
                {
                    con.Execute(insCmd.ToString(), model);
                }
            }
            catch (Exception ex)
            {
                errMsg = ex.Message.Replace("'", string.Empty);
            }
            finally
            {

            }
        }
        public static int GetUnloadingIsExits(BatteryData model, ref string errMsg)
        {
            DapperHelper dhelper = new DapperHelper();

            using (var con = dhelper.SQLConnection(Common.ConnectionString))
            {
                var insCmd = $@" select Iden from BatteryInfo_UnLoading {Common.SqlLockKey} where Iden_Loading=@Iden_Loading";

                var obj = con.ExecuteScalar(insCmd, model);

                return string.IsNullOrEmpty(Convert.ToString(obj)) ? -1 : Convert.ToInt32(obj);
            }
        }

        /// <summary>
        /// 下料表数据保存
        /// </summary>
        /// <param name="model"></param>
        /// <param name="errMsg"></param>
        /// <returns></returns>
        public static long InsertUnLoadingData(BatteryData model, ref string errMsg)
        {
            errMsg = string.Empty;
            StringBuilder insCmd = new StringBuilder();
            insCmd.AppendFormat("insert into {0}", "BatteryInfo_UnLoading");
            insCmd.Append("(Iden_Loading,CreateDate,BatteryNo,Barcode)");
            insCmd.Append(" Values(@Iden_Loading,@CreateDate_UnLoading,@BatteryNo_UnLoading,@Barcode_UnLoading)");

            var Iden = GetUnloadingIsExits(model, ref errMsg); //检查下料表数据是否存在
            if (Iden > 0)
            {
                return Iden;
            }

            //switch (Setting.DBConType) {
            //    case Setting.DbType.MySQL:
            //        insCmd.Append($" from dual ");
            //        break;
            //    default:
            //        break;
            //}
            //insCmd.Append($" WHERE NOT EXISTS(SELECT 1 FROM BatteryInfo_UnLoading {Common.SqlLockKey} WHERE Iden_Loading = @Iden_Loading)");
            insCmd.Append(";select @@IDENTITY");

            DapperHelper dhelper = new DapperHelper();

            try
            {
                using (var con = dhelper.SQLConnection(Common.ConnectionString))
                {
                    object obj = con.ExecuteScalar(insCmd.ToString(), model);
                    return Convert.ToInt64(obj);
                }
            }
            catch (Exception ex)
            {
                errMsg = ex.Message.Replace("'", string.Empty);
            }


            return -1;
        }

        /// <summary>
        /// 更新下表料参数
        /// </summary>
        /// <param name="model"></param>
        /// <param name="errMsg"></param>
        public static void UpdateBatteryDataUnLoadingArg(BatteryData model, ref string errMsg, bool isUnLoading = false, bool isAfterWeigh = false, bool isLoseFlag = false, bool isUpdateUnLoading = false, bool isUnBarCode = false)
        {
            errMsg = string.Empty;
            StringBuilder insCmd = new StringBuilder();
            insCmd.Append(@"Update BatteryData set ");
            insCmd.Append(@" Define1 =@Define1,Define2 =@Define2,
                Define3 =@Define3,Define4 =@Define4,Define5 =@Define5,Define6 =@Define6,Define7 =@Define7,
                Define8 =@Define8,Define9 =@Define9,Define10 =@Define10,Define11 =@Define11,
                Define12 =@Define12,Define13 =@Define13,Define14 =@Define14,Define15 =@Define15,Define16 =@Define16,
                Define17 =@Define17,Define18 =@Define18,Define19 =@Define19,Define20 =@Define20,
                Define21 =@Define21,Define22 =@Define22,Define23 =@Define23,Define24 =@Define24,Define25 =@Define25,
                Define26 =@Define26,Define27 =@Define27,Define28 =@Define28,Define29 =@Define29,Define30 =@Define30,
                Define31 =@Define31,Define32 =@Define32,Define33 =@Define33,Define34 =@Define34,Define35 =@Define35,
                Define36 =@Define36,Define37 =@Define37,Define38 =@Define38,Define39 =@Define39,Define40 =@Define40,
                Define41 =@Define41,Define42 =@Define42,Define43 =@Define43,Define44 =@Define44,Define45 =@Define45,
                Define46 =@Define46,Define47 =@Define47,Define48 =@Define48,Define49 =@Define49,Define50 =@Define50,
                Define51 =@Define51,Define52 =@Define52,Define53 =@Define53,Define54 =@Define54,Define55 =@Define55,
                Define56 =@Define56,Define57 =@Define57,Define58 =@Define58,Define59 =@Define59,Define60 =@Define60,
                Define61 =@Define61,Define62 =@Define62,Define63 =@Define63,Define64 =@Define64,Define65 =@Define65,
                Define66 =@Define66,Define67 =@Define67,Define68 =@Define68,Define69 =@Define69,Define70 =@Define70
 ,              Define71 =@Define71,Define72 =@Define72,Define73 =@Define73,Define74 =@Define74,Define75 =@Define75,
                Define76 =@Define76,Define77 =@Define77,Define78 =@Define78,Define79 =@Define79,Define80 =@Define80，
                Iden_Loading=@Iden_Loading
             ");

            DapperHelper dhelper = new DapperHelper();

            try
            {
                using (var con = dhelper.SQLConnection(Common.ConnectionString))
                {
                    con.Execute(insCmd.ToString(), model);

                }
            }
            catch (Exception ex)
            {
                errMsg = ex.Message.Replace("'", string.Empty);
            }
            finally
            {

            }
        }

        /// <summary>
        /// 获取极耳间的电压电阻测试值
        /// </summary>
        /// <param name="model"></param>
        /// <param name="errMsg"></param>
        /// <returns></returns>
        public static int UpdateTabRIVITestingData(BatteryData model, ref string errMsg)
        {
            errMsg = string.Empty;
            StringBuilder insCmd = new StringBuilder();
            insCmd.AppendFormat(@"Update {0} set TabRIDuration=@TabRIDuration,TabRISetTest=@TabRISetTest,TabRITest=@TabRITest,TabRIFlag=@TabRIFlag
                                  ,TabVIDuration=@TabVIDuration,TabVISetTest=@TabVISetTest,TabVITest=@TabVITest,TabVIFlag=@TabVIFlag 
                                  where Iden=@Iden_UnLoading", "BatteryInfo_UnLoading");
            DapperHelper dhelper = new DapperHelper();

            try
            {
                using (var con = dhelper.SQLConnection(Common.ConnectionString))
                {

                    object obj = con.ExecuteScalar(insCmd.ToString(), model);
                    return Convert.ToInt32(obj);
                }
            }
            catch (Exception ex)
            {
                errMsg = ex.Message.Replace("'", string.Empty);
            }
            finally
            {

            }
            return -1;
        }

        public static int UpdateUnLoadingThicknessTestingData(BatteryData model, ref string errMsg)
        {
            errMsg = string.Empty;
            StringBuilder insCmd = new StringBuilder();
            insCmd.Append(@"Update BatteryInfo_UnLoading set ");


            insCmd.Append(@"ThicknessTemp1=@ThicknessTemp,ThicknessDuration1=@ThicknessDuration
                                ,ThicknessTest1_1=@ThicknessTest1_1,ThicknessTest1_2=@ThicknessTest1_2,ThicknessTest1_3=@ThicknessTest1_3
                                ,ThicknessFlag1=@ThicknessFlag,ThicknessSetTest1=@ThicknessSetTest");
            insCmd.Append(@" where Iden=@Iden_Loading");

            DapperHelper dhelper = new DapperHelper();

            try
            {
                using (var con = dhelper.SQLConnection(Common.ConnectionString))
                {

                    object obj = con.ExecuteScalar(insCmd.ToString(), model);
                    return Convert.ToInt32(obj);
                }
            }
            catch (Exception ex)
            {
                errMsg = ex.Message.Replace("'", string.Empty);
            }
            finally
            {

            }
            return -1;
        }

        /// <summary>
        /// 获取电池厚度测试值
        /// </summary>
        /// <param name="model"></param>
        /// <param name="errMsg"></param>
        /// <returns></returns>
        public static int UpdateThicknessTestingData(BatteryData model, ref string errMsg)
        {
            errMsg = string.Empty;
            StringBuilder insCmd = new StringBuilder();
            insCmd.AppendFormat(@"Update {0} set ThicknessTemp=@ThicknessTemp,ThicknessDuration=@ThicknessDuration
                                                ,ThicknessVal=@ThicknessVal,ThicknessFlag=@ThicknessFlag,ThicknessSetVal=@ThicknessSetVal 
                                  where Iden=@Iden_UnLoading", "BatteryInfo_UnLoading");
            DapperHelper dhelper = new DapperHelper();

            try
            {
                using (var con = dhelper.SQLConnection(Common.ConnectionString))
                {

                    object obj = con.ExecuteScalar(insCmd.ToString(), model);
                    return Convert.ToInt32(obj);
                }
            }
            catch (Exception ex)
            {
                errMsg = ex.Message.Replace("'", string.Empty);
            }
            finally
            {

            }
            return -1;
        }


        public static List<string> GetDataBase()
        {

            DapperHelper helper = new DapperHelper();
            using (var db = helper.SQLConnection(Common.ConnectionString))
            {
                string selCmd = $"select name from master.dbo.SysDatabases WHERE name LIKE '{db.Database}_%' ";
                selCmd = "show databases;";

                var list = db.Query<string>(selCmd).ToList();

                //  list = list.Where(o => o.StartsWith(db.Database)).ToList();

                return list.ToList();
            }

        }

        /// <summary>
        /// 在前段清除后段上料表记忆
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="errMsg"></param>
        public static void ExecuteSQLClearData(string sql, ref string errMsg)
        {
            errMsg = string.Empty;
            try
            {
                var helper = DBHelperFactory.CreateHelperFactory(Common.UnLoadConnectionString);
                helper.ExecuteNonQuery(sql);
            }
            catch (Exception ex)
            {
                errMsg = ex.Message.Replace("'", string.Empty);
            }

        }


        /// <summary>
        /// 下料单机生产时，同时数据来源于上料表时，更新下料表Iden
        /// </summary>
        /// <param name="model"></param>
        /// <param name="errMsg"></param>
        /// <returns></returns>
        public static int UpdateIden_Loading(BatteryData model, ref string errMsg)
        {
            errMsg = string.Empty;
            StringBuilder insCmd = new StringBuilder();

            DapperHelper dhelper = new DapperHelper();
            try
            {

                using (var con = dhelper.SQLConnection(Common.ConnectionString))
                {
                    insCmd.Append(@"Update BatteryInfo_UnLoading set Iden_Loading=@Iden_Loading  where Iden = @Iden_UnLoading");

                    int i = con.Execute(insCmd.ToString(), model);

                    insCmd.Clear();
                    insCmd.Append($"Update BatteryInfo_UnLoading set IsDelete=1 ,DeleteUser='切折烫单机生产',DeleteDate='{DateTime.Now}' where BarCode=@BarCode and Iden <> @Iden_UnLoading");
                    con.Execute(insCmd.ToString(), model);

                    return i;
                }
            }
            catch (Exception ex)
            {
                errMsg = ex.Message.Replace("'", string.Empty);
            }
            finally
            {
            }
            return -1;
        }



        ///// <summary>
        ///// 数据实时备份转移
        ///// </summary>
        ///// <param name="errMsg"></param>
        ///// <param name="rtx"></param>
        //public static void BackupBatteryData(ref string errMsg, System.Windows.Forms.RichTextBox rtx, WaitDialogForm waitFrm)
        //{
        //    errMsg = string.Empty;
        //    Stopwatch swTotal = new Stopwatch();
        //    Stopwatch sw = new Stopwatch();
        //    DbConnection con = null, newCon = null;
        //    try
        //    {
        //        sw.Start();
        //        swTotal.Start();
        //        var year = DateTime.Now.Year;
        //        var path = Common.BackupDataBasePath + year;
        //        if (!Directory.Exists(path))
        //        {
        //            Directory.CreateDirectory(path);
        //        }
        //        var conString = Common.ConnectionString;

        //        var arrCon = conString.Split(';');
        //        var arrConDb = arrCon[1].Split('=');
        //        var dbName = $"{arrConDb[1]}_{year}";


        //        DapperHelper dhelper = new DapperHelper();
        //        con = dhelper.SQLConnection(conString);
        //        waitFrm.SetCaption(string.Format(Common.GetMessage("STRING_DATABASE_BACKUP_1"), dbName));

        //        Common.SetRichText(rtx, string.Format(Common.GetMessage("STRING_DATABASE_BACKUP_1"), dbName));
        //        string sqlFile = "sql";
        //        string insCmd = "";
        //        switch (Setting.DBConType)
        //        {
        //            case Setting.DbType.MSSQL:
        //                insCmd = $"select count(1) from sys.databases where name ='{dbName}'";
        //                break;
        //            case Setting.DbType.MySQL:
        //                sqlFile = "mysql";
        //                insCmd = $"select count(1) from information_schema.SCHEMATA where SCHEMA_NAME = '{dbName}'";
        //                break;
        //        }
        //        var objDb = con.ExecuteScalar(insCmd);
        //        if (Convert.ToInt32(objDb) <= 0)
        //        {
        //            waitFrm.SetCaption(string.Format(Common.GetMessage("STRING_DATABASE_BACKUP_2"), dbName));
        //            Common.SetRichText(rtx, string.Format(Common.GetMessage("STRING_DATABASE_BACKUP_2"), dbName));

        //            string cmdDb = "";
        //            switch (Setting.DBConType)
        //            {
        //                case Setting.DbType.MSSQL:
        //                    cmdDb = $@" CREATE DATABASE {dbName} ON PRIMARY
        //                            ( NAME = '{dbName}', FILENAME = '{path}\\{dbName}.mdf' , SIZE = 47104KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
        //                                LOG ON 
        //                            ( NAME = '{dbName}_Log', FILENAME ='{path}\\{dbName}_Log.ldf' , SIZE = 35712KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
        //                    ";
        //                    con.ExecuteScalar(cmdDb);

        //                    var cmdIns = $@"select * into {dbName}.dbo.Batteryinfo_Loading from InjectionData_Loading where 1=2;
        //                                select * into {dbName}.dbo.BatteryInfo_UnLoading from InjectionData_UnLoading where 1=2;";
        //                    con.Execute(cmdIns);
        //                    cmdIns = $@"     alter table {dbName}.dbo.Batteryinfo_Loading drop column Iden;
        //                                 ALTER TABLE {dbName}.dbo.Batteryinfo_Loading ADD Iden int not null  ;
        //                                 ALTER TABLE {dbName}.dbo.Batteryinfo_Loading ADD ID int IDENTITY(1,1) not null primary key ;";
        //                    con.Execute(cmdIns);
        //                    cmdIns = $@"     alter table {dbName}.dbo.BatteryInfo_UnLoading drop column Iden;
        //                                 ALTER TABLE {dbName}.dbo.BatteryInfo_UnLoading ADD Iden int not null  ;
        //                                 ALTER TABLE {dbName}.dbo.BatteryInfo_UnLoading ADD ID int IDENTITY(1,1) not null primary key ; ";
        //                    con.Execute(cmdIns);
        //                    break;
        //                case Setting.DbType.MySQL:
        //                    string selCmd = $"create database if not exists {dbName} DEFAULT CHARACTER SET utf8; ";
        //                    con.Execute(selCmd);

        //                    selCmd = $"CREATE TABLE IF NOT EXISTS {dbName}.Batteryinfo_Loading LIKE Batteryinfo_Loading ; ";
        //                    con.Execute(selCmd);

        //                    selCmd = $"CREATE TABLE IF NOT EXISTS {dbName}.BatteryInfo_UnLoading LIKE BatteryInfo_UnLoading ; ";
        //                    con.Execute(selCmd);


        //                    break;
        //            }
        //            waitFrm.SetCaption(string.Format(Common.GetMessage("STRING_DATABASE_BACKUP_3"), dbName));
        //            Common.SetRichText(rtx, string.Format(Common.GetMessage("STRING_DATABASE_BACKUP_3"), dbName));

        //        }


        //        var newConString = conString.Replace(arrConDb[1], dbName);
        //        DapperHelper dhNewHelper = new DapperHelper();
        //        newCon = dhNewHelper.SQLConnection(newConString);

        //        waitFrm.SetCaption(Common.GetMessage("STRING_CHECKDBSRCIPT"));
        //        Common.SetRichText(rtx, Common.GetMessage("STRING_CHECKDBSRCIPT"));

        //        DirectoryInfo DirectoryArray = new DirectoryInfo(Application.StartupPath + @"\" + sqlFile);
        //        FileInfo[] Files = DirectoryArray.GetFiles();//获取该文件夹下的文件列表
        //        DirectoryInfo[] Directorys = DirectoryArray.GetDirectories();//获取该文件夹下的文件夹列表
        //        var ALLFiles = Files.ToList().FindAll(o => o.Extension.Equals(".sql") && o.Name.ToUpper().Contains("IPTSDB_"));
        //        if (ALLFiles != null)
        //        {
        //            try
        //            {
        //                foreach (FileInfo inf in ALLFiles)
        //                {//逐个复制文件
        //                    waitFrm.SetCaption(Common.GetMessage("STRING_CHECKDBSRCIPT_1", inf.Name));
        //                    Common.SetRichText(rtx, Common.GetMessage("STRING_CHECKDBSRCIPT_1", inf.Name));
        //                    FileInfo fi = new FileInfo(inf.FullName);
        //                    var fileLastWriteTime = fi.LastWriteTime.ToString("yyyy-MM-dd HH:mm:ss");
        //                    string filePath = fi.Name.Replace(".sql", string.Empty);

        //                    var lastWriteTime = FilesManager.GetPrivateProfileString(Application.StartupPath + "\\sysConfig.ini", "config_backup", filePath + "WriteTime", "1601-01-01 08:00:00");
        //                    if (Convert.ToDateTime(fileLastWriteTime) > Convert.ToDateTime(lastWriteTime))
        //                    {
        //                        string exeSql = File.ReadAllText(inf.FullName);//

        //                        if (!string.IsNullOrEmpty(exeSql))
        //                        {

        //                            var lstExSQL = exeSql.Split(new string[] { "GO" }, StringSplitOptions.RemoveEmptyEntries);

        //                            foreach (string sql in lstExSQL)
        //                            {
        //                                var _sql = sql.Replace("supercomdb", dbName);
        //                                newCon.Execute(_sql);
        //                            }
        //                            FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\sysConfig.ini", "config_backup", filePath + "WriteTime", fileLastWriteTime);
        //                        }
        //                    }
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                errMsg = ex.Message;
        //                Common.SetRichText(rtx, errMsg);
        //            }
        //        }


        //        var now = DateTime.Now.AddMonths(-Common.BackupDataBaseMonths);
        //        Common.SetRichText(rtx, string.Format(Common.GetMessage("STRING_DATABASE_BACKUP_4"), now));
        //        waitFrm.SetCaption(string.Format(Common.GetMessage("STRING_DATABASE_BACKUP_4"), now));

        //        insCmd = string.Format($@"insert into {dbName}.{Common.SqlDBKey}Batteryinfo_Loading(Iden,TranfIden,CreateDate,EquipmentNo,No,ProductType,ProductLineNo,Shift,EndProductNo,LoadPalletBarCode,LoadPalletDate,BarCode,BarCodeDate,ScanCodePass,ScanDefectCode,InjectWeight,InjectFlag,BeforeWeighCode,BeforeWeight,BeforeFlag,BeforeDefectCode,Weigh_LSL,Weigh_USL,IsUnLoading,UnLoadingDate,UnLoadingReason,CreateUser,IsDelete,DeleteDate,DeleteUser,IsUpload,UploadDate,UploadState,UploadNum,UploadLog,Remark,CreateTime,ThicknessPressure,ThicknessTemp,ThicknessDuration,ThicknessTest,ThicknessSetTest,ThicknessFlag,ThicknessPressure1,ThicknessTemp1,ThicknessDuration1,ThicknessTest1,ThicknessSetTest1,ThicknessFlag1,ThicknessPressure2,ThicknessTemp2,ThicknessDuration2,ThicknessTest2,ThicknessSetTest2,ThicknessFlag2,ThicknessPressure3,ThicknessTemp3,ThicknessDuration3,ThicknessTest3,ThicknessSetTest3,ThicknessFlag3,ThicknessPressure4,ThicknessTemp4,ThicknessDuration4,ThicknessTest4,ThicknessSetTest4,ThicknessFlag4,ThicknessPressure5,ThicknessTemp5,ThicknessDuration5,ThicknessTest5,ThicknessSetTest5,ThicknessFlag5,TabRIAnodeTest,TabRIAnodeFlag,TabRIAnode_USL,TabRIAnode_LSL,TabRIAnodeDuration,TabRIAnodeSetTest,TabRICathodeTest,TabRICathodeFlag,TabRICathode_USL,TabRICathode_LSL,TabRICathodeDuration,TabRICathodeSetTest,DegasType,ThicknessTest_1,ThicknessTest_2,ThicknessTest_3,ThicknessTest1_1,ThicknessTest1_2,ThicknessTest1_3,ThicknessTest2_1,ThicknessTest2_2,ThicknessTest2_3,ThicknessTest3_1,ThicknessTest3_2,ThicknessTest3_3,ThicknessTest4_1,ThicknessTest4_2,ThicknessTest4_3,ThicknessTest5_1,ThicknessTest5_2,ThicknessTest5_3,InjBeforeWeight,InjAfterWeight,ThicknessTest_USL,ThicknessTest_LSL,Define1,Define2,Define3,Define4,Define5,Define6,Define7,Define8,Define9,Define10,Define11,Define12,Define13,Define14,Define15,Define16,Define17,Define18,Define19,Define20,Define21,Define22,Define23,Define24,Define25,Define26,Define27,Define28,Define29,Define30,Define31,Define32,Define33,Define34,Define35,Define36,Define37,Define38,Define39,Define40,Define41,Define42,Define43,Define44,Define45,Define46,Define47,Define48,Define49,Define50,TKNGNumCheck,InjectRetention_LSL,InjectRetention_USL)
        //                    SELECT Iden,TranfIden,CreateDate,EquipmentNo,No,ProductType,ProductLineNo,Shift,EndProductNo,LoadPalletBarCode,LoadPalletDate,BarCode,BarCodeDate,ScanCodePass,ScanDefectCode,InjectWeight,InjectFlag,BeforeWeighCode,BeforeWeight,BeforeFlag,BeforeDefectCode,Weigh_LSL,Weigh_USL,IsUnLoading,UnLoadingDate,UnLoadingReason,CreateUser,IsDelete,DeleteDate,DeleteUser,IsUpload,UploadDate,UploadState,UploadNum,UploadLog,Remark,CreateTime,ThicknessPressure,ThicknessTemp,ThicknessDuration,ThicknessTest,ThicknessSetTest,ThicknessFlag,ThicknessPressure1,ThicknessTemp1,ThicknessDuration1,ThicknessTest1,ThicknessSetTest1,ThicknessFlag1,ThicknessPressure2,ThicknessTemp2,ThicknessDuration2,ThicknessTest2,ThicknessSetTest2,ThicknessFlag2,ThicknessPressure3,ThicknessTemp3,ThicknessDuration3,ThicknessTest3,ThicknessSetTest3,ThicknessFlag3,ThicknessPressure4,ThicknessTemp4,ThicknessDuration4,ThicknessTest4,ThicknessSetTest4,ThicknessFlag4,ThicknessPressure5,ThicknessTemp5,ThicknessDuration5,ThicknessTest5,ThicknessSetTest5,ThicknessFlag5,TabRIAnodeTest,TabRIAnodeFlag,TabRIAnode_USL,TabRIAnode_LSL,TabRIAnodeDuration,TabRIAnodeSetTest,TabRICathodeTest,TabRICathodeFlag,TabRICathode_USL,TabRICathode_LSL,TabRICathodeDuration,TabRICathodeSetTest,DegasType,ThicknessTest_1,ThicknessTest_2,ThicknessTest_3,ThicknessTest1_1,ThicknessTest1_2,ThicknessTest1_3,ThicknessTest2_1,ThicknessTest2_2,ThicknessTest2_3,ThicknessTest3_1,ThicknessTest3_2,ThicknessTest3_3,ThicknessTest4_1,ThicknessTest4_2,ThicknessTest4_3,ThicknessTest5_1,ThicknessTest5_2,ThicknessTest5_3,InjBeforeWeight,InjAfterWeight,ThicknessTest_USL,ThicknessTest_LSL,Define1,Define2,Define3,Define4,Define5,Define6,Define7,Define8,Define9,Define10,Define11,Define12,Define13,Define14,Define15,Define16,Define17,Define18,Define19,Define20,Define21,Define22,Define23,Define24,Define25,Define26,Define27,Define28,Define29,Define30,Define31,Define32,Define33,Define34,Define35,Define36,Define37,Define38,Define39,Define40,Define41,Define42,Define43,Define44,Define45,Define46,Define47,Define48,Define49,Define50,TKNGNumCheck,InjectRetention_LSL,InjectRetention_USL FROM {arrConDb[1]}.{Common.SqlDBKey}Batteryinfo_Loading WHERE CreateDate<'{now}'");

        //        int _insRow = con.Execute(insCmd, null, null, 216000);

        //        string delCmd = $"delete from {arrConDb[1]}.{Common.SqlDBKey}Batteryinfo_Loading where CreateDate<'{now}'";
        //        int rowCount = con.Execute(delCmd, null, null, 216000);


        //        //var lstLoad = BLL.Batteryinfo_LoadingBLL.GetList($"CreateDate<'{now}'", ref errMsg);
        //        //if (!string.IsNullOrEmpty(errMsg)) {
        //        //    Common.SetRichText(rtx, string.Format(Common.GetMessage("STRING_DATABASE_BACKUP_5"), now, errMsg));
        //        //} else {
        //        //    Common.SetRichText(rtx, string.Format(Common.GetMessage("STRING_DATABASE_BACKUP_6"), now, lstLoad.Count));
        //        //    waitFrm.SetCaption(string.Format(Common.GetMessage("STRING_DATABASE_BACKUP_6"), now, lstLoad.Count));

        //        //    insCmd = $@"insert into Batteryinfo_Loading(Iden,TranfIden,CreateDate,EquipmentNo,No,ProductType,ProductLineNo,Shift,EndProductNo,LoadPalletBarCode,LoadPalletDate,BarCode,BarCodeDate,ScanCodePass,ScanDefectCode,InjectWeight,InjectFlag,BeforeWeighCode,BeforeWeight,BeforeFlag,BeforeDefectCode,Weigh_LSL,Weigh_USL,IsUnLoading,UnLoadingDate,UnLoadingReason,CreateUser,IsDelete,DeleteDate,DeleteUser,IsUpload,UploadDate,UploadState,UploadNum,UploadLog,Remark,CreateTime,ThicknessPressure,ThicknessTemp,ThicknessDuration,ThicknessTest,ThicknessSetTest,ThicknessFlag,ThicknessPressure1,ThicknessTemp1,ThicknessDuration1,ThicknessTest1,ThicknessSetTest1,ThicknessFlag1,ThicknessPressure2,ThicknessTemp2,ThicknessDuration2,ThicknessTest2,ThicknessSetTest2,ThicknessFlag2,ThicknessPressure3,ThicknessTemp3,ThicknessDuration3,ThicknessTest3,ThicknessSetTest3,ThicknessFlag3,ThicknessPressure4,ThicknessTemp4,ThicknessDuration4,ThicknessTest4,ThicknessSetTest4,ThicknessFlag4,ThicknessPressure5,ThicknessTemp5,ThicknessDuration5,ThicknessTest5,ThicknessSetTest5,ThicknessFlag5,TabRIAnodeTest,TabRIAnodeFlag,TabRIAnode_USL,TabRIAnode_LSL,TabRIAnodeDuration,TabRIAnodeSetTest,TabRICathodeTest,TabRICathodeFlag,TabRICathode_USL,TabRICathode_LSL,TabRICathodeDuration,TabRICathodeSetTest,DegasType,ThicknessTest_1,ThicknessTest_2,ThicknessTest_3,ThicknessTest1_1,ThicknessTest1_2,ThicknessTest1_3,ThicknessTest2_1,ThicknessTest2_2,ThicknessTest2_3,ThicknessTest3_1,ThicknessTest3_2,ThicknessTest3_3,ThicknessTest4_1,ThicknessTest4_2,ThicknessTest4_3,ThicknessTest5_1,ThicknessTest5_2,ThicknessTest5_3,InjBeforeWeight,InjAfterWeight,ThicknessTest_USL,ThicknessTest_LSL,Define1,Define2,Define3,Define4,Define5,Define6,Define7,Define8,Define9,Define10,Define11,Define12,Define13,Define14,Define15,Define16,Define17,Define18,Define19,Define20,Define21,Define22,Define23,Define24,Define25,Define26,Define27,Define28,Define29,Define30,Define31,Define32,Define33,Define34,Define35,Define36,Define37,Define38,Define39,Define40,Define41,Define42,Define43,Define44,Define45,Define46,Define47,Define48,Define49,Define50,TKNGNumCheck,InjectRetention_LSL,InjectRetention_USL)
        //        //         VALUES(@Iden,@TranfIden,@CreateDate,@EquipmentNo,@No,@ProductType,@ProductLineNo,@Shift,@EndProductNo,@LoadPalletBarCode,@LoadPalletDate,@BarCode,@BarCodeDate,@ScanCodePass,@ScanDefectCode,@InjectWeight,@InjectFlag,@BeforeWeighCode,@BeforeWeight,@BeforeFlag,@BeforeDefectCode,@Weigh_LSL,@Weigh_USL,@IsUnLoading,@UnLoadingDate,@UnLoadingReason,@CreateUser,@IsDelete,@DeleteDate,@DeleteUser,@IsUpload,@UploadDate,@UploadState,@UploadNum,@UploadLog,@Remark,@CreateTime,@ThicknessPressure,@ThicknessTemp,@ThicknessDuration,@ThicknessTest,@ThicknessSetTest,@ThicknessFlag,@ThicknessPressure1,@ThicknessTemp1,@ThicknessDuration1,@ThicknessTest1,@ThicknessSetTest1,@ThicknessFlag1,@ThicknessPressure2,@ThicknessTemp2,@ThicknessDuration2,@ThicknessTest2,@ThicknessSetTest2,@ThicknessFlag2,@ThicknessPressure3,@ThicknessTemp3,@ThicknessDuration3,@ThicknessTest3,@ThicknessSetTest3,@ThicknessFlag3,@ThicknessPressure4,@ThicknessTemp4,@ThicknessDuration4,@ThicknessTest4,@ThicknessSetTest4,@ThicknessFlag4,@ThicknessPressure5,@ThicknessTemp5,@ThicknessDuration5,@ThicknessTest5,@ThicknessSetTest5,@ThicknessFlag5,@TabRIAnodeTest,@TabRIAnodeFlag,@TabRIAnode_USL,@TabRIAnode_LSL,@TabRIAnodeDuration,@TabRIAnodeSetTest,@TabRICathodeTest,@TabRICathodeFlag,@TabRICathode_USL,@TabRICathode_LSL,@TabRICathodeDuration,@TabRICathodeSetTest,@DegasType,@ThicknessTest_1,@ThicknessTest_2,@ThicknessTest_3,@ThicknessTest1_1,@ThicknessTest1_2,@ThicknessTest1_3,@ThicknessTest2_1,@ThicknessTest2_2,@ThicknessTest2_3,@ThicknessTest3_1,@ThicknessTest3_2,@ThicknessTest3_3,@ThicknessTest4_1,@ThicknessTest4_2,@ThicknessTest4_3,@ThicknessTest5_1,@ThicknessTest5_2,@ThicknessTest5_3,@InjBeforeWeight,@InjAfterWeight,@ThicknessTest_USL,@ThicknessTest_LSL,@Define1,@Define2,@Define3,@Define4,@Define5,@Define6,@Define7,@Define8,@Define9,@Define10,@Define11,@Define12,@Define13,@Define14,@Define15,@Define16,@Define17,@Define18,@Define19,@Define20,@Define21,@Define22,@Define23,@Define24,@Define25,@Define26,@Define27,@Define28,@Define29,@Define30,@Define31,@Define32,@Define33,@Define34,@Define35,@Define36,@Define37,@Define38,@Define39,@Define40,@Define41,@Define42,@Define43,@Define44,@Define45,@Define46,@Define47,@Define48,@Define49,@Define50,@TKNGNumCheck,@InjectRetention_LSL,@InjectRetention_USL)";

        //        //    for (int i = 0; i < lstLoad.Count; i++) {
        //        //        var model = lstLoad[i];
        //        //        int dbRow = newCon.Execute(insCmd, model);
        //        //        if (dbRow > 0) {
        //        //            string delCmd = $"delete from Batteryinfo_Loading where Iden ={model.Iden}";
        //        //            con.Execute(delCmd);
        //        //        }
        //        //        waitFrm.SetCaption(string.Format(Common.GetMessage("STRING_DATABASE_BACKUP_7"), i, model.Iden, model.BarCode, model.CreateDate, i, lstLoad.Count, dbRow));
        //        //        //  Common.SetRichText(rtx, string.Format("5-{0}.ID:[{1}],条码[{2}],时间[{3}]正在转移第[{4}/{5}]行数据,转移结果:{6}", i, model.Iden, model.BarCode, model.CreateDate, i, lstLoad.Count, dbRow));
        //        //    }
        //        //}

        //        sw.Stop();

        //        waitFrm.SetCaption(string.Format(Common.GetMessage("STRING_DATABASE_BACKUP_8"), $"{_insRow}/{rowCount}", now, sw.Elapsed));
        //        Common.SetRichText(rtx, string.Format(Common.GetMessage("STRING_DATABASE_BACKUP_8"), $"{_insRow}/{rowCount}", now, sw.Elapsed));
        //        sw.Restart();

        //        Common.SetRichText(rtx, string.Format(Common.GetMessage("STRING_DATABASE_BACKUP_"), now));
        //        waitFrm.SetCaption(string.Format(Common.GetMessage("STRING_DATABASE_BACKUP_"), now));


        //        insCmd = string.Format($@"insert into {dbName}.{Common.SqlDBKey}BatteryInfo_UnLoading(Iden,Iden_Loading,IsAfter,CreateDate,EquipmentNo,No,ProductType,ProductLineNo,Shift,EndProductNo,BarCode,BarCodeDate,ScanCodePass,ScanDefectCode,HotSideDuration,CCDX,CCDY,CCDFlag,ThicknessPressure,ThicknessTemp,ThicknessDuration,ThicknessVal,ThicknessSetVal,ThicknessFlag,TabRIDuration,TabRISetTest,TabRITest,TabRIFlag,TabVIDuration,TabVISetTest,TabVITest,TabVIFlag,TabIADuration,TabIASetTest,TabIATest,TabIAFlag,AfterWeighCode,AfterWeight,LoseInjectWeigh,AfterFlag,After_LSL,After_USL,TotalNum,TotalOKNum,TotalNGNum,VoltageNGNum,InsulatorNGNum,CCDNGNum,WeightNGNum,ThicknessNGNum,AfterDefectCode,UnBarCode,UnBarCodeDate,UnScanCodePass,UnScanDefectCode,UnPalletBarCode,UnPalletDate,IsUnLoading,UnLoadingDate,UnLoadingReason,CreateUser,IsDelete,DeleteDate,DeleteUser,IsUpload,UploadDate,UploadState,UploadNum,UploadLog,Remark,CreateTime,HotSideLTemp1,HotSideLTemp2,HotSideRTemp1,HotSideRTemp2,CutterLeftUseCount,CutterRightUseCount,TapeCutterLeftUseNum,TapeCutterRightUseNum,AiTabIAResistorTest,AiTabIAVoltageTest,AiTabIADuration,AiTabIAFlag,NiTabIAResistorTest,NiTabIAVoltageTest,NiTabIADuration,NiTabIAFlag,HotSnapSideLTemp1,HotSnapSideLTemp2,HotSnapSideRTemp1,HotSnapSideRTemp2,HotSnapSideDuration,HotSnap2SideLTemp1,HotSnap2SideLTemp2,HotSnap2SideRTemp1,HotSnap2SideRTemp2,HotSnap2SideDuration,Hot2SideLTemp1,Hot2SideLTemp2,Hot2SideRTemp1,Hot2SideRTemp2,Hot2SideDuration,SnapSideDuration,InjectRetention,LoseInj_USL,LoseInj_LSL,LoseInjFlag,Define1,Define2,Define3,Define4,Define5,Define6,Define7,Define8,Define9,Define10,Define11,Define12,Define13,Define14,Define15,Define16,Define17,Define18,Define19,Define20,Define21,Define22,Define23,Define24,Define25,Define26,Define27,Define28,Define29,Define30,Define31,Define32,Define33,Define34,Define35,Define36,Define37,Define38,Define39,Define40,Define41,Define42,Define43,Define44,Define45,Define46,Define47,Define48,Define49,Define50,InjVolume,PalletNo,UploadLog_Pallet,UploadNum_Pallet,UploadDate_Pallet,IsUpload_Pallet,IsScv,ScvDate,Define51,Define52,Define53,Define54,Define55,Define56,Define57,Define58,Define59,Define60)
        //                    SELECT Iden,Iden_Loading,IsAfter,CreateDate,EquipmentNo,No,ProductType,ProductLineNo,Shift,EndProductNo,BarCode,BarCodeDate,ScanCodePass,ScanDefectCode,HotSideDuration,CCDX,CCDY,CCDFlag,ThicknessPressure,ThicknessTemp,ThicknessDuration,ThicknessVal,ThicknessSetVal,ThicknessFlag,TabRIDuration,TabRISetTest,TabRITest,TabRIFlag,TabVIDuration,TabVISetTest,TabVITest,TabVIFlag,TabIADuration,TabIASetTest,TabIATest,TabIAFlag,AfterWeighCode,AfterWeight,LoseInjectWeigh,AfterFlag,After_LSL,After_USL,TotalNum,TotalOKNum,TotalNGNum,VoltageNGNum,InsulatorNGNum,CCDNGNum,WeightNGNum,ThicknessNGNum,AfterDefectCode,UnBarCode,UnBarCodeDate,UnScanCodePass,UnScanDefectCode,UnPalletBarCode,UnPalletDate,IsUnLoading,UnLoadingDate,UnLoadingReason,CreateUser,IsDelete,DeleteDate,DeleteUser,IsUpload,UploadDate,UploadState,UploadNum,UploadLog,Remark,CreateTime,HotSideLTemp1,HotSideLTemp2,HotSideRTemp1,HotSideRTemp2,CutterLeftUseCount,CutterRightUseCount,TapeCutterLeftUseNum,TapeCutterRightUseNum,AiTabIAResistorTest,AiTabIAVoltageTest,AiTabIADuration,AiTabIAFlag,NiTabIAResistorTest,NiTabIAVoltageTest,NiTabIADuration,NiTabIAFlag,HotSnapSideLTemp1,HotSnapSideLTemp2,HotSnapSideRTemp1,HotSnapSideRTemp2,HotSnapSideDuration,HotSnap2SideLTemp1,HotSnap2SideLTemp2,HotSnap2SideRTemp1,HotSnap2SideRTemp2,HotSnap2SideDuration,Hot2SideLTemp1,Hot2SideLTemp2,Hot2SideRTemp1,Hot2SideRTemp2,Hot2SideDuration,SnapSideDuration,InjectRetention,LoseInj_USL,LoseInj_LSL,LoseInjFlag,Define1,Define2,Define3,Define4,Define5,Define6,Define7,Define8,Define9,Define10,Define11,Define12,Define13,Define14,Define15,Define16,Define17,Define18,Define19,Define20,Define21,Define22,Define23,Define24,Define25,Define26,Define27,Define28,Define29,Define30,Define31,Define32,Define33,Define34,Define35,Define36,Define37,Define38,Define39,Define40,Define41,Define42,Define43,Define44,Define45,Define46,Define47,Define48,Define49,Define50,InjVolume,PalletNo,UploadLog_Pallet,UploadNum_Pallet,UploadDate_Pallet,IsUpload_Pallet,IsScv,ScvDate,Define51,Define52,Define53,Define54,Define55,Define56,Define57,Define58,Define59,Define60 FROM {arrConDb[1]}.{Common.SqlDBKey}BatteryInfo_UnLoading WHERE CreateDate<'{now}'");

        //        _insRow = con.Execute(insCmd, null, null, 216000);

        //        delCmd = $"delete from {arrConDb[1]}.{Common.SqlDBKey}BatteryInfo_UnLoading where CreateDate<'{now}'";
        //        rowCount = con.Execute(delCmd, null, null, 216000);


        //        //var lstUnLoad = BLL.BatteryData_UnLoadingBLL.GetList($"CreateDate<'{now}'", ref errMsg);

        //        //if (!string.IsNullOrEmpty(errMsg)) {
        //        //    Common.SetRichText(rtx, string.Format(Common.GetMessage("STRING_DATABASE_BACKUP_10"), now, errMsg));
        //        //} else {
        //        //    Common.SetRichText(rtx, string.Format(Common.GetMessage("STRING_DATABASE_BACKUP_11"), now, lstUnLoad.Count));
        //        //    waitFrm.SetCaption(string.Format(Common.GetMessage("STRING_DATABASE_BACKUP_11"), now, lstUnLoad.Count));

        //        //    insCmd = $@"insert into BatteryInfo_UnLoading(Iden,Iden_Loading,IsAfter,CreateDate,EquipmentNo,No,ProductType,ProductLineNo,Shift,EndProductNo,BarCode,BarCodeDate,ScanCodePass,ScanDefectCode,HotSideDuration,CCDX,CCDY,CCDFlag,ThicknessPressure,ThicknessTemp,ThicknessDuration,ThicknessVal,ThicknessSetVal,ThicknessFlag,TabRIDuration,TabRISetTest,TabRITest,TabRIFlag,TabVIDuration,TabVISetTest,TabVITest,TabVIFlag,TabIADuration,TabIASetTest,TabIATest,TabIAFlag,AfterWeighCode,AfterWeight,LoseInjectWeigh,AfterFlag,After_LSL,After_USL,TotalNum,TotalOKNum,TotalNGNum,VoltageNGNum,InsulatorNGNum,CCDNGNum,WeightNGNum,ThicknessNGNum,AfterDefectCode,UnBarCode,UnBarCodeDate,UnScanCodePass,UnScanDefectCode,UnPalletBarCode,UnPalletDate,IsUnLoading,UnLoadingDate,UnLoadingReason,CreateUser,IsDelete,DeleteDate,DeleteUser,IsUpload,UploadDate,UploadState,UploadNum,UploadLog,Remark,CreateTime,HotSideLTemp1,HotSideLTemp2,HotSideRTemp1,HotSideRTemp2,CutterLeftUseCount,CutterRightUseCount,TapeCutterLeftUseNum,TapeCutterRightUseNum,AiTabIAResistorTest,AiTabIAVoltageTest,AiTabIADuration,AiTabIAFlag,NiTabIAResistorTest,NiTabIAVoltageTest,NiTabIADuration,NiTabIAFlag,HotSnapSideLTemp1,HotSnapSideLTemp2,HotSnapSideRTemp1,HotSnapSideRTemp2,HotSnapSideDuration,HotSnap2SideLTemp1,HotSnap2SideLTemp2,HotSnap2SideRTemp1,HotSnap2SideRTemp2,HotSnap2SideDuration,Hot2SideLTemp1,Hot2SideLTemp2,Hot2SideRTemp1,Hot2SideRTemp2,Hot2SideDuration,SnapSideDuration,InjectRetention,LoseInj_USL,LoseInj_LSL,LoseInjFlag,Define1,Define2,Define3,Define4,Define5,Define6,Define7,Define8,Define9,Define10,Define11,Define12,Define13,Define14,Define15,Define16,Define17,Define18,Define19,Define20,Define21,Define22,Define23,Define24,Define25,Define26,Define27,Define28,Define29,Define30,Define31,Define32,Define33,Define34,Define35,Define36,Define37,Define38,Define39,Define40,Define41,Define42,Define43,Define44,Define45,Define46,Define47,Define48,Define49,Define50,InjVolume,PalletNo,UploadLog_Pallet,UploadNum_Pallet,UploadDate_Pallet,IsUpload_Pallet,IsScv,ScvDate,Define51,Define52,Define53,Define54,Define55,Define56,Define57,Define58,Define59,Define60)
        //        //            VALUES(@Iden,@Iden_Loading,@IsAfter,@CreateDate,@EquipmentNo,@No,@ProductType,@ProductLineNo,@Shift,@EndProductNo,@BarCode,@BarCodeDate,@ScanCodePass,@ScanDefectCode,@HotSideDuration,@CCDX,@CCDY,@CCDFlag,@ThicknessPressure,@ThicknessTemp,@ThicknessDuration,@ThicknessVal,@ThicknessSetVal,@ThicknessFlag,@TabRIDuration,@TabRISetTest,@TabRITest,@TabRIFlag,@TabVIDuration,@TabVISetTest,@TabVITest,@TabVIFlag,@TabIADuration,@TabIASetTest,@TabIATest,@TabIAFlag,@AfterWeighCode,@AfterWeight,@LoseInjectWeigh,@AfterFlag,@After_LSL,@After_USL,@TotalNum,@TotalOKNum,@TotalNGNum,@VoltageNGNum,@InsulatorNGNum,@CCDNGNum,@WeightNGNum,@ThicknessNGNum,@AfterDefectCode,@UnBarCode,@UnBarCodeDate,@UnScanCodePass,@UnScanDefectCode,@UnPalletBarCode,@UnPalletDate,@IsUnLoading,@UnLoadingDate,@UnLoadingReason,@CreateUser,@IsDelete,@DeleteDate,@DeleteUser,@IsUpload,@UploadDate,@UploadState,@UploadNum,@UploadLog,@Remark,@CreateTime,@HotSideLTemp1,@HotSideLTemp2,@HotSideRTemp1,@HotSideRTemp2,@CutterLeftUseCount,@CutterRightUseCount,@TapeCutterLeftUseNum,@TapeCutterRightUseNum,@AiTabIAResistorTest,@AiTabIAVoltageTest,@AiTabIADuration,@AiTabIAFlag,@NiTabIAResistorTest,@NiTabIAVoltageTest,@NiTabIADuration,@NiTabIAFlag,@HotSnapSideLTemp1,@HotSnapSideLTemp2,@HotSnapSideRTemp1,@HotSnapSideRTemp2,@HotSnapSideDuration,@HotSnap2SideLTemp1,@HotSnap2SideLTemp2,@HotSnap2SideRTemp1,@HotSnap2SideRTemp2,@HotSnap2SideDuration,@Hot2SideLTemp1,@Hot2SideLTemp2,@Hot2SideRTemp1,@Hot2SideRTemp2,@Hot2SideDuration,@SnapSideDuration,@InjectRetention,@LoseInj_USL,@LoseInj_LSL,@LoseInjFlag,@Define1,@Define2,@Define3,@Define4,@Define5,@Define6,@Define7,@Define8,@Define9,@Define10,@Define11,@Define12,@Define13,@Define14,@Define15,@Define16,@Define17,@Define18,@Define19,@Define20,@Define21,@Define22,@Define23,@Define24,@Define25,@Define26,@Define27,@Define28,@Define29,@Define30,@Define31,@Define32,@Define33,@Define34,@Define35,@Define36,@Define37,@Define38,@Define39,@Define40,@Define41,@Define42,@Define43,@Define44,@Define45,@Define46,@Define47,@Define48,@Define49,@Define50,@InjVolume,@PalletNo,@UploadLog_Pallet,@UploadNum_Pallet,@UploadDate_Pallet,@IsUpload_Pallet,@IsScv,@ScvDate,@Define51,@Define52,@Define53,@Define54,@Define55,@Define56,@Define57,@Define58,@Define59,@Define60)";

        //        //    for (int i = 0; i < lstUnLoad.Count; i++) {
        //        //        var model = lstUnLoad[i];
        //        //        int dbRow = newCon.Execute(insCmd, model);
        //        //        if (dbRow > 0) {
        //        //            string delCmd = $"delete from BatteryInfo_UnLoading where Iden ={model.Iden}";
        //        //            con.Execute(delCmd);
        //        //        }
        //        //        //   Common.SetRichText(rtx, string.Format("10-{0}.ID:[{1}],条码[{2}],时间[{3}]正在转移第[{4}/{5}]行数据,转移结果:{6}", i, model.Iden, model.BarCode, model.CreateDate, i, lstUnLoad.Count, dbRow));
        //        //        waitFrm.SetCaption(string.Format(Common.GetMessage("STRING_DATABASE_BACKUP_12"), i, model.Iden, model.BarCode, model.CreateDate, i, lstUnLoad.Count, dbRow));
        //        //    }
        //        //}
        //        sw.Stop();
        //        switch (Setting.DBConType)
        //        {
        //            case Setting.DbType.MSSQL:

        //                string cmdSql = $@"
        //                DBCC SHRINKDATABASE('{arrConDb[1]}');
        //                USE [master]   ;
        //                ALTER DATABASE {arrConDb[1]} SET RECOVERY SIMPLE WITH NO_WAIT ;
        //                ALTER DATABASE {arrConDb[1]} SET RECOVERY SIMPLE ;

        //                USE {arrConDb[1]};
        //                DBCC SHRINKFILE ('{arrConDb[1]}_Log', 11, TRUNCATEONLY);

        //                USE [master]  ;
        //                ALTER DATABASE {arrConDb[1]} SET RECOVERY FULL WITH NO_WAIT ;
        //                ALTER DATABASE {arrConDb[1]} SET RECOVERY FULL ;";

        //                con.Execute(cmdSql, null, null, 216000);

        //                break;
        //        }

        //        swTotal.Stop();
        //        waitFrm.SetCaption(string.Format(Common.GetMessage("STRING_DATABASE_BACKUP_13"), $"{_insRow}/{rowCount}", now, sw.Elapsed, swTotal.Elapsed));
        //        Common.SetRichText(rtx, string.Format(Common.GetMessage("STRING_DATABASE_BACKUP_13"), $"{_insRow}/{rowCount}", now, sw.Elapsed, swTotal.Elapsed));

        //    }
        //    catch (Exception ex)
        //    {
        //        errMsg = ex.Message;
        //        Common.SetRichText(rtx, string.Format(Common.GetMessage("STRING_DATABASE_BACKUP_14"), errMsg));
        //    }
        //    finally
        //    {
        //        if (con != null)
        //            con.Close();
        //        if (newCon != null)
        //            newCon.Close();
        //    }
        //}

    }
}
