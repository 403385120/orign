using ATL.UI.Controls;
using ATL.UI.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;
using System.ComponentModel;
using System.Windows.Threading;
using ATL.Engine;
using ATL.Core;
using ATL.Station;
using ATL.Common;

using ATL.MES;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Drawing;
using ZYXray.ViewModels;

namespace ZYXray
{
    /// <summary>
    /// DashBoardPage.xaml 的交互逻辑
    /// </summary>
    public partial class DashBoardPage : BasePage, IComponentConnector
    {
        public DashBoardPage()
        {
            InitializeComponent();

            this.DataContext = new DashBoardVm();
        }

        private void slider_ValueChanged_1(object sender, object e)
        {
            this.imgScale.RenderTransform = new ScaleTransform()
            {
                ScaleX = Models.SystemConfig.Instance.ResultScale,
                ScaleY = Models.SystemConfig.Instance.ResultScale
            };
        }
    }
}
