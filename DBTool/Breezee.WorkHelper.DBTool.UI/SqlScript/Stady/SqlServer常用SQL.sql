/*SQL表对象*/
sys.objects、syscolumns、systypes、sys.default_constraints、sys.all_sql_modules

/*查询表信息*/
SELECT b.name OWER,a.name Table_Name,a.* 
FROM sys.objects a 
JOIN sys.schemas b ON a.schema_id=b.schema_id WHERE a.type='U' AND a.name='FI_MATAINGATHER_M'

/*动态执行SQL函数（sp_executesql）示例*/
EXECUTE sp_executesql 
          N'SELECT * FROM pa_part_list 
          WHERE partno = @level',
          N'@level varchar(50)',
          @level = '0009141430-A034';

/*指定字段指定位置为固定值的SQL*/
SELECT * FROM dbo.PA_PART_LIST WHERE Charindex('A',PARTNO)=2 OR Charindex('1',PARTNO)=2

/*配置启用CLR语句，即可以使用VS 2005创建的存储过程*/
EXEC sp_configure 'clr enabled' , 1 
RECONFIGURE WITH override

/*如有启用分布式事务，则要把Distributed Transaction Coordinator服务启动*/
---操作过程：“运行”==>“net start msdtc”

-/*由零件号前5码中数字与字母对照表（1－A， 2－B， 3－C， 4－D， 5－E， 6－F， 7－G， 8－H， 9－K， 0－M），
 查询以输入大于等于前五位备件号为开头的后模糊查询示例（这里只以两位为例）*/ 
 SELECT * FROM PA_PART_LIST 
 WHERE 1=1 --AND Charindex('5',PARTNO)=1
 AND (Charindex('E',PARTNO)=1 OR Charindex('5',PARTNO)=1)
 AND (Charindex('B',PARTNO)=2 OR Charindex('2',PARTNO)=2) AND PARTNO LIKE '%112A%'

/*SQL Server收缩日志及数据库文件代码*/
--收缩数据库文件： 
DBCC SHRINKDATABASE(@dbName ) 
DUMP TRANSACTION @dbName WITH NO_LOG 
--截断事务日志： 
BACKUP LOG @dbName WITH NO_LOG 


/*收缩日志文件*/ 
DBCC SHRINKFILE 
( 
{ 'file_name'  file_id } 
{ [ , EMPTYFILE ] 
 [ [ , target_size ] [ , { NOTRUNCATE  TRUNCATEONLY } ] ] 
} 
) 
[ WITH NO_INFOMSGS ] 

/*修改表增加列*/
if Not Exists(Select 1 From sysobjects a,syscolumns b 
Where a.id = b.id And a.name = 'PA_STORAGE' And b.name = 'RDCQTY' And a.type = 'U')
ALTER TABLE PA_STORAGE ADD RDCQTY DECIMAL(14,2) /*RDC库数量*/

/*行列转换*/
--PIVOT 提供的语法比一系列复杂的 SELECT...CASE 语句中所指定的语法更简单、更具可读性。
--常见的可能会用到 PIVOT 的情形是，需要生成交叉表格报表以汇总数据时。
--A、行转列示例(需要筛选行数据中某列值作为具体列，其他列的值根据该值作汇总等)：
USE AdventureWorks;
GO
SELECT VendorID, [164] AS Emp1, [198] AS Emp2, [223] AS Emp3, [231] AS Emp4, [233] AS Emp5
FROM 
(SELECT PurchaseOrderID, EmployeeID, VendorID
FROM PurchaseOrderHeader) p
PIVOT
(
COUNT (PurchaseOrderID)
FOR EmployeeID IN
( [164], [198], [223], [231], [233] )
) AS pvt
ORDER BY VendorID
---B、列转行
--UNPIVOT 将与 PIVOT 执行几乎完全相反的操作，将列转换为行。
CREATE TABLE pvt (VendorID int, Emp1 int, Emp2 int,
Emp3 int, Emp4 int, Emp5 int)
GO
INSERT INTO pvt VALUES (1,4,3,5,4,4)
INSERT INTO pvt VALUES (2,4,1,5,5,5)
INSERT INTO pvt VALUES (3,4,3,5,4,4)
INSERT INTO pvt VALUES (4,4,2,5,5,4)
INSERT INTO pvt VALUES (5,5,1,5,5,5)
GO
--Unpivot the table.
SELECT VendorID, Employee, Orders
FROM 
   (SELECT VendorID, Emp1, Emp2, Emp3, Emp4, Emp5
   FROM pvt) p
UNPIVOT
   (Orders FOR Employee IN 
      (Emp1, Emp2, Emp3, Emp4, Emp5)
)AS unpvt
GO

/*不写日志删除数据*/
TRUNCATE TABLE 表名;

/*获取按指定列排序后，取出指定位置行记录*/
SELECT * FROM (
select ROW_NUMBER() OVER(ORDER BY createtime) rowNum,*
 FROM UC_T_CUST_TYPE
  WHERE CHECKSTATE = '2'
 AND ISNULL(STORE_STATUS, 0) = '0'
  AND ISENABLE = '1') a
where rowNum BETWEEN 1 AND 10

/*增加指定的自增长列值记录*/
Set   IDENTITY_INSERT   Temp_Test   ON   
INSERT   INTO   Temp_Test(CHEKCID,CHEKCLISTNO,CheckModel )   
SELECT   CHEKCID,CHEKCLISTNO,CheckModel    
FROM dbo.PA_STORAGE_CHECK_M WHERE CHEKCLISTNO='PC100903'
Set   IDENTITY_INSERT   Temp_Test   Off

/*创建新表*/
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[OP_PLAN_Audit]') AND type in (N'U'))
CREATE TABLE [dbo].[OP_PLAN_Audit] (
[ID]  [BIGINT ] IDENTITY(1,1) NOT NULL,
[YEAR]  [int ] NULL,
[MONTH]  [int ] NULL,
[ISENABLE]  [VARCHAR ](1) NULL,
[ISSYSTEM]  [VARCHAR ](1) NULL,
[NETCODE]  [VARCHAR ](10) NULL,
CONSTRAINT [ OP_Audit_ID] PRIMARY KEY CLUSTERED  ( [ID] ASC ))  
GO
---创建统计用到的表PA_TEMP_IN_OUT_TOTAL，如存在，则删除后重建
  IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[PA_TEMP_IN_OUT_TOTAL]') AND type in (N'U'))
    TRUNCATE TABLE PA_TEMP_IN_OUT_TOTAL;
 CREATE TABLE PA_TEMP_IN_OUT_TOTAL
 (
      PARTNO VARCHAR(50),/*备件编号*/
      PARTNAME VARCHAR(50),/*备件名称*/
      UNIT VARCHAR(50),/*单位*/
      JPXSLR DECIMAL(14, 2)/*精品销售利润*/
 )

/*Union和Union all 的区别*/
在数据库中，union和union all关键字都是将两个结果集合并为一个，但这两者从使用和效率上来说都有所不同。
union在进行表链接后会筛选掉重复的记录，所以在表链接后会对所产生的结果集进行排序运算，删除重复的记录再返回结果。
如：
select * from test_union1
   union
select * from test_union2
      这个SQL在运行时先取出两个表的结果，再用排序空间进行排序删除重复的记录，最后返回结果集，如果表数据量大的话可能会导致用磁盘进行排序。
    而union all只是简单的将两个结果合并后就返回。这样，如果返回的两个结果集中有重复的数据，那么返回的结果集就会包含重复的数据了。
     从效率上说，union all要比union快很多，所以，如果可以确认合并的两个结果集中不包含重复的数据的话，那么就使用union all，如下：
select * from test_union1
union all
select * from test_union2
使用 union 组合查询的结果集有两个最基本的规则：
1。所有查询中的列数和列的顺序必须相同。
2。数据类型必须兼容

/*判断临时表是否存在*/
 if object_id('tempdb..#TEMP_TABLE') is not null 
 print '临时表#TEMP_TABLE已存在!' 

/*删除SQL Server的JOB语句*/
DECLARE @JobID VARBINARY(100)
IF  EXISTS (SELECT job_id FROM msdb.dbo.sysjobs_view WHERE name = N'P_AJUST_PA_MONTH_TOTAL')
BEGIN
SELECT @JobID=job_id FROM msdb.dbo.sysjobs_view WHERE name = N'P_AJUST_PA_MONTH_TOTAL'
EXEC msdb.dbo.sp_delete_job @job_id=@JobID, @delete_unused_schedule=1
END

/*增加启辰品牌*/
---如有ID为2的记录先删除再新增
IF EXISTS(SELECT * FROM SE_CARBRAND WHERE CARBRAND_ID=2)
DELETE FROM SE_CARBRAND WHERE CARBRAND_ID=2
GO
SET IDENTITY_INSERT SE_CARBRAND ON
INSERT INTO SE_CARBRAND(CARBRAND_ID ,
        CARBRAND_CODE ,
        BRANDNAME ,
        REMARK ,
        ORDERNO ,
        IS_HD ,
        CREATETIME ,
        CREATOR ,
        LAST_UPDATED_DATE ,
        MODIFIER ,
        ISENABLE ,
        ISSYSTEM ,
        NETCODE)
SELECT 2,--CARBRAND_ID ,
       2,--CARBRAND_CODE ,
        '启辰',--BRANDNAME ,
        '',--REMARK ,
        '2',--ORDERNO ,
        '1',--IS_HD ,
        GETDATE(),--CREATETIME ,
        'Sys',--CREATOR ,
        GETDATE(),--LAST_UPDATED_DATE ,
        'Sys',--MODIFIER ,
        '1',--ISENABLE ,
        '0',--ISSYSTEM ,
        NETCODE 
FROM dbo.BD_SYSINFO WHERE DLR_TYPE=1
SET IDENTITY_INSERT SE_CARBRAND OFF
GO

/*只算维修出库的备件满足率情况*/
SELECT CAL_MONTH,CAST(SUM(REAL_PZ)/SUM(TOTAL_PZ)*100  AS DECIMAL(14,2)) PZ_RATE,
CASE WHEN SUM(PARTQTY)=0 THEN 0 else CAST((SUM(PARTQTY)-SUM(ISNULL(LP_QTY,0)))*100/SUM(PARTQTY) AS DECIMAL(14,2)) end QTY_RATE,
CASE WHEN SUM(PARTQTY*PRICE)=0 THEN 0 else CAST((SUM(PARTQTY*PRICE)-SUM(ISNULL(LP_QTY,0)*PRICE))*100/SUM(PARTQTY*PRICE) AS DECIMAL(14,2)) end MONEY_RATE
INTO #T_REPAIR_LP
FROM (
	SELECT MONTH(A.CREATETIME) CAL_MONTH,A.PARTNO,CASE WHEN ISNULL(B.QTY,0)=0 THEN 1 
		WHEN A.PARTQTY=B.QTY THEN 0 ELSE 0.5 END REAL_PZ,
		1 TOTAL_PZ,A.PARTQTY,B.QTY AS LP_QTY,A.PRICE
	FROM	(SELECT A.REPAIRPAPER_ID,A.CREATETIME,B.PARTNO,SUM(B.PARTQTY) PARTQTY,
						CAST(SUM(B.PRICE)/COUNT(*) AS DECIMAL(14,2)) PRICE
				 FROM dbo.SE_REPAIRPAPER A
				 JOIN dbo.SE_REPAIRPART B ON A.REPAIRPAPER_ID=B.REPAIRPAPER_ID
				 JOIN dbo.PA_PART_LIST C ON C.PARTNO=B.PARTNO
				 WHERE B.IS_OUT_STORAGE = '1' /*要出库的*/
					 AND A.ISENABLE=1/*有效的*/
					 AND YEAR(A.CREATETIME)='#YEAR#'/*以创建日期为准*/
					 AND A.NETCODE='#NETCODE#' /*计算的网点*/
					 AND B.REPAIR_TYPE='#REPAIR_TYPE#' 
					 #MUST_TYPE# 
				GROUP BY A.REPAIRPAPER_ID,A.CREATETIME,B.PARTNO
				)A
		LEFT JOIN (SELECT AA.REL_ID,BB.PARTNO,SUM(QTY) QTY
					FROM dbo.PA_LP_BOOK_M AA,dbo.PA_LP_BOOK_D BB 
					WHERE  AA.BOOK_M_ID=BB.BOOK_M_ID AND AA.REL_TYPE='1'/*维修出库*/
						AND	 AA.AUDIT_STATUS='2' /*审核通过*/
						AND YEAR(AA.AUDIT_DATE)='#YEAR#'/*以审核日期为准*/
						AND AA.NETCODE='#NETCODE#' /*计算的网点*/
					GROUP BY AA.REL_ID,BB.PARTNO 
			  ) B ON B.REL_ID=A.REPAIRPAPER_ID AND B.PARTNO=A.PARTNO
	 
 ) AA
GROUP BY  CAL_MONTH 
SELECT  1 AS SORTNUM,'种类(%)' CALYEAR ,SUM([1]) [1],SUM([2]) [2] ,SUM([3]) [3] ,SUM([4]) [4] ,SUM([5]) [5] ,SUM([6]) [6],
  SUM([7]) [7] ,SUM([8]) [8] ,SUM([9]) [9] ,SUM([10]) [10] ,SUM([11]) [11] ,SUM([12]) [12]
FROM #T_REPAIR_LP p PIVOT
( SUM(PZ_RATE) FOR CAL_MONTH IN ( [1], [2], [3], [4], [5], [6], [7],
                                            [8], [9], [10], [11], [12] ) ) AS pvt
UNION
SELECT  2 AS SORTNUM,'数量(%)' CALYEAR ,SUM([1]) [1],SUM([2]) [2] ,SUM([3]) [3] ,SUM([4]) [4] ,SUM([5]) [5] ,SUM([6]) [6],
  SUM([7]) [7] ,SUM([8]) [8] ,SUM([9]) [9] ,SUM([10]) [10] ,SUM([11]) [11] ,SUM([12]) [12]
FROM #T_REPAIR_LP p PIVOT
( SUM(QTY_RATE) FOR CAL_MONTH IN ( [1], [2], [3], [4], [5], [6], [7],
                                            [8], [9], [10], [11], [12] ) ) AS pvt
UNION
SELECT  3,'金额(%)' AS CALYEAR ,SUM([1]) [1],SUM([2]) [2] ,SUM([3]) [3] ,SUM([4]) [4] ,SUM([5]) [5] ,SUM([6]) [6],
  SUM([7]) [7] ,SUM([8]) [8] ,SUM([9]) [9] ,SUM([10]) [10] ,SUM([11]) [11] ,SUM([12]) [12]
FROM  #T_REPAIR_LP p PIVOT
( SUM(MONEY_RATE) FOR CAL_MONTH IN ( [1], [2], [3], [4], [5], [6], [7], [8],
                                     [9], [10], [11], [12] ) ) AS pvt
ORDER BY SORTNUM

/*查询列*/
Select * From sysobjects a,syscolumns b 
Where a.id = b.id and b.name = 'LOCATIONCODE' And a.type = 'U'

/*执行动态SQL并返回值*/
DECLARE @IntVariable int;
DECLARE @SQLString nvarchar(500);
DECLARE @ParmDefinition nvarchar(500);
DECLARE @max_title varchar(30);

SET @IntVariable = 197;
SET @SQLString = N'SELECT @max_titleOUT = max(Title) 
   FROM AdventureWorks.HumanResources.Employee
   WHERE ManagerID = @level';
SET @ParmDefinition = N'@level tinyint, @max_titleOUT varchar(30) OUTPUT';

EXECUTE sp_executesql @SQLString, @ParmDefinition, @level = @IntVariable, @max_titleOUT=@max_title OUTPUT;
SELECT @max_title;
 
/*删除存储过程*/
--1、删除CLR存储过程
IF ( OBJECT_ID('P_P_E3S_OWEQTY_RECEIPT', 'PC') IS NOT NULL ) 
	DROP PROCEDURE [dbo].[P_P_E3S_OWEQTY_RECEIPT] ; 
--2、删除SQL或CLR存储过程
IF ( OBJECT_ID('P_PA_E3S_OUTSTOCK_RECEIPT') IS NOT NULL ) 
	DROP PROCEDURE P_PA_E3S_OUTSTOCK_RECEIPT
--3、删除SQL或CLR存储过程
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[IFR_UC_T_BASE_ASSESSMENT_ITEMProcedure]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[IFR_UC_T_BASE_ASSESSMENT_ITEMProcedure]

/*游标嵌套*/
DECLARE @curTemp CURSOR
	SET @curTemp = Cursor For SELECT In_Id 
		FROM  AA		  
	OPEN @curTemp
	FETCH NEXT FROM @curTemp INTO @In_Id 		  
	WHILE @@fetch_status = 0 
		BEGIN		
			DECLARE @curButton CURSOR
			SET @curButton	= Cursor For SELECT BUTTON_NAME 
				FROM BBB
				WHERE In_Id=@In_Id
			
			 /*按钮游标*/
			 OPEN @curButton
				FETCH NEXT FROM @curButton INTO @BUTTON_NAME  
				WHILE @@fetch_status = 0 
					BEGIN   
						/*增加按钮表记录*/
						
				FETCH NEXT FROM @curButton INTO @BUTTON_NAME
				
			END 
			close @curButton
			deallocate @curButton 			  
	FETCH NEXT FROM @curTemp INTO @In_Id
END 	
close @curTemp
deallocate @curTemp 

/*创建存储过程例子*/
/*删除存储过程*/
IF ( OBJECT_ID('IFR_UC_T_CAR_SERIESProcedure') IS NOT NULL ) 
	DROP PROCEDURE IFR_UC_T_CAR_SERIESProcedure
GO	
/*创建存储过程*/
CREATE PROCEDURE IFR_UC_T_CAR_SERIESProcedure 
AS
/***********************************************************************************
* 存储过程名称: 二手车车系接收
* 创建作者: 黄国辉
* 创建日期: 2012-12-05   
* 使用系统: DMS
* 使用模块: 二手车管理
* 说明：二手车车系接收
* 修改历史说明：
*	V1.7.06: 由CLR存储过程转换为SQL存储过程(CLR布署太麻烦了) hgh 2012-12-05 
***********************************************************************************/ 
BEGIN
	DECLARE @curTemp CURSOR
	SET @curTemp = Cursor For SELECT In_Id 
		FROM  AA		  
	OPEN @curTemp
	FETCH NEXT FROM @curTemp INTO @In_Id 		  
	WHILE @@fetch_status = 0 
		BEGIN		
					  
			FETCH NEXT FROM @curTemp INTO @In_Id
		END 	
	close @curTemp
	deallocate @curTemp 	
END


/*创建存储过程例子(含写日志)*/
/*删除存储过程*/
IF ( OBJECT_ID('IFR_UC_T_CAR_SERIESProcedure') IS NOT NULL ) 
	DROP PROCEDURE IFR_UC_T_CAR_SERIESProcedure
GO	
/*创建存储过程*/
CREATE PROCEDURE IFR_UC_T_CAR_SERIESProcedure 
AS
/***********************************************************************************
* 存储过程名称: 二手车车系接收
* 创建作者: 黄国辉
* 创建日期: 2012-12-05   
* 使用系统: DMS
* 使用模块: 二手车管理
* 说明：二手车车系接收
* 修改历史说明：
*	V1.7.06: 由CLR存储过程转换为SQL存储过程(CLR布署太麻烦了) hgh 2012-12-05 
***********************************************************************************/ 
BEGIN
	/*变量声明*/
	DECLARE @NETCODE VARCHAR(10),
			@FUN_NAME VARCHAR(200) ,
			@IF_NO VARCHAR(50) ,    
			@IF_NAME VARCHAR(200),  
			@IF_TYPE VARCHAR(1),    
			@MESSAGE VARCHAR(MAX),
			@curTemp CURSOR
    
	/*接口信息*/  
	SELECT  @FUN_NAME = '服务管理', 
			@IF_NO = 'DMS_SE_E3SS_006',
			@IF_NAME = '有偿单台KPI数据上传',
			@IF_TYPE = 'N',
			@MESSAGE = ''
    /*取一网编码*/	
	SELECT @NETCODE=NETCODE FROM dbo.BD_SYSINFO WHERE DLR_TYPE='1'
	/*设置游标*/ 
	SET @curTemp = Cursor For SELECT In_Id FROM  AA
	OPEN @curTemp
	FETCH NEXT FROM @curTemp INTO @In_Id 		  
	WHILE @@fetch_status = 0 
		BEGIN
			BEGIN TRY		
				/*游标内逻辑处理:出错可以用:GOTO Final_Deal*/
				/**此处略******/
				
				/*写入处理成功信息*/
				SELECT  @MESSAGE = 'E3S出库单' +@E3S_OUT_CODE+ '接收成功！',@IF_TYPE = 'N'
			END TRY
			BEGIN CATCH 
				IF @@trancount > 0 
					BEGIN
						ROLLBACK TRAN
					END
					SELECT  @MESSAGE = ERROR_MESSAGE()+'. Error Line: '+ CAST(ERROR_LINE() AS VARCHAR(10)),@IF_TYPE = 'E'
					SET @MESSAGE = 'E3S出库单: '+ CAST(@E3S_OUT_CODE AS VARCHAR(50)) + '接收失败！'+ @MESSAGE
			END CATCH
			/*定义最终处理标签*/
			Final_Deal:
			/*更新接口表记录为已处理状态1*/        
			UPDATE dbo.IFR_PA_E3S_OUTSTOCK SET MQSENDSTATUS='1'
			WHERE NETCODE=@NETCODE AND OUTSTORAGENO=@E3S_OUT_CODE
			/*提交事务*/
			IF @@trancount > 0 
			BEGIN
				COMMIT TRAN
			END      
			--写日志    
			EXEC PROC_LOG_MESSAGE @FUN_NAME_ = @FUN_NAME,@IF_NO_ = @IF_NO, @IF_NAME_ = @IF_NAME, @SCENS_ = '',
				   @DIRECT_ = 'SR', @IF_TYPE_ = @IF_TYPE,@REMARK_ = @MESSAGE		  
			/*处理下一条游标记录*/
			FETCH NEXT FROM @curTemp INTO @In_Id
		END	
	--关闭游标	        	
	close @curTemp
	deallocate @curTemp 	
END

/*SQLSERVER 2005和所有数据库类型的对应关系*/
SELECT * FROM msdb.dbo.MSdatatype_mappings

/*SQL中产生序号*/
SELECT ROW_NUMBER() OVER (ORDER BY a.name)  REQ,a.name table_name  
FROM sys.objects a  
JOIN sys.schemas b ON a.schema_id=b.schema_id WHERE a.type='U'

/*修改表或列说明*/
IF EXISTS(SELECT * FROM sys.extended_properties A
JOIN sys.objects B ON a.major_id=b.object_id
LEFT JOIN sys.all_columns C ON A.minor_id=C.column_id AND B.object_id = C.object_id
WHERE B.name='IFS_BAS_USER' AND (C.name IS NULL OR C.name='MQSENDSTATUS'))
BEGIN
	EXEC sys.sp_dropextendedproperty @name = N'MS_Description',
    @level0type = N'SCHEMA',@level0name = N'dbo', @level1type = N'TABLE',@level1name = N'IFS_BAS_USER',  
    @level2type = N'COLUMN', @level2name = N'MQSENDSTATUS' 
END
EXEC sys.sp_addextendedproperty @name = N'MS_Description', @value = N'传输标志：0未上传(默认)，1已上传',
    @level0type = N'SCHEMA',@level0name = N'dbo', @level1type = N'TABLE',@level1name = N'IFS_BAS_USER',  
    @level2type = N'COLUMN', @level2name = N'MQSENDSTATUS'  

/*删除重复值的一条*/
SELECT * FROM PA_LP_DEAL 
WHERE DEAL_ID IN (
select MIN(DEAL_ID) 
from   PA_LP_DEAL 
WHERE BOOK_D_ID IN( select BOOK_D_ID from   PA_LP_DEAL 
WHERE DEAL_TYPE='1' GROUP BY BOOK_D_ID 
HAVING  COUNT (BOOK_D_ID) > 1
  ) 
GROUP BY BOOK_D_ID
)

/*同一服务器的跨库查询*/
SELECT kcsx,stockupperlimit,kcxx,stocklowerlimit
from zyddb..j_bjml a,dms.dbo.pa_part_list b
where b.partno=a.bjbh

/*ROW_NUMBER的使用*/
WITH TABS AS  
(  
	SELECT ROW_NUMBER() OVER(PARTITION BY ORG_ID  ORDER BY EMP_ID  ) AS ROWNO,ORG_ID,EMP_ID FROM ORG_EMPLOYEE  
 )  
SELECT MAX(ROWS) AS '员工个数',ORG_ID FROM TABS GROUP BY ORG_ID 

/*SQL Server中将多行数据拼接为一行数据（一个字符串）*/
SQL Server中将多行数据拼接为一行数据（一个字符串） (2013-11-06 18:06:38)转载▼
标签： it	分类： 专业
方法一: 使用T-SQL
DECLARE @Users NVARCHAR(MAX)
SET @Users = ''

SELECT @Users = @Users + ',' + UserName FROM dbo.[User]
WHERE RoleID = 1

SELECT @Users

转载自：http://www.fengfly.com/plus/view-172336-1.html
 
 
方法二:使用for xml path('') 和stuff
--使用 自连接、for xml path('')和stuff合并显示多行数据到一行中   
  
--注   
--1、计算列可以不用包含在聚合函数中而直接显示，如下面语句的val。   
--2、for xml path('') 应该应用于语句的最后面，继而生成xml。   
--3、for xml path('root')中的path参数是生成的xml最顶级节点。   
--4、字段名或是别名将成为xml的子节点，对于没有列名(字段+'')或是没有别名的字段将直接显示。[value] +','则是用,分隔的数据(aa,bb,)。   
--5、对于合并多行数据显示为一行数据时使用自连。   
  
--生成测试表并插入测试数据   
create table tb(id int, value varchar(10))  
insert into tb values(1, 'aa')  
insert into tb values(1, 'bb')  
insert into tb values(2, 'aaa')  
insert into tb values(2, 'bbb')  
insert into tb values(2, 'ccc')  
go  
  
--第一种显示   
select id, [val]=(  
select [value] +',' from tb as b where b.id = a.id for xml path('')) from tb as a  
--第一种显示结果   
--1 aa,bb,   
--1 aa,bb,   
--2 aaa,bbb,ccc,   
--2 aaa,bbb,ccc,   
--2 aaa,bbb,ccc,   
  
  
--第二种显示   
select id, [val]=(  
select [value] +',' from tb as b where b.id = a.id for xml path('')) from tb as a  
group by id  
--第二种显示结果   
--1 aa,bb,   
--2 aaa,bbb,ccc,   
  
--第三种显示   
select id, [val]=stuff((  
select ','+[value] from tb as b where b.id = a.id for xml path('')),1,1,'') from tb as a  
group by id  
--第三种显示结果   
--1 aa,bb   
--2 aaa,bbb,ccc   
  
  
--典型应用   
--AMD_GiftNew中获取所有的管理员ID   
--select adminIds = stuff((select ','+cast(UserId as varchar) from MM_Users where RoleId = 1 and flag =0 for xml path('')),1,1,'')   
--典型应用显示结果   
--3,27
 
转载自：http://blog.csdn.net/kula_dkj/article/details/8568599

 
select PPID, [val]=stuff(( 
 select CAST(SKUCD AS VARCHAR) + ',' from PPSKUTbl as b where b.PPID = a.PPID for xml path('')),1,1,'') from PPSKUTbl as a where PPID = @PPID group by PPID

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


/**/