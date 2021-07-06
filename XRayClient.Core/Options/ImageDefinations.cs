using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XRayClient.Core
{
    /// <summary>
    /// 图片参数定义
    /// 用于内存分配
    /// </summary>
    public static class ImageDefinations
    {
        public static readonly int MaxImgWidth = 1600;  // 最大图片宽度(预留内存)
        public static readonly int MaxImgHeight = 1600;

        public static readonly int ActualWidth = 864;  // 实际相机图片宽度
        public static readonly int ActualHeight = 1536;
    }
}
