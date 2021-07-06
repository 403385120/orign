using ATL.UI.Controls;
using DeviceLib;
using System;
using System.Collections.Generic;
using System.Data;
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
    /// DeviceInitPage.xaml 的交互逻辑
    /// </summary>
    public partial class DeviceInitPage : BasePage
    {
        public DeviceInitPage()
        {
            InitializeComponent();
        }
        DataTable data;
        private void BasePage_Loaded(object sender, RoutedEventArgs e)
        {
            initUI();
            //data = ModVariable.deviceOverView.getAllCarNumToPos();
            //this.datagrid.ItemsSource = data.DefaultView;
            //this.datagrid.Columns[0].Width = 100;
            //this.datagrid.Columns[1].Width = 200;
            //this.datagrid.Columns[2].Width = 100;
            //this.datagrid.Columns[3].Width = 150;
            //this.datagrid.Columns[4].Width = 150;
            //this.datagrid.Columns[0].IsReadOnly = true;
            //this.datagrid.Columns[1].IsReadOnly = true;
            //this.datagrid.Columns[0].Header = "序号";
            //this.datagrid.Columns[1].Header = "小车二维码";
            //this.datagrid.Columns[2].Header = "位置";
            //this.datagrid.Columns[3].Header = "电芯数量";
            //this.datagrid.Columns[4].Header = "状态";


        }
        private void initUI()
        {
            data = ModVariable.deviceOverView.getAllCarNumToPos();
            this.datagrid.ItemsSource = data.DefaultView;
            this.datagrid.Columns[0].Width = 100;
            this.datagrid.Columns[1].Width = 200;
            this.datagrid.Columns[2].Width = 100;
            this.datagrid.Columns[3].Width = 150;
            this.datagrid.Columns[4].Width = 150;
            this.datagrid.Columns[0].IsReadOnly = true;
            this.datagrid.Columns[1].IsReadOnly = true;
            this.datagrid.Columns[0].Header = "序号";
            this.datagrid.Columns[1].Header = "小车二维码";
            this.datagrid.Columns[2].Header = "位置";
            this.datagrid.Columns[3].Header = "电芯数量";
            this.datagrid.Columns[4].Header = "状态";
        }
       
        DataRowView lastselect = null;
        int pos, cellnum, carstate;

        private void datagrid_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            //DataRowView lastselect = this.datagrid.SelectedItem as DataRowView;
            //pos = Int16.Parse(lastselect.Row[2].ToString());
            //cellnum = Int16.Parse(lastselect.Row[3].ToString());
            //carstate = Int16.Parse(lastselect.Row[4].ToString());
            //MessageBox.Show("eidt: "+pos + "," + cellnum + "," + carstate);
        }

        private void datagrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lastselect != null)
            {
                //保存数据
                int pos_cur= Int16.Parse(lastselect.Row[2].ToString());
                int cellnum_cur= Int16.Parse(lastselect.Row[3].ToString());
                int carstate_cur= Int16.Parse(lastselect.Row[4].ToString());
                if(carstate_cur!=10&& carstate_cur != 20&& carstate_cur != 30&& carstate_cur != 40&& carstate_cur != 60)
                {
                    MessageBox.Show("请输入正确的状态");
                    lastselect = null;
                    initUI();              
                    return;
                }
               //如果没有发生任何变化 则不用修改  保存本次选择内容 退出即可
                if (pos == pos_cur && cellnum == cellnum_cur && carstate == carstate_cur)
                {                   
                    lastselect = this.datagrid.SelectedItem as DataRowView;
                    if (lastselect != null)
                    {
                        pos = Int16.Parse(lastselect.Row[2].ToString());
                        cellnum = Int16.Parse(lastselect.Row[3].ToString());
                        carstate = Int16.Parse(lastselect.Row[4].ToString());
                    }
                    return;
                }
                DeviceProcess.CarNumToPos car = new DeviceProcess.CarNumToPos();
                car.CarID=Int16.Parse(lastselect.Row[0].ToString());
                car.Pos = pos_cur;
                car.CellNum = cellnum_cur;
                car.CarState = carstate_cur;
                int res = ModVariable.deviceOverView.UpdateCarNumToPos(car);

                if (res == 1)
                    MessageBox.Show("修改完毕");

            }
            lastselect = this.datagrid.SelectedItem as DataRowView;
            if (lastselect != null)
            {
                pos = Int16.Parse(lastselect.Row[2].ToString());
                cellnum = Int16.Parse(lastselect.Row[3].ToString());
                carstate = Int16.Parse(lastselect.Row[4].ToString());
            }
          
        }

      
    }
}
