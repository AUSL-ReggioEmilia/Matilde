CREATE PROCEDURE [dbo].[MSP_SelStampaCartella](@xParametri XML)
AS
BEGIN
	
		
		DECLARE @uIDCartella AS UNIQUEIDENTIFIER

		DECLARE @sGUID AS VARCHAR(50)
	DECLARE @sTempGUID AS VARCHAR(50)
	DECLARE @sTmp AS VARCHAR(MAX)
    DECLARE @xTimeStamp AS XML	    
    DECLARE @xPar AS XML	
        
    DECLARE @sNumeroCartella VARCHAR(50)	
    DECLARE @uIDTrasferimento AS UNIQUEIDENTIFIER
    
    DECLARE @uIDPaziente AS UNIQUEIDENTIFIER
    
	DECLARE @uIDEpisodio AS UNIQUEIDENTIFIER
    
    DECLARE @sIntestazioneStampa AS VARCHAR(MAX)
	DECLARE @sIntestazioneCartellaReparto AS VARCHAR(MAX)
	DECLARE @sFirmaCartella AS VARCHAR(MAX)

	DECLARE @sCodUA AS VARCHAR(20)
	DECLARE @sCodStatoCartella AS VARCHAR(20)
	DECLARE @sNumeroNosologico AS VARCHAR(20)
	DECLARE @sNumeroNosologicoAltroEpisodio AS VARCHAR(20)
	DECLARE @sCodTipoEpisodio AS VARCHAR(20)
	DECLARE @dDataRicovero AS DATETIME
	DECLARE @dDataDimissione AS DATETIME
	DECLARE @dDataListaAttesa AS DATETIME
	DECLARE @sDescrizioneStatoCartella AS VARCHAR(255)
	DECLARE @sDescrizioneAltroEpisodio AS VARCHAR(255)
	DECLARE @sUnitaAtomica AS VARCHAR(255)
	DECLARE @sUnitaOperativa AS VARCHAR(255)
	DECLARE @sSettore AS VARCHAR(255)
	DECLARE @sRegime AS VARCHAR(255)
	DECLARE @sCodRegime AS VARCHAR(20)
	DECLARE @sCodTipoSchedaPaziente AS VARCHAR(20)
	DECLARE @sCodTipoSchedaEpisodio AS VARCHAR(20)
	DECLARE @dDataRiferimentoIntestazione AS DATETIME
	
	DECLARE @xParametriReport AS XML
	DECLARE @bScrittaPSC AS BIT
	DECLARE @nTmpScrittaPSC AS INTEGER

				
		
	SET @uIDCartella=(SELECT TOP 1 ValoreParametro.IDCartella.value('.','UNIQUEIDENTIFIER')
					  FROM @xParametri.nodes('/Parametri/IDCartella') as ValoreParametro(IDCartella))	
	SET @uIDCartella=ISNULL(@uIDCartella,'')	
		
				
    SET @xTimeStamp=(SELECT TOP 1 ValoreParametro.TS.query('.')
					  FROM @xParametri.nodes('/Parametri/TimeStamp') as ValoreParametro(TS))
		
					
			
				CREATE TABLE #tmpCartelleEpi
	(
		IDCartella UNIQUEIDENTIFIER
	)
	
	
		SET @xPar=CONVERT(XML,'<Parametri>
							<IDCartella>' + CONVERT(VARCHAR(50),@uIDCartella) + '</IDCartella>
							</Parametri>')
									
	INSERT INTO #tmpCartelleEpi					
	EXEC MSP_SelCartelleCollegate @xPar	
		
	SET @sNumeroNosologicoAltroEpisodio=(SELECT TOP 1 ISNULL(NumeroNosologico,NumeroListaAttesa) 
										 FROM T_MovEpisodi
										 WHERE ID IN 
																										(SELECT TOP 1 E.ID
													  FROM T_MovRelazioniEntita R
														INNER JOIN T_MovTrasferimenti T
															ON (IDCartella=IDEntitaCollegata)
														INNER JOIN T_MovEpisodi E
															ON (E.ID=T.IDEpisodio)
													  WHERE 
														R.CodEntita='CAR' AND 
														R.CodEntitaCollegata='CAR' AND 
														R.IDEntita=@uIDCartella
													  )
										)			  
	
	SET @sNumeroNosologicoAltroEpisodio=ISNULL(@sNumeroNosologicoAltroEpisodio,'')
	
	IF @sNumeroNosologicoAltroEpisodio<> ''
		SET @sDescrizioneAltroEpisodio=' CON PRESECUZIONE SU ALTRO EPISODIO ' + @sNumeroNosologicoAltroEpisodio
	ELSE
		SET @sDescrizioneAltroEpisodio=''
		
					
			
		 SELECT TOP 1 
		@sNumeroCartella= NumeroCartella,
		@dDataRiferimentoIntestazione = CASE 
											WHEN CodStatoCartella='CH' THEN DataChiusura
											ELSE GETDATE()
										END
	 FROM T_MovCartelle WHERE ID=@uIDcartella
	
		SELECT TOP 1 		
		@sCodUA=CodUA,
		@sSettore=T.DescrSettore,
		@sUnitaOperativa=T.DescrUO,
		@sNumeroNosologico=E.NumeroNosologico,
		@sCodTipoEpisodio=E.CodTipoEpisodio,
		@uIDEpisodio=E.ID,
		@uIDTrasferimento=T.ID,
		@dDataRicovero=E.DataRicovero,
		@dDataDimissione=E.DataDimissione,
		@dDataListaAttesa=E.DataListaAttesa
	FROM T_MovTrasferimenti T WITH (NOLOCK)	
		LEFT JOIN T_MovEpisodi  E WITH (NOLOCK)	
		 ON T.IDEpisodio=E.ID
	WHERE IDCartella=@uIDCartella
	
		SET @sIntestazioneStampa=(SELECT TOP 1 RTFStampaEstesa FROM T_Aziende WITH (NOLOCK)				
							  WHERE Codice IN (SELECT CodAzienda FROM T_UnitaAtomiche WITH (NOLOCK)	 WHERE Codice=@sCodUA))
	
		SELECT TOP 1 
				@sUnitaAtomica=Descrizione
	FROM T_UnitaAtomiche WITH (NOLOCK)	
	WHERE Codice=@sCodUA
	
		SELECT TOP 1 
			@sRegime= ISNULL(T.Descrizione,''),				
			@sCodRegime = E.CodTipoEpisodio
	FROM T_MovEpisodi E WITH (NOLOCK)	
		LEFT JOIN T_TipoEpisodio T WITH (NOLOCK)	
			ON E.CodTipoEpisodio=T.Codice
	WHERE E.ID=	@uIDEpisodio			

		SET @sDescrizioneStatoCartella=(
		SELECT TOP 1 
			CASE 
				WHEN CodStatoCartella='AP' THEN 'CARTELLA APERTA'
				WHEN CodStatoCartella='CH' THEN 'CARTELLA CHIUSA IL ' + CONVERT(VARCHAR(10),DataChiusura,105) +	@sDescrizioneAltroEpisodio				
				ELSE ''
			END	
		FROM
			T_MovCartelle 
		WHERE 
			ID=@uIDCartella)
		
		
			SET @sIntestazioneCartellaReparto =(SELECT dbo.MF_CercaIntestazione('CARTSTD',@dDataRiferimentoIntestazione,@sCodUA))
	SET @sFirmaCartella=(SELECT dbo.MF_CercaIntestazione('CARFIRMA',@dDataRiferimentoIntestazione,@sCodUA))
	
		SELECT 
		'0 - Dati Generali' AS Sez00,		
		@sIntestazioneStampa AS IntestazioneStampa,
		@sIntestazioneCartellaReparto AS IntestazioneCartellaReparto,
		@sRegime AS Regime,
		@sUnitaAtomica AS UnitaAtomica,
		@sUnitaOperativa AS UnitaOperativa,
		@sSettore AS Settore,
		@sNumeroCartella AS NumeroCartella,
		@sDescrizioneStatoCartella AS DescrizioneStatoCartella,
		@sFirmaCartella AS FirmaCartella,
		@sNumeroNosologico AS NumeroNosologico,
		@sCodTipoEpisodio AS CodTipoEpisodio    

					
	CREATE TABLE #tmpUA
	(
		CodUA VARCHAR(20) COLLATE Latin1_General_CI_AS,
		Descrizione VARCHAR(255) COLLATE Latin1_General_CI_AS
	)

	DECLARE @xTmp AS XML
	
	SET @xTmp =CONVERT(XML,'<Parametri><CodUA>'+@sCodUA+'</CodUA></Parametri>')
	
	INSERT #tmpUA EXEC MSP_SelUAPadri @xTmp	
		
	CREATE INDEX IX_CodUA ON #tmpUA (CodUA)   
	
				CREATE TABLE #tmpEscludiPrescrizioneFarmaciPSC
	(
		CodUA VARCHAR(20) COLLATE Latin1_General_CI_AS		
	)

	SET @xParametriReport=(SELECT TOP 1 Parametri FROM T_Report WHERE Codice='CTLPZN1')


	
		INSERT #tmpEscludiPrescrizioneFarmaciPSC(CodUA)
	SELECT 
		T.CodUA.query('.').value('(CodUA)[1]', 'varchar(50)')  AS CodUA	
	FROM   @xParametriReport.nodes('/Parametri/EscludiPrescrizioneFarmaciPSC/CodUA') T(CodUA)  
	
		SET @nTmpScrittaPSC=(SELECT COUNT(*) FROM #tmpEscludiPrescrizioneFarmaciPSC
						   WHERE CodUA=@sCodUA)

		IF @nTmpScrittaPSC > 0
		SET @bScrittaPSC=0
	ELSE
		SET @bScrittaPSC=1

		
			
		SET @uIDPaziente=(SELECT TOP 1 
							IDPaziente 
					  FROM T_MovPazienti MP	WITH (NOLOCK)													
					  WHERE		
							MP.IDEpisodio=@uIDEpisodio
					  )	
	
		CREATE TABLE #tmpFiltroPaziente(IDPaziente UNIQUEIDENTIFIER)
	
	INSERT INTO	#tmpFiltroPaziente					  
			SELECT IDPazienteVecchio
					 FROM T_PazientiAlias WITH (NOLOCK)	
					 WHERE 
						IDPaziente IN 
							(SELECT IDPaziente
							 FROM T_PazientiAlias WITH (NOLOCK)	
							 WHERE IDPazienteVecchio=@uIDPaziente
							)
			UNION 
			SELECT @uIDPaziente		

	SELECT TOP 1
		'1 - Paziente' AS Sez01,			
		Cognome,
		Nome,
		Sesso,
		CASE 
			WHEN DataNascita IS NOT NULL THEN CONVERT(Varchar(10),DataNascita,105)
			ELSE NULL
		END DataNascita,
		CodiceFiscale,
		
		CASE 
			WHEN DataNascita IS NOT NULL AND (@dDataRicovero IS NOT NULL OR @dDataListaAttesa IS NOT NULL)
					THEN dbo.MF_CalcolaEta(DataNascita,ISNULL(@dDataRicovero,@dDataListaAttesa))
			ELSE 0		
		END AS EtaAllAccesso,
				CASE
		  WHEN	
				LTRIM(ISNULL(LocalitaNascita,'')) <> LTRIM(ISNULL(ComuneNascita,'')) THEN
					LTRIM(ISNULL(LocalitaNascita,'') + ' ' + ISNULL(ComuneNascita,''))
		  ELSE
				ISNULL(ComuneNascita,ISNULL(LocalitaNascita,''))
		END		
		AS  LuogoNascita,
		
				LTRIM(					ISNULL(IndirizzoResidenza,'')	+ ', ' +		
								CASE
				  WHEN	
						LTRIM(ISNULL(LocalitaResidenza,'')) <> LTRIM(ISNULL(ComuneResidenza,'')) THEN
							LTRIM(ISNULL(LocalitaResidenza,'') + ' ' + ISNULL(ComuneResidenza,''))
				  ELSE
						ISNULL(ComuneResidenza,ISNULL(LocalitaResidenza,''))
				END	
								+ CASE 
					WHEN ISNULL(CAPResidenza,'')='' THEN ''
					ELSE ' (' + ISNULL(CAPResidenza,'') + ')'
				  END
				)				
		AS  LuogoResidenza,
		
				LTRIM(					ISNULL(IndirizzoDomicilio,'')	+ ', ' +		
								CASE
				  WHEN	
						LTRIM(ISNULL(LocalitaDomicilio,'')) <> LTRIM(ISNULL(ComuneDomicilio,'')) THEN
							LTRIM(ISNULL(LocalitaDomicilio,'') + ' ' + ISNULL(ComuneDomicilio,''))
				  ELSE
						ISNULL(ComuneResidenza,ISNULL(LocalitaDomicilio,''))
				END	
								+ CASE 
					WHEN ISNULL(CAPDomicilio,'')='' THEN ''
					ELSE ' (' + ISNULL(CAPDomicilio,'') + ')'
				  END
				)				
		AS  LuogoDomicilio,
		
				CASE 
			WHEN ISNULL(CognomeNomeMedicoBase,'') <> '' THEN
					'Dott ' + 
							REPLACE (CognomeNomeMedicoBase,'/',' ') + 
								' (C.F.:' + ISNULL(CodFiscMedicoBase,'') + ') ' 
			ELSE ''
		END AS MedicoCurante,
		MP.ElencoEsenzioni AS Esenzioni
	FROM T_MovPazienti MP WITH (NOLOCK)													
	WHERE	
				MP.IDEpisodio=@uIDEpisodio

	
				
	EXEC MSP_SelStampaCartellaADT @uIDEpisodio

					   
				
		SET @sCodTipoSchedaPaziente=(SELECT TOP 1 Valore FROM T_Config WHERE ID=31)
	
	SELECT 
		'3 - Schede Testata Paziente' AS Sez03,		
		S.Descrizione AS NomeScheda,
		dbo.MF_PulisciRTF(AnteprimaRTF) AS AnteprimaRTF,
				CONVERT(VARCHAR(10), DataCreazione, 105) AS DataCreazione,
		
				CONVERT(VARCHAR(10), ISNULL(DataUltimaModifica,DataCreazione), 105) 
			+ ' ' + CONVERT(varchar(5), ISNULL(DataUltimaModifica,DataCreazione), 14) AS DataUltimaModifica,	
		
				ISNULL(M.CodUtenteUltimaModifica,M.CodUtenteRilevazione) AS CodUtenteUltimaModifica,
				CASE	
					WHEN ISNULL(M.CodUtenteUltimaModifica,'')<>'' THEN UM.Descrizione
					ELSE UC.Descrizione
				END	
		AS DescrUtenteUltimaModifica,
		UV.Descrizione AS DescrUtenteValidazione,
		CASE 
			WHEN DataValidazione IS NOT NULL THEN	
					CONVERT(VARCHAR(10), DataValidazione, 105) + ' ' + CONVERT(varchar(5), DataValidazione, 14)
			ELSE ''
		END AS DataValidazione
		
	FROM 
		T_MovSchede M WITH (NOLOCK)	
			INNER JOIN T_Schede S WITH (NOLOCK)		 
				ON M.CodScheda=S.Codice
			INNER JOIN
								(SELECT DISTINCT 
					A.CodVoce AS Codice												
				 FROM					
					T_AssUAEntita A
						INNER JOIN #tmpUA T WITH (NOLOCK)	 ON
							A.CodUA=T.CodUA					
				 WHERE CodEntita='SCH'			
				 ) AS W				
				ON S.Codice=W.Codice	
						LEFT JOIN T_Login UC WITH (NOLOCK)	
						ON M.CodUtenteRilevazione = UC.Codice	
							
						LEFT JOIN T_Login UM WITH (NOLOCK)	
						ON M.CodUtenteUltimaModifica = UM.Codice
			
						LEFT JOIN T_Login UV WITH (NOLOCK)	
						ON M.CodUtenteValidazione = UV.Codice					
	WHERE 
				M.Storicizzata=0 AND 
		
				M.CodStatoScheda NOT IN ('AN','CA') AND
		
				M.CodEntita='PAZ' AND
		
				M.IDPaziente IN (SELECT IDPaziente FROM #tmpFiltroPaziente)

		AND
		
				ISNULL(S.IgnoraStampaCartella,0)=0 AND 
		
				ISNULL(S.CodTipoScheda,'')=@sCodTipoSchedaPaziente
		
	ORDER BY 
		S.Ordine,
		M.DataCreazione ASC	
	OPTION (USE HINT('QUERY_OPTIMIZER_COMPATIBILITY_LEVEL_100'))

			
		SET @sCodTipoSchedaEpisodio=(SELECT TOP 1 Valore FROM T_Config WHERE ID=32)
	
	SELECT 
		'4 - Schede Testata Episodio' AS Sez04,			
		S.Descrizione AS NomeScheda,
		dbo.MF_PulisciRTF(AnteprimaRTF) AS AnteprimaRTF,
				CONVERT(VARCHAR(10), DataCreazione, 105) AS DataCreazione,
		
				CONVERT(VARCHAR(10), ISNULL(DataUltimaModifica,DataCreazione), 105) 
			+ ' ' + CONVERT(varchar(5), ISNULL(DataUltimaModifica,DataCreazione), 14) AS DataUltimaModifica,	
		
				ISNULL(M.CodUtenteUltimaModifica,M.CodUtenteRilevazione) AS CodUtenteUltimaModifica,
				CASE	
					WHEN ISNULL(M.CodUtenteUltimaModifica,'')<>'' THEN UM.Descrizione
					ELSE UC.Descrizione
				END	
		AS DescrUtenteUltimaModifica,
		UV.Descrizione AS DescrUtenteValidazione,		
		CASE 
			WHEN DataValidazione IS NOT NULL THEN	
					CONVERT(VARCHAR(10), DataValidazione, 105) + ' ' + CONVERT(varchar(5), DataValidazione, 14)
			ELSE ''
		END AS DataValidazione
	FROM 
		T_MovSchede M
			INNER JOIN T_Schede S WITH (NOLOCK)	 
				ON M.CodScheda=S.Codice	
			INNER JOIN
								(SELECT DISTINCT 
					A.CodVoce AS Codice												
				 FROM					
					T_AssUAEntita A WITH (NOLOCK)	
						INNER JOIN #tmpUA T WITH (NOLOCK)	 ON
							A.CodUA=T.CodUA					
				 WHERE CodEntita='SCH'			
				 ) AS W				
				ON S.Codice=W.Codice	
						LEFT JOIN T_Login UC WITH (NOLOCK)	
						ON M.CodUtenteRilevazione = UC.Codice	
							
						LEFT JOIN T_Login UM
						ON M.CodUtenteUltimaModifica = UM.Codice
			
						LEFT JOIN T_Login UV WITH (NOLOCK)	
						ON M.CodUtenteValidazione = UV.Codice				
	WHERE 
				M.Storicizzata=0 AND 
		
				CodStatoScheda NOT IN ('AN','CA') AND
		
				M.CodEntita='EPI' AND
		
				M.IDEpisodio=@uIDEpisodio AND 
		
				ISNULL(S.IgnoraStampaCartella,0)=0 AND 
				
				ISNULL(S.CodTipoScheda,'')=@sCodTipoSchedaEpisodio
		
	ORDER BY 
		S.Ordine ASC,
		M.DataCreazione ASC	
	OPTION (USE HINT('QUERY_OPTIMIZER_COMPATIBILITY_LEVEL_100'))

			
	SELECT 
		  '5 - Alert Anamnestici' AS Sez05,		 	 
		  Convert(varchar(20),DataEvento,105) +' ' +  Convert(varchar(5),DataEvento,108) As DataEvento,
		 dbo.MF_PulisciRTF(MS.AnteprimaRTF) AS AnteprimaRTF,
		  L.Descrizione AS UtenteInserimento
		  
	FROM 
		T_MovAlertAllergieAnamnesi	M					
			LEFT JOIN T_TipoAlertAllergiaAnamnesi T
					ON (M.CodTipoAlertAllergiaAnamnesi=T.Codice)
			LEFT JOIN 
					(SELECT ID As IDScheda, CodScheda,Versione,IDEntita,AnteprimaRTF
					 FROM
						T_MovSchede  WITH (NOLOCK)	
					 WHERE CodEntita='ALA' AND							
						Storicizzata=0 AND
						CodStatoScheda <> 'CA'
					) AS MS
				ON (MS.IDEntita=M.ID AND
					MS.CodScheda=T.CodScheda)	
			LEFT JOIN T_Login L WITH (NOLOCK)	
				ON (M.CodUtenteRilevazione=L.Codice)			
	WHERE 	
		
				M.IDPaziente IN (SELECT IDPaziente FROM #tmpFiltroPaziente) AND			
		M.CodStatoAlertAllergiaAnamnesi NOT IN ('AN','CA')		
	ORDER BY DataEvento ASC
	OPTION (USE HINT('QUERY_OPTIMIZER_COMPATIBILITY_LEVEL_100'))
				
	SET @sTempGUID=NEWID()
	
		SELECT 
		'6 - Schede cliniche' AS Sez06,			
				ISNULL(S.Descrizione,'') AS Descrizione, 
		
					CASE 
				WHEN ISNULL(M.Numero,'') <> '' AND ISNULL(S.NumerositaMassima,0) > 1
					 THEN convert(varchar(10),M.Numero) 
				ELSE ''	 
			END AS NumeroScheda,		
			
				CASE 
				WHEN ISNULL(M.Numero,'') <> '' AND ISNULL(S.NumerositaMassima,0) > 1
					 THEN CONVERT(VARCHAR(10),
									ISNULL(dbo.MF_SelNumerositaSchedaParametro('QtaSchedeTotali',M.ID,NULL,NULL,NULL,NULL),0)
								 )		
				ELSE ''	 
			END
		AS NumeroTotaleSchede,
		
		dbo.MF_PulisciRTF(AnteprimaRTF) AS AnteprimaRTF,
		
				CONVERT(VARCHAR(10), DataCreazione, 105) AS DataCreazione,
		
				CONVERT(VARCHAR(10), ISNULL(DataUltimaModifica,DataCreazione), 105) 
			+ ' ' + CONVERT(varchar(5), ISNULL(DataUltimaModifica,DataCreazione), 14) AS DataUltimaModifica,	
		
				ISNULL(M.CodUtenteUltimaModifica,M.CodUtenteRilevazione) AS CodUtenteUltimaModifica,
				CASE	
					WHEN ISNULL(M.CodUtenteUltimaModifica,'')<>'' THEN UM.Descrizione
					ELSE UC.Descrizione
				END	
		AS DescrUtenteUltimaModifica,
		UV.Descrizione AS DescrUtenteValidazione,	
		CASE 
			WHEN DataValidazione IS NOT NULL THEN	
					CONVERT(VARCHAR(10), DataValidazione, 105) + ' ' + CONVERT(varchar(5), DataValidazione, 14)
			ELSE ''
		END AS DataValidazione,
				
		M.ID AS IDScheda,
		M.IDSchedaPadre AS IDSchedaPadre
		   	 
	FROM 
		T_MovSchede M
			INNER JOIN T_Schede S WITH (NOLOCK)	
				ON M.CodScheda=S.Codice				
			INNER JOIN
								(SELECT DISTINCT 
					A.CodVoce AS Codice												
				 FROM					
					T_AssUAEntita A WITH (NOLOCK)	
						INNER JOIN #tmpUA T ON
							A.CodUA=T.CodUA					
				 WHERE CodEntita='SCH'			
				 ) AS W				
				ON S.Codice=W.Codice	
				
					
						LEFT JOIN T_Login UC WITH (NOLOCK)	
						ON M.CodUtenteRilevazione = UC.Codice	
							
						LEFT JOIN T_Login UM WITH (NOLOCK)	
						ON M.CodUtenteUltimaModifica = UM.Codice
						
						LEFT JOIN T_Login UV WITH (NOLOCK)	
						ON M.CodUtenteValidazione = UV.Codice	
											
	WHERE 
				M.Storicizzata=0 AND 
		
				CodStatoScheda NOT IN ('AN','CA') AND
		
						M.CodEntita IN ('EPI') AND
		
				M.IDPaziente IN (SELECT IDPaziente FROM #tmpFiltroPaziente) AND	

				ISNULL(M.IDEpisodio,@sTempGUID)=
						CASE 
							WHEN M.CodEntita ='EPI' THEN @uIDEpisodio 
							ELSE ISNULL(M.IDEpisodio,@sTempGUID)
						END	
		AND
		
				ISNULL(S.IgnoraStampaCartella,0)=0 AND 
		
				ISNULL(S.CodTipoScheda,'') NOT IN (@sCodTipoSchedaEpisodio,@sCodTipoSchedaPaziente)
		
	ORDER BY 
		S.Ordine ASC,
		M.DataCreazione ASC
			OPTION (USE HINT('QUERY_OPTIMIZER_COMPATIBILITY_LEVEL_100'))

					
	SELECT 
		  '7 - Diario Clinico Medico' AS Sez07,		  		  
		  Convert(varchar(20),M.DataEvento,105) +' ' +  Convert(varchar(5),M.DataEvento,108) As DataEvento,
		  Convert(varchar(20),M.DataValidazione,105) +' ' +  Convert(varchar(5),M.DataValidazione,108) As DataValidazione,
		  dbo.MF_PulisciRTF(MS.AnteprimaRTF) AS AnteprimaRTF,
		  L.Descrizione AS UtenteValidatore		 		  
	FROM 
		T_MovDiarioClinico M  WITH (NOLOCK)	
		LEFT JOIN 
				(SELECT ID As IDScheda, CodScheda,Versione,IDEntita,AnteprimaRTF
				 FROM
					T_MovSchede WITH (NOLOCK)	
				 WHERE CodEntita='DCL' AND							
					Storicizzata=0
				) AS MS
			ON MS.IDEntita=M.ID	
		LEFT JOIN T_Login L WITH (NOLOCK)	
			ON (M.CodUtenteRilevazione=L.Codice)	
		LEFT JOIN T_TipoVoceDiario AS TV WITH (NOLOCK)	
			ON (M.CodTipoVoceDiario =TV.Codice)	

	
	WHERE 
				TV.CodTipoDiario='M' AND
		
				M.CodStatoDiario IN ('VA','AN') AND
								
				M.IDTrasferimento IN (
			SELECT ID FROM T_MovTrasferimenti WITH (NOLOCK)			
			WHERE IDCartella=@uIDCartella)		
			
	ORDER BY  
		M.DataEvento,M.ID					OPTION (USE HINT('QUERY_OPTIMIZER_COMPATIBILITY_LEVEL_100')) 	
				
				
	SELECT 
		  '8 - Diario Clinico Infermieristico' AS Sez08,		  
		  Convert(varchar(20),M.DataEvento,105) +' ' +  Convert(varchar(5),M.DataEvento,108) As DataEvento,
		  Convert(varchar(20),M.DataValidazione,105) +' ' +  Convert(varchar(5),M.DataValidazione,108) As DataValidazione,
		  dbo.MF_PulisciRTF(MS.AnteprimaRTF) AS AnteprimaRTF,
		  L.Descrizione AS UtenteValidatore  
	FROM 
		T_MovDiarioClinico M WITH (NOLOCK)	
		LEFT JOIN 
				(SELECT ID As IDScheda, CodScheda,Versione,IDEntita,AnteprimaRTF
				 FROM
					T_MovSchede WITH (NOLOCK)	
				 WHERE CodEntita='DCL' AND							
					Storicizzata=0
				) AS MS
			ON MS.IDEntita=M.ID	
		LEFT JOIN T_Login L WITH (NOLOCK)	
			ON (M.CodUtenteRilevazione=L.Codice)	
		LEFT JOIN T_TipoVoceDiario AS TV WITH (NOLOCK)	
			ON (M.CodTipoVoceDiario =TV.Codice)	
	WHERE 
				TV.CodTipoDiario='I' AND
		
				M.CodStatoDiario IN ('VA','AN') AND
								
				M.IDTrasferimento IN (
			SELECT ID FROM T_MovTrasferimenti		
			WHERE IDCartella=@uIDCartella)	
			
		AND
		M.CodEntitaRegistrazione NOT IN ('WKI','PVT')	
		
	ORDER BY M.DataEvento ASC		
	OPTION (USE HINT('QUERY_OPTIMIZER_COMPATIBILITY_LEVEL_100')) 	

				
	SELECT 
		  '9 - Parametri Vitali' AS Sez09,		 
		  Convert(varchar(20),M.DataEvento,105) +' ' +  Convert(varchar(5),M.DataEvento,108) As DataEvento,
		  Convert(varchar(20),M.DataInserimento,105) +' ' +  Convert(varchar(5),M.DataInserimento,108) As DataInserimento,
		  T.Descrizione AS TipoParametroVitale,
		  dbo.MF_PulisciRTF(MS.AnteprimaRTF) AS AnteprimaRTF,
		  L.Descrizione AS UtenteRilevatore
	FROM T_MovParametriVitali M WITH (NOLOCK)				
		LEFT JOIN 
				(SELECT ID As IDScheda, CodScheda,Versione,IDEntita,AnteprimaRTF
				 FROM
					T_MovSchede  WITH (NOLOCK)	
				 WHERE CodEntita='PVT' AND							
					Storicizzata=0  
				) AS MS
			ON MS.IDEntita=M.ID	
		LEFT JOIN T_Login L WITH (NOLOCK)	
			ON (M.CodUtenteRilevazione=L.Codice)		
		LEFT JOIN T_TipoParametroVitale T WITH (NOLOCK)	
			ON (M.CodTipoParametroVitale=T.Codice)	
	WHERE 
		M.CodStatoParametroVitale NOT IN ('AN','CA') AND
		
				M.IDTrasferimento IN (
			SELECT ID FROM T_MovTrasferimenti WITH (NOLOCK)	
			WHERE IDCartella=@uIDCartella)	
			
	ORDER BY
		M.DataEvento ASC
	OPTION (USE HINT('QUERY_OPTIMIZER_COMPATIBILITY_LEVEL_100')) 	

				SELECT 
		'10Pre  - Prescrizioni Farmaci' AS Sez10Pre,
		CASE 			
			WHEN @sCodRegime ='RO' AND @bScrittaPSC=1 THEN 'Per la Terapia farmacologica vedere la Stampa Terapia PSC' 			
			ELSE ''
		END AS Messaggio
	
				SELECT 
		'10 - Prescrizioni Farmaci' AS Sez10,		
		
								
				Convert(varchar(20),M.DataValidazione,105) +' ' +  Convert(varchar(5),M.DataValidazione,108)  AS DataValidazione,					 		
		
		CASE  
			WHEN M.CodStatoPrescrizioneTempi ='SS' THEN
					Convert(varchar(20),M.DataSospensione,105) +' ' +  Convert(varchar(5),M.DataSospensione,108)  
						+ CHAR(13) + CHAR(10) + 'Da: ' +  ISNULL(L2.Descrizione,'')
								
			ELSE ''					
		END AS DataSospensione,			
		T.Descrizione AS TipoPrescrizione,
		
				L.Descrizione AS UtenteValidatore,
		 
				dbo.MF_PulisciRTF(MS.AnteprimaRTF) AS AnteprimaRTF,
		
				dbo.MF_ElencoDateTaskPrescrizioneTempi(M.ID) AS DataSomministrazione,
		ISNULL(M.Posologia,'') AS Posologia,
		ISNULL(VS.Descrizione,'') AS ViaSomministrazione
		
		
	FROM
				T_MovPrescrizioniTempi AS M WITH (NOLOCK)
		
						LEFT JOIN T_MovPrescrizioni MP WITH (NOLOCK)
				ON M.IDPrescrizione=MP.ID
			
						LEFT JOIN T_TipoPrescrizione T WITH (NOLOCK)
				ON (MP.CodTipoPrescrizione=T.Codice)	
			
						LEFT JOIN
				(SELECT ID As IDScheda, CodScheda,Versione,IDEntita,AnteprimaRTF
							 FROM
								T_MovSchede  WITH (NOLOCK) 
							 WHERE CodEntita='PRF' AND							
								Storicizzata=0  AND
								CodStatoScheda <> 'CA'
							) AS MS
						ON MS.IDEntita=MP.ID
						
			LEFT JOIN T_Login L  WITH (NOLOCK)
				ON (M.CodUtenteValidazione = L.Codice)	
			LEFT JOIN T_Login L2  WITH (NOLOCK)
				ON (M.CodUtenteSospensione = L2.Codice)		
			LEFT JOIN T_ViaSomministrazione VS  WITH (NOLOCK)
				ON MP.CodViaSomministrazione=VS.Codice
			
			INNER JOIN	 		
					(SELECT 			
						DISTINCT IDGruppo
					 FROM 
						T_MovTaskInfermieristici WITH (NOLOCK)
					 WHERE 
												CodSistema='PRF' AND
						CodStatoTaskInfermieristico <> 'CA'	AND
					 						IDGruppo IS NOT NULL AND 
						IDGruppo <> ''									
					) AS Q
				ON M.IDString=Q.IDGruppo	
	WHERE 
				M.CodStatoPrescrizioneTempi IN ('VA','SS') AND
		
				MP.IDTrasferimento IN (
			SELECT ID FROM T_MovTrasferimenti WITH (NOLOCK)		
			WHERE IDCartella=@uIDCartella)	
		
				
	ORDER BY M.DataValidazione ASC
	OPTION (USE HINT('QUERY_OPTIMIZER_COMPATIBILITY_LEVEL_100')) 	
	
				SELECT 
		'11 - Task Infermierisrtici' AS Sez11,		
		 'P: ' + Convert(varchar(20),M.DataProgrammata,105) +' ' +  Convert(varchar(5),M.DataProgrammata,108) 
		 + CHAR(13) + CHAR(10) +
		  CASE							 
			WHEN CodStatoTaskInfermieristico IN ('ER') THEN 
				 'E: ' + Convert(varchar(20),M.DataErogazione,105) +' ' +  Convert(varchar(5),M.DataErogazione,108)  
			WHEN CodStatoTaskInfermieristico IN ('AN') THEN 
				 'A: ' + Convert(varchar(20),M.DataErogazione,105) +' ' +  Convert(varchar(5),M.DataErogazione,108)  
			WHEN CodStatoTaskInfermieristico IN ('TR') THEN 
				 'T: ' + ISNULL(Convert(varchar(20),M.DataErogazione,105) +' ' +  Convert(varchar(5),M.DataErogazione,108),'')
			ELSE ''
		  END	
		 As DescrDate,		
		 
		 		 		 		 'Programmato da' AS TestoProgrammazione,
		 		CASE						
				WHEN L.Codice IS NULL THEN ISNULL(L.Descrizione,'')
				ELSE ISNULL(L.Descrizione,'')
		END AS UtenteProgrammatore,	
		
				ISNULL(Convert(varchar(20),M.DataProgrammata,105) +' ' +  Convert(varchar(5),M.DataProgrammata,108),'') 
			AS DataProgrammata,

		 T.Descrizione AS TipoTaskInfermieristico,
		
		 CASE 
			WHEN CodStatoTaskInfermieristico IN ('ER','AN','TR') THEN LEFT(S.Descrizione,1) + ':'+  ISNULL(Convert(varchar(20),M.DataErogazione,105),'') +' ' +  ISNULL(Convert(varchar(5),M.DataErogazione,108),'')
			ELSE  S.Descrizione 
		 END		 
		 AS Stato,
		 
		 dbo.MF_PulisciRTF(MS.AnteprimaRTF) AS AnteprimaRTF,
		 CASE	
			WHEN LEN(ISNULL(MS.AnteprimaTXT,'')) >= 255 THEN 1
			ELSE 0
		 END AS FlagEsteso,
		 
		 		 		 		 CASE 
			WHEN CodStatoTaskInfermieristico ='ER' THEN 'Erogato da '
			WHEN CodStatoTaskInfermieristico  IN ('AN') THEN 'Annullato da '
			WHEN CodStatoTaskInfermieristico  IN ('TR') THEN 'Trascritto da '
			ELSE ''
		 END	
		 AS TestoErogazioneAnnullamento,
		 
		 		 Convert(varchar(20),M.DataErogazione,105) +' ' +  ISNULL(Convert(varchar(5),M.DataErogazione,108) ,'')
			AS DataErogazioneAnnullamento,
			
		 		 CASE
			WHEN CodStatoTaskInfermieristico IN ('ER','AN','TR') THEN 
					CASE	
												WHEN L2.Codice IS NULL THEN ISNULL(L.Descrizione,'')
						ELSE ISNULL(L2.Descrizione,'')
					END
																					END	AS UtenteEsecutoreAnnullatore,
				 		 		 		 'Programmato da '  +  
				CASE						
					WHEN L.Codice IS NULL THEN L.Descrizione 
					ELSE ISNULL(L.Descrizione,'')
				END + CHAR(13) + CHAR(10) + ' il ' + Convert(varchar(20),M.DataProgrammata,105) +' ' +  Convert(varchar(5),M.DataProgrammata,108)
		  +
		  CASE 
			WHEN M.CodStatoTaskInfermieristico ='ER' THEN 
					CHAR(13) + CHAR(10) + 'Erogato da ' +  
					CASE	
												WHEN L2.Codice IS NULL THEN ISNULL(L.Descrizione,'')
						ELSE ISNULL(L2.Descrizione,'')
					END
					+ CHAR(13) + CHAR(10) + ' il ' + convert(varchar(20),M.DataErogazione,105) +' ' +  Convert(varchar(5),M.DataErogazione,108) 
					
			WHEN M.CodStatoTaskInfermieristico  IN ('AN') THEN 
					CHAR(13) + CHAR(10) + 'Annullato da ' +
					CASE	
												WHEN L2.Codice IS NULL THEN ISNULL(L.Descrizione,'')
						ELSE ISNULL(L2.Descrizione,'')
					END
					+ CHAR(13) + CHAR(10) + ' il '  + ISNULL(Convert(varchar(20),M.DataErogazione,105) +' ' +  Convert(varchar(5),M.DataErogazione,108),'') 

				WHEN M.CodStatoTaskInfermieristico  IN ('TR') THEN 
					CHAR(13) + CHAR(10) + 'Trascritto da ' +
					CASE	
												WHEN L2.Codice IS NULL THEN ISNULL(L.Descrizione,'')
						ELSE ISNULL(L2.Descrizione,'')
					END
					+ CHAR(13) + CHAR(10) + ' il '  + Convert(varchar(20),M.DataErogazione,105) +' ' +  Convert(varchar(5),M.DataErogazione,108) 					
			ELSE ''
		  END 
		  + 
			CASE 
				WHEN M.Note IS NOT NULL THEN  CHAR(13) + CHAR(10) +M.Note
				ELSE ''
			END	
		   + 
			CASE 
				WHEN M.PosologiaEffettiva IS NOT NULL THEN  CHAR(13) + CHAR(10) + 'Posologia Effettiva:' + M.PosologiaEffettiva 
				ELSE ''
			END		
		 AS DescrProgrammazioneErogazione,			 
		M.Note						
			
	FROM T_MovTaskInfermieristici M
		 
		LEFT JOIN 
				(SELECT ID As IDScheda, CodScheda,Versione,IDEntita,AnteprimaRTF,AnteprimaTXT
				 FROM
					T_MovSchede  WITH (NOLOCK)
				 WHERE CodEntita = 'WKI' AND							
					Storicizzata = 0
				) AS MS
			ON MS.IDEntita = M.ID
			
		LEFT JOIN T_StatoTaskInfermieristico S WITH (NOLOCK)
			ON (M.CodStatoTaskInfermieristico = S.Codice)
		
		LEFT JOIN T_TipoTaskInfermieristico T WITH (NOLOCK)
			ON M.CodTipoTaskInfermieristico=T.Codice
				
		LEFT JOIN T_Login L WITH (NOLOCK)
			ON (M.CodUtenteRilevazione = L.Codice)	
		LEFT JOIN T_Login L2  WITH (NOLOCK)
			ON (M.CodUtenteUltimaModifica = L2.Codice)						
	WHERE 
		M.CodStatoTaskInfermieristico NOT IN ('CA','IC') AND
		
				M.IDTrasferimento IN (
			SELECT ID FROM T_MovTrasferimenti		
			WHERE IDCartella=@uIDCartella)	AND
			
				1=CASE 
			WHEN @dDataDimissione IS NULL THEN 1			
			WHEN @dDataDimissione IS NOT NULL AND CodStatoTaskInfermieristico IN ('AN','PR','TR') AND 					
				DataProgrammata > @dDataDimissione THEN 0
			ELSE 1					
		  END		
	ORDER BY 
		T.Descrizione ASC, M.DataProgrammata ASC
	OPTION (USE HINT('QUERY_OPTIMIZER_COMPATIBILITY_LEVEL_100')) 	

																	  
		SELECT
		'12 - Evidenza Clinica' AS Sez12,		 
		Convert(varchar(20),M.DataEvento,105) 
					As DataEvento,
		CASE 
			WHEN DataVisione IS NOT NULL THEN
				Convert(varchar(20),M.DataVisione,105) +' ' +  Convert(varchar(5),M.DataVisione,108) 		
			ELSE NULL
			END	As DataVisione,
		T.Descrizione AS TipoReferto,
		CASE 
			WHEN ISNULL(CodStatoEvidenzaClinicaVisione,'')='VS' THEN L.Descrizione 
			ELSE ''
		END AS UtenteVistatore,
		ISNULL(M.NumeroRefertoDWH,'..........') AS NumeroReferto
		
	FROM 
		T_MovEvidenzaClinica M WITH (NOLOCK)		
			LEFT JOIN T_MovPazienti P  WITH (NOLOCK)
				ON (M.IDEpisodio=P.IDEpisodio)													
			LEFT JOIN T_TipoEvidenzaClinica T  WITH (NOLOCK)			 
				ON (M.CodTipoEvidenzaClinica=T.Codice)
			LEFT JOIN T_StatoEvidenzaClinica S  WITH (NOLOCK)
					ON (M.CodStatoEvidenzaClinica=S.Codice)
			LEFT JOIN T_StatoEvidenzaClinicaVisione S2  WITH (NOLOCK)
					ON (M.CodStatoEvidenzaClinicaVisione=S2.Codice)							
			LEFT JOIN T_Login L  WITH (NOLOCK)
					ON (M.CodUtenteVisione=L.Codice)			
	WHERE
				(M.IDEpisodio=@uIDEpisodio	
		 OR 
		 		 M.IDEpisodio 
			IN (SELECT IDEpisodio FROM T_MovTrasferimenti  WITH (NOLOCK) WHERE IDCartella IN (SELECT IDCartella FROM #tmpCartelleEpi  WITH (NOLOCK)))
		 )
						 
		AND
		
				M.CodStatoEvidenzaClinica NOT IN ('AN','CA') 
		
				AND M.CodStatoEvidenzaClinicaVisione IN ('VS','DV')
	OPTION (USE HINT('QUERY_OPTIMIZER_COMPATIBILITY_LEVEL_100')) 	
						
				
	SELECT 
		'13 - Allegati' AS Sez13,
		F.Descrizione As FormatoAllegato,		
		T.Descrizione AS TipoAllegato,
		Convert(varchar(20),M.DataRilevazione,105) +' ' +  Convert(varchar(5),M.DataRilevazione,108) As DataInserimento,
		M.TestoRTF,
		L.Descrizione AS UtenteInserimento		
	FROM		
		T_MovAllegati M  WITH (NOLOCK)
			LEFT JOIN T_TipoAllegato T  WITH (NOLOCK)
				ON (M.CodTipoAllegato=T.Codice)										
			LEFT JOIN T_StatoAllegato S  WITH (NOLOCK)
				ON (M.CodStatoAllegato=S.Codice)
		
			LEFT JOIN T_FormatoAllegati F  WITH (NOLOCK)
				ON (M.CodFormatoAllegato=F.Codice)
				
			LEFT JOIN T_Login L  WITH (NOLOCK)
				ON (M.CodUtenteRilevazione=L.Codice)	
	WHERE
				M.IDTrasferimento IN (
					SELECT ID FROM T_MovTrasferimenti		
					WHERE IDCartella=@uIDCartella)	
		AND
					M.CodStatoAllegato NOT IN ('AN','CA')
	OPTION (USE HINT('QUERY_OPTIMIZER_COMPATIBILITY_LEVEL_100')) 	

				

	SELECT 
		'14 - Appuntamenti' AS Sez14,
		T.Descrizione As TipoAppuntamento,		
		S.Descrizione AS StatoAppuntamento,
		Convert(varchar(20),M.DataInizio,105) +' ' +  Convert(varchar(5),M.DataInizio,108) As DataAppuntamento,
		M.ElencoRisorse,
		dbo.MF_PulisciRTF(MS.AnteprimaRTF) AS AnteprimaRTF,
		Convert(varchar(20),M.DataEvento,105) +' ' +  Convert(varchar(5),M.DataEvento,108) As DataInserimento,
		L.Descrizione AS UtenteInserimento		
	FROM		
		T_MovAppuntamenti M  WITH (NOLOCK)
		
						LEFT JOIN T_TipoAppuntamento T WITH (NOLOCK)	
					ON (M.CodTipoAppuntamento=T.Codice)
					
						LEFT JOIN T_StatoAppuntamento S WITH (NOLOCK)	
					ON (M.CodStatoAppuntamento=S.Codice)
					
						LEFT JOIN 
						(SELECT ID As IDScheda, CodScheda,Versione,IDEntita,AnteprimaRTF
						 FROM
							T_MovSchede WITH (NOLOCK)	  
						 WHERE CodEntita='APP' AND							
							Storicizzata=0 AND
							CodStatoScheda <> 'CA'
						) AS MS
					ON (MS.IDEntita=M.ID AND
						MS.CodScheda=T.CodScheda)	
				
			LEFT JOIN T_Login L WITH (NOLOCK)
				ON (M.CodUtenteRilevazione=L.Codice)	
	WHERE
				M.IDTrasferimento IN (
					SELECT ID FROM T_MovTrasferimenti WITH (NOLOCK)	
					WHERE IDCartella=@uIDCartella)	
		AND
				M.CodStatoAppuntamento NOT IN ('AN','CA')
		
	ORDER BY M.DataInizio ASC	
	OPTION (USE HINT('QUERY_OPTIMIZER_COMPATIBILITY_LEVEL_100'))
	
						
		SET @xTimeStamp.modify('delete (/TimeStamp/IDPaziente)[1]') 			
	SET @xTimeStamp.modify('insert <IDPaziente>{sql:variable("@uIDPaziente")}</IDPaziente> as last into (/TimeStamp)[1]')
	
		SET @xTimeStamp.modify('delete (/TimeStamp/IDEpisodio)[1]') 			
	SET @xTimeStamp.modify('insert <IDEpisodio>{sql:variable("@uIDEpisodio")}</IDEpisodio> as last into (/TimeStamp)[1]')
	
		SET @xTimeStamp.modify('delete (/TimeStamp/IDEntita)[1]') 			
	SET @xTimeStamp.modify('insert <IDEntita>{sql:variable("@uIDCartella")}</IDEntita> as last into (/TimeStamp)[1]')
	
		SET @xTimeStamp.modify('delete (/TimeStamp/CodAziente)[1]') 			
	SET @xTimeStamp.modify('insert <CodAzione>VIS</CodAzione> as first into (/TimeStamp)[1]')
	
		SET @xTimeStamp.modify('delete (/TimeStamp/Note)[1]') 			
	SET @xTimeStamp.modify('insert <Note>{sql:variable("@sNumeroCartella")}</Note> as last into (/TimeStamp)[1]')
	
	
	SET @xTimeStamp=CONVERT(XML,
						'<Parametri>' + CONVERT(varchar(max),@xTimeStamp) +
						'</Parametri>')	
						
		EXEC MSP_InsMovTimeStamp @xTimeStamp						

	DROP TABLE #tmpCartelleEpi	
	DROP TABLE #tmpEscludiPrescrizioneFarmaciPSC
END