CREATE PROCEDURE [dbo].[MSP_SelCDSSClientDaUA](@xParametri AS XML)

AS
BEGIN
	

				
	DECLARE @sCodUA AS VARCHAR(20)
	DECLARE @sCodAzione AS VARCHAR(20)
	DECLARE @sSQL AS VARCHAR(MAX)
				
		SET @sCodUA=(SELECT TOP 1 ValoreParametro.CodUA.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodUA') as ValoreParametro(CodUA))
	
		SET @sCodAzione=(SELECT TOP 1 ValoreParametro.CodAzione.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodAzione') as ValoreParametro(CodAzione))
		
				
	DECLARE @sTmpUA AS VARCHAR(20)	
	DECLARE @xTmp AS XML
	DECLARE @sWhere AS VARCHAR(Max)
	DECLARE @sTmp AS VARCHAR(Max)

		CREATE TABLE #tmpUACDSS
	(
		CodUA VARCHAR(20) COLLATE Latin1_General_CI_AS
	)
	
	
		CREATE TABLE #tmpUAGerarchia
	(
		CodUA VARCHAR(20) COLLATE Latin1_General_CI_AS,
		Descrizione VARCHAR(255) COLLATE Latin1_General_CI_AS
	)
			
	SET @xTmp=CONVERT(XML,'<Parametri><CodUA>' + @sCodUA +'</CodUA></Parametri>')
	INSERT #tmpUAGerarchia EXEC MSP_SelUAPadri @xTmp	
	
		INSERT INTO #tmpUACDSS(CodUA)
	SELECT CodUA
	FROM  #tmpUAGerarchia
	WHERE CodUA NOT IN (SELECT CodUA FROM #tmpUACDSS)		
		
	
		CREATE INDEX IX_CodUA ON #tmpUACDSS (CodUA)    
	
	SET @sSQL='
		SELECT 
				S.ID,
				S.CodUA,
				S.CodAzione,
				A.Descrizione AS DescrizioneAzione,
				S.CodPlugin,
				P.Descrizione AS DescrizionePlugin,
				P.NomePlugin AS NomePlugin,
				P.Comando AS ComandoPlugin,
				IsNull(S.Parametri,'''') AS ModalitaPlugin,
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
		'
						
	SET @sWhere=''
							
		IF 	@sCodAzione IS NOT NULL
		BEGIN	
						SET @sTmp=  ' AND 			
								S.CodAzione ='''+ @sCodAzione + '''
						'  				
			SET @sWhere= @sWhere + @sTmp														
		END		

		IF ISNULL(@sWhere,'')<> ''
		BEGIN		
				SET @sSQL=@sSQL +' WHERE ' + RIGHT(@sWhere,len(@sWhere)-5)
		END	
		
 	SET @sSQL=@sSQL + ' ORDER BY P.Ordine ASC '
 	PRINT @sSQL

 	EXEC (@sSQL)

	
END