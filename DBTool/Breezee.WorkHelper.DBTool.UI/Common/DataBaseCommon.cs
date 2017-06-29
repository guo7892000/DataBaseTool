using Breezee.Framework.Tool;
using Breezee.Global.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Breezee.WorkHelper.DBTool.UI
{
    public class DataBaseCommon
    {
        private readonly string _strNull = " NULL";   //可空
        private readonly string _strNotNull = " NOT NULL"; //非空
        private string _strImportSuccessDealType = "0";//导入成功的功能处理类型
        private static string strTableAlias = "A"; //查询和修改中的表别名
        private static string strTableAliasAndDot = "";
        private static readonly string _strBlank = " "; //空格
        private static readonly string _strTab = "\t"; //制表键
        private static readonly string _strQuotationMark = "'";
        private static readonly string strEnpty = ""; //空白
        private static readonly string _strComma = ",";

        public static AutoClassInfo ClassInfo = new AutoClassInfo();


        #region 生成单条数据SQL
        /// <summary>
        /// 生成单条数据SQL
        /// </summary>
        /// <param name="dataBaseType">导入类型</param>
        /// <param name="strDataStyle">提交方式</param>
        /// <param name="strCommit">提交字符</param>
        /// <param name="strUpdateOracleEnd">Orcale结束的字符串</param>
        /// <param name="strUpdateSQLOne">返回的字符</param>
        /// <param name="iDataNum">第几条数据</param>
        /// <param name="iCommitCount">第几次要加提交语句</param>
        /// <param name="iDataRowCount">记录数</param>
        /// <returns></returns>
        public static string GenOneDataSql(DataBaseType dataBaseType, string strDataStyle, string strCommit, string strUpdateOracleEnd, string strUpdateSQLOne, int iDataNum, int iCommitCount, int iDataRowCount)
        {
            if (dataBaseType == DataBaseType.SqlServer)
            {
                if (strDataStyle != "0" && strDataStyle != "1")//非每行提交或最后一行提交
                {
                    if (iDataNum % iCommitCount == 0 || iDataNum == iDataRowCount)//每多少行或最后一行加上提交
                    {
                        strUpdateSQLOne = strUpdateSQLOne + "\n" + strCommit;
                    }
                    else
                    {
                        strUpdateSQLOne = strUpdateSQLOne + "\n";
                    }
                }
                else if (strDataStyle == "1")//每次提交
                {
                    strUpdateSQLOne = strUpdateSQLOne + "\n" + strCommit;
                }
                else
                {
                    strUpdateSQLOne = strUpdateSQLOne + "\n";
                }

                
            }
            else
            {
                if (strDataStyle != "0" && strDataStyle != "1")//非每行提交或最后一行提交
                {
                    if (iDataNum % iCommitCount == 0 || iDataNum == iDataRowCount)//每多少行或最后一行加上提交
                    {
                        strUpdateSQLOne = strUpdateSQLOne + strUpdateOracleEnd + "\n" + strCommit;
                    }
                    else
                    {
                        strUpdateSQLOne = strUpdateSQLOne + strUpdateOracleEnd + "\n";
                    }
                }
                else if (strDataStyle == "1")//每次提交
                {
                    strUpdateSQLOne = strUpdateSQLOne + strUpdateOracleEnd + "\n" + strCommit;
                }
                else
                {
                    strUpdateSQLOne = strUpdateSQLOne + strUpdateOracleEnd + "\n";
                }

            }
            return strUpdateSQLOne;
        }
        #endregion

        #region 获取列的默认注释
        /// <summary>
        /// 获取列的默认注释：只有列说明为空时才获取
        /// </summary>
        /// <param name="strColCode">列编码</param>
        /// <param name="strColComments">列目前的注释</param>
        /// <returns></returns>
        public static string GetColumnComment(string strColCode, string strColComments)
        {
            if (string.IsNullOrEmpty(strColComments))
            {
                switch (strColCode)
                {
                    case "IS_SYSTEM":
                        strColComments = "是否系统";
                        break;
                    case "ORDER_NO":
                        strColComments = "排序号";
                        break;
                    case "CREATOR":
                        strColComments = "创建人";
                        break;
                    case "REMARK":
                        strColComments = "备注";
                        break;
                    case "PART_BRAND_CODE":
                        strColComments = "备件品牌";
                        break;
                    case "CREATED_DATE":
                        strColComments = "创建时间";
                        break;
                    case "MODIFIER":
                        strColComments = "修改人";
                        break;
                    case "LAST_UPDATED_DATE":
                        strColComments = "最后更新时间";
                        break;
                    case "IS_ENABLE":
                        strColComments = "是否启用";
                        break;
                    case "SDP_USER_ID":
                        strColComments = "SDP用户ID";
                        break;
                    case "SDP_ORG_ID":
                        strColComments = "SDP组织ID";
                        break;
                    case "UPDATE_CONTROL_ID":
                        strColComments = "并发控制ID";
                        break;
                    default:
                        break;
                }
            }
            return strColComments;
        }
        #endregion

        #region 生成增删改查SQL方法
        /// <summary>
        /// 设置表说明
        /// </summary>
        /// <param name="strTableCode"></param>
        /// <param name="strColComments"></param>
        /// <returns></returns>
        public static string MakeTableComment(string strTableCode, string strColComments)
        {
            return AddLeftBand(strTableCode) + _strTab + "/*" + strColComments + "*/\n";
        }

        /// <summary>
        /// 设置查询列说明
        /// </summary>
        /// <param name="strComma">为逗号或空</param>
        /// <param name="strColCode">列编码</param>
        /// <param name="strColComments">列说明</param>
        /// <returns></returns>
        public static string MakeQueryColumnComment(string strComma, string strColCode, string strColComments)
        {
            if (strColCode.EndsWith(".IS_ENABLE"))
            {
                //增加一个自定义状态列
                return _strTab + strColCode + strComma + _strTab + "/*" + strColComments + "*/\n"
                    + _strTab + " DECODE(" + strTableAliasAndDot + "IS_ENABLE, '1', '启用', '停用') IS_ENABLE_NAME" + strComma + _strTab + "/*" + strColComments + "*/\n";
            }
            return _strTab + strColCode + strComma + _strTab + "/*" + strColComments + "*/\n";
        }

        /// <summary>
        /// 设置新增列值说明方法
        /// </summary>
        /// <param name="strComma">为逗号或空</param>
        /// <param name="strColCode">列编码</param>
        /// <param name="strColValue">列值</param>
        /// <param name="strColComments">列说明</param>
        /// <param name="strColType">列数据类型</param>
        /// <returns></returns>
        public static string MakeAddValueColumnComment(string strComma, string strColCode, string strColValue, string strColComments, string strColType, bool isParmQuery)
        {
            string strColRemark = "/*" + strColCode + ":" + strColComments + "*/\n";
            string strFixedValue = "#" + strColCode + "#";
            if (isParmQuery)
            {
                strFixedValue = "@" + strColCode;
            }
            string strColRelValue = "";
            if (string.IsNullOrEmpty(strColValue))
            {
                //列没有默认值则加引号
                //列值为空时
                if (strColType == "DATE")
                {
                    //如果是Oracle的时间类型DATE，则需要将字符转为时间格式，要不会报“文字与字符串格式不匹配”错误
                    strColRelValue = "TO_DATE(" + _strQuotationMark + strFixedValue + _strQuotationMark + ",'YYYY-MM-DD')";
                    if (isParmQuery)
                    {
                        strColRelValue = "TO_DATE(" + strFixedValue + ",'YYYY-MM-DD')";
                    }
                }
                else
                {
                    strColRelValue = _strQuotationMark + strFixedValue + _strQuotationMark;
                    if (isParmQuery)
                    {
                        strColRelValue = strFixedValue;
                    }
                }
            }
            else
            {
                //列有默认值则不加引号
                strColRelValue = strColValue;
            }
            return _strTab + strColRelValue + strComma + _strTab + strColRemark;
        }

        /// <summary>
        /// 设置修改列值说明方法
        /// </summary>
        /// <param name="strComma">为逗号或空</param>
        /// <param name="strColCode">列编码</param>
        /// <param name="strColValue">列值</param>
        /// <param name="strColComments">列说明</param>
        /// <param name="strColType">列数据类型</param>
        /// <returns></returns>
        public static string MakeUpdateColumnComment(string _strComma, string strColCode, string strColValue, string strColComments, string strColType, bool isParmQuery)
        {
            string strRemark = "\n";
            if (!string.IsNullOrEmpty(strColComments))
            {
                //修改列只显示备注，不显示列名
                strRemark = "/*" + strColComments + "*/\n";
            }
            string strColRelValue = "";
            if (string.IsNullOrEmpty(strColValue))
            {
                //列值为空时
                if (strColType == "DATE")
                {
                    //如果是Oracle的时间类型DATE，则需要将字符转为时间格式，要不会报“文字与字符串格式不匹配”错误
                    strColRelValue = "TO_DATE(" + _strQuotationMark + "#" + strColCode + "#" + _strQuotationMark + ",'YYYY-MM-DD')";
                    if (isParmQuery)
                    {
                        strColRelValue = "TO_DATE(@" + strColCode + ",'YYYY-MM-DD')";
                    }
                }
                else
                {
                    strColRelValue = _strQuotationMark + "#" + strColCode + "#" + _strQuotationMark;
                    if (isParmQuery)
                    {
                        strColRelValue = "@" + strColCode;
                    }
                }
            }
            else
            {
                //列值非空时
                if (isParmQuery)
                {
                    strColRelValue = strColValue;
                    if (!strColValue.Contains("@"))
                    {
                        strColRelValue = strColValue.Replace("#", "");
                    }
                }
                else
                {
                    strColRelValue = strColValue;
                }
            }
            return strTableAliasAndDot + strColCode + "=" + strColRelValue + _strComma + _strTab + strRemark;
        }

        /// <summary>
        /// 设置条件列说明
        /// </summary>
        /// <param name="strColCode">列编码</param>
        /// <param name="strColValue">列值</param>
        /// <param name="strColComments">列说明</param>
        /// <returns></returns>
        public static string MakeConditionColumnComment(string strColCode, string strColValue, string strColComments, bool isParmQuery)
        {
            string strRemark = "\n";
            if (!string.IsNullOrEmpty(strColComments))
            {
                strRemark = "/*" + strColComments + "*/\n";
            }
            if (string.IsNullOrEmpty(strColValue))
            {
                if (isParmQuery)
                {
                    return strTableAliasAndDot + strColCode + " = @" + strColCode + _strTab + strRemark;
                }
                //列值为空时，设置为：'#列编码#'
                return strTableAliasAndDot + strColCode + "=" + _strQuotationMark + "#" + strColCode + "#" + _strQuotationMark + _strTab + strRemark;
            }
            else
            {
                //有固定值时
                return strTableAliasAndDot + strColCode + "=" + strColValue + _strTab + strRemark;
            }
        }

        /// <summary>
        /// 获取表列的说明字符串
        /// </summary>
        /// <param name="strColCode"></param>
        /// <param name="strColComments"></param>
        /// <returns></returns>
        public static string GetTableColumnComment(string strColCode, string strColComments)
        {
            return "/// <column name=\"" + strColCode + "\">" + strColComments + "</column>\n";
        }

        /// <summary>
        /// 获取文件中字典参数说明字符串
        /// </summary>
        /// <param name="strColCode"></param>
        /// <param name="strColComments"></param>
        /// <returns></returns>
        public static string GetFileDictionaryParamComment(string strColCode, string strColComments, bool isParmQuery)
        {
            if (isParmQuery)
            {
                return strColCode.Replace("#", "") + "：" + strColComments + "\n";
            }
            else
            {
                return "#" + strColCode.Replace("#", "") + "#：" + strColComments + "\n";
            }
        }

        /// <summary>
        /// 获取开发指导书中的列说明字符串
        /// </summary>
        /// <param name="strColCode"></param>
        /// <param name="strColComments"></param>
        /// <returns></returns>
        public static string GetFileColumnComment(string strColCode, string strColComments)
        {
            return strColCode + "：" + strColComments + "\n";
        }

        /// <summary>
        /// 获取方法字典参数的说明字符串
        /// </summary>
        /// <param name="strColCode"></param>
        /// <param name="strColComments"></param>
        /// <returns></returns>
        public static string GetFuncParamComment(string strColCode, string strColComments, bool isParmQuery, bool isPK = false)
        {
            if (isPK)
            {
                if (isParmQuery)
                {
                    return "/// <param type=\"string\" name=\"" + strColCode.Replace("#", "") + "\">" + strColComments + "：新增时为空，修改时为必填</param>\n";
                }
                else
                {
                    return "/// <param type=\"string\" name=\"#" + strColCode.Replace("#", "") + "#\">" + strColComments + "：新增时为空，修改时为必填</param>\n";
                }
            }
            else
            {
                if (isParmQuery)
                {
                    return "/// <param type=\"string\" name=\"" + strColCode.Replace("#", "") + "\">" + strColComments + "</param>\n";
                }
                else
                {
                    return "/// <param type=\"string\" name=\"#" + strColCode.Replace("#", "") + "#\">" + strColComments + "</param>\n";
                }
            }
        }

        /// <summary>
        /// 获取字典设置值的字符串
        /// </summary>
        /// <param name="strColumnCode"></param>
        /// <returns></returns>
        public static string GetDicSetValueString(string strColumnCode, bool isParmQuery)
        {
            if (isParmQuery)
            {
                return "dicParam[\"" + strColumnCode.Replace("#", "") + "\"] = dr[\"" + strColumnCode.Replace("#", "") + "\"].ToString();\n";
            }
            else
            {
                return "dicParam[\"#" + strColumnCode.Replace("#", "") + "#\"] = dr[\"" + strColumnCode.Replace("#", "") + "\"].ToString();\n";
            }
        }

        /// <summary>
        /// 获取字典中表列注释的字符串
        /// </summary>
        /// <param name="strColumnCode"></param>
        /// <returns></returns>
        public static string GetDicTableColumnRemarkString(string strColumnCode, string strColComments)
        {
            return "/// <column name=\"" + strColumnCode.Replace("#", "") + "\">" + strColComments + "</column>\n";
        }
        #endregion 

        #region MDS方法注释方法
        /// <summary>
        /// 获取数据访问层保存前缀
        /// </summary>
        /// <param name="isDataAccess">是否数据访问：true是，false否</param>
        /// <returns></returns>
        public static string GetFuncParmSavePreString(bool isDataAccess, bool isParmQuery)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("/// <summary>\n");
            sb.Append("/// 保存" + ClassInfo.ClassName + "\n");
            sb.Append("/// </summary>\n");
            sb.Append("/// <remark>\n");
            if (isDataAccess)
            {
                sb.Append(GetSaveDataAcceRemark());
            }
            else
            {
                sb.Append(GetSaveBusinessRemark());
            }
            sb.Append("/// </remark>\n");
            sb.Append("/// <author>" + ClassInfo.Author + "</author>\n");
            if (isDataAccess)
            {
                sb.Append("/// <param type=\"DbTransaction\" name=\"tran\">事务</param>\n");
            }
            string strOperate_type = "#OPERATE_TYPE#";
            if (isParmQuery)
            {
                strOperate_type = "OPERATE_TYPE";
            }
            sb.Append("/// <param type=\"string\" name=\"" + strOperate_type + "\">操作类型：ADD新增，MODIFY修改</param>\n");
            return sb.ToString();
        }

        /// <summary>
        /// 获取保存结束字符
        /// </summary>
        /// <param name="isDataAccess"></param>
        /// <param name="st"></param>
        /// <param name="isParmQuery"></param>
        /// <returns></returns>
        public static string GetFuncParmSaveEndString(bool isDataAccess, bool isParmQuery, string strSql)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("/// <returns>\n");
            if (isParmQuery)
            {
                sb.Append("/// <content type=\"string\" name=\"FLAG\">执行标志0代表失败，1代表成功</content>\n");
                sb.Append("/// <content type=\"string\" name=\"USER_MSG\">返回消息内容</content>\n");
            }
            else
            {
                sb.Append("/// <content type=\"string\" name=\"#FLAG#\">执行标志0代表失败，1代表成功</content>\n");
                sb.Append("/// <content type=\"string\" name=\"#USER_MSG#\">返回消息内容</content>\n");
            }
            sb.Append("/// </returns>\n");
            if (isDataAccess)
            {
                #region 数据访问层
                sb.Append("public IDictionary<string, object> Save" + ClassInfo.ClassCode + "(IDictionary<string, object> dicParam,DbTransaction tran)\n");
                sb.Append("{\n");
                if (!isParmQuery)
                {
                    #region 非参数化查询方式
                    sb.Append("    string strSQL = \"\";\n");
                    sb.Append("    IDictionary<string, string> dicIn = ObjectExtentions.ToStringDict(dicParam); //字符字典\n");
                    sb.Append("    //校验" + ClassInfo.ClassName + "是否已存在\n");
                    sb.Append("    int iRow = DBHelper.QueryDataFromConfig(strSqlPre + \"查询" + ClassInfo.ClassName + "是否存在\", dicParam).Rows.Count;\n");
                    sb.Append("    if (iRow > 0)\n");
                    sb.Append("    {\n");
                    sb.Append("        throw new DataAccessException(\"输入的" + ClassInfo.ClassName + "已存在，请重新录入！\"); ;\n");
                    sb.Append("    }\n");
                    sb.Append("\n");
                    sb.Append("    if (dicParam[OperationType.操作Key值定义].ToString() == OperationType.新增) //新增\n");
                    sb.Append("    {\n");
                    sb.Append("        DBHelper.UpdateDataInfo(strSqlPre + \"新增" + ClassInfo.ClassName + "\", dicParam, tran);\n");
                    sb.Append("    }\n");
                    sb.Append("    else if (dicParam[OperationType.操作Key值定义].ToString() == OperationType.修改) //修改\n");
                    sb.Append("    {\n");
                    sb.Append("        iRow = DBHelper.UpdateDataInfo(strSqlPre + \"修改" + ClassInfo.ClassName + "\", dicParam, tran);\n");
                    sb.Append("        if (iRow <= 0)\n");
                    sb.Append("        {\n");
                    sb.Append("            throw new ConcurrencyException();//并发判断\n");
                    sb.Append("        }\n");
                    sb.Append("    }\n");
                    sb.Append("    else\n");
                    sb.Append("    {\n");
                    sb.Append("        throw new DataAccessException(\"传入参数错误，没有定义该操作类型！\");\n");
                    sb.Append("     }\n");
                    sb.Append("\n");
                    #endregion
                }
                else
                {
                    #region 参数化查询方式
                    sb.Append(strSql);
                    sb.Append("    if (iEff == 0)\n");
                    sb.Append("    {\n");
                    sb.Append("         throw new ConcurrencyException();\n");
                    sb.Append("    }\n");
                    #endregion
                }
                sb.Append("    return ExecutionResult.Success();\n");
                sb.Append("}\n");
                #endregion
            }
            else
            {
                sb.Append("public IDictionary<string, object> Save" + ClassInfo.ClassCode + "(IDictionary<string, object> dicParam)\n");
                sb.Append("{\n");
                sb.Append("    DbTransaction tran = null;\n");
                sb.Append("    IDictionary<string, object> dicRet = ExecutionResult.Success();\n");
                sb.Append("    D" + ClassInfo.ClassCode + " acce = new D" + ClassInfo.ClassCode + "();\n");
                sb.Append("    try\n");
                sb.Append("    {\n");
                sb.Append("        //开启事务\n");
                sb.Append("        tran = Gateway.Default.BeginTransaction();\n");
                sb.Append("        //保存" + ClassInfo.ClassName + "\n");
                sb.Append("        dicRet = acce.Save" + ClassInfo.ClassCode + "(tran, dicParam);\n");
                //sb.Append("        //TO_DO：写日志，暂未实现\n");
                sb.Append("        //提交事务\n");
                sb.Append("        tran.Commit();\n");
                sb.Append("    }\n");
                sb.Append("    catch (Exception ex)\n");
                sb.Append("    {\n");
                sb.Append("         if (tran != null)\n");
                sb.Append("         {\n");
                sb.Append("             tran.Rollback();\n");
                sb.Append("         }\n");
                sb.Append("        dicRet = ExecutionResult.FailException(ex);\n");
                sb.Append("    }\n");
                sb.Append("    finally\n");
                sb.Append("    {\n");
                sb.Append("         if (tran != null)\n");
                sb.Append("         {\n");
                sb.Append("             tran.Dispose();\n");
                sb.Append("         }\n");
                sb.Append("    }\n");
                sb.Append("    return dicRet;\n");
                sb.Append("}\n");
            }
            return sb.ToString();
        }

        public static string GetFuncParmQueryPreString(bool isDataAccess, bool isParmQuery)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("/// <summary>\n");
            sb.Append("/// 查询" + ClassInfo.ClassName + "\n");
            sb.Append("/// </summary>\n");
            sb.Append("/// <remark>\n");
            if (isDataAccess)
            {
                sb.Append(GetQueryDataAcceRemark());
            }
            else
            {
                sb.Append(GetQueryBusinessRemark());
            }
            sb.Append("/// </remark>\n");
            sb.Append("/// <author>" + ClassInfo.Author + "</author>\n");
            return sb.ToString();
        }

        public static string GetFuncParmQueryMiddleString(bool isParmQuery)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("/// <returns>\n");
            if (!isParmQuery)
            {
                sb.Append("/// <content type=\"DataTable\" name=\"#" + ClassInfo.ClassCode + "#\">" + ClassInfo.ClassCode + "\n");
            }
            else
            {
                sb.Append("/// <content type=\"DataTable\" name=\"" + ClassInfo.ClassCode + "\">" + ClassInfo.ClassCode + "\n");
            }
            return sb.ToString();
        }

        /// <summary>
        /// 获取查询结束字符
        /// </summary>
        /// <param name="isDataAccess"></param>
        /// <param name="isParmQuery"></param>
        /// <returns></returns>
        public static string GetFuncParmQueryEndString(bool isDataAccess, bool isParmQuery, string strSql)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("/// </content>\n");
            sb.Append("/// </returns>\n");
            if (isDataAccess)
            {
                sb.Append("public DataTable Query" + ClassInfo.ClassCode + "(IDictionary<string, string> dicParam)\n");
                sb.Append("{\n");
                if (!isParmQuery)
                {
                    sb.Append("    return DBHelper.QueryDataFromConfig(strSqlPre + \"查询" + ClassInfo.ClassName + "\", dicParam);\n");
                }
                else
                {
                    sb.Append(strSql);
                }
                sb.Append("}\n");
            }
            else
            {
                sb.Append("public IDictionary<string, object> Query" + ClassInfo.ClassCode + "(IDictionary<string, string> dicParam)\n");
                sb.Append("{\n");
                sb.Append("    IDictionary<string, object> dicRet = ExecutionResult.QuerySuccess();\n");
                sb.Append("    try\n");
                sb.Append("    {\n");
                sb.Append("         D" + ClassInfo.ClassCode + " acce = new D" + ClassInfo.ClassCode + "();\n");
                sb.Append("         dicRet[StaticConstant.DT_RESULT] = acce.Query" + ClassInfo.ClassName + "(dicParam);\n");
                sb.Append("     }\n");
                sb.Append("     catch (Exception ex)\n");
                sb.Append("     {\n");
                sb.Append("         dicRet = ExecutionResult.FailException(ex);\n");
                sb.Append("     }\n");
                sb.Append("     return dicRet;\n");
                sb.Append("}\n");
            }
            return sb.ToString();
        }

        public static string GetQueryDataAcceRemark()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("/// 1、传入查询条件、查询SQL路径，执行查询方法返回查询结果。\n");
            return sb.ToString();
        }

        public static string GetQueryBusinessRemark()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("/// 1、新增" + ClassInfo.ClassName + "业务对象\n");
            sb.Append("/// 2、执行" + ClassInfo.ClassName + "业务对象的查询方法\n");
            sb.Append("/// 3、返回查询结果\n");
            return sb.ToString();
        }

        /// <summary>
        /// 获取保存数据访问层的方法
        /// </summary>
        /// <returns></returns>
        public static string GetSaveDataAcceRemark()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("/// 1、查询" + ClassInfo.ClassName + "是否存在:\n");
            sb.Append("///    存在则抛出错误提示，不存在则继续。\n");
            sb.Append("/// 2、根据不同操作类型，调用不同的SQL：\n");
            sb.Append("///    新增：执行新增SQL\n");
            sb.Append("///    修改：执行修改SQL\n");
            sb.Append("/// 3、最后返回成功信息。\n");
            return sb.ToString();
        }

        /// <summary>
        /// 获取保存业务实现层的方法
        /// </summary>
        /// <returns></returns>
        public static string GetSaveBusinessRemark()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("/// 1、开始事务\n");
            sb.Append("/// 2、新建" + ClassInfo.ClassName + "业务对象\n");
            sb.Append("/// 3、传入参数，执行" + ClassInfo.ClassName + "业务对象的保存方法\n");
            sb.Append("/// 4、如果成功，则提交事务。如果失败，则记录错误信息，跳至5步骤\n");
            sb.Append("/// 5、返回结果：成功则返回成功信息，失败会返回错误信息。\n");
            return sb.ToString();
        }

        #endregion

        #region 增加左边空格方法
        public static string AddLeftBand(string strColCode)
        {
            return _strBlank + strColCode;
        }
        #endregion

        #region 增加右边空格方法
        public static string AddRightBand(string strColCode)
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
        public static string AddLeftRightBand(string strColCode)
        {
            return _strBlank + strColCode + _strBlank;
        }
        #endregion

        #region 增加左右括号方法
        public static string AddLeftRightKuoHao(string strColCode)
        {
            return "(" + strColCode + ")";
        }
        #endregion

        #region 增加左右单引号方法
        public static string ChangeIntoSqlString(string strColCode)
        {
            return " '" + strColCode + "' ";
        }
        #endregion
    }

    public enum AutoSqlColumnSetType
    {
        /// <summary>
        /// 默认值设置
        /// </summary>
        Default = 1,

        /// <summary>
        /// 排除的字段
        /// </summary>
        Exclude = 0
    }

    public class AutoClassInfo
    {
        public string Author = string.Empty;
        public string ClassCode = string.Empty;
        public string ClassName = string.Empty;

    }
}
