using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ZY.XRayTube
{
    /// <summary>
    /// 光管位置
    /// </summary>
    [Obfuscation(Exclude = false, Feature = "-rename")]
    public enum ETubePosition
    {
        Position1,
        Position2
    }
}
