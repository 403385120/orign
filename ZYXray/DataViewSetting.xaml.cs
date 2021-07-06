using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ZYXray.Winform;

namespace ZYXray
{
    /// <summary>
    /// DataViewSetting.xaml 的交互逻辑
    /// </summary>
    public partial class DataViewSetting : Page
    {
        public DataViewSetting()
        {
            InitializeComponent();
            frmSettings.Current = new frmSettings();

            //注册窗体关闭事件

            System.Windows.Forms.Integration.WindowsFormsHost host = new System.Windows.Forms.Integration.WindowsFormsHost();
            frmSettings.Current.TopLevel = false;
            frmSettings.Current.FormBorderStyle = FormBorderStyle.None;
            frmSettings.Current.Dock = DockStyle.Fill;
            host.Child = frmSettings.Current;
            this.grid.Children.Add(host);
        }
    }
}
