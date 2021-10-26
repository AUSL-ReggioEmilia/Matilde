CREATE PROCEDURE [dbo].[MSP_BO_AggiornaVersioneScheda](@xParametri XML)
AS
BEGIN
	
			
		DECLARE @uIDScheda AS UNIQUEIDENTIFIER	
	DECLARE @nVersione INTEGER
	DECLARE @xDati XML			

	DECLARE @sGUID AS VARCHAR(50)
	DECLARE @sTmp AS VARCHAR(50)

		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDScheda.value('.','VARCHAR(50)')
					FROM @xParametri.nodes('/Parametri/IDScheda') as ValoreParametro(IDScheda))
	IF ISNULL(@sGUID,'') <> '' SET @uIDScheda=CONVERT(UNIQUEIDENTIFIER, @sGUID)			

		SET @sTmp=(SELECT TOP 1 ValoreParametro.Versione.value('.','INTEGER')
					FROM @xParametri.nodes('/Parametri/Versione') as ValoreParametro(Versione))						  
	IF ISNUMERIC(@sTMP) <> '' SET @nVersione=CONVERT(INTEGER, @sTmp)

		SET @xDati=(SELECT TOP 1 ValoreParametro.Dati.query('./*')
					FROM @xParametri.nodes('/Parametri/Dati') as ValoreParametro(Dati))				

		IF @uIDScheda IS NOT NULL AND @nVersione IS NOT NULL AND @xDati IS NOT NULL
	BEGIN
		UPDATE T_MovSchede
		SET Versione=@nVersione,
			Dati= @xDati
		WHERE ID=@uIDScheda
	END	

	RETURN 0
							 	
END