
CREATE PROCEDURE [dbo].[MSP_SelStampaSchedaModello1](@xParametri XML)
AS

BEGIN

	
	DECLARE @uIDScheda AS UNIQUEIDENTIFIER
	DECLARE @sCodUAAmbulatoriale AS  VARCHAR(20)
	
	DECLARE @sGUID AS VARCHAR(50)
	DECLARE @sTempGUID AS VARCHAR(50)
	DECLARE @sTmp AS VARCHAR(MAX)
    DECLARE @xTimeStamp AS XML	    
    DECLARE @xPar AS XML	
    DECLARE @sCodUAScheda AS VARCHAR(20)
        
    DECLARE @sNumeroCartella VARCHAR(50)	
    DECLARE @uIDTrasferimento AS UNIQUEIDENTIFIER
    
    DECLARE @uIDPaziente AS UNIQUEIDENTIFIER
    
	DECLARE @uIDEpisodio AS UNIQUEIDENTIFIER
    
    DECLARE @sIntestazioneStampa AS VARCHAR(MAX)
    DECLARE @sIntestazioneStampaSintetica AS VARCHAR(MAX)
	DECLARE @sIntestazioneCartellaReparto AS VARCHAR(MAX)
	DECLARE @sIntestazioneCartellaRepartoSintetica AS VARCHAR(MAX)
	DECLARE @sSpallaSX AS VARCHAR(MAX)
	DECLARE @sFirmaCartella AS VARCHAR(MAX)
	DECLARE @uIDCartella AS UNIQUEIDENTIFIER

	DECLARE @sCodUA AS VARCHAR(20)
	DECLARE @sCodStatoCartella AS VARCHAR(20)
	DECLARE @sNumeroNosologico AS VARCHAR(20)
	DECLARE @dDataRicovero AS DATETIME
	DECLARE @dDataListaAttesa AS DATETIME
	DECLARE @sDescrizioneStatoCartella AS VARCHAR(50)
	DECLARE @sUnitaAtomica AS VARCHAR(255)
	DECLARE @sUnitaOperativa AS VARCHAR(255)
	DECLARE @sSettore AS VARCHAR(255)
	DECLARE @sRegime AS VARCHAR(255)
	DECLARE @sCodTipoSchedaPaziente AS VARCHAR(20)
	DECLARE @sCodTipoSchedaEpisodio AS VARCHAR(20)
	DECLARE @sTrattini AS VARCHAR(10)
	DECLARE @sCodEntitaScheda AS VARCHAR(20)
	DECLARE @sOutput AS VARCHAR(255)
	DECLARE @sCodRuolo AS VARCHAR(20)
	DECLARE @sTitolo AS VARCHAR(1000)
	DECLARE @bSchedaConFigli AS BIT
	DECLARE @sCodTipoEpisodio AS VARCHAR(20)
	DECLARE @dDataRiferimentoIntestazione AS DATETIME
	DECLARE @sWatermark AS CHAR(1)
	

	SET @uIDScheda=(SELECT TOP 1 ValoreParametro.IDScheda.value('.','UNIQUEIDENTIFIER')
					  FROM @xParametri.nodes('/Parametri/IDScheda') as ValoreParametro(IDScheda))	


	SET @sCodUAAmbulatoriale=(SELECT TOP 1 ValoreParametro.CodUAAmbulatoriale.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodUAAmbulatoriale') as ValoreParametro(CodUAAmbulatoriale))	
		
    SET @xTimeStamp=(SELECT TOP 1 ValoreParametro.TS.query('.')
					  FROM @xParametri.nodes('/Parametri/TimeStamp') as ValoreParametro(TS))

	SET @sCodRuolo=(SELECT TOP 1 ValoreParametro.CodRuolo.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/TimeStamp/CodRuolo') as ValoreParametro(CodRuolo))											
	SET @sCodRuolo=ISNULL(@sCodRuolo,'')

	SET @sTrattini='-----'
			
	SET @sCodTipoSchedaPaziente=(SELECT TOP 1 Valore FROM T_Config WHERE ID=31)
		
	SET @sCodTipoSchedaEpisodio=(SELECT TOP 1 Valore FROM T_Config WHERE ID=32)
		
	SELECT TOP 1 
			@sCodEntitaScheda=CodEntita,
			@dDataRiferimentoIntestazione = ISNULL(DataUltimaModifica,DataCreazione),
			@sCodUAScheda= CodUA
	FROM T_MovSchede WHERE ID=@uIDScheda
	
	SET  @bSchedaConFigli=(SELECT TOP 1
								CASE 
									WHEN COUNT(*) > 0 THEN CONVERT(BIT,1)
									ELSE CONVERT(BIT,0)
								 END	
						   FROM T_SchedePadri WITH (NOLOCK)
						   WHERE CodSchedaPadre 
								IN (SELECT CodScheda FROM T_MovSchede WHERE ID=@uIDScheda)
						   GROUP BY CodSchedaPadre 
							)
	
		
	SET @sWatermark = (SELECT TOP 1
							CASE 
									WHEN ISNULL(M.Validabile,0) = 1 AND ISNULL(M.Validata,0)=0  AND ISNULL(S.Revisione,0) = 1 THEN '1'
									ELSE '0'
							END
						FROM T_MovSchede M
							LEFT JOIN T_Schede S
								ON M.CodScheda = S.Codice
						WHERE ID=@uIDScheda
					 )

	IF @sCodEntitaScheda='PAZ'
		BEGIN			
			
			SET @sNumeroCartella=NULL
			SET @uIDCartella=NULL
			SET @sCodUA=NULL
			SET @sSettore=NULL
			SET @sUnitaOperativa=NULL
			SET @sNumeroNosologico=NULL
			SET @uIDEpisodio=NULL
			SET @uIDTrasferimento=NULL
			SET @dDataRicovero=NULL
			SET @dDataListaAttesa=NULL
			SET @sOutput=''			
			
			SET @sUnitaAtomica=NULL
			SET @sFirmaCartella=NULL			
			SET @sDescrizioneStatoCartella=NULL
			SET @sIntestazioneCartellaReparto=NULL
			SET @sIntestazioneCartellaRepartoSintetica=NULL
			SET @sSpallaSX=NULL
			SET @sCodTipoEpisodio=NULL
			
			SET @sRegime=NULL
			
			SET @sTitolo='DOCUMENTAZIONE AMBULATORIALE'
						
			SET @uIDPaziente=(SELECT TOP 1 IDPaziente FROM T_MovSchede WHERE ID=@uIDScheda)
						
			IF ISNULL(@sCodUAAmbulatoriale,'') = ''
			BEGIN
				SET @sCodUAAmbulatoriale=(SELECT TOP 1 CodUA FROM T_MovSchede WHERE ID=@uIDScheda)
			END
			
			
			IF ISNULL(@sCodUAAmbulatoriale,'') <> ''
				BEGIN
							
					SET @sIntestazioneCartellaReparto=(SELECT dbo.MF_CercaIntestazione('CARTSTD',@dDataRiferimentoIntestazione,@sCodUAAmbulatoriale))
									
					SET @sIntestazioneCartellaRepartoSintetica=(SELECT dbo.MF_CercaIntestazione('CARTSINT',@dDataRiferimentoIntestazione,@sCodUAAmbulatoriale))
					
					SET @sSpallaSX=(SELECT dbo.MF_CercaIntestazione('SPALLASX',@dDataRiferimentoIntestazione,@sCodUAAmbulatoriale))
					
					SELECT TOP 1 
						@sUnitaAtomica=Descrizione,
						@sFirmaCartella=FirmaCartella
					FROM T_UnitaAtomiche  WITH (NOLOCK)
					WHERE Codice=@sCodUAAmbulatoriale
										
					SET @sIntestazioneStampa=(SELECT TOP 1 RTFStampaEstesa FROM T_Aziende 
											  WHERE Codice IN (SELECT CodAzienda FROM T_UnitaAtomiche WHERE Codice=@sCodUAAmbulatoriale))
					
					SET @sIntestazioneStampaSintetica=(SELECT TOP 1 RTFStampaSintetica FROM T_Aziende 
											 WHERE Codice IN (SELECT CodAzienda FROM T_UnitaAtomiche WHERE Codice=@sCodUAAmbulatoriale))
				END	
			ELSE
				BEGIN
					SET @sIntestazioneCartellaReparto=(SELECT dbo.MF_CercaIntestazione('CARTSTD',@dDataRiferimentoIntestazione,'0'))

					SET @sIntestazioneCartellaRepartoSintetica=(SELECT dbo.MF_CercaIntestazione('CARTSINT',@dDataRiferimentoIntestazione,'0'))
					
					SET @sSpallaSX=(SELECT dbo.MF_CercaIntestazione('SPALLASX',@dDataRiferimentoIntestazione,'0'))
					
					SET @sIntestazioneStampa=(SELECT TOP 1 RTFStampaEstesa FROM T_Aziende 
											  WHERE Codice IN (SELECT CodAzienda FROM T_UnitaAtomiche WHERE Codice='0'))

					SET @sIntestazioneStampaSintetica=(SELECT TOP 1 RTFStampaSintetica FROM T_Aziende 
											 WHERE Codice IN (SELECT CodAzienda FROM T_UnitaAtomiche WHERE Codice='0'))
				END			
		
		END	
	ELSE
		IF @sCodEntitaScheda='EPI'
			BEGIN
				SELECT TOP 1 
					@uIDEpisodio=IDEpisodio, 
					@uIDTrasferimento=IDTrasferimento 
				FROM T_MovSchede WITH (NOLOCK)
				WHERE ID=@uIDScheda
								
				SET @uIDcartella=(SELECT IDCartella FROM T_MovTrasferimenti WITH (NOLOCK) WHERE ID=@uIDTrasferimento)
								
				SET @sNumeroCartella=(SELECT TOP 1 NumeroCartella FROM T_MovCartelle WITH (NOLOCK) WHERE ID=@uIDcartella)	
				
							
				SELECT TOP 1 		
					@sCodUA=CodUA,
					@sSettore=T.DescrSettore,
					@sUnitaOperativa=T.DescrUO,
					@sNumeroNosologico=E.NumeroNosologico,
					@uIDEpisodio=E.ID,
					@uIDTrasferimento=T.ID,
					@dDataRicovero=E.DataRicovero,
					@dDataListaAttesa=E.DataListaAttesa,
					@sCodTipoEpisodio = E.CodTipoEpisodio
				FROM T_MovTrasferimenti T WITH (NOLOCK)
					LEFT JOIN T_MovEpisodi  E WITH (NOLOCK)
					 ON T.IDEpisodio=E.ID
				WHERE IDCartella=@uIDCartella AND
					  T.CodStatoTrasferimento NOT IN ('CA')
								
				SET @sIntestazioneStampa=(SELECT TOP 1 RTFStampaEstesa FROM T_Aziende 
											  WHERE Codice IN (SELECT CodAzienda FROM T_UnitaAtomiche WHERE Codice=@sCodUA))
				
				SET @sIntestazioneStampaSintetica=(SELECT TOP 1 RTFStampaSintetica FROM T_Aziende 
											 WHERE Codice IN (SELECT CodAzienda FROM T_UnitaAtomiche WHERE Codice=@sCodUA))
											 
				SELECT TOP 1 
					@sUnitaAtomica=Descrizione,
					@sFirmaCartella=FirmaCartella
				FROM T_UnitaAtomiche WITH (NOLOCK)
				WHERE Codice=@sCodUA
						
				SET @sDescrizioneStatoCartella=(
					SELECT TOP 1 
						CASE 
							WHEN CodStatoCartella='AP' THEN 'CARTELLA APERTA'
							WHEN CodStatoCartella='CH' THEN 'CARTELLA CHIUSA IL ' + CONVERT(VARCHAR(10),DataChiusura,105)
							ELSE ''
						END	
					FROM
						T_MovCartelle WITH (NOLOCK)
					WHERE 
						ID=@uIDCartella)
													
					SET @sIntestazioneCartellaReparto=(SELECT dbo.MF_CercaIntestazione('CARTSTD',@dDataRiferimentoIntestazione,@sCodUA))
					
					SET @sIntestazioneCartellaRepartoSintetica=(SELECT dbo.MF_CercaIntestazione('CARTSINT',@dDataRiferimentoIntestazione,@sCodUA))
						
					SET @sSpallaSX=(SELECT dbo.MF_CercaIntestazione('SPALLASX',@dDataRiferimentoIntestazione,@sCodUA))
								
				SET  @sRegime=(SELECT 
									TOP 1 
									ISNULL(T.Descrizione,'')
								FROM T_MovEpisodi E WITH (NOLOCK)
									LEFT JOIN T_TipoEpisodio T
										ON E.CodTipoEpisodio=T.Codice
								WHERE E.ID=	@uIDEpisodio	
					)		
									
				SET @uIDPaziente=(SELECT TOP 1 
							IDPaziente 
					  FROM T_MovPazienti MP	WITH (NOLOCK)												
					  WHERE		
							MP.IDEpisodio=@uIDEpisodio
					  )						
				SET @sTitolo='DOCUMENTAZIONE DI ' + ISNULL(@sRegime,'') + ' - ' + ISNULL(@sUnitaOperativa,'')
				SET @sOutput=''
			END
		ELSE
			BEGIN
		
				SET  @sOutput='Stampa non disponibile per la scheda selezionata.'
			END
		
	SELECT 
		'0 - Dati Generali' AS Sez00,		
		@sIntestazioneStampa AS IntestazioneStampa,
		@sIntestazioneStampaSintetica AS IntestazioneStampaSintetica,
		@sIntestazioneCartellaReparto AS IntestazioneCartellaReparto,
		@sIntestazioneCartellaRepartoSintetica AS IntestazioneCartellaRepartoSintetica,
		ISNULL(@sTitolo,@sTrattini) AS Titolo,
		ISNULL(@sRegime,@sTrattini) AS Regime,
		ISNULL(@sUnitaAtomica,@sTrattini) AS UnitaAtomica,
		ISNULL(@sUnitaOperativa,@sTrattini) AS UnitaOperativa,
		ISNULL(@sSettore,@sTrattini) AS Settore,
		ISNULL(@sNumeroCartella,@sTrattini) AS NumeroCartella,
		ISNULL(@sDescrizioneStatoCartella,@sTrattini) AS DescrizioneStatoCartella,
		ISNULL(@sFirmaCartella,@sTrattini) AS FirmaCartella,
		ISNULL(@sNumeroNosologico,@sTrattini) AS NumeroNosologico,
		ISNULL(@sCodTipoEpisodio, @sTrattini) AS CodTipoEpisodio,
		@sSpallaSX AS SpallaSinistra,
		ISNULL(@sWatermark,0) AS Watermark		
	
	IF @sCodEntitaScheda='PAZ'
		BEGIN				
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
						
						@sTrattini AS  EtaAllAccesso,
						
						CASE
						  WHEN	
								LTRIM(ISNULL(LocalitaNascita,'')) <> LTRIM(ISNULL(ComuneNascita,'')) THEN
									LTRIM(ISNULL(LocalitaNascita,'') + ' ' + ISNULL(ComuneNascita,''))
						  ELSE
								ISNULL(ComuneNascita,ISNULL(LocalitaNascita,''))
						END		
						AS  LuogoNascita,
						
						
						LTRIM(	
								ISNULL(IndirizzoResidenza,'')	+ ', ' +		
								
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
												
						LTRIM(	
								ISNULL(IndirizzoDomicilio,'')	+ ', ' +		
								
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
					FROM 
						T_Pazienti MP WITH (NOLOCK)												
					WHERE							
						MP.ID=@uIDPaziente
											
		END
		
	ELSE
		IF @sCodEntitaScheda='EPI'
			BEGIN 
				
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
									THEN CONVERT(VARCHAR(50),dbo.MF_CalcolaEta(DataNascita,ISNULL(@dDataRicovero,@dDataListaAttesa)))
							ELSE CONVERT(VARCHAR(50),'0')
						END AS EtaAllAccesso,
				
						CASE
						  WHEN	
								LTRIM(ISNULL(LocalitaNascita,'')) <> LTRIM(ISNULL(ComuneNascita,'')) THEN
									LTRIM(ISNULL(LocalitaNascita,'') + ' ' + ISNULL(ComuneNascita,''))
						  ELSE
								ISNULL(ComuneNascita,ISNULL(LocalitaNascita,''))
						END		
						AS  LuogoNascita,
						
				
						LTRIM(
								ISNULL(IndirizzoResidenza,'')	+ ', ' +		
								
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
						
						
						LTRIM(	
								ISNULL(IndirizzoDomicilio,'')	+ ', ' +		
								
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
					FROM 
						T_MovPazienti MP WITH (NOLOCK)													
					WHERE	
						
						MP.IDEpisodio=@uIDEpisodio

			END

	CREATE TABLE #tmpUAGerarchia
	(
		CodUA VARCHAR(20) COLLATE Latin1_General_CI_AS,
		Descrizione VARCHAR(255) COLLATE Latin1_General_CI_AS
	)
	
	CREATE TABLE #tmpIDSchede
		(
			IDScheda UNIQUEIDENTIFIER	
		)
		;
	
	CREATE INDEX IX_CodUA ON #tmpUAGerarchia (CodUA) 
	

	IF @bSchedaConFigli=1
	BEGIN
		CREATE TABLE #tmpUA
		(
			CodUA VARCHAR(20) COLLATE Latin1_General_CI_AS,
			Descrizione VARCHAR(255) COLLATE Latin1_General_CI_AS
		)

		DECLARE @xTmp AS XML
		
		SET @xTmp =CONVERT(XML,'<Parametri><CodRuolo>'+@sCodRuolo+'</CodRuolo></Parametri>')
		
		INSERT #tmpUA EXEC MSP_SelUADaRuolo @xTmp	
			
		CREATE INDEX IX_CodUA ON #tmpUA (CodUA)  
				
		CREATE TABLE #tmpUAPadri
		(
			CodUA VARCHAR(20) COLLATE Latin1_General_CI_AS,
			Descrizione VARCHAR(255) COLLATE Latin1_General_CI_AS
		)
		
		CREATE INDEX IX_CodUA ON #tmpUAPadri (CodUA) 
														
		DECLARE @sUATemp AS VARCHAR(20)
			
		DECLARE CursoreUA CURSOR FOR 
		SELECT CodUA
			FROM #tmpUA
		FOR READ ONLY ;
		
		OPEN CursoreUA
		
		FETCH NEXT FROM CursoreUA INTO @sUATemp
			
		WHILE @@FETCH_STATUS = 0
		BEGIN	
		
			SET @xTmp =CONVERT(XML,'<Parametri><CodUA>'+@sUATemp+'</CodUA></Parametri>')
		
			INSERT #tmpUAPadri EXEC MSP_SelUAPadri @xTmp	
			
			INSERT INTO #tmpUAGerarchia(CodUA,Descrizione)
			SELECT CodUA,Descrizione
			FROM #tmpUAPadri
			WHERE CodUA NOT IN (SELECT CodUA FROM #tmpUAGerarchia)
			
			FETCH NEXT FROM CursoreUA INTO @sUATemp
		END

		INSERT INTO #tmpUAGerarchia(CodUA)
		SELECT @sCodUAScheda	
		WHERE 
			@sCodUAScheda NOT IN (SELECT CodUA FROM #tmpUAGerarchia)
		
		;		
		WITH GerarchiaSchede(IDSchedaPadre,IDSchedaFiglia)
		AS
		( SELECT 		
				M.IDSchedaPadre,
				M.ID AS IDSchedaFiglia			
		  FROM T_MovSchede M WITH (NOLOCK)
		  WHERE 
				CodStatoScheda<>'CA' AND
				Storicizzata=0 AND
				(IDSchedaPadre=@uIDScheda OR ID=@uIDScheda)
		  UNION ALL
		  SELECT 		
				M2.IDSchedaPadre,
				M2.ID AS IDSchedaFiglia			
		  FROM T_MovSchede M2 WITH (NOLOCK, INDEX(IX_IDSchedaPadreCodStatoStoricizzata))
				INNER JOIN GerarchiaSchede G
					ON M2.IDSchedaPadre=G.IDSchedaFiglia

			
		  WHERE 
				CodStatoScheda<>'CA' AND
				Storicizzata=0 			
		)	
		INSERT INTO #tmpIDSchede(IDScheda)
		SELECT	DISTINCT 		
			IDSchedaFiglia AS IDScheda
		FROM  GerarchiaSchede WITH (NOLOCK)	

		

	END 
	ELSE
	BEGIN
	
		INSERT INTO #tmpUAGerarchia(CodUA)
		SELECT @sCodUAScheda
	
		INSERT INTO #tmpIDSchede(IDScheda)
		SELECT @uIDScheda
	END	
		
	SET @sTempGUID=NEWID()
			
	SELECT 
		'2 - Schede cliniche' AS Sez02,					
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
				
		dbo.MF_PulisciRTF(AnteprimaRTF)AS AnteprimaRTF,
				
		CONVERT(VARCHAR(10), DataCreazione, 105) AS DataCreazione,
				
		CONVERT(VARCHAR(10), ISNULL(DataUltimaModifica,DataCreazione), 105) 
			+ ' ' + CONVERT(varchar(5), ISNULL(DataUltimaModifica,DataCreazione), 14) AS DataUltimaModifica,	
				
		ISNULL(M.CodUtenteUltimaModifica,M.CodUtenteRilevazione) AS CodUtenteUltimaModifica,
				CASE	
					WHEN ISNULL(M.CodUtenteUltimaModifica,'')<>'' THEN 
							UM.Descrizione +
								CASE
									WHEN ISNULL(UM.CodiceFiscale,'') <> '' THEN CHAR(13) + CHAR(10) + UM.CodiceFiscale
									ELSE ''
								END	 
					ELSE UC.Descrizione +
								CASE
									WHEN ISNULL(UC.CodiceFiscale,'') <> '' THEN CHAR(13) + CHAR(10) + UC.CodiceFiscale
									ELSE ''
								END	
				END	
		AS DescrUtenteUltimaModifica,	
		UV.Descrizione + CHAR(13) + CHAR(10) + ISNULL(UV.CodiceFiscale,'') AS DescrUtenteValidazione,
		CASE 
			WHEN DataValidazione IS NOT NULL THEN	
					CONVERT(VARCHAR(10), DataValidazione, 105) + ' ' + CONVERT(varchar(5), DataValidazione, 14)
			ELSE ''
		END AS DataValidazione,
				
		M.ID AS IDScheda
		,TPadri.IDScheda AS IDSchedaPadre
		   	 
	FROM 
		T_MovSchede M WITH (NOLOCK)
			INNER JOIN T_Schede S WITH (NOLOCK)
				ON M.CodScheda=S.Codice							
			INNER JOIN				
				#tmpIDSchede TS WITH (NOLOCK)
					ON TS.IDScheda=M.ID
			
			LEFT JOIN				
				(SELECT DISTINCT 
					IDScheda
				 FROM	
					#tmpIDSchede WITH (NOLOCK)
				 ) AS TPadri
					ON TPadri.IDScheda=M.IDSchedaPadre
										
			LEFT JOIN T_Login UC
						ON M.CodUtenteRilevazione = UC.Codice	
							
			LEFT JOIN T_Login UM
						ON M.CodUtenteUltimaModifica = UM.Codice
									
			LEFT JOIN T_Login UV
						ON M.CodUtenteValidazione = UV.Codice	
								
	WHERE 
		M.Storicizzata=0 AND 
			
		CodStatoScheda NOT IN ('AN','CA') AND
				
		M.CodEntita IN ('EPI','PAZ') AND
				
		(M.IDPaziente=@uIDPaziente 
		 OR		
		 M.IDPaziente IN 
					(SELECT IDPazienteVecchio
					 FROM T_PazientiAlias WITH (NOLOCK)
					 WHERE 
						IDPaziente IN 
							(SELECT IDPaziente 
							 FROM T_PazientiAlias WITH (NOLOCK)
							 WHERE IDPazienteVecchio=@uIDPaziente
							)
					)
		)	
		AND
		
		ISNULL(M.IDEpisodio,@sTempGUID)=
						CASE 
							WHEN M.CodEntita ='EPI' THEN @uIDEpisodio 
							ELSE ISNULL(M.IDEpisodio,@sTempGUID)
						END			
		AND
		ISNULL(M.CodUA,'<@!@!>') IN (SELECT T.CodUA 
									  FROM 
									  #tmpUAGerarchia T WITH (NOLOCK)
									  UNION
									  SELECT '<@!@!>')
			
	ORDER BY 
		S.Ordine ASC,
		M.DataCreazione ASC		
	
	SELECT 
		'99 - Output' AS Sez99,
		ISNULL(@sOutput,'') AS Risultato
			
END