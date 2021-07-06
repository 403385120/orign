using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/// <summary>
/// MES收到返回用户信息结果
/// {"Header":{"SessionID":"GUID","FunctionID":"A040","PCName":"PCName","EQCode":"EQ00000001","SoftName":"ServerSoft","IsSuccess":true,"ErrorCode":"00/01/02","ErrorMsg":"XXXX","RequestTime":"2019-05-24 15:28:34 509","ResponseTime":"2019-05-24 15:28:34 509"},"ResponseInfo":{"UserID":"000777665","UserName":"xxxx","UserLevel":"Administrator"}}
/// </summary>
namespace ATL.MES.A040
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
        public string RequestTime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string ResponseTime { get; set; }

    }



    public class ResponseInfo
    {
        /// <summary>
        /// 
        /// </summary>
        public string UserID { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string UserLevel { get; set; }

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
