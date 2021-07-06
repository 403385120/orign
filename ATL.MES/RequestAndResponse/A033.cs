using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/// <summary>
/// ACT上传Output数据
/// {"Header":{"SessionID":"GUID","FunctionID":"A033","PCName":"PCName","EQCode":"EQ00000001","SoftName":"ClientSoft1","RequestTime":"2019-05-24 15:28:34 488"},"RequestInfo":{"FILE_NAME":"Y10D3098191901","MACHINE_NO":"Y10","DEVICE_NO":"4","ZONE_NO":"6","PATH":"D:\\BackupData\\AG-Y101-190819-2R\\20190819","START_TIME":"2019-08-19 19:01:36","END_TIME":"2019-08-19 23:55:02","OVER_FLAG":"2","SCHEDULE":"AG-3950-16-212-07C-M","TEST_NAME":"AG-Y101-190819-2R","MI_PACKAGE_NO":"V88","TEST_TYPE":"AG","CELL_COUNT_TEST":"13","cells":[{"CELL_NAME":"V88928D312F3","CHANNEL_INDEX":480},{"CELL_NAME":"V88929A303C7","CHANNEL_INDEX":479},{"CELL_NAME":"V88928C330E4","CHANNEL_INDEX":478},{"CELL_NAME":"V88928F31AA5","CHANNEL_INDEX":477},{"CELL_NAME":"V88929A30E8E","CHANNEL_INDEX":476},{"CELL_NAME":"V88927FW016D","CHANNEL_INDEX":475},{"CELL_NAME":"V88926A312C2","CHANNEL_INDEX":473},{"CELL_NAME":"V88928E330F7","CHANNEL_INDEX":472},{"CELL_NAME":"V88928B32A3C","CHANNEL_INDEX":470},{"CELL_NAME":"V88924F30166","CHANNEL_INDEX":468},{"CELL_NAME":"V88924E33761","CHANNEL_INDEX":467},{"CELL_NAME":"V88929A32356","CHANNEL_INDEX":466},{"CELL_NAME":"V88924D31059","CHANNEL_INDEX":465}],"RawData":[{"CELLNO":"225","STATUS":"--","CYCLICTIME":"1","STEP":"1","CURRENT":"0.0000","VOLTAGE":"513.3000","CAPACITY":"0.0","ENERGY":"0.0","STIME":"2019-08-20 08:57:24 488","DTIME":"0","TEMP":"85.4","PRESSURE":"1060"}]}}
/// </summary>
namespace ATL.MES.A033
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

    public class CellsItem
    {
        /// <summary>
        /// 
        /// </summary>
        public string CELL_NAME { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string OVER_FLAG { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int CHANNEL_INDEX { get; set; }
    }

    public class RawDataItem
    {
        /// <summary>
        /// 
        /// </summary>
        public string CELLNO { get; set; }
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


    public class RequestInfo
    {
        /// <summary>
        /// 
        /// </summary>
        public string FILE_NAME { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string MACHINE_NO { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string DEVICE_NO { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ZONE_NO { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string PATH { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string START_TIME { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string END_TIME { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string OVER_FLAG { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string SCHEDULE { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string TEST_NAME { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string MI_PACKAGE_NO { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string TEST_TYPE { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string CELL_COUNT_TEST { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<CellsItem> cells { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<RawDataItem> RawData { get; set; }
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
