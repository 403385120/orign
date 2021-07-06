using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/// <summary>
/// MES发送设备控制指令
/// {"Header":{"SessionID":"Guid","FunctionID":"A011","PCName":"PCName","EQCode":"EQ00000001","SoftName":"ServerSoft","RequestTime":"2019-05-24 15:28:34 488"},"RequestInfo":{"ControlCode":"Run","StateCode":"","StateDesc":""}
/// </summary>
namespace ATL.MES.A011
{
    public class Header
    {
        /// <summary>
        /// 
        /// </summary>
        public string SessionID { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string FunctionID { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string PCName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string EQCode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string SoftName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string RequestTime { get; set; }

    }



    public class RequestInfo
    {
        /// <summary>
        /// 
        /// </summary>
        public string ControlCode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string StateCode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string StateDesc { get; set; }

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
        public RequestInfo RequestInfo { get; set; }

    }
    
}
