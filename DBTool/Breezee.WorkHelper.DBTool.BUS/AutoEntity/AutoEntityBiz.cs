using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using Breezee.WorkHelper.DBTool.Entity;
using Breezee.Global.Entity;
using Breezee.Framework.DataAccess.INF;
using Breezee.Framework.DataAccess.Custom;

namespace Breezee.WorkHelper.DBTool.BUS
{
    /// <summary>
    /// 生成实体类
    /// </summary>
    public class AutoEntityBiz
    {
        private ICustomDataAccess CustomDataAccess;
        public AutoEntityBiz(DataBaseType dbt,DbServerInfo server)
        {
            CustomDataAccess = new CustomDataAccess(dbt, server);
        }

        #region 生成简单实体
        public List<EntityInfo> BuildEntity(DataSet dataSet, string nameSpace, string accessType, string classPre, string classEnd, string path)
        {
            List<EntityInfo> list = null;
            if (dataSet.Tables.Count > 0)
            {
                list = new List<EntityInfo>();
                for (int i = 0; i < dataSet.Tables.Count; i++)
                {
                    DataTable table = dataSet.Tables[i];
                    if (table.Rows.Count > 0)
                    {
                        string str = table.Rows[0][2].ToString();
                        using (TextWriter writer = new StreamWriter(new BufferedStream(new FileStream(path + @"\" + this.FCharToUpper(str) + classPre + ".cs", FileMode.Create, FileAccess.Write)), Encoding.GetEncoding("gbk")))
                        {
                            writer.WriteLine("using System;");
                            writer.WriteLine("using System.Collections.Generic;");
                            writer.WriteLine("using System.Text;");
                            writer.WriteLine();
                            writer.WriteLine("namespace " + nameSpace);
                            writer.WriteLine("{");
                            string entityName = classPre + this.FCharToUpper(str) + classEnd ;
                            writer.WriteLine("\t[Serializable]");
                            writer.WriteLine("\tpublic partial class " + entityName + " : IBaseEntity");//让其实现实体基类
                            writer.WriteLine("\t{");
                            writer.WriteLine();
                            writer.WriteLine("\t\t//自动生成字段");
                            string str3 = "";
                            int num2 = 0;
                            while (num2 < table.Rows.Count)
                            {
                                str3 = str3 + "," + this.ParseType(table.Rows[num2][4].ToString()) + " " + this.FCharAddDownLine(table.Rows[num2][3].ToString());
                                writer.WriteLine("\t\tprivate " + this.ParseType(table.Rows[num2][4].ToString()) + " " + this.FCharAddDownLine(table.Rows[num2][3].ToString()) + ";");
                                num2++;
                            }
                            writer.WriteLine();
                            writer.WriteLine("\t\t//自动生成默认无参构造函数");
                            writer.WriteLine("\t\tpublic " + entityName + "()");
                            writer.WriteLine("\t\t{");
                            writer.WriteLine("\t\t}");
                            writer.WriteLine();
                            string[] strArray = str3.Substring(1, str3.Length - 1).Split(new char[] { ',' });
                            writer.WriteLine("\t\t//自动生成全参参构造函数");
                            writer.WriteLine("\t\tpublic " + entityName + "(" + str3.Substring(1, str3.Length - 1) + ")");
                            writer.WriteLine("\t\t{");
                            for (int j = 0; j < strArray.Length; j++)
                            {
                                writer.WriteLine("\t\t\tthis." + strArray[j].Split(new char[] { ' ' })[1] + " = " + strArray[j].Split(new char[] { ' ' })[1] + ";");
                            }
                            writer.WriteLine("\t\t}");
                            writer.WriteLine();
                            writer.WriteLine("\t\t//自动生成属性");
                            for (num2 = 0; num2 < table.Rows.Count; num2++)
                            {
                                string str4 = "public";
                                if (accessType.Trim().Equals("internal"))
                                {
                                    str4 = "internal";
                                }
                                if (accessType.Trim().Equals("protected"))
                                {
                                    str4 = "protected";
                                }
                                string str5 = this.FCharToUpper(table.Rows[num2][3].ToString());
                                writer.WriteLine("\t\t" + str4 + " " + this.ParseType(table.Rows[num2][4].ToString()) + " " + this.FCharToUpper(str5));
                                writer.WriteLine("\t\t{");
                                writer.WriteLine("\t\t\tget { return " + FCharAddDownLine(str5) + ";}");
                                writer.WriteLine("\t\t\tset { " + FCharAddDownLine(str5) + " = value;}");
                                writer.WriteLine("\t\t}");
                                writer.WriteLine();
                            }
                            writer.WriteLine("\t}");
                            writer.WriteLine("}");
                            list.Add(new EntityInfo(nameSpace, "public", entityName, path + @"\" + entityName + ".cs"));
                        }
                    }
                }
            }
            return list;
        } 
        #endregion

        #region 生成表列名字符实体
        public List<EntityInfo> BuildTableColumnNameEntity(DataSet dataSet, string nameSpace, string accessType, string classPre, string classEnd, string path)
        {
            List<EntityInfo> list = null;
            if (dataSet.Tables.Count > 0)
            {
                list = new List<EntityInfo>();
                for (int i = 0; i < dataSet.Tables.Count; i++)
                {
                    DataTable table = dataSet.Tables[i];
                    if (table.Rows.Count > 0)
                    {
                        string str = table.Rows[0][2].ToString();
                        using (TextWriter writer = new StreamWriter(new BufferedStream(new FileStream(path + @"\" + classPre + str + classEnd + ".cs", FileMode.Create, FileAccess.Write)), Encoding.GetEncoding("gbk")))
                        {
                            writer.WriteLine("using System;");
                            writer.WriteLine("using System.Collections.Generic;");
                            writer.WriteLine("using System.Text;");
                            writer.WriteLine();
                            writer.WriteLine("namespace " + nameSpace);
                            writer.WriteLine("{");
                            string entityName = classPre + str.ToUpper() + classEnd;
                            writer.WriteLine("\t[Serializable]");
                            writer.WriteLine("\tpublic partial class " + entityName + " : IBaseEntity");//让其实现实体基类
                            writer.WriteLine("\t{");
                            writer.WriteLine();
                            writer.WriteLine("\t\t//自动生成表名");
                            writer.WriteLine(string.Format("\t\tpublic static string TName = \"{0}\"; ////本身实例", str.ToUpper())); //使用静态常量
                            writer.WriteLine("\t\tpublic string DBTableName { get{return \"" + str.ToUpper() + "\";}} ////实现接口"); //使用属性
                            writer.WriteLine("\t\tpublic static IBaseEntity NewEntity(){ return new " + entityName + "();} ////本身实例");
                            writer.WriteLine("\t\t//自动生成字段");
                            string str3 = "";
                            int num2 = 0;
                            while (num2 < table.Rows.Count)
                            {
                                str3 = str3 + "," + this.ParseType(table.Rows[num2][4].ToString()) + " " + this.FCharAddDownLine(table.Rows[num2][3].ToString());
                                writer.WriteLine(string.Format("\t\tpublic static string {0} = \"{0}\";", table.Rows[num2][3].ToString().ToUpper()));
                                num2++;
                            }
                            writer.WriteLine();
                            #region 已取消
                            //writer.WriteLine("\t\t//自动生成默认无参构造函数");
                            //writer.WriteLine("\t\tpublic " + entityName + "()");
                            //writer.WriteLine("\t\t{");
                            //writer.WriteLine("\t\t}");
                            //writer.WriteLine();
                            //string[] strArray = str3.Substring(1, str3.Length - 1).Split(new char[] { ',' });
                            //writer.WriteLine("\t\t//自动生成全参参构造函数");
                            //writer.WriteLine("\t\tpublic " + entityName + "(" + str3.Substring(1, str3.Length - 1) + ")");
                            //writer.WriteLine("\t\t{");
                            //for (int j = 0; j < strArray.Length; j++)
                            //{
                            //    writer.WriteLine("\t\t\tthis." + strArray[j].Split(new char[] { ' ' })[1] + " = " + strArray[j].Split(new char[] { ' ' })[1] + ";");
                            //}
                            //writer.WriteLine("\t\t}");
                            //writer.WriteLine();
                            //writer.WriteLine("\t\t//自动生成属性");
                            //for (num2 = 0; num2 < table.Rows.Count; num2++)
                            //{
                            //string str4 = "public";
                            //if (accessType.Trim().Equals("internal"))
                            //{
                            //    str4 = "internal";
                            //}
                            //if (accessType.Trim().Equals("protected"))
                            //{
                            //    str4 = "protected";
                            //}
                            //string str5 = this.FCharToUpper(table.Rows[num2][3].ToString());
                            //writer.WriteLine("\t\t" + str4 + " " + this.ParseType(table.Rows[num2][4].ToString()) + " " + this.FCharToUpper(str5));
                            //writer.WriteLine("\t\t{");
                            //writer.WriteLine("\t\t\tget { return " + FCharAddDownLine(str5) + ";}");
                            //writer.WriteLine("\t\t\tset { " + FCharAddDownLine(str5) + " = value;}");
                            //writer.WriteLine("\t\t}");
                            //    writer.WriteLine();
                            //} 
                            #endregion
                            writer.WriteLine("\t}");
                            writer.WriteLine("}");
                            list.Add(new EntityInfo(nameSpace, "public", entityName, path + @"\" + entityName + ".cs"));
                        }
                    }
                }
            }
            return list;
        } 
        #endregion

        #region 生成表列类实体
        public List<EntityInfo> BuildTableColumnClassEntity(DataSet dataSet, string nameSpace, string accessType, string classPre, string classEnd, string path)
        {
            List<EntityInfo> list = null;
            if (dataSet.Tables.Count > 0)
            {
                list = new List<EntityInfo>();
                for (int i = 0; i < dataSet.Tables.Count; i++)
                {
                    DataTable table = dataSet.Tables[i];
                    if (table.Rows.Count > 0)
                    {
                        string str = table.Rows[0][2].ToString();
                        using (TextWriter writer = new StreamWriter(new BufferedStream(new FileStream(path + @"\" + classPre + str + classEnd + ".cs", FileMode.Create, FileAccess.Write)), Encoding.GetEncoding("gbk")))
                        {
                            writer.WriteLine("using System;");
                            writer.WriteLine("using System.Collections.Generic;");
                            writer.WriteLine("using System.Text;");
                            writer.WriteLine("using Peach.Entity.StaticSimpleTable;");
                            writer.WriteLine();
                            writer.WriteLine("namespace " + nameSpace);
                            writer.WriteLine("{");
                            string entityName = classPre + str.ToUpper() + classEnd;
                            writer.WriteLine("\t[Serializable]");
                            writer.WriteLine("\tpublic partial class " + entityName + " : DbTable");
                            writer.WriteLine("\t{");
                            writer.WriteLine();
                            //writer.WriteLine("\t\t//自动生成表名");
                            //writer.WriteLine("\t\tpublic string TableName { get { return \"+str.ToUpper()+\"; } }");
                            writer.WriteLine("\t\t//自动生成字段");
                            //string strColumnList = "\tList<DbColumn> ColumnList = new List<DbColumn> {";
                            int num2 = 0;
                            while (num2 < table.Rows.Count)
                            {
                                writer.WriteLine(string.Format("\t\tpublic static DbColumn {0};", table.Rows[num2][3].ToString().ToUpper()));
                                //strColumnList = strColumnList + this.FCharAddDownLine(table.Rows[num2][3].ToString()) + ",";
                                num2++;
                            }
                            //if (table.Rows.Count > 0)
                            //{
                            //    strColumnList = strColumnList.Substring(0, strColumnList.Length - 1);
                            //}
                            //writer.WriteLine(strColumnList+"};");
                            #region 已取消
                            //writer.WriteLine("\t\t//自动生成默认无参构造函数");
                            //writer.WriteLine("\t\tpublic " + entityName + "()");
                            //writer.WriteLine("\t\t{");
                            //writer.WriteLine("\t\t}");
                            //writer.WriteLine();
                            //string[] strArray = str3.Substring(1, str3.Length - 1).Split(new char[] { ',' });
                            //writer.WriteLine("\t\t//自动生成全参参构造函数");
                            //writer.WriteLine("\t\tpublic " + entityName + "(" + str3.Substring(1, str3.Length - 1) + ")");
                            //writer.WriteLine("\t\t{");
                            //for (int j = 0; j < strArray.Length; j++)
                            //{
                            //    writer.WriteLine("\t\t\tthis." + strArray[j].Split(new char[] { ' ' })[1] + " = " + strArray[j].Split(new char[] { ' ' })[1] + ";");
                            //}
                            //writer.WriteLine("\t\t}");
                            //writer.WriteLine();
                            //writer.WriteLine("\t\t//自动生成属性");
                            //for (num2 = 0; num2 < table.Rows.Count; num2++)
                            //{
                            //string str4 = "public";
                            //if (accessType.Trim().Equals("internal"))
                            //{
                            //    str4 = "internal";
                            //}
                            //if (accessType.Trim().Equals("protected"))
                            //{
                            //    str4 = "protected";
                            //}
                            //string str5 = this.FCharToUpper(table.Rows[num2][3].ToString());
                            //writer.WriteLine("\t\t" + str4 + " " + this.ParseType(table.Rows[num2][4].ToString()) + " " + this.FCharToUpper(str5));
                            //writer.WriteLine("\t\t{");
                            //writer.WriteLine("\t\t\tget { return " + FCharAddDownLine(str5) + ";}");
                            //writer.WriteLine("\t\t\tset { " + FCharAddDownLine(str5) + " = value;}");
                            //writer.WriteLine("\t\t}");
                            //    writer.WriteLine();
                            //} 
                            #endregion
                            writer.WriteLine("\t}");
                            writer.WriteLine("}");
                            list.Add(new EntityInfo(nameSpace, "public", entityName, path + @"\" + entityName + ".cs"));
                        }
                    }
                }
            }
            return list;
        } 
        #endregion

        #region 生成自定义的实体
        public List<EntityInfo> BuildCustomEntity(DataSet dataSet, string nameSpace, string accessType, string classPre, string classEnd, string path)
        {
            List<EntityInfo> list = null;
            if (dataSet.Tables.Count > 0)
            {
                list = new List<EntityInfo>();
                for (int i = 0; i < dataSet.Tables.Count; i++)
                {
                    DataTable table = dataSet.Tables[i];
                    
                    if (table.Rows.Count > 0)
                    {
                        string str = table.Rows[0][2].ToString();
                        using (TextWriter writer = new StreamWriter(new BufferedStream(new FileStream(path + @"\" + classPre + str + classEnd + ".cs", FileMode.Create, FileAccess.Write)), Encoding.GetEncoding("gbk")))
                        {
                            writer.WriteLine("using System;");
                            writer.WriteLine("using System.Collections.Generic;");
                            writer.WriteLine("using System.Text;");
                            writer.WriteLine();
                            writer.WriteLine("namespace " + nameSpace);
                            writer.WriteLine("{");
                            string entityName = classPre + str.ToUpper() + classEnd;
                            writer.WriteLine("\t[Serializable]");
                            writer.WriteLine("\tpublic partial class " + entityName + " : IBaseEntity");
                            writer.WriteLine("\t{");
                            writer.WriteLine();
                            writer.WriteLine("\t\t//自动生成表名");
                            writer.WriteLine(string.Format("\t\tpublic static string TName = \"{0}\"; ////本身实例", str.ToUpper())); //使用静态常量
                            writer.WriteLine("\t\tpublic static IBaseEntity NewEntity(){ return new " + entityName + "();} ////本身实例");
                            writer.WriteLine("\t\t public override EntityType EntType {  get {return EntityType.Table;} }");
                            writer.WriteLine("\t\t public override string DBTableName {  get {return \""+str.ToUpper()+"\";} }");
                            writer.WriteLine("\t\t//自动生成字段");
                            
                            StringBuilder sbCol = new StringBuilder();
                            StringBuilder sbColString = new StringBuilder();
                            int num2 = 0;
                            while (num2 < table.Rows.Count)
                            {
                                #region 列的处理
                                string ColName = table.Rows[num2][3].ToString().ToUpper();
                                sbCol.Append("EF_"+ ColName + ",");
                                sbColString.Append("\"" + ColName + "\",");
                                //旧的纯字符格式
                                writer.WriteLine(string.Format("\t\t public static string {0} = \"{0}\";", table.Rows[num2][3].ToString().ToUpper()));
                                //新的实体列对象
                                string strVarchar = "VARCHAR";
                                string strVarchar36 = "VARCHAR(36)";
                                string DecimalBits = "-1";
                                string sNullable = "true";
                                string sIsPK = "false";
                                string sLength = "36";
                                string sFileName = "";
                                string sFieldType = "EntityFieldType.DbColumn";
                                string sUpdateControlID = "false";
                                writer.WriteLine("\t\t public BaseEntityField EF_" + ColName + " = new BaseEntityField().SetDbColumnName(\"" + ColName
                                    + "\").SetDbColumnType(\"" + strVarchar + "\").SetDbColumnTypeSize(\"" + strVarchar36
                                    + "\").SetDbDecimalBits(" + DecimalBits + ").SetDbIsNullable(" + sNullable
                                    + ").SetDbIsPK(" + sIsPK + ").SetDbLength(" + sLength + ").SetFieldName(\"" + sFileName
                                    + "\").SetFieldType(" + sFieldType + ").SetIsUpdateControlID(" + sUpdateControlID + ");");
                                writer.WriteLine();     
                                num2++; 
                                #endregion
                            }

                            #region 集合类
                            writer.WriteLine("\t\t public override List<BaseEntityField> DbColumnList");
                            writer.WriteLine("\t\t {");
                            writer.WriteLine("\t\t\tget");
                            writer.WriteLine("\t\t\t{");
                            writer.WriteLine("\t\t\t\t var DbColumn = new List<BaseEntityField>(); ");
                            writer.WriteLine("\t\t\t\t DbColumn.AddRange(new BaseEntityField[] { " + sbCol.ToString().Substring(0, sbCol.ToString().Length - 1) + " });");
                            writer.WriteLine("\t\t\t\t return DbColumn;");
                            writer.WriteLine("\t\t\t}");
                            writer.WriteLine("\t\t }");
                            //
                            writer.WriteLine("\t\t public override List<string> ColumnStringList");
                            writer.WriteLine("\t\t {");
                            writer.WriteLine("\t\t\tget");
                            writer.WriteLine("\t\t\t{");
                            writer.WriteLine("\t\t\t\t var DbColumn = new List<string>();");
                            writer.WriteLine("\t\t\t\t DbColumn.AddRange(new string[] { " + sbColString.ToString().Substring(0, sbColString.ToString().Length - 1) + " });");
                            writer.WriteLine("\t\t\t\t return DbColumn;");
                            writer.WriteLine("\t\t\t}");
                            writer.WriteLine("\t\t }");
                            #endregion
                            writer.WriteLine("\t}");
                            writer.WriteLine("}");
                            list.Add(new EntityInfo(nameSpace, "public", entityName, path + @"\" + entityName + ".cs"));
                        }
                    }
                }
            }
            return list;
        }
        #endregion

        #region 拼接字符
        public string FCharAddDownLine(string str)
        {
            //return (str.Substring(0, 1).ToLower() + str.Substring(1, str.Length - 1));
            return "_" + str;
        }

        public string FCharToUpper(string str)
        {
            return (str.Substring(0, 1).ToUpper() + str.Substring(1, str.Length - 1));
        } 
        #endregion

        #region 获取内存表
        public DataTable GetMemorySTable(string dataBase, string tblName, DbServerInfo server)
        {
            DataTable schema = null;
            DataBaseType databaseType = server.DatabaseType;
            string[] strArray;
            if ((server != null) && server.DatabaseType == DataBaseType.Oracle)
            {
                schema = this.GetSchema("Columns", new string[] { dataBase, tblName });
            }
            else if ((server != null) && server.DatabaseType == DataBaseType.MySql)
            {
                strArray = new string[3];
                strArray[1] = dataBase;
                strArray[2] = tblName;
                schema = this.GetSchema("Columns", strArray);
            }
            else
            {
                strArray = new string[3];
                strArray[0] = dataBase;
                strArray[2] = tblName;
                schema = this.GetSchema("Columns", strArray);
            }
            DataTable table2 = new DataTable();
            DataColumn column = new DataColumn("fieldId", typeof(int));
            DataColumn column2 = new DataColumn("dbName", typeof(string));
            DataColumn column3 = new DataColumn("tableName", typeof(string));
            DataColumn column4 = new DataColumn("fieldName", typeof(string));
            DataColumn column5 = new DataColumn("fieldType", typeof(string));
            DataColumn column6 = new DataColumn("typeSize", typeof(string));
            DataColumn column7 = new DataColumn("numericPrecision", typeof(string));
            table2.Columns.AddRange(new DataColumn[] { column, column2, column3, column4, column5, column6, column7 });
            for (int i = 0; i < schema.Rows.Count; i++)
            {
                DataRow row = table2.NewRow();
                row["fieldId"] = i + 1;
                row["dbName"] = dataBase;
                row["tableName"] = tblName;
                if ((server != null) && (server.DatabaseType == DataBaseType.Oracle))
                {
                    row["fieldName"] = schema.Rows[i][2].ToString();
                    row["fieldType"] = schema.Rows[i][4].ToString();
                    row["typeSize"] = schema.Rows[i][5].ToString();
                    row["numericPrecision"] = schema.Rows[i][6].ToString();
                }
                else
                {
                    row["fieldName"] = schema.Rows[i][3].ToString();
                    row["fieldType"] = schema.Rows[i][7].ToString();
                    if ((server != null) && (server.DatabaseType == DataBaseType.MySql))
                    {
                        row["typeSize"] = schema.Rows[i][8].ToString();
                        row["numericPrecision"] = schema.Rows[i][9].ToString();
                    }
                    else
                    {
                        row["typeSize"] = schema.Rows[i][9].ToString();
                        row["numericPrecision"] = schema.Rows[i][10].ToString();
                    }
                }
                table2.Rows.Add(row);
            }
            return table2;
        } 
        #endregion

        #region 获取架构
        public DataTable GetSchema(string collectionName, string[] restrictionValues)
        {
            return CustomDataAccess.DataAccess.GetDbSchema(collectionName, restrictionValues);
        } 
        #endregion

        #region 类型转换
        public string ParseType(string type)
        {
            switch (type)
            {
                case "char":
                case "nchar":
                case "ntext":
                case "text":
                case "nvarchar":
                case "varchar":
                case "xml":
                case "set":
                case "longtext":
                case "enum":
                case "blob":
                case "longblob":
                case "mediumtext":
                case "CHAR":
                case "CLOB":
                case "LONG":
                case "NCHAR":
                case "NCLOB":
                case "NVARCHAR2":
                case "ROWID":
                case "VARCHAR2":
                    return "string";

                case "smallint":
                    return "short";

                case "int":
                case "INTERVAL YEAR TO MONTH":
                    return "int";

                case "bigint":
                    return "long";

                case "binary":
                case "image":
                case "varbinary":
                case "timestamp":
                case "BFILE":
                case "BLOB":
                case "LONG RAW":
                case "RAW":
                    return "byte[]";

                case "tinyint":
                    return "SByte";

                case "bit":
                    return "bool";

                case "float":
                    return "double";

                case "real":
                    return "Guid";

                case "uniqueidentifier":
                    return "Guid";

                case "sql_variant":
                    return "object";

                case "decimal":
                case "numeric":
                case "money":
                case "FLOAT":
                case "INTEGER":
                case "NUMBER":
                case "smallmoney":
                case "UNSIGNED INTEGER":
                    return "decimal";

                case "INTERVAL DAY TO SECOND":
                    return "TimeSpan";

                case "datetime":
                case "smalldatetime":
                case "date":
                case "time":
                case "DATE":
                case "TIMESTAMP":
                case "TIMESTAMP WITH LOCAL TIME ZONE":
                case "TIMESTAMP WITH TIME ZONE":
                    return "DateTime";
            }
            return type;
        } 
        #endregion

    }
}

