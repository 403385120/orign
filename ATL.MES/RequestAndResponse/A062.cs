using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/// <summary>
/// 卷绕物料消耗计数: MaterialID：物料条码  MaterialConsumption：切刀数    Unload：是否换料，True换料；Flase不换料
/// {"Header":{"SessionID":"GUID","FunctionID":"A062","PCName":"PCName","EQCode":"EQ00000001","SoftName":"ClientSoft1","RequestTime":"2019-06-11 20:16:02 455"},"RequestInfo":{"MaterialInfo":[{"MaterialID":"","MaterialConsumption":"1235","Unload":"True/Flase"},{"MaterialID":"","MaterialConsumption":"1235","Unload":"True/Flase"},{"MaterialID":"","MaterialConsumption":"1235","Unload":"True/Flase"},{"MaterialID":"","MaterialConsumption":"1235","Unload":"True/Flase"}]}}
/// </summary>
namespace ATL.MES.A062
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

    public class MaterialInfoItem
    {
        /// <summary>
        /// 
        /// </summary>
        public string MaterialID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string MaterialConsumption { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Unload { get; set; }
    }

    public class RequestInfo
    {
        /// <summary>
        /// 
        /// </summary>
        public List<MaterialInfoItem> MaterialInfo { get; set; }
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
