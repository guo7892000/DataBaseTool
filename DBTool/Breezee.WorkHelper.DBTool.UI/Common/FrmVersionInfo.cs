using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace Breezee.WorkHelper.DBTool.UI
{
    /// <summary>
    /// 创建作者：黄国辉
    /// 创建日期：2013-06-03
    /// 功能说明：查看和维护说明内容
    /// </summary>
    public partial class FrmVersionInfo : Form
    {
        //标题和正文
        private string _strTitle; //标题
        private string _strContentText; //正文
        private string _strFilePath; //文件路径
        private bool _isReadOnly = true; //是否只读，默认为是

        public FrmVersionInfo()
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="strTitle">标题</param>
        /// <param name="strRemark">正文</param>
        public FrmVersionInfo(string strTitle, string strContentText)
        {
            _strTitle = strTitle;
            _strContentText = strContentText;
            _strFilePath = "";
            _isReadOnly = true;
            InitializeComponent();
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="strTitle">标题</param>
        /// <param name="strFilePath">文件路径</param>
        /// <param name="isReadOnly">是否只读：true是，false否</param>
        public FrmVersionInfo(string strTitle, string strFilePath, bool isReadOnly)
        {
            _strTitle = strTitle;
            _strFilePath = strFilePath;
            _isReadOnly = isReadOnly;
            InitializeComponent();
        }

        /// <summary>
        /// 加载事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FrmVersionInfo_Load(object sender, EventArgs e)
        {
            Text = _strTitle;
            if (_isReadOnly)
            {
                rtbModifyHistory.AppendText(_strContentText);
                tsbSave.Visible = false;
            }
            else
            {
                tsbSave.Visible = true;
                if (string.IsNullOrEmpty(_strFilePath))
                {
                    return;
                }

                //2014-3-24 去掉文件的的只读属性
                FileInfo f = new FileInfo(_strFilePath);
                f.Attributes = FileAttributes.Normal;

                //文件流
                FileStream fs = new FileStream(_strFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
                //字符流
                StreamReader sr = new StreamReader(fs, Encoding.Default);
                _strContentText = sr.ReadToEnd();
                sr.Close();
                rtbModifyHistory.AppendText(_strContentText);
            }
            rtbModifyHistory.SelectionStart = 0;
        }

        /// <summary>
        /// 保存按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsbSave_Click(object sender, EventArgs e)
        {
            if (!_isReadOnly)
            {
                if (string.IsNullOrEmpty(_strFilePath))
                {
                    return;
                }

                //2014-3-24 去掉文件的的只读属性
                FileInfo f = new FileInfo(_strFilePath);
                f.Attributes = FileAttributes.Normal;

                StreamWriter sw = new StreamWriter(_strFilePath,false,Encoding.Default);
                sw.Write(rtbModifyHistory.Text);
                sw.Close();
                MessageBox.Show("保存成功！", "提示", MessageBoxButtons.OK);
            }
            else
            {
                MessageBox.Show("不能保存！", "提示", MessageBoxButtons.OK);
            }
        }

        /// <summary>
        /// 退出按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsbExit_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}