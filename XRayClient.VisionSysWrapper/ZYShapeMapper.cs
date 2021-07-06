using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Drawing;

namespace XRayClient.VisionSysWrapper
{
    internal class MemHelper
    {
        [DllImport("kernel32.dll", EntryPoint = "CopyMemory", SetLastError = false)]
        public static extern void CopyMemory(IntPtr dest, IntPtr src, uint count);
    }

    /**
      * @brief 点
      */
    public struct PointStruct
    {
        public float x;
        public float y;
    }


    // /**
    //  * @brief 向量 
    //  */
    // #define VectorStruct PointStruct

    /**
     * @brief 线段
     */
    public struct SegStruct
    {
        public PointStruct ps;     // 起点
        public PointStruct pe;     // 终点
    }

    /**
      * @brief 圆
      */
    public struct CircleStruct
    {
        public PointStruct pc;     // 圆心
        public float r;            // 半径
    }

    /**
     * @brief 矩形
     */
    public struct RectStruct
    {
        public float x;
        public float y;
        public float width;
        public float height;
    }

    /**
     * @brief 旋转矩形
     */
    public struct RotRectStruct
    {
        public RectStruct rect;
        public float angle;   // 绕起点旋转角度,顺时针为负

    }

    /// <summary>
    /// 具有自动内存管理的ZYBitMapStruct包裹类
    /// </summary>
    public class ZYBitMapClass
    {
        public ZYImageStruct BitMapStruct;

        public ZYBitMapClass(Bitmap bitMap)
        {
            this.BitMapStruct = new ZYImageStruct(bitMap);
        }

        ~ZYBitMapClass()
        {
            this.BitMapStruct.Destory();
        }
    }

    /**
      * @brief 图像数据
      * @note width没有像素对齐，拷贝数据时注意数据宽度
      *       必须手动回收内存！
      */
    public struct ZYImageStruct
    {
        public IntPtr data;   // 数据
        public int width;
        public int height;
        public int channel;
        public int imgNo;

        /// <summary>
        /// 构造函数，将Bitmap数据拷贝到内存中以便C++调用
        /// </summary>
        /// <param name="bitMap"></param>
        public ZYImageStruct(Bitmap bitMap)
        {
            this.width = bitMap.Width;
            this.height = bitMap.Height;
            this.channel = Image.GetPixelFormatSize(bitMap.PixelFormat) / 8;
            this.imgNo = 0;

            var rect = new Rectangle(0, 0, bitMap.Width, bitMap.Height);
            var bmpData = bitMap.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadOnly, bitMap.PixelFormat);

            var imgBytes = bmpData.Stride * bmpData.Height;

            this.data = Marshal.AllocHGlobal(imgBytes);

            byte[] rgbValues = new byte[imgBytes];
            IntPtr ptr = bmpData.Scan0;
            for (var i = 0; i < bitMap.Height; i++)
            {
                Marshal.Copy(ptr, rgbValues, i * bmpData.Width * channel, bmpData.Width * channel);   //对齐  
                ptr += bmpData.Stride; // next row  
            }
            Marshal.Copy(rgbValues, 0, this.data, imgBytes);

            bitMap.UnlockBits(bmpData);
        }

        /// <summary>
        /// 使用Bitmap更新图片数据
        /// </summary>
        /// <param name="bitMap"></param>
        public void UpdateWithBitmap(Bitmap bitMap)
        {
            if (!(bitMap.Width == this.width && bitMap.Height == this.height
                && Image.GetPixelFormatSize(bitMap.PixelFormat) / 8 == this.channel))
            {
                this.Destory();
                this = new ZYImageStruct(bitMap);
                return;
            }

            // 如果已分配内存则再次使用
            var rect = new Rectangle(0, 0, bitMap.Width, bitMap.Height);
            var bmpData = bitMap.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadOnly, bitMap.PixelFormat);

            var imgBytes = bmpData.Stride * bmpData.Height;

            //this.data = Marshal.AllocHGlobal(imgBytes);

            byte[] rgbValues = new byte[imgBytes];
            IntPtr ptr = bmpData.Scan0;
            for (var i = 0; i < bitMap.Height; i++)
            {
                Marshal.Copy(ptr, rgbValues, i * bmpData.Width * channel, bmpData.Width * channel);   //对齐  
                ptr += bmpData.Stride; // next row  
            }
            Marshal.Copy(rgbValues, 0, this.data, imgBytes);

            bitMap.UnlockBits(bmpData);

        }

        /// <summary>
        /// 内存级拷贝
        /// </summary>
        /// <returns></returns>
     /*   public ZYImageStruct DeepCopy()
        {
            ZYImageStruct img = new ZYImageStruct();
            img.width = this.width;
            img.height = this.height;
            img.channel = this.channel;

            int strip = (int)Math.Ceiling(((float)this.width * (float)this.channel) / 4) * 4;
            int size = strip * this.height;

            if (size > 0)
            {
                img.data = Marshal.AllocHGlobal(size);
                MemHelper.CopyMemory(img.data, this.data, (uint)size);
            }
            else
            {
                img.data = IntPtr.Zero;
            }

            return img;
        }*/

        /// <summary>
        /// 将图像拷贝到另一张图像上
        /// </summary>
        /// <param name="preAllocImg">预分配图像(大小和原始图一一致)</param>
        /// <returns></returns>
        public void CopyTo(ref ZYImageStruct preAllocImg)
        {
            if (preAllocImg.width != this.width || preAllocImg.height != this.height
                || preAllocImg.channel != this.channel
                || preAllocImg.data == IntPtr.Zero || this.data == IntPtr.Zero)
            {
                throw new Exception("Invalid Copy!");
            }

            int size = this.width * this.height * this.channel;
            MemHelper.CopyMemory(preAllocImg.data, this.data, (uint)size);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public bool Save(string filePath, int iType = 0, int imagNo = 0)
        {
            if (this.width <= 0 || this.height <= 0 || this.channel <= 0)
            {
                return false;
            }
            object lockObj = new object();
            lock (lockObj)
            {
                int iRet = VisionSysWrapper.ImageProcessNativeIF_SaveImage(ref this, filePath, iType, imagNo);
                if (iRet != 0)
                {
                    throw new Exception("Save image failed: " + filePath); //XRAY GRR 更换电池时出现异常
                }
            }

            return true;
        }

        /// <summary>
        /// 转换成Bitmap图片
        /// </summary>
        /// <returns></returns>
        public Bitmap ToBitmap()
        {
            if (this.width <= 0 || this.height <= 0 || this.channel <= 0)
            {
                return null;
            }

            return ZImageConverter.ZYImageStruct2Bitmap(this);
        }

        /// <summary>
        /// 创建图像，分配内存
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="channel"></param>
        public void Create(int width, int height, int channel)
        {
            this.width = width;
            this.height = height;
            this.channel = channel;
            this.imgNo = 0;

            int strip = (int)Math.Ceiling(((float)this.width * (float)this.channel) / 4) * 4;
            int size = strip * this.height;

            if (size > 0)
            {
                this.data = Marshal.AllocHGlobal(size);
            }
        }

        /// <summary>
        /// 模拟析构，销毁内存中的数据，注意使用完后手动调用回收
        /// 这个函数不会回收VM，造成内存不足异常！！！
        /// </summary>
        public void Destory()
        {
            try
            {
                if (IntPtr.Zero != this.data)
                {
                    //int x = 0;
                    Marshal.FreeHGlobal(data);
                    this.data = IntPtr.Zero;
                }
            }
            catch (Exception)
            {

            }
        }
    }
}
;
