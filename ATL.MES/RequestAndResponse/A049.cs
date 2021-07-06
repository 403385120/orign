using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/// <summary>
/// 设备在MES宕机后，进行清尾生产时产生的电芯码及物料号进行绑定
/// {"Header":{"SessionID":"Guid","FunctionID":"A049","PCName":"PCName","EQCode":"EQ00000001","SoftName":"ServerSoft","RequestTime":"2019-05-24 15:28:34 488"},"RequestInfo":{"ModelInfo":"123","ProductSN":["12345","12345"],"UserInfo":{"UserID":"123","UserName":"ATL","UserLevel":"1"},"MaterialInfo":[{"MaterialID":"P001","MaterialName":"绿胶","LabelNumber":"001","MaterialQuantity":"100","UoM":"EA"}]}}
/// </summary>
namespace ATL.MES.A049
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



    public class UserInfo
    {
        /// <summary>
        /// 
        /// </summary>
        public string UserID { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string UserLevel { get; set; }

    }



    public class MaterialInfoItem
    {
        /// <summary>
        /// 
        /// </summary>
        public string MaterialID { get; set; }

        /// <summary>
        /// 绿胶
        /// </summary>
        public string MaterialName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string LabelNumber { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string MaterialQuantity { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string UoM { get; set; }

    }



    public class RequestInfo
    {
        /// <summary>
        /// 
        /// </summary>
        public string ModelInfo { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<string> ProductSN { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public UserInfo UserInfo { get; set; }

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
