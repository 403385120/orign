using System;
using System.Text;
using Shuyz.Framework.Mvvm;
using System.Runtime.InteropServices;
using System.IO;

namespace XRayClient.BatteryCheckManager
{
    /// <summary>
    /// 生产记录导出选项配置
    /// </summary>
    public class CheckRecordExportConfig : ObservableObject
    {
        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key,
                                                  string val, string filePath);
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def,
                                                          StringBuilder retVal, int size, string filePath);

        private readonly string _configFile = Path.Combine(Environment.CurrentDirectory, "CheckRecordExportConfig.ini");

        private static CheckRecordExportConfig _instance = new CheckRecordExportConfig();
        public static CheckRecordExportConfig Instance
        {
            get { return _instance; }
        }
        private CheckRecordExportConfig()
        {
            if (!File.Exists(this._configFile))
            {
                this.Write();
            }

            this.Read();
        }

        private void Read()
        {
            StringBuilder builder = new StringBuilder(128);

            GetPrivateProfileString("Options", "NeedSavePhotographTime", "true", builder, 128, this._configFile);
            this._needSavePhotographTime = bool.Parse(builder.ToString());

            GetPrivateProfileString("Options", "NeedSavePhotographResult", "true", builder, 128, this._configFile);
            this._needSavePhotographResult = bool.Parse(builder.ToString());

            GetPrivateProfileString("Options", "NeedSaveOriginalImagePath", "true", builder, 128, this._configFile);
            this._needSaveOriginalImagePath = bool.Parse(builder.ToString());

            GetPrivateProfileString("Options", "NeedSaveResultImagePath", "true", builder, 128, this._configFile);
            this._needSaveResultImagePath = bool.Parse(builder.ToString());

            GetPrivateProfileString("Options", "NeedSaveStatus", "true", builder, 128, this._configFile);
            this._needSaveStatus = bool.Parse(builder.ToString());

            GetPrivateProfileString("Options", "NeedSaveVoltage", "true", builder, 128, this._configFile);
            this._needSaveVoltage = bool.Parse(builder.ToString());

            GetPrivateProfileString("Options", "NeedSaveElectricCurrent", "true", builder, 128, this._configFile);
            this._needSaveElectricCurrent = bool.Parse(builder.ToString());

            GetPrivateProfileString("Options", "NeedSaveUsageTime", "true", builder, 128, this._configFile);
            this._needSaveUsageTime = bool.Parse(builder.ToString());

            GetPrivateProfileString("Options", "NeedSaveDetectionLayer", "true", builder, 128, this._configFile);
            this._needSaveDetectionLayer = bool.Parse(builder.ToString());

            GetPrivateProfileString("Options", "NeedSaveMarkingResult", "true", builder, 128, this._configFile);
            this._needSaveMarkingResult = bool.Parse(builder.ToString());

            GetPrivateProfileString("Options", "NeedSaveMin", "true", builder, 128, this._configFile);
            this._needSaveMin = bool.Parse(builder.ToString());

            GetPrivateProfileString("Options", "NeedSaveMax", "true", builder, 128, this._configFile);
            this._needSaveMax = bool.Parse(builder.ToString());

            GetPrivateProfileString("Options", "NeedSaveDistance", "true", builder, 128, this._configFile);
            this._needSaveDistance = bool.Parse(builder.ToString());

            GetPrivateProfileString("Options", "NeedSaveAngle", "true", builder, 128, this._configFile);
            this._needSaveAngle = bool.Parse(builder.ToString());

            GetPrivateProfileString("Options", "NeedSaveLayerNum", "false", builder, 128, this._configFile);
            this._needSaveLayerNum = bool.Parse(builder.ToString());


            GetPrivateProfileString("Options", "NeedSaveRecordID", "true", builder, 128, this._configFile);
            this._needSaveRecordID = bool.Parse(builder.ToString());

            GetPrivateProfileString("Options", "NeedSaveBatteryBarCode", "true", builder, 128, this._configFile);
            this._needSaveBatteryBarCode = bool.Parse(builder.ToString());

            GetPrivateProfileString("Options", "NeedSaveCreateTime", "true", builder, 128, this._configFile);
            this._needSaveCreateTime = bool.Parse(builder.ToString());

            GetPrivateProfileString("Options", "NeedSaveUserID", "true", builder, 128, this._configFile);
            this._needSaveUserID = bool.Parse(builder.ToString());

            GetPrivateProfileString("Options", "NeedSaveDetectionMode", "true", builder, 128, this._configFile);
            this._needSaveDetectionMode = bool.Parse(builder.ToString());

            GetPrivateProfileString("Options", "NeedSaveDetectExtMode", "true", builder, 128, this._configFile);
            this._needSaveDetectExtMode = bool.Parse(builder.ToString());

            GetPrivateProfileString("Options", "NeedSaveFinalResult", "true", builder, 128, this._configFile);
            this._needSaveFinalResult = bool.Parse(builder.ToString());

            GetPrivateProfileString("Options", "NeedSaveResultCode", "true", builder, 128, this._configFile);
            this._needSaveResultCode = bool.Parse(builder.ToString());

            GetPrivateProfileString("Options", "NeedSaveResultPath", "true", builder, 128, this._configFile);
            this._needSaveResultPath = bool.Parse(builder.ToString());

            GetPrivateProfileString("Options", "NeedSaveStartTime", "true", builder, 128, this._configFile);
            this._needSaveStartTime = bool.Parse(builder.ToString());

            GetPrivateProfileString("Options", "NeedSaveEndTime", "true", builder, 128, this._configFile);
            this._needSaveEndTime = bool.Parse(builder.ToString());


            GetPrivateProfileString("Options", "NeedSaveRecheckState" , "true", builder, 128, this._configFile);
            this.NeedSaveRecheckState = bool.Parse(builder.ToString());

            GetPrivateProfileString("Options", "NeedSaveRecheckTime", "true", builder, 128, this._configFile);
            this._needSaveRecheckTime = bool.Parse(builder.ToString());

            GetPrivateProfileString("Options", "NeedSaveRecheckUserID", "true", builder, 128, this._configFile);
            this._needSaveRecheckUserID = bool.Parse(builder.ToString());

            GetPrivateProfileString("Options", "NeedSaveFAQTime", "true", builder, 128, this._configFile);
            this._needSaveFAQTime = bool.Parse(builder.ToString());

            GetPrivateProfileString("Options", "NeedSaveFQAUser", "true", builder, 128, this._configFile);
            this._needSaveFQAUser = bool.Parse(builder.ToString());
        }

        public void Write()
        {
            WritePrivateProfileString("Options", "NeedSavePhotographTime", this._needSavePhotographTime.ToString(), this._configFile);
            WritePrivateProfileString("Options", "NeedSavePhotographResult", this._needSavePhotographResult.ToString(), this._configFile);
            WritePrivateProfileString("Options", "NeedSaveOriginalImagePath", this._needSaveOriginalImagePath.ToString(), this._configFile);
            WritePrivateProfileString("Options", "NeedSaveResultImagePath", this._needSaveResultImagePath.ToString(), this._configFile);
            WritePrivateProfileString("Options", "NeedSaveStatus", this._needSaveStatus.ToString(), this._configFile);
            WritePrivateProfileString("Options", "NeedSaveVoltage", this._needSaveVoltage.ToString(), this._configFile);
            WritePrivateProfileString("Options", "NeedSaveElectricCurrent", this._needSaveElectricCurrent.ToString(), this._configFile);
            WritePrivateProfileString("Options", "NeedSaveUsageTime", this._needSaveUsageTime.ToString(), this._configFile);
            WritePrivateProfileString("Options", "NeedSaveDetectionLayer", this._needSaveDetectionLayer.ToString(), this._configFile);
            WritePrivateProfileString("Options", "NeedSaveMarkingResult", this._needSaveMarkingResult.ToString(), this._configFile);
            WritePrivateProfileString("Options", "NeedSaveMin", this._needSaveMin.ToString(), this._configFile);
            WritePrivateProfileString("Options", "NeedSaveMax", this._needSaveMax.ToString(), this._configFile);
            WritePrivateProfileString("Options", "NeedSaveDistance", this._needSaveDistance.ToString(), this._configFile);
            WritePrivateProfileString("Options", "NeedSaveAngle", this._needSaveAngle.ToString(), this._configFile);
            WritePrivateProfileString("Options", "NeedSaveLayerNum", this._needSaveLayerNum.ToString(), this._configFile);

            WritePrivateProfileString("Options","NeedSaveRecordID", _needSaveRecordID.ToString(),this._configFile);
            WritePrivateProfileString("Options","NeedSaveBatteryBarCode" ,_needSaveBatteryBarCode.ToString(),this._configFile);
            WritePrivateProfileString("Options","NeedSaveCreateTime",_needSaveCreateTime.ToString(),this._configFile);
            WritePrivateProfileString("Options","NeedSaveUserID",_needSaveUserID.ToString(),this._configFile);
            WritePrivateProfileString("Options","NeedSaveDetectionMode",_needSaveDetectionMode.ToString(),this._configFile);
            WritePrivateProfileString("Options","NeedSaveDetectExtMode",_needSaveDetectExtMode.ToString(),this._configFile);
            WritePrivateProfileString("Options","NeedSaveFinalResult",_needSaveFinalResult.ToString(),this._configFile);
            WritePrivateProfileString("Options","NeedSaveResultCode",_needSaveResultCode.ToString(),this._configFile);
            WritePrivateProfileString("Options","NeedSaveResultPath",_needSaveResultPath.ToString(),this._configFile);
            WritePrivateProfileString("Options","NeedSaveStartTime",_needSaveStartTime.ToString(),this._configFile);
            WritePrivateProfileString("Options","NeedSaveEndTime",_needSaveEndTime.ToString(),this._configFile);

            WritePrivateProfileString("Options","NeedSaveRecheckState", _needSaveRecheckState.ToString(),this._configFile);
            WritePrivateProfileString("Options","NeedSaveRecheckTime",_needSaveRecheckTime.ToString(),this._configFile);
            WritePrivateProfileString("Options","NeedSaveRecheckUserID",_needSaveRecheckUserID.ToString(),this._configFile);
            WritePrivateProfileString("Options","NeedSaveFAQTime",_needSaveFAQTime.ToString(),this._configFile);
            WritePrivateProfileString("Options","NeedSaveFQAUser", _needSaveFQAUser.ToString(), this._configFile);
        }

        #region field

        private bool _needSaveRecordID = true;
        private bool _needSaveBatteryBarCode = true;
        private bool _needSaveCreateTime = true;
        private bool _needSaveUserID = true;
        private bool _needSaveDetectionMode = true;
        private bool _needSaveDetectExtMode = true;
        private bool _needSaveFinalResult = true;
        private bool _needSaveResultCode = true;
        private bool _needSaveResultPath = true;
        private bool _needSaveStartTime = true;
        private bool _needSaveEndTime = true;

        private bool _needSaveRecheckState = true;
        private bool _needSaveRecheckTime = true;
        private bool _needSaveRecheckUserID = true;
        private bool _needSaveFAQTime = true;
        private bool _needSaveFQAUser = true;

        private bool _needSavePhotographTime = true;
        private bool _needSavePhotographResult = true;
        private bool _needSaveOriginalImagePath = true;
        private bool _needSaveResultImagePath = true;
        private bool _needSaveStatus = true;
        private bool _needSaveVoltage = true;
        private bool _needSaveElectricCurrent = true;
        private bool _needSaveUsageTime = true;
        private bool _needSaveDetectionLayer = true;
        private bool _needSaveMarkingResult = true;
        private bool _needSaveMin = true;
        private bool _needSaveMax = true;
        private bool _needSaveDistance = true;
        private bool _needSaveAngle = true;
        private bool _needSaveLayerNum = true;
        #endregion

        #region Property
        public bool NeedSavePhotographTime
        {
            get { return _needSavePhotographTime; }
            set
            {
                _needSavePhotographTime = value;
                RaisePropertyChanged("NeedSavePhotographTime");
            }
        }

        public bool NeedSavePhotographResult
        {
            get { return _needSavePhotographResult; }
            set
            {
                _needSavePhotographResult = value;
                RaisePropertyChanged("NeedSavePhotographResult");
            }
        }

        public bool NeedSaveOriginalImagePath
        {
            get { return _needSaveOriginalImagePath; }
            set
            {
                _needSaveOriginalImagePath = value;
                RaisePropertyChanged("NeedSaveOriginalImagePath");
            }
        }

        public bool NeedSaveResultImagePath
        {
            get { return _needSaveResultImagePath; }
            set
            {
                _needSaveResultImagePath = value;
                RaisePropertyChanged("NeedSaveResultImagePath");
            }
        }

        public bool NeedSaveStatus
        {
            get { return _needSaveStatus; }
            set
            {
                _needSaveStatus = value;
                RaisePropertyChanged("NeedSaveStatus");
            }
        }

        public bool NeedSaveVoltage
        {
            get { return _needSaveVoltage; }
            set
            {
                _needSaveVoltage = value;
                RaisePropertyChanged("NeedSaveVoltage");
            }
        }

        public bool NeedSaveElectricCurrent
        {
            get { return _needSaveElectricCurrent; }
            set
            {
                _needSaveElectricCurrent = value;
                RaisePropertyChanged("NeedSaveElectricCurrent");
            }
        }

        public bool NeedSaveUsageTime
        {
            get { return _needSaveUsageTime; }
            set
            {
                _needSaveUsageTime = value;
                RaisePropertyChanged("NeedSaveUsageTime");
            }
        }

        public bool NeedSaveDetectionLayer
        {
            get { return _needSaveDetectionLayer; }
            set
            {
                _needSaveDetectionLayer = value;
                RaisePropertyChanged("NeedSaveDetectionLayer");
            }
        }

        public bool NeedSaveMarkingResult
        {
            get { return _needSaveMarkingResult; }
            set
            {
                _needSaveMarkingResult = value;
                RaisePropertyChanged("NeedSaveMarkingResult");
            }
        }

        public bool NeedSaveMin
        {
            get { return _needSaveMin; }
            set
            {
                _needSaveMin = value;
                RaisePropertyChanged("NeedSaveMin");
            }
        }

        public bool NeedSaveMax
        {
            get { return _needSaveMax; }
            set
            {
                _needSaveMax = value;
                RaisePropertyChanged("NeedSaveMax");
            }
        }

        public bool NeedSaveDistance
        {
            get { return _needSaveDistance; }
            set
            {
                _needSaveDistance = value;
                RaisePropertyChanged("NeedSaveDistance");
            }
        }

        public bool NeedSaveAngle
        {
            get { return _needSaveAngle; }
            set
            {
                _needSaveAngle = value;
                RaisePropertyChanged("NeedSaveAngle");
            }
        }

        public bool NeedSaveLayerNum
        {
            get { return _needSaveLayerNum; }
            private set
            {
                _needSaveLayerNum = value;
                RaisePropertyChanged("NeedSaveLayerNum");
            }
        }

        public bool NeedSaveRecordID
        {
            get { return _needSaveRecordID; }
            set
            {
                _needSaveRecordID = value;
                RaisePropertyChanged("NeedSaveRecordID");
            }
        }

        public bool NeedSaveBatteryBarCode
        {
            get { return _needSaveBatteryBarCode; }
            set
            {
                _needSaveBatteryBarCode = value;
                RaisePropertyChanged("NeedSaveBatteryBarCode");
            }
        }

        public bool NeedSaveCreateTime
        {
            get { return _needSaveCreateTime; }
            set
            {
                _needSaveCreateTime = value;
                RaisePropertyChanged("NeedSaveCreateTime");
            }
        }

        public bool NeedSaveUserID
        {
            get { return _needSaveUserID; }
            set
            {
                _needSaveUserID = value;
                RaisePropertyChanged("NeedSaveUserID");
            }
        }

        public bool NeedSaveDetectionMode
        {
            get { return _needSaveDetectionMode; }
            set
            {
                _needSaveDetectionMode = value;
                RaisePropertyChanged("NeedSaveDetectionMode");
            }
        }

        public bool NeedSaveDetectExtMode
        {
            get { return _needSaveDetectExtMode; }
            set
            {
                _needSaveDetectExtMode = value;
                RaisePropertyChanged("NeedSaveDetectExtMode");
            }
        }

        public bool NeedSaveFinalResult
        {
            get { return _needSaveFinalResult; }
            set
            {
                _needSaveFinalResult = value;
                RaisePropertyChanged("NeedSaveFinalResult");
            }
        }

        public bool NeedSaveResultCode
        {
            get { return _needSaveResultCode; }
            set
            {
                _needSaveResultCode = value;
                RaisePropertyChanged("NeedSaveResultCode");
            }
        }

        public bool NeedSaveResultPath
        {
            get { return _needSaveResultPath; }
            set
            {
                _needSaveResultPath = value;
                RaisePropertyChanged("NeedSaveResultPath");
            }
        }

        public bool NeedSaveStartTime
        {
            get { return _needSaveStartTime; }
            set
            {
                _needSaveStartTime = value;
                RaisePropertyChanged("NeedSaveStartTime");
            }
        }

        public bool NeedSaveEndTime
        {
            get { return _needSaveEndTime; }
            set
            {
                _needSaveEndTime = value;
                RaisePropertyChanged("NeedSaveEndTime");
            }
        }

        public bool NeedSaveRecheckState
        {
            get { return _needSaveRecheckState; }
            set
            {
                _needSaveRecheckState = value;
                RaisePropertyChanged("NeedSaveRecheckState");
            }
        }

        public bool NeedSaveRecheckTime
        {
            get { return _needSaveRecheckTime; }
            set
            {
                _needSaveRecheckTime = value;
                RaisePropertyChanged("NeedSaveRecheckTime");
            }
        }

        public bool NeedSaveRecheckUserID
        {
            get { return _needSaveRecheckUserID; }
            set
            {
                _needSaveRecheckUserID = value;
                RaisePropertyChanged("NeedSaveRecheckUserID");
            }
        }

        public bool NeedSaveFAQTime
        {
            get { return _needSaveFAQTime; }
            set
            {
                _needSaveFAQTime = value;
                RaisePropertyChanged("NeedSaveFAQTime");
            }
        }

        public bool NeedSaveFQAUser
        {
            get { return _needSaveFQAUser; }
            set
            {
                _needSaveFQAUser = value;
                RaisePropertyChanged("NeedSaveFQAUser");
            }
        }

        #endregion
    }
}
