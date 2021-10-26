CREATE PROCEDURE [dbo].[MSP_EsisteListaAttesa](@xParametri XML)
AS
BEGIN
		
	
	
		DECLARE @sCodAzi AS VARCHAR(20)
	DECLARE @sNumeroListaAttesa AS VARCHAR(20)
	
		DECLARE @sGUID AS VARCHAR(50)
	DECLARE @uIDEpisodio AS UNIQUEIDENTIFIER
	
	DECLARE @nRet AS INTEGER
	
	DECLARE @sTmp As VARCHAR(1800)
	DECLARE @xPar  AS XML
	DECLARE @xParMovPaz  AS XML
	DECLARE @xTemp  AS XML
	DECLARE @sRisultato AS VARCHAR(MAX)
	
	SET @sRisultato=''
	
	SET @uIDEpisodio=NULL
	SET @nRet=0
	
				
		SET @sCodAzi=(SELECT TOP 1 ValoreParametro.CodAzi.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodAzi') as ValoreParametro(CodAzi))	
	
		SET @sNumeroListaAttesa=(SELECT TOP 1 ValoreParametro.NumeroListaAttesa.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/NumeroListaAttesa') as ValoreParametro(NumeroListaAttesa))
	SET @sNumeroListaAttesa=ISNULL(@sNumeroListaAttesa,'')
	
					
	IF @sNumeroListaAttesa <> '' 
		BEGIN										
												SET @uIDEpisodio=(SELECT TOP 1 ID 
								FROM T_MovEpisodi 
								WHERE (NumeroListaAttesa=@sNumeroListaAttesa)
									   AND
									   CodAzi=ISNULL(@sCodAzi,CodAzi)
									)
			
			IF @uIDEpisodio IS NULL
				SET @nRet=0
			ELSE
				SET @nRet=1				
									 
		END	
	ELSE
		BEGIN
						PRINT 'Parametri errati'			
		END	
	
	SELECT 	@nRet AS Esiste, 
			@uIDEpisodio AS IDEpisodio					
	RETURN 0
END