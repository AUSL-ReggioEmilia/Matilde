CREATE VIEW [dbo].[Q_SelEpisodioRicoveroPU]
AS
	SELECT 
		   Y.IDEpisodio,
		   Y.IDTrasferimentoPrimo,
		   TP.CodUA AS CodUAPrimo,
		   TP.DataIngresso AS DataIngressoPrimo
		   ,		   	   	
		   Y.IDTrasferimentoUltimo,
		   TU.CodUA AS CodUAUltimo,
		   TU.DataUscita AS DataUscitaUltimo

	FROM
	(
		SELECT Q.IDEpisodio,
			   MAX(TRASF1.ID) AS IDTrasferimentoPrimo,
			   MAX(TRASF2.ID) AS IDTrasferimentoUltimo
		FROM				   		
			(SELECT IDEpisodio, MIN(DataIngresso) AS DataPrimo,MAX(DataIngresso) As DataUltimo
			FROM T_MovTrasferimenti	
			WHERE CodStatoTrasferimento NOT IN ('CA','SS','PC','PR') 
			GROUP BY IDEpisodio) AS Q
					LEFT JOIN 
						(SELECT IDEpisodio,ID,CodUA,DataIngresso,DataUscita FROM T_MovTrasferimenti 
						 WHERE CodStatoTrasferimento NOT IN ('CA','SS','PC','PR') 					 
						 ) AS TRASF1 
						ON	(TRASF1.IDEpisodio = Q.IDEpisodio AND
							 TRASF1.DataIngresso =Q.DataPrimo)
					LEFT JOIN	 				 
					(SELECT IDEpisodio,ID,CodUA,DataIngresso,DataUscita  FROM T_MovTrasferimenti 
						 WHERE CodStatoTrasferimento NOT IN ('CA','SS','PC','PR') 
						 ) AS TRASF2
						ON	(TRASF2.IDEpisodio = Q.IDEpisodio AND
							 TRASF2.DataIngresso =Q.DataUltimo)				
		GROUP BY Q.IDEpisodio
	) AS Y
		LEFT JOIN T_MovTrasferimenti TP
			ON Y.IDTrasferimentoPrimo = TP.ID
		LEFT JOIN T_MovTrasferimenti TU
			ON Y.IDTrasferimentoUltimo = TU.ID