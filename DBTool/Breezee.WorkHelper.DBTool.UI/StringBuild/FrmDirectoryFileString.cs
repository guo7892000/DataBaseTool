using Breezee.Framework.BaseUI;
using Breezee.Framework.Tool;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Breezee.WorkHelper.DBTool.UI
{
    public partial class FrmDirectoryFileString : BaseForm
    {
        #region 变量

        #endregion

        #region 构造函数
        public FrmDirectoryFileString()
        {
            InitializeComponent();
        }

        #endregion

        #region 加载事件
        private void FrmDirectoryFileString_Load(object sender, EventArgs e)
        {
            _dicString.Add("1", "仅文件");
            _dicString.Add("2", "仅目录");
            _dicString.Add("3", "目录和文件");
            UIHelper.BindTypeValueDropDownList(cbbFileType, _dicString.GetTextValueTable(false), false, true);
            //
            _dicString.Clear();
            _dicString.Add("1", "全路径");
            _dicString.Add("2", "仅文件名");
            _dicString.Add("3", "相对路径");
            UIHelper.BindTypeValueDropDownList(cbbPathType, _dicString.GetTextValueTable(false), false, true);
        } 
        #endregion

        #region 选择路径按钮事件
        private void btnSelectPath_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                txbSelectPath.Text = fbd.SelectedPath;
            }
        } 
        #endregion

        #region 生成SQL按钮事件
        private void tsbAutoSQL_Click(object sender, EventArgs e)
        {
            string sPath = txbSelectPath.Text.Trim();
            if (string.IsNullOrEmpty(sPath))
            {
                MsgHelper.ShowErr("请选择路径！");
            }
            rtbString.Clear();
            StringBuilder sb = new StringBuilder();
            DirectoryInfo rootDirectory = new DirectoryInfo(sPath);
            GetDirectoryFile(sb, rootDirectory, cbbFileType.SelectedValue.ToString(), cbbPathType.SelectedValue.ToString(), !ckbIgnorChildDir.Checked);
            rtbString.AppendText(sb.ToString());
        } 
        #endregion

        #region 退出按钮事件
        private void tsbExit_Click(object sender, EventArgs e)
        {
            Close();
        } 
        #endregion

        #region 获取目录文件方法
        private void GetDirectoryFile(StringBuilder sb, DirectoryInfo rootDirectory, string strOutType, string IsFullPath, bool IsSearchChild)
        {
            if (strOutType == "1" || strOutType == "3")
            {
                foreach (var file in rootDirectory.GetFiles()) //仅文件、目录和文件 
                {
                    if (file.Attributes == FileAttributes.System || file.Attributes == FileAttributes.Temporary || file.Attributes == FileAttributes.Hidden)
                    {
                        continue;
                    }
                    if (IsFullPath == "1")//全路径
                    {
                        sb.Append(file.FullName + "\n");
                    }
                    else if (IsFullPath == "2")//仅文件
                    {
                        sb.Append(file.Name + "\n");
                    }
                    else//相对路径
                    {
                        sb.Append(file.FullName.Replace(txbSelectPath.Text + "\\", "") + "\n");
                    }
                }

                if (IsSearchChild && strOutType == "1")
                {
                    foreach (var path in rootDirectory.GetDirectories())
                    {
                        GetDirectoryFile(sb, path, strOutType, IsFullPath, IsSearchChild);
                    }
                }
            }

            if (strOutType == "2" || strOutType == "3") //仅目录、目录和文件 
            {
                foreach (var path in rootDirectory.GetDirectories())
                {
                    if (IsFullPath == "1")//全路径
                    {
                        sb.Append(path.FullName + "\n");
                    }
                    else if (IsFullPath == "2")//仅文件
                    {
                        sb.Append(path.Name + "\n");
                    }
                    else//相对路径
                    {
                        sb.Append(path.FullName.Replace(txbSelectPath.Text + "\\", "") + "\n");
                    }

                    if (IsSearchChild)
                    {
                        GetDirectoryFile(sb, path, strOutType, IsFullPath, IsSearchChild);
                    }
                }
            }
        } 
        #endregion
    }
}
