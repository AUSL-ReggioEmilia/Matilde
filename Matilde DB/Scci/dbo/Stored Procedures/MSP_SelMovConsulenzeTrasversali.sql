CREATE PROCEDURE [dbo].[MSP_SelMovConsulenzeTrasversali](@xParametri AS XML)
AS
BEGIN
	
	DECLARE @sFiltroGenerico AS Varchar(500) 
	DECLARE @sDataNascita AS VARCHAR(10)
	DECLARE @sCodLogin AS VARCHAR(255)
	DECLARE @sCodStatoDiario AS VARCHAR(1800)	
	DECLARE @sCodUA VARCHAR(1800)	
	DECLARE @sCodRuolo AS VARCHAR(20)
	DECLARE @sCodTipoVoceDiario AS VARCHAR(1800)
	DECLARE @dDataInizio AS DATETIME
	DECLARE @dDataFine AS DATETIME
	DECLARE @sDescrizione AS VARCHAR(500)		
	DECLARE @bDatiEstesi AS Bit		
	DECLARE @sCodRuoloConsulente AS VARCHAR(20)
	DECLARE @sDataTmp AS VARCHAR(20)	
				
		DECLARE @sGUID AS VARCHAR(Max)	
	DECLARE @sSQL AS VARCHAR(MAX)
	DECLARE @sWhere AS VARCHAR(Max)
	DECLARE @sTmp AS VARCHAR(Max)
	DECLARE @gIDSessione AS UNIQUEIDENTIFIER
	
					SET @sFiltroGenerico=(SELECT	TOP 1 ValoreParametro.FiltroGenerico.value('.','VARCHAR(500)')
						 FROM @xParametri.nodes('/Parametri/FiltroGenerico') as ValoreParametro(FiltroGenerico))						 
	SET @sFiltroGenerico= ISNULL(@sFiltroGenerico,'')
	SET @sFiltroGenerico=LTRIM(RTRIM(@sFiltroGenerico))
	
		IF ISNULL(@sFiltroGenerico,'') <> ''
		SET @sFiltroGenerico=REPLACE(@sFiltroGenerico,'''','')

		SET @sDataNascita=(SELECT	TOP 1 ValoreParametro.DataNascita.value('.','VARCHAR(10)')
						 FROM @xParametri.nodes('/Parametri/DataNascita') as ValoreParametro(DataNascita))						 
	SET @sDataNascita= ISNULL(@sDataNascita,'')
	SET @sDataNascita=LTRIM(RTRIM(@sDataNascita))

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
	SET @sCodUA=UPPER(@sCodUA)

		SET @sCodRuolo=(SELECT TOP 1 ValoreParametro.CodRuolo.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/TimeStamp/CodRuolo') as ValoreParametro(CodRuolo))	
	SET @sCodRuolo=ISNULL(@sCodRuolo,'')
	
		SET @sCodLogin=(SELECT TOP 1 ValoreParametro.CodLogin.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/TimeStamp/CodLogin') as ValoreParametro(CodLogin))	
	SET @sCodLogin=ISNULL(@sCodLogin,'')

		SET @bDatiEstesi=(SELECT TOP 1 ValoreParametro.DatiEstesi.value('.','bit')
					  FROM @xParametri.nodes('/Parametri/DatiEstesi') as ValoreParametro(DatiEstesi))
	SET @bDatiEstesi=ISNULL(@bDatiEstesi,0)

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

		SET @sDescrizione=(SELECT	TOP 1 ValoreParametro.Descrizione.value('.','VARCHAR(500)')
						 FROM @xParametri.nodes('/Parametri/Descrizione') as ValoreParametro(Descrizione))						 
	
	IF ISNULL(@sDescrizione,'') <>''
		SET @sDescrizione=REPLACE(@sDescrizione,'''','')

	    DECLARE @xTimeStamp AS XML	

    SET @xTimeStamp=(SELECT TOP 1 ValoreParametro.TS.query('.')
					  FROM @xParametri.nodes('/Parametri/TimeStamp') as ValoreParametro(TS))
	
	SET @xTimeStamp=CONVERT(XML,
						'<Parametri>' + CONVERT(varchar(max),@xTimeStamp) +
						'</Parametri>')
						
												
				
				
							   
	SET @sCodRuoloConsulente=(SELECT TOP 1 Valore FROM T_Config WHERE ID=40)
						
					
	CREATE TABLE #tmpMovDiarioClinico
		(
			IDMovDiarioClinico UNIQUEIDENTIFIER,			
			CodTipoVoceDiario VARCHAR(20) COLLATE Latin1_General_CI_AS,			
			CodUA VARCHAR(20) COLLATE Latin1_General_CI_AS,
		)
	
	SET @sSQL = 'INSERT INTO #tmpMovDiarioClinico(IDMovDiarioClinico,CodTipoVoceDiario,CodUA)
				 SELECT 
					M.ID,
					M.CodTipoVoceDiario,
					TRA.CodUA
				 FROM T_MovDiarioClinico M WITH (NOLOCK)
				 '
		
		SET @sSQL=@sSQL +' INNER JOIN T_MovSchede MS WITH (NOLOCK)
								ON (MS.CodEntita=''DCL'' AND
									MS.IDEntita=M.ID AND
									MS.Storicizzata=0
									)						   
							INNER JOIN T_MovTrasferimenti TRA WITH (NOLOCK)
								ON TRA.ID = M.IDTrasferimento
						'	
				IF (@sFiltroGenerico IS NOT NULL OR @sDataNascita IS NOT NULL)
		SET @sSQL=@sSQL +' INNER JOIN T_MovPazienti P WITH (NOLOCK)
								ON (M.IDEpisodio=P.IDEpisodio)													
						   INNER JOIN T_MovEpisodi EPI WITH (NOLOCK)
								ON (M.IDEpisodio=EPI.ID)	
						   INNER JOIN T_MovCartelle C WITH (NOLOCK)
								ON (TRA.IDCartella=C.ID)		
						 '
				
	SET @sWhere=''

		IF @sCodLogin IS NOT NULL
		BEGIN
			SET @sTmp= ' AND M.CodUtenteRilevazione=''' + convert(varchar(100),@sCodLogin ) +''''
			SET @sWhere= @sWhere + @sTmp								
		END
		
		IF @sCodStatoDiario NOT IN ('')
		BEGIN
			SET @sTmp= ' AND 
								M.CodStatoDiario IN ('+ @sCodStatoDiario + ')
						'
			SET @sWhere= @sWhere + @sTmp								
		END
	ELSE			
		BEGIN
				SET @sTmp=  ' AND 			
								M.CodStatoDiario <> ''CA''
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

		IF @sCodUA NOT IN ('')
		BEGIN				
			SET @sTmp=  ' AND 			
							 TRA.CodUA IN ('+ @sCodUA + ')
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
				
		SET @sWhere= @sWhere + ' AND MS.CodRuoloRilevazione =''' + @sCodRuoloConsulente + ''''

					IF ISNULL(@sDataNascita,'') <> ''
				BEGIN
					SET @sWhere= @sWhere + ' AND P.DataNascita = convert(datetime,''' + @sDataNascita + ''',105)'
				END
				
								IF ISNULL(@sFiltroGenerico,'') <> ''
				BEGIN
										SET @sWhere= @sWhere + ' AND ('
					SET @sWhere= @sWhere + '  P.Cognome like ''%' + @sFiltroGenerico + '%'''
					SET @sWhere= @sWhere + '  OR P.Nome like ''%' + @sFiltroGenerico + '%'''	
					SET @sWhere= @sWhere + '  OR P.Cognome + '' '' + P.Nome like ''%' + @sFiltroGenerico + '%'''	
					SET @sWhere= @sWhere + '  OR P.Nome + '' '' + P.Cognome like ''%' + @sFiltroGenerico + '%'''	
					SET @sWhere= @sWhere + '  OR EPI.NumeroNosologico like ''%' + @sFiltroGenerico + '%'''	
					SET @sWhere= @sWhere + '  OR EPI.NumeroNosologico like ''%' + dbo.MF_ModificaNosologico(@sFiltroGenerico)+ '%'''	
					SET @sWhere= @sWhere + '  OR EPI.NumeroListaAttesa like ''%' + @sFiltroGenerico + '%'''	
							
					SET @sWhere= @sWhere + '  OR P.CodiceFiscale like ''%' + @sFiltroGenerico + '%'''		
					SET @sWhere= @sWhere + '  OR C.NumeroCartella like ''%' + dbo.MF_NumeroCartellaDaBarcode(@sFiltroGenerico) + '%'''							
					
											SET @sWhere= @sWhere + '     )' 					
				END

		IF ISNULL(@sWhere,'')<> ''
	BEGIN	
		SET @sSQL=@sSQL +' WHERE ' + RIGHT(@sWhere,len(@sWhere)-5)
	END	
	
	PRINT @sSQL
	EXEC (@sSQL)
	
		SET @sSQL='SELECT
					M.ID,
					P.IDPaziente,				
					M.IDEpisodio,
					M.IDTrasferimento,
					TRA.IDCartella,
						ISNULL(P.Cognome,'''') + '' '' + ISNULL(P.Nome,'''') + '' ('' + ISNULL(P.Sesso,'''') + '') '' +
						CASE
							WHEN P.DataNascita IS NOT NULL THEN '' ('' + CONVERT(VARCHAR(10),P.DataNascita,105) + '') ''
							ELSE ''''
						END +
						'' - '' + ISNULL(UA.Descrizione,'''') + 
						'' - '' + ISNULL(EPI.NumeroNosologico,'''') 
					AS DescrPaziente,
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
					M.CodUtenteRilevazione AS CodLogin,
					L.Descrizione AS DescrUtente,
					M.DataInserimento,
					M.DataInserimentoUTC, 	
					M.DataValidazione,
					M.DataValidazioneUTC,
					CASE
						WHEN M.DataValidazione IS NOT NULL THEN ''Validato il '' + Convert(varchar(20),M.DataValidazione,105) + '' '' + Convert(varchar(5),M.DataValidazione,114) + '' ''
						ELSE ''''
					END DescrValidazione,
					CASE
						WHEN M.DataAnnullamento IS NOT NULL THEN ''Annullato il '' + Convert(varchar(20),M.DataAnnullamento,105) + '' '' + Convert(varchar(5),M.DataAnnullamento,114) + '' ''
						ELSE ''''
					END DescrAnnullamento,				
					M.DataAnnullamento,
					M.DataAnnullamentoUTC,
					M.CodUA,
					MS.IDScheda,
					MS.CodScheda,
					MS.Versione,
					MS.AnteprimaRTF,
					CASE 
						WHEN  M.CodUtenteRilevazione=''' + @sCodLogin + ''' AND C.CodStatoCartella=''AP'' AND ISNULL(CodStatoDiario,'''') NOT IN (''CA'',''AN'') THEN 1 						
						ELSE 0
					END AS PermessoModifica,
					CASE
						WHEN ISNULL(FUA.CodUA,'''')='''' THEN 0
						WHEN ISNULL(FUA.CodUA,'''')<>'''' AND C.CodStatoCartella=''AP'' THEN 1
						ELSE 0
					END AS PermessoUAFirma,
										CASE 
						WHEN CodUtenteRilevazione=''' + @sCodLogin + ''' AND  ISNULL(CodStatoDiario,'''') =''VA'' AND C.CodStatoCartella=''AP''	THEN 1 
						ELSE 0
					END AS  PermessoAnnulla,
					CASE 
						WHEN CodUtenteRilevazione=''' + @sCodLogin + ''' AND  ISNULL(CodStatoDiario,'''') =''IC'' AND C.CodStatoCartella=''AP''	AND ISNULL(CodStatoDiario,'''') = ''IC''
						 THEN 1 
						ELSE 0
					END AS  PermessoValida
				FROM					
					 T_MovDiarioClinico	M WITH (NOLOCK)
						INNER JOIN #tmpMovDiarioClinico TMP
							ON M.ID = TMP.IDMovDiarioClinico
						LEFT JOIN T_MovPazienti P WITH (NOLOCK)
							ON (M.IDEpisodio=P.IDEpisodio)													
						LEFT JOIN T_MovEpisodi EPI WITH (NOLOCK)
							ON (M.IDEpisodio=EPI.ID)						
						LEFT JOIN T_MovTrasferimenti TRA WITH (NOLOCK)
							ON (M.IDTrasferimento=TRA.ID)
						LEFT JOIN T_MovCartelle C WITH (NOLOCK)
							ON (TRA.IDCartella=C.ID)		
						LEFT JOIN T_UnitaAtomiche UA WITH (NOLOCK)
							ON (TRA.CodUA=UA.Codice)		
						LEFT JOIN T_TipoDiario T WITH (NOLOCK)
							ON (M.CodTipoDiario=T.Codice)
						LEFT JOIN T_TipoVoceDiario TV WITH (NOLOCK)
							ON (M.CodTipoVoceDiario=TV.Codice)								
						LEFT JOIN T_StatoDiario S WITH (NOLOCK)
							ON (M.CodStatoDiario=S.Codice)	
						LEFT JOIN T_TipoRegistrazione TR WITH (NOLOCK)
							ON (M.CodTipoRegistrazione=TR.Codice)																	
						LEFT JOIN T_Login L WITH (NOLOCK)
							ON (M.CodUtenteRilevazione=L.Codice)	
						LEFT JOIN 
								(SELECT ID As IDScheda, CodScheda,Versione,IDEntita,AnteprimaRTF
								 FROM
									T_MovSchede WITH (NOLOCK)
								 WHERE CodEntita=''DCL'' AND							
									Storicizzata=0 AND
									CodStatoScheda <> ''CA''
								) AS MS
							ON MS.IDEntita=M.ID	
						LEFT JOIN 
								(SELECT CodUA
								 FROM T_AssUAModuli WITH (NOLOCK)
								 WHERE CodModulo=''FirmaD_Diario''
								 ) AS FUA
							ON TRA.CodUA=FUA.CodUA
						'
	IF (@bDatiEstesi=1)
	BEGIN	
		SET @sSQL = @sSQL + ' WHERE 1=0'
	END
	PRINT @sSQL
	EXEC (@sSQL)

					IF (@bDatiEstesi=1)
	BEGIN
				SELECT 
			DISTINCT
			CodTipoVoceDiario AS Codice,
			V.Descrizione AS Descrizione
		FROM #tmpMovDiarioClinico D
				INNER JOIN T_TipoVoceDiario V
					ON D.CodTipoVoceDiario=V.Codice		
		ORDER BY 2

				SELECT 
			DISTINCT
			A.Codice AS Codice,
			A.Descrizione AS Descrizione
		FROM #tmpMovDiarioClinico D			
				INNER JOIN T_UnitaAtomiche A		
					ON D.CodUA = A.Codice
		ORDER BY 2

	END

	DROP TABLE #tmpMovDiarioClinico
				
	EXEC MSP_InsMovTimeStamp @xTimeStamp		  
END