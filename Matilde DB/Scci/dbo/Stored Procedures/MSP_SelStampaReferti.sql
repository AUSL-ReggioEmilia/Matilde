CREATE PROCEDURE [dbo].[MSP_SelStampaReferti](@xParametri XML)
AS
BEGIN
	
	
		DECLARE @uIDEpisodio AS UNIQUEIDENTIFIER
	DECLARE @uIDReferto AS UNIQUEIDENTIFIER
	DECLARE @bDatiEstesi AS BIT
	DECLARE @xTimeStamp AS XML	
	
	
		DECLARE @sGUID AS VARCHAR(Max)
	DECLARE @xPar AS XML
	
		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDEpisodio.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDEpisodio') as ValoreParametro(IDEpisodio))
	IF 	ISNULL(@sGUID,'') <> '' SET @uIDEpisodio=CONVERT(UNIQUEIDENTIFIER,@sGUID)	
	
		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDReferto.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDReferto') as ValoreParametro(IDReferto))
	IF 	ISNULL(@sGUID,'') <> '' SET @uIDReferto=CONVERT(UNIQUEIDENTIFIER,@sGUID)	
	
		SET @bDatiEstesi=(SELECT TOP 1 ValoreParametro.DatiEstesi.value('.','bit')
					  FROM @xParametri.nodes('/Parametri/DatiEstesi') as ValoreParametro(DatiEstesi))
	SET @bDatiEstesi=ISNULL(@bDatiEstesi,0)

	
				
    SET @xTimeStamp=(SELECT TOP 1 ValoreParametro.TS.query('.')
					  FROM @xParametri.nodes('/Parametri/TimeStamp') as ValoreParametro(TS))
		
					
				
				CREATE TABLE #tmpEpisodiCollegati
	(
		IDEpisodio UNIQUEIDENTIFIER
	)
	
		SET @xPar=CONVERT(XML,'<Parametri>
								<IDEpisodio>' + CONVERT(VARCHAR(50),@uIDEpisodio) + '</IDEpisodio>
							</Parametri>')
									
	INSERT INTO #tmpEpisodiCollegati					
	EXEC MSP_SelEpisodiCollegati @xPar	
		
	SELECT 
		ID As IDReferto,
		CASE 
			WHEN @bDatiEstesi=1 THEN PDFDWH
			ELSE NULL
		END AS 	PDFDWH
	FROM 
		T_MovEvidenzaClinica			
	WHERE
		CodStatoEvidenzaClinica IN ('CM') AND							CodStatoEvidenzaClinicaVisione IN ('VS','DV') AND				(IDEpisodio=@uIDEpisodio OR										 IDEpisodio IN (SELECT IDEpisodio FROM #tmpEpisodiCollegati) 		)
		AND
		ID = CASE																WHEN @uIDReferto IS NOT NULL THEN @uIDReferto
				ELSE ID
			 END					

							
		SET @xTimeStamp.modify('delete (/TimeStamp/IDEpisodio)[1]') 			
	SET @xTimeStamp.modify('insert <IDEpisodio>{sql:variable("@uIDEpisodio")}</IDEpisodio> as last into (/TimeStamp)[1]')
	
		SET @xTimeStamp.modify('delete (/TimeStamp/IDEntita)[1]') 			
	SET @xTimeStamp.modify('insert <IDEntita>{sql:variable("@uIDEpisodio")}</IDEntita> as last into (/TimeStamp)[1]')
	
		SET @xTimeStamp.modify('delete (/TimeStamp/CodAziente)[1]') 			
	SET @xTimeStamp.modify('insert <CodAzione>VIS</CodAzione> as first into (/TimeStamp)[1]')
	
	
	SET @xTimeStamp=CONVERT(XML,
						'<Parametri>' + CONVERT(varchar(max),@xTimeStamp) +
						'</Parametri>')	
						
		EXEC MSP_InsMovTimeStamp @xTimeStamp	

	DROP TABLE #tmpEpisodiCollegati	
	RETURN 0
END