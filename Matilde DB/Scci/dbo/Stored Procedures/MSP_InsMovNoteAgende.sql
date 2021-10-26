CREATE PROCEDURE [dbo].[MSP_InsMovNoteAgende](@xParametri XML)
AS
BEGIN
		
	
											
	
		
	DECLARE @dDataEvento AS DATETIME	
	DECLARE @dDataEventoUTC AS DATETIME
	DECLARE @dDataInizio AS DATETIME	
	DECLARE @dDataFine AS DATETIME
	
	DECLARE @binOggetto AS VARBINARY(MAX)	
	DECLARE @binDescrizione AS VARBINARY(MAX)
	DECLARE @binColore AS  VARBINARY(MAX)
	DECLARE @uIDGruppo AS UNIQUEIDENTIFIER
	DECLARE @sCodStatoNota AS VARCHAR(20)
	DECLARE @sCodAgenda AS VARCHAR(20)	
		
	DECLARE @sCodUtenteRilevazione AS VARCHAR(100)		
	DECLARE @bRicorrenza AS BIT	
	DECLARE @bTuttoIlGiorno AS BIT					
	DECLARE @bEscludiDisponibilita AS BIT					
		
		DECLARE @sGUID AS VARCHAR(50)
	DECLARE @uGUID AS UNIQUEIDENTIFIER
	DECLARE @sDataTmp AS VARCHAR(20)	
	
	    DECLARE @xTimeStampBase AS XML	
	DECLARE @xTimeStamp AS XML	
		
		DECLARE @xLogPrima AS XML
	DECLARE @xLogDopo AS XML
	DECLARE @xTemp AS XML
	DECLARE @xParLog AS XML


		SET @bRicorrenza	=(SELECT TOP 1 ValoreParametro.Ricorrenza.value('.','BIT')
					  FROM @xParametri.nodes('/Parametri/Ricorrenza') as ValoreParametro(Ricorrenza	))
	SET @bRicorrenza=ISNULL(@bRicorrenza,0)
	
		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDGruppo.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDGruppo') as ValoreParametro(IDGruppo))
	IF 	ISNULL(@sGUID,'') <> '' SET @uIDGruppo	=CONVERT(UNIQUEIDENTIFIER,	@sGUID)					
	
		IF @uIDGruppo IS NULL AND @bRicorrenza=1
		SET @uIDGruppo=(SELECT NEWID())
				
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
		
		SET @sDataTmp=(SELECT TOP 1 ValoreParametro.DataInizio.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/DataInizio') as ValoreParametro(DataInizio))					  
	SET @sDataTmp=ISNULL(@sDataTmp,'')	

	IF @sDataTmp<> '' 
		BEGIN			
						SET @sDataTmp= SUBSTRING(@sDataTmp,4,2) + '-' 
						+ LEFT(@sDataTmp,2) + '-' +
						+ SUBSTRING(@sDataTmp,7,4) +
						' ' + RIGHT(@sDataTmp,5)
			IF ISDATE(@sDataTmp)=1
				SET	@dDataInizio=CONVERT(DATETIME,@sDataTmp,120)						
			ELSE
				SET	@dDataInizio =NULL			
		END
	
		SET @sDataTmp=(SELECT TOP 1 ValoreParametro.DataFine.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/DataFine') as ValoreParametro(DataFine))					  
	SET @sDataTmp=ISNULL(@sDataTmp,'')	

	IF @sDataTmp<> '' 
		BEGIN			
						SET @sDataTmp= SUBSTRING(@sDataTmp,4,2) + '-' 
						+ LEFT(@sDataTmp,2) + '-' +
						+ SUBSTRING(@sDataTmp,7,4) +
						' ' + RIGHT(@sDataTmp,5)
			IF ISDATE(@sDataTmp)=1
				SET	@dDataFine=CONVERT(DATETIME,@sDataTmp,120)						
			ELSE
				SET	@dDataFine =NULL			
	END
	
		SET @sCodAgenda=(SELECT TOP 1 ValoreParametro.CodAgenda.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodAgenda') as ValoreParametro(CodAgenda))	
	SET @sCodAgenda=ISNULL(@sCodAgenda,'')
			
		SET @binOggetto=(SELECT TOP 1 ValoreParametro.Oggetto.value('.','VARBINARY(MAX)')
					  FROM @xParametri.nodes('/Parametri/Oggetto') as ValoreParametro(Oggetto))	

		SET @binDescrizione=(SELECT TOP 1 ValoreParametro.Descrizione.value('.','VARBINARY(MAX)')
					  FROM @xParametri.nodes('/Parametri/Descrizione') as ValoreParametro(Descrizione))	
										
		SET  @binColore=(SELECT TOP 1 ValoreParametro.Colore.value('.','VARBINARY(MAX)')
					  FROM @xParametri.nodes('/Parametri/Colore') as ValoreParametro(Colore))	
					  
		SET @sCodStatoNota=(SELECT TOP 1 ValoreParametro.CodStatoNota.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodStatoNota') as ValoreParametro(CodStatoNota))	
	SET @sCodStatoNota=ISNULL(@sCodStatoNota,'')
	IF @sCodStatoNota='' SET @sCodStatoNota='PR'
			SET @bTuttoIlGiorno	=(SELECT TOP 1 ValoreParametro.TuttoIlGiorno.value('.','BIT')
					  FROM @xParametri.nodes('/Parametri/TuttoIlGiorno') as ValoreParametro(TuttoIlGiorno))
	SET @bTuttoIlGiorno=ISNULL(@bTuttoIlGiorno,0)

		SET @bEscludiDisponibilita	=(SELECT TOP 1 ValoreParametro.EscludiDisponibilita.value('.','BIT')
					  FROM @xParametri.nodes('/Parametri/EscludiDisponibilita') as ValoreParametro(EscludiDisponibilita))
	SET @bEscludiDisponibilita=ISNULL(@bEscludiDisponibilita,0)
					  
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
				INSERT INTO T_MovNoteAgende(
					ID     
				  
				  ,CodAgenda
				  ,CodStatoNota
  				  ,Oggetto
				  ,Descrizione		
				  ,Colore		  				 				  
				  ,IDGruppo

				  ,DataEvento
				  ,DataEventoUTC
				  ,DataInizio
				  ,DataFine
				  
				  ,CodUtenteRilevazione
				  ,CodUtenteUltimaModifica
				  ,DataUltimaModifica
				  ,DataUltimaModificaUTC
				  ,TuttoIlGiorno
				  ,EscludiDisponibilita
				  )
		VALUES
		
				(
				
					  @uGUID												  
					  ,@sCodAgenda											  ,@sCodStatoNota					  					  ,CONVERT(VARCHAR(MAX),@binOggetto)  											  ,CONVERT(VARCHAR(MAX),@binDescrizione)  										  ,CONVERT(VARCHAR(MAX),@binColore)											  ,@uIDGruppo											  ,ISNULL(@dDataEvento,Getdate())								  ,ISNULL(@dDataEventoUTC,GetUTCdate())							  ,@dDataInizio											  ,@dDataFine											  
					  ,@sCodUtenteRilevazione								  ,NULL													  ,NULL													  ,NULL													  ,@bTuttoIlGiorno										  ,@bEscludiDisponibilita										  									  						
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
					(SELECT*
					FROM T_MovNoteAgende
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