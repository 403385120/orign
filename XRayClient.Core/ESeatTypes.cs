using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace XRayClient.Core
{
    /// <summary>
    /// 工位类型
    /// </summary>
    [Obfuscation(Exclude = false, Feature = "-rename")]
    public enum ESeatTypes
    {
        FrontSeat,  // 前面的工位
        BackSeat
    }
}
