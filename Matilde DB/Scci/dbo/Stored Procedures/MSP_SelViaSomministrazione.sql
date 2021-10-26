
CREATE PROCEDURE [dbo].[MSP_SelViaSomministrazione](@xParametri AS XML )
AS
BEGIN
	

				 	
	DECLARE @bDatiEstesi AS Bit			
	DECLARE @sCodice AS VARCHAR(20)			
						
	SET @sCodice=(SELECT TOP 1 ValoreParametro.Codice.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/Codice') as ValoreParametro(Codice))	
					
	SET @bDatiEstesi=(SELECT TOP 1 ValoreParametro.DatiEstesi.value('.','bit')
					  FROM @xParametri.nodes('/Parametri/DatiEstesi') as ValoreParametro(DatiEstesi))	
					  										
				
	SELECT
		Codice,
		Descrizione,
		CASE 
			WHEN  ISNULL(@bDatiEstesi,0)=0 THEN NULL
			ELSE Icona
		END AS  Icona
	FROM T_ViaSomministrazione	
	WHERE 
		Codice=ISNULL(@sCodice,Codice)
	ORDER BY Descrizione
	RETURN 0
END