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
    /// 对象名称：键值维护窗体
    /// 创建作者：黄国辉
    /// 创建日期：2013-10-19
    /// 说明：对固定格式的XML文件维护类。感谢水哥的支持！
    /// </summary>
    public class KeyValue
    {
        private string _OrlFileName = "./Model/Oracle表列默认值清单.xml"; //新增修改的默认值设置
        private string _OrlExcludeFileName = "./Model/Oracle生成SQL排除字段清单.xml"; //新增修改查询的排除字段设置
        private string _SQLFileName = "./Model/SQL表列默认值清单.xml"; //新增修改的默认值设置
        private string _SQLExcludeFileName = "./Model/SQL生成SQL排除字段清单.xml"; //新增修改查询的排除字段设置
        private string _strRoot = "Items"; //根节点
        private string _strElement = "Item"; //子节点
        private string _strType = "TYPE"; //类型
        private string _strKey = "KEY"; //键
        private string _strValue = "VALUE"; //值
        private string _strFileName;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="strDBType">数据库类型</param>
        /// <param name="strDealType">处理类型</param>
        public KeyValue(DataBaseType dbt, AutoSqlColumnSetType ascst)
        {
            if (dbt == DataBaseType.Oracle)
            {
                if (ascst == AutoSqlColumnSetType.Default)
                {
                    _strFileName = _OrlFileName;
                }
                else
                {
                    _strFileName = _OrlExcludeFileName;
                }
            }
            else
            {
                if (ascst == AutoSqlColumnSetType.Default)
                {
                    _strFileName = _SQLFileName;
                }
                else
                {
                    _strFileName = _SQLExcludeFileName;
                }
            }
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
                    row[_strType] = (string)ele.Element(_strType);
                    row[_strKey] = (string)ele.Element(_strKey);
                    row[_strValue] = (string)ele.Element(_strValue);

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
            dt.Columns.Add(_strType, typeof(string));
            dt.Columns.Add(_strKey, typeof(string));
            dt.Columns.Add(_strValue, typeof(string));
            dt.PrimaryKey = new DataColumn[] { dt.Columns[_strType], dt.Columns[_strKey] };
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
                        new XElement(_strType, row[_strType].ToString()),
                        new XElement(_strKey, row[_strKey].ToString()),
                        new XElement(_strValue, row[_strValue].ToString())
                    )
                );
            }

            XElement ele = new XElement(_strRoot, keyItems);
            ele.Save(strFileName);
        }
    }
}
