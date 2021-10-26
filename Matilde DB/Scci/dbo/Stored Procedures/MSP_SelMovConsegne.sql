CREATE PROCEDURE [dbo].[MSP_SelMovConsegne](@xParametri AS XML)
AS
BEGIN
	   	
	   	
				
		DECLARE @sCodUA AS Varchar(MAX)
	
		DECLARE @uIDConsegna AS UNIQUEIDENTIFIER	
	DECLARE @bDatiEstesi AS Bit		
	DECLARE @sCodStatoConsegna AS VARCHAR(1800)
	DECLARE @sCodTipoConsegna AS VARCHAR(1800)
	DECLARE @sOrdinamento AS VARCHAR(255)
	DECLARE @sCodRuolo AS VARCHAR(20)
	DECLARE @dDataInizio AS DATETIME
	DECLARE @dDataFine AS DATETIME
	DECLARE @nNumRighe AS INTEGER
	
	
	DECLARE @sDataTmp AS VARCHAR(20)		

		DECLARE @sGUID AS VARCHAR(Max)
	DECLARE @nTemp AS INTEGER
	DECLARE @xTmpTS AS XML
	DECLARE @sOrderby AS VARCHAR(MAX)
	
		
	DECLARE @sCodTipoTaskDaPrescrizione AS  VARCHAR(20)
	
	DECLARE @bInserisci AS  BIT
	DECLARE @bModifica AS  BIT
	DECLARE @bCancella AS  BIT
	DECLARE @bAnnulla AS  BIT	
	
		SET @sCodUA=''
	SELECT	@sCodUA =  @sCodUA +
														CASE 
								WHEN @sCodUA='' THEN ''
								ELSE  ','
							END + ''''
						  + ValoreParametro.CodUA.value('.','VARCHAR(1800)')
						  + ''''
						 FROM @xParametri.nodes('/Parametri/CodUA') as ValoreParametro(CodUA)
	SET @sCodUA=LTRIM(RTRIM(@sCodUA))
	IF	@sCodUA='''''' SET @sCodUA=''
			
		SET @sCodRuolo=(SELECT TOP 1 ValoreParametro.CodRuolo.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/TimeStamp/CodRuolo') as ValoreParametro(CodRuolo))
	SET @sCodRuolo=ISNULL(@sCodRuolo,'')


		SET @sOrdinamento=(SELECT TOP 1 ValoreParametro.Ordinamento.value('.','VARCHAR(255)')
					  FROM @xParametri.nodes('/Parametri/Ordinamento') as ValoreParametro(Ordinamento))
	SET @sOrdinamento=ISNULL(@sOrdinamento,'')
									  											  					
		
		SET @nTemp=(SELECT COUNT(*) FROM T_AssRuoliModuli
				WHERE CodModulo='Consegne_Inserisci'	
				AND CodRuolo=@sCodRuolo)
	IF @nTemp>=1 
		SET @bInserisci=1
	ELSE
		SET @bInserisci=0	
		
	
		SET @nTemp=(SELECT COUNT(*) FROM T_AssRuoliModuli
				WHERE CodModulo='Consegne_Inserisci'	
				AND CodRuolo=@sCodRuolo)
	IF @nTemp>=1 
		SET @bModifica=1
	ELSE
		SET @bModifica=0		
				
	
		SET @nTemp=(SELECT COUNT(*) FROM T_AssRuoliModuli
				WHERE CodModulo='Consegne_Annulla'	
				AND CodRuolo=@sCodRuolo)
	IF @nTemp>=1 
		SET @bAnnulla=1
	ELSE
		SET @bAnnulla=0
				
				  				  
		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDConsegna.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDConsegna') as ValoreParametro(IDConsegna))		
	IF 	ISNULL(@sGUID,'') <> '' SET @uIDConsegna=CONVERT(UNIQUEIDENTIFIER,	@sGUID)	
					  			  
		SET @nNumRighe=(SELECT	TOP 1 ValoreParametro.NumRighe.value('.','INTEGER')
								FROM @xParametri.nodes('/Parametri/NumRighe') as ValoreParametro(NumRighe))	
	SET @nNumRighe=ISNULL(@nNumRighe,0)
		
		SET @bDatiEstesi=(SELECT TOP 1 ValoreParametro.DatiEstesi.value('.','bit')
					  FROM @xParametri.nodes('/Parametri/DatiEstesi') as ValoreParametro(DatiEstesi))
	SET @bDatiEstesi=ISNULL(@bDatiEstesi,0)

		SET @sCodStatoConsegna=''
	SELECT	@sCodStatoConsegna =  @sCodStatoConsegna +
														CASE 
								WHEN @sCodStatoConsegna='' THEN ''
								ELSE  ','
							END + ''''
						  + ValoreParametro.CodStatoConsegna.value('.','VARCHAR(1800)')
						  + ''''
						 FROM @xParametri.nodes('/Parametri/CodStatoConsegna') as ValoreParametro(CodStatoConsegna)
						 
	SET @sCodStatoConsegna=LTRIM(RTRIM(@sCodStatoConsegna))
	IF	@sCodStatoConsegna='''''' SET @sCodStatoConsegna=''
	SET @sCodStatoConsegna=UPPER(@sCodStatoConsegna)

		SET @sCodTipoConsegna=''
	SELECT	@sCodTipoConsegna =  @sCodTipoConsegna +
														CASE 
								WHEN @sCodTipoConsegna='' THEN ''
								ELSE  ','
							END + ''''
						  + ValoreParametro.CodTipoConsegna.value('.','VARCHAR(1800)')
						  + ''''
						 FROM @xParametri.nodes('/Parametri/CodTipoConsegna') as ValoreParametro(CodTipoConsegna)
						 
	SET @sCodTipoConsegna=LTRIM(RTRIM(@sCodTipoConsegna))
	IF	@sCodTipoConsegna='''''' SET @sCodTipoConsegna=''
	SET @sCodTipoConsegna=UPPER(@sCodTipoConsegna)
	
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
		
	    DECLARE @xTimeStamp AS XML	

    SET @xTimeStamp=(SELECT TOP 1 ValoreParametro.TS.query('.')
					  FROM @xParametri.nodes('/Parametri/TimeStamp') as ValoreParametro(TS))
	
	SET @xTimeStamp=CONVERT(XML,
						'<Parametri>' + CONVERT(varchar(max),@xTimeStamp) +
						'</Parametri>')		
	
				
					SET @nTemp=(SELECT COUNT(*) FROM T_AssRuoliModuli
				WHERE CodModulo='Consegne_Inserisci'	
				AND CodRuolo=@sCodRuolo)
	IF @nTemp>=1 
		SET @bInserisci=1
	ELSE
		SET @bInserisci=0	
	
		SET @bCancella=1
	
		SET @nTemp=(SELECT COUNT(*) FROM T_AssRuoliModuli
				WHERE CodModulo='Consegne_Annulla'	
				AND CodRuolo=@sCodRuolo)
	IF @nTemp>=1 
		SET @bAnnulla=1
	ELSE
		SET @bAnnulla=0

				
	
	CREATE TABLE #tmpUARuolo
		(
			CodUA VARCHAR(20) COLLATE Latin1_General_CI_AS,
			Descrizione VARCHAR(255) COLLATE Latin1_General_CI_AS
		)
	
	IF @uIDConsegna IS NULL 
	BEGIN		
		DECLARE @xTmp AS XML
		SET  @xTmp=CONVERT(XML,'<Parametri><CodRuolo>'+ @sCodRuolo + '</CodRuolo></Parametri>')
		
		INSERT #tmpUARuolo EXEC MSP_SelUADaRuolo @xTmp
		
		CREATE INDEX IX_CodUA ON #tmpUARuolo (CodUA)

			END
	
				
							   
		DECLARE @sSQL AS VARCHAR(MAX)
	DECLARE @sWhere AS VARCHAR(Max)
	DECLARE @sTmp AS VARCHAR(Max)
	DECLARE @bIgnora AS INTEGER
	
			
	CREATE TABLE #tmpFiltriConsegne
		(
			IDConsegna UNIQUEIDENTIFIER,	
			CodTipoConsegna VARCHAR(20) COLLATE Latin1_General_CI_AS,
			CodStatoConsegna VARCHAR(20) COLLATE Latin1_General_CI_AS,
			CodUA VARCHAR(20) COLLATE Latin1_General_CI_AS
		)
	
	
	SET @sSQL='		INSERT INTO #tmpFiltriConsegne(
						IDConsegna,
						CodTipoConsegna,
						CodStatoConsegna,
						CodUA
						)					
					SELECT '

		IF @nNumRighe<> 0  SET @sSQL= @sSQL + ' TOP ' + convert(varchar(10),@nNumRighe)+  ' ' 					
	
	SET @sSQL=@sSQL +				
					' M.ID AS IDConsegna,	
					  M.CodTipoConsegna,
					  M.CodStatoConsegna,
					  M.CodUA				
				    FROM
						T_MovConsegne M	WITH(NOLOCK) 
				    LEFT JOIN T_UnitaAtomiche A
						ON M.CodUA=A.Codice
					LEFT JOIN T_TipoConsegna TC
						ON M.CodTipoConsegna =TC.Codice
					LEFT JOIN T_StatoConsegna SC
						ON M.CodStatoConsegna =SC.Codice									
					' 							
	IF @uIDConsegna IS NULL
	BEGIN
			SET @sSQL=@sSQL +'
					INNER JOIN #tmpUARuolo UAR
						ON M.CodUA=UAR.CodUA '
	END	
				
	SET @sWhere=''						

		IF @uIDConsegna IS NOT NULL
		BEGIN
			SET @sTmp= ' AND M.ID=''' + convert(varchar(50),@uIDConsegna) +''''
			SET @sWhere= @sWhere + @sTmp								
		END		
	
		IF @sCodTipoConsegna NOT IN ('','''Tutti''')
		BEGIN	
						SET @sTmp=  ' AND 			
							 M.CodTipoConsegna IN ('+ @sCodTipoConsegna + ')
						'  				
			SET @sWhere= @sWhere + @sTmp														
		END   
		
		IF @sCodStatoConsegna NOT IN ('','''Tutti''')
		BEGIN	
						SET @sTmp=  ' AND 			
							 M.CodStatoConsegna IN ('+ @sCodStatoConsegna + ')
						'  				
			SET @sWhere= @sWhere + @sTmp														
		END
		
	IF @sCodUA NOT IN ('','''Tutti''')
		BEGIN	
						SET @sTmp=  ' AND 			
							 M.CodUA IN ('+ @sCodUA + ')
						'  				
			SET @sWhere= @sWhere + @sTmp														
		END   

		IF @uIDConsegna IS NULL
		BEGIN
			SET @sTmp=  ' AND 			
							 M.CodStatoConsegna NOT IN (''CA'')
						'  				
			SET @sWhere= @sWhere + @sTmp	
		END				


				
	 IF @dDataInizio IS NOT NULL OR @dDataFine IS NOT NULL
				SET @sWhere= @sWhere + ' AND ('						
		IF @dDataInizio IS NOT NULL 
		BEGIN						
			SET @sTmp= CASE 
							WHEN @dDataFine IS NULL 
									THEN ' (M.DataEvento = CONVERT(datetime,'''  + convert(varchar(20),@dDataInizio,120) +''',120)
									       )'										ELSE		 ' (M.DataEvento >= CONVERT(datetime,'''  + convert(varchar(20),@dDataInizio,120) +''',120)'	
																				END	
			SET @sWhere= @sWhere + @sTmp								
		END

		IF @dDataFine IS NOT NULL 
		BEGIN
			SET @sWhere= @sWhere + ' AND '												
			IF @dDataFine IS NULL 
				SET @sWhere= @sWhere + '('																														
			SET @sTmp= ' M.DataEvento <= CONVERT(datetime,'''  + convert(varchar(20),@dDataFine,120) +''',120)'
			SET @sWhere= @sWhere + @sTmp																	
			SET @sWhere= @sWhere + ')'															
		END					
	
		IF (@dDataInizio IS NOT NULL OR @dDataFine IS NOT NULL)
			SET @sWhere= @sWhere + ' )'						
		
				
		IF ISNULL(@sWhere,'')<> ''
	BEGIN
	
		SET @sSQL=@sSQL +' WHERE ' + RIGHT(@sWhere,len(@sWhere)-5)
	END	
										 		
 	PRINT @sSQL
	EXEC (@sSQL)			
	
					
	SET @sSQL ='
			SELECT 
				M.ID, 
				M.CodUA,
				A.Descrizione AS DescrUA,
				M.CodTipoConsegna,
				TC.Descrizione AS DescrTipoConsegna,
				M.CodStatoConsegna,
				SC.Descrizione AS DescrStatoConsegna,
				M.CodUtenteRilevazione,
				LC.Descrizione AS DescrUtenteRilevazione,
				M.CodUtenteUltimaModifica,
				LM.Descrizione AS DescrUtenteUltimaModifica,
				ISNULL(M.CodUtenteUltimaModifica,M.CodUtenteRilevazione) AS CodUtenteUltimaModificaCalcolato,
				ISNULL(LM.Descrizione,LC.Descrizione) AS DescrUtenteUltimaModificaCalcolato,
				ISNULL(M.DataUltimaModifica,M.DataInserimento) AS DataUltimaModificaCalcolato,
				M.CodUtenteAnnullamento,
				LM.Descrizione AS DescrUtenteAnnullamento,		
				S.ID AS IDScheda,
				S.CodScheda,
				S.Versione,
				M.DataEvento,
				M.DataEventoUTC,		
				M.DataInserimento,
				M.DataInserimentoUTC,
				M.DataUltimaModifica,
				M.DataUltimaModificaUTC,
				M.DataAnnullamento,
				M.DataAnnullamentoUTC,
				S.AnteprimaRTF,
				CONVERT(varbinary(max),null)  As Icona,
				I.IDIcona,				
				' + CONVERT(VARCHAR(1),@bModifica) + '
							&
									CASE 
										WHEN ISNULL(M.CodStatoConsegna,'''') IN (''IC'') THEN 1 
										ELSE 0				 
									END 
							 AS PermessoModifica,
				' + CONVERT(VARCHAR(1),@bAnnulla) + ' 
							&
									CASE 
										WHEN ISNULL(M.CodStatoConsegna,'''') IN (''IC'') THEN 1 
										ELSE 0				 
									END 
							AS PermessoAnnulla
			FROM 
				T_MovConsegne M			  											
					LEFT JOIN T_UnitaAtomiche A
						ON M.CodUA=A.Codice
					LEFT JOIN T_TipoConsegna TC
						ON M.CodTipoConsegna =TC.Codice
					LEFT JOIN T_StatoConsegna SC
						ON M.CodStatoConsegna =SC.Codice			
					LEFT JOIN T_Login LC
						ON M.CodUtenteRilevazione=LC.Codice
					LEFT JOIN T_Login LM
						ON M.CodUtenteUltimaModifica=LM.Codice
					LEFT JOIN T_Login LA
						ON M.CodUtenteUltimaModifica=LA.Codice
					LEFT JOIN T_MovSchede S
						ON (S.CodEntita=''CSG'' AND
							S.IDEntita=M.ID AND
							S.Storicizzata=0)
					LEFT JOIN 
						(SELECT 
							IDNum AS IDIcona,	
							CodTipo,
							CodStato,
														CONVERT(varbinary(max),null)  As Icona
							FROM T_Icone WITH(NOLOCK)
							WHERE CodEntita=''CSG''
						) AS I
					ON (M.CodTipoConsegna = I.CodTipo AND 	
						M.CodStatoConsegna = I.CodStato
						)		
			WHERE M.ID 
				IN (SELECT IDConsegna FROM #tmpFiltriConsegne)
		'
				
	SET @sOrderBy = ' ORDER BY M.DataEvento DESC, M.CodTipoConsegna DESC '		
	SET @sSQL=@sSQL + @sOrderBy
	
	
		EXEC (@sSQL)										

					IF @bDatiEstesi=1
		BEGIN
			SELECT DISTINCT
				T.Codice AS Codice,
				T.Descrizione AS Descrizione
			FROM #tmpFiltriConsegne M
					INNER JOIN T_TipoConsegna T
						ON M.CodTipoConsegna = T.Codice
			ORDER BY Descrizione
		END 	
	
					IF @bDatiEstesi=1
		BEGIN
			SELECT DISTINCT
				T.Codice AS Codice,
				T.Descrizione AS Descrizione
			FROM #tmpFiltriConsegne M
					INNER JOIN T_StatoConsegna T
						ON M.CodStatoConsegna = T.Codice
			ORDER BY Descrizione
		END 				
	
					IF @bDatiEstesi=1
		BEGIN		
			SELECT DISTINCT
				T.Codice AS Codice,
				T.Descrizione AS Descrizione
			FROM #tmpFiltriConsegne M
					INNER JOIN T_UnitaAtomiche T
						ON M.CodUA = T.Codice
			ORDER BY Descrizione
		END 		

				
	EXEC MSP_InsMovTimeStamp @xTimeStamp	

				
	DROP TABLE #tmpFiltriConsegne 									
	
END











