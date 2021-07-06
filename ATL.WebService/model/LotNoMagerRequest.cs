using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATL.WebService.model
{
    /// <summary>
    /// MTW分条极片批次合并请求
    /// </summary>
    public class LotNoMagerRequest
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
            /// 工单号
            /// </summary>
            public string WipOrderNo { get; set; }
            /// <summary> 
            /// 工序段  
            /// </summary >
            public string OperationType { get; set; }
            /// <summary> 
            /// 目标生产批次号  
            /// </summary >
            public string LotNo { get; set; }
            /// <summary> 
            /// 来源生产批次号  
            /// </summary >
            public string NewLotNo { get; set; }
            /// <summary> 
            /// 合并数量  
            /// </summary >
            public string Quantity { get; set; }
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
