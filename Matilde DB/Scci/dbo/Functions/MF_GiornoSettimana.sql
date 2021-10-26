CREATE FUNCTION [dbo].[MF_GiornoSettimana](@d datetime,@sFormato CHAR(1)) RETURNS varchar(20) AS
BEGIN

	DECLARE @sd varchar(20)
	SET @sd = ''
	
	IF ISDATE(@d) <> 0 
	BEGIN
		IF @sFormato='E'
			SELECT
				@sd =
						CASE (DATEPART(dw, @d) - 1 + @@DATEFIRST) % 7 
							WHEN 0 THEN 'Domenica'		
							WHEN 1 THEN 'Lunedì'
							WHEN 2 THEN 'Martedì'
							WHEN 3 THEN 'Mercoledì'
							WHEN 4 THEN 'Giovedì'
							WHEN 5 THEN 'Venerdì'
							WHEN 6 THEN 'Sabato'
						END
		ELSE
			SELECT
				@sd =
						CASE (DATEPART(dw, @d) - 1 + @@DATEFIRST) % 7 
							WHEN 0 THEN 'Dom.'		
							WHEN 1 THEN 'Lun.'
							WHEN 2 THEN 'Mar.'
							WHEN 3 THEN 'Mer.'
							WHEN 4 THEN 'Gio.'
							WHEN 5 THEN 'Ven.'
							WHEN 6 THEN 'Sab.'
						END				
	END

	RETURN @sd
END