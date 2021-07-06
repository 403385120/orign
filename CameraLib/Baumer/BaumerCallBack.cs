using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ZY.Vision.Baumer
{
    //注册回调委托事件
    public delegate void EventCallBack(byte[] imageBytes);
    

    /// <summary>
    /// 封闭相机回调功能
    /// ZhangKF 2021-1-19
    /// </summary>
    public class BaumerCallBack
    {
        ///<summary>相机设备</summary>
        BGAPI2.Device _device = null;
        BGAPI2.DataStreamList _dataStreamList = null;
        BGAPI2.DataStream _dataStream = null;
        public event EventCallBack eventCallBack = null;

        public BaumerCallBack(BGAPI2.Device device)
        {
            this._device = device;

            #region 启动相机
            _dataStreamList = device.DataStreams;
            device.DataStreams.Refresh();
            foreach (KeyValuePair<string, BGAPI2.DataStream> dst_pair in _dataStreamList)
            {
                dst_pair.Value.Open();
            }
            _dataStream = device.DataStreams.Values.First();

            var bufferList = _dataStream.BufferList;
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
            #endregion

            #region 回调事件绑定
            _dataStream.RegisterNewBufferEvent(BGAPI2.Events.EventMode.EVENT_HANDLER);
            _dataStream.NewBufferEvent += (sender, e) =>
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

                        if (this.eventCallBack != null)
                        {
                            this.eventCallBack(bytes);
                        }

                        mBufferFilled.QueueBuffer();
                    }
                    else
                    {
                        throw new Exception();
                    }
                }
                catch (BGAPI2.Exceptions.IException ex)
                {
                    string msg = string.Format("CAEX:{0} {1}", ex.GetFunctionName(), ex.GetErrorDescription());
                    Share.Error(msg);
                    Share.Error(ex.Message);
                    Share.Error(ex.StackTrace);
                }
                catch(Exception ex)
                {
                    Share.Error(ex.Message);
                    Share.Error(ex.StackTrace);
                }
                #endregion
            };
            #endregion

            _dataStream.StartAcquisition();
            //开始采集
            device.RemoteNodeList["AcquisitionStart"].Execute();
        }

        ///<summary>停止回调</summary>
        public void Stop()
        {
            //停止采集
            _device.RemoteNodeList["AcquisitionAbort"].Execute();
            _dataStream.StopAcquisition();

            //取消事件绑定
            _dataStream.UnregisterNewBufferEvent();
            _dataStream.RegisterNewBufferEvent(BGAPI2.Events.EventMode.UNREGISTERED);

            _dataStream.Close();
        }
        //end class
    }
}
