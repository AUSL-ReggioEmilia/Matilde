
CREATE PROCEDURE [dbo].[MSP_CercaUltimoTrasferimento](@xParametri XML)
AS
BEGIN


		

				DECLARE @uIDEpisodio AS UNIQUEIDENTIFIER
	DECLARE @sCodStatoTrasferimento AS VARCHAR(20)
	
		DECLARE @sGUID AS VARCHAR(50)

		
	
		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDEpisodio.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDEpisodio') as ValoreParametro(IDEpisodio))
	IF 	ISNULL(@sGUID,'') <> '' SET @uIDEpisodio=CONVERT(UNIQUEIDENTIFIER,	@sGUID)	
	
		SET @sCodStatoTrasferimento=(SELECT TOP 1 ValoreParametro.CodStatoTrasferimento.value('.','VARCHAR(200)')
					  FROM @xParametri.nodes('/Parametri/CodStatoTrasferimento') as ValoreParametro(CodStatoTrasferimento))
	SET @sCodStatoTrasferimento=ISNULL(@sCodStatoTrasferimento,'')
	
	IF @uIDEpisodio IS NOT NULL
	BEGIN				
		
				CREATE TABLE #TmpID
			(ID  UNIQUEIDENTIFIER NOT NULL,
			 DataIngresso  DATETIME NULL)

				CREATE INDEX IX_Data
				ON #TmpID (DataIngresso); 
		
		INSERT INTO #TmpID	(ID,DataIngresso)
		SELECT ID,DataIngresso
				 FROM T_MovTrasferimenti
				 WHERE 
					  IDEpisodio=@uIDEpisodio												  AND CodStatoTrasferimento NOT IN ('SS','CA')							  
					  AND CodStatoTrasferimento=												 (CASE  
							WHEN @sCodStatoTrasferimento='' THEN CodStatoTrasferimento
							ELSE @sCodStatoTrasferimento
						  END	
						  )	
				 ORDER BY DataIngresso  DESC								 
				SELECT 
			TOP 1 ID AS IDUltimo
			FROM 
				(SELECT TOP 1 ID 
				 FROM #TmpID	
				 ORDER BY DataIngresso DESC									) AS Q
	
		DROP TABLE #TmpID
	END

				
END