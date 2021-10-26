CREATE PROCEDURE [dbo].[MSP_SelUADaUtente](@xParametri AS XML)

AS
BEGIN
	

				
	DECLARE @sCodUtente AS VARCHAR(100)	
				
		SET @sCodUtente=(SELECT TOP 1 ValoreParametro.CodUtente.value('.','VARCHAR(100)')
					  FROM @xParametri.nodes('/Parametri/CodUtente') as ValoreParametro(CodUtente))
						  
	;
	
		CREATE TABLE #tmpUAUtenteTmp
		(
			CodUA VARCHAR(20) COLLATE Latin1_General_CI_AS,			
		)
	
		CREATE TABLE #tmpUAUtente
	(
		CodUA VARCHAR(20) COLLATE Latin1_General_CI_AS,			
	)
		
	INSERT INTO #tmpUAUtenteTmp		
	SELECT A.CodUA
		FROM 
			T_AssLoginRuoli U				
				INNER JOIN T_AssRuoliUA A
					ON A.CodRuolo=U.CodRuolo
	WHERE CodLogin =@sCodUtente	

		INSERT INTO #tmpUAUtente
	SELECT DISTINCT CodUA FROM #tmpUAUtenteTmp
	
	;
					
	WITH GerarchiaUA(CodUAPadre,CodUAFiglia)
	AS
	( SELECT 			
			 CodPadre,
			 Codice AS CodUAFiglia	
	  FROM T_UnitaAtomiche UAP	  
	  WHERE (CodPadre IN
						(SELECT CodUA FROM  #tmpUAUtente)					
			 OR 
			  Codice IN
						 (SELECT CodUA FROM  #tmpUAUtente)
			  )			  
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
	ORDER BY Descrizione
END