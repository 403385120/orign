using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ZY.Logging;
using XRayClient.VisionSysWrapper;
using Shuyz.Framework.Mvvm;
using ZY.BarCodeReader;
using ZY.XRayTube;
using System.Text.RegularExpressions;
using System.Drawing;
using System.IO;
using ZY.Vision;

namespace XRayClient.Core
{
    //recompose by fjy
    class TwoSides13CheckLogic : ICheckLogic
    {
        private bool _isInited = false;

        private ManualResetEvent _scanSyncEvent = new ManualResetEvent(false);
        //private readonly int _scanTimeout = 3000;                        // Time out for scanner after OnHeadShotSignal1

        private ManualResetEvent _algoSyncEvent = new ManualResetEvent(false);
        //private readonly int _algoTimeout = 2000;                       // Algo timeout between two calls

        private WorkingSeats _workingSeats = new WorkingSeats();        // Cycle queue for two parallel seats
        private CameraParams _camParam1 = new CameraParams();
        private CameraParams _camParam2 = new CameraParams();
        private ImageSaveConfig _imageSaveConfig = new ImageSaveConfig();

        private BatterySeat _resultSeat = new BatterySeat();
        private CheckStatus _checkStatus = new CheckStatus();

#if SYNC_SEAT_BY_LOGIC
        private bool _hasBatOnFrontSeat = false;
        private ManualResetEvent _seatSyncEvent = new ManualResetEvent(false); // Sync for switching seat between OnHeadShotSignal2 and OnTailShotSignal2
        //private readonly int _seatSyncTimeout = 2000; 
#endif

        private ManualResetEvent _resultSyncEvent = new ManualResetEvent(false); // Sync between TailShot2 and OnHeadShotSignal2
        //private readonly int _resultSyncTimeout = 2000;

        private bool _hasBatOnExitHand = false;
        private ManualResetEvent _exitHandSyncEvent = new ManualResetEvent(false);  // Sync between handle result and OnTailShotSignal2 CopyResult

        private bool _resetSignalReceived = false;
        private readonly int _resetProcessTime = 5000;                  // timeout waiting all status to clear after receiving a reset signal
        private DateTime _lastResetTime = DateTime.MinValue;

        private ICheckLogicExt _extSTF = new CheckLogicExtSTF();                //STF正常跑料
        private ICheckLogicExt _extRunEmpty = new CheckLogicExtRunEmpty();      //空跑
        private ICheckLogicExt _extTest = new CheckLogicExtTest();              //点检

        private ICheckLogicExt CheckLogicExt
        {
            get
            {
                switch (this._checkStatus.ActiveCheckExtension)
                {
                    case ECheckExtensions.STF: return this._extSTF;                //STF正常跑料
                    case ECheckExtensions.RunEmpty: return this._extRunEmpty;      //空跑
                    case ECheckExtensions.Test: return this._extTest;              //点检

                    default: throw new NotImplementedException();
                }
            }
        }

        public TwoSides13CheckLogic()
        {
            //_resultSeat already created
        }

        //recompose by fjy
        private bool GetReady()
        {
            bool result = true;

            LoggingIF.Log(string.Format("Get ready for mode {0}", ECheckModes.Diagonal_1_2), LogLevels.Info, "TwoSides13CheckLogic");
#if SYNC_SEAT_BY_LOGIC
            this._hasBatOnFrontSeat = false;
#endif
            this._resultSeat.Reset();
            this._workingSeats.ClearAll();

            //TODO: Send other configurations to PLC

            return result;
        }

        /// <summary>
        /// Switch between Test mode and user-set mode
        /// </summary>
        /// <param name="sn"></param>
        private void CheckLogicExtBroker(string sn)
        {
            if (!this.MyCheckStatus.MyStartupTestConfig.IsEnabled)
            {
                this.MyCheckStatus.ActiveCheckExtension = this.MyCheckStatus.CheckExtension;
                return;
            }

            // @else code is not for testing

            // If user wants to stay in test mode, we need to check nothing
            if (this.MyCheckStatus.CheckExtension == ECheckExtensions.Test) return;

            // We're leaving test mode
            if (this.MyCheckStatus.ActiveCheckExtension == ECheckExtensions.Test)
            {
                this._checkStatus.MyStartupTestConfig.lastTestTime = DateTime.Now;

                CheckParamsConfig.Instance.SaveStartupConfig();
            }
            // We're not in test mode ever, check if test is necessary
            else
            {
                if ((DateTime.Now - this._checkStatus.MyStartupTestConfig.lastTestTime).TotalHours > this._checkStatus.MyStartupTestConfig.testInterval)
                {
                }
            }

            // Restore user-set mode
            this.MyCheckStatus.ActiveCheckExtension = this.MyCheckStatus.CheckExtension;
        }

        /// <summary>
        /// Exit
        /// </summary>
        /// <param name="sn"></param>
        private void CheckLogicExtBrokerEnd(string sn)
        {
        }

        /// <summary>
        /// Switch the seat position when when leaving corner2(another seat)    //recompose by fjy
        /// </summary>
        private void SwitchSeat()
        {
            // TODO: We have to make sure this event is raised before OnTailShotSignal1 to avoid putting data in the wrong position
            LoggingIF.Log("Switch seat position", LogLevels.Debug, "TwoSides13CheckLogic");
            this._workingSeats.Switch();

            this._workingSeats.BackSeat.Corner1.Reset();
            this._workingSeats.FrontSeat.Corner3.Reset();

            //保险起见，2、4角也清理一下
            this._workingSeats.BackSeat.Corner2.Reset();
            this._workingSeats.FrontSeat.Corner4.Reset();
        }

        public BatterySeat ResultSeat
        {
            get { return this._resultSeat; }
        }

        public CheckStatus MyCheckStatus
        {
            get { return this._checkStatus; }
        }

        //recompose by fjy
        public void LoadWorkParams(CheckParamsConfig checkParams, CameraParams camParam1, CameraParams camParam2)
        {
            this._camParam1 = camParam1;
            this._imageSaveConfig = checkParams.MyImageSaveConfig;
            this._checkStatus.CheckMode = checkParams.CheckMode;

            for (int i = 0; i < 2; i++)
            {
                BatterySeat seat = null;
                if (i == 0) seat = this._workingSeats._seat1;
                else if (i == 1) seat = this._workingSeats._seat2;

                seat.CheckMode = ECheckModes.Diagonal_1_2;
                checkParams.MyInspectParams.CopyTo(ref seat.Corner1.InspectParams);
                seat.Corner1.InspectParams.iCorner = 1;
                checkParams.MyInspectParams.CopyTo(ref seat.Corner3.InspectParams);
                seat.Corner3.InspectParams.iCorner = 3;

                // Use alternative parameters for corner 1,3
                seat.Corner1.InspectParams.total_layer = checkParams.TotalLayer;
                seat.Corner1.InspectParams.detected_rect.width = checkParams.RectWidth;
                seat.Corner1.InspectParams.detected_rect.height = checkParams.RectHeight;

                seat.Corner3.InspectParams.total_layer = checkParams.TotalLayersBD;
                seat.Corner3.InspectParams.detected_rect.width = checkParams.RectWidthBD;
                seat.Corner3.InspectParams.detected_rect.height = checkParams.RectHeightBD;

                // Set pixel ratio
                seat.Corner1.InspectParams.pixel_to_mm = this._camParam1.pixelRatio;
                seat.Corner3.InspectParams.pixel_to_mm = this._camParam1.pixelRatio;
            }

            // Allow only once while system is started
            if (!this._isInited)
            {
                this._isInited = true;
                this.GetReady();
            }
        }

        public void Destroy()
        {
            this._workingSeats.BackSeat.Destroy();
            this._workingSeats.FrontSeat.Destroy();
            this._resultSeat.Destroy();
        }

        public void InspectTest(string imgPath, ref string imgLeftUp, ref string imgLeftDown,
                                ref string imgRightUp, ref string imgRightDown, bool autoDetect, ref List<float> timeList)
        {
            if (autoDetect)
            {
                string[] files = Directory.GetFiles(imgPath);
                foreach (var str in files)
                {
                    string f = str.ToUpper();
                    if (!f.EndsWith(".BMP") && !f.EndsWith(".jpg")) continue;

                    if (f.EndsWith("_1.BMP") || f.EndsWith("_1.jpg")) imgLeftUp = str;
                    else if (f.EndsWith("_2.BMP") || f.EndsWith("_2.jpg")) imgLeftDown = str;
                    else if (f.EndsWith("_3.BMP") || f.EndsWith("_3.jpg")) imgRightDown = str;
                    else if (f.EndsWith("_4.BMP") || f.EndsWith("_4.jpg")) imgRightUp = str;
                    else continue;
                }
            }

            for (int i = 0; i < 4; i++)
            {
                if (i == 1 || i == 3) continue;

                BatteryCorner corner = null;
                string bmpName = string.Empty;

                if (i == 0) { corner = this._workingSeats._seat1.Corner1; bmpName = imgLeftUp; }
                else if (i == 1) { corner = this._workingSeats._seat1.Corner2; bmpName = imgLeftDown; }
                else if (i == 2) { corner = this._workingSeats._seat1.Corner3; bmpName = imgRightDown; }
                else { corner = this._workingSeats._seat1.Corner4; bmpName = imgRightUp; }

                if (!File.Exists(bmpName))
                {
                    throw new Exception("File does not exists " + bmpName);
                }

                using (Bitmap bmp = new Bitmap(bmpName))
                {
                    if (bmp.Width != corner.ShotImage.width
                        || bmp.Height != corner.ShotImage.height
                        || Image.GetPixelFormatSize(bmp.PixelFormat) / 8 != corner.ShotImage.channel)
                    {
                        throw new Exception("Illegal width, height or channel for image " + bmpName);
                    }

                    //TOOD: 替换新版旧版相机采图  ZhangKF 2021-3-16
                    //corner.ShotImage.UpdateWithBitmap(bmp);
                    corner.ShotImage = bmp.ToTransfor(Consts.ImageTypes.Sixteen);
                    corner.IsShotOK = true;
                }
            }

            List<float> tmpList = new List<float>();
            List<float> retList = new List<float>();

            DateTime startTime = DateTime.Now;
            AlgoWrapper.Instance.InspectPreCorner(this._workingSeats._seat1.Corner1);
            tmpList.Add((float)((DateTime.Now - startTime).TotalMilliseconds));
            startTime = DateTime.Now;
            AlgoWrapper.Instance.InspectBackCorner(this._workingSeats._seat1.Corner3);
            tmpList.Add((float)((DateTime.Now - startTime).TotalMilliseconds));
            AlgoWrapper.Instance.GetInspectResult(ref this._workingSeats._seat1.Corner1.InspectResults, ref this._workingSeats._seat1.Corner3.InspectResults);
            retList.Add(this._workingSeats._seat1.Corner1.InspectResults.resultDataMin.iResult);
            retList.Add(this._workingSeats._seat1.Corner3.InspectResults.resultDataMin.iResult);

            this._workingSeats._seat1.UpdateResult();                              //判断电芯总结果及具体NG原因
            AlgoWrapper.Instance.GetResultImage(ref this._workingSeats._seat1);    //获取角1、2、3、4图、结果图、Barcord图、最终结果值

            this._workingSeats._seat1.CopyTo(ref this._resultSeat);
            ResultRelay.Instance.CheckResultHandlers(this._resultSeat);            //更新界面显示的图片

            this._workingSeats._seat1.UpdateFileName(this._imageSaveConfig, false);
            this._workingSeats._seat1.ResultImgSavePath = Path.Combine("D:\\XRayPic\\AlgoTest\\Test", DateTime.Now.ToString("yyyyMMddHH"));
            this._workingSeats._seat1.OrigImgSavePath = Path.Combine("D:\\XRayPic\\AlgoTest\\Orig", DateTime.Now.ToString("yyyyMMddHH"));
            this._workingSeats._seat1.SaveAll(this._imageSaveConfig, false);      //存图

            if (tmpList.Count == 2)
            {
                timeList.Add(tmpList[0]);
                timeList.Add(tmpList[1]);
            }

            if (retList.Count == 2)
            {
                timeList.Add(retList[0]);
                timeList.Add(retList[1]);
            }
        }

        public bool OnHeadShotSignal1()
        {
            this._scanSyncEvent.Reset();

            LoggingIF.Log(string.Format("Camera {0} shot on OnHeadShotSignal1 => corner1", ECamTypes.Camera1), LogLevels.Debug, "TwoSides13CheckLogic");

            //TODO: 采用新版相机拍照类 ZhangKF 2020-3-16
            #region Disable
            //ImageTransfor img = this._workingSeats.BackSeat.Corner1.ShotImage;
            //int iRet = VisionSysWrapperIF.ShotAndAverage(ECamTypes.Camera1, this._camParam1.PinValue, ref img);
            //LoggingIF.Log(string.Format("Camera {0} shot on OnHeadShotSignal1 => corner1 finished.", ECamTypes.Camera1), LogLevels.Debug, "FourSidesCheckLogic");
            //if (iRet != 0)
            //{
            //}
            #endregion
            var capResult = CameraHelper.CaptureOneImage(0);
            this._workingSeats.BackSeat.Corner1.ShotImage = capResult.Bitmap.ToTransfor(Consts.ImageTypes.Sixteen);
            //this._workingSeats.BackSeat.Corner1.IsShotOK = (iRet == 0 ? true : false);
            this._workingSeats.BackSeat.Corner1.IsShotOK = capResult.Result;
            this._workingSeats.BackSeat.Corner1.ShotTime = DateTime.Now;
            this._workingSeats.BackSeat.Corner1.MyXRayTubeStatus = XRayTubeIF.XRayTube1Stauts.DeepCopy();

            LoggingIF.Log(string.Format("等待扫码完成 by _scanSyncEvent.WaitOne"), LogLevels.Debug, "TwoSides13CheckLogic");
            var waitSignal = this._scanSyncEvent.WaitOne(/*this._scanTimeout*/);
            if (this._resetSignalReceived) return false;
            if (!waitSignal)
            {
                string errMsg = "Timeout waitting for OnScanCodeSignal finish on OnHeadShotSignal1";
                LoggingIF.Log(errMsg, LogLevels.Error, "TwoSides13CheckLogic");

            }

#if SYNC_SEAT_BY_LOGIC
            if (this._hasBatOnFrontSeat)
            {
                LoggingIF.Log(string.Format("等待front工位空闲 by _seatSyncEvent.WaitOne"), LogLevels.Debug, "TwoSides13CheckLogic");
                waitSignal = this._seatSyncEvent.WaitOne(/*this._seatSyncTimeout*/);
                if (this._resetSignalReceived) return false;
            }
            this.SwitchSeat();
#endif
            LoggingIF.Log(string.Format("发送头部1拍照完成SendHeadShotFinish1"), LogLevels.Debug, "TwoSides13CheckLogic");


#if SYNC_SEAT_BY_LOGIC
            LoggingIF.Log(string.Format("置front工位为忙碌 by _seatSyncEvent.Reset"), LogLevels.Debug, "TwoSides13CheckLogic");
            this._hasBatOnFrontSeat = true;
            _seatSyncEvent.Reset();
#endif

            return true;
        }

        //不用  //recompose by fjy
        public bool OnHeadShotSignal2()
        {
            LoggingIF.Log(string.Format("receive ShotSignal On HeadShotSignal2"), LogLevels.Debug, "TwoSides13CheckLogic");

            return true;
        }

        //角3   //recompose by fjy
        public bool OnTailShotSignal1()
        {
            LoggingIF.Log(string.Format("跑角1算法前，置算法为忙碌 by _algoSyncEvent.Reset"), LogLevels.Debug, "TwoSides13CheckLogic");
            this._algoSyncEvent.Reset();

            // Start calculating while shotting a image
            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                LoggingIF.Log("Call image algo for corner 1-3 => 1...", LogLevels.Debug, "TwoSides13CheckLogic");
                try
                {
                    AlgoWrapper.Instance.InspectPreCorner(this._workingSeats.FrontSeat.Corner1);
                }
                catch (Exception ex)
                {
                    LoggingIF.Log("Call image algo for corner 1-3 => 1 with error " + ex.Message.ToString(), LogLevels.Error, "TwoSides13CheckLogic");
                    this._workingSeats.FrontSeat.Corner1.InspectResults.resultDataMin.iResult = -1;
                }
                LoggingIF.Log("Call image algo for corner 1-3 => 1 finished.", LogLevels.Debug, "TwoSides13CheckLogic");

                LoggingIF.Log(string.Format("跑角1算法后，置算法为空闲 by _algoSyncEvent.Set"), LogLevels.Debug, "TwoSides13CheckLogic");
                this._algoSyncEvent.Set();
            }).Start();

            LoggingIF.Log(string.Format("Camera {0} shot on OnTailShotSignal1 => corner3", ECamTypes.Camera2), LogLevels.Debug, "TwoSides13CheckLogic");

            //TODO: 替换旧版采图 ZhangKF 2021-3-16
            //ZYImageStruct img = this._workingSeats.FrontSeat.Corner3.ShotImage;
            //int k = 0;
            //int iRet = VisionSysWrapperIF.ShotAndAverage(ECamTypes.Camera2, this._camParam2.PinValue, ref img,4,out k);
            //this._workingSeats.FrontSeat.Corner3.ImgNO = k;
            //LoggingIF.Log(string.Format("Camera {0} shot on OnTailShotSignal1 => corner3 finished.", ECamTypes.Camera2), LogLevels.Debug, "TwoSides13CheckLogic");
            //if (iRet != 0)
            //{
            //}
            var result = CameraHelper.CaptureOneImage(1);
            this._workingSeats.FrontSeat.Corner3.ShotImage = result.Bitmap.ToTransfor(Consts.ImageTypes.Sixteen);
            //this._workingSeats.FrontSeat.Corner3.IsShotOK = (iRet == 0 ? true : false);
            this._workingSeats.FrontSeat.Corner3.IsShotOK = result.Result;
            this._workingSeats.FrontSeat.Corner3.ShotTime = DateTime.Now;

            LoggingIF.Log(string.Format("等待算法空闲 by _algoSyncEvent.WaitOne"), LogLevels.Debug, "TwoSides13CheckLogic");
            var waitSignal = this._algoSyncEvent.WaitOne();
            if (this._resetSignalReceived) return false;
            if (!waitSignal)
            {
                string errMsg = "Timeout waitting for algoSyncEvent on OnTailShotSignal1";
                LoggingIF.Log(errMsg, LogLevels.Error, "TwoSides13CheckLogic");

            }

            if (this._hasBatOnExitHand)
            {
                LoggingIF.Log("Waitting result seat to be available.", LogLevels.Debug, "TwoSides13CheckLogic");
                LoggingIF.Log(string.Format("等待_exitHandSyncEvent空闲 by _exitHandSyncEvent.WaitOne"), LogLevels.Debug, "TwoSides13CheckLogic");
                this._exitHandSyncEvent.WaitOne();
                if (this._resetSignalReceived) return false;
            }

            LoggingIF.Log(string.Format("置_exitHandSyncEvent忙碌 by _exitHandSyncEvent.Reset"), LogLevels.Debug, "TwoSides13CheckLogic");
            this._hasBatOnExitHand = true;
            this._exitHandSyncEvent.Reset();

            // We have to create a new instance in case of overwriting by the next battery
            LoggingIF.Log("Push FrontSeat to ResultSeat.", LogLevels.Debug, "TwoSides13CheckLogic");
            this._workingSeats.FrontSeat.CopyTo(ref _resultSeat);

#if SYNC_SEAT_BY_LOGIC
            // We have nothing on front seat now, the seat data is allowed to switch
            LoggingIF.Log(string.Format("置front工位为空闲 by _seatSyncEvent.Set"), LogLevels.Debug, "TwoSides13CheckLogic");
            this._hasBatOnFrontSeat = false;
            this._seatSyncEvent.Set();
#endif

            // PLC starts to move while we're calculating
            LoggingIF.Log(string.Format("置_resultSyncEvent为忙碌 by _resultSyncEvent.Reset"), LogLevels.Debug, "TwoSides13CheckLogic");
            this._resultSyncEvent.Reset();

            LoggingIF.Log(string.Format("发送尾部1拍照完成SendTailShotFinish1"), LogLevels.Debug, "TwoSides13CheckLogic");


            LoggingIF.Log("Call image algo for corner 1-3 => 3...", LogLevels.Debug, "TwoSides13CheckLogic");
            AlgoWrapper.Instance.InspectBackCorner(this._resultSeat.Corner3);
            LoggingIF.Log("Call image algo for corner 1-3 => 3 finished.", LogLevels.Debug, "TwoSides13CheckLogic");

            int algoRetPre = this._resultSeat.Corner1.InspectResults.resultDataMin.iResult;
            int algoRetBack = this._resultSeat.Corner3.InspectResults.resultDataMin.iResult;

            AlgoWrapper.Instance.GetInspectResult(ref this._resultSeat.Corner1.InspectResults,
                ref this._resultSeat.Corner3.InspectResults);
            LoggingIF.Log("Get result image for corner 1-3 finished.", LogLevels.Debug, "TwoSides13CheckLogic");

            if (algoRetPre < 0 || algoRetBack < 0)
            {
                LoggingIF.Log(string.Format("Restore 1-3 algo result to {0} {1}", algoRetPre, algoRetBack), LogLevels.Warn, "TwoSides13CheckLogic");
                // this._resultSeat.Corner1.InspectResults.resultDataMin.iResult = algoRetPre;
                // this._resultSeat.Corner3.InspectResults.resultDataMin.iResult = algoRetBack;
            }

            LoggingIF.Log(string.Format("更新结果 by OnUpdateResult"), LogLevels.Debug, "TwoSides13CheckLogic");
            this.CheckLogicExt.OnUpdateResult(this._resultSeat);
            LoggingIF.Log(string.Format("Final result for {0} is {1}-{2}: Corner1({3}) Corner3({4}) ",
                            this._resultSeat.Sn,
                            this._resultSeat.FinalResult,
                            this._resultSeat.ResultCode,
                            this._resultSeat.Corner1.InspectResults.resultDataMin.iResult,
                            this._resultSeat.Corner3.InspectResults.resultDataMin.iResult), LogLevels.Info, "TwoSides13CheckLogic");

            if (this._resultSeat.ResultCode == EResultCodes.ShotFail)
            {
                string errMsg = "Unexpect result code ShotFail on OnTailShotSignal1, possible loss of data for SN " + this._resultSeat.Sn;
                LoggingIF.Log(errMsg, LogLevels.Error, "TwoSides13CheckLogic");

            }

            // Merging result images
            LoggingIF.Log("Merging result image...", LogLevels.Debug, "TwoSides13CheckLogic");
            AlgoWrapper.Instance.GetResultImage(ref this._resultSeat);
            LoggingIF.Log("Merging result image finished.", LogLevels.Debug, "TwoSides13CheckLogic");

            LoggingIF.Log(string.Format("置_resultSyncEvent为空闲 by _resultSyncEvent.Set"), LogLevels.Debug, "TwoSides13CheckLogic");
            this._resultSyncEvent.Set();

            return true;
        }

        //不用   //recompose by fjy
        public bool OnTailShotSignal2()
        {
            LoggingIF.Log(string.Format("receive ShotSignal On TailShotSignal2"), LogLevels.Debug, "TwoSides13CheckLogic");
            return true;
        }
        //recompose by fjy
        public bool OnScanCodeSignal()
        {
            this._workingSeats.BackSeat.Sn = string.Empty;

            LoggingIF.Log("Scanner triggered on OnScanCodeSignal", LogLevels.Debug, "TwoSides13CheckLogic");
            string code = string.Empty;
            CodeReaderIF.ReadBarCode(ref code);
            LoggingIF.Log("Scan result " + code, LogLevels.Info, "TwoSides13CheckLogic");


            this._workingSeats.BackSeat.Sn = code;
            this._workingSeats.BackSeat.SnScanTime = DateTime.Now;

            this.CheckLogicExtBroker(code);

            // Reset status after broker
            this._workingSeats.BackSeat.CheckMode = this.MyCheckStatus.CheckMode;
            this._workingSeats.BackSeat.CheckExtension = this.MyCheckStatus.ActiveCheckExtension;
            this._workingSeats.BackSeat.FinalResult = false;
            this._workingSeats.BackSeat.StfResult = false;
            this._workingSeats.BackSeat.ResultCode = EResultCodes.Unknow;

            this.CheckLogicExt.OnScanComplete(this._workingSeats.BackSeat, this._checkStatus);

            this._scanSyncEvent.Set();

            if (this._workingSeats.BackSeat.Sn == string.Empty)
            {
                LoggingIF.Log("Scan failed!", LogLevels.Warn, "TwoSides13CheckLogic");
            }
            else
            {
                LoggingIF.Log(string.Format("Scan OK => {0}", this._workingSeats.BackSeat.Sn), LogLevels.Debug, "TwoSides13CheckLogic");
            }

            return true;
        }
        //recompose by fjy
        public bool OnAllowsSendPhotoSignal()
        {
            this._resultSeat.EndTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            var waitSignal = this._resultSyncEvent.WaitOne(/*this._resultSyncTimeout*/);
            if (this._resetSignalReceived) return false;
            if (!waitSignal)
            {
                string errMsg = "Timeout waitting for resultSyncEvent on OnAllowsSendPhotoSignal";
                LoggingIF.Log(errMsg, LogLevels.Error, "TwoSides13CheckLogic");
            }

            // Update display image
            ResultRelay.Instance.CheckResultHandlers(this._resultSeat);
            this.CheckLogicExt.OnCheckFinished(this._resultSeat, this._imageSaveConfig, ref this._checkStatus);

            if (ResultHelper.IsOtherNG(this._resultSeat.ResultCode))
            {
                LoggingIF.Log(string.Format("Send Other NG signal because of {0}", this._resultSeat.ResultCode), LogLevels.Warn, "CheckLogicExtSTF");
                Thread.Sleep(20);
            }

            this.CheckLogicExtBrokerEnd(this._resultSeat.Sn);

            LoggingIF.Log(this._resultSeat.ToString(), LogLevels.Debug, "TwoSides13CheckLogic");

            bool result = this._resultSeat.FinalResult;
            //this._resultSeat.Reset();


            // Saving images on thread
            DateTime enterTime = DateTime.Now;
            LoggingIF.Log("Start saving images...", LogLevels.Debug, "TwoSides13CheckLogic");
            this.CheckLogicExt.OnSaveImages(this._resultSeat, this._imageSaveConfig);           //recompose by fjy   

            if (enterTime > _lastResetTime)
            {
                LoggingIF.Log("All images are saved, release result seat", LogLevels.Debug, "TwoSides13CheckLogic");

                this._exitHandSyncEvent.Set();
                this._hasBatOnExitHand = false;
            }
            else
            {
                LoggingIF.Log(string.Format("All images are saved, Not release result seat bacause system reset happened after {0}", _lastResetTime), LogLevels.Warn, "TwoSides13CheckLogic");
            }

            return true;
        }

        public bool OnPlcSystemResetSignal()
        {
            LoggingIF.Log("Handling system reset event...", LogLevels.Warn, "TwoSides13CheckLogic");

            this._resetSignalReceived = true;
            this._lastResetTime = DateTime.Now;

            // Release all events
            this._scanSyncEvent.Set();
            this._algoSyncEvent.Set();
#if SYNC_SEAT_BY_LOGIC
            this._seatSyncEvent.Set();
#endif
            this._resultSyncEvent.Set();
            this._exitHandSyncEvent.Set();
            this._hasBatOnExitHand = false;
            Thread.Sleep(this._resetProcessTime);
#if SYNC_SEAT_BY_LOGIC
            this._hasBatOnFrontSeat = false;
#endif
            this._workingSeats.ClearAll();


            // Do not reset resultSeat since the image saving thread with resulSeat may not end
            // while exception is raised
            //this._resultSeat.Reset();

            this.CheckLogicExt.OnReset();

            this._resetSignalReceived = false;
            LoggingIF.Log("Working seat status cleared!", LogLevels.Warn, "TwoSides13CheckLogic");
            return true;
        }

        public bool OnCycleMoveSignal()
        {
#if !SYNC_SEAT_BY_LOGIC
            this.SwitchSeat();
            DeviceControllerIF.SendSeatSyncFinish();
#endif

            return true;
        }

        public bool OnErrOccurred()
        {
            this._resetSignalReceived = true;
            this._lastResetTime = DateTime.Now;

            // Release all events to avoid other timeout errors
            this._scanSyncEvent.Set();
            this._algoSyncEvent.Set();
#if SYNC_SEAT_BY_LOGIC
            this._seatSyncEvent.Set();
#endif
            this._resultSyncEvent.Set();
            this._exitHandSyncEvent.Set();

            // We should wait for a while for all events to return so do not reset this flag here
            // PLC will send a reset event after all error(s) are cleared
            //this._resetSignalReceived = false;

            return true;
        }
    }
}
