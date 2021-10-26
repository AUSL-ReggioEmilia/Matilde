CREATE PROCEDURE [dbo].[MSP_CercaTrasferimentoPrecedente](@xParametri XML)
AS
BEGIN


		

				
	DECLARE @uIDTrasferimento AS UNIQUEIDENTIFIER	
	
		DECLARE @sGUID AS VARCHAR(50)
	DECLARE @dDataIngresso AS DATETIME
	DECLARE @uIDEpisodio AS UNIQUEIDENTIFIER	
	
		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDTrasferimento.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDTrasferimento') as ValoreParametro(IDTrasferimento))
	IF 	ISNULL(@sGUID,'') <> '' SET @uIDTrasferimento=CONVERT(UNIQUEIDENTIFIER,	@sGUID)	
	
	
	IF @uIDTrasferimento IS NOT NULL
	BEGIN
		
				SELECT TOP 1
			 @uIDEpisodio=IDEpisodio,
			 @dDataIngresso=DataIngresso 
		FROM T_MovTrasferimenti  
		WHERE ID=@uIDTrasferimento
		
				CREATE TABLE #TmpID
			(ID  UNIQUEIDENTIFIER NOT NULL,
			 DataIngresso  DATETIME NULL)

				CREATE INDEX IX_Data
				ON #TmpID (DataIngresso); 
		
		INSERT INTO #TmpID	(ID,DataIngresso)
		SELECT ID,DataIngresso
				 FROM T_MovTrasferimenti
				 WHERE 
					  IDEpisodio=@uIDEpisodio									  AND ISNULL(DataIngresso,getdate()) <= @dDataIngresso							  AND ID <> @uIDTrasferimento								  AND CodStatoTrasferimento NOT IN ('SS','CA','PT') 				 ORDER BY DataIngresso  DESC								 
				SELECT 
			TOP 1 ID AS IDPrecedente
			FROM 
				(SELECT TOP 1 ID 
				 FROM #TmpID	
				 ORDER BY DataIngresso DESC									) AS Q
	
		DROP TABLE #TmpID
	END

				
END