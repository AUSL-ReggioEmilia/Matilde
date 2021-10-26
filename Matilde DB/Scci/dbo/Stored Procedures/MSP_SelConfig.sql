

CREATE PROCEDURE [dbo].[MSP_SelConfig](@nID AS INT)
AS
BEGIN
		
	
				
	SELECT	
		ID, 
		Descrizione,
		Valore,
		Immagine
	FROM 
		T_Config
	WHERE 
		ID=@nID
END