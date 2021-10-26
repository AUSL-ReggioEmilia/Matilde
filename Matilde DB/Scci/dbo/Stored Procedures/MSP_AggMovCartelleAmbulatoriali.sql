CREATE PROCEDURE [dbo].[MSP_AggMovCartelleAmbulatoriali](@xParametri XML)
AS
BEGIN

									 	
	
				
		DECLARE @sNumeroCartella AS VARCHAR(20)			
	DECLARE @sCodStatoCartella AS VARCHAR(20)
	DECLARE @sCodStatoCartellaInfo AS VARCHAR(20)
	DECLARE @txtPDFCartella AS VARCHAR(MAX)
	DECLARE @uIDCartella AS UNIQUEIDENTIFIER	
	DECLARE @sCodAzione AS VARCHAR(20)

		
		
	DECLARE @binPDFCartella AS VARBINARY(MAX)			
	DECLARE @dDataApertura AS DATETIME	
	DECLARE @dDataAperturaUTC AS DATETIME
	DECLARE @dDataChiusura AS DATETIME
	DECLARE @dDataChiusuraUTC AS DATETIME
	DECLARE @sCodUtente AS VARCHAR(100)	
			
	DECLARE @sCodUtenteApertura AS VARCHAR(100)					
	DECLARE @sCodUtenteChiusura AS VARCHAR(100)			
				
	DECLARE @sCodLogin AS VARCHAR(100)	
				
		DECLARE @nTemp AS INTEGER
	DECLARE @sGUID AS VARCHAR(50)
	DECLARE @uGUID AS UNIQUEIDENTIFIER
	DECLARE @sDataTmp AS VARCHAR(20)	

	DECLARE @sSQL AS VARCHAR(MAX)
	DECLARE @sSET AS VARCHAR(MAX)
	DECLARE @sWHERE AS VARCHAR(MAX)
	
	    DECLARE @xTimeStampBase AS XML	
	DECLARE @xTimeStamp AS XML	
	
		DECLARE @xLogPrima AS XML
	DECLARE @xLogDopo AS XML
	DECLARE @xTemp AS XML
	DECLARE @xParLog AS XML
	
		
	

				
		
		SET @sCodLogin=(SELECT TOP 1 ValoreParametro.CodLogin.value('.','VARCHAR(100)')
					  FROM @xParametri.nodes('/Parametri/TimeStamp/CodLogin') as ValoreParametro(CodLogin))
	SET @sCodLogin=ISNULL(@sCodLogin,'')
	
		SET @sCodAzione=(SELECT TOP 1 ValoreParametro.CodAzione.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/TimeStamp/CodAzione') as ValoreParametro(CodAzione))
	SET @sCodAzione=ISNULL(@sCodAzione,'')
	
		IF @xParametri.exist('/Parametri/NumeroCartella')=1
		BEGIN
			SET @sNumeroCartella=(SELECT TOP 1 ValoreParametro.NumeroCartella.value('.','VARCHAR(50)')
							  FROM @xParametri.nodes('/Parametri/NumeroCartella') as ValoreParametro(NumeroCartella))
			
					END		
		
		IF @xParametri.exist('/Parametri/IDCartella')=1
		BEGIN
			SET @sGUID=(SELECT TOP 1 ValoreParametro.IDCartella.value('.','VARCHAR(50)')
							  FROM @xParametri.nodes('/Parametri/IDCartella') as ValoreParametro(IDCartella))
			IF 	ISNULL(@sGUID,'') <> '' 
					SET @uIDCartella=CONVERT(UNIQUEIDENTIFIER,	@sGUID)	
					END
	ELSE
		BEGIN
						SET @uIDCartella=(SELECT TOP 1 ID FROM T_MovCartelleAmbulatoriali WHERE NumeroCartella=@sNumeroCartella)
		END
		
	
		SET @sSQL='UPDATE T_MovCartelleAmbulatoriali ' + CHAR(13) + CHAR(10) +
			  'SET '
			  					
	SET @sSET =''			
	
		IF @xParametri.exist('/Parametri/CodStatoCartella')=1
	BEGIN
		SET @sCodStatoCartella=(SELECT TOP 1 ValoreParametro.CodStatoCartella.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodStatoCartella') as ValoreParametro(CodStatoCartella))						  
		
		IF @sCodStatoCartella='AP'
				BEGIN
						
																				SET @sSET=@sSET + ',CodStatoCartella=''' + @sCodStatoCartella + ''''
					SET @sSET=@sSET + ',CodUtenteApertura=''' + @sCodLogin + ''''
					
					SET @sSET=@sSET + ',DataApertura=GetDate()'
					SET @sSET=@sSET + ',DataAperturaUTC=GetUTCDate()'	
					
										SET @sSET=@sSET + ',CodUtenteChiusura=NULL'
					SET @sSET=@sSET + ',DataChiusura=NULL'
					SET @sSET=@sSET + ',DataChiusuraUTC=NULL'
				END
			ELSE	
			IF @sCodStatoCartella='CH'
				BEGIN
																				SET @sSET=@sSET + ',CodStatoCartella=''' + @sCodStatoCartella + ''''
					SET @sSET=@sSET + ',CodUtenteChiusura=''' + @sCodLogin + ''''
					
					SET @sSET=@sSET + ',DataChiusura=GetDate()'
					SET @sSET=@sSET + ',DataChiusuraUTC=GetUTCDate()'
					
																								END	
			ELSE
				IF @sCodStatoCartella='CA'				
				BEGIN			
																				SET @sSET=@sSET + ',CodStatoCartella=''' + @sCodStatoCartella + ''''
									END
	END	
	
		IF @xParametri.exist('/Parametri/PDFCartella')=1
	BEGIN
		SET @txtPDFCartella=(SELECT TOP 1 ValoreParametro.PDFCartella.value('.','VARCHAR(MAX)')
						  FROM @xParametri.nodes('/Parametri/PDFCartella') as ValoreParametro(PDFCartella))					  

				IF @txtPDFCartella <> ''
					SET	@sSET= @sSET +',PDFCartella= 
												CAST(N'''' AS xml).value(''xs:base64Binary("' + @txtPDFCartella
														+ '")'', ''varbinary(max)'')
													'  + CHAR(13) + CHAR(10)	
				ELSE				
					SET	@sSET= @sSET +',PDFCartella=NULL '	+ CHAR(13) + CHAR(10)	
	END		
											 
	    SET @xTimeStamp=(SELECT TOP 1 ValoreParametro.TS.query('.')
					  FROM @xParametri.nodes('/Parametri/TimeStamp') as ValoreParametro(TS))		
	
					
		IF LEFT(@sSET,1)=',' 
		SET @sSET=RIGHT(@sSET,LEN(@sSET)-1)
	END

	IF @sSET <> ''	
		BEGIN
						
						IF @uIDCartella IS NULL 
				SET @sWHERE =' WHERE 1=0'
			ELSE
				SET @sWHERE =' WHERE ID=''' + convert(varchar(50),@uIDCartella) +''''
			
						SET @sSQL=	ISNULL(@sSQL,'') + CHAR(13) + CHAR(10) + 
						ISNULL(@sSET,'') + CHAR(13) + CHAR(10) + 
						ISNULL(@sWHERE,'')	

																	
																
						SET @xTimeStamp.modify('delete (/TimeStamp/IDEntita)[1]') 						
			SET @xTimeStamp.modify('insert <IDEntita>{sql:variable("@uIDCartella")}</IDEntita> into (/TimeStamp)[1]')
	
						SET @xTimeStamp.modify('delete (/TimeStamp/CodEntita)[1]') 						
			SET @xTimeStamp.modify('insert <CodEntita>CAC</CodEntita> into (/TimeStamp)[1]')
	
			
						IF @sCodStatoCartella='CH' AND @sCodAzione='MOD' AND ISNULL(@txtPDFCartella,'') <> ''
			BEGIN				
				SET @xTimeStamp.modify('delete (/TimeStamp/CodAzione)[1]')
				SET @xTimeStamp.modify('insert <CodAzione>COM</CodAzione> into (/TimeStamp)[1]')
			END
			ELSE			
								IF @sCodStatoCartella='CH' AND @sCodAzione='COM' AND ISNULL(@txtPDFCartella,'') = ''
				BEGIN
					SET @xTimeStamp.modify('delete (/TimeStamp/CodAzione)[1]')
					SET @xTimeStamp.modify('insert <CodAzione>MOD</CodAzione> into (/TimeStamp)[1]')
				END				
	
						SET @xTimeStampBase=@xTimeStamp
	
			SET @xTimeStamp=CONVERT(XML,
										'<Parametri>' + CONVERT(varchar(max),@xTimeStamp) +
										'</Parametri>')
				
												
			SET @xParLog=CONVERT(XML,'<Parametri><LogPrima/><LogDopo/></Parametri>')										
			
			SET @xLogPrima=Convert(XML,'<DataSet></DataSet>')														
			
			SET @xTemp=
				(SELECT * FROM 
					(SELECT  ID
						  ,IDNum
						  ,NumeroCartella
						  ,CodStatoCartella
						  ,CodUtenteApertura
						  ,CodUtenteChiusura
						  ,DataApertura
						  ,DataAperturaUTC
						  ,DataChiusura
						  ,DataChiusuraUTC
						  					 FROM T_MovCartelleAmbulatoriali
					 WHERE ID=@uIDCartella										) AS [Table]
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
							(SELECT  ID
								  ,IDNum
								  ,NumeroCartella
								  ,CodStatoCartella
								  ,CodUtenteApertura
								  ,CodUtenteChiusura
								  ,DataApertura
								  ,DataAperturaUTC
								  ,DataChiusura
								  ,DataChiusuraUTC
								  							 FROM T_MovCartelleAmbulatoriali
							 WHERE ID=@uIDCartella												) AS [Table]
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