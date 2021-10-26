CREATE PROCEDURE [dbo].[MSP_CercaSchedaPrecedente](@xParametri XML) WITH RECOMPILE
AS
BEGIN

	
	

	
							
		DECLARE @sCodScheda  AS VARCHAR(20)
	DECLARE @nVersione INTEGER
	DECLARE @uIDPaziente AS UNIQUEIDENTIFIER	
	DECLARE @uIDEpisodio AS UNIQUEIDENTIFIER	
	DECLARE @sCodEntita  AS VARCHAR(20)

	DECLARE @bIgnoraVersione AS BIT


		DECLARE @sGUID AS VARCHAR(50)
	DECLARE @uGUID AS UNIQUEIDENTIFIER	
	DECLARE @sCodModalitaCopiaPrecedente  AS VARCHAR(20)
	DECLARE @bCopiaPrecedenteSelezione  AS BIT
	DECLARE @b  AS VARCHAR(20)
	DECLARE @sSQL  AS VARCHAR(MAX)
	
	
		SET @sCodScheda=(SELECT TOP 1 ValoreParametro.CodScheda.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodScheda') as ValoreParametro(CodScheda))	
	SET @sCodScheda=ISNULL(@sCodScheda,'')
	
		SET @nVersione=(SELECT TOP 1 ValoreParametro.Versione.value('.','INTEGER')
					  FROM @xParametri.nodes('/Parametri/Versione') as ValoreParametro(Versione))	
	
		
		SET @uIDPaziente=(SELECT TOP 1 ValoreParametro.IDPaziente.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDPaziente') as ValoreParametro(IDPaziente))
	IF 	ISNULL(@sGUID,'') <> ''
		 SET @uIDPaziente=CONVERT(UNIQUEIDENTIFIER,	@sGUID)		  

		SET @uIDEpisodio=(SELECT TOP 1 ValoreParametro.IDEpisodio.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDEpisodio') as ValoreParametro(IDEpisodio))
	IF 	ISNULL(@sGUID,'') <> ''
		 SET @uIDEpisodio=CONVERT(UNIQUEIDENTIFIER,	@sGUID)	
	
		SET @sCodEntita=(SELECT TOP 1 ValoreParametro.CodEntita.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodEntita') as ValoreParametro(CodEntita))	
	SET @sCodEntita=ISNULL(@sCodEntita,'')

		SET @bIgnoraVersione=(SELECT TOP 1 ValoreParametro.IgnoraVersione.value('.','BIT')
					  FROM @xParametri.nodes('/Parametri/IgnoraVersione') as ValoreParametro(IgnoraVersione))
	
	SET @bIgnoraVersione=ISNULL(@bIgnoraVersione,0)
	
				
		SELECT TOP 1
		@sCodModalitaCopiaPrecedente=CodModalitaCopiaPrecedente,
		@bCopiaPrecedenteSelezione=CopiaPrecedenteSelezione
	FROM T_Schede 
	WHERE Codice=@sCodScheda
	
	SET @sCodModalitaCopiaPrecedente=ISNULL(@sCodModalitaCopiaPrecedente,'CREA')
	SET @bCopiaPrecedenteSelezione=ISNULL(@bCopiaPrecedenteSelezione,0)

		IF @bIgnoraVersione=0
	BEGIN		
				IF ISNULL(@nVersione,0)=0
		BEGIN
						SET @nVersione=(SELECT TOP 1 			
								ISNULL(V.Versione, 0) AS Versione			
								FROM 
									T_Schede S 
										INNER JOIN T_SchedeVersioni V ON
									S.Codice = V.CodScheda
								WHERE
									S.Codice = @sCodScheda AND																					ISNULL(V.FlagAttiva, 0) = 1	AND ISNULL(V.Pubblicato,0)=1 													AND 
										V.DtValI <= GETDATE() AND																					ISNULL(V.DtValF,convert(datetime,'01-01-2100')) >=  GETDATE()														
						)
		END								
	END

	CREATE TABLE #tmpIDScheda
	(
		IDScheda UNIQUEIDENTIFIER,		
		DataRiferimento DATETIME		
	)

				SET @sSQL=''
	SET  @sSQL = 'INSERT INTO #tmpIDScheda(IDScheda,DataRiferimento) 
					SELECT '

			IF (@bCopiaPrecedenteSelezione=0) 
	BEGIN
		SET  @sSQL = @sSQL + ' TOP 1 '
	END

	SET  @sSQL = @sSQL + 'ID, 
						CASE 
							WHEN ''' + ISNULL(@sCodModalitaCopiaPrecedente,'') + ''' =''CREA'' THEN DataCreazione
							ELSE ISNULL(DataUltimaModifica,DataCreazione)
						END AS DataRiferimento
					FROM T_MovSchede M WITH (NOLOCK,INDEX (IX_Paziente)) 
					WHERE '
						
	SET  @sSQL = @sSQL + '
	 
						(M.IDPaziente=''' + CONVERT(VARCHAR(50),@uIDPaziente) + '''
							OR
														M.IDPaziente IN 
									(SELECT IDPazienteVecchio
										FROM T_PazientiAlias WITH (NOLOCK)
										WHERE 
										IDPaziente IN 
											(SELECT IDPaziente
												FROM T_PazientiAlias  WITH (NOLOCK)
												WHERE IDPazienteVecchio=''' + CONVERT(VARCHAR(50),@uIDPaziente) + '''
											)
									)
						)			
						AND
						M.CodScheda=''' +  ISNULL(@sCodScheda,'') + ''' AND '

		IF @bIgnoraVersione=0
	BEGIN
		SET  @sSQL = @sSQL + ' M.Versione='  + CONVERT(VARCHAR(20), ISNULL(@nVersione,0)) + ' AND '
	END  
								
	SET  @sSQL = @sSQL + '
					M.Storicizzata=0 AND
					M.CodStatoScheda NOT IN (''CA'',''AN'')

										AND M.ID NOT IN (SELECT IDDestinazione FROM T_MovTranscodifiche WITH (NOLOCK)
								     WHERE CodEntita=''SCH'' AND IDDestinazione=M.ID) '

		IF @uIDEpisodio IS NOT NULL
	BEGIN
		SET  @sSQL = @sSQL + ' AND M.IDEpisodio=''' + CONVERT(VARCHAR(50),@uIDEpisodio) + ''''
	END
	
	
	IF ISNULL(@sCodEntita,'') <> ''
	BEGIN
		SET  @sSQL = @sSQL + ' AND M.CodEntita=''' + ISNULL(@sCodEntita,'') + ''''
	END
	
			IF  ISNULL(@sCodModalitaCopiaPrecedente,'') = 'CREA'
		BEGIN		
				SET  @sSQL = @sSQL + '
						ORDER BY 								  
							DataCreazione DESC						
						'
		END
	ELSE
				BEGIN	
				SET  @sSQL = @sSQL + '
						ORDER BY 								  
							DataUltimaModifica DESC						
						'	
		END


									PRINT @sSQL
	EXEC (@sSQL)

					
	DECLARE @sCodSchedaCopia AS VARCHAR(20)

	DECLARE cSchedeCopia CURSOR
	FOR 
		SELECT CodSchedaCopia 
		FROM T_SchedeCopia 
		WHERE CodScheda = ISNULL(@sCodScheda,'')
	
	OPEN cSchedeCopia
	
	FETCH NEXT FROM cSchedeCopia 
	INTO @sCodSchedaCopia

	WHILE @@FETCH_STATUS = 0
	BEGIN
			SET @sSQL=''
			SET  @sSQL = 'INSERT INTO #tmpIDScheda(IDScheda,DataRiferimento)
							SELECT TOP 1 
								ID, 
								CASE 
								WHEN ''' + ISNULL(@sCodModalitaCopiaPrecedente,'') + ''' =''CREA'' THEN DataCreazione
									ELSE ISNULL(DataUltimaModifica,DataCreazione)
								END AS DataRiferimento
							FROM T_MovSchede M WITH (NOLOCK,INDEX (IX_Paziente)) 
							WHERE '
						
			SET  @sSQL = @sSQL + '
	 
								(M.IDPaziente=''' + CONVERT(VARCHAR(50),@uIDPaziente) + '''
									OR
																		M.IDPaziente IN 
											(SELECT IDPazienteVecchio
												FROM T_PazientiAlias
												WHERE 
												IDPaziente IN 
													(SELECT IDPaziente
														FROM T_PazientiAlias
														WHERE IDPazienteVecchio=''' + CONVERT(VARCHAR(50),@uIDPaziente) + '''
													)
											)
								)			
								AND
								M.CodScheda =''' +  ISNULL(@sCodSchedaCopia,'') + ''' AND '	
								
			SET  @sSQL = @sSQL + '
							M.Storicizzata=0 AND
							M.CodStatoScheda NOT IN (''CA'',''AN'')

														AND M.ID NOT IN (SELECT IDDestinazione FROM T_MovTranscodifiche  WITH (NOLOCK)
											 WHERE CodEntita=''SCH'' AND IDDestinazione=M.ID) '

						IF @uIDEpisodio IS NOT NULL
			BEGIN
				SET  @sSQL = @sSQL + ' AND M.IDEpisodio=''' + CONVERT(VARCHAR(50),@uIDEpisodio) + ''''
			END
	
			IF ISNULL(@sCodEntita,'') <> ''
			BEGIN
				SET  @sSQL = @sSQL + ' AND M.CodEntita=''' + ISNULL(@sCodEntita,'') + ''''
			END
	
						IF  ISNULL(@sCodModalitaCopiaPrecedente,'') = 'CREA'
				BEGIN		
						SET  @sSQL = @sSQL + '
								ORDER BY 								  
									DataCreazione DESC						
								'
				END
			ELSE
								BEGIN	
						SET  @sSQL = @sSQL + '
								ORDER BY 								  
									DataUltimaModifica DESC						
								'	
				END


																								
						EXEC (@sSQL)

			FETCH NEXT FROM cSchedeCopia 
			INTO @sCodSchedaCopia
	END 	
	CLOSE cSchedeCopia
    DEALLOCATE cSchedeCopia

	
	
	
	SET @sSQL='	SELECT T.IDScheda,
					   T.DataRiferimento,

					   S.CodTipoScheda,
					   TS.Descrizione AS DescrTipoScheda,
					   
					   M.CodScheda,
					   S.Descrizione AS DescrScheda,
					   M.DataCreazione,
					   M.CodUtenteRilevazione,
					   LC.Descrizione AS DescrUtenteRilevazione,

					   M.DataUltimaModifica,
					   M.CodUtenteUltimaModifica,
					   LM.Descrizione AS DescrUltimaModifica,

					   M.AnteprimaRTF

				 FROM #tmpIDScheda T
					LEFT JOIN T_MovSchede M WITH (NOLOCK)
						ON (T.IDScheda=M.ID)
					LEFT JOIN T_Login LC
						ON (M.CodUtenteRilevazione=LC.Codice)
					LEFT JOIN T_Login LM
						ON (M.CodUtenteUltimaModifica=LM.Codice)
					LEFT JOIN T_Schede S
						ON (M.CodScheda=S.Codice)
					LEFT JOIN T_TipoScheda AS TS
						ON (S.CodTipoScheda=TS.Codice)
					 '
				 

	IF (@bCopiaPrecedenteSelezione=0)
		SET @sSQL=@sSQL +'	ORDER BY T.DataRiferimento ASC'
	ELSE
		SET @sSQL= @sSQL +'	ORDER BY T.DataRiferimento DESC'	 
	
		EXEC (@sSQL)
	RETURN 0	 	
			
END