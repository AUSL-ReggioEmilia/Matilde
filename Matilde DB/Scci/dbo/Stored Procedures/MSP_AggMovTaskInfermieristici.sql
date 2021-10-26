CREATE PROCEDURE [dbo].[MSP_AggMovTaskInfermieristici](@xParametri XML)
AS
BEGIN
		
	
							
												
								
	
	DECLARE @uIDTaskInfermieristico AS UNIQUEIDENTIFIER	
	DECLARE @uIDEpisodio AS UNIQUEIDENTIFIER	
	DECLARE @uIDTrasferimento AS UNIQUEIDENTIFIER	
		
	DECLARE @dDataEvento AS DATETIME	
	DECLARE @dDataEventoUTC AS DATETIME
	
	DECLARE @sCodSistema AS VARCHAR(20)
	DECLARE @sIDSistema AS VARCHAR(50)
	DECLARE @sIDGruppo AS VARCHAR(50)	
	DECLARE @sCodTipoTaskInfermieristico AS VARCHAR(20)
	DECLARE @sCodStatoTaskInfermieristico AS VARCHAR(20)
	DECLARE @sCodTipoRegistrazione AS VARCHAR(20)
	DECLARE @sCodProtocollo AS VARCHAR(20)
	DECLARE @sCodProtocolloTempo AS VARCHAR(20)
	DECLARE @sSottoclasse AS VARCHAR(512)	
	
	DECLARE @dDataProgrammata AS DATETIME	
	DECLARE @dDataProgrammataUTC AS DATETIME
	
	DECLARE @dDataErogazione AS DATETIME
	DECLARE @dDataErogazioneUTC AS DATETIME	
	DECLARE @txtNote AS VARCHAR(MAX)
	DECLARE @txtDescrizioneFUT AS VARCHAR(MAX)
	DECLARE @txtSottoClasse AS VARCHAR(MAX)
	DECLARE @sCodUtenteRilevazione AS VARCHAR(100)					
	DECLARE @sCodUtenteUltimaModifica AS VARCHAR(100)					
	DECLARE @txtPosologiaEffettiva AS VARCHAR(MAX)
	DECLARE @txtAlert AS VARCHAR(MAX)
	DECLARE @txtBarcode AS VARCHAR(MAX)
	
		DECLARE @sGUID AS VARCHAR(50)
	DECLARE @uGUID AS UNIQUEIDENTIFIER
	DECLARE @sDataTmp AS VARCHAR(20)	
	DECLARE @sTmp AS VARCHAR(MAX)	
	
	DECLARE @sSQL AS VARCHAR(MAX)
	DECLARE @sSET AS VARCHAR(MAX)
	DECLARE @sWHERE AS VARCHAR(MAX)
	
	    DECLARE @xTimeStampBase AS XML	
	DECLARE @xTimeStamp AS XML	
	DECLARE @sCodAzione VARCHAR(20)
		
		DECLARE @xSchedaMovimento AS XML
			
		DECLARE @xLogPrima AS XML
	DECLARE @xLogDopo AS XML
	DECLARE @xTemp AS XML
	DECLARE @xParLog AS XML
	
		SET @sSQL='UPDATE T_MovTaskInfermieristici ' + CHAR(13) + CHAR(10) +
			  'SET '  				  						
			
	SET @sSET =''
				
		IF @xParametri.exist('/Parametri/IDTaskInfermieristico')=1
		BEGIN
			SET @sGUID=(SELECT TOP 1 ValoreParametro.IDTaskInfermieristico.value('.','VARCHAR(50)')
							  FROM @xParametri.nodes('/Parametri/IDTaskInfermieristico') as ValoreParametro(IDTaskInfermieristico))
			IF 	ISNULL(@sGUID,'') <> '' 
					SET @uIDTaskInfermieristico=CONVERT(UNIQUEIDENTIFIER,	@sGUID)	
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
	
		IF @xParametri.exist('/Parametri/CodSistema')=1
	BEGIN
		SET @sCodSistema=(SELECT TOP 1 ValoreParametro.CodSistema.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodSistema') as ValoreParametro(CodSistema))	
					  
		IF @sCodSistema <> ''
				SET	@sSET= @sSET + ',CodSistema=''' + @sCodSistema +''''	+ CHAR(13) + CHAR(10)				
			ELSE
				SET	@sSET= @sSET + ',CodSistema=NULL'	+ CHAR(13) + CHAR(10)		
	END	
	
		IF @xParametri.exist('/Parametri/IDSistema')=1
	BEGIN
		SET @sIDSistema=(SELECT TOP 1 ValoreParametro.IDSistema.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDSistema') as ValoreParametro(IDSistema))	
					  
		IF @sIDSistema <> ''
				SET	@sSET= @sSET + ',IDSistema=''' + @sIDSistema +''''	+ CHAR(13) + CHAR(10)				
			ELSE
				SET	@sSET= @sSET + ',IDSistema=NULL'	+ CHAR(13) + CHAR(10)		
	END	
	
		IF @xParametri.exist('/Parametri/IDGruppo')=1
	BEGIN
		SET @sIDGruppo=(SELECT TOP 1 ValoreParametro.IDGruppo	.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDGruppo') as ValoreParametro(IDGruppo	))	
					  
		IF @sIDGruppo <> ''
				SET	@sSET= @sSET + ',IDGruppo=''' + @sIDGruppo +''''	+ CHAR(13) + CHAR(10)				
			ELSE
				SET	@sSET= @sSET + ',IDGruppo=NULL'	+ CHAR(13) + CHAR(10)		
	END	
	
		IF @xParametri.exist('/Parametri/CodTipoTaskInfermieristico')=1
	BEGIN
		SET @sCodTipoTaskInfermieristico=(SELECT TOP 1 ValoreParametro.CodTipoTaskInfermieristico.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodTipoTaskInfermieristico') as ValoreParametro(CodTipoTaskInfermieristico))	
					  
		IF @sCodTipoTaskInfermieristico <> ''
				SET	@sSET= @sSET + ',CodTipoTaskInfermieristico=''' + @sCodTipoTaskInfermieristico +''''	+ CHAR(13) + CHAR(10)				
			ELSE
				SET	@sSET= @sSET + ',CodTipoTaskInfermieristico=NULL'	+ CHAR(13) + CHAR(10)		
	END				
	
		IF @xParametri.exist('/Parametri/CodStatoTaskInfermieristico')=1
	BEGIN
		SET @sCodStatoTaskInfermieristico=(SELECT TOP 1 ValoreParametro.CodStatoTaskInfermieristico.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodStatoTaskInfermieristico') as ValoreParametro(CodStatoTaskInfermieristico))	
					  
		IF @sCodStatoTaskInfermieristico <> ''
				SET	@sSET= @sSET + ',CodStatoTaskInfermieristico=''' + @sCodStatoTaskInfermieristico +''''	+ CHAR(13) + CHAR(10)				
			ELSE
				SET	@sSET= @sSET + ',CodStatoTaskInfermieristico=NULL'	+ CHAR(13) + CHAR(10)		
	END	
		
	
		IF @xParametri.exist('/Parametri/CodTipoRegistrazione')=1
	BEGIN
		SET @sCodTipoRegistrazione=(SELECT TOP 1 ValoreParametro.CodTipoRegistrazione.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodTipoRegistrazione') as ValoreParametro(CodTipoRegistrazione))	
					  
		IF @sCodTipoRegistrazione <> ''
				SET	@sSET= @sSET + ',CodTipoRegistrazione=''' + @sCodTipoRegistrazione +''''	+ CHAR(13) + CHAR(10)				
			ELSE
				SET	@sSET= @sSET + ',CodTipoRegistrazione=NULL'	+ CHAR(13) + CHAR(10)		
	END	
		
		IF @xParametri.exist('/Parametri/CodProtocollo')=1
	BEGIN
		SET  @sCodProtocollo=(SELECT TOP 1 ValoreParametro.CodProtocollo.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodProtocollo') as ValoreParametro(CodProtocollo))	
					  
		IF  @sCodProtocollo <> ''
				SET	@sSET= @sSET + ',CodProtocollo=''' +  @sCodProtocollo +''''	+ CHAR(13) + CHAR(10)				
			ELSE
				SET	@sSET= @sSET + ',CodProtocollo=NULL'	+ CHAR(13) + CHAR(10)		
	END	
	
		IF @xParametri.exist('/Parametri/CodProtocolloTempo')=1
	BEGIN
		SET  @sCodProtocolloTempo=(SELECT TOP 1 ValoreParametro.CodProtocolloTempo.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodProtocolloTempo') as ValoreParametro(CodProtocolloTempo))	
					  
		IF  @sCodProtocolloTempo <> ''
				SET	@sSET= @sSET + ',CodProtocolloTempo=''' +  @sCodProtocolloTempo +''''	+ CHAR(13) + CHAR(10)				
			ELSE
				SET	@sSET= @sSET + ',CodProtocolloTempo=NULL'	+ CHAR(13) + CHAR(10)		
	END	
	
		IF @xParametri.exist('/Parametri/Sottoclasse')=1
	BEGIN	
		SET @txtSottoClasse=(SELECT TOP 1 ValoreParametro.Sottoclasse.value('.','VARCHAR(MAX)')
				  FROM @xParametri.nodes('/Parametri/Sottoclasse') as ValoreParametro(Sottoclasse))					  

		IF @txtSottoClasse <> ''
			SET	@sSET= @sSET +',Sottoclasse=
										CONVERT(varchar(max),
												CAST(N'''' AS xml).value(''xs:base64Binary("' + @txtSottoClasse
												+ '")'', ''varbinary(max)'') 
											)'  + CHAR(13) + CHAR(10)	
		ELSE
			SET	@sSET= @sSET +',Sottoclasse=NULL '	+ CHAR(13) + CHAR(10)														
	END	
	
		IF @xParametri.exist('/Parametri/Note')=1
	BEGIN	
		SET @txtNote=(SELECT TOP 1 ValoreParametro.Note.value('.','VARCHAR(MAX)')
				  FROM @xParametri.nodes('/Parametri/Note') as ValoreParametro(Note))					  

		IF @txtNote <> ''
			SET	@sSET= @sSET +',Note=
										CONVERT(varchar(max),
												CAST(N'''' AS xml).value(''xs:base64Binary("' + @txtNote
												+ '")'', ''varbinary(max)'') 
											)'  + CHAR(13) + CHAR(10)	
		ELSE
			SET	@sSET= @sSET +',Note=NULL '	+ CHAR(13) + CHAR(10)														
	END	
	
		IF @xParametri.exist('/Parametri/DescrizioneFUT')=1
	BEGIN	
		SET @txtDescrizioneFUT=(SELECT TOP 1 ValoreParametro.DescrizioneFUT.value('.','VARCHAR(MAX)')
				  FROM @xParametri.nodes('/Parametri/DescrizioneFUT') as ValoreParametro(DescrizioneFUT))					  

		IF @txtDescrizioneFUT <> ''
			SET	@sSET= @sSET +',DescrizioneFUT=
										CONVERT(varchar(max),
												CAST(N'''' AS xml).value(''xs:base64Binary("' + @txtDescrizioneFUT
												+ '")'', ''varbinary(max)'') 
											)'  + CHAR(13) + CHAR(10)	
		ELSE
			SET	@sSET= @sSET +',DescrizioneFUT=NULL '	+ CHAR(13) + CHAR(10)														
	END	
								  

		IF @xParametri.exist('/Parametri/DataProgrammata')=1
	BEGIN	
		SET @sDataTmp=(SELECT TOP 1 ValoreParametro.DataProgrammata.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/DataProgrammata') as ValoreParametro(DataProgrammata))					  
		SET @sDataTmp=ISNULL(@sDataTmp,'')
	
		IF @sDataTmp<> '' 
		BEGIN			
						SET @sDataTmp= SUBSTRING(@sDataTmp,4,2) + '-' 
						+ LEFT(@sDataTmp,2) + '-' +
						+ SUBSTRING(@sDataTmp,7,4) +
						' ' + RIGHT(@sDataTmp,5)
			IF ISDATE(@sDataTmp)=1
				SET	@dDataProgrammata=CONVERT(DATETIME,@sDataTmp,120)						
			ELSE
				SET	@dDataProgrammata =NULL			
		END
		
		IF @dDataProgrammata IS NOT NULL		
			SET	@sSET= @sSET + ',DataProgrammata=CONVERT(DateTime,''' + CONVERT(VARCHAR(20),@dDataProgrammata,120) + ''',120)' + CHAR(13) + CHAR(10)		
		ELSE
			SET	@sSET= @sSET + ',DataProgrammata=NULL ' + CHAR(13) + CHAR(10)		
	END	
	
		IF @xParametri.exist('/Parametri/DataProgrammataUTC')=1
	BEGIN	
		SET @sDataTmp=(SELECT TOP 1 ValoreParametro.DataProgrammataUTC.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/DataProgrammataUTC') as ValoreParametro(DataProgrammataUTC))					  
		SET @sDataTmp=ISNULL(@sDataTmp,'')
	
		IF @sDataTmp<> '' 
		BEGIN			
						SET @sDataTmp= SUBSTRING(@sDataTmp,4,2) + '-' 
						+ LEFT(@sDataTmp,2) + '-' +
						+ SUBSTRING(@sDataTmp,7,4) +
						' ' + RIGHT(@sDataTmp,5)
			IF ISDATE(@sDataTmp)=1
				SET	@dDataProgrammataUTC=CONVERT(DATETIME,@sDataTmp,120)						
			ELSE
				SET	@dDataProgrammataUTC =NULL			
		END
		
		IF @dDataProgrammataUTC IS NOT NULL		
			SET	@sSET= @sSET + ',DataProgrammataUTC=CONVERT(DateTime,''' + CONVERT(VARCHAR(20),@dDataProgrammataUTC,120) + ''',120)' + CHAR(13) + CHAR(10)		
		ELSE
			SET	@sSET= @sSET + ',DataProgrammataUTC=NULL ' + CHAR(13) + CHAR(10)		
	END	
		
	
				
		IF @xParametri.exist('/Parametri/DataErogazione')=1
	BEGIN	
		SET @sDataTmp=(SELECT TOP 1 ValoreParametro.DataErogazione.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/DataErogazione') as ValoreParametro(DataErogazione))					  
		SET @sDataTmp=ISNULL(@sDataTmp,'')
	
		IF @sDataTmp<> '' 
		BEGIN			
						SET @sDataTmp= SUBSTRING(@sDataTmp,4,2) + '-' 
						+ LEFT(@sDataTmp,2) + '-' +
						+ SUBSTRING(@sDataTmp,7,4) +
						' ' + RIGHT(@sDataTmp,5)
			IF ISDATE(@sDataTmp)=1
				SET	@dDataErogazione=CONVERT(DATETIME,@sDataTmp,120)						
			ELSE
				SET	@dDataErogazione =NULL			
		END				
	END	
	
		IF @xParametri.exist('/Parametri/DataErogazioneUTC')=1
	BEGIN	
		SET @sDataTmp=(SELECT TOP 1 ValoreParametro.DataErogazioneUTC.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/DataErogazioneUTC') as ValoreParametro(DataErogazioneUTC))					  
		SET @sDataTmp=ISNULL(@sDataTmp,'')
	
		IF @sDataTmp<> '' 
		BEGIN			
						SET @sDataTmp= SUBSTRING(@sDataTmp,4,2) + '-' 
						+ LEFT(@sDataTmp,2) + '-' +
						+ SUBSTRING(@sDataTmp,7,4) +
						' ' + RIGHT(@sDataTmp,5)
			IF ISDATE(@sDataTmp)=1
				SET	@dDataErogazioneUTC=CONVERT(DATETIME,@sDataTmp,120)						
			ELSE
				SET	@dDataErogazioneUTC =NULL			
		END
	END							

		SET @sCodAzione=(SELECT TOP 1 ValoreParametro.CodAzione.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/TimeStamp/CodAzione') as ValoreParametro(CodAzione))	
					  
				IF @sCodStatoTaskInfermieristico IN ('ER','AN','TR') AND @dDataErogazione IS NULL 
		BEGIN	
			SET @dDataErogazione=GETDATE()
			SET @dDataErogazioneUTC=GETUTCDATE()
		END

	IF @sCodStatoTaskInfermieristico IN ('PR','IC') AND @dDataErogazione IS NOT NULL 
		BEGIN	
			SET @dDataErogazione=NULL
			SET @dDataErogazioneUTC=NULL
		END
		
	IF @dDataErogazione IS NOT NULL		
			SET	@sSET= @sSET + ',DataErogazione=CONVERT(DateTime,''' + CONVERT(VARCHAR(20),@dDataErogazione,120) + ''',120)' + CHAR(13) + CHAR(10)		
		ELSE
			SET	@sSET= @sSET + ',DataErogazione=NULL ' + CHAR(13) + CHAR(10)		
			
	IF @dDataErogazioneUTC IS NOT NULL		
			SET	@sSET= @sSET + ',DataErogazioneUTC=CONVERT(DateTime,''' + CONVERT(VARCHAR(20),@dDataErogazioneUTC,120) + ''',120)' + CHAR(13) + CHAR(10)		
		ELSE
			SET	@sSET= @sSET + ',DataErogazioneUTC=NULL ' + CHAR(13) + CHAR(10)	
			
				
						
		IF @xParametri.exist('/Parametri/CodUtenteRilevazione')=1
	BEGIN
		SET  @sCodUtenteRilevazione=(SELECT TOP 1 ValoreParametro.CodUtenteRilevazione.value('.','VARCHAR(100)')
					  FROM @xParametri.nodes('/Parametri/CodUtenteRilevazione') as ValoreParametro(CodUtenteRilevazione))	
					  
		IF  @sCodUtenteRilevazione <> ''
				SET	@sSET= @sSET + ',CodUtenteRilevazione=''' +  @sCodUtenteRilevazione +''''	+ CHAR(13) + CHAR(10)				
			ELSE
				SET	@sSET= @sSET + ',CodUtenteRilevazione=NULL'	+ CHAR(13) + CHAR(10)		
	END	
				
		SET @sCodUtenteUltimaModifica=(SELECT TOP 1 ValoreParametro.CodUtenteUltimaModifica.value('.','VARCHAR(100)')
					  FROM @xParametri.nodes('/Parametri/TimeStamp/CodLogin') as ValoreParametro(CodUtenteUltimaModifica))	
	SET @sCodUtenteUltimaModifica=ISNULL(@sCodUtenteUltimaModifica,'')
		
	IF @sCodUtenteUltimaModifica <> ''
		SET	@sSET= @sSET + ',CodUtenteUltimaModifica=''' + @sCodUtenteUltimaModifica + '''' + CHAR(13) + CHAR(10)		
	ELSE
		SET	@sSET= @sSET + ',CodUtenteUltimaModifica=NULL' + CHAR(13) + CHAR(10)	
	
		SET	@sSET= @sSET + ',DataUltimaModifica=getdate() ' + CHAR(13) + CHAR(10)	
		
		SET	@sSET= @sSET + ',DataUltimaModificaUTC=getUTCdate() ' + CHAR(13) + CHAR(10)
	
		IF @xParametri.exist('/Parametri/PosologiaEffettiva')=1
	BEGIN	
		SET @txtPosologiaEffettiva=(SELECT TOP 1 ValoreParametro.PosologiaEffettiva.value('.','VARCHAR(MAX)')
				  FROM @xParametri.nodes('/Parametri/PosologiaEffettiva') as ValoreParametro(PosologiaEffettiva))					  

		IF @txtPosologiaEffettiva <> ''
			SET	@sSET= @sSET +',PosologiaEffettiva=
										CONVERT(varchar(max),
												CAST(N'''' AS xml).value(''xs:base64Binary("' + @txtPosologiaEffettiva
												+ '")'', ''varbinary(max)'') 
											)'  + CHAR(13) + CHAR(10)	
		ELSE
			SET	@sSET= @sSET +',PosologiaEffettiva=NULL '	+ CHAR(13) + CHAR(10)														
	END	

		IF @xParametri.exist('/Parametri/Alert')=1
	BEGIN	
		SET @txtAlert=(SELECT TOP 1 ValoreParametro.Alert.value('.','VARCHAR(MAX)')
				  FROM @xParametri.nodes('/Parametri/Alert') as ValoreParametro(Alert))					  

		IF @txtAlert <> ''
			SET	@sSET= @sSET +',Alert=
										CONVERT(varchar(max),
												CAST(N'''' AS xml).value(''xs:base64Binary("' + @txtAlert
												+ '")'', ''varbinary(max)'') 
											)'  + CHAR(13) + CHAR(10)	
		ELSE
			SET	@sSET= @sSET +',Alert=NULL '	+ CHAR(13) + CHAR(10)														
	END	
	
		IF @xParametri.exist('/Parametri/Barcode')=1
	BEGIN	
		SET @txtBarcode=(SELECT TOP 1 ValoreParametro.Barcode.value('.','VARCHAR(MAX)')
				  FROM @xParametri.nodes('/Parametri/Barcode') as ValoreParametro(Barcode))					  

		IF @txtBarcode <> ''
			SET	@sSET= @sSET +',Barcode=
										CONVERT(varchar(max),
												CAST(N'''' AS xml).value(''xs:base64Binary("' + @txtBarcode
												+ '")'', ''varbinary(max)'') 
											)'  + CHAR(13) + CHAR(10)	
		ELSE
			SET	@sSET= @sSET +',Barcode=NULL '	+ CHAR(13) + CHAR(10)														
	END	
	    SET @xTimeStamp=(SELECT TOP 1 ValoreParametro.TS.query('.')
					  FROM @xParametri.nodes('/Parametri/TimeStamp') as ValoreParametro(TS))
	
		SET @xSchedaMovimento=(SELECT TOP 1 ValoreParametro.TS.query('.')
					  FROM @xParametri.nodes('/Parametri/SchedaMovimento') as ValoreParametro(TS))					
					  
					  							
				
					
	PRINT @sSQL
	
		IF LEFT(@sSET,1)=',' 
		SET @sSET=RIGHT(@sSET,LEN(@sSET)-1)
	
	IF @sSET <> ''		
		BEGIN
						
								SET @sWHERE =' WHERE ID=''' + convert(varchar(50),@uIDTaskInfermieristico) +''''
				
								SET @sSQL=	ISNULL(@sSQL,'') + CHAR(13) + CHAR(10) + 
						ISNULL(@sSET,'') + CHAR(13) + CHAR(10) + 
						ISNULL(@sWHERE,'')	
								
				
																				SET @xTimeStamp.modify('delete (/TimeStamp/IDEntita)[1]') 
						
				SET @xTimeStamp.modify('insert <IDEntita>{sql:variable("@uIDTaskInfermieristico")}</IDEntita> into (/TimeStamp)[1]')
	
								SET @xTimeStampBase=@xTimeStamp
			
				SET @xTimeStamp=CONVERT(XML,
									'<Parametri>' + CONVERT(varchar(max),@xTimeStamp) +
									'</Parametri>')
																
				SET @xParLog=CONVERT(XML,'<Parametri><LogPrima/><LogDopo/></Parametri>')										
				
				SET @xLogPrima=Convert(XML,'<DataSet></DataSet>')														
				
				SET @xTemp=
					(SELECT * FROM 
						(SELECT
							 *
						 FROM T_MovTaskInfermieristici
						 WHERE ID=@uIDTaskInfermieristico											) AS [Table]
					FOR XML AUTO, ELEMENTS)

				SET @xLogPrima.modify('insert sql:variable("@xTemp") as first into (/DataSet)[1]')
																			
				SET @xParLog.modify('insert sql:variable("@xLogPrima") as first into (/Parametri/LogPrima)[1]')
											
				BEGIN TRANSACTION
										PRINT @sSQL
					EXEC (@sSQL)
					
				IF @@ERROR=0 AND @@ROWCOUNT>0
					BEGIN
																		EXEC MSP_InsMovTimeStamp @xTimeStamp		
					END	
				IF @@ERROR = 0
					BEGIN
						COMMIT TRANSACTION
						
																								
						SET @xLogDopo=Convert(XML,'<DataSet></DataSet>')				
						
						SET @xTemp=
							(SELECT * FROM 
								(SELECT	*
								 FROM T_MovTaskInfermieristici
								 WHERE ID=@uIDTaskInfermieristico													) AS [Table]
							FOR XML AUTO, ELEMENTS)

						SET @xLogDopo.modify('insert sql:variable("@xTemp") as first into (/DataSet)[1]')																											
						SET @xParLog.modify('insert sql:variable("@xLogDopo") as first into (/Parametri/LogDopo)[1]')
						
												SET @xParLog.modify('insert sql:variable("@xTimeStampBase") as last into (/Parametri)[1]')
					
												SET @xParLog.modify('insert sql:variable("@xSchedaMovimento") as last into (/Parametri/LogDopo/DataSet)[1]')
												
					
												IF ISNULL(@sCodStatoTaskInfermieristico,'')='TR'  AND ISNULL(@sCodAzione,'')='ANN' 
							BEGIN								
								SET @xParLog.modify('delete (/Parametri/TimeStamp/CodAzione)[1]') 			
								SET @xParLog.modify('insert <CodAzione>TRA</CodAzione> into (/Parametri/TimeStamp)[1]')
						END
						EXEC MSP_InsMovLog @xParLog
						
											END	
				ELSE
					BEGIN
						ROLLBACK TRANSACTION	
											END	 
		END			
	RETURN 0
END