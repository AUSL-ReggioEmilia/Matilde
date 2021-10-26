CREATE PROCEDURE [dbo].[MSP_ControlloPazienteDaSAC](@xParametri XML)
AS
BEGIN
		
	
	
		DECLARE @uCodSAC AS UNIQUEIDENTIFIER	
	DECLARE @uIDPaziente AS UNIQUEIDENTIFIER	
	DECLARE @bCreaPaziente as BIT	

		DECLARE @sGUID AS VARCHAR(50)
	DECLARE @sTmp As VARCHAR(1800)
	DECLARE @xPar  AS XML
	DECLARE @xParMovPaz  AS XML
	DECLARE @xTemp  AS XML
	DECLARE @sRisultato AS VARCHAR(MAX)
	
	SET @sRisultato=''
	
	
	
				
		SET @sGUID=(SELECT TOP 1 ValoreParametro.CodSAC.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/CodSAC') as ValoreParametro(CodSAC))
	IF 	ISNULL(@sGUID,'') <> '' SET @uCodSAC=CONVERT(UNIQUEIDENTIFIER,	@sGUID)	
	
		SET @bCreaPaziente = (SELECT TOP 1 ValoreParametro.CreaPaziente.value('.','BIT')
					  FROM @xParametri.nodes('/Parametri/CreaPaziente') as ValoreParametro(CreaPaziente))
	SET @bCreaPaziente = ISNULL(@bCreaPaziente, 1)
	
					
	IF @uCodSAC IS NOT NULL
		BEGIN								
						
			DECLARE curPaz  CURSOR 
			FOR SELECT 
					ID
				FROM T_Pazienti 
				WHERE CodSAC=@uCodSAC
			FOR  READ ONLY 
     
		    OPEN curPaz

			FETCH NEXT FROM curPaz 
			INTO @uIDPaziente

			IF @@FETCH_STATUS=0			
			BEGIN		
								WHILE @@FETCH_STATUS = 0
				BEGIN			
																								
												SET @sRisultato=LTRIM(@sRisultato + ' ' + CONVERT(VARCHAR(50) ,@uIDPaziente))			
													
																		
						SET @xParMovPaz=@xParametri
							SET @xTemp=CONVERT(XML,'<ID>' + CONVERT(varchar(50),@uIDPaziente) + '</ID>')
							SET @xParMovPaz.modify('insert sql:variable("@xTemp") as first into (/Parametri)[1]')									
		
												EXEC MSP_AggPazienti @xParMovPaz	
											
						FETCH NEXT FROM curPaz 
						INTO @uIDPaziente
				  END
			  END
			  ELSE  							
				BEGIN			
																				
					IF @bCreaPaziente = 1					
					BEGIN
																												
												CREATE TABLE #tmpPaziente
							(
								IDPaziente UNIQUEIDENTIFIER
							)

												INSERT #tmpPaziente EXEC MSP_InsPazienti @xParametri

						SET @uIDPaziente=(SELECT TOP 1 IDPaziente FROM #tmpPaziente)
						
												SET @sRisultato=@sRisultato + CONVERT(VARCHAR(50) ,@uIDPaziente)
					END
				END		
					
												SELECT 
					 @sRisultato AS Risultato,						 @uIDPaziente AS IDPaziente							
			CLOSE CurPaz
			DEALLOCATE CurPaz
					 
		END	
	ELSE
		BEGIN
						PRINT 'Parametri errati'			
		END	
								
	RETURN 0
END