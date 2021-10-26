CREATE PROCEDURE [dbo].[MSP_AggPazienti](@xParametri XML)
AS
BEGIN
		
	
				

								
		DECLARE @uID AS UNIQUEIDENTIFIER					
 	DECLARE @uCodSAC	AS UNIQUEIDENTIFIER	
	DECLARE @sCognome	AS VARCHAR(255)			 
	DECLARE @sNome		AS VARCHAR(255)			  
	DECLARE @sSesso		AS VARCHAR(1)			  			  
	DECLARE @dDataNascita	AS DateTime
	DECLARE @sCodiceFiscale		AS VARCHAR(20)			  	  
	DECLARE @sCodComuneNascita	AS VARCHAR(10)			  	  			
	DECLARE @sComuneNascita	AS VARCHAR(255)			  
	DECLARE @sCodProvinciaNascita	AS VARCHAR(10)	
	DECLARE @sProvinciaNascita AS VARCHAR(50)
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
	DECLARE @sTmp AS VARCHAR(MAX)	
		
	DECLARE @sSQL AS VARCHAR(MAX)
	DECLARE @sSET AS VARCHAR(MAX)
	DECLARE @sWHERE AS VARCHAR(MAX)

	    DECLARE @xTimeStampBase AS XML	
	DECLARE @xTimeStamp AS XML		
	
		DECLARE @xLogPrima AS XML
	DECLARE @xLogDopo AS XML
	DECLARE @xTemp AS XML
	DECLARE @xParLog AS XML
	DECLARE @xParMovPaz AS XML

		SET @sSQL='UPDATE T_Pazienti ' + CHAR(13) + CHAR(10) +
			  'SET '  				  						
			
	SET @sSET =''	
	
		IF @xParametri.exist('/Parametri/ID')=1
		BEGIN
			
			SET @sGUID=(SELECT TOP 1 ValoreParametro.ID.value('.','VARCHAR(50)')
							  FROM @xParametri.nodes('/Parametri/ID') as ValoreParametro(ID))
			IF 	ISNULL(@sGUID,'') <> ''				
					SET @uID=CONVERT(UNIQUEIDENTIFIER,	@sGUID)											
							END					
			
		IF @xParametri.exist('/Parametri/CodSAC')=1
		BEGIN	
		
			SET @sGUID=(SELECT TOP 1 ValoreParametro.CodSAC.value('.','VARCHAR(50)')
				  FROM @xParametri.nodes('/Parametri/CodSAC') as ValoreParametro(CodSAC))

			IF 	ISNULL(@sGUID,'') <> '' 
				BEGIN
					SET @uCodSAC=CONVERT(UNIQUEIDENTIFIER,	@sGUID)		
					SET	@sSET= @sSET + ',CodSAC=''' + convert(VARCHAR(50),@uCodSAC) + ''''	+ CHAR(13) + CHAR(10)	
				END
			ELSE
				SET	@sSET= @sSET + ',CodSAC=NULL'	+ CHAR(13) + CHAR(10)		
			
		END	
	
					
		IF @xParametri.exist('/Parametri/Cognome')=1
	BEGIN	
		SET @sCognome=(SELECT TOP 1 ValoreParametro.Cognome.value('.','VARCHAR(255)')
					  FROM @xParametri.nodes('/Parametri/Cognome') as ValoreParametro(Cognome))			
	
		IF @sCognome <> ''
			BEGIN				
				SET @sTmp=dbo.MF_SQLVarcharInsert(@sCognome)				
				SET @sTmp='CONVERT(VARCHAR(MAX),' + @sTmp + ')'
				SET @sTmp='dbo.MF_TogliApici(' + @sTmp + ')'
				SET	@sSET= @sSET + ',Cognome=' + @sTmp + '' + CHAR(13) + CHAR(10)		
			END	
		ELSE
			SET @sSET= @sSET + ',Cognome=NULL ' + CHAR(13) + CHAR(10)			
	END	
			
		IF @xParametri.exist('/Parametri/Nome')=1
	BEGIN	
		SET @sNome=(SELECT TOP 1 ValoreParametro.Nome.value('.','VARCHAR(255)')
					  FROM @xParametri.nodes('/Parametri/Nome') as ValoreParametro(Nome))			
	
		IF @sNome <> ''
			BEGIN		
				SET @sTmp=dbo.MF_SQLVarcharInsert(@sNome)
				SET @sTmp='CONVERT(VARCHAR(MAX),' + @sTmp + ')'
				SET @sTmp='dbo.MF_TogliApici(' + @sTmp + ')'
				SET	@sSET= @sSET + ',Nome=' + @sTmp + '' + CHAR(13) + CHAR(10)		
			END	
		ELSE
			SET @sSET= @sSET + ',Nome=NULL ' + CHAR(13) + CHAR(10)			
	END	
			
		
		IF @xParametri.exist('/Parametri/Sesso')=1
	BEGIN	
		SET @sSesso=(SELECT TOP 1 ValoreParametro.Sesso.value('.','VARCHAR(1)')
					  FROM @xParametri.nodes('/Parametri/Sesso') as ValoreParametro(Sesso))			
	
		IF @sSesso <> ''
			SET	@sSET= @sSET + ',Sesso=''' + @sSesso + '''' + CHAR(13) + CHAR(10)		
		ELSE
			SET @sSET= @sSET + ',Sesso=NULL ' + CHAR(13) + CHAR(10)			
	END	
	
		IF @xParametri.exist('/Parametri/DataNascita')=1
	BEGIN	
		SET @sDataTmp=(SELECT TOP 1 ValoreParametro.DataNascita.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/DataNascita') as ValoreParametro(DataNascita))					  
		SET @sDataTmp=ISNULL(@sDataTmp,'')
	
		IF @sDataTmp<> '' 
		BEGIN			
						SET @sDataTmp= SUBSTRING(@sDataTmp,4,2) + '-' 
						+ LEFT(@sDataTmp,2) + '-' +
						+ SUBSTRING(@sDataTmp,7,4) 						
			IF ISDATE(@sDataTmp)=1
				SET	@dDataNascita =CONVERT(DATETIME,@sDataTmp,120)						
			ELSE
				SET	@dDataNascita  =NULL			
		END
		
		IF @dDataNascita  IS NOT NULL		
			SET	@sSET= @sSET + ',DataNascita =CONVERT(DateTime,''' + CONVERT(VARCHAR(20),@dDataNascita ,120) + ''',120)' + CHAR(13) + CHAR(10)		
		ELSE
			SET	@sSET= @sSET + ',DataNascita =NULL ' + CHAR(13) + CHAR(10)		
	END	
	
		IF @xParametri.exist('/Parametri/CodiceFiscale')=1
	BEGIN	
		SET @sCodiceFiscale=(SELECT TOP 1 ValoreParametro.CodiceFiscale.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodiceFiscale') as ValoreParametro(CodiceFiscale))			
	
		IF @sCodiceFiscale <> ''
		BEGIN		
			SET @sTmp=dbo.MF_SQLVarcharInsert(@sCodiceFiscale)
			SET	@sSET= @sSET + ',CodiceFiscale=' + @sTmp + '' + CHAR(13) + CHAR(10)		
		END			
		ELSE
			SET @sSET= @sSET + ',CodiceFiscale=NULL ' + CHAR(13) + CHAR(10)			
	END	
					  
		IF @xParametri.exist('/Parametri/CodComuneNascita')=1
	BEGIN	
		SET @sCodComuneNascita=(SELECT TOP 1 ValoreParametro.CodComuneNascita.value('.','VARCHAR(10)')
					  FROM @xParametri.nodes('/Parametri/CodComuneNascita') as ValoreParametro(CodComuneNascita))			
	
		IF @sCodComuneNascita <> ''
		BEGIN
			SET @sTmp=dbo.MF_SQLVarcharInsert(@sCodComuneNascita)
			SET	@sSET= @sSET + ',CodComuneNascita=' + @sTmp + '' + CHAR(13) + CHAR(10)		
		END	
		ELSE
			SET @sSET= @sSET + ',CodComuneNascita=NULL ' + CHAR(13) + CHAR(10)			
	END				
	
		IF @xParametri.exist('/Parametri/ComuneNascita')=1
	BEGIN	
		SET @sComuneNascita	=(SELECT TOP 1 ValoreParametro.ComuneNascita.value('.','VARCHAR(255)')
					  FROM @xParametri.nodes('/Parametri/ComuneNascita') as ValoreParametro(ComuneNascita))			
	
		IF @sComuneNascita	 <> ''
		BEGIN
			SET @sTmp=dbo.MF_SQLVarcharInsert(@sComuneNascita)
			SET @sTmp='CONVERT(VARCHAR(MAX),' + @sTmp + ')'
			SET @sTmp='dbo.MF_TogliApici(' + @sTmp + ')'
			SET	@sSET= @sSET + ',ComuneNascita=' + @sTmp + '' + CHAR(13) + CHAR(10)		
		END	
		ELSE
			SET @sSET= @sSET + ',ComuneNascita=NULL ' + CHAR(13) + CHAR(10)			
	END		
				  		  	  			
		IF @xParametri.exist('/Parametri/CodProvinciaNascita')=1
	BEGIN	
		SET @sCodProvinciaNascita=(SELECT TOP 1 ValoreParametro.CodProvinciaNascita.value('.','VARCHAR(10)')
					  FROM @xParametri.nodes('/Parametri/CodProvinciaNascita') as ValoreParametro(CodProvinciaNascita))	   
		IF @sCodProvinciaNascita	 <> ''
		BEGIN
			SET @sTmp=dbo.MF_SQLVarcharInsert(@sCodProvinciaNascita)
			SET	@sSET= @sSET + ',CodProvinciaNascita=' + @sTmp + '' + CHAR(13) + CHAR(10)		
		END	
		ELSE
			SET @sSET= @sSET + ',CodProvinciaNascita=NULL ' + CHAR(13) + CHAR(10)			
	END		
	
		IF @xParametri.exist('/Parametri/ProvinciaNascita')=1
	BEGIN	
		SET @sProvinciaNascita=(SELECT TOP 1 ValoreParametro.ProvinciaNascita.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/ProvinciaNascita') as ValoreParametro(ProvinciaNascita))	   
		IF @sProvinciaNascita	 <> ''
		BEGIN
			SET @sTmp=dbo.MF_SQLVarcharInsert(@sProvinciaNascita)
			SET @sTmp='CONVERT(VARCHAR(MAX),' + @sTmp + ')'
			SET @sTmp='dbo.MF_TogliApici(' + @sTmp + ')'
			SET	@sSET= @sSET + ',ProvinciaNascita=' + @sTmp + '' + CHAR(13) + CHAR(10)		
		END	
		ELSE
			SET @sSET= @sSET + ',ProvinciaNascita=NULL ' + CHAR(13) + CHAR(10)			
	END		
	
		IF @xParametri.exist('/Parametri/LocalitaNascita')=1
	BEGIN	
		SET @sLocalitaNascita=(SELECT TOP 1 ValoreParametro.LocalitaNascita.value('.','VARCHAR(255)')
					  FROM @xParametri.nodes('/Parametri/LocalitaNascita') as ValoreParametro(LocalitaNascita))	     
		IF @sLocalitaNascita	 <> ''
		BEGIN
			SET @sTmp=dbo.MF_SQLVarcharInsert(@sLocalitaNascita)
			SET @sTmp='CONVERT(VARCHAR(MAX),' + @sTmp + ')'
			SET @sTmp='dbo.MF_TogliApici(' + @sTmp + ')'
			SET	@sSET= @sSET + ',LocalitaNascita=' + @sTmp + '' + CHAR(13) + CHAR(10)		
		END	
		ELSE
			SET @sSET= @sSET + ',LocalitaNascita=NULL ' + CHAR(13) + CHAR(10)			
	END	
	
		IF @xParametri.exist('/Parametri/CAPDomicilio')=1
	BEGIN	
		SET @sCAPDomicilio=(SELECT TOP 1 ValoreParametro.CAPDomicilio.value('.','VARCHAR(10)')
					  FROM @xParametri.nodes('/Parametri/CAPDomicilio') as ValoreParametro(CAPDomicilio))     
		IF @sCAPDomicilio	 <> ''
		BEGIN
			SET @sTmp=dbo.MF_SQLVarcharInsert(@sCAPDomicilio)
			SET	@sSET= @sSET + ',CAPDomicilio=' + @sTmp + '' + CHAR(13) + CHAR(10)		
		END	
		ELSE
			SET @sSET= @sSET + ',CAPDomicilio=NULL ' + CHAR(13) + CHAR(10)			
	END	
	
		IF @xParametri.exist('/Parametri/CodComuneDomicilio')=1
	BEGIN	
		SET @sComuneDomicilio =(SELECT TOP 1 ValoreParametro.CodComuneDomicilio.value('.','VARCHAR(10)')
					  FROM @xParametri.nodes('/Parametri/CodComuneDomicilio') as ValoreParametro(CodComuneDomicilio ))	   
		IF @sComuneDomicilio	 <> ''
		BEGIN
			SET @sTmp=dbo.MF_SQLVarcharInsert(@sComuneDomicilio)
			SET	@sSET= @sSET + ',CodComuneDomicilio=' + @sTmp + '' + CHAR(13) + CHAR(10)		
		END	
		ELSE
			SET @sSET= @sSET + ',CodComuneDomicilio=NULL ' + CHAR(13) + CHAR(10)			
	END
	
	
		IF @xParametri.exist('/Parametri/ComuneDomicilio')=1
	BEGIN	
		SET @sComuneDomicilio =(SELECT TOP 1 ValoreParametro.ComuneDomicilio.value('.','VARCHAR(255)')
					  FROM @xParametri.nodes('/Parametri/ComuneDomicilio') as ValoreParametro(ComuneDomicilio ))	   
		IF @sComuneDomicilio	 <> ''
		BEGIN
			SET @sTmp=dbo.MF_SQLVarcharInsert(@sComuneDomicilio)
			SET @sTmp='CONVERT(VARCHAR(MAX),' + @sTmp + ')'
			SET @sTmp='dbo.MF_TogliApici(' + @sTmp + ')'
			SET	@sSET= @sSET + ',ComuneDomicilio=' + @sTmp + '' + CHAR(13) + CHAR(10)		
		END	
		ELSE
			SET @sSET= @sSET + ',ComuneDomicilio=NULL ' + CHAR(13) + CHAR(10)			
	END
					  				  
		IF @xParametri.exist('/Parametri/IndirizzoDomicilio')=1
	BEGIN	
		SET  @sIndirizzoDomicilio =(SELECT TOP 1 ValoreParametro.IndirizzoDomicilio.value('.','VARCHAR(255)')
					  FROM @xParametri.nodes('/Parametri/IndirizzoDomicilio') as ValoreParametro(IndirizzoDomicilio ))	  	   
		IF @sIndirizzoDomicilio	 <> ''
		BEGIN
			SET @sTmp=dbo.MF_SQLVarcharInsert(@sIndirizzoDomicilio)
			SET @sTmp='CONVERT(VARCHAR(MAX),' + @sTmp + ')'
			SET @sTmp='dbo.MF_TogliApici(' + @sTmp + ')'
			SET	@sSET= @sSET + ',IndirizzoDomicilio=' + @sTmp + '' + CHAR(13) + CHAR(10)		
		END	
		ELSE
			SET @sSET= @sSET + ',IndirizzoDomicilio=NULL ' + CHAR(13) + CHAR(10)			
	END
	
		IF @xParametri.exist('/Parametri/LocalitaDomicilio')=1
	BEGIN	
		SET @sLocalitaDomicilio =(SELECT TOP 1 ValoreParametro.LocalitaDomicilio.value('.','VARCHAR(255)')
					  FROM @xParametri.nodes('/Parametri/LocalitaDomicilio') as ValoreParametro(LocalitaDomicilio ))	  	   
		IF @sLocalitaDomicilio	 <> ''
		BEGIN
			SET @sTmp=dbo.MF_SQLVarcharInsert(@sLocalitaDomicilio)
			SET @sTmp='CONVERT(VARCHAR(MAX),' + @sTmp + ')'
			SET @sTmp='dbo.MF_TogliApici(' + @sTmp + ')'
			SET	@sSET= @sSET + ',LocalitaDomicilio=' +  @sTmp + '' + CHAR(13) + CHAR(10)		
		END	
		ELSE
			SET @sSET= @sSET + ',LocalitaDomicilio=NULL ' + CHAR(13) + CHAR(10)			
	END
	
		IF @xParametri.exist('/Parametri/CodProvinciaDomicilio')=1
	BEGIN	
		SET @sCodProvinciaDomicilio=(SELECT TOP 1 ValoreParametro.CodProvinciaDomicilio.value('.','VARCHAR(10)')
					  FROM @xParametri.nodes('/Parametri/CodProvinciaDomicilio') as ValoreParametro(CodProvinciaDomicilio))		  	   
		IF @sCodProvinciaDomicilio	 <> ''
		BEGIN
			SET @sTmp=dbo.MF_SQLVarcharInsert(@sCodProvinciaDomicilio)
			SET	@sSET= @sSET + ',CodProvinciaDomicilio=' + @sTmp + '' + CHAR(13) + CHAR(10)		
		END	
		ELSE
			SET @sSET= @sSET + ',CodProvinciaDomicilio=NULL ' + CHAR(13) + CHAR(10)			
	END
	
		IF @xParametri.exist('/Parametri/ProvinciaDomicilio')=1
	BEGIN	
		SET @sProvinciaDomicilio=(SELECT TOP 1 ValoreParametro.ProvinciaDomicilio.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/ProvinciaDomicilio') as ValoreParametro(ProvinciaDomicilio))	  
		IF @sProvinciaDomicilio	 <> ''
		BEGIN
			SET @sTmp=dbo.MF_SQLVarcharInsert(@sProvinciaDomicilio)
			SET @sTmp='CONVERT(VARCHAR(MAX),' + @sTmp + ')'
			SET @sTmp='dbo.MF_TogliApici(' + @sTmp + ')'
			SET	@sSET= @sSET + ',ProvinciaDomicilio=' + @sTmp + '' + CHAR(13) + CHAR(10)		
		END	
		ELSE
			SET @sSET= @sSET + ',ProvinciaDomicilio=NULL ' + CHAR(13) + CHAR(10)			
	END
	
		IF @xParametri.exist('/Parametri/CodRegioneDomicilio')=1
	BEGIN	
		SET @sCodRegioneDomicilio=(SELECT TOP 1 ValoreParametro.CodRegioneDomicilio.value('.','VARCHAR(10)')
					  FROM @xParametri.nodes('/Parametri/CodRegioneDomicilio') as ValoreParametro(CodRegioneDomicilio))	  	  
		IF @sCodRegioneDomicilio	 <> ''
		BEGIN
			SET @sTmp=dbo.MF_SQLVarcharInsert(@sCodRegioneDomicilio)
			SET	@sSET= @sSET + ',CodRegioneDomicilio=' + @sTmp + '' + CHAR(13) + CHAR(10)		
		END	
		ELSE
			SET @sSET= @sSET + ',CodRegioneDomicilio=NULL ' + CHAR(13) + CHAR(10)			
	END
	
		IF @xParametri.exist('/Parametri/RegioneDomicilio')=1
	BEGIN	
		SET  @sRegioneDomicilio =(SELECT TOP 1 ValoreParametro.RegioneDomicilio.value('.','VARCHAR(255)')
					  FROM @xParametri.nodes('/Parametri/RegioneDomicilio') as ValoreParametro(RegioneDomicilio ))  
		IF @sRegioneDomicilio	 <> ''
		BEGIN
			SET @sTmp=dbo.MF_SQLVarcharInsert(@sRegioneDomicilio)
			SET @sTmp='CONVERT(VARCHAR(MAX),' + @sTmp + ')'
			SET @sTmp='dbo.MF_TogliApici(' + @sTmp + ')'
			SET	@sSET= @sSET + ',RegioneDomicilio=' + @sTmp + '' + CHAR(13) + CHAR(10)		
		END	
		ELSE
			SET @sSET= @sSET + ',RegioneDomicilio=NULL ' + CHAR(13) + CHAR(10)			
	END
	
		IF @xParametri.exist('/Parametri/CAPResidenza')=1
	BEGIN	
		SET @sCAPResidenza =(SELECT TOP 1 ValoreParametro.CAPResidenza.value('.','VARCHAR(10)')
					  FROM @xParametri.nodes('/Parametri/CAPResidenza') as ValoreParametro(CAPResidenza ))  
		IF @sCAPResidenza	 <> ''
		BEGIN
			SET @sTmp=dbo.MF_SQLVarcharInsert(@sCAPResidenza)
			SET	@sSET= @sSET + ',CAPResidenza=' + @sTmp + '' + CHAR(13) + CHAR(10)		
		END	
		ELSE
			SET @sSET= @sSET + ',CAPResidenza=NULL ' + CHAR(13) + CHAR(10)			
	END
	
		IF @xParametri.exist('/Parametri/CodComuneResidenza')=1
	BEGIN	
		SET @sCodComuneResidenza =(SELECT TOP 1 ValoreParametro.CodComuneResidenza.value('.','VARCHAR(10)')
					  FROM @xParametri.nodes('/Parametri/CodComuneResidenza') as ValoreParametro(CodComuneResidenza))   
		IF @sCodComuneResidenza	 <> ''
		BEGIN
			SET @sTmp=dbo.MF_SQLVarcharInsert(@sCodComuneResidenza)
			SET	@sSET= @sSET + ',CodComuneResidenza=' + @sTmp + '' + CHAR(13) + CHAR(10)		
		END	
		ELSE
			SET @sSET= @sSET + ',CodComuneResidenza=NULL ' + CHAR(13) + CHAR(10)			
	END
	
		IF @xParametri.exist('/Parametri/ComuneResidenza')=1
	BEGIN	
		SET  @sComuneResidenza =(SELECT TOP 1 ValoreParametro.ComuneResidenza.value('.','VARCHAR(255)')
					  FROM @xParametri.nodes('/Parametri/ComuneResidenza') as ValoreParametro(ComuneResidenza))  
		IF @sComuneResidenza	 <> ''
		BEGIN
			SET @sTmp=dbo.MF_SQLVarcharInsert(@sComuneResidenza)
			SET @sTmp='CONVERT(VARCHAR(MAX),' + @sTmp + ')'
			SET @sTmp='dbo.MF_TogliApici(' + @sTmp + ')'
			SET	@sSET= @sSET + ',ComuneResidenza=' +  @sTmp + '' + CHAR(13) + CHAR(10)		
		END	
		ELSE
			SET @sSET= @sSET + ',ComuneResidenza=NULL ' + CHAR(13) + CHAR(10)			
	END
	
		IF @xParametri.exist('/Parametri/IndirizzoResidenza')=1
	BEGIN	
		SET @sIndirizzoResidenza =(SELECT TOP 1 ValoreParametro.IndirizzoResidenza.value('.','VARCHAR(255)')
					  FROM @xParametri.nodes('/Parametri/IndirizzoResidenza') as ValoreParametro(IndirizzoResidenza))  					  
		IF @sIndirizzoResidenza	 <> ''
		BEGIN
			SET @sTmp=dbo.MF_SQLVarcharInsert(@sIndirizzoResidenza)
			SET @sTmp='CONVERT(VARCHAR(MAX),' + @sTmp + ')'
			SET @sTmp='dbo.MF_TogliApici(' + @sTmp + ')'
			SET	@sSET= @sSET + ',IndirizzoResidenza=' + @sTmp + '' + CHAR(13) + CHAR(10)		
		END	
		ELSE
			SET @sSET= @sSET + ',IndirizzoResidenza=NULL ' + CHAR(13) + CHAR(10)			
	END
	
		IF @xParametri.exist('/Parametri/LocalitaResidenza')=1
	BEGIN	
		SET  @sLocalitaResidenza =(SELECT TOP 1 ValoreParametro.LocalitaResidenza.value('.','VARCHAR(255)')
					  FROM @xParametri.nodes('/Parametri/LocalitaResidenza') as ValoreParametro(LocalitaResidenza))  			
		IF @sLocalitaResidenza	 <> ''
		BEGIN
			SET @sTmp=dbo.MF_SQLVarcharInsert(@sLocalitaResidenza)
			SET @sTmp='CONVERT(VARCHAR(MAX),' + @sTmp + ')'
			SET @sTmp='dbo.MF_TogliApici(' + @sTmp + ')'
			SET	@sSET= @sSET + ',LocalitaResidenza=' + @sTmp + '' + CHAR(13) + CHAR(10)		
		END	
		ELSE
			SET @sSET= @sSET + ',LocalitaResidenza=NULL ' + CHAR(13) + CHAR(10)			
	END
	
		IF @xParametri.exist('/Parametri/CodProvinciaResidenza')=1
	BEGIN	
		SET  @sCodProvinciaResidenza =(SELECT TOP 1 ValoreParametro.CodProvinciaResidenza.value('.','VARCHAR(10)')
				  FROM @xParametri.nodes('/Parametri/CodProvinciaResidenza') as ValoreParametro(CodProvinciaResidenza)) 		
		IF @sCodProvinciaResidenza	 <> ''
		BEGIN
			SET @sTmp=dbo.MF_SQLVarcharInsert(@sCodProvinciaResidenza)
			SET	@sSET= @sSET + ',CodProvinciaResidenza=' + @sTmp + '' + CHAR(13) + CHAR(10)		
		END	
		ELSE
			SET @sSET= @sSET + ',CodProvinciaResidenza=NULL ' + CHAR(13) + CHAR(10)			
	END
	
	
		IF @xParametri.exist('/Parametri/ProvinciaResidenza')=1
	BEGIN	
		SET @sProvinciaResidenza =(SELECT TOP 1 ValoreParametro.ProvinciaResidenza.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/ProvinciaResidenza') as ValoreParametro(ProvinciaResidenza)) 					  
		IF @sProvinciaResidenza	 <> ''
		BEGIN
			SET @sTmp=dbo.MF_SQLVarcharInsert(@sProvinciaResidenza)
			SET @sTmp='CONVERT(VARCHAR(MAX),' + @sTmp + ')'
			SET @sTmp='dbo.MF_TogliApici(' + @sTmp + ')'
			SET	@sSET= @sSET + ',ProvinciaResidenza=' + @sTmp + '' + CHAR(13) + CHAR(10)		
		END	
		ELSE
			SET @sSET= @sSET + ',ProvinciaResidenza=NULL ' + CHAR(13) + CHAR(10)			
	END
		
		IF @xParametri.exist('/Parametri/CodRegioneResidenza')=1
	BEGIN	
		SET  @sCodRegioneResidenza =(SELECT TOP 1 ValoreParametro.CodRegioneResidenza.value('.','VARCHAR(10)')
					  FROM @xParametri.nodes('/Parametri/CodRegioneResidenza') as ValoreParametro(CodRegioneResidenza)) 			  
		IF @sCodRegioneResidenza	 <> ''
		BEGIN
			SET @sTmp=dbo.MF_SQLVarcharInsert(@sCodRegioneResidenza)
			SET	@sSET= @sSET + ',CodRegioneResidenza=' + @sTmp + '' + CHAR(13) + CHAR(10)		
		END	
		ELSE
			SET @sSET= @sSET + ',CodRegioneResidenza=NULL ' + CHAR(13) + CHAR(10)			
	END
	
		IF @xParametri.exist('/Parametri/RegioneResidenza')=1
	BEGIN	
		SET  @sRegioneResidenza =(SELECT TOP 1 ValoreParametro.RegioneResidenza.value('.','VARCHAR(50)')
									FROM @xParametri.nodes('/Parametri/RegioneResidenza') as ValoreParametro(RegioneResidenza)) 				  			  
		IF @sRegioneResidenza	 <> ''
		BEGIN
			SET @sTmp=dbo.MF_SQLVarcharInsert(@sRegioneResidenza)
			SET @sTmp='CONVERT(VARCHAR(MAX),' + @sTmp + ')'
			SET @sTmp='dbo.MF_TogliApici(' + @sTmp + ')'
			SET	@sSET= @sSET + ',RegioneResidenza=' +  @sTmp + '' + CHAR(13) + CHAR(10)		
		END	
		ELSE
			SET @sSET= @sSET + ',RegioneResidenza=NULL ' + CHAR(13) + CHAR(10)			
	END

	
	IF @xParametri.exist('/Parametri/Foto')=1
	BEGIN			
		SET @txtFoto=(SELECT TOP 1 ValoreParametro.Foto.value('.','VARCHAR(MAX)')
							FROM @xParametri.nodes('/Parametri/Foto') as ValoreParametro(Foto))					  

		IF @txtFoto <> ''
			SET	@sSET= @sSET +',Foto=
									CAST(N'''' AS xml).value(''xs:base64Binary("' + @txtFoto
									+ '")'', ''varbinary(max)'')
							 '  + CHAR(13) + CHAR(10)	
		ELSE
			SET	@sSET= @sSET +',Foto=NULL '	+ CHAR(13) + CHAR(10)																			
	END
					  					  		
				
		IF @xParametri.exist('/Parametri/CodMedicoBase')=1
	BEGIN	
		SET  @sCodMedicoBase =(SELECT TOP 1 ValoreParametro.CodMedicoBase.value('.','VARCHAR(50)')
									FROM @xParametri.nodes('/Parametri/CodMedicoBase') as ValoreParametro(CodMedicoBase)) 				  			  
		IF @sCodMedicoBase <> ''
		BEGIN
			SET @sTmp=dbo.MF_SQLVarcharInsert(@sCodMedicoBase)
			SET	@sSET= @sSET + ',CodMedicoBase=' +  @sTmp + '' + CHAR(13) + CHAR(10)		
		END	
		ELSE
			SET @sSET= @sSET + ',CodMedicoBase=NULL ' + CHAR(13) + CHAR(10)			
	END
	
		IF @xParametri.exist('/Parametri/CodFiscMedicoBase')=1
	BEGIN	
		SET @sCodFiscMedicoBase =(SELECT TOP 1 ValoreParametro.CodFiscMedicoBase.value('.','VARCHAR(20)')
									FROM @xParametri.nodes('/Parametri/CodFiscMedicoBase') as ValoreParametro(CodFiscMedicoBase)) 				  			  
		IF @sCodFiscMedicoBase	 <> ''
		BEGIN
			SET @sTmp=dbo.MF_SQLVarcharInsert(@sCodFiscMedicoBase)
			SET	@sSET= @sSET + ',CodFiscMedicoBase=' +  @sTmp + '' + CHAR(13) + CHAR(10)		
		END	
		ELSE
			SET @sSET= @sSET + ',CodFiscMedicoBase=NULL ' + CHAR(13) + CHAR(10)			
	END
	
		IF @xParametri.exist('/Parametri/CognomeNomeMedicoBase')=1
	BEGIN	
		SET @sCognomeNomeMedicoBase =(SELECT TOP 1 ValoreParametro.CognomeNomeMedicoBase.value('.','VARCHAR(255)')
									FROM @xParametri.nodes('/Parametri/CognomeNomeMedicoBase') as ValoreParametro(CognomeNomeMedicoBase)) 				  			  
		IF @sCognomeNomeMedicoBase	 <> ''
		BEGIN
			SET @sTmp=dbo.MF_SQLVarcharInsert(@sCognomeNomeMedicoBase)
			SET @sTmp='CONVERT(VARCHAR(MAX),' + @sTmp + ')'
			SET @sTmp='dbo.MF_TogliApici(' + @sTmp + ')'
			SET	@sSET= @sSET + ',CognomeNomeMedicoBase=' +  @sTmp + '' + CHAR(13) + CHAR(10)		
		END	
		ELSE
			SET @sSET= @sSET + ',CognomeNomeMedicoBase=NULL ' + CHAR(13) + CHAR(10)			
	END
		
		IF @xParametri.exist('/Parametri/DistrettoMedicoBase')=1
	BEGIN	
		SET @sDistrettoMedicoBase =(SELECT TOP 1 ValoreParametro.DistrettoMedicoBase.value('.','VARCHAR(255)')
									FROM @xParametri.nodes('/Parametri/DistrettoMedicoBase') as ValoreParametro(DistrettoMedicoBase)) 				  			  
		IF @sDistrettoMedicoBase <> ''
		BEGIN
			SET @sTmp=dbo.MF_SQLVarcharInsert(@sDistrettoMedicoBase)
			SET @sTmp='CONVERT(VARCHAR(MAX),' + @sTmp + ')'
			SET @sTmp='dbo.MF_TogliApici(' + @sTmp + ')'
			SET	@sSET= @sSET + ',DistrettoMedicoBase=' +  @sTmp + '' + CHAR(13) + CHAR(10)		
		END	
		ELSE
			SET @sSET= @sSET + ',DistrettoMedicoBase=NULL ' + CHAR(13) + CHAR(10)			
	END
	
		IF @xParametri.exist('/Parametri/DataSceltaMedicoBase')=1
	BEGIN	
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
							+ SUBSTRING(@sDataTmp,7,4)  +
						  ' ' + RIGHT(@sDataTmp,5)	
			END
			IF ISDATE(@sDataTmp)=1
				SET	@dDataSceltaMedicoBase =CONVERT(DATETIME,@sDataTmp,120)						
			ELSE
				SET	@dDataSceltaMedicoBase  =NULL			
		END
		
		IF @dDataSceltaMedicoBase  IS NOT NULL		
			SET	@sSET= @sSET + ',DataSceltaMedicoBase =CONVERT(DateTime,''' + CONVERT(VARCHAR(20),@dDataSceltaMedicoBase ,120) + ''',120)' + CHAR(13) + CHAR(10)		
		ELSE
			SET	@sSET= @sSET + ',DataSceltaMedicoBase =NULL ' + CHAR(13) + CHAR(10)		
	END	
	
		IF @xParametri.exist('/Parametri/DataDecesso')=1
	BEGIN	
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
						+ SUBSTRING(@sDataTmp,7,4) 	+
						  ' ' + RIGHT(@sDataTmp,5)						
			
			END
			IF ISDATE(@sDataTmp)=1
				SET	@dDataDecesso =CONVERT(DATETIME,@sDataTmp,120)						
			ELSE
				SET	@dDataDecesso  =NULL			
		END
		
		IF @dDataDecesso  IS NOT NULL		
			SET	@sSET= @sSET + ',DataDecesso =CONVERT(DateTime,''' + CONVERT(VARCHAR(20),@dDataDecesso ,120) + ''',120)' + CHAR(13) + CHAR(10)		
		ELSE
			SET	@sSET= @sSET + ',DataDecesso =NULL ' + CHAR(13) + CHAR(10)		
	END	
			
																		
		IF @xParametri.exist('/Parametri/ElencoEsenzioni')=1
	BEGIN	
		SET @sElencoEsenzioni =(SELECT TOP 1 ValoreParametro.ElencoEsenzioni.value('.','VARCHAR(MAX)')
									FROM @xParametri.nodes('/Parametri/ElencoEsenzioni') as ValoreParametro(ElencoEsenzioni)) 				  			  
		IF @sElencoEsenzioni	 <> ''
		BEGIN
			SET @sTmp=dbo.MF_SQLVarcharInsert(@sElencoEsenzioni)
			SET @sTmp='CONVERT(VARCHAR(MAX),' + @sTmp + ')'
			SET @sTmp='dbo.MF_TogliApici(' + @sTmp + ')'
			SET	@sSET= @sSET + ',ElencoEsenzioni = '+  @sTmp + '' + CHAR(13) + CHAR(10)		
		END	
		ELSE
			SET @sSET= @sSET + ',ElencoEsenzioni = NULL ' + CHAR(13) + CHAR(10)			
	END
		
		
	    SET @xTimeStamp=(SELECT TOP 1 ValoreParametro.TS.query('.')
					  FROM @xParametri.nodes('/Parametri/TimeStamp') as ValoreParametro(TS))				
					
				
				
		IF LEFT(@sSET,1)=',' 
		SET @sSET=RIGHT(@sSET,LEN(@sSET)-1)
	
	IF @sSET <> ''		
		BEGIN
						
						SET @sWHERE =' WHERE ID=''' + convert(varchar(50),@uID) +''''
				
						SET @sSQL=	ISNULL(@sSQL,'') + CHAR(13) + CHAR(10) + 
						ISNULL(@sSET,'') + CHAR(13) + CHAR(10) + 
						ISNULL(@sWHERE,'')			

												
						SET @xTimeStamp.modify('delete (/TimeStamp/IDEntita)[1]') 						
			SET @xTimeStamp.modify('insert <IDEntita>{sql:variable("@uID")}</IDEntita> into (/TimeStamp)[1]')
			
						SET @xTimeStamp.modify('delete (/TimeStamp/IDPaziente)[1]') 						
			SET @xTimeStamp.modify('insert <IDPaziente>{sql:variable("@uID")}</IDPaziente> into (/TimeStamp)[1]')
			
						SET @xTimeStamp.modify('delete (/TimeStamp/CodEntita)[1]') 
			SET @xTimeStamp.modify('insert <CodEntita>ANA</CodEntita> into (/TimeStamp)[1]')
			
						SET @xTimeStamp.modify('delete (/TimeStamp/CodAzione)[1]') 
			SET @xTimeStamp.modify('insert <CodAzione>MOD</CodAzione> into (/TimeStamp)[1]')
	
						SET @xTimeStampBase=@xTimeStamp
	
			SET @xTimeStamp=CONVERT(XML,
										'<Parametri>' + CONVERT(varchar(max),@xTimeStamp) +
										'</Parametri>')
			
												
			SET @xParLog=CONVERT(XML,'<Parametri><LogPrima/><LogDopo/></Parametri>')										
			
			SET @xLogPrima=Convert(XML,'<DataSet></DataSet>')														
			
			SET @xTemp=
				(SELECT * FROM 
					(SELECT ID
						  ,IDNum
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
					 FROM T_Pazienti
					 WHERE ID=@uID												) AS [Table]
				FOR XML AUTO, ELEMENTS)

			SET @xLogPrima.modify('insert sql:variable("@xTemp") as first into (/DataSet)[1]')
																		
			SET @xParLog.modify('insert sql:variable("@xLogPrima") as first into (/Parametri/LogPrima)[1]')
				
			BEGIN TRANSACTION
												EXEC (@sSQL)
								
			IF @@ERROR=0 AND @@ROWCOUNT > 0
				BEGIN
										EXEC MSP_InsMovTimeStamp @xTimeStamp		
				END	
			IF @@ERROR=0 
				BEGIN	
																									
					SET @xParMovPaz=CONVERT(XML,'<Parametri></Parametri>')
					SET @xTemp=CONVERT(XML,'<IDPaziente>' + CONVERT(varchar(50),@uID) + '</IDPaziente>')
					SET @xParMovPaz.modify('insert sql:variable("@xTemp") as first into (/Parametri)[1]')				
					SET @xParMovPaz.modify('insert sql:variable("@xTimeStampBase") as last into (/Parametri)[1]')
					
										SET @xParMovPaz.modify('delete (/Parametri/TimeStamp/CodEntita)[1]') 
					SET @xParMovPaz.modify('insert <CodEntita>PAZ</CodEntita> into (/Parametri/TimeStamp)[1]')
					
										SET @xParMovPaz.modify('delete (/Parametri/TimeStamp/CodAzione)[1]') 
					SET @xParMovPaz.modify('insert <CodAzione>MOD</CodAzione> into (/Parametri/TimeStamp)[1]')
					
																				EXEC MSP_AggMovPazientiDaAnagra @xParMovPaz
									END		
																					IF @@ERROR = 0
				BEGIN
					COMMIT TRANSACTION
					
																				
					SET @xLogDopo=Convert(XML,'<DataSet></DataSet>')				
					
					SET @xTemp=
						(SELECT * FROM 
							(SELECT ID
							  ,IDNum							  
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
							 FROM T_Pazienti
							 WHERE ID=@uID											) AS [Table]
						FOR XML AUTO, ELEMENTS)

					SET @xLogDopo.modify('insert sql:variable("@xTemp") as first into (/DataSet)[1]')																											
					SET @xParLog.modify('insert sql:variable("@xLogDopo") as first into (/Parametri/LogDopo)[1]')
					
										SET @xParLog.modify('insert sql:variable("@xTimeStampBase") as last into (/Parametri)[1]')
					
										EXEC MSP_InsMovLog @xParLog
					
									END	
			ELSE
				BEGIN
					ROLLBACK TRANSACTION						
				END	 
		END		
	
	RETURN 0
END