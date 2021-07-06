using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shuyz.Framework.Mvvm;
using System.Windows.Input;
using System.Threading;
using System.Windows;
using MahApps.Metro.Controls.Dialogs;
using MahApps.Metro.Controls;
using ATL.Core;
using ATL.MES;
using ZY.Logging;
using System.Data;
using ATL.Common;

namespace ZYXray.ViewModels
{
    public delegate void AuthLoginEventHandler(bool isNotWaitCheck);


    public class AuthBoxVm : ObservableObject
    {
        private string _userName = string.Empty;
        private string _userPass = string.Empty;
        private string _stfUserName = string.Empty;
        private string _stfUserPass = string.Empty;
        private string _fqa_LocalPassWord = string.Empty;
        private string _messageShow = string.Empty;


        private bool _isBusy = false;
        private bool _isNotWaitCheck = false;

        public static BaseFacade baseFacade = new BaseFacade();
        public AuthLoginEventHandler MyLoginEventHandler = null;
        public AuthLoginEventHandler MyExitEventHandler = null;

        private bool _needLocalLogin = true;

        public bool NeedLocalLogin
        {
            get { return this._needLocalLogin; }
            set
            {
                this._needLocalLogin = value;
                RaisePropertyChanged("NeedLocalLogin");
            }
        }

        public string UserName
        {
            get { return this._userName; }
            set
            {
                this._userName = value;
                RaisePropertyChanged("UserName");
            }
        }

        public string UserPass
        {
            get { return this._userPass; }
            set
            {
                this._userPass = value;
                RaisePropertyChanged("UserPass");
            }
        }

        public string STFUserName
        {
            get { return this._stfUserName; }
            set
            {
                this._stfUserName = value;
                RaisePropertyChanged("STFUserName");
            }
        }

        public string STFUserPass
        {
            get { return this._stfUserPass; }
            set
            {
                this._stfUserPass = value;
                RaisePropertyChanged("STFUserPass");
            }
        }

        public string FQA_LocalPassWord
        {
            get { return this._fqa_LocalPassWord; }
            set
            {
                this._fqa_LocalPassWord = value;
                RaisePropertyChanged("FQA_LocalPassWord");
            }
        }

        public string MessageShow
        {
            get { return this._messageShow; }
            set
            {
                this._messageShow = value;
                RaisePropertyChanged("MessageShow");
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

        public bool IsNotWaitCheck
        {
            get { return this._isNotWaitCheck; }
            set
            {
                this._isNotWaitCheck = value;
                RaisePropertyChanged("IsNotWaitCheck");
            }
        }

        public ICommand Auth
        {
            get
            {
                return new RelayCommand<object>(new Action<object>(delegate (object param)
                {
                    if (this._isBusy) return;
                    this.IsBusy = true;

                    new Thread(() =>
                    {
                        Thread.CurrentThread.IsBackground = true;

                        var values = (object[])param;

                        string passWord = (values[0] as System.Windows.Controls.PasswordBox).Password;
                        this.UserPass = passWord;

                        int mode = int.Parse(UserDefineVariableInfo.DicVariables["IsRecheckMode"].ToString());

                        LoggingIF.Log("UserID: " + UserName + " Password: " + UserPass, LogLevels.Info, "AuthBoxVm");

                        if (InterfaceClient.Current == null)
                        {
                            LoggingIF.Log("Login fail", LogLevels.Info, "AuthBoxVm");

                            MessageShow = "MES未连接上!";
                            this.IsBusy = false;
                            return;
                        }

                        ATL.MES.A040.Root root = InterfaceClient.Current.A039_New(UserName, UserPass, "2", "");
                        if (root == null || root.ResponseInfo.UserLevel == "" || root.ResponseInfo.UserLevel == "0")
                        {
                            LoggingIF.Log("Login fail", LogLevels.Info, "AuthBoxVm");

                            MessageShow = "登录失败!";
                            this.IsBusy = false;
                            return;
                        }
                        else
                        {
                            if (mode == 0)//判断FQA权限
                            {
                                string sql = "select userName from users where userName='" + UserName + "'";
                                DataSet ds = baseFacade.equipDB.ExecuteDataSet(CommandType.Text, sql);
                                if (ds.Tables[0].Rows.Count < 1)
                                {
                                    MessageShow = "没有FQA权限,登录失败!";
                                    this.IsBusy = false;
                                    return;
                                }
                            }
                        }

                        if (null != this.MyLoginEventHandler)
                        {
                            this.MyLoginEventHandler(IsNotWaitCheck);
                        }

                        this.IsBusy = false;
                    }).Start();

                }), delegate
                {
                    return !this.IsBusy;
                });
            }
        }

        public ICommand Exit
        {
            get
            {
                return new RelayCommand(new Action(delegate
                {
                    if (null != this.MyExitEventHandler)
                    {
                        this.MyExitEventHandler(IsNotWaitCheck);
                    }
                }), delegate
                {
                    return !this.IsBusy;
                });
            }
        }
    }
}
