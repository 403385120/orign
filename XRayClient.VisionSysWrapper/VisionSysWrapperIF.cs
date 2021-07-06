using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Runtime.InteropServices;
using ZY.Vision.Algorithm;
using System.Windows.Forms;

namespace XRayClient.VisionSysWrapper
{
    public class VisionSysWrapperIF
    {
        private static VisionSysWrapper _visionSysWrapper = new VisionSysWrapper();
        private static List<string> _camList = new List<string>();
        private static List<bool> _camStatusList = new List<bool>();

        public static List<string> CamList
        {
            get { return _camList; }
        }

        public static List<bool> CamStatusList
        {
            get { return _camStatusList; }
        }

        /// <summary>
        /// 添加一个相机到系统中
        /// </summary>
        /// <param name="pcameraparams">相机的参数</param>
        /// <returns></returns>
        public static int AddCamera(CameraParams pcameraparams)
        {
            try
            {
                return VisionSysWrapper.VisionSysNativeIF_Add(ref pcameraparams);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return -99;
            }
            //throw new Exception();
        }

        /// <summary>
        /// 平板初始化
        /// </summary>
        /// <param name="iResults"></param>
        /// <param name="isHigh">是否High模式</param>
        /// <returns></returns>
        public static int CameraInit(out int[] iResults, bool isHigh)
        {
            iResults = new int[10];
            for (int i = 0; i < 10; i++)
            {
                iResults[i] = -1;
            }

            _camList = GetDeviceList();
            int ret = VisionSysWrapper.VisionSysNativeIF_Init(iResults, isHigh);
            foreach (var x in iResults)
            {
                _camStatusList.Add(x == 0 ? true : false);
            }

            return ret;
        }

        /// <summary>
        /// 相机反初始化
        /// </summary>
        /// <returns></returns>
        public static int CameraUnInit()
        {
            return VisionSysWrapper.ImageProcessNativeIF_UnInit();
        }

        /// <summary>
        /// 获取相机的曝光时间
        /// </summary>
        /// <param name="cameraIndex">相机的序号</param>
        /// <returns></returns>
        public static int GetExposureTime(ECamTypes camType)
        {
            return VisionSysWrapper.VisionSysNativeIF_GetExposure((int)camType);
        }

        /// <summary>
        /// 设置相机的曝光时间
        /// </summary>
        /// <param name="cameraIndex">相机的序号</param>
        /// <param name="nvalue">曝光时间</param>
        /// <returns></returns>
        public static int SetExposureTime(ECamTypes camType, int nvalue)
        {
            return VisionSysWrapper.VisionSysNativeIF_SetExposure((int)camType, nvalue);
        }

        /// <summary>
        /// 获取相机的增益
        /// </summary>
        /// <param name="cameraIndex">相机的序号</param>
        /// <returns></returns>
        public static int GetGain(ECamTypes camType)
        {
            return VisionSysWrapper.VisionSysNativeIF_GetGain((int)camType);
        }

        /// <summary>
        /// 获取设备列表
        /// </summary>
        /// <returns></returns>
        public static List<string> GetDeviceList()
        {
            List<string> deviceList = new List<string>();

            IntPtr snBuffer = Marshal.AllocHGlobal(20 * 10);

            int iRet = VisionSysWrapper.VisionSysNativeIF_GetAllSerioNum(snBuffer, 20, 10);
            if (iRet != 0)
            {
                if (IntPtr.Zero != snBuffer)
                {
                    Marshal.FreeHGlobal(snBuffer);
                }
                return deviceList;
            }

            IntPtr pt = snBuffer;
            string sn = string.Empty;
            while (true)
            {
                string str = Marshal.PtrToStringAnsi(pt);

                if (str == string.Empty)
                {
                    break;
                }

                pt = pt + 20;
                deviceList.Add(str);
            }

            if (IntPtr.Zero != snBuffer)
            {
                Marshal.FreeHGlobal(snBuffer);
            }
            return deviceList;
        }

        /// <summary>
        /// 设置相机的增益
        /// </summary>
        /// <param name="cameraIndex">相机的序号</param>
        /// <param name="nvalue">增益</param>
        /// <returns></returns>
        public static int SetGain(ECamTypes camType, int nvalue)
        {
            return VisionSysWrapper.VisionSysNativeIF_SetGain((int)camType, nvalue);
        }

        /// <summary>
        /// 拍照获取一张原图
        /// </summary>
        /// <returns></returns>
        public static int ShotOne(ECamTypes camType, ref ZYImageStruct img)
        {
            int iAlgoType = 4;
            int imageno = 0;
            return VisionSysWrapper.VisionSysNativeIF_shot((int)camType, 1, ref img, iAlgoType, out imageno);
        }

        private static object objLock = new object();

        /// <summary>
        /// 拍摄一系列图片，并返回多帧平均后的图
        /// </summary>
        /// <param name="shotNum"></param>
        /// <param name="camType"></param>
        /// <returns></returns>
        public static int ShotAndAverage(ECamTypes camType, int avgNum, ref ZYImageStruct img, int iAlgoType, out int imgNO)
        {
            //lock (objLock)
            //{
            return VisionSysWrapper.VisionSysNativeIF_shot((int)camType, avgNum, ref img, iAlgoType, out imgNO);
            //}
        }
        #region - ALGO -

        /// <summary>
        /// 算法初始化
        /// </summary>
        public static bool InitAlgo()
        {
            int iRet = VisionSysWrapper.ImageProcessNativeIF_Init();
            return (iRet == 0);
        }

        /// <summary>
        /// 算法反初始化
        /// </summary>
        public static bool UnInitAlgo()
        {
            int iRet = VisionSysWrapper.ImageProcessNativeIF_UnInit();
            return (iRet == 0);
        }

        /// <summary>
        /// 检测前侧图
        /// </summary>
        /// <param name="param"></param>
        /// <param name="frontImage"></param>
        /// <returns></returns>
        public static int InspectPre(ref InspectParams param, ref ImageTransfor frontImage)
        {
            return VisionSysWrapper.ImageProcessNativeIF_InspectPre(ref param, ref frontImage);
        }

        /// <summary>
        /// 检测后侧图
        /// </summary>
        /// <param name="param"></param>
        /// <param name="frontImage"></param>
        /// <returns></returns>
        public static int InspectBack(ref InspectParams param, ref ImageTransfor backImage)
        {
            return VisionSysWrapper.ImageProcessNativeIF_InspectBack(ref param, ref backImage);
        }

        /// <summary>
        /// 获取检测结果
        /// </summary>
        /// <param name="preResData"></param>
        /// <param name="backResData"></param>
        /// <returns></returns>
        public static int GetInspectResult(ref ZYResultData preResData, ref ZYResultData backResData)
        {
            VisionSysWrapper.ImageProcessNativeIF_InspectResultMin(preResData.vecDis, preResData.vecPairDis, preResData.vecAngles, ref preResData.resultDataMin,
                                                                    backResData.vecDis, backResData.vecPairDis, backResData.vecAngles, ref backResData.resultDataMin);

            return 0;
        }

        /// <summary>
        /// 获取拼接结果图片
        /// </summary>
        /// <param name="image1"></param>
        /// <param name="image2"></param>
        /// <param name="image3"></param>
        /// <param name="image4"></param>
        /// <param name="imagecenter"></param>
        /// <param name="Resultflag"></param>
        /// <param name="barcode"></param>
        /// <param name="ReslutImage"></param>
        public static int GetResultImage(ref ZYImageStruct image1, ref ZYImageStruct image2, ref ZYImageStruct image3, ref ZYImageStruct image4,
                                                                       ref ZYImageStruct imagecenter, bool Resultflag, string barcode, ref ZYImageStruct ReslutImage)
        {
            VisionSysWrapper.ImageProcessNativeIF_GetResultImage(ref image1, ref image2, ref image3, ref image4, ref imagecenter, Resultflag, barcode, ref ReslutImage);

            return 0;
        }

        /// <summary>
        /// 深度学习算法初始化
        /// </summary>
        public static bool InitAlgo_DL(ref InspectParams paramsinpect)
        {
            try
            {
                int iRet = VisionSysWrapper.ImageProcessNativeIF_Init_DL(ref paramsinpect);
                return (iRet == 0);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return false;
            }
          
        }

        /// <summary>
        /// tiff算法检测
        /// TODO: 修改参数类型  ZhangKF 2021-3-16
        /// </summary>
        public static bool ImageProcessInspect_DL(string barcode, ref InspectParams paramsinpect, ref ImageTransfor image, float[] vecDis, PairDis[] vecPairDist, float[] vecAngs, ref ZYResultDataMin ResData, int iMethode, int imgNo)
        {
            int iRet = VisionSysWrapper.ImageProcessNativeIF_Inspect_DL(barcode, ref paramsinpect, ref image, vecDis, vecPairDist, vecAngs, ref ResData, iMethode, imgNo);
            return (iRet == 1);
        }

        /// <summary>
        /// 深度学习算法检测
        /// </summary>
        public static bool ImageProcessInspect_DL(string barcode, ref InspectParams paramsinpect, ref ImageTransfor image, float[] vecDis, PairDis[] vecPairDist, float[] vecAngs, ref ZYResultDataMin ResData)
        {
            int iRet = VisionSysWrapper.ImageProcessNativeIF_Inspect_DL(barcode, ref paramsinpect, ref image, vecDis, vecPairDist, vecAngs, ref ResData);
            return (iRet == 1);
        }

        #endregion

    }
}
