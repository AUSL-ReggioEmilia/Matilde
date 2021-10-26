CREATE PROCEDURE [dbo].[MSP_AggMovEpisodi](@xParametri XML)
AS
BEGIN

									 	
	
				
		DECLARE @uIDEpisodio AS UNIQUEIDENTIFIER			
	DECLARE @sCodAzi AS VARCHAR(20)	
	DECLARE @sCodTipoEpisodio AS VARCHAR(20)	
	DECLARE @sCodStatoEpisodio AS VARCHAR(20)	
	DECLARE @dDataListaAttesa AS DATETIME	
	DECLARE @dDataListaAttesaUTC AS DATETIME	
	DECLARE @dDataAnnullamentoListaAttesa AS DATETIME	
	DECLARE @dDataAnnullamentoListaAttesaUTC AS DATETIME	
	DECLARE @dDataRicovero AS DATETIME
	DECLARE @dDataRicoveroUTC AS DATETIME
	DECLARE @dDataDimissione AS DATETIME
	DECLARE @dDataDimissioneUTC AS DATETIME
	DECLARE @sNumeroNosologico AS VARCHAR(20)
	DECLARE @sNumeroListaAttesa AS VARCHAR(20)
	
		
					
	DECLARE @sCodLogin AS VARCHAR(20)	
				
		DECLARE @nTemp AS INTEGER
	DECLARE @sGUID AS VARCHAR(50)
	DECLARE @uGUID AS UNIQUEIDENTIFIER
	DECLARE @sDataTmp AS VARCHAR(30)		
	DECLARE @sSQL AS VARCHAR(MAX)
	DECLARE @sSET AS VARCHAR(MAX)
	DECLARE @sWHERE AS VARCHAR(MAX)
	
	    DECLARE @xTimeStampBase AS XML	
	DECLARE @xTimeStamp AS XML	
	
		DECLARE @xLogPrima AS XML
	DECLARE @xLogDopo AS XML
	DECLARE @xTemp AS XML
	DECLARE @xParLog AS XML
	
		
	

						
		SET @sSQL='UPDATE T_MovEpisodi ' + CHAR(13) + CHAR(10) +
			  'SET '
			  					
	SET @sSET =''	
	
		IF @xParametri.exist('/Parametri/IDEpisodio')=1
		BEGIN
			SET @sGUID=(SELECT TOP 1 ValoreParametro.IDEpisodio.value('.','VARCHAR(50)')
							  FROM @xParametri.nodes('/Parametri/IDEpisodio') as ValoreParametro(IDEpisodio))
							  
				IF 	ISNULL(@sGUID,'') <> '' 				
					SET @uIDEpisodio=CONVERT(UNIQUEIDENTIFIER,	@sGUID)		
				ELSE
					SET @uIDEpisodio=NULL
		END
		
		IF @xParametri.exist('/Parametri/CodAzi')=1
		BEGIN
			SET @sCodAzi=(SELECT TOP 1 ValoreParametro.CodAzi.value('.','VARCHAR(20)')
							  FROM @xParametri.nodes('/Parametri/CodAzi') as ValoreParametro(CodAzi))
			IF @sCodAzi <> ''
			BEGIN				
				SET	@sSET= @sSET + ',CodAzi=''' + @sCodAzi + '''' + CHAR(13) + CHAR(10)		
			END				  
		END		
	
		IF @xParametri.exist('/Parametri/CodTipoEpisodio')=1
		BEGIN
			SET @sCodTipoEpisodio=(SELECT TOP 1 ValoreParametro.CodTipoEpisodio.value('.','VARCHAR(20)')
							  FROM @xParametri.nodes('/Parametri/CodTipoEpisodio') as ValoreParametro(CodTipoEpisodio))
			IF @sCodTipoEpisodio <> ''
			BEGIN				
				SET	@sSET= @sSET + ',CodTipoEpisodio=''' + @sCodTipoEpisodio + '''' + CHAR(13) + CHAR(10)		
			END						  
		END	
	
		IF @xParametri.exist('/Parametri/CodStatoEpisodio')=1
		BEGIN
			SET @sCodStatoEpisodio=(SELECT TOP 1 ValoreParametro.CodStatoEpisodio.value('.','VARCHAR(20)')
							  FROM @xParametri.nodes('/Parametri/CodStatoEpisodio') as ValoreParametro(CodStatoEpisodio))
			IF @sCodStatoEpisodio <> ''
			BEGIN				
				SET	@sSET= @sSET + ',CodStatoEpisodio=''' + @sCodStatoEpisodio + '''' + CHAR(13) + CHAR(10)		
			END					  
		END			


		IF @xParametri.exist('/Parametri/DataListaAttesa')=1
		BEGIN			
			SET @sDataTmp=(SELECT TOP 1 ValoreParametro.DataListaAttesa.value('.','VARCHAR(30)')											  FROM @xParametri.nodes('/Parametri/DataListaAttesa') as ValoreParametro(DataListaAttesa))					  
			SET @sDataTmp=ISNULL(@sDataTmp,'')
			
			IF @sDataTmp<> '' 
				BEGIN			
											SET @sDataTmp= SUBSTRING(@sDataTmp,4,2) + '-' 
						+ LEFT(@sDataTmp,2) + '-' +
						+ SUBSTRING(@sDataTmp,7,4) +
						' ' + RIGHT(@sDataTmp,11)								IF ISDATE(@sDataTmp)=1					
						SET	@dDataListaAttesa=CONVERT(DATETIME,@sDataTmp,121)											
					ELSE
						SET	@dDataListaAttesa =NULL			
				END
			IF @dDataListaAttesa IS NOT NULL
				SET	@sSET= @sSET + ',DataListaAttesa=CONVERT(DateTime,''' + CONVERT(VARCHAR(30),@dDataListaAttesa,121) + ''',121)' + CHAR(13) + CHAR(10)				ELSE	
				SET	@sSET= @sSET + ',DataListaAttesa=NULL' + CHAR(13) + CHAR(10)		 
		END	
	
		IF @xParametri.exist('/Parametri/DataListaAttesaUTC')=1
		BEGIN			
			SET @sDataTmp=(SELECT TOP 1 ValoreParametro.DataListaAttesaUTC.value('.','VARCHAR(30)')									  FROM @xParametri.nodes('/Parametri/DataListaAttesaUTC') as ValoreParametro(DataListaAttesaUTC))					  
			SET @sDataTmp=ISNULL(@sDataTmp,'')
			
			IF @sDataTmp<> '' 
				BEGIN																
										SET @sDataTmp= SUBSTRING(@sDataTmp,4,2) + '-' 
						+ LEFT(@sDataTmp,2) + '-' +
						+ SUBSTRING(@sDataTmp,7,4) +
						' ' + RIGHT(@sDataTmp,11)							
					IF ISDATE(@sDataTmp)=1
						SET	@dDataListaAttesaUTC=CONVERT(DATETIME,@sDataTmp,121)							ELSE
						SET	@dDataListaAttesaUTC =NULL			
				END
			IF @dDataListaAttesaUTC IS NOT NULL
				SET	@sSET= @sSET + ',DataListaAttesaUTC=CONVERT(DateTime,''' + CONVERT(VARCHAR(30),@dDataListaAttesaUTC,121) + ''',121)' + CHAR(13) + CHAR(10)	 			ELSE	
				SET	@sSET= @sSET + ',DataListaAttesaUTC=NULL' + CHAR(13) + CHAR(10)		 
		END	
					
		IF @xParametri.exist('/Parametri/DataAnnullamentoListaAttesa')=1
		BEGIN			
			SET @sDataTmp=(SELECT TOP 1 ValoreParametro.DataAnnullamentoListaAttesa.value('.','VARCHAR(30)')											  FROM @xParametri.nodes('/Parametri/DataAnnullamentoListaAttesa') as ValoreParametro(DataAnnullamentoListaAttesa))					  
			SET @sDataTmp=ISNULL(@sDataTmp,'')
			
			IF @sDataTmp<> '' 
				BEGIN			
											SET @sDataTmp= SUBSTRING(@sDataTmp,4,2) + '-' 
						+ LEFT(@sDataTmp,2) + '-' +
						+ SUBSTRING(@sDataTmp,7,4) +
						' ' + RIGHT(@sDataTmp,11)								IF ISDATE(@sDataTmp)=1					
						SET	 @dDataAnnullamentoListaAttesa=CONVERT(DATETIME,@sDataTmp,121)											
					ELSE
						SET	 @dDataAnnullamentoListaAttesa=NULL			
				END
			IF @dDataAnnullamentoListaAttesa IS NOT NULL
				SET	@sSET= @sSET + ',DataAnnullamentoListaAttesa=CONVERT(DateTime,''' + CONVERT(VARCHAR(30),@dDataAnnullamentoListaAttesa,121) + ''',121)' + CHAR(13) + CHAR(10)				ELSE	
				SET	@sSET= @sSET + ',DataAnnullamentoListaAttesa=NULL' + CHAR(13) + CHAR(10)		 
		END	

			IF @xParametri.exist('/Parametri/DataAnnullamentoListaAttesaUTC')=1
		BEGIN			
			SET @sDataTmp=(SELECT TOP 1 ValoreParametro.DataAnnullamentoListaAttesaUTC.value('.','VARCHAR(30)')									  FROM @xParametri.nodes('/Parametri/DataAnnullamentoListaAttesaUTC') as ValoreParametro(DataAnnullamentoListaAttesaUTC))					  
			SET @sDataTmp=ISNULL(@sDataTmp,'')
			
			IF @sDataTmp<> '' 
				BEGIN																
										SET @sDataTmp= SUBSTRING(@sDataTmp,4,2) + '-' 
						+ LEFT(@sDataTmp,2) + '-' +
						+ SUBSTRING(@sDataTmp,7,4) +
						' ' + RIGHT(@sDataTmp,11)							
					IF ISDATE(@sDataTmp)=1
						SET	@dDataAnnullamentoListaAttesaUTC=CONVERT(DATETIME,@sDataTmp,121)							ELSE
						SET	@dDataAnnullamentoListaAttesaUTC =NULL			
				END
			IF @dDataAnnullamentoListaAttesaUTC IS NOT NULL
				SET	@sSET= @sSET + ',DataAnnullamentoListaAttesaUTC=CONVERT(DateTime,''' + CONVERT(VARCHAR(30),@dDataAnnullamentoListaAttesaUTC,121) + ''',121)' + CHAR(13) + CHAR(10)	 			ELSE	
				SET	@sSET= @sSET + ',DataAnnullamentoListaAttesaUTC=NULL' + CHAR(13) + CHAR(10)		 
		END	

		IF @xParametri.exist('/Parametri/DataRicovero')=1
		BEGIN			
			SET @sDataTmp=(SELECT TOP 1 ValoreParametro.DataRicovero.value('.','VARCHAR(30)')								  FROM @xParametri.nodes('/Parametri/DataRicovero') as ValoreParametro(DataRicovero))					  
			SET @sDataTmp=ISNULL(@sDataTmp,'')
			
			IF @sDataTmp<> '' 
				BEGIN			
										SET @sDataTmp= SUBSTRING(@sDataTmp,4,2) + '-' 
						+ LEFT(@sDataTmp,2) + '-' +
						+ SUBSTRING(@sDataTmp,7,4) +
						' ' + RIGHT(@sDataTmp,11)								IF ISDATE(@sDataTmp)=1
						SET	@dDataRicovero=CONVERT(DATETIME,@sDataTmp,121)									ELSE
						SET	@dDataRicovero=NULL			
				END
			IF @dDataRicovero IS NOT NULL
				SET	@sSET= @sSET + ',DataRicovero=CONVERT(DateTime,''' + CONVERT(VARCHAR(30),@dDataRicovero,121) + ''',121)' + CHAR(13) + CHAR(10)				ELSE	
				SET	@sSET= @sSET + ',DataRicovero=NULL' + CHAR(13) + CHAR(10)		 
		END		
			
		IF @xParametri.exist('/Parametri/DataRicoveroUTC ')=1
		BEGIN			
			SET @sDataTmp=(SELECT TOP 1 ValoreParametro.DataRicoveroUTC .value('.','VARCHAR(30)')										  FROM @xParametri.nodes('/Parametri/DataRicoveroUTC ') as ValoreParametro(DataRicoveroUTC))					  
			SET @sDataTmp=ISNULL(@sDataTmp,'')
			
			IF @sDataTmp<> '' 
				BEGIN			
										SET @sDataTmp= SUBSTRING(@sDataTmp,4,2) + '-' 
						+ LEFT(@sDataTmp,2) + '-' +
						+ SUBSTRING(@sDataTmp,7,4) +
						' ' + RIGHT(@sDataTmp,11)								IF ISDATE(@sDataTmp)=1
						SET	@dDataRicoveroUTC =CONVERT(DATETIME,@sDataTmp,121)										ELSE
						SET	@dDataRicoveroUTC =NULL			
				END
			IF @dDataRicoveroUTC  IS NOT NULL
				SET	@sSET= @sSET + ',DataRicoveroUTC=CONVERT(DateTime,''' + CONVERT(VARCHAR(30),@dDataRicoveroUTC ,121) + ''',121)' + CHAR(13) + CHAR(10)		 			ELSE	
				SET	@sSET= @sSET + ',DataRicoveroUTC=NULL' + CHAR(13) + CHAR(10)		 
		END	
		
		IF @xParametri.exist('/Parametri/DataDimissione')=1
		BEGIN			
			SET @sDataTmp=(SELECT TOP 1 ValoreParametro.DataDimissione.value('.','VARCHAR(30)')										  FROM @xParametri.nodes('/Parametri/DataDimissione') as ValoreParametro(DataDimissione))					  
			SET @sDataTmp=ISNULL(@sDataTmp,'')
			
			IF @sDataTmp<> '' 
				BEGIN			
										SET @sDataTmp= SUBSTRING(@sDataTmp,4,2) + '-' 
						+ LEFT(@sDataTmp,2) + '-' +
						+ SUBSTRING(@sDataTmp,7,4) +
						' ' + RIGHT(@sDataTmp,11)								IF ISDATE(@sDataTmp)=1
						SET	@dDataDimissione=CONVERT(DATETIME,@sDataTmp,121) 					ELSE
						SET	@dDataDimissione=NULL			
				END
			IF @dDataDimissione IS NOT NULL
				SET	@sSET= @sSET + ',DataDimissione=CONVERT(DateTime,''' + CONVERT(VARCHAR(30),@dDataDimissione,121) + ''',121)' + CHAR(13) + CHAR(10)	 			ELSE	
				SET	@sSET= @sSET + ',DataDimissione=NULL' + CHAR(13) + CHAR(10)		 
		END	
		
		IF @xParametri.exist('/Parametri/DataDimissioneUTC')=1
		BEGIN			
			SET @sDataTmp=(SELECT TOP 1 ValoreParametro.DataDimissioneUTC.value('.','VARCHAR(30)')										  FROM @xParametri.nodes('/Parametri/DataDimissioneUTC') as ValoreParametro(DataDimissioneUTC))					  
			SET @sDataTmp=ISNULL(@sDataTmp,'')
			
			IF @sDataTmp<> '' 
				BEGIN			
										SET @sDataTmp= SUBSTRING(@sDataTmp,4,2) + '-' 
						+ LEFT(@sDataTmp,2) + '-' +
						+ SUBSTRING(@sDataTmp,7,4) +
						' ' + RIGHT(@sDataTmp,11)								IF ISDATE(@sDataTmp)=1
						SET	@dDataDimissioneUTC=CONVERT(DATETIME,@sDataTmp,121)						
					ELSE
						SET	@dDataDimissioneUTC=NULL			
				END
			IF @dDataDimissioneUTC IS NOT NULL
				SET	@sSET= @sSET + ',DataDimissioneUTC=CONVERT(DateTime,''' + CONVERT(VARCHAR(30),@dDataDimissioneUTC,121) + ''',121)' + CHAR(13) + CHAR(10)				ELSE	
				SET	@sSET= @sSET + ',DataDimissioneUTC=NULL' + CHAR(13) + CHAR(10)		 
		END	
		
		IF @xParametri.exist('/Parametri/NumeroNosologico')=1
		BEGIN
			SET @sNumeroNosologico=(SELECT TOP 1 ValoreParametro.NumeroNosologico.value('.','VARCHAR(50)')
							  FROM @xParametri.nodes('/Parametri/NumeroNosologico') as ValoreParametro(NumeroNosologico))
			IF @sNumeroNosologico <> ''
			BEGIN				
				SET	@sSET= @sSET + ',NumeroNosologico=''' + @sNumeroNosologico + '''' + CHAR(13) + CHAR(10)		
			END	
			ELSE
			BEGIN				
				SET	@sSET= @sSET + ',NumeroNosologico=NULL' + CHAR(13) + CHAR(10)	
			END
		END	
	
	
		IF @xParametri.exist('/Parametri/NumeroListaAttesa')=1
		BEGIN
			SET @sNumeroListaAttesa=(SELECT TOP 1 ValoreParametro.NumeroListaAttesa.value('.','VARCHAR(50)')
							  FROM @xParametri.nodes('/Parametri/NumeroListaAttesa') as ValoreParametro(NumeroListaAttesa))
			IF @sNumeroListaAttesa <> ''
			BEGIN				
				SET	@sSET= @sSET + ',NumeroListaAttesa=''' + @sNumeroListaAttesa + '''' + CHAR(13) + CHAR(10)		
			END					  
		END	
		
		SET @sCodLogin=(SELECT TOP 1 ValoreParametro.CodLogin.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/TimeStamp/CodLogin') as ValoreParametro(CodLogin))
	SET @sCodLogin=ISNULL(@sCodLogin,'')
	
								 
	    SET @xTimeStamp=(SELECT TOP 1 ValoreParametro.TS.query('.')
					  FROM @xParametri.nodes('/Parametri/TimeStamp') as ValoreParametro(TS))		
	
			
					
		IF LEFT(@sSET,1)=',' 
		SET @sSET=RIGHT(@sSET,LEN(@sSET)-1)
	END

	IF @sSET <> ''	
		BEGIN
						
						IF @uIDEpisodio IS NULL
				SET @sWHERE =' WHERE 1=0'
			ELSE
				SET @sWHERE =' WHERE ID=''' + convert(varchar(50),@uIDEpisodio) +''''

						SET @sSQL=	ISNULL(@sSQL,'') + CHAR(13) + CHAR(10) + 
						ISNULL(@sSET,'') + CHAR(13) + CHAR(10) + 
						ISNULL(@sWHERE,'')	

																			
																
						SET @xTimeStamp.modify('delete (/TimeStamp/IDEntita)[1]') 						
			SET @xTimeStamp.modify('insert <IDEntita>{sql:variable("@uIDEpisodio")}</IDEntita> into (/TimeStamp)[1]')
	
						SET @xTimeStamp.modify('delete (/TimeStamp/CodEntita)[1]') 						
			SET @xTimeStamp.modify('insert <CodEntita>EPI</CodEntita> into (/TimeStamp)[1]')
			
										
						SET @xTimeStampBase=@xTimeStamp
	
			SET @xTimeStamp=CONVERT(XML,
										'<Parametri>' + CONVERT(varchar(max),@xTimeStamp) +
										'</Parametri>')
				
												
			SET @xParLog=CONVERT(XML,'<Parametri><LogPrima/><LogDopo/></Parametri>')										
			
			SET @xLogPrima=Convert(XML,'<DataSet></DataSet>')														
			
			SET @xTemp=
				(SELECT * FROM 
					(SELECT  *
					 FROM T_MovEpisodi
					 WHERE ID=@uIDEpisodio										) AS [Table]
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
							(SELECT  *
							 FROM T_MovEpisodi
							 WHERE ID=@uIDEpisodio												) AS [Table]
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