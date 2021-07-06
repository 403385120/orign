using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using ZY.Vision;

namespace ZY.Vision.Algorithm
{
    /// <summary>
    /// 对图像进行加工处理，如多帧平均、转8位图等
    /// ZhangKF 2020-12-28
    /// </summary>
    public class ImageProcess
    {
        public ImageProcess()
        {
        }

        ///<summary>对多张位图进行多帧平均运算(仅限处理16位图)</summary>
        public static Bitmap Average(Bitmap[] bitmaps)
        {
            Bitmap averageBitmap = null;

            List<ushort[]> list = new List<ushort[]>();
            for (var i = 0; i < bitmaps.Length; i++)
            {
                var data = bitmaps[i].ToBytes_16();
                list.Add(data);
            }

            if (list.Count > 0)
            {
                //汇总各像素点的值 ZhangKF 2021-2-3
                ulong[] longData = new ulong[list[0].Length];
                for (var i = 0; i < list.Count; i++)
                {
                    var data = list[i];
                    for (var j = 0; j < data.Length; j++)
                    {
                        ulong avg = longData[j] + data[j];
                        //溢出检查
                        if (avg > ulong.MaxValue)
                        {
                            longData[j] = ulong.MaxValue;
                        }
                        else
                        {
                            longData[j] = avg;
                        }
                    }
                }

                ushort[] merageData = new ushort[list[0].Length];
                for (var i = 0; i < longData.Length; i++)
                {
                    var avg = longData[i] / (ulong)list.Count;
                    if (avg > ushort.MaxValue)
                    {
                        merageData[i] = ushort.MaxValue;
                    }
                    else
                    {
                        merageData[i] = (ushort)avg;
                    }
                }

                averageBitmap = new Bitmap(bitmaps[0].Width, bitmaps[0].Height, PixelFormat.Format16bppGrayScale);
                Rectangle rect = new Rectangle(0, 0, bitmaps[0].Width, bitmaps[0].Height);
                BitmapData bitmapData = averageBitmap.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format16bppGrayScale);
                Share.CopyFromArray(merageData, 0, bitmapData.Scan0, longData.Length * sizeof(ushort));
                averageBitmap.UnlockBits(bitmapData);
            }

            return averageBitmap;
        }

        ///<summary>对多张位图进行多帧平均运算(仅限处理8位图)</summary>
        public static Bitmap Average8(Bitmap[] bitmaps)
        {
            Bitmap averageBitmap = null;

            List<byte[]> list = new List<byte[]>();
            for (var i = 0; i < bitmaps.Length; i++)
            {
                var data = bitmaps[i].ToBytes_8();
                list.Add(data);
            }

            if (list.Count > 0)
            {
                byte[] merageData = new byte[list[0].Length];
                ushort[] umergeData = new ushort[list[0].Length];
                for (var i = 0; i < list.Count; i++)
                {
                    var data = list[i];
                    for (var j = 0; j < data.Length; j++)
                    {
                        umergeData[j] += data[j];
                    }
                }

                for (var i = 0; i < umergeData.Length; i++)
                {
                    merageData[i] = (byte)(umergeData[i] / list.Count);
                }

                averageBitmap = new Bitmap(bitmaps[0].Width, bitmaps[0].Height, PixelFormat.Format8bppIndexed);
                Rectangle rect = new Rectangle(0, 0, bitmaps[0].Width, bitmaps[0].Height);
                BitmapData bitmapData = averageBitmap.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format8bppIndexed);
                Share.CopyFromArray(merageData, 0, bitmapData.Scan0, merageData.Length * sizeof(byte));
                averageBitmap.UnlockBits(bitmapData);
            }

            return averageBitmap;
        }

        ///<summary>字节平均算法(仅针对8位图)</summary>
        public static Bitmap AverageBytes(List<byte[]> list, int width, int height)
        {
            Bitmap averageBitmap = null;

            //List<byte[]> list = new List<byte[]>();
            //for (var i = 0; i < bitmaps.Length; i++)
            //{
            //    var data = bitmaps[i].ToBytes_8();
            //    list.Add(data);
            //}

            if (list.Count > 0)
            {
                byte[] merageData = new byte[list[0].Length];
                ushort[] umergeData = new ushort[list[0].Length];
                for (var i = 0; i < list.Count; i++)
                {
                    var data = list[i];
                    for (var j = 0; j < data.Length; j++)
                    {
                        umergeData[j] += data[j];
                    }
                }

                for (var i = 0; i < umergeData.Length; i++)
                {
                    merageData[i] = (byte)(umergeData[i] / list.Count);
                }

                averageBitmap = new Bitmap(width, height, PixelFormat.Format8bppIndexed);
                Rectangle rect = new Rectangle(0, 0, width, height);
                BitmapData bitmapData = averageBitmap.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format8bppIndexed);
                Share.CopyFromArray(merageData, 0, bitmapData.Scan0, merageData.Length * sizeof(byte));
                averageBitmap.UnlockBits(bitmapData);
            }

            return averageBitmap;
        }
        //end class
    }
}
