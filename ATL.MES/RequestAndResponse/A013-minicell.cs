using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/// <summary>
/// 发送设备生成的数据，内容主要包括原料、半成品或成品的工艺参数
/// {"Header":{"SessionID":"Guid","FunctionID":"A013","PCName":"PCName","EQCode":"EQ00000001","SoftName":"ClientSoft1","RequestTime":"2019/06/11 20:41:34"},"RequestInfo":{"Type":"Normal","Products":[{"ProductSN":"01","OutputParam":[{"ParamID":"001","ParamDesc":"压力","ParamValue":"125.78","Result ":"OK"},{"ParamID":"002","ParamDesc":"温度","ParamValue":"125.78","Result ":"UN/OK/NG"}]},{"ProductSN":"02","OutputParam":[{"ParamID":"001","ParamDesc":"压力","ParamValue":"125.78","Result ":"UN/OK/NG"},{"ParamID":"002","ParamDesc":"温度","ParamValue":"125.78","Result":"UN/OK/NG"}]}]}}
/// </summary>
namespace ATL.MES.A013_minicell
{
    //对X-RAY复判上传PRD复判以及QA复判工号（新增EmployeeNo（QA复判）以及EmployeeNo1（PRD复判）字段）
    //"SkipTransitionValidation":"True"  这个参数是为了X-RAY复判忽略检查设备当前转型model而用的，正常生产不用这个字段，人工复判上传传这个字段即可，True就是忽略检查model
    //使用tray盘出料时："LotType": "Container"， "ProductSN"为tray盘码，每个参数的"Sation"标识电芯在tray盘中的位置号

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
        public string PCName { get; set; }
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
        /// 
        /// </summary>
        public string ParamID { get; set; }
        /// <summary>
        /// 阳极放卷张力
        /// </summary>
        public string ParamDesc { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ParamValue { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Result { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Sation { get; set; }
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
        public string LotType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string QTY { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Pass { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Station { get; set; }
        /// <summary>
        /// 员工号
        /// </summary>
        public string EmployeeNo { get; set; }
        /// <summary>
        /// 员工号
        /// </summary>
        public string EmployeeNo1 { get; set; }
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
        public string SkipTransitionValidation { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<ProductsItem> Products { get; set; }

        public string OperationMark { get; set; }
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
