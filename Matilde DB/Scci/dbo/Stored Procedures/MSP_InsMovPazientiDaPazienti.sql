CREATE PROCEDURE [dbo].[MSP_InsMovPazientiDaPazienti](@xParametri XML)
AS
BEGIN
		
	
											
		DECLARE @uIDEpisodio AS UNIQUEIDENTIFIER	
	DECLARE @uIDPaziente AS UNIQUEIDENTIFIER	
 	
		DECLARE @sGUID AS VARCHAR(50)
	DECLARE @uGUID AS UNIQUEIDENTIFIER
	DECLARE @sDataTmp AS VARCHAR(20)	
	
	    DECLARE @xTimeStampBase AS XML	
	DECLARE @xTimeStamp AS XML	
		
		DECLARE @xLogPrima AS XML
	DECLARE @xLogDopo AS XML
	DECLARE @xTemp AS XML
	DECLARE @xParLog AS XML

	
		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDEpisodio.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDEpisodio') as ValoreParametro(IDEpisodio))
	IF 	ISNULL(@sGUID,'') <> '' 
			SET @uIDEpisodio=CONVERT(UNIQUEIDENTIFIER,	@sGUID)		


		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDPaziente.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDPaziente') as ValoreParametro(IDPaziente))
	IF 	ISNULL(@sGUID,'') <> '' 
			SET @uIDPaziente=CONVERT(UNIQUEIDENTIFIER,	@sGUID)		
			
	    SET @xTimeStamp=(SELECT TOP 1 ValoreParametro.TS.query('.')
					  FROM @xParametri.nodes('/Parametri/TimeStamp') as ValoreParametro(TS))
				
					
	SET @uGUID=NEWID()

					SET @xTimeStamp.modify('delete (/TimeStamp/IDEntita)[1]') 			
	SET @xTimeStamp.modify('insert <IDEntita>{sql:variable("@uGUID")}</IDEntita> into (/TimeStamp)[1]')

		SET @xTimeStamp.modify('delete (/TimeStamp/IDPaziente)[1]') 						
	SET @xTimeStamp.modify('insert <IDPaziente>{sql:variable("@uGUID")}</IDPaziente> into (/TimeStamp)[1]')

		SET @xTimeStamp.modify('delete (/TimeStamp/CodEntita)[1]') 						
	SET @xTimeStamp.modify('insert <CodEntita>PAZ</CodEntita> into (/TimeStamp)[1]')
	
		SET @xTimeStamp.modify('delete (/TimeStamp/CodAzione)[1]') 						
	SET @xTimeStamp.modify('insert <CodAzione>INS</CodAzione> into (/TimeStamp)[1]')
	
			
		SET @xTimeStampBase=@xTimeStamp

	SET @xTimeStamp=CONVERT(XML,
						'<Parametri>' + CONVERT(varchar(max),@xTimeStamp) +
						'</Parametri>')
																						
	BEGIN TRANSACTION
				INSERT INTO T_MovPazienti(
					  ID
					  ,IDEpisodio
					  ,IDPaziente
					  ,CodSAC
					  ,Cognome
					  ,Nome
					  ,Sesso
					  ,DataNascita							
					  ,CodiceFiscale
					  ,CodComuneNascita
					  ,ComuneNascita
					  ,CodProvinciaNascita
					  ,ProvinciaNascita
					  ,LocalitaNascita
					  ,CAPDomicilio
					  ,CodComuneDomicilio
					  ,ComuneDomicilio
					  ,IndirizzoDomicilio
					  ,LocalitaDomicilio
					  ,CodProvinciaDomicilio
					  ,ProvinciaDomicilio
					  ,CodRegioneDomicilio
					  ,RegioneDomicilio
					  ,CAPResidenza
					  ,CodComuneResidenza
					  ,ComuneResidenza
					  ,IndirizzoResidenza
					  ,LocalitaResidenza
					  ,CodProvinciaResidenza
					  ,ProvinciaResidenza
					  ,CodRegioneResidenza
					  ,RegioneResidenza
					  ,Foto	
					  ,CodMedicoBase
					  ,CodFiscMedicoBase	
				      ,CognomeNomeMedicoBase	
				      ,DistrettoMedicoBase	
				      ,DataSceltaMedicoBase							
				      ,ElencoEsenzioni
				  )
	SELECT
					   @uGUID														  ,@uIDEpisodio													  ,ID															  ,CodSAC														  ,Cognome														  ,Nome															  ,Sesso														  ,DataNascita													  ,CodiceFiscale												  ,CodComuneNascita												  ,ComuneNascita												  ,CodProvinciaNascita											  ,ProvinciaNascita												  ,LocalitaNascita												  ,CAPDomicilio													  ,CodComuneDomicilio											  ,ComuneDomicilio												  ,IndirizzoDomicilio											  ,LocalitaDomicilio											  ,CodProvinciaDomicilio										  ,ProvinciaDomicilio											  ,CodRegioneDomicilio											  ,RegioneDomicilio												  ,CAPResidenza													  ,CodComuneResidenza											  ,ComuneResidenza												  ,IndirizzoResidenza											  ,LocalitaResidenza											  ,CodProvinciaResidenza										  ,ProvinciaResidenza											  ,CodRegioneResidenza											  ,RegioneResidenza												  ,Foto															  ,CodMedicoBase												  ,CodFiscMedicoBase										      ,CognomeNomeMedicoBase									      ,DistrettoMedicoBase										      ,DataSceltaMedicoBase										      ,ElencoEsenzioni								FROM T_Pazienti
	WHERE ID=@uIDPaziente
		
	IF @@ERROR=0 
		BEGIN
						EXEC MSP_InsMovTimeStamp @xTimeStamp		
		END	
	IF @@ERROR = 0
		BEGIN
			COMMIT TRANSACTION
			
												
			SET @xLogDopo=Convert(XML,'<DataSet></DataSet>')
			SET @xLogPrima=''
			
			SET @xTemp=
				(SELECT  * FROM 
					(SELECT   ID
							  ,IDNum
							  ,IDEpisodio
							  ,IDPaziente
							  ,CodSAC
							  ,Cognome
							  ,Nome
							  ,Sesso
							  ,DataNascita							
							  ,CodiceFiscale
							  ,CodComuneNascita
							  ,ComuneNascita
							  ,CodProvinciaNascita
							  ,ProvinciaNascita
							  ,LocalitaNascita
							  ,CAPDomicilio
							  ,CodComuneDomicilio
							  ,ComuneDomicilio
							  ,IndirizzoDomicilio
							  ,LocalitaDomicilio
							  ,CodProvinciaDomicilio
							  ,ProvinciaDomicilio
							  ,CodRegioneDomicilio
							  ,RegioneDomicilio
							  ,CAPResidenza
							  ,CodComuneResidenza
							  ,ComuneResidenza
							  ,IndirizzoResidenza
							  ,LocalitaResidenza
							  ,CodProvinciaResidenza
							  ,ProvinciaResidenza
							  ,CodRegioneResidenza
							  ,RegioneResidenza
							  ,CodMedicoBase
							  ,CodFiscMedicoBase	
							  ,CognomeNomeMedicoBase	
							  ,DistrettoMedicoBase	
							  ,DataSceltaMedicoBase							
							  ,ElencoEsenzioni	
					  					   FROM T_MovPazienti
					 WHERE ID=@uGUID
					) AS [Table]
				FOR XML AUTO, ELEMENTS)

			SET @xLogDopo.modify('insert sql:variable("@xTemp") as first into (/DataSet)[1]')				
			
			SET @xParLog=CONVERT(XML,'<Parametri><LogDopo/></Parametri>')			
			SET @xParLog.modify('insert sql:variable("@xLogDopo") as first into (/Parametri/LogDopo)[1]')
			SET @xParLog.modify('insert sql:variable("@xTimeStampBase") as last into (/Parametri)[1]')
						
			EXEC MSP_InsMovLog @xParLog
									
			SELECT @uGUID AS ID
		END	
	ELSE
		BEGIN
			ROLLBACK TRANSACTION	
			SELECT NULL AS ID
		END	 
	
	RETURN 0
END