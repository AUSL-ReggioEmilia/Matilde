CREATE PROCEDURE [dbo].[MSP_InsMovConsegne](@xParametri XML)
AS
BEGIN
		
	
							
		DECLARE @sCodUA AS VARCHAR(20)			
	DECLARE @dDataEvento AS DATETIME	
	DECLARE @dDataEventoUTC AS DATETIME
	
	DECLARE @sCodTipoConsegna AS VARCHAR(20)
	DECLARE @sCodStatoConsegna AS VARCHAR(20)
	
	DECLARE @sCodUtenteRilevazione AS VARCHAR(100)					
	DECLARE @dDataAnnullamento AS DATETIME	
	DECLARE @dDataAnnullamentoUTC AS DATETIME
	
	DECLARE @sCodSistema AS VARCHAR(20)
	DECLARE @sIDSistema AS VARCHAR(50)
	
	
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


		SET @sCodUA=(SELECT TOP 1 ValoreParametro.CodUA.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodUA') as ValoreParametro(CodUA))	
	SET @sCodUA=ISNULL(@sCodUA,'')
		
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
		
	SET @dDataEvento=ISNULL(@dDataEvento,Getdate())	

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
		
	SET @dDataEventoUTC=ISNULL(@dDataEvento,GetUTCdate())	

		SET @sCodStatoConsegna=(SELECT TOP 1 ValoreParametro.CodStatoConsegna.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodStatoConsegna') as ValoreParametro(CodStatoConsegna))	
	SET @sCodStatoConsegna=ISNULL(@sCodStatoConsegna,'')
	IF @sCodStatoConsegna='' SET @sCodStatoConsegna='IC'	
		IF @sCodStatoConsegna='AN'
	BEGIN		
		SET @dDataAnnullamento= @dDataEvento
		SET @dDataAnnullamentoUTC= @dDataEventoUTC
	END

		SET @sCodTipoConsegna=(SELECT TOP 1 ValoreParametro.CodTipoConsegna.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodTipoConsegna') as ValoreParametro(CodTipoConsegna))	
	SET @sCodTipoConsegna=ISNULL(@sCodTipoConsegna,'')
	
		
	
		SET @sCodUtenteRilevazione=(SELECT TOP 1 ValoreParametro.CodUtenteRilevazione.value('.','VARCHAR(100)')
					  FROM @xParametri.nodes('/Parametri/CodUtenteRilevazione') as ValoreParametro(CodUtenteRilevazione))	
	SET @sCodUtenteRilevazione=ISNULL(@sCodUtenteRilevazione,'')
	
	IF @sCodUtenteRilevazione=''
	BEGIN
		SET @sCodUtenteRilevazione=(SELECT TOP 1 ValoreParametro.CodUtenteRilevazione.value('.','VARCHAR(100)')
					  FROM @xParametri.nodes('/Parametri/TimeStamp/CodLogin') as ValoreParametro(CodUtenteRilevazione))	
		SET @sCodUtenteRilevazione=ISNULL(@sCodUtenteRilevazione,'')
	END	
		
			
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
				INSERT INTO T_MovConsegne(
					  [ID]
					  ,[CodUA]					  
					  ,[CodTipoConsegna]
					  ,[CodStatoConsegna]
					  ,[CodUtenteRilevazione]
					  ,[CodUtenteUltimaModifica]
					  ,[CodUtenteAnnullamento]
					  ,[DataEvento]
					  ,[DataEventoUTC]
					  ,[DataInserimento]
					  ,[DataInserimentoUTC]
					  ,[DataUltimaModifica]
					  ,[DataUltimaModificaUTC]
					  ,[DataAnnullamento]
					  ,[DataAnnullamentoUTC]
				  )
		VALUES
				(
					  @uGUID												  ,@sCodUA												  ,@sCodTipoConsegna 									  ,@sCodStatoConsegna 									  ,@sCodUtenteRilevazione 								  ,NULL 												  ,NULL	 												  ,@dDataEvento											  ,@dDataEventoUTC										  ,GETDATE()											  ,GETUTCDATE()											  ,NULL													  ,NULL 												  ,@dDataAnnullamento 									  ,@dDataAnnullamentoUTC							)
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
					(SELECT * FROM T_MovConsegne
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