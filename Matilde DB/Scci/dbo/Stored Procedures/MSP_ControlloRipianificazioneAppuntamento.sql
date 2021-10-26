
CREATE PROCEDURE [dbo].[MSP_ControlloRipianificazioneAppuntamento](@xParametri AS XML )
AS
BEGIN
	
	
		DECLARE @sCodTipoAppuntamento AS VARCHAR(20)
	DECLARE @uIDPaziente AS UNIQUEIDENTIFIER
	DECLARE @uIDEpisodio AS UNIQUEIDENTIFIER
	DECLARE @sIDEpisodio AS VARCHAR(50)

	DECLARE @bEsito AS INTEGER
	
		DECLARE @nNumeroAppuntamentiConfigurati AS INTEGER
	DECLARE @nNumeroAppuntamentiProgrammati AS INTEGER

			
		SET @sCodTipoAppuntamento=(SELECT	TOP 1 ValoreParametro.CodTipoAppuntamento.value('.','VARCHAR(20)')
						 FROM @xParametri.nodes('/Parametri/CodTipoAppuntamento') as ValoreParametro(CodTipoAppuntamento))

		SET @uIDPaziente=(SELECT TOP 1 ValoreParametro.IDPaziente.value('.','UNIQUEIDENTIFIER')
					  FROM @xParametri.nodes('/Parametri/IDPaziente') as ValoreParametro(IDPaziente))

		IF @xParametri.exist('/Parametri/IDEpisodio')=1
	BEGIN
		SET @sIDEpisodio=(SELECT TOP 1 ValoreParametro.IDEpisodio.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDEpisodio') as ValoreParametro(IDEpisodio))		
		IF ISNULL(@sIDEpisodio,'') <> ''			
				SET @uIDEpisodio=CONVERT(UNIQUEIDENTIFIER,@sIDEpisodio)			
	END
	ELSE
		SET @uIDEpisodio = NULL
	
	

			
		SET @nNumeroAppuntamentiConfigurati=(SELECT TOP 1 Ripianificazione FROM T_TipoAppuntamento WHERE Codice=@sCodTipoAppuntamento)
	
	SET @nNumeroAppuntamentiConfigurati=ISNULL(@nNumeroAppuntamentiConfigurati,0)
	
		IF @nNumeroAppuntamentiConfigurati > 0
	BEGIN
				SET @nNumeroAppuntamentiProgrammati=(SELECT COUNT(*)
									 FROM T_MovAppuntamenti 
									 WHERE 
										IDPaziente=@uIDPaziente AND			
										CodStatoAppuntamento IN ('PR') AND
										CodTipoAppuntamento=@sCodTipoAppuntamento		
										)
	END

	SET @nNumeroAppuntamentiProgrammati=ISNULL(@nNumeroAppuntamentiProgrammati,0)

		IF @nNumeroAppuntamentiProgrammati < @nNumeroAppuntamentiConfigurati 
		SET @bEsito = 1
	ELSE
		SET @bEsito = 0 


		SELECT 	
		@nNumeroAppuntamentiConfigurati AS NumeroAppuntamentiConfigurati,
		@nNumeroAppuntamentiProgrammati AS NumeroAppuntamentiProgrammati,
		CONVERT(INTEGER,@bEsito) AS Esito
	
	RETURN 0				
END