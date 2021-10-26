

CREATE PROCEDURE [MSP_InsMovSchedeRevisione](@xParametri XML)
AS
BEGIN	
	
			
		DECLARE @uIDScheda AS UNIQUEIDENTIFIER	
	DECLARE @sCodLogin AS VARCHAR(100)

		DECLARE @sGUID AS VARCHAR(50)
	DECLARE @uGUID AS UNIQUEIDENTIFIER
	DECLARE @nNumeroScheda AS INTEGER
	DECLARE @sCodScheda AS VARCHAR(20)

		DECLARE @xLogPrima AS XML
	DECLARE @xLogDopo AS XML
	DECLARE @xTemp AS XML
	DECLARE @xParLog AS XML
	DECLARE @xTimeStampBase AS XML	

		DECLARE @xTimeStamp AS XML	
	
			
		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDScheda.value('.','VARCHAR(50)')
					FROM @xParametri.nodes('/Parametri/IDScheda') as ValoreParametro(IDScheda))
	IF ISNULL(@sGUID,'') <> '' SET @uIDScheda=CONVERT(UNIQUEIDENTIFIER,@sGUID)
	
	SET @sCodLogin=(SELECT TOP 1 ValoreParametro.CodLogin.value('.','VARCHAR(100)')
					FROM @xParametri.nodes('/Parametri/TimeStamp/CodLogin') as ValoreParametro(CodLogin))
	    SET @xTimeStamp=(SELECT TOP 1 ValoreParametro.TS.query('.')
						FROM @xParametri.nodes('/Parametri/TimeStamp') as ValoreParametro(TS))
		
			
		SET @uGUID=NEWID()

	SET @sCodScheda = (SELECT TOP 1 CodScheda FROM T_MovSchede WHERE ID=@uIDScheda)

	SET @nNumeroScheda = (SELECT MAX(Numero) AS Numero FROM T_MovSchede WHERE CodScheda=@sCodScheda And IDSchedaPadre=@uIDScheda)
	SET @nNumeroScheda= ISNULL(@nNumeroScheda,0)
	SET @nNumeroScheda=@nNumeroScheda+1
	
	BEGIN TRAN
	
	BEGIN TRY 					INSERT INTO T_MovSchede(
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
				   ,CodUtenteRilevazione
				   ,CodUtenteUltimaModifica
				   ,DataUltimaModifica
				   ,DataUltimaModificaUTC
				   ,InEvidenza
				   ,Validabile
				   ,Validata
				   ,Riservata
				   ,CodUtenteValidazione
				   ,DataValidazione
				   ,DataValidazioneUTC
				   ,CodRuoloRilevazione
				   ,CodStatoSchedaCalcolato
				   ,Revisione)
			SELECT 	
					@uGUID		
				   ,CodUA
				   ,CodEntita
				   ,IDEntita
				   ,IDPaziente
				   ,IDEpisodio
				   ,IDTrasferimento
				   ,CodScheda
				   ,Versione
				   ,@nNumeroScheda AS Numero
				   ,Dati
				   ,AnteprimaRTF
				   ,AnteprimaTXT
				   ,DatiObbligatoriMancantiRTF
				   ,DatiRilievoRTF
				   ,@uIDScheda
				   ,Storicizzata
				   ,CodStatoScheda
				   ,DataCreazione
				   ,DataCreazioneUTC
				   ,CodUtenteRilevazione
				   ,CodUtenteUltimaModifica
				   ,DataUltimaModifica
				   ,DataUltimaModificaUTC
				   ,InEvidenza
				   ,Validabile
				   ,Validata
				   ,Riservata
				   ,CodUtenteValidazione
				   ,DataValidazione
				   ,DataValidazioneUTC
				   ,CodRuoloRilevazione
				   ,CodStatoSchedaCalcolato
				   ,1 AS Revisione
				FROM T_MovSchede
				WHERE ID=@uIDScheda

												
			SET @xLogDopo=Convert(XML,'<DataSet></DataSet>')
			SET @xLogPrima=Convert(XML,'<DataSet></DataSet>')
	
										
						SET @xTimeStamp.modify('delete (/TimeStamp/IDEntita)[1]') 			
			SET @xTimeStamp.modify('insert <IDEntita>{sql:variable("@uGUID")}</IDEntita> into (/TimeStamp)[1]')

						SET @xTimeStamp.modify('delete (/TimeStamp/CodAzione)[1]') 			
			SET @xTimeStamp.modify('insert <CodAzione>REV</CodAzione> into (/TimeStamp)[1]')

						SET @xTimeStampBase=@xTimeStamp

			SET @xTimeStamp=CONVERT(XML,
								'<Parametri>' + CONVERT(varchar(max),@xTimeStamp) +
								'</Parametri>')
		
						SET @xTemp=
				(SELECT * FROM 
					(SELECT * FROM T_MovSchede
						WHERE ID=@uGUID
					) AS [Table]
				FOR XML AUTO, ELEMENTS)
			
			SET @xLogDopo.modify('insert sql:variable("@xTemp") as first into (/DataSet)[1]')

						SET @xTemp=
				(SELECT * FROM 
					(SELECT * FROM T_MovSchede
						WHERE ID=@uIDScheda
					) AS [Table]
				FOR XML AUTO, ELEMENTS)
	
			SET @xLogPrima.modify('insert sql:variable("@xTemp") as first into (/DataSet)[1]')

																																																			
						SET @xParLog=CONVERT(XML,'<Parametri><LogPrima/><LogDopo/></Parametri>')			
			SET @xParLog.modify('insert sql:variable("@xLogDopo") as first into (/Parametri/LogDopo)[1]')
			SET @xParLog.modify('insert sql:variable("@xLogPrima") as first into (/Parametri/LogPrima)[1]')
			SET @xParLog.modify('insert sql:variable("@xTimeStampBase") as last into (/Parametri)[1]')
			
						EXEC MSP_InsMovLog @xParLog
			
			COMMIT TRAN

	END TRY 
	BEGIN CATCH
								IF @@TRANCOUNT > 0	
		BEGIN 
			ROLLBACK TRANSACTION	
		END

						
		DECLARE @ErrMsg varchar(max), @ErrSev int, @ErrState int
		SELECT @ErrMsg = ERROR_MESSAGE(), @ErrSev = ERROR_SEVERITY(), @ErrState = ERROR_STATE()		
		RAISERROR(@ErrMsg, @ErrSev, @ErrState)
	END CATCH
	
	
	SELECT @uGUID AS ID			

	RETURN 0

END