CREATE FUNCTION [dbo].[MF_UOInteraziendale]
	(	
		@sCodAzi AS VARCHAR(20),
		@sCodUO AS VARCHAR(20)
	)
RETURNS BIT
AS
BEGIN

	DECLARE @bRet AS BIT
	DECLARE @nQta AS INTEGER
			
	SET @nQta=( SELECT COUNT(*) FROM T_UnitaOperative 
				WHERE 
					Codice=@sCodUO AND 
					Interaziendale=1					
									   )

	IF @nQta > 0
		SET @bRet=1
	ELSE
		SET @bRet=0

	RETURN @bRet
END