CREATE PROCEDURE [dbo].[MSP_InsMovPazienti](@xParametri XML)
AS
BEGIN
		
	
				

								
		DECLARE @uIDEpisodio AS UNIQUEIDENTIFIER	
	DECLARE @uIDPaziente AS UNIQUEIDENTIFIER	
 	DECLARE @uCodSAC	AS UNIQUEIDENTIFIER	
	DECLARE @sCognome	AS VARCHAR(255)			 
	DECLARE @sNome		AS VARCHAR(255)			  
	DECLARE @sSesso		AS VARCHAR(1)			  			  
	DECLARE @dDataNascita	AS DateTime
	DECLARE @sCodiceFiscale		AS VARCHAR(20)			  	  
	DECLARE @sCodComuneNascita	AS VARCHAR(10)			  	  			
	DECLARE @sComuneNascita	AS VARCHAR(255)			  
	DECLARE @sCodProvinciaNascita	AS VARCHAR(10)			  			
	DECLARE @sProvinciaNascita	AS VARCHAR(50)	
	DECLARE @sLocalitaNascita	AS VARCHAR(255)			  
	DECLARE @sCAPDomicilio	AS VARCHAR(10)
	DECLARE @sCodComuneDomicilio	AS VARCHAR(10)
	DECLARE @sComuneDomicilio AS VARCHAR(255)
	DECLARE @sIndirizzoDomicilio AS VARCHAR(255)
	DECLARE @sLocalitaDomicilio	AS VARCHAR(255)
	DECLARE @sCodProvinciaDomicilio	AS VARCHAR(10)
	DECLARE @sProvinciaDomicilio	AS VARCHAR(50)
	DECLARE @sCodRegioneDomicilio	AS VARCHAR(10)		
	DECLARE @sRegioneDomicilio	AS VARCHAR(255)			
	DECLARE @sCAPResidenza	AS VARCHAR(10)				  
	DECLARE @sCodComuneResidenza AS VARCHAR(10)		
	DECLARE @sComuneResidenza AS VARCHAR(255)
	DECLARE @sIndirizzoResidenza AS VARCHAR(255)
	DECLARE @sLocalitaResidenza AS VARCHAR(255)			  
	DECLARE @sCodProvinciaResidenza AS VARCHAR(10)			  			  
	DECLARE @sProvinciaResidenza AS VARCHAR(50)			  
	DECLARE @sCodRegioneResidenza AS VARCHAR(10)
	DECLARE @sRegioneResidenza AS VARCHAR(50)	
	DECLARE @txtFoto AS VARCHAR(MAX)				  
	DECLARE @binFoto AS VARBINARY(MAX)	
	
		DECLARE @sCodMedicoBase	VARCHAR(50)	
	DECLARE @sCodFiscMedicoBase	VARCHAR(20)	
	DECLARE @sCognomeNomeMedicoBase	VARCHAR(255)	
	DECLARE @sDistrettoMedicoBase	VARCHAR(255)	
	DECLARE @dDataSceltaMedicoBase	datetime
	DECLARE @sElencoEsenzioni	VARCHAR(MAX)
	DECLARE @dDataDecesso	datetime

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

		SET @sGUID=(SELECT TOP 1 ValoreParametro.CodSAC.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/CodSAC') as ValoreParametro(CodSAC))
	IF 	ISNULL(@sGUID,'') <> '' 
			SET @uCodSAC=CONVERT(UNIQUEIDENTIFIER,	@sGUID)		
			
		SET @sCognome=(SELECT TOP 1 ValoreParametro.Cognome.value('.','VARCHAR(255)')
					  FROM @xParametri.nodes('/Parametri/Cognome') as ValoreParametro(Cognome))		
	SET @sCognome=dbo.MF_TogliApici(@sCognome)
	
		SET @sNome=(SELECT TOP 1 ValoreParametro.Nome.value('.','VARCHAR(255)')
					  FROM @xParametri.nodes('/Parametri/Nome') as ValoreParametro(Nome))	
	SET @sNome=dbo.MF_TogliApici(@sNome)
		
		SET @sSesso=(SELECT TOP 1 ValoreParametro.Sesso.value('.','VARCHAR(1)')
					  FROM @xParametri.nodes('/Parametri/Sesso') as ValoreParametro(Sesso))	
	
		SET @sDataTmp=(SELECT TOP 1 ValoreParametro.DataNascita.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/DataNascita') as ValoreParametro(DataNascita))					  
	SET @sDataTmp=ISNULL(@sDataTmp,'')	

	IF @sDataTmp<> '' 
		BEGIN			
						SET @sDataTmp= SUBSTRING(@sDataTmp,4,2) + '-' 
						+ LEFT(@sDataTmp,2) + '-' +
						+ SUBSTRING(@sDataTmp,7,4)							IF ISDATE(@sDataTmp)=1
				SET	@dDataNascita=CONVERT(DATETIME,@sDataTmp,120)						
			ELSE
				SET	@dDataNascita =NULL			
		END
		
		SET @sCodiceFiscale=(SELECT TOP 1 ValoreParametro.CodiceFiscale.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodiceFiscale') as ValoreParametro(CodiceFiscale))		
					  
		SET @sCodComuneNascita=(SELECT TOP 1 ValoreParametro.CodComuneNascita.value('.','VARCHAR(10)')
					  FROM @xParametri.nodes('/Parametri/CodComuneNascita') as ValoreParametro(CodComuneNascita))			
		SET @sComuneNascita=(SELECT TOP 1 ValoreParametro.ComuneNascita.value('.','VARCHAR(255)')
				  FROM @xParametri.nodes('/Parametri/ComuneNascita') as ValoreParametro(ComuneNascita))
	
	SET @sComuneNascita=dbo.MF_TogliApici(@sComuneNascita)				  		  	  			
	
		SET @sCodProvinciaNascita=(SELECT TOP 1 ValoreParametro.CodProvinciaNascita.value('.','VARCHAR(10)')
					  FROM @xParametri.nodes('/Parametri/CodProvinciaNascita') as ValoreParametro(CodProvinciaNascita))	  
					  
		SET @sProvinciaNascita=(SELECT TOP 1 ValoreParametro.ProvinciaNascita.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/ProvinciaNascita') as ValoreParametro(ProvinciaNascita))	  
					  				  
	SET @sProvinciaNascita=dbo.MF_TogliApici(@sProvinciaNascita)
	
		SET @sLocalitaNascita=(SELECT TOP 1 ValoreParametro.LocalitaNascita.value('.','VARCHAR(255)')
					  FROM @xParametri.nodes('/Parametri/LocalitaNascita') as ValoreParametro(LocalitaNascita))	  
	SET @sLocalitaNascita=dbo.MF_TogliApici(@sLocalitaNascita)
	
		SET @sCAPDomicilio=(SELECT TOP 1 ValoreParametro.CAPDomicilio.value('.','VARCHAR(10)')
					  FROM @xParametri.nodes('/Parametri/CAPDomicilio') as ValoreParametro(CAPDomicilio))	  
	
		SET  @sCodComuneDomicilio =(SELECT TOP 1 ValoreParametro.CodComuneDomicilio.value('.','VARCHAR(10)')
					  FROM @xParametri.nodes('/Parametri/CodComuneDomicilio') as ValoreParametro(CodComuneDomicilio ))	  
					  
		SET  @sComuneDomicilio =(SELECT TOP 1 ValoreParametro.ComuneDomicilio.value('.','VARCHAR(255)')
					  FROM @xParametri.nodes('/Parametri/ComuneDomicilio') as ValoreParametro(ComuneDomicilio ))	  
	SET @sComuneDomicilio=dbo.MF_TogliApici(@sComuneDomicilio)
					  				  
		SET  @sIndirizzoDomicilio =(SELECT TOP 1 ValoreParametro.IndirizzoDomicilio.value('.','VARCHAR(255)')
					  FROM @xParametri.nodes('/Parametri/IndirizzoDomicilio') as ValoreParametro(IndirizzoDomicilio ))	  
	SET @sIndirizzoDomicilio=dbo.MF_TogliApici(@sIndirizzoDomicilio)
	
		SET  @sLocalitaDomicilio =(SELECT TOP 1 ValoreParametro.LocalitaDomicilio.value('.','VARCHAR(255)')
					  FROM @xParametri.nodes('/Parametri/LocalitaDomicilio') as ValoreParametro(LocalitaDomicilio ))	  
	SET @sLocalitaDomicilio=dbo.MF_TogliApici(@sLocalitaDomicilio)
	
		SET @sCodProvinciaDomicilio=(SELECT TOP 1 ValoreParametro.CodProvinciaDomicilio.value('.','VARCHAR(10)')
					  FROM @xParametri.nodes('/Parametri/CodProvinciaDomicilio') as ValoreParametro(CodProvinciaDomicilio))	 
	
		SET @sProvinciaDomicilio=(SELECT TOP 1 ValoreParametro.ProvinciaDomicilio.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/ProvinciaDomicilio') as ValoreParametro(ProvinciaDomicilio))	  
	SET @sProvinciaDomicilio=dbo.MF_TogliApici(@sProvinciaDomicilio)
	
		SET @sCodRegioneDomicilio=(SELECT TOP 1 ValoreParametro.CodRegioneDomicilio.value('.','VARCHAR(10)')
					  FROM @xParametri.nodes('/Parametri/CodRegioneDomicilio') as ValoreParametro(CodRegioneDomicilio))	  
	
		SET @sRegioneDomicilio =(SELECT TOP 1 ValoreParametro.RegioneDomicilio.value('.','VARCHAR(255)')
					  FROM @xParametri.nodes('/Parametri/RegioneDomicilio') as ValoreParametro(RegioneDomicilio ))  
	SET @sRegioneDomicilio=dbo.MF_TogliApici(@sRegioneDomicilio)
	
		SET  @sCAPResidenza =(SELECT TOP 1 ValoreParametro.CAPResidenza.value('.','VARCHAR(10)')
					  FROM @xParametri.nodes('/Parametri/CAPResidenza') as ValoreParametro(CAPResidenza ))  
	
		SET  @sCodComuneResidenza =(SELECT TOP 1 ValoreParametro.CodComuneResidenza.value('.','VARCHAR(10)')
					  FROM @xParametri.nodes('/Parametri/CodComuneResidenza') as ValoreParametro(CodComuneResidenza))  
	
		SET  @sComuneResidenza =(SELECT TOP 1 ValoreParametro.ComuneResidenza.value('.','VARCHAR(255)')
					  FROM @xParametri.nodes('/Parametri/ComuneResidenza') as ValoreParametro(ComuneResidenza))  
	SET @sComuneResidenza=dbo.MF_TogliApici(@sComuneResidenza)

		SET  @sIndirizzoResidenza =(SELECT TOP 1 ValoreParametro.IndirizzoResidenza.value('.','VARCHAR(255)')
					  FROM @xParametri.nodes('/Parametri/IndirizzoResidenza') as ValoreParametro(IndirizzoResidenza))  					  
	SET @sIndirizzoResidenza=dbo.MF_TogliApici(@sIndirizzoResidenza)
	
		SET  @sLocalitaResidenza =(SELECT TOP 1 ValoreParametro.LocalitaResidenza.value('.','VARCHAR(255)')
					  FROM @xParametri.nodes('/Parametri/LocalitaResidenza') as ValoreParametro(LocalitaResidenza))  			
	SET @sLocalitaResidenza=dbo.MF_TogliApici(@sLocalitaResidenza)

		SET  @sCodProvinciaResidenza =(SELECT TOP 1 ValoreParametro.CodProvinciaResidenza.value('.','VARCHAR(10)')
					  FROM @xParametri.nodes('/Parametri/CodProvinciaResidenza') as ValoreParametro(CodProvinciaResidenza)) 

		SET  @sProvinciaResidenza =(SELECT TOP 1 ValoreParametro.ProvinciaResidenza.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/ProvinciaResidenza') as ValoreParametro(ProvinciaResidenza)) 					  
	SET @sProvinciaResidenza=dbo.MF_TogliApici(@sProvinciaResidenza)
	
		SET  @sCodRegioneResidenza =(SELECT TOP 1 ValoreParametro.CodRegioneResidenza.value('.','VARCHAR(10)')
					  FROM @xParametri.nodes('/Parametri/CodRegioneResidenza') as ValoreParametro(CodRegioneResidenza)) 
	
		SET  @sRegioneResidenza =(SELECT TOP 1 ValoreParametro.RegioneResidenza.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/RegioneResidenza') as ValoreParametro(RegioneResidenza)) 				  
	SET @sRegioneResidenza=dbo.MF_TogliApici(@sRegioneResidenza)
		  					  		
	SET  @txtFoto =(SELECT TOP 1 ValoreParametro.Foto.value('.','VARCHAR(MAX)')
					  FROM @xParametri.nodes('/Parametri/Foto') as ValoreParametro(Foto)) 	
	
	SET @binFoto=CAST(N'' AS xml).value('xs:base64Binary(sql:variable("@txtFoto"))', 'varbinary(max)')
		
				
		SET  @sCodMedicoBase =(SELECT TOP 1 ValoreParametro.CodMedicoBase.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/CodMedicoBase') as ValoreParametro(CodMedicoBase)) 
	
		SET  @sCodFiscMedicoBase =(SELECT TOP 1 ValoreParametro.CodFiscMedicoBase.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodFiscMedicoBase') as ValoreParametro(CodFiscMedicoBase)) 
		
		SET  @sCognomeNomeMedicoBase =(SELECT TOP 1 ValoreParametro.CognomeNomeMedicoBase.value('.','VARCHAR(255)')
					  FROM @xParametri.nodes('/Parametri/CognomeNomeMedicoBase') as ValoreParametro(CognomeNomeMedicoBase)) 				  
	SET @sCognomeNomeMedicoBase=dbo.MF_TogliApici(@sCognomeNomeMedicoBase)
	
		SET  @sDistrettoMedicoBase =(SELECT TOP 1 ValoreParametro.DistrettoMedicoBase.value('.','VARCHAR(255)')
					  FROM @xParametri.nodes('/Parametri/DistrettoMedicoBase') as ValoreParametro(DistrettoMedicoBase)) 
	SET @sDistrettoMedicoBase=dbo.MF_TogliApici(@sDistrettoMedicoBase)
			
		SET @sDataTmp=(SELECT TOP 1 ValoreParametro.DataSceltaMedicoBase.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/DataSceltaMedicoBase') as ValoreParametro(DataSceltaMedicoBase))					  
	SET @sDataTmp=ISNULL(@sDataTmp,'')
	IF @sDataTmp<> '' 
		BEGIN
		IF LEN(@sDataTmp) = 10
			BEGIN
								SET @sDataTmp= SUBSTRING(@sDataTmp,4,2) + '-' 
							+ LEFT(@sDataTmp,2) + '-' +
							+ SUBSTRING(@sDataTmp,7,4) 							
			END
			ELSE
			BEGIN
								SET @sDataTmp= SUBSTRING(@sDataTmp,4,2) + '-' 
							+ LEFT(@sDataTmp,2) + '-' +
							+ SUBSTRING(@sDataTmp,7,4) +
							' ' + RIGHT(@sDataTmp,5)					
			END
			IF ISDATE(@sDataTmp)=1
				SET	@dDataSceltaMedicoBase=CONVERT(DATETIME,@sDataTmp,120)						
			ELSE
				SET	@dDataSceltaMedicoBase =NULL		
		END
		
		SET @sDataTmp=(SELECT TOP 1 ValoreParametro.DataDecesso.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/DataDecesso') as ValoreParametro(DataDecesso))					  
	SET @sDataTmp=ISNULL(@sDataTmp,'')
	IF @sDataTmp<> '' 
		BEGIN
		IF LEN(@sDataTmp) = 10
			BEGIN
								SET @sDataTmp= SUBSTRING(@sDataTmp,4,2) + '-' 
							+ LEFT(@sDataTmp,2) + '-' +
							+ SUBSTRING(@sDataTmp,7,4) 							
			END
			ELSE
			BEGIN
								SET @sDataTmp= SUBSTRING(@sDataTmp,4,2) + '-' 
							+ LEFT(@sDataTmp,2) + '-' +
							+ SUBSTRING(@sDataTmp,7,4) +
							' ' + RIGHT(@sDataTmp,5)					
			END
			IF ISDATE(@sDataTmp)=1
				SET	@dDataDecesso=CONVERT(DATETIME,@sDataTmp,120)						
			ELSE
				SET	@dDataDecesso =NULL		
		END
		SET @sElencoEsenzioni =(SELECT TOP 1 ValoreParametro.ElencoEsenzioni.value('.','VARCHAR(MAX)')
					  FROM @xParametri.nodes('/Parametri/ElencoEsenzioni') as ValoreParametro(ElencoEsenzioni)) 
					  
	SET @sElencoEsenzioni=dbo.MF_TogliApici(@sElencoEsenzioni)					  
			

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
					  ,DataDecesso
				  )
		VALUES
				( 
					   @uGUID															  ,@uIDEpisodio														  ,@uIDPaziente														  ,@uCodSAC															  ,@sCognome														  ,@sNome															  ,@sSesso															  ,@dDataNascita													  ,@sCodiceFiscale													  ,@sCodComuneNascita												  ,@sComuneNascita													  ,@sCodProvinciaNascita											  ,@sProvinciaNascita												  ,@sLocalitaNascita												  ,@sCAPDomicilio													  ,@sCodComuneDomicilio												  ,@sComuneDomicilio												  ,@sIndirizzoDomicilio												  ,@sLocalitaDomicilio												  ,@sCodProvinciaDomicilio											  ,@sProvinciaDomicilio												  ,@sCodRegioneDomicilio											  ,@sRegioneDomicilio												  ,@sCAPResidenza													  ,@sCodComuneResidenza												  ,@sComuneResidenza												  ,@sIndirizzoResidenza												  ,@sLocalitaResidenza												  ,@sCodProvinciaResidenza											  ,@sProvinciaResidenza												  ,@sCodRegioneResidenza											  ,@sRegioneResidenza												  ,@binFoto															  ,@sCodMedicoBase													  ,@sCodFiscMedicoBase											      ,@sCognomeNomeMedicoBase										      ,@sDistrettoMedicoBase										      ,@dDataSceltaMedicoBase										      ,@sElencoEsenzioni												  ,@dDataDecesso												)
		
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
							  ,DataDecesso
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