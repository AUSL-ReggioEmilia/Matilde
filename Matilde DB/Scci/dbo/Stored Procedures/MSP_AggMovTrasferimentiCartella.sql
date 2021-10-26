CREATE PROCEDURE [dbo].[MSP_AggMovTrasferimentiCartella](@xParametri XML)
AS
BEGIN

		
	
				
	DECLARE @uIDTrasferimento AS UNIQUEIDENTIFIER
	DECLARE @sNumeroCartella AS VARCHAR(50)
	DECLARE @sCodStatoCartella AS VARCHAR(20)
	DECLARE @sCodStatoTrasferimento AS VARCHAR(20)
	DECLARE @sCodRuolo AS VARCHAR(20)		
	DECLARE @sCodLogin AS VARCHAR(20)	
	
		DECLARE @nTemp AS INTEGER		
	DECLARE @sGUID AS VARCHAR(50)
	DECLARE @uGUID AS UNIQUEIDENTIFIER
	DECLARE @sDataTmp AS VARCHAR(20)	
	DECLARE @uIDCartella AS UNIQUEIDENTIFIER
	DECLARE @uIDEpisodio AS UNIQUEIDENTIFIER
	DECLARE @uIDCartellaCollegata AS UNIQUEIDENTIFIER
	DECLARE @sCodUARif AS VARCHAR(20)								DECLARE @sCodStatoCartellaProduzione AS VARCHAR(20)

	DECLARE @nOperazioneDiSistema AS INTEGER

	DECLARE @sOpzione AS VARCHAR(1)
	DECLARE @sCodUA AS VARCHAR(20)	
	
	DECLARE @sSQL AS VARCHAR(MAX)
	DECLARE @sSET AS VARCHAR(MAX)
	DECLARE @sWHERE AS VARCHAR(MAX)
	
	    DECLARE @xTimeStampBase AS XML	
	DECLARE @xTimeStamp AS XML	
	DECLARE @xTimeStampCartella AS XML	
	
		DECLARE @xLogPrima AS XML
	DECLARE @xLogDopo AS XML
	DECLARE @xTemp AS XML
	DECLARE @xCartella AS XML
	DECLARE @xParLog AS XML
	
		
		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDTrasferimento.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDTrasferimento') as ValoreParametro(IDTrasferimento))
	IF 	ISNULL(@sGUID,'') <> '' SET @uIDTrasferimento=CONVERT(UNIQUEIDENTIFIER,	@sGUID)	
					  				  
		SET @sNumeroCartella=(SELECT TOP 1 ValoreParametro.NumeroCartella.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/NumeroCartella') as ValoreParametro(NumeroCartella))
	
		SET @sCodStatoCartella=(SELECT TOP 1 ValoreParametro.CodStatoCartella.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodStatoCartella') as ValoreParametro(CodStatoCartella))
	SET @sCodStatoCartella=ISNULL(@sCodStatoCartella,'')


		SET @sCodStatoTrasferimento=(SELECT TOP 1 ValoreParametro.CodStatoTrasferimento.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodStatoTrasferimento') as ValoreParametro(CodStatoTrasferimento))
	SET @sCodStatoTrasferimento=ISNULL(@sCodStatoTrasferimento,'')
					
					
		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDCartellaCollegata.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDCartellaCollegata') as ValoreParametro(IDCartellaCollegata))
	IF 	ISNULL(@sGUID,'') <> '' SET @uIDCartellaCollegata=CONVERT(UNIQUEIDENTIFIER,	@sGUID)	
								  
	    SET @xTimeStamp=(SELECT TOP 1 ValoreParametro.TS.query('.')
					  FROM @xParametri.nodes('/Parametri/TimeStamp') as ValoreParametro(TS))
		
		SET @sCodRuolo=(SELECT TOP 1 ValoreParametro.CodRuolo.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/TimeStamp/CodRuolo') as ValoreParametro(CodRuolo))
	SET @sCodRuolo=ISNULL(@sCodRuolo,'')

		SET @sCodLogin=(SELECT TOP 1 ValoreParametro.CodLogin.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/TimeStamp/CodLogin') as ValoreParametro(CodLogin))
	SET @sCodLogin=ISNULL(@sCodLogin,'')	
	
		SET @nOperazioneDiSistema=(SELECT TOP 1 ValoreParametro.OperazioneDiSistema.value('.','INTEGER')
					  FROM @xParametri.nodes('/Parametri/OperazioneDiSistema') as ValoreParametro(OperazioneDiSistema))
	SET @nOperazioneDiSistema=ISNULL(@nOperazioneDiSistema,0)	

			
		SET @sOpzione=(SELECT TOP 1 ISNULL(Valore,'0') AS Valore FROM T_Config WHERE ID=39)
		
				
		
		IF @sCodUA IS NULL		
		SET @sCodUA=(SELECT TOP 1 CodUA FROM T_MovTrasferimenti WHERE ID=@uIDTrasferimento)
		

		SET @sCodUARif=(SELECT 
					CASE 
						WHEN 
							ISNULL(CodUANumerazioneCartella,'') ='' THEN @sCodUA
							ELSE CodUANumerazioneCartella
					END 	
						FROM T_UnitaAtomiche
						WHERE Codice=@sCodUA
					)
						
	IF @sCodStatoCartella IN ('CH','AP')
	BEGIN				
			
							
				IF @sNumeroCartella IS NULL AND @uIDTrasferimento IS NOT NULL
			SET @sNumeroCartella=(SELECT TOP 1 NumeroCartella 
									FROM T_MovCartelle CR
										INNER JOIN T_MovTrasferimenti M
										ON M.IDCartella=CR.ID
									WHERE M.ID=@uIDTrasferimento)
									
				SELECT TOP 1  
			@uIDCartella=IDCartella,
			@uIDEpisodio=IDEpisodio,
			@sCodUA=CodUA
		FROM T_MovTrasferimenti 
		WHERE ID=@uIDTrasferimento									

		
		
		IF @sOpzione='1' AND @uIDCartella IS NULL
		BEGIN
			SET @uIDCartella=(SELECT TOP 1 
									IDCartella
							  FROM T_MovTrasferimenti T
									INNER JOIN T_UnitaAtomiche UAT
										ON T.CodUA=UAT.Codice	
							  WHERE T.IDEpisodio=@uIDEpisodio AND
									T.ID <> @uIDTrasferimento AND
									T.CodStatoTrasferimento <> 'CA' AND
									IDCartella IS NOT NULL AND								
									(T.CodUA=@sCodUARif OR
										ISNULL(UAT.CodUANumerazioneCartella,'@')=@sCodUARif
									)
							 )			
									
		END
						
		IF @sCodStatoCartella='AP'
			BEGIN		
			
				IF @uIDCartella IS NULL
				BEGIN
					SET @xCartella='<Parametri></Parametri>'
					
					SET @xCartella.modify('insert <NumeroCartella>{sql:variable("@sNumeroCartella")}</NumeroCartella> as first into (/Parametri)[1]')
					
					SET @xCartella.modify('insert <CodStatoCartella>{sql:variable("@sCodStatoCartella")}</CodStatoCartella> as first into (/Parametri)[1]')
					
					SET @xTimeStampCartella=@xTimeStamp
									
					SET @xTimeStampCartella.modify('delete (/TimeStamp/IDEntita)[1]') 	
										
					SET @xTimeStampCartella.modify('delete (/TimeStamp/CodEntita)[1]') 						
					SET @xTimeStampCartella.modify('insert <CodEntita>CAR</CodEntita> into (/TimeStamp)[1]')
					
					SET @xTimeStampCartella.modify('delete (/TimeStamp/CodAzione)[1]') 			
					SET @xTimeStampCartella.modify('insert <CodAzione>INS</CodAzione> into (/TimeStamp)[1]')
						
										IF @uIDCartellaCollegata IS NOT NULL					
					BEGIN
						SET @xCartella.modify('insert <IDCartellaCollegata>{sql:variable("@uIDCartellaCollegata")}</IDCartellaCollegata> as first into (/Parametri)[1]')
					END
					
					
					SET @xCartella.modify('insert sql:variable("@xTimeStampCartella") as last into (/Parametri)[1]')								
				
											CREATE TABLE #tmpCartella
								(
									IDCartella UNIQUEIDENTIFIER
								)
						
												INSERT #tmpCartella EXEC MSP_InsMovCartelle @xCartella
					
						SET @uIDCartella=(SELECT TOP 1 IDCartella FROM #tmpCartella)					
				END
				ELSE
				BEGIN
										
					SET @sCodStatoCartellaProduzione=(SELECT CodStatoCartella FROM T_MovCartelle WHERE ID=@uIDCartella) 
					IF @sCodStatoCartellaProduzione='CH'
					BEGIN
						SET @xCartella='<Parametri></Parametri>'
				
												SET @xCartella.modify('insert <IDCartella>{sql:variable("@uIDCartella")}</IDCartella> as first into (/Parametri)[1]')
						
												SET @xCartella.modify('insert <NumeroCartella>{sql:variable("@sNumeroCartella")}</NumeroCartella> as first into (/Parametri)[1]')
						
												SET @xCartella.modify('insert <CodStatoCartella>{sql:variable("@sCodStatoCartella")}</CodStatoCartella> as first into (/Parametri)[1]')
						
												SET @xCartella.modify('insert <OperazioneDiSistema>{sql:variable("@nOperazioneDiSistema")}</OperazioneDiSistema> as first into (/Parametri)[1]')
																	
												SET @xTimeStampCartella=@xTimeStamp
										
						SET @xTimeStampCartella.modify('delete (/TimeStamp/IDEntita)[1]') 	
											
						SET @xTimeStampCartella.modify('delete (/TimeStamp/CodEntita)[1]') 						
						SET @xTimeStampCartella.modify('insert <CodEntita>CAR</CodEntita> into (/TimeStamp)[1]')
						
												SET @xTimeStampCartella.modify('delete (/TimeStamp/CodAzione)[1]') 			
						SET @xTimeStampCartella.modify('insert <CodAzione>MOD</CodAzione> into (/TimeStamp)[1]')
										
						SET @xCartella.modify('insert sql:variable("@xTimeStampCartella") as last into (/Parametri)[1]')
						
												EXEC MSP_AggMovCartelle @xCartella
					END
				END
																
		  END
		ELSE
			IF @sCodStatoCartella='CH' 
				BEGIN
				
					SET @xCartella='<Parametri></Parametri>'
			
										SET @xCartella.modify('insert <IDCartella>{sql:variable("@uIDCartella")}</IDCartella> as first into (/Parametri)[1]')
					
										SET @xCartella.modify('insert <NumeroCartella>{sql:variable("@sNumeroCartella")}</NumeroCartella> as first into (/Parametri)[1]')
					
										SET @xCartella.modify('insert <CodStatoCartella>{sql:variable("@sCodStatoCartella")}</CodStatoCartella> as first into (/Parametri)[1]')
					
										SET @xTimeStampCartella=@xTimeStamp
									
					SET @xTimeStampCartella.modify('delete (/TimeStamp/IDEntita)[1]') 	
										
					SET @xTimeStampCartella.modify('delete (/TimeStamp/CodEntita)[1]') 						
					SET @xTimeStampCartella.modify('insert <CodEntita>CAR</CodEntita> into (/TimeStamp)[1]')
					
										SET @xTimeStampCartella.modify('delete (/TimeStamp/CodAzione)[1]') 			
					SET @xTimeStampCartella.modify('insert <CodAzione>COM</CodAzione> into (/TimeStamp)[1]')
									
					SET @xCartella.modify('insert sql:variable("@xTimeStampCartella") as last into (/Parametri)[1]')
																			
					EXEC MSP_AggMovCartelle @xCartella					
										
				END  										 		
		
		IF @sCodStatoCartella='AP' AND @sCodStatoTrasferimento IS NOT NULL
		BEGIN			
						SET @sSQL='UPDATE T_MovTrasferimenti ' + CHAR(13) + CHAR(10) +
				  'SET '
				  					
				SET @sSET =''
				
				IF @uIDCartella IS NOT NULL
						SET @sSET=@sSET + ',IDCartella=''' + CONVERT(VARCHAR(50),@uIDCartella) + ''''
				
				IF @sCodStatoTrasferimento<> ''  
						SET @sSET=@sSET + ',CodStatoTrasferimento=''' + @sCodStatoTrasferimento + ''''
									
								IF LEFT(@sSET,1)=',' 
					SET @sSET=RIGHT(@sSET,LEN(@sSET)-1)
				END

				IF @sSET <> ''	
					BEGIN
												
												SET @sWHERE =' WHERE ID=''' + convert(varchar(50),@uIDTrasferimento) +''''
						
												SET @sSQL=	ISNULL(@sSQL,'') + CHAR(13) + CHAR(10) + 
									ISNULL(@sSET,'') + CHAR(13) + CHAR(10) + 
									ISNULL(@sWHERE,'')	
																				
																												
												SET @xTimeStamp.modify('delete (/TimeStamp/CodEntita)[1]') 							
						SET @xTimeStamp.modify('insert <CodEntita>TRA</CodEntita> into (/TimeStamp)[1]')		
				
												SET @xTimeStamp.modify('delete (/TimeStamp/IDEntita)[1]') 							
						SET @xTimeStamp.modify('insert <IDEntita>{sql:variable("@uIDTrasferimento")}</IDEntita> into (/TimeStamp)[1]')
						
												SET @xTimeStamp.modify('delete (/TimeStamp/CodAzione)[1]') 							
						SET @xTimeStamp.modify('insert <CodAzione>MOD</CodAzione> into (/TimeStamp)[1]')	
				
												SET @xTimeStampBase=@xTimeStamp
				
						SET @xTimeStamp=CONVERT(XML,
													'<Parametri>' + CONVERT(varchar(max),@xTimeStamp) +
													'</Parametri>')
							
																								
						SET @xParLog=CONVERT(XML,'<Parametri><LogPrima/><LogDopo/></Parametri>')										
						
						SET @xLogPrima=Convert(XML,'<DataSet></DataSet>')														
						
						SET @xTemp=
							(SELECT * FROM 
								(SELECT M.*,									
										CR.NumeroCartella,
										CR.CodStatoCartella,
										CR.CodUtenteApertura,
										CR.CodUtenteChiusura,
										CR.DataApertura,
										CR.DataAperturaUTC,
										CR.DataChiusura,
										CR.DataChiusuraUTC								
									FROM T_MovTrasferimenti M
										LEFT JOIN T_MovCartelle CR
											ON M.IDCartella=CR.ID
									WHERE M.ID=@uIDTrasferimento													) AS [Table]
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
										(SELECT M.*,												
												CR.NumeroCartella,
												CR.CodStatoCartella,
												CR.CodUtenteApertura,
												CR.CodUtenteChiusura,
												CR.DataApertura,
												CR.DataAperturaUTC,
												CR.DataChiusura,
												CR.DataChiusuraUTC	
										 FROM T_MovTrasferimenti M
											LEFT JOIN T_MovCartelle CR
												ON M.IDCartella=CR.ID
										 WHERE M.ID=@uIDTrasferimento															) AS [Table]
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
				END	
							END
	RETURN 0
END