CREATE PROCEDURE [dbo].[MSP_InfoMovLock](@xParametri AS XML)
AS
BEGIN
	

	DECLARE @sCodLogin		VARCHAR(100)
	DECLARE @sCodRuolo		VARCHAR(20)
	DECLARE @sNomePC		VARCHAR(500)
	DECLARE @sIndirizzoIP	VARCHAR(50)
	DECLARE @sCodEntita		VARCHAR(20)
	DECLARE @uIDEntita		UNIQUEIDENTIFIER
	DECLARE @sCodAzione		VARCHAR(20)
	DECLARE @sCodAzioneLock  VARCHAR(20)
	
	DECLARE @sIDEntita	VARCHAR(50)
	DECLARE @sGUID AS VARCHAR(50)
	DECLARE @uGUID AS UNIQUEIDENTIFIER
	
	DECLARE @dtDataLock AS DATETIME
	DECLARE @dtDataLockUTC AS DATETIME

	DECLARE @bRigaBloccata BIT
	DECLARE @sCodLoginBlocco	VARCHAR(100)
	DECLARE @bEsito BIT
	
		DECLARE @sCodScheda	VARCHAR(20)
	DECLARE @nNumScheda	INTEGER	
	DECLARE @sCodEntitaScheda VARCHAR(20)
	DECLARE @uIDSchedaSuccessiva	UNIQUEIDENTIFIER
	DECLARE @uIDSchedaPrecedente	UNIQUEIDENTIFIER
	DECLARE @uIDEntitaScheda		UNIQUEIDENTIFIER
	DECLARE @uIDSchedaPadre			UNIQUEIDENTIFIER 
	
		SET @sCodAzioneLock=(SELECT TOP 1 ValoreParametro.CodLogin.value('.','VARCHAR(100)')
					  FROM @xParametri.nodes('/Parametri/CodAzioneLock') as ValoreParametro(CodLogin))	
	SET @sCodAzioneLock=ISNULL(@sCodAzioneLock,'')
			
		SET @sCodLogin=(SELECT TOP 1 ValoreParametro.CodLogin.value('.','VARCHAR(100)')
					  FROM @xParametri.nodes('/Parametri/TimeStamp/CodLogin') as ValoreParametro(CodLogin))	
	SET @sCodLogin=ISNULL(@sCodLogin,'')
	
		SET @sCodRuolo=(SELECT TOP 1 ValoreParametro.CodRuolo.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/TimeStamp/CodRuolo') as ValoreParametro(CodRuolo))	
	SET @sCodRuolo=ISNULL(@sCodRuolo,'')
	
		SET @sNomePC=(SELECT TOP 1 ValoreParametro.NomePC.value('.','VARCHAR(500)')
					  FROM @xParametri.nodes('/Parametri/TimeStamp/NomePC') as ValoreParametro(NomePC))	
	SET @sNomePC=ISNULL(@sNomePC,'')
	
		SET @sIndirizzoIP=(SELECT TOP 1 ValoreParametro.IndirizzoIP.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/TimeStamp/IndirizzoIP') as ValoreParametro(IndirizzoIP))	
	SET @sIndirizzoIP=ISNULL(@sIndirizzoIP,'')
	
		SET @sCodEntita=(SELECT TOP 1 ValoreParametro.CodEntita.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/TimeStamp/CodEntita') as ValoreParametro(CodEntita))	
	SET @sCodEntita=ISNULL(@sCodEntita,'')
		
		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDEntita.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/TimeStamp/IDEntita') as ValoreParametro(IDEntita))
	IF 	ISNULL(@sGUID,'') <> '' SET @uIDEntita=CONVERT(UNIQUEIDENTIFIER,	@sGUID)	
	
		SET	@sCodAzione=(SELECT TOP 1 ValoreParametro.CodAzione.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/TimeStamp/CodAzione') as ValoreParametro(CodAzione))	
	SET @sCodAzione=ISNULL(@sCodAzione,'')
	
				
				
	SELECT 
		@sCodScheda=M.CodScheda,
		@nNumScheda=M.Numero,
		@uIDEntitaScheda=M.IDEntita,
		@sCodEntitaScheda=M.CodEntita,
		@uIDSchedaPadre = M.IDSchedaPadre
	FROM T_MovSchede M
	WHERE M.ID=@uIDEntita
	
			
				IF @sCodAzioneLock='SUCCESSIVA'	
	BEGIN
					
		SET @sCodScheda=ISNULL(@sCodScheda,'')	
		
		SET @uIDSchedaSuccessiva=(SELECT TOP 1 ID 
								 FROM T_MovSchede	
								 WHERE CodEntita=@sCodEntitaScheda AND													   IDEntita=@uIDEntitaScheda AND												   Storicizzata=0 AND															   CodStatoScheda <> 'CA' AND														   CodScheda=@sCodScheda AND													   Numero > @nNumScheda
								 ORDER BY Numero ASC  
								 )	   

		IF @uIDSchedaPadre IS NULL
		BEGIN			
						SET @uIDSchedaSuccessiva=(SELECT TOP 1 ID 
							FROM T_MovSchede	
							WHERE CodEntita=@sCodEntitaScheda AND										IDEntita=@uIDEntitaScheda AND											Storicizzata=0 AND														CodStatoScheda <> 'CA' AND												CodScheda=@sCodScheda AND												Numero > @nNumScheda AND
								IDSchedaPadre IS NULL
							ORDER BY Numero ASC  
							)	     
		END
		ELSE
		BEGIN
						SET @uIDSchedaSuccessiva=(SELECT TOP 1 ID 
							FROM T_MovSchede	
							WHERE CodEntita=@sCodEntitaScheda AND										IDEntita=@uIDEntitaScheda AND											Storicizzata=0 AND														CodStatoScheda <> 'CA' AND												CodScheda=@sCodScheda AND												Numero > @nNumScheda AND				
								IDSchedaPadre = @uIDSchedaPadre
							ORDER BY Numero ASC  
							)	    
		END

		IF @uIDSchedaSuccessiva	IS NOT NULL 
			SELECT 	@uIDSchedaSuccessiva AS IDSchedaTrovata					ELSE
			SELECT 	@uIDEntita AS IDSchedaTrovata									  
	END
	ELSE
								IF @sCodAzioneLock='PRECEDENTE'	
		BEGIN			

						
			SET @sCodScheda=ISNULL(@sCodScheda,'')	
			
			IF @uIDSchedaPadre IS NULL
			BEGIN
				
				SET @uIDSchedaPrecedente=(
					SELECT TOP 1 ID 
					FROM T_MovSchede	
					WHERE CodEntita=@sCodEntitaScheda AND													   IDEntita=@uIDEntitaScheda AND													   Storicizzata=0 AND																   CodStatoScheda <> 'CA' AND														   CodScheda=@sCodScheda AND														   Numero < @nNumScheda AND
							IDSchedaPadre IS NULL
						ORDER BY Numero  DESC 
						)	   
			END
			ELSE
			BEGIN
								
				SET @uIDSchedaPrecedente=(SELECT TOP 1 ID 
									FROM T_MovSchede	
									WHERE CodEntita=@sCodEntitaScheda AND													IDEntita=@uIDEntitaScheda AND													Storicizzata=0 AND																CodStatoScheda <> 'CA' AND														CodScheda=@sCodScheda AND														Numero < @nNumScheda AND
										IDSchedaPadre = @uIDSchedaPadre
									ORDER BY Numero  DESC 
									)	  
			END

			IF @uIDSchedaPrecedente	IS NOT NULL 
				SELECT 	@uIDSchedaPrecedente AS IDSchedaTrovata						ELSE
				SELECT 	@uIDEntita AS IDSchedaTrovata								  
		END
		ELSE	
												IF @sCodAzioneLock='INFO'
			BEGIN
				IF @sCodEntita='SCH'
				BEGIN
										SELECT 
						'SCHEDA ' + 
							CASE	
								WHEN LEN(ISNULL(S.Descrizione,'')) > 45 THEN LEFT(ISNULL(S.Descrizione,''),43) +'..'
								ELSE ISNULL(S.Descrizione,'')
							END  
						As DescrScheda,
						M.Numero,
						dbo.MF_SelNumerositaSchedaParametro('QtaSchedeTotali',M.ID,NULL,NULL,NULL,NULL) AS QtaSchedeTotali,
						dbo.MF_SelNumerositaSchedaParametro('QtaSchedeAttive',M.ID,NULL,NULL,NULL,NULL) AS QtaSchedeAttive,						
						ISNULL(UM.Descrizione,ISNULL(UR.Descrizione,'')) AS DescrUtenteModifica,
						ISNULL(M.DataUltimaModifica,M.DataCreazione) AS DataUltimaModifica,
						UL.Descrizione as DescrUtenteLock,
						L.Data AS DataLock,
						L.NomePC AS NomePCLock
						
						FROM T_MovSchede AS M
							LEFT JOIN T_Schede S
								ON M.CodScheda=S.Codice
							LEFT JOIN T_Login AS  UR
								ON M.CodUtenteRilevazione=UR.Codice
							LEFT JOIN T_Login AS UM
								ON M.CodUtenteUltimaModifica=UM.Codice
							LEFT JOIN T_MovLock AS L
								ON  (L.CodEntita='SCH' AND
									L.IDEntita=M.ID)
							LEFT JOIN T_Login UL
								ON L.CodLogin=UL.Codice		
							
					WHERE M.ID=@uIDEntita
				END 
			END
			ELSE
																IF @sCodAzioneLock='UNLOCKALL'	
				BEGIN
										DELETE FROM T_MovLock
					WHERE	
						CodLogin=@sCodLogin AND
						NomePC=@sNomePC
										
					SET @bEsito=1
					SELECT 	CONVERT(INTEGER,@bEsito) AS Esito	
				END
					ELSE
																												BEGIN
																SET @bRigaBloccata=0
								
								SET @bRigaBloccata=(SELECT	TOP 1		
																1				
															FROM 
																T_MovLock
															WHERE			
																CodEntita=@sCodEntita AND
																IDEntita = @uIDEntita
														)			
								SET @bRigaBloccata=ISNULL(@bRigaBloccata,0)
								
								IF @bRigaBloccata=1		
																		SET @sCodLoginBlocco=(SELECT TOP 1		
																	CodLogin			
																	
																FROM 
																	T_MovLock
																WHERE			
																	CodEntita=@sCodEntita AND
																	IDEntita = @uIDEntita
															)		

								SET @sCodLoginBlocco=ISNULL(@sCodLoginBlocco,'')								
								
																SET @bEsito=0
								
																																IF @bRigaBloccata=0 		
									BEGIN
									 IF @sCodAzioneLock='LOCK'
										BEGIN
																						
																						SET @uGUID=NEWID()
											SET @dtDataLock=GETDATE()
											SET @dtDataLockUTC=GETUTCDATE()
											
																						INSERT INTO T_MovLock(
														ID
													  ,Data
													  ,DataUTC
													  ,CodLogin
													  ,CodRuolo
													  ,NomePC
													  ,IndirizzoIP
													  ,CodEntita
													  ,IDEntita
													  ,CodAzione)
											VALUES
													(
														@uGUID																  ,@dtDataLock															  ,@dtDataLockUTC														  ,@sCodLogin															  ,@sCodRuolo															  ,@sNomePC																  ,@sIndirizzoIP														  ,@sCodEntita															  ,@uIDEntita															  ,@sCodAzione															)	
											SET @bEsito=@@ROWCOUNT		
										END				
									ELSE		
									 IF @sCodAzioneLock='UNLOCK' 
										BEGIN
																						SET @bEsito=1
										END		  			
								END	
								ELSE
									BEGIN
																																								
										 IF @sCodAzioneLock='UNLOCK'																		 BEGIN									
																								IF @sCodLoginBlocco=@sCodLogin 
													BEGIN
																												DELETE FROM T_MovLock
														WHERE	CodEntita=@sCodEntita
																AND IDEntita=@uIDEntita
															
														SET @bEsito=1
													END							 
											 END
											  ELSE
												 IF @sCodAzioneLock='LOCK'																				
													BEGIN																
														IF @sCodLoginBlocco=@sCodLogin 									
																SET @bEsito=1									
													END								
																	
										END 
									
									SELECT 	CONVERT(INTEGER,@bEsito) AS Esito
							END
		
	RETURN 0
END