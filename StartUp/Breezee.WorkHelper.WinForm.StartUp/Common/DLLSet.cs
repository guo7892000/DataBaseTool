using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Xml.Linq;
using System.IO;
using Breezee.Global.Entity;

namespace Breezee.WorkHelper.WinForm.StartUp
{
    /// <summary>
    /// 对象名称：DLL维护
    /// 创建作者：黄国辉
    /// 创建日期：2013-10-19
    /// 说明：对固定格式的XML文件维护类。
    /// </summary>
    public class DLLSet
    {
        private string _strRoot = "xml"; //根节点
        private string _strElement = "Dll"; //子节点
        //节点的项名
        private string _strGuid = "Guid"; //类型
        private string _strName = "Name"; //键
        private string _strCode = "Code"; //值
        private string _strFileName = "./Config/DllList.xml";

        /// <summary>
        /// 构造函数
        /// </summary>
        public DLLSet()
        {
            
        }

        /// <summary>
        /// 获取配置文件方法
        /// </summary>
        /// <returns></returns>
        public string GetFileName()
        {
            return _strFileName;
        }
        
        /// <summary>
        /// 加载文件
        /// </summary>
        /// <returns></returns>
        public DataTable LoadXMLFile()
        {
            DataTable dt = GenerateDataStruct();

            if (File.Exists(_strFileName))
            {
                //2014-3-24 去掉文件的的只读属性
                FileInfo f = new FileInfo(_strFileName);
                f.Attributes = FileAttributes.Normal;

                XDocument doc = XDocument.Load(_strFileName);
                foreach (XElement ele in doc.Root.Elements(_strElement))
                {
                    DataRow row = dt.NewRow();
                    row[_strGuid] = (string)ele.Attribute(_strGuid);
                    row[_strName] = (string)ele.Attribute(_strName);
                    row[_strCode] = (string)ele.Attribute(_strCode);

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
            dt.Columns.Add(_strGuid, typeof(string));
            dt.Columns.Add(_strName, typeof(string));
            dt.Columns.Add(_strCode, typeof(string));
            dt.PrimaryKey = new DataColumn[] { dt.Columns[_strGuid], dt.Columns[_strCode] };
            dt.Columns[_strGuid].DefaultValue = Guid.NewGuid().ToString();
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
                        new XAttribute(_strGuid, row[_strGuid].ToString()),
                        new XAttribute(_strName, row[_strName].ToString()),
                        new XAttribute(_strCode, row[_strCode].ToString())
                    )
                );
            }

            XElement ele = new XElement(_strRoot, keyItems);
            ele.Save(strFileName);
        }
    }
}
