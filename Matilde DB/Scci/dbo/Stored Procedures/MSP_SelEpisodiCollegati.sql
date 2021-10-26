CREATE PROCEDURE [dbo].[MSP_SelEpisodiCollegati](@xParametri AS XML)
AS
BEGIN
			
	
				DECLARE @uIDEpisodio AS UNIQUEIDENTIFIER
	
		DECLARE @uIDCartella AS UNIQUEIDENTIFIER
	DECLARE @xPar AS XML
	
	
		SET @uIDEpisodio=(SELECT TOP 1 ValoreParametro.IDEpisodio.value('.','UNIQUEIDENTIFIER')
					  FROM @xParametri.nodes('/Parametri/IDEpisodio') as ValoreParametro(IDEpisodio));

	CREATE TABLE #tmpCartelle		
	(
		IDCartella UNIQUEIDENTIFIER
	)
	
		DECLARE curCartelle CURSOR
    FOR 	
		SELECT IDCartella
		FROM T_MovTrasferimenti
		WHERE 
			IDEpisodio=@uIDEpisodio AND
			IDCartella IS NOT NULL

	OPEN curCartelle
	FETCH NEXT FROM curCartelle INTO @uIDCartella
	
	WHILE @@FETCH_STATUS = 0
	BEGIN
													
		;							WITH GerarchiaCA(IDEntita)
				AS
				( SELECT 							
						 IDEntitaCollegata AS IDEntita
				  FROM T_MovRelazioniEntita CAP	  
				  WHERE (IDEntita =@uIDCartella 
						 )	
						 AND CodEntita='CAR'		
						 AND CodEntitaCollegata='CAR'
						  				
				  UNION ALL
				  SELECT 			
						 						 CAF.IDEntitaCollegata AS IDEntita			
				  FROM T_MovRelazioniEntita CAF
					INNER JOIN GerarchiaCA G
						ON (CAF.IDEntita=G.IDEntita	
							AND CAF.CodEntita='CAR'		
							AND CAF.CodEntitaCollegata='CAR')
					
					
				)	
				INSERT INTO #tmpCartelle(IDCartella)		
				SELECT
					DISTINCT  
					IDEntita 
				FROM  GerarchiaCA
				 

				;									WITH GerarchiaCA(IDEntita)
					AS
					( SELECT 			
							 IDEntita					 
					  FROM T_MovRelazioniEntita CAP	  
					  WHERE (IDEntitaCollegata =@uIDCartella 
							 )					 	
							 AND CodEntita='CAR'		
							 AND CodEntitaCollegata='CAR'
							  				
					  UNION ALL
					  SELECT 			
							 CAF.IDEntita								
					  FROM T_MovRelazioniEntita CAF
						INNER JOIN GerarchiaCA G
							ON (CAF.IDEntitaCollegata=G.IDEntita		
								AND CAF.CodEntita='CAR'		
								AND CAF.CodEntitaCollegata='CAR')
						
					)		
				INSERT INTO #tmpCartelle(IDCartella)		
				SELECT
					DISTINCT  
					IDEntita 
				FROM  GerarchiaCA
				WHERE IDEntita NOT IN (SELECT IDCartella FROM #tmpCartelle)
				
				INSERT INTO #tmpCartelle(IDCartella)		
				SELECT @uIDCartella
				WHERE
					NOT EXISTS 
						(SELECT IDCartella
						 FROM #tmpCartelle 
						 WHERE IDCartella=@uIDCartella)
									 
		FETCH NEXT FROM curCartelle INTO @uIDCartella
	END
	
	
	
	SELECT IDEpisodio 
	FROM #tmpCartelle C
		INNER JOIN T_MovTrasferimenti M
			ON C.IDCartella=M.IDCartella
	GROUP BY IDEpisodio	
	UNION
		SELECT @uIDEpisodio	
	
	CLOSE curCartelle
	DEALLOCATE curCartelle
	
	DROP TABLE #tmpCartelle

END