using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ZY.XRayTube
{
    [Obfuscation(Exclude = false, Feature = "-rename")]
    public enum ETubeStatus
    {
        WarmStandby = 0,    // 0-预热待机中（需要预热）
        WarmingUp =1,          // 1-预热动作中(正在预热)
        XRayStandBy,        // 2-X 线照射待机中
        XRayRadiation,      // 3-X 线照射中
        Overload,           // 4-过载保护功能动作中
        XRayDisabled,          // 5-X 线无法照射状态
        SelfChecking,       // 6-自检动作中

        OtherStatus         // 其他-其他状态或者错误
                            // 如果不为上面几种则设为此状态
    }
}
