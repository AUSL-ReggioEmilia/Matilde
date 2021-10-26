CREATE PROCEDURE [dbo].[MSP_InsMovDocumentiFirmati](@xParametri XML)
AS
BEGIN
		
	
							
												
								
		DECLARE @sCodEntita AS VARCHAR(20)
	DECLARE @uIDEntita AS UNIQUEIDENTIFIER		
	DECLARE @sCodTipoDocumentoFirmato AS VARCHAR(20)
	DECLARE @sCodStatoEntita AS VARCHAR(20)
	DECLARE @txtPDFFirmato AS VARCHAR(MAX)
	DECLARE @txtPDFNonFirmato AS VARCHAR(MAX)
	
		
		DECLARE @uIDDocumento AS UNIQUEIDENTIFIER	
	DECLARE @binPDFFirmato AS VARBINARY(MAX)			
	DECLARE @binPDFNonFirmato AS VARBINARY(MAX)
	DECLARE @dDataInserimento AS DATETIME	
	DECLARE @dDataInserimentoUTC AS DATETIME

	DECLARE @sCodUtenteInserimento AS VARCHAR(100)				
	
	
	DECLARE @sGUID AS VARCHAR(50)
	DECLARE @uGUID AS UNIQUEIDENTIFIER
	DECLARE @sDataTmp AS VARCHAR(20)	
	DECLARE @nNumero AS INTEGER
	
	    DECLARE @xTimeStampBase AS XML	
	DECLARE @xTimeStamp AS XML	
		
		DECLARE @xLogPrima AS XML
	DECLARE @xLogDopo AS XML
	DECLARE @xTemp AS XML
	DECLARE @xParLog AS XML
					
		SET @sCodUtenteInserimento=(SELECT TOP 1 ValoreParametro.CodUtenteInserimento.value('.','VARCHAR(100)')
					  FROM @xParametri.nodes('/Parametri/TimeStamp/CodLogin') as ValoreParametro(CodUtenteInserimento))				
	
		SET @sCodEntita=(SELECT TOP 1 ValoreParametro.CodEntita.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodEntita') as ValoreParametro(CodEntita))		
		
		SET @sCodTipoDocumentoFirmato=(SELECT TOP 1 ValoreParametro.CodTipoDocumentoFirmato.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodTipoDocumentoFirmato') as ValoreParametro(CodTipoDocumentoFirmato))		
	
		SET @sCodStatoEntita=(SELECT TOP 1 ValoreParametro.CodStatoEntita.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodStatoEntita') as ValoreParametro(CodStatoEntita))		


		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDEntita.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDEntita') as ValoreParametro(IDEntita))
	IF 	ISNULL(@sGUID,'') <> '' SET @uIDEntita=CONVERT(UNIQUEIDENTIFIER,@sGUID)	
	
		SET  @txtPDFFirmato  =(SELECT TOP 1 ValoreParametro.PDFFirmato.value('.','VARCHAR(MAX)')
					  FROM @xParametri.nodes('/Parametri/PDFFirmato ') as ValoreParametro(PDFFirmato)) 	
	
	SET @binPDFFirmato=CAST(N'' AS xml).value('xs:base64Binary(sql:variable("@txtPDFFirmato"))', 'varbinary(max)')		
	
		SET  @txtPDFNonFirmato  =(SELECT TOP 1 ValoreParametro.PDFNonFirmato.value('.','VARCHAR(MAX)')
					  FROM @xParametri.nodes('/Parametri/PDFNonFirmato') as ValoreParametro(PDFNonFirmato)) 	
	
	SET @binPDFNonFirmato=CAST(N'' AS xml).value('xs:base64Binary(sql:variable("@txtPDFNonFirmato"))', 'varbinary(max)')		
	

		SET @dDataInserimento=GETDATE();
	SET @dDataInserimentoUTC=GETUTCDATE();
	
							
    SET @xTimeStamp=(SELECT TOP 1 ValoreParametro.TS.query('.')
					  FROM @xParametri.nodes('/Parametri/TimeStamp') as ValoreParametro(TS))						  
					   					
					
	SET @uGUID=NEWID()

					SET @xTimeStamp.modify('delete (/TimeStamp/IDEntita)[1]') 				
	SET @xTimeStamp.modify('insert <IDEntita>{sql:variable("@uGUID")}</IDEntita> into (/TimeStamp)[1]')

		SET @xTimeStamp.modify('delete (/TimeStamp/CodEntita)[1]') 						
	SET @xTimeStamp.modify('insert <CodEntita>DCF</CodEntita> into (/TimeStamp)[1]')
	
		SET @xTimeStampBase=@xTimeStamp
	
	SET @xTimeStamp=CONVERT(XML,
						'<Parametri>' + CONVERT(varchar(max),@xTimeStamp) +
						'</Parametri>')

		SET @nNumero=(SELECT MAX(Numero) FROM T_movDocumentiFirmati WHERE CodEntita=@sCodEntita AND IDEntita=@uIDEntita AND CodTipoDocumentoFirmato=@sCodTipoDocumentoFirmato)
	SET @nNumero=ISNULL(@nNumero,0) + 1
	
														
	BEGIN TRANSACTION
				INSERT INTO T_MovDocumentiFirmati(	
					  ID				  					 
					  ,CodEntita
					  ,IDEntita
					  ,CodTipoDocumentoFirmato
					  ,Numero
					  ,PDFFirmato
					  ,CodUtenteInserimento
					  ,DataInserimento
					  ,DataInserimentoUTC
					  ,CodStatoEntita
					  ,PDFNonFirmato
					 )				  
		VALUES
		
				(
					   @uGUID												   ,@sCodEntita											   ,@uIDEntita											   ,@sCodTipoDocumentoFirmato							   ,@nNumero											   ,@binPDFFirmato										   ,@sCodUtenteInserimento								   ,@dDataInserimento									   ,@dDataInserimentoUTC								   ,@sCodStatoEntita									   ,@binPDFNonFirmato								)
				
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
					(SELECT 
					   ID				  					 
					  ,CodEntita
					  ,IDEntita
					  ,Numero
					  ,CodTipoDocumentoFirmato
					  					  ,CodUtenteInserimento
					  ,DataInserimento
					  ,DataInserimentoUTC
					  ,CodStatoEntita
					  					  					  FROM T_MovDocumentiFirmati
					 WHERE ID=@uGUID
					) AS [Table]
				FOR XML AUTO, ELEMENTS)

			SET @xLogDopo.modify('insert sql:variable("@xTemp") as first into (/DataSet)[1]')
							
			SET @xParLog=CONVERT(XML,'<Parametri><LogDopo/></Parametri>')			
			SET @xParLog.modify('insert sql:variable("@xLogDopo") as first into (/Parametri/LogDopo)[1]')
			SET @xParLog.modify('insert sql:variable("@xTimeStampBase") as last into (/Parametri)[1]')
			
			SET @xParLog.modify('delete (/Parametri/TimeStamp/CodAzione)[1]') 				
			SET @xParLog.modify('insert <CodAzione>INS</CodAzione> into (/Parametri/TimeStamp)[1]')			
	
									
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