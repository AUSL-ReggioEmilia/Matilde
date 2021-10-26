CREATE PROCEDURE [dbo].[MSP_ControlloEvidenzaClinicaCasiParticolari](@xParametri XML)
AS
BEGIN
		
	
	
		DECLARE @uIDEvidenzaClinica AS UNIQUEIDENTIFIER
	
		DECLARE @sGUID AS VARCHAR(50)	
		
		DECLARE @xLogPrima AS XML
	DECLARE @xLogDopo AS XML
	DECLARE @xTemp AS XML
	DECLARE @xParLog AS XML
	
	    DECLARE @xTimeStampBase AS XML	
	DECLARE @xTimeStamp AS XML	
	
	DECLARE @CodTipoEvidenzaClinica AS VARCHAR(20)			
	
			
		SET @uIDEvidenzaClinica=(SELECT TOP 1 ValoreParametro.IDEvidenzaClinica.value('.','UNIQUEIDENTIFIER')
					  FROM @xParametri.nodes('/Parametri/IDEvidenzaClinica') as ValoreParametro(IDEvidenzaClinica))	
	
	    SET @xTimeStamp=(SELECT TOP 1 ValoreParametro.TS.query('.')
					  FROM @xParametri.nodes('/Parametri/TimeStamp') as ValoreParametro(TS))
	
	
				
	
							
	IF @uIDEvidenzaClinica IS NOT NULL 
		BEGIN										
												
			SELECT @CodTipoEvidenzaClinica=(SELECT TOP 1 CodTipoEvidenzaClinica FROM T_MovEvidenzaClinica WHERE ID=@uIDEvidenzaClinica)
			
			IF ISNULL(@CodTipoEvidenzaClinica,'')='SO'
			BEGIN
																				
																				
					SET @xTimeStamp.modify('delete (/TimeStamp/IDEntita)[1]') 			
					SET @xTimeStamp.modify('insert <IDEntita>{sql:variable("@uIDEvidenzaClinica")}</IDEntita> into (/TimeStamp)[1]')

					SET @xTimeStamp.modify('delete (/TimeStamp/CodEntita)[1]') 			
					SET @xTimeStamp.modify('insert <CodEntita>EVC</CodEntita> into (/TimeStamp)[1]')

					SET @xTimeStamp.modify('delete (/TimeStamp/CodAzione)[1]') 			
					SET @xTimeStamp.modify('insert <CodAzione>INSMAT</CodAzione> into (/TimeStamp)[1]')


										SET @xTimeStampBase=@xTimeStamp
					
					SET @xTimeStamp=CONVERT(XML,
										'<Parametri>' + CONVERT(varchar(max),@xTimeStamp) +
										'</Parametri>')
								
																					
					SET @xLogDopo=Convert(XML,'<DataSet></DataSet>')
					SET @xLogPrima=''
					
					SET @xTemp=
						(SELECT * FROM 
							(SELECT ID
								  ,IDNum
								  ,IDEpisodio
								  ,IDTrasferimento
								  ,CodTipoEvidenzaClinica
								  ,CodStatoEvidenzaClinica
								  ,CodStatoEvidenzaClinicaVisione
								  ,IDRefertoDWH
								  ,NumeroRefertoDWH
								  ,DataEvento
								  ,DataEventoUTC
								  ,DataEventoDWH
								  ,DataEventoDWHUTC
								  								  ,CodUtenteInserimento
								  ,DataInserimento
								  ,DataInserimentoUTC
								  ,CodUtenteVisione
								  ,CodUtenteUltimaModifica
								  ,DataUltimaModifica
								  ,DataUltimaModificaUTC 
								  ,DataVisione
								  ,DataVisioneUTC
								  ,Anteprima
							FROM T_MovEvidenzaClinica
							 WHERE ID=@uIDEvidenzaClinica
							) AS [Table]
						FOR XML AUTO, ELEMENTS)

					SET @xLogDopo.modify('insert sql:variable("@xTemp") as first into (/DataSet)[1]')
			
										
					SET @xParLog=CONVERT(XML,'<Parametri><LogDopo/></Parametri>')			
					SET @xParLog.modify('insert sql:variable("@xLogDopo") as first into (/Parametri/LogDopo)[1]')
					SET @xParLog.modify('insert sql:variable("@xTimeStampBase") as last into (/Parametri)[1]')
					
										EXEC MSP_InsMovLog @xParLog
				
			END
		END	
	
	RETURN 0
END