CREATE PROCEDURE [dbo].[MSP_SelStampaFirmaDCL](@xParametri XML)
AS
BEGIN			
	
	DECLARE @uIDMovDiarioClinico AS UNIQUEIDENTIFIER
		
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
	DECLARE @sOutput AS VARCHAR(255)
	DECLARE @sCodRuolo AS VARCHAR(20)
	DECLARE @sCodLogin AS VARCHAR(100)
	DECLARE @sTitolo AS VARCHAR(1000)

	SET @uIDMovDiarioClinico=(SELECT TOP 1 ValoreParametro.IDMovDiarioClinico.value('.','UNIQUEIDENTIFIER')
					  FROM @xParametri.nodes('/Parametri/IDMovDiarioClinico') as ValoreParametro(IDMovDiarioClinico))	
	
    SET @xTimeStamp=(SELECT TOP 1 ValoreParametro.TS.query('.')
					  FROM @xParametri.nodes('/Parametri/TimeStamp') as ValoreParametro(TS))

	SET @sCodRuolo=(SELECT TOP 1 ValoreParametro.CodRuolo.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/TimeStamp/CodRuolo') as ValoreParametro(CodRuolo))											
	SET @sCodRuolo=ISNULL(@sCodRuolo,'')
		
	SET @sCodLogin=(SELECT TOP 1 ValoreParametro.CodLogin.value('.','VARCHAR(100)')
					  FROM @xParametri.nodes('/Parametri/TimeStamp/CodLogin') as ValoreParametro(CodLogin))											
	SET @sCodLogin=ISNULL(@sCodLogin,'')
	
	SET @sTrattini='-----'
			
	
	SELECT TOP 1 
			@uIDEpisodio=IDEpisodio, 
			@uIDTrasferimento=IDTrasferimento 
	FROM T_MovDiarioClinico
	WHERE ID=@uIDMovDiarioClinico
						
	SET @uIDcartella=(SELECT IDCartella FROM T_MovTrasferimenti WHERE ID=@uIDTrasferimento)
					
	SET @sNumeroCartella=(SELECT TOP 1 NumeroCartella FROM T_MovCartelle WHERE ID=@uIDcartella)	
											
	SELECT TOP 1 		
		@sCodUA=CodUA,
		@sSettore=T.DescrSettore,
		@sUnitaOperativa=T.DescrUO,
		@sNumeroNosologico=E.NumeroNosologico,
		@uIDEpisodio=E.ID,
		@uIDTrasferimento=T.ID,
		@dDataRicovero=E.DataRicovero,
		@dDataListaAttesa=E.DataListaAttesa
	FROM T_MovTrasferimenti T
		LEFT JOIN T_MovEpisodi  E
		 ON T.IDEpisodio=E.ID
	WHERE IDCartella=@uIDCartella
					
	SET @sIntestazioneStampa=(SELECT TOP 1 RTFStampaEstesa FROM T_Aziende 
							  WHERE Codice IN (SELECT CodAzienda FROM T_UnitaAtomiche WHERE Codice=@sCodUA))


	SET @sIntestazioneStampaSintetica=(SELECT TOP 1 RTFStampaSintetica FROM T_Aziende 
									   WHERE Codice IN (SELECT CodAzienda FROM T_UnitaAtomiche WHERE Codice=@sCodUA))
				
	SELECT TOP 1 
		@sUnitaAtomica=Descrizione,
		@sFirmaCartella=FirmaCartella
	FROM T_UnitaAtomiche 
	WHERE Codice=@sCodUA
		
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
	
	SET @sIntestazioneCartellaReparto=(SELECT TOP 1 IntestazioneCartella FROM T_UnitaAtomiche WHERE Codice=@sCodUA)

	
	SET @sIntestazioneCartellaRepartoSintetica=(SELECT TOP 1 IntestazioneSintetica FROM T_UnitaAtomiche WHERE Codice=@sCodUA)
	
	SET  @sRegime=(SELECT 
						TOP 1 
						ISNULL(T.Descrizione,'')
					FROM T_MovEpisodi E
						LEFT JOIN T_TipoEpisodio T
							ON E.CodTipoEpisodio=T.Codice
					WHERE E.ID=	@uIDEpisodio	
		)		
			
	SET @uIDPaziente=(SELECT TOP 1 
				IDPaziente 
		  FROM T_MovPazienti MP													
		  WHERE		
				MP.IDEpisodio=@uIDEpisodio
		  )						
	SET @sTitolo='DOCUMENTAZIONE DI ' + ISNULL(@sRegime,'') + ' - ' + ISNULL(@sUnitaOperativa,'')
	SET @sOutput=''
		
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
			T_MovPazienti MP													
		WHERE				
			MP.IDEpisodio=@uIDEpisodio
	
	SELECT 
		'2 - Tipo di Diario' AS Sez02,
		ISNULL(T.Descrizione,'') AS TipoDiario
		
	FROM 
		T_MovDiarioClinico M
			LEFT JOIN T_TipoDiario T
				ON M.CodTipoDiario=T.Codice
	WHERE
		ID=@uIDMovDiarioClinico			
	
	SELECT 
		  '3 - Diario Clinico' AS Sez03,		  		  
		  Convert(varchar(20),M.DataEvento,105) +' ' +  Convert(varchar(5),M.DataEvento,108) As DataEvento,	
		  CASE 
			WHEN M.CodStatoDiario='VA' THEN
					Convert(varchar(20),M.DataValidazione,105) +' ' +  Convert(varchar(5),DataValidazione,108) 
			ELSE
				''							
		  END AS DataValidazione,
		  dbo.MF_PulisciRTF(MS.AnteprimaRTF) AS AnteprimaRTF,
		  L.Descrizione +
					CASE
						WHEN ISNULL(L.CodiceFiscale,'') <> '' THEN CHAR(13) + CHAR(10) + L.CodiceFiscale
						ELSE ''
					END	
		   AS UtenteValidatore		
		  
	FROM 
		T_MovDiarioClinico	M
		LEFT JOIN 
				(SELECT ID As IDScheda, CodScheda,Versione,IDEntita,AnteprimaRTF
				 FROM
					T_MovSchede 
				 WHERE CodEntita='DCL' AND							
					Storicizzata=0
				) AS MS
			ON MS.IDEntita=M.ID	
		LEFT JOIN T_Login L
			ON L.Codice = @sCodLogin
		LEFT JOIN T_TipoVoceDiario AS TV
			ON (M.CodTipoVoceDiario =TV.Codice)	
	WHERE 														
		M.ID = @uIDMovDiarioClinico			
	
	SELECT 
		'99 - Output' AS Sez99,
		ISNULL(@sOutput,'') AS Risultato
				
END
