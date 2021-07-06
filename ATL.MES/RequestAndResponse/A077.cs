using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATL.MES.A077
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
        public string Type { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string OperationMark { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<Containers> Containers { get; set; }
    }
    public class Containers
    {
        /// <summary>
        /// 
        /// </summary>
        public string Container { get; set; }
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
        /// <summary>
        /// 
        /// </summary>
        public string SpecParamID { get; set; }
    }
}
