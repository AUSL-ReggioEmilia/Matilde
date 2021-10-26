CREATE VIEW [dbo].[Q_SelUltimoDocumentoSchedaFirmata]
AS

	SELECT			
		T.ID AS IDDocumentoFirmato,
		T.IDNum AS IDNumDocumentoFirmato,
		T.IDEntita AS IDScheda,		
		T.DataInserimento AS UltimaDataFirma,
		T.CodStatoEntita AS UltimoCodStatoEntita
			
	FROM T_MovDocumentiFirmati T
		INNER JOIN 
			(SELECT IDEntita,MAX(IDNum) AS IDNumUltimaDocumentoFirmato
			FROM T_MovDocumentiFirmati
			WHERE 
				CodEntita='SCH'				
			GROUP BY IDEntita) AS Q
		ON 
			T.IDNum=Q.IDNumUltimaDocumentoFirmato