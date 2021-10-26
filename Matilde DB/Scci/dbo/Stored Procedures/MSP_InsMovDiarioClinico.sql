CREATE PROCEDURE [dbo].[MSP_InsMovDiarioClinico](@xParametri XML)
AS
BEGIN
		
	
							
		DECLARE @sCodUA AS VARCHAR(20)			
	DECLARE @dDataEvento AS DATETIME	
	DECLARE @dDataEventoUTC AS DATETIME
	DECLARE @uIDEpisodio AS UNIQUEIDENTIFIER	
	DECLARE @uIDTrasferimento AS UNIQUEIDENTIFIER	
	
	DECLARE @sCodTipoDiario AS VARCHAR(20)
	DECLARE @sCodTipoVoceDiario AS VARCHAR(20)
	DECLARE @sCodTipoRegistrazione AS VARCHAR(20)
	DECLARE @sCodEntitaRegistrazione AS VARCHAR(20)
	DECLARE @uIDEntitaRegistrazione AS UNIQUEIDENTIFIER
	DECLARE @sCodStatoDiario AS VARCHAR(20)
	DECLARE @sCodUtenteRilevazione AS VARCHAR(100)					
	DECLARE @dDataValidazione AS DATETIME	
	DECLARE @dDataValidazioneUTC AS DATETIME
	
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
		
		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDEpisodio.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDEpisodio') as ValoreParametro(IDEpisodio))
	IF 	ISNULL(@sGUID,'') <> '' 
			SET @uIDEpisodio=CONVERT(UNIQUEIDENTIFIER,	@sGUID)		

		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDTrasferimento.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDTrasferimento') as ValoreParametro(IDTrasferimento))
	IF 	ISNULL(@sGUID,'') <> '' 
			SET @uIDTrasferimento=CONVERT(UNIQUEIDENTIFIER,	@sGUID)		

		SET @sCodTipoVoceDiario=(SELECT TOP 1 ValoreParametro.CodTipoVoceDiario.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodTipoVoceDiario') as ValoreParametro(CodTipoVoceDiario))	
	SET @sCodTipoVoceDiario=ISNULL(@sCodTipoVoceDiario,'')
	
		SET @sCodTipoDiario=(SELECT TOP 1 ValoreParametro.CodTipoDiario.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodTipoDiario') as ValoreParametro(CodTipoDiario))	
	SET @sCodTipoDiario=ISNULL(@sCodTipoDiario,'')
	IF @sCodTipoDiario =''
		BEGIN
						SET @sCodTipoDiario=(SELECT TOP 1 CodTipoDiario FROM T_TipoVoceDiario WHERE Codice=@sCodTipoVoceDiario)
			SET @sCodTipoDiario=ISNULL(@sCodTipoDiario,'')
		END
	
		SET @sCodEntitaRegistrazione=(SELECT TOP 1 ValoreParametro.CodEntitaRegistrazione.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodEntitaRegistrazione') as ValoreParametro(CodEntitaRegistrazione))	
	SET @sCodEntitaRegistrazione=ISNULL(@sCodEntitaRegistrazione,'')			
	IF @sCodEntitaRegistrazione='' SET @sCodEntitaRegistrazione ='DCL'								
		SET @sCodTipoRegistrazione=(SELECT TOP 1 ValoreParametro.CodTipoRegistrazione.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodTipoRegistrazione') as ValoreParametro(CodTipoRegistrazione))	
	SET @sCodTipoRegistrazione=ISNULL(@sCodTipoRegistrazione,'')				
	IF @sCodTipoRegistrazione='' SET @sCodTipoRegistrazione ='M'									
		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDEntitaRegistrazione.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDEntitaRegistrazione') as ValoreParametro(IDEntitaRegistrazione))
	IF 	ISNULL(@sGUID,'') <> '' SET @uIDEntitaRegistrazione=CONVERT(UNIQUEIDENTIFIER,	@sGUID)		  
						  
		SET @sCodStatoDiario=(SELECT TOP 1 ValoreParametro.CodStatoDiario.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodStatoDiario') as ValoreParametro(CodStatoDiario))	
	SET @sCodStatoDiario=ISNULL(@sCodStatoDiario,'')
	IF @sCodStatoDiario='' SET @sCodStatoDiario='IC'										
		SET @sCodUtenteRilevazione=(SELECT TOP 1 ValoreParametro.CodUtenteRilevazione.value('.','VARCHAR(100)')
					  FROM @xParametri.nodes('/Parametri/CodUtenteRilevazione') as ValoreParametro(CodUtenteRilevazione))	
	SET @sCodUtenteRilevazione=ISNULL(@sCodUtenteRilevazione,'')
	
	IF @sCodUtenteRilevazione=''
	BEGIN
		SET @sCodUtenteRilevazione=(SELECT TOP 1 ValoreParametro.CodUtenteRilevazione.value('.','VARCHAR(100)')
					  FROM @xParametri.nodes('/Parametri/TimeStamp/CodLogin') as ValoreParametro(CodUtenteRilevazione))	
		SET @sCodUtenteRilevazione=ISNULL(@sCodUtenteRilevazione,'')
	END	
	
		SET @sDataTmp=(SELECT TOP 1 ValoreParametro.DataValidazione.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/DataValidazione') as ValoreParametro(DataValidazione))					  
	SET @sDataTmp=ISNULL(@sDataTmp,'')
	
	IF @sDataTmp<> '' 
		BEGIN			
						SET @sDataTmp= SUBSTRING(@sDataTmp,4,2) + '-' 
						+ LEFT(@sDataTmp,2) + '-' +
						+ SUBSTRING(@sDataTmp,7,4) +
						' ' + RIGHT(@sDataTmp,5)
			IF ISDATE(@sDataTmp)=1
				SET	@dDataValidazione=CONVERT(DATETIME,@sDataTmp,120)						
			ELSE
				SET	@dDataValidazione =NULL			
		END
		
		SET @sDataTmp=(SELECT TOP 1 ValoreParametro.DataValidazioneUTC.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/DataValidazioneUTC') as ValoreParametro(DataValidazioneUTC))					  
	SET @sDataTmp=ISNULL(@sDataTmp,'')
	
		IF @sDataTmp<> '' 
		BEGIN			
						SET @sDataTmp= SUBSTRING(@sDataTmp,4,2) + '-' 
						+ LEFT(@sDataTmp,2) + '-' +
						+ SUBSTRING(@sDataTmp,7,4) +
						' ' + RIGHT(@sDataTmp,5)
			IF ISDATE(@sDataTmp)=1
				SET	@dDataValidazioneUTC=CONVERT(DATETIME,@sDataTmp,120)						
			ELSE
				SET	@dDataValidazioneUTC =NULL			
		END
			
	    SET @xTimeStamp=(SELECT TOP 1 ValoreParametro.TS.query('.')
					  FROM @xParametri.nodes('/Parametri/TimeStamp') as ValoreParametro(TS))
		
		SET @xSchedaMovimento=(SELECT TOP 1 ValoreParametro.TS.query('.')
					  FROM @xParametri.nodes('/Parametri/SchedaMovimento') as ValoreParametro(TS))					
	
		SET @sCodSistema=(SELECT TOP 1 ValoreParametro.CodSistema.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodSistema') as ValoreParametro(CodSistema))	
	
		SET @sIDSistema=(SELECT TOP 1 ValoreParametro.IDSistema.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDSistema') as ValoreParametro(IDSistema))	
					  						
					
	SET @uGUID=NEWID()
	
		IF 	@sCodEntitaRegistrazione='DCL' AND @uIDEntitaRegistrazione IS NULL
			SET @uIDEntitaRegistrazione=@uGUID	

					SET @xTimeStamp.modify('delete (/TimeStamp/IDEntita)[1]') 
			
	SET @xTimeStamp.modify('insert <IDEntita>{sql:variable("@uGUID")}</IDEntita> into (/TimeStamp)[1]')

		SET @xTimeStampBase=@xTimeStamp

	SET @xTimeStamp=CONVERT(XML,
						'<Parametri>' + CONVERT(varchar(max),@xTimeStamp) +
						'</Parametri>')
														
	BEGIN TRANSACTION
				INSERT INTO T_MovDiarioClinico(
					   ID		
					  ,CodUA			  
					  ,DataEvento
					  ,DataEventoUTC
					  ,IDEpisodio
					  ,IDTrasferimento
					  ,CodTipoDiario
					  ,CodTipoVoceDiario
					  ,CodTipoRegistrazione
					  ,CodEntitaRegistrazione
					  ,IDEntitaRegistrazione
					  ,CodStatoDiario
					  ,CodUtenteRilevazione
					  ,DataInserimento
					  ,DataInserimentoUTC
					  ,DataValidazione
					  ,DataValidazioneUTC
					  ,DataAnnullamento
					  ,DataAnnullamentoUTC
					  ,CodSistema
					  ,IDSistema
				  )
		VALUES
				(
					  @uGUID												  ,@sCodUA												  ,ISNULL(@dDataEvento,Getdate())						  ,ISNULL(@dDataEventoUTC,GetUTCdate()) 					  ,@uIDEpisodio 										  ,@uIDTrasferimento 									  ,@sCodTipoDiario 										  ,@sCodTipoVoceDiario 									  ,@sCodTipoRegistrazione 								  ,@sCodEntitaRegistrazione 							  ,@uIDEntitaRegistrazione 								  ,@sCodStatoDiario 									  ,@sCodUtenteRilevazione 								  ,Getdate()											  ,GetUTCdate()											  ,@dDataValidazione									  ,@dDataValidazioneUTC 								  ,NULL 												  ,NULL 												  ,@sCodSistema 											  ,@sIDSistema 											)
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
					(SELECT * FROM T_MovDiarioClinico
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