using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Breezee.WorkHelper.Entity;

namespace Breezee.WorkHelper.WinForm.StartUp
{
    public partial class ShortCutItem : UserControl
    {
        public EventHandler<ShortCutItemClickEventArgs> ShortCutItemClick;
        public EventHandler<ShortCutItemClickEventArgs> ShortCutItemCancel;

        string strRootPath = AppDomain.CurrentDomain.BaseDirectory;
        string strInFilePah;
        string strOutFilePah;
        DMenu _Menu;

        public DMenu Menu
        {
            get { return _Menu; }
        }

        public ShortCutItem(DMenu dMenu)
        {
            InitializeComponent();
            _Menu = dMenu;

            lblMenuName.Text = dMenu.Name;
        }

        private void ShortCutItem_Load(object sender, EventArgs e)
        {
            strInFilePah = Path.Combine(strRootPath, "Image", "ShortCut_In.png");
            strOutFilePah = Path.Combine(strRootPath, "Image", "ShortCut_Out.png");
            this.BackgroundImage = Image.FromFile(strOutFilePah);

            this.Click += Item_Click;
            lblMenuName.Click += Item_Click;
        }

        private void Item_Click(object sender, EventArgs e)
        {
            if (_Menu != null && ShortCutItemClick!=null)
            {
                ShortCutItemClickEventArgs arg = new ShortCutItemClickEventArgs();
                arg.Menu = _Menu;
                ShortCutItemClick(this,arg);
            }
        }

        private void ShortCutItem_MouseEnter(object sender, EventArgs e)
        {
            this.BackgroundImage = Image.FromFile(strInFilePah);
            this.Cursor = Cursors.Hand;
        }

        private void ShortCutItem_MouseLeave(object sender, EventArgs e)
        {
            this.BackgroundImage = Image.FromFile(strOutFilePah);
        }

        private void tsmiCancelShortCutMenu_Click(object sender, EventArgs e)
        {
            if (_Menu != null && ShortCutItemCancel!=null)
            {
                ShortCutItemClickEventArgs arg = new ShortCutItemClickEventArgs();
                arg.Menu = _Menu;
                ShortCutItemCancel(this, arg);
            }
        }
    }

    public class ShortCutItemClickEventArgs : EventArgs
    {
        public DMenu Menu;
    }
}
