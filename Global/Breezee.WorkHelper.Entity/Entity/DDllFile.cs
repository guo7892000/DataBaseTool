using Breezee.Framework.Tool;
using Breezee.Global.Entity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace Breezee.WorkHelper.Entity
{
    public class DDllFile
    {
        public string Guid;
        public string Name;
        public string Code;

        public static XmlDocument ReadDllXmlFile()
        {
            XmlDocument xmlMenu = new XmlDocument();
            xmlMenu.Load(Path.Combine(GlobalValue.StartupPath, "Config", WorkHelperStaticString.DllFileName));

            return xmlMenu;
        }

        #region 节点变为菜单
        public static IDictionary<string, DDllFile> GetAllDll()
        {
            IDictionary<string, DDllFile> dicReturn = new Dictionary<string, DDllFile>();
            XmlDocument xmlMenu = ReadDllXmlFile();
            var xmlNodeList = xmlMenu.SelectNodes("xml/Dll");
            foreach (XmlNode xnModel in xmlNodeList)
            {
                if (xnModel.NodeType == XmlNodeType.Comment)
                {
                    continue;
                }
                DDllFile dMenu = new DDllFile();
                dMenu.Guid = xnModel.GetAttributeValue(DllAttrString.Guid);
                dMenu.Name = xnModel.GetAttributeValue(DllAttrString.Name);
                dMenu.Code = xnModel.GetAttributeValue(DllAttrString.Code);
                dicReturn[dMenu.Guid] = dMenu;
            }

            return dicReturn;
        }
        #endregion

    }
}
