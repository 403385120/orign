using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ZYXray.ViewModels;
using ZY.Logging;
using XRayClient.Core;

namespace ZYXray
{
    /// <summary>
    /// ImageControl.xaml 的交互逻辑
    /// </summary>
    public partial class ImageControl : UserControl
    {
        private int checktimes = 0;
        private static CheckStatus _checkStatus = BotIF.MyCheckStatus;
        private bool _canMeasure = false;
        private Point _startPoint = new Point();            // Measurement start
        public ImageControl()
        {
            InitializeComponent();

            //DataContext = new ImageControlVm();
        }

        private void Grid_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this._canMeasure = true;
            this._startPoint = e.GetPosition(sender as Grid);

            this._segment.X1 = this._startPoint.X;
            this._segment.Y1 = this._startPoint.Y;
        }

        private void Grid_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this._canMeasure = false;
            double length = double.Parse(this._axtualDist.Content.ToString());
            LoggingIF.Log(string.Format("针规测量长度: {0}mm", this._axtualDist.Content.ToString()), LogLevels.Info, "ImageControl");
            if (length < 1.05 && length > 0.95)
                checktimes += 1;

            if(checktimes >= 3)
            {
                LoggingIF.Log("针规标定通过", LogLevels.Info, "ImageControl");
                _checkStatus.MyStartupTestConfig.LastPinCheckTime = DateTime.Now;
                _checkStatus.MyStartupTestConfig.LastPinCheckHour = DateTime.Now.Hour;
                CheckParamsConfig.Instance.SaveStartupConfig();
            }
        }

       

        private void Grid_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (!this._canMeasure) return;

            Point endPoint = e.GetPosition(sender as Grid);

            this._segment.X2 = endPoint.X;
            this._segment.Y2 = endPoint.Y;

            //计算斜率
            float k1 = 0;
            float b1 = 0;
            float b2 = 0;

            float Ldistance = 10;


            if(_startPoint.X == endPoint.X)
            {
                //此时K1=0
                k1 = 0;

                b1 = (float)(_startPoint.Y);
                b2 = (float)(endPoint.Y);

                this._startSeg.X1 = _startPoint.X - Ldistance;
                this._startSeg.Y1 = b1;
                this._startSeg.X2 = _startPoint.X + Ldistance;
                this._startSeg.Y2 = b1;

                this._endSeg.X1 = endPoint.X - Ldistance;
                this._endSeg.Y1 = b2;
                this._endSeg.X2 = endPoint.X + Ldistance;
                this._endSeg.Y2 = b2;

            }
            else
            {
                if(_startPoint.Y == endPoint.Y)
                {
                    //此时是x=b
                    b1 = (float)(_startPoint.X);
                    b2 = (float)(endPoint.X);

                    this._startSeg.X1 = b1;
                    this._startSeg.Y1 = _startPoint.Y - Ldistance;
                    this._startSeg.X2 = b1;
                    this._startSeg.Y2 = _startPoint.Y + Ldistance;

                    this._endSeg.X1 = b2;
                    this._endSeg.Y1 = endPoint.Y - Ldistance;
                    this._endSeg.X2 = b2;
                    this._endSeg.Y2 = endPoint.Y + Ldistance;



                }
                else
                {
                    k1 = - ((float)(endPoint.X - _startPoint.X) / (float)(endPoint.Y - _startPoint.Y));
                    b1 = (float)(_startPoint.Y - k1 * _startPoint.X);
                    b2 = (float)(endPoint.Y - k1 * endPoint.X);

                    float cies = (float)(Ldistance / Math.Sqrt(k1 * k1 + 1));

                    this._startSeg.X1 = _startPoint.X - cies;
                    this._startSeg.Y1 = (_startPoint.X - cies) * k1 + b1;
                    this._startSeg.X2 = _startPoint.X + cies;
                    this._startSeg.Y2 = (_startPoint.X + cies) * k1 + b1;

                    this._endSeg.X1 = endPoint.X - cies;
                    this._endSeg.Y1 = (endPoint.X - cies) * k1 + b2;
                    this._endSeg.X2 = endPoint.X + cies;
                    this._endSeg.Y2 = (endPoint.X + cies) * k1 + b2;


                }

            }
            
            var dist = Math.Sqrt(Math.Pow(endPoint.X - _startPoint.X, 2) + Math.Pow(endPoint.Y - _startPoint.Y, 2));
            this._pixelDist.Content = dist.ToString("N3");
        }
    }
}
