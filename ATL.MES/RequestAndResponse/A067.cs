﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATL.MES.A067
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
        public string Type { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<ProductsItem> Products { get; set; }

    }
    public class ProductsItem
    {
        /// <summary>
        /// 
        /// </summary>
        public string ProductSN1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string HoldCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Operation { get; set; }
    }
}
