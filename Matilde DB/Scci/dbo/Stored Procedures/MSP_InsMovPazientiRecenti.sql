CREATE PROCEDURE [dbo].[MSP_InsMovPazientiRecenti](@xParametri AS XML)
AS
BEGIN
	
			
		DECLARE @sCodUtente AS VARCHAR(100)	
	
	DECLARE @uIDPaziente AS UNIQUEIDENTIFIER		
	
	DECLARE @sCodStatoPazienteRecente AS VARCHAR(20)	
			
		
		DECLARE @sGUID AS VARCHAR(50)
	DECLARE @uGUID AS UNIQUEIDENTIFIER
	DECLARE @sDataTmp AS VARCHAR(20)	
	DECLARE @nQTA AS INTEGER
	
		
		DECLARE @xTimeStamp AS XML

	DECLARE @sMaxRecord AS VARCHAR(20)
	DECLARE @sSQL AS VARCHAR(MAX)						
	

		SET @sCodUtente=(SELECT TOP 1 ValoreParametro.CodUtente.value('.','VARCHAR(100)')
					  FROM @xParametri.nodes('/Parametri/CodUtente') as ValoreParametro(CodUtente))	
	SET @sCodUtente=ISNULL(@sCodUtente,'')

		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDPaziente.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDPaziente') as ValoreParametro(IDPaziente))
	IF 	ISNULL(@sGUID,'') <> '' 
			SET @uIDPaziente=CONVERT(UNIQUEIDENTIFIER,	@sGUID)	

		SET @sCodStatoPazienteRecente=(SELECT TOP 1 ValoreParametro.CodStatoPazienteRecente.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodStatoPazienteRecente') as ValoreParametro(CodStatoPazienteRecente))	
				
	SET @sCodStatoPazienteRecente=ISNULL(@sCodStatoPazienteRecente,'IC')
	
		
	    SET @xTimeStamp=(SELECT TOP 1 ValoreParametro.TS.query('.')
					  FROM @xParametri.nodes('/Parametri/TimeStamp') as ValoreParametro(TS))	
	
								
				
	
		SET @sMaxRecord=(SELECT TOP 1 Valore FROM T_Config WHERE ID=170)
	SET @sMaxRecord=ISNULL(@sMaxRecord,'20')
	
	SET @uGUID=NEWID()

	IF @uIDPaziente IS NOT NULL AND ISNULL(@sCodUtente,'') <> ''
		BEGIN
			BEGIN TRANSACTION		
						DELETE FROM T_MovPazientiRecenti
			WHERE IDPaziente=@uIDPaziente AND
				  CodUtente= @sCodUtente
				  
						DELETE FROM T_MovPazientiRecenti
			WHERE IDPaziente IN 						
						(SELECT IDPazienteVecchio
						 FROM T_PazientiAlias
						 WHERE 
							IDPaziente IN 
								(SELECT IDPaziente
								 FROM T_PazientiAlias
								 WHERE IDPazienteVecchio='' + convert(varchar(50),@uIDPaziente) +''
								)
						)		
				  AND	
				  CodUtente= @sCodUtente
				  
									 				
			IF @@ERROR = 0
			BEGIN
					
										INSERT INTO T_MovPazientiRecenti
						   (ID
						   ,IDPaziente			   
						   ,CodUtente
						   ,CodStatoPazienteRecente			   
						   ,DataInserimento
						   ,DataInserimentoUTC
						   ,DataUltimaModifica
						   ,DataUltimaModificaUTC
							)
					 VALUES
							(@uGUID														   ,@uIDPaziente												   ,@sCodUtente													   ,@sCodStatoPazienteRecente									   ,GetDate()													   ,GetUTCDate()												   ,NULL														   ,NULL															)
			
					
					IF @@ERROR = 0
					BEGIN
						SET @sSQL='
								 DELETE  
										T_MovPazientiRecenti								 
								 WHERE					
										CodUtente= ''' + @sCodUtente +''' 
										AND
										ID NOT IN (			
														  SELECT TOP ' + @sMaxRecord + ' 
															 ID				
														   FROM 
																T_MovPazientiRecenti Q																		
														   WHERE				
																Q.CodUtente= ''' + @sCodUtente +''' 																
														   ORDER BY 
																Q.DataInserimento DESC 
													      )'
						
						PRINT @sSQL
						EXEC (@sSQL)
						IF @@ERROR=0						
							BEGIN							
								SELECT @uGUID AS IDPaziente
								COMMIT TRANSACTION								
							END	
						ELSE 							BEGIN
								ROLLBACK TRANSACTION	
								PRINT 'ERRORE'
								SELECT NULL AS IDPaziente
							END
					END	
				ELSE 					BEGIN						
						ROLLBACK TRANSACTION	
						PRINT 'ERRORE'
						SELECT NULL AS IDPaziente
					END	 	
				END		
			ELSE 				BEGIN
						ROLLBACK TRANSACTION	
						PRINT 'ERRORE'
						SELECT NULL AS IDPaziente
				END			
	END
	ELSE
		SELECT NULL AS IDPaziente
													
	RETURN 0
END