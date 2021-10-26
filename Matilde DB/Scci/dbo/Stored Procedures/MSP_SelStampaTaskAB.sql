CREATE PROCEDURE [dbo].[MSP_SelStampaTaskAB](@xParametri XML)
AS

BEGIN

	
	DECLARE @uIDTaskInfermieristico AS UNIQUEIDENTIFIER

	-- Variabile di servizio
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
	DECLARE @sCodTipoTaskInfermieristico AS VARCHAR(20)	
	DECLARE @uIDPrescrizione AS UNIQUEIDENTIFIER
	DECLARE @uIDPrescrizioneTempi AS UNIQUEIDENTIFIER	
	DECLARE @sTrattini AS VARCHAR(10)	
	DECLARE @sOutput AS VARCHAR(255)
	DECLARE @sCodRuolo AS VARCHAR(20)
	DECLARE @sTitolo AS VARCHAR(1000)
	
	-- Ricetta (2)	
	DECLARE @uTFIDScheda AS UNIQUEIDENTIFIER
	DECLARE @sTFDescrUtenteConfermaTask VARCHAR(255)
	DECLARE @sTFDataUltimaModifica VARCHAR(255)
	DECLARE @sTFBarcode VARCHAR(255)				
		
	-- Terapia del giorno (4)	
	DECLARE @uTGIDScheda AS UNIQUEIDENTIFIER
	DECLARE @sTGBarcode AS VARCHAR(255)
	DECLARE @sTGDescrUtenteConfermaTask AS VARCHAR(255)	
	DECLARE @sTGDataTask AS VARCHAR(255)
	

	--------------------------------------------------------------------
	-- Lettura Parametri XML
	--------------------------------------------------------------------
	
	---- @uIDTaskInfermieristico
	SET @uIDTaskInfermieristico=(SELECT TOP 1 ValoreParametro.IDTaskInfermieristico.value('.','UNIQUEIDENTIFIER')
					  FROM @xParametri.nodes('/Parametri/IDTaskInfermieristico') as ValoreParametro(IDTaskInfermieristico))	

	------------------
	-- TimeStamp
	------------------
		
    SET @xTimeStamp=(SELECT TOP 1 ValoreParametro.TS.query('.')
					  FROM @xParametri.nodes('/Parametri/TimeStamp') as ValoreParametro(TS))
	-- @sCodRuolo
	SET @sCodRuolo=(SELECT TOP 1 ValoreParametro.CodRuolo.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/TimeStamp/CodRuolo') as ValoreParametro(CodRuolo))											
	SET @sCodRuolo=ISNULL(@sCodRuolo,'')

	
	SET @sTrattini='-----'
			
	---- Instesazione di Stampa
	--SET @sIntestazioneStampa=(SELECT TOP  1 Valore FROM T_Config WHERE ID=43)	
	
	--------------------------------------------------------------------
	-- Lettura Valori
	--------------------------------------------------------------------

	SET @sTitolo='TERAPIA ANTIBLASTICA'
	
	
	-- IDEpisodio, IDTrasferimento				
	SELECT TOP 1 
		@sCodTipoTaskInfermieristico=CodTipoTaskInfermieristico,
		@uIDEpisodio=IDEpisodio, 
		@uIDTrasferimento=IDTrasferimento,
		@uIDPrescrizione=IDSistema,
		@uIDPrescrizioneTempi=IDGruppo
	FROM T_MovTaskInfermieristici 
	WHERE ID=@uIDTaskInfermieristico

	--SELECT @uIDTaskInfermieristico,@sCodTipoTaskInfermieristico,@uIDEpisodio,@uIDTrasferimento

	----IF @sCodTipoTaskInfermieristico='TSKCTA' OR @sCodTipoTaskInfermieristico='ABUNCTA'
	----	SET  @sOutput=''
	----ELSE
	----	BEGIN
	--			---------------------------------
	--			--  Scheda di Tipo Errato
	--			---------------------------------
	--			SET  @sOutput='Stampa non disponibile per la scheda selezionata.'
	--			--SET @sOutput=''			-- da togliere !!!
	--	--END

	SET  @sOutput=''
	
	-- IDCartella
	SET @uIDcartella=(SELECT IDCartella FROM T_MovTrasferimenti WHERE ID=@uIDTrasferimento)
				
	-- sNumeroCartella	
	SET @sNumeroCartella=(SELECT TOP 1 NumeroCartella FROM T_MovCartelle WHERE ID=@uIDcartella)							
						
	SELECT TOP 1 		
		@sCodUA=CodUA,
		@sSettore=T.DescrSettore,
		@sUnitaOperativa=T.DescrUO,
		@sNumeroNosologico=E.NumeroNosologico,
		--@uIDEpisodio=E.ID,
		--@uIDTrasferimento=T.ID,
		@dDataRicovero=E.DataRicovero,
		@dDataListaAttesa=E.DataListaAttesa
	FROM T_MovTrasferimenti T
		LEFT JOIN T_MovEpisodi  E
		 ON T.IDEpisodio=E.ID
	WHERE IDCartella=@uIDCartella
							
	-- Instestazione di Stampa (recuperata da Azienda)	
	SET @sIntestazioneStampa=(SELECT TOP 1 RTFStampaSintetica FROM T_Aziende 
							  WHERE Codice IN (SELECT CodAzienda FROM T_UnitaAtomiche WHERE Codice=@sCodUA))
	
	-- Unita Atomica, Firma Cartella
	SELECT TOP 1 
		@sUnitaAtomica=Descrizione,
		@sFirmaCartella=FirmaCartella
	FROM T_UnitaAtomiche 
	WHERE Codice=@sCodUA

	-- Descrizione Stato Cartella	
	SET @sDescrizioneStatoCartella=(
		SELECT TOP 1 
			CASE 
				WHEN CodStatoCartella='AP' THEN 'CARTELLA APERTA'
				WHEN CodStatoCartella='CH' THEN 'CARTELLA CHIUSA IL ' + CONVERT(VARCHAR(10),DataChiusura,105)
				ELSE ''
			END	
		FROM
			T_MovCartelle 
		WHERE 
			ID=@uIDCartella)
	
	-- IntestazioneCartellaReparto
	SET @sIntestazioneCartellaReparto=(SELECT TOP 1 IntestazioneCartella FROM T_UnitaAtomiche WHERE Codice=@sCodUA)

	-- Regime
	SET  @sRegime=(SELECT 
						TOP 1 
						ISNULL(T.Descrizione,'')
					FROM T_MovEpisodi E
						LEFT JOIN T_TipoEpisodio T
							ON E.CodTipoEpisodio=T.Codice
					WHERE E.ID=	@uIDEpisodio	
		)	

	--Cerco l'ID paziente associato all'episodio
	 SET @uIDPaziente=(SELECT TOP 1 
					IDPaziente 
			  FROM T_MovPazienti MP													
			  WHERE		
					MP.IDEpisodio=@uIDEpisodio
			  )	
	
					
	--------------------------------------------------------------------
	-- 0 - Dati Generali
	--------------------------------------------------------------------
	
	-- Recordset 0, query di ritorno DATI GENERALI
	SELECT 
		'0 - Dati Generali' AS Sez00,		
		@sIntestazioneStampa AS IntestazioneStampa,
		@sIntestazioneCartellaReparto AS IntestazioneCartellaReparto,
		ISNULL(@sTitolo,@sTrattini) AS Titolo,
		ISNULL(@sRegime,@sTrattini) AS Regime,
		ISNULL(@sUnitaAtomica,@sTrattini) AS UnitaAtomica,
		ISNULL(@sUnitaOperativa,@sTrattini) AS UnitaOperativa,
		ISNULL(@sSettore,@sTrattini) AS Settore,
		ISNULL(@sNumeroCartella,@sTrattini) AS NumeroCartella,
		ISNULL(@sDescrizioneStatoCartella,@sTrattini) AS DescrizioneStatoCartella,
		ISNULL(@sFirmaCartella,@sTrattini) AS FirmaCartella,
		ISNULL(@sNumeroNosologico,@sTrattini) AS NumeroNosologico
	
	
	--------------------------------------------------------------------
	-- 1 - Paziente
	--------------------------------------------------------------------							 	
		
-- Dati del Paziente prelevati dall'Episodio
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
			-- LuogoNascita
			CASE
			  WHEN	
					LTRIM(ISNULL(LocalitaNascita,'')) <> LTRIM(ISNULL(ComuneNascita,'')) THEN
						LTRIM(ISNULL(LocalitaNascita,'') + ' ' + ISNULL(ComuneNascita,''))
			  ELSE
					ISNULL(ComuneNascita,ISNULL(LocalitaNascita,''))
			END		
			AS  LuogoNascita,
			
			-- Residenza
			LTRIM(	-- Indirizzo
					ISNULL(IndirizzoResidenza,'')	+ ', ' +		
					-- Località + Comune
					CASE
					  WHEN	
							LTRIM(ISNULL(LocalitaResidenza,'')) <> LTRIM(ISNULL(ComuneResidenza,'')) THEN
								LTRIM(ISNULL(LocalitaResidenza,'') + ' ' + ISNULL(ComuneResidenza,''))
					  ELSE
							ISNULL(ComuneResidenza,ISNULL(LocalitaResidenza,''))
					END	
					-- CAPResidenza
					+ CASE 
						WHEN ISNULL(CAPResidenza,'')='' THEN ''
						ELSE ' (' + ISNULL(CAPResidenza,'') + ')'
					  END
					)				
			AS  LuogoResidenza,
			
			-- Domicilio
			LTRIM(	-- Indirizzo
					ISNULL(IndirizzoDomicilio,'')	+ ', ' +		
					-- Località + Comune
					CASE
					  WHEN	
							LTRIM(ISNULL(LocalitaDomicilio,'')) <> LTRIM(ISNULL(ComuneDomicilio,'')) THEN
								LTRIM(ISNULL(LocalitaDomicilio,'') + ' ' + ISNULL(ComuneDomicilio,''))
					  ELSE
							ISNULL(ComuneResidenza,ISNULL(LocalitaDomicilio,''))
					END	
					-- CAPDomicilio
					+ CASE 
						WHEN ISNULL(CAPDomicilio,'')='' THEN ''
						ELSE ' (' + ISNULL(CAPDomicilio,'') + ')'
					  END
					)				
			AS  LuogoDomicilio,
			
			-- Medico Curante
			CASE 
				WHEN ISNULL(CognomeNomeMedicoBase,'') <> '' THEN
						'Dott ' + 
								REPLACE (CognomeNomeMedicoBase,'/',' ') + 
									' (C.F.:' + ISNULL(CodFiscMedicoBase,'') + ') ' 
				ELSE ''
			END AS MedicoCurante,
			MP.ElencoEsenzioni AS Esenzioni
			
		FROM 
			T_MovPazienti MP													
		WHERE	
			-- Paziente dell'Episodio	
						MP.IDEpisodio=@uIDEpisodio
	
		
	--------------------------------------------------------------------
	-- 	2 - Prescrizionie
	--------------------------------------------------------------------

	-- Leggo i Dati della prescrizione
	SELECT 		
		@uTFIDScheda = MS.IDScheda,
		-- @sTFAnteprimaRTF= MS.AnteprimaRTF,	
		@sTFDescrUtenteConfermaTask=ISNULL(LM.Descrizione,LR.Descrizione),
		@sTFDataUltimaModifica=
			CONVERT(VARCHAR(10), ISNULL(M.DataUltimaModifica,M.DataEvento), 105) 
					+ ' ' + CONVERT(varchar(5), ISNULL(M.DataUltimaModifica,M.DataEvento), 14) 		
	FROM T_MovPrescrizioni M
		LEFT JOIN 
			T_Login LR
			   ON M.CodUtenteRilevazione=LR.Codice
		LEFT JOIN 
			T_Login LM
			   ON M.CodUtenteUltimaModifica=LM.Codice
			   
		-- Schede
		LEFT JOIN 			
				(SELECT ID As IDScheda, CodScheda,Versione,IDEntita,AnteprimaRTF
				 FROM
					T_MovSchede 
				 WHERE CodEntita='PRF' AND							
					Storicizzata=0 AND
					CodStatoScheda <> 'CA'
				) AS MS
		ON 	MS.IDEntita=M.ID
	WHERE ID=@uIDPrescrizione
		
	-- Leggo i Dati Del Task
	SELECT 
		@uTGIDScheda=MS.IDScheda,		
		@sTGDescrUtenteConfermaTask=ISNULL(LM.Descrizione,LR.Descrizione),
		@sTGDataTask= CONVERT(VARCHAR(10), M.DataProgrammata, 105) 
							+ ' ' + CONVERT(varchar(5), M.DataProgrammata, 14)
		
	FROM
		T_MovTaskInfermieristici M
			-- Schede
		LEFT JOIN 			
				(SELECT ID As IDScheda, CodScheda,Versione,IDEntita,AnteprimaRTF
				 FROM
					T_MovSchede 
				 WHERE CodEntita='WKI' AND							
					Storicizzata=0 AND
					CodStatoScheda <> 'CA'
				) AS MS
			ON M.ID=MS.IDEntita		
		LEFT JOIN 
			T_Login LR
			   ON M.CodUtenteRilevazione=LR.Codice
		LEFT JOIN 
			T_Login LM
			   ON M.CodUtenteUltimaModifica=LM.Codice		
	WHERE ID=@uIDTaskInfermieristico	
	
	SELECT 
		'2 - Prescrizione' AS Sez02,
		@uTFIDScheda AS IDScheda,		
		NULL AS AnteprimaRTF,	
		@sTFDescrUtenteConfermaTask AS DescrUtenteUltimaModifica,
		@sTFDataUltimaModifica AS DataUltimaModifica,	
		NULL As IDTerapia	
		

		
	--------------------------------------------------------------------
	-- 	3 - Altri Dati
	--------------------------------------------------------------------		
	
	SELECT 
		'3 - Altri Dati' AS Sez03,
		CONVERT(VARCHAR(10), M.DataOraInizio, 105) 
			+ ' ' + CONVERT(varchar(5), DataOraInizio, 14) AS DataInizioterapia,
		ISNULL(LM.Descrizione,LR.Descrizione) AS MedicoPrescrittore,
		 @sTGDataTask AS Giorno
		 
	FROM 
		T_MovPrescrizioniTempi M
		 		LEFT JOIN 
			T_Login LR
			   ON M.CodUtenteRilevazione=LR.Codice
		LEFT JOIN 
			T_Login LM
			   ON M.CodUtenteUltimaModifica=LM.Codice
	WHERE ID=@uIDPrescrizioneTempi		
	
	--------------------------------------------------------------------
	-- 	4 - Terapia del giorno
	--------------------------------------------------------------------
	
	
		
		
	SELECT 
		'4 -  Terapia del giorno' AS Sez04,
		@uTGIDScheda AS IDScheda,		
		@sTGDescrUtenteConfermaTask AS DescrUtenteConfermaTask
	
		
	--------------------------------------------------------------------
	-- 99 - Output
	--------------------------------------------------------------------
	
	SELECT 
		'99 - Output' AS Sez99,
		ISNULL(@sOutput,'') AS Risultato
	
END




