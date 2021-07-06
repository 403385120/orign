using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ZY.XRayTube
{
    [Obfuscation(Exclude = false, Feature = "-rename")]
    public enum XRayTubeTypes
    {
        HamamatsuXrayTube = 0,  //0代表滨松光管
        ReDianXrayTube = 1,//热电光管
    }
}
