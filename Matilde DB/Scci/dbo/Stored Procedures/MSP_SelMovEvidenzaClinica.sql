
CREATE PROCEDURE [dbo].[MSP_SelMovEvidenzaClinica](@xParametri AS XML)
AS
BEGIN
	   	
	   	
				
	DECLARE @uIDPaziente AS UNIQUEIDENTIFIER	
	DECLARE @uIDEpisodio AS UNIQUEIDENTIFIER
	DECLARE @sIDEpisodio AS VARCHAR(50)					DECLARE @uIDTrasferimento AS UNIQUEIDENTIFIER
	DECLARE @uIDEvidenzaClinica AS UNIQUEIDENTIFIER
	DECLARE @uIDRefertoDWH AS UNIQUEIDENTIFIER
	DECLARE @nNumRighe AS INTEGER
	DECLARE @bDatiEstesi AS Bit		
	DECLARE @sCodStatoEvidenzaClinica AS VARCHAR(1800)
	DECLARE @sCodStatoEvidenzaClinicaVisione AS VARCHAR(1800)
	DECLARE @sCodTipoEvidenzaClinica AS VARCHAR(1800)
	DECLARE @dDataInizio AS DATETIME
	DECLARE @dDataFine AS DATETIME
	DECLARE @bVisualizzazioneSintetica AS BIT
	DECLARE @bPDFReferto AS BIT
	DECLARE @sCodRuolo AS VARCHAR(20)
	DECLARE @bEvidenzaClinicaTrasversale AS BIT	
	DECLARE @sUnitaOperativa AS Varchar(MAX)						
	DECLARE @sCodUO AS Varchar(MAX)
	DECLARE @bSoloPazientiSeguiti AS BIT
	DECLARE @sCodLogin AS VARCHAR(100)		
	DECLARE @sCodFiltroSpeciale	AS VARCHAR(20)	
	DECLARE @bSoloAllegaInCartella AS BIT

		DECLARE @sFiltroGenerico AS Varchar(500) 
	DECLARE @sDataNascita AS VARCHAR(10)	
	DECLARE @sCodUA AS Varchar(MAX)
	
	DECLARE @sDataTmp AS VARCHAR(20)
	
		DECLARE @sCodRuoloSuperUser AS VARCHAR(20)
	DECLARE @bSuperUser AS BIT
	DECLARE @sGUID AS VARCHAR(Max)
	DECLARE @nTemp AS INTEGER
	DECLARE @sOrderby AS VARCHAR(MAX)
	DECLARE @uIDCartella AS UNIQUEIDENTIFIER
	DECLARE @xPar AS XML
	
		DECLARE @bDaVistare AS BIT
	DECLARE @bGrafico AS BIT
	
		SET @sCodRuolo=(SELECT TOP 1 ValoreParametro.CodRuolo.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/TimeStamp/CodRuolo') as ValoreParametro(CodRuolo))
	SET @sCodRuolo=ISNULL(@sCodRuolo,'')
	
		SET @nTemp=(SELECT COUNT(*) FROM T_AssRuoliModuli
				WHERE CodModulo='EvidenzaC_Vista'	
				AND CodRuolo=@sCodRuolo)
	IF @nTemp>=1 
		SET @bDaVistare=1
	ELSE
		SET @bDaVistare=0
		
		SET @bGrafico=1
	
		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDPaziente.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDPaziente') as ValoreParametro(IDPaziente))
	IF 	ISNULL(@sGUID,'') <> '' SET @uIDPaziente=CONVERT(UNIQUEIDENTIFIER,	@sGUID)	
		
		SET @sIDEpisodio=(SELECT TOP 1 ValoreParametro.IDEpisodio.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDEpisodio') as ValoreParametro(IDEpisodio))
					  
	IF 	ISNULL(@sIDEpisodio,'') <> '' AND ISNULL(@sIDEpisodio,'') <> 'NULL' 
	BEGIN			
		SET @uIDEpisodio=CONVERT(UNIQUEIDENTIFIER,	@sIDEpisodio)	
	END	
		
		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDTrasferimento.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDTrasferimento') as ValoreParametro(IDTrasferimento))
	IF 	ISNULL(@sGUID,'') <> '' SET @uIDTrasferimento=CONVERT(UNIQUEIDENTIFIER,	@sGUID)	
					  				  
		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDEvidenzaClinica.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDEvidenzaClinica') as ValoreParametro(IDEvidenzaClinica))	
	IF 	ISNULL(@sGUID,'') <> '' SET @uIDEvidenzaClinica=CONVERT(UNIQUEIDENTIFIER,	@sGUID)				
	
		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDRefertoDWH.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDRefertoDWH') as ValoreParametro(IDRefertoDWH))
					  
	IF 	ISNULL(@sGUID,'') <> '' AND ISNULL(@sGUID,'') <> 'NULL' 
	BEGIN			
		SET @uIDRefertoDWH=CONVERT(UNIQUEIDENTIFIER,@sGUID)	
	END	
	
						  				  
		SET @nNumRighe=(SELECT	TOP 1 ValoreParametro.NumRighe.value('.','INTEGER')
								FROM @xParametri.nodes('/Parametri/NumRighe') as ValoreParametro(NumRighe))	
	SET @nNumRighe=ISNULL(@nNumRighe,0)
		
		SET @bDatiEstesi=(SELECT TOP 1 ValoreParametro.DatiEstesi.value('.','bit')
					  FROM @xParametri.nodes('/Parametri/DatiEstesi') as ValoreParametro(DatiEstesi))
	SET @bDatiEstesi=ISNULL(@bDatiEstesi,0)

		SET @sCodStatoEvidenzaClinica=''
	SELECT	@sCodStatoEvidenzaClinica =  @sCodStatoEvidenzaClinica +
														CASE 
								WHEN @sCodStatoEvidenzaClinica='' THEN ''
								ELSE  ','
							END + ''''
						  + ValoreParametro.CodStatoEvidenzaClinica.value('.','VARCHAR(1800)')
						  + ''''
						 FROM @xParametri.nodes('/Parametri/CodStatoEvidenzaClinica') as ValoreParametro(CodStatoEvidenzaClinica)
						 
	SET @sCodStatoEvidenzaClinica=LTRIM(RTRIM(@sCodStatoEvidenzaClinica))
	IF	@sCodStatoEvidenzaClinica='''''' SET @sCodStatoEvidenzaClinica=''
	SET @sCodStatoEvidenzaClinica=UPPER(@sCodStatoEvidenzaClinica)

		SET @sCodStatoEvidenzaClinicaVisione=''
	SELECT	@sCodStatoEvidenzaClinicaVisione =  @sCodStatoEvidenzaClinicaVisione +
														CASE 
								WHEN @sCodStatoEvidenzaClinicaVisione='' THEN ''
								ELSE  ','
							END + ''''
						  + ValoreParametro.CodStatoEvidenzaClinicaVisione.value('.','VARCHAR(1800)')
						  + ''''
						 FROM @xParametri.nodes('/Parametri/CodStatoEvidenzaClinicaVisione') as ValoreParametro(CodStatoEvidenzaClinicaVisione)
						 
	SET @sCodStatoEvidenzaClinicaVisione=LTRIM(RTRIM(@sCodStatoEvidenzaClinicaVisione))
	IF	@sCodStatoEvidenzaClinicaVisione='''''' SET @sCodStatoEvidenzaClinicaVisione=''
	SET @sCodStatoEvidenzaClinicaVisione=UPPER(@sCodStatoEvidenzaClinicaVisione)

		SET @sCodTipoEvidenzaClinica=''
	SELECT	@sCodTipoEvidenzaClinica =  @sCodTipoEvidenzaClinica +
														CASE 
								WHEN @sCodTipoEvidenzaClinica='' THEN ''
								ELSE  ','
							END + ''''
						  + ValoreParametro.CodTipoEvidenzaClinica.value('.','VARCHAR(1800)')
						  + ''''
						 FROM @xParametri.nodes('/Parametri/CodTipoEvidenzaClinica') as ValoreParametro(CodTipoEvidenzaClinica)
						 
	SET @sCodTipoEvidenzaClinica=LTRIM(RTRIM(@sCodTipoEvidenzaClinica))
	IF	@sCodTipoEvidenzaClinica='''''' SET @sCodTipoEvidenzaClinica=''
	SET @sCodTipoEvidenzaClinica	=UPPER(@sCodTipoEvidenzaClinica)
	
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
		
		SET @bVisualizzazioneSintetica=(SELECT TOP 1 ValoreParametro.VisualizzazioneSintetica.value('.','bit')
					  FROM @xParametri.nodes('/Parametri/VisualizzazioneSintetica') as ValoreParametro(VisualizzazioneSintetica))											
	SET @bVisualizzazioneSintetica=ISNULL(@bVisualizzazioneSintetica,0)

		SET @bPDFReferto=(SELECT TOP 1 ValoreParametro.PDFReferto.value('.','bit')
					  FROM @xParametri.nodes('/Parametri/PDFReferto') as ValoreParametro(PDFReferto))											
	SET @bPDFReferto=ISNULL(@bPDFReferto,0)	
	
		SET @bEvidenzaClinicaTrasversale=(SELECT TOP 1 ValoreParametro.EvidenzaClinicaTrasversale.value('.','bit')
					  FROM @xParametri.nodes('/Parametri/EvidenzaClinicaTrasversale') as ValoreParametro(EvidenzaClinicaTrasversale))											
	SET @bEvidenzaClinicaTrasversale=ISNULL(@bEvidenzaClinicaTrasversale,0)
	
		SET @sFiltroGenerico=(SELECT	TOP 1 ValoreParametro.FiltroGenerico.value('.','VARCHAR(500)')
						 FROM @xParametri.nodes('/Parametri/FiltroGenerico') as ValoreParametro(FiltroGenerico))						 
	SET @sFiltroGenerico= ISNULL(@sFiltroGenerico,'')
	SET @sFiltroGenerico=LTRIM(RTRIM(@sFiltroGenerico))
	
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
	
		SET @sUnitaOperativa=''
	SELECT	@sUnitaOperativa =  @sUnitaOperativa +
														CASE 
								WHEN @sUnitaOperativa='' THEN ''
								ELSE  ','
							END + ''''
						  + ValoreParametro.UnitaOperativa.value('.','VARCHAR(1800)')
						  + ''''
						 FROM @xParametri.nodes('/Parametri/UnitaOperativa') as ValoreParametro(UnitaOperativa)	
	SET @sUnitaOperativa=LTRIM(RTRIM(@sUnitaOperativa))
	IF	@sUnitaOperativa='''''' SET @sUnitaOperativa=''

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

	
		SET @bSoloPazientiSeguiti=(SELECT TOP 1 ValoreParametro.SoloPazientiSeguiti.value('.','bit')
					  FROM @xParametri.nodes('/Parametri/SoloPazientiSeguiti') as ValoreParametro(SoloPazientiSeguiti))											
	SET @bSoloPazientiSeguiti=ISNULL(@bSoloPazientiSeguiti,0)
			
		SET @sCodLogin=(SELECT	TOP 1 ValoreParametro.CodLogin.value('.','VARCHAR(100)')
						 FROM @xParametri.nodes('/Parametri/TimeStamp/CodLogin') as ValoreParametro(CodLogin))						 
	SET @sCodLogin= LTRIM(RTRIM(ISNULL(@sCodLogin,'')))
	
		SET @sCodFiltroSpeciale=(SELECT TOP 1 ValoreParametro.CodFiltroSpeciale.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodFiltroSpeciale') as ValoreParametro(CodFiltroSpeciale))

		SET @bSoloAllegaInCartella=(SELECT TOP 1 ValoreParametro.SoloAllegaInCartella.value('.','bit')
					  FROM @xParametri.nodes('/Parametri/SoloAllegaInCartella') as ValoreParametro(SoloAllegaInCartella))											
	SET @bSoloAllegaInCartella=ISNULL(@bSoloAllegaInCartella,0)

								  
	    DECLARE @xTimeStamp AS XML	

    SET @xTimeStamp=(SELECT TOP 1 ValoreParametro.TS.query('.')
					  FROM @xParametri.nodes('/Parametri/TimeStamp') as ValoreParametro(TS))
	
	SET @xTimeStamp=CONVERT(XML,
						'<Parametri>' + CONVERT(varchar(max),@xTimeStamp) +
						'</Parametri>')
																				
				
		
		SET @sCodRuoloSuperUser=(SELECT TOP 1 Valore FROM T_Config WHERE ID=681)
	SET @sCodRuoloSuperUser=ISNULL(@sCodRuoloSuperUser,'')
	
	IF @sCodRuoloSuperUser=@sCodRuolo 
		SET @bSuperUser=1
	ELSE
		SET @bSuperUser=0
		
						
			
				CREATE TABLE #tmpUARuolo
		(
			CodUA VARCHAR(20) COLLATE Latin1_General_CI_AS,
			Descrizione VARCHAR(255) COLLATE Latin1_General_CI_AS
		)

		DECLARE @xTmp AS XML
		SET  @xTmp=CONVERT(XML,'<Parametri><CodRuolo>'+ @sCodRuolo + '</CodRuolo></Parametri>')
		
		INSERT #tmpUARuolo EXEC MSP_SelUADaRuolo @xTmp
		
		CREATE INDEX IX_CodUA ON #tmpUARuolo (CodUA)
		
								   
		DECLARE @sSQL AS VARCHAR(MAX)
	DECLARE @sWhere AS VARCHAR(Max)
	DECLARE @sTmp AS VARCHAR(Max)
	DECLARE @gIDSessione AS UNIQUEIDENTIFIER
	
	DECLARE @sVistraTRACALC AS VARCHAR(MAX)
	DECLARE @sVistraTRA	AS VARCHAR(MAX)
	
		SET @gIDSessione=NEWID()
	 

	CREATE TABLE [dbo].#tmpFiltriEvidenzaClinica(
		[IDEvidenzaClinica] [uniqueidentifier] NOT NULL,
		CodTipoEVC VARCHAR(20),		
	) 

	
	SET @sSQL='		INSERT INTO #tmpFiltriEvidenzaClinica(IDEvidenzaClinica, CodTipoEVC)					
					SELECT '

		IF @nNumRighe<> 0  SET @sSQL= @sSQL + ' TOP ' + convert(varchar(10),@nNumRighe)+  ' ' 					
	
	SET @sSQL=@sSQL +				
					' M.ID AS IDEvidenzaClinica,
					  MIN(M.CodTipoEvidenzaClinica)  AS CodTipoEvidenzaClinica
					
					FROM 
						T_MovEvidenzaClinica M	WITH (NOLOCK)
							LEFT JOIN T_MovPazienti P WITH (NOLOCK)
								ON (M.IDEpisodio=P.IDEpisodio) 
						'

		IF @uIDEpisodio IS NOT NULL 
		SET @sSQL=@sSQL + ' INNER JOIN '
	ELSE
		SET @sSQL=@sSQL + '	LEFT JOIN '
	
	SET @sSQL=@sSQL + ' T_MovEpisodi EPI WITH(NOLOCK)
							   ON (M.IDEpisodio=EPI.ID)								
					'


	IF @bEvidenzaClinicaTrasversale=1
	BEGIN
			
			
						SET @sVistraTRACALC=' (SELECT 																			
										STRA.IDEpisodio AS IDEpisodio,
										CCOL.IDEpisodio AS IDEpisodioCollegato,
										CASE 
											WHEN CR.CodStatoCartella =''AP'' THEN STRA.ID
											ELSE CCOL.IDTrasferimento
										END AS IDTrasferimento,
										CASE 
											WHEN CR.CodStatoCartella =''AP'' THEN CR.ID
											ELSE CCOL.IDCartella
										END AS IDCartella,
										CASE 
											WHEN CR.CodStatoCartella =''AP'' THEN CR.NumeroCartella
											ELSE CCOL.NumeroCartella
										END AS NumeroCartella,
										CASE 
											WHEN CR.CodStatoCartella =''AP'' THEN STRA.CodUO
											ELSE CCOL.CodUO
										END AS CodUO,
										CASE 
											WHEN CR.CodStatoCartella =''AP'' THEN STRA.DescrUO
											ELSE CCOL.DescrUO
										END AS DescrUO,		
										CASE 
											WHEN CR.CodStatoCartella =''AP'' THEN STRA.CodUA
											ELSE CCOL.CodUA
										END AS CodUA,	
										CASE 
											WHEN CR.CodStatoCartella =''AP'' THEN A.Descrizione
											ELSE CCOL.DescUA
										END AS DescUA
									FROM 
										T_MovCartelle CR WITH (NOLOCK)
											INNER JOIN T_MovTrasferimenti STRA WITH(NOLOCK)
												ON (CR.ID=STRA.IDCartella)										
											INNER JOIN T_UnitaAtomiche A
												ON (STRA.CodUA=A.Codice)

											LEFT JOIN 												
													(SELECT
															IDEntita AS IDCartellaOrigine
															,IDEntitaCollegata AS IDCartellaDestinazione
															,STRACCOL.ID AS IDTrasferimento
															,STRACCOL.ID AS IDCartella
															,STRACCOL.NumeroCartella
															,STRACCOL.CodUO
															,STRACCOL.DescrUO
															,STRACCOL.CodUA
															,STRACCOL.DescUA AS DescUA
															,STRACCOL.IDEpisodio			
													   FROM T_MovRelazioniEntita REL
																INNER JOIN 
																	(SELECT  STRACCOLTMP.ID 
																			,STRACCOLTMP.IDCartella
																			,CRCCOL.NumeroCartella
																			,STRACCOLTMP.CodUO
																			,STRACCOLTMP.DescrUO
																			,STRACCOLTMP.CodUA
																			,ACCOL.Descrizione AS DescUA
																			,STRACCOLTMP.IDEpisodio
																	 FROM 
																		T_MovTrasferimenti STRACCOLTMP WITH(NOLOCK)													
																		INNER JOIN T_UnitaAtomiche ACCOL
																			ON (STRACCOLTMP.CodUA=ACCOL.Codice)	
																		INNER JOIN T_MovCartelle CRCCOL
																			ON (CRCCOL.ID=STRACCOLTMP.IDCartella)
																	WHERE
																		STRACCOLTMP.CodUA IN (SELECT CodUA FROM #tmpUARuolo) AND
																		STRACCOLTMP.CodStatoTrasferimento IN (''AT'',''DM'')																																 
																	 ) AS STRACCOL
																 ON 
																	 REL.IDEntitaCollegata=STRACCOL.IDCartella
																
														WHERE REL.CodEntita=''CAR'' AND
															  REL.CodEntitaCollegata =''CAR'' 															  		   														
													  ) CCOL
													ON (CR.ID = CCOL.IDCartellaOrigine) 											  		
									WHERE 									 
										STRA.CodUA IN (SELECT CodUA FROM #tmpUARuolo) AND
										(
											(CR.CodStatoCartella=''AP'' AND
											STRA.CodStatoTrasferimento IN (''AT'',''DM'')
											)
										 OR
											(CR.CodStatoCartella=''CH'' AND
											CCOL.IDCartellaOrigine IS NOT NULL AND
											STRA.CodStatoTrasferimento IN (''AT'',''DM'')
											)
										)
									
								) AS TRACALC '
			SET @sVistraTRA=' (SELECT 
										STRA2.IDEpisodio,
										STRA2.ID AS IDTrasferimento, 
										STRA2.IDCartella,	
										CR2.NumeroCartella,
										STRA2.CodUO,									
										STRA2.DescrUO,
										STRA2.CodUA,
										A2.Descrizione AS DescUA		
								 FROM 
										T_MovTrasferimenti  STRA2										
											INNER JOIN T_MovCartelle CR2 WITH (NOLOCK)		
												ON (CR2.ID=STRA2.IDCartella)
											INNER JOIN T_UnitaAtomiche A2
												ON (STRA2.CodUA=A2.Codice)	
								 WHERE			
										CR2.CodStatoCartella=''AP'' AND
										STRA2.CodStatoTrasferimento IN (''AT'',''DM'')  AND
										STRA2.CodUA IN (SELECT CodUA FROM #tmpUARuolo)
								 ) AS TRA '								
			SET @sSQL=@sSQL +						
					'	LEFT JOIN ' + @sVistraTRACALC +
					'		ON
								TRACALC.IDEpisodio=M.IDEpisodio	
							LEFT JOIN ' + @sVistraTRA + '
								ON (TRA.IDTrasferimento=M.IDTrasferimento)
					' 
	END			
				
	CREATE TABLE #tmpEpisodiCollegati
	(
		IDEpisodio UNIQUEIDENTIFIER
	)
	
	CREATE TABLE #tmpCartelleTrasf		
	(
		IDCartella UNIQUEIDENTIFIER
	)
	
	SET @sWhere=''				
		
		IF @uIDPaziente IS NOT NULL
		BEGIN
			SET @sTmp= ' AND P.IDPaziente=''' + convert(varchar(50),@uIDPaziente) +''''
			SET @sWhere= @sWhere + @sTmp								
		END			

		IF @sIDEpisodio='NULL'
		BEGIN			
			SET @sTmp=''
			SET @sWhere= @sWhere + @sTmp								
		END	
	ELSE
				IF @uIDEpisodio IS NOT NULL 
			BEGIN
										SET @xPar=CONVERT(XML,'<Parametri>
											<IDEpisodio>' + CONVERT(VARCHAR(50),@uIDEpisodio) + '</IDEpisodio>
											</Parametri>')
									
					INSERT INTO #tmpEpisodiCollegati												 				
						EXEC MSP_SelEpisodiCollegati @xPar	
					
										SET @sTmp= ' AND (M.IDEpisodio=''' + convert(varchar(50), @uIDEpisodio) +'''
									  OR M.IDEpisodio IN (SELECT IDEpisodio FROM #tmpEpisodiCollegati) 
									  )
									  '
							  
								SET @sWhere= @sWhere + @sTmp								
			END	
	
		IF @uIDTrasferimento IS NOT NULL
		BEGIN
						SET @uIDCartella=(SELECT TOP 1 IDCartella FROM T_MovTrasferimenti WHERE ID=@uIDTrasferimento)
			
			SET @xPar=CONVERT(XML,'<Parametri>
									<IDCartella>' + CONVERT(VARCHAR(50),@uIDCartella) + '</IDCartella>
									</Parametri>')
											
			INSERT INTO #tmpCartelleTrasf 					
			EXEC MSP_SelCartelleCollegate @xPar	
	
			SET @sTmp= ' AND (M.IDTrasferimento=''' + convert(varchar(50), @uIDTrasferimento) +'''
							  OR M.IDTrasferimento IN (SELECT ID FROM T_MovTrasferimenti WHERE IDCartella IN (SELECT IDCartella FROM #tmpCartelleTrasf)) 
							  )
							  '						
			SET @sWhere= @sWhere + @sTmp								
		END		
		
		IF @uIDEvidenzaClinica IS NOT NULL
		BEGIN
			SET @sTmp= ' AND M.ID=''' + convert(varchar(50),@uIDEvidenzaClinica) +''''
			SET @sWhere= @sWhere + @sTmp								
		END		
					
		
		IF @uIDRefertoDWH IS NOT NULL
		BEGIN
			SET @sTmp= ' AND M.IDRefertoDWH=''' + convert(varchar(50),@uIDRefertoDWH) +''''
			SET @sWhere= @sWhere + @sTmp								
		END		
		
		IF @sCodStatoEvidenzaClinica NOT IN ('','''Tutti''')
		BEGIN	
						SET @sTmp=  ' AND 			
							 M.CodStatoEvidenzaClinica IN ('+ @sCodStatoEvidenzaClinica + ')
						'  				
			SET @sWhere= @sWhere + @sTmp														
		END
	ELSE
		BEGIN
			IF @sCodStatoEvidenzaClinica NOT IN ('''Tutti''')
				BEGIN
										SET @sTmp=  ' AND 			
									 M.CodStatoEvidenzaClinica NOT IN (''AN'',''CA'')
								'  				
					SET @sWhere= @sWhere + @sTmp
				END	
			ELSE
				BEGIN
															SET @sTmp=  ' AND 			
									 M.CodStatoEvidenzaClinica <> ''CA''
								'  				
					SET @sWhere= @sWhere + @sTmp
				END	
		END	


		IF @sCodStatoEvidenzaClinicaVisione	 <> ''
		BEGIN	
						SET @sTmp=  ' AND 			
							 M.CodStatoEvidenzaClinicaVisione IN ('+ @sCodStatoEvidenzaClinicaVisione + ')
						'  				
			SET @sWhere= @sWhere + @sTmp														
		END
	
		IF @sCodTipoEvidenzaClinica <> ''
		BEGIN	
						SET @sTmp=  ' AND 			
							 M.CodTipoEvidenzaClinica IN ('+ @sCodTipoEvidenzaClinica + ')
						'  				
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
				
		IF ISNULL(@sCodFiltroSpeciale,'') <> ''
	BEGIN
		SET @sTmp=(SELECT SQL FROM T_FiltriSpeciali WHERE Codice=@sCodFiltroSpeciale)
		IF ISNULL(@sTmp,'') <> '' 
		BEGIN						
			SET @sWhere= @sWhere + ' AND ' + 	@sTmp + ''
		END
	END
	
		IF @bSoloAllegaInCartella=1
	BEGIN
				SET @sWhere= @sWhere + ' AND M.CodTipoEvidenzaClinica IN (SELECT Codice FROM T_TipoEvidenzaClinica WHERE AllegaInCartella = 1) '
	END

			
	IF @bEvidenzaClinicaTrasversale=1 
		BEGIN					
								SET @sWhere= @sWhere + '    AND ISNULL(TRA.CodUA,TRACALC.CodUA)  IN (SELECT CodUA FROM #tmpUARuolo) '
																
								IF ISNULL(@sUnitaOperativa,'') <> ''
				BEGIN
					SET @sWhere= @sWhere + ' AND ISNULL(TRA.DescrUO,TRACALC.DescrUO)  IN ('+ @sUnitaOperativa + ')'
				END
										
								IF ISNULL(@sCodUO,'') <> ''				
					SET @sWhere= @sWhere + '    AND ISNULL(TRA.CodUO,TRACALC.CodUO)  IN (' +@sCodUO + ')'	

								IF ISNULL(@sCodUA,'') <> ''				
					SET @sWhere= @sWhere + '    AND ISNULL(TRA.CodUA,TRACALC.CodUA)  IN (' +@sCodUA + ')'	
					
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
					
					SET @sWhere= @sWhere + '  OR ISNULL(
														ISNULL(TRA.NumeroCartella,TRACALC.NumeroCartella)
														
														,'''') like ''' + dbo.MF_NumeroCartellaDaBarcode(@sFiltroGenerico) + '%'''
										SET @sWhere= @sWhere + '     )' 					
				END		
		END						
		
		IF @bSoloPazientiSeguiti=1
	BEGIN
				SET @sWhere= @sWhere + ' AND P.IDPaziente IN
								  (SELECT M.IDPaziente
								   FROM 
								     T_MovPazientiSeguiti M									
								   WHERE M.CodUtente=''' + @sCodLogin + '''							   
										  AND M.CodStatoPazienteSeguito IN (''IC'')										 
								  UNION 		  
								  SELECT PA.IDPaziente AS IDPaziente
								   FROM 
								     T_MovPazientiSeguiti M
										INNER JOIN T_PazientiAlias PA	
											ON M.IDPaziente=PA.IDPazienteVecchio 
								   WHERE M.CodUtente=''' + @sCodLogin + '''							   
										  AND M.CodStatoPazienteSeguito IN (''IC'')										  
								   )' 		  								    
	END
		

			
	IF 	@bEvidenzaClinicaTrasversale=1 AND @bDatiEstesi=1	
		SET @sWhere=@sWhere + ' AND 1=0 '

		IF ISNULL(@sWhere,'')<> ''
	BEGIN
	
		SET @sSQL=@sSQL +' WHERE ' + RIGHT(@sWhere,len(@sWhere)-5)
	END		
	


				 
		
			
	SET @sSQL=@sSQL +' GROUP BY M.ID'
			
	PRINT @sSQL 	
	EXEC (@sSQL)
		
					
					
		
	IF ISNULL(@bVisualizzazioneSintetica,0)=0
	BEGIN
				SET @sSQL='	SELECT
						M.ID,
						P.IDPaziente, '

		IF @bEvidenzaClinicaTrasversale=0 
						SET @sSQL=@sSQL + ' M.IDEpisodio, '
		ELSE
						SET @sSQL=@sSQL + ' ISNULL(TRACALC.IDEpisodioCollegato,M.IDEpisodio) AS IDEpisodio,'				
				
						
		IF @bEvidenzaClinicaTrasversale=0 
						SET @sSQL=@sSQL + ' M.IDTrasferimento, '
		ELSE
						SET @sSQL=@sSQL + ' ISNULL(M.IDTrasferimento,TRACALC.IDTrasferimento) AS IDTrasferimento,  '				
								
			
			
		SET @sSQL=@sSQL +'M.DataEvento,
						M.DataEventoUTC,
						M.DataEventoDWH,
						M.DataEventoDWHUTC,
						M.IDRefertoDWH,
						M.Anteprima,
						M.CodTipoEvidenzaClinica AS CodTipo,
						T.Descrizione AS DescrTipo, 
						M.CodStatoEvidenzaClinica AS CodStato,
						S.Descrizione As DescrStato,
						M.CodStatoEvidenzaClinicaVisione AS CodStatoVisione,
						S2.Descrizione As DescrStatoVisione,
						M.CodUtenteVisione AS CodUtenteVisione,
						L.Descrizione AS DescrUtenteVisione,
						M.DataVisione,
						M.DataVisioneUTC,
						CASE
							WHEN M.PDFDWH IS NOT NULL THEN 1
							ELSE 0
						END AS EsistePDFDWH,							
							 
						CASE 
							WHEN ISNULL(CodStatoEvidenzaClinicaVisione,'''') =''DV'' AND 
									ISNULL(CodStatoEvidenzaClinica,'''') =''CM'' AND
									' + 
								CONVERT(CHAR(1),@bDaVistare) + '=1 THEN 1 
							ELSE 0
						END	AS PermessoVista,
						CASE 
							WHEN ISNULL(CodTipoEvidenzaClinica,'''') =''LAB'' AND ' + CONVERT(CHAR(1),@bGrafico) + '=1 THEN 1 
							ELSE 0
						END	AS PermessoGrafico, 					
						CASE 
							WHEN (' + CONVERT(VARCHAR(1),@bSuperUser) + '=1) THEN 1 
							ELSE 0
						END	AS PermessoCancella '
						
		 IF @bEvidenzaClinicaTrasversale=1		
						SET @sSQL = @sSQL + '
						,ISNULL(P.Cognome,'''') + '' '' + ISNULL(P.Nome,'''') + '' ('' + ISNULL(P.Sesso,'''') + '') '' +
						CASE
							WHEN P.DataNascita IS NOT NULL THEN '' ('' + CONVERT(VARCHAR(10),P.DataNascita,105) + '') ''
							ELSE ''''
						END +
							'' - '' + ISNULL(UA.Descrizione,ISNULL(TRA.DescUA,ISNULL(TRACALC.DescUA,''''))) + 
							'' - '' + ISNULL(EPI.NumeroNosologico,'''') 
						AS DescrPaziente, 
						ISNULL(TRA.CodUA,TRACALC.CodUA) AS CodUA
						'
	END				
	ELSE
				SET @sSQL='	SELECT						
						M.IDEpisodio,					
						CASE 
							WHEN DataEvento IS NOT NULL THEN	
									Convert(VARCHAR(10),M.DataEvento,105) + '' '' + LEFT(CONVERT(varchar(20),M.DataEvento,14),5) 									
							ELSE NULL 
						END AS DataEvento,						
						T.Descrizione AS DescrTipo						
					'					
	IF @bPDFReferto=1  
			SET @sSQL=@sSQL + ',M.PDFDWH AS PDFDWH'			
	ELSE
		BEGIN
			IF ISNULL(@bVisualizzazioneSintetica,0)=0
				SET @sSQL=@sSQL + ',NULL AS PDFDWH'
		END		
	

	IF ISNULL(@bVisualizzazioneSintetica,0)=0
		SET @sSQL=@sSQL + ' , I.Icona AS Icona,IDIcona '

	SET @sSQL=@sSQL + '
				FROM 
					#tmpFiltriEvidenzaClinica TMP
						INNER JOIN T_MovEvidenzaClinica	M	
								ON (TMP.IDEvidenzaClinica=M.ID)														
						LEFT JOIN T_MovPazienti P
							ON (M.IDEpisodio=P.IDEpisodio)													
						LEFT JOIN T_TipoEvidenzaClinica T 			
							ON (M.CodTipoEvidenzaClinica=T.Codice)'
						
	IF ISNULL(@bVisualizzazioneSintetica,0)=0
						SET @sSQL=@sSQL + '
						LEFT JOIN T_StatoEvidenzaClinica S
							ON (M.CodStatoEvidenzaClinica=S.Codice)
						LEFT JOIN T_StatoEvidenzaClinicaVisione S2
							ON (M.CodStatoEvidenzaClinicaVisione=S2.Codice)							
						LEFT JOIN T_Login L
							ON (M.CodUtenteVisione=L.Codice)									
					  '

 
   IF @bEvidenzaClinicaTrasversale=1								
				SET @sSQL = @sSQL + '																				
						LEFT JOIN ' + @sVistraTRACALC + '  
									ON (M.IDEpisodio=TRACALC.IDEpisodio)

						LEFT JOIN T_MovEpisodi EPI WITH(NOLOCK) 
							ON (ISNULL(TRACALC.IDEpisodioCollegato,M.IDEpisodio)=EPI.ID)  
						LEFT JOIN T_UnitaAtomiche UACalc  WITH (NOLOCK)
									ON (TRACALC.CodUA = UACalc.Codice)
								
						LEFT JOIN ' + @sVistraTRA + '
									ON (TRA.IDTrasferimento=M.IDTrasferimento) 
						LEFT JOIN T_UnitaAtomiche AS UA  WITH (NOLOCK)
									ON (TRA.CodUA = UA.Codice)	'			
				 
		SET @sSQL=@sSQL + ' 							
					LEFT JOIN 
						(SELECT 
						  IDNum AS IDIcona,	 	
						  CodTipo,
						  CodStato,
						  						  						  						  CONVERT(varbinary(max),null)  As Icona
						 FROM T_Icone
						 WHERE CodEntita=''EVC''
						) AS I
							ON (M.CodTipoEvidenzaClinica=I.CodTipo AND 	
								M.CodStatoEvidenzaClinica=I.CodStato
								)								
				'		
					
	
		IF 	@bEvidenzaClinicaTrasversale=1 AND @bDatiEstesi=1	
		SET @sSQL=@sSQL + ' WHERE 1=0 '
		
				SET @sOrderBy = ' ORDER BY M.DataEvento DESC '
	
	
		SET @sSQL=@sSQL + @sOrderBy
 		
	
	PRINT @sSQL					
	EXEC (@sSQL)
			
					
		IF @bDatiEstesi=1 AND @bEvidenzaClinicaTrasversale=1
		BEGIN
			SELECT 
				T.Codice AS CodTipo,
				T.Descrizione AS DescTipo,							
				NULL AS Icona
			FROM
			T_TipoEvidenzaClinica T
			WHERE Codice <> 'SCCI'
		END
	ELSE
	BEGIN
		IF @bDatiEstesi=1
			BEGIN		
				SET @sSQL='
						SELECT 
							Q.CodTipo,
							T.Descrizione AS DescTipo,							
							T.Icona
						FROM
							(SELECT DISTINCT
									CodTipoEVC AS CodTipo
							 FROM 
						 		#tmpFiltriEvidenzaClinica TMP													
							) AS Q 
							LEFT JOIN T_TipoEvidenzaClinica T
									ON (Q.CodTipo  COLLATE Latin1_General_CI_AS =T.Codice)	'				
				PRINT @sSQL
				EXEC (@sSQL)	
			
			END 			
	END
						
									
	IF @bDatiEstesi=1 AND @bEvidenzaClinicaTrasversale=1
		BEGIN
						SET @sSQL=' SELECT 
								Codice AS CodUA,
								Descrizione AS DescUA
						FROM
								T_UnitaAtomiche A
						WHERE A.Codice IN (SELECT CodUA FROM #tmpUARuolo)
						ORDER BY 2
						'			
						EXEC (@sSQL)
		END 	
	
					
		IF @bDatiEstesi=1 AND @bEvidenzaClinicaTrasversale=1
	BEGIN	
				SET @sSQL ='SELECT 
						A.CodUO,
						ISNULL(O.Descrizione,A.CodUO) AS DescUO
					FROM 
						T_AssUAUOLetti	A
							LEFT JOIN T_UnitaOperative O
								ON (A.CodAzi=O.CodAzi AND
									A.CodUO=O.Codice)
					WHERE	A.CodUO <> ''*'' AND
							A.CodUA IN (SELECT CodUA FROM #tmpUARuolo)
					ORDER BY 2
					'

	
				
				EXEC (@sSQL)
	END
	
	
				
	EXEC MSP_InsMovTimeStamp @xTimeStamp	
		
					
											
		DROP TABLE #tmpFiltriEvidenzaClinica
	DROP TABLE #tmpEpisodiCollegati
	DROP TABLE #tmpCartelleTrasf							
				
END