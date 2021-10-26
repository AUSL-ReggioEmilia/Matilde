CREATE PROCEDURE [dbo].[MSP_CercaCartellaSuTrasferimento](@xParametri XML)
AS
BEGIN


		

				
	DECLARE @uIDTrasferimento AS UNIQUEIDENTIFIER	
	DECLARE @uIDTrasferimentoPrecedente AS UNIQUEIDENTIFIER	
	
		DECLARE @sGUID AS VARCHAR(50)	
	DECLARE @uIDCartella AS UNIQUEIDENTIFIER	
	DECLARE @sCodUA AS VARCHAR(20)
	DECLARE @sCodUAPrecedente AS VARCHAR(20)
	DECLARE @sCodUANumerazione AS VARCHAR(20)
	DECLARE @sCodUANumerazionePrecedente AS VARCHAR(20)
	
	SET @uIDTrasferimento=NULL
	SET @uIDTrasferimentoPrecedente=NULL
	SET @uIDCartella=NULL
	
		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDTrasferimento.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDTrasferimento') as ValoreParametro(IDTrasferimento))
	IF 	ISNULL(@sGUID,'') <> '' SET @uIDTrasferimento=CONVERT(UNIQUEIDENTIFIER,	@sGUID)	
	
		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDTrasferimentoPrecedente.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDTrasferimentoPrecedente') as ValoreParametro(IDTrasferimentoPrecedente))
	IF 	ISNULL(@sGUID,'') <> '' SET @uIDTrasferimentoPrecedente=CONVERT(UNIQUEIDENTIFIER,@sGUID)	
	
	IF @uIDTrasferimento IS NOT NULL AND @uIDTrasferimentoPrecedente IS NOT NULL
	BEGIN
	
				SELECT TOP 1 
			@uIDCartella=IDCartella,
			@sCodUAPrecedente=CodUA		
		FROM T_MovTrasferimenti 
		WHERE ID=@uIDTrasferimentoPrecedente
				
				SELECT TOP 1 			
			@sCodUA=CodUA
		FROM T_MovTrasferimenti 
		WHERE ID=@uIDTrasferimento

		IF ISNULL(@sCodUAPrecedente,'')=ISNULL(@sCodUA,'')
		BEGIN
						SELECT 			
			CASE
				WHEN @uIDCartella IS NULL THEN 0
				ELSE 1
			END AS Esito, @uIDCartella  AS IDCartella							END
		ELSE
		BEGIN		
						SET @sCodUANumerazione=(SELECT CodUANumerazioneCartella FROM T_UnitaAtomiche WHERE Codice=@sCodUA)
			SET @sCodUANumerazionePrecedente=(SELECT CodUANumerazioneCartella FROM T_UnitaAtomiche WHERE Codice=@sCodUAPrecedente)
			
			IF @sCodUANumerazione IS NOT NULL AND @sCodUANumerazionePrecedente IS NOT NULL
			BEGIN
										IF @sCodUANumerazione=@sCodUANumerazionePrecedente 
					BEGIN
						SELECT 			
							CASE
								WHEN @uIDCartella IS NULL THEN 0
								ELSE 1
							END AS Esito, @uIDCartella  AS IDCartella					
					END
					ELSE
					BEGIN
												SELECT 0 AS Esito, NULL AS IDCartella												END					
			END
			ELSE
								SELECT 0 AS Esito, NULL AS IDCartella									END			
	END
	ELSE
				SELECT 0 AS Esito, NULL AS IDCartella  
	RETURN 0
				
END