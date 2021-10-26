CREATE PROCEDURE [dbo].[MSP_InsMovParametriVitali](@xParametri XML)
AS
BEGIN
		
	
			

								
	
	DECLARE @uIDEpisodio AS UNIQUEIDENTIFIER	
	DECLARE @uIDTrasferimento AS UNIQUEIDENTIFIER	
		
	DECLARE @dDataEvento AS DATETIME	
	DECLARE @dDataEventoUTC AS DATETIME
	
	DECLARE @sCodTipoParametroVitale AS VARCHAR(20)
	DECLARE @sCodStatoParametroVitale AS VARCHAR(20)
		
	DECLARE @txtValoriGrafici AS VARCHAR(MAX)
	DECLARE @xValoriGrafici AS XML
	
	DECLARE @txtValoriFUT AS VARCHAR(MAX)
	DECLARE @xValoriFUT AS XML
		
	DECLARE @sCodUtenteRilevazione AS VARCHAR(100)					
	
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

		SET @sCodTipoParametroVitale=(SELECT TOP 1 ValoreParametro.CodTipoParametroVitale.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodTipoParametroVitale') as ValoreParametro(CodTipoParametroVitale))	
	SET @sCodTipoParametroVitale=ISNULL(@sCodTipoParametroVitale,'')
	
		SET @sCodStatoParametroVitale=(SELECT TOP 1 ValoreParametro.CodStatoParametroVitale.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodStatoParametroVitale') as ValoreParametro(CodStatoParametroVitale))	
	SET @sCodStatoParametroVitale=ISNULL(@sCodStatoParametroVitale,'')
	IF @sCodStatoParametroVitale='' SET @sCodStatoParametroVitale='ER'		
	    SET @xValoriFUT=(SELECT TOP 1 ValoriFUT.TS.query('.')
							 FROM @xParametri.nodes('/Parametri/ValoriFUT/ValoriPVT') as ValoriFUT(TS)
						)
	    SET @xValoriGrafici=(SELECT TOP 1 ValoriGrafici.TS.query('.')
							 FROM @xParametri.nodes('/Parametri/ValoriGrafici/ValoriPVT') as ValoriGrafici(TS)
						)
						
		SET @sCodUtenteRilevazione=(SELECT TOP 1 ValoreParametro.CodUtenteRilevazione.value('.','VARCHAR(100)')
					  FROM @xParametri.nodes('/Parametri/TimeStamp/CodLogin') as ValoreParametro(CodUtenteRilevazione))	
	SET @sCodUtenteRilevazione=ISNULL(@sCodUtenteRilevazione,'')
	
	    SET @xTimeStamp=(SELECT TOP 1 ValoreParametro.TS.query('.')
					  FROM @xParametri.nodes('/Parametri/TimeStamp') as ValoreParametro(TS))
	
		SET @xSchedaMovimento=(SELECT TOP 1 ValoreParametro.TS.query('.')
					  FROM @xParametri.nodes('/Parametri/SchedaMovimento') as ValoreParametro(TS))					

		SET @sCodSistema=(SELECT TOP 1 ValoreParametro.CodSistema.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodSistema') as ValoreParametro(CodSistema))	
	
		SET @sIDSistema=(SELECT TOP 1 ValoreParametro.IDSistema.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDSistema') as ValoreParametro(IDSistema))	
						  
					
	SET @uGUID=NEWID()

					SET @xTimeStamp.modify('delete (/TimeStamp/IDEntita)[1]') 
			
	SET @xTimeStamp.modify('insert <IDEntita>{sql:variable("@uGUID")}</IDEntita> into (/TimeStamp)[1]')

		SET @xTimeStampBase=@xTimeStamp
	
	SET @xTimeStamp=CONVERT(XML,
						'<Parametri>' + CONVERT(varchar(max),@xTimeStamp) +
						'</Parametri>')
						
															
	BEGIN TRANSACTION
				INSERT INTO T_MovParametriVitali(
					  ID		
					  ,DataEvento
					  ,DataEventoUTC			  
					  ,IDEpisodio
					  ,IDTrasferimento					  
					  ,CodTipoParametroVitale
					  ,CodStatoParametroVitale					  
					  ,ValoriFUT
					  ,ValoriGrafici
					  ,CodUtenteRilevazione
					  ,CodUtenteUltimaModifica
					  ,DataUltimaModifica
					  ,DataUltimaModificaUTC
					  ,DataInserimento
					  ,DataInserimentoUTC
					  ,CodSistema 												  ,IDSistema 											  )
		VALUES
				(
					  @uGUID													  ,ISNULL(@dDataEvento,Getdate())							  ,ISNULL(@dDataEventoUTC,GetUTCdate()) 					  ,@uIDEpisodio 											  ,@uIDTrasferimento 										  ,@sCodTipoParametroVitale 								  ,@sCodStatoParametroVitale 								  ,@xValoriFUT 	 											  ,@xValoriGrafici 											  ,@sCodUtenteRilevazione 									  ,NULL 													  ,NULL 													  ,NULL 													  ,Getdate()												  ,GetUTCdate()												  ,@sCodSistema 											  ,@sIDSistema 												  	  						
				)
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
					(SELECT * FROM T_MovParametriVitali
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
				   ,@xParLog AS ParLog
		END	
	ELSE
		BEGIN
			ROLLBACK TRANSACTION	
			SELECT NULL AS ID
		END	 
	
	RETURN 0
END