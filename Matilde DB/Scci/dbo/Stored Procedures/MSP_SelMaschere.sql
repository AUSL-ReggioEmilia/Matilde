CREATE PROCEDURE [dbo].[MSP_SelMaschere](@xParametri XML)
AS
BEGIN
	
	
	
	
				 	
	
	DECLARE @sCodMaschera AS VARCHAR(20)
	
	SET @sCodMaschera=(SELECT TOP 1 ValoreParametro.CodMaschera.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodMaschera') as ValoreParametro(CodMaschera))	
										
				SELECT	
		M.Codice, 
		M.Descrizione,
		IsNull(M.Modale,0) As Modale,
		IsNull(M.InCache,1) As InCache,
		IsNull(M.Aggiorna,1) As Aggiorna,
		IsNull(M.Massimizzata,0) As Massimizzata,
		IsNull(M.TimerRefresh,0) As TimerRefresh,
		IsNull(M.SegnalibroAdd,0) As SegnalibroAdd,
		IsNull(M.SegnalibroVisualizza,0) As SegnalibroVisualizza,
		ISNULL(M.CambioPercorso,0) AS CambioPercorso,
		ISNULL(M.InCacheDaPercorso,0) AS InCacheDaPercorso
	FROM 
		T_Maschere M
	WHERE 
		M.Codice=@sCodMaschera
				  
		
END