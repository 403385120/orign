using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shuyz.Framework.Mvvm;

namespace XRayClient.BatteryCheckManager
{
    public class ProductionRateItem : ObservableObject
    {
        private DateTime _recordTime = DateTime.MinValue;
        private float _totalNum = 0;
        private float _okNum = 0;
        private float _ngNum = 0;
        private float _yieldNum = 0;

        public DateTime RecordTime
        {
            get { return this._recordTime; }
            set
            {
                this._recordTime = value;
                RaisePropertyChanged("RecordTime");
            }
        }

        public float TotalNum
        {
            get { return this._totalNum; }
            set
            {
                this._totalNum = value;
                RaisePropertyChanged("TotalNum");
            }
        }

        public float OKNum
        {
            get { return this._okNum; }
            set
            {
                this._okNum = value;
                RaisePropertyChanged("OKNum");
            }
        }

        public float NGNum
        {
            get { return this._ngNum; }
            set
            {
                this._ngNum = value;
                RaisePropertyChanged("NGNum");
            }
        }

        public float YieldNum
        {
            get {
                if (TotalNum == 0)
                {
                    _yieldNum = 0;
                }
                else
                {
                    _yieldNum = (_okNum / _totalNum) * 100;
                    _yieldNum = (float)Math.Round(_yieldNum, 2);
                }
                return this._yieldNum;
            }
            set
            {
                this._yieldNum = value;
                RaisePropertyChanged("YieldNum");
            }
        }

    }

    public class BatteryCheckRecord: ObservableObject
    {
        private DateTime _createTime = DateTime.Now;
        private int _finalResult = 0;

        public DateTime CreateTime
        {
            get { return _createTime; }
            set
            {
                _createTime = value;
            }
        }

        public int FinalResult
        {
            get { return _finalResult; }
            set
            {
                _finalResult = value;
            }
        }
    }
}
