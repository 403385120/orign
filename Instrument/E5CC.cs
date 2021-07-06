using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ZY.Logging;

namespace Instrument
{
    public class E5CC
    {
        private SerialPort _e5CCSerialPort;

        /// <summary>
        /// 初始化串口
        /// </summary>
        /// <param name="PortName">串口号</param>
        /// <param name="BaudRate">波特率</param>
        /// <param name="DataBits">数据位</param>
        /// <param name="Parity">校验位</param>
        /// <param name="StopBits">停止位</param>
        /// <returns></returns>
        public bool InitialE5CC(string PortName = "COM1", int BaudRate = 9500, int DataBits = 8, System.IO.Ports.Parity Parity = System.IO.Ports.Parity.None, System.IO.Ports.StopBits StopBits = System.IO.Ports.StopBits.One)
        {
            _e5CCSerialPort = Tool.InitCom(_e5CCSerialPort, PortName, BaudRate, DataBits, Parity, StopBits);
            if (_e5CCSerialPort != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public double GetE5CCTemperatrue()
        {
            if (_e5CCSerialPort == null)
            {
                return 9999;
            }
            byte[] buffer = new byte[8];
            List<string> listReceive = new List<string>();
            List<double> listData = new List<double>();
            buffer[0] = 0X01;
            buffer[1] = 0X03;
            buffer[2] = 0X00;
            buffer[3] = 0X00;
            buffer[4] = 0X00;
            buffer[5] = 0X02;
            buffer[6] = 0XC4;
            buffer[7] = 0X0B;

            double temperat = 0;
            try
            {
                _e5CCSerialPort.Write(buffer, 0, buffer.Length);
                Thread.Sleep(150);
                int n = _e5CCSerialPort.BytesToRead;
                byte[] receivebBuffer = new byte[n];
                if (receivebBuffer.Length > 0)
                {
                    _e5CCSerialPort.Read(receivebBuffer, 0, n);
                    StringBuilder sb = new StringBuilder();
                    for (int i = 0; i < receivebBuffer.Length; i++)
                    {
                        sb.AppendFormat("{0:x2}" + " ", receivebBuffer[i]);
                    }

                    listReceive = sb.ToString().Split(' ').ToList();
                    for (int i = 5; i < listReceive.Count - 3; i = i + 1)
                    {
                        listData.Add((Convert.ToInt32(listReceive[i], 16) * 256 + Convert.ToUInt32(listReceive[i + 1], 16)) / 10.0);
                    }
                    temperat = listData[0];
                }
                else
                {
                    LoggingIF.Log("receivebBuffer.Length = 0", LogLevels.Warn, "GetE5CCTemperatrue");
                }
                return temperat;

            }
            catch (Exception ex)
            {
                LoggingIF.Log("环境温度读数异常:" + ex.Message, LogLevels.Warn, "GetE5CCTemperatrue");
                return temperat;
            }
        }
    }
}
