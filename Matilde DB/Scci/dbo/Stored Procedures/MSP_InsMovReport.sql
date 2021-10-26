CREATE PROCEDURE [dbo].[MSP_InsMovReport](@xParametri AS XML)
AS
BEGIN
	
											
		DECLARE @uIDPaziente AS UNIQUEIDENTIFIER	
	DECLARE @uIDEpisodio AS UNIQUEIDENTIFIER	
	DECLARE @uIDTrasferimento AS UNIQUEIDENTIFIER	
	DECLARE @uIDCartella AS UNIQUEIDENTIFIER	
	DECLARE @sCodReport AS VARCHAR(20)	
	DECLARE @dDataEvento AS DATETIME	
	DECLARE @dDataEventoUTC AS DATETIME
	
	DECLARE @binDocumento VARBINARY(MAX)	
	DECLARE @txtDocumento VARCHAR(MAX)	
	
	DECLARE @sCodLogin		VARCHAR(100)
	DECLARE @sCodRuolo		VARCHAR(20)
	DECLARE @sNomePC		VARCHAR(255)
	DECLARE @sIndirizzoIP	VARCHAR(50)
	
		DECLARE @sGUID AS VARCHAR(50)
	DECLARE @uGUID AS UNIQUEIDENTIFIER
	DECLARE @sDataTmp AS VARCHAR(20)	
	
	
		
	    DECLARE @xTimeStampBase AS XML	
	DECLARE @xTimeStamp AS XML	
	
		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDPaziente.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDPaziente') as ValoreParametro(IDPaziente))
	IF 	ISNULL(@sGUID,'') <> '' 
			SET @uIDPaziente=CONVERT(UNIQUEIDENTIFIER,	@sGUID)		
	
	
		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDEpisodio.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDEpisodio') as ValoreParametro(IDEpisodio))
	IF 	ISNULL(@sGUID,'') <> '' 
			SET @uIDEpisodio=CONVERT(UNIQUEIDENTIFIER,	@sGUID)		
		
		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDTrasferimento.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDTrasferimento') as ValoreParametro(IDTrasferimento))
	IF 	ISNULL(@sGUID,'') <> '' 
			SET @uIDTrasferimento=CONVERT(UNIQUEIDENTIFIER,	@sGUID)	
			
		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDCartella.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDCartella') as ValoreParametro(IDCartella))
	IF 	ISNULL(@sGUID,'') <> '' 
			SET @uIDCartella=CONVERT(UNIQUEIDENTIFIER,	@sGUID)	

		SET @sCodReport=(SELECT TOP 1 ValoreParametro.CodReport.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodReport') as ValoreParametro(CodReport))
	
		SET  @txtDocumento =(SELECT TOP 1 ValoreParametro.Documento.value('.','VARCHAR(MAX)')
					  FROM @xParametri.nodes('/Parametri/Documento') as ValoreParametro(Documento)) 	
					  
	IF @txtDocumento IS NOT NULL
		SET @binDocumento=CAST(N'' AS xml).value('xs:base64Binary(sql:variable("@txtDocumento"))', 'varbinary(max)')
				
		SET @sDataTmp=(SELECT TOP 1 ValoreParametro.DataEvento.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/DataEvento') as ValoreParametro(DataEvento))					  
	SET @sDataTmp=ISNULL(@sDataTmp,'')
	

	IF @sDataTmp<> '' 
		BEGIN			
						SET @sDataTmp= SUBSTRING(@sDataTmp,4,2) + '-' 
						+ LEFT(@sDataTmp,2) + '-' +
						+ SUBSTRING(@sDataTmp,7,4) +
						' ' + RIGHT(@sDataTmp,5)
			IF ISDATE(@sDataTmp)=1
				SET	@dDataEvento=CONVERT(DATETIME,@sDataTmp,120)						
			ELSE
				SET	@dDataEvento =NULL			
		END
		
		SET @sDataTmp=(SELECT TOP 1 ValoreParametro.DataEventoUTC.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/DataEventoUTC') as ValoreParametro(DataEventoUTC))					  
	SET @sDataTmp=ISNULL(@sDataTmp,'')
	
		IF @sDataTmp<> '' 
		BEGIN			
						SET @sDataTmp= SUBSTRING(@sDataTmp,4,2) + '-' 
						+ LEFT(@sDataTmp,2) + '-' +
						+ SUBSTRING(@sDataTmp,7,4) +
						' ' + RIGHT(@sDataTmp,5)
			IF ISDATE(@sDataTmp)=1
				SET	@dDataEventoUTC=CONVERT(DATETIME,@sDataTmp,120)						
			ELSE
				SET	@dDataEventoUTC =NULL			
		END
	
		SET @sCodLogin=(SELECT TOP 1 ValoreParametro.CodLogin.value('.','VARCHAR(100)')
					  FROM @xParametri.nodes('/Parametri/TimeStamp/CodLogin') as ValoreParametro(CodLogin))	
	IF ISNULL(@sCodLogin, '') = ''
	BEGIN
		SET @sCodLogin = NULL
	END
	
		SET @sCodRuolo=(SELECT TOP 1 ValoreParametro.CodRuolo.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/TimeStamp/CodRuolo') as ValoreParametro(CodRuolo))	
	IF ISNULL(@sCodRuolo, '') = ''
	BEGIN
		SET @sCodRuolo = NULL
	END
	
		SET @sNomePC=(SELECT TOP 1 ValoreParametro.NomePC.value('.','VARCHAR(255)')
					  FROM @xParametri.nodes('/Parametri/TimeStamp/NomePC') as ValoreParametro(NomePC))	
	SET @sNomePC=ISNULL(@sNomePC,'')
	
		SET @sIndirizzoIP=(SELECT TOP 1 ValoreParametro.IndirizzoIP.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/TimeStamp/IndirizzoIP') as ValoreParametro(IndirizzoIP))	
	SET @sIndirizzoIP=ISNULL(@sIndirizzoIP,'')
	
	
	    SET @xTimeStamp=(SELECT TOP 1 ValoreParametro.TS.query('.')
					  FROM @xParametri.nodes('/Parametri/TimeStamp') as ValoreParametro(TS))
	
	
					
	SET @uGUID=NEWID()
	
	INSERT INTO T_MovReport
           (ID
           ,IDPaziente
           ,IDEpisodio
           ,IDTrasferimento
           ,IDCartella
           ,CodReport
           ,Documento
           ,DataEvento
           ,DataEventoUTC
           ,CodLogin
           ,CodRuolo
           ,NomePC
           ,IndirizzoIP)
    VALUES
		(
			@uGUID									           ,@uIDPaziente							           ,@uIDEpisodio							           ,@uIDTrasferimento						           ,@uIDCartella							           ,@sCodReport								           ,@binDocumento							           ,ISNULL(@dDataEvento,Getdate())			           ,ISNULL(@dDataEventoUTC,GetUTCdate())	           ,@sCodLogin								           ,@sCodRuolo								           ,@sNomePC								           ,@sIndirizzoIP							           
         )  
      

					SET @xTimeStamp.modify('delete (/TimeStamp/IDEntita)[1]') 
			
	SET @xTimeStamp.modify('insert <IDEntita>{sql:variable("@uGUID")}</IDEntita> into (/TimeStamp)[1]')

		SET @xTimeStampBase=@xTimeStamp


	SET @xTimeStamp=CONVERT(XML,
						'<Parametri>' + CONVERT(varchar(max),@xTimeStamp) +
						'</Parametri>')

		RETURN 0
END