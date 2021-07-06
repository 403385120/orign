using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Instrument
{
    public class RaytekMI3
    {
        private SerialPort _raytekMI3SerialPort;

        /// <summary>
        /// 初始化串口
        /// </summary>
        /// <param name="PortName">串口号</param>
        /// <param name="BaudRate">波特率</param>
        /// <param name="DataBits">数据位</param>
        /// <param name="Parity">校验位</param>
        /// <param name="StopBits">停止位</param>
        /// <returns></returns>
        public bool InitialRaytekMI3(string PortName = "COM1", int BaudRate = 9500, int DataBits = 8, System.IO.Ports.Parity Parity = System.IO.Ports.Parity.None, System.IO.Ports.StopBits StopBits = System.IO.Ports.StopBits.One)
        {
            _raytekMI3SerialPort = Tool.InitCom(_raytekMI3SerialPort, PortName, BaudRate, DataBits, Parity, StopBits);
            if (_raytekMI3SerialPort != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public double GetRaytekMI3Temperatrue()
        {
            if (_raytekMI3SerialPort == null)
            {
                return 9999;
            }
            try
            {
                double temperatrue = 0;
                string cmd = "?T" + "\r\n";
                byte[] buffer = Encoding.ASCII.GetBytes(cmd);
                _raytekMI3SerialPort.Write(buffer, 0, buffer.Length);
                Thread.Sleep(40);
                string rev = _raytekMI3SerialPort.ReadExisting();
                if (rev.Contains("!T"))
                {
                    rev = rev.Replace("!T", "").Trim();
                    temperatrue = Convert.ToDouble(rev);
                }
                return temperatrue;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

    }
}
