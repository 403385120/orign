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
    public enum ECheckExtensions
    {
       STF,         // STF模式
       Test,        // 点检模式
       RunEmpty,    // 空跑模式(不记录数据，不执行逻辑)
       Local        //本地模式，不上传数据
    }
}
