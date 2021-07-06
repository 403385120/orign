using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/// <summary>
/// MES获取离线生产数据响应
/// {"Header":{"SessionID":"GUID","FunctionID":"A052","PCName":"PCName","EQCode":"EQ00000001","SoftName":"ClientSoft1","RequestTime":"2019-05-24 15:28:34 488"},"RequestInfo":{"Result":"OK","FolderPath":"E:\ATL-MES2.0\200_设备改造\20_改造方案\通讯协议规约"}}
/// Result--反馈结果，OK正确找到路径，NG为没有找到路径，当Result为NG时FolderPath为空
/// </summary>
namespace ATL.MES.A052
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
        
        public string IsSuccess { get; set; }

        public string ErrorCode { get; set; }

        public string ErrorMsg { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string RequestTime { get; set; }
    }

    public class ResponseInfo
    {
        /// <summary>
        /// 反馈结果，OK正确找到路径，NG为没有找到路径，当Result为NG时FolderPath为空
        /// </summary>
        public string Result { get; set; }
        /// <summary>
        /// E:ATL-MES2.0_设备改造_改造方案通讯协议规约
        /// </summary>
        public string FolderPath { get; set; }
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
