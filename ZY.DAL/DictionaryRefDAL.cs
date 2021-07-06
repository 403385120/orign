using Dapper;
using Esquel.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZY.Model;
using ZY.Systems;

namespace ZY.DAL
{
    /// <summary>
    /// DictionaryRefDAL
    /// </summary>
    public partial class DictionaryRefDAL
    {
        private string conStr = string.Empty;
        private DBHelper helper = null;
        public DictionaryRefDAL(string serverName)
        {
            string serverIP = ConfigurationManager.AppSettings["connectionString"];
            conStr = string.Format(conStr, (string.IsNullOrEmpty(serverIP) || serverIP.Equals(".")) ? "127.0.0.1" : serverIP);
        }
        public DictionaryRefDAL()
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
            StringBuilder strSql = new StringBuilder();
            try
            {

                strSql.Append("select count(1) from DictionaryRef ");
                strSql.AppendFormat(" where {0} ", filters);
                helper = DBHelperFactory.CreateHelperFactory(Common.ConnectionString);
                var obj = helper.ExecuteScalar(strSql.ToString());
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
                    object objRecord = helper.ExecuteScalar(string.Format("select count(1) from DictionaryRef where {0} ", filters));
                    rCount = Convert.ToInt32(objRecord);
                }
                if (pageSize != -1)
                {
                    selCmd.Append("SELECT ");
                    selCmd.Append("Iden,RefKey,RefCode,RefCode2,RefValue,RefValue2,RefSystem,RefRemark,RefIsUse");
                    selCmd.Append(" FROM DictionaryRef ");
                    selCmd.AppendFormat(" where {0} and Iden >= (select Iden from DictionaryRef where {0} order by Iden LIMIT {1},1) LIMIT {2};", filters, pageSize * pageIndex, pageSize);
                }
                else
                {
                    selCmd.Append("SELECT ");
                    selCmd.Append("Iden,RefKey,RefCode,RefCode2,RefValue,RefValue2,RefSystem,RefRemark,RefIsUse");
                    selCmd.Append(" FROM DictionaryRef ");
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
        public IList<DictionaryRef> GetList(string filters, string orderBy, int pageSize, int pageIndex, ref string errMsg)
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
        public IList<DictionaryRef> GetList(string filters, int pageSize, int pageIndex, ref string errMsg)
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
        public IList<DictionaryRef> GetList(string filters, string orderBy, int TopNo, ref string errMsg)
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
        public IList<DictionaryRef> GetList(string filters, string orderBy, ref string errMsg)
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
        public IList<DictionaryRef> GetList(string filters, int TopNo, ref string errMsg)
        {
            return GetList(filters, string.Empty, TopNo, -1, -1, ref errMsg);
        }

        /// <summary>
        /// 刷新数据
        /// </summary>
        /// <param name="filters">查询条件</param>
        /// <param name="errMsg">错误信息</param>
        /// <returns></returns>
        public IList<DictionaryRef> GetList(string filters, ref string errMsg)
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
        public IList<DictionaryRef> GetList(string filters, string orderBy, int TopNo, int pageSize, int pageIndex, ref string errMsg)
        {
            errMsg = string.Empty;
            DictionaryRef model = new DictionaryRef();
            if (string.IsNullOrEmpty(filters)) filters = " 1=1";
            StringBuilder selCmd = new StringBuilder();
            IList<DictionaryRef> list = new List<DictionaryRef>();
            try
            {

                if (pageSize != -1)
                {
                    selCmd.Append("SELECT ");
                    selCmd.Append("Iden,RefKey,RefCode,RefCode2,RefValue,RefValue2,RefSystem,RefRemark,RefIsUse");
                    selCmd.Append(" FROM  DictionaryRef  ");
                    selCmd.AppendFormat(" where {0} and Iden >= (select Iden from DictionaryRef  where {0} order by {1} desc LIMIT {2},1) LIMIT {3};", filters, (string.IsNullOrEmpty(orderBy) ? "Iden" : orderBy), pageSize * pageIndex, pageSize);
                    selCmd.AppendFormat("SELECT count(1) as TotalCount from DictionaryRef where {0};", filters);
                }
                else
                {
                    selCmd.Append("SELECT ");
                    selCmd.Append("Iden,RefKey,RefCode,RefCode2,RefValue,RefValue2,RefSystem,RefRemark,RefIsUse");
                    selCmd.Append(" FROM DictionaryRef  ");
                    selCmd.AppendFormat(" where {0} order by {1} ", filters, (string.IsNullOrEmpty(orderBy) ? "Iden" : orderBy));
                }

                helper = DBHelperFactory.CreateHelperFactory(Common.ConnectionString);
                list = helper.SelectReader<DictionaryRef>(selCmd.ToString());
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
        public int Add(DictionaryRef model, DBHelper helper, bool isTran, ref string errMsg)
        {
            errMsg = string.Empty;
            StringBuilder insCmd = new StringBuilder();
            insCmd.AppendFormat("insert into {0}", model.SaveTable);
            insCmd.Append("(RefKey,RefCode,RefCode2,RefValue,RefValue2,RefSystem,RefRemark,RefIsUse)");
            insCmd.Append("VALUES(@RefKey,@RefCode,@RefCode2,@RefValue,@RefValue2,@RefSystem,@RefRemark,@RefIsUse)");
            insCmd.Append(";select @@IDENTITY");
            int row = -1;
            object obj;
            try
            {
                if (helper == null)
                    helper = DBHelperFactory.CreateHelperFactory(Common.ConnectionString);
                obj = helper.ExecuteScalar(insCmd.ToString(), isTran, model);
                return Convert.ToInt32(obj);
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
        public int Add(DictionaryRef model, ref string errMsg)
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
        public bool Update(DictionaryRef model, DBHelper helper, bool isTran, ref string errMsg)
        {
            errMsg = string.Empty;
            StringBuilder updCmd = new StringBuilder();
            updCmd.AppendFormat("Update {0} Set ", model.SaveTable);
            updCmd.Append("RefKey=@RefKey");
            updCmd.Append(",RefCode=@RefCode");
            updCmd.Append(",RefCode2=@RefCode2");
            updCmd.Append(",RefValue=@RefValue");
            updCmd.Append(",RefValue2=@RefValue2");
            updCmd.Append(",RefSystem=@RefSystem");
            updCmd.Append(",RefRemark=@RefRemark");
            updCmd.Append(",RefIsUse=@RefIsUse");

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
        public bool Update(DictionaryRef model, ref string errMsg)
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
        public bool Delete(DictionaryRef model, DBHelper helper, bool isTran, ref string errMsg)
        {
            errMsg = string.Empty;
            StringBuilder delCmd = new StringBuilder();
            delCmd.AppendFormat(" Delete from {0} where ", model.SaveTable);
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
        public bool Delete(DictionaryRef model, ref string errMsg)
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
            updCmd.AppendFormat("Update DictionaryRef set {0} where {1} ", fileds, filters);
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
                updCmd.AppendFormat("select {0} from DictionaryRef where {1} LIMIT 1; ", filedName, filters);
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



        public DataTable GetMachineMainData(string filters, ref string errMsg)
        {
            errMsg = string.Empty;

            if (string.IsNullOrEmpty(filters)) filters = " 1=1";
            StringBuilder selCmd = new StringBuilder();
            try
            {
                DBHelper helper = null;

                selCmd.Append(@"SELECT * from MachineMain");
                selCmd.AppendFormat(" where {0} ", filters);
                selCmd.Append(" ORDER BY MMSeq");
                helper = new MySQLHelper(Common.ConnectionString);
                var dt = helper.ExecuteDataTable(selCmd.ToString());
                return dt;
            }
            catch (Exception ex)
            {
                errMsg = ex.ToString();
            }
            return null;
        }

        public static DataTable GetMachineLineData(string filters, ref string errMsg)
        {
            errMsg = string.Empty;

            if (string.IsNullOrEmpty(filters)) filters = " 1=1";
            StringBuilder selCmd = new StringBuilder();
            try
            {
                DBHelper helper = null;

                selCmd.Append(@"SELECT * from Line");
                selCmd.AppendFormat(" where {0} ", filters);
                helper = new MySQLHelper(Common.ConnectionString);
                var dt = helper.ExecuteDataTable(selCmd.ToString());
                return dt;
            }
            catch (Exception ex)
            {
                errMsg = ex.ToString();
            }
            return null;
        }
        public static void SaveData(List<DictionaryRef> lstUpdate, List<DictionaryRef> lstIns, ref string errMsg)
        {
            DapperHelper helper = new DapperHelper();
            System.Data.Common.DbConnection db = null;

            db = helper.MySQLConnection(Common.ConnectionString);
            db.Open();
            try
            {
                foreach (DictionaryRef model in lstUpdate)
                {
                    //  string.Format("RefValue='{0}',RefSystem=1,RefRemark='{1}'", Convert.ToInt32(chkVal), chkValText), $"RefKey='{refKey}' And RefCode='{refCode}';
                    string cmd = $@"Update DictionaryRef set RefValue='{model.RefValue}',RefSystem=1, RefIsUse=1,RefRemark='{model.RefRemark}',RefValue2='{model.RefValue2}' where RefKey = '{model.RefKey}' And RefCode = '{model.RefCode}'";
                    int row = db.Execute(cmd);

                }
                foreach (DictionaryRef model in lstIns)
                {
                    StringBuilder insCmd = new StringBuilder();
                    insCmd.AppendFormat("insert into {0}", model.SaveTable);
                    insCmd.Append("(RefKey,RefCode,RefCode2,RefValue,RefValue2,RefSystem,RefRemark,RefIsUse)");
                    insCmd.Append("VALUES(@RefKey,@RefCode,@RefCode2,@RefValue,@RefValue2,@RefSystem,@RefRemark,@RefIsUse)");

                    int row = db.Execute(insCmd.ToString(), model);
                    var jsonData = JsonConvert.SerializeObject(model);

                }
            }
            catch (Exception ex)
            {
                errMsg = ex.ToString();
            }
            finally
            {
                db.Close();
            }
        }


        /// <summary>
        /// 初始化系统设置的参数
        /// </summary> 
        public static void InitDicData(IList<DictionaryRef> list)
        {
            DictionaryRef m = new DictionaryRef();
            try
            {

                Common.TesterStateNum = 5;

                //var m = list.FirstOrDefault(o => o.RefCode.Equals("PLCConnectTime"));
                //pLCConnectTime = m == null ? 5 : Convert.ToInt32(m.RefValue);

                m = list.FirstOrDefault(o => o.RefCode.Equals("LenStaffID"));
                Common.LenStaffID = m == null ? 8 : Convert.ToInt32(m.RefValue);

                m = list.FirstOrDefault(o => o.RefCode.Equals("LenElectrolyte"));
                Common.LenElectrolyte = m == null ? 8 : Convert.ToInt32(m.RefValue);


                m = list.FirstOrDefault(o => o.RefCode.Equals("IsPpm"));
                Common.IsPpm = m == null ? false : Convert.ToBoolean(Convert.ToInt32(m.RefValue));

                m = list.FirstOrDefault(o => o.RefCode.Equals("IsPCK"));
                Common.IsCPK = m == null ? true : Convert.ToBoolean(Convert.ToInt32(m.RefValue));
                m = list.FirstOrDefault(o => o.RefCode.Equals("IsGRR"));
                Common.IsGRR = m == null ? false : Convert.ToBoolean(Convert.ToInt32(m.RefValue));


                //m = list.FirstOrDefault(o => o.RefCode.Equals("PLCConnectNum"));
                //pLCConnectNum = m == null ? 3 : Convert.ToInt32(m.RefValue);

                m = list.FirstOrDefault(o => o.RefCode.Equals("IsSelectStopReason"));
                Common.IsSelectStopReason = m == null ? false : Convert.ToBoolean(Convert.ToInt32(m.RefValue));


                m = list.FirstOrDefault(o => o.RefCode.Equals("UploadAsync"));
                Common.IsUploadAsync = m == null ? false : Convert.ToBoolean(Convert.ToInt32(m.RefValue));

                m = list.FirstOrDefault(o => o.RefCode.Equals("IsBarCodeABCAndNum"));
                Common.IsBarCodeABCAndNum = m == null ? false : Convert.ToBoolean(Convert.ToInt32(m.RefValue));

                m = list.FirstOrDefault(o => o.RefCode.Equals("UploadAlarmNum"));
                Common.uploadAlarmNum = m == null ? -1 : Convert.ToInt32(m.RefValue);


                m = list.FirstOrDefault(o => o.RefCode.Equals("IsUnBeforeSaveAfter"));
                Common.IsPLCArgSaveUnLoading = m == null ? false : Convert.ToBoolean(Convert.ToInt32(m.RefValue));

                m = list.FirstOrDefault(o => o.RefCode.Equals("IsBefore"));
                Common.IsBefore = m == null ? false : Convert.ToBoolean(Convert.ToInt32(m.RefValue));

                m = list.FirstOrDefault(o => o.RefCode.Equals("IsUploadNgPlc"));
                Common.IsUploadNgPlc = m == null ? true : Convert.ToBoolean(Convert.ToInt32(m.RefValue));

                m = list.FirstOrDefault(o => o.RefCode.Equals("IsPlcCountZero"));
                Common.IsPlcCountZero = m == null ? false : Convert.ToBoolean(Convert.ToInt32(m.RefValue));



                m = list.FirstOrDefault(o => o.RefCode.Equals("IsNetworkRealtime"));
                Common.IsNetworkRealtime = m == null ? true : Convert.ToBoolean(Convert.ToInt32(m.RefValue));


                m = list.FirstOrDefault(o => o.RefCode.Equals("IsTcpClose"));
                Common.IsTcpClose = m == null ? false : Convert.ToBoolean(Convert.ToInt32(m.RefValue));

                m = list.FirstOrDefault(o => o.RefCode.Equals("IsRealTimeChart"));
                Common.IsRealTimeChart = m == null ? false : Convert.ToBoolean(Convert.ToInt32(m.RefValue));

                m = list.FirstOrDefault(o => o.RefCode.Equals("IsRuningZero"));
                Common.IsRuningZero = m == null ? true : Convert.ToBoolean(Convert.ToInt32(m.RefValue));

                m = list.FirstOrDefault(o => o.RefCode.Equals("IsBarCodeLen"));
                Common.IsBarCodeLen = m == null ? true : Convert.ToBoolean(Convert.ToInt32(m.RefValue));

                m = list.FirstOrDefault(o => o.RefCode.Equals("IsCPKLegends"));
                Common.IsCPKLegends = m == null ? true : Convert.ToBoolean(Convert.ToInt32(m.RefValue));

                m = list.FirstOrDefault(o => o.RefCode.Equals("SaveExportTotalRow"));
                Common.SaveExportTotalRow = m == null ? 100 : Convert.ToInt32(m.RefValue);

                m = list.FirstOrDefault(o => o.RefCode.Equals("NotAutoRunCollect"));
                Common.NotAutoRunCollect = m == null ? false : Convert.ToBoolean(Convert.ToInt32(m.RefValue));


                m = list.FirstOrDefault(o => o.RefCode.Equals("IsWeldResult"));
                Common.IsWeldResult = m == null ? false : Convert.ToBoolean(Convert.ToInt32(m.RefValue));

                m = list.FirstOrDefault(o => o.RefCode.Equals("PLCAlarmSignalInterval"));
                Common.PLCReaderSignalInterval = m == null ? 50 : Convert.ToInt32(m.RefValue);

                m = list.FirstOrDefault(o => o.RefCode.Equals("IsPlcNoRepeat"));
                Common.IsPlcNoRepeat = m == null ? false : Convert.ToBoolean(Convert.ToInt32(m.RefValue));

                m = list.FirstOrDefault(o => o.RefCode.Equals("IsPlcNoRepeatNG"));
                Common.IsPlcNoRepeatNG = m == null ? false : Convert.ToBoolean(Convert.ToInt32(m.RefValue));

                //点检校验
                m = list.FirstOrDefault(o => o.RefCode.Equals("IsConPointCheck"));
                Common.IsConPointCheck = m == null ? false : Convert.ToBoolean(Convert.ToInt32(m.RefValue));

                m = list.FirstOrDefault(o => o.RefCode.Equals("IsAutoConPointCheck"));
                Common.IsAutoConPointCheck = m == null ? false : Convert.ToBoolean(Convert.ToInt32(m.RefValue));


                m = list.FirstOrDefault(o => o.RefCode.Equals("IsPointTimeOutStop"));
                Common.IsPointTimeOutStop = m == null ? false : Convert.ToBoolean(Convert.ToInt32(m.RefValue));


                m = list.FirstOrDefault(o => o.RefCode.Equals("IsImportCSV"));
                Common.IsImportCSV = m == null ? false : Convert.ToBoolean(Convert.ToInt32(m.RefValue));

                m = list.FirstOrDefault(o => o.RefCode.Equals("IsLoadCache"));
                Common.IsLoadCache = m == null ? true : Convert.ToBoolean(Convert.ToInt32(m.RefValue));

                //需要是否周次验证
                m = list.FirstOrDefault(o => o.RefCode.Equals("IseWeekCheck"));
                Common.IsWeek = m == null ? false : Convert.ToBoolean(Convert.ToInt32(m.RefValue));

                //启用MES
                m = list.FirstOrDefault(o => o.RefCode.Equals("IsMes"));
                Common.IsMes = m == null ? false : Convert.ToBoolean(Convert.ToInt32(m.RefValue));

                //是否启用模板验证
                m = list.FirstOrDefault(o => o.RefCode.Equals("IsTemplate"));
                Common.IsTemplate = m == null ? true : Convert.ToBoolean(Convert.ToInt32(m.RefValue));
                m = list.FirstOrDefault(o => o.RefCode.Equals("IsExportByDb"));
                Common.IsExportByDb = m == null ? true : Convert.ToBoolean(Convert.ToInt32(m.RefValue));

                m = list.FirstOrDefault(o => o.RefCode.Equals("IsUploadByDb"));
                Common.IsUploadByDb = m == null ? true : Convert.ToBoolean(Convert.ToInt32(m.RefValue));


                m = list.FirstOrDefault(o => o.RefCode.Equals("IseWeekCheckNG"));
                Common.IsWeekNG = m == null ? false : Convert.ToBoolean(Convert.ToInt32(m.RefValue));

                m = list.FirstOrDefault(o => o.RefCode.Equals("WeekCheckStart"));
                Common.WeekStartCut = m == null ? 0 : Convert.ToInt32(m.RefValue);

                m = list.FirstOrDefault(o => o.RefCode.Equals("WeekCheckEnd"));
                Common.WeekEndCut = m == null ? 0 : Convert.ToInt32(m.RefValue);

                //条码长度截取
                m = list.FirstOrDefault(o => o.RefCode.Equals("IsBarCodeCut"));
                if (m != null)
                {
                    Common.IsBarCodeCut = Convert.ToBoolean(Convert.ToInt32(m.RefValue));
                    if (Common.IsBarCodeCut)
                    {
                        m = list.FirstOrDefault(o => o.RefCode.Equals("IsBarCodeCutStart"));
                        if (m != null) Common.BarCodeStartCut = Convert.ToInt32(m.RefValue);
                        m = list.FirstOrDefault(o => o.RefCode.Equals("IsBarCodeCutEnd"));
                        if (m != null) Common.BarCodeEndCut = Convert.ToInt32(m.RefValue);
                    }
                }
                m = list.FirstOrDefault(o => o.RefCode.Equals("IsCheckUploadData"));
                Common.IsCheckUploadData = m == null ? false : Convert.ToBoolean(Convert.ToInt32(m.RefValue));

                m = list.FirstOrDefault(o => o.RefCode.Equals("SaveExcelTimes"));
                Common.SaveExcelTimes = m == null ? 5 : Convert.ToInt32(m.RefValue);



                m = list.FirstOrDefault(o => o.RefCode.Equals("IsSendPumpInjectWeigh"));
                Common.IsSendPumpInjectWeigh = m == null ? false : Convert.ToBoolean(Convert.ToInt32(m.RefValue));


                m = list.FirstOrDefault(o => o.RefCode.Equals("StratPointTime"));
                Common.StratPointTime = m == null ? 1 : Convert.ToInt32(m.RefValue);


                m = list.FirstOrDefault(o => o.RefCode.Equals("PointNum"));
                Common.PointNum = m == null ? 0 : Convert.ToInt32(m.RefValue);


                m = list.FirstOrDefault(o => o.RefKey.Equals("ShiftPointCheck"));//&& o.RefCode.Equals(Convert.ToString(beiShifts.EditValue)));
                                                                                 //当前班次
                Common.ShiftBegin = m == null ? "" : m.RefCode;
                //是否已检验
                Common.IsShiftPointCheck = m == null ? false : Convert.ToBoolean(Convert.ToInt32(m.RefCode2));

                //称点检校验有效分钟
                //       = m == null ? DateTime.MinValue : Convert.ToDateTime(m.RefValue);

                //班次开机时间
                Common.ShiftDate = m == null ? DateTime.MinValue : Convert.ToDateTime(m.RefValue2);

                //     Common ShiftDate = Convert.ToDateTime(m.RefValue2);
                //   Common.EndPointTime = m == null ? 8 : Convert.ToInt32(m.RefValue);

                m = list.FirstOrDefault(o => o.RefCode.Equals("WeightsNum"));
                Common.WeightsNum = m == null ? 60 : Convert.ToDouble(m.RefValue);

                m = list.FirstOrDefault(o => o.RefCode.Equals("WeightsNumRange"));
                Common.WeightsMin = m == null ? 0 : Convert.ToDouble(m.RefValue);
                Common.WeightsMax = m == null ? 0.03 : Convert.ToDouble(m.RefValue2);



                m = list.FirstOrDefault(o => o.RefCode.Equals("InjectCompensation1_1"));
                Common.InjectCompensation1_1 = m == null ? 1 : Convert.ToInt32(m.RefValue); //0:称一为计算点,1:称2为计算点



                m = list.FirstOrDefault(o => o.RefCode.Equals("InjectCompensation1_2"));
                Common.InjectCompensation1_2 = m == null ? 2 : Convert.ToInt32(m.RefValue);

                m = list.FirstOrDefault(o => o.RefCode.Equals("AfterWeighCompensation"));
                Common.AfterWeighCompensation = m == null ? 0 : Convert.ToDouble(m.RefValue);

                //m = list.FirstOrDefault(o => o.RefCode.Equals("IsScanCodeClose"));
                //tcp.IsClose = m == null ? false : Convert.ToBoolean(Convert.ToInt32(m.RefValue));

                //m = list.FirstOrDefault(o => o.RefCode.Equals("IsConPLCMatch"));
                //Common.IsAllowDataMismatch = m == null ? true : Convert.ToBoolean(Convert.ToInt32(m.RefValue));


                m = list.FirstOrDefault(o => o.RefCode.Equals("IsBeforeWeighCompensate"));
                Common.IsBeforeWeighCompensate = m == null ? false : Convert.ToBoolean(Convert.ToInt32(m.RefValue));


                //夹具个数才开始收集焊接泵的补偿值
                m = list.FirstOrDefault(o => o.RefCode.Equals("FixtureIntervalCount"));
                Common.FixtureCount = m == null ? 10 : Convert.ToInt32(m.RefValue);


                m = list.FirstOrDefault(o => o.RefCode.Equals("BeforeWeighCompensate"));
                Common.BeforeWeighCompensate = m == null ? Convert.ToDouble(0.2) : Convert.ToDouble(m.RefValue);



                m = list.FirstOrDefault(o => o.RefCode.Equals("IsFristScanBarCodeAfter"));
                Common.IsFristScanBarCodeAfter = m == null ? true : Convert.ToBoolean(Convert.ToInt32(m.RefValue));
                //是否先扫码,
                m = list.FirstOrDefault(o => o.RefCode.Equals("IsFristScanBarCode"));
                Common.IsFirstScanBarCodeBefore = m == null ? true : Convert.ToBoolean(Convert.ToInt32(m.RefValue));


                m = list.FirstOrDefault(o => o.RefCode.Equals("IsInjectCompensation"));
                Common.IsInjectCompensation = m == null ? true : Convert.ToBoolean(Convert.ToInt32(m.RefValue));

                //m = list.FirstOrDefault(o => o.RefCode.Equals("IsInjectCompensation2"));
                //Common.IsInjectCompensation2 = m == null ? true : Convert.ToBoolean(Convert.ToInt32(m.RefValue));


                m = list.FirstOrDefault(o => o.RefCode.Equals("InjectCompensationCount"));
                Common.InjectCompensationCount = m == null ? 10 : Convert.ToInt32(m.RefValue);

                m = list.FirstOrDefault(o => o.RefCode.Equals("NormalInjectCompensation"));
                Common.NormalInjectCompensation = m == null ? 5000 : Convert.ToInt32(m.RefValue);


                m = list.FirstOrDefault(o => o.RefCode.Equals("WeighWeight"));
                Common.WeighWeight = m == null ? Convert.ToDouble(2.3) : Convert.ToDouble(m.RefValue);


                m = list.FirstOrDefault(o => o.RefCode.Equals("ThreadClearInterval"));
                Common.ThreadClearInterval = m == null ? 3000 : Convert.ToInt32(m.RefValue);


                m = list.FirstOrDefault(o => o.RefCode.Equals("PLCReaderSignalInterval"));
                Common.ThreadInterval = m == null ? 50 : Convert.ToInt32(m.RefValue);

                m = list.FirstOrDefault(o => o.RefCode.Equals("InjAvg_USL"));
                Common.InjAvg_USL = m == null ? 0 : Convert.ToDouble(m.RefValue);

                m = list.FirstOrDefault(o => o.RefCode.Equals("InjAvg_LSL"));
                Common.InjAvg_LSL = m == null ? 0 : Convert.ToDouble(m.RefValue);


                m = list.FirstOrDefault(o => o.RefCode.Equals("GridRowCount"));
                Common.GridRowCount = m == null ? 500 : Convert.ToInt32(m.RefValue);

                m = list.FirstOrDefault(o => o.RefCode.Equals("CheckUploadDataDay"));
                Common.CheckUploadDataDay = m == null ? 3 : Convert.ToInt32(m.RefValue);

                m = list.FirstOrDefault(o => o.RefCode.Equals("IsMaterialBoxScan"));
                Common.IsLoadingPalletBarCodeScan = m == null ? false : Convert.ToBoolean(Convert.ToInt32(m.RefValue));

                m = list.FirstOrDefault(o => o.RefCode.Equals("IsMaterialBoxScan2"));
                Common.IsLoadingMaterialBoxScan2 = m == null ? false : Convert.ToBoolean(Convert.ToInt32(m.RefValue));

                m = list.FirstOrDefault(o => o.RefCode.Equals("IsUnLoadMaterialBoxScan"));
                Common.IsUnLoadingPalletBarCodeScan = m == null ? false : Convert.ToBoolean(Convert.ToInt32(m.RefValue));

                m = list.FirstOrDefault(o => o.RefCode.Equals("IsUnLoadMaterial2BoxScan"));
                Common.IsUnLoadingMaterialBoxScan2 = m == null ? false : Convert.ToBoolean(Convert.ToInt32(m.RefValue));



                m = list.FirstOrDefault(o => o.RefCode.Equals("IsElectrolyteScanCheck"));
                Common.IsElectrolyteScanCheck = m == null ? false : Convert.ToBoolean(Convert.ToInt32(m.RefValue));

                m = list.FirstOrDefault(o => o.RefCode.Equals("IsUnScanInBarCodeAndCount"));
                Common.IsUnScanInBarCodeAndCount = m == null ? true : Convert.ToBoolean(Convert.ToInt32(m.RefValue));

                m = list.FirstOrDefault(o => o.RefCode.Equals("InjectionNeedleCount")); //针头个数
                Common.TotalInjectNeedleNum = m == null ? 6 : Convert.ToInt32(m.RefValue);

                m = list.FirstOrDefault(o => o.RefCode.Equals("TurnCount")); //杯体总数
                Common.TurntableNeedleNum = m == null ? 0 : Convert.ToInt32(m.RefValue);

                m = list.FirstOrDefault(o => o.RefCode.Equals("Clear_Zero_MinScope"));
                Common.Clear_Zero_MinScope = m == null ? -0.5 : Convert.ToDouble(m.RefValue);

                m = list.FirstOrDefault(o => o.RefCode.Equals("Clear_Zero_MaxScope"));
                Common.Clear_Zero_MaxScope = m == null ? 0.5 : Convert.ToDouble(m.RefValue);


                m = list.FirstOrDefault(o => o.RefCode.Equals("IsAlarm"));
                Common.IsAlarm = m == null ? false : Convert.ToBoolean(Convert.ToInt32(m.RefValue));

                m = list.FirstOrDefault(o => o.RefCode.Equals("IsVarInjectStation"));
                Common.IsVarInjectStation = m == null ? false : Convert.ToBoolean(Convert.ToInt32(m.RefValue));


                //左针头数量
                m = list.FirstOrDefault(o => o.RefCode.Equals("InjectCompensationLNeedle"));
                Common.InjectCompensationLNeedle = m == null ? "" : m.RefValue;

                //右针头数量
                m = list.FirstOrDefault(o => o.RefCode.Equals("InjectCompensationRNeedle"));
                Common.InjectCompensationRNeedle = m == null ? "" : m.RefValue;


                //m = list.FirstOrDefault(o => o.RefCode.Equals("IsErrorSendNG"));
                //Common.IsErrorSendNG = m == null ? true : Convert.ToBoolean(Convert.ToInt32(m.RefValue));

                Common.IsErrorSendNG = true;

                m = list.FirstOrDefault(o => o.RefCode.Equals("IsPLC2Connect"));
                Common.IsPLC2Connect = m == null ? false : Convert.ToBoolean(Convert.ToInt32(m.RefValue));
                m = list.FirstOrDefault(o => o.RefCode.Equals("IsUploadErrorByNG"));
                Common.IsUploadErrorByNG = m == null ? false : Convert.ToBoolean(Convert.ToInt32(m.RefValue));

                //m = list.FirstOrDefault(o => o.RefCode.Equals("IsConPLCMatch"));
                //Common.IsAllowDataMismatch = m == null ? true : Convert.ToBoolean(Convert.ToInt32(m.RefValue));

                //m = list.FirstOrDefault(o => o.RefCode.Equals("IsPLCReaderConfig"));
                //IsPLCReaderConfig = m == null ? false : Convert.ToBoolean(Convert.ToInt32(m.RefValue));

                //m = list.FirstOrDefault(o => o.RefCode.Equals("IsCheckBarCode"));
                //Common.IsCheckBarCode = m == null ? false : Convert.ToBoolean(Convert.ToInt32(m.RefValue));

                //是否启用检验上料PLC计数

                m = list.FirstOrDefault(o => o.RefCode.Equals("IsCheckPLCCount"));
                Common.IsCheckPLCCount = m == null ? true : Convert.ToBoolean(Convert.ToInt32(m.RefValue));

                m = list.FirstOrDefault(o => o.RefCode.Equals("IsShareConnectReadPlcNo"));
                Common.IsShareConnectReadPlcNo = m == null ? true : Convert.ToBoolean(Convert.ToInt32(m.RefValue));

                Common.IsUploadErrorByNG = true;
                Common.IsAllowDataMismatch = true;
                Common.IsCheckBarCode = false;


                m = list.FirstOrDefault(o => o.RefCode.Equals("IsEndSoftExit"));
                Common.IsEndSoftExit = m == null ? true : Convert.ToBoolean(Convert.ToInt32(m.RefValue));

                m = list.FirstOrDefault(o => o.RefCode.Equals("RuningCheck"));
                Common.RuningCheckByDay = m == null ? 0 : Convert.ToInt32(m.RefValue);

                m = list.FirstOrDefault(o => o.RefCode.Equals("IsDynamicColumn"));
                Common.IsDynamicColumn = m == null ? false : Convert.ToBoolean(Convert.ToInt32(m.RefValue));

                m = list.FirstOrDefault(o => o.RefCode.Equals("IsChart"));
                Common.IsChart = m == null ? true : Convert.ToBoolean(Convert.ToInt32(m.RefValue));

                m = list.FirstOrDefault(o => o.RefCode.Equals("ConDataFormat"));
                Common.ConDataformat = m == null ? "F3" : $"F{Convert.ToInt32(m.RefValue)}";

                //m = list.FirstOrDefault(o => o.RefCode.Equals("IsSyncSignal"));
                //Common.IsSyncSignal = m == null ? false : Convert.ToBoolean(Convert.ToInt32(m.RefValue));

                //m = list.FirstOrDefault(o => o.RefCode.Equals("PLCDataFormat"));
                //Common.PLCDivided = m == null ? 10 : Convert.ToInt32(m.RefValue);



                m = list.FirstOrDefault(o => o.RefCode.Equals("AfterWeighReadMin"));
                Common.AfterWeighReadMin = m == null ? -0.01 : Convert.ToDouble(m.RefValue);

                m = list.FirstOrDefault(o => o.RefCode.Equals("AfterWeighReadMax"));
                Common.AfterWeighReadMax = m == null ? 0.01 : Convert.ToDouble(m.RefValue);

                m = list.FirstOrDefault(o => o.RefCode.Equals("IschkIsCollectDeviceUpload"));
                Common.chkIsCollectDeviceUpload = m == null ? false : Convert.ToBoolean(Convert.ToInt32(m.RefValue));



                m = list.FirstOrDefault(o => o.RefCode.Equals("IsDataUpload"));
                Common.IsDataUpload = m == null ? true : Convert.ToBoolean(Convert.ToInt32(m.RefValue));

                //m = list.FirstOrDefault(o => o.RefCode.Equals("isScanRepetitionVerify"));
                //Common.isScanRepetitionVerify = m == null ? true : Convert.ToBoolean(Convert.ToInt32(m.RefValue));

                Common.isScanRepetitionVerify = true;

                m = list.FirstOrDefault(o => o.RefCode.Equals("ScanRepetitionVerifyNum"));
                Common.ScanRepetitionVerifyNum = m == null ? 3 : Convert.ToInt32(m.RefValue);


                m = list.FirstOrDefault(o => o.RefCode.Equals("IsShareConnectReadPlcNo"));
                Common.IsShareConnectReadPlcNo = m == null ? true : Convert.ToBoolean(Convert.ToInt32(m.RefValue));



                m = list.FirstOrDefault(o => o.RefCode.Equals("IsNGUpload"));
                Common.IsNGUpload = m == null ? true : Convert.ToBoolean(Convert.ToInt32(m.RefValue));


                m = list.FirstOrDefault(o => o.RefCode.Equals("IsUploadMultiple"));
                Common.IsUploadMultiple = m == null ? true : Convert.ToBoolean(Convert.ToInt32(m.RefValue));
                //----作废


                m = list.FirstOrDefault(o => o.RefCode.Equals("IsUnLoadingShareScan"));
                Common.IsUnLoadingShareScan = m == null ? false : Convert.ToBoolean(Convert.ToInt32(m.RefValue));

                m = list.FirstOrDefault(o => o.RefCode.Equals("IsLoadingShareScan"));
                Common.IsLoadingShareScan = m == null ? false : Convert.ToBoolean(Convert.ToInt32(m.RefValue));
                m = list.FirstOrDefault(o => o.RefCode.Equals("IsLoadingScanShareSignal"));
                Common.IsLoadingScanShareSignal = m == null ? false : Convert.ToBoolean(Convert.ToInt32(m.RefValue));

                m = list.FirstOrDefault(o => o.RefCode.Equals("IsUnLoadingScanShareSignal"));
                Common.IsUnLoadingScanShareSignal = m == null ? false : Convert.ToBoolean(Convert.ToInt32(m.RefValue));

                m = list.FirstOrDefault(o => o.RefCode.Equals("IsBeforeWieghShareSignal"));
                Common.IsBeforeWieghShareSignal = m == null ? false : Convert.ToBoolean(Convert.ToInt32(m.RefValue));

                m = list.FirstOrDefault(o => o.RefCode.Equals("IsAfterWieghShareSignal"));
                Common.IsAfterWieghShareSignal = m == null ? false : Convert.ToBoolean(Convert.ToInt32(m.RefValue));
                m = list.FirstOrDefault(o => o.RefCode.Equals("IsBeforeWieghScanShareSignal"));
                Common.IsBeforeWieghScanShareSignal = m == null ? false : Convert.ToBoolean(Convert.ToInt32(m.RefValue));

                m = list.FirstOrDefault(o => o.RefCode.Equals("IsAfterWieghScanShareSignal"));
                Common.IsAfterWieghScanShareSignal = m == null ? false : Convert.ToBoolean(Convert.ToInt32(m.RefValue));

                m = list.FirstOrDefault(o => o.RefCode.Equals("IsSaveExcelTimes"));
                Common.IsSaveExcelTimes = m == null ? false : Convert.ToBoolean(Convert.ToInt32(m.RefValue));

                m = list.FirstOrDefault(o => o.RefCode.Equals("IsPLC2Connect"));
                Common.IsPLC2Connect = m == null ? false : Convert.ToBoolean(Convert.ToInt32(m.RefValue));



                m = list.FirstOrDefault(o => o.RefCode.Equals("IsThickness"));


                //m = list.FirstOrDefault(o => o.RefKey.Equals("IsShowLog"));
                //chkIsShowLog.Checked = m == null ? true : Convert.ToBoolean(Convert.ToInt32(m.RefValue));


                //---作废

                //早班开始时间
                m = list.FirstOrDefault(o => o.RefCode.Equals("MStartTime"));
                Common.MStartTime = m == null ? DateTime.Now.ToString("08:00") : Convert.ToDateTime(m.RefValue).ToString("HH:mm");

                m = list.FirstOrDefault(o => o.RefCode.Equals("MEndTime"));
                Common.MEndTime = m == null ? DateTime.Now.ToString("20:00") : Convert.ToDateTime(m.RefValue).ToString("HH:mm");

                //晚班开始时间
                m = list.FirstOrDefault(o => o.RefCode.Equals("EStartTime"));
                Common.EStartTime = m == null ? DateTime.Now.ToString("20:00") : Convert.ToDateTime(m.RefValue).ToString("HH:mm");

                m = list.FirstOrDefault(o => o.RefCode.Equals("EEndTime"));
                Common.EEndTime = m == null ? DateTime.Now.ToString("08:00") : Convert.ToDateTime(m.RefValue).ToString("HH:mm");


                m = list.FirstOrDefault(o => o.RefCode.Equals("IsChangeShiftsExit"));
                Common.IsChangeShiftsExit = m == null ? false : Convert.ToBoolean(Convert.ToInt32(m.RefValue));



                m = list.FirstOrDefault(o => o.RefCode.Equals("IsRealTimeCollectData"));
                Common.IsRealTimeCollectData = m == null ? false : Convert.ToBoolean(Convert.ToInt32(m.RefValue));

                m = list.FirstOrDefault(o => o.RefCode.Equals("ExportDataType"));
                Common.ExportDataType = m == null ? 2 : Convert.ToInt32(m.RefValue);


                m = list.FirstOrDefault(o => o.RefCode.Equals("CPKAlarmMin"));
                Common.CPKAlarmMin = m == null ? Convert.ToDouble(1.0) : Convert.ToDouble(m.RefValue);

                m = list.FirstOrDefault(o => o.RefCode.Equals("CPKAlarmMax"));
                Common.CPKAlarmMax = m == null ? Convert.ToDouble(100) : Convert.ToDouble(m.RefValue);


                m = list.FirstOrDefault(o => o.RefCode.Equals("CPKCount"));
                Common.CPKCount = m == null ? 20 : Convert.ToInt32(m.RefValue);


                m = list.FirstOrDefault(o => o.RefCode.Equals("ContNgAlarmNum"));
                Common.ContNgAlarmNum = m == null ? 5 : Convert.ToInt32(m.RefValue);

                m = list.FirstOrDefault(o => o.RefCode.Equals("ContUpLoadNum"));
                Common.contUpLoadNum = m == null ? 3 : Convert.ToInt32(m.RefValue);



                m = list.FirstOrDefault(o => o.RefCode.Equals("LoadCacheDay"));
                Common.LoadCacheDay = m == null ? 3 : Convert.ToInt32(m.RefValue);


                m = list.FirstOrDefault(o => o.RefCode.Equals("IsBeforeNGSaveData"));
                Common.IsBeforeNGSaveData = m == null ? true : Convert.ToBoolean(Convert.ToInt32(m.RefValue));

                m = list.FirstOrDefault(o => o.RefCode.Equals("IsStopClearData"));
                Common.IsStopClearData = m == null ? false : Convert.ToBoolean(Convert.ToInt32(m.RefValue));

                m = list.FirstOrDefault(o => o.RefCode.Equals("IsDegasAfter"));
                Common.IsDegasAfter = m == null ? false : Convert.ToBoolean(Convert.ToInt32(m.RefValue));

                m = list.FirstOrDefault(o => o.RefCode.Equals("IsGetLoadData"));
                Common.IsGetLoadData = m == null ? false : Convert.ToBoolean(Convert.ToInt32(m.RefValue));

                m = list.FirstOrDefault(o => o.RefCode.Equals("IsAfter"));
                Common.IsAfter = m == null ? false : Convert.ToBoolean(Convert.ToInt32(m.RefValue));

                m = list.FirstOrDefault(o => o.RefCode.Equals("InjectionDifference"));
                Common.InjectionDifference = m == null ? false : Convert.ToBoolean(Convert.ToInt32(m.RefValue));

                m = list.FirstOrDefault(o => o.RefCode.Equals("IsEmptyRuning"));
                Common.IsEmptyRuning = m == null ? false : Convert.ToBoolean(Convert.ToInt32(m.RefValue));

                m = list.FirstOrDefault(o => o.RefCode.Equals("IsWeldEnergy"));
                Common.IsWeldEnergy = m == null ? false : Convert.ToBoolean(Convert.ToInt32(m.RefValue));



                m = list.FirstOrDefault(o => o.RefCode.Equals("WeldEnergy_LSL"));
                Common.WeldEnergy_LSL = m == null ? 0 : Convert.ToInt32(m.RefValue);

                m = list.FirstOrDefault(o => o.RefCode.Equals("WeldEnergy_Num"));

                Common.WeldEnergy_Num = m == null ? 10 : Convert.ToInt32(m.RefValue);

                m = list.FirstOrDefault(o => o.RefCode.Equals("WeldPower_LSL"));
                Common.WeldPower_LSL = m == null ? 0 : Convert.ToInt32(m.RefValue);

                m = list.FirstOrDefault(o => o.RefCode.Equals("WeldPower_Num"));

                Common.WeldPower_Num = m == null ? 10 : Convert.ToInt32(m.RefValue);


                m = list.FirstOrDefault(o => o.RefCode.Equals("ThicknessCalibration"));
                Common.ThicknessCalibration = m == null ? Convert.ToDouble(0.000) : Convert.ToDouble(m.RefValue);

                m = list.FirstOrDefault(o => o.RefCode.Equals("ThicknessCompensate"));
                Common.ThicknessCompensate = m == null ? Convert.ToDouble(0.000) : Convert.ToDouble(m.RefValue);

                m = list.FirstOrDefault(o => o.RefCode.Equals("IsUnScanWeighMerge"));
                Common.IsUnScanWeighMerge = m == null ? false : Convert.ToBoolean(Convert.ToInt32(m.RefValue));
                m = list.FirstOrDefault(o => o.RefCode.Equals("BeforeUsL"));
                Common.BeforeUsL = m == null ? Convert.ToDouble(0.03) : Convert.ToDouble(m.RefValue);

                m = list.FirstOrDefault(o => o.RefCode.Equals("BeforeLsL"));
                Common.BeforeLsL = m == null ? Convert.ToDouble(-0.03) : Convert.ToDouble(m.RefValue);

                m = list.FirstOrDefault(o => o.RefCode.Equals("NetworkSleep"));
                Common.NetworkSleep = m == null ? 100 : Convert.ToInt32(m.RefValue);



                //var date = DateTime.Now;
                //string foder = date.Year + @"\" + date.Month;
                //Common.LogFileName = Application.StartupPath + @"\log\" + foder + @"\日志输出\";

                try
                {
                    if (!string.IsNullOrEmpty(Common.saveExcelPath))
                    {
                        if (!Directory.Exists(Common.saveExcelPath))
                        {
                            Directory.CreateDirectory(Common.saveExcelPath);
                        }
                    }

                }
                catch
                {

                }

                string errMsg = string.Empty;

                m = list.FirstOrDefault(o => o.RefCode.Equals("PpmScaleRangeMin"));
                Common.PpmScaleRangeMin = m == null ? 0 : Convert.ToInt32(m.RefValue);


                m = list.FirstOrDefault(o => o.RefCode.Equals("PpmScaleRangeSpan"));
                Common.PpmScaleRangeSpan = m == null ? 20 : Convert.ToInt32(m.RefValue);

                m = list.FirstOrDefault(o => o.RefCode.Equals("LogImportData"));
                Common.LogCount = m == null ? 20 : Convert.ToInt32(m.RefValue);

                m = list.FirstOrDefault(o => o.RefCode.Equals("IsUseSci"));
                Common.IsUseSci = m == null ? false : Convert.ToBoolean(Convert.ToInt32(m.RefValue));

                m = list.FirstOrDefault(o => o.RefCode.Equals("IsFristPartBarCode"));
                Common.IsUseFirstPart = m == null ? false : Convert.ToBoolean(Convert.ToInt32(m.RefValue));

                m = list.FirstOrDefault(o => o.RefCode.Equals("CheckThicknessNGNum"));
                Common.CheckThicknessNGNum = m == null ? 2 : Convert.ToInt32(m.RefValue);

                m = list.FirstOrDefault(o => o.RefCode.Equals("ThicknessNum1"));
                Common.ThicknessNum1 = m == null ? 0 : Convert.ToInt32(m.RefValue);

                m = list.FirstOrDefault(o => o.RefCode.Equals("ThicknessNum2"));
                Common.ThicknessNum2 = m == null ? 0 : Convert.ToInt32(m.RefValue);

                m = list.FirstOrDefault(o => o.RefCode.Equals("ThicknessNum3"));
                Common.ThicknessNum3 = m == null ? 0 : Convert.ToInt32(m.RefValue);

                m = list.FirstOrDefault(o => o.RefCode.Equals("RetetionFormula"));

                Common.RetetionFormula = m == null ? 0 : Convert.ToInt32(m.RefValue);


                var lstDevice = list.ToList().FindAll(o => o.RefKey.Equals("DeviceState"));

                m = lstDevice.FirstOrDefault(o => o.RefCode.Equals("Stop"));
                Common.DeviceState.Stop = m == null ? 0 : Convert.ToInt32(m.RefValue);
                m = lstDevice.FirstOrDefault(o => o.RefCode.Equals("Open"));
                Common.DeviceState.Open = m == null ? 0 : Convert.ToInt32(m.RefValue);
                m = lstDevice.FirstOrDefault(o => o.RefCode.Equals("Run"));
                Common.DeviceState.Run = m == null ? 0 : Convert.ToInt32(m.RefValue);

                m = lstDevice.FirstOrDefault(o => o.RefCode.Equals("Wait"));
                Common.DeviceState.Wait = m == null ? 0 : Convert.ToInt32(m.RefValue);
                m = lstDevice.FirstOrDefault(o => o.RefCode.Equals("Maintain"));
                Common.DeviceState.Maintain = m == null ? 0 : Convert.ToInt32(m.RefValue);
                m = lstDevice.FirstOrDefault(o => o.RefCode.Equals("Fault"));
                Common.DeviceState.Fault = m == null ? 0 : Convert.ToInt32(m.RefValue);




                //m = list.FirstOrDefault(o => o.RefCode.Equals("GRRSpec"));
                //GRR.Spec = m == null ? 1.41m : Convert.ToDecimal(m.RefValue);



                m = list.FirstOrDefault(o => o.RefCode.Equals("IsGRRNG"));
                Common.IsGRRNG = m == null ? false : Convert.ToBoolean(Convert.ToInt32(m.RefValue));


                m = list.FirstOrDefault(o => o.RefCode.Equals("IsGRRTemplate"));
                Common.IsGRRTemplate = m == null ? false : Convert.ToBoolean(Convert.ToInt32(m.RefValue));

                m = list.FirstOrDefault(o => o.RefCode.Equals("OmronPlcIp"));
                Common.OmronPlcIp = m == null ? 0 : Convert.ToInt32(m.RefValue);

                m = list.FirstOrDefault(o => o.RefCode.Equals("OmronLocalIp"));
                Common.OmronLocalIp = m == null ? 0 : Convert.ToInt32(m.RefValue);

                m = list.FirstOrDefault(o => o.RefCode.Equals("UploadArgInterval"));
                Common.UploadArgInterval = m == null ? 5 : Convert.ToInt32(m.RefValue);



                m = list.FirstOrDefault(o => o.RefCode.Equals("VarInjFormula"));
                Common.VarInjFormula = m == null ? 0 : Convert.ToInt32(m.RefValue);

                m = list.FirstOrDefault(o => o.RefCode.Equals("RepirInjFormula"));
                Common.RepirInjFormula = m == null ? 0 : Convert.ToInt32(m.RefValue);

                m = list.FirstOrDefault(o => o.RefCode.Equals("ThinessProgramme"));
                Common.ThinessProgramme = m == null ? 0 : Convert.ToInt32(m.RefValue);

                m = list.FirstOrDefault(o => o.RefCode.Equals("IsVarInject"));
                Common.IsVarInject = m == null ? false : Convert.ToBoolean(Convert.ToInt32(m.RefValue));
                m = list.FirstOrDefault(o => o.RefCode.Equals("VarInjectNumber"));
                Common.VarInjectNumber = m == null ? 32 : Convert.ToInt32(m.RefValue);

                m = list.FirstOrDefault(o => o.RefCode.Equals("VarInjCompensationLSL"));
                Common.VarInjCompensationLSL = m == null ? -0.02 : Convert.ToDouble(m.RefValue);
                m = list.FirstOrDefault(o => o.RefCode.Equals("VarInjCompensationUSL"));
                Common.VarInjCompensationUSL = m == null ? 0.02 : Convert.ToDouble(m.RefValue);

                m = list.FirstOrDefault(o => o.RefCode.Equals("IsThicknessOneToMore"));
                Common.IsThicknessOneToMore = m == null ? false : Convert.ToBoolean(Convert.ToInt32(m.RefValue));

                m = list.FirstOrDefault(o => o.RefCode.Equals("IsScanBarCodeOneToMore"));
                Common.IsScanBarCodeOneToMore = m == null ? false : Convert.ToBoolean(Convert.ToInt32(m.RefValue));

                //屏蔽开机电池及工位电池个数
                m = list.FirstOrDefault(o => o.RefCode.Equals("StartMachineConut"));
                Common.StartMachineConut = m == null ? 0 : Convert.ToInt32(m.RefValue);
                m = list.FirstOrDefault(o => o.RefCode.Equals("StationConut"));
                Common.StationConut = m == null ? 0 : Convert.ToInt32(m.RefValue);

                m = list.FirstOrDefault(o => o.RefCode.Equals("LEDStartIndex"));
                Common.LEDStartIndex = m == null ? 2 : Convert.ToInt32(m.RefValue);

                m = list.FirstOrDefault(o => o.RefCode.Equals("LEDEndIndex"));
                Common.LEDEndIndex = m == null ? 2 : Convert.ToInt32(m.RefValue);

                m = list.FirstOrDefault(o => o.RefCode.Equals("TotalRepairNeedleNo"));
                Common.TotalRepairNeedle = m == null ? 2 : Convert.ToInt32(m.RefValue);

             
                m = list.FirstOrDefault(o => o.RefCode.Equals("MachineType"));
                Common.MachineType = m == null ? 4 : Convert.ToInt32(m.RefValue);

                var beginTime = DateTime.Now.ToString($"yyyy-MM-dd {Common.MStartTime}");
                var endTime = DateTime.Now.ToString($"yyyy-MM-dd {Common.MEndTime}");

                var now = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));

                m = list.FirstOrDefault(o => o.RefCode.Equals("PointStartScope"));
                Common.PointStartScope = m == null ? 0 : Convert.ToInt32(m.RefValue);

                m = list.FirstOrDefault(o => o.RefCode.Equals("LogSaveMonth"));
                Common.LogSaveMonth = m == null ? 2 : Convert.ToInt32(m.RefValue);



                m = list.FirstOrDefault(o => o.RefCode.Equals("IsStopImportCSV"));
                Common.IsStopImportCSV = m == null ? false : Convert.ToBoolean(Convert.ToInt32(m.RefValue));


                m = list.FirstOrDefault(o => o.RefCode.Equals("SaveSuffix"));
                Common.SaveSuffix = m == null ? "CSV" : m.RefValue;

                m = list.FirstOrDefault(o => o.RefCode.Equals("SaveSplit"));
                Common.SaveSplit = (m == null || string.IsNullOrEmpty(m.RefValue)) ? "\t" : m.RefValue;

                m = list.FirstOrDefault(o => o.RefCode.Equals("HeartbeatReceiveStopNum"));
                Common.HeartbeatReceiveStopNum = m == null ? 1 : Convert.ToInt32(m.RefValue);

                m = list.FirstOrDefault(o => o.RefCode.Equals("IsCSVHeader"));
                Common.IsCSVHeader = m == null ? true : Convert.ToBoolean(Convert.ToInt32(m.RefValue));

                m = list.FirstOrDefault(o => o.RefCode.Equals("IsDbVerifyBarCode"));
                Common.IsDbVerifyBarCode = m == null ? false : Convert.ToBoolean(Convert.ToInt32(m.RefValue));

                m = list.FirstOrDefault(o => o.RefCode.Equals("IsDayCSV"));
                Common.IsDayCSV = m == null ? false : Convert.ToBoolean(Convert.ToInt32(m.RefValue));


                m = list.FirstOrDefault(o => o.RefCode.Equals("InjVolumeFormula"));
                Common.InjVolumeFormula = m == null ? 0 : Convert.ToInt32(m.RefValue);

                m = list.FirstOrDefault(o => o.RefCode.Equals("InjResultFormula"));
                Common.InjResultFormula = m == null ? 0 : Convert.ToInt32(m.RefValue);

                m = list.FirstOrDefault(o => o.RefCode.Equals("InjectPumpType"));
                Common.InjectPumpType = m == null ? 0 : Convert.ToInt32(m.RefValue);

                m = list.FirstOrDefault(o => o.RefCode.Equals("InjectPumpPointNum"));
                Common.InjectPumpPointNum = m == null ? 2 : Convert.ToInt32(m.RefValue);


                m = list.FirstOrDefault(o => o.RefCode.Equals("IsSaveFristBefore"));
                Common.IsSaveFristBefore = m == null ? false : Convert.ToBoolean(Convert.ToInt32(m.RefValue));


                m = list.FirstOrDefault(o => o.RefCode.Equals("IsWeighZeroData"));
                Common.IsWeighZeroData = m == null ? false : Convert.ToBoolean(Convert.ToInt32(m.RefValue));

                m = list.FirstOrDefault(o => o.RefCode.Equals("ClearWeighZeroDataNum"));
                Common.ClearWeighZeroDataNum = m == null ? 0 : Convert.ToInt32(m.RefValue);


                m = list.FirstOrDefault(o => o.RefCode.Equals("ScanNoNG"));
                Common.ScanNoNG = m == null ? false : Convert.ToBoolean(Convert.ToInt32(m.RefValue));

                m = list.FirstOrDefault(o => o.RefCode.Equals("EnableUnLoading"));
                Common.EnableUnLoading = m == null ? false : Convert.ToBoolean(Convert.ToInt32(m.RefValue));



                m = list.FirstOrDefault(o => o.RefCode.Equals("IsCacheWeigh"));
                Common.IsCacheWeigh = m == null ? false : Convert.ToBoolean(Convert.ToInt32(m.RefValue));

                m = list.FirstOrDefault(o => o.RefCode.Equals("CacheWeighNum"));
                Common.CacheWeighNum = m == null ? 0 : Convert.ToInt32(m.RefValue);

                m = list.FirstOrDefault(o => o.RefCode.Equals("IsUnScanErrorByCount"));
                Common.IsUnScanErrorByCount = m == null ? false : Convert.ToBoolean(Convert.ToInt32(m.RefValue));



                m = list.FirstOrDefault(o => o.RefCode.Equals("IsUnLoadingUpdate"));
                Common.IsUnLoadingUpdate = m == null ? false : Convert.ToBoolean(Convert.ToInt32(m.RefValue));

                m = list.FirstOrDefault(o => o.RefCode.Equals("IsUnLoadingUpdateNG"));
                Common.IsUnLoadingUpdateNG = m == null ? false : Convert.ToBoolean(Convert.ToInt32(m.RefValue));


                m = list.FirstOrDefault(o => o.RefCode.Equals("WeighRealtimeCount"));
                Common.WeighRealtimeCount = m == null ? 1 : Convert.ToInt32(m.RefValue);

                m = list.FirstOrDefault(o => o.RefCode.Equals("WeighRealtimeTotalCount"));
                Common.WeighRealtimeTotalCount = m == null ? 10 : Convert.ToInt32(m.RefValue);

                m = list.FirstOrDefault(o => o.RefCode.Equals("IsWeighRealtime"));
                Common.IsWeighRealtime = m == null ? false : Convert.ToBoolean(Convert.ToInt32(m.RefValue));

                m = list.FirstOrDefault(o => o.RefCode.Equals("seInjection_Upper"));
                Common.seInjection_Upper = m == null ? 1 : Convert.ToInt32(m.RefValue);

                m = list.FirstOrDefault(o => o.RefCode.Equals("InjVolume_USL"));
                Common.InjVolume_USL = m == null ? 0 : Convert.ToDouble(m.RefValue);

                m = list.FirstOrDefault(o => o.RefCode.Equals("InjVolume_LSL"));
                Common.InjVolume_LSL = m == null ? 0 : Convert.ToDouble(m.RefValue);


                m = list.FirstOrDefault(o => o.RefCode.Equals("IsBackupDataBase"));
                Common.IsBackupDataBase = m == null ? false : Convert.ToBoolean(Convert.ToInt32(m.RefValue));

                m = list.FirstOrDefault(o => o.RefCode.Equals("BackupDataBasePath"));
                Common.BackupDataBasePath = m == null ? Common.GetAssemblyPath : m.RefValue;

                m = list.FirstOrDefault(o => o.RefCode.Equals("BackupDataBaseMonths"));
                Common.BackupDataBaseMonths = m == null ? 2 : Convert.ToInt32(m.RefValue);

                m = list.FirstOrDefault(o => o.RefCode.Equals("IsMethodLock"));
                Common.IsMethodLock = m == null ? false : Convert.ToBoolean(Convert.ToInt32(m.RefValue));

                m = list.FirstOrDefault(o => o.RefCode.Equals("IsJudgeProModel"));
                Common.IsJudgeProModel = m == null ? false : Convert.ToBoolean(Convert.ToInt32(m.RefValue));

                m = list.FirstOrDefault(o => o.RefCode.Equals("IsContiuneNgStop"));
                Common.IsContiuneNgStop = m == null ? true : Convert.ToBoolean(Convert.ToInt32(m.RefValue));

                m = list.FirstOrDefault(o => o.RefCode.Equals("IsRightMenu"));
                Common.IsRightMenu = m == null ? false : Convert.ToBoolean(Convert.ToInt32(m.RefValue));

                m = list.FirstOrDefault(o => o.RefCode.Equals("MenuRightContorlTimer"));
                Common.MenuRightContorlTimer = m == null ? 3 : Convert.ToInt32(m.RefValue);

                m = list.FirstOrDefault(o => o.RefCode.Equals("IsScvRealtime"));
                Common.IsScvRealtime = m == null ? false : Convert.ToBoolean(Convert.ToInt32(m.RefValue));

                m = list.FirstOrDefault(o => o.RefCode.Equals("IsUnLoadScanAllInOne"));
                Common.IsUnLoadScanAllInOne = m == null ? false : Convert.ToBoolean(Convert.ToInt32(m.RefValue));

                m = list.FirstOrDefault(o => o.RefCode.Equals("IsTabVerify"));
                Common.IsTabVerify = m == null ? false : Convert.ToBoolean(Convert.ToInt32(m.RefValue));

                m = list.FirstOrDefault(o => o.RefCode.Equals("IsMesNgInfo"));
                Common.IsMesNgInfo = m == null ? false : Convert.ToBoolean(Convert.ToInt32(m.RefValue));

                m = list.FirstOrDefault(o => o.RefCode.Equals("IsEditPLCArg"));
                Common.IsEditPLCArg = m == null ? false : Convert.ToBoolean(Convert.ToInt32(m.RefValue));

                Common.PLCArgRemark = list.Where(o => o.RefCode.Equals("PLCArgRemark")).ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show("初始化数据字典出错" + "," + m.RefCode + ":" + ex.ToString());

                System.IO.File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + "Ini.log", ex.ToString());

            }

        }

        public static void SetBool(string refKey, string refCode, bool RefSystem, bool chkVal, string chkValText)
        {
            string errMsg = string.Empty;
            DapperHelper helper = new DapperHelper();
            System.Data.Common.DbConnection db = null;

            db = helper.MySQLConnection(Common.ConnectionString);

            DictionaryRef model = new DictionaryRef();
            model.RefKey = refKey;
            model.RefCode = refCode;
            model.RefValue = Convert.ToInt32(chkVal).ToString();
            model.RefRemark = chkValText;
            model.RefSystem = RefSystem;
            DictionaryRefDAL dictionaryRefDAL = new DictionaryRefDAL();
            if (dictionaryRefDAL.IsExists($"RefCode='{refCode}'", ref errMsg))
            {
                string cmd = $@"Update DictionaryRef set RefValue='{model.RefValue}',RefSystem=1,RefRemark='{model.RefRemark}',RefValue2='{model.RefValue2}' where RefKey = '{model.RefKey}' And RefCode = '{model.RefCode}'";
                int row = db.Execute(cmd);
            }
            else
            {
                StringBuilder insCmd = new StringBuilder();
                insCmd.AppendFormat("insert into {0}", model.SaveTable);
                insCmd.Append("(RefKey,RefCode,RefCode2,RefValue,RefValue2,RefSystem,RefRemark,RefIsUse)");
                insCmd.Append("VALUES(@RefKey,@RefCode,@RefCode2,@RefValue,@RefValue2,@RefSystem,@RefRemark,@RefIsUse)");

                int row = db.Execute(insCmd.ToString(), model);
                var jsonData = JsonConvert.SerializeObject(model);
            }
        }
    }
}
