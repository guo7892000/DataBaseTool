using Breezee.Framework.BaseUI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Breezee.Framework.Tool;

namespace Breezee.WorkHelper.WinForm.StartUp
{
    public partial class FrmDllManage : BaseForm
    {
        DLLSet _dllSet;
        public FrmDllManage()
        {
            InitializeComponent();
        }

        private void FrmDllManage_Load(object sender, EventArgs e)
        {
            _dllSet = new DLLSet();
            SetTag();
            
            DataTable dtDll = _dllSet.LoadXMLFile();
            UIHelper.BindDataGridView(dgvQuery, dtDll, true);
            dgvQuery.AllowUserToAddRows = true;//允许新增行
        }

        #region 设置Tag方法
        private void SetTag()
        {
            //设置Tag
            FlexGridColumnDefinition fdc = new FlexGridColumnDefinition();
            fdc.AddColumn("Code", "DLL名称", DataGridViewColumnTypeEnum.TextBox, true, 400, DataGridViewContentAlignment.MiddleLeft, true, 800);
            fdc.AddColumn("Name", "备注", DataGridViewColumnTypeEnum.TextBox, true, 400, DataGridViewContentAlignment.MiddleLeft, true, 800);
            dgvQuery.Tag = fdc.GetGridTagString();
            UIHelper.BindDataGridView(dgvQuery, null, true);
        }
        #endregion

        private void tsbSave_Click(object sender, EventArgs e)
        {
            DataTable dtSource = dgvQuery.GetBindingTable();
            
            if (dtSource==null || dtSource.Rows.Count==0)
            {
                MsgHelper.ShowErr("没有要保存的数据！");
                return;
            }
            _dllSet.SaveXMLFile(dtSource, _dllSet.GetFileName());
            MsgHelper.ShowInfo("保存成功！");
        }

        private void tsbExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void tsbDelete_Click(object sender, EventArgs e)
        {
            dgvQuery.Rows.Remove(dgvQuery.CurrentRow);
        }
    }
}
