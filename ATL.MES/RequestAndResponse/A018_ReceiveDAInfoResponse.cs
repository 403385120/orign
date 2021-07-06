using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/// <summary>
/// 设备收到MES需要数据响应
/// {"Header":{"SessionID":"GUID","FunctionID":"A018","PCName":"PCName","EQCode":"EQ00000001","SoftName":"ClientSoft1","IsSuccess":"True","ErrorCode":"00","ErrorMsg":"Null","RequestTime":"2019-05-24 15:28:34 509","ResponseTime":"2019-05-24 15:28:34 509"},"ResponseInfo":{"ParamInfo":[{"ParamID":"001","ParamDesc":"压力","ParamValue":"123"},{"ParamID":"002","ParamDesc":"转速","ParamValue":"123"},{"ParamID":"003","ParamDesc":"温度","ParamValue":"123"}]}}
/// </summary>
namespace ATL.MES.A018
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
        public string IsSuccess { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string ErrorCode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string ErrorMsg { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string RequestTime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string ResponseTime { get; set; }

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

        /// <summary>
        /// 
        /// </summary>
        public string ParamValue { get; set; }

    }



    public class ResponseInfo
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
        public ResponseInfo ResponseInfo { get; set; }

    }

}
