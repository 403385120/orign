using ATL.UI.Controls;
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
using ATL.Common;
using System.Reflection;
using System.ComponentModel;
using DeviceLib;
using System.Windows.Threading;
using System.IO.Ports;
using System.Threading;

namespace WpfApp1
{
    /// <summary>
    /// DevicePage.xaml 的交互逻辑
    /// </summary>
    public  partial class DevicePage : BasePage
    {
        Button[] bns;
        Label[] labels;
        public const int BLANK = 10;
        public const int WAITE = 20;
        public const int BAKING = 30;
        public const int OVER = 40;
        public const int EXCAPTION = 60;
        
        public const int NOCAR = 50;
    
        List<DeviceProcess.CarNumToPos> carNumToPos;
        List<DeviceProcess.BakingTime> bakingtime;
        // public delegate void BnClick();
        //  public static BnClick bnClick;

        public static DispatcherTimer RefreshTimer =null;
        public DevicePage()
        {
            InitializeComponent();
            initView();
          
        }
        private void RefreshTimer_Tick(object sender, EventArgs e)
        {
            UpdateUI();
        }

        private void BasePage_Loaded(object sender, RoutedEventArgs e)
        {
            if (RefreshTimer == null)
            {
                RefreshTimer = new DispatcherTimer();
                RefreshTimer.Interval = new TimeSpan(0, 0, 2);
                RefreshTimer.Tick += new EventHandler(this.RefreshTimer_Tick);
                RefreshTimer.Start();
            }
            UpdateUI();

            
        }

        private void initView()
        {
            bns = new Button[] { Pos1, Pos2, Pos3, Pos4, Pos5, Pos6, Pos7,Pos8,Pos9,Pos10,Pos11,Pos12};
            labels = new Label[] { pos1_lable, pos2_lable, pos3_lable, pos4_lable, pos5_lable, pos6_lable, pos7_lable,pos9_lable};
            //初始化
            for (int i = 0; i < 7; i++)

            {               
                bns[i].Click += new RoutedEventHandler(Button_Click);
            }

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button bn = (Button)sender;
            DeviceProcess.CarNumToPos node = (DeviceProcess.CarNumToPos)bn.Tag;
            if (node == null)
            {
                MessageBox.Show("这是空位,不能查看！");
                return;
            }


            if (radio1.IsChecked==true)
            {
                ModVariable.runPage.car = node;
               
                NavigationService.GetNavigationService(this).Navigate(ModVariable.runPage);
            }
            else 
            {              
                switch (node.CarState)
                {
                    case BLANK:
                        MessageBox.Show("这是空车,不能查看！"); break;
                    default:
                        ModVariable.samplePage.Tag = node;
                        NavigationService.GetNavigationService(this).Navigate(ModVariable.samplePage);
                        break;
                }
            }
        }

        public  void UpdateUI()
        {
            //初始化
            for(int i=0; i<12;i++)
            {
                setPosState(bns[i], NOCAR, 0);
                bns[i].Tag = null;
            }

            //获取数据
            carNumToPos = DeviceProcess.getInstanse().getCarNumToPos();
            bakingtime = DeviceProcess.getInstanse().getBakingTimes();
            for (int i = 0; i < carNumToPos.Count; i++)
            {
                DeviceProcess.CarNumToPos node = carNumToPos[i];
                setPosState(bns[node.Pos - 1], node.CarState, node.CarID);
                bns[node.Pos - 1].Tag = node;           
                if(node.Pos<8&&node.CellNum!=0)
                {
                    if (node.CarState == 20)
                    {
                        labels[node.Pos - 1].Content = "入炉：" + bakingtime[i].moveintime;
                    }
                    if (node.CarState == 30)
                    {
                        try
                        {
                            labels[node.Pos - 1].Content = "入炉：" + bakingtime[i].moveintime + "\n"
                                + "开始：" + bakingtime[i].bakingbegin + "\n"
                                + "时长：" + getTime(bakingtime[i].bakingbegin, DateTime.Now.ToString())+"分钟";
                        }catch(Exception ex)
                        {
                            LogInDB.Info("CarState == 30  err:" + ex.Message);
                        }
                    }
                    if(node.CarState == 40)
                    {
                        try {
                        labels[node.Pos - 1].Content = "入炉：" + bakingtime[i].moveintime + "\n"
                           + "开始：" + bakingtime[i].bakingbegin + "\n"
                           + "结束：" + bakingtime[i].bakingend + "\n"
                        + "时长：" + getTime(bakingtime[i].bakingbegin, bakingtime[i].bakingend) + "分钟";
                        }
                        catch (Exception ex)
                        {
                            LogInDB.Info("CarState == 40  err:" + ex.Message);
                        }
                    }
                        
                }
                if (node.Pos ==9)
                {
                    try
                    {
                        labels[7].Content = "入炉：" + bakingtime[i].moveintime + "\n"
                           + "开始：" + bakingtime[i].bakingbegin + "\n"
                           + "结束：" + bakingtime[i].bakingend + "\n"
                            + "时长：" + getTime(bakingtime[i].bakingbegin, bakingtime[i].bakingend) + "分钟\n"
                           + "出炉：" + bakingtime[i].moveouttime;
                    }
                    catch (Exception ex)
                    {
                        LogInDB.Info("下料位信息显示   err:" + ex.Message);
                    }
                }
            }
           
           
            this.carcode.Text = DeviceProcess.carcode;
            //LogInDB.Info("carcode="+ DeviceOverView.carcode);

        }
        private int getTime(String start,String end)
        {
            TimeSpan t1 =new TimeSpan(DateTime.Parse(start).Ticks);
            TimeSpan t2 =new TimeSpan(DateTime.Parse(end).Ticks);
            TimeSpan t3= t2.Subtract(t1).Duration();
            return (int)t3.TotalMinutes;

        }
        public static void setPosState(Button bn, int type, int carno)
        {
    
            switch (type)
            {
                case BLANK:
                    bn.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                    bn.Foreground = new SolidColorBrush(Color.FromRgb(0, 0, 0));
                    bn.Content = carno + "#车";
                    break;
                case WAITE:
                    bn.Background = new SolidColorBrush(Color.FromRgb(255, 165, 0));
                    bn.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                    bn.Content = carno + "#车";
                    break;
                case BAKING:
                    bn.Background = new SolidColorBrush(Color.FromRgb(0, 255, 0));
                    bn.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                    bn.Content = carno + "#车";
                    break;
                case OVER:
                    bn.Background = new SolidColorBrush(Color.FromRgb(255, 255, 0));
                    bn.Foreground = new SolidColorBrush(Color.FromRgb(0, 0, 0));
                    bn.Content = carno + "#车";
                    break;
                case EXCAPTION:
                    bn.Background = new SolidColorBrush(Color.FromRgb(255, 0, 0));
                    bn.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                    bn.Content = carno + "#车";
                    break;
                //case NG:
                //    bn.Background = new SolidColorBrush(Color.FromRgb(128, 0, 128));
                //    bn.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                //    bn.Content = carno + "#车";
                //    break;
                case NOCAR:
                    bn.Background = new SolidColorBrush(Color.FromRgb(0, 0, 255));
                    bn.Content = "";
                    break;
            }

        }

        private void autosamplebn_Click(object sender, RoutedEventArgs e)
        {
            AutoSampleForm f = new AutoSampleForm();
            f.Show();
        }

        private void testbn_Click(object sender, RoutedEventArgs e)
        {
            ModVariable.deviceOverView.SendData();
        }

        
    }
}
