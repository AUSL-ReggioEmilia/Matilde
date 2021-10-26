CREATE VIEW Q_SelSchedeUltimaVersione
AS
SELECT  S.Codice AS CodScheda,
		ISNULL(V.Versione, 0) AS Versione			
FROM 
	T_Schede S 
		INNER JOIN T_SchedeVersioni V ON
	S.Codice = V.CodScheda
WHERE										
	ISNULL(V.FlagAttiva, 0) = 1	AND ISNULL(V.Pubblicato,0)=1 					AND 
		V.DtValI <= GETDATE() AND													ISNULL(V.DtValF,convert(datetime,'01-01-2100')) >=  GETDATE()