CREATE PROCEDURE [dbo].[MSP_SelOggettiCollegati](@xParametri AS XML)
AS
BEGIN
			
	
				DECLARE @uIDCartella AS UNIQUEIDENTIFIER
	
		DECLARE @xPar AS XML
	DECLARE @uIDCartellaCollegata AS UNIQUEIDENTIFIER			DECLARE @sCodStatoCartella AS VARCHAR(20)
	DECLARE @sCodStatoCartellaCollegata  AS VARCHAR(20)
	DECLARE @uIDEpisodioCollegato AS UNIQUEIDENTIFIER
	DECLARE @uIDTrasferimentoCollegato AS UNIQUEIDENTIFIER
	DECLARE @uIDPazienteCollegato AS UNIQUEIDENTIFIER

	
		SET @uIDCartella=(SELECT TOP 1 ValoreParametro.IDCartella.value('.','UNIQUEIDENTIFIER')
					  FROM @xParametri.nodes('/Parametri/IDCartella') as ValoreParametro(IDCartella));


	SET @sCodStatoCartella=(SELECT CodStatoCartella FROM T_MovCartelle WHERE ID=@uIDCartella)

		IF ISNULL(@sCodStatoCartella,'') ='CH'
	BEGIN
		
				SET @uIDCartellaCollegata =(SELECT TOP 1  CR.IDEntitaCollegata
								FROM T_MovRelazioniEntita CR
								WHERE 
									CodEntita='CAR' AND 
									CodEntitaCollegata='CAR' AND 
									IDEntita=@uIDCartella
								ORDER BY idnum DESC)
	
				IF @uIDCartellaCollegata IS NOT NULL
		BEGIN				
						SET @sCodStatoCartellaCollegata=(SELECT CodStatoCartella FROM T_MovCartelle WHERE ID=@uIDCartellaCollegata)
			IF ISNULL(@sCodStatoCartellaCollegata,'') ='AP'
			BEGIN				
								SELECT TOP 1
					@uIDCartellaCollegata = T.IDCartella,
					@uIDEpisodioCollegato = T.IDEpisodio, 
					@uIDTrasferimentoCollegato = T.ID,
					@uIDPazienteCollegato = P.IDPaziente
				FROM T_MovTrasferimenti T
					INNER JOIN T_MovPazienti P
						ON T.IDEpisodio=P.IDEpisodio
				WHERE T.IDCartella= @uIDCartellaCollegata AND
					T.CodStatoTrasferimento <> 'CA'
			END 
		END
	END

	SELECT 
		@uIDPazienteCollegato  AS IDPazienteCollegato,
		@uIDEpisodioCollegato AS IDEpisodioCollegato,
		@uIDTrasferimentoCollegato AS IDTrasferimentoCollegato,
		@uIDCartellaCollegata AS IDCartellaCollegata

	END