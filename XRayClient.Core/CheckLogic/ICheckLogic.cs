using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XRayClient.VisionSysWrapper;

namespace XRayClient.Core
{
    public interface ICheckLogic
    {
        BatterySeat ResultSeat { get; }

        CheckStatus MyCheckStatus { get; }

        void LoadWorkParams(CheckParamsConfig checkParams, CameraParams camParam1, CameraParams camParam2);
        void Destroy();

        void InspectTest(string imgPath, ref string imgLeftUp, ref string imgLeftDown,
                         ref string imgRightUp, ref string imgRightDown, bool autoDetect, ref List<float> timeList);

        bool OnHeadShotSignal1();
        bool OnHeadShotSignal2();

        bool OnTailShotSignal1();
        bool OnTailShotSignal2();

        bool OnScanCodeSignal();
        bool OnAllowsSendPhotoSignal();
        bool OnPlcSystemResetSignal();

        bool OnCycleMoveSignal();

        bool OnErrOccurred();
    }
}
