CREATE PROCEDURE [dbo].[MSP_BO_SpostaSchedaEpi](@xParametri XML, @bErrore BIT OUTPUT)  
AS
BEGIN



		DECLARE @sCodUAInizio AS VARCHAR(20)
	DECLARE @sNumeroCartellaInizio AS VARCHAR(50) 	
	DECLARE @sCodUAFine AS VARCHAR(20)
	DECLARE @sNumeroCartellaFine AS VARCHAR(50) 	
	DECLARE @uIDEntitaInizio AS UNIQUEIDENTIFIER
	DECLARE @uIDSchedaPadreFine AS UNIQUEIDENTIFIER
	DECLARE @nNumerositaFine AS INTEGER
	DECLARE @xTimeStamp AS XML
	
		DECLARE @uIDCartellaInizio AS UNIQUEIDENTIFIER
	DECLARE @uIDCartellaFine AS UNIQUEIDENTIFIER		
	DECLARE @uIDEntitaFine AS UNIQUEIDENTIFIER			
	DECLARE @uIDEpisodioInizio AS UNIQUEIDENTIFIER	
	DECLARE @uIDEpisodioFine AS UNIQUEIDENTIFIER		
	DECLARE @uIDPazienteInizio AS UNIQUEIDENTIFIER
	DECLARE @uIDPazienteFine AS UNIQUEIDENTIFIER		
	DECLARE @uIDTrasferimentoInizio AS UNIQUEIDENTIFIER
	DECLARE @uIDTrasferimentoFine AS UNIQUEIDENTIFIER		
	DECLARE @uIDSchedaPadreInizio AS UNIQUEIDENTIFIER
	DECLARE @nNumerositaInizio AS INTEGER		
	DECLARE @nRecordDestinazione AS INTEGER	
	DECLARE @uIDEntitaScheda AS UNIQUEIDENTIFIER
	DECLARE @sCodEntitaScheda AS VARCHAR(20)
	DECLARE @sCodScheda AS VARCHAR(20)
	DECLARE @sCodRuolo AS VARCHAR(20)	
		
	DECLARE @sGUID AS VARCHAR(50)
	DECLARE @nQTA AS INTEGER
	DECLARE @nRecord AS INTEGER	
	DECLARE @sInfoTimeStamp AS VARCHAR(255)	
		
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
		
	IF @xParametri.exist('/Parametri/IDEntitaInizio')=1	
		BEGIN					 
			SET @sGUID=(SELECT	TOP 1 ValoreParametro.IDEntitaInizio.value('.','VARCHAR(50)')
						FROM @xParametri.nodes('/Parametri/IDEntitaInizio') as ValoreParametro(IDEntitaInizio))
			IF ISNULL(@sGUID,'')<>'' SET @uIDEntitaInizio=CONVERT(UNIQUEIDENTIFIER,@sGUID)
		END
	
	IF @xParametri.exist('/Parametri/IDEntitaFine')=1	
		BEGIN						 
			SET @sGUID=(SELECT	TOP 1 ValoreParametro.IDEntitaFine.value('.','VARCHAR(50)')
						 FROM @xParametri.nodes('/Parametri/IDEntitaFine') as ValoreParametro(IDEntitaFine))
			IF ISNULL(@sGUID,'')<>'' SET @uIDEntitaFine=CONVERT(UNIQUEIDENTIFIER,@sGUID)			 			 						 
		END

	IF @xParametri.exist('/Parametri/IDSchedaPadreFine')=1	
	BEGIN						 
		SET @sGUID=(SELECT	TOP 1 ValoreParametro.IDSchedaPadreFine.value('.','VARCHAR(50)')
					 FROM @xParametri.nodes('/Parametri/IDSchedaPadreFine') as ValoreParametro(IDSchedaPadreFine))
		IF ISNULL(@sGUID,'')<>'' SET @uIDSchedaPadreFine=CONVERT(UNIQUEIDENTIFIER,@sGUID)			 			 						 
	END
	
	IF @xParametri.exist('/Parametri/NumerositaFine')=1	
	BEGIN						 
		SET @nNumerositaFine=(SELECT TOP 1 ValoreParametro.NumerositaFine.value('.','INTEGER')
					 FROM @xParametri.nodes('/Parametri/NumerositaFine') as ValoreParametro(NumerositaFine))		
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
				INSERT INTO #tmpErrori(Errore)
						VALUES('(GG001) ERRORE : nessuna cartella di origine trovata')
				SET @bErrore=1
			END
		ELSE
			BEGIN
																																						
						SELECT TOP 1 
							@uIDTrasferimentoInizio=ID,
							@uIDEpisodioInizio=IDEpisodio
						FROM T_MovTrasferimenti WHERE IDCartella=@uIDCartellaInizio
						
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
			AND T_MovTrasferimenti.CodStatoTrasferimento <> 'CA'			GROUP BY IDCartella
		
		IF  @nQta =0 OR @uIDCartellaFine IS NULL								BEGIN							
				INSERT INTO #tmpErrori(Errore)
						VALUES('(GG003) ERRORE : nessuna cartella di destinazione trovata')
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

		SET @sCodRuolo=(SELECT TOP 1 Valore FROM T_Config WHERE ID=681)
	
					IF (@uIDEpisodioFine IS NULL OR @uIDTrasferimentoFine IS NULL)
	BEGIN
				SET @bErrore=1
		INSERT INTO #tmpErrori(Errore)
			VALUES('(GG004) ERRORE IDEpisodio o IDTrasferimento di destinazione nulli')	
	END
	ELSE
	BEGIN
				IF @uIDEntitaInizio IS NULL 
		BEGIN
			SET @bErrore=1
			INSERT INTO #tmpErrori(Errore)
				VALUES('(SCHEpi001) IDScheda Episodio non specificato')
		
		END
		ELSE
		BEGIN
									SELECT TOP 1 
				@uIDPazienteInizio=IDPaziente,
				@uIDSchedaPadreInizio=IDSchedaPadre,
				@nNumerositaInizio=Numero,
				@uIDEntitaScheda=IDEntita,
				@sCodEntitaScheda=CodEntita,
				@sCodScheda=CodScheda
			FROM T_MovSchede 
			WHERE  ID=@uIDEntitaInizio
			
												SET @nRecord=(SELECT COUNT(*) FROM T_MovSchede 
						  WHERE ID=@uIDEntitaInizio AND
								CodEntita='EPI'  
																						 )					
						IF @nRecord =0
			BEGIN
								SET @bErrore=1
				INSERT INTO #tmpErrori(Errore)
						VALUES('(SCHEpi002) ERRORE Nessuna Scheda di tipo EPI da aggiornare {' + CONVERT(VARCHAR(50),@uIDEntitaInizio)+ '}')	
			END
			ELSE
			BEGIN	
																															
				IF @uIDEntitaFine<>@uIDEpisodioFine
					BEGIN
												SET @bErrore=1
						INSERT INTO #tmpErrori(Errore)
								VALUES('(SCHEpi003) ERRORE IDEpisodio di destinazione diverso da episodio della cartella')	
					END
					
								IF @uIDSchedaPadreFine is NOT NULL AND @uIDSchedaPadreInizio IS NULL
					BEGIN
												SET @bErrore=1
						INSERT INTO #tmpErrori(Errore)
								VALUES('(SCHEpi004) ERRORE Impossibile impostare Scheda padre, la scheda di origine NON ha scheda padre {' + CONVERT(VARCHAR(50),@uIDEntitaInizio)+ '}')	
					END
			
								IF @uIDSchedaPadreFine is NOT NULL AND (SELECT COUNT(*) FROM T_MovSchede WHERE ID = @uIDSchedaPadreFine AND IDEpisodio = @uIDEpisodioFine)=0
					BEGIN
												SET @bErrore=1
						INSERT INTO #tmpErrori(Errore)
								VALUES('(SCHEpi007) ERRORE Impossibile impostare Scheda padre, verificare coerenza IDSchedaPadreFinale {' + CONVERT(VARCHAR(50),@uIDEntitaInizio)+ '}')	
					END
					
																			
				IF @uIDEntitaScheda <> @uIDEpisodioInizio 
					BEGIN
												SET @bErrore=1
						INSERT INTO #tmpErrori(Errore)
								VALUES('(SCHEpi005) ERRORE IDEpisodio associato alla scheda diverso dall''episodio della cartella {' + CONVERT(VARCHAR(50),@uIDEntitaInizio)+ '}')
					END


				
								IF @uIDSchedaPadreFine IS NULL
				BEGIN
					SET @nRecordDestinazione=
							   (SELECT COUNT(*) FROM T_MovSchede								  
								WHERE 
									Storicizzata=0 AND
									CodStatoScheda NOT IN ('CA') AND
									IDEntita=@uIDEpisodioFine AND
									CodEntita=@sCodEntitaScheda AND
									Numero=ISNULL(@nNumerositaFine,@nNumerositaInizio) AND
									CodScheda=@sCodScheda)
							
					IF @nRecordDestinazione>0
					BEGIN
												SET @bErrore=1
						INSERT INTO #tmpErrori(Errore)
								VALUES('(SCHEpi006) ERRORE Esiste già una scheda attività per la numerosità di origine {' + CONVERT(VARCHAR(50),@uIDEntitaInizio)+ '}')	
					END						
				END	
				ELSE
				BEGIN
										SET @nRecordDestinazione=
							   (SELECT COUNT(*) FROM T_MovSchede								  
								WHERE 
									Storicizzata=0 AND
									CodStatoScheda NOT IN ('CA') AND
									IDEntita=@uIDEpisodioFine AND
									CodEntita=@sCodEntitaScheda AND
									IDSchedaPadre=@uIDSchedaPadreFine AND
									Numero=ISNULL(@nNumerositaFine,@nNumerositaInizio) AND
									CodScheda=@sCodScheda)
							
					IF @nRecordDestinazione>0
					BEGIN
												SET @bErrore=1
						INSERT INTO #tmpErrori(Errore)
								VALUES('(SCHEpi008) ERRORE Esiste già una scheda attività per la numerosità di origine {' + CONVERT(VARCHAR(50),@uIDEntitaInizio)+ '}')	
					END			
				END								

				IF @bErrore=0
				BEGIN							
										UPDATE  T_MovSchede
					SET 
						IDPaziente=@uIDPazienteFine,
						IDEntita=@uIDEpisodioFine,
						IDEpisodio=@uIDEpisodioFine,
												IDTrasferimento=@uIDTrasferimentoFine,
						IDSchedaPadre=ISNULL(@uIDSchedaPadreFine,IDSchedaPadre),
						Numero=ISNULL(@nNumerositaFine,Numero)
					WHERE IDEntita=@uIDEntitaScheda AND
						  CodEntita=@sCodEntitaScheda AND
						  Numero=@nNumerositaInizio AND
						  CodScheda=@sCodScheda
																	
										IF @bErrore=0 
					BEGIN			
												SET @xTimeStamp.modify('delete (/TimeStamp/CodRuolo)[1]') 					
						SET @xTimeStamp.modify('insert <CodRuolo>{sql:variable("@sCodRuolo")}</CodRuolo> into (/TimeStamp)[1]')

												SET @xTimeStamp.modify('delete (/TimeStamp/IDEntita)[1]') 					
						SET @xTimeStamp.modify('insert <IDEntita>{sql:variable("@uIDEntitaInizio")}</IDEntita> into (/TimeStamp)[1]')
					
												SET @xTimeStamp.modify('delete (/TimeStamp/IDEpisodio)[1]') 					
						SET @xTimeStamp.modify('insert <IDEpisodio>{sql:variable("@uIDEpisodioInizio")}</IDEpisodio> into (/TimeStamp)[1]')

												SET @xTimeStamp.modify('delete (/TimeStamp/IDPaziente)[1]') 					
						SET @xTimeStamp.modify('insert <IDPaziente>{sql:variable("@uIDPazienteInizio")}</IDPaziente> into (/TimeStamp)[1]')

												SET @xTimeStamp.modify('delete (/TimeStamp/IDTrasferimento)[1]') 					
						SET @xTimeStamp.modify('insert <IDTrasferimento>{sql:variable("@uIDTrasferimentoInizio")}</IDTrasferimento> into (/TimeStamp)[1]')
					
												SET @sInfoTimeStamp='Azione: BO_SpostaSCHEpi'
												SET @sInfoTimeStamp=@sInfoTimeStamp + ' IDPazienteFine: ' + + CONVERT(VARCHAR(50),@uIDPazienteFine) 
							SET @sInfoTimeStamp=@sInfoTimeStamp + ' IDEpisodioFine: ' + + CONVERT(VARCHAR(50),@uIDEpisodioFine) 
					
						IF @uIDSchedaPadreFine IS NOT NULL
							SET @sInfoTimeStamp=@sInfoTimeStamp + ' IDSchedaPadreFine: ' + + CONVERT(VARCHAR(50),@uIDSchedaPadreFine) 
						
						IF @nNumerositaFine IS NOT NULL
							SET @sInfoTimeStamp=@sInfoTimeStamp + ' NumerositaFine: ' + + CONVERT(VARCHAR(20),@nNumerositaFine) 
					
						SET @xTimeStamp.modify('delete (/TimeStamp/Info)[1]') 		
						SET @xTimeStamp.modify('insert <Info>{sql:variable("@sInfoTimeStamp")}</Info> into (/TimeStamp)[1]')
					
												SET @xTimeStamp.modify('delete (/TimeStamp/CodAzione)[1]') 		
						SET @xTimeStamp.modify('insert <CodAzione>MOD</CodAzione> into (/TimeStamp)[1]')
				
						SET @xTimeStamp=CONVERT(XML,
											'<Parametri>' + CONVERT(varchar(max),@xTimeStamp) +
											'</Parametri>')
										
												EXEC MSP_InsMovTimeStamp @xTimeStamp					
					END 				
			  END 			END			END	
	END	
				SET @bErrore= ISNULL(@bErrore,0) 
			
	SELECT Errore FROM #tmpErrori	
	
		DROP TABLE #tmpErrori
	
END