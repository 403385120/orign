using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XRayClient.VisionSysWrapper;
using System.Drawing;
using System.Runtime.InteropServices;

namespace XRayClient.Core
{
    /// <summary>
    /// 单实例算法拓展类   MTW算法
    /// </summary>
    public class AlgoWrapperExt
    {
        /// <summary>
        /// 图片锐化
        /// </summary>
        /// <param name="SrcImage">输入图像</param>
        /// <param name="DstImage">输出图像</param>
        /// <param name="blur_width"、"blur_height">滤波尺寸, 一般不需要改动，取默认值</param>
        /// <param name="enhance_ratio">增强阈值，默认是0，增大该值，锐化效果越强，噪声水平也会增强</param>
        [DllImport("Xray_enhance_Dll.dll", EntryPoint = "?PicToEnhance@@YAXPAUtagZYImageStruct@@0HHH@Z", CallingConvention = CallingConvention.Cdecl)]
        public static extern void PicToEnhance(ref ZYImageStruct SrcImage, ref ZYImageStruct DstImage, int blur_width = 15, int blur_height = 3, int enhance_ratio = 0);


        public struct stInputParams
        {
            public int detect_height;    //检测高度,
            public int total_layer;      //层数
            public bool fold_inspection; //是否褶皱检测
            public float pixel_to_mm;    //像素和实际长度系数
            public float min_length;
            public float max_length;
        };
    }
}
