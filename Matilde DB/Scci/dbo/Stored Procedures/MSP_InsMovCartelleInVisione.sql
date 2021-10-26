
CREATE PROCEDURE [dbo].[MSP_InsMovCartelleInVisione](@xParametri AS XML)
AS
BEGIN
	
											
		DECLARE @sCodRuolo AS VARCHAR(20)
	DECLARE @sCodUtente AS VARCHAR(100)	
	
	DECLARE @uIDCartella AS UNIQUEIDENTIFIER		
	DECLARE @uIDEpisodio AS UNIQUEIDENTIFIER	
	DECLARE @uIDTrasferimento AS UNIQUEIDENTIFIER	
	DECLARE @sCodRuoloInVisione AS VARCHAR(20)
	DECLARE @sCodStatoCartellaInVisione AS VARCHAR(20)	
	
	DECLARE @dDataInizio AS DATETIME	
	DECLARE @dDataInizioUTC AS DATETIME
	
	DECLARE @dDataFine AS DATETIME	
	DECLARE @dDataFineUTC AS DATETIME			
		
		DECLARE @sGUID AS VARCHAR(50)
	DECLARE @uGUID AS UNIQUEIDENTIFIER
	DECLARE @sDataTmp AS VARCHAR(20)	
	DECLARE @nQTA AS INTEGER
	
		
	    DECLARE @xTimeStampBase AS XML	
	DECLARE @xTimeStamp AS XML
	DECLARE @xLogPrima AS XML
	DECLARE @xLogDopo AS XML
	DECLARE @xTemp AS XML
	DECLARE @xParLog AS XML	
	
		SET @sCodUtente=(SELECT TOP 1 ValoreParametro.CodUtente.value('.','VARCHAR(100)')
					  FROM @xParametri.nodes('/Parametri/TimeStamp/CodLogin') as ValoreParametro(CodUtente))	
	SET @sCodUtente=ISNULL(@sCodUtente,'')

		SET @sCodRuolo=(SELECT TOP 1 ValoreParametro.CodRuolo.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/TimeStamp/CodRuolo') as ValoreParametro(CodRuolo))
	SET @sCodRuolo=ISNULL(@sCodRuolo,'')	
	
		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDEpisodio.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/TimeStamp/IDEpisodio') as ValoreParametro(IDEpisodio))
	IF 	ISNULL(@sGUID,'') <> '' 
			SET @uIDEpisodio=CONVERT(UNIQUEIDENTIFIER,	@sGUID)	
	
		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDTrasferimento.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/TimeStamp/IDTrasferimento') as ValoreParametro(IDTrasferimento))
	IF 	ISNULL(@sGUID,'') <> '' 
			SET @uIDTrasferimento=CONVERT(UNIQUEIDENTIFIER,	@sGUID)	
					
		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDCartella.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDCartella') as ValoreParametro(IDCartella))
	IF 	ISNULL(@sGUID,'') <> '' 
			SET @uIDCartella=CONVERT(UNIQUEIDENTIFIER,	@sGUID)	
			
		SET @sCodRuoloInVisione=(SELECT TOP 1 ValoreParametro.CodRuoloInVisione.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodRuoloInVisione') as ValoreParametro(CodRuoloInVisione))	
	
		SET @sCodStatoCartellaInVisione=(SELECT TOP 1 ValoreParametro.CodStatoCartellaInVisione.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodStatoCartellaInVisione') as ValoreParametro(CodStatoCartellaInVisione))	
				
	
		SET @sDataTmp=(SELECT TOP 1 ValoreParametro.DataInizio.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/DataInizio') as ValoreParametro(DataInizio))					  
	SET @sDataTmp=ISNULL(@sDataTmp,'')	

	IF @sDataTmp<> '' 
		BEGIN			
						SET @sDataTmp= SUBSTRING(@sDataTmp,4,2) + '-' 
						+ LEFT(@sDataTmp,2) + '-' +
						+ SUBSTRING(@sDataTmp,7,4) +
						' ' + RIGHT(@sDataTmp,5)
			IF ISDATE(@sDataTmp)=1
				SET	@dDataInizio=CONVERT(DATETIME,@sDataTmp,120)						
			ELSE
				SET	@dDataInizio =NULL			
		END
		
		SET @sDataTmp=(SELECT TOP 1 ValoreParametro.DataInizioUTC.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/DataInizioUTC') as ValoreParametro(DataInizioUTC))					  
	SET @sDataTmp=ISNULL(@sDataTmp,'')
	
		IF @sDataTmp<> '' 
		BEGIN			
						SET @sDataTmp= SUBSTRING(@sDataTmp,4,2) + '-' 
						+ LEFT(@sDataTmp,2) + '-' +
						+ SUBSTRING(@sDataTmp,7,4) +
						' ' + RIGHT(@sDataTmp,5)
			IF ISDATE(@sDataTmp)=1
				SET	@dDataInizioUTC =CONVERT(DATETIME,@sDataTmp,120)						
			ELSE
				SET	@dDataInizioUTC  =NULL			
		END
	
		SET @sDataTmp=(SELECT TOP 1 ValoreParametro.DataFine.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/DataFine') as ValoreParametro(DataFine))					  
	SET @sDataTmp=ISNULL(@sDataTmp,'')	

	IF @sDataTmp<> '' 
		BEGIN			
						SET @sDataTmp= SUBSTRING(@sDataTmp,4,2) + '-' 
						+ LEFT(@sDataTmp,2) + '-' +
						+ SUBSTRING(@sDataTmp,7,4) +
						' ' + RIGHT(@sDataTmp,5)
			IF ISDATE(@sDataTmp)=1
				SET	@dDataFine=CONVERT(DATETIME,@sDataTmp,120)						
			ELSE
				SET	@dDataFine =NULL			
		END
		
		SET @sDataTmp=(SELECT TOP 1 ValoreParametro.DataFineUTC.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/DataFineUTC') as ValoreParametro(DataFineUTC))					  
	SET @sDataTmp=ISNULL(@sDataTmp,'')
	
		IF @sDataTmp<> '' 
		BEGIN			
						SET @sDataTmp= SUBSTRING(@sDataTmp,4,2) + '-' 
						+ LEFT(@sDataTmp,2) + '-' +
						+ SUBSTRING(@sDataTmp,7,4) +
						' ' + RIGHT(@sDataTmp,5)
			IF ISDATE(@sDataTmp)=1
				SET	@dDataFineUTC  =CONVERT(DATETIME,@sDataTmp,120)						
			ELSE
				SET	@dDataFineUTC   =NULL			
		END
	
	    SET @xTimeStamp=(SELECT TOP 1 ValoreParametro.TS.query('.')
					  FROM @xParametri.nodes('/Parametri/TimeStamp') as ValoreParametro(TS))	
	
				
	SET @uGUID=NEWID()
	
	BEGIN TRANSACTION
		INSERT INTO T_MovCartelleInVisione
			   (ID
			   ,IDCartella
			   ,IDEpisodio
			   ,IDTrasferimento
			   ,CodRuoloInVisione
			   ,DataInizio
			   ,DataInizioUTC
			   ,DataFine
			   ,DataFineUTC
			   ,CodStatoCartellaInVisione
			   ,DataInserimento
			   ,DataInserimentoUTC
			   ,CodUtenteInserimento
			   ,DataUltimaModifica
			   ,DataUltimaModificaUTC
			   ,CodUtenteUltimaModifica)
		 VALUES
				(@uGUID											   ,@uIDCartella									   ,@uIDEpisodio									   ,@uIDTrasferimento								   ,@sCodRuoloInVisione								   ,@dDataInizio										   ,@dDataInizioUTC									   ,@dDataFine										   ,@dDataFineUTC									   ,@sCodStatoCartellaInVisione						   ,GetDate()										   ,GetUTCDate()									   ,@sCodUtente										   ,NULL											   ,NULL											   ,NULL												)
		IF @@ERROR=0 
			BEGIN
																				SET @xTimeStamp.modify('delete (/TimeStamp/IDEntita)[1]') 					
				SET @xTimeStamp.modify('insert <IDEntita>{sql:variable("@uGUID")}</IDEntita> into (/TimeStamp)[1]')
				
								SET @xTimeStampBase=@xTimeStamp


				SET @xTimeStamp=CONVERT(XML,
									'<Parametri>' + CONVERT(varchar(max),@xTimeStamp) +
									'</Parametri>')

								EXEC MSP_InsMovTimeStamp @xTimeStamp		
			END	
		
		IF @@ERROR = 0
		BEGIN
			COMMIT TRANSACTION
												
			
			SET @xLogDopo=Convert(XML,'<DataSet></DataSet>')
			SET @xLogPrima=''
			
			SET @xTemp=
				(SELECT * FROM 
					(SELECT * FROM T_MovCartelleInVisione
					 WHERE ID=@uGUID
					) AS [Table]
				FOR XML AUTO, ELEMENTS)

			SET @xLogDopo.modify('insert sql:variable("@xTemp") as first into (/DataSet)[1]')				
			
			SET @xParLog=CONVERT(XML,'<Parametri><LogDopo/></Parametri>')			
			SET @xParLog.modify('insert sql:variable("@xLogDopo") as first into (/Parametri/LogDopo)[1]')
			SET @xParLog.modify('insert sql:variable("@xTimeStampBase") as last into (/Parametri)[1]')
						
			EXEC MSP_InsMovLog @xParLog
									
			SELECT @uGUID AS IDCartellaInVisione
		END	
	ELSE
		BEGIN
			ROLLBACK TRANSACTION	
			PRINT 'ERRORE'
			SELECT NULL AS IDEpisodio
		END	 	
	
															
	RETURN 0
END