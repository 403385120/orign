using ATL.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/// <summary>
/// MES发送初始化开机指令,内容主要包括设备关键部件及设备运行标准参数
/// {"Header":{"SessionID":"Guid","FunctionID":"A007","PCName":"PCName","EQCode":"EQ00000001","SoftName":"ServerSoft","RequestTime":"2019-05-24 15:28:34 488"},"RequestInfo":{"Count":"","CmdInfo":{"ControlCode":"Run","StateCode":"","StateDesc":""},"UserInfo":{"UserID":"123","UserName":"ATL","UserLevel":"1"},"ResourceInfo":{"ResourceID":"EQ00000001","ResourceShift":"M"},"SpartInfo":[{"SpartName":"上烘箱电机同步带","SpartID":"001","SpartExpectedLifetime":"100","ChangeFlag":"true"},{"SpartName":"下烘箱电机同步带","SpartID":"002","SpartExpectedLifetime":"100","ChangeFlag":"true"},{"SpartName":"正涂隔膜泵油封","SpartID":"003","SpartExpectedLifetime":"100","ChangeFlag":"true"},{"SpartName":"反涂隔膜泵油封","SpartID":"004","SpartExpectedLifetime":"100","ChangeFlag":"true"},{"SpartName":"入牵皮带","SpartID":"005","SpartExpectedLifetime":"100","ChangeFlag":"true"},{"SpartName":"出牵皮带","SpartID":"006","SpartExpectedLifetime":"100","ChangeFlag":"true"},{"SpartName":"中间牵引皮带","SpartID":"007","SpartExpectedLifetime":"100","ChangeFlag":"true"}],"ModelInfo":"123","ParameterInfo":[{"ParamID":"001","StandardValue":"45","UpperLimitValue":"50","LowerLimitValue":"30","Description":"温度"},{"ParamID":"002","StandardValue":"45","UpperLimitValue":"50","LowerLimitValue":"30","Description":"压力"}]}}
/// </summary>
namespace ATL.MES.A007
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



    public class CmdInfo
    {
        /// <summary>
        /// 
        /// </summary>
        public string ControlCode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string StateCode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string StateDesc { get; set; }

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



    public class ResourceInfo
    {
        /// <summary>
        /// 
        /// </summary>
        public string ResourceID { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string ResourceShift { get; set; }

    }
    
    public class SpartInfoItem
    {
        /// <summary>
        /// 上烘箱电机同步带
        /// </summary>
        public string SpartName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string SpartID { get; set; }

        public string UsedLife { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string SpartExpectedLifetime { get; set; }

        /// <summary>
        /// 等于true表示刚更换易损件，否则只指示该易损件当前的值
        /// </summary>
        public string ChangeFlag { get; set; }

    }



    public class ParameterInfoItem
    {
        /// <summary>
        /// 
        /// </summary>
        public string ParamID { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string StandardValue { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string UpperLimitValue { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string LowerLimitValue { get; set; }

        /// <summary>
        /// 温度
        /// </summary>
        public string Description { get; set; }

    }



    public class RequestInfo
    {
        /// <summary>
        /// 
        /// </summary>
        public string Count { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public CmdInfo CmdInfo { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public UserInfo UserInfo { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ResourceInfo ResourceInfo { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<SpartInfoItem> SpartInfo { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string ModelInfo { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<ParameterInfoItem> ParameterInfo { get; set; }
        public string OutputParameterInfo()
        {
            return ParameterInfo.ToJSON();
        }
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
