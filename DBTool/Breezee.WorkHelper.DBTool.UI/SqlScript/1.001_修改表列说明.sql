DECLARE @TABLE_NAME VARCHAR(100),/*表名*/
		@COLUMNS_NAME VARCHAR(100),/*列名*/
		@EXTEND_TEXT VARCHAR(8000), /*扩展属性值*/
		@TABLE_ID INT,/*表ID*/
		@COLUMNS_ID INT,/*表列ID*/
		@curTemp CURSOR
BEGIN		
	SET @curTemp = Cursor For SELECT TABLE_NAME,COLUMNS_NAME,EXTEND_TEXT 
		FROM (/*新增或修改表列扩展属性范围，有多个请用UNION*/
			  #EXTEND_LIST#
			  ) AA		  
	OPEN @curTemp
	FETCH NEXT FROM @curTemp INTO @TABLE_NAME,@COLUMNS_NAME,@EXTEND_TEXT 		  
	WHILE @@fetch_status = 0 
		BEGIN		
			/*表*/
			SELECT @TABLE_ID=object_id FROM sys.objects WHERE type='U' AND name=@TABLE_NAME
			IF @TABLE_ID IS NULL
				BEGIN
					PRINT @TABLE_NAME+'表不存在!\n'
					GOTO Final_Deal
				END
			/*表列*/
			IF ISNULL(@COLUMNS_NAME,'')<>''
				BEGIN
					SELECT @COLUMNS_ID=column_id FROM sys.all_columns WHERE object_id=@TABLE_ID AND name=@COLUMNS_NAME
					IF @COLUMNS_ID IS NULL
					BEGIN
						PRINT @TABLE_NAME+'表的'+@COLUMNS_NAME+'列不存在!\n'
						GOTO Final_Deal
					END
					IF EXISTS(SELECT 1 FROM sys.extended_properties WHERE major_id=@TABLE_ID AND minor_id=@COLUMNS_ID)
					BEGIN
						EXEC sys.sp_dropextendedproperty @name = N'MS_Description',
							@level0type = N'SCHEMA',@level0name = N'dbo', @level1type = N'TABLE',@level1name = @TABLE_NAME,  
							@level2type = N'COLUMN', @level2name = @COLUMNS_NAME
					END
					EXEC sys.sp_addextendedproperty @name = N'MS_Description', @value = @EXTEND_TEXT,
						@level0type = N'SCHEMA',@level0name = N'dbo', @level1type = N'TABLE',@level1name = @TABLE_NAME,  
						@level2type = N'COLUMN', @level2name = @COLUMNS_NAME
				END
			ELSE
				BEGIN
					IF EXISTS(SELECT 1 FROM sys.extended_properties WHERE major_id=@TABLE_ID AND minor_id=0)
					BEGIN
						EXEC sys.sp_dropextendedproperty @name = N'MS_Description',
							@level0type = N'SCHEMA',@level0name = N'dbo', @level1type = N'TABLE',@level1name = @TABLE_NAME,  
							@level2type = null, @level2name = null
					END
					EXEC sys.sp_addextendedproperty @name = N'MS_Description', @value = @EXTEND_TEXT,
						@level0type = N'SCHEMA',@level0name = N'dbo', @level1type = N'TABLE',@level1name = @TABLE_NAME,  
						@level2type = null, @level2name = null
				END
			/*定义最终处理标签*/
			Final_Deal:
			/*取下一条数据*/
			FETCH NEXT FROM @curTemp INTO @TABLE_NAME,@COLUMNS_NAME,@EXTEND_TEXT 
		END 
END 	
close @curTemp
deallocate @curTemp 
GO
