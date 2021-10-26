CREATE PROCEDURE [dbo].[MSP_CopiaVersioneScheda](@xParametri XML)
AS
BEGIN
		
	
				
	DECLARE @sCodScheda AS VARCHAR(20)		
	DECLARE @nVersione AS INTEGER
	
	SET @sCodScheda=(SELECT TOP 1 ValoreParametro.CodScheda.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodScheda') as ValoreParametro(CodScheda))	

	SET @nVersione=(SELECT TOP 1 ValoreParametro.DataRif.value('.','INTEGER')
					  FROM @xParametri.nodes('/Parametri/Versione') as ValoreParametro(DataRif))	
	
	SET @nVersione=ISNULL(@nVersione,0)
	
				
	DECLARE @nNuovaVersione AS INTEGER
	DECLARE @dDataInziale As DATETIME
		
		SET @nNuovaVersione=(SELECT MAX(Versione) FROM T_SchedeVersioni
						 WHERE CodScheda=@sCodScheda) 
		
	SET @nNuovaVersione=ISNULL(@nNuovaVersione,0)+1
		
		SET @dDataInziale=(SELECT MAX(DtValF) FROM T_SchedeVersioni
						 WHERE CodScheda=@sCodScheda) 
	IF 	@dDataInziale IS NOT NULL 
		SET @dDataInziale=DATEADD(mi,1,@dDataInziale)	
	
	INSERT INTO T_SchedeVersioni(
			CodScheda,
			Versione,
			Descrizione,
			FlagAttiva,
			Pubblicato,
			DtValI,
			DtValF,
			CampiRilevanti,
			CampiObbligatori,
			Struttura,
			Layout)
	SELECT		
			CodScheda,
			@nNuovaVersione,
			LEFT('Copia di ' + ISNULL(Descrizione,''),255),
			FlagAttiva,
			Pubblicato,
			@dDataInziale,
			NULL AS DtValF ,
			CampiRilevanti,
			CampiObbligatori,
			Struttura,
			Layout
	FROM 	
		T_SchedeVersioni		
	WHERE 
		CodScheda=@sCodScheda AND
		Versione=@nVersione
		
		SELECT @nNuovaVersione
END