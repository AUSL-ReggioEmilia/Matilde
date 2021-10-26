CREATE PROCEDURE [dbo].[MSP_AggConfigUtente](@xParametri XML)
AS
BEGIN
		
	
				DECLARE @sCodice AS VARCHAR(100)
	DECLARE @xValore AS XML
	DECLARE @txtValore AS VARCHAR(MAX)
	
		DECLARE @nTmp INTEGER
	
		SET @sCodice=(SELECT TOP 1 ValoreParametro.Codice.value('.','VARCHAR(100)')
					  FROM @xParametri.nodes('/Parametri/Codice') as ValoreParametro(Codice))	
	SET @sCodice=ISNULL(@sCodice,'')
  	
  	
  		SET @txtValore= CONVERT(VARCHAR(MAX), (SELECT TOP 1 ValoreParametro.Valore.value('.','VARCHAR(MAX)')
								FROM @xParametri.nodes('/Parametri/Valore') as ValoreParametro(Valore)))
	
	SET @xValore=CONVERT(XML, CONVERT(NVARCHAR(MAX),@txtValore))
				  
	
				
	SET @nTmp=(SELECT COUNT(*) FROM 
					T_ConfigUtente					
					WHERE Codice=@sCodice)

	IF 	@nTmp>0 	
				UPDATE T_ConfigUtente
		SET Valore=@xValore
		WHERE Codice=@sCodice	 
	ELSE
				INSERT INTO T_ConfigUtente(Codice,Valore)
		VALUES(@sCodice,@xValore)
			
RETURN 0	
					
	
END