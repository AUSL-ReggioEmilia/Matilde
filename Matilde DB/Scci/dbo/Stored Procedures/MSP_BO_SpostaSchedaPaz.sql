CREATE PROCEDURE [dbo].[MSP_BO_SpostaSchedaPaz](@xParametri XML, @bErrore BIT OUTPUT)  
AS
BEGIN


		DECLARE @uIDEntitaInizio AS UNIQUEIDENTIFIER	
	DECLARE @uIDPazienteFine AS UNIQUEIDENTIFIER		
	DECLARE @uIDSchedaPadreFine AS UNIQUEIDENTIFIER
	DECLARE @nNumerositaFine AS INTEGER
	DECLARE @xTimeStamp AS XML
	
		DECLARE @uIDSchedaPadreInizio AS UNIQUEIDENTIFIER
	DECLARE @nNumerositaInizio AS INTEGER		
	DECLARE @uIDEpisodioInizio AS UNIQUEIDENTIFIER
	DECLARE @uIDEpisodioFine AS UNIQUEIDENTIFIER
	DECLARE @uIDPazienteInizio AS UNIQUEIDENTIFIER		
	DECLARE @uIDTrasferimentoInizio AS UNIQUEIDENTIFIER
	DECLARE @uIDTrasferimentoFine AS UNIQUEIDENTIFIER	
	DECLARE @uIDEntitaScheda AS UNIQUEIDENTIFIER
	DECLARE @sCodEntitaScheda AS VARCHAR(20)
	DECLARE @sCodScheda AS VARCHAR(20)
	DECLARE @sCodRuolo AS VARCHAR(20)
	DECLARE @sCodStatoScheda AS VARCHAR(20)
		
	DECLARE @sGUID AS VARCHAR(50)
	DECLARE @nQTA AS INTEGER	
	DECLARE @nRecord AS INTEGER	
	DECLARE @nRecordDestinazione AS INTEGER	
	DECLARE @sInfoTimeStamp AS VARCHAR(255)		
	
	DECLARE @nQTASchedeFiglio AS INTEGER
	DECLARE @sSQLSchedeFiglio AS VARCHAR(MAX)
	DECLARE @uIDSchedaFiglio AS UNIQUEIDENTIFIER
	DECLARE @nNumeroSchedaFiglio AS INTEGER
	DECLARE @xSPSchedaFiglio AS XML
	DECLARE @xTimeStampIniziale AS XML
								 			
	IF @xParametri.exist('/Parametri/IDEntitaInizio')=1	
		BEGIN					 
			SET @sGUID=(SELECT	TOP 1 ValoreParametro.IDEntitaInizio.value('.','VARCHAR(50)')
						FROM @xParametri.nodes('/Parametri/IDEntitaInizio') as ValoreParametro(IDEntitaInizio))
			IF ISNULL(@sGUID,'')<>'' SET @uIDEntitaInizio=CONVERT(UNIQUEIDENTIFIER,@sGUID)
		END
	
	IF @xParametri.exist('/Parametri/IDPazienteFine')=1	
	BEGIN						 
		SET @sGUID=(SELECT	TOP 1 ValoreParametro.IDPazienteFine.value('.','VARCHAR(50)')
					 FROM @xParametri.nodes('/Parametri/IDPazienteFine') as ValoreParametro(IDPazienteFine))
		IF ISNULL(@sGUID,'')<>'' SET @uIDPazienteFine=CONVERT(UNIQUEIDENTIFIER,@sGUID)			 			 						 
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
		
			
	CREATE TABLE #tmpSchedeFiglio
		(		
			IDSchedaFiglio UNIQUEIDENTIFIER,
			Numero INTEGER
		)
		
				
	IF @uIDEntitaInizio IS NULL 
	BEGIN
		SET @bErrore=1
		INSERT INTO #tmpErrori(Errore)
			VALUES('(SCHPaz001) IDScheda Paziente non specificato')
		
	END
	ELSE
	BEGIN
				SELECT TOP 1 
			@uIDPazienteInizio=IDPaziente,
			@uIDSchedaPadreInizio=IDSchedaPadre,
			@nNumerositaInizio=Numero,
			@uIDEntitaScheda=IDEntita,
			@sCodEntitaScheda=CodEntita,
			@sCodScheda=CodScheda,
			@sCodStatoScheda=CodStatoScheda
		FROM T_MovSchede 
		WHERE  ID=@uIDEntitaInizio
							
				SET @nRecord=(SELECT COUNT(*) FROM T_MovSchede 
					  WHERE ID=@uIDEntitaInizio AND
							CodEntita='PAZ'  
																			 )					
				IF @nRecord =0
		BEGIN
						SET @bErrore=1
			INSERT INTO #tmpErrori(Errore)
					VALUES('(SCHPaz002) ERRORE Nessuna Scheda di tipo PAZ da aggiornare')	
		END
		ELSE
		BEGIN	
			IF @uIDPazienteInizio=@uIDPazienteFine AND @sCodStatoScheda <> 'CA'
			BEGIN
								SET @bErrore=1
				INSERT INTO #tmpErrori(Errore)
						VALUES('(SCHPaz003) ERRORE Scheda già associato al paziente di destinazione {' + CONVERT(VARCHAR(50),@uIDEntitaInizio)+ '}')	
			END
			
									IF @uIDSchedaPadreFine is NOT NULL AND @uIDSchedaPadreInizio IS NULL
			BEGIN
								SET @bErrore=1
				INSERT INTO #tmpErrori(Errore)
						VALUES('(SCHPaz004) ERRORE Impossibile impostare Scheda padre, la scheda di origine NON ha scheda padre {' + CONVERT(VARCHAR(50),@uIDEntitaInizio)+ '}')	
			END
			
			
			
						IF @uIDSchedaPadreFine IS NULL
			BEGIN
								
				SET @nRecordDestinazione=
						   (SELECT COUNT(*) FROM T_MovSchede								  
							WHERE 
								Storicizzata=0 AND
								CodStatoScheda NOT IN ('CA') AND
								IDEntita=@uIDPazienteFine AND
								CodEntita=@sCodEntitaScheda AND
								Numero=ISNULL(@nNumerositaFine,@nNumerositaInizio) AND
								CodScheda=@sCodScheda)

				IF @nRecordDestinazione>0
				BEGIN
									SET @bErrore=1
					INSERT INTO #tmpErrori(Errore)
							VALUES('(SCHPaz005) ERRORE Esiste già una scheda attività per la numerosità di origine {' + CONVERT(VARCHAR(50),@uIDEntitaInizio)+ '}')	
				END
			END
			ELSE
			BEGIN
								SET @nRecordDestinazione=
						   (SELECT COUNT(*) FROM T_MovSchede								  
							WHERE 
								Storicizzata=0 AND
								CodStatoScheda NOT IN ('CA') AND
								IDEntita=@uIDPazienteFine AND
								CodEntita=@sCodEntitaScheda AND
								IDSchedaPadre=@uIDSchedaPadreFine AND
								Numero=ISNULL(@nNumerositaFine,@nNumerositaInizio) AND
								CodScheda=@sCodScheda)
							
				IF @nRecordDestinazione>0
				BEGIN
										SET @bErrore=1
					INSERT INTO #tmpErrori(Errore)
							VALUES('(SCHPaz007) ERRORE Esiste già una scheda attività per la numerosità di origine {' + CONVERT(VARCHAR(50),@uIDEntitaInizio)+ '}')	
				END			
			END

			 			SET @nRecordDestinazione=
					   (SELECT COUNT(*) 
					    FROM T_Pazienti								  
						WHERE 
						    ID=@uIDPazienteFine
						)    
							
			IF @nRecordDestinazione=0
				BEGIN
							SET @bErrore=1
				INSERT INTO #tmpErrori(Errore)
						VALUES('(SCHPaz006) ERRORE ID Paziente finale non trovato {' + CONVERT(VARCHAR(50),@uIDEntitaInizio)+ '}')	
			END
			
			IF @bErrore=0
			BEGIN							
								UPDATE  T_MovSchede
				SET 
					IDPaziente=@uIDPazienteFine,
					IDEntita=@uIDPazienteFine,
					IDEpisodio=NULL,
					IDTrasferimento=NULL,
					IDSchedaPadre=ISNULL(@uIDSchedaPadreFine,IDSchedaPadre),
					Numero=ISNULL(@nNumerositaFine,Numero)
				WHERE 
					IDEntita=@uIDEntitaScheda AND
					CodEntita=@sCodEntitaScheda AND
					Numero=@nNumerositaInizio AND
					CodScheda=@sCodScheda
																	
								IF @bErrore=0 
				BEGIN			
										SET @xTimeStampIniziale= @xTimeStamp

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
					
										SET @sInfoTimeStamp='Azione: BO_SpostaSCHPaz'
										SET @sInfoTimeStamp=@sInfoTimeStamp + ' IDPazienteFine: ' + + CONVERT(VARCHAR(50),@uIDPazienteFine) 
					
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
					
															
					INSERT INTO #tmpSchedeFiglio(IDSchedaFiglio,Numero)
					SELECT ID,Numero
					FROM T_MovSchede
					WHERE IDSchedaPadre = @uIDEntitaInizio

					SET @nQTASchedeFiglio = 0
					SET @nQTASchedeFiglio = (SELECT COUNT(*) FROM #tmpSchedeFiglio)
					
					IF @nQTASchedeFiglio > 0
					BEGIN

						DECLARE cFigli CURSOR LOCAL
						FOR   
							SELECT IDSchedaFiglio,Numero
							FROM #tmpSchedeFiglio											
  
						OPEN cFigli  
  
						FETCH NEXT FROM cFigli   
						INTO @uIDSchedaFiglio, @nNumeroSchedaFiglio
  
						WHILE @@FETCH_STATUS = 0 
						BEGIN  
														SET @sSQLSchedeFiglio =''
						
							SET @sSQLSchedeFiglio = @sSQLSchedeFiglio + '<Parametri>' + CHAR(13) + CHAR(10) 							
							SET @sSQLSchedeFiglio = @sSQLSchedeFiglio + '     <IDEntitaInizio>' + CONVERT(VARCHAR(50),@uIDSchedaFiglio) + ' </IDEntitaInizio>' + CHAR(13) + CHAR(10) 
							SET @sSQLSchedeFiglio = @sSQLSchedeFiglio + '     <IDPazienteFine>' + CONVERT(VARCHAR(50),@uIDPazienteFine) + ' </IDPazienteFine>' + CHAR(13) + CHAR(10) 
							SET @sSQLSchedeFiglio = @sSQLSchedeFiglio + '     <IDSchedaPadreFine>' + CONVERT(VARCHAR(50),@uIDEntitaInizio) + ' </IDSchedaPadreFine>' + CHAR(13) + CHAR(10) 
							SET @sSQLSchedeFiglio = @sSQLSchedeFiglio + '     <NumerositaFine>' + CONVERT(VARCHAR(50),@nNumeroSchedaFiglio) + ' </NumerositaFine>' + CHAR(13) + CHAR(10) 						
							SET @sSQLSchedeFiglio = @sSQLSchedeFiglio + '</Parametri>' + CHAR(13) + CHAR(10) 

							SET @xSPSchedaFiglio=CONVERT(XML,@sSQLSchedeFiglio)														
							SET @xSPSchedaFiglio.modify('delete (/TimeStamp)[1]') 		
							SET @xSPSchedaFiglio.modify('insert sql:variable("@xTimeStampIniziale") into (/Parametri)[1]')
							
							SET @sSQLSchedeFiglio=CONVERT(VARCHAR(MAX),@xSPSchedaFiglio)														

							SET @sSQLSchedeFiglio = 'DECLARE @bErroreOut AS BIT EXEC [MSP_BO_SpostaSchedaPaz] ''' + @sSQLSchedeFiglio + ''', @bErroreOut OUTPUT'
													
							EXEC (@sSQLSchedeFiglio)
							

							FETCH NEXT FROM cFigli   
							INTO @uIDSchedaFiglio, @nNumeroSchedaFiglio
						END
					CLOSE cFigli;  
					DEALLOCATE cFigli;

						

						
					END

									
				END 		  END 		END		END	

				SET @bErrore= ISNULL(@bErrore,0) 			
	SELECT Errore FROM #tmpErrori	
	
		DROP TABLE #tmpErrori
	DROP TABLE #tmpSchedeFiglio

	
	
END