using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZY.MitutoyoReader
{
    public class MitutoyoReaderFactory
    {
        public static IMitutoyoReader CreateMitutoyoReaderReader(MitutoyoReaderConfig config)
        {
            if (config.MitutoyoReaderType == MitutoyoReaderTypes.MitutoyoSerial)
            {
                return new MitutoyoCodeReaderSerial(config.SerialConfig);
            }

            throw new NotImplementedException();
        }

    }
}

