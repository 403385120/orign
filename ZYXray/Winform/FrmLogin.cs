using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ZYXray.Winform
{
    public partial class FrmLogin : Form
    {
        int type = 0;
        public FrmLogin(int _type)
        {
            InitializeComponent();
            type = _type;
        }
        public static FrmLogin Current;
        public int blLogin = 0;
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (txtName.Text.Trim() == "")
            {
                MessageBox.Show("请输入用户名!");
            }
            switch (type)
            {
                case 1:
                    if (txtName.Text.Trim().ToUpper().Equals("SUPER"))
                    {
                        blLogin = 1;//超级权限
                        this.Hide();
                    }
                    else if (txtName.Text.Trim().ToUpper().Equals("ME2021"))
                    {
                        blLogin = 2;//权限
                        this.Hide();
                    }
                    else
                    {
                        MessageBox.Show("当前用户没有权限操作!");
                    }
                    break;
                default:
                    frmRecheckImage frmrecheck = new frmRecheckImage();
                    frmrecheck.ShowDialog();
                    break;
            }
        }

        private void btnCanel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txtName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnSave_Click(null, null);
            }
        }
    }
}
