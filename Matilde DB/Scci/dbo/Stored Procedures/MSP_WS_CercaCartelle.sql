CREATE PROCEDURE [dbo].[MSP_WS_CercaCartelle](@xParametri AS XML)
AS
BEGIN
	

				
	DECLARE @sCodAzi AS Varchar(20)
	DECLARE @sCodRuolo AS Varchar(20)
	DECLARE @sNumeroNosologico AS Varchar(20)
	DECLARE @sNumeroListaAttesa AS Varchar(20)
	DECLARE @sCodLogin AS VARCHAR(100)
	DECLARE @sCodSAC AS VARCHAR(50) 
		
		DECLARE @sSQL AS VARCHAR(MAX)
	DECLARE @sWhere AS VARCHAR(MAX)
	DECLARE @uIDPaziente AS UNIQUEIDENTIFIER
		
		SET @sCodAzi=(SELECT	TOP 1 ValoreParametro.CodAzi.value('.','VARCHAR(20)')
						 FROM @xParametri.nodes('/Parametri/CodAzi') as ValoreParametro(CodAzi))						 
	SET @sCodAzi= LTRIM(RTRIM(ISNULL(@sCodAzi,'')))	
	

	
		SET @sNumeroNosologico=(SELECT	TOP 1 ValoreParametro.NumeroNosologico.value('.','VARCHAR(20)')
						 FROM @xParametri.nodes('/Parametri/NumeroNosologico') as ValoreParametro(NumeroNosologico))						 
	SET @sNumeroNosologico= LTRIM(RTRIM(ISNULL(@sNumeroNosologico,'')))	
	
		SET @sNumeroListaAttesa=(SELECT	TOP 1 ValoreParametro.NumeroListaAttesa.value('.','VARCHAR(20)')
						 FROM @xParametri.nodes('/Parametri/NumeroListaAttesa') as ValoreParametro(NumeroListaAttesa))						 
	SET @sNumeroListaAttesa= LTRIM(RTRIM(ISNULL(@sNumeroListaAttesa,'')))	
	
		SET @sCodSAC=(SELECT	TOP 1 ValoreParametro.CodSAC.value('.','VARCHAR(50)')
						 FROM @xParametri.nodes('/Parametri/CodSAC') as ValoreParametro(CodSAC))						 
	SET @sCodSAC= LTRIM(RTRIM(ISNULL(@sCodSAC,'')))	
				
		SET @sCodRuolo=(SELECT	TOP 1 ValoreParametro.CodRuolo.value('.','VARCHAR(20)')
						 FROM @xParametri.nodes('/Parametri/TimeStamp/CodRuolo') as ValoreParametro(CodRuolo))						 
	SET @sCodRuolo= LTRIM(RTRIM(ISNULL(@sCodRuolo,'')))						 					
	
		SET @sCodLogin=(SELECT	TOP 1 ValoreParametro.CodLogin.value('.','VARCHAR(100)')
						 FROM @xParametri.nodes('/Parametri/TimeStamp/CodLogin') as ValoreParametro(CodLogin))						 
	SET @sCodLogin= LTRIM(RTRIM(ISNULL(@sCodLogin,'')))
	
				
			
					
			
	SET @sSQL=''
	
	SET @sSQL='
		SELECT 
				C.ID AS IDCartella,
				C.NumeroCartella,
				C.CodStatoCartella,
				ST.Descrizione AS DescrizioneStatoCartella,
				CodUtenteApertura,
				CodUtenteChiusura,
				E.NumeroNosologico,
				E.DataRicovero,
				E.DataDimissione,	
				E.NumeroListaAttesa,
				E.DataListaAttesa,
				T.CodUA,
				UA.Descrizione AS DescrizioneUA	
		FROM 
			T_MovCartelle C
				INNER JOIN Q_SelUltimoTrasferimentoCartella  Q
					ON C.ID=Q.IDCartella
				INNER JOIN T_MovEpisodi E
					ON Q.IDEpisodio=E.ID
				INNER JOIN T_MovPazienti P
					ON P.IDEpisodio=E.ID		
				INNER JOIN T_MovTrasferimenti T
					ON Q.IDNumTrasferimento=T.IDNum
				INNER JOIN T_StatoCartella ST
					ON C.CodStatoCartella=ST.Codice
				INNER JOIN T_UnitaAtomiche UA
					ON T.CodUA=UA.Codice
				
		'	  		
	
	SET @sWhere=''
	
		SET @sWhere= @sWhere + ' AND C.CodStatoCartella NOT IN (''CA'') AND
								 E.CodStatoEpisodio NOT IN (''CA'')'	
									 		
		IF ISNULL(@sCodAzi,'') <> ''
	BEGIN
		SET @sWhere= @sWhere + ' AND E.CodAzi = '''+ @sCodAzi + '''' 
	END

		IF ISNULL(@sNumeroNosologico,'') <> ''
	BEGIN
		SET @sWhere= @sWhere + ' AND E.NumeroNosologico = '''+ @sNumeroNosologico + '''' 
	END
	
		IF ISNULL(@sNumeroListaAttesa,'') <> ''
	BEGIN
		SET @sWhere= @sWhere + ' AND E.NumeroListaAttesa = '''+ @sNumeroListaAttesa + '''' 
	END
	
		IF ISNULL(@sCodSAC,'') <> ''
	BEGIN	
	SET @sWhere= @sWhere + ' AND P.IDPaziente IN (SELECT ID FROM T_Pazienti WHERE CodSAC =''' + @sCodSAC + ''')'
	END
	
		IF ISNULL(@sWhere,'')<> ''
	BEGIN		
		SET @sSQL=@sSQL +' WHERE ' + RIGHT(@sWhere,len(@sWhere)-5)		
	END	
	
	PRINT @sSQL
	
	EXEC (@sSQL)
	
		
	SET @sSQL=''
		
		           
	RETURN 0
	
END