CREATE PROCEDURE [dbo].[MSP_SelStatoCartella](@xParametri AS XML )
AS
BEGIN
	

				 	
	DECLARE @bDatiEstesi AS Bit			
	DECLARE @bIgnoraDaAprire AS Bit			
			
		SET @bDatiEstesi=(SELECT TOP 1 ValoreParametro.DatiEstesi.value('.','bit')
					  FROM @xParametri.nodes('/Parametri/DatiEstesi') as ValoreParametro(DatiEstesi))											
	
	SET @bDatiEstesi=ISNULL(@bDatiEstesi,0) 
	
		SET @bIgnoraDaAprire=(SELECT TOP 1 ValoreParametro.IgnoraDaAprire.value('.','bit')
					  FROM @xParametri.nodes('/Parametri/IgnoraDaAprire') as ValoreParametro(IgnoraDaAprire))											
	
	SET @bIgnoraDaAprire=ISNULL(@bIgnoraDaAprire,0)
	
				SELECT
		Codice,
		Descrizione,
		CASE 
			WHEN  ISNULL(@bDatiEstesi,0)=0 THEN NULL
			ELSE Icona
		END AS  Icona,
		Colore						 
	FROM T_StatoCartella
	WHERE 	
				Codice <> CASE	
					WHEN @bIgnoraDaAprire=1 THEN 'DA'
					ELSE ''
				   END
		AND
		Codice NOT IN ('CA')
	
	RETURN 0
END