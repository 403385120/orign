using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/// <summary>
/// 设备向MES发送弹夹号及对应各个电芯号
/// {"Header":{"SessionID":"GUID","FunctionID":"A031","EQCode":"EQ00000001","SoftName":"ClientSoft1","RequestTime":"2019-05-24 15:28:34 488"},"RequestInfo":{"Carrier":"W12345","Cell":["C00000001","C00000002","C00000003"]}}
/// </summary>
namespace ATL.MES.A031
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
        public string Carrier { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<string> Cell { get; set; }

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
