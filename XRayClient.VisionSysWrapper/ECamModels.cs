using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace XRayClient.VisionSysWrapper
{
    [Obfuscation(Exclude = false, Feature = "-rename")]
    public enum ECamModels
    {
        瓦里安 = 0,
        Baumer = 1
    }
}
