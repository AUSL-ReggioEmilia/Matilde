CREATE PROCEDURE [dbo].[MSP_SelStampaCartellaEvidenzaClinica](@xParametri XML)
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
	DECLARE @bSoloAllegaInCartella AS BIT

	DECLARE @xParametriReport AS XML
	DECLARE @bScrittaPSC AS BIT
	DECLARE @nTmpScrittaPSC AS INTEGER

				
		
	SET @uIDCartella=(SELECT TOP 1 ValoreParametro.IDCartella.value('.','UNIQUEIDENTIFIER')
					  FROM @xParametri.nodes('/Parametri/IDCartella') as ValoreParametro(IDCartella))	
	SET @uIDCartella=ISNULL(@uIDCartella,'')	
		
		SET @bSoloAllegaInCartella=(SELECT TOP 1 ValoreParametro.SoloAllegaInCartella.value('.','bit')
					  FROM @xParametri.nodes('/Parametri/SoloAllegaInCartella') as ValoreParametro(SoloAllegaInCartella))											
	SET @bSoloAllegaInCartella=ISNULL(@bSoloAllegaInCartella,0)

				
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
	FROM T_MovTrasferimenti T
		LEFT JOIN T_MovEpisodi  E
		 ON T.IDEpisodio=E.ID
	WHERE IDCartella=@uIDCartella
	
		SET @sIntestazioneStampa=(SELECT TOP 1 RTFStampaEstesa FROM T_Aziende 			
							  WHERE Codice IN (SELECT CodAzienda FROM T_UnitaAtomiche WHERE Codice=@sCodUA))
	
		SELECT TOP 1 
				@sUnitaAtomica=Descrizione
	FROM T_UnitaAtomiche 
	WHERE Codice=@sCodUA
	
		SELECT TOP 1 
			@sRegime= ISNULL(T.Descrizione,''),				
			@sCodRegime = E.CodTipoEpisodio
	FROM T_MovEpisodi E
		LEFT JOIN T_TipoEpisodio T
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
		ISNULL(M.NumeroRefertoDWH,'..........') AS NumeroReferto,
		M.PDFDWH
		
	FROM 
		T_MovEvidenzaClinica	M			
			LEFT JOIN T_MovPazienti P
				ON (M.IDEpisodio=P.IDEpisodio)													
			LEFT JOIN T_TipoEvidenzaClinica T 			
				ON (M.CodTipoEvidenzaClinica=T.Codice)
			LEFT JOIN T_StatoEvidenzaClinica S
					ON (M.CodStatoEvidenzaClinica=S.Codice)
			LEFT JOIN T_StatoEvidenzaClinicaVisione S2
					ON (M.CodStatoEvidenzaClinicaVisione=S2.Codice)							
			LEFT JOIN T_Login L
					ON (M.CodUtenteVisione=L.Codice)			
	WHERE
				(M.IDEpisodio=@uIDEpisodio	
		 OR 
		 		 M.IDEpisodio 
			IN (SELECT IDEpisodio FROM T_MovTrasferimenti WHERE IDCartella IN (SELECT IDCartella FROM #tmpCartelleEpi))
		 )
						 
		AND
		
				M.CodStatoEvidenzaClinica NOT IN ('AN','CA') 
		
				AND 
		1 = CASE 
				WHEN M.CodStatoEvidenzaClinicaVisione IN ('VS','DV') THEN 1
				WHEN M.CodTipoEvidenzaClinica IN ('PSC') THEN 1
				WHEN M.CodTipoEvidenzaClinica IN ('SOLE-CC') THEN 1
				WHEN M.CodTipoEvidenzaClinica IN ('SOLE-CCI') THEN 1
				ELSE 0
			END
		
				AND ISNULL(T.AllegaInCartella,0) = 
				CASE 
					WHEN @bSoloAllegaInCartella=0 THEN ISNULL(T.AllegaInCartella,0)
					ELSE  1
				END
				

						
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