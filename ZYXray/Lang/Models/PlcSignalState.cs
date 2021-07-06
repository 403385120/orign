using Shuyz.Framework.Mvvm;

namespace ZYXray.Models
{
    public class PlcSignalState : ObservableObject
    {
        private static PlcSignalState _instance = new PlcSignalState();

        public static PlcSignalState Instance
        {
            get { return _instance; }
        }

        private bool _softwareReset;
        public bool _xRayON;
        public bool _heartBeat;
        public bool _scanBarcodeSignal1;
        public bool _scanBarcodeSignal2;
        public bool _iVTestSignal1;
        public bool _iVTestSignal2;
        public bool _iVTestSignal3;
        public bool _iVTestSignal4;
        public bool _resistanceSignal1;
        public bool _resistanceSignal2;
        public bool _voltageSignal1;
        public bool _voltageSignal2;
        public bool _mDISignal1;
        public bool _mDISignal2;
        public bool _mDISignal3;
        public bool _mDISignal4;
        public bool _thicknessMeasureSignal1;
        public bool _thicknessMeasureSignal2;
        public bool _thicknessMeasureSignal3;
        public bool _thicknessMeasureSignal4;
        public bool _battery1CornerPhotoSignal;
        public bool _battery2CornerPhotoSignal;
        public bool _battery3CornerPhotoSignal;
        public bool _battery4CornerPhotoSignal;
        public bool _photoResultSignal;
        public bool _needleNGSignal1;
        public bool _needleNGSignal2;
        public bool _needleNGSignal3;
        public bool _needleNGSignal4;
        public bool _knifeNGSignal1;
        public bool _knifeNGSignal2;
        public bool _knifeNGSignal3;
        public bool _knifeNGSignal4;
        public bool _iVPlatform1;
        public bool _iVPlatform2;
        public bool _barcodebindingSignal1;
        public bool _barcodebindingSignal2;

        public bool _pcReady;
        public bool _softResetComplete;
        public bool _softwareOnState;
        public bool _softwareHeartbeatPackage;
        public bool _batteryScanning1OK;
        public bool _batteryScanning1NG;
        public bool _batteryScanning2OK;
        public bool _batteryScanning2NG;
        public bool _iVComplete;
        public bool _resistanceComplete1;
        public bool _resistanceComplete2;
        public bool _voltageComplete1;
        public bool _voltageComplete2;
        public bool _mDIComplete1;
        public bool _mDIComplete2;
        public bool _thicknessComplete1;
        public bool _thicknessComplete2;
        public bool _thicknessComplete3;
        public bool _thicknessComplete4;
        public bool _battery1CornerPhotoComplete;
        public bool _battery2CornerPhotoComplete;
        public bool _battery3CornerPhotoComplete;
        public bool _battery4CornerPhotoComplete;
        public bool _resultOK;
        public bool _iVNG;
        public bool _iVNoConduction;
        public bool _otherNG;
        public bool _voltageNG;
        public bool _resistanceNG;
        public bool _kNG;
        public bool _mDING;
        public bool _thicknessNG;
        public bool _xRAYNG;
        public bool _barcodeBindingComplete1;
        public bool _barcodeBindingComplete2;

        public bool SoftwareReset
        {
            get { return this._softwareReset; }
            set
            {
                this._softwareReset = value;
                RaisePropertyChanged("SoftwareReset");
            }

        }
        public bool XRayON
        {
            get { return this._xRayON; }
            set
            {
                this._xRayON = value;
                RaisePropertyChanged("XRayON");
            }
        }

        public bool HeartBeat
        {
            get { return this._heartBeat; }
            set
            {
                this._heartBeat = value;
                RaisePropertyChanged("HeartBeat");
            }
        }

        public bool ScanBarcodeSignal1
        {
            get { return this._scanBarcodeSignal1; }
            set
            {
                this._scanBarcodeSignal1 = value;
                RaisePropertyChanged("ScanBarcodeSignal1");
            }
        }
        public bool ScanBarcodeSignal2
        {
            get { return this._scanBarcodeSignal2; }
            set
            {
                this._scanBarcodeSignal2 = value;
                RaisePropertyChanged("ScanBarcodeSignal2");
            }
        }
        public bool IVTestSignal1
        {
            get { return this._iVTestSignal1; }
            set
            {
                this._iVTestSignal1 = value;
                RaisePropertyChanged("IVTestSignal1");
            }
        }
        public bool IVTestSignal2
        {
            get { return this._iVTestSignal2; }
            set
            {
                this._iVTestSignal2 = value;
                RaisePropertyChanged("IVTestSignal2");
            }
        }
        public bool IVTestSignal3
        {
            get { return this._iVTestSignal3; }
            set
            {
                this._iVTestSignal3 = value;
                RaisePropertyChanged("IVTestSignal3");
            }
        }
        public bool IVTestSignal4
        {
            get { return this._iVTestSignal4; }
            set
            {
                this._iVTestSignal4 = value;
                RaisePropertyChanged("IVTestSignal4");
            }
        }

        public bool ResistanceSignal1
        {
            get { return this._resistanceSignal1; }
            set
            {
                this._resistanceSignal1 = value;
                RaisePropertyChanged("ResistanceSignal1");
            }
        }

        public bool ResistanceSignal2
        {
            get { return this._resistanceSignal2; }
            set
            {
                this._resistanceSignal2 = value;
                RaisePropertyChanged("ResistanceSignal2");
            }
        }

        public bool VoltageSignal1
        {
            get { return this._voltageSignal1; }
            set
            {
                this._voltageSignal1 = value;
                RaisePropertyChanged("VoltageSignal1");
            }
        }

        public bool VoltageSignal2
        {
            get { return this._voltageSignal2; }
            set
            {
                this._voltageSignal2 = value;
                RaisePropertyChanged("VoltageSignal2");
            }
        }

        public bool MDISignal1
        {
            get { return this._mDISignal1; }
            set
            {
                this._mDISignal1 = value;
                RaisePropertyChanged("MDISignal1");
            }
        }

        public bool MDISignal2
        {
            get { return this._mDISignal2; }
            set
            {
                this._mDISignal2 = value;
                RaisePropertyChanged("MDISignal2");
            }
        }
        public bool MDISignal3
        {
            get { return this._mDISignal3; }
            set
            {
                this._mDISignal3 = value;
                RaisePropertyChanged("MDISignal3");
            }
        }

        public bool MDISignal4
        {
            get { return this._mDISignal4; }
            set
            {
                this._mDISignal4 = value;
                RaisePropertyChanged("MDISignal4");
            }
        }
        public bool ThicknessMeasureSignal1
        {
            get { return this._thicknessMeasureSignal1; }
            set
            {
                this._thicknessMeasureSignal1 = value;
                RaisePropertyChanged("ThicknessMeasureSignal1");
            }
        }
        public bool ThicknessMeasureSignal2
        {
            get { return this._thicknessMeasureSignal2; }
            set
            {
                this._thicknessMeasureSignal2 = value;
                RaisePropertyChanged("ThicknessMeasureSignal2");
            }
        }

        public bool ThicknessMeasureSignal3
        {
            get { return this._thicknessMeasureSignal3; }
            set
            {
                this._thicknessMeasureSignal3 = value;
                RaisePropertyChanged("ThicknessMeasureSignal3");
            }
        }
        public bool ThicknessMeasureSignal4
        {
            get { return this._thicknessMeasureSignal4; }
            set
            {
                this._thicknessMeasureSignal4 = value;
                RaisePropertyChanged("ThicknessMeasureSignal4");
            }
        }
        public bool Battery1CornerPhotoSignal
        {
            get { return this._battery1CornerPhotoSignal; }
            set
            {
                this._battery1CornerPhotoSignal = value;
                RaisePropertyChanged("Battery1CornerPhotoSignal");
            }
        }
        public bool Battery2CornerPhotoSignal
        {
            get { return this._battery2CornerPhotoSignal; }
            set
            {
                this._battery2CornerPhotoSignal = value;
                RaisePropertyChanged("Battery2CornerPhotoSignal");
            }
        }
        public bool Battery3CornerPhotoSignal
        {
            get { return this._battery3CornerPhotoSignal; }
            set
            {
                this._battery3CornerPhotoSignal = value;
                RaisePropertyChanged("Battery3CornerPhotoSignal");
            }
        }
        public bool Battery4CornerPhotoSignal
        {
            get { return this._battery4CornerPhotoSignal; }
            set
            {
                this._battery4CornerPhotoSignal = value;
                RaisePropertyChanged("Battery4CornerPhotoSignal");
            }
        }
        public bool PhotoResultSignal
        {
            get { return this._photoResultSignal; }
            set
            {
                this._photoResultSignal = value;
                RaisePropertyChanged("PhotoResultSignal");
            }
        }
        public bool NeedleNGSignal1
        {
            get { return this._needleNGSignal1; }
            set
            {
                this._needleNGSignal1 = value;
                RaisePropertyChanged("NeedleNGSignal1");
            }
        }
        public bool NeedleNGSignal2
        {
            get { return this._needleNGSignal2; }
            set
            {
                this._needleNGSignal2 = value;
                RaisePropertyChanged("NeedleNGSignal2");
            }
        }
        public bool NeedleNGSignal3
        {
            get { return this._needleNGSignal3; }
            set
            {
                this._needleNGSignal3 = value;
                RaisePropertyChanged("NeedleNGSignal3");
            }
        }
        public bool NeedleNGSignal4
        {
            get { return this._needleNGSignal4; }
            set
            {
                this._needleNGSignal4 = value;
                RaisePropertyChanged("NeedleNGSignal4");
            }
        }
        public bool KnifeNGSignal1
        {
            get { return this._knifeNGSignal1; }
            set
            {
                this._knifeNGSignal1 = value;
                RaisePropertyChanged("KnifeNGSignal1");
            }
        }
        public bool KnifeNGSignal2
        {
            get { return this._knifeNGSignal2; }
            set
            {
                this._knifeNGSignal2 = value;
                RaisePropertyChanged("KnifeNGSignal2");
            }
        }
        public bool KnifeNGSignal3
        {
            get { return this._knifeNGSignal3; }
            set
            {
                this._knifeNGSignal3 = value;
                RaisePropertyChanged("KnifeNGSignal3");
            }
        }
        public bool KnifeNGSignal4
        {
            get { return this._knifeNGSignal4; }
            set
            {
                this._knifeNGSignal4 = value;
                RaisePropertyChanged("KnifeNGSignal4");
            }
        }
        public bool IVPlatform1
        {
            get { return this._iVPlatform1; }
            set
            {
                this._iVPlatform1 = value;
                RaisePropertyChanged("IVPlatform1");
            }
        }
        public bool IVPlatform2
        {
            get { return this._iVPlatform2; }
            set
            {
                this._iVPlatform2 = value;
                RaisePropertyChanged("IVPlatform2");
            }
        }

        public bool BarcodebindingSignal1
        {
            get { return this._barcodebindingSignal1; }
            set
            {
                this._barcodebindingSignal1 = value;
                RaisePropertyChanged("BarcodebindingSignal1");
            }
        }
        public bool BarcodebindingSignal2
        {
            get { return this._barcodebindingSignal2; }
            set
            {
                this._barcodebindingSignal2 = value;
                RaisePropertyChanged("BarcodebindingSignal2");
            }
        }

        public bool PCReady
        {
            get { return this._pcReady; }
            set
            {
                this._pcReady = value;
                RaisePropertyChanged("PCReady");
            }
        }

        public bool SoftResetComplete
        {
            get { return this._softResetComplete; }
            set
            {
                this._softResetComplete = value;
                RaisePropertyChanged("SoftResetComplete");
            }
        }

        public bool SoftwareOnState
        {
            get { return this._softwareOnState; }
            set
            {
                this._softwareOnState = value;
                RaisePropertyChanged("SoftwareOnState");
            }
        }
        public bool SoftwareHeartbeatPackage
        {
            get { return this._softwareHeartbeatPackage; }
            set
            {
                this._softwareHeartbeatPackage = value;
                RaisePropertyChanged("SoftwareHeartbeatPackage");
            }
        }
        public bool BatteryScanning1OK
        {
            get { return this._batteryScanning1OK; }
            set
            {
                this._batteryScanning1OK = value;
                RaisePropertyChanged("BatteryScanning1OK");
            }
        }
        public bool BatteryScanning1NG
        {
            get { return this._batteryScanning1NG; }
            set
            {
                this._batteryScanning1NG = value;
                RaisePropertyChanged("BatteryScanning1NG");
            }
        }
        public bool BatteryScanning2OK
        {
            get { return this._batteryScanning2OK; }
            set
            {
                this._batteryScanning2OK = value;
                RaisePropertyChanged("BatteryScanning2OK");
            }
        }
        public bool BatteryScanning2NG
        {
            get { return this._batteryScanning2NG; }
            set
            {
                this._batteryScanning2NG = value;
                RaisePropertyChanged("BatteryScanning2NG");
            }
        }
        public bool IVComplete
        {
            get { return this._iVComplete; }
            set
            {
                this._iVComplete = value;
                RaisePropertyChanged("IVComplete");
            }
        }

        public bool ResistanceComplete1
        {
            get { return this._resistanceComplete1; }
            set
            {
                this._resistanceComplete1 = value;
                RaisePropertyChanged("ResistanceComplete1");
            }
        }

        public bool ResistanceComplete2
        {
            get { return this._resistanceComplete2; }
            set
            {
                this._resistanceComplete2 = value;
                RaisePropertyChanged("ResistanceComplete2");
            }
        }

        public bool VoltageComplete1
        {
            get { return this._voltageComplete1; }
            set
            {
                this._voltageComplete1 = value;
                RaisePropertyChanged("VoltageComplete1");
            }
        }

        public bool VoltageComplete2
        {
            get { return this._voltageComplete2; }
            set
            {
                this._voltageComplete2 = value;
                RaisePropertyChanged("VoltageComplete2");
            }
        }
        public bool MDIComplete1
        {
            get { return this._mDIComplete1; }
            set
            {
                this._mDIComplete1 = value;
                RaisePropertyChanged("MDIComplete1");
            }
        }
        public bool MDIComplete2
        {
            get { return this._mDIComplete2; }
            set
            {
                this._mDIComplete2 = value;
                RaisePropertyChanged("MDIComplete2");
            }
        }
        public bool ThicknessComplete1
        {
            get { return this._thicknessComplete1; }
            set
            {
                this._thicknessComplete1 = value;
                RaisePropertyChanged("ThicknessComplete1");
            }
        }
        public bool ThicknessComplete2
        {
            get { return this._thicknessComplete2; }
            set
            {
                this._thicknessComplete2 = value;
                RaisePropertyChanged("ThicknessComplete2");
            }
        }
        public bool ThicknessComplete3
        {
            get { return this._thicknessComplete3; }
            set
            {
                this._thicknessComplete3 = value;
                RaisePropertyChanged("ThicknessComplete3");
            }
        }
        public bool ThicknessComplete4
        {
            get { return this._thicknessComplete4; }
            set
            {
                this._thicknessComplete4 = value;
                RaisePropertyChanged("ThicknessComplete4");
            }
        }
        public bool Battery1CornerPhotoComplete
        {
            get { return this._battery1CornerPhotoComplete; }
            set
            {
                this._battery1CornerPhotoComplete = value;
                RaisePropertyChanged("Battery1CornerPhotoComplete");
            }
        }
        public bool Battery2CornerPhotoComplete
        {
            get { return this._battery2CornerPhotoComplete; }
            set
            {
                this._battery2CornerPhotoComplete = value;
                RaisePropertyChanged("Battery2CornerPhotoComplete");
            }
        }
        public bool Battery3CornerPhotoComplete
        {
            get { return this._battery3CornerPhotoComplete; }
            set
            {
                this._battery3CornerPhotoComplete = value;
                RaisePropertyChanged("Battery3CornerPhotoComplete");
            }
        }
        public bool Battery4CornerPhotoComplete
        {
            get { return this._battery4CornerPhotoComplete; }
            set
            {
                this._battery4CornerPhotoComplete = value;
                RaisePropertyChanged("Battery4CornerPhotoComplete");
            }
        }
        public bool ResultOK
        {
            get { return this._resultOK; }
            set
            {
                this._resultOK = value;
                RaisePropertyChanged("ResultOK");
            }
        }
        public bool IVNG
        {
            get { return this._iVNG; }
            set
            {
                this._iVNG = value;
                RaisePropertyChanged("IVNG");
            }
        }
        public bool IVNoConduction
        {
            get { return this._iVNoConduction; }
            set
            {
                this._iVNoConduction = value;
                RaisePropertyChanged("IVNoConduction");
            }
        }
        public bool OtherNG
        {
            get { return this._otherNG; }
            set
            {
                this._otherNG = value;
                RaisePropertyChanged("OtherNG");
            }
        }
        public bool VoltageNG
        {
            get { return this._voltageNG; }
            set
            {
                this._voltageNG = value;
                RaisePropertyChanged("VoltageNG");
            }
        }
        public bool ResistanceNG
        {
            get { return this._resistanceNG; }
            set
            {
                this._resistanceNG = value;
                RaisePropertyChanged("ResistanceNG");
            }
        }
        public bool KNG
        {
            get { return this._kNG; }
            set
            {
                this._kNG = value;
                RaisePropertyChanged("KNG");
            }
        }
        public bool MDING
        {
            get { return this._mDING; }
            set
            {
                this._mDING = value;
                RaisePropertyChanged("MDING");
            }
        }
        public bool ThicknessNG
        {
            get { return this._thicknessNG; }
            set
            {
                this._thicknessNG = value;
                RaisePropertyChanged("ThicknessNG");
            }
        }
        public bool XRAYNG
        {
            get { return this._xRAYNG; }
            set
            {
                this._xRAYNG = value;
                RaisePropertyChanged("XRAYNG");
            }
        }

        public bool BarcodeBindingComplete1
        {
            get { return this._barcodeBindingComplete1; }
            set
            {
                this._barcodeBindingComplete1 = value;
                RaisePropertyChanged("BarcodeBindingComplete1");
            }
        }

        public bool BarcodeBindingComplete2
        {
            get { return this._barcodeBindingComplete2; }
            set
            {
                this._barcodeBindingComplete2 = value;
                RaisePropertyChanged("BarcodeBindingComplete2");
            }
        }

    }
}
