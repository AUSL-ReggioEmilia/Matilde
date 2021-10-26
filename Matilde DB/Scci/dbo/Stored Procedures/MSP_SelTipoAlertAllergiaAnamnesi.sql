CREATE PROCEDURE [dbo].[MSP_SelTipoAlertAllergiaAnamnesi](@xParametri AS XML )
AS
BEGIN
	


	 	
	DECLARE @bDatiEstesi AS Bit		
	DECLARE @sCodAzione AS VARCHAR(20)	
	DECLARE @sCodRuolo AS Varchar(20)
	
	DECLARE @sCodEntita VARCHAR(20)
	DECLARE @bAbilitaPermessiDettaglio AS bit
	DECLARE @bAbilitaPermessiDettaglioAzione AS bit
	
		SET @sCodEntita='ALA'	
				
				
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
		SELECT * FROM 
			(SELECT
				Codice,
				Descrizione,
				CodScheda,
				CASE 
					WHEN  ISNULL(@bDatiEstesi,1)=1 THEN A.Icona						 
					ELSE NULL
				END AS Icona	
			FROM T_TipoAlertAllergiaAnamnesi A
					INNER JOIN T_AssRuoliAzioni Z
									ON (Z.CodEntita=@sCodEntita AND						
										Z.CodVoce=A.Codice AND
										Z.CodRuolo=@sCodRuolo AND
										Z.CodAzione =@sCodAzione)
			
			UNION 
			SELECT
				Codice,
				Descrizione,
				CodScheda,
				CASE 
					WHEN  ISNULL(@bDatiEstesi,1)=1 THEN A2.Icona						 
					ELSE NULL
				END AS Icona	
			FROM T_TipoAlertAllergiaAnamnesi A2
			WHERE A2.Codice NOT IN (SELECT CodVoce 
								    FROM T_AssRuoliAzioni
									WHERE 
										CodEntita=@sCodEntita AND																										
										CodAzione =@sCodAzione
									)
			) AS Q
			ORDER BY Descrizione ASC
	END		
	ELSE
		BEGIN
				SELECT
					Codice,
					Descrizione,
					CodScheda,
					CASE 
						WHEN  ISNULL(@bDatiEstesi,1)=1 THEN Icona						 
						ELSE NULL
					END AS Icona
				FROM T_TipoAlertAllergiaAnamnesi	
				ORDER BY Descrizione ASC
		END
	
	RETURN 0
END