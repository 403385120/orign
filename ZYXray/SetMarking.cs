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

namespace ZYXray
{
    public partial class SetMarking : Form
    {
        public SetMarking()
        {
            InitializeComponent();
        }

        private void InitialListBox()
        {
            lb_MarkingBase.Items.Clear();
            lb_MarkingCurrent.Items.Clear();
            if (CheckParamsConfig.Instance.MarkingBase.Contains(","))
            {
                string[] arrBase = CheckParamsConfig.Instance.MarkingBase.Split(',');
                for (int i = 0; i < arrBase.Length; i++)
                {
                    if (arrBase[i] != "")
                    {
                        lb_MarkingBase.Items.Add(arrBase[i]);
                    }
                }
                if (lb_MarkingBase.Items.Count > 0)
                {
                    lb_MarkingBase.SelectedIndex = 0;
                }
            }
            if (CheckParamsConfig.Instance.MarkingCurrent.Contains(","))
            {
                string[] arrCurrent = CheckParamsConfig.Instance.MarkingCurrent.Split(',');
                for (int i = 0; i < arrCurrent.Length; i++)
                {
                    if (arrCurrent[i] != "")
                    {
                        lb_MarkingCurrent.Items.Add(arrCurrent[i]);
                    }
                }
                if (lb_MarkingCurrent.Items.Count > 0)
                {
                    lb_MarkingCurrent.SelectedIndex = 0;
                }
            }
        }

        private void SetMarking_Load(object sender, EventArgs e)
        {
            InitialListBox();
        }

        private void btn_AddMarkingToBase_Click(object sender, EventArgs e)
        {
            if (lb_MarkingBase.Items.Count > 0)
            {
                for (int i = 0; i < lb_MarkingBase.Items.Count; i++)
                {
                    if (lb_MarkingBase.Items[i].ToString() == txt_AddMarkingToBase.Text)
                    {
                        MessageBox.Show("该Marking已存在");
                        return;
                    }
                }
            }

            if (txt_AddMarkingToBase.Text != "")
            {
                if (MessageBox.Show("是否添加Marking：" + txt_AddMarkingToBase.Text, "提醒", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                {
                    CheckParamsConfig.Instance.MarkingBase += (txt_AddMarkingToBase.Text.Trim() + ",");
                    InitialListBox();
                    txt_AddMarkingToBase.Clear();
                }
            }
        }

        private void btn_RemoveBaseMarking_Click(object sender, EventArgs e)
        {
            if (lb_MarkingBase.Items.Count > 0)
            {
                string strMarking = lb_MarkingBase.SelectedItem.ToString();
                if (MessageBox.Show("是否删除Marking：" + strMarking, "提醒", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                {
                    CheckParamsConfig.Instance.MarkingBase = CheckParamsConfig.Instance.MarkingBase.Replace(strMarking + ",", "");
                    InitialListBox();
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (lb_MarkingBase.Items.Count > 0)
            {
                if (lb_MarkingCurrent.Items.Count > 0)
                {
                    for (int i = 0; i < lb_MarkingCurrent.Items.Count; i++)
                    {
                        if (lb_MarkingCurrent.Items[i].ToString() == lb_MarkingBase.SelectedItem.ToString())
                        {
                            MessageBox.Show("该Marking正在使用");
                            return;
                        }
                    }
                }
                if (MessageBox.Show("是使用Marking：" + lb_MarkingBase.SelectedItem.ToString(), "提醒", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                {
                    CheckParamsConfig.Instance.MarkingCurrent += lb_MarkingBase.SelectedItem.ToString() + ",";
                    InitialListBox();
                }
            }
        }

        private void btn_RemoveCurrentMarking_Click(object sender, EventArgs e)
        {
            if (lb_MarkingCurrent.Items.Count > 0)
            {
                string strMarking = lb_MarkingCurrent.SelectedItem.ToString();
                if (MessageBox.Show("是否删除Marking：" + strMarking, "提醒", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                {
                    CheckParamsConfig.Instance.MarkingCurrent = CheckParamsConfig.Instance.MarkingCurrent.Replace(strMarking + ",", "");
                    InitialListBox();
                }
            }
        }
    }
}
