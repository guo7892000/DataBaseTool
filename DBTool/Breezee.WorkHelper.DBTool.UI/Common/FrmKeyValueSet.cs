using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Breezee.WorkHelper.DBTool.Entity;
using Breezee.Framework.BaseUI;
using Breezee.Global.Entity;

namespace Breezee.WorkHelper.DBTool.UI
{
    /// <summary>
    /// 创建作者：黄国辉
    /// 创建日期：2013-10-19
    /// 功能说明：查看和维护默认值、排除列
    /// </summary>
    public partial class FrmKeyValueSet : BaseForm
    {
        #region 变量
        private DataBaseType _DataBaseType = DataBaseType.SqlServer; //默认数据库类型
        private string _strType = "TYPE"; //类型
        private string _strKey = "KEY";
        private string _strValue = "VALUE";
        private string _strTitle = "";
        private string _FileName = "";
        private string _strDefalutTitleName = "列默认值的键值维护";
        private string _strExcludeTitleName = "排除列维护";
        private string _strOrcleName = "Oracle";
        private string _strSqlServerName = "SQL Server";
        private DataBaseType _DbTyp = DataBaseType.Oracle;
        private AutoSqlColumnSetType _ascst = AutoSqlColumnSetType.Default;
        private KeyValue _kv;
        #endregion

        public FrmKeyValueSet()
        {
            
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="DataBaseType"></param>
        public FrmKeyValueSet(DataBaseType dbt, AutoSqlColumnSetType ascst)
        {
            InitializeComponent();
            _DataBaseType = dbt;
            _kv = new KeyValue(dbt, ascst);
            _FileName = _kv.GetFileName();
            _DbTyp = dbt;
            _ascst = ascst;
            #region 决定标题
            if (dbt == DataBaseType.Oracle)
            {
                if (ascst == AutoSqlColumnSetType.Default)
                {
                    _strTitle = _strOrcleName + _strDefalutTitleName;
                }
                else
                {
                    _strTitle = _strOrcleName + _strExcludeTitleName;
                }
            }
            else
            {
                if (ascst == AutoSqlColumnSetType.Default)
                {
                    _strTitle = _strSqlServerName + _strDefalutTitleName;
                }
                else
                {
                    _strTitle = _strSqlServerName + _strExcludeTitleName;
                }
            }
            this.Text = _strTitle; 
            #endregion
        }

        /// <summary>
        /// 页面加载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FrmVersionInfo_Load(object sender, EventArgs e)
        {
            BindingSource bs = new BindingSource();
            bs.DataSource = _kv.LoadXMLFile();
            bs.Sort = "TYPE";
            dgvKey.DataSource = bs;
            dgvKey.Columns[_strType].Visible = false;
            dgvKey.Columns[_strKey].Width = 200;
            dgvKey.Columns[_strValue].Width = 200;
            if (_ascst == AutoSqlColumnSetType.Exclude)
            {
                dgvKey.Columns[_strValue].Visible = false;
            }
            
            //用户类型
            IDictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add(((int)SqlType.Insert).ToString(), "新增");
            dic.Add(((int)SqlType.Update).ToString(), "修改");
            dic.Add(((int)SqlType.Query).ToString(), "查询");
            dic.Add(((int)SqlType.Delete).ToString(), "删除");
            if (_ascst == AutoSqlColumnSetType.Default)
            {
                dic.Add(((int)SqlType.Parameter).ToString(), "参数");
            }
            //初始化处理类型
            DataTable dtDataSetType = UIHelper.GetTextValueTable(dic, false);
            DataGridViewComboBoxColumn cmbUserType = new DataGridViewComboBoxColumn();
            cmbUserType.DisplayIndex = 0;
            cmbUserType.HeaderText = "处理类型";
            cmbUserType.DataPropertyName = "TYPE";//
            cmbUserType.ValueMember = "VALUE";
            cmbUserType.DisplayMember = "TEXT";
            cmbUserType.DataSource = dtDataSetType.Copy();
            dgvKey.Columns.Insert(1, cmbUserType);
            
        }

        /// <summary>
        /// 保存按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsbSave_Click(object sender, EventArgs e)
        {
            groupBox1.Focus();
            BindingSource bs=(BindingSource)dgvKey.DataSource;
            bs.EndEdit();
            DataTable dt = (DataTable)bs.DataSource;
            DataTable dtSave = dt.GetChanges();
            if (dtSave==null || dtSave.Rows.Count == 0)
            {
                MsgHelper.ShowInfo("没有修改数据，不用保存！");
                return;
            }
            _kv.SaveXMLFile(dtSave, _FileName);
            MsgHelper.ShowInfo("保存成功！");
        }

        #region 关闭按钮事件
        /// <summary>
        /// 关闭按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsbExit_Click(object sender, EventArgs e)
        {
            Close();
        } 
        #endregion

        #region 网格数据出错事件
        /// <summary>
        /// 网格数据出错事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvKey_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            MessageBox.Show(this, e.Exception.Message, "提示", MessageBoxButtons.OK);
        } 
        #endregion

    }
}