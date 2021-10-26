CREATE PROCEDURE [dbo].[MSP_SelMovPrescrizioniTempiProsegui](@xParametri XML)
AS
BEGIN
	

									
		DECLARE @uIDEpisodio AS UNIQUEIDENTIFIER	
	DECLARE @dDataInizio AS DATETIME
	DECLARE @dDataFine AS DATETIME
		
	DECLARE @sCodUtenteRilevazione AS VARCHAR(20)
	

	
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
		
	    DECLARE @xTimeStampBase AS XML	
	DECLARE @xTimeStamp AS XML	
		
		DECLARE @xLogPrima AS XML
	DECLARE @xLogDopo AS XML
	DECLARE @xTemp AS XML
	DECLARE @xParLog AS XML

			SET @sCodRuolo=(SELECT TOP 1 ValoreParametro.CodRuolo.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/TimeStamp/CodRuolo') as ValoreParametro(CodRuolo))
	SET @sCodRuolo=ISNULL(@sCodRuolo,'')
						  						
		
			SET @sGUID=(SELECT TOP 1 ValoreParametro.IDEpisodio.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDEpisodio') as ValoreParametro(IDEpisodio))
	IF 	ISNULL(@sGUID,'') <> '' 
			SET @uIDEpisodio=CONVERT(UNIQUEIDENTIFIER,	@sGUID)	
													
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
	DECLARE @sCodTipoTaskDaPrescrizione AS VARCHAR(20)
	
	
	
		SET @gIDSessione=NEWID()
	
	SET @sSQL='		INSERT INTO T_TmpFiltriPrescrizioniTempi(IDSessione,IDPrescrizioneTempi)					
					SELECT '

		IF @nNumRighe<> 0  SET @sSQL= @sSQL + ' TOP ' + convert(varchar(10),@nNumRighe)+  ' ' 					
	
	SET @sSQL=@sSQL +				
					'	'''  + convert(varchar(50),@gIDSessione) + ''' AS IDSessione,	'
							+ ' M.ID AS IDPrescrizioneTempi	
					FROM 
						T_MovPrescrizioniTempi	M
							INNER JOIN T_MovPrescrizioni P
								ON M.IDPrescrizione=P.ID 
							INNER JOIN T_TipoPrescrizione TP
								ON P.CodTipoPrescrizione=TP.Codice
							INNER JOIN 
								Q_SelPrescrizioniTask PRETASK
								   ON 	(PRETASK.IDPrescrizione = M.IDPrescrizione)	
					'

				
	SET @sWhere=''										

				
	

		SET @sCodTipoTaskDaPrescrizione=(SELECT Valore FROM T_Config WHERE ID=33)

		IF @uIDEpisodio IS NOT NULL
		BEGIN
			SET @sTmp= ' AND P.IDEpisodio=''' + convert(varchar(50),@uIDEpisodio) +''''
			SET @sWhere= @sWhere + @sTmp								
		END	


		SET @sTmp= ' AND M.CodStatoPrescrizioneTempi NOT IN (''CA'')'
	SET @sWhere= @sWhere + @sTmp
	
		SET @sTmp= ' AND P.CodStatoPrescrizione NOT IN (''CA'')'
	SET @sWhere= @sWhere + @sTmp
	
		SET @sTmp= ' AND P.CodStatoContinuazione NOT IN (''CH'')'
	SET @sWhere= @sWhere + @sTmp
	
		SET @sTmp= ' AND ISNULL(M.CodTipoPrescrizioneTempi,'''') NOT IN (''AB'',''PG'')'
	SET @sWhere= @sWhere + @sTmp
	
		SET @sTmp= ' AND ISNULL(M.Manuale,0)=0'
	SET @sWhere= @sWhere + @sTmp
	
	
		SET @sTmp= ' AND ISNULL(TP.NonProseguibile,0)=0'
	SET @sWhere= @sWhere + @sTmp
																
		SET @sTmp= ' AND M.ID IN
						(SELECT IDGruppo
						 FROM T_MovTaskInfermieristici TI
						 WHERE 
							TI.CodTipoTaskInfermieristico=''' + @sCodTipoTaskDaPrescrizione + '''
							AND TI.CodStatoTaskInfermieristico <> ''CA''
							AND ISNULL(TI.DataErogazione,TI.DataProgrammata) >= CONVERT(datetime,'''  + convert(varchar(20),@dDataInizio,120) +''',120) 
							AND ISNULL(TI.DataErogazione,TI.DataProgrammata) <= CONVERT(datetime,'''  + convert(varchar(20),@dDataFine,120) +''',120)
							AND TI.IDEpisodio ='''+ convert(varchar(50),@uIDEpisodio) +'''
						)		
				'	
	SET @sWhere= @sWhere + @sTmp

					
		SET @sTmp= ' AND 
					CASE	
						WHEN PRETASK.DataUltimaProgrammazione IS NULL THEN PRETASK.DataUltimaErogazione
						WHEN PRETASK.DataUltimaErogazione IS NULL THEN PRETASK.DataUltimaProgrammazione
						ELSE
							CASE
								WHEN (PRETASK.DataUltimaProgrammazione > PRETASK.DataUltimaErogazione)  THEN PRETASK.DataUltimaProgrammazione
								ELSE PRETASK.DataUltimaErogazione
							END
					END >= CONVERT(datetime,'''  + convert(varchar(20),@dDataInizio,120) +''',120) AND
					CASE	
						WHEN PRETASK.DataUltimaProgrammazione IS NULL THEN PRETASK.DataUltimaErogazione
						WHEN PRETASK.DataUltimaErogazione IS NULL THEN PRETASK.DataUltimaProgrammazione
						ELSE
							CASE
								WHEN (PRETASK.DataUltimaProgrammazione > PRETASK.DataUltimaErogazione)  THEN PRETASK.DataUltimaProgrammazione
								ELSE PRETASK.DataUltimaErogazione
							END
					END <= CONVERT(datetime,'''  + convert(varchar(20),@dDataFine,120) +''',120)
				'
					
	SET @sWhere= @sWhere + @sTmp
				
		
	IF ISNULL(@sWhere,'')<> ''
		SET @sSQL=@sSQL +' WHERE ' + RIGHT(@sWhere,len(@sWhere)-5)			
	ELSE
		SET @sSQL=@sSQL +' WHERE 1=0'
		 	
	PRINT @sSQL
		 	
	EXEC (@sSQL)
	
							
	SET @sSQL='	SELECT							
					M.ID,
					M.IDPrescrizione,
					MS.AnteprimaTxt AS Terapia,
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
					AS Tempistica,
					'''' As BTN_ADD,
					convert(varchar(2000),'''') As [Nuova Tempistica],
					convert(varchar(2000),'''') As [Nuova Posologia],
					1 AS PermessoAggiungi,
					0 AS PermessoModifica,
					0 AS PermessoCancella,
					MS.AnteprimaRTF AS TerapiaRTF'

	SET @sSQL = @sSQL +					
					' FROM 						
						T_TmpFiltriPrescrizioniTempi TMP WITH (NOLOCK)
							INNER JOIN
								T_MovPrescrizioniTempi AS M WITH (NOLOCK)
									ON TMP.IDPrescrizioneTempi=M.ID		
							INNER JOIN
								T_MovPrescrizioni AS MP WITH (NOLOCK)
									ON M.IDPrescrizione=MP.ID
							LEFT JOIN T_Protocolli PR WITH (NOLOCK)
								ON M.CodProtocollo=PR.Codice
							LEFT JOIN	
																(SELECT IDEntita,AnteprimaTXT, AnteprimaRTF
								 FROM
									T_MovSchede 
								 WHERE CodEntita = ''PRF'' AND							
							  		   Storicizzata = 0
								) AS MS
								ON MS.IDEntita = MP.ID		
						'									
	
		SET @sSQL=@sSQL + ' WHERE TMP.IDSessione=''' + convert(varchar(50),@gIDSessione) +''''
	
	SET @sSQL=@sSQL + ' ORDER BY M.DataEvento DESC '
		
	PRINT @sSQL					
	EXEC (@sSQL)												   
							    			
				
	EXEC MSP_InsMovTimeStamp @xTimeStamp			
	
					
		DELETE FROM T_TmpFiltriPrescrizioniTempi 
	WHERE IDSessione=@gIDSessione 
									
	
	RETURN 0
END