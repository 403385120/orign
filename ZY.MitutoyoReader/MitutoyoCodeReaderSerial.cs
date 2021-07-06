using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using ZY.SerialDevice;

namespace ZY.MitutoyoReader
{
    public class MitutoyoCodeReaderSerial : IMitutoyoReader
    {
        private SerialDeviceConfig _deviceConfig;
        private SerialDevice.SerialDevice _serialDevice = new ZY.SerialDevice.SerialDevice();

        public MitutoyoCodeReaderSerial(SerialDeviceConfig deviceConfig)
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
        /// 读取厚度
        /// </summary>
        /// <param name="thickness">
        /// 存储厚度的字符串变量
        /// </param>
        /// <returns></returns>
        public int Read(ref string thickness)   //返回值是thickness
        {
            thickness = string.Empty;

            if (!_serialDevice.IsPortOpen)
            {
                return -1;
            }

            try
            {
                bool bl = true;
                int ReadCount = 0;
                while (bl)
                {
                    string endstr = "\r";   //结束符
                    _serialDevice.WriteString("1\r\n");
                    Thread.Sleep(300);
                    thickness = _serialDevice.ReadToString(endstr);
                    if (string.IsNullOrEmpty(thickness))
                    {
                        ReadCount++;
                        if (ReadCount > 4)
                        {
                            bl = false;
                            thickness = "-1";
                        }
                        Thread.Sleep(500);

                    }
                    else
                    {
                        bl = false;
                    }
                }

            }
            catch (Exception)
            {
                thickness = "0";
            }

            return 0;
        }
    }
}


