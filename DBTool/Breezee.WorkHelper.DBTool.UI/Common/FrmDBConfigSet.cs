using System;
using System.Data;
using System.Windows.Forms;
using Breezee.Framework.BaseUI;
using Breezee.Global.Entity;
using Breezee.Global.Context;
using Breezee.WorkHelper.DBTool.INF;
using Breezee.Framework.Tool;

namespace Breezee.WorkHelper.DBTool.UI
{
    /// <summary>
    /// 创建作者：黄国辉
    /// 创建日期：2013-10-19
    /// 功能说明：数据库连接配置
    /// </summary>
    public partial class FrmDBConfigSet : BaseForm
    {
        #region 变量
        IDBConfigSet _IDBConfigSet;
        bool _IsMoreSelected = false; //是否多选，默认为否
        public bool In_IsMoreSelected
        {
            get { return _IsMoreSelected; }
            set { _IsMoreSelected = value; }
        }
        #endregion

        #region 构造函数
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="DataBaseType"></param>
        public FrmDBConfigSet()
        {
            InitializeComponent();
        }
        #endregion

        #region 页面加载
        /// <summary>
        /// 页面加载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FrmVersionInfo_Load(object sender, EventArgs e)
        {
            SetTag();//设置Tag
            //数据库类型
            DataTable dtDbType = DBToolUIHelper.GetBaseDataTypeTable();
            UIHelper.BindTypeValueDropDownList(cbbDbType, dtDbType, true, true);
            //
            _IDBConfigSet = ContainerContext.Container.Resolve<IDBConfigSet>();
        }
        #endregion

        #region 设置Tag方法
        private void SetTag()
        {
            //查询结果
            FlexGridColumnDefinition fdc = new FlexGridColumnDefinition();
            fdc.AddColumn("ROWNO", "序号", DataGridViewColumnTypeEnum.TextBox, true, 40, DataGridViewContentAlignment.MiddleLeft, false, 800);
            fdc.AddColumn("IS_SELECTED", "选择", DataGridViewColumnTypeEnum.CheckBox, _IsMoreSelected, 60, DataGridViewContentAlignment.MiddleCenter, true, 800);
            fdc.AddColumn("DB_CONFIG_CODE", "配置编码", DataGridViewColumnTypeEnum.TextBox, true, 80, DataGridViewContentAlignment.MiddleLeft, false, 800);
            fdc.AddColumn("DB_CONFIG_NAME", "配置名称", DataGridViewColumnTypeEnum.TextBox, true, 100, DataGridViewContentAlignment.MiddleLeft, false, 800);
            //fdc.AddColumn("DB_TYPE_" + "_NAME", "数据库类型", DataGridViewColumnTypeEnum.TextBox, true, 160, DataGridViewContentAlignment.MiddleLeft, false, 800);
            fdc.AddColumn("SERVER_IP", "服务器IP", DataGridViewColumnTypeEnum.TextBox, true, 260, DataGridViewContentAlignment.MiddleLeft, false, 800);
            fdc.AddColumn("DB_NAME", "数据库名称", DataGridViewColumnTypeEnum.TextBox, true, 160, DataGridViewContentAlignment.MiddleLeft, false, 800);
            fdc.AddColumn(DT_VE_DB_SUPPLIER.REMARK, "备注", DataGridViewColumnTypeEnum.TextBox, true, 160, DataGridViewContentAlignment.MiddleLeft, false, 800);
            fdc.AddColumn(DT_VE_DB_SUPPLIER.CREATE_TIME, "创建日期", DataGridViewColumnTypeEnum.TextBox, true, 100, DataGridViewContentAlignment.MiddleLeft, false, 800);
            dgvQuery.Tag = fdc.GetGridTagString();
            UIHelper.BindDataGridView(dgvQuery, null, true);           
        }
        #endregion

        #region 查询按钮事件
        private void tsbQuery_Click(object sender, EventArgs e)
        {
            _dicQuery[IDBConfigSet.QueryDbConfig_InDicKey.DB_TYPE] = cbbDbType.SelectedValue.ToString();
            _dicQuery[IDBConfigSet.QueryDbConfig_InDicKey.DB_CONFIG_CODE] = txbConfigCode.Text.Trim();
            _dicQuery[IDBConfigSet.QueryDbConfig_InDicKey.DB_NAME] = txbDbName.Text.Trim();
            DataTable dtQuery = _IDBConfigSet.QueryDbConfig(_dicQuery).SafeGetDictionaryTable();
            dgvQuery.BindDataGridView(dtQuery);
        } 
        #endregion

        #region 选择按钮事件
        private void tsbSelect_Click(object sender, EventArgs e)
        {
            try
            {
                dgvQuery.EndEdit();

                BindingSource bs = (BindingSource)dgvQuery.DataSource;
                if (bs == null)
                {
                    ShowInfo("没有选择一个客户，请先查询！");
                    return;
                }
                bs.EndEdit();
                DataTable dtSelect;
                if (_IsMoreSelected)
                {
                    DataTable roleList = (DataTable)bs.DataSource;
                    dtSelect = roleList.Select("IS_SELECTED = 'True'").CopyToDataTable();
                }
                else
                {
                    DataRow dr = (bs.Current as DataRowView).Row;
                    dtSelect = dr.Table.Clone();
                    dtSelect.ImportRow(dr);
                }
                if (dtSelect.Rows.Count == 0)
                {
                    ShowInfo("请至少选择一个客户！");
                    return;
                }
                Tag = dtSelect;
                DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                ShowInfo(ex.Message);
            }
        }
        #endregion

        #region 新增按钮事件
        private void tsbNew_Click(object sender, EventArgs e)
        {
            FrmDBConfigSet_D frm = new FrmDBConfigSet_D();
            if (frm.ShowDialog() == DialogResult.OK)
            {
                tsbQuery.PerformClick();
            }
        } 
        #endregion

        #region 编辑按钮事件
        private void tsbEdit_Click(object sender, EventArgs e)
        {
            DataRow drSelect = dgvQuery.GetCurrentRow();
            if (drSelect == null)
            {
                MsgHelper.ShowErr("请选择一条数据！");
                return;
            }
            FrmDBConfigSet_D frm = new FrmDBConfigSet_D(drSelect);
            if(frm.ShowDialog() == DialogResult.OK)
            {
                tsbQuery.PerformClick();
            }
        } 
        #endregion

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

        #region 网格双击
        private void dgvQuery_DoubleClick(object sender, EventArgs e)
        {
            tsbEdit.PerformClick();
        } 
        #endregion
    }
}