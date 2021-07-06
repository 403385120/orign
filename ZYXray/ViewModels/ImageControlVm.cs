using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shuyz.Framework.Mvvm;
using XRayClient.VisionSysWrapper;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Runtime.InteropServices;
using ZYXray.Models;
using System.Drawing;
using ZY.Vision;

namespace ZYXray.ViewModels
{
    public class ImageControlVm : ObservableObject
    {
        [DllImport("kernel32.dll", EntryPoint = "CopyMemory", SetLastError = false)]
        public static extern void CopyMemory(IntPtr dest, IntPtr src, uint count);

        private WriteableBitmap _wBitmap = null;

        private bool _enableMeasurements = false;
        private float _pixelDist = 0f;

        private float _actualDist = 0f;
        private float _pixelRatio = 0f;

        private bool _isCrossVisible = true;

        private double _caliDistPointThree = 1;//标定片物理距离保留三位小数

        public ImageControlVm()
        {

        }

        ~ImageControlVm()
        {
            if (null != _wBitmap)
            {
                // TODO: recycle memory
                //_wBitmap.
            }
        }

        public bool IsCrossVisible
        {
            get { return this._isCrossVisible; }
            set
            {
                this._isCrossVisible = value;
                RaisePropertyChanged("IsCrossVisible");
            }
        }

        public bool EnableMeasurements
        {
            get { return this._enableMeasurements; }
            set
            {
                this._enableMeasurements = value;
                RaisePropertyChanged("EnableMeasurements");
            }
        }

        public float ActualDist
        {
            get { return this._actualDist; }
            set
            {
                this._actualDist = value;
                RaisePropertyChanged("ActualDist");
                CaliDistPointThree = Convert.ToDouble(this._actualDist);
            }
        }

        public double CaliDistPointThree
        {
            //get { return Math.Round(Convert.ToDouble(this.ActualDist), 3); }
            get { return Math.Round(this._caliDistPointThree, 3); }
            set
            {
                this._caliDistPointThree = value;
                RaisePropertyChanged("CaliDistPointThree");
            }
        }

        public float PixelRatio
        {
            get
            {
                if (this._pixelRatio == 0)
                {
                    return HardwareConfig.Instance.PixelRatio;
                }
                else
                {
                    return this._pixelRatio;
                }
            }
            set
            {
                HardwareConfig.Instance.PixelRatio = value;
                this._pixelRatio = value;
                RaisePropertyChanged("PixelRatio");

                this.ActualDist = this.PixelRatio * this.PixelDist;
            }
        }

        public float PixelDist
        {
            get
            {
                if (this._pixelDist == 0)
                {
                    return HardwareConfig.Instance.PixelDist;
                }
                else
                {
                    return this._pixelDist;
                }
            }
            set
            {
                HardwareConfig.Instance.PixelDist = value;
                this._pixelDist = value;
                RaisePropertyChanged("PixelDist");

                this.ActualDist = this.PixelRatio * this.PixelDist;
            }
        }

        public void UpdateImage(ref ZYImageStruct image)
        {
            if (null == _wBitmap || image.height != _wBitmap.Height || image.width != _wBitmap.Width ||
                    (image.channel == 1 && _wBitmap.Format != PixelFormats.Gray8) || (image.channel == 3 && _wBitmap.Format != PixelFormats.Bgr24))
            {
                _wBitmap = new WriteableBitmap(image.width, image.height, 96, 96,
                    (image.channel == 3 ? PixelFormats.Bgr24 : PixelFormats.Gray8), null);
            }

            CopyMemory(_wBitmap.BackBuffer, image.data, (uint)(image.width * image.height * image.channel));

            _wBitmap.Lock();
            _wBitmap.AddDirtyRect(new Int32Rect(0, 0, _wBitmap.PixelWidth, _wBitmap.PixelHeight));
            _wBitmap.Unlock();

            RaisePropertyChanged("BitmapSource");
        }

        //TODO：ZhangKF 2020-12-31
        ///<summary>新版相机封装</summary>
        public void UpdateImage2(Bitmap image)
        {
            if (null == _wBitmap || image.Width != _wBitmap.Height || image.Height != _wBitmap.Width ||
                    (image.Channel() == 1 && _wBitmap.Format != PixelFormats.Gray8) || (image.Channel() == 3 && _wBitmap.Format != PixelFormats.Bgr24))
            {
                _wBitmap = new WriteableBitmap(image.Width, image.Height, 96, 96, (image.Channel() == 3 ? PixelFormats.Bgr24 : PixelFormats.Gray8), null);
            }

            //TODO: 获取8位灰阶图指针
            var bytes = image.ToBytes_8();
            var ptr = Share.GetIntPtr(bytes);

            //CopyMemory(_wBitmap.BackBuffer, image.data, (uint)(image.width * image.height * image.channel));
            CopyMemory(_wBitmap.BackBuffer, ptr, (uint)(image.Width * image.Height * image.Channel()));

            _wBitmap.Lock();
            _wBitmap.AddDirtyRect(new Int32Rect(0, 0, _wBitmap.PixelWidth, _wBitmap.PixelHeight));
            _wBitmap.Unlock();

            //TODO: 释放非托管内存 ZhangKF
            Marshal.FreeHGlobal(ptr);

            RaisePropertyChanged("BitmapSource");
        }

        public WriteableBitmap BitmapSource
        {
            get { return this._wBitmap; }
        }
    }
}
