CREATE PROCEDURE [dbo].[MSP_InsMovPrescrizioni](@xParametri XML)
AS
BEGIN
		
	
									
		DECLARE @uIDEpisodio AS UNIQUEIDENTIFIER	
	DECLARE @uIDTrasferimento AS UNIQUEIDENTIFIER	
	DECLARE @dDataEvento AS DATETIME	
	DECLARE @dDataEventoUTC AS DATETIME
	
	DECLARE @sCodViaSomministrazione AS VARCHAR(20)
	DECLARE @sCodTipoPrescrizione AS VARCHAR(20)
	DECLARE @sCodStatoPrescrizione AS VARCHAR(20)
	DECLARE @sCodStatoContinuazione AS VARCHAR(20)
	
	DECLARE @sCodUtenteRilevazione AS VARCHAR(100)					
	
		DECLARE @sGUID AS VARCHAR(50)
	DECLARE @uGUID AS UNIQUEIDENTIFIER
	DECLARE @sDataTmp AS VARCHAR(20)	
	
		
	    DECLARE @xTimeStampBase AS XML	
	DECLARE @xTimeStamp AS XML	
		
		DECLARE @xLogPrima AS XML
	DECLARE @xLogDopo AS XML
	DECLARE @xTemp AS XML
	DECLARE @xParLog AS XML

		DECLARE @xSchedaMovimento AS XML	


		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDEpisodio.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDEpisodio') as ValoreParametro(IDEpisodio))
	IF 	ISNULL(@sGUID,'') <> '' 
			SET @uIDEpisodio=CONVERT(UNIQUEIDENTIFIER,	@sGUID)		
		
		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDTrasferimento.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDTrasferimento') as ValoreParametro(IDTrasferimento))
	IF 	ISNULL(@sGUID,'') <> '' 
			SET @uIDTrasferimento=CONVERT(UNIQUEIDENTIFIER,	@sGUID)	

		SET @sCodViaSomministrazione=(SELECT TOP 1 ValoreParametro.CodViaSomministrazione.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodViaSomministrazione') as ValoreParametro(CodViaSomministrazione))	
	SET @sCodViaSomministrazione=ISNULL(@sCodViaSomministrazione,'')
	
		SET @sCodTipoPrescrizione=(SELECT TOP 1 ValoreParametro.CodTipoPrescrizione.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodTipoPrescrizione') as ValoreParametro(CodTipoPrescrizione))	
	SET @sCodTipoPrescrizione=ISNULL(@sCodTipoPrescrizione,'')
	
		SET @sCodStatoPrescrizione=(SELECT TOP 1 ValoreParametro.CodStatoPrescrizione.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodStatoPrescrizione') as ValoreParametro(CodStatoPrescrizione))	
	SET @sCodStatoPrescrizione=ISNULL(@sCodStatoPrescrizione,'')
	IF @sCodStatoPrescrizione='' SET @sCodStatoPrescrizione='IC'							

		SET @sCodStatoContinuazione=(SELECT TOP 1 ValoreParametro.CodStatoContinuazione.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodStatoContinuazione') as ValoreParametro(CodStatoContinuazione))	
	SET @sCodStatoContinuazione=ISNULL(@sCodStatoContinuazione,'')
	IF @sCodStatoContinuazione='' SET @sCodStatoContinuazione='AP'			
	
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
							
		SET @sCodUtenteRilevazione=(SELECT TOP 1 ValoreParametro.CodUtenteRilevazione.value('.','VARCHAR(100)')
					  FROM @xParametri.nodes('/Parametri/TimeStamp/CodLogin') as ValoreParametro(CodUtenteRilevazione))	
	SET @sCodUtenteRilevazione=ISNULL(@sCodUtenteRilevazione,'')
	
	    SET @xTimeStamp=(SELECT TOP 1 ValoreParametro.TS.query('.')
					  FROM @xParametri.nodes('/Parametri/TimeStamp') as ValoreParametro(TS))
	
	
		SET @xSchedaMovimento=(SELECT TOP 1 ValoreParametro.TS.query('.')
					  FROM @xParametri.nodes('/Parametri/SchedaMovimento') as ValoreParametro(TS))					
							
					
	SET @uGUID=NEWID()

					SET @xTimeStamp.modify('delete (/TimeStamp/IDEntita)[1]') 			
	SET @xTimeStamp.modify('insert <IDEntita>{sql:variable("@uGUID")}</IDEntita> into (/TimeStamp)[1]')

		SET @xTimeStamp.modify('delete (/TimeStamp/CodEntita)[1]') 
	SET @xTimeStamp.modify('insert <CodEntita>PRT</CodEntita> into (/TimeStamp)[1]')
	
		SET @xTimeStampBase=@xTimeStamp


	SET @xTimeStamp=CONVERT(XML,
						'<Parametri>' + CONVERT(varchar(max),@xTimeStamp) +
						'</Parametri>')
													
	BEGIN TRANSACTION
				
		INSERT INTO T_MovPrescrizioni(
					   ID					  
					  ,IDEpisodio
					  ,IDTrasferimento
					  ,DataEvento
					  ,DataEventoUTC
					  ,CodViaSomministrazione
					  ,CodTipoPrescrizione
					  ,CodStatoPrescrizione
					  ,CodStatoContinuazione
					  ,CodUtenteRilevazione
					  ,CodUtenteUltimaModifica
					  ,DataUltimaModifica
					  ,DataUltimaModificaUTC
					  ,IDString
				  )
		VALUES
				( 
					   @uGUID															   ,@uIDEpisodio 													   ,@uIDTrasferimento 											  ,ISNULL(@dDataEvento,Getdate())									  ,ISNULL(@dDataEventoUTC,GetUTCdate())								  ,@sCodViaSomministrazione											  ,@sCodTipoPrescrizione											  ,@sCodStatoPrescrizione											  ,@sCodStatoContinuazione											  ,@sCodUtenteRilevazione 											  ,NULL 															  ,NULL 															  ,NULL 															  ,CONVERT(VARCHAR(50),@uGUID)									)
				

	IF @@ERROR=0 
		BEGIN
						EXEC MSP_InsMovTimeStamp @xTimeStamp		
		END	
	IF @@ERROR = 0
		BEGIN
			COMMIT TRANSACTION
						
												
			SET @xLogDopo=Convert(XML,'<DataSet></DataSet>')
			SET @xLogPrima=''
			
			SET @xTemp=
				(SELECT * FROM 
					(SELECT * FROM T_MovPrescrizioni
					 WHERE ID=@uGUID
					) AS [Table]
				FOR XML AUTO, ELEMENTS)

			SET @xLogDopo.modify('insert sql:variable("@xTemp") as first into (/DataSet)[1]')
	
						
			SET @xParLog=CONVERT(XML,'<Parametri><LogDopo/></Parametri>')			
			SET @xParLog.modify('insert sql:variable("@xLogDopo") as first into (/Parametri/LogDopo)[1]')
			SET @xParLog.modify('insert sql:variable("@xTimeStampBase") as last into (/Parametri)[1]')
			
						SET @xParLog.modify('insert sql:variable("@xSchedaMovimento") as last into (/Parametri/LogDopo/DataSet)[1]')
					
						EXEC MSP_InsMovLog @xParLog
			
			SELECT @uGUID AS ID
		END	
	ELSE
		BEGIN
			ROLLBACK TRANSACTION	
			SELECT NULL AS ID
		END	 
	RETURN 0
END