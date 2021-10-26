CREATE PROCEDURE [dbo].[MSP_SelStatoAppuntamento](@xParametri AS XML )
AS
BEGIN
	

				 	
	DECLARE @bDatiEstesi AS Bit
	DECLARE @sCodStato AS VARCHAR(20)
	DECLARE @nOrdine AS INTEGER
	DECLARE @bIncludiStato AS Bit
	DECLARE @bStatiPerFiltro AS Bit
	DECLARE @sCodRuolo AS VARCHAR(20)
	
	DECLARE @nQta AS INTEGER
	DECLARE @bStatiPrecedenti AS Bit
	
		SET @bDatiEstesi=(SELECT TOP 1 ValoreParametro.DatiEstesi.value('.','bit')
					  FROM @xParametri.nodes('/Parametri/DatiEstesi') as ValoreParametro(DatiEstesi))
	
		SET @bIncludiStato=(SELECT TOP 1 ValoreParametro.IncludiStato.value('.','bit')
					  FROM @xParametri.nodes('/Parametri/IncludiStato') as ValoreParametro(IncludiStato))
	
		SET @bStatiPerFiltro=(SELECT TOP 1 ValoreParametro.StatiPerFiltro.value('.','bit')
					  FROM @xParametri.nodes('/Parametri/StatiPerFiltro') as ValoreParametro(StatiPerFiltro))
	SET @bStatiPerFiltro=ISNULL(@bStatiPerFiltro,0)
	  
		SET @sCodStato=(SELECT TOP 1 ValoreParametro.CodStato.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodStato') as ValoreParametro(CodStato))
	
	SET @sCodStato=ISNULL(@sCodStato,'')

		SET @sCodRuolo=(SELECT TOP 1 ValoreParametro.CodRuolo.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/TimeStamp/CodRuolo') as ValoreParametro(CodRuolo))
	SET @sCodRuolo=ISNULL(@sCodRuolo,'')

				
	IF @sCodStato <> ''
	BEGIN
					
		SET @nQta =(SELECT COUNT(*) FROM T_AssRuoliModuli
					WHERE
						CodRuolo=@sCodRuolo AND 
						CodModulo='App_StatoIndietro')

		IF @nQta > 0 
			SET @bStatiPrecedenti=1
		ELSE
			SET @bStatiPrecedenti=0

				SET @nOrdine=(SELECT TOP 1 Ordine FROM T_StatoAppuntamento WHERE Codice=@sCodStato)
		SET @nOrdine=ISNULL(@nOrdine,0)	
		
		
		
		SELECT
			Codice,
			Descrizione,
			CASE 
				WHEN  ISNULL(@bDatiEstesi,0)=0 THEN NULL
				ELSE Icona
			END AS  Icona,
			Colore,
			Ordine						 
		FROM T_StatoAppuntamento
		WHERE
			ISNULL(Riservato,0)=0
			AND
			(
				Ordine > @nOrdine														OR
				(ISNULL(@bIncludiStato,0)<>0 AND Ordine >= @nOrdine)					OR
				@bStatiPrecedenti = 1												)
			
			AND Codice <> 'CA'						ORDER BY Ordine	
		
	END
	ELSE
				SELECT
			Codice,
			Descrizione,
			CASE 
				WHEN  ISNULL(@bDatiEstesi,0)=0 THEN NULL
				ELSE Icona
			END AS  Icona,
			Colore,
			Ordine						 
		FROM T_StatoAppuntamento
		WHERE 
			1 = CASE 
					WHEN @bStatiPerFiltro =1 AND Codice NOT IN ('CA') THEN 1
					WHEN @bStatiPerFiltro =1 AND Codice IN ('CA') THEN 0
					ELSE 1
				END
			
		ORDER BY Ordine
	
	
	
	RETURN 0
END