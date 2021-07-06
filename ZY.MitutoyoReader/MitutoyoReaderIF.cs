using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZY.SerialDevice;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Windows;

namespace ZY.MitutoyoReader
{
    public class MitutoyoReaderIF
    {
        #region   串口通信
        /// <summary>
        /// 串口的参数应由外部配置得到
        /// </summary>
        private static MitutoyoReaderConfig _mitutoyoReaderConfig = new MitutoyoReaderConfig(new SerialDeviceConfig("COM3", 2400, 8, 0, 0));
        private static IMitutoyoReader _mitutoyoReader = new MitutoyoCodeReaderSerial(_mitutoyoReaderConfig.SerialConfig);

        private static MitutoyoReaderConfig _mitutoyoReaderConfig2 = new MitutoyoReaderConfig(new SerialDeviceConfig("COM1", 2400, 8, 0, 0));
        private static IMitutoyoReader _mitutoyoReader2 = new MitutoyoCodeReaderSerial(_mitutoyoReaderConfig.SerialConfig);

        private static MitutoyoReaderConfig _mitutoyoReaderConfig3 = new MitutoyoReaderConfig(new SerialDeviceConfig("COM1", 2400, 8, 0, 0));
        private static IMitutoyoReader _mitutoyoReader3 = new MitutoyoCodeReaderSerial(_mitutoyoReaderConfig.SerialConfig);

        private static MitutoyoReaderConfig _mitutoyoReaderConfig4 = new MitutoyoReaderConfig(new SerialDeviceConfig("COM1", 2400, 8, 0, 0));
        private static IMitutoyoReader _mitutoyoReader4 = new MitutoyoCodeReaderSerial(_mitutoyoReaderConfig.SerialConfig);

        public MitutoyoReaderConfig MyMitutoyoReaderConfig
        {
            get { return _mitutoyoReaderConfig; }
        }

        /// <summary>
        /// 端口列表
        /// </summary>
        public static List<string> PortList
        {
            get
            {
                return _mitutoyoReader.PortList;
            }
        }

        /// <summary>
        /// 初始化，将串口打开
        /// </summary>
        /// <param name="_SerialDeviceConfig">
        /// 串口号：COM3-COM6按实际情况
        /// 波特率：默认115200
        /// 数据位：默认8
        /// 停止为：0
        /// 奇偶位：偶
        /// </param>
        /// <returns></returns>
        public static bool Init(MitutoyoReaderConfig readerConfig, int num)
        {
            if (num == 1)
            {
                _mitutoyoReaderConfig = readerConfig;

                _mitutoyoReader = MitutoyoReaderFactory.CreateMitutoyoReaderReader(_mitutoyoReaderConfig);
                _mitutoyoReader.Open();
            }
            else if (num == 2)
            {
                _mitutoyoReaderConfig2 = readerConfig;

                _mitutoyoReader2 = MitutoyoReaderFactory.CreateMitutoyoReaderReader(_mitutoyoReaderConfig2);
                _mitutoyoReader2.Open();
            }
            else if (num == 3)
            {
                _mitutoyoReaderConfig3 = readerConfig;

                _mitutoyoReader3 = MitutoyoReaderFactory.CreateMitutoyoReaderReader(_mitutoyoReaderConfig3);
                _mitutoyoReader3.Open();
            }
            else
            {
                _mitutoyoReaderConfig4 = readerConfig;

                _mitutoyoReader4 = MitutoyoReaderFactory.CreateMitutoyoReaderReader(_mitutoyoReaderConfig4);
                _mitutoyoReader4.Open();
            }



            return true;
        }

        /// <summary>
        /// 关闭串口通信
        /// </summary>
        /// <returns></returns>
        public static bool UnInit()
        {
            _mitutoyoReader.Close();
            _mitutoyoReader2.Close();
            _mitutoyoReader3.Close();
            _mitutoyoReader4.Close();

            return true;
        }

        /// <summary>
        /// 使用新的端口重新打开
        /// </summary>
        /// <param name="port"></param>
        /// <returns></returns>
        public static bool ReOpen(string portName, int num)
        {
            if (num == 1)
            {
                _mitutoyoReaderConfig.SerialConfig.PortName = portName;

                return _mitutoyoReader.Open();
            }
            else if (num == 2)
            {
                _mitutoyoReaderConfig2.SerialConfig.PortName = portName;

                return _mitutoyoReader2.Open();
            }
            else if (num == 3)
            {
                _mitutoyoReaderConfig3.SerialConfig.PortName = portName;

                return _mitutoyoReader3.Open();
            }
            else 
            {
                _mitutoyoReaderConfig4.SerialConfig.PortName = portName;

                return _mitutoyoReader4.Open();
            }

        }

        private static object objLockRead=new object();  

        /// <summary>
        /// 读取厚度
        /// </summary>
        /// <param name="thickness"> 将存储的厚度放在thickness里面</param>
        /// <returns></returns>
        public static bool ReadThickness(ref string thickness, int num)//num=1对应A测厚模组，num=2对应B测厚模组
        {
            //静态方法被多线程同时访问有冲突风险，必须加锁
            lock (objLockRead)
            {
                try
                {
                    if (num == 1)
                    {
                        _mitutoyoReader.Read(ref thickness);
                    }
                    else if (num == 2)
                    {
                        _mitutoyoReader2.Read(ref thickness);
                    }
                    else if (num == 3)
                    {
                        _mitutoyoReader3.Read(ref thickness);
                    }
                    else
                    {
                        _mitutoyoReader4.Read(ref thickness);
                    }

                }
                catch { }

                return false;
            }
        }
        /// <summary>
        /// 厚度表串口是否打开
        /// </summary>
        /// <returns></returns>
        public static bool IsOpen()
        {
            return _mitutoyoReader.IsOpen();
        }

        #endregion
    }
}
