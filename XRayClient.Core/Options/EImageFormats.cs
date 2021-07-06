using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace XRayClient.Core
{
    [Obfuscation(Exclude = false, Feature = "-rename")]
    public enum EImageFormats
    {
        BMP,
        JPG
    }
}
