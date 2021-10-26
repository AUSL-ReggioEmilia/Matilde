CREATE PROCEDURE [dbo].[MSP_SelAzioniEntita](@xParametri XML)
AS
BEGIN
	
		
	
				 	
	DECLARE @sCodEntita VARCHAR(20)	
	SET @sCodEntita=(SELECT TOP 1 ValoreParametro.CodEntita.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodEntita') as ValoreParametro(CodEntita))	
										
	DECLARE @sCodAzione VARCHAR(20)	
	SET @sCodAzione=(SELECT TOP 1 ValoreParametro.CodAzione.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodAzione') as ValoreParametro(CodAzione))	
	
			
				
		SELECT			
		CodEntita,		
		CodAzione,
		ISNULL(AbilitaPermessiDettaglio,0) AS AbilitaPermessiDettaglio,
		ISNULL(RegistraTimeStamp,0) AS 	RegistraTimeStamp					
	FROM 
		T_AzioniEntita
	WHERE			
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