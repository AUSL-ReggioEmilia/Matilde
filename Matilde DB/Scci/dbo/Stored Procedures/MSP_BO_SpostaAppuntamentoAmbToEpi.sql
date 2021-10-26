CREATE PROCEDURE [dbo].[MSP_BO_SpostaAppuntamentoAmbToEpi](@xParametri XML, @bErrore BIT OUTPUT)  
AS
BEGIN

	
	
		DECLARE @uIDEntitaInizio AS UNIQUEIDENTIFIER	

	DECLARE @sCodUAFine AS VARCHAR(20)
	DECLARE @sNumeroCartellaFine AS VARCHAR(50) 	
	DECLARE @xTimeStamp AS XML
		
		DECLARE @uIDPazienteInizio AS UNIQUEIDENTIFIER						
	DECLARE @uIDEpisodioInizio AS UNIQUEIDENTIFIER	
	DECLARE @uIDTrasferimentoInizio AS UNIQUEIDENTIFIER

	DECLARE @uIDEntitaFine AS UNIQUEIDENTIFIER		
	DECLARE @uIDPazienteFine AS UNIQUEIDENTIFIER	
	DECLARE @uIDCartellaFine AS UNIQUEIDENTIFIER	
	DECLARE @uIDEpisodioFine AS UNIQUEIDENTIFIER
	DECLARE @uIDTrasferimentoFine AS UNIQUEIDENTIFIER	
	DECLARE @uIDPazienteInizioFuso AS UNIQUEIDENTIFIER	
	DECLARE @uIDPazienteFineFuso AS UNIQUEIDENTIFIER			
	

	DECLARE @sCodRuolo AS VARCHAR(20)
			
	DECLARE @sGUID AS VARCHAR(50)
	DECLARE @nQTA AS INTEGER
	DECLARE @nRecord AS INTEGER		
	DECLARE @sInfoTimeStamp AS VARCHAR(255)	
	DECLARE @nNumTransazioni AS INTEGER
		
		IF @xParametri.exist('/Parametri/IDEntitaInizio')=1	
		BEGIN					 
			SET @sGUID=(SELECT	TOP 1 ValoreParametro.IDEntitaInizio.value('.','VARCHAR(50)')
						FROM @xParametri.nodes('/Parametri/IDEntitaInizio') as ValoreParametro(IDEntitaInizio))
			IF ISNULL(@sGUID,'')<>'' SET @uIDEntitaInizio=CONVERT(UNIQUEIDENTIFIER,@sGUID)
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

		IF @xParametri.exist('/Parametri/TimeStamp')=1	
		BEGIN								  				  				  				  	
			SET @xTimeStamp=(SELECT TOP 1 ValoreParametro.TS.query('.')
							  FROM @xParametri.nodes('/Parametri/TimeStamp') as ValoreParametro(TS))	
		END					  						

		SET @bErrore=0
	SET @nRecord=0

			
	CREATE TABLE #tmpErrori
		(		
			Errore VARCHAR(500) COLLATE Latin1_General_CI_AS
		)
		
			
	
		SET @sCodRuolo=(SELECT TOP 1 Valore FROM T_Config WHERE ID=681)

					
	IF @uIDEntitaInizio IS NULL 
	BEGIN
		SET @bErrore=1
		INSERT INTO #tmpErrori(Errore)
			VALUES('(APPAMBEPI001) IDAppuntamento NON specificato')
	END
	ELSE
	BEGIN						

				SET @uIDPazienteInizio=(SELECT TOP 1 IDPaziente FROM T_MovAppuntamenti WHERE  ID=@uIDEntitaInizio)
							
				SET @nRecord=(SELECT COUNT(*) FROM T_MovAppuntamenti 
					  WHERE ID=@uIDEntitaInizio AND
							IDEpisodio IS NULL AND
							IDTrasferimento IS NULL
					 )					
				IF @nRecord =0
		BEGIN
						SET @bErrore=1
			INSERT INTO #tmpErrori(Errore)
					VALUES('(APPAMBEPI002) ERRORE Nessun Appuntamento di tipo paziente da spostare')
		END
		ELSE
		BEGIN	

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
						SET @bErrore=1
						INSERT INTO #tmpErrori
								VALUES('(APPAMBEPI002) ERRORE : nessuna cartella di destinazione trovata')					
					END
				ELSE
					BEGIN						
												SELECT TOP 1 
							@uIDEpisodioFine=IDEpisodio
						FROM T_MovTrasferimenti WHERE IDCartella=@uIDCartellaFine
										
						SET @uIDPazienteFine=(SELECT IDPaziente FROM T_MovPazienti WHERE IDEpisodio=@uIDEpisodioFine)				
					END
			END			

			IF @uIDPazienteFine IS NULL
			BEGIN
								SET @bErrore=1
				INSERT INTO #tmpErrori(Errore)
					VALUES('(APPAMBEPI003) ERRORE Paziente di fine non trovato {IDEpisodio: ' + CONVERT(VARCHAR(50),@uIDEpisodioFine)+ '}')					
			END
			ELSE
				BEGIN
										IF @uIDPazienteInizio <> @uIDPazienteFine
					BEGIN
												
												SET @uIDPazienteInizioFuso=(SELECT TOP 1 IDPaziente FROM T_PazientiAlias WHERE IDPazienteVecchio=@uIDPazienteInizio)

												SET @uIDPazienteFineFuso=(SELECT TOP 1 IDPaziente FROM T_PazientiAlias WHERE IDPazienteVecchio=@uIDPazienteFine)

												IF  ((@uIDPazienteInizioFuso IS NOT NULL AND @uIDPazienteFineFuso IS NULL) OR 
							 (@uIDPazienteInizioFuso IS NULL AND @uIDPazienteFineFuso IS NOT NULL) OR
							 (@uIDPazienteInizioFuso IS NULL AND @uIDPazienteFineFuso IS NULL)
							 )
						BEGIN
														SET @bErrore=1
							INSERT INTO #tmpErrori(Errore)
								VALUES('(APPAMBEPI004) ERRORE Spostamento Appuntamento su paziente differente {' + CONVERT(VARCHAR(50),@uIDPazienteFine)+ '}')								
						END
						ELSE
						BEGIN
														IF @uIDPazienteInizioFuso <> @uIDPazienteFineFuso
							BEGIN
								SET @bErrore=1
								INSERT INTO #tmpErrori(Errore)
								VALUES('(APPAMBEPI005) ERRORE Spostamento Appuntamento su paziente differente {' + CONVERT(VARCHAR(50),@uIDPazienteFine)+ '}')									
							END
						END
					END	
				END
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
			AND T_MovTrasferimenti.CodStatoTrasferimento <> 'CA'			    		GROUP BY IDCartella
				
		IF  @nQta =0 OR @uIDCartellaFine IS NULL								BEGIN							
				INSERT INTO #tmpErrori(Errore)
						VALUES('(APPAMBEPI004) ERRORE : nessuna cartella di destinazione trovata')
				SET @bErrore=1
			END
		ELSE		
			BEGIN			
				
								SELECT TOP 1 
					@uIDTrasferimentoFine=ID,
					@uIDEpisodioFine=IDEpisodio
				FROM T_MovTrasferimenti 
				WHERE 
					IDCartella=@uIDCartellaFine AND
					DataUscita IS NULL					
				
								IF @uIDTrasferimentoFine IS NULL OR @uIDEpisodioFine IS NULL
				BEGIN
					SELECT TOP 1 
						@uIDTrasferimentoFine=ID,
						@uIDEpisodioFine=IDEpisodio
					FROM T_MovTrasferimenti 
					WHERE 
						IDCartella=@uIDCartellaFine AND
						CodStatoTrasferimento='DM'
				END
								SET @uIDPazienteFine=(SELECT IDPaziente FROM T_MovPazienti WHERE IDEpisodio=@uIDEpisodioFine)				
			END			
	END	

	
	IF @bErrore=0
	BEGIN
				SET @nNumTransazioni=@@TRANCOUNT
			
		BEGIN TRANSACTION
			
				UPDATE  T_MovAppuntamenti
		SET 
			IDEpisodio=@uIDEpisodioFine,
			IDTrasferimento = @uIDTrasferimentoFine				
		WHERE ID=@uIDEntitaInizio
				
		IF @@ERROR=0
			BEGIN
						
								UPDATE  T_MovSchede
				SET IDEpisodio=@uIDEpisodioFine,
					IDTrasferimento=@uIDTrasferimentoFine
				WHERE 
					IDEntita=@uIDEntitaInizio AND
					CodEntita='APP'																											
												
				IF @@ERROR=0									
					COMMIT TRANSACTION								
				ELSE
					BEGIN							
						IF @nNumTransazioni=0 ROLLBACK	TRANSACTION
						SET @bErrore=1
						INSERT INTO #tmpErrori(Errore)
							VALUES('(APPAMBEPI005) ERRORE durante aggiornamento della scheda del AMB {' + CONVERT(VARCHAR(50),@uIDEntitaInizio)+ '}')
					END	
			END		  
		ELSE
			BEGIN					
								IF @nNumTransazioni=0 ROLLBACK TRANSACTION
				SET @bErrore=1
				INSERT INTO #tmpErrori(Errore)
							VALUES('(APPAMBEPI006) ERRORE durante aggiornamento del AMB {' + CONVERT(VARCHAR(50),@uIDEntitaInizio)+ '}')
			END	
				
		IF @bErrore=0 
	BEGIN			
				SET @xTimeStamp.modify('delete (/TimeStamp/CodRuolo)[1]') 					
		SET @xTimeStamp.modify('insert <CodRuolo>{sql:variable("@sCodRuolo")}</CodRuolo> into (/TimeStamp)[1]')

				SET @xTimeStamp.modify('delete (/TimeStamp/IDEntita)[1]') 					
		SET @xTimeStamp.modify('insert <IDEntita>{sql:variable("@uIDEntitaInizio")}</IDEntita> into (/TimeStamp)[1]')
						
				SET @sInfoTimeStamp='Azione: SpostaSingoloAppAmbToEpi'				
		SET @sInfoTimeStamp=@sInfoTimeStamp + ' IDEpisodioFine: ' + + CONVERT(VARCHAR(50),@uIDEpisodioFine) + ' '
		SET @sInfoTimeStamp=@sInfoTimeStamp + ' IDTrasferimentoFine: ' + + CONVERT(VARCHAR(50),@uIDTrasferimentoFine)  									
		SET @xTimeStamp.modify('delete (/TimeStamp/Info)[1]') 		
		SET @xTimeStamp.modify('insert <Info>{sql:variable("@sInfoTimeStamp")}</Info> into (/TimeStamp)[1]')
				
				SET @xTimeStamp.modify('delete (/TimeStamp/CodAzione)[1]') 		
		SET @xTimeStamp.modify('insert <CodAzione>MOD</CodAzione> into (/TimeStamp)[1]')
			
		SET @xTimeStamp=CONVERT(XML,
							'<Parametri>' + CONVERT(varchar(max),@xTimeStamp) +
							'</Parametri>')
									
				EXEC MSP_InsMovTimeStamp @xTimeStamp					
	END 	END 

				SET @bErrore= ISNULL(@bErrore,0) 
			
	SELECT Errore FROM #tmpErrori	
	
		DROP TABLE #tmpErrori
	
END