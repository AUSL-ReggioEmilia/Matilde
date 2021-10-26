CREATE PROCEDURE [dbo].[MSP_SelCodSchedaPosologia](@xParametri AS XML )
AS
BEGIN
				 	
	DECLARE @uIDPrescrizione AS Varchar(50)
	
	SET @uIDPrescrizione=(SELECT TOP 1 ValoreParametro.IDPrescrizione.value('.','UNIQUEIDENTIFIER')
					  FROM @xParametri.nodes('/Parametri/IDPrescrizione') as ValoreParametro(IDPrescrizione))
				
	SELECT 
		TP.CodSchedaPosologia, 
		MT.CodUA,
		MP.IDPaziente,
		PR.IDEpisodio,
		PR.IDTrasferimento
		
	FROM 
		T_MovPrescrizioni PR 
			INNER JOIN T_TipoPrescrizione TP
				ON PR.CodTipoPrescrizione=TP.Codice			
			LEFT JOIN T_MovPazienti MP
				ON PR.IDEpisodio=MP.IDEpisodio
			LEFT JOIN T_MovTrasferimenti MT
				ON PR.IDTrasferimento = MT.ID
	WHERE PR.ID=@uIDPrescrizione
		

	RETURN 0
END