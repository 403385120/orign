using ATL.UI.Controls;
using System;
using System.Threading;
using System.Windows;
using System.Windows.Markup;
using ATL.Engine;
using ATL.Station;
using ATL.Common;
using ATL.Core;
using System.Linq;
using ATL.PLCVariableValueService;

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
            this.DataContext = Station.Current;
            this.statePage.Navigate(new Uri(@"ATL/StationStatePage.xaml", UriKind.Relative));
            txtEquipmentID.Text = DeivceEquipmentidPlcInfo.lstDeivceEquipmentidPlcInfos[0].EquipmentID;
        }

        public override void DoCycleWork()
        {
            Thread.Sleep(1000);
            this.Dispatcher.BeginInvoke((Action)delegate (){ });
            
            try
            {
                Station.Current.ListLog = ServiceHelper.Current.getsoftwareLog(200);

                if (ATL.Core.Core.PLCconnected)
                {
                    Station.Current.PLCState = "OK";
                }
                else
                {
                    Station.Current.PLCState = "NG";
                }

                if (!ATL.Core.Core.PLCconnected) return;
                Dispatcher.Invoke(delegate ()
                {
                    ATL.Engine.PLC plc = new ATL.Engine.PLC();
                    if (DeivceEquipmentidPlcInfo.lstDeivceEquipmentidPlcInfos.Count > 0)
                    {
                        string ProductCount = plc.ReadByVariableName(DeivceEquipmentidPlcInfo.lstDeivceEquipmentidPlcInfos[0].QuantityAddress);
                        int productcount;
                        int.TryParse(ProductCount, out productcount);
                        Station.Current.ProductCount = productcount;

                        A011StateCode.Text = DeivceEquipmentidPlcInfo.lstDeivceEquipmentidPlcInfos[0].StateCode;
                        A011StateDesc.Text = DeivceEquipmentidPlcInfo.lstDeivceEquipmentidPlcInfos[0].StateDesc;

                        UserVariableValueService Service = new UserVariableValueService();
                        string ParentEQStateCode, AndonState;
                        Service.GetStatus(DeivceEquipmentidPlcInfo.lstDeivceEquipmentidPlcInfos[0], out ParentEQStateCode, out AndonState);

                        txtRunStatus.Text = ParentEQStateCode;
                        txtANDONstatus.Text = AndonState;

                        txtControlCode.Text = DeivceEquipmentidPlcInfo.lstDeivceEquipmentidPlcInfos[0].ControlCode;
                        txtProductMode.Text = DeivceEquipmentidPlcInfo.lstDeivceEquipmentidPlcInfos[0].ProductMode == "1" ? "首件" : "量产";
                        txtA007Count.Text = DeivceEquipmentidPlcInfo.lstDeivceEquipmentidPlcInfos[0].Count.ToString();
                    }
                });
            }
            catch (Exception ex)
            {
                LogInDB.Error(ex.ToString());
            }
            try
            {
                Station.Current.ListLog = ServiceHelper.Current.getsoftwareLog(200);
            }
            catch (Exception ex)
            {
                LogInDB.Error(ex.ToString());
            }
            try
            {
                Station.Current.RealTimeAlarmList = PlcDAL.ThisObject.GetAlarmTemporaries();
            }
            catch(Exception ex)
            {
                LogInDB.Error(ex.ToString());
            }
        }
    }
}
