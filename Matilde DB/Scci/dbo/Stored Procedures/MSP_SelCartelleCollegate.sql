CREATE PROCEDURE [dbo].[MSP_SelCartelleCollegate](@xParametri AS XML)
AS
BEGIN
			
	
				DECLARE @uIDCartella AS UNIQUEIDENTIFIER
	
		SET @uIDCartella=(SELECT TOP 1 ValoreParametro.IDCartella.value('.','UNIQUEIDENTIFIER')
					  FROM @xParametri.nodes('/Parametri/IDCartella') as ValoreParametro(IDCartella));

	CREATE TABLE #tmpCartelle		
	(
		IDCartella UNIQUEIDENTIFIER
	)
	
	;			WITH GerarchiaCA(IDEntita)
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
		 

		;					WITH GerarchiaCA(IDEntita)
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
					 
	SELECT  IDCartella FROM #tmpCartelle
	GROUP BY IDCartella
	
	DROP TABLE #tmpCartelle

END