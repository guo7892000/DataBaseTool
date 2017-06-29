namespace Breezee.WorkHelper.WinForm.StartUp
{
    partial class FrmMenuManage
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmMenuManage));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.tsbSave = new System.Windows.Forms.ToolStripButton();
            this.tsbExit = new System.Windows.Forms.ToolStripButton();
            this.tvLeftMenu = new System.Windows.Forms.TreeView();
            this.cmsTreeMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiAddModel = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiAddClass = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiAddFunc = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiDelete = new System.Windows.Forms.ToolStripMenuItem();
            this.grpEdit = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.cbbMenuType = new System.Windows.Forms.ComboBox();
            this.txbMenuCode = new System.Windows.Forms.TextBox();
            this.txbClassFullPath = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.cbbDLL = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txbMenuName = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txbShortCutKey = new System.Windows.Forms.TextBox();
            this.toolStrip1.SuspendLayout();
            this.cmsTreeMenu.SuspendLayout();
            this.grpEdit.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbSave,
            this.tsbExit});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(578, 25);
            this.toolStrip1.TabIndex = 26;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // tsbSave
            // 
            this.tsbSave.Image = ((System.Drawing.Image)(resources.GetObject("tsbSave.Image")));
            this.tsbSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbSave.Name = "tsbSave";
            this.tsbSave.Size = new System.Drawing.Size(67, 22);
            this.tsbSave.Text = "保存(&S)";
            this.tsbSave.Click += new System.EventHandler(this.tsbSave_Click);
            // 
            // tsbExit
            // 
            this.tsbExit.Image = ((System.Drawing.Image)(resources.GetObject("tsbExit.Image")));
            this.tsbExit.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbExit.Name = "tsbExit";
            this.tsbExit.Size = new System.Drawing.Size(68, 22);
            this.tsbExit.Text = "退出(&X)";
            this.tsbExit.Click += new System.EventHandler(this.tsbExit_Click);
            // 
            // tvLeftMenu
            // 
            this.tvLeftMenu.ContextMenuStrip = this.cmsTreeMenu;
            this.tvLeftMenu.Dock = System.Windows.Forms.DockStyle.Left;
            this.tvLeftMenu.Location = new System.Drawing.Point(0, 25);
            this.tvLeftMenu.Name = "tvLeftMenu";
            this.tvLeftMenu.Size = new System.Drawing.Size(175, 317);
            this.tvLeftMenu.TabIndex = 27;
            this.tvLeftMenu.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvLeftMenu_AfterSelect);
            // 
            // cmsTreeMenu
            // 
            this.cmsTreeMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiAddModel,
            this.tsmiAddClass,
            this.tsmiAddFunc,
            this.tsmiDelete});
            this.cmsTreeMenu.Name = "cmsTreeMenu";
            this.cmsTreeMenu.Size = new System.Drawing.Size(149, 92);
            // 
            // tsmiAddModel
            // 
            this.tsmiAddModel.Name = "tsmiAddModel";
            this.tsmiAddModel.Size = new System.Drawing.Size(148, 22);
            this.tsmiAddModel.Text = "新增模块";
            this.tsmiAddModel.Click += new System.EventHandler(this.tsmiAddModel_Click);
            // 
            // tsmiAddClass
            // 
            this.tsmiAddClass.Name = "tsmiAddClass";
            this.tsmiAddClass.Size = new System.Drawing.Size(148, 22);
            this.tsmiAddClass.Text = "新增菜单分类";
            this.tsmiAddClass.Click += new System.EventHandler(this.tsmiAddClass_Click);
            // 
            // tsmiAddFunc
            // 
            this.tsmiAddFunc.Name = "tsmiAddFunc";
            this.tsmiAddFunc.Size = new System.Drawing.Size(148, 22);
            this.tsmiAddFunc.Text = "新增功能";
            this.tsmiAddFunc.Click += new System.EventHandler(this.tsmiAddFunc_Click);
            // 
            // tsmiDelete
            // 
            this.tsmiDelete.Name = "tsmiDelete";
            this.tsmiDelete.Size = new System.Drawing.Size(148, 22);
            this.tsmiDelete.Text = "删除";
            this.tsmiDelete.Click += new System.EventHandler(this.tsmiDelete_Click);
            // 
            // grpEdit
            // 
            this.grpEdit.Controls.Add(this.tableLayoutPanel1);
            this.grpEdit.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpEdit.Location = new System.Drawing.Point(175, 25);
            this.grpEdit.Name = "grpEdit";
            this.grpEdit.Size = new System.Drawing.Size(403, 177);
            this.grpEdit.TabIndex = 28;
            this.grpEdit.TabStop = false;
            this.grpEdit.Text = "菜单维护";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 5;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.label2, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.label3, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.cbbMenuType, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.txbMenuCode, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.txbClassFullPath, 1, 4);
            this.tableLayoutPanel1.Controls.Add(this.label5, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.cbbDLL, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.label4, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.txbMenuName, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.label6, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.txbShortCutKey, 3, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 17);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 6;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(397, 157);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.Red;
            this.label1.Location = new System.Drawing.Point(21, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "菜单类型：";
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(21, 33);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 12);
            this.label2.TabIndex = 0;
            this.label2.Text = "菜单编码：";
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label3.AutoSize = true;
            this.label3.ForeColor = System.Drawing.Color.Red;
            this.label3.Location = new System.Drawing.Point(3, 87);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(83, 12);
            this.label3.TabIndex = 0;
            this.label3.Text = "所在DLL文件：";
            // 
            // cbbMenuType
            // 
            this.cbbMenuType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.cbbMenuType.FormattingEnabled = true;
            this.cbbMenuType.Location = new System.Drawing.Point(92, 3);
            this.cbbMenuType.Name = "cbbMenuType";
            this.cbbMenuType.Size = new System.Drawing.Size(121, 20);
            this.cbbMenuType.TabIndex = 1;
            this.cbbMenuType.SelectedIndexChanged += new System.EventHandler(this.cbbMenuType_SelectedIndexChanged);
            // 
            // txbMenuCode
            // 
            this.txbMenuCode.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txbMenuCode.Location = new System.Drawing.Point(92, 29);
            this.txbMenuCode.Name = "txbMenuCode";
            this.txbMenuCode.Size = new System.Drawing.Size(121, 21);
            this.txbMenuCode.TabIndex = 2;
            // 
            // txbClassFullPath
            // 
            this.txbClassFullPath.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.SetColumnSpan(this.txbClassFullPath, 3);
            this.txbClassFullPath.Location = new System.Drawing.Point(92, 109);
            this.txbClassFullPath.Multiline = true;
            this.txbClassFullPath.Name = "txbClassFullPath";
            this.txbClassFullPath.Size = new System.Drawing.Size(286, 45);
            this.txbClassFullPath.TabIndex = 2;
            // 
            // label5
            // 
            this.label5.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label5.AutoSize = true;
            this.label5.ForeColor = System.Drawing.Color.Red;
            this.label5.Location = new System.Drawing.Point(9, 125);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(77, 12);
            this.label5.TabIndex = 0;
            this.label5.Text = "全路径类名：";
            // 
            // cbbDLL
            // 
            this.cbbDLL.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.SetColumnSpan(this.cbbDLL, 3);
            this.cbbDLL.FormattingEnabled = true;
            this.cbbDLL.Location = new System.Drawing.Point(92, 83);
            this.cbbDLL.Name = "cbbDLL";
            this.cbbDLL.Size = new System.Drawing.Size(286, 20);
            this.cbbDLL.TabIndex = 1;
            // 
            // label4
            // 
            this.label4.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label4.AutoSize = true;
            this.label4.ForeColor = System.Drawing.Color.Red;
            this.label4.Location = new System.Drawing.Point(21, 60);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(65, 12);
            this.label4.TabIndex = 0;
            this.label4.Text = "菜单名称：";
            // 
            // txbMenuName
            // 
            this.txbMenuName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.SetColumnSpan(this.txbMenuName, 3);
            this.txbMenuName.Location = new System.Drawing.Point(92, 56);
            this.txbMenuName.Name = "txbMenuName";
            this.txbMenuName.Size = new System.Drawing.Size(286, 21);
            this.txbMenuName.TabIndex = 2;
            // 
            // label6
            // 
            this.label6.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(219, 33);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(53, 12);
            this.label6.TabIndex = 0;
            this.label6.Text = "快捷键：";
            // 
            // txbShortCutKey
            // 
            this.txbShortCutKey.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txbShortCutKey.Location = new System.Drawing.Point(278, 29);
            this.txbShortCutKey.Name = "txbShortCutKey";
            this.txbShortCutKey.Size = new System.Drawing.Size(100, 21);
            this.txbShortCutKey.TabIndex = 2;
            // 
            // FrmMenuManage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(578, 342);
            this.Controls.Add(this.grpEdit);
            this.Controls.Add(this.tvLeftMenu);
            this.Controls.Add(this.toolStrip1);
            this.DoubleBuffered = true;
            this.Name = "FrmMenuManage";
            this.Text = "菜单维护";
            this.Load += new System.EventHandler(this.FrmMenuManage_Load);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.cmsTreeMenu.ResumeLayout(false);
            this.grpEdit.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton tsbSave;
        private System.Windows.Forms.ToolStripButton tsbExit;
        private System.Windows.Forms.TreeView tvLeftMenu;
        private System.Windows.Forms.GroupBox grpEdit;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cbbMenuType;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txbMenuName;
        private System.Windows.Forms.TextBox txbClassFullPath;
        private System.Windows.Forms.TextBox txbMenuCode;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txbShortCutKey;
        private System.Windows.Forms.ComboBox cbbDLL;
        private System.Windows.Forms.ContextMenuStrip cmsTreeMenu;
        private System.Windows.Forms.ToolStripMenuItem tsmiAddModel;
        private System.Windows.Forms.ToolStripMenuItem tsmiAddClass;
        private System.Windows.Forms.ToolStripMenuItem tsmiAddFunc;
        private System.Windows.Forms.ToolStripMenuItem tsmiDelete;
    }
}