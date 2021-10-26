

CREATE PROCEDURE [dbo].[MSP_AggMovNote](@xParametri XML)
AS
BEGIN
		
	
												
		DECLARE @uIDNota AS UNIQUEIDENTIFIER	
	DECLARE @uIDPaziente AS UNIQUEIDENTIFIER		
	DECLARE @uIDEpisodio AS UNIQUEIDENTIFIER		
	DECLARE @uIDTrasferimento AS UNIQUEIDENTIFIER			
		
	DECLARE @txtOggetto AS VARCHAR(MAX)	
	DECLARE @txtDescrizione AS VARCHAR(MAX)
	
	DECLARE @dDataInizio AS DATETIME	
	DECLARE @dDataFine AS DATETIME
	
	DECLARE @sCodStatoNota AS VARCHAR(20)
	DECLARE @sCodEntita AS VARCHAR(20)
	DECLARE @uIDEntita AS UNIQUEIDENTIFIER
	DECLARE @sCodSezione AS VARCHAR(20)
	DECLARE @sCodVoce AS VARCHAR(600)
	DECLARE @uIDGruppo AS UNIQUEIDENTIFIER
	DECLARE @txtColore AS VARCHAR(MAX)
	
	DECLARE @sCodUtenteRilevazione AS VARCHAR(100)
	DECLARE @sCodUtenteUltimaModifica AS VARCHAR(100)									
	
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
			
		SET @sSQL='UPDATE T_MovNote ' + CHAR(13) + CHAR(10) +
			  'SET '  				  						
			
	SET @sSET =''
	
	
		IF @xParametri.exist('(//IDNota)')=1
	BEGIN
				SET @sGUID=(SELECT TOP 1 ValoreParametro.IDNota.value('.','VARCHAR(50)')
						  FROM @xParametri.nodes('/Parametri/IDNota') as ValoreParametro(IDNota))
						  
		IF 	ISNULL(@sGUID,'') <> '' 
				SET @uIDNota=CONVERT(UNIQUEIDENTIFIER,	@sGUID)			
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
	

	
		IF @xParametri.exist('/Parametri/IDTrasferimento')=1
		BEGIN
			SET @sGUID=(SELECT TOP 1 ValoreParametro.IDTrasferimento.value('.','VARCHAR(50)')
							  FROM @xParametri.nodes('/Parametri/IDTrasferimento') as ValoreParametro(IDTrasferimento))
							  
				IF 	ISNULL(@sGUID,'') <> '' 
				BEGIN
					SET @uIDTrasferimento=CONVERT(UNIQUEIDENTIFIER,	@sGUID)		
					SET	@sSET= @sSET + ',IDTrasferimento=''' + convert(VARCHAR(50),@uIDTrasferimento) + ''''	+ CHAR(13) + CHAR(10)	
				END
				ELSE
					SET	@sSET= @sSET + ',IDTrasferimento=NULL'	+ CHAR(13) + CHAR(10)									  		
		END	
					
		IF @xParametri.exist('/Parametri/DataInizio')=1
		BEGIN			
			SET @sDataTmp=(SELECT TOP 1 ValoreParametro.DataInizio.value('.','VARCHAR(20)')
							  FROM @xParametri.nodes('/Parametri/DataInizio') as ValoreParametro(DataInizio))					  
			SET @sDataTmp=ISNULL(@sDataTmp,'')
			
			IF @sDataTmp<> '' 
				BEGIN			
										SET @sDataTmp= SUBSTRING(@sDataTmp,4,2) + '-' 
								+ LEFT(@sDataTmp,2) + '-' +
								+ SUBSTRING(@sDataTmp,7,4) +
								' ' + RIGHT(@sDataTmp,5)
					IF ISDATE(@sDataTmp)=1
						SET	@dDataInizio=CONVERT(DATETIME,@sDataTmp,120)						
					ELSE
						SET	@dDataInizio =NULL			
				END
			IF @dDataInizio IS NOT NULL
				SET	@sSET= @sSET + ',DataInizio=CONVERT(DateTime,''' + CONVERT(VARCHAR(20),@dDataInizio,120) + ''',120)' + CHAR(13) + CHAR(10)		 
			ELSE	
				SET	@sSET= @sSET + ',DataInizio=NULL' + CHAR(13) + CHAR(10)		 
		END	
			
		IF @xParametri.exist('/Parametri/DataFine')=1
		BEGIN			
			SET @sDataTmp=(SELECT TOP 1 ValoreParametro.DataFine.value('.','VARCHAR(20)')
							  FROM @xParametri.nodes('/Parametri/DataFine') as ValoreParametro(DataFine))					  
			SET @sDataTmp=ISNULL(@sDataTmp,'')
			
			IF @sDataTmp<> '' 
				BEGIN			
										SET @sDataTmp= SUBSTRING(@sDataTmp,4,2) + '-' 
								+ LEFT(@sDataTmp,2) + '-' +
								+ SUBSTRING(@sDataTmp,7,4) +
								' ' + RIGHT(@sDataTmp,5)
					IF ISDATE(@sDataTmp)=1
						SET	@dDataFine=CONVERT(DATETIME,@sDataTmp,120)						
					ELSE
						SET	@dDataFine =NULL			
				END
			IF @dDataFine IS NOT NULL
				SET	@sSET= @sSET + ',DataFine=CONVERT(DateTime,''' + CONVERT(VARCHAR(20),@dDataFine,120) + ''',120)' + CHAR(13) + CHAR(10)		 
			ELSE	
				SET	@sSET= @sSET + ',DataFine=NULL' + CHAR(13) + CHAR(10)		 
		END	
		
		IF @xParametri.exist('/Parametri/Oggetto')=1
	BEGIN	
		SET @txtOggetto=(SELECT TOP 1 ValoreParametro.Oggetto.value('.','VARCHAR(MAX)')
				  FROM @xParametri.nodes('/Parametri/Oggetto') as ValoreParametro(Oggetto))					  

		IF @txtOggetto <> ''
			SET	@sSET= @sSET +',Oggetto=
										CONVERT(varchar(max),
												CAST(N'''' AS xml).value(''xs:base64Binary("' + @txtOggetto
												+ '")'', ''varbinary(max)'') 
											)'  + CHAR(13) + CHAR(10)	
		ELSE
			SET	@sSET= @sSET +',Oggetto=NULL '	+ CHAR(13) + CHAR(10)														
	END	
					

		IF @xParametri.exist('/Parametri/Descrizione')=1
	BEGIN	
		SET @txtDescrizione=(SELECT TOP 1 ValoreParametro.Descrizione.value('.','VARCHAR(MAX)')
				  FROM @xParametri.nodes('/Parametri/Descrizione') as ValoreParametro(Descrizione))					  

		IF @txtDescrizione <> ''
			SET	@sSET= @sSET +',Descrizione=
										CONVERT(varchar(max),
												CAST(N'''' AS xml).value(''xs:base64Binary("' + @txtDescrizione
												+ '")'', ''varbinary(max)'') 
											)'  + CHAR(13) + CHAR(10)	
		ELSE
			SET	@sSET= @sSET +',Descrizione=NULL '	+ CHAR(13) + CHAR(10)														
	END	
	
		IF @xParametri.exist('/Parametri/CodEntita')=1
		BEGIN
			SET @sCodEntita=(SELECT TOP 1 ValoreParametro.CodEntita.value('.','VARCHAR(20)')
						  FROM @xParametri.nodes('/Parametri/CodEntita') as ValoreParametro(CodEntita))	
						  
			IF @sCodEntita <> ''
					SET	@sSET= @sSET + ',CodEntita=''' + @sCodEntita+''''	+ CHAR(13) + CHAR(10)				
				ELSE
					SET	@sSET= @sSET + ',CodEntita=NULL'	+ CHAR(13) + CHAR(10)		
		END	

		IF @xParametri.exist('(//IDEntita)')=1
	BEGIN
				SET @sGUID=(SELECT TOP 1 ValoreParametro.IDEntita.value('.','VARCHAR(50)')
						  FROM @xParametri.nodes('/Parametri/IDEntita') as ValoreParametro(IDEntita))
						  
		IF 	ISNULL(@sGUID,'') <> '' 
				SET @uIDEntita=CONVERT(UNIQUEIDENTIFIER,	@sGUID)			
			END
				
		IF @xParametri.exist('/Parametri/Colore')=1
	BEGIN	
		SET @txtColore=(SELECT TOP 1 ValoreParametro.Colore.value('.','VARCHAR(MAX)')
				  FROM @xParametri.nodes('/Parametri/Colore') as ValoreParametro(Colore))					  

		IF @txtColore <> ''
			SET	@sSET= @sSET +',Colore=
										CONVERT(varchar(max),
												CAST(N'''' AS xml).value(''xs:base64Binary("' + @txtColore
												+ '")'', ''varbinary(max)'') 
											)'  + CHAR(13) + CHAR(10)	
		ELSE
			SET	@sSET= @sSET +',Colore=NULL '	+ CHAR(13) + CHAR(10)														
	END	
					  
		IF @xParametri.exist('/Parametri/CodStatoNota')=1
		BEGIN
			SET @sCodStatoNota=(SELECT TOP 1 ValoreParametro.CodStatoNota.value('.','VARCHAR(20)')
						  FROM @xParametri.nodes('/Parametri/CodStatoNota') as ValoreParametro(CodStatoNota))	
						  
			IF @sCodStatoNota <> ''
					SET	@sSET= @sSET + ',CodStatoNota=''' + @sCodStatoNota+''''	+ CHAR(13) + CHAR(10)				
				ELSE
					SET	@sSET= @sSET + ',CodStatoNota=NULL'	+ CHAR(13) + CHAR(10)		
		END	
		
		IF @xParametri.exist('/Parametri/CodSezione')=1
		BEGIN
			SET @sCodSezione=(SELECT TOP 1 ValoreParametro.CodSezione.value('.','VARCHAR(20)')
						  FROM @xParametri.nodes('/Parametri/CodSezione') as ValoreParametro(CodSezione))	
						  
			IF @sCodSezione <> ''
					SET	@sSET= @sSET + ',CodSezione=''' + @sCodSezione+''''	+ CHAR(13) + CHAR(10)				
				ELSE
					SET	@sSET= @sSET + ',CodSezione=NULL'	+ CHAR(13) + CHAR(10)		
		END			

		IF @xParametri.exist('/Parametri/CodVoce')=1
		BEGIN
			SET @sCodVoce=(SELECT TOP 1 ValoreParametro.CodVoce.value('.','VARCHAR(600)')
						  FROM @xParametri.nodes('/Parametri/CodVoce') as ValoreParametro(CodVoce))	
						  
			IF @sCodVoce <> ''
					SET	@sSET= @sSET + ',CodVoce=''' + @sCodVoce+''''	+ CHAR(13) + CHAR(10)				
				ELSE
					SET	@sSET= @sSET + ',CodVoce=NULL'	+ CHAR(13) + CHAR(10)		
		END			
		
		IF @xParametri.exist('/Parametri/IDGruppo')=1
		BEGIN
			SET @sGUID=(SELECT TOP 1 ValoreParametro.IDGruppo.value('.','VARCHAR(50)')
							  FROM @xParametri.nodes('/Parametri/IDGruppo') as ValoreParametro(IDGruppo))
							  
				IF 	ISNULL(@sGUID,'') <> '' 
				BEGIN							
					SET	@sSET= @sSET + ',IDGruppo=''' + @sGUID + ''''	+ CHAR(13) + CHAR(10)	
				END
				ELSE
					SET	@sSET= @sSET + ',IDGruppo=NULL'	+ CHAR(13) + CHAR(10)									  		
		END		
						
		SET @sCodUtenteUltimaModifica=(SELECT TOP 1 ValoreParametro.CodUtenteUltimaModifica.value('.','VARCHAR(100)')
					  FROM @xParametri.nodes('/Parametri/TimeStamp/CodLogin') as ValoreParametro(CodUtenteUltimaModifica))	
	SET @sCodUtenteUltimaModifica=ISNULL(@sCodUtenteUltimaModifica,'')
		
	IF @sCodUtenteUltimaModifica <> ''
		SET	@sSET= @sSET + ',CodUtenteUltimaModifica=''' + @sCodUtenteUltimaModifica + '''' + CHAR(13) + CHAR(10)		
	ELSE
		SET	@sSET= @sSET + ',CodUtenteUltimaModifica=NULL' + CHAR(13) + CHAR(10)	
	
		SET	@sSET= @sSET + ',DataUltimaModifica=getdate() ' + CHAR(13) + CHAR(10)	
		
		SET	@sSET= @sSET + ',DataUltimaModificaUTC=getdate() ' + CHAR(13) + CHAR(10)	
					
	    SET @xTimeStamp=(SELECT TOP 1 ValoreParametro.TS.query('.')
					  FROM @xParametri.nodes('/Parametri/TimeStamp') as ValoreParametro(TS))
								
			
			
		IF LEFT(@sSET,1)=',' 
		SET @sSET=RIGHT(@sSET,LEN(@sSET)-1)


	IF LTRIM(RTRIM(@sSET)) <> ''
		BEGIN
																SET @sWHERE =' WHERE ID=''' + convert(varchar(50),@uIDNota) +''''
					
								SET @sSQL=	ISNULL(@sSQL,'') + CHAR(13) + CHAR(10) + 
							ISNULL(@sSET,'') + CHAR(13) + CHAR(10) + 
							ISNULL(@sWHERE,'')			


																
								SET @xTimeStamp.modify('delete (/TimeStamp/IDEntita)[1]') 
						
				SET @xTimeStamp.modify('insert <IDEntita>{sql:variable("@uIDNota")}</IDEntita> into (/TimeStamp)[1]')

				SET @xTimeStampBase=@xTimeStamp
	
				SET @xTimeStamp=CONVERT(XML,
									'<Parametri>' + CONVERT(varchar(max),@xTimeStamp) +
									'</Parametri>')
						
																
				SET @xParLog=CONVERT(XML,'<Parametri><LogPrima/><LogDopo/></Parametri>')										
				
				SET @xLogPrima=Convert(XML,'<DataSet></DataSet>')														
				
				SET @xTemp=
					(SELECT * FROM 
						(SELECT * FROM T_MovNote
						 WHERE ID=@uIDNota											) AS [Table]
					FOR XML AUTO, ELEMENTS)

				SET @xLogPrima.modify('insert sql:variable("@xTemp") as first into (/DataSet)[1]')
																			
				SET @xParLog.modify('insert sql:variable("@xLogPrima") as first into (/Parametri/LogPrima)[1]')
				SET @xParLog.modify('insert sql:variable("@xTimeStampBase") as last into (/Parametri)[1]')
																						
				BEGIN TRANSACTION
										PRINT @sSQL
					EXEC (@sSQL)
						
				IF @@ERROR=0 AND @@ROWCOUNT >0
					BEGIN			
						EXEC MSP_InsMovTimeStamp @xTimeStamp		
					END	
				IF @@ERROR = 0
					BEGIN
					
												COMMIT TRANSACTION
						
																												
							SET @xLogDopo=Convert(XML,'<DataSet></DataSet>')				
							
							SET @xTemp=
								(SELECT * FROM 
									(SELECT * FROM T_MovNote
									 WHERE ID=@uIDNota														) AS [Table]
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