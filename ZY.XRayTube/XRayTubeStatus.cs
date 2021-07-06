using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ATL.Common;

namespace ZY.XRayTube
{
    public class XRayTubeStatus : ViewModelBase
    {
        private XRayTubeTypes _xrayTubeType = XRayTubeTypes.HamamatsuXrayTube;
        private bool _isConnectSuccess = false;
        private bool _iswarmupComplete = false;
        private bool _isInterLockOn = false;
        private bool _isHardwareError = false;
        private int _useHoursInTotal = 0;            //光管使用的累计时间
        private ETubeStatus _tubeStatus = ETubeStatus.OtherStatus;

        private bool _shouldXRayOn = false;              //光管是否打开   true-打开  false-关闭
        private int _actualVoltage = 0;
        private int _actualCurrent = 0;

        private int _alarmTime = 10000;
     
        public XRayTubeTypes XRayTubeType
        {
            get { return this._xrayTubeType; }
            set
            {
                if (this._xrayTubeType == value) return;

                this._xrayTubeType = value;
                NotiFy("XRayTubeType");
            }
        }

        public bool IsConnectSuccess
        {
            get { return this._isConnectSuccess; }
            set
            {
                if (this._isConnectSuccess == value) return;

                this._isConnectSuccess = value;
                NotiFy("IsConnectSuccess");
            }
        }

        public bool IsWarmupComplete
        {
            get { return this._iswarmupComplete; }
            set
            {
                if (this._iswarmupComplete == value) return;

                this._iswarmupComplete = value;
                NotiFy("IsWarmupComplete");
            }
        }

        public bool IsInterLockOn
        {
            get { return this._isInterLockOn; }
            set
            {
                if (this._isInterLockOn == value) return;

                this._isInterLockOn = value;
                NotiFy("IsInterLockOn");
            }
        }

        public bool IsHardwareError
        {
            get { return this._isHardwareError; }
            set
            {
                if (this._isHardwareError == value) return;

                this._isHardwareError = value;
                NotiFy("IsHardwareError");
            }
        }

        public int UseHoursInTotal
        {
            get { return this._useHoursInTotal; }
            set
            {
                if (this._useHoursInTotal == value) return;

                this._useHoursInTotal = value;
                NotiFy("UseHoursInTotal");
            }
        }

        public ETubeStatus TubeStatus
        {
            get { return this._tubeStatus; }
            set
            {
                if (this._tubeStatus == value) return;

                this._tubeStatus = value;
                NotiFy("TubeStatus");
            }
        }

        public bool ShouldXrayOn
        {
            get { return this._shouldXRayOn; }
            set
            {
                if (this._shouldXRayOn == value) return;

                this._shouldXRayOn = value;
                NotiFy("ShouldXrayOn");
            }
        }
       
        public int ActualVoltage
        {
            get { return this._actualVoltage; }
            set
            {
                if (this._actualVoltage == value) return;

                this._actualVoltage = value;
                NotiFy("ActualVoltage");
            }
        }

        public int ActualCurrent
        {
            get { return this._actualCurrent; }
            set
            {
                if (this._actualCurrent == value) return;

                this._actualCurrent = value;
                NotiFy("ActualCurrent");
            }
        }

        public int AlarmTime
        {
            get { return _alarmTime; }
            set
            {
                if (this._alarmTime == value) return;
                this._alarmTime = value;
                NotiFy("AlarmTime");
            }
        }

        public XRayTubeStatus DeepCopy()
        {
            XRayTubeStatus _xRayTubeStatus = new XRayTubeStatus();
            _xRayTubeStatus.XRayTubeType = this.XRayTubeType;
            _xRayTubeStatus.IsConnectSuccess = this.IsConnectSuccess;
            _xRayTubeStatus.IsHardwareError = this.IsHardwareError;
            _xRayTubeStatus.IsInterLockOn = this.IsInterLockOn;
            _xRayTubeStatus.IsWarmupComplete = this.IsWarmupComplete;
            _xRayTubeStatus.ShouldXrayOn = this.ShouldXrayOn;
            _xRayTubeStatus.TubeStatus = this.TubeStatus;
            _xRayTubeStatus.UseHoursInTotal = this.UseHoursInTotal;
            _xRayTubeStatus.ActualCurrent = this.ActualCurrent;
            _xRayTubeStatus.ActualVoltage = this.ActualVoltage;
            _xRayTubeStatus.AlarmTime = this._alarmTime;

            return _xRayTubeStatus;
        }

        public override string ToString()
        {
            return "XRayTubeType: " + this.XRayTubeType.ToString() + Environment.NewLine
                + "IsConnectSuccess: " + this.IsConnectSuccess.ToString() + Environment.NewLine
                + "IsHardwareError: " + this.IsHardwareError.ToString() + Environment.NewLine
                + "IsInterLockOn: " + this.IsInterLockOn.ToString() + Environment.NewLine
                + "IsWarmupComplete: " + this.IsWarmupComplete.ToString() + Environment.NewLine
                + "ShouldXrayOn: " + this.ShouldXrayOn.ToString() + Environment.NewLine
                + "TubeStatus: " + this.TubeStatus.ToString() + Environment.NewLine
                + "UseHoursInTotal: " + this.UseHoursInTotal.ToString() + Environment.NewLine
                + "ActualCurrent: " + this.ActualCurrent.ToString() + Environment.NewLine
                + "ActualVoltage: " + this.ActualVoltage.ToString() + Environment.NewLine
                + "AlarmTime: " + this.AlarmTime.ToString() + Environment.NewLine;
        }

    }
}
