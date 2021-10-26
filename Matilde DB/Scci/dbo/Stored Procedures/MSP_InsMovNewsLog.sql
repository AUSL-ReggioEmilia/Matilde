CREATE PROCEDURE [dbo].[MSP_InsMovNewsLog](@xParametri XML)
AS
BEGIN
		
	
											
		DECLARE @sCodNews AS VARCHAR(20)
	DECLARE @sCodUtenteVisione AS VARCHAR(100)	
		
	
	    DECLARE @xTimeStampBase AS XML	
	DECLARE @xTimeStamp AS XML	
		
		
		SET @sCodNews=(SELECT TOP 1 ValoreParametro.CodNews.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodNews') as ValoreParametro(CodNews))
	
		SET @sCodUtenteVisione=(SELECT TOP 1 ValoreParametro.CodUtenteVisione.value('.','VARCHAR(100)')
					  FROM @xParametri.nodes('/Parametri/CodUtenteVisione') as ValoreParametro(CodUtenteVisione))	
	
	    SET @xTimeStamp=(SELECT TOP 1 ValoreParametro.TS.query('.')
					  FROM @xParametri.nodes('/Parametri/TimeStamp') as ValoreParametro(TS))
	
	
						
				
	INSERT INTO T_MovNewsLog(ID,CodNews,CodUtenteVisione,DataVisione,DataVisioneUTC)
	VALUES (NEWID(),@sCodNews,@sCodUtenteVisione,GETDATE(),GETUTCDATE())

				
	RETURN 0
END