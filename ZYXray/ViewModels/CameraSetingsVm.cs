using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shuyz.Framework.Mvvm;
using XRayClient.VisionSysWrapper;
using System.Threading;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using ZYXray.Models;
using System.Drawing;
using System.IO;
using ZY.XRayTube;
using MahApps.Metro.Controls.Dialogs;
using MahApps.Metro.Controls;
using ZYXray.Utils;
using XRayClient.Core;
using ATL.Station;
using ZY.Vision;
using ZY.Vision.Utils;
using ZY.Logging;

namespace ZYXray.ViewModels
{
    public class CameraSetingsVm : ObservableObject
    {
        public FullScreenToggleHandler FullScreenToggleHandlers = null;
        public static CameraSetingsVm _instance = new CameraSetingsVm();
        private readonly int _refreshInterval = 100;
        private Thread _refreshThread = null;
        private bool _isAppExit = false;
        private ManualResetEvent _waitEvent = new ManualResetEvent(false);

        private bool _isFullScreen = false;
        private bool _isCam2Selected = false;
        private bool _isVideoOn = false;
        private bool _isFilterOn = false;
        private bool _isArmOne = true;
        private bool _isArmTwo = false;
        private CameraParams _cam1Params = new CameraParams(1);
        private CameraParams _cam2Params = new CameraParams(1);

        private ZYImageStruct _cam1Image = new ZYImageStruct();
        private ZYImageStruct _cam2Image = new ZYImageStruct();

        private ImageControlVm _imageControlVm = new ImageControlVm();
        private ImageControlVm _fullScreenImageControlVm = new ImageControlVm();

        private float _tempPixelRatio = 0f; // 临时像素比
        public int _shotDelayTime1 = 0;     //相机采图延时
        public int _shotDelayTime2 = 0;
        public int _shotDelayTime3 = 0;
        public int _shotDelayTime4 = 0;
        public int _scanBarcodeDelayTime = 0;



        public static CameraSetingsVm Instance
        {
            get { return _instance; }
        }
        public CameraSetingsVm()
        {
            this._imageControlVm.EnableMeasurements = false;

            this._cam1Params = HardwareConfig.Instance.CameraParams1;
            this._cam2Params = HardwareConfig.Instance.CameraParams2;
            this._shotDelayTime1 = HardwareConfig.Instance.CameraShotDelay1;
            this._shotDelayTime2 = HardwareConfig.Instance.CameraShotDelay2;
            this._shotDelayTime3 = HardwareConfig.Instance.CameraShotDelay3;
            this._shotDelayTime4 = HardwareConfig.Instance.CameraShotDelay4;

            this._cam1Image.Create(ImageDefinations.MaxImgWidth, ImageDefinations.MaxImgHeight, 1);
            this._cam1Image.width = ImageDefinations.ActualWidth;
            this._cam1Image.height = ImageDefinations.ActualHeight;

            this._cam2Image.Create(ImageDefinations.MaxImgWidth, ImageDefinations.MaxImgHeight, 1);
            this._cam2Image.width = ImageDefinations.ActualWidth;
            this._cam2Image.height = ImageDefinations.ActualHeight;

            if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                return;
            }

            this._refreshThread = new Thread(RefreshImage);
            this._refreshThread.IsBackground = true;
            this._refreshThread.Start();
        }

        ~CameraSetingsVm()
        {
            this._cam1Image.Destory();
            this._cam2Image.Destory();
        }

        public void OnExitting()
        {
            this._isAppExit = true;
            this._waitEvent.Set();

            if (null != this._refreshThread)
            {
                _refreshThread.Join();
            }
        }

        #region - Properties -

        public int ImageWidth
        {
            get { return ImageDefinations.ActualWidth; }
        }

        public int ImageHeight
        {
            get { return ImageDefinations.ActualHeight; }
        }

        public bool IsFullScreen
        {
            get { return this._isFullScreen; }
            set
            {
                this._isFullScreen = value;
                RaisePropertyChanged("IsFullScreen");
            }
        }

        public bool IsCam2Selected
        {
            get { return this._isCam2Selected; }
            set
            {
                this._isCam2Selected = value;
                RaisePropertyChanged("IsCam2Selected");

                RaisePropertyChanged("IsVideoOn");
                RaisePropertyChanged("IsFilterOn");

                RaisePropertyChanged("CamModel");
                RaisePropertyChanged("Width");
                RaisePropertyChanged("Height");
                RaisePropertyChanged("AvgValue");
                RaisePropertyChanged("SerialNo");
                RaisePropertyChanged("PixelRatio");
                RaisePropertyChanged("Exposure");
                RaisePropertyChanged("Gain");
                RaisePropertyChanged("MinGray");
                RaisePropertyChanged("MaxGray");

                RaisePropertyChanged("XFlip");
                RaisePropertyChanged("YFlip");

                RaisePropertyChanged("XTubeVal");
                RaisePropertyChanged("XTubeCurrent");

                if (!this._isCam2Selected)
                {
                    this.TempPixelRatio = this._cam1Params.pixelRatio;
                    this._fullScreenImageControlVm.PixelRatio = this._cam1Params.pixelRatio;
                }
                else
                {
                    this.TempPixelRatio = this._cam2Params.pixelRatio;
                    this._fullScreenImageControlVm.PixelRatio = this._cam2Params.pixelRatio;
                }

                this.UpdateBindingImage();
            }
        }

        public bool IsVideoOn
        {
            get
            {
                return this._isVideoOn;
            }
            set
            {
                this._isVideoOn = value;
                RaisePropertyChanged("IsVideoOn");
            }
        }

        public bool IsFilterOn
        {
            get
            {
                return this._isFilterOn;
            }
            set
            {
                this._isFilterOn = value;
                RaisePropertyChanged("IsFilterOn");
            }
        }

        public bool IsArmOne
        {
            get
            {
                return this._isArmOne;
            }
            set
            {
                this._isArmOne = value;
                this._isArmTwo = !value;
                RaisePropertyChanged("IsArmOne");
                RaisePropertyChanged("IsArmTwo");

                this.TempPixelRatio = this._cam1Params.pixelRatio;
                this._fullScreenImageControlVm.PixelRatio = this._cam1Params.pixelRatio;
                RaisePropertyChanged("TempPixelRatio");
            }
        }

        public bool IsArmTwo
        {
            get
            {
                return this._isArmTwo;
            }
            set
            {
                this._isArmTwo = value;
                this._isArmOne = !value;
                RaisePropertyChanged("IsArmTwo");
                RaisePropertyChanged("IsArmOne");

                this.TempPixelRatio = this._cam1Params.pixelRatio2;
                this._fullScreenImageControlVm.PixelRatio = this._cam1Params.pixelRatio2;
                RaisePropertyChanged("TempPixelRatio");
            }
        }

        public ImageControlVm AvtiveImage
        {
            get
            {
                return this._imageControlVm;
            }
        }

        public ImageControlVm FullScreenActiveImage
        {
            get
            {
                return this._fullScreenImageControlVm;
            }
        }

        public float ActualDist
        {
            get { return HardwareConfig.Instance.CaliDist; }
            set
            {
                HardwareConfig.Instance.CaliDist = value;
                RaisePropertyChanged("ActualDist");
            }
        }


        public float TempPixelRatio
        {
            get
            {
                if (this._tempPixelRatio == 0)
                {
                    return HardwareConfig.Instance.PixelRatio;
                }
                else
                {
                    return this._tempPixelRatio;
                }
            }
            set
            {
                HardwareConfig.Instance.PixelRatio = value;
                this._tempPixelRatio = value;
                RaisePropertyChanged("TempPixelRatio");
            }
        }

        private void UpdateBindingImage()
        {
            if (!Application.Current.Dispatcher.CheckAccess())
            {
                Application.Current.Dispatcher.Invoke((Action)(() =>
                {
                    if (!this._isCam2Selected)
                    {
                        if (!this._isFullScreen)
                        {
                            this._imageControlVm.UpdateImage(ref this._cam1Image);
                        }
                        else
                        {
                            this._fullScreenImageControlVm.UpdateImage(ref this._cam1Image);
                        }
                    }
                    else
                    {
                        if (!this._isFullScreen)
                        {
                            this._imageControlVm.UpdateImage(ref this._cam2Image);
                        }
                        else
                        {
                            this._fullScreenImageControlVm.UpdateImage(ref this._cam2Image);
                        }
                    }
                }));
            }
            else
            {
                if (!this._isCam2Selected)
                {
                    if (!this._isFullScreen)
                    {
                        this._imageControlVm.UpdateImage(ref this._cam1Image);
                    }
                    else
                    {
                        this._fullScreenImageControlVm.UpdateImage(ref this._cam1Image);
                    }
                }
                else
                {
                    if (!this._isFullScreen)
                    {
                        this._imageControlVm.UpdateImage(ref this._cam2Image);
                    }
                    else
                    {
                        this._fullScreenImageControlVm.UpdateImage(ref this._cam2Image);
                    }
                }
            }
        }

        //TODO: 为新版相机采图替换方法 ZhangKF 2020-12-31
        ///<summary>新版相机封装</summary>
        private void UpdateBindingImage2(Bitmap bitmap)
        {
            if (!Application.Current.Dispatcher.CheckAccess())
            {
                Application.Current.Dispatcher.Invoke((Action)(() =>
                {
                    if (!this._isCam2Selected)
                    {
                        if (!this._isFullScreen)
                        {
                            this._imageControlVm.UpdateImage2(bitmap);
                        }
                        else
                        {
                            this._fullScreenImageControlVm.UpdateImage2(bitmap);
                        }
                    }
                    else
                    {
                        if (!this._isFullScreen)
                        {
                            this._imageControlVm.UpdateImage2(bitmap);
                        }
                        else
                        {
                            this._fullScreenImageControlVm.UpdateImage2(bitmap);
                        }
                    }
                }));
            }
            else
            {
                if (!this._isCam2Selected)
                {
                    if (!this._isFullScreen)
                    {
                        this._imageControlVm.UpdateImage2(bitmap);
                    }
                    else
                    {
                        this._fullScreenImageControlVm.UpdateImage2(bitmap);
                    }
                }
                else
                {
                    if (!this._isFullScreen)
                    {
                        this._imageControlVm.UpdateImage2(bitmap);
                    }
                    else
                    {
                        this._fullScreenImageControlVm.UpdateImage2(bitmap);
                    }
                }
            }
        }
        #region - Params -

        public List<string> SnList
        {
            get { return VisionSysWrapperIF.CamList; }
        }

        public IEnumerable<ECamModels> BindableCamModels
        {
            get
            {
                return Enum.GetValues(typeof(ECamModels))
                    .Cast<ECamModels>();
            }
        }

        public ECamModels CamModel
        {
            get
            {
                if (!this._isCam2Selected) return (ECamModels)this._cam1Params.camType;
                return (ECamModels)this._cam2Params.camType;
            }
            set
            {
                if (!this._isCam2Selected) this._cam1Params.camType = (int)value;
                else this._cam2Params.camType = (int)value;

                RaisePropertyChanged("CamModel");
            }
        }

        public int Width
        {
            get
            {
                if (!this._isCam2Selected) return this._cam1Params.nWidth;
                return this._cam2Params.nWidth;
            }
            set
            {
                if (!this._isCam2Selected) this._cam1Params.nWidth = value;
                else this._cam2Params.nWidth = value;

                RaisePropertyChanged("Width");
            }
        }

        public int Height
        {
            get
            {
                if (!this._isCam2Selected) return this._cam1Params.nHeight;
                return this._cam2Params.nHeight;
            }
            set
            {
                if (!this._isCam2Selected) this._cam1Params.nHeight = value;
                else this._cam2Params.nHeight = value;

                RaisePropertyChanged("Height");
            }
        }

        public int AvgValue
        {
            get
            {
                if (!this._isCam2Selected) return this._cam1Params.PinValue;
                return this._cam2Params.PinValue;
            }
            set
            {
                if (!this._isCam2Selected) this._cam1Params.PinValue = value;
                else this._cam2Params.PinValue = value;

                RaisePropertyChanged("AvgValue");
            }
        }

        public int ShotDelayTime1
        {
            get
            {
                this._shotDelayTime1 = HardwareConfig.Instance.CameraShotDelay1;
                return this._shotDelayTime1;
            }
            set
            {
                this._shotDelayTime1 = value;
                HardwareConfig.Instance.CameraShotDelay1 = this._shotDelayTime1;
                RaisePropertyChanged("ShotDelayTime1");
            }
        }

        public int ShotDelayTime2
        {
            get
            {
                this._shotDelayTime2 = HardwareConfig.Instance.CameraShotDelay2;
                return this._shotDelayTime2;
            }
            set
            {
                this._shotDelayTime2 = value;
                HardwareConfig.Instance.CameraShotDelay2 = this._shotDelayTime2;
                RaisePropertyChanged("ShotDelayTime2");
            }
        }

        public int ShotDelayTime3
        {
            get
            {
                this._shotDelayTime3 = HardwareConfig.Instance.CameraShotDelay3;
                return this._shotDelayTime3;
            }
            set
            {
                this._shotDelayTime3 = value;
                HardwareConfig.Instance.CameraShotDelay3 = this._shotDelayTime3;
                RaisePropertyChanged("ShotDelayTime3");
            }
        }

        public int ShotDelayTime4
        {
            get
            {
                this._shotDelayTime4 = HardwareConfig.Instance.CameraShotDelay4;
                return this._shotDelayTime4;
            }
            set
            {
                this._shotDelayTime4 = value;
                HardwareConfig.Instance.CameraShotDelay4 = this._shotDelayTime4;
                RaisePropertyChanged("ShotDelayTime4");
            }
        }
        public int ScanBarcodeDelayTime
        {
            get
            {
                this._scanBarcodeDelayTime = HardwareConfig.Instance.ScanBarcodeDelay;
                return this._scanBarcodeDelayTime;
            }
            set
            {
                this._scanBarcodeDelayTime = value;
                HardwareConfig.Instance.ScanBarcodeDelay = this._scanBarcodeDelayTime;
                RaisePropertyChanged("ScanBarcodeDelayTime");
            }
        }
        public string SerialNo
        {
            get
            {
                if (!this._isCam2Selected) return this._cam1Params.SzBufSeriNum;
                return this._cam2Params.SzBufSeriNum;
            }
            set
            {
                if (!this._isCam2Selected) this._cam1Params.SzBufSeriNum = value;
                else this._cam2Params.SzBufSeriNum = value;

                RaisePropertyChanged("SerialNo");
            }
        }

        public float PixelRatio
        {
            get
            {
                if (!this._isCam2Selected) return this._cam1Params.pixelRatio;
                return this._cam2Params.pixelRatio;
            }
            set
            {
                if (!this._isCam2Selected) this._cam1Params.pixelRatio = value;
                else this._cam2Params.pixelRatio = value;

                RaisePropertyChanged("PixelRatio");

                this._fullScreenImageControlVm.PixelRatio = this.PixelRatio;
            }
        }

        public int Exposure
        {
            get
            {
                if (!this._isCam2Selected) return this._cam1Params.exposure;
                return this._cam2Params.exposure;
            }
            set
            {
                if (!this._isCam2Selected) this._cam1Params.exposure = value;
                else this._cam2Params.exposure = value;

                RaisePropertyChanged("Exposure");

                Task.Run(() =>
                {
                    if (!this._isCam2Selected)
                    {
                        VisionSysWrapperIF.SetExposureTime(ECamTypes.Camera1, this.Exposure);
                    }
                    else
                    {
                        VisionSysWrapperIF.SetExposureTime(ECamTypes.Camera2, this.Exposure);
                    }
                });
            }
        }

        public int Gain
        {
            get
            {
                if (!this._isCam2Selected) return this._cam1Params.gain;
                return this._cam2Params.gain;
            }
            set
            {
                if (!this._isCam2Selected) this._cam1Params.gain = value;
                else this._cam2Params.gain = value;

                RaisePropertyChanged("Gain");

                Task.Run(() =>
                {
                    if (!this._isCam2Selected)
                    {
                        VisionSysWrapperIF.SetGain(ECamTypes.Camera1, this.Gain);
                    }
                    else
                    {
                        VisionSysWrapperIF.SetGain(ECamTypes.Camera2, this.Gain);
                    }
                });
            }
        }

        public int MinGray
        {
            get
            {
                if (!this._isCam2Selected)
                {
                    return this._cam1Params.min_graylevel;
                }
                else
                {
                    return this._cam2Params.min_graylevel;
                }
            }
            set
            {
                if (!this._isCam2Selected)
                {
                    this._cam1Params.min_graylevel = value;

                    RaisePropertyChanged("MinGray");

                    Task.Run(() =>
                    {
                        VisionSysWrapper.VisionSysNativeIF_SetSreachTable(0, MinGray, MaxGray);
                    });
                }
                else
                {
                    this._cam2Params.min_graylevel = value;

                    RaisePropertyChanged("MinGray");

                    Task.Run(() =>
                    {
                        VisionSysWrapper.VisionSysNativeIF_SetSreachTable(1, MinGray, MaxGray);
                    });
                }
            }
        }

        public int MaxGray
        {
            get
            {
                if (!this._isCam2Selected)
                {
                    return this._cam1Params.max_graylevel;
                }
                else
                {
                    return this._cam2Params.max_graylevel;
                }
            }
            set
            {
                if (!this._isCam2Selected)
                {
                    this._cam1Params.max_graylevel = value;

                    RaisePropertyChanged("MaxGray");

                    Task.Run(() =>
                    {
                        VisionSysWrapper.VisionSysNativeIF_SetSreachTable(0, MinGray, MaxGray);
                    });
                }
                else
                {
                    this._cam2Params.max_graylevel = value;

                    RaisePropertyChanged("MaxGray");

                    Task.Run(() =>
                    {
                        VisionSysWrapper.VisionSysNativeIF_SetSreachTable(1, MinGray, MaxGray);
                    });
                }
            }
        }

        public bool XFlip
        {
            get
            {
                if (!this._isCam2Selected) return this._cam1Params.xFlip;
                return this._cam2Params.xFlip;
            }
            set
            {
                if (!this._isCam2Selected) this._cam1Params.xFlip = value;
                else this._cam2Params.xFlip = value;

                RaisePropertyChanged("XFlip");
            }
        }

        public bool YFlip
        {
            get
            {
                if (!this._isCam2Selected) return this._cam1Params.yFlip;
                return this._cam2Params.yFlip;
            }
            set
            {
                if (!this._isCam2Selected) this._cam1Params.yFlip = value;
                else this._cam2Params.yFlip = value;

                RaisePropertyChanged("YFlip");
            }
        }

        public int XTubeVal
        {
            get
            {
                if (!this._isCam2Selected) return HardwareConfig.Instance.XRayConfig1.PresetVoltage;
                else return HardwareConfig.Instance.XRayConfig2.PresetVoltage;
            }
            set
            {
                if (!this._isCam2Selected)
                {
                    HardwareConfig.Instance.XRayConfig1.PresetVoltage = value;
                    XRayTubeIF.SetXrayVoltage(value, ETubePosition.Position1);
                }
                else
                {
                    HardwareConfig.Instance.XRayConfig2.PresetVoltage = value;
                    XRayTubeIF.SetXrayVoltage(value, ETubePosition.Position2);
                }

                HardwareConfig.Instance.SaveXTubeConfigOnly();
                RaisePropertyChanged("XTubeVal");
            }
        }

        public int XTubeCurrent
        {
            get
            {
                if (!this._isCam2Selected) return HardwareConfig.Instance.XRayConfig1.PresetCurrent;
                else return HardwareConfig.Instance.XRayConfig2.PresetCurrent;
            }
            set
            {
                if (!this._isCam2Selected)
                {
                    HardwareConfig.Instance.XRayConfig1.PresetCurrent = value;
                    XRayTubeIF.SetXrayCurrent(value, ETubePosition.Position1);
                }
                else
                {
                    HardwareConfig.Instance.XRayConfig2.PresetCurrent = value;
                    XRayTubeIF.SetXrayCurrent(value, ETubePosition.Position2);
                }

                HardwareConfig.Instance.SaveXTubeConfigOnly();
                RaisePropertyChanged("XTubeCurrent");
            }
        }

        #endregion

        #endregion

        /// <summary>
        /// 连续取图线程
        /// </summary>
        public void RefreshImage()
        {
            while (true)
            {
                if (this._isAppExit)
                {
                    break;
                }

                if (!Instance.IsVideoOn)
                {
                    this._waitEvent.WaitOne(this._refreshInterval);
                    continue;
                }

                // 工作状态下无法开启视频
                if (Station.Current.StationState == "Run")
                {
                    if (this._isVideoOn)
                    {
                        Instance.IsVideoOn = false;
                    }
                    continue;
                }

                this.GetOneImage();
                Thread.Sleep(300);
                this._waitEvent.WaitOne(this._refreshInterval);
            }
        }

        private void GetOneImage()
        {
            int iRet = 0;
            int k = 0;
            if (!Directory.Exists(@"D:\NeedleSrcDst"))
            {
                Directory.CreateDirectory(@"D:\NeedleSrcDst");
            }

            // 相机1刷新
            if (!this._isCam2Selected)
            {
                CaptureResult result = null;
                if (!this.IsFilterOn)
                {
                    //TODO: 替换旧版相机采图  ZhangKF 2021-3-16  
                    //iRet = VisionSysWrapperIF.ShotOne(ECamTypes.Camera1, ref this._cam1Image);
                    result = CameraHelper.CaptrueOneImageFrame(0, 1);
                }
                else
                {
                    int tickCount = Environment.TickCount;
                    ////TODO: 替换旧版相机采图  ZhangKF 2021-3-16  
                    //iRet = VisionSysWrapperIF.ShotAndAverage(ECamTypes.Camera1, this._cam1Params.PinValue, ref this._cam1Image, 4, out k);
                    result = CameraHelper.CaptrueOneImageFrame(0, _cam1Params.PinValue);
                    if (!this._isVideoOn)
                    {
                        MessageBox.Show((Environment.TickCount - tickCount).ToString());
                    }
                }
                
                if (result.Result)
                {
                    result.Bitmap=result.Bitmap.Rotate2();
                    result.Bitmap.SaveToJPG(@"D:\Test\1.jpg");
                    result.Bitmap.SaveToJPG(@"D:\NeedleSrcDst\src.jpg");
                    this.UpdateBindingImage2(result.Bitmap);
                    
                }

            }
            // 相机2刷新
            else
            {
                CaptureResult result = null;
                if (!this.IsFilterOn)
                {
                    //TODO: 替换旧版相机采图  ZhangKF 2021-3-16  
                    //iRet = VisionSysWrapperIF.ShotOne(ECamTypes.Camera2, ref this._cam2Image);
                    result = CameraHelper.CaptrueOneImageFrame(1, 1);
                }
                else
                {
                    //iRet = VisionSysWrapperIF.ShotAndAverage(ECamTypes.Camera2, this._cam2Params.PinValue, ref this._cam2Image, 4, out k);
                    result = CameraHelper.CaptrueOneImageFrame(1, _cam1Params.PinValue);
                }
               
                if (result.Result)
                {
                    result.Bitmap=result.Bitmap.Rotate2();
                    result.Bitmap.SaveToJPG(@"D:\Test\2.jpg");
                    result.Bitmap.SaveToJPG(@"D:\NeedleSrcDst\src.jpg");
                    this.UpdateBindingImage2(result.Bitmap);
                }
            }
        }

        public ICommand SaveConfig
        {
            get
            {
                return new RelayCommand(new Action(delegate
                {
                    HardwareConfig.Instance.CameraParams1 = this._cam1Params;
                    HardwareConfig.Instance.CameraParams2 = this._cam2Params;

                    HardwareConfig.Instance.SaveConfig();
                    BotIF.ChangeParam(this._cam1Params, this._cam2Params);
                }), delegate
                {
                    return true;
                });
            }
        }

        public ICommand SaveConfigPixelRatio
        {
            get
            {
                return new RelayCommand(new Action(delegate
                {
                    if (System.Windows.MessageBox.Show("是否要保存像素比?", "Confirm Message", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
                    {
                        this.PixelRatio = this.TempPixelRatio;

                        HardwareConfig.Instance.CameraParams1 = this._cam1Params;
                        HardwareConfig.Instance.CameraParams2 = this._cam2Params;

                        if (!_isCam2Selected) HardwareConfig.Instance.SaveConfig(1);
                        else HardwareConfig.Instance.SaveConfig(2);

                        BotIF.ChangeParam(this._cam1Params, this._cam2Params);
                    }


                }), delegate
                {
                    return true;
                });
            }
        }

        public ICommand CalcPixelRatio
        {
            get
            {
                return new RelayCommand(new Action(delegate
                {
                    if (0 == this._fullScreenImageControlVm.PixelDist)
                    {
                        MessageBox.Show("计算失败！请按下鼠标左键拉一条直线并输入正确的物理实际距离！", "错误");
                        return;
                    }

                    this.TempPixelRatio = this.ActualDist / this._fullScreenImageControlVm.PixelDist;
                }), delegate
                {
                    return true;
                });
            }
        }

        public ICommand ManualUpdate
        {
            get
            {
                return new RelayCommand(new Action(delegate
                {
                    if (this._isVideoOn) return;
                    this.GetOneImage();
                }), delegate
                {
                    return true;
                });
            }
        }

        public ICommand AutoCheck
        {
            get
            {
                return new RelayCommand(new Action(delegate
                {
                    if (this._isVideoOn)
                    {
                        MessageBox.Show("请先关闭视频模式");
                    }
                    else
                    {
                        //string path = @"D:\NeedleSrcDst";
                        //if (!Directory.Exists(path))
                        //{
                        //    Directory.CreateDirectory(path);
                        //}
                        //float length = 1;
                        //float pxtomm = 1;
                        //bool b=VisionSysWrapper.CshapeNeedleLengthDetect(ref length, ref pxtomm, true);

                        float length = 1;
                        float pxtomm = FullScreenActiveImage.PixelRatio;
                        try
                        {
                            bool algoResult = VisionSysWrapper.CshapeNeedleLengthDetect(ref length, ref pxtomm, true);//点检传True，传像素比，达到长度；标定传False，传长度，返回像素比

                            if (algoResult == true)
                            {
                                string filePath = @"D:\NeedleSrcDst\dst.jpg";
                                if (File.Exists(filePath))
                                {
                                    ZYImageStruct img = new ZYImageStruct();
                                    using (Bitmap bmp = new Bitmap(filePath))
                                    {
                                        img.UpdateWithBitmap(bmp);
                                    }
                                    Application.Current.Dispatcher.Invoke((Action)(() =>
                                    {
                                        this._fullScreenImageControlVm.UpdateImage(ref img);
                                    }));
                                    img.Destory();
                                    string msg = string.Empty, result = "NG";
                                    if (length >= 0.095 && length <= 1.005)
                                    {
                                        result = "OK";
                                        msg = "，针规点检通过！";
                                        CheckParamsConfig.Instance.NeedleCheckTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                        CheckParamsSettingsVm.Instance.MyCheckParamsConfig.Write();
                                    }
                                    SaveResultToFile_Needle(length, result);
                                    msg = "针规长度为：" + length + " mm" + msg;
                                    MessageBox.Show(msg);
                                    LoggingIF.Log(msg, LogLevels.Info, "AutoCheck");
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.ToString());
                        }
                    }

                }), delegate
                {
                    return true;
                });
            }
        }

        private void SaveResultToFile_Needle(float length, string result)
        {
            string filename = "D:\\测量数据\\点检数据\\针规点检\\";
            if (!Directory.Exists(filename)) Directory.CreateDirectory(filename);
            filename += "针规点检_" + DateTime.Now.ToString("yyyyMMdd") + ".csv";
            string value = string.Empty;
            if (!File.Exists(filename))
            {
                value = "点检时间,员工工号,测量值,点检结果\r\n";
            }
            try
            {
                using (FileStream fs = new FileStream(filename, FileMode.Append, FileAccess.Write, FileShare.Write))
                {
                    value += DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "," + MotionControlVm.OperaterId + "," + length + "," + result + "\r\n";
                    byte[] buffer = Encoding.Default.GetBytes(value);
                    fs.Write(buffer, 0, buffer.Length);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public ICommand ToggleFullScreen
        {
            get
            {
                return new RelayCommand(new Action(delegate
                {
                    this.IsFullScreen = !this.IsFullScreen;
                    if (this.IsFullScreen)
                    {
                        this._fullScreenImageControlVm.PixelRatio = this.PixelRatio;
                        this._fullScreenImageControlVm.EnableMeasurements = true;
                        this.TempPixelRatio = this.PixelRatio;
                    }

                    if (null != FullScreenToggleHandlers)
                    {
                        try
                        {
                            this.FullScreenToggleHandlers(this._isFullScreen);
                            this.UpdateBindingImage();
                        }
                        catch { }
                    }
                }), delegate
                {
                    return true;
                });
            }
        }

        public ICommand ShowCNC
        {
            get
            {
                return new RelayCommand(new Action(delegate
                {
                    var window = new Window();
                    MotionControlPage mv = new MotionControlPage();
                    window.Content = mv;
                    window.Height = 900;
                    window.Width = 1600;
                    window.Top = 100;
                    window.Left = 100;
                    window.Show();
                }), delegate
                {
                    return true;
                });
            }
        }

    }
}
