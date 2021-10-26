CREATE PROCEDURE [dbo].[MSP_CercaCartelleInVisione](@xParametri AS XML)
AS
BEGIN
	

				
	DECLARE @sFiltroGenerico AS Varchar(500) 
	DECLARE @sDataNascita AS VARCHAR(10)
	DECLARE @bDatiEstesi AS Bit	
	DECLARE @sCodRuolo AS Varchar(20)
	DECLARE @nNumRighe AS INTEGER
	
	DECLARE @sCodUA AS Varchar(MAX)
	DECLARE @sCodUO AS Varchar(MAX)
	DECLARE @sCodStatoTrasferimento AS Varchar(MAX)

	DECLARE @sCodUAFiltro AS Varchar(MAX)
	DECLARE @sCodStatoCartella AS Varchar(MAX)		
	DECLARE @sCodLogin AS VARCHAR(100)
	
	DECLARE @dDataUscitaInizioFiltro AS DATETIME
	DECLARE @dDataUscitaFineFiltro AS DATETIME
	
	DECLARE @bSoloCartelleInVisione AS BIT	
	DECLARE @uIDTrasferimento AS VARCHAR(50)	

		
		DECLARE @sSQL AS VARCHAR(MAX)
	DECLARE @sSQLCartelleInVisione AS VARCHAR(MAX)	
	DECLARE @sSQLCIV AS VARCHAR(MAX)
	DECLARE @sSQLCIVMia AS VARCHAR(MAX)
	DECLARE @sSQLPS AS VARCHAR(MAX)
	DECLARE @sDataTmp AS VARCHAR(20)
	DECLARE @sSQLFiltro AS VARCHAR(MAX)	
				
		SET @sFiltroGenerico=(SELECT	TOP 1 ValoreParametro.FiltroGenerico.value('.','VARCHAR(500)')
						 FROM @xParametri.nodes('/Parametri/FiltroGenerico') as ValoreParametro(FiltroGenerico))						 
	SET @sFiltroGenerico= ISNULL(@sFiltroGenerico,'')
	SET @sFiltroGenerico=LTRIM(RTRIM(@sFiltroGenerico))
	
		SET @sFiltroGenerico=REPLACE(@sFiltroGenerico,'''','''''')						
	
		SET @sDataNascita=(SELECT	TOP 1 ValoreParametro.DataNascita.value('.','VARCHAR(10)')
						 FROM @xParametri.nodes('/Parametri/DataNascita') as ValoreParametro(DataNascita))						 
	SET @sDataNascita= ISNULL(@sDataNascita,'')
	SET @sDataNascita=LTRIM(RTRIM(@sDataNascita))
		
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
	
		SET @bSoloCartelleInVisione=(SELECT TOP 1 ValoreParametro.SoloCartelleInVisione.value('.','bit')
					  FROM @xParametri.nodes('/Parametri/SoloCartelleInVisione') as ValoreParametro(SoloCartelleInVisione))											
	SET @bSoloCartelleInVisione=ISNULL(@bSoloCartelleInVisione,0)
		
		SET @sDataTmp=(SELECT TOP 1 ValoreParametro.DataUscitaInizio.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/DataUscitaInizio') as ValoreParametro(DataUscitaInizio))					  
	SET @sDataTmp=ISNULL(@sDataTmp,'')
	IF @sDataTmp<> '' 
		BEGIN
						SET @sDataTmp= SUBSTRING(@sDataTmp,4,2) + '-' 
						+ LEFT(@sDataTmp,2) + '-' +
						+ SUBSTRING(@sDataTmp,7,4) +
						' ' + RIGHT(@sDataTmp,5)
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
						+ SUBSTRING(@sDataTmp,7,4) +
						' ' + RIGHT(@sDataTmp,5)
			IF ISDATE(@sDataTmp)=1
				SET	@dDataUscitaFineFiltro=CONVERT(DATETIME,@sDataTmp,120)						
			ELSE
				SET	@dDataUscitaFineFiltro =NULL		
		END 
	
		SET @sCodUO =''
	SELECT	@sCodUO  =  @sCodUO  +
														CASE 
								WHEN @sCodUO ='' THEN ''
								ELSE  ','
							END + ''''
						  + ValoreParametro.CodUO.value('.','VARCHAR(1800)')
						  + ''''
						 FROM @xParametri.nodes('/Parametri/CodUO') as ValoreParametro(CodUO)
	SET @sCodUO=LTRIM(RTRIM(@sCodUO))
	IF	@sCodUO='''''' SET @sCodUO=''	

		SET @sCodStatoTrasferimento =''
	SELECT	@sCodStatoTrasferimento  =  @sCodStatoTrasferimento  +
														CASE 
								WHEN @sCodStatoTrasferimento ='' THEN ''
								ELSE  ','
							END + ''''
						  + ValoreParametro.CodStatoTrasferimento.value('.','VARCHAR(1800)')
						  + ''''
						 FROM @xParametri.nodes('/Parametri/CodStatoTrasferimento') as ValoreParametro(CodStatoTrasferimento)
	SET @sCodStatoTrasferimento=LTRIM(RTRIM(@sCodStatoTrasferimento))
	IF	@sCodStatoTrasferimento='''''' SET @sCodStatoTrasferimento=''
	

		SET @sCodLogin=(SELECT	TOP 1 ValoreParametro.CodLogin.value('.','VARCHAR(100)')
						 FROM @xParametri.nodes('/Parametri/TimeStamp/CodLogin') as ValoreParametro(CodLogin))						 
	SET @sCodLogin= LTRIM(RTRIM(ISNULL(@sCodLogin,'')))		
					  
	SET @uIDTrasferimento=(SELECT TOP 1 ValoreParametro.IDTrasferimento.value('.','UNIQUEIDENTIFIER')
					  FROM @xParametri.nodes('/Parametri/IDTrasferimento') as ValoreParametro(IDTrasferimento))
	

				
				
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

	
		CREATE TABLE #tmpFiltriEpisodi
		(
			IDTrasferimento uniqueidentifier NOT NULL
		)
					 
										
												
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

								INNER JOIN 
								Q_SelUltimoTrasferimentoCartella QU 
									ON (T.IDEpisodio=QU.IDEpisodio AND
										T.IDNum=QU.IDNumTrasferimento)
			'			

	
				
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
		SET @sWhere= @sWhere + '  OR ISNULL(CR.NumeroCartella,'''') like ''' + dbo.MF_NumeroCartellaDaBarcode(@sFiltroGenerico) + '%'''			
		SET @sWhere= @sWhere + '  OR P.CodiceFiscale like ''%' + @sFiltroGenerico + '%'''
		
					SET @sWhere= @sWhere + '     )' 					
	END			
	
		IF ISNULL(@sCodUA,'') <> ''
	BEGIN
		SET @sWhere= @sWhere + ' AND T.CodUA IN (SELECT CodUA FROM #tmpUAFiglie)'   
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
					
		
		IF ISNULL(@uIDTrasferimento,'') <> '' 
	BEGIN
		SET @sWhere= @sWhere + ' AND T.ID=''' + CONVERT(VARCHAR(50),@uIDTrasferimento) + ''''
	END

		IF ISNULL(@sCodUO,'') <> ''
	BEGIN
		SET @sWhere= @sWhere + ' AND T.CodUO IN (' + @sCodUO + ')'   
	END			
	
		IF ISNULL(@sCodStatoTrasferimento,'') <> ''
	BEGIN
		SET @sWhere= @sWhere + ' AND T.CodStatoTrasferimento IN (' + @sCodStatoTrasferimento + ')'   
	END		

		IF ISNULL(@sWhere,'')<> ''
	BEGIN		
		SET @sSQL=@sSQL +' WHERE ' + RIGHT(@sWhere,len(@sWhere)-5)
		SET @sSQLCartelleInVisione= @sSQLCartelleInVisione + +' WHERE ' + RIGHT(@sWhere,len(@sWhere)-5)
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
			ISNULL(P.Cognome,'''') + '' '' + ISNULL(P.Nome,'''') + '' '' + 
				CASE 
					WHEN ISNULL(Sesso,'''')='''' THEN ''''
					ELSE '' ('' + Sesso +'') ''
				END +
				CASE 
					WHEN DataNascita IS NULL THEN ''''
					ELSE Convert(varchar(10),DataNascita,105)
				END +
			
				CASE 
					WHEN ISNULL(ComuneNascita,'''')='''' THEN ''''
					ELSE '', '' + ComuneNascita
				END +
									
				CASE 
					WHEN ISNULL(CodProvinciaNascita,'''')='''' THEN ''''
					ELSE '' ('' + CodProvinciaNascita + '')''
				END 
				AS Paziente,
			
			ISNULL(P.Cognome,'''') + '' '' + ISNULL(P.Nome,'''') AS Paziente2,
			
		
			
			CASE 
				WHEN ISNULL(Sesso,'''')='''' THEN ''''
				ELSE '' ('' + Sesso +'') ''
			END +
			+ '' ''  +  
			CASE 
				WHEN DataNascita IS NULL THEN ''''
				ELSE Convert(varchar(10),DataNascita,105)
			END +
		
			CASE 
				WHEN ISNULL(ComuneNascita,'''')='''' THEN ''''
				ELSE '', '' + ComuneNascita
			END +
								
			CASE 
				WHEN ISNULL(CodProvinciaNascita,'''')='''' THEN ''''
				ELSE '' ('' + CodProvinciaNascita + '')''
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
			  
			  ISNULL(CR.NumeroCartella,'''') + 
				CASE 
					WHEN SC.Codice=''CH'' THEN CHAR(13) + CHAR(10)+ UPPER(ISNULL(SC.Descrizione,''''))
					ELSE ''''
				END AS DescrCartellaGriglia,
			  
			  NULL AS IconaStatoCartella,
			  CR.NumeroCartella,
			  CR.ID AS IDCartella,			  
			
			ST.Descrizione +
			CASE				
				WHEN T.CodStatoTrasferimento IN (''TR'',''DM'') AND DataUscita IS NOT NULL THEN 
						 +CHAR(13)+CHAR(10)	+	
						''il '' + Convert(varchar(20),DataUscita,105) + '' '' + Convert(varchar(5),DataUscita,114)						
				ELSE ''''
			END	AS DescrStatoGriglia
				
		FROM T_MovEpisodi M
			INNER JOIN T_MovTrasferimenti T
				ON M.ID=T.IDEpisodio
			INNER JOIN T_MovPazienti P
					ON M.ID=P.IDEpisodio
					
			LEFT JOIN T_MovCartelle CR
					ON T.IDCartella=CR.ID					
					
			INNER JOIN #tmpFiltriEpisodi TMP
					ON T.ID=TMP.IDTrasferimento		
			LEFT JOIN T_UnitaAtomiche UA
				ON T.CodUA=UA.Codice		
			LEFT JOIN T_StatoTrasferimento ST
				ON T.CodStatoTrasferimento=ST.Codice	
				
			LEFT JOIN T_StatoCartella SC
				ON CR.CodStatoCartella=SC.Codice				
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
									
		WHERE
			T.CodStatoTrasferimento <> ''CA''
		'	  		
		IF @bDatiEstesi=1
		SET @sSQL = @sSQL + ' AND 1=0 '
	
		
			
		EXEC (@sSQL)

	
	SET @sSQL=''
	
	
				
		DROP TABLE #tmpUARuolo			
	DROP TABLE #tmpFiltriEpisodi
								
		
	RETURN 0
	
END