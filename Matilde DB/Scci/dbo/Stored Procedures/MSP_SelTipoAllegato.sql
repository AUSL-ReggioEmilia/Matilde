

CREATE PROCEDURE [dbo].[MSP_SelTipoAllegato](@xParametri AS XML )
AS
BEGIN
	


	 	
	DECLARE @bDatiEstesi AS Bit		
	DECLARE @sCodAzione AS VARCHAR(20)	
	DECLARE @sCodRuolo AS Varchar(20)
	
	DECLARE @sCodEntita VARCHAR(20)
	DECLARE @bAbilitaPermessiDettaglio AS bit
	DECLARE @bAbilitaPermessiDettaglioAzione AS bit
	
		SET @sCodEntita='ALL'	
				
				
	SET @sCodRuolo=(SELECT	TOP 1 ValoreParametro.CodRuolo.value('.','VARCHAR(20)')
					FROM @xParametri.nodes('/Parametri/CodRuolo') as ValoreParametro(CodRuolo)) 
	IF ISNULL(@sCodRuolo,'')='' SET @sCodRuolo='###'	
	
	SET @sCodAzione=(SELECT	TOP 1 ValoreParametro.CodAzione.value('.','VARCHAR(20)')
					 FROM @xParametri.nodes('/Parametri/CodAzione') as ValoreParametro(CodAzione))				  
	IF ISNULL(@sCodAzione,'')='' SET @sCodAzione='###'
	
	SET @bDatiEstesi=(SELECT TOP 1 ValoreParametro.DatiEstesi.value('.','bit')
					  FROM @xParametri.nodes('/Parametri/DatiEstesi') as ValoreParametro(DatiEstesi))											
	
	
				
	
	SET @bAbilitaPermessiDettaglio=(SELECT ISNULL(AbilitaPermessiDettaglio,0) FROM T_Entita WHERE Codice=@sCodEntita)
	SET @bAbilitaPermessiDettaglioAzione=(SELECT ISNULL(AbilitaPermessiDettaglio,0) FROM T_AzioniEntita WHERE CodEntita=@sCodEntita AND CodAzione=@sCodAzione)
	
	IF @bAbilitaPermessiDettaglio=1 AND @bAbilitaPermessiDettaglioAzione=1
	BEGIN		
			SELECT
				Codice,
				Descrizione,
				Colore,
				CASE 
					WHEN  ISNULL(@bDatiEstesi,1)=1 THEN A.Icona						 
					ELSE NULL
				END AS Icona	
			FROM T_TipoAllegato A
					INNER JOIN T_AssRuoliAzioni Z
									ON (Z.CodEntita=@sCodEntita AND						
										Z.CodVoce=A.Codice AND
										Z.CodRuolo=@sCodRuolo AND
										Z.CodAzione =@sCodAzione)
			ORDER BY Descrizione
	END
	ELSE
		BEGIN
				SELECT
					Codice,
					Descrizione,
					Colore,
					CASE 
						WHEN  ISNULL(@bDatiEstesi,1)=1 THEN Icona						 
						ELSE NULL
					END AS Icona
				FROM T_TipoAllegato	
				ORDER BY Descrizione
		END
	
	RETURN 0
END