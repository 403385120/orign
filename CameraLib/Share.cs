using BitMiracle.LibTiff.Classic;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ZY.Logging;
using ZY.Vision.Algorithm;
using ZY.Vision.Utils;
using ZY.Vision.Varex;
using static ZY.Vision.Consts;

namespace ZY.Vision
{
    /// <summary>
    /// 公共静态方法
    /// ZhangKF 2020-12-22
    /// </summary>
    public static class Share
    {
        volatile static int _dict_point_index = 0;
        static int _log_level = -1;
        static ConcurrentDictionary<int, IntPtr> _dict_points = new ConcurrentDictionary<int, IntPtr>();

        [DllImport("kernel32.dll", SetLastError = false)]
        static extern void CopyMemory(IntPtr destination, IntPtr source, UIntPtr length);

        ///<summary>记录错误日志</summary>
        public static void Error(string msg)
        {
            int logLevel = LogLevel();
            if (logLevel > 0)
            {
                //_logger.Log(msg, Logging.LogLevels.Error, string.Empty);
                LoggingIF.Log(msg, Logging.LogLevels.Error, "Share");
            }
        }

        ///<summary>日志记录等级</summary>
        private static int LogLevel()
        {
            if (_log_level == -1)
            {
                string val = CameraParameter.ReadConfigItem(Consts.HARDWARE_CONFIG_CAMERA, Consts.HARDWARE_CONFIG_LOG_LEVEL);
                if (!string.IsNullOrEmpty(val))
                    int.TryParse(val, out _log_level);
            }
            return _log_level;
        }

        ///<summary>记录调试信息</summary>
        public static void Info(string msg)
        {
            int logLevel = LogLevel();
            if (logLevel > 1)
            {
                //_logger.Log(msg, Logging.LogLevels.Info, string.Empty);
                LoggingIF.Log(msg, Logging.LogLevels.Info, "Share");
            }
        }

        ///<summary>记录日志</summary>
        public static void Log(string msg)
        {
            LoggingIF.Log(msg, Logging.LogLevels.Info, "Share");
        }

        ///<summary>将图片对象Bitmap转换成字节数组(16位字节)</summary>
        public static ushort[] ToBytes_16(this Bitmap bitmap)
        {
            Share.Info("位图转16位字节数组 开始(Share.ToBytes_16)");
            Stopwatch watch = new Stopwatch();
            watch.Start();

            if (bitmap.PixelFormat != System.Drawing.Imaging.PixelFormat.Format16bppGrayScale)
            {
                throw new Exception("当前位图不为16位格式");
            }

            //字节数组长度
            int size = bitmap.Width * bitmap.Height * sizeof(ushort);
            int count = bitmap.Width * bitmap.Height;
            ushort[] bytes = new ushort[count];

            Rectangle rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
            //内存锁定
            BitmapData data = bitmap.LockBits(rect, ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format16bppGrayScale);

            //取得图象的首地址
            IntPtr ptr = data.Scan0;

            //复制被锁定的位图图象到数组内
            CopyToArray(ptr, bytes, 0, size);

            //解锁位图数据
            bitmap.UnlockBits(data);

            watch.Stop();
            Share.Info(string.Format("位图转16位数组耗时：{0}", watch.ElapsedMilliseconds));

            return bytes;
        }

        ///<summary>将图片对象Bitmay转换成字节数组(8位字节)(支持16位和8位图)</summary>
        public static byte[] ToBytes_8(this Bitmap bitmap)
        {
            Share.Info("位图转8位数组 开始(Share.ToBytes_8)");
            Stopwatch watch = new Stopwatch();

            byte[] bytes = null;
            if (bitmap.PixelFormat == System.Drawing.Imaging.PixelFormat.Format16bppGrayScale)
            {
                Short2Byte convertor = new Short2Byte(bitmap);
                bytes = convertor.ToBytes();
            }
            else if (bitmap.PixelFormat == System.Drawing.Imaging.PixelFormat.Format8bppIndexed)
            {
                Rectangle rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
                //内存锁定
                BitmapData data = bitmap.LockBits(rect, ImageLockMode.ReadOnly, bitmap.PixelFormat);

                //取得图象的首地址
                IntPtr ptr = data.Scan0;

                //24位图的字节数
                int byteSize = bitmap.Width * bitmap.Height;

                //定义位图数组
                bytes = new byte[byteSize];

                //复制被锁定的位图图象到数组内
                Marshal.Copy(ptr, bytes, 0, byteSize);

                //解锁位图数据
                bitmap.UnlockBits(data);
            }

            watch.Stop();
            Share.Info(string.Format("位图转8位数组耗时:{0}", watch.ElapsedMilliseconds));

            return bytes;
        }

        ///<summary>数组转换成指针(必须手动释放内存)</summary>
        public static IntPtr GetIntPtr<T>(T[] arr) where T : struct
        {
            IntPtr ptr = Marshal.AllocHGlobal(arr.Length * Marshal.SizeOf<T>());
            CopyFromArray(arr, 0, ptr, (int)arr.Length * Marshal.SizeOf<T>());
            return ptr;
        }

        ///<summary>对图像作顺时针90度旋转(仅限16位图)</summary>
        [Obsolete]
        public static Bitmap Rotate(this Bitmap bitmap)
        {
            Share.Info("图片旋转 开始(Share.Rotate)");
            Stopwatch watch = new Stopwatch();
            watch.Start();

            Rectangle rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
            //内存锁定
            BitmapData data = bitmap.LockBits(rect, ImageLockMode.ReadWrite, bitmap.PixelFormat);

            //取得图象的首地址
            IntPtr ptr = data.Scan0;

            int arrLength = bitmap.Width * bitmap.Height;

            //定义位图数组
            ushort[] bytes = new ushort[arrLength];
            ushort[] tempBytes = new ushort[arrLength];

            //复制被锁定的位图图象到数组内
            CopyToArray(ptr, bytes, 0, arrLength * sizeof(ushort));

            int newIndex, x, y, x1, y1;
            for (var i = 0; i < bytes.Length; i++)
            {
                double d = i / bitmap.Width;
                y = (int)Math.Floor(d);
                x = i - y * bitmap.Width;

                x1 = bitmap.Height - y - 1;
                y1 = x;

                //新数组索引下标
                newIndex = y1 * bitmap.Height + x1;
                tempBytes[newIndex] = bytes[i];
            }

            bitmap.UnlockBits(data);

            Bitmap newImage = new Bitmap(bitmap.Height, bitmap.Width, bitmap.PixelFormat);
            Rectangle rect2 = new Rectangle(0, 0, newImage.Width, newImage.Height);
            BitmapData newData = newImage.LockBits(rect2, ImageLockMode.ReadWrite, newImage.PixelFormat);
            CopyFromArray(tempBytes, 0, newData.Scan0, tempBytes.Length * sizeof(ushort));
            newImage.UnlockBits(newData);

            watch.Stop();
            Share.Info(string.Format("图片旋转耗时：{0}", watch.ElapsedMilliseconds));

            return newImage;
        }

        ///<summary>对图像作顺时针90度旋转(仅限16位图) 速度优化版本</summary>
        public static Bitmap Rotate2(this Bitmap bitmap)
        {
            Share.Info("图片旋转 开始(Share.Rotate)");
            Stopwatch watch = new Stopwatch();
            watch.Start();

            Rectangle rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
            //内存锁定
            BitmapData data = bitmap.LockBits(rect, ImageLockMode.ReadWrite, bitmap.PixelFormat);

            //取得图象的首地址
            IntPtr ptr = data.Scan0;

            int arrLength = bitmap.Width * bitmap.Height;

            //定义位图数组
            ushort[] bytes = new ushort[arrLength];
            ushort[] tempBytes = new ushort[arrLength];

            //复制被锁定的位图图象到数组内
            CopyToArray(ptr, bytes, 0, arrLength * sizeof(ushort));

            int newIndex, x, y, x1 = 0, y1 = 0;

            int last_y = -1; //当前点的纵坐标
            int last_x1 = -1;
            double d = 0;
            int width = bitmap.Width;
            int height = bitmap.Height;
            for (var i = 0; i < bytes.Length; i++)
            {
                d = i / width;
                y = (int)Math.Floor(d);
                x = i - y * width;


                //TODO:此部分代码主要为优化图像旋转速度，一次处理位于同一行（旋转前）的数据
                if (last_y == y) //同一行
                {
                    for (var j = 1; j < width - 1; j++) //处理旋转前同一行的数据
                    {
                        //x1 = last_x1;
                        //y1 = x + j;
                        i++;
                        //newIndex = y1 * bitmap.Height + x1;
                        newIndex = (x + j) * height + last_x1;
                        tempBytes[newIndex] = bytes[i];
                    }
                }
                else
                {
                    x1 = height - y - 1;
                    y1 = x;
                    //新数组索引下标
                    newIndex = y1 * height + x1;
                    tempBytes[newIndex] = bytes[i];
                }

                last_y = y;
                last_x1 = x1;
            }

            bitmap.UnlockBits(data);

            Bitmap newImage = new Bitmap(bitmap.Height, bitmap.Width, bitmap.PixelFormat);
            Rectangle rect2 = new Rectangle(0, 0, newImage.Width, newImage.Height);
            BitmapData newData = newImage.LockBits(rect2, ImageLockMode.ReadWrite, newImage.PixelFormat);
            CopyFromArray(tempBytes, 0, newData.Scan0, tempBytes.Length * sizeof(ushort));
            newImage.UnlockBits(newData);

            watch.Stop();
            Share.Info(string.Format("图片旋转耗时(Share.Rotate)：{0}", watch.ElapsedMilliseconds));

            return newImage;
        }

        ///<summary>保存Bitmap至位图格式(支持16位或8位图)</summary>
        public static void SaveToBMP(this Bitmap bmp, string file)
        {
            Share.Info("保存BMP图 开始(Share.SaveToBMP)");
            Stopwatch watch = new Stopwatch();
            watch.Start();

            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);

            BitmapData bitmapData = bmp.LockBits(rect, ImageLockMode.ReadOnly, bmp.PixelFormat);

            System.Windows.Media.PixelFormat format = System.Windows.Media.PixelFormats.Gray16;
            if (bmp.PixelFormat == System.Drawing.Imaging.PixelFormat.Format8bppIndexed)
                format = System.Windows.Media.PixelFormats.Gray8;

            BitmapSource source = BitmapSource.Create(bmp.Width,
                                                      bmp.Height,
                                                      bmp.HorizontalResolution,
                                                      bmp.VerticalResolution,
                                                      format,
                                                      null,
                                                      bitmapData.Scan0,
                                                      bitmapData.Stride * bmp.Height,
                                                      bitmapData.Stride);

            bmp.UnlockBits(bitmapData);

            FileStream stream = new FileStream(file, FileMode.Create);

            TiffBitmapEncoder encoder = new TiffBitmapEncoder();

            encoder.Compression = TiffCompressOption.None;
            encoder.Frames.Add(BitmapFrame.Create(source));
            encoder.Save(stream);

            stream.Close();

            watch.Stop();
            Share.Info(string.Format("保存BMP图片耗时：{0}", watch.ElapsedMilliseconds));
        }

        ///<summary>保存Bitmap至TIF格式(16位图格式)</summary>
        public static void SaveToTIF(this Bitmap bmp, string file)
        {
            Share.Info("保存16位TIF开始(Share.SaveToTIF)");
            Stopwatch watch = new Stopwatch();
            watch.Start();

            if (bmp.PixelFormat != System.Drawing.Imaging.PixelFormat.Format16bppGrayScale)
            {
                throw new Exception("当前位图不为16位格式");
            }

            MemoryStream ms = new MemoryStream();
            using (Tiff tif = Tiff.ClientOpen(@"in-memory", "w", ms, new TiffStream()))
            {

                tif.SetField(TiffTag.IMAGEWIDTH, bmp.Width);
                tif.SetField(TiffTag.IMAGELENGTH, bmp.Height);
                tif.SetField(TiffTag.SAMPLESPERPIXEL, 1);
                tif.SetField(TiffTag.BITSPERSAMPLE, 16);
                tif.SetField(TiffTag.ORIENTATION, Orientation.TOPLEFT);
                tif.SetField(TiffTag.ROWSPERSTRIP, bmp.Height);
                //tif.SetField(TiffTag.XRESOLUTION, 88.0);
                //tif.SetField(TiffTag.YRESOLUTION, 88.0);
                tif.SetField(TiffTag.RESOLUTIONUNIT, ResUnit.NONE);
                tif.SetField(TiffTag.PLANARCONFIG, PlanarConfig.CONTIG);
                tif.SetField(TiffTag.PHOTOMETRIC, Photometric.MINISBLACK);
                tif.SetField(TiffTag.COMPRESSION, Compression.NONE);
                //tif.SetField(TiffTag.FILLORDER, FillOrder.MSB2LSB);

                //将16位数组ushort存储到8位byte数组中
                var ushorts = bmp.ToBytes_16();
                byte[] bytes = new byte[ushorts.Length * sizeof(ushort)];
                Buffer.BlockCopy(ushorts, 0, bytes, 0, bytes.Length);

                tif.WriteRawStrip(0, bytes, bytes.Length);
                tif.WriteDirectory();

                FileStream fs = new FileStream(file, FileMode.Create);
                ms.Seek(0, SeekOrigin.Begin);
                fs.Write(ms.ToArray(), 0, (int)ms.Length);
                fs.Close();

                watch.Stop();
                Share.Info(string.Format("保存TIF图耗时:{0}", watch.ElapsedMilliseconds));
            }
        }

        ///<summary>将位图保存为JPEG格式文件(降为8位)</summary>
        public static void SaveToJPG(this Bitmap bitmap, string fileName)
        {
            Share.Info("保存JPG格式图片 开始(Share.SaveToJPG)");
            Stopwatch watch = new Stopwatch();
            watch.Start();

            //Short2Byte convertor = new Short2Byte(bitmap);
            var arrImages = bitmap.ToBytes_8();

            //var bitmap8 = convertor.ToByteImage();
            Bitmap bitmapJpg = new Bitmap(bitmap.Width, bitmap.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            Rectangle rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
            BitmapData imgData = bitmapJpg.LockBits(rect, ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            var byte24 = new byte[arrImages.Length * 3];
            int n = 0;
            for (var i = 0; i < arrImages.Length; i++)
            {
                byte24[n] = arrImages[i];
                n++;
                byte24[n] = arrImages[i];
                n++;
                byte24[n] = arrImages[i];
                n++;
            }
            Marshal.Copy(byte24, 0, imgData.Scan0, byte24.Length);
            bitmapJpg.UnlockBits(imgData);

            bitmapJpg.Save(fileName, ImageFormat.Jpeg);

            watch.Stop();
            Share.Info(string.Format("保存JPG格式图片：{0}", watch.ElapsedMilliseconds));
        }

        ///<summary>将图片转换为C++交换对象</summary>
        [Obsolete]
        public static ImageTransfor ToTransfor_(this Bitmap bitmap)
        {
            Share.Info("转C++交换对象开始(Share.ToTransfor)");
            Stopwatch watch = new Stopwatch();
            watch.Start();

            //转8位再传给算法
            Short2Byte sb = new Short2Byte(bitmap);
            Bitmap bitmap_8 = sb.ToByteImage();

            ImageTransfor transfor = new ImageTransfor();

            transfor.width = bitmap_8.Width;
            transfor.height = bitmap_8.Height;
            transfor.channel = Image.GetPixelFormatSize(bitmap_8.PixelFormat) / 8;

            var rect = new Rectangle(0, 0, bitmap_8.Width, bitmap_8.Height);
            var bmpData = bitmap_8.LockBits(rect, ImageLockMode.ReadOnly, bitmap_8.PixelFormat);

            var imgBytes = bmpData.Stride * bmpData.Height;

            transfor.data = Marshal.AllocHGlobal(imgBytes);
            //PointerRelease.Record(transfor.data); //自动释放记录

            byte[] rgbValues = new byte[imgBytes];
            IntPtr ptr = bmpData.Scan0;
            for (var i = 0; i < bitmap_8.Height; i++)
            {
                Marshal.Copy(ptr, rgbValues, i * bmpData.Width * transfor.channel, bmpData.Width * transfor.channel);
                ptr += bmpData.Stride;
            }
            Marshal.Copy(rgbValues, 0, transfor.data, imgBytes);

            bitmap_8.UnlockBits(bmpData);

            watch.Stop();
            Share.Info(string.Format("转C++交换对象耗时：{0}", watch.ElapsedMilliseconds));

            return transfor;
        }

        ///<summary>将图片转换为C++交换对象(传递电芯序号)</summary>
        [Obsolete]
        public static ImageTransfor ToTransfor(this Bitmap bitmap)
        {
            Share.Info("转C++交换对象开始(Share.ToTransfor)");
            Stopwatch watch = new Stopwatch();
            watch.Start();

            Bitmap bitmap_8 = bitmap;

            //转8位再传给算法
            if (bitmap.PixelFormat == System.Drawing.Imaging.PixelFormat.Format16bppGrayScale)
            {
                Short2Byte sb = new Short2Byte(bitmap);
                bitmap_8 = sb.ToByteImage();
            }

            ImageTransfor transfor = new ImageTransfor();

            transfor.width = bitmap_8.Width;
            transfor.height = bitmap_8.Height;
            transfor.channel = Image.GetPixelFormatSize(bitmap_8.PixelFormat) / 8;

            var rect = new Rectangle(0, 0, bitmap_8.Width, bitmap_8.Height);
            var bmpData = bitmap_8.LockBits(rect, ImageLockMode.ReadOnly, bitmap_8.PixelFormat);

            var imgBytes = bmpData.Stride * bmpData.Height;

            //循环使用指针
            IntPtr pointer = IntPtr.Zero;
            if (_dict_points.ContainsKey(_dict_point_index))
            {
                _dict_points.TryGetValue(_dict_point_index, out pointer);
            }
            else
            {
                pointer = Marshal.AllocHGlobal(imgBytes);
                _dict_points.TryAdd(_dict_point_index, pointer);
            }
            transfor.data = pointer;

            _dict_point_index++;
            //最大存20张图
            if (_dict_point_index > 20) _dict_point_index = 0;

            byte[] rgbValues = new byte[imgBytes];
            IntPtr ptr = bmpData.Scan0;
            for (var i = 0; i < bitmap_8.Height; i++)
            {
                Marshal.Copy(ptr, rgbValues, i * bmpData.Width * transfor.channel, bmpData.Width * transfor.channel);
                ptr += bmpData.Stride;
            }
            Marshal.Copy(rgbValues, 0, transfor.data, imgBytes);

            bitmap_8.UnlockBits(bmpData);

            watch.Stop();
            Share.Info(string.Format("转C++交换对象耗时：{0}", watch.ElapsedMilliseconds));

            return transfor;
        }

        ///<summary>将图片转换为C++交换对象(传递电芯序号 16位图)</summary>
        public static ImageTransfor ToTransfor(this Bitmap bitmap, ImageTypes imgType)
        {
            Share.Info("转C++交换对象开始(Share.ToTransfor)");
            Stopwatch watch = new Stopwatch();
            watch.Start();

            Bitmap cBitmap = bitmap;

            if (imgType == ImageTypes.Eight)
            {
                //转8位再传给算法
                if (bitmap.PixelFormat == System.Drawing.Imaging.PixelFormat.Format16bppGrayScale)
                {
                    Short2Byte sb = new Short2Byte(bitmap);
                    cBitmap = sb.ToByteImage();
                }
            }

            ImageTransfor transfor = new ImageTransfor();

            transfor.width = cBitmap.Width;
            transfor.height = cBitmap.Height;

            //TODO: ZhangKF 2021-2-5
            //transfor.channel = Image.GetPixelFormatSize(cBitmap.PixelFormat) / 8;
            transfor.channel = 1;

            var rect = new Rectangle(0, 0, cBitmap.Width, cBitmap.Height);
            var bmpData = cBitmap.LockBits(rect, ImageLockMode.ReadOnly, cBitmap.PixelFormat);

            var imgBytes = bmpData.Width * bmpData.Height;
            if (imgType == ImageTypes.Sixteen)
            {
                imgBytes = imgBytes * sizeof(ushort);
            }

            //循环使用指针
            IntPtr pointer = IntPtr.Zero;
            if (_dict_points.ContainsKey(_dict_point_index))
            {
                if (!_dict_points.TryGetValue(_dict_point_index, out pointer))
                {
                    pointer = IntPtr.Zero;
                    throw new Exception("未能获取已存在的内存分配地址！");
                }
            }
            else
            {
                pointer = Marshal.AllocHGlobal(imgBytes);
                if (!_dict_points.TryAdd(_dict_point_index, pointer))
                {
                    pointer = IntPtr.Zero;
                    throw new Exception("未能获取新增的内存分配地址！");
                }
            }
            transfor.data = pointer;

            _dict_point_index++;
            //最大存20张图
            if (_dict_point_index > 20) _dict_point_index = 0;

            if (imgType == ImageTypes.Sixteen)
            {
                #region 16位图
                ushort[] rgbValues = new ushort[bmpData.Width * bmpData.Height];
                IntPtr ptr = bmpData.Scan0;
                for (var i = 0; i < cBitmap.Height; i++)
                {
                    Share.CopyToArray(ptr, rgbValues, i * bmpData.Width, bmpData.Width * 2);
                    ptr += bmpData.Stride;
                }
                Share.CopyFromArray(rgbValues, 0, transfor.data, rgbValues.Length * sizeof(ushort));
                #endregion
            }
            else
            {
                #region 8位图
                byte[] rgbValues = new byte[imgBytes];
                IntPtr ptr = bmpData.Scan0;
                for (var i = 0; i < cBitmap.Height; i++)
                {
                    Marshal.Copy(ptr, rgbValues, i * bmpData.Width * transfor.channel, bmpData.Width * transfor.channel);
                    ptr += bmpData.Stride;
                }
                Marshal.Copy(rgbValues, 0, transfor.data, imgBytes);
                #endregion
            }

            cBitmap.UnlockBits(bmpData);

            watch.Stop();
            Share.Info(string.Format("转C++交换对象耗时：{0}", watch.ElapsedMilliseconds));

            //transfor.Save(@"e:\"+ imageIndex.ToString() +".jpg", ImageTypes.Sixteen);
            //imageIndex++;

            return transfor;
        }

        ///<summary>获取图像的通道数</summary>
        public static int Channel(this Bitmap bitmap)
        {
            //return Image.GetPixelFormatSize(bitmap.PixelFormat) / 8;
            int channel = 0;
            if (bitmap.PixelFormat == System.Drawing.Imaging.PixelFormat.Format16bppGrayScale || bitmap.PixelFormat == System.Drawing.Imaging.PixelFormat.Format8bppIndexed)
            {
                channel = 1;
            }
            else if (bitmap.PixelFormat == System.Drawing.Imaging.PixelFormat.Format24bppRgb)
            {
                channel = 3;
            }
            else
            {
                throw new Exception(string.Format("未识别图像类型:{0}", bitmap.PixelFormat));
            }

            return channel;
        }

        ///<summary>复制非托管内存数据到托管对象(指针 -> 数组),length-字节长度</summary>
        public static void CopyToArray<T>(IntPtr source, T[] destination, int startIndex, int length) where T : struct
        {
            Share.Info("复制指针到数组开始(Share.CopyToArray)");
            Stopwatch watch = new Stopwatch();
            watch.Start();

            var gch = GCHandle.Alloc(destination, GCHandleType.Pinned);
            try
            {
                var targetPtr = Marshal.UnsafeAddrOfPinnedArrayElement(destination, startIndex);
                CopyMemory(targetPtr, source, (UIntPtr)length);
            }
            finally
            {
                gch.Free();
            }

            watch.Stop();
            Share.Info(string.Format("复制指针到数组耗时：{0}", watch.ElapsedMilliseconds));
        }

        ///<summary>复制托管对象到内存数据(数组 -> 指针),length-字节长度</summary>
        public static void CopyFromArray<T>(T[] source, int startIndex, IntPtr target, int length) where T : struct
        {
            Share.Info("复制数组到指针开始(Share.CopyFromArray)");
            Stopwatch watch = new Stopwatch();
            watch.Start();

            //var gch = GCHandle.Alloc(source, GCHandleType.Pinned);
            try
            {
                var sourcePtr = Marshal.UnsafeAddrOfPinnedArrayElement(source, startIndex);
                CopyMemory(target, sourcePtr, (UIntPtr)length);
            }
            finally
            {
                //gch.Free();
            }

            watch.Stop();
            Share.Info(string.Format("复制数组到指针耗时：{0}", watch.ElapsedMilliseconds));
        }

        ///<summary>将TIFF16位灰阶图转换为Bitmap对象</summary>
        public static Bitmap TiffToBitmap(string fileName)
        {
            Bitmap result;

            using (Tiff tif = Tiff.Open(fileName, "r"))
            {
                FieldValue[] res = tif.GetField(TiffTag.IMAGELENGTH);
                int height = res[0].ToInt();

                res = tif.GetField(TiffTag.IMAGEWIDTH);
                int width = res[0].ToInt();

                res = tif.GetField(TiffTag.BITSPERSAMPLE);
                short bpp = res[0].ToShort();
                if (bpp != 16)
                    return null;

                res = tif.GetField(TiffTag.SAMPLESPERPIXEL);
                short spp = res[0].ToShort();
                if (spp != 1)
                    return null;

                res = tif.GetField(TiffTag.PHOTOMETRIC);
                Photometric photo = (Photometric)res[0].ToInt();
                if (photo != Photometric.MINISBLACK && photo != Photometric.MINISWHITE)
                    return null;

                int stride = tif.ScanlineSize();
                byte[] buffer = new byte[stride];

                result = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format16bppGrayScale);
                ushort[] buffer16Bit = new ushort[width];

                for (int i = 0; i < height; i++)
                {
                    Rectangle imgRect = new Rectangle(0, i, width, 1);
                    BitmapData imgData = result.LockBits(imgRect, ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format16bppGrayScale);

                    tif.ReadScanline(buffer, i);
                    Buffer.BlockCopy(buffer, 0, buffer16Bit, 0, buffer.Length);
                    Share.CopyFromArray(buffer16Bit, 0, imgData.Scan0, buffer16Bit.Length * sizeof(ushort));

                    result.UnlockBits(imgData);
                }
            }

            return result;
        }

        ///<summary>8位图图片翻转 transforType 0/1 水平/垂直 翻转  (仅限8位图)</summary>
        public static Bitmap Transfor8(this Bitmap imgBit, int transforType = 0)
        {
            Share.Info("图片水平翻转开始");
            Stopwatch watch = new Stopwatch();
            watch.Start();

            if (imgBit.PixelFormat != System.Drawing.Imaging.PixelFormat.Format8bppIndexed)
            {
                throw new Exception("当前不是图片位8位图");
            }

            Rectangle rect = new Rectangle(0, 0, imgBit.Width, imgBit.Height);
            //内存锁定
            BitmapData data = imgBit.LockBits(rect, ImageLockMode.ReadWrite, imgBit.PixelFormat);

            //取得图象的首地址
            IntPtr ptr = data.Scan0;

            //8位图的字节数
            int byteSize = imgBit.Width * imgBit.Height;

            //定义位图数组
            byte[] bytes = new byte[byteSize];

            //复制被锁定的位图图象到数组内
            Marshal.Copy(ptr, bytes, 0, byteSize);

            int halfWidth = imgBit.Width / 2;
            int halfHeight = imgBit.Height / 2;

            byte temp = 0;

            if (transforType == 0)
            {
                #region 水平翻转
                for (int i = 0; i < imgBit.Height; i++)
                {
                    for (int j = 0; j < halfWidth; j++)
                    {
                        //以水平轴为对称轴，两边像素值互换
                        temp = bytes[i * imgBit.Width + j];
                        bytes[i * imgBit.Width + j] = bytes[(i + 1) * imgBit.Width - 1 - j];
                        bytes[(i + 1) * imgBit.Width - 1 - j] = temp;
                    }
                }
                #endregion
            }
            else if (transforType == 1)
            {
                #region 垂直翻转
                for (int i = 0; i < imgBit.Width; i++)
                {
                    for (int j = 0; j < halfHeight; j++)
                    {
                        //以垂直中轴线为对轴，两边像素互换
                        temp = bytes[j * imgBit.Width + i];
                        bytes[j * imgBit.Width + i] = bytes[(imgBit.Height - j - 1) * imgBit.Width + i];
                        bytes[(imgBit.Height - j - 1) * imgBit.Width + i] = temp;
                    }
                }
                #endregion
            }

            imgBit.UnlockBits(data);

            Bitmap newImage = new Bitmap(imgBit.Width, imgBit.Height, imgBit.PixelFormat);
            Rectangle rect2 = new Rectangle(0, 0, newImage.Width, newImage.Height);
            BitmapData newData = newImage.LockBits(rect2, ImageLockMode.ReadWrite, newImage.PixelFormat);
            Marshal.Copy(bytes, 0, newData.Scan0, bytes.Length);
            newImage.UnlockBits(newData);

            watch.Stop();
            Share.Info(string.Format("图片翻转结束，共耗时{0}", watch.ElapsedMilliseconds));

            return newImage;
        }

        ///<summary>16位图图片翻转 transforType 0/1 水平/垂直 翻转  (仅限16位图)</summary>
        public static Bitmap Transfor16(this Bitmap imgBit, int transforType = 0)
        {
            Share.Info("16位图片水平翻转开始");
            Stopwatch watch = new Stopwatch();
            watch.Start();

            if (imgBit.PixelFormat != System.Drawing.Imaging.PixelFormat.Format16bppGrayScale)
            {
                throw new Exception("当前不是图片位16位图");
            }

            Rectangle rect = new Rectangle(0, 0, imgBit.Width, imgBit.Height);
            //内存锁定
            BitmapData data = imgBit.LockBits(rect, ImageLockMode.ReadWrite, imgBit.PixelFormat);

            //取得图象的首地址
            IntPtr ptr = data.Scan0;

            //8位图的字节数
            int byteSize = imgBit.Width * imgBit.Height;

            //定义位图数组
            ushort[] bytes = new ushort[byteSize];

            //复制被锁定的位图图象到数组内
            //Marshal.Copy(ptr, bytes, 0, byteSize);
            CopyToArray(ptr, bytes, 0, byteSize * 2);

            int halfWidth = imgBit.Width / 2;
            int halfHeight = imgBit.Height / 2;

            ushort temp = 0;

            if (transforType == 0)
            {
                #region 水平翻转
                for (int i = 0; i < imgBit.Height; i++)
                {
                    for (int j = 0; j < halfWidth; j++)
                    {
                        //以水平轴为对称轴，两边像素值互换
                        temp = bytes[i * imgBit.Width + j];
                        bytes[i * imgBit.Width + j] = bytes[(i + 1) * imgBit.Width - 1 - j];
                        bytes[(i + 1) * imgBit.Width - 1 - j] = temp;
                    }
                }
                #endregion
            }
            else if (transforType == 1)
            {
                #region 垂直翻转
                for (int i = 0; i < imgBit.Width; i++)
                {
                    for (int j = 0; j < halfHeight; j++)
                    {
                        //以垂直中轴线为对轴，两边像素互换
                        temp = bytes[j * imgBit.Width + i];
                        bytes[j * imgBit.Width + i] = bytes[(imgBit.Height - j - 1) * imgBit.Width + i];
                        bytes[(imgBit.Height - j - 1) * imgBit.Width + i] = temp;
                    }
                }
                #endregion
            }

            imgBit.UnlockBits(data);

            Bitmap newImage = new Bitmap(imgBit.Width, imgBit.Height, imgBit.PixelFormat);
            Rectangle rect2 = new Rectangle(0, 0, newImage.Width, newImage.Height);
            BitmapData newData = newImage.LockBits(rect2, ImageLockMode.ReadWrite, newImage.PixelFormat);
            //Marshal.Copy(bytes, 0, newData.Scan0, bytes.Length);
            CopyFromArray(bytes, 0, newData.Scan0, bytes.Length * 2);
            newImage.UnlockBits(newData);

            watch.Stop();
            Share.Info(string.Format("16位图片翻转结束，共耗时{0}", watch.ElapsedMilliseconds));

            return newImage;
        }
        //end class
    }
}
