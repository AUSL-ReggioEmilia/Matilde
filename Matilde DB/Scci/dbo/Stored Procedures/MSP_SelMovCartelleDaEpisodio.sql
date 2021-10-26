CREATE PROCEDURE [dbo].[MSP_SelMovCartelleDaEpisodio](@xParametri XML)
AS
BEGIN

	
	
				

	DECLARE @sCodRuolo AS VARCHAR(20)	
	DECLARE @uIDEpisodio AS UNIQUEIDENTIFIER	
	DECLARE @sCodStatoCartella AS VARCHAR(1800)
	
		DECLARE @sGUID AS VARCHAR(Max)
			
		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDEpisodio.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDEpisodio') as ValoreParametro(IDEpisodio))
	IF 	ISNULL(@sGUID,'') <> '' SET @uIDEpisodio=CONVERT(UNIQUEIDENTIFIER,	@sGUID)	
		
		SET @sCodStatoCartella= (SELECT TOP 1 ValoreParametro.CodStatoCartella.value('.','VARCHAR(20)')
							 FROM @xParametri.nodes('/Parametri/CodStatoCartella') as ValoreParametro(CodStatoCartella))
					  		  		
	    DECLARE @xTimeStamp AS XML	

    SET @xTimeStamp=(SELECT TOP 1 ValoreParametro.TS.query('.')
					  FROM @xParametri.nodes('/Parametri/TimeStamp') as ValoreParametro(TS))
	
	SET @xTimeStamp=CONVERT(XML,
						'<Parametri>' + CONVERT(varchar(max),@xTimeStamp) +
						'</Parametri>')
						
		SET @sCodRuolo=(SELECT TOP 1 ValoreParametro.CodRuolo.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/TimeStamp/CodRuolo') as ValoreParametro(CodRuolo))
	SET @sCodRuolo=ISNULL(@sCodRuolo,'')
															
					
	CREATE TABLE #tmpUARuolo
	(
		CodUA VARCHAR(20) COLLATE Latin1_General_CI_AS,
		Descrizione VARCHAR(255) COLLATE Latin1_General_CI_AS
	)

	DECLARE @xTmp AS XML
	SET  @xTmp=CONVERT(XML,'<Parametri><CodRuolo>'+ @sCodRuolo + '</CodRuolo></Parametri>')
	
	INSERT #tmpUARuolo EXEC MSP_SelUADaRuolo @xTmp
	
	CREATE INDEX IX_CodUA ON #tmpUARuolo (CodUA)    			
		
				
		SELECT 
		C2.ID AS IDCartella,
		C2.NumeroCartella, 
		C2.DataApertura,
		T3.ID AS IDTrasferimento
	FROM
		(		SELECT 
				QM.IDCartella,
				MAX(T2.IDNum) AS MaxIDNumTasf
		 FROM
								(SELECT 
					C.ID AS IDCartella,		
					MAX(T.DataIngresso) AS MaxDataIngresso
				FROM 
					T_MovCartelle C
						INNER JOIN T_MovTrasferimenti  T
							ON C.ID=T.IDCartella
						INNER JOIN #tmpUARuolo UA															ON T.CodUA =UA.CodUA
				WHERE 					
					C.CodStatoCartella =  @sCodStatoCartella AND									T.IDEpisodio = @uIDEpisodio AND													T.CodStatoTrasferimento <> 'CA'												GROUP BY 
					C.ID) AS QM
		
				INNER JOIN T_MovTrasferimenti  T2
							ON (QM.IDCartella=T2.IDCartella AND
								QM.MaxDataIngresso =T2.DataIngresso AND
								T2.IDEpisodio = @uIDEpisodio AND
								T2.CodStatoTrasferimento <> 'CA'	)
				INNER JOIN #tmpUARuolo UA2								
							ON T2.CodUA =UA2.CodUA
			GROUP BY QM.IDCartella
		) AS QT
		INNER JOIN T_MovCartelle C2
			ON (C2.ID = QT.IDCartella)
		INNER JOIN T_MovTrasferimenti  T3
			ON (QT.MaxIDNumTasf = T3.IDNum)

	ORDER BY C2.DataApertura ASC
									
				
		
				DROP TABLE #tmpUARuolo
	RETURN 0
END