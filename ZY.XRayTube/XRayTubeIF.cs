using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZY.SerialDevice;

namespace ZY.XRayTube
{
    public class XRayTubeIF
    {
        /// <summary>
        /// 光管的参数应由外部传入
        /// </summary>
        private static XRayTubeConfig _xtubeConfig1 = new XRayTubeConfig(new SerialDeviceConfig("COM1", 115200, 8, 0, 2), 0, 0, 10000, 0);
        private static XRayTubeConfig _xtubeConfig2 = new XRayTubeConfig(new SerialDeviceConfig("COM2", 115200, 8, 0, 2), 0, 0, 10000, 0);

        //private static IXRayTube _xrayTube1 = new HamamatsuXrayTube(_xtubeConfig1);
        //private static IXRayTube _xrayTube2 = new HamamatsuXrayTube(_xtubeConfig2);
        private static IXRayTube _xrayTube1 = new ReDianXRayTube(_xtubeConfig1);
        private static IXRayTube _xrayTube2 = new ReDianXRayTube(_xtubeConfig2);
        private CodeReaderFactory codeReaderFactory = new CodeReaderFactory();


        public static XRayTubeStatus XRayTube1Stauts
        {
            get { return _xrayTube1.Status; }
        }
        public static XRayTubeStatus XRayTube2Status
        {
            get { return _xrayTube2.Status; }
        }

        /// <summary>
        /// 初始化串口通信，创建光管控制线程,将某跟根光管的通信打开
        /// </summary>
        /// <param name="_SerialDeviceConfig">
        /// 包括两跟光管的串口参数列表，
        /// 光管1：COM1
        /// 光管2：COM2
        /// 其他都是：38400 ，8,0,0
        /// </param>
        /// <returns></returns>
        public static bool Init(XRayTubeConfig xrayConfig1, XRayTubeConfig xrayConfig2)
        {
            //_xrayTube1 = new HamamatsuXrayTube(xrayConfig1);
            //_xrayTube2 = new HamamatsuXrayTube(xrayConfig2);
            _xrayTube1 = CodeReaderFactory.CreateXRayTube(xrayConfig1);
            _xrayTube2 = CodeReaderFactory.CreateXRayTube(xrayConfig2);

            _xrayTube1.SetAlarmTime(xrayConfig1.AlarmTime);
            _xrayTube2.SetAlarmTime(xrayConfig2.AlarmTime);

            _xrayTube1.Connect();
            _xrayTube2.Connect();


            return true;
        }

        public static bool InitSingle(XRayTubeConfig xrayConfig, ETubePosition pos)
        {

            if (pos == ETubePosition.Position1)
            {
                _xrayTube1.DisConnect();
                // _xrayTube1 = new HamamatsuXrayTube(xrayConfig);
                _xrayTube1.ReloadConfig(xrayConfig);
                _xrayTube1.Connect();
            }
            else
            {
                _xrayTube2.DisConnect();
                //_xrayTube2 = new HamamatsuXrayTube(xrayConfig);
                _xrayTube1.ReloadConfig(xrayConfig);
                _xrayTube2.Connect();
            }

            return true;
        }

        public static bool DisconSingle(ETubePosition pos)
        {
            if (pos == ETubePosition.Position1)
            {
                _xrayTube1.DisConnect();
            }
            else
            {
                _xrayTube2.DisConnect();
            }

            return true;
        }

        /// <summary>
        /// 关闭某根光管的串口通信，并结束线程
        /// </summary>
        /// <returns></returns>
        public static bool UnInit()
        {
            _xrayTube1.DisConnect();
            _xrayTube2.DisConnect();

            return true;
        }

        /// <summary>
        /// 打开某一根光管
        /// </summary>
        /// <param name="index">
        /// 光管ID号，0-1
        /// </param>
        /// <returns></returns>
        public static bool OpenXray(ETubePosition pos)
        {
            if (pos == ETubePosition.Position1)
            {
                return _xrayTube1.XRayOpen();
            }
            else
            {
                return _xrayTube2.XRayOpen();
            }
        }

        /// <summary>
        /// 关闭某一根光管
        /// </summary>
        /// <param name="index">
        /// 光管ID号，0-1
        /// </param>
        /// <returns></returns>
        public static bool CloseXray(ETubePosition pos)
        {
            if (pos == ETubePosition.Position1)
            {
                _xrayTube1.XRayClose();
            }
            else
            {
                _xrayTube2.XRayClose();
            }
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public static bool AutoOpen(ETubePosition pos)
        {
            if (pos == ETubePosition.Position1)
            {
                _xrayTube1.XRayAutoOpen();
            }
            else
            {
                _xrayTube2.XRayAutoOpen();
            }
            return true;
        }

        /// <summary>
        /// 对具体的某根光管设置一个电压
        /// </summary>
        /// <param name="kv 0-90"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static bool SetXrayVoltage(int kv, ETubePosition pos)
        {
            if (pos == ETubePosition.Position1)
            {
                return _xrayTube1.SetVoltage(kv);
            }
            else
            {
                return _xrayTube2.SetVoltage(kv);
            }
        }

        /// <summary>
        /// 对具体的某根光管设置一个电流
        /// </summary>
        /// <param name="kcur  0-200"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static bool SetXrayCurrent(int kcur, ETubePosition pos)
        {
            if (pos == ETubePosition.Position1)
            {
                return _xrayTube1.SetCurrent(kcur);
            }
            else
            {
                return _xrayTube2.SetCurrent(kcur);
            }
        }

        /// <summary>
        /// 设置某根光管在一定时间内没有从外部控制设备收到指令时，设定停止X 线的时间。以秒为单位
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static bool SetStopTimeToTube(int time, ETubePosition pos)
        {
            if (pos == ETubePosition.Position1)
            {
                _xrayTube1.SetStopTime(time);
            }
            else
            {
                _xrayTube2.SetStopTime(time);
            }
            return true;
        }

        public static bool SetAlarmTimeToTube(int time, ETubePosition pos)
        {
            if (pos == ETubePosition.Position1)
            {
                _xrayTube1.SetAlarmTime(time);
            }
            else
            {
                _xrayTube2.SetAlarmTime(time);
            }
            return true;
        }
    }
}
