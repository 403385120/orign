using ATL.UI.Controls;
using ATL.UI.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Markup;
using System.ComponentModel;
using System.Windows.Threading;
using ATL.Engine;
using ATL.Core;
using ATL.Station;
using ATL.Common;
using ATL.MES;
using System.Threading.Tasks;
using ZY.Motion;
using ZY.Logging;
using System.Windows.Controls;
using System.Drawing;
using ZYXray.ViewModels;
using System.Runtime.InteropServices;
using XRayClient.Core;

namespace ZYXray
{
    /// <summary>
    /// TestStationInfoPage.xaml 的交互逻辑
    /// </summary>
    public partial class TestStationInfoPage : BasePage, IComponentConnector
    {
        private static TestStationInfoPage _instance = new TestStationInfoPage();
        public static TestStationInfoPage Instance
        {
            get { return _instance; }
        }
        public TestStationInfoPage()
        {
            InitializeComponent();

            DataContext = TestStationInfoVm.Instance;
        }

        
    }
}
