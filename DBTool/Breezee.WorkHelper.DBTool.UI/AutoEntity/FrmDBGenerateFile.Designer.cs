namespace Breezee.WorkHelper.DBTool.UI
{
    partial class FrmDBGenerateFile
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmDBGenerateFile));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btnImportAutoFile = new System.Windows.Forms.ToolStripButton();
            this.tsbGenIBDFile = new System.Windows.Forms.ToolStripButton();
            this.btnExit = new System.Windows.Forms.ToolStripButton();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.llModelDown = new System.Windows.Forms.LinkLabel();
            this.label13 = new System.Windows.Forms.Label();
            this.txbIBDClassPre = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.btnIBDSelectPath = new System.Windows.Forms.Button();
            this.txbIBDMainPath = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.groupBox8 = new System.Windows.Forms.GroupBox();
            this.dgvIBDFileList = new System.Windows.Forms.DataGridView();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.dgvLayerName = new System.Windows.Forms.DataGridView();
            this.toolStrip1.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.groupBox8.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvIBDFileList)).BeginInit();
            this.groupBox7.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLayerName)).BeginInit();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnImportAutoFile,
            this.tsbGenIBDFile,
            this.btnExit});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(754, 25);
            this.toolStrip1.TabIndex = 4;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // btnImportAutoFile
            // 
            this.btnImportAutoFile.Image = ((System.Drawing.Image)(resources.GetObject("btnImportAutoFile.Image")));
            this.btnImportAutoFile.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnImportAutoFile.Name = "btnImportAutoFile";
            this.btnImportAutoFile.Size = new System.Drawing.Size(133, 22);
            this.btnImportAutoFile.Text = "导入生成IBD模板(&I)";
            this.btnImportAutoFile.Click += new System.EventHandler(this.btnImportAutoFile_Click);
            // 
            // tsbGenIBDFile
            // 
            this.tsbGenIBDFile.Image = ((System.Drawing.Image)(resources.GetObject("tsbGenIBDFile.Image")));
            this.tsbGenIBDFile.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbGenIBDFile.Name = "tsbGenIBDFile";
            this.tsbGenIBDFile.Size = new System.Drawing.Size(112, 22);
            this.tsbGenIBDFile.Text = "生成IBD文件(&S)";
            this.tsbGenIBDFile.Click += new System.EventHandler(this.tsbGenIBDFile_Click);
            // 
            // btnExit
            // 
            this.btnExit.Image = ((System.Drawing.Image)(resources.GetObject("btnExit.Image")));
            this.btnExit.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(68, 22);
            this.btnExit.Text = "退出(&X)";
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.llModelDown);
            this.groupBox6.Controls.Add(this.label13);
            this.groupBox6.Controls.Add(this.txbIBDClassPre);
            this.groupBox6.Controls.Add(this.label12);
            this.groupBox6.Controls.Add(this.btnIBDSelectPath);
            this.groupBox6.Controls.Add(this.txbIBDMainPath);
            this.groupBox6.Controls.Add(this.label11);
            this.groupBox6.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox6.Location = new System.Drawing.Point(0, 25);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(754, 84);
            this.groupBox6.TabIndex = 5;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "生成设置";
            // 
            // llModelDown
            // 
            this.llModelDown.AutoSize = true;
            this.llModelDown.Location = new System.Drawing.Point(615, 31);
            this.llModelDown.Name = "llModelDown";
            this.llModelDown.Size = new System.Drawing.Size(71, 12);
            this.llModelDown.TabIndex = 11;
            this.llModelDown.TabStop = true;
            this.llModelDown.Text = "IBD模板下载";
            this.llModelDown.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.llModelDown_LinkClicked);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.ForeColor = System.Drawing.Color.Red;
            this.label13.Location = new System.Drawing.Point(210, 59);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(515, 12);
            this.label13.TabIndex = 10;
            this.label13.Text = "可空。为使用跨模块的IOC名称不重复，尽量加前缀。如在导入模板已增加前缀，那么这里留空。";
            // 
            // txbIBDClassPre
            // 
            this.txbIBDClassPre.Location = new System.Drawing.Point(101, 54);
            this.txbIBDClassPre.Name = "txbIBDClassPre";
            this.txbIBDClassPre.Size = new System.Drawing.Size(103, 21);
            this.txbIBDClassPre.TabIndex = 9;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(30, 57);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(65, 12);
            this.label12.TabIndex = 8;
            this.label12.Text = "类名前缀：";
            // 
            // btnIBDSelectPath
            // 
            this.btnIBDSelectPath.Location = new System.Drawing.Point(533, 26);
            this.btnIBDSelectPath.Name = "btnIBDSelectPath";
            this.btnIBDSelectPath.Size = new System.Drawing.Size(33, 23);
            this.btnIBDSelectPath.TabIndex = 7;
            this.btnIBDSelectPath.Text = "...";
            this.btnIBDSelectPath.UseVisualStyleBackColor = true;
            this.btnIBDSelectPath.Click += new System.EventHandler(this.btnIBDSelectPath_Click);
            // 
            // txbIBDMainPath
            // 
            this.txbIBDMainPath.Location = new System.Drawing.Point(101, 28);
            this.txbIBDMainPath.Name = "txbIBDMainPath";
            this.txbIBDMainPath.ReadOnly = true;
            this.txbIBDMainPath.Size = new System.Drawing.Size(426, 21);
            this.txbIBDMainPath.TabIndex = 1;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.ForeColor = System.Drawing.Color.Red;
            this.label11.Location = new System.Drawing.Point(6, 31);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(89, 12);
            this.label11.TabIndex = 0;
            this.label11.Text = "生成的根目录：";
            // 
            // groupBox8
            // 
            this.groupBox8.Controls.Add(this.dgvIBDFileList);
            this.groupBox8.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox8.Location = new System.Drawing.Point(410, 109);
            this.groupBox8.Name = "groupBox8";
            this.groupBox8.Size = new System.Drawing.Size(344, 293);
            this.groupBox8.TabIndex = 7;
            this.groupBox8.TabStop = false;
            this.groupBox8.Text = "导入清单";
            // 
            // dgvIBDFileList
            // 
            this.dgvIBDFileList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvIBDFileList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvIBDFileList.Location = new System.Drawing.Point(3, 17);
            this.dgvIBDFileList.Name = "dgvIBDFileList";
            this.dgvIBDFileList.RowTemplate.Height = 23;
            this.dgvIBDFileList.Size = new System.Drawing.Size(338, 273);
            this.dgvIBDFileList.TabIndex = 0;
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.dgvLayerName);
            this.groupBox7.Dock = System.Windows.Forms.DockStyle.Left;
            this.groupBox7.Location = new System.Drawing.Point(0, 109);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(410, 293);
            this.groupBox7.TabIndex = 6;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "导入清单";
            // 
            // dgvTableList
            // 
            this.dgvLayerName.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvLayerName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvLayerName.Location = new System.Drawing.Point(3, 17);
            this.dgvLayerName.Name = "dgvTableList";
            this.dgvLayerName.RowTemplate.Height = 23;
            this.dgvLayerName.Size = new System.Drawing.Size(404, 273);
            this.dgvLayerName.TabIndex = 0;
            // 
            // FrmDBGenerateFile
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(754, 402);
            this.Controls.Add(this.groupBox8);
            this.Controls.Add(this.groupBox7);
            this.Controls.Add(this.groupBox6);
            this.Controls.Add(this.toolStrip1);
            this.Name = "FrmDBGenerateFile";
            this.Text = "IBD文件生成";
            this.Load += new System.EventHandler(this.FrmDBGenerateFile_Load);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.groupBox8.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvIBDFileList)).EndInit();
            this.groupBox7.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvLayerName)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton btnImportAutoFile;
        private System.Windows.Forms.ToolStripButton tsbGenIBDFile;
        private System.Windows.Forms.ToolStripButton btnExit;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.LinkLabel llModelDown;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox txbIBDClassPre;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Button btnIBDSelectPath;
        private System.Windows.Forms.TextBox txbIBDMainPath;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.GroupBox groupBox8;
        private System.Windows.Forms.DataGridView dgvIBDFileList;
        private System.Windows.Forms.GroupBox groupBox7;
        private System.Windows.Forms.DataGridView dgvLayerName;
    }
}