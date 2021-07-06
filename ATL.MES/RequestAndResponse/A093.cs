using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATL.MES.A093
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

    public class ParameterInfoItem
    {
        /// <summary>
        /// 
        /// </summary>
        public string ParamID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string StandardValue { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string UpperLimitValue { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string LowerLimitValue { get; set; }
        /// <summary>
        /// 温度
        /// </summary>
        public string Description { get; set; }
    }

    public class DataListItem
    {
        /// <summary>
        /// 
        /// </summary>
        public string ParameterType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<ParameterInfoItem> ParameterInfo { get; set; }
    }

    public class RequestInfo
    {
        /// <summary>
        /// 
        /// </summary>
        public List<DataListItem> DataList { get; set; }
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
