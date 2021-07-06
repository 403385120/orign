using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZYXray.Model
{
    public class JsonObject
    {
        public string ParamID { get; set; }
        public string StandardValue { get; set; }
        public string UpperLimitValue { get; set; }
        public string LowerLimitValue { get; set; }
        public string Description { get; set; }

    }
}
