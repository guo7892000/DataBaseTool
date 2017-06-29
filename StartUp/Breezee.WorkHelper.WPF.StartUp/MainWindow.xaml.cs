using Breezee.Framework.Tool;
using Breezee.WorkHelper.Entity;
using Breezee.WorkHelper.Tool;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;

namespace Breezee.WorkHelper.WPF.StartUp
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        #region 变量
        public event EventHandler<EventArgs> FormClosed;
        string _strAppPath = AppDomain.CurrentDomain.BaseDirectory;
        string _strConfigFilePath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "WorkHelper/Config");
        int iStartMenu = 0;
        IDictionary<string, DMenu> _MenuDic = new Dictionary<string, DMenu>();
        //桌面
        UC_Desktop _UC_Desktop;
        TabItem _desktopTabItem;
        private string _desktopName = "桌面";
        bool IsReLoad = false;//是否重新登录
        TreeViewItem tnFind = null;
        #endregion

        #region 构造函数
        public MainWindow()
        {
            InitializeComponent();
        }
        #endregion

        #region 加载按钮事件
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Title = "工作小助手 v" + Assembly.GetExecutingAssembly().GetName().Version;
            //
            iStartMenu = menuStrip.Items.IndexOf(tsbStartMenu);
            WindowState = WindowState.Maximized;
            //加载菜单
            LoadMenu();
            //桌面
            _UC_Desktop = new UC_Desktop();
            _UC_Desktop.Name = _desktopName;
            _UC_Desktop.AllowDrop = true;
            _UC_Desktop.AddShortCutItem += AddShortCutMenuItem;
            
            //增加快捷菜单
            LoadShortCutMenu();
            //桌面tab页签
            _desktopTabItem = new TabItem();
            _desktopTabItem.Name = _desktopName;
            _desktopTabItem.Header = _desktopName;
            tcMenu.Items.Add(_desktopTabItem);
            tcMenu.SelectedItem = _desktopTabItem;
            //设置下拉框可编辑
            cbbMenuSearch.IsEditable = true;
        } 
        #endregion

        #region 加载菜单方法
        private void LoadMenu()
        {
            //加载菜单
            tvLeftMenu.Items.Clear();
            //初始化菜单管理菜单
            InitMenuManageMenu();
            //XML中的菜单处理
            _MenuDic = DMenu.GetAllMenu(AppType.WPFWindow);
            foreach (DMenu dMenu in _MenuDic.Values)
            {
                if (dMenu.MenuType != MenuTypeEnum.Modul)
                {
                    continue;
                }
                //菜单项
                MenuItem tmiNew = new MenuItem();
                //tmiNew.Name = dMenu.Guid;
                tmiNew.Header = dMenu.Name;
                tmiNew.Tag = dMenu;
                dMenu.FullPath = dMenu.Name;

                menuStrip.Items.Insert(iStartMenu + 1, tmiNew);

                //左边树
                TreeViewItem tnNew = new TreeViewItem();
                //tnNew.Name = dMenu.Guid;
                tnNew.Header = dMenu.Name;
                tnNew.Tag = dMenu;
                tvLeftMenu.Items.Add(tnNew);

                foreach (DMenu childMenu in dMenu.ChildMenu)
                {
                    if (childMenu.MenuType == MenuTypeEnum.Class)
                    {
                        AddMenuClassItem(tmiNew, new MenuItem(), childMenu);
                        AddMenuClassNode(tnNew, new TreeViewItem(), childMenu);
                    }
                    else
                    {
                        AddMenuItem(tmiNew, new MenuItem(), childMenu);
                        AddMenuNode(tnNew, new TreeViewItem(), childMenu);
                    }
                }

                iStartMenu++;
            }
        }
        #endregion

        #region 增加菜单分类
        private void AddMenuClassItem(MenuItem tmiParent, MenuItem tmiNew, DMenu dParentMenu)
        {
            AddMenuItem(tmiParent, tmiNew, dParentMenu);
            foreach (DMenu childMenu in dParentMenu.ChildMenu)
            {
                if (childMenu.MenuType == MenuTypeEnum.Class)
                {
                    AddMenuClassItem(tmiNew, new MenuItem(), childMenu);
                }
                else
                {
                    AddMenuItem(tmiNew, new MenuItem(), childMenu);
                }
            }
        }

        private void AddMenuClassNode(TreeViewItem tnParent, TreeViewItem tnNew, DMenu dParentMenu)
        {
            AddMenuNode(tnParent, tnNew, dParentMenu);
            foreach (DMenu childMenu in dParentMenu.ChildMenu)
            {
                if (childMenu.MenuType == MenuTypeEnum.Class)
                {
                    AddMenuClassNode(tnNew, new TreeViewItem(), childMenu);
                }
                else
                {
                    AddMenuNode(tnNew, new TreeViewItem(), childMenu);
                }
            }
        }
        #endregion

        #region 增加菜单
        private void AddMenuItem(MenuItem tmiParent, MenuItem tmiNew, DMenu dMenu)
        {
            tmiNew.Header = dMenu.Name;
            //tmiNew.Name = dMenu.Guid;
            tmiNew.Tag = dMenu;
            tmiNew.Click += MenuItem_Click;
            tmiParent.Items.Add(tmiNew);
            if (dMenu.MenuType == MenuTypeEnum.Menu)
            {
                //菜单查找自动完成数据源
                cbbMenuSearch.Items.Add(dMenu.Name);
            }
        }

        private void AddMenuNode(TreeViewItem tnParent, TreeViewItem tnNew, DMenu dMenu)
        {
            tnNew.Header = dMenu.Name;
            tnNew.Tag = dMenu;
            tnParent.Items.Add(tnNew);
        }
        #endregion

        #region 加载快捷菜单
        private void LoadShortCutMenu()
        {
            if (!Directory.Exists(_strConfigFilePath))
            {
                Directory.CreateDirectory(_strConfigFilePath);
            }
            string strShortCutFilePath = System.IO.Path.Combine(_strConfigFilePath, WorkHelperStaticString.ShortCutMenuFileName_WPF);
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
                _UC_Desktop.AddShortCutItem += AddShortCutMenuItem;//新增快捷菜单事件

                foreach (XmlNode xnModel in xmlList)
                {
                    if (xnModel.NodeType == XmlNodeType.Comment)
                    {
                        continue;
                    }
                    string strGuid = xnModel.GetAttributeValue("Guid");
                    if (_MenuDic.ContainsKey(strGuid))
                    {
                        var scItem = new UC_ShortCutItem(_MenuDic[strGuid]);
                        scItem.ShortCutItemClick += ShortCutMenuItem_Click;//点击快捷菜单事件
                        scItem.ShortCutItemCancel += CancelShortCutMenuItem_Click;//取消快捷菜单事件
                        _UC_Desktop.AddItem(scItem);
                    }
                }
                //
                //_ShortCutMenuList.Dock = DockStyle.Fill;
                //pnlDestop.Controls.Add(_ShortCutMenuList);

            }
        }
        #endregion

        #region 主窗体关闭中事件
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (MessageBox.Show("确定要关闭所有窗体退出？", "温馨提示", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.Cancel)
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
            tnFind = null;
            if (IsExpandTreeNode)
            {
                OpenTreeNodeMenu(dMenu.Name);
            }
            
            FrameworkElement feFind = null;
            //判断窗体是否已经打开
            foreach (FrameworkElement frm in childContainer.Children)
            {
                if (frm.Name == _desktopName)
                {
                    continue;
                }
                TabItem tiNew = frm.Tag as TabItem;
                DMenu dMenuFrm = tiNew.Tag as DMenu;
                if (dMenuFrm.Guid.Equals(dMenu.Guid))
                {
                    feFind = tiNew;
                    if (tcMenu.SelectedItem != tiNew)
                    {
                        tcMenu.SelectionChanged -= tcMenu_SelectionChanged;
                        tcMenu.SelectedItem = tiNew;
                        tcMenu.SelectionChanged += tcMenu_SelectionChanged;
                    }
                    txbMenuPath.Text = dMenu.FullPath;
                    frm.Visibility = Visibility.Visible;
                }
                else
                {
                    frm.Visibility = Visibility.Collapsed;
                }
            }

            if (feFind != null)
            {
                //移除桌面
                childContainer.Children.Remove(_UC_Desktop);
                return;
            }

            //反射得到窗体
            Assembly dll = Assembly.LoadFile(System.IO.Path.Combine(_strAppPath, dMenu.DLLName));
            object form = dll.CreateInstance(dMenu.FormName);
            if (form is FrameworkElement)
            {
                var newForm = form as FrameworkElement;

                if (form is Window)
                {
                    var newWindow = form as Window;
                    newWindow.ShowDialog();
                    return;
                }

                //移除桌面
                childContainer.Children.Remove(_UC_Desktop);
                //增加页签
                TabItem tiNew = new TabItem();
                tiNew.Header = dMenu.Name;
                tiNew.Tag = dMenu;
                tcMenu.Items.Add(tiNew);
                tcMenu.SelectionChanged -= tcMenu_SelectionChanged;
                tcMenu.SelectedItem = tiNew;
                tcMenu.SelectionChanged += tcMenu_SelectionChanged;

                
                newForm.Tag = tiNew;
                newForm.Name = dMenu.Name;
                childContainer.Children.Add(newForm);
                
                //菜单路径
                txbMenuPath.Text = dMenu.FullPath;
            } 
            else
            {
                WPFMsgHelper.ShowErr("配置错误，【" + dMenu.Name + "】菜单不是窗体类型！");
            }
        }
        #endregion

        #region 菜单项点击事件
        private void MenuItem_Click(object sender, EventArgs e)
        {
            MenuItem tsmiClick = sender as MenuItem;
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
            SaveShortCutMenuConfig(e.Menu, false);
        }
        #endregion

        #region 增加快捷菜单项点击事件
        private void AddShortCutMenuItem(object sender, ShortCutItemClickEventArgs e)
        {

            SaveShortCutMenuConfig(e.Menu, true);
        }
        #endregion

        #region 保存快捷菜单配置
        private void SaveShortCutMenuConfig(DMenu dMenu, bool IsAdd)
        {
            string strShortCutFilePath = System.IO.Path.Combine(_strConfigFilePath, WorkHelperStaticString.ShortCutMenuFileName_WPF);
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
                    if (_UC_Desktop.ItemList.ContainsKey(dMenu.Guid))
                    {
                        _UC_Desktop.MenuListPanl.Children.Remove(_UC_Desktop.ItemList[dMenu.Guid]);
                        _UC_Desktop.ItemList.Remove(dMenu.Guid);

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

                    if (!_UC_Desktop.ItemList.ContainsKey(dMenu.Guid))
                    {
                        var scItem = new UC_ShortCutItem(dMenu);
                        scItem.ShortCutItemClick += ShortCutMenuItem_Click;//点击快捷菜单事件
                        scItem.ShortCutItemCancel += CancelShortCutMenuItem_Click;//取消快捷菜单事件
                        _UC_Desktop.AddItem(scItem);
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
        private void tcMenu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TabItem tpSelect = tcMenu.SelectedItem as TabItem;
            if (tpSelect == null || tpSelect.Name == _desktopName)
            {
                if (!childContainer.Children.Contains(_UC_Desktop))
                {
                    childContainer.Children.Add(_UC_Desktop);
                }
                foreach (FrameworkElement frm in childContainer.Children)
                {
                    if (frm.Name == _desktopName)
                    {
                        continue;
                    }
                    frm.Visibility = Visibility.Collapsed;
                }
                txbMenuPath.Text = _desktopName;
            }
            else
            {
                var tapMenu = tpSelect.Tag as DMenu;
                childContainer.Children.Remove(_UC_Desktop);
                foreach (FrameworkElement frm in childContainer.Children)
                {
                    if (frm.Name == _desktopName)
                    {
                        continue;
                    }
                    var feTab = frm.Tag as TabItem;
                    var feMenu = feTab.Tag as DMenu;
                    if (tapMenu.Guid == feMenu.Guid)
                    {
                        frm.Visibility = Visibility.Visible;
                        txbMenuPath.Text = tapMenu.FullPath;
                    }
                    else
                    {
                        frm.Visibility = Visibility.Collapsed;
                    }
                }
            }
        }
        #endregion

        #region 双击打开树菜单
        private void tvLeftMenu_MouseDoubleClick(object sender, EventArgs e)
        {
            TreeViewItem tnSelect = tvLeftMenu.SelectedItem as TreeViewItem;
            if (tnSelect == null)
            {
                return;
            }

            //打开菜单
            OpenMenu(tnSelect.Tag as DMenu, false);
        }
        #endregion

        #region 树节点开始拖动事件
        private void tvLeftMenu_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var tviSelect = tvLeftMenu.SelectedItem as TreeViewItem;
                //DMenu dMenu = tviSelect.Tag as DMenu;
                // 开始进行拖放操作，并将拖放的效果设置成移动。
                DragDrop.DoDragDrop(tviSelect, e.Source, DragDropEffects.Move);
            }
        }
        #endregion

        #region 查找菜单回车键事件
        private void cbbMenuSearch_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                string strSearchMenu = cbbMenuSearch.Text.Trim();
                if (string.IsNullOrEmpty(strSearchMenu))
                {
                    return;
                }
                //打开树节点方法
                OpenTreeNodeMenu(strSearchMenu);
                if (tnFind != null)
                {
                    var mMemu = tnFind.Tag as DMenu;
                    OpenMenu(mMemu, false);//打开菜单
                }
            }
        }
        #endregion

        #region 打开树节点方法
        private void OpenTreeNodeMenu(string strSearchMenu)
        {
            foreach (TreeViewItem xn in tvLeftMenu.Items)
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
        private TreeViewItem FindNodeByText(TreeViewItem tnParent, string strText)
        {
            if (tnParent == null) return null;
            if (tnParent.Header.ToString() == strText) return tnParent;

            TreeViewItem tnRet = null;
            foreach (TreeViewItem tn in tnParent.Items)
            {
                tnRet = FindNodeByText(tn, strText);
                if (tnRet != null) break;
            }
            return tnRet;
        }
        #endregion

        #region 递归打开左边树节点方法
        private void ExpandParentNode(TreeViewItem tnParent)
        {
            tnParent.IsExpanded = true;//设置为展开
            if (tnParent.Parent != null && tnParent.Parent is TreeViewItem)
            {
                ExpandParentNode(tnParent.Parent as TreeViewItem);
            }
        }
        #endregion

        #region 隐藏左边树按钮事件
        private void btnHideTree_Click(object sender, EventArgs e)
        {
            if (tvLeftMenu.IsVisible)
            {
                tvLeftMenu.Visibility = Visibility.Collapsed;
                btnHideTree.Content = ">";
            }
            else
            {
                tvLeftMenu.Visibility = Visibility.Visible;
                btnHideTree.Content = "<";
            }
        }
        #endregion

        #region 关于菜单事件
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //AboutAuthor about = new AboutAuthor();
            //about.ShowDialog();
        }
        #endregion

        #region 子窗体关闭事件
        private void MdiChild_Close(object sender, EventArgs e)
        {
            var tapItem = tcMenu.SelectedItem as TabItem;
            if (tapItem.Header.ToString() == _desktopName)
            {
                return;
            }

            var tapMenu = tapItem.Tag as DMenu;

            FrameworkElement frmRemove = null; 
            foreach (FrameworkElement frm in childContainer.Children)
            {
                if (frm.Name == _desktopName)
                {
                    continue;
                }
                var feTab = frm.Tag as TabItem;
                var feMenu = feTab.Tag as DMenu;
                if (tapMenu.Guid == feMenu.Guid)
                {
                    frmRemove = frm;
                    break;
                }
            }

            if (frmRemove != null)
            {
                childContainer.Children.Remove(frmRemove);
                tcMenu.Items.Remove(tcMenu.SelectedItem);
            }

        }
        #endregion

        #region 菜单管理菜单
        private void InitMenuManageMenu()
        {
            //设置菜单类
            DMenu dMenuManage = new DMenu();
            dMenuManage.Guid = "G9D1EA9B-E422-4F22-88D8-A0509B14EC68";
            dMenuManage.MenuType = MenuTypeEnum.Menu;
            dMenuManage.DLLName = "Breezee.WorkHelper.WPF.StartUp.exe";
            dMenuManage.FormName = "Breezee.WorkHelper.WPF.StartUp.WMenuManage";
            dMenuManage.Name = "菜单管理";
            dMenuManage.FullPath = "开始 > 菜单管理";

            //设置菜单项
            MenuItem miMenu = new MenuItem();
            miMenu.Header = dMenuManage.Name;
            miMenu.Tag = dMenuManage;
            miMenu.Click += MenuItem_Click;
            tsbStartMenu.Items.Add(miMenu);
            //菜单查找自动完成数据源
            cbbMenuSearch.Items.Add(dMenuManage.Name);
            //左边树
            TreeViewItem tnBegin = new TreeViewItem();
            tnBegin.Header = tsbStartMenu.Header;
            tnBegin.Tag = null;
            tvLeftMenu.Items.Add(tnBegin);

            TreeViewItem tnMenuManage = new TreeViewItem();
            tnMenuManage.Header = dMenuManage.Name;
            tnMenuManage.Tag = dMenuManage;
            tnBegin.Items.Add(tnMenuManage);
        }

        #endregion

        
    }
}
