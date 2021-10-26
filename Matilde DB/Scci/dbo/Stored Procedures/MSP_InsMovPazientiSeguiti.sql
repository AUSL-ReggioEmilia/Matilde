CREATE PROCEDURE [dbo].[MSP_InsMovPazientiSeguiti](@xParametri AS XML)
AS
BEGIN
	
											
		DECLARE @sCodUtente AS VARCHAR(100)	
	DECLARE @sCodRuolo AS VARCHAR(20)	
	
	DECLARE @uIDPaziente AS UNIQUEIDENTIFIER		
	
	DECLARE @sCodStatoPazienteSeguito AS VARCHAR(20)	
			
		
		DECLARE @sGUID AS VARCHAR(50)
	DECLARE @uGUID AS UNIQUEIDENTIFIER
	DECLARE @uIDPresente AS UNIQUEIDENTIFIER
	DECLARE @sDataTmp AS VARCHAR(20)	
	DECLARE @nQTA AS INTEGER
			
	    DECLARE @xTimeStampBase AS XML	
	DECLARE @xTimeStamp AS XML	
		
		DECLARE @xLogPrima AS XML
	DECLARE @xLogDopo AS XML
	DECLARE @xTemp AS XML
	DECLARE @xParLog AS XML

		SET @sCodUtente=(SELECT TOP 1 ValoreParametro.CodUtente.value('.','VARCHAR(100)')
					  FROM @xParametri.nodes('/Parametri/CodUtente') as ValoreParametro(CodUtente))	
	SET @sCodUtente=ISNULL(@sCodUtente,'')
	
		SET @sCodRuolo=(SELECT TOP 1 ValoreParametro.CodRuolo.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodRuolo') as ValoreParametro(CodRuolo))	
	SET @sCodRuolo=ISNULL(@sCodRuolo,'')
	
	
		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDPaziente.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDPaziente') as ValoreParametro(IDPaziente))
	IF 	ISNULL(@sGUID,'') <> '' 
			SET @uIDPaziente=CONVERT(UNIQUEIDENTIFIER,	@sGUID)	

		SET @sCodStatoPazienteSeguito=(SELECT TOP 1 ValoreParametro.CodStatoPazienteSeguito.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodStatoPazienteSeguito') as ValoreParametro(CodStatoPazienteSeguito))	
				
	SET @sCodStatoPazienteSeguito=ISNULL(@sCodStatoPazienteSeguito,'IC')
	
		
	    SET @xTimeStamp=(SELECT TOP 1 ValoreParametro.TS.query('.')
					  FROM @xParametri.nodes('/Parametri/TimeStamp') as ValoreParametro(TS))	
	
								
					
	SET @uGUID=NEWID()
	
	IF @uIDPaziente IS NOT NULL AND ISNULL(@sCodUtente,'') <> '' AND ISNULL(@sCodRuolo,'') <> ''
		BEGIN
			BEGIN TRANSACTION
										
						SET @uIDPresente= (SELECT TOP 1 
									IDPaziente 
								FROM 
									T_MovPazientiSeguiti 
								WHERE CodUtente=@sCodUtente AND 
									  CodRuolo=@sCodRuolo AND 
									  IDPaziente=@uIDPaziente AND
									  CodStatoPazienteSeguito='IC'
								)
			
			IF  @uIDPresente IS NULL
				BEGIN
				
										INSERT INTO T_MovPazientiSeguiti
						   (ID
						   ,IDPaziente			   
						   ,CodUtente
						   ,CodRuolo
						   ,CodStatoPazienteSeguito			   
						   ,DataInserimento
						   ,DataInserimentoUTC
						   ,DataUltimaModifica
						   ,DataUltimaModificaUTC
							)
					 VALUES
							(@uGUID														   ,@uIDPaziente												   ,@sCodUtente													   ,@sCodRuolo													   ,@sCodStatoPazienteSeguito									   ,GetDate()													   ,GetUTCDate()												   ,NULL														   ,NULL															)
							
																									SET @xTimeStamp.modify('delete (/TimeStamp/IDEntita)[1]') 					
					SET @xTimeStamp.modify('insert <IDEntita>{sql:variable("@uGUID")}</IDEntita> into (/TimeStamp)[1]')
					
										SET @xTimeStampBase=@xTimeStamp


					SET @xTimeStamp=CONVERT(XML,
										'<Parametri>' + CONVERT(varchar(max),@xTimeStamp) +
										'</Parametri>')

										EXEC MSP_InsMovTimeStamp @xTimeStamp									
				END					
			ELSE
				BEGIN
					SET @uGUID=@uIDPresente
				END
							
			IF @@ERROR=0						
					BEGIN							
						SELECT @uGUID AS IDPaziente
						COMMIT TRANSACTION		
						
																																				
						SET @xLogDopo=Convert(XML,'<DataSet></DataSet>')
						SET @xLogPrima=''
						
						SET @xTemp=
							(SELECT * FROM 
								(SELECT * FROM T_MovPazientiSeguiti
								 WHERE ID=@uGUID
								) AS [Table]
							FOR XML AUTO, ELEMENTS)

						SET @xLogDopo.modify('insert sql:variable("@xTemp") as first into (/DataSet)[1]')				
						
						SET @xParLog=CONVERT(XML,'<Parametri><LogDopo/></Parametri>')			
						SET @xParLog.modify('insert sql:variable("@xLogDopo") as first into (/Parametri/LogDopo)[1]')
						SET @xParLog.modify('insert sql:variable("@xTimeStampBase") as last into (/Parametri)[1]')
									
						EXEC MSP_InsMovLog @xParLog
											END	
				ELSE 					BEGIN
						ROLLBACK TRANSACTION	
						PRINT 'ERRORE'
						SELECT NULL AS IDPaziente
					END				
		END					
	ELSE
		SELECT NULL AS IDPaziente
															
	RETURN 0
END