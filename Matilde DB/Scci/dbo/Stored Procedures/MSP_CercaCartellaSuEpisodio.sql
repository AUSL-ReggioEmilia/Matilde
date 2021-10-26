CREATE PROCEDURE [dbo].MSP_CercaCartellaSuEpisodio(@xParametri XML)
AS
BEGIN
		

					
	DECLARE @uIDEpisodio AS UNIQUEIDENTIFIER	
	DECLARE @uIDTrasferimento AS UNIQUEIDENTIFIER		
	
		DECLARE @sGUID AS VARCHAR(50)	
	DECLARE @uIDCartella AS UNIQUEIDENTIFIER		
	DECLARE @sCodUA AS VARCHAR(20)	
	DECLARE @dDataOraIngressoUTC AS DATETIME
	
	DECLARE @sCodUANumerazione AS VARCHAR(20)
					
				 
	SET @uIDEpisodio=NULL
	SET @uIDTrasferimento=NULL		
	
		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDEpisodio.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDEpisodio') as ValoreParametro(IDEpisodio))
	IF 	ISNULL(@sGUID,'') <> '' SET @uIDEpisodio=CONVERT(UNIQUEIDENTIFIER,	@sGUID)	

		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDTrasferimento.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDTrasferimento') as ValoreParametro(IDTrasferimento))
	IF 	ISNULL(@sGUID,'') <> '' SET @uIDTrasferimento=CONVERT(UNIQUEIDENTIFIER,	@sGUID)	
	
			
	IF (@uIDEpisodio IS NOT NULL AND @uIDTrasferimento IS NOT NULL )
	BEGIN
		SET @uIDCartella=NULL

			SELECT TOP 1 
				@sCodUA= CodUA,
				@uIDCartella=IDCartella,
				@dDataOraIngressoUTC=DataIngressoUTC
			FROM T_MovTrasferimenti 
			WHERE ID=@uIDTrasferimento
		
				IF (@uIDCartella IS NULL)
		BEGIN
			SET @sCodUANumerazione=(SELECT ISNULL(CodUANumerazioneCartella,@sCodUA) FROM T_UnitaAtomiche WHERE Codice=@sCodUA)
									
			SET @uIDCartella =
				(	SELECT TOP 1
						T.IDCartella
					FROM T_MovTrasferimenti T
							INNER JOIN T_MovCartelle C
								ON T.IDCartella=C.ID
							INNER JOIN T_UnitaAtomiche A
								ON T.CodUA=A.Codice
					WHERE	
						T.IDEpisodio=@uIDEpisodio AND															T.CodStatoTrasferimento NOT IN ('CA') AND												T.ID <> @uIDTrasferimento AND															C.CodStatoCartella <> 'CA' AND															ISNULL(A.CodUANumerazioneCartella,T.CodUA) = @sCodUANumerazione						ORDER BY T.DataIngresso DESC														)

			IF @uIDCartella IS NOT NULL
			BEGIN				
				SELECT 1 AS Esito, @uIDCartella AS IDCartella										
			END
			ELSE
			BEGIN			
				SELECT 0 AS Esito, NULL AS IDCartella												
			END
		END
		ELSE
		BEGIN
						SELECT 0 AS Esito, @uIDCartella AS IDCartella
		END
						

	END
	ELSE
				SELECT 0 AS Esito, NULL AS IDCartella  
	RETURN 0
				
END