CREATE PROCEDURE [dbo].[MSP_BO_CartellaScollega](@xParametri XML, @bErrore BIT OUTPUT)  
AS
BEGIN




		DECLARE @sCodUAInizio AS VARCHAR(20)
	DECLARE @sNumeroCartellaInizio AS VARCHAR(50) 
	DECLARE @xTimeStamp AS XML
			
		DECLARE @uIDEntitaInizio AS UNIQUEIDENTIFIER	
	DECLARE @uIDCartellaInizio AS UNIQUEIDENTIFIER		
	DECLARE @uIDEpisodioInizio AS UNIQUEIDENTIFIER	
	DECLARE @uIDPazienteInizio AS UNIQUEIDENTIFIER		
	DECLARE @uIDTrasferimentoInizio AS UNIQUEIDENTIFIER
	
	DECLARE @uIDEpisodioFine AS UNIQUEIDENTIFIER		
	DECLARE @uIDTrasferimentoFine AS UNIQUEIDENTIFIER
	
	DECLARE @sCodRuolo AS VARCHAR(20)
	
	DECLARE @uIDCartellaFine AS UNIQUEIDENTIFIER	
	DECLARE @uIDSessione AS UNIQUEIDENTIFIER
	
	DECLARE @sGUID AS VARCHAR(50)
	DECLARE @nQTA AS INTEGER
	DECLARE @nRecord AS INTEGER		
	DECLARE @sInfoTimeStamp AS VARCHAR(255)	
	DECLARE @sTemp AS VARCHAR(255)	
		
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
		SET @sCodRuolo=(SELECT TOP 1 Valore FROM T_Config WHERE ID=681)

					
		
	SET @uIDEntitaInizio=@uIDCartellaInizio
	IF @uIDEntitaInizio IS NULL 
		BEGIN
			SET @bErrore=1
			INSERT INTO #tmpErrori(Errore)
				VALUES('(AC001) IDCartella NON specificata')
			
		END
	ELSE
	BEGIN								
				SET @nRecord=(SELECT COUNT(*) FROM T_MovRelazioniEntita
					  WHERE 
							CodEntita='CAR' AND
							CodEntitaCollegata='CAR' AND
							IDEntitaCollegata=@uIDEntitaInizio AND
							IDEntita IN 
								(SELECT ID 
								 FROM T_MovCartelle 
								 WHERE CodStatoCartella='CH')
					 )		
		
				IF @nRecord =0
			BEGIN
								SET @bErrore=1
				INSERT INTO #tmpErrori(Errore)
						VALUES('(AC002) ERRORE Nessuna Cartella Collegata Chiusa da riaprire')	
			END
		ELSE
		BEGIN		
						SET @uIDCartellaFine=(SELECT TOP 1 IDEntita FROM T_MovRelazioniEntita
						  WHERE 
								CodEntita='CAR' AND
								CodEntitaCollegata='CAR' AND
								IDEntitaCollegata=@uIDEntitaInizio AND
								IDEntita IN 
									(SELECT ID 
									 FROM T_MovCartelle 
									 WHERE CodStatoCartella='CH')
						 )
			IF @uIDCartellaFine IS NULL 
				BEGIN
										SET @bErrore=1
					INSERT INTO #tmpErrori(Errore)
							VALUES('(AC003) ERRORE Nessuna Cartella Collegata Collegata da riaprire, ID Cartella non riprovato')	
				END
		  ELSE
			BEGIN
								SET @uIDEpisodioFine=(SELECT TOP 1 IDEpisodio FROM T_MovTrasferimenti WHERE IDCartella=@uIDCartellaFine)
				IF @uIDEpisodioFine IS NULL 
					BEGIN
												SET @bErrore=1
						INSERT INTO #tmpErrori(Errore)
								VALUES('(AC004) ERRORE IDEpisodio Cartella Collegata Chiusa da riparire non trovato')	
					END
				ELSE
					BEGIN
											SET @uIDTrasferimentoFine=(SELECT TOP 1 ID FROM T_MovTrasferimenti WHERE IDCartella=@uIDCartellaFine)
						IF @uIDTrasferimentoFine IS NULL 
						BEGIN
														SET @bErrore=1
							INSERT INTO #tmpErrori(Errore)
								VALUES('(AC005) ERRORE IDTrasferimento Cartella Collegata Chiusa da riparire non trovato')	
						END
						ELSE
							BEGIN
																SET @sTemp=(
											  SELECT TOP 1 ISNULL(C.NumeroCartella,'') + ' ID:' + CONVERT(VARCHAR(255),C.ID)
											  FROM T_MovRelazioniEntita E
														LEFT JOIN T_MovCartelle C
															ON E.IDEntita=C.ID
											  WHERE 
													CodEntita='CAR' AND
													CodEntitaCollegata='CAR' AND
													IDEntita=@uIDEntitaInizio													
											 )
								IF @sTemp IS NOT NULL 
									BEGIN
																				SET @bErrore=1
										INSERT INTO #tmpErrori(Errore)
											VALUES('(AC006) ERRORE Cartella collegata ad un''altra cartella ' + @sTemp )	
									END		
									
								END										
					END
			END
		END
	END 

IF @bErrore=0
	BEGIN
	
					


												
						UPDATE T_MovCartelle
			SET CodStatoCartella='CA'
			WHERE ID=@uIDCartellaInizio
			
									
						UPDATE T_MovAlertGenerici
			SET CodStatoAlertGenerico='CA'
			WHERE 
				CodStatoAlertGenerico <> 'CA' AND
				IDTrasferimento IN (SELECT ID FROM T_MovTrasferimenti WHERE IDCartella=@uIDCartellaFine)
			
			UPDATE T_MovAlertGenerici
			SET		
				IDEpisodio=@uIDEpisodioFine,
				IDTrasferimento=@uIDTrasferimentoFine			
			WHERE IDTrasferimento IN (SELECT ID FROM T_MovTrasferimenti WHERE IDCartella=@uIDCartellaInizio)
			
			UPDATE T_MovSchede
			SET IDEpisodio=@uIDEpisodioFine,
				IDTrasferimento=@uIDTrasferimentoFine
			WHERE 
				CodEntita='ALG' AND
				IDTrasferimento IN (SELECT ID FROM T_MovTrasferimenti WHERE IDCartella=@uIDCartellaInizio)
				
						UPDATE T_MovAllegati
			SET CodStatoAllegato='CA'
			WHERE 
				CodStatoAllegato <> 'CA' AND
				IDTrasferimento IN (SELECT ID FROM T_MovTrasferimenti WHERE IDCartella=@uIDCartellaFine)
			
			UPDATE T_MovAllegati
			SET IDEpisodio= @uIDEpisodioFine,
				IDTrasferimento= @uIDTrasferimentoFine			
			WHERE 
				IDTrasferimento IN (SELECT ID FROM T_MovTrasferimenti WHERE IDCartella=@uIDCartellaInizio)
			
			UPDATE T_MovSchede
			SET IDEpisodio=@uIDEpisodioFine,
				IDTrasferimento=@uIDTrasferimentoFine
			WHERE 
				CodEntita='ALL' AND
				IDTrasferimento IN (SELECT ID FROM T_MovTrasferimenti WHERE IDCartella=@uIDCartellaInizio)

			
									UPDATE T_MovAppuntamentiAgende
			SET CodStatoAppuntamentoAgenda='CA'
			WHERE
				CodStatoAppuntamentoAgenda<>'CA' AND
				IDAppuntamento IN (SELECT ID FROM T_MovAppuntamenti 
								   WHERE IDTrasferimento IN 
									(SELECT ID FROM T_MovTrasferimenti WHERE IDCartella=@uIDCartellaFine)
								   )			
														   
			UPDATE T_MovAppuntamenti
			SET CodStatoAppuntamento='CA'
			WHERE 
				CodStatoAppuntamento <> 'CA' AND
				IDTrasferimento IN (SELECT ID FROM T_MovTrasferimenti WHERE IDCartella=@uIDCartellaFine)
										
			UPDATE T_MovAppuntamenti
			SET IDEpisodio= @uIDEpisodioFine,
				IDTrasferimento= @uIDTrasferimentoFine			
			WHERE 
				IDTrasferimento IN (SELECT ID FROM T_MovTrasferimenti WHERE IDCartella=@uIDCartellaInizio)
			
			UPDATE T_MovSchede
			SET IDEpisodio=@uIDEpisodioFine,
				IDTrasferimento=@uIDTrasferimentoFine
			WHERE 
				CodEntita='APP' AND
				IDTrasferimento IN (SELECT ID FROM T_MovTrasferimenti WHERE IDCartella=@uIDCartellaInizio)
			
				
						UPDATE T_MovCartelleInVisione
			SET CodStatoCartellaInVisione='CA'
			WHERE 
				CodStatoCartellaInVisione <> 'CA' AND
				IDTrasferimento IN (SELECT ID FROM T_MovTrasferimenti WHERE IDCartella=@uIDCartellaFine)
			
			UPDATE T_MovCartelleInVisione
			SET IDEpisodio= @uIDEpisodioFine,
				IDTrasferimento= @uIDTrasferimentoFine			
			WHERE 
				IDTrasferimento IN (SELECT ID FROM T_MovTrasferimenti WHERE IDCartella=@uIDCartellaInizio)
			
							
						UPDATE T_MovDiarioClinico
			SET CodStatoDiario='CA'
			WHERE 
				CodStatoDiario <> 'CA' AND
				IDTrasferimento IN (SELECT ID FROM T_MovTrasferimenti WHERE IDCartella=@uIDCartellaFine)
			
			UPDATE T_MovDiarioClinico
			SET IDEpisodio= @uIDEpisodioFine,
				IDTrasferimento= @uIDTrasferimentoFine			
			WHERE 
				IDTrasferimento IN (SELECT ID FROM T_MovTrasferimenti WHERE IDCartella=@uIDCartellaInizio)
			
			UPDATE T_MovSchede
			SET IDEpisodio=@uIDEpisodioFine,
				IDTrasferimento=@uIDTrasferimentoFine
			WHERE 
				CodEntita='DCL' AND
				IDTrasferimento IN (SELECT ID FROM T_MovTrasferimenti WHERE IDCartella=@uIDCartellaInizio)
						
						UPDATE T_MovNote
			SET CodStatoNota='CA'
			WHERE 
				CodStatoNota <> 'CA' AND
				IDTrasferimento IN (SELECT ID FROM T_MovTrasferimenti WHERE IDCartella=@uIDCartellaFine)
			
			UPDATE T_MovNote
			SET IDEpisodio = @uIDEpisodioFine,
				IDTrasferimento = @uIDTrasferimentoFine			
			WHERE 
				IDTrasferimento IN (SELECT ID FROM T_MovTrasferimenti WHERE IDCartella=@uIDCartellaInizio)
			
									
						UPDATE T_MovParametriVitali
			SET CodStatoParametroVitale='CA'
			WHERE 
				CodStatoParametroVitale <> 'CA' AND
				IDTrasferimento IN (SELECT ID FROM T_MovTrasferimenti WHERE IDCartella=@uIDCartellaFine)
			
			UPDATE T_MovParametriVitali
			SET IDEpisodio = @uIDEpisodioFine,
				IDTrasferimento = @uIDTrasferimentoFine			
			WHERE 
				IDTrasferimento IN (SELECT ID FROM T_MovTrasferimenti WHERE IDCartella=@uIDCartellaInizio)
			
			UPDATE T_MovSchede
			SET IDEpisodio=@uIDEpisodioFine,
				IDTrasferimento=@uIDTrasferimentoFine
			WHERE 
				CodEntita='PVT' AND
				IDTrasferimento IN (SELECT ID FROM T_MovTrasferimenti WHERE IDCartella=@uIDCartellaInizio)
			
							UPDATE T_MovPrescrizioniTempi
			SET CodStatoPrescrizioneTempi='CA'
			WHERE 
				CodStatoPrescrizioneTempi <> 'CA' AND
				IDPrescrizione IN 
					(SELECT ID 
					 FROM T_MovPrescrizioni WHERE
					 IDTrasferimento IN (SELECT ID FROM T_MovTrasferimenti WHERE IDCartella=@uIDCartellaFine)
					 )			 
																
						UPDATE T_MovPrescrizioni
			SET CodStatoPrescrizione='CA'
			WHERE 
				CodStatoPrescrizione <> 'CA' AND
				IDTrasferimento IN (SELECT ID FROM T_MovTrasferimenti WHERE IDCartella=@uIDCartellaFine)
			
			UPDATE T_MovPrescrizioni
			SET IDEpisodio = @uIDEpisodioFine,
				IDTrasferimento = @uIDTrasferimentoFine			
			WHERE 
				IDTrasferimento IN (SELECT ID FROM T_MovTrasferimenti WHERE IDCartella=@uIDCartellaInizio)
			
			UPDATE T_MovSchede
			SET IDEpisodio=@uIDEpisodioFine,
				IDTrasferimento=@uIDTrasferimentoFine
			WHERE 
				CodEntita='PRF' AND
				IDTrasferimento IN (SELECT ID FROM T_MovTrasferimenti WHERE IDCartella=@uIDCartellaInizio)
					
		
				
						DELETE FROM T_MovReport			
			WHERE 				
				IDTrasferimento IN (SELECT ID FROM T_MovTrasferimenti WHERE IDCartella=@uIDCartellaFine)
			
			UPDATE T_MovReport
			SET IDEpisodio = @uIDEpisodioFine,
				IDTrasferimento = @uIDTrasferimentoFine			
			WHERE 
				IDTrasferimento IN (SELECT ID FROM T_MovTrasferimenti WHERE IDCartella=@uIDCartellaInizio)
			
							
						UPDATE T_MovSchede
			SET  CodStatoScheda='CA'
				
			WHERE 
				CodEntita='EPI' AND
				CodStatoScheda <> 'CA' AND
				IDEpisodio IN (SELECT IDEpisodio FROM T_MovTrasferimenti WHERE IDCartella=@uIDCartellaFine)
			
			UPDATE T_MovSchede			
			SET 
				IDEntita=@uIDEpisodioFine,
				IDEpisodio = @uIDEpisodioFine,
				IDTrasferimento = @uIDTrasferimentoFine			
			WHERE 
				CodEntita='EPI' AND
				IDEpisodio IN (SELECT IDEpisodio FROM T_MovTrasferimenti WHERE IDCartella=@uIDCartellaInizio)
																		
						DELETE FROM T_MovSegnalibri			
			WHERE 				
				IDCartella=@uIDCartellaFine			

			UPDATE T_MovSegnalibri
			SET IDCartella=@uIDCartellaFine
			WHERE 
				IDCartella=@uIDCartellaInizio
			
										
						UPDATE T_MovTaskInfermieristici
			SET CodStatoTaskInfermieristico='CA'
			WHERE
				CodStatoTaskInfermieristico <> 'CA' AND
				IDTrasferimento IN (SELECT ID FROM T_MovTrasferimenti WHERE IDCartella=@uIDCartellaFine)
				 
			UPDATE T_MovTaskInfermieristici
			SET IDEpisodio = @uIDEpisodioFine,
				IDTrasferimento = @uIDTrasferimentoFine			
			WHERE 
				IDTrasferimento IN (SELECT ID FROM T_MovTrasferimenti WHERE IDCartella=@uIDCartellaInizio)
			
			UPDATE T_MovSchede
			SET IDEpisodio=@uIDEpisodioFine,
				IDTrasferimento=@uIDTrasferimentoFine
			WHERE 
				CodEntita='WKI' AND
				IDTrasferimento IN (SELECT ID FROM T_MovTrasferimenti WHERE IDCartella=@uIDCartellaInizio)
				
						UPDATE T_MovTrasferimenti
			SET IDCartella=NULL
			WHERE IDCartella=@uIDCartellaInizio
			
						DELETE FROM T_MovRelazioniEntita
			WHERE
				CodEntita='CAR' AND
				CodEntitaCollegata='CAR' AND
				IDEntitaCollegata=@uIDCartellaInizio AND
				IDEntita=@uIDCartellaFine			 
			
					   UPDATE T_MovCartelle
		   SET 
				CodStatoCartella='AP',
				CodUtenteChiusura=NULL,
				DataChiusura=NULL,
				DataChiusuraUTC=NULL,
				PDFCartella=NULL
		   WHERE ID=@uIDCartellaFine
				
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
				
								SET @sInfoTimeStamp='Azione: BO_CartellaScollega'
				SET @sInfoTimeStamp=@sInfoTimeStamp+ '  IDCartellaInizio: ' + CONVERT(VARCHAR(50),@uIDCartellaInizio)  
				SET @sInfoTimeStamp=@sInfoTimeStamp+ '  IDCartellaFine: ' + CONVERT(VARCHAR(50),@uIDCartellaFine) 
				
				SET @xTimeStamp.modify('delete (/TimeStamp/Info)[1]') 		
				SET @xTimeStamp.modify('insert <Info>{sql:variable("@sInfoTimeStamp")}</Info> into (/TimeStamp)[1]')
								
								SET @xTimeStamp.modify('delete (/TimeStamp/CodAzione)[1]') 		
					SET @xTimeStamp.modify('insert <CodAzione>INS</CodAzione> into (/TimeStamp)[1]')
			
				SET @xTimeStamp=CONVERT(XML,
									'<Parametri>' + CONVERT(varchar(max),@xTimeStamp) +
									'</Parametri>')
																		
				EXEC MSP_InsMovTimeStamp @xTimeStamp					
			END 							
	END
				SET @bErrore= ISNULL(@bErrore,0) 
			
	SELECT Errore FROM #tmpErrori	
	
		DROP TABLE #tmpErrori

END