#if VAREX

using Dexela_NET;
using DexelaDefs;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ZY.Vision.Algorithm;
using ZY.Vision.Interfaces;



namespace ZY.Vision.Varex
{
    /// <summary>
    /// 相机设备
    /// ZhangKF 2020-12-22
    /// </summary>
    public class CameraDevice:ICameraDevice
    {
        #region Temp
        ///<summary>瓦里安相机设备信息</summary>
        DevInfo_NET _varex_camera_device_info = null;
        ///<summary>设备访问对象</summary>
        DexelaDetector _detector = null;
        #endregion

        ///<summary>相机参数</summary>
        public CameraParameter CameraParam
        {
            get;
            set;
        }

        ///<summary>构造函数,cameraIndex 相机索引(可选值 1 - 一号相机 ２ - 二号相机)</summary>
        public CameraDevice(DevInfo_NET varexDeviceInfo, CameraParameter parameter, int cameraIndex)
        {
            this._varex_camera_device_info = varexDeviceInfo;
            this._detector = new DexelaDetector(varexDeviceInfo, parameter.CaptureMode, parameter.ExposeTime, cameraIndex);
            this.CameraParam = parameter;
        }

        ///<summary>拍照并返回相片对象,frame - 拍照次数</summary>
        public Bitmap CaptureOneImage(int frame)
        {
            if (frame <= 0 || frame >= 10)
            {
                throw new ArgumentException(string.Format("拍照次数错误，有效范围为(0-8),当前值为:{0}", frame));
            }

            List<Bitmap> bitmaps = new List<Bitmap>();
            for (int i = 0; i < frame; i++)
            {
                var bitmap = _detector.CaptureImage(this.CameraParam.ImageWidth, this.CameraParam.ImageHeight, this.CameraParam.ExposeTime);
                bitmaps.Add(bitmap);
                //Thread.Sleep(200);
            }

            var mergeBitmap = ImageProcess.Average(bitmaps.ToArray());
            return mergeBitmap;
        }

        ///<summary>拍照并返回相片对象,frame - 拍照次数, 可以返回TIF图 2021-3-9 ZhangKF</summary>
        public Bitmap CaptureOneImageAndTIF(int frame,ref Bitmap tif)
        {
            if (frame <= 0 || frame >= 10)
            {
                throw new ArgumentException(string.Format("拍照次数错误，有效范围为(0-8),当前值为:{0}", frame));
            }

            List<Bitmap> bitmaps = new List<Bitmap>();
            for (int i = 0; i < frame; i++)
            {
                var bitmap = _detector.CaptureImage(this.CameraParam.ImageWidth, this.CameraParam.ImageHeight, this.CameraParam.ExposeTime);
                bitmaps.Add(bitmap);
                //Thread.Sleep(200);

                if (tif == null)
                {
                    tif = bitmap;
                }
            }

            var mergeBitmap = ImageProcess.Average(bitmaps.ToArray());
            return mergeBitmap;
        }

        ///<summary>拍照并返回相片对象</summary>
        public Bitmap CaptureOneImage()
        {
            return CaptureOneImage(this.CameraParam.CaptrueFrame);
        }

        ///<summary>手动关闭相机</summary>
        public void CloseCamera()
        {
            this._detector.CloseCamera();
        }
        //end class
    }
}

#endif