CREATE PROCEDURE [dbo].[MSP_AggMovDiarioClinico](@xParametri XML)
AS
BEGIN
		
	
							
		DECLARE @uIDDiarioClinico AS UNIQUEIDENTIFIER	
	DECLARE @sCodUA AS VARCHAR(20)
	DECLARE @dDataEvento AS DATETIME	
	DECLARE @dDataEventoUTC AS DATETIME
	DECLARE @uIDEpisodio AS UNIQUEIDENTIFIER	
	DECLARE @uIDTrasferimento AS UNIQUEIDENTIFIER	
	
	DECLARE @sCodTipoDiario AS VARCHAR(20)
	DECLARE @sCodTipoVoceDiario AS VARCHAR(20)
	DECLARE @sCodTipoRegistrazione AS VARCHAR(20)
	DECLARE @sCodEntitaRegistrazione AS VARCHAR(20)
	DECLARE @uIDEntitaRegistrazione AS UNIQUEIDENTIFIER
	DECLARE @sCodStatoDiario AS VARCHAR(20)
	DECLARE @sCodUtenteRilevazione AS VARCHAR(100)
	
	DECLARE @dDataInserimento AS DATETIME	
	DECLARE @dDataInserimentoUTC AS DATETIME					
	DECLARE @dDataValidazione AS DATETIME	
	DECLARE @dDataValidazioneUTC AS DATETIME
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
	
		SET @sSQL='UPDATE T_MovDiarioClinico ' + CHAR(13) + CHAR(10) +
			  'SET '  				  						
			
	SET @sSET =''
	
	IF @xParametri.exist('/Parametri/IDDiarioClinico')=1
	BEGIN
				SET @sGUID=(SELECT TOP 1 ValoreParametro.IDDiarioClinico.value('.','VARCHAR(50)')
						  FROM @xParametri.nodes('/Parametri/IDDiarioClinico') as ValoreParametro(IDDiarioClinico))
						  
		IF 	ISNULL(@sGUID,'') <> '' 
				SET @uIDDiarioClinico=CONVERT(UNIQUEIDENTIFIER,	@sGUID)			
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

		IF @xParametri.exist('/Parametri/IDEpisodio')=1
		BEGIN
			SET @sGUID=(SELECT TOP 1 ValoreParametro.IDEpisodio.value('.','VARCHAR(50)')
							  FROM @xParametri.nodes('/Parametri/IDEpisodio') as ValoreParametro(IDEpisodio))
							  
				IF 	ISNULL(@sGUID,'') <> '' 
				BEGIN
					SET @uIDEpisodio=CONVERT(UNIQUEIDENTIFIER,	@sGUID)		
					SET	@sSET= @sSET + ',IDEpisodio=''' + convert(VARCHAR(50),@uIDEpisodio) + ''''	+ CHAR(13) + CHAR(10)	
				END
				ELSE
					SET	@sSET= @sSET + ',IDEpisodio=NULL'	+ CHAR(13) + CHAR(10)									  		
		END			

		IF @xParametri.exist('/Parametri/IDTrasferimento')=1
		BEGIN
			SET @sGUID=(SELECT TOP 1 ValoreParametro.IDTrasferimento.value('.','VARCHAR(50)')
							  FROM @xParametri.nodes('/Parametri/IDTrasferimento') as ValoreParametro(IDTrasferimento))
							  
				IF 	ISNULL(@sGUID,'') <> '' 
				BEGIN
					SET @uIDTrasferimento=CONVERT(UNIQUEIDENTIFIER,	@sGUID)		
					SET	@sSET= @sSET + ',IDTrasferimento=''' + convert(VARCHAR(50),@uIDTrasferimento) + ''''	+ CHAR(13) + CHAR(10)	
				END
				ELSE
					SET	@sSET= @sSET + ',IDTrasferimento=NULL'	+ CHAR(13) + CHAR(10)									  		
		END		
			

		IF @xParametri.exist('/Parametri/CodTipoVoceDiario')=1
	BEGIN
		SET @sCodTipoVoceDiario=(SELECT TOP 1 ValoreParametro.CodTipoVoceDiario.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodTipoVoceDiario') as ValoreParametro(CodTipoVoceDiario))	
					  
		IF @sCodTipoVoceDiario <> ''
				SET	@sSET= @sSET + ',CodTipoVoceDiario=''' + @sCodTipoVoceDiario +''''	+ CHAR(13) + CHAR(10)				
			ELSE
				SET	@sSET= @sSET + ',CodTipoVoceDiario=NULL'	+ CHAR(13) + CHAR(10)		
	END	
	
		IF @xParametri.exist('/Parametri/CodTipoDiario')=1
	BEGIN
		SET @sCodTipoDiario=(SELECT TOP 1 ValoreParametro.CodTipoDiario.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodTipoDiario') as ValoreParametro(CodTipoDiario))	
					  
		IF @sCodTipoDiario <> ''
			BEGIN									
				SET	@sSET= @sSET + ',CodTipoDiario=''' + @sCodTipoDiario +''''	+ CHAR(13) + CHAR(10)				
			END	
			ELSE
				SET	@sSET= @sSET + ',CodTipoDiario=NULL'	+ CHAR(13) + CHAR(10)		
	END	
	ELSE
		BEGIN
										SET @sCodTipoDiario=(SELECT TOP 1 CodTipoDiario FROM T_TipoVoceDiario WHERE Codice=@sCodTipoVoceDiario)
				IF @sCodTipoDiario <> ''														
					SET	@sSET= @sSET + ',CodTipoDiario=''' + @sCodTipoDiario +''''	+ CHAR(13) + CHAR(10)								
		END
	

	
		IF @xParametri.exist('/Parametri/CodEntitaRegistrazione')=1
		BEGIN
			SET @sCodEntitaRegistrazione=(SELECT TOP 1 ValoreParametro.CodEntitaRegistrazione.value('.','VARCHAR(20)')
						  FROM @xParametri.nodes('/Parametri/CodEntitaRegistrazione') as ValoreParametro(CodEntitaRegistrazione))	
						  
			IF @sCodEntitaRegistrazione <> ''
					SET	@sSET= @sSET + ',CodEntitaRegistrazione=''' + @sCodEntitaRegistrazione+''''	+ CHAR(13) + CHAR(10)				
				ELSE
					SET	@sSET= @sSET + ',CodEntitaRegistrazione=NULL'	+ CHAR(13) + CHAR(10)		
		END		
		
	
		IF @xParametri.exist('/Parametri/CodTipoRegistrazione')=1
		BEGIN
			SET @sCodTipoRegistrazione=(SELECT TOP 1 ValoreParametro.CodTipoRegistrazione.value('.','VARCHAR(20)')
						  FROM @xParametri.nodes('/Parametri/CodTipoRegistrazione') as ValoreParametro(CodTipoRegistrazione))	
						  
			IF @sCodTipoRegistrazione <> ''
					SET	@sSET= @sSET + ',CodTipoRegistrazione=''' + @sCodTipoRegistrazione+''''	+ CHAR(13) + CHAR(10)				
				ELSE
					SET	@sSET= @sSET + ',CodTipoRegistrazione=NULL'	+ CHAR(13) + CHAR(10)		
		END				
		
		IF @xParametri.exist('/Parametri/IDEntitaRegistrazione')=1
		BEGIN
			SET @sGUID=(SELECT TOP 1 ValoreParametro.IDEntitaRegistrazione.value('.','VARCHAR(50)')
							  FROM @xParametri.nodes('/Parametri/IDEntitaRegistrazione') as ValoreParametro(IDEntitaRegistrazione))
							  
				IF 	ISNULL(@sGUID,'') <> '' 
				BEGIN
					SET @uIDEntitaRegistrazione=CONVERT(UNIQUEIDENTIFIER,	@sGUID)		
					SET	@sSET= @sSET + ',IDEntitaRegistrazione=''' + convert(VARCHAR(50),@uIDEntitaRegistrazione) + ''''	+ CHAR(13) + CHAR(10)	
				END
				ELSE
					SET	@sSET= @sSET + ',IDEntitaRegistrazione=NULL'	+ CHAR(13) + CHAR(10)									  		
		END		
		
						  
		IF @xParametri.exist('/Parametri/CodStatoDiario')=1
		BEGIN
			SET @sCodStatoDiario=(SELECT TOP 1 ValoreParametro.CodStatoDiario.value('.','VARCHAR(20)')
						  FROM @xParametri.nodes('/Parametri/CodStatoDiario') as ValoreParametro(CodStatoDiario))	
						  
			IF @sCodStatoDiario <> ''
					SET	@sSET= @sSET + ',CodStatoDiario=''' + @sCodStatoDiario+''''	+ CHAR(13) + CHAR(10)				
				ELSE
					SET	@sSET= @sSET + ',CodStatoDiario=NULL'	+ CHAR(13) + CHAR(10)		
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
	
		IF @xParametri.exist('/Parametri/DataInserimento')=1
		BEGIN			
			SET @sDataTmp=(SELECT TOP 1 ValoreParametro.DataInserimento.value('.','VARCHAR(20)')
							  FROM @xParametri.nodes('/Parametri/DataInserimento') as ValoreParametro(DataInserimento))					  
			SET @sDataTmp=ISNULL(@sDataTmp,'')
			
			IF @sDataTmp<> '' 
				BEGIN			
										SET @sDataTmp= SUBSTRING(@sDataTmp,4,2) + '-' 
								+ LEFT(@sDataTmp,2) + '-' +
								+ SUBSTRING(@sDataTmp,7,4) +
								' ' + RIGHT(@sDataTmp,5)
					IF ISDATE(@sDataTmp)=1
						SET	@dDataInserimento=CONVERT(DATETIME,@sDataTmp,120)						
					ELSE
						SET	@dDataInserimento =NULL			
				END
			IF @dDataInserimento IS NOT NULL
				SET	@sSET= @sSET + ',DataInserimento=CONVERT(DateTime,''' + CONVERT(VARCHAR(20),@dDataInserimento,120) + ''',120)' + CHAR(13) + CHAR(10)		 
			ELSE	
				SET	@sSET= @sSET + ',DataInserimento=NULL' + CHAR(13) + CHAR(10)		 
		END	
			
		IF @xParametri.exist('/Parametri/DataInserimentoUTC')=1
		BEGIN			
			SET @sDataTmp=(SELECT TOP 1 ValoreParametro.DataInserimentoUTC.value('.','VARCHAR(20)')
							  FROM @xParametri.nodes('/Parametri/DataInserimentoUTC') as ValoreParametro(DataInserimentoUTC))					  
			SET @sDataTmp=ISNULL(@sDataTmp,'')
			
			IF @sDataTmp<> '' 
				BEGIN			
										SET @sDataTmp= SUBSTRING(@sDataTmp,4,2) + '-' 
								+ LEFT(@sDataTmp,2) + '-' +
								+ SUBSTRING(@sDataTmp,7,4) +
								' ' + RIGHT(@sDataTmp,5)
					IF ISDATE(@sDataTmp)=1
						SET	@dDataInserimentoUTC=CONVERT(DATETIME,@sDataTmp,120)						
					ELSE
						SET	@dDataInserimentoUTC =NULL			
				END
			IF @dDataInserimentoUTC IS NOT NULL
				SET	@sSET= @sSET + ',DataInserimentoUTC=CONVERT(DateTime,''' + CONVERT(VARCHAR(20),@dDataInserimentoUTC,120) + ''',120)' + CHAR(13) + CHAR(10)		 
			ELSE	
				SET	@sSET= @sSET + ',DataInserimentoUTC=NULL' + CHAR(13) + CHAR(10)		 
		END	
	
	
	
		IF @xParametri.exist('/Parametri/DataValidazione')=1
		BEGIN			
			SET @sDataTmp=(SELECT TOP 1 ValoreParametro.DataValidazione.value('.','VARCHAR(20)')
							  FROM @xParametri.nodes('/Parametri/DataValidazione') as ValoreParametro(DataValidazione))					  
			SET @sDataTmp=ISNULL(@sDataTmp,'')
			
			IF @sDataTmp<> '' 
				BEGIN			
										SET @sDataTmp= SUBSTRING(@sDataTmp,4,2) + '-' 
								+ LEFT(@sDataTmp,2) + '-' +
								+ SUBSTRING(@sDataTmp,7,4) +
								' ' + RIGHT(@sDataTmp,5)
					IF ISDATE(@sDataTmp)=1
						SET	@dDataValidazione=CONVERT(DATETIME,@sDataTmp,120)						
					ELSE
						SET	@dDataValidazione =NULL			
				END
			IF @dDataValidazione IS NOT NULL
				SET	@sSET= @sSET + ',DataValidazione=CONVERT(DateTime,''' + CONVERT(VARCHAR(20),@dDataValidazione,120) + ''',120)' + CHAR(13) + CHAR(10)		 
			ELSE	
				SET	@sSET= @sSET + ',DataValidazione=NULL' + CHAR(13) + CHAR(10)		 
		END	
			
		IF @xParametri.exist('/Parametri/DataValidazioneUTC')=1
		BEGIN			
			SET @sDataTmp=(SELECT TOP 1 ValoreParametro.DataValidazioneUTC.value('.','VARCHAR(20)')
							  FROM @xParametri.nodes('/Parametri/DataValidazioneUTC') as ValoreParametro(DataValidazioneUTC))					  
			SET @sDataTmp=ISNULL(@sDataTmp,'')
			
			IF @sDataTmp<> '' 
				BEGIN			
										SET @sDataTmp= SUBSTRING(@sDataTmp,4,2) + '-' 
								+ LEFT(@sDataTmp,2) + '-' +
								+ SUBSTRING(@sDataTmp,7,4) +
								' ' + RIGHT(@sDataTmp,5)
					IF ISDATE(@sDataTmp)=1
						SET	@dDataValidazioneUTC=CONVERT(DATETIME,@sDataTmp,120)						
					ELSE
						SET	@dDataValidazioneUTC =NULL			
				END
			IF @dDataValidazioneUTC IS NOT NULL
				SET	@sSET= @sSET + ',DataValidazioneUTC=CONVERT(DateTime,''' + CONVERT(VARCHAR(20),@dDataValidazioneUTC,120) + ''',120)' + CHAR(13) + CHAR(10)		 
			ELSE	
				SET	@sSET= @sSET + ',DataValidazioneUTC=NULL' + CHAR(13) + CHAR(10)		 
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
				
	    SET @xTimeStamp=(SELECT TOP 1 ValoreParametro.TS.query('.')
					  FROM @xParametri.nodes('/Parametri/TimeStamp') as ValoreParametro(TS))
		
		SET @xSchedaMovimento=(SELECT TOP 1 ValoreParametro.TS.query('.')
					  FROM @xParametri.nodes('/Parametri/SchedaMovimento') as ValoreParametro(TS))					
			
										
			
			
		IF LEFT(@sSET,1)=',' 
		SET @sSET=RIGHT(@sSET,LEN(@sSET)-1)


	IF LTRIM(RTRIM(@sSET)) <> ''
		BEGIN
				SET @sWHERE =' WHERE ID=''' + convert(varchar(50),@uIDDiarioClinico) +''''
					
				SET @sSQL=	ISNULL(@sSQL,'') + CHAR(13) + CHAR(10) + 
							ISNULL(@sSET,'') + CHAR(13) + CHAR(10) + 
							ISNULL(@sWHERE,'')			
				
				SET @xTimeStamp.modify('delete (/TimeStamp/IDEntita)[1]') 
						
				SET @xTimeStamp.modify('insert <IDEntita>{sql:variable("@uIDDiarioClinico")}</IDEntita> into (/TimeStamp)[1]')

				SET @xTimeStampBase=@xTimeStamp
	
				SET @xTimeStamp=CONVERT(XML,
									'<Parametri>' + CONVERT(varchar(max),@xTimeStamp) +
									'</Parametri>')
						
																
				SET @xParLog=CONVERT(XML,'<Parametri><LogPrima/><LogDopo/></Parametri>')										
				
				SET @xLogPrima=Convert(XML,'<DataSet></DataSet>')														
				
				SET @xTemp=
					(SELECT * FROM 
						(SELECT * FROM T_MovDiarioClinico
						 WHERE ID=@uIDDiarioClinico											) AS [Table]
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
									(SELECT * FROM T_MovDiarioClinico
									 WHERE ID=@uIDDiarioClinico														) AS [Table]
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