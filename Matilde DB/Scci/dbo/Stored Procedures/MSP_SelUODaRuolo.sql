CREATE PROCEDURE [dbo].[MSP_SelUODaRuolo](@xParametri AS XML)

AS
BEGIN
	

	
	
				
	DECLARE @sCodRuolo AS VARCHAR(20)
	DECLARE @sCodAzi AS VARCHAR(20)
	DECLARE @sCodUA AS VARCHAR(20)
	
		DECLARE @xTmp AS XML
	
		SET @sCodRuolo=(SELECT TOP 1 ValoreParametro.CodRuolo.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodRuolo') as ValoreParametro(CodRuolo))
		SET @sCodAzi=(SELECT TOP 1 ValoreParametro.CodAzi.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodAzi') as ValoreParametro(CodAzi))
	
		SET  @sCodUA=(SELECT TOP 1 ValoreParametro.CodUA.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodUA') as ValoreParametro(CodUA))	
				
		CREATE TABLE #tmpUA
				(
					CodUA VARCHAR(20) COLLATE Latin1_General_CI_AS,		
					Descrizione VARCHAR(255) COLLATE Latin1_General_CI_AS
				)
									
	SET  @xTmp=CONVERT(XML,'<Parametri><CodRuolo>'+ @sCodRuolo + '</CodRuolo></Parametri>')
	INSERT #tmpUA EXEC MSP_SelUADaRuolo @xTmp			

	IF @sCodUA IS NOT NULL
	 BEGIN
		DELETE FROM #tmpUA WHERE CodUA <> @sCodUA
	 END

		IF (ISNULL(@sCodAzi,'') <> '')
	BEGIN
		SELECT DISTINCT	
			L.CodUO,
			ISNULL(O.Descrizione,L.CodUO) AS Descrizione
		FROM
			 T_AssUAUOLetti L
				INNER JOIN #tmpUA T 
				 ON (T.CodUA=L.CodUA AND
					 L.CodAzi=@sCodAzi)
				LEFT JOIN T_UnitaOperative  O
					ON (L.CodUO=O.Codice AND
				 		L.CodAzi=O.CodAzi)
	END
	ELSE
	BEGIN
		SELECT DISTINCT	
			L.CodUO,
			ISNULL(O.Descrizione,L.CodUO) AS Descrizione
		FROM
			 T_AssUAUOLetti L
				INNER JOIN #tmpUA T 
				 ON (T.CodUA=L.CodUA)
				INNER JOIN 
					T_UnitaAtomiche A
						ON (T.CodUA=A.Codice AND
						    L.CodAzi=A.CodAzienda)
				LEFT JOIN T_UnitaOperative  O
					ON (L.CodUO=O.Codice AND
				 		L.CodAzi=O.CodAzi)
				WHERE CodUO <> '*'	
		ORDER BY ISNULL(O.Descrizione,L.CodUO) ASC
	END
END