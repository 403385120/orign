using System.Reflection;

namespace ZY.Logging
{
    [Obfuscation(Exclude = false, Feature = "-rename")]
    public enum LogLevels
    {
        Debug = 0,
        Info,
        Warn,
        Error
    }
}
