using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATL.WebService.model
{
    /// <summary>
    /// MTW生产批次状态切换接口请求
    /// </summary>
    public class LotNoStatusChangeRequest
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
            /// 设备编号  
            /// </summary >
            public string Equipment { get; set; }
            /// <summary> 
            /// 生产批次号  
            /// </summary >
            public string LotNo { get; set; }
            /// <summary> 
            /// 新状态  
            /// </summary >
            public string LotNoStatus { get; set; }
            /// <summary> 
            /// 分条数量  
            /// </summary >
            public string SplitQuantity { get; set; }
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
