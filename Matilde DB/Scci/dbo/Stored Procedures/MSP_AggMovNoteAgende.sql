CREATE PROCEDURE [dbo].[MSP_AggMovNoteAgende](@xParametri XML)
AS
BEGIN
		
	
												
		DECLARE @uIDNotaAgenda AS UNIQUEIDENTIFIER		
		
	DECLARE @txtOggetto AS VARCHAR(MAX)	
	DECLARE @txtDescrizione AS VARCHAR(MAX)
	
	DECLARE @dDataInizio AS DATETIME	
	DECLARE @dDataFine AS DATETIME
	
	DECLARE @sCodStatoNota AS VARCHAR(20)
	DECLARE @sCodAgenda AS VARCHAR(20)
	DECLARE @txtColore AS VARCHAR(MAX)
	
	DECLARE @sCodUtenteRilevazione AS VARCHAR(100)
	DECLARE @sCodUtenteUltimaModifica AS VARCHAR(100)									
	
	DECLARE @bTuttoIlGiorno AS BIT	
	DECLARE @bEscludiDisponibilita AS BIT	

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
			
		SET @sSQL='UPDATE T_MovNoteAgende ' + CHAR(13) + CHAR(10) +
			  'SET '  				  						
			
	SET @sSET =''
	
	
		IF @xParametri.exist('(//IDNotaAgenda)')=1
	BEGIN
				SET @sGUID=(SELECT TOP 1 ValoreParametro.IDNotaAgenda.value('.','VARCHAR(50)')
						  FROM @xParametri.nodes('/Parametri/IDNotaAgenda') as ValoreParametro(IDNotaAgenda))
						  
		IF 	ISNULL(@sGUID,'') <> '' 
				SET @uIDNotaAgenda=CONVERT(UNIQUEIDENTIFIER,	@sGUID)			
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
	
		IF @xParametri.exist('/Parametri/CodAgenda')=1
		BEGIN
			SET @sCodAgenda=(SELECT TOP 1 ValoreParametro.CodAgenda.value('.','VARCHAR(20)')
						  FROM @xParametri.nodes('/Parametri/CodAgenda') as ValoreParametro(CodAgenda))	
						  
			IF @sCodAgenda <> ''
					SET	@sSET= @sSET + ',CodAgenda=''' + @sCodAgenda+''''	+ CHAR(13) + CHAR(10)				
				ELSE
					SET	@sSET= @sSET + ',CodAgenda=NULL'	+ CHAR(13) + CHAR(10)		
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
		
	
		IF @xParametri.exist('/Parametri/TuttoIlGiorno')=1
		BEGIN
			SET @bTuttoIlGiorno=(SELECT TOP 1 ValoreParametro.TuttoIlGiorno.value('.','BIT')
							  FROM @xParametri.nodes('/Parametri/TuttoIlGiorno') as ValoreParametro(TuttoIlGiorno))
							  									
				SET	@sSET= @sSET + ',TuttoIlGiorno=' + CONVERT(VARCHAR(1),@bTuttoIlGiorno) + CHAR(13) + CHAR(10)	
				
		END	

		IF @xParametri.exist('/Parametri/EscludiDisponibilita')=1
		BEGIN
			SET @bEscludiDisponibilita=(SELECT TOP 1 ValoreParametro.EscludiDisponibilita.value('.','BIT')
							  FROM @xParametri.nodes('/Parametri/EscludiDisponibilita') as ValoreParametro(EscludiDisponibilita))
							  									
			SET	@sSET= @sSET + ',EscludiDisponibilita=' + CONVERT(VARCHAR(1),@bEscludiDisponibilita) + CHAR(13) + CHAR(10)					
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
																SET @sWHERE =' WHERE ID=''' + convert(varchar(50),@uIDNotaAgenda) +''''
					
								SET @sSQL=	ISNULL(@sSQL,'') + CHAR(13) + CHAR(10) + 
							ISNULL(@sSET,'') + CHAR(13) + CHAR(10) + 
							ISNULL(@sWHERE,'')			


																
								SET @xTimeStamp.modify('delete (/TimeStamp/IDEntita)[1]') 
						
				SET @xTimeStamp.modify('insert <IDEntita>{sql:variable("@uIDNotaAgenda")}</IDEntita> into (/TimeStamp)[1]')

				SET @xTimeStampBase=@xTimeStamp
	
				SET @xTimeStamp=CONVERT(XML,
									'<Parametri>' + CONVERT(varchar(max),@xTimeStamp) +
									'</Parametri>')
						
																
				SET @xParLog=CONVERT(XML,'<Parametri><LogPrima/><LogDopo/></Parametri>')										
				
				SET @xLogPrima=Convert(XML,'<DataSet></DataSet>')														
				
				SET @xTemp=
					(SELECT * FROM 
						(SELECT * FROM T_MovNoteAgende
						 WHERE ID=@uIDNotaAgenda											) AS [Table]
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
									(SELECT * FROM T_MovNoteAgende
									 WHERE ID=@uIDNotaAgenda														) AS [Table]
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