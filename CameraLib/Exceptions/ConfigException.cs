using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZY.Vision.Exceptions
{
    /// <summary>
    /// 配置不正确异常
    /// ZhangKF 2020-12-22
    /// </summary>
    public class ConfigException:Exception
    {
        public ConfigException(string msg) : base(msg)
        {

        }
    }
}
