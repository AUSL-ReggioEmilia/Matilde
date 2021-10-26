CREATE PROCEDURE [dbo].[MSP_SelMovProtocolliPrescrizioni](@xParametri XML)
AS
BEGIN
		
	
									
			DECLARE @sIDPrescrizione AS VARCHAR(1800)
	DECLARE @sCodRuolo AS VARCHAR(20)
		
								
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

		SET @sIDPrescrizione=''
	SELECT	@sIDPrescrizione =  @sIDPrescrizione +
														CASE 
								WHEN @sIDPrescrizione='' THEN ''
								ELSE  ','
							END + ''''
						  + ValoreParametro.IDPrescrizione.value('.','VARCHAR(50)')
						  + ''''
						 FROM @xParametri.nodes('/Parametri/IDPrescrizione') as ValoreParametro(IDPrescrizione)

	SET @sIDPrescrizione=LTRIM(RTRIM(@sIDPrescrizione))
	IF	@sIDPrescrizione='''''' SET @sIDPrescrizione=''
	SET @sIDPrescrizione=UPPER(@sIDPrescrizione)
	
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
		
			
	    SET @xTimeStamp=(SELECT TOP 1 ValoreParametro.TS.query('.')
					  FROM @xParametri.nodes('/Parametri/TimeStamp') as ValoreParametro(TS))
	
				
				
							   
		DECLARE @sSQL AS VARCHAR(MAX)
	DECLARE @sWhere AS VARCHAR(Max)
	DECLARE @sTmp AS VARCHAR(Max)	
	
	
							
	SET @sSQL='	SELECT							
					M.ID,
					M.IDEpisodio, 
					M.IDTrasferimento, 
					PZ.IDPaziente, 					
					M.CodViaSomministrazione,
					VS.Descrizione AS DescrViaSomministrazione,
					CONVERT(VARBINARY(MAX),NULL) AS Icona,
					CONVERT(VARBINARY(MAX),NULL) AS Icona2,
					M.CodTipoPrescrizione,
					TP.Descrizione AS DescrTipoPrescrizione,
					PRETASK.CodStatoPrescrizioneCalc AS CodStatoPrescrizione ,
					SP.Descrizione AS DescrStatoPrescrizione,
					''Color [A=255, R=255, G=255, B=168]'' AS ColoreStatoPrescrizione,										
					M.DataEvento,
					TEMPI.MinDataOraInizio AS DataInizio,
					TEMPI.MaxDataOraFine AS DataFine,
					CASE 
						WHEN CodStatoPrescrizione=''VA'' THEN M.DataUltimaModifica
						ELSE NULL
					END AS  DataValidazione,				
					M.CodUtenteRilevazione AS CodUtente,
					L.Descrizione AS DescrUtente,
					MS.IDScheda,
					MS.CodScheda,
					MS.Versione,
				    MS.AnteprimaRTF, 
					MS.AnteprimaTXT,													
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
					ISNULL(TP.NonProseguibile,0) AS NonPreseguibile,
					QtaAlBisogno,
					MinIDNum
					'

	SET @sSQL = @sSQL +					
					' FROM 												
						 T_MovPrescrizioni AS M										
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
											(SELECT 
												IDPrescrizione,COUNT(*) AS QtaDaValidare,
												MIN(DataOraInizio) AS MinDataOraInizio,
												MAX(DataOraFine) AS MaxDataOraFine,
												SUM(CONVERT(INTEGER,ISNULL(AlBisogno,0))) As QtaAlBisogno,
												MIN(IDNum) AS MinIDNum
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

		SET @sWhere=''									

		IF @sIDPrescrizione IS NOT NULL
		BEGIN
			SET @sTmp= ' AND M.ID IN (' + @sIDPrescrizione +')'
			SET @sWhere= @sWhere + @sTmp								
		END				
	
	SET @sTmp=  ' AND 			
					 M.CodStatoPrescrizione <> ''CA'' 				  
				'  	
					
	IF ISNULL(@sTmp,'') <> '' 		
		SET @sWhere= @sWhere + @sTmp	
				
		IF ISNULL(@sWhere,'')<> ''
	BEGIN
	
		SET @sWhere=@sWhere +' WHERE ' + RIGHT(@sWhere,len(@sWhere)-5)
	END	
		 		
		SET @sSQL=@sSQL + @sWhere

		SET @sSQL=@sSQL + ' ORDER BY QtaAlBisogno ASC,MinIDNum ASC, TEMPI.MinDataOraInizio, DataEvento DESC '
		
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
					M.Posologia AS Posologia,
					AnteprimaRTF AS PosologiaRTF,					
					ISNULL(AnteprimaTXT,M.Posologia) AS PosologiaTXT,
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
					
										 CASE 
							WHEN ISNULL(Manuale,0) =1 THEN ''''
							ELSE PR.Descrizione 
					 END As DescrProtocollo,					
					CodProtocollo,									
					ISNULL(Manuale,0) As  Manuale,
					TempiManuali, 
					MS.AnteprimaTXT,
					MS.AnteprimaRTF,

										
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
						T_MovPrescrizioniTempi AS M WITH (NOLOCK)									
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
							LEFT JOIN 
											(SELECT ID As IDScheda, CodScheda,Versione,IDEntita,AnteprimaRTF,AnteprimaTXT
											 FROM
												T_MovSchede 
											 WHERE CodEntita=''PRT'' AND							
												Storicizzata=0  AND
												CodStatoScheda <> ''CA''
											) AS MS
										ON MS.IDEntita=M.ID	
							'									
	
		SET @sWhere=''									

		IF @sIDPrescrizione IS NOT NULL
		BEGIN
			SET @sTmp= ' AND M.IDPrescrizione IN (' + @sIDPrescrizione +')'
			SET @sWhere= @sWhere + @sTmp								
		END	
		
					
	SET @sTmp=  ' AND 			
					 M.CodStatoPrescrizioneTempi <> ''CA'' 				  
				'  	
					
	IF ISNULL(@sTmp,'') <> '' 		
		SET @sWhere= @sWhere + @sTmp	
				
		IF ISNULL(@sWhere,'')<> ''
	BEGIN
	
		SET @sWhere=@sWhere +' WHERE ' + RIGHT(@sWhere,len(@sWhere)-5)
	END	
		 	
	PRINT @sSQL
	
	SET @sSQL=@sSQL + @sWhere
	SET @sSQL=@sSQL + ' ORDER BY M.DataOraInizio ASC '
		
		EXEC (@sSQL)					
								    	
				
	
					
										
				
	RETURN 0
END