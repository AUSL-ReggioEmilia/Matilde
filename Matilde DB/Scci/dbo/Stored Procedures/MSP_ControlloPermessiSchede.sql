CREATE PROCEDURE [dbo].[MSP_ControlloPermessiSchede](@xParametri XML)
AS
		
BEGIN
	
		DECLARE @sIDScheda AS VARCHAR(MAX)
	DECLARE @sCodUA AS VARCHAR(20)
	DECLARE @sCodRuolo AS VARCHAR(20)
	
		DECLARE @bModifica AS  BIT
	DECLARE @bCancella AS  BIT
	DECLARE @nTemp AS INTEGER
	DECLARE @sSQL AS VARCHAR(MAX)
						
		SET @sCodUA=(SELECT TOP 1 ValoreParametro.CodUA.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodUA') as ValoreParametro(CodUA))
	SET @sCodUA=ISNULL(@sCodUA,'')

		SET @sCodRuolo=(SELECT TOP 1 ValoreParametro.CodRuolo.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodRuolo') as ValoreParametro(CodRuolo))
	SET @sCodRuolo=ISNULL(@sCodRuolo,'')

		SET @sIDScheda=''
	SELECT	@sIDScheda =  @sIDScheda +
														CASE 
								WHEN @sIDScheda='' THEN ''
								ELSE  ','
							END + ''''
						  + ValoreParametro.IDScheda.value('.','VARCHAR(MAX)')
						  + ''''
						 FROM @xParametri.nodes('/Parametri/IDScheda') as ValoreParametro(IDScheda)
						 
	SET @sIDScheda=LTRIM(RTRIM(@sIDScheda))
	IF	@sIDScheda='''''' SET @sIDScheda=''
	SET @sIDScheda=UPPER(@sIDScheda)
					  
				
		SET @nTemp=(SELECT COUNT(*) FROM T_AssRuoliModuli
				WHERE CodModulo='Schede_Modifica'	
				AND CodRuolo=@sCodRuolo)
	IF @nTemp>=1 
		SET @bModifica=1
	ELSE
		SET @bModifica=0	


		SET @nTemp=(SELECT COUNT(*) FROM T_AssRuoliModuli
				WHERE CodModulo='Schede_Cancella'	
				AND CodRuolo=@sCodRuolo)
	IF @nTemp>=1 
		SET @bCancella=1
	ELSE
		SET @bCancella=0
							CREATE TABLE #tmpUA
	(
		CodUA VARCHAR(20) COLLATE Latin1_General_CI_AS,
		Descrizione VARCHAR(255) COLLATE Latin1_General_CI_AS
	)

	DECLARE @xTmp AS XML
	
	SET @xTmp =CONVERT(XML,'<Parametri><CodRuolo>'+@sCodRuolo+'</CodRuolo></Parametri>')
	
	INSERT #tmpUA EXEC MSP_SelUADaRuolo @xTmp	
		
	CREATE INDEX IX_CodUA ON #tmpUA (CodUA)   
	
				
		CREATE TABLE #tmpUAPadri
	(
		CodUA VARCHAR(20) COLLATE Latin1_General_CI_AS,
		Descrizione VARCHAR(255) COLLATE Latin1_General_CI_AS
	)
	
	CREATE INDEX IX_CodUA ON #tmpUAPadri (CodUA) 
										
					CREATE TABLE #tmpUAGerarchia
	(
		CodUA VARCHAR(20) COLLATE Latin1_General_CI_AS,
		Descrizione VARCHAR(255) COLLATE Latin1_General_CI_AS
	)
	
	CREATE INDEX IX_CodUA ON #tmpUAGerarchia (CodUA) 
	
		
				DECLARE @sUATemp AS VARCHAR(20)
		
	DECLARE CursoreUA CURSOR FOR 
	SELECT CodUA
		FROM #tmpUA
	FOR READ ONLY ;
	
	OPEN CursoreUA
	
	FETCH NEXT FROM CursoreUA INTO @sUATemp
		
	WHILE @@FETCH_STATUS = 0
	BEGIN	
	
		SET @xTmp =CONVERT(XML,'<Parametri><CodUA>'+@sUATemp+'</CodUA></Parametri>')
	
		INSERT #tmpUAPadri EXEC MSP_SelUAPadri @xTmp	
		
		INSERT INTO #tmpUAGerarchia(CodUA,Descrizione)
		SELECT CodUA,Descrizione
		FROM #tmpUAPadri
		WHERE CodUA NOT IN (SELECT CodUA FROM #tmpUAGerarchia)
		
		FETCH NEXT FROM CursoreUA INTO @sUATemp
	END
		
		
		
					
	CREATE TABLE #tmpSchedeAbilitate
	(
		Codice VARCHAR(20) COLLATE Latin1_General_CI_AS
	)
	
	DECLARE @UATemp AS VARCHAR(20)
				
	INSERT #tmpSchedeAbilitate 
	SELECT W.Codice	 
			FROM						
				(SELECT DISTINCT 
					A.CodVoce AS Codice												
				 FROM					
					T_AssUAEntita A
						INNER JOIN #tmpUAGerarchia T ON
							A.CodUA=T.CodUA					
				 WHERE CodEntita='SCH'			
				 ) AS W				
				 
	CREATE INDEX IX_Codice ON #tmpSchedeAbilitate (Codice)   
	
						CREATE TABLE #tmpSchedeRuolo
		(
			Codice VARCHAR(20) COLLATE Latin1_General_CI_AS
		)
	
	INSERT INTO #tmpSchedeRuolo(Codice)
			SELECT DISTINCT CodVoce FROM T_AssRuoliAzioni
			WHERE CodAzione='INS' AND
				  CodEntita='SCH' AND
				  CodRuolo =@sCodRuolo						
				  
	CREATE INDEX IX_CodUA ON #tmpSchedeRuolo (Codice)    
	
		
		DELETE FROM #tmpSchedeAbilitate
	WHERE Codice NOT IN (SELECT Codice FROM #tmpSchedeRuolo)
				
		
	SET @sSQL = ''
	
	SET @sSQL = @sSQL +'
	SELECT 	
			ID,	
			CONVERT(INTEGER,
						' + CONVERT(VARCHAR(1),@bModifica ) + ' &																							CASE 						
							WHEN ISNULL(TUS.Codice,'''') <> ''''  THEN 1
							ELSE 0
						END	&
						CASE
							WHEN ISNULL(M.Validata, 0)=1 THEN 0
							ELSE 1
						END	
						)	
				AS PermessoModifica,
			CONVERT(INTEGER,
					' + CONVERT(VARCHAR(1),@bCancella) + ' &																						CASE 
						WHEN ISNULL(TUS.Codice,'''') <> ''''  THEN 1											
						ELSE 0
					END &
					CASE
						WHEN ISNULL(M.Validata, 0)=1 THEN 0
						ELSE 1
					END	
				)	
				AS PermessoCancella
	FROM 
		T_MovSchede M
								LEFT JOIN #tmpSchedeAbilitate TUS  WITH (NOLOCK)  ON
						M.CodScheda=TUS.Codice	'

IF @sIDScheda <> ''
BEGIN
	SET @sSQL = @sSQL +' WHERE
													M.ID IN (' + @sIDScheda + ')'
END
ELSE
BEGIN
	SET @sSQL = @sSQL +' WHERE 1=0'					
END 
		 EXEC (@sSQL)

END