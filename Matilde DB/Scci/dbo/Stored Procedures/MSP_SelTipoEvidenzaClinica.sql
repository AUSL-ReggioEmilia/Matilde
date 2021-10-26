
CREATE PROCEDURE [dbo].[MSP_SelTipoEvidenzaClinica](@xParametri AS XML )
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
	
		SET @sCodEntita='EVC'
		
	
	SELECT 
			Codice,
			Descrizione,
		CASE 
			WHEN ISNULL(@bDatiEstesi,0)=0 THEN NULL
			ELSE
				Icona
		END AS icona			
	FROM T_TipoEvidenzaClinica 	
	ORDER BY Descrizione
	
	RETURN 0
END