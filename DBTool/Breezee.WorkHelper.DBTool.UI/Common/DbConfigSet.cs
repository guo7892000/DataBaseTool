using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Xml.Linq;
using System.IO;
using Breezee.Global.Entity;

namespace Breezee.WorkHelper.DBTool.UI
{
    /// <summary>
    /// 对象名称：数据库连接维护窗体
    /// 创建作者：黄国辉
    /// 创建日期：2013-10-19
    /// 说明：对固定格式的XML文件维护类。
    /// </summary>
    public class DbConfigSet
    {
        private string _OrlFileName = "./Config/DBConfig.xml"; //新增修改的默认值设置
        private string _strRoot = "Items"; //根节点
        private string _strElement = "Item"; //子节点
        //节点的项名
        private string _strID = "ID";
        private string _strDbConfigName = "DbConfigName";
        private string _strDbType = "DbType"; //DB类型
        private string _strServerIP = "ServerIP"; //IP或TnsName
        private string _strDbName = "DbName"; //数据库名称
        private string _strUserName = "UserName"; //键
        private string _strPassword = "Password"; //键
        private string _strFileName;
        //
        private DataTable dt;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="strDBType">数据库类型</param>
        /// <param name="strDealType">处理类型</param>
        public DbConfigSet()
        {
            _strFileName = _OrlFileName;
        }
        
        /// <summary>
        /// 加载文件
        /// </summary>
        /// <returns></returns>
        public DataTable LoadXMLFile()
        {
            dt = GenerateDataStruct();

            if (File.Exists(_strFileName))
            {
                //2014-3-24 去掉文件的的只读属性
                FileInfo f = new FileInfo(_strFileName);
                f.Attributes = FileAttributes.Normal;

                XDocument doc = XDocument.Load(_strFileName);
                foreach (XElement ele in doc.Root.Elements(_strElement))
                {
                    DataRow row = dt.NewRow();
                    row[_strID] = (string)ele.Element(_strID);
                    row[_strDbConfigName] = (string)ele.Element(_strDbConfigName);
                    row[_strDbType] = (string)ele.Element(_strDbType);
                    row[_strServerIP] = (string)ele.Element(_strServerIP);
                    row[_strDbName] = (string)ele.Element(_strDbName);
                    row[_strUserName] = (string)ele.Element(_strUserName);
                    row[_strPassword] = (string)ele.Element(_strPassword);
                    dt.Rows.Add(row);
                }
            }

            return dt;
        }

        /// <summary>
        /// 生成表结构
        /// </summary>
        /// <returns></returns>
        public DataTable GenerateDataStruct()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add(_strID, typeof(string));
            dt.Columns.Add(_strDbConfigName, typeof(string));
            dt.Columns.Add(_strDbType, typeof(string));
            dt.Columns.Add(_strServerIP, typeof(string));
            dt.Columns.Add(_strDbName, typeof(string));
            dt.Columns.Add(_strUserName, typeof(string));
            dt.Columns.Add(_strPassword, typeof(string));
            dt.PrimaryKey = new DataColumn[] { dt.Columns[_strID] };
            return dt;
        }

        /// <summary>
        /// 保存XML文件方法
        /// </summary>
        /// <param name="dtItems"></param>
        public void SaveXMLFile(DataTable dtItems, string strFileName)
        {
            if (dtItems == null)
            {
                return;
            }
            List<XElement> keyItems = new List<XElement>(dtItems.Rows.Count);
            foreach (DataRow row in dtItems.Rows)
            {
                keyItems.Add
                (
                    new XElement
                    (
                        _strElement,
                        new XElement(_strID, row[_strID].ToString()),
                        new XElement(_strDbConfigName, row[_strDbConfigName].ToString()),
                        new XElement(_strDbType, row[_strDbType].ToString()),
                        new XElement(_strServerIP, row[_strServerIP].ToString()),
                        new XElement(_strDbName, row[_strDbName].ToString()),
                        new XElement(_strUserName, row[_strUserName].ToString()),
                        new XElement(_strPassword, row[_strPassword].ToString())
                    )
                );
            }

            XElement ele = new XElement(_strRoot, keyItems);
            ele.Save(strFileName);
        }
    }
}
