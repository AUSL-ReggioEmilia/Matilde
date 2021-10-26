CREATE PROCEDURE [dbo].[MSP_SelContatore](@xParametri AS XML )
AS
BEGIN
	
	
	DECLARE @sCodContatore AS VARCHAR(20)
	
		SET @sCodContatore=(SELECT	TOP 1 ValoreParametro.CodContatore.value('.','VARCHAR(20)')
						 FROM @xParametri.nodes('/Parametri/CodContatore') as ValoreParametro(CodContatore))
	
	
	SELECT Valore
	FROM T_Contatori
	WHERE Codice=@sCodContatore
	
	
	RETURN 0				
END