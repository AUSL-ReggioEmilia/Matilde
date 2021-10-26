CREATE PROCEDURE [dbo].[MSP_SelConfigCE](@nID AS INT)
AS
BEGIN
		
	
				
	SELECT	
		ID, 
		Descrizione,
		Valore,
		Immagine
	FROM 
		T_ConfigCE
	WHERE 
		ID=@nID
END