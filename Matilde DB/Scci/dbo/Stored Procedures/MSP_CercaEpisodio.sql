CREATE PROCEDURE [dbo].[MSP_CercaEpisodio](@xParametri AS XML)
AS
BEGIN
	

				
	DECLARE @sFiltroGenerico AS Varchar(500) 
	DECLARE @sDataNascita AS VARCHAR(10)
	DECLARE @bDatiEstesi AS Bit	
	DECLARE @sCodRuolo AS Varchar(20)
	DECLARE @nNumRighe AS INTEGER
	
	DECLARE @sCodUA AS Varchar(MAX)
	DECLARE @sCodUAFiltro AS Varchar(MAX)
	DECLARE @sCodTipoEpisodio AS Varchar(MAX)
	DECLARE @sCodStatoTrasferimento AS Varchar(MAX)
	DECLARE @sCodStatoEpisodio AS Varchar(MAX)
	DECLARE @sCodStatoEpisodioCartelleAperte AS Varchar(MAX)
	DECLARE @sCodStatoCartella AS Varchar(MAX)	
	DECLARE @sCodStatoCartellaInfo AS Varchar(MAX)	
	DECLARE @sUnitaOperativa AS Varchar(MAX)
	DECLARE @sSettore AS Varchar(MAX)
	DECLARE @sStanza AS Varchar(MAX)
	DECLARE @sOrdinamento AS Varchar(500)
	DECLARE @sCodLogin AS VARCHAR(100)
	
	DECLARE @dDataUscitaInizioFiltro AS DATETIME
	DECLARE @dDataUscitaFineFiltro AS DATETIME
	
	DECLARE @bSoloCartelleInVisione AS BIT	
	DECLARE @bSoloPazientiSeguiti AS BIT	
	DECLARE @bSoloUltimoTrasferimentoCartella AS BIT	
	DECLARE @bSoloUltimoTrasferimentoCartellaNoSospesi AS BIT	
	DECLARE @bSoloCartelleDaChiudere AS BIT	
	DECLARE @bSoloCartelleDaChiudereInfo AS BIT	
	DECLARE @sCodFiltroSpeciale	AS VARCHAR(20)	
	DECLARE @uIDTrasferimento AS VARCHAR(50)	

		
		DECLARE @sSQL AS VARCHAR(MAX)
	DECLARE @sSQLCartelleInVisione AS VARCHAR(MAX)
	DECLARE @sSQLPazientiSeguiti AS VARCHAR(MAX)
	DECLARE @sSQLCIV AS VARCHAR(MAX)
	DECLARE @sSQLCIVMia AS VARCHAR(MAX)
	DECLARE @sSQLPS AS VARCHAR(MAX)
	DECLARE @sSQLCNC AS VARCHAR(MAX)
	DECLARE @sDataTmp AS VARCHAR(20)
	DECLARE @sSQLFiltro AS VARCHAR(MAX)	
				
		SET @sFiltroGenerico=(SELECT	TOP 1 ValoreParametro.FiltroGenerico.value('.','VARCHAR(500)')
						 FROM @xParametri.nodes('/Parametri/FiltroGenerico') as ValoreParametro(FiltroGenerico))						 
	SET @sFiltroGenerico= ISNULL(@sFiltroGenerico,'')
	SET @sFiltroGenerico=LTRIM(RTRIM(@sFiltroGenerico))
	
		SET @sFiltroGenerico=REPLACE(@sFiltroGenerico,'''','''''')			
	
		SET @sOrdinamento=(SELECT	TOP 1 ValoreParametro.Ordinamento.value('.','VARCHAR(500)')
						 FROM @xParametri.nodes('/Parametri/Ordinamento') as ValoreParametro(Ordinamento))						 
	SET @sOrdinamento= ISNULL(@sOrdinamento,'')
	SET @sOrdinamento=LTRIM(RTRIM(@sOrdinamento))
	
	
		SET @sDataNascita=(SELECT	TOP 1 ValoreParametro.DataNascita.value('.','VARCHAR(10)')
						 FROM @xParametri.nodes('/Parametri/DataNascita') as ValoreParametro(DataNascita))						 
	SET @sDataNascita= ISNULL(@sDataNascita,'')
	SET @sDataNascita=LTRIM(RTRIM(@sDataNascita))
	
												
	SET @bDatiEstesi=(SELECT TOP 1 ValoreParametro.DatiEstesi.value('.','bit')
					  FROM @xParametri.nodes('/Parametri/DatiEstesi') as ValoreParametro(DatiEstesi))											
	SET @bDatiEstesi=ISNULL(@bDatiEstesi,0)
	
	
		SET @sCodRuolo=(SELECT	TOP 1 ValoreParametro.CodRuolo.value('.','VARCHAR(20)')
						 FROM @xParametri.nodes('/Parametri/CodRuolo') as ValoreParametro(CodRuolo))						 
	SET @sCodRuolo= LTRIM(RTRIM(ISNULL(@sCodRuolo,'')))						 		
	
		SET @nNumRighe=(SELECT	TOP 1 ValoreParametro.NumRighe.value('.','INTEGER')
								FROM @xParametri.nodes('/Parametri/NumRighe') as ValoreParametro(NumRighe))
	SET @nNumRighe=ISNULL(@nNumRighe,0)
	
		SET @sCodUA=''
	SELECT	@sCodUA =  @sCodUA +
														CASE 
								WHEN @sCodUA='' THEN ''
								ELSE  ','
							END + ''''
						  + ValoreParametro.CodUA.value('.','VARCHAR(1800)')
						  + ''''
						 FROM @xParametri.nodes('/Parametri/CodUA') as ValoreParametro(CodUA)
	SET @sCodUA=LTRIM(RTRIM(@sCodUA))
	IF	@sCodUA='''''' SET @sCodUA=''
		
		SET @sCodTipoEpisodio=''
	SELECT	@sCodTipoEpisodio =  @sCodTipoEpisodio +
														CASE 
								WHEN @sCodTipoEpisodio='' THEN ''
								ELSE  ','
							END + ''''
						  + ValoreParametro.CodTipoEpisodio.value('.','VARCHAR(1800)')
						  + ''''
						 FROM @xParametri.nodes('/Parametri/CodTipoEpisodio') as ValoreParametro(CodTipoEpisodio)					 
	
	SET @sCodTipoEpisodio=LTRIM(RTRIM(@sCodTipoEpisodio))
	IF	@sCodTipoEpisodio='''''' SET @sCodTipoEpisodio=''
		
		SET @sCodStatoTrasferimento=''
	SELECT	@sCodStatoTrasferimento =  @sCodStatoTrasferimento +
														CASE 
								WHEN @sCodStatoTrasferimento='' THEN ''
								ELSE  ','
							END + ''''
						  + ValoreParametro.CodStatoTrasferimento.value('.','VARCHAR(1800)')
						  + ''''
						 FROM @xParametri.nodes('/Parametri/CodStatoTrasferimento') as ValoreParametro(CodStatoTrasferimento)	
	SET @sCodStatoTrasferimento=LTRIM(RTRIM(@sCodStatoTrasferimento))
	IF	@sCodStatoTrasferimento='''''' SET @sCodStatoTrasferimento=''
	
		SET @sCodStatoCartella=''
	SELECT	@sCodStatoCartella =  @sCodStatoCartella +
														CASE 
								WHEN @sCodStatoCartella='' THEN ''
								ELSE  ','
							END + ''''
						  + ValoreParametro.CodStatoCartella.value('.','VARCHAR(1800)')
						  + ''''
						 FROM @xParametri.nodes('/Parametri/CodStatoCartella') as ValoreParametro(CodStatoCartella)	
	SET @sCodStatoCartella=LTRIM(RTRIM(@sCodStatoCartella))
	IF	@sCodStatoCartella='''''' SET @sCodStatoCartella=''	

		SET @sCodStatoCartellaInfo=''
	SELECT	@sCodStatoCartellaInfo =  @sCodStatoCartellaInfo +
														CASE 
								WHEN @sCodStatoCartellaInfo='' THEN ''
								ELSE  ','
							END + ''''
						  + ValoreParametro.CodStatoCartellaInfo.value('.','VARCHAR(1800)')
						  + ''''
						 FROM @xParametri.nodes('/Parametri/CodStatoCartellaInfo') as ValoreParametro(CodStatoCartellaInfo)	
	SET @sCodStatoCartellaInfo=LTRIM(RTRIM(@sCodStatoCartellaInfo))
	IF	@sCodStatoCartellaInfo='''''' SET @sCodStatoCartellaInfo=''


		SET @sUnitaOperativa=''
	SELECT	@sUnitaOperativa =  @sUnitaOperativa +
														CASE 
								WHEN @sUnitaOperativa='' THEN ''								
								ELSE  ','
							END + ''''
						  + REPLACE(ValoreParametro.UnitaOperativa.value('.','VARCHAR(1800)'),'''','''''')
						  + ''''
						 FROM @xParametri.nodes('/Parametri/UnitaOperativa') as ValoreParametro(UnitaOperativa)	
	SET @sUnitaOperativa=LTRIM(RTRIM(@sUnitaOperativa))

	IF LEN(@sUnitaOperativa) > 6
	BEGIN
		IF RIGHT(@sUnitaOperativa,6) = ' - RE''' 
		BEGIN
			SET @sUnitaOperativa = @sUnitaOperativa + ',' + LEFT(@sUnitaOperativa,LEN(@sUnitaOperativa)-6) + ''''
		END
	END

	IF	@sUnitaOperativa='''''' SET @sUnitaOperativa=''
		
		SET @sSettore=''
	SELECT	@sSettore =  @sSettore +
														CASE 
								WHEN @sSettore='' THEN ''
								ELSE  ','
							END + ''''
						  + ValoreParametro.Settore.value('.','VARCHAR(1800)')
						  + ''''
						 FROM @xParametri.nodes('/Parametri/Settore') as ValoreParametro(Settore)	
	SET @sSettore=LTRIM(RTRIM(@sSettore))
	IF	@sSettore='''''' SET @sSettore=''

		SET @sStanza=''
	SELECT	@sStanza =  @sStanza +
														CASE 
								WHEN @sStanza='' THEN ''
								ELSE  ','
							END + ''''
						  + ValoreParametro.Stanza.value('.','VARCHAR(1800)')
						  + ''''
						 FROM @xParametri.nodes('/Parametri/Stanza') as ValoreParametro(Stanza)	
	SET @sStanza=LTRIM(RTRIM(@sStanza))	
	IF	@sStanza='''''' SET @sStanza=''				 

		
	
		SET @bSoloCartelleInVisione=(SELECT TOP 1 ValoreParametro.SoloCartelleInVisione.value('.','bit')
					  FROM @xParametri.nodes('/Parametri/SoloCartelleInVisione') as ValoreParametro(SoloCartelleInVisione))											
	SET @bSoloCartelleInVisione=ISNULL(@bSoloCartelleInVisione,0)
	
		SET @bSoloPazientiSeguiti=(SELECT TOP 1 ValoreParametro.SoloPazientiSeguiti.value('.','bit')
					  FROM @xParametri.nodes('/Parametri/SoloPazientiSeguiti') as ValoreParametro(SoloPazientiSeguiti))											
	SET @bSoloPazientiSeguiti=ISNULL(@bSoloPazientiSeguiti,0)
	
		SET @bSoloUltimoTrasferimentoCartella=(SELECT TOP 1 ValoreParametro.SoloUltimoTrasferimentoCartella.value('.','bit')
					  FROM @xParametri.nodes('/Parametri/SoloUltimoTrasferimentoCartella') as ValoreParametro(SoloUltimoTrasferimentoCartella))											
	SET @bSoloUltimoTrasferimentoCartella=ISNULL(@bSoloUltimoTrasferimentoCartella,0)
	
		SET @bSoloUltimoTrasferimentoCartellaNoSospesi=(SELECT TOP 1 ValoreParametro.SoloUltimoTrasferimentoCartellaNoSospesi.value('.','bit')
					  FROM @xParametri.nodes('/Parametri/SoloUltimoTrasferimentoCartellaNoSospesi') as ValoreParametro(SoloUltimoTrasferimentoCartellaNoSospesi))											
	SET @bSoloUltimoTrasferimentoCartellaNoSospesi=ISNULL(@bSoloUltimoTrasferimentoCartellaNoSospesi,0)
	
		SET @bSoloCartelleDaChiudere=(SELECT TOP 1 ValoreParametro.SoloCartelleDaChiudere.value('.','bit')
					  FROM @xParametri.nodes('/Parametri/SoloCartelleDaChiudere') as ValoreParametro(SoloCartelleDaChiudere))											
	SET @bSoloCartelleDaChiudere=ISNULL(@bSoloCartelleDaChiudere,0)
			
		SET @bSoloCartelleDaChiudereInfo=(SELECT TOP 1 ValoreParametro.SoloCartelleDaChiudereInfo.value('.','bit')
					  FROM @xParametri.nodes('/Parametri/SoloCartelleDaChiudereInfo') as ValoreParametro(SoloCartelleDaChiudereInfo))											
	SET @bSoloCartelleDaChiudereInfo=ISNULL(@bSoloCartelleDaChiudereInfo,0)

		SET @sDataTmp=(SELECT TOP 1 ValoreParametro.DataUscitaInizio.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/DataUscitaInizio') as ValoreParametro(DataUscitaInizio))					  
	SET @sDataTmp=ISNULL(@sDataTmp,'')
	IF @sDataTmp<> '' 
		BEGIN
						SET @sDataTmp= SUBSTRING(@sDataTmp,4,2) + '-' 
						+ LEFT(@sDataTmp,2) + '-' +
						+ SUBSTRING(@sDataTmp,7,4) 
									IF ISDATE(@sDataTmp)=1
				SET	@dDataUscitaInizioFiltro=CONVERT(DATETIME,@sDataTmp,120)						
			ELSE
				SET	@dDataUscitaInizioFiltro =NULL			
		END
		
		
		SET @sDataTmp=(SELECT TOP 1 ValoreParametro.DataUscitaFine.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/DataUscitaFine') as ValoreParametro(DataUscitaFine))
	SET @sDataTmp=ISNULL(@sDataTmp,'')
	IF @sDataTmp<> '' 
		BEGIN
						SET @sDataTmp= SUBSTRING(@sDataTmp,4,2) + '-' 
						+ LEFT(@sDataTmp,2) + '-' +
						+ SUBSTRING(@sDataTmp,7,4)
									IF ISDATE(@sDataTmp)=1
				SET	@dDataUscitaFineFiltro=CONVERT(DATETIME,@sDataTmp,120)						
			ELSE
				SET	@dDataUscitaFineFiltro =NULL		
		END 
		
		SET @sCodLogin=(SELECT	TOP 1 ValoreParametro.CodLogin.value('.','VARCHAR(100)')
						 FROM @xParametri.nodes('/Parametri/TimeStamp/CodLogin') as ValoreParametro(CodLogin))						 
	SET @sCodLogin= LTRIM(RTRIM(ISNULL(@sCodLogin,'')))	
	
		SET @sCodFiltroSpeciale=(SELECT TOP 1 ValoreParametro.CodFiltroSpeciale.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodFiltroSpeciale') as ValoreParametro(CodFiltroSpeciale))
					  
		SET @uIDTrasferimento=(SELECT TOP 1 ValoreParametro.IDTrasferimento.value('.','UNIQUEIDENTIFIER')
					  FROM @xParametri.nodes('/Parametri/IDTrasferimento') as ValoreParametro(IDTrasferimento))
	
		SET @sCodStatoEpisodio=''
	SELECT	@sCodStatoEpisodio =  @sCodStatoEpisodio +
														CASE 
								WHEN @sCodStatoEpisodio='' THEN ''
								ELSE  ','
							END + ''''
						  + ValoreParametro.CodStatoEpisodio.value('.','VARCHAR(1800)')
						  + ''''
						 FROM @xParametri.nodes('/Parametri/CodStatoEpisodio') as ValoreParametro(CodStatoEpisodio)	
	SET @sCodStatoEpisodio=LTRIM(RTRIM(@sCodStatoEpisodio))
	IF	@sCodStatoEpisodio='''''' SET @sCodStatoEpisodio=''

		SET @sCodStatoEpisodioCartelleAperte=''
	SELECT	@sCodStatoEpisodioCartelleAperte =  @sCodStatoEpisodioCartelleAperte +
														CASE 
								WHEN @sCodStatoEpisodioCartelleAperte='' THEN ''
								ELSE  ','
							END + ''''
						  + ValoreParametro.CodStatoEpisodioCartelleAperte.value('.','VARCHAR(1800)')
						  + ''''
						 FROM @xParametri.nodes('/Parametri/CodStatoEpisodioCartelleAperte') as ValoreParametro(CodStatoEpisodioCartelleAperte)	
	SET @sCodStatoEpisodioCartelleAperte=LTRIM(RTRIM(@sCodStatoEpisodioCartelleAperte))
	IF	@sCodStatoEpisodioCartelleAperte='''''' SET @sCodStatoEpisodioCartelleAperte=''

				
				
	DECLARE @xTmp AS XML
			
	CREATE TABLE #tmpUARuolo
	(
		CodUA VARCHAR(20) COLLATE Latin1_General_CI_AS,
		Descrizione VARCHAR(255) COLLATE Latin1_General_CI_AS
	)


	SET  @xTmp=CONVERT(XML,'<Parametri><CodRuolo>'+ @sCodRuolo + '</CodRuolo></Parametri>')
	
		INSERT #tmpUARuolo EXEC MSP_SelUADaRuolo @xTmp
	
	CREATE INDEX IX_CodUA ON #tmpUARuolo (CodUA)    
	
					
	IF @sCodUA<> ''		
	BEGIN
		
				CREATE TABLE #tmpUAFiglie
		(
			CodUA VARCHAR(20) COLLATE Latin1_General_CI_AS,
			Descrizione VARCHAR(255) COLLATE Latin1_General_CI_AS
		)
		
				CREATE TABLE #tmpUALista
		(
			CodUA VARCHAR(20) COLLATE Latin1_General_CI_AS,		
		)
		INSERT INTO #tmpUALista(CodUA)
		SELECT 
		  ValoreParametro.CodUA.value('.','VARCHAR(20)') AS CodUA						
		FROM @xParametri.nodes('/Parametri/CodUA') as ValoreParametro(CodUA)
		
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
			INSERT #tmpUAFiglie EXEC MSP_SelUAFiglie @xTmp
			
			FETCH NEXT FROM curUA 
			INTO @sTmpUA
		END		
						
		CREATE INDEX IX_CodUA ON #tmpUAFiglie (CodUA)  
		 		
	END		
	

		
					
	SET @sSQLCIV= ' (SELECT IDCartella
						 FROM 
								T_MovCartelleInVisione CIV
						 WHERE CodRuoloInVisione=''' + @sCodRuolo + '''
							   AND DataInizio <=GetDate() AND DataFine>=GetDate() 
							   AND CodStatoCartellaInVisione=''IC''
					     GROUP BY 	IDCartella	   
						 ) '
						 
	SET @sSQLCIVMia= ' (SELECT IDCartella
						 FROM 
								T_MovCartelleInVisione CIV
						 WHERE CodUtenteInserimento=''' + @sCodLogin + '''							   
							   AND CodStatoCartellaInVisione IN (''IC'',''CM'')
							   AND DataInizio <=GetDate() AND DataFine>=GetDate() 
					     GROUP BY 	IDCartella	   
						 ) '
	
	SET @sSQLPS=' (SELECT IDPaziente
					FROM
					  (SELECT MMP.IDPaziente
					   FROM
					     T_MovPazientiSeguiti MMP									
					   WHERE MMP.CodUtente=''' + @sCodLogin + ''' AND
					   		 MMP.CodRuolo=''' + @sCodRuolo + ''' 
						    AND MMP.CodStatoPazienteSeguito IN (''IC'')
					   UNION
					   SELECT MPA.IDPaziente AS IDPaziente
					   FROM 
					     T_MovPazientiSeguiti MMP
								INNER JOIN T_PazientiAlias MPA
										ON MMP.IDPaziente=MPA.IDPazienteVecchio
						WHERE MMP.CodUtente=''' + @sCodLogin + ''' AND
					   		 MMP.CodRuolo=''' + @sCodRuolo + ''' 
							  AND MMP.CodStatoPazienteSeguito IN (''IC'')
					    ) AS SQ
				   GROUP BY 
				 	 SQ.IDPaziente
				   )'					 						 

		SET @sSQLCNC = '(SELECT 
						CncPaz.ID AS IDPaziente,
						CncPaz.CodStatoConsensoCalcolato AS CodStatoConsensoCalcolato,
						CncPazAlias.ID AS IDPazienteFuso,
						CncPazAlias.CodStatoConsensoCalcolato AS CodStatoConsensoCalcolatoFuso						
				   FROM T_Pazienti CncPaz															
						LEFT JOIN T_PazientiAlias CncAlias
							ON (CncPaz.ID=CncAlias.IDPazienteVecchio)
						LEFT JOIN T_Pazienti AS CncPazAlias
							ON (CncPazAlias.ID=CncAlias.IDPaziente)
					)'
	 
					

		CREATE TABLE #tmpFiltriEpisodi
		(
			IDTrasferimento uniqueidentifier NOT NULL
		)
					 
		IF @bSoloCartelleInVisione=0	
		BEGIN
						SET @sSQL='
					INSERT #tmpFiltriEpisodi(IDTrasferimento) ' 
			SET @sSQL= @sSQL + 
				'SELECT '
					+ CASE 
						WHEN(ISNULL(@nNumRighe,0)=0) THEN ''
						ELSE ' TOP ' + CONVERT(VARCHAR(20),@nNumRighe) 
					END 						
					+ ' T.ID AS IDTrasferimento		
						  
			 FROM T_MovEpisodi M				
					
					INNER JOIN T_MovTrasferimenti T
						ON M.ID=T.IDEpisodio
					
					INNER JOIN T_MovPazienti P
						ON M.ID=P.IDEpisodio		
													 
					LEFT JOIN T_MovCartelle CR
						ON T.IDCartella=CR.ID	
					
					INNER JOIN #tmpUARuolo Tmp		
						ON T.CodUA=Tmp.CodUA
					
										LEFT JOIN 	
						T_Letti LT
							ON 	(M.CodAzi=LT.CodAzi AND
								 T.CodSettore=LT.CodSettore AND
								 T.CodLetto=LT.CodLetto)				
			'		
		END

		IF @bSoloUltimoTrasferimentoCartella=1
	BEGIN
		SET @sSQL=@sSQL + ' INNER JOIN 
								Q_SelUltimoTrasferimentoCartella QU 
									ON (T.IDEpisodio=QU.IDEpisodio AND
										T.IDNum=QU.IDNumTrasferimento AND
										T.IDCartella=QU.IDCartella) '
	END
	
		IF @bSoloUltimoTrasferimentoCartellaNoSospesi=1
	BEGIN
		SET @sSQL=@sSQL + ' INNER JOIN 
								Q_SelUltimoTrasferimentoCartellaNoSospesi QU 
									ON (T.IDEpisodio=QU.IDEpisodio AND
										T.IDNum=QU.IDNumTrasferimento AND
										T.IDCartella=QU.IDCartella) '
	END
	
	IF @bSoloCartelleDaChiudere=1
	BEGIN
		SET @sSQL=@sSQL + ' INNER JOIN 
								T_MovCartelleDaChiudere DC
									ON (T.IDCartella=DC.IDCartella AND
										ISNULL(CR.CodStatoCartella,'''') =''AP'') '
	END

			
			SET @sSQLCartelleInVisione='
			INSERT #tmpFiltriEpisodi(IDTrasferimento) ' 
			
	SET @sSQLCartelleInVisione= @sSQLCartelleInVisione + 
			'SELECT '
				+ CASE 
					WHEN(ISNULL(@nNumRighe,0)=0) THEN ''
					ELSE ' TOP ' + CONVERT(VARCHAR(20),@nNumRighe) 
				END 				
				+ ' T.ID AS IDTrasferimento	
			
			FROM
				T_MovEpisodi M				
				
				INNER JOIN T_MovTrasferimenti T
					ON M.ID=T.IDEpisodio
				
				INNER JOIN T_MovPazienti P
					ON M.ID=P.IDEpisodio									 
				
				LEFT JOIN T_MovCartelle CR
					ON T.IDCartella=CR.ID	
				
				INNER JOIN ' + @sSQLCIV + ' AS CIV
					ON CR.ID=CIV.IDCartella
					
								LEFT JOIN 	
					T_Letti LT
						ON 	(M.CodAzi=LT.CodAzi AND
							 T.CodSettore=LT.CodSettore AND
							 T.CodLetto=LT.CodLetto)
			'			

			IF @bSoloUltimoTrasferimentoCartella=1
	BEGIN
		SET @sSQLCartelleInVisione=@sSQLCartelleInVisione + ' INNER JOIN 
								Q_SelUltimoTrasferimentoCartella QU 
									ON (T.IDEpisodio=QU.IDEpisodio AND
										T.IDNum=QU.IDNumTrasferimento) '
	END

				
	DECLARE @sWhere AS VARCHAR(Max)
	
	SET @sWhere=''
	
		IF ISNULL(@sDataNascita,'') <> ''
	BEGIN
		SET @sWhere= @sWhere + ' AND P.DataNascita = convert(datetime,''' + @sDataNascita + ''',105)'
	END

		IF ISNULL(@sFiltroGenerico,'') <> ''
	BEGIN
				SET @sWhere= @sWhere + ' AND ('
		SET @sWhere= @sWhere + '  P.Cognome like ''%' + @sFiltroGenerico + '%'''
		SET @sWhere= @sWhere + '  OR P.Nome like ''%' + @sFiltroGenerico + '%'''	
		SET @sWhere= @sWhere + '  OR P.Cognome + '' '' + P.Nome like ''%' + @sFiltroGenerico + '%'''	
		SET @sWhere= @sWhere + '  OR P.Nome + '' '' + P.Cognome like ''%' + @sFiltroGenerico + '%'''	
		SET @sWhere= @sWhere + '  OR M.NumeroNosologico like ''%' + @sFiltroGenerico + '%'''					
		SET @sWhere= @sWhere + '  OR M.NumeroNosologico like ''%' + dbo.MF_ModificaNosologico(@sFiltroGenerico) + '%'''					
		SET @sWhere= @sWhere + '  OR M.NumeroListaAttesa like ''%' + @sFiltroGenerico + '%'''	
		
						SET @sWhere= @sWhere + '  OR  (CR.NumeroCartella IS NOT NULL AND CR.NumeroCartella LIKE ''' + dbo.MF_NumeroCartellaDaBarcode(@sFiltroGenerico) + '%'')'

		SET @sWhere= @sWhere + '  OR P.CodiceFiscale like ''%' + @sFiltroGenerico + '%'''
		
					SET @sWhere= @sWhere + '     )' 					
	END
			
						
		IF ISNULL(@sCodUA,'') <> ''
	BEGIN
		SET @sWhere= @sWhere + ' AND T.CodUA IN (SELECT CodUA FROM #tmpUAFiglie)'   
	END
	
	
		IF ISNULL(@sCodTipoEpisodio,'') <> ''
	BEGIN
		SET @sWhere= @sWhere + ' AND M.CodTipoEpisodio IN ('+ @sCodTipoEpisodio + ')'  
	END
	
		IF ISNULL(@sCodStatoTrasferimento,'') <> ''
	BEGIN
		SET @sWhere= @sWhere + ' AND T.CodStatoTrasferimento IN ('+ @sCodStatoTrasferimento + ')' 
	END
	
		IF ISNULL(@sCodStatoEpisodio,'') <> ''
	BEGIN
		SET @sWhere= @sWhere + ' AND M.CodStatoEpisodio IN ('+ @sCodStatoEpisodio + ')' 
	END


		IF ISNULL(@sCodStatoCartella,'') <> ''
	BEGIN
				
		SET @sWhere= @sWhere + ' AND (CR.CodStatoCartella IN ('+ @sCodStatoCartella + ')' 	
		IF (CHARINDEX('DA', @sCodStatoCartella)) > 0
		BEGIN
						SET @sWhere= @sWhere +' OR CR.CodStatoCartella IS NULL'
		END				
		SET @sWhere= @sWhere + ')' 			
	END
		
		IF ISNULL(@sCodStatoEpisodioCartelleAperte,'') <> ''
	BEGIN
		SET @sWhere= @sWhere + ' AND 1=(CASE 
											WHEN CR.CodStatoCartella=''CH'' THEN 1
											WHEN CR.CodStatoCartella=''AP'' AND M.CodStatoEpisodio IN ('+ @sCodStatoEpisodioCartelleAperte + ') THEN 1
											ELSE 0
										END
										)' 
	END
		IF ISNULL(@sCodStatoCartellaInfo,'') <> ''
	BEGIN
		SET @sWhere= @sWhere + ' AND ISNULL(CR.CodStatoCartellaInfo,'''') IN ('+ @sCodStatoCartellaInfo + ')' 
	END
		
		IF @bSoloCartelleDaChiudereInfo=1
	BEGIN
		SET @sSQL=@sSQL + ' AND CR.CodStatoCartellaInfo IS NOT NULL '
	END

		IF ISNULL(@sUnitaOperativa,'') <> ''
	BEGIN
		SET @sWhere= @sWhere + ' AND T.DescrUO  IN ('+ @sUnitaOperativa + ')'
	END
	
		IF ISNULL(@sSettore,'') <> ''
	BEGIN
		SET @sWhere= @sWhere + ' AND T.DescrSettore IN (' + @sSettore + ')' 	
	END
	
		IF ISNULL(@sStanza,'') <> ''
	BEGIN
		SET @sWhere= @sWhere + ' AND ISNULL(T.CodStanza,LT.CodStanza) IN ('+ @sStanza + ')'	 	
	END
	
	
		
	
		IF @dDataUscitaInizioFiltro IS NOT NULL 
		BEGIN					
			SET @sWhere= @sWhere + ' AND T.DataUscita >= CONVERT(datetime,'''  + convert(varchar(20),@dDataUscitaInizioFiltro,120) +''',120)'								
		END

		IF @dDataUscitaFineFiltro IS NOT NULL 
		BEGIN						
			SET @sWhere= @sWhere + ' AND T.DataUscita <= CONVERT(datetime,'''  + convert(varchar(20),@dDataUscitaFineFiltro,120) +''',120)'									
		END				
		
		SET @sWhere= @sWhere + ' AND T.ID NOT IN (SELECT IDTrasferimento FROM  #tmpFiltriEpisodi)'		
				
		IF @bSoloPazientiSeguiti=1
	BEGIN
				SET @sWhere= @sWhere + ' AND P.IDPaziente IN
								  (SELECT M.IDPaziente
								   FROM 
								     T_MovPazientiSeguiti M									
								   WHERE M.CodUtente=''' + @sCodLogin + '''	AND
										 M.CodRuolo	=''' + @sCodRuolo + '''	
										  AND M.CodStatoPazienteSeguito IN (''IC'')										 
								  UNION 		  
								  SELECT PA.IDPaziente AS IDPaziente
								   FROM 
								     T_MovPazientiSeguiti M
										INNER JOIN T_PazientiAlias PA	
											ON M.IDPaziente=PA.IDPazienteVecchio 
								   WHERE M.CodUtente=''' + @sCodLogin + '''	AND
										 M.CodRuolo	=''' + @sCodRuolo + '''							   
										  AND M.CodStatoPazienteSeguito IN (''IC'')										  
								   )' 		  								    
	END
	
		IF ISNULL(@sCodFiltroSpeciale,'') <> ''
	BEGIN
		SET @sSQLFiltro=(SELECT SQL FROM T_FiltriSpeciali WHERE Codice=@sCodFiltroSpeciale)
		IF ISNULL(@sSQLFiltro,'') <> '' 
		BEGIN			
						SET @sWhere= @sWhere + ' AND ' + 	@sSQLFiltro + ''
		END
	END	
		
		IF ISNULL(@uIDTrasferimento,'') <> '' 
	BEGIN
		SET @sWhere= @sWhere + ' AND T.ID=''' + CONVERT(VARCHAR(50),@uIDTrasferimento) + ''''
	END

		SET @sWhere= @sWhere + '  AND T.CodUA IN (SELECT CodUA FROM #tmpUARuolo)'

	
		IF ISNULL(@sWhere,'')<> ''
	BEGIN		
		SET @sSQL=@sSQL +' WHERE ' + RIGHT(@sWhere,len(@sWhere)-5)
		SET @sSQLCartelleInVisione= @sSQLCartelleInVisione + +' WHERE ' + RIGHT(@sWhere,len(@sWhere)-5)
	END	
	
		
	IF @bSoloCartelleInVisione=0 
	BEGIN		
		EXEC (@sSQL)
	END	
		
	EXEC (@sSQLCartelleInVisione)
				
					
		
	CREATE INDEX IX_IDTrasferimento ON #tmpFiltriEpisodi (IDTrasferimento)    
		
	SET @sSQL=''
	
	SET @sSQL='
		SELECT 			
			T.ID AS IDTrasferimento,	
			M.ID AS IDEpisodio,			
			P.IDPaziente,
			P.CodSac,
			P.Cognome,
			P.Nome,
			P.DataNascita,				
			P.Sesso AS Sesso,
			P.ComuneNascita AS ComuneNascita,
			P.CodProvinciaNascita,
			ISNULL(P.Cognome,'''') + '' '' + ISNULL(P.Nome,'''') + '' '' + 
				CASE 
					WHEN ISNULL(P.Sesso,'''')='''' THEN ''''
					ELSE '' ('' + P.Sesso +'') ''
				END +
				CASE 
					WHEN P.DataNascita IS NULL THEN ''''
					ELSE Convert(varchar(10),P.DataNascita,105)
				END +
			
				CASE 
					WHEN ISNULL(P.ComuneNascita,'''')='''' THEN ''''
					ELSE '', '' + P.ComuneNascita
				END +
									
				CASE 
					WHEN ISNULL(P.CodProvinciaNascita,'''')='''' THEN ''''
					ELSE '' ('' + P.CodProvinciaNascita + '')''
				END 
				AS Paziente,
			
			ISNULL(P.Cognome,'''') + '' '' + ISNULL(P.Nome,'''') AS Paziente2,
			
		
			
			CASE 
				WHEN ISNULL(P.Sesso,'''')='''' THEN ''''
				ELSE '' ('' + P.Sesso +'') ''
			END +
			+ '' ''  +  
			CASE 
				WHEN P.DataNascita IS NULL THEN ''''
				ELSE Convert(varchar(10),P.DataNascita,105)
			END +
			+
			CASE 
				WHEN P.DataNascita IS NOT NULL THEN '' ('' + dbo.MF_CalcolaEtaPediatrica(P.DataNascita) + '')''
				ELSE ''''
			END
			+
			CASE 
				WHEN ISNULL(P.ComuneNascita,'''')='''' THEN ''''
				ELSE '', '' + P.ComuneNascita
			END +
								
			CASE 
				WHEN ISNULL(P.CodProvinciaNascita,'''')='''' THEN ''''
				ELSE '' ('' + P.CodProvinciaNascita + '')''
			END 
			AS Paziente3,
					
			  P.CodiceFiscale,	
			  T.CodUA,
			  UA.Descrizione AS DescrUA,
			  ISNULL(T.DescrUO,'''') + '' - '' +   ISNULL(T.DescrSettore,'''') As [UO - Settore],
			  M.DataRicovero,	
			  M.DataDimissione,
			  T.DataIngresso,
			  T.DataUscita,
			  ISNULL(M.DataDimissione,T.DataUscita) AS DataOrdinamentoCartella,				
			  CASE 
				WHEN ISNULL(DataIngresso,getdate())=ISNULL(DataRicovero,getdate()) THEN Convert(varchar(20),DataIngresso,105) + '' '' + Convert(varchar(5),DataIngresso,114) + '' ''
				
				WHEN DataIngresso IS NOT NULL  AND DataRicovero IS NULL 
					THEN Convert(varchar(20),DataIngresso,105) + '' '' + Convert(varchar(5),DataIngresso,114) + '' '' 		
					
				WHEN DataIngresso IS NULL  AND DataRicovero IS NOT NULL 
					THEN Convert(varchar(20),DataRicovero,105) + '' '' + Convert(varchar(5),DataRicovero,114)		
					
				WHEN DataIngresso IS NOT NULL  AND DataRicovero IS NOT NULL 
					THEN Convert(varchar(20),DataIngresso,105) + '' '' + Convert(varchar(5),DataIngresso,114) + '' '' +CHAR(13)+CHAR(10)			
						+ Convert(varchar(20),DataRicovero,105) + '' '' + Convert(varchar(5),DataRicovero,114) + '' '' 				
				END AS [Data Ingresso Data Ricovero],	
				
			  DataIngresso AS DataIngressoGriglia,
			  CASE 
				WHEN ISNULL(DataIngresso,getdate())=ISNULL(DataRicovero,getdate()) THEN NULL
				ELSE DataRicovero 
			  END
			  AS DataRicoveroGriglia,	
			  
			  CASE 
				WHEN ISNULL(DataUscita,getdate())=ISNULL(DataDimissione,getdate()) THEN Convert(varchar(20),DataUscita,105) + '' '' + Convert(varchar(5),DataUscita,114) + '' ''
				WHEN DataUscita IS NOT NULL  AND DataDimissione IS NULL 
					THEN Convert(varchar(20),DataUscita,105) + '' '' + Convert(varchar(5),DataUscita,114) + '' '' 		
				WHEN DataUscita IS NULL  AND DataDimissione IS NOT NULL 
					THEN Convert(varchar(20),DataDimissione,105) + '' '' + Convert(varchar(5),DataDimissione,114)		
				WHEN DataUscita IS NOT NULL  AND DataDimissione IS NOT NULL 
					THEN Convert(varchar(20),DataUscita,105) + '' '' + Convert(varchar(5),DataUscita,114) + '' '' +CHAR(13)+CHAR(10)			
						+ Convert(varchar(20),DataDimissione,105) + '' '' + Convert(varchar(5),DataDimissione,114) + '' '' 				
				END AS [Data Dimissione Data Trasferimento],	
					
			  T.CodStatoTrasferimento,
			  ISNULL(ST.Descrizione,'''') AS DecrStato,
			  ST.Colore AS ColoreStatoTrasferimento,
			  T.CodUO,
			  T.DescrUO,
			  T.CodSettore,
			  T.DescrSettore,
			  ISNULL(T.CodStanza,LT.CodStanza) AS CodStanza,
			  ISNULL(T.DescrStanza,TS.Descrizione) AS DescrStanza,
			  T.CodLetto,
			  T.DescrLetto,
			  ISNULL(T.DescrLetto,'''') +  '' / '' + ISNULL(ISNULL(T.DescrStanza,TS.Descrizione),'''')   AS DescStanzaLetto,
			  M.CodTipoEpisodio,			  
			  M.NumeroNosologico,
			  M.NumeroListaAttesa,
			  ISNULL(M.CodTipoEpisodio,'''') + ''-'' + 
				CASE
					WHEN ISNULL(NumeroNosologico,'''')='''' THEN ISNULL(NumeroListaAttesa,'''')
					ELSE NumeroNosologico
				END AS DescEpisodio,	
			  ISNULL(GAA.Qta,0) AS NumAllergie,
			  ISNULL(GAG.Qta,0) AS NumAlertGenerici,
			  ISNULL(GEC.Qta,0) + ISNULL(GECL.Qta,0) AS NumEvidenzaClinica,							  CASE 
				WHEN CIV.IDCartella IS NULL THEN CONVERT(INTEGER,0)
				ELSE CONVERT(INTEGER,1)
			  END AS FlagCartellaInVisione,	
			  			  
			  CASE 
				WHEN CIVMia.IDCartella IS NULL THEN CONVERT(INTEGER,0)
				ELSE CONVERT(INTEGER,1)
			  END AS FlagHoDatoCartellaInVisione,	
			  ISNULL(CR.CodStatoCartella,''DA'') AS CodStatoCartella,
			  SC.Descrizione As DescrStatoCartella,
			  SC.Colore AS ColoreStatoCartella,
			  ISNULL(CR.CodStatoCartellaInfo,'''') AS CodStatoCartellaInfo,
			  SCI.Descrizione As DescrStatoCartellaInfo,
			  SCI.Colore AS ColoreStatoCartellaInfo,

			  ISNULL(CR.NumeroCartella,'''') + 
				CASE 
					WHEN SC.Codice=''CH'' THEN CHAR(13) + CHAR(10)+ UPPER(ISNULL(SC.Descrizione,''''))
					ELSE ''''
				END AS DescrCartellaGriglia,
			  
			  NULL AS IconaStatoCartella,
			  CR.NumeroCartella,
			  			  T.IDCartella AS IDCartella,
			  CASE
				WHEN PZ.IDPaziente IS NULL THEN CONVERT(INTEGER, 0)
				ELSE CONVERT(INTEGER, 1) 
			  END	
				AS FlagPazienteSeguito,
			CASE
				WHEN ISNULL(CR.CodStatoCartella,'''')=''AP'' AND DC.IDCartella IS NOT NULL THEN CONVERT(INTEGER, 1)
				ELSE CONVERT(INTEGER, 0) 
			  END	
				AS FlagCartellaDaChiudere,
			
			ST.Descrizione +
			CASE				
				WHEN T.CodStatoTrasferimento IN (''TR'',''DM'') THEN 
						 +CHAR(13)+CHAR(10)	+	
						''il '' + Convert(varchar(20),DataUscita,105) + '' '' + Convert(varchar(5),DataUscita,114)						
				ELSE ''''
			END	AS DescrStatoGriglia			
			,CASE
				WHEN CNC.IDPazienteFuso IS NULL THEN ISNULL(CNC.CodStatoConsensoCalcolato,''ND'')
				ELSE  ISNULL(CNC.CodStatoConsensoCalcolatoFuso,''ND'') 
			END AS CodStatoConsensoCalcolato,
			CR.MotivoRiapertura
		FROM T_MovEpisodi M
			INNER JOIN T_MovTrasferimenti T
				ON M.ID=T.IDEpisodio
			INNER JOIN T_MovPazienti P
					ON M.ID=P.IDEpisodio
					
			LEFT JOIN T_MovCartelle CR
					ON T.IDCartella=CR.ID					
			
			LEFT JOIN T_MovCartelleDaChiudere DC
					ON (T.IDCartella=DC.IDCartella)
					
			INNER JOIN #tmpFiltriEpisodi TMP
					ON T.ID=TMP.IDTrasferimento		
			LEFT JOIN T_UnitaAtomiche UA
				ON T.CodUA=UA.Codice		
			LEFT JOIN T_StatoTrasferimento ST
				ON T.CodStatoTrasferimento=ST.Codice	
				
			LEFT JOIN T_StatoCartella SC
				ON CR.CodStatoCartella=SC.Codice		
			
			LEFT JOIN T_StatoCartellaInfo SCI
				ON CR.CodStatoCartellaInfo=SCI.Codice	
						
					LEFT JOIN 
					(SELECT IDPaziente,
							COUNT(ID) AS Qta 
					  FROM 
							T_MovAlertAllergieAnamnesi 
					  WHERE CodStatoAlertAllergiaAnamnesi NOT IN (''AN'',''CA'')
					  GROUP BY 	IDPaziente				  
					 ) AS GAA
				ON P.IDPaziente=GAA.IDPaziente		 

					LEFT JOIN 
					(SELECT IDEpisodio,
							COUNT(ID) AS QTA	
					 FROM 
							T_MovAlertGenerici 	
					 WHERE CodStatoAlertGenerico=''DV''									  GROUP BY 	IDEpisodio
					 ) GAG  				 		
				ON M.ID=GAG.IDEpisodio	
					 
					LEFT JOIN 
					(SELECT IDEpisodio,
							COUNT(ID) AS QTA	
					 FROM 
							T_MovEvidenzaClinica 	
					 WHERE CodStatoEvidenzaClinicaVisione=''DV''										   AND	CodStatoEvidenzaClinica=''CM''									  GROUP BY 	IDEpisodio
					 ) GEC  				 		
				ON M.ID=GEC.IDEpisodio
				
						LEFT JOIN 
					(
					SELECT 
						RE.IDEntitaCollegata AS IDCartellaCollegata,							EVC.IDEpisodio,															COUNT(TC.ID) AS QTA													FROM
						T_MovTrasferimenti TC														INNER JOIN 
																T_MovRelazioniEntita RE
									ON (TC.IDCartella=RE.IDEntita AND
									 	RE.CodEntita=''CAR'' AND
										RE.CodEntitaCollegata=''CAR''
										)
							INNER JOIN 
								T_MovEvidenzaClinica EVC
									ON (TC.IDEpisodio=EVC.IDEpisodio AND
										EVC.CodStatoEvidenzaClinicaVisione=''DV''														AND	EVC.CodStatoEvidenzaClinica=''CM''														)
					 GROUP BY 	
							RE.IDEntitaCollegata,
							EVC.IDEpisodio
						) AS GECL																				
					ON 
						(T.IDCartella=GECL.IDCartellaCollegata)					    			LEFT JOIN 	
					T_Letti LT
						ON 	(M.CodAzi=LT.CodAzi AND
							 T.CodSettore=LT.CodSettore AND
							  T.CodLetto=LT.CodLetto)
			LEFT JOIN	
					T_Stanze TS 
						ON (M.CodAzi=TS.CodAzi AND
							LT.CodStanza=TS.Codice)	
							
						LEFT JOIN ' + @sSQLCIV + ' AS CIV
						ON CR.ID=CIV.IDCartella
						
						LEFT JOIN ' + @sSQLCIVMia + ' AS CIVMia
						ON CR.ID=CIVMia.IDCartella			
						
						LEFT JOIN ' + @sSQLPS + ' AS PZ
						 ON P.IDPaziente =PZ.IDPaziente

						LEFT JOIN ' + @sSQLCNC + ' AS CNC
						 ON P.IDPaziente =CNC.IDPaziente						
		WHERE
			T.CodStatoTrasferimento <> ''CA''
		'	  		

		IF @bDatiEstesi=1
		SET @sSQL = @sSQL + ' AND 1=0 '
	
		IF @sOrdinamento<> ''
		SET @sSQL = @sSQL + ' ORDER BY ' + @sOrdinamento + ''
	
			
		EXEC (@sSQL)

	
	SET @sSQL=''
	
						
		IF @bDatiEstesi=1
	BEGIN	
		SET @sSQL='
		  SELECT 						
			T.CodUO AS Codice,
			MAX(ISNULL(O.Descrizione,T.DescrUO)) AS Descrizione			
		  FROM T_MovTrasferimenti T		
				INNER JOIN #tmpFiltriEpisodi TMP
						ON T.ID=TMP.IDTrasferimento	
				INNER JOIN T_MovEpisodi EPI
					ON T.IDEpisodio =EPI.ID
				LEFT JOIN T_UnitaOperative O
					ON (T.CodUO=o.Codice AND
						EPI.CodAzi = O.CodAzi)
			WHERE				
				LTRIM(ISNULL(T.DescrUO,'''')) <> ''''	
		 GROUP BY T.CodUO
		 ORDER BY MAX(ISNULL(O.Descrizione,T.DescrUO))
		'	   
		EXEC (@sSQL)
	END
	
					
		IF @bDatiEstesi=1
	BEGIN
		SET @sSQL='
		  SELECT 
			T.CodSettore AS Codice,
			T.DescrSettore AS Descrizione						
		  FROM T_MovTrasferimenti T		
				INNER JOIN #tmpFiltriEpisodi TMP
						ON T.ID=TMP.IDTrasferimento			
			WHERE
				LTRIM(ISNULL(T.DescrSettore,'''')) <> ''''
		 GROUP BY CodSettore, DescrSettore	
		 ORDER BY DescrSettore ASC
		'	  
		EXEC (@sSQL)
	END
	
					
		IF @bDatiEstesi=1
	BEGIN
		SET @sSQL='
		  SELECT 
			ISNULL(T.CodStanza,TS.Codice) As Codice,
			ISNULL(T.DescrStanza,TS.Descrizione) As Descrizione			
		  FROM T_MovTrasferimenti T							
				INNER JOIN #tmpFiltriEpisodi TMP
						ON T.ID=TMP.IDTrasferimento	
				INNER JOIN T_MovEpisodi M ON 
					M.ID=T.IDEpisodio
				LEFT JOIN 	
					T_Letti LT
						ON 	(M.CodAzi=LT.CodAzi AND
							 T.CodSettore=LT.CodSettore AND
							  T.CodLetto=LT.CodLetto)
				LEFT JOIN	
						T_Stanze TS 
							ON (M.CodAzi=TS.CodAzi AND
								LT.CodStanza=TS.Codice)				
			WHERE
				ISNULL(ISNULL(T.CodStanza,TS.Codice),'''') <> ''''
		 GROUP BY ISNULL(T.CodStanza,TS.Codice), ISNULL(T.DescrStanza,TS.Descrizione)		
		 ORDER BY ISNULL(T.CodStanza,TS.Codice)
		'	  
	
				
				EXEC (@sSQL)
	END	
	
	IF @bSoloCartelleDaChiudere=1 OR @bSoloCartelleDaChiudereInfo=1
	BEGIN
		SET @sSQL='
		  SELECT 
			COUNT(*) AS CartellaDaChiudureConFirmaDigitale
		  FROM T_MovTrasferimenti T							
				INNER JOIN #tmpFiltriEpisodi TMP
					ON T.ID=TMP.IDTrasferimento	
				INNER JOIN T_MovCartelleDaChiudere DC	
					ON T.IDCartella=DC.IDCartella					
		  WHERE
			T.CodUA IN (SELECT CodUA FROM T_AssUAModuli WHERE CodModulo=''FirmaD_ChCartella'')
				
		'	  
	
				
				EXEC (@sSQL)
	END
	
				
		DROP TABLE #tmpUARuolo			
	DROP TABLE #tmpFiltriEpisodi
								
		
	              
	RETURN 0
	
END