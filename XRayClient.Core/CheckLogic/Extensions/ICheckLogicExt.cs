using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace XRayClient.Core
{
    /// <summary>
    /// 检测逻辑扩展类
    /// </summary>
    public interface ICheckLogicExt
    {
        /// <summary>
        /// 扫码完成之后需要：
        /// 1. 电芯校验
        /// 2. 如果扫码异常则调用
        /// 3. 上料数据上传 -> 获取图片保存目录
        /// 注意：
        /// 扫描完成之后的可操作Seat为BackSeat
        /// </summary>
        void OnScanComplete(BatterySeat seat, CheckStatus checkStatus);

        /// <summary>
        /// 更新结果之后需要：
        /// 核心模块更新结果只考虑基本检测逻辑
        /// 即使核心模块判断OK我们仍然可以在这里将最终结果置为NG
        /// </summary>
        void OnUpdateResult(BatterySeat seat);

        /// <summary>
        /// 检测完成
        /// 1. X-Ray数据上传
        /// 2. 下料数据上传
        /// 3. 数据库更新
        /// 注意：
        /// 检测完成之后的可操作Seat为ResultSeat
        /// </summary>
        void OnCheckFinished(BatterySeat seat, ImageSaveConfig imgSaveConfig, ref CheckStatus status);

        /// <summary>
        /// 保存图片
        /// </summary>
        /// <param name="seat"></param>
        /// <param name="imgSaveConfig"></param>
        void OnSaveImages(BatterySeat seat, ImageSaveConfig imgSaveConfig);

        /// <summary>
        /// PLC重置
        /// 清除标志
        /// </summary>
        void OnReset();
    }
}
