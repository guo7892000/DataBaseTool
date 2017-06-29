using Breezee.Framework.BaseUI;
using Breezee.Framework.DataAccess.INF;
using Breezee.Framework.Tool;
using Breezee.Global.Config;
using Breezee.Global.Context;
using Breezee.Global.Entity;
using Breezee.WorkHelper.DBTool.Entity;
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
    /// 数据库配置维护
    /// </summary>
    public partial class FrmDBConfigSet_D : BaseForm
    {
        #region 变量
        private DataRow _drEdit;
        public bool IsDbNameNotNull = true;//是否数据库名非空
        public bool IsFilterDbExtnedFile = true;//是否过滤db后缀名的数据库文件
        //控件集合字典
        List<DBColumnControlRelation> _listSupply = new List<DBColumnControlRelation>();
        //
        private IDBConfigSet _IDBConfigSet;
        private Framework.DataAccess.INF.IDataAccess _IDataAccess;
        #endregion

        #region 构造函数
        public FrmDBConfigSet_D()
        {
            InitializeComponent();
        }

        public FrmDBConfigSet_D(DataRow drEdit)
        {
            InitializeComponent();
            _drEdit = drEdit;

        }
        #endregion

        #region 窗体加载事件
        private void FrmDBConfigSet_D_Load(object sender, EventArgs e)
        {
            //接口对象
            _IDBConfigSet = ContainerContext.Container.Resolve<IDBConfigSet>();
            _IDataAccess = DataAccessFactory.GetDefaultDataAccess();

            #region 绑定下拉框
            //数据库类型
            DataTable dtDbType = DBToolUIHelper.GetBaseDataTypeTable();
            UIHelper.BindTypeValueDropDownList(cbbDatabaseType, dtDbType, false, true);
            //登录类型
            IDictionary<string, string> dicQuery = new Dictionary<string, string>();
            dicQuery.Add(((int)LoginModeEnum.SQL).ToString(), "SQL身份验证");
            dicQuery.Add(((int)LoginModeEnum.Windows).ToString(), "Windows身份验证");
            UIHelper.BindTypeValueDropDownList(cbbLoginType, UIHelper.GetTextValueTable(dicQuery, false), false, true); 
            #endregion

            //设置控件关系
            SetControlColumnRelation();

            if (_drEdit == null)//新增
            {

            }
            else //修改
            {
                UIHelper.SetControlValue(_listSupply, _drEdit);
            }
        }
        #endregion

        #region 设置列名与控件关系
        private void SetControlColumnRelation()
        {
            //配置表
            _listSupply.Add(new DBColumnControlRelation(DT_WH_BD_DB_CONFIG.DB_CONFIG_ID, txbID));
            _listSupply.Add(new DBColumnControlRelation(DT_WH_BD_DB_CONFIG.UPDATE_CONTROL_ID, txbUPDATE_CONTROL_ID));
            _listSupply.Add(new DBColumnControlRelation(DT_WH_BD_DB_CONFIG.DB_TYPE, cbbDatabaseType, "数据库类型"));
            _listSupply.Add(new DBColumnControlRelation(DT_WH_BD_DB_CONFIG.DB_CONFIG_CODE, txbDBConfigCode, "配置编码"));
            _listSupply.Add(new DBColumnControlRelation(DT_WH_BD_DB_CONFIG.DB_CONFIG_NAME, txbDBConfigName));
            _listSupply.Add(new DBColumnControlRelation(DT_WH_BD_DB_CONFIG.SERVER_IP, txbServerIP, "服务器IP"));
            _listSupply.Add(new DBColumnControlRelation(DT_WH_BD_DB_CONFIG.PORT_NO, txbPortNO));
            _listSupply.Add(new DBColumnControlRelation(DT_WH_BD_DB_CONFIG.DB_NAME, txbDbName));
            _listSupply.Add(new DBColumnControlRelation(DT_WH_BD_DB_CONFIG.SCHEMA_NAME, txbSchemaName));
            _listSupply.Add(new DBColumnControlRelation(DT_WH_BD_DB_CONFIG.LOGIN_TYPE, cbbLoginType));
            _listSupply.Add(new DBColumnControlRelation(DT_WH_BD_DB_CONFIG.USER_NAME, txbUserName));
            _listSupply.Add(new DBColumnControlRelation(DT_WH_BD_DB_CONFIG.USER_PASSWORD, txbPassword));
            _listSupply.Add(new DBColumnControlRelation(DT_WH_BD_DB_CONFIG.REMARK, txbRemark));
        }
        #endregion

        #region 保存按钮事件
        private void tsbSave_Click(object sender, EventArgs e)
        {
            try
            {
                #region 保存前判断
                string strInfo = UIHelper.JudgeNotNull(_listSupply, true);
                if (!string.IsNullOrEmpty(strInfo))
                {
                    ShowInfo("保存失败！\n" + strInfo);
                    return;
                }
                #endregion

                _dicObject = CreateObjectDictionary(true);
                DataTable dtSave;

                #region 供应商表
                bool isAdd = txbID.Text.Length == 0;

                List<string> coloumns = isAdd ? null : _listSupply.GetSaveColumnNameList();
                dtSave = UIHelper.GetTableConstruct(DT_WH_BD_DB_CONFIG.NewEntity(), coloumns, true, _loginUser,false);
                UIHelper.GetControlValue(_listSupply, dtSave, isAdd);
                if (isAdd)
                {
                    dtSave.Rows[0][DT_WH_BD_DB_CONFIG.DB_CONFIG_ID] = StringHelper.GetGUID();
                }
                #endregion
                //保存传入参数处理
                _dicObject[DT_SYS_USER.ORG_ID] = _loginUser.ORG_ID;
                _dicObject[DT_SYS_USER.USER_ID] = _loginUser.USER_ID;
                _dicObject[DT_ORG_EMPLOYEE.EMP_ID] = _loginUser.EMP_ID;
                _dicObject[DT_ORG_EMPLOYEE.EMP_NAME] = _loginUser.EMP_NAME;
                _dicObject[IDBConfigSet.SaveDbConfig_InDicKey.DT_TABLE] = dtSave;
                //保存维修单
                UIHelper.SafeGetDictionary(_IDBConfigSet.SaveDbConfig(_dicObject));
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                ShowErr(ex.Message);
            }
        } 
        #endregion
        
        #region 退出按钮事件
        private void tsbExit_Click(object sender, EventArgs e)
        {
            Close();
        }
        #endregion

        #region 选择变化事件
        private void cbbDatabaseType_SelectedIndexChanged(object sender, EventArgs e)
        {
            //重置控件
            UIHelper.ResetControl(txbServerIP, txbUserName, txbPassword, txbDbName, txbPortNO, txbSchemaName);
            //默认不显示登录类型
            lblLoginType.Visible = false;
            cbbLoginType.Visible = false;
            //默认显示端口号
            lblPortNO.Visible = true;
            txbPortNO.Visible = true;
            //显示数据库
            lblDbName.Visible = true;
            txbDbName.Visible = true;
            //
            lblServerAddr.Text = "服务器地址：";
            btnSelectDbFile.Visible = false;

            int iDbType = int.Parse(cbbDatabaseType.SelectedValue.ToString());
            DataBaseType selectDBType = (DataBaseType)iDbType;

            switch (selectDBType)
            {
                case DataBaseType.SqlServer:
                    //显示登录类型
                    lblLoginType.Visible = true;
                    cbbLoginType.Visible = true;
                    //
                    txbServerIP.Text = ".";
                    break;
                case DataBaseType.Oracle:
                    lblServerAddr.Text = "TNS名称：";
                    //不显示端口号
                    lblPortNO.Visible = false;
                    txbPortNO.Visible = false;
                    //不显示数据库
                    lblDbName.Visible = false;
                    txbDbName.Visible = false;
                    break;
                case DataBaseType.MySql:
                    break;
                case DataBaseType.SQLite:
                    lblServerAddr.Text = "数据库文件路径：";
                    btnSelectDbFile.Visible = true;
                    //不显示端口号
                    lblPortNO.Visible = false;
                    txbPortNO.Visible = false;
                    //不显示数据库
                    lblDbName.Visible = false;
                    txbDbName.Visible = false;

                    break;
                case DataBaseType.PostgreSql:
                    lblPortNO.Visible = true;
                    txbPortNO.Visible = true;
                    break;
                default:
                    throw new Exception("暂不支持该数据库类型！");
                    //break;
            }
        }
        #endregion

        #region 选择数据库文件
        private void btnSelectDbFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Multiselect = false;
            if (IsFilterDbExtnedFile)
            {
                ofd.Filter = "所有db文件|*.db";
            }
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                txbServerIP.Text = ofd.FileName;
                //txbServerIP.Text = ofd.SafeFileName;
            }
        } 
        #endregion
    }
}
