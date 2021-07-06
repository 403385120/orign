using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATL.MES
{
    public class backserver
    {
        public int ID { get; set; }
        public int backupLocalUdpSendPortNo { get; set; }
        public int backupLocalUdpRecvPortNo { get; set; }
        public int backupLocalTcpPortNo { get; set; }
        public string backupServerIpAdr { get; set; }
        public int backupServerUdpPortNo { get; set; }
    }
}
