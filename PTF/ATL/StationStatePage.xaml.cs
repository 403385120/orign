using ATL.UI.Controls;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Threading;
using System;
using System.Collections.Generic;
using ATL.Core;

namespace ATL.UI.DeviceOverview
{
    /// <summary>
    /// HistoryAlarmListPage.xaml 的交互逻辑
    /// </summary>
    public partial class StationStatePage : BasePage, IComponentConnector
    {
        public StationStatePage()
        {
            InitializeComponent();
        }
        List<FacilityInfo> lstFacilityInfo = new List<FacilityInfo>();
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            lstFacilityInfo = ServiceHelper.Current.GetFacilityState();
        }

        public override void DoCycleWork()
        {
            this.Dispatcher.BeginInvoke((Action)delegate ()
            {
                FacilityItems.Children.Clear();
                GetFacilities();
            });

            Thread.Sleep(1000);
        }

        private void GetFacilities()
        {

            if (lstFacilityInfo.Count > 0)
            {
                foreach (var facilityinfo in lstFacilityInfo)
                {
                    string FacilityState;
                    Engine.PLC plc = new Engine.PLC();
                    if (facilityinfo.StateAddress != null)
                        FacilityState = plc.ReadByVariableName(facilityinfo.StateAddress);
                    else
                        FacilityState = facilityinfo.State.ToString();
                    Style style = this.FindResource("StationButtonStyle2") as Style;
                    Button button1 = new Button();
                    button1.Style = style;
                    button1.Content = facilityinfo.MMName;
                    button1.Click += new RoutedEventHandler(this.Button_Click);
                    button1.Tag = FacilityState;
                    Color color = FacilityState == "0" ? (Color)ColorConverter.ConvertFromString("Lime") : (Color)ColorConverter.ConvertFromString("OrangeRed");
                    button1.Background = new SolidColorBrush(color);
                    FacilityItems.Children.Add(button1);
                }
            }

        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string state = ((Button)sender).Tag.ToString();
            if (state != "0")
            {
                if (sendMessage != null)
                    sendMessage("pack://application:,,,/ATL.UI;component/Maintain/RealTimeAlarmPage.xaml");
            }
        }

        public delegate void SendMessage(string s);
        public static SendMessage sendMessage;
    }
}
