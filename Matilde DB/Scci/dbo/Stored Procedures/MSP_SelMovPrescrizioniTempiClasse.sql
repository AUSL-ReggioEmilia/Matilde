CREATE PROCEDURE [dbo].[MSP_SelMovPrescrizioniTempiClasse](@xParametri XML)
AS
BEGIN
	

									
		DECLARE @uIDPrescrizione AS UNIQUEIDENTIFIER
	DECLARE @uIDPrescrizioneTempi AS UNIQUEIDENTIFIER
	
	DECLARE @sCodStatoPrescrizioneTempi AS VARCHAR(20)
	DECLARE @sCodUtenteRilevazione AS VARCHAR(20)
		
	
	DECLARE @sCodRuolo AS VARCHAR(20)
	
									
		DECLARE @sGUID AS VARCHAR(50)
	DECLARE @uGUID AS UNIQUEIDENTIFIER
	
	DECLARE @nTemp AS INTEGER
	
	DECLARE @bInserisci AS  BIT
	DECLARE @bModifica AS  BIT
	DECLARE @bCancella AS  BIT
	DECLARE @bValida AS  BIT	
	DECLARE @bAnnulla AS  BIT	
	DECLARE @bTaskInfermieristici AS BIT
	DECLARE @nTaskErogati AS INTEGER		
	
	    DECLARE @xTimeStampBase AS XML	
	DECLARE @xTimeStamp AS XML	
		
		DECLARE @xTemp AS XML
	DECLARE @xParLog AS XML

			SET @sCodRuolo=(SELECT TOP 1 ValoreParametro.CodRuolo.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/TimeStamp/CodRuolo') as ValoreParametro(CodRuolo))
	SET @sCodRuolo=ISNULL(@sCodRuolo,'')
						  					
						  					
		SET @nTemp=(SELECT COUNT(*) FROM T_AssRuoliModuli
				WHERE CodModulo='Prescr_Inserisci'	
				AND CodRuolo=@sCodRuolo)
	IF @nTemp>=1 
		SET @bInserisci=1
	ELSE
		SET @bInserisci=0	
	
		SET @nTemp=(SELECT COUNT(*) FROM T_AssRuoliModuli
				WHERE CodModulo='Prescr_Modifica'	
				AND CodRuolo=@sCodRuolo)
	IF @nTemp>=1 
		SET @bModifica=1
	ELSE
		SET @bModifica=0	
				
		SET @nTemp=(SELECT COUNT(*) FROM T_AssRuoliModuli
				WHERE CodModulo='Prescr_Cancella'	
				AND CodRuolo=@sCodRuolo)
	IF @nTemp>=1 
		SET @bCancella=1
	ELSE
		SET @bCancella=0	
		
		SET @nTemp=(SELECT COUNT(*) FROM T_AssRuoliModuli
				WHERE CodModulo='Prescr_Valida'	
				AND CodRuolo=@sCodRuolo)
	IF @nTemp>=1 
		SET @bValida=1
	ELSE
		SET @bValida=0	
	
		SET @nTemp=(SELECT COUNT(*) FROM T_AssRuoliModuli
				WHERE CodModulo='Prescr_Annulla'	
				AND CodRuolo=@sCodRuolo)
	IF @nTemp>=1 
		SET @bAnnulla=1
	ELSE
		SET @bAnnulla=0	
		
			SET @nTemp=(SELECT COUNT(*) FROM T_AssRuoliModuli
				WHERE CodModulo='WorkL_Menu'	
				AND CodRuolo=@sCodRuolo)
	IF @nTemp>=1 
		SET @bTaskInfermieristici=1
	ELSE
		SET @bTaskInfermieristici=0 	
		
			
		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDPrescrizione.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDPrescrizione') as ValoreParametro(IDPrescrizione))
	IF 	ISNULL(@sGUID,'') <> '' 
			SET @uIDPrescrizione=CONVERT(UNIQUEIDENTIFIER,	@sGUID)		
	
		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDPrescrizioneTempi.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDPrescrizioneTempi') as ValoreParametro(IDPrescrizioneTempi))
	IF 	ISNULL(@sGUID,'') <> '' 
			SET @uIDPrescrizioneTempi=CONVERT(UNIQUEIDENTIFIER,	@sGUID)				
						
		SET @sCodUtenteRilevazione=(SELECT TOP 1 ValoreParametro.CodUtenteRilevazione.value('.','VARCHAR(100)')
					  FROM @xParametri.nodes('/Parametri/TimeStamp/CodLogin') as ValoreParametro(CodUtenteRilevazione))	
	SET @sCodUtenteRilevazione=ISNULL(@sCodUtenteRilevazione,'')
	
	    SET @xTimeStamp=(SELECT TOP 1 ValoreParametro.TS.query('.')
					  FROM @xParametri.nodes('/Parametri/TimeStamp') as ValoreParametro(TS))
	

					
				
				
							   
		DECLARE @sSQL AS VARCHAR(MAX)
	DECLARE @sWhere AS VARCHAR(Max)
	DECLARE @sTmp AS VARCHAR(Max)

	
			
			
				
				
	IF @uIDPrescrizioneTempi IS NOT NULL
	BEGIN	
				SET @nTaskErogati=(SELECT COUNT(*) FROM T_MovTaskInfermieristici WHERE IDGruppo=CONVERT(VARCHAR(50),@uIDPrescrizioneTempi) AND CodStatoTaskInfermieristico='ER')
		SELECT 
			M.ID, 
			M.IDPrescrizione,
			M.CodStatoPrescrizioneTempi, 
			SP.Descrizione AS DescrStatoPrescrizione, 
			M.CodTipoPrescrizioneTempi, 
			CASE   
				WHEN M.CodStatoPrescrizioneTempi='VA' THEN M.DataUltimaModifica  
				ELSE NULL 
			END AS  DataValidazione, 	
			M.DataOraInizio, 
			M.DataOraFine, 
			M.AlBisogno, 
			M.Durata, 
			M.Continuita, 
			M.PeriodicitaGiorni, 
			M.PeriodicitaOre, 
			M.PeriodicitaMinuti,     
			M.CodUtenteRilevazione AS CodUtente,
			L.Descrizione AS DescrUtente,   
			M.DataEvento,   
			M.Posologia,  
			CodProtocollo,			
			ISNULL(Manuale,0) As  Manuale, 
			TempiManuali,  
			
			CONVERT(INTEGER,@bValida)
				&
				CASE 
					WHEN CodStatoPrescrizioneTempi='IC' THEN 1
					ELSE 0
				END 
				&
				(CASE 
					WHEN ISNULL(MP.CodStatoContinuazione,'AP')='CH' THEN 0
						ELSE 1
					END
				)		
				AS  PermessoDaValidare,						
			  CONVERT(INTEGER,@bModifica) 
				&
				(CASE 
					WHEN CodStatoPrescrizioneTempi ='IC' THEN 1
					ELSE 0
				END)
				&
				(CASE 
				  WHEN ISNULL(MP.CodStatoContinuazione,'AP')='CH' THEN 0
				  ELSE 1
								END
				)						
				AS PermessoModifica,
				CONVERT(INTEGER,@bAnnulla) 
				&
				CASE 
					WHEN CodStatoPrescrizioneTempi='VA' THEN 1
					ELSE 0
				END 
					
				AS PermessoAnnulla,
				
				CONVERT(INTEGER,@bCancella) 
				 & (
					(CASE 
						WHEN CodStatoPrescrizioneTempi ='IC' THEN 1
						ELSE 0
					 END) 
					|
					  (
						CASE 
							WHEN @nTaskErogati=0  THEN 1
							ELSE 0
						END
					   & 
						CASE 
							WHEN CodStatoPrescrizioneTempi IN ('VA','SS') THEN 1
							ELSE 0
						END
					  )	
					)
				AS PermessoCancella,

				CONVERT(INTEGER,@bInserisci) 
				&
				(
					CASE 
						WHEN ISNULL(MP.CodStatoContinuazione,'AP')='CH' THEN 0
						ELSE 1
					END
				)		
				AS PermessoCopia
			FROM  
				T_MovPrescrizioniTempi AS M WITH (NOLOCK)     			 
				INNER JOIN T_MovPrescrizioni AS MP WITH (NOLOCK)     
						ON M.IDPrescrizione=MP.ID    
				LEFT JOIN T_StatoPrescrizioneTempi SP WITH (NOLOCK)    
					ON M.CodStatoPrescrizioneTempi=SP.Codice   			
				LEFT JOIN T_Login L WITH (NOLOCK)    
					ON (M.CodUtenteRilevazione=L.Codice)    		
				
			WHERE
				M.ID=@uIDPrescrizioneTempi
	END
	ELSE
		SELECT NULL						    					
				
	EXEC MSP_InsMovTimeStamp @xTimeStamp			
	
	
	RETURN 0
END