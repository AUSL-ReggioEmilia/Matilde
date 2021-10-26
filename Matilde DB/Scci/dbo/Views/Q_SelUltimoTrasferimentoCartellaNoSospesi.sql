CREATE VIEW [dbo].[Q_SelUltimoTrasferimentoCartellaNoSospesi]
AS

	SELECT T.IDEpisodio, T.IDCartella,MAX(IDNum) AS IDNumTrasferimento
	FROM T_MovTrasferimenti T
		INNER JOIN 
			(SELECT IDEpisodio,IDCartella,MAX(DataIngresso) AS UltimDataIngresso
			FROM T_MovTrasferimenti	
			WHERE 
				CodStatoTrasferimento NOT IN ('CA','SS')
				AND IDCartella IS NOT NULL
			GROUP BY IDEpisodio,IDCartella) AS Q
		ON 
			T.IDEpisodio=Q.IDEpisodio AND
			T.DataIngresso=Q.UltimDataIngresso AND
			T.IDCartella=Q.IDCartella
	WHERE T.CodStatoTrasferimento NOT IN ('CA','SS')
	GROUP BY T.IDEpisodio,T.IDCartella