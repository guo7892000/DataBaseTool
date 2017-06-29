using Breezee.Framework.BaseUI;
using Breezee.Framework.Tool;
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
    /// In清单生成
    /// 测试结果：通过
    /// </summary>
    public partial class FrmDBTInList : Form
    {
        #region 变量
        //
        private BindingSource bsTable = new BindingSource();
        private BindingSource bsCos = new BindingSource();//
        private BindingSource bsThree = new BindingSource();//
        //常量
        private string _strAutoSqlSuccess = "生成成功，并已复制到了粘贴板。详细见“生成的SQL”页签！";
        public IDictionary<string, BindingSource> dicBindingSource = new Dictionary<string, BindingSource>();

        #endregion

        #region 构造函数
        public FrmDBTInList()
        {
            InitializeComponent();
        }
        #endregion

        #region 加载事件
        private void FrmInList_Load(object sender, EventArgs e)
        {
            //初始化下拉框
            IDictionary<string, string> dic_List = new Dictionary<string, string>();
            dic_List.Add("1", "IN清单");
            dic_List.Add("2", "自定义前后缀");
            UIHelper.BindTypeValueDropDownList(cbbSqlType, dic_List.GetTextValueTable(false), false, true);
            //初始化网格
            DataTable dtIn = new DataTable();
            dtIn.Columns.Add("IN字段", typeof(string));
            bsTable.DataSource = dtIn;
            dgvTableList.DataSource = bsTable;
            lblInfo.Text = "请在Excel中复制一列内容，然后点击网格后按ctrl + v粘贴即可。";
        }
        #endregion

        #region 网格按钮事件
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
                    DataTable dtMain = (DataTable)bsTable.DataSource;
                    int iRow = 0;
                    int iColumn = 0;
                    Object[,] data = StringHelper.GetStringArray(ref pasteText, ref iRow, ref iColumn);
                    #region 生成IN清单
                    if (pasteText.IndexOf("in (", StringComparison.CurrentCultureIgnoreCase) > 0)//包括IN的为生成的SQL，不用粘贴
                    {
                        return;
                    }
                    foreach (DataRow dr in dtMain.Select("IN字段 is null or IN字段=''"))
                    {
                        dtMain.Rows.Remove(dr);
                    }
                    dtMain.AcceptChanges();
                    int rowindex = dtMain.Rows.Count;
                    int iGoodDataNum = 0;//有效数据号
                    //获取获取当前选中单元格所在的行序号
                    for (int j = 0; j < iRow; j++)
                    {
                        string strData = data[j, 0].ToString().Trim();
                        if (string.IsNullOrEmpty(strData))
                        {
                            continue;
                        }
                        if (dtMain.Select("IN字段='" + data[j, 0] + "'").Length == 0)
                        {
                            dtMain.Rows.Add(dtMain.NewRow());
                            dtMain.Rows[rowindex + iGoodDataNum][0] = strData;
                            iGoodDataNum++;
                        }
                    }
                    tsbAutoSQL.Enabled = true;

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
            //取得数据源
            DataTable dtMain = (DataTable)bsTable.DataSource;
            string strSqlType = cbbSqlType.SelectedValue.ToString();
            string strPreStr = txbPreString.Text;
            string strEndStr = txbEndString.Text;
            string strConnStr = txbConcateString.Text;
            string strReturnStr = cbkIsNewLine.Checked ? "\n" : "";
            //得到变更后数据
            dtMain.AcceptChanges();
            if (dtMain.Rows.Count == 0)
            {
                MessageBox.Show("没有可生成的数据！", "提示", MessageBoxButtons.OK);
                return;
            }
            string sbAllSql = "";
            string sbAllSqlEnd = "";
            if (strSqlType == "1")
            {
                sbAllSql = " in (";
                sbAllSqlEnd = ") ";
            }
            foreach (DataRow drTable in dtMain.Rows)//针对表清单循环
            {
                string strData = drTable[0].ToString().Trim();
                if (string.IsNullOrEmpty(strData)) continue;
                sbAllSql = sbAllSql + strPreStr + strData + strEndStr + strConnStr + strReturnStr;
            }
            sbAllSql = sbAllSql.Substring(0, sbAllSql.Length - 1) + sbAllSqlEnd;
            rtbResult.Clear();
            rtbResult.AppendText(sbAllSql.ToString() + "\n");
            Clipboard.SetData(DataFormats.UnicodeText, sbAllSql.ToString());
            tabControl1.SelectedTab = tpAutoSQL;
            //生成SQL成功后提示
            //MessageBox.Show(strInfo, "生成成功", MessageBoxButtons.OK);
            lblInfo.Text = _strAutoSqlSuccess;
            rtbResult.Select(0, 0); //返回到第一
        }
        #endregion

        #region 语句类型选择变化事件
        private void cbbSqlType_SelectedIndexChanged(object sender, EventArgs e)
        {
            string strSqlType = cbbSqlType.SelectedValue.ToString();
            if (strSqlType == "1")
            {
                txbPreString.Text = "'";
                txbEndString.Text = "'";
                txbConcateString.Text = ",";
            }
        }
        #endregion

        #region 退出按钮事件
        private void tsbExit_Click(object sender, EventArgs e)
        {
            Close();
        } 
        #endregion

        #region 帮助按钮事件
        private void tsbHelp_Click(object sender, EventArgs e)
        {

        } 
        #endregion
    }
}
