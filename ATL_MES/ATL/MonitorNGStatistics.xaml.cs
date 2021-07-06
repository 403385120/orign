using ATL.Station;
using ATL.UI.Controls;
using System.Windows;
using System.Windows.Markup;

namespace PTF
{
    /// <summary>
    /// HistoryAlarmListPage.xaml 的交互逻辑
    /// </summary>
    public partial class MonitorNGStatistics : BasePage, IComponentConnector
    {
        public MonitorNGStatistics()
        {
            InitializeComponent();
        }
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            this.DataContext = Station.Current;
        }

    }
}
