CREATE PROCEDURE [dbo].[MSP_SelSchedaDaFiller](@xParametri XML)
AS
BEGIN
	


				
	DECLARE @sCodScheda AS VARCHAR(20)
	DECLARE @uIDPaziente AS UNIQUEIDENTIFIER	
	DECLARE @uIDEpisodio AS UNIQUEIDENTIFIER	
	DECLARE @uIDTrasferimento AS UNIQUEIDENTIFIER	
	DECLARE @sTipoRicerca AS VARCHAR(2)
	DECLARE @sCodEntita AS VARCHAR(20)

	
		DECLARE @sGUID AS VARCHAR(Max)
	DECLARE @uIDScheda AS UNIQUEIDENTIFIER
	DECLARE @uIDEntitaRiferimento AS UNIQUEIDENTIFIER	
	
		SET @sCodScheda=(SELECT TOP 1 ValoreParametro.CodScheda.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodScheda') as ValoreParametro(CodScheda))		

		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDPaziente.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDPaziente') as ValoreParametro(IDPaziente))
	IF 	ISNULL(@sGUID,'') <> '' SET @uIDPaziente=CONVERT(UNIQUEIDENTIFIER,	@sGUID)		
	
		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDEpisodio.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDEpisodio') as ValoreParametro(IDEpisodio))
	IF 	ISNULL(@sGUID,'') <> '' SET @uIDEpisodio=CONVERT(UNIQUEIDENTIFIER,	@sGUID)
	
		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDTrasferimento.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDTrasferimento') as ValoreParametro(IDTrasferimento))
	IF 	ISNULL(@sGUID,'') <> '' SET @uIDTrasferimento=CONVERT(UNIQUEIDENTIFIER,	@sGUID)		
	
		
		SET @sTipoRicerca=(SELECT TOP 1 ValoreParametro.TipoRicerca.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/TipoRicerca') as ValoreParametro(TipoRicerca))	
	SET @sTipoRicerca=ISNULL(@sTipoRicerca,'')	
	IF  LTRIM(@sTipoRicerca)='' SET @sTipoRicerca='U' 		
	
	
		SET @sCodEntita=(SELECT TOP 1 ValoreParametro.CodEntita.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodEntita') as ValoreParametro(CodEntita))			 		
																
	 
 		
									
				
								
				
	
	IF ISNULL(@sCodEntita,'')=''
		BEGIN
						SET @sCodEntita=(SELECT ISNULL(CodEntita,ISNULL(CodEntita2,CodEntita3)) FROM T_Schede WHERE Codice=@sCodScheda)			
		END



CREATE TABLE #tmpPazienti
(
	IDPaziente UNIQUEIDENTIFIER
)

IF @uIDPaziente IS NOT NULL
	BEGIN
				INSERT INTO #tmpPazienti(IDPaziente)
		VALUES (@uIDPaziente)
					
		INSERT INTO #tmpPazienti(IDPaziente)
			SELECT IDPazienteVecchio
				FROM T_PazientiAlias
				WHERE 
				IDPaziente IN 
					(SELECT IDPaziente
						FROM T_PazientiAlias
						WHERE IDPazienteVecchio=@uIDPaziente
					)
				CREATE INDEX IX_IDPaziente ON #tmpPazienti (IDPaziente)								
	END			

IF 	@sTipoRicerca='U'						BEGIN
							
		
		IF @sCodEntita IN ('EPI','PAZ','SCH')	
			BEGIN				
				SET @uIDScheda=(SELECT TOP 1 ID
								FROM T_MovSchede
								WHERE 
										IDPaziente IN (SELECT IDPaziente FROM #tmpPazienti) AND
										CodStatoScheda <> 'CA' AND
										Storicizzata=0 AND 
										CodScheda=@sCodScheda 
																		ORDER BY DataCreazione DESC		
								)	  
			END
		ELSE	
			BEGIN				
				IF @sCodEntita IN ('PVT')
					BEGIN
						SET @uIDEntitaRiferimento=
								(SELECT TOP 1 ID
								FROM T_MovParametriVitali M
								WHERE M.CodTipoParametroVitale
																			IN (SELECT Codice	
											FROM T_TipoParametroVitale
											WHERE CodScheda=@sCodScheda
											)
																		AND IDEpisodio IN 
										(SELECT IDEpisodio
										 FROM T_MovPazienti
										 WHERE IDPaziente IN (SELECT IDPaziente FROM #tmpPazienti)
										 )
								AND M.CodStatoParametroVitale NOT IN ('CA','AN')									ORDER BY M.DataEvento DESC			
								)
					END

								IF @sCodEntita IN ('WKI')				
					BEGIN	
						SET @uIDEntitaRiferimento=
								(SELECT TOP 1 ID
								FROM T_MovTaskInfermieristici M
								WHERE M.CodTipoTaskInfermieristico
																			IN (SELECT Codice	
											FROM T_TipoTaskInfermieristico
											WHERE CodScheda=@sCodScheda
											)
																		AND M.IDEpisodio IN 
										(SELECT IDEpisodio
										 FROM T_MovPazienti
										 WHERE IDPaziente IN (SELECT IDPaziente FROM #tmpPazienti)
										 )
									AND M.CodStatoTaskInfermieristico NOT IN ('CA','AN')									ORDER BY ISNULL(M.DataErogazione,M.DataProgrammata)  DESC			
								)
					END	

								IF @sCodEntita IN ('DCL')				
					BEGIN	
						SET @uIDEntitaRiferimento=
								(SELECT TOP 1 ID
								FROM T_MovDiarioClinico M
								WHERE CodTipoVoceDiario
																			IN (SELECT Codice	
											FROM T_TipoVoceDiario
											WHERE CodScheda=@sCodScheda
											)
																		AND M.IDEpisodio IN 
										(SELECT IDEpisodio
										 FROM T_MovPazienti
										 WHERE IDPaziente IN (SELECT IDPaziente FROM #tmpPazienti)
										 )
									AND M.CodStatoDiario='VA'												ORDER BY DataEvento DESC			
								)
					END	

								IF @sCodEntita IN ('ALA')				
					BEGIN	
						SET @uIDEntitaRiferimento=
								(SELECT TOP 1 ID
								FROM T_MovAlertAllergieAnamnesi M
								WHERE M.CodTipoAlertAllergiaAnamnesi
																			IN (SELECT Codice	
											FROM T_TipoAlertAllergiaAnamnesi
											WHERE CodScheda=@sCodScheda
											)
																		AND IDPaziente IN (SELECT IDPaziente FROM #tmpPazienti)								 
									AND CodStatoAlertAllergiaAnamnesi NOT IN ('CA','AN')									ORDER BY M.DataEvento DESC			
								)
					END		

								IF @sCodEntita IN ('ALG')				
					BEGIN	
							SET @uIDEntitaRiferimento=
								(SELECT TOP 1 ID
								FROM T_MovAlertGenerici M
								WHERE M.CodTipoAlertGenerico
																			IN (SELECT Codice	
											FROM T_TipoAlertGenerico
											WHERE CodScheda=@sCodScheda
											)
																		AND M.IDEpisodio IN 
										(SELECT IDEpisodio
										 FROM T_MovPazienti
										 WHERE IDPaziente IN (SELECT IDPaziente FROM #tmpPazienti)
										 )
									AND CodStatoAlertGenerico NOT IN ('CA','AN')									ORDER BY M.DataEvento DESC			
								)
					END		

								IF @sCodEntita IN ('APP')				
					BEGIN	
						SET @uIDEntitaRiferimento=
								(SELECT TOP 1 ID
								FROM T_MovAppuntamenti M
								WHERE M.CodTipoAppuntamento
																			IN (SELECT Codice	
											FROM T_TipoAppuntamento
											WHERE CodScheda=@sCodScheda
											)
																		AND M.IDPaziente IN (SELECT IDPaziente FROM #tmpPazienti)
									AND M.CodStatoAppuntamento NOT IN ('CA','AN')									ORDER BY M.DataInizio DESC			
								)
					END		
				
				IF @sCodEntita IN ('PRF')				
					BEGIN	
						SET @uIDEntitaRiferimento=
								(SELECT TOP 1 ID
								FROM T_MovPrescrizioni M
								WHERE M.CodTipoPrescrizione
																			IN (SELECT Codice	
											FROM T_TipoPrescrizione
											WHERE CodScheda=@sCodScheda
											)								
																		AND M.IDEpisodio IN 
										(SELECT IDEpisodio
										 FROM T_MovPazienti
										 WHERE IDPaziente IN (SELECT IDPaziente FROM #tmpPazienti)
										 )
									AND M.CodStatoPrescrizione NOT IN ('CA','AN')									ORDER BY M.DataEvento DESC			
								)
					END		

					IF @sCodEntita IN ('PRT')				
					BEGIN	
						SET @uIDEntitaRiferimento=
								(SELECT TOP 1 MT.ID
								FROM T_MovPrescrizioniTempi MT
										INNER JOIN T_MovPrescrizioni M
											ON MT.IDPrescrizione=M.ID
								WHERE M.CodTipoPrescrizione
																			IN (SELECT Codice	
											FROM T_TipoPrescrizione
											WHERE CodSchedaPosologia=@sCodScheda
											)								
																		AND M.IDEpisodio IN 
										(SELECT IDEpisodio
										 FROM T_MovPazienti
										 WHERE IDPaziente IN (SELECT IDPaziente FROM #tmpPazienti)
										 )
									AND M.CodStatoPrescrizione NOT IN ('CA','AN')											AND MT.CodStatoPrescrizioneTempi NOT IN ('CA','AN')									ORDER BY MT.DataEvento DESC			
								)
					END	
							
																SET @uIDScheda=	(SELECT TOP 1 ID
								FROM T_MovSchede
								WHERE 									
										CodStatoScheda <> 'CA' AND
										Storicizzata=0 AND 
										CodScheda=@sCodScheda  AND
										CodEntita=@sCodEntita AND
										IDEntita=@uIDEntitaRiferimento)																										
									
			END
	END 	ELSE 	BEGIN 
								IF 	@sTipoRicerca IN ('P','1')						BEGIN
				

				IF @sCodEntita IN ('EPI','PAZ','SCH')	
					BEGIN
					
						SET @uIDScheda=(SELECT TOP 1 ID
										FROM T_MovSchede
										WHERE 
												IDPaziente IN (SELECT IDPaziente FROM #tmpPazienti) AND
												CodStatoScheda <> 'CA' AND
												Storicizzata=0 AND 
												CodScheda=@sCodScheda 
																						ORDER BY DataCreazione ASC		
										)	  
					END
				ELSE	
					BEGIN
					
						IF @sCodEntita IN ('PVT')
							BEGIN
					
																SET @uIDEntitaRiferimento=
										(SELECT TOP 1 ID
										FROM T_MovParametriVitali M
										WHERE M.CodTipoParametroVitale
																							IN (SELECT Codice	
													FROM T_TipoParametroVitale
													WHERE CodScheda=@sCodScheda
													)
																						AND IDEpisodio IN 
												(SELECT IDEpisodio
												 FROM T_MovPazienti
												 WHERE IDPaziente IN (SELECT IDPaziente FROM #tmpPazienti)
												 )
										AND M.CodStatoParametroVitale NOT IN ('CA','AN')											ORDER BY M.DataEvento ASC			
										)
							END
						IF @sCodEntita IN ('WKI')				
							BEGIN									

																SET @uIDEntitaRiferimento=
										(SELECT TOP 1 ID
										FROM T_MovTaskInfermieristici M
										WHERE M.CodTipoTaskInfermieristico
																							IN (SELECT Codice	
													FROM T_TipoTaskInfermieristico
													WHERE CodScheda=@sCodScheda
													)
																						AND M.IDEpisodio IN 
												(SELECT IDEpisodio
												 FROM T_MovPazienti
												 WHERE IDPaziente IN (SELECT IDPaziente FROM #tmpPazienti)
												 )
											AND M.CodStatoTaskInfermieristico NOT IN ('CA','AN')											ORDER BY ISNULL(M.DataErogazione,M.DataProgrammata)  ASC			
										)
							END	

						IF @sCodEntita IN ('DCL')				
							BEGIN	
								

																SET @uIDEntitaRiferimento=
										(SELECT TOP 1 ID
										FROM T_MovDiarioClinico M
										WHERE CodTipoVoceDiario
																							IN (SELECT Codice	
													FROM T_TipoVoceDiario
													WHERE CodScheda=@sCodScheda
													)
																						AND M.IDEpisodio IN 
												(SELECT IDEpisodio
												 FROM T_MovPazienti
												 WHERE IDPaziente IN (SELECT IDPaziente FROM #tmpPazienti)
												 )
											AND M.CodStatoDiario='VA'														ORDER BY DataEvento ASC			
										)
							END	

						IF @sCodEntita IN ('ALA')				
							BEGIN	
								

																SET @uIDEntitaRiferimento=
										(SELECT TOP 1 ID
										FROM T_MovAlertAllergieAnamnesi M
										WHERE M.CodTipoAlertAllergiaAnamnesi
																							IN (SELECT Codice	
													FROM T_TipoAlertAllergiaAnamnesi
													WHERE CodScheda=@sCodScheda
													)
																						AND IDPaziente IN (SELECT IDPaziente FROM #tmpPazienti)								 
											AND CodStatoAlertAllergiaAnamnesi NOT IN ('CA','AN')											ORDER BY M.DataEvento ASC			
										)
							END		

						IF @sCodEntita IN ('ALG')				
							BEGIN	
								

																SET @uIDEntitaRiferimento=
										(SELECT TOP 1 ID
										FROM T_MovAlertGenerici M
										WHERE M.CodTipoAlertGenerico
																							IN (SELECT Codice	
													FROM T_TipoAlertGenerico
													WHERE CodScheda=@sCodScheda
													)
																						AND M.IDEpisodio IN 
												(SELECT IDEpisodio
												 FROM T_MovPazienti
												 WHERE IDPaziente IN (SELECT IDPaziente FROM #tmpPazienti)
												 )
											AND CodStatoAlertGenerico NOT IN ('CA','AN')											ORDER BY M.DataEvento ASC			
										)
							END		

						IF @sCodEntita IN ('APP')				
							BEGIN	
								

																SET @uIDEntitaRiferimento=
										(SELECT TOP 1 ID
										FROM T_MovAppuntamenti M
										WHERE M.CodTipoAppuntamento
																							IN (SELECT Codice	
													FROM T_TipoAppuntamento
													WHERE CodScheda=@sCodScheda
													)
																						AND M.IDPaziente IN (SELECT IDPaziente FROM #tmpPazienti)
											AND M.CodStatoAppuntamento NOT IN ('CA','AN')											ORDER BY M.DataInizio ASC			
										)
							END		
				
						IF @sCodEntita IN ('PRF')				
							BEGIN	
								

																SET @uIDEntitaRiferimento=
										(SELECT TOP 1 ID
										FROM T_MovPrescrizioni M
										WHERE M.CodTipoPrescrizione
																							IN (SELECT Codice	
													FROM T_TipoPrescrizione
													WHERE CodScheda=@sCodScheda
													)								
																						AND M.IDEpisodio IN 
												(SELECT IDEpisodio
												 FROM T_MovPazienti
												 WHERE IDPaziente IN (SELECT IDPaziente FROM #tmpPazienti)
												 )
											AND M.CodStatoPrescrizione NOT IN ('CA','AN')											ORDER BY M.DataEvento ASC			
										)
							END		

							IF @sCodEntita IN ('PRT')				
							BEGIN	
								

																SET @uIDEntitaRiferimento=
										(SELECT TOP 1 MT.ID
										FROM T_MovPrescrizioniTempi MT
												INNER JOIN T_MovPrescrizioni M
													ON MT.IDPrescrizione=M.ID
										WHERE M.CodTipoPrescrizione
																							IN (SELECT Codice	
													FROM T_TipoPrescrizione
													WHERE CodSchedaPosologia=@sCodScheda
													)								
																						AND M.IDEpisodio IN 
												(SELECT IDEpisodio
												 FROM T_MovPazienti
												 WHERE IDPaziente IN (SELECT IDPaziente FROM #tmpPazienti)
												 )
											AND M.CodStatoPrescrizione NOT IN ('CA','AN')													AND MT.CodStatoPrescrizioneTempi NOT IN ('CA','AN')											ORDER BY MT.DataEvento ASC			
										)
							END	
							
																								SET @uIDScheda=	(SELECT TOP 1 ID
										FROM T_MovSchede
										WHERE 									
												CodStatoScheda <> 'CA' AND
												Storicizzata=0 AND 
												CodScheda=@sCodScheda  AND
												CodEntita=@sCodEntita AND
												IDEntita=@uIDEntitaRiferimento)																										
									
					END
			END 			ELSE 			BEGIN			
				IF 	@sTipoRicerca IN ('PE')							
				  BEGIN						

					IF @sCodEntita IN ('EPI','PAZ','SCH')	
					BEGIN
						
							SET @uIDScheda=(SELECT TOP 1 ID
										FROM T_MovSchede
										WHERE 
												IDEpisodio= @uIDEpisodio AND															IDPaziente IN (SELECT IDPaziente FROM #tmpPazienti) AND												
												CodStatoScheda <> 'CA' AND												
												Storicizzata=0 AND 
												CodScheda=@sCodScheda 
																						ORDER BY DataCreazione ASC		
										)	
					END 					ELSE	
					BEGIN 										

						IF @sCodEntita IN ('PVT')
							BEGIN
									SET @uIDEntitaRiferimento=
										(SELECT TOP 1 ID
										FROM T_MovParametriVitali M
										WHERE M.CodTipoParametroVitale
																							IN (SELECT Codice	
													FROM T_TipoParametroVitale
													WHERE CodScheda=@sCodScheda
													)
																																																																												AND  IDEpisodio= @uIDEpisodio 										AND M.CodStatoParametroVitale NOT IN ('CA','AN')											ORDER BY M.DataEvento ASC			
										)
							END
						IF @sCodEntita IN ('WKI')				
							BEGIN	
								SET @uIDEntitaRiferimento=
										(SELECT TOP 1 ID
										FROM T_MovTaskInfermieristici M
										WHERE M.CodTipoTaskInfermieristico
																							IN (SELECT Codice	
													FROM T_TipoTaskInfermieristico
													WHERE CodScheda=@sCodScheda
													)
																																																																													AND  IDEpisodio= @uIDEpisodio 											AND M.CodStatoTaskInfermieristico NOT IN ('CA','AN')											ORDER BY ISNULL(M.DataErogazione,M.DataProgrammata)  ASC			
										)
							END	

						IF @sCodEntita IN ('DCL')				
							BEGIN	
								SET @uIDEntitaRiferimento=
										(SELECT TOP 1 ID
										FROM T_MovDiarioClinico M
										WHERE CodTipoVoceDiario
																							IN (SELECT Codice	
													FROM T_TipoVoceDiario
													WHERE CodScheda=@sCodScheda
													)
																																																																													AND  IDEpisodio= @uIDEpisodio 											AND M.CodStatoDiario='VA'														ORDER BY DataEvento ASC			
										)
							END	

						IF @sCodEntita IN ('ALA')				
							BEGIN	
								SET @uIDEntitaRiferimento=
										(SELECT TOP 1 ID
										FROM T_MovAlertAllergieAnamnesi M
										WHERE M.CodTipoAlertAllergiaAnamnesi
																							IN (SELECT Codice	
													FROM T_TipoAlertAllergiaAnamnesi
													WHERE CodScheda=@sCodScheda
													)
																						AND IDPaziente IN (SELECT IDPaziente FROM #tmpPazienti)								 
																						AND CodStatoAlertAllergiaAnamnesi NOT IN ('CA','AN')											ORDER BY M.DataEvento ASC			
										)
							END		

						IF @sCodEntita IN ('ALG')				
							BEGIN	
								SET @uIDEntitaRiferimento=
										(SELECT TOP 1 ID
										FROM T_MovAlertGenerici M
										WHERE M.CodTipoAlertGenerico
																							IN (SELECT Codice	
													FROM T_TipoAlertGenerico
													WHERE CodScheda=@sCodScheda
													)
																																																																													AND  IDEpisodio= @uIDEpisodio 											AND CodStatoAlertGenerico NOT IN ('CA','AN')											ORDER BY M.DataEvento ASC			
										)
							END		

						IF @sCodEntita IN ('APP')				
							BEGIN	
								SET @uIDEntitaRiferimento=
										(SELECT TOP 1 ID
										FROM T_MovAppuntamenti M
										WHERE M.CodTipoAppuntamento
																							IN (SELECT Codice	
													FROM T_TipoAppuntamento
													WHERE CodScheda=@sCodScheda
													)
																																	AND  IDEpisodio= @uIDEpisodio 											AND M.CodStatoAppuntamento NOT IN ('CA','AN')											ORDER BY M.DataInizio ASC			
										)
							END		
				
						IF @sCodEntita IN ('PRF')				
							BEGIN	
								SET @uIDEntitaRiferimento=
										(SELECT TOP 1 ID
										FROM T_MovPrescrizioni M
										WHERE M.CodTipoPrescrizione
																							IN (SELECT Codice	
													FROM T_TipoPrescrizione
													WHERE CodScheda=@sCodScheda
													)								
																																																																													AND  IDEpisodio= @uIDEpisodio 											AND M.CodStatoPrescrizione NOT IN ('CA','AN')											ORDER BY M.DataEvento ASC			
										)
							END		

							IF @sCodEntita IN ('PRT')				
							BEGIN	
									SET @uIDEntitaRiferimento=
										(SELECT TOP 1 MT.ID
										FROM T_MovPrescrizioniTempi MT
												INNER JOIN T_MovPrescrizioni M
													ON MT.IDPrescrizione=M.ID
										WHERE M.CodTipoPrescrizione
																							IN (SELECT Codice	
													FROM T_TipoPrescrizione
													WHERE CodSchedaPosologia=@sCodScheda
													)								
																																																																													AND  IDEpisodio= @uIDEpisodio 											AND M.CodStatoPrescrizione NOT IN ('CA','AN')													AND MT.CodStatoPrescrizioneTempi NOT IN ('CA','AN')											ORDER BY MT.DataEvento ASC			
										)
							END	
								SET @uIDScheda=	(SELECT TOP 1 ID
										FROM T_MovSchede
										WHERE 									
												CodStatoScheda <> 'CA' AND
												Storicizzata=0 AND 
												CodScheda=@sCodScheda  AND
												CodEntita=@sCodEntita AND
												IDEntita=@uIDEntitaRiferimento)																										
									
					END

				END 				ELSE
				BEGIN 					
					IF 	@sTipoRicerca IN ('UE')								
					 BEGIN											
						IF @sCodEntita IN ('EPI','PAZ','SCH')	
						BEGIN
							SET @uIDScheda=(SELECT TOP 1 ID
									FROM T_MovSchede
									WHERE 
										IDEpisodio= @uIDEpisodio AND													IDPaziente IN (SELECT IDPaziente FROM #tmpPazienti) AND
										CodStatoScheda <> 'CA' AND
										Storicizzata=0 AND 
										CodScheda=@sCodScheda 
																			ORDER BY DataCreazione DESC		
									)	 

						END 						ELSE	
						BEGIN
							IF @sCodEntita IN ('PVT')
								BEGIN
									SET @uIDEntitaRiferimento=
											(SELECT TOP 1 ID
											FROM T_MovParametriVitali M
											WHERE M.CodTipoParametroVitale
																									IN (SELECT Codice	
														FROM T_TipoParametroVitale
														WHERE CodScheda=@sCodScheda
														)
																																																																																			AND IDEpisodio= @uIDEpisodio 											AND M.CodStatoParametroVitale NOT IN ('CA','AN')												ORDER BY M.DataEvento DESC			
											)
								END

														IF @sCodEntita IN ('WKI')				
								BEGIN	
									SET @uIDEntitaRiferimento=
											(SELECT TOP 1 ID
											FROM T_MovTaskInfermieristici M
											WHERE M.CodTipoTaskInfermieristico
																									IN (SELECT Codice	
														FROM T_TipoTaskInfermieristico
														WHERE CodScheda=@sCodScheda
														)
																																																																																				AND IDEpisodio= @uIDEpisodio 												AND M.CodStatoTaskInfermieristico NOT IN ('CA','AN')												ORDER BY ISNULL(M.DataErogazione,M.DataProgrammata)  DESC			
											)
								END	

								IF @sCodEntita IN ('DCL')				
								BEGIN	
										SET @uIDEntitaRiferimento=
											(SELECT TOP 1 ID
											FROM T_MovDiarioClinico M
											WHERE CodTipoVoceDiario
																									IN (SELECT Codice	
														FROM T_TipoVoceDiario
														WHERE CodScheda=@sCodScheda
														)
																																																																																				AND IDEpisodio= @uIDEpisodio 												AND M.CodStatoDiario='VA'															ORDER BY DataEvento DESC			
											)
								END	

								IF @sCodEntita IN ('ALA')				
								BEGIN	
									SET @uIDEntitaRiferimento=
											(SELECT TOP 1 ID
											FROM T_MovAlertAllergieAnamnesi M
											WHERE M.CodTipoAlertAllergiaAnamnesi
																									IN (SELECT Codice	
														FROM T_TipoAlertAllergiaAnamnesi
														WHERE CodScheda=@sCodScheda
														)
																								AND IDPaziente IN (SELECT IDPaziente FROM T_MovPazienti WHERE IDEpisodio=@uIDEpisodio)								 
												AND CodStatoAlertAllergiaAnamnesi NOT IN ('CA','AN')												ORDER BY M.DataEvento DESC			
											)
								END		

														IF @sCodEntita IN ('ALG')				
								BEGIN	
									SET @uIDEntitaRiferimento=
											(SELECT TOP 1 ID
											FROM T_MovAlertGenerici M
											WHERE M.CodTipoAlertGenerico
																									IN (SELECT Codice	
														FROM T_TipoAlertGenerico
														WHERE CodScheda=@sCodScheda
														)
																																																																																				AND IDEpisodio= @uIDEpisodio 												AND CodStatoAlertGenerico NOT IN ('CA','AN')												ORDER BY M.DataEvento DESC			
											)
								END		

								IF @sCodEntita IN ('APP')				
								BEGIN	
									SET @uIDEntitaRiferimento=
											(SELECT TOP 1 ID
											FROM T_MovAppuntamenti M
											WHERE M.CodTipoAppuntamento
																									IN (SELECT Codice	
														FROM T_TipoAppuntamento
														WHERE CodScheda=@sCodScheda
														)
																																				AND IDEpisodio= @uIDEpisodio 												AND M.CodStatoAppuntamento NOT IN ('CA','AN')												ORDER BY M.DataInizio DESC			
											)
								END		
				
							IF @sCodEntita IN ('PRF')				
								BEGIN	
									SET @uIDEntitaRiferimento=
											(SELECT TOP 1 ID
											FROM T_MovPrescrizioni M
											WHERE M.CodTipoPrescrizione
																									IN (SELECT Codice	
														FROM T_TipoPrescrizione
														WHERE CodScheda=@sCodScheda
														)								
																																																																																				AND IDEpisodio= @uIDEpisodio 												AND M.CodStatoPrescrizione NOT IN ('CA','AN')												ORDER BY M.DataEvento DESC			
											)
								END		

								IF @sCodEntita IN ('PRT')				
								BEGIN	
									SET @uIDEntitaRiferimento=
											(SELECT TOP 1 MT.ID
											FROM T_MovPrescrizioniTempi MT
													INNER JOIN T_MovPrescrizioni M
														ON MT.IDPrescrizione=M.ID
											WHERE M.CodTipoPrescrizione
																									IN (SELECT Codice	
														FROM T_TipoPrescrizione
														WHERE CodSchedaPosologia=@sCodScheda
														)								
																																																																																				AND IDEpisodio= @uIDEpisodio 												AND M.CodStatoPrescrizione NOT IN ('CA','AN')														AND MT.CodStatoPrescrizioneTempi NOT IN ('CA','AN')												ORDER BY MT.DataEvento DESC			
											)
								END	
							
																												SET @uIDScheda=	(SELECT TOP 1 ID
											FROM T_MovSchede
											WHERE 									
													CodStatoScheda <> 'CA' AND
													Storicizzata=0 AND 
													CodScheda=@sCodScheda  AND
													CodEntita=@sCodEntita AND
													IDEntita=@uIDEntitaRiferimento)																										
									
						END
					END 					ELSE
												PRINT 'RICERCA NON GESTITA'
				END		
			END 
	END 
			
											
		DROP TABLE #tmpPazienti		
								
	SELECT 
		CASE
			WHEN @uIDScheda IS NULL THEN 0
			ELSE 1
		END AS Esito,						
		@uIDScheda AS IDScheda
	RETURN 0
END