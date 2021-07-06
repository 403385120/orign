using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shuyz.Framework.Mvvm;
using ATL.Common;
using System.Data;
using System.IO;

namespace XRayClient.BatteryCheckManager
{
    public class RecheckRecordManager : ObservableObject
    {
        public static BaseFacade baseFacade = new BaseFacade();

        //RecheckState    0：未检测  1：重判OK   2：重判NG   3：FQA打回    4：FQA判定OK(结果未上传或上传失败)  9:重判为待判定
        private readonly string _tableName = "@zy_battery_check@";
        private readonly string _recheckSelectSQL = @"SELECT 
	                                                Iden,
                                                    ProductSN,
                                                    quality,
                                                    ResultPath,
                                                    A1_OriginalImagePath,
                                                    A2_OriginalImagePath,
                                                    A3_OriginalImagePath,
                                                    A4_OriginalImagePath,
                                                    RecheckState,
                                                    IFNULL(RecheckTime, CAST('1988-12-25 00:00:00' AS DATETIME)),
                                                    IFNULL(RecheckUserID,''),
                                                    IFNULL(FQATime, CAST('1988-12-25 00:00:00' AS DATETIME)),
                                                    IFNULL(FQAUser, ''),
                                                    A1_ResultImagePath,
                                                    A2_ResultImagePath,
                                                    A3_ResultImagePath,
                                                    A4_ResultImagePath,
                                                    MesImagePath,
                                                    CheckMode
                                                FROM
                                                    production_data
                                                WHERE
	                                                quality = 0 AND
                                                    BatteryBarCode != ''
                                                        AND (RecheckState = 0 || RecheckState = 3)
                                                ORDER BY ProductDate";

        private readonly string _recheckAllSelectSQL = @"SELECT 
	                                                Iden,
                                                    ProductSN,
                                                    quality,
                                                    ResultPath,
                                                    A1_OriginalImagePath,
                                                    A2_OriginalImagePath,
                                                    A3_OriginalImagePath,
                                                    A4_OriginalImagePath,
                                                    RecheckState,
                                                    IFNULL(RecheckTime, CAST('1988-12-25 00:00:00' AS DATETIME)),
                                                    IFNULL(RecheckUserID,''),
                                                    IFNULL(FQATime, CAST('1988-12-25 00:00:00' AS DATETIME)),
                                                    IFNULL(FQAUser, ''),
                                                    A1_ResultImagePath,
                                                    A2_ResultImagePath,
                                                    A3_ResultImagePath,
                                                    A4_ResultImagePath,
                                                    MesImagePath,
                                                    CheckMode
                                                FROM
                                                    production_data
                                                WHERE
	                                                quality = 0 AND
                                                    ProductSN != ''
                                                        AND (RecheckState = 0 || RecheckState = 3 || RecheckState = 9)
                                                ORDER BY ProductDate";

        private readonly string _fqaSelectSQL = @"SELECT 
	                                                Iden,
                                                    ProductSN,
                                                    quality,
                                                    ResultPath,
                                                    A1_OriginalImagePath,
                                                    A2_OriginalImagePath,
                                                    A3_OriginalImagePath,
                                                    A4_OriginalImagePath,
                                                    RecheckState,
                                                    IFNULL(RecheckTime, CAST('1988-12-25 00:00:00' AS DATETIME)),
                                                    IFNULL(RecheckUserID,''),
                                                    IFNULL(FQATime,  CAST('1988-12-25 00:00:00' AS DATETIME)),
                                                    IFNULL(FQAUser, ''),
                                                    A1_ResultImagePath,
                                                    A2_ResultImagePath,
                                                    A3_ResultImagePath,
                                                    A4_ResultImagePath,
                                                    MesImagePath,
                                                    CheckMode
                                                FROM
                                                    production_data
                                                WHERE
                                                        RecheckState = 1
                                                ORDER BY ProductDate";

        private readonly string _fqaNotUploadSQL = @"SELECT 
	                                                Iden,
                                                    ProductSN,
                                                    quality,
                                                    ResultPath,
                                                    A1_OriginalImagePath,
                                                    A2_OriginalImagePath,
                                                    A3_OriginalImagePath,
                                                    A4_OriginalImagePath,
                                                    RecheckState,
                                                    IFNULL(RecheckTime, CAST('1988-12-25 00:00:00' AS DATETIME)),
                                                    IFNULL(RecheckUserID,''),
                                                    IFNULL(FQATime,  CAST('1988-12-25 00:00:00' AS DATETIME)),
                                                    IFNULL(FQAUser, ''),
                                                    A1_ResultImagePath,
                                                    A2_ResultImagePath,
                                                    A3_ResultImagePath,
                                                    A4_ResultImagePath,
                                                    MesImagePath,
                                                    CheckMode
                                                FROM
                                                    production_data
                                                WHERE
                                                        RecheckState = 4
                                                ORDER BY ProductDate";

        private readonly string _fqaNotUploadSQLNG = @"SELECT 
	                                                Iden,
                                                    ProductSN,
                                                    quality,
                                                    ResultPath,
                                                    A1_OriginalImagePath,
                                                    A2_OriginalImagePath,
                                                    A3_OriginalImagePath,
                                                    A4_OriginalImagePath,
                                                    RecheckState,
                                                    IFNULL(RecheckTime, CAST('1988-12-25 00:00:00' AS DATETIME)),
                                                    IFNULL(RecheckUserID,''),
                                                    IFNULL(FQATime,  CAST('1988-12-25 00:00:00' AS DATETIME)),
                                                    IFNULL(FQAUser, ''),
                                                    A1_ResultImagePath,
                                                    A2_ResultImagePath,
                                                    A3_ResultImagePath,
                                                    A4_ResultImagePath,
                                                    MesImagePath,
                                                    CheckMode
                                                FROM
                                                    production_data
                                                WHERE
                                                        RecheckState = 2
                                                ORDER BY ProductDate";

        private readonly string _updateSQL = @"UPDATE production_data
                                                SET RecheckState = {0},
                                                RecheckTime = {1},
                                                RecheckUserID = {2},
                                                FQATime = {3},
                                                FQAUser = {4}
                                            WHERE
	                                            Iden={5}";

        private readonly string _uploadSQL = @"UPDATE production_data
                                                SET RecheckState = {1}
                                            WHERE
	                                            Iden={2}";


        private List<RecheckRecord> _recheckRecords = new List<RecheckRecord>();
        private List<RecheckRecord> _fqaRecords = new List<RecheckRecord>();
        private List<RecheckRecord> _fqaNotUpload = new List<RecheckRecord>();
        private List<RecheckRecord> _fqaNotUploadNG = new List<RecheckRecord>();

        public List<RecheckRecord> RecheckRecords
        {
            get { return this._recheckRecords; }
            set
            {
                this._recheckRecords = value;
                RaisePropertyChanged("RecheckRecords");
            }
        }

        public List<RecheckRecord> FQARecords
        {
            get { return this._fqaRecords; }
            set
            {
                this._fqaRecords = value;
                RaisePropertyChanged("FQARecords");
            }
        }

        public List<RecheckRecord> FQANotUpload
        {
            get { return this._fqaNotUpload; }
            set
            {
                this._fqaNotUpload = value;
                RaisePropertyChanged("FQANotUpload");
            }
        }

        public List<RecheckRecord> FQANotUploadNG
        {
            get { return this._fqaNotUploadNG; }
            set
            {
                this._fqaNotUploadNG = value;
                RaisePropertyChanged("FQANotUploadNG");
            }
        }

        public void SetDbConfig()
        {
        }

        public void Init()
        {
            //this.RefreshRecheckList(true);
            //this.RefreshRecheckList(false);

            //this.ModifyRecheckList(true);
            //this.ModifyRecheckList(false);
        }

        public void UnInit()
        {

        }

        /// <summary>
        /// 刷新复检列表
        /// </summary>
        public void RefreshRecheckList(bool isRecheckOrFQA, bool isNotUpload = false, bool isNotUploadOK = true, bool isNotWaitCheck = false)
        {
            string sql = string.Empty;

            List<RecheckRecord> tmpList = new List<RecheckRecord>();
            if (isNotUpload)
            {
                if (isNotUploadOK)
                {
                    this.FQANotUpload = new List<RecheckRecord>();
                    sql = this._fqaNotUploadSQL;
                }
                else
                {
                    this.FQANotUploadNG = new List<RecheckRecord>();
                    sql = this._fqaNotUploadSQLNG;
                }
            }
            else
            {
                if (isRecheckOrFQA)
                {
                    this.RecheckRecords = new List<RecheckRecord>();
                    sql = isNotWaitCheck ? this._recheckSelectSQL : this._recheckAllSelectSQL;
                }
                else
                {
                    this.FQARecords = new List<RecheckRecord>();
                    sql = this._fqaSelectSQL;
                }
            }

            DataSet ds = baseFacade.equipDB.ExecuteDataSet(CommandType.Text, sql);
            if (ds.Tables[0].Rows.Count < 1) return;

            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                RecheckRecord model = new RecheckRecord();

                model.RecordID = (int)ds.Tables[0].Rows[i][0];
                model.BatteryBarCode = (string)ds.Tables[0].Rows[i][1];
                model.FinalResult = (int)ds.Tables[0].Rows[i][2];
                model.ResultPath = (string)ds.Tables[0].Rows[i][3];
                model.A1_OriginalImagePath = (string)ds.Tables[0].Rows[i][4];
                model.A2_OriginalImagePath = (string)ds.Tables[0].Rows[i][5];
                model.A3_OriginalImagePath = (string)ds.Tables[0].Rows[i][6];
                model.A4_OriginalImagePath = (string)ds.Tables[0].Rows[i][7];
                model.RecheckState = (int)ds.Tables[0].Rows[i][8];
                model.RecheckTime = (DateTime)ds.Tables[0].Rows[i][9];
                model.RecheckUserID = (string)ds.Tables[0].Rows[i][10];
                model.FQATime = (DateTime)ds.Tables[0].Rows[i][11];
                model.FQAUser = (string)ds.Tables[0].Rows[i][12];

                model.A1_ResultImagePath = (string)ds.Tables[0].Rows[i][13];
                model.A2_ResultImagePath = (string)ds.Tables[0].Rows[i][14];
                model.A3_ResultImagePath = (string)ds.Tables[0].Rows[i][15];
                model.A4_ResultImagePath = (string)ds.Tables[0].Rows[i][16];
                model.StfPath = (string)ds.Tables[0].Rows[i][17];
                model.CheckMode = (int)ds.Tables[0].Rows[i][18];

                if (File.Exists(model.A1_ResultImagePath) && File.Exists(model.A2_ResultImagePath)
                    && File.Exists(model.A3_ResultImagePath) && File.Exists(model.A4_ResultImagePath))
                {
                    tmpList.Add(model);
                }
            }

            if (isNotUpload)
            {
                if (isNotUploadOK)
                {
                    this.FQANotUpload = tmpList;
                }
                else
                {
                    this.FQANotUploadNG = tmpList;
                }
            }
            else
            {
                if (isRecheckOrFQA)
                {
                    this.RecheckRecords = tmpList;
                }
                else
                {
                    this.FQARecords = tmpList;
                }
            }
        }

        /// <summary>
        /// 更新复检列表
        /// 全部更新
        /// </summary>
        public void ModifyRecheckList(bool isRecheckOrFQA)
        {
            bool success = false;
            string exceptionMsg = string.Empty;

            var list = isRecheckOrFQA ? this._recheckRecords : this._fqaRecords;

            foreach (var item in list)
            {
                try
                {
                    if(isRecheckOrFQA == false)
                    {
                        string sql = $"UPDATE production_data SET RecheckState = {item.RecheckState}, RecheckTime = '{item.RecheckTime}', FQATime = '{item.FQATime}', FQAUser = '{item.FQAUser}' WHERE Iden={item.RecordID};";
                        baseFacade.equipDB.ExecuteNonQuery(CommandType.Text, sql);
                    }
                    else
                    {
                        string sql = $"UPDATE production_data SET RecheckState = {item.RecheckState}, RecheckTime = '{item.RecheckTime}', RecheckUserID = '{item.RecheckUserID}', FQATime = '{item.FQATime}' WHERE Iden={item.RecordID};";
                        baseFacade.equipDB.ExecuteNonQuery(CommandType.Text, sql);
                    }
                    
                }
                catch (Exception _ex)
                {
                    exceptionMsg = _ex.ToString();
                }
                if (!success) LogHelper.Error(exceptionMsg);
            }
        }

        /// <summary>
        /// 数据上传(单个)
        /// </summary>
        /// <param name="record"></param>
        public void ModifyOnUpload(RecheckRecord record)
        {
            bool success = false;
            string exceptionMsg = string.Empty;

            try
            {
                string sql = $"UPDATE production_data SET RecheckState = {record.RecheckState} WHERE Iden={record.RecordID};";
                baseFacade.equipDB.ExecuteNonQuery(CommandType.Text, sql);

                sql = $"UPDATE production_data SET `MesImagePath` = '{record.MesImagePath}' WHERE Iden={record.RecordID};";
                baseFacade.equipDB.ExecuteNonQuery(CommandType.Text, sql);
            }
            catch (Exception _ex)
            {
                exceptionMsg = _ex.ToString();
            }
            if (!success) LogHelper.Error(exceptionMsg);
        }
    }
}
