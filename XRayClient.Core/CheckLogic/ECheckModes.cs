using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace XRayClient.Core
{
    /// <summary>
    /// 检测模式
    /// </summary>
    [Obfuscation(Exclude = false, Feature = "-rename")]
    public enum ECheckModes
    {
        FourSides = 0,      // 四角检测
        Diagonal_1_2 = 1,   // AB 角
        Diagonal_3_4 = 2,  // CD 角 
    }
}
