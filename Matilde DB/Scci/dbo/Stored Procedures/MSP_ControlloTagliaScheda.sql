CREATE PROCEDURE [dbo].[MSP_ControlloTagliaScheda](@xParametri XML)
AS
		
BEGIN

		DECLARE @uIDScheda AS UNIQUEIDENTIFIER
	DECLARE @sCodUA AS VARCHAR(20)
	DECLARE @sCodRuolo AS VARCHAR(20)
	
	
		DECLARE @sCodEntita AS VARCHAR(20)
	DECLARE @sCodScheda AS VARCHAR(20)
	DECLARE @uIDEntita AS UNIQUEIDENTIFIER
	
	DECLARE @sCodEntitaD1 AS VARCHAR(20)
	DECLARE @sCodEntitaD2 AS VARCHAR(20)
	DECLARE @nNumSchedePadri AS INTEGER
	DECLARE @nNumCartelleChiuse AS INTEGER
	DECLARE @nTmp AS INTEGER
	
	DECLARE @bEsito AS BIT		
	DECLARE @sMessaggio AS VARCHAR(2000)	
	DECLARE @sTmp AS VARCHAR(MAX)
	DECLARE @sGUID AS VARCHAR(50)
	DECLARE @xTmp AS XML
	
				
		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDScheda.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDScheda') as ValoreParametro(IDScheda))
	IF 	ISNULL(@sGUID,'') <> '' SET @uIDScheda=CONVERT(UNIQUEIDENTIFIER,@sGUID)	
	
		SET @sCodUA=(SELECT TOP 1 ValoreParametro.CodUA.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodUA') as ValoreParametro(CodUA))	
	
		SET @sCodRuolo=(SELECT TOP 1 ValoreParametro.CodRuolo.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/TimeStamp/CodRuolo') as ValoreParametro(CodRuolo))
									  
				

		SET @bEsito=1
	SET @sMessaggio=''
	
	IF @uIDScheda IS NOT NULL
	BEGIN		
				SELECT 
			@sCodEntita=CodEntita, 
			@uIDEntita=IDEntita,
			@sCodScheda=CodScheda	
		FROM T_MovSchede 
		WHERE ID=@uIDScheda
		
				SELECT @sCodEntitaD1=CodEntita,
			@sCodEntitaD2=CodEntita2
		FROM T_Schede 
		WHERE Codice=@sCodScheda
		
								
				IF @bEsito=1
		BEGIN
			SET @nTmp=(SELECT COUNT(*) AS QTA
					  FROM 
						T_AssRuoliModuli
					  WHERE 							 			
							CodRuolo =@sCodRuolo AND										CodModulo='Schede_Cancella'								 )
			IF ISNULL(@nTmp,0)=0 
			BEGIN
				SET @bEsito=0
				SET @sMessaggio=ISNULL(@sMessaggio,'') + '(001) Il ruolo selezionato non ha i permessi per cancellare schede'
			END				
		END	
  
				IF @bEsito=1
		BEGIN								
			SET @nTmp=(SELECT COUNT(*) AS QTA
					  FROM 
						T_AssRuoliAzioni
					  WHERE 
							CodAzione='INS' AND
							CodEntita='SCH' AND 			
							CodRuolo =@sCodRuolo AND										CodVoce=@sCodScheda										 )
			IF ISNULL(@nTmp,0)=0 
			BEGIN
				SET @bEsito=0
				SET @sMessaggio=ISNULL(@sMessaggio,'') + '(002) Il ruolo selezionato non ha i permessi per inserire la scheda di origine'
			END	
		END
		
				IF @bEsito=1	
		BEGIN		
			
						CREATE TABLE #tmpUAGerarchia
			(
				Codice VARCHAR(20) COLLATE Latin1_General_CI_AS,
				Descrizione VARCHAR(255) COLLATE Latin1_General_CI_AS
			)
		
						SET  @xTmp=CONVERT(XML,'<Parametri><CodUA>'+ @sCodUA + '</CodUA></Parametri>')		
			INSERT #tmpUAGerarchia EXEC MSP_SelUAPadri @xTmp	
			CREATE INDEX IX_CodUA ON #tmpUAGerarchia (Codice)    
			
			SET @nTmp=(SELECT COUNT(*) AS QTA
					  FROM T_AssUAEntita AU
						INNER JOIN #tmpUAGerarchia GA
							ON AU.CodUA=GA.Codice
					 WHERE 				
						CodEntita='SCH' AND
						CodVoce=@sCodScheda
					)	
			IF ISNULL(@nTmp,0)=0 
			BEGIN
				SET @bEsito=0
				SET @sMessaggio=ISNULL(@sMessaggio,'') + '(003) La UA selezionata non ha i permessi per inserire la scheda di origine'
			END	
				  			
			DROP TABLE #tmpUAGerarchia			
		END
		
		
		
				IF @bEsito=1
		BEGIN
						CREATE TABLE #tmpSchedePadri
			(
				CodSchedaPadre VARCHAR(20)
			)

			INSERT INTO #tmpSchedePadri(CodSchedaPadre)
			SELECT CodSchedaPadre FROM T_SchedePadri
			WHERE CodScheda=@sCodScheda
																	
			SET @nNumSchedePadri=(SELECT COUNT(*) AS QTA FROM #tmpSchedePadri)
							
									
			IF @sCodEntita IN ('APP')
			BEGIN
				SET @bEsito=0
				SET @sMessaggio=ISNULL(@sMessaggio,'') + '(004) Non è possibile spostare schede associate ad appuntamenti'
			END
			
				
			IF @sCodEntita IN ('PAZ')
			BEGIN
				IF ISNULL(@sCodEntitaD2,'')='' AND @nNumSchedePadri=0
				BEGIN
					SET @bEsito=0
					SET @sMessaggio=ISNULL(@sMessaggio,'') + '(005) Non è possibile spostare questa scheda sullo stesso paziente'
				END	
			END
			
			
			IF @sCodEntita IN ('EPI')
			BEGIN
								
								SELECT 
					@nNumCartelleChiuse=COUNT(*)
				FROM T_MovCartelle
				WHERE 
					CodStatoCartella='CH' AND							ID IN 
					(SELECT IDCartella
					 FROM T_MovTrasferimenti 
					 WHERE 
						CodStatoTrasferimento NOT IN ('CA') AND	
						IDEpisodio=@uIDEntita
				)
				
								SET @sTmp=''
				IF @nNumCartelleChiuse>0			
					SELECT 
						@sTmp=CASE 
								WHEN ISNULL(@sTmp,'')='' THEN NumeroCartella
								ELSE @sTmp + ' ,' +  NumeroCartella 
								END	
					FROM T_MovCartelle
					WHERE 
						CodStatoCartella='CH' AND								ID IN 
						(SELECT IDCartella
						 FROM T_MovTrasferimenti 
						 WHERE 
							CodStatoTrasferimento NOT IN ('CA') AND	
							IDEpisodio=@uIDEntita
					)
							
								IF @nNumCartelleChiuse > 0
				BEGIN
					SET @bEsito=0
					SET @sMessaggio=ISNULL(@sMessaggio,'') + '(006) Non è possibile spostare questa scheda, esistono ' + CONVERT(VARCHAR(10),@nNumCartelleChiuse) + ' cartelle chiuse (' + @sTmp + ')'
				END
				
								SET @nTmp=(SELECT COUNT(*) FROM T_MovCartelle
						   WHERE 
							CodStatoCartella='AP' AND
							ID IN (SELECT IDCartella FROM T_MovTrasferimenti
								   WHERE IDEpisodio=@uIDEntita AND
										 CodUA IN (SELECT CodUA FROM T_AssRuoliUA
												   WHERE CodRuolo=@sCodRuolo)							   
								   )
							)
							
				IF ISNULL(@nTmp,0)=0
				BEGIN			    
					SET @bEsito=0
					SET @sMessaggio=ISNULL(@sMessaggio,'') + '(007) Nessuna cartella aperta relativamente alle UA associate al Ruolo'
				END				
			END
			
						DROP TABLE #tmpSchedePadri
		END 	END
			
	SELECT 
		@bEsito As Esito,
		@sMessaggio As Messaggio
END