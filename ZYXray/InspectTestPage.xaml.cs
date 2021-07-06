using ATL.UI.Controls;
using ATL.UI.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Threading;
using ATL.Engine;
using ATL.Core;
using ATL.Station;
using ATL.Common;

using ATL.MES;
using System.Threading.Tasks;
using ZY.Motion;
using System.Windows.Controls;
using System.Drawing;
using ZYXray.ViewModels;
using XRayClient.VisionSysWrapper;
using XRayClient.Core;
using ZYXray.Models;

namespace ZYXray
{
    /// <summary>
    /// InspectTestPage.xaml 的交互逻辑
    /// </summary>
    public partial class InspectTestPage :  BasePage, IComponentConnector
    {
        public InspectTestPage()
        {
            InitializeComponent();

            this.DataContext = new InspectTestPageVm();

            //BotIF.Init(HardwareConfig.Instance.CameraParams1);

            //bool ret = false;
            //ret = VisionSysWrapperIF.InitAlgo();
            //if(ret == false)
            //{
            //    MessageBox.Show("算法初始化失败");
            //}
        }
    }
}
