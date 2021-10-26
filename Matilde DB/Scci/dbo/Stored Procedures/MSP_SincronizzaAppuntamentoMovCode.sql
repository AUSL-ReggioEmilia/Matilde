CREATE PROCEDURE [dbo].[MSP_SincronizzaAppuntamentoMovCode](@xParametri AS XML )
AS
BEGIN

	DECLARE @uIDAppuntamento AS UNIQUEIDENTIFIER
	DECLARE @sCodAzione AS VARCHAR(20)
	DECLARE @sCodLogin AS VARCHAR(100)
	DECLARE @xTimeStamp AS XML
	
	DECLARE @sSQL AS VARCHAR(MAX)
	DECLARE @sWHERE AS VARCHAR(Max)
	DECLARE @sDataTmp AS VARCHAR(20)
	DECLARE @uGUID AS UNIQUEIDENTIFIER
	DECLARE @xTimeStampBase AS XML	
	DECLARE @uID AS UNIQUEIDENTIFIER
	DECLARE @IDPaziente AS UNIQUEIDENTIFIER		
	DECLARE @IDCoda AS UNIQUEIDENTIFIER	
	DECLARE @CodStatoCodaEntita as varchar(MAX)
	DECLARE @CodStatoAppuntamento as varchar(MAX)
	DECLARE @numerorighe AS INTEGER
	DECLARE @dDataAssegnazione AS DaTETIME

						
	CREATE TABLE #tmpAgende
	(
		Codice VARCHAR(20)  COLLATE Latin1_General_CI_AS
	)	


	IF @xParametri.exist('/Parametri/CodAgenda')=1
		INSERT INTO #tmpAgende(Codice)	
			SELECT 	ValoreParametro.CodAgenda.value('.','VARCHAR(20)')	
				FROM @xParametri.nodes('/Parametri/CodAgenda') AS ValoreParametro(CodAgenda)

		SET @uIDAppuntamento= (SELECT TOP 1	ValoreParametro.IDAppuntamento.value('.','UNIQUEIDENTIFIER')	
						   FROM @xParametri.nodes('/Parametri/IDAppuntamento') AS ValoreParametro(IDAppuntamento))
		
		SET @sCodAzione= (SELECT TOP 1	ValoreParametro.CodAzione.value('.','VARCHAR(20)')	
						   FROM @xParametri.nodes('/Parametri/TimeStamp/CodAzione') AS ValoreParametro(CodAzione)) 																

		SET @sCodLogin= (SELECT TOP 1	ValoreParametro.CodLogin.value('.','VARCHAR(100)')	
						   FROM @xParametri.nodes('/Parametri/TimeStamp/CodLogin') AS ValoreParametro(CodLogin)) 	
		SELECT @uIDAppuntamento, @sCodAzione, @sCodLogin
				IF @sCodAzione='INS'
		BEGIN 			

			SET @uID=NULL
			SET @IDPaziente=NULL
			SET @CodStatoCodaEntita = NULL

			SELECT
				 @uID = ID, 
				 @IDPaziente = IDPaziente
			FROM T_MovAppuntamenti 
			WHERE 
				ID = @uIDAppuntamento AND 
				CodStatoAppuntamento = 'PR' AND 
				DataInizio >= Convert(date, getdate()) AND 
				DataFine < Convert(date, DATEADD(day, 1, getdate()))
			
						IF @uID IS NOT NULL
			BEGIN				
				SET @uID = (SELECT TOP 1 
								IDAppuntamento 
							FROM T_MovAppuntamentiAgende AS tmaa 
								INNER JOIN #tmpAgende AS tmpa 
									ON (tmaa.CodAgenda = tmpa.Codice)
							WHERE tmaa.IDAppuntamento = @uID)
				
				IF @uID IS NOT NULL
				BEGIN
					SET @IDCoda = NULL
					 SELECT TOP 1 
						@IDCoda = tmce.IDCoda, 
						@CodStatoCodaEntita = tmce.CodStatoCodaEntita
						FROM T_MovCodeEntita AS tmce
							INNER JOIN T_MovAppuntamenti AS tma 
								ON (tmce.IDEntita = tma.ID)
							INNER JOIN T_MovAppuntamentiAgende AS tmaa
								ON (tmaa.IDAppuntamento = tma.ID)
							INNER JOIN #tmpAgende AS tmpa 
								ON (tmaa.CodAgenda = tmpa.Codice)
						WHERE 
							tmce.CodStatoCodaEntita <> 'CA' AND					
							(tma.IDPaziente = @IDPaziente 
								OR
								 								 tma.IDPaziente IN 
											(SELECT IDPazienteVecchio
											 FROM T_PazientiAlias
											 WHERE 
												IDPaziente IN 
													(SELECT IDPaziente
													 FROM T_PazientiAlias
													 WHERE IDPazienteVecchio=@IDPaziente 
													)
											)
							)
							AND					
							tma.DataInizio >= Convert(date, getdate()) AND 
							tma.DataFine < Convert(date, DATEADD(day, 1, getdate()))
					
					IF @IDCoda IS NOT NULL
					BEGIN						
						INSERT INTO T_MovCodeEntita (ID, IDCoda, CodEntita, IDEntita, CodStatoCodaEntita, CodUtenteInserimento, CodUtenteUltimaModifica, 
							DataChiamata, DataChiamataUTC, DataInserimento, DataInserimentoUTC, DataUltimaModifica, DataUltimaModificaUTC)
						VALUES (NEWID(), @IDCoda, 'APP', @uIDAppuntamento, 'AS', @sCodLogin, @sCodLogin, 
							GETDATE(), GETUTCDATE(), GETDATE(), GETUTCDATE(), GETDATE(), GETUTCDATE())
				
					END					
				END 			END 
						SELECT @uIDAppuntamento, @sCodAzione, @sCodLogin, @uID, @IDPaziente, @IDCoda
			SELECT * FROM #tmpAgende
		END
	IF @sCodAzione='MOD'
		BEGIN 
									
			SET @uID=NULL
			SET @IDPaziente=NULL
			SET @numerorighe = 0
			SET @CodStatoCodaEntita = NULL
			SET @CodStatoAppuntamento = NULL
			
						
			SET @uID = (SELECT TOP 1 
				IDAppuntamento 
			FROM T_MovAppuntamentiAgende AS tmaa 
				INNER JOIN #tmpAgende AS tmpa 
					ON (tmaa.CodAgenda = tmpa.Codice)
			WHERE tmaa.IDAppuntamento = @uIDAppuntamento)  
			
			IF @uID IS NOT NULL 
			BEGIN 					
												
				SET @uID=NULL
				SELECT
						@uID = ID, 
						@IDPaziente = IDPaziente,
						@CodStatoAppuntamento = CodStatoAppuntamento
				FROM T_MovAppuntamenti 
				WHERE 
					ID = @uIDAppuntamento AND 
					DataInizio >= Convert(date, getdate()) AND 
					DataFine < Convert(date, DATEADD(day, 1, getdate()))
				
				IF @uID IS NOT NULL 				
				BEGIN 					

					SET @IDCoda = NULL					

					SELECT TOP 1 
						@IDCoda = IDCoda,
						@CodStatoCodaEntita = CodStatoCodaEntita						
					FROM T_MovCodeEntita 
					WHERE IDEntita = @uIDAppuntamento
						AND CodStatoCodaEntita <> 'CA'
					
					IF @IDCoda IS NOT NULL
					BEGIN  										
						IF (@CodStatoAppuntamento <> 'PR' OR @CodStatoCodaEntita = 'CH') 
						BEGIN
								SET @dDataAssegnazione=(SELECT TOP 1 DataAssegnazione FROM T_MovCode WHERE ID=@IDCoda)
													
														IF @dDataAssegnazione < Convert(date,getdate())							
							BEGIN
										
																SET @numerorighe = (SELECT COUNT(*) 
											FROM T_MovCodeEntita
											WHERE IDCoda = @IDCoda
											AND CodStatoCodaEntita <> 'CA')
						
																UPDATE T_MovCodeEntita 							
								SET CodStatoCodaEntita = 'CA',
									CodUtenteUltimaModifica = @sCodLogin,
									DataUltimaModifica = GETDATE(),
									DataUltimaModificaUTC = GETUTCDATE()
								WHERE IDCoda = @IDCoda AND IDEntita = @uIDAppuntamento
								
																if @numerorighe = 1
								BEGIN
										
							UPDATE T_MovCode
										SET CodStatoCoda = 'CA',
											CodUtenteUltimaModifica = @sCodLogin,
											DataUltimaModifica = GETDATE(),
											DataUltimaModificaUTC = GETUTCDATE()
										WHERE ID = @IDCoda
								END
							END	
						END	
						ELSE
						BEGIN		
																																	
														SET @dDataAssegnazione=(SELECT TOP 1 DataAssegnazione FROM T_MovCode WHERE ID=@IDCoda)

														IF @dDataAssegnazione <= Convert(date,getdate())
							BEGIN
																
																SET @numerorighe = (SELECT COUNT(*) 
											FROM T_MovCodeEntita
											WHERE IDCoda = @IDCoda
											AND CodStatoCodaEntita <> 'CA')
					
																UPDATE T_MovCodeEntita 							
								SET CodStatoCodaEntita = 'CA',
									CodUtenteUltimaModifica = @sCodLogin,
									DataUltimaModifica = GETDATE(),
									DataUltimaModificaUTC = GETUTCDATE()
								WHERE IDCoda = @IDCoda AND IDEntita = @uIDAppuntamento
							
																if @numerorighe = 1
								BEGIN
								
									UPDATE T_MovCode
										SET CodStatoCoda = 'CA',
											CodUtenteUltimaModifica = @sCodLogin,
											DataUltimaModifica = GETDATE(),
											DataUltimaModificaUTC = GETUTCDATE()
										WHERE ID = @IDCoda
								END

								SET @IDCoda=NULL																	
									SELECT TOP 1 
									@IDCoda = tmce.IDCoda, 
									@CodStatoCodaEntita = tmce.CodStatoCodaEntita
									FROM T_MovCodeEntita AS tmce
										INNER JOIN T_MovCode CE
											ON TMCE.IDCoda=CE.ID															
										INNER JOIN T_MovAppuntamenti AS tma 
											ON (tmce.IDEntita = tma.ID)
										INNER JOIN T_MovAppuntamentiAgende AS tmaa
											ON (tmaa.IDAppuntamento = tma.ID)
										INNER JOIN #tmpAgende AS tmpa 
											ON (tmaa.CodAgenda = tmpa.Codice)
									WHERE 
										CE.CodStatoCoda <> 'CA'		AND														
										tmce.CodStatoCodaEntita <> 'CA' AND					
										(tma.IDPaziente = @IDPaziente 
											OR
											 											 tma.IDPaziente IN 
														(SELECT IDPazienteVecchio
														 FROM T_PazientiAlias
														 WHERE 
															IDPaziente IN 
																(SELECT IDPaziente
																 FROM T_PazientiAlias
																 WHERE IDPazienteVecchio=@IDPaziente 
																)
														)
										)
										AND					
										tma.DataInizio >= Convert(date, getdate()) AND 
										tma.DataFine < Convert(date, DATEADD(day, 1, getdate()))
								
								IF @IDCoda IS NOT NULL
								BEGIN
											INSERT INTO T_MovCodeEntita (ID, IDCoda, CodEntita, IDEntita, CodStatoCodaEntita, CodUtenteInserimento, CodUtenteUltimaModifica, 
															  DataChiamata, DataChiamataUTC, DataInserimento, DataInserimentoUTC, DataUltimaModifica, DataUltimaModificaUTC)
											VALUES			(NEWID(), @IDCoda, 'APP', @uIDAppuntamento, 'AS', @sCodLogin, @sCodLogin, 
															GETDATE(), GETUTCDATE(), GETDATE(), GETUTCDATE(), GETDATE(), GETUTCDATE())
								END
							END							
						END
					END				
					ELSE  					
					 BEGIN
						IF(@CodStatoAppuntamento = 'PR')
						BEGIN 						  
						
							SET @IDCoda = NULL
							SELECT TOP 1 
								@IDCoda = tmce.IDCoda, 
								@CodStatoCodaEntita = tmce.CodStatoCodaEntita
								FROM T_MovCodeEntita AS tmce
									INNER JOIN T_MovAppuntamenti AS tma 
										ON (tmce.IDEntita = tma.ID)
									INNER JOIN T_MovAppuntamentiAgende AS tmaa
										ON (tmaa.IDAppuntamento = tma.ID)
									INNER JOIN #tmpAgende AS tmpa 
										ON (tmaa.CodAgenda = tmpa.Codice)
								WHERE 
									tmce.CodStatoCodaEntita <> 'CA' AND					
									(tma.IDPaziente = @IDPaziente 
										OR
										 										 tma.IDPaziente IN 
													(SELECT IDPazienteVecchio
													 FROM T_PazientiAlias
													 WHERE 
														IDPaziente IN 
															(SELECT IDPaziente
															 FROM T_PazientiAlias
															 WHERE IDPazienteVecchio=@IDPaziente 
															)
													)
									)
									AND					
									tma.DataInizio >= Convert(date, getdate()) AND 
									tma.DataFine < Convert(date, DATEADD(day, 1, getdate()))
					
							IF @IDCoda IS NOT NULL
							BEGIN								
								INSERT INTO T_MovCodeEntita (ID, IDCoda, CodEntita, IDEntita, CodStatoCodaEntita, CodUtenteInserimento, CodUtenteUltimaModifica, 
									DataChiamata, DataChiamataUTC, DataInserimento, DataInserimentoUTC, DataUltimaModifica, DataUltimaModificaUTC)
								VALUES (NEWID(), @IDCoda, 'APP', @uIDAppuntamento, 'AS', @sCodLogin, @sCodLogin, 
									GETDATE(), GETUTCDATE(), GETDATE(), GETUTCDATE(), GETDATE(), GETUTCDATE())								
							END
						END
					END			
				END
				ELSE
				BEGIN 									
					SET @IDCoda = NULL
										SELECT TOP 1 
						@IDCoda = IDCoda,
						@CodStatoCodaEntita = CodStatoCodaEntita
					FROM T_MovCodeEntita 
					WHERE IDEntita = @uIDAppuntamento 
						AND CodStatoCodaEntita <> 'CA'
					
					IF @IDCoda IS NOT NULL
					BEGIN  						
						
						SET @numerorighe = (SELECT COUNT(*) 
									FROM T_MovCodeEntita
									WHERE IDCoda = @IDCoda)

						UPDATE T_MovCodeEntita 															
												SET CodStatoCodaEntita = 'CA',
							CodUtenteUltimaModifica = @sCodLogin,
							DataUltimaModifica = GETDATE(),
							DataUltimaModificaUTC = GETUTCDATE()
						WHERE IDCoda = @IDCoda AND IDEntita = @uIDAppuntamento
									
						if @numerorighe = 1
						BEGIN
					
								UPDATE T_MovCode
								SET CodStatoCoda = 'CA',
									CodUtenteUltimaModifica = @sCodLogin,
									DataUltimaModifica = GETDATE(),
									DataUltimaModificaUTC = GETUTCDATE()
								WHERE ID = @IDCoda
						END				
					END					
				END
				
			END
					END
	DROP TABLE #tmpAgende
	RETURN 0
END