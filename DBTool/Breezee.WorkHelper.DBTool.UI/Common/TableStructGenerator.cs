using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Data;
using System.Reflection;
using System.IO;

namespace Breezee.WorkHelper.DBTool.UI
{
    /// <summary>
    /// 对象名称：生成表结构的html文件
    /// 创建作者：潘水庆
    /// 创建日期：2012-11-06
    /// 说明：生成表结构的html文件，方便复制到Excel
    /// </summary>
    internal class TableStructGenerator
    {
        #region 变量
        public static readonly string TABKEY_TABLE_STRUCT = "Tab_Key_Table_Struct";

        private static string htmlString = string.Empty;

        private static string HtmlString
        {
            get { return htmlString; }
            set { htmlString = value; }
        } 
        #endregion

        /// <summary>
        /// 移除表结构Tab页
        /// </summary>
        /// <param name="tabControl"></param>
        public static void RemoveTab(TabControl tabControl)
        {
            if (tabControl.TabPages.ContainsKey(TABKEY_TABLE_STRUCT))
            {
                tabControl.TabPages.RemoveByKey(TABKEY_TABLE_STRUCT);
            }
        }

        /// <summary>
        /// 生成表结构方法
        /// </summary>
        /// <param name="tabControl"></param>
        /// <param name="tableList"></param>
        /// <param name="columnList"></param>
        public static void Generate(TabControl tabControl, DataTable tableList, DataTable columnList)
        {
            HtmlString = GenerateHtmlString(tableList, columnList);

            if (!tabControl.TabPages.ContainsKey(TABKEY_TABLE_STRUCT))
            {
                tabControl.TabPages.Add(TABKEY_TABLE_STRUCT, "表结构变更");                
            }

            TabPage tabStruct = tabControl.TabPages[TABKEY_TABLE_STRUCT];
            tabStruct.Controls.Clear();

            WebBrowser browser = new WebBrowser();
            browser.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(browser_DocumentCompleted);

            tabStruct.Controls.Add(browser);
            browser.Dock = DockStyle.Fill;

            browser.Navigate("about:blank");           
        }

        /// <summary>
        /// 浏览器文档完成事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void browser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            WebBrowser browser = sender as WebBrowser;            
            browser.Document.Write(HtmlString);
        }

        /// <summary>
        /// 生成Html字符
        /// </summary>
        /// <param name="tableList">表清单</param>
        /// <param name="columnList">列清单</param>
        /// <returns></returns>
        private static string GenerateHtmlString(DataTable tableList, DataTable columnList)
        {
            string strTempPath = "Breezee.WorkHelper.DBTool.UI.Template";
            string htmlTemplate = LoadTemplate(strTempPath + ".Html.txt");
            string tableTemplate = LoadTemplate(strTempPath + ".Table.txt");
            string columnsTemplate = LoadTemplate(strTempPath + ".Columns.txt");

            StringBuilder tableBuilder = new StringBuilder();
            foreach(DataRow rowTable in tableList.Rows)
            {
                string tableCode = rowTable["表编码"].ToString();
                DataRow[] rowColumns = columnList.Select("表编码='" + tableCode + "'");
                int index = 1;
                StringBuilder columnBuilder = new StringBuilder();
                foreach (DataRow row in rowColumns)
                {
                    string strColumnChangeType = row["变更类型"].ToString().Trim() == "" ? "新增" : row["变更类型"].ToString().Trim();
                    if (strColumnChangeType == "先删后增")
                    {
                        strColumnChangeType = "修改";
                    }
                    string columnString = columnsTemplate.Replace("${ColumnName}", row["列名称"].ToString())
                        .Replace("${ColumnCode}", row["列编码"].ToString())
                        .Replace("${ColumnType}", row["类型"].ToString())
                        .Replace("${ColumnWidth}", row["长度"].ToString())
                        .Replace("${DecimalPlace}", row["小数位"].ToString())
                        .Replace("${PrimaryKey}", row["键"].ToString())
                        .Replace("${DefaultValue}", row["默认值"].ToString())
                        .Replace("${Rule}", row["必填"].ToString())
                        .Replace("${Remark}", row["备注"].ToString())
                        .Replace("$(No)", index.ToString())
                        .Replace("${ChangeType}", strColumnChangeType);
                    columnBuilder.Append(columnString);
                    index++;
                }

                string tableString = tableTemplate.Replace("$$(ColumnsHolder)", columnBuilder.ToString())
                    .Replace("${tableName}", rowTable["表名称"].ToString())
                    .Replace("${tableCode}", rowTable["表编码"].ToString())
                    .Replace("${changeType}", rowTable["变更类型"].ToString())
                    .Replace("${tableRemark}", rowTable["备注"].ToString());
                tableBuilder.Append(tableString);
            }

            string html = htmlTemplate.Replace("$$(TableHolder)", tableBuilder.ToString());
            return html;
        }

        /// <summary>
        /// 加载模板
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <returns></returns>
        private static string LoadTemplate(string path)
        {
            Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(path);
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
            {
                return reader.ReadToEnd();
            }
        }
    }
}
