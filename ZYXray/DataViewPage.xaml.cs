using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
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
using XRayClient.Core;

namespace ZYXray
{
    /// <summary>
    /// DataViewPage.xaml 的交互逻辑
    /// </summary>
    public partial class DataViewPage : Page
    {
        private static DataViewPage _instance = new DataViewPage();
        private static ObservableCollection<BatterySeat> ListString_Show = new ObservableCollection<BatterySeat>();
        public static DataViewPage Instance
        {
            get { return _instance; }
        }

        public DataViewPage()
        {
            InitializeComponent();
            listView.ItemsSource = ListString_Show;
        }

        public void UpdateListView(BatterySeat bt)
        {
            this.listView.Dispatcher.Invoke(new Action(() =>
            {
                bt.EndTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                bt.Destroy();
                ListString_Show.Insert(0,bt);
                if (ListString_Show.Count > 300)
                {
                    //ListString_Show[ListString_Show.Count - 1].Destroy();
                    ListString_Show.RemoveAt(ListString_Show.Count - 1);
                }
            }));
        }
    }
}
