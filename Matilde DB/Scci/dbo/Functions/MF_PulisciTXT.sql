CREATE FUNCTION [dbo].[MF_PulisciTXT](@sIn AS VARCHAR(MAX))
RETURNS VARCHAR(MAX)
AS
BEGIN

	DECLARE @sOUT AS VARCHAR(MAX)	
	
	SET @sOUT=NULL
	
	IF @sIn IS NOT NULL
	BEGIN
		IF ISNULL(@sIn,'') <> ''
		BEGIN
			SET @sOUT=@sIn
						
			SET @sOUT=
				REPLACE(@sOUT,'','')	
			
			SET @sOUT=
				REPLACE(@sOUT,'','')	
				
			SET @sOUT=
				REPLACE(@sOUT,'','')	
								
			SET @sOUT=
				REPLACE(@sOUT,'','')
			
			SET @sOUT=
				REPLACE(@sOUT,'·','')
				
			SET @sOUT=
				REPLACE(@sOUT,'','')
				
			SET @sOUT=
				REPLACE(@sOUT,'','')
								
			SET @sOUT=
				REPLACE(@sOUT,'','')	

			SET @sOUT=
				REPLACE(@sOUT,'','')	
				

		END			
		ELSE
			SET @sOUT=@sIN		
	END	
				
	RETURN @sOUT
	
END