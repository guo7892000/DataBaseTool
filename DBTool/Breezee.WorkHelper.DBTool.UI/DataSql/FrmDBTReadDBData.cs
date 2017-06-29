using Breezee.Framework.BaseUI;
using Breezee.Framework.DataAccess.Custom;
using Breezee.Framework.Tool;
using Breezee.Global.Context;
using Breezee.Global.Entity;
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
    /// 读取SqlServer数据库生成数据
    /// </summary>
    public partial class FrmDBTReadDBData : BaseForm
    {
        #region 变量
        private readonly string _strTableName = "变更表清单";
        private readonly string _strColName = "变更列清单";
        //
        private BindingSource bsTable = new BindingSource();
        private BindingSource bsCos = new BindingSource();//
        private BindingSource bsThree = new BindingSource();//
        //常量
        private string _strAutoSqlSuccess = "生成成功，并已复制到了粘贴板。详细见“生成的SQL”页签！";
        private string _strImportSuccess = "导入成功！可点“生成SQL”按钮得到本次导入的变更SQL。";

        //导入的SQL变量值
        private string _strMainSql = "";//主SQL
        private string _strSecondSql = "";//第二SQL

        //数据集
        private DataSet dsExcel;
        private IDBConfigSet _IDBConfigSet;
        private DbServerInfo _dbServer;
        private CustomDataAccess _dataAccess;
        #endregion

        #region 构造函数
        public FrmDBTReadDBData()
        {
            InitializeComponent();
        }
        #endregion

        #region 加载事件
        private void FrmReadSqlServerDBData_Load(object sender, EventArgs e)
        {
            #region 初始化下拉框
            _dicString.Add("0", "全部提交");
            _dicString.Add("1", "单次提交");
            _dicString.Add("20", "每20次提交");
            _dicString.Add("50", "每50次提交");
            _dicString.Add("100", "每100次提交");
            _dicString.Add("200", "每200次提交");
            _dicString.Add("500", "每500次提交");
            UIHelper.BindTypeValueDropDownList(cbbCommitType, _dicString.GetTextValueTable(false), false, true);
            //数据库类型
            DataTable dtDbType = DBToolUIHelper.GetBaseDataTypeTable();
            UIHelper.BindTypeValueDropDownList(cbbTargetDbType, dtDbType, false, true);
            cbbTargetDbType.SelectedIndexChanged += cbbTargetDbType_SelectedIndexChanged;
            #endregion

            #region 设置数据库连接控件
            _IDBConfigSet = ContainerContext.Container.Resolve<IDBConfigSet>();
            //过滤指定数据库类型配置
            DataTable dtConn = _IDBConfigSet.QueryDbConfig(_dicQuery).SafeGetDictionaryTable();
            uC_DbConnection1.SetDbConnComboBoxSource(dtConn);
            uC_DbConnection1.IsDbNameNotNull = false;
            uC_DbConnection1.DBType_SelectedIndexChanged += DataBaseType_SelectedChange;
            uC_DbConnection1.DBConnName_SelectedIndexChanged += DBConnName_SelectedChange;
            #endregion

            tsbExport.Enabled = false;
        }
        #endregion

        #region 数据库连接名选择变化事件
        private void DBConnName_SelectedChange(object sender, EventArgs e)
        {
            cbbTableName.DataSource = null;
            ckbGetTableList.Checked = false;
            tsbExport.Enabled = false;
        }
        #endregion

        #region 数据库类型选择变化事件
        private void DataBaseType_SelectedChange(object sender, DBTypeSelectedChangeEventArgs e)
        {
            ckbMainKeyInsert.Visible = false;
            rtbWhere.Enabled = true;

            //目标数据库类型
            int iDbType = int.Parse(cbbTargetDbType.SelectedValue.ToString());
            DataBaseType selectDBType = (DataBaseType)iDbType;

            switch (e.SelectDBType)
            {
                case DataBaseType.SqlServer:
                    if (selectDBType == DataBaseType.SqlServer)
                    {
                        ckbMainKeyInsert.Visible = true;
                    }
                    break;
                case DataBaseType.Oracle:
                    break;
                case DataBaseType.MySql:
                    rtbWhere.Text = string.Empty;
                    rtbWhere.Enabled = false;
                    break;
                case DataBaseType.SQLite:
                    break;
                case DataBaseType.PostgreSql:
                    break;
                default:
                    throw new Exception("暂不支持该数据库类型！");
                    //break;
            }
        }
        #endregion

        #region 查询按钮事件
        private void tsbImport_Click(object sender, EventArgs e)
        {
            try
            {
                dsExcel = new DataSet();
                _dbServer = uC_DbConnection1.GetDbServerInfo();
                DataTable dtMain;
                DataTable dtSec;
                if (_dbServer == null)
                {
                    return;
                }
                string sTableName = cbbTableName.Text.Trim();

                #region 读取数据库的表数据生成SQL处理

                #region 确定SQL
                if (_dbServer.DatabaseType == DataBaseType.SqlServer)
                {
                    #region SqlServer读取数据库
                    if (!string.IsNullOrEmpty(_dbServer.SchemaName)) //有架构名时
                    {
                        //查询表
                        _strMainSql = string.Format(
                            @"SELECT B.NAME OWNER,A.NAME TABLE_NAME 
                                FROM SYS.OBJECTS A  
                                JOIN SYS.SCHEMAS B ON A.SCHEMA_ID=B.SCHEMA_ID 
                                WHERE A.TYPE='U' AND A.NAME='{0}' AND B.NAME='{1}'"
                                    , sTableName, _dbServer.SchemaName);
                        //查询表的所有列（不要的列在界面上删除）
                        _strSecondSql = string.Format(
                            @"SELECT A.COLID COLUMN_ID,A.NAME COLUMN_NAME,'' 固定值, '' 辅助查询值,
                                    (SELECT TOP 1 NAME FROM SYS.TYPES WHERE USER_TYPE_ID = A.XUSERTYPE) DATA_TYPE, 
                                    A.LENGTH DATA_LENGTH,A.XPREC DATA_PRECISION,A.XSCALE DATA_SCALE,A.ISNULLABLE NULLABLE,
                                    C.NAME AS OWNER,
                                    B.NAME TABLE_NAME,A.COLSTAT
                                FROM SYSCOLUMNS A 
                                JOIN (SELECT * FROM SYS.OBJECTS WHERE TYPE='U' AND NAME='{0}') B ON A.ID=B.OBJECT_ID
                                JOIN SYS.SCHEMAS C ON C.SCHEMA_ID=B.SCHEMA_ID
                                WHERE C.NAME='{1}'
                                ORDER BY A.COLID", sTableName, _dbServer.SchemaName);
                    }
                    else
                    {
                        //查询表
                        _strMainSql = string.Format(
                            @"SELECT B.NAME OWNER,A.NAME TABLE_NAME 
                                FROM SYS.OBJECTS A  
                                JOIN SYS.SCHEMAS B ON A.SCHEMA_ID=B.SCHEMA_ID WHERE A.TYPE='U' AND A.NAME='{0}'"
                                    , sTableName);
                        //查询表的所有列（不要的列在界面上删除）
                        _strSecondSql = string.Format(
                            @"SELECT A.COLID COLUMN_ID,A.NAME COLUMN_NAME,'' 固定值, '' 辅助查询值,
                                    (SELECT TOP 1 NAME FROM SYS.TYPES WHERE USER_TYPE_ID = A.XUSERTYPE) DATA_TYPE, 
                                    A.LENGTH DATA_LENGTH,A.XPREC DATA_PRECISION,A.XSCALE DATA_SCALE,A.ISNULLABLE NULLABLE,
                                    (SELECT TOP 1 NAME FROM SYS.SCHEMAS WHERE SCHEMA_ID=B.SCHEMA_ID) OWNER,
                                    B.NAME TABLE_NAME,A.COLSTAT
                             FROM SYSCOLUMNS A 
                             JOIN (SELECT * FROM SYS.OBJECTS WHERE TYPE='U' AND NAME='{0}') B ON A.ID=B.OBJECT_ID
                             ORDER BY A.COLID",
                            sTableName);
                    } 
                    #endregion
                }
                else if (_dbServer.DatabaseType == DataBaseType.Oracle)
                {
                    #region Oracle读取数据库
                    if (!string.IsNullOrEmpty(_dbServer.SchemaName)) //当输入架构名称时
                    {
                        //查询表
                        _strMainSql = string.Format(
                            @"SELECT A.OWNER,A.TABLE_NAME 
                                FROM ALL_TABLES A 
                              WHERE UPPER(A.TABLE_NAME)=UPPER('{0}') AND UPPER(A.OWNER)=UPPER('{1}')",
                            sTableName, _dbServer.SchemaName);
                        //查询所有列
                        _strSecondSql = string.Format(
                            @"SELECT A.COLUMN_ID,A.COLUMN_NAME,'' 固定值,'' 辅助查询值,A.DATA_TYPE,A.DATA_LENGTH,
                                     A.DATA_PRECISION,A.DATA_SCALE,A.NULLABLE,A.OWNER,A.TABLE_NAME 
                              FROM ALL_TAB_COLS A 
                              WHERE UPPER(A.TABLE_NAME)=UPPER('{0}') AND UPPER(A.OWNER)=UPPER('{1}')
                              ORDER BY A.COLUMN_ID", sTableName, _dbServer.SchemaName);
                    }
                    else
                    {
                        //查询表
                        _strMainSql = string.Format(
                            @"SELECT A.OWNER,A.TABLE_NAME 
                              FROM ALL_TABLES A 
                              WHERE UPPER(A.TABLE_NAME)=UPPER('{0}')",
                            sTableName);
                        //查询所有列
                        _strSecondSql = string.Format(
                            @"SELECT A.COLUMN_ID,A.COLUMN_NAME,'' 固定值,'' 辅助查询值,A.DATA_TYPE,A.DATA_LENGTH,
                                        A.DATA_PRECISION,A.DATA_SCALE,A.NULLABLE,A.OWNER,A.TABLE_NAME 
                             FROM ALL_TAB_COLS A WHERE UPPER(TABLE_NAME)=UPPER('{0}') ORDER BY A.COLUMN_ID", 
                           sTableName);
                    }
                    #endregion
                }
                else if (_dbServer.DatabaseType == DataBaseType.MySql)
                {
                    #region Mariadb读取数据库
                    if (string.IsNullOrEmpty(_dbServer.Database))
                    {
                        MsgHelper.ShowErr("数据库名不能为空！");
                        return;
                    }
                    if (string.IsNullOrEmpty(sTableName))
                    {
                        MsgHelper.ShowErr("表名不能为空！");
                        cbbTableName.Focus();
                        return;
                    }
                    
                    //查询表
                    _strMainSql = string.Format(
                        @"SHOW TABLES 
                        WHERE TABLES_IN_"+ _dbServer.Database + " = LOWER('{0}')",
                        sTableName);
                    //查询所有列
                    _strSecondSql = string.Format(
                        @"SHOW COLUMNS FROM {0}",
                       sTableName);
                    #endregion
                }
                else if (_dbServer.DatabaseType == DataBaseType.PostgreSql)
                {
                    #region PostgreSql读取数据库
                    if (!string.IsNullOrEmpty(_dbServer.SchemaName)) //当输入架构名称时
                    {
                        //查询表
                        _strMainSql = string.Format(
                            @"SELECT A.SCHEMANAME AS OWNER,A.TABLENAME AS TABLE_NAME
                             FROM PG_TABLES A    
                             WHERE 1=1
                              AND UPPER(A.TABLENAME)=UPPER('{0}') AND UPPER(A.SCHEMANAME)=UPPER('{1}')",
                            sTableName, _dbServer.SchemaName);
                        //查询所有列
                        _strSecondSql = string.Format(
                            @" SELECT A.ORDINAL_POSITION as COLUMN_ID,a.COLUMN_NAME,'' 固定值,'' 辅助查询值,a.DATA_TYPE,
 	                                A.CHARACTER_MAXIMUM_LENGTH as DATA_LENGTH,A.NUMERIC_PRECISION as DATA_PRECISION,
                                    A.NUMERIC_SCALE as DATA_SCALE,A.IS_NULLABLE as NULLABLE,A.TABLE_SCHEMA as owner,A.TABLE_NAME as TABLE_NAME 
                                FROM INFORMATION_SCHEMA.COLUMNS A
                                LEFT JOIN (SELECT C.ATTNAME AS COLUMN_NAME
                                            FROM PG_CONSTRAINT A
                                            JOIN PG_CLASS B
	                                            ON A.CONRELID = B.OID 
                                            JOIN PG_ATTRIBUTE C
	                                            ON C.ATTRELID = B.OID 
	                                            AND  C.ATTNUM = A.CONKEY[1]
                                            JOIN PG_TYPE D
	                                            ON D.OID = C.ATTTYPID
                                            WHERE UPPER(B.RELNAME) = UPPER('{0}') 
	                                            AND A.CONTYPE = 'p') PK ON A.COLUMN_NAME = PK.COLUMN_NAME
                                WHERE UPPER(A.TABLE_SCHEMA)=UPPER('{1}') 
	                                AND UPPER(A.TABLE_NAME)=UPPER('{0}')
                                ORDER BY A.ORDINAL_POSITION", 
                            sTableName, _dbServer.SchemaName);
                    }
                    else
                    {
                        //查询表
                        _strMainSql = string.Format(@"SELECT A.SCHEMANAME AS OWNER,A.TABLENAME AS TABLE_NAME
                             FROM PG_TABLES A    
                             WHERE 1=1
                              AND UPPER(A.TABLENAME)=UPPER('{0}')",
                            sTableName);
                        //查询所有列
                        _strSecondSql = string.Format(
                            @" SELECT A.ORDINAL_POSITION AS COLUMN_ID,A.COLUMN_NAME,'' 固定值,'' 辅助查询值,A.DATA_TYPE,
 	                                A.CHARACTER_MAXIMUM_LENGTH AS DATA_LENGTH,A.NUMERIC_PRECISION AS DATA_PRECISION,
                                    A.NUMERIC_SCALE AS DATA_SCALE,A.IS_NULLABLE AS NULLABLE,A.TABLE_SCHEMA AS OWNER,A.TABLE_NAME AS TABLE_NAME 
                                FROM INFORMATION_SCHEMA.COLUMNS A
                                LEFT JOIN (SELECT C.ATTNAME AS COLUMN_NAME
                                            FROM PG_CONSTRAINT A
                                            JOIN PG_CLASS B
	                                            ON A.CONRELID = B.OID 
                                            JOIN PG_ATTRIBUTE C
	                                            ON C.ATTRELID = B.OID 
	                                            AND  C.ATTNUM = A.CONKEY[1]
                                            JOIN PG_TYPE D
	                                            ON D.OID = C.ATTTYPID
                                            WHERE UPPER(B.RELNAME) = UPPER('{0}') 
	                                            AND A.CONTYPE = 'P') PK ON A.COLUMN_NAME = PK.COLUMN_NAME
                                WHERE UPPER(A.TABLE_SCHEMA)=UPPER('PUBLIC') 
	                                AND UPPER(A.TABLE_NAME)=UPPER('{0}')
                                ORDER BY A.ORDINAL_POSITION",
                                   sTableName);
                    }
                    #endregion
                }
                else if (_dbServer.DatabaseType == DataBaseType.SQLite)
                {
                    #region SQLite读取数据库
                    //查询表:type,name,TBL_NAME,rootpage,sql
                    _strMainSql = string.Format(
                        @"SELECT TBL_NAME AS TABLE_NAME 
                          FROM SQLITE_MASTER 
                          WHERE UPPER(TYPE)= 'TABLE' AND UPPER(NAME)= ('{0}')",
                        sTableName);
                    //查询所有列:cid,name,type,notnull,dflt_value,pk
                    _strSecondSql = string.Format(
                        @"PRAGMA TABLE_INFO('{0}')",
                       sTableName);
                    #endregion
                }
                else
                {
                    throw new Exception("暂不支持该数据库类型！");
                }
                #endregion
                
                _dataAccess = new CustomDataAccess(_dbServer.DatabaseType, _dbServer);
                _dicQueryCondition.Clear();

                #region 查询或构造表
                if (_dbServer.DatabaseType == DataBaseType.MySql || _dbServer.DatabaseType == DataBaseType.SQLite)
                {
                    DataTable dtMainTemp = _dataAccess.DataAccess.QueryHadParamSqlData(_strMainSql, _dicQueryCondition);
                    DataTable dtSecTemp = _dataAccess.DataAccess.QueryHadParamSqlData(_strSecondSql, _dicQueryCondition);
                    if (dtMainTemp.Rows.Count == 0)
                    {
                        MsgHelper.ShowErr("表不存在！");
                        return;
                    }

                    //生成表和列
                    GenerateTableColumn(out dtMain, out dtSec);

                    dtMain.Rows[0]["TABLE_NAME"] = dtMainTemp.Rows[0][0];
                    int i = 1;
                    foreach (DataRow row in dtSecTemp.Rows)
                    {
                        DataRow drNew = dtSec.NewRow();
                        if (_dbServer.DatabaseType == DataBaseType.MySql)
                        {
                            drNew["COLUMN_ID"] = i;
                            drNew["COLUMN_NAME"] = row["Field"];
                            drNew["DATA_TYPE"] = row["Type"];
                            drNew["NULLABLE"] = row["Null"];
                        }
                        else
                        {
                            drNew["COLUMN_ID"] = row["cid"];
                            drNew["COLUMN_NAME"] = row["name"];
                            drNew["DATA_TYPE"] = row["type"];
                            drNew["NULLABLE"] = row["notnull"];
                        }

                        dtSec.Rows.Add(drNew);
                        i++;
                    }
                }
                else
                {
                    dtMain = _dataAccess.DataAccess.QueryHadParamSqlData(_strMainSql, _dicQueryCondition);
                    dtSec = _dataAccess.DataAccess.QueryHadParamSqlData(_strSecondSql, _dicQueryCondition);
                } 
                #endregion

                dtMain.TableName = _strTableName;
                bsTable.DataSource = dtMain;
                dtSec.TableName = _strColName;
                bsCos.DataSource = dtSec;

                #region SqlServer自增长列处理
                if (_dbServer.DatabaseType == DataBaseType.SqlServer)
                {
                    if (dtSec.Select("COLSTAT>0").Length > 0)
                    {
                        ckbMainKeyInsert.Enabled = true;
                    }
                    else
                    {
                        ckbMainKeyInsert.Enabled = false;
                    }
                }
                else
                {
                    ckbMainKeyInsert.Visible = false; //不可见
                } 
                #endregion

                //设置数据源
                GlobalValue.Instance.SetPublicDataSource(new DataTable[] { dtMain, dtSec });
                dgvTableList.DataSource = bsCos;
                //dgvColList.DataSource = null;
                //设置网格样式
                bsCos.AllowNew = false;
                foreach (DataGridViewColumn dgvc in dgvTableList.Columns)
                {
                    dgvc.ReadOnly = true;
                    if (dgvc.Name == "固定值" || dgvc.Name == "辅助查询值")
                    {
                        dgvc.ReadOnly = false;
                    }
                }
                #endregion
                //导入成功后处理
                tsbAutoSQL.Enabled = true;
                tsbExport.Enabled = true;
                tabControl1.SelectedTab = tpImport;

                //导入成功提示
                lblInfo.Text = _strImportSuccess;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion

        #region 生成表和列
        /// <summary>
        /// 针对MySQL
        /// </summary>
        /// <param name="dtMain"></param>
        /// <param name="dtSec"></param>
        private static void GenerateTableColumn(out DataTable dtMain, out DataTable dtSec)
        {
            dtMain = new DataTable();
            dtMain.Columns.Add("OWNER", typeof(string));
            dtMain.Columns.Add("TABLE_NAME", typeof(string));
            dtMain.Rows.Add(dtMain.NewRow());

            dtSec = new DataTable();
            dtSec.Columns.Add("COLUMN_ID", typeof(string));
            dtSec.Columns.Add("COLUMN_NAME", typeof(string));
            dtSec.Columns.Add("固定值", typeof(string));
            dtSec.Columns.Add("辅助查询值", typeof(string));
            dtSec.Columns.Add("DATA_TYPE", typeof(string));
            dtSec.Columns.Add("DATA_LENGTH", typeof(string));
            dtSec.Columns.Add("DATA_PRECISION", typeof(string));
            dtSec.Columns.Add("DATA_SCALE", typeof(string));
            dtSec.Columns.Add("NULLABLE", typeof(string));
            dtSec.Columns.Add("OWNER", typeof(string));
            dtSec.Columns.Add("TABLE_NAME", typeof(string));
        }
        #endregion

        #region 生成SQL按钮事件
        private void tsbAutoSQL_Click(object sender, EventArgs e)
        {
            #region 数据库表数据导入处理
            string strWhere = string.IsNullOrEmpty(rtbWhere.Text.Trim()) == true ? "" : " WHERE " + rtbWhere.Text.Trim();

            //取得数据源
            DataTable dtMain = (DataTable)GlobalValue.Instance.dicBindingSource[_strTableName].DataSource;
            DataTable dtSec = (DataTable)GlobalValue.Instance.dicBindingSource[_strColName].DataSource;
            //移除空行
            dtMain.DeleteNullRow();
            //得到变更后数据
            dtMain.AcceptChanges();
            dtSec.AcceptChanges();
            //目标数据库类型
            int iDbType = int.Parse(cbbTargetDbType.SelectedValue.ToString());
            DataBaseType selectDBType = (DataBaseType)iDbType;

            #region 提交字符处理
            string strDataStyle = cbbCommitType.SelectedValue.ToString();//提交方式
            string strCommit = "\n";//提交字符
            if (selectDBType == DataBaseType.SqlServer)
            {
                strCommit = "GO\n";//提交字符
            }
            else if (selectDBType == DataBaseType.Oracle)
            {
                strCommit = "commit;\n";
            }
            #endregion
            StringBuilder sbAllSql = new StringBuilder();
            int iTable = 1;
            foreach (DataRow drTable in dtMain.Rows)//针对表清单循环
            {
                string strDataTableName = drTable["TABLE_NAME"].ToString().Trim();
                string strDataDealType = "新增";
                string strColQueryAllSql = "";//所有列清单
                string strColQuerySql = "";//插入列查询SQL，有固定值的不包括
                string strColInsertSql = "";//新增数据列清单

                sbAllSql.Append("/**" + iTable.ToString() + "  " + strDataTableName + strDataDealType + "新增数据*/\n");
                if (_dbServer.DatabaseType == DataBaseType.SqlServer && selectDBType == DataBaseType.SqlServer && ckbMainKeyInsert.Checked)
                {
                    sbAllSql.Append("Set IDENTITY_INSERT " + strDataTableName + " ON\n");
                }
                #region 确定查询列SQL、新增SQL，将SQL参数化（格式：#+列名+#）
                foreach (DataRow drCol in dtSec.Rows)//针对列清单循环
                {
                    string strColCode = drCol["COLUMN_NAME"].ToString().Trim().ToUpper();
                    string strColFixedValue = drCol["固定值"].ToString().Trim();//固定值
                    string strColHelpQueryValue = drCol["辅助查询值"].ToString().Trim(); //辅助查询值
                    //查询所有列
                    strColQueryAllSql += strColCode + ",";

                    if (string.IsNullOrEmpty(strColFixedValue) && drCol["DATA_TYPE"].ToString() == "TIMESTAMP(6)")
                    {
                        strColQuerySql += "to_char(" + strColCode + ") as " + strColCode + ",";//将oracle的时间类型转换为文本
                    }
                    //查询数据列清单
                    else if(string.IsNullOrEmpty(strColFixedValue))
                    {
                        strColQuerySql += strColCode + ",";
                    }
                    else if (!string.IsNullOrEmpty(strColFixedValue) && !string.IsNullOrEmpty(strColHelpQueryValue))
                    {
                        strColQuerySql += strColCode + ",";
                    }
                    //新增SQL列清单
                    if (!string.IsNullOrEmpty(strColFixedValue))//固定值不为空时
                    {
                        strColInsertSql += strColFixedValue + ",";
                    }
                    else
                    {
                        strColInsertSql += "'#" + strColCode + "#',";//将SQL参数化
                    }
                }
                #endregion
                strColQuerySql = strColQuerySql.Substring(0, strColQuerySql.Length - 1);
                strColQueryAllSql = strColQueryAllSql.Substring(0, strColQueryAllSql.Length - 1);
                //操作记录数
                DataTable dtCount = _dataAccess.DataAccess.QueryHadParamSqlData("SELECT COUNT(*) FROM " + strDataTableName + strWhere, _dicQueryCondition);
                if (dtCount.Rows[0][0].ToString() == "0")
                {
                    MsgHelper.ShowErr("没有要生成的记录！", "提示"); 
                    return;
                }

                if (int.Parse(dtCount.Rows[0][0].ToString()) > 1000)//超过1000条才提示是否继续
                {
                    if (MsgHelper.ShowOkCancel("本次操作记录数：" + dtCount.Rows[0][0].ToString() + "，是否继续？") == DialogResult.Cancel)
                    {
                        return;
                    }
                }
                //获取数据信息
                string strColDataList = "SELECT " + strColQuerySql + "  FROM " + strDataTableName + strWhere;
                DataTable dtData = _dataAccess.DataAccess.QueryHadParamSqlData(strColDataList, _dicQueryCondition);
                //生成SQL的前缀和后缀 
                string strInsertPre = "insert into " + strDataTableName + "(" + strColQueryAllSql + ") \n values (";//Oracler的Value方式支持子查询和时间类型，Select方式不支持
                string strInsertOracleEnd = " );";
                if (selectDBType == DataBaseType.SqlServer)
                {
                    strInsertPre = "insert into " + strDataTableName + "(" + strColQueryAllSql + ") \nselect ";//Sql Server的Select支持子查询，Value方式不支持
                    strInsertOracleEnd = ";";//因为使用select方式，所以最后只加分号即可。
                }

                #region 新增数据
                int iDataNum = 1;//数据计数器
                int iCommitCount = Convert.ToInt32(strDataStyle);
                int iDataRowCount = dtData.Rows.Count;
                foreach (DataRow drColData in dtData.Rows)//导入数据表内循环
                {
                    string strColInsertSql_Replace = strColInsertSql;
                    foreach (DataRow drCol in dtSec.Rows)//针对列清单循环来替换参数
                    {
                        string strColCode = drCol["COLUMN_NAME"].ToString().Trim().ToUpper();
                        string strColHelpQueryValue = drCol["辅助查询值"].ToString().Trim(); //辅助查询值
                        if (!string.IsNullOrEmpty(strColHelpQueryValue))//辅助查询值
                        {
                            strColHelpQueryValue = strColHelpQueryValue.Replace("#" + strColCode + "#", drColData[strColCode].ToString().Trim().Replace("'", ""));
                            DataTable dtHelpData = _dataAccess.DataAccess.QueryHadParamSqlData(strColHelpQueryValue, _dicQueryCondition);
                            if (dtHelpData.Rows.Count > 0)
                            {
                                strColInsertSql_Replace = strColInsertSql_Replace.Replace("#辅助查询值#", dtHelpData.Rows[0][0].ToString());
                            }
                        }
                        else if (dtData.Columns.Contains(strColCode))//非辅助查询值
                        {
                            if (drColData[strColCode] == DBNull.Value)//null
                            {
                                strColInsertSql_Replace = strColInsertSql_Replace.Replace("'#" + strColCode + "#'", "null");
                            }
                            else
                            {
                                if (dtData.Columns[strColCode].DataType == typeof(DateTime) && selectDBType == DataBaseType.Oracle)
                                {
                                    //oracle要将时间字符转为日期
                                    strColInsertSql_Replace = strColInsertSql_Replace.Replace("'#" + strColCode + "#'", "TO_DATE('" + drColData[strColCode].ToString() + "','yyyy-MM-dd HH24:mi:ss')");
                                }
                                else if (drColData[strColCode].ToString().Trim().Contains("&") && selectDBType == DataBaseType.Oracle)
                                {
                                    //oralce的&为输入参数转义字符，需要替换为chr(38)
                                    strColInsertSql_Replace = strColInsertSql_Replace.Replace("#" + strColCode + "#", drColData[strColCode].ToString().Trim().Replace("&", "' || chr(38) || '"));
                                }
                                else
                                {
                                    //将参数化的字段以实际数据代替
                                    strColInsertSql_Replace = strColInsertSql_Replace.Replace("#" + strColCode + "#", drColData[strColCode].ToString().Trim().Replace("'", ""));
                                }
                            }
                        }
                    }
                    string strSQLOneData = strInsertPre + strColInsertSql_Replace.Substring(0, strColInsertSql_Replace.Length - 1);
                    //生成单个数据SQL
                    strSQLOneData = DataBaseCommon.GenOneDataSql(selectDBType, strDataStyle, strCommit, strInsertOracleEnd, strSQLOneData, iDataNum, iCommitCount, iDataRowCount);
                    sbAllSql.Append(strSQLOneData);
                    iDataNum++;
                }
                #endregion

                iTable++;
                if (_dbServer.DatabaseType == DataBaseType.SqlServer && selectDBType== DataBaseType.SqlServer && ckbMainKeyInsert.Checked)
                {
                    sbAllSql.Append("SET IDENTITY_INSERT " + strDataTableName + " OFF\n");
                }
            }
            //保存属性
            //PropSetting.Default.Save();

            //返回值和显示处理
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
            rtbResult.Select(0, 0); //返回到第一行
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

        #region 目标数据库类型选择变化
        private void cbbTargetDbType_SelectedIndexChanged(object sender, EventArgs e)
        {
            //目标数据库类型
            int iDbType = int.Parse(cbbTargetDbType.SelectedValue.ToString());
            DataBaseType selectDBType = (DataBaseType)iDbType;
            ckbMainKeyInsert.Visible = false;
            _dbServer = uC_DbConnection1.GetDbServerInfo();
            switch (_dbServer.DatabaseType)
            {
                case DataBaseType.SqlServer:
                    if (selectDBType == DataBaseType.SqlServer)
                    {
                        ckbMainKeyInsert.Visible = true;
                    }
                    break;
                case DataBaseType.Oracle:
                    break;
                case DataBaseType.MySql:
                    break;
                case DataBaseType.SQLite:
                    break;
                case DataBaseType.PostgreSql:
                    break;
                default:
                    throw new Exception("暂不支持该数据库类型！");
                    //break;
            }
            //调用表名选择变化事件
            cbbTableName_SelectedIndexChanged(null,null);
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
                UIHelper.BindDropDownList(cbbTableName, uC_DbConnection1.UserTableList.Sort("TABLE_NAME"), "TABLE_NAME", "TABLE_NAME",false);
            }
            else
            {
                cbbTableName.DataSource = null;
            }
        }
        #endregion

        #region 表名下拉框变化及回车键事件
        private void cbbTableName_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ckbAutoGetSql.Checked && !string.IsNullOrEmpty(cbbTableName.Text.Trim()))
            {
                tsbImport.PerformClick();
                tsbAutoSQL.PerformClick();
            }
        }

        private void cbbTableName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && !string.IsNullOrEmpty(cbbTableName.Text.Trim()))
            {
                tsbImport.PerformClick();
                tsbAutoSQL.PerformClick();
            }
        }
        #endregion

        /// <summary>
        /// 导出按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsbExport_Click(object sender, EventArgs e)
        {
            string strWhere = string.IsNullOrEmpty(rtbWhere.Text.Trim()) == true ? "" : " WHERE " + rtbWhere.Text.Trim();
            string strDataTableName = cbbTableName.Text.Trim();
            DataTable dtCount = _dataAccess.DataAccess.QueryHadParamSqlData("SELECT * FROM " + strDataTableName + strWhere, _dicQueryCondition);
            if (dtCount.Rows[0][0].ToString() == "0")
            {
                MsgHelper.ShowErr("没有要生成的记录！", "提示");
                return;
            }
            //导出Excel
            ExportHelper.ExportExcel(dtCount, "数据_" + strDataTableName);
        }
    }
}
