using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shuyz.Framework.Mvvm;

namespace XRayClient.Core
{
    /// <summary>
    /// 点检设置
    /// </summary>
    public class StartupTestConfig : ObservableObject
    {
        private DateTime _lastTestTime = DateTime.MinValue; // 上次点检时间
        private string _lastTestBy = string.Empty;          // 上次点检人
        private float _testInterval = 12;                   // 点检间隔(小时)
        private int _testNGNum = 3;                         // NG数
        private bool _isEnabled = true;                    // 是否要求点检
        private DateTime _lastCheckOutTime = DateTime.MinValue;
        private int _lastCheckOutHour = 7;

        private DateTime _lastPinCheckTime = DateTime.MinValue;
        private int _lastPinCheckHour = 7;

        public DateTime lastTestTime
        {
            get
            {
                return _lastTestTime;
            }
            set
            {
                _lastTestTime = value;
                RaisePropertyChanged("lastTestTime");
            }
        }

        public string lastTestBy
        {
            get
            {
                return _lastTestBy;
            }
            set
            {
                _lastTestBy = value;
                RaisePropertyChanged("lastTestBy");
            }
        }

        public float testInterval
        {
            get
            {
                return _testInterval;
            }
            set
            {
                _testInterval = value;
                RaisePropertyChanged("testInterval");
            }
        }

        public int TestNGNum
        {
            get { return this._testNGNum; }
            set
            {
                this._testNGNum = value;
                RaisePropertyChanged("TestNGNum");
            }
        }

        public bool IsEnabled
        {
            get { return this._isEnabled; }
            set
            {
                this._isEnabled = false;
                RaisePropertyChanged("IsEnabled");
            }
        }

        public DateTime LastCheckOutTime
        {
            get
            {
                return _lastCheckOutTime;
            }
            set
            {
                _lastCheckOutTime = value;
                RaisePropertyChanged("LastCheckOutTime");
            }
        }

        public int LastCheckOutHour
        {
            get { return this._lastCheckOutHour; }
            set
            {
                this._lastCheckOutHour = value;
                RaisePropertyChanged("LastCheckOutHour");
            }
        }

        public DateTime LastPinCheckTime
        {
            get
            {
                return _lastPinCheckTime;
            }
            set
            {
                _lastPinCheckTime = value;
                RaisePropertyChanged("LastPinCheckTime");
            }
        }

        public int LastPinCheckHour
        {
            get { return this._lastPinCheckHour; }
            set
            {
                this._lastPinCheckHour = value;
                RaisePropertyChanged("LastPinCheckHour");
            }
        }
    }
}
