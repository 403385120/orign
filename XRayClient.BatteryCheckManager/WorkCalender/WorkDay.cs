using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XRayClient.BatteryCheckManager
{
    public class WorkDay
    {
        private int _recordID = -1;
        private DateTime _workDate = new DateTime();        // 工作日期
        private bool _isUsed = false;                       // 当天是否使用
        private float _usedHours = 0;                       // 使用时间
        private int _okNum = 0;                             // OK数
        private int _ngNum = 0;                             // NG数

        public int RecordID
        {
            get { return this._recordID; }
            set
            {
                this._recordID = value;
            }
        }

        public DateTime WorkDate
        {
            get { return this._workDate; }
            set
            {
                this._workDate = value;
            }
        }

        public bool IsUsed
        {
            get { return this._isUsed; }
            set
            {
                this._isUsed = value;
            }
        }

        public float UsedHours
        {
            get { return this._usedHours; }
            set
            {
                this._usedHours = value;
            }
        }

        public int OKNum
        {
            get { return this._okNum; }
            set
            {
                this._okNum = value;
            }
        }

        public int NGNum
        {
            get { return this._ngNum; }
            set
            {
                this._ngNum = value;
            }
        }
        
    }
}
