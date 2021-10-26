CREATE PROCEDURE [dbo].[MSP_SelTipoTaskInfermieristico](@xParametri AS XML)
AS
BEGIN
	
	
				 
	DECLARE @sCodUA AS Varchar(20)
	DECLARE @sCodRuolo AS Varchar(20)
	DECLARE @sCodAzione AS VARCHAR(20)
	DECLARE @bDatiEstesi AS Bit
	DECLARE @bErogazioneDiretta AS Bit
	DECLARE @bSoloFiltroTipoTaskInfermieristico AS Bit
	DECLARE @bIgnoraFiltroUA AS Bit	
	DECLARE @sCodTipoTaskInfermieristico AS VARCHAR(20)			

	SET @sCodUA=(SELECT	TOP 1 ValoreParametro.CodUA.value('.','VARCHAR(20)')
					FROM @xParametri.nodes('/Parametri/CodUA') as ValoreParametro(CodUA))
	IF ISNULL(@sCodUA,'')='' SET @sCodUA=''	
				   	 				 
	SET @sCodRuolo=(SELECT	TOP 1 ValoreParametro.CodRuolo.value('.','VARCHAR(20)')
					FROM @xParametri.nodes('/Parametri/CodRuolo') as ValoreParametro(CodRuolo)) 
	IF ISNULL(@sCodRuolo,'')='' SET @sCodRuolo='###'					
	
	SET @sCodAzione=(SELECT	TOP 1 ValoreParametro.CodAzione.value('.','VARCHAR(20)')
					 FROM @xParametri.nodes('/Parametri/CodAzione') as ValoreParametro(CodAzione))				  
	IF ISNULL(@sCodAzione,'')='' SET @sCodAzione='###'	

		SET @bDatiEstesi=(SELECT TOP 1 ValoreParametro.DatiEstesi.value('.','bit')
					  FROM @xParametri.nodes('/Parametri/DatiEstesi') as ValoreParametro(DatiEstesi))
	
		SET @bErogazioneDiretta=(SELECT TOP 1 ValoreParametro.ErogazioneDiretta.value('.','bit')
					  FROM @xParametri.nodes('/Parametri/ErogazioneDiretta') as ValoreParametro(ErogazioneDiretta))

	SET @bErogazioneDiretta=ISNULL(@bErogazioneDiretta,0)	

		SET @sCodTipoTaskInfermieristico=(SELECT	TOP 1 ValoreParametro.CodTipoTaskInfermieristico.value('.','VARCHAR(20)')
					FROM @xParametri.nodes('/Parametri/CodTipoTaskInfermieristico') as ValoreParametro(CodTipoTaskInfermieristico)) 
			
		SET @bSoloFiltroTipoTaskInfermieristico=(SELECT TOP 1 ValoreParametro.SoloFiltroTipoTaskInfermieristico.value('.','bit')
					  FROM @xParametri.nodes('/Parametri/SoloFiltroTipoTaskInfermieristico') as ValoreParametro(SoloFiltroTipoTaskInfermieristico))
	
	SET @bSoloFiltroTipoTaskInfermieristico=ISNULL(@bSoloFiltroTipoTaskInfermieristico,0)			

		SET @bIgnoraFiltroUA=(SELECT TOP 1 ValoreParametro.IgnoraFiltroUA.value('.','bit')
					  FROM @xParametri.nodes('/Parametri/IgnoraFiltroUA') as ValoreParametro(IgnoraFiltroUA))
	
	SET @bIgnoraFiltroUA=ISNULL(@bIgnoraFiltroUA,0)		

				
	DECLARE @sCodEntita VARCHAR(20)
	DECLARE @bAbilitaPermessiDettaglio AS bit
	DECLARE @bAbilitaPermessiDettaglioAzione AS bit
	
		SET @sCodEntita='WKI'	
	
				CREATE TABLE #tmpUA
	(
		CodUA VARCHAR(20) COLLATE Latin1_General_CI_AS ,
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
				SET  @xTmp=CONVERT(XML,'<Parametri><CodRuolo>'+ @sCodRuolo + '</CodRuolo></Parametri>')
		INSERT #tmpUA EXEC MSP_SelUADaRuolo @xTmp					
	END
	
	CREATE INDEX IX_CodUA ON #tmpUA (CodUA)    				
		
	IF @bSoloFiltroTipoTaskInfermieristico=0
	BEGIN

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
						END AS Icona,
					D.Colore,
					D.CodScheda,
					D.Ripianificazione
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
							INNER JOIN T_TipoTaskInfermieristico D									ON D.Codice=W.Codice
					WHERE
						1 = 
								CASE 
									WHEN @bErogazioneDiretta=1 AND ISNULL(D.ErogazioneDiretta,0)=1 THEN 1
									WHEN @bErogazioneDiretta=1 AND ISNULL(D.ErogazioneDiretta,0)=0 THEN 0
									ELSE 1
								END
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
							END AS Icona,
						D.Colore,
						D.CodScheda,
						D.Ripianificazione
				FROM						
					(SELECT DISTINCT 
						A.CodVoce AS Codice												
					FROM					
						T_AssUAEntita A
							INNER JOIN #tmpUA T ON
								A.CodUA=T.CodUA					
					WHERE CodEntita=@sCodEntita			
					) AS W				
						INNER JOIN T_TipoTaskInfermieristico D									ON D.Codice=W.Codice	
				WHERE
					1 = 
						CASE 
							WHEN @bErogazioneDiretta=1 AND ISNULL(D.ErogazioneDiretta,0)=1 THEN 1
							WHEN @bErogazioneDiretta=1 AND ISNULL(D.ErogazioneDiretta,0)=0 THEN 0
							ELSE 1
						END
				ORDER BY 	 D.Descrizione						  
			END		
	END
	ELSE
	BEGIN
		IF ISNULL(@bIgnoraFiltroUA,0)=0
		BEGIN
						SELECT WKI.Codice,WKI.Descrizione, WKI.CodScheda, WKI.Ripianificazione
			FROM T_TipoTaskInfermieristico WKI
			WHERE 
				Codice IN 						
											(SELECT DISTINCT 
								A.CodVoce AS Codice												
							 FROM					
								T_AssUAEntita A
									INNER JOIN #tmpUA T ON
										A.CodUA=T.CodUA					
							 WHERE CodEntita=@sCodEntita			
							) 	AND					
				WKI.Codice=@sCodTipoTaskInfermieristico
				AND
					1 = 
						CASE 
							WHEN @bErogazioneDiretta=1 AND ISNULL(WKI.ErogazioneDiretta,0)=1 THEN 1
							WHEN @bErogazioneDiretta=1 AND ISNULL(WKI.ErogazioneDiretta,0)=0 THEN 0
							ELSE 1
						END
		END
		ELSE
		BEGIN
			SELECT WKI.Codice,WKI.Descrizione, WKI.CodScheda, WKI.Ripianificazione
			FROM T_TipoTaskInfermieristico WKI
			WHERE 							
				WKI.Codice=@sCodTipoTaskInfermieristico
			AND
					1 = 
						CASE 
							WHEN @bErogazioneDiretta=1 AND ISNULL(WKI.ErogazioneDiretta,0)=1 THEN 1
							WHEN @bErogazioneDiretta=1 AND ISNULL(WKI.ErogazioneDiretta,0)=0 THEN 0
							ELSE 1
						END
		END
	END
		DROP TABLE #tmpUA			
	RETURN 0
END