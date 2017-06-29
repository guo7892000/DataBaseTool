using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.OleDb;
using Breezee.Framework.BaseUI;
using Breezee.Framework.Tool;
using Breezee.Global.Entity;
using Breezee.WorkHelper.DBTool.Entity;

namespace Breezee.WorkHelper.DBTool.UI
{
    public partial class FrmDBTExcelImportData : BaseForm
    {
        #region 变量
        private readonly string _strTableName = "变更表清单";
        private readonly string _strColName = "变更列清单";
        //
        private BindingSource bsTable = new BindingSource();
        private BindingSource bsCos = new BindingSource();//
        private BindingSource bsThree = new BindingSource();//
        private string _strAutoSqlSuccess = "生成成功，并已复制到了粘贴板。详细见“生成的SQL”页签！";
        private string _strImportSuccess = "导入成功！可点“生成SQL”按钮得到本次导入的变更SQL。";

        //连接字符串
        string _DBConnString = string.Empty; 
        //数据集
        private DataSet dsExcel;
        //public IDictionary<string, BindingSource> dicBindingSource = new Dictionary<string, BindingSource>();
        private DataBaseType _selectDBType = DataBaseType.SqlServer;
        private DataBaseType _importDBType = DataBaseType.SqlServer;
        #endregion

        #region 构造函数
        public FrmDBTExcelImportData()
        {
            InitializeComponent();
        } 
        #endregion

        #region 加载事件
        private void FrmExcelImportData_Load(object sender, EventArgs e)
        {
            //生成数据库类型
            DataTable dtDbType = DBToolUIHelper.GetBaseDataTypeTable();
            UIHelper.BindTypeValueDropDownList(cbbDbType, dtDbType, false, true);
            //
            _dicString.Add("0", "全部提交");
            _dicString.Add("1", "单次提交");
            _dicString.Add("20", "每20次提交");
            _dicString.Add("50", "每50次提交");
            _dicString.Add("100", "每100次提交");
            _dicString.Add("200", "每200次提交");
            _dicString.Add("500", "每500次提交");
            UIHelper.BindTypeValueDropDownList(cbbThree, _dicString.GetTextValueTable(false), false, true);
            //
            tsbAutoSQL.Enabled = false;
        } 
        #endregion

        #region 导入按钮事件
        private void tsbImport_Click(object sender, EventArgs e)
        {
            try
            {
                int iDbType = int.Parse(cbbDbType.SelectedValue.ToString());
                _selectDBType = (DataBaseType)iDbType;
                _dicQuery[_strTableName] = DBToolStaticString.DataGenerate_Table + " AND 数据库类型 = '" + _selectDBType.ToString() + "' order by [序号]";
                _dicQuery[_strColName] = DBToolStaticString.DataGenerate_Column;

                dsExcel = ExportHelper.GetExcelData(_dicQuery,out _DBConnString);
                if (dsExcel != null)
                {
                    bsTable.DataSource = dsExcel.Tables[_strTableName];
                    bsCos.DataSource = dsExcel.Tables[_strColName];

                    dgvTableList.DataSource = bsTable;
                    dgvColList.DataSource = bsCos;
                    //初始化变量
                    //MsgHelper.ShowInfo("导入成功！");
                    lblInfo.Text = _strImportSuccess;
                    tsbAutoSQL.Enabled = true;
                    _importDBType = _selectDBType;
                }
            }
            catch (Exception ex)
            {
                MsgHelper.ShowErr(ex.Message);
            }
        } 
        #endregion

        #region 生成SQL按钮事件
        private void tsbAutoSQL_Click(object sender, EventArgs e)
        {
            int iDbType = int.Parse(cbbDbType.SelectedValue.ToString());
            _selectDBType = (DataBaseType)iDbType;
            using (OleDbConnection con = new OleDbConnection(_DBConnString))
            {
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                try
                {
                    #region 提交字符处理
                    string strDataStyle = cbbThree.SelectedValue.ToString();
                    string strCommit = "";//提交字符
                    //string strCommitLast = "";
                    strCommit = "\n";
                    if (_selectDBType == DataBaseType.SqlServer)
                    {
                        strCommit = "GO\n";
                    }
                    else if (_selectDBType == DataBaseType.Oracle)
                    {
                        strCommit = "commit;\n";
                    }
                    else
                    {
                        strCommit = "\n";
                    }
                    #endregion
                    //sbAllSql为本次生成的SQL存储对象
                    StringBuilder sbAllSql = new StringBuilder();
                    sbAllSql.Append("/***********************************************************************************\n");
                    sbAllSql.Append("* 脚本描述: 初始化数据\n");
                    sbAllSql.Append("* 创建作者: \n");
                    sbAllSql.Append("* 创建日期: " + DateTime.Now.ToShortDateString() + " \n");
                    sbAllSql.Append("* 使用模块：\n");
                    sbAllSql.Append("* 使用版本: \n");
                    sbAllSql.Append("* 说    明：\n");
                    int iTable = 1;
                    foreach (DataRow dr in dsExcel.Tables[_strTableName].Rows)
                    {
                        sbAllSql.Append("**\t" + iTable + "  " + dr["表名"].ToString() + dr["类型"].ToString() + "数据\n");
                        iTable++;
                    }
                    sbAllSql.Append("***********************************************************************************/\n");
                    iTable = 1;
                    foreach (DataRow drTable in dsExcel.Tables[_strTableName].Rows)//针对表清单循环
                    {
                        string strSheet = drTable["数据Sheet名"].ToString().Trim();
                        string strDataTableName = drTable["表名"].ToString().Trim();
                        string strDataDealType = drTable["类型"].ToString().Trim();
                        string strColQuerySql = "";//插入列查询SQL，有固定值的不包括
                        string strColQueryAllSql = "";//查询插入所有列清单
                        string strColInsertSql = "";//新增数据列清单
                        string strColUpdateSql = "";//修改的条件
                        string strColUpdateConditionSql = "";//修改的条件
                        string strDataDealType_Modify = "修改";
                        string strColModifyCondition_Yes = "是";
                        int iModifyCondition = 0;
                        sbAllSql.Append("/**" + iTable.ToString() + "  " + strDataTableName + strDataDealType + "数据*/\n");
                        #region 数据变更表必须有列配置判断
                        //筛选出表的列清单
                        DataRow[] dtColRowListAll = dsExcel.Tables[_strColName].Select(" 表名='" + strDataTableName + "'");
                        if (dtColRowListAll.Length == 0)
                        {
                            MessageBox.Show("生成失败，" + strDataTableName + "表没有列！", "提示", MessageBoxButtons.OK);
                            return;
                        }
                        DataRow[] dtColRowList = dsExcel.Tables[_strColName].Select(" 表名='" + strDataTableName + "' and (固定值 is null or 是否修改条件='是')");
                        #endregion

                        #region 修改表记录必须有条件判断
                        DataRow[] dtTableModifyList = dsExcel.Tables[_strTableName].Select(" 类型='" + strDataDealType_Modify + "'");
                        foreach (DataRow dr in dtTableModifyList)
                        {
                            DataRow[] dtColModifyConditionList = dsExcel.Tables[_strColName].Select(" 表名='" + dr["表名"].ToString().Trim() + "' and 是否修改条件='" + strColModifyCondition_Yes + "'");
                            if (dtColModifyConditionList.Length == 0)
                            {
                                MessageBox.Show("生成失败，修改的表" + dr["表名"].ToString().Trim() + "中“是否修改条件”为至少有一个字段为“是”！", "提示", MessageBoxButtons.OK);
                                return;
                            }
                        }
                        #endregion

                        #region 确定查询列SQL、更新SQL和条件、新增SQL，将SQL参数化（格式：#+列名+#）
                        foreach (DataRow drCol in dtColRowListAll)//针对列清单循环
                        {
                            string strColCode = drCol["列名"].ToString().Trim().ToUpper();
                            string strColFixedValue = drCol["固定值"].ToString().Trim().ToUpper();
                            string strColModifyCondition = drCol["是否修改条件"].ToString().Trim();
                            string strColIsRemoveYH = drCol["是否不加引号"].ToString().Trim();
                            string strColIsHelp = drCol["是否辅助列"].ToString().Trim();
                            //查询所有列：除了辅助列
                            if (strColIsHelp != strColModifyCondition_Yes)
                            {
                                strColQueryAllSql += strColCode + ",";
                            }
                            //查询数据列清单：辅助列、没有固定值、修改条件
                            if (strColIsHelp == strColModifyCondition_Yes || string.IsNullOrEmpty(strColFixedValue) || strColModifyCondition == strColModifyCondition_Yes)
                            {
                                strColQuerySql += "[" + strColCode + "],";//插入列查询SQL，有固定值的不包括。这里加上[]是为了避免跟Excel数据源的关键字一致而报错
                                if (strColIsHelp == strColModifyCondition_Yes)
                                {
                                    continue;//辅助列只查询
                                }
                            }
                            if (strDataDealType == strDataDealType_Modify)//修改数据处理
                            {
                                if (strColModifyCondition == strColModifyCondition_Yes)//修改主键处理
                                {
                                    //修改条件SQL的确定
                                    if (iModifyCondition == 0)
                                    {
                                        strColUpdateConditionSql += strColCode + "='#" + strColCode + "#'";//将SQL参数化
                                    }
                                    else
                                    {
                                        strColUpdateConditionSql += " and " + strColCode + "='#" + strColCode + "#'";//将SQL参数化
                                    }
                                    iModifyCondition++;
                                }
                                else if (!string.IsNullOrEmpty(strColFixedValue))//有固定值
                                {

                                    strColUpdateSql += strColCode + "=" + strColFixedValue + ",";
                                }
                                else if (strColIsRemoveYH == strColModifyCondition_Yes)
                                {
                                    strColUpdateSql += strColCode + "=#" + strColCode + "#,";//将SQL参数化：SQL时把单引号去掉
                                }
                                else
                                {
                                    strColUpdateSql += strColCode + "='#" + strColCode + "#',";//将SQL参数化
                                }
                            }
                            else//新增数据处理
                            {
                                if (!string.IsNullOrEmpty(strColFixedValue))
                                {
                                    strColInsertSql += strColFixedValue + ",";
                                }
                                else if (strColIsRemoveYH == strColModifyCondition_Yes)
                                {
                                    strColInsertSql += "#" + strColCode + "#,";//将SQL参数化：SQL时把单引号去掉
                                }
                                else
                                {
                                    strColInsertSql += "'#" + strColCode + "#',";//将SQL参数化
                                }
                            }
                        }
                        #endregion
                        strColQuerySql = strColQuerySql.Substring(0, strColQuerySql.Length - 1);
                        strColQueryAllSql = strColQueryAllSql.Substring(0, strColQueryAllSql.Length - 1);
                        //获取数据信息
                        string strColDataList = "SELECT " + strColQuerySql + "  FROM [" + drTable["数据Sheet名"].ToString().Trim() + "$] ";
                        //“IErrorInfo.GetDescription 因 E_FAIL(0x80004005) 而失败。”这个错误，查了半天才知道是因为使用了Access的关键字
                        //在Access中和在MS SQL中一样，对关键字的转义是使用“[”和“]”的，比如你的数据库中某个表中有以上述关键字作为字段名的，记得在该字段前后分别加上“[”和“]”，如[user],[password],[yes]，不过建议还是尽量避免使用关键字。
                        OleDbDataAdapter daOneTable = new OleDbDataAdapter(strColDataList, con);
                        daOneTable.Fill(dsExcel, strDataTableName);
                        //移除空行
                        dsExcel.Tables[strDataTableName].DeleteNullRow();
                        //生成SQL的前缀和后缀 
                        string strInsertPre = "insert into " + strDataTableName + "(" + strColQueryAllSql + ") \nselect ";
                        string strInsertOracleEnd = " from dual;";
                        string strUpdatePre = "update " + strDataTableName + " set ";
                        string strUpdateOracleEnd = ";";
                        //用于判断增加提交字符的变量
                        int iDataNum = 1;//数据计数器
                        int iCommitCount = Convert.ToInt32(strDataStyle);
                        int iDataRowCount = dsExcel.Tables[strDataTableName].Rows.Count;
                        if (strDataDealType == strDataDealType_Modify)//修改数据
                        {
                            #region 修改数据
                            //更新条件的最终确定
                            strColUpdateConditionSql = " WHERE " + strColUpdateConditionSql;
                            foreach (DataRow drColData in dsExcel.Tables[strDataTableName].Rows)//导入数据表内循环
                            {
                                string strColUpdateSql_Replace = strColUpdateSql;
                                string strColUpdateConditionSql_Replace = strColUpdateConditionSql;
                                foreach (DataRow drCol in dtColRowList)//针对列清单循环来替换参数
                                {
                                    string strColCode = drCol["列名"].ToString().Trim().ToUpper();
                                    string strColIsRemoveYH2 = drCol["是否不加引号"].ToString().Trim();
                                    string strDataValue = drColData[strColCode].ToString().Trim();
                                    if (string.IsNullOrEmpty(strColIsRemoveYH2) || strColIsRemoveYH2 != strColModifyCondition_Yes)
                                    {
                                        //去掉单引号
                                        strDataValue = drColData[strColCode].ToString().Trim().Replace("'", "");
                                    }
                                    if (string.IsNullOrEmpty(drColData[strColCode].ToString().Trim()))
                                    {
                                        //空白时赋值null
                                        strColUpdateSql_Replace = strColUpdateSql_Replace.Replace("'#" + strColCode + "#'", "null");
                                    }
                                    else
                                    {
                                        //将参数化的字段以实际数据代替
                                        strColUpdateSql_Replace = strColUpdateSql_Replace.Replace("#" + strColCode + "#", strDataValue);
                                        strColUpdateConditionSql_Replace = strColUpdateConditionSql_Replace.Replace("#" + strColCode + "#", strDataValue);
                                    }
                                }
                                string strSQLOneData = strUpdatePre + strColUpdateSql_Replace.Substring(0, strColUpdateSql_Replace.Length - 1) + strColUpdateConditionSql_Replace;
                                //生成单个数据SQL
                                strSQLOneData = DataBaseCommon.GenOneDataSql(_selectDBType, strDataStyle, strCommit, strUpdateOracleEnd, strSQLOneData, iDataNum, iCommitCount, iDataRowCount);
                                sbAllSql.Append(strSQLOneData);
                                iDataNum++;
                            }
                            #endregion
                        }
                        else//新增数据
                        {
                            #region 新增数据
                            foreach (DataRow drColData in dsExcel.Tables[strDataTableName].Rows)//导入数据表内循环
                            {
                                string strColInsertSql_Replace = strColInsertSql;
                                foreach (DataRow drCol in dtColRowList)//针对列清单循环来替换参数
                                {
                                    string strColCode = drCol["列名"].ToString().Trim().ToUpper();
                                    string strColIsSQL2 = drCol["是否不加引号"].ToString().Trim();
                                    string strDataValue = drColData[strColCode].ToString().Trim();
                                    if (string.IsNullOrEmpty(strColIsSQL2) || strColIsSQL2 != strColModifyCondition_Yes)
                                    {
                                        //非SQL时要去掉单引号，SQL不用
                                        strDataValue = drColData[strColCode].ToString().Trim().Replace("'", "");
                                    }
                                    if (dsExcel.Tables[strDataTableName].Columns.Contains(strColCode))
                                    {
                                        if (string.IsNullOrEmpty(drColData[strColCode].ToString().Trim()))
                                        {
                                            //空白时赋值null
                                            strColInsertSql_Replace = strColInsertSql_Replace.Replace("'#" + strColCode + "#'", "null");
                                        }
                                        else
                                        {
                                            //将参数化的字段以实际数据代替
                                            strColInsertSql_Replace = strColInsertSql_Replace.Replace("#" + strColCode + "#", strDataValue);
                                        }
                                    }
                                }
                                string strSQLOneData = strInsertPre + strColInsertSql_Replace.Substring(0, strColInsertSql_Replace.Length - 1);
                                //生成单个数据SQL
                                strSQLOneData = DataBaseCommon.GenOneDataSql(_selectDBType, strDataStyle, strCommit, strInsertOracleEnd, strSQLOneData, iDataNum, iCommitCount, iDataRowCount);
                                sbAllSql.Append(strSQLOneData);
                                iDataNum++;
                            }
                            #endregion
                        }
                        iTable++;
                    }//表循环结束
                    rtbResult.Clear();
                    if (strDataStyle == "0")//全部
                    {
                        sbAllSql.Append("\n" + strCommit);
                    }
                    else
                    {
                        sbAllSql.Append("\n");
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
                    MsgHelper.ShowErr(ex.Message);
                    return;
                }
            }//连接结束
        } 
        #endregion

        #region 下载模板按钮事件
        private void tsbDownLoad_Click(object sender, EventArgs e)
        {
            DBToolUIHelper.DownloadFile(@"DataModel\DataSql\模板_生成数据.xlsx", "生成数据模板", true);
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

        #region 数据库类型选择变化事件
        private void cbbDbType_SelectedIndexChanged(object sender, EventArgs e)
        {
            int iDbType = int.Parse(cbbDbType.SelectedValue.ToString());
            if (_importDBType == (DataBaseType)iDbType)
            {
                tsbAutoSQL.Enabled = true;
            }
            else
            {
                tsbAutoSQL.Enabled = false;
            }
        } 
        #endregion
    }
}
