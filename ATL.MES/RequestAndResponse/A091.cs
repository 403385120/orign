using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATL.MES.A091
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

    public class DataItem
    {
        /// <summary>
        /// 
        /// </summary>
        public string ParamID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Value { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string SpecParamID { get; set; }
        /// <summary>
        /// 极片数量
        /// </summary>
        public string ParamDesc { get; set; }
    }

    public class DataListItem
    {
        /// <summary>
        /// 膜卷号
        /// </summary>
        public string ProductSN { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string CreateTime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<DataItem> Data { get; set; }
    }

    public class RequestInfo
    {
        /// <summary>
        /// 品种
        /// </summary>
        public string Model { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string EquType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string DataType { get; set; }
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
