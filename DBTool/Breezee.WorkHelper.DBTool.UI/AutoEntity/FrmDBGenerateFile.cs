using Breezee.WorkHelper.DBTool.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Breezee.Framework.BaseUI;

namespace Breezee.WorkHelper.DBTool.UI
{
    /// <summary>
    /// 生成程序文件
    /// 测试结果：通过
    /// </summary>
    public partial class FrmDBGenerateFile : BaseForm
    {
        #region 变量
        string _strFileList = "类清单";
        string _strLayerList = "项目分层清单";
        DataSet dsExcel;
        List<EntityInfo> list = new List<EntityInfo>();
        #endregion

        #region 构造函数
        public FrmDBGenerateFile()
        {
            InitializeComponent();
        }
        #endregion

        #region 加载事件
        private void FrmDBGenerateFile_Load(object sender, EventArgs e)
        {

        } 
        #endregion

        #region 导入生成IBD文件模板文件
        private void btnImportAutoFile_Click(object sender, EventArgs e)
        {
            string strLayerNameSql = @"SELECT 分层类型,项目分层简称,项目分层全称
                     FROM [项目分层清单$] where [序号] is not null order by [序号]";

            string strFileListSql = @"SELECT 分层类型,项目分层全称,文件目录相对路径,类名
                     FROM [目录及类名$] where [序号] is not null and 类名 is not null order by [序号]";


            string _DBConnString = "";

            #region 打开显示选择文件对话框，获取导入的文件名
            OpenFileDialog opd = new OpenFileDialog();
            opd.Filter = "Excel文件(*.xls,*.xlsx)|*.xls;*.xlsx";  //支持2003、2007以上格式的Excel
            //opd.Filter = "Excel文件(*.xlsx)|*.xlsx"; //只支持2007以上格式的Excel
            opd.FilterIndex = 0;
            opd.Title = "请选择“生成IBD文件导入模板”";
            opd.RestoreDirectory = false;
            if (DialogResult.Cancel == opd.ShowDialog())
            {
                return;
            }
            string sFilePath = opd.FileName;
            string[] strFileNam = sFilePath.Split('.');
            #endregion

            #region 导入模板后处理
            string strFileFormart = strFileNam[strFileNam.Length - 1].ToString().ToLower();
            if (strFileFormart == "xls")
            {
                _DBConnString = @"Provider=Microsoft.jet.OleDb.4.0;Data Source=" + sFilePath + ";Extended Properties='Excel 8.0;IMEX=1'";
            }
            else if (strFileFormart == "xlsx")
            {
                _DBConnString = @"Provider=Microsoft.ACE.OLEDB.12.0; Data Source=" + sFilePath + "; Extended Properties='Excel 12.0 Xml;HDR=YES;IMEX=1'";
            }
            using (OleDbConnection con = new OleDbConnection(_DBConnString))
            {
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                try
                {
                    dsExcel = new DataSet();
                    BindingSource bsLayerName = new BindingSource();
                    BindingSource bsFileList = new BindingSource();
                    OleDbDataAdapter daLayerName = new OleDbDataAdapter(strLayerNameSql, con);
                    OleDbDataAdapter daFileList = new OleDbDataAdapter(strFileListSql, con);
                    //打开连接并填充表
                    daLayerName.Fill(dsExcel, _strLayerList);
                    daFileList.Fill(dsExcel, _strFileList);

                    bsLayerName.DataSource = dsExcel.Tables[_strLayerList];
                    dgvLayerName.DataSource = bsLayerName;

                    bsFileList.DataSource = dsExcel.Tables[_strFileList];
                    dgvIBDFileList.DataSource = bsFileList;

                }
                catch (Exception ex)
                {
                    MsgHelper.ShowErr(ex.Message);
                }
            }
            #endregion
        }
        #endregion

        #region 生成IBD文件
        private void tsbGenIBDFile_Click(object sender, EventArgs e)
        {
            try
            {
                if (dsExcel == null)
                {
                    MsgHelper.ShowInfo("请先导入！");
                    return;
                }
                DataTable dtMain = (DataTable)(dgvIBDFileList.DataSource as BindingSource).DataSource;
                DataTable dtList = dsExcel.Tables[_strLayerList];
                string strMainPath = txbIBDMainPath.Text.Trim();
                if (string.IsNullOrEmpty(strMainPath))
                {
                    MsgHelper.ShowInfo("请选择“生成的根目录”！");
                    return;
                }
                string strClassPre = txbIBDClassPre.Text.Trim().ToUpper();
                list = new List<EntityInfo>();
                strMainPath = strMainPath + @"\IBD自动生成文件夹\";
                for (int i = 0; i < dtMain.Rows.Count; i++)
                {
                    string strPath = dtMain.Rows[i]["文件目录相对路径"].ToString().Replace("/", "\\");
                    string strFromLayerName = dtMain.Rows[i]["项目分层全称"].ToString().Trim();
                    string strLayerType = dtMain.Rows[i]["分层类型"].ToString().Trim();
                    DataRow[] drFrom = dtList.Select("项目分层全称 = '" + strFromLayerName + "' AND 分层类型='" + strLayerType + "'");
                    if (drFrom.Length == 0)
                    {
                        MsgHelper.ShowInfo("“" + strPath + "”的项目分层全称未定义，请修改或在[项目分层清单]里增加！");
                        return;
                    }
                    string strSpaceName = strPath.Replace("\\", ".");//空间名
                    string strClassName = strClassPre + dtMain.Rows[i]["类名"].ToString();

                    if (!strPath.EndsWith("\\"))
                    {
                        strPath += "\\";
                    }

                    //当前使用的项目层集合
                    DataTable dtUseLaye = dtList.Clone();
                    foreach (DataRow dr in dtList.Select("分层类型='" + drFrom[0]["分层类型"] + "'"))
                    {
                        dtUseLaye.ImportRow(dr);
                    }

                    
                    string Ipath = strMainPath + ReplaceString(strPath, "0", "I", strFromLayerName, dtUseLaye);
                    string Bpath = strMainPath + ReplaceString(strPath, "0", "B", strFromLayerName, dtUseLaye);
                    string Dpath = strMainPath + ReplaceString(strPath, "0", "D", strFromLayerName, dtUseLaye);
                    string IDpath = strMainPath + ReplaceString(strPath, "0", "ID", strFromLayerName, dtUseLaye);
                    string UIpath = strMainPath + ReplaceString(strPath, "0", "U", strFromLayerName, dtUseLaye);

                    string ISpaceName = ReplaceString(strSpaceName + ".", "1", "I", strFromLayerName, dtUseLaye);
                    string BSpaceName = ReplaceString(strSpaceName + ".", "1", "B", strFromLayerName, dtUseLaye);
                    string DSpaceName = ReplaceString(strSpaceName + ".", "1", "D", strFromLayerName, dtUseLaye);
                    string IDSpaceName = ReplaceString(strSpaceName + ".", "1", "ID", strFromLayerName, dtUseLaye);
                    string UISpaceName = ReplaceString(strSpaceName + ".", "1", "U", strFromLayerName, dtUseLaye);

                    if (!Directory.Exists(Ipath))
                    {
                        Directory.CreateDirectory(Ipath);
                    }
                    if (!Directory.Exists(Bpath))
                    {
                        Directory.CreateDirectory(Bpath);
                    }
                    if (!Directory.Exists(IDpath))
                    {
                        Directory.CreateDirectory(IDpath);
                    }
                    if (!Directory.Exists(Dpath))
                    {
                        Directory.CreateDirectory(Dpath);
                    }
                    if (!Directory.Exists(UIpath))
                    {
                        Directory.CreateDirectory(UIpath);
                    }
                    //生成接口文件
                    GenerateIFile(Ipath, ISpaceName, strClassName);//I
                    GenerateBFile(Bpath, BSpaceName, strClassName, strFromLayerName, dtUseLaye);//B
                    GenerateIDFile(IDpath, IDSpaceName, strClassName);//ID
                    GenerateDFile(Dpath, DSpaceName, strClassName);//D
                    GenerateUIFile(UIpath, UISpaceName, strClassName, strFromLayerName, dtUseLaye);//UI
                }
                //提示成功
                MsgHelper.ShowInfo("生成成功！");
            }
            catch (Exception ex)
            {
                MsgHelper.ShowErr(ex.Message);
            }
        }
        #endregion

        #region 替换字符私有方法
        /// <summary>
        /// 替换字符私有方法
        /// </summary>
        /// <param name="sourceString"></param>
        /// <param name="FlieType"></param>
        /// <param name="ClassType"></param>
        /// <param name="dtUseLaye"></param>
        /// <returns></returns>
        private string ReplaceString(string sourceString, string FlieType, string ClassType, string OldString, DataTable dtUseLaye)
        {
            //得到转换后的类
            DataRow[] drTo = dtUseLaye.Select("项目分层简称 = '" + ClassType + "'");
            if (drTo.Length == 0)
            {
                throw new Exception("“" + ClassType + "”的项目分层简称未定义，请修改或在[项目分层清单]里增加！");
            }

            if (FlieType == "0") //路径
            {
                string strReturn = sourceString;
                string strNewStr = "." + drTo[0]["项目分层全称"].ToString() + "\\";
                foreach (DataRow dr in dtUseLaye.Rows)
                {
                    strReturn = strReturn.Replace("." + dr["项目分层全称"] + "\\", strNewStr);
                }
                return strReturn;
            }
            else //空间名
            {
                string strReturn = sourceString;
                string strReplaceStr = "." + drTo[0]["项目分层全称"].ToString() + ".";
                foreach (DataRow dr in dtUseLaye.Rows)
                {
                    strReturn = strReturn.Replace("." + dr["项目分层全称"] + ".", strReplaceStr);
                }
                return strReturn;
            }
        }
        #endregion

        #region 生成文件方法
        private void GenerateIFile(string Ipath, string nameSpace, string ClassName)
        {
            string IClassName = "I" + ClassName;

            nameSpace = RemoveLastDot(nameSpace);
            using (TextWriter writer = new StreamWriter(new BufferedStream(new FileStream(Ipath + IClassName + ".cs", FileMode.Create, FileAccess.Write)), Encoding.GetEncoding("gbk")))
            {
                writer.WriteLine("using System;");
                writer.WriteLine("using System.Collections.Generic;");
                writer.WriteLine("using System.Text;");
                writer.WriteLine("using Breezee.Framework.DataAccess.INF;");
                writer.WriteLine();
                writer.WriteLine("namespace " + nameSpace);
                writer.WriteLine("{");
                writer.WriteLine("\tpublic interface " + IClassName+ " : IBaseBUS");//继承一个基本的BUS接口，方便扩展功能
                writer.WriteLine("\t{");
                writer.WriteLine("\t");
                writer.WriteLine("\t}");
                writer.WriteLine("}");
                list.Add(new EntityInfo(nameSpace, "public", IClassName, Ipath + IClassName + ".cs"));
            }
        }

        private void GenerateBFile(string Bpath, string nameSpace, string ClassName, string OleString, DataTable dtUseLaye)
        {
            string IClassName = "I" + ClassName;
            string INameSpace = ReplaceString(nameSpace, "1", "I", OleString, dtUseLaye);
            string DClassName = "D" + ClassName;
            string DNameSpace = ReplaceString(nameSpace, "1", "D", OleString, dtUseLaye);
            string BClassName = "B" + ClassName;

            INameSpace = RemoveLastDot(INameSpace);
            DNameSpace = RemoveLastDot(DNameSpace);
            nameSpace = RemoveLastDot(nameSpace);

            using (TextWriter writer = new StreamWriter(new BufferedStream(new FileStream(Bpath + BClassName + ".cs", FileMode.Create, FileAccess.Write)), Encoding.GetEncoding("gbk")))
            {

                writer.WriteLine("using System;");
                writer.WriteLine("using System.Collections.Generic;");
                writer.WriteLine("using System.Text;");
                writer.WriteLine("using Breezee.Framework.DataAccess.INF;");//公共数据访问类
                writer.WriteLine("using Breezee.Framework.Tool;");//工具类
                writer.WriteLine("using Breezee.Global.Context;");//IOC容器类
                writer.WriteLine("using Breezee.Global.Entity;");//公共实体类
                writer.WriteLine("using " + INameSpace + ";");
                writer.WriteLine("using " + DNameSpace + ";");
                writer.WriteLine();
                writer.WriteLine("namespace " + nameSpace);
                writer.WriteLine("{");
                writer.WriteLine("\tpublic class " + BClassName + " : BaseBUS," + IClassName);//继承一个实现了BUS接口的基本类，方便扩展功能
                writer.WriteLine("\t{");
                writer.WriteLine("\t");
                writer.WriteLine("\t}");
                writer.WriteLine("}");
                list.Add(new EntityInfo(nameSpace, "public", BClassName, Bpath + BClassName + ".cs"));
            }
        }

        private static string RemoveLastDot(string nameSpace)
        {
            if (nameSpace.EndsWith("."))
            {
                nameSpace = nameSpace.Substring(0, nameSpace.Length - 1);
            }
            return nameSpace;
        }


        private void GenerateDFile(string Dpath, string nameSpace, string ClassName)
        {
            string DClassName = "D" + ClassName;

            nameSpace = RemoveLastDot(nameSpace);
            using (TextWriter writer = new StreamWriter(new BufferedStream(new FileStream(Dpath + DClassName + ".cs", FileMode.Create, FileAccess.Write)), Encoding.GetEncoding("gbk")))
            {
                writer.WriteLine("using System;");
                writer.WriteLine("using System.Collections.Generic;");
                writer.WriteLine("using System.Text;");
                writer.WriteLine("using System.Linq;");
                writer.WriteLine("using System.Data;");
                writer.WriteLine("using Breezee.Framework.DataAccess.INF;");//公共数据访问类
                writer.WriteLine("using Breezee.Framework.Tool;");//工具类
                writer.WriteLine("using Breezee.Global.Context;");//IOC容器类
                writer.WriteLine("using Breezee.Global.Entity;");//公共实体类
                writer.WriteLine();
                writer.WriteLine("namespace " + nameSpace);
                writer.WriteLine("{");
                writer.WriteLine("\tpublic class " + DClassName+ " : BaseDAC,I"+ DClassName);
                writer.WriteLine("\t{");
                writer.WriteLine("\t");
                writer.WriteLine("\t}");
                writer.WriteLine("}");
                list.Add(new EntityInfo(nameSpace, "public", DClassName, Dpath + ClassName + ".cs"));
            }
        }

        private void GenerateIDFile(string Dpath, string nameSpace, string ClassName)
        {
            string DClassName = "ID" + ClassName;

            nameSpace = RemoveLastDot(nameSpace);
            using (TextWriter writer = new StreamWriter(new BufferedStream(new FileStream(Dpath + DClassName + ".cs", FileMode.Create, FileAccess.Write)), Encoding.GetEncoding("gbk")))
            {
                writer.WriteLine("using System;");
                writer.WriteLine("using System.Collections.Generic;");
                writer.WriteLine("using System.Text;");
                writer.WriteLine("using System.Linq;");
                writer.WriteLine("using System.Data;");
                writer.WriteLine("using Breezee.Framework.DataAccess.INF;");//公共数据访问类
                writer.WriteLine();
                writer.WriteLine("namespace " + nameSpace);
                writer.WriteLine("{");
                writer.WriteLine("\tpublic interface " + DClassName + " : IBaseDAC");
                writer.WriteLine("\t{");
                writer.WriteLine("\t");
                writer.WriteLine("\t}");
                writer.WriteLine("}");
                list.Add(new EntityInfo(nameSpace, "public", DClassName, Dpath + ClassName + ".cs"));
            }
        }

        private void GenerateUIFile(string UIpath, string nameSpace, string ClassName, string OleString, DataTable dtUseLaye)
        {
            string UIClassName = "Frm" + ClassName;
            string INameSpace = ReplaceString(nameSpace, "1", "I", OleString, dtUseLaye);
            string DClassName = "D" + ClassName;
            string UINameSpace = ReplaceString(nameSpace, "1", "U", OleString, dtUseLaye);
            string BClassName = "B" + ClassName;

            INameSpace = RemoveLastDot(INameSpace);
            UINameSpace = RemoveLastDot(UINameSpace);
            nameSpace = RemoveLastDot(nameSpace);
            //生成FrmXX.cs文件
            using (TextWriter writer = new StreamWriter(new BufferedStream(new FileStream(UIpath + UIClassName + ".cs", FileMode.Create, FileAccess.Write)), Encoding.GetEncoding("gbk")))
            {
                writer.WriteLine("using System;");
                writer.WriteLine("using System.Collections.Generic;");
                writer.WriteLine("using System.Text;");
                writer.WriteLine("using System.ComponentModel;");
                writer.WriteLine("using System.Data;");
                writer.WriteLine("using System.Drawing;");
                writer.WriteLine("using System.Linq;");
                writer.WriteLine("using System.Windows.Forms;");
                //以下为非系统的空间
                writer.WriteLine("using Breezee.Framework.BaseUI;");//UI基类
                writer.WriteLine("using Breezee.Framework.DataAccess.INF;");//公共数据访问类
                writer.WriteLine("using Breezee.Framework.Tool;");//工具类
                writer.WriteLine("using Breezee.Global.Context;");//IOC容器类
                writer.WriteLine("using Breezee.Global.Entity;");//公共实体类
                //动态空间名
                writer.WriteLine("using " + INameSpace + ";");
                writer.WriteLine();
                writer.WriteLine("namespace " + UINameSpace);
                writer.WriteLine("{");
                writer.WriteLine("\tpublic partial class " + UIClassName + " : BaseForm");
                writer.WriteLine("\t{");
                writer.WriteLine("\t\tpublic " + UIClassName + "()");
                writer.WriteLine("\t\t{");
                writer.WriteLine("\t\t\tInitializeComponent();");
                writer.WriteLine("\t\t}");
                writer.WriteLine("\t}");
                writer.WriteLine("}");
                list.Add(new EntityInfo(nameSpace, "public", UIClassName, UIpath + UIClassName + ".cs"));
            }

            //生成FrmXX.Designer.cs文件
            using (TextWriter writer = new StreamWriter(new BufferedStream(new FileStream(UIpath + UIClassName + ".Designer.cs", FileMode.Create, FileAccess.Write)), Encoding.GetEncoding("gbk")))
            {
                writer.WriteLine("namespace " + UINameSpace);
                writer.WriteLine("{");
                writer.WriteLine("\tpartial class " + UIClassName);
                writer.WriteLine("\t{");

                writer.WriteLine("\t\t/// <summary>");
                writer.WriteLine("\t\t/// Required designer variable.");
                writer.WriteLine("\t\t/// </summary>");
                writer.WriteLine("\t\tprivate System.ComponentModel.IContainer components = null;");
                writer.WriteLine("");

                writer.WriteLine("\t\t/// <summary>");
                writer.WriteLine("\t\t/// Clean up any resources being used.");
                writer.WriteLine("\t\t/// </summary>");
                writer.WriteLine("\t\t/// <param name=\"disposing\">true if managed resources should be disposed; otherwise, false.</param>");
                writer.WriteLine("\t\tprotected override void Dispose(bool disposing)");
                writer.WriteLine("\t\t{");
                writer.WriteLine("\t\t\tif (disposing && (components != null))");
                writer.WriteLine("\t\t\t{");
                writer.WriteLine("\t\t\t\tcomponents.Dispose();");
                writer.WriteLine("\t\t\t}");
                writer.WriteLine("\t\t\tbase.Dispose(disposing);");
                writer.WriteLine("\t\t}");
                writer.WriteLine("");

                writer.WriteLine("\t\t#region Windows Form Designer generated code");
                writer.WriteLine("\t\t/// <summary>");
                writer.WriteLine("\t\t/// Required method for Designer support - do not modify");
                writer.WriteLine("\t\t/// the contents of this method with the code editor.");
                writer.WriteLine("\t\t/// </summary>");
                writer.WriteLine("\t\tprivate void InitializeComponent()");
                writer.WriteLine("\t\t{");
                writer.WriteLine("\t\t\tthis.components = new System.ComponentModel.Container();");
                writer.WriteLine("\t\t\tthis.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;");
                writer.WriteLine("\t\t\tthis.Text = \"" + UIClassName + "\";");
                writer.WriteLine("\t\t}");
                writer.WriteLine("\t\t#endregion");
                writer.WriteLine("");

                writer.WriteLine("\t}");
                writer.WriteLine("}");
                list.Add(new EntityInfo(nameSpace, "public", UIClassName, UIpath + UIClassName + ".Designer.cs"));
            }
        }
        #endregion

        #region 选择生成IBD文件目录
        private void btnIBDSelectPath_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                this.txbIBDMainPath.Text = dialog.SelectedPath;
            }
        }
        #endregion

        #region 模板下载事件
        private void llModelDown_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                SaveFileDialog saveDialog = new SaveFileDialog();
                saveDialog.DefaultExt = "xlsx";
                saveDialog.Filter = "Excel文件(*.xlsx)|*.xlsx";
                saveDialog.FilterIndex = 1;
                if (saveDialog.ShowDialog(this) == DialogResult.OK)
                {
                    string fileName = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase.ToString() + (@"\ModelFile\生成IBD文件导入模板.xlsx");
                    string destFileName = saveDialog.FileName;

                    if (System.IO.File.Exists(fileName))
                    {
                        System.IO.File.Copy(fileName, destFileName);
                        MsgHelper.ShowInfo(string.Format("下载成功，文件保存在 {0}", destFileName));
                    }
                    else
                    {
                        MsgHelper.ShowErr("模板文件不存在，下载失败！");
                    }
                }
            }
            catch (Exception ex)
            {
                MsgHelper.ShowErr("下载失败，出现异常！" + ex.Message);
            }
        }
        #endregion

        #region 退出按钮事件
        private void btnExit_Click(object sender, EventArgs e)
        {
            Close();
        } 
        #endregion
    }
}
