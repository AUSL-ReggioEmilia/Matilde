CREATE PROCEDURE [dbo].[MSP_SelMovAppuntamentiAgende](@xParametri AS XML)
AS
BEGIN
	   	
	   	
				
	DECLARE @bDatiEstesi AS BIT			
	DECLARE @bLista AS BIT	
	DECLARE @sCodAgenda AS VARCHAR(1800)	
	DECLARE @sFiltroGenerico AS VARCHAR(500)	
	
	DECLARE @dDataInizio AS DATETIME
	DECLARE @dDataFine AS DATETIME		
	
	DECLARE @dDataInserimentoInizio AS DATETIME
	DECLARE @dDataInserimentoFine AS DATETIME		
	
		
	DECLARE @sDataTmp AS VARCHAR(20)
	
	DECLARE @sCodStatoAppuntamento AS VARCHAR(1800)

		DECLARE @sCodRuolo AS Varchar(20)
	DECLARE @sCodUtente AS VARCHAR(100)	
	DECLARE @xTmp AS XML						  				 	
	DECLARE @sGUID AS VARCHAR(Max)
	DECLARE @sSQLQueryAgendePAZ AS VARCHAR(MAX)
	
	DECLARE @nTemp AS INTEGER
	DECLARE @bInserisci AS BIT
	DECLARE @bModifica AS BIT
	DECLARE @bCancella AS BIT
	
	
		SET @bDatiEstesi=(SELECT TOP 1 ValoreParametro.DatiEstesi.value('.','BIT')
					  FROM @xParametri.nodes('/Parametri/DatiEstesi') as ValoreParametro(DatiEstesi))
	SET @bDatiEstesi=ISNULL(@bDatiEstesi,0)

		SET @bLista=(SELECT TOP 1 ValoreParametro.Lista.value('.','BIT')
					  FROM @xParametri.nodes('/Parametri/Lista') as ValoreParametro(Lista))

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
				SET	@dDataInserimentoInizio=NULL			
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

		SET @sFiltroGenerico=(SELECT TOP 1 ValoreParametro.FiltroGenerico.value('.','VARCHAR(500)')
					  FROM @xParametri.nodes('/Parametri/FiltroGenerico') as ValoreParametro(FiltroGenerico))
	SET @sFiltroGenerico=ISNULL(@sFiltroGenerico,'')
	
		SET @sFiltroGenerico=REPLACE(@sFiltroGenerico,'''','''''')		

		SET @sCodRuolo=(SELECT	TOP 1 ValoreParametro.CodRuolo.value('.','VARCHAR(20)')
						 FROM @xParametri.nodes('/Parametri/TimeStamp/CodRuolo') as ValoreParametro(CodRuolo))						 
	SET @sCodRuolo= LTRIM(RTRIM(ISNULL(@sCodRuolo,'')))		
	
		SET @sCodUtente=(SELECT TOP 1 ValoreParametro.CodLogin.value('.','VARCHAR(100)')
					  FROM @xParametri.nodes('/Parametri/TimeStamp/CodLogin') as ValoreParametro(CodLogin))	
	SET @sCodUtente=ISNULL(@sCodUtente,'')
			
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
			  
	    DECLARE @xTimeStamp AS XML	

    SET @xTimeStamp=(SELECT TOP 1 ValoreParametro.TS.query('.')
					  FROM @xParametri.nodes('/Parametri/TimeStamp') as ValoreParametro(TS))
	
	SET @xTimeStamp=CONVERT(XML,
						'<Parametri>' + CONVERT(varchar(max),@xTimeStamp) +
						'</Parametri>')											
											
				
	CREATE TABLE #tmpUAUtente
		(
			CodUA VARCHAR(20) COLLATE Latin1_General_CI_AS,
			Descrizione VARCHAR(255) COLLATE Latin1_General_CI_AS
		)				
	
	SET  @xTmp=CONVERT(XML,'<Parametri><CodUtente>'+ @sCodUtente + '</CodUtente></Parametri>')		
	INSERT #tmpUAUtente EXEC MSP_SelUADaUtente @xTmp	
	
			CREATE INDEX IX_CodUA ON #tmpUAUtente (CodUA)
	
		
							
			
						DECLARE @sSQL AS VARCHAR(MAX)
			DECLARE @sWhere AS VARCHAR(Max)
			DECLARE @sTmp AS VARCHAR(Max)
			DECLARE @gIDSessione AS UNIQUEIDENTIFIER
			
				
						
						
			SET @sSQLQueryAgendePAZ=(SELECT dbo.MF_SQLQueryAgendePAZ())			

			CREATE TABLE #tmpMovAppuntamentiAgende
				(ID UNIQUEIDENTIFIER,
				 IDAppuntamento UNIQUEIDENTIFIER)

			CREATE TABLE #tmpMovNoteAgende
				(ID UNIQUEIDENTIFIER,
				 IDAppuntamento UNIQUEIDENTIFIER)
		


						IF @bDatiEstesi=0
 			BEGIN 				
			
																				
															
					SET @sSQL='INSERT INTO #tmpMovAppuntamentiAgende(ID,IDAppuntamento)
								SELECT		
										AGE.ID,											
										M.ID AS IDAppuntamento								
								  '					
				
					SET @sSQL=@sSQL + '
								FROM 			
										T_MovAppuntamentiAgende	AGE WITH (NOLOCK)					
											INNER JOIN T_MovAppuntamenti M WITH (NOLOCK)
												ON AGE.IDAppuntamento=M.ID
											INNER JOIN T_Agende AAG WITH (NOLOCK)
												ON AGE.CodAgenda=AAG.Codice
																								
										LEFT JOIN
												T_Pazienti P WITH (NOLOCK)
													ON (M.IDPaziente=P.ID)																																																								
										'
													
					
										SET @sWhere=''
				
										IF 	@sFiltroGenerico <> ''
						BEGIN	
														SET @sTmp=  ' AND 			
											 (ISNULL(P.Cognome,'''') + '' '' + ISNULL(P.Nome,'''') like  ''%'+ @sFiltroGenerico + '%''
											  OR
											  ISNULL(P.Nome,'''') + '' '' + ISNULL(P.Cognome,'''') like  ''%'+ @sFiltroGenerico + '%''
											 ) 
									 
										'  				
							SET @sWhere= @sWhere + @sTmp														
						END			
				
										IF 	@sCodAgenda NOT IN ('')
						BEGIN	
														SET @sTmp=  ' AND 			
											 AGE.CodAgenda IN ('+ @sCodAgenda + ')
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
				
										IF @dDataInserimentoInizio IS NOT NULL 
						BEGIN
							SET @sTmp= CASE 
											WHEN @dDataInserimentoFine IS NULL 
													THEN ' AND M.DataEvento = CONVERT(datetime,'''  + convert(varchar(20),@dDataInserimentoInizio,120) +''',120)'									
											ELSE ' AND M.DataEvento >= CONVERT(datetime,'''  + convert(varchar(20),@dDataInserimentoInizio,120) +''',120)'	
										END	
							SET @sWhere= @sWhere + @sTmp								
						END

										IF @dDataInserimentoFine IS NOT NULL 
						BEGIN
							SET @sTmp= ' AND M.DataEvento <= CONVERT(datetime,'''  + convert(varchar(20),@dDataInserimentoFine,120) +''',120)'							
							SET @sWhere= @sWhere + @sTmp								
						END


										if @bLista IS NOT NULL
						BEGIN
							SET @sTmp= ' AND ISNULL(AAG.Lista,0) = ' + CONVERT(varchar(1),@bLista)	
							SET @sWhere= @sWhere + @sTmp								
						END
			
										IF @sCodStatoAppuntamento NOT IN ('')
					BEGIN	
												SET @sTmp=  ' AND 			
									 M.CodStatoAppuntamento IN ('+ @sCodStatoAppuntamento + ')
								'  				
						SET @sWhere= @sWhere + @sTmp														
					END
		

					
										SET @sTmp=  ' AND 			
									 M.CodStatoAppuntamento NOT IN (''CA'',''TR'')	AND
									 AGE.CodStatoAppuntamentoAgenda <> ''CA''
							 			  
								'  	
				
										IF ISNULL(@sTmp,'') <> '' 		
						SET @sWhere= @sWhere + @sTmp	
											
										IF ISNULL(@sWhere,'')<> ''
					BEGIN			
						SET @sSQL=@sSQL +' WHERE ' + RIGHT(@sWhere,len(@sWhere)-5)
					END	
									
					EXEC (@sSQL)
																
					DECLARE @sSQLNote AS VARCHAR(MAX)
					DECLARE @sSQLWhereNote AS VARCHAR(MAX)

					
					SET @sSQLNote=''
			
					SET @sSQLNote='	INSERT INTO #tmpMovNoteAgende(ID,IDAppuntamento)
									SELECT					
										NOTE.ID,	
										NULL AS IDAppuntamento
									
								  '													
					SET @sSQLNote=@sSQLNote + '
								FROM 			
										T_MovNoteAgende	NOTE	
											LEFT JOIN T_Agende AAG WITH (NOLOCK)
												ON NOTE.CodAgenda=AAG.Codice
											LEFT JOIN T_StatoNoteAgende SA
											 ON (NOTE.CodStatoNota=SA.Codice)
										
										'													
					
					SET @sSQLWhereNote=''
								
					IF 	@sCodAgenda NOT IN ('')
						BEGIN	
														SET @sTmp=  ' AND 			
											 NOTE.CodAgenda IN ('+ @sCodAgenda + ')
										'  				
							SET @sSQLWhereNote= @sSQLWhereNote + @sTmp														
						END																		
								
										IF @dDataInizio IS NOT NULL 
						BEGIN
							SET @sTmp= CASE 
											WHEN @dDataFine IS NULL 
													THEN ' AND NOTE.DataInizio = CONVERT(datetime,'''  + convert(varchar(20),@dDataInizio,120) +''',120)'									
											ELSE ' AND NOTE.DataInizio >= CONVERT(datetime,'''  + convert(varchar(20),@dDataInizio,120) +''',120)'	
										END	
							SET @sSQLWhereNote= @sSQLWhereNote + @sTmp								
						END

										IF @dDataFine IS NOT NULL 
						BEGIN
							SET @sTmp= ' AND NOTE.DataFine <= CONVERT(datetime,'''  + convert(varchar(20),@dDataFine,120) +''',120)'							
							SET @sSQLWhereNote= @sSQLWhereNote + @sTmp								
						END
			
										if @bLista IS NOT NULL
						BEGIN
							SET @sTmp= ' AND ISNULL(AAG.Lista,0) = ' + CONVERT(varchar(1),@bLista)	
							SET @sSQLWhereNote= @sSQLWhereNote + @sTmp								
						END

									
										SET @sTmp=  ' AND 			
									 NOTE.CodStatoNota <> ''CA''				  
								'  	
							
					IF ISNULL(@sTmp,'') <> '' 		
						SET @sSQLWhereNote= @sSQLWhereNote + @sTmp	
					
						
										IF ISNULL(@sSQLWhereNote,'')<> ''
					BEGIN
			
						SET @sSQLNote=@sSQLNote +' WHERE ' + RIGHT(@sSQLWhereNote,len(@sSQLWhereNote)-5)
					END	
								
					EXEC (@sSQLNote)
					
																				
					SET @sSQL='	SELECT					
										AGE.ID,	
										M.ID AS IDAppuntamento,
										M.IDPaziente,
										M.IDEpisodio,
										M.IDTrasferimento,
										AGE.CodAgenda,																	
										M.DataInizio,
										M.DataFine,												
										
										CASE 
											WHEN ISNULL(Titolo,'''') ='''' THEN
												ISNULL(P.Cognome,'''') + '' '' + ISNULL(P.Nome,'''') + '' '' +																										
													CASE WHEN DataNascita IS	NULL THEN ''''
													ELSE 
														CONVERT(VARCHAR(10),DataNascita,105) 
													END																			
												+ '' '' + ISNULL(P.Sesso,'''') 
											ELSE Titolo
										END
										AS Oggetto,

										MS.AnteprimaTXT AS Descrizione,
										MS.AnteprimaRTF,
										AGE.CodStatoAppuntamentoAgenda AS CodStatoAppuntamentoAgenda,
										ISNULL(SA.Descrizione,'''') AS DescrStatoAppuntamentoAgenda,
										M.CodStatoAppuntamento,
										SAPP.Descrizione As DescrStatoAppuntamento,								
											CASE 
												WHEN ISNULL(AAG.UsaColoreTipoAppuntamento,0)=0 THEN SA.Colore 																		
												WHEN ISNULL(T.Colore, ''Color [A=0, R=0, G=0, B=0]'') =''Color [A=0, R=0, G=0, B=0]'' THEN SA.Colore
												ELSE T.Colore
											END AS Colore,								
										CONVERT(INTEGER,0) AS flagNota,
										NULL AS IDGruppo,
										CASE 
											WHEN tmpUA.CodUA IS NOT NULL AND TRA.IDCartella IS NOT NULL THEN 1 
											ELSE 0
										END
										AS PermessoCartella,
										CONVERT(BIT,0) AS TuttoIlGiorno,
										AGE.CodRaggr1,
										AGE.DescrRaggr1,
										AGE.CodRaggr2,
										AGE.DescrRaggr2,
										AGE.CodRaggr3,
										AGE.DescrRaggr3,
										M.DataEvento AS DataInserimento,
										CONVERT(INTEGER,ISNULL(UA.VisualizzaIconeAppuntamenti,0)) AS VisualizzaIconeAppuntamenti
								  '					
								  				
					SET @sSQL=@sSQL + '
								FROM 			
										T_MovAppuntamentiAgende	AGE WITH (NOLOCK)					
											LEFT JOIN T_MovAppuntamenti M WITH (NOLOCK)
												ON AGE.IDAppuntamento=M.ID
											LEFT JOIN T_Agende AAG WITH (NOLOCK)
												ON AGE.CodAgenda=AAG.Codice
											LEFT JOIN T_TipoAppuntamento T WITH (NOLOCK)
												ON (M.CodTipoAppuntamento=T.Codice)
											LEFT JOIN T_StatoAppuntamentoAgende SA WITH (NOLOCK)
												ON (AGE.CodStatoAppuntamentoAgenda=SA.Codice)	
											LEFT JOIN T_StatoAppuntamento SAPP WITH (NOLOCK)
												ON (M.CodStatoAppuntamento=SAPP.Codice)		
											LEFT JOIN 
												T_MovSchede MS WITH (NOLOCK)
													ON (MS.IDEntita=M.ID AND
														MS.CodScheda=T.CodScheda AND
														MS.CodEntita=''APP'' AND							
														MS.Storicizzata=0 AND
														MS.CodStatoScheda <> ''CA'')											
										LEFT JOIN
												T_Pazienti P WITH (NOLOCK)
													ON (M.IDPaziente=P.ID)
																		
										LEFT JOIN #tmpUAUtente AS tmpUA WITH (NOLOCK)
												ON M.CodUA=tmpUA.CodUA
										
										LEFT JOIN T_MovTrasferimenti TRA WITH (NOLOCK)
											ON M.IDTrasferimento=TRA.ID
										LEFT JOIN T_MovCartelle CAR WITH (NOLOCK)
											ON TRA.IDCartella=CAR.ID
										
										LEFT JOIN T_UnitaAtomiche UA
											ON M.CodUA=UA.Codice
										'
					
										SET @sSQL=@sSQL + ' WHERE AGE.ID IN (SELECT ID FROM #tmpMovAppuntamentiAgende) '

					PRINT @sSQL						
					
					
																				
					
					SET @sSQLNote=''
			
											SET @sSQLNote='	SELECT					
										NOTE.ID,	
										NULL AS IDAppuntamento,
										NULL AS IDPaziente,
										NULL AS IDEpisodio,
										NULL AS IDTrasferimento,
										NOTE.CodAgenda,																	
										NOTE.DataInizio,
										NOTE.DataFine,												
										NOTE.Oggetto,
										NOTE.Descrizione,
										NOTE.Descrizione AS AnteprimaRTF,
										NOTE.CodStatoNota AS CodStatoAppuntamentoAgenda,
										ISNULL(SA.Descrizione,'''') AS DescrStatoAppuntamentoAgenda,
										NULL AS CodStatoAppuntamento,
										NULL DescrStatoAppuntamento,
										CASE 
											WHEN ISNULL(NOTE.Colore, ''Color [A=0, R=0, G=0, B=0]'')  IN (''Color [A=0, R=0, G=0, B=0]'',
																										  ''Color [Empty]'') THEN ''Color [White]''
											ELSE NOTE.Colore
										END AS Colore,
										CONVERT(INTEGER,1) AS flagNota,
										NOTE.IDGruppo,
										0 AS PermessoCartella,
										ISNULL(NOTE.TuttoIlGiorno,0) AS TuttoIlGiorno,
										NULL AS CodRaggr1,
										NULL AS DescrRaggr1,
										NULL AS CodRaggr2,
										NULL AS DescrRaggr2,
										NULL AS CodRaggr3,
										NULL AS DescrRaggr3,
										NOTE.DataEvento AS DataInserimento,
										1 AS VisualizzaIconeAppuntamenti
								  '					
				
				
					SET @sSQLNote=@sSQLNote + '
								FROM 			
										T_MovNoteAgende	NOTE	
											LEFT JOIN T_Agende AAG WITH (NOLOCK)
												ON NOTE.CodAgenda=AAG.Codice
											LEFT JOIN T_StatoNoteAgende SA
											 ON (NOTE.CodStatoNota=SA.Codice)
										
										'
										SET @sSQLNote=@sSQLNote + ' WHERE NOTE.ID IN (SELECT ID FROM #tmpMovNoteAgende) '

										
																				SET @sSQL='SELECT * FROM (' + @sSQL + ' ) AS MOVAPP1 UNION ALL ( ' + @sSQLNote + ' )   '
	
					PRINT @sSQL
 	 				 					
					EXEC (@sSQL)
				
		END						
	ELSE
		BEGIN
									
						SET @sSQL='SELECT 
							D.Codice,
							D.Descrizione,
							D.OrariLavoro,
							D.IntervalloSlot,
							TA.Icona,
							D.Colore,
							D.ParametriLista
							
							
							FROM T_Agende  D
								LEFT JOIN T_TipoAgenda TA
										ON D.CodTipoAgenda=TA.Codice '	
			IF 	@sCodAgenda <> ''
				BEGIN																			 
					SET	@sSQL=@sSQL+ '	WHERE D.Codice IN ( ' +@sCodAgenda + ' )'	

					IF @bLista IS NOT NULL
						SET	@sSQL=@sSQL+ '	AND ISNULL(D.Lista,0) = ' + CONVERT(varchar(1),@bLista)		
				END
			ELSE
				SET	@sSQL=@sSQL+ '	WHERE 1=0'	

			PRINT @sSQL			
			EXEC (@sSQL)	
		END 
	
				
	EXEC MSP_InsMovTimeStamp @xTimeStamp	
															
		DROP TABLE #tmpMovAppuntamentiAgende	
	DROP TABLE #tmpMovNoteAgende	
END