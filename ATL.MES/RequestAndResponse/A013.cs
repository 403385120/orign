using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/// <summary>
/// 发送设备生成的数据，内容主要包括原料、半成品或成品的工艺参数
/// {"Header":{"SessionID":"Guid","FunctionID":"A013","PCName":"PCName","EQCode":"EQ00000001","SoftName":"ClientSoft1","RequestTime":"2019/06/11 20:41:34"},"RequestInfo":{"Type":"Normal","Products":[{"ProductSN":"01","OutputParam":[{"ParamID":"001","ParamDesc":"压力","ParamValue":"125.78","Result ":"OK"},{"ParamID":"002","ParamDesc":"温度","ParamValue":"125.78","Result ":"UN/OK/NG"}]},{"ProductSN":"02","OutputParam":[{"ParamID":"001","ParamDesc":"压力","ParamValue":"125.78","Result ":"UN/OK/NG"},{"ParamID":"002","ParamDesc":"温度","ParamValue":"125.78","Result":"UN/OK/NG"}]}]}}
/// </summary>
namespace ATL.MES.A013
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



    public class OutputParamItem
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
        /// OK/NG/null
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
        public List<OutputParamItem> OutputParam { get; set; }
    }



    public class RequestInfo
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
        public RequestInfo RequestInfo { get; set; }

    }

    public class _ProductsItem
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
    }

}
