/*1、SQL Update多表联合更新的方法*/
(1) sqlite 多表更新方法
//----------------------------------
update t1 set col1=t2.col1
from table1 t1
inner join table2 t2 on t1.col2=t2.col2
这是一个非常简单的批量更新语句 在SqlServer中支持此语法 sqlite中却不支持


sqlite中可转换为 如下语法 
update table1 set col1=(select col1 from table2 where col2=table1.col2)


update ta_jbnt_tzhd_pht_Area_xiang set t1=(select sys_xzqhdm.name from sys_xzqhdm 
 where t2=sys_xzqhdm.code) 


(2) SQL Server 多表更新方法
//----------------------------------
SQL Server语法:UPDATE { table_name WITH ( < table_hint_limited > [ ...n ] ) |
view_name | rowset_function_limited } SET { column_name = { expression | DEFAULT
| NULL } | @variable = expression | @variable = column = expression } [ ,...n ]
{ { [ FROM { < table_source > } [ ,...n ] ] [ WHERE < search_condition > ] } | [
WHERE CURRENT OF { { [ GLOBAL ] cursor_name } | cursor_variable_name } ] } [
OPTION ( < query_hint > [ ,...n ] ) ]


SQL Server示例: update a set a.gqdltks=b.gqdltks,a.bztks=b.bztks from
landleveldata a,gdqlpj b where a.GEO_Code=b.lxqdm


access数据库多表更新方法

x = "update " + DLTB + " a inner join tbarea2 b  on a.objectid=b.FID  set a." + fd_dltb_xzdwmj + "=b.area_xzdw, a." + fd_dltb_lxdwmj + "=b.area_lxdw";
 SQLList.Add(x);


(3) Oracle 多表更新方法
//----------------------------------
Oracle语法: UPDATE updatedtable SET (col_name1[,col_name2...])= (SELECT
col_name1,[,col_name2...] FROM srctable [WHERE where_definition])


Oracel 示例: update landleveldata a set (a.gqdltks, a.bztks)= (select b.gqdltks,
b.bztks from gdqlpj b where a.GEO_Code=b.lxqdm)


(4) MySQL 多表更新方法
//----------------------------------
MySQL语法: UPDATE table_references SET col_name1=expr1 [, col_name2=expr2 ...]
[WHERE where_definition]


MySQL 示例: update landleveldata a, gdqlpj b set a.gqdltks= b.gqdltks, a.bztks=
b.bztks where a.GEO_Code=b.lxqdm 


/**/


/**/


/**/


/**/


/**/


/**/


/**/


/**/


/**/


/**/


/**/


/**/


/**/


/**/


/**/


/**/


/**/