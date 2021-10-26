CREATE PROCEDURE [dbo].[MSP_CancMovOrdiniEroganti](@xParametri XML)
AS
BEGIN
		
	
																			
		
	DECLARE @uIDOrdine AS UNIQUEIDENTIFIER			
	
	    DECLARE @xTimeStampBase AS XML	
	DECLARE @xTimeStamp AS XML	
		
		DECLARE @xLogPrima AS XML
	DECLARE @xLogDopo AS XML
	DECLARE @xTemp AS XML
	DECLARE @xParLog AS XML

		DECLARE @sGUID AS VARCHAR(50)
	DECLARE @uGUID AS UNIQUEIDENTIFIER
	
	DECLARE @sCodTmp AS VARCHAR(20)
	DECLARE @vIcona AS VARBINARY(MAX)
		
		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDOrdine.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDOrdine') as ValoreParametro(IDOrdine))
	IF 	ISNULL(@sGUID,'') <> '' 
			SET @uIDOrdine=CONVERT(UNIQUEIDENTIFIER,	@sGUID)			
					  
    SET @xTimeStamp=(SELECT TOP 1 ValoreParametro.TS.query('.')
					  FROM @xParametri.nodes('/Parametri/TimeStamp') as ValoreParametro(TS))
	
					
	SET @uGUID=NEWID()
						
		DELETE FROM T_MovOrdiniEroganti
	WHERE
	 IDOrdine=@uIDOrdine
	 
	SELECT 1 AS Esito,@@ROWCOUNT AS Qta
	 
		
	RETURN 0
END