CREATE PROCEDURE [dbo].[MSP_AggMovEvidenzaClinica](@xParametri XML)
AS
BEGIN

	DECLARE @uIDEvidenzaClinica AS UNIQUEIDENTIFIER	
	DECLARE @uIDEpisodio AS UNIQUEIDENTIFIER	
	DECLARE @uIDTrasferimento AS UNIQUEIDENTIFIER	
		
	DECLARE @dDataEvento AS DATETIME	
	DECLARE @dDataEventoUTC AS DATETIME
				
	DECLARE @dDataEventoDWH AS DATETIME	
	DECLARE @dDataEventoDWHUTC AS DATETIME
	
	DECLARE @sCodTipoEvidenzaClinica AS VARCHAR(20)
	DECLARE @sCodStatoEvidenzaClinica AS VARCHAR(20)
	DECLARE @sCodStatoEvidenzaClinicaVisione AS VARCHAR(20)
	
	DECLARE @sIDRefertoDWH AS VARCHAR(50)
	DECLARE @sNumeroRefertoDWH AS VARCHAR(50)
	DECLARE @txtAnteprima AS VARCHAR(MAX)
		
	DECLARE @binPDFDWH AS VARBINARY(MAX)
	DECLARE @txtPDFDWH AS VARCHAR(MAX)
	
	DECLARE @sCodUtenteVisione AS VARCHAR(100)	
	DECLARE @sCodUtenteUltimaModifica AS VARCHAR(100)						
	
		DECLARE @sGUID AS VARCHAR(50)
	DECLARE @uGUID AS UNIQUEIDENTIFIER
	DECLARE @sDataTmp AS VARCHAR(20)		
	
	DECLARE @sTmp AS VARCHAR(MAX)
	
	DECLARE @sSQL AS VARCHAR(MAX)
	DECLARE @sSET AS VARCHAR(MAX)
	DECLARE @sWHERE AS VARCHAR(MAX)
	
	DECLARE @xTimeStampBase AS XML	
	DECLARE @xTimeStamp AS XML	
			
	DECLARE @xLogPrima AS XML
	DECLARE @xLogDopo AS XML
	DECLARE @xTemp AS XML
	DECLARE @xParLog AS XML


	SET @sSQL='UPDATE T_MovEvidenzaClinica ' + CHAR(13) + CHAR(10) +
			  'SET '  				  						
			
	SET @sSET =''
				
		IF @xParametri.exist('/Parametri/IDEvidenzaClinica')=1
		BEGIN
			SET @sGUID=(SELECT TOP 1 ValoreParametro.IDEvidenzaClinica.value('.','VARCHAR(50)')
							  FROM @xParametri.nodes('/Parametri/IDEvidenzaClinica') as ValoreParametro(IDEvidenzaClinica))
			IF 	ISNULL(@sGUID,'') <> '' 
					SET @uIDEvidenzaClinica=CONVERT(UNIQUEIDENTIFIER,	@sGUID)
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
	
						
		IF @xParametri.exist('/Parametri/DataEventoDWH')=1
		BEGIN			
			SET @sDataTmp=(SELECT TOP 1 ValoreParametro.DataEventoDWH.value('.','VARCHAR(20)')
							  FROM @xParametri.nodes('/Parametri/DataEventoDWH') as ValoreParametro(DataEventoDWH))					  
			SET @sDataTmp=ISNULL(@sDataTmp,'')
			
			IF @sDataTmp<> '' 
				BEGIN			
										SET @sDataTmp= SUBSTRING(@sDataTmp,4,2) + '-' 
								+ LEFT(@sDataTmp,2) + '-' +
								+ SUBSTRING(@sDataTmp,7,4) +
								' ' + RIGHT(@sDataTmp,5)
					IF ISDATE(@sDataTmp)=1
						SET	@dDataEventoDWH=CONVERT(DATETIME,@sDataTmp,120)						
					ELSE
						SET	@dDataEventoDWH =NULL			
				END
			IF @dDataEvento IS NOT NULL
				SET	@sSET= @sSET + ',DataEventoDWH=CONVERT(DateTime,''' + CONVERT(VARCHAR(20),@dDataEventoDWH,120) + ''',120)' + CHAR(13) + CHAR(10)		 
			ELSE	
				SET	@sSET= @sSET + ',DataEventoDWH=NULL' + CHAR(13) + CHAR(10)		 
		END	
		
	
		IF @xParametri.exist('/Parametri/DataEventoDWHUTC')=1
	BEGIN	
		SET @sDataTmp=(SELECT TOP 1 ValoreParametro.DataEventoDWHUTC.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/DataEventoDWHUTC') as ValoreParametro(DataEventoDWHUTC))					  
		SET @sDataTmp=ISNULL(@sDataTmp,'')
	
		IF @sDataTmp<> '' 
		BEGIN			
						SET @sDataTmp= SUBSTRING(@sDataTmp,4,2) + '-' 
						+ LEFT(@sDataTmp,2) + '-' +
						+ SUBSTRING(@sDataTmp,7,4) +
						' ' + RIGHT(@sDataTmp,5)
			IF ISDATE(@sDataTmp)=1
				SET	@dDataEventoDWHUTC=CONVERT(DATETIME,@sDataTmp,120)						
			ELSE
				SET	@dDataEventoDWHUTC =NULL			
		END
		
		IF @dDataEventoUTC IS NOT NULL		
			SET	@sSET= @sSET + ',DataEventoDWHUTC=CONVERT(DateTime,''' + CONVERT(VARCHAR(20),@dDataEventoDWHUTC,120) + ''',120)' + CHAR(13) + CHAR(10)		
		ELSE
			SET	@sSET= @sSET + ',DataEventoDWHUTC=NULL ' + CHAR(13) + CHAR(10)		
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

	SET @sGUID=''
		IF @xParametri.exist('/Parametri/IDTrasferimento')=1
		BEGIN
			SET @sGUID=(SELECT TOP 1 ValoreParametro.IDTrasferimento.value('.','VARCHAR(50)')
							  FROM @xParametri.nodes('/Parametri/IDTrasferimento') as ValoreParametro(IDTrasferimento))
		END
							
	 IF ISNULL(@sGUID,'') ='' 			
		BEGIN
			IF @xParametri.exist('/Parametri/TimeStamp/IDTrasferimento')=1
				BEGIN
										SET @sGUID=(SELECT TOP 1 ValoreParametro.IDTrasferimento.value('.','VARCHAR(50)')
							  FROM @xParametri.nodes('/Parametri/TimeStamp/IDTrasferimento') as ValoreParametro(IDTrasferimento))
				END			  
		END		
			  	
	IF 	ISNULL(@sGUID,'') <> '' 
	BEGIN
		SET @uIDTrasferimento=CONVERT(UNIQUEIDENTIFIER,	@sGUID)		
		SET	@sSET= @sSET + ',IDTrasferimento=ISNULL(IDTrasferimento, ''' + convert(VARCHAR(50),@uIDTrasferimento) + ''')'	+ CHAR(13) + CHAR(10)	
	END								  		
		
		
		IF @xParametri.exist('/Parametri/CodTipoEvidenzaClinica')=1
	BEGIN
		SET @sCodTipoEvidenzaClinica=(SELECT TOP 1 ValoreParametro.CodTipoEvidenzaClinica.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodTipoEvidenzaClinica') as ValoreParametro(CodTipoEvidenzaClinica))	
					  
		IF @sCodTipoEvidenzaClinica <> ''
				SET	@sSET= @sSET + ',CodTipoEvidenzaClinica=''' + @sCodTipoEvidenzaClinica +''''	+ CHAR(13) + CHAR(10)				
			ELSE
				SET	@sSET= @sSET + ',CodTipoEvidenzaClinica=NULL'	+ CHAR(13) + CHAR(10)		
	END	

		IF @xParametri.exist('/Parametri/CodStatoEvidenzaClinica')=1
	BEGIN
		SET  @sCodStatoEvidenzaClinica=(SELECT TOP 1 ValoreParametro.CodStatoEvidenzaClinica.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodStatoEvidenzaClinica') as ValoreParametro(CodStatoEvidenzaClinica))	
					  
		IF  @sCodStatoEvidenzaClinica <> ''
				SET	@sSET= @sSET + ',CodStatoEvidenzaClinica=''' +  @sCodStatoEvidenzaClinica +''''	+ CHAR(13) + CHAR(10)				
			ELSE
				SET	@sSET= @sSET + ',CodStatoEvidenzaClinica=NULL'	+ CHAR(13) + CHAR(10)		
	END	
	
		IF @xParametri.exist('/Parametri/CodStatoEvidenzaClinicaVisione')=1
	BEGIN
		SET  @sCodStatoEvidenzaClinicaVisione=(SELECT TOP 1 ValoreParametro.CodStatoEvidenzaClinicaVisione.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodStatoEvidenzaClinicaVisione') as ValoreParametro(CodStatoEvidenzaClinicaVisione))	
					  
		IF  @sCodStatoEvidenzaClinicaVisione <> ''
			BEGIN
				SET	@sSET= @sSET + ',CodStatoEvidenzaClinicaVisione=''' +  @sCodStatoEvidenzaClinicaVisione +''''	+ CHAR(13) + CHAR(10)
				IF  @sCodStatoEvidenzaClinicaVisione = 'VS'
				BEGIN
					SET	@sSET= @sSET + ',DataVisione=GETDATE()'	+ CHAR(13) + CHAR(10)
					SET	@sSET= @sSET + ',DataVisioneUTC=GETUTCDATE()'	+ CHAR(13) + CHAR(10)
				END
			END
			ELSE
				SET	@sSET= @sSET + ',CodStatoEvidenzaClinicaVisione=NULL'	+ CHAR(13) + CHAR(10)		
	END			
	
		IF @xParametri.exist('/Parametri/IDRefertoDWH')=1
	BEGIN
		SET  @sIDRefertoDWH=(SELECT TOP 1 ValoreParametro.IDRefertoDWH.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDRefertoDWH') as ValoreParametro(IDRefertoDWH))	
					  
		IF  @sIDRefertoDWH <> ''
				SET	@sSET= @sSET + ',IDRefertoDWH=''' +  @sIDRefertoDWH +''''	+ CHAR(13) + CHAR(10)				
			ELSE
				SET	@sSET= @sSET + ',IDRefertoDWH=NULL'	+ CHAR(13) + CHAR(10)		
	END	
	

		IF @xParametri.exist('/Parametri/NumeroRefertoDWH')=1
	BEGIN
		SET @sNumeroRefertoDWH=(SELECT TOP 1 ValoreParametro.NumeroRefertoDWH.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/NumeroRefertoDWH') as ValoreParametro(NumeroRefertoDWH))	
					  
		IF  @sNumeroRefertoDWH <> ''
				SET	@sSET= @sSET + ',NumeroRefertoDWH=''' +  @sNumeroRefertoDWH +''''	+ CHAR(13) + CHAR(10)				
			ELSE
				SET	@sSET= @sSET + ',NumeroRefertoDWH=NULL'	+ CHAR(13) + CHAR(10)		
	END	
		IF @xParametri.exist('/Parametri/Anteprima')=1
	BEGIN

		SET @txtAnteprima=(SELECT TOP 1 ValoreParametro.Anteprima.value('.','VARCHAR(MAX)')
				  FROM @xParametri.nodes('/Parametri/Anteprima') as ValoreParametro(Anteprima))					  
				

		IF @txtAnteprima <> ''
		BEGIN
			SET @sTmp=dbo.MF_SQLVarcharInsert(@txtAnteprima)
			SET	@sSET= @sSET + ',Anteprima=' + @sTmp + '' + CHAR(13) + CHAR(10)		
		END
		ELSE
			SET	@sSET= @sSET +',Anteprima=NULL '	+ CHAR(13) + CHAR(10)														
	END	
	

		IF @xParametri.exist('/Parametri/PDFDWH')=1
	BEGIN	
		SET @txtPDFDWH=(SELECT TOP 1 ValoreParametro.PDFDWH.value('.','VARCHAR(MAX)')
				  FROM @xParametri.nodes('/Parametri/PDFDWH') as ValoreParametro(PDFDWH))					  

		IF @txtPDFDWH <> ''
			SET	@sSET= @sSET +',PDFDWH= 
										CAST(N'''' AS xml).value(''xs:base64Binary("' + @txtPDFDWH
												+ '")'', ''varbinary(max)'')
											'  + CHAR(13) + CHAR(10)	
		ELSE
			SET	@sSET= @sSET +',PDFDWH=NULL '	+ CHAR(13) + CHAR(10)														
	END	
	
		IF @xParametri.exist('/Parametri/CodUtenteVisione')=1
	BEGIN
		SET  @sCodUtenteVisione=(SELECT TOP 1 ValoreParametro.CodUtenteVisione.value('.','VARCHAR(100)')
					  FROM @xParametri.nodes('/Parametri/CodUtenteVisione') as ValoreParametro(CodUtenteVisione))	
					  
		IF  @sCodUtenteVisione <> ''
				SET	@sSET= @sSET + ',CodUtenteVisione=''' +  @sCodUtenteVisione +''''	+ CHAR(13) + CHAR(10)				
			ELSE
				SET	@sSET= @sSET + ',CodUtenteVisione=NULL'	+ CHAR(13) + CHAR(10)		
	END	
	
		
		SET @sCodUtenteUltimaModifica=(SELECT TOP 1 ValoreParametro.CodUtenteUltimaModifica.value('.','VARCHAR(100)')
					  FROM @xParametri.nodes('/Parametri/TimeStamp/CodLogin') as ValoreParametro(CodUtenteUltimaModifica))	
	SET @sCodUtenteUltimaModifica=ISNULL(@sCodUtenteUltimaModifica,'')
		
	IF @sCodUtenteUltimaModifica <> ''
		BEGIN
			SET	@sSET= @sSET + ',CodUtenteUltimaModifica=''' + @sCodUtenteUltimaModifica + '''' + CHAR(13) + CHAR(10)					
		END 			
	ELSE
		SET	@sSET= @sSET + ',CodUtenteUltimaModifica=NULL' + CHAR(13) + CHAR(10)	
			
		SET	@sSET= @sSET + ',DataUltimaModifica=getdate() ' + CHAR(13) + CHAR(10)	
		
		SET	@sSET= @sSET + ',DataUltimaModificaUTC=getUTCdate() ' + CHAR(13) + CHAR(10)	
	
	    SET @xTimeStamp=(SELECT TOP 1 ValoreParametro.TS.query('.')
					  FROM @xParametri.nodes('/Parametri/TimeStamp') as ValoreParametro(TS))
					
	IF LEFT(@sSET,1)=',' 
		SET @sSET=RIGHT(@sSET,LEN(@sSET)-1)
	
	IF @sSET <> ''		
		BEGIN
						
				IF @uIDEvidenzaClinica IS NULL
					SET @sWHERE =' WHERE 1=0'
				ELSE
					SET @sWHERE =' WHERE ID=''' + convert(varchar(50),@uIDEvidenzaClinica) +''''

								SET @sSQL=	ISNULL(@sSQL,'') + CHAR(13) + CHAR(10) + 
						ISNULL(@sSET,'') + CHAR(13) + CHAR(10) + 
						ISNULL(@sWHERE,'')	
									
																				SET @xTimeStamp.modify('delete (/TimeStamp/IDEntita)[1]') 
						
				SET @xTimeStamp.modify('insert <IDEntita>{sql:variable("@uIDEvidenzaClinica")}</IDEntita> into (/TimeStamp)[1]')

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
							  ,IDEpisodio
							  ,IDTrasferimento
							  ,CodTipoEvidenzaClinica
							  ,CodStatoEvidenzaClinica
							  ,CodStatoEvidenzaClinicaVisione
							  ,IDRefertoDWH
							  ,NumeroRefertoDWH
							  ,DataEvento
							  ,DataEventoUTC
							  ,DataEventoDWH
							  ,DataEventoDWHUTC
							  							  ,CodUtenteInserimento
							  ,DataInserimento
							  ,DataInserimentoUTC
							  ,CodUtenteVisione
							  ,CodUtenteUltimaModifica
							  ,DataUltimaModifica
							  ,DataUltimaModificaUTC
							  ,DataVisione
							  ,DataVisioneUTC
							  ,Anteprima
						 FROM T_MovEvidenzaClinica
						 WHERE ID=@uIDEvidenzaClinica											) AS [Table]
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
								(SELECT		ID
										  ,IDNum
										  ,IDEpisodio
										  ,IDTrasferimento
										  ,CodTipoEvidenzaClinica
										  ,CodStatoEvidenzaClinica
										  ,CodStatoEvidenzaClinicaVisione
										  ,IDRefertoDWH
										  ,NumeroRefertoDWH
										  ,DataEvento
										  ,DataEventoUTC
										  ,DataEventoDWH
										  ,DataEventoDWHUTC
										  									      ,CodUtenteInserimento
										  ,DataInserimento
										  ,DataInserimentoUTC
										  ,CodUtenteVisione
										  ,CodUtenteUltimaModifica
										  ,DataUltimaModifica
										  ,DataUltimaModificaUTC
										  ,DataVisione
										  ,DataVisioneUTC
										  ,Anteprima
								 FROM T_MovEvidenzaClinica
								 WHERE ID=@uIDEvidenzaClinica													) AS [Table]
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