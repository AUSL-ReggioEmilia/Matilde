CREATE PROCEDURE [dbo].[MSP_CollegaCartella](@xParametri XML)
AS
BEGIN
									 	
			
	DECLARE @uIDCartellaOrigine AS UNIQUEIDENTIFIER	
	DECLARE @uIDCartellaDestinazione AS UNIQUEIDENTIFIER		
	DECLARE @sCodUtente AS VARCHAR(100)		
	DECLARE @xTimeStamp AS XML
	
		DECLARE @uIDEpisodioOrigine AS UNIQUEIDENTIFIER	
	DECLARE @uIDEpisodioDestinazione AS UNIQUEIDENTIFIER	
		
	DECLARE @uIDTrasferimentoOrigine AS UNIQUEIDENTIFIER	
	DECLARE @uIDTrasferimentoDestinazione AS UNIQUEIDENTIFIER	
	
	DECLARE @sNumeroEpisodioOrigine AS VARCHAR(50)
	DECLARE @sNumeroEpisodioDestinazione AS VARCHAR(50)
	DECLARE @sNumeroCartellaOrigine AS VARCHAR(50)
	DECLARE @sNumeroCartellaDestinazione AS VARCHAR(50)
	DECLARE @sDescrizioneRepartoOrigine AS VARCHAR(255)
	DECLARE @sDescrizioneRepartoDestinazione AS VARCHAR(255)
			
	DECLARE @sGUID AS VARCHAR(50)
	DECLARE @uGUID AS UNIQUEIDENTIFIER
	DECLARE @nRecord AS INTEGER	
	DECLARE @uIDSessione AS UNIQUEIDENTIFIER
	DECLARE @xTemp AS XML
	DECLARE @xParLog AS XML
	DECLARE @xLogPrima AS XML
	DECLARE @xLogDopo AS XML
	DECLARE @xTimeStampBase AS XML
	DECLARE @bErrore AS BIT
	
	
		
		SET @uIDCartellaOrigine=(SELECT TOP 1 ValoreParametro.IDCartellaOrigine.value('.','UNIQUEIDENTIFIER')
								  FROM @xParametri.nodes('/Parametri/IDCartellaOrigine') as ValoreParametro(IDCartellaOrigine))	
		
		SET @uIDCartellaDestinazione=(SELECT TOP 1 ValoreParametro.IDCartellaDestinazione.value('.','UNIQUEIDENTIFIER')
								  FROM @xParametri.nodes('/Parametri/IDCartellaDestinazione') as ValoreParametro(IDCartellaDestinazione))		
		SET @sCodUtente=(SELECT TOP 1 ValoreParametro.CodUtenteRilevazione.value('.','VARCHAR(100)')
					  FROM @xParametri.nodes('/Parametri/TimeStamp/CodLogin') as ValoreParametro(CodUtenteRilevazione))	
	
	    SET @xTimeStamp=(SELECT TOP 1 ValoreParametro.TS.query('.')
					  FROM @xParametri.nodes('/Parametri/TimeStamp') as ValoreParametro(TS))
	
	
			
			
	CREATE TABLE #tmpErrori
		(		
			Errore VARCHAR(500) COLLATE Latin1_General_CI_AS
		)
				
		SET @bErrore=0
	SET @nRecord=0
	
	SET @uIDSessione=NEWID()
	
		SET @uIDEpisodioOrigine=(SELECT TOP 1 IDEpisodio FROM T_MovTrasferimenti WHERE IDCartella=@uIDCartellaOrigine AND CodStatoTrasferimento <> 'CA')
	SET @sNumeroEpisodioOrigine =(SELECT TOP 1 ISNULL(NumeroNosologico,NumeroListaAttesa) AS NumeroEpisodio FROM T_MovEpisodi WHERE ID=@uIDEpisodioOrigine)
	SET @sNumeroEpisodioDestinazione=ISNULL(@sNumeroEpisodioOrigine,'')
	
	IF @uIDEpisodioOrigine IS NULL
	BEGIN
		INSERT INTO #tmpErrori(Errore)
				VALUES('(ERR001) ERRORE : Episodio di origine non trovato')
		SET @bErrore=1
	END	
				
		SET @uIDEpisodioDestinazione=(SELECT TOP 1 IDEpisodio FROM T_MovTrasferimenti WHERE IDCartella=@uIDCartellaDestinazione)
	
	SET @sNumeroEpisodioDestinazione =(SELECT TOP 1 ISNULL(NumeroNosologico,NumeroListaAttesa) AS NumeroEpisodio FROM T_MovEpisodi WHERE ID=@uIDEpisodioDestinazione)
	SET @sNumeroEpisodioDestinazione=ISNULL(@sNumeroEpisodioDestinazione,'')
	
	IF @uIDEpisodioDestinazione IS NULL
	BEGIN
		INSERT INTO #tmpErrori(Errore)
				VALUES('(ERR002) ERRORE : Episodio di destinazione non trovato')
		SET @bErrore=1
	END	
	
		SET @uIDTrasferimentoOrigine=(SELECT TOP 1 ID FROM T_MovTrasferimenti WHERE IDCartella=@uIDCartellaOrigine AND CodStatoTrasferimento <> 'CA')
	SET @sDescrizioneRepartoOrigine=(SELECT A.Descrizione 
										  FROM T_MovTrasferimenti M
												INNER JOIN T_UnitaAtomiche A
													ON M.CodUA=A.Codice
										  WHERE M.ID=@uIDTrasferimentoOrigine)	
										  
		SET @uIDTrasferimentoDestinazione=(SELECT TOP 1 ID FROM T_MovTrasferimenti WHERE IDCartella=@uIDCartellaDestinazione)
	SET @sDescrizioneRepartoDestinazione=(SELECT A.Descrizione 
										  FROM T_MovTrasferimenti M
												INNER JOIN T_UnitaAtomiche A
													ON M.CodUA=A.Codice
										  WHERE M.ID=@uIDTrasferimentoDestinazione)	
									
	
	IF @uIDTrasferimentoDestinazione IS NULL
	BEGIN
		INSERT INTO #tmpErrori(Errore)
				VALUES('(ERR003) ERRORE : Trasferimento di destinazione non trovato')
		SET @bErrore=1
	END	
	
	IF @bErrore=0
	BEGIN	

												SET @sNumeroCartellaOrigine=(SELECT TOP 1 NumeroCartella FROM T_MovCartelle WHERE ID=@uIDCartellaOrigine)
			SET @sNumeroCartellaOrigine=ISNULL(@sNumeroCartellaOrigine,'')
			
			SET @sNumeroCartellaDestinazione=(SELECT TOP 1 NumeroCartella FROM T_MovCartelle WHERE ID=@uIDCartellaDestinazione)
			SET @sNumeroCartellaDestinazione=ISNULL(@sNumeroCartellaDestinazione,'')
		
			SET @nRecord=(SELECT COUNT(*) FROM T_MovRelazioniEntita WHERE CodEntita='CAR' AND IDEntita=@uIDCartellaOrigine AND CodEntitaCollegata='CAR' AND IDEntitaCollegata=@uIDCartellaDestinazione)
			
			IF @nRecord=0
			BEGIN
				INSERT INTO T_MovRelazioniEntita(CodEntita,IDEntita,CodEntitaCollegata,IDEntitaCollegata)
				SELECT 
					'CAR',@uIDCartellaOrigine,'CAR',@uIDCartellaDestinazione
				WHERE
					NOT EXISTS	
						(SELECT IDNum FROM T_MovRelazioniEntita
						 WHERE CodEntita='CAR' AND
							IDEntita=@uIDCartellaOrigine AND
							CodEntitaCollegata='CAR' AND
							IDEntitaCollegata=@uIDCartellaDestinazione
						)
			END
				
			SET @nRecord=0			

																			
															
						INSERT INTO T_MovTranscodifiche
				 (IDSessione,
				  CodEntita,
				  IDOrigine,
				  IDDestinazione,
	              CodUtenteInserimento,
	              DataInserimento
	              )
	        SELECT
				@uIDSessione,
				'ALG',
				ID AS IDOrigine,
				NEWID() AS IDDestinazione,
				@sCodUtente,
				GETDATE() AS DataInserimento
	        FROM            
				T_MovAlertGenerici			
			WHERE 				
				IDTrasferimento 				
						IN (SELECT 
								T_MovTrasferimenti.ID
						    FROM 
								T_MovTrasferimenti 
						    WHERE 
								IDCartella=@uIDCartellaOrigine)   
								
																											
						INSERT INTO T_MovAlertGenerici
				(ID				  
				  ,IDEpisodio
				  ,IDTrasferimento
				  ,DataEvento
				  ,DataEventoUTC
				  ,CodTipoAlertGenerico
				  ,CodStatoAlertGenerico
				  ,CodUtenteRilevazione
				  ,CodUtenteUltimaModifica
				  ,CodUtenteVisto
				  ,DataUltimaModifica
				  ,DataUltimaModificaUTC
				  ,DataVisto
				  ,DataVistoUTC
				  )
			SELECT 
					T.IDDestinazione AS ID												,M.IDEpisodio
					,M.IDTrasferimento
					,M.DataEvento
					,M.DataEventoUTC
					,M.CodTipoAlertGenerico
					,M.CodStatoAlertGenerico
					,M.CodUtenteRilevazione
					,M.CodUtenteUltimaModifica
					,M.CodUtenteVisto
					,M.DataUltimaModifica
					,M.DataUltimaModificaUTC
					,M.DataVisto
					,M.DataVistoUTC
			FROM            
				T_MovAlertGenerici M
					INNER JOIN T_MovTranscodifiche T
						ON (M.ID=T.IDOrigine AND
						    T.CodEntita='ALG' AND IDSessione=@uIDSessione)
					LEFT JOIN T_MovAlertGenerici M2
						ON (T.IDDestinazione=M2.ID)
			WHERE 
				M2.ID IS NULL												
						
						INSERT INTO T_MovTranscodifiche
				 (IDSessione,
				  CodEntita,
				  IDOrigine,
				  IDDestinazione,
	              CodUtenteInserimento,
	              DataInserimento)
	        SELECT
				@uIDSessione,
				'SCH',
				ID AS IDOrigine,
				NEWID() AS IDDestinazione,
				@sCodUtente,
				GETDATE() AS DataInserimento
	        FROM            
				T_MovSchede			
			WHERE 
				IDEntita IN 
					(SELECT 
							M.ID 
					 FROM T_MovAlertGenerici M													 INNER JOIN T_MovTrasferimenti MT 
								ON (M.IDTrasferimento=MT.ID)													
					WHERE MT.IDCartella=@uIDCartellaOrigine) 					
								

						INSERT INTO T_MovSchede
	           (ID
	           ,CodUA
	           ,CodEntita
	           ,IDEntita
	           ,IDPaziente
	           ,IDEpisodio
	           ,IDTrasferimento
	           ,CodScheda
	           ,Versione
	           ,Numero
	           ,Dati
	           ,AnteprimaRTF
	           ,AnteprimaTXT
	           ,DatiObbligatoriMancantiRTF
	           ,DatiRilievoRTF
	           ,IDSchedaPadre
	           ,Storicizzata
	           ,CodStatoScheda
	           ,DataCreazione
	           ,DataCreazioneUTC
	           ,CodUtenteRilevazione
	           ,CodUtenteUltimaModifica
	           ,DataUltimaModifica
	           ,DataUltimaModificaUTC
	           ,InEvidenza)
	        SELECT   
			    T.IDDestinazione AS ID								           ,M.CodUA
	           ,M.CodEntita
	           ,T2.IDDestinazione
	           ,M.IDPaziente
	           ,M.IDEpisodio
	           ,M.IDTrasferimento
	           ,M.CodScheda
	           ,M.Versione
	           ,M.Numero
	           ,M.Dati
	           ,M.AnteprimaRTF
	           ,M.AnteprimaTXT
	           ,M.DatiObbligatoriMancantiRTF
	           ,M.DatiRilievoRTF
	           ,M.IDSchedaPadre
	           ,M.Storicizzata
	           ,M.CodStatoScheda
	           ,M.DataCreazione
	           ,M.DataCreazioneUTC
	           ,M.CodUtenteRilevazione
	           ,M.CodUtenteUltimaModifica
	           ,M.DataUltimaModifica
	           ,M.DataUltimaModificaUTC
	           ,M.InEvidenza
			FROM 
				T_MovSchede M
										INNER JOIN T_MovTranscodifiche T
						ON (M.ID=T.IDOrigine AND
						    T.CodEntita='SCH' AND T.IDSessione=@uIDSessione)
						    
										INNER JOIN T_MovTranscodifiche T2
						ON (M.IDEntita=T2.IDOrigine AND
						    T2.CodEntita='ALG' AND T2.IDSessione=@uIDSessione)											    
										LEFT JOIN T_MovSchede M2
						ON (T.IDDestinazione=M2.ID)
						
			WHERE 
				M2.ID IS NULL									
												
						INSERT INTO T_MovTranscodifiche
				 (IDSessione,
				  CodEntita,
				  IDOrigine,
				  IDDestinazione,
	              CodUtenteInserimento,
	              DataInserimento)
	        SELECT
				@uIDSessione,
				'ALL',
				ID AS IDOrigine,
				NEWID() AS IDDestinazione,
				@sCodUtente,
				GETDATE() AS DataInserimento				
	        FROM            
				T_MovAllegati			
			WHERE 				
				IDTrasferimento 				
						IN (SELECT 
								T_MovTrasferimenti.ID
						    FROM 
								T_MovTrasferimenti 
						    WHERE 
								IDCartella=@uIDCartellaOrigine)   
								
																											
						INSERT INTO T_MovAllegati
				( ID      
				  ,IDPaziente
				  ,IDEpisodio
				  ,IDTrasferimento
				  ,NumeroDocumento
				  ,DataEvento
				  ,DataEventoUTC
				  ,TestoRTF
				  ,NotaRTF
				  ,CodFormatoAllegato
				  ,CodTipoAllegato
				  ,CodStatoAllegato
				  ,Documento
				  ,NomeFile
				  ,Estensione
				  ,CodUtenteRilevazione
				  ,CodUtenteUltimaModifica
				  ,DataRilevazione
				  ,DataRilevazioneUTC
				  ,DataUltimaModifica
				  ,DataUltimaModificaUTC
				  )
			SELECT 
				   T.IDDestinazione AS ID									  ,M.IDPaziente
				  ,M.IDEpisodio
				  ,M.IDTrasferimento
				  ,M.NumeroDocumento
				  ,M.DataEvento
				  ,M.DataEventoUTC
				  ,M.TestoRTF
				  ,M.NotaRTF
				  ,M.CodFormatoAllegato
				  ,M.CodTipoAllegato
				  ,M.CodStatoAllegato
				  ,M.Documento
				  ,M.NomeFile
				  ,M.Estensione
				  ,M.CodUtenteRilevazione
				  ,M.CodUtenteUltimaModifica
				  ,M.DataRilevazione
				  ,M.DataRilevazioneUTC
				  ,M.DataUltimaModifica
				  ,M.DataUltimaModificaUTC
			FROM            
				T_MovAllegati M
					INNER JOIN T_MovTranscodifiche T
						ON (M.ID=T.IDOrigine AND
						    T.CodEntita='ALL' AND T.IDSessione=@uIDSessione)
					LEFT JOIN T_MovAllegati M2
						ON (T.IDDestinazione=M2.ID)
			WHERE 
				M2.ID IS NULL											
			
												
						INSERT INTO T_MovTranscodifiche
				 (IDSessione,
				  CodEntita,
				  IDOrigine,
				  IDDestinazione,
	              CodUtenteInserimento,
	              DataInserimento
	              )
	        SELECT
				@uIDSessione,
				'APP',
				ID AS IDOrigine,
				NEWID() AS IDDestinazione,
				@sCodUtente,
				GETDATE() AS DataInserimento
	        FROM            
				T_MovAppuntamenti		
			WHERE 				
				IDTrasferimento 				
						IN (SELECT 
								T_MovTrasferimenti.ID
						    FROM 
								T_MovTrasferimenti 
						    WHERE 
								IDCartella=@uIDCartellaOrigine)   
								
																											
						INSERT INTO T_MovAppuntamenti
				(	ID   
				  ,CodUA
				  ,IDPaziente
				  ,IDEpisodio
				  ,IDTrasferimento
				  ,DataEvento
				  ,DataEventoUTC
				  ,DataInizio
				  ,DataFine
				  ,CodTipoAppuntamento
				  ,CodStatoAppuntamento
				  ,ElencoRisorse
				  ,CodUtenteRilevazione
				  ,CodUtenteUltimaModifica
				  ,DataUltimaModifica
				  ,DataUltimaModificaUTC
				  )
			SELECT 
					T.IDDestinazione AS ID											  ,M.CodUA
				  ,M.IDPaziente
				  ,M.IDEpisodio
				  ,M.IDTrasferimento
				  ,M.DataEvento
				  ,M.DataEventoUTC
				  ,M.DataInizio
				  ,M.DataFine
				  ,M.CodTipoAppuntamento
				  ,CASE 
						WHEN M.CodStatoAppuntamento='PR' THEN 'TR'									ELSE M.CodStatoAppuntamento
				   END AS CodStatoAppuntamento
				  ,M.ElencoRisorse
				  ,M.CodUtenteRilevazione
				  ,M.CodUtenteUltimaModifica
				  ,M.DataUltimaModifica
				  ,M.DataUltimaModificaUTC
			FROM            
				T_MovAppuntamenti M
					INNER JOIN T_MovTranscodifiche T
						ON (M.ID=T.IDOrigine AND
						    T.CodEntita='APP' AND T.IDSessione=@uIDSessione)
					LEFT JOIN T_MovAppuntamenti M2
						ON (T.IDDestinazione=M2.ID)
			WHERE 
				M2.ID IS NULL					
						INSERT INTO T_MovTranscodifiche
				 (IDSessione,
				  CodEntita,
				  IDOrigine,
				  IDDestinazione,
	              CodUtenteInserimento,
	              DataInserimento
	              )
	        SELECT
				@uIDSessione,
				'AGE',
				M.ID AS IDOrigine,
				NEWID() AS IDDestinazione,
				@sCodUtente,
				GETDATE() AS DataInserimento
	        FROM            
				T_MovAppuntamentiAgende M
					INNER JOIN 		
						T_MovAppuntamenti A 
							ON M.IDAppuntamento=A.ID
			WHERE 				
				A.IDTrasferimento 				
						IN (SELECT 
								T_MovTrasferimenti.ID
						    FROM 
								T_MovTrasferimenti 
						    WHERE 
								IDCartella=@uIDCartellaOrigine)   
								
																															
						INSERT INTO T_MovAppuntamentiAgende
				(	ID
				  ,IDAppuntamento
				  ,CodAgenda
				  ,CodStatoAppuntamentoAgenda
				  ,CodUtenteRilevazione
				  ,CodUtenteUltimaModifica
				  ,DataUltimaModifica
				  ,DataUltimaModificaUTC
				  )
			SELECT 
				   T.IDDestinazione AS ID											  ,T2.IDDestinazione											  ,M.CodAgenda
				  ,M.CodStatoAppuntamentoAgenda
				  ,M.CodUtenteRilevazione
				  ,M.CodUtenteUltimaModifica
				  ,M.DataUltimaModifica
				  ,M.DataUltimaModificaUTC
			FROM            
				T_MovAppuntamentiAgende M			
							
					INNER JOIN T_MovTranscodifiche T
						ON (M.ID=T.IDOrigine AND
							 T.CodEntita='AGE' AND T.IDSessione=@uIDSessione )
							 
					LEFT JOIN T_MovAppuntamentiAgende M2
						ON (T.IDDestinazione=M2.ID)									
							
				  INNER JOIN T_MovTranscodifiche T2
						ON (M.IDAppuntamento=T2.IDOrigine AND
							 T2.CodEntita='APP' AND T2.IDSessione=@uIDSessione)		
					
			WHERE 
				M2.ID IS NULL													
						
						INSERT INTO T_MovTranscodifiche
				 (IDSessione,
				  CodEntita,
				  IDOrigine,
				  IDDestinazione,
	              CodUtenteInserimento,
	              DataInserimento)
	        SELECT
				@uIDSessione,
				'SCH',
				ID AS IDOrigine,
				NEWID() AS IDDestinazione,
				@sCodUtente,
				GETDATE() AS DataInserimento
	        FROM            
				T_MovSchede			
			WHERE 
				IDEntita IN 
					(SELECT 
							M.ID 
					 FROM T_MovAppuntamenti M													 INNER JOIN T_MovTrasferimenti MT 
								ON (M.IDTrasferimento=MT.ID)													
					WHERE MT.IDCartella=@uIDCartellaOrigine) 					
								

						INSERT INTO T_MovSchede
	           (ID
	           ,CodUA
	           ,CodEntita
	           ,IDEntita
	           ,IDPaziente
	           ,IDEpisodio
	           ,IDTrasferimento
	           ,CodScheda
	           ,Versione
	           ,Numero
	           ,Dati
	           ,AnteprimaRTF
	           ,AnteprimaTXT
	           ,DatiObbligatoriMancantiRTF
	           ,DatiRilievoRTF
	           ,IDSchedaPadre
	           ,Storicizzata
	           ,CodStatoScheda
	           ,DataCreazione
	           ,DataCreazioneUTC
	           ,CodUtenteRilevazione
	           ,CodUtenteUltimaModifica
	           ,DataUltimaModifica
	           ,DataUltimaModificaUTC
	           ,InEvidenza)
	        SELECT   
			    T.IDDestinazione AS ID								           ,M.CodUA
	           ,M.CodEntita
	           ,T2.IDDestinazione
	           ,M.IDPaziente
	           ,M.IDEpisodio
	           ,M.IDTrasferimento
	           ,M.CodScheda
	           ,M.Versione
	           ,M.Numero
	           ,M.Dati
	           ,M.AnteprimaRTF
	           ,M.AnteprimaTXT
	           ,M.DatiObbligatoriMancantiRTF
	           ,M.DatiRilievoRTF
	           ,M.IDSchedaPadre
	           ,M.Storicizzata
	           ,M.CodStatoScheda
	           ,M.DataCreazione
	           ,M.DataCreazioneUTC
	           ,M.CodUtenteRilevazione
	           ,M.CodUtenteUltimaModifica
	           ,M.DataUltimaModifica
	           ,M.DataUltimaModificaUTC
	           ,M.InEvidenza
			FROM 
				T_MovSchede M
										INNER JOIN T_MovTranscodifiche T
						ON (M.ID=T.IDOrigine AND
						    T.CodEntita='SCH' AND T.IDSessione=@uIDSessione)
						    
										INNER JOIN T_MovTranscodifiche T2
						ON (M.IDEntita=T2.IDOrigine AND
						    T2.CodEntita='APP' AND T2.IDSessione=@uIDSessione)								    
										LEFT JOIN T_MovSchede M2
						ON (T.IDDestinazione=M2.ID)
						
			WHERE 
				M2.ID IS NULL			
			
						UPDATE T_MovAppuntamentiAgende
			SET
				CodStatoAppuntamentoAgenda='CA'
			WHERE
				IDAppuntamento 
					IN (SELECT ID
						FROM	T_MovAppuntamenti M
								INNER JOIN T_MovTranscodifiche T
									ON (M.ID=T.IDDestinazione AND
									T.CodEntita='APP' AND T.IDSessione=@uIDSessione)
						WHERE M.CodStatoAppuntamento='TR'
						)
				AND CodStatoAppuntamentoAgenda <> 'CA'	
				
									
						INSERT INTO T_MovTranscodifiche
				 (IDSessione,
				  CodEntita,
				  IDOrigine,
				  IDDestinazione,
	              CodUtenteInserimento,
	              DataInserimento
	              )
	        SELECT
				@uIDSessione,
				'CIV',
				ID AS IDOrigine,
				NEWID() AS IDDestinazione,
				@sCodUtente,
				GETDATE() AS DataInserimento
	        FROM            
				T_MovCartelleInVisione			
			WHERE 				
				IDTrasferimento 				
						IN (SELECT 
								T_MovTrasferimenti.ID
						    FROM 
								T_MovTrasferimenti 
						    WHERE 
								IDCartella=@uIDCartellaOrigine)   
								
																											
						INSERT INTO T_MovCartelleInVisione
				(  ID
				  ,IDCartella
				  ,IDEpisodio
				  ,IDTrasferimento
				  ,CodRuoloInVisione
				  ,DataInizio
				  ,DataInizioUTC
				  ,DataFine
				  ,DataFineUTC
				  ,CodStatoCartellaInVisione
				  ,DataInserimento
				  ,DataInserimentoUTC
				  ,CodUtenteInserimento
				  ,DataUltimaModifica
				  ,DataUltimaModificaUTC
				  ,CodUtenteUltimaModifica
				  )
			SELECT 
					T.IDDestinazione AS ID											  ,M.IDCartella
				  ,M.IDEpisodio
				  ,M.IDTrasferimento
				  ,M.CodRuoloInVisione
				  ,M.DataInizio
				  ,M.DataInizioUTC
				  ,M.DataFine
				  ,M.DataFineUTC
				  ,M.CodStatoCartellaInVisione
				  ,M.DataInserimento
				  ,M.DataInserimentoUTC
				  ,M.CodUtenteInserimento
				  ,M.DataUltimaModifica
				  ,M.DataUltimaModificaUTC
				  ,M.CodUtenteUltimaModifica
			FROM            
				T_MovCartelleInVisione M
					INNER JOIN T_MovTranscodifiche T
						ON (M.ID=T.IDOrigine AND
						    T.CodEntita='CIV' AND T.IDSessione=@uIDSessione)
					LEFT JOIN T_MovCartelleInVisione M2
						ON (T.IDDestinazione=M2.ID)
			WHERE 
				M2.ID IS NULL												
			
								
													
						INSERT INTO T_MovTranscodifiche
				 (IDSessione,
				  CodEntita,
				  IDOrigine,
				  IDDestinazione,
	              CodUtenteInserimento,
	              DataInserimento
	              )
	        SELECT
				@uIDSessione,
				'DCL',
				ID AS IDOrigine,
				NEWID() AS IDDestinazione,
				@sCodUtente,
				GETDATE() AS DataInserimento
	        FROM            
				T_MovDiarioClinico			
			WHERE 				
				IDTrasferimento 				
						IN (SELECT 
								T_MovTrasferimenti.ID
						    FROM 
								T_MovTrasferimenti 
						    WHERE 
								IDCartella=@uIDCartellaOrigine)   
								
																											
						INSERT INTO T_MovDiarioClinico
				(ID
				  ,CodUA      
				  ,DataEvento
				  ,DataEventoUTC
				  ,IDEpisodio
				  ,IDTrasferimento
				  ,CodTipoDiario
				  ,CodTipoVoceDiario
				  ,CodTipoRegistrazione
				  ,CodEntitaRegistrazione
				  ,IDEntitaRegistrazione
				  ,CodStatoDiario
				  ,CodUtenteRilevazione
				  ,DataInserimento
				  ,DataInserimentoUTC
				  ,DataValidazione
				  ,DataValidazioneUTC
				  ,DataAnnullamento
				  ,DataAnnullamentoUTC
				  )
			SELECT 
				   T.IDDestinazione AS ID											  ,M.CodUA      
				  ,M.DataEvento
				  ,M.DataEventoUTC
				  ,M.IDEpisodio
				  ,M.IDTrasferimento
				  ,M.CodTipoDiario
				  ,M.CodTipoVoceDiario
				  ,M.CodTipoRegistrazione
				  ,M.CodEntitaRegistrazione
				  ,M.IDEntitaRegistrazione
				  ,M.CodStatoDiario
				  ,M.CodUtenteRilevazione
				  ,M.DataInserimento
				  ,M.DataInserimentoUTC
				  ,M.DataValidazione
				  ,M.DataValidazioneUTC
				  ,M.DataAnnullamento
				  ,M.DataAnnullamentoUTC
			FROM            
				T_MovDiarioClinico M
					INNER JOIN T_MovTranscodifiche T
						ON (M.ID=T.IDOrigine AND
						    T.CodEntita='DCL' AND T.IDSessione=@uIDSessione)
					LEFT JOIN T_MovDiarioClinico M2
						ON (T.IDDestinazione=M2.ID)
			WHERE 
				M2.ID IS NULL												
						
						INSERT INTO T_MovTranscodifiche
				 (IDSessione,
				  CodEntita,
				  IDOrigine,
				  IDDestinazione,
	              CodUtenteInserimento,
	              DataInserimento)
	        SELECT
				@uIDSessione,
				'SCH',
				ID AS IDOrigine,
				NEWID() AS IDDestinazione,
				@sCodUtente,
				GETDATE() AS DataInserimento
	        FROM            
				T_MovSchede			
			WHERE 
				IDEntita IN 
					(SELECT 
							M.ID 
					 FROM T_MovDiarioClinico M													 INNER JOIN T_MovTrasferimenti MT 
								ON (M.IDTrasferimento=MT.ID)													
					WHERE MT.IDCartella=@uIDCartellaOrigine) 								

						INSERT INTO T_MovSchede
	           (ID
	           ,CodUA
	           ,CodEntita
	           ,IDEntita
	           ,IDPaziente
	           ,IDEpisodio
	           ,IDTrasferimento
	           ,CodScheda
	           ,Versione
	           ,Numero
	           ,Dati
	           ,AnteprimaRTF
	           ,AnteprimaTXT
	           ,DatiObbligatoriMancantiRTF
	           ,DatiRilievoRTF
	           ,IDSchedaPadre
	           ,Storicizzata
	           ,CodStatoScheda
	           ,DataCreazione
	           ,DataCreazioneUTC
	           ,CodUtenteRilevazione
	           ,CodUtenteUltimaModifica
	           ,DataUltimaModifica
	           ,DataUltimaModificaUTC
	           ,InEvidenza)
	        SELECT   
			    T.IDDestinazione AS ID								           ,M.CodUA
	           ,M.CodEntita
	           ,T2.IDDestinazione
	           ,M.IDPaziente
	           ,M.IDEpisodio
	           ,M.IDTrasferimento
	           ,M.CodScheda
	           ,M.Versione
	           ,M.Numero
	           ,M.Dati
	           ,M.AnteprimaRTF
	           ,M.AnteprimaTXT
	           ,M.DatiObbligatoriMancantiRTF
	           ,M.DatiRilievoRTF
	           ,M.IDSchedaPadre
	           ,M.Storicizzata
	           ,M.CodStatoScheda
	           ,M.DataCreazione
	           ,M.DataCreazioneUTC
	           ,M.CodUtenteRilevazione
	           ,M.CodUtenteUltimaModifica
	           ,M.DataUltimaModifica
	           ,M.DataUltimaModificaUTC
	           ,M.InEvidenza
			FROM 
				T_MovSchede M
										INNER JOIN T_MovTranscodifiche T
						ON (M.ID=T.IDOrigine AND
						    T.CodEntita='SCH' AND T.IDSessione=@uIDSessione)
						    
										INNER JOIN T_MovTranscodifiche T2
						ON (M.IDEntita=T2.IDOrigine AND
						    T2.CodEntita='DCL' AND T2.IDSessione=@uIDSessione)								    
										LEFT JOIN T_MovSchede M2
						ON (T.IDDestinazione=M2.ID)
						
			WHERE 
				M2.ID IS NULL									
															
												
			
						INSERT INTO T_MovTranscodifiche
				 (IDSessione,
				  CodEntita,
				  IDOrigine,
				  IDDestinazione,
	              CodUtenteInserimento,
	              DataInserimento
	              )
	        SELECT
				@uIDSessione,
				'NTG',
				ID AS IDOrigine,
				NEWID() AS IDDestinazione,
				@sCodUtente,
				GETDATE() AS DataInserimento
	        FROM            
				T_MovNote			
			WHERE 				
				IDTrasferimento 				
						IN (SELECT 
								T_MovTrasferimenti.ID
						    FROM 
								T_MovTrasferimenti 
						    WHERE 
								IDCartella=@uIDCartellaOrigine)   
								
																											
						INSERT INTO T_MovTranscodifiche
				 (IDSessione,
				  CodEntita,
				  IDOrigine,
				  IDDestinazione,
	              CodUtenteInserimento,
	              DataInserimento
	              )
	        SELECT
				@uIDSessione,
				'GNTG',
				IDGruppo AS IDOrigine,								NEWID() AS IDDestinazione,
				@sCodUtente,
				GETDATE() AS DataInserimento
			FROM				
				(SELECT IDGruppo					
				FROM            
					T_MovNote
				WHERE 				
					IDTrasferimento 				
							IN (SELECT 
									T_MovTrasferimenti.ID
								FROM 
									T_MovTrasferimenti 
								WHERE 
									IDCartella=@uIDCartellaOrigine)   
									
																																			AND IDGruppo IS NOT NULL
				 GROUP BY IDGruppo 				
			   	) AS Q
																												
						INSERT INTO T_MovNote
			 (ID    
			  ,IDPaziente
			  ,IDEpisodio
			  ,IDTrasferimento
			  ,CodEntita
			  ,IDEntita
			  ,CodStatoNota
			  ,Oggetto
			  ,Descrizione
			  ,Colore
			  ,CodSezione
			  ,CodVoce
			  ,IDGruppo
			  ,DataEvento
			  ,DataEventoUTC
			  ,DataInizio
			  ,DataFine
			  ,CodUtenteRilevazione
			  ,CodUtenteUltimaModifica
			  ,DataUltimaModifica
			  ,DataUltimaModificaUTC)


			SELECT 
				   T.IDDestinazione AS ID											  ,M.IDPaziente
				  ,M.IDEpisodio
				  ,M.IDTrasferimento
				  ,M.CodEntita
				  ,M.IDEntita
				  ,M.CodStatoNota
				  ,M.Oggetto
				  ,M.Descrizione
				  ,M.Colore
				  ,M.CodSezione
				  ,M.CodVoce
				  ,ISNULL(T2.IDDestinazione,M.IDGruppo) AS IDGruppo						  ,M.DataEvento
				  ,M.DataEventoUTC
				  ,M.DataInizio
				  ,M.DataFine
				  ,M.CodUtenteRilevazione
				  ,M.CodUtenteUltimaModifica
				  ,M.DataUltimaModifica
				  ,M.DataUltimaModificaUTC
			FROM            
				T_MovNote M
										INNER JOIN T_MovTranscodifiche T
						ON (M.ID=T.IDOrigine AND
						    T.CodEntita='NTG' AND T.IDSessione=@uIDSessione)
					LEFT JOIN T_MovNote M2
						ON (T.IDDestinazione=M2.ID)
					
										LEFT JOIN T_MovTranscodifiche T2
						ON (M.IDGruppo=T2.IDOrigine AND									T2.CodEntita='GNTG' AND T2.IDSessione=@uIDSessione)	
			WHERE 
				M2.ID IS NULL		
												
						
												
					
											
						INSERT INTO T_MovTranscodifiche
				 (IDSessione,
				  CodEntita,
				  IDOrigine,
				  IDDestinazione,
	              CodUtenteInserimento,
	              DataInserimento
	              )
	        SELECT
				@uIDSessione,
				'PVT',
				ID AS IDOrigine,
				NEWID() AS IDDestinazione,
				@sCodUtente,
				GETDATE() AS DataInserimento
	        FROM            
				T_MovParametriVitali
			WHERE 				
				IDTrasferimento 				
						IN (SELECT 
								T_MovTrasferimenti.ID
						    FROM 
								T_MovTrasferimenti 
						    WHERE 
								IDCartella=@uIDCartellaOrigine)   
								
																											
						INSERT INTO T_MovParametriVitali
				(
					  ID      
					  ,IDEpisodio
					  ,IDTrasferimento
					  ,DataEvento
					  ,DataEventoUTC
					  ,CodTipoParametroVitale
					  ,CodStatoParametroVitale
					  ,ValoriFUT
					  ,ValoriGrafici
					  ,CodUtenteRilevazione
					  ,CodUtenteUltimaModifica
					  ,DataInserimento
					  ,DataInserimentoUTC
					  ,DataUltimaModifica
					  ,DataUltimaModificaUTC
  
				  )
			SELECT 
					T.IDDestinazione AS ID												,M.IDEpisodio
					,M.IDTrasferimento
					,M.DataEvento
					,M.DataEventoUTC
					,M.CodTipoParametroVitale
					,M.CodStatoParametroVitale
					,M.ValoriFUT
					,M.ValoriGrafici
					,M.CodUtenteRilevazione
					,M.CodUtenteUltimaModifica
					,M.DataInserimento
					,M.DataInserimentoUTC
					,M.DataUltimaModifica
					,M.DataUltimaModificaUTC
			FROM            
				T_MovParametriVitali M
					INNER JOIN T_MovTranscodifiche T
						ON (M.ID=T.IDOrigine AND
						    T.CodEntita='PVT' AND T.IDSessione=@uIDSessione)
					LEFT JOIN T_MovParametriVitali M2
						ON (T.IDDestinazione=M2.ID)
			WHERE 
				M2.ID IS NULL												
						
						INSERT INTO T_MovTranscodifiche
				 (IDSessione,
				  CodEntita,
				  IDOrigine,
				  IDDestinazione,
	              CodUtenteInserimento,
	              DataInserimento)
	        SELECT
				@uIDSessione,
				'SCH',
				ID AS IDOrigine,
				NEWID() AS IDDestinazione,
				@sCodUtente,
				GETDATE() AS DataInserimento
	        FROM            
				T_MovSchede			
			WHERE 
				IDEntita IN 
					(SELECT 
							M.ID 
					 FROM T_MovParametriVitali M													 INNER JOIN T_MovTrasferimenti MT 
								ON (M.IDTrasferimento=MT.ID)													
					WHERE MT.IDCartella=@uIDCartellaOrigine) 								

						INSERT INTO T_MovSchede
	           (ID
	           ,CodUA
	           ,CodEntita
	           ,IDEntita
	           ,IDPaziente
	           ,IDEpisodio
	           ,IDTrasferimento
	           ,CodScheda
	           ,Versione
	           ,Numero
	           ,Dati
	           ,AnteprimaRTF
	           ,AnteprimaTXT
	           ,DatiObbligatoriMancantiRTF
	           ,DatiRilievoRTF
	           ,IDSchedaPadre
	           ,Storicizzata
	           ,CodStatoScheda
	           ,DataCreazione
	           ,DataCreazioneUTC
	           ,CodUtenteRilevazione
	           ,CodUtenteUltimaModifica
	           ,DataUltimaModifica
	           ,DataUltimaModificaUTC
	           ,InEvidenza)
	        SELECT   
			    T.IDDestinazione AS ID								           ,M.CodUA
	           ,M.CodEntita
	           ,T2.IDDestinazione
	           ,M.IDPaziente
	           ,M.IDEpisodio
	           ,M.IDTrasferimento
	           ,M.CodScheda
	           ,M.Versione
	           ,M.Numero
	           ,M.Dati
	           ,M.AnteprimaRTF
	           ,M.AnteprimaTXT
	           ,M.DatiObbligatoriMancantiRTF
	           ,M.DatiRilievoRTF
	           ,M.IDSchedaPadre
	           ,M.Storicizzata
	           ,M.CodStatoScheda
	           ,M.DataCreazione
	           ,M.DataCreazioneUTC
	           ,M.CodUtenteRilevazione
	           ,M.CodUtenteUltimaModifica
	           ,M.DataUltimaModifica
	           ,M.DataUltimaModificaUTC
	           ,M.InEvidenza
			FROM 
				T_MovSchede M
										INNER JOIN T_MovTranscodifiche T
						ON (M.ID=T.IDOrigine AND
						    T.CodEntita='SCH' AND T.IDSessione=@uIDSessione)
						    
										INNER JOIN T_MovTranscodifiche T2
						ON (M.IDEntita=T2.IDOrigine AND
						    T2.CodEntita='PVT' AND T2.IDSessione=@uIDSessione)																    
										LEFT JOIN T_MovSchede M2
						ON (T.IDDestinazione=M2.ID)
						
			WHERE 
				M2.ID IS NULL				
		
											
						INSERT INTO T_MovTranscodifiche
				 (IDSessione,
				  CodEntita,
				  IDOrigine,
				  IDDestinazione,
	              CodUtenteInserimento,
	              DataInserimento
	              )
	        SELECT
				@uIDSessione,
				'PRF',
				ID AS IDOrigine,
				NEWID() AS IDDestinazione,
				@sCodUtente,
				GETDATE() AS DataInserimento
	        FROM            
				T_MovPrescrizioni
			WHERE 				
				IDTrasferimento 				
						IN (SELECT 
								T_MovTrasferimenti.ID
						    FROM 
								T_MovTrasferimenti 
						    WHERE 
								IDCartella=@uIDCartellaOrigine)   
								
																											
	
						INSERT INTO T_MovPrescrizioni
				(
					ID   
					,IDEpisodio
					,IDTrasferimento
					,DataEvento
					,DataEventoUTC
					,CodViaSomministrazione
					,CodTipoPrescrizione
					,CodStatoPrescrizione
					,CodStatoContinuazione
					,CodUtenteRilevazione
					,CodUtenteUltimaModifica
					,DataUltimaModifica
					,DataUltimaModificaUTC
					,IDString
  
				  )
			SELECT 
					T.IDDestinazione AS ID												,M.IDEpisodio
					,M.IDTrasferimento
					,M.DataEvento
					,M.DataEventoUTC
					,M.CodViaSomministrazione
					,M.CodTipoPrescrizione
					,M.CodStatoPrescrizione
					,M.CodStatoContinuazione
					,M.CodUtenteRilevazione
					,M.CodUtenteUltimaModifica
					,M.DataUltimaModifica
					,M.DataUltimaModificaUTC
					,CONVERT(VARCHAR(50),T.IDDestinazione)
			FROM            
				T_MovPrescrizioni M
					INNER JOIN T_MovTranscodifiche T
						ON (M.ID=T.IDOrigine AND
						    T.CodEntita='PRF' AND T.IDSessione=@uIDSessione)
					LEFT JOIN T_MovPrescrizioni M2
						ON (T.IDDestinazione=M2.ID)
			WHERE 
				M2.ID IS NULL												
						
						INSERT INTO T_MovTranscodifiche
				 (IDSessione,
				  CodEntita,
				  IDOrigine,
				  IDDestinazione,
	              CodUtenteInserimento,
	              DataInserimento)
	        SELECT
				@uIDSessione,
				'SCH',
				ID AS IDOrigine,
				NEWID() AS IDDestinazione,
				@sCodUtente,
				GETDATE() AS DataInserimento
	        FROM            
				T_MovSchede			
			WHERE 
				IDEntita IN 
					(SELECT 
							M.ID 
					 FROM T_MovPrescrizioni M													 INNER JOIN T_MovTrasferimenti MT 
								ON (M.IDTrasferimento=MT.ID)													
					WHERE MT.IDCartella=@uIDCartellaOrigine) 									

						INSERT INTO T_MovSchede
	           (ID
	           ,CodUA
	           ,CodEntita
	           ,IDEntita
	           ,IDPaziente
	           ,IDEpisodio
	           ,IDTrasferimento
	           ,CodScheda
	           ,Versione
	           ,Numero
	           ,Dati
	           ,AnteprimaRTF
	           ,AnteprimaTXT
	           ,DatiObbligatoriMancantiRTF
	           ,DatiRilievoRTF
	           ,IDSchedaPadre
	           ,Storicizzata
	           ,CodStatoScheda
	           ,DataCreazione
	           ,DataCreazioneUTC
	           ,CodUtenteRilevazione
	           ,CodUtenteUltimaModifica
	           ,DataUltimaModifica
	           ,DataUltimaModificaUTC
	           ,InEvidenza)
	        SELECT   
			    T.IDDestinazione AS ID								           ,M.CodUA
	           ,M.CodEntita
	           ,T2.IDDestinazione
	           ,M.IDPaziente
	           ,M.IDEpisodio
	           ,M.IDTrasferimento
	           ,M.CodScheda
	           ,M.Versione
	           ,M.Numero
	           ,M.Dati
	           ,M.AnteprimaRTF
	           ,M.AnteprimaTXT
	           ,M.DatiObbligatoriMancantiRTF
	           ,M.DatiRilievoRTF
	           ,M.IDSchedaPadre
	           ,M.Storicizzata
	           ,M.CodStatoScheda
	           ,M.DataCreazione
	           ,M.DataCreazioneUTC
	           ,M.CodUtenteRilevazione
	           ,M.CodUtenteUltimaModifica
	           ,M.DataUltimaModifica
	           ,M.DataUltimaModificaUTC
	           ,M.InEvidenza
			FROM 
				T_MovSchede M
										INNER JOIN T_MovTranscodifiche T
						ON (M.ID=T.IDOrigine AND
						    T.CodEntita='SCH' AND T.IDSessione=@uIDSessione)
						    
										INNER JOIN T_MovTranscodifiche T2
						ON (M.IDEntita=T2.IDOrigine AND
						    T2.CodEntita='PRF' AND T2.IDSessione=@uIDSessione)																    
										LEFT JOIN T_MovSchede M2
						ON (T.IDDestinazione=M2.ID)
						
			WHERE 
				M2.ID IS NULL					
											
						INSERT INTO T_MovTranscodifiche
				 (IDSessione,
				  CodEntita,
				  IDOrigine,
				  IDDestinazione,
	              CodUtenteInserimento,
	              DataInserimento
	              )
	        SELECT
				@uIDSessione,
				'PRT',
				M.ID AS IDOrigine,
				NEWID() AS IDDestinazione,
				@sCodUtente,
				GETDATE() AS DataInserimento
	        FROM            
				T_MovPrescrizioniTempi M
					INNER JOIN T_MovPrescrizioni P
						ON M.IDPrescrizione=P.ID
			WHERE 				
				P.IDTrasferimento 				
						IN (SELECT 
								T_MovTrasferimenti.ID
						    FROM 
								T_MovTrasferimenti 
						    WHERE 
								IDCartella=@uIDCartellaOrigine)   
								
																											
	
						INSERT INTO T_MovPrescrizioniTempi
				  (ID     
				  ,IDPrescrizione
				  ,Posologia
				  ,CodStatoPrescrizioneTempi
				  ,CodTipoPrescrizioneTempi
				  ,DataEvento
				  ,DataEventoUTC
				  ,DataOraInizio
				  ,DataOraFine
				  ,AlBisogno
				  ,Durata
				  ,Continuita
				  ,PeriodicitaGiorni
				  ,PeriodicitaOre
				  ,PeriodicitaMinuti
				  ,CodProtocollo
				  ,CodUtenteRilevazione
				  ,CodUtenteUltimaModifica
				  ,CodUtenteValidazione
				  ,CodUtenteSospensione
				  ,DataValidazione
				  ,DataValidazioneUTC
				  ,DataSospensione
				  ,DataSospensioneUTC
				  ,DataUltimaModifica
				  ,DataUltimaModificaUTC
				  ,Manuale
				  ,TempiManuali
				  ,IDString
				  ,IDPrescrizioneString)

			SELECT 
				  T.IDDestinazione AS ID											  ,T2.IDDestinazione AS IDPrescrizione
				  ,M.Posologia
				  ,M.CodStatoPrescrizioneTempi
				  ,M.CodTipoPrescrizioneTempi
				  ,M.DataEvento
				  ,M.DataEventoUTC
				  ,M.DataOraInizio
				  ,M.DataOraFine
				  ,M.AlBisogno
				  ,M.Durata
				  ,M.Continuita
				  ,M.PeriodicitaGiorni
				  ,M.PeriodicitaOre
				  ,M.PeriodicitaMinuti
				  ,M.CodProtocollo
				  ,M.CodUtenteRilevazione
				  ,M.CodUtenteUltimaModifica
				  ,M.CodUtenteValidazione
				  ,M.CodUtenteSospensione
				  ,M.DataValidazione
				  ,M.DataValidazioneUTC
				  ,M.DataSospensione
				  ,M.DataSospensioneUTC
				  ,M.DataUltimaModifica
				  ,M.DataUltimaModificaUTC
				  ,M.Manuale
				  ,M.TempiManuali
				  ,CONVERT(VARCHAR(50),T.IDDestinazione)
				  ,CONVERT(VARCHAR(50),T2.IDDestinazione)
			FROM            
				T_MovPrescrizioniTempi M
										INNER JOIN T_MovTranscodifiche T
						ON (M.ID=T.IDOrigine AND
						    T.CodEntita='PRT' AND T.IDSessione=@uIDSessione)
						    
										INNER JOIN 	T_MovTranscodifiche T2
						ON (M.IDPrescrizione=T2.IDOrigine AND
						    T2.CodEntita='PRF' AND T2.IDSessione=@uIDSessione)
					LEFT JOIN T_MovPrescrizioniTempi M2
						ON (T.IDDestinazione=M2.ID)
			WHERE 
				M2.ID IS NULL												
								
									
						INSERT INTO T_MovTranscodifiche
				 (IDSessione,
				  CodEntita,
				  IDOrigine,
				  IDDestinazione,
	              CodUtenteInserimento,
	              DataInserimento
	              )
	        SELECT
				@uIDSessione,
				'RPT',
				ID AS IDOrigine,
				NEWID() AS IDDestinazione,
				@sCodUtente,
				GETDATE() AS DataInserimento
	        FROM            
				T_MovReport			
			WHERE 				
				IDTrasferimento 				
						IN (SELECT 
								T_MovTrasferimenti.ID
						    FROM 
								T_MovTrasferimenti 
						    WHERE 
								IDCartella=@uIDCartellaOrigine)   
								
																											
									
						INSERT INTO T_MovReport
				(  ID    
				  ,IDPaziente
				  ,IDEpisodio
				  ,IDTrasferimento
				  ,IDCartella
				  ,CodReport
				  ,Documento
				  ,DataEvento
				  ,DataEventoUTC
				  )
			SELECT 
				   T.IDDestinazione AS ID											   ,M.IDPaziente
				  ,M.IDEpisodio
				  ,M.IDTrasferimento
				  ,M.IDCartella
				  ,M.CodReport
				  ,M.Documento
				  ,M.DataEvento
				  ,M.DataEventoUTC
			FROM            
				T_MovReport M
					INNER JOIN T_MovTranscodifiche T
						ON (M.ID=T.IDOrigine AND
						    T.CodEntita='RPT' AND T.IDSessione=@uIDSessione)
					LEFT JOIN T_MovReport M2
						ON (T.IDDestinazione=M2.ID)
			WHERE 
				M2.ID IS NULL		
																
												
						INSERT INTO T_MovTranscodifiche
				 (IDSessione,
				  CodEntita,
				  IDOrigine,
				  IDDestinazione,
	              CodUtenteInserimento,
	              DataInserimento)
	        SELECT
				@uIDSessione,
				'SCH',
				ID AS IDOrigine,
				NEWID() AS IDDestinazione,
				@sCodUtente,
				GETDATE() AS DataInserimento
	        FROM            
				T_MovSchede			
			WHERE 
								CodEntita='EPI' AND									
				IDEpisodio=@uIDEpisodioOrigine 
								
								
						INSERT INTO T_MovSchede
	           (ID
	           ,CodUA
	           ,CodEntita
	           ,IDEntita
	           ,IDPaziente
	           ,IDEpisodio
	           ,IDTrasferimento
	           ,CodScheda
	           ,Versione
	           ,Numero
	           ,Dati
	           ,AnteprimaRTF
	           ,AnteprimaTXT
	           ,DatiObbligatoriMancantiRTF
	           ,DatiRilievoRTF
	           ,IDSchedaPadre
	           ,Storicizzata
	           ,CodStatoScheda
	           ,DataCreazione
	           ,DataCreazioneUTC
	           ,CodUtenteRilevazione
	           ,CodUtenteUltimaModifica
	           ,DataUltimaModifica
	           ,DataUltimaModificaUTC
	           ,InEvidenza)
	        SELECT   
			    T.IDDestinazione AS ID								           ,M.CodUA
	           ,M.CodEntita
	           ,M.IDEntita
	           ,M.IDPaziente
	           ,M.IDEpisodio
	           ,M.IDTrasferimento
	           ,M.CodScheda
	           ,M.Versione
	           ,M.Numero
	           ,M.Dati
	           ,M.AnteprimaRTF
	           ,M.AnteprimaTXT
	           ,M.DatiObbligatoriMancantiRTF
	           ,M.DatiRilievoRTF
	           ,ISNULL(T2.IDDestinazione,M.IDSchedaPadre) AS IDSchedaPadre
	           ,M.Storicizzata
	           ,M.CodStatoScheda
	           ,M.DataCreazione
	           ,M.DataCreazioneUTC
	           ,M.CodUtenteRilevazione
	           ,M.CodUtenteUltimaModifica
	           ,M.DataUltimaModifica
	           ,M.DataUltimaModificaUTC
	           ,M.InEvidenza
			FROM 
				T_MovSchede M
										INNER JOIN T_MovTranscodifiche T
						ON (M.ID=T.IDOrigine AND
						    T.CodEntita='SCH' AND T.IDSessione=@uIDSessione)						    						    
					
										LEFT JOIN T_MovTranscodifiche T2
						ON (M.IDSchedaPadre=T2.IDOrigine AND
						    T2.CodEntita='SCH' AND T2.IDSessione=@uIDSessione)	
						        
										LEFT JOIN T_MovSchede M2
						ON (T.IDDestinazione=M2.ID)
						
			WHERE 
				M2.ID IS NULL	AND							
								M.CodEntita='EPI' AND									
				M.IDEpisodio=@uIDEpisodioOrigine 

		
									
						INSERT INTO T_MovTranscodifiche
				 (IDSessione,
				  CodEntita,
				  IDOrigine,
				  IDDestinazione,
	              CodUtenteInserimento,
	              DataInserimento
	              )
	        SELECT
				@uIDSessione,
				'SGL',
				ID AS IDOrigine,
				NEWID() AS IDDestinazione,
				@sCodUtente,
				GETDATE() AS DataInserimento
	        FROM            
				T_MovSegnalibri			
			WHERE 				
								IDCartella=@uIDCartellaOrigine AND
				IDEntita=@uIDEpisodioOrigine AND
								
								CodEntita='SCH'	AND
				CodEntitaScheda='EPI' 
															
																															
				
						INSERT INTO T_MovSegnalibri
				(  ID     
				  ,CodUtente
				  ,CodRuolo
				  ,IDPaziente
				  ,IDCartella
				  ,CodEntita
				  ,IDEntita
				  ,CodEntitaScheda
				  ,CodScheda
				  ,Numero
				  ,DataInserimento
				  ,DataInserimentoUTC
				  )
			SELECT 
				   T.IDDestinazione AS ID											  ,M.CodUtente
				  ,M.CodRuolo
				  ,M.IDPaziente
				  ,M.IDCartella
				  ,M.CodEntita
				  ,M.IDEntita
				  ,M.CodEntitaScheda
				  ,M.CodScheda
				  ,M.Numero
				  ,M.DataInserimento
				  ,M.DataInserimentoUTC
			FROM            
				T_MovSegnalibri M
					INNER JOIN T_MovTranscodifiche T
						ON (M.ID=T.IDOrigine AND
						    T.CodEntita='SGL' AND T.IDSessione=@uIDSessione )
					LEFT JOIN T_MovSegnalibri M2
						ON (T.IDDestinazione=M2.ID)
			WHERE 
				M2.ID IS NULL		
			

												
						
														
						INSERT INTO T_MovTranscodifiche
				 (IDSessione,
				  CodEntita,
				  IDOrigine,
				  IDDestinazione,
	              CodUtenteInserimento,
	              DataInserimento
	              )
	        SELECT
				@uIDSessione,
				'WKI',
				ID AS IDOrigine,
				NEWID() AS IDDestinazione,
				@sCodUtente,
				GETDATE() AS DataInserimento
	        FROM            
				T_MovTaskInfermieristici
			WHERE 				
				IDTrasferimento 				
						IN (SELECT 
								T_MovTrasferimenti.ID
						    FROM 
								T_MovTrasferimenti 
						    WHERE 
								IDCartella=@uIDCartellaOrigine)   
								
																								
		
						INSERT INTO T_MovTranscodifiche
				 (IDSessione,
				  CodEntita,
				  IDOrigine,
				  IDDestinazione,
	              CodUtenteInserimento,
	              DataInserimento
	              )
	        SELECT
				@uIDSessione,
				'GWKI',
				CONVERT(UNIQUEIDENTIFIER,IDGruppo) AS IDOrigine,								NEWID() AS IDDestinazione,
				@sCodUtente,
				GETDATE() AS DataInserimento
			FROM				
				(SELECT IDGruppo					
				FROM            
					T_MovTaskInfermieristici
				WHERE 				
					IDTrasferimento 				
							IN (SELECT 
									T_MovTrasferimenti.ID
								FROM 
									T_MovTrasferimenti 
								WHERE 
									IDCartella=@uIDCartellaOrigine)   
									
																																			AND ISNULL(IDGruppo,'')<>''				
				 GROUP BY IDGruppo 				
			   	) AS Q
																								 							
				INSERT INTO T_MovTaskInfermieristici
				(
				  ID				  
				  ,IDEpisodio
				  ,IDTrasferimento
				  ,DataEvento
				  ,DataEventoUTC
				  ,CodSistema
				  ,IDSistema
				  ,IDGruppo
				  ,IDTaskIniziale
				  ,CodTipoTaskInfermieristico
				  ,CodStatoTaskInfermieristico
				  ,CodTipoRegistrazione
				  ,CodProtocollo
				  ,CodProtocolloTempo
				  ,DataProgrammata
				  ,DataProgrammataUTC
				  ,DataErogazione
				  ,DataErogazioneUTC
				  ,Sottoclasse
				  ,Note
				  ,DescrizioneFUT
				  ,CodUtenteRilevazione
				  ,CodUtenteUltimaModifica
				  ,DataUltimaModifica
				  ,DataUltimaModificaUTC  
				  )
			SELECT 
				  T.IDDestinazione AS ID											  ,M.IDEpisodio
				  ,M.IDTrasferimento
				  ,M.DataEvento
				  ,M.DataEventoUTC
				  ,M.CodSistema
				  ,ISNULL(CONVERT(VARCHAR(50),T3.IDDestinazione),M.IDSistema) AS IDSistema						  ,ISNULL(CONVERT(VARCHAR(50),T2.IDDestinazione),M.IDGruppo) AS IDGruppo												  ,ISNULL(T4.IDDestinazione,M.IDTaskIniziale) AS IDTaskIniziale									  ,M.CodTipoTaskInfermieristico				  
				  ,CASE 
					WHEN M.CodStatoTaskInfermieristico='PR' THEN 'TR'												ELSE M.CodStatoTaskInfermieristico
				   END AS CodStatoTaskInfermieristico
				  ,M.CodTipoRegistrazione
				  ,M.CodProtocollo
				  ,M.CodProtocolloTempo
				  ,M.DataProgrammata
				  ,M.DataProgrammataUTC
				  ,M.DataErogazione
				  ,M.DataErogazioneUTC
				  ,M.Sottoclasse
				  ,CASE 
					WHEN M.CodStatoTaskInfermieristico='PR' THEN 
						' TASK RINVIATO A CARTELLA N. ' + @sNumeroCartellaDestinazione +' DI ' + @sDescrizioneRepartoDestinazione +', EPISODIO ' + @sNumeroEpisodioDestinazione +					
								ISNULL(M.Note,'')
					ELSE M.Note
				   END AS Note	
				  ,M.DescrizioneFUT
				  ,M.CodUtenteRilevazione
				  ,M.CodUtenteUltimaModifica
				  ,M.DataUltimaModifica
				  ,M.DataUltimaModificaUTC  
			FROM            
				T_MovTaskInfermieristici M
					INNER JOIN T_MovTranscodifiche T
						ON (M.ID=T.IDOrigine AND
						    T.CodEntita='WKI' AND T.IDSessione=@uIDSessione)
						    
					LEFT JOIN T_MovTaskInfermieristici M2
						ON (T.IDDestinazione=M2.ID)
						
					LEFT JOIN T_MovTranscodifiche T2
						ON (M.IDGruppo=CONVERT(VARCHAR(50),T2.IDOrigine) AND									T2.CodEntita='GWKI' AND T2.IDSessione=@uIDSessione)
					
					LEFT JOIN T_MovTranscodifiche T3
						ON (M.IDSistema=CONVERT(VARCHAR(50),T3.IDOrigine) AND									M.CodSistema='PRF' AND
							T3.CodEntita='PRF' AND T3.IDSessione=@uIDSessione)
							
					LEFT JOIN T_MovTranscodifiche T4														ON (M.IDTaskIniziale=T4.IDOrigine AND
						    T4.CodEntita='WKI' AND T4.IDSessione=@uIDSessione)		
										
			WHERE 
		
				M2.ID IS NULL						
			
			INSERT INTO T_MovTranscodifiche
				 (IDSessione,
				  CodEntita,
				  IDOrigine,
				  IDDestinazione,
	              CodUtenteInserimento,
	              DataInserimento)
	        SELECT
				@uIDSessione,
				'SCH',
				ID AS IDOrigine,
				NEWID() AS IDDestinazione,
				@sCodUtente,
				GETDATE() AS DataInserimento
	        FROM            
				T_MovSchede			
			WHERE 
				IDEntita IN 
					(SELECT 
							M.ID 
					 FROM T_MovTaskInfermieristici M													 INNER JOIN T_MovTrasferimenti MT 
								ON (M.IDTrasferimento=MT.ID)													
					WHERE MT.IDCartella=@uIDCartellaOrigine) 									

						INSERT INTO T_MovSchede
	           (ID
	           ,CodUA
	           ,CodEntita
	           ,IDEntita
	           ,IDPaziente
	           ,IDEpisodio
	           ,IDTrasferimento
	           ,CodScheda
	           ,Versione
	           ,Numero
	           ,Dati
	           ,AnteprimaRTF
	           ,AnteprimaTXT
	           ,DatiObbligatoriMancantiRTF
	           ,DatiRilievoRTF
	           ,IDSchedaPadre
	           ,Storicizzata
	           ,CodStatoScheda
	           ,DataCreazione
	           ,DataCreazioneUTC
	           ,CodUtenteRilevazione
	           ,CodUtenteUltimaModifica
	           ,DataUltimaModifica
	           ,DataUltimaModificaUTC
	           ,InEvidenza)
	        SELECT   
			    T.IDDestinazione AS ID								           ,M.CodUA
	           ,M.CodEntita
	           ,T2.IDDestinazione
	           ,M.IDPaziente
	           ,M.IDEpisodio
	           ,M.IDTrasferimento
	           ,M.CodScheda
	           ,M.Versione
	           ,M.Numero
	           ,M.Dati
	           ,M.AnteprimaRTF
	           ,M.AnteprimaTXT
	           ,M.DatiObbligatoriMancantiRTF
	           ,M.DatiRilievoRTF
	           ,M.IDSchedaPadre
	           ,M.Storicizzata
	           ,M.CodStatoScheda
	           ,M.DataCreazione
	           ,M.DataCreazioneUTC
	           ,M.CodUtenteRilevazione
	           ,M.CodUtenteUltimaModifica
	           ,M.DataUltimaModifica
	           ,M.DataUltimaModificaUTC
	           ,M.InEvidenza
			FROM 
				T_MovSchede M
										INNER JOIN T_MovTranscodifiche T
						ON (M.ID=T.IDOrigine AND
						    T.CodEntita='SCH' AND T.IDSessione=@uIDSessione)
						    
										INNER JOIN T_MovTranscodifiche T2
						ON (M.IDEntita=T2.IDOrigine AND
						    T2.CodEntita='WKI' AND T2.IDSessione=@uIDSessione)							    
										LEFT JOIN T_MovSchede M2
						ON (T.IDDestinazione=M2.ID)
						
			WHERE 
				M2.ID IS NULL							
						INSERT INTO T_MovRelazioniEntita		
					  (CodEntita
					  ,IDEntita
					  ,CodEntitaCollegata
					  ,IDEntitaCollegata)
			SELECT
				M.CodEntita,
				T.IDDestinazione,
				M.CodEntitaCollegata,
				ISNULL(T2.IDDestinazione,M.IDEntitaCollegata) AS IDEntitaCollegata
			FROM
				T_MovRelazioniEntita M
					INNER JOIN T_MovTranscodifiche T
						ON (M.IDEntita=T.IDOrigine AND
						    T.CodEntita='WKI' AND T.IDSessione=@uIDSessione)		
					LEFT JOIN T_MovTranscodifiche T2
						ON (M.IDEntitaCollegata=T2.IDOrigine AND
						    T2.CodEntita='WKI' AND T2.IDSessione=@uIDSessione)		
			WHERE
				M.CodEntita='WKI' AND
				M.CodEntitaCollegata='WKI'

							    
									
														
											
						UPDATE T_MovSchede
			SET IDEpisodio=@uIDEpisodioDestinazione,
				IDTrasferimento=@uIDTrasferimentoDestinazione
			WHERE				
				IDEntita IN (SELECT 
									M.ID 
							 FROM T_MovAlertGenerici M
									 INNER JOIN T_MovTrasferimenti MT 
										ON (M.IDTrasferimento=MT.ID)													
							WHERE MT.IDCartella=@uIDCartellaOrigine) AND
				ID IN (SELECT IDOrigine FROM T_MovTranscodifiche WHERE IDsessione=@uIDSessione AND CodEntita='SCH')			
				
						UPDATE T_MovAlertGenerici
			SET IDEpisodio=@uIDEpisodioDestinazione,
				IDTrasferimento=@uIDTrasferimentoDestinazione
			WHERE
				IDEpisodio=@uIDEpisodioOrigine AND
				IDTrasferimento IN (SELECT ID FROM T_MovTrasferimenti WHERE IDCartella=@uIDCartellaOrigine) AND
				ID IN (SELECT IDOrigine FROM T_MovTranscodifiche WHERE IDsessione=@uIDSessione AND CodEntita='ALG')
				
		
												
						UPDATE T_MovAllegati
			SET IDEpisodio=@uIDEpisodioDestinazione,
				IDTrasferimento=@uIDTrasferimentoDestinazione
			WHERE
				IDEpisodio=@uIDEpisodioOrigine AND
				IDTrasferimento IN (SELECT ID FROM T_MovTrasferimenti WHERE IDCartella=@uIDCartellaOrigine) AND
				ID IN (SELECT IDOrigine FROM T_MovTranscodifiche WHERE IDsessione=@uIDSessione AND CodEntita='ALL')

				
														
						UPDATE T_MovSchede
			SET IDEpisodio=@uIDEpisodioDestinazione,
				IDTrasferimento=@uIDTrasferimentoDestinazione
			WHERE				
				IDEntita IN (SELECT 
									M.ID 
							 FROM T_MovAppuntamenti M
									 INNER JOIN T_MovTrasferimenti MT 
										ON (M.IDTrasferimento=MT.ID)													
							WHERE MT.IDCartella=@uIDCartellaOrigine) AND
				ID IN (SELECT IDOrigine FROM T_MovTranscodifiche WHERE IDsessione=@uIDSessione AND CodEntita='SCH')		

						UPDATE T_MovAppuntamenti
			SET IDEpisodio=@uIDEpisodioDestinazione,
				IDTrasferimento=@uIDTrasferimentoDestinazione
			WHERE
				IDEpisodio=@uIDEpisodioOrigine AND
				IDTrasferimento IN (SELECT ID FROM T_MovTrasferimenti WHERE IDCartella=@uIDCartellaOrigine) AND
				ID IN (SELECT IDOrigine FROM T_MovTranscodifiche WHERE IDsessione=@uIDSessione AND CodEntita='APP')									
					
			
									
			UPDATE T_MovCartelleInVisione
			SET IDEpisodio=@uIDEpisodioDestinazione,
				IDTrasferimento=@uIDTrasferimentoDestinazione,
				IDCartella=@uIDCartellaDestinazione
			WHERE
				IDEpisodio=@uIDEpisodioOrigine AND
				IDTrasferimento IN (SELECT ID FROM T_MovTrasferimenti WHERE IDCartella=@uIDCartellaOrigine) AND
				ID IN (SELECT IDOrigine FROM T_MovTranscodifiche WHERE IDsessione=@uIDSessione AND CodEntita='CIV')
				
			
												
				
							
												
						UPDATE T_MovNote
			SET IDEpisodio=@uIDEpisodioDestinazione,
				IDTrasferimento=@uIDTrasferimentoDestinazione				
			WHERE
				IDEpisodio=@uIDEpisodioOrigine AND
				IDTrasferimento IN (SELECT ID FROM T_MovTrasferimenti WHERE IDCartella=@uIDCartellaOrigine) AND				
				ID IN (SELECT IDOrigine FROM T_MovTranscodifiche WHERE IDsessione=@uIDSessione AND CodEntita='NTG')
				
						
												
						
												
						
			
									
						
											
						UPDATE T_MovSchede
			SET IDEpisodio=@uIDEpisodioDestinazione,
				IDTrasferimento=@uIDTrasferimentoDestinazione
			WHERE				
				IDEntita IN (SELECT 
									M.ID 
							 FROM T_MovParametriVitali M
									 INNER JOIN T_MovTrasferimenti MT 
										ON (M.IDTrasferimento=MT.ID)													
							WHERE MT.IDCartella=@uIDCartellaOrigine)  AND
				ID IN (SELECT IDOrigine FROM T_MovTranscodifiche WHERE IDsessione=@uIDSessione AND CodEntita='SCH')		
				
						UPDATE T_MovParametriVitali
			SET IDEpisodio=@uIDEpisodioDestinazione,
				IDTrasferimento=@uIDTrasferimentoDestinazione
			WHERE
				IDEpisodio=@uIDEpisodioOrigine AND
				IDTrasferimento IN (SELECT ID FROM T_MovTrasferimenti WHERE IDCartella=@uIDCartellaOrigine) AND				
				ID IN (SELECT IDOrigine FROM T_MovTranscodifiche WHERE IDsessione=@uIDSessione AND CodEntita='PVT')
											
												
						UPDATE T_MovSchede
			SET IDEpisodio=@uIDEpisodioDestinazione,
				IDTrasferimento=@uIDTrasferimentoDestinazione
			WHERE				
				IDEntita IN (SELECT 
									M.ID 
							 FROM T_MovPrescrizioni M
									 INNER JOIN T_MovTrasferimenti MT 
										ON (M.IDTrasferimento=MT.ID)													
							WHERE MT.IDCartella=@uIDCartellaOrigine) AND
				ID IN (SELECT IDOrigine FROM T_MovTranscodifiche WHERE IDsessione=@uIDSessione AND CodEntita='SCH')	
				
						UPDATE T_MovPrescrizioni
			SET IDEpisodio=@uIDEpisodioDestinazione,
				IDTrasferimento=@uIDTrasferimentoDestinazione
			WHERE
				IDEpisodio=@uIDEpisodioOrigine AND
				IDTrasferimento IN (SELECT ID FROM T_MovTrasferimenti WHERE IDCartella=@uIDCartellaOrigine) AND				
				ID IN (SELECT IDOrigine FROM T_MovTranscodifiche WHERE IDsessione=@uIDSessione AND CodEntita='PRF')
																							
												
							
						
														
						UPDATE T_MovReport
			SET IDEpisodio=@uIDEpisodioDestinazione,
				IDTrasferimento=@uIDTrasferimentoDestinazione
			WHERE
				IDEpisodio=@uIDEpisodioOrigine AND
				IDTrasferimento IN (SELECT ID FROM T_MovTrasferimenti WHERE IDCartella=@uIDCartellaOrigine) AND				
				ID IN (SELECT IDOrigine FROM T_MovTranscodifiche WHERE IDsessione=@uIDSessione AND CodEntita='RPT')
				
			

			
									
									
			UPDATE T_MovSchede
			SET 
				IDEntita=@uIDEpisodioDestinazione,
				IDEpisodio=@uIDEpisodioDestinazione,
				IDTrasferimento=@uIDTrasferimentoDestinazione
			WHERE				
								CodEntita='EPI' AND									
				IDEpisodio=@uIDEpisodioOrigine AND				
				ID IN (SELECT IDOrigine FROM T_MovTranscodifiche WHERE IDsessione=@uIDSessione AND CodEntita='SCH')			
							
		
												
						UPDATE T_MovSegnalibri
			SET IDEntita=@uIDEpisodioDestinazione,
				IDCartella=@uIDCartellaDestinazione
			WHERE
				IDEntita=@uIDEpisodioOrigine AND
				IDCartella=@uIDCartellaOrigine AND				
				ID IN (SELECT IDOrigine FROM T_MovTranscodifiche WHERE IDsessione=@uIDSessione AND CodEntita='SGL')
	
												
						UPDATE T_MovSchede
			SET IDEpisodio=@uIDEpisodioDestinazione,
				IDTrasferimento=@uIDTrasferimentoDestinazione
			WHERE				
				IDEntita IN (SELECT 
									M.ID 
							 FROM T_MovTaskInfermieristici M
									 INNER JOIN T_MovTrasferimenti MT 
										ON (M.IDTrasferimento=MT.ID)													
							WHERE MT.IDCartella=@uIDCartellaOrigine) AND
				ID IN (SELECT IDOrigine FROM T_MovTranscodifiche WHERE IDsessione=@uIDSessione AND CodEntita='SCH')			
				
						UPDATE T_MovTaskInfermieristici
			SET IDEpisodio=@uIDEpisodioDestinazione,
				IDTrasferimento=@uIDTrasferimentoDestinazione,				
				Note= CASE 
							WHEN CodStatoTaskInfermieristico='PR' THEN 
									'TASK TRASCRITTO DA CARTELLA N. ' + @sNumeroCartellaOrigine + ' DI ' + @sDescrizioneRepartoOrigine +', EPISODIO ' + @sNumeroEpisodioOrigine +					
											ISNULL(Note,'')
							ELSE Note
						END
			WHERE
				IDEpisodio=@uIDEpisodioOrigine AND
				IDTrasferimento IN (SELECT ID FROM T_MovTrasferimenti WHERE IDCartella=@uIDCartellaOrigine) AND				
				ID IN (SELECT IDOrigine FROM T_MovTranscodifiche WHERE IDsessione=@uIDSessione AND CodEntita='WKI')

												
								
						UPDATE T_MovSchede
			SET IDEpisodio=@uIDEpisodioDestinazione,
				IDTrasferimento=@uIDTrasferimentoDestinazione
			WHERE				
				IDEntita IN (SELECT 
									M.ID 
							 FROM T_MovDiarioClinico M
									 INNER JOIN T_MovTrasferimenti MT 
										ON (M.IDTrasferimento=MT.ID)													
							WHERE MT.IDCartella=@uIDCartellaOrigine) AND
				ID IN (SELECT IDOrigine FROM T_MovTranscodifiche WHERE IDsessione=@uIDSessione AND CodEntita='SCH')		
			
						UPDATE T_MovDiarioClinico
			SET IDEpisodio=@uIDEpisodioDestinazione,
				IDTrasferimento=@uIDTrasferimentoDestinazione				
			WHERE
				IDEpisodio=@uIDEpisodioOrigine AND
				IDTrasferimento IN (SELECT ID FROM T_MovTrasferimenti WHERE IDCartella=@uIDCartellaOrigine) AND				
				ID IN (SELECT IDOrigine FROM T_MovTranscodifiche WHERE IDsessione=@uIDSessione AND CodEntita='DCL')
			
												
						UPDATE T_MovDiarioClinico
			SET IDEntitaRegistrazione=T.IDDestinazione
			FROM 
				T_MovDiarioClinico M
					INNER JOIN 
						 T_MovTranscodifiche T
							ON  (M.IDEntitaRegistrazione = T.IDOrigine AND
								 M.CodEntitaRegistrazione =T.CodEntita AND
								 T.IDSessione=@uIDSessione) 
			WHERE							
				ID IN (SELECT IDDestinazione FROM T_MovTranscodifiche WHERE IDsessione=@uIDSessione AND CodEntita='DCL')							
				

												
						SET @xTimeStamp.modify('delete (/TimeStamp/CodEntita)[1]') 			
			SET @xTimeStamp.modify('insert <CodEntita>CAR</CodEntita> into (/TimeStamp)[1]')	
			
						SET @xTimeStamp.modify('delete (/TimeStamp/CodAzione)[1]') 			
			SET @xTimeStamp.modify('insert <CodAzione>COL</CodAzione> into (/TimeStamp)[1]')	
			SET @xLogDopo=Convert(XML,'<DataSet></DataSet>')
			SET @xLogPrima=''
			
			SET @xTemp=
				(SELECT * FROM 
					(SELECT 
						  @uIDCartellaOrigine AS IDCartellaOriginale,
						  @uIDCartellaDestinazione AS IDCartellaDestinazione,
						  GETDATE() AS DataCollegamento,
						  @sCodUtente AS CodUtente
				
					) AS [Table]
				FOR XML AUTO, ELEMENTS)

			SET @xLogDopo.modify('insert sql:variable("@xTemp") as first into (/DataSet)[1]')
	
			
			SET @xParLog=CONVERT(XML,'<Parametri><LogDopo/></Parametri>')			
			SET @xParLog.modify('insert sql:variable("@xLogDopo") as first into (/Parametri/LogDopo)[1]')
			SET @xParLog.modify('insert sql:variable("@xTimeStamp") as last into (/Parametri)[1]')
		
			EXEC MSP_InsMovLog @xParLog
		
	END

	
				SET @bErrore= ISNULL(@bErrore,0) 
			
	SELECT Errore FROM #tmpErrori	
	
		DROP TABLE #tmpErrori
	
END