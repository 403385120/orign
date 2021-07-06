using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZY.XRayTube
{
    public interface IXRayTube
    {
        XRayTubeStatus Status
        {
            get;
        }

        bool Connect();
        void DisConnect();
        void ReloadConfig(XRayTubeConfig config);

        bool XRayOpen();
        bool XRayClose();
        bool XRayAutoOpen();

        bool SetVoltage(int kv);
        bool SetCurrent(int ua);
        bool SetStopTime(int t);

        bool SetAlarmTime(int alarmTime);
    }
}

