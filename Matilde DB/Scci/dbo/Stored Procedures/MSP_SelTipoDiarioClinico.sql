
CREATE PROCEDURE [dbo].[MSP_SelTipoDiarioClinico](@xParametri AS XML)
AS
BEGIN
	
	
	
				 
	DECLARE @sCodUA AS Varchar(20)
	DECLARE @sCodRuolo AS Varchar(20)
	DECLARE @sCodAzione AS VARCHAR(20)
	DECLARE @bDatiEstesi AS Bit
				

	SET @sCodUA=(SELECT	TOP 1 ValoreParametro.CodUA.value('.','VARCHAR(20)')
					FROM @xParametri.nodes('/Parametri/CodUA') as ValoreParametro(CodUA))
	IF ISNULL(@sCodUA,'')='' SET @sCodUA='###'	
				   	 				 
	SET @sCodRuolo=(SELECT	TOP 1 ValoreParametro.CodRuolo.value('.','VARCHAR(20)')
					FROM @xParametri.nodes('/Parametri/CodRuolo') as ValoreParametro(CodRuolo)) 
	IF ISNULL(@sCodRuolo,'')='' SET @sCodRuolo='###'					
	
	SET @sCodAzione=(SELECT	TOP 1 ValoreParametro.CodAzione.value('.','VARCHAR(20)')
					 FROM @xParametri.nodes('/Parametri/CodAzione') as ValoreParametro(CodAzione))				  
	IF ISNULL(@sCodAzione,'')='' SET @sCodAzione='###'	
											
	SET @bDatiEstesi=(SELECT TOP 1 ValoreParametro.DatiEstesi.value('.','bit')
					  FROM @xParametri.nodes('/Parametri/DatiEstesi') as ValoreParametro(DatiEstesi))
													
				
	DECLARE @sCodEntita VARCHAR(20)
	DECLARE @bAbilitaPermessiDettaglio AS bit
	DECLARE @bAbilitaPermessiDettaglioAzione AS bit

	
		SET @sCodEntita='DCL'
	
				CREATE TABLE #tmpUA
	(
		CodUA VARCHAR(20) COLLATE Latin1_General_CI_AS ,
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
								SELECT DISTINCT 
			A.CodVoce,
			D.Descrizione,
			D.CodTipoDiario,
			D.CodScheda,
			D.CopiaDaPrecedente
			
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
				INNER JOIN T_TipoVoceDiario D
					ON D.Codice=A.CodVoce			
		WHERE A.CodEntita=@sCodEntita
		ORDER BY D.Descrizione
	END
	ELSE
		BEGIN
						SELECT DISTINCT 
				A.CodVoce,
				D.Descrizione,
				D.CodTipoDiario,
				D.CodScheda,
				D.CopiaDaPrecedente
			FROM					
				T_AssUAEntita A
					INNER JOIN #tmpUA T ON
						A.CodUA=T.CodUA
					INNER JOIN T_TipoVoceDiario D
						ON D.Codice=A.CodVoce		
			WHERE CodEntita=@sCodEntita	
			ORDER BY D.Descrizione		  
		END		
	
		DROP TABLE #tmpUA			
	RETURN 0
END