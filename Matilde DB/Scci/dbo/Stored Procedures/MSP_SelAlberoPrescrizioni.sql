CREATE PROCEDURE [dbo].[MSP_SelAlberoPrescrizioni](@xParametri AS XML)
AS
BEGIN

	
				
	DECLARE @uIDPaziente AS UNIQUEIDENTIFIER
	DECLARE @uIDEpisodio AS UNIQUEIDENTIFIER
	DECLARE @uIDTrasferimento AS UNIQUEIDENTIFIER
	DECLARE @nStatoAppuntamento INTEGER
	DECLARE @nStatoEpisodio INTEGER
	

	DECLARE @dDataInizio AS DATETIME
	DECLARE @dDataFine AS DATETIME
	DECLARE @sCodRuolo AS VARCHAR(20)

	DECLARE @bDatiEstesi AS Bit	
			
	
		DECLARE @sGUID AS VARCHAR(50)
	DECLARE @sDataTmp AS VARCHAR(20)
	DECLARE @nTemp AS INTEGER
	
	DECLARE @bInserisci AS  BIT
	DECLARE @bModifica AS  BIT
	DECLARE @bCancella AS  BIT	
	
	DECLARE @xTmp AS XML
	DECLARE @sCodUA AS VARCHAR(20)
	
	DECLARE @nNumFiltroTipoPrescrizione INTEGER
	DECLARE @nNumFiltroViaSomministrazione INTEGER
	
	
		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDPaziente.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDPaziente') as ValoreParametro(IDPaziente))
	IF 	ISNULL(@sGUID,'') <> '' SET @uIDPaziente=CONVERT(UNIQUEIDENTIFIER,	@sGUID)	

		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDEpisodio.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDEpisodio') as ValoreParametro(IDEpisodio))
	IF 	ISNULL(@sGUID,'') <> '' SET @uIDEpisodio=CONVERT(UNIQUEIDENTIFIER,	@sGUID)
		
		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDTrasferimento.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDTrasferimento') as ValoreParametro(IDTrasferimento))
	IF 	ISNULL(@sGUID,'') <> '' SET @uIDTrasferimento=CONVERT(UNIQUEIDENTIFIER,	@sGUID)	
	
		SET @nStatoEpisodio=(SELECT TOP 1 ValoreParametro.StatoEpisodio.value('.','INTEGER')
					  FROM @xParametri.nodes('/Parametri/StatoEpisodio') as ValoreParametro(StatoEpisodio))											
	SET @nStatoEpisodio=ISNULL(@nStatoEpisodio,0)
	
	
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

		IF @dDataInizio IS NOT NULL AND @dDataFine IS NULL 
			SET @dDataFine=convert(datetime,'01-01-2100',105)
							
		SET @bDatiEstesi=(SELECT TOP 1 ValoreParametro.DatiEstesi.value('.','bit')
					  FROM @xParametri.nodes('/Parametri/DatiEstesi') as ValoreParametro(DatiEstesi))											
	SET @bDatiEstesi=ISNULL(@bDatiEstesi,0)
	
		SET @sCodRuolo=(SELECT TOP 1 ValoreParametro.CodRuolo.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/TimeStamp/CodRuolo') as ValoreParametro(CodRuolo))											
	SET @sCodRuolo=ISNULL(@sCodRuolo,'')	
		
	    DECLARE @xTimeStamp AS XML	

    SET @xTimeStamp=(SELECT TOP 1 ValoreParametro.TS.query('.')
					  FROM @xParametri.nodes('/Parametri/TimeStamp') as ValoreParametro(TS))
	
	SET @xTimeStamp=CONVERT(XML,
						'<Parametri>' + CONVERT(varchar(max),@xTimeStamp) +
						'</Parametri>')
	
				CREATE TABLE #tmpFiltroViaSomministrazione
	(
		Codice VARCHAR(20) COLLATE Latin1_General_CI_AS
	)	
	
	IF @xParametri.exist('/Parametri/ViaSomministrazione')=1
		INSERT INTO #tmpFiltroViaSomministrazione(Codice)	
			SELECT 	ValoreParametro.ViaSomministrazione.value('.','VARCHAR(20)')	
				FROM @xParametri.nodes('/Parametri/ViaSomministrazione') as ValoreParametro(ViaSomministrazione)

	CREATE INDEX IX_FiltroVS ON #tmpFiltroViaSomministrazione (Codice)  

	SET @nNumFiltroViaSomministrazione=(SELECT COUNT(*) FROM #tmpFiltroViaSomministrazione)

				CREATE TABLE #tmpFiltroTipoPrescrizione
	(
		Codice VARCHAR(20) COLLATE Latin1_General_CI_AS
	)	
	
	IF @xParametri.exist('/Parametri/TipoPrescrizione')=1
		INSERT INTO #tmpFiltroTipoPrescrizione(Codice)	
			SELECT 	ValoreParametro.TipoPrescrizione.value('.','VARCHAR(20)')	
				FROM @xParametri.nodes('/Parametri/TipoPrescrizione') as ValoreParametro(TipoPrescrizione)

	CREATE INDEX IX_FiltroTP ON #tmpFiltroTipoPrescrizione (Codice)  

	SET @nNumFiltroTipoPrescrizione=(SELECT COUNT(*) FROM #tmpFiltroTipoPrescrizione)
					
				
	SET @sCodUA=(SELECT TOP 1 CodUA FROM T_MovTrasferimenti WHERE ID=@uIDTrasferimento)	
	
				
				CREATE TABLE #tmpUA
	(
		CodUA VARCHAR(20) COLLATE Latin1_General_CI_AS,
		Descrizione VARCHAR(255) COLLATE Latin1_General_CI_AS
	)
	
	SET @xTmp =CONVERT(XML,'<Parametri><CodUA>'+@sCodUA+'</CodUA></Parametri>')
	
	INSERT #tmpUA EXEC MSP_SelUAPadri @xTmp	
	
	CREATE INDEX IX_CodUA ON #tmpUA (CodUA)    		
		
			
						
	CREATE TABLE #tmpUARuolo
	(
		CodUA VARCHAR(20) COLLATE Latin1_General_CI_AS,
		Descrizione VARCHAR(255) COLLATE Latin1_General_CI_AS
	)


	SET  @xTmp=CONVERT(XML,'<Parametri><CodRuolo>'+ @sCodRuolo + '</CodRuolo></Parametri>')
	
	INSERT #tmpUARuolo EXEC MSP_SelUADaRuolo @xTmp
	
	CREATE INDEX IX_CodUA ON #tmpUARuolo (CodUA)    
		
	
				
	CREATE TABLE #tmpTipoPrescrizione(
		[Codice] [varchar](20) COLLATE Latin1_General_CI_AS NOT NULL 
	)
	
	INSERT INTO #tmpTipoPrescrizione (Codice)    
		SELECT DISTINCT 
			A.CodVoce AS Codice												
		 FROM					
			T_AssUAEntita A
				INNER JOIN #tmpUA T ON
					A.CodUA=T.CodUA	
				INNER JOIN T_AssRuoliAzioni Z
					ON (Z.CodEntita=A.CodEntita AND						
						Z.CodVoce=A.CodVoce AND
						Z.CodRuolo=@sCodRuolo AND
						Z.CodAzione ='INS')					
		 WHERE A.CodEntita='PRF'
		 

	CREATE INDEX IX_Codice ON #tmpTipoPrescrizione (Codice)    
			
							 
												 
		SELECT 			
				
				E.ID AS IDEpisodio,	
				
								ISNULL(TE.Descrizione,'') + 
					(CASE 
						WHEN ISNULL(E.NumeroNosologico,ISNULL(E.NumeroListaAttesa,''))='' THEN ''
						ELSE  ' (' + ISNULL(E.NumeroNosologico,ISNULL(E.NumeroListaAttesa,'')) + ')' 
					END)	
					+ 
						(CASE 
							WHEN DataRicovero IS NOT NULL THEN 
									CHAR(10)+ CHAR(13) + 'Data Accettazione : ' + CONVERT(VARCHAR(10),E.DataRicovero,105) + 
															' ' + CONVERT(VARCHAR(5), E.DataRicovero,14)  
														 + ' (' + ISNULL(AP.Descrizione,'') + ')'
							ELSE ''			
						END)
					+				
						(CASE 
								WHEN E.DataDimissione IS NULL AND QT.DataUscitaUltimo IS NOT NULL THEN 
												CHAR(10) + CHAR(13)  
														+ 'Data Ultimo Trasf. : ' + CONVERT(VARCHAR(10),QT.DataUscitaUltimo,105) 
														+	' ' + CONVERT(VARCHAR(5), QT.DataUscitaUltimo,14) 
														+	' (' +  ISNULL(AU.Descrizione,'') + ')'
														
								WHEN E.DataDimissione IS NOT NULL THEN
													CHAR(10) + CHAR(13) 
														 + 'Data Dimissione : ' + CONVERT(VARCHAR(10),E.DataDimissione,105)
														 +  ' ' + CONVERT(VARCHAR(5), E.DataDimissione,14)  +
														 + ' (' + ISNULL(AU.Descrizione,'') + ')'
								ELSE ''							
						END)
					As Ricovero,																	
				
					Case
						WHEN E.DataDimissione IS NOT NULL THEN 1
						ELSE 0
					END AS Dimesso,							
					
															CASE 
						WHEN 
							TMP.Codice IS NOT NULL AND										EAB.IDEpisodio IS NOT NULL										THEN  M.ID 
						ELSE NULL
					END  AS IDPrescrizione,
					
					I.IDIcona AS IDIcona,
					
										
					REPLACE(
						TP.Descrizione + CHAR(13) + CHAR(10) +
							REPLACE(
									REPLACE(MS.AnteprimaTXT,
											CHAR(9)+CHAR(9),
											CHAR(9)
											),									
									CHAR(9),
									CHAR(13)+CHAR(10)
									) 
						,CHAR(13)+CHAR(10)+CHAR(13)+CHAR(10),
						CHAR(13)+CHAR(10))
						AS  DescrPrescrizione,
					MS.AnteprimaRTF,
					
					E.DataRicovero,
					M.DataEvento,
										
					M.CodTipoPrescrizione AS CodTipoPrescrizione,
					TP.Descrizione AS DescrTipoPrescrizione,
					VS.Codice AS CodViaSomministrazione,
					VS.Descrizione AS DescrViaSomministrazione,
					
										CASE 
						WHEN 
							TMP.Codice IS NOT NULL AND												EAB.IDEpisodio IS NOT NULL	AND											ISNULL(SUV.Versione,0) = MS.Versione									THEN  1 
						ELSE 0
					END
					AS Selezionabile
				INTO #tmpPrescrizioni							
			FROM 			
				T_MovEpisodi AS E
					INNER JOIN T_MovPazienti P
						ON P.IDEpisodio=E.ID
					LEFT JOIN T_TipoEpisodio TE
						ON E.CodTipoEpisodio =TE.Codice	
						
					LEFT JOIN Q_SelEpisodioRicoveroPU QT
						ON E.ID=QT.IDEpisodio
					
					LEFT JOIN T_UnitaAtomiche AP
						ON QT.CodUAPrimo=AP.Codice	
						
					LEFT JOIN T_UnitaAtomiche AU
						ON QT.CodUAUltimo=AU.Codice	
													
					LEFT JOIN 
						(SELECT * FROM 
							T_MovPrescrizioni MP
															
						 WHERE				
														CodStatoPrescrizione  NOT IN ('CA')			
						
														AND
							1=
								CASE	
									WHEN @nNumFiltroTipoPrescrizione=0 THEN 1
									WHEN MP.CodTipoPrescrizione IN (SELECT Codice FROM #tmpFiltroTipoPrescrizione) THEN 1
									ELSE 0
								END	
						
														AND
							1=CASE
								WHEN @nNumFiltroViaSomministrazione=0 THEN 1
								WHEN MP.CodViaSomministrazione IN (SELECT Codice FROM #tmpFiltroViaSomministrazione) THEN 1
								ELSE 0
							END												
						) AS M	
						ON M.IDEpisodio=E.ID
					
										LEFT  JOIN #tmpTipoPrescrizione TMP						
						ON M.CodTipoPrescrizione=TMP.Codice							
						
					LEFT JOIN T_TipoPrescrizione TP
						ON M.CodTipoPrescrizione=TP.Codice
					LEFT JOIN T_ViaSomministrazione VS
						ON M.CodViaSomministrazione=VS.Codice
						
										LEFT JOIN 
						(SELECT 
						
						  IDNum AS IDIcona,	
						  CodTipo,
						  CodStato,
						   						   CONVERT(varbinary(max),null)  As Icona
						 FROM T_Icone
						 WHERE CodEntita='VSM'
						) AS I
							ON (M.CodViaSomministrazione = I.CodTipo AND ''=I.CodStato)							
							
										LEFT JOIN 
						(SELECT ID As IDScheda, 
								CodScheda,
								Versione,
								IDEntita,
								AnteprimaRTF, 
								AnteprimaTXT
						 FROM
							T_MovSchede 
						 WHERE CodEntita = 'PRF' AND							
							Storicizzata = 0
						) AS MS
					ON MS.IDEntita = M.ID
					
					LEFT JOIN 
						Q_SelSchedeUltimaVersione SUV
					ON
						MS.CodScheda=SUV.CodScheda
					
										LEFT JOIN 
						 (SELECT IDEpisodio FROM T_MovTrasferimenti
						  WHERE CodUA IN (SELECT CodUA FROM #tmpUARuolo)
						  GROUP BY IDEpisodio
						  ) EAB
					ON E.ID=EAB.IDEpisodio	  
			WHERE 				
				P.IDPaziente=@uIDPaziente	AND										
								E.CodStatoEpisodio NOT IN ('CA','AN') 
																																																 												
																																																																				
																																	
																																						      
																																				
																																																												
		ORDER BY
			DataRicovero DESC,
			M.DataEvento DESC

	SELECT * FROM #tmpPrescrizioni
	DROP TABLE #tmpPrescrizioni
				
				
	IF @bDatiEstesi=1
		BEGIN
												SELECT 											
				V.Codice,
				V.Descrizione,
				I.Icona AS Icona
			FROM T_TipoPrescrizione V	
																LEFT JOIN
					(SELECT CodTipo,Icona48 AS Icona
					 FROM T_Icone
					 WHERE 
						CodEntita='PRF' AND 
						CodStato=''
					) AS I
				ON V.Codice=I.CodTipo							
				INNER JOIN #tmpTipoPrescrizione TTP	
					ON V.Codice=TTP.Codice
					
			ORDER BY Descrizione	

				
															SELECT 											
				V.Codice,
				V.Descrizione,
				I.Icona AS Icona
			FROM T_ViaSomministrazione V	
				LEFT JOIN
					(SELECT CodTipo,Icona48 AS Icona
					 FROM T_Icone
					 WHERE 
						CodEntita='VSM' AND 
						CodStato=''
					) AS I
				ON V.Codice=I.CodTipo					
			ORDER BY Descrizione	
		END	
		
				
	
	SELECT @nNumFiltroTipoPrescrizione,* FROM #tmpFiltroTipoPrescrizione
	RETURN 0
END