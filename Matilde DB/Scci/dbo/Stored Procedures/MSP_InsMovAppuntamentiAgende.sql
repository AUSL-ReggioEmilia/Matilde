CREATE PROCEDURE [dbo].[MSP_InsMovAppuntamentiAgende](@xParametri XML)
AS
BEGIN
		
	
											
		DECLARE @uIDAppuntamento AS UNIQUEIDENTIFIER	
	DECLARE @sCodAgenda AS VARCHAR(20)	
		DECLARE @sCodStatoAppuntamentoAgenda AS VARCHAR(20)

	DECLARE @sCodRaggr1 AS VARCHAR(20)
	DECLARE @binDescrRaggr1 VARBINARY(MAX)	
	
	DECLARE @sCodRaggr2 AS VARCHAR(20)
	DECLARE @binDescrRaggr2 VARBINARY(MAX)
	
	DECLARE @sCodRaggr3 AS VARCHAR(20)
	DECLARE @binDescrRaggr3 VARBINARY(MAX)
	
	DECLARE @sCodUtenteRilevazione AS VARCHAR(100)					
		
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

			
		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDAppuntamento.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDAppuntamento') as ValoreParametro(IDAppuntamento))
	IF 	ISNULL(@sGUID,'') <> '' 
			SET @uIDAppuntamento=CONVERT(UNIQUEIDENTIFIER,	@sGUID)		
	
		SET @sCodAgenda=(SELECT TOP 1 ValoreParametro.CodAgenda.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodAgenda') as ValoreParametro(CodAgenda))	
	SET @sCodAgenda=ISNULL(@sCodAgenda,'')
	
		
				
		SET @sCodStatoAppuntamentoAgenda=(SELECT TOP 1 ValoreParametro.CodStatoAppuntamentoAgenda.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodStatoAppuntamentoAgenda') as ValoreParametro(CodStatoAppuntamentoAgenda))	
	SET @sCodStatoAppuntamentoAgenda=ISNULL(@sCodStatoAppuntamentoAgenda,'')
	IF @sCodStatoAppuntamentoAgenda='' SET @sCodStatoAppuntamentoAgenda='PR'					
		SET @sCodUtenteRilevazione=(SELECT TOP 1 ValoreParametro.CodUtenteRilevazione.value('.','VARCHAR(100)')
					  FROM @xParametri.nodes('/Parametri/TimeStamp/CodLogin') as ValoreParametro(CodUtenteRilevazione))	
	SET @sCodUtenteRilevazione=ISNULL(@sCodUtenteRilevazione,'')
	
		SET @sCodRaggr1=(SELECT TOP 1 ValoreParametro.CodRaggr1.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/CodRaggr1') as ValoreParametro(CodRaggr1))	

		SET @binDescrRaggr1=(SELECT TOP 1 ValoreParametro.DescrRaggr1.value('.','VARBINARY(MAX)')
					  FROM @xParametri.nodes('/Parametri/DescrRaggr1') as ValoreParametro(DescrRaggr1))	

		SET @sCodRaggr2=(SELECT TOP 1 ValoreParametro.CodRaggr2.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/CodRaggr2') as ValoreParametro(CodRaggr2))	
		SET @binDescrRaggr2=(SELECT TOP 1 ValoreParametro.DescrRaggr2.value('.','VARBINARY(MAX)')
					  FROM @xParametri.nodes('/Parametri/DescrRaggr2') as ValoreParametro(DescrRaggr2))	

		SET @sCodRaggr3=(SELECT TOP 1 ValoreParametro.CodRaggr3.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/CodRaggr3') as ValoreParametro(CodRaggr3))	

		SET @binDescrRaggr3=(SELECT TOP 1 ValoreParametro.DescrRaggr3.value('.','VARBINARY(MAX)')
					  FROM @xParametri.nodes('/Parametri/DescrRaggr3') as ValoreParametro(DescrRaggr3))	

	    SET @xTimeStamp=(SELECT TOP 1 ValoreParametro.TS.query('.')
					  FROM @xParametri.nodes('/Parametri/TimeStamp') as ValoreParametro(TS))
	
		SET @xSchedaMovimento=(SELECT TOP 1 ValoreParametro.TS.query('.')
					  FROM @xParametri.nodes('/Parametri/SchedaMovimento') as ValoreParametro(TS))								
						
					
	SET @uGUID=NEWID()

					SET @xTimeStamp.modify('delete (/TimeStamp/IDEntita)[1]') 
			
	SET @xTimeStamp.modify('insert <IDEntita>{sql:variable("@uGUID")}</IDEntita> into (/TimeStamp)[1]')
	
		SET @xTimeStampBase=@xTimeStamp
	
	SET @xTimeStamp=CONVERT(XML,
						'<Parametri>' + CONVERT(varchar(max),@xTimeStamp) +
						'</Parametri>')
													
	BEGIN TRANSACTION
				INSERT INTO T_MovAppuntamentiAgende(
				  ID
				  ,IDAppuntamento
				  ,CodAgenda
				  				   ,CodStatoAppuntamentoAgenda
				  ,CodUtenteRilevazione
				  ,CodUtenteUltimaModifica
				  ,DataUltimaModifica
				  ,DataUltimaModificaUTC
				  ,CodRaggr1
				  ,DescrRaggr1
				  ,CodRaggr2
				  ,DescrRaggr2
				  ,CodRaggr3
				  ,DescrRaggr3
				  )
		VALUES		
				(	   
					   @uGUID
					  ,@uIDAppuntamento 											  ,@sCodAgenda 												  					   ,@sCodStatoAppuntamentoAgenda										  ,@sCodUtenteRilevazione										  ,NULL															  ,NULL															  ,NULL															  ,@sCodRaggr1
					  ,CONVERT(VARCHAR(MAX),@binDescrRaggr1) 
					  ,@sCodRaggr2
					  ,CONVERT(VARCHAR(MAX),@binDescrRaggr2) 
					  ,@sCodRaggr3
					  ,CONVERT(VARCHAR(MAX),@binDescrRaggr3) 
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
					(SELECT *
					FROM T_MovAppuntamentiAgende
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