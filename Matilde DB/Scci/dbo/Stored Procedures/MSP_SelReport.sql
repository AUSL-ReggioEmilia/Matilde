CREATE  PROCEDURE [dbo].[MSP_SelReport](@xParametri AS XML)
AS
BEGIN
								
	
				
	DECLARE @sCodice AS Varchar(20)  
	DECLARE @sCodUA AS Varchar(20) 
	DECLARE @sCodRuolo AS Varchar(20)
	DECLARE @sCodMaschera AS VARCHAR(20)	
	DECLARE @bDatiEstesi AS Bit
			

	SET @sCodice=(SELECT TOP 1 ValoreParametro.Codice.value('.','VARCHAR(20)')
						 FROM @xParametri.nodes('/Parametri/Codice') as ValoreParametro(Codice))
	
	SET @sCodUA=(SELECT	TOP 1 ValoreParametro.CodUA.value('.','VARCHAR(20)')
						 FROM @xParametri.nodes('/Parametri/CodUA') as ValoreParametro(CodUA))
					   	 				 
	SET @sCodRuolo=(SELECT	TOP 1 ValoreParametro.CodRuolo.value('.','VARCHAR(20)')
					FROM @xParametri.nodes('/Parametri/CodRuolo') as ValoreParametro(CodRuolo)) 
	IF ISNULL(@sCodRuolo,'')='' SET @sCodRuolo='###'					
	
	SET @sCodMaschera=(SELECT	TOP 1 ValoreParametro.CodMaschera.value('.','VARCHAR(20)')
					 FROM @xParametri.nodes('/Parametri/CodMaschera') as ValoreParametro(CodMaschera))				  
	IF ISNULL(@sCodMaschera,'')='' SET @sCodMaschera='###'
	
	SET @bDatiEstesi=(SELECT TOP 1 ValoreParametro.DatiEstesi.value('.','bit')
					  FROM @xParametri.nodes('/Parametri/DatiEstesi') as ValoreParametro(DatiEstesi))
	
	
				
	DECLARE @sCodEntita VARCHAR(20)
	DECLARE @sCodAzione AS VARCHAR(20)
	
	DECLARE @xTmp AS XML	 		
	
	DECLARE @bAbilitaPermessiDettaglio AS bit
	DECLARE @bAbilitaPermessiDettaglioAzione AS bit
	
		SET @sCodEntita='RPT'
		SET @sCodAzione='VIS'
	
	SET @bAbilitaPermessiDettaglio=(SELECT ISNULL(AbilitaPermessiDettaglio,0) FROM T_Entita WHERE Codice=@sCodEntita)
	SET @bAbilitaPermessiDettaglioAzione=(SELECT ISNULL(AbilitaPermessiDettaglio,0) FROM T_AzioniEntita WHERE CodEntita=@sCodEntita AND CodAzione=@sCodAzione)
	
	
	DECLARE @sSQL AS VARCHAR(MAX)
	
				
	IF ISNULL(@sCodice,'')= ''
	BEGIN
		CREATE TABLE #tmpUA
		(
			CodUA VARCHAR(20) COLLATE Latin1_General_CI_AS,
			Descrizione VARCHAR(255) COLLATE Latin1_General_CI_AS
		)
				
		IF  ISNULL(@sCodUA,'') <> ''
			BEGIN		
			
				SET  @xTmp=CONVERT(XML,'<Parametri><CodUA>'+ @sCodUA + '</CodUA></Parametri>')	
				INSERT #tmpUA EXEC MSP_SelUAPadri @xTmp				
			END	
		ELSE
			BEGIN			
												
				CREATE TABLE #tmpUALista
				(
					CodUA VARCHAR(20) COLLATE Latin1_General_CI_AS,		
					Descrizione VARCHAR(255) COLLATE Latin1_General_CI_AS
				)
									
				SET  @xTmp=CONVERT(XML,'<Parametri><CodRuolo>'+ @sCodRuolo + '</CodRuolo></Parametri>')
				INSERT #tmpUALista EXEC MSP_SelUADaRuolo @xTmp							
										
								
				CREATE TABLE #tmpUAPadri
				(
					CodUA VARCHAR(20) COLLATE Latin1_General_CI_AS,		
					Descrizione VARCHAR(255) COLLATE Latin1_General_CI_AS
				)
				
				DECLARE @sTmpUA AS VARCHAR(20)
				
				DECLARE curUA CURSOR FOR 
				SELECT CodUA
				FROM #tmpUALista
				
				OPEN curUA

				FETCH NEXT FROM curUA 
				INTO @sTmpUA
				
				WHILE @@FETCH_STATUS = 0
				BEGIN		
					SET  @xTmp=CONVERT(XML,'<Parametri><CodUA>'+ @sTmpUA + '</CodUA></Parametri>')
					TRUNCATE TABLE #tmpUAPadri
					INSERT #tmpUAPadri EXEC MSP_SelUAPadri @xTmp
					
					INSERT INTO #tmpUA(CodUA,Descrizione)
					SELECT CodUA, Descrizione
					FROM #tmpUAPadri
					WHERE CodUA NOT IN (SELECT CodUA FROM #tmpUA)
					
					FETCH NEXT FROM curUA 
					INTO @sTmpUA
				END		
									
				DROP TABLE #tmpUAPadri
				DROP TABLE #tmpUALista
			END
			
		
		CREATE INDEX IX_CodUA ON #tmpUA (CodUA)    	 			
			 			
		SET @sSQL='    SELECT 
							R.Codice, 
							R.Descrizione,			
							R.DaStoricizzare,
							R.[Path],
							R.Parametri,
							R.CodFormatoReport,
							FR.Descrizione AS DescFormato,
							R.CodReportVista,
							RV.Descrizione AS DescReportVista
							,R.NomePlugin
							,R.ApriBrowser
							,R.ApriIE
							,R.FlagRichiediStampante
				'

		IF (@bDatiEstesi=1)
			SET @sSQL=@sSQL + ' , R.Modello '
		ELSE			
			SET @sSQL=@sSQL + ' , NULL AS Modello '
			
		SET @sSQL=@sSQL + '
							FROM T_AssReportMaschere A WITH (NOLOCK)											INNER JOIN T_Report  R WITH (NOLOCK)
								ON A.CodReport = R.Codice	
							LEFT JOIN T_FormatoReport FR WITH (NOLOCK)
								ON R.CodFormatoReport=FR.Codice
							LEFT JOIN T_ReportViste RV WITH (NOLOCK)
								ON R.CodReportVista=RV.Codice 
							INNER JOIN																(SELECT DISTINCT 
									A.CodVoce AS Codice												
								FROM					
									T_AssUAEntita A WITH (NOLOCK)
										INNER JOIN #tmpUA T ON
											A.CodUA=T.CodUA					
								WHERE CodEntita=''' + @sCodEntita + '''
								) AS W	
							ON R.Codice= W.Codice'
							
				IF @bAbilitaPermessiDettaglio=1 AND @bAbilitaPermessiDettaglioAzione=1 
			SET @sSQL=@sSQL + ' INNER JOIN T_AssRuoliAzioni Z WITH (NOLOCK)
										ON (Z.CodEntita=''' + @sCodEntita + ''' AND						
											Z.CodVoce=R.Codice AND
											Z.CodRuolo=''' + @sCodRuolo + ''' AND
											Z.CodAzione = ''' + @sCodAzione + ''')'

												
		
		IF @sCodMaschera <>'###'
		BEGIN				
			SET @sSQL=@sSQL + ' WHERE A.CodMaschera=''' + @sCodMaschera  + ''''
		END
		SET @sSQL=@sSQL + ' ORDER BY  R.[Path] ASC, R.Descrizione ASC '

		
				EXEC (@sSQL)
		
		DROP TABLE #tmpUA
	END
	ELSE
		BEGIN
						SELECT	
				R.Codice, 
				R.Descrizione,			
				R.DaStoricizzare,
				R.[Path],
				R.Parametri,
				R.CodFormatoReport,
				FR.Descrizione AS DescFormato,
				R.CodReportVista,
				RV.Descrizione AS DescReportVista
				,R.NomePlugin
				,R.Modello
				,R.ApriBrowser
				,R.ApriIE
				,R.FlagRichiediStampante
			FROM T_Report  R
					LEFT JOIN T_FormatoReport FR
									ON R.CodFormatoReport=FR.Codice
								LEFT JOIN T_ReportViste RV
									ON R.CodReportVista=RV.Codice 
			WHERE R.Codice=@sCodice						
		END 					 
	RETURN 0
END