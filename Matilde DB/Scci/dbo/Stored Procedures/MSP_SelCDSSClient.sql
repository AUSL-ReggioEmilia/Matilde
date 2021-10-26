CREATE PROCEDURE [dbo].[MSP_SelCDSSClient](@xParametri AS XML)

AS
BEGIN
	

				
	DECLARE @sCodRuolo AS VARCHAR(20)
				
		SET @sCodRuolo=(SELECT TOP 1 ValoreParametro.CodRuolo.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodRuolo') as ValoreParametro(CodRuolo))
	;
		
					
	DECLARE @xTmp AS XML
	DECLARE @sTmpUA AS VARCHAR(20)	
	
		CREATE TABLE #tmpUACDSS
	(
		CodUA VARCHAR(20) COLLATE Latin1_General_CI_AS
	)
	
		CREATE TABLE #tmpUARuolo
	(
		CodUA VARCHAR(20) COLLATE Latin1_General_CI_AS,
		Descrizione VARCHAR(255) COLLATE Latin1_General_CI_AS
	)
	
		CREATE TABLE #tmpUAGerarchia
	(
		CodUA VARCHAR(20) COLLATE Latin1_General_CI_AS,
		Descrizione VARCHAR(255) COLLATE Latin1_General_CI_AS
	)
		
	
		SET  @xTmp=CONVERT(XML,'<Parametri><CodRuolo>'+ @sCodRuolo + '</CodRuolo></Parametri>')
	
	INSERT #tmpUARuolo EXEC MSP_SelUADaRuolo @xTmp
	

	
	INSERT INTO #tmpUACDSS
	SELECT CodUA FROM #tmpUARuolo
	
	DECLARE CurUA CURSOR LOCAL FOR 
	SELECT CodUA 	
	FROM #tmpUARuolo
	
	OPEN CurUA

	FETCH NEXT FROM CurUA 
	INTO @sTmpUA

	WHILE @@FETCH_STATUS = 0
	BEGIN	
	
				SET  @xTmp=CONVERT(XML,'<Parametri><CodUA>'+ @sTmpUA + '</CodUA></Parametri>')		
		
		TRUNCATE TABLE #tmpUAGerarchia
		
		INSERT #tmpUAGerarchia EXEC MSP_SelUAPadri @xTmp	
				
				INSERT INTO #tmpUACDSS(CodUA)
		SELECT CodUA
		FROM  #tmpUAGerarchia
		WHERE CodUA NOT IN (SELECT CodUA FROM #tmpUACDSS)
		
		FETCH NEXT FROM CurUA 
		INTO @sTmpUA
	END
	
		CREATE INDEX IX_CodUA ON #tmpUACDSS (CodUA)    
	
	SELECT 
		S.ID,
		S.CodUA,
		S.CodAzione,
		A.Descrizione AS DescrizioneAzione,
		S.CodPlugin,
		P.Descrizione AS DescrizionePlugin,
		P.NomePlugin AS NomePlugin,
		P.Comando AS ComandoPlugin,
		IsNull(S.Parametri,'') AS ModalitaPlugin,
		IsNull(P.Ordine,1) AS OrdinePlugin,
		P.Icona AS IconaPlugin
	FROM		
				
			
		T_CDSSStruttura AS S
			INNER JOIN #tmpUACDSS T
				ON S.CodUA=T.CodUA
			INNER JOIN T_CDSSAzioni A
				ON S.CodAzione = A.Codice			
			INNER JOIN T_CDSSPlugins P
				ON S.CodPlugin = P.Codice		
	ORDER BY
		P.Ordine		

END