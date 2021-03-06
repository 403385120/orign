using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ZY.Vision.Algorithm;
using ZY.Vision.Interfaces;
using ZY.Vision.Varex;

namespace ZY.Vision.Baumer
{
    /// <summary>
    /// 对宝盟相机设备的封装
    /// ZhangKF 2021-1-13
    /// </summary>
    public class CameraDevice : ICameraDevice
    {
        ///<summary>宝盟相机设备</summary>
        BGAPI2.Device _device = null;
        ///<summary>回调对象</summary>
        BaumerCallBack _callBack = null;

        ///<summary>相机参数</summary>
        public CameraParameter CameraParam
        {
            get;
            set;
        }

        ///<summary>相机索引号,有效值(1/2)</summary>
        public int CameraIndex
        {
            get;
            set;
        }

        CameraParameter ICameraDevice.CameraParam
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        ///<summary>构造函数,cameraIndex 相机索引(可选值 1 - 一号相机 ２ - 二号相机)</summary>
        public CameraDevice(CameraParameter parameter, BGAPI2.Device device, int cameraIndex)
        {
            this.CameraParam = parameter;
            this.CameraIndex = cameraIndex;

            _device = device;
            _device.Open();

            //初始化
            _device.GetRemoteNodeList()["ExposureTime"].Value = parameter.ExposeTime * 1000; //微秒为单位，1毫秒 = 1000微秒
            //_device.GetRemoteNodeList()["ExposureTime"].Value = 2000;
            _device.GetRemoteNodeList()["Width"].Value = parameter.ImageWidth;
            _device.GetRemoteNodeList()["Height"].Value = parameter.ImageHeight;
            //TODO:采图格式设备(SXG10只支持MONO8、MONO10、MONO12)
            _device.GetRemoteNodeList()["PixelFormat"].Value = "Mono8";

            //只用于回调模式()
            _device.RemoteNodeList["TriggerMode"].Value = "Off";

            //增益
            _device.GetRemoteNodeList()["Gain"].Value = parameter.gain;

            _callBack = new BaumerCallBack(_device);
        }

        ///<summary>拍照并返回相片对象,frame - 拍照次数</summary>
        public Bitmap CaptureOneImage(int frame)
        {
            if (frame <= 0 || frame >= 50)
            {
                throw new ArgumentException(string.Format("拍照次数错误，有效范围为(0-8),当前值为:{0}", frame));
            }

            //System.Drawing.Imaging.PixelFormat pixel =  System.Drawing.Imaging.PixelFormat.Format16bppGrayScale;

            //List<Bitmap> bitmaps = new List<Bitmap>();
            //for (int i = 0; i < frame; i++)
            //{
            //    var bitmap = BaumerWrapper.CaptureImage(_device);
            //    pixel = bitmap.PixelFormat;
            //    bitmaps.Add(bitmap);
            //}

            //Bitmap mergeBitmap = null;

            //if (pixel == System.Drawing.Imaging.PixelFormat.Format16bppGrayScale)
            //{
            //    mergeBitmap = ImageProcess.Average(bitmaps.ToArray());
            //}
            //else if (pixel == System.Drawing.Imaging.PixelFormat.Format8bppIndexed)
            //{
            //    mergeBitmap = ImageProcess.Average8(bitmaps.ToArray());
            //}

            //非回调方式
            //List<byte[]> list = new List<byte[]>();
            //for (var i = 0; i < frame; i++)
            //{
            //    var bytes = BaumerWrapper.CaptureBytes(_device);
            //    list.Add(bytes);
            //}

            //回调方式
            //var list = BaumerWrapper.CaptureBytes2(_device, frame);
            List<byte[]> list = new List<byte[]>();
            
            EventCallBack handle = null;
            handle = new EventCallBack(bytes =>
            {
                list.Add(bytes);

            });
            _callBack.eventCallBack += handle;

            while (true)
            {
                if (list.Count() >= frame)
                {
                    _callBack.eventCallBack -= handle;
                    break;
                }
                Thread.Sleep(50);
            }
            //var list = BaumerWrapper.CaptureBytes3(_device, frame);

            return ImageProcess.AverageBytes(list, this.CameraParam.ImageWidth, this.CameraParam.ImageHeight);
        }

        ///<summary>拍照并返回相片对象</summary>
        public Bitmap CaptureOneImage()
        {
            return CaptureOneImage(this.CameraParam.CaptrueFrame);
        }

        ///<summary>手动关闭相机</summary>
        public void CloseCamera()
        {
            _device.Close();
            _callBack.Stop();
            BaumerWrapper.Destroy();
        }

        Bitmap ICameraDevice.CaptureOneImage(int frame)
        {
            throw new NotImplementedException();
        }

        Bitmap ICameraDevice.CaptureOneImageAndTIF(int frame, ref Bitmap tif)
        {
            throw new NotImplementedException();
        }

        Bitmap ICameraDevice.CaptureOneImage()
        {
            throw new NotImplementedException();
        }

        void ICameraDevice.CloseCamera()
        {
            throw new NotImplementedException();
        }
        //end class
    }
}
