


CREATE PROCEDURE [dbo].[MSP_SelConfigUpdater]
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
		ID in (600,601,602,603,604,605,606,607,608,609)
END