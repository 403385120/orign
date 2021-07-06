using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shuyz.Framework.Mvvm;

namespace ZYXray.Models
{
    public class CncButton : ObservableObject
    {
        private static CncButton _instance = new CncButton();

        public static CncButton Instance
        {
            get { return _instance; }
        }

        private bool isStartBtnEnable = true;
        private bool isStopBtnEnable = false;

        public bool IsStartBtnEnable
        {
            get { return isStartBtnEnable; }
            set
            {
                isStartBtnEnable = value;
                RaisePropertyChanged("IsStartBtnEnable");
            }
        }

        public bool IsStopBtnEnable
        {
            get { return isStopBtnEnable; }
            set
            {
                isStopBtnEnable = value;
                RaisePropertyChanged("IsStopBtnEnable");
            }
        }
    }
}
