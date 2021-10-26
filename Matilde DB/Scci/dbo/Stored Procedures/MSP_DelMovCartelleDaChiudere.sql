CREATE PROCEDURE [dbo].[MSP_DelMovCartelleDaChiudere](@xParametri AS XML)
AS
BEGIN
	
											
		DECLARE @sCodUtente AS VARCHAR(100)	
	DECLARE @sCodRuolo AS VARCHAR(20)	
	
	DECLARE @uIDCartella AS UNIQUEIDENTIFIER		
							
		DECLARE @sGUID AS VARCHAR(50)
	DECLARE @uGUID AS UNIQUEIDENTIFIER	
	DECLARE @sDataTmp AS VARCHAR(20)	
	DECLARE @nQTA AS INTEGER
	DECLARE @uIDPresente AS UNIQUEIDENTIFIER
			
	    DECLARE @xTimeStampBase AS XML	
	DECLARE @xTimeStamp AS XML	
		
		DECLARE @xLogPrima AS XML
	DECLARE @xLogDopo AS XML
	DECLARE @xTemp AS XML
	DECLARE @xParLog AS XML

	
		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDCartella.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDCartella') as ValoreParametro(IDCartella))
	IF 	ISNULL(@sGUID,'') <> '' 
			SET @uIDCartella=CONVERT(UNIQUEIDENTIFIER,	@sGUID)	

		
	    SET @xTimeStamp=(SELECT TOP 1 ValoreParametro.TS.query('.')
					  FROM @xParametri.nodes('/Parametri/TimeStamp') as ValoreParametro(TS))	
	
								
						
	
	IF @uIDCartella IS NOT NULL 
		BEGIN
						SET @uIDPresente= (SELECT TOP 1 
									IDCartella 
								FROM 
									T_MovCartelleDaChiudere 
								WHERE IDCartella=@uIDCartella
								)
								
			IF 	@uIDPresente IS NOT NULL				
			BEGIN
					DELETE FROM T_MovCartelleDaChiudere
					WHERE IDCartella=@uIDCartella
							
																									SET @xTimeStamp.modify('delete (/TimeStamp/IDEntita)[1]') 					
					SET @xTimeStamp.modify('insert <IDEntita>{sql:variable("@uGUID")}</IDEntita> into (/TimeStamp)[1]')
					SET @xTimeStamp.modify('insert <Note>Cancellazione Cartella da chiudere</Note> into (/TimeStamp)[1]')
					
										SET @xTimeStampBase=@xTimeStamp


					SET @xTimeStamp=CONVERT(XML,
										'<Parametri>' + CONVERT(varchar(max),@xTimeStamp) +
										'</Parametri>')

															EXEC MSP_InsMovTimeStamp @xTimeStamp									
				END					
			
							
																				
																														
														
																								
										
																					
															END	
			
															
	RETURN 0
END