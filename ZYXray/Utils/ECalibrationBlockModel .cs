using System.Reflection;

namespace ZYXray.Utils
{
    [Obfuscation(Exclude = false, Feature = "-rename")]
    public enum ECalibrationBlockModel
    {
        小标定块,
        中标定块,
        大标定块
    }
}
