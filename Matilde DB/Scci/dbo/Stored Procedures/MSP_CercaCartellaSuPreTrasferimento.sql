CREATE PROCEDURE [dbo].[MSP_CercaCartellaSuPreTrasferimento](@xParametri XML)
AS
BEGIN


		

				
	DECLARE @uIDTrasferimento AS UNIQUEIDENTIFIER		
	
		DECLARE @sGUID AS VARCHAR(50)	
	DECLARE @uIDCartella AS UNIQUEIDENTIFIER	
	
	DECLARE @sCodUA AS VARCHAR(20)
	DECLARE @sCodUAPreTrasferimento AS VARCHAR(20)
	
	DECLARE @sCodUANumerazione AS VARCHAR(20)
	DECLARE @sCodUANumerazionePreTrasferimento AS VARCHAR(20)
	
	DECLARE @uIDEpisodio AS UNIQUEIDENTIFIER	
	DECLARE @uIDPreTrasferimento AS UNIQUEIDENTIFIER	
	
	
	SET @uIDTrasferimento=NULL
	SET @uIDPreTrasferimento=NULL
	SET @uIDCartella=NULL
	
		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDTrasferimento.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDTrasferimento') as ValoreParametro(IDTrasferimento))
	IF 	ISNULL(@sGUID,'') <> '' SET @uIDTrasferimento=CONVERT(UNIQUEIDENTIFIER,	@sGUID)	
	
	
	IF @uIDTrasferimento IS NOT NULL 
	BEGIN
		SET @uIDEpisodio=(SELECT TOP 1 IDEpisodio FROM T_MovTrasferimenti WHERE ID=@uIDTrasferimento)
		SET @uIDPreTrasferimento=(SELECT TOP 1 ID
								  FROM T_MovTrasferimenti 
								  WHERE IDEpisodio=@uIDEpisodio AND													CodStatoTrasferimento='PT'											 )
		IF @uIDPreTrasferimento IS NOT NULL
		BEGIN		
						SELECT TOP 1 
				@uIDCartella=IDCartella,
				@sCodUAPreTrasferimento=CodUA		
			FROM T_MovTrasferimenti 
			WHERE ID=@uIDPreTrasferimento
					
						SELECT TOP 1 			
				@sCodUA=CodUA
			FROM T_MovTrasferimenti 
			WHERE ID=@uIDTrasferimento

			IF ISNULL(@sCodUAPreTrasferimento,'')=ISNULL(@sCodUA,'')
			BEGIN
								SELECT 			
				CASE
					WHEN @uIDCartella IS NULL THEN 0
					ELSE 1
				END AS Esito, @uIDCartella  AS IDCartella								END
			ELSE
			BEGIN		
								SET @sCodUANumerazione=(SELECT CodUANumerazioneCartella FROM T_UnitaAtomiche WHERE Codice=@sCodUA)
				SET @sCodUANumerazionePreTrasferimento=(SELECT CodUANumerazioneCartella FROM T_UnitaAtomiche WHERE Codice=@sCodUAPreTrasferimento)
				
				IF @sCodUANumerazione IS NOT NULL AND @sCodUANumerazionePreTrasferimento IS NOT NULL
				BEGIN
												IF @sCodUANumerazione=@sCodUANumerazionePreTrasferimento 
						BEGIN
							SELECT 			
								CASE
									WHEN @uIDCartella IS NULL THEN 0
									ELSE 1
								END AS Esito, @uIDCartella  AS IDCartella					
						END
						ELSE
						BEGIN
														SELECT 0 AS Esito, NULL AS IDCartella													END					
				END
				ELSE
										SELECT 0 AS Esito, NULL AS IDCartella										END		
		END	
	END
	ELSE
				SELECT 0 AS Esito, NULL AS IDCartella  
	RETURN 0
				
END