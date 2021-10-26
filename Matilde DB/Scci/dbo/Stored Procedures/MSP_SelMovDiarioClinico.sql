CREATE PROCEDURE [dbo].[MSP_SelMovDiarioClinico](@xParametri AS XML)
AS
BEGIN
	   	
	   	
				
	DECLARE @uIDPaziente AS UNIQUEIDENTIFIER	
	DECLARE @uIDEpisodio AS UNIQUEIDENTIFIER
	DECLARE @uIDTrasferimento AS UNIQUEIDENTIFIER
	DECLARE @uIDDiarioClinico AS UNIQUEIDENTIFIER
	DECLARE @nNumRighe AS INTEGER
	DECLARE @bDatiEstesi AS Bit		
	DECLARE @sCodStatoDiario AS VARCHAR(1800)
	DECLARE @sCodTipoDiario AS VARCHAR(1800)
	DECLARE @sCodTipoVoceDiario AS VARCHAR(1800)
	DECLARE @sCodUtente AS VARCHAR(1800)
	DECLARE @sCodLogin AS VARCHAR(1800)
	DECLARE @sCodRuolo AS VARCHAR(20)
	DECLARE @dDataInizio AS DATETIME
	DECLARE @dDataFine AS DATETIME
	DECLARE @sDescrizione AS VARCHAR(500)
	
	DECLARE @sDataTmp AS VARCHAR(20)
	
		DECLARE @sCodRuoloSuperUser AS VARCHAR(20)
	DECLARE @bSuperUser AS BIT
	
	DECLARE @sGUID AS VARCHAR(Max)
	
		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDPaziente.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDPaziente') as ValoreParametro(IDPaziente))
	IF 	ISNULL(@sGUID,'') <> '' SET @uIDPaziente=CONVERT(UNIQUEIDENTIFIER,	@sGUID)	
	
	
	
		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDEpisodio.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDEpisodio') as ValoreParametro(IDEpisodio))
	IF 	ISNULL(@sGUID,'') <> '' SET @uIDEpisodio=CONVERT(UNIQUEIDENTIFIER,	@sGUID)
	
		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDTrasferimento.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDTrasferimento') as ValoreParametro(IDTrasferimento))
	IF 	ISNULL(@sGUID,'') <> '' SET @uIDTrasferimento=CONVERT(UNIQUEIDENTIFIER,	@sGUID)
					  				  
		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDDiarioClinico.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDDiarioClinico') as ValoreParametro(IDDiarioClinico))		
	IF 	ISNULL(@sGUID,'') <> '' SET @uIDDiarioClinico=CONVERT(UNIQUEIDENTIFIER,	@sGUID)					  			  
	
		SET @nNumRighe=(SELECT	TOP 1 ValoreParametro.NumRighe.value('.','INTEGER')
								FROM @xParametri.nodes('/Parametri/NumRighe') as ValoreParametro(NumRighe))	
	SET @nNumRighe=ISNULL(@nNumRighe,0)
		
		SET @bDatiEstesi=(SELECT TOP 1 ValoreParametro.DatiEstesi.value('.','bit')
					  FROM @xParametri.nodes('/Parametri/DatiEstesi') as ValoreParametro(DatiEstesi))
	SET @bDatiEstesi=ISNULL(@bDatiEstesi,0)

		SET @sCodStatoDiario=''
	SELECT	@sCodStatoDiario =  @sCodStatoDiario +
														CASE 
								WHEN @sCodStatoDiario='' THEN ''
								ELSE  ','
							END + ''''
						  + ValoreParametro.CodStatoDiario.value('.','VARCHAR(1800)')
						  + ''''
						 FROM @xParametri.nodes('/Parametri/CodStatoDiario') as ValoreParametro(CodStatoDiario)
						 
	SET @sCodStatoDiario=LTRIM(RTRIM(@sCodStatoDiario))
	IF	@sCodStatoDiario='''''' SET @sCodStatoDiario=''
	SET @sCodStatoDiario=UPPER(@sCodStatoDiario)

		SET @sCodTipoDiario=''
	SELECT	@sCodTipoDiario =  @sCodTipoDiario +
														CASE 
								WHEN @sCodTipoDiario='' THEN ''
								ELSE  ','
							END + ''''
						  + ValoreParametro.CodTipoDiario.value('.','VARCHAR(1800)')
						  + ''''
						 FROM @xParametri.nodes('/Parametri/CodTipoDiario') as ValoreParametro(CodTipoDiario)
						 
	SET @sCodTipoDiario=LTRIM(RTRIM(@sCodTipoDiario))
	IF	@sCodTipoDiario='''''' SET @sCodTipoDiario=''
	SET @sCodTipoDiario=UPPER(@sCodTipoDiario)

		SET @sCodTipoVoceDiario=''
	SELECT	@sCodTipoVoceDiario =  @sCodTipoVoceDiario +
														CASE 
								WHEN @sCodTipoVoceDiario='' THEN ''
								ELSE  ','
							END + ''''
						  + ValoreParametro.CodTipoVoceDiario.value('.','VARCHAR(1800)')
						  + ''''
						 FROM @xParametri.nodes('/Parametri/CodTipoVoceDiario') as ValoreParametro(CodTipoVoceDiario)
						 
	SET @sCodTipoVoceDiario=LTRIM(RTRIM(@sCodTipoVoceDiario))
	IF	@sCodTipoVoceDiario='''''' SET @sCodTipoVoceDiario=''
	SET @sCodTipoVoceDiario=UPPER(@sCodTipoVoceDiario)
	
	
		SET @sCodUtente=''
	SELECT	@sCodUtente =  @sCodUtente +
														CASE 
								WHEN @sCodUtente='' THEN ''
								ELSE  ','
							END + ''''
						  + ValoreParametro.CodUtente.value('.','VARCHAR(1800)')
						  + ''''
						 FROM @xParametri.nodes('/Parametri/CodUtente') as ValoreParametro(CodUtente)
						 
	SET @sCodUtente=LTRIM(RTRIM(@sCodUtente))
	IF	@sCodUtente='''''' SET @sCodUtente=''
	SET @sCodUtente=UPPER(@sCodUtente)
	
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
							
		SET @sCodRuolo=(SELECT TOP 1 ValoreParametro.CodRuolo.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/TimeStamp/CodRuolo') as ValoreParametro(CodRuolo))	
	SET @sCodRuolo=ISNULL(@sCodRuolo,'')
	
		SET @sDescrizione=(SELECT	TOP 1 ValoreParametro.Descrizione.value('.','VARCHAR(500)')
						 FROM @xParametri.nodes('/Parametri/Descrizione') as ValoreParametro(Descrizione))						 
	
	IF ISNULL(@sDescrizione,'') <>''
		SET @sDescrizione=REPLACE(@sDescrizione,'''','')
				
				
							   
		DECLARE @sSQL AS VARCHAR(MAX)
	DECLARE @sWhere AS VARCHAR(Max)
	DECLARE @sTmp AS VARCHAR(Max)
	DECLARE @gIDSessione AS UNIQUEIDENTIFIER
	
		SET @sCodRuoloSuperUser=(SELECT TOP 1 Valore FROM T_Config WHERE ID=681)
	SET @sCodRuoloSuperUser=ISNULL(@sCodRuoloSuperUser,'')
	
	IF @sCodRuoloSuperUser=@sCodRuolo 
		SET @bSuperUser=1
	ELSE
		SET @bSuperUser=0
				
		SET @gIDSessione=NEWID()
	
	SET @sSQL='		INSERT INTO T_TmpFiltriDiario(IDSessione,IDDiarioClinico)					
					SELECT '

		IF @nNumRighe<> 0  SET @sSQL= @sSQL + ' TOP ' + convert(varchar(10),@nNumRighe)+  ' ' 					
	
	SET @sSQL=@sSQL +				
					'	'''  + convert(varchar(50),@gIDSessione) + ''' AS IDSessione,	'
							+ ' M.ID AS IDDiarioClinico	
					FROM 
						T_MovDiarioClinico	M	
							LEFT JOIN T_MovPazienti P
								ON (M.IDEpisodio=P.IDEpisodio)				  							 
					'
	
		IF @sDescrizione IS NOT NULL
		SET @sSQL=@sSQL +' LEFT JOIN T_MovSchede MS
								ON (MS.CodEntita=''DCL'' AND
									MS.IDEntita=M.ID AND
									MS.Storicizzata=0
									)
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
		
		IF @uIDDiarioClinico IS NOT NULL
		BEGIN
			SET @sTmp= ' AND M.ID=''' + convert(varchar(50),@uIDDiarioClinico) +''''
			SET @sWhere= @sWhere + @sTmp								
		END		
					
		IF @sCodTipoDiario NOT IN ('')
		BEGIN				
			SET @sTmp=  ' AND 			
							 M.CodTipoDiario IN ('+ @sCodTipoDiario + ')
						'  				
			SET @sWhere= @sWhere + @sTmp														
		END
					
		IF @sCodTipoVoceDiario NOT IN ('')
		BEGIN				
			SET @sTmp=  ' AND 			
							 M.CodTipoVoceDiario IN ('+ @sCodTipoVoceDiario + ')
						'  				
			SET @sWhere= @sWhere + @sTmp														
		END
		
		IF @sCodStatoDiario NOT IN ('')
		BEGIN	
						SET @sTmp=  ' AND 			
							 M.CodStatoDiario IN ('+ @sCodStatoDiario + ')
						'  				
			SET @sWhere= @sWhere + @sTmp														
		END																			
	
		IF @sCodUtente NOT IN ('')
		BEGIN	
						SET @sTmp=  ' AND 			
							 M.CodUtenteRilevazione IN ('+ @sCodUtente + ')
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
		
	
	IF @sDescrizione IS NOT NULL
		BEGIN
			SET @sTmp= ' AND MS.AnteprimaTXT LIKE ''%' + @sDescrizione + '%'''
			SET @sWhere= @sWhere + @sTmp
		END
				
		SET @sCodLogin=(SELECT TOP 1 ValoreParametro.CodLogin.value('.','VARCHAR(100)')
					  FROM @xParametri.nodes('/Parametri/TimeStamp/CodLogin') as ValoreParametro(CodLogin))	
	SET @sCodLogin=ISNULL(@sCodLogin,'')



	
	
				
			IF @bSuperUser=0
		SET @sTmp=  ' AND 			
					 M.CodStatoDiario <> ''CA''
				  AND 
					 1=(CASE 
						WHEN M.CodUtenteRilevazione <>''' + @sCodLogin + ''' AND M.CodStatoDiario IN (''VA'',''AN'') THEN 1
						WHEN M.CodUtenteRilevazione=''' + @sCodLogin + ''' THEN 1
						ELSE 0
					   END)	
				'  	
	ELSE
				SET @sTmp=  ' AND 			
			 M.CodStatoDiario <> ''CA''		
		' 			
		IF ISNULL(@sTmp,'') <> '' 		
		SET @sWhere= @sWhere + @sTmp	
				
		IF ISNULL(@sWhere,'')<> ''
	BEGIN
	
		SET @sSQL=@sSQL +' WHERE ' + RIGHT(@sWhere,len(@sWhere)-5)
	END			
 	
	EXEC (@sSQL)
					
		
	SET @sSQL='	SELECT
					M.ID,
					P.IDPaziente,
					M.IDEpisodio,
					M.IDTrasferimento,
					M.DataEvento,
					M.DataEventoUTC,
					M.CodTipoDiario AS CodTipoDiario,
					T.Descrizione AS DescrTipoDiario,
					M.CodTipoVoceDiario AS CodTipo,
					TV.Descrizione AS DescrTipo, 					 					
					M.CodTipoRegistrazione,
					TR.Descrizione As DescrTipoRegistrazione,
					M.CodStatoDiario AS CodStato,
					S.Descrizione As DescrStato,					
					M.CodUtenteRilevazione AS CodUtente,
					L.Descrizione AS DescrUtente,
					M.DataInserimento,
					M.DataInserimentoUTC, 	
					M.DataValidazione,
					M.DataValidazioneUTC,
					M.DataAnnullamento,
					M.DataAnnullamentoUTC,
					M.CodUA,
					MS.IDScheda,
					MS.CodScheda,
					MS.Versione,
					MS.AnteprimaRTF, 
					MS.AnteprimaTXT, 
					M.CodEntitaRegistrazione,	
					M.IDEntitaRegistrazione,
									
					CASE 
						WHEN CodUtenteRilevazione=''' + @sCodLogin + ''' AND  ISNULL(CodStatoDiario,'''') =''IC'' THEN 1 
						ELSE 0
					END	AS PermessoValida,
					CASE 
						WHEN CodUtenteRilevazione=''' + @sCodLogin + ''' AND  ISNULL(CodStatoDiario,'''') =''IC'' THEN 1 
						ELSE 0
					END AS PermessoModifica,
					CASE 
						WHEN CodUtenteRilevazione=''' + @sCodLogin + ''' AND  ISNULL(CodStatoDiario,'''') =''VA'' THEN 1 
						ELSE 0
					END AS PermessoCopia,
					CASE 
						WHEN CodUtenteRilevazione=''' + @sCodLogin + ''' AND  ISNULL(CodStatoDiario,'''') =''VA''							
						 THEN 1 
						ELSE 0
					END AS  PermessoAnnulla,
					CASE 
						WHEN (CodUtenteRilevazione=''' + @sCodLogin + ''' AND  ISNULL(CodStatoDiario,'''') NOT IN(''VA'',''AN''))
							 OR (' + CONVERT(VARCHAR(1),@bSuperUser) + '=1)
						 THEN 1 
						ELSE 0
					END	AS PermessoCancella,
					M.CodSistema,
					M.IDSistema,
					CASE
						WHEN ISNULL(FUA.CodUA,'''')='''' THEN 0
						ELSE 1
					END AS PermessoUAFirma,	
					E.NumeroNosologico,
					E.NumeroListaAttesa,
					'
	
		
	SET @sSQL=@sSQL + ' I.Icona, I.IDIcona'
	
	SET @sSQL=@sSQL + '
				FROM 
					T_TmpFiltriDiario TMP
						INNER JOIN T_MovDiarioClinico	M	
								ON (TMP.IDDiarioClinico=M.ID)
						LEFT JOIN T_MovEpisodi E
							ON (M.IDEpisodio=E.ID)														
						LEFT JOIN T_MovPazienti P
							ON (M.IDEpisodio=P.IDEpisodio)													
						LEFT JOIN T_TipoDiario T
							ON (M.CodTipoDiario=T.Codice)
						LEFT JOIN T_TipoVoceDiario TV
							ON (M.CodTipoVoceDiario=TV.Codice)								
						LEFT JOIN T_StatoDiario S
							ON (M.CodStatoDiario=S.Codice)	
						LEFT JOIN T_TipoRegistrazione TR
							ON (M.CodTipoRegistrazione=TR.Codice)																	
						LEFT JOIN T_Login L
							ON (M.CodUtenteRilevazione=L.Codice)	
						LEFT JOIN 
								(SELECT ID As IDScheda, CodScheda,Versione,IDEntita,AnteprimaRTF,AnteprimaTXT
								 FROM
									T_MovSchede 
								 WHERE CodEntita=''DCL'' AND							
									Storicizzata=0
								) AS MS
							ON MS.IDEntita=M.ID	
						LEFT JOIN T_MovTrasferimenti TRA
							ON M.IDTrasferimento=TRA.ID			
						LEFT JOIN 
								(SELECT CodUA
								 FROM T_AssUAModuli
								 WHERE CodModulo=''FirmaD_Diario''
								 ) AS FUA
							ON TRA.CodUA=FUA.CodUA
					  '

				SET @sSQL=@sSQL + ' 							
						LEFT JOIN 
							(SELECT
							  IDNum AS IDIcona,	 
							  CodTipo,
							  CodStato,							 							  
							  CONVERT(varbinary(max),null)  As Icona
							 FROM T_Icone
							 WHERE CodEntita=''DCL''
							) AS I
								ON (M.CodTipoDiario=I.CodTipo AND 	
									M.CodStatoDiario=I.CodStato
									)								
					'		
	
		SET @sSQL=@sSQL + ' WHERE TMP.IDSessione=''' + convert(varchar(50),@gIDSessione) +''''
	
	SET @sSQL=@sSQL + ' ORDER BY DataEvento DESC, M.IDNum DESC '
		
	PRINT @sSQL					
	EXEC (@sSQL)
	
					
	IF @bDatiEstesi=1
		BEGIN
			SET @sSQL='SELECT DISTINCT
							M.CodTipoVoceDiario AS CodTipo,
							T.Descrizione AS DescTipo
						FROM 
							T_TmpFiltriDiario TMP
								INNER JOIN T_MovDiarioClinico	M	
										ON (TMP.IDDiarioClinico=M.ID)																													
								LEFT JOIN T_TipoVoceDiario T
									ON (M.CodTipoVoceDiario=T.Codice)													
						'
			
						SET @sSQL=@sSQL + ' WHERE TMP.IDSessione=''' + convert(varchar(50),@gIDSessione) +''''
							
			EXEC (@sSQL)	
		END 			

					
	IF @bDatiEstesi=1
		BEGIN
			SET @sSQL='SELECT DISTINCT
							M.CodUtenteRilevazione AS CodUtente,
							L.Descrizione AS DescrUtente
						FROM 
							T_TmpFiltriDiario TMP
								INNER JOIN T_MovDiarioClinico	M	
										ON (TMP.IDDiarioClinico=M.ID)																													
								LEFT JOIN T_Login L
										ON (M.CodUtenteRilevazione=L.Codice)													
						'
			
						SET @sSQL=@sSQL + ' WHERE TMP.IDSessione=''' + convert(varchar(50),@gIDSessione) +''''
							
			EXEC (@sSQL)	
		END 	
			
			
				
	EXEC MSP_InsMovTimeStamp @xTimeStamp	
			
					
		DELETE FROM T_TmpFiltriDiario 
	WHERE IDSessione=@gIDSessione 
								
				
END