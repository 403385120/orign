using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZY.SerialDevice
{
    public class SerialDeviceConfig
    {
        public SerialDeviceConfig(string parmname, int parmBaudRate, int parmDataBits, int parmstop, int parmParity)
        {
            PortName = parmname;
            BaudRate = parmBaudRate;
            DataBits = parmDataBits;
            StopBits = parmstop;
            Parity = parmParity;
        }
        public string PortName;
        public int BaudRate;
        public int DataBits;
        public int StopBits;
        public int Parity;
    }
}

