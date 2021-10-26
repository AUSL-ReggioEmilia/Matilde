CREATE PROCEDURE [dbo].[MSP_InsPazientiAlias](@xParametri AS XML)
AS
BEGIN
	

		DECLARE @uCodSAC AS UNIQUEIDENTIFIER
	DECLARE @uCodSACFuso AS UNIQUEIDENTIFIER
	DECLARE @sIDEvento AS VARCHAR(50)
	
	
		DECLARE @uIDPaziente aS UNIQUEIDENTIFIER
	DECLARE @uIDPazienteFuso aS UNIQUEIDENTIFIER
	DECLARE @uIDPazienteNonno aS UNIQUEIDENTIFIER
	
	DECLARE @sGUID AS VARCHAR(Max)
	DECLARE @xPar AS XML
	DECLARE @xTmp AS XML
	DECLARE @sCodUtente AS VARCHAR(100)
	DECLARE @nTmp aS INTEGER
	DECLARE @nMyError aS INTEGER
	
				
		SET @sGUID=(SELECT TOP 1 ValoreParametro.CodSAC.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/CodSAC') as ValoreParametro(CodSAC))
	IF 	ISNULL(@sGUID,'') <> '' SET @uCodSAC=CONVERT(UNIQUEIDENTIFIER,	@sGUID)	
	
		SET @sGUID=(SELECT TOP 1 ValoreParametro.CodSACFuso.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/CodSACFuso') as ValoreParametro(CodSACFuso))
	IF 	ISNULL(@sGUID,'') <> '' SET @uCodSACFuso=CONVERT(UNIQUEIDENTIFIER,	@sGUID)	
	
		SET  @sIDEvento=(SELECT TOP 1 ValoreParametro.IDEvento.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDEvento') as ValoreParametro(IDEvento))

			
		SET @sCodUtente	=(SELECT TOP 1 ValoreParametro.CodUtente.value('.','VARCHAR(100)')
					  FROM @xParametri.nodes('/Parametri/TimeStamp/CodLogin') as ValoreParametro(CodUtente))
	SET @sCodUtente	=ISNULL(@sCodUtente,'')
		
				
		SET @xPar=@xParametri
	SET @xPar.modify('delete (/Parametri/CodSACFuso)[1]')
	SET @xPar.modify('delete (/Parametri/IDEvento)[1]')
	SET @xPar.modify('insert <CreaPaziente>1</CreaPaziente> before (/Parametri/TimeStamp)[1] ')	
	
			EXEC MSP_ControlloPazienteDaSAC @xPar
	
				
	SET @sGUID=CONVERT(VARCHAR(50),@uCodSACFuso)
	
		SET @xPar=@xParametri
	SET @xPar.modify('delete (/Parametri/CodSAC)[1]')
	SET @xPar.modify('delete (/Parametri/CodSACFuso)[1]')
	SET @xPar.modify('delete (/Parametri/IDEvento)[1]')
	
	SET @xTmp=CONVERT(xml, '<CodSAC>' + CONVERT(VARCHAR(50),@uCodSACFuso) + '</CodSAC>')
	
	SET @xPar.modify('insert sql:variable("@xTmp") as first into (/Parametri)[1] ')	
	SET @xPar.modify('insert <CreaPaziente>0</CreaPaziente> before (/Parametri/TimeStamp)[1] ')	
		
	EXEC MSP_ControlloPazienteDaSAC @xPar
	
				SET @uIDPaziente=(SELECT TOP 1 ID FROM T_Pazienti WHERE CodSAC=@uCodSAC)
	SET @uIDPazienteFuso=(SELECT TOP 1 ID FROM T_Pazienti WHERE CodSAC=@uCodSACFuso)
	
	INSERT INTO T_MovPazientiAlias
		(IdPazienteNuovo,
		 CodSacNuovo,
		 IDPazienteVecchio,
		 CodSACVecchio,
		 DataEvento,
		 DataEventoUTC,
		 IDEvento)
	SELECT 
		  @uIdPaziente,							  @uCodSAC,								  @uIdPazienteFuso,						  @uCodSACFuso,							  GETDATE(),							  GETUTCDATE(),							  @sIDEvento						
				
	BEGIN TRAN
	
			SET @nMyError=0
			SET @nTmp=0
			
			
												SET @nTmp=(SELECT COUNT(IDNum) FROM T_PazientiAlias WHERE IDPazienteVecchio=@uIDPazienteFuso)
			IF @nTmp=0 
				BEGIN
										INSERT INTO T_PazientiAlias(IDPazienteVecchio,IDPaziente)
					VALUES (@uIDPazienteFuso,@uIDPaziente)
					
					SET @nMyError=@@ERROR
					IF  @nMyError<>0 
						BEGIN
							ROLLBACK TRAN 
							RETURN -1
						END	
				END	
				BEGIN
										UPDATE T_PazientiAlias
					SET IDPaziente=@uIDPaziente
					WHERE IDPazienteVecchio=@uIDPazienteFuso
					
					SET @nMyError=@@ERROR
					IF  @nMyError<>0 
						BEGIN
							ROLLBACK TRAN 
							PRINT 'ERRORE'
						END		
				END	

												SET @nTmp=(SELECT COUNT(IDNum) FROM T_PazientiAlias WHERE IDPazienteVecchio=@uIDPaziente)
			IF @nTmp=0 
				BEGIN
										INSERT INTO T_PazientiAlias(IDPazienteVecchio,IDPaziente)
					VALUES (@uIDPaziente,@uIDPaziente)
					
					SET @nMyError=@@ERROR
					IF  @nMyError<>0 
						BEGIN
							ROLLBACK TRAN 
							RETURN -1
						END	
				END	
			ELSE
				BEGIN
				
										SET @uIDPazienteNonno=(SELECT TOP 1 IDPaziente FROM T_PazientiAlias WHERE IDPazienteVecchio=@uIDPaziente)
					 					
					UPDATE T_PazientiAlias
					SET IDPaziente=@uIDPaziente
					WHERE IDPaziente=@uIDPazienteNonno	
					
					SET @nMyError=@@ERROR
					IF  @nMyError<>0 
						BEGIN
							ROLLBACK TRAN 
							RETURN -1
						END	
						
										UPDATE T_PazientiAlias
					SET IDPaziente=@uIDPaziente
					WHERE IDPazienteVecchio=@uIDPaziente
					
					SET @nMyError=@@ERROR
					IF  @nMyError<>0 
						BEGIN
							ROLLBACK TRAN 
							RETURN -1
						END	
				END	
						
						UPDATE T_PazientiAlias
				SET IDPaziente=@uIDPaziente
				WHERE IDPaziente=@uIDPazienteFuso
			

															
			UPDATE T_Pazienti
			SET IDPazienteFuso=@uIDPaziente,
				CodSACFuso=@uCodSAC										WHERE ID IN (SELECT IDPazienteVecchio 
						 FROM T_PazientiAlias
						 WHERE IDPaziente=@uIDPaziente
							   AND IDPazienteVecchio <> IDPaziente)


			SET @nMyError=@@ERROR
			IF  @nMyError<>0 
						BEGIN
							ROLLBACK TRAN 
							RETURN -1
						END					
	COMMIT TRAN
		
	
	RETURN 0
	
END