using ATL.Common;
using ATL.UI.Controls;
using DeviceLib;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// SamplePage.xaml 的交互逻辑
    /// </summary>
    public partial class DeviceSamplePage : BasePage
    {

        public static int Row = 30, Col = 20;
        DeviceProcess.CarNumToPos node;
        List<DeviceProcess.CellBarCode> cellbarcode = new List<DeviceProcess.CellBarCode>();
        Button[] bns1,bns2;
        public DeviceSamplePage()
        {
            InitializeComponent();                  
        }
      
        private void InitView()
        {
            //获取数据
            node = (DeviceProcess.CarNumToPos)this.Tag;
            cellbarcode = DeviceProcess.getInstanse().getCellBarCode(node.CarID);
           // MessageBox.Show("cellbarcode count="+ cellbarcode.Count);
            this.title.Content = node.CarID + "#车电芯抽样";

            ObservableCollection<Laminate> lamdata = new ObservableCollection<Laminate>();   //集合，即数据源
            for (int i = 1; i <= Row; i++)
                lamdata.Add(new Laminate(i, i + ""));

            this.LaminateNum.SelectedValuePath = "Id";   //程序内部维护的值
            this.LaminateNum.DisplayMemberPath = "Name";  //显示的内容
            this.LaminateNum.ItemsSource = lamdata;  //数据源
            this.LaminateNum.SelectedValue = 1;  //选中的值

            bns1 = new Button[] { bn1_1,bn1_2,bn1_3,bn1_4,bn1_5,bn1_6,bn1_7,bn1_8,bn1_9,bn1_10};
            bns2 = new Button[] { bn2_1,bn2_2,bn2_3,bn2_4,bn2_5,bn2_6,bn2_7,bn2_8,bn2_9,bn2_10};
            SetCodeButton(1);
          
        }

        private void SetCodeButton(int laminateNum)
        {
            List<DeviceProcess.CellBarCode> sub_codes1 = new List<DeviceProcess.CellBarCode>();
            List<DeviceProcess.CellBarCode> sub_codes2 = new List<DeviceProcess.CellBarCode>();
            for (int i = 0; i < 10; i++)
            {
                bns1[i].Content = "";
                bns2[i].Content = "";
                setBnBackground(bns1[i], 10);
                setBnBackground(bns2[i], 10);
            }



            foreach (DeviceProcess.CellBarCode code in cellbarcode)
            {
                if (code.LaminateNum == laminateNum&&code.RowNum==1)
                    sub_codes1.Add(code);
                if (code.LaminateNum == laminateNum && code.RowNum == 2)
                    sub_codes1.Add(code);
                if (code.LaminateNum == laminateNum && code.RowNum == 3)
                    sub_codes2.Add(code);
                if (code.LaminateNum == laminateNum && code.RowNum == 4)
                    sub_codes2.Add(code);
            }
            
            for(int i=0;i<sub_codes1.Count;i++)
            {
                DeviceProcess.CellBarCode code = sub_codes1[i];
                
                String str = "";
                for (int j = 0; j< code.CellBarcode.Length; j++)
                    str += code.CellBarcode[j]+"\n";
                bns1[i].Content = str;
                bns1[i].Tag = code;
                setBnBackground(bns1[i], code.CellBakingState);
                
            }
            for (int i = 0; i < sub_codes2.Count; i++)
            {
                DeviceProcess.CellBarCode code = sub_codes1[i];
                
                String str = "";
                for (int j = 0; j < code.CellBarcode.Length; j++)
                    str += code.CellBarcode[j] + "\n";
                bns2[i].Content = str;
                bns2[i].Tag = code;
                setBnBackground(bns2[i], code.CellBakingState);
              
            }
      

        }

        //private void checkBn(Button bn,int pos,List<DeviceProcess.CellBarCode> codes)
        //{
           
        //    foreach (DeviceProcess.CellBarCode code in codes)
        //    {

        //        if (code.PosNum == pos)
        //        {
        //            bn.Tag = code;
        //            bn.Click += Bn_Click;
        //            String str = "";
        //            foreach (char c in code.CellBarcode)
        //            {
        //                str += c + "\n";
        //            }
        //            bn.Content = str;
        //            setBnBackground(bn, code.CellBakingState);
        //            //switch (code.CellBakingState)
        //            //{
        //            //    case 20:                   
        //            //        bn.Background = Brushes.White;
        //            //        bn.Foreground = Brushes.Black;
        //            //        break;
        //            //    case 30:
        //            //        bn.Background = Brushes.Red;
        //            //        bn.Foreground = Brushes.White;
        //            //        break;
        //            //    case 40:
        //            //        bn.Background = Brushes.Black;
        //            //        bn.Foreground = Brushes.White;
        //            //        break;
        //            //    case 50:
        //            //        bn.Background = Brushes.Gray;
        //            //        bn.Foreground = Brushes.White;
        //            //        break;
        //            //    case 60:
        //            //        bn.Background = Brushes.Green;
        //            //        bn.Foreground = Brushes.Black;
        //            //        break;
        //            //    case 70:
        //            //        bn.Background = Brushes.Pink;
        //            //        bn.Foreground = Brushes.Black;
        //            //        break;
                       
        //            //    case 90:
        //            //        bn.Background = Brushes.Orange;
        //            //        bn.Foreground = Brushes.White;
        //            //        break;
        //            //    case 100:
        //            //        bn.Background = Brushes.Yellow;
        //            //        bn.Foreground = Brushes.Black;
        //            //        break;

        //            //}
        //        }
              
        //    }
        //}

        private void setBnBackground(Button bn,int CellBakingState)
        {
            switch (CellBakingState)
            {
                case 10:
                    bn.Background = Brushes.Blue;
                    bn.Foreground = Brushes.White;
                    break;
                case 20:
                    bn.Background = Brushes.White;
                    bn.Foreground = Brushes.Black;
                    break;
                case 30:
                    bn.Background = Brushes.Red;
                    bn.Foreground = Brushes.White;
                    break;
                case 40:
                    bn.Background = Brushes.Black;
                    bn.Foreground = Brushes.White;
                    break;
                case 50:
                    bn.Background = Brushes.Gray;
                    bn.Foreground = Brushes.White;
                    break;
                case 60:
                    bn.Background = Brushes.Green;
                    bn.Foreground = Brushes.Black;
                    break;
                case 70:
                    bn.Background = Brushes.Pink;
                    bn.Foreground = Brushes.Black;
                    break;

                case 90:
                    bn.Background = Brushes.Orange;
                    bn.Foreground = Brushes.White;
                    break;
                case 100:
                    bn.Background = Brushes.Yellow;
                    bn.Foreground = Brushes.Black;
                    break;

            }
        }

        //private void Bn_Click(object sender, RoutedEventArgs e)
        //{ 
        //    Button bn = (Button)sender;
        //    DeviceProcess.CellBarCode code = (DeviceProcess.CellBarCode)bn.Tag;
            
        //    if (code.CellBakingState == 10)
        //    {
        //        setBnBackground(bn, 100);
        //        code.CellBakingState = 100;
        //        ModVariable.deviceOverView.UpdateCellBarCode(node.CarID, code);

        //        return;
        //    }

        //    if (code.CellBakingState == 100)
        //    {
        //        setBnBackground(bn, 10);
        //        code.CellBakingState = 10;
        //        ModVariable.deviceOverView.UpdateCellBarCode(node.CarID, code);
        //        return;
        //    }


        //}

        private void BasePage_Loaded(object sender, RoutedEventArgs e)
        {            
            InitView();  
        }
        


        private void back_Click(object sender, RoutedEventArgs e)
        {

            NavigationService.GetNavigationService(this).Navigate(ModVariable.devicePage);

        }

        private void bn1_1_Click(object sender, RoutedEventArgs e)
        {
            Button bn = (Button)sender;
            if (bn.Content.Equals("")) return;
            DeviceProcess.CellBarCode code = (DeviceProcess.CellBarCode)bn.Tag;

            if (code.CellBakingState == 10)
            {
                setBnBackground(bn, 100);
                code.CellBakingState = 100;
                ModVariable.deviceOverView.UpdateCellBarCode(node.CarID, code);

                return;
            }

            if (code.CellBakingState == 100)
            {
                setBnBackground(bn, 10);
                code.CellBakingState = 10;
                ModVariable.deviceOverView.UpdateCellBarCode(node.CarID, code);
                return;
            }
        }

        private void LaminateNum_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
                     
            if (this.LaminateNum.SelectedValue != null)
             {
                int n = int.Parse(this.LaminateNum.SelectedValue.ToString());
                MessageBox.Show("n=" + n);
                SetCodeButton(n);
            }
          
            
        }

        class Laminate  //声明类
        {
            
            int id;
            public int Id
            {
                get { return id; }
                set { id = value; }
            }
            string name;

           
            public string Name
            {
                get { return name; }
                set { name = value; }
            }

            public Laminate(int id, string name)
            {
                this.id = id;
                this.name = name;
            }

        }
    }
}
