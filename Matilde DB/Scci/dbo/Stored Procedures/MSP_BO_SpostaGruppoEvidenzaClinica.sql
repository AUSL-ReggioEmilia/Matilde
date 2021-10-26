CREATE PROCEDURE [dbo].[MSP_BO_SpostaGruppoEvidenzaClinica](@xParametri XML, @bErrore BIT OUTPUT)  
AS
BEGIN


		DECLARE @uIDEpisodioInizio AS UNIQUEIDENTIFIER	
	DECLARE @uIDEpisodioFine AS UNIQUEIDENTIFIER		
	
	DECLARE @xTimeStamp AS XML
	
		DECLARE @sCodRuolo AS VARCHAR(20)
	DECLARE @uIDSessione AS UNIQUEIDENTIFIER
	DECLARE @sGUID AS VARCHAR(50)
	DECLARE @nQTA AS INTEGER
	DECLARE @nRecord AS INTEGER	
	DECLARE @nRecordDestinazione AS INTEGER	
	DECLARE @sInfoTimeStamp AS VARCHAR(255)	
	DECLARE @xTimeStampSingolo AS XML	
	DECLARE @uIDEvidenzaClinica AS UNIQUEIDENTIFIER
	DECLARE @bErroreSingolo AS BIT	
								 			
	IF @xParametri.exist('/Parametri/IDEpisodioInizio')=1	
		BEGIN					 
			SET @sGUID=(SELECT	TOP 1 ValoreParametro.IDEpisodioInizio.value('.','VARCHAR(50)')
						FROM @xParametri.nodes('/Parametri/IDEpisodioInizio') as ValoreParametro(IDEpisodioInizio))
			IF ISNULL(@sGUID,'')<>'' SET @uIDEpisodioInizio=CONVERT(UNIQUEIDENTIFIER,@sGUID)
		END		
	
	IF @xParametri.exist('/Parametri/IDEpisodioFine')=1	
		BEGIN					 
			SET @sGUID=(SELECT	TOP 1 ValoreParametro.IDEpisodioFine.value('.','VARCHAR(50)')
						FROM @xParametri.nodes('/Parametri/IDEpisodioFine') as ValoreParametro(IDEpisodioFine))
			IF ISNULL(@sGUID,'')<>'' SET @uIDEpisodioFine=CONVERT(UNIQUEIDENTIFIER,@sGUID)
		END	
	
	IF @xParametri.exist('/Parametri/IDSessione')=1	
	BEGIN						 
		SET @sGUID=(SELECT	TOP 1 ValoreParametro.IDSessione.value('.','VARCHAR(50)')
					 FROM @xParametri.nodes('/Parametri/IDSessione') as ValoreParametro(IDSessione))
		IF ISNULL(@sGUID,'')<>'' SET @uIDSessione=CONVERT(UNIQUEIDENTIFIER,@sGUID)			 			 						 			
	END
			
				
		IF @xParametri.exist('/Parametri/TimeStamp')=1	
		BEGIN								  				  				  				  	
			SET @xTimeStamp=(SELECT TOP 1 ValoreParametro.TS.query('.')
							  FROM @xParametri.nodes('/Parametri/TimeStamp') as ValoreParametro(TS))	
		END					  						

		SET @bErrore=0
	SET @nRecord=0

						
	DECLARE @tblErrori AS TABLE
		(
		Errore VARCHAR(500) COLLATE Latin1_General_CI_AS
		)	
	
		SET @sCodRuolo=(SELECT TOP 1 Valore FROM T_Config WHERE ID=681)
	
		
				IF @uIDEpisodioInizio IS NULL 
	BEGIN
		SET @bErrore=1
		INSERT INTO @tblErrori(Errore)
			VALUES('(EVC001) IDEpisodioInizio non specificato')
		
	END
	ELSE
	BEGIN							
				SET @nRecord=(SELECT COUNT(*) FROM T_MovEvidenzaClinica
					 WHERE IDEpisodio=@uIDEpisodioInizio 
					 )					
				IF @nRecord =0
		BEGIN
						SET @bErrore=1
			INSERT INTO @tblErrori(Errore)
					VALUES('(EVC002) ERRORE Nessuna Evidenza Clinica da spostare ')	
		END
		ELSE
		BEGIN
			IF @uIDEpisodioFine IS NULL 
			BEGIN
				SET @bErrore=1
				INSERT INTO @tblErrori(Errore)
					VALUES('(EVC003) IDEpisodioFine non specificato')
			END
			ELSE
			BEGIN			
				IF @uIDEpisodioInizio=@uIDEpisodioFine
				BEGIN
										SET @bErrore=1
					INSERT INTO @tblErrori(Errore)
							VALUES('(EVC004) ERRORE IDEpisodioInizio = IDEpisodioFine ')
				END												
				
				IF @bErrore=0
				BEGIN					
								
								
								DECLARE cursore CURSOR LOCAL READ_ONLY  FOR 
					SELECT  ID
					FROM T_MovEvidenzaClinica				
					WHERE 
						IDEpisodio=@uIDEpisodioInizio											
						
				BEGIN TRANSACTION			
				
					OPEN cursore
						
					FETCH NEXT FROM cursore 
					INTO @uIDEvidenzaClinica
					
					SET @bErrore=0
					WHILE (@@FETCH_STATUS = 0 AND @bErrore=0)
					BEGIN								
						SET @xTimeStampSingolo=@xTimeStamp
					
																												
							SET @xTimeStampSingolo.modify('delete (/IDEntitaInizio)[1]') 
							SET @xTimeStampSingolo.modify('insert <IDEntitaInizio>{sql:variable("@uIDEvidenzaClinica")}</IDEntitaInizio> into (/)[1]')
							
							SET @xTimeStampSingolo.modify('delete (/IDEpisodioFine)[1]') 			
							SET @xTimeStampSingolo.modify('insert <IDEpisodioFine>{sql:variable("@uIDEpisodioFine")}</IDEpisodioFine> into (/)[1]')
							

							SET @xTimeStampSingolo=CONVERT(XML,
											'<Parametri>' + CONVERT(varchar(max),@xTimeStampSingolo) +
											'</Parametri>')						
							
					
							INSERT INTO @tblErrori 
								EXEC MSP_BO_SpostaEvidenzaClinica @xTimeStampSingolo, @bErroreSingolo  OUTPUT

														IF 	@bErroreSingolo=1
								BEGIN
									SET @bErrore = @bErroreSingolo
									ROLLBACK TRANSACTION
								END

							FETCH NEXT FROM cursore 
							INTO @uIDEvidenzaClinica
					END
						
										IF @bErrore=0 COMMIT TRANSACTION
					
					CLOSE cursore
					DEALLOCATE cursore
																																																
				END	
		  END 		END		END	

			SET @bErrore= ISNULL(@bErrore,0) 
		
	IF @bErrore=1 
		INSERT INTO T_TmpBoErrori
		SELECT @uIDSessione,Errore
		FROM @tblErrori

	
END