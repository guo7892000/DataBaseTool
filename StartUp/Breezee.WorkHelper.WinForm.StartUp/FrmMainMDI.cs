using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Xml;
using System.Reflection;
using Breezee.WorkHelper.Entity;
using Breezee.Framework.BaseUI;
using Breezee.Framework.Tool;
using Breezee.Global.Entity;

namespace Breezee.WorkHelper.WinForm.StartUp
{
    /// <summary>
    /// 对象名称：MDI主窗体
    /// 对象类型：窗体
    /// 创建日期：2016-10-20
    /// 创建作者：黄国辉
    ///     注：要选择x86生成，因为SQLite的DLL是x86版本的
    /// </summary>
    public partial class FrmMainMDI : Form
    {
        #region 变量
        public event EventHandler<EventArgs> FormClosed;
        string _strAppPath = AppDomain.CurrentDomain.BaseDirectory;
        string _strConfigFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "WorkHelper/Config");
        int iStartMenu = 0;
        IDictionary<string, DMenu> _MenuDic = new Dictionary<string, DMenu>();
        ShortCutList _ShortCutMenuList;
        bool IsReLoad = false;
        private string _helpFileName = "DBTool.chm";
        #endregion

        #region 构造函数
        public FrmMainMDI()
        {
            InitializeComponent();
        } 
        #endregion

        #region 加载事件
        private void FrmMainMDI_Load(object sender, EventArgs e)
        {
            iStartMenu = menuStrip.Items.IndexOfKey(tsbStartMenu.Name);
            this.WindowState = FormWindowState.Maximized;
#if DEBUG
            tsbAutoGuid.Visible = true;
#endif
            tcMenu.Dock = DockStyle.Top;

            //为了方便功能参考，这里增加一个当前用户信息
            GetLoginUserInfo();
            //加载菜单
            LoadMenu();
            //增加快捷菜单
            LoadShortCutMenu();
            //默认桌面布满
            pnlDestop.Dock = DockStyle.Fill;

            //设置菜单查找数据源
            tstbMenuSearch.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            tstbMenuSearch.AutoCompleteSource = AutoCompleteSource.CustomSource;
            this.Text = "数据库小工具(开发人员专用) v1.0.0 正式版   最后更新日期：2017-06-28 by Breezee";// + Assembly.GetExecutingAssembly().GetName().Version;

            tvLeftMenu.ExpandAll();

            // set F1 help topic for this form
            helpProvider1.HelpNamespace = Application.StartupPath + @"/Help/DBTool.chm";
            helpProvider1.SetHelpNavigator(this, HelpNavigator.Topic);
        }
        #endregion

        #region 获取登录用户信息
        private void GetLoginUserInfo()
        {
            BaseForm._loginUser = new LoginUserInfo(new SYS_USER()
            {
                USER_ID = "B0C0AB05-3680-48BF-83D7-92A1AE2584BE",
                USER_NAME = "系统管理员",
                USER_CODE = "xtadmin",
                EMP_ID = "10001",
                EMP_NAME = "辉",
                ORG_ID = "1",
            });

        }
        #endregion

        #region 加载菜单方法
        private void LoadMenu()
        {
            //加载菜单
            tvLeftMenu.Nodes.Clear();
            //初始化菜单管理菜单
            InitMenuManageMenu();
            //XML中的菜单处理
            _MenuDic = DMenu.GetAllMenu(AppType.WinForm);
            foreach (DMenu dMenu in _MenuDic.Values)
            {
                if (dMenu.MenuType != MenuTypeEnum.Modul)
                {
                    continue;
                }
                //菜单项
                ToolStripMenuItem tmiNew = new ToolStripMenuItem();
                tmiNew.Name = dMenu.Guid;
                tmiNew.Text = dMenu.Name;
                tmiNew.Tag = dMenu;
                dMenu.FullPath = dMenu.Name;

                menuStrip.Items.Insert(iStartMenu + 1, tmiNew);

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
                        AddMenuClassItem(tmiNew,new ToolStripMenuItem(), childMenu);
                        AddMenuClassNode(tnNew,new TreeNode(), childMenu);
                    }
                    else
                    {
                        AddMenuItem(tmiNew, new ToolStripMenuItem(), childMenu);
                        AddMenuNode(tnNew,new TreeNode(), childMenu);
                    }
                }

                iStartMenu++;
            }
        } 
        #endregion

        #region 增加菜单分类
        private void AddMenuClassItem(ToolStripMenuItem tmiParent, ToolStripMenuItem tmiNew, DMenu dParentMenu)
        {
            AddMenuItem(tmiParent, tmiNew, dParentMenu);
            foreach (DMenu childMenu in dParentMenu.ChildMenu)
            {
                if (childMenu.MenuType == MenuTypeEnum.Class)
                {
                    AddMenuClassItem(tmiNew, new ToolStripMenuItem(), childMenu);
                }
                else
                {
                    AddMenuItem(tmiNew, new ToolStripMenuItem(), childMenu);
                }
            }
        }

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
        private void AddMenuItem(ToolStripMenuItem tmiParent, ToolStripMenuItem tmiNew, DMenu dMenu)
        {
            tmiNew.Text = dMenu.Name;
            tmiNew.Name = dMenu.Guid;
            tmiNew.Tag = dMenu;
            tmiNew.Click += MenuItem_Click;
            //菜单查找自动完成数据源
            tstbMenuSearch.AutoCompleteCustomSource.Add(dMenu.Name);
            tmiParent.DropDownItems.Add(tmiNew);
        }

        private void AddMenuNode(TreeNode tnParent, TreeNode tnNew, DMenu dMenu)
        {
            tnNew.Text = dMenu.Name;
            tnNew.Tag = dMenu;
            tnParent.Nodes.Add(tnNew);
        } 
        #endregion
        
        #region 加载快捷菜单
        private void LoadShortCutMenu()
        {
            if (!Directory.Exists(_strConfigFilePath))
            {
                Directory.CreateDirectory(_strConfigFilePath);
            }
            string strShortCutFilePath = Path.Combine(_strConfigFilePath, WorkHelperStaticString.ShortCutMenuFileName);
            if (!File.Exists(strShortCutFilePath))
            {
                XmlDocument xmlShortCut = new XmlDocument();
                XmlElement xmRoot = xmlShortCut.CreateElement("xml");
                xmlShortCut.AppendChild(xmRoot);
                xmlShortCut.Save(strShortCutFilePath);
            }
            else
            {
                XmlDocument xmlMenu = new XmlDocument();
                xmlMenu.Load(strShortCutFilePath);
                XmlNodeList xmlList = xmlMenu.SelectNodes("xml/Menu");
                _ShortCutMenuList = new ShortCutList();
                _ShortCutMenuList.AddShortCutItem += AddShortCutMenuItem;//新增快捷菜单事件

                foreach (XmlNode xnModel in xmlList)
                {
                    if (xnModel.NodeType == XmlNodeType.Comment)
                    {
                        continue;
                    }
                    string strGuid = xnModel.GetAttributeValue("Guid");
                    if (_MenuDic.ContainsKey(strGuid))
                    {
                        ShortCutItem scItem = new ShortCutItem(_MenuDic[strGuid]);
                        scItem.ShortCutItemClick += ShortCutMenuItem_Click;//点击快捷菜单事件
                        scItem.ShortCutItemCancel += CancelShortCutMenuItem_Click;//取消快捷菜单事件
                        _ShortCutMenuList.AddItem(scItem);
                    }
                }
                //
                _ShortCutMenuList.Dock = DockStyle.Fill;
                pnlDestop.Controls.Add(_ShortCutMenuList);

            }
        } 
        #endregion

        #region 退出系统
        private void tsbExitMenu_Click(object sender, EventArgs e)
        {
            this.Close();
        } 
        #endregion

        #region 工具栏可见菜单事件
        private void ToolBarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStrip.Visible = toolBarToolStripMenuItem.Checked;
        } 
        #endregion

        #region 状态栏可见菜单事件
        private void StatusBarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            statusStrip.Visible = statusBarToolStripMenuItem.Checked;
        }
        #endregion

        #region 重新排列窗体
        private void CascadeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.Cascade);
        }

        private void TileVerticalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.TileVertical);
        }

        private void TileHorizontalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.TileHorizontal);
        }

        private void ArrangeIconsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.ArrangeIcons);
        } 
        #endregion

        #region 关闭所有窗体
        private void CloseAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (Form childForm in MdiChildren)
            {
                childForm.Close();
            }
        } 
        #endregion

        #region 重新启动
        private void tsbRestartMenu_Click(object sender, EventArgs e)
        {
            this.Close();
            if (IsReLoad)
            {
                this.Dispose();
                Application.Restart();
            }
        } 
        #endregion

        #region 窗体关闭中事件
        private void FrmMainMDI_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MsgHelper.ShowOkCancel("确定要关闭所有窗体退出吗？") == DialogResult.Cancel)
            {
                e.Cancel = true;
                IsReLoad = false;
            }
            else
            {
                e.Cancel = false;
                IsReLoad = true;

                if (FormClosed != null)
                {
                    FormClosed(this, e);
                }
            }
        } 
        #endregion

        #region 打开窗体方法
        private void OpenMenu(DMenu dMenu, bool IsExpandTreeNode = true)
        {
            if (dMenu == null || dMenu.MenuType != MenuTypeEnum.Menu)
            {
                return;
            }

            if (IsExpandTreeNode)
            {
                OpenTreeNodeMenu(dMenu.Name);
            }
            //判断窗体是否已经打开
            foreach (Form frm in this.MdiChildren)
            {
                DMenu dMenuFrm = frm.Tag as DMenu;
                if (dMenuFrm.Guid.Equals(dMenu.Guid))
                {
                    //选中页签
                    if (tcMenu.SelectedTab != tcMenu.TabPages[dMenu.Guid])
                    {
                        tcMenu.SelectedTab = tcMenu.TabPages[dMenu.Guid];
                    }
                    txbMenuPath.Text = dMenu.FullPath;
                    pnlDestop.Hide();
                    frm.Activate();
                    return;
                }
            }
            //反射得到窗体
            Assembly dll = Assembly.LoadFile(Path.Combine(_strAppPath, dMenu.DLLName));
            object form = dll.CreateInstance(dMenu.FormName);
            if (form is Form)
            {
                Form newForm = form as Form;
                newForm.Tag = dMenu;
                newForm.MdiParent = this;
                newForm.WindowState = FormWindowState.Maximized;
                newForm.Activated += ChildForm_Active;
                newForm.FormClosed += MdiChild_Close;
                //增加页签
                tcMenu.TabPages.Add(dMenu.Guid, dMenu.Name);
                tcMenu.TabPages[dMenu.Guid].Tag = dMenu;
                tcMenu.SelectedTab = tcMenu.TabPages[dMenu.Guid];
                txbMenuPath.Text = dMenu.FullPath;

                newForm.Show();
            }
            else
            {
                MsgHelper.ShowErr("配置错误，【" + dMenu.Name + "】菜单不是窗体类型！");
            }
        } 
        #endregion

        #region 菜单项点击事件
        private void MenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem tsmiClick = sender as ToolStripMenuItem;
            OpenMenu(tsmiClick.Tag as DMenu, true);
        }
        #endregion

        #region 快捷菜单项点击事件
        private void ShortCutMenuItem_Click(object sender, ShortCutItemClickEventArgs e)
        {
            OpenMenu(e.Menu, true);
        }
        #endregion

        #region 取消快捷菜单项点击事件
        private void CancelShortCutMenuItem_Click(object sender, ShortCutItemClickEventArgs e)
        {
            SaveShortCutMenuConfig(e.Menu,false);
        }
        #endregion

        #region 增加快捷菜单项点击事件
        private void AddShortCutMenuItem(object sender, ShortCutItemClickEventArgs e)
        {

            SaveShortCutMenuConfig(e.Menu, true);
        }
        #endregion

        #region 保存快捷菜单配置
        private void SaveShortCutMenuConfig(DMenu dMenu,bool IsAdd)
        {
            string strShortCutFilePath = Path.Combine(_strConfigFilePath, WorkHelperStaticString.ShortCutMenuFileName);
            XmlDocument xmlMenu = new XmlDocument();
            xmlMenu.Load(strShortCutFilePath);
            XmlNodeList xmlList = xmlMenu.SelectNodes("xml/Menu");
            XmlNode xnRemove = xmlMenu.SelectSingleNode("xml/Menu[@Guid='" + dMenu.Guid + "']");
            if (xnRemove != null)
            {                
                if (!IsAdd)
                {
                    xnRemove.ParentNode.RemoveChild(xnRemove);
                    //从快捷菜单中移除
                    if(_ShortCutMenuList.ItemList.ContainsKey(dMenu.Guid))
                    {
                        _ShortCutMenuList.MenuListPanl.Controls.Remove(_ShortCutMenuList.ItemList[dMenu.Guid]);
                        _ShortCutMenuList.ItemList.Remove(dMenu.Guid);
                        
                    }
                }
            }
            else
            { 
                if (IsAdd)
                {
                    XmlElement xnNew = xmlMenu.CreateElement("Menu");
                    xnNew.SetAttribute("Guid", dMenu.Guid);
                    xnNew.SetAttribute("Name", dMenu.Name);
                    xmlMenu.DocumentElement.AppendChild(xnNew);

                    if (!_ShortCutMenuList.ItemList.ContainsKey(dMenu.Guid))
                    {
                        ShortCutItem scItem = new ShortCutItem(dMenu);
                        scItem.ShortCutItemClick += ShortCutMenuItem_Click;//点击快捷菜单事件
                        scItem.ShortCutItemCancel += CancelShortCutMenuItem_Click;//取消快捷菜单事件
                        _ShortCutMenuList.AddItem(scItem);
                    }
                }
            }
            xmlMenu.Save(strShortCutFilePath);
        }
        #endregion

        #region 生成GUID按钮事件
        private void tsbAutoGuid_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(Guid.NewGuid().ToString().ToUpper());
        } 
        #endregion

        #region 页签选择变化
        private void tcMenu_SelectedIndexChanged(object sender, EventArgs e)
        {
            TabPage tpSelect = tcMenu.SelectedTab;
            if (tpSelect == null || tcMenu.SelectedTab == tpgDesktop)
            {
                if (tpSelect == null) tcMenu.SelectedTab = tpgDesktop;
                pnlDestop.Show();
                pnlDestop.Dock = DockStyle.Fill;
                txbMenuPath.Text = "桌面";
            }
            else
            {
                pnlDestop.Hide();
                OpenMenu(tpSelect.Tag as DMenu, true);
            }
        } 
        #endregion

        #region 双击打开树菜单
        private void tvLeftMenu_DoubleClick(object sender, EventArgs e)
        {
            TreeNode tnSelect = tvLeftMenu.SelectedNode;
            if (tnSelect == null)
            {
                return;
            }

            //打开菜单
            OpenMenu(tnSelect.Tag as DMenu, false);
        } 
        #endregion

        #region 树节点开始拖动事件
        private void tvLeftMenu_ItemDrag(object sender, ItemDragEventArgs e)
        {
            // 开始进行拖放操作，并将拖放的效果设置成移动。
            this.DoDragDrop(e.Item, DragDropEffects.Move);
        } 
        #endregion

        #region 查找菜单回车键事件
        private void tstbMenuSearch_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                string strSearchMenu=tstbMenuSearch.Text.Trim();
                if (string.IsNullOrEmpty(strSearchMenu))
                {
                    return;
                }
                //打开树节点方法
                OpenTreeNodeMenu(strSearchMenu);
            }
        }
        #endregion

        #region 打开树节点方法
        private void OpenTreeNodeMenu(string strSearchMenu)
        {
            TreeNode tnFind = null;
            foreach (TreeNode xn in tvLeftMenu.Nodes)
            {
                tnFind = FindNodeByText(xn, strSearchMenu);
                if (tnFind != null)
                {
                    ExpandParentNode(tnFind);
                    break;
                }
            }
        }  
        #endregion

        #region 根据文本查找树节点
        private TreeNode FindNodeByText(TreeNode tnParent, string strText)
        {
            if (tnParent == null) return null;
            if (tnParent.Text == strText) return tnParent;

            TreeNode tnRet = null;
            foreach (TreeNode tn in tnParent.Nodes)
            {
                tnRet = FindNodeByText(tn, strText);
                if (tnRet != null) break;
            }
            return tnRet;
        } 
        #endregion

        #region 递归打开左边树节点方法
        private void ExpandParentNode(TreeNode tnParent)
        {
            tnParent.Expand();
            if (tnParent.Parent != null)
            {
                ExpandParentNode(tnParent.Parent);
            }
        } 
        #endregion

        #region 隐藏左边树按钮事件
        private void btnHideTree_Click(object sender, EventArgs e)
        {
            if (tvLeftMenu.Visible)
            {
                tvLeftMenu.Hide();
                btnHideTree.Text = ">";
            }
            else
            {
                tvLeftMenu.Show();
                btnHideTree.Text = "<";
            }
        } 
        #endregion

        #region 关于菜单事件
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutAuthor about = new AboutAuthor();
            about.ShowDialog();
        } 
        #endregion

        #region 子窗体激活事件
        private void ChildForm_Active(object sender, EventArgs e)
        {
            if (sender is Form)
            {
                Form frmCurrent = sender as Form;
                DMenu dMenu = frmCurrent.Tag as DMenu;
                if (tcMenu.SelectedTab != tcMenu.TabPages[dMenu.Guid])
                {
                    tcMenu.SelectedTab = tcMenu.TabPages[dMenu.Guid];
                }
                txbMenuPath.Text = dMenu.FullPath;
            }
        }
        #endregion

        #region 子窗体关闭事件
        private void MdiChild_Close(object sender, EventArgs e)
        {
            try
            {
                if (sender is Form)
                {
                    Form frmCurrent = sender as Form;
                    DMenu dMenu = frmCurrent.Tag as DMenu;
                    if (tcMenu.TabPages.ContainsKey(dMenu.Guid))
                    {
                        tcMenu.TabPages.Remove(tcMenu.TabPages[dMenu.Guid]);
                    }
                }
            }
            catch(Exception ex)
            {
            }
        } 
        #endregion

        #region 菜单Tag页位置
        private void cmsMenuTabPositionUp_Click(object sender, EventArgs e)
        {
            tcMenu.Dock = DockStyle.Top;
        }

        private void cmsMenuTabPositionDown_Click(object sender, EventArgs e)
        {
            tcMenu.Dock = DockStyle.Bottom;
        }
        #endregion

        #region 菜单管理菜单
        private void InitMenuManageMenu()
        {
            //设置菜单类
            DMenu dMenuManage = new DMenu();
            dMenuManage.Guid = "59D1EA9B-E422-4F22-88D8-A0509B14EC0F";
            dMenuManage.MenuType = MenuTypeEnum.Menu;
            dMenuManage.DLLName = "Breezee.WorkHelper.WinForm.StartUp.exe";
            dMenuManage.FormName = "Breezee.WorkHelper.WinForm.StartUp.FrmMenuManage";
            dMenuManage.Name = "菜单管理";
            dMenuManage.FullPath = "开始 > 菜单管理";
            
            //设置菜单项
            tsmiMenuManage.Text = dMenuManage.Name;
            tsmiMenuManage.Name = dMenuManage.Guid;
            tsmiMenuManage.Tag = dMenuManage;
            tsmiMenuManage.Click += MenuItem_Click;
            //菜单查找自动完成数据源
            tstbMenuSearch.AutoCompleteCustomSource.Add(dMenuManage.Name);
            //左边树
            TreeNode tnBegin = new TreeNode();
            if(tsbStartMenu.Text.IndexOf("(")>0)
            {
                tnBegin.Text = tsbStartMenu.Text.Substring(0, tsbStartMenu.Text.IndexOf("("));
            }
            else
            {
                tnBegin.Text = tsbStartMenu.Text;
            }
            tnBegin.Name = StringHelper.GetGUID();
            tnBegin.Tag = null;
            tvLeftMenu.Nodes.Add(tnBegin);

            TreeNode tnMenuManage = new TreeNode();
            tnMenuManage.Name = dMenuManage.Guid;
            tnMenuManage.Text = dMenuManage.Name;
            tnMenuManage.Tag = dMenuManage;
            tnBegin.Nodes.Add(tnMenuManage);
        }
        #endregion

        private void tsmiUserManual_Click(object sender, EventArgs e)
        {
            Help.ShowHelp(this, helpProvider1.HelpNamespace);
        }
    }
}
