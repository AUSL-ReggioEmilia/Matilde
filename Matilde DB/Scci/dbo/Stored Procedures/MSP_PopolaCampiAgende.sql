CREATE PROCEDURE MSP_PopolaCampiAgende(@sCodEntita AS VARCHAR(20))
AS
BEGIN
		
	
				

	IF @sCodEntita='EPI'
	BEGIN		
		DELETE FROM 
			T_CampiAgende
		WHERE CodEntita=@sCodEntita
	
		INSERT INTO T_CampiAgende(Codice,CodEntita,Descrizione)
		SELECT COLUMN_NAME,@sCodEntita,COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS
		WHERE 		
			TABLE_NAME='Q_SelCampiAgendeEPI'
	END
	ELSE
				BEGIN
			DELETE FROM 
				T_CampiAgende
			WHERE CodEntita='PAZ'
			
			INSERT INTO T_CampiAgende(Codice,CodEntita,Descrizione)
			SELECT COLUMN_NAME,'PAZ',COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS
			WHERE 		
				TABLE_NAME='Q_SelCampiAgendePAZ'
		END		
END