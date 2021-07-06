using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/// <summary>
/// 设备向MES发送程序更改履历数据
/// {"Header":{"SessionID":"GUID","FunctionID":"A023","PCName":"PCName","EQCode":"EQ00000001","SoftName":"ClientSoft1","RequestTime":"2019/06/10 19:38:46"},"RequestInfo":{"ResourceRecordInfo":[{"ResourceID":"EQ00000001","ProgramName":"上位机","ProgramVension":"Ver 19.06.02.190602"},{"ResourceID":"EQ00000001","ProgramName":"PLC","ProgramVension":"Ver 19.06.02.190602"},{"ResourceID":"EQ00000001","ProgramName":"HMI","ProgramVension":"Ver 19.06.02.190602"}]}}
/// </summary>
namespace ATL.MES.A023
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



    public class ResourceRecordInfoItem
    {
        /// <summary>
        /// 
        /// </summary>
        public string ResourceID { get; set; }

        /// <summary>
        /// 上位机
        /// </summary>
        public string ProgramName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string ProgramVension { get; set; }

    }



    public class RequestInfo
    {
        /// <summary>
        /// 
        /// </summary>
        public List<ResourceRecordInfoItem> ResourceRecordInfo { get; set; }

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
