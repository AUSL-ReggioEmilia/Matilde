CREATE PROCEDURE [dbo].[MSP_SelUAUODaRuolo](@xParametri AS XML)

AS
BEGIN
	

						
	CREATE TABLE #tmpUARuolo
		(
			CodUA VARCHAR(20) COLLATE Latin1_General_CI_AS,
			Descrizione VARCHAR(255) COLLATE Latin1_General_CI_AS
		)

	INSERT #tmpUARuolo EXEC MSP_SelUADaRuolo @xParametri
		
	CREATE INDEX IX_CodUA ON #tmpUARuolo (CodUA)
	
	SELECT 
		T.CodUA,
		T.Descrizione AS DescrizioneUA,
		Q.CodUO,
		Q.DescrizioneUO,
		ISNULL(T.Descrizione,'') + ' / ' + ISNULL(Q.DescrizioneUO,'') AS Descrizione
	FROM 
		#tmpUARuolo T
			INNER JOIN 
				(SELECT 
					A.CodUA,
					O.Codice AS CodUO,
					MIN(O.Descrizione) AS DescrizioneUO					
				 FROM 
					T_AssUAUOLetti A
						INNER JOIN T_UnitaOperative O
							ON (A.CodAzi=O.CodAzi AND
								A.CodUO=O.Codice)
					GROUP BY A.CodUA,O.Codice
				) AS Q	
			ON (T.CodUA=Q.CodUA)
	ORDER BY T.Descrizione,	DescrizioneUO
			
		
		
END