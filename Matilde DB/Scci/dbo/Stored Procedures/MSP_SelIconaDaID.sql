CREATE PROCEDURE [dbo].[MSP_SelIconaDaID](@xParametri XML)
AS
BEGIN
	
		
	
				 	
	DECLARE @nIDIcona INTEGER
	DECLARE @nFormato INTEGER
	
	
	SET @nIDIcona=(SELECT TOP 1 ValoreParametro.IDIcona.value('.','INTEGER')
					  FROM @xParametri.nodes('/Parametri/IDIcona') as ValoreParametro(IDIcona))	
	
	SET @nFormato=(SELECT TOP 1 ValoreParametro.Formato.value('.','INTEGER')
					  FROM @xParametri.nodes('/Parametri/Formato') as ValoreParametro(Formato))	
	
										
	
				SELECT	
		IDNum AS IDIcona,
		CASE 
			WHEN @nFormato=16 THEN Icona16 
			WHEN @nFormato=32 THEN Icona32
			WHEN @nFormato=48 THEN Icona48
			WHEN @nFormato=256 THEN Icona256
			ELSE NULL
		END AS Icona	
	FROM 
		T_Icone
	WHERE
		IDNum=@nIDIcona
				
END