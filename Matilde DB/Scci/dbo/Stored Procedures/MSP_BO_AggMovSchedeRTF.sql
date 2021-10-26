CREATE PROCEDURE [dbo].[MSP_BO_AggMovSchedeRTF](@uIDScheda AS UNIQUEIDENTIFIER,
												@sAnteprimaRTF AS VARCHAR(MAX),
												@sAnteprimaTXT AS VARCHAR(MAX),
												@sDatiObbligatoriMancantiRTF AS VARCHAR(MAX),
												@sDatiRilievoRTF AS VARCHAR(MAX))
AS
BEGIN	
	IF @uIDScheda IS NOT NULL
	BEGIN
		UPDATE T_MovSchede
		SET AnteprimaRTF=@sAnteprimaRTF,
			AnteprimaTXT=@sAnteprimaTXT,
			DatiObbligatoriMancantiRTF=@sDatiObbligatoriMancantiRTF,
			DatiRilievoRTF=@sDatiRilievoRTF
		WHERE ID=@uIDScheda
	END	
	RETURN 0						 	
END