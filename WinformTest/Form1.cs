using System;
using System.Windows.Forms;
using ATL.Common;
using ATL.MES;
using ATL.Core;
using ATL.Engine;
using VbFormTest;
using System.Collections.Generic;
using System.Linq;
using ATL.PLCVariableValueService;
using HslCommunication;

namespace WinformTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public static Form1 Current;

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                string CellBarcode = "testBarcode";
                ATL.MES.A016.Root root = InterfaceClient.Current.A015(CellBarcode, "4");
                if (root != null && root.ResponseInfo.Result.ToUpper() == "OK")
                {
                    //string msg = $"电芯扫码枪扫码成功:{CellBarcode},条码校验成功";
                    string msg = string.Empty;
                    if (ATL.Common.StringResources.IsDefaultLanguage)
                    {
                        msg = $"电芯扫码枪扫码成功:{CellBarcode},条码校验成功";
                    }
                    else
                    {
                        msg = $"The code scanning gun of the cell scanned the code successfully :{CellBarcode},barcode verification succeed ";
                    }
                    LogInDB.Info(msg);
                    MessageBox.Show(msg);
                }
                else
                {
                    //string msg = $"电芯:{CellBarcode} 校验失败，不可用来生产";
                    string msg = string.Empty;
                    if (ATL.Common.StringResources.IsDefaultLanguage)
                    {
                        msg = $"电芯:{CellBarcode} 校验失败，不可用来生产";
                    }
                    else
                    {
                        msg = $"cell:{CellBarcode} Verificated failed ，it is not available for production ";
                    }
                    LogInDB.Info(msg);
                    MessageBox.Show(msg);
                }
            }
            catch(Exception ex)
            {
                LogHelper.Error(ex.ToString());
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            //MessageBox.Show("当前MES服务器地址为: " + UserDefineVariableInfo.DicVariables["serverIpAdr"].ToString());
            if (ATL.Common.StringResources.IsDefaultLanguage)
            {
                MessageBox.Show("当前MES服务器地址为: " + UserDefineVariableInfo.DicVariables["serverIpAdr"].ToString());
            }
            else
            {
                MessageBox.Show("The current MES server address is : " + UserDefineVariableInfo.DicVariables["serverIpAdr"].ToString());
            }
            UserDefineVariableInfo.DicVariables["WebBrowserHeight"] = 700;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //string msg = ATL.Core.Core.PLCconnected ? "正常" : "异常";
            string msg = string.Empty;
            if (ATL.Common.StringResources.IsDefaultLanguage)
            {
                msg = ATL.Core.Core.PLCconnected ? "正常" : "异常";
            }
            else
            {
                msg = ATL.Core.Core.PLCconnected ? "normal" : "abnormal";
            }
            //MessageBox.Show($"当前PLC连接{msg}");
            if (ATL.Common.StringResources.IsDefaultLanguage)
            {
                MessageBox.Show($"当前PLC连接{msg}");
            }
            else
            {
                MessageBox.Show($"Current PLC connection {msg}");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //MessageBox.Show("当前登陆MES用户名为: " + ATL.MES.UserInfo.UserName);
            if (ATL.Common.StringResources.IsDefaultLanguage)
            {
                MessageBox.Show("当前登陆MES用户名为: " + ATL.MES.UserInfo.UserName);
            }
            else
            {
                MessageBox.Show("The user name of landing MES is : " + ATL.MES.UserInfo.UserName);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            //MessageBox.Show("当前MES下发的Model为: " + DeivceEquipmentidPlcInfo.lstDeivceEquipmentidPlcInfos[0].ModelInfo);
            if (ATL.Common.StringResources.IsDefaultLanguage)
            {
                MessageBox.Show("当前MES下发的Model为: " + DeivceEquipmentidPlcInfo.lstDeivceEquipmentidPlcInfos[0].ModelInfo);
            }
            else
            {
                MessageBox.Show("The current model issued by MES is : " + DeivceEquipmentidPlcInfo.lstDeivceEquipmentidPlcInfos[0].ModelInfo);
            }
        }

        VbFormTest.Form1 frm;
        private void button6_Click(object sender, EventArgs e)
        {
            
        }

        private void button7_Click(object sender, EventArgs e)
        {
            try
            {
                string Model = DeivceEquipmentidPlcInfo.lstDeivceEquipmentidPlcInfos[0].ModelInfo;
                string ProductSN = "22222222";
                string EquType = "";
                List<ATL.MES.A035.DataListItem> lst = new List<ATL.MES.A035.DataListItem>();//很多行
                                                                                            ////第一行EXCEL数据
                ATL.MES.A035.DataListItem DataListItem1 = new ATL.MES.A035.DataListItem();
                DataListItem1.CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff");
                DataListItem1.ProductSN = ProductSN;
                DataListItem1.Data = new List<ATL.MES.A035.DataItem>();//EXCEL一行数据

                ATL.MES.A035.DataItem DataItem1 = new ATL.MES.A035.DataItem();
                DataItem1.ParamID = "1";
                DataItem1.ParamDesc = "光纤点距离";
                DataItem1.SpecParamID = "2";
                DataItem1.Value = "3";
                DataListItem1.Data.Add(DataItem1);

                ATL.MES.A035.DataItem DataItem2 = new ATL.MES.A035.DataItem();
                DataItem2.ParamID = "4";
                DataItem2.ParamDesc = "头部错位补偿值";
                DataItem2.SpecParamID = "5";
                DataItem2.Value = "6";
                DataListItem1.Data.Add(DataItem2);
                //加第一行
                lst.Add(DataListItem1);

                ////第二行EXCEL数据
                ATL.MES.A035.DataListItem DataListItem2 = new ATL.MES.A035.DataListItem();

                DataListItem2.Data = new List<ATL.MES.A035.DataItem>();//EXCEL一行数据

                ATL.MES.A035.DataItem DataItem3 = new ATL.MES.A035.DataItem();
                DataItem3.ParamID = "1";
                DataItem3.ParamDesc = "光纤点距离";
                DataItem3.SpecParamID = "2";
                DataItem3.Value = "3";
                DataListItem1.Data.Add(DataItem3);

                ATL.MES.A035.DataItem DataItem4 = new ATL.MES.A035.DataItem();
                DataItem4.ParamID = "4";
                DataItem4.ParamDesc = "头部错位补偿值";
                DataItem4.SpecParamID = "5";
                DataItem4.Value = "6";
                DataListItem1.Data.Add(DataItem4);
                //加第二行
                lst.Add(DataListItem1);

                ATL.MES.A035.RequestInfo requestInfo = new ATL.MES.A035.RequestInfo();
                requestInfo.DataType = "ProductionData";
                requestInfo.Model = Model;
                requestInfo.EquType = EquType;
                requestInfo.DataList = lst;

                ATL.MES.A036.Root root = InterfaceClient.Current.A035(requestInfo);
                if (root != null && root.ResponseInfo.Result.ToUpper() == "OK")
                {
                    //string msg = $"A035上传成功";
                    string msg = string.Empty;
                    if (ATL.Common.StringResources.IsDefaultLanguage)
                    {
                        msg = $"A035上传成功";
                    }
                    else
                    {
                        msg = $"A035 uploaded successfully";
                    }
                    LogInDB.Info(msg);
                    MessageBox.Show(msg);
                }
                else
                {
                    //string msg = $"A035上传失败";
                    string msg = string.Empty;
                    if (ATL.Common.StringResources.IsDefaultLanguage)
                    {
                        msg = $"A035上传失败";
                    }
                    else
                    {
                        msg = $"A035 upload failed";
                    }
                    LogInDB.Info(msg);
                    MessageBox.Show(msg);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex.ToString());
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            string Model = "111111111";
            string ProductSN = "22222222";
            string EquType = "33333333";
            
            foreach(var v in DeivceEquipmentidPlcInfo.lstDeivceEquipmentidPlcInfos)
            {
                try
                {
                    List<ATL.MES.A035.DataListItem> lst = new List<ATL.MES.A035.DataListItem>();//很多行
                    #region 自动读取ActualValueAddr
                    foreach (var d in DeviceInputOutputConfigInfo.lstDeviceInputOutputConfigInfos.Where(x => x.EquipmentID == v.EquipmentID))
                    {
                        if (string.IsNullOrEmpty(d.ActualValueAddr)) continue;
                        ATL.MES.A035.DataListItem DataListItem1 = new ATL.MES.A035.DataListItem();
                        DataListItem1.CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff");
                        DataListItem1.ProductSN = ProductSN;
                        DataListItem1.Data = new List<ATL.MES.A035.DataItem>();//EXCEL一行数据
                        ATL.MES.A035.DataItem DataItem1 = new ATL.MES.A035.DataItem();
                        DataItem1.ParamID = d.UploadParamID;
                        DataItem1.ParamDesc = d.ParamName;
                        DataItem1.SpecParamID = d.SendParamID;

                        UserVariableValueService service = new UserVariableValueService();
                        string ParamValue = service.GetActualValue(d);

                        DataItem1.Value = ParamValue;
                        DataListItem1.Data.Add(DataItem1);

                        lst.Add(DataListItem1);
                    }
                    #endregion
                    ATL.MES.A035.RequestInfo requestInfo = new ATL.MES.A035.RequestInfo();
                    requestInfo.DataType = "ProductionData";
                    requestInfo.Model = Model;
                    requestInfo.EquType = EquType;
                    requestInfo.DataList = lst;
                    ATL.MES.A036.Root root = InterfaceClient.Current.A035(requestInfo);
                    if (root != null && !string.IsNullOrEmpty(root.ResponseInfo.Result) && root.ResponseInfo.Result.ToUpper() == "OK")
                    {
                        //string msg = $"设备ID：{v.EquipmentID} 的A035接口数据上传成功";
                        string msg = string.Empty;
                        if (ATL.Common.StringResources.IsDefaultLanguage)
                        {
                            msg = $"设备ID：{v.EquipmentID} 的A035接口数据上传成功";
                        }
                        else
                        {
                            msg = $"Equipment ID：{v.EquipmentID}'s A035 interface data uploaded successfully ";
                        }
                        LogInDB.Info(msg);
                    }
                    else
                    {
                        //string msg = $"设备ID：{v.EquipmentID} 的A035接口数据上传失败";
                        string msg = string.Empty;
                        if (ATL.Common.StringResources.IsDefaultLanguage)
                        {
                            msg = $"设备ID：{v.EquipmentID} 的A035接口数据上传成功";
                        }
                        else
                        {
                            msg = $"Equipment ID：{v.EquipmentID}'s A035 interface data uploaded successfully ";
                        }
                        LogInDB.Error(msg);
                    }
                }
                catch (Exception ex)
                {
                    LogInDB.Error(ex.ToString());
                }
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            try
            {
                InterfaceClient.RecvdData r = new InterfaceClient.RecvdData();
                r.cmd = "A014";
                //r.jason = @"{""Header"":{""FunctionID"":""A014"",""ResponseTime"":""2020-01-05 20:35:54 215"",""IsSuccess"":""True"",""ErrorCode"":""0"",""ErrorMsg"":""Null"",""SessionID"":""798eb62f-8ce4-4416-81a6-90aba706dd9b"",""EQCode"":""AXRX016F"",""RequestTime"":""2020-01-05 20:36:35 437"",""SoftName"":null,""PCName"":""WPRD-NFNI7062-P""},""ResponseInfo"":{""Type"":""Normal"",""Products"":[{""ProductSN"":""W76952EA16C6"",""Pass"":""OK"",""Param"":[{""ParamID"":""52166"",""ParamDesc"":null,""Value"":""1.104205"",""Result"":""OK""},{""ParamID"":""52167"",""ParamDesc"":null,""Value"":""1.135754"",""Result"":""OK""},{""ParamID"":""52168"",""ParamDesc"":null,""Value"":""1.030591"",""Result"":""OK""},{""ParamID"":""52169"",""ParamDesc"":null,""Value"":""0.9149129"",""Result"":""OK""},{""ParamID"":""52170"",""ParamDesc"":null,""Value"":""0.8202667"",""Result"":""OK""},{""ParamID"":""52171"",""ParamDesc"":null,""Value"":""0.7571693"",""Result"":""OK""},{""ParamID"":""52172"",""ParamDesc"":null,""Value"":""0.8833642"",""Result"":""OK""},{""ParamID"":""52173"",""ParamDesc"":null,""Value"":""0.7466531"",""Result"":""OK""},{""ParamID"":""52181"",""ParamDesc"":null,""Value"":""0.8097505"",""Result"":""OK""},{""ParamID"":""52182"",""ParamDesc"":null,""Value"":""0.7782018"",""Result"":""OK""},{""ParamID"":""52183"",""ParamDesc"":null,""Value"":""0.9149129"",""Result"":""OK""},{""ParamID"":""52184"",""ParamDesc"":null,""Value"":""1.030591"",""Result"":""OK""},{""ParamID"":""52185"",""ParamDesc"":null,""Value"":""1.041108"",""Result"":""OK""},{""ParamID"":""52186"",""ParamDesc"":null,""Value"":""1.030591"",""Result"":""OK""},{""ParamID"":""52187"",""ParamDesc"":null,""Value"":""1.041108"",""Result"":""OK""},{""ParamID"":""51740"",""ParamDesc"":null,""Value"":""5557"",""Result"":""OK""},{""ParamID"":""51741"",""ParamDesc"":null,""Value"":""5557"",""Result"":""OK""},{""ParamID"":""51011"",""ParamDesc"":null,""Value"":""58"",""Result"":""OK""},{""ParamID"":""51012"",""ParamDesc"":null,""Value"":""86"",""Result"":""OK""},{""ParamID"":""51856"",""ParamDesc"":null,""Value"":""OK_20200105203634957_W76952EA16C6_5_test.jpg"",""Result"":""OK""},{""ParamID"":""251"",""ParamDesc"":null,""Value"":""0.7466531"",""Result"":""OK""}]}]}}";
                r.sendTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff");
                r.receivedTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff");
                List<ATL.MES.A013.ProductsItem> lst = new List<ATL.MES.A013.ProductsItem>();

                ATL.MES.A014.Root root = ATL.MES.InterfaceClient.Current.A013("", lst);
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex.ToString());
            }
        }
        void textbox_show(string jason,bool result)
        {
            //string aa = result ? "成功" : "失败";
            string aa = string.Empty;
            if (ATL.Common.StringResources.IsDefaultLanguage)
            {
                aa = result ? "成功" : "失败";
            }
            else
            {
                aa = result ? "success" : "fail";
            }
            textBox1.Text = "返回"+aa+ "\r\n" + jason;
        }

        private void button10_Click(object sender, EventArgs e)
        {
            ATL.MES.A067.ProductsItem test = new ATL.MES.A067.ProductsItem();
            test.ProductSN1 = "asdfghjkl";
            test.HoldCode = "01";
            test.Operation = "01";
            List<ATL.MES.A067.ProductsItem> temp = new List<ATL.MES.A067.ProductsItem>();
            temp.Add(test);
            ATL.MES.A068.Root root = InterfaceClient.Current.A067("Normal", temp);
            if (root!=null)
                textbox_show(root.ToJSON(), true);
            else
                textbox_show(root.ToJSON(), false);
        }

        private void button11_Click(object sender, EventArgs e)
        {
            List<string> temp = new List<string>();
            temp.Add("12314");
            ATL.MES.A070.Root root = InterfaceClient.Current.A069("Normal","aaa", temp);
            if (root != null)
                textbox_show(root.ToJSON(), true);
            else
                textbox_show(root.ToJSON(), false);
        }

        private void button12_Click(object sender, EventArgs e)
        {
            ATL.MES.A071.ProductsItem test = new ATL.MES.A071.ProductsItem();
            test.ProductSN1 = "asdfghjkl";
            List<ATL.MES.A071.ProductsItem> temp = new List<ATL.MES.A071.ProductsItem>();
            temp.Add(test);
            ATL.MES.A072.Root root = InterfaceClient.Current.A071("Normal", temp);
            if (root != null)
                textbox_show(root.ToJSON(), true);
            else
                textbox_show(root.ToJSON(), false);
        }

        private void button15_Click(object sender, EventArgs e)
        {
            ATL.MES.A074.Root root = InterfaceClient.Current.A073("asdfghjk1");
            if (root != null)
                textbox_show(root.ToJSON(), true);
            else
                textbox_show(root.ToJSON(), false);
        }

        private void button14_Click(object sender, EventArgs e)
        {
            ATL.MES.A075.Cell test = new ATL.MES.A075.Cell();
            test.serialno = "asdfghjkl";
            test.container_station = "01";
            List<ATL.MES.A075.Cell> temp = new List<ATL.MES.A075.Cell>();
            temp.Add(test);
            ATL.MES.A076.Root root = InterfaceClient.Current.A075("W12345", temp);
            if (root != null)
                textbox_show(root.ToJSON(), true);
            else
                textbox_show(root.ToJSON(), false);
        }

        private void button13_Click(object sender, EventArgs e)
        {
            ATL.MES.A077.OutputParamItem output_temp = new ATL.MES.A077.OutputParamItem();
            output_temp.ParamDesc = "1";
            output_temp.ParamID = "1";
            output_temp.ParamValue = "1";
            output_temp.Result = "1";
            output_temp.SpecParamID = "1";
            List<ATL.MES.A077.OutputParamItem> lstoutput_temp = new List<ATL.MES.A077.OutputParamItem>();
            lstoutput_temp.Add(output_temp);
            ATL.MES.A077.Containers container = new ATL.MES.A077.Containers();
            container.ChildEquCode = "2";
            container.Container = "W6544";
            container.Pass = "OK";
            container.Station = "1";
            container.OutputParam = lstoutput_temp;
            List<ATL.MES.A077.Containers> lstcontainer = new List<ATL.MES.A077.Containers>();
            lstcontainer.Add(container);
            ATL.MES.A078.Root root = InterfaceClient.Current.A077("Normal","2", lstcontainer);
            if (root != null)
                textbox_show(root.ToJSON(), true);
            else
                textbox_show(root.ToJSON(), false);
        }

        private void button16_Click(object sender, EventArgs e)
        {
            ATL.MES.A080.Root root = InterfaceClient.Current.A079("asdfghjk1",true);
            if (root != null)
                textbox_show(root.ToJSON(), true);
            else
                textbox_show(root.ToJSON(), false);
        }

        private void button17_Click(object sender, EventArgs e)
        {
            List<ATL.MES.A013_MTW20.OutputParamItem> lstoutput = new List<ATL.MES.A013_MTW20.OutputParamItem>();
            ATL.MES.A013_MTW20.OutputParamItem output = new ATL.MES.A013_MTW20.OutputParamItem();
            output.ParamID = "1";
            output.ParamDesc = "1";
            output.ParamValue = "1";
            output.Result = "1";
            lstoutput.Add(output);
            ATL.MES.A013_MTW20.ProductsItem product = new ATL.MES.A013_MTW20.ProductsItem();
            product.ProductSN = "N3320132059002";
            product.Pass = "OK";
            product.Station = "C";
            product.EmployeeNo = "TEMP";
            product.OutputParam = lstoutput;
            List<ATL.MES.A013_MTW20.ProductsItem> lstproduct = new List<ATL.MES.A013_MTW20.ProductsItem>();
            lstproduct.Add(product);
            ATL.MES.A014_MTW20.Root root= InterfaceClient.Current.A013_MTW20("Normal", lstproduct);
            if (root != null)
                textbox_show(root.ToJSON(), true);
            else
                textbox_show(root.ToJSON(), false);
        }
    }
}
