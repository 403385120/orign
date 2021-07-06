using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATL.MES.A095
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

    /// <summary>
    /// CheckType是想预留来说，区分是什么类型的点检，比如是Hipot，还是什么，先预留，目前用1
    /// station就是仪器所在位置，要和设备绑定仪器对应的通道一致
    /// 这个指令不需要MES校验，MES只单纯做记录而已
    /// </summary>
    public class ParameterInfoItem
    {
        /// <summary>
        /// 
        /// </summary>
        public string Station { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string item { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string TestValue { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string TargetValue { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string UpperLimitValue { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string LowerLimitValue { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Result { get; set; }
    }

    public class DataListItem
    {
        /// <summary>
        /// 
        /// </summary>
        public string CheckType { get; set; }
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
