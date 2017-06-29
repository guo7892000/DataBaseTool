using Breezee.Framework.BaseUI;
using Breezee.Framework.Tool;
using Breezee.Global.Entity;
using Breezee.WorkHelper.DBTool.Entity;
using System;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Breezee.WorkHelper.DBTool.UI
{
    /// <summary>
    /// Excel导入生成表结构
    /// </summary>
    public partial class FrmDBTImportExcelGenerateTableSQL : BaseForm
    {
        #region 变量
        //创建类型下拉框
        private readonly string _strCreate_AddOnly = "0";//只是新增
        private readonly string _strCreate_DeleteAndAdd = "1";//先删后增
        private readonly string _strCreate_Delete = "2";//只是删除
        //模板中的列处理类型
        private static readonly string _strDealTypeAdd = "新增"; //新增处理类型
        private static readonly string _strDealTypeModify = "修改"; //修改处理类型
        private static readonly string _strDealTypeDelete = "删除"; //删除处理类型
        private static readonly string _strDealTypeDeleteAndAdd = "先删后增"; //先删后增处理类型
        //
        private readonly string _strTableName = "变更表清单";
        private readonly string _strColName = "变更列清单";
        //数据库类型
        private DataBaseType _importDBType;//导入数据库类型
        private DataBaseType _targetDBType;//目标数据库类型
        private bool _isAllConvert = false;//是否综合转换，即导入一种数据库模块，而生成另一种数据库类型
        //
        private BindingSource bsTable = new BindingSource();
        private BindingSource bsCos = new BindingSource();
        //
        private readonly string _strNull = " NULL";   //可空
        private readonly string _strNotNull = " NOT NULL"; //非空
        private static readonly string _strBlank = " "; //空格
        //
        private string _strAutoSqlSuccess = "生成成功，并已复制到了粘贴板。详细见“生成的SQL”页签！";
        private string _strImportSuccess = "导入成功！可点“生成SQL”按钮得到本次导入的变更SQL。";
        private string strTipInfo = "不需要的行，请选择整行后，按Delete键即可删除！";
        //
        private bool _isSqlServerDefaultValueNameAuto = false;//是否SqlServer默认值名称自动化，false即使用
        //生成过程用到的全局变量
        private StringBuilder sbSql = new StringBuilder();
        private StringBuilder sbRemark = new StringBuilder();
        private StringBuilder sbSqlFisrt;
        private DataTable dtTable;
        private DataTable dtAllCol;
        private string strCreateType;
        private int iCalNum = 1;
        private string strSql = "";
        private string strPrivSql = "";
        private string strDeleteSql = "";
        //
        private string _tableColumnAroundChar_Left = "";//围绕表或列名的左字符，例如SqlServer的[
        private string _tableColumnAroundChar_Right = "";//围绕表或列名的左字符，例如SqlServer的]
        #endregion

        #region 构造函数
        public FrmDBTImportExcelGenerateTableSQL()
        {
            InitializeComponent();
        }
        #endregion

        #region 加载事件
        private void FrmDBTImportExcelGenerateTableSQL_Load(object sender, EventArgs e)
        {
            DataTable dtDbType = DBToolUIHelper.GetBaseDataTypeTable();
            //目标数据库类型
            UIHelper.BindTypeValueDropDownList(cbbTargetDbType, dtDbType, false, true);
            //导入数据库类型
            UIHelper.BindTypeValueDropDownList(cbbImportDBType, dtDbType, false, true);
            
            //创建方式
            _dicString.Add(_strCreate_AddOnly, "不判断增加");
            _dicString.Add(_strCreate_DeleteAndAdd, "先删后增加");
            _dicString.Add(_strCreate_Delete, "生成删除SQL");
            UIHelper.BindTypeValueDropDownList(cbbCreateType, UIHelper.GetTextValueTable(_dicString, false), false, true);
            _dicString.Clear();
            //设置表、列的删除提示
            lblTableData.Text = strTipInfo;
            lblColumnInfo.Text = strTipInfo;
        }
        #endregion

        #region 导入按钮事件
        private void tsbImport_Click(object sender, EventArgs e)
        {
            _dicQuery[_strTableName] = DBToolStaticString.TableStructGenerate_Table;

            int iDbType = int.Parse(cbbImportDBType.SelectedValue.ToString());
            _importDBType = (DataBaseType)iDbType;

            if (ckbAllConvert.Checked)
            {
                _dicQuery[_strColName] = DBToolStaticString.TableStructGenerate_Column_AllSql;
                _isAllConvert = true;
            }
            else
            {
                switch (_importDBType)
                {
                    case DataBaseType.SqlServer:
                        _dicQuery[_strColName] = DBToolStaticString.TableStructGenerate_Column_SqlServer;
                        break;
                    case DataBaseType.Oracle:
                        _dicQuery[_strColName] = DBToolStaticString.TableStructGenerate_Column_Oralce;
                        break;
                    case DataBaseType.MySql:
                        _dicQuery[_strColName] = DBToolStaticString.TableStructGenerate_Column_MySql;
                        break;
                    case DataBaseType.SQLite:
                        _dicQuery[_strColName] = DBToolStaticString.TableStructGenerate_Column_SqLite;
                        break;
                    case DataBaseType.PostgreSql:
                        _dicQuery[_strColName] = DBToolStaticString.TableStructGenerate_Column_PostgreSql;
                        break;
                    default:
                        throw new Exception("暂不支持该数据库类型！");
                        //break;
                }
            }

            DataSet ds = ExportHelper.GetExcelData(_dicQuery);
            if (ds != null)
            {
                bsTable.DataSource = ds.Tables[_strTableName];
                bsCos.DataSource = ds.Tables[_strColName];

                dgvTableList.DataSource = bsTable;
                dgvColList.DataSource = bsCos;
                //MsgHelper.ShowInfo("导入成功！");
                lblInfo.Text = _strImportSuccess;
                ckbAllConvert.Enabled = false;
            }

        }
        #endregion

        #region 生成SQL按钮事件
        private void tsbAutoSQL_Click(object sender, EventArgs e)
        {
            //移除数据库变更记录页签
            TableStructGenerator.RemoveTab(tabControl1);
            rtbResult.Clear();

            #region 结束编辑处理
            bsTable.EndEdit();
            dtTable = (DataTable)bsTable.DataSource;
            dtAllCol = (DataTable)bsCos.DataSource;

            if (dtTable == null || dtTable.Rows.Count == 0)
            {
                MsgHelper.ShowErr("请先导入数据库结构生成模块！");
                return;
            }

            //移除空行
            foreach (DataRow dr in dtTable.Select("表编码 is null"))
            {
                dtTable.Rows.Remove(dr);
            }
            foreach (DataRow dr in dtAllCol.Select("列编码 is null or 表编码 is null"))
            {
                dtAllCol.Rows.Remove(dr);
            }

            dtTable.AcceptChanges();
            dtAllCol.AcceptChanges();
            #endregion

            #region 变量
            sbSql = new StringBuilder();
            sbRemark = new StringBuilder();
            iCalNum = 1;
            strSql = "";
            strPrivSql = "";
            strDeleteSql = "";
            #endregion

            //目标数据库类型
            int iDbType = int.Parse(cbbTargetDbType.SelectedValue.ToString());
            _targetDBType = (DataBaseType)iDbType;
            //创建方式
            strCreateType = cbbCreateType.SelectedValue.ToString();

            if (!ValidateData())//校验数据
            {
                return;
            }

            sbSqlFisrt = new StringBuilder();//SQL前缀
            int iTable = 1;

            #region Sql前缀处理
            sbSqlFisrt.Append("/***********************************************************************************\n");
            sbSqlFisrt.Append("* 脚本描述: 新增修改表\n");
            sbSqlFisrt.Append("* 创建作者: \n");
            sbSqlFisrt.Append("* 创建日期: " + DateTime.Now.ToShortDateString() + " \n");
            sbSqlFisrt.Append("* 使用模块：\n");
            sbSqlFisrt.Append("* 使用版本: \n");
            sbSqlFisrt.Append("* 说    明：\n");

            foreach (DataRow dr in dtTable.Rows)
            {
                sbSqlFisrt.Append("      " + iTable + " " + dr["变更类型"].ToString() + dr["表名称"].ToString() + "（" + dr["表编码"].ToString() + "）\n");
                iTable++;
            }
            sbSqlFisrt.Append("***********************************************************************************/\n");
            #endregion

            #region 表处理
            switch (_targetDBType)
            {
                case DataBaseType.SqlServer:
                    GenerateSqlServerTable();
                    break;
                case DataBaseType.Oracle:
                    GenerateOracleTable();
                    break;
                case DataBaseType.MySql:
                    GenerateMySqlTable();
                    break;
                case DataBaseType.SQLite:
                    GenerateSQLiteTable();
                    break;
                case DataBaseType.PostgreSql:
                    GeneratePostgreSqlTable();
                    break;
                default:
                    GenerateSqlServerTable();
                    break;
            }
            #endregion

            rtbResult.AppendText(sbSql.ToString() + "\n");
            Clipboard.SetData(DataFormats.UnicodeText, sbSql.ToString());
            tabControl1.SelectedTab = tpAutoSQL;

            //增加生成表结构的功能
            dtAllCol.AcceptChanges();
            TableStructGenerator.Generate(tabControl1, dtTable, dtAllCol);
            //生成SQL成功后提示
            ShowSuccessMsg(_strAutoSqlSuccess);
            //初始化控件
            ckbAllConvert.Enabled = true;
        }
        #endregion

        #region 导入数据库类型选择变化
        private void cbbImportDBType_SelectedIndexChanged(object sender, EventArgs e)
        {
            TableStructGenerator.RemoveTab(tabControl1);
            
            if (ckbAllConvert.Checked)
            {
                #region 综合转换
                cbbTargetDbType.Enabled = true; 
                #endregion
            }
            else
            {
                #region 特定模板导入
                dgvTableList.DataSource = null;
                dgvColList.DataSource = null;
                //默认的生成类型跟导入的一样
                cbbTargetDbType.SelectedValue = cbbImportDBType.SelectedValue.ToString();
                cbbTargetDbType.Enabled = false; 
                #endregion
            }
        }
        #endregion

        #region 下载模板按钮事件
        private void tsbDownLoad_Click(object sender, EventArgs e)
        {
            DBToolUIHelper.DownloadFile(@"DataModel\TableStruct\模板_表列变更.xlsx", "表列变更模板",true);
        } 
        #endregion

        #region 退出按钮事件
        private void tsbExit_Click(object sender, EventArgs e)
        {
            Close();
        }
        #endregion

        #region 增加左边空格方法
        private static string AddLeftBand(string strColCode)
        {
            return _strBlank + strColCode;
        }
        #endregion

        #region 增加右边空格方法
        private static string AddRightBand(string strColCode)
        {
            return strColCode + _strBlank;
        }
        #endregion

        #region 增加左右边空格方法
        /// <summary>
        /// 增加左右边空格方法
        /// </summary>
        /// <param name="strColCode"></param>
        /// <returns></returns>
        private static string AddLeftRightBand(string strColCode)
        {
            return _strBlank + strColCode + _strBlank;
        }
        #endregion

        #region 增加左右括号方法
        private string AddLeftRightKuoHao(string strColCode)
        {
            return "(" + strColCode + ")";
        }
        #endregion

        #region 增加左右单引号方法
        private string ChangeIntoSqlString(string strColCode)
        {
            return " '" + strColCode + "' ";
        }
        #endregion

        #region 返回注释说明
        private string ChangeIntoInfo(string strColCode)
        {
            return "/*" + strColCode + "*/";
        }
        #endregion

        #region 成功提示方法
        /// <summary>
        /// 成功提示方法
        /// </summary>
        private void ShowSuccessMsg(string strInfo)
        {
            //MessageBox.Show(strInfo, "生成成功", MessageBoxButtons.OK);
            lblInfo.Text = strInfo;
            rtbResult.Select(0, 0); //返回到第一行
        }
        #endregion

        #region 检查数据的有效性
        private bool ValidateData()
        {
            #region 全部判断
            if (dtTable.Select("变更类型 not in ('新增','修改')").Length > 0)
            {
                MsgHelper.ShowErr("变更类型只能是“新增”或“修改”！");
                return false;
            }
            if (dtTable.Select("变更类型='新增' and (表名称='' or 表编码='')").Length > 0)
            {
                MsgHelper.ShowErr("新增的表，其“表名称、表编码”都不能为空！");
                return false;
            }
            if (dtAllCol.Select("表编码=''").Length > 0)
            {
                MsgHelper.ShowErr("新增的列中表编码不能为空！");
                return false;
            }
            if (dtAllCol.Select("表编码='' or 列名称='' or 列编码='' or 类型=''").Length > 0)
            {
                MsgHelper.ShowErr("新增的列中表编码、列名称、列编码、类型不能为空！");
                return false;
            }
            foreach (DataRow dr in dtAllCol.Select("(类型 like 'VARCHAR%' or 类型 like 'NVARCHAR%' or 类型 like 'CHAR%') and (长度 is null)"))
            {
                MsgHelper.ShowErr(dr["表编码"].ToString() + "的" + dr["列编码"].ToString() + "，其" + dr["类型"].ToString() + "类型的长度不能为空！");
                return false;
            }
            #endregion

            #region 新增表和列的判断
            DataRow[] drNewArray = dtTable.Select("变更类型='新增'");
            
            foreach (DataRow drNew in drNewArray)
            {
                string strTableCode = drNew["表编码"].ToString();
                string strTableName = drNew["表名称"].ToString();
                string strChangeType = drNew["变更类型"].ToString();
                if (dtAllCol.Select("表编码='" + strTableCode + "'").Length == 0)
                {
                    MsgHelper.ShowErr("表" + strTableCode + "中没有本次更变的列，请删除该表或新增列！");
                    return false;
                }
                if (dtAllCol.Select("表编码='" + strTableCode + "' and (变更类型 is not null and 变更类型<>'新增')").Length > 0)
                {
                    MsgHelper.ShowErr("新增的表" + strTableCode + "中，只能全部为新增列！");
                    return false;
                }

                if (dtAllCol.Select("表编码='" + strTableCode + "' and 键='PK'").Length == 0)
                {
                    MsgHelper.ShowErr("新增的表" + strTableCode + "没有主键！");
                    return false;
                }

                if (_isAllConvert)
                {
                    #region 综合转换
                    if (_targetDBType == DataBaseType.Oracle)
                    {
                        if (dtAllCol.Select("表编码='" + strTableCode + "' and 键='FK' and (Oracle外键 is not null and Oracle外键名 not like 'FK_%')").Length > 0)
                        {
                            MessageBox.Show("新增的表" + strTableCode + "中键为“FK”时，“Oracle外键名”列内容格式必须以“FK_”开头！");
                            return false;
                        }

                        if (dtAllCol.Select("表编码='" + strTableCode + "' and (Oracle序列名 is not null and Oracle序列名 not like 'SQ_%')").Length > 0)
                        {
                            MsgHelper.ShowErr("新增的表" + strTableCode + "中，“Oracle序列名”列内容格式必须以“SQ_”开头！");
                            return false;
                        }
                        if (dtAllCol.Select("表编码='" + strTableCode + "' and (Oracle唯一约束名 is not null and Oracle唯一约束名 not like 'UQ_%')").Length > 0)
                        {
                            MsgHelper.ShowErr("新增的表" + strTableCode + "中，“Oracle唯一约束名”列内容格式必须以“UQ_”开头！");
                            return false;
                        }
                        if (dtAllCol.Select("表编码='" + strTableCode + "' and 键='FK' and (Oracle外键 is null or Oracle外键名 is null)").Length > 0)
                        {
                            MsgHelper.ShowErr("新增的表" + strTableCode + "中键为“FK”时，“Oracle外键、Oracle外键名”列为必填！");
                            return false;
                        }
                    }
                    #endregion
                }
                else
                {
                    #region 非综合转换
                    if (_importDBType == DataBaseType.Oracle)
                    {
                        if (dtAllCol.Select("表编码='" + strTableCode + "' and 键='FK' and (外键 is not null and 外键名 not like 'FK_%')").Length > 0)
                        {
                            MessageBox.Show("新增的表" + strTableCode + "中键为“FK”时，“外键名”列内容格式必须以“FK_”开头！");
                            return false;
                        }

                        if (dtAllCol.Select("表编码='" + strTableCode + "' and (序列名 is not null and 序列名 not like 'SQ_%')").Length > 0)
                        {
                            MsgHelper.ShowErr("新增的表" + strTableCode + "中，“序列名”列内容格式必须以“SQ_”开头！");
                            return false;
                        }
                        if (dtAllCol.Select("表编码='" + strTableCode + "' and (唯一约束名 is not null and 唯一约束名 not like 'UQ_%')").Length > 0)
                        {
                            MsgHelper.ShowErr("新增的表" + strTableCode + "中，“唯一约束名”列内容格式必须以“UQ_”开头！");
                            return false;
                        }
                        if (dtAllCol.Select("表编码='" + strTableCode + "' and 键='FK' and (外键 is null or 外键名 is null)").Length > 0)
                        {
                            MsgHelper.ShowErr("新增的表" + strTableCode + "中键为“FK”时，“外键、外键名”列为必填！");
                            return false;
                        }
                    } 
                    #endregion
                }
                
            }
            #endregion

            #region 检查字段是否符合规范(已取消)
            //bool isRuleCheck = ckbRuleCheck.Checked;
            //StringBuilder sbNameDiff = new StringBuilder();
            //StringBuilder sbCodeDiff = new StringBuilder();
            //if (isRuleCheck)
            //{
            //    DataTable dtDic = GetDicDataTable();
            //    if (dtDic != null && dtDic.Rows.Count > 0)
            //    {
            //        for (int i = 0; i < dtTable.Rows.Count; i++)
            //        {
            //            string strTableName = dtTable.Rows[i]["表编码"].ToString().Trim();
            //            DataRow[] drColArr = dtAllCol.Select(" 表编码= '" + strTableName + "'");
            //            for (int j = 0; j < drColArr.Length; j++)
            //            {
            //                string strColCode = drColArr[j]["列编码"].ToString().Trim().ToUpper();
            //                string strColName = drColArr[j]["列名称"].ToString().Trim();
            //                string strColDataType = drColArr[j]["类型"].ToString().Trim().ToUpper();
            //                string strColLen = drColArr[j]["长度"].ToString().Trim();
            //                DataRow[] drDicArr = dtDic.Select(" COLUMN_CODE= '" + strColCode + "'");
            //                if (drDicArr.Length > 0)
            //                {
            //                    //编码一样，但其他字段不一样
            //                    string strSColumnName = drDicArr[0]["COLUMN_NAME"].ToString().Trim();
            //                    string strSDataType = drDicArr[0]["DATA_TYPE"].ToString().Trim();
            //                    string strSOracleType = drDicArr[0]["ORACLE_TYPE"].ToString().Trim();
            //                    string strSColumnLen = drDicArr[0]["COLUMN_LENGTH"].ToString().Trim();
            //                    if (strSColumnName != strColName //名称不一样
            //                        || CommonTool.IsRightToStandard(strSDataType, strColDataType, DataBaseType.Oracle) //类型不一样
            //                        || strSColumnLen != strColLen //长度不一样
            //                        )
            //                    {
            //                        sbNameDiff.Append(strTableName + "表" + strColCode + "列：" + strSColumnName + "[" + strColName + "]，"
            //                            + strSDataType + "[" + strColDataType + "]，"
            //                            + strSColumnLen + "[" + strColLen + "]\n");
            //                    }
            //                }

            //                DataRow[] drDicNameArr = dtDic.Select(" COLUMN_NAME= '" + strColName + "'");
            //                if (drDicNameArr.Length > 0)
            //                {
            //                    //名称一样，但其他字段不一样
            //                    string strSColumnCode = drDicNameArr[0]["COLUMN_CODE"].ToString().Trim();
            //                    string strSDataType = drDicNameArr[0]["DATA_TYPE"].ToString().Trim();
            //                    string strSOracleType = drDicNameArr[0]["ORACLE_TYPE"].ToString().Trim();
            //                    string strSColumnLen = drDicNameArr[0]["COLUMN_LENGTH"].ToString().Trim();
            //                    if (strSColumnCode != strColCode
            //                        || CommonTool.IsRightToStandard(strSDataType, strColDataType, DataBaseType.Oracle) //类型不一样
            //                        || strSColumnLen != strColLen)
            //                    {
            //                        sbCodeDiff.Append(strTableName + "表" + strColName + "列：" + strSColumnCode + "[" + strColCode + "]，"
            //                            + strSDataType + "[" + strColDataType + "]，"
            //                            + strSColumnLen + "[" + strColLen + "]\n");
            //                    }
            //                }
            //            }
            //        }
            //    }

            //    if (!string.IsNullOrEmpty(sbNameDiff.ToString()))
            //    {
            //        sbNameDiff.Insert(0, "编码一样，名称、类型、长度三者之一不一样的列有：\n");
            //    }
            //    if (!string.IsNullOrEmpty(sbCodeDiff.ToString()))
            //    {
            //        sbCodeDiff.Insert(0, "名称一样，编码、类型、长度三者之一不一样的列有：\n");
            //    }
            //    if (!string.IsNullOrEmpty(sbNameDiff.ToString() + sbCodeDiff.ToString()))
            //    {
            //        DialogResult drSelected = MessageBox.Show(this, "存在跟标准不一致的字段，是否继续？选“是”则继续，\n选“否”则显示不符合标准字段信息，\n选“取消”则返回？", "提示", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
            //        if (drSelected == DialogResult.Yes)
            //        {
            //            rtbResult.Clear();
            //            rtbResult.AppendText(sbNameDiff.ToString() + sbCodeDiff.ToString());
            //            Clipboard.SetData(DataFormats.UnicodeText, sbSql.ToString());
            //            tabControl1.SelectedTab = tpAutoSQL;
            //            return;
            //        }
            //        else if (drSelected == DialogResult.Cancel)
            //        {
            //            return;
            //        }
            //    }
            //}
            #endregion 

            return true;
        }
        #endregion

        #region 生成SqlServer表
        private void GenerateSqlServerTable()
        {
            foreach (DataRow dr in dtTable.Rows)
            {
                string strTableCode = dr["表编码"].ToString();
                string strTableName = dr["表名称"].ToString();
                DataRow[] drList = dtAllCol.Select(" 表编码= '" + strTableCode + "'");
                string strPK = "";
                if (dr["变更类型"].ToString() == "新增")
                {
                    #region 新增处理
                    sbSql.Append("/*" + iCalNum.ToString() + "、新增表：" + strTableName + AddLeftRightKuoHao(strTableCode) + "*/\n");

                    if (strCreateType == _strCreate_AddOnly)
                    {
                        sbSql.Append("IF OBJECT_ID('" + strTableCode + "', 'U') IS  NULL \n BEGIN \n");
                    }
                    else if (strCreateType == _strCreate_DeleteAndAdd || strCreateType == _strCreate_Delete)
                    {
                        strDeleteSql = strDeleteSql.Insert(0, "IF OBJECT_ID('" + strTableCode + "', 'U') IS NOT NULL \n BEGIN \n"
                            + "\tDROP TABLE " + strTableCode + "\n END\nGO\n");
                        if (strCreateType == _strCreate_Delete)
                        {
                            continue;
                        }
                    }

                    sbSql.Append(AddRightBand("CREATE TABLE") + AddRightBand(strTableCode) + AddRightBand("\n(\n"));
                    //表说明SQL
                    sbRemark.Append("EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'" + strTableName + "：" + dr["备注"].ToString() + "',\n" +
                            "   @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'" + strTableCode + "'\n");
                    int j = drList.Length;
                    string strDefaultList = "";//默认值
                    foreach (DataRow drCol in drList)
                    {
                        //生成SqlServer的列
                        GenerateSqlServerColumn(TableDealType.Add,strTableCode, drCol, ref strPK, ref j, strTableName);
                    }
                    //最后加括号并换行
                    sbSql.Append(")\n");
                    if (strCreateType == _strCreate_AddOnly)
                    {
                        sbSql.Append(sbRemark.ToString() + " END\nGO\n");//添加列说明
                    }
                    else if (strCreateType == _strCreate_DeleteAndAdd)
                    {
                        sbSql.Append(sbRemark.ToString() + " \nGO\n");//添加列说明
                    }
                    if (!_isSqlServerDefaultValueNameAuto)
                    {
                        sbSql.Append(strDefaultList);//添加默认值
                    }
                    //重新创建备注字符对象
                    sbRemark = new StringBuilder();
                    #endregion
                }
                else
                {
                    #region 修改列处理

                    sbSql.Append("/*" + iCalNum.ToString() + "、修改表：" + strTableName + AddLeftRightKuoHao(strTableCode) + "*/\n");
                    int j = 1;
                    foreach (DataRow drCol in drList)
                    {
                        //生成SqlServer的列
                        GenerateSqlServerColumn(TableDealType.Modify, strTableCode, drCol, ref strPK, ref j, strTableName);
                    }
                    #endregion
                }
                iCalNum++;
            }
            sbSql.Insert(0, sbSqlFisrt.Append(strDeleteSql).ToString());
            if (strCreateType == _strCreate_Delete)
            {
                sbSql = new StringBuilder();
                sbSql.Append(strDeleteSql);
            }
        }

        private void GenerateSqlServerColumn(TableDealType tableDealType, string strTableCode, DataRow drCol, ref string strPK, ref int j, string strTableName)
        {
            //公共字段
            //列处理类型，当为空时表示新增
            string strColumnDealType = drCol["变更类型"].ToString().Trim() == "" ? _strDealTypeAdd : drCol["变更类型"].ToString().Trim();
            string strKey = drCol["键"].ToString().ToUpper().Trim();
            string strColCode = drCol["列编码"].ToString();
            string strColName = drCol["列名称"].ToString();
            string strColDataType = drCol["类型"].ToString().ToLower();
            string strColLen = drCol["长度"].ToString().Trim();
            string strColDecimalDigits = drCol["小数位"].ToString().Trim();
            string strColDefault = drCol["默认值"].ToString().Trim().Replace("'", "");
            //其他独有字段
            string strColUnique = _isAllConvert? drCol["SqlServer唯一性"].ToString().Trim() : drCol["唯一性"].ToString().Trim();
            string strColAddNum = _isAllConvert ? drCol["SqlServer自增长设置"].ToString().Trim() : drCol["自增长设置"].ToString().Trim();
            string strColForgKey = _isAllConvert ? drCol["SqlServer外键"].ToString().Trim() : drCol["外键"].ToString().Trim();
            //
            string strTable_Col = strTableCode + "_" + strColCode;//表编码+"_"+列编码
            string strCanNull = "";//是否可空
            string strDefault_Full = "";//默认值
            string strDefaultList = "";//修改默认值
            string strUqueList = "";//唯一性
                                    //
            #region 转换字段类型与默认值
            if (_importDBType != _targetDBType)
            {
                ConvertDbTypeDefaultValueString(ref strColDataType, ref strColDefault, _importDBType, _targetDBType);
            } 
            #endregion

            //数据类型(类型+长度+小数点)
            string sDataType_Full = GetFullTypeString(drCol, strColDataType, strColLen, strColDecimalDigits);

            if (tableDealType== TableDealType.Add)
            {
                #region 新增
                
                sbSql.Append("\t");
                if (strKey == "PK")
                {
                    //主键处理
                    if (!string.IsNullOrEmpty(strColAddNum) && (strColDataType == "int" || strColDataType == "bigint"))
                    {
                        sbSql.Append(AddRightBand(strColCode) + AddRightBand(strColDataType) + AddRightBand("IDENTITY" + AddLeftRightKuoHao(strColAddNum)
                            + " CONSTRAINT [PK_" + strTableCode + "] PRIMARY KEY(" + strColCode + ") "));
                    }
                    else
                    {
                        sbSql.Append(AddRightBand(strColCode) + AddRightBand(sDataType_Full)
                            + " CONSTRAINT [PK_" + strTableCode + "] PRIMARY KEY(" + strColCode + ") ");
                    }
                    strPK = strColCode;
                }
                else
                {
                    //没有长度的字段处理
                    sbSql.Append(AddRightBand(strColCode) + AddRightBand(sDataType_Full));
                }

                #region 非空的处理
                if (drCol["必填"].ToString() == "是")
                {
                    sbSql.Append(AddRightBand(_strNotNull));
                }
                else
                {
                    sbSql.Append(AddRightBand(_strNull));
                }
                #endregion

                #region 默认值的处理
                if (!string.IsNullOrEmpty(strColDefault))
                {
                    string strColDefaultName = "DF_" + strTable_Col;
                    string sRealDefaultValue = strColDefault.Replace("'", "");
                    if (strColDataType == "varchar" || strColDataType == "nvarchar" || strColDataType == "char" || strColDataType == "nchar")
                    {
                        sRealDefaultValue = "'" + sRealDefaultValue + "'";
                    }

                    if (_isSqlServerDefaultValueNameAuto)
                    {
                        //方式一：采用默认名
                        sbSql.Append(AddRightBand("DEFAULT(" + sRealDefaultValue + ")"));
                    }
                    else
                    {
                        //方式二：采用指定名(单独SQL)
                        //strDefaultList = strDefaultList + "\n" +
                        // "IF EXISTS(SELECT * FROM sys.objects WHERE type='D' AND name='" + strColDefaultName + "') \n" +
                        //"ALTER TABLE " + strTableCode + " DROP CONSTRAINT " + strColDefaultName + " \n" +
                        //"GO \n" +
                        //"alter table " + strTableCode + " add constraint " + strColDefaultName + "\n " +
                        //"	DEFAULT (" + sRealDefaultValue + ") for " + strColCode + " \n" +
                        //"GO	\n";

                        //方式三：采用指定名(包括在创建表列中)
                        sbSql.Append(AddRightBand(" CONSTRAINT " + strColDefaultName + " DEFAULT(" + sRealDefaultValue + ")"));
                    }
                }
                #endregion
                //唯一性处理
                if (drCol["唯一性"].ToString() == "是")
                {
                    sbSql.Append(AddRightBand("CONSTRAINT " + " UQ_" + strTable_Col + " UNIQUE"));
                }
                //外键处理
                if (strKey == "FK")
                {
                    strColForgKey = " CONSTRAINT FK_" + strTable_Col + " FOREIGN KEY (" + strColCode + ") REFERENCES " + strColForgKey;
                    sbSql.Append(strColForgKey);
                }
                //最后增加逗号
                if (j > 1)
                {
                    sbSql.Append("," + ChangeIntoInfo(strColName) + "\n");
                }
                else
                {
                    //最后一行不加逗号
                    sbSql.Append("" + ChangeIntoInfo(strColName) + "\n");
                }
                //列备注SQL
                if (string.IsNullOrEmpty(drCol["备注"].ToString()))
                {
                    sbRemark.Append("EXEC sys.sp_addextendedproperty @name = N'MS_Description', @value = N'" + strColName + "',\n" +
                        "    @level0type = N'SCHEMA',@level0name = N'dbo', @level1type = N'TABLE',@level1name = N'" + strTableCode + "',  \n" +
                        "    @level2type = N'COLUMN', @level2name = N'" + strColCode + "'  \n");
                }
                else
                {
                    sbRemark.Append("EXEC sys.sp_addextendedproperty @name = N'MS_Description', @value = N'"
                        + strColName + "：" + drCol["备注"].ToString() + "',\n" +
                        "    @level0type = N'SCHEMA',@level0name = N'dbo', @level1type = N'TABLE',@level1name = N'" + strTableCode + "',  \n" +
                        "    @level2type = N'COLUMN', @level2name = N'" + strColCode + "'  \n");
                }
                j--;
                drCol.AcceptChanges();
                #endregion
            }
            else
            {
                #region 修改表处理
                
                #region 删除列的处理
                if (strCreateType == _strCreate_Delete)
                {
                    if (strColumnDealType == _strDealTypeAdd)
                    {
                        strDeleteSql = strDeleteSql.Insert(0, "IF EXISTS(SELECT 1 FROM SYSOBJECTS A,SYSCOLUMNS B \n" +
                            " WHERE A.ID = B.ID AND A.NAME = '" + strTableCode + "' AND B.NAME = '" + strColCode + "' AND A.TYPE = 'U') \n" +
                            " BEGIN \n" +
                            "    ALTER TABLE " + strTableCode + " DROP COLUMN " + strColCode + "\n" +
                            " END\nGO\n");
                    }
                    return;// continue;
                }
                #endregion
                
                #region 非空的处理
                if (drCol["必填"].ToString() == "是")
                {
                    strCanNull = AddRightBand(_strNotNull);
                }
                else
                {
                    strCanNull = AddRightBand(_strNull);
                }
                #endregion
                #region 默认值的处理
                if (!string.IsNullOrEmpty(strColDefault))
                {
                    string strColDefaultName = "DF_" + strTable_Col;
                    if (strColDataType == "varchar" || strColDataType == "nvarchar" || strColDataType == "char" || strColDataType == "nchar")
                    {
                        #region 字符型处理
                        if (_isSqlServerDefaultValueNameAuto)
                        {
                            //方式一：采用默认名
                            strDefault_Full = AddRightBand("default('" + strColDefault + "')");
                        }
                        else
                        {
                            //方式二：采用指定名
                            strDefaultList = "\n" +
                             "IF EXISTS(SELECT * FROM SYS.OBJECTS WHERE TYPE='D' AND NAME='" + strColDefaultName + "') \n" +
                            "ALTER TABLE " + strTableCode + " DROP CONSTRAINT " + strColDefaultName + " \n" +
                            "GO \n" +
                            "ALTER TABLE " + strTableCode + " ADD CONSTRAINT " + strColDefaultName + "\n " +
                            "	DEFAULT ('" + strColDefault + "') for " + strColCode + " \n" +
                            "GO	\n";
                        }
                        #endregion
                    }
                    else
                    {
                        #region 数值型处理
                        if (_isSqlServerDefaultValueNameAuto)
                        {
                            //方式一：采用默认名
                            strDefault_Full = AddRightBand("default(" + strColDefault.Replace("'", "") + ")");
                        }
                        else
                        {
                            //方式二：采用指定名
                            strDefaultList = "\n" +
                             "IF EXISTS(SELECT * FROM SYS.OBJECTS WHERE TYPE='D' AND NAME='" + strColDefaultName + "') \n" +
                            "  ALTER TABLE " + strTableCode + " DROP CONSTRAINT " + strColDefaultName + " \n" +
                            "GO \n" +
                            "ALTER TABLE " + strTableCode + " ADD CONSTRAINT " + strColDefaultName + "\n " +
                            "	DEFAULT (" + strColDefault + ") FOR " + strColCode + " \n" +
                            "GO	\n";
                        }
                        #endregion
                    }
                }
                #endregion
                #region 唯一性的处理
                if (strColUnique == "是")
                {
                    string strColDefaultName = "UQ_" + strTable_Col;
                    strUqueList = "\n" +
                     "IF EXISTS(SELECT * FROM SYS.OBJECTS WHERE TYPE='UQ' AND NAME='" + strColDefaultName + "') \n" +
                    "ALTER TABLE " + strTableCode + " DROP CONSTRAINT " + strColDefaultName + " \n" +
                    "GO \n" +
                    "ALTER TABLE " + strTableCode + " ADD CONSTRAINT " + strColDefaultName + " UNIQUE \n " +
                    "GO	\n";
                }
                #endregion
                #region 外键的处理
                if (strKey == "FK" && !string.IsNullOrEmpty(strColForgKey) && strColForgKey.Contains("(") && strColForgKey.Contains(")"))
                {
                    string strColDefaultName = "FK_" + strTable_Col;
                    strUqueList = "\n" +
                     "IF EXISTS(SELECT * FROM SYS.OBJECTS WHERE TYPE='F' AND NAME='" + strColDefaultName + "') \n" +
                    "ALTER TABLE " + strTableCode + " DROP CONSTRAINT " + strColDefaultName + " \n" +
                    "GO \n" +
                    "ALTER TABLE " + strTableCode + " ADD CONSTRAINT " + strColDefaultName + " FOREIGN KEY (" + strColCode + ") REFERENCES " + strColForgKey + "\n" +
                    "GO	\n";
                }
                #endregion
                if (strColumnDealType == _strDealTypeAdd)
                {
                    //增加注解
                    string strColRemarkInfo = drCol["备注"].ToString();
                    if (!string.IsNullOrEmpty(strColRemarkInfo))
                    {
                        strColRemarkInfo = strColName + "：" + drCol["备注"].ToString();
                    }
                    sbSql.Append("/*" + iCalNum.ToString() + "." + j.ToString() + strTableName + AddLeftRightKuoHao(strTableCode) + "增加" + strColName + AddRightBand(AddLeftRightKuoHao(strColCode)) + strColDataType + "字段*/ \n" +
                    "IF NOT EXISTS(SELECT 1 FROM SYSOBJECTS A,SYSCOLUMNS B \n" +
                        " WHERE A.ID = B.ID AND A.NAME = '" + strTableCode + "' AND B.NAME = '" + strColCode + "' AND A.TYPE = 'U') \n" +
                        " BEGIN \n" +
                        "    ALTER TABLE " + strTableCode + " ADD " + strColCode + " " + sDataType_Full + " " + strDefault_Full + " " + strCanNull + "\n" +
                        "    EXEC sys.sp_addextendedproperty @name = N'MS_Description', @value = N'" + strColRemarkInfo + "',  \n" +
                        "    @level0type = N'SCHEMA',@level0name = N'dbo', @level1type = N'TABLE',@level1name = N'" + strTableCode + "',  \n" +
                        "    @level2type = N'COLUMN', @level2name = N'" + strColCode + "'  \n" +
                        " END\nGO\n");
                    if (!_isSqlServerDefaultValueNameAuto)
                    {
                        sbSql.Append(strDefaultList);
                    }
                }
                else if (strColumnDealType == _strDealTypeModify)
                {
                    sbSql.Append("/*" + iCalNum.ToString() + "." + j.ToString() + strTableName + AddLeftRightKuoHao(strTableCode) + "修改" + strColName + AddRightBand(AddLeftRightKuoHao(strColCode)) + strColDataType + "字段*/ \n" +
                        "IF EXISTS(SELECT 1 FROM SYSOBJECTS A,SYSCOLUMNS B \n" +
                            " WHERE A.ID = B.ID AND A.NAME = '" + strTableCode + "' AND B.NAME = '" + strColCode + "' AND A.TYPE = 'U') \n" +
                            " BEGIN \n" +
                            "    ALTER TABLE " + strTableCode + " ALTER COLUMN " + strColCode + " " + sDataType_Full + " " + strDefault_Full + " " + strCanNull + "\n" +
                            " END\nGO\n");
                }
                else if (strColumnDealType == _strDealTypeDelete)
                {
                    sbSql.Append("/*" + iCalNum.ToString() + "." + j.ToString() + strTableName + AddLeftRightKuoHao(strTableCode) + "修改" + strColName + AddRightBand(AddLeftRightKuoHao(strColCode)) + strColDataType + "字段*/ \n" +
                        "IF EXISTS(SELECT 1 FROM SYSOBJECTS A,SYSCOLUMNS B \n" +
                            " WHERE A.ID = B.ID AND A.NAME = '" + strTableCode + "' AND B.NAME = '" + strColCode + "' AND A.TYPE = 'U') \n" +
                            " BEGIN \n" +
                            "    ALTER TABLE " + strTableCode + " DROP COLUMN " + strColCode + "\n" +
                            " END\nGO\n");
                }
                else if (strColumnDealType == _strDealTypeDeleteAndAdd)
                {
                    //增加注解
                    string strColRemarkInfo = drCol["备注"].ToString();
                    if (!string.IsNullOrEmpty(strColRemarkInfo))
                    {
                        strColRemarkInfo = strColName + "：" + drCol["备注"].ToString();
                    }
                    sbSql.Append("/*" + iCalNum.ToString() + "." + j.ToString() + strTableName + AddLeftRightKuoHao(strTableCode) + "增加" + strColName + AddRightBand(AddLeftRightKuoHao(strColCode)) + strColDataType + "字段*/ \n" +
                        "IF EXISTS(SELECT 1 FROM SYSOBJECTS A,SYSCOLUMNS B \n" +
                            " WHERE A.ID = B.ID AND A.NAME = '" + strTableCode + "' AND B.NAME = '" + strColCode + "' AND A.TYPE = 'U') \n" +
                            " BEGIN \n" +
                            "    ALTER TABLE " + strTableCode + " DROP COLUMN " + strColCode + "\n" +
                            " END\n" +
                            "ALTER TABLE " + strTableCode + " ADD " + strColCode + " " + sDataType_Full + " " + strDefault_Full + " " + strCanNull + "\n" +
                            "    EXEC sys.sp_addextendedproperty @name = N'MS_Description', @value = N'" + strColRemarkInfo + "',  \n" +
                            "    @level0type = N'SCHEMA',@level0name = N'dbo', @level1type = N'TABLE',@level1name = N'" + strTableCode + "',  \n" +
                            "    @level2type = N'COLUMN', @level2name = N'" + strColCode + "'  \nGO\n");
                    if (!_isSqlServerDefaultValueNameAuto)
                    {
                        sbSql.Append(strDefaultList);
                    }
                }
                j++; 
                #endregion
            }
        }
        #endregion

        #region 生成Oracle表
        private void GenerateOracleTable()
        {
            foreach (DataRow dr in dtTable.Rows)
            {
                string strTableCode = dr["表编码"].ToString().Trim();
                string strTableName = dr["表名称"].ToString().Trim();
                string strTableDealType = dr["变更类型"].ToString().Trim();
                string strTableRemark = dr["备注"].ToString().Trim().Replace("'", "''");
                DataRow[] drList = dtAllCol.Select(" 表编码= '" + strTableCode + "'");
                string strPK = "";
                if ((strCreateType == _strCreate_Delete || strCreateType == _strCreate_DeleteAndAdd) && strTableDealType == "新增")
                {
                    //strDeleteSql = strDeleteSql.Insert(0, "DROP TABLE " + strTableCode + ";\n");//倒着删除掉
                    strDeleteSql = strDeleteSql.Insert(0, "declare  nCount  number;\n"
                        + "begin\n"
                        + "  nCount:=0;\n"
                        + "  select count(1) into nCount from user_objects\n "
                        + "  where upper(object_name) = '" + strTableCode + "' and upper(object_type) = 'TABLE';\n"
                        + "  if nCount = 1 then \n"
                        + "    begin \n"
                        + "      execute immediate 'drop TABLE " + strTableCode + "';\n"
                        + "    end;\n"
                        + "  end if;\n"
                        + "end;\n"
                        + "/\n");
                }

                string strSequence = "";//序列：一张表就一个，名字为:Se_表名

                if (strTableDealType == _strDealTypeAdd)
                {
                    #region 新增处理
                    sbSql.Append("/*" + iCalNum.ToString() + "、新增表：" + strTableName + AddLeftRightKuoHao(strTableCode) + "*/\n");
                    sbSql.Append(AddRightBand("CREATE TABLE") + AddRightBand(strTableCode) + "(\n");
                    //表说明SQL
                    sbRemark.Append("comment on table " + strTableCode + " is '" + strTableName + "：" + strTableRemark + "';\n");

                    int j = drList.Length;
                    //string strDefaultList = "";//默认值
                    string strUqueList = "";
                    
                    foreach (DataRow drCol in drList)
                    {
                        GenerateOracleColumn(TableDealType.Add, strTableCode,drCol,ref strPK,ref j,ref strSequence);
                    }
                    //表创建完毕
                    sbSql.Append(");\n");
                    if (strPK != "")
                    {
                        sbSql.Append(strPK);//主键的处理
                    }
                    sbSql.Append(strUqueList);//唯一和外键
                    sbSql.Append(sbRemark.ToString());//添加列说明
                    sbSql.Append(strSequence + "/\n");//添加序列
                    sbRemark = new StringBuilder();
                    #endregion
                }
                else
                {
                    #region 修改列处理
                    //alter table TEST1 drop column planmonth;
                    sbSql.Append("/*" + iCalNum.ToString() + "、修改表：" + strTableName + AddLeftRightKuoHao(strTableCode) + "*/\n");
                    int j = 1;
                    foreach (DataRow drCol in drList)
                    {
                        GenerateOracleColumn(TableDealType.Add, strTableCode, drCol, ref strPK, ref j, ref strSequence);
                    }
                    //sbSql.Append("/\n");
                    #endregion
                }
                iCalNum++;
            }
            //最后的处理
            sbSql.Insert(0, sbSqlFisrt.Append(strDeleteSql).ToString());
            if (strCreateType == _strCreate_Delete)
            {
                sbSql = new StringBuilder();
                sbSql.Append(strDeleteSql);
            }
        }

        private void GenerateOracleColumn(TableDealType tableDealType, string strTableCode, DataRow drCol, ref string strPK, ref int j, ref string strSequence)
        {
            //表编码,列名称,列编码,类型,长度,键,必填,约束,备注,自增长设置
            //列处理类型，当为空时表示新增
            string strColumnDealType = drCol["变更类型"].ToString().Trim() == "" ? _strDealTypeAdd : drCol["变更类型"].ToString().Trim();
            string strKey = drCol["键"].ToString().ToUpper().Trim();
            string strColCode = drCol["列编码"].ToString().Trim();
            string strColName = drCol["列名称"].ToString().Trim();
            string strColDataType = drCol["类型"].ToString().Trim().ToLower();
            string strColDefault = drCol["默认值"].ToString().Trim();
            string strColLen = drCol["长度"].ToString().Trim();
            string strColDecimalDigits = drCol["小数位"].ToString().Trim();
            string strColNoNull = drCol["必填"].ToString().Trim();
            string strColRemark = drCol["备注"].ToString().Trim().Replace("'", "''");

            //其他独有字段
            string strColPKName = _isAllConvert ? drCol["Oracle主键名"].ToString().Trim() : drCol["主键名"].ToString().Trim().ToUpper();
            string strColSeries = _isAllConvert ? drCol["Oracle序列名"].ToString().Trim() : drCol["序列名"].ToString().Trim().ToUpper();
            string strColUnique = _isAllConvert ? drCol["Oracle唯一约束名"].ToString().Trim() : drCol["唯一约束名"].ToString().Trim();
            string strColForgKey = _isAllConvert ? drCol["Oracle外键"].ToString().Trim() : drCol["外键"].ToString().Trim();
            string strColForgKeyCode = _isAllConvert ? drCol["Oracle外键名"].ToString().Trim() : drCol["外键名"].ToString().Trim();

            string strTable_Col = strTableCode + "_" + strColCode;//表编码+"_"+列编码
            string strCanNull = "";//是否可空
            string strDefault_Full = "";//默认值
            string strUqueList = "";//唯一性

            #region 转换字段类型与默认值
            if (_importDBType != _targetDBType)
            {
                ConvertDbTypeDefaultValueString(ref strColDataType, ref strColDefault, _importDBType, _targetDBType);
            }
            #endregion

            //数据类型(类型+长度+小数点)
            string sDataType_Full = GetFullTypeString(drCol, strColDataType, strColLen, strColDecimalDigits);

            if (tableDealType == TableDealType.Add)
            {
                #region 新增表处理

                #region 列类型和长度处理
                sbSql.Append(AddRightBand(strColCode) + sDataType_Full);
                if (strKey == "PK") //主键处理
                {
                    string strPK_Name = string.IsNullOrEmpty(strColPKName) ? "PK_" + strTableCode : strColPKName;
                    strPK = "alter table " + strTableCode + " add constraint " + strPK_Name + " primary key (" + strColCode + ");\n";
                }
                #endregion

                #region 序列的处理
                if (string.IsNullOrEmpty(strSequence) && !string.IsNullOrEmpty(strColSeries))
                {
                    if (strCreateType == _strCreate_DeleteAndAdd || strCreateType == _strCreate_Delete)
                    {
                        //删除序列SQL
                        strDeleteSql += "declare  nCount  number;\n"
                            + "begin\n"
                            + "  nCount:=0;\n"
                            + "  select count(1) into nCount from user_objects\n "
                            + "  where upper(object_name) = upper('" + strColSeries.ToUpper() + "') and upper(object_type) = 'SEQUENCE';\n"
                            + "  if nCount = 1 then \n"
                            + "    begin \n"
                            + "      execute immediate 'drop sequence " + strColSeries + "';\n"
                            + "    end;\n"
                            + "  end if;\n"
                            + "end;\n"
                            + "/\n";
                        if (strCreateType == _strCreate_Delete)
                        {
                            return;
                        }
                    }
                    strSequence += "create sequence " + strColSeries + " minvalue 1 maxvalue 9999999999999999999999999999 start with 1 increment by 1 cache 20;\n";
                }
                #endregion

                #region 默认值的处理
                if (!string.IsNullOrEmpty(strColDefault))
                {
                    //string strColDefaultName = "DF_" + strTableCode + "_" + strColCode;
                    sbSql.Append(AddRightBand(" DEFAULT " + strColDefault + ""));
                }
                #endregion

                #region 非空的处理
                if (strColNoNull == "是")
                {
                    sbSql.Append(_strNotNull);
                }
                else
                {
                    sbSql.Append(_strNull);
                }
                #endregion

                #region 增加逗号和列注释处理
                if (j > 1)
                {
                    sbSql.Append("," + "" + ChangeIntoInfo(strColName) + "\n");
                }
                else
                {
                    //最后一列不加逗号
                    sbSql.Append("" + ChangeIntoInfo(strColName) + "\n");
                }
                #endregion

                #region 列备注的处理
                if (string.IsNullOrEmpty(strColRemark))
                {
                    //备注为空时
                    if (!string.IsNullOrEmpty(strColSeries))
                    {
                        //序列不为空时
                        sbRemark.Append("comment on column " + strTableCode + "." + strColCode + " is '" + strColName + "。序列为" + strColSeries + "';\n");
                    }
                    else
                    {
                        //序列为空时
                        sbRemark.Append("comment on column " + strTableCode + "." + strColCode + " is '" + strColName + "';\n");
                    }
                }
                else
                {
                    //备注不为空时
                    if (!string.IsNullOrEmpty(strColSeries))
                    {
                        //序列不为空时
                        sbRemark.Append("comment on column " + strTableCode + "." + strColCode + " is '" + strColName + "：" + strColRemark + "。序列为" + strColSeries + "';\n");
                    }
                    else
                    {
                        //序列为空时
                        sbRemark.Append("comment on column " + strTableCode + "." + strColCode + " is '" + strColName + "：" + strColRemark + strColSeries + "';\n");
                    }
                }
                #endregion

                #region 唯一性的处理
                if (!string.IsNullOrEmpty(strColUnique))
                {
                    //string strColDefaultName = "UQ_" + strTable_Col;
                    strUqueList += "ALTER TABLE " + strTableCode + " ADD CONSTRAINT " + strColUnique + " UNIQUE (" + strColCode + ");\n ";
                }
                #endregion

                #region 外键的处理
                if (strKey == "FK" && !string.IsNullOrEmpty(strColForgKey) && strColForgKey.Contains("(") && strColForgKey.Contains(")"))
                {
                    //string strColDefaultName = "FK_" + strTable_Col;
                    strUqueList += "ALTER TABLE " + strTableCode + " ADD CONSTRAINT " + strColForgKeyCode + " FOREIGN KEY (" + strColCode + ") REFERENCES " + strColForgKey + ";\n";
                }
                #endregion

                //计数器
                j--; 
                #endregion
            }
            else
            {
                #region 修改表处理

                //生成删除列SQL脚本
                if (strCreateType == _strCreate_Delete || strCreateType == _strCreate_DeleteAndAdd)
                {
                    if (strColumnDealType == _strDealTypeAdd)
                    {
                        //strDeleteSql += "alter table " + strTableCode + " drop column " + strColCode + ";\n";
                        strDeleteSql += "declare iCount number;\n"
                        + "begin\n"
                        + "  iCount:=0;\n"
                        + "  select count(1) into iCount \n"
                        + "  from user_tab_columns where upper(table_name)= upper('" + strTableCode + "') and upper(column_name)=  upper('" + strColCode.ToUpper() + "'); \n"
                        + "  if iCount = 1 then \n"
                        + "    begin \n"
                        + "      execute immediate 'alter table " + strTableCode + " drop column " + strColCode + "';\n"
                        + "    end;\n"
                        + "  end if;\n"
                        + "end;\n"
                        + "/\n";
                    }
                    //对于删除，直接下一个字段
                    if (strCreateType == _strCreate_Delete)
                    {
                        return; //continue;
                    }
                }
                
                #region 非空的处理
                if (strColNoNull == "是")
                {
                    strCanNull = _strNotNull;
                }
                else
                {
                    strCanNull = _strNull;
                }
                #endregion
                #region 唯一性的处理
                if (!string.IsNullOrEmpty(strColUnique))
                {
                    //string strColDefaultName = "UQ_" + strTable_Col;
                    strUqueList = "\n" +
                    "ALTER TABLE " + strTableCode + " ADD CONSTRAINT " + strColUnique + " UNIQUE +(" + strColCode + ");\n ";
                    sbSql.Append(strUqueList);
                }
                #endregion
                #region 外键的处理
                if (strKey == "FK" && !string.IsNullOrEmpty(strColForgKey) && strColForgKey.Contains("(") && strColForgKey.Contains(")"))
                {
                    //string strColDefaultName = "FK_" + strTable_Col;
                    strUqueList = "\n" +
                    "ALTER TABLE " + strTableCode + " ADD CONSTRAINT " + strColForgKeyCode + " FOREIGN KEY (" + strColCode + ") REFERENCES " + strColForgKey + ";\n";
                    sbSql.Append(strUqueList);
                }
                #endregion
                #region 默认值的处理
                if (!string.IsNullOrEmpty(strColDefault))
                {
                    //string strColDefaultName = "DF_" + strTableCode + "_" + strColCode;
                    if (strColDataType == "varchar2" || strColDataType == "nvarchar2" || strColDataType == "char")
                    {
                        //字符型处理
                        strDefault_Full = AddRightBand("default '" + strColDefault + "'");
                    }
                    else
                    {
                        //数值型处理
                        strDefault_Full = AddRightBand("default " + strColDefault.Replace("'", ""));
                    }
                }
                #endregion

                if (strColumnDealType == _strDealTypeAdd)
                {
                    //得到修改表增加列语句
                    sbSql.Append("alter table " + strTableCode + " add " + AddRightBand(strColCode) + sDataType_Full + strDefault_Full + strCanNull + ";\n");
                    //增加注解
                    if (string.IsNullOrEmpty(strColRemark))
                    {
                        sbSql.Append("comment on column " + strTableCode + "." + strColCode + " is '" + strColName + "';\n");
                    }
                    else
                    {
                        sbSql.Append("comment on column " + strTableCode + "." + strColCode + " is '" + strColName + "：" + strColRemark + "';\n");
                    }
                }
                else if (strColumnDealType == _strDealTypeModify)
                {
                    sbSql.Append("/*注：对修改字段，如要变更是否可空类型，则自己在最后加上NULL 或NOT NULL。对字段类型的变更，需要先清空该字段值或删除该列再新增*/\n");
                    sbSql.Append("alter table " + strTableCode + " modify " + AddRightBand(strColCode) + sDataType_Full + strDefault_Full + ";\n");
                }
                else if (strColumnDealType == _strDealTypeDelete)
                {
                    sbSql.Append("alter table " + strTableCode + " drop column " + AddRightBand(strColCode) + ";\n");
                }
                else if (strColumnDealType == _strDealTypeDeleteAndAdd)
                {
                    sbSql.Append("alter table " + strTableCode + " drop column " + AddRightBand(strColCode) + ";\n");
                    //得到修改表增加列语句
                    sbSql.Append("alter table " + strTableCode + " add " + AddRightBand(strColCode) + sDataType_Full + strDefault_Full + strCanNull + ";\n");
                    //增加注解
                    if (string.IsNullOrEmpty(strColRemark))
                    {
                        sbSql.Append("comment on column " + strTableCode + "." + strColCode + " is '" + strColName + "';\n");
                    }
                    else
                    {
                        sbSql.Append("comment on column " + strTableCode + "." + strColCode + " is '" + strColName + "：" + strColRemark + "';\n");
                    }
                }
                j++;
                #endregion
            }
        }
        #endregion

        #region 生成MySql表
        private void GenerateMySqlTable()
        {
            foreach (DataRow dr in dtTable.Rows)
            {
                string strTableCode = dr["表编码"].ToString().Trim();
                string strTableName = dr["表名称"].ToString().Trim();
                string strTableDealType = dr["变更类型"].ToString().Trim();
                string strTableRemark = dr["备注"].ToString().Trim().Replace("'", "''");
                DataRow[] drList = dtAllCol.Select(" 表编码= '" + strTableCode + "'");
                TableDealType tableDealType = strTableDealType == "新增" ? TableDealType.Add : TableDealType.Modify;
                string strPK = "";
                if ((strCreateType == _strCreate_Delete || strCreateType == _strCreate_DeleteAndAdd) && tableDealType == TableDealType.Add)
                {
                    strDeleteSql = strDeleteSql.Insert(0, "DROP TABLE IF EXISTS " + strTableCode + ";\n");//倒着删除掉
                }
                if (tableDealType == TableDealType.Add)
                {
                    #region 新增处理
                    sbSql.Append("/*" + iCalNum.ToString() + "、新增表：" + strTableName + AddLeftRightKuoHao(strTableCode) + "*/\n");
                    sbSql.Append(AddRightBand("CREATE TABLE IF NOT EXISTS") + AddRightBand("`" + strTableCode+ "`") + "(\n");
                    //表说明SQL
                    sbRemark.Append(string.Format("ALTER TABLE `{0}` COMMENT '{1}';\n", strTableCode, strTableName + "：" + strTableRemark));

                    int j = drList.Length;
                    //string strDefaultList = "";//默认值
                    //string strUqueList = "";
                    //string strSequence = "";//序列：一张表就一个，名字为:Se_表名
                    foreach (DataRow drCol in drList)
                    {
                        //增加MySql列
                        GenerateMySqlColumn(TableDealType.Add, strTableCode, drCol, ref strPK, ref j);
                    }
                    if (strPK != "")
                    {
                        sbSql.Append(strPK);//主键的处理,MySql是一句独立的
                    }
                    //表创建完毕
                    sbSql.Append(");\n");
                    
                    //sbSql.Append(strUqueList);//唯一和外键
                    sbSql.Append(sbRemark.ToString());//添加列说明
                    sbRemark = new StringBuilder();
                    #endregion
                }
                else
                {
                    #region 修改表处理
                    //alter table TEST1 drop column planmonth;
                    sbSql.Append("/*" + iCalNum.ToString() + "、修改表：" + strTableName + AddLeftRightKuoHao(strTableCode) + "*/\n");
                    int j = 1;
                    foreach (DataRow drCol in drList)
                    {
                        //增加MySql列
                        GenerateMySqlColumn(TableDealType.Add, strTableCode, drCol, ref strPK, ref j);
                    }
                    #endregion
                }
                iCalNum++;
            }
            //最后的处理
            sbSql.Insert(0, sbSqlFisrt.Append(strDeleteSql).ToString());
            if (strCreateType == _strCreate_Delete)
            {
                sbSql = new StringBuilder();
                sbSql.Append(strDeleteSql);
            }
        }

        private void GenerateMySqlColumn(TableDealType tableDealType, string strTableCode, DataRow drCol, ref string strPK, ref int j)
        {
            //表编码,列名称,列编码,类型,长度,键,必填,约束,备注,自增长设置
            string strColumnDealType = drCol["变更类型"].ToString().Trim() == "" ? _strDealTypeAdd : drCol["变更类型"].ToString().Trim();//列处理类型，当为空时表示新增
            string strKey = drCol["键"].ToString().ToUpper().Trim();
            string strColCode = drCol["列编码"].ToString().Trim();
            string strColName = drCol["列名称"].ToString().Trim();
            string strColDataType = drCol["类型"].ToString().Trim().ToLower();
            string strColDefault = drCol["默认值"].ToString().Trim();
            string strColLen = drCol["长度"].ToString().Trim();
            string strColDecimalDigits = drCol["小数位"].ToString().Trim();
            string strColNoNull = drCol["必填"].ToString().Trim();
            string strColRemark = drCol["备注"].ToString().Trim().Replace("'", "''");
            //其他独有字段
            string strColUnsign = _isAllConvert ? drCol["MySql标志位"].ToString().Trim() : drCol["标志位"].ToString().Trim();
            string strColAutoAdd = _isAllConvert ? drCol["MySql自增长"].ToString().Trim() : drCol["自增长"].ToString().Trim();
            string strColForgKey = _isAllConvert ? drCol["MySql外键"].ToString().Trim() : drCol["外键"].ToString().Trim();

            string strTable_Col = strTableCode + "_" + strColCode;//表编码+"_"+列编码
            

            #region 转换字段类型与默认值
            if (_importDBType != _targetDBType)
            {
                ConvertDbTypeDefaultValueString(ref strColDataType, ref strColDefault, _importDBType, _targetDBType);
            }
            #endregion

            //数据类型(类型+长度+小数点)
            string sDataType_Full = GetFullTypeString(drCol, strColDataType, strColLen, strColDecimalDigits);

            if (tableDealType == TableDealType.Add)
            {
                #region 新增表处理

                #region 列类型和长度处理
                sbSql.Append(AddRightBand(strColCode) + sDataType_Full);
                if (strKey == "PK")
                {
                    //主键处理
                    strPK = " primary key (" + strColCode + ") \n";
                }
                #endregion

                #region 标志位Unsign
                if (strColUnsign.Equals("是"))
                {
                    sbSql.Append(" UNSIGNED ");
                }
                #endregion

                #region 非空的处理
                if (strColNoNull == "是")
                {
                    sbSql.Append(_strNotNull);
                }
                //else
                //{
                //    sbSql.Append(_strNull);
                //}
                #endregion

                #region 默认值的处理
                if (!string.IsNullOrEmpty(strColDefault))
                {
                    //string strColDefaultName = "DF_" + strTableCode + "_" + strColCode;
                    sbSql.Append(AddRightBand(" DEFAULT " + strColDefault + ""));
                }
                #endregion

                #region 自增长
                if (strColAutoAdd.Equals("是"))
                {
                    sbSql.Append(" AUTO_INCREMENT ");
                }
                #endregion

                #region 列备注的处理
                //备注为空时
                sbSql.Append(" COMMENT " + "'" + strColName + ":" + strColRemark + "'");
                #endregion

                #region 增加逗号
                sbSql.Append("," + "" + ChangeIntoInfo(strColName) + "\n");
                #endregion

                #region 外键的处理
                if (strKey == "FK" && !string.IsNullOrEmpty(strColForgKey) && strColForgKey.Contains("(") && strColForgKey.Contains(")"))
                {
                    //string strColDefaultName = "FK_" + strTable_Col;
                    //strUqueList += "ALTER TABLE " + strTableCode + " ADD CONSTRAINT " + strColForgKeyCode + " FOREIGN KEY (" + strColCode + ") REFERENCES " + strColForgKey + ";\n";
                }
                #endregion

                //计数器
                j--;
                #endregion
            }
            else
            {
                #region 修改表处理
                string strDefault_Full = "";//默认值

                StringBuilder sbColSql = new StringBuilder();//列的SQL

                //生成删除列SQL脚本
                if (strCreateType == _strCreate_Delete || strCreateType == _strCreate_DeleteAndAdd)
                {
                    if (strColumnDealType == _strDealTypeAdd)
                    {
                        strDeleteSql += "alter table `" + strTableCode + "` drop column `" + strColCode + "`;\n";
                    }
                    //对于删除，直接下一个字段
                    if (strCreateType == _strCreate_Delete)
                    {
                        return;//continue;
                    }
                }
                //列SQL
                sbColSql.Append(sDataType_Full);

                #region 标志位Unsign
                if (strColUnsign.Equals("是"))
                {
                    sbSql.Append(" UNSIGNED ");
                }
                #endregion

                #region 默认值的处理
                if (!string.IsNullOrEmpty(strColDefault))
                {
                    //string strColDefaultName = "DF_" + strTableCode + "_" + strColCode;
                    if (strColDataType == "varchar" || strColDataType == "nvarchar" || strColDataType == "char")
                    {
                        //字符型处理
                        strDefault_Full = AddRightBand("default '" + strColDefault + "'");
                    }
                    else
                    {
                        //数值型处理
                        strDefault_Full = AddRightBand("default " + strColDefault.Replace("'", ""));
                    }
                    sbColSql.Append(strDefault_Full);
                }
                #endregion

                #region 非空的处理
                if (strColNoNull == "是")
                {
                    sbColSql.Append(_strNotNull);
                }
                else
                {
                    sbColSql.Append(_strNull);
                }
                #endregion

                #region 自增长
                if (strColAutoAdd.Equals("是"))
                {
                    sbColSql.Append(" AUTO_INCREMENT ");
                }
                //备注
                sbColSql.Append(" COMMENT " + "'" + strColName + ":" + strColRemark + "';");
                #endregion

                if (strColumnDealType == _strDealTypeAdd)
                {
                    //得到修改表增加列语句
                    sbSql.Append("alter table `" + strTableCode + "` add " + AddRightBand(strColCode) + sbColSql.ToString());
                }
                else if (strColumnDealType == _strDealTypeModify)
                {
                    sbSql.Append("/*注：对修改字段，如要变更是否可空类型，则自己在最后加上NULL 或NOT NULL。对字段类型的变更，需要先清空该字段值或删除该列再新增*/\n");
                    sbSql.Append("alter table `" + strTableCode + "` CHANGE " + AddRightBand(strColCode) + sbColSql.ToString());
                }
                else if (strColumnDealType == _strDealTypeDelete)
                {
                    sbSql.Append("alter table " + strTableCode + " drop column " + AddRightBand(strColCode) + ";\n");
                }
                else if (strColumnDealType == _strDealTypeDeleteAndAdd)
                {
                    //得到修改表增加列语句
                    sbSql.Append("alter table " + strTableCode + " CHANGE " + AddRightBand(strColCode) + sbColSql.ToString());
                }
                j++; 
                #endregion
            }
        }
        #endregion

        #region 生成SQLite表
        private void GenerateSQLiteTable()
        {
            //
            _tableColumnAroundChar_Left = "\"";
            _tableColumnAroundChar_Right = "\"";
            //
            foreach (DataRow dr in dtTable.Rows)
            {
                string strTableCode = dr["表编码"].ToString().Trim();
                string strTableName = dr["表名称"].ToString().Trim();
                string strTableDealType = dr["变更类型"].ToString().Trim();
                string strTableRemark = dr["备注"].ToString().Trim().Replace("'", "''");
                DataRow[] drList = dtAllCol.Select(" 表编码= '" + strTableCode + "'");
                TableDealType tableDealType = strTableDealType == "新增" ? TableDealType.Add : TableDealType.Modify;
                string strPK = "";
                string strUqueList = "";
                if ((strCreateType == _strCreate_Delete || strCreateType == _strCreate_DeleteAndAdd) && tableDealType == TableDealType.Add)
                {
                    strDeleteSql = strDeleteSql.Insert(0, "DROP TABLE IF EXISTS " + strTableCode + ";\n");//倒着删除掉
                }
                if (tableDealType == TableDealType.Add)
                {
                    #region 新增处理
                    sbSql.Append("/*" + iCalNum.ToString() + "、新增表：" + strTableName + AddLeftRightKuoHao(strTableCode) + "*/\n");
                    sbSql.Append(AddRightBand("CREATE TABLE IF NOT EXISTS") + AddRightBand(_tableColumnAroundChar_Left + strTableCode + _tableColumnAroundChar_Right) + "(\n");
                    //无表说明SQL
                    int j = drList.Length;
                    //string strDefaultList = "";//默认值
                    //string strUqueList = "";
                    //string strSequence = "";//序列：一张表就一个，名字为:Se_表名
                    foreach (DataRow drCol in drList)
                    {
                        //增加MySql列
                        GenerateSQLiteColumn(TableDealType.Add, strTableCode, drCol, ref strPK, ref j, ref strUqueList);
                    }
                    //唯一和外键
                    if (!string.IsNullOrEmpty(strUqueList))
                    {
                        //去掉最后的逗号
                        sbSql.Append(strUqueList.Substring(0, strUqueList.Length - 1));
                    }
                    
                    //表创建完毕
                    sbSql.Append(");\n");
                    
                    sbRemark = new StringBuilder();
                    #endregion
                }
                else
                {
                    #region 修改表处理
                    //alter table TEST1 drop column planmonth;
                    sbSql.Append("/*" + iCalNum.ToString() + "、修改表：" + strTableName + AddLeftRightKuoHao(strTableCode) + "*/\n");
                    int j = 1;
                    foreach (DataRow drCol in drList)
                    {
                        //增加MySql列
                        GenerateSQLiteColumn(TableDealType.Add, strTableCode, drCol, ref strPK, ref j,ref strUqueList);
                    }
                    #endregion
                }
                iCalNum++;
            }
            //最后的处理
            sbSql.Insert(0, sbSqlFisrt.Append(strDeleteSql).ToString());
            if (strCreateType == _strCreate_Delete)
            {
                sbSql = new StringBuilder();
                sbSql.Append(strDeleteSql);
            }
        }

        private void GenerateSQLiteColumn(TableDealType tableDealType, string strTableCode, DataRow drCol, ref string strPK, ref int j,ref string strUqueList)
        {
            //表编码,列名称,列编码,类型,长度,键,必填,约束,备注,自增长设置
            string strColumnDealType = drCol["变更类型"].ToString().Trim() == "" ? _strDealTypeAdd : drCol["变更类型"].ToString().Trim();//列处理类型，当为空时表示新增
            string strKey = drCol["键"].ToString().ToUpper().Trim();
            string strColCode = drCol["列编码"].ToString().Trim();
            string strColName = drCol["列名称"].ToString().Trim();
            string strColDataType = drCol["类型"].ToString().Trim().ToLower();
            string strColDefault = drCol["默认值"].ToString().Trim();
            string strColLen = drCol["长度"].ToString().Trim();
            string strColDecimalDigits = drCol["小数位"].ToString().Trim();
            string strColNoNull = drCol["必填"].ToString().Trim();
            string strColRemark = drCol["备注"].ToString().Trim().Replace("'", "''");
            //其他独有字段
            string strColAutoAdd = _isAllConvert ? drCol["SQLite自增长"].ToString().Trim() : drCol["自增长"].ToString().Trim();
            string strColPKName = _isAllConvert ? drCol["SQLite主键名"].ToString().Trim() : drCol["主键名"].ToString().Trim().ToUpper();
            string strColUnique = _isAllConvert ? drCol["SQLite唯一约束名"].ToString().Trim() : drCol["唯一约束名"].ToString().Trim();
            string strColForgKey = _isAllConvert ? drCol["SQLite外键"].ToString().Trim() : drCol["外键"].ToString().Trim();
            string strColForgKeyCode = _isAllConvert ? drCol["SQLite外键名"].ToString().Trim() : drCol["外键名"].ToString().Trim();

            string strTable_Col = strTableCode + "_" + strColCode;//表编码+"_"+列编码
            StringBuilder sbColSql = new StringBuilder();//列的SQL

            #region 转换字段类型与默认值
            if (_importDBType != _targetDBType)
            {
                ConvertDbTypeDefaultValueString(ref strColDataType, ref strColDefault, _importDBType, _targetDBType);
            }
            #endregion

            //数据类型(类型+长度+小数点)
            string sDataType_Full = GetFullTypeString(drCol, strColDataType, strColLen, strColDecimalDigits);

            if (tableDealType == TableDealType.Add)
            {
                #region 新增表处理

                #region 列类型和长度处理
                sbColSql.Append(AddRightBand(strColCode) + sDataType_Full);
                #endregion

                #region 主键
                if (strKey.Equals("PK"))
                {
                    sbColSql.Append(" PRIMARY KEY ");
                }
                #endregion

                #region 自增长
                if (strColAutoAdd.Equals("是"))
                {
                    sbColSql.Append(" AUTOINCREMENT ");
                }
                #endregion

                #region 非空的处理
                if (strColNoNull == "是")
                {
                    sbColSql.Append(_strNotNull);
                }
                //else
                //{
                //    sbSql.Append(_strNull);
                //}
                #endregion

                #region 默认值的处理
                if (!string.IsNullOrEmpty(strColDefault))
                {
                    sbColSql.Append(AddRightBand(" DEFAULT " + strColDefault + ""));
                }
                #endregion
                
                #region 列备注的处理
                //备注为空时
                //sbSql.Append(" COMMENT " + "'" + strColName + ":" + strColRemark + "'");
                #endregion

                #region 唯一性的处理
                if (!string.IsNullOrEmpty(strColUnique))
                {
                    //string strColDefaultName = "UQ_" + strTable_Col;
                    strUqueList += "CONSTRAINT " + strColUnique + " UNIQUE (" + strColCode + "),\n ";
                }
                #endregion

                #region 外键的处理
                if (strKey == "FK" && !string.IsNullOrEmpty(strColForgKey) && strColForgKey.Contains("(") && strColForgKey.Contains(")"))
                {
                    string strColDefaultName = "FK_" + strTable_Col;
                    strUqueList += "CONSTRAINT" + strColForgKeyCode + " FOREIGN KEY (" + strColCode + ") REFERENCES " + strColForgKey + ",\n";
                }
                #endregion

                #region 增加逗号
                if(j==1 && string.IsNullOrEmpty(strUqueList)) //最后一个字段，且没有唯一性和外键约束
                {
                    sbColSql.Append("" + ChangeIntoInfo(strColName) + "\n");
                }
                else
                {
                    sbColSql.Append("," + "" + ChangeIntoInfo(strColName) + "\n");
                }
                #endregion

                //
                sbSql.Append(sbColSql.ToString());
                //计数器
                j--;
                #endregion
            }
            else
            {
                //生成删除列SQL脚本
                if (strCreateType == _strCreate_Delete || strCreateType == _strCreate_DeleteAndAdd)
                {
                    if (strColumnDealType == _strDealTypeAdd)
                    {
                        strDeleteSql += "alter table `" + strTableCode + "` drop column `" + strColCode + "`;\n";
                    }
                    //对于删除，直接下一个字段
                    if (strCreateType == _strCreate_Delete)
                    {
                        return;//continue;
                    }
                }

                #region 修改表处理
                //类型长度小数点
                sbColSql.Append(AddRightBand(sDataType_Full));

                #region 主键
                if (strKey.Equals("PK"))
                {
                    sbColSql.Append(" PRIMARY KEY ");
                }
                #endregion

                #region 自增长
                if (strColAutoAdd.Equals("是"))
                {
                    sbColSql.Append(" AUTOINCREMENT ");
                }
                #endregion

                #region 非空的处理
                if (strColNoNull == "是")
                {
                    sbColSql.Append(_strNotNull);
                }
                //else
                //{
                //    sbColSql.Append(_strNull);
                //}
                #endregion

                #region 默认值的处理
                if (!string.IsNullOrEmpty(strColDefault))
                {
                    sbColSql.Append(AddRightBand(" DEFAULT " + strColDefault + ""));
                }
                #endregion

                //备注
                //sbColSql.Append(" COMMENT " + "'" + strColName + ":" + strColRemark + "';");

                if (strColumnDealType == _strDealTypeAdd)
                {
                    //得到修改表增加列语句
                    sbSql.Append("alter table `" + strTableCode + "` add " + AddRightBand(strColCode) + sbColSql.ToString());
                }
                else if (strColumnDealType == _strDealTypeModify)
                {
                    sbSql.Append("/*注：对修改字段，如要变更是否可空类型，则自己在最后加上NULL 或NOT NULL。对字段类型的变更，需要先清空该字段值或删除该列再新增*/\n");
                    sbSql.Append("alter table `" + strTableCode + "` CHANGE " + AddRightBand(strColCode) + sbColSql.ToString());
                }
                else if (strColumnDealType == _strDealTypeDelete)
                {
                    sbSql.Append("alter table " + strTableCode + " drop column " + AddRightBand(strColCode) + ";\n");
                }
                else if (strColumnDealType == _strDealTypeDeleteAndAdd)
                {
                    //得到修改表增加列语句
                    sbSql.Append("alter table " + strTableCode + " alter " + AddRightBand(strColCode) + sbColSql.ToString());
                }
                j++;
                #endregion
            }
        }
        #endregion

        #region 生成PostgreSql表
        /// <summary>
        /// PostgreSql中的表和列名都是小写的
        /// </summary>
        private void GeneratePostgreSqlTable()
        {
            
            foreach (DataRow dr in dtTable.Rows)
            {
                string strTableCode = dr["表编码"].ToString().Trim();
                string strTableName = dr["表名称"].ToString().Trim();
                string strTableDealType = dr["变更类型"].ToString().Trim();
                string strTableRemark = dr["备注"].ToString().Trim().Replace("'", "''");
                DataRow[] drList = dtAllCol.Select(" 表编码= '" + strTableCode + "'");
                TableDealType tableDealType = strTableDealType == "新增" ? TableDealType.Add : TableDealType.Modify;
                string strPK = "";
                string strUqueList = "";

                if ((strCreateType == _strCreate_Delete || strCreateType == _strCreate_DeleteAndAdd) && tableDealType == TableDealType.Add)
                {
                    strDeleteSql = strDeleteSql.Insert(0, "DROP TABLE IF EXISTS " + strTableCode + ";\n");//倒着删除掉
                }
                if (tableDealType == TableDealType.Add)
                {
                    #region 新增处理
                    sbSql.Append("/*" + iCalNum.ToString() + "、新增表：" + strTableName + AddLeftRightKuoHao(strTableCode) + "*/\n");
                    sbSql.Append(AddRightBand("CREATE TABLE IF NOT EXISTS") + AddRightBand(strTableCode) + "(\n");
                    
                    //表说明SQL
                    sbRemark.Append("COMMENT ON TABLE " + strTableCode + " IS '" + strTableName + "：" + strTableRemark + "';\n");

                    int j = drList.Length;
                    //string strDefaultList = "";//默认值
                    //string strUqueList = "";
                    //string strSequence = "";//序列：一张表就一个，名字为:Se_表名
                    foreach (DataRow drCol in drList)
                    {
                        //增加MySql列
                        GeneratePostgreSqlColumn(TableDealType.Add, strTableCode, drCol, ref strPK, ref j, ref strUqueList);
                    }
                    if (strPK != "")
                    {
                        sbSql.Append(strPK);//主键的处理,MySql是一句独立的
                    }
                    //唯一和外键
                    if (!string.IsNullOrEmpty(strUqueList))
                    {
                        //去掉最后一个逗号和换行
                        sbSql.Append(strUqueList.Substring(0, strUqueList.Length - 2) + "\n");
                    }
                    
                    //表创建完毕
                    sbSql.Append(");\n");

                    sbSql.Append(sbRemark.ToString());//添加列说明
                    sbRemark = new StringBuilder();
                    #endregion
                }
                else
                {
                    #region 修改表处理
                    //alter table TEST1 drop column planmonth;
                    sbSql.Append("/*" + iCalNum.ToString() + "、修改表：" + strTableName + AddLeftRightKuoHao(strTableCode) + "*/\n");
                    int j = 1;
                    foreach (DataRow drCol in drList)
                    {
                        //增加MySql列
                        GeneratePostgreSqlColumn(TableDealType.Add, strTableCode, drCol, ref strPK, ref j, ref strUqueList);
                    }
                    #endregion
                }
                iCalNum++;
            }
            //最后的处理
            sbSql.Insert(0, sbSqlFisrt.Append(strDeleteSql).ToString());
            if (strCreateType == _strCreate_Delete)
            {
                sbSql = new StringBuilder();
                sbSql.Append(strDeleteSql);
            }
        }

        private void GeneratePostgreSqlColumn(TableDealType tableDealType, string strTableCode, DataRow drCol, ref string strPK, ref int j, ref string strUqueList)
        {
            //表编码,列名称,列编码,类型,长度,键,必填,约束,备注,自增长设置
            string strColumnDealType = drCol["变更类型"].ToString().Trim() == "" ? _strDealTypeAdd : drCol["变更类型"].ToString().Trim();//列处理类型，当为空时表示新增
            string strKey = drCol["键"].ToString().ToUpper().Trim();
            string strColCode = drCol["列编码"].ToString().Trim();
            string strColName = drCol["列名称"].ToString().Trim();
            string strColDataType = drCol["类型"].ToString().Trim().ToLower();
            string strColDefault = drCol["默认值"].ToString().Trim();
            string strColLen = drCol["长度"].ToString().Trim();
            string strColDecimalDigits = drCol["小数位"].ToString().Trim();
            string strColNoNull = drCol["必填"].ToString().Trim();
            string strColRemark = drCol["备注"].ToString().Trim().Replace("'", "''");
            //其他独有字段
            string strColPKName = _isAllConvert ? drCol["PostgreSql主键名"].ToString().Trim() : drCol["主键名"].ToString().Trim().ToUpper();
            string strColUnique = _isAllConvert ? drCol["PostgreSql唯一约束名"].ToString().Trim() : drCol["唯一约束名"].ToString().Trim();
            string strColForgKey = _isAllConvert ? drCol["PostgreSql外键"].ToString().Trim() : drCol["外键"].ToString().Trim();
            string strColForgKeyCode = _isAllConvert ? drCol["PostgreSql外键名"].ToString().Trim() : drCol["外键名"].ToString().Trim();

            string strTable_Col = strTableCode + "_" + strColCode;//表编码+"_"+列编码
            
            #region 转换字段类型与默认值
            if (_importDBType != _targetDBType)
            {
                ConvertDbTypeDefaultValueString(ref strColDataType, ref strColDefault, _importDBType, _targetDBType);
            }
            #endregion

            //数据类型(类型+长度+小数点)
            string sDataType_Full = GetFullTypeString(drCol, strColDataType, strColLen, strColDecimalDigits);

            if (tableDealType == TableDealType.Add)
            {
                #region 新增表处理

                #region 列类型和长度处理
                sbSql.Append(AddRightBand(strColCode) + sDataType_Full);
                #endregion

                #region 非空的处理
                if (strColNoNull == "是")
                {
                    sbSql.Append(_strNotNull);
                }
                #endregion

                #region 默认值的处理
                if (!string.IsNullOrEmpty(strColDefault))
                {
                    //string strColDefaultName = "DF_" + strTableCode + "_" + strColCode;
                    sbSql.Append(AddRightBand(" DEFAULT " + strColDefault + ""));
                }
                #endregion

                #region 主键
                if (strKey == "PK")
                {
                    if (string.IsNullOrEmpty(strColPKName))
                    {
                        strUqueList += " PRIMARY KEY (" + strColCode + "),\n";
                    }
                    else
                    {
                        strUqueList += " CONSTRAINT " + strColPKName + " PRIMARY KEY (" + strColCode + "),\n";
                    }
                }
                #endregion

                #region 唯一性约束
                if (!string.IsNullOrEmpty(strColUnique))
                {
                    strUqueList += " CONSTRAINT \"" + strColUnique + "\" UNIQUE (" + strColCode + "),\n";
                }
                #endregion

                #region 外键的处理
                if (!string.IsNullOrEmpty(strColForgKeyCode))
                {
                    //这里表名
                    strUqueList += string.Format(" CONSTRAINT \"{0}\" FOREIGN KEY ({1}) REFERENCES {2},\n", strColForgKeyCode, strColCode, strColForgKey);
                }
                #endregion

                #region 列备注的处理
                //备注
                sbRemark.Append(" COMMENT ON COLUMN " + strTableCode + "." + strColCode + " IS '" + strColName + ":" + strColRemark + "';\n");
                #endregion

                #region 增加逗号
                if (j == 1 && string.IsNullOrEmpty(strUqueList)) //最后一个字段，且没有唯一性和外键约束
                {
                    sbSql.Append("" + ChangeIntoInfo(strColName) + "\n");
                }
                else
                {
                    sbSql.Append("," + "" + ChangeIntoInfo(strColName) + "\n");
                }
                #endregion
                //计数器
                j--;
                #endregion
            }
            else
            {
                #region 修改表处理
                StringBuilder sbColSql = new StringBuilder();//列的SQL

                if (strColDataType.Contains("("))
                {
                    if (string.IsNullOrEmpty(strColLen))
                    {
                        strColLen = strColDataType.Substring(strColDataType.IndexOf("(") + 1, strColDataType.IndexOf(")") - strColDataType.IndexOf("(") - 1);
                        drCol["长度"] = strColLen;
                        drCol["类型"] = strColDataType.Substring(0, strColDataType.IndexOf("("));
                    }
                    else
                    {
                        strColDataType = strColDataType.Substring(0, strColDataType.IndexOf("("));
                        drCol["类型"] = strColDataType;
                    }
                }

                //生成删除列SQL脚本
                if (strCreateType == _strCreate_Delete || strCreateType == _strCreate_DeleteAndAdd)
                {
                    if (strColumnDealType == _strDealTypeAdd)
                    {
                        strDeleteSql += "alter table `" + strTableCode + "` drop column `" + strColCode + "`;\n";
                    }
                    //对于删除，直接下一个字段
                    if (strCreateType == _strCreate_Delete)
                    {
                        return;//continue;
                    }
                }

                //类型处理
                sbSql.Append(AddRightBand(strColCode) + sDataType_Full);

                #region 非空的处理
                if (strColNoNull == "是")
                {
                    sbSql.Append(_strNotNull);
                }
                //else
                //{
                //    sbSql.Append(_strNull);
                //}
                #endregion

                #region 默认值的处理
                if (!string.IsNullOrEmpty(strColDefault))
                {
                    //string strColDefaultName = "DF_" + strTableCode + "_" + strColCode;
                    sbSql.Append(AddRightBand(" DEFAULT " + strColDefault + ""));
                }
                #endregion

                #region 列备注的处理
                //备注
                sbRemark.Append(" COMMENT ON COLUMN " + strTableCode + "." + strColCode + " IS '" + strColName + ":" + strColRemark + "'");
                #endregion

                if (strColumnDealType == _strDealTypeAdd)
                {
                    //得到修改表增加列语句
                    sbSql.Append("alter table `" + strTableCode + "` add " + AddRightBand(strColCode) + sbColSql.ToString());
                }
                else if (strColumnDealType == _strDealTypeModify)
                {
                    sbSql.Append("/*注：对修改字段，如要变更是否可空类型，则自己在最后加上NULL 或NOT NULL。对字段类型的变更，需要先清空该字段值或删除该列再新增*/\n");
                    sbSql.Append("alter table `" + strTableCode + "` CHANGE " + AddRightBand(strColCode) + sbColSql.ToString());
                }
                else if (strColumnDealType == _strDealTypeDelete)
                {
                    sbSql.Append("alter table " + strTableCode + " drop column " + AddRightBand(strColCode) + ";\n");
                }
                else if (strColumnDealType == _strDealTypeDeleteAndAdd)
                {
                    //得到修改表增加列语句
                    sbSql.Append("alter table " + strTableCode + " CHANGE " + AddRightBand(strColCode) + sbColSql.ToString());
                }
                j++;
                #endregion
            }
        }
        #endregion

        #region 获取全类型字符
        private string GetFullTypeString(DataRow drCol, string strColDataType, string strColLen, string strColDecimalDigits)
        {
            string sDataType_Full;
            #region 类型的处理
            if (strColDataType.Contains("("))
            {
                #region 类型已经包括了长度或长度及小数点，需要修正类型、长度和小数位
                sDataType_Full = strColDataType;
                drCol["类型"] = strColDataType.Substring(0, strColDataType.IndexOf("("));
                if (strColDataType.Contains(","))//有小数位
                {
                    drCol["长度"] = strColDataType.Substring(strColDataType.IndexOf("(") + 1, strColDataType.IndexOf(",") - strColDataType.IndexOf("(") - 1);
                    drCol["小数位"] = strColDataType.Substring(strColDataType.IndexOf(",") + 1, strColDataType.IndexOf(")") - strColDataType.IndexOf(",") - 1);
                }
                else
                {
                    drCol["长度"] = strColDataType.Substring(strColDataType.IndexOf("(") + 1, strColDataType.IndexOf(")") - strColDataType.IndexOf("(") - 1);
                }
                #endregion
            }
            else
            {
                #region 类型不包括了长度或长度及小数点
                if (!string.IsNullOrEmpty(strColLen))
                {
                    if (!string.IsNullOrEmpty(strColDecimalDigits))
                    {
                        sDataType_Full = strColDataType + AddLeftRightKuoHao(strColLen + "," + strColDecimalDigits);
                    }
                    else
                    {
                        sDataType_Full = strColDataType + AddLeftRightKuoHao(strColLen);
                    }
                }
                else
                {
                    sDataType_Full = AddRightBand(strColDataType);
                }
                #endregion
            }
            #endregion
            return sDataType_Full;
        }
        #endregion

        #region 转换字段类型与默认值字符
        private void ConvertDbTypeDefaultValueString(ref string sDbType, ref string sDefaultValue, DataBaseType importDbType, DataBaseType targetDbType)
        {
            switch (targetDbType)
            {
                case DataBaseType.SqlServer:
                    #region 目标为SqlServer
                    switch (importDbType)
                    {
                        case DataBaseType.SqlServer:
                            break;
                        case DataBaseType.Oracle:
                            //类型
                            sDbType = sDbType.ToLower().Replace("varchar2", "varchar")
                                .Replace("date", "datetime");
                            //默认值
                            sDefaultValue = sDefaultValue.ToLower().Replace("sysdate", "getdate()")
                                .Replace("sys_guid()", "newid()");
                            break;
                        case DataBaseType.MySql:
                            //默认值
                            sDefaultValue = sDefaultValue.ToLower().Replace("now()", "getdate()");
                            break;
                        case DataBaseType.SQLite:
                            break;
                        case DataBaseType.PostgreSql:
                            //类型
                            sDbType = sDbType.ToLower().Replace("character varying", "varchar")
                                .Replace("date", "datetime")
                                .Replace("int", "integer");
                            //默认值
                            sDefaultValue = sDefaultValue.ToLower().Replace("now()", "getdate()");
                            break;
                        default:
                            throw new Exception("暂不支持该数据库类型！");
                    }
                    break;
                #endregion
                case DataBaseType.Oracle:
                    #region 目标为Oracle
                    switch (importDbType)
                    {
                        case DataBaseType.SqlServer:
                            //类型
                            sDbType = sDbType.ToLower().Replace("varchar", "varchar2").Replace("datetime", "date");
                            //默认值
                            sDefaultValue = sDefaultValue.ToLower().Replace("getdate()", "sysdate")
                                .Replace("newid()", "sys_guid()");
                            break;
                        case DataBaseType.Oracle:
                            break;
                        case DataBaseType.MySql:
                            //默认值
                            sDefaultValue = sDefaultValue.ToLower().Replace("now()", "sysdate");
                            break;
                        case DataBaseType.SQLite:
                            break;
                        case DataBaseType.PostgreSql:
                            //类型
                            sDbType = sDbType.ToLower().Replace("character varying", "varchar2");
                            //默认值
                            sDefaultValue = sDefaultValue.ToLower().Replace("now()", "sysdate");
                            break;
                        default:
                            throw new Exception("暂不支持该数据库类型！");
                    }
                    break;
                #endregion
                case DataBaseType.MySql:
                    #region 目标为MySql
                    switch (importDbType)
                    {
                        case DataBaseType.SqlServer:
                            //默认值
                            sDefaultValue = sDefaultValue.ToLower().Replace("getdate()", "now()");
                            break;
                        case DataBaseType.Oracle:
                            //类型
                            sDbType = sDbType.ToLower().Replace("varchar2", "varchar").Replace("date", "datetime");
                            break;
                        case DataBaseType.MySql:
                            break;
                        case DataBaseType.SQLite:
                            break;
                        case DataBaseType.PostgreSql:
                            //类型
                            sDbType = sDbType.ToLower().Replace("character varying", "varchar")
                                .Replace("date", "datetime");
                            break;
                        default:
                            throw new Exception("暂不支持该数据库类型！");
                    }
                    break;
                #endregion
                case DataBaseType.SQLite:
                    #region 目标为SQLite
                    switch (importDbType)
                    {
                        case DataBaseType.SqlServer:
                            //默认值
                            sDefaultValue = sDefaultValue.ToLower().Replace("getdate()", "")
                                .Replace("newid()", "");
                            break;
                        case DataBaseType.Oracle:
                            //类型
                            sDbType = sDbType.ToLower().Replace("varchar2", "varchar").
                                Replace("date", "datetime");
                            //默认值
                            sDefaultValue = sDefaultValue.ToLower().Replace("sysdate", "")
                                .Replace("sys_guid()", "");
                            break;
                        case DataBaseType.MySql:
                            //默认值
                            sDefaultValue = sDefaultValue.ToLower().Replace("new()", "");
                            break;
                        case DataBaseType.SQLite:
                            break;
                        case DataBaseType.PostgreSql:
                            //类型
                            sDbType = sDbType.ToLower().Replace("character varying", "varchar")
                                .Replace("date", "datetime");
                            //默认值
                            sDefaultValue = sDefaultValue.ToLower().Replace("new()", "");
                            break;
                        default:
                            throw new Exception("暂不支持该数据库类型！");
                    }
                    break;
                #endregion
                case DataBaseType.PostgreSql:
                    #region 目标为PostgreSql
                    switch (importDbType)
                    {
                        case DataBaseType.SqlServer:
                            //类型
                            sDbType = sDbType.ToLower().Replace("varchar", "character varying").
                                Replace("datetime", "date");
                            //默认值
                            sDefaultValue = sDefaultValue.ToLower().Replace("getdate()", "now()");
                            break;
                        case DataBaseType.Oracle:
                            //类型
                            sDbType = sDbType.ToLower().Replace("varchar2", "character varying");
                            //默认值
                            sDefaultValue = sDefaultValue.ToLower().Replace("sysdate", "now()");
                            break;
                        case DataBaseType.MySql:
                            //类型
                            sDbType = sDbType.ToLower().Replace("varchar", "character varying").
                                Replace("datetime", "date");
                            break;
                        case DataBaseType.SQLite:
                            //类型
                            sDbType = sDbType.ToLower().Replace("varchar", "character varying").
                                Replace("datetime", "date");
                            break;
                        case DataBaseType.PostgreSql:
                            break;
                        default:
                            throw new Exception("暂不支持该数据库类型！");
                    }
                    break;
                #endregion
                default:
                    throw new Exception("暂不支持该数据库类型！");
            }
        }
        #endregion

        #region 综合转换复选框变化事件
        private void ckbAllConvert_CheckedChanged(object sender, EventArgs e)
        {
            cbbTargetDbType.Enabled = ckbAllConvert.Checked ? true : false;
        } 
        #endregion
    }

    #region 表处理类型枚举
    public enum TableDealType
    {
        Add = 1,
        Modify = 2
    } 
    #endregion
}
