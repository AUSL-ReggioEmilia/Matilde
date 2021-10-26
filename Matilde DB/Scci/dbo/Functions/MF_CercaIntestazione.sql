CREATE FUNCTION [dbo].[MF_CercaIntestazione]
	(
		@sCodIntestazione AS VARCHAR(20),
		@dDataRiferimento AS DATETIME,
		@sCodUA AS VARCHAR(20)
	)
RETURNS VARCHAR(MAX)
AS
BEGIN
	DECLARE @sRet AS  VARCHAR(MAX)

	
				SET @sRet=(SELECT TOP 1 Intestazione
			   FROM T_AssUAIntestazioni
			   WHERE 
				CodUA=@sCodUA AND
				CodIntestazione=@sCodIntestazione AND
				@dDataRiferimento BETWEEN DataInizio AND ISNULL(DataFine,CONVERT(DATETIME,'2200-01-01',120))
				)

			
				IF (@sRet IS NULL)
	BEGIN				
				IF (@sCodIntestazione='CARFIRMA')
		BEGIN
			SET @sRet=(SELECT TOP 1 FirmaCartella FROM T_UnitaAtomiche WITH (NOLOCK) WHERE Codice=@sCodUA)
		END
		
				IF (@sCodIntestazione='CARTSINT')
		BEGIN
			SET @sRet=(SELECT TOP 1 IntestazioneSintetica FROM T_UnitaAtomiche WITH (NOLOCK) WHERE Codice=@sCodUA)
		END
	
				IF (@sCodIntestazione='CARTSTD')
		BEGIN
			SET @sRet=(SELECT TOP 1 IntestazioneCartella FROM T_UnitaAtomiche WITH (NOLOCK) WHERE Codice=@sCodUA)
		END
	
				IF (@sCodIntestazione='SPALLASX')
		BEGIN
			SET @sRet=(SELECT TOP 1 SpallaSinistra FROM T_UnitaAtomiche WITH (NOLOCK) WHERE Codice=@sCodUA)
		END
	END 

	RETURN @sRet
END