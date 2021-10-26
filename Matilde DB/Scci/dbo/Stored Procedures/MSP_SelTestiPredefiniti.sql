CREATE PROCEDURE [dbo].[MSP_SelTestiPredefiniti](@xParametri AS XML)
AS
BEGIN

	
	
				
	DECLARE @sCodice AS Varchar(20)  
	DECLARE @sCodUA AS Varchar(20) 
	DECLARE @sCodRuolo AS Varchar(20)
	DECLARE @sCodAzione AS VARCHAR(20)
	DECLARE @sCodEntitaTesto AS Varchar (20)
	DECLARE @sCodTipoEntita AS Varchar (50)
	DECLARE @sIDCampo AS Varchar (50)
	DECLARE @sCodCampo AS Varchar (50)
	DECLARE @bDatiEstesi AS Bit
	
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
	
	SET @sCodEntitaTesto=(SELECT TOP 1 ValoreParametro.CodEntitaTesto.value('.','VARCHAR(20)')
				 FROM @xParametri.nodes('/Parametri/CodEntitaTesto') as ValoreParametro(CodEntitaTesto))
	SET @sCodEntitaTesto=ISNULL(@sCodEntitaTesto,'')
	
	SET @sCodTipoEntita=(SELECT TOP 1 ValoreParametro.CodTipoEntita.value('.','VARCHAR(50)')
				 FROM @xParametri.nodes('/Parametri/CodTipoEntita') as ValoreParametro(CodTipoEntita))
	SET @sCodTipoEntita=ISNULL(@sCodTipoEntita,'')
	
	
	SET @sIDCampo=(SELECT TOP 1 ValoreParametro.IDCampo.value('.','VARCHAR(50)')
				 FROM @xParametri.nodes('/Parametri/IDCampo') as ValoreParametro(IDCampo))
	SET @sIDCampo=ISNULL(@sIDCampo,'')

											
	SET @bDatiEstesi=(SELECT TOP 1 ValoreParametro.DatiEstesi.value('.','bit')
					  FROM @xParametri.nodes('/Parametri/DatiEstesi') as ValoreParametro(DatiEstesi))
											
				 	
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
					D.Descrizione, 	'
	
	IF ISNULL(@bDatiEstesi,0)=1
		SET @sSQL=@sSQL + ' D.TestoRTF AS TestoRTF,'
		
	SET @sSQL=@sSQL + 'D.Path 
			FROM						
				(SELECT DISTINCT 
					A.CodVoce AS Codice												
				FROM					
					T_AssUAEntita A
						INNER JOIN #tmpUA T ON
							A.CodUA=T.CodUA					
				WHERE CodEntita=''TST''			
				) AS W				
					INNER JOIN T_TestiPredefiniti D								ON D.Codice=W.Codice'
	
	SET @sWhere=''
		IF ISNULL(@sCodEntitaTesto,'') <> ''
	BEGIN
		SET @sTmp= ' AND D.CodEntita=''' + @sCodEntitaTesto +''''
		SET @sWhere= @sWhere + @sTmp		
	END	
	
		IF ISNULL(@sCodice,'') <> ''
	BEGIN
		SET @sTmp= ' AND W.Codice=''' + @sCodice +''''
		SET @sWhere= @sWhere + @sTmp		
	END
	
	IF ISNULL(@sCodTipoEntita,'')<> '' AND ISNULL(@sIDCampo,'') <> ''
	BEGIN
		
				SET @nPos=CHARINDEX('_',REVERSE(@sIDCampo),1)
		IF @nPos > 0		
			SET @sCodCampo=LEFT(@sIDCampo,LEN(@sIDCampo)-@nPos)
			
				SET @sTmp= ' AND (D.Codice IN 
							(SELECT 
									CodTestoPredefinito
							 FROM 	
									T_AssTestiPredefinitiCampi ASS
							 WHERE										
								ASS.CodEntita=''' + @sCodEntitaTesto +''' 
								AND ASS.CodTipoEntita=''' + @sCodTipoEntita +''' 
								AND ASS.CodCampo=''' + @sCodCampo +'''
							)
						 OR
							D.Codice NOT IN (SELECT CodTestoPredefinito
										  FROM 	
										  T_AssTestiPredefinitiCampi ASS2
										  WHERE										
										  ASS2.CodEntita=''' + @sCodEntitaTesto +'''
										  )
						 )				   
					'		
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