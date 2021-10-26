CREATE PROCEDURE [dbo].[MSP_ControlloEvidenzaClinica](@xParametri XML)
AS
BEGIN
		
	
	
		DECLARE @uIDEpisodio AS UNIQUEIDENTIFIER
	DECLARE @sIDRefertoDWH AS VARCHAR(50)
	
		DECLARE @sGUID AS VARCHAR(50)
	DECLARE @uIDEvidenzaClinica AS UNIQUEIDENTIFIER
	
	
	DECLARE @nEsiste AS INTEGER
	
	DECLARE @sTmp As VARCHAR(1800)
	DECLARE @xPar  AS XML
	DECLARE @xParMovPaz  AS XML
	DECLARE @xTemp  AS XML
	DECLARE @sRisultato AS VARCHAR(MAX)
	
	
		SET @sRisultato=''
	
	SET @uIDEpisodio=NULL
	SET @sIDRefertoDWH=NULL
	
	SET @nEsiste=0
	
				
		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDEpisodio.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDEpisodio') as ValoreParametro(IDEpisodio))
	IF 	ISNULL(@sGUID,'') <> '' SET @uIDEpisodio=CONVERT(UNIQUEIDENTIFIER,	@sGUID)	
		

		SET @sIDRefertoDWH=(SELECT TOP 1 ValoreParametro.IDRefertoDWH.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDRefertoDWH') as ValoreParametro(IDRefertoDWH))	
	
					
	IF @uIDEpisodio IS NOT NULL AND @sIDRefertoDWH IS NOT NULL
		BEGIN										
												
						SET @xPar=@xParametri
			SET @xPar.modify('delete (/Parametri/TimeStamp/CodEntita)[1]')						SET @xPar.modify('delete (/Parametri/TimeStamp/CodAzione)[1]')						SET @xPar.modify('delete (/Parametri/TimeStamp/IDEntita)[1]')									
			SET @xPar.modify('insert <CodEntita>EVC</CodEntita> as last into (/Parametri/TimeStamp)[1]')
				
			SET @uIDEvidenzaClinica=(SELECT TOP 1 ID
						  FROM T_MovEvidenzaClinica
						  WHERE IDRefertoDWH=CONVERT(varchar(50),@sIDRefertoDWH) AND
								IDEpisodio=@uIDEpisodio AND
								CodStatoEvidenzaClinica NOT IN ('CA')
						  )
			IF @uIDEvidenzaClinica IS NOT NULL
				BEGIN
					SET @sRisultato='Aggiornato, IDEvidenzaClinica :' + Convert(VARCHAR(50),@uIDEvidenzaClinica)
					

															
										SET @xTemp=CONVERT(XML,'<IDEvidenzaClinica>' + CONVERT(varchar(50),@uIDEvidenzaClinica) + '</IDEvidenzaClinica>')
					SET @xPar.modify('insert sql:variable("@xTemp") as first into (/Parametri)[1]')
					
										SET @xPar.modify('insert <CodAzione>MOD</CodAzione> as last into (/Parametri/TimeStamp)[1]')
					
										SET @xTemp=CONVERT(XML,'<IDEntita>' + CONVERT(varchar(50),@uIDEvidenzaClinica) + '</IDEntita>')
					SET @xPar.modify('insert sql:variable("@xTemp") as last into (/Parametri/TimeStamp)[1]')
					
															EXEC MSP_AggMovEvidenzaClinica @xPar
					
				END	
			ELSE
				BEGIN
					SET @sRisultato='Non Trovato'
																									
										SET @xPar.modify('insert <CodAzione>INS</CodAzione> as last into (/Parametri/TimeStamp)[1]')																			
					
										SET @xTemp=CONVERT(XML,'<IDEntita></IDEntita>')
					SET @xPar.modify('insert sql:variable("@xTemp") as last into (/Parametri/TimeStamp)[1]')
					
										CREATE TABLE #tmpEvidenzaClinica
							(
								IDEvidenzaClinica UNIQUEIDENTIFIER
							)

										INSERT #tmpEvidenzaClinica EXEC MSP_InsMovEvidenzaClinica @xPar
				
					SET @uIDEvidenzaClinica=(SELECT TOP 1 IDEvidenzaClinica FROM #tmpEvidenzaClinica)
					
										SET @sRisultato='Inserito, IDEvidenzaClinica :' + Convert(VARCHAR(50),@uIDEvidenzaClinica)	
				END		 
									 
		END	
	ELSE
		BEGIN
						SET @sRisultato='Parametri errati'			
		END	
	
	SELECT @sRisultato AS Risultato			
	RETURN 0
END