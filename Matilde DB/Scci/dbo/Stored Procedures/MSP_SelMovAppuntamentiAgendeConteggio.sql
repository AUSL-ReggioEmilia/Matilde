CREATE PROCEDURE [dbo].[MSP_SelMovAppuntamentiAgendeConteggio](@xParametri AS XML)
AS
BEGIN
	   	
	   	
				
	DECLARE @bDatiEstesi AS Bit			
	DECLARE @sCodAgenda AS VARCHAR(1800)		
	
	DECLARE @dDataInizio AS DATETIME
	DECLARE @dDataFine AS DATETIME		
		
	DECLARE @sDataTmp AS VARCHAR(20)
	
		DECLARE @sCodRuolo AS Varchar(20)
	DECLARE @sCodUtente AS VARCHAR(100)	
	DECLARE @xTmp AS XML						  				 	
	DECLARE @sGUID AS VARCHAR(Max)
	DECLARE @sSQLQueryAgendeEPI AS VARCHAR(MAX)
	
	DECLARE @nTemp AS INTEGER
	DECLARE @bInserisci AS BIT
	DECLARE @bModifica AS BIT
	DECLARE @bCancella AS BIT
	
	
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
	
		SET @sCodRuolo=(SELECT	TOP 1 ValoreParametro.CodRuolo.value('.','VARCHAR(20)')
						 FROM @xParametri.nodes('/Parametri/TimeStamp/CodRuolo') as ValoreParametro(CodRuolo))						 
	SET @sCodRuolo= LTRIM(RTRIM(ISNULL(@sCodRuolo,'')))		
	
		SET @sCodUtente=(SELECT TOP 1 ValoreParametro.CodLogin.value('.','VARCHAR(100)')
					  FROM @xParametri.nodes('/Parametri/TimeStamp/CodLogin') as ValoreParametro(CodLogin))	
	SET @sCodUtente=ISNULL(@sCodUtente,'')
					  
	    DECLARE @xTimeStamp AS XML	

    SET @xTimeStamp=(SELECT TOP 1 ValoreParametro.TS.query('.')
					  FROM @xParametri.nodes('/Parametri/TimeStamp') as ValoreParametro(TS))
	
	SET @xTimeStamp=CONVERT(XML,
						'<Parametri>' + CONVERT(varchar(max),@xTimeStamp) +
						'</Parametri>')							
											
				
						
			
		DECLARE @sSQL AS VARCHAR(MAX)
	DECLARE @sWhere AS VARCHAR(Max)
	DECLARE @sTmp AS VARCHAR(Max)
	DECLARE @gIDSessione AS UNIQUEIDENTIFIER
	
		
		
			SET @sSQLQueryAgendeEPI=(SELECT dbo.MF_SQLQueryAgendePAZ())
		
				
		SET @sSQL='	SELECT 	
						AGE.CodAgenda,	
						MIN(AG.Descrizione) AS DescrAgenda,
						CONVERT(DATETIME,CONVERT(VARCHAR(10),M.DataInizio,105),105) As Giorno,
						COUNT(*) AS QtaApp
				FROM 
					T_MovAppuntamenti M
						INNER JOIN T_MovAppuntamentiAgende AGE
							ON (M.ID=AGE.IDAppuntamento)
						INNER JOIN T_Agende AG
							ON (AGE.CodAgenda=AG.Codice)'

											
				
	SET @sWhere=''
				
		IF 	@sCodAgenda NOT IN ('')
		BEGIN	
						SET @sTmp=  ' AND 			
							 AGE.CodAgenda IN ('+ @sCodAgenda + ')
						'  				
			SET @sWhere= @sWhere + @sTmp														
		END																		
						
		IF @dDataInizio IS NOT NULL 
		BEGIN
			SET @sTmp= CASE 
							WHEN @dDataFine IS NULL 
									THEN ' AND M.DataInizio = CONVERT(datetime,'''  + convert(varchar(20),@dDataInizio,120) +''',120)'									
							ELSE ' AND M.DataInizio >= CONVERT(datetime,'''  + convert(varchar(20),@dDataInizio,120) +''',120)'	
						END	
			SET @sWhere= @sWhere + @sTmp								
		END

		IF @dDataFine IS NOT NULL 
		BEGIN
			SET @sTmp= ' AND M.DataFine <= CONVERT(datetime,'''  + convert(varchar(20),@dDataFine,120) +''',120)'							
			SET @sWhere= @sWhere + @sTmp								
		END
	
				
	SET @sTmp=  ' AND 			
					 M.CodStatoAppuntamento NOT IN (''CA'',''TR'')	AND
					 AGE.CodStatoAppuntamentoAgenda <> ''CA''
					 			  
				'  	
		
		IF ISNULL(@sTmp,'') <> '' 		
		SET @sWhere= @sWhere + @sTmp	
			
				
		IF ISNULL(@sWhere,'')<> ''
	BEGIN
	
		SET @sSQL=@sSQL +' WHERE ' + RIGHT(@sWhere,len(@sWhere)-5)
	END	
	
	SET @sSQL=@sSQL + '
				GROUP BY 
					AGE.CodAgenda,
					CONVERT(VARCHAR(10),DataInizio,105)
				ORDER BY 
					MIN(AG.Descrizione) ASC,
					CONVERT(DATETIME,CONVERT(VARCHAR(10),M.DataInizio,105),105) ASC 
				'
				
	
	PRINT @sSQL
 	
 		EXEC (@sSQL)
								
				
	EXEC MSP_InsMovTimeStamp @xTimeStamp	
															
				
END