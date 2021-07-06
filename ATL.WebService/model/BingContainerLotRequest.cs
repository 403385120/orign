using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATL.WebService.model
{
    /// <summary>
    /// MTW批次绑定/解绑请求
    /// </summary>
    public class BingContainerLotRequest
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
            /// 贴纸标签
            /// </summary>
            public string Container { get; set; }
            /// <summary>
            /// 父标签
            /// </summary>
            public string InContainer { get; set; }
            /// <summary>
            /// 批次号
            /// </summary>
            public string LotNo { get; set; }
            /// <summary>
            /// 类型
            /// </summary>
            public string BingType { get; set; }
            /// <summary>
            /// 工序段 非必需
            /// </summary>
            public string OperationType { get; set; }
            /// <summary>
            /// 容器卷轴方向
            /// </summary>
            public string ContainerDirection { get; set; }
            /// <summary>
            /// 操作员
            /// </summary>
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
