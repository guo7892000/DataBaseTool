using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Breezee.WorkHelper.Entity
{
    public class WorkHelperStaticString
    {
        //数据库清单
        public static readonly string DBFileName_System = "DB_System.db";//系统数据库
        public static readonly string DBFileName_SimpleSalary = "DB_SimpleSalary.db";//工资系统数据库
        //
        public static readonly string MessageInfoTitle = "温馨提示";
        public static readonly string MessageWarmTitle = "警告信息";
        public static readonly string MessageErrorTitle = "错误提示";
        //WinForm使用的菜单相关
        public static readonly string DllFileName = "DllList.xml";
        public static readonly string MenuFileName = "MenuList.xml";
        public static readonly string ShortCutMenuFileName = "ShortCutMenuList.xml";
        //WPF使用的菜单相关
        public static readonly string DllFileName_WPF = "DllList_WPF.xml";
        public static readonly string MenuFileName_WPF = "MenuList_WPF.xml";
        public static readonly string ShortCutMenuFileName_WPF = "ShortCutMenuList_WPF.xml";

        /// <summary>
        /// 主机厂ID
        /// </summary>
        public static readonly string MAIN_ID = "HOST";

        /// <summary>
        /// Dictionary唯一ID关键字
        /// </summary>
        public static readonly string UNIQUE_FLAG = "LIFECYCLE_UNID";

        /// <summary>
        /// 性能追踪开关
        /// </summary>
        public static readonly string PERFORMANCE_FLAG = "PERFORMANCE_TRACE_FLAG";

        #region 分页相关
        /// <summary>
        /// 每页记录数常量
        /// </summary>
        public static readonly string PAGE_SIZE = "PAGE_SIZE";

        /// <summary>
        /// 页号常量
        /// </summary>
        public static readonly string PAGE_NO = "PAGE_NO";
        /// <summary>
        /// 总记录数常量
        /// </summary>
        public static readonly string TOTAL_COUNT = "#TOTAL_COUNT#";

        /// <summary>
        /// 是否分页
        /// </summary>
        public static readonly string IS_PAGE = "FALSE";
        #endregion

        /// <summary>
        /// 判断是否用户在线时间间隔
        /// </summary>
        public static readonly int LIVE_INTERVAL = 5;

        /// <summary>
        /// 编辑下拉框中第一项的显示文本
        /// </summary>
        public static readonly string DDL_Text_Choose = "--请选择--";

        /// <summary>
        /// 下拉框全部的显示文本
        /// </summary>
        public static readonly string DDL_Text_All = "全部";

        /// <summary>
        /// Excel_1997-2003 文件类型过滤字符串
        /// </summary>
        public static readonly string EXCEL_FILTER_03 = "Excel 97-2003(*.xls)|*.xls";

        /// <summary>
        /// Excel_1997-2012 文件类型过滤字符串
        /// </summary>
        public static readonly string EXCEL_FILTER_0307 = "Excel 97-2003(*.xls)|*.xls|Excel 工作薄(*.xlsx)|*.xlsx";

        /// <summary>
        /// Excel_2003-2012 文件类型过滤字符串
        /// </summary>
        public static readonly string EXCEL_FILTER_0710 = "Excel 工作薄(*.xlsx)|*.xlsx";

        public static readonly string Dictionary_Key = "VALUE_CODE";
        public static readonly string Dictionary_Value = "VALUE_NAME";

        public static readonly string FRA_SysteAdminUserName = "xtadmin";

        #region 返回消息类别定义
        /// <summary>
        /// 返回标记，0失败，1成功
        /// </summary>
        public static readonly string FRA_RETURN_FLAG = "#FRA_RETURN_FLAG#";

        /// <summary>
        /// 用于界面显示给用户的界面友好提示信息
        /// </summary>
        public static readonly string FRA_USER_MSG = "#FRA_USER_MSG#";

        /// <summary>
        /// 应用程序捕获的原始异常信息
        /// </summary>
        public static readonly string FRA_EXCEPTION = "#FRA_EXCEPTION#";

        /// <summary>
        /// 应用程序捕获的原始异常类型
        /// </summary>
        public static readonly string FRA_ERR_TYPE = "#FRA_ERR_TYPE#";

        /// <summary>
        /// 应用程序捕获的经过转换的异常内容
        /// </summary>
        public static readonly string FRA_ERR_MSG = "#FRA_ERR_MSG#";

        /// <summary>
        /// 查询中返回的表结果键
        /// </summary>
        public static readonly string FRA_QUERY_RESULT = "#FRA_DT_RESULT#";

        /// <summary>
        /// 表更新中的列动态固定值键
        /// </summary>
        public static readonly string FRA_COLUMNS_FIX_VALUE = "动态固定值";

        #endregion

        #region 网格Tag值定义的分隔字符
        /// <summary>
        /// 分隔每个字段属性的字符，注意两者要保持一致
        /// </summary>
        public static readonly char FRA_GRID_COLUMN_SPLIT_PROPERT_CHAR = ',';
        public static readonly string FRA_GRID_COLUMN_SPLIT_PROPERT_STR = ",";

        /// <summary>
        /// 分隔每个字段的分隔符，注意两者要保持一致
        /// </summary>
        public static readonly char FRA_GRID_COLUMN_SPLIT_CHAR = ';';
        public static readonly string FRA_GRID_COLUMN_SPLIT_STR = ";";
        #endregion

        public static readonly string FRA_GRID_ROWNO_STR = "ROWNO";
        public static readonly string FRA_GRID_IS_SELECTED_STR = "IS_SELECTED";
    }
}
