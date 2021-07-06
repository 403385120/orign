using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using ZY.SerialDevice;

namespace ZY.BarCodeReader
{
    public class KeyenceBarCodeReaderSerial : ICodeReader
    {
        private SerialDeviceConfig _deviceConfig;
        private SerialDevice.SerialDevice _serialDevice = new ZY.SerialDevice.SerialDevice();

        public KeyenceBarCodeReaderSerial(SerialDeviceConfig deviceConfig)
        {
            this._deviceConfig = deviceConfig;
        }

        public List<string> PortList
        {
            get { return _serialDevice.PortList.ToList(); }
        }

        /// <summary>
        /// 关闭串口通道
        /// </summary>
        /// <returns></returns>
        public bool Close()
        {
            _serialDevice.Close();
            return true;
        }

        /// <summary>
        /// 打开串口通道
        /// </summary>
        /// <returns></returns>
        public bool Open()
        {
            if (_serialDevice.IsPortOpen == true)
            {
                Close();
            }
            return _serialDevice.OpenPort(_deviceConfig);
        }

        /// <summary>
        /// 获取是否打开
        /// </summary>
        /// <returns></returns>
        public bool IsOpen()
        {
            return _serialDevice.IsPortOpen;
        }

        /// <summary>
        /// 读取二维码
        /// </summary>
        /// <param name="code">
        /// 存储二维码的字符串变量
        /// </param>
        /// <returns></returns>
        public int Read(ref string code)   //返回值是code
        {
            code = string.Empty;

            if (!_serialDevice.IsPortOpen)
            {
                return -1;
            }

            string endstr = "\r";   //结束符
            _serialDevice.WriteString("LON\r");
            Thread.Sleep(100);
            code = _serialDevice.ReadToString(endstr);
            
            // TODO: 检查这里不关闭会不会一直闪烁影响成像
            _serialDevice.WriteString("LOFF\r");

            return 0;
        }
    }
}


