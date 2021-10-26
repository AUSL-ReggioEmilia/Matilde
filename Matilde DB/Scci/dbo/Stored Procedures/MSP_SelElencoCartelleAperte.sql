CREATE PROCEDURE MSP_SelElencoCartelleAperte 
	@dateFrom DATETIME
AS
BEGIN
	SET NOCOUNT ON;
	
	SELECT QTRA.IDCartella,QTRA.NumeroCartella
		FROM 
			(
								SELECT IDEpisodio 
				FROM T_MovAppuntamenti WITH (NOLOCK)  
				WHERE 
					ID IN (	SELECT IDEntita	
							FROM T_MovTimeStamp WITH (NOLOCK)
							WHERE
								CodEntita='APP' AND
								CodAzione <> 'VIS' AND 
								CodLogin  not like '%web%' 
								and DataOra >=@dateFrom
							 )	
					AND IDEpisodio IS NOT NULL		 
				GROUP BY IDEpisodio			 
			UNION 
								SELECT IDEpisodio 
				FROM T_MovDiarioClinico WITH (NOLOCK) 
				WHERE 
					ID IN (	SELECT IDEntita	
							FROM T_MovTimeStamp WITH (NOLOCK)
							WHERE
								CodEntita='DCL' AND
								CodAzione <> 'VIS' AND 
								CodLogin  not like '%web%' 
								and DataOra  >=@dateFrom
							 )	
					AND IDEpisodio IS NOT NULL		 
				GROUP BY IDEpisodio	
			UNION
								SELECT IDEpisodio 
				FROM T_MovEvidenzaClinica WITH (NOLOCK) 
				WHERE 
					ID IN (	SELECT IDEntita	
							FROM T_MovTimeStamp WITH (NOLOCK)
							WHERE
								CodEntita='EVC' AND
								CodAzione <> 'VIS' AND 
								CodLogin  not like '%web%' 
								and DataOra >=@dateFrom
							 )	
					AND IDEpisodio IS NOT NULL		 
				GROUP BY IDEpisodio		
			UNION	
								SELECT IDEpisodio 
				FROM T_MovPazienti WITH (NOLOCK) 
				WHERE 
					ID IN (	SELECT IDEntita	
							FROM T_MovTimeStamp WITH (NOLOCK)
							WHERE
								CodEntita='PAZ' AND
								CodAzione <> 'VIS' AND 
								CodLogin  not like '%web%' 
								and DataOra >=@dateFrom
							 )	
					AND IDEpisodio IS NOT NULL		 
				GROUP BY IDEpisodio		
			UNION ALL	
								SELECT IDEpisodio 
				FROM T_MovPrescrizioni WITH (NOLOCK) 
				WHERE 
					ID IN (	SELECT IDEntita	
							FROM T_MovTimeStamp WITH (NOLOCK)
							WHERE
								CodEntita='PRF' AND
								CodAzione <> 'VIS' AND 
								CodLogin  not like '%web%' 
								and DataOra >=@dateFrom
							 )	
					AND IDEpisodio IS NOT NULL		 
				GROUP BY IDEpisodio	
			UNION 	
								SELECT IDEpisodio 
				FROM T_MovSchede WITH (NOLOCK) 
				WHERE 
					ID IN (	SELECT IDEntita	
							FROM T_MovTimeStamp WITH (NOLOCK)
							WHERE
								CodEntita='SCH' AND
								CodAzione <> 'VIS' AND 
								CodLogin  not like '%web%' 
								and DataOra >=@dateFrom
							 )	
					AND IDEpisodio IS NOT NULL		 
				GROUP BY IDEpisodio				
			UNION 
								SELECT IDEpisodio 
				FROM T_MovTrasferimenti WITH (NOLOCK) 
				WHERE 
					ID IN (	SELECT IDEntita	
							FROM T_MovTimeStamp WITH (NOLOCK)
							WHERE
								CodEntita='TRA' AND
								CodAzione <> 'VIS' AND 
								CodLogin  not like '%web%' 
								and DataOra >=@dateFrom
							 )	
					AND IDEpisodio IS NOT NULL		 
				GROUP BY IDEpisodio	
			UNION	
								SELECT IDEpisodio 
				FROM T_MovTrasferimenti WITH (NOLOCK) 
				WHERE 
					IDCartella IN (	SELECT IDEntita	
							FROM T_MovTimeStamp WITH (NOLOCK)
							WHERE
								CodEntita='CAR' AND
								CodAzione = 'INS' AND 
								CodLogin  not like '%web%' 
								and DataOra >=@dateFrom
							 )	
					AND IDEpisodio IS NOT NULL		 
				GROUP BY IDEpisodio			
			UNION
								SELECT IDEpisodio 
				FROM T_MovTaskInfermieristici WITH (NOLOCK) 
				WHERE 
					ID IN (	SELECT IDEntita	
							FROM T_MovTimeStamp WITH (NOLOCK)
							WHERE
								CodEntita='WKI' AND
								CodAzione <> 'VIS' AND 
								CodLogin  not like '%web%' 
								and DataOra >=@dateFrom
							 )	
					AND IDEpisodio IS NOT NULL		 
				GROUP BY IDEpisodio		
			UNION		
							SELECT IDEpisodio 
				FROM T_MovParametriVitali WITH (NOLOCK) 
				WHERE 
					ID IN (	SELECT IDEntita	
							FROM T_MovTimeStamp WITH (NOLOCK)
							WHERE
								CodEntita='PVT' AND
								CodAzione <> 'VIS' AND 
								CodLogin  not like '%web%' 
								and DataOra >=@dateFrom
							 )	
					AND IDEpisodio IS NOT NULL		 
				GROUP BY IDEpisodio			
			UNION		
							SELECT IDEpisodio 
				FROM T_MovAlertGenerici WITH (NOLOCK) 
				WHERE 
					ID IN (	SELECT IDEntita	
							FROM T_MovTimeStamp WITH (NOLOCK)
							WHERE
								CodEntita='ALG' AND
								CodAzione <> 'VIS' AND 
								CodLogin  not like '%web%' 
								and DataOra >=@dateFrom
							 )	
					AND IDEpisodio IS NOT NULL		 
				GROUP BY IDEpisodio
			) AS SEPI
			INNER JOIN 
				T_MovEpisodi MEPI WITH (NOLOCK) 
					ON SEPI.IDEpisodio=	MEPI.ID		
				
			INNER JOIN
				(SELECT 
					MTRA.IDEpisodio,MTRA.IDCartella,QCAR.NumeroCartella
				 FROM	
					T_MovTrasferimenti MTRA WITH (NOLOCK) 
					INNER JOIN		  
						(SELECT ID, NumeroCartella 
						  FROM 
							T_MovCartelle WITH (NOLOCK) 
						 WHERE
							CodStatoCartella='AP'									) AS QCAR
					ON MTRA.IDCartella=QCAR.ID	
					
				  GROUP BY IDEpisodio,IDCartella, NumeroCartella
				) AS  QTRA		
			ON MEPI.ID=QTRA.IDEpisodio	
		GROUP BY QTRA.IDCartella, QTRA.NumeroCartella	


END