CREATE PROCEDURE [dbo].[MSP_AggMovNoteGruppo](@xParametri XML)
AS
BEGIN
		
	
											
		DECLARE @sIDGruppo AS UNIQUEIDENTIFIER	
	DECLARE @sCodAzioneTask AS VARCHAR(20)
	
		DECLARE @sGUID AS VARCHAR(50)
	DECLARE @uIDNota AS UNIQUEIDENTIFIER	
	DECLARE @xTmpPar AS XML	
	DECLARE @sCodStatoNota AS VARCHAR(20)
	
	
	    DECLARE @xTimeStampBase AS XML	
	DECLARE @xTimeStamp AS XML	

		IF @xParametri.exist('/Parametri/TimeStamp/CodAzione')=1
		BEGIN
			SET @sCodAzioneTask=(SELECT TOP 1 ValoreParametro.CodAzioneTask.value('.','VARCHAR(20)')
							  FROM @xParametri.nodes('/Parametri/TimeStamp/CodAzione') as ValoreParametro(CodAzioneTask))		
			SET @sCodAzioneTask=ISNULL(@sCodAzioneTask,'')
		END
			
								
		IF @xParametri.exist('/Parametri/IDGruppo')=1
		BEGIN
			SET @sGUID=(SELECT TOP 1 ValoreParametro.IDGruppo.value('.','VARCHAR(50)')
							  FROM @xParametri.nodes('/Parametri/IDGruppo') as ValoreParametro(IDGruppo))		
					END
	
		SET @xTimeStamp=(SELECT TOP 1 ValoreParametro.TS.query('.')
	  FROM @xParametri.nodes('/Parametri/TimeStamp') as ValoreParametro(TS))
					
				
	SET @sCodStatoNota=''
	SET @sCodStatoNota=CASE 
							WHEN @sCodAzioneTask='ANN' THEN 'AN'
							WHEN @sCodAzioneTask='CAN' THEN 'CA'
							ELSE ''
						END
	
	IF 	@sCodStatoNota <> '' 
		BEGIN					
		
					PRINT @sCodStatoNota
					
				DECLARE cur CURSOR READ_ONLY FOR 
					SELECT ID
					FROM T_MovNoteAgende
					WHERE ISNULL(CodStatoNota,'') IN ('PR') AND						
						  IDGruppo=@sGUID
						
				OPEN cur

				FETCH NEXT FROM cur 
					INTO @uIDNota
					
				WHILE @@FETCH_STATUS = 0
				BEGIN	
						SET @xTimeStamp.modify('delete (/TimeStamp/IDEntita)[1]') 			
						SET @xTimeStamp.modify('insert <IDEntita>{sql:variable("@uIDNota")}</IDEntita> into (/TimeStamp)[1]')
						
						SET @xTmpPar=CONVERT(xml,'<Parametri></Parametri>')
						SET @xTmpPar.modify('insert <IDNotaAgenda>{sql:variable("@uIDNota")}</IDNotaAgenda> into (/Parametri)[1]')
						SET @xTmpPar.modify('insert <CodStatoNota>{sql:variable("@sCodStatoNota")}</CodStatoNota> into (/Parametri)[1]')
						SET @xTmpPar.modify('insert sql:variable("@xTimeStamp") as last into (/Parametri)[1]')

						SELECT 	@xTmpPar						
						EXEC MSP_AggMovNoteAgende @xTmpPar
								
						
					FETCH NEXT FROM cur 
					INTO @uIDNota
				END			        				
				
				CLOSE cur
				DEALLOCATE cur
		END
	
	RETURN 0
END