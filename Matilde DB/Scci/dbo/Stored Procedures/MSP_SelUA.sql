CREATE PROCEDURE [dbo].[MSP_SelUA](@xParametri AS XML)

AS
BEGIN
	

				
	DECLARE @sCodUA AS VARCHAR(20)	
		
		SET @sCodUA=(SELECT TOP 1 ValoreParametro.CodUA.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodUA') as ValoreParametro(CodUA))					  
	
	SELECT 
		Codice,
		Descrizione,		
		CASE 
			WHEN LEN(ISNULL(OraApertura,''))=5 AND LEN(ISNULL(OraChiusura,''))=5 THEN OraApertura
			ELSE NULL
		END AS OraApertura ,	
		CASE
			WHEN LEN(ISNULL(OraApertura,''))=5 AND LEN(ISNULL(OraChiusura,''))=5 THEN OraChiusura
			ELSE NULL
		END	AS OraChiusura,
		CodAzienda 
	FROM T_UnitaAtomiche
	WHERE Codice=@sCodUA
END