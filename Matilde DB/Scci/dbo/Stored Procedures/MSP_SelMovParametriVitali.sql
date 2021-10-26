

CREATE PROCEDURE [dbo].[MSP_SelMovParametriVitali](@xParametri AS XML)
AS
BEGIN
	   	
	   	
				
	DECLARE @uIDPaziente AS UNIQUEIDENTIFIER	
	DECLARE @uIDEpisodio AS UNIQUEIDENTIFIER
	DECLARE @uIDTrasferimento AS UNIQUEIDENTIFIER
	DECLARE @uIDParametroVitale AS UNIQUEIDENTIFIER
	DECLARE @nNumRighe AS INTEGER
	DECLARE @bDatiEstesi AS Bit		
	DECLARE @sCodTipoParametroVitale AS VARCHAR(1800)
	DECLARE @sCodStatoParametroVitale AS VARCHAR(1800)
	DECLARE @dDataInizio AS DATETIME
	DECLARE @dDataFine AS DATETIME
	DECLARE @bVisualizzazioneSintetica AS BIT
	DECLARE @bVisualizzazioneGrafici AS BIT
	DECLARE @sCodRuolo AS VARCHAR(20)
	
	DECLARE @sDataTmp AS VARCHAR(20)
	
		DECLARE @sGUID AS VARCHAR(Max)
	DECLARE @nTemp AS INTEGER
	DECLARE @bInserisci AS BIT
	DECLARE @bModifica AS BIT
	DECLARE @bCancella AS BIT
	DECLARE @bAnnulla AS BIT
	DECLARE @xTmpTS AS XML
	DECLARE @sOrderBy AS VARCHAR(MAX)
	
	
		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDPaziente.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDPaziente') as ValoreParametro(IDPaziente))
	IF 	ISNULL(@sGUID,'') <> '' SET @uIDPaziente=CONVERT(UNIQUEIDENTIFIER,	@sGUID)	
	
		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDEpisodio.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDEpisodio') as ValoreParametro(IDEpisodio))
	IF 	ISNULL(@sGUID,'') <> '' SET @uIDEpisodio=CONVERT(UNIQUEIDENTIFIER,	@sGUID)	
	
		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDTrasferimento.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDTrasferimento') as ValoreParametro(IDTrasferimento))
	IF 	ISNULL(@sGUID,'') <> '' SET @uIDTrasferimento=CONVERT(UNIQUEIDENTIFIER,	@sGUID)	
					  				  
		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDParametroVitale.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDParametroVitale') as ValoreParametro(IDParametroVitale))		
	IF 	ISNULL(@sGUID,'') <> '' SET @uIDParametroVitale=CONVERT(UNIQUEIDENTIFIER,	@sGUID)	
					  			  
		SET @nNumRighe=(SELECT	TOP 1 ValoreParametro.NumRighe.value('.','INTEGER')
								FROM @xParametri.nodes('/Parametri/NumRighe') as ValoreParametro(NumRighe))	
	SET @nNumRighe=ISNULL(@nNumRighe,0)
		
		SET @bDatiEstesi=(SELECT TOP 1 ValoreParametro.DatiEstesi.value('.','bit')
					  FROM @xParametri.nodes('/Parametri/DatiEstesi') as ValoreParametro(DatiEstesi))
	SET @bDatiEstesi=ISNULL(@bDatiEstesi,0)

		SET @sCodStatoParametroVitale=''
	SELECT	@sCodStatoParametroVitale =  @sCodStatoParametroVitale +
														CASE 
								WHEN @sCodStatoParametroVitale='' THEN ''
								ELSE  ','
							END + ''''
						  + ValoreParametro.CodStatoParametroVitale.value('.','VARCHAR(1800)')
						  + ''''
						 FROM @xParametri.nodes('/Parametri/CodStatoParametroVitale') as ValoreParametro(CodStatoParametroVitale)
						 
	SET @sCodStatoParametroVitale=LTRIM(RTRIM(@sCodStatoParametroVitale))
	IF	@sCodStatoParametroVitale='''''' SET @sCodStatoParametroVitale=''
	SET @sCodStatoParametroVitale=UPPER(@sCodStatoParametroVitale)

		SET @sCodTipoParametroVitale=''
	SELECT	@sCodTipoParametroVitale =  @sCodTipoParametroVitale +
														CASE 
								WHEN @sCodTipoParametroVitale='' THEN ''
								ELSE  ','
							END + ''''
						  + ValoreParametro.CodTipoParametroVitale.value('.','VARCHAR(1800)')
						  + ''''
						 FROM @xParametri.nodes('/Parametri/CodTipoParametroVitale') as ValoreParametro(CodTipoParametroVitale)
						 
	SET @sCodTipoParametroVitale=LTRIM(RTRIM(@sCodTipoParametroVitale))
	IF	@sCodTipoParametroVitale='''''' SET @sCodTipoParametroVitale=''
	SET @sCodTipoParametroVitale=UPPER(@sCodTipoParametroVitale)

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
				SET	@dDataInizio=CONVERT(DATETIME,@sDataTmp,120)						
			ELSE
				SET	@dDataInizio =NULL			
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
				SET	@dDataFine =NULL		
		END 		
	
	
		SET @bVisualizzazioneSintetica=(SELECT TOP 1 ValoreParametro.VisualizzazioneSintetica.value('.','bit')
					  FROM @xParametri.nodes('/Parametri/VisualizzazioneSintetica') as ValoreParametro(VisualizzazioneSintetica))											
	SET @bVisualizzazioneSintetica=ISNULL(@bVisualizzazioneSintetica,0)
	
		SET @bVisualizzazioneGrafici=(SELECT TOP 1 ValoreParametro.VisualizzazioneGrafici.value('.','bit')
					  FROM @xParametri.nodes('/Parametri/VisualizzazioneGrafici') as ValoreParametro(VisualizzazioneGrafici))											
	SET @bVisualizzazioneGrafici=ISNULL(@bVisualizzazioneGrafici,0)
						  
	    DECLARE @xTimeStamp AS XML	

    SET @xTimeStamp=(SELECT TOP 1 ValoreParametro.TS.query('.')
					  FROM @xParametri.nodes('/Parametri/TimeStamp') as ValoreParametro(TS))
	
	SET @xTimeStamp=CONVERT(XML,
						'<Parametri>' + CONVERT(varchar(max),@xTimeStamp) +
						'</Parametri>')
						
		SET @sCodRuolo=(SELECT TOP 1 ValoreParametro.CodRuolo.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/TimeStamp/CodRuolo') as ValoreParametro(CodRuolo))
	SET @sCodRuolo=ISNULL(@sCodRuolo,'')


						
												
				
				
	
	CREATE TABLE #tmpUARuolo
	(
		CodUA VARCHAR(20) COLLATE Latin1_General_CI_AS,
		Descrizione VARCHAR(255) COLLATE Latin1_General_CI_AS
	)

	DECLARE @xTmp AS XML
	SET  @xTmp=CONVERT(XML,'<Parametri><CodRuolo>'+ @sCodRuolo + '</CodRuolo></Parametri>')
	
	INSERT #tmpUARuolo EXEC MSP_SelUADaRuolo @xTmp
	
	CREATE INDEX IX_CodUA ON #tmpUARuolo (CodUA)    
		
			SET @nTemp=(SELECT COUNT(*) FROM T_AssRuoliModuli
				WHERE CodModulo='ParamV_Inserisci'	
				AND CodRuolo=@sCodRuolo)
	IF @nTemp>=1 
		SET @bInserisci=1
	ELSE
		SET @bInserisci=0	
	
		SET @nTemp=(SELECT COUNT(*) FROM T_AssRuoliModuli
				WHERE CodModulo='ParamV_Modifica'	
				AND CodRuolo=@sCodRuolo)
	IF @nTemp>=1 
		SET @bModifica=1
	ELSE
		SET @bModifica=0	
	
		SET @nTemp=(SELECT COUNT(*) FROM T_AssRuoliModuli
				WHERE CodModulo='ParamV_Cancella'	
				AND CodRuolo=@sCodRuolo)
	IF @nTemp>=1 
		SET @bCancella=1
	ELSE
		SET @bCancella=0	
		
		SET @nTemp=(SELECT COUNT(*) FROM T_AssRuoliModuli
				WHERE CodModulo='ParamV_Annulla'	
				AND CodRuolo=@sCodRuolo)
	IF @nTemp>=1 
		SET @bAnnulla=1
	ELSE
		SET @bAnnulla=0
		
				
							   
		DECLARE @sSQL AS VARCHAR(MAX)
	DECLARE @sSQLCIV AS VARCHAR(MAX)
	DECLARE @sWhere AS VARCHAR(Max)
	DECLARE @sTmp AS VARCHAR(Max)
	DECLARE @gIDSessione AS UNIQUEIDENTIFIER
	
		SET @gIDSessione=NEWID()
	
		SET @sSQLCIV= ' (SELECT SC.IDCartella,SC.IDTrasferimento, TS.CodUA
						 FROM 
								T_MovCartelleInVisione SC
									LEFT JOIN T_MovTrasferimenti TS
										ON SC.IDTrasferimento=TS.ID
						 WHERE CodRuoloInVisione=''' + @sCodRuolo + '''
							   AND DataInizio <=GetDate() AND DataFine>=GetDate() 
							   AND CodStatoCartellaInVisione=''IC''	
						 GROUP BY 
							SC.IDCartella,SC.IDTrasferimento, TS.CodUA		   	   
						 ) '
						 
						
	
	SET @sSQL='		INSERT INTO T_TmpFiltriParametriVitali(IDSessione,IDParametroVitale)					
					SELECT '

		IF @nNumRighe<> 0  SET @sSQL= @sSQL + ' TOP ' + convert(varchar(10),@nNumRighe)+  ' ' 					
	
	SET @sSQL=@sSQL +				
					'	'''  + convert(varchar(50),@gIDSessione) + ''' AS IDSessione,	'
							+ ' M.ID AS IDParametroVitale	
					FROM 
						T_MovParametriVitali	M	
							LEFT JOIN T_MovPazienti P
								ON (M.IDEpisodio=P.IDEpisodio)										  							 
					'
				
				
	SET @sWhere=''				
		
		IF @uIDPaziente IS NOT NULL
		BEGIN
			SET @sTmp= ' AND P.IDPaziente=''' + convert(varchar(50),@uIDPaziente) +''''
			SET @sWhere= @sWhere + @sTmp								
		END			

		IF @uIDEpisodio IS NOT NULL
		BEGIN
			SET @sTmp= ' AND M.IDEpisodio=''' + convert(varchar(50),@uIDEpisodio) +''''
			SET @sWhere= @sWhere + @sTmp								
		END	
	
		IF @uIDTrasferimento IS NOT NULL
		BEGIN
			SET @sTmp= ' AND M.IDTrasferimento=''' + convert(varchar(50), @uIDTrasferimento) +''''
			SET @sWhere= @sWhere + @sTmp								
		END		
		
		IF @uIDParametroVitale IS NOT NULL
		BEGIN
			SET @sTmp= ' AND M.ID=''' + convert(varchar(50),@uIDParametroVitale) +''''
			SET @sWhere= @sWhere + @sTmp								
		END		
	
		IF @sCodStatoParametroVitale NOT IN ('','''Tutti''')
		BEGIN	
						SET @sTmp=  ' AND 			
							 M.CodStatoParametroVitale IN ('+ @sCodStatoParametroVitale + ')
						'  				
			SET @sWhere= @sWhere + @sTmp														
		END
	ELSE
		BEGIN
			IF @sCodStatoParametroVitale NOT IN ('''Tutti''')
				BEGIN
															SET @sTmp=  ' AND 			
									 M.CodStatoParametroVitale NOT IN (''CA'')
								'  				
					SET @sWhere= @sWhere + @sTmp
				END	
			ELSE
				BEGIN
															SET @sTmp=  ' AND 			
									 M.CodStatoParametroVitale <> ''CA''
								'  				
					SET @sWhere= @sWhere + @sTmp
				END	
		END	
		
		IF @sCodTipoParametroVitale NOT IN ('')
		BEGIN	
						SET @sTmp=  ' AND 			
							 M.CodTipoParametroVitale IN ('+ @sCodTipoParametroVitale + ')
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
				
		
		IF ISNULL(@sWhere,'')<> ''
	BEGIN
	
		SET @sSQL=@sSQL +' WHERE ' + RIGHT(@sWhere,len(@sWhere)-5)
	END	
	
				
	IF ISNULL(@bVisualizzazioneGrafici,0)=1		  
 		SET @sOrderBy=' ORDER BY M.DataEvento DESC ' 
 	ELSE	
		SET @sOrderBy=' ORDER BY M.DataEvento DESC, M.IDnum DESC'
	
		
	SET @sSQL=@sSQL + @sOrderBy
		EXEC (@sSQL)
	
					
					
			
	IF ISNULL(@bVisualizzazioneSintetica,0)=0 AND ISNULL(@bVisualizzazioneGrafici,0)=0
				SET @sSQL='	SELECT
						M.ID,
						P.IDPaziente,
						M.IDEpisodio,
						M.IDTrasferimento,
						M.DataEvento,
						M.DataEventoUTC,	
						M.DataInserimento,					
						M.DataInserimentoUTC,
						ISNULL(M.DataUltimaModifica,M.DataInserimento) AS DataUltimaModifica,
						ISNULL(LM.Descrizione,L.Descrizione) AS DescrUtenteUltimaModifica,
						M.CodTipoParametroVitale AS CodTipo,
						T.Descrizione AS DescTipo, 
						M.CodStatoParametroVitale AS CodStato,
						S.Descrizione As DescStato,
						M.CodUtenteRilevazione AS CodUtente,
						L.Descrizione AS DescrUtente,
						MS.IDScheda,
						MS.CodScheda,
						MS.Versione,
						MS.AnteprimaRTF,
						TRASF.CodUA,
						M.ValoriGrafici,
						M.ValoriFUT,
						CASE 
							WHEN (TUA.CodUA IS NULL AND CIV.CodUA IS NULL) THEN 0 
							WHEN 
								ISNULL(CodStatoParametroVitale,'''') IN (''ER'') THEN 1 								 
							ELSE 0							
						END & ' + CONVERT(CHAR(10),@bModifica) + ' AS PermessoModifica,
						CASE 
							WHEN (TUA.CodUA IS NULL AND CIV.CodUA IS NULL) THEN 0 
							WHEN 
								ISNULL(CodStatoParametroVitale,'''') IN (''ER'') THEN 1 									
							ELSE 0							
						END  & ' + CONVERT(CHAR(10),@bCancella) + ' AS PermessoCancella,
						CASE 
							WHEN (TUA.CodUA IS NULL AND CIV.CodUA IS NULL) THEN 0 
							WHEN 
								ISNULL(CodStatoParametroVitale,'''') IN (''ER'') THEN 1 									
							ELSE 0							
						END  & ' + CONVERT(CHAR(10),@bAnnulla) + ' AS PermessoAnnulla,
						
						1 AS PermessoGrafico,
						M.CodSistema,
						M.IDSistema						
						'
	ELSE
		IF ISNULL(@bVisualizzazioneSintetica,0)=1
		BEGIN
							SET @sSQL='	SELECT						
							M.ID,	
							CASE 
								WHEN DataEvento IS NOT NULL THEN	
										Convert(VARCHAR(10),M.DataEvento,105) + '' '' + LEFT(CONVERT(varchar(20),M.DataEvento,14),5) 									
								ELSE NULL 
							END AS DataEvento,												
							MS.AnteprimaRTF 
							'
		END
		ELSE
			BEGIN
								IF ISNULL(@bVisualizzazioneGrafici,0)=1						
				BEGIN					
					SET @sSQL='	
							SELECT						
								M.ID,	
								DataEvento,												
								M.ValoriGrafici
							'
				END
			END	
			
			
	IF ISNULL(@bVisualizzazioneSintetica,0)=0 AND ISNULL(@bVisualizzazioneGrafici,0)=0		  
			 SET @sSQL=@sSQL + ' ,I.Icona, I.IDIcona'
		
	SET @sSQL=@sSQL + '
				FROM 
					T_TmpFiltriParametriVitali TMP
																	
						INNER JOIN T_MovParametriVitali	M	
								ON (TMP.IDParametroVitale=M.ID)	
								
						LEFT JOIN T_MovTrasferimenti TRASF
								ON (M.IDTrasferimento=TRASF.ID)	
								
						LEFT JOIN #tmpUARuolo AS TUA
								ON (TRASF.CodUA=TUA.CodUA)								
						
						LEFT JOIN ' + @sSQLCIV + ' AS CIV							
								ON M.IDTrasferimento=CIV.IDTrasferimento				
						LEFT JOIN 
								(SELECT ID As IDScheda, CodScheda,Versione,IDEntita,AnteprimaRTF
								 FROM
									T_MovSchede 
								 WHERE CodEntita=''PVT'' AND							
									Storicizzata=0  
								) AS MS
							ON MS.IDEntita=M.ID	'
							
	IF ISNULL(@bVisualizzazioneSintetica,0)=0 AND ISNULL(@bVisualizzazioneGrafici,0)=0
				SET @sSQL=@sSQL + '																				
						LEFT JOIN T_MovPazienti P
							ON (M.IDEpisodio=P.IDEpisodio)													
						LEFT JOIN T_TipoParametroVitale T
							ON (M.CodTipoParametroVitale=T.Codice)
						LEFT JOIN T_StatoParametroVitale S
							ON (M.CodStatoParametroVitale=S.Codice)
						LEFT JOIN T_Login L
							ON (M.CodUtenteRilevazione=L.Codice)	
						LEFT JOIN T_Login LM
							ON (M.CodUtenteUltimaModifica=LM.Codice)		
								
					  '  

			IF ISNULL(@bVisualizzazioneGrafici,0)=0		  
		SET @sSQL=@sSQL + ' 							
						LEFT JOIN 
							(SELECT 
							  IDNum AS IDIcona,
							  CodTipo,
							  CodStato,		
							  							   CONVERT(varbinary(max),null)  As Icona
							  
							 FROM T_Icone
							 WHERE CodEntita=''PVT''
							) AS I
								ON (M.CodTipoParametroVitale=I.CodTipo AND 	
									M.CodStatoParametroVitale=I.CodStato
									)								
					'		
		SET @sSQL=@sSQL + ' WHERE TMP.IDSessione=''' + convert(varchar(50),@gIDSessione) +''''	
	
		SET @sSQL=@sSQL + @sOrderBy
 
	PRINT @sSQL			
	EXEC (@sSQL)
	
					IF @bDatiEstesi=1
		BEGIN
			SET @sSQL='SELECT DISTINCT
							M.CodTipoParametroVitale AS CodTipo,
							T.Descrizione AS DescTipo
							,T.Icona
													FROM 
							T_TmpFiltriParametriVitali TMP
								INNER JOIN T_MovParametriVitali	M	
										ON (TMP.IDParametroVitale=M.ID)																													
								LEFT JOIN T_TipoParametroVitale T
									ON (M.CodTipoParametroVitale=T.Codice)													
						'
						SET @sSQL=@sSQL + ' WHERE TMP.IDSessione=''' + convert(varchar(50),@gIDSessione) +''''						
			
						EXEC (@sSQL)
		END 						
		
				
	EXEC MSP_InsMovTimeStamp @xTimeStamp	
	
					
		DELETE FROM T_TmpFiltriParametriVitali 
	WHERE IDSessione=@gIDSessione 
									
				
END