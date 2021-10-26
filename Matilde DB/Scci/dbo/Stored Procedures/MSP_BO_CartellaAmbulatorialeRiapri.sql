CREATE PROCEDURE [dbo].[MSP_BO_CartellaAmbulatorialeRiapri](@xParametri XML, @bErrore BIT OUTPUT)  
AS
BEGIN


		DECLARE @sCodSchedaAmbulatorialeInizio AS VARCHAR(20)
	DECLARE @sNumeroCartellaAmbulatorialeInizio AS VARCHAR(50) 	
	
	DECLARE @xTimeStamp AS XML
			
		DECLARE @uIDEntitaInizio AS UNIQUEIDENTIFIER	
	DECLARE @uIDCartellaInizio AS UNIQUEIDENTIFIER	
	DECLARE @sCodRuolo AS VARCHAR(20)
	
	DECLARE @sGUID AS VARCHAR(50)
	DECLARE @nQTA AS INTEGER
	DECLARE @nRecord AS INTEGER		
	DECLARE @sInfoTimeStamp AS VARCHAR(255)	
	DECLARE @sTemp AS VARCHAR(500)
		
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
		
		IF @nQta =0 
			BEGIN			
				INSERT INTO #tmpErrori(Errore)
						VALUES('(GG001) ERRORE : nessuna cartella ambulatoriale di origine trovata')
				SET @bErrore=1
			END
		ELSE
			IF @nQta>1 
			BEGIN
				INSERT INTO #tmpErrori(Errore)
					VALUES('(GG002) ERRORE : più di una cartella ambulatoriale di origine trovata')
				SET @bErrore=1				
			END			
	END	
		SET @sCodRuolo=(SELECT TOP 1 Valore FROM T_Config WHERE ID=681)

					
		
	SET @uIDEntitaInizio=@uIDCartellaInizio
	IF @uIDEntitaInizio IS NULL 
		BEGIN
			SET @bErrore=1
			INSERT INTO #tmpErrori(Errore)
				VALUES('(AC001) IDCartellaAmbulatoriale NON specificata')
			
		END
	ELSE
	BEGIN								
				SET @nRecord=(SELECT COUNT(*) FROM T_MovCartelleAmbulatoriali
					  WHERE ID=@uIDEntitaInizio AND
							CodStatoCartella='CH'
					 )		
		
				IF @nRecord =0
			BEGIN
								SET @bErrore=1
				INSERT INTO #tmpErrori(Errore)
						VALUES('(AC002) ERRORE Nessuna Cartella Ambulatoriale Chiusa da riaprire')	
			END		
	END
	
IF @bErrore=0
	BEGIN											
												
						UPDATE  T_MovCartelleAmbulatoriali
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
									
								SET @sInfoTimeStamp='Azione: MBO_CartellaAmbulatorialeRiapri'
				SET @sInfoTimeStamp=@sInfoTimeStamp+ '  IDCartellaAmbulatorialeInizio: ' + CONVERT(VARCHAR(50),@uIDCartellaInizio)  
				
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