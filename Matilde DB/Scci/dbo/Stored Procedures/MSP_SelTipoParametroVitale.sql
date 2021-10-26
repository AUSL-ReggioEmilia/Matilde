
CREATE PROCEDURE [dbo].[MSP_SelTipoParametroVitale](@xParametri AS XML )
AS
BEGIN
	
	
				 
	DECLARE @sCodUA AS Varchar(20)
	DECLARE @sListaCodUA AS Varchar(MAX)
	DECLARE @sCodRuolo AS Varchar(20)
	DECLARE @sCodAzione AS VARCHAR(20)
	DECLARE @bDatiEstesi AS Bit
				
		DECLARE @sSQL AS VARCHAR(MAX)
	DECLARE @nQta AS Bit
	
	CREATE TABLE #tmpListaUA
	(
		CodUA VARCHAR(20) COLLATE Latin1_General_CI_AS
	)
  	
  	CREATE TABLE #tmpUAFiltro
	(
		CodUA VARCHAR(20) COLLATE Latin1_General_CI_AS
	)
  	
  	
 				
		SET @sListaCodUA=''
	SELECT	@sListaCodUA =  @sListaCodUA +
														CASE 
								WHEN @sListaCodUA='' THEN ''
								ELSE  ','
							END + ''''
						  + ValoreParametro.CodUA.value('.','VARCHAR(MAX)')
						  + ''''
						 FROM @xParametri.nodes('/Parametri/CodUA') as ValoreParametro(CodUA)

	SET @sSQL='INSERT INTO #tmpListaUA (CodUA) 
			   SELECT Codice	FROM
					T_UnitaAtomiche
			   WHERE Codice IN (' + @sListaCodUA  +')'

	PRINT  @sSQL
	EXEC (@sSQL)
		

		SET @sCodRuolo=(SELECT	TOP 1 ValoreParametro.CodRuolo.value('.','VARCHAR(20)')
					FROM @xParametri.nodes('/Parametri/CodRuolo') as ValoreParametro(CodRuolo)) 	
	
		SET @sCodAzione=(SELECT	TOP 1 ValoreParametro.CodAzione.value('.','VARCHAR(20)')
					 FROM @xParametri.nodes('/Parametri/CodAzione') as ValoreParametro(CodAzione))				  
	IF ISNULL(@sCodAzione,'')='' SET @sCodAzione='###'	
	
		SET @bDatiEstesi=(SELECT TOP 1 ValoreParametro.DatiEstesi.value('.','bit')
					  FROM @xParametri.nodes('/Parametri/DatiEstesi') as ValoreParametro(DatiEstesi))
					  		
				DECLARE @sCodEntita VARCHAR(20)
	DECLARE @bAbilitaPermessiDettaglio AS bit
	DECLARE @bAbilitaPermessiDettaglioAzione AS bit
	
		SET @sCodEntita='PVT'
	
				
	DECLARE @xTmp AS XML		
	
	CREATE TABLE #tmpUA
	(
		CodUA VARCHAR(20) COLLATE Latin1_General_CI_AS,
		Descrizione VARCHAR(255) COLLATE Latin1_General_CI_AS
	)
	
	CREATE INDEX IX_CodUA ON #tmpUA (CodUA)    		
		
		DECLARE cur CURSOR READ_ONLY FOR 
		SELECT CodUA
		FROM #tmpListaUA
		
	OPEN cur

	FETCH NEXT FROM cur 
		INTO @sCodUA		
		
	WHILE @@FETCH_STATUS = 0
	BEGIN
		 
		SET @xTmp =CONVERT(XML,'<Parametri><CodUA>'+@sCodUA+'</CodUA></Parametri>')
	
		INSERT #tmpUA EXEC MSP_SelUAPadri @xTmp	 	
		
		INSERT INTO #tmpUAFiltro(CodUA)
		SELECT 
			CodUA
		FROM #tmpUA
		WHERE CodUA 
			NOT IN (SELECT CodUA 
					FROM 
					#tmpUAFiltro)
	
		DELETE FROM #tmpUA
						
		FETCH NEXT FROM cur 
		INTO @sCodUA		
	END			        				
	
	CLOSE cur
	DEALLOCATE cur
	
		
		
		SET @bAbilitaPermessiDettaglio=(SELECT ISNULL(AbilitaPermessiDettaglio,0) FROM T_Entita WHERE Codice=@sCodEntita)
	SET @bAbilitaPermessiDettaglioAzione=(SELECT ISNULL(AbilitaPermessiDettaglio,0) FROM T_AzioniEntita WHERE CodEntita=@sCodEntita AND CodAzione=@sCodAzione)
	
	IF @bAbilitaPermessiDettaglio=1 AND @bAbilitaPermessiDettaglioAzione=1 AND ISNULL(@sCodRuolo,'') <> '' 
	BEGIN		
				
		SELECT 
				W.Codice,
				D.Descrizione, 
				CASE 
					WHEN ISNULL(@bDatiEstesi,0)=0 THEN NULL
						ELSE
							D.Icona
					END AS Icona,
				D.Colore,
				CASE 
					WHEN ISNULL(@bDatiEstesi,0)=0 THEN NULL
						ELSE
							D.CampiFUT
					END AS CampiFUT,
				CASE 
					WHEN ISNULL(@bDatiEstesi,0)=0 THEN NULL
						ELSE
							D.CampiGrafici
					END AS CampiGrafici,
				D.CodScheda	
				FROM						
					(SELECT DISTINCT 
						A.CodVoce AS Codice												
					 FROM					
						T_AssUAEntita A
							INNER JOIN #tmpUAFiltro T ON
								A.CodUA=T.CodUA	
							INNER JOIN T_AssRuoliAzioni Z
								ON (Z.CodEntita=A.CodEntita AND						
									Z.CodVoce=A.CodVoce AND
									Z.CodRuolo=@sCodRuolo AND
									Z.CodAzione =@sCodAzione)					
					 WHERE A.CodEntita=@sCodEntita			
					 ) AS W				
						INNER JOIN T_TipoParametroVitale D								ON D.Codice=W.Codice	
				ORDER BY ISNULL(D.Ordine,0),D.Descrizione
	END
	ELSE
		BEGIN
						SELECT W.Codice,
				   D.Descrizione, 
				CASE 
						WHEN ISNULL(@bDatiEstesi,0)=0 THEN NULL
							ELSE
								D.Icona
						END AS Icona,
					D.Colore,
					CASE 
						WHEN ISNULL(@bDatiEstesi,0)=0 THEN NULL
							ELSE
								D.CampiFUT
						END AS CampiFUT,
					CASE 
						WHEN ISNULL(@bDatiEstesi,0)=0 THEN NULL
							ELSE
								D.CampiGrafici
						END AS CampiGrafici,
					D.CodScheda	
			FROM						
				(SELECT DISTINCT 
					A.CodVoce AS Codice												
				FROM					
					T_AssUAEntita A
						INNER JOIN #tmpUAFiltro T ON
							A.CodUA=T.CodUA					
				WHERE CodEntita=@sCodEntita			
				) AS W				
					INNER JOIN T_TipoParametroVitale D								ON D.Codice=W.Codice					  
				ORDER BY ISNULL(D.Ordine,0),D.Descrizione
		END		
	
		DROP TABLE #tmpUA			
	DROP TABLE #tmpUAFiltro	
	DROP TABLE #tmpListaUA
	RETURN 0
END