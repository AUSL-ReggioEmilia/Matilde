CREATE PROCEDURE [dbo].[MSP_SelMovConsegnePazienteTrasversale](@xParametri AS XML)
AS
BEGIN
	   	
	   	
				
	DECLARE @uIDConsegnaPaziente AS UNIQUEIDENTIFIER	
	DECLARE @uIDConsegnaPazienteRuoli AS UNIQUEIDENTIFIER
	DECLARE @uIDEpisodio AS UNIQUEIDENTIFIER

	DECLARE @sCodUA AS VARCHAR(MAX)
	DECLARE @sCodRuolo AS VARCHAR(20)
	DECLARE @sCodTipoConsegnaPaziente AS VARCHAR(1800)
	DECLARE @sCodStatoConsegnaPaziente AS VARCHAR(1800)
	DECLARE @dDataInizio AS DATETIME
	DECLARE @dDataFine AS DATETIME
	DECLARE @sFiltroGenerico AS VARCHAR(500) 
	DECLARE @bDatiEstesi AS BIT
	DECLARE @bNascondiVistati AS BIT
	DECLARE @sCodFiltroSpeciale	AS VARCHAR(20)
	DECLARE @bOrdinaLetto AS BIT

	DECLARE @sDataTmp AS VARCHAR(20)		

		DECLARE @sGUID AS VARCHAR(Max)
	DECLARE @nTemp AS INTEGER
	DECLARE @xTmpTS AS XML

	DECLARE @sSQL AS VARCHAR(MAX)
	DECLARE @sWhere AS VARCHAR(Max)
	DECLARE @sOrderby AS VARCHAR(MAX)
	DECLARE @sTmp AS VARCHAR(Max)	
	
	DECLARE @bInserisci AS BIT
	DECLARE @bModifica AS BIT
	DECLARE @bAnnulla AS BIT	
	DECLARE @bCancella AS BIT	
	DECLARE @bVisto AS BIT	

	
			
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
			
		SET @sCodRuolo=(SELECT TOP 1 ValoreParametro.CodRuolo.value('.','VARCHAR(20)')
								FROM @xParametri.nodes('/Parametri/TimeStamp/CodRuolo') as ValoreParametro(CodRuolo))
	SET @sCodRuolo=ISNULL(@sCodRuolo,'')
	
		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDConsegnaPaziente.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDConsegnaPaziente') as ValoreParametro(IDConsegnaPaziente))		
	IF 	ISNULL(@sGUID,'') <> '' SET @uIDConsegnaPaziente=CONVERT(UNIQUEIDENTIFIER,	@sGUID)	

		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDConsegnaPazienteRuoli.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDConsegnaPazienteRuoli') as ValoreParametro(IDConsegnaPazienteRuoli))		
	IF 	ISNULL(@sGUID,'') <> '' SET @uIDConsegnaPazienteRuoli=CONVERT(UNIQUEIDENTIFIER,	@sGUID)	

		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDEpisodio.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDEpisodio') as ValoreParametro(IDEpisodio))		
	IF 	ISNULL(@sGUID,'') <> '' SET @uIDEpisodio=CONVERT(UNIQUEIDENTIFIER,	@sGUID)	
						  			  
		SET @sCodTipoConsegnaPaziente=''
	SELECT @sCodTipoConsegnaPaziente = @sCodTipoConsegnaPaziente +
														CASE 
								WHEN @sCodTipoConsegnaPaziente='' THEN ''
								ELSE  ','
							END + ''''
						  + ValoreParametro.CodTipoConsegnaPaziente.value('.','VARCHAR(1800)')
						  + ''''
						 FROM @xParametri.nodes('/Parametri/CodTipoConsegnaPaziente') as ValoreParametro(CodTipoConsegnaPaziente)
						 
	SET @sCodTipoConsegnaPaziente=LTRIM(RTRIM(@sCodTipoConsegnaPaziente))
	IF	@sCodTipoConsegnaPaziente='''''' SET @sCodTipoConsegnaPaziente=''
	SET @sCodTipoConsegnaPaziente=UPPER(@sCodTipoConsegnaPaziente)

		SET @sCodStatoConsegnaPaziente=''
	SELECT @sCodStatoConsegnaPaziente = @sCodStatoConsegnaPaziente +
														CASE 
								WHEN @sCodStatoConsegnaPaziente='' THEN ''
								ELSE  ','
							END + ''''
						  + ValoreParametro.CodStatoConsegnaPaziente.value('.','VARCHAR(1800)')
						  + ''''
						 FROM @xParametri.nodes('/Parametri/CodStatoConsegnaPaziente') as ValoreParametro(CodStatoConsegnaPaziente)
						 
	SET @sCodStatoConsegnaPaziente=LTRIM(RTRIM(@sCodStatoConsegnaPaziente))
	IF	@sCodStatoConsegnaPaziente='''''' SET @sCodStatoConsegnaPaziente=''
	SET @sCodStatoConsegnaPaziente=UPPER(@sCodStatoConsegnaPaziente)
	
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
		
		SET @sFiltroGenerico=(SELECT	TOP 1 ValoreParametro.FiltroGenerico.value('.','VARCHAR(500)')
						 FROM @xParametri.nodes('/Parametri/FiltroGenerico') as ValoreParametro(FiltroGenerico))						 
	SET @sFiltroGenerico= ISNULL(@sFiltroGenerico,'')
	SET @sFiltroGenerico=LTRIM(RTRIM(@sFiltroGenerico))
	
		IF ISNULL(@sFiltroGenerico,'') <> ''
		SET @sFiltroGenerico=REPLACE(@sFiltroGenerico,'''','')

		SET @bDatiEstesi=(SELECT TOP 1 ValoreParametro.DatiEstesi.value('.','bit')
					  FROM @xParametri.nodes('/Parametri/DatiEstesi') as ValoreParametro(DatiEstesi))
	SET @bDatiEstesi=ISNULL(@bDatiEstesi,0)
	
		SET @bNascondiVistati=(SELECT TOP 1 ValoreParametro.NascondiVistati.value('.','bit')
					  FROM @xParametri.nodes('/Parametri/NascondiVistati') as ValoreParametro(NascondiVistati))
	SET @bNascondiVistati=ISNULL(@bNascondiVistati,0)
	
		SET @sCodFiltroSpeciale=(SELECT TOP 1 ValoreParametro.CodFiltroSpeciale.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodFiltroSpeciale') as ValoreParametro(CodFiltroSpeciale))
	
		SET @bOrdinaLetto=(SELECT TOP 1 ValoreParametro.OrdinaLetto.value('.','bit')
					  FROM @xParametri.nodes('/Parametri/OrdinaLetto') as ValoreParametro(OrdinaLetto))
	SET @bOrdinaLetto=ISNULL(@bOrdinaLetto,0)				
					  								
	    DECLARE @xTimeStamp AS XML	

    SET @xTimeStamp=(SELECT TOP 1 ValoreParametro.TS.query('.')
					  FROM @xParametri.nodes('/Parametri/TimeStamp') as ValoreParametro(TS))
	
	SET @xTimeStamp=CONVERT(XML,
						'<Parametri>' + CONVERT(varchar(max),@xTimeStamp) +
						'</Parametri>')		
	
				
		SET @nTemp=(SELECT COUNT(*) FROM T_AssRuoliModuli
				WHERE CodModulo='ConsegneP_Inserisci'	
				AND CodRuolo=@sCodRuolo)
	IF @nTemp>=1 
		SET @bInserisci=1
	ELSE
		SET @bInserisci=0	

		SET @nTemp=(SELECT COUNT(*) FROM T_AssRuoliModuli
				WHERE CodModulo='ConsegneP_Inserisci'	
				AND CodRuolo=@sCodRuolo)
	IF @nTemp>=1 
		SET @bModifica=1
	ELSE
		SET @bModifica=0		
			
								
		SET @nTemp=(SELECT COUNT(*) FROM T_AssRuoliModuli
				WHERE CodModulo='ConsegneP_Cancella'	
				AND CodRuolo=@sCodRuolo)
	IF @nTemp>=1 
		SET @bCancella=1
	ELSE
		SET @bCancella=0

		SET @nTemp=(SELECT COUNT(*) FROM T_AssRuoliModuli
				WHERE CodModulo='ConsegneP_Visto'	
				AND CodRuolo=@sCodRuolo)
	IF @nTemp>=1 
		SET @bVisto=1
	ELSE
		SET @bVisto=0

					
					
	CREATE TABLE #tmpUARuolo
		(
			CodUA VARCHAR(20) COLLATE Latin1_General_CI_AS,
			Descrizione VARCHAR(255) COLLATE Latin1_General_CI_AS
		)
	
	IF @uIDConsegnaPaziente IS NULL 
		BEGIN		
			DECLARE @xTmp AS XML
			SET  @xTmp=CONVERT(XML,'<Parametri><CodRuolo>'+ @sCodRuolo + '</CodRuolo></Parametri>')		
			INSERT #tmpUARuolo EXEC MSP_SelUADaRuolo @xTmp		
			CREATE INDEX IX_CodUA ON #tmpUARuolo (CodUA)
		END

								   
	CREATE TABLE #tmpFiltriConsegne
		(
			IDConsegna UNIQUEIDENTIFIER,				
			CodTipoConsegna VARCHAR(20) COLLATE Latin1_General_CI_AS,
			CodStatoConsegna VARCHAR(20) COLLATE Latin1_General_CI_AS,
			CodUA VARCHAR(20) COLLATE Latin1_General_CI_AS,
		)
		
	SET @sSQL='		INSERT INTO #tmpFiltriConsegne
						(
							IDConsegna,
							CodTipoConsegna,
							CodStatoConsegna,
							CodUA								
						)
					SELECT DISTINCT 
						M.ID,
						M.CodTipoConsegnaPaziente,
						M.CodStatoConsegnaPaziente,
						M.CodUA
				    FROM
						 T_MovConsegnePazienteRuoli MR WITH(NOLOCK) 
							INNER JOIN T_MovConsegnePaziente M WITH(NOLOCK) 
									ON (M.ID = MR.IDConsegnaPaziente)							
							LEFT JOIN T_MovPazienti P WITH(NOLOCK)									
									ON (M.IDEpisodio = P.IDEpisodio)
							LEFT JOIN T_MovEpisodi EPI WITH(NOLOCK)
									ON (M.IDEpisodio = EPI.ID)
							LEFT JOIN T_UnitaAtomiche UA WITH(NOLOCK)								
									ON (M.CodUA = UA.Codice)
							LEFT JOIN T_Ruoli RI WITH(NOLOCK)										
									ON (M.CodRuoloInserimento = RI.Codice)
							LEFT JOIN T_TipoConsegnaPaziente TC WITH(NOLOCK)						
									ON (M.CodTipoConsegnaPaziente = TC.Codice)
							LEFT JOIN T_StatoConsegnaPaziente SC WITH(NOLOCK)						
									ON (M.CodStatoConsegnaPaziente = SC.Codice)
							LEFT JOIN T_StatoConsegnaPazienteRuoli SCR WITH(NOLOCK)					
									ON (MR.CodStatoConsegnaPazienteRuolo = SCR.Codice)
							LEFT JOIN T_Ruoli R WITH(NOLOCK)										
									ON (MR.CodRuolo = R.Codice)									 
					'

	IF @uIDConsegnaPaziente IS NULL
		BEGIN
			SET @sSQL=@sSQL +'
					INNER JOIN #tmpUARuolo UAR
						ON M.CodUA = UAR.CodUA '
		END	

				
	SET @sWhere = ''						

		IF @uIDConsegnaPaziente IS NOT NULL
		BEGIN
			SET @sTmp = ' AND M.ID=''' + convert(varchar(50),@uIDConsegnaPaziente) +''''
			SET @sWhere = @sWhere + @sTmp								
		END		
	
		IF @uIDEpisodio IS NOT NULL
		BEGIN
			SET @sTmp = ' AND M.IDEpisodio=''' + convert(varchar(50),@uIDEpisodio) +''''
			SET @sWhere = @sWhere + @sTmp								
		END		

		IF @sCodTipoConsegnaPaziente NOT IN ('','''Tutti''')
		BEGIN	
						SET @sTmp = ' AND 			
							 M.CodTipoConsegnaPaziente IN ('+ @sCodTipoConsegnaPaziente + ')
						'  				
			SET @sWhere = @sWhere + @sTmp														
		END   
		
		IF @sCodStatoConsegnaPaziente NOT IN ('','''Tutti''')
		BEGIN	
						SET @sTmp = ' AND 			
							 M.CodStatoConsegnaPaziente IN ('+ @sCodStatoConsegnaPaziente + ')
						'  				
			SET @sWhere = @sWhere + @sTmp														
		END
		
	IF @sCodUA NOT IN ('','''Tutti''')
		BEGIN	
						SET @sTmp = ' AND 			
							 M.CodUA IN ('+ @sCodUA + ')
						'  				
			SET @sWhere = @sWhere + @sTmp														
		END   

		IF @uIDConsegnaPaziente IS NULL
		BEGIN
			SET @sTmp = ' AND 			
							 M.CodStatoConsegnaPaziente NOT IN (''CA'')
						'  				
			SET @sWhere = @sWhere + @sTmp	
		END				

		IF @sCodRuolo IS NOT NULL
		BEGIN
			SET @sTmp = ' AND
							(M.CodRuoloInserimento = ''' + @sCodRuolo + ''' OR MR.CodRuolo = ''' + @sCodRuolo + ''')
						'
			SET @sWhere = @sWhere + @sTmp	
		END					

			
	 IF @dDataInizio IS NOT NULL OR @dDataFine IS NOT NULL
				SET @sWhere = @sWhere + ' AND ('						
		IF @dDataInizio IS NOT NULL 
		BEGIN						
			SET @sTmp= CASE 
							WHEN @dDataFine IS NULL 
									THEN ' (M.DataInserimento = CONVERT(datetime,'''  + convert(varchar(20),@dDataInizio,120) +''',120)
									       )'										ELSE		 ' (M.DataInserimento >= CONVERT(datetime,'''  + convert(varchar(20),@dDataInizio,120) +''',120)'	
																				END	
			SET @sWhere = @sWhere + @sTmp								
		END

		IF @dDataFine IS NOT NULL 
		BEGIN
			SET @sWhere= @sWhere + ' AND '												
			IF @dDataFine IS NULL 
				SET @sWhere= @sWhere + '('																														
			SET @sTmp= ' M.DataInserimento <= CONVERT(datetime,'''  + convert(varchar(20),@dDataFine,120) +''',120)'
			SET @sWhere= @sWhere + @sTmp																	
			SET @sWhere= @sWhere + ')'															
		END					
	
		IF (@dDataInizio IS NOT NULL OR @dDataFine IS NOT NULL)
		BEGIN
			SET @sWhere = @sWhere + ' )'
		END

		IF ISNULL(@sFiltroGenerico,'') <> ''
		BEGIN
						SET @sWhere = @sWhere + ' AND ('

			SET @sWhere = @sWhere + ' P.Cognome like ''%' + @sFiltroGenerico + '%'''
			SET @sWhere = @sWhere + ' OR P.Nome like ''%' + @sFiltroGenerico + '%'''	
			SET @sWhere = @sWhere + ' OR P.Cognome + '' '' + P.Nome like ''%' + @sFiltroGenerico + '%'''	
			SET @sWhere = @sWhere + ' OR P.Nome + '' '' + P.Cognome like ''%' + @sFiltroGenerico + '%'''	
			SET @sWhere = @sWhere + ' OR P.CodiceFiscale like ''%' + @sFiltroGenerico + '%'''		

			SET @sWhere = @sWhere + ' OR EPI.NumeroNosologico like ''%' + @sFiltroGenerico + '%'''	
			SET @sWhere = @sWhere + ' OR EPI.NumeroNosologico like ''%' + dbo.MF_ModificaNosologico(@sFiltroGenerico)+ '%'''	
			SET @sWhere = @sWhere + ' OR EPI.NumeroListaAttesa like ''%' + @sFiltroGenerico + '%'''	
					
						
						SET @sWhere = @sWhere + '     )'
		END

		IF ISNULL(@sCodFiltroSpeciale,'') <> ''
	BEGIN
		SET @sTmp=(SELECT SQL FROM T_FiltriSpeciali WHERE Codice=@sCodFiltroSpeciale)
		IF ISNULL(@sTmp,'') <> '' 
		BEGIN						
			SET @sWhere= @sWhere + ' AND ' + 	@sTmp + ''
		END
	END

		IF ISNULL(@sWhere,'') <> ''
		BEGIN	
			SET @sSQL = @sSQL + ' WHERE ' + RIGHT(@sWhere,len(@sWhere)-5)
		END	
	ELSE
		BEGIN	
			SET @sSQL = @sSQL + ' WHERE 0=1'
		END	
										 		
 	PRINT @sSQL
	EXEC (@sSQL)			
								
					
	IF @bNascondiVistati=1
		BEGIN			
								
			DELETE FROM #tmpFiltriConsegne
			WHERE 
				CodStatoConsegna NOT IN ('AN') AND
				IDConsegna
					IN (
						SELECT VS.IDConsegnaPaziente
						FROM 
							(
							SELECT IDConsegnaPaziente, COUNT(*) AS QTAVistata
							FROM
								T_MovConsegnePazienteRuoli MR					
							WHERE 
								CodStatoConsegnaPazienteRuolo ='VS'
							GROUP BY IDConsegnaPaziente
							) VS

						INNER JOIN
							(SELECT IDConsegnaPaziente, COUNT(*) AS QTADaVistare
								FROM
								T_MovConsegnePazienteRuoli MR					
							WHERE 
								CodStatoConsegnaPazienteRuolo NOT IN ('CA','AN')
							GROUP BY IDConsegnaPaziente) DV
							
						ON 
							VS.IDConsegnaPaziente=DV.IDConsegnaPaziente
						WHERE
							VS.QTAVistata=DV.QTADaVistare
													)
		
						
			
		END	

					
	SET @sSQL ='
			SELECT 
				M.ID, 
				RTRIM(LTRIM(					
					'
					+
					CASE 
						WHEN @bOrdinaLetto=1 THEN ' 
									CASE 
										WHEN ISNULL(LET.CodLetto,'''') = '''' THEN ''''
										ELSE ''['' + LET.CodLetto + ''] '' 
									END	
									+ '
						ELSE ''
					END
					+ 
					' 				
					ISNULL(P.Cognome,'''') + '' '' + ISNULL(P.Nome,'''') + '' ('' + ISNULL(P.Sesso,'''') + '') '' +
									CASE
										WHEN P.DataNascita IS NOT NULL THEN '' ('' + CONVERT(VARCHAR(10),P.DataNascita,105) + '') ''
										ELSE ''''
									END +
																		'' - '' + ISNULL(EPI.NumeroNosologico,'''') 

					' 
					+
					CASE 
						WHEN @bOrdinaLetto=0 THEN '
									  + CASE 
											WHEN ISNULL(LET.CodLetto,'''') = '''' THEN ''''
											ELSE '' ['' + LET.CodLetto + ''] '' 
										END	'																											
						ELSE ''
					END
					+ 				
					'
					)
				) AS DescrPaziente,

				M.CodUA,
				UA.Descrizione AS DescrUA,

				M.CodTipoConsegnaPaziente,
				TC.Descrizione AS DescrTipoConsegnaPaziente,
				
				M.CodStatoConsegnaPaziente,
				SC.Descrizione AS DescrStatoConsegnaPaziente,
				
				M.CodUtenteInserimento,
				LI.Descrizione AS DescrUtenteInserimento,
				
				M.CodUtenteUltimaModifica,
				COALESCE(LM.Descrizione,LI.Descrizione) AS DescrUtenteUltimaModifica,
				
				M.CodUtenteAnnullamento,
				LM.Descrizione AS DescrUtenteAnnullamento,		
				
				S.ID AS IDScheda,
				S.CodScheda,
				S.Versione,
				S.AnteprimaRTF,
				
				M.DataInserimento,
				COALESCE(M.DataUltimaModifica,M.DataInserimento) AS DataUltimaModifica,				
				M.DataAnnullamento,								
				M.DataCancellazione,	

				MRL.DescrRuoli,

				CONVERT(varbinary(max),null)  As Icona,
				I.IDIcona,				
				' + CONVERT(VARCHAR(1),@bModifica) + '		
							&			
																		CASE 
										WHEN ISNULL(M.CodStatoConsegnaPaziente,'''') IN (''IC'') AND											 
											 M.CodRuoloInserimento = ''' + @sCodRuolo + ''' THEN 1	
										ELSE 0				 
									END 
							 AS PermessoModifica,				

																CASE 
										WHEN ISNULL(M.CodStatoConsegnaPaziente,'''') IN (''IC'') AND 
											 ISNULL(MVS.QtaVistati,0) > 0 AND 
											 M.CodRuoloInserimento = ''' + @sCodRuolo + ''' THEN 1 
										ELSE 0				 
									END 							
							AS PermessoAnnulla,

				' + CONVERT(VARCHAR(1),@bCancella) + ' 			
							&		
																		CASE 
										WHEN ISNULL(M.CodStatoConsegnaPaziente,'''') IN (''IC'') AND 
											ISNULL(MVS.QtaVistati,0) = 0 AND
											M.CodRuoloInserimento = ''' + @sCodRuolo + '''THEN 1 
										ELSE 0				 
									END 							
							AS PermessoCancella,

				' + CONVERT(VARCHAR(1),@bVisto) + ' 	
							&						
																		CASE 
										WHEN ISNULL(M.CodStatoConsegnaPaziente,'''') IN (''IC'') AND
											MVSMIEI.QtaDaVistare > 0 THEN 1 
										ELSE 0				 
									END 
							AS PermessoVisto,

			    MVSALL.InfoVistati
				+
					CASE 
						WHEN M.CodStatoConsegnaPaziente =''AN'' THEN
						'' - Annullata da '' +  LAN.Descrizione + '' il '' + 
							Convert(varchar(20),M.DataAnnullamento,103) + '' '' + Convert(varchar(5),M.DataAnnullamento,114) 
						ELSE ''''
					END 
				AS Info
			FROM 				
				T_MovConsegnePaziente M WITH(NOLOCK)						

								LEFT JOIN									
					(SELECT 
						M.ID AS IDConsegnaPaziente,
						STRING_AGG(R.Descrizione,'', '') As DescrRuoli
					 FROM T_MovConsegnePaziente M
						LEFT JOIN
							T_MovConsegnePazienteRuoli MR
								ON M.ID=MR.IDConsegnaPaziente
						LEFT JOIN 
							T_Ruoli R
								ON MR.CodRuolo = R.Codice
					WHERE MR.CodStatoConsegnaPazienteRuolo NOT IN (''AN'',''CA'')
					GROUP BY M.ID) AS MRL
					ON 
						M.ID=MRL.IDConsegnaPaziente

								LEFT JOIN									
					(SELECT 
						M.ID AS IDConsegnaPaziente,
						COUNT(*) AS QtaVistati
					 FROM T_MovConsegnePaziente M
						LEFT JOIN
							T_MovConsegnePazienteRuoli MR
								ON M.ID=MR.IDConsegnaPaziente											
					WHERE MR.CodStatoConsegnaPazienteRuolo IN (''VS'')
					GROUP BY M.ID) AS MVS					
					ON
						M.ID=MVS.IDConsegnaPaziente

								LEFT JOIN									
					(SELECT 
						M.ID AS IDConsegnaPaziente,
						COUNT(*) AS QtaDaVistare
					 FROM T_MovConsegnePaziente M
						LEFT JOIN
							T_MovConsegnePazienteRuoli MR
								ON M.ID=MR.IDConsegnaPaziente
					WHERE 
						MR.CodStatoConsegnaPazienteRuolo IN (''IC'') AND
						MR.CodRuolo =''' + @sCodRuolo + '''
					GROUP BY M.ID) AS MVSMIEI					
					ON
					 M.ID=MVSMIEI.IDConsegnaPaziente

								LEFT JOIN									
					(SELECT 
						M.ID AS IDConsegnaPaziente,
						COUNT(*) AS QtaVistati,						
						MAX(LV.Descrizione) AS UltimoUtenteVisione,				
						MAX(DataVisione) as UltimaDataVisione
					 FROM T_MovConsegnePaziente M
						LEFT JOIN
							T_MovConsegnePazienteRuoli MR
								ON M.ID=MR.IDConsegnaPaziente	
						LEFT JOIN
							T_Login LV
								ON MR.CodUtenteVisione=LV.Codice					
					WHERE 
						MR.CodStatoConsegnaPazienteRuolo IN (''VS'') AND
						MR.CodRuolo =''' + @sCodRuolo + '''
					GROUP BY M.ID) AS MVSRUO
					ON
						M.ID=MVSRUO.IDConsegnaPaziente

								LEFT JOIN									
					(SELECT 						 
						M.ID AS IDConsegnaPaziente,
						''Vistata da '' + STRING_AGG(ISNULL(LVA.Descrizione,'''')  + 
													 '' il '' + Convert(varchar(20), MR.DataVisione,103) + '' '' 
															  + Convert(varchar(5),MR.DataVisione,114)
													,'' - '') 
						  AS InfoVistati
					 FROM T_MovConsegnePaziente M
						LEFT JOIN
							T_MovConsegnePazienteRuoli MR
								ON M.ID=MR.IDConsegnaPaziente	
						LEFT JOIN
							T_Login LVA
								ON MR.CodUtenteVisione=LVA.Codice					
					WHERE 
						MR.CodStatoConsegnaPazienteRuolo IN (''VS'') 
					GROUP BY M.ID) AS MVSALL
					ON
						M.ID=MVSALL.IDConsegnaPaziente

				LEFT JOIN T_MovPazienti P WITH(NOLOCK)
							ON (M.IDEpisodio = P.IDEpisodio)
				LEFT JOIN T_MovEpisodi EPI WITH(NOLOCK)
							ON (M.IDEpisodio = EPI.ID)
				LEFT JOIN T_UnitaAtomiche UA WITH(NOLOCK)
							ON (M.CodUA = UA.Codice)
				LEFT JOIN T_Ruoli RI WITH(NOLOCK)
							ON (M.CodRuoloInserimento = RI.Codice)
				LEFT JOIN T_TipoConsegnaPaziente TC WITH(NOLOCK)
							ON (M.CodTipoConsegnaPaziente = TC.Codice)
				LEFT JOIN T_StatoConsegnaPaziente SC WITH(NOLOCK)
							ON (M.CodStatoConsegnaPaziente = SC.Codice)				
				LEFT JOIN T_Login LI
							ON M.CodUtenteInserimento = LI.Codice
				LEFT JOIN T_Login LM
							ON M.CodUtenteUltimaModifica = LM.Codice
				LEFT JOIN T_Login LA
							ON M.CodUtenteUltimaModifica = LA.Codice
				LEFT JOIN T_Login LAN
							ON M.CodUtenteAnnullamento = LAN.Codice
				LEFT JOIN T_MovSchede S
						ON (S.CodEntita=''CSP'' AND S.IDEntita=M.ID AND S.Storicizzata=0)
				LEFT JOIN
						(SELECT
							IDNum AS IDIcona,
							CodTipo,
							CodStato,
														CONVERT(varbinary(max),null)  As Icona
							FROM T_Icone WITH(NOLOCK)
							WHERE CodEntita=''CSP''
						) AS I
					ON (M.CodTipoConsegnaPaziente = I.CodTipo AND
						M.CodStatoConsegnaPaziente = I.CodStato
						)							
				LEFT JOIN 
					(SELECT IDEpisodio,CodLetto
						FROM T_MovTrasferimenti TR
							INNER JOIN 
								Q_SelUltimoTrasferimentoAttivo UTA
							ON TR.ID=UTA.ID
						) LET
						ON LET.IDEpisodio=M.IDEpisodio	
				WHERE
					EXISTS (SELECT TMP.IDConsegna 
							FROM
								#tmpFiltriConsegne TMP	
							WHERE 
								TMP.IDConsegna=M.ID)
		'
	
			
	SET @sOrderBy = ' ORDER BY M.DataInserimento DESC, M.CodTipoConsegnaPaziente DESC '
	SET @sSQL = @sSQL + @sOrderBy
	
		PRINT @sSQL
	
	EXEC (@sSQL)										

					IF @bDatiEstesi=1
		BEGIN
			SELECT CP.Codice,CP.Descrizione
			FROM
				(SELECT DISTINCT codVoce AS Codice
				from T_AssRuoliAzioni
				where 
					CodEntita='CSP' AND 
					CodRuolo= @sCodRuolo
				) TC
			LEFT JOIN
				T_TipoConsegnaPaziente CP
					ON TC.Codice=CP.Codice
		ORDER BY 
			CP.Descrizione
		END 	
					
					IF @bDatiEstesi=1
		BEGIN		
			SELECT Codice,Descrizione
			FROM T_UnitaAtomiche A
			WHERE
				
				EXISTS 
					(SELECT *
					 FROM T_AssUAEntita		ASS				  
							INNER JOIN
						
								(SELECT DISTINCT codVoce AS Codice
									from T_AssRuoliAzioni
									where 
										CodEntita='CSP' AND 
										CodRuolo= @sCodRuolo
								) TC
							ON (ASS.CodEntita = 'CSP' AND
								TC.Codice = ASS.CodVoce AND
								ASS.CodUA = A.Codice
								)	
					)	AND
				EXISTS
					(SELECT CodUA
					 FROM #tmpUARuolo TMP
					 WHERE TMP.CodUA=A.Codice)
			 ORDER BY 
				A.Descrizione
					 		
		END 		

				
	EXEC MSP_InsMovTimeStamp @xTimeStamp	

				DROP TABLE #tmpUARuolo
	DROP TABLE #tmpFiltriConsegne 									

END











