using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using ZY.Vision.Algorithm;

namespace XRayClient.VisionSysWrapper
{
    /// <summary>
    /// 视觉系统包裹类
    /// 封装C++的视觉系统接口对外提供服务
    /// </summary>
    public class VisionSysWrapper
    {
        [DllImport("XRayClient.VisionSysNative.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static void VisionSysNativeIF_SetSreachTable(int cameraIndex, int minGray, int maxGray);

        [DllImport("XRayClient.VisionSysNative.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static int VisionSysNativeIF_Add(ref CameraParams pVisionSysParamsList);

        [DllImport("XRayClient.VisionSysNative.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static int VisionSysNativeIF_Init(int[] results, bool isHigh);

        [DllImport("XRayClient.VisionSysNative.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static int VisionSysNativeIF_UnInit();

        [DllImport("XRayClient.VisionSysNative.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static int VisionSysNativeIF_shot(int frontcameraIndex, int imgcount, ref ZYImageStruct frontpOutImage, int iAlgoType,out int imgNo);

        [DllImport("XRayClient.VisionSysNative.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static int VisionSysNativeIF_GetExposure(int cameraIndex);

        [DllImport("XRayClient.VisionSysNative.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static int VisionSysNativeIF_SetExposure(int cameraIndex, int nvalue);

        [DllImport("XRayClient.VisionSysNative.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static int VisionSysNativeIF_GetGain(int cameraIndex);

        [DllImport("XRayClient.VisionSysNative.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static int VisionSysNativeIF_SetGain(int cameraIndex, int nvalue);

        [DllImport("XRayClient.VisionSysNative.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static int VisionSysNativeIF_GetAllSerioNum(IntPtr cameraIndex, int width, int height);

        [DllImport("XRayClient.VisionSysNative.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static int ImageProcessNativeIF_Init();

        [DllImport("XRayClient.VisionSysNative.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static int ImageProcessNativeIF_UnInit();

        [DllImport("XRayClient.VisionSysNative.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static int ImageProcessNativeIF_InspectPre(ref InspectParams paramsinpect, ref ImageTransfor image);

        [DllImport("XRayClient.VisionSysNative.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static int ImageProcessNativeIF_InspectBack(ref InspectParams paramsinpect, ref ImageTransfor image);

        //[DllImport("XRayClient.VisionSysNative.dll", CallingConvention = CallingConvention.Cdecl)]
        //public extern static void ImageProcessNativeIF_InspectResult(ref ZYResultData preResData, ref ZYResultData backResData);

        [DllImport("XRayClient.VisionSysNative.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static void ImageProcessNativeIF_InspectResultMin(float[] vecDisPre, PairDis[] vecPairDistPre, float[] vecAngsPre, ref ZYResultDataMin preResData,
                                                                        float[] vecDisBack, PairDis[] vecPairDistBack, float[] vecAngsBack, ref ZYResultDataMin backResData);

        [DllImport("XRayClient.VisionSysNative.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static void ImageProcessNativeIF_GetResultImage(ref ZYImageStruct image1, ref ZYImageStruct image2, ref ZYImageStruct image3, ref ZYImageStruct image4,
                                                                        ref ZYImageStruct imagecenter, bool Resultflag, string barcode, ref ZYImageStruct ReslutImage);
																		
		[DllImport("XRayClient.VisionSysNative.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static int ImageProcessNativeIF_SaveImage(ref ZYImageStruct image, string filePath, int iType,int imagNo);

        [DllImport("XRayClient.VisionSysNative.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static int ImageProcessNativeIF_Init_DL(ref InspectParams paramsinpect);

        [DllImport("XRayClient.VisionSysNative.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static int ImageProcessNativeIF_Inspect_DL(string barcode, ref InspectParams paramsinpect, ref ImageTransfor image, float[] vecDis, PairDis[] vecPairDist, float[] vecAngs, ref ZYResultDataMin ResData, int iMethode,int imagNo);

        [DllImport("needle_detect.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public extern static bool CshapeNeedleLengthDetect(ref float length, ref float pixel_to_mm, bool isCheck);

        [DllImport("XRayClient.ImageProcessNative.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static int ImageProcessNativeIF_Inspect_DL(string barcode, ref InspectParams paramsinpect, ref ImageTransfor image, float[] vecDis, PairDis[] vecPairDist, float[] vecAngs, ref ZYResultDataMin ResData);

    }
}
