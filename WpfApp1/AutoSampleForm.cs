using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WpfApp1
{
    public partial class AutoSampleForm : Form
    {
        public AutoSampleForm()
        {
            InitializeComponent();
        }

        private void AutoSampleForm_Load(object sender, EventArgs e)
        {
            loadData();
        }

        public void loadData()
        {

            refresh();

        }

        private void button3_Click(object sender, EventArgs e)
        {
            refresh();
        }

        private void refresh()
        {
            DataTable ds = ModVariable.deviceOverView.getNgChips();

            this.dataGridView1.DataSource = ds;
            this.dataGridView1.Columns[0].Visible = false;
            this.dataGridView1.Columns[1].HeaderText = "电芯二维码";
            this.dataGridView1.Columns[1].Width = 150;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            refresh();
        }
    }
}
