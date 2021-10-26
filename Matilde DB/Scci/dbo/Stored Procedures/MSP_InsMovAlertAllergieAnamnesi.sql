CREATE PROCEDURE [dbo].[MSP_InsMovAlertAllergieAnamnesi](@xParametri XML)
AS
BEGIN
		
	
				

								
		DECLARE @uIDPaziente AS UNIQUEIDENTIFIER	
	DECLARE @dDataEvento AS DATETIME	
	DECLARE @dDataEventoUTC AS DATETIME
	DECLARE @uIDEpisodio AS UNIQUEIDENTIFIER	
	
	DECLARE @sCodTipoAlertAllergiaAnamnesi AS VARCHAR(20)
	DECLARE @sCodStatoAlertAllergiaAnamnesi AS VARCHAR(20)

	DECLARE @sCodSistema AS VARCHAR(20)
	DECLARE @sIDSistema AS VARCHAR(50)
	DECLARE @sIDGruppo AS VARCHAR(50)

	
	DECLARE @sCodUtenteRilevazione AS VARCHAR(100)					
	
		DECLARE @sGUID AS VARCHAR(50)
	DECLARE @uGUID AS UNIQUEIDENTIFIER
	DECLARE @sDataTmp AS VARCHAR(20)	
	
	    DECLARE @xTimeStampBase AS XML	
	DECLARE @xTimeStamp AS XML	
		
		DECLARE @xSchedaMovimento AS XML
		
		DECLARE @xLogPrima AS XML
	DECLARE @xLogDopo AS XML
	DECLARE @xTemp AS XML
	DECLARE @xParLog AS XML


		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDPaziente.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDPaziente') as ValoreParametro(IDPaziente))
	IF 	ISNULL(@sGUID,'') <> '' 
			SET @uIDPaziente=CONVERT(UNIQUEIDENTIFIER,	@sGUID)		

		
		SET @sDataTmp=(SELECT TOP 1 ValoreParametro.DataEvento.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/DataEvento') as ValoreParametro(DataEvento))					  
	SET @sDataTmp=ISNULL(@sDataTmp,'')
	

	IF @sDataTmp<> '' 
		BEGIN			
						SET @sDataTmp= SUBSTRING(@sDataTmp,4,2) + '-' 
						+ LEFT(@sDataTmp,2) + '-' +
						+ SUBSTRING(@sDataTmp,7,4) +
						' ' + RIGHT(@sDataTmp,5)
			IF ISDATE(@sDataTmp)=1
				SET	@dDataEvento=CONVERT(DATETIME,@sDataTmp,120)						
			ELSE
				SET	@dDataEvento =NULL			
		END
		
		SET @sDataTmp=(SELECT TOP 1 ValoreParametro.DataEventoUTC.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/DataEventoUTC') as ValoreParametro(DataEventoUTC))					  
	SET @sDataTmp=ISNULL(@sDataTmp,'')
	
		IF @sDataTmp<> '' 
		BEGIN			
						SET @sDataTmp= SUBSTRING(@sDataTmp,4,2) + '-' 
						+ LEFT(@sDataTmp,2) + '-' +
						+ SUBSTRING(@sDataTmp,7,4) +
						' ' + RIGHT(@sDataTmp,5)
			IF ISDATE(@sDataTmp)=1
				SET	@dDataEventoUTC=CONVERT(DATETIME,@sDataTmp,120)						
			ELSE
				SET	@dDataEventoUTC =NULL			
		END
		



		SET @sCodTipoAlertAllergiaAnamnesi=(SELECT TOP 1 ValoreParametro.CodTipoAlertAllergiaAnamnesi.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodTipoAlertAllergiaAnamnesi') as ValoreParametro(CodTipoAlertAllergiaAnamnesi))	
	SET @sCodTipoAlertAllergiaAnamnesi=ISNULL(@sCodTipoAlertAllergiaAnamnesi,'')
	
		SET @sCodStatoAlertAllergiaAnamnesi=(SELECT TOP 1 ValoreParametro.CodStatoAlertAllergiaAnamnesi.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodStatoAlertAllergiaAnamnesi') as ValoreParametro(CodStatoAlertAllergiaAnamnesi))	
	SET @sCodStatoAlertAllergiaAnamnesi=ISNULL(@sCodStatoAlertAllergiaAnamnesi,'')
	IF @sCodStatoAlertAllergiaAnamnesi='' SET @sCodStatoAlertAllergiaAnamnesi='AT'		
	
		SET @sCodSistema=(SELECT TOP 1 ValoreParametro.CodSistema.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodSistema') as ValoreParametro(CodSistema))	
	SET @sCodSistema=ISNULL(@sCodSistema,'')
	
		SET @sIDSistema=(SELECT TOP 1 ValoreParametro.IDSistema.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDSistema') as ValoreParametro(IDSistema))		
	
		SET @sIDGruppo=(SELECT TOP 1 ValoreParametro.IDGruppo.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDGruppo') as ValoreParametro(IDGruppo))	
			
										
		SET @sCodUtenteRilevazione=(SELECT TOP 1 ValoreParametro.CodUtenteRilevazione.value('.','VARCHAR(100)')
					  FROM @xParametri.nodes('/Parametri/TimeStamp/CodLogin') as ValoreParametro(CodUtenteRilevazione))	
	SET @sCodUtenteRilevazione=ISNULL(@sCodUtenteRilevazione,'')
	
	    SET @xTimeStamp=(SELECT TOP 1 ValoreParametro.TS.query('.')
					  FROM @xParametri.nodes('/Parametri/TimeStamp') as ValoreParametro(TS))
		
		SET @xSchedaMovimento=(SELECT TOP 1 ValoreParametro.TS.query('.')
					  FROM @xParametri.nodes('/Parametri/SchedaMovimento') as ValoreParametro(TS))					
										
					
	SET @uGUID=NEWID()

					SET @xTimeStamp.modify('delete (/TimeStamp/IDEntita)[1]') 
			
	SET @xTimeStamp.modify('insert <IDEntita>{sql:variable("@uGUID")}</IDEntita> into (/TimeStamp)[1]')

		SET @xTimeStampBase=@xTimeStamp

	SET @xTimeStamp=CONVERT(XML,
						'<Parametri>' + CONVERT(varchar(max),@xTimeStamp) +
						'</Parametri>')
													
	BEGIN TRANSACTION
				INSERT INTO T_MovAlertAllergieAnamnesi(
					   ID					  
					  ,IDPaziente
					  ,DataEvento
					  ,DataEventoUTC
					  ,CodTipoAlertAllergiaAnamnesi
					  ,CodStatoAlertAllergiaAnamnesi		
					  ,CodSistema
					  ,IDSistema
				      ,IDGruppo
					  ,CodUtenteRilevazione
					  ,CodUtenteUltimaModifica
					  ,DataUltimaModifica
					  ,DataUltimaModificaUTC
				  )
		VALUES
				( 
					   @uGUID															   ,@uIDPaziente 													  ,ISNULL(@dDataEvento,Getdate())									  ,ISNULL(@dDataEventoUTC,GetUTCdate())								  ,@sCodTipoAlertAllergiaAnamnesi									  ,@sCodStatoAlertAllergiaAnamnesi									  ,@sCodSistema 													  ,@sIDSistema 														  ,@sIDGruppo 														  ,@sCodUtenteRilevazione 											  ,NULL 															  ,NULL 															  ,NULL 														)
				
	IF @@ERROR=0 
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
					(SELECT * FROM T_MovAlertAllergieAnamnesi
					 WHERE ID=@uGUID
					) AS [Table]
				FOR XML AUTO, ELEMENTS)

			SET @xLogDopo.modify('insert sql:variable("@xTemp") as first into (/DataSet)[1]')
	
						
			SET @xParLog=CONVERT(XML,'<Parametri><LogDopo/></Parametri>')			
			SET @xParLog.modify('insert sql:variable("@xLogDopo") as first into (/Parametri/LogDopo)[1]')
			SET @xParLog.modify('insert sql:variable("@xTimeStampBase") as last into (/Parametri)[1]')
			
						SET @xParLog.modify('insert sql:variable("@xSchedaMovimento") as last into (/Parametri/LogDopo/DataSet)[1]')

			
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