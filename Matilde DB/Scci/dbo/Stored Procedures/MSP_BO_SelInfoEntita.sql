CREATE PROCEDURE [dbo].[MSP_BO_SelInfoEntita] (@CodTipoEntita VarChar(20), @IDEntita UniqueIdentifier)
AS
BEGIN


DECLARE @sInfo AS VARCHAR(MAX)
DECLARE @sTmp AS VARCHAR(MAX)
DECLARE @sTmp2 AS VARCHAR(MAX)
DECLARE @bErrore AS BIT


IF ISNULL(@CodTipoEntita,'')='APP' 		
BEGIN
		SET @sTmp=(SELECT  TOP 1
				'Data Appuntamento: ' + ISNULL(CONVERT(Varchar(10),DataInizio,105),'') + ' ' + ISNULL(CONVERT(Varchar(5),DataInizio,14),'') + 
				'  Utente: ' + ISNULL(CodUtenteRilevazione,'') + CHAR(13)+CHAR(10) +
				'Agende: ' + ISNULL(ElencoRisorse,'') + CHAR(13)+CHAR(10) +
				'Tipo: ' + 
					CASE 
						WHEN IDEpisodio IS NULL THEN 'AMBULATORIALE'
						ELSE 'RICOVERATO'
					END +  CHAR(13)+CHAR(10) +		
				CASE 
					WHEN M.IDEpisodio IS NULL THEN ''
					ELSE 
						'Nosologico/LisaAttesa : ' + ISNULL(E.NumeroNosologico,'') + ' ' + 'ListaAttesa : ' +	ISNULL(E.NumeroListaAttesa,'')  + CHAR(13) + CHAR(10) +
						'Data Ricovero : ' + ISNULL(CONVERT(Varchar(10),E.DataRicovero,105),'') + ' ' + ISNULL(CONVERT(Varchar(5),E.DataRicovero,14),'') +  CHAR(13)+CHAR(10) 
					END + 
				'Paziente : ' + ISNULL(P.Nome,'') + ' ' + ISNULL(P.Cognome,'') + ' (' + CONVERT(VARCHAR(10), DataNascita,120) + ')' + CHAR(13) + CHAR(10) + 
				'IDPaziente: ' +	
					CASE 
						WHEN IDPaziente IS NOT NULL THEN CONVERT(varchar(50),IDPaziente)
						ELSE ''
					END						 
			   FROM 
					T_MovAppuntamenti M WITH (NOLOCK)	
						LEFT JOIN 
							T_Pazienti P
								ON M.IDPaziente=P.ID
						LEFT JOIN T_MovEpisodi E
								ON M.IDEpisodio = E.ID
			   WHERE M.ID=@IDEntita
			   )
	IF 	ISNULL(@sTmp,'')=''  
		BEGIN
			SET @bErrore=1
			SET @sTmp='ERRORE: ID non trovato'	   
		END	
	SET @sInfo=@sTmp		
END

IF ISNULL(@CodTipoEntita,'')='DCL' 		
BEGIN	
		SET @sTmp=(SELECT  TOP 1
				'Data Evento: ' + ISNULL(CONVERT(Varchar(10),DataEvento,105),'') + ' ' + ISNULL(CONVERT(Varchar(5),DataEvento,14),'') + CHAR(13)+CHAR(10) +
				'Utente: ' + ISNULL(CodUtenteRilevazione,'') + CHAR(13)+CHAR(10) +
				'Stato: ' + ISNULL(ST.Descrizione ,'') + CHAR(13)+CHAR(10)
			   FROM 
					T_MovDiarioClinico M WITH (NOLOCK)
						LEFT JOIN T_StatoDiario ST WITH (NOLOCK) 
							ON M.CodStatoDiario=ST.Codice
			   WHERE ID=@IDEntita
			   )
	IF 	ISNULL(@sTmp,'')=''  
		BEGIN
			SET @bErrore=1
			SET @sTmp='ERRORE: ID non trovato'	   
		END	
 
	SET @sInfo=@sTmp		
END

IF ISNULL(@CodTipoEntita,'')='PRF' 		
BEGIN
		SET @sTmp=(SELECT  TOP 1
				'Data Evento: ' + ISNULL(CONVERT(Varchar(10),DataEvento,105),'') + ' ' + ISNULL(CONVERT(Varchar(5),DataEvento,14),'') + CHAR(13)+CHAR(10) +
				'Utente: ' + ISNULL(CodUtenteRilevazione,'') + CHAR(13)+CHAR(10) 				
			   FROM 
					T_MovPrescrizioni WITH (NOLOCK)					
			   WHERE ID=@IDEntita
			   )
	IF 	ISNULL(@sTmp,'')=''  
	BEGIN
		SET @bErrore=1
		SET @sTmp='ERRORE: ID non trovato'	   
	END	
		   
	SET @sInfo=@sTmp		
END

IF ISNULL(@CodTipoEntita,'')='PVT' 		
BEGIN
		SET @sTmp=(SELECT  TOP 1
				'Data Evento: ' + ISNULL(CONVERT(Varchar(10),DataEvento,105),'') + ' ' + ISNULL(CONVERT(Varchar(5),DataEvento,14),'') + CHAR(13)+CHAR(10) +
				'Utente: ' + ISNULL(CodUtenteRilevazione,'') + CHAR(13)+CHAR(10) 				
			   FROM 
					T_MovParametriVitali WITH (NOLOCK)				
			   WHERE ID=@IDEntita
			   )
	IF 	ISNULL(@sTmp,'')=''  
	BEGIN
		SET @bErrore=1
		SET @sTmp='ERRORE: ID non trovato'	   
	END	
	SET @sInfo=@sTmp		
END

IF ISNULL(@CodTipoEntita,'')='WKI' 		
BEGIN
		SET @sTmp=(SELECT  TOP 1
				'Data Programmata : ' + ISNULL(CONVERT(Varchar(10),DataProgrammata,105),'') + ' ' + ISNULL(CONVERT(Varchar(5),DataProgrammata,14),'') + '    ' + 
				'Data Erogazione.: ' + ISNULL(CONVERT(Varchar(10),DataErogazione,105),'') + ' ' + ISNULL(CONVERT(Varchar(5),DataErogazione,14),'') + CHAR(13)+CHAR(10) +				
				'Utente: ' + ISNULL(CodUtenteRilevazione,'') + CHAR(13)+CHAR(10) +
				'Stato: ' + ISNULL(ST.Descrizione ,'') + CHAR(13)+CHAR(10) 
			   FROM 
					T_MovTaskInfermieristici M WITH (NOLOCK)
						LEFT JOIN T_StatoTaskInfermieristico ST WITH (NOLOCK)
							ON M.CodStatoTaskInfermieristico =ST.Codice
			   WHERE ID=@IDEntita
			   )
	IF 	ISNULL(@sTmp,'')=''  
	BEGIN
		SET @bErrore=1
		SET @sTmp='ERRORE: ID non trovato'	   
	END	
	SET @sInfo=@sTmp		
END

IF ISNULL(@CodTipoEntita,'')='SCH' 		
BEGIN
		SET @sTmp=(SELECT  TOP 1
				'Data Creazione : ' + ISNULL(CONVERT(Varchar(10),DataCreazione,105),'') + ' ' + ISNULL(CONVERT(Varchar(5),DataCreazione,14),'') + 
				', Data Modifica : ' + ISNULL(CONVERT(Varchar(10),DataUltimaModifica,105),'') + ' ' + ISNULL(CONVERT(Varchar(5),DataUltimaModifica,14),'') +  CHAR(13)+CHAR(10) +
				'Stato: ' + ISNULL(ST.Descrizione ,'') + 
				' Utente: ' + ISNULL(CodUtenteRilevazione,'') +  CHAR(13) + CHAR(10) +
				'Entita: ' + ISNULL(M.CodEntita,'') + ' Storicizza: ' + Convert(CHAR(1),M.Storicizzata) +  CHAR(13) + CHAR(10) +
				'Scheda: ' + '(' + ISNULL(M.CodScheda,'') + ') ' + ISNULL(S.Descrizione,'')  +  
				' n°: ' + CONVERT(VARCHAR(5),M.Numero) 
			   FROM 
					T_MovSchede M WITH (NOLOCK)
						LEFT JOIN T_StatoScheda ST WITH (NOLOCK)
							ON M.CodStatoScheda =ST.Codice
						LEFT JOIN T_Schede S WITH (NOLOCK)
							ON M.CodScheda=S.Codice	
			   WHERE ID=@IDEntita
			   )
		SET @sTmp2=(SELECT  TOP 1
					CHAR(13)+CHAR(10) +
					'BLOCCATA DA: ' + ISNULL(UL.Descrizione,'')  + 
					',il ' + ISNULL(CONVERT(Varchar(10),L.Data,105),'') + ' ' + ISNULL(CONVERT(Varchar(5),L.Data,14),'') + ',PC: ' + ISNULL(L.NomePC ,'') 
			   FROM 
					T_MovLock AS L
						LEFT JOIN T_Login UL
								ON L.CodLogin=UL.Codice	
			   WHERE
					L.IDEntita=@IDEntita AND
					L.CodEntita='SCH')
	
	SET @sTmp=@sTmp + ISNULL(@sTmp2,'')
	
	IF 	ISNULL(@sTmp,'')=''  
	BEGIN
		SET @bErrore=1
		SET @sTmp='ERRORE: ID non trovato'	   
	END	
	SET @sInfo=@sTmp		
END

IF ISNULL(@CodTipoEntita,'')='PAZ' 		
BEGIN
		SET @sTmp=(SELECT  TOP 1
					ISNULL(Nome,'') + ' ' +
					ISNULL(Cognome,'')  +
					' (' + CONVERT(VARCHAR(10), DataNascita,120) + ')'
			   FROM 
					T_Pazienti M WITH (NOLOCK)						
			   WHERE ID=@IDEntita
			   )
	IF 	ISNULL(@sTmp,'')=''  
	BEGIN
		SET @bErrore=1
		SET @sTmp='ERRORE: ID non trovato'	   
	END	
	SET @sInfo=@sTmp		
END

IF ISNULL(@CodTipoEntita,'')='EPI' 		
BEGIN
		
					
	SET @sTmp=(SELECT  TOP 1
					'Nosologico/LisaAttesa : ' + ISNULL(NumeroNosologico,'') + ' ' + 'ListaAttesa : ' +	ISNULL(NumeroListaAttesa,'')  + CHAR(13) + CHAR(10) +
					'Data Ricovero : ' + ISNULL(CONVERT(Varchar(10),DataRicovero,105),'') + ' ' + ISNULL(CONVERT(Varchar(5),DataRicovero,14),'')
			   FROM 
					T_MovEpisodi M WITH (NOLOCK)						
			   WHERE ID=@IDEntita
			   )
	IF 	ISNULL(@sTmp,'')=''  
	BEGIN
		SET @bErrore=1
		SET @sTmp='ERRORE: ID non trovato'	   
	END	
	SET @sInfo=@sTmp		
END


IF ISNULL(@CodTipoEntita,'')='EVC' 		
BEGIN
		SET @sTmp=(SELECT  TOP 1
				'Tipo Referto : ' + ISNULL(TE.Descrizione,'') + '    ' +  'NumeroRefertoDWH : ' + ISNULL(M.NumeroRefertoDWH,'') + '    ' + CHAR(13)+CHAR(10) +	
				'Data Evento : ' + ISNULL(CONVERT(Varchar(10),DataEvento,105),'') + ' ' + ISNULL(CONVERT(Varchar(5),DataEvento,14),'') + CHAR(13)+CHAR(10) +				
				'Stato: ' + ISNULL(SE.Descrizione ,'') + CHAR(13)+CHAR(10) +
				'Vistato: ' + ISNULL(SEV.Descrizione ,'') + ' ' + ISNULL(LV.Descrizione,'') + '    ' + 
							  ISNULL(CONVERT(Varchar(10),M.DataVisione,105),'') + ' ' + ISNULL(CONVERT(Varchar(5),M.DataVisione,14),'')  + CHAR(13)+CHAR(10) +
				'Cartella : ' +  ISNULL(CR.NumeroCartella ,'')  + CHAR(13)+CHAR(10) +				
				'UA : ' +  ISNULL(UA.Descrizione ,'')  			
				
			   FROM 
					T_MovEvidenzaClinica M WITH (NOLOCK)
						LEFT JOIN T_TipoEvidenzaClinica TE WITH (NOLOCK)
							ON M.CodTipoEvidenzaClinica =TE.Codice
						LEFT JOIN T_StatoEvidenzaClinica SE WITH (NOLOCK)
							ON M.CodStatoEvidenzaClinica =SE.Codice
						LEFT JOIN T_StatoEvidenzaClinicaVisione SEV WITH (NOLOCK)
							ON M.CodStatoEvidenzaClinica =SEV.Codice
						LEFT JOIN T_Login LV WITH (NOLOCK)
							ON M.CodUtenteVisione =LV.Codice		
						LEFT JOIN T_MovTrasferimenti TRA
							ON M.IDTrasferimento=TRA.ID							
						LEFT JOIN T_UnitaAtomiche UA WITH (NOLOCK)	
							ON TRA.CodUA =UA.Codice 
						LEFT JOIN T_MovCartelle CR WITH (NOLOCK)	
							ON TRA.IDCartella=CR.ID
			   WHERE M.ID=@IDEntita
			   )
	IF 	ISNULL(@sTmp,'')=''  
	BEGIN
		SET @bErrore=1
		SET @sTmp='ERRORE: ID non trovato'	   
	END	
	SET @sInfo=@sTmp		
END

IF ISNULL(@CodTipoEntita,'')='ALG' 		
BEGIN		
	SET @sTmp=(SELECT  TOP 1
				'Tipo: ' + + ISNULL(TA.Descrizione,'') + '' + CHAR(13)+CHAR(10) +
				'Data Evento: ' + ISNULL(CONVERT(Varchar(10),DataEvento,105),'') + ' ' + ISNULL(CONVERT(Varchar(5),DataEvento,14),'') + CHAR(13)+CHAR(10) +
				'Utente Creazione: ' + ISNULL(CodUtenteRilevazione,'') + CHAR(13)+CHAR(10) +
				'Stato: ' + ISNULL(ST.Descrizione ,'') + CHAR(13)+CHAR(10) +
				'Utente Visto: ' + ISNULL(CodUtenteVisto,'') + '   '  + ISNULL(CONVERT(Varchar(10),DataVisto,105),'') + ' ' + ISNULL(CONVERT(Varchar(5),DataVisto,14),'') + CHAR(13)+CHAR(10) 							
			   FROM 
					T_MovAlertGenerici M WITH (NOLOCK)
						LEFT JOIN T_TipoAlertGenerico TA WITH (NOLOCK) 
							ON M.CodTipoAlertGenerico=TA.Codice

						LEFT JOIN T_StatoAlertGenerico ST WITH (NOLOCK) 
							ON M.CodStatoAlertGenerico=ST.Codice
						
			   WHERE ID=@IDEntita
			   )
	IF 	ISNULL(@sTmp,'')=''  
		BEGIN
			SET @bErrore=1
			SET @sTmp='ERRORE: ID non trovato'	   
		END	
 
	SET @sInfo=@sTmp		
END
SELECT 
	ISNULL(@bErrore,0) AS Errore,
	@sInfo AS Info

END