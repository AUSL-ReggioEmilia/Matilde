CREATE PROCEDURE [dbo].[MSP_SelInfoAppuntamentiAgendeGiorni](@xParametri XML)
AS
BEGIN
	
	
			
	DECLARE @sDataTmp AS VARCHAR(20)
	
	
	DECLARE @dDataInizio datetime
	DECLARE @dDataFine datetime	
	
		
	IF @xParametri.exist('/Parametri/DataInizio')=1
		BEGIN			
			SET @sDataTmp=(SELECT TOP 1 ValoreParametro.DataInizio.value('.','VARCHAR(20)')
							  FROM @xParametri.nodes('/Parametri/DataInizio') as ValoreParametro(DataInizio))					  
			SET @sDataTmp=ISNULL(@sDataTmp,'')
			
			IF @sDataTmp<> '' 
				BEGIN			
										SET @sDataTmp= SUBSTRING(@sDataTmp,4,2) + '-' 
								+ LEFT(@sDataTmp,2) + '-' +
								+ SUBSTRING(@sDataTmp,7,4) +
																+ ' 00:00' 					IF ISDATE(@sDataTmp)=1
						SET	@dDataInizio=CONVERT(DATETIME,@sDataTmp,120)						
					ELSE
						SET	@dDataInizio =NULL			
				END

		END	

			
		IF @xParametri.exist('/Parametri/DataFine')=1
		BEGIN			
			SET @sDataTmp=(SELECT TOP 1 ValoreParametro.DataFine.value('.','VARCHAR(20)')
							  FROM @xParametri.nodes('/Parametri/DataFine') as ValoreParametro(DataFine))					  
			SET @sDataTmp=ISNULL(@sDataTmp,'')
			
			IF @sDataTmp<> '' 
				BEGIN			
										SET @sDataTmp= SUBSTRING(@sDataTmp,4,2) + '-' 
								+ LEFT(@sDataTmp,2) + '-' +
								+ SUBSTRING(@sDataTmp,7,4) +
																+ ' 00:00' 					IF ISDATE(@sDataTmp)=1
						SET	@dDataFine=CONVERT(DATETIME,@sDataTmp,120)						
					ELSE
						SET	@dDataFine =NULL			
				END

		END	

	DECLARE @nGiorni int	
	SET @nGiorni = DATEDIFF(DAY, @dDataInizio, @dDataFine)
	
	CREATE TABLE #Agende
		(CodAgenda VARCHAR(20) COLLATE Latin1_General_CI_AS)

	INSERT INTO #Agende(CodAgenda)
	SELECT	ValoreParametro.CodAgenda.value('.','VARCHAR(20)')						
			FROM @xParametri.nodes('/Parametri/CodAgenda') as ValoreParametro(CodAgenda)
			
		CREATE TABLE #Giorni
		(id int IDENTITY(1,1), 
		 [Data] datetime, 
		)
	
	DECLARE @n int
	SET @n = 0
	
	WHILE @n <= @nGiorni
	BEGIN
	
		INSERT INTO #Giorni ([data])
		VALUES
			(DATEADD(day,@n, @dDataInizio))

		SET @n = @n + 1
	END		
	
		CREATE TABLE #GiorniAgende
		(Data DATE,
		 CodAgenda VARCHAR(20) COLLATE Latin1_General_CI_AS,
		 Qta INTEGER)

	INSERT INTO #GiorniAgende(Data,CodAgenda,Qta)
	(SELECT G.Data, A.CodAgenda, 0 AS Qta
	 FROM #Giorni G
		CROSS APPLY #Agende A
	)

		CREATE TABLE #GiorniAgendePrenotazioni
		(Data DATE,
		 CodAgenda VARCHAR(20) COLLATE Latin1_General_CI_AS,
		 Qta INTEGER)

	INSERT INTO #GiorniAgendePrenotazioni(Data,CodAgenda,Qta)
	SELECT 
				CONVERT(DATE,M.DataInizio) AS Giorno,
				MA.CodAgenda,
				COUNT(*) AS QTA
			FROM 
				T_MovAppuntamenti M 
					INNER JOIN T_MovAppuntamentiAgende MA
						ON M.ID=MA.IDAppuntamento
					INNER JOIN #Agende A
						ON MA.CodAgenda=A.CodAgenda 
			WHERE 
				M.CodStatoAppuntamento NOT IN ('CA','AN','TR') AND
				MA.CodStatoAppuntamentoAgenda NOT IN ('CA') AND
		
				M.DataInizio between @dDataInizio AND @dDataFine
			GROUP BY 
				CONVERT(DATE,M.DataInizio),
				MA.CodAgenda

	UPDATE #GiorniAgende
	SET Qta=GAP.Qta
	FROM
		#GiorniAgende GA
			INNER JOIN #GiorniAgendePrenotazioni GAP
				ON GA.Data=GAP.Data AND
				   GA.CodAgenda =GAP.CodAgenda

		

	SELECT * FROM #GiorniAgende
	WHERE Qta>0

		drop table #Giorni
	drop table #Agende
	drop table #GiorniAgende
	drop table #GiorniAgendePrenotazioni
		
	RETURN 0
	
END