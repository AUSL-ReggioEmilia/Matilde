CREATE PROCEDURE [dbo].[MSP_SelTipoScheda](@xParametri XML)
AS

BEGIN	

	DECLARE @sCodEntita AS VARCHAR(20)
	DECLARE @sCodEntitaScheda AS VARCHAR(20)
	DECLARE @uIDEntita AS UNIQUEIDENTIFIER
	DECLARE @uIDSchedaPadre AS UNIQUEIDENTIFIER
	DECLARE @uIDTrasferimento AS UNIQUEIDENTIFIER
	DECLARE @sCodRuolo AS VARCHAR(20)	
	DECLARE @bSoloNumerositaMinima AS BIT 
	DECLARE @sDescrizione AS VARCHAR(500)
	
	DECLARE @sCodSchedaPadre AS VARCHAR(20)
	DECLARE @sCodEntitaSchedaPadre AS VARCHAR(20)
	DECLARE @sGUID AS VARCHAR(50)
	DECLARE @uGUID AS UNIQUEIDENTIFIER
	DECLARE @sCodAzione AS VARCHAR(20)
	DECLARE @sCodUA AS VARCHAR(20)
	DECLARE @bAppuntamentoEpisodio AS BIT	
	
	DECLARE @bAbilitaPermessiDettaglio AS bit
	DECLARE @bAbilitaPermessiDettaglioAzione AS bit
	
	
	SET @sCodEntitaScheda='SCH'	
	SET @sCodAzione='INS'
	
	SET  @bSoloNumerositaMinima=0
	
	IF @xParametri.exist('(//CodEntita)')=1
	BEGIN
			SET @sCodEntita=(SELECT TOP 1 ValoreParametro.CodEntita.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodEntita') as ValoreParametro(CodEntita))	
					  
			SET @sCodEntita=ISNULL(@sCodEntita,'')
	END	
		
	IF @xParametri.exist('(//IDEntita)')=1
	BEGIN
			SET @sGUID=(SELECT TOP 1 ValoreParametro.IDEntita.value('.','VARCHAR(50)')
							  FROM @xParametri.nodes('/Parametri/IDEntita') as ValoreParametro(IDEntita))
			IF 	ISNULL(@sGUID,'') <> '' 
				SET @uIDEntita=CONVERT(UNIQUEIDENTIFIER,	@sGUID)		
			ELSE	
				SET @uIDEntita=NULL				  										  		
	END		
		
	IF @xParametri.exist('(/Parametri/TimeStamp/IDTrasferimento)')=1
	BEGIN
			SET @sGUID=(SELECT TOP 1 ValoreParametro.IDEntita.value('.','VARCHAR(50)')
							  FROM @xParametri.nodes('Parametri/TimeStamp/IDTrasferimento') as ValoreParametro(IDEntita))
			IF 	ISNULL(@sGUID,'') <> '' 
				SET @uIDTrasferimento	=CONVERT(UNIQUEIDENTIFIER,	@sGUID)		
			ELSE	
				SET @uIDTrasferimento	=NULL				  										  		
	END	
		
	SET @sGUID=(SELECT TOP 1 ValoreParametro.IDSchedaPadre.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDSchedaPadre') as ValoreParametro(IDSchedaPadre))
	IF 	ISNULL(@sGUID,'') <> '' 
		SET @uIDSchedaPadre=CONVERT(UNIQUEIDENTIFIER,	@sGUID)		
	ELSE	
		SET @uIDSchedaPadre=NULL
	
	SET @sCodRuolo=(SELECT TOP 1 ValoreParametro.CodRuolo.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/TimeStamp/CodRuolo') as ValoreParametro(CodRuolo))
	
	IF @xParametri.exist('(/Parametri/SoloNumerositaMinima)')=1
	BEGIN
	
			SET @bSoloNumerositaMinima=(SELECT TOP 1 ValoreParametro.SoloNumerositaMinima.value('.','bit')
					  FROM @xParametri.nodes('/Parametri/SoloNumerositaMinima') as ValoreParametro(SoloNumerositaMinima))											
			SET @bSoloNumerositaMinima=ISNULL(@bSoloNumerositaMinima,0)							  										  		
	END	
	
	
	SET @sDescrizione=(SELECT	TOP 1 ValoreParametro.Descrizione.value('.','VARCHAR(500)')
						 FROM @xParametri.nodes('/Parametri/Descrizione') as ValoreParametro(Descrizione))						 
	SET @sDescrizione= ISNULL(@sDescrizione,'')
	SET @sDescrizione=LTRIM(RTRIM(@sDescrizione))		
	
	SET @bAbilitaPermessiDettaglio=(SELECT ISNULL(AbilitaPermessiDettaglio,0) FROM T_Entita WHERE Codice=@sCodEntitaScheda)
	SET @bAbilitaPermessiDettaglioAzione=(SELECT ISNULL(AbilitaPermessiDettaglio,0) FROM T_AzioniEntita WHERE CodEntita=@sCodEntitaScheda AND CodAzione=@sCodAzione)
		
	SET @sCodUA=(SELECT TOP 1 CodUA FROM T_MovTrasferimenti WHERE ID=@uIDTrasferimento)
		
	IF @sCodEntita='APP'
		BEGIN
			 SET @bAppuntamentoEpisodio=(SELECT TOP 1
												CASE 
													WHEN IDEpisodio IS NOT NULL THEN 1
													ELSE 0
												END	
												FROM T_MovAppuntamenti WHERE ID=@uIDEntita)
			 SET @bAppuntamentoEpisodio=ISNULL(@bAppuntamentoEpisodio,0)
			 			
		END
	SET @sCodUA=(SELECT TOP 1 CodUA FROM T_MovTrasferimenti WHERE ID=@uIDTrasferimento)
	
	CREATE TABLE #tmpSchede
	(
		Codice VARCHAR(20) COLLATE Latin1_General_CI_AS
	)

	CREATE TABLE #tmpUARuolo
	(
		Codice VARCHAR(20) COLLATE Latin1_General_CI_AS,
		Descrizione VARCHAR(255) COLLATE Latin1_General_CI_AS
	)

	IF @bSoloNumerositaMinima=0 
	BEGIN
		DECLARE @xTmp AS XML
		SET  @xTmp=CONVERT(XML,'<Parametri><CodRuolo>'+ @sCodRuolo + '</CodRuolo></Parametri>')
	
		INSERT #tmpUARuolo EXEC MSP_SelUADaRuolo @xTmp
		
		CREATE INDEX IX_CodUA ON #tmpUARuolo (Codice)    
			
		CREATE TABLE #tmpSchedeRuolo
		(
			Codice VARCHAR(20) COLLATE Latin1_General_CI_AS
		)
			
		IF @bAbilitaPermessiDettaglio=1 AND @bAbilitaPermessiDettaglioAzione=1
			BEGIN	
				
				INSERT INTO #tmpSchedeRuolo(Codice)
				SELECT DISTINCT CodVoce FROM T_AssRuoliAzioni
				WHERE CodAzione=@sCodAzione AND
					  CodEntita=@sCodEntitaScheda AND
					  CodRuolo =@sCodRuolo						
			END
	END
	
	CREATE TABLE #tmpUAGerarchia
	(
		Codice VARCHAR(20) COLLATE Latin1_General_CI_AS,
		Descrizione VARCHAR(255) COLLATE Latin1_General_CI_AS
	)

	CREATE TABLE #tmpSchedeGerarchia
	(
		Codice VARCHAR(20) COLLATE Latin1_General_CI_AS,		
	)
							
	IF  (@sCodEntita='EPI' AND @uIDTrasferimento IS NOT NULL) 
		OR (@bAppuntamentoEpisodio=1)
		BEGIN	
						
			SET  @xTmp=CONVERT(XML,'<Parametri><CodUA>'+ @sCodUA + '</CodUA></Parametri>')		
			INSERT #tmpUAGerarchia EXEC MSP_SelUAPadri @xTmp	
			CREATE INDEX IX_CodUA ON #tmpUAGerarchia (Codice)    
						
			INSERT INTO #tmpSchedeGerarchia(Codice)
			SELECT DISTINCT CodVoce 
			FROM T_AssUAEntita AU
				INNER JOIN #tmpUAGerarchia GA
					ON AU.CodUA=GA.Codice
			WHERE 				
				  CodEntita=@sCodEntitaScheda 	
			
			IF @bSoloNumerositaMinima=0
			BEGIN			
				INSERT INTO #tmpSchede(Codice)
				SELECT 
					TR.Codice			
				 FROM #tmpSchedeRuolo TR
					INNER JOIN #tmpSchedeGerarchia TG
						ON TR.Codice=TG.Codice
			END			
			ELSE
			BEGIN							
				INSERT INTO #tmpSchede(Codice)
				SELECT 
					TG.Codice			
				FROM
					 #tmpSchedeGerarchia TG							 			
			END		
										
		END
	ELSE
		BEGIN
			IF @bSoloNumerositaMinima=0 			
			BEGIN							
				INSERT INTO #tmpSchede(Codice)
				SELECT
					TR.Codice
				FROM #tmpSchedeRuolo TR
			END
			ELSE
				BEGIN										
					IF @sCodEntita='PAZ' AND ISNULL(@sCodUA,'')=''	AND @bSoloNumerositaMinima=1
					BEGIN	
						INSERT INTO #tmpSchede(Codice)
							SELECT Codice
								FROM T_Schede
								WHERE 				
									  CodEntita='PAZ'				
									  OR CodEntita2='PAZ'			
					END
				END
		END
	
	DELETE FROM #tmpSchede
	WHERE Codice NOT IN (SELECT 
							CodScheda 
						 FROM
						 	T_SchedeVersioni SV	
								INNER JOIN #tmpSchede TS
									ON SV.CodScheda=TS.Codice
						WHERE	
							ISNULL(FlagAttiva, 0) = 1	AND ISNULL(Pubblicato,0)=1 			
							AND 
							DtValI <= GETDATE() AND											
							ISNULL(DtValF,convert(datetime,'01-01-2100')) >=  GETDATE()
						)		


	IF @bSoloNumerositaMinima=1
	BEGIN
		DELETE #tmpSchede  
		FROM #tmpSchede  T
			INNER JOIN T_Schede S
			ON T.Codice=S.Codice
		WHERE 
			ISNULL(S.NumerositaMinima,0) <1
	END			   
		
	IF @uIDSchedaPadre IS NOT NULL		
		SELECT TOP 1 @sCodSchedaPadre=CodScheda, @sCodEntitaSchedaPadre=CodEntita FROM T_MovSchede WHERE ID=@uIDSchedaPadre

	SET @sCodSchedaPadre=ISNULL(@sCodSchedaPadre,'')	
		
	IF @uIDSchedaPadre IS NULL
		BEGIN	
			 
			SELECT S.Codice,Descrizione,Path,
						   
				   ISNULL(S.NumerositaMassima,1) AS NumerositaMassima,					   
				   ISNULL (Q.QtaSchedeAttive,0) AS SchedeAttive,
				   ISNULL (Q.QtaSchedeDisponibili, ISNULL(S.NumerositaMassima,1)) AS SchedeDisponibili,				
				   ISNULL(S.NumerositaMinima,0) AS NumerositaMinimaConfigurata,
				   CASE 	
						WHEN (ISNULL(S.NumerositaMinima,0) -									
							   ISNULL (QtaSchedeAttive,0)) > 0									
									THEN ISNULL(S.NumerositaMinima,0)- ISNULL (QtaSchedeAttive,0)
						ELSE 0
				   END NumerositaMinimaDaCreare,
				   ISNULL(S.Contenitore,0) AS Contenitore,
				   ISNULL(S.CartellaAmbulatorialeCodificata,0) AS CartellaAmbulatorialeCodificata,
				   S.CodContatore AS CodContatore
			FROM 				
				T_Schede S 																	 													
					LEFT JOIN			
							Q_SelMovSchedeNumerosita AS Q
									ON (Q.CodScheda=S.Codice AND
										Q.CodEntita=@sCodEntita AND					
										Q.IDentita=@uIDEntita)						
							
						INNER JOIN #tmpSchede TS
							ON  S.Codice=TS.Codice
				WHERE 					
					S.SchedaSemplice=0														
									 
					AND (S.CodEntita=@sCodEntita OR	
						 S.CodEntita2=@sCodEntita	
						)
					AND		
															
					ISNULL(Q.QtaSchedeDisponibili,1) > 0					
					
					AND
					S.Descrizione LIKE '%' + @sDescrizione + '%'
					
				ORDER BY S.Ordine	ASC
		END
	ELSE
		BEGIN						
			SELECT	
				   S.Codice,S.Descrizione,Path,						   
				   ISNULL(S.NumerositaMassima,1) AS NumerositaMassima,
				   ISNULL (MSF.QtaSchedeFiglioAttive,0) AS SchedeAttive,
				   CASE 
						WHEN ISNULL(S.NumerositaMassima,1) -  ISNULL (MSF.QtaSchedeFiglioAttive,0) > 0 THEN 
							 ISNULL(S.NumerositaMassima,1) -  ISNULL (MSF.QtaSchedeFiglioAttive,0)
						ELSE 0
				   END	AS SchedeDisponibili,				
				   ISNULL(S.NumerositaMinima,0) AS NumerositaMinimaConfigurata,
				   CASE 	
						WHEN  (ISNULL(S.NumerositaMinima,0) -								
							   ISNULL (MSF.QtaSchedeFiglioAttive,0)) > 0						
									THEN ISNULL(S.NumerositaMinima,0)- ISNULL (MSF.QtaSchedeFiglioAttive,0)
						ELSE 0
				   END NumerositaMinimaDaCreare,
				   ISNULL(S.Contenitore,0) AS Contenitore,
				   ISNULL(S.CartellaAmbulatorialeCodificata,0) AS CartellaAmbulatorialeCodificata,				   
				   S.CodContatore AS CodContatore
			FROM 				
				 #tmpSchede TS
					INNER JOIN T_Schede S 
							ON  S.Codice=TS.Codice					
				
					LEFT JOIN 
						(
						 SELECT 
							CodScheda,COUNT(*) AS QtaSchedeFiglioAttive 
						FROM
							 T_MovSchede
						WHERE 
							CodStatoScheda <> 'CA' AND						
							Storicizzata=0 AND							
							CodEntita=@sCodEntitaSchedaPadre AND		
							IDSchedaPadre=@uIDSchedaPadre 										
						GROUP BY CodScheda
						) AS MSF
						ON MSF.CodScheda=TS.Codice 
				WHERE 					
					S.SchedaSemplice=0	AND
									
					S.Codice IN (SELECT 
										SP.CodScheda 
								 FROM T_SchedePadri AS SP
										
								 WHERE 												
									SP.CodSchedaPadre=@sCodSchedaPadre 											
													
								) AND															
					S.Descrizione LIKE '%' + @sDescrizione + '%'		
					
					AND 					
					ISNULL(S.NumerositaMassima,1) -  ISNULL (MSF.QtaSchedeFiglioAttive,0) > 0
				ORDER BY S.Ordine ASC, S.Descrizione ASC							
		END
END


