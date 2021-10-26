CREATE FUNCTION [dbo].[MF_GetPathMovFolder](@uID AS UNIQUEIDENTIFIER)
RETURNS VARCHAR(255)
AS
BEGIN

	DECLARE @sOut AS VARCHAR(255);

	WITH CTE AS (
		SELECT 
			IDFolderPadre, ID, 
			CAST('> ' + Descrizione as VARCHAR(255)) [Folder]
		FROM T_MovFolder
		WHERE IDFolderPadre IS NULL

		UNION ALL

		SELECT 
			H.IDFolderPadre, H.ID, 
			CAST(C.[Folder] + '\' + H.Descrizione as VARCHAR(255)) [Folder] 
		FROM T_MovFolder H
		INNER JOIN CTE C ON C.ID = H.IDFolderPadre
	) 
	SELECT @sOut=[Folder] FROM CTE WHERE ID = @uID
	
	RETURN @sOut

END