
CREATE PROCEDURE [dbo].[MSP_BO_CartellaCambiaNumero](@xParametri XML, @bErrore BIT OUTPUT)  
AS
BEGIN


		DECLARE @sCodUAInizio AS VARCHAR(20)
	DECLARE @sNumeroCartellaInizio AS VARCHAR(50) 
	DECLARE @sNumeroCartellaFine AS VARCHAR(50) 
	DECLARE @xTimeStamp AS XML
	DECLARE @sAggiornaContatore AS VARCHAR(1) 
			
		DECLARE @uIDCartellaInizio AS UNIQUEIDENTIFIER	
	DECLARE @uIDCartellaFine AS UNIQUEIDENTIFIER	
	DECLARE @sCodRuolo AS VARCHAR(20)	

		DECLARE @nQTA AS INTEGER
	DECLARE @nRecord AS INTEGER		
	DECLARE @sInfoTimeStamp AS VARCHAR(255)	
		
	DECLARE @sCodUAContatore AS VARCHAR(20)

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

	IF @xParametri.exist('/Parametri/NumeroCartellaFine')=1	
		BEGIN
			SET @sNumeroCartellaFine=(SELECT TOP 1 ValoreParametro.NumeroCartellaFine.value('.','VARCHAR(50)')
						 FROM @xParametri.nodes('/Parametri/NumeroCartellaFine') as ValoreParametro(NumeroCartellaFine))	
		END

	IF @xParametri.exist('/Parametri/AggiornaContatore')=1	
		BEGIN
			SET @sAggiornaContatore=(SELECT TOP 1 ValoreParametro.AggiornaContatore.value('.','VARCHAR(1)')
						 FROM @xParametri.nodes('/Parametri/AggiornaContatore') as ValoreParametro(AggiornaContatore))	
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
		
		IF @nQta = 0 
			BEGIN			
				INSERT INTO #tmpErrori(Errore)
						VALUES('(GG001) ERRORE : nessuna cartella di origine trovata')
				SET @bErrore=1
			END

	END	
				IF @sNumeroCartellaFine IS NOT NULL
	BEGIN

				SELECT @nQta=COUNT(*)
		FROM T_MovTrasferimenti
		WHERE IDCartella 
			IN (SELECT ID FROM T_MovCartelle
				WHERE 
				NumeroCartella=@sNumeroCartellaFine
				)
			AND T_MovTrasferimenti.CodUA=@sCodUAInizio
		
		IF @nQta > 0 
			BEGIN			
				INSERT INTO #tmpErrori(Errore)
						VALUES('(GG001) ERRORE : cartella di destinazione esistente')
				SET @bErrore=1
			END

	END		
		SET @sCodRuolo=(SELECT TOP 1 Valore FROM T_Config WHERE ID=681)	

				IF @bErrore=0
		BEGIN											

									IF @sAggiornaContatore = '1'
			BEGIN
		
								SET @sCodUAContatore = (SELECT TOP 1	CASE ISNULL(CodUANumerazioneCartella,'')
															WHEN '' THEN Codice
															ELSE CodUANumerazioneCartella
														END
				FROM T_UnitaAtomiche
				WHERE Codice = @sCodUAInizio)

				UPDATE T_UnitaAtomiche 
						SET UltimoNumeroCartella = UltimoNumeroCartella + 1
				WHERE Codice = @sCodUAContatore

			END
													
						UPDATE T_MovCartelle
			SET 
				NumeroCartella=@sNumeroCartellaFine
			WHERE ID=@uIDCartellaInizio			
				
						IF @bErrore=0 
			BEGIN			
								SET @xTimeStamp.modify('delete (/TimeStamp/CodRuolo)[1]') 					
				SET @xTimeStamp.modify('insert <CodRuolo>{sql:variable("@sCodRuolo")}</CodRuolo> into (/TimeStamp)[1]')
				
								SET @sInfoTimeStamp='Azione: MSP_BO_CartellaCambiaNumero'
				SET @sInfoTimeStamp=@sInfoTimeStamp+ '  Numero CartellaInizio: ' + @sNumeroCartellaInizio
				SET @sInfoTimeStamp=@sInfoTimeStamp+ '  Numero CartellaFine  : ' + @sNumeroCartellaFine
				SET @sInfoTimeStamp=@sInfoTimeStamp+ '  Aggiorna Contatore   : ' + @sAggiornaContatore
				
				SET @xTimeStamp.modify('delete (/TimeStamp/Info)[1]') 		
				SET @xTimeStamp.modify('insert <Info>{sql:variable("@sInfoTimeStamp")}</Info> into (/TimeStamp)[1]')
								
								SET @xTimeStamp.modify('delete (/TimeStamp/CodAzione)[1]') 		
					SET @xTimeStamp.modify('insert <CodAzione>MOD</CodAzione> into (/TimeStamp)[1]')
			
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