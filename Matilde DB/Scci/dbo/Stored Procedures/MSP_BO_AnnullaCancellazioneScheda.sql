CREATE PROCEDURE [dbo].[MSP_BO_AnnullaCancellazioneScheda](@xParametri XML, @bErrore BIT OUTPUT)  
AS
BEGIN


		DECLARE @uIDEntitaInizio AS UNIQUEIDENTIFIER	
	DECLARE @xTimeStamp AS XML
	
		DECLARE @uIDEpisodioInizio AS UNIQUEIDENTIFIER	
	DECLARE	@uIDPazienteInizio AS UNIQUEIDENTIFIER	
	DECLARE	@uIDTrasferimentoInizio AS UNIQUEIDENTIFIER	
	
	DECLARE @sCodRuolo AS VARCHAR(20)		
	
	DECLARE @sGUID AS VARCHAR(50)
	DECLARE @nQTA AS INTEGER
	DECLARE @nRecord AS INTEGER			
	DECLARE @sInfoTimeStamp AS VARCHAR(255)	
	DECLARE @sSQL AS VARCHAR(MAX)			
	DECLARE @xParTmp AS XML
	DECLARE @xTimeStampBase AS XML
	DECLARE @sIDSchedeFiglie AS VARCHAR(1800)	
	DECLARE @uIDSchedaStoricizzata AS UNIQUEIDENTIFIER
		DECLARE @uIDScheda AS UNIQUEIDENTIFIER
	DECLARE @uIDEntita AS UNIQUEIDENTIFIER
	DECLARE @sCodEntitaScheda AS VARCHAR(20)
	DECLARE @sCodScheda AS VARCHAR(20)	
	DECLARE @uIDSchedaPadre AS UNIQUEIDENTIFIER
	DECLARE @nNumero AS INTEGER
	
	DECLARE @nMaxIDNumStoricizzata AS INTEGER
	DECLARE @sCodUtenteUltimaModificaStoricizzazta AS VARCHAR(100)
	DECLARE @dDataUltimaModificaStoricizzata AS DATETIME
	DECLARE @dDataUltimaModificaUTCStoricizzata AS DATETIME
	DECLARE @uIDPadreStoricizzato AS UNIQUEIDENTIFIER
		
		IF @xParametri.exist('/Parametri/IDEntitaInizio')=1	
		BEGIN					 
			SET @sGUID=(SELECT	TOP 1 ValoreParametro.IDEntitaInizio.value('.','VARCHAR(50)')
						FROM @xParametri.nodes('/Parametri/IDEntitaInizio') as ValoreParametro(IDEntitaInizio))
			IF ISNULL(@sGUID,'')<>'' SET @uIDEntitaInizio=CONVERT(UNIQUEIDENTIFIER,@sGUID)
		END

			SET @sIDSchedeFiglie=''
	SELECT	@sIDSchedeFiglie =  @sIDSchedeFiglie +
														CASE 
								WHEN @sIDSchedeFiglie='' THEN ''
								ELSE  ','
							END + ''''
						  + ValoreParametro.IDSchedeFiglie.value('.','VARCHAR(1800)')
						  + ''''
						 FROM @xParametri.nodes('/Parametri/IDSchedeFiglie') as ValoreParametro(IDSchedeFiglie)

		IF @xParametri.exist('/Parametri/TimeStamp')=1	
		BEGIN								  				  				  				  	
			SET @xTimeStamp=(SELECT TOP 1 ValoreParametro.TS.query('.')
							  FROM @xParametri.nodes('/Parametri/TimeStamp') as ValoreParametro(TS))	

			SET @xTimeStampBase = @xTimeStamp
		END					  						

		SET @bErrore=0
	SET @nRecord=0

			
	CREATE TABLE #tmpErrori
		(		
			Errore VARCHAR(500) COLLATE Latin1_General_CI_AS
		)


	CREATE TABLE #tmpSchede
		(		
			IDScheda UNIQUEIDENTIFIER
		)
					
	IF @uIDEntitaInizio IS NULL
	BEGIN
		INSERT INTO #tmpErrori(Errore)
				VALUES('(CC001) ERRORE : IDScheda non valorizzato')
		SET @bErrore=1
	END
	ELSE
		BEGIN		
						INSERT INTO #tmpSchede(IDScheda)
			SELECT @uIDEntitaInizio

						IF ISNULL(@sIDSchedeFiglie,'') <> ''
			BEGIN
				SET @sSQL = ''
				SET @sSQL = 'INSERT INTO #tmpSchede(IDScheda)
								SELECT ID FROM T_MovSchede
								WHERE ID IN ( ' + @sIDSchedeFiglie + ')'
			END

			EXEC (@sSQL)

						SELECT 
				  @nRecord=COUNT(*)
			FROM T_MovSchede
			WHERE 
				ID=@uIDEntitaInizio	AND
				Storicizzata=0 AND
				CodStatoScheda = 'CA' 
				
			IF @nRecord =0 
				BEGIN			
					INSERT INTO #tmpErrori(Errore)
							VALUES('(CC001) ERRORE : ID passato non è una scheda cancellata.')
					SET @bErrore=1
				END	
			
						SELECT 
				  @nRecord=COUNT(*)
			FROM T_MovSchede
			WHERE 
				ID=@uIDEntitaInizio	AND
				Storicizzata=0 AND
				CodEntita IN ('PAZ','EPI')

			IF @nRecord =0 
				BEGIN			
					INSERT INTO #tmpErrori(Errore)
							VALUES('(CC002) ERRORE : Scheda non ti tipo PAZ o EPI.')
					SET @bErrore=1
				END

						SET @uIDSchedaPadre=(SELECT IDSchedaPadre FROM T_MovSchede WHERE ID=@uIDEntitaInizio)
			IF @uIDSchedaPadre IS NOT NULL
			BEGIN
					SELECT 
						  @nRecord=COUNT(*)
					FROM T_MovSchede
					WHERE 
						ID=@uIDSchedaPadre AND CodStatoScheda='CA'

					IF @nRecord > 0
					BEGIN
						INSERT INTO #tmpErrori(Errore)
							VALUES('(CC003) ERRORE : Scheda con un padre in stato cancellato, recuperare la scheda padre.')
						SET @bErrore=1
					END					
			END

						IF (@bErrore=0)
			BEGIN				
				SET @nRecord=0
				SELECT 
						 @nRecord=COUNT(*)
						 					FROM #tmpSchede TMP
						INNER JOIN T_MovSchede MS
							ON TMP.IDScheda=MS.ID
						LEFT JOIN T_MovSchede MSP
							ON MS.IDSchedaPadre=MSP.ID
				WHERE 
					MS.IDSchedaPadre IS NOT NULL AND 
					MS.IDSchedaPadre NOT IN (SELECT IDScheda FROM #tmpSchede) AND
					MSP.Storicizzata=0 AND
					MSP.CodStatoScheda IN ('CA')				

			IF @nRecord > 0
					BEGIN
						INSERT INTO #tmpErrori(Errore)
							VALUES('(CC004) ERRORE : Selezione schede figlio errata, selezionare anche la scheda padre corrispondente')
						SET @bErrore=1
					END

			END	
		END	
		SET @sCodRuolo=(SELECT TOP 1 Valore FROM T_Config WHERE ID=681)

			IF @bErrore=0
		BEGIN					
			
			SELECT 
				@uIDEpisodioInizio=IDEpisodio,
				@uIDPazienteInizio=IDPaziente,
				@uIDTrasferimentoInizio=IDTrasferimento
			FROM T_MovSchede
			WHERE ID=@uIDEntitaInizio

			
						DECLARE SchCur CURSOR FOR   
			SELECT IDScheda
			FROM #tmpSchede
			 
  
			OPEN SchCur  
  
			FETCH NEXT FROM SchCur
			INTO @uIDScheda
  
			WHILE @@FETCH_STATUS = 0  
			BEGIN  
				SET @uIDSchedaPadre=NULL
				
								SELECT 	@uIDEntita=IDEntita,
						@sCodEntitaScheda = CodEntita,
						@sCodScheda=CodScheda,			
						@nNumero=Numero,
						@uIDSchedaPadre=IDSchedaPadre
				FROM T_MovSchede
				WHERE ID=@uIDScheda

								SET @nMaxIDNumStoricizzata =(SELECT MAX(IDNuM) FROM T_MovSchede
							  WHERE IDEntita=@uIDEntita AND
									CodEntita=@sCodEntitaScheda AND
									Numero=@nNumero AND
									CodStatoScheda='CA' AND								
									Storicizzata=1 AND 
									1 = CASE 
											WHEN @uIDSchedaPadre IS NULL THEN 1
											WHEN ISNULL(@uIDSchedaPadre,IDSchedaPadre) = IDSchedaPadre THEN 1
											ELSE 0
									END)

				SELECT 
						@uIDSchedaStoricizzata=ID,
						@sCodUtenteUltimaModificaStoricizzazta= CodUtenteUltimaModifica,
						@dDataUltimaModificaStoricizzata=DataUltimaModifica,
						@dDataUltimaModificaUTCStoricizzata=DataUltimaModificaUTC,
						@uIDPadreStoricizzato=IDSchedaPadre
				FROM 
						T_MovSchede
				 WHERE IDNum=@nMaxIDNumStoricizzata

								UPDATE T_MovSchede			
				SET 
					CodStatoScheda='IC',
					CodUtenteUltimaModifica = ISNULL(@sCodUtenteUltimaModificaStoricizzazta,CodUtenteUltimaModifica),
					DataUltimaModifica = ISNULL(@dDataUltimaModificaStoricizzata,DataUltimaModifica),
					DataUltimaModificaUTC = ISNULL(@dDataUltimaModificaUTCStoricizzata,DataUltimaModificaUTC)
				WHERE ID=@uIDScheda 

				
								IF (@uIDSchedaPadre IS NULL)
				BEGIN
										UPDATE 
						T_MovSchede
					SET 
						CodStatoScheda='IC'					
					WHERE ID IN (SELECT S.ID
								 FROM 
									T_MovSchede S
										INNER JOIN (SELECT *
													FROM T_MovSchede
													WHERE ID=@uIDScheda) AS Q

										ON (S.CodEntita=Q.CodEntita AND
											S.IDEntita=Q.IDEntita AND										
											S.CodScheda=Q.CodScheda AND
											S.Versione= Q.Versione AND
											S.Numero=Q.Numero)	
								   )																		   	
						AND 
						ID <> @uIDScheda AND								Storicizzata=1 AND									CodStatoScheda='CA'							END
				ELSE
				BEGIN
										UPDATE 
						T_MovSchede
					SET 
						CodStatoScheda='IC'					
					WHERE ID IN (SELECT S.ID
								 FROM 
									T_MovSchede S
										INNER JOIN (SELECT *
													FROM T_MovSchede
													WHERE ID=@uIDScheda) AS Q

										ON (S.CodEntita=Q.CodEntita AND
											S.IDEntita=Q.IDEntita AND										
											S.CodScheda=Q.CodScheda AND
											S.Versione= Q.Versione AND
											S.Numero=Q.Numero)	
								   )																		   	
						AND 
						ID <> @uIDScheda AND								Storicizzata=1 AND									CodStatoScheda='CA'	AND
						IDSchedaPadre = @uIDSchedaPadre	
				END								
								SET @xTimeStamp=@xTimeStampBase

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
				
								SET @sInfoTimeStamp='Azione: BO_AnnullaCancellazioneScheda'
				SET @sInfoTimeStamp=@sInfoTimeStamp+ '  IDScheda: ' + CONVERT(VARCHAR(50),@uIDScheda)  

											
				SET @xTimeStamp.modify('delete (/TimeStamp/Info)[1]') 		
				SET @xTimeStamp.modify('insert <Info>{sql:variable("@sInfoTimeStamp")}</Info> into (/TimeStamp)[1]')
				
								
								SET @xTimeStamp.modify('delete (/TimeStamp/CodAzione)[1]') 		
				SET @xTimeStamp.modify('insert <CodAzione>MOD</CodAzione> into (/TimeStamp)[1]')
			
				SET @xTimeStamp=CONVERT(XML,
									'<Parametri>' + CONVERT(varchar(max),@xTimeStamp) +
									'</Parametri>')
									
								EXEC MSP_InsMovTimeStamp @xTimeStamp	
				
				FETCH NEXT FROM SchCur
				INTO @uIDScheda	
			END	
			
		END			
				SET @bErrore= ISNULL(@bErrore,0) 
				
	SELECT Errore FROM #tmpErrori	
	
		DROP TABLE #tmpErrori
	DROP TABLE #tmpSchede
	
END