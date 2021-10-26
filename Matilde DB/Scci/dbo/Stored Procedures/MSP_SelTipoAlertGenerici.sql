
CREATE PROCEDURE [dbo].[MSP_SelTipoAlertGenerici](@xParametri AS XML )
AS
BEGIN
	


					 	
	DECLARE @bDatiEstesi AS Bit		
	DECLARE @sCodAzione AS VARCHAR(20)	
	DECLARE @sCodRuolo AS Varchar(20)
	DECLARE @sCodUA AS Varchar(20)
	
	DECLARE @sCodEntita VARCHAR(20)
	DECLARE @bAbilitaPermessiDettaglio AS bit
	DECLARE @bAbilitaPermessiDettaglioAzione AS bit
	
		SET @sCodEntita='ALG'	
				
				
	SET @sCodRuolo=(SELECT	TOP 1 ValoreParametro.CodRuolo.value('.','VARCHAR(20)')
					FROM @xParametri.nodes('/Parametri/CodRuolo') as ValoreParametro(CodRuolo)) 
	IF ISNULL(@sCodRuolo,'')='' SET @sCodRuolo='###'	
	
	SET @sCodAzione=(SELECT	TOP 1 ValoreParametro.CodAzione.value('.','VARCHAR(20)')
					 FROM @xParametri.nodes('/Parametri/CodAzione') as ValoreParametro(CodAzione))				  
	IF ISNULL(@sCodAzione,'')='' SET @sCodAzione='###'
	
	SET @bDatiEstesi=(SELECT TOP 1 ValoreParametro.DatiEstesi.value('.','bit')
					  FROM @xParametri.nodes('/Parametri/DatiEstesi') as ValoreParametro(DatiEstesi))											
	
	
	SET @sCodUA=(SELECT	TOP 1 ValoreParametro.CodUA.value('.','VARCHAR(20)')
					FROM @xParametri.nodes('/Parametri/CodUA') as ValoreParametro(CodUA))
	IF ISNULL(@sCodUA,'')='' SET @sCodUA='###'	
	
					
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
				G.Codice,
				G.Descrizione,
				G.CodScheda,
				CASE 
					WHEN  ISNULL(@bDatiEstesi,1)=1 THEN G.Icona						 
					ELSE NULL
				END AS Icona	
			FROM 
				T_AssUAEntita A
					INNER JOIN #tmpUA T ON
						A.CodUA=T.CodUA
					INNER JOIN T_AssRuoliAzioni Z
						ON (Z.CodEntita=A.CodEntita AND						
							Z.CodVoce=A.CodVoce AND
							Z.CodRuolo=@sCodRuolo AND
							Z.CodAzione =@sCodAzione
							)
				INNER JOIN T_TipoAlertGenerico G
					ON G.Codice=A.CodVoce	
			WHERE A.CodEntita=@sCodEntita		
			ORDER BY G.Descrizione							
	END
	ELSE
		BEGIN		
				SELECT
					G.Codice,
					G.Descrizione,
					G.CodScheda,
					CASE 
						WHEN  ISNULL(@bDatiEstesi,1)=1 THEN G.Icona						 
						ELSE NULL
					END AS Icona
				FROM 					
					T_AssUAEntita A
						INNER JOIN #tmpUA T ON
							A.CodUA=T.CodUA	
						INNER JOIN T_TipoAlertGenerico	G	
							ON G.Codice=A.CodVoce	
				WHERE A.CodEntita=@sCodEntita			
				ORDER BY Descrizione
		END
	
	RETURN 0
END