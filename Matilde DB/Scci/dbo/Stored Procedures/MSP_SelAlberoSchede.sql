		  
  

																										
				 
  

						
  

CREATE PROCEDURE [dbo].[MSP_SelAlberoSchede](@xParametri AS XML)
AS
BEGIN

	
				
	DECLARE @uIDPaziente AS UNIQUEIDENTIFIER
	DECLARE @uIDEpisodio AS UNIQUEIDENTIFIER
	DECLARE @sCodScheda VARCHAR(20)
	DECLARE @sCodTipoScheda VARCHAR(20)
	DECLARE @sCodAgenda  VARCHAR(20)
	DECLARE @bDaCompletare AS Bit	
	DECLARE @sDaCompletare AS VARCHAR(1)	
	DECLARE @dDataInizio AS DATETIME
	DECLARE @dDataFine AS DATETIME
	DECLARE @sCodRuolo AS VARCHAR(20)
	DECLARE @bSoloSchedePaziente AS BIT
	DECLARE @bSoloSchedeEpisodio AS BIT
	DECLARE @uIDEpisodioCorrente AS UNIQUEIDENTIFIER

	
	DECLARE @bDatiEstesi AS Bit	
	
	DECLARE @nStatoAppuntamento INTEGER
	DECLARE @nStatoEpisodio INTEGER
	
		DECLARE @sGUID AS VARCHAR(50)
	DECLARE @sDataTmp AS VARCHAR(20)
	DECLARE @nTemp AS INTEGER
	
	DECLARE @bInserisci AS  BIT
	DECLARE @bModifica AS  BIT
	DECLARE @bCancella AS  BIT	
	
	DECLARE @sOrdineFittizio AS VARCHAR(20)
	DECLARE @dDataCreazioneFittizia AS DATETIME

	DECLARE @uIDPazienteFuso AS UNIQUEIDENTIFIER
	
	DECLARE @sCodRuoloSuperUser AS VARCHAR(20)
	DECLARE @bSuperUser AS BIT
	
	SET @sOrdineFittizio=NULL
	SET @dDataCreazioneFittizia=NULL
	
		DECLARE @sCodTipoConsensoGenerico AS VARCHAR(20)	
	DECLARE @dDataConsensoGenerico AS DATETIME

	DECLARE @sCodTipoConsensoDossier AS VARCHAR(20)	
	DECLARE @dDataConsensoDossier AS DATETIME
	
	DECLARE @sCodTipoConsensoDossierStorico AS VARCHAR(20)	
	DECLARE @dDataConsensoDossierStorico AS DATETIME

	DECLARE @sCodTipoConsensoPiuAlto AS VARCHAR(20)	

	DECLARE @dDataConsensoRiferimento AS DATETIME
	DECLARE @bAbilitaConsensi AS BIT

		DECLARE @bDebug AS BIT

			
		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDPaziente.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDPaziente') as ValoreParametro(IDPaziente))
	IF 	ISNULL(@sGUID,'') <> '' SET @uIDPaziente=CONVERT(UNIQUEIDENTIFIER,	@sGUID)	

		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDEpisodio.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDEpisodio') as ValoreParametro(IDEpisodio))
	IF 	ISNULL(@sGUID,'') <> '' SET @uIDEpisodio=CONVERT(UNIQUEIDENTIFIER,	@sGUID)
	
		SET @sCodTipoScheda=(SELECT TOP 1 ValoreParametro.CodTipoScheda.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodTipoScheda') as ValoreParametro(CodTipoScheda))
	SET @sCodTipoScheda=ISNULL(@sCodTipoScheda,'')
	
		SET @sCodScheda=(SELECT TOP 1 ValoreParametro.CodScheda.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodScheda') as ValoreParametro(CodScheda))
	SET @sCodScheda=ISNULL(@sCodScheda,'')
	
		SET @sCodAgenda=(SELECT TOP 1 ValoreParametro.CodAgenda.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodAgenda') as ValoreParametro(CodAgenda))
	SET @sCodAgenda=ISNULL(@sCodAgenda,'')
	
		SET @bDaCompletare=(SELECT TOP 1 ValoreParametro.DaCompletare.value('.','bit')
					  FROM @xParametri.nodes('/Parametri/DaCompletare') as ValoreParametro(DaCompletare))											
	SET @bDaCompletare=ISNULL(@bDaCompletare,0)
	
	IF @bDaCompletare=0 
		SET @sDaCompletare='0'
	ELSE 
		SET @sDaCompletare='1'
		
		
		SET @bSoloSchedePaziente=(SELECT TOP 1 ValoreParametro.SoloSchedePaziente.value('.','bit')
										  FROM @xParametri.nodes('/Parametri/SoloSchedePaziente	') as ValoreParametro(SoloSchedePaziente))											
	SET @bSoloSchedePaziente=ISNULL(@bSoloSchedePaziente,0)
	

		SET @bSoloSchedeEpisodio=(SELECT TOP 1 ValoreParametro.SoloSchedeEpisodio.value('.','bit')
										  FROM @xParametri.nodes('/Parametri/SoloSchedeEpisodio') as ValoreParametro(SoloSchedeEpisodio))											
	SET @bSoloSchedeEpisodio=ISNULL(@bSoloSchedeEpisodio,0)
			
		SET @nStatoAppuntamento=(SELECT TOP 1 ValoreParametro.StatoAppuntamento.value('.','INTEGER')
					  FROM @xParametri.nodes('/Parametri/StatoAppuntamento') as ValoreParametro(StatoAppuntamento))											
	SET @nStatoAppuntamento=ISNULL(@nStatoAppuntamento,0)
	
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
	
		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDEpisodioCorrente.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/TimeStamp/IDEpisodio') as ValoreParametro(IDEpisodioCorrente))
	IF 	ISNULL(@sGUID,'') <> '' SET @uIDEpisodioCorrente=CONVERT(UNIQUEIDENTIFIER,	@sGUID)
	ELSE
		SET @uIDEpisodioCorrente=NULL
		
	    DECLARE @xTimeStamp AS XML	

    SET @xTimeStamp=(SELECT TOP 1 ValoreParametro.TS.query('.')
					  FROM @xParametri.nodes('/Parametri/TimeStamp') as ValoreParametro(TS))
	
	SET @xTimeStamp=CONVERT(XML,
						'<Parametri>' + CONVERT(varchar(max),@xTimeStamp) +
						'</Parametri>')


		
		SET @nTemp=(SELECT COUNT(*) FROM T_AssRuoliModuli
				WHERE CodModulo='Schede_Inserisci'	
				AND CodRuolo=@sCodRuolo)
	IF @nTemp>=1 
		SET @bInserisci=1
	ELSE
		SET @bInserisci=0	
	
		SET @nTemp=(SELECT COUNT(*) FROM T_AssRuoliModuli
				WHERE CodModulo='Schede_Modifica'	
				AND CodRuolo=@sCodRuolo)
	IF @nTemp>=1 
		SET @bModifica=1
	ELSE
		SET @bModifica=0	
				
		SET @nTemp=(SELECT COUNT(*) FROM T_AssRuoliModuli
				WHERE CodModulo='Schede_Cancella'	
				AND CodRuolo=@sCodRuolo)
	IF @nTemp>=1 
		SET @bCancella=1
	ELSE
		SET @bCancella=0	
	
			
			
		SET @uIDPazienteFuso=(SELECT TOP 1 IDPaziente FROM T_PazientiAlias WHERE IDPazienteVecchio=@uIDPaziente)
	IF @uIDPazienteFuso IS NULL SET @uIDPazienteFuso=@uIDPaziente

			
		SELECT TOP 1
		@sCodTipoConsensoGenerico=CodTipoConsenso,
		@dDataConsensoGenerico=DataConsenso
	FROM T_PazientiConsensi 
	WHERE
		IDPaziente=@uIDPazienteFuso AND 
		CodStatoConsenso='SI' AND
		CodTipoConsenso='Generico'

		SELECT TOP 1
		@sCodTipoConsensoDossier=CodTipoConsenso,
		@dDataConsensoDossier=DataConsenso
	FROM T_PazientiConsensi 
	WHERE
		IDPaziente=@uIDPazienteFuso AND 
		CodStatoConsenso='SI' AND
		CodTipoConsenso='Dossier'

		SELECT TOP 1
		@sCodTipoConsensoDossierStorico=CodTipoConsenso,
		@dDataConsensoDossierStorico=DataConsenso
	FROM T_PazientiConsensi 
	WHERE
		IDPaziente=@uIDPazienteFuso AND 
		CodStatoConsenso='SI' AND
		CodTipoConsenso='DossierStorico'

		IF @sCodTipoConsensoDossierStorico IS NOT NULL
	BEGIN		
				SET @sCodTipoConsensoPiuAlto=@sCodTipoConsensoDossierStorico
		SET @dDataConsensoRiferimento=CONVERT(DATETIME,'1900-01-01',120)		
	END
	ELSE
	BEGIN
		IF @sCodTipoConsensoDossier IS NOT NULL
		BEGIN
						SET @sCodTipoConsensoPiuAlto=@sCodTipoConsensoDossier
			SET @dDataConsensoRiferimento=@dDataConsensoDossier			
		END
		ELSE
		IF @sCodTipoConsensoGenerico IS NOT NULL
		BEGIN
						SET @sCodTipoConsensoPiuAlto=@sCodTipoConsensoGenerico
			SET @dDataConsensoRiferimento= NULL
		END
		ELSE
		BEGIN
						SET @sCodTipoConsensoPiuAlto= NULL
			SET @dDataConsensoRiferimento= NULL
		END
	END

		SET @bDebug=0

	IF (@@SERVERNAME='UNISRV-SQLTEST\SQL2014')
	BEGIN
		SET @bDebug=1
		SET @sCodTipoConsensoPiuAlto ='DossierStorico'
		SET @dDataConsensoRiferimento=NULL
	END
							
	SET @sCodRuoloSuperUser=(SELECT TOP 1 Valore FROM T_Config WHERE ID=681)
	SET @sCodRuoloSuperUser=ISNULL(@sCodRuoloSuperUser,'')
	
	IF @sCodRuoloSuperUser=@sCodRuolo 
		SET @bSuperUser=1
	ELSE
		SET @bSuperUser=0
				
	
		SET @bAbilitaConsensi= (SELECT CONVERT(BIT,ISNULL(Valore,0)) FROM T_Config WHERE ID=508)
	
						
				
		
	CREATE TABLE  #tmpPazientiAlias
	(
		IDPaziente UNIQUEIDENTIFIER
	)
	
	INSERT INTO  #tmpPazientiAlias(IDPaziente)
	VALUES (@uIDPaziente)
	
	INSERT INTO   #tmpPazientiAlias(IDPaziente)
	SELECT IDPazienteVecchio
	FROM 
		(SELECT IDPazienteVecchio
		 FROM T_PazientiAlias
		 WHERE 
			IDPaziente IN 
				(SELECT IDPaziente
				 FROM T_PazientiAlias
				 WHERE IDPazienteVecchio=@uIDPaziente
				) 
		GROUP BY IDPazienteVecchio						
		) AS Q
	WHERE
		IDPazienteVecchio NOT IN (SELECT @uIDPaziente FROM  #tmpPazientiAlias) 	
	
	CREATE INDEX IX_IDPaziente ON  #tmpPazientiAlias (IDPaziente)  
	
	
	
				
	
		
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

	CREATE TABLE #tmpUAFiglie
	(
		CodUA VARCHAR(20) COLLATE Latin1_General_CI_AS,
		Descrizione VARCHAR(255) COLLATE Latin1_General_CI_AS
	)
	CREATE INDEX IX_CodUA ON #tmpUAFiglie (CodUA) 
										
					CREATE TABLE #tmpUAGerarchia
	(
		CodUA VARCHAR(20) COLLATE Latin1_General_CI_AS,
		Descrizione VARCHAR(255) COLLATE Latin1_General_CI_AS
	)
	
	CREATE INDEX IX_CodUA ON #tmpUAGerarchia (CodUA) 
	
		
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
		
		IF (@bAbilitaConsensi=1)
			INSERT #tmpUAFiglie EXEC MSP_SelUAFiglie @xTmp

		INSERT INTO #tmpUAGerarchia(CodUA,Descrizione)
		SELECT CodUA,Descrizione
		FROM #tmpUAPadri
		WHERE CodUA NOT IN (SELECT CodUA FROM #tmpUAGerarchia)
		
		FETCH NEXT FROM CursoreUA INTO @sUATemp
	END
		
			
		
					
	CREATE TABLE #tmpSchedeAbilitate
	(
		Codice VARCHAR(20) COLLATE Latin1_General_CI_AS
	)
	
	DECLARE @UATemp AS VARCHAR(20)
				
	INSERT #tmpSchedeAbilitate 
	SELECT W.Codice	 
			FROM						
				(SELECT DISTINCT 
					A.CodVoce AS Codice												
				 FROM					
					T_AssUAEntita A
						INNER JOIN #tmpUAGerarchia T ON
							A.CodUA=T.CodUA					
				 WHERE CodEntita='SCH'			
				 ) AS W				
				 
	CREATE INDEX IX_Codice ON #tmpSchedeAbilitate (Codice)   
	
						CREATE TABLE #tmpSchedeRuolo
		(
			Codice VARCHAR(20) COLLATE Latin1_General_CI_AS
		)
	
	INSERT INTO #tmpSchedeRuolo(Codice)
			SELECT DISTINCT CodVoce FROM T_AssRuoliAzioni
			WHERE CodAzione='INS' AND
				  CodEntita='SCH' AND
				  CodRuolo =@sCodRuolo						
				  
	CREATE INDEX IX_CodUA ON #tmpSchedeRuolo (Codice)    
	
		
		DELETE FROM #tmpSchedeAbilitate
	WHERE Codice NOT IN (SELECT Codice FROM #tmpSchedeRuolo)
				
			
		
				CREATE TABLE #tmpSchedeRiservateVisualizza
		(
			Codice VARCHAR(20) COLLATE Latin1_General_CI_AS
		)
	INSERT INTO #tmpSchedeRiservateVisualizza(Codice)
			SELECT DISTINCT CodVoce FROM T_AssRuoliAzioni
			WHERE CodAzione='VSR' AND
				  CodEntita='SCH' AND
				  CodRuolo =@sCodRuolo		

				CREATE TABLE #tmpSchedeRevisione
		(
			Codice VARCHAR(20) COLLATE Latin1_General_CI_AS
		)
	INSERT INTO #tmpSchedeRevisione(Codice)
			SELECT DISTINCT CodVoce FROM T_AssRuoliAzioni
			WHERE CodAzione='REV' AND
				  CodEntita='SCH' AND
				  CodRuolo =@sCodRuolo		
				  						  		
				
	CREATE TABLE #tmpIDFiltroSchede
	(
		IDScheda UNIQUEIDENTIFIER  
	)

	DECLARE @xTmpSchede AS XML
	
		SET @xTmpSchede =CONVERT(XML,
					'<Parametri>'+
						'<CodScheda>'+@sCodScheda + '</CodScheda>' +
						'<IDPaziente>'+CONVERT(VARCHAR(50),@uIDPaziente) + '</IDPaziente>'+
					' </Parametri>')
	
		INSERT #tmpIDFiltroSchede EXEC MSP_SelSchedePadriID @xTmpSchede		
	
	CREATE INDEX IX_IDScheda ON #tmpIDFiltroSchede (IDScheda)   
	
			
	
	

									

					SELECT 
								M.CodEntita,
				IDEntita,
				M.ID AS IDScheda,
				IDSchedaPadre,	
				
																																												
				NULL AS IDRaggruppamento,
				NULL AS DescrizioneRaggruppamento,
				
				M.CodScheda,
								S.Descrizione + 

								CASE 
										WHEN CA.ID IS NULL THEN
												CASE	
							WHEN ISNULL(S.NumerositaMassima,0) > 1	THEN ' (' + CONVERT(varchar(20),M.Numero) + ')'
							ELSE '' 
						END						
				
						+ ' [' +
						(CASE 
								WHEN DataCreazione IS NOT NULL THEN
									CONVERT(VARCHAR(10),M.DataCreazione,105) + 
											' ' + CONVERT(VARCHAR(5), M.DataCreazione,14) 
								ELSE ''
						END)
						+ '] '


					ELSE
												+ CA.NumeroCartella + CHAR(10) + CHAR(13)
						+ 'Aperta il: ' +
						(CASE 
								WHEN CA.DataApertura IS NOT NULL THEN
									CONVERT(VARCHAR(10),CA.DataApertura,105)
								ELSE ''
						END)
						+
							(CASE 
								WHEN CA.DataChiusura IS NOT NULL THEN
									', Chiusa il: ' + CONVERT(VARCHAR(10),CA.DataChiusura,105)
								ELSE ''
						END)
						

				END
			
				As Descrizione,
																		
				(CASE 
					WHEN M.ID IS NOT NULL AND ISNULL(M.DatiObbligatoriMancantiRTF,'') <> '' THEN 1								ELSE 0
				END) AS DatiMancanti,
				CONVERT(INTEGER, ISNULL(M.InEvidenza, 0)) AS InEvidenza,
				CONVERT(INTEGER, ISNULL(M.Validata, 0)) AS Validata,											CONVERT(INTEGER, ISNULL(M.Riservata, 0)) AS Riservata,															ISNULL(S.Ordine,'~~~~') AS Ordine,																
								CONVERT(INTEGER,
					@bInserisci
										& CASE		
						WHEN CA.ID IS NULL THEN 1
						WHEN ISNULL(CA.CodStatoCartella,'') ='AP' THEN 1
						ELSE 0
					  END
				) 
				AS	PermessoInserisci,												CONVERT(INTEGER,
						@bModifica &																							CASE 						
							WHEN ISNULL(TUS.Codice,'')<>''  THEN 1
							ELSE 0
						END	&
						CASE
							WHEN ISNULL(M.Validata, 0)=1 THEN 0
							ELSE 1
						END	
												& CASE		
							WHEN CA.ID IS NULL THEN 1
							WHEN ISNULL(CA.CodStatoCartella,'') ='AP' THEN 1
							ELSE 0
						  END
						)	
				AS PermessoModifica,			
				CONVERT(INTEGER,
					@bCancella &																						CASE 
						WHEN ISNULL(TUS.Codice,'')<>''  THEN 1											
						ELSE 0
					END &
					CASE
						WHEN ISNULL(M.Validata, 0)=1 THEN 0
						ELSE 1
					END	
										& CASE		
						WHEN CA.ID IS NULL THEN 1
						WHEN ISNULL(CA.CodStatoCartella,'') ='AP' THEN 1
						ELSE 0
					  END
				)	
				AS PermessoCancella,
				CONVERT(INTEGER,
					CASE 						
						WHEN ISNULL(TUS.Codice,'')<>''  THEN 1
						ELSE 0
					END	
					& ISNULL(M.Validabile,0) 
					& CASE 
						WHEN ISNULL(M.Validata,0)=0 THEN 1
						ELSE 0
					  END
										& CASE		
						WHEN CA.ID IS NULL THEN 1
						WHEN ISNULL(CA.CodStatoCartella,'') ='AP' THEN 1
						ELSE 0
					  END
				)
				AS	PermessoValida,
				CONVERT(INTEGER,
					@bSuperUser
					& ISNULL(M.Validabile,0) 
					& ISNULL(M.Validata,0) 
										& CASE		
						WHEN CA.ID IS NULL THEN 1
						WHEN ISNULL(CA.CodStatoCartella,'') ='AP' THEN 1
						ELSE 0
					  END
				) AS PermessoSvalida,

				CONVERT(INTEGER,
					CASE 						
						WHEN ISNULL(REV.Codice,'') <> ''  THEN 1
						ELSE 0
					END	
					& ISNULL(M.Validabile,0) 
					& CASE 
						WHEN ISNULL(M.Validata,0) = 1 THEN 1
						ELSE 0
					  END
					& 
					  CASE 
						WHEN ISNULL(M.Revisione,0) = 0 THEN 1
						ELSE 0
					  END
										& CASE		
						WHEN CA.ID IS NULL THEN 1
						WHEN ISNULL(CA.CodStatoCartella,'') ='AP' THEN 1
						ELSE 0
					  END
				) 
				AS PermessoRevisione,
				M.CodStatoSchedaCalcolato,
				SC.Descrizione AS DescrizioneStatoCalcolato,				
				SC.Colore AS ColoreStatoCalcolato,
				CONVERT(INTEGER,ISNULL(S.Contenitore,0)) AS Contenitore,
				M.CodUA,
			    ISNULL(M.DataUltimaModifica,M.DataCreazione) AS DataRiferimentoConsenso,
				
				CONVERT(INTEGER,ISNULL(S.FirmaDigitale,0)) AS FirmaDigitale,						SF.IDDocumentoFirmato,																M.DataCreazione,
				M.IDCartellaAmbulatoriale,															CA.CodStatoCartella AS CodStatoCartellaAmbulatoriale,							
								CASE		
					WHEN CA.ID IS NULL THEN 0
					WHEN ISNULL(CA.CodStatoCartella,'') ='AP' THEN 1
					ELSE 0
				END	AS PermessoChiusuraCartellaAmbulatoriale	
				INTO #tmp1SchedePaziente			 
			FROM 
				T_MovSchede M  WITH (NOLOCK) 		
					LEFT JOIN T_MovCartelleAmbulatoriali CA WITH (NOLOCK)
						ON M.IDCartellaAmbulatoriale = CA.ID

					LEFT JOIN T_Schede S  WITH (NOLOCK)  ON
						M.CodScheda=S.Codice	
					
										LEFT JOIN T_StatoSchedaCalcolato SC
						ON (M.CodScheda=SC.CodScheda AND
							M.CodStatoSchedaCalcolato=SC.Codice)

										LEFT JOIN #tmpSchedeAbilitate TUS  WITH (NOLOCK)  ON
						M.CodScheda=TUS.Codice				
					
										LEFT JOIN #tmpSchedeRiservateVisualizza VSR  WITH (NOLOCK)  ON
						M.CodScheda=VSR.Codice
					
										LEFT JOIN #tmpSchedeRevisione REV  WITH (NOLOCK)  ON
						M.CodScheda=REV.Codice

										LEFT JOIN #tmpIDFiltroSchede SG
						ON M.ID=SG.IDScheda
					
										INNER JOIN #tmpPazientiAlias PA WITH (NOLOCK)
						ON IDEntita=PA.IDPaziente
						
										LEFT JOIN Q_SelUltimoDocumentoSchedaFirmata SF  WITH (NOLOCK)  ON
						M.ID = SF.IDScheda
			WHERE							
				M.CodEntita='PAZ'
				AND	M.Storicizzata=0 
				AND M.CodStatoScheda <> 'CA'											
								AND
					ISNULL(S.CodTipoScheda,'') =CASE		

										WHEN @sCodTipoScheda='' THEN ISNULL(S.CodTipoScheda,'')
										ELSE @sCodTipoScheda
									 END	

								AND
					1=CASE		
						WHEN @sCodScheda='' THEN 1
						ELSE 
							CASE 
								WHEN SG.IDScheda IS NULL THEN 0
								ELSE 1
							END		
					  END	
																															
								AND 
					1=(CASE 
							WHEN @bDaCompletare=0 THEN 1												WHEN M.ID IS NULL THEN 1													ELSE																				(CASE 
										WHEN ISNULL(M.DatiObbligatoriMancantiRTF,'')='' THEN 0												ELSE 1																							END)	
						 END)		
				AND	 
								1=CASE 
					WHEN @bSoloSchedeEpisodio=1 THEN 0
					ELSE 1
				  END
				  
				
								AND
				1=CASE
					WHEN ISNULL(M.Riservata,0)=0 THEN 1															WHEN ISNULL(M.Riservata,0)=1 AND ISNULL(VSR.Codice,'') = '' THEN 0							WHEN ISNULL(M.Riservata,0)=1 AND ISNULL(VSR.Codice,'') <> '' THEN 1												ELSE 0
				  END	  	
				  
				  				  AND
							1=(CASE	
									WHEN @nStatoEpisodio=0 THEN 1												WHEN @nStatoEpisodio=1 THEN																					(CASE 
																		WHEN ISNULL(CA.CodStatoCartella,'') = 'CH' THEN 0
																		ELSE 1
																	 END)
									WHEN @nStatoEpisodio=2 THEN 																			(CASE 
																		WHEN ISNULL(CA.CodStatoCartella,'CH') = 'CH' THEN 1
																		ELSE 0
																	 END)
									WHEN @nStatoEpisodio=3 THEN 1												ELSE 1																   END	)	 			 																														
			ORDER BY Ordine ASC,
					 DataCreazione ASC
	
		
	

		CREATE TABLE #tmpSchedeCartelleAmbulatoriali
	(IDScheda UNIQUEIDENTIFIER)

	
	CREATE TABLE #tmpSchedePazienteNonModificabili
	(IDScheda UNIQUEIDENTIFIER)

		INSERT INTO #tmpSchedeCartelleAmbulatoriali(IDScheda)
	SELECT DISTINCT IDScheda 
	FROM #tmp1SchedePaziente SP
			INNER JOIN T_MovCartelleAmbulatoriali CA
				ON (SP.IDCartellaAmbulatoriale = CA.ID AND
					CA.CodStatoCartella='CH')

	DECLARE @nCountCartelleAmbulatoriali AS INTEGER	

	SET @nCountCartelleAmbulatoriali= (SELECT COUNT(*) FROM #tmpSchedeCartelleAmbulatoriali)

	IF (@nCountCartelleAmbulatoriali > 0 )
	BEGIN
				
		DECLARE @uIDSchedaAmb UNIQUEIDENTIFIER 
		DECLARE curAmb CURSOR  
		FOR SELECT IDScheda FROM #tmpSchedeCartelleAmbulatoriali

		OPEN curAmb  
		FETCH NEXT FROM curAmb 
		INTO @uIDSchedaAmb

		WHILE @@FETCH_STATUS = 0  
		BEGIN  
			;
			WITH GerarchiaSchede(IDSchedaPadre,IDScheda)
			AS
			( SELECT 			
					IDSchedaPadre,
					IDScheda	
			  FROM #tmp1SchedePaziente UAP	  
			  WHERE (IDSchedaPadre = @uIDSchedaAmb)		
			  				
			  UNION ALL
			  SELECT 			
					G.IDSchedaPadre,
					G.IDScheda
			  FROM #tmp1SchedePaziente UAF
				INNER JOIN GerarchiaSchede G
					ON UAF.IDSchedaPadre=G.IDScheda		
			)		
			INSERT INTO #tmpSchedePazienteNonModificabili
			SELECT
			  IDScheda
			FROM  GerarchiaSchede 	

			FETCH NEXT FROM curAmb 
			INTO @uIDSchedaAmb
		END

				UPDATE #tmp1SchedePaziente
		SET PermessoModifica=0,PermessoRevisione=0,PermessoValida=0,PermessoSvalida=0,PermessoInserisci=0,PermessoCancella=0,PermessoChiusuraCartellaAmbulatoriale=0
		WHERE IDScheda IN (SELECT IDScheda FROM #tmpSchedePazienteNonModificabili)
	END

					
				
		SELECT * INTO #tmp2AppuntamentiSchedePaziente FROM 
		(	
												SELECT DISTINCT 
							
				'APP' AS CodEntita,
				APP.ID AS IDEntita,
				AGE.CodAgenda,
				A.Descrizione AS Agenda,
				APP.ID AS IDAppuntamento,
									(CASE 
						WHEN DataInizio IS NOT NULL THEN
							CONVERT(VARCHAR(10),APP.DataInizio,105) + 
									' ' + CONVERT(VARCHAR(5), APP.DataInizio,14) 
						ELSE ''
					END)
					+ ' - ' +
					(CASE	
						WHEN DataFine IS NOT NULL THEN				
								CONVERT(VARCHAR(10),APP.DataFine,105) + 
									' ' + CONVERT(VARCHAR(5), APP.DataFine,14) + ' ' 
						ELSE ''
					 END)				
									AS  Appuntamento,	
				NULL AS IDScheda,
				NULL AS IDSchedaPadre,
				
								NULL AS IDRaggruppamento,
				NULL AS DescrizioneRaggruppamento,
				NULL AS CodScheda,
				NULL As Descrizione,
				NULL AS DatiMancanti,
				CONVERT(INTEGER, 0) AS InEvidenza,
				CONVERT(INTEGER, 0) AS Validata,												CONVERT(INTEGER, 0) AS Riservata,												APP.DataInizio,
				@sOrdineFittizio AS Ordine,														@dDataCreazioneFittizia AS DataCreazione,														CONVERT(INTEGER,@bInserisci) AS 	PermessoInserisci, 												CONVERT(INTEGER,0) AS PermessoModifica,															CONVERT(INTEGER,0) AS PermessoCancella,															CONVERT(INTEGER,0) AS PermessoValida,															CONVERT(INTEGER,0) AS PermessoSvalida,															CONVERT(INTEGER,0) AS PermessoRevisione,														M.CodStatoSchedaCalcolato,
				SC.Descrizione AS DescrizioneStatoCalcolato,				
				SC.Colore AS ColoreStatoCalcolato,
				CONVERT(INTEGER,0) AS Contenitore,
				APP.CodUA,
				ISNULL(APP.DataUltimaModifica,APP.DataEvento) AS DataRiferimentoConsenso,
				CONVERT(INTEGER,0) AS FirmaDigitale,															NULL AS IDDocumentoFirmato,																		NULL AS IDCartellaAmbulatoriale,																NULL AS CodStatoCartellaAmbulatoriale,															0 AS PermessoChiusuraCartellaAmbulatoriale													FROM 
				T_MovAppuntamenti APP  WITH (NOLOCK) 				
					INNER JOIN 
						T_MovAppuntamentiAgende AGE  WITH (NOLOCK) 
							ON AGE.IDAppuntamento=APP.ID
					LEFT JOIN T_Agende A  WITH (NOLOCK) 
							ON AGE.CodAgenda=A.Codice
					LEFT JOIN T_MovSchede M  WITH (NOLOCK) 
							ON (M.CodEntita='APP' AND
								M.IDEntita=APP.ID AND
								M.Storicizzata=0 AND
								M.CodStatoScheda <> 'CA'										)		
					LEFT JOIN T_Schede S  WITH (NOLOCK) 
							ON 	M.CodScheda=S.Codice	
							
										LEFT JOIN T_StatoSchedaCalcolato SC
						ON (M.CodStatoSchedaCalcolato=SC.Codice AND
						    M.CodScheda=SC.CodScheda)
																							
										LEFT JOIN #tmpIDFiltroSchede SG
						ON M.ID=SG.IDScheda
						
										INNER JOIN #tmpPazientiAlias PA WITH (NOLOCK)
						ON APP.IDPaziente=PA.IDPaziente								
																													  	
			WHERE													
				APP.IDEpisodio IS NULL									
								AND
					ISNULL(S.CodTipoScheda,'')=CASE		
										WHEN @sCodTipoScheda='' THEN ISNULL(S.CodTipoScheda,'')
										ELSE @sCodTipoScheda
									END	
											
								AND
					1=CASE		
						WHEN @sCodScheda='' THEN 1
						ELSE 
							CASE 
								WHEN SG.IDScheda IS NULL THEN 0
								ELSE 1
							END		
					  END	
															
								AND					
					ISNULL(AGE.CodAgenda,'')=CASE		
										WHEN @sCodAgenda='' THEN ISNULL(AGE.CodAgenda,'')
										ELSE @sCodAgenda
									END	

								AND 
					1=(CASE 
							WHEN @bDaCompletare=0 THEN 1												WHEN M.ID IS NULL THEN 1													ELSE																				(CASE 
										WHEN ISNULL(M.DatiObbligatoriMancantiRTF,'') ='' THEN 0												ELSE 1																						END)	
						 END)	
				
								AND
					1=(CASE	
							WHEN @nStatoAppuntamento=0 THEN 1										WHEN @nStatoAppuntamento=1 THEN																			(CASE 
																WHEN APP.CodStatoAppuntamento IN ('IC','PR') THEN 1
																ELSE 0
															END)
							WHEN @nStatoAppuntamento=2 THEN 																		(CASE 
																WHEN APP.CodStatoAppuntamento IN ('ER') THEN 1
																ELSE 0
															END)
							ELSE 1															   END	)

								
								AND
					1=(CASE
							WHEN @dDataInizio IS NULL THEN 1										WHEN @dDataInizio IS NOT NULL THEN 											(CASE 
									WHEN APP.DataInizio <= @dDataInizio AND APP.DataFine >= @dDataFine THEN 1																																																															      
									WHEN APP.DataFine >= @dDataInizio AND APP.DataFine <= @dDataFine THEN 1																																																														
									WHEN APP.DataInizio >= @dDataInizio AND APP.DataInizio <= @dDataFine THEN 1																																										ELSE 0
								 END	
								)
							ELSE 0	
					   END)
					   
				AND	   
								APP.ID=CASE 
						WHEN @uIDEpisodio IS NOT NULL THEN @uIDEpisodio
						ELSE APP.ID
					 END
				AND	 		
								1=CASE 
					WHEN @bSoloSchedeEpisodio=1 OR @bSoloSchedePaziente=1 THEN 0
					ELSE 1
				  END		   
				AND		   
								APP.CodStatoAppuntamento NOT IN ('CA','AN')	
				AND 				AGE.CodStatoAppuntamentoAgenda NOT IN ('CA','AN')		
		UNION 			
								SELECT					
				'APP' AS CodEntita,
				APP.ID AS IDEntita,
				AGE.CodAgenda,
				A.Descrizione AS Agenda,
				APP.ID AS IDAppuntamento,
									(CASE 
						WHEN DataInizio IS NOT NULL THEN
							CONVERT(VARCHAR(10),APP.DataInizio,105) + 
									' ' + CONVERT(VARCHAR(5), APP.DataInizio,14) 
						ELSE ''
					END)
					+ ' - ' +
					(CASE	
						WHEN DataFine IS NOT NULL THEN				
								CONVERT(VARCHAR(10),APP.DataFine,105) + 
									' ' + CONVERT(VARCHAR(5), APP.DataFine,14) + ' ' 
						ELSE ''
					 END)				
									AS  Appuntamento,	
				M.ID AS IDScheda,
				M.IDSchedaPadre,
				
				
																																												
				NULL AS IDRaggruppamento,
				NULL AS DescrizioneRaggruppamento,
				
				M.CodScheda,
								S.Descrizione 
				+
								CASE	
					WHEN ISNULL(S.NumerositaMassima,0) > 1	THEN ' (' + CONVERT(varchar(20),M.Numero) + ')'
					ELSE '' 
				END
				+ ' [' +
				(CASE 
						WHEN DataCreazione IS NOT NULL THEN
							CONVERT(VARCHAR(10),M.DataCreazione,105) + 
									' ' + CONVERT(VARCHAR(5), M.DataCreazione,14) 
						ELSE ''
				END)
				+ ']' 
				AS Descrizione,
				
				CASE 
					WHEN M.ID IS NOT NULL AND ISNULL(M.DatiObbligatoriMancantiRTF,'') <> '' THEN 1
					ELSE 0
				END AS DatiMancanti,
				CONVERT(INTEGER, ISNULL(M.InEvidenza, 0)) AS InEvidenza,
				CONVERT(INTEGER, ISNULL(M.Validata, 0)) AS Validata,							CONVERT(INTEGER, ISNULL(M.Riservata, 0)) AS Riservata,							APP.DataInizio,				
				ISNULL(S.Ordine,'~~~~') AS Ordine,												M.DataCreazione,																
								CONVERT(INTEGER,@bInserisci) AS 	PermessoInserisci, 																				
				CONVERT(INTEGER,
					@bModifica &																			CASE 						
							WHEN ISNULL(TUS.Codice,'')<>''  THEN 1
							ELSE 0
						END	&
						CASE
							WHEN ISNULL(M.Validata, 0)=1 THEN 0
							ELSE 1
						END	
				) AS PermessoModifica,	
						
				CONVERT(INTEGER,
					@bCancella &																		CASE 						
						WHEN ISNULL(TUS.Codice,'')<>''  THEN 1
						ELSE 0
					END	&
					CASE
						WHEN ISNULL(M.Validata, 0)=1 THEN 0
						ELSE 1
					END	
				)	
				AS PermessoCancella,
				CONVERT(INTEGER,
					CASE 						
						WHEN ISNULL(TUS.Codice,'')<>''  THEN 1
						ELSE 0
					END	
					& ISNULL(M.Validabile,0) 
					& CASE 
						WHEN ISNULL(M.Validata,0)=0 THEN 1
						ELSE 0
					  END
				)
				AS	PermessoValida,
				CONVERT(INTEGER,
					 @bSuperUser
					& ISNULL(M.Validabile,0) 
					& ISNULL(M.Validata,0) 
				) AS PermessoSvalida,
				
				CONVERT(INTEGER,
					CASE 						
						WHEN ISNULL(REV.Codice,'') <> ''  THEN 1
						ELSE 0
					END	
					& ISNULL(M.Validabile,0) 
					& CASE 
						WHEN ISNULL(M.Validata,0) = 1 THEN 1
						ELSE 0
					  END
					& 
					  CASE 
						WHEN ISNULL(M.Revisione,0) = 0 THEN 1
						ELSE 0
					  END
				) AS PermessoRevisione,
				M.CodStatoSchedaCalcolato,
				SC.Descrizione AS DescrizioneStatoCalcolato,				
				SC.Colore AS ColoreStatoCalcolato,
				CONVERT(INTEGER,0) AS Contenitore,
				APP.CodUA,
				ISNULL(APP.DataUltimaModifica,APP.DataEvento) AS DataRiferimentoConsenso,
				CONVERT(INTEGER,0) AS FirmaDigitale,															NULL AS IDDocumentoFirmato,																		NULL AS IDCartellaAmbulatoriale,																NULL AS CodStatoCartellaAmbulatoriale,															0 AS PermessoChiusuraCartellaAmbulatoriale													FROM 
				T_MovAppuntamenti APP  WITH (NOLOCK) 				
					INNER JOIN 
						T_MovAppuntamentiAgende AGE  WITH (NOLOCK) 
							ON AGE.IDAppuntamento=APP.ID
					LEFT JOIN T_Agende A  WITH (NOLOCK) 
							ON AGE.CodAgenda=A.Codice
					INNER JOIN T_MovSchede M  WITH (NOLOCK) 												ON (M.CodEntita='APP' AND
								M.IDEntita=APP.ID AND
								M.Storicizzata=0 AND
								M.CodStatoScheda <> 'CA'										)		
					LEFT JOIN T_Schede S  WITH (NOLOCK) 
							ON 	M.CodScheda=S.Codice					
					
										LEFT JOIN T_StatoSchedaCalcolato SC
						ON (M.CodStatoSchedaCalcolato=SC.Codice AND
							M.CodScheda=SC.CodScheda)

										LEFT JOIN #tmpSchedeAbilitate TUS WITH (NOLOCK) ON
						M.CodScheda=TUS.Codice
   
   										LEFT JOIN #tmpSchedeRevisione REV  WITH (NOLOCK)  ON
						M.CodScheda=REV.Codice

										LEFT JOIN #tmpIDFiltroSchede SG  WITH (NOLOCK) 
							ON M.ID=SG.IDScheda
					
										INNER JOIN #tmpPazientiAlias PA WITH (NOLOCK)
						ON APP.IDPaziente=PA.IDPaziente																				  	
			WHERE	
								APP.CodStatoAppuntamento NOT IN ('CA','AN') AND
								AGE.CodStatoAppuntamentoAgenda NOT IN ('CA','AN') AND
				
				APP.IDEpisodio IS NULL									
								AND
					ISNULL(S.CodTipoScheda,'')=CASE		

										WHEN @sCodTipoScheda='' THEN ISNULL(S.CodTipoScheda,'')
										ELSE @sCodTipoScheda
									END	
		
								AND
					1=CASE		
						WHEN @sCodScheda='' THEN 1
						ELSE 
							CASE 
								WHEN SG.IDScheda IS NULL THEN 0
								ELSE 1
							END		
					  END	
															
								AND					
					ISNULL(AGE.CodAgenda,'')=CASE		
										WHEN @sCodAgenda='' THEN ISNULL(AGE.CodAgenda,'')
										ELSE @sCodAgenda
									END	

								AND 
					1=(CASE 
							WHEN @bDaCompletare=0 THEN 1												WHEN M.ID IS NULL THEN 1													ELSE																				(CASE 
										WHEN ISNULL(M.DatiObbligatoriMancantiRTF,'') ='' THEN 0												ELSE 1																						END)	
						 END)	
				
								AND
					1=(CASE	
							WHEN @nStatoAppuntamento=0 THEN 1										WHEN @nStatoAppuntamento=1 THEN																			(CASE 
																WHEN APP.CodStatoAppuntamento IN ('IC','PR') THEN 1
																ELSE 0
															END)
							WHEN @nStatoAppuntamento=2 THEN 																		(CASE 
																WHEN APP.CodStatoAppuntamento IN ('ER') THEN 1
																ELSE 0
															END)
							ELSE 1															   END	)

								
								AND
					1=(CASE
							WHEN @dDataInizio IS NULL THEN 1										WHEN @dDataInizio IS NOT NULL THEN 											(CASE 
									WHEN APP.DataInizio <= @dDataInizio AND APP.DataFine >= @dDataFine THEN 1																																																															      
									WHEN APP.DataFine >= @dDataInizio AND APP.DataFine <= @dDataFine THEN 1																																																														
									WHEN APP.DataInizio >= @dDataInizio AND APP.DataInizio <= @dDataFine THEN 1																																										ELSE 0
								 END	
								)
							ELSE 0	
					   END)
								
				AND	 
								1=CASE 
					WHEN @bSoloSchedeEpisodio=1 OR @bSoloSchedePaziente=1 THEN 0
					ELSE 1
				  END		   
								   
			) AS Q		
			ORDER BY 
				Agenda,									DataInizio DESC,						Ordine	   ASC,							DataCreazione ASC				
				
					 
				
		SELECT * INTO #tmp3EpisodiSchede FROM 
			(
																SELECT DISTINCT
						'EPI' AS CodEntita,
						E.ID AS IDEntita,
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
									WHEN DataListaAttesa IS NOT NULL THEN 
											CHAR(10)+ CHAR(13) + 'Data Lista Attesa : ' + CONVERT(VARCHAR(10),E.DataListaAttesa,105) + 
																	' ' + CONVERT(VARCHAR(5), E.DataListaAttesa,14)  
																 + ' (' + ISNULL(ALP.Descrizione,'') + ')'
									ELSE ''			
								END)
						 +
								(CASE	
									WHEN E.DataAnnullamentoListaAttesa IS NOT NULL THEN
										CHAR(10) + CHAR(13)  
																+ 'Annullata il : ' + CONVERT(VARCHAR(10),E.DataAnnullamentoListaAttesa,105) 
																+	' ' + CONVERT(VARCHAR(5), E.DataAnnullamentoListaAttesa,14) 																
									ELSE ''
								END )

							+				
								(CASE 
										WHEN E.DataAnnullamentoListaAttesa IS NULL AND E.DataDimissione IS NULL AND QT.DataUscitaUltimo IS NOT NULL THEN 
														CHAR(10) + CHAR(13)  
																+ 'Data Ultimo Trasf. : ' + CONVERT(VARCHAR(10),QT.DataUscitaUltimo,105) 
																+	' ' + CONVERT(VARCHAR(5), QT.DataUscitaUltimo,14) 
																+	' (' +  ISNULL(AU.Descrizione,'') + ')'
																
										WHEN E.DataAnnullamentoListaAttesa IS NULL AND E.DataDimissione IS NOT NULL THEN
															CHAR(10) + CHAR(13) 
																 + 'Data Dimissione : ' + CONVERT(VARCHAR(10),E.DataDimissione,105)
																 +  ' ' + CONVERT(VARCHAR(5), E.DataDimissione,14)  +
																 + ' (' + ISNULL(AU.Descrizione,'') + ')'
										ELSE ''							
								END)
							As Ricovero,
							NULL AS IDScheda,
							NULL AS IDSchedaPadre,				
						
						
														NULL AS IDRaggruppamento,
							NULL AS DescrizioneRaggruppamento,
							
							NULL AS CodScheda,				
														NULL AS  Descrizione,
						
							Case
								WHEN E.DataDimissione IS NOT NULL THEN 1
								WHEN E.DataAnnullamentoListaAttesa IS NOT NULL THEN 2
								ELSE 0
							END AS Dimesso,
							NULL AS DatiMancanti,
							CONVERT(INTEGER, 0) AS InEvidenza,
							CONVERT(INTEGER, 0) AS Validata,												CONVERT(INTEGER, 0) AS Riservata,												E.DataRicovero,
							@sOrdineFittizio AS Ordine,														@dDataCreazioneFittizia AS DataCreazione,																	CONVERT(INTEGER,
								@bInserisci &																		CASE																				WHEN E.ID=@uIDEpisodioCorrente THEN 1
									ELSE 0
								END 																															
							)
							AS PermessoInserisci,							
							CONVERT(INTEGER,0) AS PermessoModifica,															CONVERT(INTEGER,0) AS PermessoCancella,															CONVERT(INTEGER,0) AS PermessoValida,															CONVERT(INTEGER,0) AS PermessoSvalida,															CONVERT(INTEGER,0) AS PermessoRevisione,														CONVERT(DATETIME,'2100-01-01 00:00') AS DataAperturaCartella,
							M.CodStatoSchedaCalcolato,
							SC.Descrizione AS DescrizioneStatoCalcolato,				
							SC.Colore AS ColoreStatoCalcolato,		
							CONVERT(INTEGER,0) AS Contenitore,
							NULL AS CodUA,

														CASE
								WHEN E.DataDimissione IS NOT NULL THEN E.DataDimissione												WHEN E.DataDimissione IS NULL AND E.DataRicovero IS NOT NULL THEN CONVERT(DATETIME,'2100-01-01',120) 								WHEN E.DataListaAttesa IS NOT NULL THEN E.DataListaAttesa											ELSE CONVERT(DATETIME,'2100-01-01',120)															END AS DataRiferimentoConsenso,

							CONVERT(INTEGER,0)	AS FirmaDigitale,																NULL AS	IDDocumentoFirmato,																			NULL AS IDCartellaAmbulatoriale,																	NULL AS CodStatoCartellaAmbulatoriale,																0 AS PermessoChiusuraCartellaAmbulatoriale													FROM 			
						T_MovEpisodi AS E  WITH (NOLOCK) 
							INNER JOIN T_MovPazienti P  WITH (NOLOCK) 
								ON P.IDEpisodio=E.ID
							LEFT JOIN T_TipoEpisodio TE  WITH (NOLOCK) 
								ON E.CodTipoEpisodio =TE.Codice	
								
							LEFT JOIN Q_SelEpisodioRicoveroPU QT  WITH (NOLOCK) 
								ON E.ID=QT.IDEpisodio

							LEFT JOIN T_UnitaAtomiche AP  WITH (NOLOCK) 
								ON QT.CodUAPrimo=AP.Codice	

							LEFT JOIN Q_SelEpisodioListaAttesaPU QTL  WITH (NOLOCK) 
								ON E.ID=QTL.IDEpisodio
							
							LEFT JOIN T_UnitaAtomiche ALP  WITH (NOLOCK) 
								ON QTL.CodUAPrimo=ALP.Codice	
							
								
							LEFT JOIN T_UnitaAtomiche AU  WITH (NOLOCK) 
								ON QT.CodUAUltimo=AU.Codice	
								
							LEFT JOIN T_MovSchede M  WITH (NOLOCK) 
										ON (M.CodEntita='EPI' AND
											M.IDEntita=E.ID AND
											M.Storicizzata=0 AND
											M.CodStatoScheda <> 'CA'													)		
							LEFT JOIN 
									T_Schede S  WITH (NOLOCK) 
									ON 	M.CodScheda=S.Codice	

														LEFT JOIN T_StatoSchedaCalcolato SC
								ON (M.CodStatoSchedaCalcolato=SC.Codice AND
									M.CodScheda=SC.CodScheda)


														LEFT JOIN #tmpIDFiltroSchede SG  WITH (NOLOCK) 
									ON M.ID=SG.IDScheda
							
														INNER JOIN #tmpPazientiAlias PA WITH (NOLOCK)
								ON P.IDPaziente=PA.IDPaziente				
																																																							
					WHERE 										
												E.CodStatoEpisodio NOT IN ('CA') AND

												1=CASE 
							WHEN NumeroListaAttesa IS NULL AND E.CodStatoEpisodio = 'AN' THEN 0
							ELSE  1
						   END						
						
												AND
								ISNULL(S.CodTipoScheda,'')=CASE		
													WHEN @sCodTipoScheda='' THEN ISNULL(S.CodTipoScheda,'')
													ELSE @sCodTipoScheda
												END			
											AND
						1=CASE		
							WHEN @sCodScheda='' THEN 1
							ELSE 
								CASE 
									WHEN SG.IDScheda IS NULL THEN 0
									ELSE 1
								END		
						  END										
																									
												AND 
							1=(CASE 
									WHEN @bDaCompletare=0 THEN 1														WHEN M.ID IS NULL THEN 1															ELSE																						(CASE 
												WHEN ISNULL(M.DatiObbligatoriMancantiRTF,'')='' THEN 0														ELSE 1																								END)	
								 END)				
						
																		AND
							1=(CASE	
									WHEN @nStatoEpisodio=0 THEN 1												WHEN @nStatoEpisodio=1 THEN																					(CASE 
																		WHEN E.DataDimissione IS NULL AND E.DataAnnullamentoListaAttesa IS NULL THEN 1				 																		ELSE 0
																	END)
									WHEN @nStatoEpisodio=2 THEN 																				(CASE 
																		WHEN (E.DataDimissione IS NOT NULL OR E.DataAnnullamentoListaAttesa IS NOT NULL) THEN 1		 																		ELSE 0
																	END)
									WHEN @nStatoEpisodio=3 THEN 0												ELSE 1																   END	)					

													AND
								1=(CASE
										WHEN @dDataInizio IS NULL THEN 1													WHEN @dDataInizio IS NOT NULL AND E.DataDimissione IS NOT NULL THEN 														(CASE 
												
												WHEN E.DataRicovero <= @dDataInizio AND E.DataDimissione>= @dDataFine THEN 1																																																																						      
												WHEN E.DataRicovero >= @dDataInizio AND E.DataDimissione <= @dDataFine THEN 1																																																																				
												WHEN E.DataRicovero >= @dDataInizio AND E.DataDimissione <= @dDataFine THEN 1																																																	ELSE 0
											 END	
											)
										WHEN @dDataInizio IS NOT NULL AND E.DataDimissione IS  NULL THEN 														(CASE 									
												WHEN E.DataRicovero <= @dDataInizio  THEN 1																																																										ELSE 0
											 END	
											)	
										ELSE 0	
								   END)
						AND	   
												E.ID=CASE 
								WHEN @uIDEpisodio IS NOT NULL THEN @uIDEpisodio
								ELSE E.ID
							 END
						AND	 
												1=CASE 
							WHEN @bSoloSchedePaziente=1 THEN 0
							ELSE 1
						  END			   
			UNION 
																SELECT 
						'EPI' AS CodEntita,
						E.ID AS IDEntita,
						E.ID AS IDEpisodio,	
						
												ISNULL(TE.Descrizione,'') + 
							(CASE 
								WHEN ISNULL(E.NumeroNosologico,'')='' THEN ''
								ELSE  ' (' + ISNULL(E.NumeroNosologico,'') + ')' 
							END)	
							+ 
								(CASE 
									WHEN DataRicovero IS NOT NULL THEN 
											CHAR(10)+ CHAR(13) + 'Data Accettazione : ' + CONVERT(VARCHAR(10),E.DataRicovero,105) + 
																	' ' + CONVERT(VARCHAR(5), E.DataRicovero,14)  
																 + ' (' + ISNULL(AP.Descrizione,'') + ')'
									WHEN DataListaAttesa IS NOT NULL THEN 
											CHAR(10)+ CHAR(13) + 'Data Lista Attesa : ' + CONVERT(VARCHAR(10),E.DataListaAttesa,105) + 
																	' ' + CONVERT(VARCHAR(5), E.DataListaAttesa,14)  
																 + ' (' + ISNULL(ALP.Descrizione,'') + ')'
									ELSE ''			
								END)
							+
								(CASE	
									WHEN E.DataAnnullamentoListaAttesa IS NOT NULL THEN
										CHAR(10) + CHAR(13)  
																+ 'Annullata il : ' + CONVERT(VARCHAR(10),E.DataAnnullamentoListaAttesa,105) 
																+	' ' + CONVERT(VARCHAR(5), E.DataAnnullamentoListaAttesa,14) 																
									ELSE ''
								END )
							+				
								(CASE 
										WHEN E.DataAnnullamentoListaAttesa IS NULL AND E.DataDimissione IS NULL AND QT.DataUscitaUltimo IS NOT NULL THEN 
														CHAR(10) + CHAR(13)  
																+ 'Data Ultimo Trasf. : ' + CONVERT(VARCHAR(10),QT.DataUscitaUltimo,105) 
																+	' ' + CONVERT(VARCHAR(5), QT.DataUscitaUltimo,14) 
																+	' (' +  ISNULL(AU.Descrizione,'') + ')'
																
										WHEN E.DataAnnullamentoListaAttesa IS NULL AND E.DataDimissione IS NOT NULL THEN
															CHAR(10) + CHAR(13) 
																 + 'Data Dimissione : ' + CONVERT(VARCHAR(10),E.DataDimissione,105)
																 +  ' ' + CONVERT(VARCHAR(5), E.DataDimissione,14)  +
																 + ' (' + ISNULL(AU.Descrizione,'') + ')'
										ELSE ''							
								END)
							As Ricovero,
							M.ID AS IDScheda,
							M.IDSchedaPadre,				
						
																																																																						

							NULL AS IDRaggruppamento,
							NULL AS DescrizioneRaggruppamento,
							
							M.CodScheda,
														S.Descrizione  +
														CASE	
								WHEN ISNULL(S.NumerositaMassima,0) > 1	THEN ' (' + CONVERT(varchar(20),M.Numero) + ')'
								ELSE '' 
							END	
							+ ' [' +														
							(CASE 
									WHEN DataCreazione IS NOT NULL THEN
										CONVERT(VARCHAR(10),M.DataCreazione,105) + 
												' ' + CONVERT(VARCHAR(5), M.DataCreazione,14) 
									ELSE ''
							END)			 
							+ ']'
							AS Descrizione,
						
							Case
								WHEN E.DataDimissione IS NOT NULL THEN 1
								WHEN E.DataAnnullamentoListaAttesa IS NOT NULL THEN 2
								ELSE 0
							END AS Dimesso,
							
							CASE 
								WHEN M.ID IS NOT NULL AND ISNULL(M.DatiObbligatoriMancantiRTF,'') <> '' THEN 1
								ELSE 0
							END AS DatiMancanti,
							CONVERT(INTEGER, ISNULL(M.InEvidenza, 0)) AS InEvidenza,
							CONVERT(INTEGER, ISNULL(M.Validata, 0)) AS Validata,
							CONVERT(INTEGER, ISNULL(M.Riservata, 0)) AS Riservata,
							E.DataRicovero,
							ISNULL(S.Ordine,'~~~~') AS Ordine,														M.DataCreazione,																									CONVERT(INTEGER,
								@bInserisci	&							 													CASE																						WHEN E.ID=@uIDEpisodioCorrente THEN 1
									ELSE 0
								END 								
								)
							AS 	PermessoInserisci,							
							CONVERT(INTEGER,
								@bModifica &																				CASE																						WHEN E.ID=@uIDEpisodioCorrente THEN 1
									ELSE 0
								END 
								&																						CASE 						
									WHEN ISNULL(TUS.Codice,'')<>''  THEN 1
									ELSE 0
								END	&
								CASE
									WHEN ISNULL(M.Validata, 0)=1 THEN 0
									ELSE 1
								END	
							)
							AS PermessoModifica,							
							CONVERT(INTEGER,
								@bCancella & 																				CASE																						WHEN E.ID=@uIDEpisodioCorrente THEN 1
									ELSE 0
								END 
								&																						CASE 						
									WHEN ISNULL(TUS.Codice,'')<>''  THEN 1
									ELSE 0
								END	&								
								CASE
									WHEN ISNULL(M.Validata, 0)=1 THEN 0
									ELSE 1
								END	
							)
							AS PermessoCancella,
							CONVERT(INTEGER,
								CASE 						
									WHEN ISNULL(TUS.Codice,'')<>''  THEN 1
									ELSE 0
								END	
								& ISNULL(M.Validabile,0) 
								& CASE 
									WHEN ISNULL(M.Validata,0)=0 THEN 1
									ELSE 0
								  END
							)
							AS	PermessoValida,
							CONVERT(INTEGER,
								@bSuperUser
								& ISNULL(M.Validabile,0) 
								& ISNULL(M.Validata,0) 
							) AS PermessoSvalida,
							CONVERT(INTEGER,
								CASE 						
									WHEN ISNULL(REV.Codice,'') <> ''  THEN 1
									ELSE 0
								END	
								& ISNULL(M.Validabile,0) 
								& CASE 
									WHEN ISNULL(M.Validata,0) = 1 THEN 1
									ELSE 0
								  END
								& 
								   CASE 
									WHEN ISNULL(M.Revisione,0) = 0 THEN 1
									ELSE 0
								   END
								)  AS PermessoRevisione,
							CAR.DataApertura AS DataAperturaCartella,
							M.CodStatoSchedaCalcolato,
							SC.Descrizione AS DescrizioneStatoCalcolato,				
							SC.Colore AS ColoreStatoCalcolato,
							CONVERT(INTEGER,ISNULL(S.Contenitore,0)) AS Contenitore,
							M.CodUA,
														CASE
								WHEN E.DataDimissione IS NOT NULL THEN E.DataDimissione												WHEN E.DataDimissione IS NULL AND E.DataRicovero IS NOT NULL THEN E.DataRicovero 								WHEN E.DataListaAttesa IS NOT NULL THEN E.DataListaAttesa											ELSE '2100-01-01'																				END AS DataRiferimentoConsenso,

							CONVERT(INTEGER,ISNULL(S.FirmaDigitale,0)) AS FirmaDigitale,									SF.IDDocumentoFirmato,																			NULL AS IDCartellaAmbulatoriale,																NULL AS CodStatoCartellaAmbulatoriale,															0 AS PermessoChiusuraCartellaAmbulatoriale												FROM 			
						T_MovEpisodi AS E  WITH (NOLOCK) 
							INNER JOIN T_MovPazienti P  WITH (NOLOCK) 
								ON P.IDEpisodio=E.ID
								
							LEFT JOIN T_TipoEpisodio TE  WITH (NOLOCK) 
								ON E.CodTipoEpisodio =TE.Codice	
								
							LEFT JOIN Q_SelEpisodioRicoveroPU QT  WITH (NOLOCK) 
								ON E.ID=QT.IDEpisodio
							
							LEFT JOIN T_UnitaAtomiche AP  WITH (NOLOCK) 
								ON QT.CodUAPrimo=AP.Codice	
								
							LEFT JOIN Q_SelEpisodioListaAttesaPU QTL  WITH (NOLOCK) 
								ON E.ID=QTL.IDEpisodio
							
							LEFT JOIN T_UnitaAtomiche ALP  WITH (NOLOCK) 
								ON QTL.CodUAPrimo=ALP.Codice	
							
							LEFT JOIN T_UnitaAtomiche AU  WITH (NOLOCK) 
								ON QT.CodUAUltimo=AU.Codice	
								
							INNER JOIN T_MovSchede M  WITH (NOLOCK)													ON (M.CodEntita='EPI' AND
											M.IDEntita=E.ID AND
											M.Storicizzata=0 AND
											M.CodStatoScheda <> 'CA'													)		
							LEFT JOIN 
									T_Schede S 	WITH (NOLOCK) 					
									ON 	M.CodScheda=S.Codice	
								
														LEFT JOIN T_StatoSchedaCalcolato SC
								ON (M.CodStatoSchedaCalcolato=SC.Codice AND
									M.CodScheda =SC.CodScheda)
														
														LEFT JOIN #tmpSchedeAbilitate TUS WITH (NOLOCK) ON
								M.CodScheda=TUS.Codice
							
														LEFT JOIN #tmpSchedeRiservateVisualizza VSR  WITH (NOLOCK)  ON
								M.CodScheda=VSR.Codice

														LEFT JOIN #tmpSchedeRevisione REV  WITH (NOLOCK)  ON
									M.CodScheda=REV.Codice
							
														LEFT JOIN #tmpIDFiltroSchede SG WITH (NOLOCK)
									ON M.ID=SG.IDScheda
							
														INNER JOIN #tmpPazientiAlias PA WITH (NOLOCK)
								ON P.IDPaziente=PA.IDPaziente	
							
							LEFT JOIN T_MovTrasferimenti TRA
								ON M.IDTrasferimento=TRA.ID	
							LEFT JOIN T_MovCartelle CAR
								ON TRA.IDCartella=CAR.ID
								
														LEFT JOIN Q_SelUltimoDocumentoSchedaFirmata SF  WITH (NOLOCK)  ON
								M.ID = SF.IDScheda								
					WHERE 															
												E.CodStatoEpisodio NOT IN ('CA') AND

												1=CASE 
							WHEN NumeroListaAttesa IS NULL AND E.CodStatoEpisodio = 'AN' THEN 0
							ELSE  1
						   END
						
												AND
								ISNULL(S.CodTipoScheda,'') =CASE		
													WHEN @sCodTipoScheda='' THEN ISNULL(S.CodTipoScheda,'')
													ELSE @sCodTipoScheda
												END			
												
													AND
								1=CASE		
									WHEN @sCodScheda='' THEN 1
									ELSE 
										CASE 
											WHEN SG.IDScheda IS NULL THEN 0
											ELSE 1
										END		
								  END	
																										
												AND 
							1=(CASE 
									WHEN @bDaCompletare=0 THEN 1														WHEN M.ID IS NULL THEN 1															ELSE																						(CASE 
												WHEN ISNULL(M.DatiObbligatoriMancantiRTF,'')='' THEN 0														ELSE 1																								END)	
								 END)				
						
																		AND
							1=(CASE	
									WHEN @nStatoEpisodio=0 THEN 1												WHEN @nStatoEpisodio=1 THEN																					(CASE 
																		WHEN E.DataDimissione IS NULL AND E.DataAnnullamentoListaAttesa IS NULL THEN 1				 																		ELSE 0
																	END)
									WHEN @nStatoEpisodio=2 THEN 																				(CASE 
																		WHEN (E.DataDimissione IS NOT NULL OR E.DataAnnullamentoListaAttesa IS NOT NULL) THEN 1		 																		ELSE 0
																	END)
									WHEN @nStatoEpisodio=3 THEN 0												ELSE 1																   END	)					

													AND
								1=(CASE
										WHEN @dDataInizio IS NULL THEN 1													WHEN @dDataInizio IS NOT NULL AND E.DataDimissione IS NOT NULL THEN 														(CASE 
												
												WHEN E.DataRicovero <= @dDataInizio AND E.DataDimissione>= @dDataFine THEN 1																																																																						      
												WHEN E.DataRicovero >= @dDataInizio AND E.DataDimissione <= @dDataFine THEN 1																																																																				
												WHEN E.DataRicovero >= @dDataInizio AND E.DataDimissione <= @dDataFine THEN 1																																																	ELSE 0
											 END	
											)
										WHEN @dDataInizio IS NOT NULL AND E.DataDimissione IS  NULL THEN 														(CASE 									
												WHEN E.DataRicovero <= @dDataInizio  THEN 1																																																										ELSE 0
											 END	
											)	
										ELSE 0	
								   END)
								AND	   
								
												E.ID=CASE 
								WHEN @uIDEpisodio IS NOT NULL THEN @uIDEpisodio
								ELSE E.ID
							 END
						AND	 
												1=CASE 
							WHEN @bSoloSchedePaziente=1 THEN 0
							ELSE 1
						  END
						 
						 						AND
						1=CASE
							WHEN ISNULL(M.Riservata,0)=0 THEN 1																	WHEN ISNULL(M.Riservata,0)=1 AND ISNULL(VSR.Codice,'') = '' THEN 0									WHEN ISNULL(M.Riservata,0)=1 AND ISNULL(VSR.Codice,'') <> '' THEN 1																ELSE 0
						  END	 			   
				) AS Q
		ORDER BY 
				DataRicovero DESC,
				ISNULL(DataAperturaCartella,DataRicovero) DESC,							Ordine ASC,
				DataCreazione ASC							

									   
										
				SELECT	* INTO #tmp4AppuntamentiSchedeEpisodi FROM				
			( SELECT DISTINCT
					'EPI' AS CodEntitaPadre,
					'APP' AS CodEntita,
					APP.ID AS IDEntita,
					APP.IDEpisodio,	
						'Ricoverato il : ' + CONVERT(VARCHAR(10),E.DataRicovero,105) + 
							' ' + CONVERT(VARCHAR(5), E.DataRicovero,14)  As Ricovero,
					AGE.CodAgenda,
					A.Descrizione AS Agenda,
					APP.ID AS IDAppuntamento,				
										(CASE 
							WHEN DataInizio IS NOT NULL THEN
								CONVERT(VARCHAR(10),APP.DataInizio,105) + 
										' ' + CONVERT(VARCHAR(5), APP.DataInizio,14) 
							ELSE ''
						END)
						+ ' - ' +
						(CASE	
							WHEN DataFine IS NOT NULL THEN				
									CONVERT(VARCHAR(10),APP.DataFine,105) + 
										' ' + CONVERT(VARCHAR(5), APP.DataFine,14) + ' ' 
							ELSE ''
						 END)		
					As Appuntamento,					
					NULL AS IDScheda,
					NULL AS IDSchedaPadre,
					
										NULL AS IDRaggruppamento,
					NULL AS DescrizioneRaggruppamento,
				
					NULL AS CodScheda,
					NULL As Descrizione,
					NULL DatiMancanti,	
					CONVERT(INTEGER, 0) AS InEvidenza,					
					CONVERT(INTEGER, 0) AS Validata,
					CONVERT(INTEGER, 0) AS Riservata,
					APP.DataInizio,									
					@sOrdineFittizio AS Ordine,															@dDataCreazioneFittizia AS DataCreazione,											
										CONVERT(INTEGER,
						@bInserisci &																			CASE																					WHEN E.ID=@uIDEpisodioCorrente THEN 1
							ELSE 0
						END 	
					)
					AS 	PermessoInserisci,										
					
					CONVERT(INTEGER,0) AS PermessoModifica,															CONVERT(INTEGER,0) AS PermessoCancella,															CONVERT(INTEGER,0) AS PermessoValida,															CONVERT(INTEGER,0) AS PermessoSvalida,															CONVERT(INTEGER,0) AS PermessoRevisione,														M.CodStatoSchedaCalcolato,
					SC.Descrizione AS DescrizioneStatoCalcolato,				
					SC.Colore AS ColoreStatoCalcolato,
					CONVERT(INTEGER,0) AS Contenitore,
					APP.CodUA,
					ISNULL(APP.DataUltimaModifica,APP.DataEvento) AS DataRiferimentoConsenso,

					CONVERT(INTEGER,0) AS FirmaDigitale,															NULL IDDocumentoFirmato,																		NULL AS IDCartellaAmbulatoriale,																NULL AS CodStatoCartellaAmbulatoriale,															0 AS PermessoChiusuraCartellaAmbulatoriale													FROM 
					T_MovAppuntamenti APP WITH (NOLOCK)				
						INNER JOIN 
							T_MovAppuntamentiAgende AGE WITH (NOLOCK)
								ON AGE.IDAppuntamento=APP.ID
						LEFT JOIN T_Agende A WITH (NOLOCK)
								ON AGE.CodAgenda=A.Codice
						LEFT JOIN T_MovSchede M WITH (NOLOCK)
								ON (M.CodEntita='APP' AND
									M.IDEntita=APP.ID AND
									M.Storicizzata=0 AND
									M.CodStatoScheda <> 'CA'											)		
						LEFT JOIN	
								T_MovEpisodi E WITH (NOLOCK)	
							ON APP.IDEpisodio=E.ID			
						LEFT JOIN 
								T_Schede S WITH (NOLOCK)
								ON 	M.CodScheda=S.Codice					

												LEFT JOIN T_StatoSchedaCalcolato SC
								ON (M.CodStatoSchedaCalcolato=SC.Codice AND
								    M.CodScheda=SC.CodScheda)
											
												LEFT JOIN #tmpIDFiltroSchede SG WITH (NOLOCK)
							ON M.ID=SG.IDScheda
						
												INNER JOIN #tmpPazientiAlias PA WITH (NOLOCK)
								ON APP.IDPaziente=PA.IDPaziente		
				WHERE						
					APP.IDEpisodio IS NOT NULL	AND										 
										E.CodStatoEpisodio NOT IN ('CA') AND

										1=CASE 
						WHEN NumeroListaAttesa IS NULL AND E.CodStatoEpisodio = 'AN' THEN 0
						ELSE  1
					  END
					
					 					AND
						ISNULL(S.CodTipoScheda,'') =CASE		
											WHEN @sCodTipoScheda='' THEN ISNULL(S.CodTipoScheda,'')
											ELSE @sCodTipoScheda
										END	
					
										AND
						1=CASE		
							WHEN @sCodScheda='' THEN 1
							ELSE 
								CASE 
									WHEN SG.IDScheda IS NULL THEN 0
									ELSE 1
								END		
						  END	
						
									
										AND					
						ISNULL(AGE.CodAgenda,'')=CASE		
											WHEN @sCodAgenda='' THEN ISNULL(AGE.CodAgenda,'')
											ELSE @sCodAgenda
									   END	
					
										AND 
						1=(CASE 
								WHEN @bDaCompletare=0 THEN 1													WHEN M.ID IS NULL THEN 1														ELSE																					(CASE 
											WHEN ISNULL(M.DatiObbligatoriMancantiRTF,'')='' THEN 0													ELSE 1																								END)	
							 END)	
					
										AND
						1=(CASE	
								WHEN @nStatoAppuntamento=0 THEN 1											WHEN @nStatoAppuntamento=1 THEN																				(CASE 
																	WHEN APP.CodStatoAppuntamento IN ('IC','PR') THEN 1
																	ELSE 0
																END)
								WHEN @nStatoAppuntamento=2 THEN 																			(CASE 
																	WHEN APP.CodStatoAppuntamento IN ('ER') THEN 1
																	ELSE 0
																END)
								ELSE 1																   END	)
					
										AND
					1=(CASE	
							WHEN @nStatoEpisodio=0 THEN 1										WHEN @nStatoEpisodio=1 THEN																					(CASE 
																		WHEN E.DataDimissione IS NULL AND E.DataAnnullamentoListaAttesa IS NULL THEN 1				 																		ELSE 0
																	END)
									WHEN @nStatoEpisodio=2 THEN 																				(CASE 
																		WHEN (E.DataDimissione IS NOT NULL OR E.DataAnnullamentoListaAttesa IS NOT NULL) THEN 1		 																		ELSE 0
																	END)
							WHEN @nStatoEpisodio=3 THEN 0										ELSE 1														   END	)	
					   
					 AND
						 						E.ID=CASE 
								WHEN @uIDEpisodio IS NOT NULL THEN @uIDEpisodio
								ELSE E.ID
							 END
						AND	 
												1=CASE 
							WHEN @bSoloSchedePaziente=1 THEN 0
							ELSE 1
						  END		  			
					AND
										APP.CodStatoAppuntamento NOT IN ('CA','AN')  						
					AND 					AGE.CodStatoAppuntamentoAgenda NOT IN ('CA','AN')
	   		UNION 
		   		
		   											   								SELECT					
						'EPI' AS CodEntitaPadre,
						'APP' AS CodEntita,
						APP.ID AS IDEntita,
						APP.IDEpisodio,	
							'Ricoverato il : ' + CONVERT(VARCHAR(10),E.DataRicovero,105) + 
								' ' + CONVERT(VARCHAR(5), E.DataRicovero,14)  As Ricovero,
						AGE.CodAgenda,
						A.Descrizione AS Agenda,
						APP.ID AS IDAppuntamento,				
												(CASE 
								WHEN DataInizio IS NOT NULL THEN
									CONVERT(VARCHAR(10),APP.DataInizio,105) + 
											' ' + CONVERT(VARCHAR(5), APP.DataInizio,14) 
								ELSE ''
							END)
							+ ' - ' +
							(CASE	
								WHEN DataFine IS NOT NULL THEN				
										CONVERT(VARCHAR(10),APP.DataFine,105) + 
											' ' + CONVERT(VARCHAR(5), APP.DataFine,14) + ' ' 
								ELSE ''
							 END)		
						As Appuntamento,					
						M.ID AS IDScheda,
						M.IDSchedaPadre,
						
																																																																		
						NULL AS IDRaggruppamento,
						NULL AS DescrizioneRaggruppamento,
						
						M.CodScheda,
												S.Descrizione 	+
													CASE	
							WHEN ISNULL(S.NumerositaMassima,0) > 1	THEN ' (' + CONVERT(varchar(20),M.Numero) + ')'
							ELSE '' 
						END	
						+ ' [' +											
						(CASE 
								WHEN DataCreazione IS NOT NULL THEN
									CONVERT(VARCHAR(10),M.DataCreazione,105) + 
											' ' + CONVERT(VARCHAR(5), M.DataCreazione,14) 
								ELSE ''
						END)		
						+ ']'
						AS Descrizione,
						
						CASE 
							WHEN M.ID IS NOT NULL AND ISNULL(M.DatiObbligatoriMancantiRTF,'') <> '' THEN 1
							ELSE 0
						END AS DatiMancanti,
						CONVERT(INTEGER, ISNULL(M.InEvidenza, 0)) AS InEvidenza,
						CONVERT(INTEGER, ISNULL(M.Validata, 0)) AS Validata,
						CONVERT(INTEGER, ISNULL(M.Riservata, 0)) AS Riservata,
						APP.DataInizio,
						ISNULL(S.Ordine,'~~~~') AS  Ordine,													M.DataCreazione,																	
										CONVERT(INTEGER,
						@bInserisci &																			CASE																						WHEN E.ID=@uIDEpisodioCorrente THEN 1
								ELSE 0
						END 	
					)	
					AS 	PermessoInserisci,	
														
					CONVERT(INTEGER,
						@bModifica &																				CASE																					WHEN E.ID=@uIDEpisodioCorrente THEN 1
								ELSE 0
							END 							
							&																					CASE 						
								WHEN ISNULL(TUS.Codice,'')<>''  THEN 1
								ELSE 0
							END	&
							CASE
								WHEN ISNULL(M.Validata, 0)=1 THEN 0
								ELSE 1
							END	
						)	
					AS PermessoModifica,			
					
					CONVERT(INTEGER,
						@bCancella & 																			CASE																				WHEN E.ID=@uIDEpisodioCorrente THEN 1
								ELSE 0
							END 
							&																				CASE 						
								WHEN ISNULL(TUS.Codice,'')<>''  THEN 1
								ELSE 0
							END	&
							CASE
								WHEN ISNULL(M.Validata, 0)=1 THEN 0
								ELSE 1
							END	
						)	
					AS PermessoCancella,
					CONVERT(INTEGER,
						CASE 						
							WHEN ISNULL(TUS.Codice,'')<>''  THEN 1
							ELSE 0
						END	
						& ISNULL(M.Validabile,0) 
						& CASE 
							WHEN ISNULL(M.Validata,0)=0 THEN 1
								ELSE 0
							END
					)
					AS	PermessoValida,
					CONVERT(INTEGER,
						@bSuperUser
						& ISNULL(M.Validabile,0) 
						& ISNULL(M.Validata,0) 
					) AS PermessoSvalida,
					CONVERT(INTEGER,
						CASE 						
							WHEN ISNULL(REV.Codice,'') <> ''  THEN 1
							ELSE 0
						END	
						& ISNULL(M.Validabile,0) 
						& CASE 
							WHEN ISNULL(M.Validata,0) = 1 THEN 1
							ELSE 0
						  END
						& 
						  CASE 
							WHEN ISNULL(M.Revisione,0) = 0 THEN 1
							ELSE 0
						  END
					) AS PermessoRevisione,
					M.CodStatoSchedaCalcolato,
					SC.Descrizione AS DescrizioneStatoCalcolato,				
					SC.Colore AS ColoreStatoCalcolato,
					CONVERT(INTEGER,0) AS Contenitore,
					APP.CodUA,
					ISNULL(APP.DataUltimaModifica,APP.DataEvento) AS DataRiferimentoConsenso,
					CONVERT(INTEGER,0) AS FirmaDigitale,															NULL IDDocumentoFirmato,																		NULL AS IDCartellaAmbulatoriale,																NULL AS CodStatoCartellaAmbulatoriale,															0 AS PermessoChiusuraCartellaAmbulatoriale														FROM 
						T_MovAppuntamenti APP WITH (NOLOCK)				
							INNER JOIN 
								T_MovAppuntamentiAgende AGE WITH (NOLOCK)
									ON AGE.IDAppuntamento=APP.ID
							LEFT JOIN T_Agende A WITH (NOLOCK)
									ON AGE.CodAgenda=A.Codice	
							INNER JOIN T_MovSchede M WITH (NOLOCK)											ON (M.CodEntita='APP' AND
										M.IDEntita=APP.ID AND
										M.Storicizzata=0 AND
										M.CodStatoScheda <> 'CA'												)		
							LEFT JOIN	
									T_MovEpisodi E	WITH (NOLOCK)
								ON APP.IDEpisodio=E.ID			
							LEFT JOIN 
									T_Schede S WITH (NOLOCK)
									ON M.CodScheda=S.Codice					
						
						 							LEFT JOIN T_StatoSchedaCalcolato SC
									ON (M.CodStatoSchedaCalcolato=SC.Codice AND
									    M.CodScheda=SC.CodScheda)

						    							LEFT JOIN #tmpSchedeAbilitate TUS WITH (NOLOCK) ON
								M.CodScheda=TUS.Codice
							
														LEFT JOIN #tmpSchedeRevisione REV  WITH (NOLOCK)  ON
								M.CodScheda=REV.Codice

														LEFT JOIN #tmpIDFiltroSchede SG WITH (NOLOCK)
									ON M.ID=SG.IDScheda
		  												INNER JOIN #tmpPazientiAlias PA WITH (NOLOCK)
								ON APP.IDPaziente=PA.IDPaziente		
					WHERE							
												APP.CodStatoAppuntamento NOT IN ('CA','AN')  		
						AND 						AGE.CodStatoAppuntamentoAgenda NOT IN ('CA','AN') AND						
						APP.IDEpisodio IS NOT NULL	AND					
												E.CodStatoEpisodio NOT IN ('CA') AND

												1=CASE 
							WHEN NumeroListaAttesa IS NULL AND E.CodStatoEpisodio = 'AN' THEN 0
							ELSE  1
						  END
						
						 						AND
							ISNULL(S.CodTipoScheda,'')=CASE		
												WHEN @sCodTipoScheda='' THEN ISNULL(S.CodTipoScheda,'')
												ELSE @sCodTipoScheda
											END	

				
										AND
						1=CASE		
							WHEN @sCodScheda='' THEN 1
							ELSE 
								CASE 
									WHEN SG.IDScheda IS NULL THEN 0
									ELSE 1
								END		
						  END	
						
																				
												AND					
							ISNULL(AGE.CodAgenda,'')=CASE		
												WHEN @sCodAgenda='' THEN ISNULL(AGE.CodAgenda,'')
												ELSE @sCodAgenda
										   END	
						
												AND 
							1=(CASE 
									WHEN @bDaCompletare=0 THEN 1														WHEN M.ID IS NULL THEN 1															ELSE																						(CASE 
												WHEN ISNULL(M.DatiObbligatoriMancantiRTF,'')='' THEN 0														ELSE 1																									END)	
								 END)	
						
												AND
							1=(CASE	
									WHEN @nStatoAppuntamento=0 THEN 1												WHEN @nStatoAppuntamento=1 THEN																					(CASE 
																		WHEN APP.CodStatoAppuntamento IN ('IC','PR') THEN 1
																		ELSE 0
																	END)
									WHEN @nStatoAppuntamento=2 THEN 																				(CASE 
																		WHEN APP.CodStatoAppuntamento IN ('ER') THEN 1
																		ELSE 0
																	END)
									ELSE 1																	   END	)
						
												AND
						1=(CASE	
								WHEN @nStatoEpisodio=0 THEN 1											WHEN @nStatoEpisodio=1 THEN																					(CASE 
																		WHEN E.DataDimissione IS NULL AND E.DataAnnullamentoListaAttesa IS NULL THEN 1				 																		ELSE 0
																	END)
								WHEN @nStatoEpisodio=2 THEN 																				(CASE 
																		WHEN (E.DataDimissione IS NOT NULL OR E.DataAnnullamentoListaAttesa IS NOT NULL) THEN 1		 																		ELSE 0
																	END)
								WHEN @nStatoEpisodio=3 THEN 0											ELSE 1															   END	)	
						   
						 AND
						 						E.ID=CASE 
								WHEN @uIDEpisodio IS NOT NULL THEN @uIDEpisodio
								ELSE E.ID
							 END
						AND	 
												1=CASE 
							WHEN @bSoloSchedePaziente=1 THEN 0
							ELSE 1
						  END  			
				) AS Q	   			
		ORDER BY			
				Agenda,									DataInizio DESC,						Ordine ASC,								DataCreazione ASC						
				
				
	IF (@bAbilitaConsensi =1)
	BEGIN
		
				IF (@sCodTipoConsensoPiuAlto='Generico' OR ISNULL(@sCodTipoConsensoPiuAlto,'')='') 
		BEGIN		
						DELETE 
			FROM #tmp1SchedePaziente 
			WHERE CodUA NOT IN (SELECT CodUA FROM #tmpUAFiglie)
			
						DELETE 
			FROM #tmp2AppuntamentiSchedePaziente 
			WHERE CodUA NOT IN (SELECT CodUA FROM #tmpUAFiglie)
		
						DELETE FROM 
				#tmp3EpisodiSchede
			WHERE IDEpisodio NOT IN (SELECT E.IDEpisodio 
									 FROM #tmp3EpisodiSchede E
											INNER JOIN T_MovTrasferimenti T
												ON E.IDEpisodio=T.IDEpisodio
											INNER JOIN #tmpUAFiglie G
												ON T.CodUA=G.CodUA
									 WHERE T.CodStatoTrasferimento NOT IN ('CA')
									 )
						
						DELETE FROM 
				#tmp4AppuntamentiSchedeEpisodi
			WHERE IDEpisodio NOT IN (SELECT IDEpisodio FROM #tmp3EpisodiSchede)		
		END

				IF (@sCodTipoConsensoPiuAlto='Dossier') 
		BEGIN
																					DELETE 
			FROM #tmp2AppuntamentiSchedePaziente 
			WHERE CodUA NOT IN (SELECT CodUA FROM #tmpUAFiglie)
				  AND DataRiferimentoConsenso < @dDataConsensoRiferimento		
			 		
						CREATE TABLE #tmpSchedeAltrui
				(IDScheda UNIQUEIDENTIFIER NOT NULL,			
				 ParenteAbilitato BIT
				)

									
			
			TRUNCATE TABLE #tmpSchedeAltrui 

			INSERT INTO #tmpSchedeAltrui(IDScheda,ParenteAbilitato)
			SELECT IDScheda, 0 AS ParenteAbilitato
			FROM  #tmp1SchedePaziente 
			WHERE 
				CodUA NOT IN (SELECT CodUA FROM #tmpUAFiglie)
				AND DataRiferimentoConsenso < @dDataConsensoRiferimento

						UPDATE
				#tmpSchedeAltrui
			SET
				ParenteAbilitato=1
			FROM		
				#tmpSchedeAltrui SA
					CROSS APPLY dbo.MF_ElencoSchedeFiglio(SA.IDScheda)  AS ESF
					INNER JOIN #tmpUAFiglie G
						ON G.CodUA=ESF.CodUAFiglia
		
						DELETE FROM #tmp1SchedePaziente
			WHERE IDScheda IN (SELECT IDScheda FROM #tmpSchedeAltrui WHERE ParenteAbilitato=0)

				
											
			TRUNCATE TABLE #tmpSchedeAltrui 

			
	
			DELETE #tmp3EpisodiSchede 			
			FROM #tmp3EpisodiSchede S
					INNER JOIN T_MovEpisodi M
						ON S.IDEpisodio = M.ID
			WHERE S.IDEpisodio NOT IN (SELECT E.IDEpisodio 
									 FROM #tmp3EpisodiSchede E
											INNER JOIN T_MovTrasferimenti T
												ON E.IDEpisodio=T.IDEpisodio
											INNER JOIN #tmpUAFiglie G
												ON T.CodUA=G.CodUA
									 WHERE 									
										T.CodStatoTrasferimento NOT IN ('CA')
									 )	
						 	
				AND S.DataRiferimentoConsenso < @dDataConsensoRiferimento			
				AND M.CodStatoEpisodio NOT IN ('AT')

		
									
						DELETE FROM 
				#tmp4AppuntamentiSchedeEpisodi
			WHERE 
				 IDEpisodio NOT IN (SELECT IDEpisodio FROM #tmp3EpisodiSchede)	
				 AND DataRiferimentoConsenso < @dDataConsensoRiferimento				
		END 

		
	END 				SELECT * from #tmp1SchedePaziente
	ORDER BY Ordine ASC,
			 DataCreazione ASC

	SELECT * from #tmp2AppuntamentiSchedePaziente 		
	ORDER BY 
				Agenda,									DataInizio DESC,						Ordine	   ASC,							DataCreazione ASC							
	SELECT * from #tmp3EpisodiSchede
	ORDER BY 		
		DataRicovero DESC,  
						CASE 
			WHEN IDScheda IS NULL THEN DataRicovero
			ELSE 
				CASE 
					WHEN ISNULL(DataAperturaCartella,DataRicovero) <  DataRicovero THEN DataRicovero
					ELSE ISNULL(DataAperturaCartella,DataRicovero)
				END
		END ASC,		
		Ordine ASC,
		DataCreazione ASC
		
			
	SELECT * from #tmp4AppuntamentiSchedeEpisodi
	ORDER BY
			Agenda,								DataInizio DESC,					Ordine ASC,							DataCreazione ASC		
	IF @bDatiEstesi=1
		BEGIN
																		
															CREATE TABLE #tmpScheda
			(
				Codice VARCHAR(20) COLLATE Latin1_General_CI_AS
			)

			CREATE INDEX IX_Codice ON #tmpScheda (Codice)    	
													
						 
			INSERT INTO #tmpScheda(Codice)
			SELECT DISTINCT 
				CodScheda
			FROM 
				(SELECT 
					CodScheda
				FROM #tmp1SchedePaziente
				UNION
				SELECT 
					CodScheda
				FROM #tmp2AppuntamentiSchedePaziente
				UNION
				SELECT 
					CodScheda
				FROM #tmp3EpisodiSchede
				UNION			
				SELECT 
					CodScheda
				FROM #tmp3EpisodiSchede
				#tmp4AppuntamentiSchedeEpisodi) AS Q

						SELECT 
					TMP.Codice,
					T.Descrizione
				FROM #tmpScheda AS TMP WITH (NOLOCK)
					INNER JOIN T_Schede AS T WITH (NOLOCK)
						ON TMP.Codice=T.Codice										
				ORDER BY T.Descrizione
							
								
			
															CREATE TABLE #tmpAgende
			(
				Codice VARCHAR(20) COLLATE Latin1_General_CI_AS		
			)

			CREATE INDEX IX_CodAgenda ON #tmpAgende (Codice)   
			
			
			INSERT INTO #tmpAgende(Codice)
			SELECT DISTINCT CodAgenda
			FROM 
				(SELECT 
					CodAgenda 
				 FROM
					#tmp2AppuntamentiSchedePaziente
				 UNION
				 SELECT 
					CodAgenda 
				 FROM
					#tmp4AppuntamentiSchedeEpisodi
				) AS Q 
							
												SELECT 
					TMP.Codice,
					T.Descrizione
				FROM #tmpAgende AS TMP WITH (NOLOCK)
					INNER JOIN T_Agende AS T
						ON TMP.Codice=T.Codice			
				ORDER BY T.Descrizione	
				
		END			
			
				
	EXEC MSP_InsMovTimeStamp @xTimeStamp	

										
	DROP TABLE  #tmpPazientiAlias							 
		
	RETURN 0
END