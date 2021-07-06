using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/// <summary>
/// 设备向MES发送工产品信息请求
/// {"Header":{"SessionID":"GUID","FunctionID":"A037","EQCode":"EQ00000001","SoftName":"ClientSoft1","RequestTime":"2019-05-24 15:28:34 488"},"RequestInfo":{"ProductID":"XXXXXXXX"}}
/// </summary>
namespace ATL.MES.A037
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
        public string OperationCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ReworkFlag { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<SerialNoInfoItem> SerialNoInfo { get; set; }

    }

    public class SerialNoInfoItem
    {
        /// <summary>
        /// 
        /// </summary>
        public string SerialNo { get; set; }
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
