using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.OleDb;
using System.IO;
using Breezee.Framework.BaseUI;
using Breezee.Framework.Tool;
using Breezee.Global.Entity;

namespace Breezee.WorkHelper.DBTool.UI
{
    /// <summary>
    /// 表列说明变更
    /// </summary>
    public partial class FrmDBTTableExtendProperty : Form
    {
        #region 变量
        private readonly string _strTableName = "变更表清单";
        //
        private BindingSource bsTable = new BindingSource();
        private BindingSource bsCos = new BindingSource();//
        private BindingSource bsThree = new BindingSource();//
        //常量
        private string _strAutoSqlSuccess = "生成成功，并已复制到了粘贴板。详细见“生成的SQL”页签！";
        private string _strImportSuccess = "导入成功！可点“生成SQL”按钮得到本次导入的变更SQL。";
        //文件路径
        string _DBConnString; //连接字符串

        //导入的SQL变量值
        private string _strMainSql = "";//主SQL
        private string _strErr = "";//错误提示

        //数据集
        private DataSet dsExcel;
        public IDictionary<string, BindingSource> dicBindingSource = new Dictionary<string, BindingSource>();

        //创建类型下拉框
        //private readonly string _strCreate_AddOnly = "0";//只是新增
        //private readonly string _strCreate_DeleteAndAdd = "1";//先删后增
        //private readonly string _strCreate_Delete = "2";//只是删除
        #endregion

        public FrmDBTTableExtendProperty()
        {
            InitializeComponent();
        }

        private void FrmTableExtendProperty_Load(object sender, EventArgs e)
        {
            //数据库类型
            DataTable dtDbType = DBToolUIHelper.GetBaseDataTypeTable();
            UIHelper.BindTypeValueDropDownList(cbbDbType, dtDbType, false, true);
            //
            lblTableData.Text = "SQLite不支持注释。MySql只支持增加表注释，因为导入的信息不足以生成列注释！";
        }

        private void tsbImport_Click(object sender, EventArgs e)
        {
            try
            {
                dsExcel = new DataSet();

                #region 扩展说明变更
                //表清单
                _strMainSql = @"SELECT 表名,列名,扩展属性说明
                     FROM [表列扩展属性说明$] where 表名 is not null order by [表名]";
                //错误提示信息
                _strErr = @"请确定工作表包括名为“表列扩展属性说明”。其中“变更表清单”包括列“表名,列名,扩展说明”。";
                #endregion

                OpenFileDialog opd = new OpenFileDialog();
                //opd.Filter = "Excel文件(*.xls,*.xlsx)|*.xls;*.xlsx";  //支持2003、2007以上格式的Excel
                opd.Filter = "Excel文件(*.xlsx)|*.xlsx"; //只支持2007以上格式的Excel
                opd.FilterIndex = 0;
                opd.Title = "选择对应类型的导入模板Excel文件";
                opd.RestoreDirectory = false;

                if (DialogResult.Cancel == opd.ShowDialog())
                {
                    return;
                }
                string sFilePath = opd.FileName;
                string[] strFileNam = sFilePath.Split('.');


                #region 导入模板后处理
                string strFileFormart = strFileNam[strFileNam.Length - 1].ToString().ToLower();
                if (strFileFormart == "xls")
                {
                    _DBConnString = @"Provider=Microsoft.jet.OleDb.4.0;Data Source=" + sFilePath + ";Extended Properties='Excel 8.0;IMEX=1'";
                }
                else if (strFileFormart == "xlsx")
                {
                    _DBConnString = @"Provider=Microsoft.ACE.OLEDB.12.0; Data Source=" + sFilePath + "; Extended Properties='Excel 12.0 Xml;HDR=YES;IMEX=1'";
                }
                using (OleDbConnection con = new OleDbConnection(_DBConnString))
                {
                    if (con.State != ConnectionState.Open)
                    {
                        con.Open();
                    }
                    try
                    {
                        OleDbDataAdapter daTable = new OleDbDataAdapter(_strMainSql, con);
                        //打开连接并填充表
                        daTable.Fill(dsExcel, _strTableName);
                        bsTable.DataSource = dsExcel.Tables[_strTableName];
                        dgvTableList.DataSource = bsTable;
                    }
                    catch (Exception ex)
                    {
                        MsgHelper.ShowErr(ex.Message);
                        return;
                    }
                }
                #endregion
                //导入成功后处理
                tsbAutoSQL.Enabled = true;
                tabControl1.SelectedTab = tpImport;

                //导入成功提示
                lblInfo.Text = _strImportSuccess;
            }
            catch (Exception ex)
            {
                MsgHelper.ShowErr(ex.Message);
            }
        }

        private void tsbAutoSQL_Click(object sender, EventArgs e)
        {
            #region 表列扩展属性变更
            int iDbType = int.Parse(cbbDbType.SelectedValue.ToString());
            DataBaseType selectDBType = (DataBaseType)iDbType;
            if (selectDBType == DataBaseType.SQLite)
            {
                MsgHelper.ShowInfo("SQLite不支持注释！");
                return;
            }
            //取得数据源
            DataTable dtMain = (DataTable)bsTable.DataSource;

            #region 移除空行
            string strColNameList = "";
            for (int k = 0; k < dtMain.Columns.Count; k++)
            {
                string strOneDataCol = "([" + dtMain.Columns[k].ColumnName + "] is null or [" + dtMain.Columns[k].ColumnName + "]='') ";
                if (k != 0)
                {
                    strColNameList = strColNameList + "and " + strOneDataCol;
                }
                else
                {
                    strColNameList = strColNameList + strOneDataCol;
                }

            }
            foreach (DataRow dr in dtMain.Select(strColNameList))
            {
                dtMain.Rows.Remove(dr);
            }
            #endregion

            //得到变更后数据
            dtMain.AcceptChanges();
            if (dtMain.Rows.Count == 0)
            {
                MsgHelper.ShowInfo("没有可生成的数据！");
                return;
            }
            string strExtendList = "";
            int iTableExtend = 1;
            string sbAllSql = "";
            if (selectDBType == DataBaseType.SqlServer)
            {
                #region SqlServer
                foreach (DataRow drCol in dtMain.Rows)
                {
                    string strExtendTableName = drCol["表名"].ToString();
                    string strExtendColName = drCol["列名"].ToString();
                    string strExtendText = drCol["扩展属性说明"].ToString();

                    string strExtend = " SELECT " + StringHelper.ChangeIntoSqlString(strExtendTableName) + " TABLE_NAME,"
                            + StringHelper.ChangeIntoSqlString(strExtendColName) + " COLUMNS_NAME,"
                            + StringHelper.ChangeIntoSqlString(strExtendText) + " EXTEND_TEXT"
                            + " \n";
                    if (iTableExtend != 1)
                    {
                        strExtend = " UNION " + strExtend;
                    }
                    strExtendList = strExtendList + strExtend;
                    iTableExtend++;
                }
                //读取并替换
                sbAllSql = File.ReadAllText(UIHelper.GetSystemFullPath(@"SqlScript/1.001_修改表列说明.sql"), Encoding.Default).Replace("#EXTEND_LIST#", strExtendList); 
                #endregion
            }
            else
            {
                foreach (DataRow drCol in dtMain.Rows)
                {
                    string strExtendTableName = drCol["表名"].ToString();
                    string strExtendColName = drCol["列名"].ToString();
                    string strExtendText = drCol["扩展属性说明"].ToString().Trim();
                    if (string.IsNullOrEmpty(strExtendTableName))
                    {
                        iTableExtend++;
                        continue;
                    }

                    if (string.IsNullOrEmpty(strExtendColName))
                    {
                        if (selectDBType == DataBaseType.Oracle)
                        {
                            strExtendList += string.Format(@"COMMENT ON TABLE {0} IS '{1}';" + "\n", strExtendTableName, strExtendText);
                        }
                        else if (selectDBType == DataBaseType.MySql)
                        {
                            strExtendList += string.Format(@"ALTER TABLE {0} COMMENT = '{1}';" + "\n", strExtendTableName, strExtendText);
                        }
                        else if (selectDBType == DataBaseType.PostgreSql)
                        {
                            //
                            strExtendList += string.Format(@"COMMENT ON TABLE {0} IS '{1}';" + "\n", strExtendTableName, strExtendText);
                        }
                        else
                        {
                            throw new Exception("暂不支持该数据库类型");
                        }
                    }
                    else
                    {
                        if (selectDBType == DataBaseType.Oracle)
                        {
                            strExtendList += string.Format(@"COMMENT ON COLUMN {0}.{1} IS '{2}';" + "\n", strExtendTableName, strExtendColName, strExtendText);
                        }
                        else if (selectDBType == DataBaseType.MySql)
                        {
                            //需要加上原字段信息，所以建议利用修改表列的SQL来修改
                            //strExtendList += string.Format(@"ALTER TABLE {0} MODIFY COLUMN {1} COMMENT '{2}';" + "\n", strExtendTableName, strExtendColName, strExtendText);
                        }
                        else if (selectDBType == DataBaseType.PostgreSql)
                        {
                            //
                            strExtendList += string.Format(@"COMMENT ON COLUMN {0}.{1} IS '{2}';" + "\n", strExtendTableName, strExtendColName, strExtendText);
                        }
                        else
                        {
                            throw new Exception("暂不支持该数据库类型");
                        }
                        
                    }
                    iTableExtend++;
                }
                sbAllSql = strExtendList;
            }
            //生成SQL并提示
            rtbResult.Clear();
            rtbResult.AppendText(sbAllSql.ToString() + "\n");
            Clipboard.SetData(DataFormats.UnicodeText, sbAllSql.ToString());
            tabControl1.SelectedTab = tpAutoSQL;
            //生成SQL成功后提示
            //MessageBox.Show(strInfo, "生成成功", MessageBoxButtons.OK);
            lblInfo.Text = _strAutoSqlSuccess;
            rtbResult.Select(0, 0); //返回到第一
            return;
            #endregion
        }

        private void tsbDownLoad_Click(object sender, EventArgs e)
        {
            DBToolUIHelper.DownloadFile(@"DataModel\TableStruct\模板_表列说明变更.xlsx", "模板_表列说明变更", true);
        }

        private void tsbExit_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
