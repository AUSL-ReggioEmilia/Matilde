CREATE PROCEDURE [dbo].[MSP_AggContatore](@xParametri AS XML )
AS
BEGIN
	
	
	DECLARE @sCodContatore AS VARCHAR(20)
	DECLARE @nValore	AS INTEGER	
	DECLARE @sCodLogin AS Varchar(100)
	DECLARE @bImposta AS BIT	
		
		SET @bImposta=(SELECT TOP 1 ValoreParametro.Imposta.value('.','bit')
					  FROM @xParametri.nodes('/Parametri/Imposta') as ValoreParametro(Imposta))											
					 
	SET @bImposta=ISNULL(@bImposta,0)
	
		SET @sCodContatore=(SELECT	TOP 1 ValoreParametro.CodContatore.value('.','VARCHAR(20)')
						 FROM @xParametri.nodes('/Parametri/CodContatore') as ValoreParametro(CodContatore))
						 
		SET @nValore=(SELECT	TOP 1 ValoreParametro.Valore.value('.','INTEGER')
						 FROM @xParametri.nodes('/Parametri/Valore') as ValoreParametro(Valore))					 
	
		SET @sCodLogin=(SELECT	TOP 1 ValoreParametro.CodLogin.value('.','VARCHAR(100)')
						 FROM @xParametri.nodes('/Parametri/TimeStamp/CodLogin') as ValoreParametro(CodLogin))						 
	SET @sCodLogin= LTRIM(RTRIM(ISNULL(@sCodLogin,'')))	
	
	
	IF @sCodContatore IS NOT NULL AND @nValore IS NOT NULL
	BEGIN		
		UPDATE T_Contatori
		SET 
			Valore=	@nValore,
			DataImpostazione= CASE 
								WHEN @bImposta=1 THEN GETDATE()
								ELSE DataImpostazione
							  END,
			DataImpostazioneUTC = CASE 
								WHEN @bImposta=1 THEN GETUTCDATE()
								ELSE DataImpostazioneUTC
							  END, 	
			CodUtenteImpostazione=	CASE 
								WHEN @bImposta=1 THEN @sCodLogin
								ELSE CodUtenteImpostazione
							  END, 				  			  	
			DataUltimaModifica=GETDATE(),
			DataUltimaModificaUTC=GETUTCDATE(),
			CodUtenteUltimaModifica=@sCodLogin
		WHERE Codice=@sCodContatore
		
	END
	RETURN 0				
END