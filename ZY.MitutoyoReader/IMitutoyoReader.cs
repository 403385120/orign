using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZY.MitutoyoReader
{
    public interface IMitutoyoReader
    {
        List<string> PortList
        {
            get;
        }

        bool Open();
        bool Close();
        bool IsOpen();
        int Read(ref string code);
    }
}
