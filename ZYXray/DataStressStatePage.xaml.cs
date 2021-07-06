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
using System.Windows.Threading;
using ZYXray.Models;
using ZYXray.ViewModels;

namespace ZYXray
{
    /// <summary>
    /// DataStressStatePage.xaml 的交互逻辑
    /// </summary>
    public partial class DataStressStatePage : UserControl
    {
        private DispatcherTimer RefreshTimer = new DispatcherTimer();

        public DataStressStatePage()
        {
            InitializeComponent();

            this.DataContext = DataStressStateVm.Instance;

            this.RefreshTimer.Interval = new TimeSpan(0, 0, 1);
            this.RefreshTimer.Tick += new EventHandler(this.RefreshTimer_Tick);
            this.RefreshTimer.Start();
        }

        private void RefreshTimer_Tick(object sender, EventArgs e)
        {
            if (DataStressStateVm.Instance.CurTime >= 0)
            {
                DataStressStateVm.Instance.MessageAndTime = DataStressStateVm.Instance.WarnMsg + " (等待 " + DataStressStateVm.Instance.CurTime + " s后关闭)";
                DataStressStateVm.Instance.CurTime--;
            }
             if(DataStressStateVm.Instance.CurTime < 0)
            {
                lblWarmMsg.Visibility = Visibility.Hidden;
                this.RefreshTimer.Stop();
            }
        }

        public DataStressStateVm MyDataStressVm
        {
            get { return DataStressStateVm.Instance; }
        }

        
    }
}
