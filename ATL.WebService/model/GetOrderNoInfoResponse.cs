using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATL.WebService.model
{
    /// <summary>
    /// 查询领料单信息返回内容
    /// </summary>
    public class GetOrderNoInfoResponse
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
        public class OrderHeader
        {
            /// <summary>
            /// 单据号
            /// </summary>
            public string OrderNo { get; set; }
            /// <summary> 
            /// 单据类型  
            /// </summary >
            public string OrderType { get; set; }
            /// <summary> 
            /// 单据状态  
            /// </summary >
            public string ProgressStatus { get; set; }
            /// <summary> 
            /// 备注  
            /// </summary >
            public string Comment { get; set; }

        }
        /// <summary>
        /// 单据行
        /// </summary>
        public class OrderDetail
        {
            /// <summary> 
            /// 单据号  
            /// </summary >
            public string OrderNo { get; set; }
            /// <summary>  
            /// 单据类型      
            /// </summary  >
            public string OrderType { get; set; }
            /// <summary>  
            /// 单据行号      
            /// </summary  >
            public string OrderLineNo { get; set; }
            /// <summary>  
            /// 物料编码      
            /// </summary  >
            public string ProductNo { get; set; }
            /// <summary>  
            /// 单位      
            /// </summary  >
            public string OrderedUomCode { get; set; }
            /// <summary>  
            /// 数量      
            /// </summary  >
            public string QuantityOrdered { get; set; }
            /// <summary>  
            /// 来源仓库      
            /// </summary  >
            public string FromWarehouse { get; set; }
            /// <summary>  
            /// 目标仓库      
            /// </summary  >
            public string ToWarehouse { get; set; }
            /// <summary>  
            /// 来源库位      
            /// </summary  >
            public string FromWarehouseLocation { get; set; }
            /// <summary>  
            /// 目标库位      
            /// </summary  >
            public string ToWarehouseLocation { get; set; }
            /// <summary>  
            /// 参考标记      
            /// </summary  >
            public string Tag { get; set; }
            /// <summary>  
            /// 操作员      
            /// </summary  >
            public string EmployeeNo { get; set; }
        }
        /// <summary>
        /// 物料明细
        /// </summary>
        public class OrderDetailContent
        {
            /// <summary>  
            /// 单据号      
            /// </summary  >
            public string OrderNo { get; set; }
            /// <summary>   
            /// 单据类型       
            /// </summary   >
            public string OrderType { get; set; }
            /// <summary>   
            /// 单据行号       
            /// </summary   >
            public string OrderLineNo { get; set; }
            /// <summary>   
            /// 数量       
            /// </summary   >
            public string Quantity { get; set; }
            /// <summary>   
            /// 条码标签       
            /// </summary   >
            public string ContainerNumber { get; set; }
            /// <summary>   
            /// 批次号       
            /// </summary   >
            public string LotNumber { get; set; }


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
            /// 单据头
            /// </summary>
            public OrderHeader OrderHeader { get; set; }

            /// <summary>
            /// 输出内容
            /// </summary>
            public OrderDetail OrderDetail { get; set; }

            /// <summary>
            /// 物料明细
            /// </summary>
            public OrderDetailContent OrderDetailContent { get; set; }
        }
    }
}
