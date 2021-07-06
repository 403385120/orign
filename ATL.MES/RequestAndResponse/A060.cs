using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/// <summary>
/// MES下发配置
/// {"Header":{"SessionID":"GUID","FunctionID":"A060","PCName":"PCName","EQCode":"EQ00000001","SoftName":"ServerSoft","RequestTime":"2019-05-24 15:28:34 488"},"RequestInfo":[{"ParamType":"InputOutput","Param":[{"EquipmentID":"PIEF2522","SendParamID":"13033" ,"UploadParamID":"51262","ParamName":"冷压速度","Type":"Float","SettingValueAddr":"D3000","UpperLimitValueAddr":"D3002","LowerLimitValueAddr":"D3004","InputChangeMonitorAddr":"D5000","ActualValueAddr":"D3006","BycellOutputAddr":"D3008","ParamValueRatio":"1","AlertLevel":"A","AlertBitAddr":""}]},{"ParamType":"Alert","Param":[{"EquipmentID":"PIEF2522","SendParamID":"13033","UploadParamID":"51262","ParamName":"冷压速度","Type":"Float","SettingValueAddr":"D3000","UpperLimitValueAddr":"D3002","LowerLimitValueAddr":"D3004","InputChangeMonitorAddr":"D5000","ActualValueAddr":"D3006","BycellOutputAddr":"D3008","ParamValueRatio":"1","AlertLevel":"A","AlertBitAddr":""}]},{"ParamType":"Spart","Param":[{"EquipmentID":"PIEF2522","SendParamID":"13033","UploadParamID":"51262","ParamName":"冷压速度","Type":"Float","SettingValueAddr":"D3000","UpperLimitValueAddr":"D3002","LowerLimitValueAddr":"D3004","InputChangeMonitorAddr":"D5000","ActualValueAddr":"D3006","BycellOutputAddr":"D3008","ParamValueRatio":"1","AlertLevel":"A","AlertBitAddr":""}]}]}
/// </summary>
namespace ATL.MES.A060
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

    public class ParamItem
    {
        /// <summary>
        /// 设备ID（"EquipmentID":"PIEF2522"）
        /// </summary>
        public string EquipmentID { get; set; }
        /// <summary>
        /// input参数ID-A007（"SendParamID":"13033"），	当同一个参数的input参数ID和output参数ID不同时上传接收A007指令时使用input参数ID（SendParamID）进行接收，上传A013、A018数据时使用output参数ID（UploadParamID）进行上传；
        /// </summary>
        public string SendParamID { get; set; }
        /// <summary>
        /// output参数ID-A013（"UploadParamID":"51262"）; 报警编码（UploadParamID）; 关键配件ID（UploadParamID）
        /// </summary>
        public string UploadParamID { get; set; }
        /// <summary>
        /// input/output参数名称（"ParamName":"冷压速度"）; 报警信息:升降气缸报警; 关键配件名称; 
        /// </summary>
        public string ParamName { get; set; }
        /// <summary>
        /// Float、
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// input/output参数设定值地址-A007（"SettingValueAddr":"D3000"）;关键配件预期寿命-A007（"SettingValueAddr":"D3000"）下发地址
        /// </summary>
        public string SettingValueAddr { get; set; }
        /// <summary>
        /// input/output参数设定上限地址-A007（"UpperLimitValueAddr":"D3002"）;
        /// </summary>
        public string UpperLimitValueAddr { get; set; }
        /// <summary>
        /// input/output参数设定下限地址-A007（"LowerLimitValueAddr":"D3004"）;
        /// </summary>
        public string LowerLimitValueAddr { get; set; }
        /// <summary>
        /// 参数是否启用的管控地址
        /// </summary>
        public string LimitControl { get; set; }
        /// <summary>
        /// input/output参数设定参数变更监控地址-A045（"InputChangeMonitorAddr":"D5000"）; Input参数下发界面的当前值监控地址  	当同一个参数的input参数ID和output参数ID不同时监控input参数设定值变化A045的上传指令使用output参数ID进行上传；
        /// </summary>
        public string InputChangeMonitorAddr { get; set; }
        /// <summary>
        /// input/output参数实际值地址-A017（"ActualValueAddr":"D3006"）; 关键配件实际使用寿命地址-A021（"ActualValueAddr":"D3006"）
        /// </summary>
        public string ActualValueAddr { get; set; }
        /// <summary>
        /// input/output参数:by产品的过程值地址-A013（"BycellOutputAddr":"D3008"）
        /// </summary>
        public string BycellOutputAddr { get; set; }
        /// <summary>
        /// input/output参数倍率（"ParamValueRatio":"1"），下载PLC则乘以，上传则除以
        /// </summary>
        public string ParamValueRatio { get; set; }
        /// <summary>
        /// 报警级别（"AlertLevel":"A"）
        /// </summary>
        public string AlertLevel { get; set; }
        /// <summary>
        /// 报警地址位
        /// </summary>
        public string AlertBitAddr { get; set; }
    }

    public class RequestInfoItem
    {
        /// <summary>
        /// InputOutput、Alert、Spart
        /// </summary>
        public string ParamType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<ParamItem> Param { get; set; }
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
        public List<RequestInfoItem> RequestInfo { get; set; }
    }

}
