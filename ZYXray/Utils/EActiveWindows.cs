using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ZYXray.Utils
{
    [Obfuscation(Exclude = false, Feature = "-rename")]
    public enum EActiveWindows
    {
        Login,
        Main,
        CameraPreview,
        ManualRecheck
    }
}
