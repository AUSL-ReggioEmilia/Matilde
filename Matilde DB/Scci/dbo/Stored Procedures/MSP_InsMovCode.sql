CREATE PROCEDURE [dbo].[MSP_InsMovCode](@xParametri AS XML )
AS
BEGIN
	

				DECLARE @sCodAgenda AS Varchar(MAX)
	DECLARE @sCodTipoTaskInfermieristico AS Varchar(MAX)	
	DECLARE @sCodContatore AS Varchar(20)
	DECLARE @sNumeroCoda AS Varchar(50)
	DECLARE @nPriorita AS INTEGER
	DECLARE @sCodLogin AS Varchar(100)
	DECLARE @sCodRuolo AS Varchar(20)
	DECLARE @xTimeStamp AS XML
	
		DECLARE @sSQL AS VARCHAR(MAX)
	DECLARE @sWhere AS VARCHAR(Max)
	DECLARE @sDataTmp AS VARCHAR(20)
	DECLARE @uGUID AS UNIQUEIDENTIFIER
	DECLARE @xTimeStampBase AS XML	
			
				CREATE TABLE #tmpAppuntamenti
	(
		IDAppuntamento UNIQUEIDENTIFIER 
	)	
	
	IF @xParametri.exist('/Parametri/IDAppuntamento')=1
		INSERT INTO #tmpAppuntamenti(IDAppuntamento)	
			SELECT 	ValoreParametro.IDAppuntamento.value('.','UNIQUEIDENTIFIER')	
				FROM @xParametri.nodes('/Parametri/IDAppuntamento') as ValoreParametro(IDAppuntamento)
																	
				CREATE TABLE #tmpTaskInfermieristici
	(
		IDTaskInfermieristico UNIQUEIDENTIFIER 
	)	
	
	IF @xParametri.exist('/Parametri/IDTaskInfermieristico')=1
		INSERT INTO #tmpTaskInfermieristici(IDTaskInfermieristico)	
			SELECT 	ValoreParametro.IDTaskInfermieristico.value('.','UNIQUEIDENTIFIER')	
				FROM @xParametri.nodes('/Parametri/IDTaskInfermieristico') as ValoreParametro(IDTaskInfermieristico)
						
		SET @sCodContatore=(SELECT	TOP 1 ValoreParametro.CodContatore.value('.','VARCHAR(20)')
						 FROM @xParametri.nodes('/Parametri/CodContatore') as ValoreParametro(CodContatore))						 
	SET @sCodContatore= LTRIM(RTRIM(ISNULL(@sCodContatore,'')))
	
		SET @sNumeroCoda=(SELECT	TOP 1 ValoreParametro.NumeroCoda.value('.','VARCHAR(50)')
						 FROM @xParametri.nodes('/Parametri/NumeroCoda') as ValoreParametro(NumeroCoda))						 
	SET @sNumeroCoda= LTRIM(RTRIM(ISNULL(@sNumeroCoda,'')))
	
		SET @nPriorita=(SELECT	TOP 1 ValoreParametro.Priorita.value('.','INTEGER')
						 FROM @xParametri.nodes('/Parametri/Priorita') as ValoreParametro(Priorita))						 
	SET @nPriorita= ISNULL(@nPriorita,0)
	
	

		SET @sCodRuolo=(SELECT	TOP 1 ValoreParametro.CodRuolo.value('.','VARCHAR(20)')
						 FROM @xParametri.nodes('/Parametri/TimeStamp/CodRuolo') as ValoreParametro(CodRuolo))						 
	SET @sCodRuolo= LTRIM(RTRIM(ISNULL(@sCodRuolo,'')))		
	
		SET @sCodLogin=(SELECT	TOP 1 ValoreParametro.CodLogin.value('.','VARCHAR(100)')
						 FROM @xParametri.nodes('/Parametri/TimeStamp/CodLogin') as ValoreParametro(CodLogin))						 
	SET @sCodLogin= LTRIM(RTRIM(ISNULL(@sCodLogin,'')))	
		
	    SET @xTimeStamp=(SELECT TOP 1 ValoreParametro.TS.query('.')
					  FROM @xParametri.nodes('/Parametri/TimeStamp') as ValoreParametro(TS))	
	
	SET @uGUID=NEWID()

			
	SET @xTimeStamp.modify('delete (/TimeStamp/IDEntita)[1]') 			
	SET @xTimeStamp.modify('insert <IDEntita>{sql:variable("@uGUID")}</IDEntita> into (/TimeStamp)[1]')
	
	SET @xTimeStamp.modify('delete (/TimeStamp/CodEntita)[1]') 
	SET @xTimeStamp.modify('insert <CodEntita>CDA</CodEntita> into (/TimeStamp)[1]')
	
		SET @xTimeStampBase=@xTimeStamp


	SET @xTimeStamp=CONVERT(XML,
						'<Parametri>' + CONVERT(varchar(max),@xTimeStamp) +
						'</Parametri>')
	BEGIN TRANSACTION
					INSERT INTO T_MovCode
				   (ID
				   ,NumeroCoda
				   ,CodContatore
				   ,CodStatoCoda
				   ,CodUtenteAssegnazione				 
				   ,CodUtenteInserimento
				   ,CodUtenteUltimaModifica
				   ,DataAssegnazione
				   ,DataAssegnazioneUTC				  
				   ,DataInserimento
				   ,DataInserimentoUTC
				   ,DataUltimaModifica
				   ,DataUltimaModificaUTC
				   ,Priorita)
			 VALUES
				   (@uGUID												   ,@sNumeroCoda										   ,@sCodContatore										   ,'AS'												   ,@sCodLogin											   ,@sCodLogin											   ,NULL												   ,GETDATE()											   ,GETUTCDATE()										   ,GETDATE()											   ,GETUTCDATE()										   ,NULL												   ,NULL												   ,@nPriorita
					)
	IF @@ERROR=0 
		BEGIN
						INSERT INTO T_MovCodeEntita
			   (ID
			   ,IDCoda
			   ,CodEntita
			   ,IDEntita
			   ,CodStatoCodaEntita
			   ,CodUtenteChiamata
			   ,CodUtenteInserimento
			   ,CodUtenteUltimaModifica
			   ,DataChiamata
			   ,DataChiamataUTC
			   ,DataInserimento
			   ,DataInserimentoUTC
			   ,DataUltimaModifica
			   ,DataUltimaModificaUTC)
			 SELECT 
				NEWID()  AS ID,
				@uGUID AS IDCoda,
				'APP' AS CodEntita,
				IDAppuntamento AS IDEntita,
				'AS' AS CodStatoCodaEntita,
				NULL AS CodUtenteChiamata,
				@sCodLogin AS CodUtenteInserimento,
				NULL AS CodUtenteUltimaModifica,
				NULL AS DataChiamata,
				NULL AS DataChiamataUTC,
				GETDATE() AS DataInserimento,
				GETUTCDATE() AS DataInserimentoUTC,
				NULL AS DataUltimaModifica,
				NULL AS DataUltimaModificaUTC
			FROM #tmpAppuntamenti		  
	
		END	
	
	IF @@ERROR=0 
	BEGIN
						INSERT INTO T_MovCodeEntita
			   (ID
			   ,IDCoda
			   ,CodEntita
			   ,IDEntita
			   ,CodStatoCodaEntita
			   ,CodUtenteChiamata
			   ,CodUtenteInserimento
			   ,CodUtenteUltimaModifica
			   ,DataChiamata
			   ,DataChiamataUTC
			   ,DataInserimento
			   ,DataInserimentoUTC
			   ,DataUltimaModifica
			   ,DataUltimaModificaUTC)
			 SELECT 
				NEWID()  AS ID,
				@uGUID AS IDCoda,
				'WKI' AS CodEntita,
				IDTaskInfermieristico AS IDEntita,
				'AS' AS CodStatoCodaEntita,
				NULL AS CodUtenteChiamata,
				@sCodLogin AS CodUtenteInserimento,
				NULL AS CodUtenteUltimaModifica,
				NULL AS DataChiamata,
				NULL AS DataChiamataUTC,
				GETDATE() AS DataInserimento,
				GETUTCDATE() AS DataInserimentoUTC,
				NULL AS DataUltimaModifica,
				NULL AS DataUltimaModificaUTC			
		FROM #tmpTaskInfermieristici		
	END	
							
	IF @@ERROR=0 
		BEGIN
						EXEC MSP_InsMovTimeStamp @xTimeStamp		
		END	
	IF @@ERROR = 0
		BEGIN
			COMMIT TRANSACTION
			SELECT @uGUID AS ID
		END		
		ELSE
			BEGIN
				ROLLBACK TRANSACTION			
				SELECT NULL AS ID
			END	

		DROP TABLE #tmpAppuntamenti
	DROP TABLE #tmpTaskInfermieristici
END