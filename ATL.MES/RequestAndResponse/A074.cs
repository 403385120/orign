using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATL.MES.A074
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
        public ResponseInfo ResponseInfo { get; set; }

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
        public string IsSuccess { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ErrorCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ErrorMsg { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string RequestTime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ResponseTime { get; set; }
    }
    public class ResponseInfo
    {
        /// <summary>
        /// 
        /// </summary>
        public List<Cell> CELLLIST { get; set; }
    }
    public class Cell
    {
        /// <summary>
        /// 
        /// </summary>
        public string CELL { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string CONTAINER_STATION { get; set; }
    }
}
