
CREATE PROCEDURE [dbo].[MSP_SelUAModuli](@xParametri AS XML)

AS
BEGIN
	

				
	DECLARE @sCodUA AS VARCHAR(20)
	DECLARE @sCodModulo AS VARCHAR(50)
				
		SET @sCodUA=(SELECT TOP 1 ValoreParametro.CodUA.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodUA') as ValoreParametro(CodUA))
				
		SET @sCodModulo=(SELECT TOP 1 ValoreParametro.CodModulo.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/CodModulo') as ValoreParametro(CodModulo))
	;
	
	SELECT 
		A.CodModulo AS Codice,
		M.Descrizione
	FROM T_AssUAModuli A
		INNER JOIN T_ModuliUA M
			ON A.CodModulo=M.Codice
	WHERE CodUA=@sCodUA
		And A.CodModulo = ISNULL(@sCodModulo, A.CodModulo)


END