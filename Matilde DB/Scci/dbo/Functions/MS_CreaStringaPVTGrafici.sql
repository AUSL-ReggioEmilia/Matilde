CREATE FUNCTION [dbo].[MS_CreaStringaPVTGrafici](@x AS XML)

RETURNS varchar(max)
AS
BEGIN
	DECLARE @ResultVar VARCHAR(MAX)
	
	SET @ResultVar=''
	
	IF @x IS NOT NULL
		SELECT @ResultVar = @ResultVar +
							CASE 
								WHEN @ResultVar <> '' THEN ',' 
								ELSE ''
							END	 +
																														ISNULL(ValoriPVT.value('(Valore)[1]','VARCHAR(255)')	,'')
		FROM @x.nodes('/ValoriPVT/ValorePVT') as ValoreParametro(ValoriPVT)

	RETURN @ResultVar

END