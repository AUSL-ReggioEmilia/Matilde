CREATE PROCEDURE [dbo].[MSP_SelAgendePeriodi](@xParametri AS XML )
AS
BEGIN
	

				 
	DECLARE @sCodAgenda AS VARCHAR(1800)
	DECLARE @dDataInizio AS DATETIME
	DECLARE @dDataFine AS DATETIME		
	
		DECLARE @sDataTmp AS VARCHAR(20)	
	DECLARE @sSQL AS VARCHAR(MAX)	
					
	
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
				
	
	
	IF @sCodAgenda <> '' AND @dDataInizio IS NOT NULL AND @dDataFine IS NOT NULL
	BEGIN
		SET @sSQL='	SELECT * FROM T_AgendePeriodi
					WHERE 
						CodAgenda IN ('+ @sCodAgenda + ') AND
						((DataInizio >= CONVERT(datetime,'''  + convert(varchar(20),@dDataInizio,120) + ''',120) AND
										CONVERT(datetime,'''  + convert(varchar(20),@dDataInizio,120) + ''',120) <= CONVERT(datetime,'''  + convert(varchar(20),@dDataFine,120) + ''',120)) OR

						(DataInizio <= CONVERT(datetime,'''  + convert(varchar(20),@dDataInizio,120) + ''',120) AND
						DataFine <= CONVERT(datetime,'''  + convert(varchar(20),@dDataFine,120) + ''',120)) OR

						(DataInizio >= CONVERT(datetime,'''  + convert(varchar(20),@dDataInizio,120) + ''',120) AND
						DataFine <= CONVERT(datetime,'''  + convert(varchar(20),@dDataFine,120) + ''',120)) OR

						(DataInizio <= CONVERT(datetime,'''  + convert(varchar(20),@dDataInizio,120) + ''',120) AND
						DataFine >= CONVERT(datetime,'''  + convert(varchar(20),@dDataFine,120) + ''',120)))
					'

		PRINT @sSQL
		EXEC (@sSQL)
	END
	ELSE
				SELECT * FROM T_AgendePeriodi WHERE 1=0
	RETURN 0
END