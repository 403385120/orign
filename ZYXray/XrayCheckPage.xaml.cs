using System;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using ATL.UI.Core;
using ATL.Station;
using System.Threading.Tasks;
using ZYXray.ViewModels;

namespace ZYXray
{
    /// <summary>
    /// XrayCheckPage.xaml 的交互逻辑
    /// </summary>
    public partial class XrayCheckPage : Window
    {
        public XrayCheckPage()
        {
            InitializeComponent();
        }
        //public delegate void SendMessage(string value);
        //public SendMessage sendMessage;
        ManualRecheckPage manualRecheckPage = new ManualRecheckPage();

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.ManualRecheckPage1.Navigate(manualRecheckPage);

            if(null != manualRecheckPage)
            {
                ((ManualRecheckVm)manualRecheckPage.DataContext).FullScreenToggleHandlers += (isFullScreen) =>
                {
                    if (isFullScreen)
                    {
                        this.Close();
                    }
                    else
                    {
                        this.WindowState = WindowState.Minimized;
                    }
                };
            }
        }
    }
}
