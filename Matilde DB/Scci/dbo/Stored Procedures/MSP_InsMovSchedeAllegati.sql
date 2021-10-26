CREATE  PROCEDURE [dbo].[MSP_InsMovSchedeAllegati](@xParametri XML)
AS
BEGIN
		
	
											
		DECLARE @uIDScheda AS UNIQUEIDENTIFIER	
	DECLARE @dDataEvento AS DATETIME	
	DECLARE @dDataEventoUTC AS DATETIME
	DECLARE @sCodCampo AS VARCHAR(255)
	DECLARE @sCodSezione AS VARCHAR(255)
	DECLARE @nSequenza AS INTEGER
	DECLARE @sIDGruppo AS VARCHAR(50)
	DECLARE @sDescrizioneAllegato AS VARCHAR(255)
	DECLARE @sDescrizioneCampo AS VARCHAR(255)
	DECLARE @sCodTipoAllegatoScheda AS VARCHAR(20)
	DECLARE @sCodStatoAllegatoScheda AS VARCHAR(20)
	DECLARE @sCodFormatoAllegato AS VARCHAR(20)	
	DECLARE @binAnteprima VARBINARY(MAX)	
	DECLARE @txtAnteprima VARCHAR(MAX)		
	DECLARE @binDocumento VARBINARY(MAX)	
	DECLARE @txtDocumento VARCHAR(MAX)	
	DECLARE @binNomeFile VARBINARY(MAX)		
	DECLARE @sEstensione AS VARCHAR(10)	
	
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
		
		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDScheda.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDScheda') as ValoreParametro(IDScheda))
	IF 	ISNULL(@sGUID,'') <> '' 
			SET @uIDScheda=CONVERT(UNIQUEIDENTIFIER,	@sGUID)		
		
		SET @sCodCampo=(SELECT TOP 1 ValoreParametro.CodCampo.value('.','VARCHAR(255)')
					  FROM @xParametri.nodes('/Parametri/CodCampo') as ValoreParametro(CodCampo))								
		SET @sCodSezione=(SELECT TOP 1 ValoreParametro.CodSezione.value('.','VARCHAR(255)')
					  FROM @xParametri.nodes('/Parametri/CodSezione') as ValoreParametro(CodSezione))	
	
		SET @nSequenza=(SELECT TOP 1 ValoreParametro.Sequenza.value('.','INTEGER')
					  FROM @xParametri.nodes('/Parametri/Sequenza') as ValoreParametro(Sequenza))					  
		SET @sIDGruppo=(SELECT TOP 1 ValoreParametro.IDGruppo.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDGruppo') as ValoreParametro(IDGruppo))	
					  
		SET  @txtAnteprima =(SELECT TOP 1 ValoreParametro.Anteprima.value('.','VARCHAR(MAX)')
					  FROM @xParametri.nodes('/Parametri/Anteprima') as ValoreParametro(Anteprima)) 	
	
	SET @binAnteprima=CAST(N'' AS xml).value('xs:base64Binary(sql:variable("@txtAnteprima"))', 'varbinary(max)')
	
		SET  @txtDocumento =(SELECT TOP 1 ValoreParametro.Documento.value('.','VARCHAR(MAX)')
					  FROM @xParametri.nodes('/Parametri/Documento') as ValoreParametro(Documento)) 	
	
	SET @binDocumento=CAST(N'' AS xml).value('xs:base64Binary(sql:variable("@txtDocumento"))', 'varbinary(max)')
	
		SET @binNomeFile=(SELECT TOP 1 ValoreParametro.NomeFile.value('.','VARBINARY(MAX)')
					  FROM @xParametri.nodes('/Parametri/NomeFile') as ValoreParametro(NomeFile))	
					  
					  
		SET @sEstensione=(SELECT TOP 1 ValoreParametro.Estensione.value('.','VARCHAR(10)')
					  FROM @xParametri.nodes('/Parametri/Estensione') as ValoreParametro(Estensione))	
	SET @sEstensione=ISNULL(@sEstensione,'')

		SET @sDescrizioneAllegato=(SELECT TOP 1 ValoreParametro.DescrizioneAllegato.value('.','VARCHAR(255)')
					  FROM @xParametri.nodes('/Parametri/DescrizioneAllegato') as ValoreParametro(DescrizioneAllegato))		
	
		SET @sDescrizioneCampo=(SELECT TOP 1 ValoreParametro.DescrizioneCampo.value('.','VARCHAR(255)')
					  FROM @xParametri.nodes('/Parametri/DescrizioneCampo') as ValoreParametro(DescrizioneCampo))	
					  	
		SET @sCodTipoAllegatoScheda=(SELECT TOP 1 ValoreParametro.CodTipoAllegatoScheda.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodTipoAllegatoScheda') as ValoreParametro(CodTipoAllegatoScheda))		
	
		SET @sCodStatoAllegatoScheda=(SELECT TOP 1 ValoreParametro.CodStatoAllegatoScheda.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodStatoAllegatoScheda') as ValoreParametro(CodStatoAllegatoScheda))	
	SET @sCodStatoAllegatoScheda=ISNULL(@sCodStatoAllegatoScheda,'')
	IF @sCodStatoAllegatoScheda='' SET @sCodStatoAllegatoScheda='IC'								  						
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
	
					
					
	SET @uGUID=NEWID()

					SET @xTimeStamp.modify('delete (/TimeStamp/IDEntita)[1]') 
			
	SET @xTimeStamp.modify('insert <IDEntita>{sql:variable("@uGUID")}</IDEntita> into (/TimeStamp)[1]')

		SET @xTimeStampBase=@xTimeStamp


	SET @xTimeStamp=CONVERT(XML,
						'<Parametri>' + CONVERT(varchar(max),@xTimeStamp) +
						'</Parametri>')
													
	BEGIN TRANSACTION
				INSERT INTO T_MovSchedeAllegati(	
					   ID
					  ,IDScheda
					  ,CodCampo
					  ,IDGruppo
					  ,Anteprima
					  ,Documento
					  ,NomeFile
					  ,Estensione
					  ,DescrizioneAllegato
					  ,DescrizioneCampo
					  ,CodTipoAllegatoScheda
					  ,CodStatoAllegatoScheda
					  ,DataEvento
					  ,DataEventoUTC
					  ,CodUtenteRilevazione
					  ,CodUtenteUltimaModifica
					  ,DataRilevazione
					  ,DataRilevazioneUTC
					  ,DataUltimaModifica
					  ,DataUltimaModificaUTC
					  ,CodSezione
					  ,Sequenza				  					  				  
				  )
		VALUES
				( 
					   @uGUID															   ,@uIDScheda														   ,@sCodCampo														   ,@sIDGruppo														   ,@binAnteprima													   ,@binDocumento													  ,CONVERT(VARCHAR(MAX),@binNomeFile)								  ,@sEstensione														  ,@sDescrizioneAllegato											  ,@sDescrizioneCampo												  ,@sCodTipoAllegatoScheda											  ,@sCodStatoAllegatoScheda											 
					  ,ISNULL(@dDataEvento,Getdate())									  ,ISNULL(@dDataEventoUTC,GetUTCdate())								  ,@sCodUtenteRilevazione 											  ,NULL 															  ,Getdate()														  ,GetUTCdate()														  ,NULL 															  ,NULL 															  ,@sCodSezione														  ,@nSequenza													)
				
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
					(SELECT  
					  ID
					  ,IDScheda
					  ,CodCampo
					  ,IDGruppo
					  					  					  ,NomeFile
					  ,Estensione
					  ,DescrizioneAllegato
					  ,DescrizioneCampo
					  ,CodTipoAllegatoScheda
					  ,CodStatoAllegatoScheda
					  ,DataEvento
					  ,DataEventoUTC
					  ,CodUtenteRilevazione
					  ,CodUtenteUltimaModifica
					  ,DataRilevazione
					  ,DataRilevazioneUTC
					  ,DataUltimaModifica
					  ,DataUltimaModificaUTC			
					 FROM T_MovSchedeAllegati
					 WHERE ID=@uGUID
					) AS [Table]
				FOR XML AUTO, ELEMENTS)

			SET @xLogDopo.modify('insert sql:variable("@xTemp") as first into (/DataSet)[1]')
	
						
			SET @xParLog=CONVERT(XML,'<Parametri><LogDopo/></Parametri>')			
			SET @xParLog.modify('insert sql:variable("@xLogDopo") as first into (/Parametri/LogDopo)[1]')
			SET @xParLog.modify('insert sql:variable("@xTimeStampBase") as last into (/Parametri)[1]')
			
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