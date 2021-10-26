CREATE PROCEDURE [dbo].[MSP_BO_CartellaCancella](@xParametri XML, @bErrore BIT OUTPUT)  
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
	DECLARE @sSQL AS VARCHAR(MAX)			
	DECLARE @xParTmp AS XML
	DECLARE @xTimeStampBase AS XML
		
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
				VALUES('(CC001) IDCartella NON specificata')		
		END
	ELSE
	BEGIN							
		SET @nRecord=(SELECT COUNT(*) FROM T_MovCartelle
					  WHERE ID=@uIDEntitaInizio 
					 )		
					 				
		IF @nRecord =0
		BEGIN			
			SET @bErrore=1
			INSERT INTO #tmpErrori(Errore)
					VALUES('(CC002) ERRORE: Nessuna Cartella da cancellare')	
		END
		
		SET @nRecord=(SELECT COUNT(*) 
							FROM T_MovTaskInfermieristici M
								INNER JOIN T_MovTrasferimenti T
									ON 	M.IDTrasferimento=T.ID										
					  WHERE 
						M.CodStatoTaskInfermieristico NOT IN ('CA') AND
						T.IDCartella=@uIDEntitaInizio 
					 )		
			
		IF @nRecord >0
		BEGIN				
			SET @bErrore=1
			INSERT INTO #tmpErrori(Errore)
					VALUES('(CC003) ERRORE: Esistono ' + CONVERT(varchar(10),@nRecord) + ' Task Infermieristici NON cancellati')	
		END
		
		SET @nRecord=(SELECT COUNT(*) 
							FROM T_MovPrescrizioni M
								INNER JOIN T_MovTrasferimenti T
									ON 	M.IDTrasferimento=T.ID										
					  WHERE 
						M.CodStatoPrescrizione NOT IN ('CA') AND
						T.IDCartella=@uIDEntitaInizio 
					 )		
			
		IF @nRecord >0
		BEGIN				
			SET @bErrore=1
			INSERT INTO #tmpErrori(Errore)
					VALUES('(CC004) ERRORE: Esistono ' + CONVERT(varchar(10),@nRecord) + ' Prescrizioni NON cancellate')	
		END
		
		SET @nRecord=(SELECT COUNT(*) 
							FROM T_MovParametriVitali M
								INNER JOIN T_MovTrasferimenti T
									ON 	M.IDTrasferimento=T.ID										
					  WHERE 
						M.CodStatoParametroVitale NOT IN ('CA') AND
						T.IDCartella=@uIDEntitaInizio 
					 )		
			
		IF @nRecord >0
		BEGIN				
			SET @bErrore=1
			INSERT INTO #tmpErrori(Errore)
					VALUES('(CC005) ERRORE: Esistono ' + CONVERT(varchar(10),@nRecord) + ' Parametri Vitali NON cancellati')	
		END
		
		SET @nRecord=(SELECT COUNT(*) 
							FROM T_MovDiarioClinico M
								INNER JOIN T_MovTrasferimenti T
									ON 	M.IDTrasferimento=T.ID										
					  WHERE 
						M.CodStatoDiario NOT IN ('CA') AND
						T.IDCartella=@uIDEntitaInizio 
					 )		
			
		IF @nRecord >0
		BEGIN				
			SET @bErrore=1
			INSERT INTO #tmpErrori(Errore)
					VALUES('(CC006) ERRORE: Esistono ' + CONVERT(varchar(10),@nRecord) + ' registrazioni di Diario Clinico NON cancellate')	
		END
		
		SET @nRecord=(SELECT COUNT(*) 
							FROM T_MovAppuntamenti M
								INNER JOIN T_MovTrasferimenti T
									ON 	M.IDTrasferimento=T.ID										
					  WHERE 
						M.CodStatoAppuntamento NOT IN ('CA') AND
						T.IDCartella=@uIDEntitaInizio 
					 )		
			
		IF @nRecord >0
		BEGIN				
			SET @bErrore=1
			INSERT INTO #tmpErrori(Errore)
					VALUES('(CC007) ERRORE: Esistono ' + CONVERT(varchar(10),@nRecord) + ' Appuntamenti NON cancellati')	
		END
								
		SET @nRecord=(SELECT COUNT(*) 
							FROM T_MovSchede M
								INNER JOIN T_MovTrasferimenti T
									ON 	M.IDTrasferimento=T.ID										
					  WHERE 
						M.CodStatoScheda NOT IN ('CA') AND	
						M.Storicizzata=0 AND							
						T.IDCartella=@uIDEntitaInizio AND
						M.CodEntita NOT IN ('APP','ALG','ALA')
					 )		
			
		IF @nRecord >0
		BEGIN				
			SET @bErrore=1
			INSERT INTO #tmpErrori(Errore)
					VALUES('(CC008) ERRORE: Esistono ' + CONVERT(varchar(10),@nRecord) + ' Schede NON cancellate')	
		END
		
		SET @nRecord=(SELECT COUNT(*) 
							FROM T_MovSchede M
								INNER JOIN T_MovTrasferimenti T
									ON 	M.IDTrasferimento=T.ID										
					  WHERE 
						M.CodEntita='EPI' AND
						M.CodStatoScheda NOT IN ('CA') AND		
						M.Storicizzata=0 AND					
						T.IDCartella=@uIDEntitaInizio 
					 )		
			
		IF @nRecord >0
		BEGIN				
			SET @bErrore=1
			INSERT INTO #tmpErrori(Errore)
					VALUES('(CC009) ---------> ' + CONVERT(varchar(10),@nRecord) + ' Schede di tipo Episodio NON cancellate')	
		END

		SET @nRecord=(SELECT COUNT(*) 
							FROM T_MovSchede M
								INNER JOIN T_MovTrasferimenti T
									ON 	M.IDTrasferimento=T.ID										
					  WHERE 
						M.CodEntita NOT IN ('APP','EPI','ALG','ALA') AND
						M.CodStatoScheda NOT IN ('CA') AND	
						M.Storicizzata=0 AND						
						T.IDCartella=@uIDEntitaInizio 
					 )		
			
		IF @nRecord >0
		BEGIN				
			SET @bErrore=1
			INSERT INTO #tmpErrori(Errore)
					VALUES('(CC011) ---------> ' + CONVERT(varchar(10),@nRecord) + ' Schede di Altri tipi NON cancellate')	
		END
	
		IF @bErrore=0
		BEGIN						
			UPDATE  T_MovCartelle
			SET 
				NumeroCartella='ERRATO ' + NumeroCartella					
			WHERE ID=@uIDEntitaInizio
					
			SET @xTimeStampBase=@xTimeStamp
			
			SET @xTimeStampBase.modify('delete (/TimeStamp/CodEntita)[1]') 						
			SET @xTimeStampBase.modify('insert <CodEntita>CAR</CodEntita> into (/TimeStamp)[1]')
					
			SET @xTimeStampBase.modify('delete (/TimeStamp/CodAzione)[1]') 						
			SET @xTimeStampBase.modify('insert <CodAzione>CAN</CodAzione> into (/TimeStamp)[1]')
						
			SET @xTimeStampBase.modify('delete (/TimeStamp/IDEntita)[1]') 						
			SET @xTimeStampBase.modify('insert <IDEntita>{sql:variable("@uIDEntitaInizio")}</IDEntita> into (/TimeStamp)[1]')
				
			SET @xParTmp=CONVERT(XML,
										'<Parametri>' + CONVERT(varchar(max),@xTimeStampBase) +
										'</Parametri>')
							
			SET @xParTmp.modify('insert <CodStatoCartella>CA</CodStatoCartella>  as first into (/Parametri)[1]')																					
			SET @xParTmp.modify('insert <IDCartella>{sql:variable("@uIDEntitaInizio")}</IDCartella> as first into (/Parametri)[1]')			
							
																				
			EXEC MSP_AggMovCartelle @xParTmp
						 									 			
			SET @sSQL=''
			
			UPDATE  T_MovTrasferimenti
			SET 
				IDCartella=NULL				
			WHERE IDCartella=@uIDEntitaInizio
				
			DELETE 
			FROM T_MovRelazioniEntita
			WHERE IDEntitaCollegata=@uIDEntitaInizio AND CodEntita='CAR' AND CodEntitaCollegata='CAR'

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
				
				SET @sInfoTimeStamp='Azione: BO_CartellaCancella'
				SET @sInfoTimeStamp=@sInfoTimeStamp+ '  IDCartellaInizio: ' + CONVERT(VARCHAR(50),@uIDCartellaInizio)  
				
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
	END 
		
	SET @bErrore= ISNULL(@bErrore,0) 
				
	SELECT Errore FROM #tmpErrori	
		
	DROP TABLE #tmpErrori
	
END