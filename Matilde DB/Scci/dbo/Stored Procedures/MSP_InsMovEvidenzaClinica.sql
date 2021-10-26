CREATE PROCEDURE [dbo].[MSP_InsMovEvidenzaClinica](@xParametri XML)
AS
BEGIN
											
	
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
	DECLARE @sCodUtenteInserimento AS VARCHAR(100)	
				
		DECLARE @sGUID AS VARCHAR(50)
	DECLARE @uGUID AS UNIQUEIDENTIFIER
	DECLARE @sDataTmp AS VARCHAR(20)	
	
	    DECLARE @xTimeStampBase AS XML	
	DECLARE @xTimeStamp AS XML	
		
		DECLARE @xLogPrima AS XML
	DECLARE @xLogDopo AS XML
	DECLARE @xTemp AS XML
	DECLARE @xParLog AS XML

		SET @sCodUtenteInserimento=(SELECT TOP 1 ValoreParametro.CodUtenteInserimento.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/TimeStamp/CodLogin') as ValoreParametro(CodUtenteInserimento))	
	
				
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
				SET	@dDataEventoDWH	=CONVERT(DATETIME,@sDataTmp,120)						
			ELSE
				SET	@dDataEventoDWH	 =NULL			
		END
		
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
				SET	@dDataEventoDWHUTC =CONVERT(DATETIME,@sDataTmp,120)						
			ELSE
				SET	@dDataEventoDWHUTC  =NULL			
		END
			
		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDEpisodio.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDEpisodio') as ValoreParametro(IDEpisodio))
	IF 	ISNULL(@sGUID,'') <> '' 
			SET @uIDEpisodio=CONVERT(UNIQUEIDENTIFIER,	@sGUID)		

		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDTrasferimento.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDTrasferimento') as ValoreParametro(IDTrasferimento))
	IF 	ISNULL(@sGUID,'') <> '' 
			SET @uIDTrasferimento=CONVERT(UNIQUEIDENTIFIER,	@sGUID)		

		SET @sCodTipoEvidenzaClinica=(SELECT TOP 1 ValoreParametro.CodTipoEvidenzaClinica.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodTipoEvidenzaClinica') as ValoreParametro(CodTipoEvidenzaClinica))	
	SET @sCodTipoEvidenzaClinica=ISNULL(@sCodTipoEvidenzaClinica,'')
	
		SET @sCodStatoEvidenzaClinica=(SELECT TOP 1 ValoreParametro.CodStatoEvidenzaClinica.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodStatoEvidenzaClinica') as ValoreParametro(CodStatoEvidenzaClinica))	
	SET @sCodStatoEvidenzaClinica=ISNULL(@sCodStatoEvidenzaClinica,'')

		SET @sCodStatoEvidenzaClinicaVisione=(SELECT TOP 1 ValoreParametro.CodStatoEvidenzaClinicaVisione.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodStatoEvidenzaClinicaVisione') as ValoreParametro(CodStatoEvidenzaClinicaVisione))	
	SET @sCodStatoEvidenzaClinicaVisione=ISNULL(@sCodStatoEvidenzaClinicaVisione,'')
	IF @sCodStatoEvidenzaClinicaVisione='' SET @sCodStatoEvidenzaClinicaVisione='DV'		
		SET @sIDRefertoDWH=(SELECT TOP 1 ValoreParametro.IDRefertoDWH.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDRefertoDWH') as ValoreParametro(IDRefertoDWH))	
	SET @sIDRefertoDWH=ISNULL(@sIDRefertoDWH,'')

		SET @sNumeroRefertoDWH=(SELECT TOP 1 ValoreParametro.NumeroRefertoDWH.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/NumeroRefertoDWH') as ValoreParametro(NumeroRefertoDWH))	
	
		SET @txtAnteprima=(SELECT TOP 1 ValoreParametro.Anteprima.value('.','VARCHAR(MAX)')
					  FROM @xParametri.nodes('/Parametri/Anteprima') as ValoreParametro(Anteprima))	
					  
					
		SET @txtPDFDWH=(SELECT TOP 1 ValoreParametro.PDFDWH.value('.','VARCHAR(MAX)')
			  FROM @xParametri.nodes('/Parametri/PDFDWH') as ValoreParametro(PDFDWH))	

	SET @binPDFDWH=CAST(N'' AS xml).value('xs:base64Binary(sql:variable("@txtPDFDWH"))', 'varbinary(max)')
		
		SET @sCodUtenteVisione=(SELECT TOP 1 ValoreParametro.CodUtenteVisione.value('.','VARCHAR(100)')
					  FROM @xParametri.nodes('/Parametri/CodUtenteVisione') as ValoreParametro(CodUtenteVisione))	
	SET @sCodUtenteVisione=ISNULL(@sCodUtenteVisione,'')

	
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
				INSERT INTO T_MovEvidenzaClinica(
					 ID      
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
					,Anteprima
					,PDFDWH
					,CodUtenteInserimento
					,DataInserimento
					,DataInserimentoUTC
					,CodUtenteVisione
					,CodUtenteUltimaModifica
					,DataUltimaModifica
					,DataUltimaModificaUTC
					,DataVisione
					,DataVisioneUTC
				  )
		VALUES
				(
					  @uGUID													  ,@uIDEpisodio 											  ,@uIDTrasferimento 										  ,@sCodTipoEvidenzaClinica 								  ,@sCodStatoEvidenzaClinica 								  ,@sCodStatoEvidenzaClinicaVisione							  ,@sIDRefertoDWH 											  ,@sNumeroRefertoDWH										  ,ISNULL(@dDataEvento,Getdate())							  ,ISNULL(@dDataEventoUTC,GetUTCdate()) 					  ,@dDataEventoDWH											  ,@dDataEventoDWHUTC											  ,@txtAnteprima					 						  ,@binPDFDWH												  ,@sCodUtenteInserimento									  ,GetDATE()												  ,GETUTCDATE()												  ,@sCodUtenteVisione	 									  ,NULL			 											  ,NULL 													  ,NULL 													  ,Case																WHEN ISNULL(@sCodStatoEvidenzaClinicaVisione,'') ='VS' THEN GETDATE()
							ELSE NULL
					   END
					  ,Case																WHEN ISNULL(@sCodStatoEvidenzaClinicaVisione,'') ='VS' THEN GETUTCDATE()
							ELSE NULL
					   END
				)
	IF @@ERROR=0 AND @@ROWCOUNT>0
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