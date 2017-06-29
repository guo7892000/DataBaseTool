using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Forms;
using Breezee.WorkHelper.WinForm.StartUp;

namespace Breezee.WorkHelper.WPF.BrowserApp
{
    /// <summary>
    /// WWinForm.xaml 的交互逻辑
    /// </summary>
    public partial class WWinForm : Window
    {
        #region 变量
        public event EventHandler<EventArgs> FormClosed;
        
        #endregion

        #region 构造函数
        public WWinForm()
        {
            InitializeComponent();
            //启动WinForm
            StartWinForm();
        } 
        #endregion

        #region 启动WinForm
        private void StartWinForm()
        {
            var form = new FrmMainMDI();
            WindowInteropHelper helper = new WindowInteropHelper(this);
            form.StartPosition = FormStartPosition.CenterScreen;
            form.WindowState = FormWindowState.Maximized;
            form.FormClosed += Window_Closed;
            form.Show(new WindowWapper(helper.Handle));
        }
        #endregion

        #region 窗体关闭事件
        private void Window_Closed(object sender, EventArgs e)
        {
            if (FormClosed != null)
            {
                FormClosed(this, e);
            }
        } 
        #endregion
    }

    #region WindowWapper类
    public class WindowWapper : System.Windows.Forms.IWin32Window
    {
        private IntPtr _handle;

        public IntPtr Handle
        {
            get
            {
                return this._handle;
            }
        }

        public WindowWapper(IntPtr handle)
        {
            this._handle = handle;
        }
    }
    #endregion
}
