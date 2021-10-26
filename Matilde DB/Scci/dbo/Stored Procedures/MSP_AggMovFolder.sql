CREATE PROCEDURE [dbo].[MSP_AggMovFolder](@xParametri XML)
AS
BEGIN
		
	
											
	DECLARE @uIDFolder AS UNIQUEIDENTIFIER	
	DECLARE @uIDPaziente AS UNIQUEIDENTIFIER	
	DECLARE @uIDEpisodio AS UNIQUEIDENTIFIER	
	DECLARE @uIDTrasferimento AS UNIQUEIDENTIFIER	
	DECLARE @uIDFolderPadre AS UNIQUEIDENTIFIER	
	DECLARE @sDescrizione AS VARCHAR(255)
	DECLARE @sCodStatoFolder AS VARCHAR(20)
	DECLARE @sCodUA AS VARCHAR(20)
	DECLARE @sCodEntita AS VARCHAR(20)
			
	DECLARE @txtDocumento VARCHAR(MAX)	
	DECLARE @sEstensione AS VARCHAR(10)	
	DECLARE @txtTestoRTF VARCHAR(MAX)		
	DECLARE @txtNotaRTF VARCHAR(MAX)
	DECLARE @txtNomeFile VARCHAR(MAX)		
	
	DECLARE @sCodAzione AS VARCHAR(20)										
	DECLARE @sCodUtenteUltimaModifica AS VARCHAR(100)					
	
	
		DECLARE @sGUID AS VARCHAR(50)
	DECLARE @uGUID AS UNIQUEIDENTIFIER
	DECLARE @sDataTmp AS VARCHAR(20)	

	DECLARE @sSQL AS VARCHAR(MAX)
	DECLARE @sSET AS VARCHAR(MAX)
	DECLARE @sWHERE AS VARCHAR(MAX)
	
	    DECLARE @xTimeStampBase AS XML	
	DECLARE @xTimeStamp AS XML	
	
		DECLARE @xLogPrima AS XML
	DECLARE @xLogDopo AS XML
	DECLARE @xTemp AS XML
	DECLARE @xParLog AS XML
	
		SET @sSQL='UPDATE T_MovFolder ' + CHAR(13) + CHAR(10) +
			  'SET '  				  						
			  					
	SET @sSET =''
	
		IF @xParametri.exist('/Parametri/IDFolder')=1
		BEGIN
			SET @sGUID=(SELECT TOP 1 ValoreParametro.IDFolder.value('.','VARCHAR(50)')
							  FROM @xParametri.nodes('/Parametri/IDFolder') as ValoreParametro(IDFolder))
			IF 	ISNULL(@sGUID,'') <> '' SET @uIDFolder=CONVERT(UNIQUEIDENTIFIER, @sGUID)	
					END

		IF @xParametri.exist('/Parametri/IDPaziente')=1
		BEGIN
			SET @sGUID=(SELECT TOP 1 ValoreParametro.IDPaziente.value('.','VARCHAR(50)')
							  FROM @xParametri.nodes('/Parametri/IDPaziente') as ValoreParametro(IDPaziente))
							  
				IF 	ISNULL(@sGUID,'') <> '' 
				BEGIN
					SET @uIDPaziente=CONVERT(UNIQUEIDENTIFIER,	@sGUID)		
					SET	@sSET= @sSET + ',IDPaziente=''' + convert(VARCHAR(50),@uIDPaziente) + '''' + CHAR(13) + CHAR(10)	
				END
				ELSE
					SET	@sSET= @sSET + ',IDPaziente=NULL' + CHAR(13) + CHAR(10)									  		
		END
							
		IF @xParametri.exist('/Parametri/IDEpisodio')=1
		BEGIN
			SET @sGUID=(SELECT TOP 1 ValoreParametro.IDEpisodio.value('.','VARCHAR(50)')
							  FROM @xParametri.nodes('/Parametri/IDEpisodio') as ValoreParametro(IDEpisodio))
							  
				IF 	ISNULL(@sGUID,'') <> '' 
				BEGIN
					SET @uIDEpisodio=CONVERT(UNIQUEIDENTIFIER,	@sGUID)		
					SET	@sSET= @sSET + ',IDEpisodio=''' + convert(VARCHAR(50),@uIDEpisodio) + '''' + CHAR(13) + CHAR(10)	
				END
				ELSE
					SET	@sSET= @sSET + ',IDEpisodio=NULL' + CHAR(13) + CHAR(10)									  		
		END			

		IF @xParametri.exist('/Parametri/IDTrasferimento')=1
		BEGIN
			SET @sGUID=(SELECT TOP 1 ValoreParametro.IDTrasferimento.value('.','VARCHAR(50)')
							  FROM @xParametri.nodes('/Parametri/IDTrasferimento') as ValoreParametro(IDTrasferimento))
							  
				IF 	ISNULL(@sGUID,'') <> '' 
				BEGIN
					SET @uIDTrasferimento=CONVERT(UNIQUEIDENTIFIER,	@sGUID)		
					SET	@sSET= @sSET + ',IDTrasferimento=''' + convert(VARCHAR(50),@uIDTrasferimento) + '''' + CHAR(13) + CHAR(10)	
				END
				ELSE
					SET	@sSET= @sSET + ',IDTrasferimento=NULL' + CHAR(13) + CHAR(10)									  		
		END		

		IF @xParametri.exist('/Parametri/IDFolderPadre')=1
		BEGIN
			SET @sGUID=(SELECT TOP 1 ValoreParametro.IDFolderPadre.value('.','VARCHAR(50)')
							  FROM @xParametri.nodes('/Parametri/IDFolderPadre') as ValoreParametro(IDFolderPadre))
							  
				IF 	ISNULL(@sGUID,'') <> '' 
				BEGIN
					SET @uIDFolderPadre=CONVERT(UNIQUEIDENTIFIER,	@sGUID)		
					SET	@sSET= @sSET + ',IDFolderPadre=''' + convert(VARCHAR(50),@uIDFolderPadre) + '''' + CHAR(13) + CHAR(10)	
				END
				ELSE
					SET	@sSET= @sSET + ',IDFolderPadre=NULL' + CHAR(13) + CHAR(10)									  		
		END	
						
		IF @xParametri.exist('/Parametri/Descrizione')=1
	BEGIN
		SET @sDescrizione=(SELECT TOP 1 ValoreParametro.Descrizione.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/Descrizione') as ValoreParametro(Descrizione))	
					  
		IF @sDescrizione <> ''
				SET	@sSET= @sSET + ',Descrizione=''' + @sDescrizione +''''	+ CHAR(13) + CHAR(10)				
			ELSE
				SET	@sSET= @sSET + ',Descrizione=NULL'	+ CHAR(13) + CHAR(10)		
	END	
	
		IF @xParametri.exist('/Parametri/CodStatoFolder')=1
	BEGIN
		SET @sCodStatoFolder=(SELECT TOP 1 ValoreParametro.CodStatoFolder.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodStatoFolder') as ValoreParametro(CodStatoFolder))	
					  
		IF @sCodStatoFolder <> ''
				SET	@sSET= @sSET + ',CodStatoFolder=''' + @sCodStatoFolder +''''	+ CHAR(13) + CHAR(10)				
			ELSE
				SET	@sSET= @sSET + ',CodStatoFolder=NULL'	+ CHAR(13) + CHAR(10)		
	END	
	
		IF @xParametri.exist('/Parametri/CodUA')=1
	BEGIN
		SET @sCodUA=(SELECT TOP 1 ValoreParametro.CodUA.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodUA') as ValoreParametro(CodUA))	
					  
		IF @sCodUA <> ''
				SET	@sSET= @sSET + ',CodUA=''' + @sCodUA + ''''	+ CHAR(13) + CHAR(10)				
			ELSE
				SET	@sSET= @sSET + ',CodUA=NULL'	+ CHAR(13) + CHAR(10)		
	END	

		IF @xParametri.exist('/Parametri/CodEntita')=1
	BEGIN
		SET @sCodEntita=(SELECT TOP 1 ValoreParametro.CodEntita.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodEntita') as ValoreParametro(CodEntita))	
					  
		IF @sCodEntita <> ''
				SET	@sSET= @sSET + ',CodEntita=''' + @sCodEntita + ''''	+ CHAR(13) + CHAR(10)				
			ELSE
				SET	@sSET= @sSET + ',CodEntita=NULL'	+ CHAR(13) + CHAR(10)		
	END	

		SET @sCodUtenteUltimaModifica=(SELECT TOP 1 ValoreParametro.CodUtenteUltimaModifica.value('.','VARCHAR(100)')
					  FROM @xParametri.nodes('/Parametri/TimeStamp/CodLogin') as ValoreParametro(CodUtenteUltimaModifica))	
	SET @sCodUtenteUltimaModifica=ISNULL(@sCodUtenteUltimaModifica,'')
	
	IF @sCodUtenteUltimaModifica <> ''
		SET	@sSET= @sSET + ',CodUtenteUltimaModifica=''' + @sCodUtenteUltimaModifica + '''' + CHAR(13) + CHAR(10)		
	ELSE
		SET	@sSET= @sSET + ',CodUtenteUltimaModifica=NULL' + CHAR(13) + CHAR(10)	

		SET	@sSET= @sSET + ',DataUltimaModifica=GetDate()'	+ CHAR(13) + CHAR(10)	
		SET	@sSET= @sSET + ',DataUltimaModificaUTC=GetUTCDate()'	+ CHAR(13) + CHAR(10)					
							   
	    SET @xTimeStamp=(SELECT TOP 1 ValoreParametro.TS.query('.')
					  FROM @xParametri.nodes('/Parametri/TimeStamp') as ValoreParametro(TS))
		
				
		IF LEFT(@sSET,1)=',' 
		SET @sSET=RIGHT(@sSET,LEN(@sSET)-1)
	
	IF @sSET <> ''		
		BEGIN
						
								SET @sWHERE =' WHERE ID=''' + convert(varchar(50),@uIDFolder) +''''
				
								SET @sSQL=	ISNULL(@sSQL,'') + CHAR(13) + CHAR(10) + 
						ISNULL(@sSET,'') + CHAR(13) + CHAR(10) + 
						ISNULL(@sWHERE,'')	
																				SET @xTimeStamp.modify('delete (/TimeStamp/IDEntita)[1]') 
						
				SET @xTimeStamp.modify('insert <IDEntita>{sql:variable("@uIDFolder")}</IDEntita> into (/TimeStamp)[1]')

								SET @xTimeStampBase=@xTimeStamp
		
	
				SET @xTimeStamp=CONVERT(XML,
									'<Parametri>' + CONVERT(varchar(max),@xTimeStamp) +
									'</Parametri>')

																
				SET @xParLog=CONVERT(XML,'<Parametri><LogPrima/><LogDopo/></Parametri>')										
				
				SET @xLogPrima=Convert(XML,'<DataSet></DataSet>')														
				
				SET @xTemp=
					(SELECT * FROM 
						(SELECT 
								  ID
								  ,IDNum
								  ,IDPaziente
								  ,IDEpisodio
								  ,IDTrasferimento
								  ,IDFolderPadre
								  ,Descrizione
								  ,CodStatoFolder
								  ,CodEntita
								  ,CodUA
								  ,CodUtenteRilevazione
								  ,CodUtenteUltimaModifica
								  ,DataUltimaModifica
								  ,DataUltimaModificaUTC  
						 FROM T_MovFolder
						 WHERE ID=@uIDFolder							) AS [Table]
					FOR XML AUTO, ELEMENTS)

				SET @xLogPrima.modify('insert sql:variable("@xTemp") as first into (/DataSet)[1]')
																			
				SET @xParLog.modify('insert sql:variable("@xLogPrima") as first into (/Parametri/LogPrima)[1]')
																										
				BEGIN TRANSACTION
					PRINT @sSQL
					EXEC (@sSQL)
							
				IF @@ERROR=0 AND @@ROWCOUNT > 0
					BEGIN
												EXEC MSP_InsMovTimeStamp @xTimeStamp		
					END	
				IF @@ERROR = 0
					BEGIN
						COMMIT TRANSACTION
						
																				
					SET @xLogDopo=Convert(XML,'<DataSet></DataSet>')				
					
					SET @xTemp=
						(SELECT * FROM 
							(SELECT
								  ID
								  ,IDNum
								  ,IDPaziente
								  ,IDEpisodio
								  ,IDTrasferimento
								  ,IDFolderPadre
								  ,Descrizione
								  ,CodStatoFolder
								  ,CodEntita
								  ,CodUA
								  ,CodUtenteRilevazione
								  ,CodUtenteUltimaModifica
								  ,DataUltimaModifica
								  ,DataUltimaModificaUTC  
							 FROM T_MovFolder
							 WHERE ID=@uIDFolder								) AS [Table]
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