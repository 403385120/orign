using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/// <summary>
/// 设备向MES发送用户信息请求
/// {"Header":{"SessionID":"GUID","FunctionID":"A039","EQCode":"EQ00000001","SoftName":"ClientSoft1","RequestTime":"2019-05-24 15:28:34 488"},"RequestInfo":{"UserID":"XXXXXXXX","UserPassword":"1234"}}
/// </summary>
namespace ATL.MES.A039
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
        public string EQCode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string SoftName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string RequestTime { get; set; }
        public string PCName { get; internal set; }
    }
    
    public class RequestInfo
    {
        /// <summary>
        /// 
        /// </summary>
        public string UserID { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string UserPassword { get; set; }
        /// <summary>
        /// TYPE为1（或者不加这段）的话，就是和之前一样，是正常的登录，MES里头会更新这个设备的登录人员atl_resource_content里头的人员字段；
        ///如果填TYPE为2，那么就只校验，MES里头不更新设备的登录人员；（就是专门给复判，抽检做校验用的）
        /// </summary>
        public string TYPE { get; set; }

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
