CREATE FUNCTION [dbo].[MF_NumeroCartellaDaBarcode](@sBarcode AS VARCHAR(500))
RETURNS VARCHAR(500)
AS
BEGIN

	DECLARE @sOut AS VARCHAR(500)	
	DECLARE @nPosizioneSimbolo AS INTEGER

	SET @sOut=''
	
	IF (ISNULL(@sBarcode,'') <> '')
	BEGIN						
		SET @nPosizioneSimbolo = PATINDEX('%#%',@sBarcode)
		IF (@nPosizioneSimbolo > 1)
			BEGIN
				SET @sOut = RIGHT(@sBarcode,LEN(@sBarcode)-@nPosizioneSimbolo)
			END
		ELSE
			BEGIN
				SET @sOut = @sBarcode
			END
								
	END
	RETURN @sOut

END