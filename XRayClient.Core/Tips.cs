using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace XRayClient.Core
{
    public partial class Tips : Form
    {
        public Tips(string mi)
        {
            InitializeComponent();
            label2.Text += mi;
        }
    }
}
