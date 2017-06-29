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
    /// �������ߣ��ƹ���
    /// �������ڣ�2013-10-19
    /// ����˵�������ݿ���������
    /// </summary>
    public partial class FrmDBConfigSet : BaseForm
    {
        #region ����
        IDBConfigSet _IDBConfigSet;
        bool _IsMoreSelected = false; //�Ƿ��ѡ��Ĭ��Ϊ��
        public bool In_IsMoreSelected
        {
            get { return _IsMoreSelected; }
            set { _IsMoreSelected = value; }
        }
        #endregion

        #region ���캯��
        /// <summary>
        /// ���캯��
        /// </summary>
        /// <param name="DataBaseType"></param>
        public FrmDBConfigSet()
        {
            InitializeComponent();
        }
        #endregion

        #region ҳ�����
        /// <summary>
        /// ҳ�����
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FrmVersionInfo_Load(object sender, EventArgs e)
        {
            SetTag();//����Tag
            //���ݿ�����
            DataTable dtDbType = DBToolUIHelper.GetBaseDataTypeTable();
            UIHelper.BindTypeValueDropDownList(cbbDbType, dtDbType, true, true);
            //
            _IDBConfigSet = ContainerContext.Container.Resolve<IDBConfigSet>();
        }
        #endregion

        #region ����Tag����
        private void SetTag()
        {
            //��ѯ���
            FlexGridColumnDefinition fdc = new FlexGridColumnDefinition();
            fdc.AddColumn("ROWNO", "���", DataGridViewColumnTypeEnum.TextBox, true, 40, DataGridViewContentAlignment.MiddleLeft, false, 800);
            fdc.AddColumn("IS_SELECTED", "ѡ��", DataGridViewColumnTypeEnum.CheckBox, _IsMoreSelected, 60, DataGridViewContentAlignment.MiddleCenter, true, 800);
            fdc.AddColumn("DB_CONFIG_CODE", "���ñ���", DataGridViewColumnTypeEnum.TextBox, true, 80, DataGridViewContentAlignment.MiddleLeft, false, 800);
            fdc.AddColumn("DB_CONFIG_NAME", "��������", DataGridViewColumnTypeEnum.TextBox, true, 100, DataGridViewContentAlignment.MiddleLeft, false, 800);
            //fdc.AddColumn("DB_TYPE_" + "_NAME", "���ݿ�����", DataGridViewColumnTypeEnum.TextBox, true, 160, DataGridViewContentAlignment.MiddleLeft, false, 800);
            fdc.AddColumn("SERVER_IP", "������IP", DataGridViewColumnTypeEnum.TextBox, true, 260, DataGridViewContentAlignment.MiddleLeft, false, 800);
            fdc.AddColumn("DB_NAME", "���ݿ�����", DataGridViewColumnTypeEnum.TextBox, true, 160, DataGridViewContentAlignment.MiddleLeft, false, 800);
            fdc.AddColumn(DT_VE_DB_SUPPLIER.REMARK, "��ע", DataGridViewColumnTypeEnum.TextBox, true, 160, DataGridViewContentAlignment.MiddleLeft, false, 800);
            fdc.AddColumn(DT_VE_DB_SUPPLIER.CREATE_TIME, "��������", DataGridViewColumnTypeEnum.TextBox, true, 100, DataGridViewContentAlignment.MiddleLeft, false, 800);
            dgvQuery.Tag = fdc.GetGridTagString();
            UIHelper.BindDataGridView(dgvQuery, null, true);           
        }
        #endregion

        #region ��ѯ��ť�¼�
        private void tsbQuery_Click(object sender, EventArgs e)
        {
            _dicQuery[IDBConfigSet.QueryDbConfig_InDicKey.DB_TYPE] = cbbDbType.SelectedValue.ToString();
            _dicQuery[IDBConfigSet.QueryDbConfig_InDicKey.DB_CONFIG_CODE] = txbConfigCode.Text.Trim();
            _dicQuery[IDBConfigSet.QueryDbConfig_InDicKey.DB_NAME] = txbDbName.Text.Trim();
            DataTable dtQuery = _IDBConfigSet.QueryDbConfig(_dicQuery).SafeGetDictionaryTable();
            dgvQuery.BindDataGridView(dtQuery);
        } 
        #endregion

        #region ѡ��ť�¼�
        private void tsbSelect_Click(object sender, EventArgs e)
        {
            try
            {
                dgvQuery.EndEdit();

                BindingSource bs = (BindingSource)dgvQuery.DataSource;
                if (bs == null)
                {
                    ShowInfo("û��ѡ��һ���ͻ������Ȳ�ѯ��");
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
                    ShowInfo("������ѡ��һ���ͻ���");
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

        #region ������ť�¼�
        private void tsbNew_Click(object sender, EventArgs e)
        {
            FrmDBConfigSet_D frm = new FrmDBConfigSet_D();
            if (frm.ShowDialog() == DialogResult.OK)
            {
                tsbQuery.PerformClick();
            }
        } 
        #endregion

        #region �༭��ť�¼�
        private void tsbEdit_Click(object sender, EventArgs e)
        {
            DataRow drSelect = dgvQuery.GetCurrentRow();
            if (drSelect == null)
            {
                MsgHelper.ShowErr("��ѡ��һ�����ݣ�");
                return;
            }
            FrmDBConfigSet_D frm = new FrmDBConfigSet_D(drSelect);
            if(frm.ShowDialog() == DialogResult.OK)
            {
                tsbQuery.PerformClick();
            }
        } 
        #endregion

        #region �رհ�ť�¼�
        /// <summary>
        /// �رհ�ť�¼�
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsbExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        #endregion

        #region ����˫��
        private void dgvQuery_DoubleClick(object sender, EventArgs e)
        {
            tsbEdit.PerformClick();
        } 
        #endregion
    }
}