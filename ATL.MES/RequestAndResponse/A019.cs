using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/// <summary>
/// 设备向MES发送设备状态
/// {"Header":{"SessionID":"GUID","PCName":"PCName","FunctionID":"A019","EQCode":"EQ00000001","SoftName":"ClientSoft1","RequestTime":"2019-05-24 15:28:34 488"},"RequestInfo":{"ParentEQStateCode":"Run/Stop","AndonState":"","ChildEQ":[{"ChildEQCode":"","ChildEQState":""},{"ChildEQCode":"","ChildEQState":""}],"Quantity":"1000"}}
/// </summary>
namespace ATL.MES.A019
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



    public class ChildEQItem
    {
        /// <summary>
        /// 
        /// </summary>
        public string ChildEQCode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string ChildEQState { get; set; }

    }



    public class RequestInfo
    {
        /// <summary>
        /// 
        /// </summary>
        public string ParentEQStateCode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string AndonState { get; set; }

        public string MiniCMState { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<ChildEQItem> ChildEQ { get; set; }

        /// <summary>
        /// A019指令中上传的Quantity的值为设备自投产以来生产的产品总数量--（设备中无法被人工操作清零）
        /// </summary>
        public string Quantity { get; set; }

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
