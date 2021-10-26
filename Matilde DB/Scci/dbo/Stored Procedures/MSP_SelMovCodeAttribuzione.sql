CREATE PROCEDURE [dbo].[MSP_SelMovCodeAttribuzione](@xParametri AS XML )
AS
BEGIN
	

				DECLARE @sData AS VARCHAR(20)
	DECLARE @dData AS DATETIME	
	DECLARE @dDataData AS Date
	DECLARE @bNascondiCompletati AS BIT	
	DECLARE @bNascondiAssegnati AS BIT
	
	DECLARE @sCodAgenda AS Varchar(MAX)
	DECLARE @sCodTipoTaskInfermieristico AS Varchar(MAX)
	DECLARE @sFiltroGenerico AS Varchar(500) 
	DECLARE @sFiltroNumeroCoda AS Varchar(500) 
	DECLARE @sCodRuolo AS Varchar(20)
	DECLARE @sCodiceFiscale AS Varchar(50)
	
	
		DECLARE @sSQL AS VARCHAR(MAX)
	DECLARE @sSQL1 AS VARCHAR(MAX)
	DECLARE @sSQL2 AS VARCHAR(MAX)
	DECLARE @sSQL3 AS VARCHAR(MAX)
	DECLARE @sWhere AS VARCHAR(Max)
	DECLARE @sDataTmp AS VARCHAR(20)
		
		SET @sData=(SELECT	TOP 1 ValoreParametro.Data.value('.','VARCHAR(20)')
						 FROM @xParametri.nodes('/Parametri/Data') as ValoreParametro(Data))						 
	SET @sData= ISNULL(@sData,'')
	SET @sData=LTRIM(RTRIM(@sData))


	SET @sDataTmp=@sData
	IF @sDataTmp<> '' 
		BEGIN
						SET @sDataTmp= SUBSTRING(@sDataTmp,7,4) + '-' + SUBSTRING(@sDataTmp,4,2) + '-' 
						+ LEFT(@sDataTmp,2) +
						' ' + RIGHT(@sDataTmp,5)
						IF ISDATE(@sDataTmp)=1
			BEGIN
				SET	@dData=CONVERT(Datetime,@sDataTmp,120)
				SET @dDataData = CONVERT(date, CONVERT(varchar(20), @dData, 103), 103)
							END						
			ELSE
				SET	@dData =NULL			
		END


		SET @sFiltroGenerico=(SELECT	TOP 1 ValoreParametro.FiltroGenerico.value('.','VARCHAR(500)')
						 FROM @xParametri.nodes('/Parametri/FiltroGenerico') as ValoreParametro(FiltroGenerico))						 
	SET @sFiltroGenerico= ISNULL(@sFiltroGenerico,'')
	SET @sFiltroGenerico=LTRIM(RTRIM(@sFiltroGenerico))
	
		
		IF ISNUMERIC(@sFiltroGenerico)=1 
		BEGIN
			IF LEN(LTRIM(RTRIM(@sFiltroGenerico))) <= 4
			BEGIN
				SET @sFiltroNumeroCoda=CONVERT(VARCHAR(10),CONVERT(NUMERIC,@sFiltroGenerico))
				SET @sFiltroGenerico = ''
			END
				
		END
		
	ELSE
		SET @sFiltroNumeroCoda=NULL
	
		SET @sCodiceFiscale=(SELECT	TOP 1 ValoreParametro.CodiceFiscale.value('.','VARCHAR(50)')
						 FROM @xParametri.nodes('/Parametri/CodiceFiscale') as ValoreParametro(CodiceFiscale))		
		
		SET @bNascondiCompletati=(SELECT TOP 1 ValoreParametro.NascondiCompletati.value('.','bit')
					  FROM @xParametri.nodes('/Parametri/NascondiCompletati') as ValoreParametro(NascondiCompletati))											
					 
	SET @bNascondiCompletati=ISNULL(@bNascondiCompletati,1)
	
		SET @bNascondiAssegnati=(SELECT TOP 1 ValoreParametro.NascondiAssegnati.value('.','bit')
					 FROM @xParametri.nodes('/Parametri/NascondiAssegnati') as ValoreParametro(NascondiAssegnati))											
	SET @bNascondiAssegnati=ISNULL(@bNascondiAssegnati,0)
		
				CREATE TABLE #tmpAgende
	(
		Codice VARCHAR(20)  COLLATE Latin1_General_CI_AS
	)	
	
	IF @xParametri.exist('/Parametri/CodAgenda')=1
		INSERT INTO #tmpAgende(Codice)	
			SELECT 	ValoreParametro.CodAgenda.value('.','VARCHAR(20)')	
				FROM @xParametri.nodes('/Parametri/CodAgenda') as ValoreParametro(CodAgenda)
																											
				CREATE TABLE #tmpTaskInfermieristici
	(
		Codice VARCHAR(20)  COLLATE Latin1_General_CI_AS
	)	
	
	IF @xParametri.exist('/Parametri/CodTipoTaskInfermieristico')=1
		INSERT INTO #tmpTaskInfermieristici(Codice)	
			SELECT 	ValoreParametro.CodTipoTaskInfermieristico.value('.','VARCHAR(20)')	
				FROM @xParametri.nodes('/Parametri/CodTipoTaskInfermieristico') as ValoreParametro(CodTipoTaskInfermieristico)
	
		SET @sCodRuolo=(SELECT	TOP 1 ValoreParametro.CodRuolo.value('.','VARCHAR(20)')
						 FROM @xParametri.nodes('/Parametri/TimeStamp/CodRuolo') as ValoreParametro(CodRuolo))						 
	SET @sCodRuolo= LTRIM(RTRIM(ISNULL(@sCodRuolo,'')))		
	
				
	DECLARE @xTmp AS XML
			
	CREATE TABLE #tmpUARuolo
	(
		CodUA VARCHAR(20) COLLATE Latin1_General_CI_AS,
		Descrizione VARCHAR(255) COLLATE Latin1_General_CI_AS
	)


	SET  @xTmp=CONVERT(XML,'<Parametri><CodRuolo>'+ @sCodRuolo + '</CodRuolo></Parametri>')
	
		INSERT #tmpUARuolo EXEC MSP_SelUADaRuolo @xTmp
	
	CREATE INDEX IX_CodUA ON #tmpUARuolo (CodUA)   
		
				
	CREATE TABLE #tmpAppTask
	(
		IdCoda uniqueidentifier,
		NumeroCoda int,
		Priorita int,
		DescrPaziente varchar(max) COLLATE Latin1_General_CI_AS,
		CodEntita varchar(20) COLLATE Latin1_General_CI_AS,
		IdEntita uniqueidentifier,
		DataInizio datetime,
		DataFine datetime,
		Tipo varchar(255) COLLATE Latin1_General_CI_AS,
		Descrizione varchar(2000) COLLATE Latin1_General_CI_AS,
		CodStato varchar(20) COLLATE Latin1_General_CI_AS,
		Stato varchar(255) COLLATE Latin1_General_CI_AS,
		IdEpisodio uniqueidentifier,
		Trasferimento uniqueidentifier,
		IdPaziente uniqueidentifier,
		IdCartella uniqueidentifier,
		Icona varbinary(max),
		IdIcona numeric(18,0),
		Anteprima varchar(max) COLLATE Latin1_General_CI_AS,
		CodUA varchar(20) COLLATE Latin1_General_CI_AS,
		CodTipoAppuntamento varchar(20) COLLATE Latin1_General_CI_AS,
		CodTipoTaskInfermieristico varchar(20) COLLATE Latin1_General_CI_AS,
		CodStatoTaskInfermieristico varchar(20) COLLATE Latin1_General_CI_AS,		
		IdPazienteFuso uniqueidentifier, 		CodAgendaApp varchar(20) COLLATE Latin1_General_CI_AS,
		Cognome varchar(255) COLLATE Latin1_General_CI_AS,
		Nome varchar(255) COLLATE Latin1_General_CI_AS,
		NumeroNosologico varchar(20) COLLATE Latin1_General_CI_AS,
		NumeroListaAttesa VARCHAR(20) COLLATE Latin1_General_CI_AS,
		NumeroCartella varchar(50) COLLATE Latin1_General_CI_AS,
		NrNoso_NrCartella varchar(100) COLLATE Latin1_General_CI_AS,
		CodFiscale varchar(50) COLLATE Latin1_General_CI_AS
		)



		INSERT INTO #tmpAppTask
		(IdCoda, NumeroCoda, Priorita, DescrPaziente,	CodEntita, IdEntita, DataInizio, DataFine,
		 Tipo, Descrizione, CodStato, Stato, IdEpisodio, Trasferimento, IdPaziente, IdCartella, Icona,
		 Anteprima, CodUA, CodTipoAppuntamento, IdPazienteFuso, CodAgendaApp, Cognome, Nome, CodFiscale)

		SELECT
		CDA.ID AS IDCoda,	
		CDA.NumeroCoda AS NumeroCoda,	
		CDA.Priorita,			
		CASE 
			WHEN ISNULL(CDA.Priorita, 0) <> 0 THEN ' '
			ELSE ''
		END +
		' ' +								
		CASE
			WHEN CDA.NumeroCoda IS NULL THEN ''
			ELSE RIGHT('0000' + CONVERT(VARCHAR(4),CDA.NumeroCoda),4) + ' '
		END
		+ ISNULL(P.Cognome,'') + ' ' + ISNULL(P.Nome,'') + ' (' + ISNULL(P.Sesso,'') + ') ' +
									CASE
										WHEN P.DataNascita IS NOT NULL THEN ' (' + CONVERT(VARCHAR(10),P.DataNascita,105) + ') '
										ELSE ''
									END 
		AS DescrPaziente,
		'APP' AS CodEntita,
		APP.ID AS IDEntita,
		APP.DataInizio,
		APP.DataFine,					
		TA.Descrizione AS Tipo,
		APP.ElencoRisorse AS Descrizione,
		APP.CodStatoAppuntamento AS CodStato,
		STA.Descrizione AS Stato,
		APP.IDEpisodio AS IDEpisodio,
		NULL AS Trasferimento,
		P.ID as IdPaziente,
		NULL as IdCartella,
		CONVERT(varbinary(max),null)  As Icona,			
		MS.AnteprimaTXT AS Anteprima,
		APP.CodUA,
		APP.CodTipoAppuntamento ,
		P.ID as IdPazienteFuso,
		APAGE.CodAgenda as CodAgendaApp,
		P.Cognome,
		P.Nome,
		ISNULL(P.CodiceFiscale, '') as CodiceFiscale
		FROM 
		T_MovAppuntamenti APP 
		INNER JOIN T_MovAppuntamentiAgende APAGE ON APAGE.IDAppuntamento = APP.ID
		INNER JOIN T_Pazienti P ON P.Id = APP.IDPaziente
		INNER JOIN #tmpAgende AGE ON AGE.Codice = APAGE.CodAgenda
		LEFT OUTER JOIN T_MovCodeEntita REL ON (REL.CodEntita = 'APP' AND REL.IDEntita = APP.ID AND REL.CodStatoCodaEntita NOT IN ('CA') )
		LEFT OUTER JOIN T_MovCode CDA ON (CDA.ID = REL.IDCoda AND CDA.CodStatoCoda <> 'CA')
		LEFT JOIN T_TipoAppuntamento TA	ON TA.Codice = APP.CodTipoAppuntamento								
		LEFT JOIN T_StatoAppuntamento STA ON STA.Codice = APP.CodStatoAppuntamento
		LEFT JOIN T_MovSchede MS ON MS.CodEntita='APP' AND
										MS.IDEntita=APP.ID AND
										MS.Storicizzata=0
							
		WHERE	
		APP.DataInizio >= @dDataData AND
		APP.DataInizio < DATEADD(DAY, 1, @dDataData) AND
		APAGE.CodStatoAppuntamentoAgenda <> 'CA' AND
				APP.CodStatoAppuntamento NOT IN ('CA','AN','TR')


		
		INSERT INTO #tmpAppTask
		(IdCoda, NumeroCoda, Priorita, DescrPaziente, CodEntita, IdEntita, DataInizio, DataFine, Tipo,
		Descrizione, CodStato, Stato, IdEpisodio, Trasferimento, IdPaziente, IdCartella, Icona, IdIcona, 
		Anteprima, CodUA, CodTipoTaskInfermieristico, CodStatoTaskInfermieristico, IdPazienteFuso, Cognome,	Nome, NumeroNosologico,
		NumeroListaAttesa, NumeroCartella, CodFiscale)

		 	
			SELECT 		
			CDA.ID AS IDCoda,		
			CDA.NumeroCoda AS NumeroCoda,
			CDA.Priorita,
			CASE 
				WHEN ISNULL(CDA.Priorita, 0) <> 0 THEN ' '
				ELSE ''
			END +
			' ' +
			CASE
				WHEN CDA.NumeroCoda IS NULL THEN ''
				ELSE RIGHT('0000' + CONVERT(VARCHAR(4),CDA.NumeroCoda),4) + ' '
			END
			+ ISNULL(P.Cognome,'') + ' ' + ISNULL(P.Nome,'') + ' (' + ISNULL(P.Sesso,'') + ') ' +
										CASE
											WHEN P.DataNascita IS NOT NULL THEN ' (' + CONVERT(VARCHAR(10),P.DataNascita,105) + ') '
											ELSE ''
										END 
			AS DescrPaziente,
			'WKI' AS CodEntita,
			WKI.ID AS IDEntita,
			WKI.DataProgrammata,
			NULL AS DataFine,					
			TT.Descrizione AS Tipo,
			NULL AS Descrizione,
			STT.Codice AS CodStato,
			STT.Descrizione AS Stato,		
			M.ID AS IDEpisodio,
			T.ID AS Trasferimento,
			P.IDPaziente,
			T.IDCartella,
			CONVERT(varbinary(max),null)  As Icona,
			null AS IDIcona,
			ISNULL(M.NumeroNosologico,'')  +
			' - ' + ISNULL(CR.NumeroCartella,'')
			AS Anteprima,
			T.CodUA,
			WKI.CodTipoTaskInfermieristico,
			WKI.CodStatoTaskInfermieristico,
			P.IDPaziente,
			P.Cognome,
			P.Nome,
			M.NumeroNosologico,
			M.NumeroListaAttesa,
			CR.NumeroCartella,
			ISNULL(P.CodiceFiscale, '')
		FROM
			T_MovEpisodi M
			INNER JOIN T_MovPazienti AS P ON P.IDEpisodio = M.ID
			INNER JOIN T_MovTrasferimenti AS T ON T.IDEpisodio = M.ID
			INNER JOIN T_MovCartelle AS CR ON CR.ID  = T.IDCartella
			INNER JOIN T_MovTaskInfermieristici WKI	ON (WKI.IDEpisodio=M.ID AND WKI.IDTrasferimento = T.ID AND WKI.CodStatoTaskInfermieristico NOT IN ('CA'))
			INNER JOIN #tmpTaskInfermieristici TMPWKI ON WKI.CodTipoTaskInfermieristico=TMPWKI.Codice					
			LEFT JOIN T_TipoTaskInfermieristico TT ON WKI.CodTipoTaskInfermieristico=TT.Codice					
			LEFT JOIN T_StatoTaskInfermieristico STT ON STT.Codice=WKI.CodStatoTaskInfermieristico				
			LEFT OUTER JOIN T_MovCodeEntita REL ON (REL.CodEntita='WKI' AND REL.IDEntita=WKI.ID AND
								REL.CodStatoCodaEntita <> 'CA')											
			LEFT OUTER JOIN T_MovCode CDA ON (CDA.ID = REL.IDCoda AND CDA.CodStatoCoda <> 'CA')
		WHERE
		WKI.DataProgrammata >= @dDataData AND 
		WKI.DataProgrammata < DATEADD(DAY, 1, @dDataData)





	UPDATE #tmpAppTask
	SET 
		NumeroNosologico = E.NumeroNosologico,
		NumeroListaAttesa = E.NumeroListaAttesa
	FROM #tmpAppTask AC
	INNER JOIN T_MovEpisodi E ON E.ID = AC.IdEpisodio
	WHERE
	AC.CodEntita = 'APP'
	
	


	UPDATE #tmpAppTask
	SET IdIcona = (SELECT ICO.IDNum FROM T_Icone ICO 
					WHERE 
						ICO.CodTipo = #tmpAppTask.CodTipoAppuntamento AND 
						ICO.CodStato = #tmpAppTask.CodStato AND
						ICO.CodEntita = 'APP'
						)
						
	UPDATE #tmpAppTask
	SET IdIcona = (SELECT 
					IDNum AS IDIcona
					FROM T_Icone ICO WITH(NOLOCK)
					WHERE 
					CodEntita='WKI' AND
					ICO.CodTipo = #tmpAppTask.CodTipoTaskInfermieristico AND 	
					ICO.CodStato = #tmpAppTask.CodStatoTaskInfermieristico
					)							


	UPDATE #tmpAppTask
	SET IdPazienteFuso = PA.IDPaziente
	FROM
	#tmpAppTask 
	INNER JOIN T_PazientiAlias PA ON PA.IDPazienteVecchio = #tmpAppTask.IdPaziente	



		CREATE TABLE #tmpElencoTrasferimenti
		(IdEpisodio uniqueidentifier, 
		 IdCartella uniqueidentifier, 
		 IdTrasferimento uniqueidentifier,
		 NumeroCartella varchar(50) COLLATE Latin1_General_CI_AS,
		 DataIngresso datetime)


		INSERT INTO #tmpElencoTrasferimenti
		(IdEpisodio, IdCartella, IdTrasferimento, NumeroCartella, DataIngresso)	
		SELECT
		AC.IdEpisodio, 
		T.IDCartella,
		T.ID,
		CR.NumeroCartella,
		T.DataIngresso
		FROM #tmpAppTask AC
		INNER JOIN T_MovEpisodi E ON E.ID = AC.IdEpisodio						
		INNER JOIN T_MovTrasferimenti AS T ON (T.IDEpisodio = E.ID) AND	T.CodStatoTrasferimento IN ('AT','PR','DM')				
		INNER JOIN T_MovCartelle AS CR ON CR.ID = T.IDCartella
		INNER JOIN #tmpUARuolo R ON R.CodUA = T.CodUA
		WHERE
		CodEntita = 'APP'
		GROUP BY AC.IdEpisodio, T.IDCartella, T.ID, CR.NumeroCartella, T.DataIngresso


						
		CREATE TABLE #tmpUltimoTrasferimento
		(IdEpisodio uniqueidentifier, 
		 IdCartella uniqueidentifier, 
		 IdTrasferimento uniqueidentifier,
		 NumeroCartella varchar(50) COLLATE Latin1_General_CI_AS,
		 DataIngresso datetime)
		 
		 
		INSERT INTO #tmpUltimoTrasferimento
		(IdEpisodio, IdCartella, IdTrasferimento, NumeroCartella, DataIngresso)
		SELECT 
			ET.IdEpisodio,
			ET.IdCartella,
			ET.IdTrasferimento,
			ET.NumeroCartella,
			ET.DataIngresso
		FROM #tmpElencoTrasferimenti ET
		INNER JOIN
			(SELECT ET1.IDEpisodio, ET1.IdCartella, MAX(ET1.DataIngresso) as UltDataIngresso
			 FROM #tmpElencoTrasferimenti ET1
			 GROUP BY ET1.IdEpisodio, ET1.IdCartella) X 
			 ON X.IdEpisodio = ET.IdEpisodio AND X.IdCartella = ET.IdCartella AND X.UltDataIngresso = ET.DataIngresso

	
		
		UPDATE #tmpAppTask
		SET
		Trasferimento = UT.IdTrasferimento,
		IdCartella = UT.IdCartella,
		NumeroCartella = UT.NumeroCartella,
		NrNoso_NrCartella = ISNULL(AC.NumeroNosologico, '')  +
						' - ' + ISNULL(UT.NumeroCartella, '') ,
		Anteprima = ISNULL(AC.NumeroNosologico,'')  +
						' - ' + ISNULL(UT.NumeroCartella,'') + CHAR(13) + CHAR(10) + Anteprima
		FROM #tmpAppTask AC
		INNER JOIN #tmpUltimoTrasferimento UT ON UT.IdEpisodio = AC.IdEpisodio AND AC.CodEntita = 'APP'




		IF @bNascondiAssegnati=1
	BEGIN
		DELETE FROM #tmpAppTask
		WHERE
		IdCoda IS NOT NULL
	END
	
		IF @bNascondiCompletati=1
	BEGIN
					
		DELETE FROM #tmpAppTask
		WHERE
		CodEntita = 'APP' AND
		ISNULL(CodStato, '') <> 'PR'
		
		
		DELETE FROM #tmpAppTask
		WHERE
		CodEntita = 'WKI' AND
		ISNULL(CodStatoTaskInfermieristico, '') <> 'PR'

	END
	ELSE
				BEGIN
						DELETE FROM #tmpAppTask
			WHERE
			CodEntita = 'APP' AND
			CodStato IN ('CA', 'AN', 'TR')
			
			
			DELETE FROM #tmpAppTask
			WHERE
			CodEntita = 'WKI' AND
			CodStato IN ('CA', 'AN', 'TR')
			
		END	







	CREATE TABLE #tmpAppTaskFiltrati
	(
		IdCoda uniqueidentifier,
		NumeroCoda int,
		Priorita int,
		DescrPaziente varchar(max) COLLATE Latin1_General_CI_AS,
		CodEntita varchar(20) COLLATE Latin1_General_CI_AS,
		IdEntita uniqueidentifier,
		DataInizio datetime,
		DataFine datetime,
		Tipo varchar(255) COLLATE Latin1_General_CI_AS,
		Descrizione varchar(2000) COLLATE Latin1_General_CI_AS,
		CodStato varchar(20) COLLATE Latin1_General_CI_AS,
		Stato varchar(255) COLLATE Latin1_General_CI_AS,
		IdEpisodio uniqueidentifier,
		Trasferimento uniqueidentifier,
		IdPaziente uniqueidentifier,
		IdCartella uniqueidentifier,
		Icona varbinary(max),
		IdIcona numeric(18,0),
		Anteprima varchar(max) COLLATE Latin1_General_CI_AS,
		CodUA varchar(20) COLLATE Latin1_General_CI_AS,
		CodTipoAppuntamento varchar(20) COLLATE Latin1_General_CI_AS,
		CodTipoTaskInfermieristico varchar(20) COLLATE Latin1_General_CI_AS,
		CodStatoTaskInfermieristico varchar(20) COLLATE Latin1_General_CI_AS,
		IdPazienteFuso uniqueidentifier, 		CodAgendaApp varchar(20) COLLATE Latin1_General_CI_AS,
		Cognome varchar(255) COLLATE Latin1_General_CI_AS,
		Nome varchar(255) COLLATE Latin1_General_CI_AS,
		NumeroNosologico varchar(20) COLLATE Latin1_General_CI_AS,
		NumeroListaAttesa VARCHAR(20) COLLATE Latin1_General_CI_AS,
		NumeroCartella varchar(50) COLLATE Latin1_General_CI_AS,
		NrNoso_NrCartella varchar(100) COLLATE Latin1_General_CI_AS,
		CodFiscale varchar(50) COLLATE Latin1_General_CI_AS
		)


					IF ISNULL(@sFiltroNumeroCoda, '') <> ''
	BEGIN	
		
		INSERT INTO #tmpAppTaskFiltrati
		SELECT AC.* FROM 
		#tmpAppTask AC
		LEFT OUTER JOIN #tmpAppTaskFiltrati ACF ON ACF.IdEntita=AC.IdEntita
		WHERE
			PATINDEX(@sFiltroNumeroCoda + '%', CONVERT(varchar(500), AC.NumeroCoda)) >0
	END			
	
		
	IF ISNULL(@sCodiceFiscale, '') <> ''
	BEGIN	
		
		INSERT INTO #tmpAppTaskFiltrati
		SELECT AC.* FROM 
		#tmpAppTask AC
		LEFT OUTER JOIN #tmpAppTaskFiltrati ACF ON ACF.IdEntita=AC.IdEntita
		WHERE
		AC.CodFiscale = @sCodiceFiscale  AND
		ACF.IdEntita IS NULL	
	END				
		
	
	IF ISNULL(@sFiltroGenerico, '') <> ''
	BEGIN
		
		INSERT INTO #tmpAppTaskFiltrati
		SELECT AC.* FROM 
		#tmpAppTask AC
		LEFT OUTER JOIN #tmpAppTaskFiltrati ACF ON ACF.IdEntita=AC.IdEntita
		WHERE
			AC.Cognome LIKE '%' + @sFiltroGenerico + '%' AND
			ACF.IdEntita IS NULL
						
		INSERT INTO #tmpAppTaskFiltrati
		SELECT AC.* FROM #tmpAppTask AC
		LEFT OUTER JOIN #tmpAppTaskFiltrati ACF ON ACF.IdEntita=AC.IdEntita
		WHERE
			AC.Nome LIKE '%' + @sFiltroGenerico + '%'
						
		INSERT INTO #tmpAppTaskFiltrati
		SELECT AC.* FROM #tmpAppTask AC
		LEFT OUTER JOIN #tmpAppTaskFiltrati ACF ON ACF.IdEntita=AC.IdEntita
		WHERE
			AC.Cognome + ' ' + AC.Nome LIKE '%' + @sFiltroGenerico + '%' AND
			ACF.IdEntita IS NULL			
			
		INSERT INTO #tmpAppTaskFiltrati
		SELECT AC.* FROM #tmpAppTask AC
		LEFT OUTER JOIN #tmpAppTaskFiltrati ACF ON ACF.IdEntita=AC.IdEntita
		WHERE
			AC.Nome + ' ' + AC.Cognome LIKE '%' + @sFiltroGenerico + '%'	 AND
			ACF.IdEntita IS NULL	
			

		INSERT INTO #tmpAppTaskFiltrati
		SELECT AC.* FROM #tmpAppTask AC
		LEFT OUTER JOIN #tmpAppTaskFiltrati ACF ON ACF.IdEntita=AC.IdEntita
		WHERE
			AC.Nome + ' ' + AC.Cognome LIKE '%' + @sFiltroGenerico + '%'	 AND
			ACF.IdEntita IS NULL	
			

		INSERT INTO #tmpAppTaskFiltrati
		SELECT AC.* FROM #tmpAppTask AC
		LEFT OUTER JOIN #tmpAppTaskFiltrati ACF ON ACF.IdEntita=AC.IdEntita
		WHERE
			AC.NumeroNosologico LIKE '%' + @sFiltroGenerico + '%'	 AND
			ACF.IdEntita IS NULL	

		INSERT INTO #tmpAppTaskFiltrati
		SELECT AC.* FROM #tmpAppTask AC
		LEFT OUTER JOIN #tmpAppTaskFiltrati ACF ON ACF.IdEntita=AC.IdEntita
		WHERE
			AC.NumeroNosologico LIKE '%' + dbo.MF_ModificaNosologico(@sFiltroGenerico) + '%'	 AND
			ACF.IdEntita IS NULL	
			
						
		INSERT INTO #tmpAppTaskFiltrati
		SELECT AC.* FROM #tmpAppTask AC
		LEFT OUTER JOIN #tmpAppTaskFiltrati ACF ON ACF.IdEntita=AC.IdEntita
		WHERE
			AC.NumeroCartella LIKE '%' + dbo.MF_NumeroCartellaDaBarcode(@sFiltroGenerico) + '%'	 AND
			ACF.IdEntita IS NULL	
			
					
	END
	
	IF (ISNULL(@sFiltroNumeroCoda, '') = '' AND ISNULL(@sFiltroGenerico, '') = '' AND ISNULL(@sCodiceFiscale, '') = '' )
	BEGIN
	
			
		INSERT INTO #tmpAppTaskFiltrati
		SELECT AC.* FROM #tmpAppTask AC
		LEFT OUTER JOIN #tmpAppTaskFiltrati ACF ON ACF.IdEntita=AC.IdEntita
		WHERE
			ACF.IdEntita IS NULL		
	END

	


	CREATE TABLE #tmpPazientiInCoda
	(IdPaziente uniqueidentifier,
	 IdPazienteFuso uniqueidentifier,
	 DescrPaziente varchar(max) COLLATE Latin1_General_CI_AS)
	
	
			
	INSERT INTO #tmpPazientiInCoda
	(IdPaziente, IdPazienteFuso, DescrPaziente)
	SELECT
	IdPaziente,
	IdPazienteFuso,
	DescrPaziente
	FROM #tmpAppTaskFiltrati
	GROUP BY IdPaziente, IdPazienteFuso, DescrPaziente


	
	INSERT INTO #tmpPazientiInCoda
	(IdPaziente, IdPazienteFuso, DescrPaziente)
	SELECT 
	PA.IDPazienteVecchio, PA.IDPaziente, PC.DescrPaziente
	FROM
	#tmpPazientiInCoda PC
	INNER JOIN T_PazientiAlias PA ON PA.IDPaziente = PC.IdPazienteFuso
	WHERE
	NOT EXISTS (SELECT TPC.IDPaziente FROM #tmpPazientiInCoda TPC WHERE TPC.IdPaziente = PA.IDPazienteVecchio AND TPC.IdPazienteFuso = PA.IDPaziente)
	GROUP BY PA.IDPazienteVecchio, PA.IDPaziente, PC.DescrPaziente






		CREATE TABLE #tmpAltriAppuntamenti
		(
		IdCoda uniqueidentifier,
		NumeroCoda int,
		Priorita int, 
		DescrPaziente varchar(max) COLLATE Latin1_General_CI_AS,
		CodEntita varchar(20) COLLATE Latin1_General_CI_AS,
		IdEntita uniqueidentifier,
		DataInizio datetime,
		DataFine datetime,
		Tipo varchar(255) COLLATE Latin1_General_CI_AS,
		Descrizione varchar(2000) COLLATE Latin1_General_CI_AS,
		CodStato varchar(20) COLLATE Latin1_General_CI_AS,
		Stato varchar(255) COLLATE Latin1_General_CI_AS,
		IdEpisodio uniqueidentifier,
		Trasferimento uniqueidentifier,
		IdPaziente uniqueidentifier,
		IdCartella uniqueidentifier,
		Icona varbinary(max),
		IdIcona numeric(18,0),
		Anteprima varchar(max) COLLATE Latin1_General_CI_AS,
		CodUA varchar(20) COLLATE Latin1_General_CI_AS,
		CodTipoAppuntamento varchar(20) COLLATE Latin1_General_CI_AS,
		IdPazienteFuso uniqueidentifier, 		CodAgendaApp varchar(20) COLLATE Latin1_General_CI_AS,
		Cognome varchar(255) COLLATE Latin1_General_CI_AS,
		Nome varchar(255) COLLATE Latin1_General_CI_AS,
		NumeroNosologico varchar(20) COLLATE Latin1_General_CI_AS,
		NumeroListaAttesa VARCHAR(20) COLLATE Latin1_General_CI_AS,
		NumeroCartella varchar(50) COLLATE Latin1_General_CI_AS,
		NrNoso_NrCartella varchar(100) COLLATE Latin1_General_CI_AS,
		DescrPazienteOrig varchar(max) COLLATE Latin1_General_CI_AS
		)

	INSERT INTO #tmpAltriAppuntamenti
	(IdCoda, NumeroCoda, Priorita, DescrPaziente,	CodEntita, IdEntita, DataInizio, DataFine,
	 Tipo, Descrizione, CodStato, Stato, IdEpisodio, Trasferimento, IdPaziente, IdCartella, Icona,
	 Anteprima, CodUA, CodTipoAppuntamento, IdPazienteFuso, CodAgendaApp, DescrPazienteOrig)
	 
	SELECT 
	NULL AS IDCoda,		
	NULL AS NumeroCoda,	
	NULL AS Priorita,
	ISNULL(P.Cognome,'') + ' ' + ISNULL(P.Nome,'') + ' (' + ISNULL(P.Sesso,'') + ') ' +
								CASE
									WHEN P.DataNascita IS NOT NULL THEN ' (' + CONVERT(VARCHAR(10),P.DataNascita,105) + ') '
									ELSE ''
								END 
	AS DescrPaziente,
	'APP' AS CodEntita,
	APP.ID AS IDEntita,
	APP.DataInizio,
	APP.DataFine,					
	TA.Descrizione AS Tipo,
	APP.ElencoRisorse AS Descrizione,
	APP.CodStatoAppuntamento AS CodStato,
	STA.Descrizione AS Stato,
	APP.IDEpisodio AS IDEpisodio,
	NULL AS Trasferimento,
	P.ID as IdPaziente,
	NULL AS IDCartella,
	CONVERT(varbinary(max),null)  As Icona,			
	MS.AnteprimaTXT AS Anteprima,
	APP.CodUA,
	APP.CodTipoAppuntamento, 
	P.ID as IdPazienteFuso,
	APAGE.CodAgenda as CodAgendaApp,
	PAZ.DescrPaziente as DescrPazienteOrig
	FROM 
	T_Pazienti AS P 
	INNER JOIN T_MovAppuntamenti APP ON APP.IDPaziente = P.ID
	INNER JOIN T_MovAppuntamentiAgende APAGE ON APAGE.IDAppuntamento = APP.ID	
	INNER JOIN #tmpPazientiInCoda PAZ ON PAZ.IdPaziente = P.ID
											
	LEFT JOIN T_TipoAppuntamento TA	ON TA.Codice = APP.CodTipoAppuntamento								
	LEFT JOIN T_StatoAppuntamento STA ON STA.Codice = APP.CodStatoAppuntamento
	LEFT JOIN T_MovSchede MS ON MS.CodEntita='APP' AND
									MS.IDEntita=APP.ID AND
									MS.Storicizzata=0
						
	WHERE
	APP.DataInizio >= @dDataData AND
	APP.DataInizio < DATEADD(DAY, 1, @dDataData) AND
	APAGE.CodStatoAppuntamentoAgenda <> 'CA' AND
	NOT EXISTS (SELECT APPAGECART.IdEntita FROM #tmpAppTask APPAGECART WHERE APPAGECART.IdEntita = app.ID AND CodEntita = 'APP') AND
	APP.CodStatoAppuntamento NOT IN ('CA','AN','TR')


		
	UPDATE #tmpAltriAppuntamenti
	SET
		IdCoda = CDA.ID,
		NumeroCoda = CDA.NumeroCoda, 
		Priorita = CDA.Priorita 
	FROM
	#tmpAltriAppuntamenti
	INNER JOIN 	T_MovCodeEntita REL ON REL.IDEntita = #tmpAltriAppuntamenti.IdEntita
	INNER JOIN T_MovCode CDA ON CDA.ID = REL.IDCoda 
	WHERE
	REL.CodEntita='APP' AND
	CDA.CodStatoCoda <> 'CA'		

	



	UPDATE #tmpAltriAppuntamenti
	SET 
		NumeroNosologico = E.NumeroNosologico,
		NumeroListaAttesa = E.NumeroListaAttesa
	FROM #tmpAltriAppuntamenti AC
	INNER JOIN T_MovEpisodi E ON E.ID = AC.IdEpisodio
	
	


	UPDATE #tmpAltriAppuntamenti
	SET IdIcona = (SELECT ICO.IDNum FROM T_Icone ICO 
					WHERE 
						ICO.CodTipo = #tmpAltriAppuntamenti.CodTipoAppuntamento AND 
						ICO.CodStato = #tmpAltriAppuntamenti.CodStato AND
						ICO.CodEntita = 'APP'
						)


	UPDATE #tmpAltriAppuntamenti
	SET IdPazienteFuso = PA.IDPaziente
	FROM
	#tmpAltriAppuntamenti 
	INNER JOIN T_PazientiAlias PA ON PA.IDPazienteVecchio = #tmpAltriAppuntamenti.IdPaziente	




	CREATE TABLE #tmpElencoTrasAltriApp
	(IdEpisodio uniqueidentifier, 
	 IdCartella uniqueidentifier, 
	 IdTrasferimento uniqueidentifier,
	 NumeroCartella varchar(50) COLLATE Latin1_General_CI_AS,
	 DataIngresso datetime)


	INSERT INTO #tmpElencoTrasAltriApp
	(IdEpisodio, IdCartella, IdTrasferimento, NumeroCartella, DataIngresso)	
	SELECT
	AC.IdEpisodio, 
	T.IDCartella,
	T.ID,
	CR.NumeroCartella,
	T.DataIngresso
	FROM #tmpAltriAppuntamenti AC
	INNER JOIN T_MovEpisodi E ON E.ID = AC.IdEpisodio						
	INNER JOIN T_MovTrasferimenti AS T ON (T.IDEpisodio = E.ID) AND	T.CodStatoTrasferimento IN ('AT','PR','DM')				
	INNER JOIN T_MovCartelle AS CR ON CR.ID = T.IDCartella
	INNER JOIN #tmpUARuolo R ON R.CodUA = T.CodUA
	GROUP BY AC.IdEpisodio, T.IDCartella, T.ID, CR.NumeroCartella, T.DataIngresso

	

			
	CREATE TABLE #tmpUltimoTrasfAltriApp
	(IdEpisodio uniqueidentifier, 
	 IdCartella uniqueidentifier, 
	 IdTrasferimento uniqueidentifier,
	 NumeroCartella varchar(50) COLLATE Latin1_General_CI_AS,
	 DataIngresso datetime)
	 
	 
	INSERT INTO #tmpUltimoTrasfAltriApp
	(IdEpisodio, IdCartella, IdTrasferimento, NumeroCartella, DataIngresso)
	SELECT 
		ET.IdEpisodio,
		ET.IdCartella,
		ET.IdTrasferimento,
		ET.NumeroCartella,
		ET.DataIngresso
	FROM #tmpElencoTrasAltriApp ET
	INNER JOIN
		(SELECT ET1.IDEpisodio, ET1.IdCartella, MAX(ET1.DataIngresso) as UltDataIngresso
		 FROM #tmpElencoTrasAltriApp ET1
		 GROUP BY ET1.IdEpisodio, ET1.IdCartella) X 
		 ON X.IdEpisodio = ET.IdEpisodio AND X.IdCartella = ET.IdCartella AND X.UltDataIngresso = ET.DataIngresso

	

	
	UPDATE #tmpAltriAppuntamenti
	SET
	Trasferimento = UT.IdTrasferimento,
	IdCartella = UT.IdCartella,
	NumeroCartella = UT.NumeroCartella,
	NrNoso_NrCartella = ISNULL(AC.NumeroNosologico, '')  +
					' - ' + ISNULL(UT.NumeroCartella, '') ,
	Anteprima = ISNULL(AC.NumeroNosologico, '')  +
					' - ' + ISNULL(UT.NumeroCartella, '') + CHAR(13) + CHAR(10) +  AC.Anteprima
	FROM #tmpAltriAppuntamenti AC
	INNER JOIN #tmpUltimoTrasfAltriApp UT ON UT.IdEpisodio = AC.IdEpisodio



	
	
	

	CREATE TABLE #tmpAltriTask
	(
		IdCoda uniqueidentifier,
		NumeroCoda int,
		Priorita int,
		DescrPaziente varchar(max) COLLATE Latin1_General_CI_AS,
		CodEntita varchar(20) COLLATE Latin1_General_CI_AS,
		IdEntita uniqueidentifier,
		DataInizio datetime,
		DataFine datetime,
		Tipo varchar(255) COLLATE Latin1_General_CI_AS,
		Descrizione varchar(2000) COLLATE Latin1_General_CI_AS,
		CodStato varchar(20) COLLATE Latin1_General_CI_AS,
		Stato varchar(255) COLLATE Latin1_General_CI_AS,
		IdEpisodio uniqueidentifier,
		Trasferimento uniqueidentifier,
		IdPaziente uniqueidentifier,
		IdCartella uniqueidentifier,
		Icona varbinary(max),
		IdIcona numeric(18,0),
		Anteprima varchar(max) COLLATE Latin1_General_CI_AS,
		CodUA varchar(20) COLLATE Latin1_General_CI_AS,
		CodTipoTaskInfermieristico varchar(20) COLLATE Latin1_General_CI_AS,
		CodStatoTaskInfermieristico varchar(20) COLLATE Latin1_General_CI_AS,
		IdPazienteFuso uniqueidentifier, 		Cognome varchar(255) COLLATE Latin1_General_CI_AS,
		Nome varchar(255) COLLATE Latin1_General_CI_AS,
		NumeroNosologico varchar(20) COLLATE Latin1_General_CI_AS,
		NumeroListaAttesa VARCHAR(20) COLLATE Latin1_General_CI_AS,
		NumeroCartella varchar(50) COLLATE Latin1_General_CI_AS
		)
	
	
		INSERT INTO #tmpAltriTask
		(
		IdCoda,
		NumeroCoda,
		Priorita,
		DescrPaziente,
		CodEntita,
		IdEntita,
		DataInizio,
		DataFine,
		Tipo,
		Descrizione,
		CodStato,
		Stato,
		IdEpisodio,
		Trasferimento,
		IdPaziente,
		IdCartella,
		Icona,
		IdIcona,
		Anteprima,
		CodUA,
		CodTipoTaskInfermieristico,
		CodStatoTaskInfermieristico,
		IdPazienteFuso,
		Cognome,
		Nome,
		NumeroNosologico,
		NumeroListaAttesa,
		NumeroCartella)
						 
		SELECT 			
			CDA.ID AS IDCoda,		
			CDA.NumeroCoda AS NumeroCoda,
			CDA.Priorita,
			CASE 
				WHEN ISNULL(CDA.Priorita, 0) <> 0 THEN ' '
				ELSE ''
			END +
			' ' +
			CASE
				WHEN CDA.NumeroCoda IS NULL THEN ''
				ELSE RIGHT('0000' + CONVERT(VARCHAR(4),CDA.NumeroCoda),4) + ' '
			END
			+ ISNULL(P.Cognome,'') + ' ' + ISNULL(P.Nome,'') + ' (' + ISNULL(P.Sesso,'') + ') ' +
										CASE
											WHEN P.DataNascita IS NOT NULL THEN ' (' + CONVERT(VARCHAR(10),P.DataNascita,105) + ') '
											ELSE ''
										END 
			AS DescrPaziente,
			'WKI' AS CodEntita,
			WKI.ID AS IDEntita,
			WKI.DataProgrammata,
			NULL AS DataFine,					
			TT.Descrizione AS Tipo,
			NULL AS Descrizione,
			STT.Codice AS CodStato,
			STT.Descrizione AS Stato,		
			M.ID AS IDEpisodio,
			T.ID AS Trasferimento,
			P.IDPaziente,
			T.IDCartella,
			CONVERT(varbinary(max),null)  As Icona,
			null AS IDIcona,
			ISNULL(M.NumeroNosologico,'')  +
			' - ' + ISNULL(CR.NumeroCartella,'')
			AS Anteprima,
			T.CodUA,
			WKI.CodTipoTaskInfermieristico,
			WKI.CodStatoTaskInfermieristico,
			P.IDPaziente,
			P.Cognome,
			P.Nome,
			M.NumeroNosologico,
			M.NumeroListaAttesa,
			CR.NumeroCartella	
		FROM
			T_MovEpisodi M
			INNER JOIN T_MovPazienti AS P ON P.IDEpisodio = M.ID
			INNER JOIN #tmpPazientiInCoda PAZ ON PAZ.IdPaziente = P.IDPaziente
			INNER JOIN T_MovTrasferimenti AS T ON T.IDEpisodio = M.ID
			INNER JOIN T_MovCartelle AS CR ON CR.ID  = T.IDCartella
			INNER JOIN T_MovTaskInfermieristici WKI	ON (WKI.IDEpisodio=M.ID AND WKI.IDTrasferimento = T.ID AND WKI.CodStatoTaskInfermieristico NOT IN ('CA'))
			LEFT JOIN #tmpTaskInfermieristici TMPWKI ON WKI.CodTipoTaskInfermieristico=TMPWKI.Codice					
			INNER JOIN #tmpUARuolo UA ON (T.CodUA=UA.CodUA)						
			LEFT JOIN T_TipoTaskInfermieristico TT ON WKI.CodTipoTaskInfermieristico=TT.Codice					
			LEFT JOIN T_StatoTaskInfermieristico STT ON STT.Codice=WKI.CodStatoTaskInfermieristico				
			LEFT JOIN T_MovCodeEntita REL ON (REL.CodEntita='WKI' AND REL.IDEntita=WKI.ID)											
			INNER JOIN T_MovCode CDA ON (REL.IDCoda=CDA.ID AND CDA.CodStatoCoda <> 'CA')
		WHERE
		NOT EXISTS (SELECT APPAGECART.IdEntita FROM #tmpAppTask APPAGECART WHERE APPAGECART.IdEntita = WKI.ID AND CodEntita = 'WKI') AND
		WKI.DataProgrammata >= @dDataData AND 
		WKI.DataProgrammata < DATEADD(DAY, 1, @dDataData)






	UPDATE #tmpAltriTask
	SET IdIcona = (SELECT 
					IDNum AS IDIcona
					FROM T_Icone ICO WITH(NOLOCK)
					WHERE 
					CodEntita='WKI' AND
					ICO.CodTipo = #tmpAltriTask.CodTipoTaskInfermieristico AND 	
					ICO.CodStato = #tmpAltriTask.CodStatoTaskInfermieristico
					)									
								


	UPDATE #tmpAltriTask
	SET IdPazienteFuso = PA.IDPaziente
	FROM
	#tmpAltriTask 
	INNER JOIN T_PazientiAlias PA ON PA.IDPazienteVecchio = #tmpAltriTask.IdPaziente		


	CREATE TABLE #tmpFinale
	(
		IDCoda uniqueidentifier,
		NumeroCoda int,
		Priorita int,
		DescrPaziente varchar(max) COLLATE Latin1_General_CI_AS,
		CodEntita varchar(20) COLLATE Latin1_General_CI_AS,
		IdEntita uniqueidentifier,
		DataInizio datetime,
		DataFine datetime,
		Tipo varchar(255) COLLATE Latin1_General_CI_AS,
		Descrizione varchar(2000) COLLATE Latin1_General_CI_AS,
		CodStato varchar(20) COLLATE Latin1_General_CI_AS,
		Stato varchar(255) COLLATE Latin1_General_CI_AS,
		IDEpisodio uniqueidentifier,
		Trasferimento uniqueidentifier,
		IDPaziente uniqueidentifier,
		IDCartella uniqueidentifier,
		Icona varbinary(max),
		IDIcona numeric(18,0),
		Anteprima varchar(max) COLLATE Latin1_General_CI_AS,
		CodUA varchar(20) COLLATE Latin1_General_CI_AS,
		Attribuibile bit DEFAULT 0
		)
	INSERT INTO #tmpFinale
	(IdCoda, NumeroCoda, Priorita, DescrPaziente,	CodEntita, IdEntita, DataInizio,
		DataFine, Tipo,	Descrizione, CodStato, Stato, IdEpisodio, Trasferimento, IdPaziente, IdCartella,
		Icona, IdIcona, Anteprima,CodUA, Attribuibile)		
	SELECT 
	IdCoda, NumeroCoda, Priorita, DescrPaziente,	CodEntita, IdEntita, DataInizio,
		DataFine, Tipo,	Descrizione, CodStato, Stato, IdEpisodio, Trasferimento, IdPaziente, IdCartella,
		Icona, IdIcona, Anteprima,CodUA, 1
	FROM #tmpAppTaskFiltrati
	
	INSERT INTO #tmpFinale
	(IdCoda, NumeroCoda, Priorita, DescrPaziente,	CodEntita, IdEntita, DataInizio,
		DataFine, Tipo,	Descrizione, CodStato, Stato, IdEpisodio, Trasferimento, IdPaziente, IdCartella,
		Icona, IdIcona, Anteprima,CodUA, Attribuibile)		
	SELECT 
	IdCoda, NumeroCoda, Priorita, DescrPaziente,	CodEntita, IdEntita, DataInizio,
		DataFine, Tipo,	Descrizione, CodStato, Stato, IdEpisodio, Trasferimento, IdPaziente, IdCartella,
		Icona, IdIcona, Anteprima,CodUA, 0
	FROM #tmpAltriAppuntamenti
	
	INSERT INTO #tmpFinale
	(IdCoda, NumeroCoda, Priorita, DescrPaziente,	CodEntita, IdEntita, DataInizio,
		DataFine, Tipo,	Descrizione, CodStato, Stato, IdEpisodio, Trasferimento, IdPaziente, IdCartella,
		Icona, IdIcona, Anteprima,CodUA, Attribuibile)		
	SELECT 
	IdCoda, NumeroCoda, Priorita, DescrPaziente,	CodEntita, IdEntita, DataInizio,
		DataFine, Tipo,	Descrizione, CodStato, Stato, IdEpisodio, Trasferimento, IdPaziente, IdCartella,
		Icona, IdIcona,Anteprima,CodUA, 0
	FROM #tmpAltriTask
	
			
	
	

		
	UPDATE #tmpFinale
	SET
		DescrPaziente = PAZ.DescrPaziente
	FROM #tmpFinale F
	INNER JOIN #tmpPazientiInCoda PAZ ON PAZ.IdPaziente = F.IDPaziente
		
	
	
	SELECT DISTINCT 
		TMP.*,
		 UA.Descrizione AS DescUA 
		FROM #tmpFinale TMP
			LEFT JOIN T_UnitaAtomiche UA
				ON TMP.CodUA=UA.Codice
	ORDER BY DataInizio ASC
			
		DROP TABLE #tmpUARuolo
	DROP TABLE #tmpAgende	
	DROP TABLE #tmpTaskInfermieristici
	DROP TABLE #tmpAppTask
	DROP TABLE #tmpAppTaskFiltrati
	DROP TABLE #tmpElencoTrasferimenti
	DROP TABLE #tmpPazientiInCoda
	DROP TABLE #tmpUltimoTrasferimento
	DROP TABLE #tmpAltriAppuntamenti
	DROP table #tmpAltriTask
	DROP TABLE #tmpElencoTrasAltriApp
	DROP TABLE #tmpFinale
	DROP TABLE #tmpUltimoTrasfAltriApp
	
END