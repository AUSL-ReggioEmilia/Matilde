CREATE PROCEDURE [dbo].[MSP_SelConfigUtente](@xParametri XML)
AS
BEGIN
		
	
				DECLARE @sCodice AS VARCHAR(20)
	
	
	SET @sCodice=(SELECT TOP 1 ValoreParametro.Codice.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/Codice') as ValoreParametro(Codice))	
	SET @sCodice=ISNULL(@sCodice,'')

	

					

	SELECT Valore FROM T_ConfigUtente
		WHERE Codice=@sCodice				
	
END