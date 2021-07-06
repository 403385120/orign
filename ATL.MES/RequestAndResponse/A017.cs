using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/// <summary>
/// MES发送采集设备运行参数指令，用于点检
/// {"Header":{"SessionID":"GUID","FunctionID":"A017","PCName":"PCName","EQCode":"EQ00000001","SoftName":"ServerSoft","RequestTime":"2019-05-24 15:28:34 488"},"RequestInfo":{"ParamInfo":[{"ParamID":"001","ParamDesc":"压力"},{"ParamID":"002","ParamDesc":"转速"},{"ParamID":"003","ParamDesc":"温度"}]}}
/// </summary>
namespace ATL.MES.A017
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



    public class ParamInfoItem
    {
        /// <summary>
        /// 
        /// </summary>
        public string ParamID { get; set; }

        /// <summary>
        /// 压力
        /// </summary>
        public string ParamDesc { get; set; }

    }



    public class RequestInfo
    {
        /// <summary>
        /// 
        /// </summary>
        public List<ParamInfoItem> ParamInfo { get; set; }

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
