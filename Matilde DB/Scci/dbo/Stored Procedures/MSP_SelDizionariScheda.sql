CREATE PROCEDURE [dbo].[MSP_SelDizionariScheda](@xParametri XML)
AS
BEGIN
		
	
				
	DECLARE @sCodScheda AS VARCHAR(20)	
	
	DECLARE @nVersione AS INTEGER
	
	SET @sCodScheda=(SELECT TOP 1 ValoreParametro.CodScheda.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodScheda') as ValoreParametro(CodScheda))	

	SET @nVersione=(SELECT TOP 1 ValoreParametro.DataRif.value('.','INTEGER')
					  FROM @xParametri.nodes('/Parametri/Versione') as ValoreParametro(DataRif))	
	
	SET @nVersione=ISNULL(@nVersione,0)
	
			
							
							
		
	SELECT DISTINCT Codice FROM 
		(SELECT  ValoreParametro.Decodifica.value('.','VARCHAR(20)') AS Codice
		FROM T_SchedeVersioni 
			CROSS APPLY
								Struttura.nodes('/DcScheda/Sezioni/Item/Value/DcSezione/Voci/Item/Value/DcVoce/Decodifica') as ValoreParametro(Decodifica)
		WHERE CodScheda=@sCodScheda AND 
			  Versione= CASE 
							WHEN @nVersione=0 THEN Versione
							ELSE @nVersione
						END	
		) AS Q
	WHERE ISNULL(Codice,'') NOT IN ('0','')		

	
	RETURN 0
END