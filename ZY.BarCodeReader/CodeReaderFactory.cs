using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZY.BarCodeReader
{
    public class CodeReaderFactory
    {
        public static ICodeReader CreateCodeReader(CodeReaderConfig config)
        {
            if (config.CodeReaderType == CodeReaderTypes.KeyenceSerial)
            {
                return new KeyenceBarCodeReaderSerial(config.SerialConfig);
            }

            throw new NotImplementedException();
        }

    }
}

