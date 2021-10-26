CREATE PROCEDURE [dbo].[MSP_InsMovLog](@xParametri AS XML)
AS

BEGIN


	DECLARE @sCodUtente AS VARCHAR(100)
	DECLARE @sCodEntita AS VARCHAR(50)
	DECLARE @sCodAzione AS VARCHAR(20)
	DECLARE @sComputerName AS VARCHAR(50)
	DECLARE @sIpAddress AS VARCHAR(50)
	DECLARE @sCodEvento AS VARCHAR(100)
	DECLARE @sCodEventoParametro AS VARCHAR(100)
	DECLARE @nTipoOperazione AS SMALLINT		
	DECLARE @sOperazione AS VARCHAR(50)
	DECLARE @xLogPrima AS XML
	DECLARE @xLogDopo AS XML
	
	DECLARE @bFlagInserimento AS bit

	-- @sCodUtente
	SET @sCodUtente	=(SELECT TOP 1 ValoreParametro.CodUtente.value('.','VARCHAR(100)')
					  FROM @xParametri.nodes('/Parametri/TimeStamp/CodLogin') as ValoreParametro(CodUtente))	
	SET @sCodUtente	=ISNULL(@sCodUtente,'')
	
	-- @sNomePC
	SET @sComputerName	=(SELECT TOP 1 ValoreParametro.NomePC.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/TimeStamp/NomePC') as ValoreParametro(NomePC))	
	SET @sComputerName	=ISNULL(@sComputerName,'')
	
	-- @sIpAddress
	SET @sIpAddress	=(SELECT TOP 1 ValoreParametro.IpAddress.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/TimeStamp/IndirizzoIP') as ValoreParametro(IpAddress))	
	SET @sIpAddress	=ISNULL(@sIpAddress,'')
	
	-- @xLogPrima	
	IF @xParametri.exist('(//LogPrima)')=1		  
		SET @xLogPrima=(SELECT TOP 1 ValoreParametro.CodLogPrima.query('./*')
						  FROM @xParametri.nodes('/Parametri/LogPrima') as ValoreParametro(CodLogPrima))		
		ELSE
			SET @xLogPrima=NULL					
	
	
	-- @xLogDopo	
	IF @xParametri.exist('(//LogDopo)')=1			  
		SET @xLogDopo=(SELECT TOP 1 ValoreParametro.CodLogDopo.query('./*')
						  FROM @xParametri.nodes('/Parametri/LogDopo') as ValoreParametro(CodLogDopo))	
		ELSE 
			SET @xLogDopo=NULL				  			
	
	
	-- @sCodAzione	
	SET @sCodAzione	=(SELECT TOP 1 ValoreParametro.CodAzione.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/TimeStamp/CodAzione') as ValoreParametro(CodAzione))	
	SET @sCodAzione	=ISNULL(@sCodAzione,'')
	
	-- @sCodEntita
	SET @sCodEntita	=(SELECT TOP 1 ValoreParametro.CodEntita.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/TimeStamp/CodEntita') as ValoreParametro(CodEntita))	
					  	
	-- @sCodEventoParametro
	SET @sCodEventoParametro	=(SELECT TOP 1 ValoreParametro.CodEvento.value('.','VARCHAR(100)')
					  FROM @xParametri.nodes('/Parametri/CodEvento') as ValoreParametro(CodEvento))	
					  		
	SET @nTipoOperazione=(CASE 
							WHEN @sCodAzione IN ('INS', 'TRA') THEN 1
							ELSE 2
						   END)	
	
	SET @sOperazione=(CASE 
						WHEN @nTipoOperazione=1 THEN 'Nuovo'
						WHEN @nTipoOperazione=2 THEN 'Modifica'
						WHEN @nTipoOperazione=3 THEN 'Cancella'
						ELSE ''
					 END)

	
	----------------------------
	--		Diario Clinico
	---------------------------	

	IF @sCodEntita='DCL' 
		BEGIN
			/*
				ScciDCL01	Nuova Voce Diario Clinico
				ScciDCL02	Modifica Voce Diario Clinicio
				ScciDCL03	Annulla Voce Diario Clinico
				ScciDCL04	Valida Voce Diario Clinico
				ScciDCL05	Cancella Vocde Diario Clinico
			*/
			
			SET @sCodEvento=(CASE 
								WHEN @sCodAzione='INS' THEN 'ScciDCL01'				-- Inserisci
								WHEN @sCodAzione='MOD' THEN 'ScciDCL02'				-- Modifica
								WHEN @sCodAzione='CAN' THEN 'ScciDCL05'				-- Cancella
								WHEN @sCodAzione='ANN' THEN 'ScciDCL03'				-- Annulla
								WHEN @sCodAzione='COM' THEN ''						-- Completa
								WHEN @sCodAzione='VAL' THEN 'ScciDCL04'				-- Valida
								WHEN @sCodAzione='VIS' THEN ''						-- Visualizza								
								ELSE ''
							 END)		
		END
	
	----------------------------
	--		AlertAllergieAnamnesi
	---------------------------	
	IF @sCodEntita='ALA' 
		BEGIN
			/*
					ScciALA01	Nuova Alert Allergie Anamnesi 	
					ScciALA02	Modifica Alert Allergie Anamnesi 
					ScciALA03	Annulla Alert Allergie Anamnesi 
					ScciALA04	Visualizza Alert Allergie Anamnesi 
					ScciALA05	Cancella Alert Allergie Anamnesi 
			*/
			
			SET @sCodEvento=(CASE 
								WHEN @sCodAzione='INS' THEN 'ScciALA01'				-- Inserisci
								WHEN @sCodAzione='MOD' THEN 'ScciALA02'				-- Modifica
								WHEN @sCodAzione='CAN' THEN 'ScciALA05'				-- Cancella
								WHEN @sCodAzione='ANN' THEN 'ScciALA03'				-- Annulla
								WHEN @sCodAzione='COM' THEN ''						-- Completa
								WHEN @sCodAzione='VAL' THEN ''						-- Valida
								WHEN @sCodAzione='VIS' THEN 'ScciALA04'				-- Visualizza								
								ELSE ''
							 END)		
		END	
	
	----------------------------
	--		AlertGenerici
	---------------------------	
	IF @sCodEntita='ALG' 
		BEGIN
			/*
					ScciALG01	Nuova Alert Generico	
					ScciALG02	Modifica Alert Generico	
					ScciALG03	Annulla Alert Generico	
					ScciALG04	Visualizza Alert Generico
					ScciALG05	Valida Alert Generico	
					ScciALA06	Cancella Alert Generico	
			*/
			
			SET @sCodEvento=(CASE 
								WHEN @sCodAzione='INS' THEN 'ScciALG01'				-- Inserisci
								WHEN @sCodAzione='MOD' THEN 'ScciALG02'				-- Modifica
								WHEN @sCodAzione='CAN' THEN 'ScciALA06'				-- Cancella
								WHEN @sCodAzione='ANN' THEN 'ScciALG03'				-- Annulla
								WHEN @sCodAzione='COM' THEN ''						-- Completa
								WHEN @sCodAzione='VAL' THEN 'ScciALG05'				-- Valida
								WHEN @sCodAzione='VIS' THEN 'ScciALG04'				-- Visualizza								
								ELSE ''
							 END)		
		END		

	----------------------------
	--		Evidenza Clinica
	---------------------------	
	IF @sCodEntita='EVC' 
		BEGIN
			/*
					ScciEVC01	Nuova Evidenza Cinica
					ScciEVC02	Modifica Evidenza Cinica
					ScciEVC03	Annulla Evidenza Cinica
					ScciEVC04	Visualizza Evidenza Cinica
					ScciEVC06	Cancella Evidenza Clinica
					ScciEVC05	Valida Evidenza Clinica	
					ScciEVC07	Inserimento Evidenza Clinica da Matilde
			*/
			
			SET @sCodEvento=(CASE 
								WHEN @sCodAzione='INS' THEN 'ScciEVC01'				-- Inserisci
								WHEN @sCodAzione='MOD' THEN 'ScciEVC02'				-- Modifica
								WHEN @sCodAzione='CAN' THEN 'ScciEVC06'				-- Cancella
								WHEN @sCodAzione='ANN' THEN 'ScciEVC03'				-- Annulla
								WHEN @sCodAzione='COM' THEN ''						-- Completa
								WHEN @sCodAzione='VAL' THEN 'ScciEVC05'				-- Valida
								WHEN @sCodAzione='VIS' THEN 'ScciEVC04'				-- Visualizza
								WHEN @sCodAzione='INSMAT' THEN 'ScciEVC07'			-- Inserimento Evidenza Clinica da Matilde
								ELSE ''
							 END)		
		END		
	

	----------------------------
	--		Parametri Vitali
	---------------------------	
	IF @sCodEntita='PVT' 
		BEGIN
			/*
					ScciPVT01	Nuova Parametro Vitale
					ScciPVT02	Modifica Parametro Vitale
					ScciPVT03	Annulla Parametro Vitale
					ScciPVT04	Visualizza Parametro Vitale
					ScciPVT05	Cancella Parametro Vitale 
					ScciPVT06	Cancella Parametro Vitale
			*/
			
			SET @sCodEvento=(CASE 
								WHEN @sCodAzione='INS' THEN 'ScciPVT01'				-- Inserisci
								WHEN @sCodAzione='MOD' THEN 'ScciPVT02'				-- Modifica
								WHEN @sCodAzione='CAN' THEN 'ScciPVT05'				-- Cancella
								WHEN @sCodAzione='ANN' THEN 'ScciPVT03'				-- Annulla
								WHEN @sCodAzione='COM' THEN ''						-- Completa
								WHEN @sCodAzione='VAL' THEN 'ScciPVT05'				-- Valida
								WHEN @sCodAzione='VIS' THEN 'ScciPVT04'				-- Visualizza								
								ELSE ''
							 END)		
		END		
		

	----------------------------
	--		Task Infermieristici
	---------------------------	
	IF @sCodEntita='WKI' 
		BEGIN
			/*
					ScciWKI01	Nuova Task Infermieristico
					ScciWKI02	Modifica Task Infermieristico
					ScciWKI03	Annulla Task Infermieristico
					ScciWKI04	Trascrizione
					ScciWKI05	Valida Task Infermieristico
					ScciWKI06	Cancella Task Infermieristico
					ScciWKI07	NON USATO
			*/
			
			SET @sCodEvento=(CASE 
								WHEN @sCodAzione='INS' THEN 'ScciWKI01'				-- Inserisci
								WHEN @sCodAzione='MOD' THEN 'ScciWKI02'				-- Modifica
								WHEN @sCodAzione='CAN' THEN 'ScciWKI06'				-- Cancella
								WHEN @sCodAzione='ANN' THEN 'ScciWKI03'				-- Annulla								
								WHEN @sCodAzione='VAL' THEN 'ScciWKI05'				-- Valida
								WHEN @sCodAzione='TRA' THEN 'ScciWKI04'				-- Trascritto
								ELSE ''
							 END)		
		END		

	----------------------------
	--		Schede Cliniche
	---------------------------	
	IF @sCodEntita='SCH' 
		BEGIN
			/*
					ScciSCH01	Nuova Scheda
					ScciSCH02	Modifica Scheda
					ScciSCH03	Annulla Scheda
					ScciSCH04	Visualizza Scheda
					ScciSCH05	Valida Scheda
					ScciSCH06	Cancella Scheda
					ScciSCH07	Completa Task Infermieristico
			*/
			
			SET @sCodEvento=(CASE 
								WHEN @sCodAzione='INS' THEN 'ScciSCH01'				-- Inserisci
								WHEN @sCodAzione='MOD' THEN 'ScciSCH02'				-- Modifica
								WHEN @sCodAzione='CAN' THEN 'ScciSCH06'				-- Cancella
								WHEN @sCodAzione='ANN' THEN 'ScciSCH03'				-- Annulla
								WHEN @sCodAzione='COM' THEN 'ScciSCH07'				-- Completa
								WHEN @sCodAzione='VAL' THEN 'ScciSCH05'				-- Valida
								WHEN @sCodAzione='VIS' THEN 'ScciSCH04'				-- Visualizza								
								WHEN @sCodAzione='REV' THEN 'ScciSCH08'				-- Visualizza	
								ELSE ''
							 END)		
		END	

	----------------------------	
	--		Paziente (Movimento)
	---------------------------	
	IF @sCodEntita='PAZ' 
		BEGIN
			/*
			ScciMPZ01	Nuovo Paziente (Movimento)
			ScciMPZ02	Modifica Paziente (Movimento)
			ScciMPZ03	Cancella Paziente (Movimento)

			*/
			
			SET @sCodEvento=(CASE 
								WHEN @sCodAzione='INS' THEN 'ScciPAZ01'				-- Inserisci
								WHEN @sCodAzione='MOD' THEN 'ScciPAZ02'				-- Modifica
								WHEN @sCodAzione='CAN' THEN 'ScciPAZ03'				-- Cancella
								WHEN @sCodAzione='ANN' THEN ''						-- Annulla
								WHEN @sCodAzione='COM' THEN ''						-- Completa
								WHEN @sCodAzione='VAL' THEN ''						-- Valida
								WHEN @sCodAzione='VIS' THEN ''						-- Visualizza								
								ELSE ''
							 END)		
		END	
				
	----------------------------	
	--		Paziente (Anagrafica)
	---------------------------	
	IF @sCodEntita='ANA' 
		BEGIN
			/*
			ScciMPZ01	Nuovo Paziente (Movimento)
			ScciMPZ02	Modifica Paziente (Movimento)
			ScciMPZ03	Cancella Paziente (Movimento)

			*/
			
			SET @sCodEvento=(CASE 
								WHEN @sCodAzione='INS' THEN 'ScciANA01'				-- Inserisci
								WHEN @sCodAzione='MOD' THEN 'ScciANA02'				-- Modifica
								WHEN @sCodAzione='CAN' THEN 'ScciANA03'				-- Cancella
								WHEN @sCodAzione='ANN' THEN ''						-- Annulla
								WHEN @sCodAzione='COM' THEN ''						-- Completa
								WHEN @sCodAzione='VAL' THEN ''						-- Valida
								WHEN @sCodAzione='VIS' THEN ''						-- Visualizza								
								ELSE ''
							 END)		
		END	
	
	--------------------------------	
	--		Prescrizioni - Tempi
	--------------------------------
	IF @sCodEntita='PRF' 
		BEGIN
			/*
				ScciPRF01	Nuova Prescrizione
				ScciPRF02	Modifica Prescrizione
				ScciPRF03	Annulla Prescrizione
				ScciPRF04	Cancella Prescrizione
				ScciPRF05	Valida Prescrizione
			*/
			
			SET @sCodEvento=(CASE 
								WHEN @sCodAzione='INS' THEN 'ScciPRF01'				-- Inserisci
								WHEN @sCodAzione='MOD' THEN 'ScciPRF02'				-- Modifica
								WHEN @sCodAzione='CAN' THEN 'ScciPRF04'				-- Cancella
								WHEN @sCodAzione='ANN' THEN 'ScciPRF03'				-- Annulla
								WHEN @sCodAzione='COM' THEN ''						-- Completa
								WHEN @sCodAzione='VAL' THEN 'ScciPRF05'				-- Valida
								WHEN @sCodAzione='VIS' THEN ''						-- Visualizza								
								ELSE ''
							 END)		
		END	


	--------------------------------	
	--		Prescrizioni - Tempi
	--------------------------------
	IF @sCodEntita='PRT' 
		BEGIN
			/*
				ScciPRT01	Nuova Prescrizione Testata	
				ScciPRT02	Modifica Prescrizione Testata
				ScciPRT03	Cancella Prescrizione Testata
			*/
			
			SET @sCodEvento=(CASE 
								WHEN @sCodAzione='INS' THEN 'ScciPRT01'				-- Inserisci
								WHEN @sCodAzione='MOD' THEN 'ScciPRT02'				-- Modifica
								WHEN @sCodAzione='CAN' THEN 'ScciPRT03'				-- Cancella															
								ELSE ''
							 END)		
		END	

	----------------------------	
	--		Appuntamenti 
	---------------------------	
	IF @sCodEntita='APP' 
		BEGIN
			/*				
				ScciAPP01	Nuova Appuntamento
				ScciAPP02	Modifica Appuntamento
				ScciAPP03	Annulla Appuntamento
				ScciAPP04	Cancella Appuntamento
			*/
			
			SET @sCodEvento=(CASE 
								WHEN @sCodAzione='INS' THEN 'ScciAPP01'				-- Inserisci
								WHEN @sCodAzione='MOD' THEN 'ScciAPP02'				-- Modifica
								WHEN @sCodAzione='CAN' THEN 'ScciAPP04'				-- Cancella
								WHEN @sCodAzione='ANN' THEN 'ScciAPP03'				-- Annulla
								WHEN @sCodAzione='COM' THEN ''						-- Completa
								WHEN @sCodAzione='VAL' THEN ''						-- Valida
								WHEN @sCodAzione='VIS' THEN ''						-- Visualizza								
								ELSE ''
							 END)		
		END	

	----------------------------	
	--		Agende 
	---------------------------	
	IF @sCodEntita='AGE' 
		BEGIN
			/*				
				ScciAGE01	Nuova Appuntamento in Agenda
				ScciAGE02	Modifica Appuntamento in Agenda
				ScciAGE03	Annulla Appuntamento in Agenda
				ScciAGE04	Cancella Appuntamento in Agenda
			*/
			
			SET @sCodEvento=(CASE 
								WHEN @sCodAzione='INS' THEN 'ScciAGE01'				-- Inserisci
								WHEN @sCodAzione='MOD' THEN 'ScciAGE02'				-- Modifica
								WHEN @sCodAzione='CAN' THEN 'ScciAGE04'				-- Cancella
								WHEN @sCodAzione='ANN' THEN 'ScciAGE03'				-- Annulla
								WHEN @sCodAzione='COM' THEN ''						-- Completa
								WHEN @sCodAzione='VAL' THEN ''						-- Valida
								WHEN @sCodAzione='VIS' THEN ''						-- Visualizza								
								ELSE ''
							 END)		
		END

	----------------------------	
	--		Note Agente
	---------------------------	
	IF @sCodEntita='NTE' 
		BEGIN
			/*				
				ScciNTE01	Nuova Nota in Agenda
				ScciNTE02	Modifica Nota in Agenda
				ScciNTE03	Annulla Nota in Agenda
				ScciNTE04	Cancella Notain Agenda
			*/
			
			SET @sCodEvento=(CASE 
								WHEN @sCodAzione='INS' THEN 'ScciNTE01'				-- Inserisci
								WHEN @sCodAzione='MOD' THEN 'ScciNTE02'				-- Modifica
								WHEN @sCodAzione='CAN' THEN 'ScciNTE04'				-- Cancella
								WHEN @sCodAzione='ANN' THEN 'ScciNTE03'				-- Annulla
								WHEN @sCodAzione='COM' THEN ''						-- Completa
								WHEN @sCodAzione='VAL' THEN ''						-- Valida
								WHEN @sCodAzione='VIS' THEN ''						-- Visualizza								
								ELSE ''
							 END)		
		END

	----------------------------	
	--		Note Generali
	---------------------------	
	IF @sCodEntita='NTG' 
		BEGIN
			/*				
				ScciNTG01	Nuova Nota in Agenda
				ScciNTG02	Modifica Nota in Agenda
				ScciNTG03	Annulla Nota in Agenda
				ScciNTG04	Cancella Notain Agenda
			*/
			
			SET @sCodEvento=(CASE 
								WHEN @sCodAzione='INS' THEN 'ScciNTG01'				-- Inserisci
								WHEN @sCodAzione='MOD' THEN 'ScciNTG02'				-- Modifica
								WHEN @sCodAzione='CAN' THEN 'ScciNTG04'				-- Cancella
								WHEN @sCodAzione='ANN' THEN 'ScciNTG03'				-- Annulla
								WHEN @sCodAzione='COM' THEN ''						-- Completa
								WHEN @sCodAzione='VAL' THEN ''						-- Valida
								WHEN @sCodAzione='VIS' THEN ''						-- Visualizza								
								ELSE ''
							 END)		
		END
				
	----------------------------	
	--		Ordini
	---------------------------	
	IF @sCodEntita='OE' 
		BEGIN
			/*				
				ScciOE01	Nuova Richiesta OE
				ScciOE02	Modifica Richiesta OE
				ScciOE03	Cancella Richiesta OE
				ScciOE04	Valida Richiesta OE
			*/
						
			SET @sCodEvento=(CASE 
								WHEN @sCodAzione='INS' THEN 'ScciOE01'				-- Inserisci
								WHEN @sCodAzione='MOD' THEN 'ScciOE02'				-- Modifica
								WHEN @sCodAzione='CAN' THEN 'ScciOE03'				-- Cancella
								WHEN @sCodAzione='ANN' THEN ''						-- Annulla
								WHEN @sCodAzione='COM' THEN ''						-- Completa
								WHEN @sCodAzione='VAL' THEN 'ScciOE04'				-- Valida
								WHEN @sCodAzione='VIS' THEN ''						-- Visualizza								
								ELSE ''
							 END)		
		END
		
	----------------------------	
	--		Cartella
	---------------------------		
	IF @sCodEntita='CAR' 
		BEGIN
			/*				
				ScciCAR01	Apri Cartella
				ScciCAR02	Chiudi Cartella	
				ScciCAR03	Modifica Cartella
				ScciCAR04	Cancella Cartella
				SccoCAR05	Collega Cartella
			*/
										
			SET @sCodEvento=(CASE 
								WHEN @sCodAzione='INS' THEN 'ScciCAR01'				-- Inserisci
								WHEN @sCodAzione='MOD' THEN 'ScciCAR03'				-- Modifica
								WHEN @sCodAzione='CAN' THEN 'ScciCAR04'				-- Cancella
								WHEN @sCodAzione='ANN' THEN ''						-- Annulla
								WHEN @sCodAzione='COM' THEN 'ScciCAR02'				-- Completa
								WHEN @sCodAzione='VAL' THEN ''						-- Valida
								WHEN @sCodAzione='VIS' THEN ''						-- Visualizza
								WHEN @sCodAzione='COL' THEN 'ScciCAR05'				-- Collega Cartella	
								ELSE ''
							 END)		
		END
	
	----------------------------	
	--		Allegati
	---------------------------	
	
	IF @sCodEntita='ALL' 
		BEGIN
			/*				
				ScciALL01	Nuovo Allegato
				ScciALL02	Modifica Allegato
				ScciALL03	Annulla Allegato
				ScciALL04	Cancella Allegato
		
			*/
							

			SET @sCodEvento=(CASE 
								WHEN @sCodAzione='INS' THEN 'ScciALL01'				-- Inserisci
								WHEN @sCodAzione='MOD' THEN 'ScciALL02'				-- Modifica
								WHEN @sCodAzione='CAN' THEN 'ScciALL04'				-- Cancella
								WHEN @sCodAzione='ANN' THEN 'ScciALL03'				-- Annulla
								WHEN @sCodAzione='COM' THEN ''						-- Completa
								WHEN @sCodAzione='VAL' THEN ''						-- Valida
								WHEN @sCodAzione='VIS' THEN ''						-- Visualizza								
								ELSE ''
							 END)		
		END
		
	----------------------------	
	--		Episodio
	---------------------------	
	
	IF @sCodEntita='EPI' 
		BEGIN
			/*				
				ScciEPI01	Nuovo Trasferimento
				ScciEPI02	Modifica Trasferimento
				ScciEPI03	Annulla Trasferimento
				ScciEPI04	Cancella Trasferimento
		
			*/
							

			SET @sCodEvento=(CASE 
								WHEN @sCodAzione='INS' THEN 'ScciEPI01'				-- Inserisci
								WHEN @sCodAzione='MOD' THEN 'ScciEPI02'				-- Modifica
								WHEN @sCodAzione='CAN' THEN 'ScciEPI04'				-- Cancella
								WHEN @sCodAzione='ANN' THEN 'ScciEPI03'				-- Annulla
								WHEN @sCodAzione='COM' THEN ''						-- Completa
								WHEN @sCodAzione='VAL' THEN ''						-- Valida
								WHEN @sCodAzione='VIS' THEN ''						-- Visualizza								
								ELSE ''
							 END)		
		END

	----------------------------	
	--		Trasferimenti
	---------------------------	
	
	IF @sCodEntita='TRA' 
		BEGIN
			/*				
				ScciTRA01	Nuovo Trasferimento
				ScciTRA02	Modifica Trasferimento
				ScciTRA03	Annulla Trasferimento
				ScciTRA04	Cancella Trasferimento
		
			*/
							

			SET @sCodEvento=(CASE 
								WHEN @sCodAzione='INS' THEN 'ScciTRA01'				-- Inserisci
								WHEN @sCodAzione='MOD' THEN 'ScciTRA02'				-- Modifica
								WHEN @sCodAzione='CAN' THEN 'ScciTRA04'				-- Cancella
								WHEN @sCodAzione='ANN' THEN 'ScciTRA03'				-- Annulla
								WHEN @sCodAzione='COM' THEN ''						-- Completa
								WHEN @sCodAzione='VAL' THEN ''						-- Valida
								WHEN @sCodAzione='VIS' THEN ''						-- Visualizza								
								ELSE ''
							 END)		
		END
	
	----------------------------	
	--		Cartelle In Visione
	---------------------------	
	
	IF @sCodEntita='CIV' 
		BEGIN
			/*				
				ScciCIV01	Nuovo Cartella In Visione
				ScciCIV02	Modifica Cartella In Visione				
				ScciCIV03	Cancella Cartella In Visione
		
			*/
							

			SET @sCodEvento=(CASE 
								WHEN @sCodAzione='INS' THEN 'ScciCIV01'				-- Inserisci
								WHEN @sCodAzione='MOD' THEN 'ScciCIV02'				-- Modifica
								WHEN @sCodAzione='CAN' THEN 'ScciCIV03'				-- Cancella
								WHEN @sCodAzione='ANN' THEN ''						-- Annulla
								WHEN @sCodAzione='COM' THEN ''						-- Completa
								WHEN @sCodAzione='VAL' THEN ''						-- Valida
								WHEN @sCodAzione='VIS' THEN ''						-- Visualizza								
								ELSE ''
							 END)		
		END
	
	----------------------------	
	--		Pazienti Seguiti
	---------------------------		
	IF @sCodEntita='PZS' 
		BEGIN
			/*				
				ScciPZS01	Nuovo Paziente Seguito
				ScciPZS02	Modifica Paziente Seguito
				ScciPZS03	Cancella Paziente Seguito
			*/
										
			SET @sCodEvento=(CASE 
								WHEN @sCodAzione='INS' THEN 'ScciPZS01'				-- Inserisci
								WHEN @sCodAzione='MOD' THEN 'ScciPZS02'				-- Modifica
								WHEN @sCodAzione='CAN' THEN 'ScciPZS03'				-- Cancella
								WHEN @sCodAzione='ANN' THEN ''						-- Annulla
								WHEN @sCodAzione='COM' THEN ''						-- Completa
								WHEN @sCodAzione='VAL' THEN ''						-- Valida
								WHEN @sCodAzione='VIS' THEN ''						-- Visualizza								
								ELSE ''
							 END)		
		END
	
	----------------------------	
	--		Documenti Firmati
	---------------------------		
	IF @sCodEntita='DCF' 
		BEGIN
			/*				
				ScciDCF01	Nuovo Documento Firmato
				
			*/
										
			SET @sCodEvento=(CASE 
								WHEN @sCodAzione='INS' THEN 'ScciDCF01'				-- Inserisci
								WHEN @sCodAzione='MOD' THEN ''						-- Modifica
								WHEN @sCodAzione='CAN' THEN ''						-- Cancella
								WHEN @sCodAzione='ANN' THEN ''						-- Annulla
								WHEN @sCodAzione='COM' THEN ''						-- Completa
								WHEN @sCodAzione='VAL' THEN ''						-- Valida
								WHEN @sCodAzione='VIS' THEN ''						-- Visualizza								
								ELSE ''
							 END)		
		END		
		
	----------------------------	
	--		Allegati Schda
	---------------------------		
	IF @sCodEntita='ALS' 
		BEGIN
			/*				
				ScciALS01	Nuovo Allegato Scheda
				ScciALS02	Modifica Allegato Scheda
				ScciALS03	Cancella Allegato Scheda				
			*/
										
			SET @sCodEvento=(CASE 
								WHEN @sCodAzione='INS' THEN 'ScciALS01'				-- Inserisci
								WHEN @sCodAzione='MOD' THEN 'ScciALS02'				-- Modifica
								WHEN @sCodAzione='CAN' THEN 'ScciALS03'				-- Cancella
								WHEN @sCodAzione='ANN' THEN ''						-- Annulla
								WHEN @sCodAzione='COM' THEN ''						-- Completa
								WHEN @sCodAzione='VAL' THEN ''						-- Valida
								WHEN @sCodAzione='VIS' THEN ''						-- Visualizza								
								ELSE ''
							 END)		
		END		
				

	----------------------------	
	--		Pazienti In Visione
	---------------------------	
	
	IF @sCodEntita='PIV' 
		BEGIN
			/*				
				ScciPIV01	Nuovo Paziente In Visione
				ScciPIV02	Modifica Paziente In Visione				
				ScciPIV03	Cancella Paziente In Visione
		
			*/
							

			SET @sCodEvento=(CASE 
								WHEN @sCodAzione='INS' THEN 'ScciPIV01'				-- Inserisci
								WHEN @sCodAzione='MOD' THEN 'ScciPIV02'				-- Modifica
								WHEN @sCodAzione='CAN' THEN 'ScciPIV03'				-- Cancella
								WHEN @sCodAzione='ANN' THEN ''						-- Annulla
								WHEN @sCodAzione='COM' THEN ''						-- Completa
								WHEN @sCodAzione='VAL' THEN ''						-- Valida
								WHEN @sCodAzione='VIS' THEN ''						-- Visualizza								
								ELSE ''
							 END)		
		END

	----------------------------	
	--		Matilde@Home Login
	---------------------------	
	
	IF @sCodEntita='HLG' 
		BEGIN
			/*		
				ScciHLG01 Nuovo Matilde@Home Login
				ScciHLG02 Modifica Matilde@Home Login
				ScciHLG03 Cancella Matilde@Home Login
			*/
							

			SET @sCodEvento=(CASE 
								WHEN @sCodAzione='INS' THEN 'ScciHLG01'				-- Inserisci
								WHEN @sCodAzione='MOD' THEN 'ScciHLG02'				-- Modifica
								WHEN @sCodAzione='CAN' THEN 'ScciHLG03'				-- Cancella
								WHEN @sCodAzione='ANN' THEN ''						-- Annulla
								WHEN @sCodAzione='COM' THEN ''						-- Completa
								WHEN @sCodAzione='VAL' THEN ''						-- Valida
								WHEN @sCodAzione='VIS' THEN ''						-- Visualizza								
								ELSE ''
							 END)		
		END

	----------------------------	
	--		Matilde@Home Login Contatti
	---------------------------	
	
	IF @sCodEntita='HLC' 
		BEGIN
			/*		
				ScciHLC01 Nuovo Matilde@Home Contatto
				ScciHLC02 Modifica Matilde@Home Contatto
				ScciHLC03 Cancella Matilde@Home Contatto
			*/
							

			SET @sCodEvento=(CASE 
								WHEN @sCodAzione='INS' THEN 'ScciHLC01'				-- Inserisci
								WHEN @sCodAzione='MOD' THEN 'ScciHLC02'				-- Modifica
								WHEN @sCodAzione='CAN' THEN 'ScciHLC03'				-- Cancella
								WHEN @sCodAzione='ANN' THEN ''						-- Annulla
								WHEN @sCodAzione='COM' THEN ''						-- Completa
								WHEN @sCodAzione='VAL' THEN ''						-- Valida
								WHEN @sCodAzione='VIS' THEN ''						-- Visualizza								
								ELSE ''
							 END)		
		END

	----------------------------	
	--		Matilde@Home Login UA
	---------------------------	
	
	IF @sCodEntita='HLC' 
		BEGIN
			/*		
				ScciHLU01 Nuovo Matilde@Home UA
				ScciHLU02 Modifica Matilde@Home UA
				ScciHLU03 Cancella Matilde@Home UA
			*/
							

			SET @sCodEvento=(CASE 
								WHEN @sCodAzione='INS' THEN 'ScciHLU01'				-- Inserisci
								WHEN @sCodAzione='MOD' THEN 'ScciHLU02'				-- Modifica
								WHEN @sCodAzione='CAN' THEN 'ScciHLU03'				-- Cancella
								WHEN @sCodAzione='ANN' THEN ''						-- Annulla
								WHEN @sCodAzione='COM' THEN ''						-- Completa
								WHEN @sCodAzione='VAL' THEN ''						-- Valida
								WHEN @sCodAzione='VIS' THEN ''						-- Visualizza								
								ELSE ''
							 END)		
		END

	----------------------------	
	--		Matilde@Home Accesso
	---------------------------	
	
	IF @sCodEntita='HLA' 
		BEGIN
			/*		
				ScciHLA01 Nuovo Matilde@Home UA
			*/
							

			SET @sCodEvento=(CASE 
								WHEN @sCodAzione='INS' THEN 'ScciHLA01'				-- Inserisci
								WHEN @sCodAzione='MOD' THEN ''						-- Modifica
								WHEN @sCodAzione='CAN' THEN ''						-- Cancella
								WHEN @sCodAzione='ANN' THEN ''						-- Annulla
								WHEN @sCodAzione='COM' THEN ''						-- Completa
								WHEN @sCodAzione='VAL' THEN ''						-- Valida
								WHEN @sCodAzione='VIS' THEN ''						-- Visualizza								
								ELSE ''
							 END)		
		END


	----------------------------	
	--	Consensi
	---------------------------	
	
	IF @sCodEntita='CNS' 
		BEGIN
			/*		
				ScciCNS01 Inserisci Consenso
				ScciCNS02 Modifica Consenso
			*/
							

			SET @sCodEvento=(CASE 
								WHEN @sCodAzione='INS' THEN 'ScciCNS01'				-- Inserisci
								WHEN @sCodAzione='MOD' THEN 'ScciCNS02'				-- Modifica
								WHEN @sCodAzione='CAN' THEN ''						-- Cancella
								WHEN @sCodAzione='ANN' THEN ''						-- Annulla
								WHEN @sCodAzione='COM' THEN ''						-- Completa
								WHEN @sCodAzione='VAL' THEN ''						-- Valida
								WHEN @sCodAzione='VIS' THEN ''						-- Visualizza								
								ELSE ''
							 END)		
		END

	----------------------------	
	--	ADT
	---------------------------	
	
	IF @sCodEntita='ADT' 
		BEGIN
			/*		
				DWHADT002 Spostamento entità pre doppie liste di attesa				
			*/
							
			SET @sCodEvento=(CASE 
								WHEN @sCodAzione='INS' THEN ''						-- Inserisci
								WHEN @sCodAzione='MOD' THEN 'DWHADT002'				-- Modifica
								WHEN @sCodAzione='CAN' THEN ''						-- Cancella
								WHEN @sCodAzione='ANN' THEN ''						-- Annulla
								WHEN @sCodAzione='COM' THEN ''						-- Completa
								WHEN @sCodAzione='VAL' THEN ''						-- Valida
								WHEN @sCodAzione='VIS' THEN ''						-- Visualizza								
								ELSE ''
							 END)		
		END

	----------------------------	
	--	CSG
	---------------------------	
	
	IF @sCodEntita='CSG' 
		BEGIN
			/*		
				ScciCSG01 Inserisci Consegna
				ScciCSG02 Modifica Consegna
				ScciCSG03 Cancella Consegna
				ScciCSG04 Annulla Consegna
			*/
							

			SET @sCodEvento=(CASE 
								WHEN @sCodAzione='INS' THEN 'ScciCSG01'				-- Inserisci
								WHEN @sCodAzione='MOD' THEN 'ScciCSG02'				-- Modifica
								WHEN @sCodAzione='CAN' THEN 'ScciCSG03'				-- Cancella
								WHEN @sCodAzione='ANN' THEN 'ScciCSG04'				-- Annulla
								WHEN @sCodAzione='COM' THEN ''						-- Completa
								WHEN @sCodAzione='VAL' THEN ''						-- Valida
								WHEN @sCodAzione='VIS' THEN ''						-- Visualizza								
								ELSE ''
							 END)		
		END

	------------------------------
	--	CSP - Consegne Paziente --
	------------------------------
	
	IF @sCodEntita='CSP' 
		BEGIN
			/*		
				ScciCSP01 Inserisci Consegna
				ScciCSP02 Modifica Consegna
				ScciCSP04 Annulla Consegna
				ScciCSP05 Visto Consegna
			*/															

			SET @sCodEvento=(CASE 
								WHEN @sCodAzione='INS' THEN 'ScciCSP01'				-- Inserisci
								WHEN @sCodAzione='MOD' THEN 'ScciCSP02'				-- Modifica
								WHEN @sCodAzione='CAN' THEN 'ScciCSP03'				-- Cancella
								WHEN @sCodAzione='ANN' THEN 'ScciCSP04'				-- Annulla
								ELSE ''
							 END)		
		END

	------------------------------------
	--	CSR - Consegne Paziente Ruoli --
	------------------------------------
	
	IF @sCodEntita='CSR'
		BEGIN
			/*		
				ScciCSR01 Inserisci Consegna
				ScciCSR02 Modifica Consegna
				ScciCSR04 Annulla Consegna
				ScciCSR05 Visione Consegna
			*/
							

			SET @sCodEvento=(CASE 
								WHEN @sCodAzione='INS' THEN 'ScciCSR01'				-- Inserisci
								WHEN @sCodAzione='MOD' THEN 'ScciCSR02'				-- Modifica
								WHEN @sCodAzione='CAN' THEN 'ScciCSR03'				-- Cancella
								WHEN @sCodAzione='ANN' THEN 'ScciCSR04'				-- Annulla
								WHEN @sCodAzione='VAL' THEN 'ScciCSR05'				-- Visione
								ELSE ''
							 END)		
		END

	--------------
	-- T_Schede --
	--------------
	IF @sCodEntita='T_Schede' 
		BEGIN
			/*
					ScciMANSCH01	Nuova T_Schede	
					ScciMANSCH02	Modifica T_Schede
					ScciMANSCH03	Cancella T_Schede
			*/
			
			SET @sCodEvento=(CASE 
								WHEN @sCodAzione='INS' THEN 'ScciMANSCH01'			-- Inserisci
								WHEN @sCodAzione='MOD' THEN 'ScciMANSCH02'			-- Modifica
								WHEN @sCodAzione='CAN' THEN 'ScciMANSCH03'			-- Cancella
								ELSE ''
							 END)		
		END

	-------------------
	-- T_SchedePadri --
	-------------------
	IF @sCodEntita='T_SchedePadri' 
		BEGIN
			/*
					ScciMANSCHPADRI02	Modifica T_SchedePadri
			*/
			
			SET @sCodEvento=(CASE 
								WHEN @sCodAzione='INS' THEN 'ScciMANSCHPADRI02'			-- Modifica
								WHEN @sCodAzione='MOD' THEN 'ScciMANSCHPADRI02'			-- Modifica
								WHEN @sCodAzione='CAN' THEN 'ScciMANSCHPADRI02'			-- Modifica
								ELSE ''
							 END)		
		END

	-------------------
	-- T_SchedeCopia --
	-------------------
	IF @sCodEntita='T_SchedeCopia' 
		BEGIN
			/*
					ScciMANSCHCOPIA02	Modifica T_SchedeCopia
			*/
			
			SET @sCodEvento=(CASE 
								WHEN @sCodAzione='INS' THEN 'ScciMANSCHCOPIA02'			-- Modifica
								WHEN @sCodAzione='MOD' THEN 'ScciMANSCHCOPIA02'			-- Modifica
								WHEN @sCodAzione='CAN' THEN 'ScciMANSCHCOPIA02'			-- Modifica
								ELSE ''
							 END)		
		END

	-------------------
	-- T_AssUAEntita --
	-------------------
	IF @sCodEntita='T_AssUAEntita' 
		BEGIN
			/*
					ScciMANSCHUAENTITA02	Modifica T_AssUAEntita
			*/
			
			SET @sCodEvento=(CASE 
								WHEN @sCodAzione='INS' THEN 'ScciMANSCHUAENTITA02'		-- Modifica
								WHEN @sCodAzione='MOD' THEN 'ScciMANSCHUAENTITA02'		-- Modifica
								WHEN @sCodAzione='CAN' THEN 'ScciMANSCHUAENTITA02'		-- Modifica
								ELSE ''
							 END)		
		END

	-------------------
	-- T_AssRuoliAzioni --
	-------------------
	IF @sCodEntita='T_AssRuoliAzioni' 
		BEGIN
			/*
					ScciMANSCHRUOLIAZIONI02	Modifica T_AssRuoliAzioni
			*/
			
			SET @sCodEvento=(CASE 
								WHEN @sCodAzione='INS' THEN 'ScciMANSCHRUOLIAZIONI02'		-- Modifica
								WHEN @sCodAzione='MOD' THEN 'ScciMANSCHRUOLIAZIONI02'		-- Modifica
								WHEN @sCodAzione='CAN' THEN 'ScciMANSCHRUOLIAZIONI02'		-- Modifica
								ELSE ''
							 END)		
		END

	----------------------
	-- T_SchedeVersioni --
	----------------------
	IF @sCodEntita='T_SchedeVersioni' 
		BEGIN
			/*
					ScciMANSCHVER01	Nuova T_Schede	
					ScciMANSCHVER02	Modifica T_Schede
					ScciMANSCHVER03	Cancella T_Schede
			*/
			
			SET @sCodEvento=(CASE 
								WHEN @sCodAzione='INS' THEN 'ScciMANSCHVER01'			-- Inserisci
								WHEN @sCodAzione='MOD' THEN 'ScciMANSCHVER02'			-- Modifica
								WHEN @sCodAzione='CAN' THEN 'ScciMANSCHVER03'			-- Cancella
								ELSE ''
							 END)		
		END
			
	---------------------------
	-- T_TipoEvidenzaClinica --
	---------------------------
	IF @sCodEntita='T_TipoEvidenzaClinica' 
		BEGIN
			/*
					ScciMANTIPOEVC01	Nuovo Tipo Evidenza Clinica	
					ScciMANTIPOEVC02	Modifica Tipo Evidenza Clinica
					ScciMANTIPOEVC03	Cancella Tipo Evidenza Clinica
			*/
			
			SET @sCodEvento=(CASE 
								WHEN @sCodAzione='INS' THEN 'ScciMANTIPOEVC01'			-- Inserisci
								WHEN @sCodAzione='MOD' THEN 'ScciMANTIPOEVC02'			-- Modifica
								WHEN @sCodAzione='CAN' THEN 'ScciMANTIPOEVC03'			-- Cancella
								ELSE ''
							 END)		
		END



	-------------------------------------	
	--		Cartella Ambulatoriale
	-------------------------------------

	IF @sCodEntita='CAC' 
		BEGIN
			/*				
				ScciCAR01	Apri Cartella Ambulatoriale
				ScciCAR02	Chiudi Cartella	 Ambulatoriale
				ScciCAR03	Modifica Cartella Ambulatoriale
				ScciCAR04	Cancella Cartella Ambulatoriale
				
			*/
										
			SET @sCodEvento=(CASE 
								WHEN @sCodAzione='INS' THEN 'ScciCAC01'				-- Inserisci
								WHEN @sCodAzione='MOD' THEN 'ScciCAC03'				-- Modifica
								WHEN @sCodAzione='CAN' THEN 'ScciCAC04'				-- Cancella
								WHEN @sCodAzione='ANN' THEN ''						-- Annulla
								WHEN @sCodAzione='COM' THEN 'ScciCAC02'				-- Completa
								WHEN @sCodAzione='VAL' THEN ''						-- Valida
								WHEN @sCodAzione='VIS' THEN ''						-- Visualizza								
								ELSE ''
							 END)		
		END

	------------------------
	-- T_MovTrasferimenti --
	------------------------
	IF @sCodEntita='T_MovTrasferimenti' 
		BEGIN
			/*
					ScciMANMOVTRAS02	Modifica MovTrasferimenti
			*/			
			SET @sCodEvento=(CASE 
								WHEN @sCodAzione='MOD' THEN 'ScciMANMOVTRAS02'			-- Modifica
								ELSE ''
							 END)		
		END

	-------------------------------------
	-- Inserisci Movimento di Log
	-------------------------------------	
			
	SET @bFlagInserimento=(SELECT ISNULL(Attivo,0)
								FROM SCCILog.dbo.T_Eventi
								WHERE Codice=@sCodEvento )
	SET @bFlagInserimento=ISNULL(@bFlagInserimento,0)								
		
	IF @bFlagInserimento=1
	BEGIN				
		INSERT INTO SCCILog.dbo.T_MovDataLog
			(Data,
			 DataUTC,
			 CodUtente,
			 ComputerName,
			 IpAddress,
			 CodEvento,
			 TipoOperazione,
			 Operazione,
			 LogPrima,
			 LogDopo
			 )
		VALUES
			(GETDATE()		
			 ,GETUTCDATE()
			 ,@sCodUtente
			 ,@sComputerName
			 ,@sIpAddress
			 ,@sCodEvento
			 ,@nTipoOperazione
			 ,@sOperazione
			 ,@xLogPrima
			 ,@xLogDopo
			 )
		RETURN 0
	END	 
		 	 
	RETURN 0
	
END



