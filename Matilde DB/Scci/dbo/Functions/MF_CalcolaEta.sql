CREATE FUNCTION [dbo].[MF_CalcolaEta](@dDataNascita AS DateTime, @dDataRif AS DATETIME)
RETURNS SMALLINT
AS
BEGIN
	DECLARE @nOut AS SMALLINT
	
	SET @nOut=
						
						
				DATEDIFF(hour,@dDataNascita, @dDataRif )/8766 
	
	RETURN @nOut

END