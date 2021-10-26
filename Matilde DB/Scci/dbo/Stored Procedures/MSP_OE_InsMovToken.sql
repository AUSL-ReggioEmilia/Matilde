CREATE PROCEDURE [dbo].[MSP_OE_InsMovToken] (@CodUtente VARCHAR(100), @IndirizzoIP VARCHAR(100), @DataScadenza DATETIME, @Token XML, @CodAzienda AS VARCHAR(20) ='')
AS
BEGIN

	INSERT INTO T_MovTokenOE (CodUtente, IndirizzoIP, DataScadenza, Token, CodAzienda)
    VALUES (@CodUtente, @IndirizzoIP, @DataScadenza, @Token,@CodAzienda)

END