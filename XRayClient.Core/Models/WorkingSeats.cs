using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shuyz.Framework.Mvvm;

namespace XRayClient.Core
{
    /// <summary>
    /// Cycling two seats
    /// </summary>
    public class WorkingSeats : ObservableObject
    {
        public BatterySeat _seat1 = new BatterySeat();
        public BatterySeat _seat2 = new BatterySeat();

        private bool _isSwitched = false;
        private object _syncObj = new object();
        
        public BatterySeat FrontSeat
        {
            get
            {
                lock (this._syncObj)
                {
                    if (!_isSwitched) return _seat1;
                    else return this._seat2;
                }
            }
        }

        public BatterySeat BackSeat
        {
            get
            {
                lock(this._syncObj)
                {
                    if (!_isSwitched) return _seat2;
                    else return _seat1;
                }
            }
        }

        public void Switch()
        {
            lock(this._syncObj)
            {
                this._isSwitched = !this._isSwitched;

                RaisePropertyChanged("FrontSeat");
                RaisePropertyChanged("BackSeat");
            }
        }

        public void ClearAll()
        {
            this._isSwitched = false;

            this.BackSeat.Reset();
            this.FrontSeat.Reset();

            RaisePropertyChanged("FrontSeat");
            RaisePropertyChanged("BackSeat");
        }
    }
}
