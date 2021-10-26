CREATE PROCEDURE [dbo].[MSP_SelMovOrdiniTrasversali](@xParametri AS XML)
AS
BEGIN
	   	
	   	
				DECLARE @sFiltroGenerico AS Varchar(500) 
	DECLARE @sDataNascita AS VARCHAR(10)	
	DECLARE @sCodRuolo AS VARCHAR(20)	
	
	DECLARE @uIDOrdine AS UNIQUEIDENTIFIER				
	DECLARE @uIDPaziente AS UNIQUEIDENTIFIER	
	DECLARE @uIDEpisodio AS UNIQUEIDENTIFIER	
	DECLARE @uIDTrasferimento AS UNIQUEIDENTIFIER
	DECLARE @sIDOrdineOE AS VARCHAR(50)	
	DECLARE @sNumeroOrdineOE AS VARCHAR(50)
	DECLARE @sCodTipoOrdine AS VARCHAR(1800)
	DECLARE @sCodStatoOrdine AS VARCHAR(1800)
	DECLARE @dDataInserimentoInizio AS DATETIME
	DECLARE @dDataInserimentoFine AS DATETIME
	DECLARE @dDataProgrammazioneOEInizio AS DATETIME
	DECLARE @dDataProgrammazioneOEFine AS DATETIME		
	DECLARE @dDataUltimaModificaInizio AS DATETIME
	DECLARE @dDataUltimaModificaFine AS DATETIME
	DECLARE @sCodUA AS VARCHAR(20)
	DECLARE @sCodFiltroSpeciale	AS VARCHAR(20)	
	
	DECLARE @nNumRighe AS INTEGER
	DECLARE @bDatiEstesi AS Bit		
	
	
		DECLARE @sGUID AS VARCHAR(Max)
	DECLARE @sDataTmp AS VARCHAR(20)
	DECLARE @sIDEpisodio AS VARCHAR(50)											   
		DECLARE @sSQL AS VARCHAR(MAX)
	DECLARE @sWhere AS VARCHAR(Max)
	DECLARE @sOrderBy AS VARCHAR(MAX)
	DECLARE @sTmp AS VARCHAR(Max)

	DECLARE @sSQLFiltro AS VARCHAR(MAX)	
	
		SET @sFiltroGenerico=(SELECT	TOP 1 ValoreParametro.FiltroGenerico.value('.','VARCHAR(500)')
						 FROM @xParametri.nodes('/Parametri/FiltroGenerico') as ValoreParametro(FiltroGenerico))						 
	SET @sFiltroGenerico= ISNULL(@sFiltroGenerico,'')
	SET @sFiltroGenerico=LTRIM(RTRIM(@sFiltroGenerico))
	
		SET @sDataNascita=(SELECT	TOP 1 ValoreParametro.DataNascita.value('.','VARCHAR(10)')
						 FROM @xParametri.nodes('/Parametri/DataNascita') as ValoreParametro(DataNascita))						 
	SET @sDataNascita= ISNULL(@sDataNascita,'')
	SET @sDataNascita=LTRIM(RTRIM(@sDataNascita))
	
		SET @sCodRuolo=(SELECT TOP 1 ValoreParametro.CodRuolo.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/TimeStamp/CodRuolo') as ValoreParametro(CodRuolo))
	SET @sCodRuolo=ISNULL(@sCodRuolo,'')

		SET @sCodUA=(SELECT TOP 1 ValoreParametro.CodUA.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodUA') as ValoreParametro(CodUA))

		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDOrdine.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDOrdine') as ValoreParametro(IDOrdine))
	IF 	ISNULL(@sGUID,'') <> '' SET @uIDOrdine=CONVERT(UNIQUEIDENTIFIER,	@sGUID)	
	
		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDPaziente.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDPaziente') as ValoreParametro(IDPaziente))
	IF 	ISNULL(@sGUID,'') <> '' SET @uIDPaziente=CONVERT(UNIQUEIDENTIFIER,	@sGUID)	
	
		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDEpisodio.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDEpisodio') as ValoreParametro(IDEpisodio))
	IF 	ISNULL(@sGUID,'') <> '' SET @uIDEpisodio=CONVERT(UNIQUEIDENTIFIER,	@sGUID)	
	
	
		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDTrasferimento.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDTrasferimento') as ValoreParametro(IDTrasferimento))
	IF 	ISNULL(@sGUID,'') <> '' SET @uIDTrasferimento=CONVERT(UNIQUEIDENTIFIER,	@sGUID)	
					  				  
		SET @sIDOrdineOE=(SELECT TOP 1 ValoreParametro.IDOrdineOE.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDOrdineOE') as ValoreParametro(IDOrdineOE))	

		SET @sNumeroOrdineOE=(SELECT TOP 1 ValoreParametro.NumeroOrdineOE.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/NumeroOrdineOE') as ValoreParametro(NumeroOrdineOE))	
	
		SET @sCodTipoOrdine=''
	SELECT	@sCodTipoOrdine =  @sCodTipoOrdine +
														CASE 
								WHEN @sCodTipoOrdine='' THEN ''
								ELSE  ','
							END + ''''
						  + ValoreParametro.CodTipoOrdine.value('.','VARCHAR(1800)')
						  + ''''
						 FROM @xParametri.nodes('/Parametri/CodTipoOrdine') as ValoreParametro(CodTipoOrdine)
						 
	SET @sCodTipoOrdine=LTRIM(RTRIM(@sCodTipoOrdine))
	IF	@sCodTipoOrdine='''''' SET @sCodTipoOrdine=''
	SET @sCodTipoOrdine=UPPER(@sCodTipoOrdine)
	
	
		SET @sCodStatoOrdine=''
	SELECT	@sCodStatoOrdine =  @sCodStatoOrdine +
														CASE 
								WHEN @sCodStatoOrdine='' THEN ''
								ELSE  ','
							END + ''''
						  + ValoreParametro.CodStatoOrdine.value('.','VARCHAR(1800)')
						  + ''''
						 FROM @xParametri.nodes('/Parametri/CodStatoOrdine') as ValoreParametro(CodStatoOrdine)
						 				  					  				  							 
	SET @sCodStatoOrdine=LTRIM(RTRIM(@sCodStatoOrdine))
	IF	@sCodStatoOrdine='''''' SET @sCodStatoOrdine=''
	SET @sCodStatoOrdine=UPPER(@sCodStatoOrdine)

		SET @sDataTmp=(SELECT TOP 1 ValoreParametro.DataInserimentoInizio.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/DataInserimentoInizio') as ValoreParametro(DataInserimentoInizio))					  
	SET @sDataTmp=ISNULL(@sDataTmp,'')
	IF @sDataTmp<> '' 
		BEGIN
						SET @sDataTmp= SUBSTRING(@sDataTmp,4,2) + '-' 
						+ LEFT(@sDataTmp,2) + '-' +
						+ SUBSTRING(@sDataTmp,7,4) +
						' ' + RIGHT(@sDataTmp,5)
			IF ISDATE(@sDataTmp)=1
				SET	@dDataInserimentoInizio=CONVERT(DATETIME,@sDataTmp,120)						
			ELSE
				SET	@dDataInserimentoInizio =NULL			
		END
		
		
		SET @sDataTmp=(SELECT TOP 1 ValoreParametro.DataInserimentoFine.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/DataInserimentoFine') as ValoreParametro(DataInserimentoFine))
	SET @sDataTmp=ISNULL(@sDataTmp,'')
	IF @sDataTmp<> '' 
		BEGIN
						SET @sDataTmp= SUBSTRING(@sDataTmp,4,2) + '-' 
						+ LEFT(@sDataTmp,2) + '-' +
						+ SUBSTRING(@sDataTmp,7,4) +
						' ' + RIGHT(@sDataTmp,5)
			IF ISDATE(@sDataTmp)=1
				SET	@dDataInserimentoFine=CONVERT(DATETIME,@sDataTmp,120)						
			ELSE
				SET	@dDataInserimentoFine =NULL		
		END 		
	
		SET @sDataTmp=(SELECT TOP 1 ValoreParametro.DataProgrammazioneOEInizio.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/DataProgrammazioneOEInizio') as ValoreParametro(DataProgrammazioneOEInizio))					  
	SET @sDataTmp=ISNULL(@sDataTmp,'')
	IF @sDataTmp<> '' 
		BEGIN
						SET @sDataTmp= SUBSTRING(@sDataTmp,4,2) + '-' 
						+ LEFT(@sDataTmp,2) + '-' +
						+ SUBSTRING(@sDataTmp,7,4) +
						' ' + RIGHT(@sDataTmp,5)
			IF ISDATE(@sDataTmp)=1
				SET	@dDataProgrammazioneOEInizio=CONVERT(DATETIME,@sDataTmp,120)						
			ELSE
				SET	@dDataProgrammazioneOEInizio =NULL			
		END
		
		
		SET @sDataTmp=(SELECT TOP 1 ValoreParametro.DataProgrammazioneOEFine.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/DataProgrammazioneOEFine') as ValoreParametro(DataProgrammazioneOEFine))
	SET @sDataTmp=ISNULL(@sDataTmp,'')
	IF @sDataTmp<> '' 
		BEGIN
						SET @sDataTmp= SUBSTRING(@sDataTmp,4,2) + '-' 
						+ LEFT(@sDataTmp,2) + '-' +
						+ SUBSTRING(@sDataTmp,7,4) +
						' ' + RIGHT(@sDataTmp,5)
			IF ISDATE(@sDataTmp)=1
				SET	@dDataProgrammazioneOEFine=CONVERT(DATETIME,@sDataTmp,120)						
			ELSE
				SET	@dDataProgrammazioneOEFine =NULL		
		END 
	
		SET @sDataTmp=(SELECT TOP 1 ValoreParametro.DataUltimaModificaInizio .value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/DataUltimaModificaInizio ') as ValoreParametro(DataUltimaModificaInizio ))					  
	SET @sDataTmp=ISNULL(@sDataTmp,'')
	IF @sDataTmp<> '' 
		BEGIN
						SET @sDataTmp= SUBSTRING(@sDataTmp,4,2) + '-' 
						+ LEFT(@sDataTmp,2) + '-' +
						+ SUBSTRING(@sDataTmp,7,4) +
						' ' + RIGHT(@sDataTmp,5)
			IF ISDATE(@sDataTmp)=1
				SET	@dDataUltimaModificaInizio =CONVERT(DATETIME,@sDataTmp,120)						
			ELSE
				SET	@dDataUltimaModificaInizio =NULL			
		END
		

	
		SET @sDataTmp=(SELECT TOP 1 ValoreParametro.DataUltimaModificaFine.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/DataUltimaModificaFine') as ValoreParametro(DataUltimaModificaFine))
	SET @sDataTmp=ISNULL(@sDataTmp,'')
	IF @sDataTmp<> '' 
		BEGIN
						SET @sDataTmp= SUBSTRING(@sDataTmp,4,2) + '-' 
						+ LEFT(@sDataTmp,2) + '-' +
						+ SUBSTRING(@sDataTmp,7,4) +
						' ' + RIGHT(@sDataTmp,5)
			IF ISDATE(@sDataTmp)=1
				SET	@dDataUltimaModificaFine=CONVERT(DATETIME,@sDataTmp,120)						
			ELSE
				SET	@dDataUltimaModificaFine =NULL		
		END 
				
			
		SET @nNumRighe=(SELECT	TOP 1 ValoreParametro.NumRighe.value('.','INTEGER')
								FROM @xParametri.nodes('/Parametri/NumRighe') as ValoreParametro(NumRighe))	
	SET @nNumRighe=ISNULL(@nNumRighe,0)
		
		SET @bDatiEstesi=(SELECT TOP 1 ValoreParametro.DatiEstesi.value('.','bit')
					  FROM @xParametri.nodes('/Parametri/DatiEstesi') as ValoreParametro(DatiEstesi))
	SET @bDatiEstesi=ISNULL(@bDatiEstesi,0)

		SET @sCodFiltroSpeciale=(SELECT TOP 1 ValoreParametro.CodFiltroSpeciale.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodFiltroSpeciale') as ValoreParametro(CodFiltroSpeciale))


	    DECLARE @xTimeStamp AS XML	

    SET @xTimeStamp=(SELECT TOP 1 ValoreParametro.TS.query('.')
					  FROM @xParametri.nodes('/Parametri/TimeStamp') as ValoreParametro(TS))
	
	SET @xTimeStamp=CONVERT(XML,
						'<Parametri>' + CONVERT(varchar(max),@xTimeStamp) +
						'</Parametri>')
						
												
				
			
	CREATE TABLE #tmpUARuolo
	(
		CodUA VARCHAR(20) COLLATE Latin1_General_CI_AS,
		Descrizione VARCHAR(255) COLLATE Latin1_General_CI_AS
	)

	DECLARE @xTmp AS XML
	SET  @xTmp=CONVERT(XML,'<Parametri><CodRuolo>'+ @sCodRuolo + '</CodRuolo></Parametri>')
	
	INSERT #tmpUARuolo EXEC MSP_SelUADaRuolo @xTmp
	
	CREATE INDEX IX_CodUA ON #tmpUARuolo (CodUA)
	
	
			
	CREATE TABLE #tmpFiltriOrdini(IDOrdine UNIQUEIDENTIFIER)
		
	
	SET @sSQL='		INSERT INTO #tmpFiltriOrdini(IDOrdine)					
					SELECT '

		IF @nNumRighe<> 0  SET @sSQL= @sSQL + ' TOP ' + convert(varchar(10),@nNumRighe)+  ' ' 					
	
	SET @sSQL=@sSQL +				
					' M.ID AS IDOrdine
					FROM 
						T_MovOrdini	M							
							LEFT JOIN 
								T_Pazienti P
									ON M.IDPaziente=P.ID
							LEFT JOIN 
								T_MovEpisodi EPI
									ON M.IDEpisodio=EPI.ID	
							LEFT JOIN 
								T_MovTrasferimenti TRA
									ON M.IDTrasferimento=TRA.ID			
					'
	
					
					
	SET @sWhere=''				
	
		IF @uIDOrdine IS NOT NULL
		BEGIN
			SET @sTmp= ' AND M.ID=''' + convert(varchar(50),@uIDOrdine) +''''
			SET @sWhere= @sWhere + @sTmp								
		END	
						
		IF @uIDEpisodio IS NOT NULL
		BEGIN		
			SET @sTmp= ' AND M.IDEpisodio=''' + convert(varchar(50),@uIDEpisodio) +''''
			SET @sWhere= @sWhere + @sTmp								
		END	
	ELSE
	BEGIN
				SET @sTmp= ' AND M.CodUAInserimento IN (SELECT CodUA FROM #tmpUARuolo)'
		SET @sWhere= @sWhere + @sTmp
	END	

		IF @uIDTrasferimento IS NOT NULL
		BEGIN
			SET @sTmp= ' AND M.IDTrasferimento=''' + convert(varchar(50), @uIDTrasferimento) +''''
			SET @sWhere= @sWhere + @sTmp								
		END		
	
		IF @uIDPaziente IS NOT NULL
		BEGIN					
				SET @sTmp= ' AND 
					(M.IDPaziente=''' + convert(varchar(50),@uIDPaziente) +'''
					 OR
					 					 M.IDPaziente IN 
								(SELECT IDPazienteVecchio
								 FROM T_PazientiAlias
								 WHERE 
									IDPaziente IN 
										(SELECT IDPaziente
										 FROM T_PazientiAlias
										 WHERE IDPazienteVecchio=''' + convert(varchar(50),@uIDPaziente) +'''
										)
								)
							
					 )			
					'
					
			SET @sWhere= @sWhere + @sTmp								
		END			

		IF @sCodUA IS NOT NULL
		BEGIN
			SET @sTmp= ' AND ISNULL(M.CodUAUltimaModifica,M.CodUAInserimento)=''' + @sCodUA +''''
			SET @sWhere= @sWhere + @sTmp								
		END		
	
		IF @sIDOrdineOE IS NOT NULL
		BEGIN
			SET @sTmp= ' AND M.IDOrdineOE=''' + convert(varchar(50),@sIDOrdineOE) +''''
			SET @sWhere= @sWhere + @sTmp								
		END		
	
		IF @uIDPaziente IS NOT NULL
		BEGIN
			SET @sTmp= ' AND M.IDPaziente=''' + convert(varchar(50),@uIDPaziente) +''''
			SET @sWhere= @sWhere + @sTmp								
		END			
				
		IF @sNumeroOrdineOE IS NOT NULL
		BEGIN
			SET @sTmp= ' AND M.NumeroOrdineOE=''' + convert(varchar(50),@sNumeroOrdineOE) +''''
			SET @sWhere= @sWhere + @sTmp								
		END		
	
		IF @sCodTipoOrdine	NOT IN ('','''Tutti''')
		BEGIN	
						SET @sTmp=  ' AND 			
							 M.ID IN (
										SELECT IDOrdine 
										FROM T_MovOrdiniEroganti
										WHERE CodTipoOrdine IN ('+ @sCodTipoOrdine + ')
									  ) 
						'  				
			SET @sWhere= @sWhere + @sTmp														
		END
		
		IF @sCodStatoOrdine NOT IN ('','''Tutti''')
		BEGIN	
						SET @sTmp=  ' AND 			
							 M.CodStatoOrdine IN ('+ @sCodStatoOrdine + ')							 
						'  				
			SET @sWhere= @sWhere + @sTmp														
		END	
			
		

		IF @dDataInserimentoInizio IS NOT NULL 
		BEGIN
			SET @sTmp= CASE 
							WHEN @dDataInserimentoFine IS NULL 
									THEN ' AND M.DataInserimento = CONVERT(datetime,'''  + convert(varchar(20),@dDataInserimentoInizio,120) +''',120)'									
							ELSE ' AND M.DataInserimento >= CONVERT(datetime,'''  + convert(varchar(20),@dDataInserimentoInizio,120) +''',120)'	
						END	
			SET @sWhere= @sWhere + @sTmp								
		END

		IF @dDataInserimentoFine IS NOT NULL 
		BEGIN
			SET @sTmp= ' AND M.DataInserimento <= CONVERT(datetime,'''  + convert(varchar(20),@dDataInserimentoFine,120) +''',120)'							
			SET @sWhere= @sWhere + @sTmp								
		END
	
		IF  @dDataUltimaModificaInizio IS NOT NULL 
		BEGIN
			SET @sTmp= CASE 
							WHEN @dDataInserimentoFine IS NULL 
									THEN ' AND M.DataUltimaModifica = CONVERT(datetime,'''  + convert(varchar(20), @dDataUltimaModificaInizio,120) +''',120)'									
							ELSE ' AND M.DataUltimaModifica >= CONVERT(datetime,'''  + convert(varchar(20), @dDataUltimaModificaInizio,120) +''',120)'	
						END	
			SET @sWhere= @sWhere + @sTmp								
		END

		IF @dDataUltimaModificaFine	 IS NOT NULL 
		BEGIN
			SET @sTmp= ' AND M.DataUltimaModifica <= CONVERT(datetime,'''  + convert(varchar(20),@dDataUltimaModificaFine,120) +''',120)'							
			SET @sWhere= @sWhere + @sTmp								
		END
				
		IF  @dDataProgrammazioneOEInizio IS NOT NULL 
		BEGIN
			SET @sTmp= CASE 
							WHEN @dDataProgrammazioneOEFine IS NULL 
									THEN ' AND M.DataProgrammazioneOE = CONVERT(datetime,'''  + convert(varchar(20), @dDataProgrammazioneOEInizio,120) +''',120)'									
							ELSE ' AND M.DataProgrammazioneOE >= CONVERT(datetime,'''  + convert(varchar(20), @dDataProgrammazioneOEInizio,120) +''',120)'	
						END	
			SET @sWhere= @sWhere + @sTmp								
		END

		IF @dDataProgrammazioneOEFine	 IS NOT NULL 
		BEGIN
			SET @sTmp= ' AND M.DataProgrammazioneOE <= CONVERT(datetime,'''  + convert(varchar(20),@dDataProgrammazioneOEFine,120) +''',120)'							
			SET @sWhere= @sWhere + @sTmp								
		END
		
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
		SET @sWhere= @sWhere + '  OR EPI.NumeroNosologico like ''%' + @sFiltroGenerico + '%'''	
		SET @sWhere= @sWhere + '  OR EPI.NumeroNosologico like ''%' + dbo.MF_ModificaNosologico(@sFiltroGenerico)+ '%'''	
		SET @sWhere= @sWhere + '  OR EPI.NumeroListaAttesa like ''%' + @sFiltroGenerico + '%'''	
		
					SET @sWhere= @sWhere + '     )' 					
	END
																		
		IF ISNULL(@sCodFiltroSpeciale,'') <> ''
	BEGIN
		SET @sSQLFiltro=(SELECT SQL FROM T_FiltriSpeciali WHERE Codice=@sCodFiltroSpeciale)
		IF ISNULL(@sSQLFiltro,'') <> '' 
		BEGIN						
			SET @sWhere= @sWhere + ' AND ' + 	@sSQLFiltro + ''
		END
	END	

	  	IF @bDatiEstesi=1
		BEGIN
			SET @sWhere= @sWhere + ' AND 1=0'
		END
			
	IF ISNULL(@sWhere,'')<> ''
	BEGIN	
		SET @sSQL=@sSQL +' WHERE ' + RIGHT(@sWhere,len(@sWhere)-5)
	END	
	
				SET @sOrderBy = ' ORDER BY DataProgrammazioneOE ASC '	
	SET @sSQL=@sSQL + @sOrderBy
	
	PRINT @sSQL 	
	EXEC (@sSQL)
	
	SET @sSQL='	SELECT
				  M.ID
				  ,M.IDNum
				  ,M.IDPaziente
				  ,P.CodSac
				  ,P.Cognome
				  ,P.Nome
				  ,P.DataNascita				
				  ,ISNULL(P.Cognome,'''') + '' '' + ISNULL(P.Nome,'''') + '' ('' + ISNULL(P.Sesso,'''') + '') '' +
									CASE
										WHEN P.DataNascita IS NOT NULL THEN '' ('' + CONVERT(VARCHAR(10),P.DataNascita,105) + '') ''
										ELSE ''''
									END +
																		'' - '' + ISNULL(EPI.NumeroNosologico,'''') 
				   AS DescrPaziente
				  ,P.CodiceFiscale
				  ,IDEpisodio
				  ,IDTrasferimento
				  ,IDOrdineOE
				  ,DataProgrammazioneOE
				  ,NumeroOrdineOE				
				  ,Eroganti
				  ,Prestazioni					 
				  ,CodStatoOrdine
				  ,So.Descrizione AS DescrStatoOrdine
				  ,CodUtenteInserimento
				  ,CodPriorita
				  ,PO.Descrizione as DescrPrioritaOrdine
				  ,LI.Descrizione AS DescrUtenteInserimento
				  ,CodUtenteUltimaModifica
				  ,LM.Descrizione AS DescrUtenteUltimaModifica
				  ,CodUtenteInoltro
				  ,LV.Descrizione AS DescrUtenteInoltro
				  ,CASE 
							WHEN DataInserimento IS NOT NULL THEN	
									Convert(VARCHAR(10),M.DataInserimento,105) + '' '' + LEFT(CONVERT(varchar(20),M.DataInserimento,14),5) 									
							ELSE NULL 
				   END AS DataInserimento					  
				  ,CASE 
							WHEN DataUltimaModifica IS NOT NULL THEN	
									Convert(VARCHAR(10),M.DataUltimaModifica,105) + '' '' + LEFT(CONVERT(varchar(20),M.DataUltimaModifica,14),5) 									
							ELSE NULL 
				   END AS DataUltimaModifica
				  ,CASE 
							WHEN DataInoltro IS NOT NULL THEN	
									Convert(VARCHAR(10),M.DataInoltro,105) + '' '' + LEFT(CONVERT(varchar(20),M.DataInoltro,14),5) 									
							ELSE NULL 
				   END AS DataInoltro					  
				'
				
	IF @bDatiEstesi=1	
			SET @sSQL=@sSQL + ',M.XMLOE AS XMLOE'

	SET @sSQL=@sSQL + '
				FROM 
					  T_MovOrdini M								
						LEFT JOIN 
								T_Pazienti P
									ON M.IDPaziente=P.ID
						LEFT JOIN 
								T_MovEpisodi EPI
									ON M.IDEpisodio=EPI.ID																																				
						LEFT JOIN T_StatoOrdine SO 
							ON (M.CodStatoOrdine=SO.Codice)
						LEFT JOIN T_PrioritaOrdine PO
							ON (M.CodPriorita=PO.Codice)	
						'
						
		SET @sSQL=@sSQL + '											
						LEFT JOIN T_Login LI
							ON (M.CodUtenteInserimento=LI.Codice)
						LEFT JOIN T_Login LM
							ON (M.CodUtenteUltimaModifica=LM.Codice)
						LEFT JOIN T_Login LV
							ON (M.CodUtenteInoltro=LV.Codice)			
					  '	

		SET @sSQL=@sSQL + '	WHERE M.ID IN (SELECT IDOrdine FROM #tmpFiltriOrdini) '						

		IF @bDatiEstesi=1
		SET @sSQL=@sSQL + ' AND 1=0'	
			
	
		SET @sSQL=@sSQL + @sOrderBy
	
	PRINT @sSQL						
	EXEC (@sSQL)
	
					
	IF @bDatiEstesi=1
		BEGIN
			SET @sSQL='SELECT DISTINCT
							T.Codice AS CodStato,
							T.Descrizione AS DescStato							
						FROM
							T_StatoOrdine T							
						'			

						EXEC (@sSQL)
		END

					
	IF @bDatiEstesi=1
		BEGIN
			
			SET @sSQL='SELECT 
							T.Codice AS CodTipoOrdine,
							CASE 
								WHEN LTRIM(ISNULL(T.Descrizione,''''))=''()'' THEN ''(Profilo)'' 
								ELSE T.Descrizione
								END 
								AS DescTipoORdine							
						FROM 							
							T_TipoOrdine T
						ORDER BY Descrizione
						'
		
						
						EXEC (@sSQL)
		END
					
	IF @bDatiEstesi=1
		BEGIN
																																																																														
			SELECT 
				CodUA,
				Descrizione AS DescUA
			FROM #tmpUARuolo

								END
		
				
			
		
					
		DROP TABLE #tmpFiltriOrdini
								
				
END