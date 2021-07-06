using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Threading;

namespace ZY.SerialDevice
{
    public class SerialDevice
    {
        private SerialPort _port = null;
        private object _writeSync = new object();

        /// <summary>
        /// 获取设备串口列表
        /// </summary>
        public string[] PortList
        {
            get
            {
                return SerialPort.GetPortNames();
            }
        }

        /// <summary>
        /// 端口是否打开
        /// </summary>
        public bool IsPortOpen
        {
            get
            {
                if ((null != this._port) && this._port.IsOpen)
                {
                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// 打开端口
        /// </summary>
        /// <param name="devConfig"></param>
        /// <returns></returns>
        public bool OpenPort(SerialDeviceConfig devConfig)
        {
            if (!this.PortList.Contains(devConfig.PortName))
            {
                return false;
            }

            // 如果端口已打开，关闭之后重新打开
            if (this.IsPortOpen)
            {
                if (this._port.PortName == devConfig.PortName)
                {
                    return true;
                }
                else
                {
                    this._port.Close();
                }
            }
            try
            {

                this._port = new SerialPort(devConfig.PortName, devConfig.BaudRate, (Parity)devConfig.Parity, devConfig.DataBits);
                this._port.ReceivedBytesThreshold = 1;
                this._port.ReadTimeout = 500;
                if (devConfig.BaudRate == 2400)
                {
                    this._port.DtrEnable = true;
                }

                this._port.Open();
                this._port.DiscardInBuffer();
                this._port.DiscardOutBuffer();
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public void Close()
        {
            if (this.IsPortOpen)
            {
                this._port.Close();
                this._port.Dispose();
            }
        }

        public bool WriteString(string str)
        {
            this._port.ReadExisting();
            if (!this.IsPortOpen) return false;

            lock (this._writeSync)
            {
                this._port.DiscardInBuffer();
                this._port.DiscardOutBuffer();
                this._port.Write(str);
            }
            return true;
        }

        /// <summary>
        /// 读取一行数据
        /// </summary>
        /// <returns></returns>
        public string ReadLine()
        {
            string str;

            this._port.ReadTimeout = 200;
            try
            {
                str = this._port.ReadLine();
            }
            catch (Exception ex)
            {
                str = string.Empty;
            }
            if (this._port.BytesToRead > 0)
            {
                str = this._port.ReadExisting();
            }

            return str;
        }

        /// <summary>
        /// 读取到固定结束字符
        /// </summary>
        /// <param name="endString"></param>
        /// <returns></returns>
        public string ReadToString(string endString)
        {
            string str;

            this._port.ReadTimeout = 200;
            try
            {
                str = this._port.ReadTo(endString);
            }
            catch
            {
                str = string.Empty;
            }

            if (this._port.BytesToRead > 0)
            {
                this._port.ReadExisting();
            }

            return str;
        }

        public string ReadAll()
        {
            string str = "";
            str = this._port.ReadExisting();
            return str;
        }
    }
}

