CREATE PROCEDURE [dbo].[MSP_AggMovConsegne](@xParametri XML)
AS
BEGIN
		
	
							
		DECLARE @uIDConsegna AS UNIQUEIDENTIFIER	
	DECLARE @sCodUA AS VARCHAR(20)
	DECLARE @dDataEvento AS DATETIME	
	DECLARE @dDataEventoUTC AS DATETIME
	
	DECLARE @sCodTipoConsegna AS VARCHAR(20)	
	DECLARE @sCodStatoConsegna AS VARCHAR(20)	
	DECLARE @sCodUtenteRilevazione AS VARCHAR(100)
	DECLARE @sCodUtenteAnnullamento AS VARCHAR(100)
	DECLARE @sCodUtenteUltimaModifica AS VARCHAR(100)
	
							
	DECLARE @dDataAnnullamento AS DATETIME	
	DECLARE @dDataAnnullamentoUTC AS DATETIME
	
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
	
		SET @sSQL='UPDATE T_MovConsegne ' + CHAR(13) + CHAR(10) +
			  'SET '  				  						
			
	SET @sSET =''
	
	IF @xParametri.exist('/Parametri/IDConsegna')=1
	BEGIN
				SET @sGUID=(SELECT TOP 1 ValoreParametro.IDConsegna.value('.','VARCHAR(50)')
						  FROM @xParametri.nodes('/Parametri/IDConsegna') as ValoreParametro(IDConsegna))
						  
		IF 	ISNULL(@sGUID,'') <> '' 
				SET @uIDConsegna=CONVERT(UNIQUEIDENTIFIER,	@sGUID)			
			END
	
		IF @xParametri.exist('/Parametri/CodUA')=1
	BEGIN
			SET @sCodUA=(SELECT TOP 1 ValoreParametro.CodUA.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodUA') as ValoreParametro(CodUA))	
					  
			IF @sCodUA <> ''
				SET	@sSET= @sSET + ',CodUA=''' + @sCodUA +''''	+ CHAR(13) + CHAR(10)				
			ELSE
				SET	@sSET= @sSET + ',CodUA=NULL'	+ CHAR(13) + CHAR(10)	
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
		
		IF @dDataEventoUTC IS NOT NULL		
			SET	@sSET= @sSET + ',DataEventoUTC=CONVERT(DateTime,''' + CONVERT(VARCHAR(20),@dDataEventoUTC,120) + ''',120)' + CHAR(13) + CHAR(10)		
		ELSE
			SET	@sSET= @sSET + ',DataEventoUTC=NULL ' + CHAR(13) + CHAR(10)		
	END	

		IF @xParametri.exist('/Parametri/CodTipoConsegna')=1
	BEGIN
		SET @sCodTipoConsegna=(SELECT TOP 1 ValoreParametro.CodTipoConsegna.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodTipoConsegna') as ValoreParametro(CodTipoConsegna))	
					  
		IF @sCodTipoConsegna <> ''
			BEGIN									
				SET	@sSET= @sSET + ',CodTipoConsegna=''' + @sCodTipoConsegna +''''	+ CHAR(13) + CHAR(10)				
			END	
			ELSE
				SET	@sSET= @sSET + ',CodTipoConsegna=NULL'	+ CHAR(13) + CHAR(10)		
	END			
						  
		IF @xParametri.exist('/Parametri/CodStatoConsegna')=1
		BEGIN
			SET @sCodStatoConsegna=(SELECT TOP 1 ValoreParametro.CodStatoConsegna.value('.','VARCHAR(20)')
						  FROM @xParametri.nodes('/Parametri/CodStatoConsegna') as ValoreParametro(CodStatoConsegna))	
						  
			IF @sCodStatoConsegna <> ''
					SET	@sSET= @sSET + ',CodStatoConsegna=''' + @sCodStatoConsegna+''''	+ CHAR(13) + CHAR(10)				
				ELSE
					SET	@sSET= @sSET + ',CodStatoConsegna=NULL'	+ CHAR(13) + CHAR(10)		
		END	

		IF @xParametri.exist('/Parametri/CodUtenteRilevazione')=1
	BEGIN		
		SET @sCodUtenteRilevazione=(SELECT TOP 1 ValoreParametro.CodUtenteRilevazione.value('.','VARCHAR(100)')
						  FROM @xParametri.nodes('/Parametri/CodUtenteRilevazione') as ValoreParametro(CodUtenteRilevazione))	
		SET @sCodUtenteRilevazione=ISNULL(@sCodUtenteRilevazione,'')
		
		IF @sCodUtenteRilevazione <> ''
			SET	@sSET= @sSET + ',CodUtenteRilevazione=''' + @sCodUtenteRilevazione + '''' + CHAR(13) + CHAR(10)		
		ELSE
			SET	@sSET= @sSET + ',CodUtenteRilevazione=NULL' + CHAR(13) + CHAR(10)		
	END	

		IF @xParametri.exist('/Parametri/CodUtenteUltimaModifica')=1
	BEGIN		
		SET @sCodUtenteUltimaModifica=(SELECT TOP 1 ValoreParametro.CodUtenteUltimaModifica.value('.','VARCHAR(100)')
						  FROM @xParametri.nodes('/Parametri/CodUtenteUltimaModifica') as ValoreParametro(CodUtenteUltimaModifica))	
		SET @sCodUtenteUltimaModifica=ISNULL(@sCodUtenteUltimaModifica,'')
		
		IF @sCodUtenteUltimaModifica <> ''
			SET	@sSET= @sSET + ',CodUtenteUltimaModifica=''' + @sCodUtenteUltimaModifica + '''' + CHAR(13) + CHAR(10)				
	END
	ELSE
		BEGIN
						SET @sCodUtenteUltimaModifica=(SELECT TOP 1 ValoreParametro.CodUtenteUltimaModifica.value('.','VARCHAR(100)')
					  FROM @xParametri.nodes('/Parametri/TimeStamp/CodLogin') as ValoreParametro(CodUtenteUltimaModifica))	
			SET @sCodUtenteUltimaModifica=ISNULL(@sCodUtenteUltimaModifica,'')
			IF @sCodUtenteUltimaModifica <> ''
				SET	@sSET= @sSET + ',CodUtenteUltimaModifica=''' + @sCodUtenteUltimaModifica + '''' + CHAR(13) + CHAR(10)	
		END
		
		IF @xParametri.exist('/Parametri/CodUtenteAnnullamento')=1
	BEGIN		
		SET @sCodUtenteAnnullamento=(SELECT TOP 1 ValoreParametro.CodUtenteAnnullamento.value('.','VARCHAR(100)')
						  FROM @xParametri.nodes('/Parametri/CodUtenteAnnullamento') as ValoreParametro(CodUtenteAnnullamento))	
		SET @sCodUtenteAnnullamento=ISNULL(@sCodUtenteUltimaModifica,'')
		
		IF @sCodUtenteAnnullamento <> ''
			SET	@sSET= @sSET + ',CodUtenteAnnullamento=''' + @sCodUtenteAnnullamento + '''' + CHAR(13) + CHAR(10)		
		ELSE
			SET	@sSET= @sSET + ',CodUtenteAnnullamento=NULL' + CHAR(13) + CHAR(10)		
	END	
	 ELSE
		BEGIN
						IF @sCodStatoConsegna ='AN'
				SET	@sSET= @sSET + ',CodUtenteAnnullamento='''  + @sCodUtenteUltimaModifica + '''' + CHAR(13) + CHAR(10)	
		END

		IF @xParametri.exist('/Parametri/DataAnnullamento')=1
		BEGIN			
			SET @sDataTmp=(SELECT TOP 1 ValoreParametro.DataAnnullamento.value('.','VARCHAR(20)')
							  FROM @xParametri.nodes('/Parametri/DataAnnullamento') as ValoreParametro(DataAnnullamento))					  
			SET @sDataTmp=ISNULL(@sDataTmp,'')
			
			IF @sDataTmp<> '' 
				BEGIN			
										SET @sDataTmp= SUBSTRING(@sDataTmp,4,2) + '-' 
								+ LEFT(@sDataTmp,2) + '-' +
								+ SUBSTRING(@sDataTmp,7,4) +
								' ' + RIGHT(@sDataTmp,5)
					IF ISDATE(@sDataTmp)=1
						SET	@dDataAnnullamento=CONVERT(DATETIME,@sDataTmp,120)						
					ELSE
						SET	@dDataAnnullamento =NULL			
				END
			IF @dDataAnnullamento IS NOT NULL
				SET	@sSET= @sSET + ',DataAnnullamento=CONVERT(DateTime,''' + CONVERT(VARCHAR(20),@dDataAnnullamento,120) + ''',120)' + CHAR(13) + CHAR(10)		 
			ELSE	
				SET	@sSET= @sSET + ',DataAnnullamento=NULL' + CHAR(13) + CHAR(10)		 
		END	
	 ELSE
		BEGIN
						IF @sCodStatoConsegna ='AN'
				SET	@sSET= @sSET + ',DataAnnullamento=GETDATE()' + CHAR(13) + CHAR(10)	
		END

			
		IF @xParametri.exist('/Parametri/DataAnnullamentoUTC')=1
		BEGIN			
			SET @sDataTmp=(SELECT TOP 1 ValoreParametro.DataAnnullamentoUTC.value('.','VARCHAR(20)')
							  FROM @xParametri.nodes('/Parametri/DataAnnullamentoUTC') as ValoreParametro(DataAnnullamentoUTC))					  
			SET @sDataTmp=ISNULL(@sDataTmp,'')
			
			IF @sDataTmp<> '' 
				BEGIN			
										SET @sDataTmp= SUBSTRING(@sDataTmp,4,2) + '-' 
								+ LEFT(@sDataTmp,2) + '-' +
								+ SUBSTRING(@sDataTmp,7,4) +
								' ' + RIGHT(@sDataTmp,5)
					IF ISDATE(@sDataTmp)=1
						SET	@dDataAnnullamentoUTC=CONVERT(DATETIME,@sDataTmp,120)						
					ELSE
						SET	@dDataAnnullamentoUTC =NULL			
				END
			IF @dDataAnnullamentoUTC IS NOT NULL
				SET	@sSET= @sSET + ',DataAnnullamentoUTC=CONVERT(DateTime,''' + CONVERT(VARCHAR(20),@dDataAnnullamentoUTC,120) + ''',120)' + CHAR(13) + CHAR(10)		 
			ELSE	
				SET	@sSET= @sSET + ',DataAnnullamentoUTC=NULL' + CHAR(13) + CHAR(10)		 
		END	
	ELSE
		BEGIN
						IF @sCodStatoConsegna ='AN'
				SET	@sSET= @sSET + ',DataAnnullamentoUTC=GETUTCDATE()' + CHAR(13) + CHAR(10)	
		END
				
		SET	@sSET= @sSET + ',DataUltimaModifica=GETDATE()' + CHAR(13) + CHAR(10)
	SET	@sSET= @sSET + ',DataUltimaModificaUTC=GETUTCDATE()' + CHAR(13) + CHAR(10)

	    SET @xTimeStamp=(SELECT TOP 1 ValoreParametro.TS.query('.')
					  FROM @xParametri.nodes('/Parametri/TimeStamp') as ValoreParametro(TS))
		
		SET @xSchedaMovimento=(SELECT TOP 1 ValoreParametro.TS.query('.')
					  FROM @xParametri.nodes('/Parametri/SchedaMovimento') as ValoreParametro(TS))					
			
										
			
			
		IF LEFT(@sSET,1)=',' 
		SET @sSET=RIGHT(@sSET,LEN(@sSET)-1)


	IF LTRIM(RTRIM(@sSET)) <> ''
		BEGIN
				
				SET @sWHERE =' WHERE ID=''' + convert(varchar(50),@uIDConsegna) +''''
					
				SET @sSQL=	ISNULL(@sSQL,'') + CHAR(13) + CHAR(10) + 
					ISNULL(@sSET,'') + CHAR(13) + CHAR(10) + 
					ISNULL(@sWHERE,'')			


																
				SET @xTimeStamp.modify('delete (/TimeStamp/IDEntita)[1]') 
						
				SET @xTimeStamp.modify('insert <IDEntita>{sql:variable("@uIDConsegna")}</IDEntita> into (/TimeStamp)[1]')

				SET @xTimeStampBase=@xTimeStamp
	
				SET @xTimeStamp=CONVERT(XML,
									'<Parametri>' + CONVERT(varchar(max),@xTimeStamp) +
									'</Parametri>')
						
																
				SET @xParLog=CONVERT(XML,'<Parametri><LogPrima/><LogDopo/></Parametri>')										
				
				SET @xLogPrima=Convert(XML,'<DataSet></DataSet>')														
				
				SET @xTemp=
					(SELECT * FROM 
						(SELECT * FROM T_MovConsegne
						 WHERE ID=@uIDConsegna											) AS [Table]
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
									(SELECT * FROM T_MovConsegne
									 WHERE ID=@uIDConsegna														) AS [Table]
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