using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shuyz.Framework.Mvvm;
using System.Windows;
using XRayClient.VisionSysWrapper;
using XRayClient.Core;
using XRayClient.DeviceController;
using System.Windows.Input;
using ZYXray.Models;
using MahApps.Metro.Controls.Dialogs;
using MahApps.Metro.Controls;
using System.Drawing;
using XRayClient.BatteryCheckManager;
using XRayClient.Core.Options;
using System.Threading;
using ZY.XRayTube;
using ZY.Logging;

namespace ZYXray.ViewModels
{
    public class DashBoardVm : ObservableObject
    {
        private ImageControlVm _imageControlVm = new ImageControlVm();

        private EDashBoardActiveLayers _activeLayer = EDashBoardActiveLayers.Main;
        private CameraSetingsVm _cameraSettinsVm = null;

        private string _warnMsg = string.Empty;

        private ManualRecheckVm _manualCheckVm = null;

        public SystemConfig MySystemConfig
        {
            get { return SystemConfig.Instance; }
        }

        public CameraSetingsVm MyCameraSetingsVm
        {
            set { this._cameraSettinsVm = value; }
        }

        public string WarnMsg
        {
            get { return this._warnMsg; }
            set
            {
                this._warnMsg = value;
                RaisePropertyChanged("WarnMsg");
            }
        }


        public IEnumerable<ECheckExtensions> BindableCheckExtensions
        {
            get
            {
                return Enum.GetValues(typeof(ECheckExtensions))
                    .Cast<ECheckExtensions>();
            }
        }

        public DashBoardVm()
        {
            BotIF.CheckResultHandlers += BotIF_CheckResultHandlers;

            this.ChangeActiveLayer.Execute((int)EDashBoardActiveLayers.Main);
        }

        private void BotIF_CheckResultHandlers(BatterySeat batSeat)
        {
            this.UpdateMsg(batSeat.ResultCode);

            if (!Application.Current.Dispatcher.CheckAccess())
            {
                Application.Current.Dispatcher.Invoke((Action)(() =>
                {
                    this.DoUpdate(batSeat);
                }));
            }
            else
            {
                this.DoUpdate(batSeat);
            }
        }

        ~DashBoardVm()
        {
            BotIF.CheckResultHandlers -= BotIF_CheckResultHandlers;
        }

        public Statistics MyStatistics
        {
            get { return BotIF.MyCheckStatus.MyStatistics; }
        }

        public CheckStatus MyCheckStatus
        {
            get { return BotIF.MyCheckStatus; }
        }

        public ImageControlVm AvtiveImage
        {
            get
            {
                return this._imageControlVm;
            }
        }

        public ManualRecheckVm MyManualRecheckVm
        {
            set { this._manualCheckVm = value; }
        }

        private void DoUpdate(BatterySeat batSeat)
        {
            this._imageControlVm.UpdateImage(ref batSeat.ResultImage);
        }

        private void UpdateMsg(EResultCodes resultCode)
        {
            if(resultCode != EResultCodes.OK && resultCode != EResultCodes.AlgoFail)
            {
                this.WarnMsg = LangHelper.getTranslation("LL_EResultCodes" + "_" + resultCode.ToString());
            }
            else
            {
                this.WarnMsg = string.Empty;
            }

            if ((XRayTubeIF.XRayTube1Stauts.AlarmTime < XRayTubeIF.XRayTube1Stauts.UseHoursInTotal))
            {
                this.WarnMsg += LangHelper.getTranslation("LL_XRayOutTimeTips");
            }
        }

        public DeviceStatus MyDeviceStatus
        {
            get { return DeviceControllerIF.MyDeviceStatus; }
        }

        public EDashBoardActiveLayers ActiveLayer
        {
            get
            {
                return this._activeLayer;
            }
            set
            {
                this._activeLayer = value;
                RaisePropertyChanged("ActiveLayer");
            }
        }

        public ICommand OpenManualCheckPage
        {
            get
            {
                return new RelayCommand(new Action(delegate
                {
                    if (null != this._manualCheckVm)
                    {
                        this._manualCheckVm.MyManualRecheck.MyRecheckStatus.IsRecheckMode = true;
                        this._manualCheckVm.ToggleFullScreen.Execute(null);
                    }
                }), delegate
                {
                    return true;
                });
            }
        }

        public ICommand OpenCameraCaliPage
        {
            get
            {
                return new RelayCommand(new Action(delegate
                {
                    if (null != this._cameraSettinsVm)
                    {
                        this._cameraSettinsVm.ToggleFullScreen.Execute(null);
                    }
                }), delegate
                {
                    return true;
                });
            }
        }

        public ICommand OpenFQAPage
        {
            get
            {
                return new RelayCommand(new Action(delegate
                {
                    if (null != this._manualCheckVm)
                    {
                        this._manualCheckVm.MyManualRecheck.MyRecheckStatus.IsRecheckMode = false;
                        this._manualCheckVm.ToggleFullScreen.Execute(null);
                    }
                }), delegate
                {
                    return true;
                });
            }
        }

        public ICommand AutoModeSwitch
        {
            get
            {
                return new RelayCommand(new Action(delegate
                {
                    if (DeviceControllerIF.MyDeviceStatus.MyCommandStatus.PCReady.Flag)
                    {
                        BotIF.Stop();
                    }
                    else
                    {
                        BotIF.Start();
                    }
                    System.Threading.Thread.Sleep(100);
                }), delegate
                {
                    return true;
                });
            }
        }

        public ICommand ClearStat
        {
            get
            {
                return new RelayCommand(new Action(delegate
                {
                    this.MyStatistics.Reset();
                }), delegate
                {
                    return true;
                });
            }
        }

        public ICommand ChangeActiveLayer
        {
            get
            {
                return new RelayCommand<object>(new Action<object>(delegate (object layerIdx)
                {
                    int idx = int.Parse(layerIdx.ToString());

                    if (idx > (int)EDashBoardActiveLayers.StartupValidate) return;

                    this.ActiveLayer = (EDashBoardActiveLayers)idx;
                }), delegate
               {
                   return true;
               });
            }
        }
    }
}
