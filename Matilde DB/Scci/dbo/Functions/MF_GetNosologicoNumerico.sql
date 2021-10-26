CREATE FUNCTION [dbo].[MF_GetNosologicoNumerico](@sNosologico AS VARCHAR(500), @bEpisodioDH AS BIT)
RETURNS VARCHAR(500)
AS
BEGIN
	DECLARE @sOut AS VARCHAR(500)	
	DECLARE @sDecodI AS VARCHAR(2)
	
	SET @sOut=''
	
	IF (ISNULL(@bEpisodioDH, 0) = 1)
		SET @sDecodI = '12'
	ELSE
		SET @sDecodI = '10'
	
	IF LTRIM(ISNULL(@sNosologico,''))='' 
		SET @sOut=''
	ELSE
		BEGIN
			SET @sOut=RTRIM(LTRIM(@sNosologico))
			IF LEFT(@sOut,1) IN ('I','D','P') AND LEFT(@sOut,3)<> 'DSA'
			BEGIN
				SET @sOut=
							CASE
								WHEN LEFT(@sOut,1) IN ('I') THEN @sDecodI 								WHEN LEFT(@sOut,1) IN ('D') THEN '11'
								WHEN LEFT(@sOut,1) IN ('P') THEN '13'
								ELSE LEFT(@sOut,1)
							END					
							+ RIGHT(@sOut,LEN(@sOut)-1)
			END	
			ELSE
			BEGIN	
				IF LEFT(@sOut,3) = 'DSA'
					SET @sOut='11'+ RIGHT(@sOut,LEN(@sOut)-3)				
			END
		END
	
	RETURN @sOut

END