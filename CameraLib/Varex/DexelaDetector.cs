#if VAREX

using Dexela_NET;
using DexelaDefs;
using System;
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
using ZY.Vision.Exceptions;

namespace ZY.Vision.Varex
{
    /// <summary>
    /// 平板相机访问对象，也是对瓦里安平板探测器的装
    /// ZhangKF 2020-12-22
    /// </summary>
    internal class DexelaDetector
    {
        ///<summary>瓦力安平板原始对象</summary>
        DexelaDetector_NET _detector = null;
        ///<summary>相机图像(全局为载入校正文件)</summary>
        DexImage_NET _image = new DexImage_NET();
        /// <summary> 是否进行相机坏线校准</summary>
        bool _DefectCheckEnable = false;

        ///<summary>相机索引</summary>
        public int CameraIndex
        {
            get;
            set;
        }

        ///<summary>构造函数,cameraIndex 相机索引(范围1或2)</summary>
        public DexelaDetector(DevInfo_NET deviceInfo, FullWellModes_NET captureMode, float exposeTime, int cameraIndex)
        {
            Share.Info(string.Format("初始化相机 SN={0} 相机索引 cameraIndex = {1}", deviceInfo.serialNum, cameraIndex));

            //TODO:是否存在坏线校准文件，利用配置文件开启此功能 ZhangKF 2021-3-11
            if (File.Exists(string.Format(Consts.DEFECT_MAP_FILE_INPUT, cameraIndex)))
            {
                _DefectCheckEnable = true;
            }

            if (cameraIndex != 1 && cameraIndex != 2)
            {
                throw new ArgumentException("相机索引值错误");
            }

            _detector = new DexelaDetector_NET(deviceInfo);
            this.CameraIndex = cameraIndex;

            //打开平板
            if (!_detector.IsConnected())
            {
                this._detector.OpenBoard();
                this._detector.SetFullWellMode(captureMode);
                this._detector.SetExposureTime(exposeTime);
                this._detector.SetBinningMode(bins_NET.x11);
                this._detector.SetTriggerSource(ExposureTriggerSource_NET.Internal_Software);
                this._detector.SetExposureMode(ExposureModes_NET.Expose_and_read);

                //载入校正文件
                _image.LoadDarkImage(string.Format(Consts.DARK_CORRECTION_FILE_OUTPUT, this.CameraIndex));
                _image.LoadFloodImage(string.Format(Consts.FLOOD_CORRECTION_FILE_OUTPUT, this.CameraIndex));
                if (_DefectCheckEnable)
                    _image.LoadDefectMap(string.Format(Consts.DEFECT_MAP_FILE_INPUT, this.CameraIndex));
            }
        }

        ///<summary>手动关闭相机</summary>
        public void CloseCamera()
        {
            if (_detector != null)
            {
                _detector.CloseBoard();
            }
        }

        ///<summary>调用设备拍照</summary>
        public Bitmap CaptureImage(int imgWidth, int imgHeight, int exposeTime)
        {
            Share.Info("相机拍照开始(DexelaDetector.CaptureImage)");
            Stopwatch watch = new Stopwatch();
            watch.Start();

            int imgSize = imgWidth * imgHeight * sizeof(ushort);

            _detector.Snap(1, exposeTime + 1000);

            ushort[] bytes = new ushort[imgSize];
            _detector.ReadBuffer(1, _image, 1);
            _image.UnscrambleImage();
            _image.SubtractDark();
            _image.FloodCorrection();
            if (_DefectCheckEnable)
                _image.DefectCorrection();

            IntPtr imgPtr = _image.GetDataPointerToPlane(1);

            Share.CopyToArray(imgPtr, bytes, 0, imgSize);

            Bitmap newImg = new Bitmap(imgWidth, imgHeight, PixelFormat.Format16bppGrayScale);
            Rectangle rect = new Rectangle(0, 0, newImg.Width, newImg.Height);
            BitmapData data = newImg.LockBits(rect, ImageLockMode.ReadWrite, newImg.PixelFormat);

            Share.CopyFromArray(bytes, 0, data.Scan0, imgSize);

            newImg.UnlockBits(data);

            //对图像进行旋转
            //var rotateImage = newImg.Rotate();

            watch.Stop();
            Share.Info(string.Format("相机拍照耗时：{0}", watch.ElapsedMilliseconds));

            return newImg;
        }
        //end class
    }
}

#endif