using Breezee.WorkHelper.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
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

namespace Breezee.WorkHelper.WPF.StartUp
{
    /// <summary>
    /// UC_Desktop.xaml 的交互逻辑
    /// </summary>
    public partial class UC_Desktop : UserControl
    {
        Dictionary<string, UC_ShortCutItem> _ItemList = new Dictionary<string, UC_ShortCutItem>();

        public Dictionary<string, UC_ShortCutItem> ItemList
        {
            get { return _ItemList; }
        }

        public WrapPanel MenuListPanl
        {
            get { return flpMenuList; }
        }

        public EventHandler<ShortCutItemClickEventArgs> AddShortCutItem;
        public UC_Desktop()
        {
            InitializeComponent();
        }

        public void AddItem(UC_ShortCutItem sci)
        {
            this.flpMenuList.Children.Add(sci);
            _ItemList.Add(sci.Menu.Guid, sci);
        }

        private void UC_Desktop_Load(object sender, EventArgs e)
        {
            flpMenuList.AllowDrop = true;
            //flpMenuList.Dock = DockStyle.Fill;
        }

        private void flpMenuList_DragEnter(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.Move;
        }

        private void flpMenuList_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent("System.Windows.Controls.TreeViewItem", false))
            {
                // 获取被拖动的节点
                var treeNode = (TreeViewItem)e.Data.GetData("System.Windows.Controls.TreeViewItem");
                var dMenu = treeNode.Tag as DMenu;
                if (!_ItemList.ContainsKey(dMenu.Guid))
                {
                    //触发快捷菜单保存
                    var arg = new ShortCutItemClickEventArgs();
                    arg.Menu = dMenu;
                    AddShortCutItem(this, arg);
                }
            }
        }
    }
}
