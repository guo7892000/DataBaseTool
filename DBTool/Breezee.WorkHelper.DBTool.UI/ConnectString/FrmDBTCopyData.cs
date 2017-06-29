using Breezee.Framework.BaseUI;
using Breezee.Framework.Tool;
using Breezee.Global.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Breezee.WorkHelper.DBTool.UI
{
    /// <summary>
    /// 复制数据生成拼接字符
    /// 测试结果：通过
    /// </summary>
    public partial class FrmDBTCopyData : BaseForm
    {
        #region 变量
        private readonly string _strTableName = "变更表清单";
        private BindingSource bsTable = new BindingSource();
        private string _strAutoSqlSuccess = "生成成功，并已复制到了粘贴板。详细见“生成的SQL”页签！";
        #endregion

        #region 构造函数
        public FrmDBTCopyData()
        {
            InitializeComponent();
        } 
        #endregion

        #region 加载事件
        private void FrmCopyData_Load(object sender, EventArgs e)
        {
            IDictionary<string, string> dic_List = new Dictionary<string, string>();
            dic_List.Add("1", "自定义拼接字符串");
            dic_List.Add("2", "UNION清单");
            dic_List.Add("3", "UNION ALL清单");
            UIHelper.BindTypeValueDropDownList(cbbSqlType, dic_List.GetTextValueTable(false), false, true);

            //数据库类型
            DataTable dtDbType = DBToolUIHelper.GetBaseDataTypeTable();
            UIHelper.BindTypeValueDropDownList(cbbDbType, dtDbType, false, true);

            DataTable dtCopy = new DataTable();
            dtCopy.TableName = _strTableName;
            bsTable.DataSource = dtCopy;
            dgvTableList.DataSource = bsTable;
            GlobalValue.Instance.SetPublicDataSource(new DataTable[] { dtCopy });
            //
            lblTableData.Text = "可在Excel中复制数据后，点击网格后按ctrl + v粘贴即可。注：第一行为列名！";
        }
        #endregion

        #region 网格粘贴事件
        private void dgvTableList_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Modifiers == Keys.Control && e.KeyCode == Keys.V)
                {
                    string pasteText = Clipboard.GetText().Trim();
                    if (string.IsNullOrEmpty(pasteText))//包括IN的为生成的SQL，不用粘贴
                    {
                        return;
                    }
                    DataTable dtMain = (DataTable)GlobalValue.Instance.dicBindingSource[_strTableName].DataSource;
                    int iRow = 0;
                    int iColumn = 0;
                    Object[,] data = StringHelper.GetStringArray(ref pasteText, ref iRow, ref iColumn);
                    #region 生成Union处理
                    int iLastRow = dtMain.Rows.Count;
                    int iColCount = dtMain.Columns.Count;
                    for (int i = 0; i < iRow; i++)
                    {
                        if (iColCount != 0 || (iColCount == 0 && i != 0))
                        {
                            dtMain.Rows.Add(dtMain.NewRow());//不是第一次增加的第0时，才加新行
                        }
                        for (int j = 0; j < iColumn; j++)
                        {
                            if (iColCount != 0 && j >= iColCount)
                            {
                                break;//当后面导入的列比原来多时，退出循环
                            }
                            if (i == 0 && iColCount == 0)
                            {
                                dtMain.Columns.Add(data[i, j].ToString());//第一次增加列
                            }
                            else if (iColCount == 0)
                            {
                                dtMain.Rows[iLastRow + i - 1][j] = data[i, j];//第一次数据赋值
                            }
                            else
                            {
                                dtMain.Rows[iLastRow + i][j] = data[i, j];//非第一次数据赋值
                            }
                        }
                    }
                    dtMain.AcceptChanges();

                    #endregion
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "出错提示", MessageBoxButtons.OK);
            }
        }
        #endregion

        #region 生成SQL按钮事件
        private void tsbAutoSQL_Click(object sender, EventArgs e)
        {
            try
            {
                string sbAllSql = "";
                string strSqlType = cbbSqlType.SelectedValue.ToString();
                if (strSqlType == "1")
                {
                    #region 拼接字符串
                    DataTable dtMain = (DataTable)GlobalValue.Instance.dicBindingSource[_strTableName].DataSource;
                    if (dtMain.Rows.Count == 0)
                    {
                        MessageBox.Show("没有可生成的数据！", "提示", MessageBoxButtons.OK);
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
                    #endregion
                }
                else
                {
                    #region UNION清单生成
                    string strUnion = "UNION SELECT ";
                    string strUnionAll = "UNION ALL SELECT ";
                    string strOrcEnd = " FROM DUAL ";
                    int iDbType = int.Parse(cbbDbType.SelectedValue.ToString());
                    DataBaseType selectDBType = (DataBaseType)iDbType;

                    DataTable dtMain = (DataTable)GlobalValue.Instance.dicBindingSource[_strTableName].DataSource;
                    if (dtMain.Rows.Count == 0)
                    {
                        MessageBox.Show("没有可生成的数据！", "提示", MessageBoxButtons.OK);
                        return;
                    }
                    for (int i = 0; i < dtMain.Rows.Count; i++)
                    {
                        string strOneData = "";
                        for (int j = 0; j < dtMain.Columns.Count; j++)
                        {
                            string strData = dtMain.Rows[i][j].ToString().Trim();
                            if (i == 0)
                            {
                                #region 第一行定义列名
                                if (selectDBType == DataBaseType.SqlServer)
                                {
                                    //SQL Server以中括号([])作为列别名
                                    if (string.IsNullOrEmpty(strData))
                                    {
                                        strOneData += "NULL AS [" + dtMain.Columns[j].ColumnName + "],";
                                    }
                                    else
                                    {
                                        strOneData += StringHelper.ChangeIntoSqlString(strData) + " AS [" + dtMain.Columns[j].ColumnName + "],";
                                    }
                                }
                                else
                                {
                                    //其他类型以双括号("")作为列别名
                                    if (string.IsNullOrEmpty(strData))
                                    {
                                        strOneData += "NULL AS \"" + dtMain.Columns[j].ColumnName + "\",";
                                    }
                                    else
                                    {
                                        strOneData += StringHelper.ChangeIntoSqlString(strData) + " AS \"" + dtMain.Columns[j].ColumnName + "\",";
                                    }
                                }
                                #endregion
                            }
                            else
                            {
                                #region 非第一行不用定义列名
                                if (string.IsNullOrEmpty(strData))
                                {
                                    strOneData += "NULL,";//其他列不定义列名
                                }
                                else
                                {
                                    strOneData += StringHelper.ChangeIntoSqlString(strData) + ",";//其他列不定义列名
                                }
                                #endregion
                            }
                        }

                        #region 构造生成SQL
                        strOneData = strOneData.Substring(0, strOneData.Length - 1);
                        if (i == 0)
                        {
                            if (selectDBType == DataBaseType.Oracle)
                            {
                                strOneData += strOrcEnd;//Oracle需要增加from dual
                            }
                        }
                        else if (i != 0)
                        {
                            if (strSqlType == "2")
                            {
                                strOneData = strUnion + strOneData;
                            }
                            else
                            {
                                strOneData = strUnionAll + strOneData;
                            }
                            if (selectDBType == DataBaseType.Oracle)
                            {
                                strOneData += strOrcEnd;
                            }
                        }
                        sbAllSql += strOneData + "\n";
                        #endregion
                    }
                    sbAllSql = "SELECT " + sbAllSql;

                    #endregion
                }
                rtbResult.Clear();
                rtbResult.AppendText(sbAllSql.ToString() + "\n");
                Clipboard.SetData(DataFormats.UnicodeText, sbAllSql.ToString());
                tabControl1.SelectedTab = tpAutoSQL;
                //生成SQL成功后提示
                //MessageBox.Show(strInfo, "生成成功", MessageBoxButtons.OK);
                lblInfo.Text = _strAutoSqlSuccess;
                rtbResult.Select(0, 0); //返回到第一
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion

        #region 类型选择变化事件
        private void cbbSqlType_SelectedIndexChanged(object sender, EventArgs e)
        {
            string strSqlType = cbbSqlType.SelectedValue.ToString();
            if (strSqlType == "1")
            {
                grbConSting.Visible = true;
                cbbDbType.Visible = false;
                lblDbType.Visible = false;
            }
            else
            {
                grbConSting.Visible = false;
                cbbDbType.Visible = true;
                lblDbType.Visible = true;
            }
        }
        #endregion

        #region 退出按钮事件
        private void tsbExit_Click(object sender, EventArgs e)
        {
            Close();
        } 
        #endregion
    }
}
