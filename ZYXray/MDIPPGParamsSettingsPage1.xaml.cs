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
    /// MDIPPGParamsSettingsPage1.xaml 的交互逻辑
    /// </summary>
    public partial class MDIPPGParamsSettingsPage1 : Page
    {
        public MDIPPGParamsSettingsPage1()
        {
            InitializeComponent();
            frmProductType.Current = new frmProductType();

            //注册窗体关闭事件

            System.Windows.Forms.Integration.WindowsFormsHost host = new System.Windows.Forms.Integration.WindowsFormsHost();
            frmProductType.Current.TopLevel = false;
            frmProductType.Current.FormBorderStyle = FormBorderStyle.None;
            frmProductType.Current.Dock = DockStyle.Fill;
            host.Child = frmProductType.Current;
            this.grid.Children.Add(host);
        }
    }
}
