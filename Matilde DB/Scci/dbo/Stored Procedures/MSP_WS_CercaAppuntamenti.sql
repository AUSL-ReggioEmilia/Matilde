CREATE PROCEDURE [dbo].[MSP_WS_CercaAppuntamenti](@xParametri AS XML)
AS
BEGIN
					

					
	DECLARE @sCodRuolo AS Varchar(20)
	DECLARE @sCodAgenda AS Varchar(20)
	DECLARE @uIDCartella AS UNIQUEIDENTIFIER
	DECLARE @sCodLogin AS VARCHAR(100)
	DECLARE @sCodSAC AS VARCHAR(50) 
	DECLARE @dDataInizio AS DATETIME
	DECLARE @dDataFine AS DATETIME
	DECLARE @sCodTipoAppuntamento AS VARCHAR(20)
		
		DECLARE @sSQL AS VARCHAR(MAX)
	DECLARE @sWhere AS VARCHAR(MAX)
	DECLARE @uIDPaziente AS UNIQUEIDENTIFIER
	DECLARE @sGUID AS VARCHAR(Max)		
	DECLARE @sDataTmp AS VARCHAR(20)
	DECLARE @sSQLQueryAgendeEPI AS VARCHAR(MAX)
	
		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDCartella.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDCartella') as ValoreParametro(IDCartella))	
	IF 	ISNULL(@sGUID,'') <> '' 
		SET @uIDCartella=CONVERT(UNIQUEIDENTIFIER,	@sGUID)	
			
		SET @sCodSAC=(SELECT	TOP 1 ValoreParametro.CodSAC.value('.','VARCHAR(50)')
						 FROM @xParametri.nodes('/Parametri/CodSAC') as ValoreParametro(CodSAC))						 
	SET @sCodSAC= LTRIM(RTRIM(ISNULL(@sCodSAC,'')))	
	
		SET @sCodTipoAppuntamento=(SELECT	TOP 1 ValoreParametro.CodTipoAppuntamento.value('.','VARCHAR(20)')
						 FROM @xParametri.nodes('/Parametri/CodTipoAppuntamento') as ValoreParametro(CodTipoAppuntamento))						 
	SET @sCodTipoAppuntamento= LTRIM(RTRIM(ISNULL(@sCodTipoAppuntamento,'')))
	
		SET @sCodAgenda=(SELECT	TOP 1 ValoreParametro.CodAgenda.value('.','VARCHAR(20)')
						 FROM @xParametri.nodes('/Parametri/CodAgenda') as ValoreParametro(CodAgenda))						 
	SET @sCodAgenda= LTRIM(RTRIM(ISNULL(@sCodAgenda,'')))
	
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
	
												
			
					
		SET @sSQLQueryAgendeEPI=(SELECT dbo.MF_SQLQueryAgendePAZ())
			
	SET @sSQL=''
	
	SET @sSQL=' SELECT
						M.ID AS IDAppuntamento,	
						TRAS.IDCartella,
						CAR.NumeroCartella,											
						M.DataInizio,
						M.DataFine,						
						M.ElencoRisorse,	
						QEPI.Oggetto,	
						M.CodTipoAppuntamento AS CodTipoAppuntamento,
						T.Descrizione AS DescrizioneTipoAppuntamento,								
						M.CodStatoAppuntamento AS CodStatoAppuntamento,
						S.Descrizione As DescrizioneStatoAppuntamento,
														
						M.CodUtenteRilevazione AS CodUtenteRilevazione,
						L.Descrizione AS DescrizioneUtenteRilevazione,
						
						M.CodUtenteUltimaModifica,
						LM.Descrizione AS DescrizioneUtenteUltimaModifica
				FROM 
						T_MovAppuntamenti M						
							LEFT JOIN T_Pazienti P
								ON (M.IDPaziente=P.ID)									
							LEFT JOIN T_MovTrasferimenti TRAS
								ON (M.IDEpisodio=M.IDEpisodio AND
									M.IDTrasferimento=TRAS.ID)
							LEFT JOIN T_MovCartelle CAR
								ON (TRAS.IDCartella=CAR.ID)
							LEFT JOIN 							
								 (' + @sSQLQueryAgendeEPI + ')  AS QEPI													
								ON QEPI.IDPaziente=M.IDPaziente
							LEFT JOIN T_TipoAppuntamento T
								ON M.CodTipoAppuntamento=T.Codice
							LEFT JOIN T_StatoAppuntamento S
								ON M.CodStatoAppuntamento=S.Codice	
							LEFT JOIN T_Login L
								ON (M.CodUtenteRilevazione=L.Codice)	
							LEFT JOIN T_Login LM
								ON (M.CodUtenteUltimaModifica=LM.Codice)		
			'	  		
	
	SET @sWhere=''
		
		IF ISNULL(@sCodAgenda,'') <> ''
		BEGIN							 				
			SET @sWhere= @sWhere +  ' AND M.ID IN (SELECT IDAppuntamento 
												FROM T_MovAppuntamentiAgende 
												WHERE IDAppuntamento=M.ID AND
													CodStatoAppuntamentoAgenda <> ''CA'' AND
													CodAgenda='''+ @sCodAgenda + '''
												)' 													
		END
		
		IF ISNULL(@sCodTipoAppuntamento,'') <> ''
		BEGIN							 				
			SET @sWhere= @sWhere +  ' AND M.CodTipoAppuntamento ='''+ @sCodTipoAppuntamento + '''' 													
		END
						
		IF @dDataInizio IS NOT NULL 
		BEGIN		
			SET @sWhere= @sWhere + ' AND M.DataEvento >= CONVERT(datetime,'''  + convert(varchar(20),@dDataInizio,120) +''',120)'									
		END

		IF @dDataFine IS NOT NULL 
		BEGIN		
			SET @sWhere= @sWhere + ' AND M.DataEvento <= CONVERT(datetime,'''  + convert(varchar(20),@dDataFine,120) +''',120)'												
		END	
		
		SET @sWhere= @sWhere + ' AND M.CodStatoAppuntamento NOT IN (''CA'')'	
									 				
		IF ISNULL(@sCodSAC,'') <> ''
	BEGIN			
		SET @sWhere= @sWhere + ' AND M.IDPaziente IN (SELECT ID FROM T_Pazienti WHERE CodSAC =''' + @sCodSAC + ''')'
	END
	
		IF @uIDCartella IS NOT NULL
	BEGIN
		SET @sWhere= @sWhere + ' AND TRAS.IDCartella =''' + CONVERT(VARCHAR(50),@uIDCartella) + ''''
	END	
	
		IF ISNULL(@sWhere,'')<> ''
	BEGIN				
		SET @sSQL=@sSQL +' WHERE ' + RIGHT(@sWhere,len(@sWhere)-5)		
	END	
	ELSE
		SET @sSQL=@sSQL +' WHERE 1=0'
	
	PRINT @sSQL
	
	EXEC (@sSQL)
	
		
	SET @sSQL=''
		
		           
	RETURN 0
	
END