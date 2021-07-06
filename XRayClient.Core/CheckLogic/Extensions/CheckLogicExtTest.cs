using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ZY.Logging;

namespace XRayClient.Core
{
    /// <summary>
    /// 点检扩展
    /// </summary>
    public class CheckLogicExtTest : ICheckLogicExt
    {
        //private int _ngCount = 0;

        public void OnScanComplete(BatterySeat seat, CheckStatus checkStatus)
        {
           if(seat.Sn == string.Empty || seat.Sn.Equals("Empty"))
            {
               
            }
        }

        public void OnUpdateResult(BatterySeat seat)
        {
            seat.UpdateResult();
        }

        // 点检料到此肯定是有条码的--点检跑
        public void OnCheckFinished(BatterySeat seat, ImageSaveConfig imgSaveConfig, ref CheckStatus status)
        {
            LoggingIF.Log("Updating image file names...", LogLevels.Debug, "CheckLogicExtTest");
            seat.UpdateFileName(imgSaveConfig, false, true);
        }

        public void OnSaveImages(BatterySeat seat, ImageSaveConfig imgSaveConfig)
        {
            try
            {
                seat.SaveAll(imgSaveConfig, false, true);
            }
            catch (Exception ex)
            {
            }
        }

        public void OnReset()
        {

        }
    }
}
