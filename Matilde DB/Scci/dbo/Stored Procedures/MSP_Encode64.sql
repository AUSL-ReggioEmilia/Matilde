CREATE PROCEDURE MSP_Encode64(@sIn AS VARCHAR(MAX)) 
AS
BEGIN
	DECLARE @sOut AS VARCHAR(MAX)
	DECLARE @sInbin VARBINARY(max)

	SET @sInbin = CONVERT(VARBINARY(max), @sIn)
	SET @sOut = CAST(N'' AS xml).value('xs:base64Binary(sql:variable("@sInbin"))', 'varchar(max)') 

	SELECT @sOut AS Risultato
END