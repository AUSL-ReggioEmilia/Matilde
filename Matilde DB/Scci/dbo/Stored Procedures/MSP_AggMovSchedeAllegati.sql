CREATE PROCEDURE [dbo].[MSP_AggMovSchedeAllegati](@xParametri XML)
AS
BEGIN
		
	
	
			
								
	DECLARE @uIDAllegato AS UNIQUEIDENTIFIER	
	DECLARE @uIDScheda AS UNIQUEIDENTIFIER	
	DECLARE @sCodCampo AS VARCHAR(255)
	DECLARE @sCodSezione AS VARCHAR(255)
	DECLARE @nSequenza AS INTEGER
	DECLARE @sIDGruppo AS VARCHAR(50)
	DECLARE @txtAnteprima VARCHAR(MAX)			
	DECLARE @txtDocumento VARCHAR(MAX)	
	DECLARE @txtNomeFile VARCHAR(2000)
	DECLARE @sEstensione AS VARCHAR(10)		
	DECLARE @sDescrizioneAllegato AS VARCHAR(255)
	DECLARE @sDescrizioneCampo AS VARCHAR(255)
	DECLARE @sCodStatoAllegatoScheda AS VARCHAR(20)
	DECLARE @sCodFormatoAllegato AS VARCHAR(20)	
	DECLARE @dDataEvento AS DATETIME	
	DECLARE @dDataEventoUTC AS DATETIME
	
	DECLARE @sCodUtenteRilevazione AS VARCHAR(100)		
	
	DECLARE @sCodAzione AS VARCHAR(20)										
	DECLARE @sCodUtenteUltimaModifica AS VARCHAR(100)										
	
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
		
		SET @sSQL='UPDATE T_MovSchedeAllegati ' + CHAR(13) + CHAR(10) +
			  'SET '  				  						
			  					
	SET @sSET =''

		IF @xParametri.exist('/Parametri/IDAllegato')=1
		BEGIN
			SET @sGUID=(SELECT TOP 1 ValoreParametro.IDAllegato.value('.','VARCHAR(50)')
							  FROM @xParametri.nodes('/Parametri/IDAllegato') as ValoreParametro(IDAllegato))
			IF 	ISNULL(@sGUID,'') <> '' 
					SET @uIDAllegato=CONVERT(UNIQUEIDENTIFIER,	@sGUID)	
					END
	
		IF @xParametri.exist('/Parametri/IDScheda')=1
		BEGIN
			SET @sGUID=(SELECT TOP 1 ValoreParametro.IDScheda.value('.','VARCHAR(50)')
							  FROM @xParametri.nodes('/Parametri/IDScheda') as ValoreParametro(IDScheda))
			IF 	ISNULL(@sGUID,'') <> '' 
			BEGIN
					SET @uIDScheda=CONVERT(UNIQUEIDENTIFIER,	@sGUID)	
					SET	@sSET= @sSET + ',IdScheda=''' + convert(VARCHAR(50),@uIDScheda) +''''	+ CHAR(13) + CHAR(10)				
			END
			ELSE
				SET	@sSET= @sSET + ',IdScheda=NULL'	+ CHAR(13) + CHAR(10)
						
			
		END
					
		IF @xParametri.exist('/Parametri/CodCampo')=1
		BEGIN
			SET @sCodCampo=(SELECT TOP 1 ValoreParametro.CodCampo.value('.','VARCHAR(255)')
							  FROM @xParametri.nodes('/Parametri/CodCampo') as ValoreParametro(CodCampo))
			
			IF ISNULL(@sCodCampo,'') <> ''
			BEGIN
				SET	@sSET= @sSET + ',CodCampo=''' + @sCodCampo +''''	+ CHAR(13) + CHAR(10)	
			END											  		
		END	
						
		IF @xParametri.exist('/Parametri/CodSezione')=1
		BEGIN
			SET @sCodSezione=(SELECT TOP 1 ValoreParametro.CodSezione.value('.','VARCHAR(255)')
							  FROM @xParametri.nodes('/Parametri/CodSezione') as ValoreParametro(CodSezione))	
							  IF ISNULL(@sCodCampo,'') <> ''
							  
			IF ISNULL(@sCodSezione,'') <> ''
			BEGIN
				SET	@sSET= @sSET + ',CodSezione=''' + @sCodSezione +''''	+ CHAR(13) + CHAR(10)	
			END	
		END		
	
		IF @xParametri.exist('/Parametri/Sequenza')=1
		BEGIN
			SET @nSequenza=(SELECT TOP 1 ValoreParametro.Sequenza.value('.','INTEGER')
							  FROM @xParametri.nodes('/Parametri/Sequenza') as ValoreParametro(Sequenza))
							  
			IF @nSequenza IS NOT NULL
			BEGIN
				SET	@sSET= @sSET + ',Sequenza=' + CONVERT(VARCHAR(50),@nSequenza) + ''	+ CHAR(13) + CHAR(10)	
			END					  
		END		
					
		IF @xParametri.exist('/Parametri/IDGruppo')=1
		BEGIN
			SET @sIDGruppo=(SELECT TOP 1 ValoreParametro.IDGruppo.value('.','VARCHAR(50)')
							  FROM @xParametri.nodes('/Parametri/IDGruppo') as ValoreParametro(IDGruppo))
			
					END
		
	IF @xParametri.exist('/Parametri/Anteprima')=1
	BEGIN			
		SET @txtAnteprima=(SELECT TOP 1 ValoreParametro.Anteprima.value('.','VARCHAR(MAX)')
							FROM @xParametri.nodes('/Parametri/Anteprima') as ValoreParametro(Anteprima))					  

		IF @txtAnteprima <> ''
			SET	@sSET= @sSET +',Anteprima=
									CAST(N'''' AS xml).value(''xs:base64Binary("' + @txtAnteprima
									+ '")'', ''varbinary(max)'')
							 '  + CHAR(13) + CHAR(10)	
		ELSE
			SET	@sSET= @sSET +',Anteprima=NULL '	+ CHAR(13) + CHAR(10)																			
	END	
	
	
		IF @xParametri.exist('/Parametri/Documento')=1
	BEGIN			
		SET @txtDocumento=(SELECT TOP 1 ValoreParametro.Documento.value('.','VARCHAR(MAX)')
							FROM @xParametri.nodes('/Parametri/Documento') as ValoreParametro(Documento))					  

		IF @txtDocumento <> ''
			SET	@sSET= @sSET +',Documento=
									CAST(N'''' AS xml).value(''xs:base64Binary("' + @txtDocumento
									+ '")'', ''varbinary(max)'')
							 '  + CHAR(13) + CHAR(10)	
		ELSE
			SET	@sSET= @sSET +',Documento=NULL '	+ CHAR(13) + CHAR(10)																			
	END	

		IF @xParametri.exist('/Parametri/NomeFile')=1
	BEGIN	
		SET @txtNomeFile=(SELECT TOP 1 ValoreParametro.NomeFile.value('.','VARCHAR(MAX)')
				  FROM @xParametri.nodes('/Parametri/NomeFile') as ValoreParametro(NomeFile))					  

		IF @txtNomeFile <> ''
			SET	@sSET= @sSET +',NomeFile=
										CONVERT(varchar(max),
												CAST(N'''' AS xml).value(''xs:base64Binary("' + @txtNomeFile
												+ '")'', ''varbinary(max)'') 
											)'  + CHAR(13) + CHAR(10)	

		ELSE
			SET	@sSET= @sSET +',NomeFile=NULL '	+ CHAR(13) + CHAR(10)				
	END	
	
	
		IF @xParametri.exist('/Parametri/Estensione')=1
	BEGIN
		SET @sEstensione=(SELECT TOP 1 ValoreParametro.Estensione.value('.','VARCHAR(10)')
					  FROM @xParametri.nodes('/Parametri/Estensione') as ValoreParametro(Estensione))	
					  
		IF @sEstensione <> ''
				SET	@sSET= @sSET + ',Estensione=''' + @sEstensione +''''	+ CHAR(13) + CHAR(10)				
			ELSE
				SET	@sSET= @sSET + ',Estensione=NULL'	+ CHAR(13) + CHAR(10)		
	END	

	IF @xParametri.exist('/Parametri/DescrizioneAllegato')=1
	BEGIN
		SET @sDescrizioneAllegato=(SELECT TOP 1 ValoreParametro.DescrizioneAllegato.value('.','VARCHAR(255)')
					  FROM @xParametri.nodes('/Parametri/DescrizioneAllegato') as ValoreParametro(DescrizioneAllegato))	
					  
		IF @sDescrizioneAllegato <> ''
				SET	@sSET= @sSET + ',DescrizioneAllegato=''' + @sDescrizioneAllegato +''''	+ CHAR(13) + CHAR(10)				
			ELSE
				SET	@sSET= @sSET + ',DescrizioneAllegato=NULL'	+ CHAR(13) + CHAR(10)		
	END	

	IF @xParametri.exist('/Parametri/DescrizioneCampo')=1
	BEGIN
		SET @sDescrizioneCampo=(SELECT TOP 1 ValoreParametro.DescrizioneCampo.value('.','VARCHAR(255)')
					  FROM @xParametri.nodes('/Parametri/DescrizioneCampo') as ValoreParametro(DescrizioneCampo))	
					  
		IF @sDescrizioneCampo <> ''
				SET	@sSET= @sSET + ',DescrizioneCampo=''' + @sDescrizioneCampo +''''	+ CHAR(13) + CHAR(10)				
			ELSE
				SET	@sSET= @sSET + ',DescrizioneCampo=NULL'	+ CHAR(13) + CHAR(10)		
	END	



		IF @xParametri.exist('/Parametri/CodStatoAllegatoScheda')=1
	BEGIN
		SET @sCodStatoAllegatoScheda=(SELECT TOP 1 ValoreParametro.CodStatoAllegatoScheda.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodStatoAllegatoScheda') as ValoreParametro(CodStatoAllegatoScheda))	
					  
		IF @sCodStatoAllegatoScheda <> ''
				SET	@sSET= @sSET + ',CodStatoAllegatoScheda=''' + @sCodStatoAllegatoScheda +''''	+ CHAR(13) + CHAR(10)				
			ELSE
				SET	@sSET= @sSET + ',CodStatoAllegatoScheda=NULL'	+ CHAR(13) + CHAR(10)		
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

	
		SET @sCodUtenteUltimaModifica=(SELECT TOP 1 ValoreParametro.CodUtenteUltimaModifica.value('.','VARCHAR(100)')
					  FROM @xParametri.nodes('/Parametri/TimeStamp/CodLogin') as ValoreParametro(CodUtenteUltimaModifica))	
	SET @sCodUtenteUltimaModifica=ISNULL(@sCodUtenteUltimaModifica,'')
	
	IF @sCodUtenteUltimaModifica <> ''
		SET	@sSET= @sSET + ',CodUtenteUltimaModifica=''' + @sCodUtenteUltimaModifica + '''' + CHAR(13) + CHAR(10)		
	ELSE
		SET	@sSET= @sSET + ',CodUtenteUltimaModifica=NULL' + CHAR(13) + CHAR(10)	

		SET	@sSET= @sSET + ',DataUltimaModifica=GetDate()'	+ CHAR(13) + CHAR(10)	
		SET	@sSET= @sSET + ',DataUltimaModificaUTC=GetUTCDate()'	+ CHAR(13) + CHAR(10)					
	
	
						 
					   
	    SET @xTimeStamp=(SELECT TOP 1 ValoreParametro.TS.query('.')
					  FROM @xParametri.nodes('/Parametri/TimeStamp') as ValoreParametro(TS))
	

										
	
	
				
	
		IF LEFT(@sSET,1)=',' 
		SET @sSET=RIGHT(@sSET,LEN(@sSET)-1)
	
	IF @sSET <> ''		
		BEGIN
						
								SET @sWHERE =' WHERE ID=''' + convert(varchar(50),@uIDAllegato) +''''
								
								SET @sSQL=	ISNULL(@sSQL,'') + CHAR(13) + CHAR(10) + 
						ISNULL(@sSET,'') + CHAR(13) + CHAR(10) + 
						ISNULL(@sWHERE,'')	
																				SET @xTimeStamp.modify('delete (/TimeStamp/IDEntita)[1]') 
						
				SET @xTimeStamp.modify('insert <IDEntita>{sql:variable("@uIDAllegato")}</IDEntita> into (/TimeStamp)[1]')

								SET @xTimeStampBase=@xTimeStamp
		
	
				SET @xTimeStamp=CONVERT(XML,
									'<Parametri>' + CONVERT(varchar(max),@xTimeStamp) +
									'</Parametri>')

																
				SET @xParLog=CONVERT(XML,'<Parametri><LogPrima/><LogDopo/></Parametri>')										
				
				SET @xLogPrima=Convert(XML,'<DataSet></DataSet>')														
				
				SET @xTemp=
					(SELECT * FROM 
						(SELECT 
								ID
							  ,IDNum
							  ,IDScheda
							  ,CodCampo
							  ,IDGruppo
							  							  							  ,NomeFile
							  ,Estensione
							  ,DescrizioneAllegato
							  ,DescrizioneCampo
							  ,CodTipoAllegatoScheda
							  ,CodStatoAllegatoScheda
							  ,DataEvento
							  ,DataEventoUTC
							  ,CodUtenteRilevazione
							  ,CodUtenteUltimaModifica
							  ,DataRilevazione
							  ,DataRilevazioneUTC
							  ,DataUltimaModifica
							  ,DataUltimaModificaUTC
							  ,CodSezione	
							  ,Sequenza
						 FROM T_MovSchedeAllegati
						 WHERE ID=@uIDAllegato											) AS [Table]
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
							(SELECT 
								ID
							  ,IDNum
							  ,IDScheda
							  ,CodCampo
							  ,IDGruppo
							 							 							  ,NomeFile
							  ,Estensione
							  ,DescrizioneAllegato
							  ,DescrizioneCampo
							  ,CodTipoAllegatoScheda
							  ,CodStatoAllegatoScheda
							  ,DataEvento
							  ,DataEventoUTC
							  ,CodUtenteRilevazione
							  ,CodUtenteUltimaModifica
							  ,DataRilevazione
							  ,DataRilevazioneUTC
							  ,DataUltimaModifica
							  ,DataUltimaModificaUTC
							  ,CodSezione
							  ,Sequenza
							FROM T_MovSchedeAllegati
							 WHERE ID=@uIDAllegato												) AS [Table]
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