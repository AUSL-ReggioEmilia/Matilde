CREATE PROCEDURE [dbo].[MSP_SelNumeroEpisodioDaNosologicoLA](@xParametri AS XML)
AS
BEGIN
			
	
				
	DECLARE @sNumeroNosologico AS VARCHAR(50)
	DECLARE @sNumeroListaAttesa AS VARCHAR(50)
	
		DECLARE @uIDEpisodio AS UNIQUEIDENTIFIER
	DECLARE @xPar AS XML
	
		CREATE TABLE #tmpEpisodiCollegati		
	(
		IDEpisodio UNIQUEIDENTIFIER
	)
	
	
		SET @sNumeroNosologico=(SELECT TOP 1 ValoreParametro.NumeroNosologico.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/NumeroNosologico') as ValoreParametro(NumeroNosologico));


		SET @sNumeroListaAttesa=(SELECT TOP 1 ValoreParametro.NumeroListaAttesa.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/NumeroListaAttesa') as ValoreParametro(NumeroListaAttesa));

			IF @sNumeroNosologico IS NOT NULL
		SET @uIDEpisodio= (SELECT TOP 1 ID  FROM T_MovEpisodi WHERE NumeroNosologico=@sNumeroNosologico)

		IF @sNumeroListaAttesa IS NOT NULL AND @uIDEpisodio IS NULL
		SET @uIDEpisodio= (SELECT TOP 1 ID  FROM T_MovEpisodi WHERE NumeroListaAttesa=@sNumeroListaAttesa)
	
	IF @uIDEpisodio IS NOT NULL
		BEGIN
						SET @xPar=CONVERT(XML,'<Parametri>
												<IDEpisodio>' + CONVERT(VARCHAR(50),@uIDEpisodio) + '</IDEpisodio>
									</Parametri>')
										
			INSERT INTO #tmpEpisodiCollegati												 				
							EXEC MSP_SelEpisodiCollegati @xPar	
			SELECT 
				CodAzi,
				NumeroNosologico,
				NumeroListaAttesa,
				ISNULL(NumeroNosologico,NumeroListaAttesa) AS NumeroEpisodio,
				E.ID AS IDEpisodio
			FROM T_MovEpisodi E
					INNER JOIN #tmpEpisodiCollegati C
						ON (E.ID=C.IDEpisodio)
			GROUP BY
				CodAzi,NumeroNosologico,NumeroListaAttesa,E.ID
		END	
	ELSE
		BEGIN
						SELECT 
			 NULL AS CodAzi,
			 @sNumeroNosologico AS NumeroNosologico,
			 @sNumeroListaAttesa AS NumeroListaAttesa,
			 @uIDEpisodio AS IDEpisodio
		END
		
	DROP TABLE #tmpEpisodiCollegati	
	RETURN 0	
END