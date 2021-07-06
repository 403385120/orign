using ATL.UI.Controls;
using ATL.UI.Core;
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
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PTF
{
    /// <summary>
    /// DataCapacityTracing.xaml 的交互逻辑
    /// </summary>
    public partial class DataCapacityTracing : BasePage, IComponentConnector
    {
        public DataCapacityTracing()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            DateTimeStart.Value = DateTime.Now.AddDays(-1);
            DateTimeEnd.Value = DateTime.Now;
            RefreshData();
        }

        private void DateTimeStart_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (DateTimeStart.Value > DateTimeEnd.Value)
            {
                DateTime temp = (DateTime)DateTimeEnd.Value;
                DateTimeEnd.Value = DateTimeStart.Value;
                DateTimeStart.Value = temp;
            }
        }

        private void DateTimeEnd_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (DateTimeStart.Value > DateTimeEnd.Value)
            {
                DateTime temp = (DateTime)DateTimeEnd.Value;
                DateTimeEnd.Value = DateTimeStart.Value;
                DateTimeStart.Value = temp;
            }
        }

        private void btnQuery_Click(object sender, RoutedEventArgs e)
        {
            RefreshData();
        }

        public void RefreshData()
        {
            
        }
    }
}
