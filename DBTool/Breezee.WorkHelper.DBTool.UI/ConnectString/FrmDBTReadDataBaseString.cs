using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Breezee.Framework.BaseUI;
using Breezee.Framework.DataAccess.Custom;
using Breezee.Framework.DataAccess.INF;
using Breezee.Framework.Tool;
using Breezee.Global.Context;
using Breezee.Global.Entity;
using Breezee.WorkHelper.DBTool.INF;

namespace Breezee.WorkHelper.DBTool.UI
{
    /// <summary>
    /// 读取数据拼接字符
    /// 测试结果：通过
    /// </summary>
    public partial class FrmDBTReadDataBaseString : BaseForm
    {
        #region 变量
        private readonly string _strTableName = "变更表清单";
        //
        private BindingSource bsTable = new BindingSource();
        private BindingSource bsCos = new BindingSource();//
        private BindingSource bsThree = new BindingSource();//
        //常量
        private string _strAutoSqlSuccess = "生成成功，并已复制到了粘贴板。详细见“生成的SQL”页签！";

        //导入的SQL变量值
        private string _strMainSql = "";//主SQL
        //数据访问层
        private IDBConfigSet _IDBConfigSet;
        private DbServerInfo _dbServer;
        #endregion

        #region 构造函数
        public FrmDBTReadDataBaseString()
        {
            InitializeComponent();
        }
        #endregion

        #region 加载事件
        private void FrmReadDataBaseString_Load(object sender, EventArgs e)
        {
            //设置数据库连接控件
            _IDBConfigSet = ContainerContext.Container.Resolve<IDBConfigSet>();
            DataTable dtConn = _IDBConfigSet.QueryDbConfig(_dicQuery).SafeGetDictionaryTable();
            uC_DbConnection1.SetDbConnComboBoxSource(dtConn);
            uC_DbConnection1.IsDbNameNotNull = false;
            //
            lblTableWhereInfo.Text = "表名不为空时，Where条件作为表的过滤条件；表为空时，Where条件为自定义SQL。";
        } 
        #endregion

        #region 连接按钮事件
        private void tsbConnect_Click(object sender, EventArgs e)
        {
            try
            {
                //非空判断
                string strTableName = cbbTableName.Text.Trim();
                string strWhere = rtbWhere.Text.Trim();
                if (string.IsNullOrEmpty(strTableName) && string.IsNullOrEmpty(strWhere))
                {
                    MsgHelper.ShowErr("表名和Where条件不能同时为空！");
                    return;
                }
                //得到服务器对象
                _dbServer = uC_DbConnection1.GetDbServerInfo();
                if (_dbServer == null)
                {
                    return;
                }
                //得到数据库访问对象
                ICustomDataAccess customDataAccess = new CustomDataAccess(_dbServer.DatabaseType, _dbServer);

                
                //构造查询SQL
                if (string.IsNullOrEmpty(strWhere)) //Where条件为空
                {
                    _strMainSql = "SELECT *  FROM " + strTableName;
                }
                else if (string.IsNullOrEmpty(strTableName))//表名为空，那么Where中为自定义SQL
                {
                    _strMainSql = strWhere;
                }
                else //表名和Where条件都不为空，那么拼接语句
                {
                    _strMainSql = "SELECT *  FROM " + strTableName + " WHERE " + strWhere;
                }
                //查询数据
                DataTable dtMain = customDataAccess.DataAccess.QueryHadParamSqlData(_strMainSql,_dicQuery);
                dtMain.TableName = _strTableName;
                bsTable.DataSource = dtMain;
                //设置数据源
                GlobalValue.Instance.SetPublicDataSource(new DataTable[] { dtMain });
                dgvTableList.DataSource = bsTable;
            }
            catch (Exception ex)
            {
                MsgHelper.ShowErr(ex.Message);
            }
        } 
        #endregion

        #region 自动生成SQL按钮事件
        private void tsbAutoSQL_Click(object sender, EventArgs e)
        {
            try
            {
                string sbAllSql = "";
                DataTable dtMain = (DataTable)bsTable.DataSource;
                if (dtMain.Rows.Count == 0)
                {
                    MsgHelper.ShowInfo("没有可生成的数据！");
                    return;
                }
                for (int i = 0; i < dtMain.Rows.Count; i++)
                {
                    //初始化单条数据为书写的文本
                    string strOneData = rtbConString.Text.Trim();
                    for (int j = 0; j < dtMain.Columns.Count; j++)
                    {
                        string strData = dtMain.Rows[i][j].ToString().Trim();
                        //将数据中的列名替换为单元格中的数据
                        strOneData = strOneData.Replace("#" + dtMain.Columns[j].ColumnName + "#", strData);
                    }
                    //所有SQL文本累加
                    sbAllSql += strOneData + "\n";
                }
                //保存属性
                //PropSetting.Default.Save();
                rtbResult.Clear();
                rtbResult.AppendText(sbAllSql.ToString() + "\n");
                Clipboard.SetData(DataFormats.UnicodeText, sbAllSql.ToString());
                tabControl1.SelectedTab = tpAutoSQL;
                //生成SQL成功后提示
                //MessageBox.Show(strInfo, "生成成功", MessageBoxButtons.OK);
                lblInfo.Text = _strAutoSqlSuccess;
                rtbResult.Select(0, 0); //返回到第一行
            }
            catch (Exception ex)
            {
                MsgHelper.ShowErr(ex.Message);
            }
        } 
        #endregion

        #region 帮助按钮事件
        private void tsbHelp_Click(object sender, EventArgs e)
        {

        } 
        #endregion

        #region 退出按钮事件
        private void tsbExit_Click(object sender, EventArgs e)
        {
            Close();
        }
        #endregion

        #region 获取表清单复选框变化事件
        private void ckbGetTableList_CheckedChanged(object sender, EventArgs e)
        {
            if (ckbGetTableList.Checked)
            {
                _dbServer = uC_DbConnection1.GetDbServerInfo();
                if (_dbServer == null)
                {
                    return;
                }
                //绑定下拉框
                UIHelper.BindDropDownList(cbbTableName, uC_DbConnection1.UserTableList.Sort("TABLE_NAME"), "TABLE_NAME", "TABLE_NAME", false);
            }
            else
            {
                cbbTableName.DataSource = null;
            }
        }
        #endregion
    }
}
