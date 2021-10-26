

CREATE VIEW [dbo].[Q_SelEpisodioPU]
AS
	SELECT Q.IDEpisodio,
		   TRASF1.ID AS IDTrasferimentoPrimo,
		   TRASF1.CodUA AS CodUAPrimo,
		   TRASF1.DataIngresso AS DataIngressoPrimo,
		   	   	
		   TRASF2.ID AS IDTrasferimentoUltimo,
		   TRASF2.CodUA AS CodUAUltimo,
		   TRASF2.DataUscita AS DataUscitaUltimo
		   
	FROM		
		(SELECT IDEpisodio, MIN(DataIngresso) AS DataPrimo,MAX(DataIngresso) As DataUltimo
		FROM T_MovTrasferimenti	
		WHERE CodStatoTrasferimento NOT IN ('CA','SS')
		GROUP BY IDEpisodio) AS Q
				LEFT JOIN 
					(SELECT * FROM T_MovTrasferimenti 
					 WHERE CodStatoTrasferimento NOT IN ('CA','SS') 
					 ) AS TRASF1 
					ON	(TRASF1.IDEpisodio = Q.IDEpisodio AND
						 TRASF1.DataIngresso =Q.DataPrimo)
				LEFT JOIN	 				 
				(SELECT *  FROM T_MovTrasferimenti 
					 WHERE CodStatoTrasferimento NOT IN ('CA','SS') 
					 ) AS TRASF2
					ON	(TRASF2.IDEpisodio = Q.IDEpisodio AND
						 TRASF2.DataIngresso =Q.DataUltimo)