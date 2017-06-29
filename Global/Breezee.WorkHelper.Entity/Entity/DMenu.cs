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
    /// <summary>
    /// 菜单表
    /// </summary>
    public class DMenu
    {
        #region 公共变量
        public string Guid;
        public MenuTypeEnum MenuType;
        public string Name;
        public string Code;
        public string ShortCutKey;
        public string DLLName;
        public string FormName;
        public string FullPath;//菜单全路径

        public string ParentGuid;//上级菜单Guid

        public List<DMenu> ChildMenu = new List<DMenu>();
        public static string MenuXmlFilePath = Path.Combine(GlobalValue.EntryAssemblyPath, "Config", WorkHelperStaticString.MenuFileName);
        public static string MenuXmlFilePath_WPF = Path.Combine(GlobalValue.EntryAssemblyPath, "Config", WorkHelperStaticString.MenuFileName_WPF);
        #endregion


        #region 读取菜单XML
        public static XmlDocument ReadMenuXmlFile(AppType at)
        {
            XmlDocument xmlMenu = new XmlDocument();
            
            if (at == AppType.WinForm)
            {
                xmlMenu.Load(MenuXmlFilePath);
            }
            else
            {
                xmlMenu.Load(MenuXmlFilePath_WPF);
            }
            return xmlMenu;
        } 
        #endregion

        #region 增加所有菜单
        public static IDictionary<string, DMenu> GetAllMenu(AppType at)
        {
            var dicAllMenu = new Dictionary<string, DMenu>();

            XmlDocument xmlMenu = ReadMenuXmlFile(at);

            XmlNodeList xmlList = xmlMenu.SelectNodes("xml/Model");

            foreach (XmlNode xnModel in xmlList)
            {
                if (xnModel.NodeType == XmlNodeType.Comment)
                {
                    continue;
                }
                DMenu dMenu = NodeToMenu(xnModel, MenuTypeEnum.Modul, string.Empty);
                dicAllMenu[dMenu.Guid] = dMenu;

                foreach (XmlNode xnClass in xnModel.ChildNodes)
                {
                    if (xnClass.NodeType == XmlNodeType.Comment)
                    {
                        continue;
                    }
                    if (xnClass.Name == MenuConfigString.Class)
                    {
                        DMenu dMenuChild = NodeToMenu(xnClass, MenuTypeEnum.Class, dMenu.FullPath);
                        dicAllMenu[dMenuChild.Guid] = dMenuChild;
                        dMenu.ChildMenu.Add(dMenuChild);
                        //增加子菜单分类
                        AddChildMenuClass(xnClass, dMenuChild, dicAllMenu);
                    }
                    else
                    {
                        DMenu dMenuChild = NodeToMenu(xnClass, MenuTypeEnum.Menu, dMenu.FullPath);
                        dicAllMenu[dMenuChild.Guid] = dMenuChild;
                        dMenu.ChildMenu.Add(dMenuChild);
                    }
                }
            }

            return dicAllMenu;
        }
        #endregion

        #region 节点变为菜单
        public static DMenu NodeToMenu(XmlNode xnModel, MenuTypeEnum mte, string parentMenuPath)
        {
            DMenu dMenu = new DMenu();
            dMenu.MenuType = mte;
            dMenu.Guid = xnModel.GetAttributeValue(MemuAttrString.Guid);
            dMenu.Name = xnModel.GetAttributeValue(MemuAttrString.Name);
            dMenu.Code = xnModel.GetAttributeValue(MemuAttrString.Code);
            dMenu.ShortCutKey = xnModel.GetAttributeValue(MemuAttrString.ShortCutKey);
            dMenu.DLLName = xnModel.GetAttributeValue(MemuAttrString.DLLName);
            dMenu.FormName = xnModel.GetAttributeValue(MemuAttrString.FormName);
            if (!string.IsNullOrEmpty(parentMenuPath))
            {
                dMenu.FullPath = parentMenuPath + " > " + dMenu.Name;
            }
            else
            {
                dMenu.FullPath = dMenu.Name;
            }
            return dMenu;
        }
        #endregion

        #region 增加子菜单分类
        /// <summary>
        /// 增加子菜单分类
        /// </summary>
        /// <param name="xParentNode"></param>
        /// <param name="dParentMenu"></param>
        /// <param name="allMenu"></param>
        private static void AddChildMenuClass(XmlNode xParentNode, DMenu dParentMenu, IDictionary<string, DMenu> allMenu)
        {
            foreach (XmlNode xnChild in xParentNode.ChildNodes)
            {
                if (xnChild.NodeType == XmlNodeType.Comment)
                {
                    continue;
                }
                if (xnChild.Name == MenuConfigString.Class)
                {
                    DMenu dMenuChild = NodeToMenu(xnChild, MenuTypeEnum.Class, dParentMenu.FullPath);
                    dParentMenu.ChildMenu.Add(dMenuChild);
                    allMenu[dMenuChild.Guid] = dMenuChild;
                }
                else
                {
                    DMenu dMenuChild = NodeToMenu(xnChild, MenuTypeEnum.Menu, dParentMenu.FullPath);
                    dParentMenu.ChildMenu.Add(dMenuChild);
                    allMenu[dMenuChild.Guid] = dMenuChild;
                }
            }
        }
        #endregion
    }

    public enum AppType
    {
        WinForm,
        WPFWindow
    }
}
