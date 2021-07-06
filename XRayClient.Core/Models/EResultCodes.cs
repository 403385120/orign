using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace XRayClient.Core
{
    [Obfuscation(Exclude = false, Feature = "-rename")]
    public enum EResultCodes
    {
        OK          =   0x00000,     // 检测OK
        AlgoFail    =   0x00001,     // 算法检测判定NG

        ScanFail    =   0x20001,     // 扫码失败
        ShotFail    =   0x20002,     // 拍照失败

        STFCellValidatePrevCraft    =   0x30001,    // STF电芯校验失败
        STFDataXRAYAddFail          =   0x30002,    // STF上料失败
        STFCellValidateScanFail     =   0x30003,    // 扫码NG接口失败
        STFDataXRAYEditEnd          =   0x30004,    // XRay数据上传失败
        STFDataXRAYEditFinal           =   0x30005,    // 下料数据上传失败

        AlgoErr = 0x40001,         // 算法位置错误或黑白图(下面的不用)
        //PosErr   = 0x40002,         // Position Error
        //WhiteImg = 0x40003,         // 白图
        //BlackImg = 0x40004,         // 黑图
        //Broken   = 0x40005,         // 
        //ShapeNG  = 0x40006,         // Shape NG

        FoundOK   =   0x80001,      // DB记录以前的结果OK
        FoundNG   =   0x80002,      // DB记录以前的结果NG

        Unknow      =   0x99999      // 未知异常

    }

    public static class ResultHelper
    {
        /// <summary>
        /// 是否其他NG(当作扫码NG处理)
        /// </summary>
        /// <param name="resultCode"></param>
        /// <returns></returns>
        public static bool IsOtherNG(EResultCodes resultCode)
        {
            return resultCode == EResultCodes.AlgoErr
                || (resultCode >= EResultCodes.STFCellValidatePrevCraft && resultCode <= EResultCodes.STFDataXRAYEditFinal);
        }
    }
}
