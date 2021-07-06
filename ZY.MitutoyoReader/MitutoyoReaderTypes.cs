using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ZY.MitutoyoReader
{
    [Obfuscation(Exclude = false, Feature = "-rename")]
    public enum MitutoyoReaderTypes
    {
        MitutoyoSerial = 0
    }
}
