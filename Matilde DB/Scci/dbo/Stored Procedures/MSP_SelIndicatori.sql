CREATE PROCEDURE [dbo].[MSP_SelIndicatori](@xParametri XML)
AS
BEGIN
		
	
	
	
		DECLARE @sCodRuolo AS VARCHAR(20)
	DECLARE @sCodUtente AS VARCHAR(100)
	DECLARE @uIDPaziente AS UNIQUEIDENTIFIER	
	DECLARE @uIDEpisodio AS UNIQUEIDENTIFIER
	DECLARE @uIDCartella AS UNIQUEIDENTIFIER
	DECLARE @nNumDaFirmare AS INTEGER
	DECLARE @nNumAllergie AS INTEGER
	DECLARE @nNumAlert AS INTEGER
	DECLARE @nNumEvidenzaClinica AS INTEGER
	DECLARE @nNumSegnalibri AS INTEGER
	DECLARE @nNumCartelleInVisione AS INTEGER
	DECLARE @nNumPazientiInVisione AS INTEGER
	DECLARE @nNumPazientiSeguiti AS INTEGER
	DECLARE @nNumPazienteSeguito AS INTEGER
	DECLARE @nNumPazientiSeguitiDaAltri AS INTEGER
	DECLARE @nNumNewsHard AS INTEGER
	DECLARE @nMatHome AS INTEGER

		DECLARE @xPar AS XML
	
				
	SET @sCodUtente=(SELECT TOP 1 ValoreParametro.CodUtente.value('.','VARCHAR(100)')
					  FROM @xParametri.nodes('/Parametri/CodUtente') as ValoreParametro(CodUtente))	
	SET @sCodUtente=ISNULL(@sCodUtente,'')

	SET @sCodRuolo=(SELECT TOP 1 ValoreParametro.CodRuolo.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodRuolo') as ValoreParametro(CodRuolo))	
	SET @sCodRuolo=ISNULL(@sCodRuolo,'')
	
	SET @uIDPaziente=(SELECT TOP 1 ValoreParametro.IDPaziente.value('.','UNIQUEIDENTIFIER')
					  FROM @xParametri.nodes('/Parametri/IDPaziente') as ValoreParametro(IDPaziente))	

	SET @uIDEpisodio=(SELECT TOP 1 ValoreParametro.IDEpisodio.value('.','UNIQUEIDENTIFIER')
					  FROM @xParametri.nodes('/Parametri/IDEpisodio') as ValoreParametro(IDEpisodio))	

	SET @uIDCartella=(SELECT TOP 1 ValoreParametro.IDCartella.value('.','UNIQUEIDENTIFIER')
					  FROM @xParametri.nodes('/Parametri/IDCartella') as ValoreParametro(IDCartella))
					  	
				
		CREATE TABLE #tmpEpisodiCollegati
	(
		IDEpisodio UNIQUEIDENTIFIER
	)
			
		IF @sCodUtente<> '' 
	BEGIN			
		SET @nNumDaFirmare=	(SELECT COUNT(M.ID) AS Qta 
							  FROM 
									T_MovDiarioClinico M  WITH (NOLOCK)
										INNER JOIN T_MovTrasferimenti T  WITH (NOLOCK)
											ON M.IDTrasferimento=T.ID										
										INNER JOIN T_MovCartelle C  WITH (NOLOCK)
											ON T.IDCartella=C.ID	
							  WHERE M.CodUtenteRilevazione=@sCodUtente
									AND CodStatoDiario ='IC'														AND C.CodStatoCartella='AP'													 ) 	 	
	END


	IF ISNULL(CONVERT(VARCHAR(50),@uIDPaziente),'')<> ''
	BEGIN
				SET @nNumAllergie=	(SELECT COUNT(ID) AS Qta 
							  FROM 
									T_MovAlertAllergieAnamnesi  WITH (NOLOCK)											
							  WHERE IDPaziente=	@uIDPaziente	
									AND 	ISNULL(CodStatoAlertAllergiaAnamnesi,'')='AT'
								 ) 	 		
	END
	
	IF ISNULL(CONVERT(VARCHAR(50),@uIDEpisodio),'')<> ''
	BEGIN
	
				SET @xPar=CONVERT(XML,'<Parametri>
								<IDEpisodio>' + CONVERT(VARCHAR(50),@uIDEpisodio) + '</IDEpisodio>
								</Parametri>')
		
				INSERT INTO #tmpEpisodiCollegati												 				
			EXEC MSP_SelEpisodiCollegati @xPar	
		
				INSERT INTO #tmpEpisodiCollegati(IDEpisodio)
		SELECT @uIDEpisodio
		WHERE
			NOT EXISTS (SELECT IDEpisodio
						FROM #tmpEpisodiCollegati
						WHERE IDEpisodio=@uIDEpisodio)
						
						
					SET @nNumAlert=	
					(SELECT COUNT(ID) AS QTA	
					 FROM 
							T_MovAlertGenerici 	
					 WHERE CodStatoAlertGenerico='DV'									       AND IDEpisodio=@uIDEpisodio
					) 				
					 
				 SET @nNumEvidenzaClinica=
					(SELECT COUNT(ID) AS QTA	
					 FROM 
							T_MovEvidenzaClinica M  WITH (NOLOCK)
																INNER JOIN 	#tmpEpisodiCollegati T 
									ON M.IDEpisodio=T.IDEpisodio
					 WHERE CodStatoEvidenzaClinicaVisione='DV'										   AND	CodStatoEvidenzaClinica='CM'										
					 ) 		
	
	END
			
	SET @nNumCartelleInVisione = 0
	
		IF ISNULL(CONVERT(VARCHAR(50),@uIDCartella),'')<> ''
		BEGIN			
						SET @nNumSegnalibri=(SELECT COUNT(ID) AS QTA
								 FROM T_MovSegnalibri  WITH (NOLOCK)
								 WHERE
									CodUtente=@sCodUtente AND
									CodRuolo=@sCodRuolo AND
									(IDCartella=@uIDCartella OR 
										(CodEntitaScheda='PAZ' AND
											(IDPaziente=@uIDPaziente OR
												 												 IDPaziente IN 
													(SELECT IDPazienteVecchio
													 FROM T_PazientiAlias  WITH (NOLOCK)
													 WHERE 
														IDPaziente IN 
															(SELECT IDPaziente
															 FROM T_PazientiAlias  WITH (NOLOCK)
															 WHERE IDPazienteVecchio=@uIDPaziente
															)
													)
												)
										)		
									)
								)	
							SET @nNumCartelleInVisione=(SELECT COUNT(ID) AS QTA
							 FROM T_MovCartelleInVisione  WITH (NOLOCK)
							 WHERE
								IDCartella=@uIDCartella AND
								CodStatoCartellaInVisione IN ('IC')									)		
		END
	ELSE		
		IF ISNULL(CONVERT(VARCHAR(50),@uIDPaziente),'')<> ''
			BEGIN	
								
								SET @nNumSegnalibri=(SELECT COUNT(ID) AS QTA
									FROM T_MovSegnalibri  WITH (NOLOCK)
									WHERE
										CodUtente=@sCodUtente AND
										CodRuolo=@sCodRuolo AND
										IDCartella IS NULL AND
										(IDPaziente=@uIDPaziente OR
										 										 IDPaziente IN 
											(SELECT IDPazienteVecchio
											 FROM T_PazientiAlias  WITH (NOLOCK)
											 WHERE 
												IDPaziente IN 
													(SELECT IDPaziente
													 FROM T_PazientiAlias  WITH (NOLOCK)
													 WHERE IDPazienteVecchio=@uIDPaziente
													)
											)
										)
									)				
			END
	
				
		SET @nNumPazientiSeguiti =(
					SELECT COUNT(*) 
					FROM T_MovPazientiSeguiti  WITH (NOLOCK)
					WHERE CodUtente = @sCodUtente AND
						  CodRuolo = @sCodRuolo AND
						  CodStatoPazienteSeguito IN ('IC')
					)	  
					
		IF ISNULL(CONVERT(VARCHAR(50),@uIDPaziente),'') = ''
		BEGIN
			SET @nNumPazientiSeguitiDaAltri=0
		END
	ELSE
		BEGIN
			SET @nNumPazientiSeguitiDaAltri =(
					SELECT COUNT(*) 
					FROM T_MovPazientiSeguiti  WITH (NOLOCK)
					WHERE IDPaziente=@uIDPaziente AND
						  (CodUtente <> @sCodUtente OR
						  CodRuolo <> @sCodRuolo) AND
						  CodStatoPazienteSeguito IN ('IC')
					)	  
		END			
		
		IF ISNULL(CONVERT(VARCHAR(50),@uIDPaziente),'') = ''
		BEGIN
			SET @nNumPazienteSeguito=0
		END
	ELSE
		BEGIN
			SET @nNumPazienteSeguito =(
					SELECT COUNT(*) 
					FROM T_MovPazientiSeguiti  WITH (NOLOCK)
					WHERE IDPaziente=@uIDPaziente AND
						  CodUtente = @sCodUtente AND
						  CodRuolo = @sCodRuolo AND
						  CodStatoPazienteSeguito IN ('IC')
					)	  
		END			
		
		SET @nNumPazientiInVisione =(
					SELECT COUNT(*) 
					FROM T_MovPazientiInVisione  WITH (NOLOCK)
					WHERE 	 
						  CodStatoPazienteInVisione IN ('IC') AND
						  (IDPaziente=@uIDPaziente OR
										 										 IDPaziente IN 
											(SELECT IDPazienteVecchio
											 FROM T_PazientiAlias  WITH (NOLOCK)
											 WHERE 
												IDPaziente IN 
													(SELECT IDPaziente
													 FROM T_PazientiAlias  WITH (NOLOCK)
													 WHERE IDPazienteVecchio=@uIDPaziente
													)
											)
										)
					)	  

		SET @nMatHome = (
					SELECT COUNT(*) 
					FROM T_MH_Login  WITH (NOLOCK)
					WHERE 	 						
						  (IDPaziente=@uIDPaziente OR
										 										 IDPaziente IN 
											(SELECT IDPazienteVecchio
											 FROM T_PazientiAlias  WITH (NOLOCK)
											 WHERE 
												IDPaziente IN 
													(SELECT IDPaziente
													 FROM T_PazientiAlias  WITH (NOLOCK)
													 WHERE IDPazienteVecchio=@uIDPaziente
													)
											)
										)
					)


	SELECT 	IsNull(@nNumDaFirmare,0) AS DiarioClinico		
		,IsNull(@nNumAllergie,0) AS Allergie			
		,IsNull(@nNumAlert,0) AS Alert				
		,IsNull(@nNumEvidenzaClinica,0) AS EvidenzaClinica
		,IsNull(@nNumSegnalibri,0) AS Segnalibri
		,IsNull(@nNumCartelleInVisione,0) AS CartelleInVisione
		,IsNull(@nNumPazientiInVisione,0) AS PazientiInVisione
		,IsNull(@nNumPazientiSeguiti,0) AS PazientiSeguiti
		,IsNull(@nNumPazienteSeguito,0) AS PazienteSeguito
		,IsNull(@nNumPazientiSeguitiDaAltri,0) AS PazientiSeguitiDaAltri
		,IsNull(@nMatHome,0) AS MatHome
	
END