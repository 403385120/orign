using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using Shuyz.Framework.Mvvm;

/// <summary>
/// 实时检测记录表
/// </summary>
namespace XRayClient.BatteryCheckManager
{
    public class ProductionDataManage : ObservableObject
    {
        private readonly string _tableName = "@zy_battery_check@";
        private readonly string _selectSql = "SELECT RecordId, BatteryBarCode, StartTiem, EndTime, FinalResult, A1_Min, A1_Max, A2_Min, A2_Max, A3_Min, A3_Max, A4_Min, A4_Max FROM {0} ORDER BY RecordID Desc LIMIT 20;";

        public void SetDbConfig()
        {
        }

        public void Init()
        {
            this.RefreshProductionDataList();
        }

        public void UnInit()
        {
            // do nothing
        }

        public List<BatteryCheck> _productionDataList = new List<BatteryCheck>();

        public List<BatteryCheck> ProductionDataList
        {
            get { return this._productionDataList; }
            set
            {
                this._productionDataList = value;
                RaisePropertyChanged("ProductionDataList");
            }
        }

        /// <summary>
        /// 刷新生产信息
        /// </summary>
        /// <returns></returns>
        public bool RefreshProductionDataList()
        {

            return true;
        }
        
    }
}
