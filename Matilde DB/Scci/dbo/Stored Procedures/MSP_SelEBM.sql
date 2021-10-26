CREATE PROCEDURE [dbo].[MSP_SelEBM](@xParametri XML)
AS
BEGIN
		
	
				DECLARE @sCodUA AS VARCHAR(20)
	DECLARE @sCodRuolo AS Varchar(20)
	
		DECLARE @sCodEntita AS Varchar(20)
	
		SET @sCodUA=(SELECT TOP 1 ValoreParametro.CodUA.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodUA') as ValoreParametro(CodUA))	

		SET @sCodRuolo=(SELECT	TOP 1 ValoreParametro.CodRuolo.value('.','VARCHAR(20)')
					FROM @xParametri.nodes('/Parametri/CodRuolo') as ValoreParametro(CodRuolo)) 
	
				CREATE TABLE #tmpUA
	(
		CodUA VARCHAR(20) COLLATE Latin1_General_CI_AS,
		Descrizione VARCHAR(255) COLLATE Latin1_General_CI_AS
	)

	DECLARE @xTmp AS XML
	
	SET @xTmp =CONVERT(XML,'<Parametri><CodUA>'+@sCodUA+'</CodUA></Parametri>')	
	INSERT #tmpUA EXEC MSP_SelUAPadri @xTmp	
	
	CREATE INDEX IX_CodUA ON #tmpUA (CodUA)    
	
					SET @sCodEntita='EBM'
		
	SELECT	
			M.*
	FROM T_EBM M
	WHERE
		Codice IN (SELECT DISTINCT
						A.CodVoce AS Codice		
					FROM	
						T_AssUAEntita A
							INNER JOIN #tmpUA T ON
								(A.CodUA=T.CodUA)
					WHERE A.CodEntita=@sCodEntita
				   )
	ORDER BY Ordine,Descrizione		
	
END