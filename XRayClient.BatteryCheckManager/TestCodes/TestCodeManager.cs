using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shuyz.Framework.Mvvm;
using ATL.Common;
using System.Data;

namespace XRayClient.BatteryCheckManager
{
    /// <summary>
    /// 点检条码定义
    /// </summary>
    public class TestCodeManager : ObservableObject
    {
        private List<TestCode> _codeList = new List<TestCode>();

        public static BaseFacade baseFacade = new BaseFacade();

        public List<TestCode> CodeList
        {
            get { return this._codeList; }
            set
            {
                this._codeList = value;
                RaisePropertyChanged("CodeList");
            }
        }

        public void Init()
        {
            this.RefreshTestCodeList();
        }

        public void UnInit()
        {

        }

        public bool AddTestCode(TestCode code)
        {
            string sql = $"INSERT INTO test_code(`BarCode`,`CreateTime`,`CreateBy`,`Remarks`) VALUES('{code.BarCode}','{code.CreateTime.ToString("yyyy-MM-dd HH:mm:ss")}','{code.CreateBy}','{code.Remarks}');";
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

            this.RefreshTestCodeList();
            return true;
        }

        public bool RemoveTestCode(int recordID)
        {
            string sql = $"DELETE FROM test_code WHERE RecordID={recordID};";
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

            this.RefreshTestCodeList();
            return true;
        }

        public bool RefreshTestCodeList()
        {
            string sql = "SELECT RecordID, BarCode, CreateTime, CreateBy, Remarks FROM test_code;";
            DataSet ds = baseFacade.equipDB.ExecuteDataSet(CommandType.Text,sql);

            List <TestCode> tmpList = new List<TestCode>();

            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                TestCode model = new TestCode();

                model.RecordID = (int)ds.Tables[0].Rows[i][0];
                model.BarCode = (string)ds.Tables[0].Rows[i][1];
                model.CreateTime = (DateTime)ds.Tables[0].Rows[i][2];
                model.CreateBy = ds.Tables[0].Rows[i][3].ToString();
                model.Remarks = (string)ds.Tables[0].Rows[i][4];
                tmpList.Add(model);
            }

            this.CodeList = tmpList;

            return true;
        }
    }
}
