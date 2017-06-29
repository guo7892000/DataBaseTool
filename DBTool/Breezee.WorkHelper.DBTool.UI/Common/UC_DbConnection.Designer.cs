namespace Breezee.WorkHelper.DBTool.UI
{
    partial class UC_DbConnection
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

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.cbbDatabaseType = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txbServerIP = new System.Windows.Forms.TextBox();
            this.lblServerAddr = new System.Windows.Forms.Label();
            this.lblLoginType = new System.Windows.Forms.Label();
            this.cbbLoginType = new System.Windows.Forms.ComboBox();
            this.lblDbName = new System.Windows.Forms.Label();
            this.txbDbName = new System.Windows.Forms.TextBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.cbbDbConnName = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.btnSelectDbFile = new System.Windows.Forms.Button();
            this.lblPortNO = new System.Windows.Forms.Label();
            this.txbPortNO = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txbUserName = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.txbPassword = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txbSchemaName = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // cbbDatabaseType
            // 
            this.cbbDatabaseType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.cbbDatabaseType.FormattingEnabled = true;
            this.cbbDatabaseType.Location = new System.Drawing.Point(296, 4);
            this.cbbDatabaseType.Name = "cbbDatabaseType";
            this.cbbDatabaseType.Size = new System.Drawing.Size(88, 20);
            this.cbbDatabaseType.TabIndex = 4;
            this.cbbDatabaseType.SelectedIndexChanged += new System.EventHandler(this.cbbDatabaseType_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(77, 12);
            this.label1.TabIndex = 3;
            this.label1.Text = "连接配置名：";
            // 
            // txbServerIP
            // 
            this.txbServerIP.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txbServerIP.Location = new System.Drawing.Point(473, 4);
            this.txbServerIP.Name = "txbServerIP";
            this.txbServerIP.Size = new System.Drawing.Size(99, 21);
            this.txbServerIP.TabIndex = 6;
            // 
            // lblServerAddr
            // 
            this.lblServerAddr.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lblServerAddr.AutoSize = true;
            this.lblServerAddr.Location = new System.Drawing.Point(390, 8);
            this.lblServerAddr.Name = "lblServerAddr";
            this.lblServerAddr.Size = new System.Drawing.Size(77, 12);
            this.lblServerAddr.TabIndex = 5;
            this.lblServerAddr.Text = "服务器地址：";
            // 
            // lblLoginType
            // 
            this.lblLoginType.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lblLoginType.AutoSize = true;
            this.lblLoginType.Location = new System.Drawing.Point(698, 8);
            this.lblLoginType.Name = "lblLoginType";
            this.lblLoginType.Size = new System.Drawing.Size(65, 12);
            this.lblLoginType.TabIndex = 7;
            this.lblLoginType.Text = "验证方式：";
            // 
            // cbbLoginType
            // 
            this.cbbLoginType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.cbbLoginType.FormattingEnabled = true;
            this.cbbLoginType.Location = new System.Drawing.Point(769, 4);
            this.cbbLoginType.Name = "cbbLoginType";
            this.cbbLoginType.Size = new System.Drawing.Size(103, 20);
            this.cbbLoginType.TabIndex = 8;
            // 
            // lblDbName
            // 
            this.lblDbName.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lblDbName.AutoSize = true;
            this.lblDbName.Location = new System.Drawing.Point(3, 36);
            this.lblDbName.Name = "lblDbName";
            this.lblDbName.Size = new System.Drawing.Size(77, 12);
            this.lblDbName.TabIndex = 5;
            this.lblDbName.Text = "数据库名称：";
            // 
            // txbDbName
            // 
            this.txbDbName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txbDbName.Location = new System.Drawing.Point(86, 32);
            this.txbDbName.Name = "txbDbName";
            this.txbDbName.Size = new System.Drawing.Size(121, 21);
            this.txbDbName.TabIndex = 6;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 13;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.cbbDbConnName, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.label5, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.cbbDatabaseType, 3, 0);
            this.tableLayoutPanel1.Controls.Add(this.lblServerAddr, 4, 0);
            this.tableLayoutPanel1.Controls.Add(this.txbServerIP, 5, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnSelectDbFile, 6, 0);
            this.tableLayoutPanel1.Controls.Add(this.lblPortNO, 7, 0);
            this.tableLayoutPanel1.Controls.Add(this.txbPortNO, 8, 0);
            this.tableLayoutPanel1.Controls.Add(this.cbbLoginType, 10, 0);
            this.tableLayoutPanel1.Controls.Add(this.lblLoginType, 9, 0);
            this.tableLayoutPanel1.Controls.Add(this.lblDbName, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.txbDbName, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.label6, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.txbUserName, 3, 1);
            this.tableLayoutPanel1.Controls.Add(this.label7, 4, 1);
            this.tableLayoutPanel1.Controls.Add(this.txbPassword, 5, 1);
            this.tableLayoutPanel1.Controls.Add(this.txbSchemaName, 8, 1);
            this.tableLayoutPanel1.Controls.Add(this.label2, 6, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 17);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(891, 60);
            this.tableLayoutPanel1.TabIndex = 9;
            // 
            // cbbDbConnName
            // 
            this.cbbDbConnName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.cbbDbConnName.FormattingEnabled = true;
            this.cbbDbConnName.Location = new System.Drawing.Point(86, 4);
            this.cbbDbConnName.Name = "cbbDbConnName";
            this.cbbDbConnName.Size = new System.Drawing.Size(121, 20);
            this.cbbDbConnName.TabIndex = 4;
            this.cbbDbConnName.SelectedIndexChanged += new System.EventHandler(this.cbbDbConnName_SelectedIndexChanged);
            // 
            // label5
            // 
            this.label5.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(213, 8);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(77, 12);
            this.label5.TabIndex = 3;
            this.label5.Text = "数据库类型：";
            // 
            // btnSelectDbFile
            // 
            this.btnSelectDbFile.Location = new System.Drawing.Point(578, 3);
            this.btnSelectDbFile.Name = "btnSelectDbFile";
            this.btnSelectDbFile.Size = new System.Drawing.Size(10, 23);
            this.btnSelectDbFile.TabIndex = 9;
            this.btnSelectDbFile.Text = "...";
            this.btnSelectDbFile.UseVisualStyleBackColor = true;
            this.btnSelectDbFile.Click += new System.EventHandler(this.btnSelectDbFile_Click);
            // 
            // lblPortNO
            // 
            this.lblPortNO.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lblPortNO.AutoSize = true;
            this.lblPortNO.Location = new System.Drawing.Point(594, 8);
            this.lblPortNO.Name = "lblPortNO";
            this.lblPortNO.Size = new System.Drawing.Size(53, 12);
            this.lblPortNO.TabIndex = 5;
            this.lblPortNO.Text = "端口号：";
            // 
            // txbPortNO
            // 
            this.txbPortNO.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txbPortNO.Location = new System.Drawing.Point(653, 4);
            this.txbPortNO.Name = "txbPortNO";
            this.txbPortNO.Size = new System.Drawing.Size(39, 21);
            this.txbPortNO.TabIndex = 6;
            // 
            // label6
            // 
            this.label6.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(237, 36);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(53, 12);
            this.label6.TabIndex = 5;
            this.label6.Text = "用户名：";
            // 
            // txbUserName
            // 
            this.txbUserName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txbUserName.Location = new System.Drawing.Point(296, 32);
            this.txbUserName.Name = "txbUserName";
            this.txbUserName.Size = new System.Drawing.Size(88, 21);
            this.txbUserName.TabIndex = 6;
            // 
            // label7
            // 
            this.label7.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(426, 36);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(41, 12);
            this.label7.TabIndex = 5;
            this.label7.Text = "密码：";
            // 
            // txbPassword
            // 
            this.txbPassword.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txbPassword.Location = new System.Drawing.Point(473, 32);
            this.txbPassword.Name = "txbPassword";
            this.txbPassword.PasswordChar = '*';
            this.txbPassword.Size = new System.Drawing.Size(99, 21);
            this.txbPassword.TabIndex = 6;
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label2.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.label2, 2);
            this.label2.Location = new System.Drawing.Point(582, 36);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 12);
            this.label2.TabIndex = 7;
            this.label2.Text = "架构名称：";
            // 
            // txbSchemaName
            // 
            this.txbSchemaName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.SetColumnSpan(this.txbSchemaName, 2);
            this.txbSchemaName.Location = new System.Drawing.Point(653, 32);
            this.txbSchemaName.Name = "txbSchemaName";
            this.txbSchemaName.Size = new System.Drawing.Size(110, 21);
            this.txbSchemaName.TabIndex = 6;
            // 
            // groupBox1
            // 
            this.groupBox1.AutoSize = true;
            this.groupBox1.Controls.Add(this.tableLayoutPanel1);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(897, 80);
            this.groupBox1.TabIndex = 10;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "数据库连接";
            // 
            // UC_DbConnection
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Name = "UC_DbConnection";
            this.Size = new System.Drawing.Size(897, 80);
            this.Load += new System.EventHandler(this.UC_DbConnection_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cbbDatabaseType;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txbServerIP;
        private System.Windows.Forms.Label lblServerAddr;
        private System.Windows.Forms.Label lblLoginType;
        private System.Windows.Forms.ComboBox cbbLoginType;
        private System.Windows.Forms.Label lblDbName;
        private System.Windows.Forms.TextBox txbDbName;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox cbbDbConnName;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txbUserName;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txbPassword;
        private System.Windows.Forms.Label lblPortNO;
        private System.Windows.Forms.TextBox txbPortNO;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txbSchemaName;
        private System.Windows.Forms.Button btnSelectDbFile;
    }
}
