using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATL.WebService.model
{
    /// <summary>
    /// MTW上传在线粘度检测请求
    /// </summary>
    public class UploadviscosityRequest
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
            /// 测试烧杯容器  
            /// </summary >
            public string Container { get; set; }
            /// <summary> 
            /// 物料编码  
            /// </summary >
            public string ProductNo { get; set; }
            /// <summary> 
            /// 品质参数代码  
            /// </summary >
            public string defectreasoncode { get; set; }
            /// <summary> 
            /// 品质参数值  
            /// </summary >
            public string Coordinate { get; set; }
            /// <summary> 
            /// 当前生产的批次号  
            /// </summary >
            public string LotNo { get; set; }
            /// <summary> 
            /// 当前生产的订单  
            /// </summary >
            public string WipOrderNo { get; set; }
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
