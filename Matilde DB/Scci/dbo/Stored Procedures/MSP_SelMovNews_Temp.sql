
CREATE PROCEDURE [MSP_SelMovNews_Temp](@xParametri AS XML)
AS
BEGIN
	   	
	   	
					
	DECLARE @nNumRighe AS INTEGER
	DECLARE @sCodRuolo AS VARCHAR(20)
	DECLARE @sCodLogin AS VARCHAR(100)
	DECLARE @sCodTipoNews AS VARCHAR(20)
	DECLARE @bDatiEstesi AS Bit
	DECLARE @bNonVisionate AS Bit
	DECLARE @bContaNews AS BIT
	DECLARE @nID AS INTEGER				
			
	SET @nNumRighe=(SELECT	TOP 1 ValoreParametro.NumRighe.value('.','INTEGER')
								FROM @xParametri.nodes('/Parametri/NumRighe') as ValoreParametro(NumRighe))
	
	SET @nNumRighe=ISNULL(@nNumRighe,0)
	
		SET @sCodRuolo=(SELECT TOP 1 ValoreParametro.CodRuolo.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/TimeStamp/CodRuolo') as ValoreParametro(CodRuolo))											
	SET @sCodRuolo=ISNULL(@sCodRuolo,'')
	
		SET @sCodLogin=(SELECT TOP 1 ValoreParametro.CodLogin.value('.','VARCHAR(100)')
					  FROM @xParametri.nodes('/Parametri/TimeStamp/CodLogin') as ValoreParametro(CodLogin))											
	SET @sCodLogin=ISNULL(@sCodLogin,'')
	

		SET @sCodTipoNews=(SELECT TOP 1 ValoreParametro.CodTipoNews.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodTipoNews') as ValoreParametro(CodTipoNews))											
	SET @sCodTipoNews=ISNULL(@sCodTipoNews,'')

		SET @bDatiEstesi=(SELECT TOP 1 ValoreParametro.DatiEstesi.value('.','bit')
					  FROM @xParametri.nodes('/Parametri/DatiEstesi') as ValoreParametro(DatiEstesi))					  
	SET @bDatiEstesi=ISNULL(@bDatiEstesi,0)
	
		SET @bNonVisionate=(SELECT TOP 1 ValoreParametro.NonVisionate.value('.','bit')
					  FROM @xParametri.nodes('/Parametri/NonVisionate') as ValoreParametro(NonVisionate))					  
	SET @bNonVisionate=ISNULL(@bNonVisionate,0)

		SET @bContaNews=(SELECT TOP 1 ValoreParametro.ContaNews.value('.','bit')
					  FROM @xParametri.nodes('/Parametri/ContaNews') as ValoreParametro(ContaNews))					  
	SET @bContaNews=ISNULL(@bContaNews,0)

	SET @nID=(SELECT TOP 1 ValoreParametro.ID.value('.','INTEGER')
								FROM @xParametri.nodes('/Parametri/ID') as ValoreParametro(ID))
		
				
				
				
	IF @sCodRuolo <> '' 
	BEGIN
								CREATE TABLE #tmpUA
		(
			CodUA VARCHAR(20) COLLATE Latin1_General_CI_AS,
			Descrizione VARCHAR(255) COLLATE Latin1_General_CI_AS
		)

		DECLARE @xTmp AS XML
		
		SET @xTmp =CONVERT(XML,'<Parametri><CodRuolo>'+@sCodRuolo+'</CodRuolo></Parametri>')
		
		INSERT #tmpUA EXEC MSP_SelUADaRuolo @xTmp		
			
		CREATE INDEX IX_CodUA ON #tmpUA (CodUA)   
		
					
				CREATE TABLE #tmpUAPadri
		(
			CodUA VARCHAR(20) COLLATE Latin1_General_CI_AS,
			Descrizione VARCHAR(255) COLLATE Latin1_General_CI_AS
		)
		
		CREATE INDEX IX_CodUA ON #tmpUAPadri (CodUA) 
												
								CREATE TABLE #tmpUAGerarchia
		(
			CodUA VARCHAR(20) COLLATE Latin1_General_CI_AS,
			Descrizione VARCHAR(255) COLLATE Latin1_General_CI_AS
		)
		
		CREATE INDEX IX_CodUA ON #tmpUAGerarchia (CodUA) 
			
								DECLARE @sUATemp AS VARCHAR(20)
			
		DECLARE CursoreUA CURSOR FOR 
		SELECT CodUA
			FROM #tmpUA
		FOR READ ONLY ;
		
		OPEN CursoreUA
		
		FETCH NEXT FROM CursoreUA INTO @sUATemp
			
		WHILE @@FETCH_STATUS = 0
		BEGIN	
		
			SET @xTmp =CONVERT(XML,'<Parametri><CodUA>'+@sUATemp+'</CodUA></Parametri>')
		
			INSERT #tmpUAPadri EXEC MSP_SelUAPadri @xTmp	
			
			INSERT INTO #tmpUAGerarchia(CodUA,Descrizione)
			SELECT CodUA,Descrizione
			FROM #tmpUAPadri
			WHERE CodUA NOT IN (SELECT CodUA FROM #tmpUAGerarchia)
			
			FETCH NEXT FROM CursoreUA INTO @sUATemp
		END
		
		 SELECT 'tmpUAGerarchia', * FROM #tmpUAGerarchia	
					
		DECLARE  @sSQL AS VARCHAR(4000)
		
		SET @sSQL='	SELECT	'
		
		
		
				IF @bContaNews=1
			BEGIN
				SET @sSQL=@sSQL + ' 
						COUNT(ID) AS Qta 
						'
			END
		ELSE
			BEGIN
								IF @nNumRighe<> 0  SET @sSQL= @sSQL + ' TOP ' + convert(varchar(10),@nNumRighe)+  ' ' 
				
				SET @sSQL=@sSQL + ' 
						ID,
						DataOra,
						Titolo,'			

								IF @bDatiEstesi=0 						
					SET @sSQL=@sSQL + 'NULL AS TestoRTF,' 
				ELSE
					SET @sSQL=@sSQL + 'TestoRTF,' 		
				
				SET @sSQL=@sSQL + 'Rilevante, Codice '

			END

				SET @sSQL=@sSQL + '
					FROM 
						T_MovNews
					WHERE '

						
				
		SET @sSQL=@sSQL + 'DataInizioPubblicazione < GETDATE()	AND 
						   DataFinePubblicazione >= GETDATE() AND '
		
				IF @nID IS NOT NULL
			SET @sSQL=@sSQL + 'ID=' + CONVERT(VARCHAR(20),@nID)	+ ' AND '

				IF ISNULL(@sCodTipoNews,'') <> ''
			BEGIN				
								SET @sSQL=@sSQL + 'CodTipoNews=''' + @sCodTipoNews	+ ''' AND '
			END

				IF ISNULL(@sCodTipoNews,'') IN ('LITE','HARD') AND @bNonVisionate=1
			BEGIN				
				SET @sSQL=@sSQL + ' Codice NOT IN (SELECT CodNews FROM T_MovNewsLog WHERE CodUtenteVisione=''' + @sCodLogin + ''' ) AND '				
			END

		SET @sSQL=@sSQL + 'Codice IN (SELECT CodVoce
								   FROM T_AssUAEntita
								   WHERE CodEntita=''NWS'' AND 
										 CodUA IN (SELECT CodUA FROM #tmpUAGerarchia)
								   ) '
		
										IF @bContaNews=0
			BEGIN
				SET @sSQL=@sSQL + 'ORDER BY 	
									Rilevante DESC , 
									DataOra DESC
								'
				EXEC (@sSQL)
			END
		ELSE
			BEGIN
				EXEC (@sSQL)
			END
					
		
		
		PRINT @sSQL

		SELECT 'voci T_AssUAentita', CodUA, CodVoce
								   FROM T_AssUAEntita
								   WHERE CodEntita='NWS' AND 
										 CodUA IN (SELECT CodUA FROM #tmpUAGerarchia) order by CodUA, CodVoce

	END
	ELSE
	BEGIN
				SELECT 
			ID,
			DataOra,
			Titolo,
			TestoRTF,
			Rilevante,
			Codice
		FROM T_MovNews
		WHERE 1=0
	END
			
END