using ATL.UI.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Markup;
using System.Windows.Threading;

namespace PTF
{
    /// <summary>
    /// HistoryAlarmListPage.xaml 的交互逻辑
    /// </summary>
    public partial class DeviceDebug : BasePage, IComponentConnector
    {
        public DeviceDebug()
        {
            InitializeComponent();
        }
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void CarrierScanner_Click(object sender, RoutedEventArgs e)
        {
            if(ModVariable.frmCarrierBarcode != null)
            {
                ModVariable.frmCarrierBarcode.Show();

            }
                
        }
        

        private void CellScanner_Click(object sender, RoutedEventArgs e)
        {
            if (ModVariable.frmCellBarcode != null)
                ModVariable.frmCellBarcode.Show();
        }
    }
}
