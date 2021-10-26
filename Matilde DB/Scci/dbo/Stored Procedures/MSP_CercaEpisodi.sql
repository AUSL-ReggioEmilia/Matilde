CREATE PROCEDURE [dbo].[MSP_CercaEpisodi](@xParametri AS XML)
AS
BEGIN



	DECLARE @sCodLogin AS VARCHAR(100)		
	DECLARE @sCodRuolo AS VARCHAR(20)	
	DECLARE @sSQL AS VARCHAR(MAX)
		
	DECLARE @dDataDimissioneInizio AS DATETIME
	DECLARE @dDataDimissioneFine AS DATETIME
	DECLARE @sOrdinamento AS Varchar(500)

	DECLARE @uIDPaziente AS UNIQUEIDENTIFIER	

	DECLARE @sCodStatoEpisodio AS Varchar(MAX)	
	DECLARE @sCodUA AS Varchar(MAX)	

		DECLARE @sGUID AS VARCHAR(Max)
	DECLARE @sDataTmp AS VARCHAR(20)

		SET @sCodLogin=(SELECT TOP 1 ValoreParametro.CodLogin.value('.','VARCHAR(100)')
					  FROM @xParametri.nodes('/Parametri/TimeStamp/CodLogin') as ValoreParametro(CodLogin))	
	SET @sCodLogin=ISNULL(@sCodLogin,'')
	
		SET @sCodRuolo=(SELECT TOP 1 ValoreParametro.CodRuolo.value('.','VARCHAR(100)')
					  FROM @xParametri.nodes('/Parametri/TimeStamp/CodRuolo') as ValoreParametro(CodRuolo))	
	SET @sCodRuolo=ISNULL(@sCodRuolo,'')
	
		

		SET @sDataTmp=(SELECT TOP 1 ValoreParametro.DataDimissioneInizio.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/DataDimissioneInizio') as ValoreParametro(DataDimissioneInizio))					  
	SET @sDataTmp=ISNULL(@sDataTmp,'')

	IF @sDataTmp<> '' 
		BEGIN
						SET @sDataTmp= SUBSTRING(@sDataTmp,4,2) + '-' 
						+ LEFT(@sDataTmp,2) + '-' +
						+ SUBSTRING(@sDataTmp,7,4) 
									IF ISDATE(@sDataTmp)=1
				SET	@dDataDimissioneInizio=CONVERT(DATETIME,@sDataTmp,120)						
			ELSE
				SET	@dDataDimissioneFine =NULL			
		END

		SET @sDataTmp=(SELECT TOP 1 ValoreParametro.DataDimissioneFine.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/DataDimissioneFine') as ValoreParametro(DataDimissioneFine))
	SET @sDataTmp=ISNULL(@sDataTmp,'')
	IF @sDataTmp<> '' 
		BEGIN
						SET @sDataTmp= SUBSTRING(@sDataTmp,4,2) + '-' 
						+ LEFT(@sDataTmp,2) + '-' +
						+ SUBSTRING(@sDataTmp,7,4)
									IF ISDATE(@sDataTmp)=1
				SET	@dDataDimissioneFine=CONVERT(DATETIME,@sDataTmp,120)						
			ELSE
				SET	@dDataDimissioneFine =NULL		
		END 

		SET @sCodStatoEpisodio=''
	SELECT	@sCodStatoEpisodio =  @sCodStatoEpisodio +
														CASE 
								WHEN @sCodStatoEpisodio='' THEN ''
								ELSE  ','
							END + ''''
						  + ValoreParametro.CodStatoEpisodio.value('.','VARCHAR(1800)')
						  + ''''
						 FROM @xParametri.nodes('/Parametri/CodStatoEpisodio') as ValoreParametro(CodStatoEpisodio)	
	SET @sCodStatoEpisodio=LTRIM(RTRIM(@sCodStatoEpisodio))
	IF	@sCodStatoEpisodio='''''' SET @sCodStatoEpisodio=''

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

		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDPaziente.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDPaziente') as ValoreParametro(IDPaziente))
	IF 	ISNULL(@sGUID,'') <> '' SET @uIDPaziente=CONVERT(UNIQUEIDENTIFIER,	@sGUID)	


	
	
				
				DECLARE @xTmp AS XML

					
		
			
	
	
		CREATE TABLE #tmpFiltriEpisodi
		(
			IDEpisodio uniqueidentifier NOT NULL,
			IDTrasferimento uniqueidentifier NOT NULL
		)

	SET @sSQL='INSERT #tmpFiltriEpisodi(IDEpisodio,IDTrasferimento) ' 

	SET @sSQL= @sSQL + 
				'SELECT	
					 T.IDEpisodio AS IDEpisodio,
					 T.ID AS IDTrasferimento		
						  
			 FROM T_MovEpisodi E				
			 
					INNER JOIN T_MovTrasferimenti T
						ON E.ID=T.IDEpisodio
					
					INNER JOIN T_MovPazienti P
						ON E.ID=P.IDEpisodio															 												
			'		
	
				
	DECLARE @sWhere AS VARCHAR(Max)
	
	SET @sWhere=''
	
		IF @dDataDimissioneInizio IS NOT NULL 
		BEGIN					
			SET @sWhere= @sWhere + ' AND E.DataDimissione >= CONVERT(datetime,'''  + convert(varchar(20),@dDataDimissioneInizio,120) +''',120)'								
		END

		IF @dDataDimissioneFine IS NOT NULL 
		BEGIN						
			SET @sWhere= @sWhere + ' AND E.DataDimissione <= CONVERT(datetime,'''  + convert(varchar(20),@dDataDimissioneFine,120) +''',120)'									
		END	

		IF ISNULL(@sCodStatoEpisodio,'') <> ''
	BEGIN
		SET @sWhere= @sWhere + ' AND E.CodStatoEpisodio IN ('+ @sCodStatoEpisodio + ')' 
	END

		IF ISNULL(@sCodUA,'') <> ''
	BEGIN
		SET @sWhere= @sWhere + ' AND T.CodUA IN ('+ @sCodUA + ')' 
	END

		SET @sOrdinamento=(SELECT	TOP 1 ValoreParametro.Ordinamento.value('.','VARCHAR(500)')
						 FROM @xParametri.nodes('/Parametri/Ordinamento') as ValoreParametro(Ordinamento))						 
	SET @sOrdinamento= ISNULL(@sOrdinamento,'')
	SET @sOrdinamento=LTRIM(RTRIM(@sOrdinamento))

		IF @uIDPaziente IS NOT NULL
	BEGIN
		SET @sWhere= @sWhere + 
						' AND 
							(P.IDPaziente=''' + CONVERT(VARCHAR(50),@uIDPaziente) + '''
							OR
														P.IDPaziente IN 
									(SELECT IDPazienteVecchio
										FROM T_PazientiAlias WITH (NOLOCK)
										WHERE 
										IDPaziente IN 
											(SELECT IDPaziente
												FROM T_PazientiAlias  WITH (NOLOCK)
												WHERE IDPazienteVecchio=''' + CONVERT(VARCHAR(50),@uIDPaziente) + '''
											)
									)
							)'
	END
	
		IF ISNULL(@sWhere,'')<> ''
	BEGIN		
		SET @sSQL=@sSQL +' WHERE ' + RIGHT(@sWhere,len(@sWhere)-5)	
	END	

	PRINT @sSQL

	EXEC (@sSQL)

	
	SET @sSQL ='
			SELECT 
				E.ID AS IDEpisodio,
				P.ID AS IDPaziente,
				E.NumeroNosologico, 
				E.NumeroListaAttesa, 
				E.DataListaAttesa,
				E.DataRicovero,
				E.DataDimissione,
				NULL AS DescrDimissione,
				NULL AS DescrRicovero,
				RD.CodUA AS CodRepartoDimissione, 
				ARD.Descrizione DescrRepartoDimissione
			FROM T_MovEpisodi E
					INNER JOIN T_MovPazienti P
						ON E.ID=P.IDEpisodio 					

					LEFT JOIN T_MovTrasferimenti RD
						ON (E.ID=RD.IDEpisodio AND
							RD.CodStatoTrasferimento=''DM''
							)
					LEFT JOIN T_UnitaAtomiche ARD
						ON (ARD.Codice = RD.CodUA
							)
			WHERE 
				E.ID IN (SELECT IDEpisodio FROM #tmpFiltriEpisodi)
		'
	
		IF @sOrdinamento<> ''
		SET @sSQL = @sSQL + ' ORDER BY ' + @sOrdinamento + ''

	PRINT @sSQL

	EXEC (@sSQL)
END