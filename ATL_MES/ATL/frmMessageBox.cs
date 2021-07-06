using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PTF.ATL
{
    public partial class frmMessageBox : Form
    {
        private int currentX;//横坐标         
        private int currentY;//纵坐标         
        private int screenHeight;//屏幕高度         
        private int screenWidth;//屏幕宽度    

        int AW_ACTIVE = 0x20000; //激活窗口，在使用了AW_HIDE标志后不要使用这个标志      
        int AW_HIDE = 0x10000;//隐藏窗口      
        int AW_BLEND = 0x80000;// 使用淡入淡出效果      
        int AW_SLIDE = 0x40000;//使用滑动类型动画效果，默认为滚动动画类型，当使用AW_CENTER标志时，这个标志就被忽略      
        int AW_CENTER = 0x0010;//若使用了AW_HIDE标志，则使窗口向内重叠；否则向外扩展      
        int AW_HOR_POSITIVE = 0x0001;//自左向右显示窗口，该标志可以在滚动动画和滑动动画中使用。使用AW_CENTER标志时忽略该标志      
        int AW_HOR_NEGATIVE = 0x0002;//自右向左显示窗口，该标志可以在滚动动画和滑动动画中使用。使用AW_CENTER标志时忽略该标志      
        int AW_VER_POSITIVE = 0x0004;//自顶向下显示窗口，该标志可以在滚动动画和滑动动画中使用。使用AW_CENTER标志时忽略该标志      
        int AW_VER_NEGATIVE = 0x0008;//自下向上显示窗口，该标志可以在滚动动画和滑动动画中使用。使用AW_CENTER标志时忽略该标志   

        public frmMessageBox()
        {
            InitializeComponent();
            //this.Visible = false;
        }
        [DllImport("user32 ")]

        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("user32 ")]
        private static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);
        [DllImport("user32.dll")]
        //hwnd窗口句柄.dateTime:动画时长.dwFlags:动画类型组合 
        private static extern bool AnimateWindow(IntPtr hwnd, int dateTime, int dwFlags);

        [DllImportAttribute("user32.dll")]
        private extern static bool ReleaseCapture();

        [DllImportAttribute("user32.dll")]
        private extern static int SendMessage(IntPtr handle, int m, int p, int h);

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            AnimateWindow(this.Handle, 2000, AW_HIDE | AW_BLEND);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            IntPtr hDeskTop = FindWindow("Progman ", "Program Manager");
            SetParent(this.Handle, hDeskTop);
            //timerClose.Enabled = true; 
            ThreadPool.QueueUserWorkItem(state => whileForm());

        }
        private void whileForm()
        {
            while (true)
            {
                this.BeginInvoke(new Action(() =>
                {
                    Rectangle rect = Screen.PrimaryScreen.WorkingArea;
                    screenHeight = rect.Height;
                    screenWidth = rect.Width;
                    currentX = screenWidth - this.Width;
                    currentY = screenHeight - this.Height;
                    this.Location = new System.Drawing.Point(currentX - 10, currentY);
                    AnimateWindow(this.Handle, 1000, AW_SLIDE | AW_VER_NEGATIVE);
                    Application.DoEvents();
                    IntPtr hDeskTop = FindWindow("Progman ", "Program Manager");
                    SetParent(this.Handle, hDeskTop);
                }));

                Thread.Sleep(5000);
            }
        }
        private void timer_Close(object sender, EventArgs e)
        {

            Rectangle rect = Screen.PrimaryScreen.WorkingArea;
            screenHeight = rect.Height;
            screenWidth = rect.Width;
            currentX = screenWidth - this.Width;
            currentY = screenHeight - this.Height;
            this.Location = new System.Drawing.Point(currentX - 10, currentY);
            AnimateWindow(this.Handle, 1000, AW_SLIDE | AW_VER_NEGATIVE);
            Application.DoEvents();

        }

        private void Form1_MouseLeave(object sender, EventArgs e)
        {
            timerClose.Enabled = true;
        }

        private void Form1_MouseEnter(object sender, EventArgs e)
        {
            timerClose.Enabled = false;
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            //MessageBox.Show("您的Q币木有了！");
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.Cursor = Cursors.SizeAll;
                ReleaseCapture();
                SendMessage(this.Handle, 0xA1, 0x2, 0);
                this.Cursor = Cursors.Default;
            }
        }


        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            Form1_MouseDown(sender, e);
        }

        private void label1_MouseDown(object sender, MouseEventArgs e)
        {
            Form1_MouseDown(sender, e);
        }

        private void panel2_MouseDown(object sender, MouseEventArgs e)
        {
            Form1_MouseDown(sender, e);
        }

        private void panel1_MouseEnter(object sender, EventArgs e)
        {
            timerClose.Enabled = false;
        }

        private void label1_MouseEnter(object sender, EventArgs e)
        {
            timerClose.Enabled = false;
        }

        private void label1_MouseLeave(object sender, EventArgs e)
        {
            timerClose.Enabled = true;
        }

        private void panel1_MouseLeave(object sender, EventArgs e)
        {
            timerClose.Enabled = true;
        }

        private void pictureBox1_MouseEnter(object sender, EventArgs e)
        {
            timerClose.Enabled = false;
        }

        private void pictureBox1_MouseLeave(object sender, EventArgs e)
        {
            timerClose.Enabled = true;
        }

    }
}
