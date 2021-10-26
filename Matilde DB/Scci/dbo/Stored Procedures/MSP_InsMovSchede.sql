

CREATE PROCEDURE [dbo].[MSP_InsMovSchede](@xParametri XML)
AS
BEGIN
		
	
							
		DECLARE @sCodUA AS VARCHAR(20)	
	DECLARE @sCodEntita AS VARCHAR(20)
	DECLARE @uIDEntita AS UNIQUEIDENTIFIER			
	DECLARE @uIDPaziente AS UNIQUEIDENTIFIER		
	DECLARE @uIDEpisodio AS UNIQUEIDENTIFIER		
	DECLARE @uIDTrasferimento AS UNIQUEIDENTIFIER		
	DECLARE @uIDScheda AS UNIQUEIDENTIFIER	
	DECLARE @sCodScheda  AS VARCHAR(20)
	DECLARE @nVersione INTEGER
	DECLARE @nNumero INTEGER
	DECLARE @xDati XML

	DECLARE @binAnteprimaRTF VARBINARY(MAX)
	DECLARE @binAnteprimaTXT VARBINARY(MAX)
	DECLARE @binDatiObbligatoriMancantiRTF VARBINARY(MAX)
	DECLARE @binDatiRilievoRTF VARBINARY(MAX)
	
	DECLARE @sAnteprimaRTF VARCHAR(MAX)
	DECLARE @sAnteprimaTXT VARCHAR(MAX)
	DECLARE @sDatiObbligatoriMancantiRTF VARCHAR(MAX)
	DECLARE @sDatiRilievoRTF VARCHAR(MAX)

	DECLARE @uIDSchedaPadre AS UNIQUEIDENTIFIER		
	DECLARE @sCodStatoScheda AS VARCHAR(20) 
	DECLARE @sCodStatoSchedaCalcolato AS VARCHAR(20) 
	
	DECLARE @sCodUtente AS VARCHAR(100)
	DECLARE @sCodRuolo AS VARCHAR(20)
	DECLARE @bInEvidenza AS BIT
			
	DECLARE @uIDCartellaAmbulatoriale AS UNIQUEIDENTIFIER	

		DECLARE @sGUID AS VARCHAR(50)
	DECLARE @uGUID AS UNIQUEIDENTIFIER
	DECLARE @bSchedaRipetibile AS bit
	DECLARE @bValidabile AS BIT
	DECLARE @bValidata AS BIT
	DECLARE @bRiservata AS BIT
	
	    DECLARE @xTimeStampBase AS XML	
	DECLARE @xTimeStamp AS XML	
		
		DECLARE @xLogPrima AS XML
	DECLARE @xLogDopo AS XML
	DECLARE @xTemp AS XML
	DECLARE @xParLog AS XML

		
		SET @sCodUA=(SELECT TOP 1 ValoreParametro.CodUA.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodUA') as ValoreParametro(CodUA))	
	if @sCodUA = '' SET @sCodUA = null
		
		SET @sCodEntita=(SELECT TOP 1 ValoreParametro.CodEntita.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodEntita') as ValoreParametro(CodEntita))	
	SET @sCodEntita=ISNULL(@sCodEntita,'')
	
		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDEntita.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDEntita') as ValoreParametro(IDEntita))
	IF 	ISNULL(@sGUID,'') <> ''
		 SET @uIDEntita=CONVERT(UNIQUEIDENTIFIER,	@sGUID)		  
	
						  
		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDPaziente.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDPaziente') as ValoreParametro(IDPaziente))
	IF 	ISNULL(@sGUID,'') <> '' SET @uIDPaziente=CONVERT(UNIQUEIDENTIFIER,	@sGUID)		  
	
		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDEpisodio.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDEpisodio') as ValoreParametro(IDEpisodio))
	IF 	ISNULL(@sGUID,'') <> '' SET @uIDEpisodio=CONVERT(UNIQUEIDENTIFIER,	@sGUID)		  
	
		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDTrasferimento.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDTrasferimento') as ValoreParametro(IDTrasferimento))
	IF 	ISNULL(@sGUID,'') <> '' SET @uIDTrasferimento=CONVERT(UNIQUEIDENTIFIER,	@sGUID)		
	
		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDScheda.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDScheda') as ValoreParametro(IDScheda))
	IF 	ISNULL(@sGUID,'') <> '' SET @uIDScheda=CONVERT(UNIQUEIDENTIFIER,	@sGUID)	  
	
		SET @sCodScheda=(SELECT TOP 1 ValoreParametro.CodScheda.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodScheda') as ValoreParametro(CodScheda))	
	SET @sCodScheda=ISNULL(@sCodScheda,'')
	
		SET @nVersione=(SELECT TOP 1 ValoreParametro.Versione.value('.','INTEGER')
					  FROM @xParametri.nodes('/Parametri/Versione') as ValoreParametro(Versione))	
	SET @nVersione=ISNULL(@nVersione,0)
	
					
        SET @xDati=(SELECT TOP 1 ValoreParametro.Scheda.query('.')
					  FROM @xParametri.nodes('/Parametri/Dati/DcSchedaDati') as ValoreParametro(Scheda))	
		
		SET @binAnteprimaRTF=(SELECT TOP 1 ValoreParametro.AnteprimaRTF.value('.','VARBINARY(MAX)')
					  FROM @xParametri.nodes('/Parametri/AnteprimaRTF') as ValoreParametro(AnteprimaRTF))	

	SET @sAnteprimaRTF=(SELECT TOP 1 ValoreParametro.AnteprimaRTF.value('.','VARCHAR(MAX)')
					  FROM @xParametri.nodes('/Parametri/AnteprimaRTF') as ValoreParametro(AnteprimaRTF))	

		SET @binAnteprimaTXT=(SELECT TOP 1 ValoreParametro.AnteprimaTXT.value('.','VARBINARY(MAX)')
					  FROM @xParametri.nodes('/Parametri/AnteprimaTXT') as ValoreParametro(AnteprimaTXT))	

	SET @sAnteprimaTXT=(SELECT TOP 1 ValoreParametro.AnteprimaTXT.value('.','VARCHAR(MAX)')
					  FROM @xParametri.nodes('/Parametri/AnteprimaTXT') as ValoreParametro(AnteprimaTXT))	

		SET @binDatiObbligatoriMancantiRTF=(SELECT TOP 1 ValoreParametro.DatiObbligatoriMancantiRTF.value('.','VARBINARY(MAX)')
					  FROM @xParametri.nodes('/Parametri/DatiObbligatoriMancantiRTF') as ValoreParametro(DatiObbligatoriMancantiRTF))
	
	SET @sDatiObbligatoriMancantiRTF=(SELECT TOP 1 ValoreParametro.DatiObbligatoriMancantiRTF.value('.','VARCHAR(MAX)')
					  FROM @xParametri.nodes('/Parametri/DatiObbligatoriMancantiRTF') as ValoreParametro(DatiObbligatoriMancantiRTF))	

		SET @binDatiRilievoRTF=(SELECT TOP 1 ValoreParametro.DatiRilievoRTF.value('.','VARBINARY(MAX)')
					  FROM @xParametri.nodes('/Parametri/DatiRilievoRTF') as ValoreParametro(DatiRilievoRTF))	

	SET @sDatiRilievoRTF=(SELECT TOP 1 ValoreParametro.DatiRilievoRTF.value('.','VARCHAR(MAX)')
					  FROM @xParametri.nodes('/Parametri/DatiRilievoRTF') as ValoreParametro(DatiRilievoRTF))	

		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDSchedaPadre.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDSchedaPadre') as ValoreParametro(IDSchedaPadre))
	IF 	ISNULL(@sGUID,'') <> '' SET @uIDSchedaPadre=CONVERT(UNIQUEIDENTIFIER,	@sGUID)		  

		SET @sCodStatoScheda=(SELECT TOP 1 ValoreParametro.CodStatoScheda.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodStatoScheda') as ValoreParametro(CodStatoScheda))	
	SET @sCodStatoScheda=ISNULL(@sCodStatoScheda,'')
	IF @sCodStatoScheda='' SET @sCodStatoScheda='IC'			

		SET @bInEvidenza=(SELECT TOP 1 ValoreParametro.InEvidenza.value('.','BIT')
					  FROM @xParametri.nodes('/Parametri/InEvidenza') as ValoreParametro(InEvidenza))
	
	SET @bInEvidenza=ISNULL(@bInEvidenza,0)
	
		SET @sCodUtente=(SELECT TOP 1 ValoreParametro.CodUtente.value('.','VARCHAR(100)')
					  FROM @xParametri.nodes('/Parametri/TimeStamp/CodLogin') as ValoreParametro(CodUtente))	
	SET @sCodUtente=ISNULL(@sCodUtente,'')
	
		SET @sCodRuolo=(SELECT TOP 1 ValoreParametro.CodRuolo.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/TimeStamp/CodRuolo') as ValoreParametro(CodRuolo))	
	
		SET @sCodStatoSchedaCalcolato=(SELECT TOP 1 ValoreParametro.CodStatoSchedaCalcolato.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodStatoSchedaCalcolato') as ValoreParametro(CodStatoSchedaCalcolato))	
	
		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDCartellaAmbulatoriale.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDCartellaAmbulatoriale') as ValoreParametro(IDCartellaAmbulatoriale))
	IF 	ISNULL(@sGUID,'') <> '' SET @uIDCartellaAmbulatoriale=CONVERT(UNIQUEIDENTIFIER,	@sGUID)	 

	     SET @xTimeStamp=(SELECT TOP 1 ValoreParametro.TS.query('.')
					  FROM @xParametri.nodes('/Parametri/TimeStamp') as ValoreParametro(TS))
					  
					
	IF @uIDScheda IS NULL
		SET @uGUID=NEWID()
	ELSE
		SET @uGUID=@uIDScheda
		
		SET @bSchedaRipetibile=	(SELECT	TOP 1
										CASE 
											WHEN ISNULL(NumerositaMassima,0) > 1 AND ISNULL(SchedaSemplice,1)=0 THEN 1												ELSE 0										
										END AS Ripetibile
									FROM T_Schede
									WHERE Codice =@sCodScheda
							 )	
	SET @bSchedaRipetibile=ISNULL(@bSchedaRipetibile,0)
	
		IF @bSchedaRipetibile=1
		BEGIN
																					
			IF @uIDSchedaPadre IS NOT NULL
			BEGIN
																SET @nNumero= dbo.MF_SelNumerositaSchedaParametro('MassimoNumero',NULL,@uIDSchedaPadre,@sCodEntita,@uIDEntita,@sCodScheda)
				
			END
			ELSE
			BEGIN
												SET @nNumero = dbo.MF_SelNumerositaSchedaParametro('MassimoNumero',NULL,NULL,@sCodEntita,@uIDEntita,@sCodScheda)	
			END
			SET @nNumero=ISNULL(@nNumero,0) +1 
		END	
	ELSE
		SET @nNumero=1					  	
	
	SELECT	TOP 1
		@bRiservata=ISNULL(Riservata,0),
		@bValidabile=ISNULL(Validabile,0) 
	FROM T_Schede
	WHERE Codice =@sCodScheda
	
		SET @bValidata=0
					  	 
				
		SET @xTimeStamp.modify('delete (/TimeStamp/IDEntita)[1]') 
			
	SET @xTimeStamp.modify('insert <IDEntita>{sql:variable("@uGUID")}</IDEntita> into (/TimeStamp)[1]')

		SET @xTimeStampBase=@xTimeStamp

	SET @xTimeStamp=CONVERT(XML,
						'<Parametri>' + CONVERT(varchar(max),@xTimeStamp) +
						'</Parametri>')
						
		IF ISNULL(@sCodEntita,'')='PAZ'
	BEGIN
		SET @uIDEpisodio =NULL
		SET	@uIDTrasferimento =NULL	
	END
	

	BEGIN TRANSACTION
				INSERT INTO T_MovSchede(
					ID
				  ,CodUA
				  ,CodEntita
				  ,IDEntita
				  ,IDPaziente
				  ,IDEpisodio
				  ,IDTrasferimento
				  ,CodScheda
				  ,Versione
				  ,Numero
				  ,Dati
				  ,AnteprimaRTF
				  ,AnteprimaTXT
				  ,DatiObbligatoriMancantiRTF
				  ,DatiRilievoRTF
				  ,IDSchedaPadre
				  ,Storicizzata
				  ,CodStatoScheda
				  ,DataCreazione
				  ,DataCreazioneUTC
				  ,DataUltimaModifica
				  ,DataUltimaModificaUTC
				  ,CodUtenteRilevazione
				  ,CodUtenteUltimaModifica
				  ,InEvidenza
				  ,Riservata
				  ,Validabile
				  ,Validata
				  ,CodUtenteValidazione
				  ,DataValidazione
				  ,DataValidazioneUTC
				  ,CodRuoloRilevazione
				  ,CodStatoSchedaCalcolato
				  ,IDCartellaAmbulatoriale
				  )
		VALUES
				(
					@uGUID,												@sCodUA,											@sCodEntita,										@uIDEntita, 										@uIDPaziente,										@uIDEpisodio,										@uIDTrasferimento,									@sCodScheda,										@nVersione,											@nNumero,											@xDati,												CONVERT(VARCHAR(MAX),@binAnteprimaRTF),									dbo.MF_PulisciTXT(
						CONVERT(VARCHAR(MAX),@binAnteprimaTXT)						
						),
										CONVERT(VARCHAR(MAX),@binDatiObbligatoriMancantiRTF),						CONVERT(VARCHAR(MAX),@binDatiRilievoRTF),				
					@uIDSchedaPadre,									0,													@sCodStatoScheda,									getdate(),											getUTCdate(),										getdate(),											getUTCdate(),										@sCodUtente,										@sCodUtente,										@bInEvidenza,										@bRiservata,										@bValidabile,										@bValidata,											NULL,												NULL,												NULL,												@sCodRuolo,
					@sCodStatoSchedaCalcolato,
					@uIDCartellaAmbulatoriale
				)
	IF @@ERROR=0 AND @@ROWCOUNT >0
		EXEC MSP_InsMovTimeStamp @xTimeStamp		
	IF @@ERROR = 0
		BEGIN
			COMMIT TRANSACTION
						
												
			SET @xLogDopo=Convert(XML,'<DataSet></DataSet>')
			SET @xLogPrima=''
			
			SET @xTemp=
				(SELECT * FROM 
					(SELECT * FROM T_MovSchede
					 WHERE ID=@uGUID
					) AS [Table]
				FOR XML AUTO, ELEMENTS)
			
			SET @xLogDopo.modify('insert sql:variable("@xTemp") as first into (/DataSet)[1]')
	
		
						
			SET @xParLog=CONVERT(XML,'<Parametri><LogDopo/></Parametri>')			
			SET @xParLog.modify('insert sql:variable("@xLogDopo") as first into (/Parametri/LogDopo)[1]')
			SET @xParLog.modify('insert sql:variable("@xTimeStampBase") as last into (/Parametri)[1]')
			
			EXEC MSP_InsMovLog @xParLog
						
			SELECT @uGUID AS ID
		END	
	ELSE
		BEGIN
			ROLLBACK TRANSACTION	
			SELECT NULL AS ID
		END	 
	RETURN 0
END