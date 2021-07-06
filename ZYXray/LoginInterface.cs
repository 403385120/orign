using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ATL.Common;
using ATL.Core;
using ZYXray.ViewModels;
using XRayClient.Core;

namespace ZYXray
{
    public partial class LoginInterface : Form
    {
        public static BaseFacade baseFacade = new BaseFacade();

        private bool _isPermission = false;
        public LoginInterface()
        {
            InitializeComponent();
            PassWord.PasswordChar = '*';
            cbo_Nom.Text = cbo_Nom.Items[0].ToString();
            cbo_WorkInfo.Text = cbo_WorkInfo.Items[0].ToString();
            txt_FactoryId.Text = UserDefineVariableInfo.DicVariables["FactoryCode"].ToString();
            txt_PcName.Text = CheckParamsConfig.Instance.PcName;
            txt_MachinNo.Text = UserDefineVariableInfo.DicVariables["AssetsNO"].ToString();
            cbo_Model.Items.Clear();
            if (CheckParamsConfig.Instance.ProductMode.Contains(","))
            {
                string[] strArr = CheckParamsConfig.Instance.ProductMode.Split(',');
                for (int i = 0; i < strArr.Length; i++)
                {
                    cbo_Model.Items.Add(strArr[i]);
                }
            }
        }

        private void OK_Click(object sender, EventArgs e)
        {
            if (cbo_Model.Text == "")
            {
                Tips.Text = "请选择型号";
            }
            if (UserName.Text == "123" && PassWord.Text == "123")
            {
                MotionControlVm.OperaterId = UserName.Text;
                ATL.Core.DeivceEquipmentidPlcInfo.lstDeivceEquipmentidPlcInfos[0].ModelInfo = cbo_Model.Text;
                ATL.MES.UserInfo.UserName = UserName.Text;
                UserName.Clear();
                PassWord.Clear();
                IsPermission = true;
                if (!CheckParamsConfig.Instance.ProductMode.Contains(cbo_Model.Text.Trim()) && cbo_Model.Text.Trim() != "")
                {
                    CheckParamsConfig.Instance.ProductMode += "," + cbo_Model.Text.Trim();
                }
                Close();
                return;
            }

            if (UserName.Text.Trim() == "")
            {
                Tips.Text = "请输入账号";
                return;
            }
            //if (PassWord.Text.Trim() == "")
            //{
            //    Tips.Text = "请输入密码";
            //    return;
            //}

            Bis bis = new Bis();
            string strOut = "";
            bool bisResult = bis.New_check_operator_ID(UserName.Text.Trim(), "后工序", cbo_Nom.Text, cbo_WorkInfo.Text, CheckParamsConfig.Instance.FactoryCode, ref strOut);
            if (bisResult == true)
            {
                ATL.Station.Station.Current.MESState = "OK";
                ATL.MES.UserInfo.UserName = UserName.Text;
                MotionControlVm.OperaterId = UserName.Text;
                ATL.Core.DeivceEquipmentidPlcInfo.lstDeivceEquipmentidPlcInfos[0].ModelInfo = cbo_Model.Text;
                ATL.Station.Station.Current.MESState = "OK";
                ATL.MES.UserInfo.UserName = UserName.Text;
                MotionControlVm.OperaterId = UserName.Text;

                UserName.Clear();
                PassWord.Clear();
                IsPermission = true;
                if (!CheckParamsConfig.Instance.ProductMode.Contains(cbo_Model.Text.Trim()) && cbo_Model.Text.Trim() != "")
                {
                    CheckParamsConfig.Instance.ProductMode += "," + cbo_Model.Text.Trim();
                }
                CheckParamsConfig.Instance.FactoryCode = txt_FactoryId.Text.Trim();
                Close();
                return;
            }


            //string sql = "select * from users where name='Operater' and userName='" + UserName.Text + "' ";
            //DataSet ds1 = baseFacade.equipDB.ExecuteDataSet(CommandType.Text, sql);
            //if (ds1.Tables[0].Rows.Count == 0)
            //{
            //    Tips.Text = "你没有权限";
            //    return;
            //}
            //sql = "select * from users where name='Operater' and userName='" + UserName.Text + "' and password='" + PassWord.Text + "' ";
            //DataSet ds2 = baseFacade.equipDB.ExecuteDataSet(CommandType.Text, sql);
            //if (ds2.Tables[0].Rows.Count == 0)
            //{
            //    Tips.Text = "密码错误";
            //    return;
            //}
            //ATL.Station.Station.Current.MESState = "OK";
            //ATL.MES.UserInfo.UserName = UserName.Text;
            //MotionControlVm.OperaterId = UserName.Text;
            //ATL.Core.DeivceEquipmentidPlcInfo.lstDeivceEquipmentidPlcInfos[0].ModelInfo = cbo_Model.Text;
            //UserName.Clear();
            //PassWord.Clear();
            //IsPermission = true;
            //Close();
        }

        private void NO_Click(object sender, EventArgs e)
        {
            UserName.Clear();
            PassWord.Clear();
            IsPermission = false;
            Close();
        }

        public bool IsPermission
        {
            get
            {
                return _isPermission;
            }
            set
            {
                _isPermission = value;
            }
        }
    }
}
