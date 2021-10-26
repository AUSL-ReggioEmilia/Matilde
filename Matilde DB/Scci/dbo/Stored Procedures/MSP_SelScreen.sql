CREATE PROCEDURE [dbo].[MSP_SelScreen](@xParametri AS XML)
AS
BEGIN
	
	
				 
	DECLARE @sCodUA AS Varchar(20)
	DECLARE @sCodRuolo AS Varchar(20)
	DECLARE @sCodTipoScreen  VARCHAR(20) 
	
	SET @sCodUA=(SELECT	TOP 1 ValoreParametro.CodUA.value('.','VARCHAR(20)')
					FROM @xParametri.nodes('/Parametri/CodUA') as ValoreParametro(CodUA))	
				   	 				 
	SET @sCodRuolo=(SELECT	TOP 1 ValoreParametro.CodRuolo.value('.','VARCHAR(20)')
					FROM @xParametri.nodes('/Parametri/CodRuolo') as ValoreParametro(CodRuolo)) 

		SET @sCodTipoScreen=(SELECT	TOP 1 ValoreParametro.CodTipoScreen.value('.','VARCHAR(20)')
					FROM @xParametri.nodes('/Parametri/CodTipoScreen') as ValoreParametro(CodTipoScreen)) 
						
							

				
	DECLARE @sCodEntita VARCHAR(20)
	DECLARE @bAbilitaPermessiDettaglio AS bit
	DECLARE @bAbilitaPermessiDettaglioAzione AS bit
	DECLARE @sCodAzione  VARCHAR(20) 
	

		SET @sCodEntita='SCR'
	SET @sCodAzione='VIS'
	
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
		

										
		SET @bAbilitaPermessiDettaglio=(SELECT ISNULL(AbilitaPermessiDettaglio,0) FROM T_Entita WHERE Codice=@sCodEntita)
		SET @bAbilitaPermessiDettaglioAzione=(SELECT ISNULL(AbilitaPermessiDettaglio,0) FROM T_AzioniEntita WHERE CodEntita=@sCodEntita AND CodAzione=@sCodAzione)
		
		IF @bAbilitaPermessiDettaglio=1 AND @bAbilitaPermessiDettaglioAzione=1
		BEGIN		
						
		SELECT 
				W.Codice,
				S.Descrizione
				,S.Attributi
				,S.Righe
				,S.Colonne		
				,S.CodTipoScreen
				,S.AltezzaRigaGrid		
				,S.LarghezzaColonnaGrid
				,S.CaricaPerRiga	
				,S.AdattaAltezzaRighe	
				FROM						
					(SELECT DISTINCT 
						Z.CodVoce AS Codice												
						FROM					
						 T_AssRuoliAzioni Z
						WHERE (Z.CodEntita=@sCodEntita AND
									Z.CodRuolo=@sCodRuolo AND
									Z.CodAzione =@sCodAzione)											
						) AS W				
						INNER JOIN T_Screen S								ON S.Codice=W.Codice
				WHERE
					S.CodTipoScreen =ISNULL(@sCodTipoScreen,S.CodTipoScreen)
				ORDER BY S.Descrizione							
	END
	ELSE
		BEGIN
						SELECT W.Codice,
					S.Descrizione		
					,S.Attributi
					,S.Righe
					,S.Colonne
					,S.CodTipoScreen
					,S.AltezzaRigaGrid	
					,S.LarghezzaColonnaGrid	
					,S.CaricaPerRiga	
					,S.AdattaAltezzaRighe
			FROM						
				(SELECT DISTINCT 
					A.CodVoce AS Codice												
				FROM					
					T_AssUAEntita A
						INNER JOIN #tmpUA T ON
							A.CodUA=T.CodUA					
				WHERE CodEntita=@sCodEntita			
				) AS W				
					INNER JOIN T_Screen S								ON S.Codice=W.Codice
			WHERE
			S.CodTipoScreen =ISNULL(@sCodTipoScreen,S.CodTipoScreen)								
			ORDER BY 	 S.Descrizione						  
		END		
	
	
		DROP TABLE #tmpUA			
	RETURN 0
END