using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using BGAPI2;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Threading;

namespace ZY.Vision.Baumer
{
    /// <summary>
    /// 对宝盟相机功能的封装
    /// ZhangKF 2021-1-13
    /// </summary>
    public class BaumerWrapper
    {
        ///<summary>获取可用的相机系统</summary>
        private static List<BGAPI2.System> GetSystemList()
        {
            Share.Info("开始获取相机系统");
            Stopwatch watch = new Stopwatch();
            watch.Start();

            List<BGAPI2.System> list = new List<BGAPI2.System>();

            try
            {
                var systemList = BGAPI2.SystemList.Instance;
                systemList.Refresh();

                foreach (KeyValuePair<string, BGAPI2.System> sys_pair in BGAPI2.SystemList.Instance)
                {
                    list.Add(sys_pair.Value);
                    sys_pair.Value.Open();
                }
            }
            catch (BGAPI2.Exceptions.IException ex)
            {
                Share.Error(ex.Message);
                Share.Error(ex.StackTrace);
            }

            watch.Stop();
            Share.Info(string.Format("结束获取相机系统：{0}", watch.ElapsedMilliseconds));

            return list;
        }

        ///<summary>获取可用的相机接口</summary>
        private static List<BGAPI2.Interface> GetInterfaceList()
        {
            Share.Info("开始获取可用的相机接口");
            Stopwatch watch = new Stopwatch();
            watch.Start();

            List<BGAPI2.Interface> list = new List<Interface>();

            try
            {
                var listSystems = GetSystemList();
                listSystems.ForEach(system =>
                {
                    var interfaces = system.Interfaces;
                    system.Interfaces.Refresh(100);

                    foreach (KeyValuePair<string, BGAPI2.Interface> ifc in interfaces)
                    {
                        ifc.Value.Open();
                        list.Add(ifc.Value);
                    }
                });
            }
            catch (Exception ex)
            {
                Share.Error(ex.Message);
                Share.Error(ex.StackTrace);
            }

            watch.Stop();
            Share.Info(string.Format("结束获取相机接口:{0}", watch.ElapsedMilliseconds));

            return list;
        }

        ///<summary>获取可用的相机设备</summary>
        public static List<BGAPI2.Device> GetDevices()
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            Share.Info("开始获取可用相机设备");

            List<BGAPI2.Device> list = new List<Device>();

            try
            {
                var interfaces = GetInterfaceList();
                interfaces.ForEach(ifc =>
                {
                    ifc.Devices.Refresh(100);
                    var devices = ifc.Devices;
                    foreach (KeyValuePair<string, BGAPI2.Device> dec in devices)
                    {
                        list.Add(dec.Value);
                    }
                });
            }
            catch (Exception ex)
            {
                Share.Error(ex.Message);
                Share.Error(ex.StackTrace);
            }

            watch.Stop();
            Share.Info(string.Format("获取相机设备结束:{0}", watch.ElapsedMilliseconds));

            return list;
        }

        ///<summary>调用设备拍一张图</summary>
        public static Bitmap CaptureImage(BGAPI2.Device device)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            Share.Info("开始拍摄(CaptureImage)");

            Bitmap bmp = null;
            try
            {
                //device.Open();
                var datastreamList = device.DataStreams;
                device.DataStreams.Refresh();
                var dataStream = device.DataStreams.Values.First();
                dataStream.Open();
                var bufferList = dataStream.BufferList;

                //buffers using internal buffer mode
                for (int i = 0; i < 4; i++)
                {
                    var buffer = new BGAPI2.Buffer();
                    bufferList.Add(buffer);
                }

                foreach (KeyValuePair<string, BGAPI2.Buffer> buf_pair in bufferList)
                {
                    buf_pair.Value.QueueBuffer();
                }

                dataStream.StartAcquisition();

                //开始采集
                device.RemoteNodeList["AcquisitionStart"].Execute();

                var bufferField = dataStream.GetFilledBuffer(1000);
                if (!bufferField.IsIncomplete)
                {
                    var imgProcessor = new BGAPI2.ImageProcessor();
                    BGAPI2.Image mImage = imgProcessor.CreateImage((uint)bufferField.Width,
                        (uint)bufferField.Height,
                        (string)bufferField.PixelFormat,
                        bufferField.MemPtr,
                        (ulong)bufferField.MemSize);

                    var mTransformImage = imgProcessor.CreateTransformedImage(mImage, "Mono8");

                    var bytes = new byte[(uint)((uint)mImage.Width * (uint)mImage.Height)];
                    Marshal.Copy(mImage.Buffer, bytes, 0, (int)((int)mImage.Width * (int)mImage.Height));

                    //默认采集8位图
                    var imagePtr = Marshal.UnsafeAddrOfPinnedArrayElement(bytes, 0);
                    bmp = new Bitmap((int)mTransformImage.Width, (int)mTransformImage.Height, (int)mImage.Width, PixelFormat.Format8bppIndexed, imagePtr);
                    //var bys = bmp.ToBytes_8();
                }
                else
                {
                    throw new Exception();
                }

                device.RemoteNodeList["AcquisitionAbort"].Execute();
                dataStream.StopAcquisition();
                //bufferList.DiscardAllBuffers();
                dataStream.Close();
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
            finally
            {
                //device.Close();
            }

            watch.Stop();
            Share.Info(string.Format("拍摄完成:{0}", watch.ElapsedMilliseconds));

            return bmp;
        }

        ///<summary>采一张图并以字节的形式返回</summary>
        public static byte[] CaptureBytes(BGAPI2.Device device)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            Share.Info("开始拍摄(CaptureImage)");

            byte[] bytes = null;
            //Bitmap bmp = null;
            try
            {
                //device.Open();
                var datastreamList = device.DataStreams;
                device.DataStreams.Refresh();
                var dataStream = device.DataStreams.Values.First();
                dataStream.Open();
                var bufferList = dataStream.BufferList;

                //buffers using internal buffer mode
                for (int i = 0; i < 4; i++)
                {
                    var buffer = new BGAPI2.Buffer();
                    bufferList.Add(buffer);
                }

                foreach (KeyValuePair<string, BGAPI2.Buffer> buf_pair in bufferList)
                {
                    buf_pair.Value.QueueBuffer();
                }

                dataStream.StartAcquisition();

                //开始采集
                device.RemoteNodeList["AcquisitionStart"].Execute();

                var bufferField = dataStream.GetFilledBuffer(1000);
                if (!bufferField.IsIncomplete)
                {
                    bytes = new byte[(uint)((uint)bufferField.Width * (uint)bufferField.Height)];
                    Marshal.Copy(bufferField.MemPtr, bytes, 0, (int)((uint)bufferField.Width * (uint)bufferField.Height));
                    bufferField.QueueBuffer();
                }
                else
                {
                    throw new Exception();
                }

                device.RemoteNodeList["AcquisitionAbort"].Execute();
                dataStream.StopAcquisition();
                bufferList.DiscardAllBuffers();
                dataStream.Close();
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
            finally
            {
                //device.Close();
            }

            watch.Stop();
            Share.Info(string.Format("拍摄完成:{0}", watch.ElapsedMilliseconds));

            return bytes;
        }

        ///<summary>采集多张图以字节的形式返回(frame-采图张数)</summary>
        public static List<byte[]> CaptureBytes3(BGAPI2.Device device, int frame)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            Share.Info("开始拍摄(CaptureImage)");

            List<byte[]> listBytes = new List<byte[]>();

            try
            {
                //device.Open();
                var datastreamList = device.DataStreams;
                device.DataStreams.Refresh();
                var dataStream = device.DataStreams.Values.First();
                dataStream.Open();
                var bufferList = dataStream.BufferList;

                //buffers using internal buffer mode
                for (int i = 0; i < 4; i++)
                {
                    var buffer = new BGAPI2.Buffer();
                    bufferList.Add(buffer);
                }

                foreach (KeyValuePair<string, BGAPI2.Buffer> buf_pair in bufferList)
                {
                    buf_pair.Value.QueueBuffer();
                }

                dataStream.StartAcquisition((ulong)frame);

                //开始采集
                device.RemoteNodeList["AcquisitionStart"].Execute();

                for (var i = 0; i < frame; i++)
                {
                    var bufferField = dataStream.GetFilledBuffer(1000);
                    if (!bufferField.IsIncomplete)
                    {

                        var bytes = new byte[(uint)((uint)bufferField.Width * (uint)bufferField.Height)];
                        Marshal.Copy(bufferField.MemPtr, bytes, 0, (int)((uint)bufferField.Width * (uint)bufferField.Height));
                        listBytes.Add(bytes);

                        bufferField.QueueBuffer();
                    }
                    else
                    {
                        throw new Exception();
                    }
                }

                device.RemoteNodeList["AcquisitionAbort"].Execute();
                dataStream.StopAcquisition();

                bufferList.DiscardAllBuffers();
                dataStream.Close();
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
            finally
            {
                //device.Close();
            }

            watch.Stop();
            Share.Info(string.Format("拍摄完成:{0}", watch.ElapsedMilliseconds));

            return listBytes;
        }

        ///<summary>利用回调模式采集多张图(frame-采图张数)</summary>
        public static List<byte[]> CaptureBytes2(BGAPI2.Device device, int frame)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            Share.Info("开始拍摄(CaptureImage)");

            //采图结果
            List<byte[]> listBytes = new List<byte[]>();

            try
            {
                //device.Open();
                var datastreamList = device.DataStreams;
                device.DataStreams.Refresh();
                foreach (KeyValuePair<string, BGAPI2.DataStream> dst_pair in datastreamList)
                {
                    dst_pair.Value.Open();
                }
                var dataStream = device.DataStreams.Values.First();

                var bufferList = dataStream.BufferList;
                for (int i = 0; i < 4; i++)
                {
                    var buffer = new BGAPI2.Buffer();
                    bufferList.Add(buffer);
                }

                //给缓存排序
                foreach (KeyValuePair<string, BGAPI2.Buffer> buf_pair in bufferList)
                {
                    buf_pair.Value.QueueBuffer();
                }

                //回调事件注册
                dataStream.RegisterNewBufferEvent(BGAPI2.Events.EventMode.EVENT_HANDLER);
                dataStream.NewBufferEvent += (sender, e) =>
                {
                    #region 回调处理
                    try
                    {
                        BGAPI2.Buffer mBufferFilled = null;
                        mBufferFilled = e.BufferObj;
                        if (mBufferFilled == null)
                        {
                            throw new Exception();
                        }
                        else if (mBufferFilled.IsIncomplete == true)
                        {
                            mBufferFilled.QueueBuffer();
                        }

                        if (!mBufferFilled.IsIncomplete)
                        {
                            uint width = (uint)mBufferFilled.Width;
                            uint height = (uint)mBufferFilled.Height;
                            ulong size = mBufferFilled.SizeFilled;
                            var bytes = new byte[(uint)((uint)mBufferFilled.Width * (uint)mBufferFilled.Height)];
                            Marshal.Copy(mBufferFilled.MemPtr, bytes, 0, (int)((uint)mBufferFilled.Width * (uint)mBufferFilled.Height));

                            listBytes.Add(bytes);

                            mBufferFilled.QueueBuffer();
                        }
                        else
                        {
                            throw new Exception();
                        }
                    }
                    catch (BGAPI2.Exceptions.IException ex)
                    {
                        Share.Error(ex.Message);
                        Share.Error(ex.StackTrace);
                    }
                    #endregion
                };

                dataStream.StartAcquisition();

                //开始采集
                device.RemoteNodeList["AcquisitionStart"].Execute();
                //等待采图完成
                while (listBytes.Count < frame)
                {
                    Thread.Sleep(50);
                }

                //停止采集
                device.RemoteNodeList["AcquisitionAbort"].Execute();
                dataStream.StopAcquisition();

                //取消事件绑定
                dataStream.UnregisterNewBufferEvent();
                dataStream.RegisterNewBufferEvent(BGAPI2.Events.EventMode.UNREGISTERED);

                dataStream.Close();
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
            finally
            {
                //device.Close();
            }

            watch.Stop();
            Share.Info(string.Format("拍摄完成:{0}", watch.ElapsedMilliseconds));

            return listBytes;
        }

        ///<summary>释放相机</summary>
        public static void Destroy()
        {
            GetSystemList().ForEach(system =>
            {
                system.Close();
            });
        }
        //end class
    }
}
