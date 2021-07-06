using ATL.Common;
using ATL.UI.Controls;
using DeviceLib;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic;
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
    /// SearchPage.xaml 的交互逻辑
    /// </summary>
    public partial class SearchPage : BasePage
    {
        public class CategoryInfo
        {
            public string Name { get; set; }
            public string Value { get; set; }
        }
        List<CategoryInfo> categoryList = new List<CategoryInfo>();
        List<CategoryInfo> categoryList1 = new List<CategoryInfo>();
        public SearchPage()
        {
            InitializeComponent();
        }

        private void BasePage_Loaded(object sender, RoutedEventArgs e)
        {
            categoryList.Add(new CategoryInfo { Name = "1", Value = "1" });
            categoryList.Add(new CategoryInfo { Name = "2", Value = "2" });
            categoryList.Add(new CategoryInfo { Name = "3", Value = "3" });
            categoryList.Add(new CategoryInfo { Name = "4", Value = "4" });
            categoryList.Add(new CategoryInfo { Name = "5", Value = "5" });
            categoryList.Add(new CategoryInfo { Name = "6", Value = "6" });
            categoryList.Add(new CategoryInfo { Name = "7", Value = "7" });
            categoryList.Add(new CategoryInfo { Name = "8", Value = "8" });
            categoryList.Add(new CategoryInfo { Name = "9", Value = "9" });
            categoryList.Add(new CategoryInfo { Name = "10", Value = "10" });
            this.carid.ItemsSource = categoryList;
            this.carid.DisplayMemberPath = "Name";//显示出来的值
            this.carid.SelectedValuePath = "Value";//实际选中后获取的结果的值
            this.carid.SelectedIndex = 0;

            categoryList1.Add(new CategoryInfo { Name = "1", Value = "1" });
            categoryList1.Add(new CategoryInfo { Name = "2", Value = "2" });
            categoryList1.Add(new CategoryInfo { Name = "3", Value = "3" });
            categoryList1.Add(new CategoryInfo { Name = "4", Value = "4" });
            categoryList1.Add(new CategoryInfo { Name = "5", Value = "5" });
            categoryList1.Add(new CategoryInfo { Name = "6", Value = "6" });
            categoryList1.Add(new CategoryInfo { Name = "7", Value = "7" });

            this.oval.ItemsSource = categoryList1;
            this.oval.DisplayMemberPath = "Name";//显示出来的值
            this.oval.SelectedValuePath = "Value";//实际选中后获取的结果的值
            this.oval.SelectedIndex = 0;
        }

       
       
        //电芯查询
        private void Button_Click(object sender, RoutedEventArgs e)
        {
          
            List<DeviceProcess.Chip> chips= ModVariable.deviceOverView.SearchCode(this.codestr.Text.ToString());
            this.datagrid.ItemsSource = chips;
            setColumns1();
        }

        private void setColumns1()
        {
            this.datagrid.Columns[0].Header = "电芯";
            this.datagrid.Columns[1].Header = "车";
            this.datagrid.Columns[2].Header = "炉";
            this.datagrid.Columns[3].Header = "层";
            this.datagrid.Columns[4].Header = "行";
            this.datagrid.Columns[5].Header = "列";
            this.datagrid.Columns[6].Header = "上料";
            this.datagrid.Columns[7].Header = "进炉";
            this.datagrid.Columns[8].Header = "烘烤开始";
            this.datagrid.Columns[9].Header = "烘烤结束";
            this.datagrid.Columns[10].Header = "出炉";
            this.datagrid.Columns[11].Header = "下料";
            this.datagrid.Columns[12].Header = "温度";
            this.datagrid.Columns[13].Header = "真空";
            this.datagrid.Columns[14].Header = "状态";
            this.datagrid.Columns[15].Header = "次数";
            this.datagrid.Columns[16].Visibility = Visibility.Hidden;
            this.datagrid.IsReadOnly = true;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            List<DeviceProcess.Chip> chips = ModVariable.deviceOverView.SearchCar(this.carid.SelectedValue.ToString(),
                this.car_start.Text.ToString(), this.car_end.Text.ToString());
            this.datagrid.ItemsSource = chips;  
            setColumns2();
        }
       

        private void setColumns2()
        {
            this.datagrid.Columns[0].Visibility = Visibility.Hidden;
            this.datagrid.Columns[1].Header = "车";
            this.datagrid.Columns[2].Header = "炉";
            this.datagrid.Columns[3].Visibility = Visibility.Hidden;
            this.datagrid.Columns[4].Visibility = Visibility.Hidden;
            this.datagrid.Columns[5].Visibility = Visibility.Hidden;
            this.datagrid.Columns[6].Header = "上料时间";
            this.datagrid.Columns[7].Header = "进炉时间";
            this.datagrid.Columns[8].Header = "烘烤开始时间";
            this.datagrid.Columns[9].Header = "烘烤结束时间";
            this.datagrid.Columns[10].Header = "出炉时间";
            this.datagrid.Columns[11].Header = "下料时间";
            this.datagrid.Columns[12].Visibility = Visibility.Hidden;
            this.datagrid.Columns[13].Visibility = Visibility.Hidden;
            this.datagrid.Columns[14].Visibility = Visibility.Hidden;
            this.datagrid.Columns[15].Visibility = Visibility.Hidden;
            this.datagrid.Columns[16].Visibility = Visibility.Hidden;
            this.datagrid.IsReadOnly = true;
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            List<DeviceProcess.Chip> chips = ModVariable.deviceOverView.SearchOval(this.oval.SelectedValue.ToString(),
               this.oval_start.Text.ToString(), this.oval_end.Text.ToString());
            this.datagrid.ItemsSource = chips;
            setColumns2();
        }
    }
}
