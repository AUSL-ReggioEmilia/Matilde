CREATE PROCEDURE [dbo].[MSP_AggMovConsegnePaziente](@xParametri XML)
AS
BEGIN
		
	
							
		DECLARE @uIDConsegnaPaziente AS UNIQUEIDENTIFIER	
	DECLARE @sCodStatoConsegnaPaziente AS VARCHAR(20)	

	DECLARE @sCodUtente AS VARCHAR(100)
	
		DECLARE @sGUID AS VARCHAR(50)
	DECLARE @uGUID AS UNIQUEIDENTIFIER
	
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
	
		SET @sSQL='UPDATE T_MovConsegnePaziente ' + CHAR(13) + CHAR(10) +
			  'SET '  				  						
			
	SET @sSET =''
	
	IF @xParametri.exist('/Parametri/IDConsegnaPaziente')=1
	BEGIN
				SET @sGUID=(SELECT TOP 1 ValoreParametro.IDConsegnaPaziente.value('.','VARCHAR(50)')
						  FROM @xParametri.nodes('/Parametri/IDConsegnaPaziente') as ValoreParametro(IDConsegnaPaziente))
						  
		IF 	ISNULL(@sGUID,'') <> '' 
				SET @uIDConsegnaPaziente=CONVERT(UNIQUEIDENTIFIER,	@sGUID)			
	END
	
		IF @xParametri.exist('/Parametri/CodStatoConsegnaPaziente')=1
		BEGIN
			SET @sCodStatoConsegnaPaziente=(SELECT TOP 1 ValoreParametro.CodStatoConsegnaPaziente.value('.','VARCHAR(20)')
						  FROM @xParametri.nodes('/Parametri/CodStatoConsegnaPaziente') as ValoreParametro(CodStatoConsegnaPaziente))	
						  
			IF @sCodStatoConsegnaPaziente <> ''
					SET	@sSET= @sSET + ',CodStatoConsegnaPaziente=''' + @sCodStatoConsegnaPaziente+''''	+ CHAR(13) + CHAR(10)				
				ELSE
					SET	@sSET= @sSET + ',CodStatoConsegnaPaziente=NULL'	+ CHAR(13) + CHAR(10)		
		END	
	
		SET @sCodUtente=(SELECT TOP 1 ValoreParametro.CodLogin.value('.','VARCHAR(100)')
					FROM @xParametri.nodes('/Parametri/TimeStamp/CodLogin') as ValoreParametro(CodLogin))	
	SET @sCodUtente=ISNULL(@sCodUtente,'')
	IF @sCodUtente <> ''
		BEGIN
			SET	@sSET= @sSET + ',CodUtenteUltimaModifica=''' + @sCodUtente + '''' + CHAR(13) + CHAR(10)	
			SET	@sSET= @sSET + ',DataUltimaModifica=GETDATE()' + CHAR(13) + CHAR(10)
			SET	@sSET= @sSET + ',DataUltimaModificaUTC=GETUTCDATE()' + CHAR(13) + CHAR(10)
		END
		
		IF @sCodStatoConsegnaPaziente ='AN'
		BEGIN
			SET	@sSET= @sSET + ',CodUtenteAnnullamento='''  + @sCodUtente + '''' + CHAR(13) + CHAR(10)	
			SET	@sSET= @sSET + ',DataAnnullamento=GETDATE()' + CHAR(13) + CHAR(10)
			SET	@sSET= @sSET + ',DataAnnullamentoUTC=GETUTCDATE()' + CHAR(13) + CHAR(10)
		END

		IF @sCodStatoConsegnaPaziente ='CA'
		BEGIN
			SET	@sSET= @sSET + ',CodUtenteCancellazione='''  + @sCodUtente + '''' + CHAR(13) + CHAR(10)	
			SET	@sSET= @sSET + ',DataCancellazione=GETDATE()' + CHAR(13) + CHAR(10)
			SET	@sSET= @sSET + ',DataCancellazioneUTC=GETUTCDATE()' + CHAR(13) + CHAR(10)
		END
				
	    SET @xTimeStamp=(SELECT TOP 1 ValoreParametro.TS.query('.')
					  FROM @xParametri.nodes('/Parametri/TimeStamp') as ValoreParametro(TS))
		
			
		IF LEFT(@sSET,1)=',' 
		SET @sSET=RIGHT(@sSET,LEN(@sSET)-1)

	IF LTRIM(RTRIM(@sSET)) <> ''
		BEGIN

								SET @sWHERE =' WHERE ID=''' + convert(varchar(50),@uIDConsegnaPaziente) +''''
					
								SET @sSQL=	ISNULL(@sSQL,'') + CHAR(13) + CHAR(10) + 
							ISNULL(@sSET,'') + CHAR(13) + CHAR(10) + 
							ISNULL(@sWHERE,'')			

																
								SET @xTimeStamp.modify('delete (/TimeStamp/IDEntita)[1]') 
						
				SET @xTimeStamp.modify('insert <IDEntita>{sql:variable("@uIDConsegnaPaziente")}</IDEntita> into (/TimeStamp)[1]')

				SET @xTimeStampBase=@xTimeStamp
	
				SET @xTimeStamp=CONVERT(XML,
									'<Parametri>' + CONVERT(varchar(max),@xTimeStamp) +
									'</Parametri>')
						
																
				SET @xParLog=CONVERT(XML,'<Parametri><LogPrima/><LogDopo/></Parametri>')										
				
				SET @xLogPrima=Convert(XML,'<DataSet></DataSet>')														
				
				SET @xTemp=
					(SELECT * FROM 
						(SELECT * FROM T_MovConsegnePaziente
						 WHERE ID=@uIDConsegnaPaziente											) AS [Table]
					FOR XML AUTO, ELEMENTS)

				SET @xLogPrima.modify('insert sql:variable("@xTemp") as first into (/DataSet)[1]')
																			
				SET @xParLog.modify('insert sql:variable("@xLogPrima") as first into (/Parametri/LogPrima)[1]')
				SET @xParLog.modify('insert sql:variable("@xTimeStampBase") as last into (/Parametri)[1]')
																						
				BEGIN TRANSACTION
										PRINT @sSQL
					EXEC (@sSQL)
						
				IF @@ERROR=0 AND @@ROWCOUNT >0
					BEGIN			
						EXEC MSP_InsMovTimeStamp @xTimeStamp		
					END	
				IF @@ERROR = 0
					BEGIN
					
												COMMIT TRANSACTION
						
																												
							SET @xLogDopo=Convert(XML,'<DataSet></DataSet>')				
							
							SET @xTemp=
								(SELECT * FROM 
									(SELECT * FROM T_MovConsegnePaziente
									 WHERE ID=@uIDConsegnaPaziente														) AS [Table]
								FOR XML AUTO, ELEMENTS)

							SET @xLogDopo.modify('insert sql:variable("@xTemp") as first into (/DataSet)[1]')																											
							SET @xParLog.modify('insert sql:variable("@xLogDopo") as first into (/Parametri/LogDopo)[1]')
							
														SET @xParLog.modify('insert sql:variable("@xTimeStampBase") as last into (/Parametri)[1]')
													
							EXEC MSP_InsMovLog @xParLog
														
														
												END	
				ELSE
					BEGIN
						ROLLBACK TRANSACTION	
											END	 
		END			
	RETURN 0
END