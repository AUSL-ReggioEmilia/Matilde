CREATE PROCEDURE [MSP_AggMovSchede](@xParametri XML)
AS
BEGIN	
	

			
		DECLARE @sCodUA AS VARCHAR(20)	
	DECLARE @sCodEntita AS VARCHAR(20)
	DECLARE @uIDEntita AS UNIQUEIDENTIFIER			
	DECLARE @uIDPaziente AS UNIQUEIDENTIFIER		
	DECLARE @uIDEpisodio AS UNIQUEIDENTIFIER		
	DECLARE @uIDTrasferimento AS UNIQUEIDENTIFIER		
	DECLARE @uIDScheda AS UNIQUEIDENTIFIER	
	DECLARE @sCodScheda  AS VARCHAR(20)
	DECLARE @nVersione INTEGER
	DECLARE @nNumero INTEGER
	DECLARE @xDati XML	

	DECLARE @binAnteprimaRTF VARBINARY(MAX)
	DECLARE @binAnteprimaTXT VARBINARY(MAX)
	DECLARE @binDatiObbligatoriMancantiRTF VARBINARY(MAX)
	DECLARE @binDatiRilievoRTF VARBINARY(MAX)
	
	DECLARE @txtAnteprimaRTF VARCHAR(MAX)
	
	DECLARE @txtAnteprimaTXT VARCHAR(MAX)
	DECLARE @txtDatiObbligatoriMancantiRTF VARCHAR(MAX)
	DECLARE @txtDatiRilievoRTF VARCHAR(MAX)

	DECLARE @binDati VARBINARY(MAX)
	DECLARE @txtDati VARCHAR(MAX)
	DECLARE @uIDSchedaPadre AS UNIQUEIDENTIFIER		
	DECLARE @sCodStatoScheda AS VARCHAR(20) 
	DECLARE @sCodStatoSchedaCalcolato AS VARCHAR(20) 
	DECLARE @sCodRuolo AS VARCHAR(20) 
	DECLARE @sCodUtenteUltimaModifica AS VARCHAR(100)	
	
	DECLARE @sCodUtente AS VARCHAR(100)
	DECLARE @bInEvidenza AS BIT
	DECLARE @bValidata AS BIT	
	DECLARE @bNuovaRevisione AS BIT	
	DECLARE @sCodUtenteValidazione AS VARCHAR(100)
	DECLARE @bSchedaSemplice AS BIT
	DECLARE @sCodAzione AS VARCHAR(20)
	
		DECLARE @bAnnullaIDSchedaPadre AS BIT
	DECLARE @bAnnullaIDTrasferimento AS BIT
		
		DECLARE @sCodRuoloSuperUser AS VARCHAR(20)
	DECLARE @bAggiornaNullDatiMancanti AS BIT
	DECLARE @bAggiornaNullDatiRilievo AS BIT
	DECLARE @sGUID AS VARCHAR(50)
	DECLARE @uGUID AS UNIQUEIDENTIFIER
	DECLARE @sTmp AS VARCHAR(50)
	
	DECLARE @sSQL AS VARCHAR(MAX)
	DECLARE @sSET AS VARCHAR(MAX)
	DECLARE @sWHERE AS VARCHAR(MAX)
	
	
	    DECLARE @xTimeStampBase AS XML	
	DECLARE @xTimeStamp AS XML	
		
		DECLARE @xLogPrima AS XML
	DECLARE @xLogDopo AS XML
	DECLARE @xTemp AS XML
	DECLARE @xParLog AS XML
	
		SET  @bAggiornaNullDatiMancanti=0
	SET  @bAggiornaNullDatiRilievo=0	
	
		SET @sCodRuoloSuperUser=(SELECT TOP 1 Valore FROM T_Config WHERE ID=681)
	SET @sCodRuoloSuperUser=ISNULL(@sCodRuoloSuperUser,'')
	
		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDScheda.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDScheda') as ValoreParametro(IDScheda))
	IF 	ISNULL(@sGUID,'') <> '' SET @uIDScheda=CONVERT(UNIQUEIDENTIFIER,	@sGUID)			
	
	    SET @xTimeStamp=(SELECT TOP 1 ValoreParametro.TS.query('.')
					  FROM @xParametri.nodes('/Parametri/TimeStamp') as ValoreParametro(TS))
	

		SET @sSQL='UPDATE T_MovSchede ' + CHAR(13) + CHAR(10) +
			  'SET '  				  						
			
	SET @sSET =''

		IF @xParametri.exist('/Parametri/CodUA')=1
	BEGIN
			SET @sCodUA=(SELECT TOP 1 ValoreParametro.CodUA.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodUA') as ValoreParametro(CodUA))	
					  
			IF @sCodUA <> ''
				SET	@sSET= @sSET + ',CodUA=''' + @sCodUA +''''	+ CHAR(13) + CHAR(10)				
			ELSE
				SET	@sSET= @sSET + ',CodUA=NULL'	+ CHAR(13) + CHAR(10)	
	END	
	
	
	
		IF @xParametri.exist('/Parametri/CodEntita')=1
	BEGIN
			SET @sCodEntita=(SELECT TOP 1 ValoreParametro.CodEntita.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodEntita') as ValoreParametro(CodEntita))	
					  
			IF @sCodEntita <> ''
				SET	@sSET= @sSET + ',CodEntita=''' + @sCodEntita +''''	+ CHAR(13) + CHAR(10)				
			ELSE
				SET	@sSET= @sSET + ',CodEntita=NULL'	+ CHAR(13) + CHAR(10)	
	END	
	

		IF @xParametri.exist('/Parametri/IDEntita')=1
		BEGIN
			SET @sGUID=(SELECT TOP 1 ValoreParametro.IDEntita.value('.','VARCHAR(50)')
							  FROM @xParametri.nodes('/Parametri/IDEntita') as ValoreParametro(IDEntita))
							  
				IF 	ISNULL(@sGUID,'') <> '' 
				BEGIN
					SET @uIDEntita=CONVERT(UNIQUEIDENTIFIER,	@sGUID)		
					SET	@sSET= @sSET + ',IDEntita=''' + convert(VARCHAR(50),@uIDEntita) + ''''	+ CHAR(13) + CHAR(10)	
				END
				ELSE
					SET	@sSET= @sSET + ',IDEntita=NULL'	+ CHAR(13) + CHAR(10)									  		
		END		
			
		IF @xParametri.exist('/Parametri/IDPaziente')=1
		BEGIN
			SET @sGUID=(SELECT TOP 1 ValoreParametro.IDPaziente.value('.','VARCHAR(50)')
							  FROM @xParametri.nodes('/Parametri/IDPaziente') as ValoreParametro(IDPaziente))
							  
				IF 	ISNULL(@sGUID,'') <> '' 
				BEGIN
					SET @uIDPaziente=CONVERT(UNIQUEIDENTIFIER,	@sGUID)		
					SET	@sSET= @sSET + ',IDPaziente=''' + convert(VARCHAR(50),@uIDPaziente) + ''''	+ CHAR(13) + CHAR(10)	
				END
				ELSE
					SET	@sSET= @sSET + ',IDPaziente=NULL'	+ CHAR(13) + CHAR(10)									  		
		END		
	
		IF @xParametri.exist('/Parametri/IDEpisodio')=1
		BEGIN
			SET @sGUID=(SELECT TOP 1 ValoreParametro.IDEpisodio.value('.','VARCHAR(50)')
							  FROM @xParametri.nodes('/Parametri/IDEpisodio') as ValoreParametro(IDEpisodio))
							  
				IF 	ISNULL(@sGUID,'') <> '' 
				BEGIN
					SET @uIDEpisodio=CONVERT(UNIQUEIDENTIFIER,	@sGUID)		
					SET	@sSET= @sSET + ',IDEpisodio=''' + convert(VARCHAR(50),@uIDEpisodio) + ''''	+ CHAR(13) + CHAR(10)	
				END
				ELSE
					SET	@sSET= @sSET + ',IDEpisodio=NULL'	+ CHAR(13) + CHAR(10)									  		
		END	
	

	SET @bAnnullaIDTrasferimento=0
		IF @xParametri.exist('/Parametri/IDTrasferimento')=1
		BEGIN
			SET @sGUID=(SELECT TOP 1 ValoreParametro.IDTrasferimento.value('.','VARCHAR(50)')
							  FROM @xParametri.nodes('/Parametri/IDTrasferimento') as ValoreParametro(IDTrasferimento))
							  
				IF 	ISNULL(@sGUID,'') <> '' 
				BEGIN
					SET @bAnnullaIDTrasferimento=0
					SET @uIDTrasferimento=CONVERT(UNIQUEIDENTIFIER,	@sGUID)		
					SET	@sSET= @sSET + ',IDTrasferimento=''' + convert(VARCHAR(50),@uIDTrasferimento) + ''''	+ CHAR(13) + CHAR(10)	
				END
				ELSE
					SET @bAnnullaIDTrasferimento=1
					SET	@sSET= @sSET + ',IDTrasferimento=NULL'	+ CHAR(13) + CHAR(10)									  		
		END	
		
		IF @xParametri.exist('/Parametri/CodScheda')=1
	BEGIN
			SET @sCodScheda=(SELECT TOP 1 ValoreParametro.CodScheda.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodScheda') as ValoreParametro(CodScheda))	
					  
			IF @sCodScheda <> ''
				SET	@sSET= @sSET + ',CodScheda=''' + @sCodScheda +''''	+ CHAR(13) + CHAR(10)				
			ELSE
				SET	@sSET= @sSET + ',CodScheda=NULL'	+ CHAR(13) + CHAR(10)	
	END
		
		IF @xParametri.exist('/Parametri/Versione')=1
	BEGIN
			SET @sTmp=(SELECT TOP 1 ValoreParametro.Versione.value('.','INTEGER')
					  FROM @xParametri.nodes('/Parametri/Versione') as ValoreParametro(Versione))	
					  
			IF ISNUMERIC(@sTMP) <> ''
			BEGIN
				SET @nVersione=CONVERT(INTEGER,@sTmp)
				SET	@sSET= @sSET + ',Versione=' + @sTmp +''	+ CHAR(13) + CHAR(10)	
			END				
			ELSE
				SET	@sSET= @sSET + ',Versione=NULL'	+ CHAR(13) + CHAR(10)	
	END	

	
		IF @xParametri.exist('/Parametri/Numero')=1
	BEGIN
			SET  @sTmp=(SELECT TOP 1 ValoreParametro.Numero.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/Numero') as ValoreParametro(Numero))	
			
			IF ISNUMERIC(@sTMP)	=1 	
				BEGIN
					SET @nNumero=CONVERT(INTEGER,@sTmp)							
					SET	@sSET= @sSET + ',Numero=' +  @sTmp +''	+ CHAR(13) + CHAR(10)				
				END	
				ELSE
					SET	@sSET= @sSET + ',Numero=NULL'	+ CHAR(13) + CHAR(10)	
	END	
	
	
		IF @xParametri.exist('/Parametri/Dati')=1
	BEGIN					  	
		SET @xDati=(SELECT TOP 1 ValoreParametro.Dati.query('./*')
											FROM @xParametri.nodes('/Parametri/Dati') as ValoreParametro(Dati))				
			
				IF @uIDScheda IS NULL			  
		BEGIN		
				SET @binDati=CONVERT(VARBINARY(MAX),@xDati)
				SET @txtDati=CAST(N'' AS xml).value('xs:base64Binary(sql:variable("@binDati"))', 'varchar(max)')
				SET @binDati=CAST(N'' AS xml).value('xs:base64Binary(sql:variable("@txtDati"))', 'varbinary(max)')
					
				SET	@sSET= @sSET +',Dati=
											CONVERT(XML,
													CAST(N'''' AS xml).value(''xs:base64Binary("' + @txtDati + 
													'")'', ''varbinary(max)'') 
												)'  + CHAR(13) + CHAR(10)	
		END								
	END
	ELSE
			SET	@sSET= @sSET +',Dati=NULL '	+ CHAR(13) + CHAR(10)														
		
	
		IF @xParametri.exist('/Parametri/AnteprimaRTF')=1
	BEGIN	
	
		SET @binAnteprimaRTF=(SELECT TOP 1 ValoreParametro.AnteprimaRTF.value('.','VARBINARY(MAX)')
							  FROM @xParametri.nodes('/Parametri/AnteprimaRTF') as ValoreParametro(AnteprimaRTF))	
		
				IF @uIDScheda IS NULL			  
		BEGIN			  
			SET @txtAnteprimaRTF=(SELECT TOP 1 ValoreParametro.AnteprimaRTF.value('.','VARCHAR(MAX)')
					  FROM @xParametri.nodes('/Parametri/AnteprimaRTF') as ValoreParametro(AnteprimaRTF))					  

			IF @txtAnteprimaRTF <> ''
				SET	@sSET= @sSET +',AnteprimaRTF=
											CONVERT(varchar(max),
													CAST(N'''' AS xml).value(''xs:base64Binary("' + @txtAnteprimaRTF
													+ '")'', ''varbinary(max)'') 
												)'  + CHAR(13) + CHAR(10)	
			ELSE
				SET	@sSET= @sSET +',AnteprimaRTF=NULL '	+ CHAR(13) + CHAR(10)														
		END		
	END	
	
		IF @xParametri.exist('/Parametri/AnteprimaTXT')=1
	BEGIN	
		SET @binAnteprimaTXT=(SELECT TOP 1 ValoreParametro.AnteprimaTXT.value('.','VARBINARY(MAX)')
					  FROM @xParametri.nodes('/Parametri/AnteprimaTXT') as ValoreParametro(AnteprimaTXT))	
		
				IF @uIDScheda IS NULL
		BEGIN			  
			SET @txtAnteprimaTXT=(SELECT TOP 1 ValoreParametro.AnteprimaTXT.value('.','VARCHAR(MAX)')
					  FROM @xParametri.nodes('/Parametri/AnteprimaTXT') as ValoreParametro(AnteprimaTXT))					  

			IF @txtAnteprimaTXT <> ''
				SET	@sSET= @sSET +',AnteprimaTXT =
											CONVERT(varchar(max),
													CAST(N'''' AS xml).value(''xs:base64Binary("' + @txtAnteprimaTXT
													+ '")'', ''varbinary(max)'') 
												)'  + CHAR(13) + CHAR(10)	
			ELSE
				SET	@sSET= @sSET +',AnteprimaTXT=NULL '	+ CHAR(13) + CHAR(10)														
		END		
	END	

		
	
		IF @xParametri.exist('/Parametri/DatiObbligatoriMancantiRTF')=1
	BEGIN	
		
		SET @binDatiObbligatoriMancantiRTF=(SELECT TOP 1 ValoreParametro.DatiObbligatoriMancantiRTF.value('.','VARBINARY(MAX)')
					  FROM @xParametri.nodes('/Parametri/DatiObbligatoriMancantiRTF') as ValoreParametro(DatiObbligatoriMancantiRTF))
	
				IF @uIDScheda IS NULL
		BEGIN
			SET @txtDatiObbligatoriMancantiRTF=(SELECT TOP 1 ValoreParametro.DatiObbligatoriMancantiRTF.value('.','VARCHAR(MAX)')
					  FROM @xParametri.nodes('/Parametri/DatiObbligatoriMancantiRTF') as ValoreParametro(DatiObbligatoriMancantiRTF))					  

			IF @txtDatiObbligatoriMancantiRTF <> ''
				SET	@sSET= @sSET +',DatiObbligatoriMancantiRTF=
											CONVERT(varchar(max),
													CAST(N'''' AS xml).value(''xs:base64Binary("' + @txtDatiObbligatoriMancantiRTF
													+ '")'', ''varbinary(max)'') 
												)'  + CHAR(13) + CHAR(10)	
			ELSE
				SET	@sSET= @sSET +',DatiObbligatoriMancantiRTF=NULL '	+ CHAR(13) + CHAR(10)
		END
		
		IF @binDatiObbligatoriMancantiRTF IS NULL
			SET  @bAggiornaNullDatiMancanti=1
		
	END	
	
		IF @xParametri.exist('/Parametri/DatiRilievoRTF')=1
	BEGIN	
		SET @binDatiRilievoRTF=(SELECT TOP 1 ValoreParametro.DatiRilievoRTF.value('.','VARBINARY(MAX)')
					  FROM @xParametri.nodes('/Parametri/DatiRilievoRTF') as ValoreParametro(DatiRilievoRTF))	
		
				IF @uIDScheda IS NULL			  
		BEGIN
			SET @txtDatiRilievoRTF=(SELECT TOP 1 ValoreParametro.DatiRilievoRTF.value('.','VARCHAR(MAX)')
					  FROM @xParametri.nodes('/Parametri/DatiRilievoRTF') as ValoreParametro(DatiRilievoRTF))					  

			IF @txtDatiRilievoRTF <> ''
				SET	@sSET= @sSET +',DatiRilievoRTF=
											CONVERT(varchar(max),
													CAST(N'''' AS xml).value(''xs:base64Binary("' + @txtDatiRilievoRTF
													+ '")'', ''varbinary(max)'') 
												)'  + CHAR(13) + CHAR(10)	
			ELSE
				SET	@sSET= @sSET +',DatiRilievoRTF=NULL '	+ CHAR(13) + CHAR(10)														
		END
		
		IF @binDatiRilievoRTF IS NULL
			SET  @bAggiornaNullDatiRilievo=1				
	END	
	
		SET @bAnnullaIDSchedaPadre=0
	IF @xParametri.exist('/Parametri/IDSchedaPadre')=1
		BEGIN		
			SET @sGUID=(SELECT TOP 1 ValoreParametro.IDSchedaPadre.value('.','VARCHAR(50)')
							  FROM @xParametri.nodes('/Parametri/IDSchedaPadre') as ValoreParametro(IDSchedaPadre))
					
						  
				IF 	ISNULL(@sGUID,'') <> '' 
				BEGIN					
					SET @bAnnullaIDSchedaPadre=0
					SET @uIDSchedaPadre=CONVERT(UNIQUEIDENTIFIER,	@sGUID)		
					SET	@sSET= @sSET + ',IDSchedaPadre=''' + convert(VARCHAR(50),@uIDSchedaPadre) + ''''	+ CHAR(13) + CHAR(10)	
				END
				ELSE
				BEGIN					
					SET @bAnnullaIDSchedaPadre=1
					SET	@sSET= @sSET + ',IDSchedaPadre=NULL'	+ CHAR(13) + CHAR(10)									  		
				END	
		END	
	
		
		IF @xParametri.exist('/Parametri/CodStatoScheda')=1
	BEGIN
			SET @sCodStatoScheda=(SELECT TOP 1 ValoreParametro.CodStatoScheda.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodStatoScheda') as ValoreParametro(CodStatoScheda))	
					  
			IF @sCodStatoScheda <> ''
				SET	@sSET= @sSET + ',CodStatoScheda=''' + @sCodStatoScheda +''''	+ CHAR(13) + CHAR(10)				
			ELSE
				SET	@sSET= @sSET + ',CodStatoScheda=NULL'	+ CHAR(13) + CHAR(10)	
	END

		
		IF @xParametri.exist('/Parametri/InEvidenza')=1
	BEGIN
			SET @bInEvidenza=(SELECT TOP 1 ValoreParametro.InEvidenza.value('.','BIT')
					  FROM @xParametri.nodes('/Parametri/InEvidenza') as ValoreParametro(InEvidenza))	
					  
			IF @bInEvidenza IS NOT NULL
				SET	@sSET= @sSET + ',InEvidenza=''' + CONVERT(VARCHAR(1),ISNULL(@bInEvidenza,0)) + +''''	+ CHAR(13) + CHAR(10)				
	END		

		IF @xParametri.exist('/Parametri/Validata')=1
	BEGIN
			SET @bValidata=(SELECT TOP 1 ValoreParametro.Validata.value('.','BIT')
					  FROM @xParametri.nodes('/Parametri/Validata') as ValoreParametro(Validata))	
					  
			IF @bValidata IS NOT NULL
				SET	@sSET= @sSET + ',Validata=''' + CONVERT(VARCHAR(1),ISNULL(@bValidata,0)) + +''''	+ CHAR(13) + CHAR(10)				
	END
	
		IF ISNULL(@bValidata,0)=1
	BEGIN					
				SET @sCodUtenteValidazione=(SELECT TOP 1 ValoreParametro.CodUtenteUltimaModifica.value('.','VARCHAR(100)')
									 FROM @xParametri.nodes('/Parametri/TimeStamp/CodLogin') as ValoreParametro(CodUtenteUltimaModifica))	
		
		SET	@sSET= @sSET + ',CodUtenteValidazione=''' + @sCodUtenteValidazione + '''' + CHAR(13) + CHAR(10)		
		
				SET	@sSET= @sSET + ',DataValidazione=getdate() ' + CHAR(13) + CHAR(10)	
			
				SET	@sSET= @sSET + ',DataValidazioneUTC=GetUTCDate() ' + CHAR(13) + CHAR(10)

	END

		IF @xParametri.exist('/Parametri/TimeStamp/CodRuolo')=1
	BEGIN
			SET @sCodRuolo=(SELECT TOP 1 ValoreParametro.CodRuolo.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('Parametri/TimeStamp/CodRuolo') as ValoreParametro(CodRuolo))	
					  					
	END
	
			IF ISNULL(@bValidata,-1)=0 AND @sCodRuolo=@sCodRuoloSuperUser
	BEGIN		
				SET @sCodUtenteUltimaModifica='WebNotificationService'
				SET	@sSET= @sSET + ',DataValidazione=NULL ' + CHAR(13) + CHAR(10)	
			
				SET	@sSET= @sSET + ',DataValidazioneUTC=NULL ' + CHAR(13) + CHAR(10)
	END
	ELSE	
		BEGIN
						SET @sCodUtenteUltimaModifica=(SELECT TOP 1 ValoreParametro.CodUtenteUltimaModifica.value('.','VARCHAR(100)')
							  FROM @xParametri.nodes('/Parametri/TimeStamp/CodLogin') as ValoreParametro(CodUtenteUltimaModifica))	
			SET @sCodUtenteUltimaModifica=ISNULL(@sCodUtenteUltimaModifica,'')
		END
			
	IF @sCodUtenteUltimaModifica <> ''
		SET	@sSET= @sSET + ',CodUtenteUltimaModifica=''' + @sCodUtenteUltimaModifica + '''' + CHAR(13) + CHAR(10)		
	ELSE
		SET	@sSET= @sSET + ',CodUtenteUltimaModifica=NULL' + CHAR(13) + CHAR(10)	
		
			SET	@sSET= @sSET + ',DataUltimaModifica=getdate() ' + CHAR(13) + CHAR(10)	
		
		SET	@sSET= @sSET + ',DataUltimaModificaUTC=GetUTCDate() ' + CHAR(13) + CHAR(10)
			
		IF @xParametri.exist('/Parametri/CodStatoSchedaCalcolato')=1
	BEGIN
			SET @sCodStatoSchedaCalcolato=(SELECT TOP 1 ValoreParametro.CodStatoSchedaCalcolato.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodStatoSchedaCalcolato') as ValoreParametro(CodStatoSchedaCalcolato))	
					  
			IF @sCodStatoSchedaCalcolato <> ''
				SET	@sSET= @sSET + ',CodStatoSchedaCalcolato=''' + @sCodStatoSchedaCalcolato +''''	+ CHAR(13) + CHAR(10)				
			ELSE
				SET	@sSET= @sSET + ',CodStatoSchedaCalcolato=NULL'	+ CHAR(13) + CHAR(10)	
	END

		SET @bNuovaRevisione=0
	IF @xParametri.exist('/Parametri/NuovaRevisione')=1
	BEGIN			
			SET @bNuovaRevisione=(SELECT TOP 1 ValoreParametro.NuovaRevisione.value('.','BIT')
					  FROM @xParametri.nodes('/Parametri/NuovaRevisione') as ValoreParametro(NuovaRevisione))																
	END

		IF @uIDScheda IS NOT NULL
	BEGIN
		SET @bSchedaSemplice=(SELECT ISNULL(SchedaSemplice,0) FROM T_Schede WHERE Codice = (SELECT CodScheda FROM T_MovSchede WHERE ID=@uIDScheda))			
	END
	
					
			
	
			
		IF LEFT(@sSET,1)=',' 
		SET @sSET=RIGHT(@sSET,LEN(@sSET)-1)
			
	IF LTRIM(RTRIM(@sSET)) <> ''
		BEGIN			
				SET @sWHERE =' WHERE ID=''' + convert(varchar(50),@uIDScheda) +''''
				
				SET @sSQL=	ISNULL(@sSQL,'') + CHAR(13) + CHAR(10) + 
							ISNULL(@sSET,'') + CHAR(13) + CHAR(10) + 
							ISNULL(@sWHERE,'WHERE 1=0')														
				
				SET @xTimeStamp.modify('delete (/TimeStamp/IDEntita)[1]') 
						
				SET @xTimeStamp.modify('insert <IDEntita>{sql:variable("@uIDScheda")}</IDEntita> into (/TimeStamp)[1]')

				SET @xTimeStampBase=@xTimeStamp
	
				SET @xTimeStamp=CONVERT(XML,
									'<Parametri>' + CONVERT(varchar(max),@xTimeStamp) +
									'</Parametri>')
						
																							
				SET @xParLog=CONVERT(XML,'<Parametri><LogPrima/><LogDopo/></Parametri>')																		
					
				SET @xLogPrima=Convert(XML,'<DataSet></DataSet>')																						
					
				SET @xTemp=
					(SELECT * FROM 
						(SELECT * FROM T_MovSchede
						 WHERE ID=@uIDScheda											) AS [Table]
					FOR XML AUTO, ELEMENTS)				
				
				SET @xLogPrima.modify('insert sql:variable("@xTemp") as first into (/DataSet)[1]')
																			
				SET @xParLog.modify('insert sql:variable("@xLogPrima") as first into (/Parametri/LogPrima)[1]')
				
																						
	
				SET @uGUID=NEWID()

				BEGIN TRANSACTION
	
								INSERT INTO T_MovSchede
				  (ID
				   ,CodUA
				   ,CodEntita
				   ,IDEntita
				   ,IDPaziente
				   ,IDEpisodio
				   ,IDTrasferimento
				   ,CodScheda
				   ,Versione
				   ,Numero
				   ,Dati
				   ,AnteprimaRTF
				   ,AnteprimaTXT
				   ,DatiObbligatoriMancantiRTF
				   ,DatiRilievoRTF
				   ,IDSchedaPadre
				   ,Storicizzata
				   ,CodStatoScheda
				   ,DataCreazione
				   ,DataCreazioneUTC
				   ,CodUtenteRilevazione
				   ,CodUtenteUltimaModifica
				   ,DataUltimaModifica
				   ,DataUltimaModificaUTC
				   ,InEvidenza
				   ,Riservata
				   ,Validabile
				   ,Validata
				   ,CodUtenteValidazione
				   ,DataValidazione
				   ,DataValidazioneUTC
				   ,CodRuoloRilevazione
				   ,CodStatoSchedaCalcolato
				   )
				SELECT
					@uGUID AS ID										   ,CodUA
				   ,CodEntita
				   ,IDEntita
				   ,IDPaziente
				   ,IDEpisodio
				   ,IDTrasferimento
				   ,CodScheda
				   ,Versione
				   ,Numero
				   ,Dati
				   ,AnteprimaRTF
				   ,AnteprimaTXT
				   ,DatiObbligatoriMancantiRTF
				   ,DatiRilievoRTF
				   ,IDSchedaPadre
				   ,1 AS Storicizzata									   ,CodStatoScheda
				   ,DataCreazione
				   ,DataCreazioneUTC
				   ,CodUtenteRilevazione
				   ,CodUtenteUltimaModifica
				   ,DataUltimaModifica
				   ,DataUltimaModificaUTC
				   ,InEvidenza 
				   ,Riservata
				   ,Validabile
				   ,Validata
				   ,CodUtenteValidazione
				   ,DataValidazione
				   ,DataValidazioneUTC
				   ,CodRuoloRilevazione
				   ,CodStatoSchedaCalcolato
				 FROM T_MovSchede          
				 WHERE ID= @uIDScheda

								IF @uIDScheda IS NULL	
				BEGIN					
					PRINT @sSQL		
					EXEC (@sSQL)

					
					IF @@ERROR=0 AND @@ROWCOUNT >0
					BEGIN			
						EXEC MSP_InsMovTimeStamp @xTimeStamp		
					END	
					
				END	
				ELSE
					BEGIN									
						UPDATE T_MovSchede
						SET 
							CodUA=CASE 
										WHEN ISNULL(@sCodUA,'') <> '' THEN ISNULL(@sCodUA,CodUA)
										ELSE CodUA
								  END,		 
							CodEntita=ISNULL(@sCodEntita,CodEntita),
							IDEntita=ISNULL(@uIDEntita,IDEntita),
							IDPaziente=ISNULL(@uIDPaziente,IDPaziente),
							IDEpisodio=CASE 
											WHEN ISNULL(@sCodEntita,CodEntita) ='PAZ' THEN NULL
										    ELSE ISNULL(@uIDEpisodio,IDEpisodio)
										END,
							IDTrasferimento=
								CASE 
									WHEN  @bAnnullaIDTrasferimento=1 THEN NULL
									ELSE ISNULL(@uIDTrasferimento,IDTrasferimento)
								END,
							CodScheda=ISNULL(@sCodScheda,CodScheda),
							Versione=ISNULL(@nVersione,Versione),
							Numero=ISNULL(@nNumero,Numero),
							Dati=ISNULL(@xDati,Dati),
							AnteprimaRTF= ISNULL(CONVERT(VARCHAR(MAX),@binAnteprimaRTF),AnteprimaRTF),
							AnteprimaTXT=dbo.MF_PulisciTXT(ISNULL(CONVERT(VARCHAR(MAX),@binAnteprimaTXT),AnteprimaTXT)),
							DatiObbligatoriMancantiRTF=
										CASE 
											WHEN @bAggiornaNullDatiMancanti=1 THEN NULL
											ELSE 
												ISNULL(CONVERT(VARCHAR(MAX),@binDatiObbligatoriMancantiRTF),DatiObbligatoriMancantiRTF)
										END		
										 ,
							DatiRilievoRTF=
										CASE 
											WHEN @bAggiornaNullDatiRilievo=1 THEN NULL
											ELSE 
												ISNULL(CONVERT(VARCHAR(MAX),@binDatiRilievoRTF),DatiRilievoRTF)
										END		
										,
							IDSchedaPadre=
								CASE 
									WHEN @bAnnullaIDSchedaPadre=1 THEN NULL
									ELSE ISNULL(@uIDSchedaPadre,IDSchedaPadre)
								END,
														CodStatoScheda=ISNULL(@sCodStatoScheda,CodStatoScheda),
																												CodUtenteUltimaModifica=CASE
														WHEN ISNULL(@sCodUtenteUltimaModifica,'')='' THEN CodUtenteUltimaModifica
														ELSE @sCodUtenteUltimaModifica
													END,	
							DataUltimaModifica=GetDate(),
							DataUltimaModificaUTC=GetUTCDate(),
							InEvidenza=ISNULL(@bInEvidenza,InEvidenza),
							Validata=ISNULL(@bValidata,Validata),
							CodUtenteValidazione=CASE 
													WHEN @bValidata IS NULL THEN CodUtenteValidazione			
													WHEN @bValidata=1 THEN @sCodUtenteUltimaModifica
													WHEN @bValidata = 0 THEN NULL
													ELSE CodUtenteValidazione												
												  END,
							DataValidazione=CASE 
													WHEN @bValidata IS NULL THEN DataValidazione
													WHEN @bValidata =1 THEN GETDATE()
													WHEN @bValidata = 0 THEN NULL
													ELSE DataValidazione
												  END,					  	
							DataValidazioneUTC=CASE 
													WHEN @bValidata IS NULL THEN DataValidazioneUTC
													WHEN @bValidata =1 THEN GETUTCDATE()
													WHEN @bValidata = 0 THEN NULL
													ELSE DataValidazioneUTC
												  END,	
							CodStatoSchedaCalcolato=ISNULL(@sCodStatoSchedaCalcolato,CodStatoSchedaCalcolato),
							CodUtenteRilevazione = CASE 
														WHEN @bNuovaRevisione = 1 THEN @sCodUtenteUltimaModifica
														ELSE CodUtenteRilevazione
													END,
							DataCreazione = CASE 
														WHEN @bNuovaRevisione = 1 THEN GETDATE()
														ELSE DataCreazione
													END,
											
							DataCreazioneUTC = CASE 
														WHEN @bNuovaRevisione = 1 THEN GETUTCDATE()
														ELSE DataCreazioneUTC
													END							
						WHERE 
							ID= @uIDScheda
						
						IF @@ERROR=0 AND @@ROWCOUNT >0
						BEGIN			
							EXEC MSP_InsMovTimeStamp @xTimeStamp		
						END	
					
																								IF 	@sCodStatoScheda='CA' AND  
								@sCodEntita IS NOT NULL AND
								@uIDEntita IS NOT NULL AND
								@sCodScheda IS NOT NULL AND 
								@nVersione IS NOT NULL AND 
								@nNumero IS NOT NULL
						BEGIN							
							UPDATE T_MovSchede
								SET CodStatoScheda='CA'
							WHERE	
								CodEntita=@sCodEntita AND
								IDEntita= @uIDEntita AND
								CodScheda = @sCodScheda AND
								Versione = @nVersione AND
								Numero= @nNumero AND
								Storicizzata=1 AND
								CodStatoScheda <> 'CA'
							
						END														
					END	
															
				IF @@ERROR = 0
					BEGIN
					
												 COMMIT TRANSACTION
						
																												
							SET @xLogDopo=Convert(XML,'<DataSet></DataSet>')				
							
							SET @xTemp=
								(SELECT * FROM 
									(SELECT * FROM T_MovSchede
									 WHERE ID=@uIDScheda														) AS [Table]
								FOR XML AUTO, ELEMENTS)

							SET @xLogDopo.modify('insert sql:variable("@xTemp") as first into (/DataSet)[1]')																											
							SET @xParLog.modify('insert sql:variable("@xLogDopo") as first into (/Parametri/LogDopo)[1]')
							
														SET @xParLog.modify('insert sql:variable("@xTimeStampBase") as last into (/Parametri)[1]')
							
							IF @bSchedaSemplice=1 							
								BEGIN
																		SET  @sCodAzione=(SELECT TOP 1 ValoreParametro.CodAzione.value('.','VARCHAR(20)')
													  FROM @xParLog.nodes('/Parametri/TimeStamp/CodAzione') as ValoreParametro(CodAzione))	
									IF ISNULL(@sCodAzione,'')='VAL' 
									BEGIN
																				SET @xParLog.modify('delete (Parametri/TimeStamp/CodAzione)[1]') 			
										SET @xParLog.modify('insert <CodAzione>MOD</CodAzione> into (Parametri/TimeStamp)[1]')
									END
								END
		
							
														EXEC MSP_InsMovLog @xParLog
														
														
												END	
				ELSE
					BEGIN
						ROLLBACK TRANSACTION	
											END	 
		END			
		
	RETURN 0						 	
END