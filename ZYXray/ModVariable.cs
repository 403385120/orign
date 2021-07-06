using ATL.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XRayClient.Core;
using XRayClient.VisionSysWrapper;
using ZY.BarCodeReader;
using ZY.Logging;
using ZY.MitutoyoReader;
using ZY.Systems;
using ZY.Vision;
using ZY.XRayTube;
using ZYXray.Models;

namespace ZYXray
{
    public class ModVariable
    {
        
        public static DashBoardPage dashBoardPage = null;
        public static MotionControlPage motionControlPage = null;
        public static XrayTubeControlPage xrayTubeControlPage = null;
        public static CameraSettingPage cameraSettingPage = null;
        public static CameraCaliPage cameraCaliPage = null;
        public static CheckParamsSettingsPage checkParamsSettingsPage = null;
        public static InspectTestPage inspectTestPage = null;
        public static ThicknessCheckPage thicknessCheck = null;
        public static DimensionCheckPage dimensionCheck = null;
        public static TestStationInfoPage testStationInfo = null;
        public static DataViewPage dataViewPage = null;
        public static TestCodesPage testCodesPage = null;
        public static ChannelCheckPage channelCheckPage = null;
        public static IVOCVParamsSettingsPage iVOCVParamsSettingsPage = null;
        public static MDIPPGParamsSettingsPage mDIPPGParamsSettingsPage = null;
        public static MDIPPGParamsSettingsPage1 mDIPPGParamsSettingsPage1 = null;
        public static ReModelPage reModelPage = null;
        public static DataViewRecheck DataViewRecheck = null;
        public static DataViewCommunication DataViewCommunication = null;
        public static DataViewSetting DataViewSetting = null;

        public static void init()
        {
            LoggingIF.Init();
            LoggingIF.Log("系统启动", LogLevels.Info, "MainWindow");
            UserDefineVariableInfo.DicVariables["AlgoInitFinished"] = 0;

            BotIF.Init(HardwareConfig.Instance.CameraParams1, HardwareConfig.Instance.CameraParams2);

            XRayTubeIF.Init(HardwareConfig.Instance.XRayConfig1, HardwareConfig.Instance.XRayConfig2);
            //CodeReaderIF.Init_ClientConnet(HardwareConfig.Instance.ScanBarcodeIPAdress, HardwareConfig.Instance.ScanBarcodePort, 1);//扫码1
            //CodeReaderIF.Init_ClientConnet(HardwareConfig.Instance.ScanBarcodeIPAdress2, HardwareConfig.Instance.ScanBarcodePort2, 3);//扫码2
            CodeReaderIF.Init_ClientConnet(HardwareConfig.Instance.DimensionIPAdress, HardwareConfig.Instance.DimensionPort, 2);//尺寸测量
            if (Common.IsShortSocket)//短连接
            {

            }
            else {
                MitutoyoReaderIF.Init(HardwareConfig.Instance.MitutoyoConfig, 1);//A测厚
                MitutoyoReaderIF.Init(HardwareConfig.Instance.MitutoyoConfig2, 2);//B测厚
                MitutoyoReaderIF.Init(HardwareConfig.Instance.MitutoyoConfig3, 3);//A测厚
                MitutoyoReaderIF.Init(HardwareConfig.Instance.MitutoyoConfig4, 4);//B测厚
            }
            VisionSysWrapperIF.AddCamera(HardwareConfig.Instance.CameraParams1);
            VisionSysWrapperIF.AddCamera(HardwareConfig.Instance.CameraParams2);

            //TODO:相机初始化 ZhangKF 2021-3-16
            CameraHelper.Init();
            //int[] iResults;
            //int iRet = VisionSysWrapperIF.CameraInit(out iResults,false);
            //if (0 != iRet)
            //{
            //    LoggingIF.Log("相机初始化失败", LogLevels.Info, "MainWindow");
            //    System.Windows.MessageBox.Show("相机初始化失败!", "提示");
            //}
            //else
            //{
            //    VisionSysWrapper.VisionSysNativeIF_SetSreachTable(0, HardwareConfig.Instance.CameraParams1.min_graylevel, HardwareConfig.Instance.CameraParams1.max_graylevel);//设置平板1灰度值
            //    VisionSysWrapper.VisionSysNativeIF_SetSreachTable(1, HardwareConfig.Instance.CameraParams2.min_graylevel, HardwareConfig.Instance.CameraParams2.max_graylevel);//设置平板2灰度值
            //    LoggingIF.Log("相机初始化成功", LogLevels.Info, "MainWindow");
            //}


            bool ret = false;
            InspectParams paramsinpect = new InspectParams();
            paramsinpect.total_layer = 16;
            paramsinpect.iLine = 1;
            paramsinpect.iCorner = 1;
            paramsinpect.pixel_to_mm = (float)0.0075;

            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                ret = VisionSysWrapperIF.InitAlgo_DL(ref paramsinpect);
                if (ret == false)
                {
                    //MessageBox.Show("算法初始化失败");
                    LoggingIF.Log("算法初始化失败", LogLevels.Info, "MainWindow");
                }
                else
                {
                    //MessageBox.Show("算法初始化成功");
                    UserDefineVariableInfo.DicVariables["AlgoInitFinished"] = 1;
                    LoggingIF.Log("算法初始化成功", LogLevels.Info, "MainWindow");
                }
            }).Start();

        }
    }
}
