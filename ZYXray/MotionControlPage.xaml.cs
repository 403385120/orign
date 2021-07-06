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


namespace ZYXray
{
    /// <summary>
    /// MotionControlPage.xaml 的交互逻辑
    /// </summary>
    public partial class MotionControlPage : BasePage, IComponentConnector
    {
        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key,
                                                         string val, string filePath);
        public MotionControlPage()
        {
            InitializeComponent();

            DataContext = MotionControlVm.Instance;
        }

        private void Log_TextChanged(object sender, TextChangedEventArgs e)
        {
            Log.ScrollToEnd();
        }

        private void textBox_LostFocus(object sender, RoutedEventArgs e)
        {
            string _configFile = System.Environment.CurrentDirectory + "\\HardwareConfig.ini";
            int res = 0;
            if (int.TryParse(textBox.Text, out res))
            {
                WritePrivateProfileString("XRayTube1Config", "XRayTubeIdleTime", res.ToString(), _configFile);
            }
            else
            {
                MessageBox.Show("请输入有效数字！");
            }
            
        }
    }
}
