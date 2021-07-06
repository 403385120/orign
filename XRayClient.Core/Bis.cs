using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZY.Logging;

namespace XRayClient.Core
{
    public class Bis
    {
        public static DTS.AutoLine bisInterface = new DTS.AutoLine();

        /// <summary>
        /// OCV
        /// </summary>
        /// <param name="CellName"></param>
        /// <param name="TestType"></param>
        /// <param name="OCV"></param>
        /// <param name="IMP"></param>
        /// <param name="MachineNO"></param>
        /// <returns></returns>
        public int TransferData(string CellName, string TestType, float OCV, float IMP, string MachineNO,
            out string strBis)
        {
            //-1表示条件输入错误；0表示pass；1表示OCV坏品；2表示K,DROP坏品；3表示imp坏品；4表示超时或者缺失项目; 5未知
            int result = -2;
            strBis = "";
            try
            {
                result = bisInterface.TransferData(CellName, TestType, OCV, IMP, MachineNO);
            }
            catch (Exception ex)
            {
                strBis = "OCV数据上传BIS异常" + ex;
            }
            switch (result)
            {
                case -1:
                    strBis = "条件输入错误";
                    break;
                case 0:
                    strBis = "pass";
                    break;
                case 2:
                    strBis = "K,DROP坏品";
                    break;
                case 3:
                    strBis = "imp坏品";
                    break;
                case 4:
                    strBis = "超时或者缺失项目";
                    break;
                case 5:
                    strBis = "未知";
                    break;
            }
            return result;
        }

        /// <summary>
        /// XRay上传接口
        /// </summary>
        /// <param name="CELL_NAME">电芯条码</param>
        /// <param name="user_name">用户名</param>
        /// <param name="machineno">设备编号</param>
        /// <param name="marking">mark 起到一个自动检测和手动检测的区别</param>
        /// <param name="max">最大值</param>
        /// <param name="min">最小值</param>
        /// <param name="results">测量结果</param>
        /// <param name="ng_type">NG或OK类型描述：anode edge error;first layer miss;overhang NG;edge NG;褶皱 NG;Shape NG;Barcode error;Grey error;OK</param>
        /// <param name="info_1">备用字段1</param>
        /// <param name="info_2">备用字段2</param>
        /// <param name="overhang_min">overhang min</param>
        /// <param name="overhang_max">overhang max</param>
        /// <param name="angle">角度值</param>
        /// <returns></returns>
        public int BIS_TransfXRayDataNew(string CELL_NAME, string user_name, string machineno, string marking,
            string max, string min, string results, string ng_type, string info_1, string info_2, string overhang_min,
            string overhang_max, string angle, out string strBis)
        {
            //-1:系统异常信息 0:测量OK 2:OK料不可再测; 3:测量NG； 4:NG料不可再测/人工判定NG不可以再测； 5:人工判定OK; 6:人工判定NG；9:出现其他没考虑到的情况；
            int result = -2;
            strBis = "";
            try
            {
                result = bisInterface.BIS_TransfXRayDataNew(CELL_NAME, user_name, machineno, marking, max, min, results,
                    ng_type, info_1, info_2, overhang_min, overhang_max, angle);
            }
            catch (Exception ex)
            {
                strBis = "XRAY数据上传BIS异常" + ex;
            }
            switch (result)
            {
                case -1:
                    strBis = "系统异常信息";
                    break;
                case 0:
                    strBis = "测量OK";
                    break;
                case 2:
                    strBis = "OK料不可再测";
                    break;
                case 3:
                    strBis = "测量NG";
                    break;
                case 4:
                    strBis = "NG料不可再测/人工判定NG不可以再测";
                    break;
                case 5:
                    strBis = "人工判定OK";
                    break;
                case 6:
                    strBis = "人工判定NG";
                    break;
                case 9:
                    strBis = "出现其他没考虑到的情况";
                    break;
            }
            return result;
        }

        /// <summary>
        /// XRay上传接口
        /// </summary>
        /// <param name="CELL_NAME">电芯条码</param>
        /// <param name="user_name">用户名</param>
        /// <param name="machineno">设备编号</param>
        /// <param name="marking">mark 起到一个自动检测和手动检测的区别</param>
        /// <param name="max">最大值</param>
        /// <param name="min">最小值</param>
        /// <param name="results">测量结果</param>
        /// <param name="ng_type">NG或OK类型描述：anode edge error;first layer miss;overhang NG;edge NG;褶皱 NG;Shape NG;Barcode error;Grey error;OK</param>
        /// <param name="model_type">MWT模式</param>
        /// <param name="info_1">备用字段1</param>
        /// <param name="info_2">备用字段2</param>
        /// <returns></returns>
        public int BIS_TransfXRayDataNew(string CELL_NAME, string user_name, string machineno, string marking,
            string max, string min, string results, string ng_type, string info_1, string info_2, out string strBis)
        {
            //-1:系统异常信息 0:测量OK2:OK料不可再测; 3:测量NG； 4:NG料不可再测/人工判定NG不可以再测； 5:人工判定OK; 6:人工判定NG；9:出现其他没考虑到的情况；
            int result = -2;
            strBis = "";
            try
            {
                result = bisInterface.BIS_TransfXRayDataNew(CELL_NAME, user_name, machineno, marking, max, min, results,
                    ng_type, info_1, info_2);
            }
            catch (Exception ex)
            {
                strBis = "XRAY数据上传BIS异常" + ex;
            }
            switch (result)
            {
                case -1:
                    strBis = "系统异常信息";
                    break;
                case 0:
                    strBis = "测量OK";
                    break;
                case 2:
                    strBis = "OK料不可再测";
                    break;
                case 3:
                    strBis = "测量NG";
                    break;
                case 4:
                    strBis = "NG料不可再测/人工判定NG不可以再测";
                    break;
                case 5:
                    strBis = "人工判定OK";
                    break;
                case 6:
                    strBis = "人工判定NG";
                    break;
                case 9:
                    strBis = "出现其他没考虑到的情况";
                    break;
            }
            return result;
        }

        /// <summary>
        /// Mylar 数据上传
        /// </summary>
        /// <param name="CELL_NAME">电芯条码 </param>   
        /// <param name="BARCODE_THICK">电芯厚度</param>
        /// <param name="BARCODE_LENGTH">电芯长度</param>
        /// <param name="BARCODE_WIDTH">电芯宽度</param>
        /// <param name="TOP_EDGE_WIDTH">顶封边宽</param>
        /// <param name="SEALANT_HEIGHT">sealant高度</param>
        /// <param name="NITAB_LENGTH">Ni Tab边距</param>
        /// <param name="ALTAB_LENGTH">Al Tab边距</param> 
        /// <param name="MID_LENGTH">电芯中心距</param>
        /// <param name="MACHINE_NO">上传机器编号</param>
        /// <param name="AITAB_HEIGHT">Ai Tab高度</param>
        /// <param name="NITAB_HEIGHT">Ni Tab高度</param>
        /// <param name="AISEA_HEIGHT">Ai sealant高度</param>
        /// <param name="NISEA_HEIGHT">Ni sealant高度</param>
        /// <param name="TOP_SHOULDER_WIDTH">头部肩宽</param>
        /// <param name="FLAG">OK/NG标识</param>
        /// <param name="REMARK">备注参数</param>
        /// <param name="L_LENGTH">L边长度</param>
        /// <param name="L_WIDTH">短L边宽度</param>
        /// <param name="NI_RIGHT_GLUE">Ni_右侧小白胶</param>
        /// <param name="AL_RIGHT_GLUE">Al_右侧小白胶</param>
        /// <param name="PPG_Weight">PPG重量</param>
        /// <returns>1:上传成功</returns>
        public int BIS_TransfMylarData(string CELL_NAME, string BARCODE_THICK, string BARCODE_LENGTH,
            string BARCODE_WIDTH, string TOP_EDGE_WIDTH, string SEALANT_HEIGHT, string NITAB_LENGTH, string ALTAB_LENGTH,
            string MID_LENGTH, string MACHINE_NO,
            string AITAB_HEIGHT, string NITAB_HEIGHT, string AISEA_HEIGHT, string NISEA_HEIGHT,
            string TOP_SHOULDER_WIDTH, string FLAG, string REMARK, string L_LENGTH, string L_WIDTH, string NI_RIGHT_GLUE,
            string AL_RIGHT_GLUE, string PPG_Weight, out string strBis)
        {
            //0:OK -1:NG 99:超次数 -100超静置时间
            int result = -2;
            strBis = "";
            try
            {
                result = bisInterface.BIS_TransfMylarData(CELL_NAME, BARCODE_THICK, BARCODE_LENGTH, BARCODE_WIDTH,
                    TOP_EDGE_WIDTH, SEALANT_HEIGHT, NITAB_LENGTH, ALTAB_LENGTH,
                    MID_LENGTH, MACHINE_NO, AITAB_HEIGHT, NITAB_HEIGHT, AISEA_HEIGHT, NISEA_HEIGHT, TOP_SHOULDER_WIDTH,
                    FLAG, REMARK, L_LENGTH, L_WIDTH, NI_RIGHT_GLUE,
                    AL_RIGHT_GLUE, PPG_Weight);
            }
            catch (Exception ex)
            {
                strBis = "MDI数据上传BIS异常" + ex;
            }
            switch (result)
            {
                case -1:
                    strBis = "NG";
                    break;
                case 0:
                    strBis = "OK";
                    break;
                case 99:
                    strBis = "超次数";
                    break;
                case -100:
                    strBis = "超静置时间";
                    break;
            }
            return result;
        }

        /// <summary>
        /// 检查员工工号是否合法函数
        /// </summary>
        /// <param name="strOperator">操作员工号</param>
        /// <param name="strSegment">工段</param>
        /// <param name="strProc">工序</param>
        /// <param name="strQuarters">岗位</param>
        /// <param name="strFactoryCode">工厂代码</param>
        /// <param name="strMsg">返回信息</param>
        /// <returns>true 合法，false 不合法</returns>
        public bool New_check_operator_ID(string strOperator, string strSegment, string strProc, string strQuarters,
            string strFactoryCode, ref string strMsg)
        {
            bool result = false;
            try
            {
                result = bisInterface.GetMHRQuartersInfo(strOperator, strSegment, strProc, strQuarters, strFactoryCode, ref strMsg);
            }
            catch (Exception ex)
            {
                strMsg = "检查员工工号异常 " + ex;
            }
            return result;
        }


        /// <summary>
        /// IV TESTER数据上传
        /// </summary>
        /// <param name="CellName">电芯条码</param>
        /// <param name="TestType">测试类型</param>
        /// <param name="MachineNO">设备名</param>
        /// <param name="ExTestType">固定值IV</param>
        /// <param name="ExValue">值</param>
        /// <param name="ExPassFlag">是否导通</param>
        /// <returns></returns>
        public int TransferDataExTest(string CellName, string TestType, string MachineNO, string ExTestType,
            float ExValue, string ExPassFlag, out string strBis)
        {
            //0:OK 6:NG
            int result = -2;
            strBis = "";
            try
            {
                result = bisInterface.TransferDataExTest(CellName, TestType, MachineNO, ExTestType, ExValue, ExPassFlag);
            }
            catch (Exception ex)
            {
                strBis = "MDI数据上传BIS异常" + ex;
            }
            switch (result)
            {
                case 0:
                    strBis = "OK";
                    break;
                case 6:
                    strBis = "NG";
                    break;
            }
            return result;
        }

        /// <summary>
        /// X-Ray图片上传接口
        /// </summary>
        /// <param name="cellname">电芯barcode</param>
        /// <param name="picpath">图片本地路径</param>
        /// <param name="picname">图片名称</param>
        /// <param name="machine_no">机器号</param>
        /// <param name="isreset">是否重传，0是不重传，1是重传</param>
        /// <param name="isreset">上传用户</param>
        /// <param name="msg">输出信息</param>
        /// <returns></returns>
        public bool BISXRayPicUpload(string cell_name, string picpath, string picname, string machine_no, string isreset,
            string username, out string msg)
        {
            //true:OK false:NG
            bool result = false;
            msg = "";
            try
            {
                result = bisInterface.BISXRayPicUpload(cell_name, picpath, picname, machine_no, isreset, username,
                 out msg);
            }
            catch (Exception ex)
            {
                msg = "X-Ray图片上传BIS异常" + ex;
            }
            return result;
        }

        /// <summary>
        /// Mylar 数据上传
        /// </summary>
        /// <param name="CELL_NAME">电芯条码 </param>   
        /// <param name="BARCODE_THICK">电芯厚度</param>
        /// <param name="BARCODE_LENGTH">电芯长度</param>
        /// <param name="BARCODE_WIDTH">电芯宽度</param>
        /// <param name="TOP_EDGE_WIDTH">顶封边宽</param>
        /// <param name="SEALANT_HEIGHT">sealant高度</param>
        /// <param name="NITAB_LENGTH">Ni Tab边距</param>
        /// <param name="ALTAB_LENGTH">Al Tab边距</param> 
        /// <param name="MID_LENGTH">电芯中心距</param>
        /// <param name="MACHINE_NO">上传机器</param>
        /// <param name="AITAB_HEIGHT">Ai Tab高度</param>
        /// <param name="NITAB_HEIGHT">Ni Tab高度</param>
        /// <param name="AISEA_HEIGHT">Ai sealant高度</param>
        /// <param name="NISEA_HEIGHT">Ni sealant高度</param>
        /// <param name="TOP_SHOULDER_WIDTH">头部肩宽</param>
        /// <param name="FLAG">OK/NG标识</param>
        /// <param name="REMARK">备注参数</param>
        /// <param name="L_LENGTH">L边长度</param>
        /// <param name="L_WIDTH">短L边宽度</param>
        /// <param name="NI_RIGHT_GLUE">Ni_右侧小白胶</param>
        /// <param name="AL_RIGHT_GLUE">Al_右侧小白胶</param>
        /// <param name="PPG_Weight">PPG重量</param>
        /// <param name="BARCODE_WIDTH2">宽度W2</param>
        /// <param name="NI_SEA_LEFT">NI selant左外露</param>
        /// <param name="NI_SEA_RIGHT">NI selant右外露</param>
        /// <param name="AL_SEA_LEFT">ALselant左外露</param>
        /// <param name="AL_SEA_RIGHT">AL selant右外露</param>
        /// <param name="AL_TAB_LENGTH">AL Tab 长度</param>
        /// <param name="NI_TAB_LENGTH">NI Tab 长度</param>
        /// <param name="AL_TAB_LEFT_TOP">AL TAB到左侧的距离【上】</param>
        /// <param name="NI_TAB_LEFT_BOTTOM">NI TAB到左侧距离【下】</param>
        /// <param name="EMPLOYEE">工号</param>
        /// <param name="CELL_GROUP">组别</param>
        /// <param name="CELL_LENGTH">电芯总长</param>
        /// <returns>-100：超复测静置时间 -99：超复测次数 -1:上传失败 0：保存失败 1：上传成功</returns>
        public int BIS_TransfMylarData_New(string CELL_NAME, string BARCODE_THICK, string BARCODE_LENGTH,
            string BARCODE_WIDTH, string TOP_EDGE_WIDTH, string SEALANT_HEIGHT, string NITAB_LENGTH, string ALTAB_LENGTH,
            string MID_LENGTH, string MACHINE_NO,
            string AITAB_HEIGHT, string NITAB_HEIGHT, string AISEA_HEIGHT, string NISEA_HEIGHT,
            string TOP_SHOULDER_WIDTH, string FLAG, string REMARK, string L_LENGTH, string L_WIDTH, string NI_RIGHT_GLUE,
            string AL_RIGHT_GLUE, string PPG_Weight,
            string BARCODE_WIDTH2, string NI_SEA_LEFT, string NI_SEA_RIGHT, string AL_SEA_LEFT, string AL_SEA_RIGHT,
            string AL_TAB_LENGTH, string NI_TAB_LENGTH,
            string AL_TAB_LEFT_TOP, string NI_TAB_LEFT_BOTTOM, string EMPLOYEE, string CELL_GROUP, string CELL_LENGTH,
            out string strBis)
        {
            //-100：超复测静置时间 -99：超复测次数 -1:上传失败 0：保存失败 1：上传成功
            int result = -2;
            strBis = "";
            try
            {
                result = bisInterface.BIS_TransfMylarData_New(CELL_NAME, BARCODE_THICK, BARCODE_LENGTH, BARCODE_WIDTH,
                    TOP_EDGE_WIDTH, SEALANT_HEIGHT, NITAB_LENGTH,
                    ALTAB_LENGTH, MID_LENGTH, MACHINE_NO, AITAB_HEIGHT, NITAB_HEIGHT, AISEA_HEIGHT, NISEA_HEIGHT,
                    TOP_SHOULDER_WIDTH, FLAG, REMARK, L_LENGTH,
                    L_WIDTH, NI_RIGHT_GLUE, AL_RIGHT_GLUE, PPG_Weight, BARCODE_WIDTH2, NI_SEA_LEFT, NI_SEA_RIGHT,
                    AL_SEA_LEFT, AL_SEA_RIGHT, AL_TAB_LENGTH,
                    NI_TAB_LENGTH, AL_TAB_LEFT_TOP, NI_TAB_LEFT_BOTTOM, EMPLOYEE, CELL_GROUP, CELL_LENGTH);
            }
            catch (Exception ex)
            {
                strBis = "MDI数据上传BIS异常" + ex;
            }
            switch (result)
            {
                case -100:
                    strBis = "超复测静置时间";
                    break;
                case -99:
                    strBis = "超复测次数";
                    break;
                case -1:
                    strBis = "上传失败";
                    break;
                case 1:
                    strBis = "上传成功";
                    break;
            }
            return result;

        }

        /// <summary> 
        /// 批量电芯加marking 
        /// </summary> 
        /// <param name="arrCellName"></param> 
        /// <param name="Marking"></param> 
        public bool SetMarking(string[] arrCellName, string Marking, out string strBis)
        {
            bool result = false;
            strBis = "";
            try
            {
                bisInterface.SetMarking(arrCellName, Marking);
                result = true;
            }
            catch (Exception ex)
            {
                strBis = "批量电芯加marking异常" + ex;
            }
            return result;
        }

        /// <summary>
        /// 通过BIS获取机台绑定校验是否成功
        /// </summary>
        /// <param name="MiNo">PN号，MI号，工序，设备，计算机名</param>
        /// <param name="error"></param>
        public string GetXRAYSpec(string MiNo, out string error)
        {
            string strResult = "";
            try
            {
                error = "";
                strResult = bisInterface.GetXRAYSpec(MiNo, out error);
            }
            catch (Exception ex)
            {
                error = "获取机台绑定校验异常 " + ex;
            }
            return strResult;
        }

        /// <summary>
        /// 通过BIS获取机台绑定校验是否成功
        /// </summary>
        /// <param name="pn">PN号 空值</param>
        /// <param name="mi">MI号</param>
        /// <param name="process">工序 传入 11</param>
        /// <param name="machine">设备 资产编号</param>
        /// <param name="computer">计算机名 空值</param>
        /// <param name="strBis">异常信息</param>
        /// <returns></returns>
        public bool CheckProcessBindMachine(string pn, string mi, string process, string machine, string computer, out string strBis)
        {
            bool result = false;
            strBis = "";
            try
            {
                result = bisInterface.CheckProcessBindMachine(pn, mi, process, machine, computer);
            }
            catch (Exception ex)
            {
                strBis = "BIS获取机台绑定校验异常 " + ex;
            }
            return result;
        }

        /// <summary>
        /// 通过BIS获取机台上传图片路径
        /// </summary>
        /// <param name="cellname">二维码</param>
        /// <param name="error">异常信息</param>
        /// <returns>路径列表</returns>
        public List<string> GetXRAYFilePath(string cellname, out string error)
        {
            error = "";
            List<string> list = new List<string>();
            try
            {
                list = bisInterface.GetXRAYFilePath(cellname, out error);
            }
            catch (Exception ex)
            {
                error = "BIS获取机台上传图片路径 " + ex;
            }
            return list;
        }

        /// <summary>
        /// 上传OCV数据
        /// </summary>
        /// <param name="TestType">测试类型</param>
        /// <param name="CellName1">O1条码</param>
        /// <param name="ocv1">O1电压</param>
        /// <param name="imp1">O1电阻</param>
        /// <param name="temp1">O1温度</param>
        /// <param name="DeviceChannel1">O1通道</param>
        /// <param name="CellName2">O2条码</param>
        /// <param name="ocv2">02电压</param>
        /// <param name="imp2">O2电阻</param>
        /// <param name="temp2">O2温度</param>
        /// <param name="DeviceChannel2">O2通道</param>
        /// <param name="MachineNo">设备编号</param>
        /// <param name="result1">结果1</param>
        /// <param name="result2">结果2</param>
        /// <param name="surfacetemp1">电芯表面温度1</param>
        /// <param name="surfacetemp2">电芯表面温度2</param>
        /// <returns></returns>
        public bool AutoTransfData(string TestType, string CellName1, float ocv1, float imp1, float temp1,
            string DeviceChannel1, string CellName2, float ocv2, float imp2, float temp2, string DeviceChannel2,
            string MachineNo, out int result1, out int result2, string surfacetemp1, string surfacetemp2, out string outResult)
        {
            //-1表示条件输入错误；0表示pass；1表示OCV坏品；2表示K,DROP坏品；3表示imp坏品；4表示超时或者缺失项目;5未知
            bool result = false;
            outResult = "";
            result1 = -2;
            result2 = -2;
            try
            {
                result = bisInterface.AutoTransfData(TestType, CellName1, ocv1, imp1, temp1, DeviceChannel1, CellName2, ocv2, imp2, temp2, DeviceChannel2, MachineNo, out result1, out result2, surfacetemp1, surfacetemp2);
                switch (result1)
                {
                    case -1:
                        outResult = "条件输入错误";
                        break;
                    case 0:
                        outResult = "pass";
                        result = true;
                        break;
                    case 1:
                        outResult = "OCV坏品";
                        break;
                    case 2:
                        outResult = "DROP坏品";
                        break;
                    case 3:
                        outResult = "imp坏品";
                        break;
                    case 4:
                        outResult = "超时或者缺失项目";
                        break;
                    case 5:
                        outResult = "未知";
                        break;
                    default:
                        outResult = "未知错误";
                        break;
                }
            }
            catch (Exception ex)
            {
                outResult = "BIS上传OCV数据异常 " + ex;
            }
            return result;
        }
    }
}
