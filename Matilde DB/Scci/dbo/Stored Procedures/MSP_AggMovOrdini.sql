
CREATE PROCEDURE [dbo].[MSP_AggMovOrdini](@xParametri XML)
AS
BEGIN
		
	
			

								
		DECLARE @uIDOrdine AS UNIQUEIDENTIFIER
	DECLARE @uIDPaziente AS UNIQUEIDENTIFIER	
	DECLARE @uIDEpisodio AS UNIQUEIDENTIFIER	
	DECLARE @uIDTrasferimento AS UNIQUEIDENTIFIER		
	DECLARE @sIDOrdineOE AS VARCHAR(50)	
	DECLARE @sNumeroOrdineOE AS VARCHAR(50)
	DECLARE @xXMLOE AS XML	
	DECLARE @sEroganti AS VARCHAR(MAX)
	DECLARE @sPrestazioni AS VARCHAR(MAX)
	DECLARE @sCodStatoOrdine AS VARCHAR(20)
	DECLARE @sCodUtenteInserimento AS VARCHAR(100)	
	DECLARE @sCodUtenteUltimaModifica AS VARCHAR(100)
	DECLARE @sCodUtenteInoltro AS VARCHAR(100)
	DECLARE @sCodRuolo AS VARCHAR(20)			
	DECLARE @dDataInserimento AS DATETIME	
	DECLARE @dDataInserimentoUTC AS DATETIME	
	DECLARE @dDataUltimaModifica AS DATETIME	
	DECLARE @dDataUltimaModificaUTC AS DATETIME	
	DECLARE @dDataInoltro AS DATETIME	
	DECLARE @dDataInoltroUTC AS DATETIME
	DECLARE @dDataProgrammazioneOE AS DATETIME
	DECLARE @dDataProgrammazioneOEUTC AS DATETIME	
	DECLARE @sCodAzione AS VARCHAR(20)
	DECLARE @sCodUAUltimaModifica AS VARCHAR(20)
	DECLARE @sCodPriorita AS VARCHAR(20)
	DECLARE @sCodSistema AS VARCHAR(20)
	DECLARE @sIDSistema AS VARCHAR(50)
	DECLARE @sIDGruppo AS VARCHAR(50)
	DECLARE @sInfoSistema AS VARCHAR(50)
	DECLARE @xStrutturaDatiAccessori AS XML
	DECLARE @xDatiDatiAccessori AS XML
	DECLARE @xLayoutDatiAccessori AS XML
					
		DECLARE @binXMLOE VARBINARY(MAX)
	DECLARE @txtXMLOE VARCHAR(MAX)
	
	DECLARE @sGUID AS VARCHAR(50)	
	DECLARE @sDataTmp AS VARCHAR(20)	

	DECLARE @binTmp VARBINARY(MAX)
	DECLARE @txtTmp VARCHAR(MAX)
	
	DECLARE @sSQL AS VARCHAR(MAX)
	DECLARE @sSET AS VARCHAR(MAX)
	DECLARE @sWHERE AS VARCHAR(MAX)
	
	    DECLARE @xTimeStampBase AS XML	
	DECLARE @xTimeStamp AS XML	
			
		DECLARE @xLogPrima AS XML
	DECLARE @xLogDopo AS XML
	DECLARE @xTemp AS XML
	DECLARE @xParLog AS XML


		SET @sSQL='UPDATE T_MovOrdini ' + CHAR(13) + CHAR(10) +
			  'SET '  				  						
			
	SET @sSET =''

				
		IF @xParametri.exist('/Parametri/IDOrdine')=1
		BEGIN
			SET @sGUID=(SELECT TOP 1 ValoreParametro.IDOrdine.value('.','VARCHAR(50)')
							  FROM @xParametri.nodes('/Parametri/IDOrdine') as ValoreParametro(IDOrdine))
			IF 	ISNULL(@sGUID,'') <> '' 
					SET @uIDOrdine=CONVERT(UNIQUEIDENTIFIER,	@sGUID)	
					END
	
		IF @xParametri.exist('/Parametri/IDPaziente')=1
		BEGIN
			SET @sGUID=(SELECT TOP 1 ValoreParametro.IDPaziente.value('.','VARCHAR(50)')
							  FROM @xParametri.nodes('/Parametri/IDPaziente') as ValoreParametro(IDPaziente))
							  
				IF 	ISNULL(@sGUID,'') <> '' 
				BEGIN
					SET @uIDPaziente=CONVERT(UNIQUEIDENTIFIER,	@sGUID)		
					SET	@sSET= @sSET + ',IDPaziente=''' + convert(VARCHAR(50),@uIDPaziente) + ''''	+ CHAR(13) + CHAR(10)	
				END
				ELSE
					SET	@sSET= @sSET + ',IDPaziente=NULL'	+ CHAR(13) + CHAR(10)									  		
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
		
		IF @xParametri.exist('/Parametri/IDOrdineOE')=1
		BEGIN
			SET @sIDOrdineOE=(SELECT TOP 1 ValoreParametro.IDOrdineOE.value('.','VARCHAR(50)')
							  FROM @xParametri.nodes('/Parametri/IDOrdineOE') as ValoreParametro(IDOrdineOE))
							  
				IF 	ISNULL(@sIDOrdineOE,'') <> '' 
				BEGIN					
					SET	@sSET= @sSET + ',IDOrdineOE=''' + convert(VARCHAR(50),@sIDOrdineOE) + ''''	+ CHAR(13) + CHAR(10)	
				END
				ELSE
					SET	@sSET= @sSET + ',IDOrdineOE=NULL'	+ CHAR(13) + CHAR(10)									  		
		END		
	
		IF @xParametri.exist('/Parametri/NumeroOrdineOE')=1
	BEGIN
		SET @sNumeroOrdineOE=(SELECT TOP 1 ValoreParametro.NumeroOrdineOE.value('.','VARCHAR(50)')
						  FROM @xParametri.nodes('/Parametri/NumeroOrdineOE') as ValoreParametro(NumeroOrdineOE))
						  
			IF 	ISNULL(@sNumeroOrdineOE,'') <> '' 
			BEGIN					
				SET	@sSET= @sSET + ',NumeroOrdineOE=''' + convert(VARCHAR(50),@sNumeroOrdineOE) + ''''	+ CHAR(13) + CHAR(10)	
			END
			ELSE
				SET	@sSET= @sSET + ',NumeroOrdineOE=NULL'	+ CHAR(13) + CHAR(10)									  		
	END		
	
		IF @xParametri.exist('/Parametri/XMLOE')=1
	BEGIN
		SET @xXMLOE	=(SELECT TOP 1 ValoreParametro.XMLOE.query('./*')
					  FROM @xParametri.nodes('/Parametri/XMLOE') as ValoreParametro(XMLOE))		
					  
		
		SET @binXMLOE=CONVERT(VARBINARY(MAX),@xXMLOE)
		SET @txtXMLOE=CAST(N'' AS xml).value('xs:base64Binary(sql:variable("@binXMLOE"))', 'varchar(max)')
		SET @binXMLOE=CAST(N'' AS xml).value('xs:base64Binary(sql:variable("@txtXMLOE"))', 'varbinary(max)')
					
		SET	@sSET= @sSET +',XMLOE=
											CONVERT(XML,
													CAST(N'''' AS xml).value(''xs:base64Binary("' + @txtXMLOE + 
													'")'', ''varbinary(max)'') 
												)'  + CHAR(13) + CHAR(10)	
													  					  			  				  	
	END	
	
		IF @xParametri.exist('/Parametri/Eroganti')=1
	BEGIN
		SET @sEroganti=(SELECT TOP 1 ValoreParametro.Eroganti.value('.','VARCHAR(MAX)')
						  FROM @xParametri.nodes('/Parametri/Eroganti') as ValoreParametro(Eroganti))
						  
			IF 	ISNULL(@sEroganti,'') <> '' 
			BEGIN					
				SET	@sSET= @sSET + ',Eroganti=''' + REPLACE(convert(VARCHAR(MAX),@sEroganti),'''','''''') + ''''	+ CHAR(13) + CHAR(10)	
			END
			ELSE
				SET	@sSET= @sSET + ',Eroganti=NULL'	+ CHAR(13) + CHAR(10)									  		
	END	
	
		IF @xParametri.exist('/Parametri/Prestazioni')=1
	BEGIN
		SET @sPrestazioni=(SELECT TOP 1 ValoreParametro.Prestazioni.value('.','VARCHAR(MAX)')
						  FROM @xParametri.nodes('/Parametri/Prestazioni') as ValoreParametro(Prestazioni))
						  
			IF 	ISNULL(@sPrestazioni,'') <> '' 
			BEGIN					
				SET	@sSET= @sSET + ',Prestazioni=''' + REPLACE(convert(VARCHAR(MAX),@sPrestazioni),'''','''''') + ''''	+ CHAR(13) + CHAR(10)	
			END
			ELSE
				SET	@sSET= @sSET + ',Prestazioni=NULL'	+ CHAR(13) + CHAR(10)									  		
	END	
		
		IF @xParametri.exist('/Parametri/CodStatoOrdine')=1
	BEGIN
		SET @sCodStatoOrdine=(SELECT TOP 1 ValoreParametro.CodStatoOrdine.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodStatoOrdine') as ValoreParametro(CodStatoOrdine))	
					  
		IF  ISNULL(@sCodStatoOrdine,'') <> ''
				SET	@sSET= @sSET + ',CodStatoOrdine=''' + @sCodStatoOrdine +''''	+ CHAR(13) + CHAR(10)				
			ELSE
				SET	@sSET= @sSET + ',CodStatoOrdine=NULL'	+ CHAR(13) + CHAR(10)		
	END
	
		SET @sCodUtenteInserimento=(SELECT TOP 1 ValoreParametro.CodUtenteInserimento.value('.','VARCHAR(100)')
					  FROM @xParametri.nodes('/Parametri/CodUtenteInserimento') as ValoreParametro(CodUtenteInserimento))	
	SET @sCodUtenteInserimento=ISNULL(@sCodUtenteInserimento,'')
	
	IF @sCodUtenteInserimento <> ''
		SET	@sSET= @sSET + ',CodUtenteInserimento=''' + @sCodUtenteInserimento + '''' + CHAR(13) + CHAR(10)		
									  
		SET @sCodUtenteUltimaModifica=(SELECT TOP 1 ValoreParametro.CodUtenteUltimaModifica.value('.','VARCHAR(100)')
					  FROM @xParametri.nodes('/Parametri/CodUtenteUltimaModifica') as ValoreParametro(CodUtenteUltimaModifica))	
	SET @sCodUtenteUltimaModifica=ISNULL(@sCodUtenteUltimaModifica,'')
	
	IF @sCodUtenteUltimaModifica <> ''
		SET	@sSET= @sSET + ',CodUtenteUltimaModifica=''' + @sCodUtenteUltimaModifica + '''' + CHAR(13) + CHAR(10)		
	
		SET @sCodUtenteInoltro=(SELECT TOP 1 ValoreParametro.CodUtenteInoltro.value('.','VARCHAR(100)')
					  FROM @xParametri.nodes('/Parametri/CodUtenteInoltro') as ValoreParametro(CodUtenteInoltro))	
	SET @sCodUtenteInoltro=ISNULL(@sCodUtenteInoltro,'')
	
		IF @sCodStatoOrdine='VA' 
	BEGIN		
		SET @sCodUtenteInoltro=@sCodUtenteUltimaModifica		
	END
	
	IF @sCodUtenteInoltro <> ''
		SET	@sSET= @sSET + ',CodUtenteInoltro=''' + @sCodUtenteInoltro + '''' + CHAR(13) + CHAR(10)		

	
		SET @sCodRuolo=(SELECT TOP 1 ValoreParametro.CodRuolo.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/TimeStamp/CodRuolo') as ValoreParametro(CodRuolo))
					  
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
	END	
						
		IF @xParametri.exist('/Parametri/DataUltimaModifica')=1
		BEGIN			
			SET @sDataTmp=(SELECT TOP 1 ValoreParametro.DataUltimaModifica	.value('.','VARCHAR(20)')
							  FROM @xParametri.nodes('/Parametri/DataUltimaModifica') as ValoreParametro(DataUltimaModifica	))					  
			SET @sDataTmp=ISNULL(@sDataTmp,'')
			
			IF @sDataTmp<> '' 
				BEGIN			
										SET @sDataTmp= SUBSTRING(@sDataTmp,4,2) + '-' 
								+ LEFT(@sDataTmp,2) + '-' +
								+ SUBSTRING(@sDataTmp,7,4) +
								' ' + RIGHT(@sDataTmp,5)
					IF ISDATE(@sDataTmp)=1
						SET	@dDataUltimaModifica=CONVERT(DATETIME,@sDataTmp,120)						
					ELSE
						SET	@dDataUltimaModifica=NULL			
				END
			IF @dDataUltimaModifica	IS NOT NULL
				SET	@sSET= @sSET + ',DataUltimaModifica=CONVERT(DateTime,''' + CONVERT(VARCHAR(20),@dDataUltimaModifica,120) + ''',120)' + CHAR(13) + CHAR(10)		 			
		END	
		
		IF @xParametri.exist('/Parametri/DataUltimaModificaUTC')=1
	BEGIN			
		SET @sDataTmp=(SELECT TOP 1 ValoreParametro.DataUltimaModificaUTC.value('.','VARCHAR(20)')
						  FROM @xParametri.nodes('/Parametri/DataUltimaModificaUTC') as ValoreParametro(DataUltimaModificaUTC))					  
		SET @sDataTmp=ISNULL(@sDataTmp,'')
		
		IF @sDataTmp<> '' 
			BEGIN			
								SET @sDataTmp= SUBSTRING(@sDataTmp,4,2) + '-' 
							+ LEFT(@sDataTmp,2) + '-' +
							+ SUBSTRING(@sDataTmp,7,4) +
							' ' + RIGHT(@sDataTmp,5)
				IF ISDATE(@sDataTmp)=1
					SET	@dDataUltimaModificaUTC=CONVERT(DATETIME,@sDataTmp,120)						
				ELSE
					SET	@dDataUltimaModificaUTC =NULL			
			END
		IF @dDataUltimaModificaUTC IS NOT NULL
			SET	@sSET= @sSET + ',DataUltimaModificaUTC=CONVERT(DateTime,''' + CONVERT(VARCHAR(20),@dDataUltimaModificaUTC,120) + ''',120)' + CHAR(13) + CHAR(10)		 			
	END	

		IF @xParametri.exist('/Parametri/DataInoltro')=1
		BEGIN			
			SET @sDataTmp=(SELECT TOP 1 ValoreParametro.DataInoltro.value('.','VARCHAR(20)')
							  FROM @xParametri.nodes('/Parametri/DataInoltro') as ValoreParametro(DataInoltro))					  
			SET @sDataTmp=ISNULL(@sDataTmp,'')
			
			IF @sDataTmp<> '' 
				BEGIN			
										SET @sDataTmp= SUBSTRING(@sDataTmp,4,2) + '-' 
								+ LEFT(@sDataTmp,2) + '-' +
								+ SUBSTRING(@sDataTmp,7,4) +
								' ' + RIGHT(@sDataTmp,5)
					IF ISDATE(@sDataTmp)=1
						SET	@dDataInoltro=CONVERT(DATETIME,@sDataTmp,120)						
					ELSE
						SET	@dDataInoltro=NULL			
				END
			IF @dDataInoltro	IS NOT NULL
				SET	@sSET= @sSET + ',DataInoltro=CONVERT(DateTime,''' + CONVERT(VARCHAR(20),@dDataInoltro,120) + ''',120)' + CHAR(13) + CHAR(10)		 			
		END	
		
		IF @xParametri.exist('/Parametri/DataInoltroUTC')=1
	BEGIN			
		SET @sDataTmp=(SELECT TOP 1 ValoreParametro.DataInoltroUTC.value('.','VARCHAR(20)')
						  FROM @xParametri.nodes('/Parametri/DataInoltroUTC') as ValoreParametro(DataInoltroUTC))					  
		SET @sDataTmp=ISNULL(@sDataTmp,'')
		
		IF @sDataTmp<> '' 
			BEGIN			
								SET @sDataTmp= SUBSTRING(@sDataTmp,4,2) + '-' 
							+ LEFT(@sDataTmp,2) + '-' +
							+ SUBSTRING(@sDataTmp,7,4) +
							' ' + RIGHT(@sDataTmp,5)
				IF ISDATE(@sDataTmp)=1
					SET	@dDataInoltroUTC=CONVERT(DATETIME,@sDataTmp,120)						
				ELSE
					SET	@dDataInoltroUTC =NULL			
			END
		IF @dDataInoltroUTC IS NOT NULL
			SET	@sSET= @sSET + ',DataInoltroUTC=CONVERT(DateTime,''' + CONVERT(VARCHAR(20),@dDataInoltroUTC,120) + ''',120)' + CHAR(13) + CHAR(10)		 			
	END	
	ELSE
	BEGIN
				IF @sCodStatoOrdine='VA' 
		BEGIN		
			SET @dDataInoltro=@dDataUltimaModifica		
			SET @dDataInoltroUTC=@dDataUltimaModificaUTC
			SET	@sSET= @sSET + ',DataInoltro=CONVERT(DateTime,''' + CONVERT(VARCHAR(20),@dDataInoltro,120) + ''',120)' + CHAR(13) + CHAR(10)		 			
			SET	@sSET= @sSET + ',DataInoltroUTC=CONVERT(DateTime,''' + CONVERT(VARCHAR(20),@dDataInoltroUTC,120) + ''',120)' + CHAR(13) + CHAR(10)		 			
		END
	END


		IF @xParametri.exist('/Parametri/DataProgrammazioneOE')=1
		BEGIN			
			SET @sDataTmp=(SELECT TOP 1 ValoreParametro.DataProgrammazioneOE.value('.','VARCHAR(20)')
							  FROM @xParametri.nodes('/Parametri/DataProgrammazioneOE') as ValoreParametro(DataProgrammazioneOE))					  
			SET @sDataTmp=ISNULL(@sDataTmp,'')
			
			IF @sDataTmp<> '' 
				BEGIN			
										SET @sDataTmp= SUBSTRING(@sDataTmp,4,2) + '-' 
								+ LEFT(@sDataTmp,2) + '-' +
								+ SUBSTRING(@sDataTmp,7,4) +
								' ' + RIGHT(@sDataTmp,5)
					IF ISDATE(@sDataTmp)=1
						SET	@dDataProgrammazioneOE=CONVERT(DATETIME,@sDataTmp,120)						
					ELSE
						SET	@dDataProgrammazioneOE=NULL			
				END
			IF @dDataProgrammazioneOE	IS NOT NULL
				SET	@sSET= @sSET + ',DataProgrammazioneOE=CONVERT(DateTime,''' + CONVERT(VARCHAR(20),@dDataProgrammazioneOE,120) + ''',120)' + CHAR(13) + CHAR(10)		 			
		END	
		
		IF @xParametri.exist('/Parametri/DataProgrammazioneOEUTC')=1
	BEGIN			
		SET @sDataTmp=(SELECT TOP 1 ValoreParametro.DataProgrammazioneOEUTC.value('.','VARCHAR(20)')
						  FROM @xParametri.nodes('/Parametri/DataProgrammazioneOEUTC') as ValoreParametro(DataProgrammazioneOEUTC))					  
		SET @sDataTmp=ISNULL(@sDataTmp,'')
		
		IF @sDataTmp<> '' 
			BEGIN			
								SET @sDataTmp= SUBSTRING(@sDataTmp,4,2) + '-' 
							+ LEFT(@sDataTmp,2) + '-' +
							+ SUBSTRING(@sDataTmp,7,4) +
							' ' + RIGHT(@sDataTmp,5)
				IF ISDATE(@sDataTmp)=1
					SET	@dDataProgrammazioneOEUTC=CONVERT(DATETIME,@sDataTmp,120)						
				ELSE
					SET	@dDataProgrammazioneOEUTC =NULL			
			END
		IF @dDataProgrammazioneOEUTC IS NOT NULL
			SET	@sSET= @sSET + ',DataProgrammazioneOEUTC=CONVERT(DateTime,''' + CONVERT(VARCHAR(20),@dDataProgrammazioneOEUTC,120) + ''',120)' + CHAR(13) + CHAR(10)		 			
	END	
	
		IF @xParametri.exist('/Parametri/CodUAUltimaModifica')=1
	BEGIN
		SET @sCodUAUltimaModifica	=(SELECT TOP 1 ValoreParametro.CodUAUltimaModifica.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodUAUltimaModifica') as ValoreParametro(CodUAUltimaModifica))	
		SET @sCodUAUltimaModifica=ISNULL(@sCodUAUltimaModifica,'')
	
		IF @sCodUAUltimaModifica <> ''
			SET	@sSET= @sSET + ',CodUAUltimaModifica=''' + @sCodUAUltimaModifica + '''' + CHAR(13) + CHAR(10)		
	END
	
		IF @xParametri.exist('/Parametri/CodPriorita')=1
	BEGIN
		SET @sCodPriorita	=(SELECT TOP 1 ValoreParametro.CodPriorita.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodPriorita') as ValoreParametro(CodPriorita))		
		SET @sCodPriorita=ISNULL(@sCodPriorita,'')
	
		IF @sCodPriorita <> ''
			SET	@sSET= @sSET + ',CodPriorita=''' + @sCodPriorita + '''' + CHAR(13) + CHAR(10)		
	END
	
		IF @xParametri.exist('/Parametri/CodSistema')=1
	BEGIN		
		SET @sCodSistema=(SELECT TOP 1 ValoreParametro.CodSistema.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodSistema') as ValoreParametro(CodSistema))	
		IF ISNULL(@sCodSistema,'')=''
			SET	@sSET= @sSET + ',CodSistema=NULL' + CHAR(13) + CHAR(10)					  		
		ELSE
			SET	@sSET= @sSET + ',CodSistema=''' + @sCodSistema + '''' + CHAR(13) + CHAR(10)					  		
	END
		
		IF @xParametri.exist('/Parametri/IDSistema')=1
	BEGIN
		SET @sIDSistema=(SELECT TOP 1 ValoreParametro.IDSistema.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDSistema') as ValoreParametro(IDSistema))
					  
		IF ISNULL(@sIDSistema,'')=''	
			SET	@sSET= @sSET + ',IDSistema=NULL' + CHAR(13) + CHAR(10)			
		ELSE		  
			SET	@sSET= @sSET + ',IDSistema=''' + @sIDSistema + '''' + CHAR(13) + CHAR(10)						  					  
	END
	
		IF @xParametri.exist('/Parametri/IDGruppo')=1
	BEGIN
		SET @sIDGruppo=(SELECT TOP 1 ValoreParametro.IDGruppo.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDGruppo') as ValoreParametro(IDGruppo))
					  
		IF ISNULL(@sIDGruppo,'')=''	
			SET	@sSET= @sSET + ',IDGruppo=NULL' + CHAR(13) + CHAR(10)			
		ELSE		  		
			SET	@sSET= @sSET + ',IDGruppo=''' + @sIDGruppo + '''' + CHAR(13) + CHAR(10)						  
					  
	END
			
		IF @xParametri.exist('/Parametri/InfoSistema')=1
	BEGIN
		SET @sIDGruppo=(SELECT TOP 1 ValoreParametro.InfoSistema.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/InfoSistema') as ValoreParametro(InfoSistema))
					  
		IF ISNULL(@sIDGruppo,'')=''	
			SET	@sSET= @sSET + ',InfoSistema=NULL' + CHAR(13) + CHAR(10)			
		ELSE		  		
			SET	@sSET= @sSET + ',InfoSistema=''' + @sIDGruppo + '''' + CHAR(13) + CHAR(10)						  
					  
	END

		IF @xParametri.exist('/Parametri/StrutturaDatiAccessori')=1
	BEGIN					  	
		SET @xStrutturaDatiAccessori=(SELECT TOP 1 ValoreParametro.StrutturaDatiAccessori.query('./*')
											FROM @xParametri.nodes('/Parametri/StrutturaDatiAccessori') as ValoreParametro(StrutturaDatiAccessori))				
			
		IF @xStrutturaDatiAccessori IS NOT NULL
		BEGIN
			SET @binTmp=CONVERT(VARBINARY(MAX),@xStrutturaDatiAccessori)
			SET @txtTmp=CAST(N'' AS xml).value('xs:base64Binary(sql:variable("@binTmp"))', 'varchar(max)')
			SET @binTmp=CAST(N'' AS xml).value('xs:base64Binary(sql:variable("@txtTmp"))', 'varbinary(max)')
					
			SET	@sSET= @sSET +',StrutturaDatiAccessori=
												CONVERT(XML,
														CAST(N'''' AS xml).value(''xs:base64Binary("' + @txtTmp + 
														'")'', ''varbinary(max)'') 
													)'  + CHAR(13) + CHAR(10)	
		END
		ELSE
			SET	@sSET= @sSET +',StrutturaDatiAccessori=NULL '	+ CHAR(13) + CHAR(10)															
	END
			
		IF @xParametri.exist('/Parametri/DatiDatiAccessori')=1
	BEGIN					  	
		SET @xDatiDatiAccessori=(SELECT TOP 1 ValoreParametro.DatiDatiAccessori.query('./*')
											FROM @xParametri.nodes('/Parametri/DatiDatiAccessori') as ValoreParametro(DatiDatiAccessori))				
			
		IF @xDatiDatiAccessori IS NOT NULL
		BEGIN
			SET @binTmp=CONVERT(VARBINARY(MAX),@xDatiDatiAccessori)
			SET @txtTmp=CAST(N'' AS xml).value('xs:base64Binary(sql:variable("@binTmp"))', 'varchar(max)')
			SET @binTmp=CAST(N'' AS xml).value('xs:base64Binary(sql:variable("@txtTmp"))', 'varbinary(max)')
					
			SET	@sSET= @sSET +',DatiDatiAccessori=
												CONVERT(XML,
														CAST(N'''' AS xml).value(''xs:base64Binary("' + @txtTmp + 
														'")'', ''varbinary(max)'') 
													)'  + CHAR(13) + CHAR(10)	
		END
		ELSE
			SET	@sSET= @sSET +',DatiDatiAccessori=NULL '	+ CHAR(13) + CHAR(10)															
	END
	
		IF @xParametri.exist('/Parametri/LayoutDatiAccessori')=1
	BEGIN					  	
		SET @xLayoutDatiAccessori=(SELECT TOP 1 ValoreParametro.LayoutDatiAccessori.query('./*')
											FROM @xParametri.nodes('/Parametri/LayoutDatiAccessori') as ValoreParametro(LayoutDatiAccessori))				
			
		IF @xLayoutDatiAccessori IS NOT NULL
		BEGIN
			SET @binTmp=CONVERT(VARBINARY(MAX),@xLayoutDatiAccessori)
			SET @txtTmp=CAST(N'' AS xml).value('xs:base64Binary(sql:variable("@binTmp"))', 'varchar(max)')
			SET @binTmp=CAST(N'' AS xml).value('xs:base64Binary(sql:variable("@txtTmp"))', 'varbinary(max)')
					
			SET	@sSET= @sSET +',LayoutDatiAccessori=
												CONVERT(XML,
														CAST(N'''' AS xml).value(''xs:base64Binary("' + @txtTmp + 
														'")'', ''varbinary(max)'') 
													)'  + CHAR(13) + CHAR(10)	
		END
		ELSE
			SET	@sSET= @sSET +',LayoutDatiAccessori=NULL '	+ CHAR(13) + CHAR(10)															
	END
					  
	    SET @xTimeStamp=(SELECT TOP 1 ValoreParametro.TS.query('.')
					  FROM @xParametri.nodes('/Parametri/TimeStamp') as ValoreParametro(TS))
	
	
		SET @sCodAzione	=(SELECT TOP 1 ValoreParametro.CodAzione.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/TimeStamp/CodAzione') as ValoreParametro(CodAzione))	
	SET @sCodAzione	=ISNULL(@sCodAzione,'')
		
	
	
						
					


				
		IF LEFT(@sSET,1)=',' 
		SET @sSET=RIGHT(@sSET,LEN(@sSET)-1)
	
	IF @sSET <> ''		
		BEGIN
						
						IF @uIDOrdine IS NULL AND ISNULL(@sNumeroOrdineOE,'') <>''
				SET @uIDOrdine=(SELECT TOP 1 ID FROM T_MovOrdini WHERE NumeroOrdineOE=@sNumeroOrdineOE)
			
						IF @uIDOrdine IS NOT NULL
				SET @sWHERE =' WHERE ID=''' + convert(varchar(50),@uIDOrdine) +''''
			ELSE										
				SET @sWHERE =' WHERE 1=0'			
				
						SET @sSQL=	ISNULL(@sSQL,'') + CHAR(13) + CHAR(10) + 
						ISNULL(@sSET,'') + CHAR(13) + CHAR(10) + 
						ISNULL(@sWHERE,'')			
																								
											
			
						SET @xTimeStamp.modify('delete (/TimeStamp/IDEntita)[1]') 
					
			SET @xTimeStamp.modify('insert <IDEntita>{sql:variable("@uIDOrdine")}</IDEntita> into (/TimeStamp)[1]')
			
						
						IF @sCodAzione='CAN' 
			BEGIN
				
				IF @sCodStatoOrdine='VA' 
				BEGIN
					SET @sCodAzione='VAL'
					SET @xTemp='<CodAzione>VAL</CodAzione>'
					SET @xTimeStamp.modify('delete (/TimeStamp/CodAzione)[1]') 
					SET @xTimeStamp.modify('insert sql:variable("@xTemp") as first into (/TimeStamp)[1]')
					SET @sCodUtenteInoltro=@sCodUtenteUltimaModifica
				END
				ELSE
				IF @sCodStatoOrdine IN ('CA','AN')
				BEGIN				
					SET @sCodAzione='CAN'
					SET @xTemp='<CodAzione>CAN</CodAzione>'
					SET @xTimeStamp.modify('delete (/TimeStamp/CodAzione)[1]') 
					SET @xTimeStamp.modify('insert sql:variable("@xTemp") as first into (/TimeStamp)[1]')				
				END
				ELSE
				BEGIN				
					SET @sCodAzione='MOD'
					SET @xTemp='<CodAzione>MOD</CodAzione>'
					SET @xTimeStamp.modify('delete (/TimeStamp/CodAzione)[1]') 
					SET @xTimeStamp.modify('insert sql:variable("@xTemp") as first into (/TimeStamp)[1]')				
				END
			END
			
						SET @xTimeStampBase=@xTimeStamp
				
			SET @xTimeStamp=CONVERT(XML,
								'<Parametri>' + CONVERT(varchar(max),@xTimeStamp) +
								'</Parametri>')		
			
			
												
			SET @xParLog=CONVERT(XML,'<Parametri><LogPrima/><LogDopo/></Parametri>')										
			
			SET @xLogPrima=Convert(XML,'<DataSet></DataSet>')														
			
			SET @xTemp=
				(SELECT * FROM 
					(SELECT
						 *,@sCodRuolo AS  CodRuolo
					 FROM T_MovOrdini
					 WHERE ID=@uIDOrdine										) AS [Table]
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
							(SELECT	*,@sCodRuolo AS  CodRuolo
							 FROM T_MovOrdini
							 WHERE ID=@uIDOrdine												) AS [Table]
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