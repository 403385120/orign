using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using XRayClient.VisionSysWrapper;
using Shuyz.Framework.Mvvm;
using System.IO;
using System.Threading;
using ZY.Logging;
using ATL.Core;
using ATL.Station;

namespace XRayClient.Core
{
    /// <summary>
    /// 电池工位信息
    /// </summary>
    public class BatterySeat : ObservableObject
    {
        private ECheckModes _checkMode = ECheckModes.FourSides; // 检测方式
        private ECheckExtensions _checkExtension = ECheckExtensions.Test;

        private string _lengthBarcode = "";                 //30位的长条码
        private string _sn = string.Empty;                  // 电池序列号
        private DateTime _snScanTime = DateTime.MinValue;   // 扫码时间
        private string _endTime = DateTime.MinValue.ToLongTimeString();    // 结束时间(发送结果时间)
        private int _tempBarcode;
        private string _cellType = "";
        private bool _isMarkingCell = false;//是否是marking电池，标记为marking的电池最后当NG扔掉
        private string _marking = "";
        private string _masterType = "";//Master点检电池类型

        private BatteryCorner _corner1 = new BatteryCorner();
        private BatteryCorner _corner2 = new BatteryCorner();
        private BatteryCorner _corner3 = new BatteryCorner();
        private BatteryCorner _corner4 = new BatteryCorner();

        private bool _stfResult = false;      // 当STF启用时从STF获取的结果
        public bool _isGrrMode = false;

        private bool _finalResult = false;
        private bool _thicknessResult = false;
        private bool _backThicknessResult = false;
        private bool _iVResult = false;
        private bool _knifeNoConduction = true;
        private bool _needleNoConduction = true;
        private bool _dataFinish = false;//数据处理结果

        private bool _oCVResult = false;
        private bool _dimensionResult = false;
        private bool _temperatureResult = false;
        private bool _resistanceResult = false;
        private bool _voltageResult = false;
        private bool _kResult = false;
        private bool _mesResult = true;
        private bool _mesOCVResult = true;
        private bool _mesMDIResult = true;
        private bool _mesXRAYResult = true;

        private EResultCodes _resultCode = EResultCodes.Unknow;    // 检测结果不合格代码
        public ZYImageStruct ResultImage; // = new ZYImageStruct(); // 仅构造一次     合成图
        private string _resultImgFileName = string.Empty;

        private int _scanCodeCount = 0;
        private int _captureACount = 0;
        private int _captureBCount = 0;
        private int _captureCCount = 0;
        private int _captureDCount = 0;
        private int _algoACount = 0;
        private int _algoBCount = 0;
        private int _algoCCount = 0;
        private int _algoDCount = 0;
        private int _thicknessCount = 0;
        private int _dimensionCount = 0;
        private int _ocvCount = 0;
        private int _catchNGCount = 0;

        private float _thickness = 0;
        private float _thicknessBack = 0;

        private float _batLength = 0;//电池主体长度
        private float _batWidth = 0;//电池主体宽度
        private float _leftLugMargin = 0;//左极耳边距
        private float _rightLugMargin = 0;//右极耳边距
        private float _leftLugLength = 0;//左极耳长度
        private float _rightLugLength = 0;//右极耳长度
        private float _allBatLength = 0;//电池总长（包含极耳）
        private float _left1WhiteGlue = 0;//左1小白胶
        private float _left2WhiteGlue = 0;//左2小白胶
        private float _right1WhiteGlue = 0;//右1小白胶
        private float _right2WhiteGlue = 0;//右2小白胶
        private float _resistance = 0;//电阻
        private float _voltage = 0;//电压
        private float _kValue = 0;//K值
        private float _temperature = 0;//温度
        private float _envirementTemperature = 0;//环境温度
        private string _ocvRemark = "";//OCV备注
        private string _iV = "";//边电压原始以数据
        private float _ivData = 0;//边电压值
        private string _iVPLCConduction = "";//PLC判定的IV导通情况
        private string _iVRemark = "";//IV备注
        private int _cannel = 0;//通道号
        private int _ivChannel = 0;//IV工位
        private int _ocvChannel = 0;//OCV工位
        private int _mdiChannel = 0;//MDI工位
        private int _ppgChannel = 0;//PPG工位
        private string _ngItem = "";//NG项

        private string _origImgSavePath = string.Empty;    // 原始图片保存地址
        private string _resultImgSavePath = string.Empty;  // 结果图片保存地址
        private string _outlinePath = string.Empty;        // 单机图片保存地址
        private string _stfImgSavePath = string.Empty;     // STF图片保存路径


        public void Reset()
        {
            // CheckMode    // No need for resetting

            this.Sn = string.Empty;
            this.SnScanTime = DateTime.MinValue;
            this.EndTime = DateTime.MinValue.ToString("yyyy-MM-dd HH:mm:ss");


            this.ScanCodeCount = 0;
            this.CaptureACount = 0;
            this.CaptureBCount = 0;
            this.CaptureCCount = 0;
            this.CaptureDCount = 0;
            this.ThicknessCount = 0;
            this.DimensionCount = 0;
            this.OCVCount = 0;
            this.CatchNGCount = 0;

            this.Thickness = 0;
            this.BatLength = 0;
            this.BatWidth = 0;
            this.LeftLugMargin = 0;
            this.RightLugMargin = 0;
            this.AllBatLength = 0;
            this.Left1WhiteGlue = 0;
            this.Left2WhiteGlue = 0;
            this.Right1WhiteGlue = 0;
            this.Right2WhiteGlue = 0;

            this.Resistance = 0;
            this.Voltage = 0;
            this.K_Value = 0;
            this.Temperature = 0;
            EnvirementTemperature = 0;

            this.Corner1.Reset();
            this.Corner2.Reset();
            this.Corner3.Reset();
            this.Corner4.Reset();

            // InspectParams // No need for resetting
            this.FinalResult = false;
            this.ThicknessResult = false;
            this.DimensionResult = false;
            this.ResistanceResult = false;
            this.VoltageResult = false;
            this.TemperatureResult = false;
            this.KValueResult = false;
            this.MesResult = false;
            this.StfResult = false;
            this.IsGrrMode = false;
            this.ResultCode = EResultCodes.Unknow;

            this.ResultImgFileName = string.Empty;

            this.DataFinish = false;
        }
        public BatterySeat()
        {
            this.ResultImage.Create(ImageDefinations.MaxImgWidth * 2, ImageDefinations.MaxImgHeight * 2, 3);
            this.ResultImage.width = ImageDefinations.ActualWidth * 2;
            this.ResultImage.height = ImageDefinations.ActualHeight * 2;
            this.OutLinePath = "-OutLine";
        }

        ~BatterySeat()
        {
            this.Destroy();
        }

        public void Destroy()
        {
            if (this.Corner1 != null)
            {
                this.Corner1.Destroy();
            }
            if (this.Corner2 != null)
            {
                this.Corner2.Destroy();
            }
            if (this.Corner3 != null)
            {
                this.Corner3.Destroy();
            }
            if (this.Corner4 != null)
            {
                this.Corner4.Destroy();
            }

            this.ResultImage.Destory();
        }

        public void CopyTo(ref BatterySeat s)
        {
            s.CheckMode = this.CheckMode;
            s.CheckExtension = this.CheckExtension;

            s.Sn = this.Sn;
            s.SnScanTime = this.SnScanTime;
            s.EndTime = this.EndTime;
            s.TempBarcode = this.TempBarcode;
            this.Corner1.CopyTo(ref s._corner1);
            this.Corner2.CopyTo(ref s._corner2);
            this.Corner3.CopyTo(ref s._corner3);
            this.Corner4.CopyTo(ref s._corner4);

            s.FinalResult = this.FinalResult;
            s.ThicknessResult = this.ThicknessResult;
            s.MesResult = this.MesResult;
            s.StfResult = this.StfResult;
            s.IsGrrMode = this.IsGrrMode;
            s.ResultCode = this.ResultCode;
            this.ResultImage.CopyTo(ref s.ResultImage);

            s.OrigImgSavePath = this.OrigImgSavePath;
            s.ResultImgSavePath = this.ResultImgSavePath;
            s.StfImgSavePath = this.StfImgSavePath;
            s.OutLinePath = this.OutLinePath;

            s.Sn = this.Sn;
        }

        public ECheckModes CheckMode
        {
            get { return this._checkMode; }
            set
            {
                if (value == this._checkMode) return;

                this._checkMode = value;
                RaisePropertyChanged("CheckMode");
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

        public string Sn
        {
            get { return this._sn; }
            set
            {
                if (value == this._sn) return;

                this._sn = value;
                RaisePropertyChanged("Sn");
            }
        }

        public string CellType
        {
            get
            {
                return this._cellType;
            }
            set
            {
                this._cellType = value;
                RaisePropertyChanged("CellType");
            }
        }

        public bool IsMarkingCell
        {
            get
            {
                return this._isMarkingCell;
            }
            set
            {
                this._isMarkingCell = value;
                RaisePropertyChanged("IsMarkingCell");
            }
        }

        public string Marking
        {
            get { return this._marking; }
            set
            {
                this._marking = value;
                RaisePropertyChanged("Marking");
            }
        }

        public string MasterType
        {
            get { return this._masterType; }
            set
            {
                this._masterType = value;
                RaisePropertyChanged("MasterType");
            }
        }

        public string LengthBarcode
        {
            get { return this._lengthBarcode; }
            set
            {
                if (value == this._lengthBarcode) return;

                this._lengthBarcode = value;
                RaisePropertyChanged("LengthBarcode");
            }
        }

        public DateTime SnScanTime
        {
            get { return this._snScanTime; }
            set
            {
                if (value == this._snScanTime) return;

                this._snScanTime = value;
                RaisePropertyChanged("SnScanTime");
            }
        }

        public int TempBarcode
        {
            get { return this._tempBarcode; }
            set
            {
                if (value == this._tempBarcode) return;

                this._tempBarcode = value;
                RaisePropertyChanged("TempBarcode");
            }
        }

        public string EndTime
        {
            get { return this._endTime; }
            set
            {
                if (value == this._endTime) return;

                this._endTime = value;
                RaisePropertyChanged("EndTime");
            }
        }

        public BatteryCorner Corner1
        {
            get { return this._corner1; }
            set
            {
                if (value == this._corner1) return;

                this._corner1 = value;
                RaisePropertyChanged("Corner1");
            }
        }

        public BatteryCorner Corner2
        {
            get { return this._corner2; }
            set
            {
                if (value == this._corner2) return;

                this._corner2 = value;
                RaisePropertyChanged("Corner2");
            }
        }

        public BatteryCorner Corner3
        {
            get { return this._corner3; }
            set
            {
                if (value == this._corner3) return;

                this._corner3 = value;
                RaisePropertyChanged("Corner3");
            }
        }

        public BatteryCorner Corner4
        {
            get { return this._corner4; }
            set
            {
                if (value == this._corner4) return;

                this._corner4 = value;
                RaisePropertyChanged("Corner4");
            }
        }

        public bool StfResult
        {
            get
            {
                return this._stfResult;
            }
            set
            {
                this._stfResult = value;
                RaisePropertyChanged("StfResult");
            }
        }
        public bool IsGrrMode
        {
            get
            {
                return this._isGrrMode;
            }
            set
            {
                this._isGrrMode = value;
                RaisePropertyChanged("IsGrrMode");
            }
        }

        public bool FinalResult
        {
            get
            {
                return this._finalResult;
            }
            set
            {
                if (value == this._finalResult) return;

                this._finalResult = value;
                RaisePropertyChanged("FinalResult");
            }
        }

        public bool ThicknessResult
        {
            get
            {
                return this._thicknessResult;
            }
            set
            {
                if (value == this._thicknessResult) return;
                this._thicknessResult = value;
                RaisePropertyChanged("ThicknessResult");
            }
        }

        public bool BackThicknessResult
        {
            get
            {
                return this._backThicknessResult;
            }
            set
            {
                if (value == this._backThicknessResult) return;
                this._backThicknessResult = value;
                RaisePropertyChanged("BackThicknessResult");
            }
        }

        public bool DimensionResult
        {
            get
            {
                return this._dimensionResult;
            }
            set
            {
                if (value == this._dimensionResult) return;

                this._dimensionResult = value;
                RaisePropertyChanged("DimensionResult");
            }
        }

        public bool OCVResult
        {
            get
            {
                return this._oCVResult;
            }
            set
            {
                if (value == this._oCVResult) return;

                this._oCVResult = value;
                RaisePropertyChanged("OCVResult");
            }
        }

        public bool IVResult
        {
            get
            {
                return this._iVResult;
            }
            set
            {
                if (value == this._iVResult) return;

                this._iVResult = value;
                RaisePropertyChanged("IVResult");
            }
        }

        public bool KnifeNoConduction
        {
            get
            {
                return this._knifeNoConduction;
            }
            set
            {
                if (value == this._knifeNoConduction) return;

                this._knifeNoConduction = value;
                RaisePropertyChanged("KnifeNoConduction");
            }
        }

        public bool NeedleNoConduction
        {
            get
            {
                return this._needleNoConduction;
            }
            set
            {
                if (value == this._needleNoConduction) return;

                this._needleNoConduction = value;
                RaisePropertyChanged("NeedleNoConduction");
            }
        }

        public bool DataFinish
        {
            get
            {
                return this._dataFinish;
            }
            set
            {
                if (value == this._dataFinish) return;

                this._dataFinish = value;
                RaisePropertyChanged("DataFinish");
            }
        }

        public bool TemperatureResult
        {
            get
            {
                return this._temperatureResult;
            }
            set
            {
                if (value == this._temperatureResult) return;

                this._temperatureResult = value;
                RaisePropertyChanged("TemperatureResult");
            }
        }

        public bool ResistanceResult
        {
            get
            {
                return this._resistanceResult;
            }
            set
            {
                if (value == this._resistanceResult) return;

                this._resistanceResult = value;
                RaisePropertyChanged("ResistanceResult");
            }
        }

        public bool VoltageResult
        {
            get
            {
                return this._voltageResult;
            }
            set
            {
                if (value == this._voltageResult) return;

                this._voltageResult = value;
                RaisePropertyChanged("VoltageResult");
            }
        }

        public bool KValueResult
        {
            get
            {
                return this._kResult;
            }
            set
            {
                if (value == this._kResult) return;

                this._kResult = value;
                RaisePropertyChanged("KValueResult");
            }
        }

        public bool MesResult
        {
            get
            {
                return this._mesResult;
            }
            set
            {
                if (value == this._mesResult) return;

                this._mesResult = value;
                RaisePropertyChanged("MesResult");
            }
        }

        public bool MesOCVResult
        {
            get
            {
                return this._mesOCVResult;
            }
            set
            {
                if (value == this._mesOCVResult) return;

                this._mesOCVResult = value;
                RaisePropertyChanged("MesOCVResult");
            }
        }

        public bool MesMDIResult
        {
            get
            {
                return this._mesMDIResult;
            }
            set
            {
                if (value == this._mesMDIResult) return;

                this._mesMDIResult = value;
                RaisePropertyChanged("MesMDIResult");
            }
        }

        public bool MesXRAYResult
        {
            get
            {
                return this._mesXRAYResult;
            }
            set
            {
                if (value == this._mesXRAYResult) return;

                this._mesXRAYResult = value;
                RaisePropertyChanged("MesXRAYResult");
            }
        }

        public EResultCodes ResultCode
        {
            get { return this._resultCode; }
            set
            {
                this._resultCode = value;
                RaisePropertyChanged("ResultCode");
            }
        }

        public string ResultImgFileName
        {
            get { return this._resultImgFileName; }
            set
            {
                this._resultImgFileName = value;
                RaisePropertyChanged("ResultImgFileName");
            }
        }

        public string OrigImgSavePath
        {
            get
            {
                if (string.Empty == this._origImgSavePath) return string.Empty;
                return this._origImgSavePath;
            }
            set
            {
                this._origImgSavePath = value;
                RaisePropertyChanged("OrigImgSavePath");
            }
        }

        public string ResultImgSavePath
        {
            get
            {
                if (string.Empty == this._resultImgSavePath) return string.Empty;
                return this._resultImgSavePath;
            }
            set
            {
                this._resultImgSavePath = value;
                RaisePropertyChanged("ResultImgSavePath");
            }
        }

        public string StfImgSavePath
        {
            get
            {
                return this._stfImgSavePath;
            }
            set
            {
                this._stfImgSavePath = value;
                RaisePropertyChanged("StfImgSavePath");
            }
        }

        public string OutLinePath
        {
            get
            {
                return this._outlinePath;
            }
            set
            {
                this._outlinePath = value;
                RaisePropertyChanged("OutLinePath");
            }
        }

        public int ScanCodeCount
        {
            get
            {
                return this._scanCodeCount;
            }
            set
            {
                this._scanCodeCount = value;
                RaisePropertyChanged("ScanCodeCount");
            }
        }

        public int CaptureACount
        {
            get
            {
                return this._captureACount;
            }
            set
            {
                this._captureACount = value;
                RaisePropertyChanged("CaptureACount");
            }
        }

        public int CaptureBCount
        {
            get
            {
                return this._captureBCount;
            }
            set
            {
                this._captureBCount = value;
                RaisePropertyChanged("CaptureBCount");
            }
        }

        public int CaptureCCount
        {
            get
            {
                return this._captureCCount;
            }
            set
            {
                this._captureCCount = value;
                RaisePropertyChanged("CaptureCCount");
            }
        }

        public int CaptureDCount
        {
            get
            {
                return this._captureDCount;
            }
            set
            {
                this._captureDCount = value;
                RaisePropertyChanged("CaptureDCount");
            }
        }

        public int AlgoACount
        {
            get
            {
                return this._algoACount;
            }
            set
            {
                this._algoACount = value;
                RaisePropertyChanged("AlgoACount");
            }
        }

        public int AlgoBCount
        {
            get
            {
                return this._algoBCount;
            }
            set
            {
                this._algoBCount = value;
                RaisePropertyChanged("AlgoBCount");
            }
        }

        public int AlgoCCount
        {
            get
            {
                return this._algoCCount;
            }
            set
            {
                this._algoCCount = value;
                RaisePropertyChanged("AlgoCCount");
            }
        }

        public int AlgoDCount
        {
            get
            {
                return this._algoDCount;
            }
            set
            {
                this._algoDCount = value;
                RaisePropertyChanged("AlgoDCount");
            }
        }

        public int ThicknessCount
        {
            get
            {
                return this._thicknessCount;
            }
            set
            {
                this._thicknessCount = value;
                RaisePropertyChanged("ThicknessCount");
            }
        }

        public int DimensionCount
        {
            get
            {
                return this._dimensionCount;
            }
            set
            {
                this._dimensionCount = value;
                RaisePropertyChanged("DimensionCount");
            }
        }

        public int OCVCount
        {
            get
            {
                return this._ocvCount;
            }
            set
            {
                this._ocvCount = value;
                RaisePropertyChanged("OCVCount");
            }
        }

        public int CatchNGCount
        {
            get
            {
                return this._catchNGCount;
            }
            set
            {
                this._catchNGCount = value;
                RaisePropertyChanged("CatchNGCount");
            }
        }

        public float Thickness
        {
            get
            {
                return this._thickness;
            }
            set
            {
                this._thickness = value;
                RaisePropertyChanged("Thickness");
            }
        }
        public float ThicknessBack
        {
            get
            {
                return this._thicknessBack;
            }
            set
            {
                this._thicknessBack = value;
                RaisePropertyChanged("ThicknessBack");
            }
        }

        public float BatLength
        {
            get
            {
                return this._batLength;
            }
            set
            {
                this._batLength = value;
                RaisePropertyChanged("BatLength");
            }
        }
        public float BatWidth
        {
            get
            {
                return this._batWidth;
            }
            set
            {
                this._batWidth = value;
                RaisePropertyChanged("BatWidth");
            }
        }

        public float LeftLugMargin
        {
            get
            {
                return this._leftLugMargin;
            }
            set
            {
                this._leftLugMargin = value;
                RaisePropertyChanged("LeftLugMargin");
            }
        }
        public float RightLugMargin
        {
            get
            {
                return this._rightLugMargin;
            }
            set
            {
                this._rightLugMargin = value;
                RaisePropertyChanged("RightLugMargin");
            }
        }

        public float LeftLugLength
        {
            get
            {
                return this._leftLugLength;
            }
            set
            {
                this._leftLugLength = value;
                RaisePropertyChanged("LeftLugLength");
            }
        }

        public float RightLugLength
        {
            get
            {
                return this._rightLugLength;
            }
            set
            {
                this._rightLugLength = value;
                RaisePropertyChanged("RightLugLength");
            }
        }

        public float AllBatLength
        {
            get
            {
                return this._allBatLength;
            }
            set
            {
                this._allBatLength = value;
                RaisePropertyChanged("AllBatLength");
            }
        }

        public float Left1WhiteGlue
        {
            get
            {
                return this._left1WhiteGlue;
            }
            set
            {
                this._left1WhiteGlue = value;
                RaisePropertyChanged("Left1WhiteGlue");
            }
        }

        public float Left2WhiteGlue
        {
            get
            {
                return this._left2WhiteGlue;
            }
            set
            {
                this._left2WhiteGlue = value;
                RaisePropertyChanged("Left2WhiteGlue");
            }
        }

        public float Right1WhiteGlue
        {
            get
            {
                return this._right1WhiteGlue;
            }
            set
            {
                this._right1WhiteGlue = value;
                RaisePropertyChanged("Right1WhiteGlue");
            }
        }

        public float Right2WhiteGlue
        {
            get
            {
                return this._right2WhiteGlue;
            }
            set
            {
                this._right2WhiteGlue = value;
                RaisePropertyChanged("Right2WhiteGlue");
            }
        }

        public float Resistance
        {
            get
            {
                return this._resistance;
            }
            set
            {
                this._resistance = value;
                RaisePropertyChanged("Resistance");
            }
        }

        public float Voltage
        {
            get
            {
                return this._voltage;
            }
            set
            {
                this._voltage = value;
                RaisePropertyChanged("Voltage");
            }
        }

        public float K_Value
        {
            get
            {
                return this._kValue;
            }
            set
            {
                this._kValue = value;
                RaisePropertyChanged("K_Value");
            }
        }

        public float Temperature
        {
            get
            {
                return this._temperature;
            }
            set
            {
                this._temperature = value;
                RaisePropertyChanged("Temperature");
            }
        }

        public float EnvirementTemperature
        {
            get
            {
                return this._envirementTemperature;
            }
            set
            {
                this._envirementTemperature = value;
                RaisePropertyChanged("EnvirementTemperature");
            }
        }

        public string OcvRemark
        {
            get
            {
                return this._ocvRemark;
            }
            set
            {
                this._ocvRemark = value;
                RaisePropertyChanged("OcvRemark");
            }
        }

        public string IV
        {
            get
            {
                return this._iV;
            }
            set
            {
                this._iV = value;
                RaisePropertyChanged("IV");
            }
        }

        public float IvData
        {
            get
            {
                return this._ivData;
            }
            set
            {
                this._ivData = value;
                RaisePropertyChanged("IvData");
            }
        }

        public string IVPLCConduction
        {
            get
            {
                return this._iVPLCConduction;
            }
            set
            {
                this._iVPLCConduction = value;
                RaisePropertyChanged("IVPLCConduction");
            }
        }

        public string IVRemark
        {
            get
            {
                return this._iVRemark;
            }
            set
            {
                this._iVRemark = value;
                RaisePropertyChanged("IVRemark");
            }
        }

        //public int Cannel
        //{
        //    get
        //    {
        //        return this._cannel;
        //    }
        //    set
        //    {
        //        this._cannel = value;
        //        RaisePropertyChanged("Cannel");
        //    }
        //}

        public int IVChannel
        {
            get { return this._ivChannel; }

            set
            {
                this._ivChannel = value;
                RaisePropertyChanged("IVChannel");
            }
        }

        public int OCVChannel
        {
            get { return this._ocvChannel; }

            set
            {
                this._ocvChannel = value;
                RaisePropertyChanged("OCVChannel");
            }
        }

        public int MDIChannel
        {
            get { return this._mdiChannel; }

            set
            {
                this._mdiChannel = value;
                RaisePropertyChanged("MDIChannel");
            }
        }

        public int PPGChannel
        {
            get { return this._ppgChannel; }

            set
            {
                this._ppgChannel = value;
                RaisePropertyChanged("PPGChannel");
            }
        }


        public string NgItem
        {
            get
            {
                return this._ngItem;
            }
            set
            {
                this._ngItem = value;
                RaisePropertyChanged("NgItem");
            }
        }

        /// <summary>
        /// 更新结果及不合格原因
        /// 注意这里没有检测STF结果
        /// </summary>
        /// <returns></returns>
        public bool UpdateResult()
        {
            if (this._checkMode == ECheckModes.FourSides)
            {
                this.FinalResult = (this._corner1.InspectResults.resultDataMin.iResult == 1
                    && this._corner2.InspectResults.resultDataMin.iResult == 1
                    && this._corner3.InspectResults.resultDataMin.iResult == 1
                    && this._corner4.InspectResults.resultDataMin.iResult == 1);

                if (this.FinalResult)
                {
                    this.ResultCode = EResultCodes.OK;
                    return true;
                }

                if (!this._corner1.IsShotOK || !this._corner2.IsShotOK
                    || !this._corner3.IsShotOK || !this._corner4.IsShotOK)
                {
                    this.ResultCode = EResultCodes.ShotFail;
                    return false;
                }

                // 黑白图
                if (this._corner1.InspectResults.resultDataMin.iResult == -1
                  || this._corner2.InspectResults.resultDataMin.iResult == -1
                  || this._corner3.InspectResults.resultDataMin.iResult == -1
                  || this._corner4.InspectResults.resultDataMin.iResult == -1)
                {
                    this.ResultCode = EResultCodes.AlgoErr;
                    return false;
                }

                // 位置错误
                if (this._corner1.InspectResults.resultDataMin.iResult == -2
                  || this._corner2.InspectResults.resultDataMin.iResult == -2
                  || this._corner3.InspectResults.resultDataMin.iResult == -2
                  || this._corner4.InspectResults.resultDataMin.iResult == -2)
                {
                    this.ResultCode = EResultCodes.AlgoErr;
                    return false;
                }

                if (this._corner1.InspectResults.resultDataMin.iResult != 1
                || this._corner2.InspectResults.resultDataMin.iResult != 1
                || this._corner3.InspectResults.resultDataMin.iResult != 1
                || this._corner4.InspectResults.resultDataMin.iResult != 1)
                {
                    this.ResultCode = EResultCodes.AlgoFail;
                    return false;
                }

                this.ResultCode = EResultCodes.Unknow;

                return true;
            }
            else if (this._checkMode == ECheckModes.Diagonal_1_2)       //recompose by fjy
            {
                this.FinalResult = (this._corner1.InspectResults.resultDataMin.iResult == 1
                    && this._corner3.InspectResults.resultDataMin.iResult == 1);

                if (this.FinalResult)
                {
                    this.ResultCode = EResultCodes.OK;
                    return true;
                }

                if (!this._corner1.IsShotOK || !this._corner3.IsShotOK)
                {
                    this.ResultCode = EResultCodes.ShotFail;
                    return false;
                }

                // 黑白图
                if (this._corner1.InspectResults.resultDataMin.iResult == -1
                  || this._corner3.InspectResults.resultDataMin.iResult == -1)
                {
                    this.ResultCode = EResultCodes.AlgoErr;
                    return false;
                }

                // 位置错误
                if (this._corner1.InspectResults.resultDataMin.iResult == -2
                  || this._corner3.InspectResults.resultDataMin.iResult == -2)
                {
                    this.ResultCode = EResultCodes.AlgoErr;
                    return false;
                }

                if (this._corner1.InspectResults.resultDataMin.iResult != 1
                || this._corner3.InspectResults.resultDataMin.iResult != 1)
                {
                    this.ResultCode = EResultCodes.AlgoFail;
                    return false;
                }

                this.ResultCode = EResultCodes.Unknow;

                return true;
            }
            else if (this._checkMode == ECheckModes.Diagonal_3_4)       //recompose by fjy
            {
                this.FinalResult = (this._corner3.InspectResults.resultDataMin.iResult == 1
                    && this._corner4.InspectResults.resultDataMin.iResult == 1);

                if (this.FinalResult)
                {
                    this.ResultCode = EResultCodes.OK;
                    return true;
                }

                if (!this._corner3.IsShotOK || !this._corner4.IsShotOK)
                {
                    this.ResultCode = EResultCodes.ShotFail;
                    return false;
                }

                // 黑白图
                if (this._corner3.InspectResults.resultDataMin.iResult == -1
                  || this._corner4.InspectResults.resultDataMin.iResult == -1)
                {
                    this.ResultCode = EResultCodes.AlgoErr;
                    return false;
                }

                if (this._corner3.InspectResults.resultDataMin.iResult != 1
                || this._corner4.InspectResults.resultDataMin.iResult != 1)
                {
                    this.ResultCode = EResultCodes.AlgoFail;
                    return false;
                }

                this.ResultCode = EResultCodes.Unknow;

                return true;
            }

            throw new NotImplementedException();
        }

        public void UpdateFileName(ImageSaveConfig imgconfig, bool useSTFPath, bool isStartup = false, bool isRunEmpty = false)
        {
            string filepath = DateTime.Now.ToString("yyyyMMdd") + "\\" + DateTime.Now.Hour.ToString();

            if (!isRunEmpty)
            {
                this._origImgSavePath = Path.Combine(imgconfig.SaveOrigPath, filepath);
                this._resultImgSavePath = Path.Combine(imgconfig.SaveTestPath, filepath);
            }
            else
            {
                string OrigImageOutLineSavePath = Path.Combine(imgconfig.SaveOrigPath, this.OutLinePath);
                string ResultImageOutLineSavePath = Path.Combine(imgconfig.SaveTestPath, this.OutLinePath);
                this._origImgSavePath = Path.Combine(OrigImageOutLineSavePath, filepath);
                this._resultImgSavePath = Path.Combine(ResultImageOutLineSavePath, filepath);
            }

            if (isStartup)
            {
                this._resultImgSavePath = Path.Combine(imgconfig.StartupTestPath, filepath);
            }

            if (this.FinalResult)
            {
                this._origImgSavePath = Path.Combine(this._origImgSavePath, "OK");
                this._resultImgSavePath = Path.Combine(this._resultImgSavePath, "OK");
            }
            else
            {
                this._origImgSavePath = Path.Combine(this._origImgSavePath, "NG");
                this._resultImgSavePath = Path.Combine(this._resultImgSavePath, "NG");
            }

            string baseFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff") + "_" + (this._sn == string.Empty ? "Empty" : this._sn);

            for (int i = 0; i < 4; i++)
            {
                BatteryCorner corner = null;
                if (i == 0) corner = this._corner1;
                else if (i == 1) corner = this._corner2;
                else if (i == 2) corner = this._corner3;
                else corner = this._corner4;

                if (this.FinalResult)
                {
                    corner.OrigImageFileName = "OK_" + baseFileName + "_" + (i + 1).ToString() + ".jpg";
                    corner.ResultImgFileName = "OK_" + baseFileName + "_" + (i + 1).ToString() + "_test.jpg";
                }
                else
                {
                    corner.OrigImageFileName = "NG_" + baseFileName + "_" + (i + 1).ToString() + ".jpg";
                    corner.ResultImgFileName = "NG_" + baseFileName + "_" + (i + 1).ToString() + "_test.jpg";
                }
            }

            this.ResultImgFileName = (this.FinalResult ? "OK_" : "NG_") + baseFileName + "_5_test.jpg";
        }

        public void SaveAll(ImageSaveConfig imgconfig, bool useSTFPath, bool isStartup = false)
        {
            int writeTimeout = 5000;

            //根据日期时间先新建图片文件夹
            if (!Directory.Exists(this._origImgSavePath)) Directory.CreateDirectory(this._origImgSavePath);
            if (!Directory.Exists(this._resultImgSavePath)) Directory.CreateDirectory(this._resultImgSavePath);

            // 5张图片到服务器需要1.5s！
            // 每张图片开一个线程传输
            ManualResetEvent origSync = new ManualResetEvent(false);
            ManualResetEvent testSync = new ManualResetEvent(false);
            string failMsg = string.Empty;
            object syncObj = new object();

            if (!isStartup)
            {
                new Thread(() =>
                {
                    Thread.CurrentThread.IsBackground = true;

                    if (this._checkMode == ECheckModes.Diagonal_1_2 || this._checkMode == ECheckModes.FourSides)
                    {
                        for (int i = 0; i < 2; i++)
                        {
                            try
                            {
                                this.SaveOneCorner(i, imgconfig, useSTFPath, true);
                            }
                            catch (Exception ex)
                            {
                                lock (syncObj)
                                {
                                    failMsg = ex.Message.ToString();
                                }
                            }
                        }
                    }

                    if (this._checkMode == ECheckModes.Diagonal_3_4 || this._checkMode == ECheckModes.FourSides)
                    {
                        for (int i = 2; i < 4; i++)
                        {
                            try
                            {
                                this.SaveOneCorner(i, imgconfig, useSTFPath, true);
                            }
                            catch (Exception ex)
                            {
                                lock (syncObj)
                                {
                                    failMsg = ex.Message.ToString();
                                }
                            }
                        }
                    }
                    origSync.Set();
                }).Start();
            }
            else
            {
                origSync.Set();
            }

            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;

                if (this._checkMode == ECheckModes.Diagonal_1_2 || this._checkMode == ECheckModes.FourSides)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        try
                        {
                            this.SaveOneCorner(i, imgconfig, useSTFPath, false);
                        }
                        catch (Exception ex)
                        {
                            lock (syncObj)
                            {
                                failMsg = ex.Message.ToString();
                            }
                        }
                    }
                }

                if (this._checkMode == ECheckModes.Diagonal_3_4 || this._checkMode == ECheckModes.FourSides)
                {
                    for (int i = 2; i < 4; i++)
                    {
                        try
                        {
                            this.SaveOneCorner(i, imgconfig, useSTFPath, false);
                        }
                        catch (Exception ex)
                        {
                            lock (syncObj)
                            {
                                failMsg = ex.Message.ToString();
                            }
                        }
                    }
                }
                testSync.Set();
            }).Start();

            if (imgconfig.SaveTestNGImage || imgconfig.SaveTestOKImage)
            {
                this.ResultImage.Save(Path.Combine(this.ResultImgSavePath, this.ResultImgFileName));
                if (this.FinalResult == true && this.StfResult == true && this.IsGrrMode == false && (_sn.Length == 12 || _sn.Length == 30)) // Upload OK Image Only!
                {
                    DateTime startTime = DateTime.Now;
                    LoggingIF.Log("开始上传图片", LogLevels.Info, "SaveAll");

                    string picName = ResultImgFileName;
                    string mesSavePath = string.Empty;    //_stfImgSavePath

                    mesSavePath = UserDefineVariableInfo.DicVariables["mesImgSavePath"].ToString();

                    mesSavePath = mesSavePath.Replace("//", "\\\\");
                    mesSavePath = mesSavePath.Replace("/", "\\");


                    if (mesSavePath.EndsWith("\\"))
                    {
                        //
                    }
                    else
                    {
                        mesSavePath += "\\";
                    }

                    mesSavePath += _sn.Length == 12 ? _sn.Substring(0, 3) : _sn.Substring(18, 3);
                    mesSavePath += "\\";
                    mesSavePath += Station.Current.EquipmentID;//UserDefineVariableInfo.DicVariables["AssetsNO"].ToString();
                    mesSavePath += "\\";
                    mesSavePath += DateTime.Now.ToString("yyyyMMdd");
                    mesSavePath += "\\";
                    mesSavePath += _sn.Length == 12 ? _sn.Substring(3, 4) : _sn.Substring(21, 4);
                    mesSavePath += "\\";
                    mesSavePath += _sn;
                    mesSavePath += "\\";

                    if (!Directory.Exists(mesSavePath)) Directory.CreateDirectory(mesSavePath);
                    bool ret = false;

                    try
                    {
                        int flat = 30;
                        if (_finalResult == false)
                        {
                            flat = 60;
                        }
                        ret = PicCompress.PicCompress.GetPicThumbnail(Path.Combine(this._resultImgSavePath, this.ResultImgFileName), Path.Combine(mesSavePath, picName), flat);
                        this.StfImgSavePath = Path.Combine(mesSavePath, picName);
                    }
                    catch (Exception ex)
                    {
                        string msg = "sFile: " + Path.Combine(this._resultImgSavePath, this.ResultImgFileName) + " dFile: " + Path.Combine(mesSavePath, picName) + "flag: 60";
                        LoggingIF.Log(msg, LogLevels.Info, "SaveAll");
                        LoggingIF.Log("压缩上传图片异常: " + ex.Message, LogLevels.Info, "SaveAll");

                        lock (syncObj)
                        {
                            failMsg = ex.Message.ToString();
                        }
                    }
                    LoggingIF.Log("upload picture to " + Path.Combine(mesSavePath, picName), LogLevels.Info, "SaveAll");
                    DateTime endTime = DateTime.Now;
                    LoggingIF.Log(string.Format("上传图片完成, 耗时 = {0} 毫秒", (endTime - startTime).TotalMilliseconds), LogLevels.Info, "SaveAll");
                }
            }

            if (!origSync.WaitOne(writeTimeout) || !testSync.WaitOne(writeTimeout))
            {
                throw new Exception(string.Format("Save image for {0} timeout.", this.Sn));
            }

            if (failMsg != string.Empty)
            {
                throw new Exception(string.Format("Save image for {0} with error {1}", this.Sn, failMsg));
            }
        }

        private void SaveOneCorner(int i, ImageSaveConfig imgconfig, bool useSTFPath, bool isOrig)
        {
            BatteryCorner corner = null;
            if (i == 0) corner = this._corner1;
            else if (i == 1) corner = this._corner2;//MTW5  保存 1、3角
            else if (i == 2) corner = this._corner3;
            else corner = this._corner4;

            if (this.FinalResult)
            {
                if (imgconfig.SaveOrigOKImage && isOrig)
                {
                    //TODO: 增加图片类型参数(8/16位图) ZhangKF 2021-3-16
                    //corner.ShotImage.Save(Path.Combine(this._origImgSavePath, corner.OrigImageFileName), 1, corner.ImgNO);
                    corner.ShotImage.Save(Path.Combine(this._origImgSavePath, corner.OrigImageFileName), ZY.Vision.Consts.ImageTypes.Sixteen);

                    //LoggingIF.Log($"角{i + 1} 保存原图，图片编号={corner.ImgNO}", LogLevels.Info, "SaveOneCorner");
                }

                if (imgconfig.SaveTestOKImage && !isOrig)
                {
                    corner.InspectResults.resultDataMin.colorMat.Save(Path.Combine(this._resultImgSavePath, corner.ResultImgFileName));
                    //if (useSTFPath)
                    //{
                    //   corner.InspectResults.resultDataMin.colorMat.Save(Path.Combine(this._stfImgSavePath, corner.ResultImgFileName));
                    //}
                }
            }
            else
            {
                if (imgconfig.SaveOrigNGImage && isOrig)
                {
                    //TODO:增加图片类型参数(8/16位图) ZhangKF 2021-3-16
                    //corner.ShotImage.Save(Path.Combine(this._origImgSavePath, corner.OrigImageFileName), 1, corner.ImgNO);
                    corner.ShotImage.Save(Path.Combine(this._origImgSavePath, corner.OrigImageFileName), ZY.Vision.Consts.ImageTypes.Sixteen);

                    //LoggingIF.Log($"角{i + 1} 保存原图，图片编号={corner.ImgNO}", LogLevels.Info, "SaveOneCorner");
                }

                if (imgconfig.SaveTestNGImage && !isOrig)
                {
                    corner.InspectResults.resultDataMin.colorMat.Save(Path.Combine(this._resultImgSavePath, corner.ResultImgFileName));

                    // NG结果图片不上传，记住Path待人工复判定后重新上传
                    //if (useSTFPath)
                    //{
                    //    corner.InspectResults.resultDataMin.colorMat.Save(Path.Combine(this._stfImgSavePath, corner.ResultImgFileName));
                    //}
                }
            }
        }

        public override string ToString()
        {
            return "SN: " + this.Sn + Environment.NewLine
                + "CheckMode: " + this.CheckMode.ToString() + Environment.NewLine
                + "CheckExt: " + this.CheckExtension.ToString() + Environment.NewLine
                + "Scan Time: " + this.SnScanTime.ToString() + Environment.NewLine
                + "End Time: " + this.EndTime.ToString() + Environment.NewLine
                + "Result: " + this.FinalResult.ToString() + Environment.NewLine
                + "Result Code: " + this.ResultCode.ToString() + Environment.NewLine
                + "OrigImg Path: " + this.OrigImgSavePath.ToString() + Environment.NewLine
                + "ResultImg Path: " + this.ResultImgSavePath.ToString() + Environment.NewLine
                + "StfImg Path: " + this.StfImgSavePath.ToString() + Environment.NewLine
                + "ResultImg: " + this.ResultImgFileName.ToString() + Environment.NewLine
                + "Corner 1: " + this._corner1.ToString() + Environment.NewLine
                + "Corner 2: " + this._corner2.ToString() + Environment.NewLine
                + "Corner 3: " + this._corner3.ToString() + Environment.NewLine
                + "Corner 4: " + this._corner4.ToString() + Environment.NewLine
                + " batLength: " + this._batLength.ToString() + Environment.NewLine
                + " batWidth: " + this._batWidth.ToString() + Environment.NewLine;
        }
        public string StringSpilt(string PicName) //by lhw
        {
            int index = PicName.IndexOf('+') + 1;
            string last = PicName.Substring(index);
            string before = PicName.Substring(0, index - 1);
            index = before.LastIndexOf('_');
            before = before.Substring(0, index + 1);
            string newPicName = before + last;
            return newPicName;
        }
    }
}
