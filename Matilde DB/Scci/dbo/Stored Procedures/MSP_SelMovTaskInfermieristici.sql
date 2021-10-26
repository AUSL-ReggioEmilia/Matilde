CREATE PROCEDURE [dbo].[MSP_SelMovTaskInfermieristici](@xParametri AS XML)
AS
BEGIN
	   	
	   	
				
		DECLARE @sFiltroGenerico AS Varchar(500) 
	DECLARE @sDataNascita AS VARCHAR(10)
	DECLARE @sCodUA AS Varchar(MAX)
	DECLARE @sCodUO AS Varchar(MAX)
	DECLARE @sCodSettore AS Varchar(MAX)
	
		DECLARE @uIDPaziente AS UNIQUEIDENTIFIER	
	DECLARE @uIDEpisodio AS UNIQUEIDENTIFIER
	DECLARE @uIDTrasferimento AS UNIQUEIDENTIFIER
	DECLARE @uIDTaskInfermieristico AS UNIQUEIDENTIFIER
	DECLARE @nNumRighe AS INTEGER
	DECLARE @bDatiEstesi AS Bit		
	DECLARE @sCodStatoTaskInfermieristico AS VARCHAR(1800)
	DECLARE @sCodTipoTaskInfermieristico AS VARCHAR(1800)
	DECLARE @dDataProgrammataInizioFiltro AS DATETIME
	DECLARE @dDataProgrammataFineFiltro AS DATETIME
	DECLARE @dDataErogazioneInizio AS DATETIME
	DECLARE @dDataErogazioneFine AS DATETIME
	DECLARE @bWorkListTrasversale AS BIT
	DECLARE @bWorkListTrasversaleSoloFiltri AS BIT
	DECLARE @sOrdinamento AS VARCHAR(255)
	
	DECLARE @bVisualizzazioneSintetica AS BIT
	DECLARE @bTaskInRitardo AS BIT
	DECLARE @nGGInRitardo AS INTEGER
	
	DECLARE @sCodRuolo AS VARCHAR(20)	
	
	DECLARE @sDataTmp AS VARCHAR(20)
	
	DECLARE @sCodSistema as VARCHAR(20)
	DECLARE @sIDSistema AS VARCHAR(50)
	DECLARE @sIDGruppo AS VARCHAR(50)

	DECLARE @sNosologico AS Varchar(255) 
	DECLARE @sCodSacca AS Varchar(255) 
	
	DECLARE @sDescrizione AS VARCHAR(500)
	DECLARE @sCodFiltroSpeciale	AS VARCHAR(20)


		DECLARE @sGUID AS VARCHAR(Max)
	DECLARE @nTemp AS INTEGER
	DECLARE @xTmpTS AS XML
	DECLARE @sOrderby AS VARCHAR(MAX)
	
		
	DECLARE @sCodTipoTaskDaPrescrizione AS  VARCHAR(20)
	
	DECLARE @bInserisci AS  BIT
	DECLARE @bModifica AS  BIT
	DECLARE @bCancella AS  BIT
	DECLARE @bCompleta AS  BIT
	DECLARE @bAnnulla AS  BIT
	DECLARE @bSposta AS  BIT
	DECLARE @sCodRuoloSuperUser AS VARCHAR(100)
	
	
		SET @sFiltroGenerico=(SELECT	TOP 1 ValoreParametro.FiltroGenerico.value('.','VARCHAR(500)')
						 FROM @xParametri.nodes('/Parametri/FiltroGenerico') as ValoreParametro(FiltroGenerico))						 
	SET @sFiltroGenerico= ISNULL(@sFiltroGenerico,'')
	SET @sFiltroGenerico=LTRIM(RTRIM(@sFiltroGenerico))
	
		IF ISNULL(@sFiltroGenerico,'') <> ''
		SET @sFiltroGenerico=REPLACE(@sFiltroGenerico,'''','')
	
	
		SET @sDataNascita=(SELECT	TOP 1 ValoreParametro.DataNascita.value('.','VARCHAR(10)')
						 FROM @xParametri.nodes('/Parametri/DataNascita') as ValoreParametro(DataNascita))						 
	SET @sDataNascita= ISNULL(@sDataNascita,'')
	SET @sDataNascita=LTRIM(RTRIM(@sDataNascita))
	
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
	

		SET @sCodUO=''
	SELECT	@sCodUO =  @sCodUO +
														CASE 
								WHEN @sCodUO='' THEN ''
								ELSE  ','
							END + ''''
						  + ValoreParametro.CodUO.value('.','VARCHAR(1800)')
						  + ''''
						 FROM @xParametri.nodes('/Parametri/CodUO') as ValoreParametro(CodUO)	
	SET @sCodUO=LTRIM(RTRIM(@sCodUO))
	IF	@sCodUO='''''' SET @sCodUO=''
	
	
		SET @sCodSettore=''
	SELECT	@sCodSettore =  @sCodSettore +
														CASE 
								WHEN @sCodSettore='' THEN ''
								ELSE  ','
							END + ''''
						  + ValoreParametro.CodSettore.value('.','VARCHAR(1800)')
						  + ''''
						 FROM @xParametri.nodes('/Parametri/CodSettore') as ValoreParametro(CodSettore)	
	SET @sCodSettore=LTRIM(RTRIM(@sCodSettore))
	IF	@sCodSettore='''''' SET @sCodSettore=''	
		
		SET @sCodRuolo=(SELECT TOP 1 ValoreParametro.CodRuolo.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/TimeStamp/CodRuolo') as ValoreParametro(CodRuolo))
	SET @sCodRuolo=ISNULL(@sCodRuolo,'')


		SET @sOrdinamento=(SELECT TOP 1 ValoreParametro.Ordinamento.value('.','VARCHAR(255)')
					  FROM @xParametri.nodes('/Parametri/Ordinamento') as ValoreParametro(Ordinamento))
	SET @sOrdinamento=ISNULL(@sOrdinamento,'')
									  											  					
		
		SET @nTemp=(SELECT COUNT(*) FROM T_AssRuoliModuli
				WHERE CodModulo='WorkL_Inserisci'	
				AND CodRuolo=@sCodRuolo)
	IF @nTemp>=1 
		SET @bInserisci=1
	ELSE
		SET @bInserisci=0	
		
	
		SET @nTemp=(SELECT COUNT(*) FROM T_AssRuoliModuli
				WHERE CodModulo='WorkL_Modifica'	
				AND CodRuolo=@sCodRuolo)
	IF @nTemp>=1 
		SET @bModifica=1
	ELSE
		SET @bModifica=0		
			
		SET @nTemp=(SELECT COUNT(*) FROM T_AssRuoliModuli
				WHERE CodModulo='WorkL_Cancella'	
				AND CodRuolo=@sCodRuolo)
	IF @nTemp>=1 
		SET @bCancella=1
	ELSE
		SET @bCancella=0	
	
		SET @nTemp=(SELECT COUNT(*) FROM T_AssRuoliModuli
				WHERE CodModulo='WorkL_Annulla'	
				AND CodRuolo=@sCodRuolo)
	IF @nTemp>=1 
		SET @bAnnulla=1
	ELSE
		SET @bAnnulla=0
		
		SET @nTemp=(SELECT COUNT(*) FROM T_AssRuoliModuli
				WHERE CodModulo='WorkL_Completa'	
				AND CodRuolo=@sCodRuolo)
	IF @nTemp>=1 
		SET @bCompleta=1
	ELSE
		SET @bCompleta=0	
		
		SET @nTemp=(SELECT COUNT(*) FROM T_AssRuoliModuli
				WHERE CodModulo='WorkL_Sposta'	
				AND CodRuolo=@sCodRuolo)
	IF @nTemp>=1 
		SET @bSposta=1
	ELSE
		SET @bSposta=0			
	
		
		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDPaziente.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDPaziente') as ValoreParametro(IDPaziente))
	IF 	ISNULL(@sGUID,'') <> '' SET @uIDPaziente=CONVERT(UNIQUEIDENTIFIER,	@sGUID)	
	
		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDEpisodio.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDEpisodio') as ValoreParametro(IDEpisodio))
	IF 	ISNULL(@sGUID,'') <> '' SET @uIDEpisodio=CONVERT(UNIQUEIDENTIFIER,	@sGUID)	
	
		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDTrasferimento.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDTrasferimento') as ValoreParametro(IDTrasferimento))
	IF 	ISNULL(@sGUID,'') <> '' SET @uIDTrasferimento=CONVERT(UNIQUEIDENTIFIER,	@sGUID)	
					  				  
		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDTaskInfermieristico.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDTaskInfermieristico') as ValoreParametro(IDTaskInfermieristico))		
	IF 	ISNULL(@sGUID,'') <> '' SET @uIDTaskInfermieristico=CONVERT(UNIQUEIDENTIFIER,	@sGUID)	
					  			  
		SET @nNumRighe=(SELECT	TOP 1 ValoreParametro.NumRighe.value('.','INTEGER')
								FROM @xParametri.nodes('/Parametri/NumRighe') as ValoreParametro(NumRighe))	
	SET @nNumRighe=ISNULL(@nNumRighe,0)
		
		SET @bDatiEstesi=(SELECT TOP 1 ValoreParametro.DatiEstesi.value('.','bit')
					  FROM @xParametri.nodes('/Parametri/DatiEstesi') as ValoreParametro(DatiEstesi))
	SET @bDatiEstesi=ISNULL(@bDatiEstesi,0)

		SET @sCodStatoTaskInfermieristico=''
	SELECT	@sCodStatoTaskInfermieristico =  @sCodStatoTaskInfermieristico +
														CASE 
								WHEN @sCodStatoTaskInfermieristico='' THEN ''
								ELSE  ','
							END + ''''
						  + ValoreParametro.CodStatoTaskInfermieristico.value('.','VARCHAR(1800)')
						  + ''''
						 FROM @xParametri.nodes('/Parametri/CodStatoTaskInfermieristico') as ValoreParametro(CodStatoTaskInfermieristico)
						 
	SET @sCodStatoTaskInfermieristico=LTRIM(RTRIM(@sCodStatoTaskInfermieristico))
	IF	@sCodStatoTaskInfermieristico='''''' SET @sCodStatoTaskInfermieristico=''
	SET @sCodStatoTaskInfermieristico=UPPER(@sCodStatoTaskInfermieristico)

		SET @sCodTipoTaskInfermieristico=''
	SELECT	@sCodTipoTaskInfermieristico =  @sCodTipoTaskInfermieristico +
														CASE 
								WHEN @sCodTipoTaskInfermieristico='' THEN ''
								ELSE  ','
							END + ''''
						  + ValoreParametro.CodTipoTaskInfermieristico.value('.','VARCHAR(1800)')
						  + ''''
						 FROM @xParametri.nodes('/Parametri/CodTipoTaskInfermieristico') as ValoreParametro(CodTipoTaskInfermieristico)
						 
	SET @sCodTipoTaskInfermieristico=LTRIM(RTRIM(@sCodTipoTaskInfermieristico))
	IF	@sCodTipoTaskInfermieristico='''''' SET @sCodTipoTaskInfermieristico=''
	SET @sCodTipoTaskInfermieristico=UPPER(@sCodTipoTaskInfermieristico)
	
		SET @sDataTmp=(SELECT TOP 1 ValoreParametro.DataProgrammataInizio.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/DataProgrammataInizio') as ValoreParametro(DataProgrammataInizio))					  
	SET @sDataTmp=ISNULL(@sDataTmp,'')
	IF @sDataTmp<> '' 
		BEGIN
						SET @sDataTmp= SUBSTRING(@sDataTmp,4,2) + '-' 
						+ LEFT(@sDataTmp,2) + '-' +
						+ SUBSTRING(@sDataTmp,7,4) +
						' ' + RIGHT(@sDataTmp,5)
			IF ISDATE(@sDataTmp)=1
				SET	@dDataProgrammataInizioFiltro=CONVERT(DATETIME,@sDataTmp,120)						
			ELSE
				SET	@dDataProgrammataInizioFiltro =NULL			
		END
		
		
		SET @sDataTmp=(SELECT TOP 1 ValoreParametro.DataProgrammataFine.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/DataProgrammataFine') as ValoreParametro(DataProgrammataFine))
	SET @sDataTmp=ISNULL(@sDataTmp,'')
	IF @sDataTmp<> '' 
		BEGIN
						SET @sDataTmp= SUBSTRING(@sDataTmp,4,2) + '-' 
						+ LEFT(@sDataTmp,2) + '-' +
						+ SUBSTRING(@sDataTmp,7,4) +
						' ' + RIGHT(@sDataTmp,5)
			IF ISDATE(@sDataTmp)=1
				SET	@dDataProgrammataFineFiltro=CONVERT(DATETIME,@sDataTmp,120)						
			ELSE
				SET	@dDataProgrammataFineFiltro =NULL		
		END 		
	
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
	

		SET @bVisualizzazioneSintetica=(SELECT TOP 1 ValoreParametro.VisualizzazioneSintetica.value('.','bit')
					  FROM @xParametri.nodes('/Parametri/VisualizzazioneSintetica') as ValoreParametro(VisualizzazioneSintetica))											
	SET @bVisualizzazioneSintetica=ISNULL(@bVisualizzazioneSintetica,0)
						  
		SET @bTaskInRitardo=(SELECT TOP 1 ValoreParametro.TaskInRitardo.value('.','bit')
					  FROM @xParametri.nodes('/Parametri/TaskInRitardo') as ValoreParametro(TaskInRitardo))											
	SET @bTaskInRitardo=ISNULL(@bTaskInRitardo,0)
		
		
		SET @nGGInRitardo = (SELECT TOP 1 ValoreParametro.GGInRitardo.value('.','INTEGER')
					  FROM @xParametri.nodes('/Parametri/GGInRitardo') as ValoreParametro(GGInRitardo))				
	SET @nGGInRitardo=ISNULL(@nGGInRitardo,0)

		SET @sCodSistema = ''
	SET @sCodSistema=(SELECT	TOP 1 ValoreParametro.CodSistema.value('.','VARCHAR(20)')
						 FROM @xParametri.nodes('/Parametri/CodSistema') as ValoreParametro(CodSistema))						 
	SET @sCodSistema= ISNULL(@sCodSistema,'')
	SET @sCodSistema=LTRIM(RTRIM(@sCodSistema))

		SET @sIDSistema = ''
	SET @sIDSistema=(SELECT	TOP 1 ValoreParametro.IDSistema.value('.','VARCHAR(36)')
						 FROM @xParametri.nodes('/Parametri/IDSistema') as ValoreParametro(IDSistema))						 
	SET @sIDSistema= ISNULL(@sIDSistema,'')
	SET @sIDSistema=LTRIM(RTRIM(@sIDSistema))

		SET @sIDGruppo = ''
	SET @sIDGruppo=(SELECT	TOP 1 ValoreParametro.IDGruppo.value('.','VARCHAR(50)')
						 FROM @xParametri.nodes('/Parametri/IDGruppo') as ValoreParametro(IDGruppo))						 
	SET @sIDGruppo= ISNULL(@sIDGruppo,'')
	SET @sIDGruppo=LTRIM(RTRIM(@sIDGruppo))
	
		SET @bWorkListTrasversale=(SELECT TOP 1 ValoreParametro.WorkListTrasversale.value('.','bit')
					  FROM @xParametri.nodes('/Parametri/WorkListTrasversale') as ValoreParametro(WorkListTrasversale))											
	SET @bWorkListTrasversale=ISNULL(@bWorkListTrasversale,0)
	
		SET @bWorkListTrasversaleSoloFiltri=(SELECT TOP 1 ValoreParametro.WorkListTrasversaleSoloFiltri.value('.','bit')
					  FROM @xParametri.nodes('/Parametri/WorkListTrasversaleSoloFiltri') as ValoreParametro(WorkListTrasversaleSoloFiltri))											
	SET @bWorkListTrasversaleSoloFiltri=ISNULL(@bWorkListTrasversaleSoloFiltri,0)
	
					
		SET @sNosologico = ''
	SET @sNosologico=(SELECT TOP 1 ValoreParametro.Nosologico.value('.','VARCHAR(255)')
						 FROM @xParametri.nodes('/Parametri/Nosologico') as ValoreParametro(Nosologico))
	SET @sNosologico= ISNULL(@sNosologico,'')
	SET @sNosologico=LTRIM(RTRIM(@sNosologico))

		SET @sCodSacca = ''
	SET @sCodSacca=(SELECT TOP 1 ValoreParametro.CodSacca.value('.','VARCHAR(255)')
						 FROM @xParametri.nodes('/Parametri/CodSacca') as ValoreParametro(CodSacca))
	SET @sCodSacca= ISNULL(@sCodSacca,'')
	SET @sCodSacca=LTRIM(RTRIM(@sCodSacca))

		SET @sDescrizione=(SELECT	TOP 1 ValoreParametro.Descrizione.value('.','VARCHAR(500)')
						 FROM @xParametri.nodes('/Parametri/Descrizione') as ValoreParametro(Descrizione))	
	IF ISNULL(@sDescrizione,'') <> ''
		SET @sDescrizione=REPLACE(@sDescrizione,'''','')

	SET @sCodFiltroSpeciale=(SELECT TOP 1 ValoreParametro.CodFiltroSpeciale.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodFiltroSpeciale') as ValoreParametro(CodFiltroSpeciale))

				CREATE TABLE #tmpViaSomministrazione
	(
		Codice VARCHAR(20) COLLATE Latin1_General_CI_AS
	)	
	
	IF @xParametri.exist('/Parametri/ViaSomministrazione')=1
		INSERT INTO #tmpViaSomministrazione(Codice)	
			SELECT 	ValoreParametro.ViaSomministrazione.value('.','VARCHAR(20)')	
				FROM @xParametri.nodes('/Parametri/ViaSomministrazione') as ValoreParametro(ViaSomministrazione)
						

	    DECLARE @xTimeStamp AS XML	

    SET @xTimeStamp=(SELECT TOP 1 ValoreParametro.TS.query('.')
					  FROM @xParametri.nodes('/Parametri/TimeStamp') as ValoreParametro(TS))
	
	SET @xTimeStamp=CONVERT(XML,
						'<Parametri>' + CONVERT(varchar(max),@xTimeStamp) +
						'</Parametri>')
						  											



				
					
		SET @sCodTipoTaskDaPrescrizione=(SELECT TOP 1 Valore FROM T_Config WHERE ID=33)
	
		SET @sCodRuoloSuperUser=(SELECT TOP 1 Valore FROM T_Config WHERE ID=681)	
		 
				IF @bWorkListTrasversale=1 
	BEGIN
		CREATE TABLE #tmpUARuolo
		(
			CodUA VARCHAR(20) COLLATE Latin1_General_CI_AS,
			Descrizione VARCHAR(255) COLLATE Latin1_General_CI_AS
		)

		DECLARE @xTmp AS XML
		SET  @xTmp=CONVERT(XML,'<Parametri><CodRuolo>'+ @sCodRuolo + '</CodRuolo></Parametri>')
		
		INSERT #tmpUARuolo EXEC MSP_SelUADaRuolo @xTmp
		
		CREATE INDEX IX_CodUA ON #tmpUARuolo (CodUA)
	END
		
				
							   
		DECLARE @sSQL AS VARCHAR(MAX)
	DECLARE @sWhere AS VARCHAR(Max)
	DECLARE @sTmp AS VARCHAR(Max)
	DECLARE @bIgnora AS INTEGER
	
			
	CREATE TABLE #tmpFiltriTaskInfermieristici
		(
			IDTaskInfermieristico UNIQUEIDENTIFIER,
			IDTrasferimento UNIQUEIDENTIFIER,
			CodTipoTaskInfermieristico VARCHAR(20) COLLATE Latin1_General_CI_AS,
			CodStatoTaskInfermieristico VARCHAR(20) COLLATE Latin1_General_CI_AS,
			CodUA VARCHAR(20) COLLATE Latin1_General_CI_AS,
			CodUo VARCHAR(20) COLLATE Latin1_General_CI_AS,
			CodSettore VARCHAR(20) COLLATE Latin1_General_CI_AS,
			DescrSettore VARCHAR(255) COLLATE Latin1_General_CI_AS,
			IDSistema VARCHAR(50) COLLATE Latin1_General_CI_AS,			
			IDPrescrizione UNIQUEIDENTIFIER
		)
	
	
	SET @sSQL='		INSERT INTO #tmpFiltriTaskInfermieristici(
						IDTaskInfermieristico,
						IDTrasferimento,
						CodUA,
						CodUO,
						CodSettore,
						DescrSettore,
						CodTipoTaskInfermieristico,
						CodStatoTaskInfermieristico,
						IDSistema,
						IDPrescrizione)					
					SELECT '

		IF @nNumRighe<> 0  SET @sSQL= @sSQL + ' TOP ' + convert(varchar(10),@nNumRighe)+  ' ' 					
	
	SET @sSQL=@sSQL +				
					' M.ID AS IDTaskInfermieristico,
					  M.IDTrasferimento,'
					  
	IF @bWorkListTrasversale=1
			SET @sSQL=@sSQL + '
						TRA.CodUA,
						TRA.CodUO,
						TRA.CodSettore,
						TRA.DescrSettore,
							  '
		ELSE
			SET @sSQL=@sSQL + '
						NULL AS CodUA,
						NULL AS CodUO,
						NULL AS CodSettore,
						NULL AS DescrSettore,
							  '				  
	
	SET @sSQL=@sSQL + 'M.CodTipoTaskInfermieristico,
					   M.CodStatoTaskInfermieristico,
					   M.IDSistema,
					   CASE
						WHEN CodSistema=''PRF'' THEN M.IDSistema
						ELSE NULL
					   END AS IDPrescrizione
					FROM 
						T_MovTaskInfermieristici M	WITH(NOLOCK)
							LEFT JOIN T_MovPazienti P WITH(NOLOCK)
								ON (M.IDEpisodio=P.IDEpisodio)												  							 
					'	
	IF @bWorkListTrasversale=1
		SET @sSQL=@sSQL +				
					'		LEFT JOIN T_MovEpisodi EPI WITH(NOLOCK)
								ON (M.IDEpisodio=EPI.ID)	
							LEFT JOIN T_MovTrasferimenti TRA WITH (NOLOCK)	
								ON (M.IDTrasferimento=TRA.ID)
							LEFT JOIN T_MovCartelle CAR WITH (NOLOCK)
								ON (TRA.IDCartella=CAR.ID) 
					' 

		IF @xParametri.exist('/Parametri/ViaSomministrazione')=1						
		SET @sSQL=@sSQL +				
					'		LEFT JOIN T_MovPrescrizioni MPS WITH(NOLOCK)
								ON (MPS.IDString=M.IDSistema)	' 
								
								
		IF @sDescrizione IS NOT NULL
		SET @sSQL=@sSQL +' LEFT JOIN T_MovSchede MS
								ON (MS.CodEntita=''WKI'' AND
									MS.IDEntita=M.ID AND
									MS.Storicizzata=0
									)
						'							
								
				
	SET @sWhere=''				
		
		IF @uIDPaziente IS NOT NULL
		BEGIN
			SET @sTmp= ' AND P.IDPaziente=''' + convert(varchar(50),@uIDPaziente) +''''
			SET @sWhere= @sWhere + @sTmp								
		END			

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
		
		IF @uIDTaskInfermieristico IS NOT NULL
		BEGIN
			SET @sTmp= ' AND M.ID=''' + convert(varchar(50),@uIDTaskInfermieristico) +''''
			SET @sWhere= @sWhere + @sTmp								
		END		
	
	IF @sCodTipoTaskInfermieristico NOT IN ('','''Tutti''')
		BEGIN	
						SET @sTmp=  ' AND 			
							 M.CodTipoTaskInfermieristico IN ('+ @sCodTipoTaskInfermieristico + ')
						'  				
			SET @sWhere= @sWhere + @sTmp														
		END   
		
		IF @sCodStatoTaskInfermieristico NOT IN ('','''Tutti''')
		BEGIN	
						SET @sTmp=  ' AND 			
							 M.CodStatoTaskInfermieristico IN ('+ @sCodStatoTaskInfermieristico + ')
						'  				
			SET @sWhere= @sWhere + @sTmp														
		END
		
		
	IF @uIDTaskInfermieristico IS NULL
		BEGIN
			SET @sTmp=  ' AND 			
							 M.CodStatoTaskInfermieristico NOT IN (''CA'')
						'  				
			SET @sWhere= @sWhere + @sTmp	
		END				

	IF ISNULL(@sCodSistema, '') <> ''
		BEGIN	
						SET @sTmp=  ' AND M.CodSistema = ''' + @sCodSistema + ''''  				
			SET @sWhere = @sWhere + @sTmp														
		END

	IF ISNULL(@sIDSistema, '') <> ''
		BEGIN	
						SET @sTmp=  ' AND M.IDSistema = ''' + @sIDSistema + ''''  				
			SET @sWhere = @sWhere + @sTmp														
		END
		
	IF ISNULL(@sIDGruppo, '') <> ''
		BEGIN	
						SET @sTmp=  ' AND M.IDGruppo = ''' + @sIDGruppo + ''''  				
			SET @sWhere = @sWhere + @sTmp														
		END

	IF ISNULL(@sNosologico, '') <> ''
		BEGIN	
						SET @sTmp=  ' AND (EPI.NumeroNosologico = ''' + @sNosologico + '''
								OR
								EPI.NumeroNosologico = ''' + dbo.MF_ModificaNosologico(@sNosologico) + '''
								)'  				
			SET @sWhere = @sWhere + @sTmp														
		END

	IF ISNULL(@sCodSacca, '') <> ''
		BEGIN	
						SET @sTmp=  ' AND M.Barcode = ''' + @sCodSacca + ''''  				
			SET @sWhere = @sWhere + @sTmp														
		END

				
	 IF @dDataProgrammataInizioFiltro IS NOT NULL OR @dDataProgrammataInizioFiltro IS NOT NULL
				SET @sWhere= @sWhere + ' AND ('						
		IF @dDataProgrammataInizioFiltro IS NOT NULL 
		BEGIN						
			SET @sTmp= CASE 
							WHEN @dDataProgrammataFineFiltro IS NULL 
									THEN ' (M.DataProgrammata = CONVERT(datetime,'''  + convert(varchar(20),@dDataProgrammataInizioFiltro,120) +''',120)
									       )'										ELSE ' (M.DataProgrammata >= CONVERT(datetime,'''  + convert(varchar(20),@dDataProgrammataInizioFiltro,120) +''',120)'	
																				END	
			SET @sWhere= @sWhere + @sTmp								
		END

		IF @dDataProgrammataFineFiltro IS NOT NULL 
		BEGIN
			SET @sWhere= @sWhere + ' AND '												
			IF @dDataProgrammataInizioFiltro IS NULL 
				SET @sWhere= @sWhere + '('																														
			SET @sTmp= ' M.DataProgrammata <= CONVERT(datetime,'''  + convert(varchar(20),@dDataProgrammataFineFiltro,120) +''',120)'
			SET @sWhere= @sWhere + @sTmp																	
			SET @sWhere= @sWhere + ')'															
		END					

		IF ISNULL(@bTaskInRitardo,0)<>0 	
		BEGIN
			IF (@dDataProgrammataInizioFiltro IS NOT NULL OR @dDataProgrammataFineFiltro IS NOT NULL) 				
					SET @sWhere= @sWhere + ' OR '				ELSE		
					SET @sWhere= @sWhere + ' AND '	
			SET @sWhere= @sWhere + '     (M.CodStatoTaskInfermieristico=''PR'''

			IF @nGGInRitardo = 0 						
				SET @sWhere= @sWhere + ' 	  AND DataProgrammata <=GetDate())'
			ELSE
				SET @sWhere= @sWhere + ' 	  AND DataProgrammata <=GetDate() AND DataProgrammata >=DATEADD(day, -' +  CONVERT(VARCHAR(10), @nGGInRitardo) + ',GetDate()))'

					END	
	
		IF (@dDataProgrammataInizioFiltro IS NOT NULL OR @dDataProgrammataFineFiltro IS NOT NULL)
			SET @sWhere= @sWhere + ' )'						
			
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

				IF @xParametri.exist('/Parametri/ViaSomministrazione')=1
		BEGIN
			SET @sTmp= ' AND 
							1= (CASE 
								WHEN MPS.CodViaSomministrazione IS NULL THEN 1
								WHEN MPS.CodViaSomministrazione IS NOT NULL AND MPS.CodViaSomministrazione IN (SELECT Codice FROM  #tmpViaSomministrazione) THEN 1
								ELSE 0
							   END)
						'
						SET @sWhere= @sWhere + @sTmp										
		END
	
		
		IF ISNULL(@sCodFiltroSpeciale,'') <> ''
	BEGIN
		SET @sTmp=(SELECT SQL FROM T_FiltriSpeciali WHERE Codice=@sCodFiltroSpeciale)
		IF ISNULL(@sTmp,'') <> '' 
		BEGIN						
			SET @sWhere= @sWhere + ' AND ' + 	@sTmp + ''
		END
	END

			
	IF @bWorkListTrasversale=1 
		BEGIN
		
								IF ISNULL(@bWorkListTrasversale,0)=1
				BEGIN
					SET @sTmp= ' AND CAR.CodStatoCartella =''AP'''							
					SET @sWhere= @sWhere + @sTmp
								
				END				
	
								SET @sTmp='	AND M.IDEpisodio IN 
									(SELECT IDEpisodio 
										FROM T_MovTrasferimenti WITH (NOLOCK)
										WHERE CodStatoTrasferimento IN (''AT'',''PR'')										
											AND CodUA IN (SELECT CodUA FROM #tmpUARuolo)
									'	
				
								IF ISNULL(@sCodUA,'') <> ''				
					SET @sTmp= @sTmp + '    AND CodUA IN (' +@sCodUA + ')'				
				
								IF ISNULL(@sCodUO,'') <> ''				
					SET @sTmp= @sTmp + '    AND CodUO IN (' +@sCodUO + ')'				
				
								IF ISNULL(@sCodSettore,'') <> ''				
					SET @sTmp= @sTmp + '    AND CodSettore IN (' +@sCodSettore + ')'	
									
				SET @sTmp=@sTmp + ')'
				SET @sWhere= @sWhere + @sTmp						
				
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
					
					IF @bWorkListTrasversale=1
					 BEGIn
					 	SET @sWhere= @sWhere + '  OR P.CodiceFiscale like ''%' + @sFiltroGenerico + '%'''		
						SET @sWhere= @sWhere + '  OR CAR.NumeroCartella like ''%' + dbo.MF_NumeroCartellaDaBarcode(@sFiltroGenerico) + '%'''		
						SET @sWhere= @sWhere + '  OR M.Barcode = ''' + @sFiltroGenerico + ''''
					END
											SET @sWhere= @sWhere + '     )' 					
				END
		END						
		
		IF @sDescrizione IS NOT NULL
		BEGIN
			SET @sTmp= ' AND (MS.AnteprimaTXT LIKE ''%' + @sDescrizione + '%'' OR M.Barcode = ''' + @sDescrizione + ''')'
			SET @sWhere= @sWhere + @sTmp
		END

						IF ISNULL(@sWhere,'')<> ''
	BEGIN
	
		SET @sSQL=@sSQL +' WHERE ' + RIGHT(@sWhere,len(@sWhere)-5)
	END	
				
	SET @sOrderBy = ' ORDER BY M.DataProgrammata, M.CodTipoTaskInfermieristico DESC '		
	SET @sSQL=@sSQL + @sOrderBy 	 	
 		 	
	EXEC (@sSQL)			
									
					
		IF ISNULL(@bVisualizzazioneSintetica,0)=0
		BEGIN
								
				SET @sSQL='	SELECT
								M.ID,
								P.IDPaziente,
								M.IDEpisodio,
								M.IDTrasferimento,
								TRA.CodUA,
						  '
				IF @bWorkListTrasversale=1				  		
					SET @sSQL = @sSQL + '
									ISNULL(P.Cognome,'''') + '' '' + ISNULL(P.Nome,'''') + '' ('' + ISNULL(P.Sesso,'''') + '') '' +
									CASE
										WHEN P.DataNascita IS NOT NULL THEN '' ('' + CONVERT(VARCHAR(10),P.DataNascita,105) + '') ''
										ELSE ''''
									END +
																		'' - '' + ISNULL(EPI.NumeroNosologico,'''') 
								AS DescrPaziente, 						
								'
								
					SET @sSQL = @sSQL + '						
								M.DataEvento,
								M.DataEventoUTC,
								M.CodSistema,
								M.IDSistema,
								M.IDGruppo,
								M.IDTaskIniziale,
								M.CodTipoTaskInfermieristico AS CodTipo,
								T.Descrizione 							
								AS DescTipo, 								
								M.CodStatoTaskInfermieristico AS CodStato,
								S.Descrizione 	AS DescStato, 	
								S.Descrizione +															
								CASE 
									WHEN M.CodStatoTaskInfermieristico IN (''ER'',''AN'',''TR'') THEN 
										CHAR(13) + CHAR(10) + ''da: '' + ISNULL(LM.Descrizione,L.Descrizione) 									
										 + CHAR(13) + CHAR(10) + ''il: '' + Convert(varchar(20),M.DataErogazione,105) + '' '' +  Convert(varchar(5),M.DataErogazione,108) 										
									ELSE ''''
								END 
								AS DescStatoEsteso, 
								
								M.CodTipoRegistrazione,
								M.CodProtocollo,
								M.CodProtocolloTempo,
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
																
								CASE 
									WHEN M.CodStatoTaskInfermieristico IN (''ER'',''AN'',''TR'')  THEN ISNULL(DataErogazione,M.DataProgrammata)
									ELSE DataProgrammata
								END AS DataRiferimento,
								
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
								M.DescrizioneFUT,	
								M.Sottoclasse,	
													
								MS.IDScheda,
								MS.CodScheda,
								MS.Versione,
								MS.AnteprimaRTF,			
								MS.AnteprimaTXT,			
								CASE 
										WHEN ISNULL(M.CodTipoRegistrazione,'''') =''A'' THEN 1 
										ELSE 0
								END AS PermessoBloccato,	
								
								' + CONVERT(VARCHAR(1),@bModifica) + ' 
																		&
									CASE 
										WHEN ISNULL(M.CodStatoTaskInfermieristico,'''') IN (''PR'') THEN 1 
										ELSE 0				 
									END
									& 
									CASE 
										WHEN ISNULL(M.CodTipoRegistrazione,'''') NOT IN (''A'') THEN 1 
										ELSE 0	
									END	
					 			 AS PermessoModifica,
					 			 
					 			' + CONVERT(VARCHAR(1),@bModifica) + ' 									
									&
									CASE 
										WHEN ISNULL(M.CodStatoTaskInfermieristico,'''') IN (''PR'') THEN 1 
										ELSE 0				 
									END									
					 			 AS PermessoModificaOrario,
								 	 
								' + CONVERT(VARCHAR(1),@bCompleta) + '
																				&
										CASE 
											WHEN ISNULL(M.CodStatoTaskInfermieristico,'''') IN (''PR'') THEN 1 
											ELSE 0				 
										END
								 AS PermessoErogazione,
							
								
								' + CONVERT(VARCHAR(1),@bAnnulla) + ' 
										 										 &
										CASE 
											WHEN ISNULL(M.CodStatoTaskInfermieristico,'''') IN (''PR'') THEN 1 
											ELSE 0				 
										END										
										AS PermessoAnnulla,		
										
								(' 
									+ CONVERT(VARCHAR(1),@bCancella) + '
																				&
										CASE 
											WHEN ISNULL(M.CodStatoTaskInfermieristico,'''') IN (''PR'') THEN 1 
											ELSE 0	
										END	 
										& 
										CASE 
											WHEN ISNULL(M.CodTipoRegistrazione,'''') NOT IN (''A'') THEN 1 
											ELSE 0	
										END	 
										&										
										CASE 
											WHEN ISNULL(CodSistema,'''')=''PRF'' THEN 0
											ELSE 1
										END
								) 
								|
								(' + 
																			CASE 
											WHEN ISNULL(@sCodRuolo,'')=ISNULL(@sCodRuoloSuperUser,'.') THEN '1'
											ELSE '0'
										END
									  +
									'  										
								)
								AS PermessoCancella,
								
								' + CONVERT(VARCHAR(1),@bInserisci) + ' & ISNULL(AzINSPermesso,0) 
										& 										
										CASE 
											WHEN ISNULL(M.CodTipoTaskInfermieristico,'''') NOT IN (''' + @sCodTipoTaskDaPrescrizione + ''') THEN 1
											ELSE 0
										END	
										AS PermessoCopia,		
								'  + CONVERT(VARCHAR(1),@bSposta) +  
								'		& 										
										CASE 
											WHEN ISNULL(M.CodStatoTaskInfermieristico,'''') IN (''PR'') THEN 1 
											ELSE 0
										END	
										AS PermessoSposta,
								M.PosologiaEffettiva,
								M.Alert,
								M.Barcode,
								ISNULL(T.Anticipo,0) AS Anticipo
								'			
			END					
	ELSE
						SET @sSQL='	SELECT						
						M.ID,	
						CASE 
							WHEN DataProgrammata IS NOT NULL THEN	
									Convert(VARCHAR(10),M.DataProgrammata,105) + '' '' + LEFT(CONVERT(varchar(20),M.DataProgrammata,14),5) 									
							ELSE NULL 
						END AS DataProgrammata,	
						T.Descrizione As Tipo,								
						MS.AnteprimaRTF 
						'
			  IF @bVisualizzazioneSintetica=0
			SET @sSQL=@sSQL + ' , I.Icona, I.IDIcona, IVSM.Icona AS IconaVS,IVSM.IDIcona AS IDIconaVS'

	SET @sSQL=@sSQL + '
				FROM 
					#tmpFiltriTaskInfermieristici TMP WITH(NOLOCK)
						INNER JOIN T_MovTaskInfermieristici	M	WITH(NOLOCK)
								ON (TMP.IDTaskInfermieristico=M.ID)														
						LEFT JOIN T_MovPazienti P WITH(NOLOCK)
							ON (M.IDEpisodio = P.IDEpisodio)
						LEFT JOIN T_MovTrasferimenti TRA WITH(NOLOCK)
							ON (M.IDTrasferimento=TRA.ID)'
	IF @bWorkListTrasversale=1							
		SET @sSQL = @sSQL + '								
						LEFT JOIN T_MovEpisodi EPI WITH(NOLOCK)
							ON (M.IDEpisodio=EPI.ID)								
						LEFT JOIN T_UnitaAtomiche UA WITH(NOLOCK)
							ON (TRA.CodUA=UA.Codice)
						LEFT JOIN T_MovCartelle CAR WITH(NOLOCK)
							ON (TRA.IDCartella=CAR.ID)'
							 
	SET @sSQL=@sSQL + '							 																				
						LEFT JOIN T_TipoTaskInfermieristico T WITH(NOLOCK)
							ON (M.CodTipoTaskInfermieristico = T.Codice)
						LEFT JOIN T_StatoTaskInfermieristico S WITH(NOLOCK)
							ON (M.CodStatoTaskInfermieristico = S.Codice)
						LEFT JOIN T_Login L WITH(NOLOCK)
							ON (M.CodUtenteRilevazione = L.Codice)	
						LEFT JOIN T_Login LM WITH(NOLOCK)
							ON (M.CodUtenteUltimaModifica = LM.Codice)		
						LEFT JOIN 
								(SELECT CodVoce, 1 AS AzINSPermesso
								 FROM         T_AssRuoliAzioni WITH(NOLOCK)
								 WHERE      CodEntita = ''WKI'' AND 
											CodAzione= ''INS'' AND
											CodRuolo = ''' + @sCodRuolo + '''
								 ) AS AzINS
								 ON (AzINS.CodVoce = M.CodTipoTaskInfermieristico) 
						LEFT JOIN		 
								(SELECT CodVoce, 1 AS AzCOMPermesso
										 FROM         T_AssRuoliAzioni WITH(NOLOCK)
										 WHERE      CodEntita = ''WKI'' AND 
													CodAzione= ''COM'' AND
													CodRuolo = ''' + @sCodRuolo + '''
										 ) AS AzCOM
										 ON (AzCOM.CodVoce = M.CodTipoTaskInfermieristico) 	
						LEFT JOIN				 
								(SELECT CodVoce, 1 AS AzCANPermesso
										 FROM         T_AssRuoliAzioni WITH(NOLOCK)
										 WHERE      CodEntita = ''WKI'' AND 
													CodAzione= ''CAN'' AND
													CodRuolo = ''' + @sCodRuolo + '''
										 ) AS AzCAN
										 ON (AzCAN.CodVoce = M.CodTipoTaskInfermieristico) 			 		 
						LEFT JOIN 
								(SELECT ID As IDScheda, CodScheda,Versione,IDEntita,AnteprimaRTF, AnteprimaTXT
								 FROM
									T_MovSchede WITH(NOLOCK)
								 WHERE CodEntita = ''WKI'' AND							
									Storicizzata = 0
								) AS MS
							ON MS.IDEntita = M.ID
					  '

			IF @bVisualizzazioneSintetica=0
		BEGIN
			  SET @sSQL=@sSQL + ' 							
						LEFT JOIN 
							(SELECT 
							  IDNum AS IDIcona,	
							  CodTipo,
							  CodStato,
							   							   CONVERT(varbinary(max),null)  As Icona
							 FROM T_Icone WITH(NOLOCK)
							 WHERE CodEntita=''WKI''
							) AS I
								ON (M.CodTipoTaskInfermieristico = I.CodTipo AND 	
									M.CodStatoTaskInfermieristico = I.CodStato
									)								
					'	
				SET @sSQL=@sSQL + ' 							
						LEFT JOIN
							T_MovPrescrizioni MPS WITH(NOLOCK)
								ON (MPS.IDString=M.IDSistema)
						LEFT JOIN
							T_ViaSomministrazione VS WITH(NOLOCK)
								ON (VS.Codice=MPS.CodViaSomministrazione)											
						LEFT JOIN 
							(SELECT 
							  IDNum AS IDIcona,	
							  CodTipo,
							  CodStato,
							   							   CONVERT(varbinary(max),null)  As Icona
							 FROM T_Icone WITH(NOLOCK)
							 WHERE CodEntita=''VSM'' 
							) AS IVSM
								ON (VS.Codice = IVSM.CodTipo AND 	
									'''' = IVSM.CodStato
									)		
					'
		END									
	
	IF @uIDTaskInfermieristico IS NULL
	BEGIN
		
		IF ISNULL(@sOrdinamento,'')=''
		BEGIN
			
			
			SET @sOrderBy = ' ORDER BY 
									CASE 
										WHEN M.CodStatoTaskInfermieristico IN (''ER'',''AN'',''TR'')
										THEN 
											ISNULL(DataErogazione,M.DataProgrammata)
										ELSE 
											M.DataProgrammata 			 
									 END '

						IF @sCodStatoTaskInfermieristico NOT IN ('','''Tutti''')	
				SET @sOrderBy = @sOrderBy +  ' ASC '
			ELSE	
				SET @sOrderBy = @sOrderBy +  ' DESC '

			SET @sOrderBy = @sOrderBy + '
									 , 
									 M.CodTipoTaskInfermieristico DESC, 
									 MS.AnteprimaTXT '		
			
			SET @sSQL=@sSQL + @sOrderBy
		END
		ELSE
			BEGIN
				SET @sSQL=@sSQL + ' ORDER BY ' + @sOrdinamento  
				SET @sSQL=@sSQL + ' , MS.AnteprimaTXT '							
			END	
	END
	
		IF ISNULL(@bWorkListTrasversaleSoloFiltri,0)=1
		BEGIN
			SET @bIgnora=0
						SET @sSQL= ' SELECT TOP 1 M.* FROM T_MovTaskInfermieristici M WHERE 1=0
							   '						
		END	
		ELSE
			SET @bIgnora =0
		 
 				
 	IF @bIgnora	=0
 		BEGIN 			
			EXEC (@sSQL)
		END	
	
					IF @bDatiEstesi=1
		BEGIN
						SET @sSQL='SELECT DISTINCT
							TMP.CodTipoTaskInfermieristico AS CodTipo,
							T.Descrizione AS DescTipo		
						FROM 
							#TmpFiltriTaskInfermieristici TMP WITH(NOLOCK)																																		
								LEFT JOIN T_TipoTaskInfermieristico T WITH(NOLOCK)
									ON (TMP.CodTipoTaskInfermieristico=T.Codice)													
						'						
			SET @sSQL=@sSQL + ' ORDER BY T.Descrizione '						
			
						EXEC (@sSQL)
		END 	

	
					IF @bDatiEstesi=1
		BEGIN
						SET @sSQL='SELECT DISTINCT
							TMP.CodStatoTaskInfermieristico AS CodStato,
							T.Descrizione AS DescStato		
						FROM 
							#TmpFiltriTaskInfermieristici TMP WITH(NOLOCK)
								LEFT JOIN T_StatoTaskInfermieristico T WITH(NOLOCK)
									ON (TMP.CodStatoTaskInfermieristico=T.Codice)																				
						'			
						EXEC (@sSQL)
		END 				
	
					IF @bDatiEstesi=1 AND @bWorkListTrasversale=1
		BEGIN
			CREATE TABLE #tmpMovCodUA
			(
				CodUA VARCHAR(20) COLLATE Latin1_General_CI_AS
			)
			
			
			INSERT INTO #tmpMovCodUA(CodUA)
			SELECT CodUA
			FROM #tmpFiltriTaskInfermieristici
			GROUP BY CodUA
			
			SET @sSQL='SELECT DISTINCT
							TMP.CodUA,
							UA.Descrizione AS DescUA
						FROM 
							#tmpMovCodUA TMP WITH(NOLOCK)								
								LEFT JOIN T_UnitaAtomiche UA WITH(NOLOCK)
									ON (TMP.CodUA=UA.Codice)																														
						'						
			SET @sSQL=@sSQL + ' ORDER BY UA.Descrizione '
						EXEC (@sSQL)		
			
			DROP TABLE #tmpMovCodUA	
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
							V.Codice,
							V.Descrizione,
							I.Icona AS Icona
						FROM 
							#tmpMovIDPescrizioni TMP WITH(NOLOCK)
								INNER JOIN T_MovPrescrizioni MPS WITH(NOLOCK)
									ON (MPS.ID=TMP.IDPrescrizione)
								INNER JOIN								
									T_ViaSomministrazione V	WITH(NOLOCK)
										ON (MPS.CodViaSomministrazione=V.Codice)
								LEFT JOIN
									(SELECT CodTipo,Icona48 AS Icona
									 FROM T_Icone WITH(NOLOCK)
									 WHERE 
										CodEntita=''VSM'' AND 
										CodStato=''''
									) AS I
								ON V.Codice=I.CodTipo																															
						'			
			SET @sSQL=@sSQL + ' ORDER BY V.Descrizione '
						
					     EXEC (@sSQL)
		     
		     DROP TABLE #tmpMovIDPescrizioni
		END 	
	
					
		IF @bDatiEstesi=1 AND @bWorkListTrasversale=1
	BEGIN	
		SET @sSQL='SELECT DISTINCT
							TMP.CodUO,
							UO.Descrizione AS DescUO
						FROM 
							#tmpFiltriTaskInfermieristici TMP WITH(NOLOCK)							
								LEFT JOIN T_UnitaOperative UO WITH(NOLOCK)
									ON (TMP.CodUO=UO.Codice)																														
						'
						SET @sSQL=@sSQL + ' WHERE UO.Descrizione IS NOT NULL '		
			SET @sSQL=@sSQL + ' ORDER BY UO.Descrizione ASC'	
				
		EXEC (@sSQL)
	END
			
					
		IF @bDatiEstesi=1 AND @bWorkListTrasversale=1
	BEGIN	
		SET @sSQL='SELECT DISTINCT
							TMP.CodSettore,
							TMP.DescrSettore AS DescSettore
						FROM 
							#tmpFiltriTaskInfermieristici TMP WITH(NOLOCK)																																												
						'
						SET @sSQL=@sSQL + ' WHERE LTRIM(ISNULL(TMP.DescrSettore,'''')) <> '''' '		
			SET @sSQL=@sSQL + ' ORDER BY TMP.DescrSettore ASC'		
		
		
		EXEC (@sSQL)
	END		
				
	EXEC MSP_InsMovTimeStamp @xTimeStamp	

				
	DROP TABLE #tmpFiltriTaskInfermieristici 									
				
END











