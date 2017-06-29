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
    /// 对象名称：数据字典维护窗体
    /// 创建作者：黄国辉
    /// 创建日期：2014-3-28
    /// 说明：对固定格式的字典XML文件维护类
    /// </summary>
    public class DataDictinary
    {
        private string _OralceDataDictinaryFileName = "./SaveFile/Oracle数据字典.xml"; //Oracle数据字典
        private string _SqlServerDataDictinaryFileName = "./SaveFile/Sql Server数据字典.xml"; //Sql Server数据字典
        private string _strRoot = "Items"; //根节点
        private string _strElement = "Item"; //子节点
        //节点的项名
        private string _strColumnName = "列名称"; //类型
        private string _strColumnCode = "列编码"; //键
        private string _strColumnType = "类型"; //值
        private string _strColumnLength = "长度"; //值
        private string _strFileName;

        /// <summary>
        /// 构造函数
        /// </summary>
        public DataDictinary()
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
        public DataTable LoadXMLFile(DataBaseType dbt)
        {
            DataTable dt = GenerateDataStruct();
            if (dbt == DataBaseType.Oracle)
            {
                _strFileName = _OralceDataDictinaryFileName;
            }
            else
            {
                _strFileName = _SqlServerDataDictinaryFileName;
            }
            if (File.Exists(_strFileName))
            {
                //2014-3-24 去掉文件的的只读属性
                FileInfo f = new FileInfo(_strFileName);
                f.Attributes = FileAttributes.Normal;

                XDocument doc = XDocument.Load(_strFileName);
                foreach (XElement ele in doc.Root.Elements(_strElement))
                {
                    DataRow row = dt.NewRow();
                    row[_strColumnName] = (string)ele.Element(_strColumnName);
                    row[_strColumnCode] = (string)ele.Element(_strColumnCode);
                    row[_strColumnType] = (string)ele.Element(_strColumnType);
                    row[_strColumnLength] = (string)ele.Element(_strColumnLength);
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
            dt.Columns.Add(_strColumnName, typeof(string));
            dt.Columns.Add(_strColumnCode, typeof(string));
            dt.Columns.Add(_strColumnType, typeof(string));
            dt.Columns.Add(_strColumnLength, typeof(string));
            //编码作为唯一主键
            dt.PrimaryKey = new DataColumn[] { dt.Columns[_strColumnCode] };
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
                        new XElement(_strColumnName, row[_strColumnName].ToString()),
                        new XElement(_strColumnCode, row[_strColumnCode].ToString()),
                        new XElement(_strColumnType, row[_strColumnType].ToString()),
                        new XElement(_strColumnLength, row[_strColumnLength].ToString())
                    )
                );
            }

            XElement ele = new XElement(_strRoot, keyItems);
            ele.Save(strFileName);
        }
    }
}
