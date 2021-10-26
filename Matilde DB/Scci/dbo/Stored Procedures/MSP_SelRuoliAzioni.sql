CREATE PROCEDURE [dbo].[MSP_SelRuoliAzioni](@xParametri XML)
AS
BEGIN

	
			
				
	DECLARE @sCodRuolo VARCHAR(20)	
	SET @sCodRuolo=(SELECT TOP 1 ValoreParametro.CodRuolo.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodRuolo') as ValoreParametro(CodRuolo))	
	 	
	DECLARE @sCodEntita VARCHAR(20)	
	SET @sCodEntita=(SELECT TOP 1 ValoreParametro.CodEntita.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodEntita') as ValoreParametro(CodEntita))	
										
	DECLARE @sCodAzione VARCHAR(20)	
	SET @sCodAzione=(SELECT TOP 1 ValoreParametro.CodAzione.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodAzione') as ValoreParametro(CodAzione))	
	
	
				
	SELECT	
		CodRuolo,
		CodEntita,
		CodVoce,
		CodAzione						
	FROM 
		T_AssRuoliAzioni
	WHERE	
		CodRuolo=	CASE
				WHEN ISNULL(@sCodRuolo,'')='' THEN CodRuolo
				ELSE @sCodRuolo
			END AND
		CodEntita=
			CASE
				WHEN ISNULL(@sCodEntita,'')='' THEN CodEntita
				ELSE @sCodEntita
			END AND
		CodAzione =	
			CASE	
				WHEN ISNULL(@sCodAzione,'')='' THEN CodAzione
				ELSE @sCodAzione
			END
			
				
END