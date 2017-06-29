using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Breezee.WorkHelper.Entity;

namespace Breezee.WorkHelper.WinForm.StartUp
{
    public partial class ShortCutList : UserControl
    {
        Dictionary<string, ShortCutItem> _ItemList = new Dictionary<string, ShortCutItem>();

        public Dictionary<string, ShortCutItem> ItemList
        {
            get { return _ItemList; }
        }

        public FlowLayoutPanel MenuListPanl
        {
            get { return flpMenuList; }
        }

        public EventHandler<ShortCutItemClickEventArgs> AddShortCutItem;

        public ShortCutList()
        {
            InitializeComponent();
        }

        public void AddItem(ShortCutItem sci)
        {
            this.flpMenuList.Controls.Add(sci);
            _ItemList.Add(sci.Menu.Guid, sci);
        }

        private void ShortCutList_Load(object sender, EventArgs e)
        {
            flpMenuList.AllowDrop = true;
            flpMenuList.Dock = DockStyle.Fill;
        }

        private void flpMenuList_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }

        private void flpMenuList_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent("System.Windows.Forms.TreeNode", false))
            {
                // 获取被拖动的节点
                TreeNode treeNode = (TreeNode)e.Data.GetData("System.Windows.Forms.TreeNode");
                DMenu dMenu = treeNode.Tag as DMenu;
                if (!_ItemList.ContainsKey(dMenu.Guid))
                {
                    //触发快捷菜单保存
                    ShortCutItemClickEventArgs arg = new ShortCutItemClickEventArgs();
                    arg.Menu = dMenu;
                    AddShortCutItem(this, arg);
                }
            }
        }

    }
}
