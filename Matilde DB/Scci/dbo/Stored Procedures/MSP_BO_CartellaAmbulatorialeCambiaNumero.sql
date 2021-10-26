CREATE PROCEDURE [dbo].[MSP_BO_CartellaAmbulatorialeCambiaNumero](@xParametri XML, @bErrore BIT OUTPUT)  
AS
BEGIN


		DECLARE @sCodSchedaAmbulatorialeInizio AS VARCHAR(20)
	DECLARE @sNumeroCartellaAmbulatorialeInizio AS VARCHAR(50) 
	DECLARE @sNumeroCartellaAmbulatorialeFine AS VARCHAR(50) 
	DECLARE @xTimeStamp AS XML
	DECLARE @sAggiornaContatore AS VARCHAR(1) 
			
		DECLARE @uIDCartellaInizio AS UNIQUEIDENTIFIER	
	DECLARE @uIDCartellaFine AS UNIQUEIDENTIFIER	
	DECLARE @sCodRuolo AS VARCHAR(20)	

		DECLARE @nQTA AS INTEGER
	DECLARE @nRecord AS INTEGER		
	DECLARE @sInfoTimeStamp AS VARCHAR(255)	
		
	DECLARE @sCodContatore AS VARCHAR(20)
	DECLARE @sValoreContatore AS VARCHAR(20)
	DECLARE @nValoreContatore AS INTEGER

		IF @xParametri.exist('/Parametri/CodSchedaAmbulatorialeInizio')=1
		BEGIN
			SET @sCodSchedaAmbulatorialeInizio=(SELECT	TOP 1 ValoreParametro.CodSchedaAmbulatorialeInizio.value('.','VARCHAR(20)')
						 FROM @xParametri.nodes('/Parametri/CodSchedaAmbulatorialeInizio') as ValoreParametro(CodSchedaAmbulatorialeInizio))	
		END
	
	IF @xParametri.exist('/Parametri/NumeroCartellaAmbulatorialeInizio')=1	
		BEGIN
			SET @sNumeroCartellaAmbulatorialeInizio=(SELECT	TOP 1 ValoreParametro.NumeroCartellaAmbulatorialeInizio.value('.','VARCHAR(50)')
						 FROM @xParametri.nodes('/Parametri/NumeroCartellaAmbulatorialeInizio') as ValoreParametro(NumeroCartellaAmbulatorialeInizio))	
		END

	IF @xParametri.exist('/Parametri/NumeroCartellaAmbulatorialeFine')=1	
		BEGIN
			SET @sNumeroCartellaAmbulatorialeFine=(SELECT TOP 1 ValoreParametro.NumeroCartellaAmbulatorialeFine.value('.','VARCHAR(50)')
						 FROM @xParametri.nodes('/Parametri/NumeroCartellaAmbulatorialeFine') as ValoreParametro(NumeroCartellaAmbulatorialeFine))	
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
					
	
				IF @sCodSchedaAmbulatorialeInizio IS NOT NULL AND @sNumeroCartellaAmbulatorialeInizio IS NOT NULL
	BEGIN

				SELECT @nQta=COUNT(*),
			   @uIDCartellaInizio=MIN(ID)
		FROM T_MovCartelleAmbulatoriali
		WHERE 
			NumeroCartella=@sNumeroCartellaAmbulatorialeInizio AND 
			ID IN (SELECT IDCartellaAmbulatoriale
				   FROM T_MovSchede
				   WHERE
					CodScheda= @sCodSchedaAmbulatorialeInizio AND
					CodStatoScheda NOT IN ('CA') AND
					Storicizzata=0
				   )	
		
		IF @nQta = 0 
			BEGIN			
				INSERT INTO #tmpErrori(Errore)
						VALUES('(GG001) ERRORE : nessuna cartella ambulatoriale di origine trovata')
				SET @bErrore=1
			END

	END	
				IF @sNumeroCartellaAmbulatorialeFine IS NOT NULL
	BEGIN

				SELECT @nQta=COUNT(*),
			   @uIDCartellaFine=MIN(ID)
		FROM T_MovCartelleAmbulatoriali
		WHERE 
			NumeroCartella=@sNumeroCartellaAmbulatorialeFine AND 
			ID IN (SELECT IDCartellaAmbulatoriale
				   FROM T_MovSchede
				   WHERE
					CodScheda= @sCodSchedaAmbulatorialeInizio AND
					CodStatoScheda NOT IN ('CA') AND
					Storicizzata=0
				   )	
		
		IF @nQta > 0 
			BEGIN			
				INSERT INTO #tmpErrori(Errore)
						VALUES('(GG001) ERRORE : cartella ambulatoriale di destinazione esistente')
				SET @bErrore=1
			END

	END		
	
	IF (@uIDCartellaInizio IS NULL)
	BEGIN
			INSERT INTO #tmpErrori(Errore)
						VALUES('(GG002) ERRORE : cartella ambulatoriale di origine esistente')
				SET @bErrore=1
	END

		SET @sCodRuolo=(SELECT TOP 1 Valore FROM T_Config WHERE ID=681)	


		IF @sAggiornaContatore = '1'
	BEGIN
		
			SET @sCodContatore = (SELECT CodContatore FROM T_Schede WHERE Codice=@sCodSchedaAmbulatorialeInizio)

			SET @sValoreContatore= LEFT(@sNumeroCartellaAmbulatorialeFine, LEN(@sNumeroCartellaAmbulatorialeFine) -
												CHARINDEX('/', REVERSE(@sNumeroCartellaAmbulatorialeFine))) 

			IF @sValoreContatore = '' OR ISNUMERIC(@sValoreContatore) = 0
			BEGIN
				INSERT INTO #tmpErrori(Errore)
						VALUES('(GG003) ERRORE : valore contatore non impostabile')
				SET @bErrore=1
			END
			ELSE
			BEGIN
				SET @nValoreContatore = CONVERT(INTEGER,@sValoreContatore)
			END
	END

					
	IF @bErrore=0
		BEGIN											
	
									IF @sAggiornaContatore = '1'
			BEGIN
				
								UPDATE T_Contatori
				SET Valore=@nValoreContatore +1
				WHERE Codice=@sCodContatore
								
			END
																								
						UPDATE T_MovCartelleAmbulatoriali
			SET 
				NumeroCartella=@sNumeroCartellaAmbulatorialeFine
			WHERE ID=@uIDCartellaInizio			
							
						IF @bErrore=0 
			BEGIN			
								SET @xTimeStamp.modify('delete (/TimeStamp/CodRuolo)[1]') 					
				SET @xTimeStamp.modify('insert <CodRuolo>{sql:variable("@sCodRuolo")}</CodRuolo> into (/TimeStamp)[1]')
				
								SET @xTimeStamp.modify('delete (/TimeStamp/IDEntita)[1]') 					
				SET @xTimeStamp.modify('insert <IDEntita>{sql:variable("@uIDCartellaInizio")}</IDEntita> into (/TimeStamp)[1]')
				
								SET @sInfoTimeStamp='Azione: MSP_BO_CartellaCambiaNumero'
				SET @sInfoTimeStamp=@sInfoTimeStamp+ '  NumeroCartellaAmbulatorialeInizio: ' + @sNumeroCartellaAmbulatorialeInizio
				SET @sInfoTimeStamp=@sInfoTimeStamp+ '  NumeroCartellaAmbulatorialeFine  : ' + @sNumeroCartellaAmbulatorialeFine
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