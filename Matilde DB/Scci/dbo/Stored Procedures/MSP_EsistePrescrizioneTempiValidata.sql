CREATE PROCEDURE [dbo].[MSP_EsistePrescrizioneTempiValidata](@xParametri XML)
AS
BEGIN
		
	
	
		DECLARE @uIDPrescrizione AS UNIQUEIDENTIFIER
	
		DECLARE @sGUID AS VARCHAR(50)	
	DECLARE @nQtaValidati AS INTEGER
	DECLARE @nRet AS INTEGER
	
	DECLARE @sTmp As VARCHAR(1800)
	DECLARE @sRisultato AS VARCHAR(MAX)
	
	SET @sRisultato=''
		
	SET @nRet=0
	
				
		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDPrescrizione.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDPrescrizione') as ValoreParametro(IDPrescrizione))
	IF 	ISNULL(@sGUID,'') <> '' 
			SET @uIDPrescrizione=CONVERT(UNIQUEIDENTIFIER,	@sGUID)	

					
	IF @uIDPrescrizione IS NOT NULL
		BEGIN										
									
			

			SELECT @nQtaValidati = COUNT(*)
				FROM T_MovPrescrizioniTempi
				WHERE CodStatoPrescrizioneTempi IN ('VA','SS') AND
					IDPrescrizione = @uIDPrescrizione

			IF @nQtaValidati = 0
				SET @nRet=0
			ELSE
				SET @nRet=1													 
		END	
	ELSE
		BEGIN
						PRINT 'Parametri errati'			
		END	
	
	SELECT 	@nRet AS Esiste, 
			@uIDPrescrizione AS IDPrescrizione					
	RETURN 0
END