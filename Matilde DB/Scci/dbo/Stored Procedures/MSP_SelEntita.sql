CREATE PROCEDURE [dbo].[MSP_SelEntita](@xParametri XML)
AS
BEGIN
	
		
	
				 	
	DECLARE @sCodEntita VARCHAR(20)
	
	SET @sCodEntita=(SELECT TOP 1 ValoreParametro.CodEntita.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodEntita') as ValoreParametro(CodEntita))	
										
	
				SELECT	
		Codice,
		Descrizione,
		ISNULL(AbilitaPermessiDettaglio,0) AS AbilitaPermessiDettaglio
	FROM 
		T_Entita E
	WHERE
		Codice=
			CASE
				WHEN ISNULL(@sCodEntita,'')='' THEN Codice
				ELSE @sCodEntita
			END 		 

				
END