CREATE PROCEDURE [dbo].[MSP_WS_CercaDiarioClinico](@xParametri AS XML)
AS
BEGIN
	

				
	DECLARE @sCodRuolo AS Varchar(20)
	DECLARE @sCodLogin AS VARCHAR(100)
	DECLARE @uIDCartella AS UNIQUEIDENTIFIER
	DECLARE @sCodTipoVoceDiario AS VARCHAR(20)
	DECLARE @sCodTipoDiario AS VARCHAR(20)
	DECLARE @dDataInizio AS DATETIME
	DECLARE @dDataFine AS DATETIME
	

	
		DECLARE @sSQL AS VARCHAR(MAX)
	DECLARE @sWhere AS VARCHAR(MAX)
	DECLARE @sGUID AS VARCHAR(Max)
	DECLARE @sDataTmp AS VARCHAR(20)
							  		
		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDCartella.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDCartella') as ValoreParametro(IDCartella))	
	IF 	ISNULL(@sGUID,'') <> '' SET @uIDCartella=CONVERT(UNIQUEIDENTIFIER,	@sGUID)	
	
		SET @sCodTipoDiario=(SELECT	TOP 1 ValoreParametro.CodTipoDiario.value('.','VARCHAR(20)')
						 FROM @xParametri.nodes('/Parametri/CodTipoDiario') as ValoreParametro(CodTipoDiario))						 
	SET @sCodTipoDiario= LTRIM(RTRIM(ISNULL(@sCodTipoDiario,'')))	
	
		SET @sCodTipoVoceDiario=(SELECT	TOP 1 ValoreParametro.CodTipoVoceDiario.value('.','VARCHAR(20)')
						 FROM @xParametri.nodes('/Parametri/CodTipoVoceDiario') as ValoreParametro(CodTipoVoceDiario))						 
	SET @sCodTipoVoceDiario= LTRIM(RTRIM(ISNULL(@sCodTipoVoceDiario,'')))	
	
		
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
		
		SET @sCodRuolo=(SELECT	TOP 1 ValoreParametro.CodRuolo.value('.','VARCHAR(20)')
						 FROM @xParametri.nodes('/Parametri/TimeStamp/CodRuolo') as ValoreParametro(CodRuolo))						 
	SET @sCodRuolo= LTRIM(RTRIM(ISNULL(@sCodRuolo,'')))						 					
	
		SET @sCodLogin=(SELECT	TOP 1 ValoreParametro.CodLogin.value('.','VARCHAR(100)')
						 FROM @xParametri.nodes('/Parametri/TimeStamp/CodLogin') as ValoreParametro(CodLogin))						 
	SET @sCodLogin= LTRIM(RTRIM(ISNULL(@sCodLogin,'')))
	
					SET @sWhere=''
	
		IF @sCodTipoDiario NOT IN ('')
		BEGIN							 				
			SET @sWhere= @sWhere +  ' AND M.CodTipoDiario ='''+ @sCodTipoDiario + '''' 													
		END
		
		IF @sCodTipoVoceDiario NOT IN ('')
		BEGIN							 				
			SET @sWhere= @sWhere +  ' AND M.CodTipoVoceDiario ='''+ @sCodTipoVoceDiario + '''' 													
		END
				
		IF @dDataInizio IS NOT NULL 
		BEGIN		
			SET @sWhere= @sWhere + ' AND M.DataEvento >= CONVERT(datetime,'''  + convert(varchar(20),@dDataInizio,120) +''',120)'									
		END

		IF @dDataFine IS NOT NULL 
		BEGIN		
			SET @sWhere= @sWhere + ' AND M.DataEvento <= CONVERT(datetime,'''  + convert(varchar(20),@dDataFine,120) +''',120)'												
		END	
			
			
		
	SET @sSQL='	SELECT
					M.ID AS IDDiarioClinico,	
					TRA.IDCartella,
					CAR.NumeroCartella,				
					M.DataEvento,
					M.DataEventoUTC,
					M.CodTipoDiario AS CodTipoDiario,
					T.Descrizione AS DescrizioneTipoDiario,
					M.CodTipoVoceDiario AS CodTipoVoceDiario,
					TV.Descrizione AS DescrizioneTipoVoceDiario, 					 					
					M.CodTipoRegistrazione,
					TR.Descrizione As DescrizioneTipoRegistrazione,
					M.CodStatoDiario,
					S.Descrizione As DescrizioneStatoDiario,					
					M.CodUtenteRilevazione,
					L.Descrizione AS DescrizioneUtenteRilevazione,
					M.DataInserimento,					
					M.DataValidazione,					
					M.DataAnnullamento,		
				
					M.CodEntitaRegistrazione,	
					M.IDEntitaRegistrazione,
												
					M.CodSistema,
					M.IDSistema
				
					'	
	
	SET @sSQL=@sSQL + '
				FROM 					
						T_MovDiarioClinico	M																					
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
						LEFT JOIN T_MovTrasferimenti TRA
							ON M.IDTrasferimento=TRA.ID	
						LEFT JOIN T_MovCartelle CAR
							ON TRA.IDCartella=CAR.ID
						
					  '
	
	
	SET @sWhere= @sWhere + ' AND TRA.CodStatoTrasferimento <> ''CA'' 
							 AND M.CodStatoDiario <> ''CA'' 
						     AND TRA.IDCartella =''' + CONVERT(VARCHAR(50),@uIDCartella) + '''
							'						
		IF ISNULL(@sWhere,'')<> ''
	BEGIN
	
		SET @sSQL=@sSQL +' WHERE ' + RIGHT(@sWhere,len(@sWhere)-5)
	END	
			
		
	PRINT @sSQL					
	EXEC (@sSQL)
	
	RETURN 0
	
END