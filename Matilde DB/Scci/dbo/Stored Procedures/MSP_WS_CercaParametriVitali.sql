CREATE PROCEDURE [dbo].[MSP_WS_CercaParametriVitali](@xParametri AS XML)
AS
BEGIN
	

				
	DECLARE @sCodRuolo AS Varchar(20)
	DECLARE @sCodLogin AS VARCHAR(100)
	DECLARE @uIDCartella AS UNIQUEIDENTIFIER	
	DECLARE @sCodTipoParametroVitale AS VARCHAR(20)
	DECLARE @dDataInizio AS DATETIME
	DECLARE @dDataFine AS DATETIME
	

	
		DECLARE @sSQL AS VARCHAR(MAX)
	DECLARE @sWhere AS VARCHAR(MAX)
	DECLARE @sGUID AS VARCHAR(Max)
	DECLARE @sDataTmp AS VARCHAR(20)
							  		
		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDCartella.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDCartella') as ValoreParametro(IDCartella))	
	IF 	ISNULL(@sGUID,'') <> '' SET @uIDCartella=CONVERT(UNIQUEIDENTIFIER,	@sGUID)	
	
		SET @sCodTipoParametroVitale=(SELECT	TOP 1 ValoreParametro.CodTipoParametroVitale.value('.','VARCHAR(20)')
						 FROM @xParametri.nodes('/Parametri/CodTipoParametroVitale') as ValoreParametro(CodTipoParametroVitale))						 
	SET @sCodTipoParametroVitale= LTRIM(RTRIM(ISNULL(@sCodTipoParametroVitale,'')))	
	
		
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
	
		IF @sCodTipoParametroVitale NOT IN ('')
		BEGIN							 				
			SET @sWhere= @sWhere +  ' AND M.CodTipoParametroVitale ='''+ @sCodTipoParametroVitale + '''' 													
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
						M.ID AS IDParametroVitale,
						TRASF.IDCartella,
						CAR.NumeroCartella,
						M.DataEvento,
						M.DataInserimento,
						M.CodUtenteRilevazione,
						L.Descrizione AS DescrizioneUtenteRilevazione,
						M.DataUltimaModifica AS DataUltimaModifica,
						M.CodUtenteUltimaModifica,
						LM.Descrizione AS DescrizioneUtenteUltimaModifica,
						M.CodTipoParametroVitale,
						T.Descrizione AS DescrizioneTipoParametroVitale,
						M.CodStatoParametroVitale,
						S.Descrizione As DescrizioneStatoParametroVitale,	
						M.CodSistema,
						M.IDSistema
					'

	SET @sSQL=@sSQL + '
				FROM
						T_MovParametriVitali M
						LEFT JOIN T_MovTrasferimenti TRASF
								ON (M.IDTrasferimento=TRASF.ID)
						LEFT JOIN T_MovCartelle CAR
								ON (TRASF.IDCartella=CAR.ID)		
						LEFT JOIN
								(SELECT ID As IDScheda, CodScheda,Versione,IDEntita,AnteprimaRTF
								 FROM
									T_MovSchede
								 WHERE CodEntita=''PVT'' AND
									Storicizzata=0
								) AS MS
							ON MS.IDEntita=M.ID
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
					  
	SET @sWhere= @sWhere + ' AND M.CodStatoParametroVitale NOT IN (''CA'')
						     AND TRASF.IDCartella =''' + CONVERT(VARCHAR(50),@uIDCartella) + '''
							'
		IF ISNULL(@sWhere,'')<> ''
	BEGIN
		SET @sSQL=@sSQL +' WHERE ' + RIGHT(@sWhere,len(@sWhere)-5)
	END	

	PRINT @sSQL					
	EXEC (@sSQL)

	RETURN 0
	
END