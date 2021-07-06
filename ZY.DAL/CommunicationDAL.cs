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
    public partial class CommunicationDAL
    {
        private string conStr = string.Empty;
        private DBHelper helper = null;
        public CommunicationDAL(string serverName)
        {
            string serverIP = ConfigurationManager.AppSettings["connectionString"];
            serverIP = string.Format(Common.ConnectionString, (string.IsNullOrEmpty(serverIP) || serverIP.Equals(".")) ? "127.0.0.1" : serverIP);
        }
        public CommunicationDAL()
        {
            string serverIP = ConfigurationManager.AppSettings["connectionString"];
            serverIP = string.Format(Common.ConnectionString, (string.IsNullOrEmpty(serverIP) || serverIP.Equals(".")) ? "127.0.0.1" : serverIP);
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

                strSql.Append("select count(1) from Communication ");
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
                    object objRecord = helper.ExecuteScalar(string.Format("select count(1) from Communication where {0} ", filters));
                    rCount = Convert.ToInt32(objRecord);
                }
                if (pageSize != -1)
                {
                    selCmd.Append("SELECT ");
                    selCmd.Append("Iden,Factory,Code,Name,Ip,IpPort,IpLocalPort,IpLocalPortNum,ComPort,ComBaudRate,ComParity,ComStopBits,ComDataBits,SendCommand1,SendCommand2,SendCommand3,SendCommand4,IsEnter,IsLine,IsHex,IsByte,IsCRC,IsFCS,Remark,ScanCount,ScanInterval,IsUse,IsSocket,IsSocketTcp,SocketTcpType,SocketTimeOut,CheckDate,CheckUser,CheckResult,CheckData,InsImage,IsCheck,IsLock,IsClose,SendCommand5,SendCommand6,SendCommand7,SendCommand8,IsLoading,IsUnLoading,InstrumentBrand,ReadDelay,MachineID,IsWriteLog,CommunicationType,IsClearCacheBeforeRead,ClearDelayTime,CommDelayBfTime,CommDelayIngTime,CommDelayTime1,CommDelayTime2,DeviedVal,EndStrLen,EndByteLen,IsRecPlc,SendCommand1Remark,SendCommand2Remark,SendCommand3Remark,SendCommand4Remark,SendCommand5Remark,SendCommand6Remark,SendCommand7Remark,SendCommand8Remark,DeviedVal2,DeviedVal3");
                    selCmd.Append(" FROM Communication ");
                    selCmd.AppendFormat(" where {0} and Iden >= (select Iden from Communication where {0} order by Iden LIMIT {1},1) LIMIT {2};", filters, pageSize * pageIndex, pageSize);
                }
                else
                {
                    selCmd.Append("SELECT ");
                    selCmd.Append("Iden,Factory,Code,Name,Ip,IpPort,IpLocalPort,IpLocalPortNum,ComPort,ComBaudRate,ComParity,ComStopBits,ComDataBits,SendCommand1,SendCommand2,SendCommand3,SendCommand4,IsEnter,IsLine,IsHex,IsByte,IsCRC,IsFCS,Remark,ScanCount,ScanInterval,IsUse,IsSocket,IsSocketTcp,SocketTcpType,SocketTimeOut,CheckDate,CheckUser,CheckResult,CheckData,InsImage,IsCheck,IsLock,IsClose,SendCommand5,SendCommand6,SendCommand7,SendCommand8,IsLoading,IsUnLoading,InstrumentBrand,ReadDelay,MachineID,IsWriteLog,CommunicationType,IsClearCacheBeforeRead,ClearDelayTime,CommDelayBfTime,CommDelayIngTime,CommDelayTime1,CommDelayTime2,DeviedVal,EndStrLen,EndByteLen,IsRecPlc,SendCommand1Remark,SendCommand2Remark,SendCommand3Remark,SendCommand4Remark,SendCommand5Remark,SendCommand6Remark,SendCommand7Remark,SendCommand8Remark,DeviedVal2,DeviedVal3");
                    selCmd.Append(" FROM Communication ");
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
        public IList<Communication> GetList(string filters, string orderBy, int pageSize, int pageIndex, ref string errMsg)
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
        public IList<Communication> GetList(string filters, int pageSize, int pageIndex, ref string errMsg)
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
        public IList<Communication> GetList(string filters, string orderBy, int TopNo, ref string errMsg)
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
        public IList<Communication> GetList(string filters, string orderBy, ref string errMsg)
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
        public IList<Communication> GetList(string filters, int TopNo, ref string errMsg)
        {
            return GetList(filters, string.Empty, TopNo, -1, -1, ref errMsg);
        }

        /// <summary>
        /// 刷新数据
        /// </summary>
        /// <param name="filters">查询条件</param>
        /// <param name="errMsg">错误信息</param>
        /// <returns></returns>
        public IList<Communication> GetList(string filters, ref string errMsg)
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
        public IList<Communication> GetList(string filters, string orderBy, int TopNo, int pageSize, int pageIndex, ref string errMsg)
        {
            errMsg = string.Empty;
            Communication model = new Communication();
            if (string.IsNullOrEmpty(filters)) filters = " 1=1";
            StringBuilder selCmd = new StringBuilder();
            IList<Communication> list = new List<Communication>();
            try
            {
                if (pageSize != -1)
                {
                    selCmd.Append("SELECT ");
                    selCmd.Append("Iden,Factory,Code,Name,Ip,IpPort,IpLocalPort,IpLocalPortNum,ComPort,ComBaudRate,ComParity,ComStopBits,ComDataBits,SendCommand1,SendCommand1Remark,SendCommand2,SendCommand2Remark,SendCommand3,SendCommand3Remark,SendCommand4,SendCommand4Remark,SendCommand5,SendCommand5Remark,SendCommand6,SendCommand6Remark,SendCommand7,SendCommand7Remark,SendCommand8,SendCommand8Remark,IsEnter,IsLine,IsHex,IsByte,IsCRC,IsFCS,Remark,ScanCount,ScanInterval,IsUse,IsSocket,IsSocketTcp,SocketTcpType,SocketTimeOut,CheckDate,CheckUser,CheckResult,CheckData,InsImage,IsCheck,IsLock,IsClose,IsLoading,IsUnLoading,InstrumentBrand,ReadDelay,MachineID,IsWriteLog,CommunicationType,IsClearCacheBeforeRead,ClearDelayTime,CommDelayBfTime,CommDelayIngTime,CommDelayTime1,CommDelayTime2,DeviedVal,EndStrLen,EndByteLen,IsRecPlc,Encoding,BufferSize,DeviedVal2,DeviedVal3");
                    selCmd.Append(" FROM  Communication  ");
                    selCmd.AppendFormat(" where {0} and Iden >= (select Iden from Communication  where {0} order by {1} desc LIMIT {2},1) LIMIT {3};", filters, (string.IsNullOrEmpty(orderBy) ? "Iden" : orderBy), pageSize * pageIndex, pageSize);
                    selCmd.AppendFormat("SELECT count(1) as TotalCount from Communication where {0};", filters);
                }
                else
                {
                    selCmd.Append("SELECT ");
                    selCmd.Append("Iden,Factory,Code,Name,Ip,IpPort,IpLocalPort,IpLocalPortNum,ComPort,ComBaudRate,ComParity,ComStopBits,ComDataBits,SendCommand1,SendCommand1Remark,SendCommand2,SendCommand2Remark,SendCommand3,SendCommand3Remark,SendCommand4,SendCommand4Remark,SendCommand5,SendCommand5Remark,SendCommand6,SendCommand6Remark,SendCommand7,SendCommand7Remark,SendCommand8,SendCommand8Remark,IsEnter,IsLine,IsHex,IsByte,IsCRC,IsFCS,Remark,ScanCount,ScanInterval,IsUse,IsSocket,IsSocketTcp,SocketTcpType,SocketTimeOut,CheckDate,CheckUser,CheckResult,CheckData,InsImage,IsCheck,IsLock,IsClose,IsLoading,IsUnLoading,InstrumentBrand,ReadDelay,MachineID,IsWriteLog,CommunicationType,IsClearCacheBeforeRead,ClearDelayTime,CommDelayBfTime,CommDelayIngTime,CommDelayTime1,CommDelayTime2,DeviedVal,EndStrLen,EndByteLen,IsRecPlc,Encoding,BufferSize,DeviedVal2,DeviedVal3");
                    selCmd.Append(" FROM Communication  ");
                    selCmd.AppendFormat(" where {0} order by {1} ", filters, (string.IsNullOrEmpty(orderBy) ? "Iden" : orderBy));
                }

                helper = DBHelperFactory.CreateHelperFactory(Common.ConnectionString);
                list = helper.SelectReader<Communication>(selCmd.ToString());
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
        public int Add(Communication model, DBHelper helper, bool isTran, ref string errMsg)
        {
            errMsg = string.Empty;
            StringBuilder insCmd = new StringBuilder();
            insCmd.AppendFormat("insert into {0}", model.SaveTable);
            insCmd.Append("(Factory,Code,Name,Ip,IpPort,IpLocalPort,IpLocalPortNum,ComPort,ComBaudRate,ComParity,ComStopBits,ComDataBits,SendCommand1,SendCommand1Remark,SendCommand2,SendCommand2Remark,SendCommand3,SendCommand3Remark,SendCommand4,SendCommand4Remark,SendCommand5,SendCommand5Remark,SendCommand6,SendCommand6Remark,SendCommand7,SendCommand7Remark,SendCommand8,SendCommand8Remark,IsEnter,IsLine,IsHex,IsByte,IsCRC,IsFCS,Remark,ScanCount,ScanInterval,IsUse,IsSocket,IsSocketTcp,SocketTcpType,SocketTimeOut,CheckDate,CheckUser,CheckResult,CheckData,InsImage,IsCheck,IsLock,IsClose,IsLoading,IsUnLoading,InstrumentBrand,ReadDelay,MachineID,IsWriteLog,CommunicationType,IsClearCacheBeforeRead,ClearDelayTime,CommDelayBfTime,CommDelayIngTime,CommDelayTime1,CommDelayTime2,DeviedVal,EndStrLen,EndByteLen,IsRecPlc,Encoding,BufferSize,DeviedVal2,DeviedVal3)");
            insCmd.Append("VALUES(@Factory,@Code,@Name,@Ip,@IpPort,@IpLocalPort,@IpLocalPortNum,@ComPort,@ComBaudRate,@ComParity,@ComStopBits,@ComDataBits,@SendCommand1,@SendCommand1Remark,@SendCommand2,@SendCommand2Remark,@SendCommand3,@SendCommand3Remark,@SendCommand4,@SendCommand4Remark,@SendCommand5,@SendCommand5Remark,@SendCommand6,@SendCommand6Remark,@SendCommand7,@SendCommand7Remark,@SendCommand8,@SendCommand8Remark,@IsEnter,@IsLine,@IsHex,@IsByte,@IsCRC,@IsFCS,@Remark,@ScanCount,@ScanInterval,@IsUse,@IsSocket,@IsSocketTcp,@SocketTcpType,@SocketTimeOut,@CheckDate,@CheckUser,@CheckResult,@CheckData,@InsImage,@IsCheck,@IsLock,@IsClose,@IsLoading,@IsUnLoading,@InstrumentBrand,@ReadDelay,@MachineID,@IsWriteLog,@CommunicationType,@IsClearCacheBeforeRead,@ClearDelayTime,@CommDelayBfTime,@CommDelayIngTime,@CommDelayTime1,@CommDelayTime2,@DeviedVal,@EndStrLen,@EndByteLen,@IsRecPlc,@Encoding,@BufferSize,@DeviedVal2,@DeviedVal3)");
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
        public int Add(Communication model, ref string errMsg)
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
        public bool Update(Communication model, DBHelper helper, bool isTran, ref string errMsg)
        {
            errMsg = string.Empty;
            StringBuilder updCmd = new StringBuilder();
            updCmd.AppendFormat("Update {0} Set ", model.SaveTable);
            updCmd.Append("Factory=@Factory");
            updCmd.Append(",Code=@Code");
            updCmd.Append(",Name=@Name");
            updCmd.Append(",Ip=@Ip");
            updCmd.Append(",IpPort=@IpPort");
            updCmd.Append(",IpLocalPort=@IpLocalPort");
            updCmd.Append(",IpLocalPortNum=@IpLocalPortNum");
            updCmd.Append(",ComPort=@ComPort");
            updCmd.Append(",ComBaudRate=@ComBaudRate");
            updCmd.Append(",ComParity=@ComParity");
            updCmd.Append(",ComStopBits=@ComStopBits");
            updCmd.Append(",ComDataBits=@ComDataBits");
            updCmd.Append(",SendCommand1=@SendCommand1");
            updCmd.Append(",SendCommand1Remark=@SendCommand1Remark");
            updCmd.Append(",SendCommand2=@SendCommand2");
            updCmd.Append(",SendCommand2Remark=@SendCommand2Remark");
            updCmd.Append(",SendCommand3=@SendCommand3");
            updCmd.Append(",SendCommand3Remark=@SendCommand3Remark");
            updCmd.Append(",SendCommand4=@SendCommand4");
            updCmd.Append(",SendCommand4Remark=@SendCommand4Remark");
            updCmd.Append(",SendCommand5=@SendCommand5");
            updCmd.Append(",SendCommand5Remark=@SendCommand5Remark");
            updCmd.Append(",SendCommand6=@SendCommand6");
            updCmd.Append(",SendCommand6Remark=@SendCommand6Remark");
            updCmd.Append(",SendCommand7=@SendCommand7");
            updCmd.Append(",SendCommand7Remark=@SendCommand7Remark");
            updCmd.Append(",SendCommand8=@SendCommand8");
            updCmd.Append(",SendCommand8Remark=@SendCommand8Remark");
            updCmd.Append(",IsEnter=@IsEnter");
            updCmd.Append(",IsLine=@IsLine");
            updCmd.Append(",IsHex=@IsHex");
            updCmd.Append(",IsByte=@IsByte");
            updCmd.Append(",IsCRC=@IsCRC");
            updCmd.Append(",IsFCS=@IsFCS");
            updCmd.Append(",Remark=@Remark");
            updCmd.Append(",ScanCount=@ScanCount");
            updCmd.Append(",ScanInterval=@ScanInterval");
            updCmd.Append(",IsUse=@IsUse");
            updCmd.Append(",IsSocket=@IsSocket");
            updCmd.Append(",IsSocketTcp=@IsSocketTcp");
            updCmd.Append(",SocketTcpType=@SocketTcpType");
            updCmd.Append(",SocketTimeOut=@SocketTimeOut");
            updCmd.Append(",CheckDate=@CheckDate");
            updCmd.Append(",CheckUser=@CheckUser");
            updCmd.Append(",CheckResult=@CheckResult");
            updCmd.Append(",CheckData=@CheckData");
            updCmd.Append(",InsImage=@InsImage");
            updCmd.Append(",IsCheck=@IsCheck");
            updCmd.Append(",IsLock=@IsLock");
            updCmd.Append(",IsClose=@IsClose");
            updCmd.Append(",IsLoading=@IsLoading");
            updCmd.Append(",IsUnLoading=@IsUnLoading");
            updCmd.Append(",InstrumentBrand=@InstrumentBrand");
            updCmd.Append(",ReadDelay=@ReadDelay");
            updCmd.Append(",MachineID=@MachineID");
            updCmd.Append(",IsWriteLog=@IsWriteLog");
            updCmd.Append(",CommunicationType=@CommunicationType");
            updCmd.Append(",IsClearCacheBeforeRead=@IsClearCacheBeforeRead");
            updCmd.Append(",ClearDelayTime=@ClearDelayTime");
            updCmd.Append(",CommDelayBfTime=@CommDelayBfTime");
            updCmd.Append(",CommDelayIngTime=@CommDelayIngTime");
            updCmd.Append(",CommDelayTime1=@CommDelayTime1");
            updCmd.Append(",CommDelayTime2=@CommDelayTime2");
            updCmd.Append(",DeviedVal=@DeviedVal");
            updCmd.Append(",EndStrLen=@EndStrLen");
            updCmd.Append(",EndByteLen=@EndByteLen");
            updCmd.Append(",IsRecPlc=@IsRecPlc");
            updCmd.Append(",Encoding=@Encoding");
            updCmd.Append(",BufferSize=@BufferSize");
            updCmd.Append(",DeviedVal2=@DeviedVal2");
            updCmd.Append(",DeviedVal3=@DeviedVal3");
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
        public bool Update(Communication model, ref string errMsg)
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
        public bool Delete(Communication model, DBHelper helper, bool isTran, ref string errMsg)
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
        public bool Delete(Communication model, ref string errMsg)
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
            updCmd.AppendFormat("Update Communication set {0} where {1} ", fileds, filters);
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
                updCmd.AppendFormat("select {0} from Communication where {1} LIMIT 1; ", filedName, filters);
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
