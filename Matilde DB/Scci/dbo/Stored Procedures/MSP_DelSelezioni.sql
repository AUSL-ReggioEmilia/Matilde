CREATE PROCEDURE [dbo].[MSP_DelSelezioni](@xParametri AS XML)
AS
BEGIN
	
											
		DECLARE @sCodice AS VARCHAR(20)
	
	
		IF @xParametri.exist('/Parametri/Codice')=1
		BEGIN
					SET @sCodice=(SELECT TOP 1 ValoreParametro.Codice.value('.','VARCHAR(20)')
						  FROM @xParametri.nodes('/Parametri/Codice') as ValoreParametro(Codice))	
			
			IF ISNULL(@sCodice,'')<>''
			BEGIN
				DELETE FROM T_Selezioni
				WHERE Codice=@sCodice
			END
		END
				

	
												
	RETURN 0
END