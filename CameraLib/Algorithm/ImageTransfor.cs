using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using ZY.Vision.Utils;

namespace ZY.Vision.Algorithm
{
    /// <summary>
    /// 传给算法结构体交换对象,延用旧版ImageStruct对象
    /// ZhangKF 2020-12-24
    /// </summary>
    //[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct ImageTransfor
    {
        #region API
        [DllImport("XRayClient.ImageProcessNative.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static int ImageProcessNativeIF_SaveImage(ref ImageTransfor image, string filePath);
        #endregion

        ///<summary>图像数据(指针)</summary>
        public IntPtr data;

        ///<summary>图像宽度</summary>
        public int width;

        ///<summary>图像高度</summary>
        public int height;

        ///<summary>图像层数</summary>
        public int channel;

        ///<summary>克隆当前对象(仅限8位图)</summary>
        public ImageTransfor Clone()
        {
            int size = this.width * this.height;
            byte[] bytes = new byte[size];

            //从内存复制数据到数组
            Marshal.Copy(this.data, bytes, 0, size);

            IntPtr newData = Marshal.AllocHGlobal(size);
            //记录待释放对象
            //PointerRelease.Record(newData);

            //复制数据到内存
            Marshal.Copy(bytes, 0, newData, size);

            ImageTransfor transfor = new ImageTransfor();
            transfor.data = newData;
            transfor.width = this.width;
            transfor.height = this.height;
            transfor.channel = this.channel;

            return transfor;
        }

        ///<summary>保存图片</summary>
        //public bool Save(string fileName, Consts.ImageTypes imgType)
        //{
        //    if (this.width <= 0 || this.height <= 0 || this.channel <= 0)
        //    {
        //        return false;
        //    }

        //    //int iRet = ImageProcessNativeIF_SaveImage(ref this, fileName);
        //    //if (iRet != 0)
        //    //{
        //    //    throw new Exception("保存文件失败:" + fileName);
        //    //}

        //    byte[] bytes = null;
        //    if (imgType == Consts.ImageTypes.Eight)
        //    {
        //        bytes = new byte[width * height];
        //    }
        //    else
        //    {
        //        bytes = new byte[width * height * 2];
        //    }

        //    Share.CopyToArray(this.data, bytes, 0, bytes.Length);

        //    Bitmap newImg = new Bitmap(width, height, PixelFormat.Format16bppGrayScale);
        //    Rectangle rect = new Rectangle(0, 0, newImg.Width, newImg.Height);
        //    BitmapData data = newImg.LockBits(rect, ImageLockMode.ReadWrite, newImg.PixelFormat);
        //    Share.CopyFromArray(bytes, 0, data.Scan0, bytes.Length);
        //    newImg.UnlockBits(data);
        //    if (imgType== Consts.ImageTypes.Eight)
        //    {
        //        newImg.SaveToJPG(fileName);
        //    }
        //    else
        //    {
        //        newImg.SaveToTIF(fileName);
        //    }
            
        //    return true;
            
        //}

        ///<summary>保存图片(16位图时保存为TIF格式)</summary>
        public bool Save(string fileName, Consts.ImageTypes imgType)
        {
            if (this.width <= 0 || this.height <= 0 || this.channel <= 0)
            {
                return false;
            }

            //int iRet = ImageProcessNativeIF_SaveImage(ref this, fileName);
            //if (iRet != 0)
            //{
            //    throw new Exception("保存文件失败:" + fileName);
            //}

            if (!fileName.ToLower().Trim().EndsWith(".jpg"))
            {
                throw new Exception("文件名必须以jpg格式结尾!");
            }

            byte[] bytes = null;
            if (imgType == Consts.ImageTypes.Eight)
            {
                bytes = new byte[width * height];
            }
            else if (imgType == Consts.ImageTypes.Sixteen)
            {
                bytes = new byte[width * height * 2];
            }

            Share.CopyToArray(this.data, bytes, 0, bytes.Length);

            Bitmap newImg = null;

            if (imgType == Consts.ImageTypes.Eight)
            {
                newImg = new Bitmap(width, height, PixelFormat.Format8bppIndexed);
            }
            else if (imgType == Consts.ImageTypes.Sixteen)
            {
                newImg = new Bitmap(width, height, PixelFormat.Format16bppGrayScale);
            }

            Rectangle rect = new Rectangle(0, 0, newImg.Width, newImg.Height);
            BitmapData data = newImg.LockBits(rect, ImageLockMode.ReadWrite, newImg.PixelFormat);
            Share.CopyFromArray(bytes, 0, data.Scan0, bytes.Length);
            newImg.UnlockBits(data);

            //16位图时，同时保存TIF格式
            if (imgType == Consts.ImageTypes.Sixteen)
            {
                //TODO: 算法冼工要求不需要旋转图片 ZhangKF 2021-4-3
                //var rotateBmp = newImg.Rotate2();
                string tifFileName = fileName.ToUpper().Replace(".JPG", ".TIF");
                newImg.SaveToTIF(tifFileName);
            }
            else if (imgType == Consts.ImageTypes.Eight)
            {
                newImg.SaveToJPG(fileName);
            }

            return true;
        }

        ///<summary>手动释放内存</summary>
        public void Destory()
        {
            //TODO：采用循环内存的使用方式，不需要手工释放 ZhangKF 2021-1-8
            //if (IntPtr.Zero != this.data)
            //{
            //    Marshal.FreeHGlobal(data);
            //    this.data = IntPtr.Zero;
            //}

            //TODO: 清空图片数据 ZhangKF 2021-3-14
            this.data = IntPtr.Zero;
        }
        //end struct
    }
}
