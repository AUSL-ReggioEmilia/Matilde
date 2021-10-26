CREATE PROCEDURE [dbo].[MSP_SelAgende](@xParametri AS XML )
AS
BEGIN
	

				 
	DECLARE @uIDAppuntamento AS UNIQUEIDENTIFIER
	DECLARE @sCodUA AS Varchar(20)
	DECLARE @sCodTipoAppuntamento AS Varchar(20)
	DECLARE @sCodRuolo AS Varchar(20)
	DECLARE @sCodAzione AS VARCHAR(20)
	DECLARE @sCodAgenda AS VARCHAR(20)
	DECLARE @bDatiEstesi AS Bit
	DECLARE @bSoloFiltroAgenda AS Bit
	
		DECLARE @sGUID AS VARCHAR(50)
				
			  				  
		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDAppuntamento.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDAppuntamento') as ValoreParametro(IDAppuntamento))		
	IF 	ISNULL(@sGUID,'') <> '' 
		SET @uIDAppuntamento=CONVERT(UNIQUEIDENTIFIER,	@sGUID)	
	
		SET @sCodTipoAppuntamento=(SELECT	TOP 1 ValoreParametro.CodTipoAppuntamento.value('.','VARCHAR(20)')
						 FROM @xParametri.nodes('/Parametri/CodTipoAppuntamento') as ValoreParametro(CodTipoAppuntamento))
	IF ISNULL(@sCodTipoAppuntamento,'')='' SET @sCodTipoAppuntamento=''	
	
		SET @sCodUA=(SELECT	TOP 1 ValoreParametro.CodUA.value('.','VARCHAR(20)')
						 FROM @xParametri.nodes('/Parametri/CodUA') as ValoreParametro(CodUA))
	SET @sCodUA =ISNULL(@sCodUA,'')
	
		SET @sCodRuolo=(SELECT	TOP 1 ValoreParametro.CodRuolo.value('.','VARCHAR(20)')
					FROM @xParametri.nodes('/Parametri/CodRuolo') as ValoreParametro(CodRuolo)) 
	IF ISNULL(@sCodRuolo,'')='' SET @sCodRuolo='###'					
	
		SET @sCodAzione=(SELECT	TOP 1 ValoreParametro.CodAzione.value('.','VARCHAR(20)')
					 FROM @xParametri.nodes('/Parametri/CodAzione') as ValoreParametro(CodAzione))				  
	IF ISNULL(@sCodAzione,'')='' SET @sCodAzione='###'	
	
		SET @sCodAgenda=(SELECT	TOP 1 ValoreParametro.CodAgenda.value('.','VARCHAR(20)')
					 FROM @xParametri.nodes('/Parametri/CodAgenda') as ValoreParametro(CodAgenda))				  
	IF ISNULL(@sCodAgenda,'')='' SET @sCodAgenda=''	
	
	
	
		SET @bDatiEstesi=(SELECT TOP 1 ValoreParametro.DatiEstesi.value('.','bit')
					  FROM @xParametri.nodes('/Parametri/DatiEstesi') as ValoreParametro(DatiEstesi))											
		
		SET @bSoloFiltroAgenda=(SELECT TOP 1 ValoreParametro.SoloFiltroAgenda.value('.','bit')
					  FROM @xParametri.nodes('/Parametri/SoloFiltroAgenda') as ValoreParametro(SoloFiltroAgenda))											
	
	SET @bSoloFiltroAgenda=ISNULL(@bSoloFiltroAgenda,0)
	
				
	DECLARE @sCodEntita VARCHAR(20)
	DECLARE @bAbilitaPermessiDettaglio AS bit
	DECLARE @bAbilitaPermessiDettaglioAzione AS bit
	
	DECLARE @xTmp AS XML
	
	
		SET @sCodEntita='AGE'
	
					
	CREATE TABLE #tmpUA
	(
		CodUA VARCHAR(20) COLLATE Latin1_General_CI_AS,
		Descrizione VARCHAR(255) COLLATE Latin1_General_CI_AS
	)

	IF @sCodUA<> ''
	BEGIN		
				SET  @xTmp=CONVERT(XML,'<Parametri><CodUA>'+ @sCodUA + '</CodUA></Parametri>')

		INSERT #tmpUA EXEC MSP_SelUAPadri @xTmp	
	END
	ELSE
	BEGIN		
				SET  @xTmp=CONVERT(XML,'<Parametri><CodRuolo>'+ @sCodRuolo + '</CodRuolo></Parametri>')

		INSERT #tmpUA EXEC MSP_SelUADaRuolo @xTmp			
		
	END
		CREATE INDEX IX_CodUA ON #tmpUA (CodUA)    		
			
		IF @bSoloFiltroAgenda=0 	
	BEGIN		
		IF @uIDAppuntamento IS NULL
																																				BEGIN					
										SELECT	
							D.Codice,
							D.Descrizione,
							D.CodTipoAgenda,
							TA.Descrizione AS DescrTipoAgende,
							CASE 
								WHEN ISNULL(@bDatiEstesi,0)=0 THEN NULL
									ELSE
									TA.Icona
							END AS  Icona,
															
							D.Colore,
							CASE 
								WHEN ISNULL(@bDatiEstesi,0)=0 THEN NULL
									ELSE
									D.ElencoCampi
							END AS  ElencoCampi,
									
							CASE 
								WHEN ISNULL(@bDatiEstesi,0)=0 THEN NULL
									ELSE
									D.IntervalloSlot
							END AS  IntervalloSlot,
									
							CASE 
								WHEN ISNULL(@bDatiEstesi,0)=0 THEN NULL
									ELSE
									D.OrariLavoro
							END AS  OrariLavoro,
														0 AS Selezionata,
							1 AS PermessoAgenda,
							'' As IDAppuntamentoAgenda,
							UsaColoreTipoAppuntamento,
							ISNULL(MassimoAnticipoPrenotazione,0) AS MassimoAnticipoPrenotazione,
							ISNULL(MassimoRitardoPrenotazione,0) AS MassimoRitardoPrenotazione,	
							D.Lista,
							D.ParametriLista,
							'' AS CodRaggr1,
							'' AS DescrRaggr1,
							'' AS CodRaggr2,
							'' AS DescrRaggr2,
							'' AS CodRaggr3,
							'' AS DescrRaggr3,
							D.Risorse						
					FROM						
						(SELECT DISTINCT 
							A.CodVoce AS Codice												
							FROM					
							T_AssUAEntita A
								INNER JOIN #tmpUA T ON
									A.CodUA=T.CodUA					
							WHERE CodEntita=@sCodEntita			
							) AS W				
							INNER JOIN T_Agende D													ON D.Codice=W.Codice	
							
							LEFT JOIN T_TipoAgenda TA
									ON D.CodTipoAgenda=TA.Codice

																																			LEFT JOIN 
								(SELECT CodVoce AS CodAgenda, COUNT(*) AS QtaRuoli
							 	 FROM T_AssRuoliAzioni
								 WHERE 
									CodEntita='AGE' AND CodAzione='VIS'
								  GROUP BY CodVoce) AS QTAAGERUO
								ON D.Codice=QTAAGERUO.CodAgenda

														LEFT JOIN 
								(SELECT CodRuolo, CodVoce AS CodAgenda
								 FROM T_AssRuoliAzioni
								 WHERE CodEntita='AGE' AND CodAzione='VIS') AS RUOAGE
								 ON (RUOAGE.CodRuolo = @sCodRuolo AND
									 RUOAGE.CodAgenda=D.Codice
									)

					WHERE				
												D.Codice IN (
							SELECT DISTINCT CodAgenda
							FROM								
								T_AssAgendeTipoAppuntamenti AssAT
							WHERE										
								CodTipoApp=CASE 
													WHEN @sCodTipoAppuntamento<> '' THEN @sCodTipoAppuntamento
													ELSE CodTipoApp
											END		
							)														
						AND										
												D.Codice = CASE 
										WHEN @sCodAgenda <> '' THEN @sCodAgenda
										ELSE D.Codice
									END	
						
												AND
							1= CASE 
																WHEN ISNULL(QTAAGERUO.QtaRuoli,0) > 0 THEN 
										CASE 
											WHEN RUOAGE.CodRuolo IS NULL THEN 0														ELSE 1
										END										
								ELSE 1								   END
						ORDER BY D.Ordine,D.Descrizione													  						
			END
			ELSE
			BEGIN
					
																
												SET @sCodTipoAppuntamento=(SELECT CodTipoAppuntamento FROM T_MovAppuntamenti WHERE ID=@uIDAppuntamento)									
				SET @sCodTipoAppuntamento=ISNULL(@sCodTipoAppuntamento,'')
				
																
																				SELECT 				
					AgeUA.Codice,
					D.Descrizione,
					D.CodTipoAgenda,
					TA.Descrizione AS DescrTipoAgende,
					CASE 
						WHEN ISNULL(@bDatiEstesi,0)=0 THEN NULL
							ELSE
							TA.Icona
					END AS  Icona,
											
					D.Colore,
					CASE 
						WHEN ISNULL(@bDatiEstesi,0)=0 THEN NULL
							ELSE
							D.ElencoCampi
					END AS  ElencoCampi,
									
					CASE 
						WHEN ISNULL(@bDatiEstesi,0)=0 THEN NULL
							ELSE
							D.IntervalloSlot
					END AS  IntervalloSlot,
					
					CASE 
						WHEN ISNULL(@bDatiEstesi,0)=0 THEN NULL
							ELSE
							D.OrariLavoro
					END AS  OrariLavoro,
					
					CASE 
						WHEN AgeAPP.IDAppuntamento IS NOT NULL THEN 1
						ELSE 0
					END  AS Selezionata,
									
																									 1 AS PermessoAgenda,											 IDAppuntamentoAgenda,
					 UsaColoreTipoAppuntamento,
					 ISNULL(MassimoAnticipoPrenotazione,0) AS MassimoAnticipoPrenotazione,
					 ISNULL(MassimoRitardoPrenotazione,0) AS MassimoRitardoPrenotazione,
					 D.Lista,
					 D.ParametriLista,
					 CodRaggr1,
					 DescrRaggr1,
					 CodRaggr2,
					 DescrRaggr2,
					 CodRaggr3,
					 DescrRaggr3,
					 D.Risorse

				FROM 		
														(SELECT DISTINCT 
												A.CodVoce AS Codice												
											 FROM					
												T_AssUAEntita A
													INNER JOIN #tmpUA T ON
														A.CodUA=T.CodUA					
											 WHERE CodEntita=@sCodEntita			
							) AS AgeUA		
			
										LEFT JOIN
														(SELECT 
								DISTINCT ID AS IDAppuntamentoAgenda,
										 IDAppuntamento,
										 CodAgenda AS Codice,
										 CodRaggr1,
										 DescrRaggr1,
										 CodRaggr2,
										 DescrRaggr2,
										 CodRaggr3,
										 DescrRaggr3
								FROM 
									T_MovAppuntamentiAgende MovAGE
								WHERE 
																		ISNULL(CodStatoAppuntamentoAgenda,'') <> 'CA'  AND
									
																		IDAppuntamento=@uIDAppuntamento								
							) AS AgeAPP						
					
						ON AgeAPP.Codice=AgeUA.Codice

					 LEFT JOIN T_Agende D																	ON D.Codice=AgeUA.Codice	
					 LEFT JOIN T_TipoAgenda TA
									ON D.CodTipoAgenda=TA.Codice									
				
					WHERE				
												D.Codice IN (
							SELECT DISTINCT CodAgenda
							FROM								
								T_AssAgendeTipoAppuntamenti AssAT
							WHERE										
								CodTipoApp=CASE 
													WHEN @sCodTipoAppuntamento<> '' THEN @sCodTipoAppuntamento
													ELSE CodTipoApp
											END		
							)			
						AND				
														D.Codice = CASE 
											WHEN @sCodAgenda <> '' THEN @sCodAgenda
											ELSE D.Codice
										END														
					ORDER BY D.Ordine,D.Descrizione

																	
								SELECT 				
					AgeAPP.Codice,
					D.Descrizione,
					TA.Descrizione AS DescrTipoAgende,
					CASE 
						WHEN ISNULL(@bDatiEstesi,0)=0 THEN NULL
							ELSE
							TA.Icona
					END AS  Icona,
											
					D.Colore,
					CASE 
						WHEN ISNULL(@bDatiEstesi,0)=0 THEN NULL
							ELSE
							D.ElencoCampi
					END AS  ElencoCampi,
									
					CASE 
						WHEN ISNULL(@bDatiEstesi,0)=0 THEN NULL
							ELSE
							D.IntervalloSlot
					END AS  IntervalloSlot,
					
					CASE 
						WHEN ISNULL(@bDatiEstesi,0)=0 THEN NULL
							ELSE
							D.OrariLavoro
					END AS  OrariLavoro,
					
					CASE 
						WHEN AgeAPP.IDAppuntamento IS NOT NULL THEN 	1
						ELSE 0
					END AS Selezionata,
									
					0 AS PermessoAgenda,
					NULL AS IDAppuntamentoAgenda,
					UsaColoreTipoAppuntamento,
					ISNULL(MassimoAnticipoPrenotazione,0) AS MassimoAnticipoPrenotazione,
					ISNULL(MassimoRitardoPrenotazione,0) AS MassimoRitardoPrenotazione,
					D.Lista,
					D.ParametriLista,
					CodRaggr1,
					DescrRaggr1,
					CodRaggr2,
					DescrRaggr2,
					CodRaggr3,
					DescrRaggr3,
					D.Risorse	
					
				FROM 
								
										
														(SELECT 
								DISTINCT 
									IDAppuntamento,
									CodAgenda AS Codice,
									CodRaggr1,
									DescrRaggr1,
									CodRaggr2,
									DescrRaggr2,
									CodRaggr3,
									DescrRaggr3	 
								FROM 
									T_MovAppuntamentiAgende MovAGE
								WHERE 
																		IDAppuntamento=@uIDAppuntamento								
							) AS  AgeAPP	
							
					LEFT JOIN		
				
														(SELECT DISTINCT 
												A.CodVoce AS Codice												
											 FROM					
												T_AssUAEntita A
													INNER JOIN #tmpUA T ON
														A.CodUA=T.CodUA					
											 WHERE CodEntita=@sCodEntita			
							) AS AgeUA															
			
					ON AgeAPP.Codice=AgeUA.Codice

					LEFT JOIN T_Agende D															ON D.Codice=AgeAPP.Codice	
					 LEFT JOIN T_TipoAgenda TA
							ON D.CodTipoAgenda=TA.Codice	
															
			   WHERE AgeUA.Codice IS NULL		
						AND			
							
												D.Codice IN (
							SELECT DISTINCT CodAgenda
							FROM								
								T_AssAgendeTipoAppuntamenti AssAT
							WHERE										
								CodTipoApp=CASE 
													WHEN @sCodTipoAppuntamento<> '' THEN @sCodTipoAppuntamento
													ELSE CodTipoApp
											END		
							)	
						AND				
														D.Codice = CASE 
											WHEN @sCodAgenda <> '' THEN @sCodAgenda
											ELSE D.Codice
										END				
				ORDER BY D.Ordine,D.Descrizione
			END
	END
	ELSE
	BEGIN
														SELECT AG.Codice,AG.Descrizione, AG.UsaColoreTipoAppuntamento 
		FROM T_Agende AG
		WHERE
			Codice IN 						
									(SELECT DISTINCT 
							A.CodVoce AS Codice												
						 FROM					
							T_AssUAEntita A
								INNER JOIN #tmpUA T ON
									A.CodUA=T.CodUA					
						 WHERE CodEntita=@sCodEntita			
						) AND
		AG.Codice=@sCodAgenda
		
	END			
	
		DROP TABLE #tmpUA			
	RETURN 0
END