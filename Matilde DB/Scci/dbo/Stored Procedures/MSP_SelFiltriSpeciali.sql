CREATE PROCEDURE [dbo].[MSP_SelFiltriSpeciali](@xParametri AS XML )
AS
BEGIN
				
	
				 
	DECLARE @sCodUA AS Varchar(20)
	DECLARE @sCodRuolo AS Varchar(20)	
	DECLARE @sCodTipoFiltroSpeciale AS Varchar(20)	
				

	SET @sCodUA=(SELECT	TOP 1 ValoreParametro.CodUA.value('.','VARCHAR(20)')
					FROM @xParametri.nodes('/Parametri/CodUA') as ValoreParametro(CodUA))
				   	 				 
	SET @sCodRuolo=(SELECT	TOP 1 ValoreParametro.CodRuolo.value('.','VARCHAR(20)')
					FROM @xParametri.nodes('/Parametri/CodRuolo') as ValoreParametro(CodRuolo)) 
											
	SET @sCodTipoFiltroSpeciale=(SELECT	TOP 1 ValoreParametro.CodTipoFiltroSpeciale.value('.','VARCHAR(20)')
					FROM @xParametri.nodes('/Parametri/CodTipoFiltroSpeciale') as ValoreParametro(CodTipoFiltroSpeciale)) 

				
	DECLARE @sCodEntita VARCHAR(20)
	
		SET @sCodEntita='FLS'
	
			
				CREATE TABLE #tmpUAGerarchia
	(
		CodUA VARCHAR(20) COLLATE Latin1_General_CI_AS,
		Descrizione VARCHAR(255) COLLATE Latin1_General_CI_AS
	)
		
	DECLARE @xTmp AS XML
	
	IF ISNULL(@sCodTipoFiltroSpeciale,'') <> ''
	BEGIN
		
		
				IF @sCodTipoFiltroSpeciale  IN ('PAZAMB','TCDASS','TCDCHI')
		BEGIN		
						IF ISNULL(@sCodUA,'')<> ''
			BEGIN					
								SET  @xTmp=CONVERT(XML,'<Parametri><CodUA>'+ @sCodUA + '</CodUA></Parametri>')
				INSERT #tmpUAGerarchia EXEC MSP_SelUAPadri @xTmp				
			END
		END
		ELSE
		BEGIN
									
						CREATE TABLE #tmpUADaRuolo
			(
				CodUA VARCHAR(20) COLLATE Latin1_General_CI_AS,
				Descrizione VARCHAR(255) COLLATE Latin1_General_CI_AS
			)	
		
			SET	 @xTmp=CONVERT(XML,'<Parametri><CodRuolo>'+ @sCodRuolo + '</CodRuolo></Parametri>')	
			INSERT #tmpUADaRuolo EXEC MSP_SelUADaRuolo @xTmp	
		
									CREATE TABLE #tmpUAPadri
			(
				CodUA VARCHAR(20) COLLATE Latin1_General_CI_AS,
				Descrizione VARCHAR(255) COLLATE Latin1_General_CI_AS
			)
		
			CREATE INDEX IX_CodUA ON #tmpUAPadri (CodUA) 
																	
												DECLARE @sUATemp AS VARCHAR(20)
			
			DECLARE CursoreUA CURSOR FOR 
			SELECT CodUA
				FROM #tmpUADaRuolo
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
		
		
						
		END

						CREATE INDEX IX_CodUA ON #tmpUAGerarchia (CodUA)    					
		
		IF @sCodTipoFiltroSpeciale IN ('EPICAR','AMBCAR','OETRA')
		BEGIN
									SELECT 
				F.Codice,
				F.Descrizione,
				F.SQL
			FROM T_FiltriSpeciali F
					INNER JOIN
						(SELECT DISTINCT 
							A.CodVoce AS Codice												
						FROM					
							T_AssUAEntita A
								INNER JOIN #tmpUAGerarchia T ON
									A.CodUA=T.CodUA					
						WHERE CodEntita=@sCodEntita			
						) AS W
					ON F.Codice=W.Codice	
					INNER JOIN 
												(SELECT DISTINCT
							CodVoce AS Codice
						 FROM T_AssRuoliAzioni
						 WHERE CodEntita='FLS' AND CodAzione='VIS' AND CodRuolo=@sCodRuolo
						 ) AS  R
					ON F.Codice=R.Codice
			WHERE 
				F.CodTipoFiltroSpeciale=@sCodTipoFiltroSpeciale
			ORDER BY F.Descrizione
		END
		ELSE
		BEGIN
						SELECT 
				F.Codice,
				F.Descrizione,
				F.SQL
			FROM T_FiltriSpeciali F
					INNER JOIN
						(SELECT DISTINCT 
							A.CodVoce AS Codice												
						FROM					
							T_AssUAEntita A
								INNER JOIN #tmpUAGerarchia T ON
									A.CodUA=T.CodUA					
						WHERE CodEntita=@sCodEntita			
						) AS W
					ON F.Codice=W.Codice	
			WHERE 
				F.CodTipoFiltroSpeciale=@sCodTipoFiltroSpeciale
			ORDER BY F.Descrizione
		END
				DROP TABLE #tmpUAGerarchia	
	END	
	ELSE
	BEGIN
				SELECT 
			F.Codice,
			F.Descrizione,
			F.SQL
		FROM T_FiltriSpeciali F
		WHERE 1=0
	END

	RETURN 0
END