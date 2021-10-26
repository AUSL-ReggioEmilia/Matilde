

CREATE VIEW [dbo].[Q_SelCampiAgendeEPI]
AS
	SELECT 
		
		P.IDPaziente,
		P.IDEpisodio,
				ISNULL(CodSAC,'') AS CodSAC,
		ISNULL(Cognome,'') AS Cognome,
		ISNULL(Nome,'') AS Nome,
		ISNULL(Sesso,'') AS Sesso,
		CASE 
			WHEN DataNascita IS NULL THEN ''
			ELSE 
				CONVERT(VARCHAR(10),DataNascita,105) 
		END AS DataNascita,
		ISNULL(CodiceFiscale,'') AS CodiceFiscale ,
		ISNULL(ComuneNascita,'') AS ComuneNascita		 
	 FROM
	   T_MovPazienti  P WITH (NOLOCK)
						