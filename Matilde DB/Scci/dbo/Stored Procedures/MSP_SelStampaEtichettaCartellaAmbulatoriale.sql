CREATE PROCEDURE [dbo].[MSP_SelStampaEtichettaCartellaAmbulatoriale](@xParametri XML)
AS
BEGIN
	
	
				
		
		DECLARE @uIDScheda AS UNIQUEIDENTIFIER

		DECLARE @sGUID AS VARCHAR(50)
	
		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDScheda.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDScheda') as ValoreParametro(IDScheda))
	IF 	ISNULL(@sGUID,'') <> '' 
			SET @uIDScheda=CONVERT(UNIQUEIDENTIFIER,@sGUID)			

			
	SELECT 
			RTRIM(LTRIM(ISNULL(P.Cognome,'') + ' ' + ISNULL(Nome,''))) AS NomePaziente, 
			P.Cognome,
			P.Nome,			
			P.Sesso,
			P.DataNascita,
			CAC.NumeroCartella AS NumeroCartella,
			CASE 
				WHEN ISNULL(S.DescrizioneAlternativa,'') <> '' THEN S.DescrizioneAlternativa
				ELSE S.Descrizione
			END AS DescrizioneCartella
	from 
		T_MovSchede M
			LEFT JOIN T_Schede S ON
				M.CodScheda=S.Codice
			LEFT JOIN T_Pazienti P
				ON M.IDPaziente=P.ID
			LEFT JOIN T_MovCartelleAmbulatoriali CAC 
				ON M.IDCartellaAmbulatoriale =CAC.ID			
	WHERE
		M.ID = @uIDScheda
			
						
END