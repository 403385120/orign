using System;
using Shuyz.Framework.Mvvm;
using System.Windows.Input;
using XRayClient.BatteryCheckManager;
using System.Threading;
using ZY.Logging;
using XRayClient.Core;
using System.Windows.Forms;

namespace ZYXray.ViewModels
{
    public class TestCodesVm : ObservableObject
    {
        private Login login = new Login();
        private int _numPerPage = 10;
        private int _curPageNum = 1;
        private bool _isBusy = false;

        private string _barCode = string.Empty;
        private string _remarks = string.Empty;

        public TestCodesVm()
        {
            BatteryCheckIF.MyTestCodeManager.Init();

            return;
        }

        public CheckParamsConfig MyCheckParamsConfig
        {
            get { return CheckParamsConfig.Instance; }
        }

        public TestCodeManager MyTestCodeManager
        {
            get { return BatteryCheckIF.MyTestCodeManager; }
        }

        public string BarCode
        {
            get { return this._barCode; }
            set
            {
                this._barCode = value.ToUpper();
                RaisePropertyChanged("BarCode");
            }
        }

        public string Remarks
        {

            get { return this._remarks; }
            set
            {
                this._remarks = value;
                RaisePropertyChanged("Remarks");
            }
        }

        public bool IsBusy
        {
            get { return this._isBusy; }
            set
            {
                this._isBusy = value;
                RaisePropertyChanged("IsBusy");
            }
        }

        public int NumPerPage
        {
            get { return this._numPerPage; }
            set
            {
                if (value == this._numPerPage) return;

                this._numPerPage = value;
                RaisePropertyChanged("NumPerPage");
            }
        }

        public int CurPageNum
        {
            get { return this._curPageNum; }
            set
            {
                if (value == this._curPageNum) return;

                this._curPageNum = value;
                RaisePropertyChanged("CurPageNum");
            }
        }

        public ICommand NavNext
        {
            get
            {
                return new RelayCommand(new Action(delegate
                {
                    int x = (int)Math.Ceiling((float)(this.MyTestCodeManager.CodeList.Count) / (float)this._numPerPage);
                    if (this._curPageNum >= x) return;

                    this.CurPageNum = this._curPageNum + 1;
                }), delegate
                {
                    return true;
                });
            }
        }

        public ICommand NavPrevious
        {
            get
            {
                return new RelayCommand(new Action(delegate
                {
                    if (this._curPageNum <= 1) return;
                    this.CurPageNum = this._curPageNum - 1;
                }), delegate
                {
                    return true;
                });
            }
        }

        public ICommand DeleteCode
        {
            get
            {
                return new RelayCommand<object>(new Action<object>(delegate (object group)
                {
                    new Thread(() =>
                    {
                        Thread.CurrentThread.IsBackground = true;
                        this.IsBusy = true;

                        try
                        {
                            if (DialogResult.Yes == MessageBox.Show("确定删除吗?", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information, 
                                MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification))
                            {
                                BatteryCheckIF.RemoveTestCode(((TestCode)group).RecordID);
                                this.CurPageNum = 1;
                            }
                        }
                        catch (System.Exception ex)
                        {
                            LoggingIF.Log(string.Format("Failed to delete test code {0}", ex.Message.ToString()), LogLevels.Error, "TestCodesVm");

                            System.Windows.MessageBox.Show("错误\r\n" + ex.Message.ToString());
                        }

                        this.IsBusy = false;
                    }).Start();
                }), delegate
                {
                    return !this.IsBusy;
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

        public ICommand AddCode
        {
            get
            {
                return new RelayCommand<object>(new Action<object>(delegate (object group)
                {
                    if (string.Empty == this.BarCode.Trim()) return;
                    if (Remarks.Trim().ToUpper().Contains("IV") || Remarks.Trim().ToUpper().Contains("XRAY"))
                    {
                        new Thread(() =>
                        {
                            Thread.CurrentThread.IsBackground = true;
                            this.IsBusy = true;

                            try
                            {
                                BatteryCheckIF.AddTestCode(new TestCode()
                                {
                                    BarCode = this.BarCode.Trim(),
                                    CreateBy = "tttttttt",
                                    CreateTime = DateTime.Now,
                                    Remarks = this.Remarks
                                });
                                this.CurPageNum = 1;
                            }
                            catch (System.Exception ex)
                            {
                                LoggingIF.Log(string.Format("Failed to add test code {0}", ex.Message.ToString()), LogLevels.Error, "TestCodesVm");

                                System.Windows.MessageBox.Show("错误\r\n" + ex.Message.ToString());
                            }

                            this.IsBusy = false;
                        }).Start();
                    }
                    else
                    {
                        System.Windows.MessageBox.Show("Master类型必须填写包含“IV”或“XRAY”的字符以区分是哪种类型的Master");
                    }
                }), delegate
                {
                    return !this.IsBusy;
                });
            }
        }

        public ICommand SaveConfig
        {
            get
            {
                return new RelayCommand(new Action(delegate
                {
                    MyCheckParamsConfig.SaveStartupConfig();
                    MyCheckParamsConfig.WriteGeneralParams();
                }), delegate
                {
                    return !this.IsBusy;
                });
            }
        }
    }
}
