CREATE PROCEDURE [dbo].[MSP_SelRuoliCartelleInVisione](@xParametri AS XML)
AS
BEGIN
	

				
	DECLARE @sCodUA AS VARCHAR(20)
	DECLARE @sCodRuolo VARCHAR(20)
				
		DECLARE @sCodEntita VARCHAR(20)
	DECLARE @sCodAzione VARCHAR(20)
	DECLARE @bAbilitaPermessiDettaglio AS bit
	DECLARE @bAbilitaPermessiDettaglioAzione AS bit
	DECLARE @nQta AS INTEGER

		SET @sCodUA=(SELECT TOP 1 ValoreParametro.CodUA.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodUA') as ValoreParametro(CodUA))
	
			SET @sCodRuolo=(SELECT TOP 1 ValoreParametro.CodRuolo.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/TimeStamp/CodRuolo') as ValoreParametro(CodRuolo))
	SET @sCodRuolo=ISNULL(@sCodRuolo,'')
	
		SET @sCodEntita='CIV'

		SET @sCodAzione='INS'

		SET @bAbilitaPermessiDettaglio=(SELECT ISNULL(AbilitaPermessiDettaglio,0) FROM T_Entita WHERE Codice=@sCodEntita)
	SET @bAbilitaPermessiDettaglioAzione=(SELECT ISNULL(AbilitaPermessiDettaglio,0) FROM T_AzioniEntita WHERE CodEntita=@sCodEntita AND CodAzione=@sCodAzione)	
	
	IF (@bAbilitaPermessiDettaglio=1 AND @bAbilitaPermessiDettaglioAzione=1 AND ISNULL(@sCodRuolo,'') <> '')
	BEGIN
				
		SET @nQta =(SELECT COUNT(*) AS Qta
							FROM T_AssRuoliAzioni
							WHERE
								CodEntita=@sCodEntita	AND
								CodRuolo=@sCodRuolo AND
								CodAzione =@sCodAzione)		

		IF (@nQta > 0)
		BEGIN
						SELECT 
				R.Codice, 
				MIN(R.Descrizione) AS Descrizione			
			FROM 
				T_AssUARuoliCartellaInVisione ASS
					INNER JOIN T_Ruoli R
						ON (ASS.CodRuolo=R.Codice)		
			WHERE
				ASS.CodUA=@sCodUA AND
				R.Codice IN (
								SELECT CodVoce AS CodRuolo
								FROM T_AssRuoliAzioni
								WHERE
									CodEntita=@sCodEntita	AND
									CodRuolo=@sCodRuolo AND
									CodAzione =@sCodAzione
							)
			GROUP BY R.Codice	
			ORDER BY MIN(R.Descrizione)
		END		
		ELSE
		BEGIN
						SELECT 
				R.Codice, 
				MIN(R.Descrizione) AS Descrizione
			FROM 
				T_AssUARuoliCartellaInVisione ASS
					INNER JOIN T_Ruoli R
						ON (ASS.CodRuolo=R.Codice)		
			WHERE
				ASS.CodUA=@sCodUA
			GROUP BY R.Codice	
			ORDER BY MIN(R.Descrizione)
		END		
	END
	ELSE
	BEGIN
				SELECT 
			R.Codice, 
			MIN(R.Descrizione) AS Descrizione
		FROM 
			T_AssUARuoliCartellaInVisione ASS
				INNER JOIN T_Ruoli R
					ON (ASS.CodRuolo=R.Codice)		
		WHERE
			ASS.CodUA=@sCodUA
		GROUP BY R.Codice	
		ORDER BY MIN(R.Descrizione)
	END

END