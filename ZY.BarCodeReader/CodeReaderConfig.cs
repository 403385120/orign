using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZY.SerialDevice;

namespace ZY.BarCodeReader
{
    public class CodeReaderConfig
    {
        public CodeReaderConfig(SerialDeviceConfig param)
        {
            SerialConfig = param;
        }
        public CodeReaderTypes CodeReaderType = CodeReaderTypes.KeyenceSerial;

        public SerialDeviceConfig SerialConfig;
    }
}

