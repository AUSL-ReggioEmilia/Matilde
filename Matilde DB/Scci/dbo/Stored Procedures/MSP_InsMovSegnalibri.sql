CREATE PROCEDURE [dbo].[MSP_InsMovSegnalibri](@xParametri AS XML)
AS
BEGIN
	
											
		DECLARE @sCodRuolo AS VARCHAR(20)
	DECLARE @sCodUtente AS VARCHAR(100)
	DECLARE @uIDPaziente AS UNIQUEIDENTIFIER		
	DECLARE @uIDCartella AS UNIQUEIDENTIFIER
	DECLARE @uIDTrasferimento AS UNIQUEIDENTIFIER
	DECLARE @sCodEntita AS VARCHAR(20)	
	DECLARE @uIDEntita AS UNIQUEIDENTIFIER	
	DECLARE @sCodEntitaScheda AS VARCHAR(20)	
	DECLARE @sCodScheda AS VARCHAR(20)	
	DECLARE @nNumero AS INTEGER	
	DECLARE @dDataInserimento AS DATETIME	
	DECLARE @dDataInserimentoUTC AS DATETIME
		
		DECLARE @sGUID AS VARCHAR(50)
	DECLARE @uGUID AS UNIQUEIDENTIFIER
	DECLARE @sDataTmp AS VARCHAR(20)	
	DECLARE @nQTA AS INTEGER
	
		
	    DECLARE @xTimeStampBase AS XML	
	DECLARE @xTimeStamp AS XML	
	
		SET @sCodUtente=(SELECT TOP 1 ValoreParametro.CodUtente.value('.','VARCHAR(100)')
					  FROM @xParametri.nodes('/Parametri/TimeStamp/CodLogin') as ValoreParametro(CodUtente))	
	SET @sCodUtente=ISNULL(@sCodUtente,'')

		SET @sCodRuolo=(SELECT TOP 1 ValoreParametro.CodRuolo.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/TimeStamp/CodRuolo') as ValoreParametro(CodRuolo))
	SET @sCodRuolo=ISNULL(@sCodRuolo,'')
	
		SET @sCodEntitaScheda=(SELECT TOP 1 ValoreParametro.CodEntitaScheda.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodEntitaScheda') as ValoreParametro(CodEntitaScheda))	
					  
		SET @sCodScheda=(SELECT TOP 1 ValoreParametro.CodScheda.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodScheda') as ValoreParametro(CodScheda))	
	
		SET @nNumero=(SELECT TOP 1 ValoreParametro.Numero.value('.','INTEGER')
					  FROM @xParametri.nodes('/Parametri/Numero') as ValoreParametro(Numero))
	
	
		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDPaziente.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDPaziente') as ValoreParametro(IDPaziente))
	IF 	ISNULL(@sGUID,'') <> '' 
			SET @uIDPaziente=CONVERT(UNIQUEIDENTIFIER,	@sGUID)				
		
		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDEntita.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDEntita') as ValoreParametro(IDEntita))
	IF 	ISNULL(@sGUID,'') <> '' 
			SET @uIDEntita=CONVERT(UNIQUEIDENTIFIER,	@sGUID)	
			
		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDCartella.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDCartella') as ValoreParametro(IDCartella))
	IF 	ISNULL(@sGUID,'') <> '' 
			SET @uIDCartella=CONVERT(UNIQUEIDENTIFIER,	@sGUID)	
	
		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDTrasferimento.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/TimeStamp/IDTrasferimento') as ValoreParametro(IDTrasferimento))
	IF 	ISNULL(@sGUID,'') <> '' 
			SET @uIDTrasferimento=CONVERT(UNIQUEIDENTIFIER,	@sGUID)	
			
		SET @sCodEntita=(SELECT TOP 1 ValoreParametro.CodEntita.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodEntita') as ValoreParametro(CodEntita))	
				
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
				SET	@dDataInserimento=CONVERT(DATETIME,@sDataTmp,120)						
			ELSE
				SET	@dDataInserimento =NULL			
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
				SET	@dDataInserimentoUTC=CONVERT(DATETIME,@sDataTmp,120)						
			ELSE
				SET	@dDataInserimentoUTC =NULL			
		END
	
	    SET @xTimeStamp=(SELECT TOP 1 ValoreParametro.TS.query('.')
					  FROM @xParametri.nodes('/Parametri/TimeStamp') as ValoreParametro(TS))
	
	
				
	IF @uIDCartella IS NULL AND @uIDTrasferimento IS NOT NULL 
		SET @uIDCartella=(SELECT TOP 1 IDCartella FROM T_MovTrasferimenti WHERE ID=@uIDTrasferimento)
	
		
	SET @nQta=0
	
		IF @sCodEntita='SCH'
		BEGIN				
						IF @sCodEntitaScheda='PAZ' SET @uIDCartella=NULL
			SET @nQta=(SELECT COUNT(*)
					   FROM T_MovSegnalibri
					   WHERE 
							CodEntitaScheda=@sCodEntitaScheda AND
							CodScheda=@sCodScheda AND
							Numero=@nNumero AND
							CodUtente=@sCodUtente AND
							CodRuolo=@sCodRuolo AND
							IDEntita=@uIDEntita
						)
		END
	
	IF @nQta=0
		BEGIN
			SET @uGUID=NEWID()
			
			INSERT INTO T_MovSegnalibri
				   (ID
				   ,CodUtente
				   ,CodRuolo
				   ,IDPaziente                      
				   ,IDCartella           
				   ,CodEntita
				   ,IDEntita
				   ,CodEntitaScheda
				   ,CodScheda
				   ,Numero
				   ,DataInserimento
				   ,DataInserimentoUTC
				   )
			VALUES
				(			
					@uGUID													   ,@sCodUtente												   ,@sCodRuolo												   ,@uIDPaziente											   ,@uIDCartella											   ,@sCodEntita												   ,@uIDEntita												   ,@sCodEntitaScheda										   ,@sCodScheda												   ,@nNumero												   ,ISNULL(@dDataInserimento,Getdate())							   ,ISNULL(@dDataInserimentoUTC,GetUTCdate())					 )  
		END	      
	
	SELECT @uGUID AS  IDSegnalibro
															
	RETURN 0
END