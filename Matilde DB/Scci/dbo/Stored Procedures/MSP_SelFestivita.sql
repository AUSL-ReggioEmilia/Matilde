CREATE PROCEDURE [dbo].[MSP_SelFestivita](@xParametri AS XML)
AS
BEGIN
	   	
	   	
								
	DECLARE @dDataInizio AS DATETIME
	DECLARE @dDataFine AS DATETIME		

	DECLARE @sCodAgenda AS VARCHAR(1800)
			
	SET @dDataInizio = NULL		
	SET @dDataFine = NULL	
				
		DECLARE @sDataTmp AS VARCHAR(20)
	
		SET @sDataTmp=(SELECT TOP 1 ValoreParametro.DataInizio.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/DataInizio') as ValoreParametro(DataInizio))					  
	SET @sDataTmp=ISNULL(@sDataTmp,'')
	IF @sDataTmp<> '' 
		BEGIN
						SET @sDataTmp= SUBSTRING(@sDataTmp,4,2) + '-' 
						+ LEFT(@sDataTmp,2) + '-' +
						+ SUBSTRING(@sDataTmp,7,4) +
						' ' + RIGHT(@sDataTmp,5)
			IF ISDATE(@sDataTmp)=1
				SET	@dDataInizio=CONVERT(DATETIME,@sDataTmp,120)						
			ELSE
				SET	@dDataInizio =NULL			
		END
		
		
		SET @sDataTmp=(SELECT TOP 1 ValoreParametro.DataFine.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/DataFine') as ValoreParametro(DataFine))
	SET @sDataTmp=ISNULL(@sDataTmp,'')
	IF @sDataTmp<> '' 
		BEGIN
						SET @sDataTmp= SUBSTRING(@sDataTmp,4,2) + '-' 
						+ LEFT(@sDataTmp,2) + '-' +
						+ SUBSTRING(@sDataTmp,7,4) +
						' ' + RIGHT(@sDataTmp,5)
			IF ISDATE(@sDataTmp)=1
				SET	@dDataFine=CONVERT(DATETIME,@sDataTmp,120)						
			ELSE
				SET	@dDataFine =NULL		
		END 		

	SET @sCodAgenda=''
	SELECT	@sCodAgenda =  @sCodAgenda +
														CASE 
								WHEN @sCodAgenda='' THEN ''
								ELSE  ','
							END + ''''
						  + ValoreParametro.CodAgenda.value('.','VARCHAR(1800)')
						  + ''''
						 FROM @xParametri.nodes('/Parametri/CodAgenda') as ValoreParametro(CodAgenda)
						 
	SET @sCodAgenda=LTRIM(RTRIM(@sCodAgenda))
	IF	@sCodAgenda='''''' SET @sCodAgenda=''
	SET @sCodAgenda=UPPER(@sCodAgenda)
						
						  
	    DECLARE @xTimeStamp AS XML	

    SET @xTimeStamp=(SELECT TOP 1 ValoreParametro.TS.query('.')
					  FROM @xParametri.nodes('/Parametri/TimeStamp') as ValoreParametro(TS))
	
	SET @xTimeStamp=CONVERT(XML,
						'<Parametri>' + CONVERT(varchar(max),@xTimeStamp) +
						'</Parametri>')
						
												
						
		DECLARE @sSQL AS VARCHAR(MAX)
	DECLARE @sWhere AS VARCHAR(Max)
	DECLARE @sTmp AS VARCHAR(Max)										
						
						SET @sSQL='	SELECT
							*
						  '					
			SET @sSQL=@sSQL + '
						FROM 			
								T_Festivita AS M
						'
													
												
			SET @sWhere=''																																											
										
						IF @dDataInizio IS NOT NULL 
				BEGIN
					SET @sTmp= CASE 
									WHEN @dDataFine IS NULL 
											THEN ' AND M.Data = CONVERT(datetime,'''  + convert(varchar(20),@dDataInizio,120) +''',120)'									
									ELSE ' AND M.Data >= CONVERT(datetime,'''  + convert(varchar(20),@dDataInizio,120) +''',120)'	
								END	
					SET @sWhere= @sWhere + @sTmp								
				END

						IF @dDataFine IS NOT NULL 
				BEGIN
					SET @sTmp= ' AND M.Data <= CONVERT(datetime,'''  + convert(varchar(20),@dDataFine,120) +''',120)'							
					SET @sWhere= @sWhere + @sTmp								
				END
			

						IF 	@sCodAgenda IN ('')
			BEGIN	
								SET @sTmp=  ' AND 			
								M.DATA NOT IN (SELECT Data FROM T_FestivitaAgende )
							'  				
				SET @sWhere= @sWhere + @sTmp														
			END	
			ELSE
			BEGIN
								SET @sTmp=  ' AND 			
								( M.DATA IN (SELECT Data FROM T_FestivitaAgende WHERE CodAgenda IN (' + @sCodAgenda + ')) OR
								  M.DATA NOT IN (SELECT Data FROM T_FestivitaAgende )
								)
							'  				
				SET @sWhere= @sWhere + @sTmp		
			END


															
						IF ISNULL(@sWhere,'')<> ''
			BEGIN			
				SET @sSQL=@sSQL +' WHERE ' + RIGHT(@sWhere,len(@sWhere)-5)
			END	
			
					
	PRINT @sSQL
 	SET @sSQL=@sSQL + ' ORDER BY Data ASC '
 	
 	EXEC (@sSQL)
		
	
								
END