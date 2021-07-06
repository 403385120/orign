using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATL.WebService.model
{
    /// <summary>
    /// MTW产出报工请求
    /// </summary>
    public class ReportProductionRequest
    {
        public class Header
        {
            /// <summary>
            /// 接口代码
            /// </summary>
            public string InterfaceCode { get; set; }
            /// <summary>
            /// 请求时间
            /// </summary>
            public string RequestTime { get; set; }
        }
        /// <summary>
        /// 请求内容
        /// </summary>
        public class RequestInfo
        {
            /// <summary>
            /// 设备编号
            /// </summary>
            public string Equipment { get; set; }
            /// <summary> 
            /// 工序段  
            /// </summary >
            public string OperationType { get; set; }
            /// <summary> 
            /// 完工数量  
            /// </summary >
            public string CompletionQuantity { get; set; }
            /// <summary> 
            /// 报废数量  
            /// </summary >
            public string ScrapQuantity { get; set; }
            /// <summary> 
            /// 当前生产的批次号  
            /// </summary >
            public string LotNo { get; set; }
            /// <summary> 
            /// 当前生产的订单  非必需
            /// </summary >
            public string WipOrderNo { get; set; }
            /// <summary>
            /// 物料方向 非必需
            /// </summary>
            public string ProductDirection { get; set; }
            
            /// <summary> 
            /// 操作员  
            /// </summary >
            public string EmployeeNo { get; set; }


        }
        /// <summary>
        /// 输入
        /// </summary>
        public class Root
        {
            /// <summary>
            /// 请求头
            /// </summary>
            public Header Header { get; set; }
            /// <summary>
            /// 请求内容
            /// </summary>
            public RequestInfo RequestInfo { get; set; }
        }
    }
}
