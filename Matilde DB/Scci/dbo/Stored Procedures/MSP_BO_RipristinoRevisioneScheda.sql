CREATE PROCEDURE [dbo].[MSP_BO_RipristinoRevisioneScheda](@xParametri XML, @bErrore BIT OUTPUT)  
AS
BEGIN



		DECLARE @uIDEntitaInizio AS UNIQUEIDENTIFIER	
	DECLARE @uIDEntitaFine AS UNIQUEIDENTIFIER	
	DECLARE @xTimeStamp AS XML
	
		DECLARE @sGUID AS VARCHAR(50)
	DECLARE @nRecord AS INTEGER				
	DECLARE @sCodSchedaI AS VARCHAR(20)
	DECLARE @sCodSchedaF AS VARCHAR(20)
	DECLARE @nVersioneI AS INTEGER
	DECLARE @nVersioneF AS INTEGER
	DECLARE @nUltimoNumero AS INTEGER
	DECLARE @uIDUltimaScheda AS UNIQUEIDENTIFIER

		DECLARE @xLogPrima AS XML
	DECLARE @xLogDopo AS XML
	DECLARE @xTemp AS XML
	DECLARE @xParLog AS XML
	DECLARE @sInfoTimeStamp AS VARCHAR(255)	

		IF @xParametri.exist('/Parametri/IDEntitaInizio')=1	
		BEGIN					 
			SET @sGUID=(SELECT	TOP 1 ValoreParametro.IDEntitaInizio.value('.','VARCHAR(50)')
						FROM @xParametri.nodes('/Parametri/IDEntitaInizio') as ValoreParametro(IDEntitaInizio))
			IF ISNULL(@sGUID,'')<>'' SET @uIDEntitaInizio=CONVERT(UNIQUEIDENTIFIER,@sGUID)
		END

	IF @xParametri.exist('/Parametri/IDEntitaFine')=1	
		BEGIN					 
			SET @sGUID=(SELECT	TOP 1 ValoreParametro.IDEntitaFine.value('.','VARCHAR(50)')
						FROM @xParametri.nodes('/Parametri/IDEntitaFine') as ValoreParametro(IDEntitaFine))
			IF ISNULL(@sGUID,'')<>'' SET @uIDEntitaFine=CONVERT(UNIQUEIDENTIFIER,@sGUID)
		END

		IF @xParametri.exist('/Parametri/TimeStamp')=1	
		BEGIN								  				  				  				  	
			SET @xTimeStamp=(SELECT TOP 1 ValoreParametro.TS.query('.')
							  FROM @xParametri.nodes('/Parametri/TimeStamp') as ValoreParametro(TS))	
		END					  						

		SET @bErrore=0
	SET @nRecord=0

				
	CREATE TABLE #tmpErrori
		(		
			Errore VARCHAR(500) COLLATE Latin1_General_CI_AS
		)

	CREATE TABLE #tmpSchede
		(		
			IDScheda UNIQUEIDENTIFIER
		)
					
	IF @uIDEntitaInizio IS NULL
	BEGIN
		INSERT INTO #tmpErrori(Errore)
				VALUES('(CC001) ERRORE : IDScheda destinazione non valorizzato')
		SET @bErrore=1
	END

	IF @uIDEntitaFine IS NULL
	BEGIN
		INSERT INTO #tmpErrori(Errore)
				VALUES('(CC002) ERRORE : IDScheda origine non valorizzato')
		SET @bErrore=1
	END

	IF (@bErrore=0)
	BEGIN

				SELECT @nRecord = COUNT(*)
		FROM T_MovSchede
		WHERE ID = @uIDEntitaInizio
				AND CodStatoScheda = 'IC'
				AND Storicizzata = 0
								AND ISNULL(Revisione,0) = 0
		IF @nRecord <> 1
		BEGIN
			INSERT INTO #tmpErrori(Errore)
				VALUES('(CC003) ERRORE : Scheda di destinazione NON valida.')
			SET @bErrore=1
		END			

	END

	IF (@bErrore=0)
	BEGIN

				SELECT @nRecord = COUNT(*)
		FROM T_MovSchede
		WHERE ID = @uIDEntitaFine
				AND CodStatoScheda = 'IC'
				AND Storicizzata = 0
				AND IDSchedaPadre IS NOT NULL
				AND ISNULL(Revisione,0) = 1
		IF @nRecord <> 1
		BEGIN
			INSERT INTO #tmpErrori(Errore)
				VALUES('(CC004) ERRORE : Scheda di orgine NON valida.')
			SET @bErrore=1
		END			

	END

	IF (@bErrore=0)
	BEGIN

				SELECT @nRecord = COUNT(*)
		FROM T_MovSchede MSF
			INNER JOIN T_MovSchede MS ON MSF.IDSchedaPadre=MS.ID
		WHERE MSF.ID = @uIDEntitaFine
				AND MSF.IDSchedaPadre = @uIDEntitaInizio
		IF @nRecord <> 1
		BEGIN
			INSERT INTO #tmpErrori(Errore)
				VALUES('(CC005) ERRORE : Scheda di orgine NON figlia della scheda di destinazione.')
			SET @bErrore=1
		END		
		ELSE
		BEGIN

						SELECT @sCodSchedaI = CodScheda FROM T_MovSchede WHERE ID = @uIDEntitaInizio
			SELECT @sCodSchedaF = CodScheda FROM T_MovSchede WHERE ID = @uIDEntitaFine
			IF @sCodSchedaI <> @sCodSchedaF
			BEGIN
				INSERT INTO #tmpErrori(Errore)
					VALUES('(CC006) ERRORE : Codice Scheda NON corrispondente.')
				SET @bErrore=1
			END	

			SELECT @nVersioneI = Versione FROM T_MovSchede WHERE ID = @uIDEntitaInizio
			SELECT @nVersioneF = Versione FROM T_MovSchede WHERE ID = @uIDEntitaFine
			IF @nVersioneI <> @nVersioneI
			BEGIN
				INSERT INTO #tmpErrori(Errore)
					VALUES('(CC007) ERRORE : Versione scheda NON corrispondente.')
				SET @bErrore=1
			END	

						SELECT TOP 1 @nUltimoNumero = Numero 
			FROM T_MovSchede 
			WHERE IDSchedaPadre = @uIDEntitaFine AND Storicizzata=0 AND CodStatoScheda <> 'CA'
			ORDER BY IDNum DESC

			SELECT @uIDUltimaScheda = ID 
			FROM T_MovSchede 
			WHERE IDSchedaPadre = @uIDEntitaFine
					AND Versione = @nUltimoNumero

			IF @uIDUltimaScheda <> @uIDEntitaInizio
			BEGIN
				INSERT INTO #tmpErrori(Errore)
					VALUES('(CC008) ERRORE : Scheda non ultima della serie')
				SET @bErrore=1
			END	

		END

	END

			
	IF (@bErrore=0)
	BEGIN

		BEGIN TRY

			BEGIN TRANSACTION

															INSERT INTO T_MovSchede
					   (ID
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
					   ,Revisione
					   ,IDCartellaAmbulatoriale)
			SELECT	
					   NEWID()
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
					   ,'CA'
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
					   ,Revisione
					   ,IDCartellaAmbulatoriale
			FROM T_Movschede
			WHERE ID = @uIDEntitaInizio			
																				
			SET @xParLog=CONVERT(XML,'<Parametri><LogPrima/><LogDopo/></Parametri>')																					
			SET @xLogPrima=Convert(XML,'<DataSet></DataSet>')																						
					
			SET @xTemp=
				(SELECT * FROM 
					(SELECT * FROM T_MovSchede
						WHERE ID = @uIDEntitaInizio
					) AS [Table]
				FOR XML AUTO, ELEMENTS)				
				
			SET @xLogPrima.modify('insert sql:variable("@xTemp") as first into (/DataSet)[1]')											
			SET @xParLog.modify('insert sql:variable("@xLogPrima") as first into (/Parametri/LogPrima)[1]')

															UPDATE Destinazione
			SET 
				Destinazione.CodUA = Origine.CodUA
				,Destinazione.CodEntita = Origine.CodEntita
				,Destinazione.IDEntita = Origine.IDEntita
				,Destinazione.IDPaziente = Origine.IDPaziente
				,Destinazione.IDEpisodio = Origine.IDEpisodio
				,Destinazione.IDTrasferimento = Origine.IDTrasferimento
				,Destinazione.CodScheda = Origine.CodScheda
				,Destinazione.Versione = Origine.Versione
				,Destinazione.Numero = Origine.Numero
				,Destinazione.Dati = Origine.Dati
				,Destinazione.AnteprimaRTF = Origine.AnteprimaRTF
				,Destinazione.AnteprimaTXT = Origine.AnteprimaTXT
				,Destinazione.DatiObbligatoriMancantiRTF = Origine.DatiObbligatoriMancantiRTF
				,Destinazione.DatiRilievoRTF = Origine.DatiRilievoRTF
								,Destinazione.Storicizzata = Origine.Storicizzata
				,Destinazione.CodStatoScheda = Origine.CodStatoScheda
				,Destinazione.DataCreazione = Origine.DataCreazione
				,Destinazione.DataCreazioneUTC = Origine.DataCreazioneUTC
				,Destinazione.CodUtenteRilevazione = Origine.CodUtenteRilevazione
				,Destinazione.CodUtenteUltimaModifica = Origine.CodUtenteUltimaModifica
				,Destinazione.DataUltimaModifica = Origine.DataUltimaModifica
				,Destinazione.DataUltimaModificaUTC = Origine.DataUltimaModificaUTC
				,Destinazione.InEvidenza = Origine.InEvidenza
				,Destinazione.Validabile = Origine.Validabile
				,Destinazione.Validata = Origine.Validata
				,Destinazione.Riservata = Origine.Riservata
				,Destinazione.CodUtenteValidazione = Origine.CodUtenteValidazione
				,Destinazione.DataValidazione = Origine.DataValidazione
				,Destinazione.DataValidazioneUTC = Origine.DataValidazioneUTC
				,Destinazione.CodRuoloRilevazione = Origine.CodRuoloRilevazione
				,Destinazione.CodStatoSchedaCalcolato = Origine.CodStatoSchedaCalcolato
								,Destinazione.Revisione = 0
				,Destinazione.IDCartellaAmbulatoriale = Origine.IDCartellaAmbulatoriale
			FROM T_MovSchede Destinazione INNER JOIN T_MovSchede Origine
				ON Destinazione.ID = @uIDEntitaInizio AND Origine.ID = @uIDEntitaFine
									
			SET @xLogDopo=Convert(XML,'<DataSet></DataSet>')				
							
			SET @xTemp=
				(SELECT * FROM 
					(SELECT * FROM T_MovSchede
						WHERE ID = @uIDEntitaInizio
					) AS [Table]
				FOR XML AUTO, ELEMENTS)

			SET @xLogDopo.modify('insert sql:variable("@xTemp") as first into (/DataSet)[1]')																											
			SET @xParLog.modify('insert sql:variable("@xLogDopo") as first into (/Parametri/LogDopo)[1]')
							
						SET @xParLog.modify('insert sql:variable("@xTimeStamp") as last into (/Parametri)[1]')
							
						SET @xParLog.modify('delete (Parametri/TimeStamp/CodAzione)[1]') 			
			SET @xParLog.modify('insert <CodAzione>VAL</CodAzione> into (Parametri/TimeStamp)[1]')
		
						EXEC MSP_InsMovLog @xParLog

												UPDATE T_MovSchede			
			SET 
				CodStatoScheda='CA'
			WHERE ID = @uIDEntitaFine 
			
			SET @xTimeStamp.modify('delete (/TimeStamp/IDEntitaInizio)[1]') 					
			SET @xTimeStamp.modify('insert <IDEntitaInizio>{sql:variable("@uIDEntitaInizio")}</IDEntitaInizio> into (/TimeStamp)[1]')

						SET @xTimeStamp.modify('delete (/TimeStamp/IDEntitaFine)[1]') 					
			SET @xTimeStamp.modify('insert <IDEntitaFine>{sql:variable("@uIDEntitaFine")}</IDEntitaFine> into (/TimeStamp)[1]')

						SET @sInfoTimeStamp='Azione: MSP_BO_RipristinoRevisioneScheda'		
			SET @sInfoTimeStamp=@sInfoTimeStamp+ ' - IDEntitaInizio: ' + CONVERT(VARCHAR(50),@uIDEntitaInizio)
			SET @sInfoTimeStamp=@sInfoTimeStamp+ ' / IDEntitaFine: ' + CONVERT(VARCHAR(50),@uIDEntitaFine)
				
			SET @xTimeStamp.modify('delete (/TimeStamp/Info)[1]') 		
			SET @xTimeStamp.modify('insert <Info>{sql:variable("@sInfoTimeStamp")}</Info> into (/TimeStamp)[1]')
							
						SET @xTimeStamp.modify('delete (/TimeStamp/CodAzione)[1]') 		
			SET @xTimeStamp.modify('insert <CodAzione>MOD</CodAzione> into (/TimeStamp)[1]')
			
			SET @xTimeStamp=CONVERT(XML,
								'<Parametri>' + CONVERT(varchar(max),@xTimeStamp) +
								'</Parametri>')
									
						EXEC MSP_InsMovTimeStamp @xTimeStamp		

			COMMIT TRAN
			PRINT 'COMMIT TRAN'

		END TRY
		BEGIN CATCH
			IF @@TRANCOUNT > 0
				BEGIN
					ROLLBACK
					PRINT 'ROLLBACK'
										INSERT INTO #tmpErrori(Errore)
					VALUES('(CC999) ERRORE : Errore in elaborazione')
				SET @bErrore=1
				END
		END CATCH

	END
			
				SET @bErrore= ISNULL(@bErrore,0) 
				
	SELECT Errore FROM #tmpErrori	
	
		DROP TABLE #tmpErrori
	DROP TABLE #tmpSchede
		
END