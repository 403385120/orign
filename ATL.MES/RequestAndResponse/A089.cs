using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATL.MES.A089
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

    public class RequestInfo
    {
        /// <summary>
        /// 电芯列表，指令支持多个传送
        /// </summary>
        public List<string> SERIALNOS { get; set; }
        /// <summary>
        /// 工序ID，输入G0506\G0506这样的工序
        /// </summary>
        public string OPERATION { get; set; }
        /// <summary>
        /// 工序参数ID，例如3297为ICLV，3257为容量，这些都是固定的。
        /// </summary>
        public string CHARACTERISTIC { get; set; }
        /// <summary>
        /// 分组的code，举例，默认只有一种参数进行分组，这个就可以不填。但如果一个工序里面存在多个参数需要进行分组，
        /// 例如3257和3297都需要分组，那么这两个参数就会各有一套分组规格，就需要对这些参数的进行分类，
        /// 选择当前是那个参数进行分组，所以需要填入GROUPCODE
        /// </summary>
        public string GROUPCODE { get; set; }
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

