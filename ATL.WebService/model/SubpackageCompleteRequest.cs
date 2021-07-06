using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATL.WebService.model
{
    /// <summary>
    /// MTW浆料分罐完成生成库存请求
    /// </summary>
    public class SubpackageCompleteRequest
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
            /// <summaryg >
            /// 设备编号
            /// </summarg y>
            public string Equipment { get; set; }
            /// <summaryg >
            /// 气动泵 
            /// </summarg y>
            public string AirDrivePump { get; set; }
            /// <summaryg >
            /// 目标中转罐 
            /// </summarg y>
            public string Container { get; set; }
            /// <summaryg >
            /// 浆料总重量 
            /// </summarg y>
            public string Productionquantity { get; set; }
            /// <summaryg >
            /// 容器重量 
            /// </summarg y>
            public string Quantity { get; set; }
            /// <summaryg >
            /// 目标中转罐液位高度 
            /// </summarg y>
            public string VesselHight { get; set; }
            /// <summaryg >
            /// 容器直径 
            /// </summarg y>
            public string Vesseldiameter { get; set; }
            /// <summaryg >
            /// 当前生产的批次号 
            /// </summarg y>
            public string LotNo { get; set; }
            /// <summaryg >
            /// 操作员 
            /// </summarg y>
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
