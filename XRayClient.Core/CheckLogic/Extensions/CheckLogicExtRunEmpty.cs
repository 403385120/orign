using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XRayClient.Core.Converters;

namespace XRayClient.Core
{
    /// <summary>
    /// 本地跑扩展
    /// </summary>
    public class CheckLogicExtRunEmpty : ICheckLogicExt
    {
        public void OnScanComplete(BatterySeat seat, CheckStatus checkStatus)
        {
            return;
        }

        public void OnUpdateResult(BatterySeat seat)
        {
            seat.UpdateResult();

            return;
        }

        /// <summary>
        /// 结果本地处理--空跑
        /// 警告：单线程函数，耗时任务请使用线程处理
        /// </summary>
        public void OnCheckFinished(BatterySeat batSeat, ImageSaveConfig imgSaveConfig, ref CheckStatus status)
        {
            // Note:
            // 结果图片刷新将在主窗口事件Handler
            // 此处无需处理

            // TODO:
            // 1. 保存图片
            // 2. 更新统计数据
            // 3. 保存数据库
            batSeat.UpdateFileName(imgSaveConfig, imgSaveConfig.UseDynamicTestPath,false,false);

            //add by fjy  
            if (batSeat.Sn == string.Empty || batSeat.Sn == "ERROR")
            {
                //status.MyStatistics.ScanNG++;
            }
            else if (batSeat.ResultCode == EResultCodes.Unknow || batSeat.ResultCode == EResultCodes.AlgoErr)//黑白图和位置错误和未知错误的图当成扫码NG，可以重新投料
            {
                //status.MyStatistics.OtherNG++;
            }
            else if (batSeat.ThicknessResult == false)
            {
                //status.MyStatistics.ThincknessNG++;
            }
            else if (batSeat.DimensionResult == false)
            {
                //status.MyStatistics.DimensionNG++;
            }
            if (batSeat.FinalResult == false)
            {
                status.MyStatistics.NGNum++;
                int[] NGtype = { -5,-7,-8};
                for (int i = 0; i < NGtype.Length; i++)
                {
                    if (batSeat.Corner1.InspectResults.resultDataMin.iResult == NGtype[i])
                    {
                        if (NGtype[i] == -5)
                            status.MyStatistics.WrinkleNG++;
                        if (NGtype[i] == -7)
                            status.MyStatistics.DistanceNG++;
                        if (NGtype[i] == -8)
                            status.MyStatistics.AngleNG++;
                    }
                    if (batSeat.Corner2.InspectResults.resultDataMin.iResult == NGtype[i])
                    {
                        if (NGtype[i] == -5)
                            status.MyStatistics.WrinkleNG++;
                        if (NGtype[i] == -7)
                            status.MyStatistics.DistanceNG++;
                        if (NGtype[i] == -8)
                            status.MyStatistics.AngleNG++;
                    }
                    if (batSeat.Corner3.InspectResults.resultDataMin.iResult == NGtype[i])
                    {
                        if (NGtype[i] == -5)
                            status.MyStatistics.WrinkleNG++;
                        if (NGtype[i] == -7)
                            status.MyStatistics.DistanceNG++;
                        if (NGtype[i] == -8)
                            status.MyStatistics.AngleNG++;
                    }
                    if (batSeat.Corner4.InspectResults.resultDataMin.iResult == NGtype[i])
                    {
                        if (NGtype[i] == -5)
                            status.MyStatistics.WrinkleNG++;
                        if (NGtype[i] == -7)
                            status.MyStatistics.DistanceNG++;
                        if (NGtype[i] == -8)
                            status.MyStatistics.AngleNG++;
                    }
                }
            }
            else if (batSeat.MesResult == false)
            {
                //status.MyStatistics.MesNG++;
            }
            else
            {
                //status.MyStatistics.OKNum++;
            }
            
            // 2. 更新统计数据
            //status.MyStatistics.TotalNum++;
            status.MyStatistics.Save();  
        }

        public void OnSaveImages(BatterySeat seat, ImageSaveConfig imgSaveConfig)
        {
            // 此处没有使用多线程，因为如果数据库失败整机也必须停止防止丢失数据

            try
            {
                seat.SaveAll(imgSaveConfig, false);
            }
            catch (System.Exception ex)
            {
            }
        }

        public void OnReset()
        {
            return;
        }
    }
}
