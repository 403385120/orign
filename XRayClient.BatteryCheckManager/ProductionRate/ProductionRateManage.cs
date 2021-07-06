using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using Shuyz.Framework.Mvvm;



namespace XRayClient.BatteryCheckManager
{
    /// <summary>
    /// 生产优率
    /// </summary>
    public class ProductionRateManage : ObservableObject
    {
        private readonly string _selectSql = "SELECT NOW() AS RecordTime, IFNULL(SUM(CASE FinalResult WHEN 1 THEN 1 ELSE 0 END), 0) AS OKNum, IFNULL(SUM(CASE FinalResult WHEN 0 THEN 1 ELSE 0 END), 0) AS NGNum FROM zy_battery_check WHERE CreateTime >= CURDATE() AND CreateTime <= NOW()";
        private readonly string _selectHistorySql = "SELECT CreateTime, FinalResult FROM zy_battery_check WHERE CreateTime >= '{0}' AND CreateTime <= '{1}' ORDER BY CreateTime ASC";
        private ProductionRateItem _CurrentProductionRateItem = new ProductionRateItem();


        public ProductionRateItem CurrentProductionRateItem
        {
            get { return this._CurrentProductionRateItem; }
            set
            {
                this._CurrentProductionRateItem = value;
                RaisePropertyChanged("CurrentProductionRateItem");
                //RaisePropertyChanged("PushRecordList_PushOnly");
                //RaisePropertyChanged("PushRecordList_UploadOnly");
            }
        }



        public void SetDbConfig()
        {
        }

        public void Init()
        {
            this.GetHistoryRecord(10, DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd 00:00:00")), DateTime.Now);
        }

        public void UnInit()
        {
            // do nothing
        }

        /// <summary>
        /// 获取当前生产信息
        /// </summary>
        /// <returns></returns>
        public ProductionRateItem GetProductionRateItem()
        {

            return _CurrentProductionRateItem;
        }

        /// <summary>
        /// 获取历史生产记录，按10分钟统计
        /// </summary>
        public void GetHistoryRecord(int SecondNum, DateTime dtStartTime, DateTime dtEndTime)
        {
        }

    }
}
