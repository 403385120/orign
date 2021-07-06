using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATL.WebService.model
{
    /// <summary>
    /// 浆料分罐登记请求
    /// </summary>
    public class MixingSubpackageRequest
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
            /// 工序段
            /// </summary>
            public string OperationType { get; set; }
            /// <summary> 
            /// 设备编号  
            /// </summary >
            public string Equipment { get; set; }
            /// <summary> 
            /// 气动泵  
            /// </summary >
            public string AirDrivePump { get; set; }
            /// <summary> 
            /// 目标中转罐  
            /// </summary >
            public string Container { get; set; }
            /// <summary> 
            /// 浆料总重量  
            /// </summary >
            public string Productionquantity { get; set; }
            /// <summary> 
            /// 容器重量  
            /// </summary >
            public string Quantity { get; set; }
            /// <summary> 
            /// 目标中转罐液位高度  
            /// </summary >
            public string VesselHight { get; set; }
            /// <summary> 
            /// 容器直径  
            /// </summary >
            public string Vesseldiameter { get; set; }
            /// <summary> 
            /// 当前生产的批次号  
            /// </summary >
            public string LotNo { get; set; }
            /// <summary> 
            /// 操作员  
            /// </summary >
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
