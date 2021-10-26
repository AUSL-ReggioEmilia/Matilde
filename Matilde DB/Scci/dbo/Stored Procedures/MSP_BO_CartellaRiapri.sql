CREATE PROCEDURE [dbo].[MSP_BO_CartellaRiapri](@xParametri XML, @bErrore BIT OUTPUT)  
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
	DECLARE @sCodRuolo AS VARCHAR(20)
	
	DECLARE @sGUID AS VARCHAR(50)
	DECLARE @nQTA AS INTEGER
	DECLARE @nRecord AS INTEGER		
	DECLARE @sInfoTimeStamp AS VARCHAR(255)	
	DECLARE @sTemp AS VARCHAR(500)
		
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
				SET @nRecord=(SELECT COUNT(*) FROM T_MovCartelle
					  WHERE ID=@uIDEntitaInizio AND
							CodStatoCartella='CH'
					 )		
		
				IF @nRecord =0
			BEGIN
								SET @bErrore=1
				INSERT INTO #tmpErrori(Errore)
						VALUES('(AC002) ERRORE Nessuna Cartella Chiusa da riaprire')	
			END
		ELSE
		BEGIN					
						SET @sTemp=(SELECT TOP 1 ISNULL(C.NumeroCartella,'') + ' ID:' + CONVERT(VARCHAR(255),C.ID)
						FROM T_MovRelazioniEntita E
							LEFT JOIN T_MovCartelle C
									ON E.IDEntitaCollegata=C.ID
						 WHERE 
							CodEntita='CAR' AND
							CodEntitaCollegata='CAR' AND
							IDEntita=@uIDEntitaInizio													
						)
			IF @sTemp IS NOT NULL 
				BEGIN
										SET @bErrore=1
					INSERT INTO #tmpErrori(Errore)
						VALUES('(AC003) ERRORE Cartella collegata ad un''altra cartella ' + @sTemp )	
				END
			

		END
	END
	
IF @bErrore=0
	BEGIN											
												
						UPDATE  T_MovCartelle
			SET 
				CodStatoCartella='AP',
				DataChiusura=NULL,
				DataChiusuraUTC=NULL,
				CodUtenteChiusura=NULL
			WHERE ID=@uIDEntitaInizio
			
				
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
				
								SET @sInfoTimeStamp='Azione: BO_CartellaRiapri'
				SET @sInfoTimeStamp=@sInfoTimeStamp+ '  IDCartellaInizio: ' + CONVERT(VARCHAR(50),@uIDCartellaInizio)  
				
				SET @xTimeStamp.modify('delete (/TimeStamp/Info)[1]') 		
				SET @xTimeStamp.modify('insert <Info>{sql:variable("@sInfoTimeStamp")}</Info> into (/TimeStamp)[1]')
								
								SET @xTimeStamp.modify('delete (/TimeStamp/CodAzione)[1]') 		
					SET @xTimeStamp.modify('insert <CodAzione>MOD</CodAzione> into (/TimeStamp)[1]')
			
				SET @xTimeStamp=CONVERT(XML,
									'<Parametri>' + CONVERT(varchar(max),@xTimeStamp) +
									'</Parametri>')
																			
				EXEC MSP_InsMovTimeStamp @xTimeStamp					
			END 	END 				SET @bErrore= ISNULL(@bErrore,0) 
			
	SELECT Errore FROM #tmpErrori	
	
		DROP TABLE #tmpErrori
	
END