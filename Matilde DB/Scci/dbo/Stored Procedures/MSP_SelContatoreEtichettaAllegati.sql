CREATE PROCEDURE [dbo].[MSP_SelContatoreEtichettaAllegati](@xParametri XML)
AS
BEGIN
		
	
			

				
	DECLARE @sUltimoValore VARCHAR(50)
	DECLARE @nUltimoValore VARCHAR(50)
	DECLARE @nIDConfig VARCHAR(50)
	
	SET @nIDConfig=37
	
	SET @sUltimoValore=(SELECT TOP 1 ISNULL(Valore,'') FROM T_Config WHERE ID=@nIDConfig)
	
	SET @sUltimoValore=ISNULL(@sUltimoValore,'')
	
	IF @sUltimoValore='' SET @sUltimoValore='1'
	
	SET @nUltimoValore=CONVERT(INTEGER,@sUltimoValore)
	
	UPDATE T_Config	
	SET  Valore=@nUltimoValore +1
	WHERE ID=@nIDConfig		  
	
	SELECT @nUltimoValore AS NumeroEtichetta
	
END