namespace Breezee.WorkHelper.WinForm.StartUp
{
    partial class FrmMainMDI
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
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmMainMDI));
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.tsbStartMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiMenuManage = new System.Windows.Forms.ToolStripMenuItem();
            this.tsbRestartMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbExitMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.viewMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.toolBarToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusBarToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.windowsMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.cascadeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tileVerticalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tileHorizontalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.arrangeIconsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.tstbMenuSearch = new System.Windows.Forms.ToolStripTextBox();
            this.tsbAutoGuid = new System.Windows.Forms.ToolStripButton();
            this.saveToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.printToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.printPreviewToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.helpToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.tvLeftMenu = new System.Windows.Forms.TreeView();
            this.pnlLeftMenu = new System.Windows.Forms.Panel();
            this.btnHideTree = new System.Windows.Forms.Button();
            this.pnlMenuNavigate = new System.Windows.Forms.Panel();
            this.txbMenuPath = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tcMenu = new System.Windows.Forms.TabControl();
            this.tpgDesktop = new System.Windows.Forms.TabPage();
            this.pnlDestop = new System.Windows.Forms.Panel();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.cmsMenuTabPositionUp = new System.Windows.Forms.ToolStripMenuItem();
            this.cmsMenuTabPositionDown = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiUserManual = new System.Windows.Forms.ToolStripMenuItem();
            this.helpProvider1 = new System.Windows.Forms.HelpProvider();
            this.menuStrip.SuspendLayout();
            this.toolStrip.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.pnlLeftMenu.SuspendLayout();
            this.pnlMenuNavigate.SuspendLayout();
            this.tcMenu.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbStartMenu,
            this.viewMenu,
            this.windowsMenu,
            this.helpMenu});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.MdiWindowListItem = this.windowsMenu;
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(733, 25);
            this.menuStrip.TabIndex = 0;
            this.menuStrip.Text = "MenuStrip";
            // 
            // tsbStartMenu
            // 
            this.tsbStartMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiMenuManage,
            this.tsbRestartMenu,
            this.toolStripSeparator5,
            this.tsbExitMenu});
            this.tsbStartMenu.ImageTransparentColor = System.Drawing.SystemColors.ActiveBorder;
            this.tsbStartMenu.Name = "tsbStartMenu";
            this.tsbStartMenu.Size = new System.Drawing.Size(60, 21);
            this.tsbStartMenu.Text = "开始(&B)";
            // 
            // tsmiMenuManage
            // 
            this.tsmiMenuManage.Name = "tsmiMenuManage";
            this.tsmiMenuManage.Size = new System.Drawing.Size(124, 22);
            this.tsmiMenuManage.Text = "菜单维护";
            // 
            // tsbRestartMenu
            // 
            this.tsbRestartMenu.Name = "tsbRestartMenu";
            this.tsbRestartMenu.Size = new System.Drawing.Size(124, 22);
            this.tsbRestartMenu.Text = "重新启动";
            this.tsbRestartMenu.Click += new System.EventHandler(this.tsbRestartMenu_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(121, 6);
            // 
            // tsbExitMenu
            // 
            this.tsbExitMenu.Name = "tsbExitMenu";
            this.tsbExitMenu.Size = new System.Drawing.Size(124, 22);
            this.tsbExitMenu.Text = "退出(&X)";
            this.tsbExitMenu.Click += new System.EventHandler(this.tsbExitMenu_Click);
            // 
            // viewMenu
            // 
            this.viewMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolBarToolStripMenuItem,
            this.statusBarToolStripMenuItem});
            this.viewMenu.Name = "viewMenu";
            this.viewMenu.Size = new System.Drawing.Size(60, 21);
            this.viewMenu.Text = "视图(&V)";
            // 
            // toolBarToolStripMenuItem
            // 
            this.toolBarToolStripMenuItem.Checked = true;
            this.toolBarToolStripMenuItem.CheckOnClick = true;
            this.toolBarToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.toolBarToolStripMenuItem.Name = "toolBarToolStripMenuItem";
            this.toolBarToolStripMenuItem.Size = new System.Drawing.Size(127, 22);
            this.toolBarToolStripMenuItem.Text = "工具栏(&T)";
            this.toolBarToolStripMenuItem.Click += new System.EventHandler(this.ToolBarToolStripMenuItem_Click);
            // 
            // statusBarToolStripMenuItem
            // 
            this.statusBarToolStripMenuItem.Checked = true;
            this.statusBarToolStripMenuItem.CheckOnClick = true;
            this.statusBarToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.statusBarToolStripMenuItem.Name = "statusBarToolStripMenuItem";
            this.statusBarToolStripMenuItem.Size = new System.Drawing.Size(127, 22);
            this.statusBarToolStripMenuItem.Text = "状态栏(&S)";
            this.statusBarToolStripMenuItem.Click += new System.EventHandler(this.StatusBarToolStripMenuItem_Click);
            // 
            // windowsMenu
            // 
            this.windowsMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cascadeToolStripMenuItem,
            this.tileVerticalToolStripMenuItem,
            this.tileHorizontalToolStripMenuItem,
            this.closeAllToolStripMenuItem,
            this.arrangeIconsToolStripMenuItem});
            this.windowsMenu.Name = "windowsMenu";
            this.windowsMenu.Size = new System.Drawing.Size(64, 21);
            this.windowsMenu.Text = "窗口(&W)";
            // 
            // cascadeToolStripMenuItem
            // 
            this.cascadeToolStripMenuItem.Name = "cascadeToolStripMenuItem";
            this.cascadeToolStripMenuItem.Size = new System.Drawing.Size(141, 22);
            this.cascadeToolStripMenuItem.Text = "层叠(&C)";
            this.cascadeToolStripMenuItem.Click += new System.EventHandler(this.CascadeToolStripMenuItem_Click);
            // 
            // tileVerticalToolStripMenuItem
            // 
            this.tileVerticalToolStripMenuItem.Name = "tileVerticalToolStripMenuItem";
            this.tileVerticalToolStripMenuItem.Size = new System.Drawing.Size(141, 22);
            this.tileVerticalToolStripMenuItem.Text = "垂直平铺(&V)";
            this.tileVerticalToolStripMenuItem.Click += new System.EventHandler(this.TileVerticalToolStripMenuItem_Click);
            // 
            // tileHorizontalToolStripMenuItem
            // 
            this.tileHorizontalToolStripMenuItem.Name = "tileHorizontalToolStripMenuItem";
            this.tileHorizontalToolStripMenuItem.Size = new System.Drawing.Size(141, 22);
            this.tileHorizontalToolStripMenuItem.Text = "水平平铺(&H)";
            this.tileHorizontalToolStripMenuItem.Click += new System.EventHandler(this.TileHorizontalToolStripMenuItem_Click);
            // 
            // closeAllToolStripMenuItem
            // 
            this.closeAllToolStripMenuItem.Name = "closeAllToolStripMenuItem";
            this.closeAllToolStripMenuItem.Size = new System.Drawing.Size(141, 22);
            this.closeAllToolStripMenuItem.Text = "全部关闭(&L)";
            this.closeAllToolStripMenuItem.Click += new System.EventHandler(this.CloseAllToolStripMenuItem_Click);
            // 
            // arrangeIconsToolStripMenuItem
            // 
            this.arrangeIconsToolStripMenuItem.Name = "arrangeIconsToolStripMenuItem";
            this.arrangeIconsToolStripMenuItem.Size = new System.Drawing.Size(141, 22);
            this.arrangeIconsToolStripMenuItem.Text = "排列图标(&A)";
            this.arrangeIconsToolStripMenuItem.Click += new System.EventHandler(this.ArrangeIconsToolStripMenuItem_Click);
            // 
            // helpMenu
            // 
            this.helpMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiUserManual,
            this.aboutToolStripMenuItem});
            this.helpMenu.Name = "helpMenu";
            this.helpMenu.Size = new System.Drawing.Size(61, 21);
            this.helpMenu.Text = "帮助(&H)";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.aboutToolStripMenuItem.Text = "关于(&A) ...";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // toolStrip
            // 
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabel1,
            this.tstbMenuSearch,
            this.tsbAutoGuid,
            this.saveToolStripButton,
            this.toolStripSeparator1,
            this.printToolStripButton,
            this.printPreviewToolStripButton,
            this.toolStripSeparator2,
            this.helpToolStripButton});
            this.toolStrip.Location = new System.Drawing.Point(0, 25);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Size = new System.Drawing.Size(733, 25);
            this.toolStrip.TabIndex = 1;
            this.toolStrip.Text = "ToolStrip";
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(44, 22);
            this.toolStripLabel1.Text = "搜索：";
            // 
            // tstbMenuSearch
            // 
            this.tstbMenuSearch.Name = "tstbMenuSearch";
            this.tstbMenuSearch.Size = new System.Drawing.Size(150, 25);
            this.tstbMenuSearch.KeyUp += new System.Windows.Forms.KeyEventHandler(this.tstbMenuSearch_KeyUp);
            // 
            // tsbAutoGuid
            // 
            this.tsbAutoGuid.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbAutoGuid.Image = ((System.Drawing.Image)(resources.GetObject("tsbAutoGuid.Image")));
            this.tsbAutoGuid.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbAutoGuid.Name = "tsbAutoGuid";
            this.tsbAutoGuid.Size = new System.Drawing.Size(23, 22);
            this.tsbAutoGuid.Text = "生成GUID";
            this.tsbAutoGuid.Visible = false;
            this.tsbAutoGuid.Click += new System.EventHandler(this.tsbAutoGuid_Click);
            // 
            // saveToolStripButton
            // 
            this.saveToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.saveToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("saveToolStripButton.Image")));
            this.saveToolStripButton.ImageTransparentColor = System.Drawing.Color.Black;
            this.saveToolStripButton.Name = "saveToolStripButton";
            this.saveToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.saveToolStripButton.Text = "保存";
            this.saveToolStripButton.Visible = false;
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // printToolStripButton
            // 
            this.printToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.printToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("printToolStripButton.Image")));
            this.printToolStripButton.ImageTransparentColor = System.Drawing.Color.Black;
            this.printToolStripButton.Name = "printToolStripButton";
            this.printToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.printToolStripButton.Text = "打印";
            this.printToolStripButton.Visible = false;
            // 
            // printPreviewToolStripButton
            // 
            this.printPreviewToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.printPreviewToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("printPreviewToolStripButton.Image")));
            this.printPreviewToolStripButton.ImageTransparentColor = System.Drawing.Color.Black;
            this.printPreviewToolStripButton.Name = "printPreviewToolStripButton";
            this.printPreviewToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.printPreviewToolStripButton.Text = "打印预览";
            this.printPreviewToolStripButton.Visible = false;
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            this.toolStripSeparator2.Visible = false;
            // 
            // helpToolStripButton
            // 
            this.helpToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.helpToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("helpToolStripButton.Image")));
            this.helpToolStripButton.ImageTransparentColor = System.Drawing.Color.Black;
            this.helpToolStripButton.Name = "helpToolStripButton";
            this.helpToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.helpToolStripButton.Text = "帮助";
            this.helpToolStripButton.Visible = false;
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel});
            this.statusStrip.Location = new System.Drawing.Point(0, 431);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(733, 22);
            this.statusStrip.TabIndex = 2;
            this.statusStrip.Text = "StatusStrip";
            // 
            // toolStripStatusLabel
            // 
            this.toolStripStatusLabel.Name = "toolStripStatusLabel";
            this.toolStripStatusLabel.Size = new System.Drawing.Size(32, 17);
            this.toolStripStatusLabel.Text = "状态";
            // 
            // tvLeftMenu
            // 
            this.tvLeftMenu.Dock = System.Windows.Forms.DockStyle.Left;
            this.tvLeftMenu.Location = new System.Drawing.Point(0, 50);
            this.tvLeftMenu.Name = "tvLeftMenu";
            this.tvLeftMenu.Size = new System.Drawing.Size(175, 381);
            this.tvLeftMenu.TabIndex = 4;
            this.tvLeftMenu.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.tvLeftMenu_ItemDrag);
            this.tvLeftMenu.DoubleClick += new System.EventHandler(this.tvLeftMenu_DoubleClick);
            // 
            // pnlLeftMenu
            // 
            this.pnlLeftMenu.Controls.Add(this.btnHideTree);
            this.pnlLeftMenu.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlLeftMenu.Location = new System.Drawing.Point(175, 50);
            this.pnlLeftMenu.Name = "pnlLeftMenu";
            this.pnlLeftMenu.Size = new System.Drawing.Size(13, 381);
            this.pnlLeftMenu.TabIndex = 5;
            // 
            // btnHideTree
            // 
            this.btnHideTree.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.btnHideTree.Location = new System.Drawing.Point(-1, 191);
            this.btnHideTree.Name = "btnHideTree";
            this.btnHideTree.Size = new System.Drawing.Size(14, 23);
            this.btnHideTree.TabIndex = 0;
            this.btnHideTree.Text = "<";
            this.btnHideTree.UseVisualStyleBackColor = true;
            this.btnHideTree.Click += new System.EventHandler(this.btnHideTree_Click);
            // 
            // pnlMenuNavigate
            // 
            this.pnlMenuNavigate.Controls.Add(this.txbMenuPath);
            this.pnlMenuNavigate.Controls.Add(this.label1);
            this.pnlMenuNavigate.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlMenuNavigate.Location = new System.Drawing.Point(188, 50);
            this.pnlMenuNavigate.Name = "pnlMenuNavigate";
            this.pnlMenuNavigate.Size = new System.Drawing.Size(545, 23);
            this.pnlMenuNavigate.TabIndex = 6;
            // 
            // txbMenuPath
            // 
            this.txbMenuPath.BackColor = System.Drawing.SystemColors.Control;
            this.txbMenuPath.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txbMenuPath.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.txbMenuPath.Location = new System.Drawing.Point(75, 5);
            this.txbMenuPath.Name = "txbMenuPath";
            this.txbMenuPath.Size = new System.Drawing.Size(319, 14);
            this.txbMenuPath.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 5);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "菜单导航：";
            // 
            // tcMenu
            // 
            this.tcMenu.Alignment = System.Windows.Forms.TabAlignment.Bottom;
            this.tcMenu.Controls.Add(this.tpgDesktop);
            this.tcMenu.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.tcMenu.Location = new System.Drawing.Point(188, 407);
            this.tcMenu.Name = "tcMenu";
            this.tcMenu.SelectedIndex = 0;
            this.tcMenu.Size = new System.Drawing.Size(545, 24);
            this.tcMenu.TabIndex = 7;
            this.tcMenu.SelectedIndexChanged += new System.EventHandler(this.tcMenu_SelectedIndexChanged);
            // 
            // tpgDesktop
            // 
            this.tpgDesktop.Location = new System.Drawing.Point(4, 4);
            this.tpgDesktop.Name = "tpgDesktop";
            this.tpgDesktop.Size = new System.Drawing.Size(537, 0);
            this.tpgDesktop.TabIndex = 0;
            this.tpgDesktop.Text = "桌面";
            this.tpgDesktop.UseVisualStyleBackColor = true;
            // 
            // pnlDestop
            // 
            this.pnlDestop.AccessibleDescription = "";
            this.pnlDestop.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlDestop.Location = new System.Drawing.Point(188, 73);
            this.pnlDestop.Name = "pnlDestop";
            this.pnlDestop.Size = new System.Drawing.Size(545, 47);
            this.pnlDestop.TabIndex = 8;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cmsMenuTabPositionUp,
            this.cmsMenuTabPositionDown});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(101, 48);
            // 
            // cmsMenuTabPositionUp
            // 
            this.cmsMenuTabPositionUp.Name = "cmsMenuTabPositionUp";
            this.cmsMenuTabPositionUp.Size = new System.Drawing.Size(100, 22);
            this.cmsMenuTabPositionUp.Text = "居上";
            this.cmsMenuTabPositionUp.Click += new System.EventHandler(this.cmsMenuTabPositionUp_Click);
            // 
            // cmsMenuTabPositionDown
            // 
            this.cmsMenuTabPositionDown.Name = "cmsMenuTabPositionDown";
            this.cmsMenuTabPositionDown.Size = new System.Drawing.Size(100, 22);
            this.cmsMenuTabPositionDown.Text = "居下";
            this.cmsMenuTabPositionDown.Click += new System.EventHandler(this.cmsMenuTabPositionDown_Click);
            // 
            // tsmiUserManual
            // 
            this.tsmiUserManual.Name = "tsmiUserManual";
            this.tsmiUserManual.Size = new System.Drawing.Size(152, 22);
            this.tsmiUserManual.Text = "用户手册(&U)";
            this.tsmiUserManual.Click += new System.EventHandler(this.tsmiUserManual_Click);
            // 
            // FrmMainMDI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(733, 453);
            this.Controls.Add(this.pnlDestop);
            this.Controls.Add(this.tcMenu);
            this.Controls.Add(this.pnlMenuNavigate);
            this.Controls.Add(this.pnlLeftMenu);
            this.Controls.Add(this.tvLeftMenu);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.toolStrip);
            this.Controls.Add(this.menuStrip);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.IsMdiContainer = true;
            this.MainMenuStrip = this.menuStrip;
            this.Name = "FrmMainMDI";
            this.Text = "主窗体";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmMainMDI_FormClosing);
            this.Load += new System.EventHandler(this.FrmMainMDI_Load);
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.pnlLeftMenu.ResumeLayout(false);
            this.pnlMenuNavigate.ResumeLayout(false);
            this.pnlMenuNavigate.PerformLayout();
            this.tcMenu.ResumeLayout(false);
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion


        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripMenuItem tsbRestartMenu;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tileHorizontalToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tsbStartMenu;
        private System.Windows.Forms.ToolStripMenuItem tsbExitMenu;
        private System.Windows.Forms.ToolStripMenuItem viewMenu;
        private System.Windows.Forms.ToolStripMenuItem toolBarToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem statusBarToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem windowsMenu;
        private System.Windows.Forms.ToolStripMenuItem cascadeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tileVerticalToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem arrangeIconsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpMenu;
        private System.Windows.Forms.ToolStripButton saveToolStripButton;
        private System.Windows.Forms.ToolStripButton printToolStripButton;
        private System.Windows.Forms.ToolStripButton printPreviewToolStripButton;
        private System.Windows.Forms.ToolStripButton helpToolStripButton;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.TreeView tvLeftMenu;
        private System.Windows.Forms.Panel pnlLeftMenu;
        private System.Windows.Forms.Panel pnlMenuNavigate;
        private System.Windows.Forms.TabControl tcMenu;
        private System.Windows.Forms.Panel pnlDestop;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripTextBox tstbMenuSearch;
        private System.Windows.Forms.ToolStripButton tsbAutoGuid;
        private System.Windows.Forms.TabPage tpgDesktop;
        private System.Windows.Forms.TextBox txbMenuPath;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnHideTree;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem cmsMenuTabPositionUp;
        private System.Windows.Forms.ToolStripMenuItem cmsMenuTabPositionDown;
        private System.Windows.Forms.ToolStripMenuItem tsmiMenuManage;
        private System.Windows.Forms.ToolStripMenuItem tsmiUserManual;
        private System.Windows.Forms.HelpProvider helpProvider1;
    }
}



