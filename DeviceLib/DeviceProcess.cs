using ATL.Common;
using ATL.Core;
using ATL.MES;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Threading;
using System.Runtime.ExceptionServices;
using Microsoft.Practices.EnterpriseLibrary.Data.Configuration.Fluent;
//using System.Windows.Documents;

namespace DeviceLib
{
    public class DeviceProcess : BaseFacade
    {
        public  List<CarNumToPos> carnumtopos = new List<CarNumToPos>();
        public  List<CellBarCode> cellbarcode = new List<CellBarCode>();
        public  List<BakingTime> bakingTimes = new List<BakingTime>();
        public  Params p = new Params();
        //上次搬运前后位置
        public  String move;
        //上次上料二维码
        public  String[] shangliaocode=new string[12];
        //上次炉体加热状态
        public  String status;
        //上次扫码电芯二维码
        public  String saoma_code;
        //上次上料数据
        public  String shangliao;
        //位置状态
        public  Int16[] pos_status = new Int16[11];
        //上次异常判断的时间
        public  DateTime lastchecktime;
        public  DateTime lastTemptime;
        public DateTime lastA055time;
        //高低温阈值
        public  float TempSetUpLimit=110;
        public  float TempSetLowLimit=100;
        public static DeviceProcess instance;
        public static DeviceProcess getInstanse()
        {
            if (instance == null)
                instance = new DeviceProcess();
            return instance;
        }
      
        private DeviceProcess()
        {
            init();
        }
        public int ceng=30;
        private int A055time;//发送A055的时间间隔
        //初始化
        public void init()
        {
            setCarNumToPos();
            getCarNumToPos();
            lastchecktime = DateTime.Now;
            lastTemptime = DateTime.Now;
            lastA055time = DateTime.Now;
            A055time = int.Parse(UserDefineVariableInfo.DicVariables["a055time"].ToString());
            InstanceSerialPort();
            //测试
           
        }
        //private void test1()
        //{
        //    String sql = "select * from cellbarcode_01 where LaminateNum=1  and (RowNum=1  or RowNum=2 )";
        //    LogInDB.Info("sql=" + sql.Replace('\'', ' '));
        //}
        #region mes通信 A015 A013
        //上料mes验证
        private bool A015(string MaterialID)
        {
            foreach (var v in DeivceEquipmentidPlcInfo.lstDeivceEquipmentidPlcInfos)
            {
                ATL.MES.A016.Root root = InterfaceClient.Current.A015(MaterialID,"0", ATL.MES.UserInfo.UserName,"", v.EquipmentID);
                if(root!=null)
                {
                    if (root.ResponseInfo.Result != null)
                    {
                        if (root.ResponseInfo.Result.ToUpper().Equals("OK"))
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            
            
            }
            return false;
         }
        //下料mes验证
        private void A013(List<ATL.MES.A013.ProductsItem> lst)
        {
          
            foreach (var v in DeivceEquipmentidPlcInfo.lstDeivceEquipmentidPlcInfos)
            {
                //InterfaceClient.lstReceivedJason.Clear();
                //InterfaceClient.RecvdData r = new InterfaceClient.RecvdData();
                //r.cmd = "A014";
                ////r.jason = @"{""Header"":{""FunctionID"":""A014"",""ResponseTime"":""2020-01-05 20:35:54 215"",""IsSuccess"":""True"",""ErrorCode"":""0"",""ErrorMsg"":""Null"",""SessionID"":""798eb62f-8ce4-4416-81a6-90aba706dd9b"",""EQCode"":""AXRX016F"",""RequestTime"":""2020-01-05 20:36:35 437"",""SoftName"":null,""PCName"":""WPRD-NFNI7062-P""},""ResponseInfo"":{""Type"":""Normal"",""Products"":[{""ProductSN"":""W76952EA16C6"",""Pass"":""OK"",""Param"":[{""ParamID"":""52166"",""ParamDesc"":null,""Value"":""1.104205"",""Result"":""OK""},{""ParamID"":""52167"",""ParamDesc"":null,""Value"":""1.135754"",""Result"":""OK""},{""ParamID"":""52168"",""ParamDesc"":null,""Value"":""1.030591"",""Result"":""OK""},{""ParamID"":""52169"",""ParamDesc"":null,""Value"":""0.9149129"",""Result"":""OK""},{""ParamID"":""52170"",""ParamDesc"":null,""Value"":""0.8202667"",""Result"":""OK""},{""ParamID"":""52171"",""ParamDesc"":null,""Value"":""0.7571693"",""Result"":""OK""},{""ParamID"":""52172"",""ParamDesc"":null,""Value"":""0.8833642"",""Result"":""OK""},{""ParamID"":""52173"",""ParamDesc"":null,""Value"":""0.7466531"",""Result"":""OK""},{""ParamID"":""52181"",""ParamDesc"":null,""Value"":""0.8097505"",""Result"":""OK""},{""ParamID"":""52182"",""ParamDesc"":null,""Value"":""0.7782018"",""Result"":""OK""},{""ParamID"":""52183"",""ParamDesc"":null,""Value"":""0.9149129"",""Result"":""OK""},{""ParamID"":""52184"",""ParamDesc"":null,""Value"":""1.030591"",""Result"":""OK""},{""ParamID"":""52185"",""ParamDesc"":null,""Value"":""1.041108"",""Result"":""OK""},{""ParamID"":""52186"",""ParamDesc"":null,""Value"":""1.030591"",""Result"":""OK""},{""ParamID"":""52187"",""ParamDesc"":null,""Value"":""1.041108"",""Result"":""OK""},{""ParamID"":""51740"",""ParamDesc"":null,""Value"":""5557"",""Result"":""OK""},{""ParamID"":""51741"",""ParamDesc"":null,""Value"":""5557"",""Result"":""OK""},{""ParamID"":""51011"",""ParamDesc"":null,""Value"":""58"",""Result"":""OK""},{""ParamID"":""51012"",""ParamDesc"":null,""Value"":""86"",""Result"":""OK""},{""ParamID"":""51856"",""ParamDesc"":null,""Value"":""OK_20200105203634957_W76952EA16C6_5_test.jpg"",""Result"":""OK""},{""ParamID"":""251"",""ParamDesc"":null,""Value"":""0.7466531"",""Result"":""OK""}]}]}}";
                //r.sendTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff");
                //r.receivedTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff");
                //InterfaceClient.lstReceivedJason.Add(r);

                ATL.MES.A014.Root root = InterfaceClient.Current.A013("Normal",lst, v.EquipmentID);
                if (root != null)
                {
                    if (root.Header.IsSuccess.ToUpper().Equals("TRUE"))
                    {
                        LogInDB.Info("A014 success");
                        return;
                    }
                    else
                    {
                        String errorcode = root.Header.ErrorCode;
                        String errormsg = root.Header.ErrorMsg;
                        LogInDB.Info("下料mes反馈错误："+errorcode+"-"+errormsg);
                    }
                }
            }
        }

        //过程数据
        private bool A055(String childeq,List<ATL.MES.A055_MTW20.ParamInfoItem> lst)
        {
            foreach (var v in DeivceEquipmentidPlcInfo.lstDeivceEquipmentidPlcInfos)
            {
                ATL.MES.A056.Root root = InterfaceClient.Current.A055_MTW20(childeq, lst, v.EquipmentID); ;
                if (root != null)
                {
                    if (root.ResponseInfo.Result != null)
                    {
                        if (root.ResponseInfo.Result.ToUpper().Equals("OK"))
                        {
                            LogInDB.Info("A056 success");
                            return true;
                        }
                        else
                        {
                            String errorcode = root.Header.ErrorCode;
                            String errormsg = root.Header.ErrorMsg;
                            LogInDB.Info("A056反馈错误：" + errorcode + "-" + errormsg);
                            return false;
                        }
                    }
                }


            }
            return false;
        }


        private void sendA055(Int16[] temps, float[] vacus)
        {
            List<ATL.MES.A055_MTW20.ParamInfoItem> lst = new List<ATL.MES.A055_MTW20.ParamInfoItem>();
            Int16[] avgtemp = new Int16[vacus.Length];
            for (int i = 0; i < 7; i++)
            {
                int sum = 0;
                for(int j=0;j<ceng;j++)
                {
                    sum += temps[i * 32 + j];
                }
                sum = sum / ceng;
                avgtemp[i] = (Int16)sum;
            }
            ATL.MES.A055_MTW20.ParamInfoItem parm1 = new ATL.MES.A055_MTW20.ParamInfoItem();
            parm1.Data = new List<ATL.MES.A055_MTW20.DataItem>();
            //发送7个炉子过程数据
            for (int i = 0; i < vacus.Length; i++)
            {
                ATL.MES.A055_MTW20.DataItem item = new ATL.MES.A055_MTW20.DataItem();
                item.ParamID = "85558";//1#炉子

                item.ParamDesc = "真空度";
                item.ParamValue = vacus[i]+"";
                item.TestDate = DateTime.Now.ToString();
                parm1.Data.Add(item);

                item = new ATL.MES.A055_MTW20.DataItem();
                item.ParamID = "199";//1#炉子

                item.ParamDesc = "温度";
                item.ParamValue = avgtemp[i]+"";//第1层的问题
                item.TestDate = DateTime.Now.ToString();
                parm1.Data.Add(item);
            }
            lst.Add(parm1);
            // LogInDB.Info(lst.ToJSON());
            A055(1 + "", lst);
        }

        //自动抽检
        private bool A067(String chipcode)
        {
            foreach (var v in DeivceEquipmentidPlcInfo.lstDeivceEquipmentidPlcInfos)
            {
                ATL.MES.A067.ProductsItem test = new ATL.MES.A067.ProductsItem();
                test.ProductSN1 = chipcode;
                test.HoldCode = "watercontent";
                test.Operation = "G0109";
                List<ATL.MES.A067.ProductsItem> temp = new List<ATL.MES.A067.ProductsItem>();
                temp.Add(test);

                ATL.MES.A068.Root root = InterfaceClient.Current.A067("Normal", temp, v.EquipmentID);
                if (root != null)
                {
                    if (root.ResponseInfo.Result != null)
                    {
                        if (root.ResponseInfo.Result.ToUpper().Equals("OK"))
                        {
                            LogInDB.Info("A068 success");
                            return true;
                        }
                        else
                        {
                            String errorcode = root.Header.ErrorCode;
                            String errormsg = root.Header.ErrorMsg;
                            LogInDB.Info("A068反馈错误：" + errorcode + "-" + errormsg);
                            return false;
                        }
                    }
                }


            }
            return false;
        }
        #endregion
        //温度值
        Int16[] temps = null;
        //真空度
        float[] vacus = null;
        #region plc交互
        //检查PlC变量变化情况
        public void CheckPlcVar()
        {
            if (!ATL.Core.PLC.Communicating) return;

            Int16 isshangliaook_cur = -1;
            String[] shangliaocode_cur = new String[12];
            String shangliaoinfo_cur = null;

            Int16 issaoma = -1;
            String code = null;

            bool ischeckTemp = false;//本轮是否采集过温度和真空度数据了么
            bool ischecksaoma = false;
            bool ischeckshangliao = false;
            foreach (var UserDefineVariable in UserDefineVariableInfo.lstUserDefineVariables.Where(x => x.PlcRwConfig != null).ToList())//PLC变量
            {
                String name = UserDefineVariable.VariableName.ToString();
                String value = UserDefineVariable.VariableValue.ToString();

                //ATL.Engine.PLC plc = new ATL.Engine.PLC();
                //string s = plc.ReadByVariableName("plc_shangliao_code2");
                //LogInDB.Info("s=" + s);
                //bool res = plc.WriteByVariableName("plc_shangliao_code2", "11111");

                //if (name.Contains("plc_"))
                //{
                //    LogInDB.Info("name=" + name + ",value=" + value);
                //    sendToDebug("name=" + name + ",value=" + value);
                //}
                //搬运
                if (name.Equals("plc_move"))                   
                    CheckMove(value);

                //上料扫码
                if (name.Equals("plc_saoma_ok"))
                {
                    issaoma = Int16.Parse(value);
                  //  LogInDB.Info("issaomaok="+issaoma);
                }
                if (name.Equals("plc_saoma_code"))
                    code = value;
              

                //上料
                if (name.Equals("plc_shangliao_ok"))
                    isshangliaook_cur = Int16.Parse(value);
                if (name.Equals("plc_shangliao_code1"))
                {
                    ///  LogInDB.Info("name=" + name + ",value=" + value);
                    shangliaocode_cur[0] = value;
                  
                    // LogInDB.Info("1="+ shangliaocode_cur[0]);
                }
                if (name.Equals("plc_shangliao_code2"))
                {
                    shangliaocode_cur[1] = value;
                   
                    // LogInDB.Info("2=" + shangliaocode_cur[1]);
                }
                if (name.Equals("plc_shangliao_code3"))
                {
                    shangliaocode_cur[2] = value;
                   
                    // LogInDB.Info("3=" + shangliaocode_cur[2]);
                }
                if (name.Equals("plc_shangliao_code4"))
                {
                    shangliaocode_cur[3] = value;
                    
                    //  LogInDB.Info("4=" + shangliaocode_cur[3]);
                }
                if (name.Equals("plc_shangliao_code5"))
                {
                    shangliaocode_cur[4] = value;
                    
                }
                    
                if (name.Equals("plc_shangliao_code6"))
                {
                    shangliaocode_cur[5] = value;
                   
                }
                if (name.Equals("plc_shangliao_code7"))
                {
                    shangliaocode_cur[6] = value;
                   
                }
                if (name.Equals("plc_shangliao_code8"))
                    shangliaocode_cur[7] = value;
                if (name.Equals("plc_shangliao_code9"))
                    shangliaocode_cur[8] = value;
                if (name.Equals("plc_shangliao_code10"))
                    shangliaocode_cur[9] = value;
                if (name.Equals("plc_shangliao_code11"))
                    shangliaocode_cur[10] = value;
                if (name.Equals("plc_shangliao_code12"))
                    shangliaocode_cur[11] = value;

                if (name.Equals("plc_shangliao_info"))
                    shangliaoinfo_cur = value;

              

                //出满车空车调度
                if (name.Equals("plc_out"))
                    CheckMoveOut(value);


                //加热状态
                if (name.Equals("plc_status"))                 
                    CheckStatus(value);
                //温度和真空度
                if ((DateTime.Now - lastTemptime).TotalSeconds > 5)
                {
                    if (name.Equals("plc_wendu"))
                    {
                        temps = StrsToInt16(value);
                       // LogInDB.Info("temp:"+value);
                    }
                    if (name.Equals("plc_vacu"))
                    {
                        vacus = StrsToFloat(value);
                     //   LogInDB.Info("vacu:" + value);
                    }
                    if (!ischeckTemp && temps != null && vacus != null)
                    {
                        CheckTempAndVacus(temps, vacus);
                        ischeckTemp = true;
                        lastTemptime = DateTime.Now;
                        temps = null;
                        vacus = null;
                       // LogInDB.Info("lastTemptime="+ lastTemptime);
                    }
                }
                //下料
                if (name.Equals("plc_xialiao"))
                    CheckXialiao(UserDefineVariable.VariableValue.ToString());

                ////位置状态数据
                //if (name.Equals("plc_positon"))
                //    pos_status = StrsToInt16(UserDefineVariable.VariableValue.ToString());

            }
            //LogInDB.Info("issaoma=" + issaoma + ",code=" + code );
           
                CheckSaoMa(issaoma, code);
          

            //上料
            if (CheckShangliao(isshangliaook_cur, shangliaocode_cur, shangliaoinfo_cur))
            {
                shangliaoinfo_cur = null;
                shangliaocode_cur = new String[12];
            }
          

            //定期检查真空度异常 3分钟检查一次
            if ((DateTime.Now - lastchecktime).TotalMinutes>3)
            { 
            //    CheckException(status_cur);
               // lastchecktime = DateTime.Now;
            }

            if ((DateTime.Now - lastA055time).TotalSeconds > A055time)
            {
                if (temps != null && vacus != null)
                {
                    sendA055(temps, vacus);
                    lastA055time = DateTime.Now;
                }
            }

            }
        //检查上料扫码
        private void CheckSaoMa(short issaoma, string code_cur)
        {
            try
            {
                
                //如果扫码验证，重置扫码反馈数据
                if (issaoma == 0)
                {
                    UserDefineVariableInfo.DicVariables["plc_saoma_response"] = 0;
                    saoma_code = null;
                  //  LogInDB.Info("issaoma=0");
                    return;
                }
                //数据不全
                if (code_cur == null || code_cur.Trim().Equals("")) return;
                //数据重复
                if (saoma_code != null &&!saoma_code.Trim().Equals("")&& saoma_code.Equals(code_cur)) return;
                //保存当前数据
                saoma_code = code_cur;
                LogInDB.Info("saoma_code=" + saoma_code);
                bool res = A015(saoma_code);
                LogInDB.Info("res=" + res);
                if (res)
                    UserDefineVariableInfo.DicVariables["plc_saoma_response"] = 10;
                else
                    UserDefineVariableInfo.DicVariables["plc_saoma_response"] = 20;

            }catch(Exception ex)
            {
                LogInDB.Info("上料扫描异常："+ex.Message);
                sendToDebug("上料扫描异常："+ex.Message);
            }
        }

       
        //检查下料
        private void CheckXialiao(string var)
        {
            try
            {
                //从plc变量中读取下料数据
                Int16[] data = StrsToInt16(var);
                if (data == null) return;
                int rownum1 = data[2] * 2 - 1;
                int rownum2 = data[2] * 2;
                //如果数据跟上次重复 表示已经处理过了 
                if (shangliao != null && shangliao.Equals(var)) return;
                //保存本次数据
                shangliao = var;
                String[] response = new String[]{ "0", "0", "0", "0", "0", "0",
                "0", "0", "0", "0", "0","0" };
                //下料验证不为10  跳过      
                if (data[0] != 10)
                {
                    UserDefineVariableInfo.DicVariables["plc_xialiao_response"] = string.Join(",", response);
                    return;
                }
                LogInDB.Info("下料开始");

                //找出下料位的小车信息
                CarNumToPos car = null;
                foreach (CarNumToPos tmp in carnumtopos)
                {
                    if (tmp.Pos == 9)
                    {
                        car = tmp;
                        break;
                    }
                }
                if (car == null)
                {
                    LogInDB.Info("下料位没有小车，请检查！");
                    return;
                }
                LogInDB.Info("carId=" + car.CarID);

                //查找bakingtime的出炉时间
                String sql = "select * from bakingtime where CarNum=" + car.CarID;
                DataSet ds = equipDB.ExecuteDataSet(CommandType.Text, sql);
                DataTable table = ds.Tables[0];
                if (table.Rows.Count == 0)
                {
                    LogInDB.Info("bakingtime 中没有" + car.CarID + "号小车信息");
                    return;
                }
                String moveoutstr = table.Rows[0].ItemArray[3].ToString();//出炉时间
                String bakingbeginstr = table.Rows[0].ItemArray[4].ToString();//保存烘烤开始时间
                String bakingendstr = table.Rows[0].ItemArray[3].ToString();//出炉时间
                LogInDB.Info("出炉时间=" + moveoutstr);
                if (moveoutstr.Equals(""))
                {
                    LogInDB.Info("出炉时间为空，检查bakingtime表的" + car.CarID + "号车的出炉时间");
                    return;
                }

                DateTime moveout = DateTime.Parse(moveoutstr);
                int chululength = 0;
                TimeSpan ts1 = new TimeSpan(moveout.Ticks);
                TimeSpan ts2 = new TimeSpan(DateTime.Now.Ticks);
                TimeSpan ts3 = ts2.Subtract(ts1).Duration();
                Int16 chuluchaoshi = Int16.Parse(UserDefineVariableInfo.DicVariables["chuluchaoshi"].ToString());
                LogInDB.Info("出炉与下料时长=" + ts3.TotalMinutes + "分钟");
                chululength = (int)ts3.TotalMinutes;


                int bakinglength = 0;
                if (!bakingbeginstr.Equals("") && !bakingendstr.Equals(""))
                {
                    DateTime begin = DateTime.Parse(bakingbeginstr);
                    DateTime end = DateTime.Parse(bakingendstr);
                    ts1 = new TimeSpan(begin.Ticks);
                    ts2 = new TimeSpan(end.Ticks);
                    ts3 = ts2.Subtract(ts1).Duration();
                    LogInDB.Info("烘烤时长=" + ts3.TotalMinutes + "分钟");
                    bakinglength = (int)ts3.TotalMinutes;
                }

                //查找小车从那个炉子出来的
                String oval = "";
                sql = "select * from carprocess where CarNum=" + car.CarID + " and State='moveout' order by Time desc limit 1";
                ds = equipDB.ExecuteDataSet(CommandType.Text, sql);
                table = ds.Tables[0];
                if (table.Rows.Count > 0)
                    oval = table.Rows[0].ItemArray[2].ToString();
                
                String name = car.CarID < 10 ? "0" + car.CarID : "" + car.CarID;
                //查找这层温度
                DateTime bakingstart = DateTime.Parse(bakingbeginstr).AddMinutes(60);
                String temp = "";
                String Lam = data[1] < 0 ? "0" + data[1] : ""+data[1];
                sql = "select avg(Temp" + Lam + ") from carpara_" + name + " where RecodeTime>'" + bakingstart.ToString() + "' and RecodeTime<'" + bakingendstr + "'";
                ds = equipDB.ExecuteDataSet(CommandType.Text, sql);
                table = ds.Tables[0];
                if (table.Rows.Count > 0)
                   temp = table.Rows[0].ItemArray[0].ToString();
                //查找这层的真空度
                String vacu = "";
                bakingstart = DateTime.Parse(bakingbeginstr).AddMinutes(70);
                sql = "select avg(Vacuum) from carpara_" + name + " where RecodeTime>'" + bakingstart.ToString() + "' and RecodeTime<'" + bakingendstr + "'";
                ds = equipDB.ExecuteDataSet(CommandType.Text, sql);
                table = ds.Tables[0];
                if (table.Rows.Count > 0)
                   vacu = table.Rows[0].ItemArray[0].ToString();
                LogInDB.Info(data[1]+"层平均温度："+temp+",平均真空度："+vacu);
                //检查目前电芯状态 
                sql = "select * from cellbarcode_" + name + " where LaminateNum = " + data[1] + " and RowNum = " + rownum1 + " limit 1";               
                ds = equipDB.ExecuteDataSet(CommandType.Text, sql);
                table = ds.Tables[0];
                Int16 states;
                String bakingnumstr;
                if (table.Rows.Count > 0)
                {
                    states = Int16.Parse(table.Rows[0].ItemArray[8].ToString());
                    bakingnumstr = table.Rows[0].ItemArray[8].ToString();//烘烤次数
                }
                else
                {
                    LogInDB.Info("LaminateNum =" + data[1] + ",RowNum =" + data[2]);
                    LogInDB.Info("查不到电芯数据");
                    return;
                }

                //LogInDB.Info("states=" + states);

                //修改cellbarcode数据表  填写下料时间
                sql = "update cellbarcode_" + name + " set CellIOState='Remove',Outtime='" + DateTime.Now.ToString() + "' where LaminateNum=" + data[1] + " and (RowNum=" + rownum1+ " or RowNum="+rownum2+")";

                //异常判断   判断是否出炉超时异常
                if ((states == 10 || states == 100) && chululength > chuluchaoshi)
                    sql = "update cellbarcode_" + name + " set CellIOState='Remove',CellBakingState=50,Outtime='" + DateTime.Now.ToString() + "' where LaminateNum=" + data[1] + " and  (RowNum=" + rownum1 + " or RowNum=" + rownum2 + ")";
               // LogInDB.Info("sql=" + sql.Replace('\'', ' '));
                equipDB.ExecuteNonQuery(CommandType.Text, sql);
                //如果有异常  修改carnumtopos 的小车状态
                if ((states == 10 || states == 100) && ts3.TotalMinutes > chuluchaoshi)
                {
                    car.CarState = 60;//异常状态  出炉超时
                    UpdateCarNumToPos(car);
                }

                //下料后修改carnumtopos的电芯数量
                sql = "select count(CellBarcodeID) from cellbarcode_" + name + " where CellIOState='Upload'";
               
                ds = equipDB.ExecuteDataSet(CommandType.Text, sql);
                table = ds.Tables[0];
                car.CellNum = int.Parse(table.Rows[0].ItemArray[0].ToString());
                if (car.CellNum == 0) car.CarState = 10;//设置空车状态
                                                        // LogInDB.Info("CellNum=" + car.CellNum);              
                UpdateCarNumToPos(car);
                LogInDB.Info(car.CarID+"号车电芯数量:"+car.CellNum);
                LogInDB.Info(car.CarID + "号车状态:" + car.CarState);
                //为mes准备上传资料
                List<ATL.MES.A013.ProductsItem> lst = new List<ATL.MES.A013.ProductsItem>();
                //反馈下料结果
              
                sql = "select * from cellbarcode_" + name + " where LaminateNum=" + data[1] + " and (RowNum=" + rownum1 + " or RowNum="+rownum2+")";
                LogInDB.Info("sql=" + sql.Replace('\'', ' '));
                ds = equipDB.ExecuteDataSet(CommandType.Text, sql);
                table = ds.Tables[0];
                int count = table.Rows.Count;
                LogInDB.Info("本次下料数量："+count);
                for (int i = 0; i < count; i++)
                {

                    // int col = int.Parse(table.Rows[i].ItemArray[7].ToString());
                    response[i] = table.Rows[i].ItemArray[8].ToString();

                    ATL.MES.A013.ProductsItem item = new ATL.MES.A013.ProductsItem();
                    item.ProductSN = table.Rows[i].ItemArray[1].ToString();
                    if (response[i].Equals("10"))
                        item.Pass = "OK";
                    else if (response[i].Equals("100"))
                    {
                        item.Pass = "OK";
                        A067(table.Rows[i].ItemArray[1].ToString());
                    }
                    else
                        item.Pass = "NG";
                    item.ChildEquCode = "";
                    item.Station = car.CarID + "-" + data[1] + "-" + oval;
                    item.OutputParam = new List<ATL.MES.A013.OutputParamItem>();

                    ATL.MES.A013.OutputParamItem parm1 = new ATL.MES.A013.OutputParamItem();
                    parm1.ParamID = "310181";
                    parm1.ParamDesc = "真空Baking时间";
                    parm1.ParamValue = bakinglength + "";
                    parm1.Result = "OK";
                    parm1.SpecParamID = "";
                    item.OutputParam.Add(parm1);

                    ATL.MES.A013.OutputParamItem parm2 = new ATL.MES.A013.OutputParamItem();
                    parm2.ParamID = "199";
                    parm2.ParamDesc = "温度";
                    parm2.ParamValue = temp;
                    parm2.Result = "OK";
                    parm2.SpecParamID = "";
                    item.OutputParam.Add(parm2);

                    ATL.MES.A013.OutputParamItem parm3 = new ATL.MES.A013.OutputParamItem();
                    parm3.ParamID = "85548";
                    parm3.ParamDesc = "抽真空时间";
                    parm3.ParamValue = "";
                    parm3.Result = "OK";
                    parm3.SpecParamID = "";
                    item.OutputParam.Add(parm3);

                    ATL.MES.A013.OutputParamItem parm8 = new ATL.MES.A013.OutputParamItem();
                    parm8.ParamID = "85553";
                    parm8.ParamDesc = "保压时间一";
                    parm8.ParamValue = "10";
                    parm8.Result = "OK";
                    parm8.SpecParamID = "";
                    item.OutputParam.Add(parm8);

                    ATL.MES.A013.OutputParamItem parm13 = new ATL.MES.A013.OutputParamItem();
                    parm13.ParamID = "85558";
                    parm13.ParamDesc = "真空度";
                    parm13.ParamValue = vacu;
                    parm13.Result = "OK";
                    parm13.SpecParamID = "";
                    item.OutputParam.Add(parm13);

                    ATL.MES.A013.OutputParamItem parm14 = new ATL.MES.A013.OutputParamItem();
                    parm14.ParamID = "85646";
                    parm14.ParamDesc = "开始时间";
                    parm14.ParamValue = bakingbeginstr;
                    parm14.Result = "OK";
                    parm14.SpecParamID = "";
                    item.OutputParam.Add(parm14);


                    ATL.MES.A013.OutputParamItem parm15 = new ATL.MES.A013.OutputParamItem();
                    parm15.ParamID = "85647";
                    parm15.ParamDesc = "结束时间";
                    parm15.ParamValue = bakingendstr;
                    parm15.Result = "OK";
                    parm15.SpecParamID = "";
                    item.OutputParam.Add(parm15);

                    lst.Add(item);
                    LogInDB.Info(item.ToJSON());
                }

                //发送给mes 下料验证
                A013(lst);
              
                LogInDB.Info("response=" + string.Join(",", response));
               

                UserDefineVariableInfo.DicVariables["plc_xialiao_response"] = string.Join(",", response);

               

                LogInDB.Info("下料完毕");
            }catch(Exception ex)
            {
                LogInDB.Info("下料异常:"+ex.Message);
                sendToDebug("下料异常:" + ex.Message);
            }
    }
    

        //搬动动作
        private void CheckMove(String var)
        {
            string msg = string.Empty;
            if(ATL.Core.PlcConfigurationInfo.lstPlcConfiguration[0].PLC != null)
            {
                object i = ATL.Core.PlcConfigurationInfo.lstPlcConfiguration[0].PLC.Read<short>("D0", ref msg);
                if(i != null)
                {
                    int j = (short)i;
                }

                short[] ii = (short[])ATL.Core.PlcConfigurationInfo.lstPlcConfiguration[0].PLC.Read<short[]>("D0", ref msg, 5);
                if(ii == null)
                {

                }

                string value;
                ATL.Core.PlcConfigurationInfo.lstPlcConfiguration[0].PLC.ReadByAddress("D0", "Float", out value, 4);

            }

            ATL.Engine.ID_Device ID_Device = ATL.Engine.ID_Device.lstDevices.Where(x => x.PlcConfig.PLC != null && x.PlcConfig.PLC.id == 0).FirstOrDefault();
            if(ID_Device != null)
            {
                bool i = (bool)ID_Device.PlcConfig.PLC.Read<bool>("D0", ref msg);
            }

            ATL.Engine.PLC plc = new ATL.Engine.PLC();
            plc.ReadByVariableName("test1");




            try
            {
                //  LogInDB.Info("开始搬运"+var);
                //1. 从plc变量中读取搬运数据
                Int16[] move_cur = StrsToInt16(var);
                if (move_cur == null) return;

                //2. 判断如果有没有搬运动作
                if (move_cur[0] == 0)
                {
                    move = null;
                    UserDefineVariableInfo.DicVariables["plc_move_response"] = 0;
                    return;
                }

                //如果数据重复  表示已经处理过了  跳过                       
                if (move != null && move.Equals(var)) return;
                move = var;//保存本次记录
                           //LogInDB.Info("move="+ move);
                           //5.判断搬运后位置是否异常  不是空位就为异常          
                bool isblank = true;
                getCarNumToPos();

                foreach (CarNumToPos tmp in carnumtopos)
                {
                    if (tmp.Pos == move_cur[2])
                    {
                        isblank = false;
                        break;
                    }
                }
                if (isblank == false)//如果异常
                {
                    LogInDB.Info("搬运后位置不为空位，搬运前" + move_cur[1] + ",搬运后" + move_cur[2]);
                    UserDefineVariableInfo.DicVariables["plc_move_response"] = 30;
                    return;
                }

                //6.判断搬运前位置是否异常 是空位就为异常
                int pos = -1;
                foreach (CarNumToPos tmp in carnumtopos)
                {
                    if (tmp.Pos == move_cur[1])
                    {
                        pos = tmp.Pos;
                        tmp.Pos = move_cur[2];
                        //上料
                        if (move_cur[2] == 8)
                        {
                            LogInDB.Info("搬运到上料位");
                            // int n = 5;
                            //while (n > 0)
                            //{
                            //    n--;
                            //    SendData();
                            //    Thread.Sleep(1000);
                            //    if (carcode == null || carcode.Equals(""))
                            //    {                            
                            //        continue;
                            //    }
                            //    else
                            //    {
                            //        //如果扫码信息不对 返回40
                            //        if (!carcode.Equals(tmp.CarCode))
                            //        {
                            //            LogInDB.Info("小车扫码不符，扫码="+carcode+",实际="+tmp.CarCode);
                            //            UserDefineVariableInfo.DicVariables["plc_move_response"] = 40;
                            //            return;
                            //        }
                            //        else
                            //        {
                            //            LogInDB.Info("小车扫码相符");
                            //            break;
                            //        }
                            //    }
                            //}
                            //if(n<=0)
                            //{
                            //    LogInDB.Info("搬运到上料位，没有扫到小车编码");

                            //}

                            //核对串口扫码信息
                            //while(carcode==null||carcode.Equals(""))
                            //{
                            //     Thread.Sleep(1000);
                            // }


                            //1.保存小车轨迹
                            CarProcess car = new CarProcess();
                            car.CarNum = tmp.CarID;
                            car.Pos = tmp.Pos;
                            car.State = "upload";
                            car.Time = DateTime.Now;
                            insertCarProcess(car);

                            //2.重置小车bakingtime时间
                            BakingTime bakingTime = new BakingTime();
                            bakingTime.CarNum = tmp.CarID;
                            UpdateBakingTime(bakingTime);

                            //3.将cellbarcode表数据移到historycellbarcode表中，清空cellbarcode
                            CellBarcodeToHistory(tmp.CarID);
                            //4.将carpara表数据移到historycarpara，清空carpara
                            CarParaToHistory(tmp.CarID);

                        }
                        //下料
                        if (move_cur[2] == 9)
                        {
                            LogInDB.Info("搬运到下料位");
                            //保存小车轨迹
                            CarProcess car = new CarProcess();
                            car.CarNum = tmp.CarID;
                            car.Pos = tmp.Pos;
                            car.State = "download";
                            car.Time = DateTime.Now;
                            insertCarProcess(car);
                        }
                        //进炉  
                        if (move_cur[2] < 8)
                        {
                            LogInDB.Info("搬运到" + move_cur[2] + "号炉，入炉");
                            //小车状态为待烘烤
                            if(tmp.CarState!=10)
                                tmp.CarState = 20;
                            //保存小车轨迹
                            CarProcess car = new CarProcess();
                            car.CarNum = tmp.CarID;
                            car.Pos = tmp.Pos;
                            car.State = "movein";
                            car.Time = DateTime.Now;
                            insertCarProcess(car);

                            //保存小车进炉时间
                            BakingTime bakingTime = new BakingTime();
                            bakingTime.CarNum = tmp.CarID;
                            bakingTime.moveintime = DateTime.Now.ToString();
                            UpdateBakingTime(bakingTime);
                        }
                        //出炉
                        if (move_cur[1] < 8)
                        {
                            LogInDB.Info("搬运" + move_cur[1] + "号炉，出炉");
                            //保存小车轨迹
                            CarProcess car = new CarProcess();
                            car.CarNum = tmp.CarID;
                            car.Pos = move_cur[1];
                            car.State = "moveout";
                            car.Time = DateTime.Now;
                            insertCarProcess(car);

                            //保存小车出炉时间
                            String sql = "update bakingtime set moveouttime='" + DateTime.Now.ToString() + "' where CarNum=" + tmp.CarID;
                            equipDB.ExecuteNonQuery(CommandType.Text, sql);


                        }

                        //保存小车位置
                        UpdateCarNumToPos(tmp);

                        break;
                    }
                }
                if (pos == -1)//搬运前位置是空位  异常
                {
                    LogInDB.Info("搬运前位置是空位，搬运前" + move_cur[1] + ",搬运后" + move_cur[2]);
                    UserDefineVariableInfo.DicVariables["plc_move_response"] = 20;
                    return;

                }
                //7. 反馈正常数据
                if (pos != -1 && isblank)
                    UserDefineVariableInfo.DicVariables["plc_move_response"] = 10;
            }catch(Exception ex)
            {
                LogInDB.Info("搬运异常："+ex.Message);
            }


            
        }
        //上料动作
        private bool CheckShangliao(int isok_cur, String[] code_cur, String info_cur)
        {
            try
            {
                //如果没有上料动作 重置上料反馈数据
                if (isok_cur == 0)
                {
                    UserDefineVariableInfo.DicVariables["plc_shangliao_response"] = 0;
                    shangliaocode = null;
                    
                    //  LogInDB.Info("isok_cur == 0");
                    return true;
                }
                //上料信息数据是否齐全
                if (info_cur == null) return true;

                String[] info = info_cur.Split(new char[] { ',' });
                Int16 count = Int16.Parse(info[0]);
                Int16 LaminateNum = Int16.Parse(info[1]);
                Int16 Num = Int16.Parse(info[2]);
                Int16 isok = Int16.Parse(info[3]);
                if (count == 0 || LaminateNum == 0 || Num == 0) return true;
                //上料二维码数据是否齐全

                for (int i = 0; i < count; i++)
                {
                    if (code_cur[i] == null || code_cur[i].Equals(""))
                    {
                      ///  LogInDB.Info((i+1) + "=null");
                        return false;
                    }

                }
                //判断是否与上次二维码记录重复
                if (shangliaocode != null && code_cur[0].Equals(shangliaocode[0]))
                {
                    return true;
                }
                shangliaocode = new string[10];
               // int n = 0;
                //保留当前数据到全局变量中
                for (int i = 0; i < count; i++)
                {
                    //if (code_cur[i] == null || code_cur[i].Equals(""))
                    //{
                    //    shangliaocode[i] = "";
                    //}else
                    //{
                     //   n++;
                        shangliaocode[i] = code_cur[i];
                   // }

                }
                //输出所有二维码
                String str = "";
                for (int i = 0; i < 10; i++)
                {
                    str += code_cur[i] + "  ";
                }
                LogInDB.Info("codes=" + str);

                //1.找出上料位的小车编号
                String name = null;
                int CarID = -1;
                getCarNumToPos();
                foreach (CarNumToPos car in carnumtopos)
                {
                    if (car.Pos == 8)//8号位置为上料位
                    {
                        name = car.CarID < 10 ? "0" + car.CarID : car.CarID + "";
                        CarID = car.CarID;
                        break;
                    }
                }
                if (name == null)
                {
                    LogInDB.Info("上料位置没有小车");
                    return true;//如果上料位置没有小车 返回
                }
                LogInDB.Info("上料位置小车" + CarID);

                //2.检查异常
                //找出目前小车所有的电芯二维码
                List<String> codelist = new List<string>();
                String sql = "select CellBarcode from cellbarcode_" + name;
                DataSet ds = equipDB.ExecuteDataSet(CommandType.Text, sql);
                DataTable table = ds.Tables[0];
                int c = table.Rows.Count;
                for (int i = 0; i < c; i++)
                {
                    codelist.Add(table.Rows[i].ItemArray[0].ToString());
                }

                //上料数量异常
                if ((count > 10 || count < 1))
                {
                    LogInDB.Info("plc上传数量异常：" + count);
                    LogInDB.Info("上料信息="+ info_cur);
                    LogInDB.Info("上料二维码=" + string.Join(",",code_cur));
                    UserDefineVariableInfo.DicVariables["plc_shangliao_response"] = 30;
                    return true;
                }

               
                //层数异常
                if (LaminateNum < 1 || LaminateNum > 32)
                {
                    LogInDB.Info("层数异常："+LaminateNum);
                    LogInDB.Info("上料信息=" + info_cur);
                    LogInDB.Info("上料二维码=" + string.Join(",", code_cur));
                    UserDefineVariableInfo.DicVariables["plc_shangliao_response"] = 40;
                    return true;
                }
                //次数异常
                if (Num < 1 || Num > 2)
                {
                    LogInDB.Info("次数异常：" + Num);
                    LogInDB.Info("上料信息=" + info_cur);
                    LogInDB.Info("上料二维码=" + string.Join(",", code_cur));
                    UserDefineVariableInfo.DicVariables["plc_shangliao_response"] = 50;
                    return true;
                }
                for (int i = 0; i < count; i++)
                {
                    String code = code_cur[i];
                    if (code.Equals(""))
                    {
                        LogInDB.Info($"[{i}]电池条码异常");
                        continue;
                    }
                    
                    //检查二维码是否重复
                    bool isrepeat = false;

                    foreach (String tmp in codelist)
                    {
                        
                        if (code.Equals(tmp))
                        {
                            LogInDB.Info("重复二维码：" + tmp);
                            LogInDB.Info("上料信息=" + info_cur);
                            LogInDB.Info("上料二维码=" + string.Join(",", code_cur));
                            isrepeat = true;
                            break;
                        }
                    }

                    if (isrepeat)
                    {
                        UserDefineVariableInfo.DicVariables["plc_shangliao_response"] = 20;
                        return true;
                    }
                    else
                    {
                        codelist.Add(code);
                    }
                }

                 for (int i = 0; i < count; i++)
                {
                    String code = code_cur[i];
                    if (code.Equals("")) continue;
                    //if (code == null || code.Trim().Equals("")) break;
                    
                    int row = -1;
                    int col = -1;
                    int halfcount = 5;
                    if (Num == 1)
                    {
                        if (i + 1 <= halfcount)
                        { row = 1; col = (i + 1); }
                        else
                        { row = 2; col = (i + 1 - halfcount); }
                    }
                    else
                    {
                        if (i + 1 <= halfcount)
                        { row = 3; col = (i + 1); }
                        else
                        { row = 4; col = (i + 1 - halfcount); }

                    }
                    //3.在cellbarcode中添加一条记录   
                    sql = "insert into cellbarcode_" + name + "(CellBarcode,CellIOState,Intime,LaminateNum,RowNum,PosNum,CellBakingState,bakingNum) values('" + code + "','Upload','" + DateTime.Now.ToString() + "'," + LaminateNum + "," + row + "," + col + ",90,0)";
                    //  LogInDB.Info("sql=" + sql.Replace('\'',' '));
                    int res = equipDB.ExecuteNonQuery(CommandType.Text, sql);
                    // LogInDB.Info("res="+res);
                }
                //4.修改carnumtopos中电芯的数量 小车状态改为20（待烘烤）
              //  if (isok == 10)
               // {
                    sql = "update carnumtopos set CellNum=" + codelist.Count + ",CarState=20 where CarID=" + CarID;
                    equipDB.ExecuteNonQuery(CommandType.Text, sql);
                    //  LogInDB.Info("sql=" + sql.Replace('\'', ' '));
                    //  LogInDB.Info("res=" + res);
              //  }
              
                //5.上料反馈正常
                UserDefineVariableInfo.DicVariables["plc_shangliao_response"] = 10;
                LogInDB.Info("上料完毕");
                return true;
            }catch(Exception ex)
            {
                LogInDB.Info("上料异常："+ex.Message);
                //sendToDebug("上料异常：" + ex.Message);
                return false;
            }

    }
        //出满车和空车调度
        private void CheckMoveOut(String var)
        {
            try
            {
                //1. 从plc变量中读取出炉调度验证数据
                Int16 moveok = Int16.Parse(var);
                //2.没有出炉调度动作，则重置炉号和反馈
                if (moveok == 0)
                {
                    UserDefineVariableInfo.DicVariables["plc_out_response"] = "0";
                    UserDefineVariableInfo.DicVariables["plc_out_response_ok"] = "0";
                    return;
                }

                //3.如果出满车
                if (moveok == 10)
                {
                    String sql = "select * from carnumtopos where CarState=40";
                    DataSet ds = equipDB.ExecuteDataSet(CommandType.Text, sql);
                    DataTable table = ds.Tables[0];
                    int count = table.Rows.Count;
                    if (count == 0)
                    {
                        LogInDB.Info("目前没有烘烤完成的炉子");
                        UserDefineVariableInfo.DicVariables["plc_out_response"] = "0";
                        UserDefineVariableInfo.DicVariables["plc_out_response_ok"] = "10";
                        return;

                    }

                    if (count == 1)
                    {
                        int oval = int.Parse(table.Rows[0].ItemArray[2].ToString());
                        if (oval >= 1 && oval <= 7)
                        {
                            LogInDB.Info("待出炉炉号=" + oval);
                            UserDefineVariableInfo.DicVariables["plc_out_response"] = oval;
                            UserDefineVariableInfo.DicVariables["plc_out_response_ok"] = "10";
                            return;
                        }
                        else
                        {
                            LogInDB.Info("目前没有烘烤完成的炉子");
                            UserDefineVariableInfo.DicVariables["plc_out_response"] = "0";
                            UserDefineVariableInfo.DicVariables["plc_out_response_ok"] = "10";
                            return;
                        }
                    }
                    else
                    {
                        DateTime now = DateTime.Now;
                        int outoval = -1;

                        //在bakingtime中找到最早烘烤号的炉号
                        for (int i = 0; i < table.Rows.Count; i++)
                        {
                            int oval = int.Parse(table.Rows[i].ItemArray[2].ToString());
                            if (oval >= 1 && oval <= 7)
                            {
                                LogInDB.Info("满车待出炉=" + oval);
                                String carnum = table.Rows[i].ItemArray[0].ToString();
                                sql = "select bakingend from bakingtime where CarNum=" + carnum;
                                DataSet ds1 = equipDB.ExecuteDataSet(CommandType.Text, sql);
                                DataTable table1 = ds1.Tables[0];
                                if (table1.Rows.Count == 0) continue;
                                if (table1.Rows[0].ItemArray[0].ToString().Equals(""))
                                    continue;
                                DateTime time = DateTime.Parse(table1.Rows[0].ItemArray[0].ToString());
                                if (time.CompareTo(now) < 0)
                                {
                                    now = time;
                                    outoval = oval;
                                }
                            }

                        }

                        if (outoval != -1)
                        {
                            LogInDB.Info("待出炉炉号=" + outoval);
                            UserDefineVariableInfo.DicVariables["plc_out_response"] = outoval;
                            UserDefineVariableInfo.DicVariables["plc_out_response_ok"] = "10";
                            return;
                        }
                        else
                        {
                            LogInDB.Info("目前没有烘烤完成的炉子");
                            UserDefineVariableInfo.DicVariables["plc_out_response"] = "0";
                            UserDefineVariableInfo.DicVariables["plc_out_response_ok"] = "10";
                            return;
                        }
                    }
                }
                //出空车
                if (moveok == 20)
                {
                    String sql = "select * from carnumtopos where CarState=10";
                    DataSet ds = equipDB.ExecuteDataSet(CommandType.Text, sql);
                    DataTable table = ds.Tables[0];
                    int count = table.Rows.Count;
                    //  LogInDB.Info("空车个数=" + count);
                    if (count != 0)
                    {
                        for (int i = 0; i < count; i++)
                        {
                            int oval = int.Parse(table.Rows[i].ItemArray[2].ToString());
                            //LogInDB.Info("空车炉号=" + oval);
                            if (oval >= 1 && oval <= 7)
                            {
                                LogInDB.Info("空车炉号=" + oval);
                                UserDefineVariableInfo.DicVariables["plc_out_response"] = oval + "";
                                UserDefineVariableInfo.DicVariables["plc_out_response_ok"] = "10";
                                return;
                            }
                        }
                    }

                    LogInDB.Info("目前没有存放空车的炉子");
                    UserDefineVariableInfo.DicVariables["plc_out_response"] = "0";
                    UserDefineVariableInfo.DicVariables["plc_out_response_ok"] = "10";
                    return;
                }
            }
            catch (Exception ex)
            {
                LogInDB.Info("出炉调度异常："+ex.Message);
                
            }
            


        }
        public static void sendToDebug(String str)
        {
            try
            {
                //  richtext.Invoke(d1, str);
                //ETHAN.Extensions.Log(str);
            }
            catch (Exception ex)
            {

            }
        }


        //加热状态
        private void CheckStatus(String var)
        {
            try { 
            //转化为整形类型
            Int16[] status_cur = StrsToInt16(var);
             
            if (status_cur == null) return;
           
          
            //判断是否与上次数据相同
            if (status != null && status.Equals(var)) return;
            //保存当前数据
            status = var;
            LogInDB.Info("status="+var);
            //处理7个炉子的状态

            Int16[] status_response= new Int16[status_cur.Length];
            getCarNumToPos();
            for (int i=0;i<status_cur.Length;i++)
            {
                //找出本位置的小车数据
                CarNumToPos car = null;
                foreach(CarNumToPos tmp in carnumtopos)
                    if(tmp.Pos==(i+1))
                    {
                        car = tmp;
                        break;
                    }

                if (car == null)
                {
                    LogInDB.Info((i+1)+"号炉体没有小车");
                    continue;
                }
                String name = car.CarID < 10 ? "0" + car.CarID : "" + car.CarID;
                //开始加热 改为烘烤状态
                if (status_cur[i]==10)
                {
                    //数据记录正常
                    status_response[i] = 20;
                    if (car.CarState == 30) continue;
                    LogInDB.Info((i + 1) + "号炉子开始加热");
                    sendToDebug((i + 1) + "号炉子开始加热," + DateTime.Now);
                    //1.更新carnumtopos表
                    car.CarState = 30;//正在烘烤
                    UpdateCarNumToPos(car);
                  // LogInDB.Info("here1");
                    //2.更新bakingtime表 记录开始时间  将结束时间置为null
                    String sql = "update bakingtime set bakingbegin='" + DateTime.Now.ToString() + "',bakingend=null where CarNum=" + car.CarID;
                 //   LogInDB.Info("sql="+sql.Replace('\'',' '));
                    equipDB.ExecuteNonQuery(CommandType.Text, sql);
                  //  LogInDB.Info("here2");
                     //3.保存小车轨迹
                     CarProcess car1 = new CarProcess();
                    car1.CarNum = car.CarID;
                    car1.Pos = i+1;
                    car1.State = "bakingbegin";
                    car1.Time = DateTime.Now;
                    insertCarProcess(car1);
                   
                     //4.修改cellbarcode_表的bakingNum为第一次烘烤
                    sql = "update cellbarcode_" + name + " set bakingNum=1,CellBakingState=10";
                    equipDB.ExecuteNonQuery(CommandType.Text, sql);
                   // LogInDB.Info("here2");
                }

                //继续加热 改为烘烤状态
                if (status_cur[i] == 20)
                {
                    //数据记录正常
                    status_response[i] = 20;
                    if (car.CarState == 30) continue;
                    LogInDB.Info((i + 1) + "号炉子继续加热");
                    sendToDebug((i + 1) + "号炉子继续加热," + DateTime.Now);
                    //1.更新carnumtopos表                  
                    car.CarState = 30;//正在烘烤
                    UpdateCarNumToPos(car);
                    //2.更新bakingtime表 取消之前的结束时间 不修改开始时间
                    //   String sql = "update bakingtime set bakingbegin='" + DateTime.Now.ToString() + "' where CarNum=" + car.CarID;
                    String sql = "update bakingtime set bakingend=null where CarNum=" + car.CarID;
                    equipDB.ExecuteNonQuery(CommandType.Text, sql);
                    //3.保存小车轨迹
                    CarProcess car1 = new CarProcess();
                    car1.CarNum = car.CarID;
                    car1.Pos = i + 1;
                    car1.State = "bakingcontinue";//继续加热
                    car1.Time = DateTime.Now;
                    insertCarProcess(car1);
                  //  LogInDB.Info(i + "=20");

                    //4.修改CellBarcode中的烘烤次数 bakingnum       
                    sql = "select * from cellbarcode_" + name+ " limit 1";
                    DataSet ds = equipDB.ExecuteDataSet(CommandType.Text, sql);
                    DataTable table = ds.Tables[0];
                    int num =int.Parse(table.Rows[0].ItemArray[9].ToString())+1;
                                     
                    sql = "update cellbarcode_" + name + " set bakingNum=" + num;
                    //5.异常判断  加热次数
                    if(num>3)
                    {
                        int states = int.Parse(table.Rows[0].ItemArray[8].ToString());
                        if(states==10||states==100)//如果是正常或抽样 则改为烘烤次数异常
                            sql = "update cellbarcode_" + name + " set bakingNum=" + num+ ",CellBakingState=70";
                    }
                    equipDB.ExecuteNonQuery(CommandType.Text, sql);
                }

                //加热故障 改为异常状态
                if (status_cur[i] == 30 )
                {
                    //数据记录暂停
                    status_response[i] = 10;
                        //如果是空车
                        if (car.CellNum == 0)
                        {
                            car.CarState = 10;
                            UpdateCarNumToPos(car);
                            continue;
                        }
                        if (car.CarState == 60) continue;
                    LogInDB.Info((i + 1) + "号炉子加热故障");
                    sendToDebug((i + 1) + "号炉子加热故障," + DateTime.Now);
                    //1.更新carnumtopos表
                    car.CarState = 60;
                    UpdateCarNumToPos(car);
                    //2.更新bakingtime表 记录结束时间位故障时间
                    String sql = "update bakingtime set bakingend='" + DateTime.Now.ToString() + "' where CarNum=" + car.CarID;
                    equipDB.ExecuteNonQuery(CommandType.Text, sql);
                    //3.保存小车轨迹
                    CarProcess car1 = new CarProcess();
                    car1.CarNum = car.CarID;
                    car1.Pos = i + 1;
                    car1.State = "bakingbreakdown";//加热故障
                    car1.Time = DateTime.Now;
                    insertCarProcess(car1);
                   // LogInDB.Info(i + "=30");
                }

                //加热终止  改为待烘烤状态
                if (status_cur[i] == 40)
                {
                    //数据记录暂停
                    status_response[i] = 10;
                        //如果是空车
                        if (car.CellNum == 0)
                        {
                            car.CarState = 10;
                            UpdateCarNumToPos(car);
                            continue;
                        }
                        if (car.CarState == 60) continue;
                    LogInDB.Info((i + 1) + "号炉子加热终止");
                    sendToDebug((i + 1) + "号炉子加热终止," + DateTime.Now);
                    //1.更新carnumtopos表
                    car.CarState = 60;
                    UpdateCarNumToPos(car);
                    //2.更新bakingtime表 记录结束时间（有异议  之前的故障已经记录过结束时间了）
                   // String sql = "update bakingtime set bakingend='" + DateTime.Now.ToString() + "' where CarNum=" + car.CarID;
                   // equipDB.ExecuteNonQuery(CommandType.Text, sql);
                    //3.保存小车轨迹
                    CarProcess car1 = new CarProcess();
                    car1.CarNum = car.CarID;
                    car1.Pos = i + 1;
                    car1.State = "bakingstop";//加热终止
                    car1.Time = DateTime.Now;
                    insertCarProcess(car1);
                    //LogInDB.Info(i + "=40");
                }

                //加热完成  改为烘烤完成状态
                if (status_cur[i] == 50 )
                {
                    //30-数据记录异常/40-真空NG/50-烘烤时长不够NG/60-整夹具温度NG/70-烘烤OK
                    status_response[i] = 70;
                        //如果是空车
                        if (car.CellNum == 0)
                        {
                            car.CarState = 10;
                            UpdateCarNumToPos(car);
                            continue;
                        }
                
                    if (car.CarState == 40) continue;
                    LogInDB.Info((i + 1) + "号炉子加热完成");
                    sendToDebug((i + 1) + "号炉子加热完成,"+DateTime.Now);
                    //1.更新carnumtopos表
                    car.CarState = 40;//烘烤完成
                    UpdateCarNumToPos(car);
                    //2.更新bakingtime表
                    String sql = "update bakingtime set bakingend='" + DateTime.Now.ToString() + "' where CarNum=" + car.CarID;
                    equipDB.ExecuteNonQuery(CommandType.Text, sql);
                    //3.保存小车轨迹
                    CarProcess car1 = new CarProcess();
                    car1.CarNum = car.CarID;
                    car1.Pos = i + 1;
                    car1.State = "bakingend";//加热故障
                    car1.Time = DateTime.Now;
                    insertCarProcess(car1);
                     // LogInDB.Info(i + "=50");
                     //4.异常判断     

                    //判断加热时间不够长 异常情况
                   
                    bool isok=CheckBakingTime(car);
                    if (!isok)
                    {
                        //反馈给plc  50表示烘烤时间不够长
                        status_response[i] = 50;

                        //修改CellBarcode_表电芯状态为60 如果其状态不为异常的话
                        sql = "select CellBakingState from cellbarcode_" + name + " limit 1";
                        DataSet ds = equipDB.ExecuteDataSet(CommandType.Text, sql);
                        DataTable table = ds.Tables[0];
                        if (table.Rows.Count != 0)
                        {
                            int states = int.Parse(table.Rows[0].ItemArray[0].ToString());
                            if (states == 10 || states == 100)//目前没有其他异常
                            {
                                sql = "update cellbarcode_" + name + " set CellBakingState=60";
                                equipDB.ExecuteNonQuery(CommandType.Text, sql);
                            }
                        }
                        //修改carnumtopos 小车状态为异常60
                        car.CarState = 60;
                        UpdateCarNumToPos(car);
                    }
                }

                //待机状态
                if (status_cur[i] == 60)
                {
                    //80-无工作状态
                    status_response[i] = 80;
                    LogInDB.Info(i + "=60");

                }

            }
            LogInDB.Info("status response="+string.Join(",", status_response));
            //加热状态反馈
            UserDefineVariableInfo.DicVariables["plc_status_response"] = string.Join(",", status_response);
            }catch(Exception ex)
            {
                LogInDB.Info("加热状态异常："+ex.Message);
            }


    }
        //实时检查真空度异常  每隔3分钟
        private void CheckException(short[] status_cur)
        {
            try
            {
                getCarNumToPos();
                //检查加热的炉子  不加热的不检查
                for (int i = 0; i < status_cur.Length; i++)
                {
                    //加热或继续加热
                    if (status_cur[i] == 10 || status_cur[i] == 20)
                    {
                        //找出本位置的小车数据
                        CarNumToPos car = null;
                        foreach (CarNumToPos tmp in carnumtopos)
                            if (tmp.Pos == (i + 1))
                            {
                                car = tmp;
                                break;
                            }
                        if (car == null) continue;

                        List<DateTime> bakinglist = getBakinglist(car);
                        CheckVaccum(car, bakinglist);
                        CheckTemp(car, bakinglist);
                    }
                }
            }catch(Exception ex)
            {
                LogInDB.Info("温度真空度检查异常:"+ex.Message);
            }
        }

        //温度和真空度
        private void CheckTempAndVacus(short[] temps, float[] vacus)
        {
            try { 
            getCarNumToPos();
            for (int i = 0; i < 7; i++)
            {
                //找出本位置的小车数据
                CarNumToPos car = null;
                foreach (CarNumToPos tmp in carnumtopos)
                    if (tmp.Pos == (i + 1))
                    {
                        car = tmp;
                        break;
                    }
                //如果此位置没有小车 则跳过
                if (car == null) continue;
                //如果此位置小车不是正在烘烤 则跳过
                if (car.CarState != 30) continue;
                //从Plc变量获得数据
                float[] temp = new float[ceng];
                    // LogInDB.Info("i="+i);
               // LogInDB.Info((i + 1) + "号炉温度：");
                for (int j = 0; j < ceng; j++)
                {
                    temp[j] = temps[j + i * 32]/10;
                   // LogInDB.Info("D"+(j + i * 32) +"="+temp[j]+",");
                }
                   
                
               //  LogInDB.Info(string.Join(",", temp));
                   
                 float vacu = vacus[i];
                //LogInDB.Info(string.Join(",", temp));
                //添加carpara_表数据，记录温度和真空度
                String name = car.CarID < 10?"0"+car.CarID:car.CarID + "";
                String sql = "insert into carpara_" + name + "(";
                for (int j = 1; j <= ceng; j++)
                    sql += "Temp" + j + ",";
                sql += "Vacuum,RecodeTime,FurnaceNum) values(";
                for (int j = 0; j < ceng; j++)
                    sql += temp[j] + ",";

                sql += vacu + ",'"
                    + DateTime.Now.ToString() + "',"
                    + (i+1) + ")";
                equipDB.ExecuteNonQuery(CommandType.Text, sql);
                }
            }catch(Exception ex)
            {
                LogInDB.Info(ex.Message);
                sendToDebug(ex.Message);
            }
        }


        //将字符串转化为Int16数组
        private Int16[]  StrsToInt16(String var)
        {
            String[] strs = var.Split(new char[] { ',' });
            Int16[] data = new Int16[strs.Length];
            for (int i = 0; i < strs.Length; i++)
            {

                try
                {
                    data[i] = Int16.Parse(strs[i]);
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
            return data;
        }

        //将字符串转化为Float数组
        private float[] StrsToFloat(String var)
        {
            String[] strs = var.Split(new char[] { ',' });
            float[] data = new float[strs.Length];
            for (int i = 0; i < strs.Length; i++)
            {

                try
                {
                    data[i] = float.Parse(strs[i]);
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
            return data;
        }
        #endregion

        #region  数据表操作

        public DataTable getAllCarNumToPos()
        {
            String sql = "select * from carnumtopos";
            DataSet ds = equipDB.ExecuteDataSet(CommandType.Text, sql);
            DataTable table = ds.Tables[0];
            return table;
        }
       public class CarNumToPos
        {
            public int CarID;
            public String CarCode;
            public int Pos;
            public int CellNum;
            public int CarState;
        }

        public int UpdateCarNumToPos(CarNumToPos data)
        {
            String sql = "update carnumtopos set Pos="+data.Pos+ ",CellNum="+data.CellNum+ ",CarState="+data.CarState
                +" where CarID="+data.CarID;
            
            int res = equipDB.ExecuteNonQuery(CommandType.Text, sql);
            getCarNumToPos();//重新拿一下数据
            return res;

        }
        private void setCarNumToPos()
        {
            try
            {
                for (int i = 1; i <= 10; i++)
                {
                    String name = i < 10 ? "0" + i : "" + i;
                    String sql = "select count(CellBarcode) from cellbarcode_" + name + " where CellIOState='Upload'";
                  //  LogInDB.Info(sql.Replace('\'',' '));
                    DataSet ds = equipDB.ExecuteDataSet(CommandType.Text, sql);
                    DataTable table = ds.Tables[0];
                    int count = int.Parse(table.Rows[0].ItemArray[0].ToString());
                   // LogInDB.Info("count="+count);
                    if (count == 0)
                    {
                        sql = "select CarState from carnumtopos where CarID=" + i;
                        ds = equipDB.ExecuteDataSet(CommandType.Text, sql);
                        table = ds.Tables[0];
                        String state = table.Rows[0].ItemArray[0].ToString();
                        if (state.Equals("30"))
                            sql = "update carnumtopos set CellNum=0 where CarID=" + i;
                        else
                            sql = "update carnumtopos set CellNum=0,CarState=10 where CarID=" + i;
                        equipDB.ExecuteNonQuery(CommandType.Text, sql);
                    }
                    else
                    {
                        sql = "update carnumtopos set CellNum=" + count + " where CarID=" + i;
                        equipDB.ExecuteNonQuery(CommandType.Text, sql);
                    }

                }
            }catch(Exception ex)
            {
                LogInDB.Info("setnumtopos err:"+ex.Message);
            }
        }

        public List<CarNumToPos> getCarNumToPos()
        {
            carnumtopos.Clear();
            String sql = "select * from carnumtopos order by CarID";
            DataSet ds = equipDB.ExecuteDataSet(CommandType.Text, sql);
            DataTable table = ds.Tables[0];
            int count = table.Rows.Count;
            for (int i = 0; i < count; i++)
            {
                CarNumToPos d = new CarNumToPos();
                d.CarID = int.Parse(table.Rows[i].ItemArray[0].ToString());
                d.CarCode = table.Rows[i].ItemArray[1].ToString();            
                d.Pos = int.Parse(table.Rows[i].ItemArray[2].ToString());
                d.CellNum = int.Parse(table.Rows[i].ItemArray[3].ToString());
                d.CarState = int.Parse(table.Rows[i].ItemArray[4].ToString());
               
                carnumtopos.Add(d);
            }
            


            return carnumtopos;

        }

        public List<CellBarCode> getCellBarCode(int n)
        {
            if (n < 1 || n > 10) return null;
            cellbarcode.Clear();

            String name = n < 10 ? "0" + n : 10 + "";
            String sql = "select * from cellbarcode_"+name;
            DataSet ds = equipDB.ExecuteDataSet(CommandType.Text, sql);
            DataTable table = ds.Tables[0];
            int count = table.Rows.Count;
            for (int i = 0; i < count; i++)
            {
                CellBarCode d = new CellBarCode();
                d.CellBarcodeID = int.Parse(table.Rows[i].ItemArray[0].ToString());
                d.CellBarcode = table.Rows[i].ItemArray[1].ToString();
                d.CellIOState = table.Rows[i].ItemArray[2].ToString();
                d.intime = table.Rows[i].ItemArray[3].ToString();
                d.Outtime = table.Rows[i].ItemArray[4].ToString();
                d.LaminateNum = int.Parse(table.Rows[i].ItemArray[5].ToString());
                d.RowNum = int.Parse(table.Rows[i].ItemArray[6].ToString());
                d.PosNum = int.Parse(table.Rows[i].ItemArray[7].ToString());
                d.CellBakingState = int.Parse(table.Rows[i].ItemArray[8].ToString());
                d.bakingnum = int.Parse(table.Rows[i].ItemArray[9].ToString());

                cellbarcode.Add(d);
            }

            return cellbarcode;
        }

        public int UpdateCellBarCode(int n,CellBarCode code)
        {
            String name = n < 10 ? "0" + n : 10 + "";
            String sql = "update cellbarcode_"+name+ " set CellIOState='"+code.CellIOState+ "' , CellBakingState="+code.CellBakingState+ " , bakingNum="+code.bakingnum+ " where CellBarcodeID=" + code.CellBarcodeID;
            int res=equipDB.ExecuteNonQuery(CommandType.Text, sql);
            return res;
        }

        public class CellBarCode
        {
            public int CellBarcodeID;
            public String CellBarcode;
            public String CellIOState;
            public String intime;
            public String Outtime;
            public int LaminateNum;
            public int RowNum;
            public int PosNum;
            public int CellBakingState;
            public int bakingnum;
        }
       
        public int insertCarProcess(CarProcess car)
        {
            String sql = "insert into carprocess(CarNum,Pos,State,Time) values("+car.CarNum+","+
                car.Pos+",'"+car.State+"','"+car.Time.ToString()+"')";
            int res = equipDB.ExecuteNonQuery(CommandType.Text, sql);
            return res;
        }
        public class CarProcess
        {
            public int CarProcessID;
            public int CarNum;
            public int Pos;
            public String State;
            public DateTime Time;
        }

        public int UpdateBakingTime(BakingTime bakingTime)
        {
            String movein = bakingTime.moveintime == null ? "null" :"'"+ bakingTime.moveintime+"'";
            String moveout= bakingTime.moveouttime == null ? "null" : "'" + bakingTime.moveouttime + "'";
            String bakingbegin = bakingTime.bakingbegin == null ? "null" : "'" + bakingTime.bakingbegin + "'";
            String bakingend = bakingTime.bakingend == null ? "null" : "'" + bakingTime.bakingend + "'";
            String sql = "update bakingtime set moveintime=" + movein
                + ",moveouttime=" + moveout + ",bakingbegin=" + bakingbegin + ",bakingend=" + bakingend + " where CarNum=" + bakingTime.CarNum;
            //LogInDB.Info("sql=" + sql);
            int res = equipDB.ExecuteNonQuery(CommandType.Text, sql);
           
            return res;
        }

        public List<BakingTime> getBakingTimes()
        {
            bakingTimes.Clear();
            String sql = "select * from bakingtime";
            DataSet ds = equipDB.ExecuteDataSet(CommandType.Text, sql);
            DataTable table = ds.Tables[0];
            int count = table.Rows.Count;
            for (int i = 0; i < count; i++)
            {
                BakingTime d = new BakingTime();
                d.BakingTimeID= int.Parse(table.Rows[i].ItemArray[0].ToString());
                d.CarNum= int.Parse(table.Rows[i].ItemArray[1].ToString());
                d.moveintime =table.Rows[i].ItemArray[2].ToString();
                d.moveouttime = table.Rows[i].ItemArray[3].ToString();
                d.bakingbegin= table.Rows[i].ItemArray[4].ToString();
                d.bakingend = table.Rows[i].ItemArray[5].ToString();
                bakingTimes.Add(d);

            }
            return bakingTimes;
        }

        public class BakingTime
        {
            public int BakingTimeID;
            public int CarNum;
            public String moveintime;
            public String moveouttime;
            public String bakingbegin;
            public String bakingend;

        }
        //将电芯数据移到历史记录 并清空数据
        public void CellBarcodeToHistory(int carnum)
        {
            try
            {
                String name = carnum < 10 ? "0" + carnum : carnum + "";
                String sql = "select * from cellbarcode_" + name;
                DataSet ds = equipDB.ExecuteDataSet(CommandType.Text, sql);
                DataTable table = ds.Tables[0];
                int count = table.Rows.Count;
                for (int i = 0; i < count; i++)
                {
                    LogInDB.Info("intime=" + table.Rows[i].ItemArray[3].ToString());
                    String intime = table.Rows[i].ItemArray[3].ToString().Equals("") ? "null" : "'" + table.Rows[i].ItemArray[3].ToString() + "'";
                    String outtime = table.Rows[i].ItemArray[4].ToString().Equals("") ? "null" : "'" + table.Rows[i].ItemArray[4].ToString() + "'";

                    sql = "insert into historycellbarcode(CarNum,CellBarcode,CellIOState,Intime,Outtime,LaminateNum,RowNum,PosNum,CellBakingState,bakingNum)" +
                        " values(" + carnum + ",'" + table.Rows[i].ItemArray[1].ToString() + "','"
                        + table.Rows[i].ItemArray[2].ToString() + "',"
                        + intime + ","
                        + outtime + ","
                        + table.Rows[i].ItemArray[5].ToString() + ","
                        + table.Rows[i].ItemArray[6].ToString() + ","
                        + table.Rows[i].ItemArray[7].ToString() + ","
                        + table.Rows[i].ItemArray[8].ToString() + ","
                        + table.Rows[i].ItemArray[9].ToString() + ")";
                    LogInDB.Info("sql=" + sql.Replace('\'', ' '));

                    int res = equipDB.ExecuteNonQuery(CommandType.Text, sql);

                    LogInDB.Info("res=" + res);
                }
                //清空cellbarcode表
                sql = "truncate table cellbarcode_" + name;
                equipDB.ExecuteNonQuery(CommandType.Text, sql);
            }catch(Exception ex)
            {
                LogInDB.Info(ex.Message);
            }
        }

        public class CellBarcode
        {
          
            public Int16 CellBarcodeID;
            public Int16 CarNum;
            public String code;
            public String CellIOState;
            public String Intime;
            public String Outtime;
            public Int16 LaminateNum;
            public Int16 RowNum;
            public Int16 PosNum;
            public Int16 CellBakingState;
            public Int16 bakingNum;

        }

        //将温度数据移到历史记录 并清空数据
        public void CarParaToHistory(int carnum)
        {
            try
            {
                String name = carnum < 10 ? "0" + carnum : carnum + "";
                String sql = "select * from carpara_" + name;
                LogInDB.Info("sql="+sql.Replace('\'',' '));
                DataSet ds = equipDB.ExecuteDataSet(CommandType.Text, sql);
                DataTable table = ds.Tables[0];
                int count = table.Rows.Count;
                LogInDB.Info("count=" + count);

                for (int i = 0; i < count; i++)
                {
                    sql = "insert into historycarpara_" + name + "(";

                    for (int j = 1; j <= 30; j++)
                        sql += "Temp" + j + ",";

                    sql += "Vacuum,RecodeTime,FurnaceNum) values(";

                    for (int j = 1; j <= 30; j++)
                        sql += table.Rows[i].ItemArray[j].ToString() + ",";

                    sql += table.Rows[i].ItemArray[33].ToString() + ",'"
                        + table.Rows[i].ItemArray[34].ToString() + "',"
                        + table.Rows[i].ItemArray[35].ToString() + ")";

                    if (i == 0)
                        LogInDB.Info("sql=" + sql.Replace('\'', ' '));
                    equipDB.ExecuteNonQuery(CommandType.Text, sql);

                }

                //清空cellbarcode表
                sql = "truncate table carpara_" + name;
                equipDB.ExecuteNonQuery(CommandType.Text, sql);
            }catch(Exception ex)
            {
                LogInDB.Info("CarParaToHistory异常"+ex.Message);
            }

        }

        public class CarPara
        {
            public Int16 CarParaID;
            public float[] Temp = new float[30];
            public float Vacuum;
            public String RecordTime;
            public Int16 furnaceNum;
        }
        #endregion

        #region 曲线参数
        public Params GetParams()
        {
            if (p == null)
                p = new Params();
            return p;
        }
        public class Params
        {
            //温度曲线参数
            public int wendu1=20;
            public int wendu2=105;
            public int wendu_time1=2;
            public int wendu_time2=20;
            //真空度曲线参数
            public int kpa1 = 5;//真3ergG空度上下界
            public int kpa2 = 15;
            public int[] time1 = new int[4];
            public int[] time2 = new int[5];

            public Params()
            {
               
                wendu1 =int.Parse(UserDefineVariableInfo.DicVariables["wendu1"].ToString());
                wendu2 = int.Parse(UserDefineVariableInfo.DicVariables["wendu2"].ToString());
                wendu_time1 = int.Parse(UserDefineVariableInfo.DicVariables["wendu_time1"].ToString());
                wendu_time2 = int.Parse(UserDefineVariableInfo.DicVariables["wendu_time2"].ToString());

                kpa1 = int.Parse(UserDefineVariableInfo.DicVariables["Kpa1"].ToString()); 
                kpa2 = int.Parse(UserDefineVariableInfo.DicVariables["Kpa2"].ToString());
                //九次呼吸的时间
                String[] list = UserDefineVariableInfo.DicVariables["time1"].ToString().Split(new char[] { ','});
                time2[0] = int.Parse(list[0]);
                time2[1] = int.Parse(list[1]);
                time2[2] = int.Parse(list[2]);
                time2[3] = int.Parse(list[3]);
                time2[4] = int.Parse(list[4]);

                list = UserDefineVariableInfo.DicVariables["time2"].ToString().Split(new char[] { ',' });
                time1[0] = int.Parse(list[0]); 
                time1[1] = int.Parse(list[1]); 
                time1[2] = int.Parse(list[2]); 
                time1[3] = int.Parse(list[3]);

            }
        }

       public class TempNode
        {
            public float[] temp = new float[30];//30层温度
            public float Vacu;//真空度         
            public DateTime time;//时间  日期格式
            public DateTime begintime;//烘烤开始的时间
            public double disminutes;//距离上一个时间段的分钟数
            public double totalminiutes;//实际所在的分钟数
        }
        //获取温度和真空度
        public List<TempNode> FindTempNode(CarNumToPos car)
        {
             List<DateTime> bakinglist = getBakinglist(car);
            if (bakinglist == null) return null;
            //转化为timeslot
            if (bakinglist.Count % 2 != 0)
                bakinglist.Add(DateTime.Now);
            List<TimeSlot> timeSlots = new List<TimeSlot>();
            for (int i = 0; i < bakinglist.Count - 1; i += 2)
                timeSlots.Add(new TimeSlot(bakinglist[i], bakinglist[i + 1]));

            List<TempNode> nodes = searchTempandVacu(car, timeSlots);

            return nodes;
           // return new List<TempNode>();

        }

        //寻找温度和真空度     
        public  List<TempNode> searchTempandVacu(CarNumToPos car, List<TimeSlot> times)
        {
            if (times == null || times.Count == 0) return null;
            List<TempNode> list = new List<TempNode>();    
            DateTime begintime = times[0].begin;
            String sql = "";
            String name = car.CarID < 10 ? "0" + car.CarID : "" + car.CarID;
            for (int i = 0; i < times.Count; i++)
              {
                TimeSlot t = times[i];
                sql = "select * from carpara_"+name+ " where RecodeTime>='"+times[i].begin.ToString()+ "' and RecodeTime<='"+times[i].end.ToString()+"'";
                LogInDB.Info(sql.Replace('\'',' '));
                DataSet ds = equipDB.ExecuteDataSet(CommandType.Text, sql);
                DataTable table = ds.Tables[0];
                int count = table.Rows.Count;
                for (int j = 0; j < count; j++)
                {
                    TempNode n = new TempNode();
                    for (int x = 1; x <= 30; x++)
                    {
                        n.temp[x-1] = float.Parse(table.Rows[j].ItemArray[x].ToString());
                    }
                    n.Vacu = (float)float.Parse(table.Rows[j].ItemArray[33].ToString());
                   
                   
                    n.time = DateTime.Parse(table.Rows[j].ItemArray[34].ToString()); ;
                    n.disminutes = 0;
                    for (int y = 0; y < i; y++)
                    {
                        n.disminutes += getTimecha(times[y].end, times[y + 1].begin);
                    }
                    n.begintime = begintime;
                    n.totalminiutes = getTimecha(begintime, n.time);
                    n.totalminiutes -= n.disminutes;

                    list.Add(n);
                }

            }

           



            return list;
        }
        #endregion

        #region 数据追踪检测
        private List<DateTime> getBakinglist(CarNumToPos car)
        {
            //1.查找进炉时间
            String sql = "select moveintime from bakingtime where CarNum=" + car.CarID;
            LogInDB.Info(sql);
            DataSet ds = equipDB.ExecuteDataSet(CommandType.Text, sql);
            DataTable table = ds.Tables[0];
            String moveintime = table.Rows[0].ItemArray[0].ToString();
            if (moveintime.Equals(""))
            {
                LogInDB.Info("进炉时间为空，请检查原因！");
                return null;
            }
            LogInDB.Info("入炉时间："+moveintime);
            //2.根据入炉时间找到carprocess表中的烘烤过程
            List<DateTime> bakinglist = new List<DateTime>();
            sql = "select * from carprocess where CarNum=" + car.CarID + " and Pos=" + car.Pos + " and Time>='" + moveintime + "'";
            ds = equipDB.ExecuteDataSet(CommandType.Text, sql);
            table = ds.Tables[0];
            int count = table.Rows.Count;

            for (int i = 0; i < count; i++)
            {
                LogInDB.Info(table.Rows[i].ItemArray[3].ToString() + ":" + table.Rows[i].ItemArray[4].ToString());
                String states = table.Rows[i].ItemArray[3].ToString();
                String time = table.Rows[i].ItemArray[4].ToString();
                if (states.Equals("bakingbegin") || states.Equals("bakingend") || states.Equals("bakingbreakdown") || states.Equals("bakingcontinue"))
                {
                    bakinglist.Add(DateTime.Parse(time));
                }
                if (states.Equals("bakingstop"))//加热终止
                {
                    bakinglist.Clear();
                }
                
            }

            LogInDB.Info("bakinglist：" + bakinglist.Count);
            return bakinglist;
        }

        //检测烘烤时长是否不够       
        private bool CheckBakingTime(CarNumToPos car)
        {
            try
            {
                String sql = "select * from bakingtime where CarNum=" + car.CarID;
                DataSet ds = equipDB.ExecuteDataSet(CommandType.Text, sql);
                DataTable table = ds.Tables[0];
                if (table.Rows.Count == 0)
                {
                    LogInDB.Info("bakingtime表里没有" + car.CarID + "号小车的信息");
                    return true;
                }
                DateTime bakingbegin = DateTime.Parse(table.Rows[0].ItemArray[4].ToString());
                DateTime bakingend = DateTime.Parse(table.Rows[0].ItemArray[5].ToString());

                TimeSpan t1 = new TimeSpan(bakingbegin.Ticks);
                TimeSpan t2 = new TimeSpan(bakingend.Ticks);
                TimeSpan t3 = t2.Subtract(t1).Duration();
                int timelength = (int)t3.TotalMinutes;

                //获得系统参数的烘烤时长
                Int16 bakingtime = Int16.Parse(UserDefineVariableInfo.DicVariables["bakingtime"].ToString()); ;
                LogInDB.Info("total=" + timelength + "   bakingtime=" + bakingtime);
                //进行比较
                if (timelength >= bakingtime)
                    return true;
                else
                    return false;
            }catch(Exception ex)
            {
                LogInDB.Info("CheckBakingTime err:"+ex.Message);
                return true;
            }
        }
        //实时真空度异常判断
        private void CheckVaccum(CarNumToPos car, List<DateTime> bakinglist)
        {
            //获取烘烤时间列表
           // List<DateTime> bakinglist = getBakinglist(car);
            if (bakinglist == null || bakinglist.Count == 0) return;
            //转化为timeslot
            if (bakinglist.Count % 2 != 0)
                bakinglist.Add(DateTime.Now);
            List<TimeSlot> timeSlots = new List<TimeSlot>();
            for(int i=0;i<bakinglist.Count-1;i+=2)           
                timeSlots.Add(new TimeSlot(bakinglist[i], bakinglist[i + 1]));

            List<DateTime>[] time_datatime = convertDateTime(timeSlots, VacuParam.time);

            bool res = searchCarVacu(car.CarID, time_datatime);
            if (res == false)
            {
                //设置carnumtopos的小车状态为异常
                car.CarState = 60;
                UpdateCarNumToPos(car);
                //设置cellbarcode表的电芯状态为真空度异常
                String name = car.CarID < 10 ? "0" + car.CarID : "" + car.CarID;
                String sql = "update cellbarcode_" + car.CarID + " set CellBakingState=40";
                equipDB.ExecuteNonQuery(CommandType.Text, sql);
            }
            return;
        }

        private bool searchCarVacu(int carid, List<DateTime>[] times)
        {
            bool res = true;
            try
            {
                //找出carpara表中最后一条记录的时间
                String name = carid < 10 ? "0" + carid : "" + carid;
                String sql = "select RecodeTime from carpara_" + name + " order by CarParaID desc LIMIT 1";
                DataSet ds = equipDB.ExecuteDataSet(CommandType.Text, sql);
                DataTable table = ds.Tables[0];   
                DateTime endtime = DateTime.Parse(table.Rows[0].ItemArray[0].ToString());
                //分阶段检查真空度
                for (int i = 0; i < times.Length; i++)
                {
                    if (times[i] == null) break;
                    List<DateTime> t = times[i];
               
                    for (int j = 0; j < t.Count - 1; j += 2)
                    {
                        //如果最后一条记录的时间早于此阶段的开始时间，说明没有记录了
                        if (endtime.CompareTo(t[j]) < 0)
                        {                            
                            return true;
                        }
                        sql = "select avg(Vacuum) from carpara_" + name + " where RecodeTime>='" + t[j].ToString() + "' and TIME<='" + t[j + 1].ToString()+"'";
                        ds = equipDB.ExecuteDataSet(CommandType.Text, sql);
                        table = ds.Tables[0];
                        float avg = float.Parse(table.Rows[0].ItemArray[0].ToString());

                        res = isVacuOK(i, avg);
                        // mainForm.sendToDebug("i=" + i + ",j=" + j + " :" + avg + ",res=" + res);

                        if (res == false) return false;


                    }
                }
            }
            catch (Exception ex)
            {
                LogInDB.Info("searchCarVacu err:"+ex.Message);
            }



            return res;
        }

        public static bool isVacuOK(int i, float avg)
        {
            i = i + 1;
            if (i % 2 == 1)
            {
                if (avg <= 300)
                    return true;
                else
                {
                 //   mainForm.sendToDebug("Vacu Exp: " + i + " avg=" + avg + "   >300");
                    return false;
                }
            }
            if (i < 12)
            {
                if (avg >= VacuParam.value1_low && avg <= VacuParam.value1_up)
                    return true;
                else
                {
                  //  mainForm.sendToDebug("Vacu Exp: " + i + " avg=" + avg);
                    return false;
                }
            }
            else
            {
                if (avg >= VacuParam.value2_low && avg <= VacuParam.value2_up)
                    return true;
                else
                {
                  //  mainForm.sendToDebug("Vacu Exp: " + i + " avg=" + avg);
                    return false;
                }
            }

        }


        private List<DateTime>[] convertDateTime(List<TimeSlot> slots, int[,] time)
        {
            List<DateTime>[] time_datatime = new List<DateTime>[19];
            int k = 0;
            DateTime begin = slots[0].begin;//起始时间
            for (int i = 0; i < 19; i++)
            {
                if (k == slots.Count) break;
                time_datatime[i] = new List<DateTime>();

                int start = time[i, 0];
                int end = time[i, 1];
                // mainForm.sendToDebug("i="+(i+1)+",start=" + start +",end="+end);
                for (int j = k; j < slots.Count; j++)
                {
                    int slots_start = getTimecha(begin, slots[j].begin);
                    int slots_end = getTimecha(begin, slots[j].end);
                    // mainForm.sendToDebug("j=" + j + ",slots_start=" + slots_start + ",slots_end=" + slots_end);
                    if (slots_start < start)
                    {
                        if (slots_end > start)
                        {
                            time_datatime[i].Add(begin.AddMinutes(start));
                            // mainForm.sendToDebug("add begin=" + begin.AddMinutes(start).ToString());
                        }
                        else
                        {
                            continue;
                        }
                    }
                    else
                    {
                        if (slots_start < end)
                        {
                            time_datatime[i].Add(slots[j].begin);
                            // mainForm.sendToDebug("add begin=" + slots[j].begin.ToString());
                        }
                        else
                        {
                            k = j;
                            break;
                        }
                    }

                    if (slots_end < end)
                    {
                        time_datatime[i].Add(slots[j].end);
                        // mainForm.sendToDebug("add end=" + slots[j].end.ToString());
                        k = j + 1;
                        continue;
                    }
                    else
                    {
                        time_datatime[i].Add(begin.AddMinutes(end));
                        // mainForm.sendToDebug("add end=" + begin.AddMinutes(end).ToString());   
                        k = j;
                        break;
                    }
                }


            }

            return time_datatime;
        }

        private int getTimecha(DateTime time1, DateTime time2)
        {
            TimeSpan ts1 = new TimeSpan(time1.Ticks);
            TimeSpan ts2 = new TimeSpan(time2.Ticks);
            TimeSpan cha = ts2.Subtract(ts1);
            return (int)cha.TotalMinutes;
        }

        //时间段
        public class TimeSlot
        {
            public DateTime begin;
            public DateTime end;
            public int timecha;//时间差
            public TimeSlot(DateTime begin, DateTime end)
            {
                this.begin = begin;
                this.end = end;
                TimeSpan ts1 = new TimeSpan(begin.Ticks);
                TimeSpan ts2 = new TimeSpan(end.Ticks);
                timecha = (int)ts2.Subtract(ts1).TotalMinutes;
            }
            public DateTime GetTime(int n)
            {
                DateTime res = begin.AddMinutes(n);
                return res;
            }

        }

        //真空度检查的时间段和阈值要求
        class VacuParam
        {
            public static int value1_low = 9000;
            public static int value1_up = 15000;

            public static int value2_low = 4000;
            public static int value2_up = 7000;

            public static int value_com = 300;
            //真空度异常检测时间段
            public static int[,] time = new int[,]{{2,38},{41,44},
                            {48,67}, {71,74},
                            {78,97}, {101,104},
                            {108,128}, {131,134},
                            {138,157}, {161,164},
                            {168,187}, {191,194},
                            {198,237}, {241,244},
                            {248,287}, {291,294},
                            {298,337}, {341,344},
                            {348,390}
                 };
            //真空度曲线绘制时间段
            public static int[,] huizhi_time = new int[,]{{0,40},{40,45},
                            {45,70}, {70,75},
                            {75,100}, {100,105},
                            {105,130}, {130,135},
                            {135,160}, {160,165},
                            {165,190}, {190,195},
                            {195,240}, {240,245},
                            {245,290}, {290,295},
                            {295,340}, {340,345},
                            {345,390}
                 };

        }



        //实时温度异常检测
        private void CheckTemp(CarNumToPos car, List<DateTime> bakinglist)
        {
            
            if (bakinglist==null||bakinglist.Count == 0) return;
            DateTime starttime = bakinglist[0];
            //计算当前时间是实际烘烤时间的第几分钟
            int n = (int)(DateTime.Now - starttime).TotalMinutes;
            for(int i=2;i<bakinglist.Count;i+=2)
            {
                n -= (int)(bakinglist[i] - bakinglist[i - 1]).TotalMinutes;
            }

            if(n>50)//距离50分钟后
            {
                bool isng = false;
                String name = car.CarID < 10 ? "0" + car.CarID : "" + car.CarID;
                for (int i = 1; i <= 30; i++)
                {
                    String sql = "select avg(Temp" + i + ") from carpara_" + name + " where RecodeTime>'"
                             + DateTime.Now.AddMinutes(-5).ToString() + "' and RecodeTime<'" + DateTime.Now.ToString()+"'";
                    DataSet ds = equipDB.ExecuteDataSet(CommandType.Text, sql);
                    DataTable table = ds.Tables[0];
                    int avg= int.Parse(table.Rows[0].ItemArray[0].ToString());

                    if(avg < TempSetLowLimit)//低温异常
                    {
                        sql = "update cellbarcode_" + name + " set CellBakingState=20 where LaminateNum=" + i;
                        equipDB.ExecuteNonQuery(CommandType.Text, sql);
                        isng = true;
                    }
                    
                    if (avg > TempSetUpLimit)//高温异常
                    {
                        sql = "update cellbarcode_" + name + " set CellBakingState=30 where LaminateNum=" + i;
                        equipDB.ExecuteNonQuery(CommandType.Text, sql);
                        isng = true;
                    }
                          
                }
                if (isng)
                {
                    car.CarState = 60;
                    UpdateCarNumToPos(car);
                }


          }

        }
        #endregion

        #region 电芯追溯

        public class Chip
        {
            public String chipcode { get; set; }

            public String car { get; set; }
            public String oval { get; set; }
            public String Lam { get; set; }
            public String Row { get; set; }
            public String Pos { get; set; }
            public String shangliaotime { get; set; }          
            public String intime { get; set; }
            public String bakingbegintime { get; set; }
            public String bakingendtime { get; set; }
            public String outtime { get; set; }
            public String xialiaotime { get; set; }
            public String temp { get; set; }
            public String vapu { get; set; }
            public String CellBakingState { get; set; }
            public String bakingNum { get; set; }
            public List<DateTime> bakinglist { get; set; }

            public Chip()
            { 
            }

            public Chip(Chip c)
            {
                this.chipcode = c.chipcode;
                this.car = c.oval;
                this.oval = c.oval;
                this.Lam= c.Lam;
                this.Row = c.Row;
                this.Pos = c.Pos;
                this.shangliaotime = c.shangliaotime;
                this.xialiaotime = c.xialiaotime;
                this.intime = c.intime;
                this.outtime = c.outtime;
                this.bakingbegintime = c.bakingbegintime;
                this.bakingendtime = c.bakingendtime;
                this.temp = c.temp;
                this.vapu = c.vapu;
                this.CellBakingState = c.CellBakingState;
                this.bakingNum = c.bakingNum;
                this.bakinglist = c.bakinglist;
            }
           
        }
        //根据电芯二维码查询
        public List<Chip> SearchCode(String code)
        {
            //1.查询历史记录
         
            List <Chip> chips = new List<Chip>();
            String sql = "select * from historycellbarcode where CellBarcode='"+code+"'";
            DataSet ds = equipDB.ExecuteDataSet(CommandType.Text, sql);
            DataTable table = ds.Tables[0];
           
            for (int i=0;i<table.Rows.Count;i++)
            {
                Chip p = new Chip();
                p.chipcode = code;
                p.car= table.Rows[i].ItemArray[1].ToString();
                p.shangliaotime = table.Rows[i].ItemArray[4].ToString();
                p.xialiaotime = table.Rows[i].ItemArray[5].ToString();
                p.Lam= table.Rows[i].ItemArray[6].ToString();
                p.Row= table.Rows[i].ItemArray[7].ToString();
                p.Pos= table.Rows[i].ItemArray[8].ToString();
                p.CellBakingState= table.Rows[i].ItemArray[9].ToString();
                p.bakingNum= table.Rows[i].ItemArray[10].ToString();
                getCarProcess(p,chips,true);
                
            }
            //2.查询当前记录 从10个小车中找电芯
            for(int i=1;i<=10;i++)
            {
                String name = i < 10 ? "0" + i : "" + i;
                sql = "select * from cellbarcode_"+name+ " where CellBarcode='"+code+"'";
                ds = equipDB.ExecuteDataSet(CommandType.Text, sql);
                table = ds.Tables[0];
                if(table.Rows.Count>0)
                {
                    Chip p = new Chip();
                    p.chipcode = code;
                    p.car = i+"";
                    p.shangliaotime = table.Rows[i].ItemArray[4].ToString();
                    p.xialiaotime = table.Rows[i].ItemArray[5].ToString();
                    p.Lam = table.Rows[i].ItemArray[6].ToString();
                    p.Row = table.Rows[i].ItemArray[7].ToString();
                    p.Pos= table.Rows[i].ItemArray[8].ToString();
                    p.CellBakingState = table.Rows[i].ItemArray[9].ToString();
                    p.bakingNum = table.Rows[i].ItemArray[10].ToString();
                    getCarProcess(p, chips, false);
                    break;
                }
            }
          //  LogInDB.Info("count=" + chips.Count);
            return chips;
        }

        //获取电芯烘烤转炉记录  提供车号 上料下料时间
        private List<Chip> getCarProcess(Chip c,List<Chip> chips,bool ishistory)
        {   
            //根据时间找到carprocess表中的烘烤过程          
            String sql = "select * from carprocess where CarNum=" + c.car + " and Time>='" + c.shangliaotime + "' and Time<='" + c.xialiaotime + "'"; 
            DataSet ds = equipDB.ExecuteDataSet(CommandType.Text, sql);
            DataTable table = ds.Tables[0];
            int count = table.Rows.Count;
            Chip c1 = null;
            List<DateTime> bakinglist =null;
           // LogInDB.Info("count1="+count);
            for (int i = 0; i < count; i++)
            {              
                String oval = table.Rows[i].ItemArray[2].ToString();
                String states = table.Rows[i].ItemArray[3].ToString();
                String time = table.Rows[i].ItemArray[4].ToString();
               // LogInDB.Info("i=" + i+",oval="+oval+",states="+states+",time="+time);
               
                if (states.Equals("movein"))
                {
                    c.oval = oval;
                    c.intime = time;
                    c1 = new Chip(c);
                
                    bakinglist = new List<DateTime>();
                }
                if (states.Equals("moveout"))
                {
                    c1.outtime = time;
                    if (bakinglist.Count > 0)
                    {
                        c1.bakingbegintime = bakinglist[0].ToString();
                        c1.bakingendtime = bakinglist[bakinglist.Count - 1].ToString();
                        c1.bakinglist = bakinglist;
                        String name = int.Parse(c.car) < 10 ? "0" + c.car : "" + c.car;
                        if (ishistory)
                            sql = "select avg(Temp" + c.Lam + ") from historycarpara_" + name + " where RecodeTime>='" + c1.bakingbegintime + "' and RecodeTime<='" + c1.bakingendtime + "'";
                        else
                            sql = "select avg(Temp" + c.Lam + ") from carpara_" + name + " where RecodeTime>='" + c1.bakingbegintime + "' and RecodeTime<='" + c1.bakingendtime + "'";
                        DataSet  ds1 = equipDB.ExecuteDataSet(CommandType.Text, sql);
                        DataTable table1 = ds1.Tables[0];
                        if (table1.Rows.Count > 0)
                            c1.temp = table1.Rows[0].ItemArray[0].ToString();
                        if (ishistory)
                            sql = "select avg(Vacuum) from historycarpara_" + name + " where Vacuum<70 and RecodeTime>='" + c1.bakingbegintime + "' and RecodeTime<='" + c1.bakingendtime + "'";
                        else
                            sql = "select avg(Vacuum) from carpara_" + name + " where Vacuum<70 and RecodeTime>='" + c1.bakingbegintime + "' and RecodeTime<='" + c1.bakingendtime + "'";
                        ds1 = equipDB.ExecuteDataSet(CommandType.Text, sql);
                        table1 = ds1.Tables[0];
                        if (table1.Rows.Count > 0)
                            c1.vapu = table1.Rows[0].ItemArray[0].ToString();
                    }
                    chips.Add(c1);
                    c1 = null;
                    bakinglist = null;
                }
                if (states.Equals("bakingbegin") || states.Equals("bakingend") || states.Equals("bakingbreakdown") || states.Equals("bakingcontinue"))
                {
                  
                    bakinglist.Add(DateTime.Parse(time));
                  
                }
                if (states.Equals("bakingstop"))//加热终止
                {
                    bakinglist.Clear();
                }

            }
            return chips;
        }

        //根据小车和时间查询电芯
        public List<Chip> SearchCar(String carId,String start,String end)
        {
           
            List<Chip> chips = new List<Chip>();     
            String sql = "select * from carprocess where CarNum=" + carId + " and Time>='" + start + "' and Time<='" + end+"'";          
            DataSet ds = equipDB.ExecuteDataSet(CommandType.Text, sql);
            DataTable table = ds.Tables[0];
            List<DateTime> bakinglist = null;
            Chip c = null;
         
            for (int i = 0; i < table.Rows.Count; i++)
            {
                String oval = table.Rows[i].ItemArray[2].ToString();
                String states = table.Rows[i].ItemArray[3].ToString();
                String time = table.Rows[i].ItemArray[4].ToString();
                if (states.Equals("shangliao"))
                {
                    c = new Chip();
                    c.car = carId;
                    c.oval = oval;
                    c.shangliaotime = time;
                    bakinglist = new List<DateTime>();
                }
                if (states.Equals("xialiao"))
                {
                    if (c == null) continue;
                    c.xialiaotime = time;
                    if (bakinglist.Count > 0)
                    {
                        c.bakingbegintime = bakinglist[0].ToString();
                        c.bakingendtime = bakinglist[bakinglist.Count - 1].ToString();
                        c.bakinglist = bakinglist;
                    }
                    chips.Add(c);

                    c = null;
                    bakinglist = null;
                }
                if (states.Equals("movein"))
                {
                    if (c == null) continue;
                    c.intime = time;
                }
                if (states.Equals("moveout"))
                {
                    if (c == null) continue;
                    c.outtime = time;
                }
                if (states.Equals("bakingbegin") || states.Equals("bakingend") || states.Equals("bakingbreakdown") || states.Equals("bakingcontinue"))
                {
                    if (c == null) continue;
                    bakinglist.Add(DateTime.Parse(time));
                }
                if (states.Equals("bakingstop"))//加热终止
                {
                    if (c == null) continue;
                    bakinglist.Clear();
                }
            }

            return chips;
        }


        public List<Chip> SearchOval(String oval, String start, String end)
        {

            List<Chip> chips = new List<Chip>();
            String sql = "select * from carprocess where Pos=" + oval + " and Time>='" + start + "' and Time<='" + end + "'";
            DataSet ds = equipDB.ExecuteDataSet(CommandType.Text, sql);
            DataTable table = ds.Tables[0];
            List<DateTime> bakinglist = null;
            Chip c = null;

            for (int i = 0; i < table.Rows.Count; i++)
            {
                String carId = table.Rows[i].ItemArray[1].ToString();
                String states = table.Rows[i].ItemArray[3].ToString();
                String time = table.Rows[i].ItemArray[4].ToString();
                if (states.Equals("shangliao"))
                {
                    c = new Chip();
                    c.car = carId;
                    c.oval = oval;
                    c.shangliaotime = time;
                    bakinglist = new List<DateTime>();
                }
                if (states.Equals("xialiao"))
                {
                    if (c == null) continue;
                    c.xialiaotime = time;
                    if (bakinglist.Count > 0)
                    {
                        c.bakingbegintime = bakinglist[0].ToString();
                        c.bakingendtime = bakinglist[bakinglist.Count - 1].ToString();
                        c.bakinglist = bakinglist;
                    }
                    chips.Add(c);

                    c = null;
                    bakinglist = null;
                }
                if (states.Equals("movein"))
                {
                    if (c == null) continue;
                    c.intime = time;
                }
                if (states.Equals("moveout"))
                {
                    if (c == null) continue;
                    c.outtime = time;
                }
                if (states.Equals("bakingbegin") || states.Equals("bakingend") || states.Equals("bakingbreakdown") || states.Equals("bakingcontinue"))
                {
                    if (c == null) continue;
                    bakinglist.Add(DateTime.Parse(time));
                }
                if (states.Equals("bakingstop"))//加热终止
                {
                    if (c == null) continue;
                    bakinglist.Clear();
                }
            }

            return chips;
        }

        #endregion

        #region  串口通信
        public static SerialPort serialPort = new SerialPort();
        /// <summary>
        /// 实例化串行端口资源
        /// </summary>
        private void InstanceSerialPort()
        {
            try
            {
                //实例化串行端口
               // serialPort = new SerialPort();
                //端口名  注:因为使用的是USB转RS232 所以去设备管理器中查看一下虚拟com口的名字
                serialPort.PortName = UserDefineVariableInfo.DicVariables["com"].ToString();
                 //波特率
                 serialPort.BaudRate = 9600;
                 //奇偶校验
                 serialPort.Parity = Parity.None;
                 //停止位
                  serialPort.StopBits = StopBits.One;
                 //数据位
                 serialPort.DataBits = 8;
                  //忽略null字节
                  serialPort.DiscardNull = true;
                   //接收事件
                   serialPort.DataReceived += serialPort_DataReceived;
                  // serialPort.DataReceived += new SerialDataReceivedEventHandler(serialPort_DataReceived);

                   serialPort.Open();
                   if (serialPort.IsOpen)
                        LogInDB.Info("串口打开成功：" + serialPort.PortName);                   
                   else
                       LogInDB.Info("串口打开关闭：" + serialPort.PortName);
                     
                
            }
            catch(Exception ex)
            {
                LogInDB.Info("串口通信异常："+ex.Message);
            }
        }

        /// <summary>
		/// 接收数据
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
               // SerialPort serialPort = (SerialPort)sender;
                //开启接收数据线程
                Thread threadReceiveSub = new Thread(new ParameterizedThreadStart(ReceiveData));
                threadReceiveSub.Start(serialPort);
             //   LogInDB.Info("开启串口接收数据线程");
            }
            catch (Exception ex)
            {
                SetMessage(ex.Message);
                LogInDB.Info("开启串口接收数据线程 错误："+ex.Message);
            }
        }

        private void ReceiveData(object serialPortobj)
        {
            try
            {
                SerialPort serialPort = (SerialPort)serialPortobj;

                //防止数据接收不完整 线程sleep(100)
                System.Threading.Thread.Sleep(100);

                string str = serialPort.ReadExisting();

                if (str == string.Empty)
                {
                    return;
                }
                else
                {
                    SetMessage(str);
                }
            }
            catch (Exception ex)
            {
                SetMessage(ex.Message);
            }
        }

        public void SendData()
        {          
            byte[] WriteBuffer = Encoding.ASCII.GetBytes("0A");
            serialPort.Write(WriteBuffer, 0, WriteBuffer.Length);
           // serialPort.Write("OA");
            LogInDB.Info("发送数据");
        }
        public static String carcode = "";
        /// <summary>
        /// 添加记录
        /// </summary>
        /// <param name="msg"></param>
        private void SetMessage(string msg)
        {
            carcode = msg.Substring(msg.Length-4);
            LogInDB.Info("扫描枪扫码:"+carcode);
        }
        #endregion

        #region 电芯型号
        public class ChipModel
        {
            public String id;
            public String name;
            public String area;
            public String type;
            public String width;
            public String height;
            public String houdu;
            public String time;
            public String beizhu;
            public String tempup;
            public String tempsb;
            public String templow;
            public String bakingtimeup;
            public String bakingtimesb;
            public String bakingtimelow;
            public String vacuup;
            public String vacusb;
            public String vaculow;
            public String isselect;

        }

        public List<ChipModel> GetChipModels()
        {
            List<ChipModel> models = new List<ChipModel>();
            String sql = "select * from modeltype";
            DataSet ds = equipDB.ExecuteDataSet(CommandType.Text, sql);
            DataTable table = ds.Tables[0];
            for(int i=0;i<table.Rows.Count;i++)
            {
                ChipModel m = new ChipModel();
                m.id= table.Rows[i].ItemArray[0].ToString();
                m.name = table.Rows[i].ItemArray[1].ToString();
                m.width= table.Rows[i].ItemArray[2].ToString();
                m.height= table.Rows[i].ItemArray[3].ToString();
                m.houdu= table.Rows[i].ItemArray[4].ToString();
                m.time= table.Rows[i].ItemArray[5].ToString();
                m.area=table.Rows[i].ItemArray[6].ToString();
                m.type=table.Rows[i].ItemArray[7].ToString();
               
                m.beizhu=table.Rows[i].ItemArray[8].ToString();
                m.tempup=table.Rows[i].ItemArray[9].ToString();
                m.tempsb=table.Rows[i].ItemArray[10].ToString();
                m.templow=table.Rows[i].ItemArray[11].ToString();
                m.vacuup=table.Rows[i].ItemArray[12].ToString();
                m.vacusb=table.Rows[i].ItemArray[13].ToString();
                m.vaculow=table.Rows[i].ItemArray[14].ToString();
                m.bakingtimeup=table.Rows[i].ItemArray[15].ToString();
                m.bakingtimesb=table.Rows[i].ItemArray[16].ToString();
                m.bakingtimelow=table.Rows[i].ItemArray[17].ToString();
                m.isselect= table.Rows[i].ItemArray[18].ToString();
                models.Add(m);
            }
           // LogInDB.Info("size="+models.Count);

            return models;

        }
       
        public int InsertChipModels(ChipModel model)
        {
            String sql = "insert into modeltype(name,width,height,houdu,time,area,type,beizhu,tempUp,tempStandard,tempLow,vacuUp,vacuStandard,vacuLow,bakingtimeUp,bakingtimeStandard,bakingtimeLow)" +
                " values('" + model.name + "'," + model.width + "," + model.height + "," + model.houdu + ",'" +model.time.ToString()+"','"+model.area+"','"+model.type+"','"+model.beizhu
                +"',"+model.tempup+","+model.tempsb+","+model.templow+","+model.vacuup+","+model.vacusb+","+model.vaculow+","
                +model.bakingtimeup+","+model.bakingtimesb+","+model.bakingtimelow+ ")";

            int res=equipDB.ExecuteNonQuery(CommandType.Text, sql);
            return res;
        }

        public int UpdateChipModels(ChipModel model)
        {
            String sql = "Update modeltype set width=" + model.width + ",height=" + model.height +
                ",houdu=" + model.houdu + ",time='" + model.time + "',area='" + model.area + "',type='" +
                model.type + "',beizhu='" + model.beizhu + "',tempUp=" + model.tempup + ",tempStandard=" + model.tempsb + ",tempLow=" + model.templow
                + ",vacuUp=" + model.vacuup + ",vacuStandard=" + model.vacusb + ",vacuLow=" + model.vaculow + ",bakingtimeUp=" + model.bakingtimeup
                + ",bakingtimeStandard=" + model.bakingtimesb + ",bakingtimeLow=" + model.bakingtimelow + " where name=" + model.name;
           int res= equipDB.ExecuteNonQuery(CommandType.Text, sql);
            return res;
        }
        public int DelChipModels(String modelname)
        {
            String sql = "delete from modeltype where name=" + modelname;
            int res = equipDB.ExecuteNonQuery(CommandType.Text, sql);
            return res;
        }

        public int setModel(int id)
        {
            String sql = "update modeltype set isselect=0";
            equipDB.ExecuteNonQuery(CommandType.Text, sql);
            sql = "update modeltype set isselect=1 where id="+id;
            int res=equipDB.ExecuteNonQuery(CommandType.Text, sql);
            return res;
        }

        public int getModel()
        {
            String sql = "select id from modeltype where isselect=1";
            DataSet ds = equipDB.ExecuteDataSet(CommandType.Text, sql);
            DataTable table = ds.Tables[0];
            if (table.Rows.Count == 0)
                return -1;
            return int.Parse(table.Rows[0].ItemArray[0].ToString());
        }
        #endregion

        #region 自动抽检水含量
        public DataTable getNgChips()
        {
            String sql = "select * from ngchips";
            DataSet ds = equipDB.ExecuteDataSet(CommandType.Text, sql);
            DataTable table = ds.Tables[0];
            return table;
        }


        public void Autosample_Init()
        {

        }
        #endregion
        #region 上料下料查询
        public class SXChip
        {
            public String car { get; set; }
            public String CellBarcode { get; set; }
            public String CellIOState { get; set; }
            public String Intime { get; set; }
            public String Outtime { get; set; }
            public String LaminateNum { get; set; }
            public String RowNum { get; set; }
            public String PosNum { get; set; }
            public String CellBakingState { get; set; }
            public String bakingNum { get; set; }
        }
        public List<SXChip> getShangXialiao(int type,String start,String end)
        {
            List<SXChip> lst = new List<SXChip>();
            String sql = "";
            DateTime endtime = DateTime.Parse(end).AddDays(1);
            for(int i=1;i<=10;i++)
            {
                String name = i < 10 ? "0" + i:"" + i;
                if(type==1)
                   sql = "select * from cellbarcode_" + name + " where Intime>='" + start + "' and Intime<='" + endtime.ToString() + "'";
                else
                   sql = "select * from cellbarcode_" + name + " where Outtime>='" + start + "' and Outtime<='" + endtime.ToString() + "'";
                DataSet ds = equipDB.ExecuteDataSet(CommandType.Text, sql);
                DataTable table = ds.Tables[0];
                if (table.Rows.Count == 0) continue;
                for (int j = 0; j < table.Rows.Count; j++)
                {
                    SXChip chip = new SXChip();
                    chip.car = i + "";
                    chip.CellBarcode = table.Rows[j].ItemArray[1].ToString();
                    chip.CellIOState = table.Rows[j].ItemArray[2].ToString();
                    chip.Intime = table.Rows[j].ItemArray[3].ToString();
                    chip.Outtime = table.Rows[j].ItemArray[4].ToString();
                    chip.LaminateNum = table.Rows[j].ItemArray[5].ToString();
                    chip.RowNum = table.Rows[j].ItemArray[6].ToString();
                    chip.PosNum = table.Rows[j].ItemArray[7].ToString();
                    chip.CellBakingState= table.Rows[j].ItemArray[8].ToString();
                    chip.bakingNum = table.Rows[j].ItemArray[9].ToString();
                    lst.Add(chip);

                }
                if(type==1)
                   sql = "select * from historycellbarcode where Intime>='" + start + "' and Intime<='" + end + "'";
                else
                    sql = "select * from historycellbarcode where Outtime>='" + start + "' and Outtime<='" + end + "'";
                ds = equipDB.ExecuteDataSet(CommandType.Text, sql);
                table = ds.Tables[0];
                if (table.Rows.Count == 0) continue;
                for (int j = 0; j < table.Rows.Count; j++)
                {
                    SXChip chip = new SXChip();
                    chip.car = table.Rows[j].ItemArray[1].ToString();
                    chip.CellBarcode = table.Rows[j].ItemArray[2].ToString();
                    chip.CellIOState = table.Rows[j].ItemArray[3].ToString();
                    chip.Intime = table.Rows[j].ItemArray[4].ToString();
                    chip.Outtime = table.Rows[j].ItemArray[5].ToString();
                    chip.LaminateNum = table.Rows[j].ItemArray[6].ToString();
                    chip.RowNum = table.Rows[j].ItemArray[7].ToString();
                    chip.PosNum = table.Rows[j].ItemArray[8].ToString();
                    chip.CellIOState = table.Rows[j].ItemArray[9].ToString();
                    chip.bakingNum = table.Rows[j].ItemArray[10].ToString();
                    lst.Add(chip);
                }
             }
            return lst;
        }
        #endregion
    }
}
