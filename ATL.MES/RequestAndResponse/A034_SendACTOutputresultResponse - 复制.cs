using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/// <summary>
/// MES收到ACT的output数据,反馈电芯结果和不良代码
/// {"Header":{"SessionID":"GUID","FunctionID":"A034","PCName":"PCName","EQCode":"EQ00000001","SoftName":"ServerSoft","IsSuccess":"True","ErrorCode":"0","ErrorMsg":"Null","RequestTime":"2019-05-24 15:28:34 509","ResponseTime":"2019-05-24 15:28:34 509"},"ResponseInfo":{"Type":"Normal","Products":[{"ProductSN":"01","Pass":"OK/NG","ResultCode":"","Param":{"STATUS":"OK/NG","CYCLICTIME":"OK/NG","STEP":"OK/NG","CURRENT":"OK/NG","VOLTAGE":"OK/NG","CAPACITY":"OK/NG","ENERGY":"OK/NG","STIME":"OK/NG","DTIME":"OK/NG","TEMP":"OK/NG","PRESSURE":"OK/NG"}},{"ProductSN":"02","Pass":"OK/NG","ResultCode":"","Param":{"STATUS":"OK/NG","CYCLICTIME":"OK/NG","STEP":"OK/NG","CURRENT":"OK/NG","VOLTAGE":"OK/NG","CAPACITY":"OK/NG","ENERGY":"OK/NG","STIME":"OK/NG","DTIME":"OK/NG","TEMP":"OK/NG","PRESSURE":"OK/NG"}}]}}
/// </summary>
namespace ATL.MES.A034
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

    public class Param
    {
        /// <summary>
        /// 
        /// </summary>
        public string STATUS { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string CYCLICTIME { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string STEP { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string CURRENT { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string VOLTAGE { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string CAPACITY { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ENERGY { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string STIME { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string DTIME { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string TEMP { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string PRESSURE { get; set; }
    }

    public class ProductsItem
    {
        /// <summary>
        /// 
        /// </summary>
        public string ProductSN { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Pass { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ResultCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Param Param { get; set; }
    }

    public class ResponseInfo
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




}
