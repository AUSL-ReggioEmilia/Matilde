


CREATE PROCEDURE [dbo].[MSP_SelExportDWHScheda](@xParametri XML)
AS
BEGIN
	
		
		DECLARE @uIDScheda AS UNIQUEIDENTIFIER	
	DECLARE @sGUID AS VARCHAR(50)
	
		
	    DECLARE @dDataAggiornamento AS DATETIME	
	SET @dDataAggiornamento='2017-11-07 05:00'			
					
		IF @xParametri.exist('/Parametri/IDScheda')=1
		BEGIN			
			SET @sGUID=(SELECT TOP 1 ValoreParametro.IDScheda.value('.','VARCHAR(50)')
							  FROM @xParametri.nodes('/Parametri/IDScheda') as ValoreParametro(IDScheda))
			IF 	ISNULL(@sGUID,'') <> '' 				
					SET @uIDScheda=CONVERT(UNIQUEIDENTIFIER,	@sGUID)	
					END	
	

						
	SELECT 
		M.ID AS IDScheda,
				CASE	
			WHEN CodStatoScheda='CA' THEN 2
			ELSE 1
		END	AS Azione,		
				
				CASE 
			 WHEN M.DataCreazione <= @dDataAggiornamento THEN
				CASE 
					WHEN LEN(REPLACE(M.CodEntita + CONVERT(VARCHAR(50),IDEntita)+ M.CodScheda + CONVERT(VARCHAR(10),M.Numero),'-','')) >  48 THEN
						REPLACE(CONVERT(VARCHAR(50),M.ID),'-','')
					ELSE 
						REPLACE(M.CodEntita + CONVERT(VARCHAR(50),IDEntita)+ M.CodScheda + CONVERT(VARCHAR(10),M.Numero),'-','') 
				END
			ELSE
				CONVERT(VARCHAR(50),M.ID) 		END
		AS IDEsterno,
		
        ISNULL(M.DataUltimaModifica,M.DataCreazione) AS DataSequenza,
		CASE 
			WHEN M.CodEntita='PAZ' THEN 'ASMN'
			ELSE ISNULL(E.CodAzi,'ASMN')
		END AS AziendaErogante,
		CASE 
			WHEN ISNULL(S.SistemaDWH,'') <> '' THEN S.SistemaDWH					ELSE
				CASE 
					WHEN M.CodEntita='PAZ' THEN 'SCCI-AMB'
					ELSE 'SCCI'
				END
		END AS SistemaErogante,
		
		ISNULL(A.Codice,ISNULL(ASCH.Codice,'')) AS RepartoEroganteCodice,
		ISNULL(A.Descrizione,ISNULL(ASCH.Descrizione,'')) AS RepartoEroganteDescrizione,
		P.CodSAC AS PazienteIDSAC,						P.Cognome AS PazienteCognome,
		P.Nome AS PazienteNome,
		P.DataNascita As PazienteDataNascita,
		P.Sesso AS PazienteSesso,
		P.CodiceFiscale As PazienteCodiceFiscale,

		
		CASE 
			WHEN M.CodStatoScheda='CA' THEN 2
						WHEN DataCreazione < ISNULL(DataUltimaModifica,GETDATE()) THEN 1
			ELSE 1
		END	AS StatoRichiesta,			        ISNULL(M.DataUltimaModifica,M.DataCreazione) AS DataReferto,
        CONVERT(VARCHAR(50),M.IDNum)  NumeroReferto,
        ISNULL(E.NumeroNosologico,'') AS Nosologico,
        CASE	
						WHEN LM.Codice IS NULL THEN 
					CASE 
						WHEN ISNULL(L.CodiceFiscale,'') =''  THEN L.Codice
						ELSE L.CodiceFiscale
					END
			ELSE 
										CASE 
						WHEN ISNULL(LM.CodiceFiscale,'') =''  THEN LM.Codice
						ELSE LM.CodiceFiscale
					END
		END AS MedicoRefertanteCodice,
		
		CASE	
			WHEN LM.Codice IS NULL THEN L.Descrizione
			ELSE LM.Descrizione
		END AS MedicoRefertanteDescrizione,
		S.CodPrestazioneDWH,
		S.DescrizionePrestazioneDWH,
				Dati.query('/DcSchedaDati/Dati/Item/Value/DcDato[(ID[1] = "AnteprimaDWH")]').value('(/DcDato/Value)[1]', 'varchar(MAX)') AS AnteprimaDWH,
		ISNULL(S.EsportaLayerDWH,0) AS EsportaLayerDWH
                
	FROM 
		T_MovSchede M
			LEFT JOIN T_MovEpisodi E	
				ON m.IDEpisodio=E.ID
			LEFT JOIN T_MovTrasferimenti T
				ON M.IDTrasferimento=T.ID
			LEFT JOIN T_UnitaAtomiche A
					ON T.CodUA=A.Codice	
			LEFT JOIN T_UnitaAtomiche ASCH
					ON M.CodUA=ASCH.Codice	
			LEFT JOIN T_Pazienti P
				ON M.IDPaziente=P.ID
			LEFT JOIN T_Login L
				ON M.CodUtenteRilevazione=L.Codice
			LEFT JOIN T_Login LM
				ON M.CodUtenteUltimaModifica=LM.Codice
			LEFT JOIN T_Schede S
				ON M.CodScheda=S.Codice
	WHERE M.ID=@uIDScheda	
	
	

                											 
				
END