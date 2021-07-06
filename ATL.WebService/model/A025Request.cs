using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATL.WebService.model
{
    /// <summary>
    /// MTW设备报警信息A025请求
    /// </summary>
    public class A025Request
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
            /// 当前生产的批次号  
            /// </summary >
            public string Lot_No { get; set; }
            /// <summary> 
            /// 步次  
            /// </summary >
            public string Step { get; set; }
            /// <summary> 
            /// 公转  
            /// </summary >
            public string Revolution { get; set; }
            /// <summary> 
            /// 自转  
            /// </summary >
            public string Retation { get; set; }
            /// <summary> 
            /// 温度  
            /// </summary >
            public string Temperature { get; set; }
            /// <summary> 
            /// 真空度  
            /// </summary >
            public string Vacuum { get; set; }
            /// <summary> 
            /// 发送时间  
            /// </summary >
            public string Sendtime { get; set; }
            /// <summary> 
            /// 警报类型  
            /// </summary >
            public string Alarm { get; set; }
            
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
