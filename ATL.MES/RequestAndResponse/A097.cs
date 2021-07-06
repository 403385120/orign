using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATL.MES.A097
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
    /// 备注：若是整个弹夹解绑，则LotType输入container，ProductSn输入弹夹条码，这样发送给mes完成整个弹夹解绑
    /// 若是单个电芯从弹夹解绑，则LotType输入 serialno，ProductSn输入单电芯条码，这样发送给mes完成单个电芯从弹夹解绑
    /// </summary>
    public class DataListItem
    {
        /// <summary>
        /// 
        /// </summary>
        public string LotType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ProductSn { get; set; }
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
