namespace ZYXray.Winform
{
    partial class frmAtlLogin
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.labTittle = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnLoginOK = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.btnLoginCancel = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.labelControl4 = new DevExpress.XtraEditors.LabelControl();
            this.cbo_Model = new DevExpress.XtraEditors.LookUpEdit();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.txt_PcName = new DevExpress.XtraEditors.TextEdit();
            this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
            this.txt_MachinNo = new DevExpress.XtraEditors.TextEdit();
            this.cbo_Nom = new DevExpress.XtraEditors.ComboBoxEdit();
            this.labelControl3 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl5 = new DevExpress.XtraEditors.LabelControl();
            this.cbo_WorkInfo = new DevExpress.XtraEditors.ComboBoxEdit();
            this.labelControl6 = new DevExpress.XtraEditors.LabelControl();
            this.txt_FactoryId = new DevExpress.XtraEditors.TextEdit();
            this.labelControl7 = new DevExpress.XtraEditors.LabelControl();
            this.txtUserName = new DevExpress.XtraEditors.TextEdit();
            this.labelControl8 = new DevExpress.XtraEditors.LabelControl();
            this.txtPassWord = new DevExpress.XtraEditors.TextEdit();
            this.labelControl9 = new DevExpress.XtraEditors.LabelControl();
            this.cmbModel = new DevExpress.XtraEditors.ComboBoxEdit();
            this.label1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cbo_Model.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txt_PcName.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txt_MachinNo.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbo_Nom.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbo_WorkInfo.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txt_FactoryId.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtUserName.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPassWord.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbModel.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // labTittle
            // 
            this.labTittle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(155)))), ((int)(((byte)(219)))));
            this.labTittle.Dock = System.Windows.Forms.DockStyle.Top;
            this.labTittle.Font = new System.Drawing.Font("微软雅黑", 14F);
            this.labTittle.ForeColor = System.Drawing.Color.White;
            this.labTittle.Location = new System.Drawing.Point(0, 0);
            this.labTittle.Name = "labTittle";
            this.labTittle.Size = new System.Drawing.Size(671, 40);
            this.labTittle.TabIndex = 9;
            this.labTittle.Text = "开机校验";
            this.labTittle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.labTittle.MouseDown += new System.Windows.Forms.MouseEventHandler(this.frmLogin_MouseDown);
            this.labTittle.MouseMove += new System.Windows.Forms.MouseEventHandler(this.frmLogin_MouseMove);
            this.labTittle.MouseUp += new System.Windows.Forms.MouseEventHandler(this.frmLogin_MouseUp);
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(229)))), ((int)(((byte)(239)))), ((int)(((byte)(247)))));
            this.label1.Controls.Add(this.panel1);
            this.label1.Controls.Add(this.panel2);
            this.label1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.label1.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(0, 249);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(671, 40);
            this.label1.TabIndex = 10;
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(229)))), ((int)(((byte)(239)))), ((int)(((byte)(247)))));
            this.panel1.Controls.Add(this.btnLoginOK);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(297, 40);
            this.panel1.TabIndex = 18;
            // 
            // btnLoginOK
            // 
            this.btnLoginOK.BackColor = System.Drawing.Color.PaleGreen;
            this.btnLoginOK.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnLoginOK.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold);
            this.btnLoginOK.ForeColor = System.Drawing.Color.Black;
            this.btnLoginOK.Location = new System.Drawing.Point(153, 2);
            this.btnLoginOK.Name = "btnLoginOK";
            this.btnLoginOK.Size = new System.Drawing.Size(100, 35);
            this.btnLoginOK.TabIndex = 11;
            this.btnLoginOK.Text = "确认";
            this.btnLoginOK.UseVisualStyleBackColor = false;
            this.btnLoginOK.Click += new System.EventHandler(this.simpleButton2_Click);
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(229)))), ((int)(((byte)(239)))), ((int)(((byte)(247)))));
            this.panel2.Controls.Add(this.btnLoginCancel);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel2.Location = new System.Drawing.Point(369, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(302, 40);
            this.panel2.TabIndex = 19;
            // 
            // btnLoginCancel
            // 
            this.btnLoginCancel.BackColor = System.Drawing.Color.LightPink;
            this.btnLoginCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnLoginCancel.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnLoginCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold);
            this.btnLoginCancel.ForeColor = System.Drawing.Color.Black;
            this.btnLoginCancel.Location = new System.Drawing.Point(58, 2);
            this.btnLoginCancel.Name = "btnLoginCancel";
            this.btnLoginCancel.Size = new System.Drawing.Size(100, 35);
            this.btnLoginCancel.TabIndex = 12;
            this.btnLoginCancel.Text = "取消";
            this.btnLoginCancel.UseVisualStyleBackColor = false;
            this.btnLoginCancel.Click += new System.EventHandler(this.simpleButton1_Click);
            // 
            // label2
            // 
            this.label2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.label2.Location = new System.Drawing.Point(0, 247);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(671, 2);
            this.label2.TabIndex = 15;
            // 
            // labelControl4
            // 
            this.labelControl4.Appearance.Font = new System.Drawing.Font("微软雅黑", 12F);
            this.labelControl4.Appearance.Options.UseFont = true;
            this.labelControl4.Location = new System.Drawing.Point(338, 56);
            this.labelControl4.Name = "labelControl4";
            this.labelControl4.Size = new System.Drawing.Size(68, 21);
            this.labelControl4.TabIndex = 17;
            this.labelControl4.Text = "产品型号:";
            // 
            // cbo_Model
            // 
            this.cbo_Model.Location = new System.Drawing.Point(436, 52);
            this.cbo_Model.Name = "cbo_Model";
            this.cbo_Model.Properties.Appearance.Font = new System.Drawing.Font("微软雅黑", 12F);
            this.cbo_Model.Properties.Appearance.Options.UseFont = true;
            this.cbo_Model.Properties.AutoHeight = false;
            this.cbo_Model.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cbo_Model.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[] {
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Iden", "ID", 17, DevExpress.Utils.FormatType.None, "", true, DevExpress.Utils.HorzAlignment.Center, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default),
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Product_type", "产品型号", 105, DevExpress.Utils.FormatType.None, "", true, DevExpress.Utils.HorzAlignment.Center, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default),
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("BarcodeLenth", "条码前缀", 17, DevExpress.Utils.FormatType.None, "", true, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default),
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("MI", "MI", 17, DevExpress.Utils.FormatType.None, "", true, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default),
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Define1", "总长度最小值", 114, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Center, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default),
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Define2", "总长度最大值", 437, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Center, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default),
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Define3", "主体长度最小值", 122, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Center, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default),
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Define4", "主体长度最大值", 122, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Center, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default),
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Define5", "主体宽度最小值", 114, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Center, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default),
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Define6", "主体宽度最大值", 114, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Center, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default),
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Define7", "左极耳边距最小值", 122, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Center, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default),
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Define8", "左极耳边距最大值", 122, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Center, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default),
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Define9", "右极耳边距最小值", 122, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Center, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default),
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Define10", "右极耳边距最大值", 122, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Center, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default),
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Define11", "左1小白胶最小值", 122, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Center, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default),
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Define12", "左1小白胶最大值", 122, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Center, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default),
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Define13", "左2小白胶最小值", 122, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Center, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default),
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Define14", "左2小白胶最大值", 122, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Center, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default),
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Define15", "右1小白胶最小值", 122, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Center, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default),
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Define16", "右1小白胶最大值", 122, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Center, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default),
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Define17", "右2小白胶最小值", 122, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Center, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default),
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Define18", "右2小白胶最大值", 122, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Center, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default),
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Define19", "左极耳长度最小值", 122, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Center, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default),
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Define20", "左极耳长度最大值", 122, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Center, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default),
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Define21", "右极耳长度最小值", 122, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Center, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default),
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Define22", "右极耳长度最大值", 122, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Center, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default),
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Define23", "中间极耳边距最小值", 131, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Center, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default),
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Define24", "中间极耳边距最大值", 131, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Center, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default),
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Define25", "中间1小白胶最小值", 122, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Center, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default),
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Define26", "中间1小白胶最大值", 122, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Center, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default),
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Define27", "中间2小白胶最小值", 122, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Center, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default),
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Define28", "中间2小白胶最大值", 122, DevExpress.Utils.FormatType.None, "", true, DevExpress.Utils.HorzAlignment.Center, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default),
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Define29", "中间极耳长度最小值", 131, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Center, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default),
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Define30", "中间极耳长度最大值", 131, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Center, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default),
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Define31", "厚度最小值", 114, DevExpress.Utils.FormatType.None, "", true, DevExpress.Utils.HorzAlignment.Center, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default),
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Define32", "厚度最大值", 114, DevExpress.Utils.FormatType.None, "", true, DevExpress.Utils.HorzAlignment.Center, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default),
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Define33", "厚度标定值", 114, DevExpress.Utils.FormatType.None, "", true, DevExpress.Utils.HorzAlignment.Center, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default),
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Define34", "厚度K值1", 105, DevExpress.Utils.FormatType.None, "", true, DevExpress.Utils.HorzAlignment.Center, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default),
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Define35", "厚度B值1", 105, DevExpress.Utils.FormatType.None, "", true, DevExpress.Utils.HorzAlignment.Center, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default),
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Define36", "相关性K值1", 105, DevExpress.Utils.FormatType.None, "", true, DevExpress.Utils.HorzAlignment.Center, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default),
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Define37", "相关性K值2", 105, DevExpress.Utils.FormatType.None, "", true, DevExpress.Utils.HorzAlignment.Center, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default),
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Define38", "相关性K值3", 105, DevExpress.Utils.FormatType.None, "", true, DevExpress.Utils.HorzAlignment.Center, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default),
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Define39", "相关性K值4", 105, DevExpress.Utils.FormatType.None, "", true, DevExpress.Utils.HorzAlignment.Center, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default),
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Define40", "相关性B值1", 105, DevExpress.Utils.FormatType.None, "", true, DevExpress.Utils.HorzAlignment.Center, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default),
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Define41", "相关性B值2", 105, DevExpress.Utils.FormatType.None, "", true, DevExpress.Utils.HorzAlignment.Center, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default),
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Define42", "相关性B值3", 105, DevExpress.Utils.FormatType.None, "", true, DevExpress.Utils.HorzAlignment.Center, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default),
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Define43", "相关性B值4", 105, DevExpress.Utils.FormatType.None, "", true, DevExpress.Utils.HorzAlignment.Center, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default),
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Define44", "各工位测厚极差", 122, DevExpress.Utils.FormatType.None, "", true, DevExpress.Utils.HorzAlignment.Center, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default),
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Define45", "极差检测个数", 122, DevExpress.Utils.FormatType.None, "", true, DevExpress.Utils.HorzAlignment.Center, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default),
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Define46", "工位均值报警", 122, DevExpress.Utils.FormatType.None, "", true, DevExpress.Utils.HorzAlignment.Center, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default),
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Define47", "工位均值报警公差", 131, DevExpress.Utils.FormatType.None, "", true, DevExpress.Utils.HorzAlignment.Center, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)});
            this.cbo_Model.Properties.NullText = "";
            this.cbo_Model.Size = new System.Drawing.Size(216, 29);
            this.cbo_Model.TabIndex = 4;
            this.cbo_Model.EditValueChanged += new System.EventHandler(this.txtProductNO_EditValueChanged);
            // 
            // labelControl1
            // 
            this.labelControl1.Appearance.Font = new System.Drawing.Font("微软雅黑", 12F);
            this.labelControl1.Appearance.Options.UseFont = true;
            this.labelControl1.Location = new System.Drawing.Point(24, 56);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(52, 21);
            this.labelControl1.TabIndex = 17;
            this.labelControl1.Text = "主机名:";
            // 
            // txt_PcName
            // 
            this.txt_PcName.Enabled = false;
            this.txt_PcName.Location = new System.Drawing.Point(96, 52);
            this.txt_PcName.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txt_PcName.Name = "txt_PcName";
            this.txt_PcName.Properties.AutoHeight = false;
            this.txt_PcName.Size = new System.Drawing.Size(216, 29);
            this.txt_PcName.TabIndex = 1;
            // 
            // labelControl2
            // 
            this.labelControl2.Appearance.Font = new System.Drawing.Font("微软雅黑", 12F);
            this.labelControl2.Appearance.Options.UseFont = true;
            this.labelControl2.Location = new System.Drawing.Point(6, 93);
            this.labelControl2.Name = "labelControl2";
            this.labelControl2.Size = new System.Drawing.Size(68, 21);
            this.labelControl2.TabIndex = 17;
            this.labelControl2.Text = "设备编号:";
            // 
            // txt_MachinNo
            // 
            this.txt_MachinNo.Enabled = false;
            this.txt_MachinNo.Location = new System.Drawing.Point(96, 89);
            this.txt_MachinNo.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txt_MachinNo.Name = "txt_MachinNo";
            this.txt_MachinNo.Properties.AutoHeight = false;
            this.txt_MachinNo.Size = new System.Drawing.Size(216, 29);
            this.txt_MachinNo.TabIndex = 2;
            // 
            // cbo_Nom
            // 
            this.cbo_Nom.Location = new System.Drawing.Point(436, 127);
            this.cbo_Nom.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.cbo_Nom.Name = "cbo_Nom";
            this.cbo_Nom.Properties.AutoHeight = false;
            this.cbo_Nom.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cbo_Nom.Properties.Items.AddRange(new object[] {
            "OCV测试",
            "最终目检"});
            this.cbo_Nom.Size = new System.Drawing.Size(216, 29);
            this.cbo_Nom.TabIndex = 5;
            // 
            // labelControl3
            // 
            this.labelControl3.Appearance.Font = new System.Drawing.Font("微软雅黑", 12F);
            this.labelControl3.Appearance.Options.UseFont = true;
            this.labelControl3.Location = new System.Drawing.Point(338, 131);
            this.labelControl3.Name = "labelControl3";
            this.labelControl3.Size = new System.Drawing.Size(68, 21);
            this.labelControl3.TabIndex = 17;
            this.labelControl3.Text = "产品工序:";
            // 
            // labelControl5
            // 
            this.labelControl5.Appearance.Font = new System.Drawing.Font("微软雅黑", 12F);
            this.labelControl5.Appearance.Options.UseFont = true;
            this.labelControl5.Location = new System.Drawing.Point(338, 167);
            this.labelControl5.Name = "labelControl5";
            this.labelControl5.Size = new System.Drawing.Size(68, 21);
            this.labelControl5.TabIndex = 17;
            this.labelControl5.Text = "产品岗位:";
            // 
            // cbo_WorkInfo
            // 
            this.cbo_WorkInfo.Location = new System.Drawing.Point(436, 163);
            this.cbo_WorkInfo.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.cbo_WorkInfo.Name = "cbo_WorkInfo";
            this.cbo_WorkInfo.Properties.AutoHeight = false;
            this.cbo_WorkInfo.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cbo_WorkInfo.Properties.Items.AddRange(new object[] {
            "自动OCV测试",
            "自动X-RAY测试"});
            this.cbo_WorkInfo.Size = new System.Drawing.Size(216, 29);
            this.cbo_WorkInfo.TabIndex = 6;
            // 
            // labelControl6
            // 
            this.labelControl6.Appearance.Font = new System.Drawing.Font("微软雅黑", 12F);
            this.labelControl6.Appearance.Options.UseFont = true;
            this.labelControl6.Location = new System.Drawing.Point(9, 131);
            this.labelControl6.Name = "labelControl6";
            this.labelControl6.Size = new System.Drawing.Size(68, 21);
            this.labelControl6.TabIndex = 17;
            this.labelControl6.Text = "工厂代码:";
            // 
            // txt_FactoryId
            // 
            this.txt_FactoryId.Enabled = false;
            this.txt_FactoryId.Location = new System.Drawing.Point(96, 127);
            this.txt_FactoryId.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txt_FactoryId.Name = "txt_FactoryId";
            this.txt_FactoryId.Properties.AutoHeight = false;
            this.txt_FactoryId.Size = new System.Drawing.Size(216, 29);
            this.txt_FactoryId.TabIndex = 3;
            // 
            // labelControl7
            // 
            this.labelControl7.Appearance.Font = new System.Drawing.Font("微软雅黑", 12F);
            this.labelControl7.Appearance.Options.UseFont = true;
            this.labelControl7.Location = new System.Drawing.Point(9, 204);
            this.labelControl7.Name = "labelControl7";
            this.labelControl7.Size = new System.Drawing.Size(68, 21);
            this.labelControl7.TabIndex = 17;
            this.labelControl7.Text = "登录账号:";
            // 
            // txtUserName
            // 
            this.txtUserName.Location = new System.Drawing.Point(96, 200);
            this.txtUserName.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtUserName.Name = "txtUserName";
            this.txtUserName.Properties.AutoHeight = false;
            this.txtUserName.Size = new System.Drawing.Size(216, 29);
            this.txtUserName.TabIndex = 7;
            // 
            // labelControl8
            // 
            this.labelControl8.Appearance.Font = new System.Drawing.Font("微软雅黑", 12F);
            this.labelControl8.Appearance.Options.UseFont = true;
            this.labelControl8.Location = new System.Drawing.Point(338, 204);
            this.labelControl8.Name = "labelControl8";
            this.labelControl8.Size = new System.Drawing.Size(68, 21);
            this.labelControl8.TabIndex = 17;
            this.labelControl8.Text = "登录密码:";
            // 
            // txtPassWord
            // 
            this.txtPassWord.Location = new System.Drawing.Point(436, 200);
            this.txtPassWord.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtPassWord.Name = "txtPassWord";
            this.txtPassWord.Properties.AutoHeight = false;
            this.txtPassWord.Properties.PasswordChar = '*';
            this.txtPassWord.Size = new System.Drawing.Size(216, 29);
            this.txtPassWord.TabIndex = 8;
            // 
            // labelControl9
            // 
            this.labelControl9.Appearance.Font = new System.Drawing.Font("微软雅黑", 12F);
            this.labelControl9.Appearance.Options.UseFont = true;
            this.labelControl9.Location = new System.Drawing.Point(338, 93);
            this.labelControl9.Name = "labelControl9";
            this.labelControl9.Size = new System.Drawing.Size(68, 21);
            this.labelControl9.TabIndex = 17;
            this.labelControl9.Text = "工艺路线:";
            // 
            // cmbModel
            // 
            this.cmbModel.Location = new System.Drawing.Point(436, 89);
            this.cmbModel.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.cmbModel.Name = "cmbModel";
            this.cmbModel.Properties.AutoHeight = false;
            this.cmbModel.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cmbModel.Properties.Items.AddRange(new object[] {
            "OB - XRay - FQI",
            "OC - XRay - FQI",
            "O4 - XRay - FQI",
            "XRay - FQI",
            "O4 - FQI",
            "OG - FQI",
            "FQI",
            "OB - XRAY",
            "OCVG",
            "离线生产"});
            this.cmbModel.Size = new System.Drawing.Size(216, 29);
            this.cmbModel.TabIndex = 5;
            this.cmbModel.SelectedIndexChanged += new System.EventHandler(this.cmbModel_SelectedIndexChanged);
            // 
            // frmAtlLogin
            // 
            this.AcceptButton = this.btnLoginOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnLoginCancel;
            this.ClientSize = new System.Drawing.Size(671, 289);
            this.Controls.Add(this.cbo_WorkInfo);
            this.Controls.Add(this.cmbModel);
            this.Controls.Add(this.cbo_Nom);
            this.Controls.Add(this.txtPassWord);
            this.Controls.Add(this.labelControl8);
            this.Controls.Add(this.txtUserName);
            this.Controls.Add(this.labelControl7);
            this.Controls.Add(this.txt_FactoryId);
            this.Controls.Add(this.labelControl6);
            this.Controls.Add(this.txt_MachinNo);
            this.Controls.Add(this.labelControl2);
            this.Controls.Add(this.txt_PcName);
            this.Controls.Add(this.labelControl1);
            this.Controls.Add(this.labelControl5);
            this.Controls.Add(this.cbo_Model);
            this.Controls.Add(this.labelControl9);
            this.Controls.Add(this.labelControl3);
            this.Controls.Add(this.labelControl4);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.labTittle);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.IconOptions.ShowIcon = false;
            this.Name = "frmAtlLogin";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Load += new System.EventHandler(this.frmTip_Load);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.frmLogin_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.frmLogin_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.frmLogin_MouseUp);
            this.label1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.cbo_Model.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txt_PcName.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txt_MachinNo.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbo_Nom.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbo_WorkInfo.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txt_FactoryId.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtUserName.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPassWord.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbModel.Properties)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labTittle;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnLoginOK;
        private System.Windows.Forms.Button btnLoginCancel;
        private DevExpress.XtraEditors.LabelControl labelControl4;
        private DevExpress.XtraEditors.LookUpEdit cbo_Model;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.TextEdit txt_PcName;
        private DevExpress.XtraEditors.LabelControl labelControl2;
        private DevExpress.XtraEditors.TextEdit txt_MachinNo;
        private DevExpress.XtraEditors.ComboBoxEdit cbo_Nom;
        private DevExpress.XtraEditors.LabelControl labelControl3;
        private DevExpress.XtraEditors.LabelControl labelControl5;
        private DevExpress.XtraEditors.ComboBoxEdit cbo_WorkInfo;
        private DevExpress.XtraEditors.LabelControl labelControl6;
        private DevExpress.XtraEditors.TextEdit txt_FactoryId;
        private DevExpress.XtraEditors.LabelControl labelControl7;
        private DevExpress.XtraEditors.TextEdit txtUserName;
        private DevExpress.XtraEditors.LabelControl labelControl8;
        private DevExpress.XtraEditors.TextEdit txtPassWord;
        private DevExpress.XtraEditors.LabelControl labelControl9;
        private DevExpress.XtraEditors.ComboBoxEdit cmbModel;
    }
}