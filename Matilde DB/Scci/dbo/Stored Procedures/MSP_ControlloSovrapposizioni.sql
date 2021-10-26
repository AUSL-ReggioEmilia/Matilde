
CREATE PROCEDURE [MSP_ControlloSovrapposizioni](@xParametri AS XML)
AS
BEGIN
	   	
	   	
					
	DECLARE @sCodAgenda AS VARCHAR(1800)		
	
	DECLARE @sDataInizio AS varchar(30)
	DECLARE @sDataFine AS varchar(30)
	DECLARE @sDataTmp AS VARCHAR(20)

	DECLARE @sIdAppuntamento AS varchar(50)		
	
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
	SET @sDataInizio=ISNULL(@sDataTmp,'')

		SET @sDataTmp=(SELECT TOP 1 ValoreParametro.DataFine.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/DataFine') as ValoreParametro(DataFine))
	SET @sDataFine=ISNULL(@sDataTmp,'')
	
	IF @xParametri.exist('/Parametri/IdAppuntamento')=1	
	BEGIN
		SET @sIdAppuntamento=(SELECT TOP 1 ValoreParametro.IdAppuntamento.value('.','VARCHAR(50)')
						FROM @xParametri.nodes('/Parametri/IdAppuntamento') as ValoreParametro(IdAppuntamento))	
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
	
		
				
		SET @sSQL='SELECT COUNT(*)
				FROM
				(
					SELECT 	
						M.ID,
						DataInizio,
						DataFine,		
						CodStatoAppuntamento,
						CodStatoAppuntamentoAgenda
					FROM 
						T_MovAppuntamenti M
						INNER JOIN T_MovAppuntamentiAgende AGE ON AGE.IDAppuntamento = M.ID
					WHERE 
						(									
							(
							M.DataInizio <= CONVERT(datetime,''' + @sDataInizio + ''',120) AND 
							M.DataFine > CONVERT(datetime,''' + @sDataInizio + ''',120)  		
							) OR
							(
							M.DataInizio < CONVERT(datetime,''' + @sDataFine + ''',120) AND 
							M.DataFine >= CONVERT(datetime,''' + @sDataFine + ''',120) 
							) OR
							(
							M.DataInizio >= CONVERT(datetime,''' + @sDataInizio + ''',120) AND 
							M.DataFine <= CONVERT(datetime,''' + @sDataFine + ''',120) 
							)
						) AND
						M.CodStatoAppuntamento NOT IN (''CA'', ''DP'')	AND
						AGE.CodStatoAppuntamentoAgenda <> ''CA''
					'
					IF 	@sCodAgenda <> ''
						BEGIN	
														SET @sSQL= @sSQL + ' AND 			
											 AGE.CodAgenda IN ('+ @sCodAgenda + ')'  																
						END	
						
					IF 	@sIdAppuntamento <> ''
						BEGIN	
														SET @sSQL= @sSQL + ' AND 			
											 CONVERT(varchar(50), M.ID) <> '''+ @sIdAppuntamento + ''''															
						END									

		SET @sSQL = @sSQL + 
					'
					UNION 

					SELECT 	
						N.ID,
						DataInizio,
						DataFine,		
						CodStatoNota,
						NULL

					FROM 
						T_MovNoteAgende N
					WHERE 			
						(
							(
							N.DataInizio <= CONVERT(datetime,''' + @sDataInizio + ''',120) AND 
							N.DataFine > CONVERT(datetime,''' + @sDataInizio + ''',120)
							) OR
							(
							N.DataInizio < CONVERT(datetime,''' + @sDataFine + ''',120) AND 
							N.DataFine >= CONVERT(datetime,''' + @sDataFine + ''',120) 
							) OR
							(
							N.DataInizio >= CONVERT(datetime,''' + @sDataInizio + ''',120) AND 
							N.DataFine <= CONVERT(datetime,''' + @sDataFine + ''',120) 
							)
						) AND
						N.CodStatoNota NOT IN (''CA'')
					'
					IF 	@sCodAgenda <> ''
						BEGIN	
														SET @sSQL= @sSQL + ' AND 			
											 N.CodAgenda IN ('+ @sCodAgenda + ')'  																
						END		
					IF 	@sIdAppuntamento <> ''
						BEGIN	
														SET @sSQL= @sSQL + ' AND 			
											 CONVERT(varchar(50), N.ID) <> '''+ @sIdAppuntamento + ''''  																
						END	
		SET @sSQL = @sSQL + ') X'
	

											
	
	PRINT @sSQL
 	
 		EXEC (@sSQL)
								
				
	EXEC MSP_InsMovTimeStamp @xTimeStamp	
															
				
END