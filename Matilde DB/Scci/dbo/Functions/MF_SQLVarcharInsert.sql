CREATE FUNCTION [dbo].[MF_SQLVarcharInsert](@sInput AS VARCHAR(MAX))
RETURNS VARCHAR(MAX)
AS
BEGIN

DECLARE @sOUT AS varchar(MAX)
DECLARE @bTmp AS VARBINARY(MAX)

SET @bTmp = CONVERT(varbinary(max), @sInput)

IF @sInput IS NOT NULL
	BEGIN
		SET @sOUT =CAST(N'' AS xml).value('xs:base64Binary(sql:variable("@bTmp"))', 'varchar(max)')
		SET @sOUT= 'CAST(N'''' AS xml).value(''xs:base64Binary("' +
								 @sOUT + '")'', ''varbinary(max)'')'
	END							 
ELSE
	SET @sOUT=''
	
RETURN 	@sOUT						 
END