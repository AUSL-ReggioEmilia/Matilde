
CREATE PROCEDURE [dbo].[MSP_SelUADaRuolo](@xParametri AS XML)

AS
BEGIN
	

				
	DECLARE @sCodRuolo AS VARCHAR(20)
	DECLARE @nAccesso AS INTEGER								
				
		SET @sCodRuolo=(SELECT TOP 1 ValoreParametro.CodRuolo.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodRuolo') as ValoreParametro(CodRuolo))

		SET @nAccesso=(SELECT	TOP 1 ValoreParametro.Accesso.value('.','INTEGER')
								FROM @xParametri.nodes('/Parametri/Accesso') as ValoreParametro(Accesso))	
	SET @nAccesso=ISNULL(@nAccesso,2)
						  
	;
	
	WITH GerarchiaUA(CodUAPadre,CodUAFiglia)
	AS
	( SELECT 			
			 CodPadre,
			 Codice AS CodUAFiglia	
	  FROM T_UnitaAtomiche UAP	  
	  WHERE (CodPadre IN
						(SELECT CodUA						 
					     FROM T_AssRuoliUA 
						 WHERE CodRuolo=@sCodRuolo)					
			 OR 
			  Codice IN
						 (SELECT CodUA						 
						  FROM T_AssRuoliUA 
						  WHERE CodRuolo=@sCodRuolo)
			
			  )
			  And 
			  1 = CASE @nAccesso
					WHEN 0 THEN CASE WHEN ISNULL(UAP.AccessoAmbulatoriale,0)=0 THEN 1 ELSE 0 END
					WHEN 1 THEN CASE WHEN ISNULL(UAP.AccessoAmbulatoriale,0)=1 THEN 1 ELSE 0 END
					WHEN 2 THEN CASE WHEN ISNULL(UAP.AccessoAmbulatoriale,0)=1 OR ISNULL(UAP.AccessoAmbulatoriale,0)=0 THEN 1 ELSE 0 END
					END
	  UNION ALL
	  SELECT 			
			 CodPadre As CodUAPadre, 
			 Codice AS CodUAFiglia			
	  FROM T_UnitaAtomiche UAF
		INNER JOIN GerarchiaUA G
			ON UAF.CodPadre=G.CodUAFiglia	
		WHERE 1 = CASE @nAccesso
					WHEN 0 THEN CASE WHEN ISNULL(UAF.AccessoAmbulatoriale,0)=0 THEN 1 ELSE 0 END
					WHEN 1 THEN CASE WHEN ISNULL(UAF.AccessoAmbulatoriale,0)=1 THEN 1 ELSE 0 END				
					WHEN 2 THEN CASE WHEN ISNULL(UAF.AccessoAmbulatoriale,0)=1 OR ISNULL(UAF.AccessoAmbulatoriale,0)=0 THEN 1 ELSE 0 END
					END
	)		
	SELECT 
	   DISTINCT 
			CodUAFiglia AS Codice,
			U.Descrizione
	FROM  GerarchiaUA G
			INNER JOIN T_UnitaAtomiche  U
				 ON G.CodUAFiglia = U.Codice	
	ORDER BY Descrizione



END