using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATL.WebService.model
{
    /// <summary>
    /// MTW过程数据A000请求
    /// </summary>
    public class GetOrderNoInfoRequest
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
        public class OrderHeader
        {
            /// <summary>
            /// 单据号
            /// </summary>
            public string OrderNo { get; set; }
            /// <summary>
            /// 物料标签
            /// </summary>
            public string Container { get; set; }
            /// <summary>
            /// 备注
            /// </summary>
            public string Comment { get; set; }

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
            public OrderHeader OrderHeader { get; set; }
        }
    }
}
