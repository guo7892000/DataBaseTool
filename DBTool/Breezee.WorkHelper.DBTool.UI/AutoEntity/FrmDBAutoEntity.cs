using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Breezee.WorkHelper.DBTool.BUS;
using Breezee.WorkHelper.DBTool.Entity;
using Breezee.Framework.BaseUI;
using Breezee.Global.Entity;
using Breezee.Framework.DataAccess.INF;
using Breezee.WorkHelper.DBTool.INF;
using Breezee.Global.Context;
using Breezee.Framework.Tool;

namespace Breezee.WorkHelper.DBTool.UI
{
    /// <summary>
    /// 生成实体
    /// 测试结果：通过
    /// </summary>
	public partial class FrmDBAutoEntity: BaseForm
	{
        #region 变量
        private DbServerInfo _dbServer;
        private List<EntityInfo> entityList;
        private AutoEntityBiz _entiyBiz;
        private DataSet dataTables = null;
        private TreeViewEventArgs treeNode;
        private List<EntityInfo> list = new List<EntityInfo>();
        private IDBConfigSet _IDBConfigSet;
        //
        string _sStringColumnList = "1";
        string _sStringColumn = "2";
        string _sSimpleEntity = "3";
        #endregion

        #region 构造函数
        public FrmDBAutoEntity()
        {
            InitializeComponent();
        } 
        #endregion

        #region 加载事件
        private void FrmAutoEntity_Load(object sender, EventArgs e)
        {
            Text = "生成实体类文件";
            ctxmBuilt.Visible = false;
            txtAccess.Text = "public";
            txtClassPreString.Text = "DT_";
            //设置数据库连接控件
            _IDBConfigSet = ContainerContext.Container.Resolve<IDBConfigSet>();
            DataTable dtConn = _IDBConfigSet.QueryDbConfig(_dicQuery).SafeGetDictionaryTable();
            uC_DbConnection1.SetDbConnComboBoxSource(dtConn);
            uC_DbConnection1.IsDbNameNotNull = false;

            #region 绑定下拉框
            //登录类型
            _dicString.Add(_sStringColumnList, "字符列名和集合"); 
            _dicString.Add(_sStringColumn, "字符列名");
            _dicString.Add(_sSimpleEntity, "简单实体");
            UIHelper.BindTypeValueDropDownList(cbbEntityType, UIHelper.GetTextValueTable(_dicString, false), false, true);
            cbbEntityType.SelectedIndex = 0;
            #endregion

            //设置网格的信息
            tvDataBaseInfo.Font = new Font("新宋体", 10f, FontStyle.Bold);
            dgvTableInfo.ReadOnly = true;
            dgvTableInfo.AutoGenerateColumns = false;
            dgvTableInfo.AllowUserToAddRows = false;
            dgvTableInfo.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvEntityInfo.ReadOnly = true;
            dgvEntityInfo.AutoGenerateColumns = false;
            dgvEntityInfo.AllowUserToAddRows = false;
        } 
        #endregion

        #region 测试连接按钮事件
        private void btnLinkServer_Click(object sender, EventArgs e)
        {
            try
            {
                _dbServer = uC_DbConnection1.GetDbServerInfo();
                if (_dbServer == null)
                {
                    return;
                }

                _entiyBiz = new AutoEntityBiz(_dbServer.DatabaseType, _dbServer);
                //加载数据库
                LoadTreeView(_dbServer, _entiyBiz);

                dgvEntityInfo.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

                tabControl1.SelectedTab = tpgEntity;
            }
            catch (Exception exception)
            {
                MsgHelper.ShowErr(exception.Message);
            }
        } 
        #endregion

        #region 关闭按钮事件
        private void btnExit_Click(object sender, EventArgs e)
        {
            Close();
        } 
        #endregion

        #region 选择路径按钮事件
        private void btnPath_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                txtPath.Text = dialog.SelectedPath;
            }
        } 
        #endregion

        #region 检查文本方法
        public bool checkTxt()
        {
            if ((((txtAccess.Text.Trim() == null) || (txtAccess.Text.Trim() == "")) || ((txtNameSpace.Text.Trim() == null) || (txtNameSpace.Text.Trim() == ""))) || (txtClassEndString.Text.Trim() == null))
            {
                MsgHelper.ShowErr("访问修饰符或命名空间不能为空！");
                return false;
            }
            return true;
        } 
        #endregion

        #region 加载树方法
        private void LoadTreeView(DbServerInfo server, AutoEntityBiz entiyBiz)
        {
            string collectionName = DbSchemaString.Databases;
            string[] restrictionValues = null;
            switch (server.DatabaseType)
            {
                case DataBaseType.SqlServer:
                    collectionName = DbSchemaString.Databases;
                    break;
                case DataBaseType.Oracle:
                    collectionName = DbSchemaString.Users;
                    break;
                case DataBaseType.MySql:
                    collectionName = DbSchemaString.Databases;
                    break;
                case DataBaseType.SQLite:
                    collectionName = DbSchemaString.Tables;//对SQLite，只有唯一的Main数据库，没有数据库清单表
                    break;
                case DataBaseType.PostgreSql:
                    collectionName = DbSchemaString.Databases;
                    break;
                default:
                    break;
            }

            try
            {
                DataTable schema = entiyBiz.GetSchema(collectionName, null);
                tvDataBaseInfo.Nodes.Clear();
                TreeNode node = new TreeNode();
                node.ToolTipText = "请您点选后，在按鼠标右键";
                node.Text = server.ServerName;
                for (int i = 0; i < schema.Rows.Count; i++)
                {
                    TreeNode node2 = new TreeNode();
                    string str2 = schema.Rows[i][0].ToString();
                    if (server.DatabaseType == DataBaseType.MySql)
                    {
                        str2 = schema.Rows[i][1].ToString();
                    }
                    server.Database = str2;
                    if (server.DatabaseType == DataBaseType.Oracle)
                    {
                        restrictionValues = new string[] { str2 };
                    }
                    else if (server.DatabaseType == DataBaseType.MySql)
                    {
                        string[] strArray2 = new string[2];
                        strArray2[1] = str2;
                        restrictionValues = strArray2;
                    }
                    DataTable table2 = entiyBiz.GetSchema("Tables", restrictionValues);
                    DataView dv = table2.DefaultView;
                    table2.DefaultView.Sort = "TABLE_NAME ASC";
                    for (int j = 0; j < dv.Count; j++)
                    {
                        string str3 = "";
                        if (server.DatabaseType == DataBaseType.Oracle)
                        {
                            str3 = dv[j][1].ToString();
                        }
                        else
                        {
                            str3 = dv[j][2].ToString();
                        }
                        TreeNode node3 = new TreeNode();
                        node3.ToolTipText = "请您点选后，在按鼠标右键";
                        node3.Text = str3;
                        node2.Nodes.Add(node3);
                    }
                    node2.ToolTipText = "请您点选后，在按鼠标右键";
                    node2.Text = str2;
                    node.Nodes.Add(node2);
                    //对于SQLite，终止循环
                    if (server.DatabaseType == DataBaseType.SQLite)
                    {
                        break;
                    }
                }
                tvDataBaseInfo.Nodes.Add(node);

                //展开默认的数据库表树
                string strDefuatExpandDBName = server.Database;
                foreach (TreeNode tnode in tvDataBaseInfo.Nodes)
                {
                    ExpandDefaultDB(tnode, strDefuatExpandDBName);
                }
            }
            catch (Exception exception)
            {
                MsgHelper.ShowErr(exception.Message);
            }
        } 
        #endregion

        #region 展开默认数据库表视图方法
        /// <summary>
        /// 找到默认数据库并展开
        /// </summary>
        /// <param name="tn"></param>
        /// <param name="strDefuatExpandDBName"></param>
        private void ExpandDefaultDB(TreeNode tn, string strDefuatExpandDBName)
        {
            if (tn.Text == strDefuatExpandDBName)
            {
                tn.ExpandAll();
                ExpandParentNode(tn);
            }
            else
            {
                foreach (TreeNode node in tn.Nodes)
                {
                    ExpandDefaultDB(node, strDefuatExpandDBName);
                }
            }
        }

        /// <summary>
        /// 展开父节点
        /// </summary>
        /// <param name="node"></param>
        private void ExpandParentNode(TreeNode node)
        {
            if (node.Parent != null)
            {
                node.Parent.Expand();
                ExpandParentNode(node.Parent);
            }
        } 
        #endregion

        #region 单部表生成右键菜单事件
        private void tsmiStable_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvTableInfo.DataSource != null)
                {
                    DataSet dataSet = new DataSet();
                    DataTable dataSource = (DataTable)dgvTableInfo.DataSource;
                    dataSet.Tables.Add(dataSource);
                    if (txtPath.Text == "")
                    {
                        MsgHelper.ShowInfo("请您设置存放路径!");
                    }
                    else if (checkTxt() && (treeNode.Node.Level == 2))
                    {
                        string sEntityType = cbbEntityType.SelectedValue.ToString();
                        if (sEntityType == _sStringColumnList)
                        {
                            entityList = _entiyBiz.BuildCustomEntity(dataSet, txtNameSpace.Text.Trim(), txtAccess.Text.Trim(), txtClassPreString.Text.Trim(), txtClassEndString.Text.Trim(), txtPath.Text.Trim());
                        }
                        else if (sEntityType == _sStringColumn)
                        {
                            entityList = _entiyBiz.BuildTableColumnNameEntity(dataSet, txtNameSpace.Text.Trim(), txtAccess.Text.Trim(), txtClassPreString.Text.Trim(), txtClassEndString.Text.Trim(), txtPath.Text.Trim());
                        }
                        else
                        {
                            entityList = _entiyBiz.BuildEntity(dataSet, txtNameSpace.Text.Trim(), txtAccess.Text.Trim(), txtClassPreString.Text.Trim(), txtClassEndString.Text.Trim(), txtPath.Text.Trim());
                        }
                        //
                        if (entityList.Count > 0)
                        {
                            dgvEntityInfo.DataSource = entityList;
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                MsgHelper.ShowErr(exception.Message);
            }
        } 
        #endregion

        #region 全部表生成右键菜单事件
        private void tsmiFtable_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtPath.Text == "")
                {
                    MsgHelper.ShowInfo("请您设置存放路径!");
                }
                else if (checkTxt() && (treeNode.Node.Level == 1))
                {
                    string sEntityType = cbbEntityType.SelectedValue.ToString();
                    if (sEntityType == _sStringColumnList)
                    {
                        entityList = _entiyBiz.BuildCustomEntity(dataTables, txtNameSpace.Text.Trim(), txtAccess.Text.Trim(), txtClassPreString.Text.Trim(), txtClassEndString.Text.Trim(), txtPath.Text.Trim());
                    }
                    else if (sEntityType == _sStringColumn)
                    {
                        entityList = _entiyBiz.BuildTableColumnNameEntity(dataTables, txtNameSpace.Text.Trim(), txtAccess.Text.Trim(), txtClassPreString.Text.Trim(), txtClassEndString.Text.Trim(), txtPath.Text.Trim());
                    }
                    else
                    {
                        entityList = _entiyBiz.BuildEntity(dataTables, txtNameSpace.Text.Trim(), txtAccess.Text.Trim(), txtClassPreString.Text.Trim(), txtClassEndString.Text.Trim(), txtPath.Text.Trim());
                    }
                    
                    if (entityList.Count > 0)
                    {
                        dgvEntityInfo.DataSource = entityList;
                    }
                }
            }
            catch (Exception exception)
            {
                MsgHelper.ShowErr(exception.Message);
            }
        } 
        #endregion

        #region 选择树后事件
        private void tvDataBaseInfo_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e != null)
            {
                treeNode = e;
            }
        } 
        #endregion

        #region 按下鼠标事件
        private void tvDataBaseInfo_MouseUp(object sender, MouseEventArgs e)
        {
            try
            {
                if (treeNode != null)
                {
                    if ((treeNode.Node.Level == 1) || (treeNode.Node.Level == 2))
                    {
                        if (treeNode.Node.IsSelected && (treeNode.Node.Level == 1))
                        {
                            if (treeNode.Node.Nodes.Count > 0)
                            {
                                tsmiStable.Visible = false;
                                tsmiFtable.Visible = true;
                            }
                            else
                            {
                                ctxmBuilt.Visible = false;
                            }
                        }
                        else
                        {
                            tsmiFtable.Visible = false;
                            tsmiStable.Visible = true;
                        }
                    }
                    else
                    {
                        ctxmBuilt.Visible = false;
                    }
                }
            }
            catch (Exception exception)
            {
                MsgHelper.ShowErr(exception.Message);
            }
        } 
        #endregion

        #region 树鼠标单击事件
        private void tvDataBaseInfo_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            DataTable table = null;
            try
            {
                if (e.Node.Level == 2)
                {
                    _dbServer.Database = e.Node.Parent.Text;
                    table = _entiyBiz.GetMemorySTable(e.Node.Parent.Text, e.Node.Text, _dbServer);
                }
                else if ((e.Node.Level == 1) && (e.Node.Nodes.Count > 0))
                {
                    _dbServer.Database = e.Node.Text;
                    DataTable schema = null;
                    if (_dbServer.DatabaseType == DataBaseType.Oracle) 
                    {
                        schema = _entiyBiz.GetSchema("Tables", new string[] { _dbServer.Database });
                    }
                    else
                    {
                        schema = _entiyBiz.GetSchema("Tables", null);
                    }
                    dataTables = new DataSet();
                    for (int i = 0; i < schema.Rows.Count; i++)
                    {
                        string tblName = "";
                        if (_dbServer.DatabaseType == DataBaseType.Oracle)
                        {
                            tblName = schema.Rows[i][1].ToString();
                        }
                        else
                        {
                            tblName = schema.Rows[i][2].ToString();
                        }
                        dataTables.Tables.Add(_entiyBiz.GetMemorySTable(_dbServer.Database, tblName, _dbServer));
                    }
                }
                if (table != null)
                {
                    dgvTableInfo.DataSource = table;
                }
            }
            catch (Exception exception)
            {
                MsgHelper.ShowErr(exception.Message);
            }
        } 
        #endregion

    
	}
}