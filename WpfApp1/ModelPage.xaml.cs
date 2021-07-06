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
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace WpfApp1
{
    /// <summary>
    /// ModelPage.xaml 的交互逻辑
    /// </summary>
    public partial class ModelPage : BasePage
    {
        List<DeviceProcess.ChipModel> models;
        public ModelPage()
        {
            InitializeComponent();
           
        }
        public class CategoryInfo
        {
            public string Name { get; set; }
            public string Value { get; set; }
        }
        List<CategoryInfo> categoryList = new List<CategoryInfo>();
        private void BasePage_Loaded(object sender, RoutedEventArgs e)
        {

            updateUI();
        }

        private void updateUI()
        {
            models = ModVariable.deviceOverView.GetChipModels();
            categoryList.Clear();
            int id= ModVariable.deviceOverView.getModel();
           // MessageBox.Show("id=" + id);
            int index =0;
            foreach (DeviceProcess.ChipModel m in models)
            {
                if (m.id.Equals(id + ""))
                {
                    categoryList.Add(new CategoryInfo { Name = m.name + "*", Value = m.id });
                    index = categoryList.Count - 1;
                }else
                    categoryList.Add(new CategoryInfo { Name = m.name , Value = m.id });

            }
            if (categoryList.Count > 0)
            {

                this.modellist.ItemsSource = null;
                this.modellist.ItemsSource = categoryList;
                this.modellist.DisplayMemberPath = "Name";//显示出来的值
                this.modellist.SelectedValuePath = "Value";//实际选中后获取的结果的值
                this.modellist.SelectedIndex = index;
                this.choosebn.IsEnabled = false;


            }
            else
            {
                this.modellist.ItemsSource = null;
                this.choosebn.IsEnabled = false;
                clearModel();
            }
        }
        private bool check()
        {
            try
            {
                int w = int.Parse(this.width.Text);
                int h = int.Parse(this.height.Text);
                int hou = int.Parse(this.houdu.Text);
                float tempup = float.Parse(this.tempup.Text);
                float tempsb = float.Parse(this.tempsb.Text);
                float templow = float.Parse(this.templow.Text);
                float vacuup = float.Parse(this.vacuup.Text);
                float vacusb = float.Parse(this.vacusb.Text);
                float vaculow = float.Parse(this.vaculow.Text);
                float bakingup = float.Parse(this.bakingtimeup.Text);
                float bakingsb = float.Parse(this.bakingtimesb.Text);
                float bakinglow = float.Parse(this.bakingtimelow.Text);
                return true;
            }
            catch(Exception ex)
            {
                return false;
            }


        }
        private void showModel(DeviceProcess.ChipModel m)
        {
            this.name.Text = m.name;
            this.area.Text = m.area;
            this.type.Text = m.type;

            this.beizhu.Text = m.beizhu;
            this.width.Text = m.width;
            this.height.Text = m.height;
            this.houdu.Text = m.houdu;
            this.tempup.Text = m.tempup;
            this.tempsb.Text = m.tempsb;
            this.templow.Text = m.templow;
            this.bakingtimeup.Text = m.bakingtimeup;
            this.bakingtimesb.Text = m.bakingtimesb;
            this.bakingtimelow.Text = m.bakingtimelow;
            this.vacuup.Text = m.vacuup;
            this.vacusb.Text = m.vacusb;
            this.vaculow.Text = m.vaculow;
            if (m.isselect.Equals("1"))
                this.choosebn.IsEnabled = false;
            else
                this.choosebn.IsEnabled = true;
        }

        private void modellist_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {   
            if(this.modellist.SelectedIndex!=-1)
               showModel(models[this.modellist.SelectedIndex]);
        }
        private void clearModel()
        {
            this.name.Text = "";
            this.area.Text = "";
            this.type.Text = "";

            this.beizhu.Text = "";
            this.width.Text = "";
            this.height.Text = "";
            this.houdu.Text = "";
            this.tempup.Text = "";
            this.tempsb.Text = "";
            this.templow.Text = "";
            this.bakingtimeup.Text = "";
            this.bakingtimesb.Text = "";
            this.bakingtimelow.Text = "";
            this.vacuup.Text = "";
            this.vacusb.Text = "";
            this.vaculow.Text = "";
        }
        private void newbn_Click(object sender, RoutedEventArgs e)
        {
            clearModel();
        }

        private void savebn_Click(object sender, RoutedEventArgs e)
        {
            if(!check())
            {
                MessageBox.Show("请正确填写带*号数值字段");
                return;
            }
            String chipname = this.name.Text;

            DeviceProcess.ChipModel model = new DeviceProcess.ChipModel();

            model.name = chipname;
            model.area = this.area.Text;
            model.type = this.type.Text;
            model.time = this.time.Text;
            model.beizhu = this.beizhu.Text;
            model.width = this.width.Text;
            model.height = this.height.Text;
            model.houdu = this.houdu.Text;
            model.tempup = this.tempup.Text;
            model.tempsb = this.tempsb.Text;
            model.templow = this.templow.Text;
            model.vacuup = this.vacuup.Text;
            model.vacusb = this.vacusb.Text;
            model.vaculow = this.vaculow.Text;
            model.bakingtimeup = this.bakingtimeup.Text;
            model.bakingtimesb = this.bakingtimesb.Text;
            model.bakingtimelow = this.bakingtimelow.Text;
            
            foreach(DeviceProcess.ChipModel m in models)
            {
                if(m.name.Equals(name))
                {
                    int res1 = ModVariable.deviceOverView.UpdateChipModels(model);
                    if (res1 > 0)
                        MessageBox.Show("更新成功");
                    else
                        MessageBox.Show("更新失败");
                    return;
                }
            }
            
           
              int res = ModVariable.deviceOverView.InsertChipModels(model);
              if (res > 0)
                    MessageBox.Show("新建成功");
              else
                    MessageBox.Show("新建失败");
            

            updateUI();
        }
       
        private void delbn_Click(object sender, RoutedEventArgs e)
        {
            String chipname = this.name.Text;
            if (this.modellist.Text.Replace("*", "").Equals(chipname))
            {
               int res= ModVariable.deviceOverView.DelChipModels(chipname);
                if (res > 0)
                {
                    MessageBox.Show("删除成功");
                    updateUI();
                }
                else
                    MessageBox.Show("删除失败");
               
            }
            else
            {
                clearModel();
            }
            
        }

        private void choosebn_Click(object sender, RoutedEventArgs e)
        {
            if(categoryList.Count==0)
            {
                MessageBox.Show("请首先新建并保存数据");
                return;
            }
           int res= ModVariable.deviceOverView.setModel(int.Parse(categoryList[this.modellist.SelectedIndex].Value));
            if(res>0)
            {
                updateUI();
                MessageBox.Show("设定成功");
            }
            else
             {
                MessageBox.Show("设定失败");
            }
            
        }
    }
}
