using ATL.Station;
using ATL.UI.Controls;
using ATL.UI.Core;
using System;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Markup;

namespace PTF
{
    /// <summary>
    /// HistoryAlarmListPage.xaml 的交互逻辑
    /// </summary>
    public partial class MonitorDataCapacityStatistics : BasePage, IComponentConnector
    {
        public MonitorDataCapacityStatistics()
        {
            InitializeComponent();
        }
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            this.DataContext = Station.Current;
        }
    }
}
