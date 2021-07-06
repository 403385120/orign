using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace XRayClient.VisionSysWrapper
{
    [Obfuscation(Exclude = false, Feature = "-rename")]
    public enum ECamTypes
    {
        Camera1 = 0,
        Camera2
    }
}
