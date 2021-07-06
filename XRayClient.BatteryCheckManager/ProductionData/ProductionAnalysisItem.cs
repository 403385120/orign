using Shuyz.Framework.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XRayClient.BatteryCheckManager
{
    public class ProductionAnalysisItem:ObservableObject
    {
        private int _hour = 0;
        private int _count = 0;
        //each hour at one day
        public int Hour
        {
            get { return this._hour; }
            set
            {
                this._hour = value;
                RaisePropertyChanged("Hour");
            }
        }
        //the number each hour
        public int Count
        {
            get { return this._count; }
            set
            {
                this._count = value;
                RaisePropertyChanged("Count");
            }
        }
    }
}
