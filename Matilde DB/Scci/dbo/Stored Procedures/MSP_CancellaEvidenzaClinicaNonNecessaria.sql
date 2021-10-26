CREATE PROCEDURE [dbo].[MSP_CancellaEvidenzaClinicaNonNecessaria]
	(@nRecord AS INTEGER, @nDeltaGG AS INTEGER)
AS
BEGIN
DECLARE @sSQL AS VARCHAR(MAX)
DECLARE @sSQLWhere AS VARCHAR(MAX)
DECLARE @sSQLOrderBy AS VARCHAR(MAX)
DECLARE @bContinua AS BIT
DECLARE @nRecordElaborati AS INTEGER


SET @bContinua=1
SET @nRecordElaborati=0


CREATE TABLE #tmpEvidenzaClinica
	(
		ID  UNIQUEIDENTIFIER
	)

CREATE INDEX IX_ID ON #tmpEvidenzaClinica (ID)   

WHILE (@bContinua=1)
	BEGIN
		SET @sSQL=''
		SET @sSQLWhere=''
		SET @sSQLOrderBy=''

	
				SET @sSQL = 'INSERT INTO #tmpEvidenzaClinica(ID) '
		SET @sSQL = @sSQL + 'SELECT '

		IF ISNULL(@nRecord,0)>0 
			SET @sSQL=@sSQL + ' TOP ' + CONVERT(VARCHAR(10),@nRecord) + ' '
			

		SET @sSQL = @sSQL + ' M.ID 
							FROM 
								T_MovEvidenzaClinica M WITH (NOLOCK) 
								    INNER JOIN T_MovEpisodi E WITH (NOLOCK) 
										ON (M.IDEpisodio=E.ID) ' 
		SET @sSQLWhere = @sSQLWhere + ' WHERE ' 
		SET @sSQLWhere = @sSQLWhere + '   IDEpisodio
											NOT IN (SELECT		
														IDEpisodio
													FROM 
														T_MovTrasferimenti WITH (NOLOCK)
													WHERE 
														IDCartella IS NOT NULL
													)
										
											AND E.DataDimissione IS NOT NULL 
											AND E.DataDimissione  <= DATEADD(day,' + CONVERT(VARCHAR(10),@nDeltaGG*-1) +',GETDATE()) 	
											AND ISNULL(CodStatoEvidenzaClinicaVisione,'''')<>''VS''		
										'
		
		SET @sSQLOrderBy=' ORDER BY E.DataDimissione ASC '

		SET @sSQL = @sSQL + @sSQLWhere + @sSQLOrderBy

				EXEC (@sSQL)	
				
		SET @nRecordElaborati= @@ROWCOUNT
		
		IF @nRecordElaborati > 0
			BEGIN
								SET @bContinua=1
				
								DELETE FROM T_MovEvidenzaClinica
				FROM 
					T_MovEvidenzaClinica
						 INNER JOIN  #tmpEvidenzaClinica
								ON T_MovEvidenzaClinica.ID= #tmpEvidenzaClinica.ID	
				
							END	
		ELSE
			SET @bContinua=0
		
				TRUNCATE TABLE #tmpEvidenzaClinica
		
	END
 
END