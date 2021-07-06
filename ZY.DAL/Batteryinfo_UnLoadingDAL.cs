﻿    using Esquel.Utility;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZY.Model;
using ZY.Systems;

namespace ZY.DAL
{
    public partial class Batteryinfo_UnLoadingDAL
    {
        private string conStr = string.Empty;
        private DBHelper helper = null;
        public Batteryinfo_UnLoadingDAL(string serverName)
        {
            string serverIP = ConfigurationManager.AppSettings["connectionString"];
            conStr = string.Format(conStr, (string.IsNullOrEmpty(serverIP) || serverIP.Equals(".")) ? "127.0.0.1" : serverIP);
        }
        public Batteryinfo_UnLoadingDAL()
        {
            string serverIP = ConfigurationManager.AppSettings["connectionString"];
            conStr = string.Format(conStr, (string.IsNullOrEmpty(serverIP) || serverIP.Equals(".")) ? "127.0.0.1" : serverIP);
        }

        /// <summary>
        /// 判断数据是否存在
        /// </summary>
        /// <param name="filters">查询条件</param>
        /// <param name="errMsg">错误信息</param>
        /// <returns></returns>
        public bool IsExists(string filters, ref string errMsg)
        {
            errMsg = string.Empty;
            StringBuilder updCmd = new StringBuilder();
            try
            {

                updCmd.Append("select count(1) from Batteryinfo_UnLoading ");
                updCmd.AppendFormat(" where {0} ", filters);
                helper = DBHelperFactory.CreateHelperFactory(Common.ConnectionString);
                var obj = helper.ExecuteScalar(updCmd.ToString());
                return Convert.ToInt32(obj) > 0 ? true : false;
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
            }
            return false;
        }


        /// <summary>
        /// 刷新数据
        /// </summary>
        /// <param name="filters">查询条件</param>
        /// <param name="pageSize">页面大小</param>
        /// <param name="pageIndex">当前页</param>
        /// <param name="rCount">总页数</param>
        /// <param name="errMsg">错误信息</param>
        /// <returns></returns>
        public DataTable GetDataTable(string filters, int pageSize, int pageIndex, ref int rCount, ref string errMsg)
        {
            errMsg = string.Empty;
            if (string.IsNullOrEmpty(filters)) filters = " 1=1";
            StringBuilder selCmd = new StringBuilder();
            try
            {

                if (pageSize != -1)
                {
                    helper = DBHelperFactory.CreateHelperFactory(Common.ConnectionString);
                    object objRecord = helper.ExecuteScalar(string.Format("select count(1) from Batteryinfo_UnLoading where {0} ", filters));
                    rCount = Convert.ToInt32(objRecord);
                }
                if (pageSize != -1)
                {
                    selCmd.Append("SELECT ");
                    selCmd.Append("Iden,Iden_Loading,CreateDate,BatteryNo,Barcode,Define1,Define2,Define3,Define4,Define5,Define6,Define7,Define8,Define9,Define10,Define11,Define12,Define13,Define14,Define15,Define16,Define17,Define18,Define19,Define20,Define21,Define22,Define23,Define24,Define25,Define26,Define27,Define28,Define29,Define30,Define31,Define32,Define33,Define34,Define35,Define36,Define37,Define38,Define39,Define40,Define41,Define42,Define43,Define44,Define45,Define46,Define47,Define48,Define49,Define50,Define51,Define52,Define53,Define54,Define55,Define56,Define57,Define58,Define59,Define60,Define61,Define62,Define63,Define64,Define65,Define66,Define67,Define68,Define69,Define70,Define71,Define72,Define73,Define74,Define75,Define76,Define77,Define78,Define79,Define80,Define1_Reason,Define2_Reason,Define3_Reason,Define4_Reason,Define5_Reason,Define6_Reason,Define7_Reason,Define8_Reason,Define9_Reason,Define10_Reason");
                    selCmd.Append(" FROM Batteryinfo_UnLoading ");
                    selCmd.AppendFormat(" where {0} and Iden >= (select Iden from Batteryinfo_UnLoading where {0} order by Iden LIMIT {1},1) LIMIT {2};", filters, pageSize * pageIndex, pageSize);
                }
                else
                {
                    selCmd.Append("SELECT ");
                    selCmd.Append("Iden,Iden_Loading,CreateDate,BatteryNo,Barcode,Define1,Define2,Define3,Define4,Define5,Define6,Define7,Define8,Define9,Define10,Define11,Define12,Define13,Define14,Define15,Define16,Define17,Define18,Define19,Define20,Define21,Define22,Define23,Define24,Define25,Define26,Define27,Define28,Define29,Define30,Define31,Define32,Define33,Define34,Define35,Define36,Define37,Define38,Define39,Define40,Define41,Define42,Define43,Define44,Define45,Define46,Define47,Define48,Define49,Define50,Define51,Define52,Define53,Define54,Define55,Define56,Define57,Define58,Define59,Define60,Define61,Define62,Define63,Define64,Define65,Define66,Define67,Define68,Define69,Define70,Define71,Define72,Define73,Define74,Define75,Define76,Define77,Define78,Define79,Define80,Define1_Reason,Define2_Reason,Define3_Reason,Define4_Reason,Define5_Reason,Define6_Reason,Define7_Reason,Define8_Reason,Define9_Reason,Define10_Reason");
                    selCmd.Append(" FROM Batteryinfo_UnLoading ");
                    selCmd.AppendFormat(" where {0} ", filters);
                }

                helper = DBHelperFactory.CreateHelperFactory(Common.ConnectionString);
                return helper.ExecuteDataTable(selCmd.ToString());
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
            }
            return null;
        }

        /// <summary>
        /// 刷新数据
        /// </summary>
        /// <param name="filters">查询条件</param>
        /// <param name="errMsg">错误信息</param>
        /// <returns></returns>
        public DataTable GetDataTable(string filters, ref string errMsg)
        {
            int rCount = 0;
            return GetDataTable(filters, -1, -1, ref rCount, ref errMsg);
        }


        /// <summary>
        /// 刷新数据
        /// </summary>
        /// <param name="filters">查询条件</param>
        /// <param name="orderBy">字段排序</param>
        /// <param name="errMsg">错误信息</param>
        /// <returns></returns>
        public IList<Batteryinfo_UnLoading> GetList(string filters, string orderBy, int pageSize, int pageIndex, ref string errMsg)
        {
            return GetList(filters, orderBy, -1, pageSize, pageIndex, ref errMsg);
        }

        /// <summary>
        /// 刷新数据
        /// </summary>
        /// <param name="filters">查询条件</param>
        /// <param name="TopNo">Top 前几条</param>
        /// <param name="errMsg">错误信息</param>
        /// <returns></returns>
        public IList<Batteryinfo_UnLoading> GetList(string filters, int pageSize, int pageIndex, ref string errMsg)
        {
            return GetList(filters, string.Empty, -1, pageSize, pageIndex, ref errMsg);
        }


        /// <summary>
        /// 刷新数据
        /// </summary>
        /// <param name="filters">查询条件</param>
        /// <param name="orderBy">字段排序</param>
        /// <param name="TopNo">前几行</param>
        /// <param name="errMsg">错误信息</param>
        /// <returns></returns>
        public IList<Batteryinfo_UnLoading> GetList(string filters, string orderBy, int TopNo, ref string errMsg)
        {
            return GetList(filters, orderBy, TopNo, -1, -1, ref errMsg);
        }

        /// <summary>
        /// 自定义排序字段进行刷新数据
        /// </summary>
        /// <param name="filters">查询条件</param>
        /// <param name="orderBy">字段排序</param>
        /// <param name="errMsg">错误信息</param>
        /// <returns></returns>
        public IList<Batteryinfo_UnLoading> GetList(string filters, string orderBy, ref string errMsg)
        {
            return GetList(filters, orderBy, -1, -1, -1, ref errMsg);
        }
        /// <summary>
        /// 刷新数据
        /// </summary>
        /// <param name="filters">查询条件</param>
        /// <param name="TopNo">Top 前几条</param>
        /// <param name="errMsg">错误信息</param>
        /// <returns></returns>
        public IList<Batteryinfo_UnLoading> GetList(string filters, int TopNo, ref string errMsg)
        {
            return GetList(filters, string.Empty, TopNo, -1, -1, ref errMsg);
        }

        /// <summary>
        /// 刷新数据
        /// </summary>
        /// <param name="filters">查询条件</param>
        /// <param name="errMsg">错误信息</param>
        /// <returns></returns>
        public IList<Batteryinfo_UnLoading> GetList(string filters, ref string errMsg)
        {
            return GetList(filters, string.Empty, -1, -1, -1, ref errMsg);
        }



        /// <summary>
        /// 刷新数据
        /// </summary>
        /// <param name="filters">查询条件</param>
        /// <param name="orderBy">字段排序</param>
        /// <param name="TopNo">前几行</param>
        /// <param name="pageSize">页面大小</param>
        /// <param name="pageIndex">当前页</param>
        /// <param name="rCount">总页数</param>
        /// <param name="errMsg">错误信息</param>
        /// <returns></returns>
        public IList<Batteryinfo_UnLoading> GetList(string filters, string orderBy, int TopNo, int pageSize, int pageIndex, ref string errMsg)
        {
            errMsg = string.Empty;
            Batteryinfo_UnLoading model = new Batteryinfo_UnLoading();
            if (string.IsNullOrEmpty(filters)) filters = " 1=1";
            StringBuilder selCmd = new StringBuilder();
            IList<Batteryinfo_UnLoading> list = new List<Batteryinfo_UnLoading>();
            try
            {

                if (pageSize != -1)
                {
                    selCmd.Append("SELECT ");
                    selCmd.Append("Iden,TranfIden,CreateDate,EquipmentNo,No,ProductType,ProductLineNo,Shift,EndProductNo,LoadPalletBarCode,LoadPalletDate,BarCode,BarCodeDate,ScanCodePass,ScanDefectCode,InjectWeight,InjectFlag,BeforeWeighCode,BeforeWeight,BeforeFlag,BeforeDefectCode,Weigh_LSL,Weigh_USL,IsUnLoading,UnLoadingDate,UnLoadingReason,CreateUser,IsDelete,DeleteDate,DeleteUser,IsUpload,UploadDate,UploadState,UploadNum,UploadLog,Remark,CreateTime,ThicknessPressure,ThicknessTemp,ThicknessDuration,ThicknessTest,ThicknessSetTest,ThicknessFlag,ThicknessPressure1,ThicknessTemp1,ThicknessDuration1,ThicknessTest1,ThicknessSetTest1,ThicknessFlag1,ThicknessPressure2,ThicknessTemp2,ThicknessDuration2,ThicknessTest2,ThicknessSetTest2,ThicknessFlag2,ThicknessPressure3,ThicknessTemp3,ThicknessDuration3,ThicknessTest3,ThicknessSetTest3,ThicknessFlag3,ThicknessPressure4,ThicknessTemp4,ThicknessDuration4,ThicknessTest4,ThicknessSetTest4,ThicknessFlag4,ThicknessPressure5,ThicknessTemp5,ThicknessDuration5,ThicknessTest5,ThicknessSetTest5,ThicknessFlag5,TabRIAnodeTest,TabRIAnodeFlag,TabRIAnode_USL,TabRIAnode_LSL,TabRIAnodeDuration,TabRIAnodeSetTest,TabRICathodeTest,TabRICathodeFlag,TabRICathode_USL,TabRICathode_LSL,TabRICathodeDuration,TabRICathodeSetTest,DegasType,ThicknessTest_1,ThicknessTest_2,ThicknessTest_3,ThicknessTest1_1,ThicknessTest1_2,ThicknessTest1_3,ThicknessTest2_1,ThicknessTest2_2,ThicknessTest2_3,ThicknessTest3_1,ThicknessTest3_2,ThicknessTest3_3,ThicknessTest4_1,ThicknessTest4_2,ThicknessTest4_3,ThicknessTest5_1,ThicknessTest5_2,ThicknessTest5_3,InjBeforeWeight,InjAfterWeight,ThicknessTest_USL,ThicknessTest_LSL,Define1,Define2,Define3,Define4,Define5,Define6,Define7,Define8,Define9,Define10,Define11,Define12,Define13,Define14,Define15,Define16,Define17,Define18,Define19,Define20,Define21,Define22,Define23,Define24,Define25,Define26,Define27,Define28,Define29,Define30,Define31,Define32,Define33,Define34,Define35,Define36,Define37,Define38,Define39,Define40,Define41,Define42,Define43,Define44,Define45,Define46,Define47,Define48,Define49,Define50,TKNGNumCheck,InjectRetention_LSL,InjectRetention_USL");
                    selCmd.Append(" FROM  Batteryinfo_UnLoading  ");
                    selCmd.AppendFormat(" where {0} and Iden >= (select Iden from Batteryinfo_UnLoading  where {0} order by {1} desc LIMIT {2},1) LIMIT {3};", filters, (string.IsNullOrEmpty(orderBy) ? "Iden" : orderBy), pageSize * pageIndex, pageSize);
                    selCmd.AppendFormat("SELECT count(1) as TotalCount from Batteryinfo_UnLoading where {0};", filters);
                }
                else
                {
                    selCmd.Append("SELECT ");
                    selCmd.Append("Iden,TranfIden,CreateDate,EquipmentNo,No,ProductType,ProductLineNo,Shift,EndProductNo,LoadPalletBarCode,LoadPalletDate,BarCode,BarCodeDate,ScanCodePass,ScanDefectCode,InjectWeight,InjectFlag,BeforeWeighCode,BeforeWeight,BeforeFlag,BeforeDefectCode,Weigh_LSL,Weigh_USL,IsUnLoading,UnLoadingDate,UnLoadingReason,CreateUser,IsDelete,DeleteDate,DeleteUser,IsUpload,UploadDate,UploadState,UploadNum,UploadLog,Remark,CreateTime,ThicknessPressure,ThicknessTemp,ThicknessDuration,ThicknessTest,ThicknessSetTest,ThicknessFlag,ThicknessPressure1,ThicknessTemp1,ThicknessDuration1,ThicknessTest1,ThicknessSetTest1,ThicknessFlag1,ThicknessPressure2,ThicknessTemp2,ThicknessDuration2,ThicknessTest2,ThicknessSetTest2,ThicknessFlag2,ThicknessPressure3,ThicknessTemp3,ThicknessDuration3,ThicknessTest3,ThicknessSetTest3,ThicknessFlag3,ThicknessPressure4,ThicknessTemp4,ThicknessDuration4,ThicknessTest4,ThicknessSetTest4,ThicknessFlag4,ThicknessPressure5,ThicknessTemp5,ThicknessDuration5,ThicknessTest5,ThicknessSetTest5,ThicknessFlag5,TabRIAnodeTest,TabRIAnodeFlag,TabRIAnode_USL,TabRIAnode_LSL,TabRIAnodeDuration,TabRIAnodeSetTest,TabRICathodeTest,TabRICathodeFlag,TabRICathode_USL,TabRICathode_LSL,TabRICathodeDuration,TabRICathodeSetTest,DegasType,ThicknessTest_1,ThicknessTest_2,ThicknessTest_3,ThicknessTest1_1,ThicknessTest1_2,ThicknessTest1_3,ThicknessTest2_1,ThicknessTest2_2,ThicknessTest2_3,ThicknessTest3_1,ThicknessTest3_2,ThicknessTest3_3,ThicknessTest4_1,ThicknessTest4_2,ThicknessTest4_3,ThicknessTest5_1,ThicknessTest5_2,ThicknessTest5_3,InjBeforeWeight,InjAfterWeight,ThicknessTest_USL,ThicknessTest_LSL,Define1,Define2,Define3,Define4,Define5,Define6,Define7,Define8,Define9,Define10,Define11,Define12,Define13,Define14,Define15,Define16,Define17,Define18,Define19,Define20,Define21,Define22,Define23,Define24,Define25,Define26,Define27,Define28,Define29,Define30,Define31,Define32,Define33,Define34,Define35,Define36,Define37,Define38,Define39,Define40,Define41,Define42,Define43,Define44,Define45,Define46,Define47,Define48,Define49,Define50,TKNGNumCheck,InjectRetention_LSL,InjectRetention_USL");
                    selCmd.Append(" FROM Batteryinfo_UnLoading  ");
                    selCmd.AppendFormat(" where {0} order by {1} ", filters, (string.IsNullOrEmpty(orderBy) ? "Iden" : orderBy));
                }
                helper = DBHelperFactory.CreateHelperFactory(Common.ConnectionString);
                list = helper.SelectReader<Batteryinfo_UnLoading>(selCmd.ToString());
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
            }
            return list;
        }


        /// <summary>
        /// 新增数据
        /// </summary>
        /// <param name="model">实体类</param>
        /// <param name="helper">数据库类</param>
        /// <param name="isTran">是否为事务</param>
        /// <param name="errMsg">错误信息</param>
        /// <returns></returns>
        public long Add(Batteryinfo_UnLoading model, DBHelper helper, bool isTran, ref string errMsg)
        {
            errMsg = string.Empty;
            StringBuilder insCmd = new StringBuilder();
            insCmd.AppendFormat("insert into Batteryinfo_UnLoading");
            insCmd.Append("(Iden_Loading,CreateDate,BatteryNo,Barcode,Define1,Define2,Define3,Define4,Define5,Define6,Define7,Define8,Define9,Define10,Define11,Define12,Define13,Define14,Define15,Define16,Define17,Define18,Define19,Define20,Define21,Define22,Define23,Define24,Define25,Define26,Define27,Define28,Define29,Define30,Define31,Define32,Define33,Define34,Define35,Define36,Define37,Define38,Define39,Define40,Define41,Define42,Define43,Define44,Define45,Define46,Define47,Define48,Define49,Define50,Define51,Define52,Define53,Define54,Define55,Define56,Define57,Define58,Define59,Define60,Define61,Define62,Define63,Define64,Define65,Define66,Define67,Define68,Define69,Define70,Define71,Define72,Define73,Define74,Define75,Define76,Define77,Define78,Define79,Define80,Define1_Reason,Define2_Reason,Define3_Reason,Define4_Reason,Define5_Reason,Define6_Reason,Define7_Reason,Define8_Reason,Define9_Reason,Define10_Reason)");
            insCmd.Append("VALUES(@Iden_Loading,@CreateDate,@BatteryNo,@Barcode,@Define1,@Define2,@Define3,@Define4,@Define5,@Define6,@Define7,@Define8,@Define9,@Define10,@Define11,@Define12,@Define13,@Define14,@Define15,@Define16,@Define17,@Define18,@Define19,@Define20,@Define21,@Define22,@Define23,@Define24,@Define25,@Define26,@Define27,@Define28,@Define29,@Define30,@Define31,@Define32,@Define33,@Define34,@Define35,@Define36,@Define37,@Define38,@Define39,@Define40,@Define41,@Define42,@Define43,@Define44,@Define45,@Define46,@Define47,@Define48,@Define49,@Define50,@Define51,@Define52,@Define53,@Define54,@Define55,@Define56,@Define57,@Define58,@Define59,@Define60,@Define61,@Define62,@Define63,@Define64,@Define65,@Define66,@Define67,@Define68,@Define69,@Define70,@Define71,@Define72,@Define73,@Define74,@Define75,@Define76,@Define77,@Define78,@Define79,@Define80,@Define1_Reason,@Define2_Reason,@Define3_Reason,@Define4_Reason,@Define5_Reason,@Define6_Reason,@Define7_Reason,@Define8_Reason,@Define9_Reason,@Define10_Reason)");
            insCmd.Append(";select @@IDENTITY");
            int row = -1;
            object obj;
            try
            {
                if (helper == null)
                    helper = DBHelperFactory.CreateHelperFactory(Common.ConnectionString);
                obj = helper.ExecuteScalar(insCmd.ToString(), isTran, model);
                return Convert.ToInt64(obj);
            }
            catch (Exception ex)
            {
                errMsg = ex.Message.Replace("'", string.Empty);
            }
            return row;
        }

        /// <summary>
        /// 新增数据
        /// </summary>
        /// <param name="model">实体类</param>
        /// <param name="errMsg">错误信息</param>
        /// <returns></returns>
        public long Add(Batteryinfo_UnLoading model, ref string errMsg)
        {
            return Add(model, null, false, ref errMsg);
        }


        /// <summary>
        /// 修改数据
        /// </summary>
        /// <param name="model">实体类</param>
        /// <param name="helper">数据库类</param>
        /// <param name="isTran">是否为事务</param>
        /// <param name="errMsg">错误信息</param>
        /// <returns></returns>
        public bool Update(Batteryinfo_UnLoading model, DBHelper helper, bool isTran, ref string errMsg)
        {
            errMsg = string.Empty;
            StringBuilder updCmd = new StringBuilder();
            updCmd.AppendFormat("Update Batteryinfo_UnLoading Set ");
            updCmd.Append("Iden_Loading=@Iden_Loading,");
            updCmd.Append("CreateDate=@CreateDate,");
            updCmd.Append("BatteryNo=@BatteryNo,");
            updCmd.Append("Barcode=@Barcode,");
            updCmd.Append("Define1=@Define1,");
            updCmd.Append("Define2=@Define2,");
            updCmd.Append("Define3=@Define3,");
            updCmd.Append("Define4=@Define4,");
            updCmd.Append("Define5=@Define5,");
            updCmd.Append("Define6=@Define6,");
            updCmd.Append("Define7=@Define7,");
            updCmd.Append("Define8=@Define8,");
            updCmd.Append("Define9=@Define9,");
            updCmd.Append("Define10=@Define10,");
            updCmd.Append("Define11=@Define11,");
            updCmd.Append("Define12=@Define12,");
            updCmd.Append("Define13=@Define13,");
            updCmd.Append("Define14=@Define14,");
            updCmd.Append("Define15=@Define15,");
            updCmd.Append("Define16=@Define16,");
            updCmd.Append("Define17=@Define17,");
            updCmd.Append("Define18=@Define18,");
            updCmd.Append("Define19=@Define19,");
            updCmd.Append("Define20=@Define20,");
            updCmd.Append("Define21=@Define21,");
            updCmd.Append("Define22=@Define22,");
            updCmd.Append("Define23=@Define23,");
            updCmd.Append("Define24=@Define24,");
            updCmd.Append("Define25=@Define25,");
            updCmd.Append("Define26=@Define26,");
            updCmd.Append("Define27=@Define27,");
            updCmd.Append("Define28=@Define28,");
            updCmd.Append("Define29=@Define29,");
            updCmd.Append("Define30=@Define30,");
            updCmd.Append("Define31=@Define31,");
            updCmd.Append("Define32=@Define32,");
            updCmd.Append("Define33=@Define33,");
            updCmd.Append("Define34=@Define34,");
            updCmd.Append("Define35=@Define35,");
            updCmd.Append("Define36=@Define36,");
            updCmd.Append("Define37=@Define37,");
            updCmd.Append("Define38=@Define38,");
            updCmd.Append("Define39=@Define39,");
            updCmd.Append("Define40=@Define40,");
            updCmd.Append("Define41=@Define41,");
            updCmd.Append("Define42=@Define42,");
            updCmd.Append("Define43=@Define43,");
            updCmd.Append("Define44=@Define44,");
            updCmd.Append("Define45=@Define45,");
            updCmd.Append("Define46=@Define46,");
            updCmd.Append("Define47=@Define47,");
            updCmd.Append("Define48=@Define48,");
            updCmd.Append("Define49=@Define49,");
            updCmd.Append("Define50=@Define50,");
            updCmd.Append("Define51=@Define51,");
            updCmd.Append("Define52=@Define52,");
            updCmd.Append("Define53=@Define53,");
            updCmd.Append("Define54=@Define54,");
            updCmd.Append("Define55=@Define55,");
            updCmd.Append("Define56=@Define56,");
            updCmd.Append("Define57=@Define57,");
            updCmd.Append("Define58=@Define58,");
            updCmd.Append("Define59=@Define59,");
            updCmd.Append("Define60=@Define60,");
            updCmd.Append("Define61=@Define61,");
            updCmd.Append("Define62=@Define62,");
            updCmd.Append("Define63=@Define63,");
            updCmd.Append("Define64=@Define64,");
            updCmd.Append("Define65=@Define65,");
            updCmd.Append("Define66=@Define66,");
            updCmd.Append("Define67=@Define67,");
            updCmd.Append("Define68=@Define68,");
            updCmd.Append("Define69=@Define69,");
            updCmd.Append("Define70=@Define70,");
            updCmd.Append("Define71=@Define71,");
            updCmd.Append("Define72=@Define72,");
            updCmd.Append("Define73=@Define73,");
            updCmd.Append("Define74=@Define74,");
            updCmd.Append("Define75=@Define75,");
            updCmd.Append("Define76=@Define76,");
            updCmd.Append("Define77=@Define77,");
            updCmd.Append("Define78=@Define78,");
            updCmd.Append("Define79=@Define79,");
            updCmd.Append("Define80=@Define80,");
            updCmd.Append("Define1_Reason=@Define1_Reason,");
            updCmd.Append("Define2_Reason=@Define2_Reason,");
            updCmd.Append("Define3_Reason=@Define3_Reason,");
            updCmd.Append("Define4_Reason=@Define4_Reason,");
            updCmd.Append("Define5_Reason=@Define5_Reason,");
            updCmd.Append("Define6_Reason=@Define6_Reason,");
            updCmd.Append("Define7_Reason=@Define7_Reason,");
            updCmd.Append("Define8_Reason=@Define8_Reason,");
            updCmd.Append("Define9_Reason=@Define9_Reason,");
            updCmd.Append("Define10_Reason=@Define10_Reason");
            updCmd.Append(" where Iden=@Iden");

            int row = -1;
            try
            {
                if (helper == null)
                    helper = DBHelperFactory.CreateHelperFactory(Common.ConnectionString);
                row = helper.ExecuteNonQuery(updCmd.ToString(), isTran, model);
            }
            catch (Exception ex)
            {
                errMsg = ex.Message.Replace("'", string.Empty);
                return false;
            }
            return true;
        }
        /// <summary>
        /// 修改数据
        /// </summary>
        /// <param name="model">实体类</param>
        /// <param name="errMsg">错误信息</param>
        /// <returns></returns>
        public bool Update(Batteryinfo_UnLoading model, ref string errMsg)
        {
            return Update(model, null, false, ref errMsg);
        }


        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="model">实体类</param>
        /// <param name="helper">数据库类</param>
        /// <param name="isTran">是否为事务</param>
        /// <param name="errMsg">错误信息</param>
        /// <returns></returns>
        public bool Delete(Batteryinfo_UnLoading model, DBHelper helper, bool isTran, ref string errMsg)
        {
            errMsg = string.Empty;
            StringBuilder delCmd = new StringBuilder();
            delCmd.AppendFormat(" Delete from Batteryinfo_UnLoading where ");
            delCmd.Append("Iden=@Iden");


            int row = -1;
            try
            {
                if (helper == null)
                    helper = DBHelperFactory.CreateHelperFactory(Common.ConnectionString);
                row = helper.ExecuteNonQuery(delCmd.ToString(), isTran, model);
            }
            catch (Exception ex)
            {
                errMsg = ex.Message.Replace("'", string.Empty);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="model">实体类</param>
        /// <param name="errMsg">错误信息</param>
        /// <returns></returns>
        public bool Delete(Batteryinfo_UnLoading model, ref string errMsg)
        {
            return Delete(model, null, false, ref errMsg);
        }


        /// <summary>
        /// 修改数据
        /// </summary>
        /// <param name="fileds">需要更新的表达式</param>
        /// <param name="filters">更新条件</param>
        /// <param name="errMsg">错误信息</param>
        /// <returns></returns>
        public bool UpdateFiledsByFilters(string fileds, string filters, ref string errMsg)
        {
            errMsg = string.Empty;
            StringBuilder updCmd = new StringBuilder();
            updCmd.AppendFormat("Update Batteryinfo_UnLoading set {0} where {1} ", fileds, filters);
            try
            {
                helper = DBHelperFactory.CreateHelperFactory(Common.ConnectionString);
                helper.ExecuteNonQuery(updCmd.ToString());
            }
            catch (Exception ex)
            {
                errMsg = ex.Message.Replace("'", string.Empty);
            }
            return false;
        }


        /// <summary>
        /// 查询数据
        /// </summary>
        /// <param name="fileds">需要更新的表达式</param>
        /// <param name="filters">更新条件</param>
        /// <param name="errMsg">错误信息</param>
        /// <returns></returns>
        public string SelectFiledsByFilters(string filedName, string filters, ref string errMsg)
        {
            errMsg = string.Empty;
            StringBuilder updCmd = new StringBuilder();
            try
            {
                updCmd.AppendFormat("select {0} from Batteryinfo_UnLoading where {1} LIMIT 1; ", filedName, filters);
                helper = DBHelperFactory.CreateHelperFactory(Common.ConnectionString);
                var obj = helper.ExecuteScalar(updCmd.ToString());
                if (!Convert.IsDBNull(obj) && !string.IsNullOrEmpty(Convert.ToString(obj)))
                {
                    return Convert.ToString(obj);
                }
            }
            catch (Exception ex)
            {
                errMsg = ex.Message.Replace("'", string.Empty);
            }
            return string.Empty;
        }


        /// <summary>
        /// 修改数据
        /// </summary>
        /// <param name="fileds">需要更新的表达式</param>
        /// <param name="filters">更新条件</param>
        /// <param name="errMsg">错误信息</param>
        /// <returns></returns>
        public bool UpdateSQL(string sql, ref string errMsg)
        {
            errMsg = string.Empty;
            try
            {
                helper = DBHelperFactory.CreateHelperFactory(Common.ConnectionString);
                helper.ExecuteNonQuery(sql);
                return true;
            }
            catch (Exception ex)
            {
                errMsg = ex.Message.Replace("'", string.Empty);
            }
            return false;
        }


        /// <summary>
        /// 修改数据
        /// </summary>
        /// <param name="fileds">需要更新的表达式</param>
        /// <param name="filters">更新条件</param>
        /// <param name="errMsg">错误信息</param>
        /// <returns></returns>
        public bool ExecuteSQL(string sql, ref string errMsg)
        {
            return ExecuteSQL(sql, null, false, ref errMsg);
        }


        /// <summary>
        /// 修改数据
        /// </summary>
        /// <param name="fileds">需要更新的表达式</param>
        /// <param name="filters">更新条件</param>
        /// <param name="errMsg">错误信息</param>
        /// <returns></returns>
        public bool ExecuteSQL(string sql, DBHelper helper, bool isTran, ref string errMsg)
        {
            errMsg = string.Empty;
            try
            {
                if (helper == null)
                    helper = DBHelperFactory.CreateHelperFactory(Common.ConnectionString);
                helper.ExecuteNonQuery(sql, isTran);
                return true;
            }
            catch (Exception ex)
            {
                errMsg = ex.Message.Replace("'", string.Empty);
            }
            return false;
        }

    }
}
