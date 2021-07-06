using ATL.Core;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using Esquel.BaseManager;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using XRayClient.Core;
using ZY.BLL;
using ZY.Model;
using ZY.Systems;
using ZYXray.ViewModels;

namespace ZYXray.Winform
{
    public partial class frmAtlLogin : XtraForm
    {
        public frmAtlLogin()
        {
            InitializeComponent();
        }

        public bool IsPermission { get; set; }
        public int Model { get; set; }
        public string BarcodeLenth { get; set; }

        private string errMsg = string.Empty;

        private void frmTip_Load(object sender, EventArgs e)
        {
            try
            {
                cbo_Nom.SelectedIndex = 0;
                cbo_WorkInfo.SelectedIndex = 0;
                txt_FactoryId.Text = UserDefineVariableInfo.DicVariables["FactoryCode"].ToString();
                txt_PcName.Text = CheckParamsConfig.Instance.PcName;
                txt_MachinNo.Text = UserDefineVariableInfo.DicVariables["AssetsNO"].ToString();
                txtUserName.Text = ATL.MES.UserInfo.UserName;
                //cbo_Model.Items.Clear();
                //if (CheckParamsConfig.Instance.ProductMode.Contains(","))
                //{
                //    string[] strArr = CheckParamsConfig.Instance.ProductMode.Split(',');
                //    for (int i = 0; i < strArr.Length; i++)
                //    {
                //        cbo_Model.Items.Add(strArr[i]);
                //    }
                //}

                //txtStaffID.Properties.MaxLength = Common.LenStaffID;


                //cbo_Model.Properties.Columns.Add(new LookUpColumnInfo("Iden", 40, "Iden"));
                //cbo_Model.Properties.Columns.Add(new LookUpColumnInfo("Product_type", 100, "产品型号"));
                //cbo_Model.Properties.Columns.Add(new LookUpColumnInfo("Define1", 120, "总长度最小值"));

                //cbo_Model.Properties.Columns.Add(new LookUpColumnInfo("Define2", 120, "总长度最小值"));
                //cbo_Model.Properties.Columns.Add(new LookUpColumnInfo("Define3", 120, "总长度最大值"));
                //cbo_Model.Properties.Columns.Add(new LookUpColumnInfo("Define4", 140, "主体长度最小值"));
                //cbo_Model.Properties.Columns.Add(new LookUpColumnInfo("Define5", 140, "主体长度最大值"));
                //cbo_Model.Properties.Columns.Add(new LookUpColumnInfo("Define6", 140, "主体宽度最小值"));
                //cbo_Model.Properties.Columns.Add(new LookUpColumnInfo("Define7", 140, "主体宽度最大值"));
                //cbo_Model.Properties.Columns.Add(new LookUpColumnInfo("Define8", 140, "左极耳边距最小值"));
                //cbo_Model.Properties.Columns.Add(new LookUpColumnInfo("Define9", 140, "左极耳边距最大值"));
                //cbo_Model.Properties.Columns.Add(new LookUpColumnInfo("Define10", 140, "右极耳边距最小值"));
                //cbo_Model.Properties.Columns.Add(new LookUpColumnInfo("Define11", 140, "右极耳边距最大值"));
                //cbo_Model.Properties.Columns.Add(new LookUpColumnInfo("Define12", 140, "左1小白胶最小值"));
                //cbo_Model.Properties.Columns.Add(new LookUpColumnInfo("Define13", 140, "左1小白胶最大值"));
                //cbo_Model.Properties.Columns.Add(new LookUpColumnInfo("Define16", 140, "左2小白胶最小值"));
                //cbo_Model.Properties.Columns.Add(new LookUpColumnInfo("Define17", 140, "左2小白胶最大值"));
                //cbo_Model.Properties.Columns.Add(new LookUpColumnInfo("Define18", 140, "右1小白胶最小值"));
                //cbo_Model.Properties.Columns.Add(new LookUpColumnInfo("Define19", 140, "右1小白胶最大值"));
                //cbo_Model.Properties.Columns.Add(new LookUpColumnInfo("Define20", 140, "右2小白胶最小值"));
                //cbo_Model.Properties.Columns.Add(new LookUpColumnInfo("Define21", 140, "右2小白胶最大值"));
                //cbo_Model.Properties.Columns.Add(new LookUpColumnInfo("Define22", 140, "左极耳长度最小值"));
                //cbo_Model.Properties.Columns.Add(new LookUpColumnInfo("Define23", 140, "左极耳长度最大值"));
                //cbo_Model.Properties.Columns.Add(new LookUpColumnInfo("Define24", 140, "右极耳长度最小值"));
                //cbo_Model.Properties.Columns.Add(new LookUpColumnInfo("Define25", 140, "右极耳长度最大值"));
                //cbo_Model.Properties.Columns.Add(new LookUpColumnInfo("Define26", 140, "中间极耳边距最小值"));
                //cbo_Model.Properties.Columns.Add(new LookUpColumnInfo("Define27", 140, "中间极耳边距最大值"));
                //cbo_Model.Properties.Columns.Add(new LookUpColumnInfo("Define28", 140, "中间1小白胶最小值"));
                //cbo_Model.Properties.Columns.Add(new LookUpColumnInfo("Define29", 140, "中间1小白胶最大值"));
                //cbo_Model.Properties.Columns.Add(new LookUpColumnInfo("Define30", 140, "中间2小白胶最小值"));
                //cbo_Model.Properties.Columns.Add(new LookUpColumnInfo("Define31", 140, "中间2小白胶最大值"));
                //cbo_Model.Properties.Columns.Add(new LookUpColumnInfo("Define32", 140, "中间极耳长度最小值"));
                //cbo_Model.Properties.Columns.Add(new LookUpColumnInfo("Define33", 140, "中间极耳长度最大值"));
                //cbo_Model.Properties.Columns.Add(new LookUpColumnInfo("Define34", 100, "厚度最小值"));
                //cbo_Model.Properties.Columns.Add(new LookUpColumnInfo("Define35", 100, "厚度最大值"));
                //cbo_Model.Properties.Columns.Add(new LookUpColumnInfo("Define36", 100, "厚度标定值"));
                //cbo_Model.Properties.Columns.Add(new LookUpColumnInfo("Define37", 100, "厚度K值1"));
                //cbo_Model.Properties.Columns.Add(new LookUpColumnInfo("Define38", 100, "厚度B值1"));
                //cbo_Model.Properties.Columns.Add(new LookUpColumnInfo("Define39", 100, "相关性K值1"));
                //cbo_Model.Properties.Columns.Add(new LookUpColumnInfo("Define40", 100, "相关性K值2"));
                //cbo_Model.Properties.Columns.Add(new LookUpColumnInfo("Define41", 100, "相关性K值3"));
                //cbo_Model.Properties.Columns.Add(new LookUpColumnInfo("Define42", 100, "相关性K值4"));
                //cbo_Model.Properties.Columns.Add(new LookUpColumnInfo("Define43", 100, "相关性B值1"));
                //cbo_Model.Properties.Columns.Add(new LookUpColumnInfo("Define44", 100, "相关性B值2"));
                //cbo_Model.Properties.Columns.Add(new LookUpColumnInfo("Define45", 100, "相关性B值3"));
                //cbo_Model.Properties.Columns.Add(new LookUpColumnInfo("Define46", 100, "相关性B值4"));
                //cbo_Model.Properties.Columns.Add(new LookUpColumnInfo("Define47", 140, "各工位测厚极差"));
                //cbo_Model.Properties.Columns.Add(new LookUpColumnInfo("Define1", 120, "极差检测个数"));
                //cbo_Model.Properties.Columns.Add(new LookUpColumnInfo("Define1", 120, "工位均值报警"));
                //cbo_Model.Properties.Columns.Add(new LookUpColumnInfo("Define1", 140, "工位均值报警公差"));
                //cbo_Model.Properties.Columns.Add(new LookUpColumnInfo("Define1", 100, "创建人"));
                //cbo_Model.Properties.Columns.Add(new LookUpColumnInfo("Define1", 100, "创建时间"));
                var list = ProductTypeBLL.GetList(string.Empty, ref errMsg);

                cbo_Model.Properties.DataSource = list;
                cbo_Model.Properties.DisplayMember = "Product_type";
                cbo_Model.Properties.ValueMember = "Product_type";


                //txtProductNO.Properties.Columns[0].Visible = false;
                //txtProductNO.Properties.Columns[1].Visible = false;
                //txtProductNO.Properties.Columns[2].Visible = false;

                cbo_Model.Properties.DropDownItemHeight = 20;
                cbo_Model.Properties.BestFitMode = BestFitMode.BestFitResizePopup;

                // txtProductNO.Text = FilesManager.GetPrivateProfileString(Application.StartupPath + "\\EquiMachine.ini", "DeviceValidate", "ProductNO", string.Empty);

                //txtStaffID.Text = FilesManager.GetPrivateProfileString(Application.StartupPath + "\\EquiMachine.ini", "DeviceValidate", "StaffID", string.Empty);

                //txtStaffID.Focus();

            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(ex.ToString());
            }
        }


        private void simpleButton1_Click(object sender, EventArgs e)
        {
            txtUserName.Text = string.Empty;
            txtPassWord.Text = string.Empty;
            IsPermission = false;
            Close();
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            try
            {
                if (cbo_Model.Text == "")
                {
                    labTittle.Text = "请选择型号";
                }
                if (cbo_Model.Text.Trim() == "")
                {
                    MessageBox.Show("请选择产品型号");
                    return;
                }
                if (cmbModel.Text.Trim() == "")
                {
                    MessageBox.Show("请选择工艺路线");
                    return;
                }
                if (txtUserName.Text.Trim() == "")
                {
                    MessageBox.Show("请输入账号");
                    return;
                }
                if (Common.IsProductType)
                {
                    if (DeivceEquipmentidPlcInfo.lstDeivceEquipmentidPlcInfos[0].ModelInfo != cbo_Model.Text)
                    {
                        var arrProductType = DeivceEquipmentidPlcInfo.lstDeivceEquipmentidPlcInfos[0].ModelInfo.Split('-');
                        if (arrProductType.Length > 2)
                        {
                            string productType = arrProductType[2];
                            if (productType != cbo_Model.Text)
                            {
                                MessageBox.Show($"当前选择model:{cbo_Model.Text}与MES下发model{DeivceEquipmentidPlcInfo.lstDeivceEquipmentidPlcInfos[0].ModelInfo}不一致，不允许开机生产!");
                                return;
                            }
                        }
                    }
                }
                Model = cmbModel.SelectedIndex;
                int No = 0;
                if (!int.TryParse(txtUserName.Text.Trim(), out No))
                {
                    MessageBox.Show("请输入正确工号!");
                    return;
                }
                if (txtUserName.Text == "123" && txtPassWord.Text == "123")
                {
                    MotionControlVm.OperaterId = txtUserName.Text;
                    ATL.Core.DeivceEquipmentidPlcInfo.lstDeivceEquipmentidPlcInfos[0].ModelInfo = cbo_Model.Text;
                    ATL.MES.UserInfo.UserName = txtUserName.Text;
                    txtUserName.Text = string.Empty;
                    txtPassWord.Text = string.Empty;
                    IsPermission = true;
                    if (!CheckParamsConfig.Instance.ProductMode.Contains(cbo_Model.Text.Trim()) && cbo_Model.Text.Trim() != "")
                    {
                        CheckParamsConfig.Instance.ProductMode += "," + cbo_Model.Text.Trim();
                    }
                    var model = cbo_Model.GetSelectedDataRow() as ProductType;
                    BarcodeLenth = model.BarcodeLenth;
                    CheckParamsConfig.Instance.MinAllBatLength = float.Parse(model.Define1.GetValueOrDefault(0).ToString());//总长度最大值
                    CheckParamsConfig.Instance.MaxAllBatLength = float.Parse(model.Define2.GetValueOrDefault(0).ToString());//总长度最小值
                    CheckParamsConfig.Instance.MinBatLength = float.Parse(model.Define3.GetValueOrDefault(0).ToString());//主体长度最小值
                    CheckParamsConfig.Instance.MaxBatLength = float.Parse(model.Define4.GetValueOrDefault(0).ToString());//主体长度最大值
                    CheckParamsConfig.Instance.MinBatWidth = float.Parse(model.Define5.GetValueOrDefault(0).ToString());//主体宽度最小值
                    CheckParamsConfig.Instance.MaxBatWidth = float.Parse(model.Define6.GetValueOrDefault(0).ToString());//主体宽度最大值

                    CheckParamsConfig.Instance.MinLeftLugMargin = float.Parse(model.Define7.GetValueOrDefault(0).ToString());//左极耳边距最小值
                    CheckParamsConfig.Instance.MaxLeftLugMargin = float.Parse(model.Define8.GetValueOrDefault(0).ToString());//左极耳边距最大值

                    CheckParamsConfig.Instance.MinRightLugMargin = float.Parse(model.Define9.GetValueOrDefault(0).ToString());//右极耳边距最小值
                    CheckParamsConfig.Instance.MaxRightLugMargin = float.Parse(model.Define10.GetValueOrDefault(0).ToString());//右极耳边距最大值

                    CheckParamsConfig.Instance.MinLeft1WhiteGlue = float.Parse(model.Define11.GetValueOrDefault(0).ToString());//左1小白胶最小值
                    CheckParamsConfig.Instance.MaxLeft1WhiteGlue = float.Parse(model.Define12.GetValueOrDefault(0).ToString());//左1小白胶最大值
                    CheckParamsConfig.Instance.MinLeft2WhiteGlue = float.Parse(model.Define13.GetValueOrDefault(0).ToString());//左2小白胶最小值
                    CheckParamsConfig.Instance.MaxLeft2WhiteGlue = float.Parse(model.Define14.GetValueOrDefault(0).ToString());//=左2小白胶最大值

                    CheckParamsConfig.Instance.MinRight1WhiteGlue = float.Parse(model.Define15.GetValueOrDefault(0).ToString());//右1小白胶最小值
                    CheckParamsConfig.Instance.MaxRight1WhiteGlue = float.Parse(model.Define16.GetValueOrDefault(0).ToString());//右1小白胶最大值
                    CheckParamsConfig.Instance.MinRight2WhiteGlue = float.Parse(model.Define17.GetValueOrDefault(0).ToString());//右2小白胶最小值
                    CheckParamsConfig.Instance.MaxRight2WhiteGlue = float.Parse(model.Define18.GetValueOrDefault(0).ToString());//右2小白胶最大值

                    CheckParamsConfig.Instance.MinLeftLugLength = float.Parse(model.Define19.GetValueOrDefault(0).ToString());//左极耳长度最小值
                    CheckParamsConfig.Instance.MaxLeftLugLength = float.Parse(model.Define20.GetValueOrDefault(0).ToString());//左极耳长度最大值

                    CheckParamsConfig.Instance.MinRightLugLength = float.Parse(model.Define21.GetValueOrDefault(0).ToString());//右极耳长度最小值
                    CheckParamsConfig.Instance.MaxRightLugLength = float.Parse(model.Define22.GetValueOrDefault(0).ToString());//右极耳长度最大值

                    CheckParamsConfig.Instance.MinMidLugMargin = float.Parse(model.Define23.GetValueOrDefault(0).ToString());//中间极耳边距最小值
                    CheckParamsConfig.Instance.MaxMidLugMargin = float.Parse(model.Define24.GetValueOrDefault(0).ToString());//中间极耳边距最大值
                    CheckParamsConfig.Instance.MinMid1WhiteGlue = float.Parse(model.Define25.GetValueOrDefault(0).ToString());//中间1小白胶最小值
                    CheckParamsConfig.Instance.MaxMid1WhiteGlue = float.Parse(model.Define26.GetValueOrDefault(0).ToString());//中间1小白胶最大值
                    CheckParamsConfig.Instance.MinMid2WhiteGlue = float.Parse(model.Define27.GetValueOrDefault(0).ToString());//中间2小白胶最小值
                    CheckParamsConfig.Instance.MaxMid2WhiteGlue = float.Parse(model.Define28.GetValueOrDefault(0).ToString());//中间2小白胶最大值
                    CheckParamsConfig.Instance.MinMidLugLength = float.Parse(model.Define29.GetValueOrDefault(0).ToString());//中间极耳长度最小值
                    CheckParamsConfig.Instance.MaxMidLugLength = float.Parse(model.Define30.GetValueOrDefault(0).ToString());//中间极耳长度最大值
                    CheckParamsConfig.Instance.MinThickness = float.Parse(model.Define31.GetValueOrDefault(0).ToString());//厚度最小值
                    CheckParamsConfig.Instance.MaxThickness = float.Parse(model.Define32.GetValueOrDefault(0).ToString());//厚度最大值
                    CheckParamsConfig.Instance.CaliValThickness = float.Parse(model.Define33.GetValueOrDefault(0).ToString());//厚度标定值
                    CheckParamsConfig.Instance.ThicknessKValue = float.Parse(model.Define34.GetValueOrDefault(0).ToString());//厚度K值1
                    CheckParamsConfig.Instance.ThicknessBValue = float.Parse(model.Define35.GetValueOrDefault(0).ToString());//厚度B值1
                    CheckParamsConfig.Instance.CellKValue = float.Parse(model.Define36.GetValueOrDefault(0).ToString());//相关性K值1
                    CheckParamsConfig.Instance.CellKValue2 = float.Parse(model.Define37.GetValueOrDefault(0).ToString());//相关性K值2
                    CheckParamsConfig.Instance.CellKValue3 = float.Parse(model.Define38.GetValueOrDefault(0).ToString());//相关性K值3
                    CheckParamsConfig.Instance.CellKValue4 = float.Parse(model.Define39.GetValueOrDefault(0).ToString());//相关性K值4
                    CheckParamsConfig.Instance.CellBValue = float.Parse(model.Define40.GetValueOrDefault(0).ToString());//相关性B值1
                    CheckParamsConfig.Instance.CellBValue2 = float.Parse(model.Define41.GetValueOrDefault(0).ToString());//相关性B值2
                    CheckParamsConfig.Instance.CellBValue3 = float.Parse(model.Define42.GetValueOrDefault(0).ToString());//相关性B值3
                    CheckParamsConfig.Instance.CellBValue4 = float.Parse(model.Define43.GetValueOrDefault(0).ToString());//相关性B值4
                    CheckParamsConfig.Instance.StationRange = float.Parse(model.Define44.GetValueOrDefault(0).ToString());//各工位测厚极差
                    CheckParamsConfig.Instance.StationRangeNum = int.Parse(model.Define45.GetValueOrDefault(0).ToString());//极差检测个数
                    CheckParamsConfig.Instance.StationWarmingAverage = float.Parse(model.Define46.GetValueOrDefault(0).ToString());//工位均值报警
                    CheckParamsConfig.Instance.StationWarmingTolerance = float.Parse(model.Define47.GetValueOrDefault(0).ToString());//工位均值报警公差
                    CheckParamsConfig.Instance.MinVoltage = float.Parse(model.Define48.GetValueOrDefault(0).ToString());//电压最小值
                    CheckParamsConfig.Instance.MaxVoltage = float.Parse(model.Define49.GetValueOrDefault(0).ToString());//电压最大值
                    CheckParamsConfig.Instance.MinResistance = float.Parse(model.Define50.GetValueOrDefault(0).ToString());//内阻最小值
                    CheckParamsConfig.Instance.MaxResistance = float.Parse(model.Define51.GetValueOrDefault(0).ToString());//内阻最大值
                    CheckParamsConfig.Instance.MinTemperature = float.Parse(model.Define52.GetValueOrDefault(0).ToString());//温度最小值
                    CheckParamsConfig.Instance.MaxTemperature = float.Parse(model.Define53.GetValueOrDefault(0).ToString());//温度最大值
                    CheckParamsConfig.Instance.VoltageCoefficient = float.Parse(model.Define54.GetValueOrDefault(0).ToString());//电压补偿
                    CheckParamsConfig.Instance.ResistanceCoefficient = float.Parse(model.Define55.GetValueOrDefault(0).ToString());//内阻补偿
                    CheckParamsConfig.Instance.TemperatureCoefficient = float.Parse(model.Define56.GetValueOrDefault(0).ToString());//工位1温度补偿
                    CheckParamsConfig.Instance.TemperatureCoefficient2 = float.Parse(model.Define57.GetValueOrDefault(0).ToString());//工位2温度补偿
                    CheckParamsConfig.Instance.RangeOfTemperatrue = float.Parse(model.Define58.GetValueOrDefault(0).ToString());//电池温度和环境温度差
                    CheckParamsConfig.Instance.ResistanceFixedValue = float.Parse(model.Define59.GetValueOrDefault(0).ToString());//不测内阻时内阻设定值
                    CheckParamsConfig.Instance.TemperatureFixedValue = float.Parse(model.Define60.GetValueOrDefault(0).ToString());//不测温度时温度设定值
                    CheckParamsConfig.Instance.Source = float.Parse(model.Define61.GetValueOrDefault(0).ToString());//IV初始值(判断依据4)
                    CheckParamsConfig.Instance.Range = float.Parse(model.Define62.GetValueOrDefault(0).ToString());//IV跳变值(判断一句5)
                    CheckParamsConfig.Instance.MaxIV = float.Parse(model.Define63.GetValueOrDefault(0).ToString());//IV上限
                    CheckParamsConfig.Instance.MinIV = float.Parse(model.Define64.GetValueOrDefault(0).ToString());//IV下限
                    CheckParamsConfig.Instance.ExceptionData1 = float.Parse(model.Define65.GetValueOrDefault(0).ToString());//IV异常值1
                    CheckParamsConfig.Instance.ExceptionData2 = float.Parse(model.Define66.GetValueOrDefault(0).ToString());//IV异常值2
                    CheckParamsConfig.Instance.IvTestTime = int.Parse(model.Define67.GetValueOrDefault(0).ToString());//IV测试时间
                    CheckParamsConfig.Instance.IvStation1Channel = int.Parse(model.Define68.GetValueOrDefault(0).ToString());//工位1对应通道
                    CheckParamsConfig.Instance.IvStation2Channel = int.Parse(model.Define69.GetValueOrDefault(0).ToString());//工位2对应通道
                    CheckParamsConfig.Instance.IvStation3Channel = int.Parse(model.Define70.GetValueOrDefault(0).ToString());//工位3对应通道
                    CheckParamsConfig.Instance.IvStation4Channel = int.Parse(model.Define71.GetValueOrDefault(0).ToString());//工位4对应通道

                    CheckParamsConfig.Instance.MinLengthHead = float.Parse(model.Define72.GetValueOrDefault(0).ToString());//头部最小值
                    CheckParamsConfig.Instance.MaxLengthHead = float.Parse(model.Define73.GetValueOrDefault(0).ToString());//头部最大值
                    CheckParamsConfig.Instance.MinLengthTail = float.Parse(model.Define74.GetValueOrDefault(0).ToString());//尾部最小值
                    CheckParamsConfig.Instance.MaxLengthTail = float.Parse(model.Define75.GetValueOrDefault(0).ToString());//尾部最大值

                    CheckParamsConfig.Instance.TotalLayer = int.Parse(model.Define76.GetValueOrDefault(0).ToString());//AC层数
                    CheckParamsConfig.Instance.RectWidth = int.Parse(model.Define77.GetValueOrDefault(0).ToString());//AC宽
                    CheckParamsConfig.Instance.RectHeight = int.Parse(model.Define78.GetValueOrDefault(0).ToString());//AC高

                    CheckParamsConfig.Instance.TotalLayersBD = int.Parse(model.Define79.GetValueOrDefault(0).ToString());//BD层数
                    CheckParamsConfig.Instance.RectWidthBD = int.Parse(model.Define80.GetValueOrDefault(0).ToString());//BD宽
                    CheckParamsConfig.Instance.RectHeightBD = int.Parse(model.Define81.GetValueOrDefault(0).ToString());//BD高

                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "MaxAllBatLength", model.Define2.GetValueOrDefault(0).ToString());//总长度最小值
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "MinBatLength", model.Define3.GetValueOrDefault(0).ToString());//主体长度最小值
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "MaxBatLength", model.Define4.GetValueOrDefault(0).ToString());//主体长度最大值
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "MinBatWidth", model.Define5.GetValueOrDefault(0).ToString());//主体宽度最小值
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "MaxBatWidth", model.Define6.GetValueOrDefault(0).ToString());//主体宽度最大值

                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "MinLeftLugMargin", model.Define7.GetValueOrDefault(0).ToString());//左极耳边距最小值
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "MaxLeftLugMargin", model.Define8.GetValueOrDefault(0).ToString());//左极耳边距最大值

                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "MinRightLugMargin", model.Define9.GetValueOrDefault(0).ToString());//右极耳边距最小值
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "MaxRightLugMargin", model.Define10.GetValueOrDefault(0).ToString());//右极耳边距最大值

                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "MinLeft1WhiteGlue", model.Define11.GetValueOrDefault(0).ToString());//左1小白胶最小值
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "MaxLeft1WhiteGlue", model.Define12.GetValueOrDefault(0).ToString());//左1小白胶最大值
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "MinLeft2WhiteGlue", model.Define13.GetValueOrDefault(0).ToString());//左2小白胶最小值
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "MaxLeft2WhiteGlue", model.Define14.GetValueOrDefault(0).ToString());//左2小白胶最大值

                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "MinRight1WhiteGlue", model.Define15.GetValueOrDefault(0).ToString());//右1小白胶最小值
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "MaxRight1WhiteGlue", model.Define16.GetValueOrDefault(0).ToString());//右1小白胶最大值
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "MinRight2WhiteGlue", model.Define17.GetValueOrDefault(0).ToString());//右2小白胶最小值
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "MaxRight2WhiteGlue", model.Define18.GetValueOrDefault(0).ToString());//右2小白胶最大值

                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "MinLeftLugLength", model.Define19.GetValueOrDefault(0).ToString());//左极耳长度最小值
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "MaxLeftLugLength", model.Define20.GetValueOrDefault(0).ToString());//左极耳长度最大值

                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "MinRightLugLength", model.Define21.GetValueOrDefault(0).ToString());//右极耳长度最小值
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "MaxRightLugLength", model.Define22.GetValueOrDefault(0).ToString());//右极耳长度最大值

                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "MinMidLugMargin", model.Define23.GetValueOrDefault(0).ToString());//中间极耳边距最小值
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "MaxMidLugMargin", model.Define24.GetValueOrDefault(0).ToString());//中间极耳边距最大值
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "MinMid1WhiteGlue", model.Define25.GetValueOrDefault(0).ToString());//中间1小白胶最小值
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "MaxMid1WhiteGlue", model.Define26.GetValueOrDefault(0).ToString());//中间1小白胶最大值
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "MinMid2WhiteGlue", model.Define27.GetValueOrDefault(0).ToString());//中间2小白胶最小值
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "MaxMid2WhiteGlue", model.Define28.GetValueOrDefault(0).ToString());//中间2小白胶最大值
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "MinMidLugLength", model.Define29.GetValueOrDefault(0).ToString());//中间极耳长度最小值
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "MaxMidLugLength", model.Define30.GetValueOrDefault(0).ToString());//中间极耳长度最大值
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "MinThickness", model.Define31.GetValueOrDefault(0).ToString());//厚度最小值
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "MaxThickness", model.Define32.GetValueOrDefault(0).ToString());//厚度最大值
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "calival_thickness", model.Define33.GetValueOrDefault(0).ToString());//厚度标定值
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "ThicknessKValue", model.Define34.GetValueOrDefault(0).ToString());//厚度K值1
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "ThicknessBValue", model.Define35.GetValueOrDefault(0).ToString());//厚度B值1
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "CellKValue", model.Define36.GetValueOrDefault(0).ToString());//相关性K值1
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "CellKValue2", model.Define37.GetValueOrDefault(0).ToString());//相关性K值2
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "CellKValue3", model.Define38.GetValueOrDefault(0).ToString());//相关性K值3
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "CellKValue4", model.Define39.GetValueOrDefault(0).ToString());//相关性K值4
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "CellBValue", model.Define40.GetValueOrDefault(0).ToString());//相关性B值1
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "CellBValue2", model.Define41.GetValueOrDefault(0).ToString());//相关性B值2
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "CellBValue3", model.Define42.GetValueOrDefault(0).ToString());//相关性B值3
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "CellBValue4", model.Define43.GetValueOrDefault(0).ToString());//相关性B值4
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "StationRange", model.Define44.GetValueOrDefault(0).ToString());//各工位测厚极差
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "StationRangeNum", model.Define45.GetValueOrDefault(0).ToString());//极差检测个数
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "StationWarmingAverage", model.Define46.GetValueOrDefault(0).ToString());//工位均值报警
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "StationWarmingTolerance", model.Define47.GetValueOrDefault(0).ToString());//工位均值报警公差
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "MinVoltage", model.Define48.GetValueOrDefault(0).ToString());//电压最小值
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "MaxVoltage", model.Define49.GetValueOrDefault(0).ToString());//电压最大值
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "MinResistance", model.Define50.GetValueOrDefault(0).ToString());//内阻最小值
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "MaxResistance", model.Define51.GetValueOrDefault(0).ToString());//内阻最大值
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "MinTemperature", model.Define52.GetValueOrDefault(0).ToString());//温度最小值
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "MaxTemperature", model.Define53.GetValueOrDefault(0).ToString());//温度最大值
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "VoltageCoefficient", model.Define54.GetValueOrDefault(0).ToString());//电压补偿
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "ResistanceCoefficient", model.Define55.GetValueOrDefault(0).ToString());//内阻补偿
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "TemperatureCoefficient", model.Define56.GetValueOrDefault(0).ToString());//工位1温度补偿
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "TemperatureCoefficient2", model.Define57.GetValueOrDefault(0).ToString());//工位2温度补偿
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "RangeOfTemperatrue", model.Define58.GetValueOrDefault(0).ToString());//电池温度和环境温度差
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "ResistanceFixedValue", model.Define59.GetValueOrDefault(0).ToString());//不测内阻时内阻设定值
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "TemperatureFixedValue", model.Define60.GetValueOrDefault(0).ToString());//不测温度时温度设定值
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "Source", model.Define61.GetValueOrDefault(0).ToString());//IV初始值(判断依据4)
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "Range", model.Define62.GetValueOrDefault(0).ToString());//IV跳变值(判断一句5)
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "MaxIV", model.Define63.GetValueOrDefault(0).ToString());//IV上限
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "MinIV", model.Define64.GetValueOrDefault(0).ToString());//IV下限
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "ExceptionData1", model.Define65.GetValueOrDefault(0).ToString());//IV异常值1
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "ExceptionData2", model.Define66.GetValueOrDefault(0).ToString());//IV异常值2
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "IvTestTime", model.Define67.GetValueOrDefault(0).ToString());//IV测试时间
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "IvStation1Channel", model.Define68.GetValueOrDefault(0).ToString());//工位1对应通道
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "IvStation2Channel", model.Define69.GetValueOrDefault(0).ToString());//工位2对应通道
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "IvStation3Channel", model.Define70.GetValueOrDefault(0).ToString());//工位3对应通道
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "IvStation4Channel", model.Define71.GetValueOrDefault(0).ToString());//工位4对应通道

                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "min_length_head", model.Define72.GetValueOrDefault(0).ToString());//头部最小值
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "max_length_head", model.Define73.GetValueOrDefault(0).ToString());//头部最大值
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "min_length_tail", model.Define74.GetValueOrDefault(0).ToString());//尾部最小值
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "max_length_tail", model.Define75.GetValueOrDefault(0).ToString());//尾部最大值

                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "total_layer", model.Define76.GetValueOrDefault(0).ToString());//AC层数
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "total_layer_bd", model.Define79.GetValueOrDefault(0).ToString());//BD层数


                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "width", model.Define77.GetValueOrDefault(0).ToString());//AC宽
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "height", model.Define78.GetValueOrDefault(0).ToString());//AC高
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "width_bd", model.Define80.GetValueOrDefault(0).ToString());//BD宽
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "height_bd", model.Define81.GetValueOrDefault(0).ToString());//BD高

                    DialogResult = DialogResult.OK;

                    Close();
                    return;
                }


                //if (PassWord.Text.Trim() == "")
                //{
                //    Tips.Text = "请输入密码";
                //    return;
                //}
                Bis bis = new Bis();
                string strOut = "";
                bool bisResult = true;
                if (!Common.IsProductType)
                {
                    ATL.Core.DeivceEquipmentidPlcInfo.lstDeivceEquipmentidPlcInfos[0].ModelInfo = cbo_Model.Text;
                    bisResult = bis.New_check_operator_ID(txtUserName.Text.Trim(), "后工序", cbo_Nom.Text, cbo_WorkInfo.Text, CheckParamsConfig.Instance.FactoryCode, ref strOut);
                }
                if (bisResult)
                {
                    ATL.Station.Station.Current.MESState = "OK";
                    ATL.MES.UserInfo.UserName = txtUserName.Text;
                    MotionControlVm.OperaterId = txtUserName.Text;

                    ATL.Station.Station.Current.MESState = "OK";
                    ATL.MES.UserInfo.UserName = txtUserName.Text;
                    MotionControlVm.OperaterId = txtUserName.Text;

                    txtUserName.Text = string.Empty;
                    txtPassWord.Text = string.Empty;
                    IsPermission = true;
                    if (!CheckParamsConfig.Instance.ProductMode.Contains(cbo_Model.Text.Trim()) && cbo_Model.Text.Trim() != "")
                    {
                        CheckParamsConfig.Instance.ProductMode += "," + cbo_Model.Text.Trim();
                    }
                    CheckParamsConfig.Instance.FactoryCode = txt_FactoryId.Text.Trim();
                    var model = cbo_Model.GetSelectedDataRow() as ProductType;
                    BarcodeLenth = model.BarcodeLenth;

                    CheckParamsConfig.Instance.MinAllBatLength = float.Parse(model.Define1.GetValueOrDefault(0).ToString());//总长度最大值
                    CheckParamsConfig.Instance.MaxAllBatLength = float.Parse(model.Define2.GetValueOrDefault(0).ToString());//总长度最小值
                    CheckParamsConfig.Instance.MinBatLength = float.Parse(model.Define3.GetValueOrDefault(0).ToString());//主体长度最小值
                    CheckParamsConfig.Instance.MaxBatLength = float.Parse(model.Define4.GetValueOrDefault(0).ToString());//主体长度最大值
                    CheckParamsConfig.Instance.MinBatWidth = float.Parse(model.Define5.GetValueOrDefault(0).ToString());//主体宽度最小值
                    CheckParamsConfig.Instance.MaxBatWidth = float.Parse(model.Define6.GetValueOrDefault(0).ToString());//主体宽度最大值

                    CheckParamsConfig.Instance.MinLeftLugMargin = float.Parse(model.Define7.GetValueOrDefault(0).ToString());//左极耳边距最小值
                    CheckParamsConfig.Instance.MaxLeftLugMargin = float.Parse(model.Define8.GetValueOrDefault(0).ToString());//左极耳边距最大值

                    CheckParamsConfig.Instance.MinRightLugMargin = float.Parse(model.Define9.GetValueOrDefault(0).ToString());//右极耳边距最小值
                    CheckParamsConfig.Instance.MaxRightLugMargin = float.Parse(model.Define10.GetValueOrDefault(0).ToString());//右极耳边距最大值

                    CheckParamsConfig.Instance.MinLeft1WhiteGlue = float.Parse(model.Define11.GetValueOrDefault(0).ToString());//左1小白胶最小值
                    CheckParamsConfig.Instance.MaxLeft1WhiteGlue = float.Parse(model.Define12.GetValueOrDefault(0).ToString());//左1小白胶最大值
                    CheckParamsConfig.Instance.MinLeft2WhiteGlue = float.Parse(model.Define13.GetValueOrDefault(0).ToString());//左2小白胶最小值
                    CheckParamsConfig.Instance.MaxLeft2WhiteGlue = float.Parse(model.Define14.GetValueOrDefault(0).ToString());//=左2小白胶最大值

                    CheckParamsConfig.Instance.MinRight1WhiteGlue = float.Parse(model.Define15.GetValueOrDefault(0).ToString());//右1小白胶最小值
                    CheckParamsConfig.Instance.MaxRight1WhiteGlue = float.Parse(model.Define16.GetValueOrDefault(0).ToString());//右1小白胶最大值
                    CheckParamsConfig.Instance.MinRight2WhiteGlue = float.Parse(model.Define17.GetValueOrDefault(0).ToString());//右2小白胶最小值
                    CheckParamsConfig.Instance.MaxRight2WhiteGlue = float.Parse(model.Define18.GetValueOrDefault(0).ToString());//右2小白胶最大值

                    CheckParamsConfig.Instance.MinLeftLugLength = float.Parse(model.Define19.GetValueOrDefault(0).ToString());//左极耳长度最小值
                    CheckParamsConfig.Instance.MaxLeftLugLength = float.Parse(model.Define20.GetValueOrDefault(0).ToString());//左极耳长度最大值

                    CheckParamsConfig.Instance.MinRightLugLength = float.Parse(model.Define21.GetValueOrDefault(0).ToString());//右极耳长度最小值
                    CheckParamsConfig.Instance.MaxRightLugLength = float.Parse(model.Define22.GetValueOrDefault(0).ToString());//右极耳长度最大值

                    CheckParamsConfig.Instance.MinMidLugMargin = float.Parse(model.Define23.GetValueOrDefault(0).ToString());//中间极耳边距最小值
                    CheckParamsConfig.Instance.MaxMidLugMargin = float.Parse(model.Define24.GetValueOrDefault(0).ToString());//中间极耳边距最大值
                    CheckParamsConfig.Instance.MinMid1WhiteGlue = float.Parse(model.Define25.GetValueOrDefault(0).ToString());//中间1小白胶最小值
                    CheckParamsConfig.Instance.MaxMid1WhiteGlue = float.Parse(model.Define26.GetValueOrDefault(0).ToString());//中间1小白胶最大值
                    CheckParamsConfig.Instance.MinMid2WhiteGlue = float.Parse(model.Define27.GetValueOrDefault(0).ToString());//中间2小白胶最小值
                    CheckParamsConfig.Instance.MaxMid2WhiteGlue = float.Parse(model.Define28.GetValueOrDefault(0).ToString());//中间2小白胶最大值
                    CheckParamsConfig.Instance.MinMidLugLength = float.Parse(model.Define29.GetValueOrDefault(0).ToString());//中间极耳长度最小值
                    CheckParamsConfig.Instance.MaxMidLugLength = float.Parse(model.Define30.GetValueOrDefault(0).ToString());//中间极耳长度最大值
                    CheckParamsConfig.Instance.MinThickness = float.Parse(model.Define31.GetValueOrDefault(0).ToString());//厚度最小值
                    CheckParamsConfig.Instance.MaxThickness = float.Parse(model.Define32.GetValueOrDefault(0).ToString());//厚度最大值
                    CheckParamsConfig.Instance.CaliValThickness = float.Parse(model.Define33.GetValueOrDefault(0).ToString());//厚度标定值
                    CheckParamsConfig.Instance.ThicknessKValue = float.Parse(model.Define34.GetValueOrDefault(0).ToString());//厚度K值1
                    CheckParamsConfig.Instance.ThicknessBValue = float.Parse(model.Define35.GetValueOrDefault(0).ToString());//厚度B值1
                    CheckParamsConfig.Instance.CellKValue = float.Parse(model.Define36.GetValueOrDefault(0).ToString());//相关性K值1
                    CheckParamsConfig.Instance.CellKValue2 = float.Parse(model.Define37.GetValueOrDefault(0).ToString());//相关性K值2
                    CheckParamsConfig.Instance.CellKValue3 = float.Parse(model.Define38.GetValueOrDefault(0).ToString());//相关性K值3
                    CheckParamsConfig.Instance.CellKValue4 = float.Parse(model.Define39.GetValueOrDefault(0).ToString());//相关性K值4
                    CheckParamsConfig.Instance.CellBValue = float.Parse(model.Define40.GetValueOrDefault(0).ToString());//相关性B值1
                    CheckParamsConfig.Instance.CellBValue2 = float.Parse(model.Define41.GetValueOrDefault(0).ToString());//相关性B值2
                    CheckParamsConfig.Instance.CellBValue3 = float.Parse(model.Define42.GetValueOrDefault(0).ToString());//相关性B值3
                    CheckParamsConfig.Instance.CellBValue4 = float.Parse(model.Define43.GetValueOrDefault(0).ToString());//相关性B值4
                    CheckParamsConfig.Instance.StationRange = float.Parse(model.Define44.GetValueOrDefault(0).ToString());//各工位测厚极差
                    CheckParamsConfig.Instance.StationRangeNum = int.Parse(model.Define45.GetValueOrDefault(0).ToString());//极差检测个数
                    CheckParamsConfig.Instance.StationWarmingAverage = float.Parse(model.Define46.GetValueOrDefault(0).ToString());//工位均值报警
                    CheckParamsConfig.Instance.StationWarmingTolerance = float.Parse(model.Define47.GetValueOrDefault(0).ToString());//工位均值报警公差
                    CheckParamsConfig.Instance.MinVoltage = float.Parse(model.Define48.GetValueOrDefault(0).ToString());//电压最小值
                    CheckParamsConfig.Instance.MaxVoltage = float.Parse(model.Define49.GetValueOrDefault(0).ToString());//电压最大值
                    CheckParamsConfig.Instance.MinResistance = float.Parse(model.Define50.GetValueOrDefault(0).ToString());//内阻最小值
                    CheckParamsConfig.Instance.MaxResistance = float.Parse(model.Define51.GetValueOrDefault(0).ToString());//内阻最大值
                    CheckParamsConfig.Instance.MinTemperature = float.Parse(model.Define52.GetValueOrDefault(0).ToString());//温度最小值
                    CheckParamsConfig.Instance.MaxTemperature = float.Parse(model.Define53.GetValueOrDefault(0).ToString());//温度最大值
                    CheckParamsConfig.Instance.VoltageCoefficient = float.Parse(model.Define54.GetValueOrDefault(0).ToString());//电压补偿
                    CheckParamsConfig.Instance.ResistanceCoefficient = float.Parse(model.Define55.GetValueOrDefault(0).ToString());//内阻补偿
                    CheckParamsConfig.Instance.TemperatureCoefficient = float.Parse(model.Define56.GetValueOrDefault(0).ToString());//工位1温度补偿
                    CheckParamsConfig.Instance.TemperatureCoefficient2 = float.Parse(model.Define57.GetValueOrDefault(0).ToString());//工位2温度补偿
                    CheckParamsConfig.Instance.RangeOfTemperatrue = float.Parse(model.Define58.GetValueOrDefault(0).ToString());//电池温度和环境温度差
                    CheckParamsConfig.Instance.ResistanceFixedValue = float.Parse(model.Define59.GetValueOrDefault(0).ToString());//不测内阻时内阻设定值
                    CheckParamsConfig.Instance.TemperatureFixedValue = float.Parse(model.Define60.GetValueOrDefault(0).ToString());//不测温度时温度设定值
                    CheckParamsConfig.Instance.Source = float.Parse(model.Define61.GetValueOrDefault(0).ToString());//IV初始值(判断依据4)
                    CheckParamsConfig.Instance.Range = float.Parse(model.Define62.GetValueOrDefault(0).ToString());//IV跳变值(判断一句5)
                    CheckParamsConfig.Instance.MaxIV = float.Parse(model.Define63.GetValueOrDefault(0).ToString());//IV上限
                    CheckParamsConfig.Instance.MinIV = float.Parse(model.Define64.GetValueOrDefault(0).ToString());//IV下限
                    CheckParamsConfig.Instance.ExceptionData1 = float.Parse(model.Define65.GetValueOrDefault(0).ToString());//IV异常值1
                    CheckParamsConfig.Instance.ExceptionData2 = float.Parse(model.Define66.GetValueOrDefault(0).ToString());//IV异常值2
                    CheckParamsConfig.Instance.IvTestTime = int.Parse(model.Define67.GetValueOrDefault(0).ToString());//IV测试时间
                    CheckParamsConfig.Instance.IvStation1Channel = int.Parse(model.Define68.GetValueOrDefault(0).ToString());//工位1对应通道
                    CheckParamsConfig.Instance.IvStation2Channel = int.Parse(model.Define69.GetValueOrDefault(0).ToString());//工位2对应通道
                    CheckParamsConfig.Instance.IvStation3Channel = int.Parse(model.Define70.GetValueOrDefault(0).ToString());//工位3对应通道
                    CheckParamsConfig.Instance.IvStation4Channel = int.Parse(model.Define71.GetValueOrDefault(0).ToString());//工位4对应通道

                    CheckParamsConfig.Instance.MinLengthHead = float.Parse(model.Define72.GetValueOrDefault(0).ToString());//头部最小值
                    CheckParamsConfig.Instance.MaxLengthHead = float.Parse(model.Define73.GetValueOrDefault(0).ToString());//头部最大值
                    CheckParamsConfig.Instance.MinLengthTail = float.Parse(model.Define74.GetValueOrDefault(0).ToString());//尾部最小值
                    CheckParamsConfig.Instance.MaxLengthTail = float.Parse(model.Define75.GetValueOrDefault(0).ToString());//尾部最大值

                    CheckParamsConfig.Instance.TotalLayer = int.Parse(model.Define76.GetValueOrDefault(0).ToString());//AC层数
                    CheckParamsConfig.Instance.TotalLayersBD = int.Parse(model.Define79.GetValueOrDefault(0).ToString());//BD层数

                    CheckParamsConfig.Instance.RectWidth = int.Parse(model.Define77.GetValueOrDefault(0).ToString());//AC宽
                    CheckParamsConfig.Instance.RectHeight = int.Parse(model.Define78.GetValueOrDefault(0).ToString());//AC高

                    CheckParamsConfig.Instance.RectWidthBD = int.Parse(model.Define80.GetValueOrDefault(0).ToString());//BD宽
                    CheckParamsConfig.Instance.RectHeightBD = int.Parse(model.Define81.GetValueOrDefault(0).ToString());//BD高

                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "MaxAllBatLength", model.Define2.GetValueOrDefault(0).ToString());//总长度最小值
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "MinBatLength", model.Define3.GetValueOrDefault(0).ToString());//主体长度最小值
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "MaxBatLength", model.Define4.GetValueOrDefault(0).ToString());//主体长度最大值
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "MinBatWidth", model.Define5.GetValueOrDefault(0).ToString());//主体宽度最小值
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "MaxBatWidth", model.Define6.GetValueOrDefault(0).ToString());//主体宽度最大值

                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "MinLeftLugMargin", model.Define7.GetValueOrDefault(0).ToString());//左极耳边距最小值
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "MaxLeftLugMargin", model.Define8.GetValueOrDefault(0).ToString());//左极耳边距最大值

                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "MinRightLugMargin", model.Define9.GetValueOrDefault(0).ToString());//右极耳边距最小值
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "MaxRightLugMargin", model.Define10.GetValueOrDefault(0).ToString());//右极耳边距最大值

                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "MinLeft1WhiteGlue", model.Define11.GetValueOrDefault(0).ToString());//左1小白胶最小值
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "MaxLeft1WhiteGlue", model.Define12.GetValueOrDefault(0).ToString());//左1小白胶最大值
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "MinLeft2WhiteGlue", model.Define13.GetValueOrDefault(0).ToString());//左2小白胶最小值
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "MaxLeft2WhiteGlue", model.Define14.GetValueOrDefault(0).ToString());//左2小白胶最大值

                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "MinRight1WhiteGlue", model.Define15.GetValueOrDefault(0).ToString());//右1小白胶最小值
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "MaxRight1WhiteGlue", model.Define16.GetValueOrDefault(0).ToString());//右1小白胶最大值
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "MinRight2WhiteGlue", model.Define17.GetValueOrDefault(0).ToString());//右2小白胶最小值
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "MaxRight2WhiteGlue", model.Define18.GetValueOrDefault(0).ToString());//右2小白胶最大值

                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "MinLeftLugLength", model.Define19.GetValueOrDefault(0).ToString());//左极耳长度最小值
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "MaxLeftLugLength", model.Define20.GetValueOrDefault(0).ToString());//左极耳长度最大值

                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "MinRightLugLength", model.Define21.GetValueOrDefault(0).ToString());//右极耳长度最小值
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "MaxRightLugLength", model.Define22.GetValueOrDefault(0).ToString());//右极耳长度最大值

                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "MinMidLugMargin", model.Define23.GetValueOrDefault(0).ToString());//中间极耳边距最小值
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "MaxMidLugMargin", model.Define24.GetValueOrDefault(0).ToString());//中间极耳边距最大值
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "MinMid1WhiteGlue", model.Define25.GetValueOrDefault(0).ToString());//中间1小白胶最小值
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "MaxMid1WhiteGlue", model.Define26.GetValueOrDefault(0).ToString());//中间1小白胶最大值
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "MinMid2WhiteGlue", model.Define27.GetValueOrDefault(0).ToString());//中间2小白胶最小值
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "MaxMid2WhiteGlue", model.Define28.GetValueOrDefault(0).ToString());//中间2小白胶最大值
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "MinMidLugLength", model.Define29.GetValueOrDefault(0).ToString());//中间极耳长度最小值
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "MaxMidLugLength", model.Define30.GetValueOrDefault(0).ToString());//中间极耳长度最大值
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "MinThickness", model.Define31.GetValueOrDefault(0).ToString());//厚度最小值
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "MaxThickness", model.Define32.GetValueOrDefault(0).ToString());//厚度最大值
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "calival_thickness", model.Define33.GetValueOrDefault(0).ToString());//厚度标定值
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "ThicknessKValue", model.Define34.GetValueOrDefault(0).ToString());//厚度K值1
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "ThicknessBValue", model.Define35.GetValueOrDefault(0).ToString());//厚度B值1
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "CellKValue", model.Define36.GetValueOrDefault(0).ToString());//相关性K值1
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "CellKValue2", model.Define37.GetValueOrDefault(0).ToString());//相关性K值2
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "CellKValue3", model.Define38.GetValueOrDefault(0).ToString());//相关性K值3
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "CellKValue4", model.Define39.GetValueOrDefault(0).ToString());//相关性K值4
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "CellBValue", model.Define40.GetValueOrDefault(0).ToString());//相关性B值1
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "CellBValue2", model.Define41.GetValueOrDefault(0).ToString());//相关性B值2
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "CellBValue3", model.Define42.GetValueOrDefault(0).ToString());//相关性B值3
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "CellBValue4", model.Define43.GetValueOrDefault(0).ToString());//相关性B值4
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "StationRange", model.Define44.GetValueOrDefault(0).ToString());//各工位测厚极差
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "StationRangeNum", model.Define45.GetValueOrDefault(0).ToString());//极差检测个数
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "StationWarmingAverage", model.Define46.GetValueOrDefault(0).ToString());//工位均值报警
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "StationWarmingTolerance", model.Define47.GetValueOrDefault(0).ToString());//工位均值报警公差
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "MinVoltage", model.Define48.GetValueOrDefault(0).ToString());//电压最小值
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "MaxVoltage", model.Define49.GetValueOrDefault(0).ToString());//电压最大值
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "MinResistance", model.Define50.GetValueOrDefault(0).ToString());//内阻最小值
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "MaxResistance", model.Define51.GetValueOrDefault(0).ToString());//内阻最大值
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "MinTemperature", model.Define52.GetValueOrDefault(0).ToString());//温度最小值
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "MaxTemperature", model.Define53.GetValueOrDefault(0).ToString());//温度最大值
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "VoltageCoefficient", model.Define54.GetValueOrDefault(0).ToString());//电压补偿
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "ResistanceCoefficient", model.Define55.GetValueOrDefault(0).ToString());//内阻补偿
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "TemperatureCoefficient", model.Define56.GetValueOrDefault(0).ToString());//工位1温度补偿
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "TemperatureCoefficient2", model.Define57.GetValueOrDefault(0).ToString());//工位2温度补偿
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "RangeOfTemperatrue", model.Define58.GetValueOrDefault(0).ToString());//电池温度和环境温度差
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "ResistanceFixedValue", model.Define59.GetValueOrDefault(0).ToString());//不测内阻时内阻设定值
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "TemperatureFixedValue", model.Define60.GetValueOrDefault(0).ToString());//不测温度时温度设定值
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "Source", model.Define61.GetValueOrDefault(0).ToString());//IV初始值(判断依据4)
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "Range", model.Define62.GetValueOrDefault(0).ToString());//IV跳变值(判断一句5)
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "MaxIV", model.Define63.GetValueOrDefault(0).ToString());//IV上限
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "MinIV", model.Define64.GetValueOrDefault(0).ToString());//IV下限
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "ExceptionData1", model.Define65.GetValueOrDefault(0).ToString());//IV异常值1
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "ExceptionData2", model.Define66.GetValueOrDefault(0).ToString());//IV异常值2
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "IvTestTime", model.Define67.GetValueOrDefault(0).ToString());//IV测试时间
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "IvStation1Channel", model.Define68.GetValueOrDefault(0).ToString());//工位1对应通道
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "IvStation2Channel", model.Define69.GetValueOrDefault(0).ToString());//工位2对应通道
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "IvStation3Channel", model.Define70.GetValueOrDefault(0).ToString());//工位3对应通道
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "IvStation4Channel", model.Define71.GetValueOrDefault(0).ToString());//工位4对应通道

                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "min_length_head", model.Define72.GetValueOrDefault(0).ToString());//头部最小值
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "max_length_head", model.Define73.GetValueOrDefault(0).ToString());//头部最大值
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "min_length_tail", model.Define74.GetValueOrDefault(0).ToString());//尾部最小值
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "max_length_tail", model.Define75.GetValueOrDefault(0).ToString());//尾部最大值

                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "total_layer", model.Define76.GetValueOrDefault(0).ToString());//AC层数
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "total_layer_bd", model.Define79.GetValueOrDefault(0).ToString());//BD层数


                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "width", model.Define77.GetValueOrDefault(0).ToString());//AC宽
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "height", model.Define78.GetValueOrDefault(0).ToString());//AC高
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "width_bd", model.Define80.GetValueOrDefault(0).ToString());//BD宽
                    FilesManager.WritePrivateProfilestring(Application.StartupPath + "\\CheckParamsConfig.ini", "InspectParamsConfig", "height_bd", model.Define81.GetValueOrDefault(0).ToString());//BD高

                    DialogResult = DialogResult.OK;

                    Close();
                    return;
                }

                //if (string.IsNullOrEmpty(txtStaffID.Text))
                //{
                //    MessageBox.Show("工号不能为空!");
                //    txtStaffID.Focus();
                //    return;
                //}
                //if (string.IsNullOrEmpty(txtPwd.Text)) {
                //    frmTip.MessageBox("密码不能为空!");
                //    waitAlarm.Close();
                //    txtProductNO.Focus();
                //    return;
                //}
                if (string.IsNullOrEmpty(cbo_Model.Text))
                {
                    MessageBox.Show("产品型号不能为空!");
                    cbo_Model.Focus();
                    return;
                }


            }
            catch (Exception ex)
            {
                MessageBox.Show($"登录发生错误:" + ex.ToString());

            }
        }

        Point mouseOff;//鼠标移动位置变量
        bool leftFlag;//标签是否为左键
        private void frmLogin_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                mouseOff = new Point(-e.X, -e.Y); //得到变量的值
                leftFlag = true;                  //点击左键按下时标注为true;
            }
        }

        private void frmLogin_MouseMove(object sender, MouseEventArgs e)
        {
            if (leftFlag)
            {
                Point mouseSet = Control.MousePosition;
                mouseSet.Offset(mouseOff.X, mouseOff.Y);  //设置移动后的位置
                Location = mouseSet;
            }
        }

        private void frmLogin_MouseUp(object sender, MouseEventArgs e)
        {
            if (leftFlag)
            {
                leftFlag = false;//释放鼠标后标注为false;
            }
        }

        private void txtProductNO_EditValueChanged(object sender, EventArgs e)
        {
            var model = cbo_Model.GetSelectedDataRow() as ProductType;
            //if (ucArgConfig1.LstArg.Count >= 11)
            //{

            //    ucArgConfig1.LstArg[0].ArgDesc = "DEGAS_1";
            //    ucArgConfig1.LstArg[0].ArgVal = model.MinWeight.GetValueOrDefault(0).ToString();
            //    ucArgConfig1.LstArg[0].Remark = "前称下限";


            //    ucArgConfig1.LstArg[1].ArgDesc = "DEGAS_2";
            //    ucArgConfig1.LstArg[1].ArgVal = model.MaxWeight.GetValueOrDefault(0).ToString();
            //    ucArgConfig1.LstArg[1].Remark = "前称上限";

            //    ucArgConfig1.LstArg[2].ArgDesc = "DEGAS_3";
            //    ucArgConfig1.LstArg[2].ArgVal = model.AfterWeight_LSL.GetValueOrDefault(0).ToString();
            //    ucArgConfig1.LstArg[2].Remark = "后称下限";

            //    ucArgConfig1.LstArg[3].ArgDesc = "DEGAS_4";
            //    ucArgConfig1.LstArg[3].ArgVal = model.AfterWeight_USL.GetValueOrDefault(0).ToString();
            //    ucArgConfig1.LstArg[3].Remark = "后称上限";

            //    //ucArgConfig1.LstArg[4].ArgDesc = "DEGAS_5";
            //    //ucArgConfig1.LstArg[4].ArgVal = Common.Logon.InjVolume.ToString();
            //    //ucArgConfig1.LstArg[4].Remark = "切气袋重量";

            //    //ucArgConfig1.LstArg[5].ArgDesc = "DEGAS_6";
            //    //ucArgConfig1.LstArg[5].ArgVal = Common.Inject_USL.ToString();
            //    //ucArgConfig1.LstArg[5].Remark = "抽液量下限";

            //    //ucArgConfig1.LstArg[6].ArgDesc = "DEGAS_7";
            //    //ucArgConfig1.LstArg[6].ArgVal = Common.Inject_LSL.ToString();
            //    //ucArgConfig1.LstArg[6].Remark = "抽液量上限";

            //    ucArgConfig1.LstArg[7].ArgDesc = "DEGAS_8";
            //    ucArgConfig1.LstArg[7].ArgVal = model.InjectRetention_USL.GetValueOrDefault(0).ToString();
            //    ucArgConfig1.LstArg[7].Remark = "保液量下限";

            //    ucArgConfig1.LstArg[8].ArgDesc = "DEGAS_9";
            //    ucArgConfig1.LstArg[8].ArgVal = model.InjectRetention_LSL.GetValueOrDefault(0).ToString();
            //    ucArgConfig1.LstArg[8].Remark = "保液量上限";

            //    ucArgConfig1.LstArg[9].ArgDesc = "DEGAS_10";
            //    ucArgConfig1.LstArg[9].ArgVal = model.Thickness_LSL.GetValueOrDefault(0).ToString();
            //    ucArgConfig1.LstArg[9].Remark = "测厚下限";

            //    ucArgConfig1.LstArg[10].ArgDesc = "DEGAS_11";
            //    ucArgConfig1.LstArg[10].ArgVal = model.Thickness_USL.GetValueOrDefault(0).ToString();
            //    ucArgConfig1.LstArg[10].Remark = "测厚上限";

            //}
        }

        private void cmbModel_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbModel.SelectedIndex == 7)
            {
                txtUserName.Text = "123";
                txtPassWord.Text = "123";
            }
        }
    }
}
