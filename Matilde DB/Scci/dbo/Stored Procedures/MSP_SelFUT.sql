CREATE PROCEDURE [dbo].[MSP_SelFUT](@xParametri AS XML)
AS
BEGIN
	
	--------------------------------------------------------------------
	-- Impostazione parametri visualizzazione
	--------------------------------------------------------------------	

	DECLARE @bSeparaSezioni AS BIT
	DECLARE @bSeparaVieSomm AS BIT
	
	SET @bSeparaSezioni = 1
	SET @bSeparaVieSomm = 1
		   	
	--------------------------------------------------------------------
	-- Lettura Parametri XML
	--------------------------------------------------------------------
	
	DECLARE @uIDPaziente AS UNIQUEIDENTIFIER	
	DECLARE @uIDEpisodio AS UNIQUEIDENTIFIER
	DECLARE @uIDTrasferimento AS UNIQUEIDENTIFIER

	DECLARE @nNumRighe AS INTEGER
	DECLARE @nModalita AS INTEGER
	DECLARE @dDataOraInizio AS DATETIME
	DECLARE @dDataOraFine AS DATETIME
	DECLARE @sCodRuolo AS VARCHAR(20)
	DECLARE @sCodLogin AS VARCHAR(100)
	DECLARE @bModalita1SoloRange AS BIT
	
	DECLARE @sDataTmp AS VARCHAR(20)
	
	DECLARE @nStep  As INTEGER
	
	-- Variabili di servizio
	DECLARE @sGUID AS VARCHAR(Max)
	DECLARE @nTemp AS INTEGER
	DECLARE @sSQL AS VARCHAR(MAX)
	DECLARE @xTmpTS AS XML
	
	DECLARE @sSQLQueryAgendeEPI AS VARCHAR(MAX)
	DECLARE @sSQLQueryAgendePAZ AS VARCHAR(MAX)
	DECLARE @nNumMaxCaratteri AS INTEGER
		
	DECLARE @uIDCartella AS UNIQUEIDENTIFIER
			
	DECLARE @sColoreUltimaSomministrazioneProgrammata AS VARCHAR(50)		-- 56
	DECLARE @sCarattereUltimaSomministrazioneProgrammata AS VARCHAR(50)		-- 57
	DECLARE @sColoreUltimaSomministrazioneErogata AS VARCHAR(50)			-- 58
	DECLARE @sCarattereUltimaSomministrazioneErogata AS VARCHAR(50)			-- 59	
	DECLARE @sColorePrescrizioniChiuse AS VARCHAR(50)						-- 64
	
	DECLARE @sCodUA AS VARCHAR(20)
	DECLARE @sOraApertura AS VARCHAR(5)
	DECLARE @sOraChiusura AS VARCHAR(5)
	DECLARE @nOraApertura AS INTEGER
	DECLARE @nOraChiusura AS INTEGER
	
	-----------------------------------
	-- Permessi
	-----------------------------------
	
	-- Permessi Diario Medico
	DECLARE @bDCMVisualizza AS BIT
	DECLARE @bDCMInserisci AS BIT
	DECLARE @bDCMModifica AS BIT
	
	-- Permessi Diario Infermieristico
	DECLARE @bDCIVisualizza AS BIT
	DECLARE @bDCIInserisci AS BIT
	DECLARE @bDCIModifica AS BIT
	
	-- Permessi Evidenza Clinica
	DECLARE @bEVCVisualizza AS BIT
	DECLARE @bEVCModifica AS BIT
	
	-- Permessi Ordini
	DECLARE @bOEVisualizza AS BIT
	DECLARE @bOEInserisci AS BIT
	DECLARE @bOEModifica AS BIT
	
	-- Permessi Worklist Infermieristica
	DECLARE @bWKIVisualizza AS BIT
	DECLARE @bWKIInserisci AS BIT
	DECLARE @bWKIModifica AS BIT
	DECLARE @bWKICancella AS BIT
	DECLARE @bWKIAnnulla AS BIT
	DECLARE @bWKICompleta AS BIT
	
	-- Permessi Farmaci
	DECLARE @bPRFVisualizza AS BIT
	DECLARE @bPRFInserisci AS BIT
	DECLARE @bPRFModifica AS BIT
	DECLARE @bPRFValida AS BIT	
	
	-- Appuntamenti
	DECLARE @bAPPVisualizza AS BIT
	DECLARE @bAPPInserisci AS BIT
	DECLARE @bAPPModifica AS BIT
	DECLARE @bAPPCancella AS BIT
	
	-- Parametri Vitali
	DECLARE @bPVTVisualizza AS BIT
	DECLARE @bPVTInserisci AS BIT
	DECLARE @bPVTModifica AS BIT
	DECLARE @bPVTCancella AS BIT
	DECLARE @bPVTAnnulla AS BIT
	
	-- Note Generali
	DECLARE @bNTGVisualizza AS BIT
	DECLARE @bNTGInserisci AS BIT
	DECLARE @bNTGModifica AS BIT
	DECLARE @bNTGCancella AS BIT
	
	------------------------------------
	-- Sezioni
	------------------------------------
	
	DECLARE @bADT AS BIT			-- ADT	Eventi ADT
	DECLARE @bDCM AS BIT			-- DCM	Diario Medico
	DECLARE @bDCI AS BIT			-- DCI	Diario Infermieristico
	DECLARE @bEVC AS BIT			-- EVC	Evidenza Clinica
	DECLARE @bWKI AS BIT			-- WKI	WorkList Infermieristica
	DECLARE @bPFM AS BIT			-- PFM	Farmaci
	DECLARE @bPFA AS BIT			-- PFA	Al Bisogno
	DECLARE @bAPP AS BIT			-- APP	Appuntamenti
	DECLARE @bPVT AS BIT			-- PVT	Parametri Vitali
	DECLARE @bOE AS BIT				-- OE 	Order Entry
	DECLARE @bNTG AS BIT			-- NTG 	Note Generali
	
	
	-- @uIDPaziente	
	SET @sGUID=(SELECT TOP 1 ValoreParametro.IDPaziente.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDPaziente') as ValoreParametro(IDPaziente))
	IF 	ISNULL(@sGUID,'') <> '' SET @uIDPaziente=CONVERT(UNIQUEIDENTIFIER,	@sGUID)	
	
	-- @uIDEpisodio
	SET @sGUID=(SELECT TOP 1 ValoreParametro.IDEpisodio.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDEpisodio') as ValoreParametro(IDEpisodio))
	IF 	ISNULL(@sGUID,'') <> '' SET @uIDEpisodio=CONVERT(UNIQUEIDENTIFIER,	@sGUID)	
	
	-- @uIDTrasferimento
	SET @sGUID=(SELECT TOP 1 ValoreParametro.IDTrasferimento.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDTrasferimento') as ValoreParametro(IDTrasferimento))
	IF 	ISNULL(@sGUID,'') <> '' SET @uIDTrasferimento=CONVERT(UNIQUEIDENTIFIER,	@sGUID)	
					  				  	
	-- @nNumRighe
	SET @nNumRighe=(SELECT	TOP 1 ValoreParametro.NumRighe.value('.','INTEGER')
								FROM @xParametri.nodes('/Parametri/NumRighe') as ValoreParametro(NumRighe))	
	SET @nNumRighe=ISNULL(@nNumRighe,0)
		
	-- 	@nModalita							
	SET @nModalita=(SELECT TOP 1 ValoreParametro.Modalita.value('.','INTEGER')
					  FROM @xParametri.nodes('/Parametri/Modalita') as ValoreParametro(Modalita))
	SET @nModalita=ISNULL(@nModalita,0)

	
	-- @dDataOraInizio
	SET @sDataTmp=(SELECT TOP 1 ValoreParametro.DataOraInizio.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/DataOraInizio') as ValoreParametro(DataOraInizio))					  
	SET @sDataTmp=ISNULL(@sDataTmp,'')
	IF @sDataTmp<> '' 
		BEGIN
		
			SET @sDataTmp= SUBSTRING(@sDataTmp,4,2) + '-' 
						+ LEFT(@sDataTmp,2) + '-' +
						+ SUBSTRING(@sDataTmp,7,4) +
						' ' + RIGHT(@sDataTmp,5)
			IF ISDATE(@sDataTmp)=1
				SET	@dDataOraInizio=CONVERT(DATETIME,@sDataTmp,120)						
			ELSE
				SET	@dDataOraInizio =NULL			
		END
			
	
	-- @dDataOraFine
	SET @sDataTmp=(SELECT TOP 1 ValoreParametro.DataOraFine.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/DataOraFine') as ValoreParametro(DataOraFine))
	SET @sDataTmp=ISNULL(@sDataTmp,'')
	IF @sDataTmp<> '' 
		BEGIN			
			SET @sDataTmp= SUBSTRING(@sDataTmp,4,2) + '-' 
						+ LEFT(@sDataTmp,2) + '-' +
						+ SUBSTRING(@sDataTmp,7,4) +
						' ' + RIGHT(@sDataTmp,5)
			IF ISDATE(@sDataTmp)=1
				SET	@dDataOraFine=CONVERT(DATETIME,@sDataTmp,120)						
			ELSE
				SET	@dDataOraFine =NULL		
		END 		
	

	-- 	@bModalita1SoloRange
	SET @bModalita1SoloRange=(SELECT TOP 1 ValoreParametro.Modalita1SoloRange.value('.','BIT')
					  FROM @xParametri.nodes('/Parametri/Modalita1SoloRange') as ValoreParametro(Modalita1SoloRange))
	SET @bModalita1SoloRange=ISNULL(@bModalita1SoloRange,0)
	
	-- @nStep
	SET @nStep=(SELECT TOP 1 ValoreParametro.Step.value('.','INTEGER')
					  FROM @xParametri.nodes('/Parametri/Step') as ValoreParametro(Step))
	SET @nStep=ISNULL(@nStep,0)
	
	
	-- @xTimeStamp 
    DECLARE @xTimeStamp AS XML	

    SET @xTimeStamp=(SELECT TOP 1 ValoreParametro.TS.query('.')
					  FROM @xParametri.nodes('/Parametri/TimeStamp') as ValoreParametro(TS))
	
	SET @xTimeStamp=CONVERT(XML,
						'<Parametri>' + CONVERT(varchar(max),@xTimeStamp) +
						'</Parametri>')
						
	-- @sCodRuolo
	SET @sCodRuolo=(SELECT TOP 1 ValoreParametro.CodRuolo.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/TimeStamp/CodRuolo') as ValoreParametro(CodRuolo))
	SET @sCodRuolo=ISNULL(@sCodRuolo,'')
	
	-- @sCodLogin
	SET @sCodLogin=(SELECT TOP 1 ValoreParametro.CodLogin.value('.','VARCHAR(100)')
					  FROM @xParametri.nodes('/Parametri/TimeStamp/CodLogin') as ValoreParametro(CodLogin))
	SET @sCodLogin=ISNULL(@sCodLogin,'')
			
											
	SET @nNumMaxCaratteri=(SELECT Valore FROM T_Config WITH (NOLOCK) WHERE ID=161)
	SET @nNumMaxCaratteri=ISNULL(@nNumMaxCaratteri,60)
	
	----------------------------			
	-- Elenco Vie di Somministrazione
	----------------------------	
	CREATE TABLE #tmpViaSomministrazione
	(
		Codice VARCHAR(20) COLLATE Latin1_General_CI_AS
	)	
	
	IF @xParametri.exist('/Parametri/ViaSomministrazione')=1
		INSERT INTO #tmpViaSomministrazione(Codice)	
			SELECT 	ValoreParametro.ViaSomministrazione.value('.','VARCHAR(20)')	
				FROM @xParametri.nodes('/Parametri/ViaSomministrazione') as ValoreParametro(ViaSomministrazione)
			
	----------------------------			
	-- Elenco Sezioni passate	
	----------------------------	
	CREATE TABLE #tmpSezioni
	(
		Codice VARCHAR(20) COLLATE Latin1_General_CI_AS
	)	
	
	IF @xParametri.exist('/Parametri/CodSezione')=1
		INSERT INTO #tmpSezioni(Codice)	
			SELECT 	ValoreParametro.CodSezione.value('.','VARCHAR(20)')	
				FROM @xParametri.nodes('/Parametri/CodSezione') as ValoreParametro(CodSezione)
	
	
	-- ADT	Eventi ADT
	SET @bADT=(SELECT COUNT(*) FROM #tmpSezioni WHERE Codice='ADT')
	
	-- DCM	Diario Medico
	SET @bDCM=(SELECT COUNT(*) FROM #tmpSezioni WHERE Codice='DCM')
	
	-- DCI	Diario Infermieristico
	SET @bDCI=(SELECT COUNT(*) FROM #tmpSezioni WHERE Codice='DCI')
	
	-- EVC	Evidenza Clinica
	SET @bEVC=(SELECT COUNT(*) FROM #tmpSezioni WHERE Codice='EVC')
	
	-- WKI	WorkList Infermieristica
	SET @bWKI=(SELECT COUNT(*) FROM #tmpSezioni WHERE Codice='WKI')
	
	-- PFM	Farmaci
	SET @bPFM=(SELECT COUNT(*) FROM #tmpSezioni WHERE Codice='PFM')
	
	-- PFA	Al Bisogno
	SET @bPFA=(SELECT COUNT(*) FROM #tmpSezioni WHERE Codice='PFA')
	
	-- APP	Appuntamenti
	SET @bAPP=(SELECT COUNT(*) FROM #tmpSezioni WHERE Codice='APP')
	
	-- PVT	Parametri Vitali
	SET @bPVT=(SELECT COUNT(*) FROM #tmpSezioni WHERE Codice='PVT')
	
	-- OE	Order Entry
	SET @bOE=(SELECT COUNT(*) FROM #tmpSezioni WHERE Codice='OE')
	
	-- NTG	Note Generali
	SET @bNTG=(SELECT COUNT(*) FROM #tmpSezioni WHERE Codice='NTG')
	
	--------------------------------------------------------------------
	-- Elabora
	--------------------------------------------------------------------	
	
	---------------------------------------------------------------
	-- Lettura Permessi
	---------------------------------------------------------------
	
	--
	-- Permessi Diario Medico
	--
	
	-- @bDCMVisualizza
	SET @nTemp=(SELECT COUNT(*) FROM T_AssRuoliModuli
				WHERE CodModulo='DiarioC_Visualizza'	
				AND CodRuolo=@sCodRuolo)
	IF @nTemp>=1 
		SET @bDCMVisualizza=1
	ELSE
		SET @bDCMVisualizza=0	

	-- @bDCMInserisci		
	SET @nTemp=(SELECT COUNT(*) FROM T_AssRuoliModuli
				WHERE CodModulo='DiarioC_Inserisci'	
				AND CodRuolo=@sCodRuolo)
	IF @nTemp>=1 
		SET @bDCMInserisci=1
	ELSE
		SET @bDCMInserisci=0	
			
	-- @bDCMModifica		
	SET @bDCMModifica=@bDCMInserisci
		
	
	--	
	-- Permessi Diario Infermieristico	
	--
	
	-- @bDCIVisualizza
	SET @nTemp=(SELECT COUNT(*) FROM T_AssRuoliModuli
				WHERE CodModulo='DiarioC_Visualizza'	
				AND CodRuolo=@sCodRuolo)
	IF @nTemp>=1 
		SET @bDCIVisualizza=1
	ELSE
		SET @bDCIVisualizza=0	

	-- @bDCIInserisci		
	SET @nTemp=(SELECT COUNT(*) FROM T_AssRuoliModuli
				WHERE CodModulo='DiarioC_Inserisci'	
				AND CodRuolo=@sCodRuolo)
	IF @nTemp>=1 
		SET @bDCIInserisci=1
	ELSE
		SET @bDCIInserisci=0	
			
	-- @bDCIModifica	
	SET @bDCIModifica=@bDCIInserisci
	

	--		
	-- Permessi Evidenza Clinica
	--
	
	-- @bEVCVisualizza		
	SET @nTemp=(SELECT COUNT(*) FROM T_AssRuoliModuli
				WHERE CodModulo='EvidenzaC_Visualizza'	
				AND CodRuolo=@sCodRuolo)
	IF @nTemp>=1 
		SET @bEVCVisualizza=1
	ELSE
		SET @bEVCVisualizza=0	
	
	-- @@bEVCModifica	
	SET @nTemp=(SELECT COUNT(*) FROM T_AssRuoliModuli
				WHERE CodModulo='EvidenzaC_Vista'	
				AND CodRuolo=@sCodRuolo)
	IF @nTemp>=1 
		SET @bEVCModifica=1
	ELSE
		SET @bEVCModifica=0	
		
	--
	-- Permessi Ordini
	--
	
	-- @bOEVisualizza	
	SET @nTemp=(SELECT COUNT(*) FROM T_AssRuoliModuli
				WHERE CodModulo='Ordini_Visualizza'	
				AND CodRuolo=@sCodRuolo)
	IF @nTemp>=1 
		SET @bOEVisualizza=1
	ELSE
		SET @bOEVisualizza=0	
	
	-- @bOEInserisci
	SET @nTemp=(SELECT COUNT(*) FROM T_AssRuoliModuli
				WHERE CodModulo='Ordini_Inserisci'	
				AND CodRuolo=@sCodRuolo)
	IF @nTemp>=1 
		SET @bOEInserisci=1
	ELSE
		SET @bOEInserisci=0	
	
	-- @bOEModifica		
	SET @nTemp=(SELECT COUNT(*) FROM T_AssRuoliModuli
				WHERE CodModulo='Ordini_Modifica'	
				AND CodRuolo=@sCodRuolo)
	IF @nTemp>=1 
		SET @bOEModifica=1
	ELSE
		SET @bOEModifica=0	
		
	--
	-- Permessi Worklist Infermieristica
	--
	-- @bWKIVisualizza
	SET @nTemp=(SELECT COUNT(*) FROM T_AssRuoliModuli
				WHERE CodModulo='WorkL_Visualizza'	
				AND CodRuolo=@sCodRuolo)
	IF @nTemp>=1 
		SET @bWKIVisualizza=1
	ELSE
		SET @bWKIVisualizza=0	
		
	-- @bWKIInserisci
	SET @nTemp=(SELECT COUNT(*) FROM T_AssRuoliModuli
				WHERE CodModulo='WorkL_Inserisci'	
				AND CodRuolo=@sCodRuolo)
	IF @nTemp>=1 
		SET @bWKIInserisci=1
	ELSE
		SET @bWKIInserisci=0	
		
	-- @bWKIModifica
	-- In realtà è il pemresso di Completamento 
	SET @nTemp=(SELECT COUNT(*) FROM T_AssRuoliModuli
				WHERE CodModulo='WorkL_Completa'	
				AND CodRuolo=@sCodRuolo)
	IF @nTemp>=1 
		SET @bWKIModifica=1
	ELSE
		SET @bWKIModifica=0	
	
		
	-- @bWKICancella	
	SET @nTemp=(SELECT COUNT(*) FROM T_AssRuoliModuli
				WHERE CodModulo='WorkL_Cancella'	
				AND CodRuolo=@sCodRuolo)
	IF @nTemp>=1 
		SET @bWKICancella=1
	ELSE
		SET @bWKICancella=0	
		
	-- @bWKIAnnulla
	SET @nTemp=(SELECT COUNT(*) FROM T_AssRuoliModuli
				WHERE CodModulo='WorkL_Annulla'	
				AND CodRuolo=@sCodRuolo)
	IF @nTemp>=1 
		SET @bWKIAnnulla=1
	ELSE
		SET @bWKIAnnulla=0	
	
	-- @bWKICompleta
	SET @nTemp=(SELECT COUNT(*) FROM T_AssRuoliModuli
				WHERE CodModulo='WorkL_Completa'	
				AND CodRuolo=@sCodRuolo)
	IF @nTemp>=1 
		SET @bWKICompleta=1
	ELSE
		SET @bWKICompleta=0	
	
	--
	-- Permessi Farmaci
	--
	-- @bPRFVisualizza
	SET @nTemp=(SELECT COUNT(*) FROM T_AssRuoliModuli
				WHERE CodModulo='Prescr_Visualizza'	
				AND CodRuolo=@sCodRuolo)
	IF @nTemp>=1 
		SET @bPRFVisualizza=1
	ELSE
		SET @bPRFVisualizza=0	
		
	-- @bPRFInserisci
	SET @nTemp=(SELECT COUNT(*) FROM T_AssRuoliModuli
				WHERE CodModulo='Prescr_Inserisci'	
				AND CodRuolo=@sCodRuolo)
	IF @nTemp>=1 
		SET @bPRFInserisci=1
	ELSE
		SET @bPRFInserisci=0	

	-- @bPRFModifica
	SET @nTemp=(SELECT COUNT(*) FROM T_AssRuoliModuli
				WHERE CodModulo='Prescr_Modifica'	
				AND CodRuolo=@sCodRuolo)
	IF @nTemp>=1 
		SET @bPRFModifica=1
	ELSE
		SET @bPRFModifica=0	
	
	SET @nTemp=(SELECT COUNT(*) FROM T_AssRuoliModuli
				WHERE CodModulo='Prescr_Valida'	
				AND CodRuolo=@sCodRuolo)
	IF @nTemp>=1 
		SET @bPRFValida=1
	ELSE
		SET @bPRFValida=0
					
	--
	-- Appuntamenti
	--
	--@bAPPVisualizza
	SET @nTemp=(SELECT COUNT(*) FROM T_AssRuoliModuli
				WHERE CodModulo='App_Visualizza'	
				AND CodRuolo=@sCodRuolo)
	IF @nTemp>=1 
		SET @bAPPVisualizza=1
	ELSE
		SET @bAPPVisualizza=0	

	-- @bAPPInserisci
	SET @nTemp=(SELECT COUNT(*) FROM T_AssRuoliModuli
				WHERE CodModulo='App_Inserisci'	
				AND CodRuolo=@sCodRuolo)
	IF @nTemp>=1 
		SET @bAPPInserisci=1
	ELSE
		SET @bAPPInserisci=0	
		
	-- @bAPPModifica
	SET @nTemp=(SELECT COUNT(*) FROM T_AssRuoliModuli
				WHERE CodModulo='App_Modifica'	
				AND CodRuolo=@sCodRuolo)
	IF @nTemp>=1 
		SET @bAPPModifica=1
	ELSE
		SET @bAPPModifica=0	
		
	-- @bAPPCancella
	SET @nTemp=(SELECT COUNT(*) FROM T_AssRuoliModuli
				WHERE CodModulo='App_Cancella'	
				AND CodRuolo=@sCodRuolo)
	IF @nTemp>=1 
		SET @bAPPCancella=1
	ELSE
		SET @bAPPCancella=0	
	

	--	
	-- Parametri Vitali
	--
	--@bPVTVisualizza
	SET @nTemp=(SELECT COUNT(*) FROM T_AssRuoliModuli
				WHERE CodModulo='ParamV_Visualizza'	
				AND CodRuolo=@sCodRuolo)
	IF @nTemp>=1 
		SET @bPVTVisualizza=1
	ELSE
		SET @bPVTVisualizza=0	

	--@bPVTInserisci
	SET @nTemp=(SELECT COUNT(*) FROM T_AssRuoliModuli
				WHERE CodModulo='ParamV_Inserisci'	
				AND CodRuolo=@sCodRuolo)
	IF @nTemp>=1 
		SET @bPVTInserisci=1
	ELSE
		SET @bPVTInserisci=0	

	--@bPVTModifica
	SET @nTemp=(SELECT COUNT(*) FROM T_AssRuoliModuli
				WHERE CodModulo='ParamV_Modifica'	
				AND CodRuolo=@sCodRuolo)
	IF @nTemp>=1 
		SET @bPVTModifica=1
	ELSE
		SET @bPVTModifica=0	
		
	
	--@bPVTCancella
	SET @nTemp=(SELECT COUNT(*) FROM T_AssRuoliModuli
				WHERE CodModulo='ParamV_Cancella'	
				AND CodRuolo=@sCodRuolo)
	IF @nTemp>=1 
		SET @bPVTCancella=1
	ELSE
		SET @bPVTCancella=0	
			
		
	--@bPVTAnnulla
	SET @nTemp=(SELECT COUNT(*) FROM T_AssRuoliModuli
				WHERE CodModulo='ParamV_Annulla'	
				AND CodRuolo=@sCodRuolo)
	IF @nTemp>=1 
		SET @bPVTAnnulla=1
	ELSE
		SET @bPVTAnnulla=0	
		
	--	
	-- Note Generali
	--
	--@bNTGVisualizza
	SET @nTemp=(SELECT COUNT(*) FROM T_AssRuoliModuli
				WHERE CodModulo='NoteG_Visualizza'	
				AND CodRuolo=@sCodRuolo)
	IF @nTemp>=1 
		SET @bNTGVisualizza=1
	ELSE
		SET @bNTGVisualizza=0	

	--@bNTGInserisci
	SET @nTemp=(SELECT COUNT(*) FROM T_AssRuoliModuli
				WHERE CodModulo='NoteG_Inserisci'	
				AND CodRuolo=@sCodRuolo)
	IF @nTemp>=1 
		SET @bNTGInserisci=1
	ELSE
		SET @bNTGInserisci=0	

	--@bNTGModifica
	SET @nTemp=(SELECT COUNT(*) FROM T_AssRuoliModuli
				WHERE CodModulo='NoteG_Modifica'	
				AND CodRuolo=@sCodRuolo)
	IF @nTemp>=1 
		SET @bNTGModifica=1
	ELSE
		SET @bNTGModifica=0	
		
	
	--@bNTGCancella
	SET @nTemp=(SELECT COUNT(*) FROM T_AssRuoliModuli
				WHERE CodModulo='NoteG_Cancella'	
				AND CodRuolo=@sCodRuolo)
	IF @nTemp>=1 
		SET @bNTGCancella=1
	ELSE
		SET @bNTGCancella=0	
			


	-- Dal trasferimento leggo l'ID Cartella
	IF  @nModalita IN (1,2)						
		SET @uIDCartella=(SELECT TOP 1 IDCartella FROM T_MovTrasferimenti WHERE ID=@uIDTrasferimento)
			
	---------------------------------------------------------------
	---------------------------------------------------------------		
	---------------------------------------------------------------
	-- Modalita = 0, elenco sezioni
	---------------------------------------------------------------
	---------------------------------------------------------------
	---------------------------------------------------------------
	IF @nModalita=0
		BEGIN
			SET @sSQL =''
			SET @sSQL = @sSQL + 'SELECT 
											Q.CodEntita,
											Q.Codice,
											S.Descrizione,
											PermessoInserisci,
											S.Colore,
											S.Ordine
									 FROM  
										( -- ADT
										SELECT 
												''ADT'' AS CodEntita,
												''ADT'' AS Codice,
												0 AS PermessoInserisci 											
										UNION
										-- Diario Medico 		
										SELECT 
												''DCL'' AS CodEntita,
												''DCM'' AS Codice,
												1 AS PermessoInserisci
										UNION		
										-- Diario Infermieristico
										SELECT 
												''DCL'' AS CodEntita,
												''DCI'' AS Codice,
												1 AS PermessoInserisci 	
										UNION		
										-- Evidenza Clinica
										SELECT 
												''EVC'' AS CodEntita,
												''EVC'' AS Codice,
												0 AS PermessoInserisci 			-- Da specifica non si inserisce NULLA
										UNION		
										-- WorkList Infermieristica
										SELECT 
												''WKI'' AS CodEntita,
												''WKI'' AS Codice,
												CONVERT(INTEGER,''' + CONVERT(CHAR(1),@bWKIInserisci) + ''') AS PermessoInserisci 	 						
										UNION		
										-- Farmaci
										SELECT 
												''PRF'' AS CodEntita,
												''PFM'' AS Codice,												
												CONVERT(INTEGER,''' + CONVERT(CHAR(1),@bPRFInserisci) + ''') AS PermessoInserisci
										UNION		 	
										-- Farmaci, Al Bisogno
										SELECT 
												''PRF'' AS CodEntita,
												''PFA'' AS Codice,																						
												CONVERT(INTEGER,''' + CONVERT(CHAR(1),@bPRFInserisci) + ''') AS PermessoInserisci
										UNION
										-- Appuntamenti
										SELECT 
												''APP'' AS CodEntita,
												''APP'' AS Codice,											
												CONVERT(INTEGER,''' + CONVERT(CHAR(1),@bAPPInserisci) + ''') AS PermessoInserisci 	
										UNION
										-- Parametri Vitali
										SELECT		
												''PVT'' AS CodEntita,
												''PVT'' AS Codice,												
												CONVERT(INTEGER,''' + CONVERT(CHAR(1),@bPVTInserisci) + ''') AS PermessoInserisci 	
										UNION
										-- Order Entry
										SELECT 
												''OE'' AS CodEntita,
												''OE'' AS Codice,												
												CONVERT(INTEGER,''' + CONVERT(CHAR(1),@bOEInserisci) + ''') AS PermessoInserisci 					
										UNION
										-- Note Generali
										SELECT 
												''NTG'' AS CodEntita,
												''NTG'' AS Codice,												
												CONVERT(INTEGER,''' + CONVERT(CHAR(1),@bNTGInserisci) + ''') AS PermessoInserisci												
										) AS Q
									LEFT JOIN T_SezioniFUT S
										ON Q.Codice=S.Codice
									ORDER BY S.Ordine ASC
									
									'												 			
			EXEC (@sSQL)		
			
			------------------------------------------------------
			-- DataSet di filtro ulteriore Via di Somministrazione
			------------------------------------------------------
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
			
											 								 
	END -- fine IF Modalit=0
	ELSE			
		---------------------------------------------------------------
		---------------------------------------------------------------		
		---------------------------------------------------------------
		-- Modalita = 1, Elenco Righe
		---------------------------------------------------------------
		---------------------------------------------------------------
		---------------------------------------------------------------
		IF @nModalita =1
		BEGIN			
			
			-- Crea tabella temporanea per risultati di Output			
				CREATE TABLE #tmpMovMod1
				(							
					CodSezione VARCHAR(20) COLLATE Latin1_General_CI_AS,
					CodVoce VARCHAR(600) COLLATE Latin1_General_CI_AS,
					DescrVoce VARCHAR(255) COLLATE Latin1_General_CI_AS,
					RTF VARCHAR(MAX) COLLATE Latin1_General_CI_AS,				
					IconaVoce  VARbinary(MAX),					
					IDIcona INTEGER,
					PermessoDettaglio INTEGER,
					PermessoInserimento INTEGER,
					PermessoGrafico INTEGER,				
					PermessoSospendi INTEGER,	
					IDEntita VARCHAR(50),
					Azione VARCHAR(5) COLLATE Latin1_General_CI_AS,	
					OrdVoce VARCHAR(MAX) COLLATE Latin1_General_CI_AS,
					DescrViaSomm VARCHAR(255) COLLATE Latin1_General_CI_AS,
					CodViaSomm VARCHAR(20) COLLATE Latin1_General_CI_AS			
				)			
				
				---------------------------------------------------------------
				-- Modalita = 1, elenco righe
				---------------------------------------------------------------
				
					-- ADT	Eventi ADT
					-- DCM	Diario Medico
					-- DCI	Diario Infermieristico
					-- EVC	Evidenza Clinica
					-- WKI	WorkList Infermieristica
					-- PFM	Farmaci
					-- PFA	Al Bisogno
					-- APP	Appuntamenti
					-- PVT	Parametri Vitali
					-- OE	Order Entry
					-- NTG	Note Generali

				-- ADT	Eventi ADT, modalità 1
				IF @bADT=1 
				BEGIN						
					-- Elenco 
					INSERT INTO #tmpMovMod1(												
								CodSezione,
								CodVoce,	
								DescrVoce,						
								RTF,																			
								IconaVoce,
								IDIcona,								
								PermessoInserimento,							
								PermessoGrafico,
								IDEntita,
								Azione,								
								OrdVoce)								
				
					SELECT 
						 'ADT' AS CodSezione,
						 'T' AS CodVoce,
						 'Eventi ADT' AS DescrVoce,
				 		 NULL AS Rtf,		   				 			 
						 NULL AS IconaVoce,
						 I.Icona AS IDIcona,
						 0 AS PermessoInserimento,			-- Non si inseriscono mai ADT
				   		 0 AS  PermessoGrafico,				-- Non ci sono grafici ADT
				   		 NULL AS IDEntita,
				   		 NULL AS Azione,
						 NULL AS OrdVoce				
				  FROM 	(SELECT 
									  IDNum AS Icona
								 FROM T_Icone
								 WHERE CodEntita='FUT' 
									   AND CodTipo='ADT') AS I		 
				END
					
				---- DCM	Diario Medico, modalità 1
				IF @bDCM=1
				BEGIN
						
						INSERT INTO #tmpMovMod1(													
									CodSezione,
									CodVoce,
									DescrVoce,
									RTF,														
									IconaVoce,		
									IDIcona,						
									PermessoInserimento,								
									PermessoGrafico,
									IDEntita,
				   					Azione,
									OrdVoce)
						SELECT 
							'DCM' AS CodSezione,
							CodVoce,
							T.Descrizione  AS DescrVoce,
							NULL AS RTF,			
							NULL AS IconaVoce,
							I.IDIcona,
							@bDCMInserisci AS PermessoInserimento,			-- Peremsso di Modulo per Inserimento			   		
							0 AS  PermessoGrafico,							-- Non ci sono grafici Diario
							NULL AS IDEntita,
				   			NULL AS Azione,
							T.Descrizione AS OrdVoce 
						FROM			
								(SELECT 																		
									T.Codice AS CodVoce			   			-- Tipo Voce Diario
								FROM 
									T_MovDiarioClinico	M								
										LEFT JOIN T_MovPazienti P
													ON (M.IDEpisodio = P.IDEpisodio)
																
										LEFT JOIN T_TipoVoceDiario T
													ON (M.CodTipoVoceDiario = T.Codice)																									   																	
								WHERE 
									IDPaziente=@uIDPaziente AND
									M.IDEpisodio=@uIDEpisodio AND
									M.CodStatoDiario IN ('VA') AND				-- Solo Validate
									T.CodTipoDiario='M'	AND						-- Diario Medico
									M.CodTipoRegistrazione='M'					-- Tipo Registrazione Manuale	
									AND
									(@bModalita1SoloRange=0 OR
										( DataEvento >=@dDataOraInizio 
										AND DataEvento <= @dDataOraFine)
									)								
								GROUP BY 								
									T.Codice	
								) AS Q
								
							LEFT JOIN T_TipoVoceDiario T
									ON (Q.CodVoce = T.Codice)
							LEFT JOIN T_TipoDiario TD
									ON T.CodTipoDiario=TD.Codice	
							-- Icone		
							LEFT JOIN 
									(SELECT
									  IDNum AS IDIcona,
									  CodTipo,
									  CodStato					 
									 FROM T_Icone
									 WHERE CodEntita='DCL'
									) AS I
								ON (I.CodTipo='M' AND 	
								    I.CodStato=''
									)
					
				END
		
				
				-- DCI	Diario Infermieristico, modalità 1
				IF @bDCI=1			
					BEGIN
						INSERT INTO #tmpMovMod1(														
										CodSezione,
										CodVoce,
										DescrVoce,
										RTF,														
										IconaVoce,	
										IDIcona,							
										PermessoInserimento,								
										PermessoGrafico,
										IDEntita,
				   						Azione,
										OrdVoce)
						SELECT 
								'DCI' AS CodSezione,
								CodVoce,
								T.Descrizione  AS DescrVoce,
								NULL AS RTF,
								NULL AS IconaVoce,
								I.IDIcona,
								@bDCIInserisci AS PermessoInserimento,			-- Peremsso di Modulo per Inserimento	
								0 AS  PermessoGrafico,							-- Non ci sono grafici Diario
								NULL AS IDEntita,
				   				NULL AS Azione,
								T.Descrizione AS OrdVoce   
						FROM				
								(SELECT 						
										T.Codice AS CodVoce						-- Tipo Voce Diario
								FROM 
										T_MovDiarioClinico	M								
										LEFT JOIN T_MovPazienti P
														ON (M.IDEpisodio = P.IDEpisodio)
																	
										LEFT JOIN T_TipoVoceDiario T
														ON (M.CodTipoVoceDiario = T.Codice)
																																																
								WHERE 
										IDPaziente=@uIDPaziente AND
										M.IDEpisodio=@uIDEpisodio AND
										M.CodStatoDiario IN ('VA')	AND				-- Non annullato e non cancellato
										T.CodTipoDiario='I'	AND						-- Diario Infermieristico
										M.CodTipoRegistrazione='M'					-- Tipo Registrazione Manuale						
										AND
										(@bModalita1SoloRange=0 OR
											( DataEvento >=@dDataOraInizio 
											AND DataEvento <= @dDataOraFine)
										)
								GROUP BY 
									T.Codice 		
								) AS Q		
						LEFT JOIN T_TipoVoceDiario T
								ON (Q.CodVoce = T.Codice)		
						LEFT JOIN T_TipoDiario TD
								ON T.CodTipoDiario=TD.Codice	
						-- Icone		
						LEFT JOIN 
									(SELECT
									  IDNum AS IDIcona,
									  CodTipo,
									  CodStato					 
									 FROM T_Icone
									 WHERE CodEntita='DCL'
									) AS I
								ON (I.CodTipo='I' AND 	
								    I.CodStato=''
									)
								
				END

					
				-- EVC	Evidenza Clinica, modalità 1
				IF @bEVC=1
				BEGIN
					INSERT INTO	#tmpMovMod1(														
										CodSezione,
										CodVoce,
										DescrVoce,
										RTF,														
										IconaVoce,	
										IDIcona,							
										PermessoInserimento,								
										PermessoGrafico,
										IDEntita,
				   						Azione,
										OrdVoce)	
					SELECT 
						'EVC' AS CodSezione,
						Q.CodVoce,
						T.Descrizione  AS DescrVoce,
						NULL AS RTF,					
						NULL AS IconaVoce,
						I.IDIcona,							
						@bOEInserisci AS PermessoInserimento,					-- Peremsso di Modulo per Inserimento =0 (si fa un nuovo ordine)
			   			0 AS  PermessoGrafico,									-- Non ci sono grafici 
			   			NULL AS IDEntita,
				   		NULL AS Azione,
						T.Descrizione AS OrdVoce   
						
						FROM 
						(								
							SELECT 					
								T.Codice AS CodVoce								-- 	Cod Tipo Evidenza Clinica
							FROM 
									T_MovEvidenzaClinica	M								
									LEFT JOIN T_MovPazienti P
													ON (M.IDEpisodio = P.IDEpisodio)													
									LEFT JOIN T_TipoEvidenzaClinica T
													ON (M.CodTipoEvidenzaCLinica = T.Codice)																																								
							WHERE 
								IDPaziente=@uIDPaziente AND
								M.IDEpisodio=@uIDEpisodio AND
								M.CodStatoEvidenzaClinica NOT IN ('AN','CA') AND
								(@bModalita1SoloRange=0 OR
								 ( M.DataEvento >=@dDataOraInizio 
									AND   M.DataEvento<= @dDataOraFine 
								  )
								 )
							GROUP BY
								T.Codice	
						) AS Q		
						LEFT JOIN T_TipoEvidenzaClinica T
								ON (Q.CodVoce= T.Codice)	
						-- Icone		
						LEFT JOIN 
									(SELECT
									  IDNum AS IDIcona,
									  CodTipo,
									  CodStato					 
									 FROM T_Icone
									 WHERE CodEntita='EVC'
									) AS I
								ON (I.CodTipo=Q.CodVoce AND 	
								    I.CodStato=''
									)		
				END
			
				-- WKI	WorkList Infermieristica, modalità 1
				IF @bWKI=1 
				BEGIN
					INSERT INTO #tmpMovMod1(														
										CodSezione,
										CodVoce,
										DescrVoce,
										RTF,														
										IconaVoce,	
										IDIcona,							
										PermessoInserimento,								
										PermessoGrafico,
										IDEntita,
				   						Azione,
										OrdVoce)	
					SELECT
						'WKI' AS CodSezione,
						Q.CodVoce,
						T.Descrizione  AS DescrVoce,
						NULL AS RTF,
						NULL AS IconaVoce,
						I.IDIcona,
						@bWKIInserisci AS PermessoInserimento,			-- Peremsso di Modulo per Inserimento
						0 AS  PermessoGrafico,							-- Non ci sono grafici ADT
						NULL AS IDEntita,
				   		NULL AS Azione,
						T.Descrizione AS OrdVoce
					FROM
						  (							
							SELECT 														
								T.Codice AS CodVoce						-- Cod Tipo Task Infermieristico
							FROM 
								T_MovTaskInfermieristici	M								
									LEFT JOIN T_MovPazienti P
													ON (M.IDEpisodio = P.IDEpisodio)													
									LEFT JOIN T_TipoTaskInfermieristico T
													ON (M.CodTipoTaskInfermieristico = T.Codice)
							WHERE 
								IDPaziente=@uIDPaziente AND
								M.IDEpisodio=@uIDEpisodio AND
								M.CodStatoTaskInfermieristico NOT IN ('AN','CA','TR') AND
								
								-- Escludo i task di tipo prescrizione
								ISNULL(M.CodSistema,'') <> 'PRF' AND
								
								-- Prova filtro sui Dati
								(@bModalita1SoloRange=0 OR
								
									(CASE											
										WHEN M.CodStatoTaskInfermieristico ='PR'  THEN DataProgrammata
										ELSE ISNULL(DataErogazione,DataProgrammata)
									END >=@dDataOraInizio AND
					
									CASE											
										WHEN M.CodStatoTaskInfermieristico ='PR'  THEN DataProgrammata
										ELSE ISNULL(DataErogazione,DataProgrammata)
									END <= @dDataOraFine)
								)	
								
							GROUP BY
								T.Codice	
						) AS Q				
					LEFT JOIN T_TipoTaskInfermieristico T
								ON (Q.CodVoce = T.Codice)
					-- Icone		
						LEFT JOIN 
									(SELECT
									  IDNum AS IDIcona,
									  CodTipo,
									  CodStato					 
									 FROM T_Icone
									 WHERE CodEntita='WKI'
									) AS I
								ON (I.CodTipo=Q.CodVoce AND 	
								    I.CodStato=''
									)				
				END
				
				-- PFM	Farmaci, modalità 1
				
				-- Gestione personalizza
				IF @bPFM=1 
					BEGIN
							
						INSERT INTO #tmpMovMod1(														
										CodSezione,
										CodVoce,
										DescrVoce,
										RTF,														
										IconaVoce,	
										IDIcona,				
										PermessoDettaglio,			
										PermessoInserimento,								
										PermessoGrafico,
										PermessoSospendi,
										IDEntita,
										Azione,
										OrdVoce,
										DescrViaSomm,
										CodViaSomm)
						SELECT
							'PFM' AS CodSezione,				
							convert(varchar(600),Q.CodVoce) AS CodVoce,
							CASE 
								WHEN ISNULL(Q.Sottoclasse,'')='' THEN LEFT(ISNULL(MS.AnteprimaTXT,''),255)
								ELSE LEFT(Q.Sottoclasse + 
											CHAR(10) + CHAR(13) +
											 '----------------' 
											 + CHAR(10) + CHAR(13)
											 + ISNULL(MS.AnteprimaTXT,'')										 
										  ,255)
							END	 AS DescrVoce,	
							CASE 
								WHEN ISNULL(Q.Sottoclasse,'')='' 
									THEN MS.AnteprimaRTF
								ELSE 					
									MS.AnteprimaRTF
							END	 							
							AS RTF,
							NULL AS IconaVoce,
							I.IDIcona,
							1 AS  PermessoDettaglio,
							CASE 
								WHEN ISNULL(Q.Sottoclasse,'')='' AND ISNULL(MP.CodStatoContinuazione,'AP') <> 'CH' THEN @bPRFInserisci
								ELSE 0
							END	 
							 AS PermessoInserimento,
							0 AS  PermessoGrafico,							-- Non ci sono grafici 
							
							CASE 
								WHEN ISNULL(Q.Sottoclasse,'')='' THEN @bPRFValida
								ELSE 0
							END
							 AS PermessoSospendi,							-- Permesso di sospensioe							 
				   			CONVERT(VARCHAR(50),Q.IDEntita) AS IDEntita,	-- ID Prescrizione			   		 
							CASE
								WHEN ISNULL(MP.CodStatoContinuazione,'AP')='AP' AND @bPRFModifica=1 THEN 'C'
								WHEN ISNULL(MP.CodStatoContinuazione,'AP')='CH' AND @bPRFModifica=1 THEN 'A'
								ELSE ''
							END AS Azione,
							ISNULL(VS.Descrizione,'') + 							
								convert(varchar(600),Q.CodVoce) + ISNULL(Q.Sottoclasse,'') AS OrdVoce,
							VS.Descrizione as DescrCodViaSomministrazione,
							CodViaSomministrazione

						FROM									
							(SELECT 														
								CONVERT(VARCHAR(50),MPT.IDPrescrizione) + ISNULL(M.Sottoclasse,'') AS CodVoce,				-- ID Prescrizione + Sottoclasse
							    ISNULL(M.Sottoclasse,'') AS Sottoclasse,
							    MIN(Convert(VARCHAR(50),MPT.IDPrescrizione)) AS IDEntita
							FROM 
									T_MovTaskInfermieristici M								
									LEFT JOIN T_MovPazienti P
													ON (M.IDEpisodio = P.IDEpisodio)								
									INNER JOIN 
										T_MovPrescrizioniTempi MPT
											ON MPT.ID=M.IDGruppo
									LEFT JOIN 
										T_MovPrescrizioni MP	
											ON MP.ID=MPT.IDPrescrizione																				
							WHERE 
								IDPaziente=@uIDPaziente AND		
								M.IDEpisodio=@uIDEpisodio AND					
								M.CodStatoTaskInfermieristico NOT IN ('AN','CA','TR') AND
								
								-- includo SOLO i task di tipo prescrizione
								ISNULL(M.CodSistema,'') ='PRF' AND
								ISNULL(M.IDGruppo,'') <> '' AND
								
								-- escludo quelli "Al Bisogno"
								ISNULL(MPT.AlBisogno,0)=0  AND
								
								-- Prova filtro sui Dati
								(@bModalita1SoloRange=0 OR
								
									(
										-- Task Programmati o Erogati nel Range
										(	CASE											
												WHEN M.CodStatoTaskInfermieristico ='PR'  THEN DataProgrammata
												ELSE ISNULL(DataErogazione,DataProgrammata)
											END >=@dDataOraInizio 
										AND
						
											CASE											
												WHEN M.CodStatoTaskInfermieristico ='PR'  THEN DataProgrammata
												ELSE ISNULL(DataErogazione,DataProgrammata)
											END <= @dDataOraFine
										)
									
									-- Oppure prescrizione NON chiusa
									OR
										ISNULL(MP.CodStatoContinuazione,'') NOT IN ('CH')
									)	
								)	
								
																	
								AND 
									-- Filtro su Via Somministrazione	
									MP.CodViaSomministrazione IN (SELECT Codice FROM  #tmpViaSomministrazione)
							
							GROUP BY
								CONVERT(VARCHAR(50),MPT.IDPrescrizione) + ISNULL(M.Sottoclasse,''),
								ISNULL(M.Sottoclasse,'')
							) AS Q	
						LEFT JOIN 
								T_MovPrescrizioni MP	
									ON MP.ID=Q.CodVoce
						LEFT JOIN	
								-- Scheda della Prescrizione (non del Task Infermieristico)
								(SELECT IDEntita,AnteprimaTXT, AnteprimaRTF
								 FROM
									T_MovSchede 
								 WHERE CodEntita = 'PRF' AND							
							  		   Storicizzata = 0
								) AS MS
								ON MS.IDEntita = MP.ID							
						LEFT JOIN T_ViaSomministrazione VS
								ON VS.Codice=MP.CodViaSomministrazione	
						-- Icone		
						LEFT JOIN 
									(SELECT
									  IDNum AS IDIcona,
									  CodTipo,
									  CodStato					 
									 FROM T_Icone
									 WHERE CodEntita='VSM'
									) AS I
								ON (I.CodTipo=VS.Codice AND 	
								    I.CodStato=ISNULL(MP.CodStatoContinuazione,'AP')
									)		
					
							
										
				END
				
				-- PFA	Al Bisogno, modalità 1
				IF @bPFA=1 
					BEGIN
						-- In caso di PFA devo visualizzare tutte le prescrizioni tempi della cartella in oggetto										
											
						INSERT INTO #tmpMovMod1(														
										CodSezione,
										CodVoce,
										DescrVoce,
										RTF,														
										IconaVoce,
										IDIcona,				
										PermessoDettaglio,														
										PermessoInserimento,														
										PermessoGrafico,
										PermessoSospendi,
										IDEntita,
										Azione,
										OrdVoce,
										DescrViaSomm,
										CodViaSomm)	
										
						SELECT
							'PFA' AS CodSezione,		
							convert(varchar(50),CodVoce)	AS CodVoce,
							LEFT(ISNULL(MS.AnteprimaTXT,''),255)  AS DescrVoce,
							MS.AnteprimaRTF AS RTF,
							NULL AS IconaVoce,
							I.IDIcona,
							1 AS  PermessoDettaglio,
							CASE 
								WHEN ISNULL(MP.CodStatoContinuazione,'AP') <> 'CH' AND
									ISNULL(MPT.CodStatoPrescrizioneTempi,'')='VA' THEN @bWKIInserisci
								ELSE 0
							END	
							AS PermessoInserimento,			-- Peremsso di Modulo per Inserimento Task
						
							0 AS  PermessoGrafico,							-- Non ci sono grafici ADT
							
							CASE 
								WHEN ISNULL(MPT.CodStatoPrescrizioneTempi,'')='VA' THEN @bPRFValida
								ELSE 0
							END	
							AS PermessoSospendi,				-- Permesso di sospensioe
							MP.ID AS IDEntita,
							CASE
								WHEN ISNULL(MP.CodStatoContinuazione,'AP')='AP' AND @bPRFModifica=1 AND ISNULL(MPT.CodStatoPrescrizioneTempi,'')='VA' THEN 'C'
								WHEN ISNULL(MP.CodStatoContinuazione,'AP')='CH' AND @bPRFModifica=1 AND ISNULL(MPT.CodStatoPrescrizioneTempi,'')='VA' THEN 'A'
								ELSE ''
							END AS Azione,
							VS.Descrizione As OrdVoce,
							VS.Descrizione as DescrViaSomm,
							MP.CodViaSomministrazione						   
						FROM				
							(SELECT 															
								MPT.ID	AS CodVoce							-- ID Prescrizione 
							FROM 										
								T_MovPrescrizioniTempi MPT																
									INNER JOIN 
										T_MovPrescrizioni MP	
											ON MP.ID=MPT.IDPrescrizione				
									INNER JOIN 
										T_MovTrasferimenti TRA
											ON MP.IDTrasferimento=TRA.ID										
									LEFT JOIN
										T_MovTaskInfermieristici M										
											ON (
												-- Task di Tipo Prescrizione
												MPT.ID=CONVERT(UNIQUEIDENTIFIER,
														 CASE 
															WHEN ISNULL(M.IDGruppo,'')='' THEN NEWID() 
															ELSE IDGruppo
														 END			
															)
														
														AND
												ISNULL(M.CodSistema,'')='PRF' AND
												-- Attivi
												M.CodStatoTaskInfermieristico NOT IN ('AN','CA','TR') 
												)
									LEFT JOIN T_MovPazienti P
											ON (TRA.IDEpisodio = P.IDEpisodio)												
									LEFT JOIN T_TipoTaskInfermieristico T
											ON (M.CodTipoTaskInfermieristico = T.Codice)																																																			
							WHERE 
								IDPaziente=@uIDPaziente AND		
								MP.IDEpisodio=@uIDEpisodio AND
								
								-- includo SOLO quelli "Al Bisogno"
								ISNULL(MPT.AlBisogno,0)=1 AND
								
								-- Filtro Sulla Cartella 
								TRA.IDCartella=@uIDCartella AND							
								
								-- Prescrizione Tempi Validata
								MPT.CodStatoPrescrizioneTempi IN ('VA','SS')  AND
								
								-- Filtro su prescrizioni chiuse per Solo Range
								(@bModalita1SoloRange=0 OR
								 ISNULL(MP.CodStatoContinuazione,'AP') <> 'CH'
								)	
									 
														
							GROUP BY	
								MPT.ID
							) AS Q
							
						INNER JOIN	
							T_MovPrescrizioniTempi MPT	
								ON MPT.ID=Q.CodVoce
						INNER JOIN 
							T_MovPrescrizioni MP	
								ON MP.ID=MPT.IDPrescrizione	
						LEFT JOIN T_ViaSomministrazione VS
								ON VS.Codice=MP.CodViaSomministrazione			
						LEFT JOIN	
							-- Scheda della Prescrizione (non del Task Infermieristico)
								(SELECT IDEntita,AnteprimaTXT, AnteprimaRTF
								 FROM
									T_MovSchede 
								 WHERE CodEntita = 'PRF' AND							
									Storicizzata = 0
								) AS MS
							ON MS.IDEntita = MP.ID	
							-- Icone		
						LEFT JOIN 
									(SELECT
									  IDNum AS IDIcona,
									  CodTipo,
									  CodStato					 
									 FROM T_Icone
									 WHERE CodEntita='VSM'
									) AS I
								ON (I.CodTipo=VS.Codice AND 	
								    I.CodStato=ISNULL(MP.CodStatoContinuazione,'AP')
									)			
									
				END
						
				-- APP	Appuntamenti, modalità 1
				IF @bAPP=1
				BEGIN
					
					---------------------------------------------
					-- AGENDE DI TIPO EPISODIO modalità 1
					---------------------------------------------
					SET @sSQLQueryAgendeEPI=(SELECT dbo.MF_SQLQueryAgendeEPI())
									
					SET @sSQL='
					INSERT INTO #tmpMovMod1(														
										CodSezione,
										CodVoce,
										DescrVoce,
										RTF,														
										IconaVoce,	
										IDIcona,							
										PermessoInserimento,								
										PermessoGrafico,
										IDEntita,
										Azione,
										OrdVoce)				
					SELECT 					
						''APP'' AS CodSezione,			
						Q.CodVoce,
						AG.Descrizione  AS DescrVoce,
						NULL AS RTF,			
						NULL AS IconaVoce,		
						I.IDIcona,										
						' + CONVERT(CHAR(1),@bAPPInserisci) + ' AS PermessoInserimento,			-- Permesso di Modulo per Inserimento			   		
						0 AS  PermessoGrafico,													-- Non ci sono grafici 
						NULL AS IDEntita,
						NULL AS Azione,
						AG.Descrizione AS OrdVoce   
					FROM	
						(SELECT 												
							M.CodAgenda AS CodVoce
						 FROM 				
							T_MovAppuntamentiAgende M
							
								INNER JOIN T_MovAppuntamenti APP
									ON (M.IDAppuntamento=APP.ID) 
									
								LEFT JOIN 
										(' + @sSQLQueryAgendeEPI + ')  AS QEPI						
									ON QEPI.IDEpisodio=APP.IDEpisodio	 	
																			
								LEFT JOIN T_MovPazienti P
										ON (APP.IDEpisodio = P.IDEpisodio)
														
								LEFT JOIN T_TipoAppuntamento T
										ON (APP.CodTipoAppuntamento = T.Codice)													
							
								LEFT JOIN T_StatoAppuntamentoAgende STA
										ON M.CodStatoAppuntamentoAgenda=STA.Codice
							
								LEFT JOIN T_Agende AG
										ON M.CodAgenda=AG.Codice																														
						WHERE 
							P.IDPaziente=''' + CONVERT(VARCHAR(50),@uIDPaziente) +''' AND
							APP.CodStatoAppuntamento NOT IN (''AN'',''CA'') AND
							M.CodStatoAppuntamentoAgenda NOT IN (''AN'',''CA'') AND
							AG.CodEntita=''EPI'' AND
							(' + CONVERT(VARCHAR(1),@bModalita1SoloRange) + '=0 OR
								(APP.DataInizio >=CONVERT(Datetime,''' + CONVERT(VARCHAR(50), @dDataOraInizio,120) + ''',120) AND
								 APP.DataInizio <=CONVERT(Datetime,''' + CONVERT(VARCHAR(50), @dDataOraFine,120) + ''',120)  
								)
							)	
							
						GROUP BY 
							M.CodAgenda	
						) AS Q	
					LEFT JOIN T_Agende AG
						ON Q.CodVoce=AG.Codice	
					LEFT JOIN T_TipoAgenda TAG
						ON AG.CodTipoAgenda=TAG.Codice
					LEFT JOIN 
							(SELECT
							  IDNum AS IDIcona,
							  CodTipo,
							  CodStato					 
							 FROM T_Icone
							 WHERE CodEntita=''TAG''
							) AS I
							ON (I.CodTipo=AG.CodTipoAgenda AND 	
								    I.CodStato=''''
								)	
									
					 ' 					
					EXEC (@sSQL)

						
					-------------------------------
					-- AGENDE DI TIPO PAZIENTE  modalità 1 O SENZA TIPO
					-------------------------------
					-- come sopra, cambia la WHERE con PAZ e la vista per l'oggetto
					SET @sSQLQueryAgendePAZ=(SELECT dbo.MF_SQLQueryAgendePAZ())
								
					SET @sSQL='
					INSERT INTO #tmpMovMod1(														
										CodSezione,
										CodVoce,
										DescrVoce,
										RTF,														
										IconaVoce,
										IDIcona,								
										PermessoInserimento,								
										PermessoGrafico,
										IDEntita,
										Azione,
										OrdVoce)	
					SELECT 
						''APP'' AS CodSezione,
						Q.CodVoce,
						AG.Descrizione  AS DescrVoce,
						NULL AS RTF,										
						NULL AS IconaVoce,	
						I.IDIcona,						
						' + CONVERT(CHAR(1),@bAPPInserisci) + ' AS PermessoInserimento,			-- Permesso di Modulo per Inserimento			   		
						0 AS  PermessoGrafico,													-- Non ci sono grafici 
						NULL AS IDEntita,
						NULL AS Azione,
						AG.Descrizione As OrdVoce   
					FROM	
						(SELECT 											
							M.CodAgenda AS CodVoce						
						FROM 				
							T_MovAppuntamentiAgende M
								
								INNER JOIN T_MovAppuntamenti APP
									ON (M.IDAppuntamento=APP.ID) 
								LEFT JOIN 
										(' + @sSQLQueryAgendePAZ + ')  AS QPAZ						
									ON QPAZ.IDPaziente=APP.IDEpisodio	 																						
														
								LEFT JOIN T_TipoAppuntamento T
											ON (APP.CodTipoAppuntamento = T.Codice)
										LEFT JOIN T_StatoAppuntamento S
											ON (APP.CodStatoAppuntamento = S.Codice)
								
								LEFT JOIN T_Agende AG
											ON M.CodAgenda=AG.Codice	
																																				
						WHERE 
							APP.IDPaziente=''' + CONVERT(VARCHAR(50),@uIDPaziente) +''' AND
							APP.CodStatoAppuntamento NOT IN (''AN'',''CA'') AND
							M.CodStatoAppuntamentoAgenda NOT IN (''AN'',''CA'') AND
							ISNULL(AG.CodEntita,''PAZ'') =''PAZ'' AND
							(' + CONVERT(VARCHAR(1),@bModalita1SoloRange) + '=0 OR
								(APP.DataInizio >=CONVERT(Datetime,''' + CONVERT(VARCHAR(50), @dDataOraInizio,120) + ''',120) AND
								 APP.DataInizio <=CONVERT(Datetime,''' + CONVERT(VARCHAR(50), @dDataOraFine,120) + ''',120)  
								)
							)	
							
						GROUP BY
							M.CodAgenda
					) AS Q
					LEFT JOIN T_Agende AG
						ON Q.CodVoce=AG.Codice		 
					LEFT JOIN T_TipoAgenda TAG
						ON AG.CodTipoAgenda=TAG.Codice	
					LEFT JOIN 
							(SELECT
							  IDNum AS IDIcona,
							  CodTipo,
							  CodStato					 
							 FROM T_Icone
							 WHERE CodEntita=''TAG''
							) AS I
							ON (I.CodTipo=AG.CodTipoAgenda AND 	
								    I.CodStato=''''
								)		
					 ' 							
					EXEC (@sSQL)
				END
						
				-- PVT	Parametri Vitali, modalità 1
				IF @bPVT=1 
				BEGIN
					
					INSERT INTO #tmpMovMod1(														
										CodSezione,
										CodVoce,
										DescrVoce,
										RTF,														
										IconaVoce,
										IDIcona,								
										PermessoInserimento,								
										PermessoGrafico,
										IDEntita,
										Azione,
										OrdVoce)	
					SELECT 
						'PVT' AS CodSezione,
						Q.CodVoce,
						T.Descrizione AS DescrVoce,		
						NULL AS RTF,				
						NULL AS IconaVoce,	
						IDIcona,					
						@bPVTInserisci AS PermessoInserimento,					-- Permesso di Modulo per Inserimento
						@bPVTVisualizza AS  PermessoGrafico,						-- Permesso di Modulo per Visualizzazione 
						NULL AS IDEntita,
						NULL AS Azione,
						T.Descrizione AS OrdVoce
					FROM																
						(SELECT 														
							T.Codice AS CodVoce									-- Cod Tipo Parametro Vitale
		   				FROM 
							T_MovParametriVitali	M	
									LEFT JOIN T_MovPazienti P
										ON (M.IDEpisodio=P.IDEpisodio)	
									LEFT JOIN T_TipoParametroVitale T
												ON (M.CodTipoParametroVitale=T.Codice)
						WHERE 
							P.IDPaziente=@uIDPaziente and
							M.IDEpisodio=@uIDEpisodio AND
							M.CodStatoParametroVitale NOT IN ('AN','CA')
							AND
							(@bModalita1SoloRange=0 OR
							  (M.DataEvento >= @dDataOraInizio  AND M.DataEvento <= @dDataOraFine)
							 )	
							
						GROUP BY
							T.Codice	
						) AS Q
					LEFT JOIN T_TipoParametroVitale T
						ON (Q.CodVoce=T.Codice)		
					LEFT JOIN	
						(SELECT
							  IDNum AS IDIcona,
							  CodTipo,
							  CodStato					 
							 FROM T_Icone
							 WHERE CodEntita='PVT'
							) AS I
							ON (I.CodTipo=Q.CodVoce AND 	
								  I.CodStato=''
								)			
				END	
							
				-- OE	Order Entry, modalità 1
				IF @bOE=1 
				BEGIN				
					INSERT INTO #tmpMovMod1(														
										CodSezione,
										CodVoce,
										DescrVoce,
										RTF,														
										IconaVoce,
										IDIcona,								
										PermessoInserimento,								
										PermessoGrafico,
										IDEntita,
										Azione,
										OrdVoce)	
					SELECT 
						'OE' AS CodSezione,			
						Q.CodVoce,
						T.Descrizione  AS DescrVoce,	
						NULL AS RTF,									
						NULL AS IconaVoce,
						I.IDIcona,							
						@bOEInserisci AS PermessoInserimento,			-- Peremsso di Modulo per Inserimento			   				   			
		   				0 AS  PermessoGrafico,							-- Non ci sono grafici ADT
		   				NULL AS IDEntita,
						NULL AS Azione,
						T.Descrizione AS OrdVoce   
					FROM									
						(SELECT 					
							T.Codice AS CodVoce							-- Cod Tipo Ordine						
						 FROM 
							T_MovOrdini	M														
								INNER JOIN T_MovOrdiniEroganti ME
									ON (M.ID=ME.IDOrdine)						
								LEFT JOIN T_TipoOrdine T
									ON (ME.CodTipoOrdine = T.Codice)							
						WHERE 
							M.IDPaziente=@uIDPaziente AND
							M.IDEpisodio=@uIDEpisodio AND
							M.CodStatoOrdine NOT IN ('CA') AND
							(@bModalita1SoloRange=0 OR
								(ISNULL(M.DataProgrammazioneOE,M.DataInoltro) >=@dDataOraInizio 
									AND    ISNULL(M.DataProgrammazioneOE,M.DataInoltro) <= @dDataOraFine 
								)
							)	
						
						GROUP BY 	
							T.Codice
					) AS Q			
					LEFT JOIN T_TipoOrdine T
						ON (Q.CodVoce = T.Codice)
					LEFT JOIN	
						(SELECT
							  IDNum AS IDIcona,
							  CodTipo,
							  CodStato					 
							 FROM T_Icone
							 WHERE CodEntita='OE'
							) AS I
							ON (I.CodTipo=Q.CodVoce AND 	
								  I.CodStato=''
								)	
				END

				-- NTG Note Generali
				IF @bNTG=1 
				BEGIN				
					-- Elenco 
					INSERT INTO #tmpMovMod1(												
								CodSezione,
								CodVoce,	
								DescrVoce,						
								RTF,																			
								IconaVoce,	
								IDIcona,							
								PermessoInserimento,							
								PermessoGrafico,	
								IDEntita,
								Azione,							
								OrdVoce)								
				
					SELECT 
						 'NTG' AS CodSezione,
						 'N' AS CodVoce,
						 'Note' AS DescrVoce,
				 		 NULL AS Rtf,
				 		 NULL AS IconaVoce,		   				 			 
						 I.Icona AS IDIcona,
						 @bNTGInserisci AS PermessoInserimento,			-- Non si inseriscono mai ADT
				   		 0 AS  PermessoGrafico,							-- Non ci sono grafici ADT
				   		 NULL AS IDEntita,
						 NULL AS Azione,
						 NULL AS OrdVoce
				    FROM
				    		
								(SELECT 
									  IDNum AS Icona
								 FROM T_Icone
								 WHERE CodEntita='FUT' 
									   AND CodTipo='NTG') AS I
								 					 		
				END
		----------------------------------------------------------------------------------------------------------
		-- Modalita = 1, Ritorno elenco righe
		----------------------------------------------------------------------------------------------------------		

				CREATE TABLE #tmpMovMod1Sep
				(													
					ID INTEGER IDENTITY(1,1),	
					CodSezioneOrig VARCHAR(20) COLLATE Latin1_General_CI_AS,				
					CodSezione VARCHAR(20) COLLATE Latin1_General_CI_AS,
					CodVoce VARCHAR(600) COLLATE Latin1_General_CI_AS,
					DescrVoce VARCHAR(255) COLLATE Latin1_General_CI_AS,
					RTF VARCHAR(MAX) COLLATE Latin1_General_CI_AS,				
					IconaVoce  VARbinary(MAX),					
					IDIcona INTEGER,
					PermessoDettaglio INTEGER,
					PermessoInserimento INTEGER,
					PermessoGrafico INTEGER,				
					PermessoSospendi INTEGER,	
					IDEntita VARCHAR(50),
					Azione VARCHAR(5) COLLATE Latin1_General_CI_AS,	
					OrdineSez numeric(18, 0),
					TipoSepSezione varchar(50),
					TipoSepViaSomm varchar(50),
					OrdVoce VARCHAR(MAX) COLLATE Latin1_General_CI_AS,
					OrdVoce1 INTEGER DEFAULT(1)
				)	

				IF @bSeparaSezioni = 1 
				BEGIN
					INSERT INTO #tmpMovMod1Sep
					(
						CodSezioneOrig,
						CodSezione,
						CodVoce,
						DescrVoce,
						RTF,
						IconaVoce,
						IDIcona,
						PermessoDettaglio, 
						PermessoInserimento,
						PermessoGrafico,
						PermessoSospendi,
						IDEntita,
						Azione,
						OrdineSez,
						TipoSepSezione,
						ORdVoce
					)		
					SELECT 
						CodSezione as CodSezioneOrig, 												
						'BLK' as CodSezione,
						'' as CodVoce,							
						UPPER(S.Descrizione) AS DescrVoce,		
						NULL AS RTF,
						NULL AS IconaVoce,
						NULL AS IDIcona,
						0 as PermessoDettaglio, 
						0 as PermessoInserimento,
						0 as PermessoGrafico,
						0 as PermessoSospendi,
						NULL AS IDEntita,
						NULL AS Azione,
						S.Ordine as OrdineSez,
						CodSezione + '_0' as TipoSepSezione,
						0 AS ORdVoce						
					FROM #tmpMovMod1
						LEFT JOIN T_SezioniFUT S
							ON #tmpMovMod1.CodSezione=S.Codice
					WHERE #tmpMovMod1.CodSezione NOT IN ('NTG','ADT')		
					GROUP BY #tmpMovMod1.CodSezione, S.Descrizione, S.Ordine
						
					
				END				


				IF @bSeparaVieSomm = 1 
				BEGIN
					INSERT INTO #tmpMovMod1Sep
					(
						CodSezioneOrig,
						CodSezione,
						CodVoce,
						DescrVoce,
						RTF,
						IconaVoce,
						IDIcona,
						PermessoDettaglio, 
						PermessoInserimento,
						PermessoGrafico,
						PermessoSospendi,
						IDEntita,
						Azione,
						OrdineSez,
						TipoSepSezione,
						ORdVoce
					)		
					SELECT 
						CodSezione as CodSezioneOrig, 												
						'BLKV' as CodSezione,
						'' as CodVoce,
						UPPER(' - ' + DescrViaSomm) AS DescrVoce,		
						NULL AS RTF,
						NULL AS IconaVoce,
						NULL AS IDIcona,
						--MIN(IDIcona) AS IDIcona,
						0 as PermessoDettaglio, 
						0 as PermessoInserimento,
						0 as PermessoGrafico,
						0 as PermessoSospendi,
						NULL AS IDEntita,
						NULL AS Azione,
						S.Ordine as OrdineSez,
						CodSezione + '_1' as TipoSepSezione,
						ISNULL(DescrViaSomm, '') + '_' + ISNULL(CodViaSomm, '') AS ORdVoce						
					FROM #tmpMovMod1
						LEFT JOIN T_SezioniFUT S
							ON #tmpMovMod1.CodSezione=S.Codice
					WHERE #tmpMovMod1.CodSezione IN ('PFM')		
					GROUP BY #tmpMovMod1.CodSezione, DescrViaSomm, CodViaSomm, S.Ordine
										

					INSERT INTO #tmpMovMod1Sep
					(
						CodSezioneOrig,
						CodSezione,
						CodVoce,
						DescrVoce,
						RTF,
						IconaVoce,
						IDIcona,
						PermessoDettaglio, 
						PermessoInserimento,
						PermessoGrafico,
						PermessoSospendi,
						IDEntita,
						Azione,
						OrdineSez,
						TipoSepSezione,
						ORdVoce,
						OrdVoce1
					)		
					SELECT 
						CodSezione as CodSezioneOrig, 												
						'BLKV' as CodSezione,
						'' as CodVoce,
						UPPER(' - ' + DescrViaSomm) AS DescrVoce,		
						NULL AS RTF,
						NULL AS IconaVoce,
						NULL AS IDIcona,
						--MIN(IDIcona) AS IDIcona,
						0 as PermessoDettaglio, 
						0 as PermessoInserimento,
						0 as PermessoGrafico,
						0 as PermessoSospendi,
						NULL AS IDEntita,
						NULL AS Azione,
						S.Ordine as OrdineSez,
						CodSezione + '_1' as TipoSepSezione,
						ISNULL(DescrViaSomm, '') AS ORdVoce,
						0 AS OrdVoce1					
					FROM #tmpMovMod1
						LEFT JOIN T_SezioniFUT S
							ON #tmpMovMod1.CodSezione=S.Codice
					WHERE #tmpMovMod1.CodSezione IN ('PFA')		
					GROUP BY #tmpMovMod1.CodSezione, DescrViaSomm, CodViaSomm, S.Ordine					
				END				
								
				UPDATE #tmpMovMod1Sep SET 	CodVoce = CONVERT(varchar, #tmpMovMod1Sep.ID) WHERE ISNULL(#tmpMovMod1Sep.CodVoce, '') = ''

				INSERT INTO #tmpMovMod1Sep
				(
					CodSezioneOrig,
					CodSezione,
					CodVoce,
					DescrVoce,
					RTF,
					IconaVoce,
					IDIcona,
					PermessoDettaglio, 
					PermessoInserimento,
					PermessoGrafico,
					PermessoSospendi,
					IDEntita,
					Azione,
					OrdineSez,
					TipoSepSezione,
					ORdVoce
				)

				SELECT  		
						CodSezione,				
						CodSezione,
						CodVoce,							
						DescrVoce AS DescrVoce,		
						RTF AS RTF,
						NULL AS IconaVoce,
						IDIcona,
						PermessoDettaglio,
						PermessoInserimento AS PermessoInserimento,
						PermessoGrafico AS PermessoGrafico,
						PermessoSospendi,
						IDEntita,
						ISNULL(Azione,'') AS Azione,
						S.Ordine as OrdineSez,
						#tmpMovMod1.CodSezione + '_1' as TipoSepSezione,
						OrdVoce As OrdVoce							
				FROM #tmpMovMod1
					LEFT JOIN T_SezioniFUT S
						ON #tmpMovMod1.CodSezione=S.Codice											 									   		

			
			SELECT  		
--			 OrdineSez, TipoSepSezione, OrdVoce, Ordvoce1,				
						CodSezione + '_' + CodVoce AS CodAgenda,
						CASE 
							WHEN CodSezione='BLK' THEN 'Color Color [A=255, R=211, G=211, B=211]'
							WHEN CodSezione='BLKV' THEN 'Color Color [A=255, R=211, G=211, B=211]'
							ELSE S.Colore  
						END AS ColoreAgenda,
						NULL As IconaAgenda,
						--CONVERT(VARBINARY(NULL)) AS IconaAgenda,
						IDIcona,
						CodEntita AS CodEntita,
						CodSezione,
						CodVoce,							
						DescrVoce AS DescrVoce,		
						RTF AS RTF,
						LEFT(ISNULL(DescrVoce,''),@nNumMaxCaratteri) AS DescrVoceCorta,
						ISNULL(PermessoDettaglio,0) AS PermessoDettaglio,
						PermessoInserimento AS PermessoInserimento,
						PermessoGrafico AS PermessoGrafico,
						ISNULL(PermessoSospendi,0) AS PermessoSospendi,
						IDEntita,
						ISNULL(Azione,'') AS Azione,
						OrdVoce As OrdVoce			
				FROM #tmpMovMod1Sep
					LEFT JOIN T_SezioniFUT S
						ON #tmpMovMod1Sep.CodSezione=S.Codice
				--GROUP BY CodSezione, CodVoce,S.ordine
				--ORDER BY S.Ordine, TipoSezione, OrdVoce
				ORDER BY OrdineSez, TipoSepSezione, OrdVoce, OrdVoce1
				
				
			--select * from #tmpMovMod1					

			DROP TABLE 	#tmpMovMod1
			DROP TABLE 	#tmpMovMod1Sep
							
		END  -- Fine IF modalità =1	 
		   
	    ELSE
			---------------------------------------------------------------
			---------------------------------------------------------------		
			---------------------------------------------------------------
			-- Modalita = 2, elenco movimenti
			---------------------------------------------------------------
			---------------------------------------------------------------
			---------------------------------------------------------------
		
			IF @nModalita =2
				BEGIN										
					
					-- Crea tabella temporanea per risultati di Output			
					CREATE TABLE #tmpMov
					(				
						IDRiferimento VARCHAR(50),	
						CodSezione VARCHAR(20) COLLATE Latin1_General_CI_AS,
						CodVoce VARCHAR(600) COLLATE Latin1_General_CI_AS,	
						IDOrdinamento VARCHAR(50) COLLATE Latin1_General_CI_AS,			
						DataOraInizio  DATETIME,
						DataOraFine DATETIME,								
						Valore  VARCHAR(4000) COLLATE Latin1_General_CI_AS,
						IconaValore  VARBINARY(MAX),
						ColoreValore  VARCHAR(50) COLLATE Latin1_General_CI_AS,
						PermessoInserimento INTEGER,
						PermessoModifica INTEGER,
						PermessoVisualizza INTEGER,
						PermessoGrafico INTEGER,
						PermessoEroga INTEGER,
						PermessoAnnulla INTEGER,
						PermessoCancella  INTEGER,
						PermessoCambiaStato  INTEGER				
					)			
							
					---------------------------------------------------------------
					-- Modalita = 2, elenco movimenti
					---------------------------------------------------------------
					
						-- ADT	Eventi ADT
						-- DCM	Diario Medico
						-- DCI	Diario Infermieristico
						-- EVC	Evidenza Clinica
						-- WKI	WorkList Infermieristica
						-- PFM	Farmaci
						-- PFA	Al Bisogno
						-- APP	Appuntamenti
						-- PVT	Parametri Vitali
						-- OE	Order Entry
						-- NTG	Note Generali

					-- ADT	Eventi ADT, modalità 2				
					IF @bADT=1 
					BEGIN
						-- Elenco 
						INSERT INTO #tmpMov(
									IDRiferimento,	
									CodSezione,
									CodVoce,		
									IDOrdinamento,											
									DataOraInizio,
									DataOraFine,							
									Valore,													
									IconaValore,
									ColoreValore,	
									PermessoInserimento,
									PermessoModifica,
									PermessoVisualizza,
									PermessoGrafico,
									PermessoEroga,
									PermessoAnnulla,
									PermessoCancella,
									PermessoCambiaStato				
									)								
						SELECT 				   			   
						   M.ID AS IDRiferimento,		
							 'ADT' AS CodSezione,				
						   CASE 
							   WHEN M.DataIngresso=E.DataRicovero THEN 'T'
							   WHEN M.DataIngresso=E.DataDimissione THEN 'T'
							   ELSE 'T'
						   END AS CodVoce,		   		
						   NULL AS IDOrdinamento,		   				   
						   DataIngresso AS DataOraInizio,
						   DataIngresso AS DataOraFine,				 				   
						   CASE 
							   WHEN M.DataIngresso=E.DataRicovero THEN 'Accettazione'
							   WHEN M.DataIngresso=E.DataDimissione THEN 'Dimissione'
							   ELSE 'Trasferimento'
						   END AS Valore,
						   NULL AS IconaValore,
						   'Color [A=255, R=84, G=139, B=212]' AS ColoreValore,
							0 AS PermessoInserimento,			-- Non si inseriscono mai ADT
							0 AS PermessoModifica,				-- Non si modifica mai ADT
							1 AS PermessoVisualizza,			-- Si visualizzao mai ADT
							0 AS  PermessoGrafico,				-- Non ci sono grafici ADT										
							0 AS PermessoEroga,					-- Non si erogano 
							0 AS PermessoAnnulla,				-- Non si annullano
							0 AS PermessoCancella,				-- Non si cancella
							0 AS PermessoCambiaStato			-- Non si cambia lo stato
						FROM T_MovTrasferimenti M
							LEFT JOIN T_MovPazienti P
								ON P.IDEpisodio=M.IDEpisodio
							LEFT JOIN T_MovEpisodi E
								ON M.IDEpisodio=E.ID
							LEFT JOIN T_UnitaAtomiche A
								ON M.CodUA=A.Descrizione							
						WHERE				
							P.IDPaziente=@uIDPaziente AND
							M.IDEpisodio=@uIDEpisodio AND
							E.CodStatoEpisodio NOT IN ('CA','AN') AND
							DataIngresso >= @dDataOraInizio  AND DataIngresso <= @dDataOraFine
					END
						
					---- DCM	Diario Medico, modalità 2
					IF @bDCM=1
					BEGIN
							
							INSERT INTO  #tmpMov(
												IDRiferimento,	
												CodSezione,
												CodVoce,		
												IDOrdinamento,											
												DataOraInizio,
												DataOraFine,							
												Valore,													
												IconaValore,
												ColoreValore,	
												PermessoInserimento,
												PermessoModifica,
												PermessoVisualizza,
												PermessoGrafico,
												PermessoEroga,
												PermessoAnnulla,
												PermessoCancella,
												PermessoCambiaStato)
							SELECT 
							M.ID AS IDRiferimento,
							'DCM' AS CodSezione,			
							T.Codice AS CodVoce,		
							NULL AS IDOrdinamento,											
							DataEvento AS DataOraInizio,
							DataEvento AS DataOraFine,										
							L.Descrizione AS Valore,		
							I.Icona AS IconaValore,	
							S.Colore AS ColoreValore,	
			   				@bDCMInserisci AS PermessoInserimento,			-- Peremsso di Modulo per Inserimento			   		
			   				0 AS PermessoModifica,							-- Il Diario Clinico NON si modifica dal FUT
							@bDCMVisualizza AS  PermessoVisualizza,			-- Permesso di Modulo per Visualizzazione 
							0 AS  PermessoGrafico,							-- Non ci sono grafici Diario		
							0 AS PermessoEroga,								-- Non si erogano 
							0 AS PermessoAnnulla,							-- Non si annullano
							0 AS PermessoCancella,							-- Non si cancella			
							0 AS PermessoCambiaStato			-- Non si cambia lo stato
						FROM 
								T_MovDiarioClinico	M								
								LEFT JOIN T_MovPazienti P
												ON (M.IDEpisodio = P.IDEpisodio)
															
								LEFT JOIN T_TipoVoceDiario T
												ON (M.CodTipoVoceDiario = T.Codice)									
								LEFT JOIN T_StatoDiario S
												ON (M.CodStatoDiario = S.Codice)
								LEFT JOIN 
										(SELECT 
											  CodTipo,
											  CodStato,
											  Icona32 AS Icona
										 FROM T_Icone
										 WHERE CodEntita='DCL'
										) AS I
									ON (M.CodTipoDiario= I.CodTipo AND 	
										M.CodStatoDiario = I.CodStato
										)												
								LEFT JOIN T_Login L
										ON M.CodUtenteRilevazione =L.Codice		
																			
						WHERE 
							IDPaziente=@uIDPaziente AND
							M.IDEpisodio=@uIDEpisodio AND
							M.CodStatoDiario IN ('VA') AND				-- Solo Validate
							T.CodTipoDiario='M'	AND						-- Diario Medico
							M.CodTipoRegistrazione='M'					-- Tipo Registrazione Manuale
							AND DataEvento >=@dDataOraInizio 
							AND DataEvento <= @dDataOraFine 			
					
					END
							
					-- DCI	Diario Infermieristico, modalità 2
					IF @bDCI=1			
						BEGIN
								INSERT INTO #tmpMov(
												IDRiferimento,	
												CodSezione,
												CodVoce,		
												IDOrdinamento,											
												DataOraInizio,
												DataOraFine,							
												Valore,													
												IconaValore,
												ColoreValore,	
												PermessoInserimento,
												PermessoModifica,
												PermessoVisualizza,
												PermessoGrafico,
												PermessoEroga,
												PermessoAnnulla,
												PermessoCancella,
												PermessoCambiaStato)
								SELECT 
										M.ID AS IDRiferimento,
										'DCI' AS CodSezione,			
										T.Codice AS CodVoce,		
										NULL AS IDOrdinamento,													
										DataEvento AS DataOraInizio,
										DataEvento AS DataOraFine,																		
										L.Descrizione AS Valore,		
										I.Icona AS IconaValore,	
										S.Colore AS ColoreValore,	
			   							@bDCIInserisci AS PermessoInserimento,			-- Peremsso di Modulo per Inserimento			   			
			   							0 AS PermessoModifica,							-- Il Diario Clinico NON si modifica dal FUT
										@bDCIVisualizza AS  PermessoVisualizza,			-- Permesso di Modulo per Visualizzazione 
										0 AS  PermessoGrafico,							-- Non ci sono grafici Diario						  
										0 AS PermessoEroga,								-- Non si erogano 
										0 AS PermessoAnnulla,							-- Non si annullano
										0 AS PermessoCancella,							-- Non si cancella		
										0 AS PermessoCambiaStato						-- Non si cambia lo stato
							FROM 
									T_MovDiarioClinico	M								
									LEFT JOIN T_MovPazienti P
													ON (M.IDEpisodio = P.IDEpisodio)														
									LEFT JOIN T_TipoVoceDiario T
													ON (M.CodTipoVoceDiario = T.Codice)
												
									LEFT JOIN T_StatoDiario S
													ON (M.CodStatoDiario = S.Codice)
									LEFT JOIN 
											(SELECT 
												  CodTipo,
												  CodStato,
												  Icona32 AS Icona
											 FROM T_Icone
											 WHERE CodEntita='DCL'
											) AS I
										ON (M.CodTipoDiario= I.CodTipo AND 	
											M.CodStatoDiario = I.CodStato
											)					
									LEFT JOIN 
												(SELECT IDEntita,AnteprimaTXT
												 FROM
													T_MovSchede 
												 WHERE CodEntita = 'DCL' AND							
													Storicizzata = 0
												) AS MS
											ON MS.IDEntita = M.ID	
									 LEFT JOIN T_Login L
										ON M.CodUtenteRilevazione =L.Codice					
																				
							WHERE 
								IDPaziente=@uIDPaziente AND
								M.IDEpisodio=@uIDEpisodio AND
								M.CodStatoDiario IN ('VA')	AND				-- Non annullato e non cancellato
								T.CodTipoDiario='I'	AND						-- Diario Infermieristico
								M.CodTipoRegistrazione='M'					-- Tipo Registrazione Manuale
								AND DataEvento >=@dDataOraInizio 
								AND DataEvento <= @dDataOraFine 
					END
					
					-- EVC	Evidenza Clinica, modalità 2
					IF @bEVC=1
					BEGIN
						INSERT INTO #tmpMov(
											IDRiferimento,	
											CodSezione,
											CodVoce,	
											IDOrdinamento,												
											DataOraInizio,
											DataOraFine,							
											Valore,													
											IconaValore,
											ColoreValore,	
											PermessoInserimento,
											PermessoModifica,
											PermessoVisualizza,
											PermessoGrafico,
											PermessoEroga,
											PermessoAnnulla,
											PermessoCancella,
											PermessoCambiaStato
											)			
						SELECT 
							M.ID AS IDRiferimento,
							'EVC' AS CodSezione,			
							T.Codice AS CodVoce,		
							NULL AS IDOrdinamento,	
							DataEvento AS DataOraInizio,
							DataEvento AS DataOraFine,
							S.Descrizione AS Valore,		
							I.Icona AS IconaValore,	
							S.Colore AS ColoreValore,	
			   				@bOEInserisci AS PermessoInserimento,					-- Peremsso di Modulo per Inserimento =0 (si fa un nuovo ordine)
			   				0 AS PermessoModifica,									-- Permesso di Modulo per Modifica (0)			   		
							@bEVCVisualizza AS  PermessoVisualizza,					-- Permesso di Modulo per Visualizzazione 
							0 AS  PermessoGrafico,									-- Non ci sono grafici 
							0 AS PermessoEroga,										-- Non si erogano 
							0 AS PermessoAnnulla,									-- Non si annullano
							0 AS PermessoCancella,									-- Non si cancella		
							0 AS PermessoCambiaStato								-- Non si cambia lo stato
						FROM 
								T_MovEvidenzaClinica	M								
								LEFT JOIN T_MovPazienti P
												ON (M.IDEpisodio = P.IDEpisodio)
															
								LEFT JOIN T_TipoEvidenzaClinica T
												ON (M.CodTipoEvidenzaCLinica = T.Codice)
											LEFT JOIN T_StatoEvidenzaClinica S
												ON (M.CodStatoEvidenzaClinica = S.Codice)
								LEFT JOIN 
										(SELECT 
											  CodTipo,
											  CodStato,
											  Icona32 AS Icona
										 FROM T_Icone
										 WHERE CodEntita='EVC'
										) AS I
									ON (M.CodTipoEvidenzaCLinica = I.CodTipo AND 	
										M.CodStatoEvidenzaClinica = I.CodStato
										)												
																			
						WHERE 
							IDPaziente=@uIDPaziente AND
							M.IDEpisodio=@uIDEpisodio AND
							M.CodStatoEvidenzaClinica NOT IN ('AN','CA') 				
							AND DataEvento >=@dDataOraInizio 
							AND DataEvento <= @dDataOraFine 								
					END

					-- WKI	WorkList Infermieristica, modalità 2
					IF @bWKI=1 
					BEGIN
						INSERT INTO #tmpMov(
											IDRiferimento,	
											CodSezione,
											CodVoce,	
											IDOrdinamento,												
											DataOraInizio,
											DataOraFine,							
											Valore,													
											IconaValore,
											ColoreValore,	
											PermessoInserimento,
											PermessoModifica,
											PermessoVisualizza,
											PermessoGrafico,
											PermessoEroga,
											PermessoAnnulla,
											PermessoCancella,
											PermessoCambiaStato
											)					
						SELECT 
							M.ID AS IDRiferimento,
							'WKI' AS CodSezione,			
							T.Codice AS CodVoce,			
							NULL AS IDOrdinamento,													
							ISNULL(DataErogazione,DataProgrammata) AS DataOraInizio,
							ISNULL(DataErogazione,DataProgrammata) AS DataOraFine,										
							ISNULL(T.Sigla,'') AS Valore,
							I.Icona AS IconaValore,	
							S.Colore AS ColoreValore,	
			   				@bWKIInserisci AS PermessoInserimento,			-- Peremsso di Modulo per Inserimento					   		
			   				CASE											-- Permesso di Modifica valido solo su Task ancora da Erogare
			   					WHEN M.CodStatoTaskInfermieristico='PR' THEN @bWKIModifica
			   					ELSE 0
			   				END
			   				AS PermessoModifica,							-- Permesso di Modulo per Modifica			   		
			   				CASE											-- Permesso di Visualizzazione solo se Erogato o Annullato o Trascritto
			   					WHEN M.CodStatoTaskInfermieristico IN ('ER','AN','TR') THEN @bWKIVisualizza
			   					ELSE 0
			   				END			   				
							AS  PermessoVisualizza,			-- Permesso di Modulo per Visualizzazione 
							0 AS  PermessoGrafico,							-- Non ci sono grafici							
							CASE											-- Permesso di Erogazione valido solo su Task ancora da Erogare
			   					WHEN M.CodStatoTaskInfermieristico='PR' THEN @bWKICompleta
			   					ELSE 0
			   				END AS PermessoEroga,		
			   				
			   				CASE											-- Permesso di Annullamento valido solo su Task ancora da Erogare
			   					WHEN M.CodStatoTaskInfermieristico='PR' THEN @bWKIAnnulla
			   					ELSE 0
			   				END AS PermessoAnnulla,		
			   				
			   				CASE											-- Permesso di Cancellazione valido solo su Task ancora da Erogare e Registrazioni NON automatiche
			   					WHEN M.CodStatoTaskInfermieristico='PR' AND	
			   						 ISNULL(M.CodTipoRegistrazione,'') NOT IN ('A') THEN @bWKICancella
			   					ELSE 0
			   				END AS PermessoCancella,
			   				0 AS PermessoCambiaStato						-- Non si cambia lo stato		
			   										
						FROM 
								T_MovTaskInfermieristici	M								
								LEFT JOIN T_MovPazienti P
												ON (M.IDEpisodio = P.IDEpisodio)
															
								LEFT JOIN T_TipoTaskInfermieristico T
												ON (M.CodTipoTaskInfermieristico = T.Codice)
											LEFT JOIN T_StatoTaskInfermieristico S
												ON (M.CodStatoTaskInfermieristico = S.Codice)
								LEFT JOIN 
										(SELECT 
											  CodTipo,
											  CodStato,
											  Icona32 AS Icona
										 FROM T_Icone
										 WHERE CodEntita='WKI'
										) AS I
									ON (M.CodTipoTaskInfermieristico = I.CodTipo AND 	
										M.CodStatoTaskInfermieristico = I.CodStato
										)												
																			
						WHERE 
							IDPaziente=@uIDPaziente AND
							M.IDEpisodio=@uIDEpisodio AND
							M.CodStatoTaskInfermieristico NOT IN ('AN','CA','TR') AND
							
							-- Escludo i task di tipo prescrizione
							ISNULL(M.CodSistema,'') <> 'PRF' 
							AND ISNULL(DataErogazione,DataProgrammata) >=@dDataOraInizio 
							AND ISNULL(DataErogazione,DataProgrammata) <= @dDataOraFine 					
					END
								
					-- PFM	Farmaci, modalità 2
					IF @bPFM=1 
						BEGIN
										
							-- Leggo colori e caratteri dalla tabella di configurazione						
							SET @sColoreUltimaSomministrazioneProgrammata=(SELECT TOP 1 Valore FROM T_Config WITH (NOLOCK) WHERE ID=56)
							SET @sColoreUltimaSomministrazioneProgrammata=ISNULL(@sColoreUltimaSomministrazioneProgrammata,'')
							
							SET @sCarattereUltimaSomministrazioneProgrammata=(SELECT TOP 1 Valore FROM T_Config WITH (NOLOCK) WHERE ID=57)
							SET @sCarattereUltimaSomministrazioneProgrammata=ISNULL(@sCarattereUltimaSomministrazioneProgrammata,'')
							
							SET @sColoreUltimaSomministrazioneErogata=(SELECT TOP 1 Valore FROM T_Config WITH (NOLOCK) WHERE ID=58)													
							SET @sColoreUltimaSomministrazioneErogata=ISNULL(@sColoreUltimaSomministrazioneErogata,'')
																					
							SET @sCarattereUltimaSomministrazioneErogata=(SELECT TOP 1 Valore FROM T_Config WITH (NOLOCK)  WHERE ID=59)			
							SET @sCarattereUltimaSomministrazioneErogata=ISNULL(@sCarattereUltimaSomministrazioneErogata,'')					
							
							SET @sColorePrescrizioniChiuse=(SELECT TOP 1 Valore FROM T_Config WITH (NOLOCK) WHERE ID=64)													
							SET @sColorePrescrizioniChiuse=ISNULL(@sColorePrescrizioniChiuse,'')					
							
							INSERT INTO #tmpMov(
											IDRiferimento,	
											CodSezione,
											CodVoce,	
											IDOrdinamento,												
											DataOraInizio,
											DataOraFine,							
											Valore,													
											IconaValore,
											ColoreValore,	
											PermessoInserimento,
											PermessoModifica,
											PermessoVisualizza,										
											PermessoGrafico,
											PermessoEroga,
											PermessoAnnulla,
											PermessoCancella,
											PermessoCambiaStato)	
								SELECT 
									M.ID AS IDRiferimento,
									'PFM' AS CodSezione,	
									-- convert(varchar(50),MPT.IDPrescrizione)		
									CONVERT(VARCHAR(50),MPT.IDPrescrizione) + ISNULL(M.Sottoclasse,'')	 AS CodVoce,	
									M.IDGruppo AS IDOrdinamento,
									-- Calcolo DataOra Inizio
									CASE 
										WHEN MI.ID IS NULL THEN								
											-- Nessun Task Collegato
											CASE											
												WHEN M.CodStatoTaskInfermieristico ='PR'  THEN M.DataProgrammata
												ELSE ISNULL(M.DataErogazione,M.DataProgrammata) 
											END 
										ELSE
											-- Esiste un task Collegato
											-- Calcolo la Data di Inizio Retificata
											CASE											
												WHEN MI.CodStatoTaskInfermieristico ='PR'  THEN DateAdd(minute,@nStep,MI.DataProgrammata)
												ELSE DateAdd(minute,@nStep,ISNULL(MI.DataErogazione,MI.DataProgrammata))
											END 
									END	
									AS DataOraInizio,
									
									
									-- Calcolo DataOra Fine
									CASE 
										WHEN MI.ID IS NULL THEN		
											-- Nessun Task Collegato
											-- ritorno la DataFine	
											CASE											
												WHEN M.CodStatoTaskInfermieristico ='PR'  THEN M.DataProgrammata
												ELSE ISNULL(M.DataErogazione,M.DataProgrammata)
											END
										ELSE	
											-- Esiste un task Collegato
											CASE 
												WHEN
														-- Calcolo la Data di Inizio Retificata
														CASE											
															WHEN MI.CodStatoTaskInfermieristico ='PR'  THEN DateAdd(minute,@nStep,MI.DataProgrammata)
															ELSE DateAdd(minute,@nStep,ISNULL(MI.DataErogazione,MI.DataProgrammata))
														END 
														>	
														-- se maggiore di 
														-- DataFine												
														CASE											
															WHEN M.CodStatoTaskInfermieristico ='PR'  THEN M.DataProgrammata
															ELSE ISNULL(M.DataErogazione,M.DataProgrammata)
														END														
													THEN
														-- restituisco 
														-- Calcolo la Data di Inizio Retificata
														CASE											
															WHEN MI.CodStatoTaskInfermieristico ='PR'  THEN DateAdd(minute,@nStep,MI.DataProgrammata)
															ELSE DateAdd(minute,@nStep,ISNULL(MI.DataErogazione,MI.DataProgrammata))
														END 
													ELSE
														-- altrimenti
														-- ritorno DataFine												
														CASE											
															WHEN M.CodStatoTaskInfermieristico ='PR'  THEN M.DataProgrammata
															ELSE ISNULL(M.DataErogazione,M.DataProgrammata)
														END	 	
											END
											
									END 
									AS DataOraFine,	
									CASE						
										-- CarattereUltimaSomministrazioneProgrammata per prescrizioni			
										WHEN (M.CodStatoTaskInfermieristico ='PR' AND M.DataProgrammata=PRETASK.DataUltimaProgrammazione) AND	
											@sCarattereUltimaSomministrazioneProgrammata <> '' 
											AND ISNULL(MP.CodStatoContinuazione,'AP') <> 'CH'
												 THEN @sCarattereUltimaSomministrazioneProgrammata + ' '
												 
										-- CarattereUltimaSomministrazioneErogata		 										
										WHEN (M.CodStatoTaskInfermieristico ='ER' AND M.DataErogazione=PRETASK.DataUltimaErogazione) AND
											PRETASK.DataUltimaProgrammazione IS NULL AND 
											@sCarattereUltimaSomministrazioneProgrammata <> ''
												 THEN @sCarattereUltimaSomministrazioneProgrammata + ' '
											 
										WHEN (M.CodStatoTaskInfermieristico ='ER' AND M.DataErogazione=PRETASK.DataUltimaErogazione) AND
											@sCarattereUltimaSomministrazioneErogata <> ''
											 THEN @sCarattereUltimaSomministrazioneErogata + ' '													
										ELSE ''
									END +																											
									
									-- Valore
									CASE 
										WHEN MI.ID IS NOT NULL THEN	'--> ' 		-- Se sono un task finale 
										ELSE ''
									END +									
									-- ISNULL(M.DescrizioneFUT,ISNULL(ISNULL(M.PosologiaEffettiva,MPT.Posologia),ISNULL(T.Sigla,''))) 
									CASE 
										WHEN ISNULL(M.DescrizioneFUT,'') = '' THEN	
											CASE 
												WHEN ISNULL(
																CASE 
																	WHEN ISNULL(M.PosologiaEffettiva,'') ='' THEN MPT.Posologia
																	ELSE M.PosologiaEffettiva
																END
																,'') 
															= '' THEN ISNULL(T.Sigla,'')
												ELSE 
													CASE 
															WHEN ISNULL(M.PosologiaEffettiva,'') ='' THEN MPT.Posologia
															ELSE M.PosologiaEffettiva
													END
											END					
										ELSE M.DescrizioneFUT
									END
									+
									CASE 
										WHEN MF.ID IS NOT NULL THEN	' <--' 		-- Se sono un task iniziale 
										ELSE ''
									END 									
									AS Valore,									
									I.Icona AS IconaValore,	
									
									CASE							
										-- Ultima Somministrazione Programmata Aperta				
										WHEN (M.CodStatoTaskInfermieristico ='PR' AND M.DataProgrammata=PRETASK.DataUltimaProgrammazione) AND
											@sColoreUltimaSomministrazioneProgrammata <> '' AND 
											ISNULL(MP.CodStatoContinuazione,'AP') <> 'CH'
												THEN @sColoreUltimaSomministrazioneProgrammata 
									
										-- Ultima Somministrazione Programmata Chiusa
										WHEN (M.CodStatoTaskInfermieristico ='PR' AND M.DataProgrammata=PRETASK.DataUltimaProgrammazione) AND
											@sColorePrescrizioniChiuse <> ''AND 
											ISNULL(MP.CodStatoContinuazione,'AP') = 'CH'
												THEN @sColorePrescrizioniChiuse

										WHEN (M.CodStatoTaskInfermieristico ='ER' AND M.DataErogazione=PRETASK.DataUltimaErogazione) AND
												PRETASK.DataUltimaProgrammazione IS NULL AND 											
											@sCarattereUltimaSomministrazioneErogata <> ''
												 THEN @sCarattereUltimaSomministrazioneErogata + ' '
												 		
										WHEN (M.CodStatoTaskInfermieristico ='ER' AND M.DataErogazione=PRETASK.DataUltimaErogazione) AND	
											@sColoreUltimaSomministrazioneErogata <> ''
												THEN @sColoreUltimaSomministrazioneErogata 
										ELSE S.Colore 
									END
			
									--S.Colore 
									AS ColoreValore,
									
			   						@bPRFInserisci AS PermessoInserimento,			-- Permesso di Modulo per Inserimento PRESCRIZIONE					   		
			   						CASE											-- Permesso di Modifica TASK valido solo su Task ancora da Erogare
			   							WHEN M.CodStatoTaskInfermieristico='PR' THEN @bWKIModifica
			   							ELSE 0
			   						END
			   						AS PermessoModifica,							-- Permesso di Modulo per Modifica		
			   						CASE											-- Permesso di Visualizzazione solo se Erogato o Annullato
			   								WHEN M.CodStatoTaskInfermieristico IN ('ER','AN','TR') THEN @bWKIVisualizza
			   								ELSE 0
			   						END				   							   		
									AS  PermessoVisualizza,							-- Permesso di Modulo per Visualizzazione  TASK
									0 AS  PermessoGrafico,							-- Non ci sono grafici 		
									
									CASE											-- Permesso di Erogazione valido solo su Task ancora da Erogare
			   								WHEN M.CodStatoTaskInfermieristico='PR' THEN @bWKICompleta
			   								ELSE 0
			   						END AS PermessoEroga,		
			   				
			   						CASE											-- Permesso di Annullamento valido solo su Task ancora da Erogare
			   								WHEN M.CodStatoTaskInfermieristico='PR' THEN @bWKIAnnulla
			   								ELSE 0
			   						END AS PermessoAnnulla,		
			   				
			   						0 AS PermessoCancella,							-- Non si cancellano
			   						0 AS PermessoCambiaStato						-- Non si cambia lo stato
			   													
								FROM 
										T_MovTaskInfermieristici M								
										LEFT JOIN T_MovPazienti P
												ON (M.IDEpisodio = P.IDEpisodio)
												
										-- Collegamento con Task Iniziale
										LEFT JOIN			
											T_MovTaskInfermieristici MI								
												ON M.IDTaskIniziale=MI.ID		
										
										-- Collegamento con Task Finale
										LEFT JOIN			
											T_MovTaskInfermieristici MF								
												ON MF.IDTaskIniziale=M.ID	
																																
										INNER JOIN 
											T_MovPrescrizioniTempi MPT
												ON MPT.ID=M.IDGruppo
										LEFT JOIN 
											T_MovPrescrizioni MP	
												ON MP.ID=MPT.IDPrescrizione		
										LEFT JOIN 
											(SELECT 
													  CodTipo,
													  CodStato,
													  Icona32 AS Icona
												 FROM T_Icone
												 WHERE CodEntita='WKI'
												) AS I
											ON 
												(M.CodTipoTaskInfermieristico = I.CodTipo AND 	
												 M.CodStatoTaskInfermieristico = I.CodStato
												 )
										LEFT JOIN T_TipoTaskInfermieristico T
												ON (M.CodTipoTaskInfermieristico = T.Codice)
										LEFT JOIN T_StatoTaskInfermieristico S
												ON (M.CodStatoTaskInfermieristico = S.Codice)
										LEFT JOIN 
												Q_SelPrescrizioniTask PRETASK
												ON 	(PRETASK.IDPrescrizione = MP.ID)
													 --PRETMPTASK.IDPrescrizioneTempi = CONVERT(VARCHAR(50),MPT.ID))
												
																					
								WHERE 
									IDPaziente=@uIDPaziente AND		
									M.IDEpisodio=@uIDEpisodio AND					
									M.CodStatoTaskInfermieristico NOT IN ('TR','AN','CA') AND
									
									-- includo SOLO i task di tipo prescrizione
									ISNULL(M.CodSistema,'') ='PRF' AND
									ISNULL(M.IDGruppo,'') <> '' AND
									
									-- escludo quelli "Al Bisogno"
									ISNULL(MPT.AlBisogno,0)=0 AND
									
									CASE											
										WHEN M.CodStatoTaskInfermieristico ='PR'  THEN M.DataProgrammata
										ELSE ISNULL(M.DataErogazione,M.DataProgrammata)
									END >=@dDataOraInizio AND
						
									CASE											
										WHEN M.CodStatoTaskInfermieristico ='PR'  THEN M.DataProgrammata
										ELSE ISNULL(M.DataErogazione,M.DataProgrammata)
									END <= @dDataOraFine 
									
									-- Filtro su Via Somministrazione										
									AND 
									MP.CodViaSomministrazione IN (SELECT Codice FROM  #tmpViaSomministrazione)
									
									--AND ISNULL(DataErogazione,DataProgrammata) >=@dDataOraInizio 
									--AND ISNULL(DataErogazione,DataProgrammata) <= @dDataOraFine 
					END
					
					-- PFA	Al Bisogno, modalità 2
					IF @bPFA=1 
						BEGIN
							-- In caso di PFA devo visualizzare tutte le prescrizioni tempi della cartella in oggetto
												
							INSERT INTO #tmpMov(
											IDRiferimento,	
											CodSezione,
											CodVoce,		
											IDOrdinamento,											
											DataOraInizio,
											DataOraFine,							
											Valore,													
											IconaValore,
											ColoreValore,	
											PermessoInserimento,
											PermessoModifica,
											PermessoVisualizza,
											PermessoGrafico,
											PermessoEroga,
											PermessoAnnulla,
											PermessoCancella,
											PermessoCambiaStato)	
								SELECT 
									M.ID AS IDRiferimento,
									'PFA' AS CodSezione,			
									convert(varchar(50),MPT.ID)	AS CodVoce,		
									NULL AS IDOrdinamento,							
									ISNULL(DataErogazione,DataProgrammata) AS DataOraInizio,
									ISNULL(DataErogazione,DataProgrammata) AS DataOraFine,
									ISNULL(ISNULL(M.PosologiaEffettiva,MPT.Posologia),ISNULL(T.Sigla,'')) AS Valore,									
									I.Icona AS IconaValore,	
									S.Colore AS ColoreValore,							
			   						@bWKIInserisci AS PermessoInserimento,			-- Peremsso di Modulo per Inserimento PRESCRIZIONE			   									   		
			   						CASE											-- Permesso di Modifica TASK valido solo su Task ancora da Erogare
			   							WHEN M.CodStatoTaskInfermieristico='PR' THEN @bWKIModifica
			   							ELSE 0
			   						END
			   						AS PermessoModifica,							-- Permesso di Modulo per Modifica			   		
									CASE											-- Permesso di Visualizzazione solo se Erogato o Annullato o Trascritto
			   								WHEN M.CodStatoTaskInfermieristico IN ('ER','AN','TR') THEN @bWKIVisualizza
			   								ELSE 0
			   						END				   							   		
									AS  PermessoVisualizza,							-- Permesso di Modulo per Visualizzazione  TASK
									0 AS  PermessoGrafico,							-- Non ci sono grafici 
									0 AS PermessoEroga,								-- Non si erogano 
									0 AS PermessoAnnulla,							-- Non si annullano
									0 AS PermessoCancella,							-- Non si cancella
									0 AS PermessoCambiaStato			-- Non si cambia lo stato
								FROM 										
										T_MovPrescrizioniTempi MPT																
										INNER JOIN 
											T_MovPrescrizioni MP	
												ON MP.ID=MPT.IDPrescrizione				
										INNER JOIN 
											T_MovTrasferimenti TRA
												ON MP.IDTrasferimento=TRA.ID										
										LEFT JOIN
											T_MovTaskInfermieristici M										
												ON (
													-- Task di Tipo Prescrizione
													MPT.ID=CONVERT(UNIQUEIDENTIFIER,
															 CASE 
																WHEN ISNULL(M.IDGruppo,'')='' THEN NEWID() 
																ELSE IDGruppo
															 END			
																)
															
															AND
													ISNULL(M.CodSistema,'')='PRF' AND
													-- Attivi
													M.CodStatoTaskInfermieristico NOT IN ('AN','CA','TR') AND
													-- Nel range selezionato
													ISNULL(M.DataErogazione,M.DataProgrammata) >=@dDataOraInizio AND
													ISNULL(M.DataErogazione,M.DataProgrammata) <= @dDataOraFine 
													)
										LEFT JOIN T_MovPazienti P
												ON (TRA.IDEpisodio = P.IDEpisodio)
										LEFT JOIN 
											(SELECT 
													  CodTipo,
													  CodStato,
													  Icona32 AS Icona
												 FROM T_Icone
												 WHERE CodEntita='WKI'
												) AS I
											ON (M.CodTipoTaskInfermieristico = I.CodTipo AND 	
												M.CodStatoTaskInfermieristico = I.CodStato
												)					
										LEFT JOIN T_TipoTaskInfermieristico T
												ON (M.CodTipoTaskInfermieristico = T.Codice)
										LEFT JOIN T_StatoTaskInfermieristico S
												ON (M.CodStatoTaskInfermieristico = S.Codice)
								WHERE 
									IDPaziente=@uIDPaziente AND		
									M.IDEpisodio=@uIDEpisodio AND

									-- includo SOLO quelli "Al Bisogno"
									ISNULL(MPT.AlBisogno,0)=1 AND
									
									-- Filtro Sulla Cartella 
									TRA.IDCartella=@uIDCartella AND							
									
									-- Prescrizione Tempi Validata
									MPT.CodStatoPrescrizioneTempi IN ('VA','SS')												
					END
							
					-- APP	Appuntamenti, modalità 2
					IF @bAPP=1
					BEGIN
						
						-------------------------------
						-- AGENDE DI TIPO EPISODIO
						-------------------------------
						SET @sSQLQueryAgendeEPI=(SELECT dbo.MF_SQLQueryAgendeEPI())
										
						SET @sSQL='
						INSERT INTO #tmpMov(								
											IDRiferimento,	
											CodSezione,
											CodVoce,	
											IDOrdinamento,												
											DataOraInizio,
											DataOraFine,							
											Valore,													
											IconaValore,
											ColoreValore,	
											PermessoInserimento,
											PermessoModifica,
											PermessoVisualizza,
											PermessoGrafico,
											PermessoEroga,
											PermessoAnnulla,
											PermessoCancella,
											PermessoCambiaStato
											)
						SELECT 
							APP.ID AS IDRiferimento,
							''APP'' AS CodSezione,			
							M.CodAgenda AS CodVoce,		
							NULL AS IDOrdinamento,											
							DataInizio AS DataOraInizio,
							DataFine AS DataOraFine,																
							S.Descrizione AS Valore,		
							I.Icona AS IconaValore,	
							S.Colore AS ColoreValore,
							' + CONVERT(CHAR(1),@bAPPInserisci) + ' AS PermessoInserimento,			-- Permesso di Modulo per Inserimento
					   		
			   				CASE								-- Permesso di Modifica valido solo su Task ancora da Erogare
			   					WHEN APP.CodStatoAppuntamento=''PR'' THEN ' + CONVERT(CHAR(1),@bAPPModifica) + '
			   					ELSE 0
			   				END
			   				AS PermessoModifica,													-- Permesso di Modulo per Modifica			   		
							' + CONVERT(CHAR(1),@bAPPVisualizza) +' AS  PermessoVisualizza,			-- Permesso di Modulo per Visualizzazione 
							0 AS  PermessoGrafico,													-- Non ci sono grafici 
							0 AS PermessoEroga,					-- Non si erogano 
							0 AS PermessoAnnulla,				-- Non si annullano
							
							CASE								-- Permesso di Cancella valido solo su Appuntamenti ancora da Erogare
			   					WHEN APP.CodStatoAppuntamento=''PR'' THEN ' + CONVERT(CHAR(1),@bAPPCancella) + '
			   					ELSE 0
			   				END			   																									
							AS  PermessoCancella,				-- Non si cancella
							
							CASE								-- Permesso di Cambia Stato, valido solo su Appuntamenti ancora da Erogare
			   					WHEN APP.CodStatoAppuntamento IN (''PR'',''IC'',''AN'',''SS'') THEN ' + CONVERT(CHAR(1),@bAPPModifica) + '
			   					ELSE 0
			   				END			   																									
							AS PermessoCambiaStato			-- Non si cambia lo stato
						FROM 				
								T_MovAppuntamentiAgende M
								
									INNER JOIN T_MovAppuntamenti APP
										ON (M.IDAppuntamento=APP.ID) 
										
									LEFT JOIN 
											(' + @sSQLQueryAgendeEPI + ')  AS QEPI						
										ON QEPI.IDEpisodio=APP.IDEpisodio	 	
																				
									LEFT JOIN T_MovPazienti P
											ON (APP.IDEpisodio = P.IDEpisodio)
															
									LEFT JOIN T_TipoAppuntamento T
											ON (APP.CodTipoAppuntamento = T.Codice)
								
									LEFT JOIN T_StatoAppuntamento S
											ON (APP.CodStatoAppuntamento = S.Codice)					
								
									LEFT JOIN T_Agende AG
											ON M.CodAgenda=AG.Codice	
								
									LEFT JOIN T_TipoAgenda TAG
											ON AG.CodTipoAgenda=TAG.Codice
								
									LEFT JOIN 
											(SELECT 
												  CodTipo,
												  CodStato,
												  Icona32 AS Icona
											 FROM T_Icone
											 WHERE CodEntita=''APP''
											) AS I
										ON (APP.CodTipoAppuntamento = I.CodTipo AND 	
											APP.CodStatoAppuntamento = I.CodStato
											)													
						WHERE 
							P.IDPaziente=''' + CONVERT(VARCHAR(50),@uIDPaziente) +''' AND
							APP.CodStatoAppuntamento NOT IN (''AN'',''CA'') AND
							M.CodStatoAppuntamentoAgenda NOT IN (''AN'',''CA'') AND
							AG.CodEntita=''EPI'' 
							AND APP.DataInizio >=CONVERT(Datetime,''' + CONVERT(VARCHAR(50), @dDataOraInizio,120) + ''',120) AND
							APP.DataInizio <=CONVERT(Datetime,''' + CONVERT(VARCHAR(50), @dDataOraFine,120) + ''',120)  
						 ' 		
											
						EXEC (@sSQL)
							
						-------------------------------
						-- AGENDE DI TIPO PAZIENTE
						-------------------------------
						-- come sopra, cambia la WHERE con PAZ e la vista per l'oggetto
						SET @sSQLQueryAgendePAZ=(SELECT dbo.MF_SQLQueryAgendePAZ())
									
						SET @sSQL='
						INSERT INTO #tmpMov(
											IDRiferimento,	
											CodSezione,
											CodVoce,		
											IDOrdinamento,											
											DataOraInizio,
											DataOraFine,							
											Valore,													
											IconaValore,
											ColoreValore,	
											PermessoInserimento,
											PermessoModifica,
											PermessoVisualizza,
											PermessoGrafico,
											PermessoEroga,
											PermessoAnnulla,
											PermessoCancella,
											PermessoCambiaStato)				
						SELECT 
							APP.ID AS IDRiferimento,
							''APP'' AS CodSezione,			
							M.CodAgenda AS CodVoce,		
							NULL AS IDOrdinamento,											
							DataInizio AS DataOraInizio,
							DataFine AS DataOraFine,											
							S.Descrizione AS Valore,		
							I.Icona AS IconaValore,	
							S.Colore AS ColoreValore,
							' + CONVERT(CHAR(1),@bAPPInserisci) + ' AS PermessoInserimento,			-- Permesso di Modulo per Inserimento
					   		
			   				CASE								-- Permesso di Modifica valido solo su Task ancora da Erogare
			   					WHEN APP.CodStatoAppuntamento=''PR'' THEN ' + CONVERT(CHAR(1),@bAPPModifica) + '
			   					ELSE 0
			   				END
			   				AS PermessoModifica,													-- Permesso di Modulo per Modifica			   		
							' + CONVERT(CHAR(1),@bAPPVisualizza) +' AS  PermessoVisualizza,			-- Permesso di Modulo per Visualizzazione 
							0 AS  PermessoGrafico,													-- Non ci sono grafici 
							0 AS PermessoEroga,														-- Non si erogano 
							0 AS PermessoAnnulla,													-- Non si annullano
							0 AS PermessoCancella,													-- Non si cancella
							CASE																	-- Permesso di Cambia Stato, valido solo su Appuntamenti ancora da Erogare
			   					WHEN APP.CodStatoAppuntamento IN (''PR'',''IC'',''AN'',''SS'') THEN ' + CONVERT(CHAR(1),@bAPPModifica) + '
			   					ELSE 0
			   				END			   																									
							AS PermessoCambiaStato												
						FROM 				
								T_MovAppuntamentiAgende M
									
									INNER JOIN T_MovAppuntamenti APP
										ON (M.IDAppuntamento=APP.ID) 
									LEFT JOIN 
											(' + @sSQLQueryAgendePAZ + ')  AS QPAZ						
										ON QPAZ.IDPaziente=APP.IDEpisodio	 																						
															
									LEFT JOIN T_TipoAppuntamento T
												ON (APP.CodTipoAppuntamento = T.Codice)
											LEFT JOIN T_StatoAppuntamento S
												ON (APP.CodStatoAppuntamento = S.Codice)
									LEFT JOIN T_Agende AG
												ON M.CodAgenda=AG.Codice	
									LEFT JOIN T_TipoAgenda TAG
												ON AG.CodTipoAgenda=TAG.Codice
									LEFT JOIN 
												(SELECT 
													  CodTipo,
													  CodStato,
													  Icona32 AS Icona
												 FROM T_Icone
												 WHERE CodEntita=''APP''
												) AS I
											ON (APP.CodTipoAppuntamento = I.CodTipo AND 	
												APP.CodStatoAppuntamento = I.CodStato
												)					
									
						WHERE 
							APP.IDPaziente=''' + CONVERT(VARCHAR(50),@uIDPaziente) +''' AND
							APP.CodStatoAppuntamento NOT IN (''AN'',''CA'') AND
							M.CodStatoAppuntamentoAgenda NOT IN (''AN'',''CA'') AND
							ISNULL(AG.CodEntita,''PAZ'') =''PAZ'' 
							AND APP.DataInizio >=CONVERT(Datetime,''' + CONVERT(VARCHAR(50), @dDataOraInizio,120) + ''',120) AND
							APP.DataInizio <=CONVERT(Datetime,''' + CONVERT(VARCHAR(50), @dDataOraFine,120) + ''',120)  
						 ' 								
						EXEC (@sSQL)
					END
					
					-- PVT	Parametri Vitali, modalità 2
					IF @bPVT=1 
					BEGIN
						
						INSERT INTO #tmpMov(
											IDRiferimento,	
											CodSezione,
											CodVoce,		
											IDOrdinamento,											
											DataOraInizio,
											DataOraFine,							
											Valore,													
											IconaValore,
											ColoreValore,	
											PermessoInserimento,
											PermessoModifica,
											PermessoVisualizza,
											PermessoGrafico,
											PermessoEroga,
											PermessoAnnulla,
											PermessoCancella,
											PermessoCambiaStato)								
						SELECT 				
							M.ID AS IDRiferimento,
							'PVT' AS CodSezione,
							T.Codice AS CodVoce,	
							NULL AS IDOrdinamento,							
							M.DataEvento As DataOraInizio,
							M.DataEvento As DataOraFine,												
							dbo.MS_CreaStringaPVTFUT(M.ValoriFUT) AS Valore					,
							NULL AS IconaValore,
							NULL AS ColoreValore,
							@bPVTInserisci AS PermessoInserimento,					-- Permesso di Modulo per Inserimento
							@bPVTModifica AS PermessoModifica,						-- Permesso di Modulo per Modifica
							@bPVTVisualizza AS PermessoVisualizza,					-- Permesso di Modulo per Visualizza
							@bPVTVisualizza AS  PermessoGrafico,					-- Permesso di Modulo per Visualizzazione 												
							0 AS PermessoEroga,										-- Non si erogano 
							0 AS PermessoAnnulla,									-- Non si annullano
							@bPVTCancella AS PermessoCancella,						-- Non si cancella
							0 AS PermessoCambiaStato								-- Non si cambia lo stato
		   				FROM 
							T_MovParametriVitali	M	
									LEFT JOIN T_MovPazienti P
										ON (M.IDEpisodio=P.IDEpisodio)	
									LEFT JOIN T_TipoParametroVitale T
												ON (M.CodTipoParametroVitale=T.Codice)							
									LEFT JOIN 
										(SELECT 
										  CodTipo,
										  CodStato,							  
										  Icona32 As Icona
										 FROM T_Icone
										 WHERE CodEntita='PVT'
										) AS I
											ON (M.CodTipoParametroVitale=I.CodTipo AND 	
												M.CodStatoParametroVitale=I.CodStato
												)					
						WHERE 
						P.IDPaziente=@uIDPaziente and
						M.IDEpisodio=@uIDEpisodio AND
						M.CodStatoParametroVitale NOT IN ('AN','CA') 
						AND M.DataEvento >= @dDataOraInizio  AND M.DataEvento <= @dDataOraFine							
					END	
							
					-- OE	Order Entry, modalità 2
					IF @bOE=1 
					BEGIN						
						INSERT INTO #tmpMov(
											IDRiferimento,	
											CodSezione,
											CodVoce,		
											IDOrdinamento,											
											DataOraInizio,
											DataOraFine,							
											Valore,													
											IconaValore,
											ColoreValore,	
											PermessoInserimento,
											PermessoModifica,
											PermessoVisualizza,
											PermessoGrafico,
											PermessoEroga,
											PermessoAnnulla,
											PermessoCancella,
											PermessoCambiaStato)				
						SELECT 
							M.NumeroOrdineOE AS IDRiferimento,
							'OE' AS CodSezione,			
							T.Codice AS CodVoce,			
							NULL AS IDOrdinamento,			
							ISNULL(DataProgrammazioneOE,DataInoltro) AS DataOraInizio,
							ISNULL(DataProgrammazioneOE,DataInoltro) AS DataOraFine,										
							M.NumeroOrdineOE AS Valore,
							NULL AS IconaValore,	
							S.Colore AS ColoreValore,	
		   					@bOEInserisci AS PermessoInserimento,			-- Peremsso di Modulo per Inserimento			   				   			
		   					0 AS PermessoModifica,							-- Modulo per Modifica	sempre 0
							@bOEVisualizza AS  PermessoVisualizza,			-- Permesso di Modulo per Visualizzazione 
							0 AS  PermessoGrafico,							-- Non ci sono grafici 												
							0 AS PermessoEroga,								-- Non si erogano 
							0 AS PermessoAnnulla,							-- Non si annullano
							0 AS PermessoCancella,							-- Non si cancella
							0 AS PermessoCambiaStato						-- Non si cambia lo stato
						FROM 
								T_MovOrdini	M														
									INNER JOIN T_MovOrdiniEroganti ME
										ON (M.ID=ME.IDOrdine)						
									LEFT JOIN T_TipoOrdine T
										ON (ME.CodTipoOrdine = T.Codice)
									LEFT JOIN T_StatoOrdine S
										ON (M.CodStatoOrdine = S.Codice)																														
						WHERE 
							M.IDPaziente=@uIDPaziente AND
							M.IDEpisodio=@uIDEpisodio AND
							M.CodStatoOrdine NOT IN ('CA') 
							AND ISNULL(DataProgrammazioneOE,DataInoltro) >=@dDataOraInizio 
							AND    ISNULL(DataProgrammazioneOE,DataInoltro) <= @dDataOraFine 					
					END
					
					-- NTG	Note Generali, modalità 2				
					IF @bNTG=1 
					BEGIN
						-- Elenco 
						INSERT INTO #tmpMov(
									IDRiferimento,	
									CodSezione,
									CodVoce,	
									IDOrdinamento,												
									DataOraInizio,
									DataOraFine,							
									Valore,													
									IconaValore,
									ColoreValore,	
									PermessoInserimento,
									PermessoModifica,
									PermessoVisualizza,
									PermessoGrafico,
									PermessoEroga,
									PermessoAnnulla,
									PermessoCancella,
									PermessoCambiaStato				
									)								
						SELECT 				   			   
						   M.ID AS IDRiferimento,		
						   'NTG' AS CodSezione,				
						   'N' AS CodVoce,		 
						   NULL AS IDOrdinamento,  				   				   
						   DataInizio AS DataOraInizio,
						   DataFine AS DataOraFine,				 				   
						   M.Oggetto Valore,
						   NULL AS IconaValore,						   
						   'Color [A=255, R=255, G=255, B=128]' AS ColoreValore,
							@bNTGInserisci AS PermessoInserimento,			
							@bNTGModifica AS PermessoModifica,				
							@bNTGVisualizza  AS PermessoVisualizza,			
							0 AS  PermessoGrafico,				-- Non ci sono grafici ADT										
							0 AS PermessoEroga,					-- Non si erogano 
							0 AS PermessoAnnulla,				-- Non si annullano
							@bNTGCancella AS PermessoCancella,	-- Non si cancella
							0 AS PermessoCambiaStato			-- Non si cambia lo stato
						FROM T_MovNote M									
						WHERE				
							M.CodEntita='NTG' AND
							M.CodStatoNota NOT IN ('CA','AN') AND
							M.IDPaziente=@uIDPaziente AND
							M.IDEpisodio=@uIDEpisodio AND							
							DataInizio >= @dDataOraInizio  AND DataFine <= @dDataOraFine
					END
					
					------------------------------------------------------
					-- Modalità 2 Rercodset 1 ) Modalità Elenco Movimenti
					------------------------------------------------------
					
					SELECT  S.CodEntita,
							CodSezione + '_' + CodVoce AS CodAgenda,
							* 
						FROM #tmpMov 
							INNER JOIN T_SezioniFUT S
								ON #tmpMov.CodSezione=S.Codice								
						WHERE 
							-- Per il dettaglio filtro per le date passate
							DataOraInizio >=@dDataOraInizio AND
							DataOraInizio <= @dDataOraFine 	
						ORDER BY CodSezione, DataOraInizio																		
					
					
					------------------------------------------------------
					-- Modalità 2 Rercodset 2 ) Bit appuntamenti fuori Orari
					------------------------------------------------------
					
					DECLARE @bFuoriOrario AS BIT
					SET @bFuoriOrario = 0
					
					-- Leggo l'unità atomica dal trasferimento
					SET @sCodUA=(SELECT CodUA FROM T_MovTrasferimenti WHERE ID=@uIDTrasferimento)
					
					-- leggo gli orari dall'UA
					SELECT @sOraApertura=OraApertura,
							@sOraChiusura=OraChiusura
					FROM T_UnitaAtomiche
					WHERE Codice=@sCodUA	
					
					SET @sOraApertura=ISNULL(@sOraApertura,'')
					SET @sOraChiusura=ISNULL(@sOraChiusura,'')
					
					IF LEN(@sOraApertura) <> 5 SET @sOraApertura=''
					IF LEN(@sOraChiusura) <> 5 SET @sOraChiusura=''
					
					IF @sOraApertura <> '' AND @sOraChiusura <> ''
					BEGIN
						-- Orari validi
						
						-- Calcolo i minuti dalle 00 degli orari di apertura 
						SET @nOraApertura=LEFT(@sOraApertura,2) * 60 + RIGHT(@sOraApertura,2) 
						SET @nOraChiusura=LEFT(@sOraChiusura,2) * 60 + RIGHT(@sOraChiusura,2) 
						
						--	'ADT','DCM','DCI','EVC','WKI','PFM','PFA','APP','PVT','NTG'					
						SET @nTemp =(SELECT COUNT(*)
									 FROM #tmpMov
									 WHERE
										DATEPART(HOUR,DataOraInizio) * 60 + DATEPART(MINUTE,DataOraInizio) < @nOraApertura OR
										DATEPART(HOUR,DataOraFine) * 60 + DATEPART(MINUTE,DataOraFine) > @nOraChiusura
									)
						IF @nTemp > 0 
							SET @bFuoriOrario=1
						ELSE
							SET @bFuoriOrario=0
							
					END
					ELSE
						BEGIN
							-- Orari non validi
							SET @bFuoriOrario=0
						END
								
					SELECT @bFuoriOrario AS FuoriOrario
					
					---
					DROP TABLE #tmpMov
					
				END -- fine IF Modalità =2

	DROP TABLE #tmpSezioni
	RETURN 0
END



