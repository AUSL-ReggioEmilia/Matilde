CREATE PROCEDURE [dbo].[MSP_SelMovPrescrizioni](@xParametri XML)
AS
BEGIN
		
	
									
		DECLARE @uIDEpisodio AS UNIQUEIDENTIFIER	
	DECLARE @uIDPrescrizione AS UNIQUEIDENTIFIER	
	
	DECLARE @sCodViaSomministrazione AS VARCHAR(20)
	DECLARE @sCodTipoPrescrizione AS VARCHAR(20)
	DECLARE @sCodStatoPrescrizione AS VARCHAR(2000)
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
				WHERE CodModulo='Prescr_Valdia'	
				AND CodRuolo=@sCodRuolo)
	IF @nTemp>=1 
		SET @bValida=1
	ELSE
		SET @bValida=0	
	
			SET @nTemp=(SELECT COUNT(*) FROM T_AssRuoliModuli
				WHERE CodModulo='WorkL_Menu'	
				AND CodRuolo=@sCodRuolo)
	IF @nTemp>=1 
		SET @bTaskInfermieristici=1
	ELSE
		SET @bTaskInfermieristici=0 	
		
			
		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDEpisodio.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDEpisodio') as ValoreParametro(IDEpisodio))
	IF 	ISNULL(@sGUID,'') <> '' 
			SET @uIDEpisodio=CONVERT(UNIQUEIDENTIFIER,	@sGUID)	
	
		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDPrescrizione.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDPrescrizione') as ValoreParametro(IDPrescrizione))
	IF 	ISNULL(@sGUID,'') <> '' 
			SET @uIDPrescrizione=CONVERT(UNIQUEIDENTIFIER,	@sGUID)					
			
		SET @sCodViaSomministrazione=''
	SELECT	@sCodViaSomministrazione =  @sCodViaSomministrazione +
														CASE 
								WHEN @sCodViaSomministrazione='' THEN ''
								ELSE  ','
							END + ''''
						  + ValoreParametro.CodViaSomministrazione.value('.','VARCHAR(1800)')
						  + ''''
						 FROM @xParametri.nodes('/Parametri/CodViaSomministrazione') as ValoreParametro(CodViaSomministrazione)
						 
	SET @sCodViaSomministrazione=LTRIM(RTRIM(@sCodViaSomministrazione))
	IF	@sCodViaSomministrazione='''''' SET @sCodViaSomministrazione=''
	SET @sCodViaSomministrazione=UPPER(@sCodViaSomministrazione)

		SET @sCodTipoPrescrizione=''
	SELECT	@sCodTipoPrescrizione =  @sCodTipoPrescrizione +
														CASE 
								WHEN @sCodTipoPrescrizione='' THEN ''
								ELSE  ','
							END + ''''
						  + ValoreParametro.CodTipoPrescrizione.value('.','VARCHAR(1800)')
						  + ''''
						 FROM @xParametri.nodes('/Parametri/CodTipoPrescrizione') as ValoreParametro(CodTipoPrescrizione)
						 
	SET @sCodTipoPrescrizione=LTRIM(RTRIM(@sCodTipoPrescrizione))
	IF	@sCodTipoPrescrizione='''''' SET @sCodTipoPrescrizione=''
	SET @sCodTipoPrescrizione=UPPER(@sCodTipoPrescrizione)
	
		SET @sCodStatoPrescrizione=''
	SELECT	@sCodStatoPrescrizione =   @sCodStatoPrescrizione	 +
														CASE 
								WHEN  @sCodStatoPrescrizione	='' THEN ''
								ELSE  ','
							END + ''''
						  + ValoreParametro.CodStatoPrescrizione.value('.','VARCHAR(1800)')
						  + ''''
						 FROM @xParametri.nodes('/Parametri/CodStatoPrescrizione') as ValoreParametro(CodStatoPrescrizione)
						 
	SET @sCodStatoPrescrizione=LTRIM(RTRIM(@sCodStatoPrescrizione))
	IF	@sCodStatoPrescrizione='''''' SET @sCodStatoPrescrizione=''
	SET @sCodStatoPrescrizione=UPPER(@sCodStatoPrescrizione)

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
	
	SET @sSQL='		INSERT INTO T_TmpFiltriPrescrizioni(IDSessione,IDPrescrizione)					
					SELECT '

		IF @nNumRighe<> 0  SET @sSQL= @sSQL + ' TOP ' + convert(varchar(10),@nNumRighe)+  ' ' 					
	
	SET @sSQL=@sSQL +				
					'	'''  + convert(varchar(50),@gIDSessione) + ''' AS IDSessione,	'
							+ ' M.ID AS IDPrescrizione	
					FROM 
						T_MovPrescrizioni	M '

	IF @sCodStatoPrescrizione NOT IN ('')
		BEGIN
			SET @sSQL=@sSQL +
					' LEFT JOIN 
						Q_SelPrescrizioniTask AS PRETASK
							ON CONVERT(VARCHAR(50),M.ID)=PRETASK.IDPrescrizione
					'
			END

				
	SET @sWhere=''									

		IF @uIDEpisodio IS NOT NULL
		BEGIN
			SET @sTmp= ' AND M.IDEpisodio=''' + convert(varchar(50),@uIDEpisodio) +''''
			SET @sWhere= @sWhere + @sTmp								
		END	

		IF  @uIDPrescrizione	 IS NOT NULL
		BEGIN
						SET @sTmp= ' AND M.ID=''' + convert(varchar(50), @uIDPrescrizione	) +''''
			SET @sWhere= @sWhere + @sTmp								
		END	
		
		IF @sCodTipoPrescrizione NOT IN ('')
		BEGIN				
			SET @sTmp=  ' AND 			
							 M.CodTipoPrescrizione IN ('+ @sCodTipoPrescrizione + ')
						'  				
			SET @sWhere= @sWhere + @sTmp														
		END
		
		IF @sCodStatoPrescrizione NOT IN ('')
		BEGIN				
			SET @sTmp=  ' AND 			
							 PRETASK.CodStatoPrescrizioneCalc IN ('+ @sCodStatoPrescrizione + ')
						'  				
			SET @sWhere= @sWhere + @sTmp														
		END	
	
		IF @sCodViaSomministrazione NOT IN ('')
		BEGIN				
			SET @sTmp=  ' AND 			
							 M.CodViaSomministrazione IN ('+ @sCodViaSomministrazione + ')
						'  				
			SET @sWhere= @sWhere + @sTmp														
		END	
						
		IF @dDataInizio IS NOT NULL 
		BEGIN
			SET @sTmp= ' AND M.DataEvento >= CONVERT(datetime,'''  + convert(varchar(20),@dDataInizio,120) +''',120)'	
																																	SET @sWhere= @sWhere + @sTmp								
		END

		IF @dDataFine IS NOT NULL 
		BEGIN
			SET @sTmp= ' AND M.DataEvento <= CONVERT(datetime,'''  + convert(varchar(20),@dDataFine,120) +''',120)'							
			SET @sWhere= @sWhere + @sTmp								
		END
	
						
	SET @sTmp=  ' AND 			
					 M.CodStatoPrescrizione <> ''CA''
					 				  
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
					M.IDEpisodio, 
					M.IDTrasferimento, 
					PZ.IDPaziente, 
					TRASF.CodUA,
					M.CodViaSomministrazione,
					VS.Descrizione AS DescrViaSomministrazione,
					CONVERT(VARBINARY(MAX),NULL) AS Icona,
					CONVERT(VARBINARY(MAX),NULL) AS Icona2,
					M.CodTipoPrescrizione,
					TP.Descrizione AS DescrTipoPrescrizione,
					PRETASK.CodStatoPrescrizioneCalc AS CodStatoPrescrizione ,
					SP.Descrizione AS DescrStatoPrescrizione,
					SP.Colore AS ColoreStatoPrescrizione,
					ISNULL(M.CodStatoContinuazione,''AP'') AS CodStatoContinuazione,						
					M.DataEvento,
					CASE 
						WHEN CodStatoPrescrizione=''VA'' THEN M.DataUltimaModifica
						ELSE NULL
					END AS  DataValidazione,				
					M.CodUtenteRilevazione AS CodUtente,
					L.Descrizione AS DescrUtente,
					MS.IDScheda,
					MS.CodScheda,
					MS.Versione,
					MS.AnteprimaRTF, MS.AnteprimaTXT,
								
					''E:'' + CONVERT(VARCHAR(10),ISNULL(PRETASK.TDPNumTaskErogati,0)) +
					 '' P:'' + CONVERT(VARCHAR(10),ISNULL(PRETASK.TDPNumTaskProgrammati,0)) +
					 '' A:'' + CONVERT(VARCHAR(10),ISNULL(PRETASK.TDPNumTaskAnnullati,0))
					AS EPA,
					
					CASE 
							WHEN PRETASK.TDPDataUltimaErogazione IS NOT NULL THEN 
									''Ultima Somm.: '' + CONVERT(varchar(10),TDPDataUltimaErogazione,105) + '' '' + CONVERT(varchar(5),TDPDataUltimaErogazione,14)
							ELSE ''''
					END AS US,
					CASE 
								WHEN PRETASK.TDPDataProssimaProgrammazione IS NOT NULL THEN 
										''Prossima Pianificata: '' + CONVERT(varchar(10),TDPDataProssimaProgrammazione,105) + '' '' + CONVERT(varchar(5),TDPDataProssimaProgrammazione,14)
								ELSE '''' 							
					END AS PP,
					CASE 
								WHEN PRETASK.TDPDataUltimaProgrammazione IS NOT NULL THEN 
										''Ultima Pianificata: '' + CONVERT(varchar(10),TDPDataUltimaProgrammazione,105) + '' '' + CONVERT(varchar(5),TDPDataUltimaProgrammazione,14)
								ELSE '''' 							
					END AS UP,
						 							
					''E:'' + CONVERT(VARCHAR(10),ISNULL(PRETASK.TDPNumTaskErogati,0)) +
					 '' P:'' + CONVERT(VARCHAR(10),ISNULL(PRETASK.TDPNumTaskProgrammati,0)) +
					 '' A:'' + CONVERT(VARCHAR(10),ISNULL(PRETASK.TDPNumTaskAnnullati,0)) +					 					
					+ CHAR(13) + CHAR(10) +
				 	CASE 
							WHEN PRETASK.TDPDataUltimaErogazione IS NOT NULL THEN 
									''U.S.: '' + CONVERT(varchar(10),TDPDataUltimaErogazione,105) + '' '' + CONVERT(varchar(5),TDPDataUltimaErogazione,14)	+ CHAR(13) + CHAR(10)
							ELSE ''''
					END 						 
					AS DescrTask,
										
					CASE 
								WHEN PRETASK.TDPDataProssimaProgrammazione IS NOT NULL THEN 
										''P.P.: '' + CONVERT(varchar(10),TDPDataProssimaProgrammazione,105) + '' '' + CONVERT(varchar(5),TDPDataProssimaProgrammazione,14)
								ELSE '''' 							
						END  + CHAR(13) + CHAR(10) 
					+
						CASE 
								WHEN PRETASK.TDPDataUltimaProgrammazione IS NOT NULL THEN 
										''U.P.: '' + CONVERT(varchar(10),TDPDataUltimaProgrammazione,105) + '' '' + CONVERT(varchar(5),TDPDataUltimaProgrammazione,14)
								ELSE '''' 							
						END 
					AS  DescrTask2,
					
							
					''E:'' + CONVERT(VARCHAR(10),ISNULL(PRETASK.TDPNumTaskErogati,0)) +
					 '' P:'' + CONVERT(VARCHAR(10),ISNULL(PRETASK.TDPNumTaskProgrammati,0)) +
					 '' A:'' + CONVERT(VARCHAR(10),ISNULL(PRETASK.TDPNumTaskAnnullati,0)) 
					AS EPA,
										
					CASE 
							WHEN PRETASK.TDPDataUltimaErogazione IS NOT NULL THEN 
									''U.S.: '' + CONVERT(varchar(10),TDPDataUltimaErogazione,105) + '' '' + CONVERT(varchar(5),TDPDataUltimaErogazione,14)	+ CHAR(13) + CHAR(10)
							ELSE '''' 							
					END  				
					AS US,
									 	
					CASE 
							WHEN PRETASK.TDPDataProssimaProgrammazione IS NOT NULL THEN 
									''P.P.: '' + CONVERT(varchar(10),TDPDataProssimaProgrammazione,105) + '' '' + CONVERT(varchar(5),TDPDataProssimaProgrammazione,14)
							ELSE '''' 							
					END  
					AS PP,
					CASE 
							WHEN PRETASK.TDPDataUltimaProgrammazione IS NOT NULL THEN 
									''U.P.: '' + CONVERT(varchar(10),TDPDataUltimaProgrammazione,105) + '' '' + CONVERT(varchar(5),TDPDataUltimaProgrammazione,14)
							ELSE '''' 							
					END  AS UP,
			
					 
					CONVERT(INTEGER,
						CASE 
							WHEN TEMPI.QtaDaValidare >0 THEN 1 
							ELSE 0
						END
					) AS TempiDaValidare,	
					CONVERT(INTEGER,' + CONVERT(CHAR(1),@bModifica) + ') 					
					AS PermessoModifica,
										
					CONVERT(INTEGER,' + CONVERT(CHAR(1),@bModifica) + ') 
					& CONVERT(BIT,
						CASE 
							WHEN TEMPIVALIDATI.QtaValidati >0 THEN 0 
							ELSE 1
						END)
					AS PermessoModificaScheda,
					CONVERT(INTEGER,' + CONVERT(CHAR(1),@bTaskInfermieristici) + '
					
																																			
							) AS PermessoTaskInfermiristici,					
					CONVERT(INTEGER,' + CONVERT(CHAR(1),@bCancella) + ') 
					& CONVERT(BIT,
						CASE 
							WHEN TEMPIVALIDATI.QtaValidati >0 THEN 0 
							ELSE 1
						END
					) 
					AS PermessoCancella,
					CONVERT(INTEGER,' + CONVERT(CHAR(1),@bInserisci) + ') AS PermessoCopia,
					ISNULL(TP.PrescrizioneASchema,0) AS PrescrizioneASchema,
					ISNULL(TP.NonProseguibile,0) AS NonPreseguibile
					'

	SET @sSQL = @sSQL +					
					' FROM 						
						T_TmpFiltriPrescrizioni TMP
							INNER JOIN
								T_MovPrescrizioni AS M
								ON TMP.IDPrescrizione=M.ID
							INNER JOIN T_MovTrasferimenti TRASF
								ON M.IDTrasferimento = TRASF.ID
							LEFT JOIN T_ViaSomministrazione VS
								ON M.CodViaSomministrazione=VS.Codice
							LEFT JOIN T_MovPazienti PZ
								ON M.IDEpisodio = PZ.IDEpisodio							
							LEFT JOIN T_Login L
										ON (M.CodUtenteRilevazione=L.Codice)	
							LEFT JOIN 
											(SELECT ID As IDScheda, CodScheda,Versione,IDEntita,AnteprimaRTF,AnteprimaTXT
											 FROM
												T_MovSchede 
											 WHERE CodEntita=''PRF'' AND							
												Storicizzata=0  AND
												CodStatoScheda <> ''CA''
											) AS MS
										ON MS.IDEntita=M.ID
							LEFT JOIN 
											(SELECT IDPrescrizione,COUNT(*) AS QtaDaValidare
											 FROM T_MovPrescrizioniTempi
											 WHERE CodStatoPrescrizioneTempi=''IC''
											 GROUP BY IDPrescrizione
											 ) AS TEMPI
										ON M.ID=TEMPI.IDPrescrizione
							LEFT JOIN 
											(SELECT IDPrescrizione,COUNT(*) AS QtaValidati
											 FROM T_MovPrescrizioniTempi
											 WHERE CodStatoPrescrizioneTempi IN (''VA'',''SS'')
											 GROUP BY IDPrescrizione
											 ) AS TEMPIVALIDATI
										ON M.ID=TEMPIVALIDATI.IDPrescrizione										
							LEFT JOIN T_TipoPrescrizione TP 
								ON M.CodTipoPrescrizione = TP.Codice
								
							LEFT JOIN 
								Q_SelPrescrizioniTask AS PRETASK
									ON CONVERT(VARCHAR(50),M.ID)=PRETASK.IDPrescrizione								
							
							LEFT JOIN T_StatoPrescrizione SP
								ON PRETASK.CodStatoPrescrizioneCalc=SP.Codice
								
																					'
													
	
		SET @sSQL=@sSQL + ' WHERE TMP.IDSessione=''' + convert(varchar(50),@gIDSessione) +''''
	
	IF @uIDPrescrizione IS NULL
		BEGIN
			SET @sSQL=@sSQL + ' ORDER BY DataEvento DESC '
		END
		
	PRINT @sSQL					
	EXEC (@sSQL)												   
							    	
						
	IF @bDatiEstesi=1
		BEGIN
			SET @sSQL='SELECT DISTINCT
							M.CodViaSomministrazione AS Codice,
							T.Descrizione AS Descrizione
						FROM 
							T_TmpFiltriPrescrizioni TMP
								INNER JOIN T_MovPrescrizioni	M	
										ON (TMP.IDPrescrizione=M.ID)																													
								LEFT JOIN T_ViaSomministrazione T
									ON (M.CodViaSomministrazione=T.Codice)													
						'
			
						SET @sSQL=@sSQL + ' WHERE TMP.IDSessione=''' + convert(varchar(50),@gIDSessione) +''''
							
			EXEC (@sSQL)	
		END 	
		
						
		IF @bDatiEstesi=1
		BEGIN
			SET @sSQL='SELECT DISTINCT
							M.CodTipoPrescrizione AS Codice,
							T.Descrizione AS Descrizione
						FROM 
							T_TmpFiltriPrescrizioni TMP
								INNER JOIN T_MovPrescrizioni	M	
										ON (TMP.IDPrescrizione=M.ID)																													
								LEFT JOIN T_TipoPrescrizione T
									ON (M.CodTipoPrescrizione=T.Codice)													
						'
									
						SET @sSQL=@sSQL + ' WHERE TMP.IDSessione=''' + convert(varchar(50),@gIDSessione) +''''
		
						
			EXEC (@sSQL)	
		END 	
			
	
					
		IF @bDatiEstesi=1
		BEGIN
		
			SELECT Codice,Descrizione
			FROM T_StatoPrescrizione
			WHERE Codice IN ('AB','DI','IC','ER','DD','AB','SC','DP','AC')
			ORDER BY Descrizione
			
																																					
																	
														
												
																
					END 	
		
				
	EXEC MSP_InsMovTimeStamp @xTimeStamp	

					
		DELETE FROM T_TmpFiltriPrescrizioni 
	WHERE IDSessione=@gIDSessione 
									
				
	RETURN 0
END