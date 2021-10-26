CREATE PROCEDURE [dbo].[MSP_CercaCartellaSuPaziente](@xParametri XML)
AS
BEGIN


		

				
	DECLARE @uIDPaziente AS UNIQUEIDENTIFIER
	DECLARE @sCodUA AS VARCHAR(20)
	DECLARE @sCodRuolo AS Varchar(20)
	
		DECLARE @sGUID AS VARCHAR(50)
	DECLARE @sSQL AS VARCHAR(MAX)
	DECLARE @sWhere AS VARCHAR(MAX)
	DECLARE @sTmp AS VARCHAR(Max)		
	
	SET @uIDPaziente=NULL	
	
		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDPaziente.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDPaziente') as ValoreParametro(IDPaziente))
	IF 	ISNULL(@sGUID,'') <> '' SET @uIDPaziente=CONVERT(UNIQUEIDENTIFIER,	@sGUID)	
	
		SET @sCodRuolo=(SELECT	TOP 1 ValoreParametro.CodRuolo.value('.','VARCHAR(20)')
						 FROM @xParametri.nodes('/Parametri/TimeStamp/CodRuolo') as ValoreParametro(CodRuolo))						 
	SET @sCodRuolo= LTRIM(RTRIM(ISNULL(@sCodRuolo,'')))	

				
	IF @uIDPaziente IS NOT NULL
	BEGIN
	
								DECLARE @xTmp AS XML
			
		CREATE TABLE #tmpUARuolo
		(
			CodUA VARCHAR(20) COLLATE Latin1_General_CI_AS,
			Descrizione VARCHAR(255) COLLATE Latin1_General_CI_AS
		)

		SET  @xTmp=CONVERT(XML,'<Parametri><CodRuolo>'+ @sCodRuolo + '</CodRuolo></Parametri>')
	

		INSERT #tmpUARuolo EXEC MSP_SelUADaRuolo @xTmp
	
		CREATE INDEX IX_CodUA ON #tmpUARuolo (CodUA)   

												CREATE TABLE #tmpPazienti
		(
				IDPaziente UNIQUEIDENTIFIER
		)
				INSERT INTO #tmpPazienti(IDPaziente)
		VALUES (@uIDPaziente)
			
		INSERT INTO #tmpPazienti(IDPaziente)
			SELECT IDPazienteVecchio
				FROM T_PazientiAlias
				WHERE 
				IDPaziente IN 
					(SELECT IDPaziente
						FROM T_PazientiAlias
						WHERE IDPazienteVecchio=@uIDPaziente
					)
			
		CREATE INDEX IX_IDPaziente ON #tmpPazienti (IDPaziente)
		
		
				SELECT 
			C.NumeroCartella,
			T.IDCartella,
			T.ID AS IDTrasferimento,
			T.IDEpisodio
		FROM T_MovTrasferimenti T
				INNER JOIN T_MovPazienti P
					ON T.IDEpisodio=P.IDEpisodio
				INNER JOIN #tmpUARuolo 
					ON (T.CodUA=#tmpUARuolo.CodUA)
				INNER JOIN T_MovCartelle C
					ON T.IDCartella=C.ID
		WHERE 
			P.IDPaziente IN (SELECT IDPaziente FROM #tmpPazienti) AND
			T.CodStatoTrasferimento NOT IN ('CA') AND
			C.CodStatoCartella = 'AP'
	
		DROP TABLE #tmpUARuolo
		DROP TABLE #tmpPazienti
	END
					
END