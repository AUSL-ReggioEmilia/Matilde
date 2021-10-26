CREATE PROCEDURE [dbo].[MSP_CercaCartelle](@xParametri AS XML)
AS
BEGIN
	

				
	DECLARE @sFiltroGenerico AS Varchar(500) 
	DECLARE @sDataNascita AS VARCHAR(10)
		
	DECLARE @sCodRuolo AS Varchar(20)
	DECLARE @nNumRighe AS INTEGER
	DECLARE @bSoloCartelleFirmate AS BIT
	
	DECLARE @sOrdinamento AS Varchar(500)
	DECLARE @sCodStatoCartella AS Varchar(MAX)		
	DECLARE @sCodLogin AS VARCHAR(100)
	
		DECLARE @sSQL AS VARCHAR(MAX)
				
		SET @sFiltroGenerico=(SELECT	TOP 1 ValoreParametro.FiltroGenerico.value('.','VARCHAR(500)')
						 FROM @xParametri.nodes('/Parametri/FiltroGenerico') as ValoreParametro(FiltroGenerico))						 
	SET @sFiltroGenerico= ISNULL(@sFiltroGenerico,'')
	SET @sFiltroGenerico=LTRIM(RTRIM(@sFiltroGenerico))
	
		SET @sFiltroGenerico=REPLACE(@sFiltroGenerico,'''','''''')			
	
			SET @sOrdinamento=(SELECT	TOP 1 ValoreParametro.Ordinamento.value('.','VARCHAR(500)')
						 FROM @xParametri.nodes('/Parametri/Ordinamento') as ValoreParametro(Ordinamento))						 
	SET @sOrdinamento= ISNULL(@sOrdinamento,'')
	SET @sOrdinamento=LTRIM(RTRIM(@sOrdinamento))
	
	
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
	
		SET @bSoloCartelleFirmate=(SELECT TOP 1 ValoreParametro.SoloCartelleFirmate.value('.','bit')
					  FROM @xParametri.nodes('/Parametri/SoloCartelleFirmate') as ValoreParametro(SoloCartelleFirmate))											
	SET @bSoloCartelleFirmate=ISNULL(@bSoloCartelleFirmate,0)
		
		SET @sCodStatoCartella=''
	SELECT	@sCodStatoCartella =  @sCodStatoCartella +
														CASE 
								WHEN @sCodStatoCartella='' THEN ''
								ELSE  ','
							END + ''''
						  + ValoreParametro.CodStatoCartella.value('.','VARCHAR(1800)')
						  + ''''
						 FROM @xParametri.nodes('/Parametri/CodStatoCartella') as ValoreParametro(CodStatoCartella)	
	SET @sCodStatoCartella=LTRIM(RTRIM(@sCodStatoCartella))
	IF	@sCodStatoCartella='''''' SET @sCodStatoCartella=''

	
		SET @sCodLogin=(SELECT	TOP 1 ValoreParametro.CodLogin.value('.','VARCHAR(100)')
						 FROM @xParametri.nodes('/Parametri/TimeStamp/CodLogin') as ValoreParametro(CodLogin))						 
	SET @sCodLogin= LTRIM(RTRIM(ISNULL(@sCodLogin,'')))
	
				
				
	DECLARE @xTmp AS XML
			
	CREATE TABLE #tmpUARuolo
	(
		CodUA VARCHAR(20) COLLATE Latin1_General_CI_AS,
		Descrizione VARCHAR(255) COLLATE Latin1_General_CI_AS
	)

	SET  @xTmp=CONVERT(XML,'<Parametri><CodRuolo>'+ @sCodRuolo + '</CodRuolo></Parametri>')
	
	INSERT #tmpUARuolo EXEC MSP_SelUADaRuolo @xTmp
	
	CREATE INDEX IX_CodUA ON #tmpUARuolo (CodUA)    
	
	
				
			
	DECLARE @gIDSessione AS UNIQUEIDENTIFIER
	SET @gIDSessione=NEWID()
	
		SET @sSQL='
			INSERT T_TmpFiltriEpisodi(IDSessione,IDTrasferimento) ' 
	SET @sSQL= @sSQL + 
		'SELECT '
			+ CASE 
				WHEN(ISNULL(@nNumRighe,0)=0) THEN ''
				ELSE ' TOP ' + CONVERT(VARCHAR(20),@nNumRighe) 
			END 	
			+ ' '''  + convert(varchar(50),@gIDSessione) + ''' AS IDSessione,	'
			+ ' T.ID AS IDTrasferimento		
				  
		 FROM T_MovEpisodi M							
			INNER JOIN T_MovTrasferimenti T
				ON M.ID=T.IDEpisodio
			
			INNER JOIN T_MovPazienti P
				ON M.ID=P.IDEpisodio		
											 
			LEFT JOIN T_MovCartelle CR
				ON T.IDCartella=CR.ID	
			
			INNER JOIN #tmpUARuolo Tmp		
				ON T.CodUA=Tmp.CodUA							
			
			INNER JOIN Q_SelUltimoTrasferimentoCartella QU 
				ON (T.IDEpisodio=QU.IDEpisodio AND
					T.IDNum=QU.IDNumTrasferimento AND
					T.IDCartella=QU.IDCartella)										
			'	

		IF @bSoloCartelleFirmate=1
	BEGIN
		SET @sSQL= @sSQL + 
			' INNER JOIN Q_SelUltimoDocumentoCartellaFirmata UCF
				ON (T.IDCartella=UCF.IDCartella) '	
	END
					
				
	DECLARE @sWhere AS VARCHAR(Max)
	
	SET @sWhere=''
	
		SET @sWhere= @sWhere + ' AND  T.IDCartella IS NOT NULL '
	
		IF @bSoloCartelleFirmate=1 AND ISNULL(@sFiltroGenerico,'')='' AND ISNULL(@sDataNascita,'')=''  
		SET @sWhere= @sWhere + ' AND  1=0 '
		
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
					SET @sWhere= @sWhere + '     )' 					
	END
						
		IF ISNULL(@sCodStatoCartella,'') <> ''
	BEGIN
		SET @sWhere= @sWhere + ' AND ISNULL(CR.CodStatoCartella,''DA'') IN ('+ @sCodStatoCartella + ')' 
	END

		IF @bSoloCartelleFirmate=1 
	BEGIN
		SET @sWhere= @sWhere + ' AND CR.CodStatoCartella = UCF.UltimoCodStatoEntita '
	END

		IF ISNULL(@sWhere,'')<> ''
	BEGIN		
		SET @sSQL=@sSQL +' WHERE ' + RIGHT(@sWhere,len(@sWhere)-5)		
	END	
	
	PRINT @sSQL
		EXEC (@sSQL)
		
					
			
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
								  
			  M.CodTipoEpisodio,			  
			  M.NumeroNosologico,
			  M.NumeroListaAttesa,
			  ISNULL(M.CodTipoEpisodio,'''') + ''-'' + 
				CASE
					WHEN ISNULL(NumeroNosologico,'''')='''' THEN ISNULL(NumeroListaAttesa,'''')
					ELSE NumeroNosologico
				END AS DescEpisodio,					
			  			  
			  ISNULL(CR.CodStatoCartella,''DA'') AS CodStatoCartella,
			  SC.Descrizione As DescrStatoCartella,
			  SC.Colore AS ColoreStatoCartella,			 
			  NULL AS IconaStatoCartella,
			  CR.NumeroCartella,
			  CR.ID AS IDCartella,
			  LI.Descrizione AS MedicoFirma,
			  CASE 
				WHEN MD.IDNum IS NOT NULL THEN
					Convert(varchar(20),DataInserimento,105) + '' '' + Convert(varchar(5),DataInserimento,114)
				ELSE '''' 
			  END AS DataFirma,
			  MD.ID AS IDUltimoDocumentoFirmato,
			  CR.CodStatoCartellaInfo,
			  CR.MotivoRiapertura
		FROM T_MovEpisodi M
			INNER JOIN T_MovTrasferimenti T
				ON M.ID=T.IDEpisodio
				
			INNER JOIN T_MovPazienti P
					ON M.ID=P.IDEpisodio
					
			LEFT JOIN T_MovCartelle CR
					ON T.IDCartella=CR.ID					
					
			INNER JOIN T_TmpFiltriEpisodi TMP
					ON T.ID=TMP.IDTrasferimento		
					
			LEFT JOIN T_UnitaAtomiche UA
				ON T.CodUA=UA.Codice		
				
			LEFT JOIN T_StatoCartella SC
				ON CR.CodStatoCartella=SC.Codice
			
			LEFT JOIN Q_SelUltimoDocumentoCartellaFirmata UCF
				ON T.IDCartella=UCF.IDCartella

			LEFT JOIN T_MovDocumentiFirmati MD
				ON MD.IDNum=UCF.IDNumUltimaDocumentoCartellaFirmata
				
			LEFT JOIN T_Login LI 
				ON LI.Codice=MD.CodUtenteInserimento
					
				
		WHERE
			TMP.IDSessione=''' + convert(varchar(50),@gIDSessione) +'''	
			AND T.CodStatoTrasferimento <> ''CA''
		'	  		

		IF @sOrdinamento<> ''
		SET @sSQL = @sSQL + ' ORDER BY ' + @sOrdinamento + ''
			
	
		EXEC (@sSQL)
	
	SET @sSQL=''
		
				
		DROP TABLE #tmpUARuolo	
	
		DELETE FROM T_TmpFiltriEpisodi 
	WHERE IDSessione=@gIDSessione
								
		           
	RETURN 0
	
END