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
using ZYXray.ViewModels;

namespace ZYXray
{
    /// <summary>
    /// ManualRecheckPage.xaml 的交互逻辑
    /// </summary>
    public partial class ManualRecheckPage : UserControl
    {
        public ManualRecheckPage()
        {
            InitializeComponent();

            this.DataContext = new ManualRecheckVm(this);
        }

        private void s1_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            s1.ScrollToVerticalOffset(s1.ActualHeight / 2);
            s1.ScrollToHorizontalOffset(s1.ActualWidth / 2);
        }

        private void s2_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            s2.ScrollToVerticalOffset(s2.ActualHeight / 2);
            s2.ScrollToHorizontalOffset(s2.ActualWidth / 2);
        }
    }
}
