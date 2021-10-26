CREATE PROCEDURE [dbo].[MSP_InsMovTaskInfermieristici](@xParametri XML)
AS
BEGIN
		
	
							
												
								
	
	DECLARE @uIDEpisodio AS UNIQUEIDENTIFIER	
	DECLARE @uIDTrasferimento AS UNIQUEIDENTIFIER	
		
	DECLARE @dDataEvento AS DATETIME	
	DECLARE @dDataEventoUTC AS DATETIME
	
	DECLARE @sCodSistema AS VARCHAR(20)
	DECLARE @sIDSistema AS VARCHAR(50)
	DECLARE @sIDGruppo AS VARCHAR(50)
	DECLARE @sIDTaskIniziale AS VARCHAR(50)
	DECLARE @sCodTipoTaskInfermieristico AS VARCHAR(20)
	DECLARE @sCodStatoTaskInfermieristico AS VARCHAR(20)
	DECLARE @sCodTipoRegistrazione AS VARCHAR(20)
	DECLARE @sCodProtocollo AS VARCHAR(20)
	DECLARE @sCodProtocolloTempo AS VARCHAR(20)
	
	DECLARE @dDataProgrammata AS DATETIME	
	DECLARE @dDataProgrammataUTC AS DATETIME
	DECLARE @dDataErogazione AS DATETIME
	DECLARE @dDataErogazioneUTC AS DATETIME
	DECLARE @binNote VARBINARY(MAX)
	DECLARE @binDescrizioneFUT VARBINARY(MAX)
	DECLARE @sCodUtenteRilevazione AS VARCHAR(100)		
	DECLARE @binSottoclasse AS VARBINARY(MAX)
	DECLARE @binPosologiaEffettiva VARBINARY(MAX)
	DECLARE @binAlert VARBINARY(MAX)
	DECLARE @binBarcode VARBINARY(MAX)
	
		DECLARE @sCodUtenteUltimaModifica AS VARCHAR(100)	
	DECLARE @dDataUltimaModifica AS DATETIME	
	DECLARE @dDataUltimaModificaUTC AS DATETIME
	
			
		DECLARE @sGUID AS VARCHAR(50)
	DECLARE @uGUID AS UNIQUEIDENTIFIER
	DECLARE @sDataTmp AS VARCHAR(20)	
	
	    DECLARE @xTimeStampBase AS XML	
	DECLARE @xTimeStamp AS XML	
	
		DECLARE @xSchedaMovimento AS XML
		
		DECLARE @xLogPrima AS XML
	DECLARE @xLogDopo AS XML
	DECLARE @xTemp AS XML
	DECLARE @xParLog AS XML
		
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
		
		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDEpisodio.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDEpisodio') as ValoreParametro(IDEpisodio))
	IF 	ISNULL(@sGUID,'') <> '' 
			SET @uIDEpisodio=CONVERT(UNIQUEIDENTIFIER,	@sGUID)		

		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDTrasferimento.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDTrasferimento') as ValoreParametro(IDTrasferimento))
	IF 	ISNULL(@sGUID,'') <> '' 
			SET @uIDTrasferimento=CONVERT(UNIQUEIDENTIFIER,	@sGUID)		

		SET @sCodSistema=(SELECT TOP 1 ValoreParametro.CodSistema.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodSistema') as ValoreParametro(CodSistema))	
	SET @sCodSistema=ISNULL(@sCodSistema,'')
	
		SET @sIDSistema=(SELECT TOP 1 ValoreParametro.IDSistema.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDSistema') as ValoreParametro(IDSistema))	
	SET @sIDSistema=ISNULL(@sIDSistema,'')
	
		SET @sIDGruppo=(SELECT TOP 1 ValoreParametro.IDGruppo.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDGruppo') as ValoreParametro(IDGruppo))	
	SET @sIDGruppo=ISNULL(@sIDGruppo,'')
	
		SET  @sIDTaskIniziale=(SELECT TOP 1 ValoreParametro.IDTaskIniziale.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDTaskIniziale') as ValoreParametro(IDTaskIniziale))	

		SET @sCodTipoTaskInfermieristico=(SELECT TOP 1 ValoreParametro.CodTipoTaskInfermieristico.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodTipoTaskInfermieristico') as ValoreParametro(CodTipoTaskInfermieristico))	
	SET @sCodTipoTaskInfermieristico=ISNULL(@sCodTipoTaskInfermieristico,'')
	
		SET @sCodStatoTaskInfermieristico=(SELECT TOP 1 ValoreParametro.CodStatoTaskInfermieristico.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodStatoTaskInfermieristico') as ValoreParametro(CodStatoTaskInfermieristico))	
	SET @sCodStatoTaskInfermieristico=ISNULL(@sCodStatoTaskInfermieristico,'')
	IF @sCodStatoTaskInfermieristico='' SET @sCodStatoTaskInfermieristico='PR'	
		SET @sCodTipoRegistrazione=(SELECT TOP 1 ValoreParametro.CodTipoRegistrazione.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodTipoRegistrazione') as ValoreParametro(CodTipoRegistrazione))	
	SET @sCodTipoRegistrazione=ISNULL(@sCodTipoRegistrazione,'')
	
		SET @sCodProtocollo=(SELECT TOP 1 ValoreParametro.CodProtocollo.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodProtocollo') as ValoreParametro(CodProtocollo))	
		
	
		SET @sCodProtocolloTempo=(SELECT TOP 1 ValoreParametro.CodProtocolloTempo.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodProtocolloTempo') as ValoreParametro(CodProtocolloTempo))	
			
		SET @binSottoclasse=(SELECT TOP 1 ValoreParametro.Sottoclasse.value('.','VARBINARY(MAX)')
					  FROM @xParametri.nodes('/Parametri/Sottoclasse') as ValoreParametro(Sottoclasse))						  
		SET @binNote=(SELECT TOP 1 ValoreParametro.NoteRTF.value('.','VARBINARY(MAX)')
					  FROM @xParametri.nodes('/Parametri/Note') as ValoreParametro(NoteRTF))						  
	
		SET @binDescrizioneFUT=(SELECT TOP 1 ValoreParametro.DescrizioneFUT.value('.','VARBINARY(MAX)')
					  FROM @xParametri.nodes('/Parametri/DescrizioneFUT') as ValoreParametro(DescrizioneFUT))	
				  					  
		SET @sDataTmp=(SELECT TOP 1 ValoreParametro.DataProgrammata.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/DataProgrammata') as ValoreParametro(DataProgrammata))					  
	SET @sDataTmp=ISNULL(@sDataTmp,'')	

	IF @sDataTmp<> '' 
		BEGIN			
						SET @sDataTmp= SUBSTRING(@sDataTmp,4,2) + '-' 
						+ LEFT(@sDataTmp,2) + '-' +
						+ SUBSTRING(@sDataTmp,7,4) +
						' ' + RIGHT(@sDataTmp,5)
			IF ISDATE(@sDataTmp)=1
				SET	@dDataProgrammata=CONVERT(DATETIME,@sDataTmp,120)						
			ELSE
				SET	@dDataProgrammata =NULL			
		END
	
		SET @sDataTmp=(SELECT TOP 1 ValoreParametro.DataProgrammataUTC.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/DataProgrammataUTC') as ValoreParametro(DataProgrammataUTC))					  
	SET @sDataTmp=ISNULL(@sDataTmp,'')	

	IF @sDataTmp<> '' 
		BEGIN			
						SET @sDataTmp= SUBSTRING(@sDataTmp,4,2) + '-' 
						+ LEFT(@sDataTmp,2) + '-' +
						+ SUBSTRING(@sDataTmp,7,4) +
						' ' + RIGHT(@sDataTmp,5)
			IF ISDATE(@sDataTmp)=1
				SET	@dDataProgrammataUTC=CONVERT(DATETIME,@sDataTmp,120)						
			ELSE
				SET	@dDataProgrammataUTC =NULL			
		END
	
	
		SET @sDataTmp=(SELECT TOP 1 ValoreParametro.DataErogazione.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/DataErogazione') as ValoreParametro(DataErogazione))					  
	SET @sDataTmp=ISNULL(@sDataTmp,'')	

	IF @sDataTmp<> '' 
		BEGIN			
						SET @sDataTmp= SUBSTRING(@sDataTmp,4,2) + '-' 
						+ LEFT(@sDataTmp,2) + '-' +
						+ SUBSTRING(@sDataTmp,7,4) +
						' ' + RIGHT(@sDataTmp,5)
			IF ISDATE(@sDataTmp)=1
				SET	@dDataErogazione=CONVERT(DATETIME,@sDataTmp,120)						
			ELSE
				SET	@dDataErogazione =NULL			
		END
		
		SET @sDataTmp=(SELECT TOP 1 ValoreParametro.DataErogazioneUTC.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/DataErogazioneUTC') as ValoreParametro(DataErogazioneUTC))					  
	SET @sDataTmp=ISNULL(@sDataTmp,'')	

	IF @sDataTmp<> '' 
		BEGIN			
						SET @sDataTmp= SUBSTRING(@sDataTmp,4,2) + '-' 
						+ LEFT(@sDataTmp,2) + '-' +
						+ SUBSTRING(@sDataTmp,7,4) +
						' ' + RIGHT(@sDataTmp,5)
			IF ISDATE(@sDataTmp)=1
				SET	@dDataErogazioneUTC=CONVERT(DATETIME,@sDataTmp,120)						
			ELSE
				SET	@dDataErogazioneUTC =NULL			
		END
			
	
			SET @sCodUtenteRilevazione=(SELECT TOP 1 ValoreParametro.CodUtenteRilevazione.value('.','VARCHAR(100)')
					  FROM @xParametri.nodes('/Parametri/CodUtenteRilevazione') as ValoreParametro(CodUtenteRilevazione))	
	SET @sCodUtenteRilevazione=ISNULL(@sCodUtenteRilevazione,'')	

	IF @sCodUtenteRilevazione='' 
	BEGIN
				SET @sCodUtenteRilevazione=(SELECT TOP 1 ValoreParametro.CodUtenteRilevazione.value('.','VARCHAR(100)')
					  FROM @xParametri.nodes('/Parametri/TimeStamp/CodLogin') as ValoreParametro(CodUtenteRilevazione))	
		SET @sCodUtenteRilevazione=ISNULL(@sCodUtenteRilevazione,'')						
	END				

		SET @binPosologiaEffettiva=(SELECT TOP 1 ValoreParametro.PosologiaEffettiva.value('.','VARBINARY(MAX)')
					  FROM @xParametri.nodes('/Parametri/PosologiaEffettiva') as ValoreParametro(PosologiaEffettiva))
	
		SET @binAlert=(SELECT TOP 1 ValoreParametro.Alert.value('.','VARBINARY(MAX)')
					  FROM @xParametri.nodes('/Parametri/Alert') as ValoreParametro(Alert))
	
		SET @binBarcode=(SELECT TOP 1 ValoreParametro.Barcode.value('.','VARBINARY(MAX)')
					  FROM @xParametri.nodes('/Parametri/Barcode') as ValoreParametro(Barcode))
	
	    SET @xTimeStamp=(SELECT TOP 1 ValoreParametro.TS.query('.')
					  FROM @xParametri.nodes('/Parametri/TimeStamp') as ValoreParametro(TS))
	
	
		SET @xSchedaMovimento=(SELECT TOP 1 ValoreParametro.TS.query('.')
					  FROM @xParametri.nodes('/Parametri/SchedaMovimento') as ValoreParametro(TS))			
	
			IF ISNULL(@sCodStatoTaskInfermieristico,'') IN ('TR','AN','ER' ) AND @dDataErogazione IS NULL
	BEGIN
		SET @dDataErogazione=GETDATE()
		SET @dDataErogazioneUTC=GETDATE()
	END
	

	SET @dDataUltimaModifica=NULL	
	SET @dDataUltimaModificaUTC=NULL
	SET @sCodUtenteUltimaModifica=NULL
	
			IF ISNULL(@sCodStatoTaskInfermieristico,'') IN ('TR') 
	BEGIN
				SET @dDataUltimaModifica=GETDATE()
		SET @dDataUltimaModificaUTC=GETDATE()
				
				SET @sCodUtenteUltimaModifica=(SELECT TOP 1 ValoreParametro.CodUtenteUltimaModifica.value('.','VARCHAR(100)')
					  FROM @xParametri.nodes('/Parametri/TimeStamp/CodLogin') as ValoreParametro(CodUtenteUltimaModifica))	
		SET @sCodUtenteUltimaModifica=ISNULL(@sCodUtenteUltimaModifica,'')
		
	END
		
	
					
	SET @uGUID=NEWID()

				
		SET @xTimeStamp.modify('delete (/TimeStamp/IDEntita)[1]') 			
	SET @xTimeStamp.modify('insert <IDEntita>{sql:variable("@uGUID")}</IDEntita> into (/TimeStamp)[1]')		

		SET @xTimeStampBase=@xTimeStamp
	
	SET @xTimeStamp=CONVERT(XML,
						'<Parametri>' + CONVERT(varchar(max),@xTimeStamp) +
						'</Parametri>')
													
	BEGIN TRANSACTION
				INSERT INTO T_MovTaskInfermieristici(
					ID				
				   ,IDEpisodio
				  ,IDTrasferimento
				  ,DataEvento
				  ,DataEventoUTC
				  ,CodSistema
				  ,IDSistema
				  ,IDGruppo
				  ,IDTaskIniziale
				  ,CodTipoTaskInfermieristico
				  ,CodStatoTaskInfermieristico
				  ,CodTipoRegistrazione
				  ,CodProtocollo
				  ,CodProtocolloTempo
				  ,Sottoclasse
				  ,DataProgrammata
				  ,DataProgrammataUTC				 
				  ,DataErogazione
				  ,DataErogazioneUTC
				  ,Note
				  ,DescrizioneFUT
				  ,CodUtenteRilevazione
				  ,CodUtenteUltimaModifica
				  ,DataUltimaModifica
				  ,DataUltimaModificaUTC
				  ,PosologiaEffettiva
				  ,Alert
				  ,Barcode
				  )
		VALUES
		
				(
					   @uGUID														  ,@uIDEpisodio 												  ,@uIDTrasferimento 											  ,ISNULL(@dDataEvento,Getdate())								  ,ISNULL(@dDataEventoUTC,GetUTCdate())							  ,@sCodSistema 												  ,@sIDSistema 													  ,@sIDGruppo 													  ,CASE WHEN ISNULL(@sIDTaskIniziale, '') = '' THEN NULL ELSE @sIDTaskIniziale END						  ,@sCodTipoTaskInfermieristico 								  ,@sCodStatoTaskInfermieristico 								  ,@sCodTipoRegistrazione 										  ,@sCodProtocollo												  ,@sCodProtocolloTempo											  ,CONVERT(VARCHAR(MAX),@binSottoClasse)						  ,@dDataProgrammata											  ,@dDataProgrammataUTC											  ,@dDataErogazione												  ,@dDataErogazioneUTC											  ,CONVERT(VARCHAR(MAX),@binNote)								  ,CONVERT(VARCHAR(MAX),@binDescrizioneFUT)						  ,@sCodUtenteRilevazione										  ,@sCodUtenteUltimaModifica									  ,@dDataUltimaModifica											  ,@dDataUltimaModificaUTC										  ,CONVERT(VARCHAR(MAX),@binPosologiaEffettiva)							  ,CONVERT(VARCHAR(MAX),@binAlert)								  ,CONVERT(VARCHAR(MAX),@binBarcode)						)
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
					(SELECT*
					FROM T_MovTaskInfermieristici
					 WHERE ID=@uGUID
					) AS [Table]
				FOR XML AUTO, ELEMENTS)

			SET @xLogDopo.modify('insert sql:variable("@xTemp") as first into (/DataSet)[1]')
	
						
			SET @xParLog=CONVERT(XML,'<Parametri><LogDopo/></Parametri>')			
			SET @xParLog.modify('insert sql:variable("@xLogDopo") as first into (/Parametri/LogDopo)[1]')
			SET @xParLog.modify('insert sql:variable("@xTimeStampBase") as last into (/Parametri)[1]')
			
						SET @xParLog.modify('insert sql:variable("@xSchedaMovimento") as last into (/Parametri/LogDopo/DataSet)[1]')
			
						IF ISNULL(@sCodStatoTaskInfermieristico,'')='TR' 
				BEGIN
					SET @xParLog.modify('delete (Parametri/TimeStamp/CodAzione)[1]') 			
					SET @xParLog.modify('insert <CodAzione>TRA</CodAzione> into (Parametri/TimeStamp)[1]')
			END
		
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