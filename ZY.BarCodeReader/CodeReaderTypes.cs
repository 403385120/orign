using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ZY.BarCodeReader
{
    [Obfuscation(Exclude = false, Feature = "-rename")]
    public enum CodeReaderTypes
    {
        KeyenceSerial = 0,
        KeyenceTCP = 1,
    }
}
