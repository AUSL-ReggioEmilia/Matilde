CREATE PROCEDURE [dbo].[MSP_AggMovTrasferimenti](@xParametri XML)
AS
BEGIN

									 	
	
				
		DECLARE @uIDTrasferimento AS UNIQUEIDENTIFIER
	DECLARE @uIDEpisodio AS UNIQUEIDENTIFIER
	DECLARE @nSequenza AS INTEGER
	DECLARE @sCodUA AS VARCHAR(20)		
	DECLARE @sCodStatoTrasferimento AS VARCHAR(20)		
	DECLARE @dDataIngresso AS DATETIME	
	DECLARE @dDataIngressoUTC AS DATETIME		
	DECLARE @dDataUscita AS DATETIME	
	DECLARE @dDataUscitaUTC AS DATETIME		
	DECLARE @sCodUO AS VARCHAR(20)		
	DECLARE @sDescrUO AS VARCHAR(255)			
	DECLARE @sCodSettore AS VARCHAR(20)		
	DECLARE @sDescrSettore AS VARCHAR(255)		 
	DECLARE @sCodStanza AS VARCHAR(20)		
	DECLARE @sDescrStanza AS VARCHAR(255)	
	DECLARE @sCodLetto AS VARCHAR(20)		
	DECLARE @sDescrLetto AS VARCHAR(255)	
	DECLARE @uIDCartella AS UNIQUEIDENTIFIER
	
		
					
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
	
		
	

						
		SET @sSQL='UPDATE T_MovTrasferimenti ' + CHAR(13) + CHAR(10) +
			  'SET '
			  					
	SET @sSET =''	
	
		IF @xParametri.exist('/Parametri/IDTrasferimento')=1
		BEGIN
			SET @sGUID=(SELECT TOP 1 ValoreParametro.IDTrasferimento.value('.','VARCHAR(50)')
							  FROM @xParametri.nodes('/Parametri/IDTrasferimento') as ValoreParametro(IDTrasferimento))
							  
				IF 	ISNULL(@sGUID,'') <> '' 				
					SET @uIDTrasferimento=CONVERT(UNIQUEIDENTIFIER,	@sGUID)		
				ELSE
					SET @uIDTrasferimento=NULL
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

		IF @xParametri.exist('/Parametri/Sequenza')=1
		BEGIN
			SET @nSequenza=(SELECT TOP 1 ValoreParametro.Sequenza.value('.','INTEGER')
							  FROM @xParametri.nodes('/Parametri/Sequenza') as ValoreParametro(Sequenza))
			IF @nSequenza <> ''
			BEGIN				
				SET	@sSET= @sSET + ',Sequenza=''' + @nSequenza + '''' + CHAR(13) + CHAR(10)		
			END	
		ELSE
			SET @sSET= @sSET + ',Sequenza=NULL ' + CHAR(13) + CHAR(10)				  
		END		
	
		IF @xParametri.exist('/Parametri/CodUA')=1
		BEGIN
			SET @sCodUA=(SELECT TOP 1 ValoreParametro.CodUA.value('.','VARCHAR(20)')
							  FROM @xParametri.nodes('/Parametri/CodUA') as ValoreParametro(CodUA))
			IF @sCodUA <> ''
			BEGIN				
				SET	@sSET= @sSET + ',CodUA=''' + @sCodUA + '''' + CHAR(13) + CHAR(10)		
			END	
		ELSE
			SET @sSET= @sSET + ',CodUA=NULL ' + CHAR(13) + CHAR(10)				  
		END		
	
		IF @xParametri.exist('/Parametri/CodStatoTrasferimento')=1
		BEGIN
			SET @sCodStatoTrasferimento=(SELECT TOP 1 ValoreParametro.CodStatoTrasferimento.value('.','VARCHAR(20)')
							  FROM @xParametri.nodes('/Parametri/CodStatoTrasferimento') as ValoreParametro(CodStatoTrasferimento))
			IF @sCodStatoTrasferimento <> ''
			BEGIN				
				SET	@sSET= @sSET + ',CodStatoTrasferimento=''' + @sCodStatoTrasferimento + '''' + CHAR(13) + CHAR(10)		
			END	
		ELSE
			SET @sSET= @sSET + ',CodStatoTrasferimento=NULL ' + CHAR(13) + CHAR(10)				  
		END		

		IF @xParametri.exist('/Parametri/DataIngresso')=1
		BEGIN			
			SET @sDataTmp=(SELECT TOP 1 ValoreParametro.DataIngresso.value('.','VARCHAR(30)')											  FROM @xParametri.nodes('/Parametri/DataIngresso') as ValoreParametro(DataIngresso))					  
			SET @sDataTmp=ISNULL(@sDataTmp,'')
			
			IF @sDataTmp<> '' 
				BEGIN			
											SET @sDataTmp= SUBSTRING(@sDataTmp,4,2) + '-' 
						+ LEFT(@sDataTmp,2) + '-' +
						+ SUBSTRING(@sDataTmp,7,4) +
						' ' + RIGHT(@sDataTmp,11)								IF ISDATE(@sDataTmp)=1					
						SET	@dDataIngresso=CONVERT(DATETIME,@sDataTmp,121)											
					ELSE
						SET	@dDataIngresso =NULL			
				END
			IF @dDataIngresso IS NOT NULL
				SET	@sSET= @sSET + ',DataIngresso=CONVERT(DateTime,''' + CONVERT(VARCHAR(30),@dDataIngresso,121) + ''',121)' + CHAR(13) + CHAR(10)				ELSE	
				SET	@sSET= @sSET + ',DataIngresso=NULL' + CHAR(13) + CHAR(10)		 
		END	
	
		IF @xParametri.exist('/Parametri/DataIngressoUTC')=1
		BEGIN			
			SET @sDataTmp=(SELECT TOP 1 ValoreParametro.DataIngressoUTC.value('.','VARCHAR(30)')											  FROM @xParametri.nodes('/Parametri/DataIngressoUTC') as ValoreParametro(DataIngressoUTC))					  
			SET @sDataTmp=ISNULL(@sDataTmp,'')
			
			IF @sDataTmp<> '' 
				BEGIN			
											SET @sDataTmp= SUBSTRING(@sDataTmp,4,2) + '-' 
						+ LEFT(@sDataTmp,2) + '-' +
						+ SUBSTRING(@sDataTmp,7,4) +
						' ' + RIGHT(@sDataTmp,11)								IF ISDATE(@sDataTmp)=1					
						SET	@dDataIngressoUTC=CONVERT(DATETIME,@sDataTmp,121)											
					ELSE
						SET	@dDataIngressoUTC =NULL			
				END
			IF @dDataIngressoUTC IS NOT NULL
				SET	@sSET= @sSET + ',DataIngressoUTC=CONVERT(DateTime,''' + CONVERT(VARCHAR(30),@dDataIngressoUTC,121) + ''',121)' + CHAR(13) + CHAR(10)				ELSE	
				SET	@sSET= @sSET + ',DataIngressoUTC=NULL' + CHAR(13) + CHAR(10)		 
		END	
	
		IF @xParametri.exist('/Parametri/DataUscita')=1
		BEGIN			
			SET @sDataTmp=(SELECT TOP 1 ValoreParametro.DataUscita.value('.','VARCHAR(30)')											  FROM @xParametri.nodes('/Parametri/DataUscita') as ValoreParametro(DataUscita))					  
			SET @sDataTmp=ISNULL(@sDataTmp,'')
			
			IF @sDataTmp<> '' 
				BEGIN			
											SET @sDataTmp= SUBSTRING(@sDataTmp,4,2) + '-' 
						+ LEFT(@sDataTmp,2) + '-' +
						+ SUBSTRING(@sDataTmp,7,4) +
						' ' + RIGHT(@sDataTmp,11)								IF ISDATE(@sDataTmp)=1					
						SET	@dDataUscita=CONVERT(DATETIME,@sDataTmp,121)											
					ELSE
						SET	@dDataUscita =NULL			
				END
			IF @dDataUscita IS NOT NULL
				SET	@sSET= @sSET + ',DataUscita=CONVERT(DateTime,''' + CONVERT(VARCHAR(30),@dDataUscita,121) + ''',121)' + CHAR(13) + CHAR(10)				ELSE	
				SET	@sSET= @sSET + ',DataUscita=NULL' + CHAR(13) + CHAR(10)		 
		END	
	
		IF @xParametri.exist('/Parametri/DataUscitaUTC')=1
		BEGIN			
			SET @sDataTmp=(SELECT TOP 1 ValoreParametro.DataUscitaUTC.value('.','VARCHAR(30)')											  FROM @xParametri.nodes('/Parametri/DataUscitaUTC') as ValoreParametro(DataUscitaUTC))					  
			SET @sDataTmp=ISNULL(@sDataTmp,'')
			
			IF @sDataTmp<> '' 
				BEGIN			
											SET @sDataTmp= SUBSTRING(@sDataTmp,4,2) + '-' 
						+ LEFT(@sDataTmp,2) + '-' +
						+ SUBSTRING(@sDataTmp,7,4) +
						' ' + RIGHT(@sDataTmp,11)								IF ISDATE(@sDataTmp)=1					
						SET	@dDataUscitaUTC=CONVERT(DATETIME,@sDataTmp,121)											
					ELSE
						SET	@dDataUscitaUTC =NULL			
				END
			IF @dDataUscitaUTC IS NOT NULL
				SET	@sSET= @sSET + ',DataUscitaUTC=CONVERT(DateTime,''' + CONVERT(VARCHAR(30),@dDataUscitaUTC,121) + ''',121)' + CHAR(13) + CHAR(10)				ELSE	
				SET	@sSET= @sSET + ',DataUscitaUTC=NULL' + CHAR(13) + CHAR(10)		 
		END	
	
		IF @xParametri.exist('/Parametri/CodUO')=1
		BEGIN
			SET @sCodUO=(SELECT TOP 1 ValoreParametro.CodUO.value('.','VARCHAR(20)')
							  FROM @xParametri.nodes('/Parametri/CodUO') as ValoreParametro(CodUO))
			IF @sCodUO <> ''
			BEGIN				
				SET	@sSET= @sSET + ',CodUO=''' + @sCodUO + '''' + CHAR(13) + CHAR(10)		
			END	
		ELSE
			SET @sSET= @sSET + ',CodUO=NULL ' + CHAR(13) + CHAR(10)				  
		END		
	
		IF @xParametri.exist('/Parametri/DescrUO')=1
		BEGIN
			SET @sDescrUO=(SELECT TOP 1 ValoreParametro.DescrUO.value('.','VARCHAR(255)')
							  FROM @xParametri.nodes('/Parametri/DescrUO') as ValoreParametro(DescrUO))
			IF @sDescrUO <> ''
			BEGIN				
				SET	@sSET= @sSET + ',DescrUO=''' + @sDescrUO + '''' + CHAR(13) + CHAR(10)		
			END	
		ELSE
			SET @sSET= @sSET + ',DescrUO=NULL ' + CHAR(13) + CHAR(10)				  
		END	
	
		IF @xParametri.exist('/Parametri/CodSettore')=1
		BEGIN
			SET @sCodSettore=(SELECT TOP 1 ValoreParametro.CodSettore.value('.','VARCHAR(20)')
							  FROM @xParametri.nodes('/Parametri/CodSettore') as ValoreParametro(CodSettore))
			IF @sCodSettore <> ''
			BEGIN				
				SET	@sSET= @sSET + ',CodSettore=''' + @sCodSettore + '''' + CHAR(13) + CHAR(10)		
			END	
		ELSE
			SET @sSET= @sSET + ',CodSettore=NULL ' + CHAR(13) + CHAR(10)				  
		END		
	
		IF @xParametri.exist('/Parametri/DescrSettore')=1
		BEGIN
			SET @sDescrSettore=(SELECT TOP 1 ValoreParametro.DescrSettore.value('.','VARCHAR(255)')
							  FROM @xParametri.nodes('/Parametri/DescrSettore') as ValoreParametro(DescrSettore))
			IF @sDescrSettore <> ''
			BEGIN				
				SET	@sSET= @sSET + ',DescrSettore=''' + @sDescrSettore + '''' + CHAR(13) + CHAR(10)		
			END	
		ELSE
			SET @sSET= @sSET + ',DescrSettore=NULL ' + CHAR(13) + CHAR(10)				  
		END	
	
		IF @xParametri.exist('/Parametri/CodStanza')=1
		BEGIN
			SET @sCodStanza=(SELECT TOP 1 ValoreParametro.CodStanza.value('.','VARCHAR(20)')
							  FROM @xParametri.nodes('/Parametri/CodStanza') as ValoreParametro(CodStanza))
			IF @sCodStanza <> ''
			BEGIN				
				SET	@sSET= @sSET + ',CodStanza=''' + @sCodStanza + '''' + CHAR(13) + CHAR(10)		
			END	
		ELSE
			SET @sSET= @sSET + ',CodStanza=NULL ' + CHAR(13) + CHAR(10)				  
		END		
	
		IF @xParametri.exist('/Parametri/DescrStanza')=1
		BEGIN
			SET @sDescrStanza=(SELECT TOP 1 ValoreParametro.DescrStanza.value('.','VARCHAR(255)')
							  FROM @xParametri.nodes('/Parametri/DescrStanza') as ValoreParametro(DescrStanza))
			IF @sDescrStanza <> ''
			BEGIN				
				SET	@sSET= @sSET + ',DescrStanza=''' + @sDescrStanza + '''' + CHAR(13) + CHAR(10)		
			END	
		ELSE
			SET @sSET= @sSET + ',DescrStanza=NULL ' + CHAR(13) + CHAR(10)				  
		END		
	
		
		IF @xParametri.exist('/Parametri/CodLetto')=1
		BEGIN
			SET @sCodLetto=(SELECT TOP 1 ValoreParametro.CodLetto.value('.','VARCHAR(20)')
							  FROM @xParametri.nodes('/Parametri/CodLetto') as ValoreParametro(CodLetto))
			IF @sCodLetto <> ''
			BEGIN				
				SET	@sSET= @sSET + ',CodLetto=''' + @sCodLetto + '''' + CHAR(13) + CHAR(10)		
			END	
		ELSE
			SET @sSET= @sSET + ',CodLetto=NULL ' + CHAR(13) + CHAR(10)				  
		END		
	
		IF @xParametri.exist('/Parametri/DescrLetto')=1
		BEGIN
			SET @sDescrLetto=(SELECT TOP 1 ValoreParametro.DescrLetto.value('.','VARCHAR(255)')
							  FROM @xParametri.nodes('/Parametri/DescrLetto') as ValoreParametro(DescrLetto))
			IF @sDescrLetto <> ''
			BEGIN				
				SET	@sSET= @sSET + ',DescrLetto=''' + @sDescrLetto + '''' + CHAR(13) + CHAR(10)		
			END	
		ELSE
			SET @sSET= @sSET + ',DescrLetto=NULL ' + CHAR(13) + CHAR(10)				  
		END		
	
		IF @xParametri.exist('/Parametri/IDCartella')=1
		BEGIN	
		
			SET @sGUID=(SELECT TOP 1 ValoreParametro.IDCartella.value('.','VARCHAR(50)')
				  FROM @xParametri.nodes('/Parametri/IDCartella') as ValoreParametro(IDCartella))

			IF 	ISNULL(@sGUID,'') <> '' 
				BEGIN
					SET @uIDCartella=CONVERT(UNIQUEIDENTIFIER,	@sGUID)		
					SET	@sSET= @sSET + ',IDCartella=''' + convert(VARCHAR(50),@uIDCartella) + ''''	+ CHAR(13) + CHAR(10)	
				END
			ELSE
				SET	@sSET= @sSET + ',IDCartella=NULL'	+ CHAR(13) + CHAR(10)					
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
						
			IF (@uIDTrasferimento IS NULL)
			BEGIN
				SET @sWHERE =' WHERE 1=0'
			END
			ELSE
			BEGIN
								SET @sWHERE =' WHERE ID=''' + convert(varchar(50),@uIDTrasferimento) +''''
			END
			
						SET @sSQL=	ISNULL(@sSQL,'') + CHAR(13) + CHAR(10) + 
						ISNULL(@sSET,'') + CHAR(13) + CHAR(10) + 
						ISNULL(@sWHERE,'')	

																			
																
						SET @xTimeStamp.modify('delete (/TimeStamp/IDEntita)[1]') 						
			SET @xTimeStamp.modify('insert <IDEntita>{sql:variable("@uIDTrasferimento")}</IDEntita> into (/TimeStamp)[1]')
	
						SET @xTimeStamp.modify('delete (/TimeStamp/CodEntita)[1]') 						
			SET @xTimeStamp.modify('insert <CodEntita>TRA</CodEntita> into (/TimeStamp)[1]')
			
										
						SET @xTimeStamp.modify('delete (/TimeStamp/IDTrasferimento)[1]') 						
			SET @xTimeStamp.modify('insert <IDTrasferimento>{sql:variable("@uIDTrasferimento")}</IDTrasferimento> into (/TimeStamp)[1]')
			
						SET @xTimeStampBase=@xTimeStamp
	
			SET @xTimeStamp=CONVERT(XML,
										'<Parametri>' + CONVERT(varchar(max),@xTimeStamp) +
										'</Parametri>')
				
												
			SET @xParLog=CONVERT(XML,'<Parametri><LogPrima/><LogDopo/></Parametri>')										
			
			SET @xLogPrima=Convert(XML,'<DataSet></DataSet>')														
			
			SET @xTemp=
				(SELECT * FROM 
					(SELECT  *
					 FROM T_MovTrasferimenti
					 WHERE ID=@uIDTrasferimento										) AS [Table]
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
							 FROM T_MovTrasferimenti
							 WHERE ID=@uIDTrasferimento													) AS [Table]
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