CREATE FUNCTION [dbo].[MF_XMLUnEscape]( @s varchar(max)) returns varchar(max)
AS
BEGIN
  DECLARE @rs VARCHAR(MAX) 
  SET @rs = @s
  SET @rs = replace(@rs, '&amp;', '&')
  SET @rs = replace(@rs, '&apos;','''')
  SET @rs = replace(@rs, '&quot;','"')
  SET @rs = replace(@rs, '&gt;','>' )
  SET @rs = replace(@rs, '&lt;','<' )
  RETURN (@rs)
END