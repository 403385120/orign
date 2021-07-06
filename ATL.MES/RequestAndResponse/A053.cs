using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/// <summary>
/// MES向ACT发送路径
/// {"Header":{"SessionID":"GUID","FunctionID":"A053","PCName":"PCName","EQCode":"EQ00000001","SoftName":"ClientSoft1","RequestTime":"2019-05-24 15:28:34 488"},"RequestInfo":{"FolderPath":"E:\\ATL-MES2.0\\200_设备改造\\20_改造方案\\通讯协议规约"}}
/// </summary>
namespace ATL.MES.A053
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
        /// 路径
        /// </summary>
        public string FolderPath { get; set; }
        /// <summary>
        /// 备用
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// 备用
        /// </summary>
        public string TypeDesc { get; set; }
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
