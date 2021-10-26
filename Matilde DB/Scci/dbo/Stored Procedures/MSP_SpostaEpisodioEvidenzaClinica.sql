CREATE PROCEDURE [dbo].[MSP_SpostaEpisodioEvidenzaClinica](@xParametri XML)
AS
BEGIN
		
	
	
		DECLARE @uIDEpisodio AS UNIQUEIDENTIFIER	
	DECLARE @sIDRefertoDWH AS VARCHAR(50)
	
		DECLARE @sGUID AS VARCHAR(50)
	DECLARE @uIDEvidenzaClinica AS UNIQUEIDENTIFIER
	DECLARE @sNumeroNosologico AS VARCHAR(20)
	DECLARE @sNumeroListaAttesa AS VARCHAR(20)
				
	DECLARE @sTmp As VARCHAR(1800)
	DECLARE @xPar  AS XML
		
	DECLARE @xTimeStamp AS XML	
	DECLARE @xTemp  AS XML
	DECLARE @sRisultato AS VARCHAR(MAX)
	
	
		SET @sRisultato=''
	
	SET @uIDEpisodio=NULL
	SET @sIDRefertoDWH=NULL
		
				
		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDEpisodio.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDEpisodio') as ValoreParametro(IDEpisodio))
	IF 	ISNULL(@sGUID,'') <> '' SET @uIDEpisodio=CONVERT(UNIQUEIDENTIFIER,	@sGUID)	
		

		SET @sIDRefertoDWH=(SELECT TOP 1 ValoreParametro.IDRefertoDWH.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDRefertoDWH') as ValoreParametro(IDRefertoDWH))	
	
	
	    SET @xTimeStamp=(SELECT TOP 1 ValoreParametro.TS.query('.')
					  FROM @xParametri.nodes('/Parametri/TimeStamp') as ValoreParametro(TS))
		  
					
	IF @sIDRefertoDWH IS NOT NULL
		BEGIN						
			
			IF @uIDEpisodio IS NOT NULL
				BEGIN		
					
										SELECT 
						@sNumeroNosologico=ISNULL(NumeroNosologico,''),
						@sNumeroListaAttesa=ISNULL(NumeroListaAttesa,'')
					FROM T_MovEpisodi
					WHERE ID=@uIDEpisodio
					
					
										SET @uIDEvidenzaClinica=(SELECT TOP 1 
													R.ID
												  FROM 
													T_MovEvidenzaClinica R
														INNER JOIN T_MovEpisodi E
															ON R.IDEpisodio=E.ID
												  WHERE R.IDRefertoDWH = @sIDRefertoDWH AND
														R.IDEpisodio <> @uIDEpisodio AND
														R.CodStatoEvidenzaClinica <> 'CA'	AND
														R.CodStatoEvidenzaClinicaVisione <> 'VS' AND
														(ISNULL(E.NumeroNosologico,'') <> '' AND 
														 ISNULL(E.NumeroNosologico,'') <> @sNumeroNosologico)
										  )
				END				
			ELSE
				BEGIN
																																			SET @uIDEvidenzaClinica = NULL
				END
			
												
		    IF @uIDEvidenzaClinica IS NOT NULL
				BEGIN
											 		
										SET @xPar=CONVERT(XML,
									'<Parametri>' + CONVERT(varchar(max),@xTimeStamp) +
									'</Parametri>')
				
										SET @xPar.modify('delete (/Parametri/TimeStamp/CodEntita)[1]')								SET @xPar.modify('delete (/Parametri/TimeStamp/CodAzione)[1]')								SET @xPar.modify('delete (/Parametri/TimeStamp/IDEntita)[1]')											
					SET @xPar.modify('insert <CodEntita>EVC</CodEntita> as last into (/Parametri/TimeStamp)[1]')
					SET @xPar.modify('insert <CodAzione>CAN</CodAzione> as last into (/Parametri/TimeStamp)[1]')
					
					SET @xTemp=CONVERT(XML,'<IDEntita>' + CONVERT(varchar(50),@uIDEvidenzaClinica) + '</IDEntita>')
					SET @xPar.modify('insert sql:variable("@xTemp") as last into (/Parametri/TimeStamp)[1]')				
														
										SET @xTemp=CONVERT(XML,'<IDEvidenzaClinica>' + CONVERT(varchar(50),@uIDEvidenzaClinica) + '</IDEvidenzaClinica>')
					SET @xPar.modify('insert sql:variable("@xTemp") as first into (/Parametri)[1]')
					
										SET @xPar.modify('insert <CodStatoEvidenzaClinica>CA</CodStatoEvidenzaClinica> as last into (/Parametri)[1]')
														
															
						
															EXEC MSP_AggMovEvidenzaClinica @xPar
					
				END			
		END
	ELSE
		BEGIN
						SET @sRisultato='Parametri errati'			
		END	
	
	PRINT @sRisultato 	
	RETURN 0
END