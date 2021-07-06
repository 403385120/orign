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

namespace ZYXray
{
    /// <summary>
    /// CheckParamsSettingsPage.xaml 的交互逻辑
    /// </summary>
    public partial class MDIPPGParamsSettingsPage : BasePage, IComponentConnector
    {
        public MDIPPGParamsSettingsPage()
        {
            InitializeComponent();

            this.DataContext = CheckParamsSettingsVm.Instance;
        }
    }
}
