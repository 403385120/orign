using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ATL.Devices;
using log4net;
using ATL.Common;
using ATL.UI.MES;
using ATL.MES;

namespace PTF
{
    public class ModVariable
    {
        public static WinformMainPage testFormPage = null;
        public static WinformUserControlPage testUserControlPage = null;
        public static DeviceMainPage deviceMainPage = null;
        public static DashBoardPage dashBoardPage = null;
        public static MotionControlPage motionControlPage = null;
        public static XrayTubeControlPage xrayTubeControlPage = null;
        public static CameraSettingPage cameraSettingPage = null;
        public static CameraCaliPage cameraCaliPage = null;
        public static CheckParamsSettingsPage checkParamsSettingsPage = null;
        public static InspectTestPage inspectTestPage = null;
        public static frmBarcode frmCarrierBarcode;
        public static frmTCPCCD frmCellBarcode;
        //public static frmTCPDevice frmCellBarcode;
        public static void MainClose()
        {
            if (frmCarrierBarcode != null)
            {
                frmCarrierBarcode.ShouldHide = false;
            }
            if (frmCellBarcode != null)
            {
                frmCellBarcode.ShouldHide = false;
                frmCellBarcode.Close();
            }
        }
    }
}
