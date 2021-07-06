using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZY.SerialDevice;

namespace ZY.XRayTube
{
    public class XRayTubeConfig
    {
        public XRayTubeConfig(SerialDeviceConfig param, int presetVoltage, int presetCurrent, int alarmTime, int stopTime, bool autoOpen=false)
        {
            SerialConfig = param;
            this.PresetVoltage = presetVoltage;
            this.PresetCurrent = presetCurrent;
            this.StopTime = stopTime;
            this.AlarmTime = alarmTime;
            this.AutoOpen = autoOpen;
        }
        public XRayTubeTypes XRayTubeType = XRayTubeTypes.HamamatsuXrayTube;
        public int PresetVoltage = 0;
        public int PresetCurrent = 0;
        public int StopTime = 0;
        public int AlarmTime = 0;
        public bool AutoOpen = true;
        public SerialDeviceConfig SerialConfig;
    }
}

