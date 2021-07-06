using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATL.WebService.model
{
    /// <summary>
    /// MTW每一步结束信号A027请求
    /// </summary>
    public class A027NoRequest
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
            public string Device_NO { get; set; }
            /// <summary> 
            /// 生次批次  
            /// </summary >
            public string Lot_No { get; set; }
            /// <summary> 
            /// 步次  
            /// </summary >
            public string Step { get; set; }
            /// <summary> 
            /// 状态  
            /// </summary >
            public string Result { get; set; }
            /// <summary> 
            /// 发送时间  
            /// </summary >
            public string Sendtime { get; set; }
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
