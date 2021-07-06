using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/// <summary>
/// 涂布、冷压、分条Output数据
/// {"Header":{"SessionID":"Guid","FunctionID":"A035","PCName":"PCName","EQCode":"EQ00000001","SoftName":"ClientSoft1","RequestTime":"2019/06/11 20:41:34"},"RequestInfo":{"Model":"品种","EquType":"","ProductSN":"膜卷号","DataList":[{"Data ":[{"ParamID":"","Value":"","ParamDesc":"极片数量"},{"ParamID":"","Value":"","ParamDesc":"下膜长"},{"ParamID":"","Value":"","ParamDesc":"下留白"},{"ParamID":"","Value":"","ParamDesc":"头部错位"},{"ParamID":"","Value":"","ParamDesc":"尾部错位"},{"ParamID":"","Value":"","ParamDesc":"下膜补偿值"},{"ParamID":"","Value":"","ParamDesc":"下留白补偿值"},{"ParamID":"","Value":"","ParamDesc":"上膜长补偿值"},{"ParamID":"","Value":"","ParamDesc":"上留白补偿值"},{"ParamID":"","Value":"","ParamDesc":"头部错位补偿值"},{"ParamID":"","Value":"","ParamDesc":"尾部错位补偿值"},{"ParamID":"","Value":"","ParamDesc":"光纤点距离"},{"ParamID":"","Value":"","ParamDesc":"备注"},{"ParamID":"","Value":"","ParamDesc":"过辊直径"},{"ParamID":"","Value":"","ParamDesc":"上下膜长规格是否对调"},{"ParamID":"","Value":"","ParamDesc":"头尾错位规格是否对调"},{"ParamID":"","Value":"","ParamDesc":"是否闭环"}]}]}}
/// </summary>
namespace ATL.MES.A035
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
    /// <summary>
    /// 一行里单元格数据
    /// </summary>
    public class DataItem
    {
        /// <summary>
        /// 上传ID
        /// </summary>
        public string ParamID { get; set; }
        /// <summary>
        /// 下发ID
        /// </summary>
        public string SpecParamID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Value { get; set; }
        /// <summary>
        /// 极片数量
        /// </summary>
        public string ParamDesc { get; set; }
    }

    /// <summary>
    /// 行数据    
    /// </summary>
    public class DataListItem
    {
        /// <summary>
        /// 膜卷号
        /// </summary>
        public string ProductSN { get; set; }
        /// <summary>
        /// 2019-06-11 20:41:34 488
        /// </summary>
        public string CreateTime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<DataItem> Data { get; set; }
    }

    public class RequestInfo
    {
        /// <summary>
        /// 品种
        /// </summary>
        public string Model { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string EquType { get; set; }
        /// <summary>
        /// ProcessData/ProductionData
        /// </summary>
        public string DataType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<DataListItem> DataList { get; set; }
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
