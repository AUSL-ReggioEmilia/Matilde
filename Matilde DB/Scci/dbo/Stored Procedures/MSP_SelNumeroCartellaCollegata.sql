CREATE PROCEDURE [dbo].[MSP_SelNumeroCartellaCollegata](@xParametri XML)
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

				
	IF 	@uIDCartella IS NOT NULL
	BEGIN
	
		CREATE TABLE #tmpCartelle		
		(
			IDCartella UNIQUEIDENTIFIER
		)
	
		SET @sNumeroCartella=(SELECT TOP 1 NumeroCartella FROM T_MovCartelle WHERE ID=@uIDCartella)
		
		SET @xPar=CONVERT(XML,'<Parametri>
									<IDCartella>' + CONVERT(VARCHAR(50),@uIDCartella) + '</IDCartella>
									</Parametri>')
									
		
		INSERT INTO #tmpCartelle 					
			EXEC MSP_SelCartelleCollegate @xPar
	
		SET @nQta =(SELECT COUNT(IDCartella) FROM #tmpCartelle)		
		
				
				
										
		SET @sOut=@sNumeroCartella + '_' + CONVERT(VARCHAR(10),@nQta+1)
		DROP TABLE #tmpCartelle
	END

	SELECT @sOut AS NumeroCartella

	
	RETURN 0
END