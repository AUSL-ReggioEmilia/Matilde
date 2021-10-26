CREATE PROCEDURE [dbo].[MSP_AggMovAllegati](@xParametri XML)
AS
BEGIN
		
	
											
	DECLARE @uIDAllegato AS UNIQUEIDENTIFIER	
	DECLARE @uIDPaziente AS UNIQUEIDENTIFIER	
	DECLARE @uIDEpisodio AS UNIQUEIDENTIFIER	
	DECLARE @uIDTrasferimento AS UNIQUEIDENTIFIER	
	DECLARE @dDataEvento AS DATETIME	
	DECLARE @dDataEventoUTC AS DATETIME
	DECLARE @sNumeroDocumento AS VARCHAR(50)
	DECLARE @sCodTipoAllegato AS VARCHAR(20)
	DECLARE @sCodStatoAllegato AS VARCHAR(20)
	DECLARE @sCodFormatoAllegato AS VARCHAR(20)

	DECLARE @sCodUA AS VARCHAR(20)
	DECLARE @sCodEntita AS VARCHAR(20)
	DECLARE @uIDFolder AS UNIQUEIDENTIFIER	
				
	DECLARE @txtDocumento VARCHAR(MAX)	
	DECLARE @sEstensione AS VARCHAR(10)	
	DECLARE @txtTestoRTF VARCHAR(MAX)		
	DECLARE @txtNotaRTF VARCHAR(MAX)
	DECLARE @txtTestoTXT VARCHAR(MAX)
	DECLARE @txtNotaTXT VARCHAR(MAX)
	DECLARE @txtNomeFile VARCHAR(MAX)		
	
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
	
		SET @sSQL='UPDATE T_MovAllegati ' + CHAR(13) + CHAR(10) +
			  'SET '  				  						
			  					
	SET @sSET =''
	
		IF @xParametri.exist('/Parametri/IDAllegato')=1
		BEGIN
			SET @sGUID=(SELECT TOP 1 ValoreParametro.IDAllegato.value('.','VARCHAR(50)')
							  FROM @xParametri.nodes('/Parametri/IDAllegato') as ValoreParametro(IDAllegato))
			IF 	ISNULL(@sGUID,'') <> '' 
					SET @uIDAllegato=CONVERT(UNIQUEIDENTIFIER,	@sGUID)	
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
		
		IF @xParametri.exist('/Parametri/NumeroDocumento')=1
	BEGIN
		SET @sNumeroDocumento=(SELECT TOP 1 ValoreParametro.NumeroDocumento.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/NumeroDocumento') as ValoreParametro(NumeroDocumento))	
					  
		IF @sNumeroDocumento <> ''
				SET	@sSET= @sSET + ',NumeroDocumento=''' + @sNumeroDocumento +''''	+ CHAR(13) + CHAR(10)				
			ELSE
				SET	@sSET= @sSET + ',NumeroDocumento=NULL'	+ CHAR(13) + CHAR(10)		
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
	
		IF @xParametri.exist('/Parametri/TestoRTF')=1
	BEGIN	
		SET @txtTestoRTF=(SELECT TOP 1 ValoreParametro.TestoRTF.value('.','VARCHAR(MAX)')
				  FROM @xParametri.nodes('/Parametri/TestoRTF') as ValoreParametro(TestoRTF))					  

		IF @txtTestoRTF <> ''
			SET	@sSET= @sSET +',TestoRTF=
										CONVERT(varchar(max),
												CAST(N'''' AS xml).value(''xs:base64Binary("' + @txtTestoRTF
												+ '")'', ''varbinary(max)'') 
											)'  + CHAR(13) + CHAR(10)	
		ELSE
			SET	@sSET= @sSET +',TestoRTF=NULL '	+ CHAR(13) + CHAR(10)														
	END	
	
		IF @xParametri.exist('/Parametri/TestoRTF')=1
	BEGIN	
		SET @txtNotaRTF=(SELECT TOP 1 ValoreParametro.NotaRTF.value('.','VARCHAR(MAX)')
				  FROM @xParametri.nodes('/Parametri/NotaRTF') as ValoreParametro(NotaRTF))					  

		IF @txtNotaRTF <> ''
			SET	@sSET= @sSET +',NotaRTF=
										CONVERT(varchar(max),
												CAST(N'''' AS xml).value(''xs:base64Binary("' + @txtNotaRTF
												+ '")'', ''varbinary(max)'') 
											)'  + CHAR(13) + CHAR(10)	
		ELSE
			SET	@sSET= @sSET +',NotaRTF=NULL '	+ CHAR(13) + CHAR(10)														
	END	
	
		IF @xParametri.exist('/Parametri/TestoTXT')=1
	BEGIN	
		SET @txtTestoTXT=(SELECT TOP 1 ValoreParametro.TestoTXT.value('.','VARCHAR(MAX)')
					  FROM @xParametri.nodes('/Parametri/TestoTXT') as ValoreParametro(TestoTXT))	
				  				  
		IF ISNULL(@txtTestoTXT,'') <> ''
				SET	@sSET= @sSET +',TestoTXT =
											CONVERT(varchar(max),
													CAST(N'''' AS xml).value(''xs:base64Binary("' + @txtTestoTXT
													+ '")'', ''varbinary(max)'') 
												)'  + CHAR(13) + CHAR(10)	
		ELSE
				SET	@sSET= @sSET +',TestoTXT=NULL '	+ CHAR(13) + CHAR(10)																
	END	

		IF @xParametri.exist('/Parametri/NotaTXT')=1
	BEGIN	
		SET @txtNotaTXT=(SELECT TOP 1 ValoreParametro.NotaTXT.value('.','VARCHAR(MAX)')
					  FROM @xParametri.nodes('/Parametri/NotaTXT') as ValoreParametro(NotaTXT))	
				  				  
		IF ISNULL(@txtNotaTXT,'') <> ''
				SET	@sSET= @sSET +',NotaTXT =
											CONVERT(varchar(max),
													CAST(N'''' AS xml).value(''xs:base64Binary("' + @txtNotaTXT
													+ '")'', ''varbinary(max)'') 
												)'  + CHAR(13) + CHAR(10)	
		ELSE
				SET	@sSET= @sSET +',NotaTXT=NULL '	+ CHAR(13) + CHAR(10)																
	END	

		IF @xParametri.exist('/Parametri/CodFormatoAllegato')=1
	BEGIN
		SET @sCodFormatoAllegato=(SELECT TOP 1 ValoreParametro.CodFormatoAllegato.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodFormatoAllegato') as ValoreParametro(CodFormatoAllegato))	
					  
		IF @sCodFormatoAllegato <> ''
				SET	@sSET= @sSET + ',CodFormatoAllegato=''' + @sCodFormatoAllegato +''''	+ CHAR(13) + CHAR(10)				
			ELSE
				SET	@sSET= @sSET + ',CodFormatoAllegato=NULL'	+ CHAR(13) + CHAR(10)		
	END	


		IF @xParametri.exist('/Parametri/CodTipoAllegato')=1
	BEGIN
		SET @sCodTipoAllegato=(SELECT TOP 1 ValoreParametro.CodTipoAllegato.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodTipoAllegato') as ValoreParametro(CodTipoAllegato))	
					  
		IF @sCodTipoAllegato <> ''
				SET	@sSET= @sSET + ',CodTipoAllegato=''' + @sCodTipoAllegato +''''	+ CHAR(13) + CHAR(10)				
			ELSE
				SET	@sSET= @sSET + ',CodTipoAllegato=NULL'	+ CHAR(13) + CHAR(10)		
	END	


		IF @xParametri.exist('/Parametri/CodStatoAllegato')=1
	BEGIN
		SET @sCodStatoAllegato=(SELECT TOP 1 ValoreParametro.CodStatoAllegato.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodStatoAllegato') as ValoreParametro(CodStatoAllegato))	
					  
		IF @sCodStatoAllegato <> ''
				SET	@sSET= @sSET + ',CodStatoAllegato=''' + @sCodStatoAllegato +''''	+ CHAR(13) + CHAR(10)				
			ELSE
				SET	@sSET= @sSET + ',CodStatoAllegato=NULL'	+ CHAR(13) + CHAR(10)		
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

		IF @xParametri.exist('/Parametri/CodEntita')=1
	BEGIN
		SET @sCodEntita=(SELECT TOP 1 ValoreParametro.CodEntita.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodEntita') as ValoreParametro(CodEntita))	
					  
		IF @sCodEntita <> ''
				SET	@sSET= @sSET + ',CodEntita=''' + @sCodEntita +''''	+ CHAR(13) + CHAR(10)				
			ELSE
				SET	@sSET= @sSET + ',CodEntita=NULL'	+ CHAR(13) + CHAR(10)		
	END	

		IF @xParametri.exist('/Parametri/IDFolder')=1
		BEGIN
			SET @sGUID=(SELECT TOP 1 ValoreParametro.IDFolder.value('.','VARCHAR(50)')
							  FROM @xParametri.nodes('/Parametri/IDFolder') as ValoreParametro(IDFolder))
							  
				IF 	ISNULL(@sGUID,'') <> '' 
				BEGIN
					SET @uIDFolder=CONVERT(UNIQUEIDENTIFIER,	@sGUID)		
					SET	@sSET= @sSET + ',IDFolder=''' + convert(VARCHAR(50),@uIDFolder) + ''''	+ CHAR(13) + CHAR(10)	
				END
				ELSE
					SET	@sSET= @sSET + ',IDFolder=NULL'	+ CHAR(13) + CHAR(10)									  		
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
								  ,IDPaziente
								  ,IDEpisodio
								  ,IDTrasferimento
								  ,NumeroDocumento
								  ,DataEvento
								  ,DataEventoUTC
								  ,TestoRTF
								  ,NotaRTF
								  ,TestoTXT
								  ,NotaTXT
								  ,CodFormatoAllegato
								  ,CodTipoAllegato
								  ,CodStatoAllegato
								 								  ,NomeFile
								  ,Estensione
								  ,CodUtenteRilevazione
								  ,CodUtenteUltimaModifica
								  ,DataUltimaModifica
								  ,DataUltimaModificaUTC
								  ,CodUA
								  ,CodEntita
								  ,IDFolder 
						 FROM T_MovAllegati
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
								  ,IDPaziente
								  ,IDEpisodio
								  ,IDTrasferimento
								  ,NumeroDocumento
								  ,DataEvento
								  ,DataEventoUTC
								  ,TestoRTF
								  ,NotaRTF
								  ,TestoTXT
								  ,NotaTXT
								  ,CodFormatoAllegato
								  ,CodTipoAllegato
								  ,CodStatoAllegato
								 								  ,NomeFile
								  ,Estensione
								  ,CodUtenteRilevazione
								  ,CodUtenteUltimaModifica
								  ,DataUltimaModifica
								  ,DataUltimaModificaUTC 
								   ,CodUA
								  ,CodEntita
								  ,IDFolder
							 FROM T_MovAllegati
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