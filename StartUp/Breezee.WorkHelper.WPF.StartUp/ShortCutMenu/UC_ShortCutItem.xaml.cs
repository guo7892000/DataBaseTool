using Breezee.WorkHelper.Entity;
using Breezee.WorkHelper.Tool;
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
    /// UC_ShortCutItem.xaml 的交互逻辑
    /// </summary>
    public partial class UC_ShortCutItem : UserControl
    {
        #region 全局变量
        public EventHandler<ShortCutItemClickEventArgs> ShortCutItemClick;
        public EventHandler<ShortCutItemClickEventArgs> ShortCutItemCancel;

        string strRootPath = AppDomain.CurrentDomain.BaseDirectory;
        string strInFilePah;
        string strOutFilePah;
        DMenu _Menu;
        #endregion
        public DMenu Menu
        {
            get { return _Menu; }
        }

        #region 构造函数
        public UC_ShortCutItem(DMenu dMenu)
        {
            InitializeComponent();
            //初始化变量
            _Menu = dMenu;
            tbkShortCut.Content = dMenu.Name;
        }
        #endregion

        #region 加载事件
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            //设置图片
            strInFilePah = System.IO.Path.Combine(strRootPath, "Image", "ShortCut_In.png");
            strOutFilePah = System.IO.Path.Combine(strRootPath, "Image", "ShortCut_Out.png");
            Background = WPFImageHelper.GetImage(strOutFilePah);

            MouseLeftButtonDown += Item_Click;
            //tbkShortCut.MouseLeftButtonDown += Item_Click;
        } 
        #endregion

        private void UserControl_MouseEnter(object sender, MouseEventArgs e)
        {
            Background = WPFImageHelper.GetImage(strInFilePah);
            this.Cursor = Cursors.Hand;
        }

        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            Background = WPFImageHelper.GetImage(strOutFilePah);
        }

        private void Item_Click(object sender, EventArgs e)
        {
            if (_Menu != null && ShortCutItemClick != null)
            {
                ShortCutItemClickEventArgs arg = new ShortCutItemClickEventArgs();
                arg.Menu = _Menu;
                ShortCutItemClick(this, arg);
            }
        }
        private void miCancel_Click(object sender, EventArgs e)
        {
            if (_Menu != null && ShortCutItemCancel != null)
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
