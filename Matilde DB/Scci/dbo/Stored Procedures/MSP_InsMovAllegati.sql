CREATE  PROCEDURE [dbo].[MSP_InsMovAllegati](@xParametri XML)
AS
BEGIN
		
	
											
		DECLARE @uID AS UNIQUEIDENTIFIER
	DECLARE @uIDPaziente AS UNIQUEIDENTIFIER	
	DECLARE @uIDEpisodio AS UNIQUEIDENTIFIER	
	DECLARE @uIDTrasferimento AS UNIQUEIDENTIFIER	
	DECLARE @dDataEvento AS DATETIME	
	DECLARE @dDataEventoUTC AS DATETIME
	DECLARE @sNumeroDocumento AS VARCHAR(50)
	DECLARE @sCodTipoAllegato AS VARCHAR(20)
	DECLARE @sCodStatoAllegato AS VARCHAR(20)
	DECLARE @sCodFormatoAllegato AS VARCHAR(20)

	DECLARE @sCodUA AS VARCHAR(20)
	DECLARE @sCodEntita AS VARCHAR(20)
	DECLARE @uIDFolder AS UNIQUEIDENTIFIER	

	DECLARE @binDocumento VARBINARY(MAX)	
	DECLARE @txtDocumento VARCHAR(MAX)	
	DECLARE @binNomeFile VARBINARY(MAX)		
	DECLARE @sEstensione AS VARCHAR(10)	
	DECLARE @binTestoRTF VARBINARY(MAX)	
	DECLARE @binNotaRTF VARBINARY(MAX)	
	DECLARE @binTestoTXT VARBINARY(MAX)
	DECLARE @binNotaTXT VARBINARY(MAX)
	
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

		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDPaziente.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/ID') as ValoreParametro(IDPaziente))
	IF 	ISNULL(@sGUID,'') <> '' 
		SET @uID=CONVERT(UNIQUEIDENTIFIER,	@sGUID)	
	ELSE
		SET @uID=NULL

		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDPaziente.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDPaziente') as ValoreParametro(IDPaziente))
	IF 	ISNULL(@sGUID,'') <> '' 
			SET @uIDPaziente=CONVERT(UNIQUEIDENTIFIER,	@sGUID)		
	
	
		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDEpisodio.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDEpisodio') as ValoreParametro(IDEpisodio))
	IF 	ISNULL(@sGUID,'') <> '' 
			SET @uIDEpisodio=CONVERT(UNIQUEIDENTIFIER,	@sGUID)		
		
		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDTrasferimento.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDTrasferimento') as ValoreParametro(IDTrasferimento))
	IF 	ISNULL(@sGUID,'') <> '' 
			SET @uIDTrasferimento=CONVERT(UNIQUEIDENTIFIER,	@sGUID)	

		SET @sNumeroDocumento=(SELECT TOP 1 ValoreParametro.NumeroDocumento.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/NumeroDocumento') as ValoreParametro(NumeroDocumento))	
	SET @sNumeroDocumento=ISNULL(@sNumeroDocumento,'')
				
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
		
		SET @binTestoRTF=(SELECT TOP 1 ValoreParametro.TestoRTF.value('.','VARBINARY(MAX)')
					  FROM @xParametri.nodes('/Parametri/TestoRTF') as ValoreParametro(TestoRTF))	
		
		SET @binNotaRTF=(SELECT TOP 1 ValoreParametro.NotaRTF.value('.','VARBINARY(MAX)')
					  FROM @xParametri.nodes('/Parametri/NotaRTF') as ValoreParametro(NotaRTF))	

		SET @binTestoTXT=(SELECT TOP 1 ValoreParametro.TestoTXT.value('.','VARBINARY(MAX)')
					  FROM @xParametri.nodes('/Parametri/TestoTXT') as ValoreParametro(TestoTXT))	

		SET @binNotaTXT=(SELECT TOP 1 ValoreParametro.NotaTXT.value('.','VARBINARY(MAX)')
					  FROM @xParametri.nodes('/Parametri/NotaTXT') as ValoreParametro(NotaTXT))	

		SET  @txtDocumento =(SELECT TOP 1 ValoreParametro.Documento.value('.','VARCHAR(MAX)')
					  FROM @xParametri.nodes('/Parametri/Documento') as ValoreParametro(Documento)) 	
	
	SET @binDocumento=CAST(N'' AS xml).value('xs:base64Binary(sql:variable("@txtDocumento"))', 'varbinary(max)')
	
		SET @binNomeFile=(SELECT TOP 1 ValoreParametro.NomeFile.value('.','VARBINARY(MAX)')
					  FROM @xParametri.nodes('/Parametri/NomeFile') as ValoreParametro(NomeFile))	
					  
					  
		SET @sEstensione=(SELECT TOP 1 ValoreParametro.Estensione.value('.','VARCHAR(10)')
					  FROM @xParametri.nodes('/Parametri/Estensione') as ValoreParametro(Estensione))	
	SET @sEstensione=ISNULL(@sEstensione,'')
					  
		SET @sCodTipoAllegato=(SELECT TOP 1 ValoreParametro.CodTipoAllegato.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodTipoAllegato') as ValoreParametro(CodTipoAllegato))	
	SET @sCodTipoAllegato=ISNULL(@sCodTipoAllegato,'')
	
		SET @sCodStatoAllegato=(SELECT TOP 1 ValoreParametro.CodStatoAllegato.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodStatoAllegato') as ValoreParametro(CodStatoAllegato))	
	SET @sCodStatoAllegato=ISNULL(@sCodStatoAllegato,'')
	IF @sCodStatoAllegato='' SET @sCodStatoAllegato='IC'			
		SET @sCodFormatoAllegato=(SELECT TOP 1 ValoreParametro.CodFormatoAllegato.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodFormatoAllegato') as ValoreParametro(CodFormatoAllegato))	
	SET @sCodFormatoAllegato=ISNULL(@sCodFormatoAllegato,'')	
	
		SET @sCodUA=(SELECT TOP 1 ValoreParametro.CodUA.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodUA') as ValoreParametro(CodUA))	
	SET @sCodUA=ISNULL(@sCodUA,'')	

		SET @sCodEntita=(SELECT TOP 1 ValoreParametro.CodEntita.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodEntita') as ValoreParametro(CodEntita))	
	SET @sCodEntita=ISNULL(@sCodEntita,'')	

	
		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDFolder.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDFolder') as ValoreParametro(IDFolder))
	IF 	ISNULL(@sGUID,'') <> '' AND ISNULL(@sGUID,'') <> 'Root'
			SET @uIDFolder=CONVERT(UNIQUEIDENTIFIER,	@sGUID)	

					
		SET @sCodUtenteRilevazione=(SELECT TOP 1 ValoreParametro.CodUtenteRilevazione.value('.','VARCHAR(100)')
					  FROM @xParametri.nodes('/Parametri/TimeStamp/CodLogin') as ValoreParametro(CodUtenteRilevazione))	
	SET @sCodUtenteRilevazione=ISNULL(@sCodUtenteRilevazione,'')
	
	    SET @xTimeStamp=(SELECT TOP 1 ValoreParametro.TS.query('.')
					  FROM @xParametri.nodes('/Parametri/TimeStamp') as ValoreParametro(TS))
	
				
		IF @uID IS NULL	
		SET @uGUID=NEWID()
	ELSE
		SET @uGUID = @uID

					SET @xTimeStamp.modify('delete (/TimeStamp/IDEntita)[1]') 
			
	SET @xTimeStamp.modify('insert <IDEntita>{sql:variable("@uGUID")}</IDEntita> into (/TimeStamp)[1]')

		SET @xTimeStampBase=@xTimeStamp


	SET @xTimeStamp=CONVERT(XML,
						'<Parametri>' + CONVERT(varchar(max),@xTimeStamp) +
						'</Parametri>')
													
	BEGIN TRANSACTION
				INSERT INTO T_MovAllegati(
					   ID	
					  ,IDPaziente 				  
					  ,IDEpisodio
					  ,IDTrasferimento
					  ,NumeroDocumento
					  ,DataEvento
					  ,DataEventoUTC
					  ,TestoRTF
					  ,NotaRTF
					  ,TestoTXT
					  ,NotaTXT
					  ,CodFormatoAllegato
					  ,CodTipoAllegato
					  ,CodStatoAllegato			
					  ,Documento		
					  ,NomeFile
					  ,Estensione
					  ,CodUtenteRilevazione
					  ,CodUtenteUltimaModifica
					  ,DataRilevazione
					  ,DataRilevazioneUTC
					  ,DataUltimaModifica
					  ,DataUltimaModificaUTC
					  ,CodUA
					  ,CodEntita
					  ,IDFolder					  
				  )
		VALUES
				( 
					   @uGUID															   ,@uIDPaziente 													   ,@uIDEpisodio 													   ,@uIDTrasferimento 												   ,@sNumeroDocumento												  ,ISNULL(@dDataEvento,Getdate())									  ,ISNULL(@dDataEventoUTC,GetUTCdate())								  ,CONVERT(VARCHAR(MAX),@binTestoRTF)								  ,CONVERT(VARCHAR(MAX),@binNotaRTF)								  , dbo.MF_PulisciTXT(
						CONVERT(VARCHAR(MAX),@binTestoTXT)						
						)																  , dbo.MF_PulisciTXT(
						CONVERT(VARCHAR(MAX),@binNotaTXT)						
						)																  ,@sCodFormatoAllegato												  ,@sCodTipoAllegato												  ,@sCodStatoAllegato												  ,@binDocumento													  ,CONVERT(VARCHAR(MAX),@binNomeFile)
					  ,@sEstensione														  ,@sCodUtenteRilevazione 											  ,NULL 															  ,Getdate()														  ,GetUTCdate()														  ,NULL 															  ,NULL 															  ,@sCodUA															  ,@sCodEntita														  ,@uIDFolder														  					  		  						
				)
				
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
					(SELECT ID
					  ,IDNum
					  ,IDPaziente
					  ,IDEpisodio
					  ,IDTrasferimento
					  ,NumeroDocumento
					  ,DataEvento
					  ,DataEventoUTC
					  ,TestoRTF
					  ,NotaRTF
					  ,TestoTXT
					  ,NotaTXT
					  ,CodFormatoAllegato
					  ,CodTipoAllegato
					  ,CodStatoAllegato
					 					  ,NomeFile
					  ,Estensione					 
					  ,CodUtenteRilevazione
					  ,CodUtenteUltimaModifica
					  ,DataUltimaModifica
					  ,DataUltimaModificaUTC 					  
					  ,CodUA
					  ,CodEntita
					  ,IDFolder	
					 FROM T_MovAllegati
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