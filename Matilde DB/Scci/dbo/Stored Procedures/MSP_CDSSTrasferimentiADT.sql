
CREATE PROCEDURE [dbo].[MSP_CDSSTrasferimentiADT](@xParametri AS XML)

AS
BEGIN
	

				
	DECLARE @sCodRuolo AS VARCHAR(20)
				
		SET @sCodRuolo=(SELECT TOP 1 ValoreParametro.CodRuolo.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodRuolo') as ValoreParametro(CodRuolo))
	;
		
	
	
	SELECT 
		UA.Descrizione,
		UA.OraApertura,
		UA.OraChiusura
	FROM									
		T_UnitaAtomiche AS UA
	ORDER BY
		UA.Descrizione		

END