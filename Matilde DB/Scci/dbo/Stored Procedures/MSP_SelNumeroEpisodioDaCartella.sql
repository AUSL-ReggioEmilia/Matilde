CREATE PROCEDURE [dbo].[MSP_SelNumeroEpisodioDaCartella](@xParametri XML)
AS
BEGIN

	
	
					
	DECLARE @sCodUtente AS VARCHAR(100)	
	DECLARE @uIDCartella AS UNIQUEIDENTIFIER
		
		DECLARE @sGUID AS VARCHAR(Max)	
	DECLARE @sNumeroCartella AS VARCHAR(50)
	DECLARE @sOut AS VARCHAR(50)
	DECLARE @nQta AS INTEGER
	DECLARE @nI AS INTEGER
	DECLARE @xPar AS XML
	DECLARE @sTmp AS VARCHAR(50)

	
		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDCartella.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDCartella') as ValoreParametro(IDCartella))
	IF 	ISNULL(@sGUID,'') <> '' 
		SET @uIDCartella=CONVERT(UNIQUEIDENTIFIER,	@sGUID)	

	SET @sOut=''

				
	
	CREATE TABLE #tmpCartelle		
	(
			IDCartella UNIQUEIDENTIFIER
	)
		
	IF 	@uIDCartella IS NOT NULL
	BEGIN
	
	
		SET @sNumeroCartella=(SELECT TOP 1 NumeroCartella FROM T_MovCartelle WHERE ID=@uIDCartella)
		
		SET @xPar=CONVERT(XML,'<Parametri>
									<IDCartella>' + CONVERT(VARCHAR(50),@uIDCartella) + '</IDCartella>
									</Parametri>')
									
		
		INSERT INTO #tmpCartelle 					
			EXEC MSP_SelCartelleCollegate @xPar	
	
	END

	
	SELECT 
		CodAzi,
		NumeroNosologico,
		NumeroListaAttesa,
		ISNULL(NumeroNosologico,NumeroListaAttesa) AS NumeroEpisodio,
		E.ID AS IDEpisodio
	FROM T_MovEpisodi E
			INNER JOIN T_MovTrasferimenti T
				ON (E.ID=T.IDEpisodio)
			INNER JOIN #tmpCartelle C
				ON (T.IDCartella=C.IDCartella)
	GROUP BY
	CodAzi,NumeroNosologico,NumeroListaAttesa,E.ID
	
	DROP TABLE #tmpCartelle
	
	RETURN 0
END