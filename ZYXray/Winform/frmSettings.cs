using Esquel.BaseManager;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using XRayClient.Core;
using ZY.DAL;
using ZY.MitutoyoReader;
using ZY.Model;
using ZY.Systems;
using ZYXray.Models;

namespace ZYXray.Winform
{
    public partial class frmSettings : Form
    {
        public static frmSettings Current;
        DictionaryRefDAL dictionaryRefDAL = new DictionaryRefDAL();
        public frmSettings()
        {
            InitializeComponent();
        }

        private void bbiSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (MessageBox.Show("保存数据立即生效,是否确定保存？[是]/否", "", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                lstUpdateRef = new List<DictionaryRef>();
                lstInsRef = new List<DictionaryRef>();
                //上传
                Common.IsUpload = chkUpload.Checked;
                SetValue("Settings", "IsUpload", chkUpload.Checked.ToString(), true, chkUpload.Text);
                //日志
                Common.IsPLCWriteLog = chkLog.Checked;
                SetValue("Settings", "IsPLCWriteLog", chkLog.Checked.ToString(), true, chkLog.Text);
                //扫码
                Common.IsScan = chkScan.Checked;
                SetValue("Settings", "IsScan", chkScan.Checked.ToString(), true, chkScan.Text);
                //原图路径
                Common.OrigImageFile = txtOrigImageFile.Text.Trim();
                SetValue("ImageSaveConfig", "_saveOrigPath", txtOrigImageFile.Text.Trim().Replace("\\", "_"), true, label1.Text);
                //结果图路径
                Common.ResultImageFile = txtResultImageFile.Text.Trim();
                SetValue("ImageSaveConfig", "_saveTestPath", txtResultImageFile.Text.Trim().Replace("\\", "_"), true, label2.Text);
                //删除图片时间  小时
                Common.DelImageHour = Convert.ToInt32(txtDelHour.Text.Trim());
                SetValue("Settings", "DelImageHour", txtDelHour.Text.Trim(), true, label3.Text + label6.Text);
                //删除图片时间  分钟
                Common.DelImageMinute = Convert.ToInt32(txtDelMinute.Text.Trim());
                SetValue("Settings", "DelImageMinute", txtDelMinute.Text.Trim(), true, label3.Text + label7.Text);
                //原图保存天数  
                Common.OrigSaveDay = Convert.ToInt32(txtOrigSaveDay.Text.Trim());
                SetValue("Settings", "OrigSaveDay", txtOrigSaveDay.Text.Trim(), true, label4.Text);
                //结果图保存天数  
                Common.ResultSaveDay = Convert.ToInt32(txtResultSaveDay.Text.Trim());
                SetValue("Settings", "ResultSaveDay", txtResultSaveDay.Text.Trim(), true, label5.Text);

                //分拣启用后扫码  
                Common.SortingScan = chkSortingScan.Checked;
                SetValue("Settings", "SortingScan", chkSortingScan.Checked.ToString(), true, chkSortingScan.Text);


                //复判新版本
                Common.NewCheck = chkNewCheck.Checked;
                SetValue("Settings", "NewCheck", chkNewCheck.Checked.ToString(), true, chkNewCheck.Text);

                //硬盘最小百分比
                Common.MinMemory = double.Parse(txtMinMemory.Text.Trim());
                SetValue("Settings", "MinMemory", txtMinMemory.Text.Trim(), true, label9.Text);

                //工厂区域编号
                Common.FactoryNo = txtFactory.Text.Trim().Substring(0,1);
                SetValue("Settings", "FactoryNo", txtFactory.Text.Trim(), true, label15.Text);

                //4测厚线性
                Common.FourThickness = chkFourThickness.Checked;
                SetValue("Settings", "FourThickness", chkFourThickness.Checked.ToString(), true, chkFourThickness.Text);

                //是否周次抽检
                Common.IsWeekCheck = chkIsWeekCheck.Checked;
                SetValue("Settings", "IsWeekCheck", chkIsWeekCheck.Checked.ToString(), true, chkIsWeekCheck.Text);

                //抽检周次
                Common.WeekCheck = int.Parse(seWeekCheck.Text);
                SetValue("Settings", "WeekCheck", seWeekCheck.Text.ToString(), true, "抽检周次");


                //抽检个数
                Common.WeekCheckCount = int.Parse(seWeekCheckCount.Text);
                SetValue("Settings", "WeekCheckCount", seWeekCheckCount.Text.ToString(), true, label16.Text);

                
                //FQI 文件路径
                Common.FQI_MI_Path = txtFQI_MI_Path.Text.Trim().Replace("_",@"\\");
                SetValue("Settings", "FQI_MI_Path", txtFQI_MI_Path.Text.ToString().Replace("\\","_"), true, label17.Text);
                //XRAY文件路径
                Common.FQI_MI_Path = txtXRAY_MI_Path.Text.Trim().Replace("_", @"\\");
                SetValue("Settings", "XRAY_MI_Path", txtXRAY_MI_Path.Text.ToString().Replace("\\", "_"), true, label18.Text);

       

                //二次码上传
                Common.IsCheckTwoBarcode = chkCheckTwoBarcode.Checked;
                SetValue("Settings", "IsCheckTwoBarcode", chkCheckTwoBarcode.Checked.ToString(), true, chkCheckTwoBarcode.Text);
            
                //仪器短连接
                Common.IsShortSocket = chkShortSocket.Checked;
                SetValue("Settings", "IsShortSocket", chkShortSocket.Checked.ToString(), true, chkShortSocket.Text);

                //条码前缀校验
                Common.IsCheckBarcodeLenth = chkIsCheckBarcodeLenth.Checked;
                SetValue("Settings", "IsCheckBarcodeLenth", chkIsCheckBarcodeLenth.Checked.ToString(), true, chkIsCheckBarcodeLenth.Text);

                //MES参数下发
                Common.IsProductType = chkIsProductType.Checked;
                SetValue("Settings", "IsProductType", chkIsProductType.Checked.ToString(), true, chkIsProductType.Text);


                //尺寸标快正负差
                Common.FQIRange = double.Parse(seFQIRange.Text);
                SetValue("Settings", "FQIRange", seFQIRange.Text.ToString(), true, label19.Text);

                errMsg = string.Empty;
                DictionaryRefDAL.SaveData(lstUpdateRef, lstInsRef, ref errMsg);


                if (chkShortSocket.Checked)
                {
                    MitutoyoReaderIF.UnInit();
                }
              







                CheckParamsConfig.Instance.WeekCounts = int.Parse(seWeek.Text);//周次校验
                CheckParamsConfig.Instance.Mi = txtMI.Text.Trim();
                CheckParamsConfig.Instance.SetMarking = txtMarking.Text.Trim();
                CheckParamsConfig.Instance.IsSetMarking = chkIsMarking.Checked;
                CheckParamsConfig.Instance.IsCheckMI = chkIsMI.Checked;
                CheckParamsConfig.Instance.IsCheckWeekCount = chkIsWeek.Checked;
                CheckParamsConfig.Instance.IsNoTemperature = chkIsNoTemperature.Checked;
                CheckParamsConfig.Instance.IsNoResistance = chkIsNoResistance.Checked;
                CheckParamsConfig.Instance.MarkingCurrent = txtMarkingCurrent.Text.Trim();
                CheckParamsConfig.Instance.IsCheckMarking = chkIsCheckMarking.Checked;
                CheckParamsConfig.Instance.FactoryCode = txtFactory.Text.Trim();
                CheckParamsConfig.Instance.IsNoUpLoadMdiAndPPGData = chkIsNoUpLoadMdiAndPPGData.Checked;
                CheckParamsConfig.Instance.IsAlOnLeft = chkIsAlOnLeft.Checked;
                //CheckParamsConfig.Instance.IsThreeTab = chkIsThreeTab.Checked;
                CheckParamsConfig.Instance.PPGWarmingCounts = int.Parse(sePPGWarmingCounts.Text);
                CheckParamsConfig.Instance.MDIWarmingCounts = int.Parse(seMDIWarmingCounts.Text);
                CheckParamsConfig.Instance.StationRangeOutTimes = int.Parse(seMDIWarmingCounts.Text);
                CheckParamsConfig.Instance.WarmWaitTime = int.Parse(seMDIWarmingCounts.Text);

                FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "IsNoUpLoadMdiAndPPGData", chkIsNoUpLoadMdiAndPPGData.Checked.ToString());
                FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "IsAlOnLeft", chkIsAlOnLeft.Checked.ToString());
                FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "IsThreeTab", chkIsThreeTab.Checked.ToString());
                FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "General", "PPGWarmingCounts", sePPGWarmingCounts.Text.ToString());
                FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "General", "MDIWarmingCounts", seMDIWarmingCounts.Text.ToString());
                FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "General", "StationRangeOutTimes", seMDIWarmingCounts.Text.ToString());
                FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "General", "WarmWaitTime", seMDIWarmingCounts.Text.ToString());
                FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "General", "FactoryCode", txtFactory.Text.ToString());

                FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "WeekCounts", seWeek.Text);
                FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "Mi", txtMI.Text);
                FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "SetMarking", txtMarking.Text);
                FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "IsSetMarking", chkIsMarking.Checked.ToString());
                FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "IsCheckMI", chkIsMI.Checked.ToString());
                FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "IsCheckWeekCount", chkIsWeek.Checked.ToString());
                FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "IsNoTemperature", chkIsNoTemperature.Checked.ToString());
                FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "IsNoResistance", chkIsNoResistance.Checked.ToString());
                FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "General", "MarkingCurrent", txtMarkingCurrent.Text.ToString());
                FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "IsCheckMarking", chkIsCheckMarking.Checked.ToString());

                MessageBox.Show("保存成功!");
                DataRefresh();
                NoEnable();
            }
        }
        string errMsg = string.Empty;
        List<DictionaryRef> lstInsRef = new List<DictionaryRef>();
        List<DictionaryRef> lstUpdateRef = new List<DictionaryRef>();
        private void SetValue(string refKey, string refCode, string RefValue, bool RefSystem, string chkValText)
        {
            DictionaryRef m = listdic.FirstOrDefault(o => o.RefCode != null && o.RefKey != null && o.RefKey.Equals(refKey) && o.RefCode.Equals(refCode));
            if (m == null)
            {
                DictionaryRef model = new DictionaryRef();
                model.RefKey = refKey;
                model.RefCode = refCode;
                model.RefValue = RefValue;
                model.RefRemark = chkValText;
                model.RefSystem = RefSystem;
                model.RefIsUse = true;
                lstInsRef.Add(model);
            }
            else
            {
                lstUpdateRef.Add(new DictionaryRef() { RefKey = refKey, RefCode = refCode, RefValue = RefValue, RefSystem = true, RefRemark = chkValText });
            }
        }
        List<DictionaryRef> listdic = new List<DictionaryRef>();
        public void DataRefresh()
        {
            try
            {
                //chkIsWeek.Checked = 
                //chkIsMI.Checked = bool.Parse(FilesManager.GetPrivateProfileString(Application.StartupPath + "\\CheckParamsConfig.ini", "Settings", "IsScan", "False"));
                //chkIsMarking.Checked = bool.Parse(FilesManager.GetPrivateProfileString(Application.StartupPath + "\\CheckParamsConfig.ini", "Settings", "IsLog", "False"));
                //原始配置文件读取

                seWeek.Text = FilesManager.GetPrivateProfileString(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "WeekCounts", "0");
                txtMI.Text = FilesManager.GetPrivateProfileString(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "Mi", "");
                txtMarking.Text = FilesManager.GetPrivateProfileString(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "SetMarking", "");
                txtMarkingCurrent.Text = FilesManager.GetPrivateProfileString(Application.StartupPath + "\\CheckParamsConfig.ini", "General", "MarkingCurrent", "");
                chkIsMarking.Checked = bool.Parse(FilesManager.GetPrivateProfileString(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "IsSetMarking", "False"));
                chkIsMI.Checked = bool.Parse(FilesManager.GetPrivateProfileString(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "IsCheckMI", "False"));
                chkIsWeek.Checked = bool.Parse(FilesManager.GetPrivateProfileString(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "IsCheckWeekCount", "False"));
                chkIsCheckMarking.Checked = bool.Parse(FilesManager.GetPrivateProfileString(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "IsCheckMarking", "False"));



                chkIsNoUpLoadMdiAndPPGData.Checked = bool.Parse(FilesManager.GetPrivateProfileString(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "IsNoUpLoadMdiAndPPGData", "False"));
                chkIsAlOnLeft.Checked = bool.Parse(FilesManager.GetPrivateProfileString(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "IsAlOnLeft", "False"));
                chkIsThreeTab.Checked = bool.Parse(FilesManager.GetPrivateProfileString(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "IsThreeTab", "False"));
                sePPGWarmingCounts.Text = FilesManager.GetPrivateProfileString(Application.StartupPath + "\\CheckParamsConfig.ini", "General", "PPGWarmingCounts", "0");
                seMDIWarmingCounts.Text = FilesManager.GetPrivateProfileString(Application.StartupPath + "\\CheckParamsConfig.ini", "General", "MDIWarmingCounts", "0");
                seStationRangeOutTimes.Text = FilesManager.GetPrivateProfileString(Application.StartupPath + "\\CheckParamsConfig.ini", "General", "StationRangeOutTimes", "3");
                seWarmWaitTime.Text = FilesManager.GetPrivateProfileString(Application.StartupPath + "\\CheckParamsConfig.ini", "General", "WarmWaitTime", "60");
                txtFactory.Text = FilesManager.GetPrivateProfileString(Application.StartupPath + "\\CheckParamsConfig.ini", "General", "FactoryCode", "W01");


                Common.FactoryNo = txtFactory.Text.Substring(0,1);

                string filters = string.Empty, errMsg = string.Empty;

                filters = $" {Common.SqlIsNullKey}RefIsUse = 1";

                var listRef = dictionaryRefDAL.GetList(filters, ref errMsg);
                listdic = listRef as List<DictionaryRef>;

                DictionaryRef m = new DictionaryRef();
                //上传
                m = listRef.FirstOrDefault(o => o.RefCode != null && o.RefCode.Equals("IsUpload"));
                chkUpload.Checked = Common.IsUpload = m == null ? false : bool.Parse(m.RefValue);
                //扫码
                m = listRef.FirstOrDefault(o => o.RefCode != null && o.RefCode.Equals("IsScan"));
                chkScan.Checked = Common.IsScan = m == null ? false : bool.Parse(m.RefValue);
                //写日志
                m = listRef.FirstOrDefault(o => o.RefCode != null && o.RefCode.Equals("IsPLCWriteLog"));
                chkLog.Checked = Common.IsPLCWriteLog = m == null ? false : bool.Parse(m.RefValue);

                //原图路径
                m = listRef.FirstOrDefault(o => o.RefCode != null && o.RefCode.Equals("_saveOrigPath"));
                txtOrigImageFile.Text = Common.OrigImageFile = m == null ? @"D:\XRayPic\Orig\" : m.RefValue.Replace("_",@"\");

                //结果图路径
                m = listRef.FirstOrDefault(o => o.RefCode != null && o.RefCode.Equals("_saveTestPath"));
                txtResultImageFile.Text = Common.ResultImageFile = m == null ? @"D:\XRayPic\Test\" : m.RefValue.Replace("_", @"\");

                //删除图片时间  小时

                m = listRef.FirstOrDefault(o => o.RefCode != null && o.RefCode.Equals("DelImageHour"));
                Common.DelImageHour = m == null ? 7 : Convert.ToInt32(m.RefValue);
                txtDelHour.Text = Common.DelImageHour.ToString();
                //删除图片时间  分钟
                m = listRef.FirstOrDefault(o => o.RefCode != null && o.RefCode.Equals("DelImageMinute"));
                Common.DelImageMinute = m == null ? 30 : Convert.ToInt32(m.RefValue);
                txtDelMinute.Text = Common.DelImageMinute.ToString();

                //原图保存天数  
                m = listRef.FirstOrDefault(o => o.RefCode != null && o.RefCode.Equals("OrigSaveDay"));
                Common.OrigSaveDay = m == null ? 7 : Convert.ToInt32(m.RefValue);
                txtOrigSaveDay.Text = Common.OrigSaveDay.ToString();
                //结果图保存天数  
                m = listRef.FirstOrDefault(o => o.RefCode != null && o.RefCode.Equals("ResultSaveDay"));
                Common.ResultSaveDay = m == null ? 7 : Convert.ToInt32(m.RefValue);
                txtResultSaveDay.Text = Common.ResultSaveDay.ToString();

                //分拣启用后扫码  
                m = listRef.FirstOrDefault(o => o.RefCode != null && o.RefCode.Equals("SortingScan"));
                Common.SortingScan = m == null ? false : bool.Parse(m.RefValue);
                chkSortingScan.Checked = Common.SortingScan;

                //复判新版本  
                m = listRef.FirstOrDefault(o => o.RefCode != null && o.RefCode.Equals("NewCheck"));
                Common.NewCheck = m == null ? false : bool.Parse(m.RefValue);
                chkNewCheck.Checked = Common.NewCheck;


                //硬盘最小百分比
                m = listRef.FirstOrDefault(o => o.RefCode != null && o.RefCode.Equals("MinMemory"));
                Common.MinMemory = m == null ? 10 : double.Parse(m.RefValue);
                txtMinMemory.Text = Common.MinMemory.ToString();

                //4测厚线性  
                m = listRef.FirstOrDefault(o => o.RefCode != null && o.RefCode.Equals("FourThickness"));
                Common.FourThickness = m == null ? false : bool.Parse(m.RefValue);
                chkFourThickness.Checked = Common.FourThickness;

                //是否周次抽检  
                m = listRef.FirstOrDefault(o => o.RefCode != null && o.RefCode.Equals("IsWeekCheck"));
                Common.IsWeekCheck = m == null ? false : bool.Parse(m.RefValue);
                chkIsWeekCheck.Checked = Common.IsWeekCheck;

                //抽检周次  
                m = listRef.FirstOrDefault(o => o.RefCode != null && o.RefCode.Equals("WeekCheck"));
                Common.WeekCheck = m == null ? 0 : int.Parse(m.RefValue);
                seWeekCheck.Text = Common.WeekCheck.ToString();
                //抽检个数  
                m = listRef.FirstOrDefault(o => o.RefCode != null && o.RefCode.Equals("WeekCheckCount"));
                Common.WeekCheckCount = m == null ? 0 : int.Parse(m.RefValue);
                seWeekCheckCount.Text = Common.WeekCheckCount.ToString();

                //FQI_MI路径  
                m = listRef.FirstOrDefault(o => o.RefCode != null && o.RefCode.Equals("FQI_MI_Path"));
                txtFQI_MI_Path.Text = Common.FQI_MI_Path = m == null ? @"\\atlbattery.com\nd-fqi\MISpec-FQI正常规格\" : m.RefValue.Replace("_", @"\");

                //XRAY_MI路径  
                m = listRef.FirstOrDefault(o => o.RefCode != null && o.RefCode.Equals("XRAY_MI_Path"));
                txtXRAY_MI_Path.Text = Common.XRAY_MI_Path = m == null ? @"\\nd-app01\XRaySpec\MISpec.csv" : m.RefValue.Replace("_", @"\");


                //二次码上传  
                m = listRef.FirstOrDefault(o => o.RefCode != null && o.RefCode.Equals("IsCheckTwoBarcode"));
                Common.IsCheckTwoBarcode = m == null ? false : bool.Parse(m.RefValue);
                chkCheckTwoBarcode.Checked = Common.IsCheckTwoBarcode;
                //仪器短连接  
                m = listRef.FirstOrDefault(o => o.RefCode != null && o.RefCode.Equals("IsShortSocket"));
                Common.IsShortSocket = m == null ? false : bool.Parse(m.RefValue);
                chkShortSocket.Checked = Common.IsShortSocket;
                //条码前缀校验  
                m = listRef.FirstOrDefault(o => o.RefCode != null && o.RefCode.Equals("IsCheckBarcodeLenth"));
                Common.IsCheckBarcodeLenth = m == null ? false : bool.Parse(m.RefValue);
                chkIsCheckBarcodeLenth.Checked = Common.IsCheckBarcodeLenth;

                //MES参数下发
                m = listRef.FirstOrDefault(o => o.RefCode != null && o.RefCode.Equals("IsProductType"));
                Common.IsProductType = m == null ? false : bool.Parse(m.RefValue);
                chkIsProductType.Checked = Common.IsProductType;

                //尺寸标快正负差
                m = listRef.FirstOrDefault(o => o.RefCode != null && o.RefCode.Equals("FQIRange"));
                Common.FQIRange = m == null ? 0 : double.Parse(m.RefValue);
                seFQIRange.Text = Common.FQIRange.ToString();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        private void frmSettings_Load(object sender, EventArgs e)
        {
            //chkIsWeek.Checked = bool.Parse(FilesManager.GetPrivateProfileString(Application.StartupPath + "\\CheckParamsConfig.ini", "Settings", "IsUpload", "False"));
            //chkIsMI.Checked = bool.Parse(FilesManager.GetPrivateProfileString(Application.StartupPath + "\\CheckParamsConfig.ini", "Settings", "IsScan", "False"));
            //chkIsMarking.Checked = bool.Parse(FilesManager.GetPrivateProfileString(Application.StartupPath + "\\CheckParamsConfig.ini", "Settings", "IsLog", "False"));

            DataRefresh();
        }

        private void bbiUpdate_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DataRefresh();
            int type = 1;
            FrmLogin login = new FrmLogin(type);
            login.ShowDialog();
            switch (login.blLogin)
            {
                case 1:
                    IniSuperCheck();
                    break;
                case 2:
                    IniMeCheck();
                    break;
                case 3:
                    break;
                default:
                    break;
            }
        }
        //me权限
        private void IniMeCheck()
        {
            chkLog.Enabled = true;
            chkScan.Enabled = true;
            chkUpload.Enabled = true;
            txtDelHour.Enabled = true;
            txtDelMinute.Enabled = true;
            txtOrigImageFile.Enabled = true;
            txtOrigSaveDay.Enabled = true;
            txtResultImageFile.Enabled = true;
            txtResultSaveDay.Enabled = true;
        }
        /// <summary>
        /// 超级权限
        /// </summary>
        private void IniSuperCheck()
        {
            chkLog.Enabled = true;
            chkScan.Enabled = true;
            chkUpload.Enabled = true;
            txtDelHour.Enabled = true;
            txtDelMinute.Enabled = true;
            txtOrigImageFile.Enabled = true;
            txtOrigSaveDay.Enabled = true;
            txtResultImageFile.Enabled = true;
            txtResultSaveDay.Enabled = true;
            chkSortingScan.Enabled = true;
            btnDel.Enabled = true;
            chkDelFile.Enabled = true;
            txtLoadingFile.Enabled = true;
            btnLoadDelete.Enabled = true;
            btnDel.Enabled = true;
            //groupBox5.Enabled = true;
            btnDelall.Enabled = true;
            chkNewCheck.Enabled = true;
            chkIsProductType.Enabled = true;
            chkFourThickness.Enabled = true;
            chkShortSocket.Enabled = true;
            txtMinMemory.Enabled = true;
        }

        private void NoEnable()
        {
            chkLog.Enabled = false;
            chkScan.Enabled = false;
            chkUpload.Enabled = false;
            txtDelHour.Enabled = false;
            txtDelMinute.Enabled = false;
            txtOrigImageFile.Enabled = false;
            txtOrigSaveDay.Enabled = false;
            txtResultImageFile.Enabled = false;
            txtResultSaveDay.Enabled = false;
            chkSortingScan.Enabled = false;
            btnDel.Enabled = false;
            chkDelFile.Enabled = false;
            txtLoadingFile.Enabled = false;
            btnLoadDelete.Enabled = false;
            btnDel.Enabled = false;
            //groupBox5.Enabled = false;
            btnDelall.Enabled = false;
            chkNewCheck.Enabled = false;
            chkIsProductType.Enabled = false;
            chkFourThickness.Enabled = false;
            chkShortSocket.Enabled = false;
            txtMinMemory.Enabled = false;
        }

        private void btnLoadDelete_Click(object sender, EventArgs e)
        {
            try
            {
                string dirD = txtLoadingFile.Text.Trim();
                dirD = txtLoadingFile.Text.Trim();
                DirectoryInfo dir = new DirectoryInfo(dirD);
                FileSystemInfo[] files = dir.GetFileSystemInfos();//获取文件夹中所有文件和文件夹
                                                                  //自定义数组
                List<string> list = new List<string>();

                //对单个FileSystemInfo进行判断,如果是文件夹则进行递归操作
                foreach (FileSystemInfo FSys in files)
                {
                    list.Add(FSys.Name);
                }
                list.RemoveAt(list.Count - 1);
                if (list.Count == 0)
                {
                    MessageBox.Show("没有多余可删除文件，请检查");
                    return;
                }
                //清空项
                chkDelFile.Properties.Items.Clear();

                string[] arr = list.ToArray();
                //添加项
                chkDelFile.Properties.Items.AddRange(arr);

                ////设置选中状态
                //if (checkedComboBoxEdit1.Properties.Items.Count > 0)
                //{
                //    //设置选中状态
                //    checkedComboBoxEdit1.Properties.Items[strs[0]].CheckState = CheckState.Checked;
                //    //设置选项是否可用
                //    checkedComboBoxEdit1.Properties.Items[strs[0]].Enabled = false;
                //}
                //取值
                chkDelFile.EditValue.ToString();
                //获取各项值 放在List集合中
                List<object> listFile = chkDelFile.Properties.Items.GetCheckedValues();

                //注意 当取得值是多项时，各项之间的间隔是 英文状态下 逗号+空格
                //转换方法
                string result = chkDelFile.EditValue.ToString().Replace(", ", ",");

                //是否显示 确定、取消按钮
                chkDelFile.Properties.ShowButtons = true;
                //是否显示 取消按钮
                //chkDelFile.Properties.ShowPopupCloseButton = false;

                //下拉显示项的个数 (设置为下拉个数加1正好可以显示全部，因为有一行是全选项)
                chkDelFile.Properties.DropDownRows = chkDelFile.Properties.Items.Count + 1;
            }
            catch (Exception ex)
            {

                MessageBox.Show("文件夹输入路径错误，请检查!");
            }
        }

        private void btnDel_Click_1(object sender, EventArgs e)
        {
            new Thread(new ThreadStart(delegate
            {
                var strList = chkDelFile.Text.Trim().Split(',');
                for (int i = 0; i < strList.Length; i++)
                {
                    string FileDel = txtLoadingFile.Text.Trim() + @"\" + strList[i].Trim();
                    if (FileDel.Substring(0, 1) == "." || strList[i] == "")
                    {
                        continue;
                    }
                    Directory.Delete(FileDel, true);
                }
                groupBox5.Invoke(new EventHandler(delegate
                {
                    MessageBox.Show("删除成功!");
                }));
            })).Start();
        }

        private void btnDelall_Click(object sender, EventArgs e)
        {
            new Thread(new ThreadStart(delegate
            {
                if (!txtLoadingFile.Text.Trim().Contains("20"))
                {
                    groupBox5.Invoke(new EventHandler(delegate
                    {
                        MessageBox.Show("不能删除不包含年份的路径，请检查!");
                    }));
                    return;
                }
                else
                {
                    Directory.Delete(txtLoadingFile.Text.Trim(), true);
                    groupBox5.Invoke(new EventHandler(delegate
                    {
                        MessageBox.Show("删除成功!");
                    }));
                }
            })).Start();
        }

        private void chkDelFile_EditValueChanged(object sender, EventArgs e)
        {

        }

        private void btnMarkingCurrent_Click(object sender, EventArgs e)
        {
            SetMarking setMarking = new SetMarking();
            if (setMarking.ShowDialog() == DialogResult.OK)
            {
                txtMarkingCurrent.Text = CheckParamsConfig.Instance.MarkingBase;
            }
        }
    }
}
