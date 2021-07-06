using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATL.MES.A090
{
    public class Header
    {
        /// <summary>
        /// 
        /// </summary>
        public string FunctionID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ResponseTime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string IsSuccess { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ErrorCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ErrorMsg { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string SessionID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string EQCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string RequestTime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string SoftName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string PCName { get; set; }
    }

    public class SERIALNOSItem
    {
        /// <summary>
        /// 电芯号
        /// </summary>
        public string SERIALNO { get; set; }
        /// <summary>
        /// 对应的组别
        /// </summary>
        public string GROUP { get; set; }
        /// <summary>
        /// 异常信息
        /// </summary>
        public string MSG { get; set; }
    }

    public class ResponseInfo
    {
        /// <summary>
        /// 返回的电芯列表
        /// </summary>
        public List<SERIALNOSItem> SERIALNOS { get; set; }
    }

    public class Root
    {
        /// <summary>
        /// 
        /// </summary>
        public Header Header { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ResponseInfo ResponseInfo { get; set; }
    }


}
