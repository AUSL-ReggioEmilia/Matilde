CREATE PROCEDURE [dbo].[MSP_BO_SpostaGruppoPrescrizioni]
		(@xParametri XML, 
		 @bErrore BIT OUTPUT		
		 )  
AS
BEGIN

	
	
	
		DECLARE @sCodUAInizio AS VARCHAR(20)
	DECLARE @sNumeroCartellaInizio AS VARCHAR(50) 	
	DECLARE @sCodUAFine AS VARCHAR(20)
	DECLARE @sNumeroCartellaFine AS VARCHAR(50) 		
	
	
	DECLARE @uIDSessione AS UNIQUEIDENTIFIER
	DECLARE @xTimeStamp AS XML
		
		DECLARE @uIDCartellaInizio AS UNIQUEIDENTIFIER				
	DECLARE @uIDPazienteInizio AS UNIQUEIDENTIFIER							
	DECLARE @uIDEpisodioInizio AS UNIQUEIDENTIFIER	
	DECLARE @uIDTrasferimentoInizio AS UNIQUEIDENTIFIER

	DECLARE @uIDCartellaFine AS UNIQUEIDENTIFIER
	DECLARE @uIDPazienteFine AS UNIQUEIDENTIFIER	
	DECLARE @uIDEpisodioFine AS UNIQUEIDENTIFIER	
	DECLARE @uIDTrasferimentoFine AS UNIQUEIDENTIFIER	
	
	DECLARE @sCodRuolo AS VARCHAR(20)
			
	DECLARE @sGUID AS VARCHAR(50)
	DECLARE @nQTA AS INTEGER
	DECLARE @nRecord AS INTEGER		
	DECLARE @sInfoTimeStamp AS VARCHAR(255)	
	DECLARE @uIDPrescrizione AS UNIQUEIDENTIFIER	
	DECLARE @xTimeStampSingolo AS XML
	DECLARE @bErroreSingolo AS BIT	



		IF @xParametri.exist('/Parametri/CodUAInizio')=1
		BEGIN
			SET @sCodUAInizio=(SELECT	TOP 1 ValoreParametro.CodUAInizio.value('.','VARCHAR(20)')
						 FROM @xParametri.nodes('/Parametri/CodUAInizio') as ValoreParametro(CodUAInizio))	
		END
	
	IF @xParametri.exist('/Parametri/NumeroCartellaInizio')=1	
		BEGIN
			SET @sNumeroCartellaInizio=(SELECT	TOP 1 ValoreParametro.NumeroCartellaInizio.value('.','VARCHAR(50)')
						 FROM @xParametri.nodes('/Parametri/NumeroCartellaInizio') as ValoreParametro(NumeroCartellaInizio))	
		END
	

	IF @xParametri.exist('/Parametri/CodUAFine')=1	
		BEGIN	
			SET @sCodUAFine=(SELECT	TOP 1 ValoreParametro.CodUAFine.value('.','VARCHAR(20)')
						 FROM @xParametri.nodes('/Parametri/CodUAFine') as ValoreParametro(CodUAFine))	
		END
		
	IF @xParametri.exist('/Parametri/NumeroCartellaFine')=1	
		BEGIN	
		SET @sNumeroCartellaFine=(SELECT TOP 1 ValoreParametro.NumeroCartellaFine.value('.','VARCHAR(50)')
						 FROM @xParametri.nodes('/Parametri/NumeroCartellaFine') as ValoreParametro(NumeroCartellaFine))	
		END		
	
	IF @xParametri.exist('/Parametri/IDSessione')=1	
		BEGIN						 
			SET @sGUID=(SELECT	TOP 1 ValoreParametro.IDSessione.value('.','VARCHAR(50)')
						 FROM @xParametri.nodes('/Parametri/IDSessione') as ValoreParametro(IDSessione))
			IF ISNULL(@sGUID,'')<>'' SET @uIDSessione=CONVERT(UNIQUEIDENTIFIER,@sGUID)			 			 						 			
		END
			
		IF @xParametri.exist('/Parametri/TimeStamp')=1	
		BEGIN								  				  				  				  	
			SET @xTimeStamp=(SELECT TOP 1 ValoreParametro.TS.query('.')
							  FROM @xParametri.nodes('/Parametri/TimeStamp') as ValoreParametro(TS))	
		END					  						

		SET @bErrore=0
	SET @nRecord=0
	SET @bErroreSingolo=0
	
	
						
	DECLARE @tblErrori AS TABLE
		(
		Errore VARCHAR(500) COLLATE Latin1_General_CI_AS
		)	
	
		SET @sCodRuolo=(SELECT TOP 1 Valore FROM T_Config WHERE ID=681)


				IF @sCodUAInizio IS NOT NULL AND @sNumeroCartellaInizio IS NOT NULL
	BEGIN

				SELECT @uIDCartellaInizio=IDCartella,
			  @nQta=COUNT(*)
		FROM T_MovTrasferimenti
		WHERE IDCartella 
			IN (SELECT ID FROM T_MovCartelle
				WHERE 
				NumeroCartella=@sNumeroCartellaInizio
				)
			AND T_MovTrasferimenti.CodUA=@sCodUAInizio
		GROUP BY IDCartella
		
		IF @nQta =0 
			BEGIN			
				INSERT INTO @tblErrori
						VALUES('(GDPRF001) ERRORE : nessuna cartella di origine trovata')
				SET @bErrore=1
			END
		ELSE
			BEGIN						
								SELECT TOP 1 
					@uIDEpisodioInizio=IDEpisodio
				FROM T_MovTrasferimenti 
				WHERE IDCartella=@uIDCartellaInizio
				
				SET @uIDPazienteInizio=(SELECT IDPaziente FROM T_MovPazienti WHERE IDEpisodio=@uIDEpisodioInizio)							
			END	
	END

				IF @sCodUAFine IS NOT NULL AND @sNumeroCartellaFine IS NOT NULL
	BEGIN

				SELECT @uIDCartellaFine=IDCartella,
			  @nQta=COUNT(*)
		FROM T_MovTrasferimenti
		WHERE IDCartella 
			IN (SELECT ID FROM T_MovCartelle
				WHERE 
				NumeroCartella=@sNumeroCartellaFine
				)
			AND T_MovTrasferimenti.CodUA=@sCodUAFine
		GROUP BY IDCartella
		
		IF @nQta =0 
			BEGIN			
				INSERT INTO @tblErrori
						VALUES('(GDPRF002) ERRORE : nessuna cartella di destinazione trovata')
				SET @bErrore=1
			END
		ELSE
			BEGIN						
								SELECT TOP 1 
					@uIDEpisodioFine=IDEpisodio
				FROM T_MovTrasferimenti WHERE IDCartella=@uIDCartellaFine
				
				SET @uIDPazienteFine=(SELECT IDPaziente FROM T_MovPazienti WHERE IDEpisodio=@uIDEpisodioFine)				
			END
	END

				IF @uIDPazienteInizio IS NULL 
	BEGIN
		SET @bErrore=1
		INSERT INTO @tblErrori
				VALUES ('(GDPRF003) IDPaziente NON specificato')
	END
	ELSE
	BEGIN								
				SET @nRecord=(SELECT COUNT(*) FROM T_MovPrescrizioni
					  WHERE 
							IDEpisodio=@uIDEpisodioInizio AND											IDTrasferimento																IN (SELECT ID FROM T_MovTrasferimenti
						 		WHERE 
									IDEpisodio=@uIDEpisodioInizio AND 
									IDCartella=@uIDCartellaInizio) 					
						)	
							
				IF @nRecord =0
		BEGIN
						SET @bErrore=1
			
			INSERT INTO @tblErrori
				VALUES ('(GDPRF004) ERRORE Nessuna prescrizione da aggiornare per la cartella {' + CONVERT(VARCHAR(50),@uIDCartellaInizio) +'}')
		END
		ELSE
		BEGIN	
			IF @uIDCartellaInizio=@uIDCartellaFine
			BEGIN
								SET @bErrore=1
					INSERT INTO @tblErrori
						VALUES('(GDPRF005) ERRORE Stai cercando di spostare prescrizioni sulla stessa cartella {' + CONVERT(VARCHAR(50),@uIDCartellaInizio) +'}')	
			END
		
			IF @bErrore=0
			BEGIN
				
								DECLARE cursore CURSOR LOCAL READ_ONLY  FOR 
				SELECT  ID
				FROM T_MovPrescrizioni
				WHERE 
						 	IDEpisodio=@uIDEpisodioInizio AND											IDTrasferimento																IN (SELECT ID FROM T_MovTrasferimenti
						 		WHERE 
									IDEpisodio=@uIDEpisodioInizio AND 
									IDCartella=@uIDCartellaInizio) 														
																					
				BEGIN TRANSACTION											
					
					OPEN cursore
					
					FETCH NEXT FROM cursore 
					INTO @uIDPrescrizione
					
					SET @bErrore=0
					WHILE (@@FETCH_STATUS = 0 AND @bErrore=0)
					BEGIN								
						SET @xTimeStampSingolo=@xTimeStamp
						
						
						
						
																								
						SET @xTimeStampSingolo.modify('delete (/CodUAInizio)[1]') 
						SET @xTimeStampSingolo.modify('insert <CodUAInizio>{sql:variable("@sCodUAInizio")}</CodUAInizio> into (/)[1]')
						
						SET @xTimeStampSingolo.modify('delete (/NumeroCartellaInizio)[1]') 			
						SET @xTimeStampSingolo.modify('insert <NumeroCartellaInizio>{sql:variable("@sNumeroCartellaInizio")}</NumeroCartellaInizio> into (/)[1]')

						SET @xTimeStampSingolo.modify('delete (/CodUAFine)[1]') 
						SET @xTimeStampSingolo.modify('insert <CodUAFine>{sql:variable("@sCodUAFine")}</CodUAFine> into (/)[1]')
						
						SET @xTimeStampSingolo.modify('delete (/NumeroCartellaFine)[1]') 			
						SET @xTimeStampSingolo.modify('insert <NumeroCartellaFine>{sql:variable("@sNumeroCartellaFine")}</NumeroCartellaFine> into (/)[1]')


						SET @xTimeStampSingolo.modify('delete (/IDEntitaInizio)[1]') 			
						SET @xTimeStampSingolo.modify('insert <IDEntitaInizio>{sql:variable("@uIDPrescrizione")}</IDEntitaInizio> into (/)[1]')


						SET @xTimeStampSingolo=CONVERT(XML,
										'<Parametri>' + CONVERT(varchar(max),@xTimeStampSingolo) +
										'</Parametri>')						
						   
												INSERT INTO @tblErrori 
							EXEC MSP_BO_SpostaPrescrizioni @xTimeStampSingolo, @bErroreSingolo  OUTPUT

												IF 	@bErroreSingolo=1
							BEGIN
								SET @bErrore = @bErroreSingolo
								ROLLBACK TRANSACTION
							END

						FETCH NEXT FROM cursore 
						INTO @uIDPrescrizione
					END
					
								IF @bErrore=0 COMMIT TRANSACTION
				
				CLOSE cursore
				DEALLOCATE cursore
					
		  END 		END		END	
		
				SET @bErrore= ISNULL(@bErrore,0) 
		
	IF @bErrore=1 
		INSERT INTO T_TmpBoErrori
		SELECT @uIDSessione,Errore
		FROM @tblErrori


END