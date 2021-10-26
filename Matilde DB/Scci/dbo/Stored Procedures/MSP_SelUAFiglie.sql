CREATE PROCEDURE [dbo].[MSP_SelUAFiglie] (@xParametri AS XML)


AS
BEGIN
			
		
	
				
	DECLARE @sCodUA AS VARCHAR(20)
	
		SET @sCodUA=(SELECT TOP 1 ValoreParametro.CodUA.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodUA') as ValoreParametro(CodUA))
	;
	
	WITH GerarchiaUA(CodUAPadre,CodUAFiglia)
	AS
	( SELECT 			
			 CodPadre,
			 Codice AS CodUAFiglia	
	  FROM T_UnitaAtomiche UAP	  
	  WHERE (CodPadre = @sCodUA OR 
			 Codice =@sCodUA)			
			  				
	  UNION ALL
	  SELECT 			
			 CodPadre As CodUAPadre, 
			 Codice AS CodUAFiglia			
	  FROM T_UnitaAtomiche UAF
		INNER JOIN GerarchiaUA G
			ON UAF.CodPadre=G.CodUAFiglia		
	)		
	SELECT
	   DISTINCT  
			CodUAFiglia AS Codice,
			U.Descrizione
	FROM  GerarchiaUA G
			INNER JOIN T_UnitaAtomiche  U
				 ON G.CodUAFiglia = U.Codice	

END