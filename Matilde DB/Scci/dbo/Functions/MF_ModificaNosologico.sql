CREATE FUNCTION [dbo].[MF_ModificaNosologico](@sNosologico AS VARCHAR(500))
RETURNS VARCHAR(500)
AS
BEGIN
	DECLARE @sOut AS VARCHAR(500)	
	
	SET @sOut=''
	
	IF LTRIM(ISNULL(@sNosologico,''))='' 
		SET @sOut=''
	ELSE
		BEGIN
			SET @sOut=RTRIM(LTRIM(@sNosologico))
			IF LEFT(@sOut,2) IN ('10','11','12','13') AND ISNUMERIC(RIGHT(@sOut,LEN(@sOut)-2))=1
			BEGIN
				SET @sOut=
							CASE
								WHEN LEFT(@sOut,2) IN ('10','12') THEN 'I'
								WHEN LEFT(@sOut,2) IN ('11') THEN 'D'
								WHEN LEFT(@sOut,2) IN ('13') THEN 'P'
								ELSE LEFT(@sOut,2)
							END					
							+ RIGHT(@sOut,LEN(@sOut)-2)
			END	
		END
	
	RETURN @sOut

END