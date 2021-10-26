CREATE PROCEDURE [dbo].[MSP_SelUO](@xParametri AS XML)

AS
BEGIN
	

	
	
				
	DECLARE @sCodRuolo AS VARCHAR(20)
	DECLARE @sCodUO AS VARCHAR(20)
	DECLARE @sCodAzi AS VARCHAR(20)
	
		DECLARE @xTmp AS XML
	
	
		SET @sCodUO=(SELECT TOP 1 ValoreParametro.CodUO.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodUO') as ValoreParametro(CodUO))
		SET @sCodAzi=(SELECT TOP 1 ValoreParametro.CodAzi.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodAzi') as ValoreParametro(CodAzi))
		
				

	SELECT DISTINCT	
		L.CodUO,
		ISNULL(O.Descrizione,L.CodUO) AS Descrizione
	FROM
		 T_AssUAUOLetti L			
			LEFT JOIN T_UnitaOperative  O
				ON (L.CodUO=O.Codice AND
				 	L.CodAzi=O.CodAzi)
	WHERE L.CodUO=@sCodUO  AND	
			  L.CodAzi=@sCodAzi

SELECT * FROM T_AssUAUOLetti
END