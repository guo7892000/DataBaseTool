/*格式转换*/
TO_CHAR('#DATE#','YYYY-MM-DD')
TO_DATE('#DATE#','YYYY-MM-DD')

/*注：Oracle的对象命名长度：小于等于30*/

/*查询变更的对象*/
SELECT A.OWNER, A.OBJECT_NAME
  FROM ALL_OBJECTS A
 WHERE A.OBJECT_TYPE IN ('PROCEDURE', 'TRIGGER', 'VIEW')
   AND A.OWNER IN ('UC')
   AND A.LAST_DDL_TIME > SYSDATE - 2
   AND A.STATUS = 'VALID'
UNION
SELECT A.OWNER, A.OBJECT_NAME
  FROM ALL_OBJECTS A
 WHERE A.OBJECT_TYPE IN ('PROCEDURE', 'TRIGGER', 'VIEW')
   AND A.OWNER IN ('IFR', 'IFS')
   AND A.LAST_DDL_TIME > SYSDATE - 2
   AND A.STATUS = 'VALID';

/*创建序列：从10000开始，每次增长1。*/
CREATE SEQUENCE SEQT_PORTAL_RPT_ID MINVALUE 1 MAXVALUE 999999999999999999999999999 START WITH 10000 INCREMENT BY 1 CACHE 20;

/*操作表常用SQL*/
COMMENT ON TABLE TEST1 IS '测试表';--给表填加注释
COMMENT ON COLUMN T_AI_BU_INSURE_ORDER.LINK_NO IS '合同单号';--给列加注释
ALTER TABLE TEST1 MODIFY PLANMONTH NUMBER;
ALTER TABLE TEST1 ADD LONG1 LONG;
ALTER TABLE TEST1 ADD DFD NUMBER(14,2) DEFAULT 0;
ALTER TABLE TEST1 ADD FDFED NUMBER DEFAULT 1;
ALTER TABLE TEST1 ADD CONSTRAINT PK_ID PRIMARY KEY (ID);
ALTER TABLE TEST1 ADD CONSTRAINT FK_D_ID FOREIGN KEY (D_ID) REFERENCES TEST2 (RID);
ALTER TABLE TEST1 ADD CONSTRAINT U_DFD UNIQUE (PLANMONTH);
ALTER TABLE TEST1 DROP COLUMN PLANMONTH;

/*增加列和注释*/
1、给表填加注释：COMMENT ON TABLE 表名 IS '表注释';
2、给列加注释：COMMENT ON COLUMN 表.列 IS '列注释';
3、读取表注释：SELECT * FROM USER_TAB_COMMENTS WHERE COMMENTS IS NOT NULL;
4、读取列注释：SELECT * FROM USER_COL_COMMNENTS WHERE COMMENTS IS NOT NULL AND TABLE_NAME='表名';

/*oracle 排序后取第一条记录*/
select * from (select * from table order by a )C
where rownum=1;

/*查询被锁住的表*/
SELECT /*+ rule */ lpad(' ',decode(l.xidusn ,0,3,0))||l.oracle_username User_name, 
o.owner,o.object_name,o.object_type,s.sid,s.serial# 
FROM v$locked_object l,dba_objects o,v$session s 
WHERE l.object_id=o.object_id 
AND l.session_id=s.sid 
ORDER BY o.object_id,xidusn DESC;

/*解决新增或修改视图报授权选项问题*/
--查询表权限
SELECT * FROM USER_TAB_PRIVS T 
WHERE T.GRANTOR='MDM' AND T.grantee='UC'
ORDER BY T.TABLE_NAME;
select * from T_MDM_COMP_BRAND;
--在UC用户下执行视图V_UC_BASE_ALLCARBRAND变更时报错。提示：ORA-01720：不存在“****.****"授权选项。
--执行以下语句解决
GRANT SELECT ON MDM.T_MDM_COMP_BRAND TO UC WITH GRANT OPTION;

/*增加索引*/
CREATE INDEX IDX1_T_UC_BU_BUY_CONTRACT ON T_UC_BU_BUY_CONTRACT(OPRATE_TIME) TABLESPACE UC_IDX_TBS;

/*返回时间段的自增长的自定列*/
SELECT TO_CHAR(ADD_MONTHS(TO_DATE('20150525', 'YYYYMMDD'), 1 - LEVEL),
                 'YYYYMM') MONTHS
    FROM DUAL
  CONNECT BY LEVEL <= 12;

/*针对重复数据中获取某行数据*/
SELECT *
  FROM (SELECT *
          FROM (SELECT A.*,
                       ROW_NUMBER() OVER(PARTITION BY A.BUYUPNO ORDER BY A.BUYUPNO) RN
                  FROM T_CAR_STORE@MD_DL_EABUC A) X
         WHERE X.RN = 1) A
  LEFT JOIN (SELECT *
               FROM (SELECT B.*,
                            ROW_NUMBER() OVER(PARTITION BY B.BUYUPNO ORDER BY B.BUYUPNO) RN
                       FROM T_MEMU_CONTRACT_M@MD_DL_EABUC B
                      WHERE B.CONTRACT_TYPE IN ('0', '1')) X
              WHERE X.RN = 1) B
    ON A.BUYUPNO = B.BUYUPNO
  LEFT JOIN T_CAR_CATTYPESET@MD_DL_EABUC C
    ON A.CATTYPESETID = C.CATTYPESETID
;

---测试单号生成
DECLARE V_CODE VARCHAR2(50); 
        v_num NUMBER;
BEGIN
	for v_num in 1 .. 100 LOOP
		p_mds_get_form_code('','2057',V_CODE);
		dbms_output.put_line(V_CODE);
	END LOOP;
END;  

/*返回时间段（年、月、日）的自增长的自定列*/
SELECT DECODE('2',/*指定时间类型：1年，2月份，3天*/
  '1',
  ADD_MONTHS(TO_DATE('20160525', 'YYYYMMDD'), (LEVEL-1)*12),
  '2',
  ADD_MONTHS(TO_DATE('20150525', 'YYYYMMDD'), LEVEL-1),
  '3',
  TO_DATE('20150522', 'YYYYMMDD') + LEVEL-1)
FROM DUAL
CONNECT BY LEVEL <= DECODE('2',/*指定时间类型：1年，2月份，3天*/
  '1',
  TO_CHAR(TO_DATE('20160625', 'YYYYMMDD'),'YYYY')-TO_CHAR(TO_DATE('20160125', 'YYYYMMDD'),'YYYY')+1,
  '2',
  FLOOR(MONTHS_BETWEEN(TO_DATE('20150625', 'YYYYMMDD'), TO_DATE('20150501', 'YYYYMMDD')))+1,
  '3',
  TRUNC(TO_DATE('20150523', 'YYYYMMDD'))-TRUNC(TO_DATE('20150522', 'YYYYMMDD'))+1
	)
;

/*增加列*/
  ALTER TABLE emp01 ADD eno NUMBER(4);

/*修改列定义*/
  ALTER TABLE emp01 MODIFY job VARCHAR2(15)  DEFAULT 'CLERK';

/*删除列*/
  ALTER TABLE emp01 DROP COLUMN dno;

/*修改列名*/
  ALTER TABLE emp01 RENAME COLUMN eno TO empno;

/*修改表名*/
  RENAME emp01 TO employee;

/*先删除后增加主键*/
ALTER TABLE MQ_PS_PREPARE DROP CONSTRAINT PK_MQ_PS_PREPARE;
ALTER TABLE MQ_PS_PREPARE ADD CONSTRAINT PK_MQ_PS_PREPARE PRIMARY KEY (OID);

/*增加自增长序列*/
CREATE OR REPLACE TRIGGER TR_MQSEND_TEMP_RECORDID
BEFORE INSERT ON MQSEND_TEMP
FOR EACH ROW
BEGIN
SELECT SE_MQSEND_TEMP.NEXTVAL INTO :NEW.RECORDID FROM DUAL;
END;

/*视图创建同义词并授权*/
GRANT SELECT ON UC.V_UC_QUERY_STORE_CAR TO MDS_ROLE;  
CREATE OR REPLACE SYNONYM MDS.V_UC_QUERY_STORE_CAR FOR UC.V_UC_QUERY_STORE_CAR;  
GRANT SELECT ON UC.V_UC_QUERY_STORE_CAR TO MDS WITH GRANT OPTION;

/*判断影响行*/
UPDATE TB_NETYKRUNLOG SET NETYKSPNAME='P_AUTO_QJ' WHERE  NUM=1 AND NETYKSPNAME IS NULL;
IF SQL%ROWCOUNT=1 THEN
 --dong something
 v:='可d要地一杀敌';
 dbms_output.put_line(substr(v, 2, 3));
END IF;

/*动态删除表SQL：*/
declare  cnt  number;
begin
  ---删除表COC_CAL_GoodReturn
  select count(1) into cnt from user_objects where upper(object_name) = 'COC_CAL_GOODRETURN' and upper(object_type) = 'TABLE'; 
  if cnt = 1 then 
    begin 
      execute immediate 'DROP TABLE COC_CAL_GOODRETURN';
    end;
  end if;
END;

/*动态修改表，并增加注释*/
declare cnt number; 
begin 
 ---修改专营店表增加列DEALER_CODE_O,并增加注释
 select count(1) into cnt from user_tab_columns where upper(table_name) = 'COC_BAS_DEALER' and upper(column_name) = 'DEALER_CODE_O'; 
 if cnt = 0 then 
   begin 
      execute immediate 'ALTER TABLE COC_BAS_DEALER ADD (DEALER_CODE_O varchar2(10))'; 
      execute immediate 'COMMENT ON column COC_BAS_DEALER.DEALER_CODE_O IS ' || '''专营店编码'''; 
   end; 
 end if;
END;

/*合并更新语句*/
MERGE INTO T_PA_BU_PUR_ORDER_D U
USING (SELECT SUM(NVL(B.IN_QTY, 0)) AS T_IN_QTY, B.PART_NO
             FROM T_PA_BU_IN_ORDER A
             LEFT JOIN T_PA_BU_IN_ORDER_D B
               ON A.IN_ORDER_ID = B.IN_ORDER_ID
            WHERE B.SOURCE_ORDER_CODE = V_ORDER_NO /*关联单号:外购单号*/
              and A.net_code = NetCode
            GROUP BY B.PART_NO
	  ) F
ON (U.PART_NO = F.PART_NO)
    WHEN MATCHED THEN
      UPDATE
         SET U.IN_STOCK_QTY      = F.T_IN_QTY,
             U.LAST_UPDATED_DATE = systimestamp
       WHERE POD.PUR_ORDER_ID =
             (select pur_order_id
                from T_PA_BU_PUR_ORDER
               where pur_order_code = V_ORDER_NO
                 and net_code = NetCode); /*采购单ID*/

/*将某一列数据以单列显示*/
SELECT A.PART_BRAND_CODE,
       LISTAGG(A.CAR_BRAND_CODE, ',') WITHIN GROUP(ORDER BY A.PART_BRAND_CODE) AS CAR_BRAND_CODE
  FROM T_PA_DB_PART_BRAND_DETAIL A
 WHERE A.IS_ENABLE = '1'
 GROUP BY A.PART_BRAND_CODE;

/*视图授权与创建同义词*/
grant select on uc.v_uc_carprice to mds ;
create or replace synonym mds.v_uc_carprice for uc.v_uc_carprice;

/*取年月日*/
select to_char(sysdate,'yyyy'), to_char(sysdate, 'mm'), to_char(sysdate, 'dd') from dual; 

/*查询子字符和赋值应用*/
declare v varchar2(20);
begin
 v:='可d要地一杀敌';
 dbms_output.put_line(substr(v, 2, 3));
end;

/*添加JOB*/
DECLARE JOBID NUMBER;
BEGIN
  SELECT MAX(JOB)+1 INTO JOBID FROM ALL_JOBS;
  DBMS_JOB.SUBMIT(JOBID, 'P_PA_DELETE_OVERDUE_LACK_AUTO;', SYSDATE,'TRUNC(SYSDATE+1)');
  COMMIT;
END;

/*修改JOB*/
declare jobID number;
begin
  select JOB into jobID from all_jobs where what='P_SE_IR_DAY_ONCE;';
  dbms_job.change(jobID ,'P_SE_IR_DAY_ONCE;', sysdate+1,'sysdate+1/24' );
  commit;
end;

/*JOB的其他命令*/
begin
 dbms_job.remove(41); --删除JO
 dbms_job.broken(25,true); --停止job
 dbms_job.run(25); --运行job
 dbms_job.what(v_job,'sp_fact_charge_code;'); --修改What内容
 dbms_job.next_date(v_job,sysdate); --修改某个job名 修改下一次运行时间
end;

--查看任务：
select * from user_jobs;select * from all_jobs;
--查看正在运行的任务（不推荐使用，速度慢）：
select * from dba_jobs_running;


/*声明*/
v_Dms_NetCode eabuc.t_org_dealer.dealerno%Type;
Cursor curBalance is
    Select * from eabuc.PS_U_T_MEMU_CERTIFICATE;
  RecBalance curBalance%RowType;
begin
  open curBalance;
  loop
    Fetch curBalance
      into RecBalance;
    Exit when curBalance%NotFound;
	  begin
		/*逻辑处理，此处略*/

		  commit;
		WHEN OTHERS THEN
			/*事务回滚*/
			Rollback; 
			/*抛出系统错误*/
			RAISE_APPLICATION_ERROR(-20999, sqlerrm);
	 end;
  end loop;
  close curBalance;
end;

/*字符连接*/
select concat('中国','人民') "test",'中国'||'人民' from dual;

/*INITCAP(n)函数*/ 
 将字符串n中每个单词首字母大写，其余小写(区分单词的规则是按空格或非字母字符；可以输入中文字符，但没有任何作用)。例如：
select initcap('中 国 人 民') "test",initcap('my word') "test1",initcap('my中国word') "test2" from dual;

/*INSTR(chr1,chr2,[n,[m]])函数*/
获取字符串chr2在字符串chr1中出现的位置。n和m可选,省略是默认为1；n代表开始查找的起始位置，当n为负数从尾部开始搜索；m代表字串出现的次数。例如：
select instr('pplkoopijk','k',-1,1) "test",instr('pplkoopijk','k',1,2) from dual;

/*LENGTH(n)函数*/
 返回字符或字符串长度。(当n为null时，返回nll；返回的长度包括后面的空格)。例如：
select length('ppl ') "test",length(null) "test1",length(trim('ppl ')) "test" from dual;

/*LOWER(n)函数*/
 将n转换为小写;UPPER(n)函数的描述: 将n转换为大写。例如：
select lower('KKKD') "test",UPPER('hello') from dual;

/*LPAD(chr1,n,[chr2])函数*/
在chr1左边填充字符chr2，使得字符总长度为n。chr2可选，默认为空格；当chr1字符串长度大于n时，则从左边截取chr1的n个字符显示。RPAD(chr1,n,chr2)函数的描述：在chr1右边填充chr2，使返回字符串长度为n..当chr1长度大于n时，返回左端n个字符。例如：
select lpad('kkk',5) "test",lpad('kkkkk',4) "test1",lpad('kkk',6,'lll') "test2" from dual;

/*LTRIM(chr,[n])函数*/
去掉字符串chr左边包含的n字符串中的任何字符，直到出现一个不包含在n中的字符为止。例如：
select ltrim('abcde','a') "test",ltrim('abcde','b') "test1",ltrim('abcdefg','cba') "test2",ltrim('  abcdefg') from dual;

/*REPLACE(chr,search_string,[,replacement_string])函数*/
将chr中满足search_string条件的替换为replacement_string指定的字符串，当search_string为null时，返回chr；当replacement_string为null时，返回chr中截取掉search_string部分的字符串。例如：
SELECT REPLACE('abcdeef','e','oo') "test",REPLACE('abcdeef','ee','oo') "test1",REPLACE('abcdeef',NULL,'oo') "test2",REPLACE('abcdeef','ee',NULL) "test3" FROM dual;

/*查询版本*/
select * from v$version

/*备份表语句*/
create table userinfo_bak as select * from userinfo;

/*变动日期时间数值*/
select
trunc(sysdate)+(interval '1' second), --加1秒(1/24/60/60)
trunc(sysdate)+(interval '1' minute), --加1分钟(1/24/60)
trunc(sysdate)+(interval '1' hour), --加1小时(1/24)
trunc(sysdate)+(INTERVAL '1' DAY),  --加1天(1)
trunc(sysdate)+(INTERVAL '1' MONTH), --加1月
trunc(sysdate)+(INTERVAL '1' YEAR), --加1年
trunc(sysdate)+(interval '01:02:03' hour to second), --加指定小时到秒
trunc(sysdate)+(interval '01:02' minute to second), --加指定分钟到秒
trunc(sysdate)+(interval '01:02' hour to minute), --加指定小时到分钟
trunc(sysdate)+(interval '2 01:02' day to minute) --加指定天数到分钟
from dual;

/*使用临时表查询SQL*/
WITH tempKPI AS (select TO_NUMBER(SUBSTR(KPI.YEAR_MONTH, -2)) AS MONTHS,
       KPI.PART_PZ_RATE /*备件品种满足率*/,
       KPI.NEED_VARIETY_ALL /*必备件品种满足率*/,
       KPI.LP_RELIEVE_TIME_ALL /*缺件平均解除时间*/,
       KPI.STORE_RATE /*库存度*/,
       KPI.BO_RATE /*长期BO率*/,
       KPI.ORDER_RATE /*订货波动率*/,
       KPI.REACH_RATE /*到货满足率*/,
       KPI.EO_RATE /*紧急订单率EO*/
 FROM T_PA_BU_PART_MONTH_KPI KPI
 WHERE KPI.NET_CODE= 'H2904'--'#NET_CODE#'
 AND SUBSTR(KPI.YEAR_MONTH,0,4) = '2011'--'#YYYY#%'
 ) 
select '', decode(months,1,PART_PZ_RATE,0)
from tempKPI


/*截取日期*/
select sysdate 当时日期,
to_char(sysdate-1,'d') 星期几,
round(sysdate) 最近0点日期,
round(sysdate,'day') 最近星期日,
round(sysdate,'month') 最近月初,
round(sysdate,'q') 最近季初日期, 
round(sysdate,'year') 最近年初日期 from dual;

/*时间差值*/
select 
round(to_number(end_date-start_date)) "消逝的时间（以天为单位）", 
round(to_number(end_date-start_date)*24) "消逝的时间（以小时为单位）", 
round(to_number(end_date-start_date)*1440) "消逝的时间（以分钟为单位）"
from dual;

/*SQL中的SQL，把单引号换成两个单引号就好了*/
SELECT 'INSERT INTO (a, b,c,d) VALUES(''aa'', 2, '''',3)' from dual;

/*转换为毫秒字符*/
select to_char(systimestamp,'yyyy-MM-dd HH24:mi:ss.ff3') last_updated_date,TO_TIMESTAMP('2012-07' || '-01 00:00:00:000000','YYYY-MM-DD hh24:mi:ss:ff3')
from t_se_bu_tool_store

/*oracle 10g 创建和删出表空间，用户，表*/
--第一步：创建表空间（要建两个临时表空间和数据表空间） 
--1、创建临时表空间： 
--保证目录（E:/oracle/product/10.2.0/oradata/hui）存在
create temporary tablespace hui_temp 
tempfile 'E:/oracle/product/10.2.0/oradata/hui/hui_temp01.dbf' 
size 100m 
autoextend on 
next 100m maxsize 500m 
extent management local; 

---
--2、创建数据表空间： 
CREATE TABLESPACE test 
DATAFILE 'E:/oracle/product/10.2.0/oradata/hui/hui_data01.DBF' 
SIZE 1000M AUTOEXTEND ON NEXT 100M MAXSIZE UNLIMITED LOGGING PERMANENT EXTENT 
MANAGEMENT LOCAL AUTOALLOCATE BLOCKSIZE 8K SEGMENT SPACE MANAGEMENT MANUAL FLASHBACK ON; 
---
--第二步：创建用户 
create user hui identified by hui 
default tablespace test 
temporary tablespace hui_temp; 
--其中test为用户名，testpassword为密码 
--最后：给用户授予权限 
grant connect,resource to hui; 

--建表就和其它数据库没有什么区别了，普通sql 
--如： 
create table test_user( 
first_name varchar2(15), 
last_name varchar2(20) 
); 

--删除用户：test 
drop user test cascade 
--删除表空间： 
DROP TABLESPACE hui_temp INCLUDING CONTENTS AND DATAFILES; 
--删除表: 
delete from users;


/*执行存储过程*/
begin
   p_se_if_to_other_system('SE_TOOL_MAINTAIN_002','D2305','MD23051207001',null,null,null,null);
end;

/*删除重复的JOB*/
declare jobID number;
CURSOR CUR_UC IS
      select what,max(job) max_job  from all_jobs 
      where what in (select what from (select what,count(1) icount
                        from all_jobs
                        group by what
                        having count(1)>1)
      )
      --and what='P_DC_IS_NO_CONSUME;'
      group by what;
      REC_UC  CUR_UC%ROWTYPE;
BEGIN
  OPEN CUR_UC;
  LOOP
    FETCH CUR_UC
      INTO REC_UC;
    IF CUR_UC%FOUND THEN
        declare CURSOR CUR_UC2 IS select job from all_jobs where what=REC_UC.what;
         REC_UC2 CUR_UC2%ROWTYPE;
        begin 
          OPEN CUR_UC2;
          LOOP
            FETCH CUR_UC2
              INTO REC_UC2; 
          IF CUR_UC2%FOUND THEN
            if REC_UC2.job<>REC_UC.max_job then
               begin
                 dbms_job.remove(REC_UC2.job);
                 commit;
               end;
            end if; 
          ELSE
            CLOSE CUR_UC2;
            EXIT;
          END IF;  
          END LOOP;
        end;
    ELSE
      CLOSE CUR_UC;
      EXIT;
    END IF;  
  END LOOP; 
end;   

/*动态SQL返回值*/
--准备数据
create table EE(REPORT_M_ID INTEGER,YEAR VARCHAR(20),YEAR_MONTH VARCHAR(20),START_DATE VARCHAR(20), ORG_CODE VARCHAR(20),ORDER_NO INTEGER,SON_ORG_CODE VARCHAR2(20));
INSERT INTO EE SELECT 1,'2012','201206','2012-07-01','DLR1',1,'a,b,c,d' from dual;
INSERT INTO EE SELECT 2,'2012','201206','2012-07-01','DLR1',2,'a,c' from dual;
--动态SQL使用
declare 
      V_SON_ORG_CODE VARCHAR2(200);
      V_SON_ORG_CODE1 VARCHAR2(200);
      vStrTempSql VARCHAR2(2000);
      V_REPORT_FREQUENCY VARCHAR2(2);
      --新加
      V_CONN_REAL VARCHAR2(200);
BEGIN
  V_REPORT_FREQUENCY := 'Y';
  select decode(V_REPORT_FREQUENCY,'Y',' AND CC.YEAR=TT.YEAR ',
         	'M',' AND CC.START_DATE=TT.START_DATE ',
          ' AND CC.YEAR_MONTH=TT.YEAR_MONTH ') into V_CONN_REAL from dual;     
  vStrTempSql :='SELECT TT.SON_ORG_CODE,(SELECT SON_ORG_CODE FROM EE CC
    WHERE CC.ORG_CODE = TT.ORG_CODE ' || V_CONN_REAL || ' AND TT.ORDER_NO-1=CC.ORDER_NO ) SON_ORG_CODE1 
     FROM EE TT WHERE REPORT_M_ID=2 ';
    dbms_output.put_line(vStrTempSql);
    execute immediate vStrTempSql into V_SON_ORG_CODE,V_SON_ORG_CODE1;             
    dbms_output.put_line(V_SON_ORG_CODE);
    dbms_output.put_line(V_SON_ORG_CODE1);          
END; 

/*动态SQL中字符的处理*/
declare strSql varchar2(8000);
begin
  strSql:='UPDATE T_FI_BU_GATHERING_ORDER_D T 
            SET T.VOUCHER_STATUE = V_VOUCHERSTATE
       WHERE NET_CODE = V_NETCODE
       AND T.GATHERING_ORDER_D_ID IN(V_ORDERID)
       AND T.IS_ENABLE=''1'' and IS_SYSTEM=''0''';
  dbms_output.put_line(strSql);     
end;

/*查询存储过程内容*/
select * from dba_source a where a.name='SP_MDM_PRO_SEND_TOUCCUST'

/*行转列*/
WITH tempKPI AS (select TO_NUMBER(SUBSTR(KPI.YEAR_MONTH, -2)) AS MONTHS,
       KPI.PART_PZ_RATE /*备件品种满足率*/,
       KPI.NEED_VARIETY_ALL /*必备件品种满足率*/
 FROM T_PA_BU_PART_MONTH_KPI KPI
 WHERE KPI.NET_CODE= '#NET_CODE#'
 AND SUBSTR(KPI.YEAR_MONTH,0,4) = '#YYYY#'
 ) 
select '总体满足率(%)' ""CAL_TYPE"", 
sum(decode(months,1,PART_PZ_RATE,0)) ""1"",sum(decode(months,2,PART_PZ_RATE,0)) ""2"",sum(decode(months,3,PART_PZ_RATE,0)) ""3"",
sum(decode(months,4,PART_PZ_RATE,0)) ""4"",sum(decode(months,5,PART_PZ_RATE,0)) ""5"",sum(decode(months,6,PART_PZ_RATE,0)) ""6"",
sum(decode(months,7,PART_PZ_RATE,0)) ""7"",sum(decode(months,8,PART_PZ_RATE,0)) ""8"",sum(decode(months,9,PART_PZ_RATE,0)) ""9"",
sum(decode(months,10,PART_PZ_RATE,0)) ""10"",sum(decode(months,11,PART_PZ_RATE,0)) ""11"",sum(decode(months,12,PART_PZ_RATE,0)) ""12""
from tempKPI
union 
select '必备件品种满足率(%)' ""CAL_TYPE"", 
sum(decode(months,1,NEED_VARIETY_ALL,0)) ""1"",sum(decode(months,2,NEED_VARIETY_ALL,0)) ""2"",sum(decode(months,3,NEED_VARIETY_ALL,0)) ""3"",
sum(decode(months,4,NEED_VARIETY_ALL,0)) ""4"",sum(decode(months,5,NEED_VARIETY_ALL,0)) ""5"",sum(decode(months,6,NEED_VARIETY_ALL,0)) ""6"",
sum(decode(months,7,NEED_VARIETY_ALL,0)) ""7"",sum(decode(months,8,NEED_VARIETY_ALL,0)) ""8"",sum(decode(months,9,NEED_VARIETY_ALL,0)) ""9"",
sum(decode(months,10,NEED_VARIETY_ALL,0)) ""10"",sum(decode(months,11,NEED_VARIETY_ALL,0)) ""11"",sum(decode(months,12,NEED_VARIETY_ALL,0)) ""12""
from tempKPI;

/*查看oracle数据文件存放路径*/
select * from v$datafile;


--实现自增长序列的触发器
CREATE OR REPLACE TRIGGER TRG_PK_PS_QJ_SH_BAK
 BEFORE INSERT ON PS_QJ_SH_BAK
 FOR EACH ROW
BEGIN
 SELECT SEQ_PK_PS_QJ_SH_BAK.NEXTVAL INTO :NEW.RECORDNO FROM DUAL;
END;

--获取含字母的流水号
CREATE OR REPLACE FUNCTION F_MDS_GET_WORD_SEQUENCE_NO
(
  V_SEQUENCE_NO INTEGER, ---加1后的十进制流水号，例如：10
  V_SEQUENCE_LENGTH INTEGER --流水号长度：例如4
)
/*******************************************************************************************
* 对象名称：获取含字母的流水号
* 创建作者：黄国辉
* 创建日期：2014-11-12
* 对象描述:  传入流水号及流水号长度，返回可能包含字母的流水号。
*       这样的流水号比原来只有数字的会多一些。
* 变更历史(格式：版本号\本次变更内容简述\修改人\修改日期)：
*     V1.00：新增 HGH 2014-11-12
*     V1.01：完全重写了该算法，解决之前有重复单号的问题 HUANGGH 2015-8-3
********************************************************************************************/
RETURN VARCHAR2
IS
  --自定义流水号的相关变量
  V_MY_DEFINE_VALUE  VARCHAR2(50); --自定义的流水号
  V_MY_DEFINE_WORD_LIST VARCHAR2(50):= '0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ'; --自定义字符集
  N_MY_DEFINE_WORD_LIST_LEN NUMBER;  --自定义字符集长度

  I_AFTER_DIVIDE_VALUE INTEGER:=0; --除后的值
	N_MOD_VALUE           NUMBER; --余数值
	V_LOOP_NUM            NUMBER; --循环中的值
BEGIN
  --有效性判断
  IF V_SEQUENCE_NO IS NULL OR V_SEQUENCE_LENGTH IS NULL THEN
    RAISE_APPLICATION_ERROR(-20999,'传入的流水号、流水号长度都不能为空！');
  END IF;

  IF V_SEQUENCE_LENGTH <=0 THEN
    RAISE_APPLICATION_ERROR(-20999,'传入的流水号长度必须大于0！');
  END IF;

  --变量初始化
  N_MY_DEFINE_WORD_LIST_LEN := LENGTH(V_MY_DEFINE_WORD_LIST);--自定数据集长度
  V_MY_DEFINE_VALUE:=''; --返回的值
	I_AFTER_DIVIDE_VALUE:=V_SEQUENCE_NO; --值为传入的流水号

  --循环转换为流水号
	FOR V_LOOP_NUM IN 1 .. V_SEQUENCE_LENGTH LOOP
		  --得到余数值
			N_MOD_VALUE := MOD(I_AFTER_DIVIDE_VALUE,N_MY_DEFINE_WORD_LIST_LEN);
			--得到除后的值
			I_AFTER_DIVIDE_VALUE :=FLOOR(I_AFTER_DIVIDE_VALUE/N_MY_DEFINE_WORD_LIST_LEN);--这里要向下取整
			--并接
			V_MY_DEFINE_VALUE:=  SUBSTR(V_MY_DEFINE_WORD_LIST,N_MOD_VALUE + 1, 1) || V_MY_DEFINE_VALUE;
			IF I_AFTER_DIVIDE_VALUE < N_MY_DEFINE_WORD_LIST_LEN THEN --当除数小于进制数时退出
				 --得到前一位
				 V_MY_DEFINE_VALUE:=  SUBSTR(V_MY_DEFINE_WORD_LIST,I_AFTER_DIVIDE_VALUE + 1, 1) || V_MY_DEFINE_VALUE;
				 EXIT; --退出循环
			END IF;
	END LOOP;
	
	V_MY_DEFINE_VALUE :=LPAD(V_MY_DEFINE_VALUE,V_SEQUENCE_LENGTH,'0');	
  V_MY_DEFINE_VALUE := SUBSTR(V_MY_DEFINE_VALUE,LENGTH(V_MY_DEFINE_VALUE)-V_SEQUENCE_LENGTH,V_SEQUENCE_LENGTH);

  --返回值
  RETURN(V_MY_DEFINE_VALUE);
END;

