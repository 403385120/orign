using Esquel.Utility;
using Permission.Model;
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
    public class Sys_ColumnSettingDAL
    {
       

        private DBHelper helper = null;

        public Sys_ColumnSettingDAL(string serverName)
        {
            
        }

        public Sys_ColumnSettingDAL()
        {
            string serverIP = ConfigurationManager.AppSettings["connectionString"];
            serverIP = string.Format(Common.ConnectionString, (string.IsNullOrEmpty(serverIP) || serverIP.Equals(".")) ? "127.0.0.1" : serverIP);
        }

        public bool IsExists(string filters, ref string errMsg)
        {
            errMsg = string.Empty;
            StringBuilder stringBuilder = new StringBuilder();
            try
            {

                stringBuilder.Append("select count(1) from Sys_ColumnSetting ");
                stringBuilder.AppendFormat(" where {0} ", filters);

                helper = DBHelperFactory.CreateHelperFactory(Common.ConnectionString);
                object value = helper.ExecuteScalar(stringBuilder.ToString());
                return (Convert.ToInt32(value) > 0) ? true : false;
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
            }
            return false;
        }

        public DataTable GetDataTable(string filters, int pageSize, int pageIndex, ref int rCount, ref string errMsg)
        {
          
            errMsg = string.Empty;
            if (string.IsNullOrEmpty(filters))
            {
                filters = " 1=1";
            }
            StringBuilder stringBuilder = new StringBuilder();
            if (pageSize != -1)
            {
                helper = DBHelperFactory.CreateHelperFactory(Common.ConnectionString);
                object value = helper.ExecuteScalar($"select count(1) from Sys_ColumnSetting where {filters} ");
                rCount = Convert.ToInt32(value);
            }
            if (pageSize != -1)
            {
                stringBuilder.Append("SELECT ");
                stringBuilder.Append("Iden,Factory,MenuCode,FieldEn,FieldCn,Width,IsFixed,IsVisible,Seq,IsUse,IsFormatRule,IsApplyToRow,Condition1,Value1,BackColor1,ForeColor1,IsFormatRule1,Condition2,Value2,BackColor2,ForeColor2,FormatString,Remark,DataType,IsReadOnly,IsAllotEdit,FieldGroupCn,ConvertOldValue,ConvertNewValue,FieldGroupCnEn,FieldCnEn");
                stringBuilder.Append(" FROM Sys_ColumnSetting ");
                stringBuilder.AppendFormat(" where {0} and Iden >= (select Iden from Sys_ColumnSetting where {0} order by Iden LIMIT {1},1) LIMIT {2};", filters, pageSize * pageIndex, pageSize * (pageIndex + 1));
            }
            else
            {
                stringBuilder.Append("SELECT ");
                stringBuilder.Append("Iden,Factory,MenuCode,FieldEn,FieldCn,Width,IsFixed,IsVisible,Seq,IsUse,IsFormatRule,IsApplyToRow,Condition1,Value1,BackColor1,ForeColor1,IsFormatRule1,Condition2,Value2,BackColor2,ForeColor2,FormatString,Remark,DataType,IsReadOnly,IsAllotEdit,FieldGroupCn,ConvertOldValue,ConvertNewValue,FieldGroupCnEn,FieldCnEn");
                stringBuilder.Append(" FROM Sys_ColumnSetting ");
                stringBuilder.AppendFormat(" where {0} ", filters);
            }
            helper = DBHelperFactory.CreateHelperFactory(Common.ConnectionString);
            return helper.ExecuteDataTable(stringBuilder.ToString());

        }

        public DataTable GetDataTable(string filters, ref string errMsg)
        {
            int rCount = 0;
            return GetDataTable(filters, -1, -1, ref rCount, ref errMsg);
        }

        public IList<Sys_ColumnSetting> GetList(string filters, string orderBy, int pageSize, int pageIndex, ref string errMsg)
        {
            return GetList(filters, orderBy, -1, pageSize, pageIndex, ref errMsg);
        }

        public IList<Sys_ColumnSetting> GetList(string filters, int pageSize, int pageIndex, ref string errMsg)
        {
            return GetList(filters, string.Empty, -1, pageSize, pageIndex, ref errMsg);
        }

        public IList<Sys_ColumnSetting> GetList(string filters, string orderBy, int TopNo, ref string errMsg)
        {
            return GetList(filters, orderBy, TopNo, -1, -1, ref errMsg);
        }

        public IList<Sys_ColumnSetting> GetList(string filters, string orderBy, ref string errMsg)
        {
            return GetList(filters, orderBy, -1, -1, -1, ref errMsg);
        }

        public IList<Sys_ColumnSetting> GetList(string filters, int TopNo, ref string errMsg)
        {
            return GetList(filters, string.Empty, TopNo, -1, -1, ref errMsg);
        }

        public IList<Sys_ColumnSetting> GetList(string filters, ref string errMsg)
        {
            return GetList(filters, string.Empty, -1, -1, -1, ref errMsg);
        }

        public IList<Sys_ColumnSetting> GetList(string filters, string orderBy, int TopNo, int pageSize, int pageIndex, ref string errMsg)
        {
            errMsg = string.Empty;
            Sys_ColumnSetting sys_ColumnSetting = new Sys_ColumnSetting();
            if (string.IsNullOrEmpty(filters))
            {
                filters = " 1=1";
            }
            StringBuilder stringBuilder = new StringBuilder();
            IList<Sys_ColumnSetting> result = new List<Sys_ColumnSetting>();
            try
            {

                if (pageSize != -1)
                {
                    stringBuilder.Append("SELECT ");
                    stringBuilder.Append("Iden,Factory,MenuCode,FieldEn,FieldCn,Width,IsFixed,IsVisible,Seq,IsUse,IsFormatRule,IsApplyToRow,Condition1,Value1,BackColor1,ForeColor1,IsFormatRule1,Condition2,Value2,BackColor2,ForeColor2,FormatString,Remark,DataType,IsReadOnly,IsAllotEdit,FieldGroupCn,ConvertOldValue,ConvertNewValue,FieldGroupCnEn,FieldCnEn,ColumnEdit,TextEditStyle,ImmediatePopup,Predefined1Name,Predefined2Name");
                    stringBuilder.Append(" FROM  Sys_ColumnSetting  ");
                    stringBuilder.AppendFormat(" where {0} and Iden >= (select Iden from Sys_ColumnSetting  where {0} order by {1} desc LIMIT {2},1) LIMIT {3};", filters, string.IsNullOrEmpty(orderBy) ? "Iden" : orderBy, pageSize * pageIndex, pageSize * (pageIndex + 1));
                    stringBuilder.AppendFormat("SELECT count(1) as TotalCount from Sys_ColumnSetting where {0};", filters);
                }
                else
                {
                    stringBuilder.Append("SELECT ");
                    stringBuilder.Append("Iden,Factory,MenuCode,FieldEn,FieldCn,Width,IsFixed,IsVisible,Seq,IsUse,IsFormatRule,IsApplyToRow,Condition1,Value1,BackColor1,ForeColor1,IsFormatRule1,Condition2,Value2,BackColor2,ForeColor2,FormatString,Remark,DataType,IsReadOnly,IsAllotEdit,FieldGroupCn,ConvertOldValue,ConvertNewValue,FieldGroupCnEn,FieldCnEn,ColumnEdit,TextEditStyle,ImmediatePopup,Predefined1Name,Predefined2Name");
                    stringBuilder.Append(" FROM Sys_ColumnSetting  ");
                    stringBuilder.AppendFormat(" where {0} order by {1} ", filters, string.IsNullOrEmpty(orderBy) ? "Iden" : orderBy);
                }

                helper = DBHelperFactory.CreateHelperFactory(Common.ConnectionString);
                result = helper.SelectReader<Sys_ColumnSetting>(stringBuilder.ToString());
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
            }
            return result;
        }

        public int Add(Sys_ColumnSetting model, DBHelper helper, bool isTran, ref string errMsg)
        {
            errMsg = string.Empty;
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendFormat("insert into Sys_ColumnSetting");
            stringBuilder.Append("(Factory,MenuCode,FieldEn,FieldCn,Width,IsFixed,IsVisible,Seq,IsUse,IsFormatRule,IsApplyToRow,Condition1,Value1,BackColor1,ForeColor1,IsFormatRule1,Condition2,Value2,BackColor2,ForeColor2,FormatString,Remark,DataType,IsReadOnly,IsAllotEdit,FieldGroupCn,ConvertOldValue,ConvertNewValue,FieldGroupCnEn,FieldCnEn,ColumnEdit,TextEditStyle,ImmediatePopup,Predefined1Name,Predefined2Name)");
            stringBuilder.Append("VALUES(@Factory,@MenuCode,@FieldEn,@FieldCn,@Width,@IsFixed,@IsVisible,@Seq,@IsUse,@IsFormatRule,@IsApplyToRow,@Condition1,@Value1,@BackColor1,@ForeColor1,@IsFormatRule1,@Condition2,@Value2,@BackColor2,@ForeColor2,@FormatString,@Remark,@DataType,@IsReadOnly,@IsAllotEdit,@FieldGroupCn,@ConvertOldValue,@ConvertNewValue,@FieldGroupCnEn,@FieldCnEn,@ColumnEdit,@TextEditStyle,@ImmediatePopup,@Predefined1Name,@Predefined2Name)");
            stringBuilder.Append(";select @@IDENTITY");
            int result = -1;
            try
            {
                if (helper == null)
                {
                    helper = DBHelperFactory.CreateHelperFactory(Common.ConnectionString);
                }
                object value = helper.ExecuteScalar(stringBuilder.ToString(), isTran, model);
                return Convert.ToInt32(value);
            }
            catch (Exception ex)
            {
                errMsg = ex.Message.Replace("'", string.Empty);
            }
            return result;
        }

        public int Add(Sys_ColumnSetting model, ref string errMsg)
        {
            return Add(model, null, false, ref errMsg);
        }

        public bool Update(Sys_ColumnSetting model, DBHelper helper, bool isTran, ref string errMsg)
        {
            errMsg = string.Empty;
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendFormat("Update Sys_ColumnSetting Set ");
            stringBuilder.Append("Factory=@Factory");
            stringBuilder.Append(",MenuCode=@MenuCode");
            stringBuilder.Append(",FieldEn=@FieldEn");
            stringBuilder.Append(",FieldCn=@FieldCn");
            stringBuilder.Append(",Width=@Width");
            stringBuilder.Append(",IsFixed=@IsFixed");
            stringBuilder.Append(",IsVisible=@IsVisible");
            stringBuilder.Append(",Seq=@Seq");
            stringBuilder.Append(",IsUse=@IsUse");
            stringBuilder.Append(",IsFormatRule=@IsFormatRule");
            stringBuilder.Append(",IsApplyToRow=@IsApplyToRow");
            stringBuilder.Append(",Condition1=@Condition1");
            stringBuilder.Append(",Value1=@Value1");
            stringBuilder.Append(",BackColor1=@BackColor1");
            stringBuilder.Append(",ForeColor1=@ForeColor1");
            stringBuilder.Append(",IsFormatRule1=@IsFormatRule1");
            stringBuilder.Append(",Condition2=@Condition2");
            stringBuilder.Append(",Value2=@Value2");
            stringBuilder.Append(",BackColor2=@BackColor2");
            stringBuilder.Append(",ForeColor2=@ForeColor2");
            stringBuilder.Append(",FormatString=@FormatString");
            stringBuilder.Append(",Remark=@Remark");
            stringBuilder.Append(",DataType=@DataType");
            stringBuilder.Append(",IsReadOnly=@IsReadOnly");
            stringBuilder.Append(",IsAllotEdit=@IsAllotEdit");
            stringBuilder.Append(",FieldGroupCn=@FieldGroupCn");
            stringBuilder.Append(",ConvertOldValue=@ConvertOldValue");
            stringBuilder.Append(",ConvertNewValue=@ConvertNewValue");
            stringBuilder.Append(",FieldGroupCnEn=@FieldGroupCnEn");
            stringBuilder.Append(",FieldCnEn=@FieldCnEn");
            stringBuilder.Append(",ColumnEdit=@ColumnEdit");
            stringBuilder.Append(",TextEditStyle=@TextEditStyle");
            stringBuilder.Append(",ImmediatePopup=@ImmediatePopup");
            stringBuilder.Append(",Predefined1Name=@Predefined1Name");
            stringBuilder.Append(",Predefined2Name=@Predefined2Name");
            stringBuilder.Append(" where Iden=@Iden");
            int num = -1;
            try
            {
                if (helper == null)
                {
                    helper = DBHelperFactory.CreateHelperFactory(Common.ConnectionString);
                }
                num = helper.ExecuteNonQuery(stringBuilder.ToString(), isTran, model);
            }
            catch (Exception ex)
            {
                errMsg = ex.Message.Replace("'", string.Empty);
                return false;
            }
            return true;
        }

        public bool Update(Sys_ColumnSetting model, ref string errMsg)
        {
            return Update(model, null, false, ref errMsg);
        }

        public bool Delete(Sys_ColumnSetting model, DBHelper helper, bool isTran, ref string errMsg)
        {
            errMsg = string.Empty;
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendFormat(" Delete from Sys_ColumnSetting where ");
            stringBuilder.Append("Iden=@Iden");
            int num = -1;
            try
            {
                if (helper == null)
                {
                    helper = DBHelperFactory.CreateHelperFactory(Common.ConnectionString);
                }
                num = helper.ExecuteNonQuery(stringBuilder.ToString(), isTran, model);
            }
            catch (Exception ex)
            {
                errMsg = ex.Message.Replace("'", string.Empty);
                return false;
            }
            return true;
        }

        public bool Delete(Sys_ColumnSetting model, ref string errMsg)
        {
            return Delete(model, null, false, ref errMsg);
        }

        public bool UpdateFiledsByFilters(string fileds, string filters, ref string errMsg)
        {
            errMsg = string.Empty;
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendFormat("Update Sys_ColumnSetting set {0} where {1} ", fileds, filters);
            try
            {
                helper = DBHelperFactory.CreateHelperFactory(Common.ConnectionString);
                helper.ExecuteNonQuery(stringBuilder.ToString());
            }
            catch (Exception ex)
            {
                errMsg = ex.Message.Replace("'", string.Empty);
            }
            return false;
        }

        public string SelectFiledsByFilters(string filedName, string filters, ref string errMsg)
        {
            errMsg = string.Empty;
            StringBuilder stringBuilder = new StringBuilder();
            try
            {
              
                    stringBuilder.AppendFormat("select {0} from Sys_ColumnSetting where {1} LIMIT 1; ", filedName, filters);
               
                helper = DBHelperFactory.CreateHelperFactory(Common.ConnectionString);
                object value = helper.ExecuteScalar(stringBuilder.ToString());
                if (!Convert.IsDBNull(value) && !string.IsNullOrEmpty(Convert.ToString(value)))
                {
                    return Convert.ToString(value);
                }
            }
            catch (Exception ex)
            {
                errMsg = ex.Message.Replace("'", string.Empty);
            }
            return string.Empty;
        }

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

        public bool ExecuteSQL(string sql, ref string errMsg)
        {
            return ExecuteSQL(sql, null, false, ref errMsg);
        }

        public bool ExecuteSQL(string sql, DBHelper helper, bool isTran, ref string errMsg)
        {
            errMsg = string.Empty;
            try
            {
                if (helper == null)
                {
                    helper = DBHelperFactory.CreateHelperFactory(Common.ConnectionString);
                }
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
