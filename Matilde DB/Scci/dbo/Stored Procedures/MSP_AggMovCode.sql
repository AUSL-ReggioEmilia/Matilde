CREATE PROCEDURE [dbo].[MSP_AggMovCode](@xParametri AS XML )
AS
BEGIN
	

				DECLARE @uIDCoda AS UNIQUEIDENTIFIER
	DECLARE @sNumeroCoda AS Varchar(50)
	DECLARE @sCodStatoCoda AS Varchar(20)
	DECLARE @sCodLogin AS Varchar(100)
	DECLARE @sCodRuolo AS Varchar(20)
	DECLARE @xTimeStamp AS XML
	
		DECLARE @sSQL AS VARCHAR(MAX)
	DECLARE @sSET AS VARCHAR(MAX)
	DECLARE @sWHERE AS VARCHAR(MAX)
	
	DECLARE @sDataTmp AS VARCHAR(20)
	DECLARE @sGUID AS VARCHAR(50)
	DECLARE @uGUID AS UNIQUEIDENTIFIER
	DECLARE @xTimeStampBase AS XML	
	DECLARE @sCodAzione AS Varchar(20)
			
		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDCoda.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDCoda') as ValoreParametro(IDCoda))
	IF 	ISNULL(@sGUID,'') <> '' SET @uIDCoda=CONVERT(UNIQUEIDENTIFIER,	@sGUID)	
	
		IF @xParametri.exist('/Parametri/NumeroCoda')=1
	BEGIN		
		SET @sNumeroCoda=(SELECT	TOP 1 ValoreParametro.NumeroCoda.value('.','VARCHAR(50)')
						 FROM @xParametri.nodes('/Parametri/NumeroCoda') as ValoreParametro(NumeroCoda))
	END	
	
		IF @xParametri.exist('/Parametri/CodStatoCoda')=1
	BEGIN		
		SET @sCodStatoCoda=(SELECT	TOP 1 ValoreParametro.CodStatoCoda.value('.','VARCHAR(50)')
						 FROM @xParametri.nodes('/Parametri/CodStatoCoda') as ValoreParametro(CodStatoCoda))						 
		
	END	
	
		SET @sCodRuolo=(SELECT	TOP 1 ValoreParametro.CodRuolo.value('.','VARCHAR(20)')
						 FROM @xParametri.nodes('/Parametri/TimeStamp/CodRuolo') as ValoreParametro(CodRuolo))						 
	SET @sCodRuolo= LTRIM(RTRIM(ISNULL(@sCodRuolo,'')))		
	
		SET @sCodLogin=(SELECT	TOP 1 ValoreParametro.CodLogin.value('.','VARCHAR(100)')
						 FROM @xParametri.nodes('/Parametri/TimeStamp/CodLogin') as ValoreParametro(CodLogin))						 
	SET @sCodLogin= LTRIM(RTRIM(ISNULL(@sCodLogin,'')))	
				
	    SET @xTimeStamp=(SELECT TOP 1 ValoreParametro.TS.query('.')
					  FROM @xParametri.nodes('/Parametri/TimeStamp') as ValoreParametro(TS))	
	
		SET @sSQL='UPDATE T_MovCode ' + CHAR(13) + CHAR(10) +
			  'SET '
			  					
	SET @sSET =''	
	
	
	IF ISNULL(@sCodStatoCoda,'')='CA'
			BEGIN						
																SET @sSET=@sSET + ',CodStatoCoda=''' + @sCodStatoCoda + ''''	
				SET @sCodAzione='CAN'
				
								UPDATE T_MovCodeEntita
				SET CodStatoCodaEntita='CA'
				WHERE IDCoda=@uIDCoda
			END
		ELSE		
			IF ISNULL(@sCodStatoCoda,'')='AS'	
				BEGIN
																				SET @sSET=@sSET + ',CodStatoCoda=''' + @sCodStatoCoda + ''''											
					SET @sCodAzione='MOD'
				END
			ELSE
				BEGIN
					SET @sCodAzione='MOD'
				END	
	
		SET @sSET=@sSET + ',CodUtenteUltimaModifica=''' + @sCodLogin + ''''
	
		SET @sSET=@sSET + ',DataUltimaModifica=GetDate()'
	SET @sSET=@sSET + ',DataUltimaModificaUTC=GetUTCDate()'	
	
	IF ISNULL(@sNumeroCoda,'') <> ''
	BEGIN
		SET @sSET=@sSET + ',NumeroCoda=''' + @sNumeroCoda + ''''
		SET @sSET=@sSET + ',DataAssegnazione=GetDate()'
		SET @sSET=@sSET + ',DataAssegnazioneUTC=GetDate()'
	END

	IF LEFT(@sSET,1)=',' 
		SET @sSET=RIGHT(@sSET,LEN(@sSET)-1)	

	IF @sSET <> ''	
		BEGIN
						
						IF @uIDCoda IS NULL 
				SET @sWHERE =' WHERE 1=0'
			ELSE
				SET @sWHERE =' WHERE ID=''' + convert(varchar(50),@uIDCoda) +''''
			
						SET @sSQL=	ISNULL(@sSQL,'') + CHAR(13) + CHAR(10) + 
						ISNULL(@sSET,'') + CHAR(13) + CHAR(10) + 
						ISNULL(@sWHERE,'')	
						
			PRINT @sSQL
			EXEC (@sSQL)			
	     END

					SET @xTimeStamp.modify('delete (/TimeStamp/IDEntita)[1]') 			
	SET @xTimeStamp.modify('insert <IDEntita>{sql:variable("@uIDCoda")}</IDEntita> into (/TimeStamp)[1]')

	SET @xTimeStamp.modify('delete (/TimeStamp/CodEntita)[1]') 
	SET @xTimeStamp.modify('insert <CodEntita>CDA</CodEntita> into (/TimeStamp)[1]')
		
	SET @xTimeStamp.modify('delete (/TimeStamp/CodAzione)[1]') 
	SET @xTimeStamp.modify('insert <CodAzione>{sql:variable("@sCodAzione")}</CodAzione> into (/TimeStamp)[1]')
	
		SET @xTimeStampBase=@xTimeStamp


	SET @xTimeStamp=CONVERT(XML,
						'<Parametri>' + CONVERT(varchar(max),@xTimeStamp) +
						'</Parametri>')
	
	IF @@ERROR=0 
		BEGIN
						EXEC MSP_InsMovTimeStamp @xTimeStamp					
		END	
END