using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Instrument
{
    public class BT3562
    {
        private SerialPort _bT3562SerialPort;

        /// <summary>
        /// 初始化串口
        /// </summary>
        /// <param name="PortName">串口号</param>
        /// <param name="BaudRate">波特率</param>
        /// <param name="DataBits">数据位</param>
        /// <param name="Parity">校验位</param>
        /// <param name="StopBits">停止位</param>
        /// <returns></returns>
        public bool InitialBT3562(string PortName = "COM1", int BaudRate = 9500, int DataBits = 8, System.IO.Ports.Parity Parity = System.IO.Ports.Parity.None, System.IO.Ports.StopBits StopBits = System.IO.Ports.StopBits.One)
        {
            _bT3562SerialPort=Tool.InitCom( _bT3562SerialPort, PortName, BaudRate, DataBits, Parity, StopBits);
            if(_bT3562SerialPort != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 读取BT3562电阻值
        /// </summary>
        /// <returns>返回电阻值</returns>
        public double GetRisitance()
        {
            if (_bT3562SerialPort == null)
            {
                return 9999;
            }
            //return "0.5";
            string rec = "";
            string[] arr = new string[] { };
            try
            {
                for (int i = 0; i < 3; i++)
                {
                    string cmd = ":FETCH?" + "\r\n";
                    byte[] buffer = Encoding.ASCII.GetBytes(cmd);
                    _bT3562SerialPort.Write(buffer, 0, buffer.Length);
                    Thread.Sleep(30);
                    rec = _bT3562SerialPort.ReadTo("\r");
                    rec = rec.Trim();
                    arr = rec.Split(',');
                    if (Convert.ToDouble(arr[0]) < 100)
                    {
                        break;
                    }
                }
                return 1000 * Convert.ToDouble(arr[0]);//读到的是欧，转换成毫欧
            }
            catch (Exception ex)
            {
                return 999;
            }
        }
    }
}
