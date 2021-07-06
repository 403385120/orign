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
    /// DataViewRecheck.xaml 的交互逻辑
    /// </summary>
    public partial class DataViewRecheck : Page
    {
        public DataViewRecheck()
        {
            InitializeComponent();
            FrmLogin.Current = new FrmLogin(2);

            //注册窗体关闭事件

            System.Windows.Forms.Integration.WindowsFormsHost host = new System.Windows.Forms.Integration.WindowsFormsHost();
            FrmLogin.Current.TopLevel = false;
            FrmLogin.Current.FormBorderStyle = FormBorderStyle.None;
            FrmLogin.Current.Dock = DockStyle.Fill;
            host.Child = FrmLogin.Current;
            this.grid.Children.Add(host);
        }
    }
}
