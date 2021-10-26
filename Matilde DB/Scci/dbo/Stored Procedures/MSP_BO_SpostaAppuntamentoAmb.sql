
CREATE PROCEDURE [dbo].[MSP_BO_SpostaAppuntamentoAmb](@xParametri XML, @bErrore BIT OUTPUT)  
AS
BEGIN

	
	
		DECLARE @uIDEntitaInizio AS UNIQUEIDENTIFIER	
	DECLARE @uIDPazienteFine AS UNIQUEIDENTIFIER	
	DECLARE @xTimeStamp AS XML
		
		DECLARE @uIDPazienteInizio AS UNIQUEIDENTIFIER						
	DECLARE @uIDEntitaFine AS UNIQUEIDENTIFIER		
	DECLARE @uIDEpisodioInizio AS UNIQUEIDENTIFIER	
	DECLARE @uIDTrasferimentoInizio AS UNIQUEIDENTIFIER
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
	
	IF @xParametri.exist('/Parametri/IDPazienteFine')=1	
		BEGIN						 
			SET @sGUID=(SELECT	TOP 1 ValoreParametro.IDPazienteFine.value('.','VARCHAR(50)')
						 FROM @xParametri.nodes('/Parametri/IDPazienteFine') as ValoreParametro(IDPazienteFine))
			IF ISNULL(@sGUID,'')<>'' SET @uIDPazienteFine=CONVERT(UNIQUEIDENTIFIER,@sGUID)			 			 						 
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
			VALUES('(APPAMB001) IDAppuntamento NON specificato')
		
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
					VALUES('(APPAMB002) ERRORE Nessun Appuntamento da aggiornare')	
		END
		ELSE
		BEGIN	
			IF @uIDPazienteInizio=@uIDPazienteFine
			BEGIN
								SET @bErrore=1
				INSERT INTO #tmpErrori(Errore)
						VALUES('(APPAMB003) ERRORE Appuntamento già associato al paziente di destinazione {' + CONVERT(VARCHAR(50),@uIDEntitaInizio)+ '}')	
			END
			
			IF @bErrore=0
			BEGIN
								SET @nNumTransazioni=@@TRANCOUNT
			
				BEGIN TRANSACTION
			
								UPDATE  T_MovAppuntamenti
				SET IDPaziente=@uIDPazienteFine
				WHERE ID=@uIDEntitaInizio
				
				IF @@ERROR=0
					BEGIN
						
												UPDATE  T_MovSchede
						SET		
														IDPaziente=@uIDPazienteFine
						WHERE IDEntita=@uIDEntitaInizio AND
							  CodEntita='APP'																											
												
						IF @@ERROR=0									
							COMMIT TRANSACTION								
						ELSE
							BEGIN							
								IF @nNumTransazioni=0 ROLLBACK	TRANSACTION
								SET @bErrore=1
								INSERT INTO #tmpErrori(Errore)
									VALUES('(APPAMB004) ERRORE durante aggiornamento della scheda del PAZ {' + CONVERT(VARCHAR(50),@uIDEntitaInizio)+ '}')
							END	
					END		  
				ELSE
					BEGIN					
												IF @nNumTransazioni=0 ROLLBACK TRANSACTION
						SET @bErrore=1
						INSERT INTO #tmpErrori(Errore)
									VALUES('(APPAMB005) ERRORE durante aggiornamento del PAZ {' + CONVERT(VARCHAR(50),@uIDEntitaInizio)+ '}')
					END	
						
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
				
								SET @sInfoTimeStamp='Azione: BO_SpostaAPPAmb'				
				SET @sInfoTimeStamp=@sInfoTimeStamp + ' IDPazienteFine: ' + + CONVERT(VARCHAR(50),@uIDPazienteFine)  									
				SET @xTimeStamp.modify('delete (/TimeStamp/Info)[1]') 		
				SET @xTimeStamp.modify('insert <Info>{sql:variable("@sInfoTimeStamp")}</Info> into (/TimeStamp)[1]')
				
								SET @xTimeStamp.modify('delete (/TimeStamp/CodAzione)[1]') 		
				SET @xTimeStamp.modify('insert <CodAzione>MOD</CodAzione> into (/TimeStamp)[1]')
			
				SET @xTimeStamp=CONVERT(XML,
									'<Parametri>' + CONVERT(varchar(max),@xTimeStamp) +
									'</Parametri>')
									
								EXEC MSP_InsMovTimeStamp @xTimeStamp					
			END 		  END 		END		END			
				SET @bErrore= ISNULL(@bErrore,0) 
			
	SELECT Errore FROM #tmpErrori	
	
		DROP TABLE #tmpErrori
	
END