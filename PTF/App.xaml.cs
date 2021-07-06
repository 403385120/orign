using ATL.Common;
using ATL.MES;
using ATL.UI;
using ATL.UI.Core;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace PTF
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        static App()
        {
            if (Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName).Count() > 1)
            {
                System.Windows.Forms.MessageBox.Show("程序已经启动 ");
                Application.Current.Shutdown();
            }
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            LogInDB.Info("软件启动");
            base.OnStartup(e);
            System.Windows.Forms.Application.EnableVisualStyles();
            DispatcherUnhandledException += new DispatcherUnhandledExceptionEventHandler(this.Current_DispatcherUnhandledException);
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(this.CurrentDomain_UnhandledException);

            //StartupUri = new Uri("pack://application:,,,/ATL.UI;component/LoginWindow.xaml", UriKind.Absolute);
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
            if(loginWindow.ValidateOK)
            {
                Current.MainWindow = PTF.MainWindow.Current;
                PTF.MainWindow.Current.Show();
                Task.Run(() => {
                    //InterfaceClient.Current = new InterfaceClient("127.0.0.1", 50001, 50002, 60000, "127.0.0.1", 50000);
                    InterfaceClient.Current = new InterfaceClient();
                });
                
                loginWindow.Close();
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            LogInDB.Error($"系统捕获Error：" + e.ExceptionObject.ToString());
            //MessageBox.Show(e.ExceptionObject.ToString());
            //MessageBox.Show("UnhandledException我们很抱歉，当前应用程序遇到一些问题，该操作已经终止，请进行重试，如果问题继续存在，请联系管理员.", "意外的操作", MessageBoxButton.OK, MessageBoxImage.Asterisk);
        }

        private void Current_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            LogInDB.Error($"系统捕获Error：" + e.Exception.Message);
            MessageBox.Show(e.Exception.Message);
            MessageBox.Show("DispatcherUnhandledException我们很抱歉，当前应用程序遇到一些问题，该操作已经终止，请进行重试，如果问题继续存在，请联系管理员.", "意外的操作", MessageBoxButton.OK, MessageBoxImage.Asterisk);
            e.Handled = true;
        }
    }
}
