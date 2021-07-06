using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/// <summary>
/// 真空baking设备与freebaking设备OUTPUT数据专用,因Freebaking与真空baking无法获取电芯码，故使用此结构的OUTPUT数据;数据上传
/// {"Header":{"SessionID":"GUID","FunctionID":"A055","PCName":"PCName","EQCode":"EQ00000001","SoftName":"ServerSoft","RequestTime":"2019-05-24 15:28:34 488"},"RequestInfo":{"ParamInfo":[{"ParamID":"001","ParamDesc":"压力","ParamValue":"123","TestDate":"2019-05-24 15:28:34 488"},{"ParamID":"002","ParamDesc":"转速","ParamValue":"123","TestDate":"2019-05-24 15:28:34 488"},{"ParamID":"003","ParamDesc":"温度","ParamValue":"123","TestDate":"2019-05-24 15:28:34 488"}]}}
/// </summary>
namespace ATL.MES.A055
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
        /// 上传ID
        /// </summary>
        public string ParamID { get; set; }

        /// <summary>
        /// 下发ID
        /// </summary>
        public string SpecParamID { get; set; }
        /// <summary>
        /// 压力
        /// </summary>
        public string ParamDesc { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ParamValue { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string TestDate { get; set; }
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
