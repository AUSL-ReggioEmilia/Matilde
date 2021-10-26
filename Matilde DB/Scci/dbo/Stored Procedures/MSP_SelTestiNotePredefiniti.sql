CREATE PROCEDURE [dbo].[MSP_SelTestiNotePredefiniti](@xParametri AS XML)
AS
BEGIN

	
	
				
	DECLARE @sCodice AS Varchar(20)  
	DECLARE @sCodUA AS Varchar(20) 
	DECLARE @sCodRuolo AS Varchar(20)
	DECLARE @sCodAzione AS VARCHAR(20)
	
		
	
		DECLARE @sSQL AS VARCHAR(MAX)
	DECLARE @sWhere AS VARCHAR(MAX)
	DECLARE @sTmp AS VARCHAR(MAX)
	DECLARE @nPos AS INTEGER
			
	SET @sCodice=(SELECT	TOP 1 ValoreParametro.Codice.value('.','VARCHAR(20)')
						 FROM @xParametri.nodes('/Parametri/Codice') as ValoreParametro(Codice))
	SET @sCodice=ISNULL(@sCodice,'')
	
	SET @sCodUA=(SELECT	TOP 1 ValoreParametro.CodUA.value('.','VARCHAR(20)')
						 FROM @xParametri.nodes('/Parametri/CodUA') as ValoreParametro(CodUA))
	SET @sCodUA=ISNULL(@sCodUA,'')
				   	 				 
	SET @sCodRuolo=(SELECT	TOP 1 ValoreParametro.CodRuolo.value('.','VARCHAR(20)')
					FROM @xParametri.nodes('/Parametri/CodRuolo') as ValoreParametro(CodRuolo)) 
	IF ISNULL(@sCodRuolo,'')='' SET @sCodRuolo='###'					
	
	SET @sCodAzione=(SELECT	TOP 1 ValoreParametro.CodAzione.value('.','VARCHAR(20)')
					 FROM @xParametri.nodes('/Parametri/CodAzione') as ValoreParametro(CodAzione))				  
	IF ISNULL(@sCodAzione,'')='' SET @sCodAzione='###'
													
											
				 	
				CREATE TABLE #tmpUA
	(
		CodUA VARCHAR(20) COLLATE Latin1_General_CI_AS,
		Descrizione VARCHAR(255) COLLATE Latin1_General_CI_AS
	)

	DECLARE @xTmp AS XML
	

	IF @sCodUA<> ''
	BEGIN
				SET  @xTmp=CONVERT(XML,'<Parametri><CodUA>'+ @sCodUA + '</CodUA></Parametri>')

		INSERT #tmpUA EXEC MSP_SelUAPadri @xTmp	
	END
	ELSE
	BEGIN
				
						CREATE TABLE #tmpUALista
			(
				CodUA VARCHAR(20) COLLATE Latin1_General_CI_AS,		
				Descrizione VARCHAR(255) COLLATE Latin1_General_CI_AS
			)
								
			SET  @xTmp=CONVERT(XML,'<Parametri><CodRuolo>'+ @sCodRuolo + '</CodRuolo></Parametri>')
			INSERT #tmpUALista EXEC MSP_SelUADaRuolo @xTmp							
									
						
			CREATE TABLE #tmpUAPadri
			(
				CodUA VARCHAR(20) COLLATE Latin1_General_CI_AS,		
				Descrizione VARCHAR(255) COLLATE Latin1_General_CI_AS
			)
			
			DECLARE @sTmpUA AS VARCHAR(20)
			
			DECLARE curUA CURSOR FOR 
			SELECT CodUA
			FROM #tmpUALista
			
			OPEN curUA

			FETCH NEXT FROM curUA 
			INTO @sTmpUA
			
			WHILE @@FETCH_STATUS = 0
			BEGIN		
				SET  @xTmp=CONVERT(XML,'<Parametri><CodUA>'+ @sTmpUA + '</CodUA></Parametri>')
				TRUNCATE TABLE #tmpUAPadri
				INSERT #tmpUAPadri EXEC MSP_SelUAPadri @xTmp
				
				INSERT INTO #tmpUA(CodUA,Descrizione)
				SELECT CodUA, Descrizione
				FROM #tmpUAPadri
				WHERE CodUA NOT IN (SELECT CodUA FROM #tmpUA)
				
				FETCH NEXT FROM curUA 
				INTO @sTmpUA
			END		
								
			DROP TABLE #tmpUAPadri
			DROP TABLE #tmpUALista
		
		
	END
	
		CREATE INDEX IX_CodUA ON #tmpUA (CodUA)    		
				
	SET @sSQL='SELECT
					W.Codice,
					D.Descrizione, 
					D.Path ,
					D.OggettoNota,
					D.DescrizioneNota,
					D.Colore
			FROM						
				(SELECT DISTINCT 
					A.CodVoce AS Codice												
				FROM					
					T_AssUAEntita A
						INNER JOIN #tmpUA T ON
							A.CodUA=T.CodUA					
				WHERE CodEntita=''TNT''			
				) AS W				
					INNER JOIN T_TestiNotePredefiniti D		
						ON D.Codice=W.Codice'
	
	SET @sWhere=''
	
	
		IF ISNULL(@sCodice,'') <> ''
	BEGIN
		SET @sTmp= ' AND W.Codice=''' + @sCodice +''''
		SET @sWhere= @sWhere + @sTmp		
	END	

   			
		IF ISNULL(@sWhere,'')<> ''
	BEGIN
	
		SET @sSQL=@sSQL +' WHERE ' + RIGHT(@sWhere,len(@sWhere)-5)		
	END	
	
	PRINT @sSQL
 	
	EXEC (@sSQL)				  			
	
		DROP TABLE #tmpUA			
	
	
	RETURN 0
END