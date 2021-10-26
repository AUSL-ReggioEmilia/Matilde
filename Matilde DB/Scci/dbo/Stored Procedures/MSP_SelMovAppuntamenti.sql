CREATE PROCEDURE [dbo].[MSP_SelMovAppuntamenti](@xParametri AS XML)
AS

BEGIN
					
	DECLARE @uIDPaziente AS UNIQUEIDENTIFIER	
	DECLARE @uIDEpisodio AS UNIQUEIDENTIFIER
	DECLARE @sIDEpisodio AS VARCHAR(50)				
	DECLARE @uIDTrasferimento AS UNIQUEIDENTIFIER
	DECLARE @uIDAppuntamento AS UNIQUEIDENTIFIER
	DECLARE @nNumRighe AS INTEGER
	DECLARE @bDatiEstesi AS Bit		
	DECLARE @sCodStatoAppuntamento AS VARCHAR(1800)
	DECLARE @sCodAgenda AS VARCHAR(1800)
	DECLARE @sCodTipoAppuntamento AS VARCHAR(1800)
	
	DECLARE @sCodUtente AS VARCHAR(1800)
	DECLARE @sCodLogin AS VARCHAR(1800)
	DECLARE @sCodRuolo AS VARCHAR(20)
	DECLARE @dDataInizio AS DATETIME
	DECLARE @dDataFine AS DATETIME
	DECLARE @bVisualizzazioneSintetica AS BIT
	DECLARE @sSoloEpisodio AS CHAR(1)
	DECLARE @sIgnoraFiltroCartella AS CHAR(1)
	DECLARE @bAppuntamentiTrasversali AS BIT
	DECLARE @bEscludiListe AS BIT	
	
	DECLARE @sCodSistema as VARCHAR(20)
	DECLARE @sIDSistema AS VARCHAR(1800)
	DECLARE @sIDGruppo AS VARCHAR(50)
	DECLARE @sDataTmp AS VARCHAR(20)
	
	DECLARE @sFiltroGenerico AS Varchar(500) 
	DECLARE @bCaricaCancellati AS BIT
	DECLARE @bOrdinaDataAsc AS BIT 
	
	DECLARE @gIDSessione AS UNIQUEIDENTIFIER
	DECLARE @sGUID AS VARCHAR(Max)
	DECLARE @sSQL AS VARCHAR(MAX)
	DECLARE @sWhere AS VARCHAR(Max)
	DECLARE @sTmp AS VARCHAR(Max)			
	DECLARE @sOrderBy AS VARCHAR(MAX)
	DECLARE @sSQLQueryAgendeEPI AS VARCHAR(MAX)
	

	DECLARE @nTemp AS INTEGER
	DECLARE @bInserisci AS BIT
	DECLARE @bModifica AS BIT
	DECLARE @bCancella AS BIT
		
	SET @sGUID=(SELECT TOP 1 ValoreParametro.IDPaziente.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDPaziente') as ValoreParametro(IDPaziente))
	IF 	ISNULL(@sGUID,'') <> '' SET @uIDPaziente=CONVERT(UNIQUEIDENTIFIER,	@sGUID)	
					
	SET @sIDEpisodio=(SELECT TOP 1 ValoreParametro.IDEpisodio.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/TimeStamp/IDEpisodio') as ValoreParametro(IDEpisodio))
					  
	IF 	ISNULL(@sIDEpisodio,'') <> '' AND ISNULL(@sIDEpisodio,'') <> 'NULL' 
		BEGIN			
			SET @uIDEpisodio=CONVERT(UNIQUEIDENTIFIER,	@sIDEpisodio)	
		END	
				
	SET @sGUID=(SELECT TOP 1 ValoreParametro.IDTrasferimento.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/TimeStamp/IDTrasferimento') as ValoreParametro(IDTrasferimento))
	IF 	ISNULL(@sGUID,'') <> '' SET @uIDTrasferimento=CONVERT(UNIQUEIDENTIFIER,	@sGUID)
					  				  	
	SET @sGUID=(SELECT TOP 1 ValoreParametro.IDAppuntamento.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDAppuntamento') as ValoreParametro(IDAppuntamento))		
	IF 	ISNULL(@sGUID,'') <> '' SET @uIDAppuntamento=CONVERT(UNIQUEIDENTIFIER,	@sGUID)					  			  
		
	SET @nNumRighe=(SELECT	TOP 1 ValoreParametro.NumRighe.value('.','INTEGER')
								FROM @xParametri.nodes('/Parametri/NumRighe') as ValoreParametro(NumRighe))	
	SET @nNumRighe=ISNULL(@nNumRighe,0)
			
	SET @bDatiEstesi=(SELECT TOP 1 ValoreParametro.DatiEstesi.value('.','bit')
					  FROM @xParametri.nodes('/Parametri/DatiEstesi') as ValoreParametro(DatiEstesi))
	SET @bDatiEstesi=ISNULL(@bDatiEstesi,0)
	
	SET @bAppuntamentiTrasversali=(SELECT TOP 1 ValoreParametro.AppuntamentiTrasversali.value('.','bit')
					  FROM @xParametri.nodes('/Parametri/AppuntamentiTrasversali') as ValoreParametro(AppuntamentiTrasversali))
	SET @bAppuntamentiTrasversali=ISNULL(@bAppuntamentiTrasversali,0)	

	SET  @bEscludiListe=(SELECT TOP 1 ValoreParametro.EscludiListe.value('.','bit')
					  FROM @xParametri.nodes('/Parametri/EscludiListe') as ValoreParametro(EscludiListe))
	SET  @bEscludiListe=ISNULL( @bEscludiListe,0)	

	SET @sCodStatoAppuntamento=''
	SELECT	@sCodStatoAppuntamento =  @sCodStatoAppuntamento +							
							CASE 
								WHEN @sCodStatoAppuntamento='' THEN ''
								ELSE  ','
							END + ''''
						  + ValoreParametro.CodStatoAppuntamento.value('.','VARCHAR(1800)')
						  + ''''
						 FROM @xParametri.nodes('/Parametri/CodStatoAppuntamento') as ValoreParametro(CodStatoAppuntamento)
						 
	SET @sCodStatoAppuntamento=LTRIM(RTRIM(@sCodStatoAppuntamento))
	IF	@sCodStatoAppuntamento='''''' SET @sCodStatoAppuntamento=''
	SET @sCodStatoAppuntamento=UPPER(@sCodStatoAppuntamento)
		
	SET @sCodTipoAppuntamento=''
	SELECT	@sCodTipoAppuntamento =  @sCodTipoAppuntamento +	
							CASE 
								WHEN @sCodTipoAppuntamento='' THEN ''
								ELSE  ','
							END + ''''
						  + ValoreParametro.CodTipoAppuntamento.value('.','VARCHAR(1800)')
						  + ''''
						 FROM @xParametri.nodes('/Parametri/CodTipoAppuntamento') as ValoreParametro(CodTipoAppuntamento)
						 
	SET @sCodTipoAppuntamento=LTRIM(RTRIM(@sCodTipoAppuntamento))
	IF	@sCodTipoAppuntamento='''''' SET @sCodTipoAppuntamento=''
	SET @sCodTipoAppuntamento=UPPER(@sCodTipoAppuntamento)
	
	SET @sCodAgenda=''
	SELECT	@sCodAgenda =  @sCodAgenda +
							CASE 
								WHEN @sCodAgenda='' THEN ''
								ELSE  ','
							END + ''''
						  + ValoreParametro.CodAgenda.value('.','VARCHAR(1800)')
						  + ''''
						 FROM @xParametri.nodes('/Parametri/CodAgenda') as ValoreParametro(CodAgenda)
						 
	SET @sCodAgenda=LTRIM(RTRIM(@sCodAgenda))
	IF	@sCodAgenda='''''' SET @sCodAgenda=''
	SET @sCodAgenda=UPPER(@sCodAgenda)
					
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
			 
	SET @sCodUtente=''
	SELECT	@sCodUtente =  @sCodUtente +		
							CASE 
								WHEN @sCodUtente='' THEN ''
								ELSE  ','
							END + ''''
						  + ValoreParametro.CodUtente.value('.','VARCHAR(1800)')
						  + ''''
						 FROM @xParametri.nodes('/Parametri/CodUtente') as ValoreParametro(CodUtente)
						 
	SET @sCodUtente=LTRIM(RTRIM(@sCodUtente))
	IF	@sCodUtente='''''' SET @sCodUtente=''
	SET @sCodUtente=UPPER(@sCodUtente)
	
	SET @sCodRuolo=(SELECT TOP 1 ValoreParametro.CodRuolo.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/TimeStamp/CodRuolo') as ValoreParametro(CodRuolo))
	SET @sCodRuolo=ISNULL(@sCodRuolo,'')
	
	SET @bVisualizzazioneSintetica=(SELECT TOP 1 ValoreParametro.VisualizzazioneSintetica.value('.','bit')
					  FROM @xParametri.nodes('/Parametri/VisualizzazioneSintetica') as ValoreParametro(VisualizzazioneSintetica))											
	SET @bVisualizzazioneSintetica=ISNULL(@bVisualizzazioneSintetica,0)
	
	SET @sSoloEpisodio=(SELECT TOP 1 ValoreParametro.SoloEpisodio.value('.','CHAR(1)')
					  FROM @xParametri.nodes('/Parametri/SoloEpisodio') as ValoreParametro(SoloEpisodio))											
	SET @sSoloEpisodio=ISNULL(@sSoloEpisodio,'X')			

	SET @sIgnoraFiltroCartella=(SELECT TOP 1 ValoreParametro.IgnoraFiltroCartella.value('.','CHAR(1)')
					  FROM @xParametri.nodes('/Parametri/IgnoraFiltroCartella') as ValoreParametro(IgnoraFiltroCartella))											
	SET @sIgnoraFiltroCartella=ISNULL(@sIgnoraFiltroCartella,'X')	
	
	SET @sCodSistema=(SELECT	TOP 1 ValoreParametro.CodSistema.value('.','VARCHAR(20)')
						 FROM @xParametri.nodes('/Parametri/CodSistema') as ValoreParametro(CodSistema))						 

	SET @sIDSistema=''
	SELECT	@sIDSistema =  @sIDSistema +
							CASE 
								WHEN @sIDSistema='' THEN ''
								ELSE  ','
							END + ''''
						  + ValoreParametro.IDSistema.value('.','VARCHAR(1800)')
						  + ''''
						 FROM @xParametri.nodes('/Parametri/IDSistema') as ValoreParametro(IDSistema)
						 
	SET @sIDSistema=LTRIM(RTRIM(@sIDSistema))
	IF	@sIDSistema='''''' SET @sIDSistema=''
	SET @sIDSistema=UPPER(@sIDSistema)				 
		
	SET @sIDGruppo=(SELECT	TOP 1 ValoreParametro.IDGruppo.value('.','VARCHAR(50)')
						 FROM @xParametri.nodes('/Parametri/IDGruppo') as ValoreParametro(IDGruppo))						 
	
	SET @sFiltroGenerico=(SELECT	TOP 1 ValoreParametro.FiltroGenerico.value('.','VARCHAR(500)')
						 FROM @xParametri.nodes('/Parametri/FiltroGenerico') as ValoreParametro(FiltroGenerico))						 
	SET @sFiltroGenerico= ISNULL(@sFiltroGenerico,'')
	SET @sFiltroGenerico=LTRIM(RTRIM(@sFiltroGenerico))
	
	SET @sFiltroGenerico=REPLACE(@sFiltroGenerico,'''','''''')	
	
	SET @sCodLogin=(SELECT TOP 1 ValoreParametro.CodLogin.value('.','VARCHAR(100)')
					  FROM @xParametri.nodes('/Parametri/TimeStamp/CodLogin') as ValoreParametro(CodLogin))	
	SET @sCodLogin=ISNULL(@sCodLogin,'')

	SET @bCaricaCancellati=(SELECT TOP 1 ValoreParametro.CaricaCancellati.value('.','bit')
					  FROM @xParametri.nodes('/Parametri/CaricaCancellati') as ValoreParametro(CaricaCancellati))
	SET @bCaricaCancellati=ISNULL(@bCaricaCancellati,0)

	SET @bOrdinaDataAsc=(SELECT TOP 1 ValoreParametro.OrdinaDataAsc.value('.','bit')
					  FROM @xParametri.nodes('/Parametri/OrdinaDataAsc') as ValoreParametro(OrdinaDataAsc))
	SET @bOrdinaDataAsc=ISNULL(@bOrdinaDataAsc,0)
		 	
    DECLARE @xTimeStamp AS XML	

    SET @xTimeStamp=(SELECT TOP 1 ValoreParametro.TS.query('.')
					  FROM @xParametri.nodes('/Parametri/TimeStamp') as ValoreParametro(TS))
	
	SET @xTimeStamp=CONVERT(XML,
						'<Parametri>' + CONVERT(varchar(max),@xTimeStamp) +
						'</Parametri>')
							
	CREATE TABLE #tmpPazienti
	(
		IDPaziente UNIQUEIDENTIFIER
	)
			
	
	CREATE TABLE #tmpUARuolo
	(
		CodUA VARCHAR(20) COLLATE Latin1_General_CI_AS,
		Descrizione VARCHAR(255) COLLATE Latin1_General_CI_AS
	)

	DECLARE @xTmp AS XML
	SET  @xTmp=CONVERT(XML,'<Parametri><CodRuolo>'+ @sCodRuolo + '</CodRuolo></Parametri>')
	
	INSERT #tmpUARuolo EXEC MSP_SelUADaRuolo @xTmp
	
	CREATE INDEX IX_CodUA ON #tmpUARuolo (CodUA)    
			
	SET @gIDSessione=NEWID()
	
	SET @sSQL='		INSERT INTO T_TmpFiltriAppuntamenti(IDSessione,IDAppuntamento)
					
					SELECT  '
	
	IF @nNumRighe<> 0  SET @sSQL= @sSQL + ' TOP ' + convert(varchar(10),@nNumRighe)+  ' ' 					
	
	SET @sSQL=@sSQL +				
					'	'''  + convert(varchar(50),@gIDSessione) + ''' AS IDSessione,	'
							+ ' M.ID AS IDAppuntamento
					
					FROM 
						T_MovAppuntamenti	M	
							LEFT JOIN T_Pazienti P
								ON (M.IDPaziente=P.ID)	
								
							INNER JOIN 	T_MovAppuntamentiAgende AGE
								ON M.ID=AGE.IDAppuntamento
							
							LEFT JOIN 	T_Agende ANAAGE
								ON AGE.CodAgenda = ANAAGE.Codice

							-- Associazione agende ad UA	
							INNER JOIN 
									(SELECT 
											CodUA,
											CodVoce AS CodAgenda
									 FROM T_AssUAEntita
									 WHERE CodEntita=''AGE''
									 ) AS RA
							ON RA.CodAgenda=AGE.CodAgenda 	
							
							-- UA delle agende in join con le UA del ruolo
							INNER JOIN #tmpUARuolo TMPUA
								ON 	TMPUA.CodUA=RA.CodUA															  							 
					'						
	
	SET @sWhere=''
						
	IF @uIDPaziente IS NOT NULL
		BEGIN			
			INSERT INTO #tmpPazienti(IDPaziente)
			VALUES (@uIDPaziente)
			
			INSERT INTO #tmpPazienti(IDPaziente)
				SELECT IDPazienteVecchio
				 FROM T_PazientiAlias
				 WHERE 
					IDPaziente IN 
						(SELECT IDPaziente
						 FROM T_PazientiAlias
						 WHERE IDPazienteVecchio=@uIDPaziente
						)
			
			CREATE INDEX IX_IDPaziente ON #tmpPazienti (IDPaziente)
				
			SET @sTmp= ' AND P.ID IN (SELECT IDPaziente FROM #tmpPazienti) '			
			SET @sWhere= @sWhere + @sTmp								
		END			


	IF @sIDEpisodio='NULL'
		BEGIN
			SET @sTmp= ''
			SET @sWhere= @sWhere + @sTmp								
		END	
	ELSE		
		IF @uIDEpisodio IS NOT NULL AND (@sSoloEpisodio='1')
					
			BEGIN
				SET @sTmp= ' AND M.IDEpisodio=''' + convert(varchar(50),@uIDEpisodio) +''''
				SET @sWhere= @sWhere + @sTmp								
			END	
		
	IF @uIDTrasferimento IS NOT NULL  AND @sSoloEpisodio='1' AND @sIgnoraFiltroCartella <> '1'
		BEGIN
			SET @sTmp= ' AND M.IDTrasferimento IN (SELECT ID FROM T_MovTrasferimenti 
												  WHERE IDCartella=(SELECT TOP 1 IDCartella FROM T_MovTrasferimenti WHERE ID=''' + CONVERT(VARCHAR(50),@uIDTrasferimento) + '''))'
			SET @sWhere= @sWhere + @sTmp								
		END

	IF @uIDAppuntamento IS NOT NULL
		BEGIN
			SET @sTmp= ' AND M.ID=''' + convert(varchar(50),@uIDAppuntamento) +''''
			SET @sWhere= @sWhere + @sTmp								
		END		
					
	IF @sCodTipoAppuntamento NOT IN ('')
		BEGIN				
			SET @sTmp=  ' AND 			
							 M.CodTipoAppuntamento IN ('+ @sCodTipoAppuntamento + ')
						'  				
			SET @sWhere= @sWhere + @sTmp														
		END	
			
	IF @sCodStatoAppuntamento NOT IN ('')
		BEGIN			
			SET @sTmp=  ' AND 			
							 M.CodStatoAppuntamento IN ('+ @sCodStatoAppuntamento + ')
						'  				
			SET @sWhere= @sWhere + @sTmp														
		END
			
	IF 	@sCodAgenda NOT IN ('')
		BEGIN			
			SET @sTmp=  ' AND 			
							 RA.CodAgenda IN ('+ @sCodAgenda + ')
						'  				
			SET @sWhere= @sWhere + @sTmp														
		END																		
		
	IF @sCodUtente NOT IN ('')
		BEGIN		
			SET @sTmp=  ' AND 			
							 M.CodUtenteRilevazione IN ('+ @sCodUtente + ')
						'  				
			SET @sWhere= @sWhere + @sTmp														
		END			
				
	IF @dDataInizio IS NOT NULL 
		BEGIN
			SET @sTmp= CASE 
							WHEN @dDataFine IS NULL 
									THEN ' AND M.DataInizio = CONVERT(datetime,'''  + convert(varchar(20),@dDataInizio,120) +''',120)'									
							ELSE ' AND M.DataInizio >= CONVERT(datetime,'''  + convert(varchar(20),@dDataInizio,120) +''',120)'	
						END	
			SET @sWhere= @sWhere + @sTmp								
		END
	
	IF @dDataFine IS NOT NULL 
		BEGIN
			SET @sTmp= ' AND M.DataFine <= CONVERT(datetime,'''  + convert(varchar(20),@dDataFine,120) +''',120)'							
			SET @sWhere= @sWhere + @sTmp								
		END
		
	IF ISNULL(@sCodSistema, '') <> ''
		BEGIN	
			SET @sTmp=  ' AND M.CodSistema = ''' + @sCodSistema + ''''  				
			SET @sWhere = @sWhere + @sTmp														
		END
	
	IF  @sIDSistema NOT IN ('','''Tutti''')
		BEGIN	
			SET @sTmp=  ' AND 			
							 M. IDSistema IN ('+  @sIDSistema + ')							 
						'  				
			SET @sWhere= @sWhere + @sTmp														
		END	
		
	IF ISNULL(@sIDGruppo, '') <> ''
		BEGIN		
			SET @sTmp=  ' AND M.IDGruppo = ''' + @sIDGruppo + ''''  				
			SET @sWhere = @sWhere + @sTmp														
		END
			
	IF @bEscludiListe = 1
	BEGIN		

		SET @sTmp=  ' AND ISNULL(ANAAGE.Lista,0)=0' 
		SET @sWhere = @sWhere + @sTmp			
	END

	
	if (@bCaricaCancellati = 0)
	BEGIN 		
		SET @sTmp=  ' AND 			
						 M.CodStatoAppuntamento <> ''CA''				  
					'  							
		IF ISNULL(@sTmp,'') <> '' 		
			SET @sWhere= @sWhere + @sTmp		
	END			
				
	IF ISNULL(@sFiltroGenerico,'') <> ''
	BEGIN		
		SET @sWhere= @sWhere + ' AND ('
		SET @sWhere= @sWhere + '  P.Cognome like ''%' + @sFiltroGenerico + '%'''
		SET @sWhere= @sWhere + '  OR P.Nome like ''%' + @sFiltroGenerico + '%'''	
		SET @sWhere= @sWhere + '  OR P.Cognome + '' '' + P.Nome like ''%' + @sFiltroGenerico + '%'''	
		SET @sWhere= @sWhere + '  OR P.Nome + '' '' + P.Cognome like ''%' + @sFiltroGenerico + '%'''	

		IF ISNUMERIC(@sFiltroGenerico) =1
			SET @sWhere= @sWhere + '  OR M.IDNum =' + @sFiltroGenerico + ''	
					
		SET @sWhere= @sWhere + '     )' 					
	END
				
	IF ISNULL(@sWhere,'')<> ''
	BEGIN				
		SET @sSQL=@sSQL +' WHERE ' + RIGHT(@sWhere,len(@sWhere)-5)		
	END	
	
	SET @sSQL=@sSQL + '  GROUP BY M.ID,M.DataInizio'
	
	
	IF @bOrdinaDataAsc IS NULL
	BEGIN	
		IF @bVisualizzazioneSintetica=1	
			BEGIN
				SET @sOrderBy = '  ORDER BY M.DataInizio ASC'
			END
		ELSE					
			BEGIN
				SET @sOrderBy = '  ORDER BY M.DataInizio DESC'				
			END
	END
	ELSE
		BEGIN		
			IF @bOrdinaDataAsc = 1
				BEGIN
					SET @sOrderBy = '  ORDER BY M.DataInizio ASC'
				END
			ELSE
				BEGIN
					SET @sOrderBy = '  ORDER BY M.DataInizio DESC'
				END			
		END

	SET @sSQL=@sSQL + @sOrderBy	
 	
	EXEC (@sSQL)

	SET @nTemp=(SELECT COUNT(*) FROM T_AssRuoliModuli
				WHERE CodModulo='App_Inserisci'	
				AND CodRuolo=@sCodRuolo)
	IF @nTemp>=1 
		SET @bInserisci=1
	ELSE
		SET @bInserisci=0
			
	SET @nTemp=(SELECT COUNT(*) FROM T_AssRuoliModuli
				WHERE CodModulo='App_Modifica'	
				AND CodRuolo=@sCodRuolo)
	IF @nTemp>=1 
		SET @bModifica=1
	ELSE
		SET @bModifica=0
						
	SET @nTemp=(SELECT COUNT(*) FROM T_AssRuoliModuli
				WHERE CodModulo='App_Cancella'	
				AND CodRuolo=@sCodRuolo)
	IF @nTemp>=1 
		SET @bCancella=1
	ELSE
		SET @bCancella=0   
   
	
	SET @sSQLQueryAgendeEPI=(SELECT dbo.MF_SQLQueryAgendePAZ())	
	
	IF ISNULL(@bVisualizzazioneSintetica,0)=0
		BEGIN
						
		SET @sSQL='	SELECT 
						M.ID,
						M.IDPaziente,
						M.IDEpisodio,
						M.IDTrasferimento,
						M.CodUA,
						CASE 
							WHEN M.CodStatoAppuntamento=''DP'' THEN ''..'' 
							WHEN CONVERT(varchar(10),DataInizio,120) = CONVERT(varchar(10),DataFine,120) THEN
								''Dalle '' + CONVERT(varchar(10),DataInizio,105) + '' '' +CONVERT(varchar(5),DataInizio,14) + char(10) + char(13) + ''alle '' + CONVERT(varchar(5),DataFine,14)																																			  
						ELSE
								''Dalle '' + CONVERT(varchar(10),DataInizio,105) + '' '' +CONVERT(varchar(5),DataInizio,14) + char(10) + char(13) + ''alle '' +  CONVERT(varchar(10),DataFine,105) + '' '' + CONVERT(varchar(5),DataFine,14)						 
						END AS DescrData,
						M.DataInizio,
						M.DataFine,						
						M.ElencoRisorse,	
						QEPI.Oggetto,	
						M.CodTipoAppuntamento AS CodTipoAppuntamento,
						T.Descrizione AS DescrTipoAppuntamento,		
						ISNULL(T.TimeSlotInterval,0) AS TimeSlotInterval,		
						M.CodStatoAppuntamento AS CodStatoAppuntamento,
						S.Descrizione As DescrStatoAppuntamento,
														
						--M.CodUtenteRilevazione AS CodUtente,
						--L.Descrizione AS DescrUtente,
						--M.DataInizio,
						--M.DataFine, 						
						MS.IDScheda,
						MS.CodScheda,
						MS.Versione,						
						MS.AnteprimaRTF,
						MS.AnteprimaTXT,
						MS.CodScheda,
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
									
							--CASE 
							--	WHEN ISNULL(P.CodProvinciaNascita,'''')='''' THEN ''''
							--	ELSE '' '' + P.CodProvinciaNascita + '')''
							--END +
							CASE 
								WHEN ISNULL(CAR.NumeroCartella,'''')='''' THEN ''''
								ELSE '' ['' + CAR.NumeroCartella + '']''
							END +
							CASE 
								WHEN M.IDEpisodio IS NOT NULL AND ISNULL(EPI.NumeroNosologico,'''') <> ''''  THEN '' - '' + EPI.NumeroNosologico 
								WHEN M.IDEpisodio IS NOT NULL AND ISNULL(EPI.NumeroListaAttesa,'''') <> ''''  THEN '' - '' + EPI.NumeroListaAttesa 
								ELSE ''''
							END 
						AS Paziente2,
							
						CONVERT(INTEGER,' + CONVERT(CHAR(10),@bModifica) + ')
						& 
								CASE 
									WHEN CodStatoAppuntamento IN (''PR'',''IC'',''DP'') THEN 1 
									ELSE 0
								END	
						&
								CASE 
									WHEN ' + CONVERT(VARCHAR(1),@bAppuntamentiTrasversali) + ' = 1 THEN 1	-- Percorso Trasversale
									WHEN M.IDEpisodio IS NULL AND ''X''=''' + @sSoloEpisodio + ''' THEN 1	-- Ambulatoriale
									WHEN M.IDEpisodio IS NULL AND ''0''=''' + @sSoloEpisodio + ''' THEN 0	-- Ricoverato TUTTI -> Nessun Permesso
									WHEN M.IDEpisodio IS NOT NULL AND ' +
										CASE 
											WHEN @uIDEpisodio IS NULL THEN ' 1=1 THEN 0						-- IDEpisodio passato NULL -> Nessun Permesso '				
											ELSE 'M.IDEpisodio=''' + CONVERT(VARCHAR(50),@uIDEpisodio) + ''' THEN 1 -- IDEpisodio valorizzato e uguale a quello pasasto->  '
										END											
									+ '
									ELSE 0
								END		
						AS PermessoModifica,														
						
						CONVERT(bit,'+ CONVERT(CHAR(10),@bCancella) + ')
							& 
								CASE 
									WHEN CodStatoAppuntamento  IN (''PR'',''DP'') THEN 1 
									ELSE 0
								END	
							& 
								CASE 
										WHEN ' + CONVERT(VARCHAR(1),@bAppuntamentiTrasversali) + ' = 1 THEN 1	-- Percorso Trasversale
										WHEN M.IDEpisodio IS NULL AND ''X''=''' + @sSoloEpisodio + ''' THEN 1	-- Ambulatoriale
										WHEN M.IDEpisodio IS NULL AND ''0''=''' + @sSoloEpisodio + ''' THEN 0	-- Ricoverato TUTTI -> Nessun Permesso
										WHEN M.IDEpisodio IS NOT NULL AND ' +
											CASE 
												WHEN @uIDEpisodio IS NULL THEN ' 1=1 THEN 0						-- IDEpisodio passato NULL -> Nessun Permesso '				
												ELSE 'M.IDEpisodio=''' + CONVERT(VARCHAR(50),@uIDEpisodio) + ''' THEN 1 -- IDEpisodio valorizzato e uguale a quello pasasto->  '
											END											
										+ '
										ELSE 0
								END			
						AS PermessoCancella,

						CONVERT(bit,'+ CONVERT(CHAR(10),@bModifica) + ')
							& 
								CASE 
									WHEN CodStatoAppuntamento IN (''PR'',''DP'') THEN 1 
									ELSE 0
								END	
							& 
								CASE 
										WHEN ' + CONVERT(VARCHAR(1),@bAppuntamentiTrasversali) + ' = 1 THEN 1	-- Percorso Trasversale
										WHEN M.IDEpisodio IS NULL AND ''X''=''' + @sSoloEpisodio + ''' THEN 1	-- Ambulatoriale
										WHEN M.IDEpisodio IS NULL AND ''0''=''' + @sSoloEpisodio + ''' THEN 0	-- Ricoverato TUTTI -> Nessun Permesso
										WHEN M.IDEpisodio IS NOT NULL AND ' + 
											CASE 
												WHEN @uIDEpisodio IS NULL THEN ' 1=1 THEN 0						-- IDEpisodio passato NULL -> Nessun Permesso '				
												ELSE 'M.IDEpisodio=''' + CONVERT(VARCHAR(50),@uIDEpisodio) + ''' THEN 1 -- IDEpisodio valorizzato e uguale a quello pasasto->  '
											END											
										+ ' ELSE 0
								END			
						AS PermessoAnnulla,

						CONVERT(bit,'+ CONVERT(CHAR(10),@bModifica) + ')
							& 
								CASE 
									WHEN CodStatoAppuntamento IN (''PR'',''DP'') THEN 1 
									ELSE 0
								END	
							& 
								CASE 
										WHEN ' + CONVERT(VARCHAR(1),@bAppuntamentiTrasversali) + ' = 1 THEN 1	-- Percorso Trasversale
										WHEN M.IDEpisodio IS NULL AND ''X''=''' + @sSoloEpisodio + ''' THEN 1	-- Ambulatoriale
										WHEN M.IDEpisodio IS NULL AND ''0''=''' + @sSoloEpisodio + ''' THEN 0	-- Ricoverato TUTTI -> Nessun Permesso
										WHEN M.IDEpisodio IS NOT NULL AND ' +	
											CASE 
												WHEN @uIDEpisodio IS NULL THEN ' 1=1 THEN 0						-- IDEpisodio passato NULL -> Nessun Permesso '				
												ELSE 'M.IDEpisodio=''' + CONVERT(VARCHAR(50),@uIDEpisodio) + ''' THEN 1 -- IDEpisodio valorizzato e uguale a quello pasasto->  '
											END											
										+ '
										ELSE 0
								END			
						AS PermessoEroga,
												
						CONVERT(INTEGER,' + CONVERT(CHAR(10),@bModifica) + ')				
						  & 
								CASE 
									WHEN CodStatoAppuntamento  IN (''PR'',''IC'',''AN'',''SS'',''DP'') THEN 1 
									ELSE 0
								END	
						  & 
								CASE 									
									WHEN ' + CONVERT(VARCHAR(1),@bAppuntamentiTrasversali) + ' = 1 THEN 1	-- Percorso Trasversale
									WHEN M.IDEpisodio IS NULL AND ''X''=''' + @sSoloEpisodio + ''' THEN 1	-- Ambulatoriale
									WHEN M.IDEpisodio IS NULL AND ''0''=''' + @sSoloEpisodio + ''' THEN 0	-- Ricoverato TUTTI -> Nessun Permesso
									WHEN M.IDEpisodio IS NOT NULL AND ' +
										CASE 
											WHEN @uIDEpisodio IS NULL THEN ' 1=1 THEN 0						-- IDEpisodio passato NULL -> Nessun Permesso '				
											ELSE 'M.IDEpisodio=''' + CONVERT(VARCHAR(50),@uIDEpisodio) + ''' THEN 1 -- IDEpisodio valorizzato e uguale a quello pasasto->  '
										END											
									+ '
									ELSE 0
								END				
						AS PermessoCambiaStato,
						
						CONVERT(INTEGER,' + CONVERT(CHAR(10),@bInserisci) + ') 
						&
						CASE 
									WHEN ' + CONVERT(VARCHAR(1),@bAppuntamentiTrasversali) + ' = 1 THEN 1	-- Percorso Trasversale
									WHEN M.IDEpisodio IS NULL AND ''X''=''' + @sSoloEpisodio + ''' THEN 1	-- Ambulatoriale
									WHEN M.IDEpisodio IS NULL AND ''0''=''' + @sSoloEpisodio + ''' THEN 0	-- Ricoverato TUTTI -> Nessun Permesso
									WHEN M.IDEpisodio IS NOT NULL AND ' +
										CASE 
											WHEN @uIDEpisodio IS NULL THEN ' 1=1 THEN 0						-- IDEpisodio passato NULL -> Nessun Permesso '				
											ELSE 'M.IDEpisodio=''' + CONVERT(VARCHAR(50),@uIDEpisodio) + ''' THEN 1 -- IDEpisodio valorizzato e uguale a quello pasasto->  '
										END											
									+ '
									ELSE 0
								END	
							AS PermessoCopia,
						CodSistema,
						IDSistema,
						IDGruppo,
						InfoSistema,
						UA.Descrizione AS DescUA,
						M.Titolo,
						M.IDNum,
						ISNULL(T.Multiplo,0) AS Multiplo,
						ISNULL(T.SenzaData,0) AS SenzaData,
						ISNULL(T.SenzaDataSempre,0) AS SenzaDataSempre,
						ISNULL(T.Settimanale,0) AS Settimanale,
						I.IDIcona,
						CONVERT(varbinary(max),null) AS Icona
						'
			END				
	ELSE		
			SET @sSQL='	SELECT						
							M.ID,	
							CASE 
								WHEN DataInizio IS NOT NULL THEN	
										Convert(VARCHAR(10),M.DataInizio,105) + '' '' + LEFT(CONVERT(varchar(20),M.DataInizio,14),5) 									
								ELSE NULL 
							END AS DataInizio,	
							M.ElencoRisorse,								
							--M.Oggetto
							QEPI.Oggetto,
							UA.Descrizione AS DescUA
					  '			
		
	SET @sSQL=@sSQL + '
				FROM 
					T_TmpFiltriAppuntamenti TMP
						INNER JOIN T_MovAppuntamenti	M	
								ON (TMP.IDAppuntamento=M.ID) 
						LEFT JOIN 
							-- Occhi, ora punta al paziente
							 (' + @sSQLQueryAgendeEPI + ')  AS QEPI						
							--	ON QEPI.IDEpisodio=M.IDEpisodio	 
							ON QEPI.IDPaziente=M.IDPaziente
						 LEFT JOIN T_UnitaAtomiche UA
								ON M.CodUA=UA.Codice	
								
						'
								
	IF ISNULL(@bVisualizzazioneSintetica,0)=0		
			SET @sSQL=@sSQL + '						
						-- LEFT JOIN T_MovPazienti P
						--	ON (M.IDPaziente=P.IDPaziente)		
						
						LEFT JOIN T_MovTrasferimenti TRA
							ON (M.IDTrasferimento=TRA.ID)
						
						LEFT JOIN T_MovCartelle CAR
							ON (TRA.IDCartella = CAR.ID)

						LEFT JOIN T_MovEpisodi EPI
							ON (TRA.IDEpisodio = EPI.ID)
																			
						LEFT JOIN T_TipoAppuntamento T
							ON (M.CodTipoAppuntamento=T.Codice)
						LEFT JOIN T_StatoAppuntamento S
							ON (M.CodStatoAppuntamento=S.Codice)	
																						
						LEFT JOIN T_Login L
							ON (M.CodUtenteRilevazione=L.Codice)	
						LEFT JOIN 
								(SELECT ID As IDScheda, CodScheda,Versione,IDEntita,AnteprimaRTF,AnteprimaTXT
								 FROM
									T_MovSchede 
								 WHERE CodEntita=''APP'' AND							
									Storicizzata=0 AND
									CodStatoScheda <> ''CA''
								) AS MS
							ON (MS.IDEntita=M.ID AND
								MS.CodScheda=T.CodScheda)
						LEFT JOIN T_Pazienti P
							ON P.ID=M.IDPaziente																				
														
					  '		  
		SET @sSQL=@sSQL + ' 							
						LEFT JOIN 
							(SELECT 
							  IDNum AS IDIcona,	
							  CodTipo,
							  CodStato							  
							 FROM T_Icone
							 WHERE CodEntita=''APP''
							) AS I
								ON (M.CodTipoAppuntamento=I.CodTipo AND 	
									M.CodStatoAppuntamento=I.CodStato
									)								
					'		
		
	SET @sSQL=@sSQL + ' WHERE TMP.IDSessione=''' + convert(varchar(50),@gIDSessione) +''''
		
	SET @sSQL=@sSQL + @sOrderBy
	
	EXEC (@sSQL)
	
	IF @bDatiEstesi=1
		BEGIN
			SET @sSQL='SELECT DISTINCT
							M.CodTipoAppuntamento AS Codice,
							T.Descrizione AS Descrizione
						FROM 
							T_TmpFiltriAppuntamenti TMP
								INNER JOIN T_MovAppuntamenti	M	
										ON (TMP.IDAppuntamento=M.ID)																													
								LEFT JOIN T_TipoAppuntamento T
									ON (M.CodTipoAppuntamento=T.Codice)													
						'
						
			SET @sSQL=@sSQL + ' WHERE TMP.IDSessione=''' + convert(varchar(50),@gIDSessione) +''''
							
			EXEC (@sSQL)	
		END 			
	
	IF @bDatiEstesi=1
		BEGIN
			SET @sSQL='SELECT DISTINCT
							M.CodStatoAppuntamento AS Codice,
							T.Descrizione AS Descrizione
						FROM 
							T_TmpFiltriAppuntamenti TMP
								INNER JOIN T_MovAppuntamenti	M	
										ON (TMP.IDAppuntamento=M.ID)																													
								LEFT JOIN T_StatoAppuntamento T
									ON (M.CodStatoAppuntamento=T.Codice)													
						'
						
			SET @sSQL=@sSQL + ' WHERE TMP.IDSessione=''' + convert(varchar(50),@gIDSessione) +''''
							
			EXEC (@sSQL)	
		END 			
				
	IF @bDatiEstesi=1
		BEGIN
			SET @sSQL='SELECT DISTINCT
							MovAGE.CodAgenda AS Codice,
							AGE.Descrizione AS Descrizione
						FROM 
							T_TmpFiltriAppuntamenti TMP
																		
								INNER JOIN T_MovAppuntamentiAgende MovAGE
										ON (TMP.IDAppuntamento=MovAGE.IDAppuntamento)

								LEFT JOIN T_Agende AGE
										ON (MOVAGE.CodAgenda=AGE.Codice)
																				
								-- Associazione agende ad UA	
								INNER JOIN 
										(SELECT 
												CodUA,
												CodVoce AS CodAgenda
										 FROM T_AssUAEntita
										 WHERE CodEntita=''AGE''
										 ) AS RA
										ON RA.CodAgenda=MovAGE.CodAgenda 	
							
								-- UA delle agende in join con le UA del ruolo
									INNER JOIN #tmpUARuolo TMPUA
										ON 	TMPUA.CodUA=RA.CodUA				
						'
								
			SET @sSQL=@sSQL + ' WHERE TMP.IDSessione=''' + convert(varchar(50),@gIDSessione) +''''
				
			PRINT @sSQL			
			EXEC (@sSQL)	
		END 			
		
	EXEC MSP_InsMovTimeStamp @xTimeStamp	
							
	DELETE FROM T_TmpFiltriAppuntamenti 
	WHERE IDSessione=@gIDSessione 
	
	DROP TABLE #tmpUARuolo		
		
	DROP TABLE #tmpPazienti								
			
END





