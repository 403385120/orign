using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/// <summary>
/// MES接收上料相关数据做验证、返回验证结果
/// {"Header":{"SessionID":"GUID","FunctionID":"A016","PCName":"PCName","EQCode":"EQ00000001","SoftName":"ServerSoft","IsSuccess":"True ","ErrorCode":"0","ErrorMsg":"Null","RequestTime":"2019-05-24 15:28:34 509","ResponseTime":"2019-05-24 15:28:34 509"},"ResponseInfo":{"Result":"Ok"}}
/// </summary>
namespace ATL.MES.A016_minicell
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

    public class ParamItem
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
        public string ChildEquCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Station { get; set; }
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
