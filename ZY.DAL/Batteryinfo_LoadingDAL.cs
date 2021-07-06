using Esquel.Utility;
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
    public partial class Batteryinfo_LoadingDAL
    {
        private string conStr = string.Empty;

        private DBHelper helper = null;
        public Batteryinfo_LoadingDAL(string serverName)
        {
            string serverIP = ConfigurationManager.AppSettings["connectionString"];
            conStr = string.Format(conStr, (string.IsNullOrEmpty(serverIP) || serverIP.Equals(".")) ? "127.0.0.1" : serverIP);
        }
        public Batteryinfo_LoadingDAL()
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
        public DataTable IsExists(string filters, ref string errMsg)
        {
            DataTable dt =null;
            errMsg = string.Empty;
            StringBuilder updCmd = new StringBuilder();

            try
            {
                updCmd.Append("select NGreason from production_data ");
                updCmd.AppendFormat(" where ProductSN= '{0}' ", filters);
                helper = DBHelperFactory.CreateHelperFactory(Common.ConnectionString);
                dt = helper.ExecuteDataTable(updCmd.ToString());
                return dt;
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
                    object objRecord = helper.ExecuteScalar(string.Format("select count(1) from Batteryinfo_Loading where {0} ", filters));
                    rCount = Convert.ToInt32(objRecord);
                }
                if (pageSize != -1)
                {
                    selCmd.Append("SELECT ");
                    selCmd.Append("Iden,MachineNo,BatteryNo,ProductType,Barcode,BarcodeFlag,CreateDate,ThicknessTest,ThicknessChannel,ThicknessFlag,Voltage,Resistance,Temperature,Kvalue,AmbientTemp,OCVChannel,OCVFlag,OCVNGReason,IVValue1,IVValue2,IVValue3,IVValue4,IVValue5,IVValue6,IVChannel,Conduction,ConductionFlag,ConductionReason,IsUpload,UploadDate,UploadNum,UploadLog,IsUnLoading,UnLoadingDate,UnLoadingReason");
                    selCmd.Append(" FROM Batteryinfo_Loading ");
                    selCmd.AppendFormat(" where {0} and Iden >= (select Iden from Batteryinfo_Loading where {0} order by Iden LIMIT {1},1) LIMIT {2};", filters, pageSize * pageIndex, pageSize);
                }
                else
                {
                    selCmd.Append("SELECT ");
                    selCmd.Append("Iden,MachineNo,BatteryNo,ProductType,Barcode,BarcodeFlag,CreateDate,ThicknessTest,ThicknessChannel,ThicknessFlag,Voltage,Resistance,Temperature,Kvalue,AmbientTemp,OCVChannel,OCVFlag,OCVNGReason,IVValue1,IVValue2,IVValue3,IVValue4,IVValue5,IVValue6,IVChannel,Conduction,ConductionFlag,ConductionReason,IsUpload,UploadDate,UploadNum,UploadLog,IsUnLoading,UnLoadingDate,UnLoadingReason");
                    selCmd.Append(" FROM Batteryinfo_Loading ");
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
        public IList<Batteryinfo_Loading> GetList(string filters, string orderBy, int pageSize, int pageIndex, ref string errMsg)
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
        public IList<Batteryinfo_Loading> GetList(string filters, int pageSize, int pageIndex, ref string errMsg)
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
        public IList<Batteryinfo_Loading> GetList(string filters, string orderBy, int TopNo, ref string errMsg)
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
        public IList<Batteryinfo_Loading> GetList(string filters, string orderBy, ref string errMsg)
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
        public IList<Batteryinfo_Loading> GetList(string filters, int TopNo, ref string errMsg)
        {
            return GetList(filters, string.Empty, TopNo, -1, -1, ref errMsg);
        }

        /// <summary>
        /// 刷新数据
        /// </summary>
        /// <param name="filters">查询条件</param>
        /// <param name="errMsg">错误信息</param>
        /// <returns></returns>
        public IList<Batteryinfo_Loading> GetList(string filters, ref string errMsg)
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
        public IList<Batteryinfo_Loading> GetList(string filters, string orderBy, int TopNo, int pageSize, int pageIndex, ref string errMsg)
        {
            errMsg = string.Empty;
            Batteryinfo_Loading model = new Batteryinfo_Loading();
            if (string.IsNullOrEmpty(filters)) filters = " 1=1";
            StringBuilder selCmd = new StringBuilder();
            IList<Batteryinfo_Loading> list = new List<Batteryinfo_Loading>();
            try
            {
                if (pageSize != -1)
                {
                    selCmd.Append("SELECT ");
                    selCmd.Append("Iden,MachineNo,BatteryNo,ProductType,Barcode,BarcodeFlag,CreateDate,ThicknessTest,ThicknessChannel,ThicknessFlag,Voltage,Resistance,Temperature,Kvalue,AmbientTemp,OCVChannel,OCVFlag,OCVNGReason,IVValue1,IVValue2,IVValue3,IVValue4,IVValue5,IVValue6,IVChannel,Conduction,ConductionFlag,ConductionReason,IsUpload,UploadDate,UploadNum,UploadLog,IsUnLoading,UnLoadingDate,UnLoadingReason");
                    selCmd.Append(" FROM  Batteryinfo_Loading  ");
                    selCmd.AppendFormat(" where {0} and Iden >= (select Iden from Batteryinfo_Loading  where {0} order by {1} desc LIMIT {2},1) LIMIT {3};", filters, (string.IsNullOrEmpty(orderBy) ? "Iden" : orderBy), pageSize * pageIndex, pageSize);
                    selCmd.AppendFormat("SELECT count(1) as TotalCount from Batteryinfo_Loading where {0};", filters);
                }
                else
                {
                    selCmd.Append("SELECT ");
                    selCmd.Append("Iden,MachineNo,BatteryNo,ProductType,Barcode,BarcodeFlag,CreateDate,ThicknessTest,ThicknessChannel,ThicknessFlag,Voltage,Resistance,Temperature,Kvalue,AmbientTemp,OCVChannel,OCVFlag,OCVNGReason,IVValue1,IVValue2,IVValue3,IVValue4,IVValue5,IVValue6,IVChannel,Conduction,ConductionFlag,ConductionReason,IsUpload,UploadDate,UploadNum,UploadLog,IsUnLoading,UnLoadingDate,UnLoadingReason");
                    selCmd.Append(" FROM Batteryinfo_Loading  ");
                    selCmd.AppendFormat(" where {0} order by {1} ", filters, (string.IsNullOrEmpty(orderBy) ? "Iden" : orderBy));
                }

                helper = DBHelperFactory.CreateHelperFactory(Common.ConnectionString);
                list = helper.SelectReader<Batteryinfo_Loading>(selCmd.ToString());
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
        public long Add(Batteryinfo_Loading model, DBHelper helper, bool isTran, ref string errMsg)
        {
            errMsg = string.Empty;
            StringBuilder insCmd = new StringBuilder();
            insCmd.AppendFormat("insert into Batteryinfo_Loading");
            insCmd.Append("(MachineNo,BatteryNo,ProductType,Barcode,BarcodeFlag,CreateDate,ThicknessTest,ThicknessChannel,ThicknessFlag,Voltage,Resistance,Temperature,Kvalue,AmbientTemp,OCVChannel,OCVFlag,OCVNGReason,IVValue1,IVValue2,IVValue3,IVValue4,IVValue5,IVValue6,IVChannel,Conduction,ConductionFlag,ConductionReason,IsUpload,UploadDate,UploadNum,UploadLog,IsUnLoading,UnLoadingDate,UnLoadingReason)");
            insCmd.Append("VALUES(@MachineNo,@BatteryNo,@ProductType,@Barcode,@BarcodeFlag,@CreateDate,@ThicknessTest,@ThicknessChannel,@ThicknessFlag,@Voltage,@Resistance,@Temperature,@Kvalue,@AmbientTemp,@OCVChannel,@OCVFlag,@OCVNGReason,@IVValue1,@IVValue2,@IVValue3,@IVValue4,@IVValue5,@IVValue6,@IVChannel,@Conduction,@ConductionFlag,@ConductionReason,@IsUpload,@UploadDate,@UploadNum,@UploadLog,@IsUnLoading,@UnLoadingDate,@UnLoadingReason)");
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
        public long Add(Batteryinfo_Loading model, ref string errMsg)
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
        public bool Update(Batteryinfo_Loading model, DBHelper helper, bool isTran, ref string errMsg)
        {
            errMsg = string.Empty;
            StringBuilder updCmd = new StringBuilder();
            updCmd.AppendFormat("Update Batteryinfo_Loading Set ");
            updCmd.Append("MachineNo=@MachineNo,");
            updCmd.Append("BatteryNo=@BatteryNo,");
            updCmd.Append("ProductType=@ProductType,");
            updCmd.Append("Barcode=@Barcode,");
            updCmd.Append("BarcodeFlag=@BarcodeFlag,");
            updCmd.Append("CreateDate=@CreateDate,");
            updCmd.Append("ThicknessTest=@ThicknessTest,");
            updCmd.Append("ThicknessChannel=@ThicknessChannel,");
            updCmd.Append("ThicknessFlag=@ThicknessFlag,");
            updCmd.Append("Voltage=@Voltage,");
            updCmd.Append("Resistance=@Resistance,");
            updCmd.Append("Temperature=@Temperature,");
            updCmd.Append("Kvalue=@Kvalue,");
            updCmd.Append("AmbientTemp=@AmbientTemp,");
            updCmd.Append("OCVChannel=@OCVChannel,");
            updCmd.Append("OCVFlag=@OCVFlag,");
            updCmd.Append("OCVNGReason=@OCVNGReason,");
            updCmd.Append("IVValue1=@IVValue1,");
            updCmd.Append("IVValue2=@IVValue2,");
            updCmd.Append("IVValue3=@IVValue3,");
            updCmd.Append("IVValue4=@IVValue4,");
            updCmd.Append("IVValue5=@IVValue5,");
            updCmd.Append("IVValue6=@IVValue6,");
            updCmd.Append("IVChannel=@IVChannel,");
            updCmd.Append("Conduction=@Conduction,");
            updCmd.Append("ConductionFlag=@ConductionFlag,");
            updCmd.Append("ConductionReason=@ConductionReason,");
            updCmd.Append("IsUpload=@IsUpload,");
            updCmd.Append("UploadDate=@UploadDate,");
            updCmd.Append("UploadNum=@UploadNum,");
            updCmd.Append("UploadLog=@UploadLog,");
            updCmd.Append("IsUnLoading=@IsUnLoading,");
            updCmd.Append("UnLoadingDate=@UnLoadingDate,");
            updCmd.Append("UnLoadingReason=@UnLoadingReason");
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
        public bool Update(Batteryinfo_Loading model, ref string errMsg)
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
        public bool Delete(Batteryinfo_Loading model, DBHelper helper, bool isTran, ref string errMsg)
        {
            errMsg = string.Empty;
            StringBuilder delCmd = new StringBuilder();
            delCmd.AppendFormat(" Delete from Batteryinfo_Loading where ");
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
        public bool Delete(Batteryinfo_Loading model, ref string errMsg)
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
            updCmd.AppendFormat("Update Batteryinfo_Loading set {0} where {1} ", fileds, filters);
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

                updCmd.AppendFormat("select {0} from Batteryinfo_Loading where {1} LIMIT 1; ", filedName, filters);

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
