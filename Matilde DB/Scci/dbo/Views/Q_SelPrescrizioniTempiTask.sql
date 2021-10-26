
CREATE VIEW [dbo].[Q_SelPrescrizioniTempiTask]
AS					
	SELECT
		IDPrescrizione AS IDPrescrizione,
		IDPrescrizioneTempi  AS IDPrescrizioneTempi,		
		COUNT(*) AS NumTaskTotali,
		SUM(PR) AS NumTaskProgrammati,
		SUM(ER) AS NumTaskErogati,
		SUM(AN) AS NumTaskAnnullati,
		SUM(CA) AS NumTaskCancellati,
		MAX(TmpDataErogazione) As DataUltimaErogazione,
		MAX(TmpDataProgrammata) As DataUltimaProgrammazione,
				SUM(TDP_QTA) AS TDPNumTaskTotali,
		SUM(TDP_PR) AS TDPNumTaskProgrammati,
		SUM(TDP_ER) AS TDPNumTaskErogati,
		SUM(TDP_AN) AS TDPNumTaskAnnullati,
		SUM(TDP_CA) AS TDPNumTaskCancellati,
		MAX(TDP_TmpDataErogazione) As TDPDataUltimaErogazione,
		MAX(TDP_TmpDataProgrammata) As TDPDataUltimaProgrammazione,
		MIN(TDP_TmpDataProgrammata) AS TDPDataProssimaProgrammazione
	FROM 	
		(SELECT 
			 T.IDSistema AS IDPrescrizione,
			 T.IDGruppo AS IDPrescrizioneTempi,
			 			 			 			 CASE 
				WHEN CodStatoTaskInfermieristico='PR' THEN 1 
				ELSE 0
			 END PR,
			 CASE 
				WHEN CodStatoTaskInfermieristico='ER' THEN 1 
				ELSE 0
			 END ER,
			 CASE 
				WHEN CodStatoTaskInfermieristico IN ('AN','TR') THEN 1 
				ELSE 0
			 END AN,
			 CASE 
				WHEN CodStatoTaskInfermieristico='CA' THEN 1 
				ELSE 0
			 END CA,
			 CASE 
				WHEN CodStatoTaskInfermieristico='ER' THEN DataErogazione 
				ELSE NULL
			 END TmpDataErogazione,
			  CASE 
				WHEN CodStatoTaskInfermieristico='PR' THEN DataProgrammata
				ELSE NULL
			 END TmpDataProgrammata,
			 
			 			 			 			  			 CASE 
				WHEN  CodTipoTaskInfermieristico=ISNULL(CTDP.Valore,'@!') THEN 1 
				ELSE 0
			 END TDP_QTA,
			 
			  			 CASE 
				WHEN CodStatoTaskInfermieristico='PR' AND 
					 CodTipoTaskInfermieristico=ISNULL(CTDP.Valore,'@!')
					 THEN 1 
				ELSE 0
			 END TDP_PR,
			 
			 			 CASE 
				WHEN CodStatoTaskInfermieristico='ER' AND 
					CodTipoTaskInfermieristico=ISNULL(CTDP.Valore,'@!') THEN 1 
				ELSE 0
			 END TDP_ER,
			 
			 			 CASE 
				WHEN CodStatoTaskInfermieristico IN ('AN','TR') AND
					CodTipoTaskInfermieristico=ISNULL(CTDP.Valore,'@!') THEN 1 
				ELSE 0
			 END TDP_AN,
			 
			 			 CASE 
				WHEN CodStatoTaskInfermieristico='CA' AND 
					CodTipoTaskInfermieristico=ISNULL(CTDP.Valore,'@!') THEN 1 
				ELSE 0
			 END TDP_CA,
			 
			 			 CASE 
				WHEN CodStatoTaskInfermieristico='ER' AND 
					CodTipoTaskInfermieristico=ISNULL(CTDP.Valore,'@!') THEN DataErogazione 
				ELSE NULL
			 END TDP_TmpDataErogazione,
			 
			 			 CASE 
				WHEN CodStatoTaskInfermieristico='PR' AND
					CodTipoTaskInfermieristico=ISNULL(CTDP.Valore,'@!') THEN DataProgrammata
				ELSE NULL
			 END TDP_TmpDataProgrammata
							 
			 
		FROM T_MovTaskInfermieristici T
			,(SELECT TOP 1 Valore FROM T_Config WHERE ID=33) AS CTDP
		WHERE		
			T.CodSistema='PRF' AND 
			ISNULL(T.IDSistema,'') <> '' AND
			ISNULL(T.IDGruppo,'') <> '') AS Q
	GROUP BY 
		IDPrescrizione,
		IDPrescrizioneTempi