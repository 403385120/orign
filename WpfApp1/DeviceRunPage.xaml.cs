using ATL.Common;
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
    /// DeviceRunPage.xaml 的交互逻辑
    /// </summary>
    public partial class DeviceRunPage : BasePage
    {
        List<DeviceProcess.TempNode> tempNodes;
        public DeviceProcess.CarNumToPos car;
        List<Point> points = new List<Point>();//真空度
        List<Point>[] pointlist = new List<Point>[30]; //30层温度
        int start_x = 50;
        int start_y = 10;
        int end_x = 1000;
        int end_y = 250;
        public DeviceRunPage()
        {
            InitializeComponent();
            
        }

        private void back_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GetNavigationService(this).Navigate(ModVariable.devicePage);
        }
        DeviceProcess.Params p;
        private void BasePage_Loaded(object sender, RoutedEventArgs e)
        {
            p = ModVariable.deviceOverView.GetParams();           
            getData();
            Draw_back1();//温度曲线曲线        
            Draw_back2();//真空度曲线
         
        }

        private void getData()
        {
            //初始化数据
            points = new List<Point>();//真空度数据
            for (int i = 0; i < 30; i++)
                pointlist[i] = new List<Point>();
            //获取数据并分解
            tempNodes = ModVariable.deviceOverView.FindTempNode(car);

            if (tempNodes == null || tempNodes.Count == 0) return;
            points.Clear();
           
            int y1, y2; int x = 0;
            foreach (DeviceProcess.TempNode n in tempNodes)
            {
                x = (int)(start_x + n.totalminiutes * 2);

                y1 = (int)(end_y - n.Vacu / 100);
                points.Add(new Point(x, y1));

                Point[] temps = new Point[30];
                for (int i = 0; i < 30; i++)
                {
                    y2 = end_y - (int)n.temp[i];
                    pointlist[i].Add(new Point(x, y2));
                }
                
            }
            LogInDB.Info("points="+points.Count);
        }

      
      

        private void Draw_xy(Canvas canvas)
        {
            //y轴
            drawline(canvas, Brushes.Black, start_x, start_y, start_x,end_y);
            drawline(canvas, Brushes.Black, start_x, start_y, start_x-5,start_y+10);
            drawline(canvas, Brushes.Black, start_x, start_y, start_x+5,start_y+10);

            //x轴
            drawline(canvas, Brushes.Black, start_x, end_y, end_x, end_y);
            drawline(canvas, Brushes.Black, end_x, end_y, end_x-10, end_y-5);
            drawline(canvas, Brushes.Black, end_x, end_y, end_x-10, end_y+5);
 

        }

        private void Draw_back1()
        {
            Draw_xy(this.canvas1);
            drawText(this.canvas1, Brushes.White, "温度公差±5°C，时间公差±3min", 450, 60);
            drawText(this.canvas1, Brushes.White, p.wendu1 + "°C", 0, end_y - p.wendu1 * 2);
            drawText(this.canvas1, Brushes.White, p.wendu2 + "°C", 0, end_y - p.wendu2 * 2);
            drawline(this.canvas1, Brushes.Gray, start_x, end_y - p.wendu1 * 2, start_x + p.wendu_time1 * 2, end_y - p.wendu1 * 2);
            drawline(this.canvas1, Brushes.Gray, start_x + p.wendu_time1 * 2, end_y - p.wendu1 * 2, start_x + p.wendu_time2 * 2, end_y - p.wendu2 * 2);
            drawline(this.canvas1, Brushes.Gray, start_x + p.wendu_time2 * 2, end_y - p.wendu2 * 2, end_x - 50, end_y - p.wendu2 * 2);


            for (int i = 0; i < 30; i++)
            {

                List<Point> ps = pointlist[i];

                for (int j = 0; j < ps.Count - 1; j++)
                {
                    drawline(this.canvas1, Brushes.Yellow, (int)ps[j].X, (int)ps[j].Y, (int)ps[j + 1].X, (int)ps[j + 1].Y);

                }
            }
        }


        private void Draw_back2()
        {
            Draw_xy(this.canvas2);
            drawText(this.canvas2, Brushes.White, "真空度公差±500pa，时间公差±2min", 450, 60);
           
            drawText(this.canvas2, Brushes.White, p.kpa1 + "kPa", 0, end_y - p.kpa1 * 10);
            drawText(this.canvas2, Brushes.White, p.kpa2 + "kPa", 0, end_y - p.kpa2 * 10);
            for (int i = 0; i < p.time2.Length; i++)
            {
                int l = start_x + p.time2[i] * 2;
                int r = l + 10;
                //e.DrawString(time2[i] + "", new Font("宋体", 10), s1, new Point(l - 15, 215)); //标线
                //e.DrawString((time2[i] + 5) + "", new Font("宋体", 10), s1, new Point(r, 215));
                drawline(this.canvas2, Brushes.Gray, l, end_y , l, end_y - p.kpa2 * 10);
                drawline(this.canvas2, Brushes.Gray, r, end_y , r, end_y - p.kpa2 * 10);
                drawline(this.canvas2, Brushes.Gray, l, end_y - p.kpa2 * 10, r, end_y - p.kpa2 * 10);

              
            }

            for (int i = 0; i < p.time1.Length; i++)
            {
                int l = start_x + p.time1[i] * 2;
                int r = l + 10;
                //e.DrawString(time1[i] + "", new Font("宋体", 10), s1, new Point(l - 15, 215)); //标线
                //e.DrawString((time1[i] + 5) + "", new Font("宋体", 10), s1, new Point(r, 215));

                drawline(this.canvas2, Brushes.Gray, l, end_y, l, end_y - p.kpa1 * 10);
                drawline(this.canvas2, Brushes.Gray, r, end_y, r, end_y - p.kpa1 * 10);
                drawline(this.canvas2, Brushes.Gray, l, end_y - p.kpa1 * 10, r, end_y - p.kpa1 * 10);

            }

            for (int i = 0; i < points.Count - 1; i++)
                drawline(this.canvas2, Brushes.Yellow, (int)points[i].X, (int)points[i].Y, (int)points[i + 1].X, (int)points[i + 1].Y);


        }

        private void drawline(Canvas canvas, Brush brush, int x1, int y1, int x2, int y2)
        {
            Line base_line1 = new Line();
            base_line1.Stroke = brush;
            base_line1.StrokeThickness = 2;
            base_line1.X1 = x1;
            base_line1.Y1 = y1;
            base_line1.X2 = x2;
            base_line1.Y2 = y2;
            canvas.Children.Add(base_line1);
        }
        private void drawText(Canvas canvas, Brush brush, String text,int x,int y)
        {
            TextBlock textBlock = new TextBlock();
            textBlock.Text = text;
            textBlock.FontSize = 15;
            textBlock.Foreground = brush;
            Canvas.SetLeft(textBlock, x);
            Canvas.SetTop(textBlock, y);
            canvas.Children.Add(textBlock);

        }
    }
}
