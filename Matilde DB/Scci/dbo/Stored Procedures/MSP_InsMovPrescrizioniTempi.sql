CREATE PROCEDURE [dbo].[MSP_InsMovPrescrizioniTempi](@xParametri XML)
AS
BEGIN
		
	
			
		DECLARE @uIDPrescrizione AS UNIQUEIDENTIFIER
	DECLARE @sCodStatoPrescrizioneTempi AS VARCHAR(20)	
	DECLARE @sCodTipoPrescrizioneTempi AS VARCHAR(20)	
	DECLARE @binPosologia VARBINARY(MAX)		
	DECLARE @bAlBisogno AS BIT
	DECLARE @dDataOraInizio AS DATETIME	
	DECLARE @dDataOraFine AS DATETIME
		
	DECLARE @nDurata AS INT
	DECLARE @bContinuita AS BIT
	DECLARE @nPeriodicitaGiorni AS INTEGER
	DECLARE @nPeriodicitaOre AS INTEGER
	DECLARE @nPeriodicitaMinuti AS INTEGER
	DECLARE @binCodProtocollo VARBINARY(MAX)			
	DECLARE @bManuale AS BIT		
	DECLARE @xTempiManuali AS XML
	DECLARE @txtTempiManuali AS VARCHAR(MAX)	
		
	DECLARE @sCodUtenteRilevazione AS VARCHAR(100)					
	
		DECLARE @sGUID AS VARCHAR(50)
	DECLARE @uGUID AS UNIQUEIDENTIFIER
	DECLARE @sDataTmp AS VARCHAR(20)	
	DECLARE @sTmp AS VARCHAR(20)	

	DECLARE @dDataValidazione AS DATETIME	
	DECLARE @dDataValidazioneUTC AS DATETIME
	DECLARE @dDataSospensione AS DATETIME	
	DECLARE @dDataSospensioneUTC AS DATETIME	

	DECLARE @sCodUtenteValidazione AS VARCHAR(100)
	DECLARE @sCodUtenteSospensione AS VARCHAR(100)
	
		
	    DECLARE @xTimeStampBase AS XML	
	DECLARE @xTimeStamp AS XML	
	
		DECLARE @xSchedaMovimento AS XML
		
		DECLARE @xLogPrima AS XML
	DECLARE @xLogDopo AS XML
	DECLARE @xTemp AS XML
	DECLARE @xParLog AS XML


		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDPrescrizione.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDPrescrizione') as ValoreParametro(IDPrescrizione))
	IF 	ISNULL(@sGUID,'') <> '' 
			SET @uIDPrescrizione=CONVERT(UNIQUEIDENTIFIER,	@sGUID)		
	
		SET @sCodStatoPrescrizioneTempi=(SELECT TOP 1 ValoreParametro.CodStatoPrescrizioneTempi.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodStatoPrescrizioneTempi') as ValoreParametro(CodStatoPrescrizioneTempi))	
	SET @sCodStatoPrescrizioneTempi=ISNULL(@sCodStatoPrescrizioneTempi,'')
	IF @sCodStatoPrescrizioneTempi='' SET @sCodStatoPrescrizioneTempi='IC'			
		SET @sCodTipoPrescrizioneTempi=(SELECT TOP 1 ValoreParametro.CodTipoPrescrizioneTempi.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodTipoPrescrizioneTempi') as ValoreParametro(CodTipoPrescrizioneTempi))	
			
		SET @binPosologia=(SELECT TOP 1 ValoreParametro.Posologia.value('.','VARBINARY(MAX)')
					  FROM @xParametri.nodes('/Parametri/Posologia') as ValoreParametro(Posologia))
					  	
		SET @bAlBisogno=(SELECT TOP 1 ValoreParametro.AlBisogno.value('.','BIT')
					  FROM @xParametri.nodes('/Parametri/AlBisogno') as ValoreParametro(AlBisogno))	
	SET @bAlBisogno=ISNULL(@bAlBisogno,0)
	
		SET @nDurata=(SELECT TOP 1 ValoreParametro.Durata.value('.','INTEGER')
					  FROM @xParametri.nodes('/Parametri/Durata') as ValoreParametro(Durata))	
	SET @nDurata=ISNULL(@nDurata,0)
	
		SET @bContinuita=(SELECT TOP 1 ValoreParametro.Continuita.value('.','BIT')
					  FROM @xParametri.nodes('/Parametri/Continuita') as ValoreParametro(Continuita))	
	SET @bContinuita=ISNULL(@bContinuita,0)

		SET  @bManuale=(SELECT TOP 1 ValoreParametro.Manuale.value('.','BIT')
					  FROM @xParametri.nodes('/Parametri/Manuale') as ValoreParametro(Manuale))	
	SET  @bManuale=ISNULL(@bManuale,0)
	
	    SET @xTempiManuali=(SELECT TOP 1 TempiManuali.TS.query('.')
							 FROM @xParametri.nodes('/Parametri/TempiManuali') as TempiManuali(TS)
						)
							
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

		SET @sTmp=(SELECT TOP 1 ValoreParametro.PeriodicitaGiorni.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/PeriodicitaGiorni') as ValoreParametro(PeriodicitaGiorni))					  
	IF @sTmp IS NULL
		SET @nPeriodicitaGiorni=0
	ELSE	
		BEGIN
			IF ISNUMERIC(@sTmp)=1					  
				SET @nPeriodicitaGiorni=CONVERT(INTEGER,@sTmp)
			ELSE
				SET @nPeriodicitaGiorni=0
		END	
	
		SET @sTmp=(SELECT TOP 1 ValoreParametro.PeriodicitaOre.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/PeriodicitaOre') as ValoreParametro(PeriodicitaOre))					  
	IF @sTmp IS NULL
		SET  @nPeriodicitaOre=0
	ELSE	
		BEGIN
			IF ISNUMERIC(@sTmp)=1					  
				SET  @nPeriodicitaOre=CONVERT(INTEGER,@sTmp)
			ELSE
				SET  @nPeriodicitaOre=0
		END	
	
		SET @sTmp=(SELECT TOP 1 ValoreParametro.PeriodicitaMinuti.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/PeriodicitaMinuti') as ValoreParametro(PeriodicitaMinuti))					  
	IF @sTmp IS NULL
		SET @nPeriodicitaMinuti=0
	ELSE	
		BEGIN
			IF ISNUMERIC(@sTmp)=1					  
				SET @nPeriodicitaMinuti=CONVERT(INTEGER,@sTmp)
			ELSE
				SET @nPeriodicitaMinuti=0
		END	
	
		SET @binCodProtocollo=(SELECT TOP 1 ValoreParametro.CodProtocollo.value('.','VARBINARY(MAX)')
					  FROM @xParametri.nodes('/Parametri/CodProtocollo') as ValoreParametro(CodProtocollo))
					  														
		SET @sCodUtenteRilevazione=(SELECT TOP 1 ValoreParametro.CodUtenteRilevazione.value('.','VARCHAR(100)')
					  FROM @xParametri.nodes('/Parametri/TimeStamp/CodLogin') as ValoreParametro(CodUtenteRilevazione))	
	SET @sCodUtenteRilevazione=ISNULL(@sCodUtenteRilevazione,'')
	
	    SET @xTimeStamp=(SELECT TOP 1 ValoreParametro.TS.query('.')
					  FROM @xParametri.nodes('/Parametri/TimeStamp') as ValoreParametro(TS))
	
		SET @xSchedaMovimento=(SELECT TOP 1 ValoreParametro.TS.query('.')
					  FROM @xParametri.nodes('/Parametri/SchedaMovimento') as ValoreParametro(TS))		
				
					
	SET @uGUID=NEWID()

		IF @sCodStatoPrescrizioneTempi='VA'
		BEGIN
			SET @dDataValidazione=GETDATE()
			SET @dDataValidazioneUTC=GETUTCDATE()
			SET @sCodUtenteValidazione=@sCodUtenteRilevazione
		END	
	ELSE
		BEGIN
			SET @dDataValidazione=NULL
			SET @dDataValidazioneUTC=NULL
			SET @sCodUtenteValidazione=NULL
		END	
		
		IF @sCodStatoPrescrizioneTempi='SS'
		BEGIN
			SET @dDataSospensione=GETDATE()
			SET @dDataSospensioneUTC=GETUTCDATE()
			SET @sCodUtenteSospensione=@sCodUtenteRilevazione
		END	
	ELSE
		BEGIN
			SET @dDataSospensione=NULL
			SET @dDataSospensioneUTC=NULL
			SET @sCodUtenteSospensione=NULL
		END	
	

					SET @xTimeStamp.modify('delete (/TimeStamp/IDEntita)[1]') 
			
	SET @xTimeStamp.modify('insert <IDEntita>{sql:variable("@uGUID")}</IDEntita> into (/TimeStamp)[1]')

		SET @xTimeStampBase=@xTimeStamp


	SET @xTimeStamp=CONVERT(XML,
						'<Parametri>' + CONVERT(varchar(max),@xTimeStamp) +
						'</Parametri>')
														
	BEGIN TRANSACTION
			
		INSERT INTO T_MovPrescrizioniTempi(
					   ID					  
					  ,IDPrescrizione	
					  ,CodStatoPrescrizioneTempi
					  ,CodTipoPrescrizioneTempi	
					  ,Posologia
					  ,DataEvento
					  ,DataEventoUTC			  
					  ,DataOraInizio
					  ,DataOraFine
					  ,AlBisogno
					  ,Durata
					  ,Continuita
					  ,PeriodicitaGiorni
					  ,PeriodicitaOre
					  ,PeriodicitaMinuti
					  ,CodProtocollo
					  ,Manuale
					  ,TempiManuali
					  ,CodUtenteRilevazione
					  ,CodUtenteUltimaModifica
					  ,DataUltimaModifica
					  ,DataUltimaModificaUTC
					  ,DataValidazione
					  ,DataValidazioneUTC
					  ,DataSospensione
					  ,DataSospensioneUTC
					  ,CodUtenteValidazione
					  ,CodUtenteSospensione
					  ,IDString
					  ,IDPrescrizioneString
				  )
		VALUES
				( 
					   @uGUID															  ,@uIDPrescrizione 												  ,@sCodStatoPrescrizioneTempi										  ,@sCodTipoPrescrizioneTempi										  ,CONVERT(VARCHAR(MAX),@binPosologia)								  ,GETDATE()														  ,GETUTCDATE()														  ,@dDataOraInizio													  ,@dDataOraFine													  ,@bAlBisogno														  ,@nDurata															  ,@bContinuita														  ,@nPeriodicitaGiorni												  ,@nPeriodicitaOre													  ,@nPeriodicitaMinuti												  ,CONVERT(VARCHAR(MAX),@binCodProtocollo)							  ,@bManuale														  ,@xTempiManuali													  ,@sCodUtenteRilevazione											  ,NULL																  ,NULL 															  ,NULL 															  ,@dDataValidazione												  ,@dDataValidazioneUTC												  ,@dDataSospensione												  ,@dDataSospensioneUTC												  ,@sCodUtenteValidazione											  ,@sCodUtenteSospensione 	  		  								  ,CONVERT(VARCHAR(50),@uGUID)										  ,CONVERT(VARCHAR(50),@uIDPrescrizione)						)
			
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
				(SELECT * FROM 
					(SELECT P.*
							,PT.ID AS IDPrescrizioneTempi
							,PT.IDNum AS IDNumPrescrizioneTempi      
							,PT.CodStatoPrescrizioneTempi
							,PT.CodTipoPrescrizioneTempi
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
							,PT.IDString AS IDPrescrizioneTempiString
							,PT.IDPrescrizioneString
							,PT.IDPrescrizioneTempiOrigine	
											 
						FROM T_MovPrescrizioniTempi PT
						  LEFT JOIN T_MovPrescrizioni P
							ON (PT.IDPrescrizione=P.ID)
					 WHERE PT.ID=@uGUID
					) AS [Table]
				FOR XML AUTO, ELEMENTS)

			SET @xLogDopo.modify('insert sql:variable("@xTemp") as first into (/DataSet)[1]')
	
						
			SET @xParLog=CONVERT(XML,'<Parametri><LogDopo/></Parametri>')			
			SET @xParLog.modify('insert sql:variable("@xLogDopo") as first into (/Parametri/LogDopo)[1]')
			SET @xParLog.modify('insert sql:variable("@xTimeStampBase") as last into (/Parametri)[1]')
			
						SET @xParLog.modify('insert sql:variable("@xSchedaMovimento") as last into (/Parametri/LogDopo/DataSet)[1]')

                    
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