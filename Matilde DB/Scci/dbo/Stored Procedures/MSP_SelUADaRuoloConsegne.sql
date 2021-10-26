CREATE PROCEDURE [dbo].[MSP_SelUADaRuoloConsegne](@xParametri AS XML)
AS
BEGIN
	

				
	DECLARE @sCodRuolo AS VARCHAR(20)
	DECLARE @sCodTipoConsegna AS VARCHAR(20)
	
		SET @sCodRuolo=(SELECT TOP 1 ValoreParametro.CodRuolo.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodRuolo') as ValoreParametro(CodRuolo))

		SET @sCodTipoConsegna=(SELECT TOP 1 ValoreParametro.CodTipoConsegna.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodTipoConsegna') as ValoreParametro(CodTipoConsegna))
	
		DECLARE @xTmp AS XML

				CREATE TABLE #tmpTipoConsegne
	(
		CodTipoConsegna VARCHAR(20) COLLATE Latin1_General_CI_AS		
	)

		IF ISNULL(@sCodTipoConsegna,'') <> ''
	BEGIN
		INSERT INTO #tmpTipoConsegne(CodTipoConsegna)
		VALUES (@sCodTipoConsegna)
	END
	ELSE
	BEGIN
		INSERT INTO #tmpTipoConsegne(CodTipoConsegna)
		SELECT CodVoce
		FROM T_AssRuoliAzioni
		WHERE
			CodEntita='CSG' AND CodAzione='INS'
			AND CodRuolo=@sCodRuolo
	END	
					
	CREATE TABLE #tmpUADaRuolo
	(
		CodUA VARCHAR(20) COLLATE Latin1_General_CI_AS,
		Descrizione VARCHAR(255) COLLATE Latin1_General_CI_AS
	)

	SET  @xTmp=CONVERT(XML,'<Parametri><CodRuolo>'+ @sCodRuolo + '</CodRuolo></Parametri>')

	INSERT #tmpUADaRuolo EXEC MSP_SelUADaRuolo @xTmp

					
	CREATE TABLE #tmpUAPerTipo
	(
		CodUA VARCHAR(20) COLLATE Latin1_General_CI_AS,		
	)

	INSERT INTO #tmpUAPerTipo(CodUA)
	SELECT CodUA
	FROM T_AssUAEntita
	WHERE CodEntita='CSG'
		AND CodVoce IN (SELECT CodTipoConsegna FROM #tmpTipoConsegne)				AND CodUA IN (SELECT CodUA FROM #tmpUADaRuolo)									
			
	CREATE TABLE #tmpUA
	(
		CodUA VARCHAR(20) COLLATE Latin1_General_CI_AS,
		Descrizione VARCHAR(255) COLLATE Latin1_General_CI_AS
	)

	;	
	WITH GerarchiaUA(CodUAPadre,CodUAFiglia)
	AS
	( SELECT 			
			 CodPadre,
			 Codice AS CodUAFiglia	
	  FROM T_UnitaAtomiche UAP	  
	  WHERE (CodPadre IN
						(SELECT CodUA						 
					     FROM #tmpUAPerTipo
						)					
			 OR 
			  Codice IN
						 (SELECT CodUA						 
					      FROM #tmpUAPerTipo)			
			  )			 
	  UNION ALL
	  SELECT 			
			 CodPadre As CodUAPadre, 
			 Codice AS CodUAFiglia			
	  FROM T_UnitaAtomiche UAF
		INNER JOIN GerarchiaUA G
			ON UAF.CodPadre=G.CodUAFiglia		
	)	
	INSERT INTO #tmpUA(CodUA,Descrizione)	
	SELECT 
	   DISTINCT 
			CodUAFiglia AS Codice,
			U.Descrizione		
	FROM  GerarchiaUA G
			INNER JOIN T_UnitaAtomiche  U
				 ON G.CodUAFiglia = U.Codice	
	
		SELECT 
		CodUA AS Codice,
		Descrizione 
	FROM #tmpUA
END