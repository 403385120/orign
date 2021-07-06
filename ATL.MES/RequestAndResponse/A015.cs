using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/// <summary>
/// 发送上料相关数据
/// {"Header":{"SessionID":"GUID","FunctionID":"A015","PCName":"PCName","EQCode":"EQ00000001","SoftName":"ClientSoft1","RequestTime":"2019-05-24 15:28:34 488"},"RequestInfo":{"MaterialID":"xxxx"}}
/// </summary>
namespace ATL.MES.A015
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
        public string MaterialID { get; set; }
        /// <summary>
        /// HighOrLowFlag 高低阻标签 H高阻L低阻
        /// </summary>
        public string HighOrLowFlag { get; set; }
        /// <summary>
        /// 0-电芯进料校验，1-电芯出料校验，2-弹夹进设备校验，3弹夹下料位校验，4-弹夹机电芯校验（专用）；无此字段默认为0-电芯进料校验
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// 工序标识:IC/AC
        /// </summary>
        public string Operation { get; set; }
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
