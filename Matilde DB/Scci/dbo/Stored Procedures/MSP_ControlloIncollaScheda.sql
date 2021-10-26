CREATE PROCEDURE [dbo].[MSP_ControlloIncollaScheda](@xParametri XML)
AS
		
BEGIN
	
		DECLARE @uIDSchedaOrigine AS UNIQUEIDENTIFIER
	DECLARE @sCodEntitaDestinazione AS VARCHAR(20)
	DECLARE @uIDEntitaDestinazione AS UNIQUEIDENTIFIER
	DECLARE @uIDSchedaDestinazione AS UNIQUEIDENTIFIER	
	DECLARE @sCodUA AS VARCHAR(20)
	DECLARE @sCodRuolo AS VARCHAR(20)
	
		
	DECLARE @sCodEntitaOrigine AS VARCHAR(20)
	DECLARE @sCodSchedaOrigine AS VARCHAR(20)
	DECLARE @uIDEntitaOrigine AS UNIQUEIDENTIFIER
	DECLARE @uIDSchedaPadreOrigine AS UNIQUEIDENTIFIER
	DECLARE @nNumSchedePadriOrigine AS INTEGER
	
	DECLARE @sCodEntitaD1 AS VARCHAR(20)
	DECLARE @sCodEntitaD2 AS VARCHAR(20)
		
		
	DECLARE @sCodSchedaDestinazione AS VARCHAR(20)	
	DECLARE @nQtaSchedeDisponibiliDestinazione AS INTEGER
	DECLARE @nNumerositaMassimaDestinazione AS INTEGER
	
	DECLARE @nNumTmp AS INTEGER
	
	DECLARE @bEsito AS BIT	
	DECLARE @sMessaggio AS VARCHAR(2000)	
	DECLARE @sTmp AS VARCHAR(MAX)
	DECLARE @sGUID AS VARCHAR(50)
	
	DECLARE @nTmp AS INTEGER
	DECLARE @xTmp AS XML
	
				
		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDSchedaOrigine.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDSchedaOrigine') as ValoreParametro(IDSchedaOrigine))
	IF 	ISNULL(@sGUID,'') <> '' SET @uIDSchedaOrigine=CONVERT(UNIQUEIDENTIFIER,@sGUID)		
	
		SET @sCodEntitaDestinazione=(SELECT TOP 1 ValoreParametro.CodEntitaDestinazione.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodEntitaDestinazione') as ValoreParametro(CodEntitaDestinazione))
	
		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDEntitaDestinazione.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDEntitaDestinazione') as ValoreParametro(IDEntitaDestinazione))
	IF 	ISNULL(@sGUID,'') <> '' SET @uIDEntitaDestinazione=CONVERT(UNIQUEIDENTIFIER,@sGUID)	
	
		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDSchedaDestinazione.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDSchedaDestinazione') as ValoreParametro(IDSchedaDestinazione))
	IF 	ISNULL(@sGUID,'') <> '' SET @uIDSchedaDestinazione=CONVERT(UNIQUEIDENTIFIER,@sGUID)	
	
		SET @sCodUA=(SELECT TOP 1 ValoreParametro.CodUA.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodUA') as ValoreParametro(CodUA))	
	
		SET @sCodRuolo=(SELECT TOP 1 ValoreParametro.CodRuolo.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/TimeStamp/CodRuolo') as ValoreParametro(CodRuolo))
					  
				
	SET @bEsito=1
	SET @sMessaggio=''
	
					SELECT 
		@sCodEntitaOrigine=CodEntita, 
		@uIDEntitaOrigine=IDEntita,
		@sCodSchedaOrigine=CodScheda,
		@uIDSchedaPadreOrigine=IDSchedaPadre	
	FROM T_MovSchede 
	WHERE ID=@uIDSchedaOrigine
		
		
				
			IF @bEsito=1
		BEGIN
			SET @nTmp=(SELECT COUNT(*) AS QTA
					  FROM 
						T_AssRuoliModuli
					  WHERE 							 			
							CodRuolo =@sCodRuolo AND										CodModulo='Schede_Inserisci'							 )
			IF ISNULL(@nTmp,0)=0 
			BEGIN
				SET @bEsito=0
				SET @sMessaggio=ISNULL(@sMessaggio,'') + 'Il ruolo selezionato non ha i permessi per inserire schede'
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
						CodRuolo =@sCodRuolo AND									CodVoce=@sCodSchedaOrigine							 )
		IF ISNULL(@nTmp,0)=0 
		BEGIN
			SET @bEsito=0
			SET @sMessaggio=ISNULL(@sMessaggio,'') + 'Il ruolo selezionato non ha i permessi per inserire la scheda di origine'
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
					CodVoce=@sCodSchedaOrigine
				)	
			
				IF ISNULL(@nTmp,0)=0 
		BEGIN
			SET @bEsito=0
			SET @sMessaggio=ISNULL(@sMessaggio,'') + 'La UA selezionata non ha i permessi per inserire la scheda di origine'
		END	
			  			
				IF @bEsito=1 AND @sCodEntitaDestinazione='EPI'
		BEGIN
						SET @nTmp=(SELECT COUNT(*) FROM T_MovCartelle
					   WHERE 
						CodStatoCartella='AP' AND
						ID IN (SELECT IDCartella FROM T_MovTrasferimenti
							   WHERE IDEpisodio=@uIDEntitaDestinazione AND
									 CodUA IN (SELECT CodUA FROM T_AssRuoliUA
											   WHERE CodRuolo=@sCodRuolo)							   
							   )
						)
						
			IF ISNULL(@nTmp,0)=0
			BEGIN			    
				SET @bEsito=0
				SET @sMessaggio=ISNULL(@sMessaggio,'') + 'Nessuna cartella aperta relativamente alle UA associate al Ruolo'
			END				
		END	
		
			  			
		DROP TABLE #tmpUAGerarchia			
	END
		
		IF @bEsito=1
	BEGIN		
			
				CREATE TABLE #tmpSchedePadriOrigine
		(
			CodSchedaPadre VARCHAR(20)			
		)

		INSERT INTO #tmpSchedePadriOrigine(CodSchedaPadre)
		SELECT CodSchedaPadre FROM T_SchedePadri
		WHERE CodScheda=@sCodSchedaOrigine							

		SET @nNumSchedePadriOrigine=(SELECT COUNT(*) AS QTA FROM #tmpSchedePadriOrigine)
		

		IF @uIDSchedaDestinazione IS NOT NULL
		BEGIN		
						SET @nQtaSchedeDisponibiliDestinazione= dbo.MF_SelNumerositaSchedaParametro('QtaSchedeDisponibili',@uIDSchedaOrigine,@uIDSchedaDestinazione,NULL,NULL,NULL)
		END
		ELSE
		BEGIN			
						SET @nQtaSchedeDisponibiliDestinazione= dbo.MF_SelNumerositaSchedaParametro('QtaSchedeDisponibili',@uIDSchedaOrigine,NULL,@sCodEntitaDestinazione,@uIDEntitaDestinazione,@sCodSchedaOrigine)	
			SELECT 'MF_SelNumerositaSchedaParametro','QtaSchedeDisponibili',@uIDSchedaOrigine,NULL,@sCodEntitaDestinazione,@uIDEntitaDestinazione,@sCodSchedaOrigine
			
		END
			
		IF @uIDSchedaDestinazione IS NOT NULL
		BEGIN
			SELECT 
				@nNumerositaMassimaDestinazione =ISNULL(S.NumerositaMassima,0),
				@sCodSchedaDestinazione=S.Codice
			FROM
				T_MovSchede MS
					INNER JOIN T_Schede S 
						ON MS.CodScheda=S.Codice
			WHERE 
				MS.ID=@uIDSchedaDestinazione
		END
				SET @nQtaSchedeDisponibiliDestinazione=ISNULL(@nQtaSchedeDisponibiliDestinazione,@nNumerositaMassimaDestinazione) 
		  
		  
						
		SELECT 
			@sCodEntitaD1=CodEntita,
			@sCodEntitaD2=CodEntita2
		FROM T_Schede 
		WHERE Codice=@sCodSchedaOrigine
		
									
				IF @sCodEntitaDestinazione IN ('APP','AGE')
		BEGIN
			SET @bEsito=0
			SET @sMessaggio=ISNULL(@sMessaggio,'') + '(001) Non è possibile spostare schede associate ad appuntamenti'
		END

				IF @uIDSchedaPadreOrigine IS NOT NULL AND @uIDSchedaDestinazione IS NOT NULL
		BEGIN
						IF @uIDSchedaPadreOrigine=@uIDSchedaDestinazione
			BEGIN
				SET @bEsito=0
				SET @sMessaggio=ISNULL(@sMessaggio,'') + '(002) Non è possibile spostare la scheda sullo stesso padre'
			END											
		END		
				
				
																		
				IF @sCodEntitaOrigine=@sCodEntitaDestinazione AND 
			@uIDSchedaDestinazione IS NOT NULL AND
			@nNumSchedePadriOrigine =0
		BEGIN
			SET @bEsito=0
			SET @sMessaggio=ISNULL(@sMessaggio,'') + '(004) Non è possibile spostare questa scheda come figlia di altra scheda'
		END						
		
		IF @uIDSchedaDestinazione IS NOT NULL
		BEGIN
			SET @nNumTmp=(SELECT COUNT(*) FROM #tmpSchedePadriOrigine WHERE CodSchedaPadre=@sCodSchedaDestinazione)	
			IF 	@nNumTmp=0
			BEGIN
				SET @bEsito=0
				SET @sMessaggio=ISNULL(@sMessaggio,'') + '(005) Non è possibile spostare la scheda su una scheda che non è una dei suoi padri'
			END	
		END	

	
		
				IF @nQtaSchedeDisponibiliDestinazione<=0
		BEGIN
			SET @bEsito=0
			SET @sMessaggio=ISNULL(@sMessaggio,'') + '(006) Non è rispettato il vincolo di numerosità'
		END	
				
				IF @sCodEntitaOrigine<> @sCodEntitaDestinazione AND @uIDSchedaDestinazione IS NULL
		BEGIN
			IF @sCodEntitaDestinazione <> ISNULL(@sCodEntitaD1,'') AND @sCodEntitaDestinazione <> ISNULL(@sCodEntitaD2,'')
			BEGIN
				SET @bEsito=0
				SET @sMessaggio=ISNULL(@sMessaggio,'') + '(007) Non è possibile spostare questa scheda sull''entita selezionata'
			END
		END		
		
				IF @sCodEntitaDestinazione <> ISNULL(@sCodEntitaD1,'') AND @sCodEntitaDestinazione <> ISNULL(@sCodEntitaD2,'') AND @uIDSchedaDestinazione IS NULL
		BEGIN
			SET @bEsito=0
			SET @sMessaggio=ISNULL(@sMessaggio,'') + '(008) Non è possibile spostare questa scheda sull''entita selezionata'
		END
		
				IF @uIDEntitaDestinazione=@uIDEntitaOrigine AND @uIDSchedaDestinazione IS NULL 
		BEGIN
			SET @bEsito=0
			SET @sMessaggio=ISNULL(@sMessaggio,'') + '(009) Non è possibile spostare questa scheda sulla stessa entia'
		END

																												
				DROP TABLE #tmpSchedePadriOrigine
	END	
		SELECT 
		@bEsito As Esito,
		@sMessaggio As Messaggio
END