using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Breezee.WorkHelper.DBTool.Entity
{
    public class DBToolStaticString
    {
        #region 生成表结构相关
        //表
        public static readonly string TableStructGenerate_Table = @"SELECT 序号,变更类型,表名称,表编码,备注 
                    FROM [表$] 
                    WHERE [序号] IS NOT NULL AND 表编码 IS NOT NULL 
                    ORDER BY [序号]";
        //列
        public static readonly string TableStructGenerate_Column_SqlServer = @"SELECT  变更类型,表编码,列名称,列编码,类型,长度,小数位,键,必填,默认值,备注,自增长设置,唯一性,外键
                     FROM [SqlServer列$] 
                     WHERE 列编码 IS NOT NULL AND 表编码 IS NOT NULL
                           AND 变更类型 IS NOT NULL";

        public static readonly string TableStructGenerate_Column_Oralce = string.Format(@"SELECT  变更类型,表编码,列名称,列编码,类型,长度,小数位,键,必填,默认值,备注,主键名,序列名,唯一约束名,外键,外键名
                     FROM [{0}$] 
                     WHERE 列编码 IS NOT NULL AND 表编码 IS NOT NULL
                     AND 变更类型 IS NOT NULL", "Oracle列");
        public static readonly string TableStructGenerate_Column_MySql = string.Format(@"SELECT  变更类型,表编码,列名称,列编码,类型,长度,小数位,键,必填,默认值,备注,标志位,自增长,外键
                     FROM [{0}$] 
                     WHERE 列编码 IS NOT NULL AND 表编码 IS NOT NULL
                     AND 变更类型 IS NOT NULL", "MySql列");
        public static readonly string TableStructGenerate_Column_SqLite = string.Format(@"SELECT  变更类型,表编码,列名称,列编码,类型,长度,小数位,键,必填,默认值,备注,主键名,自增长,唯一约束名,外键,外键名
                     FROM [{0}$] 
                     WHERE 列编码 IS NOT NULL AND 表编码 IS NOT NULL
                     AND 变更类型 IS NOT NULL", "SQLite列");
        public static readonly string TableStructGenerate_Column_PostgreSql = string.Format(@"SELECT  变更类型,表编码,列名称,列编码,类型,小数位,长度,键,必填,默认值,备注,主键名,唯一约束名,外键,外键名
                     FROM [{0}$] 
                     WHERE 列编码 IS NOT NULL AND 表编码 IS NOT NULL
                     AND 变更类型 IS NOT NULL", "PostgreSql列");
        //综合列：包括所有数据库类型
        public static readonly string TableStructGenerate_Column_AllSql = string.Format(@"SELECT  变更类型,表编码,列名称,列编码,类型,小数位,长度,键,必填,默认值,备注,
SqlServer自增长设置,SqlServer唯一性,SqlServer外键,
Oracle主键名,Oracle序列名,Oracle唯一约束名,Oracle外键,Oracle外键名,
MySql标志位,MySql自增长,MySql外键,
SQLite主键名,SQLite自增长,SQLite唯一约束名,SQLite外键,SQLite外键名,
PostgreSql主键名,PostgreSql唯一约束名,PostgreSql外键,PostgreSql外键名
                     FROM [{0}$] 
                     WHERE 列编码 IS NOT NULL AND 表编码 IS NOT NULL
                     AND 变更类型 IS NOT NULL", "综合列");

        #endregion

        #region Excel导入数据生成
        //数据变更表清单
        public static readonly string DataGenerate_Table = @"SELECT 序号,数据库类型,类型,表名,数据Sheet名
                         FROM [表$] 
                         WHERE [序号] IS NOT NULL AND 表名 IS NOT NULL 
                                AND 数据SHEET名 IS NOT NULL ";
        //数据变更列清单
        public static readonly string DataGenerate_Column = @"SELECT 表名,列名,固定值,是否修改条件,是否不加引号,是否辅助列,日期格式
                         FROM [列$] 
                         WHERE 表名 IS NOT NULL AND 列名 IS NOT NULL";
        #endregion
    }
}
