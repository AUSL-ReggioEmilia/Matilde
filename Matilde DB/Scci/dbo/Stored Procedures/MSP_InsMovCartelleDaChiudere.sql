CREATE PROCEDURE [dbo].[MSP_InsMovCartelleDaChiudere](@xParametri AS XML)
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

		SET @sCodUtente=(SELECT TOP 1 ValoreParametro.CodUtente.value('.','VARCHAR(100)')
					  FROM @xParametri.nodes('/Parametri/TimeStamp/CodLogin') as ValoreParametro(CodUtente))	
	SET @sCodUtente=ISNULL(@sCodUtente,'')
	
		SET @sCodRuolo=(SELECT TOP 1 ValoreParametro.CodRuolo.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/TimeStamp/CodRuolo') as ValoreParametro(CodRuolo))	
	SET @sCodRuolo=ISNULL(@sCodRuolo,'')
	
	
		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDCartella.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDCartella') as ValoreParametro(IDCartella))
	IF 	ISNULL(@sGUID,'') <> '' 
			SET @uIDCartella=CONVERT(UNIQUEIDENTIFIER,	@sGUID)	

		
	    SET @xTimeStamp=(SELECT TOP 1 ValoreParametro.TS.query('.')
					  FROM @xParametri.nodes('/Parametri/TimeStamp') as ValoreParametro(TS))	
	
								
						
	
	IF @uIDCartella IS NOT NULL AND ISNULL(@sCodUtente,'') <> '' AND ISNULL(@sCodRuolo,'') <> ''
		BEGIN
						SET @uIDPresente= (SELECT TOP 1 
									IDCartella 
								FROM 
									T_MovCartelleDaChiudere 
								WHERE IDCartella=@uIDCartella
								)
				
			IF 	@uIDPresente IS NULL				
			BEGIN
										INSERT INTO T_MovCartelleDaChiudere
						   (IDCartella			   
						   ,CodUtente
						   ,CodRuolo						  	   
						   ,DataInserimento
						   ,DataInserimentoUTC						   
							)
					 VALUES
							(@uIDCartella												   ,@sCodUtente													   ,@sCodRuolo													   ,GetDate()													   ,GetUTCDate()													)
							
																									SET @xTimeStamp.modify('delete (/TimeStamp/IDEntita)[1]') 					
					SET @xTimeStamp.modify('insert <IDEntita>{sql:variable("@uGUID")}</IDEntita> into (/TimeStamp)[1]')
					SET @xTimeStamp.modify('insert <Note>Cartella da chiudere</Note> into (/TimeStamp)[1]')
					
										SET @xTimeStampBase=@xTimeStamp


					SET @xTimeStamp=CONVERT(XML,
										'<Parametri>' + CONVERT(varchar(max),@xTimeStamp) +
										'</Parametri>')

										SELECT @xTimeStamp
					EXEC MSP_InsMovTimeStamp @xTimeStamp									
				END					
				
																				
																														
														
																								
										
																					
															END	
			
															
	RETURN 0
END