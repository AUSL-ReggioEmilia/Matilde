CREATE PROCEDURE [dbo].[MSP_AggMovAppuntamenti](@xParametri XML)
AS
BEGIN
		
	
												
		DECLARE @uIDAppuntamento AS UNIQUEIDENTIFIER	
	DECLARE @sCodUA AS VARCHAR(20)

	DECLARE @uIDEpisodio AS UNIQUEIDENTIFIER	
	DECLARE @uIDPaziente AS UNIQUEIDENTIFIER	
	DECLARE @uIDTrasferimento AS UNIQUEIDENTIFIER	
	
		DECLARE @sCodTipoAppuntamento AS VARCHAR(20)
	DECLARE @sCodStatoAppuntamento AS VARCHAR(20)
	DECLARE @sCodSistema AS VARCHAR(20)
	DECLARE @sIDSistema AS VARCHAR(50)
	DECLARE @sIDGruppo AS VARCHAR(50)
	DECLARE @sInfoSistema AS VARCHAR(50)
	
	DECLARE @sElencoRisorse AS VARCHAR(900)	
		
	DECLARE @dDataEvento AS DATETIME	
	DECLARE @dDataEventoUTC AS DATETIME
	DECLARE @dDataInizio AS DATETIME	
	DECLARE @dDataFine AS DATETIME							
	
	DECLARE @sCodUtenteRilevazione AS VARCHAR(100)
	DECLARE @sCodUtenteUltimaModifica AS VARCHAR(100)

	DECLARE @txtTitolo AS VARCHAR(MAX)
	
		DECLARE @sGUID AS VARCHAR(50)
	DECLARE @uGUID AS UNIQUEIDENTIFIER
	DECLARE @sDataTmp AS VARCHAR(20)	
	DECLARE @sTmp AS VARCHAR(MAX)	
	
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
			
		SET @sSQL='UPDATE T_MovAppuntamenti ' + CHAR(13) + CHAR(10) +
			  'SET '  				  						
			
	SET @sSET =''
	
	IF @xParametri.exist('/Parametri/IDAppuntamento')=1
	BEGIN
				SET @sGUID=(SELECT TOP 1 ValoreParametro.IDAppuntamento.value('.','VARCHAR(50)')
						  FROM @xParametri.nodes('/Parametri/IDAppuntamento') as ValoreParametro(IDAppuntamento))
						  
		IF 	ISNULL(@sGUID,'') <> '' 
				SET @uIDAppuntamento=CONVERT(UNIQUEIDENTIFIER,	@sGUID)			
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

	

		IF @xParametri.exist('/Parametri/Titolo')=1
	BEGIN	
		SET @txtTitolo=(SELECT TOP 1 ValoreParametro.Titolo.value('.','VARCHAR(MAX)')
				  FROM @xParametri.nodes('/Parametri/Titolo') as ValoreParametro(Titolo))					  

		IF @txtTitolo <> ''
			SET	@sSET= @sSET +',Titolo=
										CONVERT(varchar(max),
												CAST(N'''' AS xml).value(''xs:base64Binary("' + @txtTitolo
												+ '")'', ''varbinary(max)'') 
											)'  + CHAR(13) + CHAR(10)	
		ELSE
			SET	@sSET= @sSET +',Titolo=NULL '	+ CHAR(13) + CHAR(10)														
	END	
	

		IF @xParametri.exist('/Parametri/CodTipoAppuntamento')=1
	BEGIN
		SET @sCodTipoAppuntamento=(SELECT TOP 1 ValoreParametro.CodTipoAppuntamento.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodTipoAppuntamento') as ValoreParametro(CodTipoAppuntamento))	
					  
		IF @sCodTipoAppuntamento <> ''
			BEGIN									
				SET	@sSET= @sSET + ',CodTipoAppuntamento=''' + @sCodTipoAppuntamento +''''	+ CHAR(13) + CHAR(10)				
			END	
			ELSE
				SET	@sSET= @sSET + ',CodTipoAppuntamento=NULL'	+ CHAR(13) + CHAR(10)			
	END
							  
		IF @xParametri.exist('/Parametri/CodStatoAppuntamento')=1
		BEGIN
			SET @sCodStatoAppuntamento=(SELECT TOP 1 ValoreParametro.CodStatoAppuntamento.value('.','VARCHAR(20)')
						  FROM @xParametri.nodes('/Parametri/CodStatoAppuntamento') as ValoreParametro(CodStatoAppuntamento))	
						  
			IF @sCodStatoAppuntamento <> ''
					SET	@sSET= @sSET + ',CodStatoAppuntamento=''' + @sCodStatoAppuntamento+''''	+ CHAR(13) + CHAR(10)				
				ELSE
					SET	@sSET= @sSET + ',CodStatoAppuntamento=NULL'	+ CHAR(13) + CHAR(10)		
		END	
		
		IF @xParametri.exist('/Parametri/ElencoRisorse')=1
		BEGIN
			SET @sElencoRisorse=(SELECT TOP 1 ValoreParametro.ElencoRisorse.value('.','VARCHAR(900)')
						  FROM @xParametri.nodes('/Parametri/ElencoRisorse') as ValoreParametro(ElencoRisorse))	
						  
			IF @sElencoRisorse <> ''
				BEGIN
											SET @sTmp=dbo.MF_SQLVarcharInsert(@sElencoRisorse)									
					SET	@sSET= @sSET + ',ElencoRisorse=' + @sTmp+''	+ CHAR(13) + CHAR(10)				
				END	
				ELSE
					SET	@sSET= @sSET + ',ElencoRisorse=NULL'	+ CHAR(13) + CHAR(10)		
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
	
	
		SET @sCodUtenteUltimaModifica=(SELECT TOP 1 ValoreParametro.CodUtenteUltimaModifica.value('.','VARCHAR(100)')
					  FROM @xParametri.nodes('/Parametri/TimeStamp/CodLogin') as ValoreParametro(CodUtenteUltimaModifica))	
	SET @sCodUtenteUltimaModifica=ISNULL(@sCodUtenteUltimaModifica,'')
		
	IF @sCodUtenteUltimaModifica <> ''
		SET	@sSET= @sSET + ',CodUtenteUltimaModifica=''' + @sCodUtenteUltimaModifica + '''' + CHAR(13) + CHAR(10)		
	ELSE
		SET	@sSET= @sSET + ',CodUtenteUltimaModifica=NULL' + CHAR(13) + CHAR(10)	
	
		SET	@sSET= @sSET + ',DataUltimaModifica=getdate() ' + CHAR(13) + CHAR(10)	
		
		SET	@sSET= @sSET + ',DataUltimaModificaUTC=getdate() ' + CHAR(13) + CHAR(10)	
					
	    SET @xTimeStamp=(SELECT TOP 1 ValoreParametro.TS.query('.')
					  FROM @xParametri.nodes('/Parametri/TimeStamp') as ValoreParametro(TS))
	
		SET @xSchedaMovimento=(SELECT TOP 1 ValoreParametro.TS.query('.')
					  FROM @xParametri.nodes('/Parametri/SchedaMovimento') as ValoreParametro(TS))					
					  		
		IF @xParametri.exist('/Parametri/CodSistema')=1
	BEGIN
		SET @sCodSistema=(SELECT TOP 1 ValoreParametro.CodSistema.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodSistema') as ValoreParametro(CodSistema))		

		IF ISNULL(@sCodSistema,'')=''
			SET	@sSET= @sSET + ',CodSistema=NULL'
		ELSE
			SET	@sSET= @sSET + ',CodSistema=''' + @sCodSistema + ''''	 			  
	END


		IF @xParametri.exist('/Parametri/IDSistema')=1
	BEGIN
		SET @sIDSistema=(SELECT TOP 1 ValoreParametro.IDSistema.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDSistema') as ValoreParametro(IDSistema))		
		IF ISNULL(@sIDSistema,'')=''
			SET	@sSET= @sSET + ',IDSistema = NULL'
		ELSE
			SET	@sSET= @sSET + ',IDSistema=''' + @sIDSistema + ''''	 			  
	END

		IF @xParametri.exist('/Parametri/IDGruppo')=1
	BEGIN
		SET @sIDGruppo=(SELECT TOP 1 ValoreParametro.IDGruppo.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDGruppo') as ValoreParametro(IDGruppo))		

		IF ISNULL(@sIDGruppo,'')=''
			SET	@sSET= @sSET + ',IDGruppo = NULL'	
		ELSE
			SET	@sSET= @sSET + ',IDGruppo=''' + @sIDGruppo + ''''	 			  
	END
						
		IF @xParametri.exist('/Parametri/InfoSistema')=1
	BEGIN
		SET @sInfoSistema=(SELECT TOP 1 ValoreParametro.InfoSistema.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/InfoSistema') as ValoreParametro(InfoSistema))		
		SET	@sSET= @sSET + ',InfoSistema=''' + @sInfoSistema + ''''	 			  
	END

			
			
		IF LEFT(@sSET,1)=',' 
		SET @sSET=RIGHT(@sSET,LEN(@sSET)-1)


	IF LTRIM(RTRIM(@sSET)) <> ''
		BEGIN
																SET @sWHERE =' WHERE ID=''' + convert(varchar(50),@uIDAppuntamento) +''''
					
								SET @sSQL=	ISNULL(@sSQL,'') + CHAR(13) + CHAR(10) + 
							ISNULL(@sSET,'') + CHAR(13) + CHAR(10) + 
							ISNULL(@sWHERE,'')			


																
								SET @xTimeStamp.modify('delete (/TimeStamp/IDEntita)[1]') 
						
				SET @xTimeStamp.modify('insert <IDEntita>{sql:variable("@uIDAppuntamento")}</IDEntita> into (/TimeStamp)[1]')

				SET @xTimeStampBase=@xTimeStamp
	
				SET @xTimeStamp=CONVERT(XML,
									'<Parametri>' + CONVERT(varchar(max),@xTimeStamp) +
									'</Parametri>')
						
																
				SET @xParLog=CONVERT(XML,'<Parametri><LogPrima/><LogDopo/></Parametri>')										
				
				SET @xLogPrima=Convert(XML,'<DataSet></DataSet>')														
				
				SET @xTemp=
					(SELECT * FROM 
						(SELECT * FROM T_MovAppuntamenti
						 WHERE ID=@uIDAppuntamento											) AS [Table]
					FOR XML AUTO, ELEMENTS)

				SET @xLogPrima.modify('insert sql:variable("@xTemp") as first into (/DataSet)[1]')
																			
				SET @xParLog.modify('insert sql:variable("@xLogPrima") as first into (/Parametri/LogPrima)[1]')
																										
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
									(SELECT * FROM T_MovAppuntamenti
									 WHERE ID=@uIDAppuntamento														) AS [Table]
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