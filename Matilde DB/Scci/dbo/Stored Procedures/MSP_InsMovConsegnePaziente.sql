CREATE PROCEDURE [dbo].[MSP_InsMovConsegnePaziente](@xParametri XML)
AS
BEGIN
		
	
							
		DECLARE @uIDEpisodio AS UNIQUEIDENTIFIER
	DECLARE @sCodUA AS VARCHAR(20)
	DECLARE @sCodTipoConsegnaPaziente AS VARCHAR(20)
	DECLARE @sCodStatoConsegnaPaziente AS VARCHAR(20)
		
	DECLARE @sCodRuoloInserimento AS VARCHAR(20)
	DECLARE @sCodUtenteInserimento AS VARCHAR(100)					
		
		DECLARE @sGUID AS VARCHAR(50)
	DECLARE @uGUID AS UNIQUEIDENTIFIER

	    DECLARE @xTimeStampBase AS XML	
	DECLARE @xTimeStamp AS XML	
		
		DECLARE @xLogPrima AS XML
	DECLARE @xLogDopo AS XML
	DECLARE @xTemp AS XML
	DECLARE @xParLog AS XML

		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDEpisodio.value('.','VARCHAR(50)')
						FROM @xParametri.nodes('/Parametri/IDEpisodio') as ValoreParametro(IDEpisodio))						  
	IF 	ISNULL(@sGUID,'') <> '' 
			SET @uIDEpisodio=CONVERT(UNIQUEIDENTIFIER,	@sGUID)	

		SET @sCodUA=(SELECT TOP 1 ValoreParametro.CodUA.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodUA') as ValoreParametro(CodUA))	
	SET @sCodUA=ISNULL(@sCodUA,'')

		SET @sCodTipoConsegnaPaziente=(SELECT TOP 1 ValoreParametro.CodTipoConsegnaPaziente.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodTipoConsegnaPaziente') as ValoreParametro(CodTipoConsegnaPaziente))	
	SET @sCodTipoConsegnaPaziente=ISNULL(@sCodTipoConsegnaPaziente,'')

		SET @sCodStatoConsegnaPaziente=(SELECT TOP 1 ValoreParametro.CodStatoConsegnaPaziente.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodStatoConsegnaPaziente') as ValoreParametro(CodStatoConsegnaPaziente))	
	SET @sCodStatoConsegnaPaziente=ISNULL(@sCodStatoConsegnaPaziente,'')
	IF @sCodStatoConsegnaPaziente='' SET @sCodStatoConsegnaPaziente='IC'	
		SET @sCodRuoloInserimento=(SELECT TOP 1 ValoreParametro.CodRuolo.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/TimeStamp/CodRuolo') as ValoreParametro(CodRuolo))	
	SET @sCodRuoloInserimento=ISNULL(@sCodRuoloInserimento,'')
						
		SET @sCodUtenteInserimento=(SELECT TOP 1 ValoreParametro.CodLogin.value('.','VARCHAR(100)')
					FROM @xParametri.nodes('/Parametri/TimeStamp/CodLogin') as ValoreParametro(CodLogin))	
	SET @sCodUtenteInserimento=ISNULL(@sCodUtenteInserimento,'')
					
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
				INSERT INTO T_MovConsegnePaziente(
					  [ID]
					  ,[IDEpisodio]
					  ,[CodUA]
					  ,[CodRuoloInserimento]
					  ,[CodTipoConsegnaPaziente]
					  ,[CodStatoConsegnaPaziente]
					  ,[CodUtenteInserimento]
					  ,[CodUtenteUltimaModifica]
					  ,[CodUtenteAnnullamento]
					  ,[CodUtenteCancellazione]
					  ,[DataInserimento]
					  ,[DataInserimentoUTC]
					  ,[DataUltimaModifica]
					  ,[DataUltimaModificaUTC]
					  ,[DataAnnullamento]
					  ,[DataAnnullamentoUTC]
					  ,[DataCancellazione]
					  ,[DataCancellazioneUTC]
				  )
		VALUES
				(
					  @uGUID												  ,@uIDEpisodio											  ,@sCodUA												  ,@sCodRuoloInserimento								  ,@sCodTipoConsegnaPaziente 							  ,@sCodStatoConsegnaPaziente							  ,@sCodUtenteInserimento 								  ,NULL 												  ,NULL	 												  ,NULL	 												  ,GETDATE()											  ,GETUTCDATE()											  ,NULL													  ,NULL 												  ,NULL 												  ,NULL													  ,NULL 												  ,NULL												)
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
					(SELECT * FROM T_MovConsegnePaziente
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