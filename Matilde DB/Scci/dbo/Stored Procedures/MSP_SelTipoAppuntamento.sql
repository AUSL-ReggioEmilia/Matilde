CREATE PROCEDURE [dbo].[MSP_SelTipoAppuntamento](@xParametri AS XML )
AS
BEGIN
	

				 
	DECLARE @sCodUA AS Varchar(20)
	DECLARE @sCodRuolo AS Varchar(20)
	DECLARE @sCodAzione AS VARCHAR(20)
	DECLARE @sCodAgendaPartenza AS VARCHAR(20)
	DECLARE @bDatiEstesi AS Bit
				
		SET @sCodUA=(SELECT	TOP 1 ValoreParametro.CodUA.value('.','VARCHAR(20)')
						 FROM @xParametri.nodes('/Parametri/CodUA') as ValoreParametro(CodUA))
	SET @sCodUA= ISNULL(@sCodUA,'')
				
		SET @sCodRuolo=(SELECT	TOP 1 ValoreParametro.CodRuolo.value('.','VARCHAR(20)')
					FROM @xParametri.nodes('/Parametri/CodRuolo') as ValoreParametro(CodRuolo)) 
	IF ISNULL(@sCodRuolo,'')='' SET @sCodRuolo='###'					
	
		SET @sCodAzione=(SELECT	TOP 1 ValoreParametro.CodAzione.value('.','VARCHAR(20)')
					 FROM @xParametri.nodes('/Parametri/CodAzione') as ValoreParametro(CodAzione))				  
	IF ISNULL(@sCodAzione,'')='' SET @sCodAzione='###'	
			
		SET @sCodAgendaPartenza=(SELECT	TOP 1 ValoreParametro.CodAgendaPartenza.value('.','VARCHAR(20)')
					 FROM @xParametri.nodes('/Parametri/CodAgendaPartenza') as ValoreParametro(CodAgendaPartenza))				  

		SET @bDatiEstesi=(SELECT TOP 1 ValoreParametro.DatiEstesi.value('.','bit')
					  FROM @xParametri.nodes('/Parametri/DatiEstesi') as ValoreParametro(DatiEstesi))											
	
				
	DECLARE @sCodEntita VARCHAR(20)
	DECLARE @bAbilitaPermessiDettaglio AS bit
	DECLARE @bAbilitaPermessiDettaglioAzione AS bit		
	
				CREATE TABLE #tmpUA
	(
		CodUA VARCHAR(20) COLLATE Latin1_General_CI_AS,
		Descrizione VARCHAR(255) COLLATE Latin1_General_CI_AS
	)

	DECLARE @xTmp AS XML
	
	IF @sCodUA<> ''
	BEGIN
		SET @xTmp =CONVERT(XML,'<Parametri><CodUA>'+@sCodUA+'</CodUA></Parametri>')	
		INSERT #tmpUA EXEC MSP_SelUAPadri @xTmp	
	END
	ELSE
	BEGIN
		SET @xTmp =CONVERT(XML,'<Parametri><CodRuolo>'+@sCodRuolo+'</CodRuolo></Parametri>')	
		INSERT #tmpUA EXEC MSP_SelUADaRuolo @xTmp	
	END
	
	CREATE INDEX IX_CodUA ON #tmpUA (CodUA)    		

				
	CREATE TABLE #tmpTipoAppuntamento
	(
		Codice VARCHAR(20) COLLATE Latin1_General_CI_AS,
		NumAgende INTEGER,
		NumAgendeRuoli INTEGER,
		NumAgendeEsisteRuolo INTEGER
	)
	
	CREATE INDEX IX_TipoAppuntamento ON #tmpTipoAppuntamento (Codice)    		
	
		
		SET @sCodEntita='APP'
				
		INSERT INTO #tmpTipoAppuntamento(Codice,NumAgende,NumAgendeRuoli,NumAgendeEsisteRuolo)
	SELECT
			W.Codice AS CodTipoAppuntamento,		  
			0 AS NumAgende,
			0 AS NumAgendeRuoli,
			0 AS NumAgendeEsisteRuolo			
	FROM						
		(SELECT DISTINCT 
			A.CodVoce AS Codice
			FROM					
			T_AssUAEntita A
				INNER JOIN #tmpUA T ON
					A.CodUA=T.CodUA					
			WHERE CodEntita=@sCodEntita				
			) AS W				
			INNER JOIN T_TipoAppuntamento D						ON D.Codice=W.Codice		
		GROUP BY W.Codice
													  
		
		
		IF ISNULL(@sCodAgendaPartenza,'') <> '' 
		BEGIN
			DELETE FROM #tmpTipoAppuntamento
			WHERE Codice NOT IN (SELECT CodTipoApp FROM T_AssAgendeTipoAppuntamenti 
								 WHERE CodAgenda=@sCodAgendaPartenza)
		END
	
			
		UPDATE #tmpTipoAppuntamento
	SET
		NumAgende = Q.NumAgende
	FROM 
		#tmpTipoAppuntamento  TMP
			INNER JOIN 
				(SELECT ATA.CodTipoApp,
						COUNT(*) AS NumAgende
				 FROM 				  
					T_AssAgendeTipoAppuntamenti ATA									INNER JOIN 
														(SELECT CodVoce AS CodAgenda			
							 FROM					
								T_AssUAEntita A
									INNER JOIN #tmpUA T ON
									A.CodUA=T.CodUA					
							  WHERE CodEntita='AGE'
							  GROUP BY CodVoce
							) AS AGEUA
						ON ATA.CodAgenda=AGEUA.CodAgenda
				 GROUP BY ATA.CodTipoApp
				) AS Q	
	    ON TMP.Codice=Q.CodTipoApp
	
		UPDATE #tmpTipoAppuntamento
	SET
		NumAgendeRuoli = Q.NumAgendeRuoli,
		NumAgendeEsisteRuolo = Q.NumAgendeEsisteRuolo
	FROM 
		#tmpTipoAppuntamento  TMP
			INNER JOIN
				(SELECT 
					ATA.CodTipoApp, 
					COUNT(*) AS NumAgendeRuoli,
					SUM(AGERUO.NumAgendeEsisteRuolo) AS NumAgendeEsisteRuolo
				 FROM 
										T_AssAgendeTipoAppuntamenti ATA											
					INNER JOIN 
												(SELECT A.CodVoce AS CodAgenda			
						  FROM					
							T_AssUAEntita A
								INNER JOIN #tmpUA T ON
								A.CodUA=T.CodUA					
							WHERE CodEntita='AGE'
							GROUP BY CodVoce
						) AS AGEUA
					ON ATA.CodAgenda=AGEUA.CodAgenda
				
										INNER JOIN 
							(SELECT 
								CodVoce AS CodAgenda,
								SUM(CASE 
									WHEN CodRuolo = @sCodRuolo THEN 1
									ELSE 0
								END) AS NumAgendeEsisteRuolo
							FROM 		
								T_AssRuoliAzioni ASS		
							WHERE 
								ASS.CodEntita='AGE' AND 
								ASS.CodAzione='VIS'
							GROUP BY ASS.CodVoce
							) AS AGERUO				
					 ON AGEUA.CodAgenda=AGERUO.CodAgenda
				 GROUP BY ATA.CodTipoApp
				)  AS Q
		  ON TMP.Codice=Q.CodTipoApp

	
		DELETE FROM #tmpTipoAppuntamento
	WHERE 		
		NumAgende = NumAgendeRuoli AND
		NumAgendeEsisteRuolo = 0
	
  		SELECT 
		 W.Codice,
		 D.Descrizione,				   
		 CASE 
				WHEN ISNULL(@bDatiEstesi,0)=0 THEN NULL
						ELSE
							D.Icona
					END AS Icona,
		D.Colore,
		D.CodScheda,
		ISNULL(D.TimeSlotInterval,0) AS TimeSlotInterval,
		FormulaTitolo,
		ISNULL(Multiplo,0) AS Multiplo,
		ISNULL(SenzaData,0) AS SenzaData,
		ISNULL(SenzaDataSempre,0) AS SenzaDataSempre,
		ISNULL(Settimanale,0) AS Settimanale,
		ISNULL(Ripianificazione,0) AS Ripianificazione

		FROM						
			#tmpTipoAppuntamento AS W
				INNER JOIN T_TipoAppuntamento D							ON D.Codice=W.Codice
		ORDER BY D.Descrizione					

		DROP TABLE #tmpUA			
	DROP TABLE #tmpTipoAppuntamento	
	RETURN 0
END