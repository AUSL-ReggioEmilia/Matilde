CREATE PROCEDURE [dbo].[MSP_SelMovAllegati](@xParametri AS XML)
AS
BEGIN
	   	
	   	
				
	DECLARE @uIDPaziente AS UNIQUEIDENTIFIER	
	DECLARE @uIDEpisodio AS UNIQUEIDENTIFIER
	DECLARE @sIDEpisodio AS VARCHAR(50)					DECLARE @uIDTrasferimento AS UNIQUEIDENTIFIER
	DECLARE @uIDTrasferimentoTimeStamp AS UNIQUEIDENTIFIER
	DECLARE @uIDAllegato AS UNIQUEIDENTIFIER
	DECLARE @uIDFolder AS UNIQUEIDENTIFIER
	DECLARE @nNumRighe AS INTEGER
	DECLARE @bDatiEstesi AS Bit	
	DECLARE @bDocumenti AS Bit	
		
	DECLARE @sCodTipoAllegato AS VARCHAR(MAX)		
	DECLARE @sCodStatoAllegato AS VARCHAR(MAX)
	DECLARE @sCodFormatoAllegato AS VARCHAR(MAX)
	DECLARE @sNumeroDocumento AS VARCHAR(50)
	DECLARE @sEntitaAssociata AS VARCHAR(10)
	DECLARE @sFiltroGenerico AS Varchar(500) 
	DECLARE @sCodUA AS VARCHAR(MAX)	

	DECLARE @sCodRuolo AS VARCHAR(20)
	DECLARE @dDataInizio AS DATETIME
	DECLARE @dDataFine AS DATETIME
	
	DECLARE @sDataTmp AS VARCHAR(20)
	
		DECLARE @nTemp AS INTEGER
	DECLARE @bCancella AS BIT
	DECLARE @bModifica AS BIT
	DECLARE @bVisualizza AS BIT
	
		DECLARE @sGUID AS VARCHAR(Max)	
	DECLARE @sSQL AS VARCHAR(MAX)
	DECLARE @sWhere AS VARCHAR(Max)
	DECLARE @sTmp AS VARCHAR(Max)
	DECLARE @gIDSessione AS UNIQUEIDENTIFIER
	DECLARE @nQta AS INTEGER
	DECLARE @bEntitaPAZ AS BIT
	DECLARE @bEntitaCAR AS BIT
	DECLARE @bEntitaALTRE AS BIT
	DECLARE @uIDCartella AS UNIQUEIDENTIFIER
	
	
	
		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDPaziente.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDPaziente') as ValoreParametro(IDPaziente))
	IF 	ISNULL(@sGUID,'') <> '' SET @uIDPaziente=CONVERT(UNIQUEIDENTIFIER,	@sGUID)	
							  
		SET @sIDEpisodio=(SELECT TOP 1 ValoreParametro.IDEpisodio.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDEpisodio') as ValoreParametro(IDEpisodio))
	
	
	IF 	(ISNULL(@sIDEpisodio,'') <> '' AND ISNULL(@sIDEpisodio,'') <> 'NULL')
		BEGIN					
			SET @uIDEpisodio=CONVERT(UNIQUEIDENTIFIER,	@sIDEpisodio)	
		END			
		
		
		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDTrasferimento.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDTrasferimento') as ValoreParametro(IDTrasferimento))
	IF 	ISNULL(@sGUID,'') <> '' SET @uIDTrasferimento=CONVERT(UNIQUEIDENTIFIER,	@sGUID)		

		SET @sGUID =(SELECT TOP 1 ValoreParametro.IDTrasferimentoTimeStamp.value('.','VARCHAR(50)')
									 FROM @xParametri.nodes('/Parametri/TimeStamp/IDTrasferimento') as ValoreParametro(IDTrasferimentoTimeStamp))
	IF 	ISNULL(@sGUID,'') <> '' SET @uIDTrasferimentoTimeStamp=CONVERT(UNIQUEIDENTIFIER,	@sGUID)	
	
		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDAllegato.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDAllegato') as ValoreParametro(IDAllegato))	
	IF 	ISNULL(@sGUID,'') <> '' SET @uIDAllegato=CONVERT(UNIQUEIDENTIFIER, @sGUID)	


		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDFolder.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDFolder') as ValoreParametro(IDFolder))	
	IF 	ISNULL(@sGUID,'') <> '' SET @uIDFolder=CONVERT(UNIQUEIDENTIFIER, @sGUID)	
							  				  
		SET @nNumRighe=(SELECT	TOP 1 ValoreParametro.NumRighe.value('.','INTEGER')
								FROM @xParametri.nodes('/Parametri/NumRighe') as ValoreParametro(NumRighe))	
	SET @nNumRighe=ISNULL(@nNumRighe,0)
		
		SET @bDatiEstesi=(SELECT TOP 1 ValoreParametro.DatiEstesi.value('.','bit')
					  FROM @xParametri.nodes('/Parametri/DatiEstesi') as ValoreParametro(DatiEstesi))
	SET @bDatiEstesi=ISNULL(@bDatiEstesi,0)

		SET @bDocumenti=(SELECT TOP 1 ValoreParametro.Documenti.value('.','bit')
					  FROM @xParametri.nodes('/Parametri/Documenti') as ValoreParametro(Documenti))
	SET @bDocumenti=ISNULL(@bDocumenti,0)
	
		SET @sCodTipoAllegato=''
	SELECT	@sCodTipoAllegato =  @sCodTipoAllegato +
														CASE 
								WHEN @sCodTipoAllegato='' THEN ''
								ELSE  ','
							END + ''''
						  + ValoreParametro.CodTipoAllegato.value('.','VARCHAR(1800)')
						  + ''''
						 FROM @xParametri.nodes('/Parametri/CodTipoAllegato') as ValoreParametro(CodTipoAllegato)
						 
	SET @sCodTipoAllegato=LTRIM(RTRIM(@sCodTipoAllegato))
	IF	@sCodTipoAllegato='''''' SET @sCodTipoAllegato=''
	SET @sCodTipoAllegato=UPPER(@sCodTipoAllegato)
	
		SET @sCodFormatoAllegato	=''
	SELECT	@sCodFormatoAllegato	 =  @sCodFormatoAllegato	 +
														CASE 
								WHEN @sCodFormatoAllegato	='' THEN ''
								ELSE  ','
							END + ''''
						  + ValoreParametro.CodFormatoAllegato.value('.','VARCHAR(1800)')
						  + ''''
						 FROM @xParametri.nodes('/Parametri/CodFormatoAllegato') as ValoreParametro(CodFormatoAllegato)
						 
	SET @sCodFormatoAllegato	=LTRIM(RTRIM(@sCodFormatoAllegato))
	IF	@sCodFormatoAllegato	='''''' SET @sCodFormatoAllegato=''
	SET @sCodFormatoAllegato	=UPPER(@sCodFormatoAllegato	)
	
		SET @sCodStatoAllegato=''
	SELECT	@sCodStatoAllegato =  @sCodStatoAllegato +
														CASE 
								WHEN @sCodStatoAllegato='' THEN ''
								ELSE  ','
							END + ''''
						  + ValoreParametro.CodStatoAllegato.value('.','VARCHAR(1800)')
						  + ''''
						 FROM @xParametri.nodes('/Parametri/CodStatoAllegato') as ValoreParametro(CodStatoAllegato)
						 
	SET @sCodStatoAllegato=LTRIM(RTRIM(@sCodStatoAllegato))
	IF	@sCodStatoAllegato='''''' SET @sCodStatoAllegato=''
	SET @sCodStatoAllegato=UPPER(@sCodStatoAllegato)
	
		SET @sNumeroDocumento=(SELECT	TOP 1 ValoreParametro.NumeroDocumento.value('.','VARCHAR(50)')
								FROM @xParametri.nodes('/Parametri/NumeroDocumento') as ValoreParametro(NumeroDocumento))	
	SET @sNumeroDocumento=ISNULL(@sNumeroDocumento,'')		
	
	
		SET @sDataTmp=(SELECT TOP 1 ValoreParametro.DataInizio.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/DataInizio') as ValoreParametro(DataInizio))					  
	SET @sDataTmp=ISNULL(@sDataTmp,'')
	IF @sDataTmp<> '' 
		BEGIN
						SET @sDataTmp= SUBSTRING(@sDataTmp,4,2) + '-' 
						+ LEFT(@sDataTmp,2) + '-' +
						+ SUBSTRING(@sDataTmp,7,4) +
						' ' + RIGHT(@sDataTmp,5)
			IF ISDATE(@sDataTmp)=1
				SET	@dDataInizio=CONVERT(DATETIME,@sDataTmp,120)						
			ELSE
				SET	@dDataInizio =NULL			
		END
		
		
		SET @sDataTmp=(SELECT TOP 1 ValoreParametro.DataFine.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/DataFine') as ValoreParametro(DataFine))
	SET @sDataTmp=ISNULL(@sDataTmp,'')
	IF @sDataTmp<> '' 
		BEGIN
						SET @sDataTmp= SUBSTRING(@sDataTmp,4,2) + '-' 
						+ LEFT(@sDataTmp,2) + '-' +
						+ SUBSTRING(@sDataTmp,7,4) +
						' ' + RIGHT(@sDataTmp,5)
			IF ISDATE(@sDataTmp)=1
				SET	@dDataFine=CONVERT(DATETIME,@sDataTmp,120)						
			ELSE
				SET	@dDataFine =NULL		
		END 

		SET @sCodRuolo=(SELECT TOP 1 ValoreParametro.CodRuolo.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/TimeStamp/CodRuolo') as ValoreParametro(CodRuolo))
	SET @sCodRuolo=ISNULL(@sCodRuolo,'')

		SET @sFiltroGenerico=(SELECT	TOP 1 ValoreParametro.FiltroGenerico.value('.','VARCHAR(500)')
						 FROM @xParametri.nodes('/Parametri/FiltroGenerico') as ValoreParametro(FiltroGenerico))	
	
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

			
	CREATE TABLE #tmpEntitaAssociata(CodEntita VARCHAR(10))

		SET @sEntitaAssociata= (SELECT TOP 1
							 ValoreParametro.EntitaAssociata.value('.','VARCHAR(10)')						  
							FROM @xParametri.nodes('/Parametri/EntitaAssociata[1]') as ValoreParametro(EntitaAssociata)
							)
						 
	IF @sEntitaAssociata IS NOT NULL
		INSERT INTO #tmpEntitaAssociata(CodEntita) VALUES (@sEntitaAssociata)

		SET @sEntitaAssociata= (SELECT TOP 1
							 ValoreParametro.EntitaAssociata.value('.','VARCHAR(10)')						  
							FROM @xParametri.nodes('/Parametri/EntitaAssociata[2]') as ValoreParametro(EntitaAssociata)
							)
						 
	IF @sEntitaAssociata IS NOT NULL
		INSERT INTO #tmpEntitaAssociata(CodEntita) VALUES (@sEntitaAssociata)

		SET @sEntitaAssociata= (SELECT TOP 1
							 ValoreParametro.EntitaAssociata.value('.','VARCHAR(10)')						  
							FROM @xParametri.nodes('/Parametri/EntitaAssociata[3]') as ValoreParametro(EntitaAssociata)
							)
						 
	IF @sEntitaAssociata IS NOT NULL
		INSERT INTO #tmpEntitaAssociata(CodEntita) VALUES (@sEntitaAssociata)

	
	    DECLARE @xTimeStamp AS XML	

    SET @xTimeStamp=(SELECT TOP 1 ValoreParametro.TS.query('.')
					  FROM @xParametri.nodes('/Parametri/TimeStamp') as ValoreParametro(TS))
	
	SET @xTimeStamp=CONVERT(XML,
						'<Parametri>' + CONVERT(varchar(max),@xTimeStamp) +
						'</Parametri>')
						
												
				
				
	DECLARE @xTmp AS XML
			
	CREATE TABLE #tmpUARuolo
	(
		CodUA VARCHAR(20) COLLATE Latin1_General_CI_AS,
		Descrizione VARCHAR(255) COLLATE Latin1_General_CI_AS
	)


	SET  @xTmp=CONVERT(XML,'<Parametri><CodRuolo>'+ @sCodRuolo + '</CodRuolo></Parametri>')
	
		INSERT #tmpUARuolo EXEC MSP_SelUADaRuolo @xTmp
	
	CREATE INDEX IX_CodUA ON #tmpUARuolo (CodUA)    
		
		
		SET @nTemp=(SELECT COUNT(*) FROM T_AssRuoliModuli
				WHERE CodModulo='Allegati_Cancella'	
				AND CodRuolo=@sCodRuolo)
	IF @nTemp>=1 
		SET @bCancella=1
	ELSE
		SET @bCancella=0	
		
		SET @nTemp=(SELECT COUNT(*) FROM T_AssRuoliModuli
				WHERE CodModulo='Allegati_Modifica'	
				AND CodRuolo=@sCodRuolo)
	IF @nTemp>=1 
		SET @bModifica=1
	ELSE
		SET @bModifica=0	
					
		SET @nTemp=(SELECT COUNT(*) FROM T_AssRuoliModuli
				WHERE CodModulo='Allegati_Visualizza'	
				AND CodRuolo=@sCodRuolo)
	IF @nTemp>=1 
		SET @bVisualizza=1
	ELSE
		SET @bVisualizza=0	
					
					
				
			
		SET @gIDSessione=NEWID()
	
	SET @sSQL='		INSERT INTO T_TmpFiltriAllegati(IDSessione,IDAllegato)	
					SELECT '
					
		IF @nNumRighe<> 0  SET @sSQL= @sSQL + ' TOP ' + convert(varchar(10),@nNumRighe)+  ' '
	
	SET @sSQL=@sSQL +				
					'	'''  + convert(varchar(50),@gIDSessione) + ''' AS IDSessione,	'
							+ ' M.ID AS IDAllegato
					FROM 
						T_MovAllegati	M	
							LEFT JOIN T_MovPazienti P
								ON (M.IDEpisodio=P.IDEpisodio)
							INNER JOIN #tmpUARuolo U
								ON M.CodUA=U.CodUA														 
					'
	
	
				
	SET @sWhere=''		
	
		IF @uIDPaziente IS NOT NULL
		BEGIN
			SET @sTmp= ' AND (M.IDPaziente=''' + CONVERT(VARCHAR(50),@uIDPaziente) + '''
							OR
														M.IDPaziente IN 
									(SELECT IDPazienteVecchio
										FROM T_PazientiAlias
										WHERE 
										IDPaziente IN 
											(SELECT IDPaziente
												FROM T_PazientiAlias
												WHERE IDPazienteVecchio=''' + CONVERT(VARCHAR(50),@uIDPaziente) + '''
											)
									)
						)'	

			SET @sWhere= @sWhere + @sTmp								
		END			

		IF @sIDEpisodio='NULL'
		BEGIN
			SET @sTmp= ' AND M.IDEpisodio IS NULL'
			SET @sWhere= @sWhere + @sTmp								
		END	
	ELSE
				IF @uIDEpisodio IS NOT NULL 
			BEGIN
				SET @sTmp= ' AND M.IDEpisodio=''' + convert(varchar(50),@uIDEpisodio) +''''
				SET @sWhere= @sWhere + @sTmp								
			END	
	
		IF @uIDTrasferimento IS NOT NULL
		BEGIN
			SET @sTmp= ' AND M.IDTrasferimento=''' + convert(varchar(50), @uIDTrasferimento) +''''
			SET @sWhere= @sWhere + @sTmp								
		END		
		
		IF @uIDAllegato IS NOT NULL
		BEGIN
			SET @sTmp= ' AND M.ID=''' + convert(varchar(50),@uIDAllegato) +''''
			SET @sWhere= @sWhere + @sTmp								
		END		

		IF @uIDFolder IS NOT NULL
		BEGIN
			SET @sTmp= ' AND M.IDFolder=''' + convert(varchar(50),@uIDFolder) +''''
			SET @sWhere= @sWhere + @sTmp								
		END
				
		IF @sCodTipoAllegato NOT IN ('')
		BEGIN				
			SET @sTmp=  ' AND 			
							 M.CodTipoAllegato IN ('+ @sCodTipoAllegato + ')
						'  				
			SET @sWhere= @sWhere + @sTmp														
		END
		
	
		IF @sCodFormatoAllegato	 NOT IN ('')
		BEGIN				
			SET @sTmp=  ' AND 			
							 M.CodFormatoAllegato IN ('+  @sCodFormatoAllegato + ')
						'  				
			SET @sWhere= @sWhere + @sTmp														
		END
	
		 IF ISNULL(@sCodUA,'') <> ''
		BEGIN
			SET @sTmp=  ' AND 			
							M.CodUA IN (' +@sCodUA + ')
						 '  				
			SET @sWhere= @sWhere + @sTmp
		END
			
		IF @sCodStatoAllegato NOT IN ('','''Tutti''')
		BEGIN	
						SET @sTmp=  ' AND 			
							 M.CodStatoAllegato IN ('+ @sCodStatoAllegato + ')
						'  				
			SET @sWhere= @sWhere + @sTmp														
		END
	ELSE
		BEGIN
			IF @sCodTipoAllegato NOT IN ('''Tutti''')
				BEGIN
										SET @sTmp=  ' AND 			
									 M.CodStatoAllegato NOT IN (''AN'',''CA'')
								'  				
					SET @sWhere= @sWhere + @sTmp
				END	
			ELSE
				BEGIN
															SET @sTmp=  ' AND 			
									 M.CodStatoAllegato <> ''CA''
								'  				
					SET @sWhere= @sWhere + @sTmp
				END	
		END							

		SET @nQta=(SELECT COUNT(*) FROM #tmpEntitaAssociata)

	IF @nQta > 0
		BEGIN	
			
			SET  @bEntitaPAZ =0
			SET  @bEntitaCAR =0
			SET  @bEntitaALTRE =0

			SET @nQta=(SELECT COUNT(*) FROM #tmpEntitaAssociata WHERE CodEntita='PAZ')
			IF @nQta > 0 			
				SET  @bEntitaPAZ =1

			SET @nQta=(SELECT COUNT(*) FROM #tmpEntitaAssociata WHERE CodEntita='CAR')
			IF @nQta > 0 
				SET  @bEntitaCAR =1

			SET @nQta=(SELECT COUNT(*) FROM #tmpEntitaAssociata WHERE CodEntita='ALTRE')
			IF @nQta > 0 
				SET  @bEntitaALTRE =1

			
						SET @uIDCartella=(SELECT IDCartella FROM T_MovTrasferimenti WHERE ID=@uIDTrasferimentoTimeStamp)

						
						IF (@bEntitaPAZ=1 AND @bEntitaCAR=0 AND @bEntitaALTRE=0)
			BEGIN				
				SET @sTmp=  ' AND 			
								M.CodEntita IN (''PAZ'')
							'  				
				SET @sWhere= @sWhere + @sTmp
			END
			
			IF (@bEntitaPAZ=1 AND (@bEntitaCAR=1 OR @bEntitaALTRE=1))
			BEGIN				
				SET @sTmp=  ' AND 	
								 M.CodEntita IN (''PAZ'',''CAR'') 							  
							'  				
				SET @sWhere= @sWhere + @sTmp
			END
		
			IF (@bEntitaPAZ=0 AND (@bEntitaCAR=1 OR @bEntitaALTRE=1))
			BEGIN				
				SET @sTmp=  ' AND 	
								 M.CodEntita IN (''CAR'') 							  
							'  				
				SET @sWhere= @sWhere + @sTmp
			END

						IF (@bEntitaPAZ=0 AND @bEntitaCAR=1 AND @bEntitaALTRE=0)
			BEGIN
				SET @sTmp=  ' AND 	
								M.IDTrasferimento IN (SELECT ID FROM T_MovTrasferimenti WHERE IDCartella=''' + CONVERT(VARCHAR(50),@uIDCartella) + ''')
							'  				
				SET @sWhere= @sWhere + @sTmp				
			END

			IF (@bEntitaPAZ=0 AND @bEntitaCAR=0 AND @bEntitaALTRE=1)
			BEGIN
				SET @sTmp=  ' AND 	
								M.IDTrasferimento NOT IN (SELECT ID FROM T_MovTrasferimenti WHERE IDCartella=''' + CONVERT(VARCHAR(50),@uIDCartella) + ''')
							'  				
				SET @sWhere= @sWhere + @sTmp				
			END

			IF (@bEntitaPAZ=1 AND @bEntitaCAR=1 AND @bEntitaALTRE=0)
			BEGIN
				SET @sTmp=  ' AND 	
								(M.IDTrasferimento IN (SELECT ID FROM T_MovTrasferimenti WHERE IDCartella=''' + CONVERT(VARCHAR(50),@uIDCartella) + ''')
								 OR M.IDTrasferimento IS NULL)

							'  				
				SET @sWhere= @sWhere + @sTmp				
			END

			
			IF (@bEntitaPAZ=1 AND @bEntitaCAR=0 AND @bEntitaALTRE=1)
			BEGIN
				SET @sTmp=  ' AND 	
								(M.IDTrasferimento NOT IN (SELECT ID FROM T_MovTrasferimenti WHERE IDCartella=''' + CONVERT(VARCHAR(50),@uIDCartella) + ''')
								 OR M.IDTrasferimento IS NULL)
							'  				
				SET @sWhere= @sWhere + @sTmp				
			END
			
					END


		IF @sNumeroDocumento <> ''
		BEGIN	
						SET @sTmp=  ' AND 			
							 M.NumeroDocumento LIKE ''%'+ @sNumeroDocumento + '''
						'  				
			SET @sWhere= @sWhere + @sTmp														
		END
	
		IF ISNULL(@sFiltroGenerico,'') <> ''
	BEGIN
		
		SET @sTmp= ' AND ('
		SET @sTmp= @sTmp + '  M.NumeroDocumento LIKE ''%'+ @sFiltroGenerico + ''' '
		SET @sTmp= @sTmp + '  OR M.TestoTXTBreve LIKE ''%'+ @sFiltroGenerico + '%'' '		
				
					SET @sTmp= @sTmp + '     )' 					
	
		SET @sWhere= @sWhere + @sTmp		
	END
	
		IF @dDataInizio IS NOT NULL 
		BEGIN
			SET @sTmp= ' AND M.DataEvento >= CONVERT(datetime,'''  + convert(varchar(20),@dDataInizio,120) +''',120)'	
																																	SET @sWhere= @sWhere + @sTmp								
		END

		IF @dDataFine IS NOT NULL 
		BEGIN
			SET @sTmp= ' AND M.DataEvento <= CONVERT(datetime,'''  + convert(varchar(20),@dDataFine,120) +''',120)'							
			SET @sWhere= @sWhere + @sTmp								
		END
				
					
		IF ISNULL(@sWhere,'')<> ''
	BEGIN	
		SET @sSQL=@sSQL +' WHERE ' + RIGHT(@sWhere,len(@sWhere)-5)
	END	
		
	EXEC (@sSQL)	

	
	SET @sSQL='	SELECT	'
	
		
	SET @sSQL=@sSQL + ' 
						M.ID,
						M.IDPaziente,
						M.IDEpisodio,
						M.IDTrasferimento,
						I.Icona,
						I.IDIcona,
						IFA.Icona AS IconaFormato,
						IFA.IDICona AS IDIconaFormato,
						M.NumeroDocumento,
						M.DataEvento,
						M.DataEventoUTC,						
						M.TestoRTF,
						M.NotaRTF,
						M.CodFormatoAllegato,
						F.Descrizione AS DescrFormatoAllegato,
						M.CodTipoAllegato,
						T.Descrizione AS DescrTipoAllegato,
						M.CodStatoAllegato,
						S.Descrizione AS DescrStatoAllegato,
												M.CodUA,
						UA.Descrizione AS UA,
						M.CodEntita,
						M.IDFolder,
						dbo.MF_GetPathMovFolder(M.IDFolder) AS Folder,
						'
					    
					    
				IF @bDocumenti=1
			SET @sSQL=@sSQL + ' M.Documento, '
		ELSE
			SET @sSQL=@sSQL + ' NULL AS Documento, '
			
		SET @sSQL=@sSQL + '						   
					    NomeFile,
					    Estensione,
						TestoTXT,
						NotaTXT,						
					    M.CodUtenteRilevazione,
					    L.Descrizione AS DescrUtenteRilevazione,
					    M.CodUtenteUltimaModifica,
					    LMOD.Descrizione AS DescrUtenteModifica,
					    M.DataRilevazione,
					    M.DataRilevazioneUTC,
					    M.DataUltimaModifica,
					    M.DataUltimaModificaUTC,
						DF.ID AS IDDocumentoFirmato,
						CASE
							WHEN DF.ID IS NULL THEN NULL
							ELSE ''Firmato da '' + ISNULL(LDF.Cognome,'''') + '' '' + ISNULL(LDF.Nome,'''') + '' il '' +
								CASE 
									WHEN DF.DataInserimento IS NULL THEN ''''
									ELSE CONVERT(VARCHAR(20),DF.DataInserimento,105) + '' '' + CONVERT(VARCHAR(5),DF.DataInserimento,14) 
								END
						END AS InfoFirmaDigitale,
				        CONVERT(INTEGER, + ' + CONVERT(CHAR(1), @bCancella) + '
						&
						CASE 
							WHEN ISNULL(CAR.CodStatoCartella,'''') =''CH'' THEN 0
							ELSE 1
						END
						
						) AS PermessoCancella,
				        CONVERT(INTEGER, + ' + CONVERT(CHAR(1), @bModifica) + '
							&
						CASE 
							WHEN ISNULL(CAR.CodStatoCartella,'''') =''CH'' THEN 0
							ELSE 1
						END
						) AS PermessoModifica,
				        CONVERT(INTEGER, + ' 
								+ CONVERT(CHAR(1), @bVisualizza) + '								
								&
									CASE 
										WHEN CodFormatoAllegato =''E'' THEN 1 
										ELSE 0
									END	
								
						) AS PermessoVisualizza'
								
								
						
	
		SET @sSQL=@sSQL + ' 						
				FROM
					T_TmpFiltriAllegati TMP
						INNER JOIN
							T_MovAllegati	M 
							ON (TMP.IDAllegato=M.ID)					
						LEFT JOIN T_Pazienti P
							ON (M.IDPaziente=P.ID)
							
						LEFT JOIN T_TipoAllegato T
							ON (M.CodTipoAllegato=T.Codice)						
							
						LEFT JOIN T_StatoAllegato S
							ON (M.CodStatoAllegato=S.Codice)
					
						LEFT JOIN T_FormatoAllegati F
							ON (M.CodFormatoAllegato=F.Codice)
							
						LEFT JOIN T_Login L
							ON (M.CodUtenteRilevazione=L.Codice)	
						LEFT JOIN T_Login LMOD
							ON (M.CodUtenteUltimaModifica=LMod.Codice)	
						
						LEFT JOIN T_MovTrasferimenti TRA
							ON (M.IDTrasferimento=TRA.ID)

						LEFT JOIN T_MovCartelle CAR
							ON (TRA.IDCartella=CAR.ID)

						LEFT JOIN T_UnitaAtomiche UA
							ON (M.CodUA=UA.Codice)

						LEFT JOIN T_MovDocumentiFirmati DF
								ON (M.ID=DF.IDEntita AND
									DF.CodEntita=''ALL'')

						LEFT JOIN T_Login LDF
								ON (DF.CodUtenteInserimento = LDF.Codice  )
							
					  '
		SET @sSQL=@sSQL + ' 							
						LEFT JOIN 
							(SELECT 
							  IDNum AS IDIcona,
							  CodTipo,
							  CodStato,							 
							  							  CONVERT(varbinary(max),null)  As Icona
							 FROM T_Icone
							 WHERE CodEntita=''ALL''
							) AS I
						ON (M.CodTipoAllegato=I.CodTipo AND 	
							M.CodEntita=I.CodStato
							)	
							
						LEFT JOIN 
							(SELECT 
							  IDNum AS IDIcona,
							  CodTipo,
							  CodStato,							 
							  							  CONVERT(varbinary(max),null)  As Icona
							 FROM T_Icone
							 WHERE CodEntita=''ALLFMT''
							) AS IFA
						ON (M.CodFormatoAllegato=IFA.CodTipo)								
					'				
	
	SET @sSQL=@sSQL + ' WHERE TMP.IDSessione=''' + convert(varchar(50),@gIDSessione) +''''

	SET @sSQL=@sSQL +' ORDER BY M.DataEvento DESC ' 	
	PRINT @sSQL
 	
	EXEC (@sSQL)
	
					
	IF @bDatiEstesi=1
		BEGIN
			SET @sSQL='SELECT DISTINCT
							M.CodTipoAllegato AS CodTipo,
							T.Descrizione AS DescTipo
						FROM 
							T_TmpFiltriAllegati TMP
								INNER JOIN T_MovAllegati	M	
										ON (TMP.IDAllegato=M.ID)																													
								LEFT JOIN T_TipoAllegato T
									ON (M.CodTipoAllegato=T.Codice)													
						'
			
						SET @sSQL=@sSQL + ' WHERE TMP.IDSessione=''' + convert(varchar(50),@gIDSessione) +''''
							
			EXEC (@sSQL)	
		END 			
	
					
	IF @bDatiEstesi=1
		BEGIN
			SET @sSQL='SELECT DISTINCT
							M.CodUA AS CodUA,
							UA.Descrizione AS DescUA
						FROM 
							T_TmpFiltriAllegati TMP
								INNER JOIN T_MovAllegati	M	
										ON (TMP.IDAllegato=M.ID)
								INNER JOIN T_UnitaAtomiche AS UA
										ON (M.CodUA = UA.Codice)
																			
						'
			
						SET @sSQL=@sSQL + ' WHERE TMP.IDSessione=''' + convert(varchar(50),@gIDSessione) +''''
			SET @sSQL=@sSQL + ' ORDER BY UA.Descrizione '				
			EXEC (@sSQL)	
		END 	
				
	EXEC MSP_InsMovTimeStamp @xTimeStamp	

					
	DROP TABLE #tmpEntitaAssociata

		DELETE FROM T_TmpFiltriAllegati
	WHERE IDSessione=@gIDSessione 
								
				
END