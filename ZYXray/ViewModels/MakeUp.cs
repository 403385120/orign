using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ATL.Common;

namespace ZYXray.ViewModels
{
    public class MakeUp
    {
        private static MakeUp current;
        public static MakeUp Current
        {
            get
            {
                return current;
            }
            set
            {
                if (current == null)
                {
                    current = value;
                }
            }
        }
        public static BaseFacade baseFacade = new BaseFacade();
        public static List<Inquire> MakeUpData(DateTime StartTime,int IFOK,string barcode="")
        {
            List<Inquire> InquireInfo = new List<Inquire>();
            string sql;
            if(!(barcode==""))
            {
                sql = $"SELECT ProductSN, RecheckUserID, FQAUser FROM production_data WHERE RecheckState ={IFOK} AND FQATime ='{StartTime}' AND ProductSN='{barcode}';";
            }
            else
            {
                sql = $"SELECT ProductSN, RecheckUserID, FQAUser FROM production_data WHERE RecheckState ={IFOK} AND FQATime ='{StartTime}';";
            }
            DataSet ds = baseFacade.equipDB.ExecuteDataSet(CommandType.Text, sql);
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                string a = (string)ds.Tables[0].Rows[i][0];
                string sqll= $"SELECT Data, keyValue FROM recheck_production_data WHERE ProductSN ='{a}';";
                DataSet dss = baseFacade.equipDB.ExecuteDataSet(CommandType.Text, sqll);
                Inquire inquire = new Inquire();
                inquire.Barcode = (string)ds.Tables[0].Rows[i][0];
                inquire.data= (string)dss.Tables[0].Rows[0][0];
                inquire.keyValue= (string)dss.Tables[0].Rows[0][1];
                inquire.PRDUser= (string)ds.Tables[0].Rows[i][1];
                inquire.FQAUser = (string)ds.Tables[0].Rows[i][2];
                if(IFOK==5)
                {
                    #region 替换复判OK电芯json
                   
                    string ngdata = "{\"ParamID\":\"251\",\"ParamDesc\":null,\"ParamValue\":\"-1\",\"Result\":null";
                    string okdata = "{\"ParamID\":\"1184\",\"ParamDesc\":null,\"ParamValue\":\"" + inquire.keyValue + "\",\"Result\":null";

                    inquire.data = inquire.data.Replace(ngdata, okdata);

                    string prd = "\"EmployeeNo\":\"PRD\"";
                    string prd1 = "\"EmployeeNo\":\"" + inquire.PRDUser + "\"";
                    inquire.data = inquire.data.Replace(prd, prd1);

                    string fqa = "\"EmployeeNo1\":\"\"";
                    string fqa1 = "\"EmployeeNo1\":\"" + inquire.FQAUser + "\"";
                    inquire.data = inquire.data.Replace(fqa, fqa1);
                    #endregion
                }
                else if(IFOK==6)
                {
                    #region 替换复判NG电芯json
                    inquire.data = inquire.data.Replace("\"Pass\":\"OK\"", "\"Pass\":\"NG\"");
                    inquire.data = inquire.data.Replace("{\"ParamID\":\"251\",", "{\"ParamID\":\"1184\",");
                    
                    string prd = "\"EmployeeNo\":\"PRD\"";
                    string prd1 = "\"EmployeeNo\":\"" + inquire.PRDUser + "\"";
                    inquire.data = inquire.data.Replace(prd, prd1);
                    #endregion
                }

                InquireInfo.Add(inquire);
                
            }
            return InquireInfo;
        }
        public static bool UpdateSQL(Inquire inquire, int IFOK)
        {
            try
            {
                string sqlll;
                if (IFOK==5)//OK(99)
                {
                    sqlll = $"UPDATE production_data SET RecheckState = {(int)99} WHERE ProductSN ='{inquire.Barcode}';";
                }
                else//NG(98)
                {
                    sqlll = $"UPDATE production_data SET RecheckState = {(int)98} WHERE ProductSN ='{inquire.Barcode}';";
                }
                
                baseFacade.equipDB.ExecuteNonQuery(CommandType.Text, sqlll);
                return true;
            }
            catch
            {
                return false;
            }
            
        }
    }
    
    public class Inquire
    {
        public string Barcode { get; set; }
        public string PRDUser { get; set; }
        public string FQAUser { get; set; }
        public string data { get; set; }
        public string keyValue { get; set; }
    }
}
