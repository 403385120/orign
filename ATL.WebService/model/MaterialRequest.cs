using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATL.WebService.model
{
    /// <summary>
    /// MTW设备叫料和换料请求
    /// </summary>
    public class MaterialRequest
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
            /// 生产批次号  
            /// </summary >
            public string LotNo { get; set; }
            /// <summary> 
            /// 请求类型  
            /// </summary >
            public string RequestType { get; set; }
            /// <summary> 
            /// 设备编号  
            /// </summary >
            public string Equipment { get; set; }
            /// <summary> 
            /// 换料机构序号  
            /// </summary >
            public string OrganSequence { get; set; }
            /// <summary> 
            /// 换料机构编号  
            /// </summary >
            public string OrganCode { get; set; }
            /// <summary> 
            /// 工序号  
            /// </summary >
            public string OperationCode { get; set; }

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
