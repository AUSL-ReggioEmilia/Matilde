CREATE PROCEDURE [dbo].[MSP_InsMovTrasferimenti](@xParametri AS XML)
AS
BEGIN
	
											
						
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
	DECLARE @sCodAziTrasferimento AS VARCHAR(20)		
	 
		DECLARE @uIDTrasferimento AS UNIQUEIDENTIFIER	
	DECLARE @sGUID AS VARCHAR(50)
	DECLARE @uGUID AS UNIQUEIDENTIFIER
	DECLARE @sDataTmp AS VARCHAR(30)			
		
	    DECLARE @xTimeStampBase AS XML	
	DECLARE @xTimeStamp AS XML
	DECLARE @xLogPrima AS XML
	DECLARE @xLogDopo AS XML
	DECLARE @xTemp AS XML
	DECLARE @xParLog AS XML	
	
		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDEpisodio.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDEpisodio') as ValoreParametro(IDEpisodio))
	IF 	ISNULL(@sGUID,'') <> '' 
			SET @uIDEpisodio=CONVERT(UNIQUEIDENTIFIER,	@sGUID)	
			
		SET @nSequenza=(SELECT TOP 1 ValoreParametro.Sequenza.value('.','INTEGER')
					  FROM @xParametri.nodes('/Parametri/Sequenza') as ValoreParametro(Sequenza))
	IF ISNULL(@nSequenza,-1) =-1 SET @nSequenza=0
	
		SET @sCodUA=(SELECT TOP 1 ValoreParametro.CodUA.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodUA') as ValoreParametro(CodUA))
	
		SET @sCodStatoTrasferimento=(SELECT TOP 1 ValoreParametro.CodStatoTrasferimento.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodStatoTrasferimento') as ValoreParametro(CodStatoTrasferimento))
	IF ISNULL(@sCodStatoTrasferimento,'')='' 
		SET @sCodStatoTrasferimento='AT'			
		SET @sDataTmp=(SELECT TOP 1 ValoreParametro.DataIngresso.value('.','VARCHAR(30)')								  FROM @xParametri.nodes('/Parametri/DataIngresso') as ValoreParametro(DataIngresso))					  
	SET @sDataTmp=ISNULL(@sDataTmp,'')
	
	IF @sDataTmp<> '' 
		BEGIN			
						SET @sDataTmp= SUBSTRING(@sDataTmp,4,2) + '-' 
						+ LEFT(@sDataTmp,2) + '-' +
						+ SUBSTRING(@sDataTmp,7,4) +
						' ' + RIGHT(@sDataTmp,11)						IF ISDATE(@sDataTmp)=1
				SET	@dDataIngresso=CONVERT(DATETIME,@sDataTmp,121)						ELSE
				SET	@dDataIngresso =NULL			
		END
	
		SET @sDataTmp=(SELECT TOP 1 ValoreParametro.DataIngressoUTC.value('.','VARCHAR(30)')								  FROM @xParametri.nodes('/Parametri/DataIngressoUTC') as ValoreParametro(DataIngressoUTC))					  
	SET @sDataTmp=ISNULL(@sDataTmp,'')
	
	IF @sDataTmp<> '' 
		BEGIN			
						SET @sDataTmp= SUBSTRING(@sDataTmp,4,2) + '-' 
						+ LEFT(@sDataTmp,2) + '-' +
						+ SUBSTRING(@sDataTmp,7,4) +
						' ' + RIGHT(@sDataTmp,11)						IF ISDATE(@sDataTmp)=1
				SET	@dDataIngressoUTC=CONVERT(DATETIME,@sDataTmp,121)						ELSE
				SET	@dDataIngressoUTC =NULL			
		END
	
		SET @sDataTmp=(SELECT TOP 1 ValoreParametro.DataUscita.value('.','VARCHAR(30)')								  FROM @xParametri.nodes('/Parametri/DataUscita') as ValoreParametro(DataUscita))					  
	SET @sDataTmp=ISNULL(@sDataTmp,'')
	
	IF @sDataTmp<> '' 
		BEGIN			
						SET @sDataTmp= SUBSTRING(@sDataTmp,4,2) + '-' 
						+ LEFT(@sDataTmp,2) + '-' +
						+ SUBSTRING(@sDataTmp,7,4) +
						' ' + RIGHT(@sDataTmp,11)						IF ISDATE(@sDataTmp)=1
				SET	@dDataUscita=CONVERT(DATETIME,@sDataTmp,121)						ELSE
				SET	@dDataUscita =NULL			
		END
	
		SET @sDataTmp=(SELECT TOP 1 ValoreParametro.DataUscitaUTC.value('.','VARCHAR(30)')								  FROM @xParametri.nodes('/Parametri/DataUscitaUTC') as ValoreParametro(DataUscitaUTC))					  
	SET @sDataTmp=ISNULL(@sDataTmp,'')
	
	IF @sDataTmp<> '' 
		BEGIN			
						SET @sDataTmp= SUBSTRING(@sDataTmp,4,2) + '-' 
						+ LEFT(@sDataTmp,2) + '-' +
						+ SUBSTRING(@sDataTmp,7,4) +
						' ' + RIGHT(@sDataTmp,11)						IF ISDATE(@sDataTmp)=1
				SET	@dDataUscitaUTC=CONVERT(DATETIME,@sDataTmp,121)						ELSE
				SET	@dDataUscitaUTC =NULL			
		END
	
		SET @sCodUO=(SELECT TOP 1 ValoreParametro.CodUO.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodUO') as ValoreParametro(CodUO))
	
		SET @sDescrUO=(SELECT TOP 1 ValoreParametro.DescrUO.value('.','VARCHAR(255)')
					  FROM @xParametri.nodes('/Parametri/DescrUO') as ValoreParametro(DescrUO))
	IF ISNULL(@sDescrUO,'')<>''
		BEGIN
						SET @sDescrUO=REPLACE(@sDescrUO,'''''','''')
		END
		SET @sCodSettore=(SELECT TOP 1 ValoreParametro.CodSettore.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodSettore') as ValoreParametro(CodSettore))
	
		SET @sDescrSettore=(SELECT TOP 1 ValoreParametro.DescrSettore.value('.','VARCHAR(255)')
					  FROM @xParametri.nodes('/Parametri/DescrSettore') as ValoreParametro(DescrSettore))
	
		SET @sCodStanza=(SELECT TOP 1 ValoreParametro.CodStanza.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodStanza') as ValoreParametro(CodStanza))
	
		SET @sDescrStanza=(SELECT TOP 1 ValoreParametro.DescrStanza.value('.','VARCHAR(255)')
					  FROM @xParametri.nodes('/Parametri/DescrStanza') as ValoreParametro(DescrStanza))
					  
		SET @sCodLetto=(SELECT TOP 1 ValoreParametro.CodLetto.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodLetto') as ValoreParametro(CodLetto))
	
		SET @sDescrLetto=(SELECT TOP 1 ValoreParametro.DescrLetto.value('.','VARCHAR(255)')
					  FROM @xParametri.nodes('/Parametri/DescrLetto') as ValoreParametro(DescrLetto))
	
		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDCartella.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDCartella') as ValoreParametro(IDCartella))
	IF 	ISNULL(@sGUID,'') <> '' 
			SET @uIDCartella=CONVERT(UNIQUEIDENTIFIER,	@sGUID)	
	ELSE
		SET @uIDCartella=NULL
	
		SET @sCodAziTrasferimento=(SELECT TOP 1 ValoreParametro.CodAziTrasferimento.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodAziTrasferimento') as ValoreParametro(CodAziTrasferimento))
					  							  				  				  				  
	    SET @xTimeStamp=(SELECT TOP 1 ValoreParametro.TS.query('.')
					  FROM @xParametri.nodes('/Parametri/TimeStamp') as ValoreParametro(TS))
		
					
	SET @uGUID=NEWID()

		IF ISNULL(@sCodAziTrasferimento,'')=''
	BEGIN
		SET @sCodAziTrasferimento=(SELECT CodAzi FROM T_MovEpisodi WHERE ID=@uIDEpisodio)
	END
	
	BEGIN TRANSACTION
		INSERT INTO T_MovTrasferimenti
			   (	
					ID
				   ,IDEpisodio
				   ,Sequenza
				   ,CodUA
				   ,CodStatoTrasferimento
				   ,DataIngresso
				   ,DataIngressoUTC
				   ,DataUscita
				   ,DataUscitaUTC
				   ,CodUO
				   ,DescrUO
				   ,CodSettore
				   ,DescrSettore
				   ,CodStanza
				   ,DescrStanza
				   ,CodLetto
				   ,DescrLetto
				   ,IDCartella
				   ,CodAziTrasferimento
			   )
		VALUES
			(
					@uGUID
				   ,@uIDEpisodio								   ,@nSequenza									   ,@sCodUA										   ,@sCodStatoTrasferimento						   ,@dDataIngresso								   ,@dDataIngressoUTC							   ,@dDataUscita								   ,@dDataUscitaUTC								   ,@sCodUO										   ,@sDescrUO									   ,@sCodSettore								   ,@sDescrSettore								   ,@sCodStanza									   ,@sDescrStanza								   ,@sCodLetto									   ,@sDescrLetto								   ,@uIDCartella								   ,@sCodAziTrasferimento					 )  
	      
		IF @@ERROR=0 
		BEGIN
												
						SET @xTimeStamp.modify('delete (/TimeStamp/CodEntita)[1]') 					
			SET @xTimeStamp.modify('insert <CodEntita>TRA</CodEntita> into (/TimeStamp)[1]')
			
						SET @xTimeStamp.modify('delete (/TimeStamp/IDEntita)[1]') 					
			SET @xTimeStamp.modify('insert <IDEntita>{sql:variable("@uGUID")}</IDEntita> into (/TimeStamp)[1]')

						SET @xTimeStamp.modify('delete (/TimeStamp/IDEpisodio)[1]') 					
			SET @xTimeStamp.modify('insert <IDEpisodio>{sql:variable("@uGUID")}</IDEpisodio> into (/TimeStamp)[1]')
			
						SET @xTimeStampBase=@xTimeStamp


			SET @xTimeStamp=CONVERT(XML,
								'<Parametri>' + CONVERT(varchar(max),@xTimeStamp) +
								'</Parametri>')


						EXEC MSP_InsMovTimeStamp @xTimeStamp		
		END	
		
		IF @@ERROR = 0
		BEGIN
			COMMIT TRANSACTION
			
											
			
			SET @xLogDopo=Convert(XML,'<DataSet></DataSet>')
			SET @xLogPrima=''
			
			SET @xTemp=
				(SELECT * FROM 
					(SELECT * FROM T_MovTrasferimenti
					 WHERE ID=@uGUID
					) AS [Table]
				FOR XML AUTO, ELEMENTS)

			SET @xLogDopo.modify('insert sql:variable("@xTemp") as first into (/DataSet)[1]')				
			
			SET @xParLog=CONVERT(XML,'<Parametri><LogDopo/></Parametri>')			
			SET @xParLog.modify('insert sql:variable("@xLogDopo") as first into (/Parametri/LogDopo)[1]')
			SET @xParLog.modify('insert sql:variable("@xTimeStampBase") as last into (/Parametri)[1]')
						
			EXEC MSP_InsMovLog @xParLog
									
			SELECT @uGUID AS IDTrasferimento
		END	
	ELSE
		BEGIN
			ROLLBACK TRANSACTION	
			PRINT 'ERRORE'
			SELECT NULL AS IDTrasferimento
		END	 
				
	RETURN 0
END