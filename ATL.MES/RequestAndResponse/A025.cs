using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/// <summary>
/// 设备向MES发送设备报警数据
/// {"Header": {"SessionID": "GUID","FunctionID": "A025","PCName":"PCName","EQCode": "EQ00000001","SoftName": "ClientSoft1","RequestTime": "2019/06/11 19:55:00"},"RequestInfo": {"ResourceAlertInfo": [{"AlertCode": "001","AlertName": "Red","AlertLevel": "A"},{"AlertCode": "002","AlertName": "yellow","AlertLevel": "B"}]}}
/// </summary>
namespace ATL.MES.A025
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



    public class ResourceAlertInfoItem
    {
        /// <summary>
        /// 
        /// </summary>
        public string AlertCode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string AlertName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string AlertLevel { get; set; }

    }



    public class RequestInfo
    {
        /// <summary>
        /// 
        /// </summary>
        public List<ResourceAlertInfoItem> ResourceAlertInfo { get; set; }

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
