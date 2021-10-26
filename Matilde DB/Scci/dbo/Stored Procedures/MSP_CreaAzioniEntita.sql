CREATE PROCEDURE [dbo].[MSP_CreaAzioniEntita] (@sCodiceSicurezza VARCHAR(20))
AS
BEGIN 
	
	
				
	IF @sCodiceSicurezza='123456' 
	BEGIN
		INSERT INTO T_AzioniEntita(CodAzione,CodEntita)
		SELECT Q.*
		FROM 
			(SELECT A.Codice AS CodAzione,
				 E.Codice As CodEntita 
				FROM T_Azioni A
					CROSS JOIN T_Entita E 
				WHERE ISNULL(SistemaEsterno,0)=0
				) AS Q
			LEFT JOIN T_AzioniEntita AE
				ON 	(Q.CodAzione=AE.CodAzione AND
					 Q.CodEntita=AE.CodEntita)	
			WHERE AE.CodAzione IS NULL
	END
END