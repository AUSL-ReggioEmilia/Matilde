CREATE VIEW [dbo].[Q_SelPrescrizioniTempiTaskInterna]
 
AS					
	SELECT 
			 T.IDGruppo,
			 COUNT_BIG(*) AS QtaPrescrizioniTempi,
			 			 			 			 SUM(CASE 
				WHEN CodStatoTaskInfermieristico='PR' THEN 1 
				ELSE 0
			 END) PR,
			 SUM(CASE 
				WHEN CodStatoTaskInfermieristico='ER' THEN 1 
				ELSE 0
			 END) ER,
			 SUM(CASE 
				WHEN CodStatoTaskInfermieristico IN ('AN','TR') THEN 1 
				ELSE 0
			 END) AN,
			 SUM(CASE 
				WHEN CodStatoTaskInfermieristico='CA' THEN 1 
				ELSE 0
			 END) CA
			 											 			 											 			 
			 			 			 			  			 ,SUM(CASE 
				WHEN  CodTipoTaskInfermieristico='TDP' THEN 1 
				ELSE 0
			 END) TDP_QTA,
			 
			  			 SUM(CASE 
				WHEN CodStatoTaskInfermieristico='PR' AND 
					 CodTipoTaskInfermieristico='TDP'
					 THEN 1 
				ELSE 0
			 END) TDP_PR,
			 
			 			 SUM(CASE 
				WHEN CodStatoTaskInfermieristico='ER' AND 
					CodTipoTaskInfermieristico='TDP' THEN 1 
				ELSE 0
			 END) TDP_ER,
			 
			 			 SUM(CASE 
				WHEN CodStatoTaskInfermieristico IN ('AN','TR') AND
					CodTipoTaskInfermieristico='TDP' THEN 1 
				ELSE 0
			 END) TDP_AN,
			 
			 			 SUM(CASE 
				WHEN CodStatoTaskInfermieristico='CA' AND 
					CodTipoTaskInfermieristico='TDP' THEN 1 
				ELSE 0
			 END) TDP_CA
			 
			 			 															 			 
			 			 															 							 
			 
		FROM dbo.T_MovTaskInfermieristici T			
		WHERE		
			T.CodSistema='PRF' AND 
			ISNULL(T.IDSistema,'') <> '' AND
			ISNULL(T.IDGruppo,'') <> ''
		GROUP BY
			 T.IDGruppo