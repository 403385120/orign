using Shuyz.Framework.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using ZYXray.Utils;

namespace ZYXray.ViewModels
{
    public class DataStressStateVm : ObservableObject
    {
        public FullScreenToggleHandler FullScreenToggleHandlers = null;
        private static DataStressStateVm _instance = new DataStressStateVm();
        private AuthBoxVm _authBoxVm = new AuthBoxVm();

        public DataStressStateVm()
        {
            // 登陆关闭
            this._authBoxVm.MyLoginEventHandler += delegate (bool isNotWaitCheck)
            {
                //登录成功关闭页面
                Application.Current.Dispatcher.Invoke((Action)(() =>
                {
                    ZY.Logging.LoggingIF.Log("用户：" + this._authBoxVm.UserName + " 登录", ZY.Logging.LogLevels.Info, "DataStressStateVm");
                    if (null != FullScreenToggleHandlers)
                    {
                        try
                        {
                            this.FullScreenToggleHandlers(true);
                        }
                        catch { }
                    }
                    //this.ToggleFullScreen.Execute(null);
                }));
            };
        }

        public static DataStressStateVm Instance
        {
            get { return _instance; }
        }

        public AuthBoxVm MyAuthBoxVm
        {
            get { return this._authBoxVm; }
            set
            {
                this._authBoxVm = value;
                RaisePropertyChanged("MyAuthBoxVm");
            }
        }

        private bool _isAuthVisible = true;
        private bool _isBusy = false;
        private int _curTime = 60;
        private string _msg = "";
        private string _msgtime = "";

        public bool IsBusy
        {
            get { return _isBusy; }

            set
            {
                _isBusy = value;
                RaisePropertyChanged("IsBusy");
            }
        }

        public int CurTime
        {
            get { return _curTime; }

            set
            {
                _curTime = value;
                RaisePropertyChanged("CurTime");
            }
        }

        public string WarnMsg
        {
            get { return _msg; }

            set
            {
                _msg = value;
                RaisePropertyChanged("WarnMsg");
            }
        }

        public string MessageAndTime
        {
            get { return _msgtime; }

            set
            {
                _msgtime = value;
                RaisePropertyChanged("MessageAndTime");
            }
        }

        public bool IsAuthVisible
        {
            get { return _isAuthVisible; }

            set
            {
                _isAuthVisible = value;
                RaisePropertyChanged("IsAuthVisible");
            }
        }

        public ICommand ToggleFullScreen
        {
            get
            {
                return new RelayCommand(new Action(delegate
                {

                    // Call 外部逻辑以便本页面显示最前端
                    if (null != FullScreenToggleHandlers)
                    {
                        try
                        {
                            this.FullScreenToggleHandlers(true);
                        }
                        catch { }
                    }

                }), delegate
                {
                    return !this.IsBusy;
                });
            }
        }
    }
}
