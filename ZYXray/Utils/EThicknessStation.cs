using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace ZYXray.Utils
{
    [Obfuscation(Exclude = false, Feature = "-rename")]
    public enum EThicknessStation
    {
        前测厚,
        后测厚
    }
}
