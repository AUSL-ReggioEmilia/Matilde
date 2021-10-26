CREATE PROCEDURE [dbo].[MSP_InsMovEpisodi](@xParametri AS XML)
AS
BEGIN
	
											
		
	DECLARE @sCodAzi AS VARCHAR(20)	
	DECLARE @sCodTipoEpisodio AS VARCHAR(20)	
	DECLARE @sCodStatoEpisodio AS VARCHAR(20)	
	DECLARE @dDataListaAttesa AS DATETIME	
	DECLARE @dDataListaAttesaUTC AS DATETIME	
	DECLARE @dDataRicovero AS DATETIME
	DECLARE @dDataRicoveroUTC AS DATETIME
	DECLARE @dDataDimissione AS DATETIME
	DECLARE @dDataDimissioneUTC AS DATETIME
	DECLARE @sNumeroNosologico AS VARCHAR(20)
	DECLARE @sNumeroListaAttesa AS VARCHAR(20)
		 
		DECLARE @uIDEpisodio AS UNIQUEIDENTIFIER	
	DECLARE @sGUID AS VARCHAR(50)
	DECLARE @uGUID AS UNIQUEIDENTIFIER
	DECLARE @sDataTmp AS VARCHAR(30)				
		
	    DECLARE @xTimeStampBase AS XML	
	DECLARE @xTimeStamp AS XML
	DECLARE @xLogPrima AS XML
	DECLARE @xLogDopo AS XML
	DECLARE @xTemp AS XML
	DECLARE @xParLog AS XML	
	

		SET @sCodAzi=(SELECT TOP 1 ValoreParametro.CodAzi.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodAzi') as ValoreParametro(CodAzi))
	
		SET @sCodTipoEpisodio=(SELECT TOP 1 ValoreParametro.CodTipoEpisodio.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodTipoEpisodio') as ValoreParametro(CodTipoEpisodio))
	
		SET @sCodStatoEpisodio=(SELECT TOP 1 ValoreParametro.CodStatoEpisodio.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodStatoEpisodio') as ValoreParametro(CodStatoEpisodio))
	IF ISNULL(@sCodStatoEpisodio,'')=''
		SET @sCodStatoEpisodio='AT'				
		SET @sNumeroNosologico=(SELECT TOP 1 ValoreParametro.NumeroNosologico.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/NumeroNosologico') as ValoreParametro(NumeroNosologico ))
	
		SET @sNumeroListaAttesa =(SELECT TOP 1 ValoreParametro.NumeroListaAttesa.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/NumeroListaAttesa') as ValoreParametro(NumeroListaAttesa ))
				
		SET @sDataTmp=(SELECT TOP 1 ValoreParametro.DataListaAttesa.value('.','VARCHAR(30)')								  FROM @xParametri.nodes('/Parametri/DataListaAttesa') as ValoreParametro(DataListaAttesa))					  
	SET @sDataTmp=ISNULL(@sDataTmp,'')
	
	IF @sDataTmp<> '' 
		BEGIN			
						SET @sDataTmp= SUBSTRING(@sDataTmp,4,2) + '-' 
						+ LEFT(@sDataTmp,2) + '-' +
						+ SUBSTRING(@sDataTmp,7,4) +
						' ' + RIGHT(@sDataTmp,11)						IF ISDATE(@sDataTmp)=1
				SET	@dDataListaAttesa=CONVERT(DATETIME,@sDataTmp,121)						ELSE
				SET	@dDataListaAttesa =NULL			
		END
		
		SET @sDataTmp=(SELECT TOP 1 ValoreParametro.DataListaAttesaUTC .value('.','VARCHAR(30)')						  FROM @xParametri.nodes('/Parametri/DataListaAttesaUTC ') as ValoreParametro(DataListaAttesaUTC ))					  
	SET @sDataTmp=ISNULL(@sDataTmp,'')
	
	IF @sDataTmp<> '' 
		BEGIN			
						SET @sDataTmp= SUBSTRING(@sDataTmp,4,2) + '-' 
						+ LEFT(@sDataTmp,2) + '-' +
						+ SUBSTRING(@sDataTmp,7,4) +
						' ' + RIGHT(@sDataTmp,11)						IF ISDATE(@sDataTmp)=1
				SET	@dDataListaAttesaUTC =CONVERT(DATETIME,@sDataTmp,121)				ELSE
				SET	@dDataListaAttesaUTC  =NULL			
		END
			
		SET @sDataTmp=(SELECT TOP 1 ValoreParametro.DataRicovero.value('.','VARCHAR(30)')						  FROM @xParametri.nodes('/Parametri/DataRicovero') as ValoreParametro(DataRicovero))					  
	SET @sDataTmp=ISNULL(@sDataTmp,'')
	

	IF @sDataTmp<> '' 
		BEGIN			
						SET @sDataTmp= SUBSTRING(@sDataTmp,4,2) + '-' 
						+ LEFT(@sDataTmp,2) + '-' +
						+ SUBSTRING(@sDataTmp,7,4) +
						' ' + RIGHT(@sDataTmp,11)						IF ISDATE(@sDataTmp)=1
				SET	@dDataRicovero=CONVERT(DATETIME,@sDataTmp,121)						ELSE
				SET	@dDataRicovero=NULL			
		END	
	
		SET @sDataTmp=(SELECT TOP 1 ValoreParametro.DataRicoveroUTC.value('.','VARCHAR(30)')								  FROM @xParametri.nodes('/Parametri/DataRicoveroUTC') as ValoreParametro(DataRicoveroUTC))					  
	SET @sDataTmp=ISNULL(@sDataTmp,'')
	

	IF @sDataTmp<> '' 
		BEGIN			
						SET @sDataTmp= SUBSTRING(@sDataTmp,4,2) + '-' 
						+ LEFT(@sDataTmp,2) + '-' +
						+ SUBSTRING(@sDataTmp,7,4) +
						' ' + RIGHT(@sDataTmp,11)						IF ISDATE(@sDataTmp)=1
				SET	@dDataRicoveroUTC=CONVERT(DATETIME,@sDataTmp,121)						ELSE
				SET	@dDataRicoveroUTC=NULL			
		END
		
		SET @sDataTmp=(SELECT TOP 1 ValoreParametro.DataDimissione.value('.','VARCHAR(30)') 								  FROM @xParametri.nodes('/Parametri/DataDimissione') as ValoreParametro(DataDimissione))					  
	SET @sDataTmp=ISNULL(@sDataTmp,'')
	

	IF @sDataTmp<> '' 
		BEGIN			
						SET @sDataTmp= SUBSTRING(@sDataTmp,4,2) + '-' 
						+ LEFT(@sDataTmp,2) + '-' +
						+ SUBSTRING(@sDataTmp,7,4) +
						' ' + RIGHT(@sDataTmp,11)						IF ISDATE(@sDataTmp)=1
				SET	@dDataDimissione=CONVERT(DATETIME,@sDataTmp,121)					ELSE
				SET	@dDataDimissione =NULL			
		END
		
		SET @sDataTmp=(SELECT TOP 1 ValoreParametro.DataDimissioneUTC.value('.','VARCHAR(30)')								  FROM @xParametri.nodes('/Parametri/DataDimissioneUTC') as ValoreParametro(DataDimissioneUTC))					  
	SET @sDataTmp=ISNULL(@sDataTmp,'')
	

	IF @sDataTmp<> '' 
		BEGIN			
						SET @sDataTmp= SUBSTRING(@sDataTmp,4,2) + '-' 
						+ LEFT(@sDataTmp,2) + '-' +
						+ SUBSTRING(@sDataTmp,7,4) +
						' ' + RIGHT(@sDataTmp,11)						IF ISDATE(@sDataTmp)=1
				SET	@dDataDimissioneUTC=CONVERT(DATETIME,@sDataTmp,121)					ELSE
				SET	@dDataDimissioneUTC =NULL			
		END
	
	    SET @xTimeStamp=(SELECT TOP 1 ValoreParametro.TS.query('.')
					  FROM @xParametri.nodes('/Parametri/TimeStamp') as ValoreParametro(TS))
		
					
	SET @uGUID=NEWID()
	
	BEGIN TRANSACTION
		INSERT INTO T_MovEpisodi
			   (ID
			   ,CodAzi
			   ,CodTipoEpisodio
			   ,CodStatoEpisodio
			   ,DataListaAttesa
			   ,DataListaAttesaUTC
			   ,DataRicovero
			   ,DataRicoveroUTC
			   ,DataDimissione
			   ,DataDimissioneUTC
			   ,NumeroNosologico
			   ,NumeroListaAttesa
			   )
		VALUES
			(
				@uGUID								   ,@sCodAzi							   ,@sCodTipoEpisodio					   ,@sCodStatoEpisodio					   ,@dDataListaAttesa					   ,@dDataListaAttesaUTC				   ,@dDataRicovero						   ,@dDataRicoveroUTC					   ,@dDataDimissione					   ,@dDataDimissioneUTC					   ,@sNumeroNosologico					   ,@sNumeroListaAttesa					 )  
	      
		IF @@ERROR=0 
		BEGIN
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
					(SELECT * FROM T_MovEpisodi
					 WHERE ID=@uGUID
					) AS [Table]
				FOR XML AUTO, ELEMENTS)

			SET @xLogDopo.modify('insert sql:variable("@xTemp") as first into (/DataSet)[1]')				
			
			SET @xParLog=CONVERT(XML,'<Parametri><LogDopo/></Parametri>')			
			SET @xParLog.modify('insert sql:variable("@xLogDopo") as first into (/Parametri/LogDopo)[1]')
			SET @xParLog.modify('insert sql:variable("@xTimeStampBase") as last into (/Parametri)[1]')
						
			EXEC MSP_InsMovLog @xParLog
									
			SELECT @uGUID AS IDEpisodio
		END	
	ELSE
		BEGIN
			ROLLBACK TRANSACTION	
			PRINT 'ERRORE'
			SELECT NULL AS IDEpisodio
		END	 
				
	RETURN 0
END