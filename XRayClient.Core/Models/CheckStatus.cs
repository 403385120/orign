using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shuyz.Framework.Mvvm;

namespace XRayClient.Core
{
    /// <summary>
    /// 检测状态
    /// </summary>
    public class CheckStatus : ObservableObject
    {
        private ECheckModes _checkMode = ECheckModes.FourSides;
        private ECheckExtensions _checkExtension = ECheckExtensions.RunEmpty;
        private ECheckExtensions _activeCheckExtension = ECheckExtensions.RunEmpty;

        private int _alarmCount = 0;
        private bool _isReWorkMode = false;
        private Statistics _statistics = new Statistics();


        public CheckStatus()
        {
            this._checkExtension = ECheckExtensions.STF;
        }

        public StartupTestConfig MyStartupTestConfig
        {
            get { return CheckParamsConfig.Instance.MyStartupTestConfig; }
        }



        public int AlarmCount
        {
            get { return this._alarmCount; }
            set
            {
                this._alarmCount = value;
                RaisePropertyChanged("AlarmCount");
            }
        }

        public ECheckModes CheckMode
        {
            get { return this._checkMode; }
            set
            {
                this._checkMode = value;
                RaisePropertyChanged("CheckMode");
            }
        }

        public ECheckExtensions ActiveCheckExtension
        {
            get { return this._activeCheckExtension; }
            set
            {
                this._activeCheckExtension = value;
                RaisePropertyChanged("ActiveCheckExtension");
            }
        }

        public ECheckExtensions CheckExtension
        {
            get { return this._checkExtension; }
            set
            {
                this._checkExtension = value;
                RaisePropertyChanged("CheckExtension");
            }
        }

        public bool IsReworkMode
        {
            get { return this._isReWorkMode; }
            set
            {
                this._isReWorkMode = value;
                RaisePropertyChanged("IsReworkMode");
            }
        }

        public Statistics MyStatistics
        {
            get
            {
                return this._statistics;
            }
            private set { this._statistics = value; }
        }
    }
}
