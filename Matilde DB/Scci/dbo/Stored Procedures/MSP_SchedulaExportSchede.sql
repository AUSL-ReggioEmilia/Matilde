CREATE PROCEDURE MSP_SchedulaExportSchede
AS
BEGIN
	DECLARE @dDataUltimoPeriodo AS DATETIME
	DECLARE @dDataInizio AS DATETIME
	DECLARE @dDataFine AS DATETIME
	DECLARE @nStep AS INTEGER
	DECLARE @xXML AS XML
	
	SELECT TOP 1 
		@dDataUltimoPeriodo=DataUltimoPeriodo,
		@nStep=Step
	FROM T_Schedulazioni 
	WHERE Codice='SCHSCH'
	
	IF @dDataUltimoPeriodo IS NOT NULL
	BEGIN
		SET @dDataInizio=@dDataUltimoPeriodo
		SET @dDataFine=DATEADD(minute,@nStep,@dDataInizio)
		
		SET @xXML=CONVERT(XML,'<Parametri>
									<DataInizio>' + CONVERT(VARCHAR(19),@dDataInizio,126) + '</DataInizio>	
									<DataFine>' + CONVERT(VARCHAR(19),@dDataFine,126) + '</DataFine>
								</Parametri>')

				
		BEGIN TRANSACTION
		
		INSERT INTO SCCILog.dbo.T_MovDataLog
			(Data,
			 DataUTC,
			 CodUtente,
			 ComputerName,
			 IpAddress,
			 CodEvento,
			 TipoOperazione,
			 Operazione,
			 LogPrima,
			 LogDopo
			 )
		VALUES
			( GETDATE()		
			 ,GETUTCDATE()
			 ,'WebNotificationService'
			 ,@@SERVERNAME
			 ,CONVERT(VARCHAR(50),CONNECTIONPROPERTY('local_net_address'))
			 ,'ScciEXS01'
			 ,1
			 ,'Nuovo'
			 ,NULL
			 ,@xXML
			 )
		
		IF @@ERROR=0
			BEGIN
				UPDATE T_Schedulazioni 
				SET DataUltimoPeriodo=@dDataFine,
					DataUltimaElaborazione=GETDATE()
				WHERE Codice='SCHSCH'
			END	
		IF @@ERROR=0 
			COMMIT TRANSACTION	 
		ELSE
			ROLLBACK TRANSACTION	
	END	
END