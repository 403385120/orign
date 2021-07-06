using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Shuyz.Framework.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ZYXray.Utils
{
    public delegate void ShutdownEventHandler();

    public class WindowController
    {
        public ShutdownEventHandler ShutDownEventHandlers = null;

        private static WindowController _instance = new WindowController();
        public static WindowController Instance
        {
            get { return _instance; }
        }

        private WindowController()
        {

        }

        public ICommand MinimizeWindow
        {
            get
            {
                return new RelayCommand(new Action(delegate
                {
                    Application.Current.MainWindow.WindowState = WindowState.Minimized;
                }), delegate
                {
                    return true;
                });
            }
        }

        public ICommand MaximizeWindow
        {
            get
            {
                return new RelayCommand(new Action(delegate
                {
                    if(Application.Current.MainWindow.WindowState != WindowState.Maximized)
                    {
                        Application.Current.MainWindow.WindowState = WindowState.Maximized;
                    }
                    else
                    {
                        Application.Current.MainWindow.WindowState = WindowState.Normal;
                    }
                }), delegate
                {
                    return true;
                });
            }
        }

        public ICommand ExitApplication
        {
            get
            {
                var mySettings = new MetroDialogSettings()
                {
                    AffirmativeButtonText = "退出",
                    NegativeButtonText = "取消",
                    AnimateShow = true,
                    AnimateHide = false
                };

                return new RelayCommand(new Action(delegate
                {
                    MessageDialogResult dialogResult = ((MetroWindow)Application.Current.MainWindow).ShowModalMessageExternal(
                        "提示", "是否退出系统?",
                                                 MessageDialogStyle.AffirmativeAndNegative, mySettings);

                    if (MessageDialogResult.Affirmative == dialogResult)
                    {
                        if(null != this.ShutDownEventHandlers)
                        {
                            try
                            {
                                this.ShutDownEventHandlers();
                            }
                            catch { }
                        }

                        Application.Current.Shutdown();
                    }
                }), delegate
                {
                    return true;
                });
            }
        }

    }
}
