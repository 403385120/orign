using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATL.MES.A029_MTW20
{
    public class Root
    {
        /// <summary>
        /// 
        /// </summary>
        public Header Header { get; set; }

        /// <summary>
        /// MTW TWF工序，Tab和Cell字段可以不输入，NewSerialno和OldSerialno因为绿胶刻码还是主码，因此这俩还是统一输入绿胶刻码即可，LinkSerialno里头暂时LotNo置空即可，SerialNo就是阴极Tab码，如果还有阳极tab码，就在里头增加一组数据
        ///MTW拉线包装工序，Tab和Cell不输入，NewSerialno为电芯最新喷码，OldSerialno为绿胶刻码，后面的全部不输入即可
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
        public string Tab { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Cell { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string NewSerialno { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string OldSerialno { get; set; }
        /// <summary>
        /// 绑定相关
        /// </summary>
        public List<linkserialno> Linkserialno { get; set; }

    }
    public class linkserialno
    {
        /// <summary>
        /// 
        /// </summary>
        public string LotNo { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string SerialNo { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Quantity { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Uom { get; set; }
    }
}
