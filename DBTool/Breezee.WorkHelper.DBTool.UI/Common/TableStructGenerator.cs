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
    /// �������ƣ����ɱ�ṹ��html�ļ�
    /// �������ߣ���ˮ��
    /// �������ڣ�2012-11-06
    /// ˵�������ɱ�ṹ��html�ļ������㸴�Ƶ�Excel
    /// </summary>
    internal class TableStructGenerator
    {
        #region ����
        public static readonly string TABKEY_TABLE_STRUCT = "Tab_Key_Table_Struct";

        private static string htmlString = string.Empty;

        private static string HtmlString
        {
            get { return htmlString; }
            set { htmlString = value; }
        } 
        #endregion

        /// <summary>
        /// �Ƴ���ṹTabҳ
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
        /// ���ɱ�ṹ����
        /// </summary>
        /// <param name="tabControl"></param>
        /// <param name="tableList"></param>
        /// <param name="columnList"></param>
        public static void Generate(TabControl tabControl, DataTable tableList, DataTable columnList)
        {
            HtmlString = GenerateHtmlString(tableList, columnList);

            if (!tabControl.TabPages.ContainsKey(TABKEY_TABLE_STRUCT))
            {
                tabControl.TabPages.Add(TABKEY_TABLE_STRUCT, "��ṹ���");                
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
        /// ������ĵ�����¼�
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void browser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            WebBrowser browser = sender as WebBrowser;            
            browser.Document.Write(HtmlString);
        }

        /// <summary>
        /// ����Html�ַ�
        /// </summary>
        /// <param name="tableList">���嵥</param>
        /// <param name="columnList">���嵥</param>
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
                string tableCode = rowTable["�����"].ToString();
                DataRow[] rowColumns = columnList.Select("�����='" + tableCode + "'");
                int index = 1;
                StringBuilder columnBuilder = new StringBuilder();
                foreach (DataRow row in rowColumns)
                {
                    string strColumnChangeType = row["�������"].ToString().Trim() == "" ? "����" : row["�������"].ToString().Trim();
                    if (strColumnChangeType == "��ɾ����")
                    {
                        strColumnChangeType = "�޸�";
                    }
                    string columnString = columnsTemplate.Replace("${ColumnName}", row["������"].ToString())
                        .Replace("${ColumnCode}", row["�б���"].ToString())
                        .Replace("${ColumnType}", row["����"].ToString())
                        .Replace("${ColumnWidth}", row["����"].ToString())
                        .Replace("${DecimalPlace}", row["С��λ"].ToString())
                        .Replace("${PrimaryKey}", row["��"].ToString())
                        .Replace("${DefaultValue}", row["Ĭ��ֵ"].ToString())
                        .Replace("${Rule}", row["����"].ToString())
                        .Replace("${Remark}", row["��ע"].ToString())
                        .Replace("$(No)", index.ToString())
                        .Replace("${ChangeType}", strColumnChangeType);
                    columnBuilder.Append(columnString);
                    index++;
                }

                string tableString = tableTemplate.Replace("$$(ColumnsHolder)", columnBuilder.ToString())
                    .Replace("${tableName}", rowTable["������"].ToString())
                    .Replace("${tableCode}", rowTable["�����"].ToString())
                    .Replace("${changeType}", rowTable["�������"].ToString())
                    .Replace("${tableRemark}", rowTable["��ע"].ToString());
                tableBuilder.Append(tableString);
            }

            string html = htmlTemplate.Replace("$$(TableHolder)", tableBuilder.ToString());
            return html;
        }

        /// <summary>
        /// ����ģ��
        /// </summary>
        /// <param name="path">�ļ�·��</param>
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
