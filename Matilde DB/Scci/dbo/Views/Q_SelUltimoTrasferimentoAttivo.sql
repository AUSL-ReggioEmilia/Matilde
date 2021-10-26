CREATE VIEW [dbo].[Q_SelUltimoTrasferimentoAttivo]
AS


	SELECT ID 		
	FROM
		T_MovTrasferimenti T
			INNER JOIN
				(SELECT IDEpisodio, MAX(IDNum) AS IDNumTrasferimento
				FROM 
					T_MovTrasferimenti
				WHERE CodStatoTrasferimento = 'AT'
				GROUP BY IDEpisodio)  Q
			ON T.IDnum = Q.IDNumTrasferimento