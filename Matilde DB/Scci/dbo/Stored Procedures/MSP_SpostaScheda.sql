CREATE PROCEDURE [dbo].[MSP_SpostaScheda](@xParametri XML)
AS
		
BEGIN

		DECLARE @uIDSchedaOrigine AS UNIQUEIDENTIFIER
	DECLARE @sCodEntitaDestinazione AS VARCHAR(20)
	DECLARE @uIDEntitaDestinazione AS UNIQUEIDENTIFIER
	DECLARE @uIDSchedaDestinazione AS UNIQUEIDENTIFIER	
	
	DECLARE @uIDTrasferimento AS UNIQUEIDENTIFIER
	DECLARE @sCodLogin AS  VARCHAR(100)
	DECLARE @sCodRuolo AS VARCHAR(20)
	DECLARE @sNomePC AS VARCHAR(100)
	DECLARE @sIndirizzoIP AS VARCHAR(50)
		
	
		DECLARE @sCodEntitaOrigine AS VARCHAR(20)
	DECLARE @sCodSchedaOrigine AS VARCHAR(20)
	DECLARE @uIDEntitaOrigine AS UNIQUEIDENTIFIER
	DECLARE @uIDSchedaPadreOrigine AS UNIQUEIDENTIFIER
	DECLARE @nNumSchedePadriOrigine AS INTEGER
	DECLARE @nNumMovSchedeFiglioOrigine AS INTEGER
	DECLARE @nNumeroSchedaDiOrigine AS INTEGER
	
	DECLARE @uIDPazienteDestinazione AS UNIQUEIDENTIFIER
	DECLARE @uIDEpisodioDestinazione AS UNIQUEIDENTIFIER	
	DECLARE @sIDEpisodioDestinazione AS VARCHAR(50)
	DECLARE @uIDTrasferimentoDestinazione AS UNIQUEIDENTIFIER
	DECLARE @nConta AS INTEGER

	DECLARE @nNumeroSchedaDestinazione AS INTEGER
	
	DECLARE @bEsito AS BIT	
	DECLARE @sMessaggio AS VARCHAR(2000)
	DECLARE @sTmp AS VARCHAR(MAX)		
	DECLARE @sGUID AS VARCHAR(50)
	DECLARE @uIDSchedaFiglio AS UNIQUEIDENTIFIER
	
	DECLARE @xParametriStored AS XML
	DECLARE @xTmp AS XML
	
				
		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDSchedaOrigine.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDSchedaOrigine') as ValoreParametro(IDSchedaOrigine))
	IF 	ISNULL(@sGUID,'') <> '' SET @uIDSchedaOrigine=CONVERT(UNIQUEIDENTIFIER,@sGUID)		
	
		SET @sCodEntitaDestinazione=(SELECT TOP 1 ValoreParametro.CodEntitaDestinazione.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodEntitaDestinazione') as ValoreParametro(CodEntitaDestinazione))
	
		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDEntitaDestinazione.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDEntitaDestinazione') as ValoreParametro(IDEntitaDestinazione))
	IF 	ISNULL(@sGUID,'') <> '' SET @uIDEntitaDestinazione=CONVERT(UNIQUEIDENTIFIER,@sGUID)	
	
		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDSchedaDestinazione.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDSchedaDestinazione') as ValoreParametro(IDSchedaDestinazione))
	IF 	ISNULL(@sGUID,'') <> '' SET @uIDSchedaDestinazione=CONVERT(UNIQUEIDENTIFIER,@sGUID)	
	ELSE
		SET @uIDSchedaDestinazione=NULL
	
		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDTrasferimento.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/TimeStamp/IDTrasferimento') as ValoreParametro(IDTrasferimento))
	IF 	ISNULL(@sGUID,'') <> '' 
		SET @uIDTrasferimento=CONVERT(UNIQUEIDENTIFIER,@sGUID)	
	ELSE
		SET @uIDTrasferimento=NULL
			

			SET @sCodLogin=(SELECT TOP 1 ValoreParametro.CodLogin.value('.','VARCHAR(100)')
					  FROM @xParametri.nodes('/Parametri/TimeStamp/CodLogin') as ValoreParametro(CodLogin))
					  
		SET @sCodRuolo=(SELECT TOP 1 ValoreParametro.CodRuolo.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/TimeStamp/CodRuolo') as ValoreParametro(CodRuolo))
	
		SET @sNomePC=(SELECT TOP 1 ValoreParametro.NomePC.value('.','VARCHAR(100)')
					  FROM @xParametri.nodes('/Parametri/TimeStamp/NomePC') as ValoreParametro(NomePC))
	
		SET @sIndirizzoIP=(SELECT TOP 1 ValoreParametro.IndirizzoIP.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/TimeStamp/IndirizzoIP') as ValoreParametro(IndirizzoIP))
	

	
				
	SET @bEsito=1
	SET @sMessaggio=''
	
					SELECT 
		@sCodEntitaOrigine=CodEntita, 
		@uIDEntitaOrigine=IDEntita,
		@sCodSchedaOrigine=CodScheda,
		@uIDSchedaPadreOrigine=IDSchedaPadre,
		@uIDPazienteDestinazione=IDPaziente,
		@nNumeroSchedaDiOrigine=Numero
	FROM T_MovSchede 
	WHERE ID=@uIDSchedaOrigine
	
	
		CREATE TABLE #tmpMovSchedeFiglio
	(
		IDScheda UNIQUEIDENTIFIER,		
	)

	INSERT INTO #tmpMovSchedeFiglio(IDScheda)
	SELECT ID FROM T_MovSchede
	WHERE IDSchedaPadre=@uIDSchedaOrigine AND Storicizzata=0
	
	SET @nNumMovSchedeFiglioOrigine=(SELECT COUNT(*) FROM #tmpMovSchedeFiglio)
	
	IF @sCodEntitaDestinazione='PAZ'
	BEGIN	
		SET @uIDEpisodioDestinazione=NULL		
	END
	
		IF @sCodEntitaDestinazione='EPI'
	BEGIN		
		SET @uIDEpisodioDestinazione=@uIDEntitaDestinazione		
		SET @sIDEpisodioDestinazione =CONVERT(VARCHAR(50),@uIDEpisodioDestinazione)
	END
	
	IF @sCodEntitaDestinazione='PAZ'
	BEGIN
		SET @uIDEpisodioDestinazione=NULL
		SET @sIDEpisodioDestinazione =''
	END

		SET @nNumeroSchedaDestinazione=0

	IF @uIDSchedaDestinazione IS NOT NULL
	BEGIN
		
		 		 
		
		DECLARE @sIDSchedaPadreOrigine AS VARCHAR(50)
		DECLARE @sIDSchedaDestinazione AS VARCHAR(50)

		SET @sIDSchedaPadreOrigine='A'
		SET @sIDSchedaDestinazione='B'

		IF @uIDSchedaPadreOrigine IS NOT NULL SET @sIDSchedaPadreOrigine=CONVERT(VARCHAR(50),@uIDSchedaPadreOrigine)
		IF @uIDSchedaPadreOrigine IS NOT NULL SET @sIDSchedaDestinazione=CONVERT(VARCHAR(50),@uIDSchedaDestinazione)

		IF @sIDSchedaPadreOrigine = @sIDSchedaDestinazione
		BEGIN
									
						SET @nNumeroSchedaDestinazione=@nNumeroSchedaDiOrigine						SET @nNumeroSchedaDestinazione = @nNumeroSchedaDestinazione -1			END
		ELSE
		BEGIN			
						SET @nNumeroSchedaDestinazione= dbo.MF_SelNumerositaSchedaParametro('MassimoNumero',@uIDSchedaOrigine,@uIDSchedaDestinazione,@sCodEntitaDestinazione,@uIDEntitaDestinazione,@sCodSchedaOrigine) 					END
				
	END
	ELSE
	BEGIN
		
				SET @nNumeroSchedaDestinazione = dbo.MF_SelNumerositaSchedaParametro('MassimoNumero',NULL,NULL,@sCodEntitaDestinazione,@uIDEntitaDestinazione,@sCodSchedaOrigine)
	END

				IF @sCodEntitaDestinazione ='EPI' AND @uIDEpisodioDestinazione IS NOT NULL
	BEGIN
		SET @nConta = ( SELECT COUNT(IDNum) 
						FROM T_MovTrasferimenti 
						WHERE 
							ID=@uIDTrasferimento AND
							IDEpisodio=@uIDEpisodioDestinazione AND
							CodStatoTrasferimento NOT IN ('CA')
					  )
		IF @nConta > 0
			SET @uIDTrasferimentoDestinazione=@uIDTrasferimento
		ELSE
			SET @uIDTrasferimentoDestinazione=NULL							
	END
	
	SET  @nNumeroSchedaDestinazione=ISNULL(@nNumeroSchedaDestinazione,0)+1		
	
		SET @xParametriStored=Convert(XML,'<Parametri></Parametri>')
		
				SET @xTmp=CONVERT(XML,'<IDScheda>' + CONVERT(VARCHAR(50),@uIDSchedaOrigine) + '</IDScheda>')
		SET @xParametriStored.modify('insert sql:variable("@xTmp") as last into (/Parametri)[1]')
		
		SET @xTmp=CONVERT(XML,'<CodEntita>' + @sCodEntitaDestinazione + '</CodEntita>')
		SET @xParametriStored.modify('insert sql:variable("@xTmp") as last into (/Parametri)[1]')		
		
		SET @xTmp=CONVERT(XML,'<IDEntita>' + CONVERT(VARCHAR(50),@uIDEntitaDestinazione) + '</IDEntita>')
		SET @xParametriStored.modify('insert sql:variable("@xTmp") as last into (/Parametri)[1]')				
		
		SET @xTmp=CONVERT(XML,'<IDPaziente>' + CONVERT(VARCHAR(50),@uIDPazienteDestinazione) + '</IDPaziente>')
		SET @xParametriStored.modify('insert sql:variable("@xTmp") as last into (/Parametri)[1]')
		
		SET @xTmp=CONVERT(XML,'<IDEpisodio>' + @sIDEpisodioDestinazione + '</IDEpisodio>')
		SET @xParametriStored.modify('insert sql:variable("@xTmp") as last into (/Parametri)[1]')
		
		IF @uIDTrasferimentoDestinazione IS NOT NULL
			SET @xTmp=CONVERT(XML,'<IDTrasferimento>' +CONVERT(VARCHAR(50),@uIDTrasferimentoDestinazione) + '</IDTrasferimento>')
		ELSE
			SET @xTmp=CONVERT(XML,'<IDTrasferimento></IDTrasferimento>')		
		SET @xParametriStored.modify('insert sql:variable("@xTmp") as last into (/Parametri)[1]')			
		
		IF @uIDSchedaDestinazione IS NOT NULL
		BEGIN
						SET @xTmp=CONVERT(XML,'<IDSchedaPadre>' + CONVERT(VARCHAR(50),@uIDSchedaDestinazione) + '</IDSchedaPadre>')
			SET	@xParametriStored.modify('insert sql:variable("@xTmp") as last into (/Parametri)[1]')
		END			
		BEGIN
			SET @xTmp=CONVERT(XML,'<IDSchedaPadre></IDSchedaPadre>')
			SET	@xParametriStored.modify('insert sql:variable("@xTmp") as last into (/Parametri)[1]')			
		END	
		
								
		SET @xTmp=CONVERT(XML,'<Numero>' + CONVERT(VARCHAR(50),@nNumeroSchedaDestinazione) + '</Numero>')
		SET @xParametriStored.modify('insert sql:variable("@xTmp") as last into (/Parametri)[1]')
			
				SET @xTmp=CONVERT(XML,'<TimeStamp></TimeStamp>')
		SET @xParametriStored.modify('insert sql:variable("@xTmp") as last into (/Parametri)[1]')					

		SET @xTmp=CONVERT(XML,'<CodLogin>' + @sCodLogin + '</CodLogin>')
		SET @xParametriStored.modify('insert sql:variable("@xTmp") as last into (/Parametri/TimeStamp)[1]')
		
		SET @xTmp=CONVERT(XML,'<CodRuolo>' + @sCodRuolo + '</CodRuolo>')
		SET @xParametriStored.modify('insert sql:variable("@xTmp") as last into (/Parametri/TimeStamp)[1]')
		
		SET @xTmp=CONVERT(XML,'<NomePC>' + @sNomePC + '</NomePC>')
		SET @xParametriStored.modify('insert sql:variable("@xTmp") as last into (/Parametri/TimeStamp)[1]')
		
		SET @xTmp=CONVERT(XML,'<IndirizzoIP>' + @sIndirizzoIP + '</IndirizzoIP>')
		SET @xParametriStored.modify('insert sql:variable("@xTmp") as last into (/Parametri/TimeStamp)[1]')
		
		SET @xTmp=CONVERT(XML,'<CodEntita>' + @sCodEntitaDestinazione + '</CodEntita>')
		SET @xParametriStored.modify('insert sql:variable("@xTmp") as last into (/Parametri/TimeStamp)[1]')
		
		SET @xTmp=CONVERT(XML,'<IDEntita>' + CONVERT(VARCHAR(50),@uIDEntitaDestinazione) + '</IDEntita>')
		SET @xParametriStored.modify('insert sql:variable("@xTmp") as last into (/Parametri/TimeStamp)[1]')
		
		SET @xTmp=CONVERT(XML,'<IDPaziente>' + CONVERT(VARCHAR(50),@uIDPazienteDestinazione) + '</IDPaziente>')
		SET @xParametriStored.modify('insert sql:variable("@xTmp") as last into (/Parametri/TimeStamp)[1]')
		
		SET @xTmp=CONVERT(XML,'<IDEpisodio>' + @sIDEpisodioDestinazione + '</IDEpisodio>')
		SET @xParametriStored.modify('insert sql:variable("@xTmp") as last into (/Parametri/TimeStamp)[1]')
		
				SET @xTmp=CONVERT(XML,'<IDTrasferimento>' +CONVERT(VARCHAR(50),@uIDTrasferimento) + '</IDTrasferimento>')
		SET @xParametriStored.modify('insert sql:variable("@xTmp") as last into (/Parametri/TimeStamp)[1]')
		
		SET @xTmp=CONVERT(XML,'<CodAzione>MOD</CodAzione>')
		SET @xParametriStored.modify('insert sql:variable("@xTmp") as last into (/Parametri/TimeStamp)[1]')
						
											EXEC MSP_AggMovSchede @xParametriStored		
				
						UPDATE T_MovSchede
			SET CodEntita=@sCodEntitaDestinazione,
				IDEntita=@uIDEntitaDestinazione,
				Numero=@nNumeroSchedaDestinazione,
				IDEpisodio=@uIDEpisodioDestinazione,
								IDTrasferimento=@uIDTrasferimentoDestinazione, 
				IDPaziente=@uIDPazienteDestinazione,
				IDSchedaPadre=@uIDSchedaDestinazione
			WHERE 
				CodEntita=@sCodEntitaOrigine AND
				IDEntita=@uIDEntitaOrigine AND 
				CodScheda=@sCodSchedaOrigine AND
								Numero=@nNumeroSchedaDiOrigine AND 				
				Storicizzata=1
				
						IF @nNumMovSchedeFiglioOrigine>=0
			BEGIN	
				select * from #tmpMovSchedeFiglio
				DECLARE curFigli CURSOR LOCAL FOR 
				SELECT IDScheda
				FROM #tmpMovSchedeFiglio			

				OPEN curFigli

				FETCH NEXT FROM curFigli 
				INTO @uIDSchedaFiglio

				WHILE @@FETCH_STATUS = 0
				BEGIN
					
  
					SET @xParametriStored=Convert(XML,'<Parametri></Parametri>')		
					SET @xTmp=CONVERT(XML,'<IDSchedaOrigine>' + CONVERT(VARCHAR(50),@uIDSchedaFiglio) + '</IDSchedaOrigine>')
					SET @xParametriStored.modify('insert sql:variable("@xTmp") as last into (/Parametri)[1]')
					
					SET @xTmp=CONVERT(XML,'<CodEntitaDestinazione>' + @sCodEntitaDestinazione + '</CodEntitaDestinazione>')
					SET @xParametriStored.modify('insert sql:variable("@xTmp") as last into (/Parametri)[1]')		
		
					SET @xTmp=CONVERT(XML,'<IDEntitaDestinazione>' + CONVERT(VARCHAR(50),@uIDEntitaDestinazione) + '</IDEntitaDestinazione>')
					SET @xParametriStored.modify('insert sql:variable("@xTmp") as last into (/Parametri)[1]')				
		
					SET @xTmp=CONVERT(XML,'<IDSchedaDestinazione>' + CONVERT(VARCHAR(50),@uIDSchedaOrigine) + '</IDSchedaDestinazione>')
					SET @xParametriStored.modify('insert sql:variable("@xTmp") as last into (/Parametri)[1]')						
										  				
										SET @xTmp=CONVERT(XML,'<TimeStamp></TimeStamp>')
					SET @xParametriStored.modify('insert sql:variable("@xTmp") as last into (/Parametri)[1]')
									
					SET @xTmp=CONVERT(XML,'<CodLogin>' + @sCodLogin + '</CodLogin>')
					SET @xParametriStored.modify('insert sql:variable("@xTmp") as last into (/Parametri/TimeStamp)[1]')
					
					SET @xTmp=CONVERT(XML,'<CodRuolo>' + @sCodRuolo + '</CodRuolo>')
					SET @xParametriStored.modify('insert sql:variable("@xTmp") as last into (/Parametri/TimeStamp)[1]')
					
					SET @xTmp=CONVERT(XML,'<NomePC>' + @sNomePC + '</NomePC>')
					SET @xParametriStored.modify('insert sql:variable("@xTmp") as last into (/Parametri/TimeStamp)[1]')
					
					SET @xTmp=CONVERT(XML,'<IndirizzoIP>' + @sIndirizzoIP + '</IndirizzoIP>')
					SET @xParametriStored.modify('insert sql:variable("@xTmp") as last into (/Parametri/TimeStamp)[1]')
					
					SET @xTmp=CONVERT(XML,'<CodEntita></CodEntita>')
					SET @xParametriStored.modify('insert sql:variable("@xTmp") as last into (/Parametri/TimeStamp)[1]')
					
					SET @xTmp=CONVERT(XML,'<IDEntita></IDEntita>')
					SET @xParametriStored.modify('insert sql:variable("@xTmp") as last into (/Parametri/TimeStamp)[1]')
					
					SET @xTmp=CONVERT(XML,'<IDPaziente></IDPaziente>')
					SET @xParametriStored.modify('insert sql:variable("@xTmp") as last into (/Parametri/TimeStamp)[1]')
					
					SET @xTmp=CONVERT(XML,'<IDEpisodio></IDEpisodio>')
					SET @xParametriStored.modify('insert sql:variable("@xTmp") as last into (/Parametri/TimeStamp)[1]')
					
										SET @xTmp=CONVERT(XML,'<IDTrasferimento>' +CONVERT(VARCHAR(50),@uIDTrasferimento) + '</IDTrasferimento>')
					SET @xParametriStored.modify('insert sql:variable("@xTmp") as last into (/Parametri/TimeStamp)[1]')
					
					SET @xTmp=CONVERT(XML,'<CodAzione></CodAzione>')
					SET @xParametriStored.modify('insert sql:variable("@xTmp") as last into (/Parametri/TimeStamp)[1]')
		
					 SELECT @xParametriStored
					 EXEC MSP_SpostaScheda @xParametriStored			
			
					FETCH NEXT FROM curFigli 
					INTO @uIDSchedaFiglio
				END
			
			END
				
	

		DROP TABLE #tmpMovSchedeFiglio
	
		SELECT 
		@bEsito As Esito,
		@sMessaggio As Messaggio
END