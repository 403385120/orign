using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ZYXray.ViewModels;

namespace ZYXray
{
    /// <summary>
    /// StressStatePage.xaml 的交互逻辑
    /// </summary>
    public partial class StressStatePage : Window
    {
        DataStressStatePage dataStressStatePage = new DataStressStatePage();

        public StressStatePage(string msg, int showtime = 60)
        {
            dataStressStatePage.MyDataStressVm.WarnMsg = msg;
            dataStressStatePage.MyDataStressVm.CurTime = showtime;
            dataStressStatePage.MyDataStressVm.MessageAndTime = msg + " (等待 " + showtime + " s后关闭)";
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.ManualDataStress.Navigate(dataStressStatePage);

            if (null != dataStressStatePage)
            {
                ((DataStressStateVm)dataStressStatePage.DataContext).FullScreenToggleHandlers += (isFullScreen) =>
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
