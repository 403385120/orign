using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATL.WebService.model
{
    /// <summary>
    /// 创建箱号接口请求
    /// </summary>
    public class EstablishContainerRequest
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
            /// 打包位置
            /// </summary>
            public string PACKLOCATION { get; set; }
            /// <summary> 
            /// 物料编码  
            /// </summary >
            public string PRODUCTNO { get; set; }
            /// <summary> 
            /// 周次  
            /// </summary >
            public string WEEKLYTIME { get; set; }
            /// <summary> 
            /// 组别  
            /// </summary >
            public string HL { get; set; }
            /// <summary> 
            /// 标注  
            /// </summary >
            public string LABELCODE { get; set; }
            /// <summary> 
            /// MI号  
            /// </summary >
            public string MINO { get; set; }
            /// <summary> 
            /// 整箱电芯数  
            /// </summary >
            public string NUM { get; set; }
            /// <summary> 
            /// 物料类型：M-正常； S-Sorting  
            /// </summary >
            public string PACKTYPE { get; set; }
            /// <summary> 
            /// 年  
            /// </summary >
            public string YEARS { get; set; }
            /// <summary> 
            /// 天  
            /// </summary >
            public string DAYS { get; set; }
            /// <summary> 
            /// GroupTimes：1，2，3，4		  
            /// </summary >
            public string GROUP_RULE_NO { get; set; }
            /// <summary> 
            /// 是否校验实时规格：0-是；1-否
            /// </summary >
            public string ISACTUAL { get; set; }
            /// <summary> 
            /// 是否不校验电芯二次码：0-是；1-否
            /// </summary >
            public string ISDONTSECONDBARCODE { get; set; }
            /// <summary> 
            /// 是否不校验组别信息：0-是；1-否  
            /// </summary >
            public string ISDONTCHECKGROUP { get; set; }
            /// <summary> 
            ///托盘数量
            /// </summary >
            public string TRAYNUM { get; set; }
            /// <summary> 
            /// 周次管控-起始周次
            /// </summary >
            public string STARWEEK { get; set; }
            /// <summary> 
            /// 周次管控-结束周次
            /// </summary >
            public string ENDWEEK { get; set; }
            /// <summary> 
            /// 是否忽略坏品：1-是；0-否  
            /// </summary >
            public string IGNOREBAD { get; set; }
            
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
