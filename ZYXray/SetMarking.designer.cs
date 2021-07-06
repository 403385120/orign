namespace ZYXray
{
    partial class SetMarking
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.lb_MarkingBase = new System.Windows.Forms.ListBox();
            this.lb_MarkingCurrent = new System.Windows.Forms.ListBox();
            this.btn_AddMarkingToBase = new System.Windows.Forms.Button();
            this.txt_AddMarkingToBase = new System.Windows.Forms.TextBox();
            this.btn_RemoveBaseMarking = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.btn_RemoveCurrentMarking = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btn_RemoveBaseMarking);
            this.groupBox1.Controls.Add(this.txt_AddMarkingToBase);
            this.groupBox1.Controls.Add(this.btn_AddMarkingToBase);
            this.groupBox1.Controls.Add(this.lb_MarkingBase);
            this.groupBox1.Location = new System.Drawing.Point(12, 40);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(204, 425);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Marking库";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btn_RemoveCurrentMarking);
            this.groupBox2.Controls.Add(this.lb_MarkingCurrent);
            this.groupBox2.Location = new System.Drawing.Point(390, 40);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(200, 425);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "使用的Marking";
            // 
            // lb_MarkingBase
            // 
            this.lb_MarkingBase.FormattingEnabled = true;
            this.lb_MarkingBase.ItemHeight = 15;
            this.lb_MarkingBase.Location = new System.Drawing.Point(25, 90);
            this.lb_MarkingBase.Name = "lb_MarkingBase";
            this.lb_MarkingBase.Size = new System.Drawing.Size(149, 289);
            this.lb_MarkingBase.TabIndex = 0;
            // 
            // lb_MarkingCurrent
            // 
            this.lb_MarkingCurrent.FormattingEnabled = true;
            this.lb_MarkingCurrent.ItemHeight = 15;
            this.lb_MarkingCurrent.Location = new System.Drawing.Point(24, 90);
            this.lb_MarkingCurrent.Name = "lb_MarkingCurrent";
            this.lb_MarkingCurrent.Size = new System.Drawing.Size(149, 289);
            this.lb_MarkingCurrent.TabIndex = 1;
            // 
            // btn_AddMarkingToBase
            // 
            this.btn_AddMarkingToBase.Location = new System.Drawing.Point(25, 17);
            this.btn_AddMarkingToBase.Name = "btn_AddMarkingToBase";
            this.btn_AddMarkingToBase.Size = new System.Drawing.Size(149, 27);
            this.btn_AddMarkingToBase.TabIndex = 2;
            this.btn_AddMarkingToBase.Text = "增加Marking";
            this.btn_AddMarkingToBase.UseVisualStyleBackColor = true;
            this.btn_AddMarkingToBase.Click += new System.EventHandler(this.btn_AddMarkingToBase_Click);
            // 
            // txt_AddMarkingToBase
            // 
            this.txt_AddMarkingToBase.Location = new System.Drawing.Point(25, 47);
            this.txt_AddMarkingToBase.Name = "txt_AddMarkingToBase";
            this.txt_AddMarkingToBase.Size = new System.Drawing.Size(149, 25);
            this.txt_AddMarkingToBase.TabIndex = 3;
            // 
            // btn_RemoveBaseMarking
            // 
            this.btn_RemoveBaseMarking.Location = new System.Drawing.Point(25, 396);
            this.btn_RemoveBaseMarking.Name = "btn_RemoveBaseMarking";
            this.btn_RemoveBaseMarking.Size = new System.Drawing.Size(149, 27);
            this.btn_RemoveBaseMarking.TabIndex = 4;
            this.btn_RemoveBaseMarking.Text = "删除Marking";
            this.btn_RemoveBaseMarking.UseVisualStyleBackColor = true;
            this.btn_RemoveBaseMarking.Click += new System.EventHandler(this.btn_RemoveBaseMarking_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(222, 217);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(149, 27);
            this.button1.TabIndex = 5;
            this.button1.Text = "使用Marking -->";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // btn_RemoveCurrentMarking
            // 
            this.btn_RemoveCurrentMarking.Location = new System.Drawing.Point(19, 396);
            this.btn_RemoveCurrentMarking.Name = "btn_RemoveCurrentMarking";
            this.btn_RemoveCurrentMarking.Size = new System.Drawing.Size(154, 27);
            this.btn_RemoveCurrentMarking.TabIndex = 5;
            this.btn_RemoveCurrentMarking.Text = "删除Marking";
            this.btn_RemoveCurrentMarking.UseVisualStyleBackColor = true;
            this.btn_RemoveCurrentMarking.Click += new System.EventHandler(this.btn_RemoveCurrentMarking_Click);
            // 
            // SetMarking
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(749, 490);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "SetMarking";
            this.Text = "SetMarking";
            this.Load += new System.EventHandler(this.SetMarking_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ListBox lb_MarkingBase;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ListBox lb_MarkingCurrent;
        private System.Windows.Forms.Button btn_RemoveBaseMarking;
        private System.Windows.Forms.TextBox txt_AddMarkingToBase;
        private System.Windows.Forms.Button btn_AddMarkingToBase;
        private System.Windows.Forms.Button btn_RemoveCurrentMarking;
        private System.Windows.Forms.Button button1;
    }
}