using ATL.Common;
using ATL.MES;
using ATL.UI;
using ATL.UI.Core;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms.Integration;
using System.Windows.Threading;

namespace PTF
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {

        /// <summary>
		/// mutex互斥锁
		/// </summary>
		private static Mutex mutex = null;

        protected override void OnStartup(StartupEventArgs e)
        {
            bool createdNew = false;
            try
            {
                mutex = new System.Threading.Mutex(false, "RunOnce", out createdNew);
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                Thread.Sleep(1000);
                Environment.Exit(1);
            }
            if (!createdNew)
            {
                if (ATL.Common.StringResources.IsDefaultLanguage)
                {
                    LogInDB.Info("程序已经启动");
                    MessageBox.Show("程序已经启动");
                }
                else
                {
                    LogInDB.Info("The program has started ");
                    MessageBox.Show("The program has started ");
                }
                Thread.Sleep(1000);
                Environment.Exit(1);
                return;
            }

            //LogInDB.Info("软件启动");
            if (ATL.Common.StringResources.IsDefaultLanguage)
            {
                LogInDB.Info("软件启动");
            }
            else
            {
                LogInDB.Info("Software startup");
            }
            base.OnStartup(e);
            System.Windows.Forms.Application.EnableVisualStyles();
            DispatcherUnhandledException += new DispatcherUnhandledExceptionEventHandler(this.Current_DispatcherUnhandledException);
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(this.CurrentDomain_UnhandledException);

            //StartupUri = new Uri("pack://application:,,,/ATL.UI;component/LoginWindow.xaml", UriKind.Absolute);
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
            if (loginWindow.ValidateOK)
            {
                Current.MainWindow = PTF.MainWindow.Current;
                ElementHost.EnableModelessKeyboardInterop(Current.MainWindow);
                WindowsFormsHost.EnableWindowsFormsInterop();
                PTF.MainWindow.Current.Show();
                Task.Run(() => {
                    //InterfaceClient.Current = new InterfaceClient("127.0.0.1", 50001, 50002, 60000, "127.0.0.1", 50000);
                    InterfaceClient.Current = new InterfaceClient();
                });

                loginWindow.Close();
            }
            UpdateLanguage();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            //关闭所有线程，即关闭此进程
            System.Environment.Exit(0);
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            //LogInDB.Error($"系统捕获Error：" + e.ExceptionObject.ToString());
            if (ATL.Common.StringResources.IsDefaultLanguage)
            {
                LogInDB.Error($"系统捕获Error：" + e.ExceptionObject.ToString());
            }
            else
            {
                LogInDB.Error($"System capture Error ：" + e.ExceptionObject.ToString());
            }
            //MessageBox.Show(e.ExceptionObject.ToString());
            //MessageBox.Show("UnhandledException我们很抱歉，当前应用程序遇到一些问题，该操作已经终止，请进行重试，如果问题继续存在，请联系管理员.", "意外的操作", MessageBoxButton.OK, MessageBoxImage.Asterisk);
        }

        private void Current_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            //LogInDB.Error($"系统捕获Error：" + e.Exception.Message);
            if (ATL.Common.StringResources.IsDefaultLanguage)
            {
                LogInDB.Error($"系统捕获Error：" + e.Exception.StackTrace);
            }
            else
            {
                LogInDB.Error($"System capture Error：" + e.Exception.StackTrace);
            }
            MessageBox.Show(e.Exception.Message);
            //MessageBox.Show("DispatcherUnhandledException我们很抱歉，当前应用程序遇到一些问题，该操作已经终止，请进行重试，如果问题继续存在，请联系管理员.", "意外的操作", MessageBoxButton.OK, MessageBoxImage.Asterisk);
            if (ATL.Common.StringResources.IsDefaultLanguage)
            {
                MessageBox.Show("DispatcherUnhandledException我们很抱歉，当前应用程序遇到一些问题，该操作已经终止，请进行重试，如果问题继续存在，请联系管理员.", "意外的操作", MessageBoxButton.OK, MessageBoxImage.Asterisk);
            }
            else
            {
                MessageBox.Show("DispatcherUnhandledException,We are sorry, the current application encountered some problems, the operation has been terminated, please try again, if the problem continues, please contact the administrator .", "Unexpected operation ", MessageBoxButton.OK, MessageBoxImage.Asterisk);
            }
            e.Handled = true;
        }

        #region Method
        /// <summary>
        /// 更换语言包
        /// </summary>
        public void UpdateLanguage()
        {
            CultureInfo currentCultureInfo = CultureInfo.CurrentCulture;
            List<ResourceDictionary> dictionaryList = new List<ResourceDictionary>();
            foreach (ResourceDictionary dictionary in Application.Current.Resources.MergedDictionaries)
            {
                dictionaryList.Add(dictionary);
            }
            string requestedLanguage = string.Format(@"Resources/{0}.xaml", currentCultureInfo.Name);
            ResourceDictionary resourceDictionary = dictionaryList.FirstOrDefault(d => d.Source.OriginalString.Equals(requestedLanguage));
            if (resourceDictionary == null)
            {
                requestedLanguage = @"Resources/en-US.xaml";
                resourceDictionary = dictionaryList.FirstOrDefault(d => d.Source.OriginalString.Equals(requestedLanguage));
            }
            if (resourceDictionary != null)
            {
                Application.Current.Resources.MergedDictionaries.Remove(resourceDictionary);
                Application.Current.Resources.MergedDictionaries.Add(resourceDictionary);
            }
        }
        #endregion
    }
}
