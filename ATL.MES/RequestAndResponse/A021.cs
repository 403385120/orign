using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/// <summary>
/// 设备向MES发送设备关键件寿命数据
/// {"Header":{"SessionID":"GUID","FunctionID":"A021","PCName":"PCName","EQCode":"EQ00000001","SoftName":"ClientSoft1","RequestTime":"2019/06/11 20:16:02"},"RequestInfo":{"SpartLifeTimeInfo":[{"SpartID":"001","SpartName":"同步带","UseLifetime":"568"},{"SpartID":"002","SpartName":"电动隔膜泵密封","UseLifetime":"854"},{"SpartID":"003","SpartName":"牵引皮带","UseLifetime":"124"},{"SpartID":"004","SpartName":"皮带","UseLifetime":"674"}]}}
/// </summary>
namespace ATL.MES.A021
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



    public class SpartLifeTimeInfoItem
    {
        /// <summary>
        /// 
        /// </summary>
        public string SpartID { get; set; }

        /// <summary>
        /// 同步带
        /// </summary>
        public string SpartName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string UseLifetime { get; set; }

    }



    public class RequestInfo
    {
        /// <summary>
        /// 
        /// </summary>
        public List<SpartLifeTimeInfoItem> SpartLifeTimeInfo { get; set; }

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
