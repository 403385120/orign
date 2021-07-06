using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/// <summary>
/// 发送上料相关数据
/// {"Header":{"SessionID":"GUID","FunctionID":"A015","PCName":"PCName","EQCode":"EQ00000001","SoftName":"ClientSoft1","RequestTime":"2019-05-24 15:28:34 488"},"RequestInfo":{"MaterialID":"xxxx"}}
/// </summary>
namespace ATL.MES.A015_minicell
{
    //【type= 0,      MaterialID    代表电芯，      电芯是否允许进入弹夹机以外的设备验证时使用】
    //【type=1,       MaterialID 代表电芯，      电芯即将从设备流出并上传mes时使用，暂时无用】
    //【type=2,       MaterialID 代表弹夹，      弹夹是否允许进入设备验证时使用（注液弹夹进入暂时是取消扫描）】
    //【type=3,       MaterialID 代表弹夹, 设备出料时使用的弹夹验证】
    //【type=4,       MaterialID 代表电芯, 弹夹机电芯上料验证专用】
    //【type=6,       MaterialID 代表托盘, tray卷绕上料验证专用】
    //【type=7,       MaterialID 代表托盘, tray包装上料验证专用，MaterialID1为上一个Tray，在包装上料时，清空上一个tray盘的信息】
    //【type=8,       MaterialID 代表电芯, 给转接焊, 包装等非卷绕的首道工序校验裸电芯是否有过站记录】
    //【type=9,       MaterialID 代表电芯, MTW允许通过tab码入站, MES通过tab码找出绿胶码做入站处理, 并返回绿胶码给上位机】
    //【type=10,       MaterialID 代表电芯, 新增 MaterialID1，表示弹夹，给到W2-4F Mini 拉线注液机反绑Baking数据使用，同时进行inbound操作。这个与其他场景不冲突   】

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

    public class ParamItem
    {
        /// <summary>
        /// 
        /// </summary>
        public string ParamID { get; set; }
        /// <summary>
        /// 压力
        /// </summary>
        public string ParamDesc { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Value { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Result { get; set; }
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
        public string ChildEquCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Station { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<ParamItem> Param { get; set; }
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
