CREATE PROCEDURE [dbo].[MSP_SelStampaGrafici1](@xParametri XML)
AS

BEGIN

	DECLARE @sCodUAAmbulatoriale AS  VARCHAR(20)
	
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
    DECLARE @sIntestazioneStampaSintetica AS VARCHAR(MAX)
	DECLARE @sIntestazioneCartellaReparto AS VARCHAR(MAX)
	DECLARE @sIntestazioneCartellaRepartoSintetica AS VARCHAR(MAX)
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
	DECLARE @sTrattini AS VARCHAR(10)
	DECLARE @sCodEntita AS VARCHAR(20)
	DECLARE @sOutput AS VARCHAR(255)
	DECLARE @sCodRuolo AS VARCHAR(20)
	DECLARE @sTitolo AS VARCHAR(1000)
	DECLARE @bSchedaConFigli AS BIT
	
	SET @sCodUAAmbulatoriale=(SELECT TOP 1 ValoreParametro.CodUAAmbulatoriale.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodUAAmbulatoriale') as ValoreParametro(CodUAAmbulatoriale))	

    SET @xTimeStamp=(SELECT TOP 1 ValoreParametro.TS.query('.')
					  FROM @xParametri.nodes('/Parametri/TimeStamp') as ValoreParametro(TS))

	SET @sCodRuolo=(SELECT TOP 1 ValoreParametro.CodRuolo.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/TimeStamp/CodRuolo') as ValoreParametro(CodRuolo))											
	SET @sCodRuolo=ISNULL(@sCodRuolo,'')
	
	SET @sGUID=(SELECT TOP 1 ValoreParametro.IDEpisodio.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/TimeStamp/IDEpisodio') as ValoreParametro(IDEpisodio))
	IF 	ISNULL(@sGUID,'') <> '' 
			SET @uIDEpisodio=CONVERT(UNIQUEIDENTIFIER,	@sGUID)	
		
	SET @sGUID=(SELECT TOP 1 ValoreParametro.IDTrasferimento.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/TimeStamp/IDTrasferimento') as ValoreParametro(IDTrasferimento))
	IF 	ISNULL(@sGUID,'') <> '' 
			SET @uIDTrasferimento=CONVERT(UNIQUEIDENTIFIER,	@sGUID)	
				
	SET @sGUID=(SELECT TOP 1 ValoreParametro.IDPaziente.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/TimeStamp/IDPaziente') as ValoreParametro(IDPaziente))
	IF 	ISNULL(@sGUID,'') <> '' 
			SET @uIDPaziente=CONVERT(UNIQUEIDENTIFIER,	@sGUID)	
					
	SET @sTrattini='-----'
			
	IF @uIDEpisodio IS NULL
		SET @sCodEntita='PAZ'
	ELSE
		SET @sCodEntita='EPI'
	
	IF @sCodEntita='PAZ'
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
			
			SET @sRegime=NULL
			
			SET @sTitolo='DOCUMENTAZIONE AMBULATORIALE'						
			
			IF ISNULL(@sCodUAAmbulatoriale,'') <> ''
				BEGIN
									
					SET @sIntestazioneCartellaReparto=(SELECT TOP 1 IntestazioneCartella FROM T_UnitaAtomiche WITH (NOLOCK) WHERE Codice=@sCodUAAmbulatoriale)
									
					SET @sIntestazioneCartellaRepartoSintetica=(SELECT TOP 1 IntestazioneSintetica FROM T_UnitaAtomiche WITH (NOLOCK) WHERE Codice=@sCodUAAmbulatoriale)
										
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
	
					SET @sIntestazioneCartellaReparto=(SELECT TOP 1 IntestazioneCartella FROM T_UnitaAtomiche WITH (NOLOCK) WHERE Codice='0')
					
					SET @sIntestazioneCartellaRepartoSintetica=(SELECT TOP 1 IntestazioneSintetica FROM T_UnitaAtomiche WITH (NOLOCK) WHERE Codice='0')
										
					SET @sIntestazioneStampa=(SELECT TOP 1 RTFStampaEstesa FROM T_Aziende 
							  WHERE Codice IN (SELECT CodAzienda FROM T_UnitaAtomiche WHERE Codice='0'))
					
					SET @sIntestazioneStampaSintetica=(SELECT TOP 1 RTFStampaSintetica FROM T_Aziende 
									   WHERE Codice IN (SELECT CodAzienda FROM T_UnitaAtomiche WHERE Codice='0'))
				END			

		END	
	ELSE
		IF @sCodEntita='EPI'
			BEGIN
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
					@dDataListaAttesa=E.DataListaAttesa
				FROM T_MovTrasferimenti T WITH (NOLOCK)
					LEFT JOIN T_MovEpisodi  E WITH (NOLOCK)
					 ON T.IDEpisodio=E.ID
				WHERE IDCartella=@uIDCartella
								
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
									
					SET @sIntestazioneCartellaReparto=(SELECT TOP 1 IntestazioneCartella FROM T_UnitaAtomiche WHERE Codice=@sCodUA)
					
					SET @sIntestazioneCartellaRepartoSintetica=(SELECT TOP 1 IntestazioneSintetica FROM T_UnitaAtomiche WHERE Codice=@sCodUA)
				
	
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
		ISNULL(@sNumeroNosologico,@sTrattini) AS NumeroNosologico

	IF @sCodEntita='PAZ'
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
		IF @sCodEntita='EPI'
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
	
			
END