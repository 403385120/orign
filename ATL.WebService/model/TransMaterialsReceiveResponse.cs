using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATL.WebService.model
{
    /// <summary>
    /// 收料确认返回内容
    /// </summary>
    public class TransMaterialsReceiveResponse
    {
        /// <summary>
        /// 头信息
        /// </summary>
        public class Header
        {
            /// <summary>
            /// 接口代码
            /// </summary>
            public string InterfaceCode { get; set; }
            /// <summary>
            /// 接口执行结果
            /// </summary>
            public string IsSuccess { get; set; }
            /// <summary>
            /// 错误代码
            /// </summary>
            public string ErrorCode { get; set; }
            /// <summary>
            /// 详细错误信息
            /// </summary>
            public string ErrorMsg { get; set; }
            /// <summary>
            /// 请求时间
            /// </summary>
            public string RequestTime { get; set; }
            /// <summary>
            /// 返回时间
            /// </summary>
            public string ResponseTime { get; set; }

        }
        /// <summary>
        /// 输出内容
        /// </summary>
        public class ResponseInfo
        {
            /// <summary> 
            /// 单据号  
            /// </summary >
            public string OrderNo { get; set; }
            /// <summary> 
            /// 接口执行结果  
            /// </summary >
            public string IsSuccess { get; set; }
            /// <summary> 
            /// 错误代码  
            /// </summary >
            public string ErrorCode { get; set; }
            /// <summary> 
            /// 详细错误信息  
            /// </summary >
            public string ErrorMsg { get; set; }

        }
        /// <summary>
        /// 输出
        /// </summary>
        public class Root
        {
            /// <summary>
            /// 头信息
            /// </summary>
            public Header Header { get; set; }
            /// <summary>
            /// 返回内容
            /// </summary>
            public ResponseInfo ResponseInfo { get; set; }
        }
    }
}
