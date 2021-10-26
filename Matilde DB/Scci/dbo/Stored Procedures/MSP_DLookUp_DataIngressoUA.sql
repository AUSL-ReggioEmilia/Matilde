CREATE PROCEDURE [dbo].[MSP_DLookUp_DataIngressoUA](@uIDTrasferimento AS UNIQUEIDENTIFIER)
AS
BEGIN


DECLARE @sCodUA AS VARCHAR(20)
DECLARE @uIDEpisodio AS UNIQUEIDENTIFIER
DECLARE @dtDataIngressoUA AS DATETIME
	
	SET @dtDataIngressoUA = NULL
	IF @uIDTrasferimento IS NOT NULL
	BEGIN
		SELECT TOP 1 
			@uIDEpisodio=IDEpisodio,
			@sCodUA = CodUA
		FROM T_MovTrasferimenti 
   	    WHERE ID=@uIDTrasferimento

				SET @dtDataIngressoUA =(SELECT MIN(DataIngresso)
								FROM T_MovTrasferimenti 
								WHERE 
									IDEpisodio= @uIDEpisodio AND
									CodUA = @sCodUA AND
									CodStatoTrasferimento 
										NOT IN ('PC','PA','PR','PT','SS','CA','AN')
								)
	END

	SELECT @dtDataIngressoUA AS DataIngressoUA
END