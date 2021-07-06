using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZY.SerialDevice;

namespace ZY.MitutoyoReader
{
    public class MitutoyoReaderConfig
    {
        public MitutoyoReaderConfig(SerialDeviceConfig param)
        {
            SerialConfig = param;
        }
        public MitutoyoReaderTypes MitutoyoReaderType = MitutoyoReaderTypes.MitutoyoSerial;

        public SerialDeviceConfig SerialConfig;
    }
}

