CREATE PROCEDURE [dbo].[MSP_AggMovAlertAllergieAnamnesi](@xParametri XML)
AS
BEGIN
		
	
				

								
		DECLARE @uIDAlertAllergiaAnamnesi AS UNIQUEIDENTIFIER	
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
	DECLARE @sCodUtenteUltimaModifica AS VARCHAR(100)					
	
	
		DECLARE @sGUID AS VARCHAR(50)
	DECLARE @uGUID AS UNIQUEIDENTIFIER
	DECLARE @sDataTmp AS VARCHAR(20)	
	DECLARE @sSQL AS VARCHAR(MAX)
	DECLARE @sSET AS VARCHAR(MAX)
	DECLARE @sWHERE AS VARCHAR(MAX)

	    DECLARE @xTimeStampBase AS XML	
	DECLARE @xTimeStamp AS XML	

		DECLARE @xSchedaMovimento AS XML			
	
		DECLARE @xLogPrima AS XML
	DECLARE @xLogDopo AS XML
	DECLARE @xTemp AS XML
	DECLARE @xParLog AS XML

		SET @sSQL='UPDATE T_MovAlertAllergieAnamnesi ' + CHAR(13) + CHAR(10) +
			  'SET '  				  						
			
	SET @sSET =''	

		IF @xParametri.exist('/Parametri/IDAlertAllergiaAnamnesi')=1
		BEGIN
			
			SET @sGUID=(SELECT TOP 1 ValoreParametro.IDAlertAllergiaAnamnesi.value('.','VARCHAR(50)')
							  FROM @xParametri.nodes('/Parametri/IDAlertAllergiaAnamnesi') as ValoreParametro(IDAlertAllergiaAnamnesi))
			IF 	ISNULL(@sGUID,'') <> ''				
					SET @uIDAlertAllergiaAnamnesi=CONVERT(UNIQUEIDENTIFIER,	@sGUID)											
							END
			
		IF @xParametri.exist('/Parametri/IDPaziente')=1
		BEGIN	
		
			SET @sGUID=(SELECT TOP 1 ValoreParametro.IDPaziente.value('.','VARCHAR(50)')
				  FROM @xParametri.nodes('/Parametri/IDPaziente') as ValoreParametro(IDPaziente))

			IF 	ISNULL(@sGUID,'') <> '' 
				BEGIN
					SET @uIDPaziente=CONVERT(UNIQUEIDENTIFIER,	@sGUID)		
					SET	@sSET= @sSET + ',IDPaziente=''' + convert(VARCHAR(50),@uIDPaziente) + ''''	+ CHAR(13) + CHAR(10)	
				END
			ELSE
				SET	@sSET= @sSET + ',IDPaziente=NULL'	+ CHAR(13) + CHAR(10)		
			
		END	
		
		IF @xParametri.exist('/Parametri/DataEvento')=1
		BEGIN			
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
			IF @dDataEvento IS NOT NULL
				SET	@sSET= @sSET + ',DataEvento=CONVERT(DateTime,''' + CONVERT(VARCHAR(20),@dDataEvento,120) + ''',120)' + CHAR(13) + CHAR(10)		 
			ELSE	
				SET	@sSET= @sSET + ',DataEvento=NULL' + CHAR(13) + CHAR(10)		 
		END	
		
	
		IF @xParametri.exist('/Parametri/DataEventoUTC')=1
	BEGIN	
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
		
		IF @dDataEvento IS NOT NULL		
			SET	@sSET= @sSET + ',DataEventoUTC=CONVERT(DateTime,''' + CONVERT(VARCHAR(20),@dDataEventoUTC,120) + ''',120)' + CHAR(13) + CHAR(10)		
		ELSE
			SET	@sSET= @sSET + ',DataEventoUTC=NULL ' + CHAR(13) + CHAR(10)		
	END	
		
	 
		IF @xParametri.exist('/Parametri/CodTipoAlertAllergiaAnamnesi')=1
	BEGIN	
		SET @sCodTipoAlertAllergiaAnamnesi=(SELECT TOP 1 ValoreParametro.CodTipoAlertAllergiaAnamnesi.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodTipoAlertAllergiaAnamnesi') as ValoreParametro(CodTipoAlertAllergiaAnamnesi))			
	
		IF @sCodTipoAlertAllergiaAnamnesi <> ''
			SET	@sSET= @sSET + ',CodTipoAlertAllergiaAnamnesi=''' + @sCodTipoAlertAllergiaAnamnesi + '''' + CHAR(13) + CHAR(10)		
		ELSE
			SET @sSET= @sSET + ',CodTipoAlertAllergiaAnamnesi=NULL ' + CHAR(13) + CHAR(10)			
	END	
			
	
		IF @xParametri.exist('/Parametri/CodStatoAlertAllergiaAnamnesi')=1
	BEGIN
		SET @sCodStatoAlertAllergiaAnamnesi=(SELECT TOP 1 ValoreParametro.CodStatoAlertAllergiaAnamnesi.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodStatoAlertAllergiaAnamnesi') as ValoreParametro(CodStatoAlertAllergiaAnamnesi))	
		
		IF @sCodStatoAlertAllergiaAnamnesi <> ''		
			SET	@sSET= @sSET + ',CodStatoAlertAllergiaAnamnesi=''' + @sCodStatoAlertAllergiaAnamnesi + '''' + CHAR(13) + CHAR(10)		
		ELSE
			SET	@sSET= @sSET + ',CodStatoAlertAllergiaAnamnesi=NULL ' + CHAR(13) + CHAR(10)		
	END		

	
		IF @xParametri.exist('/Parametri/CodSistema')=1
	BEGIN
		SET @sCodSistema=(SELECT TOP 1 ValoreParametro.CodSistema.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodSistema') as ValoreParametro(CodSistema))	
					  
		IF @sCodSistema <> ''
				SET	@sSET= @sSET + ',CodSistema=''' + @sCodSistema +''''	+ CHAR(13) + CHAR(10)				
			ELSE
				SET	@sSET= @sSET + ',CodSistema=NULL'	+ CHAR(13) + CHAR(10)		
	END	
	
		IF @xParametri.exist('/Parametri/IDSistema')=1
	BEGIN
		SET @sIDSistema=(SELECT TOP 1 ValoreParametro.IDSistema.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDSistema') as ValoreParametro(IDSistema))	
					  
		IF @sIDSistema <> ''
				SET	@sSET= @sSET + ',IDSistema=''' + @sIDSistema +''''	+ CHAR(13) + CHAR(10)				
			ELSE
				SET	@sSET= @sSET + ',IDSistema=NULL'	+ CHAR(13) + CHAR(10)		
	END	
	
		IF @xParametri.exist('/Parametri/IDGruppo')=1
	BEGIN
		SET @sIDGruppo=(SELECT TOP 1 ValoreParametro.IDGruppo	.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDGruppo') as ValoreParametro(IDGruppo	))	
					  
		IF @sIDGruppo <> ''
				SET	@sSET= @sSET + ',IDGruppo=''' + @sIDGruppo +''''	+ CHAR(13) + CHAR(10)				
			ELSE
				SET	@sSET= @sSET + ',IDGruppo=NULL'	+ CHAR(13) + CHAR(10)		
	END	

		SET @sCodUtenteUltimaModifica=(SELECT TOP 1 ValoreParametro.CodUtenteUltimaModifica.value('.','VARCHAR(100)')
					  FROM @xParametri.nodes('/Parametri/TimeStamp/CodLogin') as ValoreParametro(CodUtenteUltimaModifica))	
	SET @sCodUtenteUltimaModifica=ISNULL(@sCodUtenteUltimaModifica,'')
	
	IF @sCodUtenteUltimaModifica <> ''
		SET	@sSET= @sSET + ',CodUtenteUltimaModifica=''' + @sCodUtenteUltimaModifica + '''' + CHAR(13) + CHAR(10)		
	ELSE
		SET	@sSET= @sSET + ',CodUtenteUltimaModifica=NULL' + CHAR(13) + CHAR(10)		
					  
		SET	@sSET= @sSET + ',DataUltimaModifica=getdate() ' + CHAR(13) + CHAR(10)	
		
		SET	@sSET= @sSET + ',DataUltimaModificaUTC=getdate() ' + CHAR(13) + CHAR(10)	
					
    SET @xTimeStamp=(SELECT TOP 1 ValoreParametro.TS.query('.')
					  FROM @xParametri.nodes('/Parametri/TimeStamp') as ValoreParametro(TS))
	
		SET @xSchedaMovimento=(SELECT TOP 1 ValoreParametro.TS.query('.')
					  FROM @xParametri.nodes('/Parametri/SchedaMovimento') as ValoreParametro(TS))					
					  				
				
				
		IF LEFT(@sSET,1)=',' 
		SET @sSET=RIGHT(@sSET,LEN(@sSET)-1)
	
	IF @sSET <> ''		
		BEGIN
						
						SET @sWHERE =' WHERE ID=''' + convert(varchar(50),@uIDAlertAllergiaAnamnesi) +''''
				
						SET @sSQL=	ISNULL(@sSQL,'') + CHAR(13) + CHAR(10) + 
						ISNULL(@sSET,'') + CHAR(13) + CHAR(10) + 
						ISNULL(@sWHERE,'')			

												
						SET @xTimeStamp.modify('delete (/TimeStamp/IDEntita)[1]') 
						
			SET @xTimeStamp.modify('insert <IDEntita>{sql:variable("@uIDAlertAllergiaAnamnesi")}</IDEntita> into (/TimeStamp)[1]')
	
						SET @xTimeStampBase=@xTimeStamp
	
			SET @xTimeStamp=CONVERT(XML,
										'<Parametri>' + CONVERT(varchar(max),@xTimeStamp) +
										'</Parametri>')
			
												
			SET @xParLog=CONVERT(XML,'<Parametri><LogPrima/><LogDopo/></Parametri>')										
			
			SET @xLogPrima=Convert(XML,'<DataSet></DataSet>')														
			
			SET @xTemp=
				(SELECT * FROM 
					(SELECT * FROM T_MovAlertAllergieAnamnesi
					 WHERE ID=@uIDAlertAllergiaAnamnesi										) AS [Table]
				FOR XML AUTO, ELEMENTS)

			SET @xLogPrima.modify('insert sql:variable("@xTemp") as first into (/DataSet)[1]')
																		
			SET @xParLog.modify('insert sql:variable("@xLogPrima") as first into (/Parametri/LogPrima)[1]')
				
			BEGIN TRANSACTION
												EXEC (@sSQL)
								
			IF @@ERROR=0 AND @@ROWCOUNT > 0
				BEGIN
										EXEC MSP_InsMovTimeStamp @xTimeStamp		
				END	
			IF @@ERROR = 0
				BEGIN
					COMMIT TRANSACTION
					
																				
					SET @xLogDopo=Convert(XML,'<DataSet></DataSet>')				
					
					SET @xTemp=
						(SELECT * FROM 
							(SELECT * FROM T_MovAlertAllergieAnamnesi
							 WHERE ID=@uIDAlertAllergiaAnamnesi												) AS [Table]
						FOR XML AUTO, ELEMENTS)

					SET @xLogDopo.modify('insert sql:variable("@xTemp") as first into (/DataSet)[1]')																											
					SET @xParLog.modify('insert sql:variable("@xLogDopo") as first into (/Parametri/LogDopo)[1]')
					
										SET @xParLog.modify('insert sql:variable("@xTimeStampBase") as last into (/Parametri)[1]')
					
										SET @xParLog.modify('insert sql:variable("@xSchedaMovimento") as last into (/Parametri/LogDopo/DataSet)[1]')

				
										EXEC MSP_InsMovLog @xParLog
									END	
			ELSE
				BEGIN
					ROLLBACK TRANSACTION	
									END	 
		END		
	RETURN 0
END