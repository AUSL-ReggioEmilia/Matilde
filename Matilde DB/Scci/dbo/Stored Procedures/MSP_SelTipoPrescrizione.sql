CREATE PROCEDURE [dbo].[MSP_SelTipoPrescrizione](@xParametri AS XML )
AS
BEGIN
	
	
				 
	DECLARE @sCodUA AS Varchar(20)
	DECLARE @sCodRuolo AS Varchar(20)
	DECLARE @sCodAzione AS VARCHAR(20)
	DECLARE @sCodice AS Varchar(20)
	DECLARE @bDatiEstesi AS Bit
	DECLARE @sDescrizione AS VARCHAR(500)				

	SET @sCodUA=(SELECT	TOP 1 ValoreParametro.CodUA.value('.','VARCHAR(20)')
					FROM @xParametri.nodes('/Parametri/CodUA') as ValoreParametro(CodUA))
	IF ISNULL(@sCodUA,'')='' SET @sCodUA='###'	
				   	 				 
	SET @sCodRuolo=(SELECT	TOP 1 ValoreParametro.CodRuolo.value('.','VARCHAR(20)')
					FROM @xParametri.nodes('/Parametri/CodRuolo') as ValoreParametro(CodRuolo)) 
	IF ISNULL(@sCodRuolo,'')='' SET @sCodRuolo='###'					
	
	SET @sCodAzione=(SELECT	TOP 1 ValoreParametro.CodAzione.value('.','VARCHAR(20)')
					 FROM @xParametri.nodes('/Parametri/CodAzione') as ValoreParametro(CodAzione))				  
	IF ISNULL(@sCodAzione,'')='' SET @sCodAzione='###'	
	
	SET @sCodice=(SELECT	TOP 1 ValoreParametro.Codice.value('.','VARCHAR(20)')
					FROM @xParametri.nodes('/Parametri/Codice') as ValoreParametro(Codice))
	
											
	SET @bDatiEstesi=(SELECT TOP 1 ValoreParametro.DatiEstesi.value('.','bit')
					  FROM @xParametri.nodes('/Parametri/DatiEstesi') as ValoreParametro(DatiEstesi))
	
		SET @sDescrizione=(SELECT	TOP 1 ValoreParametro.Descrizione.value('.','VARCHAR(500)')
						 FROM @xParametri.nodes('/Parametri/Descrizione') as ValoreParametro(Descrizione))						 
	SET @sDescrizione= ISNULL(@sDescrizione,'')
	SET @sDescrizione=LTRIM(RTRIM(@sDescrizione))	
			
				
	DECLARE @sCodEntita VARCHAR(20)
	DECLARE @bAbilitaPermessiDettaglio AS bit
	DECLARE @bAbilitaPermessiDettaglioAzione AS bit
	
		SET @sCodEntita='PRF'
	
				CREATE TABLE #tmpUA
	(
		CodUA VARCHAR(20) COLLATE Latin1_General_CI_AS,
		Descrizione VARCHAR(255) COLLATE Latin1_General_CI_AS
	)

	DECLARE @xTmp AS XML
	
	SET @xTmp =CONVERT(XML,'<Parametri><CodUA>'+@sCodUA+'</CodUA></Parametri>')
	
	INSERT #tmpUA EXEC MSP_SelUAPadri @xTmp	
	
	
	CREATE INDEX IX_CodUA ON #tmpUA (CodUA)    		
		
			
	
	SET @bAbilitaPermessiDettaglio=(SELECT ISNULL(AbilitaPermessiDettaglio,0) FROM T_Entita WHERE Codice=@sCodEntita)
	SET @bAbilitaPermessiDettaglioAzione=(SELECT ISNULL(AbilitaPermessiDettaglio,0) FROM T_AzioniEntita WHERE CodEntita=@sCodEntita AND CodAzione=@sCodAzione)
	
	IF @bAbilitaPermessiDettaglio=1 AND @bAbilitaPermessiDettaglioAzione=1
		BEGIN		
						
			SELECT 
					W.Codice,
					D.Descrizione, 
					CASE 
						WHEN ISNULL(@bDatiEstesi,0)=0 THEN NULL
							ELSE
								D.Icona
						END
					AS Icona,
					D.CodScheda,
					D.CodViaSomministrazione,
					VS.Descrizione AS DescrViaSomministrazione,
					ISNULL(D.PrescrizioneASchema,0) AS PrescrizioneASchema,
					ISNULL(D.NonProseguibile,0) AS NonProseguibile,
					D.Path,
					D.CodSchedaPosologia
					FROM						
						(SELECT DISTINCT 
							A.CodVoce AS Codice												
						 FROM					
							T_AssUAEntita A
								INNER JOIN #tmpUA T ON
									A.CodUA=T.CodUA	
								INNER JOIN T_AssRuoliAzioni Z
									ON (Z.CodEntita=A.CodEntita AND						
										Z.CodVoce=A.CodVoce AND
										Z.CodRuolo=@sCodRuolo AND
										Z.CodAzione =@sCodAzione)					
						 WHERE A.CodEntita=@sCodEntita			
						 ) AS W				
							INNER JOIN T_TipoPrescrizione D									ON D.Codice=W.Codice	
							LEFT JOIN T_ViaSomministrazione VS
								ON D.CodViaSomministrazione=VS.Codice	
					WHERE 
						D.Codice=ISNULL(@sCodice,D.Codice)			
						AND D.Descrizione LIKE '%' + @sDescrizione + '%'
					ORDER BY D.Descrizione						
		END
	ELSE
		BEGIN
						SELECT W.Codice,
				   D.Descrizione, 				   
				   CASE 
						WHEN ISNULL(@bDatiEstesi,0)=0 THEN NULL
							ELSE
								D.Icona
						END
					AS Icona,
					D.CodScheda,
					D.CodViaSomministrazione,
					VS.Descrizione AS DescrViaSomministrazione,
					ISNULL(D.PrescrizioneASchema,0) AS PrescrizioneASchema,
					ISNULL(D.NonProseguibile,0) AS NonProseguibile,
					D.Path,
					D.CodSchedaPosologia		
			FROM						
				(SELECT DISTINCT 
					A.CodVoce AS Codice												
				FROM					
					T_AssUAEntita A
						INNER JOIN #tmpUA T ON
							A.CodUA=T.CodUA					
				WHERE CodEntita=@sCodEntita			
				) AS W				
					INNER JOIN T_TipoPrescrizione D								ON D.Codice=W.Codice	
					LEFT JOIN T_ViaSomministrazione VS
								ON D.CodViaSomministrazione=VS.Codice						  
			WHERE 
				D.Codice=ISNULL(@sCodice,D.Codice)	
				AND D.Descrizione LIKE '%' + @sDescrizione + '%'
			ORDER BY D.Descrizione			
		END		
		
	
		DROP TABLE #tmpUA			
	RETURN 0
END