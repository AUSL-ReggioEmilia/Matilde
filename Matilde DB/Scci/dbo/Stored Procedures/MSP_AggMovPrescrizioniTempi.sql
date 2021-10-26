

CREATE PROCEDURE [dbo].[MSP_AggMovPrescrizioniTempi](@xParametri XML)
AS
BEGIN
		
	
											
		DECLARE @uIDPrescrizioneTempi AS UNIQUEIDENTIFIER		
	DECLARE @uIDPrescrizione AS UNIQUEIDENTIFIER		
	DECLARE @sCodStatoPrescrizioneTempi AS VARCHAR(20)
	DECLARE @sCodTipoPrescrizioneTempi AS VARCHAR(20)
	DECLARE @txtPosologia AS VARCHAR(MAX)
		
	DECLARE @bAlBisogno AS BIT
	DECLARE @dDataOraInizio AS DATETIME	
	DECLARE @dDataOraFine AS DATETIME
		
	DECLARE @nDurata AS INT
	DECLARE @bContinuita AS BIT
	DECLARE @nPeriodicitaGiorni AS INTEGER
	DECLARE @nPeriodicitaOre AS INTEGER
	DECLARE @nPeriodicitaMinuti AS INTEGER	
	DECLARE @txtCodProtocollo AS VARCHAR(MAX)
	DECLARE @bManuale AS BIT		
	DECLARE @xTempiManuali AS XML
	DECLARE @txtTempiManuali AS VARCHAR(MAX)	
	
		
	DECLARE @sCodUtenteUltimaModifica AS VARCHAR(100)					
	
	
		DECLARE @sGUID AS VARCHAR(50)
	DECLARE @uGUID AS UNIQUEIDENTIFIER
	DECLARE @sDataTmp AS VARCHAR(20)	
	DECLARE @sTmp AS VARCHAR(20)	

	DECLARE @sCodUtenteValidazione AS VARCHAR(100)
	DECLARE @sCodUtenteSospensione AS VARCHAR(100)
	
	DECLARE @sSQL AS VARCHAR(MAX)
	DECLARE @sSET AS VARCHAR(MAX)
	DECLARE @sWHERE AS VARCHAR(MAX)
	
	    DECLARE @xTimeStampBase AS XML	
	DECLARE @xTimeStamp AS XML	
	
		DECLARE @xSchedaMovimento AS XML
	
		DECLARE @xLogPrima AS XML
	DECLARE @xLogDopo AS XML
	DECLARE @xTemp AS XML
	DECLARE @xParLog AS XML
	
	

		
		SET @sSQL='UPDATE T_MovPrescrizioniTempi ' + CHAR(13) + CHAR(10) +
			  'SET '  				  						
			  					
	SET @sSET =''
	
		IF @xParametri.exist('/Parametri/IDPrescrizioneTempi')=1
		BEGIN
			SET @sGUID=(SELECT TOP 1 ValoreParametro.IDPrescrizioneTempi.value('.','VARCHAR(50)')
							  FROM @xParametri.nodes('/Parametri/IDPrescrizioneTempi') as ValoreParametro(IDPrescrizioneTempi))
			IF 	ISNULL(@sGUID,'') <> '' 
					SET @uIDPrescrizioneTempi=CONVERT(UNIQUEIDENTIFIER,	@sGUID)	
					END
	
		IF @xParametri.exist('/Parametri/IDPrescrizione')=1
		BEGIN
			SET @sGUID=(SELECT TOP 1 ValoreParametro.IDPrescrizione.value('.','VARCHAR(50)')
							  FROM @xParametri.nodes('/Parametri/IDPrescrizione') as ValoreParametro(IDPrescrizione))
			IF 	ISNULL(@sGUID,'') <> '' 
				BEGIN
					SET @uIDPrescrizione=CONVERT(UNIQUEIDENTIFIER,	@sGUID)		
					SET	@sSET= @sSET + ',IDPrescrizione=''' + convert(VARCHAR(50),@uIDPrescrizione) + ''''	+ CHAR(13) + CHAR(10)	
				END
				ELSE
					SET	@sSET= @sSET + ',IDPrescrizione=NULL'	+ CHAR(13) + CHAR(10)	
					END

		IF @xParametri.exist('/Parametri/CodStatoPrescrizioneTempi')=1
	BEGIN
		SET @sCodStatoPrescrizioneTempi=(SELECT TOP 1 ValoreParametro.CodStatoPrescrizioneTempi.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodStatoPrescrizioneTempi') as ValoreParametro(CodStatoPrescrizioneTempi))	
					  
		IF @sCodStatoPrescrizioneTempi <> ''
				SET	@sSET= @sSET + ',CodStatoPrescrizioneTempi=''' + @sCodStatoPrescrizioneTempi +''''	+ CHAR(13) + CHAR(10)				
			ELSE
				SET	@sSET= @sSET + ',CodStatoPrescrizioneTempi=NULL'	+ CHAR(13) + CHAR(10)		
	END	
	
		IF @xParametri.exist('/Parametri/CodTipoPrescrizioneTempi')=1
	BEGIN
		SET @sCodTipoPrescrizioneTempi=(SELECT TOP 1 ValoreParametro.CodTipoPrescrizioneTempi.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodTipoPrescrizioneTempi') as ValoreParametro(CodTipoPrescrizioneTempi))	
					  
		IF @sCodTipoPrescrizioneTempi <> ''
				SET	@sSET= @sSET + ',CodTipoPrescrizioneTempi=''' + @sCodTipoPrescrizioneTempi +''''	+ CHAR(13) + CHAR(10)				
			ELSE
				SET	@sSET= @sSET + ',CodTipoPrescrizioneTempi=NULL'	+ CHAR(13) + CHAR(10)		
	END	
		
		IF @xParametri.exist('/Parametri/Posologia')=1
	BEGIN	
		SET @txtPosologia=(SELECT TOP 1 ValoreParametro.Posologia.value('.','VARCHAR(MAX)')
				  FROM @xParametri.nodes('/Parametri/Posologia') as ValoreParametro(Posologia))					  

		IF @txtPosologia <> ''
			SET	@sSET= @sSET +',Posologia=
										CONVERT(varchar(max),
												CAST(N'''' AS xml).value(''xs:base64Binary("' + @txtPosologia
												+ '")'', ''varbinary(max)'') 
											)'  + CHAR(13) + CHAR(10)	
		ELSE
			SET	@sSET= @sSET +',Posologia=NULL '	+ CHAR(13) + CHAR(10)														
	END	
			
											
		IF @xParametri.exist('/Parametri/DataOraInizio')=1
		BEGIN			
			SET @sDataTmp=(SELECT TOP 1 ValoreParametro.DataOraInizio.value('.','VARCHAR(20)')
							  FROM @xParametri.nodes('/Parametri/DataOraInizio') as ValoreParametro(DataOraInizio))					  
			SET @sDataTmp=ISNULL(@sDataTmp,'')
			
			IF @sDataTmp<> '' 
				BEGIN			
										SET @sDataTmp= SUBSTRING(@sDataTmp,4,2) + '-' 
								+ LEFT(@sDataTmp,2) + '-' +
								+ SUBSTRING(@sDataTmp,7,4) +
								' ' + RIGHT(@sDataTmp,5)
					IF ISDATE(@sDataTmp)=1
						SET	@dDataOraInizio=CONVERT(DATETIME,@sDataTmp,120)						
					ELSE
						SET	@dDataOraInizio =NULL			
				END
			IF @dDataOraInizio IS NOT NULL
				SET	@sSET= @sSET + ',DataOraInizio=CONVERT(DateTime,''' + CONVERT(VARCHAR(20),@dDataOraInizio,120) + ''',120)' + CHAR(13) + CHAR(10)		 
			ELSE	
				SET	@sSET= @sSET + ',DataOraInizio=NULL' + CHAR(13) + CHAR(10)		 
		END	
		
	
		IF @xParametri.exist('/Parametri/DataOraFine')=1
	BEGIN	
		SET @sDataTmp=(SELECT TOP 1 ValoreParametro.DataOraFine.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/DataOraFine') as ValoreParametro(DataOraFine))					  
		SET @sDataTmp=ISNULL(@sDataTmp,'')
	
		IF @sDataTmp<> '' 
		BEGIN			
						SET @sDataTmp= SUBSTRING(@sDataTmp,4,2) + '-' 
						+ LEFT(@sDataTmp,2) + '-' +
						+ SUBSTRING(@sDataTmp,7,4) +
						' ' + RIGHT(@sDataTmp,5)
			IF ISDATE(@sDataTmp)=1
				SET	@dDataOraFine=CONVERT(DATETIME,@sDataTmp,120)						
			ELSE
				SET	@dDataOraFine =NULL			
		END
		
		IF @dDataOraFine IS NOT NULL		
			SET	@sSET= @sSET + ',DataOraFine=CONVERT(DateTime,''' + CONVERT(VARCHAR(20),@dDataOraFine,120) + ''',120)' + CHAR(13) + CHAR(10)		
		ELSE
			SET	@sSET= @sSET + ',DataOraFine=NULL ' + CHAR(13) + CHAR(10)		
	END	
	
		IF @xParametri.exist('/Parametri/AlBisogno')=1
	BEGIN
		SET @bAlBisogno=(SELECT TOP 1 ValoreParametro.AlBisogno.value('.','BIT')
					  FROM @xParametri.nodes('/Parametri/AlBisogno') as ValoreParametro(AlBisogno))	
					  
		IF ISNULL(@bAlBisogno,'') <> ''
				SET	@sSET= @sSET + ',AlBisogno=' + CONVERT(VARCHAR(1),@bAlBisogno) +	+ CHAR(13) + CHAR(10)				
			ELSE
				SET	@sSET= @sSET + ',AlBisogno=0'	+ CHAR(13) + CHAR(10)		END	
			
			IF @xParametri.exist('/Parametri/Durata')=1
	BEGIN
		SET @nDurata=(SELECT TOP 1 ValoreParametro.Durata.value('.','INTEGER')
				  FROM @xParametri.nodes('/Parametri/Durata') as ValoreParametro(Durata))	
					  
		IF ISNULL(@nDurata,'') <> ''
				SET	@sSET= @sSET + ',Durata=' + CONVERT(VARCHAR(20),@nDurata) +	+ CHAR(13) + CHAR(10)				
			ELSE
				SET	@sSET= @sSET + ',Durata=0'	+ CHAR(13) + CHAR(10)		END													
	
		IF @xParametri.exist('/Parametri/Continuita')=1
	BEGIN
		SET @bContinuita=(SELECT TOP 1 ValoreParametro.Continuita.value('.','BIT')
					  FROM @xParametri.nodes('/Parametri/Continuita') as ValoreParametro(Continuita))	
					  
		IF ISNULL(@bContinuita,'') <> ''
				SET	@sSET= @sSET + ',Continuita=' + CONVERT(VARCHAR(1),@bContinuita) +	+ CHAR(13) + CHAR(10)				
			ELSE
				SET	@sSET= @sSET + ',Continuita=0'	+ CHAR(13) + CHAR(10)		END	
			
						
		IF @xParametri.exist('/Parametri/PeriodicitaGiorni')=1
	BEGIN
		SET @sTmp=(SELECT TOP 1 ValoreParametro.PeriodicitaGiorni.value('.','VARCHAR(20)')
						  FROM @xParametri.nodes('/Parametri/PeriodicitaGiorni') as ValoreParametro(PeriodicitaGiorni))					  
						  
	   IF ISNULL(@sTmp,'') <> '' AND ISNUMERIC(@sTmp)=1
				SET	@sSET= @sSET + ',PeriodicitaGiorni=CONVERT(INTEGER,''' + CONVERT(VARCHAR(20),@sTmp) + ''')'	+ CHAR(13) + CHAR(10)				
			ELSE
				SET	@sSET= @sSET + ',PeriodicitaGiorni=0'	+ CHAR(13) + CHAR(10)		END		
	
		IF @xParametri.exist('/Parametri/PeriodicitaOre')=1
	BEGIN
		SET @sTmp=(SELECT TOP 1 ValoreParametro.PeriodicitaOre.value('.','VARCHAR(20)')
						  FROM @xParametri.nodes('/Parametri/PeriodicitaOre') as ValoreParametro(PeriodicitaOre))					  
						  
	   IF ISNULL(@sTmp,'') <> '' AND ISNUMERIC(@sTmp)=1
				SET	@sSET= @sSET + ',PeriodicitaOre=CONVERT(INTEGER,''' + CONVERT(VARCHAR(20),@sTmp) + ''')'	+ CHAR(13) + CHAR(10)				
			ELSE
				SET	@sSET= @sSET + ',PeriodicitaOre=0'	+ CHAR(13) + CHAR(10)		END		
	
	
		IF @xParametri.exist('/Parametri/PeriodicitaMinuti')=1
	BEGIN
		SET @sTmp=(SELECT TOP 1 ValoreParametro.PeriodicitaMinuti.value('.','VARCHAR(20)')
						  FROM @xParametri.nodes('/Parametri/PeriodicitaMinuti') as ValoreParametro(PeriodicitaMinuti))					  
						  
	   IF ISNULL(@sTmp,'') <> '' AND ISNUMERIC(@sTmp)=1
				SET	@sSET= @sSET + ',PeriodicitaMinuti=CONVERT(INTEGER,''' + CONVERT(VARCHAR(20),@sTmp) + ''')'	+ CHAR(13) + CHAR(10)				
			ELSE
				SET	@sSET= @sSET + ',PeriodicitaMinuti=0'	+ CHAR(13) + CHAR(10)		END		
	
		IF @xParametri.exist('/Parametri/CodProtocollo')=1
	BEGIN	
		SET @txtCodProtocollo=(SELECT TOP 1 ValoreParametro.CodProtocollo.value('.','VARCHAR(MAX)')
				  FROM @xParametri.nodes('/Parametri/CodProtocollo') as ValoreParametro(CodProtocollo))					  

		IF @txtCodProtocollo <> ''
			SET	@sSET= @sSET +',CodProtocollo=
										CONVERT(varchar(max),
												CAST(N'''' AS xml).value(''xs:base64Binary("' + @txtCodProtocollo
												+ '")'', ''varbinary(max)'') 
											)'  + CHAR(13) + CHAR(10)	
		ELSE
			SET	@sSET= @sSET +',CodProtocollo=NULL '	+ CHAR(13) + CHAR(10)														
	END	
										
		IF @xParametri.exist('/Parametri/Continuita')=1
	BEGIN
		SET @bManuale=(SELECT TOP 1 ValoreParametro.Manuale.value('.','BIT')
					  FROM @xParametri.nodes('/Parametri/Manuale') as ValoreParametro(Manuale))	
					  
		IF ISNULL(@bManuale,'') <> ''
				SET	@sSET= @sSET + ',Manuale=' + CONVERT(VARCHAR(1),@bManuale) +	+ CHAR(13) + CHAR(10)				
			ELSE
				SET	@sSET= @sSET + ',Manuale=0'	+ CHAR(13) + CHAR(10)		END	
										 
		IF @xParametri.exist('/Parametri/TempiManuali')=1
	BEGIN
						SET @xTempiManuali=(SELECT TOP 1 TempiManuali.TS.query('.')
							 FROM @xParametri.nodes('/Parametri/TempiManuali') as TempiManuali(TS))
							 
			SET @txtTempiManuali= CONVERT(VARCHAR(MAX), @xTempiManuali)
			
			SET	@sSET= @sSET + ',TempiManuali=CONVERT(XML, CONVERT(NVARCHAR(MAX), ''' + @txtTempiManuali + '''))'
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

		SET @xSchedaMovimento=(SELECT TOP 1 ValoreParametro.TS.query('.')
					  FROM @xParametri.nodes('/Parametri/SchedaMovimento') as ValoreParametro(TS))					
				
				
		IF ISNULL(@sCodStatoPrescrizioneTempi,'') = 'VA'  
		BEGIN
			SET	@sSET= @sSET + ',DataValidazione=ISNULL(DataValidazione,GetDate())' + CHAR(13) + CHAR(10)	
			SET	@sSET= @sSET + ',DataValidazioneUTC=ISNULL(DataValidazioneUTC,GetUTCDate())' + CHAR(13) + CHAR(10)	
			SET	@sSET= @sSET + ',CodUtenteValidazione=ISNULL(CodUtenteValidazione, ''' + @sCodUtenteUltimaModifica + ''')' + CHAR(13) + CHAR(10)		
		END

		IF ISNULL(@sCodStatoPrescrizioneTempi,'') = 'SS'
		BEGIN
			SET	@sSET= @sSET + ',DataSospensione=ISNULL(DataSospensione,GetDate())' + CHAR(13) + CHAR(10)	
			SET	@sSET= @sSET + ',DataSospensioneUTC=ISNULL(DataSospensioneUTC,GetUTCDate())' + CHAR(13) + CHAR(10)	
			SET	@sSET= @sSET + ',CodUtenteSospensione=ISNULL(CodUtenteSospensione,''' + @sCodUtenteUltimaModifica + ''')' + CHAR(13) + CHAR(10)	
		END
	

				
				
		IF LEFT(@sSET,1)=',' 
		SET @sSET=RIGHT(@sSET,LEN(@sSET)-1)
	
	IF @sSET <> ''		
		BEGIN
						
								SET @sWHERE =' WHERE ID=''' + convert(varchar(50),@uIDPrescrizioneTempi) +''''
				
								SET @sSQL=	ISNULL(@sSQL,'') + CHAR(13) + CHAR(10) + 
						ISNULL(@sSET,'') + CHAR(13) + CHAR(10) + 
						ISNULL(@sWHERE,'')
						
										
																				SET @xTimeStamp.modify('delete (/TimeStamp/IDEntita)[1]') 
						
				SET @xTimeStamp.modify('insert <IDEntita>{sql:variable("@uIDPrescrizione")}</IDEntita> into (/TimeStamp)[1]')

								SET @xTimeStampBase=@xTimeStamp
		
	
				SET @xTimeStamp=CONVERT(XML,
									'<Parametri>' + CONVERT(varchar(max),@xTimeStamp) +
									'</Parametri>')
				
																
				SET @xParLog=CONVERT(XML,'<Parametri><LogPrima/><LogDopo/></Parametri>')										
				
				SET @xLogPrima=Convert(XML,'<DataSet></DataSet>')														
				
				SET @xTemp=
						(SELECT * FROM 
								(SELECT P.*
										,PT.ID AS IDPrescrizioneTempi
										,PT.IDNum AS IDNumPrescrizioneTempi      
										,PT.CodStatoPrescrizioneTempi
										,PT.Posologia
										,PT.DataEvento AS DataEventoPrescrizioneTempi
										,PT.DataEventoUTC AS DataEventoUTCPrescrizioneTempi
										,PT.DataOraInizio
										,PT.DataOraFine
										,PT.AlBisogno
										,PT.Durata
										,PT.Continuita
										,PT.PeriodicitaGiorni
										,PT.PeriodicitaOre
										,PT.PeriodicitaMinuti
										,PT.CodProtocollo
										,PT.Manuale
										,PT.TempiManuali
										,PT.CodUtenteRilevazione AS CodUtenteRilevazionePrescrizioneTempi
										,PT.CodUtenteUltimaModifica AS CodUtenteUltimaModificaPrescrizioneTempi
										,PT.DataUltimaModifica AS DataUltimaModificaPrescrizioneTempi
										,PT.DataUltimaModificaUTC AS DataUltimaModificaUTCPrescrizioneTempi
										,PT.DataValidazione AS DataValidazionePrescrizioneTempi
										,PT.DataValidazioneUTC AS DataValidazioneUTCPrescrizioneTempi					 
										,PT.DataSospensione aS DataSospensionePrescrizioneTempi
										,PT.DataSospensioneUTC AS DataSospensionePrescrizioneTempiUTC
										,PT.CodUtenteValidazione AS CodUtenteValidazionePrescrizioneTempi										
										,PT.CodUtenteSospensione AS CodUtenteSospensionePrescrizioneTempi
									FROM T_MovPrescrizioniTempi PT
									  LEFT JOIN T_MovPrescrizioni P
										ON (PT.IDPrescrizione=P.ID)
								 WHERE PT.ID=@uIDPrescrizioneTempi
								) AS [Table]
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
								(SELECT P.*
										,PT.ID AS IDPrescrizioneTempi
										,PT.IDNum AS IDNumPrescrizioneTempi
										,PT.CodStatoPrescrizioneTempi
										,PT.Posologia
										,PT.DataEvento AS DataEventoPrescrizioneTempi
										,PT.DataEventoUTC AS DataEventoUTCPrescrizioneTempi
										,PT.DataOraInizio
										,PT.DataOraFine
										,PT.AlBisogno
										,PT.Durata
										,PT.Continuita
										,PT.PeriodicitaGiorni
										,PT.PeriodicitaOre
										,PT.PeriodicitaMinuti
										,PT.CodProtocollo
										,PT.Manuale
										,PT.TempiManuali
										,PT.CodUtenteRilevazione AS CodUtenteRilevazionePrescrizioneTempi
										,PT.CodUtenteUltimaModifica AS CodUtenteUltimaModificaPrescrizioneTempi
										,PT.DataUltimaModifica AS DataUltimaModificaPrescrizioneTempi
										,PT.DataUltimaModificaUTC AS DataUltimaModificaUTCPrescrizioneTempi		
										,PT.DataValidazione AS DataValidazionePrescrizioneTempi
										,PT.DataValidazioneUTC AS DataValidazioneUTCPrescrizioneTempi					 
										,PT.DataSospensione aS DataSospensionePrescrizioneTempi
										,PT.DataSospensioneUTC AS DataSospensionePrescrizioneTempiUTC
										,PT.CodUtenteValidazione AS CodUtenteValidazionePrescrizioneTempi										
										,PT.CodUtenteSospensione AS CodUtenteSospensionePrescrizioneTempi
									FROM T_MovPrescrizioniTempi PT
									  LEFT JOIN T_MovPrescrizioni P
										ON (PT.IDPrescrizione=P.ID)
								 WHERE PT.ID=@uIDPrescrizioneTempi
								) AS [Table]
							FOR XML AUTO, ELEMENTS)


						SET @xLogDopo.modify('insert sql:variable("@xTemp") as first into (/DataSet)[1]')																											
						SET @xParLog.modify('insert sql:variable("@xLogDopo") as first into (/Parametri/LogDopo)[1]')
						
												SET @xParLog.modify('insert sql:variable("@xTimeStampBase") as last into (/Parametri)[1]')
						
												SET @xParLog.modify('insert sql:variable("@xSchedaMovimento") as last into (/Parametri/LogDopo/DataSet)[1]')

					    SELECT @xParLog
						EXEC MSP_InsMovLog @xParLog
						
						
										END	
				ELSE
					BEGIN
						ROLLBACK TRANSACTION	
											END	 
				
		END		
	RETURN 0
END