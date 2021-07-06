using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATL.MES.A013_MTW20
{
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
    public class RequestInfo
    {
        /// <summary>
        /// 
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// SkipTransitionValidation":"True"  这个参数是为了X-RAY复判忽略检查设备当前转型model而用的，正常生产不用这个字段，人工复判上传传这个字段即可，True就是忽略检查model
        /// </summary>
        public bool SkipTransitionValidation { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<ProductsItem> Products { get; set; }

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
        public string Station { get; set; }
        /// <summary>
        /// QA复判员工号
        /// </summary>
        public string EmployeeNo { get; set; }
        /// <summary>
        /// PRD复判员工号
        /// </summary>
        public string EmployeeNo1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<OutputParamItem> OutputParam { get; set; }
    }
    public class OutputParamItem
    {
        /// <summary>
        /// 上传ID
        /// </summary>
        public string ParamID { get; set; }

        /// <summary>
        /// 参数名称
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
        public string Station { get; set; }
        /// <summary>
        /// QA复判员工号
        /// </summary>
        public string EmployeeNo { get; set; }
        /// <summary>
        /// PRD复判员工号
        /// </summary>
        public string EmployeeNo1 { get; set; }

    }
}
