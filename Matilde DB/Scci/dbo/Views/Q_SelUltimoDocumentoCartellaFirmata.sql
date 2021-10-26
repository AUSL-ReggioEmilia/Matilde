CREATE VIEW [dbo].[Q_SelUltimoDocumentoCartellaFirmata]
AS

	SELECT	
		T.IDEntita AS IDCartella,
		Q.IDNumUltimaDocumentoCartellaFirmata,
		T.DataInserimento AS UltimaDataFirma,
		T.CodStatoEntita AS UltimoCodStatoEntita
			
	FROM T_MovDocumentiFirmati T
		INNER JOIN 
			(SELECT IDEntita,MAX(IDNum) AS IDNumUltimaDocumentoCartellaFirmata
			FROM T_MovDocumentiFirmati
			WHERE 
				CodEntita='CAR'				
			GROUP BY IDEntita) AS Q
		ON 
			T.IDNum=Q.IDNumUltimaDocumentoCartellaFirmata