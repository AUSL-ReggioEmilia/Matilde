CREATE PROCEDURE [dbo].[MSP_SelStatoTrasferimento](@xParametri AS XML )
AS
BEGIN
	

				 	
	DECLARE @bDatiEstesi AS Bit
	DECLARE @bIncludiCancellati AS Bit
											
	SET @bDatiEstesi=(SELECT TOP 1 ValoreParametro.DatiEstesi.value('.','bit')
					  FROM @xParametri.nodes('/Parametri/DatiEstesi') as ValoreParametro(DatiEstesi))											
	
	SET @bIncludiCancellati=(SELECT TOP 1 ValoreParametro.DatiEstesi.value('.','bit')
					  FROM @xParametri.nodes('/Parametri/IncludiCancellati') as ValoreParametro(DatiEstesi))			
					  								
	SET @bIncludiCancellati =ISNULL(@bIncludiCancellati,0)

				
	IF (@bIncludiCancellati=1)
	BEGIN
		IF @bDatiEstesi=1
			SELECT
				Codice,
				Descrizione,
				Icona			
				Colore						 
			FROM T_StatoTrasferimento				
			ORDER BY Ordine ASC
		ELSE
			SELECT
				Codice,
				Descrizione									 
			FROM T_StatoTrasferimento			
			ORDER BY Ordine ASC
	END
	ELSE
	BEGIN
		IF @bDatiEstesi=1
			SELECT
				Codice,
				Descrizione,
				Icona			
				Colore						 
			FROM T_StatoTrasferimento	
			WHERE Codice <> 'CA'
			ORDER BY Ordine ASC
		ELSE
			SELECT
				Codice,
				Descrizione									 
			FROM T_StatoTrasferimento
			WHERE Codice <> 'CA'
			ORDER BY Ordine ASC
	END 
	RETURN 0
END