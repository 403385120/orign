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
using XRayClient.Core;

namespace ZYXray
{
    public partial class Login : Form
    {
        public static BaseFacade baseFacade = new BaseFacade();

        private bool _isPermission = false;
        public Login()
        {
            InitializeComponent();
            PassWord.PasswordChar = '*';
        }

        private void OK_Click(object sender, EventArgs e)
        {
            if (UserName.Text.Trim()=="")
            {
                Tips.Text = "请输入账号";
                return;
            }
            if (PassWord.Text.Trim() == "")
            {
                Tips.Text = "请输入密码";
                return;
            }
            if (UserName.Text.Trim() == "qwerty")
            {
                if (PassWord.Text.Trim() != "qwerty")
                {
                    Tips.Text = "密码错误";
                    return;
                }
            }
            else
            {
                string sql = "select * from users where name='update' and userName='" + UserName.Text + "' ";
                DataSet ds1 = baseFacade.equipDB.ExecuteDataSet(CommandType.Text, sql);
                if (ds1.Tables[0].Rows.Count == 0)
                {
                    Tips.Text = "你没有权限";
                    return;
                }
                sql = "select * from users where name='update' and userName='" + UserName.Text + "' and password='" + PassWord.Text + "' ";
                DataSet ds2 = baseFacade.equipDB.ExecuteDataSet(CommandType.Text, sql);
                if (ds2.Tables[0].Rows.Count == 0)
                {
                    Tips.Text = "密码错误";
                    return;
                }
            }
            UserName.Clear();
            PassWord.Clear();
            IsPermission =true;
            Close();
        }

        private void NO_Click(object sender, EventArgs e)
        {
            UserName.Clear();
            PassWord.Clear();
            IsPermission =false;
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
