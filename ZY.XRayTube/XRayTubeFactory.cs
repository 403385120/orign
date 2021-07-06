using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZY.XRayTube
{
    public class CodeReaderFactory
    {
        public static IXRayTube CreateXRayTube(XRayTubeConfig config)
        {
            if (config.XRayTubeType == XRayTubeTypes.HamamatsuXrayTube)
            {
                return new HamamatsuXrayTube(config);
            }
            else if (config.XRayTubeType == XRayTubeTypes.ReDianXrayTube)
            {
                return new ReDianXRayTube(config);
            }

            throw new NotImplementedException();
        }
    }
}

