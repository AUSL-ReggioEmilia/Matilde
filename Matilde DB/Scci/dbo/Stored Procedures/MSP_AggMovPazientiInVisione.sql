CREATE PROCEDURE [dbo].[MSP_AggMovPazientiInVisione](@xParametri AS XML)
AS
BEGIN
	
											
		DECLARE @sCodRuolo AS VARCHAR(20)
	DECLARE @sCodUtenteUltimaModifica AS VARCHAR(100)	
	
	DECLARE @uIDPazienteInVisione AS UNIQUEIDENTIFIER	
	DECLARE @sCodRuoloInVisione AS VARCHAR(20)
	DECLARE @sCodStatoPazienteInVisione AS VARCHAR(20)	
	
	DECLARE @dDataInizio AS DATETIME	
	DECLARE @dDataInizioUTC AS DATETIME
	
	DECLARE @dDataFine AS DATETIME	
	DECLARE @dDataFineUTC AS DATETIME			
		
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
	
						
		SET @sSQL='UPDATE T_MovPazientiInVisione ' + CHAR(13) + CHAR(10) +
			  'SET '
			  					
	SET @sSET =''
	
		SET @sCodUtenteUltimaModifica=(SELECT TOP 1 ValoreParametro.CodUtente.value('.','VARCHAR(100)')
					  FROM @xParametri.nodes('/Parametri/TimeStamp/CodLogin') as ValoreParametro(CodUtente))	
	SET @sCodUtenteUltimaModifica=ISNULL(@sCodUtenteUltimaModifica,'')

		SET @sCodRuolo=(SELECT TOP 1 ValoreParametro.CodRuolo.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/TimeStamp/CodRuolo') as ValoreParametro(CodRuolo))
	SET @sCodRuolo=ISNULL(@sCodRuolo,'')	
			
		IF @xParametri.exist('/Parametri/IDPazienteInVisione')=1
		BEGIN
			SET @sGUID=(SELECT TOP 1 ValoreParametro.IDPazienteInVisione.value('.','VARCHAR(50)')
							  FROM @xParametri.nodes('/Parametri/IDPazienteInVisione') as ValoreParametro(IDPazienteInVisione))
			IF 	ISNULL(@sGUID,'') <> '' 
					SET @uIDPazienteInVisione=CONVERT(UNIQUEIDENTIFIER,	@sGUID)	
					END	
					
		IF @xParametri.exist('/Parametri/CodRuoloInVisione')=1
	BEGIN
		SET @sCodRuoloInVisione=(SELECT TOP 1 ValoreParametro.CodRuoloInVisione.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodRuoloInVisione') as ValoreParametro(CodRuoloInVisione))	
					  
		IF @sCodRuoloInVisione <> ''
				SET	@sSET= @sSET + ',CodRuoloInVisione=''' + @sCodRuoloInVisione +''''	+ CHAR(13) + CHAR(10)				
			ELSE
				SET	@sSET= @sSET + ',CodRuoloInVisione=NULL'	+ CHAR(13) + CHAR(10)		
	END	
	
		IF @xParametri.exist('/Parametri/CodStatoPazienteInVisione')=1
	BEGIN
		SET @sCodStatoPazienteInVisione=(SELECT TOP 1 ValoreParametro.CodStatoPazienteInVisione.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodStatoPazienteInVisione') as ValoreParametro(CodStatoPazienteInVisione))	
					  
		IF @sCodStatoPazienteInVisione <> ''
				SET	@sSET= @sSET + ',CodStatoPazienteInVisione=''' + @sCodStatoPazienteInVisione +''''	+ CHAR(13) + CHAR(10)				
			ELSE
				SET	@sSET= @sSET + ',CodStatoPazienteInVisione=NULL'	+ CHAR(13) + CHAR(10)		
	END		
		
		IF @xParametri.exist('/Parametri/DataInizio')=1
		BEGIN			
			SET @sDataTmp=(SELECT TOP 1 ValoreParametro.DataInizio.value('.','VARCHAR(20)')
							  FROM @xParametri.nodes('/Parametri/DataInizio') as ValoreParametro(DataInizio))					  
			SET @sDataTmp=ISNULL(@sDataTmp,'')
			
			IF @sDataTmp<> '' 
				BEGIN			
										SET @sDataTmp= SUBSTRING(@sDataTmp,4,2) + '-' 
								+ LEFT(@sDataTmp,2) + '-' +
								+ SUBSTRING(@sDataTmp,7,4) +
								' ' + RIGHT(@sDataTmp,5)
					IF ISDATE(@sDataTmp)=1
						SET	@dDataInizio=CONVERT(DATETIME,@sDataTmp,120)						
					ELSE
						SET	@dDataInizio =NULL			
				END
			IF @dDataInizio IS NOT NULL
				SET	@sSET= @sSET + ',DataInizio=CONVERT(DateTime,''' + CONVERT(VARCHAR(20),@dDataInizio,120) + ''',120)' + CHAR(13) + CHAR(10)		 
			ELSE	
				SET	@sSET= @sSET + ',DataInizio=NULL' + CHAR(13) + CHAR(10)		 
		END	
		
		
		IF @xParametri.exist('/Parametri/DataInizioUTC ')=1
		BEGIN			
			SET @sDataTmp=(SELECT TOP 1 ValoreParametro.DataInizioUTC .value('.','VARCHAR(20)')
							  FROM @xParametri.nodes('/Parametri/DataInizioUTC ') as ValoreParametro(DataInizioUTC ))					  
			SET @sDataTmp=ISNULL(@sDataTmp,'')
			
			IF @sDataTmp<> '' 
				BEGIN			
										SET @sDataTmp= SUBSTRING(@sDataTmp,4,2) + '-' 
								+ LEFT(@sDataTmp,2) + '-' +
								+ SUBSTRING(@sDataTmp,7,4) +
								' ' + RIGHT(@sDataTmp,5)
					IF ISDATE(@sDataTmp)=1
						SET	@dDataInizioUTC =CONVERT(DATETIME,@sDataTmp,120)						
					ELSE
						SET	@dDataInizioUTC  =NULL			
				END
			IF @dDataInizioUTC  IS NOT NULL
				SET	@sSET= @sSET + ',DataInizioUTC =CONVERT(DateTime,''' + CONVERT(VARCHAR(20),@dDataInizioUTC ,120) + ''',120)' + CHAR(13) + CHAR(10)		 
			ELSE	
				SET	@sSET= @sSET + ',DataInizioUTC =NULL' + CHAR(13) + CHAR(10)		 
		END			

			
		IF @xParametri.exist('/Parametri/DataFine')=1
		BEGIN			
			SET @sDataTmp=(SELECT TOP 1 ValoreParametro.DataFine.value('.','VARCHAR(20)')
							  FROM @xParametri.nodes('/Parametri/DataFine') as ValoreParametro(DataFine))					  
			SET @sDataTmp=ISNULL(@sDataTmp,'')
			
			IF @sDataTmp<> '' 
				BEGIN			
										SET @sDataTmp= SUBSTRING(@sDataTmp,4,2) + '-' 
								+ LEFT(@sDataTmp,2) + '-' +
								+ SUBSTRING(@sDataTmp,7,4) +
								' ' + RIGHT(@sDataTmp,5)
					IF ISDATE(@sDataTmp)=1
						SET	@dDataFine=CONVERT(DATETIME,@sDataTmp,120)						
					ELSE
						SET	@dDataFine =NULL			
				END
			IF @dDataFine IS NOT NULL
				SET	@sSET= @sSET + ',DataFine=CONVERT(DateTime,''' + CONVERT(VARCHAR(20),@dDataFine,120) + ''',120)' + CHAR(13) + CHAR(10)		 
			ELSE	
				SET	@sSET= @sSET + ',DataFine=NULL' + CHAR(13) + CHAR(10)		 
		END	
	
		IF @xParametri.exist('/Parametri/DataFineUTC ')=1
		BEGIN			
			SET @sDataTmp=(SELECT TOP 1 ValoreParametro.DataFineUTC .value('.','VARCHAR(20)')
							  FROM @xParametri.nodes('/Parametri/DataFineUTC ') as ValoreParametro(DataFineUTC ))					  
			SET @sDataTmp=ISNULL(@sDataTmp,'')
			
			IF @sDataTmp<> '' 
				BEGIN			
										SET @sDataTmp= SUBSTRING(@sDataTmp,4,2) + '-' 
								+ LEFT(@sDataTmp,2) + '-' +
								+ SUBSTRING(@sDataTmp,7,4) +
								' ' + RIGHT(@sDataTmp,5)
					IF ISDATE(@sDataTmp)=1
						SET	@dDataFineUTC =CONVERT(DATETIME,@sDataTmp,120)						
					ELSE
						SET	@dDataFineUTC  =NULL			
				END
			IF @dDataFineUTC  IS NOT NULL
				SET	@sSET= @sSET + ',DataFineUTC =CONVERT(DateTime,''' + CONVERT(VARCHAR(20),@dDataFineUTC ,120) + ''',120)' + CHAR(13) + CHAR(10)		 
			ELSE	
				SET	@sSET= @sSET + ',DataFineUTC =NULL' + CHAR(13) + CHAR(10)		 
		END			
	
		IF @sCodUtenteUltimaModifica <> ''
		SET	@sSET= @sSET + ',CodUtenteUltimaModifica=''' + @sCodUtenteUltimaModifica + '''' + CHAR(13) + CHAR(10)		
	ELSE
		SET	@sSET= @sSET + ',CodUtenteUltimaModifica=NULL' + CHAR(13) + CHAR(10)		
	
	
		SET	@sSET= @sSET + ',DataUltimaModifica=getdate() ' + CHAR(13) + CHAR(10)	
		
		SET	@sSET= @sSET + ',DataUltimaModificaUTC=getUTCdate() ' + CHAR(13) + CHAR(10)	
					
	    SET @xTimeStamp=(SELECT TOP 1 ValoreParametro.TS.query('.')
					  FROM @xParametri.nodes('/Parametri/TimeStamp') as ValoreParametro(TS))	
	
				
		IF LEFT(@sSET,1)=',' 
		SET @sSET=RIGHT(@sSET,LEN(@sSET)-1)
	END
		
		
	IF @sSET <> ''	
		BEGIN
						
						IF @uIDPazienteInVisione IS NULL
				SET @sWHERE='WHERE 1=0'
			ELSE	
				SET @sWHERE =' WHERE ID=''' + convert(varchar(50),@uIDPazienteInVisione) +''''
			
			
						SET @sSQL=	ISNULL(@sSQL,'') + CHAR(13) + CHAR(10) + 
						ISNULL(@sSET,'') + CHAR(13) + CHAR(10) + 
						ISNULL(@sWHERE,'')	
			
																
						SET @xTimeStamp.modify('delete (/TimeStamp/IDEntita)[1]') 						
			SET @xTimeStamp.modify('insert <IDEntita>{sql:variable("@uIDPazienteInVisione")}</IDEntita> into (/TimeStamp)[1]')
	
						SET @xTimeStamp.modify('delete (/TimeStamp/CodEntita)[1]') 						
			SET @xTimeStamp.modify('insert <CodEntita>PIV</CodEntita> into (/TimeStamp)[1]')
						
												
						SET @xTimeStampBase=@xTimeStamp
	
			SET @xTimeStamp=CONVERT(XML,
										'<Parametri>' + CONVERT(varchar(max),@xTimeStamp) +
										'</Parametri>')
				
												
			SET @xParLog=CONVERT(XML,'<Parametri><LogPrima/><LogDopo/></Parametri>')										
			
			SET @xLogPrima=Convert(XML,'<DataSet></DataSet>')														
			
			SET @xTemp=
				(SELECT * FROM 
					(SELECT  *
					 FROM T_MovPazientiInVisione
					 WHERE ID=@uIDPazienteInVisione									) AS [Table]
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
								 FROM T_MovPazientiInVisione
								 WHERE ID=@uIDPazienteInVisione											) AS [Table]
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