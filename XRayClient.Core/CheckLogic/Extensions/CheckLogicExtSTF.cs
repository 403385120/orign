using System;
using XRayClient.Core.Converters;
using System.Threading;
using ZY.Logging;
using XRayClient.BatteryCheckManager;

namespace XRayClient.Core 
{
    /// <summary>
    /// STF扩展类
    /// </summary>
    public class CheckLogicExtSTF : ICheckLogicExt
    {
        public void OnScanComplete(BatterySeat seat, CheckStatus checkStatus)
        {
            if (seat.Sn != string.Empty)
            {
                seat.StfResult = true;
            }
            else
            {
            }
        }

        public void OnUpdateResult(BatterySeat seat)
        {
            if (seat.ResultCode == EResultCodes.FoundOK
                || seat.ResultCode == EResultCodes.FoundNG
                || ResultHelper.IsOtherNG(seat.ResultCode))
            {
                LoggingIF.Log( string.Format("OnUpdateResult ignored with result code {0}, keep result as {1}。", seat.ResultCode, seat.FinalResult), LogLevels.Warn, "CheckLogicExtSTF");
                return;
            }

            seat.UpdateResult();
            bool oldResult = seat.FinalResult; // Do not modify this

            if (seat.FinalResult)
            {
                if (!seat.StfResult)
                {
                    seat.FinalResult = false;
                    //seat.ResultCode = EResultCodes.Unknow;
                }
            }

            if (seat.FinalResult != oldResult)
            {
                LoggingIF.Log("Update Final results to " + seat.FinalResult.ToString() + " from " + oldResult.ToString(), LogLevels.Warn, "CheckLogicExtSTF");
            }
        }

        ////STF正常跑料
        public void OnCheckFinished(BatterySeat seat, ImageSaveConfig imgSaveConfig, ref CheckStatus status)
        {
            bool noLogData = false;

            if (seat.ResultCode == EResultCodes.FoundOK
                || seat.ResultCode == EResultCodes.FoundNG
                //|| ResultHelper.IsOtherNG(seat.ResultCode)
                )
            {
                LoggingIF.Log("Apply old result from DB for the battery: "
                      + "BatteryBarCode: " + seat.Sn + Environment.NewLine
                      + "FinalResult: " + seat.FinalResult.ToString() + Environment.NewLine
                      + "ResultCode: " + seat.ResultCode.ToString() + Environment.NewLine, LogLevels.Warn, "FourSidesCheckLogicExtSTF");

                noLogData = true;
            }

            if (seat.FinalResult == true)
            {
                //status.MyStatistics.OKNum++;
            }
            else
            {
                
                if (seat.Sn == string.Empty|| seat.Sn == "ERROR"
                    || seat.ResultCode== EResultCodes.Unknow|| seat.ResultCode == EResultCodes.AlgoErr)//黑白图和位置错误和未知错误的图当成扫码NG，可以重新投料
                {
                    //status.MyStatistics.ScanNG++;   //扫码
                    LoggingIF.Log(string.Format("数据统计：条码{0}，扫码NG+1,ResultCode为{1}", seat.Sn,seat.ResultCode), LogLevels.Info, "CheckLogicExtSTF");
                }
                else if (seat.ThicknessResult==false)
                {
                    //status.MyStatistics.OtherNG++;//测厚
                    LoggingIF.Log(string.Format("数据统计：条码{0}，测厚NG+1,ResultCode为{1}", seat.Sn, seat.ResultCode), LogLevels.Info, "CheckLogicExtSTF");
                }
                else
                {
                    //status.MyStatistics.NGNum++;//xray
                }
            }

            // 2. 更新统计数据
            //status.MyStatistics.TotalNum++;
            status.MyStatistics.Save();

            if (noLogData) return;

            //throw new NotImplementedException();
            this.handleResult(seat, imgSaveConfig, ref status);
        }

        /// <summary>
        /// 结果本地处理
        /// 警告：单线程函数，耗时任务请使用线程处理
        /// </summary>
        private void handleResult(BatterySeat batSeat, ImageSaveConfig imgSaveConfig, ref CheckStatus status)
        {
            // Note:
            // 结果图片刷新将在主窗口事件Handler
            // 此处无需处理

            // TODO:
            // 1. 保存图片
            // 2. 更新统计数据
            // 3. 上传数据
            // 4. 保存数据库

            // 1. 更新图片名称(用于存STF和数据库)
            // 接口会校验图片是否存在，在调用接口之前先上传图片
            LoggingIF.Log("Updating image file names...", LogLevels.Debug, "FourSidesCheckLogicExtSTF");
            batSeat.UpdateFileName(imgSaveConfig, imgSaveConfig.UseDynamicTestPath);

           
            if (batSeat.Sn == string.Empty)
            {
                LoggingIF.Log("STF not called since there is no SN...", LogLevels.Warn, "FourSidesCheckLogicExtSTF");
            }
            else
            {
            }
            
        }

        public void OnSaveImages(BatterySeat seat, ImageSaveConfig imgSaveConfig)
        {
            if (seat.ResultCode == EResultCodes.FoundOK
                    || seat.ResultCode == EResultCodes.FoundNG
                    || ResultHelper.IsOtherNG(seat.ResultCode)
                    || seat.ResultCode==EResultCodes.Unknow
                    || seat.Sn == "ERROR"
                    || seat.Sn == string.Empty)
            {
                LoggingIF.Log("Image not saved. " + Environment.NewLine
                      + "BatteryBarCode: " + seat.Sn + Environment.NewLine
                      + "A015 Result: " + seat.StfResult.ToString() + Environment.NewLine
                      + "FinalResult: " + seat.FinalResult.ToString() + Environment.NewLine
                      + "ResultCode: " + seat.ResultCode.ToString() + Environment.NewLine, LogLevels.Warn, "FourSidesCheckLogicExtSTF");

                return;
            }

            // 3. 更新数据库
            // 此处没有使用多线程，因为如果数据库失败整机也必须停止防止丢失数据

            try
            {
                seat.SaveAll(imgSaveConfig, imgSaveConfig.UseDynamicTestPath);
            }
            catch (Exception ex)
            {
                LoggingIF.Log("OnSaveImages Exception:  " + Environment.NewLine
                      + ex.Message + Environment.NewLine, LogLevels.Warn, "CheckLogicExtSTF");
            }

            if(seat.CheckExtension != ECheckExtensions.Local)
            {
                BatteryCheck dbData = ConertersUtils.BatterySeatToBatteryCheck(seat);
                try
                {
                    LoggingIF.Log("Saving data to DB " + seat.Sn, LogLevels.Debug, "FourSidesCheckLogicExtSTF");
                    BatteryCheckIF.AddBatteryCheck(dbData);
                    LoggingIF.Log("Saving data to DB finished.", LogLevels.Debug, "FourSidesCheckLogicExtSTF");
                }
                catch (System.Exception ex)
                {
                    LoggingIF.Log("DB error " + ex.Message.ToString(), LogLevels.Debug, "FourSidesCheckLogicExtSTF");
                }
            }
        }

        public void OnReset()
        {
           
        }
    }
}
