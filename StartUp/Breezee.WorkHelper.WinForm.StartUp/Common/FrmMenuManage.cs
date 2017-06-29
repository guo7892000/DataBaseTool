using Breezee.Framework.BaseUI;
using Breezee.Framework.Tool;
using Breezee.WorkHelper.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace Breezee.WorkHelper.WinForm.StartUp
{
    public partial class FrmMenuManage : BaseForm
    {
        #region 变量
        IDictionary<string, DMenu> _MenuDic = new Dictionary<string, DMenu>();
        DMenu _saveMenu = new DMenu();
        #endregion

        #region 构造函数
        public FrmMenuManage()
        {
            InitializeComponent();
        }
        #endregion

        #region 窗体加载
        private void FrmMenuManage_Load(object sender, EventArgs e)
        {
            _dicQuery.Clear();
            _dicQuery.Add(((int)MenuTypeEnum.Modul).ToString(), "模块");
            _dicQuery.Add(((int)MenuTypeEnum.Class).ToString(), "菜单分类");
            _dicQuery.Add(((int)MenuTypeEnum.Menu).ToString(), "功能");
            UIHelper.BindTypeValueDropDownList(cbbMenuType, _dicQuery.GetTextValueTable(false), false, true);
            //
            _dicQuery.Clear();
            foreach (var item in DDllFile.GetAllDll().Values)
            {
                _dicQuery[item.Code] = item.Code;
            }
            UIHelper.BindTypeValueDropDownList(cbbDLL, _dicQuery.GetTextValueTable(true), false, true);
            //
            cbbMenuType.Enabled = false;
            LoadMenu();
        } 
        #endregion

        #region 加载菜单方法
        private void LoadMenu()
        {
            //加载菜单
            tvLeftMenu.Nodes.Clear();
            _MenuDic = DMenu.GetAllMenu(AppType.WinForm);
            foreach (DMenu dMenu in _MenuDic.Values)
            {
                if (dMenu.MenuType != MenuTypeEnum.Modul)
                {
                    continue;
                }

                //左边树
                TreeNode tnNew = new TreeNode();
                tnNew.Name = dMenu.Guid;
                tnNew.Text = dMenu.Name;
                tnNew.Tag = dMenu;
                tvLeftMenu.Nodes.Add(tnNew);

                foreach (DMenu childMenu in dMenu.ChildMenu)
                {
                    if (childMenu.MenuType == MenuTypeEnum.Class)
                    {
                        AddMenuClassNode(tnNew, new TreeNode(), childMenu);
                    }
                    else
                    {
                        AddMenuNode(tnNew, new TreeNode(), childMenu);
                    }
                }
            }
        }
        #endregion

        #region 增加菜单分类
        private void AddMenuClassNode(TreeNode tnParent, TreeNode tnNew, DMenu dParentMenu)
        {
            AddMenuNode(tnParent, tnNew, dParentMenu);
            foreach (DMenu childMenu in dParentMenu.ChildMenu)
            {
                if (childMenu.MenuType == MenuTypeEnum.Class)
                {
                    AddMenuClassNode(tnNew, new TreeNode(), childMenu);
                }
                else
                {
                    AddMenuNode(tnNew, new TreeNode(), childMenu);
                }
            }
        }
        #endregion

        #region 增加菜单
        private void AddMenuNode(TreeNode tnParent, TreeNode tnNew, DMenu dMenu)
        {
            tnNew.Text = dMenu.Name;
            tnNew.Tag = dMenu;
            tnParent.Nodes.Add(tnNew);
        }
        #endregion

        #region 保存按钮事件
        private void tsbSave_Click(object sender, EventArgs e)
        {
            _saveMenu.Name = txbMenuName.Text.Trim();
            _saveMenu.Code = txbMenuCode.Text.Trim();
            _saveMenu.DLLName = cbbDLL.Text;
            _saveMenu.FormName = txbClassFullPath.Text.Trim();
            _saveMenu.ShortCutKey = txbShortCutKey.Text.Trim();
            if (string.IsNullOrEmpty(_saveMenu.Name))
            {
                MsgHelper.ShowErr("【菜单名称】不能为空！");
                return;
            }
            bool isAdd = false;
            XmlDocument xmlMenu = DMenu.ReadMenuXmlFile(AppType.WinForm);
            if (string.IsNullOrEmpty(_saveMenu.Guid))
            {
                isAdd = true;
            }

            #region 新增
            XmlElement xnNew;
            switch (_saveMenu.MenuType)
            {
                case MenuTypeEnum.Modul:
                    xnNew = xmlMenu.CreateElement("Model");
                    if (isAdd)
                    {
                        xnNew.SetAttribute(MemuAttrString.Guid, StringHelper.GetGUID());
                        xmlMenu.DocumentElement.AppendChild(xnNew);
                    }
                    else
                    {
                        xnNew = xmlMenu.SelectSingleNode("xml/Model[@Guid='" + _saveMenu.Guid + "']") as XmlElement;
                    }
                    break;
                case MenuTypeEnum.Class:
                    xnNew = xmlMenu.CreateElement("Class");
                    if (isAdd)
                    {
                        xnNew.SetAttribute(MemuAttrString.Guid, StringHelper.GetGUID());
                        //先找模块
                        XmlNode xnParent = xmlMenu.SelectSingleNode("xml/Model[@Guid='" + _saveMenu.ParentGuid + "']");
                        if (xnParent != null)
                        {
                            xnParent.AppendChild(xnNew);
                        }
                    }
                    if (xnNew.ParentNode == null)
                    {
                        //第一个分类
                        GetClassNode(isAdd, xmlMenu, "xml/Model/Class", ref xnNew);
                    }

                    if (xnNew.ParentNode == null)
                    {
                        //第二个菜单分类
                        GetClassNode(isAdd, xmlMenu, "xml/Model/Class/Class", ref xnNew);
                    }

                    if (xnNew.ParentNode == null)
                    {
                        //最多支持第三个菜单分类
                        GetClassNode(isAdd, xmlMenu, "xml/Model/Class/Class/Class", ref xnNew);
                    }
                    break;
                case MenuTypeEnum.Menu:
                default:
                    if (string.IsNullOrEmpty(_saveMenu.DLLName))
                    {
                        MsgHelper.ShowErr("【所在DLL文件】不能为空！");
                        return;
                    }
                    if (string.IsNullOrEmpty(_saveMenu.FormName))
                    {
                        MsgHelper.ShowErr("【全路径类名】不能为空！");
                        return;
                    }

                    xnNew = xmlMenu.CreateElement("Menu");
                    if (isAdd)
                    {
                        xnNew.SetAttribute(MemuAttrString.Guid, StringHelper.GetGUID());
                    }
                    //第一个分类
                    GetMenuNode(isAdd, xmlMenu, "xml/Model/Class","xml /Model/Class/Menu", ref xnNew);

                    if (xnNew.ParentNode == null)
                    {
                        //第二个菜单分类
                        GetMenuNode(isAdd, xmlMenu, "xml/Model/Class/Class", "xml/Model/Class/Class/Menu", ref xnNew);
                    }

                    if (xnNew.ParentNode == null)
                    {
                        //最多支持第三个菜单分类
                        GetMenuNode(isAdd, xmlMenu, "xml/Model/Class/Class/Class", "xml/Model/Class/Class/Class/Menu", ref xnNew);
                    }
                    break;
            }
            #endregion

            xnNew.SetAttribute(MemuAttrString.Name, _saveMenu.Name);
            xnNew.SetAttribute(MemuAttrString.Code, _saveMenu.Code);
            xnNew.SetAttribute(MemuAttrString.DLLName, _saveMenu.DLLName);
            xnNew.SetAttribute(MemuAttrString.FormName, _saveMenu.FormName);
            xnNew.SetAttribute(MemuAttrString.ShortCutKey, _saveMenu.ShortCutKey);
            //保存
            xmlMenu.Save(DMenu.MenuXmlFilePath);
            MsgHelper.ShowInfo("保存成功！");
            UIHelper.ResetControl(grpEdit);
            //重新加载菜单
            LoadMenu();
        }
        #endregion

        #region 获取菜单分类节点
        private void GetClassNode(bool isAdd, XmlDocument xmlMenu, string path, ref XmlElement xnNew)
        {
            XmlNodeList xmlList = xmlMenu.SelectNodes(path);
            foreach (XmlNode xn in xmlList)
            {
                DMenu xnMenu = DMenu.NodeToMenu(xn, MenuTypeEnum.Class, "");

                if (isAdd)
                {
                    if (xnMenu.Guid == _saveMenu.ParentGuid)
                    {
                        xn.AppendChild(xnNew);
                        break;
                    }
                }
                else
                {
                    if (xnMenu.Guid == _saveMenu.Guid)
                    {
                        xnNew = xn as XmlElement;
                        break;
                    }
                }
            }
        }
        #endregion

        #region 获取菜单节点
        private void GetMenuNode(bool isAdd, XmlDocument xmlMenu, string AddPath, string ModifyPath, ref XmlElement xnNew)
        {
            XmlNodeList xmlList = xmlMenu.SelectNodes(isAdd==true? AddPath : ModifyPath);
            foreach (XmlNode xn in xmlList)
            {
                DMenu xnMenu = DMenu.NodeToMenu(xn, MenuTypeEnum.Menu, "");

                if (isAdd)
                {
                    if (xnMenu.Guid == _saveMenu.ParentGuid)
                    {
                        xn.AppendChild(xnNew);
                        break;
                    }
                }
                else
                {
                    if (xnMenu.Guid == _saveMenu.Guid)
                    {
                        xnNew = xn as XmlElement;
                        break;
                    }
                }
            }
        } 
        #endregion

        #region 退出按钮事件
        private void tsbExit_Click(object sender, EventArgs e)
        {
            Close();
        } 
        #endregion

        #region 树选择后事件
        private void tvLeftMenu_AfterSelect(object sender, TreeViewEventArgs e)
        {
            TreeNode trSelect = e.Node;
            if (trSelect == null)
            {
                tsmiAddModel.Visible = true;
                tsmiAddClass.Visible = false;
                tsmiAddFunc.Visible = false;
                return;
            }
            DMenu selectMenu = trSelect.Tag as DMenu;
            switch (selectMenu.MenuType)
            {
                case MenuTypeEnum.Modul:
                    tsmiAddModel.Visible = true;
                    tsmiAddClass.Visible = true;
                    tsmiAddFunc.Visible = false;
                    break;
                case MenuTypeEnum.Class:
                    tsmiAddModel.Visible = false;
                    tsmiAddClass.Visible = true;
                    tsmiAddFunc.Visible = true;
                    break;
                case MenuTypeEnum.Menu:
                default:
                    tsmiAddModel.Visible = false;
                    tsmiAddClass.Visible = false;
                    tsmiAddFunc.Visible = false;
                    break;
            }
            //要保存的为当前选择的菜单
            _saveMenu = selectMenu;
            //绑定界面数据
            cbbMenuType.SelectedValue = ((int)_saveMenu.MenuType).ToString();
            txbMenuName.Text = _saveMenu.Name;
            txbMenuCode.Text = _saveMenu.Code;
            cbbDLL.Text = _saveMenu.DLLName;
            txbClassFullPath.Text = _saveMenu.FormName;
            txbShortCutKey.Text = _saveMenu.ShortCutKey;
        }
        #endregion

        #region 菜单类型选择变化事件
        private void cbbMenuType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbbMenuType.SelectedValue != null)
            {
                string sType = cbbMenuType.SelectedValue.ToString();
                if (!string.IsNullOrEmpty(sType))
                {
                    MenuTypeEnum selectType = (MenuTypeEnum)int.Parse(sType);
                    switch (selectType)
                    {
                        case MenuTypeEnum.Modul:
                            cbbDLL.Enabled = false;
                            txbClassFullPath.ReadOnly = true;
                            break;
                        case MenuTypeEnum.Class:
                            cbbDLL.Enabled = false;
                            txbClassFullPath.ReadOnly = true;
                            break;
                        case MenuTypeEnum.Menu:
                        default:
                            cbbDLL.Enabled = true;
                            txbClassFullPath.ReadOnly = false;
                            break;
                    }
                }
            }
        }
        #endregion

        #region 右键菜单事件
        private void tsmiAddModel_Click(object sender, EventArgs e)
        {
            UIHelper.ResetControl(grpEdit);
            _saveMenu = new DMenu();
            _saveMenu.MenuType = MenuTypeEnum.Modul;
            //改变下拉框
            cbbMenuType.SelectedValue = ((int)_saveMenu.MenuType).ToString();
        }

        private void tsmiAddClass_Click(object sender, EventArgs e)
        {
            //设置上级菜单
            TreeNode trSelect = tvLeftMenu.SelectedNode;
            if (trSelect == null)
            {
                return;
            }
            UIHelper.ResetControl(grpEdit);
            _saveMenu = new DMenu();
            _saveMenu.MenuType = MenuTypeEnum.Class;
            DMenu selectMenu = trSelect.Tag as DMenu;
            _saveMenu.ParentGuid = selectMenu.Guid;
            //改变下拉框
            cbbMenuType.SelectedValue = ((int)_saveMenu.MenuType).ToString();
        }

        private void tsmiAddFunc_Click(object sender, EventArgs e)
        {
            //设置上级菜单
            TreeNode trSelect = tvLeftMenu.SelectedNode;
            if (trSelect == null)
            {
                return;
            }
            UIHelper.ResetControl(grpEdit);
            _saveMenu = new DMenu();
            _saveMenu.MenuType = MenuTypeEnum.Menu;
            DMenu selectMenu = trSelect.Tag as DMenu;
            _saveMenu.ParentGuid = selectMenu.Guid;
            //改变下拉框
            cbbMenuType.SelectedValue = ((int)_saveMenu.MenuType).ToString();
        }
        #endregion

        #region 删除右键菜单事件
        private void tsmiDelete_Click(object sender, EventArgs e)
        {
            //设置上级菜单
            TreeNode trSelect = tvLeftMenu.SelectedNode;
            if (trSelect == null)
            {
                return;
            }
            if (trSelect.GetNodeCount(true) >0)
            {
                MsgHelper.ShowErr("请先删除子节点！");
                return;
            }

            if (MsgHelper.ShowOkCancel("确定要删除该菜单？")== DialogResult.Cancel)
            {
                return;
            }

            DMenu selectMenu = trSelect.Tag as DMenu;
            XmlDocument xmlMenu = DMenu.ReadMenuXmlFile(AppType.WinForm);
            #region 删除
            XmlNode xnNew;
            switch (_saveMenu.MenuType)
            {
                case MenuTypeEnum.Modul:
                    xnNew = xmlMenu.SelectSingleNode("xml/Model[@Guid='" + selectMenu.Guid + "']");
                    xnNew.ParentNode.RemoveChild(xnNew);
                    break;
                case MenuTypeEnum.Class:
                    //第一个分类
                    xnNew = xmlMenu.SelectSingleNode("xml/Model/Class[@Guid='" + selectMenu.Guid + "']");
                    
                    if (xnNew == null)
                    {
                        //第二个菜单分类
                        xnNew = xmlMenu.SelectSingleNode("xml/Model/Class/Class[@Guid='" + selectMenu.Guid + "']");
                    }

                    if (xnNew == null)
                    {
                        //最多支持第三个菜单分类
                        xnNew = xmlMenu.SelectSingleNode("xml/Model/Class/Class/Class[@Guid='" + selectMenu.Guid + "']");
                    }
                    xnNew.ParentNode.RemoveChild(xnNew);
                    break;
                case MenuTypeEnum.Menu:
                default:
                    //第一个分类
                    xnNew = xmlMenu.SelectSingleNode("xml/Model/Class/Menu[@Guid='" + selectMenu.Guid + "']");

                    if (xnNew == null)
                    {
                        //第二个菜单分类
                        xnNew = xmlMenu.SelectSingleNode("xml/Model/Class/Class/Menu[@Guid='" + selectMenu.Guid + "']");
                    }

                    if (xnNew == null)
                    {
                        //最多支持第三个菜单分类
                        xnNew = xmlMenu.SelectSingleNode("xml/Model/Class/Class/Class/Menu[@Guid='" + selectMenu.Guid + "']");
                    }

                    xnNew.ParentNode.RemoveChild(xnNew);
                    break;
            }
            #endregion

            //保存
            xmlMenu.Save(DMenu.MenuXmlFilePath);
            MsgHelper.ShowInfo("删除成功！");
            UIHelper.ResetControl(grpEdit);
            //重新加载菜单
            LoadMenu();
        }
        #endregion
    }
}
