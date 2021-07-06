using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace XRayClient.Core
{
    [Obfuscation(Exclude = false, Feature = "-rename")]
    public enum EAlgoResults
    {
        PositionErr =   -4, // 位置错误、缺层
        NearByLayer =   -3, // 相邻层比较
        TailAlign   =   -2, // 尾部平齐
        BlackImage  =   -1,
        NG          =   0,
        OK          =   1
    }
}
