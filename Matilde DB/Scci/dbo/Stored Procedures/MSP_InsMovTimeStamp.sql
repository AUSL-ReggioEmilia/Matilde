CREATE PROCEDURE [dbo].[MSP_InsMovTimeStamp](@xParametri AS XML)
AS
BEGIN
	

	DECLARE @sCodLogin		VARCHAR(100)
	DECLARE @sCodRuolo		VARCHAR(20)
	DECLARE @sNomePC		VARCHAR(255)
	DECLARE @sIndirizzoIP	VARCHAR(50)
	DECLARE @sCodEntita		VARCHAR(20)
	DECLARE @uIDEntita		UNIQUEIDENTIFIER
	DECLARE @sCodAzione		VARCHAR(20)
	DECLARE @uIDPaziente AS UNIQUEIDENTIFIER	
	DECLARE @uIDEpisodio AS UNIQUEIDENTIFIER	
	DECLARE @uIDTrasferimento AS UNIQUEIDENTIFIER	
	DECLARE @sNote			VARCHAR(255)
	DECLARE @sInfo			VARCHAR(MAX)
	
	DECLARE @sIDEntita	VARCHAR(50)
	DECLARE @sGUID AS VARCHAR(50)
	DECLARE @uGUID AS UNIQUEIDENTIFIER
	
	DECLARE @dtDataOra AS DATETIME
	DECLARE @dtDataOraUTC AS DATETIME

	DECLARE @bRegistraTimeStamp BIT
			
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
	
		SET @sCodEntita=(SELECT TOP 1 ValoreParametro.CodEntita.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/TimeStamp/CodEntita') as ValoreParametro(CodEntita))	
	SET @sCodEntita=ISNULL(@sCodEntita,'')
		
		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDEntita.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/TimeStamp/IDEntita') as ValoreParametro(IDEntita))
	IF 	ISNULL(@sGUID,'') <> '' SET @uIDEntita=CONVERT(UNIQUEIDENTIFIER,	@sGUID)	
	
		SET	@sCodAzione=(SELECT TOP 1 ValoreParametro.CodAzione.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/TimeStamp/CodAzione') as ValoreParametro(CodAzione))	
	SET @sCodAzione=ISNULL(@sCodAzione,'')
	
		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDPaziente.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/TimeStamp/IDPaziente') as ValoreParametro(IDPaziente))
	IF 	ISNULL(@sGUID,'') <> '' 
			SET @uIDPaziente=CONVERT(UNIQUEIDENTIFIER,	@sGUID)		
	
		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDEpisodio.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/TimeStamp/IDEpisodio') as ValoreParametro(IDEpisodio))
	IF 	ISNULL(@sGUID,'') <> '' 
			SET @uIDEpisodio=CONVERT(UNIQUEIDENTIFIER,	@sGUID)		

		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDTrasferimento.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/TimeStamp/IDTrasferimento') as ValoreParametro(IDTrasferimento))
	IF 	ISNULL(@sGUID,'') <> '' 
			SET @uIDTrasferimento=CONVERT(UNIQUEIDENTIFIER,	@sGUID)	
	
	IF @xParametri.exist('/Parametri/TimeStamp/Note')=1			
	BEGIN
				SET	@sNote=(SELECT TOP 1 ValoreParametro.Note.value('.','VARCHAR(255)')
					  FROM @xParametri.nodes('/Parametri/TimeStamp/Note') as ValoreParametro(Note))	
	END
	
		IF @xParametri.exist('/Parametri/TimeStamp/Info')=1						  
	BEGIN
		SET	@sInfo=(SELECT TOP 1 ValoreParametro.Info.value('.','VARCHAR(MAX)')
					  FROM @xParametri.nodes('/Parametri/TimeStamp/Info') as ValoreParametro(Info))
	END					  
	
	
		SET @bRegistraTimeStamp=(SELECT	TOP 1		
									ISNULL(RegistraTimeStamp,0) 					
								FROM 
									T_AzioniEntita
								WHERE			
									CodEntita=@sCodEntita AND
									CodAzione = @sCodAzione
							)					
	IF ISNULL(@bRegistraTimeStamp,0)=1
	BEGIN
				
				SET @uGUID=NEWID()
		SET @dtDataOra=GETDATE()
		SET @dtDataOraUTC=GETUTCDATE()
		
				INSERT INTO T_MovTimeStamp(
					ID
				  ,DataOra
				  ,DataOraUTC
				  ,CodLogin
				  ,CodRuolo
				  ,NomePC
				  ,IndirizzoIP
				  ,CodEntita
				  ,IDEntita
				  ,CodAzione
				  ,IDPaziente
				  ,IDEpisodio
				  ,IDTrasferimento
				  ,Note
				  ,Info
				  )
		VALUES
				(
					@uGUID								  ,@dtDataOra							  ,@dtDataOraUTC						  ,@sCodLogin							  ,@sCodRuolo							  ,@sNomePC								  ,@sIndirizzoIP						  ,@sCodEntita							  ,@uIDEntita							  ,@sCodAzione							  ,@uIDPaziente							  ,@uIDEpisodio							  ,@uIDTrasferimento					  ,@sNote								  ,@sInfo								)
				
					END	
										RETURN 0
END