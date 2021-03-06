using Dexela_NET;
using DexelaDefs;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using ZY.Vision.Exceptions;
using ZY.Vision.Interfaces;
using ZY.Vision.Utils;
using ZY.Vision.Varex;

namespace ZY.Vision
{
    /// <summary>
    /// 对万睿视（瓦里安）平板的封装访问
    /// ZhangKF 2020-12-22
    /// </summary>
    public class CameraHelper
    {
        ///<summary>图片旋转次数</summary>
        static int _rotate = -1;

        ///<summary>可使用相机设置</summary>
        public static List<ICameraDevice> Devices
        {
            get;
            private set;
        } = new List<ICameraDevice>();

        ///<summary>相机初始化，包括查询可用相机、相机校准</summary>
        public static void Init()
        {
            try
            {
                Stopwatch watch = new Stopwatch();
                watch.Start();

                Share.Info("相机初始化开始:Init");
                FindCameraDevice();

                watch.Stop();
                Share.Info(string.Format("相机初始化结束,耗时:{0}", watch.ElapsedMilliseconds));
            }
            catch (BGAPI2.Exceptions.IException ex)
            {
                string msg = ex.GetErrorDescription();
                Share.Error(msg);
                Share.Error(ex.StackTrace);
            }
            catch (Exception ex)
            {
                Share.Error(ex.Message);
                Share.Error(ex.StackTrace);
            }
        }

        ///<summary>搜索可用相机</summary>
        private static void FindCameraDevice()
        {
            #region VAREX(瓦力安相机)
#if VAREX
            //扫描已连接到机器的相机
            BusScanner_NET scanner = new BusScanner_NET();
            List<DevInfo_NET> devInfos = scanner.GetDeviceListCL();
            int devCount = devInfos.Count;

            #region 无设备
            if (devCount == 0)
            {
                throw new CameraNotFoundException("未找到任何相机设备!");
            }
            else
            {
                Share.Info(string.Format("FindCameraDeivce:查询到相机设备数量={0}", devCount));
            }
            #endregion

            List<CameraParameter> cameraParameters = new List<CameraParameter>();
            #region 获取所有相机配置参数
            for (var i = 1; i <= devCount; i++)
            {
                //加载相机配置
                CameraParameter param = CameraParameter.ReadConfig(string.Format("Camera{0}Config", i));
                if (param.ImageWidth < param.ImageHeight)
                {
                    throw new Exception(string.Format("请检查HarewareConfig.ini的长宽设置（Width:{0} Height:{1}）！",param.ImageWidth,param.ImageHeight));
                }

                if (param == null) throw new ArgumentNullException(string.Format("在HardwareConfig.ini中未找到相机{0}配置项", i));
                cameraParameters.Add(param);
            }
            #endregion

            int cameraIndex = 1;
            //以配置文件定义相机的先后顺序
            cameraParameters.ForEach(cameraParameter =>
            {
                var device = devInfos.Where(dev => dev.serialNum.ToString().Equals(cameraParameter.SzBufSeriNum));
                if (device.Count() > 0)
                {
                    //TODO:先生成校准文件，然后再初始化相机，避免校准文件不存在报错 ZhangKF 2021-4-5
                    CameraCorrectHelper.Correct(cameraIndex);
                    CameraDevice cameraDevice = new CameraDevice(device.First(), cameraParameter, cameraIndex);
                    Devices.Add(cameraDevice);
                }
                else
                {
                    throw new ArgumentException(string.Format("配置序列号({0})的相机不存在", cameraParameter.SzBufSeriNum));
                }

                cameraIndex++;
            });
#endif
            #endregion

            #region BAUMER(宝盟相机)

#if BAUMER
            var buamerDevices = Baumer.BaumerWrapper.GetDevices();
            List<CameraParameter> cameraParameters = new List<CameraParameter>();
            #region 获取所有相机配置参数
            for (var i = 1; i <= 2; i++)
            {
                //加载相机配置
                CameraParameter param = CameraParameter.ReadConfig(string.Format("Camera{0}Config", i));
                if (param.ImageWidth < param.ImageHeight)
                {
                    throw new Exception(string.Format("请检查HarewareConfig.ini的长宽设置（Width:{0} Height:{1}）！", param.ImageWidth, param.ImageHeight));
                }

                if (param == null) throw new ArgumentNullException(string.Format("在HardwareConfig.ini中未找到相机{0}配置项", i));
                cameraParameters.Add(param);
            }
            #endregion

            int cameraIndex = 1;
            //以配置文件定义相机的先后顺序
            cameraParameters.ForEach(cameraParameter =>
            {
                var device = buamerDevices.Where(dev => dev.SerialNumber.ToString().Equals(cameraParameter.SzBufSeriNum));
                if (device.Count() > 0)
                {
                    var dev = new Baumer.CameraDevice(cameraParameter, device.First(), cameraIndex);
                    Devices.Add(dev);
                }
                else
                {
                    throw new ArgumentException(string.Format("配置序列号({0})的相机不存在", cameraParameter.SzBufSeriNum));
                }

                cameraIndex++;
            });
#endif

            #endregion
        }

        ///<summary>利用指定的设备拍取一张图 frame 拍照帧数</summary>
        public static CaptureResult CaptureOneImage(ICameraDevice device, int frame)
        {
            CaptureResult result = new CaptureResult();
            try
            {
                Share.Info(string.Format("拍照开始:CaptureOneImage,拍照张数:{0}", frame));
                Stopwatch watch = new Stopwatch();
                watch.Start();

                //TODO:同时获取TIF原图  ZhangKF 2021-3-9
                //var bitmap = device.CaptureOneImage(frame);
                Bitmap tifImage = null;
                var bitmap = device.CaptureOneImageAndTIF(frame,ref tifImage);
                result.TIFImage = tifImage;

                #region 图片旋转
                if (_rotate == -1)
                {
                    string val = CameraParameter.ReadConfigItem(Consts.HARDWARE_CONFIG_CAMERA, Consts.HARDWARE_CONFIG_ROTATE);
                    if (!string.IsNullOrEmpty(val))
                    {
                        int.TryParse(val, out _rotate);
                    }
                }
                #endregion
                for (int i = 0; i < _rotate; i++)
                {
                    bitmap = bitmap.Rotate2();
                }

                result.Bitmap = bitmap;
                result.Result = true;

                watch.Stop();
                Share.Info(string.Format("拍照共耗时:{0}", watch.ElapsedMilliseconds));
                Share.Log(string.Format("拍照帧数:{0} 耗时:{1}", frame, watch.ElapsedMilliseconds));
            }
            catch(DexelaException_NET dex)
            {
                result.Result = false;
                result.ErrorMessage = dex.Message;

                string errMsg = string.Format("{0} {1} {2}", dex.Code, dex.TransportError, dex.TransportMessage);
                Share.Error(errMsg);
                Share.Error(dex.StackTrace);
            }
            catch (Exception ex)
            {
                result.Result = false;
                result.ErrorMessage = ex.Message;

                Share.Error(ex.Message);
                Share.Error(ex.StackTrace);
            }
            return result;
        }

        ///<summary>利用指定的设备拍取一张图</summary>
        public static CaptureResult CaptureOneImage(ICameraDevice device)
        {
            int cameraIndex = Devices.IndexOf(device);
            return CaptrueOneImageFrame(cameraIndex, device.CameraParam.CaptrueFrame);
        }

        ///<summary>利用指定的设备拍取一张图(重载)</summary>
        public static CaptureResult CaptureOneImage(int cameraIndex)
        {
            if (cameraIndex >= Devices.Count)
            {
                throw new Exception(string.Format("指定相机索引不存在:(index={0})", cameraIndex));
            }

            return CaptureOneImage(Devices[cameraIndex]);
        }

        ///<summary>利用指定的设备拍取一张图(重载)，frame 拍照帧数</summary>
        public static CaptureResult CaptrueOneImageFrame(int cameraIndex, int frame)
        {
            if (cameraIndex >= Devices.Count)
            {
                throw new Exception(string.Format("指定相机索引不存在:(index={0})", cameraIndex));
            }

            return CaptureOneImage(Devices[cameraIndex], frame);
        }

        ///<summary>手动关闭相机</summary>
        public static void CloseCamera(ICameraDevice device)
        {
            device.CloseCamera();
            Share.Info("相机关闭");
        }

        ///<summary>关闭相机</summary>
        public static void CloseCamera()
        {
            Devices.ForEach(device =>
            {
                device.CloseCamera();
            });
        }
        //end class
    }
}
