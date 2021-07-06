using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XRayClient.BatteryCheckManager
{
    public enum ERecheckState
    {
        Default = 0,        // 默认

        OK = 1,             // 复检OK
        NG = 2,             // 复判NG

        FQANG = 3,          // FQA判定NG(需要重判禁止上传)
        FQAOK = 4,          // FQA判定OK(结果未上传或上传失败)

        FQAUPDOK = 5,       // 已上传FQAOK的结果
        FQAUPDNG = 6,       // 已上传复判NG的结果

        //FQAUPDFAILOK = 7, // FQA OK上传失败
        //FQAUPDFAILNG = 8  // FQA NG上传失败

        WAITCHECK = 9,      //复检待判
    }
}
