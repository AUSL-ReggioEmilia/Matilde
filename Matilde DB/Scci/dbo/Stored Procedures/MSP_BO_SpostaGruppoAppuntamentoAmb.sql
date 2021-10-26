CREATE PROCEDURE [dbo].[MSP_BO_SpostaGruppoAppuntamentoAmb]
		(@xParametri XML, 
		 @bErrore BIT OUTPUT		
		 )  
AS
BEGIN

	
	
		DECLARE @uIDEntitaInizio AS UNIQUEIDENTIFIER	
	DECLARE @uIDPazienteFine AS UNIQUEIDENTIFIER	
	DECLARE @uIDSessione AS UNIQUEIDENTIFIER
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
	DECLARE @uIDAppuntamento AS UNIQUEIDENTIFIER	
	DECLARE @xTimeStampSingolo AS XML
	DECLARE @bErroreSingolo AS BIT	

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
	SET @bErroreSingolo=0
	

						
	DECLARE @tblErrori AS TABLE
		(
		Errore VARCHAR(500) COLLATE Latin1_General_CI_AS
		)	
	
		SET @sCodRuolo=(SELECT TOP 1 Valore FROM T_Config WHERE ID=681)

				SET @bErrore=0
		
	IF @uIDEntitaInizio IS NULL 
	BEGIN
		SET @bErrore=1
		INSERT INTO @tblErrori
				VALUES ('(GAPPAMB001) IDPaziente NON specificato')
	END
	ELSE
	BEGIN
				SET @uIDPazienteInizio=@uIDEntitaInizio
		SET @uIDEntitaFine=@uIDPazienteFine
						
						SET @nRecord=(SELECT COUNT(*) FROM T_MovAppuntamenti 
					  WHERE IDPaziente=@uIDPazienteInizio AND
					        IDEpisodio IS NULL
					 )					
				IF @nRecord =0
		BEGIN
						SET @bErrore=1
			
			INSERT INTO @tblErrori
				VALUES ('(GAPPAMB002) ERRORE Nessun Appuntamento Ambulatoriale da aggiornare')
		END
		ELSE
		BEGIN	
			IF @uIDPazienteInizio=@uIDPazienteFine
			BEGIN
								SET @bErrore=1
					INSERT INTO @tblErrori
						VALUES('(GAPPAMB003) ERRORE Stai cercando di spostare appuntamenti sullo stesso paziente')	
			END
		
			IF @bErrore=0
			BEGIN
				
								DECLARE cursore CURSOR LOCAL READ_ONLY  FOR 
				SELECT  ID
				FROM T_MovAppuntamenti	WITH (NOLOCK)			
				WHERE IDPaziente=@uIDPazienteInizio AND
					  IDEpisodio IS NULL
																					
				BEGIN TRANSACTION											
					
					OPEN cursore
					
					FETCH NEXT FROM cursore 
					INTO @uIDAppuntamento
					
					SET @bErrore=0
					WHILE (@@FETCH_STATUS = 0 AND @bErrore=0)
					BEGIN								
						SET @xTimeStampSingolo=@xTimeStamp
						
						
						
						
																														SET @xTimeStampSingolo.modify('delete (/IDEntitaInizio)[1]') 
						SET @xTimeStampSingolo.modify('insert <IDEntitaInizio>{sql:variable("@uIDAppuntamento")}</IDEntitaInizio> into (/)[1]')
						
						SET @xTimeStampSingolo.modify('delete (/IDPazienteFine)[1]') 			
						SET @xTimeStampSingolo.modify('insert <IDPazienteFine>{sql:variable("@uIDPazienteFine")}</IDPazienteFine> into (/)[1]')

						SET @xTimeStampSingolo=CONVERT(XML,
										'<Parametri>' + CONVERT(varchar(max),@xTimeStampSingolo) +
										'</Parametri>')						
	
					    						INSERT INTO @tblErrori 
							EXEC MSP_BO_SpostaAppuntamentoAmb @xTimeStampSingolo, @bErroreSingolo  OUTPUT

												IF 	@bErroreSingolo=1
							BEGIN
								SET @bErrore = @bErroreSingolo
								ROLLBACK TRANSACTION
							END

						FETCH NEXT FROM cursore 
						INTO @uIDAppuntamento
					END
					
								IF @bErrore=0 COMMIT TRANSACTION
				
				CLOSE cursore
				DEALLOCATE cursore
					
		  END 		END		END	
		
				SET @bErrore= ISNULL(@bErrore,0) 
		
	IF @bErrore=1 
		INSERT INTO T_TmpBoErrori
		SELECT @uIDSessione,Errore
		FROM @tblErrori
END