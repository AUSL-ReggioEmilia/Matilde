




CREATE VIEW [dbo].[Q_SelPrescrizioniTask]
AS							

SELECT
	   P.ID AS IDPrescrizione,
	   	   ISNULL(NumTaskTotali,0) AS NumTaskTotali,
	   ISNULL(NumTaskProgrammati,0) AS NumTaskProgrammati,
	   ISNULL(NumTaskErogati,0) AS NumTaskErogati,
	   ISNULL(NumTaskAnnullati,0) AS NumTaskAnnullati,
	   ISNULL(NumTaskCancellati,0) AS NumTaskCancellati,
	   DataUltimaErogazione,
	   DataUltimaProgrammazione,
	   
	   CASE		
		   WHEN ISNULL(P.CodStatoContinuazione,'AP')='CH' THEN
								CASE
						WHEN NumPrescrizioniAlBisogno >0 AND NumPrescrizioniAlBisogno=NumPrescrizioniTotali THEN 'AC'			
						WHEN ISNULL(CodStatoPrescrizioneCalcTask,'DD')= 'DD' THEN 'ER'								WHEN ISNULL(CodStatoPrescrizioneCalcTask,'DD')= 'DI' THEN 'SC'								WHEN ISNULL(CodStatoPrescrizioneCalcTask,'DD')= 'IC' THEN 'SC'								WHEN ISNULL(CodStatoPrescrizioneCalcTask,'DD')= 'DP' THEN 'ER'								ELSE 'NA'
				END		
					
		   ELSE	
								   CASE 
						WHEN NumPrescrizioniAlBisogno >0 
							AND NumPrescrizioniAlBisogno=NumPrescrizioniTotali THEN 'AB'			
						ELSE
							ISNULL(CodStatoPrescrizioneCalcTask,'DD')				 
				   END 
			END	   
	   AS  CodStatoPrescrizioneCalc,
					
	   
	   	   ISNULL(TDPNumTaskTotali,0) AS TDPNumTaskTotali,
	   ISNULL(TDPNumTaskProgrammati,0) AS TDPNumTaskProgrammati,
	   ISNULL(TDPNumTaskErogati,0) AS TDPNumTaskErogati,
	   ISNULL(TDPNumTaskAnnullati,0) AS TDPNumTaskAnnullati,
	   ISNULL(TDPNumTaskCancellati,0) AS TDPNumTaskCancellati,
	   TDPDataUltimaErogazione,
	   TDPDataUltimaProgrammazione,
	   TDPDataProssimaProgrammazione,
	   
	   		NumPrescrizioniAlBisogno,
		NumPrescrizioniTotali
FROM 	   				
	T_MovPrescrizioni P
		LEFT JOIN 
			(SELECT 
				IDPrescrizione,
				SUM(QtaAlBisogno) AS NumPrescrizioniAlBisogno,
				SUM(QtaTotali) AS NumPrescrizioniTotali
			 FROM	
					(SELECT 
						T.IDPrescrizione,
						CASE
							WHEN T.AlBisogno=1 THEN 1
							ELSE 0
						END AS QtaAlBisogno,	
						1 QtaTotali					
					 FROM
						T_MovPrescrizioniTempi T
					 WHERE 	
						T.CodStatoPrescrizioneTempi='VA' 
					 ) AS MPT
			 GROUP BY 
				IDPrescrizione
			 )	 AS MPTG	
		ON P.ID=MPTG.IDPrescrizione	 

		LEFT JOIN 	
			(
				SELECT
					IDPrescrizione AS IDPrescrizione,
					COUNT(*) AS NumTaskTotali,
					SUM(PR) AS NumTaskProgrammati,
					SUM(ER) AS NumTaskErogati,
					SUM(AN) AS NumTaskAnnullati,
					SUM(CA) AS NumTaskCancellati,
					MAX(TmpDataErogazione) As DataUltimaErogazione,
					MAX(TmpDataProgrammata) As DataUltimaProgrammazione,
					CASE
						WHEN SUM(ER)=0 AND SUM(PR) = 0 AND SUM(AN)=0	THEN 'DD'								WHEN SUM(ER)=0 AND SUM(PR) > 0	THEN 'DI'												WHEN SUM(ER)>0 AND SUM(PR) > 0  THEN 'IC'												WHEN SUM(PR) = 0 AND (SUM(ER)>0 OR SUM(AN) > 0) THEN 'DP'								ELSE 'NA'
					END
					AS CodStatoPrescrizioneCalcTask,
										SUM(TDP_QTA) AS TDPNumTaskTotali,
					SUM(TDP_PR) AS TDPNumTaskProgrammati,
					SUM(TDP_ER) AS TDPNumTaskErogati,
					SUM(TDP_AN) AS TDPNumTaskAnnullati,
					SUM(TDP_CA) AS TDPNumTaskCancellati,
					MAX(TDP_TmpDataErogazione) As TDPDataUltimaErogazione,
					MAX(TDP_TmpDataProgrammata) As TDPDataUltimaProgrammazione,
					MIN(TDP_TmpDataProgrammata) As TDPDataProssimaProgrammazione
				FROM 	
						(SELECT 
							 T.IDSistema AS IDPrescrizione,
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
							ISNULL(IDSistema,'') <> ''
							AND T.CodStatoTaskInfermieristico <> 'CA') AS Q
				WHERE 
					 ISNULL(IDPrescrizione,'') <> ''	
					 	
				GROUP BY
					 IDPrescrizione
		) AS M			
		
	ON P.IDString=M.IDPrescrizione
WHERE P.CodStatoPrescrizione <> 'CA'