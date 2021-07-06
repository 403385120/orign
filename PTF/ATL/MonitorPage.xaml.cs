using System;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using ATL.Station;
using ATL.UI.MES;
using System.Windows.Forms;
using ATL.Core;

namespace PTF
{
    /// <summary>
    /// HistoryAlarmListPage.xaml 的交互逻辑
    /// </summary>
    public partial class MonitorPage : Window
    {
        public MonitorPage()
        {
            InitializeComponent();
            this.DataContext = Station.Current;
        }

        public delegate void SendMessage(string value);
        public SendMessage sendMessage;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.DataContext = Station.Current;
            ThreadPool.QueueUserWorkItem(DoWork);
            Uri uri = new Uri(UserDefineVariableInfo.DicVariables["DefaultMesUrl"].ToString());

            ExtendedWebBrowser webBrowser1 = new ExtendedWebBrowser();
            webBrowser1.Dock = DockStyle.Fill;

            WebControl.Child = webBrowser1;
            webBrowser1.BeforeNewWindow += new EventHandler<WebBrowserExtendedNavigatingEventArgs>(webBrowser1_BeforeNewWindow);
            webBrowser1.Url = uri;
            webBrowser1.ScriptErrorsSuppressed = true;//禁止弹出脚本错误
        }
        void webBrowser1_BeforeNewWindow(object sender, WebBrowserExtendedNavigatingEventArgs e)
        {
            e.Cancel = true;
            ((ExtendedWebBrowser)sender).Navigate(e.Url);
            txtURL.Text = e.Url;
        }

        private void btnGo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Uri uri = new Uri(txtURL.Text);
                ExtendedWebBrowser webBrowser1 = new ExtendedWebBrowser();

                this.WebControl.Child=(webBrowser1);
                webBrowser1.BeforeNewWindow += new EventHandler<WebBrowserExtendedNavigatingEventArgs>(webBrowser1_BeforeNewWindow);
                webBrowser1.Url = uri;
                webBrowser1.ScriptErrorsSuppressed = true;//禁止弹出脚本错误
            }
            catch
            {
                Xceed.Wpf.Toolkit.MessageBox.Show("链接地址错误");
            }
        }

        private void MesPage_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sendMessage != null)
                sendMessage("pack://application:,,,/ATL.UI;component/MES/MesWebPage.xaml");
            refreshUI = false;
            this.Close();
        }

        private void btnMin_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnMax_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            refreshUI = false;
            this.Close();
        }
        private bool refreshUI;
        private void DoWork(object stateInfo)
        {
            refreshUI = true;
            while (refreshUI)
            {
                DoCycleWork();
            }
        }
        
        public void DoCycleWork()
        {
            Thread.Sleep(1500);
            try
            {
                Dispatcher.BeginInvoke((Action)delegate ()
                {
                    //this.Topmost = true;
                    //if (MaterialInfo.ItemsSource != Device_FEF.lstMaterial)
                    //{
                    //    if (MaterialInfo.ItemsSource != null)
                    //        MaterialInfo.ItemsSource = null;
                    //    MaterialInfo.ItemsSource = Device_FEF.lstMaterial;
                    //}
                });
            }
            catch (Exception ex)
            {

            }
        }
    }
}
