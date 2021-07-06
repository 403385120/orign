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
    public class WorkCalenderManage : ObservableObject
    {
        private readonly string _tableName = "@zy_work_calender@";
    
        public void SetDbConfig()
        {
        }

        public void Init()
        {
            this.CheckAndCreateTable();

            this.RefreshWorkDaysList(DateTime.Now);
        }

        public void UnInit()
        {
            // do nothing
        }

        public List<WorkDay> _workDaysList = new List<WorkDay>();

        public List<WorkDay> WorkDaysList
        {
            get { return this._workDaysList; }
            set
            {
                this._workDaysList = value;
                RaisePropertyChanged("WorkDaysList");
            }
        }

        /// <summary>
        /// 刷新生产信息
        /// </summary>
        /// <returns></returns>
        public bool RefreshWorkDaysList(DateTime year)
        {


            return true;
        }

        /// <summary>
        /// 添加或者更新WorkDay
        /// </summary>
        /// <param name="workDay"></param>
        /// <returns></returns>
        public bool AddOrUpdateWorkDay(WorkDay workDay)
        {
            return true;
        }

        private bool CheckAndCreateTable()
        {
            return true;
        }

    }
}
