CREATE PROCEDURE [dbo].[MSP_SelTrasferimento](@xParametri AS XML)
AS
BEGIN
	   	
	   	
				DECLARE @uIDTrasferimento AS UNIQUEIDENTIFIER	
	SET @uIDTrasferimento=(SELECT TOP 1 ValoreParametro.IDTrasferimento.value('.','UNIQUEIDENTIFIER')
					  FROM @xParametri.nodes('/Parametri/IDTrasferimento') as ValoreParametro(IDTrasferimento))	
				
				
	SELECT M.ID AS IDTrasferimento,
		   M.CodUA As CodUA,
		   M.CodUO As CodUO,
		   A.Descrizione AS Descrizione,
		   O.Descrizione AS DescrizioneUO,
           M.DataIngresso,
           M.DataUscita,
		   M.CodStatoTrasferimento, 
		   CR.CodStatoCartella, 
		   CR.NumeroCartella,
		   CR.ID AS IDCartella,
		   M.IDEpisodio AS IDEpisodio,
		   A.CodPadre AS CodUAPadre,
		   A.Descrizione AS DescrizioneUAPadre,
		   M.CodStanza,
		   M.DescrStanza,
		   M.CodLetto,
		   M.DescrLetto,
		   ISNULL(M.CodLetto,'') +  ' / ' + ISNULL(ISNULL(M.CodStanza,LT.CodStanza),'')   AS DescrLettoStanza,
		   M.CodAziTrasferimento
	FROM T_MovTrasferimenti	M
						LEFT JOIN T_MovCartelle CR
				ON M.IDCartella=CR.ID
						LEFT JOIN T_UnitaAtomiche	A
				ON M.CodUA=A.Codice
						LEFT JOIN T_MovEpisodi E
				ON M.IDEpisodio=E.ID
						LEFT JOIN T_UnitaOperative O
				ON 
				   E.CodAzi=O.CodAzi AND
				   M.CodUO=O.Codice
						LEFT JOIN T_UnitaAtomiche PA
				ON A.CodPadre=PA.Codice	   

						LEFT JOIN 	
				T_Letti LT
					ON 	(E.CodAzi=LT.CodAzi AND
						 M.CodSettore=LT.CodSettore AND
						 M.CodLetto=LT.CodLetto)
	WHERE 
		M.ID=@uIDTrasferimento
		   
		   
			
END