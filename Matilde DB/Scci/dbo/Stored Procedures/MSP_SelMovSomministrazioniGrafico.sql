CREATE PROCEDURE [dbo].[MSP_SelMovSomministrazioniGrafico](@xParametri AS XML)
AS
BEGIN
	   	
	   	
						
		DECLARE @uIDPaziente AS UNIQUEIDENTIFIER	
	DECLARE @uIDEpisodio AS UNIQUEIDENTIFIER
	DECLARE @bDatiEstesi AS Bit		
	DECLARE @dDataErogazioneInizio AS DATETIME
	DECLARE @dDataErogazioneFine AS DATETIME
	DECLARE @sCodTipoPrescrizione AS VARCHAR(MAX)
	DECLARE @sIDPrescrizione AS VARCHAR(MAX)	
	
	DECLARE @sCodRuolo AS VARCHAR(20)		
	DECLARE @sDataTmp AS VARCHAR(20)	
		
		DECLARE @sGUID AS VARCHAR(Max)
	DECLARE @nTemp AS INTEGER
	DECLARE @xTmpTS AS XML
	DECLARE @sOrderby AS VARCHAR(MAX)						
	
	DECLARE @sCodTipoTaskDaPrescrizione AS VARCHAR(20)
		
		SET @sCodRuolo=(SELECT TOP 1 ValoreParametro.CodRuolo.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/TimeStamp/CodRuolo') as ValoreParametro(CodRuolo))
	SET @sCodRuolo=ISNULL(@sCodRuolo,'')

		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDPaziente.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDPaziente') as ValoreParametro(IDPaziente))
	IF 	ISNULL(@sGUID,'') <> '' SET @uIDPaziente=CONVERT(UNIQUEIDENTIFIER,	@sGUID)	
	
		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDEpisodio.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDEpisodio') as ValoreParametro(IDEpisodio))
	IF 	ISNULL(@sGUID,'') <> '' SET @uIDEpisodio=CONVERT(UNIQUEIDENTIFIER,	@sGUID)	
	
		
		SET @bDatiEstesi=(SELECT TOP 1 ValoreParametro.DatiEstesi.value('.','bit')
					  FROM @xParametri.nodes('/Parametri/DatiEstesi') as ValoreParametro(DatiEstesi))
	SET @bDatiEstesi=ISNULL(@bDatiEstesi,0)
				
		SET @sDataTmp=(SELECT TOP 1 ValoreParametro.DataErogazioneInizio.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/DataErogazioneInizio') as ValoreParametro(DataErogazioneInizio))					  
	SET @sDataTmp=ISNULL(@sDataTmp,'')
	IF @sDataTmp<> '' 
		BEGIN
						SET @sDataTmp= SUBSTRING(@sDataTmp,4,2) + '-' 
						+ LEFT(@sDataTmp,2) + '-' +
						+ SUBSTRING(@sDataTmp,7,4) +
						' ' + RIGHT(@sDataTmp,5)
			IF ISDATE(@sDataTmp)=1
				SET	@dDataErogazioneInizio=CONVERT(DATETIME,@sDataTmp,120)						
			ELSE
				SET	@dDataErogazioneInizio =NULL			
		END		
	
		SET @sDataTmp=(SELECT TOP 1 ValoreParametro.DataErogazioneFine.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/DataErogazioneFine') as ValoreParametro(DataErogazioneFine))
	SET @sDataTmp=ISNULL(@sDataTmp,'')
	IF @sDataTmp<> '' 
		BEGIN
						SET @sDataTmp= SUBSTRING(@sDataTmp,4,2) + '-' 
						+ LEFT(@sDataTmp,2) + '-' +
						+ SUBSTRING(@sDataTmp,7,4) +
						' ' + RIGHT(@sDataTmp,5)
			IF ISDATE(@sDataTmp)=1
				SET	@dDataErogazioneFine=CONVERT(DATETIME,@sDataTmp,120)						
			ELSE
				SET	@dDataErogazioneFine =NULL		
		END 				

		SET @sCodTipoPrescrizione=''
	SELECT	@sCodTipoPrescrizione =  @sCodTipoPrescrizione +
														CASE 
								WHEN @sCodTipoPrescrizione='' THEN ''
								ELSE  ','
							END + ''''
						  + ValoreParametro.CodTipoPrescrizione.value('.','VARCHAR(1800)')
						  + ''''
						 FROM @xParametri.nodes('/Parametri/CodTipoPrescrizione') as ValoreParametro(CodTipoPrescrizione)


	SET @sCodTipoPrescrizione=LTRIM(RTRIM(@sCodTipoPrescrizione))
	IF	@sCodTipoPrescrizione='''''' SET @sCodTipoPrescrizione=''
	SET @sCodTipoPrescrizione=UPPER(@sCodTipoPrescrizione)

		SET @sIDPrescrizione=''
	SELECT	@sIDPrescrizione =  @sIDPrescrizione +
														CASE 
								WHEN @sIDPrescrizione='' THEN ''
								ELSE  ','
							END + ''''
						  + ValoreParametro.IDPrescrizione.value('.','VARCHAR(1800)')
						  + ''''
						 FROM @xParametri.nodes('/Parametri/IDPrescrizione') as ValoreParametro(IDPrescrizione)


	SET @sIDPrescrizione=LTRIM(RTRIM(@sIDPrescrizione))
	IF	@sIDPrescrizione='''''' SET @sIDPrescrizione=''
	SET @sIDPrescrizione=UPPER(@sIDPrescrizione)

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
	
		
	

		CREATE TABLE #tmpPazienti
	(
		IDPaziente UNIQUEIDENTIFIER
	)

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
		END		

					
		SET @sCodTipoTaskDaPrescrizione=(SELECT TOP 1 Valore FROM T_Config WHERE ID=33)

				
							   
		DECLARE @sSQL AS VARCHAR(MAX)
	DECLARE @sWhere AS VARCHAR(Max)
	DECLARE @sTmp AS VARCHAR(Max)
	DECLARE @bIgnora AS INTEGER
	
			
	CREATE TABLE #tmpFiltriTaskInfermieristici
		(
			IDTaskInfermieristico UNIQUEIDENTIFIER,				
			IDPrescrizione UNIQUEIDENTIFIER,
			CodTipoPrescrizione VARCHAR(20)  COLLATE Latin1_General_CI_AS
		)
	
	
	SET @sSQL='		INSERT INTO #tmpFiltriTaskInfermieristici(
						IDTaskInfermieristico,						
						IDPrescrizione,
						CodTipoPrescrizione)					
					SELECT '				
	SET @sSQL=@sSQL +				
					' M.ID AS IDTaskInfermieristico, '					
					  		  	
	SET @sSQL=@sSQL + ' CASE
							WHEN CodSistema=''PRF'' THEN M.IDSistema
						ELSE NULL
					    END AS IDPrescrizione,
						PRF.CodTipoPrescrizione
					FROM 
						T_MovTaskInfermieristici M	WITH(NOLOCK)							
							LEFT JOIN T_MovPazienti P WITH(NOLOCK)
								ON (M.IDEpisodio=P.IDEpisodio)
							LEFT JOIN T_MovPrescrizioni PRF
								ON M.IDSistema=PRF.IDString
																	  							 
					'	
								
				
	SET @sWhere=''				
			
		IF @uIDPaziente IS NOT NULL
		BEGIN			
							SET @sTmp= ' AND P.IDPaziente IN (SELECT IDPaziente FROM #tmpPazienti) '	
			SET @sWhere= @sWhere + @sTmp								
		END			

		IF @uIDEpisodio IS NOT NULL
		BEGIN
			SET @sTmp= ' AND M.IDEpisodio=''' + convert(varchar(50),@uIDEpisodio) +''''
			SET @sWhere= @sWhere + @sTmp								
		END	
	
		IF ISNULL(@sCodTipoPrescrizione,'') <> ''
		BEGIN
			SET @sTmp= ' AND PRF.CodTipoPrescrizione IN (' + convert(varchar(MAX),@sCodTipoPrescrizione) +')'
			SET @sWhere= @sWhere + @sTmp								
		END	
	
		IF ISNULL(@sIDPrescrizione,'') <> ''
		BEGIN
			SET @sTmp= ' AND M.IDSistema IN (' + convert(varchar(MAX),@sIDPrescrizione) +')'
			SET @sWhere= @sWhere + @sTmp								
		END	
	
		SET @sTmp=  ' AND 			
						M.CodTipoTaskInfermieristico IN ('''+ @sCodTipoTaskDaPrescrizione + ''')
				'  				
	SET @sWhere= @sWhere + @sTmp														
			
		SET @sTmp=  ' AND 			
						M.CodStatoTaskInfermieristico = ''ER''
				'  				
	SET @sWhere= @sWhere + @sTmp														
			
		IF @dDataErogazioneInizio IS NOT NULL 
		BEGIN			
			SET @sTmp= CASE 
							WHEN @dDataErogazioneFine IS NULL 
									THEN ' AND M.DataErogazione = CONVERT(datetime,'''  + convert(varchar(20),@dDataErogazioneInizio,120) +''',120)'									
							ELSE ' AND M.DataErogazione >= CONVERT(datetime,'''  + convert(varchar(20),@dDataErogazioneInizio,120) +''',120)'	
						END	
			SET @sWhere= @sWhere + @sTmp								
		END

		IF @dDataErogazioneFine IS NOT NULL 
		BEGIN
			SET @sTmp= ' AND M.DataErogazione <= CONVERT(datetime,'''  + convert(varchar(20),@dDataErogazioneFine,120) +''',120)'							
			SET @sWhere= @sWhere + @sTmp								
		END
	

								
	IF ISNULL(@sWhere,'')<> ''
	BEGIN
	
		SET @sSQL=@sSQL +' WHERE ' + RIGHT(@sWhere,len(@sWhere)-5)
	END	
		
	EXEC (@sSQL)			
									
					

		SET @sSQL='	SELECT
					M.ID,
					P.IDPaziente,
					M.IDEpisodio,
					M.IDTrasferimento,
					M.CodSistema,
					M.IDSistema,
					M.IDGruppo,		
					TMP.CodTipoPrescrizione,
					TP.Descrizione AS DescrTipoPrescrizione,
					M.CodUtenteRilevazione,
					L.Descrizione AS DescrUtente,
					M.CodUtenteUltimaModifica,
					ISNULL(LM.Descrizione,L.Descrizione) AS DescrUtenteUltimaModifica,								
					CASE 
						WHEN M.CodStatoTaskInfermieristico IN (''ER'',''AN'',''TR'')  THEN ISNULL(LM.Descrizione,L.Descrizione)
						ELSE ''''
					END AS DescrUtenteErogazioneAnnullamento,
																									
					DataProgrammata,   								
					M.DataErogazione,								
					ISNULL(M.DataUltimaModifica,M.DataEvento) AS DataUltimaModifica,								
					M.Note,
					CASE	
						WHEN ISNULL(M.Note,'''')='''' THEN ''''
						ELSE ISNULL(M.Note,'''') + CHAR(13) + CHAR(10)
					END			
					+ 
					CASE 
						WHEN ISNULL(M.PosologiaEffettiva,'''')='''' THEN ''''
						ELSE ''Posologia Effettiva: '' + ISNULL(M.PosologiaEffettiva,'''')
					END	
					AS NoteCalc,																					
					MS.AnteprimaRTF,			
					MS.AnteprimaTXT,											
					M.PosologiaEffettiva,

																									MS.AnteprimaTXT AS  DescrizioneGrafico						
					'						
						
	SET @sSQL=@sSQL + '
				FROM 
					#tmpFiltriTaskInfermieristici TMP WITH(NOLOCK)
						INNER JOIN T_MovTaskInfermieristici	M	WITH(NOLOCK)
								ON (TMP.IDTaskInfermieristico=M.ID)														
						LEFT JOIN T_MovPazienti P WITH(NOLOCK)
							ON (M.IDEpisodio = P.IDEpisodio) '							
	SET @sSQL=@sSQL + '							 																										
						LEFT JOIN T_Login L WITH(NOLOCK)
							ON (M.CodUtenteRilevazione = L.Codice)	
						LEFT JOIN T_Login LM WITH(NOLOCK)
							ON (M.CodUtenteUltimaModifica = LM.Codice)						
						 		 
						LEFT JOIN 
								(SELECT ID As IDScheda, CodScheda,Versione,IDEntita,AnteprimaRTF, AnteprimaTXT
								 FROM
									T_MovSchede WITH(NOLOCK)
								 WHERE CodEntita = ''WKI'' AND							
									Storicizzata = 0
								) AS MS
							ON MS.IDEntita = M.ID
						LEFT JOIN T_TipoPrescrizione TP WITH(NOLOCK)
							ON (TMP.CodTipoPrescrizione = TP.Codice) 
					  '																
	
		IF ISNULL(@bDatiEstesi,0)=1		
			SET @bIgnora=1						
	ELSE
			SET @bIgnora =0
	
		SET @sOrderBy = ' ORDER BY 					
						DataErogazione ASC '	
			
	SET @sSQL=@sSQL + @sOrderBy
			
 			
 	IF @bIgnora	=0
 		BEGIN 			
			EXEC (@sSQL)
		END	
	
										
	IF @bDatiEstesi=1 
		BEGIN
												CREATE TABLE #tmpMovIDPescrizioni 
			(
				IDPrescrizione UNIQUEIDENTIFIER
			)	
			
			INSERT INTO #tmpMovIDPescrizioni(IDPrescrizione)
			SELECT IDPrescrizione
			FROM #tmpFiltriTaskInfermieristici
			GROUP BY IDPrescrizione
	
						SET @sSQL='SELECT DISTINCT
							MPS.ID AS IDPrescrizione,
							'' ('' + CONVERT(VARCHAR(10),MPS.DataEvento,105) + '') '' + MS.AnteprimaTXT AS Descrizione,
							ISNULL(I.IDNum,0) AS IDIcona,						
							TP.ColoreGrafico,
							MPS.CodViaSomministrazione,
							VIA.Descrizione AS ViaSomministrazione,
							MPS.DataEvento
						FROM 
							#tmpMovIDPescrizioni TMP WITH(NOLOCK)
								INNER JOIN T_MovPrescrizioni MPS WITH(NOLOCK)
									ON (MPS.ID=TMP.IDPrescrizione)
								INNER JOIN								
									T_TipoPrescrizione TP	WITH(NOLOCK)
										ON (MPS.CodTipoPrescrizione=TP.Codice)
								LEFT JOIN
									(SELECT IDNum, CodTipo,Icona48 AS Icona
									 FROM T_Icone WITH(NOLOCK)
									 WHERE 
										CodEntita=''VSM'' AND 
										CodStato=''''
									) AS I
								ON TP.CodViaSomministrazione=I.CodTipo
								LEFT JOIN								
									T_ViaSomministrazione VIA	WITH(NOLOCK)
										ON (MPS.CodViaSomministrazione=VIA.Codice)
								LEFT JOIN 
								(SELECT ID As IDScheda, CodScheda,Versione,IDEntita,AnteprimaRTF, AnteprimaTXT
								 FROM
									T_MovSchede WITH(NOLOCK)
								 WHERE CodEntita = ''PRF'' AND							
									Storicizzata = 0
								) AS MS					
								ON MS.IDEntita=MPS.ID																								
						'			
			SET @sSQL=@sSQL + ' ORDER BY VIA.Descrizione, MPS.DataEvento DESC'
					
		     EXEC (@sSQL)
		     
		     DROP TABLE #tmpMovIDPescrizioni
		END 	
	
				
	EXEC MSP_InsMovTimeStamp @xTimeStamp	

				
	DROP TABLE #tmpFiltriTaskInfermieristici 
	DROP TABLE #tmpUARuolo

		DROP TABLE #tmpPazienti										
				
END











