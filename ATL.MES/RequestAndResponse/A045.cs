using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/// <summary>
/// 设备运行标准参数人工更改后需要向MES递交参数
/// {"Header":{"SessionID":"Guid","FunctionID":"A013","PCName":"PCName","EQCode":"EQ00000001","SoftName":"ClientSoft1","RequestTime":"2019/06/11 20:41:34"},"RequestInfo":{"UserInfo":{"UserID":"123","UserName":"ATL","UserLevel":"1"},"EquParam":[{"ParamID":"001","ParamDesc":"压力","OldParamValue":"125.78","NewParamValue":"100"},{"ParamID":"002","ParamDesc":"温度","OldParamValue":"20","NewParamValue":"32"}]}}
/// </summary>
namespace ATL.MES.A045
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

    public class EquParamItem
    {
        /// <summary>
        /// 
        /// </summary>
        public string ParamID { get; set; }

        /// <summary>
        /// 压力
        /// </summary>
        public string ParamDesc { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string OldParamValue { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string NewParamValue { get; set; }

    }



    public class RequestInfo
    {
        /// <summary>
        /// 
        /// </summary>
        public UserInfo UserInfo { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<EquParamItem> EquParam { get; set; }

    }

    public class UserInfo
    {
        public string UserID;
        public string UserLevel;
        public string UserName;
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
