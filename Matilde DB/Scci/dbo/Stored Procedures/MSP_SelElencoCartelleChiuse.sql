CREATE PROCEDURE [dbo].[MSP_SelElencoCartelleChiuse]
AS
BEGIN
	SELECT TOP 3 
	ID AS IDCartella,NumeroCartella
	FROM T_MovCartelle WITH (NOLOCK)		
	WHERE CodStatoCartella='CH' AND ID='fe968483-e736-4502-8429-f5f2863ca7c4'
	
		
	

					
END