CREATE PROCEDURE [dbo].[MSP_SelStampaCartellaADT](@uIDEpisodio AS UNIQUEIDENTIFIER)
AS
BEGIN
	
	
	
	DECLARE @nPrimoIDTrasferimento AS UNIQUEIDENTIFIER
	DECLARE @dPrimaDataIngresso AS DATETIME
	DECLARE @nUltimoIDTrasferimento AS UNIQUEIDENTIFIER
	DECLARE @dUltimaDataUscita AS DATETIME
	
		SELECT TOP 1 
			@nPrimoIDTrasferimento=ID,
			@dPrimaDataIngresso=DataIngresso 
	FROM T_MovTrasferimenti MT
	WHERE IDEpisodio=@uIDEpisodio AND
		  MT.CodStatoTrasferimento IN ('AT','TR','DM')
		  ORDER BY DataIngresso 
	
		SELECT TOP 1 
					@nUltimoIDTrasferimento=ID,
					@dUltimaDataUscita=DataUscita
			FROM T_MovTrasferimenti MT
			WHERE IDEpisodio=@uIDEpisodio AND
				  MT.CodStatoTrasferimento IN ('DM')
				  ORDER BY DataIngresso 
	
	CREATE TABLE #TmpMovADT
	(
		ID INTEGER IDENTITY(1,1),
		IDTrasferimento UNIQUEIDENTIFIER,
		Movimento VARCHAR(255) COLLATE Latin1_General_CI_AS,
		DataMovimento DATETIME,
		sDataMovimento VARCHAR(30) COLLATE Latin1_General_CI_AS,
		Descrizione VARCHAR(1000) COLLATE Latin1_General_CI_AS
	)	
	
		INSERT INTO #TmpMovADT(IDTrasferimento,Movimento, DataMovimento ,sDataMovimento,Descrizione)
	SELECT 
		MT.ID AS IDTrasferimento,
		'Lista di Attesa ' +
			CASE 
				WHEN ISNULL(ME.NumeroListaAttesa,'') <> '' THEN ' (' + ME.NumeroListaAttesa + ')'
				ELSE ''
			END
		AS 	Movimento,
		ME.DataListaAttesa AS DataMovimento,
		Convert(varchar(20),ME.DataListaAttesa,105) +' ' +  Convert(varchar(5),ME.DataListaAttesa,108) AS sDataMovimento,				
				ISNULL(MT.DescrUO,ISNULL(MT.CodUO,'')) +
			+  
				CASE 
					WHEN
						RTRIM(ISNULL(MT.DescrSettore,ISNULL(MT.CodSettore,''))) <> '' THEN ' (Settore: ' + RTRIM(ISNULL(MT.DescrSettore,ISNULL(MT.CodSettore,''))) 
					ELSE ''
				END	+
				CASE
					WHEN RTRIM(ISNULL(MT.DescrLetto,ISNULL(MT.CodLetto,''))) <> '' THEN ', Letto: ' + RTRIM(ISNULL(MT.DescrLetto,ISNULL(MT.CodLetto,''))) 
					ELSE ''
				END 
				+
				CASE 
					WHEN
						ISNULL(MT.DescrSettore,ISNULL(MT.CodSettore,'')) <> '' THEN ')' 
					ELSE ''
				END	
		 AS Descrizione								
	FROM 
		T_MovTrasferimenti MT
		  INNER JOIN T_MovEpisodi ME
			ON MT.IDEpisodio=ME.ID
		  LEFT JOIN T_UnitaAtomiche UA
			ON MT.CodUA=UA.Codice			   	   	
	WHERE 
		IDEpisodio=@uIDEpisodio AND
		MT.CodStatoTrasferimento IN ('PR','PC')			
		
		INSERT INTO #TmpMovADT(IDTrasferimento,Movimento, DataMovimento ,sDataMovimento,Descrizione)
	SELECT 
		MT.ID AS IDTrasferimento,
		'Accettato' 
		AS 	Movimento,
		ME.DataRicovero AS DataMovimento,
		Convert(varchar(20),ME.DataRicovero,105) +' ' +  Convert(varchar(5),ME.DataRicovero,108) AS sDataMovimento,				
				ISNULL(MT.DescrUO,ISNULL(MT.CodUO,'')) +
			+  
				CASE 
					WHEN
						RTRIM(ISNULL(MT.DescrSettore,ISNULL(MT.CodSettore,''))) <> '' THEN ' (Settore: ' + RTRIM(ISNULL(MT.DescrSettore,ISNULL(MT.CodSettore,''))) 
					ELSE ''
				END	+
				CASE
					WHEN RTRIM(ISNULL(MT.DescrLetto,ISNULL(MT.CodLetto,''))) <> '' THEN ', Letto: ' + RTRIM(ISNULL(MT.DescrLetto,ISNULL(MT.CodLetto,'')))
					ELSE ''
				END 
				+
				CASE 
					WHEN
						ISNULL(MT.DescrSettore,ISNULL(MT.CodSettore,'')) <> '' THEN ')' 
					ELSE ''
				END	
		 AS Descrizione								
	FROM 
		T_MovTrasferimenti MT
		  INNER JOIN T_MovEpisodi ME
			ON MT.IDEpisodio=ME.ID
		  LEFT JOIN T_UnitaAtomiche UA
			ON MT.CodUA=UA.Codice	
		  LEFT JOIN T_Settori SE
			ON  MT.CodSettore=SE.Codice AND
				ME.CodAzi=SE.CodAzi		   	   	
	WHERE 
		  MT.IDEpisodio=@uIDEpisodio AND
		  MT.ID=@nPrimoIDTrasferimento AND
		  CONVERT(VARCHAR(20),ME.DataRicovero,102)=CONVERT(VARCHAR(20),@dPrimaDataIngresso,102)				  AND MT.ID NOT IN (SELECT IDTrasferimento FROM #TmpMovADT)
	
		INSERT INTO #TmpMovADT(IDTrasferimento,Movimento, DataMovimento ,sDataMovimento,Descrizione)
	SELECT 
		MT.ID AS IDTrasferimento,
		'Trasferito' 
		AS 	Movimento,
		MT.DataIngresso AS DataMovimento,
		Convert(varchar(20),MT.DataIngresso,105) +' ' +  Convert(varchar(5),MT.DataIngresso,108) AS sDataMovimento,						ISNULL(MT.DescrUO,ISNULL(MT.CodUO,'')) +
			+  
				CASE 
					WHEN
						RTRIM(ISNULL(MT.DescrSettore,ISNULL(MT.CodSettore,''))) <> '' THEN ' (Settore: ' + RTRIM(ISNULL(MT.DescrSettore,ISNULL(MT.CodSettore,''))) 
					ELSE ''
				END	+
				CASE
					WHEN RTRIM(ISNULL(MT.DescrLetto,ISNULL(MT.CodLetto,''))) <> '' THEN ', Letto: ' + RTRIM(ISNULL(MT.DescrLetto,ISNULL(MT.CodLetto,'')))
					ELSE ''
				END 
				+
				CASE 
					WHEN
						ISNULL(MT.DescrSettore,ISNULL(MT.CodSettore,'')) <> '' THEN ')' 
					ELSE ''
				END	
		 AS Descrizione								
	FROM 
		T_MovTrasferimenti MT
		  INNER JOIN T_MovEpisodi ME
			ON MT.IDEpisodio=ME.ID
		  LEFT JOIN T_UnitaAtomiche UA
			ON MT.CodUA=UA.Codice			  	   	
	WHERE 
		  IDEpisodio=@uIDEpisodio AND
		  MT.CodStatoTrasferimento IN ('AT','TR','DM')	AND
		  MT.ID NOT IN (SELECT IDTrasferimento FROM #TmpMovADT)
		  
			INSERT INTO #TmpMovADT(IDTrasferimento,Movimento, DataMovimento ,sDataMovimento,Descrizione)
	SELECT 
		MT.ID AS IDTrasferimento,
		'Dimesso' 
		AS 	Movimento,
		ME.DataDimissione AS DataMovimento,
		Convert(varchar(20),ME.DataDimissione,105) +' ' +  Convert(varchar(5),ME.DataDimissione,108) AS sDataMovimento,							ISNULL(MT.DescrUO,ISNULL(MT.CodUO,'')) +
			+  
				CASE 
					WHEN
						RTRIM(ISNULL(MT.DescrSettore,ISNULL(MT.CodSettore,''))) <> '' THEN ' (Settore: ' + RTRIM(ISNULL(MT.DescrSettore,ISNULL(MT.CodSettore,''))) 
					ELSE ''
				END	+
				CASE
					WHEN RTRIM(ISNULL(MT.DescrLetto,ISNULL(MT.CodLetto,''))) <> '' THEN ', Letto: ' + RTRIM(ISNULL(MT.DescrLetto,ISNULL(MT.CodLetto,'')))
					ELSE ''
				END 
				+
				CASE 
					WHEN
						ISNULL(MT.DescrSettore,ISNULL(MT.CodSettore,'')) <> '' THEN ')' 
					ELSE ''
				END	
		 AS Descrizione									
	FROM 
		T_MovTrasferimenti MT
		  INNER JOIN T_MovEpisodi ME
			ON MT.IDEpisodio=ME.ID
		  LEFT JOIN T_UnitaAtomiche UA
			ON MT.CodUA=UA.Codice		
			
	WHERE 
		  IDEpisodio=@uIDEpisodio AND
		  MT.ID=@nUltimoIDTrasferimento AND
		  CONVERT(VARCHAR(20),ME.DataDimissione,102)=CONVERT(VARCHAR(20),@dUltimaDataUscita,102)				  		  
	SELECT
		 '2 - Elenco Accessi' AS Sez02, 
		 Movimento,
		DataMovimento,
		sDataMovimento,
		
		Descrizione	
	FROM #TmpMovADT
	ORDER BY ID
	
	DROP TABLE #TmpMovADT
	RETURN 0
END