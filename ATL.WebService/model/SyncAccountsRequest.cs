using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATL.WebService.model
{
    /// <summary>
    /// MTW交易转移单请求
    /// </summary>
    public class SyncAccountsRequest
    {								


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
            /// <summary>
            /// 单据行
            /// </summary>
            public OrderDetail OrderDetail { get; set; }
        }
        /// <summary>
        /// 请求内容
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
            /// <summary>
            /// 物料明细
            /// </summary>
            public OrderDetailContent OrderDetailContent { get; set; }
        }
        /// <summary>
        /// 请求内容
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
        /// 输入
        /// </summary>
        public class Root
        {
            /// <summary>
            /// 单据头
            /// </summary>
            public OrderHeader OrderHeader { get; set; }
            
            
        }
    }
}
