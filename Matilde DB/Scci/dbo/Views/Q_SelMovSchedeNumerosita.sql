



CREATE VIEW [dbo].[Q_SelMovSchedeNumerosita]
AS
	SELECT M.CodEntita,
		   M.IDEntita,
		   M.CodScheda,
		   ISNULL(M.QtaSchedeAttive,0) AS QtaSchedeAttive ,
		   ISNULL(Q.QtaSchedeTotali,0) AS QtaSchedeTotali,		   
		   ISNULL(S.NumerositaMassima,0) AS QtaSchedeMassima,
		   ISNULL(S.NumerositaMassima,0) - ISNULL(M.QtaSchedeAttive,0) AS QtaSchedeDisponibili,
		   ISNULL(Q.MaxNumero,0) AS MassimoNumero
	FROM
						(SELECT
					CodEntita,
					IDEntita,
					CodScheda,
					COUNT(ID) AS QtaSchedeAttive
			FROM T_MovSchede WITH (NOLOCK)
					
			WHERE Storicizzata=0 									  AND CodStatoScheda <> 'CA'						  AND IDSchedaPadre IS NULL						GROUP BY
				CodEntita,
				IDEntita,
				CodScheda
				
			)	AS M
			
		LEFT JOIN
						(SELECT		CodEntita,
						IDEntita,
						CodScheda,
						COUNT(ID) AS QtaSchedeTotali,
						MAX(Numero) AS MaxNumero						
				FROM T_MovSchede WITH (NOLOCK) 			   
				WHERE	Storicizzata=0 											AND IDSchedaPadre IS NULL						GROUP BY
					CodEntita,
					IDEntita,
					CodScheda
			) AS Q 
		ON 
			(M.CodEntita=Q.CodEntita	AND
			 M.IDEntita=Q.IDEntita AND
			 M.CodScheda=Q.CodScheda)
			 
		LEFT JOIN T_Schede S WITH (NOLOCK)
			ON M.CodScheda=S.Codice