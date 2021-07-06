using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ZYXray.ViewModels
{
    [Obfuscation(Exclude = false, Feature = "-rename")]
    public enum EDashBoardActiveLayers
    {
        Main = 0,           // 主界面
        StopReasonPage = 1,     // 停机原因界面
        StartupValidate = 2,    // 开机校验 
    }
}
