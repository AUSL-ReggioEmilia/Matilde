
CREATE FUNCTION [dbo].[MF_CalcolaEtaPediatrica](@dDataNascita AS DateTime)
RETURNS VARCHAR(50)
AS
BEGIN
	
	DECLARE @sOut AS VARCHAR(50)

	DECLARE @dtGiorno AS DATETIME
	SET @dtGiorno = Getdate() 

	DECLARE @nAnni AS int
	SET @nAnni = 0
	DECLARE @nMesi as int
	SET @nMesi = 0
	DECLARE @nGiorni as int
	SET @nGiorni = 0

	DECLARE @giornoOggi as int
	DECLARE @meseOggi as int
	DECLARE @annoOggi as int

	DECLARE @meseNascita as int
	DECLARE @annoNascita as int


	IF ( (@dDataNascita IS NOT NULL) AND (@dDataNascita <= @dtGiorno) )
	BEGIN

		

		SET @giornoOggi = DAY(@dtGiorno)
		SET @meseOggi = MONTH(@dtGiorno)
		SET @annoOggi = YEAR(@dtGiorno)

		SET @meseNascita = MONTH(@dDataNascita)
		SET @annoNascita = YEAR(@dDataNascita)

		DECLARE @incremento int
		SET @incremento = 0

		DECLARE @nGGMeseNascita int
		SET @nGGMeseNascita = 0

		IF ( DAY(@dDataNascita) > DAY(getdate()) )			BEGIN
				

			IF @meseNascita = 12 SET @nGGMeseNascita = 31
			ELSE
			BEGIN
				SET @nGGMeseNascita = DATEDIFF(day, CONVERT(datetime, '01/' + RIGHT('00' + convert(varchar, @meseNascita), 2) + '/' + convert(varchar, @annoNascita), 103), 
							CONVERT(datetime, '01/' + RIGHT('00' + CONVERT(varchar, @meseNascita + 1), 2) + '/' + convert(varchar, @annoNascita), 103))

			END

		END

		
		IF (@nGGMeseNascita <> 0)
		BEGIN
			SET @nGiorni = @giornoOggi + @nGGMeseNascita - DAY(@dDataNascita);
			SET @incremento = 1;
		END
		ELSE
		BEGIN
			SET @nGiorni = @giornoOggi - DAY(@dDataNascita);
		END
				
		IF ((MONTH(@dDataNascita) + @incremento) > @meseOggi)
		BEGIN
			SET @nMesi = (@meseOggi + 12) - (MONTH(@dDataNascita) + @incremento);
			SET @incremento = 1;
		END
		ELSE
		BEGIN
			SET @nMesi = (@meseOggi ) - (MONTH(@dDataNascita) + @incremento);
			SET @incremento = 0;
		END
			
					
		SET @nAnni = @annoOggi - (YEAR(@dDataNascita) + @incremento);


		SET @sOut = ''

				SET @sOut = CONVERT(varchar, @nAnni) + 'aa'

		IF (@nAnni < 14)
		BEGIN
			IF @sOut <> '' SET @sOut = @sOut + ' ' 
			SET @sOut = @sOut + convert(varchar, @nMesi) + 'm'
		END
			
		IF (@nAnni < 14)
		BEGIN
			IF @sOut <> '' SET @sOut = @sOut + ' ' 		
			SET @sOut = @sOut + convert(varchar, @nGiorni) + 'g'
		END			
	END
	ELSE
	BEGIN
		SET @sOut ='' 
	END

	RETURN @sOut
	
END