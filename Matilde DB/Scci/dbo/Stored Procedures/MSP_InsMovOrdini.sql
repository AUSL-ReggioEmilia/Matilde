
CREATE PROCEDURE [dbo].[MSP_InsMovOrdini](@xParametri XML)
AS
BEGIN
		
	
																			
		
	DECLARE @uIDPaziente AS UNIQUEIDENTIFIER	
	DECLARE @uIDEpisodio AS UNIQUEIDENTIFIER	
	DECLARE @uIDTrasferimento AS UNIQUEIDENTIFIER		
	DECLARE @sIDOrdineOE AS VARCHAR(50)	
	DECLARE @sNumeroOrdineOE AS VARCHAR(50)
	DECLARE @sXMLOE VARCHAR(MAX)
	DECLARE @xXMLOE AS XML	
	DECLARE @sEroganti AS VARCHAR(MAX)
	DECLARE @sPrestazioni AS VARCHAR(MAX)
	DECLARE @sCodStatoOrdine AS VARCHAR(20)
	DECLARE @sCodUtenteInserimento AS VARCHAR(100)	
	DECLARE @sCodUtenteUltimaModifica AS VARCHAR(100)
	DECLARE @sCodUtenteInoltro AS VARCHAR(100)		
	DECLARE @sCodRuolo AS VARCHAR(20)	
	DECLARE @dDataProgrammazioneOE AS DATETIME	
	DECLARE @dDataProgrammazioneOEUTC AS DATETIME
	
	DECLARE @dDataInserimento AS DATETIME	
	DECLARE @dDataInserimentoUTC AS DATETIME	
	DECLARE @dDataUltimaModifica AS DATETIME	
	DECLARE @dDataUltimaModificaUTC AS DATETIME	
	DECLARE @dDataInoltro AS DATETIME	
	DECLARE @dDataInoltroUTC AS DATETIME
	DECLARE @sCodUAInserimento AS VARCHAR(20)
	DECLARE @sCodPriorita AS VARCHAR(20)
	DECLARE @sCodSistema AS VARCHAR(20)
	DECLARE @sIDSistema AS VARCHAR(50)
	DECLARE @sIDGruppo AS VARCHAR(50)
	DECLARE @sInfoSistema AS VARCHAR(50)
	DECLARE @sInfoSistema2 AS VARCHAR(50)
	DECLARE @xDatiDatiAccessori AS XML
	DECLARE @xStrutturaDatiAccessori AS XML
	DECLARE @xLayoutDatiAccessori AS XML
		
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
	
		SET @sIDOrdineOE=(SELECT TOP 1 ValoreParametro.IDOrdineOE.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDOrdineOE') as ValoreParametro(IDOrdineOE))	
	
		SET @sNumeroOrdineOE=(SELECT TOP 1 ValoreParametro.NumeroOrdineOE.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/NumeroOrdineOE') as ValoreParametro(NumeroOrdineOE))	

		SET @xXMLOE=(SELECT TOP 1 ValoreParametro.XMLOE.query('./*') FROM @xParametri.nodes('/Parametri/XMLOE') as ValoreParametro(XMLOE))	

		SET @sEroganti=(SELECT TOP 1 ValoreParametro.Eroganti.value('.','VARCHAR(MAX)')
				  FROM @xParametri.nodes('/Parametri/Eroganti') as ValoreParametro(Eroganti))	
	
		SET @sPrestazioni=(SELECT TOP 1 ValoreParametro.Prestazioni.value('.','VARCHAR(MAX)')
				  FROM @xParametri.nodes('/Parametri/Prestazioni') as ValoreParametro(Prestazioni))	
				  
		SET @sCodStatoOrdine=(SELECT TOP 1 ValoreParametro.CodStatoOrdine.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodStatoOrdine') as ValoreParametro(CodStatoOrdine))	
							
		SET @sCodUtenteInserimento=(SELECT TOP 1 ValoreParametro.CodUtenteInserimento.value('.','VARCHAR(100)')
					  FROM @xParametri.nodes('/Parametri/CodUtenteInserimento') as ValoreParametro(CodUtenteInserimento))	
	
		SET @sCodUtenteUltimaModifica=(SELECT TOP 1 ValoreParametro.CodUtenteUltimaModifica.value('.','VARCHAR(100)')
					  FROM @xParametri.nodes('/Parametri/CodUtenteUltimaModifica') as ValoreParametro(CodUtenteUltimaModifica))	
	
		SET @sCodUtenteInoltro=(SELECT TOP 1 ValoreParametro.CodUtenteInoltro.value('.','VARCHAR(100)')
					  FROM @xParametri.nodes('/Parametri/CodUtenteInoltro') as ValoreParametro(CodUtenteInoltro))	
	
		SET @sCodRuolo=(SELECT TOP 1 ValoreParametro.CodRuolo.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/TimeStamp/CodRuolo') as ValoreParametro(CodRuolo))
					  
		SET @sDataTmp=(SELECT TOP 1 ValoreParametro.DataProgrammazioneOE.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/DataProgrammazioneOE') as ValoreParametro(DataProgrammazioneOE))					  
	SET @sDataTmp=ISNULL(@sDataTmp,'')	

	IF @sDataTmp<> '' 
		BEGIN			
						SET @sDataTmp= SUBSTRING(@sDataTmp,4,2) + '-' 
						+ LEFT(@sDataTmp,2) + '-' +
						+ SUBSTRING(@sDataTmp,7,4) +
						' ' + RIGHT(@sDataTmp,5)
			IF ISDATE(@sDataTmp)=1
				SET	@dDataProgrammazioneOE=CONVERT(DATETIME,@sDataTmp,120)						
			ELSE
				SET	@dDataProgrammazioneOE =NULL			
		END
		
		SET @sDataTmp=(SELECT TOP 1 ValoreParametro.DataProgrammazioneOEUTC.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/DataProgrammazioneOEUTC') as ValoreParametro(DataProgrammazioneOEUTC))					  
	SET @sDataTmp=ISNULL(@sDataTmp,'')	

	IF @sDataTmp<> '' 
		BEGIN			
						SET @sDataTmp= SUBSTRING(@sDataTmp,4,2) + '-' 
						+ LEFT(@sDataTmp,2) + '-' +
						+ SUBSTRING(@sDataTmp,7,4) +
						' ' + RIGHT(@sDataTmp,5)
			IF ISDATE(@sDataTmp)=1
				SET	@dDataProgrammazioneOEUTC=CONVERT(DATETIME,@sDataTmp,120)						
			ELSE
				SET	@dDataProgrammazioneOEUTC =NULL			
		END
					  			
		SET @sDataTmp=(SELECT TOP 1 ValoreParametro.DataInserimento .value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/DataInserimento') as ValoreParametro(DataInserimento))					  
	SET @sDataTmp=ISNULL(@sDataTmp,'')	

	IF @sDataTmp<> '' 
		BEGIN			
						SET @sDataTmp= SUBSTRING(@sDataTmp,4,2) + '-' 
						+ LEFT(@sDataTmp,2) + '-' +
						+ SUBSTRING(@sDataTmp,7,4) +
						' ' + RIGHT(@sDataTmp,5)
			IF ISDATE(@sDataTmp)=1
				SET	@dDataInserimento=CONVERT(DATETIME,@sDataTmp,120)						
			ELSE
				SET	@dDataInserimento =NULL			
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
				SET	@dDataInserimentoUTC=CONVERT(DATETIME,@sDataTmp,120)						
			ELSE
				SET	@dDataInserimentoUTC =NULL			
		END
		

		SET @sDataTmp=(SELECT TOP 1 ValoreParametro.DataUltimaModifica .value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/DataUltimaModifica') as ValoreParametro(DataUltimaModifica))					  
	SET @sDataTmp=ISNULL(@sDataTmp,'')	

	IF @sDataTmp<> '' 
		BEGIN			
						SET @sDataTmp= SUBSTRING(@sDataTmp,4,2) + '-' 
						+ LEFT(@sDataTmp,2) + '-' +
						+ SUBSTRING(@sDataTmp,7,4) +
						' ' + RIGHT(@sDataTmp,5)
			IF ISDATE(@sDataTmp)=1
				SET	@dDataUltimaModifica=CONVERT(DATETIME,@sDataTmp,120)						
			ELSE
				SET	@dDataUltimaModifica =NULL			
		END
		
		SET @sDataTmp=(SELECT TOP 1 ValoreParametro.DataUltimaModificaUTC.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/DataUltimaModificaUTC') as ValoreParametro(DataUltimaModificaUTC))					  
	SET @sDataTmp=ISNULL(@sDataTmp,'')	

	IF @sDataTmp<> '' 
		BEGIN			
						SET @sDataTmp= SUBSTRING(@sDataTmp,4,2) + '-' 
						+ LEFT(@sDataTmp,2) + '-' +
						+ SUBSTRING(@sDataTmp,7,4) +
						' ' + RIGHT(@sDataTmp,5)
			IF ISDATE(@sDataTmp)=1
				SET	@dDataUltimaModificaUTC=CONVERT(DATETIME,@sDataTmp,120)						
			ELSE
				SET	@dDataUltimaModificaUTC =NULL			
		END
			
		SET @sDataTmp=(SELECT TOP 1 ValoreParametro.DataInoltro .value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/DataInoltro') as ValoreParametro(DataInoltro))					  
	SET @sDataTmp=ISNULL(@sDataTmp,'')	

	IF @sDataTmp<> '' 
		BEGIN			
						SET @sDataTmp= SUBSTRING(@sDataTmp,4,2) + '-' 
						+ LEFT(@sDataTmp,2) + '-' +
						+ SUBSTRING(@sDataTmp,7,4) +
						' ' + RIGHT(@sDataTmp,5)
			IF ISDATE(@sDataTmp)=1
				SET	 @dDataInoltro=CONVERT(DATETIME,@sDataTmp,120)						
			ELSE
				SET	 @dDataInoltro =NULL			
		END
		
		SET @sDataTmp=(SELECT TOP 1 ValoreParametro.DataInoltroUTC.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/DataInoltroUTC') as ValoreParametro(DataInoltroUTC))					  
	SET @sDataTmp=ISNULL(@sDataTmp,'')	

	IF @sDataTmp<> '' 
		BEGIN			
						SET @sDataTmp= SUBSTRING(@sDataTmp,4,2) + '-' 
						+ LEFT(@sDataTmp,2) + '-' +
						+ SUBSTRING(@sDataTmp,7,4) +
						' ' + RIGHT(@sDataTmp,5)
			IF ISDATE(@sDataTmp)=1
				SET	@dDataInoltroUTC=CONVERT(DATETIME,@sDataTmp,120)						
			ELSE
				SET	@dDataInoltroUTC =NULL			
		END

		SET @sCodUAInserimento	=(SELECT TOP 1 ValoreParametro.CodUAInserimento.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodUAInserimento') as ValoreParametro(CodUAInserimento))	

		SET @sCodPriorita	=(SELECT TOP 1 ValoreParametro.CodPriorita.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodPriorita') as ValoreParametro(CodPriorita))	

		SET @sCodSistema=(SELECT TOP 1 ValoreParametro.CodSistema.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodSistema') as ValoreParametro(CodSistema))		
	
	IF ISNULL(@sCodSistema,'')='' 
		SET @sCodSistema=NULL
		
		SET @sIDSistema=(SELECT TOP 1 ValoreParametro.IDSistema.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDSistema') as ValoreParametro(IDSistema))		
	IF ISNULL(@sIDSistema,'')='' 
		SET @sIDSistema=NULL
		
		SET @sIDGruppo=(SELECT TOP 1 ValoreParametro.IDGruppo.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDGruppo') as ValoreParametro(IDGruppo))		
	IF ISNULL(@sIDGruppo,'')='' 
		SET @sIDGruppo=NULL
	
		SET @sInfoSistema=(SELECT TOP 1 ValoreParametro.InfoSistema.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/InfoSistema') as ValoreParametro(InfoSistema))
					  
	IF ISNULL(@sInfoSistema,'')='' 
		SET @sInfoSistema=NULL

		SET @sInfoSistema2=(SELECT TOP 1 ValoreParametro.InfoSistema2.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/InfoSistema2') as ValoreParametro(InfoSistema2))
					  
	IF ISNULL(@sInfoSistema2,'')='' 
		SET @sInfoSistema2=NULL

	    SET @xDatiDatiAccessori=(SELECT TOP 1 ValoreParametro.DatiDatiAccessori.query('./*')
					  FROM @xParametri.nodes('/Parametri/DatiDatiAccessori') as ValoreParametro(DatiDatiAccessori))	

	    SET @xStrutturaDatiAccessori=(SELECT TOP 1 ValoreParametro.StrutturaDatiAccessori.query('./*')
					  FROM @xParametri.nodes('/Parametri/StrutturaDatiAccessori') as ValoreParametro(StrutturaDatiAccessori))	

	    SET @xLayoutDatiAccessori=(SELECT TOP 1 ValoreParametro.LayoutDatiAccessori.query('./*')
					  FROM @xParametri.nodes('/Parametri/LayoutDatiAccessori') as ValoreParametro(LayoutDatiAccessori))	

						  
	    SET @xTimeStamp=(SELECT TOP 1 ValoreParametro.TS.query('.')
					  FROM @xParametri.nodes('/Parametri/TimeStamp') as ValoreParametro(TS))
	
	DECLARE @sCodAzione AS VARCHAR(20)
		SET @sCodAzione	=(SELECT TOP 1 ValoreParametro.CodAzione.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/TimeStamp/CodAzione') as ValoreParametro(CodAzione))	
	SET @sCodAzione	=ISNULL(@sCodAzione,'')
							
					
	SET @uGUID=NEWID()

					SET @xTimeStamp.modify('delete (/TimeStamp/IDEntita)[1]') 
			
	SET @xTimeStamp.modify('insert <IDEntita>{sql:variable("@uGUID")}</IDEntita> into (/TimeStamp)[1]')
	
	IF @sCodStatoOrdine='VA' 
	BEGIN
		SET @sCodAzione='VAL'
		SET @xTemp='<CodAzione>VAL</CodAzione>'
		SET @xTimeStamp.modify('delete (/TimeStamp/CodAzione)[1]') 
		SET @xTimeStamp.modify('insert sql:variable("@xTemp") as first into (/TimeStamp)[1]')
		
	END
	ELSE
	IF @sCodStatoOrdine IN ('CA','AN')
	BEGIN				
		SET @sCodAzione='CAN'
		SET @xTemp='<CodAzione>CAN</CodAzione>'
		SET @xTimeStamp.modify('delete (/TimeStamp/CodAzione)[1]') 
		SET @xTimeStamp.modify('insert sql:variable("@xTemp") as first into (/TimeStamp)[1]')	
		SET @sCodUtenteInoltro=NULL	
		SET @dDataInoltro=NULL		
		SET @dDataInoltroUTC=NULL		
	END
	ELSE
	BEGIN				
		SET @sCodAzione='INS'
		SET @xTemp='<CodAzione>INS</CodAzione>'
		SET @xTimeStamp.modify('delete (/TimeStamp/CodAzione)[1]') 
		SET @xTimeStamp.modify('insert sql:variable("@xTemp") as first into (/TimeStamp)[1]')
		SET @sCodUtenteInoltro=NULL			
		SET @dDataInoltro=NULL		
		SET @dDataInoltroUTC=NULL					
	END
	
	
		SET @xTimeStampBase=@xTimeStamp
	
	SET @xTimeStamp=CONVERT(XML,
						'<Parametri>' + CONVERT(varchar(max),@xTimeStamp) +
						'</Parametri>')
													
	BEGIN TRANSACTION
				INSERT INTO T_MovOrdini(
					ID	
					,IDPaziente
					,IDEpisodio
					,IDTrasferimento
					,IDOrdineOE
					,NumeroOrdineOE
					,XMLOE
					,Eroganti
					,Prestazioni					
					,CodStatoOrdine
					,CodUtenteInserimento
					,CodUtenteUltimaModifica
					,CodUtenteInoltro
					,DataProgrammazioneOE
					,DataProgrammazioneOEUTC
					,DataInserimento
					,DataInserimentoUTC
					,DataUltimaModifica
					,DataUltimaModificaUTC
					,DataInoltro
					,DataInoltroUTC
					,CodUAInserimento
					,CodPriorita
					,CodSistema
					,IDSistema
					,IDGruppo
					,InfoSistema
					,InfoSistema2
					,StrutturaDatiAccessori
					,DatiDatiAccessori
					,LayoutDatiAccessori
				  )
		VALUES
				(
					@uGUID														,@uIDPaziente 												,@uIDEpisodio 												,@uIDTrasferimento 											,@sIDOrdineOE												,@sNumeroOrdineOE											,@xXMLOE													,@sEroganti													,@sPrestazioni												,@sCodStatoOrdine											,@sCodUtenteInserimento										,@sCodUtenteUltimaModifica									,@sCodUtenteInoltro											,@dDataProgrammazioneOE										,@dDataProgrammazioneOEUTC									,@dDataInserimento											,@dDataInserimentoUTC										,@dDataUltimaModifica										,@dDataUltimaModificaUTC									,@dDataInoltro												,@dDataInoltroUTC											,@sCodUAInserimento											,@sCodPriorita												,@sCodSistema												,@sIDSistema												,@sIDGruppo													,@sInfoSistema												,@sInfoSistema2
					,@xStrutturaDatiAccessori									,@xDatiDatiAccessori										,@xLayoutDatiAccessori									)
				
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
					(SELECT *, @sCodRuolo AS  CodRuolo
					FROM T_MovOrdini
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