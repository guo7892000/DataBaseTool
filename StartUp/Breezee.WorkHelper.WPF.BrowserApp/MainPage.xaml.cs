using Breezee.WorkHelper.WPF.StartUp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Breezee.WorkHelper.WPF.BrowserApp
{
    /// <summary>
    /// Page1.xaml 的交互逻辑
    /// </summary>
    public partial class MainPage : Page
    {
        #region 变量
        private WWinForm _WWinForm = null;
        private MainWindow _WPFWindowMain = null;
        #endregion

        #region 构造函数
        public MainPage()
        {
            InitializeComponent();
        }
        #endregion

        #region 加载事件
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {

        }
        #endregion

        #region 打开WinForm窗体按钮事件
        private void btnWinForm_Click(object sender, RoutedEventArgs e)
        {
            if(_WWinForm == null)
            {
                _WWinForm = new WWinForm();
                _WWinForm.FormClosed += _WWinForm_Closed;
            }
        }

        private void _WWinForm_Closed(object sender, EventArgs e)
        {
            _WWinForm = null;
        }
        #endregion

        #region 打开WPF窗体按钮事件
        private void btnWPF_Click(object sender, RoutedEventArgs e)
        {
            if (_WPFWindowMain == null)
            {
                _WPFWindowMain = new MainWindow();
                _WPFWindowMain.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                _WPFWindowMain.WindowState = WindowState.Maximized;
                _WPFWindowMain.FormClosed += _WPFWindowMain_Closed;
                _WPFWindowMain.Show();
            }
        }

        private void _WPFWindowMain_Closed(object sender, EventArgs e)
        {
            _WPFWindowMain = null;
        }
        #endregion

        

    }
}
