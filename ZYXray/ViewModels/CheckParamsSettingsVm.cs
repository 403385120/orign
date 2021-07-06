using System;
using Shuyz.Framework.Mvvm;
using XRayClient.Core;
using System.Windows.Forms;
using System.Windows.Input;
using XRayClient.Core.Options;
using XRayClient.DeviceController;
using ZYXray.Models;

namespace ZYXray.ViewModels
{
    public class CheckParamsSettingsVm : ObservableObject
    {
        private Login login = new Login();

        private static CheckParamsSettingsVm _instance = new CheckParamsSettingsVm();
        public static CheckParamsSettingsVm Instance
        {
            get { return _instance; }
        }

        public CheckParamsConfig MyCheckParamsConfig
        {
            get { return CheckParamsConfig.Instance; }
        }

        public ICommand SelectOrigSavePath
        {
            get
            {
                return new RelayCommand(new Action(delegate
                {
                    using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
                    {
                        System.Windows.Forms.DialogResult result = dialog.ShowDialog();

                        if (result == DialogResult.OK)
                        {

                            this.MyCheckParamsConfig.MyImageSaveConfig.SaveOrigPath = dialog.SelectedPath;
                        }
                    }
                }), delegate
                {
                    return true;
                });
            }
        }

        public ICommand SelectRecheckPath
        {
            get
            {
                return new RelayCommand(new Action(delegate
                {
                    using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
                    {
                        System.Windows.Forms.DialogResult result = dialog.ShowDialog();

                        if (result == DialogResult.OK)
                        {
                            this.MyCheckParamsConfig.MyImageSaveConfig.SaveRecheckPath = dialog.SelectedPath;
                        }
                    }
                }), delegate
                {
                    return true;
                });
            }
        }

        public ICommand SelectStartupPath
        {
            get
            {
                return new RelayCommand(new Action(delegate
                {
                    using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
                    {
                        System.Windows.Forms.DialogResult result = dialog.ShowDialog();

                        if (result == DialogResult.OK)
                        {
                            this.MyCheckParamsConfig.MyImageSaveConfig.StartupTestPath = dialog.SelectedPath;
                        }
                    }
                }), delegate
                {
                    return true;
                });
            }
        }

        public ICommand SelectTestSavePath
        {
            get
            {
                return new RelayCommand(new Action(delegate
                {
                    using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
                    {
                        System.Windows.Forms.DialogResult result = dialog.ShowDialog();

                        if (result == DialogResult.OK)
                        {
                            this.MyCheckParamsConfig.MyImageSaveConfig.SaveTestPath = dialog.SelectedPath;
                        }
                    }
                }), delegate
                {
                    return true;
                });
            }
        }

        public ICommand SaveConfig
        {
            get
            {
                return new RelayCommand(new Action(delegate
                {
                    MyCheckParamsConfig.Write();

                    BotIF.ChangeParam(HardwareConfig.Instance.CameraParams1, HardwareConfig.Instance.CameraParams2);
                    MotionControlVm.Instance.LoadWorkParams(CheckParamsConfig.Instance, HardwareConfig.Instance.CameraParams1, HardwareConfig.Instance.CameraParams2);
                    MyCheckParamsConfig.IsEnabled = false;
                }), delegate
                {
                    return true;
                });
            }
        }

        public ICommand MarkingSetting
        {
            get
            {
                return new RelayCommand(new Action(delegate
                {
                    SetMarking setMarking = new SetMarking();
                    setMarking.ShowDialog();
                }), delegate
                {
                    return true;
                });
            }
        }

        public ICommand Update
        {
            get
            {
                return new RelayCommand(new Action(delegate
                {
                    login.ShowDialog();
                    if (login.IsPermission == false)
                        return;
                    MyCheckParamsConfig.IsEnabled = true;

                }), delegate
                {
                    return true;
                });
            }
        }

        public DeviceStatus MyDeviceStatus
        {
            get { return DeviceControllerIF.MyDeviceStatus; }
        }

        public ICommand LoadMIData
        {
            get
            {
                return new RelayCommand(new Action(delegate
                {
                    MIConfig miconfig = new MIConfig();
                    MIRead.changedir(MyCheckParamsConfig.MIDataDir);
                    if (-1 == MIRead.GetMiConfig(MyCheckParamsConfig.ProductNO, ref miconfig))
                    {
                        System.Windows.MessageBox.Show("读取MI数据文件失败！请检查文件目录是否存在！", "错误");
                        return;
                    }

                    MyCheckParamsConfig.MinLengthHead = float.Parse(miconfig.StrSpecMin);
                    MyCheckParamsConfig.MaxLengthHead = float.Parse(miconfig.StrSpecMax);
                    MyCheckParamsConfig.TotalLayer = int.Parse(miconfig.StrLayerNum);
                    MyCheckParamsConfig.TotalLayersBD = int.Parse(miconfig.StrLayerNum_BD);
                }), delegate
                {
                    return true;
                });
            }
        }

        public ICommand ActiveMIData
        {
            get
            {
                return new RelayCommand(new Action(delegate
                {
                    MyCheckParamsConfig.Write();

                    BotIF.ChangeParam(HardwareConfig.Instance.CameraParams1, HardwareConfig.Instance.CameraParams2);
                }), delegate
                {
                    return true;
                });
            }
        }
    }
}
