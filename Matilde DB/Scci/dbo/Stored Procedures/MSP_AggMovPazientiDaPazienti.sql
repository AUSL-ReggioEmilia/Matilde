
CREATE PROCEDURE [dbo].[MSP_AggMovPazientiDaPazienti](@xParametri XML)
AS
BEGIN
		
	
											
		DECLARE @uIDEpisodio AS UNIQUEIDENTIFIER	
	DECLARE @uIDPaziente AS UNIQUEIDENTIFIER	

	DECLARE @uIDPazienteVecchio AS UNIQUEIDENTIFIER
	DECLARE @uIDMovPaziente AS UNIQUEIDENTIFIER	
 		

		DECLARE @sGUID AS VARCHAR(50)
	DECLARE @uGUID AS UNIQUEIDENTIFIER
	DECLARE @sDataTmp AS VARCHAR(20)	
	
	    DECLARE @xTimeStampBase AS XML	
	DECLARE @xTimeStamp AS XML	
	DECLARE @xTimeStampOriginale AS XML
		
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
				
	SET @xTimeStampOriginale= @xTimeStamp

	SET @xTimeStamp = @xTimeStampOriginale					
				
				
			
	
	
	SELECT TOP 1 
			@uIDMovPaziente = ID,
			@uIDPazienteVecchio = IDPaziente
	FROM T_MovPazienti WHERE IDEpisodio=@uIDEpisodio
	
		IF @uIDMovPaziente IS NOT NULL
	BEGIN
		SET @xTimeStamp.modify('delete (/TimeStamp/IDEntita)[1]') 			
		SET @xTimeStamp.modify('insert <IDEntita>{sql:variable("@uIDMovPaziente")}</IDEntita> into (/TimeStamp)[1]')

				SET @xTimeStamp.modify('delete (/TimeStamp/IDPaziente)[1]') 						
		SET @xTimeStamp.modify('insert <IDPaziente>{sql:variable("@uIDMovPaziente")}</IDPaziente> into (/TimeStamp)[1]')

				SET @xTimeStamp.modify('delete (/TimeStamp/CodEntita)[1]') 						
		SET @xTimeStamp.modify('insert <CodEntita>PAZ</CodEntita> into (/TimeStamp)[1]')
		
				SET @xTimeStamp.modify('delete (/TimeStamp/CodAzione)[1]') 						
		SET @xTimeStamp.modify('insert <CodAzione>MOD</CodAzione> into (/TimeStamp)[1]')
		
				
				SET @xTimeStampBase=@xTimeStamp

		SET @xTimeStamp=CONVERT(XML,
							'<Parametri>' + CONVERT(varchar(max),@xTimeStamp) +
							'</Parametri>')
																							
		BEGIN TRANSACTION
		
												
			SET @xParLog=CONVERT(XML,'<Parametri><LogPrima></LogPrima><LogDopo></LogDopo></Parametri>')										
			
			SET @xLogPrima=Convert(XML,'<DataSet></DataSet>')														
			
			SET @xTemp=
				(SELECT * FROM 
					(SELECT ID
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
					 WHERE ID=@uIDMovPaziente												) AS [Table]
				FOR XML AUTO, ELEMENTS)

			SET @xLogPrima.modify('insert sql:variable("@xTemp") as first into (/DataSet)[1]')
																		
			SET @xParLog.modify('insert sql:variable("@xLogPrima") as first into (/Parametri/LogPrima)[1]')

																							
				
						UPDATE T_MovPazienti
			SET 						 						 
						  IDPaziente=P.ID
						  ,CodSAC=P.CodSAC
						  ,Cognome=P.Cognome
						  ,Nome=P.Nome
						  ,Sesso=P.Sesso
						  ,DataNascita=P.DataNascita							
						  ,CodiceFiscale=P.CodiceFiscale
						  ,CodComuneNascita=P.CodComuneNascita
						  ,ComuneNascita=P.ComuneNascita
						  ,CodProvinciaNascita=P.CodProvinciaNascita
						  ,ProvinciaNascita=P.ProvinciaNascita
						  ,LocalitaNascita=P.LocalitaNascita
						  ,CAPDomicilio=P.CAPDomicilio
						  ,CodComuneDomicilio=P.CodComuneDomicilio
						  ,ComuneDomicilio=P.ComuneDomicilio
						  ,IndirizzoDomicilio=P.IndirizzoDomicilio
						  ,LocalitaDomicilio=P.LocalitaDomicilio
						  ,CodProvinciaDomicilio=P.CodProvinciaDomicilio
						  ,ProvinciaDomicilio=P.ProvinciaDomicilio
						  ,CodRegioneDomicilio=P.CodRegioneDomicilio
						  ,RegioneDomicilio=P.RegioneDomicilio
						  ,CAPResidenza=P.CAPResidenza
						  ,CodComuneResidenza=P.CodComuneResidenza
						  ,ComuneResidenza=P.ComuneResidenza
						  ,IndirizzoResidenza=P.IndirizzoResidenza
						  ,LocalitaResidenza=P.LocalitaResidenza
						  ,CodProvinciaResidenza=P.CodProvinciaResidenza
						  ,ProvinciaResidenza=P.ProvinciaResidenza
						  ,CodRegioneResidenza=P.CodRegioneResidenza
						  ,RegioneResidenza=P.RegioneResidenza
						  ,Foto=P.Foto
						  ,CodMedicoBase=P.CodMedicoBase
						  ,CodFiscMedicoBase=P.CodFiscMedicoBase
						  ,CognomeNomeMedicoBase=P.CognomeNomeMedicoBase	
						  ,DistrettoMedicoBase=P.DistrettoMedicoBase	
						  ,DataSceltaMedicoBase=P.	DataSceltaMedicoBase						
						  ,ElencoEsenzioni=P.ElencoEsenzioni					 
		FROM					  
			(SELECT						  			  						 
						  ID																  ,CodSAC															  ,Cognome															  ,Nome																  ,Sesso															  ,DataNascita														  ,CodiceFiscale													  ,CodComuneNascita													  ,ComuneNascita													  ,CodProvinciaNascita												  ,ProvinciaNascita													  ,LocalitaNascita													  ,CAPDomicilio														  ,CodComuneDomicilio												  ,ComuneDomicilio													  ,IndirizzoDomicilio												  ,LocalitaDomicilio												  ,CodProvinciaDomicilio											  ,ProvinciaDomicilio												  ,CodRegioneDomicilio												  ,RegioneDomicilio													  ,CAPResidenza														  ,CodComuneResidenza												  ,ComuneResidenza													  ,IndirizzoResidenza												  ,LocalitaResidenza												  ,CodProvinciaResidenza											  ,ProvinciaResidenza												  ,CodRegioneResidenza												  ,RegioneResidenza													  ,Foto																  ,CodMedicoBase													  ,CodFiscMedicoBase												  ,CognomeNomeMedicoBase											  ,DistrettoMedicoBase												  ,DataSceltaMedicoBase												  ,ElencoEsenzioni										FROM T_Pazienti
			WHERE ID=@uIDPaziente			  
			 )	AS P	 		  					  		
		WHERE T_MovPazienti.ID=@uIDMovPaziente
	
					
		IF @@ERROR=0 
			BEGIN
								EXEC MSP_InsMovTimeStamp @xTimeStamp		
			END	
		IF @@ERROR = 0
			BEGIN
				COMMIT TRANSACTION
				
																
				SET @xLogDopo=Convert(XML,'<DataSet></DataSet>')				
				
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
						 WHERE ID=@uIDMovPaziente
						) AS [Table]
					FOR XML AUTO, ELEMENTS)

				SET @xLogDopo.modify('insert sql:variable("@xTemp") as first into (/DataSet)[1]')				
											
				SET @xParLog.modify('insert sql:variable("@xLogDopo") as first into (/Parametri/LogDopo)[1]')
				SET @xParLog.modify('insert sql:variable("@xTimeStampBase") as last into (/Parametri)[1]')
				
				SELECT 		@xParLog	
								
				EXEC MSP_InsMovLog @xParLog

												
				SET @xTimeStamp = @xTimeStampOriginale					
				
				
				SET @xTimeStamp.modify('delete (/TimeStamp/IDEpisodio)[1]') 
				SET @xTimeStamp.modify('insert <IDEpisodio></IDEpisodio> into (/TimeStamp)[1]')

				SET @xTimeStamp.modify('delete (/TimeStamp/IDEntita)[1]') 
				SET @xTimeStamp.modify('insert <IDEntita></IDEntita> into (/TimeStamp)[1]')

				SET @xTimeStamp.modify('delete (/TimeStamp/CodEntita)[1]') 
				SET @xTimeStamp.modify('insert <CodEntita></CodEntita> into (/TimeStamp)[1]')

				SET @xTimeStamp.modify('delete (/TimeStamp/CodAzione)[1]') 
				SET @xTimeStamp.modify('insert <CodAzione></CodAzione> into (/TimeStamp)[1]')

				SET @xTimeStamp.modify('delete (/TimeStamp/CodAzione)[1]') 

				SET @xTimeStamp=CONVERT(XML,
							'<Parametri>' + CONVERT(varchar(max),@xTimeStamp) +
							'</Parametri>')
				
				SET @xTimeStamp.modify('insert <IDPazienteVecchio>{sql:variable("@uIDPazienteVecchio")}</IDPazienteVecchio> as first into (/Parametri)[1]')								
				SET @xTimeStamp.modify('insert <IDPazienteNuovo>{sql:variable("@uIDPaziente")}</IDPazienteNuovo> as first into (/Parametri)[1]')
				SET @xTimeStamp.modify('insert <IDEpisodio>{sql:variable("@uIDEpisodio")}</IDEpisodio> as first into (/Parametri)[1]')
								
								EXEC MSP_AggMovPazientiDaPazienti_Entita @xTimeStamp

				
											
				SELECT @uGUID AS ID
			END	
		ELSE
			BEGIN
				ROLLBACK TRANSACTION	
				SELECT NULL AS ID
			END	 
	END
	RETURN 0
END