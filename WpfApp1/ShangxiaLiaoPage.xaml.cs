using ATL.UI.Controls;
using DeviceLib;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApp1
{
    /// <summary>
    /// ShangxiaLiaoPage.xaml 的交互逻辑
    /// </summary>
    public partial class ShangxiaLiaoPage : BasePage
    {
        public ShangxiaLiaoPage()
        {
            InitializeComponent();
        }


        private void shangliaobn_Click(object sender, RoutedEventArgs e)
        {
            search(1);
        }
        private void setColumns1()
        {
            this.datagrid.Columns[0].Header = "小车编号";
            this.datagrid.Columns[1].Header = "电芯二维码";
            this.datagrid.Columns[2].Header = "电芯状态";
            this.datagrid.Columns[3].Header = "上料时间";
            this.datagrid.Columns[4].Header = "下料时间";
            this.datagrid.Columns[5].Header = "层";
            this.datagrid.Columns[6].Header = "行";
            this.datagrid.Columns[7].Header = "列";
            this.datagrid.Columns[8].Header = "烘烤状态";
            this.datagrid.Columns[9].Header = "烘烤次数";
        
            this.datagrid.IsReadOnly = true;
        }


        private void xialiaobn_Click(object sender, RoutedEventArgs e)
        {
            search(2);
        }

        private void search(int type)
        {

            List<DeviceProcess.SXChip> chips = ModVariable.deviceOverView.getShangXialiao(type, this.start.Text.ToString(), this.end.Text.ToString());
            if (chips.Count == 0)
            {
                this.datagrid.ItemsSource = null;
                MessageBox.Show("目前没有数据！");
                return;
            }

            this.datagrid.ItemsSource = chips;

            setColumns1();
        }
    }
}
