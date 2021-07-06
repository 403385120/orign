﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATL.MES.A083
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
        /// 
        /// </summary>
        public string oldcontainer { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string newcontainer { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string lotType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string qty { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Operation { get; set; }
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
