CREATE PROCEDURE [dbo].[MSP_ControlloRipianificazioneTask](@xParametri AS XML )
AS
BEGIN
	
	
		DECLARE @sCodTipoTaskInfermieristico AS VARCHAR(20)
	DECLARE @uIDEpisodio AS UNIQUEIDENTIFIER

	DECLARE @bEsito AS INTEGER
	
		DECLARE @nNumeroTaskConfigurati AS INTEGER
	DECLARE @nNumeroTaskProgrammati AS INTEGER

			
		SET @sCodTipoTaskInfermieristico=(SELECT	TOP 1 ValoreParametro.CodTipoTaskInfermieristico.value('.','VARCHAR(20)')
						 FROM @xParametri.nodes('/Parametri/CodTipoTaskInfermieristico') as ValoreParametro(CodTipoTaskInfermieristico))

		SET @uIDEpisodio=(SELECT TOP 1 ValoreParametro.IDEpisodio.value('.','UNIQUEIDENTIFIER')
					  FROM @xParametri.nodes('/Parametri/IDEpisodio') as ValoreParametro(IDEpisodio))		


			
		SET @nNumeroTaskConfigurati=(SELECT TOP 1 Ripianificazione FROM T_TipoTaskInfermieristico WHERE Codice=@sCodTipoTaskInfermieristico)
	
	SET @nNumeroTaskConfigurati=ISNULL(@nNumeroTaskConfigurati,0)
	
		IF @nNumeroTaskConfigurati > 0
	BEGIN
		
		SET @nNumeroTaskProgrammati=(SELECT COUNT(*)
									 FROM T_MovTaskInfermieristici 
									 WHERE 
										IDEpisodio=@uIDEpisodio AND																			CodStatoTaskInfermieristico='PR' AND																CodTipoTaskInfermieristico=@sCodTipoTaskInfermieristico												)
	END

	SET @nNumeroTaskProgrammati=ISNULL(@nNumeroTaskProgrammati,0)

		IF @nNumeroTaskProgrammati < @nNumeroTaskConfigurati 
		SET @bEsito = 1
	ELSE
		SET @bEsito = 0 


		SELECT 	
		@nNumeroTaskConfigurati AS NumeroTaskConfigurati,
		@nNumeroTaskProgrammati AS NumeroTaskProgrammati,
		CONVERT(INTEGER,@bEsito) AS Esito
	
	RETURN 0				
END