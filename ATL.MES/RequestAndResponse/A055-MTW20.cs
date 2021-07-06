using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATL.MES.A055_MTW20
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
        public string ChildEquipment { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<ParamInfoItem> ParamInfo { get; set; }
    }

    public class ParamInfoItem
    {
        /// <summary>
        /// 
        /// </summary>
        public List<DataItem> Data { get; set; }
    }

    public class DataItem
    {
        /// <summary>
        /// 
        /// </summary>
        public string ParamID { get; set; }
        /// <summary>
        /// 真空度
        /// </summary>
        public string ParamDesc { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ParamValue { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string TestDate { get; set; }
    }
}
