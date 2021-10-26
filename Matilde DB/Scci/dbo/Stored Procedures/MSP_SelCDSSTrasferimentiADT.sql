CREATE PROCEDURE [dbo].[MSP_SelCDSSTrasferimentiADT](@xParametri AS XML)

AS
BEGIN

	

				
	DECLARE @uIDEpisodio AS UNIQUEIDENTIFIER
	DECLARE @sIDEpisodio AS VARCHAR(50)

	DECLARE @uID AS UNIQUEIDENTIFIER
		DECLARE @sCodUO AS VARCHAR(20)
	DECLARE @DataIngresso AS DATETIME
	DECLARE @DataUscita AS DATETIME

	DECLARE @uIDPrec AS UNIQUEIDENTIFIER
		DECLARE @sCodUOPrec AS VARCHAR(20)
	DECLARE @DataIngressoPrec AS DATETIME
	DECLARE @DataUscitaPrec AS DATETIME


		SET @sIDEpisodio=(SELECT TOP 1 ValoreParametro.IDEpisodio.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDEpisodio') as ValoreParametro(IDEpisodio))					  
	IF 	ISNULL(@sIDEpisodio,'') <> '' AND ISNULL(@sIDEpisodio,'') <> 'NULL' 
		BEGIN			
			SET @uIDEpisodio=CONVERT(UNIQUEIDENTIFIER,	@sIDEpisodio)	
		END	

			
	CREATE TABLE #tmpTra
	(
		ID UNIQUEIDENTIFIER,
				CodUO VARCHAR(20),
		Reparto VARCHAR(255),
		DataIngresso DATETIME,
		DataUscita DATETIME
	)

	CREATE TABLE #tmpTraCur
	(
		ID UNIQUEIDENTIFIER,
				CodUO VARCHAR(20),
		Reparto VARCHAR(255),
		DataIngresso DATETIME,
		DataUscita DATETIME
	)

			
		INSERT INTO #tmpTra
	SELECT 
		T.ID,
				CodUO,
				DescrUO AS Reparto,
		DataIngresso,
		DataUscita
	FROM								
		T_MovTrasferimenti T
							WHERE 
		T.IDEpisodio=@uIDEpisodio AND
		T.CodStatoTrasferimento IN ('AT','TR','DM')
	ORDER BY
		T.DataIngresso ASC	

	INSERT INTO #tmpTraCur 
	SELECT * FROM #tmpTra 
	ORDER BY DataIngresso

			

	DECLARE cur CURSOR FOR   
		SELECT 
			ID,
						CodUO,
			DataIngresso,
			DataUscita 
		FROM #tmpTraCur
		ORDER BY DataIngresso;  

	OPEN cur  

		
	FETCH NEXT FROM cur   
	INTO @uID, @sCodUO ,@DataIngresso,@DataUscita


	WHILE @@FETCH_STATUS = 0  
	BEGIN  

		IF @uIDPrec IS NOT NULL
		BEGIN
									IF @sCodUO = @sCodUOPrec
			BEGIN
				
				
								UPDATE #tmpTra
				SET DataUscita = ISNULL(@DataUscita,@DataIngresso)
				WHERE ID=@uIDPrec

				
				
								DELETE FROM  #tmpTra
				WHERE ID=@uID

							END
			ELSE
			BEGIN	
			
				
								SET @uIDPrec = @uID
								SET @sCodUOPrec = @sCodUO
				SET @DataIngressoPrec = @DataIngresso
				SET @DataUscitaPrec = @DataUscita

							END 
		END
		ELSE
		BEGIN
						SET @uIDPrec = @uID
						SET @sCodUOPrec = @sCodUO
			SET @DataIngressoPrec = @DataIngresso
			SET @DataUscitaPrec = @DataUscita
					END

						FETCH NEXT FROM cur   
		INTO @uID, @sCodUO ,@DataIngresso,@DataUscita
	END
		
	CLOSE cur
	DEALLOCATE cur

											
	SELECT 
				T.Reparto,
		CASE 
			WHEN T.DataIngresso IS NOT NULL THEN  CONVERT(varchar(10),DataIngresso,105) + ' ' +CONVERT(varchar(5),DataIngresso,14)
			ELSE NULL
		END AS [Data Ora Ingresso],
		CASE
			WHEN T.DataUscita IS NOT NULL THEN  CONVERT(varchar(10),DataUscita,105) + ' ' +CONVERT(varchar(5),DataUscita,14)
			ELSE Null
		END AS [Data Ora Uscita]
	FROM								
		#tmpTra T			
	ORDER BY
		T.DataIngresso

		DROP TABLE #tmpTra
	DROP TABLE #tmpTraCur

END