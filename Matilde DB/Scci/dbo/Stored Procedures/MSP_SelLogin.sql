
CREATE PROCEDURE [dbo].[MSP_SelLogin](@sCodLogin AS VARCHAR(100))
AS
BEGIN
		
	
				
	SELECT	
		L.Codice, 
		L.Descrizione,
		L.Cognome,
		L.Nome,
		L.FlagAdmin,
		L.Foto,
		IsNull(CE.Valore,'') As Valore,
		L.CodiceFiscale
	FROM 
		T_Login L
			LEFT JOIN 
				T_ConfigUtente CE
					ON L.Codice=CE.Codice
	WHERE 
		L.Codice=@sCodLogin AND
		isnull(L.FlagObsoleto,0)=0
END