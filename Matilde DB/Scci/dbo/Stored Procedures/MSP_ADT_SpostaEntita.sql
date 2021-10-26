
CREATE PROCEDURE [dbo].[MSP_ADT_SpostaEntita](@xParametri XML)
AS
BEGIN
	

	DECLARE @uIDCartellaOrigine AS UNIQUEIDENTIFIER	
	DECLARE @uIDEpisodioOrigine AS UNIQUEIDENTIFIER	
	DECLARE @uIDEpisodioDestinazione AS UNIQUEIDENTIFIER	
	DECLARE @uIDTrasferimentoDestinazione AS UNIQUEIDENTIFIER

	DECLARE @xTimeStampBase AS XML	
	DECLARE @xTimeStamp AS XML	

		DECLARE @sGUID AS VARCHAR(50)
	DECLARE @uGUID AS UNIQUEIDENTIFIER
	DECLARE @sCodEntita VARCHAR(20)	

		DECLARE @xLogPrima AS XML
	DECLARE @xLogDopo AS XML
	DECLARE @xTemp AS XML
	DECLARE @xParLog AS XML			

		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDCartellaOrigine.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDCartellaOrigine') as ValoreParametro(IDCartellaOrigine))
	IF 	ISNULL(@sGUID,'') <> '' SET @uIDCartellaOrigine=CONVERT(UNIQUEIDENTIFIER,	@sGUID)		  

		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDEpisodioOrigine.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDEpisodioOrigine') as ValoreParametro(IDEpisodioOrigine))
	IF 	ISNULL(@sGUID,'') <> '' SET @uIDEpisodioOrigine=CONVERT(UNIQUEIDENTIFIER,	@sGUID)		  


		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDEpisodioDestinazione.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDEpisodioDestinazione') as ValoreParametro(IDEpisodioDestinazione))
	IF 	ISNULL(@sGUID,'') <> '' SET @uIDEpisodioDestinazione=CONVERT(UNIQUEIDENTIFIER,	@sGUID)		  

			SET @sGUID=(SELECT TOP 1 ValoreParametro.IDTrasferimentoDestinazione.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDTrasferimentoDestinazione') as ValoreParametro(IDTrasferimentoDestinazione))
	IF 	ISNULL(@sGUID,'') <> '' SET @uIDTrasferimentoDestinazione=CONVERT(UNIQUEIDENTIFIER,	@sGUID)		  


	    SET @xTimeStamp=(SELECT TOP 1 ValoreParametro.TS.query('.')
					  FROM @xParametri.nodes('/Parametri/TimeStamp') as ValoreParametro(TS))

	
			
	

		SET @xTimeStamp.modify('delete (/TimeStamp/CodAzione)[1]') 			
	SET @xTimeStamp.modify('insert <CodAzione>MOD</CodAzione> into (/TimeStamp)[1]')

		SET @xTimeStamp.modify('delete (/TimeStamp/CodEntita)[1]') 			
	SET @xTimeStamp.modify('insert <CodEntita>ADT</CodEntita> into (/TimeStamp)[1]')

		SET @xTimeStampBase=@xTimeStamp

	SET @xTimeStamp=CONVERT(XML,
						'<Parametri>' + CONVERT(varchar(max),@xTimeStamp) +
						'</Parametri>')


		CREATE TABLE #tmpEntita(
		DataElaborazione DATETIME,
		CodEntita VARCHAR(20),
		IDEntita UNIQUEIDENTIFIER,
		IDEpisodioOrigine UNIQUEIDENTIFIER,
		IDTrasfertimentoOrigine  UNIQUEIDENTIFIER,
		IDEpisodioDestinazione UNIQUEIDENTIFIER,
		IDTrasfertimentoDestinazione  UNIQUEIDENTIFIER,
		CodEntitaRiferimento VARCHAR(20),
		IDEntitaRiferimento UNIQUEIDENTIFIER
	)

	CREATE INDEX IX_Chiave ON #tmpEntita(CodEntita,IDEntita)	

			
	SET @sCodEntita='ALG'

	TRUNCATE TABLE #tmpEntita			
	INSERT INTO #tmpEntita(DataElaborazione,CodEntita,IDEntita,IDEpisodioOrigine,IDTrasfertimentoOrigine,IDEpisodioDestinazione,IDTrasfertimentoDestinazione,CodEntitaRiferimento,IDEntitaRiferimento)
	SELECT
		GETDATE() AS DataElaborazione,
		@sCodEntita AS CodEntita,
		ID AS IDEntita,
		IDEpisodio,
		IDTrasferimento,
		@uIDEpisodioDestinazione AS IDEpisodioDestinazione,
		@uIDTrasferimentoDestinazione AS IDTrasfertimentoDestinazione,
		NULL AS CodEntitaRiferimento,
		NULL AS IDEntitaRiferimento
	FROM T_MovAlertGenerici
	WHERE IDEpisodio= @uIDEpisodioOrigine


	UPDATE T_MovAlertGenerici
	SET IDEpisodio=@uIDEpisodioDestinazione,
		IDTrasferimento=@uIDTrasferimentoDestinazione
	WHERE ID IN (SELECT IDEntita FROM #tmpEntita WHERE CodEntita=@sCodEntita)


	INSERT INTO #tmpEntita(DataElaborazione,CodEntita,IDEntita,IDEpisodioOrigine,IDTrasfertimentoOrigine,IDEpisodioDestinazione,IDTrasfertimentoDestinazione,CodEntitaRiferimento,IDEntitaRiferimento)
	SELECT
		GETDATE() AS DataElaborazione,
		'SCH' AS CodEntita,
		ID AS IDEntita,
		IDEpisodio,
		IDTrasferimento,
		@uIDEpisodioDestinazione AS IDEpisodioDestinazione,
		@uIDTrasferimentoDestinazione AS IDTrasfertimentoDestinazione,
		CodEntita AS CodEntitaRiferimento,
		IDEntita AS IDEntitaRiferimento
	FROM T_MovSchede
	WHERE 		
		CodEntita=@sCodEntita AND
		IDEntita IN (SELECT IDEntita FROM #tmpEntita WHERE CodEntita=@sCodEntita)

	UPDATE T_MovSchede
	SET IDEpisodio=@uIDEpisodioDestinazione,
		IDTrasferimento=@uIDTrasferimentoDestinazione
	WHERE
		 CodEntita=@sCodEntita AND
		 ID IN (SELECT IDEntita FROM #tmpEntita WHERE CodEntita='SCH' AND CodEntitaRiferimento=@sCodEntita)

		SET @xLogDopo=Convert(XML,'<DataSet></DataSet>')
	SET @xLogPrima=''
			
	SET @xTemp=
		(SELECT * FROM 
			(SELECT * FROM #tmpEntita
			) AS [Table]
		FOR XML AUTO, ELEMENTS)

	SET @xLogDopo.modify('insert sql:variable("@xTemp") as first into (/DataSet)[1]')
	
				
	SET @xParLog=CONVERT(XML,'<Parametri><LogDopo/></Parametri>')			
	SET @xParLog.modify('insert sql:variable("@xLogDopo") as first into (/Parametri/LogDopo)[1]')
	SET @xParLog.modify('insert sql:variable("@xTimeStampBase") as last into (/Parametri)[1]')
	
	SELECT @xParLog
	EXEC MSP_InsMovLog @xParLog
		
			
	SET @sCodEntita='ALL'

	TRUNCATE TABLE #tmpEntita				
	INSERT INTO #tmpEntita(DataElaborazione,CodEntita,IDEntita,IDEpisodioOrigine,IDTrasfertimentoOrigine,IDEpisodioDestinazione,IDTrasfertimentoDestinazione,CodEntitaRiferimento,IDEntitaRiferimento)
	SELECT
		GETDATE() AS DataElaborazione,
		@sCodEntita AS CodEntita,
		ID AS IDEntita,
		IDEpisodio,
		IDTrasferimento,
		@uIDEpisodioDestinazione AS IDEpisodioDestinazione,
		@uIDTrasferimentoDestinazione AS IDTrasfertimentoDestinazione,
		NULL AS CodEntitaRiferimento,
		NULL AS IDEntitaRiferimento
	FROM T_MovAllegati
	WHERE IDEpisodio= @uIDEpisodioOrigine


	UPDATE T_MovAllegati
	SET IDEpisodio=@uIDEpisodioDestinazione,
		IDTrasferimento=@uIDTrasferimentoDestinazione
	WHERE ID IN (SELECT IDEntita FROM #tmpEntita WHERE CodEntita=@sCodEntita)

	
		SET @xLogDopo=Convert(XML,'<DataSet></DataSet>')
	SET @xLogPrima=''
			
	SET @xTemp=
		(SELECT * FROM 
			(SELECT * FROM #tmpEntita
			) AS [Table]
		FOR XML AUTO, ELEMENTS)

	SET @xLogDopo.modify('insert sql:variable("@xTemp") as first into (/DataSet)[1]')
	
				
	SET @xParLog=CONVERT(XML,'<Parametri><LogDopo/></Parametri>')			
	SET @xParLog.modify('insert sql:variable("@xLogDopo") as first into (/Parametri/LogDopo)[1]')
	SET @xParLog.modify('insert sql:variable("@xTimeStampBase") as last into (/Parametri)[1]')
	
	EXEC MSP_InsMovLog @xParLog

			
	SET @sCodEntita='APP'
	
	TRUNCATE TABLE #tmpEntita			
	INSERT INTO #tmpEntita(DataElaborazione,CodEntita,IDEntita,IDEpisodioOrigine,IDTrasfertimentoOrigine,IDEpisodioDestinazione,IDTrasfertimentoDestinazione,CodEntitaRiferimento,IDEntitaRiferimento)
	SELECT
		GETDATE() AS DataElaborazione,
		@sCodEntita AS CodEntita,
		ID AS IDEntita,
		IDEpisodio,
		IDTrasferimento,
		@uIDEpisodioDestinazione AS IDEpisodioDestinazione,
		@uIDTrasferimentoDestinazione AS IDTrasfertimentoDestinazione,
		NULL AS CodEntitaRiferimento,
		NULL AS IDEntitaRiferimento
	FROM T_MovAppuntamenti
	WHERE IDEpisodio= @uIDEpisodioOrigine

	UPDATE T_MovAppuntamenti
	SET IDEpisodio=@uIDEpisodioDestinazione,
		IDTrasferimento=@uIDTrasferimentoDestinazione
	WHERE ID IN (SELECT IDEntita FROM #tmpEntita WHERE CodEntita=@sCodEntita)


	INSERT INTO #tmpEntita(DataElaborazione,CodEntita,IDEntita,IDEpisodioOrigine,IDTrasfertimentoOrigine,IDEpisodioDestinazione,IDTrasfertimentoDestinazione,CodEntitaRiferimento,IDEntitaRiferimento)
	SELECT
		GETDATE() AS DataElaborazione,
		'SCH' AS CodEntita,
		ID AS IDEntita,
		IDEpisodio,
		IDTrasferimento,
		@uIDEpisodioDestinazione AS IDEpisodioDestinazione,
		@uIDTrasferimentoDestinazione AS IDTrasfertimentoDestinazione,
		CodEntita AS CodEntitaRiferimento,
		IDEntita AS IDEntitaRiferimento
	FROM T_MovSchede
	WHERE 		
		CodEntita=@sCodEntita AND
		IDEntita IN (SELECT IDEntita FROM #tmpEntita WHERE CodEntita=@sCodEntita)

	UPDATE T_MovSchede
	SET IDEpisodio=@uIDEpisodioDestinazione,
		IDTrasferimento=@uIDTrasferimentoDestinazione
	WHERE
		 CodEntita=@sCodEntita AND
		 ID IN (SELECT IDEntita FROM #tmpEntita WHERE CodEntita='SCH' AND CodEntitaRiferimento=@sCodEntita)

		SET @xLogDopo=Convert(XML,'<DataSet></DataSet>')
	SET @xLogPrima=''
			
	SET @xTemp=
		(SELECT * FROM 
			(SELECT * FROM #tmpEntita
			) AS [Table]
		FOR XML AUTO, ELEMENTS)

	SET @xLogDopo.modify('insert sql:variable("@xTemp") as first into (/DataSet)[1]')
	
				
	SET @xParLog=CONVERT(XML,'<Parametri><LogDopo/></Parametri>')			
	SET @xParLog.modify('insert sql:variable("@xLogDopo") as first into (/Parametri/LogDopo)[1]')
	SET @xParLog.modify('insert sql:variable("@xTimeStampBase") as last into (/Parametri)[1]')
	
	EXEC MSP_InsMovLog @xParLog

			
			
			
	SET @sCodEntita='DCL'
	
	TRUNCATE TABLE #tmpEntita			
	INSERT INTO #tmpEntita(DataElaborazione,CodEntita,IDEntita,IDEpisodioOrigine,IDTrasfertimentoOrigine,IDEpisodioDestinazione,IDTrasfertimentoDestinazione,CodEntitaRiferimento,IDEntitaRiferimento)
	SELECT
		GETDATE() AS DataElaborazione,
		@sCodEntita AS CodEntita,
		ID AS IDEntita,
		IDEpisodio,
		IDTrasferimento,
		@uIDEpisodioDestinazione AS IDEpisodioDestinazione,
		@uIDTrasferimentoDestinazione AS IDTrasfertimentoDestinazione,
		NULL AS CodEntitaRiferimento,
		NULL AS IDEntitaRiferimento
	FROM T_MovDiarioClinico
	WHERE IDEpisodio= @uIDEpisodioOrigine

	UPDATE T_MovDiarioClinico
	SET IDEpisodio=@uIDEpisodioDestinazione,
		IDTrasferimento=@uIDTrasferimentoDestinazione
	WHERE ID IN (SELECT IDEntita FROM #tmpEntita WHERE CodEntita=@sCodEntita)


	INSERT INTO #tmpEntita(DataElaborazione,CodEntita,IDEntita,IDEpisodioOrigine,IDTrasfertimentoOrigine,IDEpisodioDestinazione,IDTrasfertimentoDestinazione,CodEntitaRiferimento,IDEntitaRiferimento)
	SELECT
		GETDATE() AS DataElaborazione,
		'SCH' AS CodEntita,
		ID AS IDEntita,
		IDEpisodio,
		IDTrasferimento,
		@uIDEpisodioDestinazione AS IDEpisodioDestinazione,
		@uIDTrasferimentoDestinazione AS IDTrasfertimentoDestinazione,
		CodEntita AS CodEntitaRiferimento,
		IDEntita AS IDEntitaRiferimento
	FROM T_MovSchede
	WHERE 		
		CodEntita=@sCodEntita AND
		IDEntita IN (SELECT IDEntita FROM #tmpEntita WHERE CodEntita=@sCodEntita)

	UPDATE T_MovSchede
	SET IDEpisodio=@uIDEpisodioDestinazione,
		IDTrasferimento=@uIDTrasferimentoDestinazione
	WHERE
		 CodEntita=@sCodEntita AND
		 ID IN (SELECT IDEntita FROM #tmpEntita WHERE CodEntita='SCH' AND CodEntitaRiferimento=@sCodEntita)

		SET @xLogDopo=Convert(XML,'<DataSet></DataSet>')
	SET @xLogPrima=''
			
	SET @xTemp=
		(SELECT * FROM 
			(SELECT * FROM #tmpEntita
			) AS [Table]
		FOR XML AUTO, ELEMENTS)

	SET @xLogDopo.modify('insert sql:variable("@xTemp") as first into (/DataSet)[1]')
	
				
	SET @xParLog=CONVERT(XML,'<Parametri><LogDopo/></Parametri>')			
	SET @xParLog.modify('insert sql:variable("@xLogDopo") as first into (/Parametri/LogDopo)[1]')
	SET @xParLog.modify('insert sql:variable("@xTimeStampBase") as last into (/Parametri)[1]')
	
	EXEC MSP_InsMovLog @xParLog

			
	SET @sCodEntita='EVC'
	
	TRUNCATE TABLE #tmpEntita			
	INSERT INTO #tmpEntita(DataElaborazione,CodEntita,IDEntita,IDEpisodioOrigine,IDTrasfertimentoOrigine,IDEpisodioDestinazione,IDTrasfertimentoDestinazione,CodEntitaRiferimento,IDEntitaRiferimento)
	SELECT
		GETDATE() AS DataElaborazione,
		@sCodEntita AS CodEntita,
		ID AS IDEntita,
		IDEpisodio,
		IDTrasferimento,
		@uIDEpisodioDestinazione AS IDEpisodioDestinazione,
		@uIDTrasferimentoDestinazione AS IDTrasfertimentoDestinazione,
		NULL AS CodEntitaRiferimento,
		NULL AS IDEntitaRiferimento
	FROM T_MovEvidenzaClinica
	WHERE IDEpisodio= @uIDEpisodioOrigine

	UPDATE T_MovEvidenzaClinica
	SET IDEpisodio=@uIDEpisodioDestinazione,
		IDTrasferimento=@uIDTrasferimentoDestinazione
	WHERE ID IN (SELECT IDEntita FROM #tmpEntita WHERE CodEntita=@sCodEntita)

	
		SET @xLogDopo=Convert(XML,'<DataSet></DataSet>')
	SET @xLogPrima=''
			
	SET @xTemp=
		(SELECT * FROM 
			(SELECT * FROM #tmpEntita
			) AS [Table]
		FOR XML AUTO, ELEMENTS)

	SET @xLogDopo.modify('insert sql:variable("@xTemp") as first into (/DataSet)[1]')
	
				
	SET @xParLog=CONVERT(XML,'<Parametri><LogDopo/></Parametri>')			
	SET @xParLog.modify('insert sql:variable("@xLogDopo") as first into (/Parametri/LogDopo)[1]')
	SET @xParLog.modify('insert sql:variable("@xTimeStampBase") as last into (/Parametri)[1]')
	
	EXEC MSP_InsMovLog @xParLog

			
	SET @sCodEntita='NTG'

	
	TRUNCATE TABLE #tmpEntita			
	INSERT INTO #tmpEntita(DataElaborazione,CodEntita,IDEntita,IDEpisodioOrigine,IDTrasfertimentoOrigine,IDEpisodioDestinazione,IDTrasfertimentoDestinazione,CodEntitaRiferimento,IDEntitaRiferimento)
	SELECT
		GETDATE() AS DataElaborazione,
		@sCodEntita AS CodEntita,
		ID AS IDEntita,
		IDEpisodio,
		IDTrasferimento,
		@uIDEpisodioDestinazione AS IDEpisodioDestinazione,
		@uIDTrasferimentoDestinazione AS IDTrasfertimentoDestinazione,
		NULL AS CodEntitaRiferimento,
		NULL AS IDEntitaRiferimento
	FROM T_MovNote
	WHERE 
		IDEpisodio= @uIDEpisodioOrigine AND
		CodEntita= @sCodEntita								

	UPDATE T_MovNote
	SET IDEpisodio=@uIDEpisodioDestinazione,
		IDTrasferimento=@uIDTrasferimentoDestinazione
	WHERE ID IN (SELECT IDEntita FROM #tmpEntita WHERE CodEntita=@sCodEntita)

	
			
				
	SET @sCodEntita='PVT'
	
	TRUNCATE TABLE #tmpEntita			
	INSERT INTO #tmpEntita(DataElaborazione,CodEntita,IDEntita,IDEpisodioOrigine,IDTrasfertimentoOrigine,IDEpisodioDestinazione,IDTrasfertimentoDestinazione,CodEntitaRiferimento,IDEntitaRiferimento)
	SELECT
		GETDATE() AS DataElaborazione,
		@sCodEntita AS CodEntita,
		ID AS IDEntita,
		IDEpisodio,
		IDTrasferimento,
		@uIDEpisodioDestinazione AS IDEpisodioDestinazione,
		@uIDTrasferimentoDestinazione AS IDTrasfertimentoDestinazione,
		NULL AS CodEntitaRiferimento,
		NULL AS IDEntitaRiferimento
	FROM T_MovParametriVitali
	WHERE IDEpisodio= @uIDEpisodioOrigine

	UPDATE T_MovParametriVitali
	SET IDEpisodio=@uIDEpisodioDestinazione,
		IDTrasferimento=@uIDTrasferimentoDestinazione
	WHERE ID IN (SELECT IDEntita FROM #tmpEntita WHERE CodEntita=@sCodEntita)

	INSERT INTO #tmpEntita(DataElaborazione,CodEntita,IDEntita,IDEpisodioOrigine,IDTrasfertimentoOrigine,IDEpisodioDestinazione,IDTrasfertimentoDestinazione,CodEntitaRiferimento,IDEntitaRiferimento)
	SELECT
		GETDATE() AS DataElaborazione,
		'SCH' AS CodEntita,
		ID AS IDEntita,
		IDEpisodio,
		IDTrasferimento,
		@uIDEpisodioDestinazione AS IDEpisodioDestinazione,
		@uIDTrasferimentoDestinazione AS IDTrasfertimentoDestinazione,
		CodEntita AS CodEntitaRiferimento,
		IDEntita AS IDEntitaRiferimento
	FROM T_MovSchede
	WHERE 		
		CodEntita=@sCodEntita AND
		IDEntita IN (SELECT IDEntita FROM #tmpEntita WHERE CodEntita=@sCodEntita)

	UPDATE T_MovSchede
	SET IDEpisodio=@uIDEpisodioDestinazione,
		IDTrasferimento=@uIDTrasferimentoDestinazione
	WHERE
		 CodEntita=@sCodEntita AND
		 ID IN (SELECT IDEntita FROM #tmpEntita WHERE CodEntita='SCH' AND CodEntitaRiferimento=@sCodEntita)
	
		SET @xLogDopo=Convert(XML,'<DataSet></DataSet>')
	SET @xLogPrima=''
			
	SET @xTemp=
		(SELECT * FROM 
			(SELECT * FROM #tmpEntita
			) AS [Table]
		FOR XML AUTO, ELEMENTS)

	SET @xLogDopo.modify('insert sql:variable("@xTemp") as first into (/DataSet)[1]')
	
				
	SET @xParLog=CONVERT(XML,'<Parametri><LogDopo/></Parametri>')			
	SET @xParLog.modify('insert sql:variable("@xLogDopo") as first into (/Parametri/LogDopo)[1]')
	SET @xParLog.modify('insert sql:variable("@xTimeStampBase") as last into (/Parametri)[1]')
	
	EXEC MSP_InsMovLog @xParLog

			
	SET @sCodEntita='PRF'
	
	TRUNCATE TABLE #tmpEntita			
	INSERT INTO #tmpEntita(DataElaborazione,CodEntita,IDEntita,IDEpisodioOrigine,IDTrasfertimentoOrigine,IDEpisodioDestinazione,IDTrasfertimentoDestinazione,CodEntitaRiferimento,IDEntitaRiferimento)
	SELECT
		GETDATE() AS DataElaborazione,
		@sCodEntita AS CodEntita,
		ID AS IDEntita,
		IDEpisodio,
		IDTrasferimento,
		@uIDEpisodioDestinazione AS IDEpisodioDestinazione,
		@uIDTrasferimentoDestinazione AS IDTrasfertimentoDestinazione,
		NULL AS CodEntitaRiferimento,
		NULL AS IDEntitaRiferimento
	FROM T_MovPrescrizioni
	WHERE IDEpisodio= @uIDEpisodioOrigine

	UPDATE T_MovPrescrizioni
	SET IDEpisodio=@uIDEpisodioDestinazione,
		IDTrasferimento=@uIDTrasferimentoDestinazione
	WHERE ID IN (SELECT IDEntita FROM #tmpEntita WHERE CodEntita=@sCodEntita)	

	INSERT INTO #tmpEntita(DataElaborazione,CodEntita,IDEntita,IDEpisodioOrigine,IDTrasfertimentoOrigine,IDEpisodioDestinazione,IDTrasfertimentoDestinazione,CodEntitaRiferimento,IDEntitaRiferimento)
	SELECT
		GETDATE() AS DataElaborazione,
		'SCH' AS CodEntita,
		ID AS IDEntita,
		IDEpisodio,
		IDTrasferimento,
		@uIDEpisodioDestinazione AS IDEpisodioDestinazione,
		@uIDTrasferimentoDestinazione AS IDTrasfertimentoDestinazione,
		CodEntita AS CodEntitaRiferimento,
		IDEntita AS IDEntitaRiferimento
	FROM T_MovSchede
	WHERE 		
		CodEntita=@sCodEntita AND
		IDEntita IN (SELECT IDEntita FROM #tmpEntita WHERE CodEntita=@sCodEntita)

	UPDATE T_MovSchede
	SET IDEpisodio=@uIDEpisodioDestinazione,
		IDTrasferimento=@uIDTrasferimentoDestinazione
	WHERE
		 CodEntita=@sCodEntita AND
		 ID IN (SELECT IDEntita FROM #tmpEntita WHERE CodEntita='SCH' AND CodEntitaRiferimento=@sCodEntita)

		SET @xLogDopo=Convert(XML,'<DataSet></DataSet>')
	SET @xLogPrima=''
			
	SET @xTemp=
		(SELECT * FROM 
			(SELECT * FROM #tmpEntita
			) AS [Table]
		FOR XML AUTO, ELEMENTS)

	SET @xLogDopo.modify('insert sql:variable("@xTemp") as first into (/DataSet)[1]')
	
				
	SET @xParLog=CONVERT(XML,'<Parametri><LogDopo/></Parametri>')			
	SET @xParLog.modify('insert sql:variable("@xLogDopo") as first into (/Parametri/LogDopo)[1]')
	SET @xParLog.modify('insert sql:variable("@xTimeStampBase") as last into (/Parametri)[1]')
	
	EXEC MSP_InsMovLog @xParLog

			
	SET @sCodEntita='EPI'
	
	TRUNCATE TABLE #tmpEntita			
	INSERT INTO #tmpEntita(DataElaborazione,CodEntita,IDEntita,IDEpisodioOrigine,IDTrasfertimentoOrigine,IDEpisodioDestinazione,IDTrasfertimentoDestinazione,CodEntitaRiferimento,IDEntitaRiferimento)
	SELECT
		GETDATE() AS DataElaborazione,
		'SCH' AS CodEntita,
		ID AS IDEntita,
		IDEpisodio,
		IDTrasferimento,
		@uIDEpisodioDestinazione AS IDEpisodioDestinazione,
		@uIDTrasferimentoDestinazione AS IDTrasfertimentoDestinazione,
		'EPI' AS CodEntitaRiferimento,								IDEntita AS IDEntitaRiferimento
	FROM T_MovSchede
	WHERE 		
		CodEntita=@sCodEntita AND
		IDEntita = @uIDEpisodioOrigine						
	UPDATE T_MovSchede
	SET 
		IDEntita=@uIDEpisodioDestinazione,
		IDEpisodio=@uIDEpisodioDestinazione,
		IDTrasferimento=@uIDTrasferimentoDestinazione
	WHERE
		 CodEntita=@sCodEntita AND
		 ID IN (SELECT IDEntita FROM #tmpEntita WHERE CodEntita='SCH' AND CodEntitaRiferimento=@sCodEntita)

	 	SET @xLogDopo=Convert(XML,'<DataSet></DataSet>')
	SET @xLogPrima=''
			
	SET @xTemp=
		(SELECT * FROM 
			(SELECT * FROM #tmpEntita
			) AS [Table]
		FOR XML AUTO, ELEMENTS)

	SET @xLogDopo.modify('insert sql:variable("@xTemp") as first into (/DataSet)[1]')
	
				
	SET @xParLog=CONVERT(XML,'<Parametri><LogDopo/></Parametri>')			
	SET @xParLog.modify('insert sql:variable("@xLogDopo") as first into (/Parametri/LogDopo)[1]')
	SET @xParLog.modify('insert sql:variable("@xTimeStampBase") as last into (/Parametri)[1]')
	
	EXEC MSP_InsMovLog @xParLog

			
	SET @sCodEntita='WKI'
	
	TRUNCATE TABLE #tmpEntita			
	INSERT INTO #tmpEntita(DataElaborazione,CodEntita,IDEntita,IDEpisodioOrigine,IDTrasfertimentoOrigine,IDEpisodioDestinazione,IDTrasfertimentoDestinazione,CodEntitaRiferimento,IDEntitaRiferimento)
	SELECT
		GETDATE() AS DataElaborazione,
		@sCodEntita AS CodEntita,
		ID AS IDEntita,
		IDEpisodio,
		IDTrasferimento,
		@uIDEpisodioDestinazione AS IDEpisodioDestinazione,
		@uIDTrasferimentoDestinazione AS IDTrasfertimentoDestinazione,
		NULL AS CodEntitaRiferimento,
		NULL AS IDEntitaRiferimento
	FROM T_MovTaskInfermieristici
	WHERE IDEpisodio= @uIDEpisodioOrigine

	UPDATE T_MovTaskInfermieristici
	SET IDEpisodio=@uIDEpisodioDestinazione,
		IDTrasferimento=@uIDTrasferimentoDestinazione
	WHERE ID IN (SELECT IDEntita FROM #tmpEntita WHERE CodEntita=@sCodEntita)


	INSERT INTO #tmpEntita(DataElaborazione,CodEntita,IDEntita,IDEpisodioOrigine,IDTrasfertimentoOrigine,IDEpisodioDestinazione,IDTrasfertimentoDestinazione,CodEntitaRiferimento,IDEntitaRiferimento)
	SELECT
		GETDATE() AS DataElaborazione,
		'SCH' AS CodEntita,
		ID AS IDEntita,
		IDEpisodio,
		IDTrasferimento,
		@uIDEpisodioDestinazione AS IDEpisodioDestinazione,
		@uIDTrasferimentoDestinazione AS IDTrasfertimentoDestinazione,
		CodEntita AS CodEntitaRiferimento,
		IDEntita AS IDEntitaRiferimento
	FROM T_MovSchede
	WHERE 		
		CodEntita=@sCodEntita AND
		IDEntita IN (SELECT IDEntita FROM #tmpEntita WHERE CodEntita=@sCodEntita)

	UPDATE T_MovSchede
	SET IDEpisodio=@uIDEpisodioDestinazione,
		IDTrasferimento=@uIDTrasferimentoDestinazione
	WHERE
		 CodEntita=@sCodEntita AND
		 ID IN (SELECT IDEntita FROM #tmpEntita WHERE CodEntita='SCH' AND CodEntitaRiferimento=@sCodEntita)

		SET @xLogDopo=Convert(XML,'<DataSet></DataSet>')
	SET @xLogPrima=''
			
	SET @xTemp=
		(SELECT * FROM 
			(SELECT * FROM #tmpEntita
			) AS [Table]
		FOR XML AUTO, ELEMENTS)

	SET @xLogDopo.modify('insert sql:variable("@xTemp") as first into (/DataSet)[1]')
	
				
	SET @xParLog=CONVERT(XML,'<Parametri><LogDopo/></Parametri>')			
	SET @xParLog.modify('insert sql:variable("@xLogDopo") as first into (/Parametri/LogDopo)[1]')
	SET @xParLog.modify('insert sql:variable("@xTimeStampBase") as last into (/Parametri)[1]')
	
	EXEC MSP_InsMovLog @xParLog

		
	DROP TABLE #tmpEntita
	RETURN 0

END