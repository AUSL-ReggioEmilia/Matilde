
CREATE PROCEDURE [dbo].[MSP_SelModuli](@xParametri XML)
AS
BEGIN
		
	
				DECLARE @sCodRuolo AS VARCHAR(20)
	
	SET @sCodRuolo=(SELECT TOP 1 ValoreParametro.CodRuolo.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodRuolo') as ValoreParametro(CodRuolo))	

				SELECT	
			M.Codice, 
			M.Descrizione,
			CASE 
				WHEN ISNULL(A.CodModulo,'')='' THEN 0
				ELSE 1
			END AS Abilitato	 		
		 FROM T_Moduli  M
			INNER JOIN 
				(SELECT CodModulo FROM 
					T_AssRuoliModuli 
				 WHERE CodRuolo=@sCodRuolo	
				) A
			 ON  M.Codice=A.CodModulo
	
END