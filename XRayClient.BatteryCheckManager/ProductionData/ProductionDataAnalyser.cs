using Shuyz.Framework.Mvvm;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XRayClient.BatteryCheckManager
{
    public class ProductionDataAnalyser:ObservableObject
    {
        private readonly string _tableName = "@zy_battery_check@";
        private readonly string _selectSql = @"SELECT allHours.hour,IFNULL(SUMVALUE,0) AS SUMVALUE
                                                FROM (SELECT 0 AS HOUR
	                                                UNION SELECT 1 UNION SELECT 2
                                                    UNION SELECT 3 UNION SELECT 4
                                                    UNION SELECT 5 UNION SELECT 6
                                                    UNION SELECT 7 UNION SELECT 8
                                                    UNION SELECT 9 UNION SELECT 10
                                                    UNION SELECT 11 UNION SELECT 12
                                                    UNION SELECT 13 UNION SELECT 14
                                                    UNION SELECT 15 UNION SELECT 16
                                                    UNION SELECT 17 UNION SELECT 18
                                                    UNION SELECT 19 UNION SELECT 20
                                                    UNION SELECT 21 UNION SELECT 22
                                                    UNION SELECT 23) AS allHours
                                                LEFT JOIN
                                                 (SELECT HOURVALUE, SUM(CNT) SUMVALUE FROM (
	                                                SELECT HOUR(CreateTime) AS hourValue, 1 cnt
	                                                FROM {0}
	                                                WHERE DATE(CreateTime) =  {1} 
	                                                )TEST GROUP BY HOURVALUE 
                                                ) AS totals
                                                ON allHours.hour = totals.HOURVALUE ORDER BY HOUR";
        private List<ProductionAnalysisItem> _analysisResultList = new List<ProductionAnalysisItem>();
        public List<ProductionAnalysisItem> AnalysisResultList
        {
            get { return this._analysisResultList; }
            set
            {
                this._analysisResultList = value;
                RaisePropertyChanged("AnalysisResultList");
            }
        }

        public void SetDbConfig()
        {
        }

        public void Init()
        {
            this.Analysis(DateTime.Now);
        }

        public void UnInit()
        {

        }

        public bool Analysis(DateTime SpecifiedTime)
        {
            return true;
        }
    }
}
