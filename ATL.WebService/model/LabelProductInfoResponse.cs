using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATL.WebService.model
{
    /// <summary>
    /// MTW获取物料返回内容
    /// </summary>
    public class LabelProductInfoResponse
    {
        /// <summary>
        /// 头信息
        /// </summary>
        public class Header
        {
            /// <summary>
            /// 接口代码
            /// </summary>
            public string InterfaceCode { get; set; }
            /// <summary>
            /// 接口执行结果
            /// </summary>
            public string IsSuccess { get; set; }
            /// <summary>
            /// 错误代码
            /// </summary>
            public string ErrorCode { get; set; }
            /// <summary>
            /// 详细错误信息
            /// </summary>
            public string ErrorMsg { get; set; }
            /// <summary>
            /// 请求时间
            /// </summary>
            public string RequestTime { get; set; }
            /// <summary>
            /// 返回时间
            /// </summary>
            public string ResponseTime { get; set; }

        }
        /// <summary>
        /// 输出内容
        /// </summary>
        public class ResponseInfo
        {
            /// <summary>
            /// 物料编号
            /// </summary>
            public string ProductNo { get; set; }
            /// <summary> 
            /// 批次号  
            /// </summary >
            public string LotNo { get; set; }
            /// <summary> 
            /// 序列号  
            /// </summary >
            public string SerialNo { get; set; }
            /// <summary> 
            /// 单位  
            /// </summary >
            public string UomCode { get; set; }
            /// <summary> 
            /// 数量  
            /// </summary >
            public string Qutity { get; set; }
            /// <summary> 
            /// 产出时间  
            /// </summary >
            public string OutboundTime { get; set; }
            /// <summary> 
            /// 保质期截止日期  
            /// </summary >
            public string ExpirationDate { get; set; }
            /// <summary> 
            /// 工单号  
            /// </summary >
            public string WipOrderNo { get; set; }
            /// <summary> 
            /// 工单类型  
            /// </summary >
            public string WipOrderType { get; set; }
            /// <summary> 
            /// 成品编码  
            /// </summary >
            public string FinishedProductNo { get; set; }
            /// <summary> 
            /// 容器编号(卷轴)  
            /// </summary >
            public string Container { get; set; }
            /// <summary> 
            /// 容器型号(卷轴)  
            /// </summary >
            public string ContainerSpec { get; set; }
            /// <summary> 
            /// 容器编号(托盘)  
            /// </summary >
            public string InContainer { get; set; }
            /// <summary> 
            /// 容器型号(托盘)  
            /// </summary >
            public string InContainerSpec { get; set; }
            /// <summary> 
            /// 物料方向  
            /// </summary >
            public string ProductDirection { get; set; }
            /// <summary> 
            /// 容器卷轴方向  
            /// </summary >
            public string ContainerDirection { get; set; }
                       
        }
        /// <summary>
        /// 输出
        /// </summary>
        public class Root
        {
            /// <summary>
            /// 头信息
            /// </summary>
            public Header Header { get; set; }
            /// <summary>
            /// 返回内容
            /// </summary>
            public ResponseInfo ResponseInfo { get; set; }
        }
    }
}
