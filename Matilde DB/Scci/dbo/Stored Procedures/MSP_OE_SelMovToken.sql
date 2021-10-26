CREATE PROCEDURE [dbo].[MSP_OE_SelMovToken] (@CodUtente VARCHAR(100), @IndirizzoIP VARCHAR(100), @CodAzienda VARCHAR(20)=NULL)
AS
BEGIN

		DELETE FROM T_MovTokenOE WHERE DataScadenza < GETDATE()

		SELECT * FROM T_MovTokenOE 
	WHERE 
		CodUtente = @CodUtente AND 
		IndirizzoIP = @IndirizzoIP AND
		CodAzienda = ISNULL(@CodAzienda,CodAzienda)

END