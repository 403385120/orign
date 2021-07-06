using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Instrument
{
    class Tool
    {
        /// <summary>
        /// 初始化串口对象
        /// </summary>
        /// <param name="Serial">串口对象</param>
        /// <param name="PortName">串口名</param>
        /// <param name="BaudRate">波特率</param>
        /// <param name="DataBits">数据位</param>
        /// <param name="Parity">奇偶校验位</param>
        /// <param name="StopBits">停止位</param>
        /// <returns></returns>
        public static SerialPort InitCom(SerialPort Serial, string PortName = "COM1", int BaudRate = 115200, int DataBits = 8, System.IO.Ports.Parity Parity = System.IO.Ports.Parity.None, System.IO.Ports.StopBits StopBits = System.IO.Ports.StopBits.One)
        {
            if (Serial == null)
            {
                Serial = new SerialPort();
                Serial.PortName = PortName;
                Serial.BaudRate = BaudRate;
                Serial.DataBits = DataBits;
                Serial.Parity = Parity;
                Serial.StopBits = StopBits;
            }
            else if (Serial.IsOpen)
            {
                return Serial;
            }

            try
            {
                Serial.ReadTimeout = 1000;
                Serial.WriteTimeout = 500;
                Serial.Open();
                Serial.DtrEnable = true;
                Serial.RtsEnable = false;
                return Serial;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static Socket InitSocketClient(Socket client, string ipAddress, int port)
        {
            try
            {
                client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPAddress ip = IPAddress.Parse(ipAddress);
                IPEndPoint ipPort = new IPEndPoint(ip, port);
                client.ReceiveTimeout = 300;
                client.Connect(ipPort);
                return client;
            }
            catch (Exception ex)
            {
                return null;
            }

        }

    }
}
