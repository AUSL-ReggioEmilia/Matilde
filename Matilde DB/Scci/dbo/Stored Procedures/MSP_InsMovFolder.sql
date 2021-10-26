CREATE PROCEDURE [dbo].[MSP_InsMovFolder](@xParametri XML)
AS
BEGIN
		
	
											
		DECLARE @uIDPaziente AS UNIQUEIDENTIFIER	
	DECLARE @uIDEpisodio AS UNIQUEIDENTIFIER	
	DECLARE @uIDTrasferimento AS UNIQUEIDENTIFIER	
	DECLARE @uIDFolderPadre AS UNIQUEIDENTIFIER	
	DECLARE @sDescrizione AS VARCHAR(50)
	DECLARE @sCodStatoFolder AS VARCHAR(20)
	DECLARE @sCodEntita AS VARCHAR(20)
	DECLARE @sCodUA AS VARCHAR(20)
	
	DECLARE @sCodUtenteRilevazione AS VARCHAR(100)		
					
		DECLARE @sGUID AS VARCHAR(50)
	DECLARE @uGUID AS UNIQUEIDENTIFIER
		
		
	    DECLARE @xTimeStampBase AS XML	
	DECLARE @xTimeStamp AS XML	
		
		DECLARE @xLogPrima AS XML
	DECLARE @xLogDopo AS XML
	DECLARE @xTemp AS XML
	DECLARE @xParLog AS XML


		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDPaziente.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDPaziente') as ValoreParametro(IDPaziente))
	IF 	ISNULL(@sGUID,'') <> '' SET @uIDPaziente=CONVERT(UNIQUEIDENTIFIER,	@sGUID)		
		
		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDEpisodio.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDEpisodio') as ValoreParametro(IDEpisodio))
	IF 	ISNULL(@sGUID,'') <> '' SET @uIDEpisodio=CONVERT(UNIQUEIDENTIFIER,	@sGUID)		
		
		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDTrasferimento.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDTrasferimento') as ValoreParametro(IDTrasferimento))
	IF 	ISNULL(@sGUID,'') <> '' SET @uIDTrasferimento=CONVERT(UNIQUEIDENTIFIER,	@sGUID)	

		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDFolderPadre.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDFolderPadre') as ValoreParametro(IDFolderPadre))
	IF 	ISNULL(@sGUID,'') <> '' SET @uIDFolderPadre=CONVERT(UNIQUEIDENTIFIER,	@sGUID)	

		SET @sDescrizione=(SELECT TOP 1 ValoreParametro.Descrizione.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/Descrizione') as ValoreParametro(Descrizione))	
	SET @sDescrizione=ISNULL(@sDescrizione,'')
					
		SET @sCodStatoFolder=(SELECT TOP 1 ValoreParametro.CodStatoFolder.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodStatoFolder') as ValoreParametro(CodStatoFolder))	
	SET @sCodStatoFolder=ISNULL(@sCodStatoFolder,'')
	IF @sCodStatoFolder='' SET @sCodStatoFolder='AT'		
		SET @sCodEntita=(SELECT TOP 1 ValoreParametro.CodEntita.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodEntita	') as ValoreParametro(CodEntita))		
								
		SET @sCodUA=(SELECT TOP 1 ValoreParametro.CodUA.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodUA') as ValoreParametro(CodUA))	
								
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
				INSERT INTO T_MovFolder(
					   ID	
					  ,IDPaziente 				  
					  ,IDEpisodio
					  ,IDTrasferimento
					  ,IDFolderPadre
					  ,Descrizione
					  ,CodStatoFolder
					  ,CodEntita
					  ,CodUA
					  ,CodUtenteRilevazione
					  ,CodUtenteUltimaModifica
					  ,DataRilevazione
					  ,DataRilevazioneUTC
					  ,DataUltimaModifica
					  ,DataUltimaModificaUTC					  
				  )
		VALUES
				( 
						@uGUID																,@uIDPaziente 														,@uIDEpisodio 														,@uIDTrasferimento 													,@uIDFolderPadre 													,@sDescrizione														,@sCodStatoFolder													,@sCodEntita														,@sCodUA															,@sCodUtenteRilevazione 											,NULL 																,Getdate()															,GetUTCdate()														,NULL 																,NULL 															  					  		  						
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
					  ,IDFolderPadre
					  ,Descrizione
					  ,CodStatoFolder
					  ,CodEntita
					  ,CodUA
					  ,CodUtenteRilevazione
					  ,CodUtenteUltimaModifica
					  ,DataUltimaModifica
					  ,DataUltimaModificaUTC 
					 FROM T_MovFolder
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