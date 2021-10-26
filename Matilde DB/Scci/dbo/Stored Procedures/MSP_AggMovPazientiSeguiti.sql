CREATE PROCEDURE [dbo].[MSP_AggMovPazientiSeguiti](@xParametri AS XML)
AS
BEGIN
	
											
		DECLARE @uIDPazienteSeguito AS UNIQUEIDENTIFIER		
	DECLARE @sCodStatoPazienteSeguito AS VARCHAR(20)			
		
		DECLARE @sGUID AS VARCHAR(50)
	DECLARE @uGUID AS UNIQUEIDENTIFIER
	DECLARE @sDataTmp AS VARCHAR(20)	
	DECLARE @nQTA AS INTEGER
	
	DECLARE @sSQL AS VARCHAR(MAX)
	DECLARE @sSET AS VARCHAR(MAX)
	DECLARE @sWHERE AS VARCHAR(MAX)
	
		
	    DECLARE @xTimeStampBase AS XML	
	DECLARE @xTimeStamp AS XML	
		
		DECLARE @xLogPrima AS XML
	DECLARE @xLogDopo AS XML
	DECLARE @xTemp AS XML
	DECLARE @xParLog AS XML
	
						
		SET @sSQL='UPDATE T_MovPazientiSeguiti ' + CHAR(13) + CHAR(10) +
			  'SET '
			  					
	SET @sSET =''
					
		IF @xParametri.exist('/Parametri/IDPazienteSeguito')=1
		BEGIN
			SET @sGUID=(SELECT TOP 1 ValoreParametro.IDPazienteSeguito.value('.','VARCHAR(50)')
							  FROM @xParametri.nodes('/Parametri/IDPazienteSeguito') as ValoreParametro(IDPazienteSeguito))
			IF 	ISNULL(@sGUID,'') <> '' 
					SET @uIDPazienteSeguito=CONVERT(UNIQUEIDENTIFIER,	@sGUID)	
					END	
						
		IF @xParametri.exist('/Parametri/CodStatoPazienteSeguito')=1
	BEGIN
		SET @sCodStatoPazienteSeguito=(SELECT TOP 1 ValoreParametro.CodStatoPazienteSeguito.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodStatoPazienteSeguito') as ValoreParametro(CodStatoPazienteSeguito))	
					  
		IF @sCodStatoPazienteSeguito <> ''
				SET	@sSET= @sSET + ',CodStatoPazienteSeguito=''' + @sCodStatoPazienteSeguito +''''	+ CHAR(13) + CHAR(10)
	END		
				
		SET	@sSET= @sSET + ',DataUltimaModifica=getdate() ' + CHAR(13) + CHAR(10)	
		
		SET	@sSET= @sSET + ',DataUltimaModificaUTC=getUTCdate() ' + CHAR(13) + CHAR(10)	
					
	    SET @xTimeStamp=(SELECT TOP 1 ValoreParametro.TS.query('.')
					  FROM @xParametri.nodes('/Parametri/TimeStamp') as ValoreParametro(TS))	
	
				
		IF LEFT(@sSET,1)=',' 
		SET @sSET=RIGHT(@sSET,LEN(@sSET)-1)
	END
		
		
	IF @sSET <> ''	
		BEGIN
						
						IF @uIDPazienteSeguito IS NULL
				SET @sWHERE='WHERE 1=0'
			ELSE	
				SET @sWHERE =' WHERE ID=''' + convert(varchar(50),@uIDPazienteSeguito) +''''
			
			
						SET @sSQL=	ISNULL(@sSQL,'') + CHAR(13) + CHAR(10) + 
						ISNULL(@sSET,'') + CHAR(13) + CHAR(10) + 
						ISNULL(@sWHERE,'')	
			
																
						SET @xTimeStamp.modify('delete (/TimeStamp/IDEntita)[1]') 						
			SET @xTimeStamp.modify('insert <IDEntita>{sql:variable("@uIDPazienteSeguito")}</IDEntita> into (/TimeStamp)[1]')
	
						SET @xTimeStamp.modify('delete (/TimeStamp/CodEntita)[1]') 						
			SET @xTimeStamp.modify('insert <CodEntita>PZS</CodEntita> into (/TimeStamp)[1]')
										
						SET @xTimeStampBase=@xTimeStamp
	
			SET @xTimeStamp=CONVERT(XML,
										'<Parametri>' + CONVERT(varchar(max),@xTimeStamp) +
										'</Parametri>')
				
												
			SET @xParLog=CONVERT(XML,'<Parametri><LogPrima/><LogDopo/></Parametri>')										
			
			SET @xLogPrima=Convert(XML,'<DataSet></DataSet>')														
			
			SET @xTemp=
				(SELECT * FROM 
					(SELECT  *
					 FROM T_MovPazientiSeguiti
					 WHERE ID=@uIDPazienteSeguito								) AS [Table]
				FOR XML AUTO, ELEMENTS)

			SET @xLogPrima.modify('insert sql:variable("@xTemp") as first into (/DataSet)[1]')																		
			SET @xParLog.modify('insert sql:variable("@xLogPrima") as first into (/Parametri/LogPrima)[1]')

			BEGIN TRANSACTION
				PRINT @sSQL
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
							(SELECT  *
								 FROM T_MovPazientiSeguiti
								 WHERE ID=@uIDPazienteSeguito										) AS [Table]
						FOR XML AUTO, ELEMENTS)

					SET @xLogDopo.modify('insert sql:variable("@xTemp") as first into (/DataSet)[1]')																											
					SET @xParLog.modify('insert sql:variable("@xLogDopo") as first into (/Parametri/LogDopo)[1]')

										SET @xParLog.modify('insert sql:variable("@xTimeStampBase") as last into (/Parametri)[1]')
										
										EXEC MSP_InsMovLog @xParLog

					END
			ELSE
				BEGIN
					PRINT 'ROLLBACK'
					ROLLBACK TRANSACTION							
				END			

															
	RETURN 0
END