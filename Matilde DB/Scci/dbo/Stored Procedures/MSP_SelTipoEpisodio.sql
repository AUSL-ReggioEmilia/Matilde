
CREATE PROCEDURE [dbo].[MSP_SelTipoEpisodio](@xParametri AS XML )
AS
BEGIN
	

				 	
	DECLARE @bDatiEstesi AS Bit			
											
	SET @bDatiEstesi=(SELECT TOP 1 ValoreParametro.DatiEstesi.value('.','bit')
					  FROM @xParametri.nodes('/Parametri/DatiEstesi') as ValoreParametro(DatiEstesi))											
	
				
	IF @bDatiEstesi=1 
		SELECT
			Codice,
			Descrizione,
			Icona						 
		FROM T_TipoEpisodio		
		ORDER BY Descrizione
	ELSE
		SELECT
			Codice,
			Descrizione					 
		FROM T_TipoEpisodio	
		ORDER BY Descrizione
		
	RETURN 0
END