using Breezee.Framework.BaseUI;
using Breezee.Framework.DataAccess.Custom;
using Breezee.Framework.Tool;
using Breezee.Global.Context;
using Breezee.Global.Entity;
using Breezee.WorkHelper.DBTool.Entity;
using Breezee.WorkHelper.DBTool.INF;
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
    /// 获取表的增删改SQL
    /// </summary>
    public partial class FrmDBTGetTableQuerySql : BaseForm
    {
        #region 变量
        private readonly string _strTableName = "变更表清单";
        private readonly string _strColName = "变更列清单";
        //
        private BindingSource bsTable = new BindingSource();
        private BindingSource bsCos = new BindingSource();//
        private BindingSource bsThree = new BindingSource();//
        //常量
        private static string strTableAlias = "A"; //查询和修改中的表别名
        private static string strTableAliasAndDot = "";
        private static readonly string _strComma = ",";
        private static readonly string _strUpdateCtrolColumnCode = "UPDATE_CONTROL_ID";
        private string _strAutoSqlSuccess = "生成成功，并已复制到了粘贴板。详细见“生成的SQL”页签！";
        private string _strImportSuccess = "导入成功！可点“生成SQL”按钮得到本次导入的变更SQL。";

        string strClassCode = "";
        string strClassCn = "";

        //导入的SQL变量值
        private string _strMainSql = "";//主SQL
        private string _strSecondSql = "";//第二SQL
        string _paraPre = ":";
        private static readonly string _strPreIf = "<if>\n";
        private static readonly string _strEndIf = "<endif>\n";
        bool _columnNameCodeCheck = false;
        //键值维护类
        KeyValue _kv;
        ExcludeColumnSet _ecs;
        //数据集
        private DataSet dsExcel;
        private IDBConfigSet _IDBConfigSet;
        private DbServerInfo _dbServer;
        private CustomDataAccess _dataAccess;
        #endregion

        #region 构造函数
        public FrmDBTGetTableQuerySql()
        {
            InitializeComponent();
        }
        #endregion

        #region 加载事件
        private void FrmGetOracleSql_Load(object sender, EventArgs e)
        {
            #region 绑定下拉框
            _dicString.Add("1", "新增");
            _dicString.Add("2", "修改");
            _dicString.Add("3", "查询");
            _dicString.Add("4", "删除");
            UIHelper.BindTypeValueDropDownList(cmbType, _dicString.GetTextValueTable(false), false, true);
            //
            _dicString.Clear();
            _dicString.Add("1", "左右#号");
            _dicString.Add("2", "SQL参数化");
            UIHelper.BindTypeValueDropDownList(cbbParaType, _dicString.GetTextValueTable(false), false, true);
            #endregion

            #region 设置数据库连接控件
            _IDBConfigSet = ContainerContext.Container.Resolve<IDBConfigSet>();
            DataTable dtConn = _IDBConfigSet.QueryDbConfig(_dicQuery).SafeGetDictionaryTable();
            uC_DbConnection1.SetDbConnComboBoxSource(dtConn);
            uC_DbConnection1.IsDbNameNotNull = true;
            #endregion
        }
        #endregion

        #region 连接数据库事件
        private void tsbImport_Click(object sender, EventArgs e)
        {
            _dbServer = uC_DbConnection1.GetDbServerInfo();
            if (_dbServer == null)
            {
                return;
            }

            #region 查询数据库中的源表
            if (_dbServer.DatabaseType == DataBaseType.Oracle)
            {
                #region Oracle增删改查SQL生成
                if (!string.IsNullOrEmpty(_dbServer.SchemaName))
                {
                    #region 架构不为空
                    //查询表
                    _strMainSql = string.Format(
                            @"SELECT A.OWNER, 
                               A.TABLE_NAME,
                               B.COMMENTS
                          FROM ALL_TABLES A
                          JOIN ALL_TAB_COMMENTS B
                            ON A.TABLE_NAME = B.TABLE_NAME AND A.OWNER=B.OWNER
                         WHERE UPPER(A.TABLE_NAME) = UPPER('{0}') and UPPER(A.OWNER) = UPPER('{1}')",
                             cbbTableName.Text.Trim(), _dbServer.SchemaName);
                    //查询所有列
                    _strSecondSql = string.Format(
                            @"selECT A.COLUMN_ID ,
                               A.COLUMN_NAME,
                               '' 固定值,
                               B.COMMENTS,
                               A.DATA_TYPE,
                               A.DATA_LENGTH,
                               A.DATA_PRECISION,
                               A.DATA_SCALE,
                               A.NULLABLE,
                               A.OWNER,
                               A.TABLE_NAME,
                               (select decode(BB.COLUMN_NAME,null,0,1)  
                                 from all_constraints AA
                                 join all_cons_columns BB on AA.CONSTRAINT_NAME=BB.CONSTRAINT_NAME
                                 where UPPER(AA.TABLE_NAME) = UPPER(A.TABLE_NAME)
                                       AND UPPER(BB.COLUMN_NAME) = UPPER(A.COLUMN_NAME)
                                       AND AA.OWNER=BB.OWNER AND AA.OWNER=A.OWNER
                                 and AA.CONSTRAINT_TYPE='P' and rownum=1) IS_PK
                          FROM ALL_TAB_COLS A
                          JOIN ALL_COL_COMMENTS B ON A.TABLE_NAME=B.TABLE_NAME AND A.COLUMN_NAME=B.COLUMN_NAME AND A.OWNER=B.OWNER
                         WHERE UPPER(A.TABLE_NAME) = UPPER('{0}') and UPPER(A.OWNER) = UPPER('{1}')
                         ORDER BY A.COLUMN_ID",
                          cbbTableName.Text.Trim(), _dbServer.SchemaName);
                    #endregion
                }
                else
                {
                    #region 架构为空
                    //查询表
                    _strMainSql = string.Format(
                            @"SELECT A.OWNER, 
                               A.TABLE_NAME,
                               B.COMMENTS
                          FROM ALL_TABLES A
                          JOIN ALL_TAB_COMMENTS B
                            ON A.TABLE_NAME = B.TABLE_NAME AND A.OWNER=B.OWNER
                         WHERE UPPER(A.TABLE_NAME) = UPPER('{0}')",
                             cbbTableName.Text.Trim());
                    //查询所有列
                    _strSecondSql = string.Format(
                            @"selECT A.COLUMN_ID ,
                               A.COLUMN_NAME,
                               '' 固定值,
                               B.COMMENTS,
                               A.DATA_TYPE,
                               A.DATA_LENGTH,
                               A.DATA_PRECISION,
                               A.DATA_SCALE,
                               A.NULLABLE,
                               A.OWNER,
                               A.TABLE_NAME,
                               (select decode(BB.COLUMN_NAME,null,0,1)  
                                 from all_constraints AA
                                 join all_cons_columns BB on AA.CONSTRAINT_NAME=BB.CONSTRAINT_NAME
                                 where UPPER(AA.TABLE_NAME) = UPPER(A.TABLE_NAME)
                                       and UPPER(BB.COLUMN_NAME) = UPPER(A.COLUMN_NAME)
                                       and AA.OWNER=BB.OWNER AND AA.OWNER=A.OWNER
                                 and AA.CONSTRAINT_TYPE='P' and rownum=1) IS_PK
                          FROM ALL_TAB_COLS A
                          JOIN ALL_COL_COMMENTS B ON A.TABLE_NAME=B.TABLE_NAME AND A.COLUMN_NAME=B.COLUMN_NAME
                         WHERE UPPER(A.TABLE_NAME) = UPPER('{0}')
                         ORDER BY A.COLUMN_ID",
                          cbbTableName.Text.Trim());
                    #endregion
                }
                #endregion
            }
            else if (_dbServer.DatabaseType == DataBaseType.SqlServer)
            {
                #region SQL Server增删改查SQL生成
                if (!string.IsNullOrEmpty(_dbServer.SchemaName))
                {
                    #region 架构不为空时
                    //查询表
                    _strMainSql = string.Format(
                        @"SELECT  b.name owner ,
                            a.name table_name,
                            c.value COMMENTS
                    FROM    sys.objects a
                            JOIN sys.schemas b ON a.schema_id = b.schema_id
                            JOIN sys.extended_properties C ON c.major_id=a.object_id AND C.minor_id=0
                    WHERE   a.type = 'U'
                            AND a.name = '{0}' and b.name= '{1}'",
                          cbbTableName.Text.Trim(), _dbServer.SchemaName);
                    //查询表的所有列（不要的列在界面上删除）
                    _strSecondSql = string.Format(
                        @"SELECT  a.colid COLUMN_ID,
                        a.NAME COLUMN_NAME ,
                        '' 固定值 ,
                        c.value COMMENTS,
                        ( SELECT TOP 1
                                    NAME
                          FROM      sys.types
                          WHERE     user_type_id = a.xusertype
                        ) DATA_TYPE ,
                        a.length DATA_LENGTH ,
                        a.xprec DATA_PRECISION ,
                        a.xscale DATA_SCALE ,
                        a.isnullable ,
                        D.Name as OWNER ,
                        b.NAME TABLE_NAME ,
                        a.COLSTAT
                FROM    syscolumns a
                        JOIN ( SELECT   *
                               FROM     sys.objects
                               WHERE    type = 'U'
                                        AND name = '{0}'
                             ) b ON a.id = b.object_id
                        JOIN sys.extended_properties C ON c.major_id = b.object_id
                                                          AND C.minor_id = a.colid
                                                          AND c.name = 'MS_Description'
                        JOIN sys.schemas D ON D.schema_id = b.SCHEMA_ID
                where D.name='{1}'"
                        , cbbTableName.Text.Trim(), _dbServer.SchemaName);
                    #endregion
                }
                else
                {
                    #region 架构为空时
                    //查询表
                    _strMainSql = string.Format(
                        @"SELECT  b.name owner ,
                            a.name table_name,
                            c.value COMMENTS
                    FROM    sys.objects a
                            JOIN sys.schemas b ON a.schema_id = b.schema_id
                            JOIN sys.extended_properties C ON c.major_id=a.object_id AND C.minor_id=0
                    WHERE   a.type = 'U'
                            AND a.name = '{0}'",
                          cbbTableName.Text.Trim());
                    //查询表的所有列（不要的列在界面上删除）
                    _strSecondSql = string.Format(
                        @"SELECT  a.colid COLUMN_ID,
                        a.NAME COLUMN_NAME ,
                        '' 固定值 ,
                        c.value COMMENTS,
                        ( SELECT TOP 1
                                    NAME
                          FROM      sys.types
                          WHERE     user_type_id = a.xusertype
                        ) DATA_TYPE ,
                        a.length DATA_LENGTH ,
                        a.xprec DATA_PRECISION ,
                        a.xscale DATA_SCALE ,
                        a.isnullable ,
                        ( SELECT TOP 1
                                    name
                          FROM      sys.schemas
                          WHERE     schema_id = b.SCHEMA_ID
                        ) OWNER ,
                        b.NAME TABLE_NAME ,
                        a.COLSTAT
                FROM    syscolumns a
                        JOIN ( SELECT   *
                               FROM     sys.objects
                               WHERE    type = 'U'
                                        AND name = '{0}'
                             ) b ON a.id = b.object_id
                        JOIN sys.extended_properties C ON c.major_id = b.object_id
                                                          AND C.minor_id = a.colid
                                                          AND c.name = 'MS_Description'", cbbTableName.Text.Trim());
                    #endregion
                }
                #endregion
            }
            else
            {
                throw new Exception("暂不支持该数据库类型！");
            }
            #endregion

            dsExcel = new DataSet();
            #region 生成增删改查SQL
            _dataAccess = new CustomDataAccess(_dbServer.DatabaseType, _dbServer);
            DataTable dtMain = _dataAccess.DataAccess.QueryHadParamSqlData(_strMainSql, _dicQueryCondition);
            dtMain.TableName = _strTableName;
            bsTable.DataSource = dtMain;
            DataTable dtSec = _dataAccess.DataAccess.QueryHadParamSqlData(_strSecondSql, _dicQueryCondition);
            //增加条件列
            dtSec.Columns.Add("条件", typeof(bool));
            //增加选择列
            DataColumn dcSelected = new DataColumn("选择", typeof(bool));
            dcSelected.DefaultValue = Boolean.TrueString;
            dtSec.Columns.Add(dcSelected);
            dtSec.TableName = _strColName;
            bsCos.DataSource = dtSec;
            //设置数据源
            GlobalValue.Instance.SetPublicDataSource(new DataTable[] { dtMain, dtSec });
            dgvTableList.DataSource = bsTable;
            dgvColList.DataSource = bsCos;
            //设置网格样式
            bsCos.AllowNew = false;
            foreach (DataGridViewColumn dgvc in dgvColList.Columns)
            {
                dgvc.ReadOnly = true;
                if (dgvc.Name == "固定值" || dgvc.Name == "条件" || dgvc.Name == "选择")
                {
                    dgvc.ReadOnly = false;
                }
            }

            dgvColList.Columns["条件"].DisplayIndex = 0;
            dgvColList.Columns["条件"].Width = 60;
            dgvColList.Columns["选择"].DisplayIndex = 0;
            dgvColList.Columns["选择"].Width = 60;
            dgvColList.Columns["COLUMN_ID"].Width = 40;
            dgvColList.Columns["COMMENTS"].Width = 100;
            #endregion
            //导入成功后处理
            tsbAutoSQL.Enabled = true;
            tabControl1.SelectedTab = tpImport;

            //导入成功提示
            lblInfo.Text = _strImportSuccess;
        }
        #endregion

        #region 生成SQL按钮事件
        private void tsbAutoSQL_Click(object sender, EventArgs e)
        {
            #region 生成增删改查SQL

            #region 变量
            StringBuilder sbAllSql = new StringBuilder();
            string strEndDicSetValue = ""; //字典参数赋值，在SQL末尾增加。目前MDS中用到
            string strEndDicInRemark = "";//注释中的传入字典参数说明。目前MDS中用到
            string strEndColumnsRemark = "";//注释中的列参数说明。目前MDS中用到 
            string strEndTableColumnsRemark = "/// <param type=\"DataTable\" name=\"#PART#\">出库备件清单\n";//注释中的表列参数说明。目前MDS中用到
            string strFileDictionaryRemark = ""; //开发指导书字典注释。目前MDS中用到
            string strFileTableColumnRemark = ""; //开发指导书表的列注释。目前MDS中用到
            string strWhereFirst = "WHERE 1=1 \r";
            string strWhereNoFirst = "WHERE ";
            string strConditionPre = "WHERE 1=1 \r"; //条件前缀
            string strAnd = " AND ";
            string strWhere = "";

            string strMDSQueryParm = "";  //MDS查询的参数
            string strMDSInsertUpdateParm = "";  //新增修改的参数
                                                 //已增加的列键
            IDictionary<string, string> dicHaveAddColumnKey = new Dictionary<string, string>();
            strClassCode = txbClassCode.Text.Trim();
            strClassCn = txbClassNameCn.Text.Trim();
            bool _isQueryParm = cbbParaType.SelectedValue.ToString() == "2" ? true : false;//是否SQL参数化
            #endregion

            #region 取得数据
            string strTwoType = cmbType.SelectedValue.ToString();
            SqlType sqlTypeNow = SqlType.Query; ;
            switch (strTwoType)
            {
                case "1":
                    sqlTypeNow = SqlType.Insert;
                    break;
                case "2":
                    sqlTypeNow = SqlType.Update;
                    break;
                case "3":
                    sqlTypeNow = SqlType.Query;
                    break;
                case "4":
                    sqlTypeNow = SqlType.Delete;
                    break;
                default:
                    sqlTypeNow = SqlType.Query;
                    break;
            }
            //取得数据源
            DataTable dtMain = (DataTable)GlobalValue.Instance.dicBindingSource[_strTableName].DataSource;
            DataTable dtSec = (DataTable)GlobalValue.Instance.dicBindingSource[_strColName].DataSource;
            //移除空行
            dtMain.DeleteNullRow();
            //得到变更后数据
            dtMain.AcceptChanges();
            dtSec.AcceptChanges();
            #endregion

            #region 获取默认值、排除列配置信息
            //获取默认值、排除列配置信息
            DataTable dtDefault = null;
            DataTable dtExclude = null;
            string strTSName = "";

            if (_dbServer.DatabaseType == DataBaseType.SqlServer)
            {
                #region Sql Server
                _kv = new KeyValue(DataBaseType.SqlServer, AutoSqlColumnSetType.Default);
                dtDefault = _kv.LoadXMLFile();
                _kv = new KeyValue(DataBaseType.SqlServer, AutoSqlColumnSetType.Exclude);
                dtExclude = _kv.LoadXMLFile();
                strTSName = txbOrcTableShortName.Text.Trim().Replace(".", "").Replace("'", "");
                strTableAlias = " " + strTSName;//只有查询用别名，其他语句不能使用
                strTableAliasAndDot = strTableAlias + ".";
                if (_isQueryParm)
                {
                    strEndTableColumnsRemark = "/// <param type=\"DataTable\" name=\"PART\">出库备件清单\n";//注释中的表列参数说明。目前MDS中用到
                }
                #endregion
            }
            else
            {
                #region Oracle
                _kv = new KeyValue(DataBaseType.Oracle, AutoSqlColumnSetType.Default);
                dtDefault = _kv.LoadXMLFile();
                _kv = new KeyValue(DataBaseType.Oracle, AutoSqlColumnSetType.Exclude);
                dtExclude = _kv.LoadXMLFile();
                strTSName = txbOrcTableShortName.Text.Trim().Replace(".", "").Replace("'", "");
                strTableAlias = string.IsNullOrEmpty(strTSName) ? " A" : " " + strTSName;//查询和修改中的别名:注前面的空格为必须
                strTableAliasAndDot = strTableAlias + ".";
                if (_isQueryParm)
                {
                    strEndTableColumnsRemark = "/// <param type=\"DataTable\" name=\"PART\">出库备件清单\n";//注释中的表列参数说明。目前MDS中用到
                }
                #endregion
            }


            if (sqlTypeNow == SqlType.Insert && string.IsNullOrEmpty(strTSName))
            {
                strTableAliasAndDot = "";
            }
            #endregion

            #region 得到有效的数据
            DataTable dtSecCopy = dtSec.Clone();
            DataTable dtSecCondition = dtSec.Clone();
            //得到并发ID行
            DataRow[] drUpdateControlColumn = dtSec.Select("COLUMN_NAME='" + _strUpdateCtrolColumnCode + "'");
            //得到查询条件集合
            foreach (DataRow dr in dtSec.Select("条件='True' and COLUMN_NAME<>'" + _strUpdateCtrolColumnCode + "'"))
            {
                dtSecCondition.ImportRow(dr);
            }
            //得到有效的列
            foreach (DataRow dr in dtSec.Select("选择='True' and COLUMN_NAME<>'" + _strUpdateCtrolColumnCode + "'"))
            {
                string strColCodeCopy = dr["COLUMN_NAME"].ToString();
                if (dtExclude.Select("TYPE='" + strTwoType + "' AND KEY='" + strColCodeCopy + "'").Length == 0)
                {
                    if (sqlTypeNow == SqlType.Update) //修改
                    {
                        if (dtSecCondition.Select("COLUMN_NAME='" + strColCodeCopy + "'").Length == 0)
                        {
                            dtSecCopy.ImportRow(dr); //当条件不存在时才导入
                        }
                    }
                    else
                    {
                        dtSecCopy.ImportRow(dr); //对非修改，不是排除列就导入
                    }
                }
            }

            //并发ID字段处理
            if (drUpdateControlColumn.Length > 0)
            {
                if (sqlTypeNow == SqlType.Update)
                {
                    //对于修改的并发ID字段，修改列和条件都增加
                    dtSecCopy.ImportRow(drUpdateControlColumn[0]);
                    dtSecCondition.ImportRow(drUpdateControlColumn[0]);
                }
                else
                {
                    if (drUpdateControlColumn[0]["选择"].ToString() == "True")
                    {
                        if (dtExclude.Select("TYPE='" + strTwoType + "' AND KEY='" + _strUpdateCtrolColumnCode + "'").Length == 0)
                        {
                            dtSecCopy.ImportRow(drUpdateControlColumn[0]);
                        }
                    }
                    if (drUpdateControlColumn[0]["条件"].ToString() == "True")
                    {
                        dtSecCondition.ImportRow(drUpdateControlColumn[0]);
                    }
                }
            }
            #endregion

            #region 得到条件

            //返回按列ID排序的行数组
            DataRow[] drConditionArr = dtSecCondition.Select("1=1", "COLUMN_ID");
            for (int i = 0; i < drConditionArr.Length; i++)
            {
                //变量声明
                string strColCode = drConditionArr[i]["COLUMN_NAME"].ToString().Trim().ToUpper();
                string strColType = drConditionArr[i]["DATA_TYPE"].ToString().Trim().ToUpper();
                string strColFixedValue = drConditionArr[i]["固定值"].ToString().Trim();//固定值
                string strColComments = drConditionArr[i]["COMMENTS"].ToString().Trim();//列说明
                string strColCodeParm = "#" + strColCode + "#"; //加上#号的列编码
                string strNowAnd = strAnd;
                //查询的时间范围
                string strBeginDateParm = "#BEGIN_" + strColCode + "#";
                string strEndDateParm = "#END_" + strColCode + "#";
                string strQueryWhereDateRange = "";
                if (_isQueryParm)
                {
                    strColCodeParm = "@" + strColCode;
                    strBeginDateParm = "@BEGIN_" + strColCode;
                    strEndDateParm = "@END_" + strColCode;
                }
                //列空注释的处理
                strColComments = DataBaseCommon.GetColumnComment(strColCode, strColComments);
                //对修改和删除，WHERE后面不加1=1
                if (i == 0)
                {
                    strConditionPre = strWhereFirst;
                    if (sqlTypeNow == SqlType.Delete || sqlTypeNow == SqlType.Update)
                    {
                        strNowAnd = "";
                        strConditionPre = strWhereNoFirst;
                    }
                }
                else
                {
                    strConditionPre = "";
                }
                if (_dbServer.DatabaseType == DataBaseType.SqlServer)
                {
                    //SQL Server的时间范围
                    strQueryWhereDateRange = strNowAnd + strTableAliasAndDot + strColCode + " >='" + strBeginDateParm + "' \n"
                                    + strAnd + strTableAliasAndDot + strColCode + " < '" + strBeginDateParm + "' \r"; //结束日期：注要传入界面结束时间的+1天。目前MDS中用到
                    if (_columnNameCodeCheck && sqlTypeNow == SqlType.Query) //增加IF
                    {
                        strQueryWhereDateRange = _strPreIf + strNowAnd + strTableAliasAndDot + strColCode + " >='" + strBeginDateParm + "' \n" + _strEndIf
                            + _strPreIf + strAnd + strTableAliasAndDot + strColCode + " < '" + strBeginDateParm + "' \r" + _strEndIf; //结束日期：注要传入界面结束时间的+1天。目前MDS中用到
                    }
                    if (_isQueryParm)
                    {
                        strQueryWhereDateRange = strNowAnd + strTableAliasAndDot + strColCode + " >=" + strBeginDateParm + " \n"
                                    + strAnd + strTableAliasAndDot + strColCode + " < " + strBeginDateParm + " \r"; //结束日期：注要传入界面结束时间的+1天。目前MDS中用到
                    }
                }
                else
                {
                    #region Oracle的时间范围
                    strQueryWhereDateRange = strNowAnd + strTableAliasAndDot + strColCode + " >= TO_DATE('" + strBeginDateParm + "','YYYY-MM-DD') \n"
                                    + strAnd + strTableAliasAndDot + strColCode + " < TO_DATE('" + strEndDateParm + "','YYYY-MM-DD') + 1 \r"; //目前MDS中用到
                    if (_columnNameCodeCheck && sqlTypeNow == SqlType.Query) //增加IF
                    {
                        strQueryWhereDateRange = _strPreIf + strNowAnd + strTableAliasAndDot + strColCode + " >= TO_DATE('" + strBeginDateParm + "','YYYY-MM-DD') \n" + _strEndIf
                                        + _strPreIf + strAnd + strTableAliasAndDot + strColCode + " < TO_DATE('" + strEndDateParm + "','YYYY-MM-DD') + 1 \r" + _strEndIf; //目前MDS中用到
                    }
                    if (_isQueryParm)
                    {
                        strQueryWhereDateRange = strNowAnd + strTableAliasAndDot + strColCode + " >= " + strBeginDateParm + " \n"
                                        + strAnd + strTableAliasAndDot + strColCode + " < " + strEndDateParm + " \r"; //目前MDS中用到
                    }
                    #endregion

                }



                if (sqlTypeNow == SqlType.Query && (strColType == "DATE" || strColType == "DATETIME" || strColType.Contains("TIMESTAMP"))) //列为日期时间类型
                {
                    #region 查询的日期时间段处理
                    strWhere += strConditionPre + strQueryWhereDateRange; //使用范围查询条件
                    if (!dicHaveAddColumnKey.ContainsKey(strBeginDateParm))
                    {
                        dicHaveAddColumnKey[strBeginDateParm] = strBeginDateParm;
                        dicHaveAddColumnKey[strEndDateParm] = strEndDateParm;
                        //字典赋值
                        strEndDicSetValue += DataBaseCommon.GetDicSetValueString(strBeginDateParm, _isQueryParm);
                        strEndDicInRemark += DataBaseCommon.GetFuncParamComment(strBeginDateParm, strColComments, _isQueryParm);
                        strFileDictionaryRemark += DataBaseCommon.GetFileDictionaryParamComment(strBeginDateParm, strColComments, _isQueryParm);
                        //字典赋值
                        strEndDicSetValue += DataBaseCommon.GetDicSetValueString(strEndDateParm, _isQueryParm);
                        strEndDicInRemark += DataBaseCommon.GetFuncParamComment(strEndDateParm, strColComments, _isQueryParm);
                        strFileDictionaryRemark += DataBaseCommon.GetFileDictionaryParamComment(strEndDateParm, strColComments, _isQueryParm);
                        strMDSQueryParm += "parser.Sql(\"BEGIN_" + strColCode + "\", \"AND " + strTableAliasAndDot + strColCode + " >= @BEGIN_" + strColCode + "\",DbDataType.DateTime);\n";
                        strMDSQueryParm += "parser.Sql(\"END_" + strColCode + "\", \"AND " + strTableAliasAndDot + strColCode + " < @END_" + strColCode + " + 1\",DbDataType.DateTime);\n";
                    }

                    #endregion
                }
                else
                {
                    if (_isQueryParm)
                    {
                        strWhere += strConditionPre + strNowAnd + DataBaseCommon.MakeConditionColumnComment(strColCode, strColCodeParm, "", _isQueryParm);
                    }
                    else
                    {
                        //MDS中条件不能加注释，所以列注释传空。固定值传：'#列编码#'
                        if (_columnNameCodeCheck && sqlTypeNow == SqlType.Query)
                        {

                            strWhere += strConditionPre + _strPreIf + strNowAnd + DataBaseCommon.MakeConditionColumnComment(strColCode, "'" + strColCodeParm + "'", "", _isQueryParm) + _strEndIf;
                        }
                        else
                        {

                            strWhere += strConditionPre + strNowAnd + DataBaseCommon.MakeConditionColumnComment(strColCode, "'" + strColCodeParm + "'", "", _isQueryParm);
                        }
                    }
                    if (sqlTypeNow != SqlType.Insert) //新增没有条件
                    {
                        if (!dicHaveAddColumnKey.ContainsKey(strColCodeParm)) //不存在才增加
                        {
                            strEndDicSetValue += DataBaseCommon.GetDicSetValueString(strColCode, _isQueryParm);
                            if (strColCode == _strUpdateCtrolColumnCode)
                            {
                                strEndDicInRemark += DataBaseCommon.GetFuncParamComment(strColCode, strColComments, true); //并发控制ID
                            }
                            else
                            {
                                strEndDicInRemark += DataBaseCommon.GetFuncParamComment(strColCode, strColComments, _isQueryParm);
                            }
                            strFileDictionaryRemark += DataBaseCommon.GetFileDictionaryParamComment(strColCode, strColComments, _isQueryParm);
                            dicHaveAddColumnKey[strColCodeParm] = strColCodeParm;
                            if (_isQueryParm)
                            {
                                if (sqlTypeNow == SqlType.Query)
                                {
                                    strMDSQueryParm += "parser.Sql(\"" + strColCode + "\", \"AND " + strTableAliasAndDot + strColCode + " = @" + strColCode + "\");\n";
                                }
                                else
                                {
                                    strMDSInsertUpdateParm += "\"" + strColCode + "\"" + ",";
                                }
                            }
                        }
                    }
                }
            }
            #endregion

            int iTable = 1; //表计数器
            foreach (DataRow drTable in dtMain.Rows)//针对表清单循环
            {
                #region 变量声明
                string strDataTableName = drTable["TABLE_NAME"].ToString().Trim();
                string strDataTableComment = drTable["COMMENTS"].ToString().Trim();
                string strSelect = "";
                string strInsertColums = "";
                string strInsertVale = "";
                string strUpdate = "";
                string strDelete = "";
                string strOneSql = "";
                int iColumnCount = dtSecCopy.Rows.Count; //选择的总行数
                int j = 0;


                #endregion

                #region 生成SQL
                DataRow[] drArr = dtSecCopy.Select("1=1", "COLUMN_ID");
                for (int i = 0; i < drArr.Length; i++)//针对列清单循环：因为只有一个表，所以第二个网格是该表的全部列
                {
                    #region 变量
                    j = i + 1;
                    string strColCode = drArr[i]["COLUMN_NAME"].ToString().Trim().ToUpper();
                    string strColCodeOld = drArr[i]["COLUMN_NAME"].ToString().Trim().ToUpper();
                    string strColType = drArr[i]["DATA_TYPE"].ToString().Trim().ToUpper();
                    string strColFixedValue = drArr[i]["固定值"].ToString().Trim();//固定值
                    string strColComments = drArr[i]["COMMENTS"].ToString().Trim();//列说明
                    string strColValue = ""; //列值 
                    string strColCodeParm = "#" + strColCode + "#"; //加上#号的列编码
                    string strNowComma = ","; //当前使用的逗号，最行一列的新增和修改是不用加逗号的
                    #endregion
                    if (_isQueryParm)
                    {
                        strColCodeParm = "@" + strColCode;
                    }
                    #region 默认字段注释处理(针对MDS项目)
                    strColComments = DataBaseCommon.GetColumnComment(strColCode, strColComments);
                    #endregion
                    //表列说明的备注构造
                    strEndTableColumnsRemark += DataBaseCommon.GetDicTableColumnRemarkString(strColCode, strColComments);
                    //优先使用本次写的固定值，其次才是维护中的键值。对修改，非条件列，或虽然是条件，但为UPDATE_CONTROL_ID字段，都查找固定设置值
                    if (sqlTypeNow == SqlType.Insert || sqlTypeNow == SqlType.Update)
                    {
                        #region 新增修改处理
                        if (string.IsNullOrEmpty(strColFixedValue))
                        {
                            #region 没有输入固定值
                            //查询全局默认值配置
                            DataRow[] drKeyArr = dtDefault.Select("TYPE='" + strTwoType + "' AND KEY='" + strColCode + "'");

                            if (dtDefault.Rows.Count > 0 && drKeyArr.Length > 0 && !string.IsNullOrEmpty(drKeyArr[0]["VALUE"].ToString()))
                            {
                                #region 全局有该类型设置，且非空
                                strColValue = drKeyArr[0]["VALUE"].ToString().Trim();
                                string strColValuePara = strColValue.Replace("'", ""); //移除引号
                                if (strColValuePara.StartsWith("#") && strColValuePara.EndsWith("#"))
                                {
                                    #region 固定值为参数时
                                    //查找参数设置
                                    DataRow[] drKeyRemark = dtDefault.Select("TYPE='" + ((int)SqlType.Parameter).ToString() + "' AND KEY='" + strColValuePara + "'");
                                    if (drKeyRemark.Length > 0 && !dicHaveAddColumnKey.ContainsKey(strColValuePara))
                                    {
                                        #region 有注释，且字典中没有该键时
                                        //注：strColValuePara已包括了#号
                                        strEndDicSetValue += DataBaseCommon.GetDicSetValueString(strColValuePara, _isQueryParm);
                                        strEndDicInRemark += DataBaseCommon.GetFuncParamComment(strColValuePara, drKeyRemark[0]["VALUE"].ToString(), _isQueryParm);
                                        strFileDictionaryRemark += DataBaseCommon.GetFileDictionaryParamComment(strColValuePara, drKeyRemark[0]["VALUE"].ToString(), _isQueryParm);
                                        //列名不用加#号
                                        strEndColumnsRemark += DataBaseCommon.GetTableColumnComment(strColCode, strColComments);
                                        strFileTableColumnRemark += DataBaseCommon.GetFileColumnComment(strColCode, strColComments);
                                        dicHaveAddColumnKey[strColValuePara] = strColValuePara;
                                        if (_isQueryParm)
                                        {
                                            strMDSInsertUpdateParm += "\"" + strColValuePara.Replace("#", "") + "\"" + ",";
                                        }
                                        #endregion
                                    }
                                    else
                                    {
                                        #region 没有注释，或已增加了该键
                                        //列名不用加#号
                                        strEndColumnsRemark += DataBaseCommon.GetTableColumnComment(strColCode, strColComments);
                                        strFileTableColumnRemark += DataBaseCommon.GetFileColumnComment(strColCode, strColComments);
                                        #endregion
                                    }
                                    #endregion
                                    //参数加上引号
                                    strColValue = "'" + strColValuePara + "'";
                                    if (_isQueryParm)
                                    {
                                        strColValue = "@" + strColValuePara.Replace("#", "");
                                    }
                                }
                                else
                                {
                                    //非参数不加上引号
                                    strColValue = strColValuePara;
                                }
                                #endregion
                            }
                            else //全局默认值设置为空时
                            {
                                #region 全局默认值设置为空时
                                if (!dicHaveAddColumnKey.ContainsKey(strColCodeParm)) //不存在才增加
                                {
                                    strEndDicSetValue += DataBaseCommon.GetDicSetValueString(strColCode, _isQueryParm);
                                    //列名不用加#号
                                    strEndColumnsRemark += DataBaseCommon.GetTableColumnComment(strColCode, strColComments);

                                    if (_dbServer.DatabaseType== DataBaseType.Oracle && drArr[i]["IS_PK"].ToString().Trim() == "1")
                                    {
                                        strEndDicInRemark += DataBaseCommon.GetFuncParamComment(strColCode, strColComments, true);
                                    }
                                    else
                                    {
                                        strEndDicInRemark += DataBaseCommon.GetFuncParamComment(strColCode, strColComments, _isQueryParm);
                                    }
                                    strFileDictionaryRemark += DataBaseCommon.GetFileDictionaryParamComment(strColCode, strColComments, _isQueryParm);
                                    strFileTableColumnRemark += DataBaseCommon.GetFileColumnComment(strColCode, strColComments);
                                    dicHaveAddColumnKey[strColCodeParm] = strColCodeParm;
                                    strMDSInsertUpdateParm += "\"" + strColCode + "\"" + ",";
                                }
                                #endregion
                            }
                            #endregion
                        }
                        else //网格输入了固定值
                        {
                            strColValue = strColFixedValue;
                        }
                        #endregion
                    }
                    else if (sqlTypeNow == SqlType.Query)
                    {
                        //增加程序参数中的列说明、开发指导书的表列说明
                        strEndColumnsRemark += DataBaseCommon.GetTableColumnComment(strColCode, strColComments);
                        strFileTableColumnRemark += DataBaseCommon.GetFileColumnComment(strColCode, strColComments);
                    }
                    //生成SQL
                    if (sqlTypeNow == SqlType.Insert)
                    {
                        #region 新增
                        strWhere = "";
                        if (j == 1) //第一行
                        {
                            if (j == iColumnCount)
                            {
                                strNowComma = "";
                                strInsertColums += "INSERT INTO " + DataBaseCommon.MakeTableComment(strDataTableName + DataBaseCommon.AddRightBand(strTableAlias), strDataTableComment)
                                        + "(\n" + "\t" + strTableAliasAndDot + strColCode + strNowComma + "\n)\n";
                                strInsertVale += "VALUES\n(\n" + DataBaseCommon.MakeAddValueColumnComment(strNowComma, strColCode, strColValue, strColComments, strColType, _isQueryParm) + "\n)\n"; ;
                            }
                            else
                            {
                                strInsertColums += "INSERT INTO " + DataBaseCommon.MakeTableComment(strDataTableName + DataBaseCommon.AddRightBand(strTableAlias), strDataTableComment)
                                        + "(\n" + "\t" + strTableAliasAndDot + strColCode + strNowComma + "\n";
                                strInsertVale += "VALUES\n(\n" + DataBaseCommon.MakeAddValueColumnComment(strNowComma, strColCode, strColValue, strColComments, strColType, _isQueryParm);
                            }
                        }
                        else if (j != iColumnCount) //非首行，并且非尾行
                        {
                            strInsertColums += "\t" + strTableAliasAndDot + strColCode + _strComma + "\n"; ;
                            strInsertVale += DataBaseCommon.MakeAddValueColumnComment(strNowComma, strColCode, strColValue, strColComments, strColType, _isQueryParm);
                        }
                        else //最后一行
                        {
                            strNowComma = "";
                            strInsertColums += "\t" + strTableAliasAndDot + strColCode + "\n)\n";
                            //最后一行不用加逗号
                            strInsertVale += DataBaseCommon.MakeAddValueColumnComment(strNowComma, strColCode, strColValue, strColComments, strColType, _isQueryParm) + ")\n";
                        }
                        strOneSql = strInsertColums + strInsertVale;
                        #endregion
                    }
                    else if (sqlTypeNow == SqlType.Update)
                    {
                        #region 修改
                        if (j == 1) //第一行
                        {
                            if (j == iColumnCount)
                            {
                                strNowComma = "";
                            }
                            strUpdate += "UPDATE " + DataBaseCommon.MakeTableComment(strDataTableName + DataBaseCommon.AddRightBand(strTableAlias), strDataTableComment)
                                    + "SET " + DataBaseCommon.MakeUpdateColumnComment(strNowComma, strColCode, strColValue, strColComments, strColType, _isQueryParm);
                        }
                        else if (j != iColumnCount) //中间行
                        {
                            strUpdate += DataBaseCommon.MakeUpdateColumnComment(strNowComma, strColCode, strColValue, strColComments, strColType, _isQueryParm);
                        }
                        else //最后一行
                        {
                            strNowComma = "";
                            strUpdate += DataBaseCommon.MakeUpdateColumnComment(strNowComma, strColCode, strColValue, strColComments, strColType, _isQueryParm);
                        }
                        strOneSql = strUpdate;
                        #endregion}
                    }
                    else if (sqlTypeNow == SqlType.Query)
                    {
                        #region 查询
                        if (j == 1) //第一行
                        {
                            if (j == iColumnCount)
                            {
                                strNowComma = "";
                                strSelect += "SELECT " + DataBaseCommon.MakeQueryColumnComment(strNowComma, strTableAliasAndDot + strColCode, strColComments) + "FROM "
                                + DataBaseCommon.MakeTableComment(strDataTableName + DataBaseCommon.AddRightBand(strTableAlias), strDataTableComment);
                            }
                            else
                            {
                                strSelect += "SELECT " + DataBaseCommon.MakeQueryColumnComment(strNowComma, strTableAliasAndDot + strColCode, strColComments);
                            }
                        }
                        else if (j != iColumnCount) //中间行
                        {
                            strSelect += DataBaseCommon.MakeQueryColumnComment(strNowComma, strTableAliasAndDot + strColCode, strColComments);
                        }
                        else //最后一行
                        {
                            strNowComma = "";
                            strSelect += DataBaseCommon.MakeQueryColumnComment(strNowComma, strTableAliasAndDot + strColCode, strColComments) + "FROM "
                                + DataBaseCommon.MakeTableComment(strDataTableName + DataBaseCommon.AddRightBand(strTableAlias), strDataTableComment);
                        }
                        strOneSql = strSelect;
                        #endregion
                    }
                }
                if (sqlTypeNow == SqlType.Delete)
                {
                    #region 删除
                    strDelete += "DELETE FROM " + strDataTableName + DataBaseCommon.AddRightBand(strTableAlias) + "\n";
                    strOneSql = strDelete;
                    #endregion
                }
                #endregion

                if (_isQueryParm)
                {
                    if (!string.IsNullOrEmpty(strMDSInsertUpdateParm))
                    {
                        if (sqlTypeNow == SqlType.Update)
                        {

                        }
                        strMDSInsertUpdateParm = strMDSInsertUpdateParm.Substring(0, strMDSInsertUpdateParm.Length - 1);
                    }
                    if (sqlTypeNow == SqlType.Query)
                    {
                        strOneSql = "SqlQueryParser parser = new SqlQueryParser();\nparser.Sql(@\"" + strOneSql + " WHERE 1=1 \");\n" + strMDSQueryParm
                            + "DataTable dtReturn = parser.Query(dicParam);\nreturn dtReturn;\n";
                    }
                    else
                    {
                        strOneSql = "int iEff = SqlQueryParser.Update(\n@\"" + strOneSql + strWhere + "\",dicParam,tran," + strMDSInsertUpdateParm + ");\n"
                            + "if (iEff == 0)\n"
                            + "{\n"
                            + "    throw new ConcurrencyException();\n"
                            + "}\n"
                            + "return ExecutionResult.Success();\n";
                    }
                }
                else
                {
                    strOneSql += strWhere;
                }
                string strFuncRemark = "";
                if (sqlTypeNow == SqlType.Query)
                {
                    strFuncRemark = "\r/*DataAcce方法*/\r" + DataBaseCommon.GetFuncParmQueryPreString(true, _isQueryParm) + strEndDicInRemark
                        + DataBaseCommon.GetFuncParmQueryMiddleString(_isQueryParm) + strEndColumnsRemark + DataBaseCommon.GetFuncParmQueryEndString(true, _isQueryParm, strOneSql)
                        + "\r/*Bussiness方法*/\r" + DataBaseCommon.GetFuncParmQueryPreString(false, _isQueryParm) + strEndDicInRemark
                        + DataBaseCommon.GetFuncParmQueryMiddleString(_isQueryParm) + strEndColumnsRemark + DataBaseCommon.GetFuncParmQueryEndString(false, _isQueryParm, strOneSql);
                }
                else
                {
                    strFuncRemark = "\r/*DataAcce方法*/\r" + DataBaseCommon.GetFuncParmSavePreString(true, _isQueryParm) + strEndDicInRemark
                         + DataBaseCommon.GetFuncParmSaveEndString(true, _isQueryParm, strOneSql)
                        + "\r/*Bussiness方法*/\r" + DataBaseCommon.GetFuncParmSavePreString(false, _isQueryParm) + strEndDicInRemark
                         + DataBaseCommon.GetFuncParmSaveEndString(false, _isQueryParm, strOneSql);
                }
                strEndTableColumnsRemark += "/// </param>";
                sbAllSql.Append(strOneSql + strFuncRemark
                    + "\r/*开发指导书字典注释*/\r" + strFileDictionaryRemark
                    + "\r/*开发指导书表的列注释*/\r" + strFileTableColumnRemark
                    + "\r/*参数赋值*/\r" + strEndDicSetValue
                    + "\r/*表列注释*/\r" + strEndTableColumnsRemark);
                iTable++;
            }
            rtbResult.Clear();
            rtbResult.AppendText(sbAllSql.ToString() + "\n");
            Clipboard.SetData(DataFormats.UnicodeText, sbAllSql.ToString());
            tabControl1.SelectedTab = tpAutoSQL;
            //生成SQL成功后提示
            MsgHelper.ShowInfo(_strAutoSqlSuccess);
            return;
            #endregion
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
