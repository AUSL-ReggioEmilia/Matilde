CREATE PROCEDURE [dbo].[MSP_SpostaEpisodioSchedaEvidenzaClinica](@xParametri XML)
AS
BEGIN
		
	
	
		DECLARE @uIDEpisodio AS UNIQUEIDENTIFIER	
	DECLARE @sIDRefertoDWH AS VARCHAR(50)
	
		DECLARE @sGUID AS VARCHAR(50)
	DECLARE @nIDNumRelazioniEntita AS INTEGER
	DECLARE @uIDScheda AS UNIQUEIDENTIFIER
	
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
					
										SELECT @nIDNumRelazioniEntita=R.IDNum,
						   @uIDScheda=R.IDEntitaCollegata													
					FROM 
						T_MovRelazioniEntita R
							INNER JOIN T_MovSchede S
								ON R.IDEntitaCollegata=S.ID
					WHERE 
						R.CodEntita = 'DWH' AND 
						R.CodEntitaCollegata='SCH' AND
						S.CodStatoScheda <> 'CA' AND
						S.IDEpisodio <> @uIDEpisodio AND
						R.IDEntita=@sIDRefertoDWH					  
				END				
			ELSE
				BEGIN
										SELECT @nIDNumRelazioniEntita=R.IDNum,
						   @uIDScheda=R.IDEntitaCollegata													
					FROM 
						T_MovRelazioniEntita R
							INNER JOIN T_MovSchede S
								ON R.IDEntitaCollegata=S.ID
					WHERE 
						R.CodEntita = 'DWH' AND 
						R.CodEntitaCollegata='SCH' AND
						S.CodStatoScheda <> 'CA' AND
						R.IDEntita=@sIDRefertoDWH					  
				END
			
												
		    IF @nIDNumRelazioniEntita IS NOT NULL AND @uIDScheda IS NOT NULL
				BEGIN
					
												
									 		
										SET @xPar=CONVERT(XML,
									'<Parametri>' + CONVERT(varchar(max),@xTimeStamp) +
									'</Parametri>')
				
										SET @xPar.modify('delete (/Parametri/TimeStamp/CodEntita)[1]')								SET @xPar.modify('delete (/Parametri/TimeStamp/CodAzione)[1]')								SET @xPar.modify('delete (/Parametri/TimeStamp/IDEntita)[1]')											
					SET @xPar.modify('insert <CodEntita>SCH</CodEntita> as last into (/Parametri/TimeStamp)[1]')
					SET @xPar.modify('insert <CodAzione>CAN</CodAzione> as last into (/Parametri/TimeStamp)[1]')
					
					SET @xTemp=CONVERT(XML,'<IDEntita>' + CONVERT(varchar(50),@uIDScheda) + '</IDEntita>')
					SET @xPar.modify('insert sql:variable("@xTemp") as last into (/Parametri/TimeStamp)[1]')				
														
										SET @xTemp=CONVERT(XML,'<IDScheda>' + CONVERT(varchar(50),@uIDScheda) + '</IDScheda>')
					SET @xPar.modify('insert sql:variable("@xTemp") as first into (/Parametri)[1]')
					
										SET @xPar.modify('insert <CodStatoScheda>CA</CodStatoScheda> as last into (/Parametri)[1]')
														
															
						
															EXEC MSP_AggMovSchede @xPar
					
										
					DELETE FROM T_MovRelazioniEntita
					WHERE IDNum=@nIDNumRelazioniEntita
										
				END			
		END
	ELSE
		BEGIN
						SET @sRisultato='Parametri errati'			
		END	
	
	PRINT @sRisultato 	
	RETURN 0
END