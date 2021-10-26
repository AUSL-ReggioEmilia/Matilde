CREATE PROCEDURE [dbo].[MSP_SelExportDWHSchede](@DataInizio AS DATETIME, @DataFine AS DATETIME)
AS
BEGIN
		

	DECLARE @sSQL AS VARCHAR(5000)



	SET @sSQL = 'SELECT 
						MS.ID, 
						S.SistemaDWH, 
						ISNULL(S.CodReportDWH,'''') AS CodReportDWH 
				 FROM 
						T_MovSchede MS WITH (NOLOCK)
							INNER JOIN T_Schede S 
								ON MS.CodScheda = S.Codice
                 WHERE 
					MS.Storicizzata = 0 AND 
						(	
							(	
								ISNULL(S.EsportaDWH,0) = 1 AND
								MS.CodEntita = ''PAZ''
							 ) 							 
							 OR
								ISNULL(S.EsportaDWHSingola,0) = 1
						) 
                        AND  
						(	
							 (
								ISNULL(MS.Validabile,0) = 0
							 ) 
							 OR  
							(
																ISNULL(MS.Validabile,0) = 1 AND 
								ISNULL(MS.Validata,0) = 1 AND
								ISNULL(MS.Revisione,0) = 0
							)
						) 
                 '

		IF (@DataInizio IS NOT NULL)		
		BEGIN
			SET @sSQL = @sSQL +
                      ' AND ISNULL(MS.DataUltimaModifica, MS.DataCreazione) >= CONVERT(DATETIME,''' + CONVERT(VARCHAR,@DataInizio,120) + ''',120) '
        END
    
		IF (@DataFine IS NOT NULL)		
		BEGIN
			SET @sSQL = @sSQL +
                      ' AND ISNULL(MS.DataUltimaModifica, MS.DataCreazione) <= CONVERT(DATETIME,''' + CONVERT(VARCHAR,@DataFine,120) + ''',120) '
        END
                            
	SET @sSQL = @sSQL + ' ORDER BY MS.IDNum '
				
	PRINT @sSQL

	EXEC (@sSQL)
END