CREATE PROCEDURE [dbo].[MSP_SelMovPrescrizioniTempi](@xParametri XML)
AS
BEGIN
	

									
		DECLARE @uIDPrescrizione AS UNIQUEIDENTIFIER
	DECLARE @uIDPrescrizioneTempi AS UNIQUEIDENTIFIER
	
	DECLARE @sCodStatoPrescrizioneTempi AS VARCHAR(20)
	DECLARE @sCodUtenteRilevazione AS VARCHAR(20)
	
	DECLARE @dDataInizio AS DATETIME
	DECLARE @dDataFine AS DATETIME
	DECLARE @bDatiEstesi AS Bit		
	DECLARE @sCodRuolo AS VARCHAR(20)
	
	DECLARE @nNumRighe AS INTEGER
								
		DECLARE @sGUID AS VARCHAR(50)
	DECLARE @uGUID AS UNIQUEIDENTIFIER
	DECLARE @sDataTmp AS VARCHAR(20)	
	DECLARE @nTemp AS INTEGER
	
	DECLARE @bInserisci AS  BIT
	DECLARE @bModifica AS  BIT
	DECLARE @bCancella AS  BIT
	DECLARE @bValida AS  BIT	
	DECLARE @bAnnulla AS  BIT	
	DECLARE @bTaskInfermieristici AS BIT
		
		DECLARE @xTimeStamp AS XML	
		
		DECLARE @xLogPrima AS XML
	DECLARE @xLogDopo AS XML
	DECLARE @xTemp AS XML
	DECLARE @xParLog AS XML

			SET @sCodRuolo=(SELECT TOP 1 ValoreParametro.CodRuolo.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/TimeStamp/CodRuolo') as ValoreParametro(CodRuolo))
	SET @sCodRuolo=ISNULL(@sCodRuolo,'')
						  					
						  					
		SET @nTemp=(SELECT COUNT(*) FROM T_AssRuoliModuli
				WHERE CodModulo='Prescr_Inserisci'	
				AND CodRuolo=@sCodRuolo)
	IF @nTemp>=1 
		SET @bInserisci=1
	ELSE
		SET @bInserisci=0	
	
		SET @nTemp=(SELECT COUNT(*) FROM T_AssRuoliModuli
				WHERE CodModulo='Prescr_Modifica'	
				AND CodRuolo=@sCodRuolo)
	IF @nTemp>=1 
		SET @bModifica=1
	ELSE
		SET @bModifica=0	
				
		SET @nTemp=(SELECT COUNT(*) FROM T_AssRuoliModuli
				WHERE CodModulo='Prescr_Cancella'	
				AND CodRuolo=@sCodRuolo)
	IF @nTemp>=1 
		SET @bCancella=1
	ELSE
		SET @bCancella=0	
		
		SET @nTemp=(SELECT COUNT(*) FROM T_AssRuoliModuli
				WHERE CodModulo='Prescr_Valida'	
				AND CodRuolo=@sCodRuolo)
	IF @nTemp>=1 
		SET @bValida=1
	ELSE
		SET @bValida=0	
	
		SET @nTemp=(SELECT COUNT(*) FROM T_AssRuoliModuli
				WHERE CodModulo='Prescr_Annulla'	
				AND CodRuolo=@sCodRuolo)
	IF @nTemp>=1 
		SET @bAnnulla=1
	ELSE
		SET @bAnnulla=0	
		
			SET @nTemp=(SELECT COUNT(*) FROM T_AssRuoliModuli
				WHERE CodModulo='WorkL_Menu'	
				AND CodRuolo=@sCodRuolo)
	IF @nTemp>=1 
		SET @bTaskInfermieristici=1
	ELSE
		SET @bTaskInfermieristici=0 	
		
			
		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDPrescrizione.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDPrescrizione') as ValoreParametro(IDPrescrizione))
	IF 	ISNULL(@sGUID,'') <> '' 
			SET @uIDPrescrizione=CONVERT(UNIQUEIDENTIFIER,	@sGUID)		
	
		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDPrescrizioneTempi.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDPrescrizioneTempi') as ValoreParametro(IDPrescrizioneTempi))
	IF 	ISNULL(@sGUID,'') <> '' 
			SET @uIDPrescrizioneTempi=CONVERT(UNIQUEIDENTIFIER,	@sGUID)				

		SET @sCodStatoPrescrizioneTempi=''
	SELECT	@sCodStatoPrescrizioneTempi =   @sCodStatoPrescrizioneTempi	 +
														CASE 
								WHEN  @sCodStatoPrescrizioneTempi	='' THEN ''
								ELSE  ','
							END + ''''
						  + ValoreParametro.CodStatoPrescrizioneTempi.value('.','VARCHAR(1800)')
						  + ''''
						 FROM @xParametri.nodes('/Parametri/CodStatoPrescrizioneTempi') as ValoreParametro(CodStatoPrescrizioneTempi)
						 
	SET @sCodStatoPrescrizioneTempi=LTRIM(RTRIM(@sCodStatoPrescrizioneTempi))
	IF	@sCodStatoPrescrizioneTempi='''''' SET @sCodStatoPrescrizioneTempi=''
	SET @sCodStatoPrescrizioneTempi=UPPER(@sCodStatoPrescrizioneTempi)

		SET @bDatiEstesi=(SELECT TOP 1 ValoreParametro.DatiEstesi.value('.','bit')
					  FROM @xParametri.nodes('/Parametri/DatiEstesi') as ValoreParametro(DatiEstesi))
	SET @bDatiEstesi=ISNULL(@bDatiEstesi,0)
								
		SET @sDataTmp=(SELECT TOP 1 ValoreParametro.DataInizio.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/DataInizio') as ValoreParametro(DataInizio))					  
	SET @sDataTmp=ISNULL(@sDataTmp,'')
	

	IF @sDataTmp<> '' 
		BEGIN			
						SET @sDataTmp= SUBSTRING(@sDataTmp,4,2) + '-' 
						+ LEFT(@sDataTmp,2) + '-' +
						+ SUBSTRING(@sDataTmp,7,4) +
						' ' + RIGHT(@sDataTmp,5)
			IF ISDATE(@sDataTmp)=1
				SET	@dDataInizio =CONVERT(DATETIME,@sDataTmp,120)						
			ELSE
				SET	@dDataInizio  =NULL			
		END

		SET @sDataTmp=(SELECT TOP 1 ValoreParametro.DataFine.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/DataFine') as ValoreParametro(DataFine))					  
	SET @sDataTmp=ISNULL(@sDataTmp,'')
	
	IF @sDataTmp<> '' 
		BEGIN			
						SET @sDataTmp= SUBSTRING(@sDataTmp,4,2) + '-' 
						+ LEFT(@sDataTmp,2) + '-' +
						+ SUBSTRING(@sDataTmp,7,4) +
						' ' + RIGHT(@sDataTmp,5)
			IF ISDATE(@sDataTmp)=1
				SET	@dDataFine=CONVERT(DATETIME,@sDataTmp,120)						
			ELSE
				SET	@dDataFine  =NULL			
		END

		SET @nNumRighe=(SELECT	TOP 1 ValoreParametro.NumRighe.value('.','INTEGER')
								FROM @xParametri.nodes('/Parametri/NumRighe') as ValoreParametro(NumRighe))	
	SET @nNumRighe=ISNULL(@nNumRighe,0)
						
		SET @sCodUtenteRilevazione=(SELECT TOP 1 ValoreParametro.CodUtenteRilevazione.value('.','VARCHAR(100)')
					  FROM @xParametri.nodes('/Parametri/TimeStamp/CodLogin') as ValoreParametro(CodUtenteRilevazione))	
	SET @sCodUtenteRilevazione=ISNULL(@sCodUtenteRilevazione,'')
	
	    SET @xTimeStamp=(SELECT TOP 1 ValoreParametro.TS.query('.')
					  FROM @xParametri.nodes('/Parametri/TimeStamp') as ValoreParametro(TS))
	

					
				
				
							   
		DECLARE @sSQL AS VARCHAR(MAX)
	DECLARE @sWhere AS VARCHAR(Max)
	DECLARE @sTmp AS VARCHAR(Max)
	DECLARE @gIDSessione AS UNIQUEIDENTIFIER
	
		SET @gIDSessione=NEWID()
	
	SET @sSQL='		INSERT INTO T_TmpFiltriPrescrizioniTempi(IDSessione,IDPrescrizioneTempi)					
					SELECT '

		IF @nNumRighe<> 0  SET @sSQL= @sSQL + ' TOP ' + convert(varchar(10),@nNumRighe)+  ' ' 					
	
	SET @sSQL=@sSQL +				
					'	'''  + convert(varchar(50),@gIDSessione) + ''' AS IDSessione,	'
							+ ' M.ID AS IDPrescrizioneTempi	
					FROM 
						T_MovPrescrizioniTempi	M											  							 
					'

				
	SET @sWhere=''									

		IF @uIDPrescrizione IS NOT NULL
		BEGIN
			SET @sTmp= ' AND M.IDPrescrizione=''' + convert(varchar(50),@uIDPrescrizione) +''''
			SET @sWhere= @sWhere + @sTmp
		END	
					
		IF @uIDPrescrizioneTempi IS NOT NULL
		BEGIN
			SET @sTmp= ' AND M.ID=''' + convert(varchar(50),@uIDPrescrizioneTempi) +''''
			SET @sWhere= @sWhere + @sTmp								
		END	
				
	IF @sCodStatoPrescrizioneTempi NOT IN ('')
		BEGIN				
			SET @sTmp=  ' AND 			
							 M.CodStatoPrescrizioneTempi IN ('+ @sCodStatoPrescrizioneTempi + ')
						'  				
			SET @sWhere= @sWhere + @sTmp
		END

					
	SET @sTmp=  ' AND 			
					 M.CodStatoPrescrizioneTempi <> ''CA''
				'  	
					
	IF ISNULL(@sTmp,'') <> '' 		
		SET @sWhere= @sWhere + @sTmp	
						

	IF ISNULL(@sWhere,'')<> ''
	BEGIN
	
		SET @sSQL=@sSQL +' WHERE ' + RIGHT(@sWhere,len(@sWhere)-5)
	END	
		 	
	PRINT @sSQL
		 	
	EXEC (@sSQL)
	
							
	SET @sSQL='	SELECT							
					M.ID,
					M.IDPrescrizione,					
					M.CodStatoPrescrizioneTempi,
					SP.Descrizione AS DescrStatoPrescrizione,
					M.CodTipoPrescrizioneTempi,
					CASE 
						WHEN M.CodStatoPrescrizioneTempi=''VA'' THEN M.DataUltimaModifica
						ELSE NULL
					END AS  DataValidazione,
					LV.Descrizione AS DescrUtenteValidazione,
					
					CASE 
						WHEN M.CodStatoPrescrizioneTempi=''SS'' THEN M.DataSospensione
						ELSE NULL
					END AS  DataSospensione,
					LS.Descrizione AS DescrUtenteSospensione,
					
					M.DataOraInizio, M.DataOraFine,
					M.AlBisogno, M.Durata, M.Continuita,
					M.PeriodicitaGiorni, M.PeriodicitaOre, M.PeriodicitaMinuti,				
					M.CodUtenteRilevazione AS CodUtente,
					L.Descrizione AS DescrUtente,
					
					
					M.DataEvento,
					
					
					M.Posologia,
					
										LTRIM(
					
						CASE
														WHEN ISNULL(Manuale,0) =1 THEN ''Manuale''
							
														WHEN ISNULL(AlBisogno,0) =1 THEN ''Al Bisogno''
							
														WHEN DataOraInizio IS NOT NULL THEN													
									 CASE
											WHEN (ISNULL(PeriodicitaGiorni,0)<>0 OR ISNULL(PeriodicitaOre,0)<>0 OR ISNULL(PeriodicitaMinuti,0)<> 0) 
												  OR PR.CodTipoProtocollo=''ORA''											
												THEN ''Da: ''
											ELSE ''''
									 END 
									 + CONVERT(varchar(10),DataOraInizio,105) + '' '' + CONVERT(varchar(5),DataOraInizio,14)  
									+ CASE
											WHEN DataOraFine is not null THEN 										
													CASE 
														WHEN DataOraInizio<> DataOraFine AND CONVERT(varchar(10),DataOraInizio,120) = CONVERT(varchar(10),DataOraFine,120)  
															AND ((ISNULL(PeriodicitaGiorni,0)<>0 OR ISNULL(PeriodicitaOre,0)<>0 OR ISNULL(PeriodicitaMinuti,0) <>0)
																 OR PR.CodTipoProtocollo=''ORA''
																)	
																																THEN  CHAR(13) + CHAR(10) + ''A:  '' + CONVERT(varchar(5),DataOraFine,14)
														WHEN DataOraInizio<> DataOraFine  
															AND ((ISNULL(PeriodicitaGiorni,0)<>0 OR ISNULL(PeriodicitaOre,0)<>0 OR ISNULL(PeriodicitaMinuti,0) <>0)
																  OR PR.CodTipoProtocollo=''ORA''
																 ) 	
																																THEN CHAR(13) + CHAR(10) +  ''A:  '' +  CONVERT(varchar(10),DataOraFine,105) + '' '' + CONVERT(varchar(5),DataOraFine,14)	
														ELSE ''''
													END
											ELSE ''''
										  END
							ELSE ''''
						END
						+
						
												 CASE 
							WHEN ISNULL(Manuale,0) =1 THEN ''''
							WHEN  ISNULL(PeriodicitaGiorni,0)<>0 OR ISNULL(PeriodicitaOre,0)<>0 OR ISNULL(PeriodicitaMinuti,0)<>0
								THEN CHAR(10) + CHAR(13) + ''Periodicità : '' +
										 										 CASE 
											WHEN ISNULL(PeriodicitaGiorni,0)<>0 THEN CONVERT(varchar(20),PeriodicitaGiorni) + '' gg '' 
											ELSE ''''
										 END	
										+
																				 CASE 
											WHEN ISNULL(PeriodicitaOre,0)<>0 THEN CONVERT(varchar(20),PeriodicitaOre) + '' hh '' 
											ELSE ''''		
										 END
										+
																				 CASE 
											WHEN ISNULL(PeriodicitaMinuti,0)<>0 THEN CONVERT(varchar(20),PeriodicitaMinuti) + '' min '' 
											ELSE ''''		
										 END 
							ELSE ''''
						 END	
						 
						+
						
												 CASE 
							WHEN ISNULL(Manuale,0) =1 THEN ''''
							WHEN ISNULL(M.Continuita,0)<>0 THEN 
								 CHAR(13) + CHAR(10) + ''Continua: '' + CONVERT(varchar(20),ISNULL(M.Durata,0)) + '' min''
							ELSE ''''		
						 END)  						 					 
					AS DescrTempo,
					
					''E:'' + CONVERT(VARCHAR(10),ISNULL(PRETMPTASK.TDPNumTaskErogati,0)) +
					'' P:'' + CONVERT(VARCHAR(10),ISNULL(PRETMPTASK.TDPNumTaskProgrammati,0)) +
					'' A:'' + CONVERT(VARCHAR(10),ISNULL(PRETMPTASK.TDPNumTaskAnnullati,0)) 
					AS EPA,
					CASE 
							WHEN PRETMPTASK.TDPDataUltimaErogazione IS NOT NULL THEN 
									''Ultima Somm.: '' + CONVERT(varchar(10),TDPDataUltimaErogazione,105) + '' '' + CONVERT(varchar(5),TDPDataUltimaErogazione,14)
							ELSE ''''
					END AS US,
					CASE 
								WHEN PRETMPTASK.TDPDataProssimaProgrammazione IS NOT NULL THEN 
										''Prossima Pianificata: '' + CONVERT(varchar(10),TDPDataProssimaProgrammazione,105) + '' '' + CONVERT(varchar(5),TDPDataProssimaProgrammazione,14)
								ELSE '''' 							
					END AS PP,
					
					CASE 
								WHEN PRETMPTASK.TDPDataUltimaProgrammazione IS NOT NULL THEN 
										''Ultima Pianificata: '' + CONVERT(varchar(10),TDPDataUltimaProgrammazione,105) + '' '' + CONVERT(varchar(5),TDPDataUltimaProgrammazione,14)
								ELSE '''' 							
					END AS UP,
					
					CASE 
							WHEN PRETMPTASK.TDPDataUltimaErogazione IS NOT NULL THEN 
									''U.S.: '' + CONVERT(varchar(10),TDPDataUltimaErogazione,105) + '' '' + CONVERT(varchar(5),TDPDataUltimaErogazione,14)	+ CHAR(13) + CHAR(10)
							ELSE ''''
					END 	
					+
					''E:'' + CONVERT(VARCHAR(10),ISNULL(PRETMPTASK.TDPNumTaskErogati,0)) +
					'' P:'' + CONVERT(VARCHAR(10),ISNULL(PRETMPTASK.TDPNumTaskProgrammati,0)) +
					'' A:'' + CONVERT(VARCHAR(10),ISNULL(PRETMPTASK.TDPNumTaskAnnullati,0)) 
					 AS DescrTask,
					
					CASE 
								WHEN PRETMPTASK.TDPDataProssimaProgrammazione IS NOT NULL THEN 
										''P.P.: '' + CONVERT(varchar(10),TDPDataProssimaProgrammazione,105) + '' '' + CONVERT(varchar(5),TDPDataProssimaProgrammazione,14)
								ELSE '''' 							
						END  + CHAR(13) + CHAR(10) 
					+
						CASE 
								WHEN PRETMPTASK.TDPDataUltimaProgrammazione IS NOT NULL THEN 
										''U.P.: '' + CONVERT(varchar(10),TDPDataUltimaProgrammazione,105) + '' '' + CONVERT(varchar(5),TDPDataUltimaProgrammazione,14)
								ELSE '''' 							
						END 
					AS  DescrTask2,
					 
					ISNULL(PRETMPTASK.NumTaskProgrammati,0) AS NumTaskProgrammati,
					
										 CASE 
							WHEN ISNULL(Manuale,0) =1 THEN ''''
							ELSE PR.Descrizione 
					 END As DescrProtocollo,					
					CodProtocollo,									
					ISNULL(Manuale,0) As  Manuale,
					TempiManuali, 
										
					CONVERT(INTEGER,' + CONVERT(CHAR(1),@bValida) + ')
					 &
						CASE 
							WHEN CodStatoPrescrizioneTempi=''IC'' THEN 1
							ELSE 0
						END 
					&
						(
							CASE 
								WHEN ISNULL(MP.CodStatoContinuazione,''AP'')=''CH'' THEN 0
								ELSE 1
							END
						)		
					AS  PermessoDaValidare,						
					CONVERT(INTEGER,' + CONVERT(CHAR(1),@bModifica) + ') 
					 &
						(CASE 
							WHEN CodStatoPrescrizioneTempi =''IC'' THEN 1
							ELSE 0
						END)
						
					&
						(
							CASE 
								WHEN ISNULL(MP.CodStatoContinuazione,''AP'')=''CH'' THEN 0
								ELSE 1
							END
						)						
					AS PermessoModifica,
					CONVERT(INTEGER,' + CONVERT(CHAR(1),@bAnnulla) + ') 
					 &
						CASE 
							WHEN CodStatoPrescrizioneTempi=''VA'' THEN 1
							ELSE 0
						END 
																																				AS PermessoAnnulla,
					CONVERT(INTEGER,' + CONVERT(CHAR(1),@bCancella) + ') 
					 & (
						(CASE 
							WHEN CodStatoPrescrizioneTempi =''IC'' THEN 1
							ELSE 0
						 END) 
						|
						  (CASE 
								WHEN ISNULL(PRETMPTASK.NumTaskErogati,0)=0  THEN 1
								ELSE 0
							END
						   & 
							CASE 
								WHEN CodStatoPrescrizioneTempi IN (''VA'',''SS'') THEN 1
								ELSE 0
							END
						  )	
						)
					AS PermessoCancella,

					CONVERT(INTEGER,' + CONVERT(CHAR(1),@bInserisci) + ') 
					&
						(
							CASE 
								WHEN ISNULL(MP.CodStatoContinuazione,''AP'')=''CH'' THEN 0
								ELSE 1
							END
						)		
					AS PermessoCopia,
					IDPrescrizioneTempiOrigine'

	SET @sSQL = @sSQL +					
					' FROM 						
						T_TmpFiltriPrescrizioniTempi TMP WITH (NOLOCK)
							INNER JOIN
								T_MovPrescrizioniTempi AS M WITH (NOLOCK)
									ON TMP.IDPrescrizioneTempi=M.ID		
							INNER JOIN
								T_MovPrescrizioni AS MP WITH (NOLOCK)
									ON M.IDPrescrizione=MP.ID							
							LEFT JOIN T_StatoPrescrizioneTempi SP WITH (NOLOCK)
								ON M.CodStatoPrescrizioneTempi=SP.Codice
							LEFT JOIN T_Protocolli PR WITH (NOLOCK)
								ON M.CodProtocollo=PR.Codice
							LEFT JOIN T_Login L WITH (NOLOCK)
								ON (M.CodUtenteRilevazione=L.Codice) 
							LEFT JOIN T_Login LV WITH (NOLOCK)
								ON (M.CodUtenteValidazione=LV.Codice)	
							LEFT JOIN T_Login LS WITH (NOLOCK)
								ON (M.CodUtenteSospensione=LS.Codice)		
							LEFT JOIN Q_SelPrescrizioniTempiTask PRETMPTASK WITH (NOLOCK)
								ON 	PRETMPTASK.IDPrescrizioneTempi = M.IDString		
										'									
	
		SET @sSQL=@sSQL + ' WHERE TMP.IDSessione=''' + convert(varchar(50),@gIDSessione) +''''
	
	SET @sSQL=@sSQL + ' ORDER BY M.DataEvento DESC '
		
	PRINT @sSQL					
	EXEC (@sSQL)												   
							    	
	
					
		IF @bDatiEstesi=1
		BEGIN
			SET @sSQL='SELECT DISTINCT
							M.CodStatoPrescrizioneTempi AS Codice,
							T.Descrizione AS Descrizione
						FROM 
							T_TmpFiltriPrescrizioniTempi TMP
								INNER JOIN T_MovPrescrizioniTempi	M	
										ON (TMP.IDPrescrizioneTempi=M.ID)																													
								INNER JOIN T_StatoPrescrizione T
									ON (M.CodStatoPrescrizioneTempi=T.Codice)													
						'
			
						SET @sSQL=@sSQL + ' WHERE TMP.IDSessione=''' + convert(varchar(50),@gIDSessione) +''''
							
			EXEC (@sSQL)	
		END 
		
				
		
					
					
		DELETE FROM T_TmpFiltriPrescrizioniTempi 
	WHERE IDSessione=@gIDSessione 
									
	
	RETURN 0
END