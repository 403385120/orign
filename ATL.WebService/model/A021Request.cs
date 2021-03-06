using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATL.WebService.model
{
    /// <summary>
    /// MTW关键零配件寿命A021请求
    /// </summary>
    public class A021Request
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
            /// 发送时间
            /// </summary>
            public string Sendtime { get; set; }
            /// <summary>
            /// 关键零部件信息
            /// </summary>
            public List<Keyparts> Keyparts { get; set; }                     

        }
        /// <summary>
        /// 关键零部件信息
        /// </summary>
        public class Keyparts
        {
            /// <summary>
            /// 
            /// </summary>
            public string PartID { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string lifetime { get; set; }

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
