CREATE PROCEDURE [dbo].[MSP_InsMovConsegnePazienteRuoli](@xParametri XML)
AS
BEGIN
		
	
							
		DECLARE @uIDConsegnaPaziente AS UNIQUEIDENTIFIER		
	DECLARE @sCodStatoConsegnaPazienteRuolo AS VARCHAR(20)
	DECLARE @sCodRuolo AS VARCHAR(20)		
	DECLARE @sCodUtente AS VARCHAR(100)					
		
		DECLARE @sGUID AS VARCHAR(50)
	DECLARE @uGUID AS UNIQUEIDENTIFIER

	    DECLARE @xTimeStampBase AS XML	
	DECLARE @xTimeStamp AS XML	
		
		DECLARE @xLogPrima AS XML
	DECLARE @xLogDopo AS XML
	DECLARE @xTemp AS XML
	DECLARE @xParLog AS XML

		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDConsegnaPaziente.value('.','VARCHAR(50)')
				  FROM @xParametri.nodes('/Parametri/IDConsegnaPaziente') as ValoreParametro(IDConsegnaPaziente))						  
	IF 	ISNULL(@sGUID,'') <> '' 
				SET @uIDConsegnaPaziente=CONVERT(UNIQUEIDENTIFIER,	@sGUID)			

		SET @sCodStatoConsegnaPazienteRuolo=(SELECT TOP 1 ValoreParametro.CodStatoConsegnaPazienteRuolo.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodStatoConsegnaPazienteRuolo') as ValoreParametro(CodStatoConsegnaPazienteRuolo))	
	SET @sCodStatoConsegnaPazienteRuolo=ISNULL(@sCodStatoConsegnaPazienteRuolo,'')
	IF @sCodStatoConsegnaPazienteRuolo='' SET @sCodStatoConsegnaPazienteRuolo='IC'	
		SET @sCodRuolo=(SELECT TOP 1 ValoreParametro.CodRuolo.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodRuolo') as ValoreParametro(CodRuolo))	
	SET @sCodRuolo=ISNULL(@sCodRuolo,'')
						
		SET @sCodUtente=(SELECT TOP 1 ValoreParametro.CodLogin.value('.','VARCHAR(100)')
					FROM @xParametri.nodes('/Parametri/TimeStamp/CodLogin') as ValoreParametro(CodLogin))	
	SET @sCodUtente=ISNULL(@sCodUtente,'')
					
	    SET @xTimeStamp=(SELECT TOP 1 ValoreParametro.TS.query('.')
					  FROM @xParametri.nodes('/Parametri/TimeStamp') as ValoreParametro(TS))
				  					  						
					
	SET @uGUID=NEWID()
	
					SET @xTimeStamp.modify('delete (/TimeStamp/IDEntita)[1]') 
			
	SET @xTimeStamp.modify('insert <IDEntita>{sql:variable("@uGUID")}</IDEntita> into (/TimeStamp)[1]')

		SET @xTimeStampBase=@xTimeStamp

	SET @xTimeStamp=CONVERT(XML,
						'<Parametri>' + CONVERT(varchar(max),@xTimeStamp) +
						'</Parametri>')
														
	BEGIN TRANSACTION
				INSERT INTO T_MovConsegnePazienteRuoli(
					  [ID]
					  ,[IDConsegnaPaziente]
					  ,[CodStatoConsegnaPazienteRuolo]
					  ,[CodRuolo]
					  ,[CodUtenteInserimento]
					  ,[CodUtenteUltimaModifica]
					  ,[CodUtenteAnnullamento]
					  ,[CodUtenteCancellazione]
					  ,[CodUtenteVisione]
					  ,[DataInserimento]
					  ,[DataInserimentoUTC]
					  ,[DataUltimaModifica]
					  ,[DataUltimaModificaUTC]
					  ,[DataAnnullamento]
					  ,[DataAnnullamentoUTC]
					  ,[DataCancellazione]
					  ,[DataCancellazioneUTC]
					  ,[DataVisione]
					  ,[DataVisioneUTC]
				  )
		VALUES
				(
					  @uGUID												  ,@uIDConsegnaPaziente									  ,@sCodStatoConsegnaPazienteRuolo						  ,@sCodRuolo											  ,@sCodUtente			 								  ,NULL 												  ,NULL	 												  ,NULL	 												  ,NULL	 												  ,GETDATE()											  ,GETUTCDATE()											  ,NULL													  ,NULL 												  ,NULL 												  ,NULL													  ,NULL 												  ,NULL													  ,NULL 												  ,NULL												)
	IF @@ERROR=0 AND @@ROWCOUNT>0
		BEGIN
					
						EXEC MSP_InsMovTimeStamp @xTimeStamp		
				
			
		END	
	IF @@ERROR = 0
		BEGIN
			COMMIT TRANSACTION
			
												
			SET @xLogDopo=Convert(XML,'<DataSet></DataSet>')
			SET @xLogPrima=''
			
			SET @xTemp=
				(SELECT * FROM 
					(SELECT * FROM T_MovConsegnePazienteRuoli
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