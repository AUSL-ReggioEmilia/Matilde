CREATE PROCEDURE [dbo].[MSP_SelEpisodio](@xParametri XML)
AS
BEGIN
		
	
				DECLARE @uIDEpisodio AS UNIQUEIDENTIFIER
	DECLARE @dMinDataIngresso AS DATETIME
	DECLARE @uIDPrimoTrasferimento AS UNIQUEIDENTIFIER
	
	SET @uIDEpisodio=(SELECT TOP 1 ValoreParametro.IDEpisodio.value('.','UNIQUEIDENTIFIER')
					  FROM @xParametri.nodes('/Parametri/IDEpisodio') as ValoreParametro(IDEpisodio))	
				
				
	
	SET @dMinDataIngresso=(SELECT MIN(DataIngresso)
						   FROM T_MovTrasferimenti 
						   WHERE 
								CodStatoTrasferimento IN ('AT','DM','TR') AND
								IDEpisodio=@uIDEpisodio
						   )
						  
	SET @uIDPrimoTrasferimento=(SELECT TOP 1 ID
								FROM T_MovTrasferimenti
								WHERE 
									DataIngresso=@dMinDataIngresso AND 
									CodStatoTrasferimento IN ('AT','DM','TR') AND
									IDEpisodio=@uIDEpisodio
								)	
					  
	SELECT M.ID AS IDEpisodio
		  ,M.IDNum
		  ,M.CodAzi
		  ,M.CodTipoEpisodio
		  ,M.DataRicovero
		  ,M.DataDimissione
		  ,M.NumeroNosologico
		  ,M.NumeroListaAttesa
		  ,T.Descrizione AS DescrizioneTipoEpisodio
		  ,TR.CodUA AS CodUAAccesso
		  ,A.Descrizione As DescrizioneUAAccesso
		  ,TR.CodUO AS CodUOAccesso
		  ,O.Descrizione AS DescrizioneUOAccesso
	  FROM T_MovEpisodi M
		LEFT JOIN T_TipoEpisodio T
			ON M.CodTipoEpisodio=T.Codice
		LEFT JOIN T_MovTrasferimenti TR
			ON (M.ID=TR.IDEpisodio AND
				TR.ID=@uIDPrimoTrasferimento)
		LEFT JOIN T_UnitaAtomiche A
			ON (TR.CodUA=A.Codice)
		LEFT JOIN T_UnitaOperative O
			ON (TR.CodUO=O.Codice AND
				M.CodAzi=O.CodAzi)	
	WHERE M.ID = @uIDEpisodio

	
	RETURN 0
END