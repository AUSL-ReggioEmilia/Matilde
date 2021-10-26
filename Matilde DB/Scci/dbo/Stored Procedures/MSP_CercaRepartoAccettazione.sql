CREATE PROCEDURE [dbo].[MSP_CercaRepartoAccettazione](@xParametri XML)
AS
BEGIN
		
	
	
		DECLARE @uIDEpisodio AS UNIQUEIDENTIFIER
	
		DECLARE @sGUID AS VARCHAR(50)	
	DECLARE @nQtaValidati AS INTEGER
	DECLARE @nRet AS INTEGER
	
	DECLARE @sTmp As VARCHAR(1800)
	DECLARE @sRisultato AS VARCHAR(MAX)
	
	SET @sRisultato=''
		
	SET @nRet=0
	
				
		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDEpisodio.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDEpisodio') as ValoreParametro(IDEpisodio))
	IF 	ISNULL(@sGUID,'') <> '' 
			SET @uIDEpisodio=CONVERT(UNIQUEIDENTIFIER,	@sGUID)	

					
	IF @uIDEpisodio IS NOT NULL
		BEGIN										
									
			SELECT TOP 1 
				   M.ID AS IDTraferimento,
				   CodUA
				FROM 
					T_MovTrasferimenti M	
						INNER JOIN T_MovEpisodi E
							ON M.IDEpisodio=E.ID
				WHERE 
					M.IDEpisodio=@uIDEpisodio AND
					M.DataIngresso=E.DataRicovero AND
					M.CodStatoTrasferimento IN ('AT','TR','DM','SS')					
													 
		END	
	ELSE
		BEGIN
						PRINT 'Parametri errati'			
		END	
				
	RETURN 0
END