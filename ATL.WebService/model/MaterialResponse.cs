using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATL.WebService.model
{
    /// <summary>
    /// MTW设备叫料和换料请求返回内容
    /// </summary>
    public class MaterialResponse
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
            /// 工单号
            /// </summary>
            public string WipOrderNo { get; set; }
            /// <summary> 
            /// 生产批次号  
            /// </summary >
            public string LotNo { get; set; }
            /// <summary> 
            /// 生产数量  
            /// </summary >
            public string ProQuantity { get; set; }
            /// <summary> 
            /// 物料编码  
            /// </summary >
            public string ProductNo { get; set; }
            /// <summary> 
            /// 单位  
            /// </summary >
            public string OrderedUomCode { get; set; }
            /// <summary> 
            /// 仓库  
            /// </summary >
            public string Warehouse { get; set; }
            /// <summary> 
            /// 库位  
            /// </summary >
            public string Warehouselocation { get; set; }
            /// <summary> 
            /// 物料总需求数量  
            /// </summary >
            public string Quantity { get; set; }
            /// <summary> 
            /// 参考标记  
            /// </summary >
            public string Tag { get; set; }
            /// <summary> 
            /// 方向  
            /// </summary >
            public string direction { get; set; }
            /// <summary> 
            /// 建单时间  
            /// </summary >
            public string Createon { get; set; }
            /// <summary> 
            /// 换料机构序号  
            /// </summary >
            public string OrganSequence { get; set; }
            /// <summary> 
            /// 换料机构编号  
            /// </summary >
            public string OrganCode { get; set; }
            /// <summary> 
            /// BOM用量  
            /// </summary >
            public string BOMQTY { get; set; }
            /// <summary> 
            /// 用量系列  
            /// </summary >
            public string BOMSERIAL { get; set; }
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
            public ResponseInfo[] ResponseInfo { get; set; }
        }
    }
}
