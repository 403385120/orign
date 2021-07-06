using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/// <summary>
/// 接收设备生成的数据
/// {"Header":{"SessionID":"GUID","FunctionID":"A014","PCName":"PCName","EQCode":"EQ00000001","SoftName":"ServerSoft","IsSuccess":"True","ErrorCode":"0","ErrorMsg":"Null","RequestTime":"2019-05-24 15:28:34 509","ResponseTime":"2019-05-24 15:28:34 509"},"ResponseInfo":{"Type":"Normal","Products":[{"ProductSN":"01","Pass":"OK/NG","Param":[{"ParamID":"001","ParamDesc":"压力","Installation":"125.78","KValue":"125.78","Result":"OK"},{"ParamID":"002","ParamDesc":"温度","Installation":"125.78","KValue":"125.78","Result ":"UN/OK/NG"}]},{"ProductSN":"02","Pass":"OK/NG","Param":[{"ParamID":"001","ParamDesc":"压力","Installation":"125.78","KValue":"125.78","Result ":"UN/OK/NG"},{"ParamID":"002","ParamDesc":"温度","Installation":"125.78","KValue":"125.78","Result ":"UN/OK/NG"}]}]}}

///{"Header":{"SessionID":"GUID","FunctionID":"A014","PCName":"PCName","EQCode":"EQ00000001","SoftName":"ServerSoft","IsSuccess":"True","ErrorCode":"0","ErrorMsg":"Null","RequestTime":"2019-05-24 15:28:34 509","ResponseTime":"2019-05-24 15:28:34 509"},"ResponseInfo":{"Type":"AUTO","Products":[{}]}}
/// </summary>
namespace ATL.MES.A014
{
    public class Header
    {
        /// <summary>
        /// 
        /// </summary>
        public string FunctionID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ResponseTime { get; set; }
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
        public string SessionID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string EQCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string RequestTime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string SoftName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string PCName { get; set; }
    }

    public class ParamItem
    {
        /// <summary>
        /// 
        /// </summary>
        public string ParamID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ParamDesc { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Value { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Result { get; set; }
    }

    public class ProductsItem
    {
        /// <summary>
        /// 
        /// </summary>
        public string ProductSN { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Pass { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<ParamItem> Param { get; set; }
    }

    public class ResponseInfo
    {
        /// <summary>
        /// 
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<ProductsItem> Products { get; set; }
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
