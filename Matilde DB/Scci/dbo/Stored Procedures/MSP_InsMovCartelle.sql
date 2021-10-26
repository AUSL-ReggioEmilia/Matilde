CREATE PROCEDURE [dbo].[MSP_InsMovCartelle](@xParametri XML)
AS
BEGIN
		
	
							
												
								
		DECLARE @sNumeroCartella AS VARCHAR(50)			
	DECLARE @sCodStatoCartella AS VARCHAR(20)
	DECLARE @sCodStatoCartellaInfo AS VARCHAR(20)
	DECLARE @txtPDFCartella AS VARCHAR(MAX)
	DECLARE @uIDCartellaCollegata AS UNIQUEIDENTIFIER		
	
		
		DECLARE @uIDCartella AS UNIQUEIDENTIFIER	
	DECLARE @binPDFCartella AS VARBINARY(MAX)			
	DECLARE @dDataApertura AS DATETIME	
	DECLARE @dDataAperturaUTC AS DATETIME
	DECLARE @dDataChiusura AS DATETIME
	DECLARE @dDataChiusuraUTC AS DATETIME
	DECLARE @sCodUtente AS VARCHAR(100)	
			
	DECLARE @sCodUtenteApertura AS VARCHAR(100)					
	DECLARE @sCodUtenteChiusura AS VARCHAR(100)	
	
	
	DECLARE @sGUID AS VARCHAR(50)
	DECLARE @uGUID AS UNIQUEIDENTIFIER
	DECLARE @sDataTmp AS VARCHAR(20)	
	
	    DECLARE @xTimeStampBase AS XML	
	DECLARE @xTimeStamp AS XML	
		
		DECLARE @xLogPrima AS XML
	DECLARE @xLogDopo AS XML
	DECLARE @xTemp AS XML
	DECLARE @xParLog AS XML
					
		SET @sCodUtente=(SELECT TOP 1 ValoreParametro.CodUtenteRilevazione.value('.','VARCHAR(100)')
					  FROM @xParametri.nodes('/Parametri/TimeStamp/CodLogin') as ValoreParametro(CodUtenteRilevazione))	
	SET @sCodUtente=ISNULL(@sCodUtente,'')

		SET @sNumeroCartella=(SELECT TOP 1 ValoreParametro.NumeroCartella.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/NumeroCartella') as ValoreParametro(NumeroCartella))
			
	
		SET @sCodStatoCartella=(SELECT TOP 1 ValoreParametro.CodStatoCartella.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodStatoCartella') as ValoreParametro(CodStatoCartella))	
	SET @sCodStatoCartella=ISNULL(@sCodStatoCartella,'')
		
	
	IF @sCodStatoCartella='AP'
		BEGIN
			SET @dDataApertura=GETDATE();
			SET @dDataAperturaUTC=GETUTCDATE();
			SET @sCodUtenteApertura=@sCodUtente						
		END
	ELSE	
		IF  @sCodStatoCartella='CH'
			BEGIN
				SET @dDataChiusura=GETDATE();
				SET @dDataChiusuraUTC=GETUTCDATE();
				SET @sCodUtenteChiusura=@sCodUtente						
			END		  							
	
		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDCartellaCollegata.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDCartellaCollegata') as ValoreParametro(IDCartellaCollegata))
	IF 	ISNULL(@sGUID,'') <> '' SET @uIDCartellaCollegata=CONVERT(UNIQUEIDENTIFIER,	@sGUID)	
			
		SET  @txtPDFCartella  =(SELECT TOP 1 ValoreParametro.PDFCartella.value('.','VARCHAR(MAX)')
					  FROM @xParametri.nodes('/Parametri/PDFCartella ') as ValoreParametro(PDFCartella)) 	
	
	SET @binPDFCartella=CAST(N'' AS xml).value('xs:base64Binary(sql:variable("@txtPDFCartella"))', 'varbinary(max)')		
		
		
		SET @sCodStatoCartellaInfo=(SELECT TOP 1 ValoreParametro.CodStatoCartellaInfo.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodStatoCartellaInfo') as ValoreParametro(CodStatoCartellaInfo))		
							
    SET @xTimeStamp=(SELECT TOP 1 ValoreParametro.TS.query('.')
					  FROM @xParametri.nodes('/Parametri/TimeStamp') as ValoreParametro(TS))						  
					   					
					
	SET @uGUID=NEWID()

					SET @xTimeStamp.modify('delete (/TimeStamp/IDEntita)[1]') 				
	SET @xTimeStamp.modify('insert <IDEntita>{sql:variable("@uGUID")}</IDEntita> into (/TimeStamp)[1]')

		SET @xTimeStamp.modify('delete (/TimeStamp/CodEntita)[1]') 						
	SET @xTimeStamp.modify('insert <CodEntita>CAR</CodEntita> into (/TimeStamp)[1]')
	
		SET @xTimeStampBase=@xTimeStamp
	
	SET @xTimeStamp=CONVERT(XML,
						'<Parametri>' + CONVERT(varchar(max),@xTimeStamp) +
						'</Parametri>')
													
	BEGIN TRANSACTION
				INSERT INTO T_MovCartelle(	
					  ID				  
					  ,NumeroCartella
					  ,CodStatoCartella
					  ,CodUtenteApertura
					  ,CodUtenteChiusura
					  ,DataApertura
					  ,DataAperturaUTC
					  ,DataChiusura
					  ,DataChiusuraUTC
					  ,PDFCartella
					  ,CodStatoCartellaInfo
				  )
		VALUES
		
				(
					   @uGUID														   ,@sNumeroCartella											   ,@sCodStatoCartella											   ,@sCodUtenteApertura											   ,@sCodUtenteChiusura											   ,@dDataApertura												   ,@dDataAperturaUTC											   ,@dDataChiusura												   ,@dDataChiusuraUTC											   ,@binPDFCartella												   ,@sCodStatoCartellaInfo									)
				
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
					  ,NumeroCartella
					  ,CodStatoCartella
					  ,CodUtenteApertura
					  ,CodUtenteChiusura
					  ,DataApertura
					  ,DataAperturaUTC
					  ,DataChiusura
					  ,DataChiusuraUTC
					  					  ,CodStatoCartellaInfo
					  FROM T_MovCartelle
					 WHERE ID=@uGUID
					) AS [Table]
				FOR XML AUTO, ELEMENTS)

			SET @xLogDopo.modify('insert sql:variable("@xTemp") as first into (/DataSet)[1]')
	
						
			SET @xParLog=CONVERT(XML,'<Parametri><LogDopo/></Parametri>')			
			SET @xParLog.modify('insert sql:variable("@xLogDopo") as first into (/Parametri/LogDopo)[1]')
			SET @xParLog.modify('insert sql:variable("@xTimeStampBase") as last into (/Parametri)[1]')
			
						IF @uIDCartellaCollegata IS NOT NULL
			BEGIN				
								SET @xParLog.modify('delete (/Parametri/TimeStamp/CodAzione)[1]') 				
				SET @xParLog.modify('insert <CodAzione>COP</CodAzione> into (/Parametri/TimeStamp)[1]')

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