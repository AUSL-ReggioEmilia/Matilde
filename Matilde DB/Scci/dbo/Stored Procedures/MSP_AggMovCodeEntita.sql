CREATE PROCEDURE [dbo].[MSP_AggMovCodeEntita](@xParametri AS XML )
AS
BEGIN
	

	DECLARE @uIDCodaEntita AS UNIQUEIDENTIFIER	
	DECLARE @sCodStatoCodaEntita AS Varchar(20)
	DECLARE @sCodLogin AS Varchar(100)
	DECLARE @sCodRuolo AS Varchar(20)
	DECLARE @xTimeStamp AS XML
	
	DECLARE @uIDCoda AS UNIQUEIDENTIFIER
	DECLARE @nNumeroCoda AS INTEGER
	DECLARE @sCodStatoCodaEntitaPrecedente AS Varchar(20)
	DECLARE @sCodEntitaCoda AS Varchar(20)
	DECLARE @uIDEntitaCoda AS UNIQUEIDENTIFIER

	DECLARE @sSQL AS VARCHAR(MAX)
	DECLARE @sSET AS VARCHAR(MAX)
	DECLARE @sWHERE AS VARCHAR(MAX)
	
	DECLARE @sDataTmp AS VARCHAR(20)
	DECLARE @sGUID AS VARCHAR(50)
	DECLARE @uGUID AS UNIQUEIDENTIFIER
	DECLARE @xTimeStampBase AS XML	
	DECLARE @sCodAzione AS Varchar(20)
			
	SET @sGUID=(SELECT TOP 1 ValoreParametro.IDCodaEntita.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDCodaEntita') as ValoreParametro(IDCodaEntita))
	IF 	ISNULL(@sGUID,'') <> '' SET @uIDCodaEntita=CONVERT(UNIQUEIDENTIFIER,	@sGUID)	
			
		IF @xParametri.exist('/Parametri/CodStatoCodaEntita')=1
	BEGIN		
		SET @sCodStatoCodaEntita=(SELECT	TOP 1 ValoreParametro.CodStatoCodaEntita.value('.','VARCHAR(50)')
						 FROM @xParametri.nodes('/Parametri/CodStatoCodaEntita') as ValoreParametro(CodStatoCodaEntita))						 
		
	END	
	
	SET @sCodRuolo=(SELECT	TOP 1 ValoreParametro.CodRuolo.value('.','VARCHAR(20)')
						 FROM @xParametri.nodes('/Parametri/TimeStamp/CodRuolo') as ValoreParametro(CodRuolo))						 
	SET @sCodRuolo= LTRIM(RTRIM(ISNULL(@sCodRuolo,'')))		
	
	SET @sCodLogin=(SELECT	TOP 1 ValoreParametro.CodLogin.value('.','VARCHAR(100)')
						 FROM @xParametri.nodes('/Parametri/TimeStamp/CodLogin') as ValoreParametro(CodLogin))						 
	SET @sCodLogin= LTRIM(RTRIM(ISNULL(@sCodLogin,'')))	
				
	SET @xTimeStamp=(SELECT TOP 1 ValoreParametro.TS.query('.')
					FROM @xParametri.nodes('/Parametri/TimeStamp') as ValoreParametro(TS))	
	
	SET @sCodStatoCodaEntitaPrecedente = (SELECT CodStatoCodaEntita FROM T_MovCodeEntita WHERE ID=@uIDCodaEntita)

	SET @sSQL='UPDATE T_MovCodeEntita ' + CHAR(13) + CHAR(10) +
			'SET '
			  					
	SET @sSET =''	
		
	IF ISNULL(@sCodStatoCodaEntita,'')='CA'
			BEGIN						
																SET @sSET=@sSET + ',CodStatoCodaEntita=''' + @sCodStatoCodaEntita + ''''	
				SET @sSET=@sSET + ',DataChiamata=NULL'
				SET @sSET=@sSET + ',DataChiamataUTC=NULL'
				SET @sSET=@sSET + ',CodUtenteChiamata=NULL'
				
				SET @sCodAzione='CAN'
				
			
			END
		ELSE		
			IF ISNULL(@sCodStatoCodaEntita,'')='AS'	
				BEGIN
																				SET @sSET=@sSET + ',CodStatoCodaEntita=''' + @sCodStatoCodaEntita + ''''
					SET @sSET=@sSET + ',DataChiamata=NULL'
					SET @sSET=@sSET + ',DataChiamataUTC=NULL'
					SET @sSET=@sSET + ',CodUtenteChiamata=NULL'
					
					SET @sCodAzione='MOD'
				END
			ELSE
				IF ISNULL(@sCodStatoCodaEntita,'')='CH'	
				BEGIN
																				SET @sSET=@sSET + ',CodStatoCodaEntita=''' + @sCodStatoCodaEntita + ''''	
					SET @sSET=@sSET + ',DataChiamata=GETDATE()'
					SET @sSET=@sSET + ',DataChiamataUTC=GETUTCDATE()'															
					SET @sSET=@sSET + ',CodUtenteChiamata=''' + @sCodLogin + ''''
					SET @sCodAzione='MOD'
				END
	
	SET @sSET=@sSET + ',CodUtenteUltimaModifica=''' + @sCodLogin + ''''
	
	SET @sSET=@sSET + ',DataUltimaModifica=GetDate()'
	SET @sSET=@sSET + ',DataUltimaModificaUTC=GetUTCDate()'	
		

	IF LEFT(@sSET,1)=',' 
		SET @sSET=RIGHT(@sSET,LEN(@sSET)-1)	

	IF @sSET <> ''	
		BEGIN
						
						IF @uIDCodaEntita IS NULL 
				SET @sWHERE =' WHERE 1=0'
			ELSE
				SET @sWHERE =' WHERE ID=''' + convert(varchar(50),@uIDCodaEntita) +''''
			
						SET @sSQL=	ISNULL(@sSQL,'') + CHAR(13) + CHAR(10) + 
						ISNULL(@sSET,'') + CHAR(13) + CHAR(10) + 
						ISNULL(@sWHERE,'')	
						
			PRINT @sSQL
			EXEC (@sSQL)			

												
			DECLARE @sMessaggio VARCHAR(8000)
			DECLARE @sLuogo VARCHAR(8000)
			DECLARE @sTipologia VARCHAR(64)
			DECLARE @NumeroTelefonoDestinatario VARCHAR(64)

			SELECT TOP 1 
				@uIDCoda = IDCoda,
				@sCodEntitaCoda = CodEntita,
				@uIDEntitaCoda = IDEntita
			FROM T_MovCodeEntita 
			WHERE ID=@uIDCodaEntita

			IF ISNULL(@sCodEntitaCoda,'') = 'APP'
			BEGIN
				SET @sLuogo = (SELECT ElencoRisorse FROM T_MovAppuntamenti WHERE ID=@uIDEntitaCoda)
			END

			IF ISNULL(@sCodEntitaCoda,'') = 'WKI'
			BEGIN
				SET @sLuogo=''
			END

			SET @nNumeroCoda = (SELECT TOP 1 NumeroCoda FROM T_MovCode WHERE ID=@uIDCoda)
			SET @NumeroTelefonoDestinatario = (SELECT TOP 1 NumeroTelefono FROM T_MH_MovAutoCheckin WHERE IDCoda=@uIDCoda)

			
						IF ISNULL(@NumeroTelefonoDestinatario,'') <> ''
			BEGIN				
								IF ISNULL(@sCodStatoCodaEntita,'') = 'CH'	
				BEGIN
					SET @sTipologia = 'CHIAMATA NUMERO'
					SET @sMessaggio = 'CHIAMATO IL N. ' + ISNULL(CONVERT(VARCHAR(20),@nNumeroCoda),'') + '.'
					SET @sMessaggio = @sMessaggio + ' (' + @sLuogo + ')'								
				END
				ELSE
				BEGIN
										IF ISNULL(@sCodStatoCodaEntitaPrecedente,'') = 'CH'
					BEGIN
						SET @sTipologia = 'ANNULLAMENTO CHIAMATA'
						SET @sMessaggio = 'ANNULLATA LA CHIAMATA AL N. ' + ISNULL(CONVERT(VARCHAR(20),@nNumeroCoda),'') + '.'					
					END				
				END 

				SELECT @sCodStatoCodaEntita,@sCodStatoCodaEntitaPrecedente,@sMessaggio, @sTipologia
				IF ISNULL(@sMessaggio,'') <> '' AND ISNULL(@sTipologia,'') <> ''
				BEGIN
										EXEC MSP_MH_InsCodaSmsOutput @sMessaggio, @sTipologia, @NumeroTelefonoDestinatario
				END 
			END
	     END

	SET @xTimeStamp.modify('delete (/TimeStamp/IDEntita)[1]') 			
	SET @xTimeStamp.modify('insert <IDEntita>{sql:variable("@uIDCodaEntita")}</IDEntita> into (/TimeStamp)[1]')

	SET @xTimeStamp.modify('delete (/TimeStamp/CodEntita)[1]') 
	SET @xTimeStamp.modify('insert <CodEntita>CDA</CodEntita> into (/TimeStamp)[1]')
		
	SET @xTimeStamp.modify('delete (/TimeStamp/CodAzione)[1]') 
	SET @xTimeStamp.modify('insert <CodAzione>{sql:variable("@sCodAzione")}</CodAzione> into (/TimeStamp)[1]')
	
	SET @xTimeStamp.modify('delete (/TimeStamp/Note)[1]') 
	SET @xTimeStamp.modify('insert <Note>StatoEntita: {sql:variable("@sCodStatoCodaEntita")}</Note> into (/TimeStamp)[1]')
	
	SET @xTimeStampBase=@xTimeStamp


	SET @xTimeStamp=CONVERT(XML,
						'<Parametri>' + CONVERT(varchar(max),@xTimeStamp) +
						'</Parametri>')
	
	IF @@ERROR=0 
		BEGIN
						EXEC MSP_InsMovTimeStamp @xTimeStamp					
		END	
END