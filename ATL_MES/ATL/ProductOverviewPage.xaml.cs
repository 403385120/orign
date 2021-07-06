using ATL.UI.Controls;
using System;
using System.Threading;
using System.Windows;
using System.Windows.Markup;
using ATL.Station;
using ATL.Common;
using ATL.Core;
using System.Linq;
using ATL.PLCVariableValueService;
using System.Windows.Media;
using System.Threading.Tasks;
using ATL.UI;

namespace PTF
{
    /// <summary>
    /// HistoryAlarmListPage.xaml 的交互逻辑
    /// </summary>
    public partial class ProductOverviewPage : BasePage, IComponentConnector
    {
        public ProductOverviewPage()
        {
            InitializeComponent();
        }
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //PictureOfGif.Image = System.Drawing.Image.FromFile(System.IO.Directory.GetCurrentDirectory() + "/assets/mes_ng.gif");
            this.DataContext = Station.Current;
            this.statePage.Navigate(new Uri(@"ATL/StationStatePage.xaml", UriKind.Relative));
            txtEquipmentID.Text = DeivceEquipmentidPlcInfo.lstDeivceEquipmentidPlcInfos[0].EquipmentID;

            if (ATL.Common.StringResources.IsDefaultLanguage)
            {
                biDeviceGeneralSituation.Content = "设备概况";
                biDeviceState.Text = "设备状态";
                biRealtimeAlarm.Text = "实时报警";
                biRunLog.Text = "运行日志";
                biEquipID.Text = "设备ID:";
                biEquipState.Text = "设备状态:";
                biAndonState.Text = "Andon状态:";
                biControlState.Text = "控机状态:";
                FirstState.Text = "首件状态:";
                FirstMeter.Text = "首件米数:";
            }
            else
            {
                biDeviceGeneralSituation.Content = "Equipment General View";
                biDeviceState.Text = "EquipmentState";
                biRealtimeAlarm.Text = "RealtimeAlarm";
                biRunLog.Text = "RunLog";
                biEquipID.Text = "EquipID:";
                biEquipState.Text = "EquipState:";
                biAndonState.Text = "AndonState:";
                biControlState.Text = "ControlState:";
                FirstState.Text = "FirstPState:";
                FirstMeter.Text = "FirstPMeters:";
            }
        }

        public override void DoCycleWork()
        {
            Thread.Sleep(1000);
            try
            {
                Dispatcher.Invoke(delegate ()
                {
                    if (ATL.Core.Core.PLCconnected)
                    {
                        Station.Current.PLCState = "OK";
                        PlcStatus.Text = ATL.Common.StringResources.IsDefaultLanguage ? "PLC连接正常" : "PLC Conn OK";
                        PlcStatus.Foreground = new SolidColorBrush(Colors.White);
                    }
                    else
                    {
                        Station.Current.PLCState = "NG";
                        PlcStatus.Text = ATL.Common.StringResources.IsDefaultLanguage ? "PLC连接异常" : "PLC Conn NG";
                        PlcStatus.Foreground = new SolidColorBrush(Colors.Red);
                    }
                    if (Station.Current.MESState == "OK")
                    {
                        MesStatus.Text = ATL.Common.StringResources.IsDefaultLanguage ? "MES在线" : "MES Online";
                        MesStatus.Foreground = new SolidColorBrush(Colors.White);
                    }
                    else
                    {
                        MesStatus.Text = ATL.Common.StringResources.IsDefaultLanguage ? "MES离线" : "MES Offline";
                        MesStatus.Foreground = new SolidColorBrush(Colors.Red);
                    }

                    try
                    {
                        //Station.Current.ListLog = ServiceHelper.Current.getsoftwareLog(200);
                        LogList.ItemsSource = null;
                        LogList.ItemsSource = ServiceHelper.Current.getsoftwareLog(200);
                    }
                    catch (Exception ex)
                    {
                        LogInDB.Error(ex.ToString());
                    }
                });
                
                Dispatcher.Invoke(delegate ()
                {
                    ATL.Engine.PLC plc = new ATL.Engine.PLC();
                    if (DeivceEquipmentidPlcInfo.lstDeivceEquipmentidPlcInfos.Count > 0)
                    {
                        if (ATL.Core.Core.PLCconnected)
                        {
                            string ProductCount = plc.ReadByVariableName(DeivceEquipmentidPlcInfo.lstDeivceEquipmentidPlcInfos[0].QuantityAddress);
                            int productcount;
                            int.TryParse(ProductCount, out productcount);
                            Station.Current.ProductCount = productcount;
                            
                            txtRunStatus.Text = DeivceEquipmentidPlcInfo.lstDeivceEquipmentidPlcInfos[0].ParentEQStateCode;
                            txtANDONstatus.Text = DeivceEquipmentidPlcInfo.lstDeivceEquipmentidPlcInfos[0].AndonState;

                        }

                        A011StateCode.Text = DeivceEquipmentidPlcInfo.lstDeivceEquipmentidPlcInfos[0].StateCode;
                        A011StateDesc.Text = DeivceEquipmentidPlcInfo.lstDeivceEquipmentidPlcInfos[0].StateDesc;

                        txtControlCode.Text = DeivceEquipmentidPlcInfo.lstDeivceEquipmentidPlcInfos[0].ControlCode;
                        txtA007Count.Text = DeivceEquipmentidPlcInfo.lstDeivceEquipmentidPlcInfos[0].Count.ToString();
                        if (ATL.Common.StringResources.IsDefaultLanguage)
                        {
                            txtProductMode.Text = DeivceEquipmentidPlcInfo.lstDeivceEquipmentidPlcInfos[0].ProductMode == "1" ? "首件" : "量产";
                        }
                        else
                        {
                            txtProductMode.Text = DeivceEquipmentidPlcInfo.lstDeivceEquipmentidPlcInfos[0].ProductMode == "1" ? "test production" : "mass production";
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                LogInDB.Error(ex.ToString());
            }
            try
            {
                Dispatcher.Invoke(delegate ()
                {
                    try
                    {
                        RealTimeAlarmList.ItemsSource = null;
                        RealTimeAlarmList.ItemsSource = PlcDAL.ThisObject.GetAlarmTemporaries();
                    }
                    catch (Exception ex)
                    {
                        LogInDB.Error(ex.ToString());
                    }
                });
            }
            catch (Exception ex)
            {
                LogInDB.Error(ex.ToString());
            }
        }
    }
}
