using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATL.WebService.model
{
    /// <summary>
    /// MTW上料接口请求
    /// </summary>
    public class InstallProductRequest
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
            /// </summary>
            public string OperationType { get; set; }
            /// <summary>
            /// 物料贴纸标签或容器
            /// </summary>
            public string Container { get; set; }
            /// <summary>
            /// 标签或容器剩余数量
            /// </summary>
            public string SQuantity { get; set; }
            /// <summary>
            /// 投料数量
            /// </summary>
            public string Quantity { get; set; }
            /// <summary>
            /// 是否整箱投放
            /// </summary>
            public string IsWholeContainer { get; set; }
            /// <summary>
            /// 投料机构
            /// </summary>
            public string OrganCode { get; set; }
            /// <summary>
            /// 当前生产的批次号
            /// </summary>
            public string LotNo { get; set; }
            /// <summary>
            /// 当前生产的订单
            /// </summary>
            public string WipOrderNo { get; set; }
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
