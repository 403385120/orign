namespace ZYXray.Winform
{
    partial class frmRecheckImage
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
            this.pictureEdit1 = new DevExpress.XtraEditors.PictureEdit();
            this.btnPrev = new System.Windows.Forms.Button();
            this.btnNext = new System.Windows.Forms.Button();
            this.btnWaitCheck = new System.Windows.Forms.Button();
            this.btnSubmit = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.btnMarkOK = new System.Windows.Forms.Button();
            this.btnMarkNG = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureEdit1.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureEdit1
            // 
            this.pictureEdit1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureEdit1.Location = new System.Drawing.Point(-1, 0);
            this.pictureEdit1.Name = "pictureEdit1";
            this.pictureEdit1.Properties.ShowCameraMenuItem = DevExpress.XtraEditors.Controls.CameraMenuItemVisibility.Auto;
            this.pictureEdit1.Size = new System.Drawing.Size(1186, 686);
            this.pictureEdit1.TabIndex = 0;
            // 
            // btnPrev
            // 
            this.btnPrev.Location = new System.Drawing.Point(373, 582);
            this.btnPrev.Name = "btnPrev";
            this.btnPrev.Size = new System.Drawing.Size(76, 39);
            this.btnPrev.TabIndex = 1;
            this.btnPrev.Text = "上一组";
            this.btnPrev.UseVisualStyleBackColor = true;
            this.btnPrev.Click += new System.EventHandler(this.btnPrev_Click);
            // 
            // btnNext
            // 
            this.btnNext.Location = new System.Drawing.Point(629, 582);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(76, 39);
            this.btnNext.TabIndex = 1;
            this.btnNext.Text = "下一组";
            this.btnNext.UseVisualStyleBackColor = true;
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            // 
            // btnWaitCheck
            // 
            this.btnWaitCheck.Location = new System.Drawing.Point(57, 582);
            this.btnWaitCheck.Name = "btnWaitCheck";
            this.btnWaitCheck.Size = new System.Drawing.Size(76, 39);
            this.btnWaitCheck.TabIndex = 1;
            this.btnWaitCheck.Text = "待判";
            this.btnWaitCheck.UseVisualStyleBackColor = true;
            this.btnWaitCheck.Click += new System.EventHandler(this.btnWaitCheck_Click);
            // 
            // btnSubmit
            // 
            this.btnSubmit.Location = new System.Drawing.Point(1073, 586);
            this.btnSubmit.Name = "btnSubmit";
            this.btnSubmit.Size = new System.Drawing.Size(76, 39);
            this.btnSubmit.TabIndex = 1;
            this.btnSubmit.Text = "提交结果";
            this.btnSubmit.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(514, 598);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(55, 15);
            this.label1.TabIndex = 2;
            this.label1.Text = "label1";
            // 
            // btnMarkOK
            // 
            this.btnMarkOK.Location = new System.Drawing.Point(507, 538);
            this.btnMarkOK.Name = "btnMarkOK";
            this.btnMarkOK.Size = new System.Drawing.Size(76, 39);
            this.btnMarkOK.TabIndex = 1;
            this.btnMarkOK.Text = "OK";
            this.btnMarkOK.UseVisualStyleBackColor = true;
            this.btnMarkOK.Click += new System.EventHandler(this.btnMarkOK_Click);
            // 
            // btnMarkNG
            // 
            this.btnMarkNG.Location = new System.Drawing.Point(507, 633);
            this.btnMarkNG.Name = "btnMarkNG";
            this.btnMarkNG.Size = new System.Drawing.Size(76, 39);
            this.btnMarkNG.TabIndex = 1;
            this.btnMarkNG.Text = "NG";
            this.btnMarkNG.UseVisualStyleBackColor = true;
            this.btnMarkNG.Click += new System.EventHandler(this.btnMarkNG_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(31, 37);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(55, 15);
            this.label2.TabIndex = 3;
            this.label2.Text = "label2";
            // 
            // FrmRecheckImage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1184, 687);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnSubmit);
            this.Controls.Add(this.btnNext);
            this.Controls.Add(this.btnWaitCheck);
            this.Controls.Add(this.btnMarkNG);
            this.Controls.Add(this.btnMarkOK);
            this.Controls.Add(this.btnPrev);
            this.Controls.Add(this.pictureEdit1);
            this.Name = "FrmRecheckImage";
            this.Text = "人工复判";
            this.Load += new System.EventHandler(this.FrmRecheckImage_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureEdit1.Properties)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraEditors.PictureEdit pictureEdit1;
        private System.Windows.Forms.Button btnPrev;
        private System.Windows.Forms.Button btnNext;
        private System.Windows.Forms.Button btnWaitCheck;
        private System.Windows.Forms.Button btnSubmit;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnMarkOK;
        private System.Windows.Forms.Button btnMarkNG;
        private System.Windows.Forms.Label label2;
    }
}