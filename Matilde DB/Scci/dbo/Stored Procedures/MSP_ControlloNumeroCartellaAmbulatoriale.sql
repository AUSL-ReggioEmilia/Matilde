CREATE PROCEDURE [dbo].[MSP_ControlloNumeroCartellaAmbulatoriale](@xParametri XML)
AS
BEGIN
		
		DECLARE @sCodScheda AS VARCHAR(20)	
	DECLARE @sNumeroCartella AS VARCHAR(50)

		DECLARE @sCodContatore AS VARCHAR(20)
	DECLARE @nQTA AS INTEGER
				
	DECLARE @bEsito AS BIT	
	DECLARE @sMessaggio AS VARCHAR(2000)
	DECLARE @sTmp AS VARCHAR(MAX)
				
		SET @sCodScheda=(SELECT TOP 1 ValoreParametro.CodScheda.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodScheda') as ValoreParametro(CodScheda))	


		SET @sNumeroCartella=(SELECT TOP 1 ValoreParametro.NumeroCartella.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/NumeroCartella') as ValoreParametro(NumeroCartella))	
		  
					  
				
	CREATE TABLE #tmpSchedeContatori
		(CodScheda VARCHAR(20) COLLATE Latin1_General_CI_AS)
	
		SET @sCodContatore=(SELECT CodContatore FROM T_Schede WITH (NOLOCK) WHERE Codice=@sCodScheda)

		INSERT INTO  #tmpSchedeContatori(CodScheda)
	SELECT Codice
	FROM T_Schede
	WHERE 
		CodContatore=@sCodContatore AND
		ISNULL(CartellaAmbulatorialeCodificata,0)=1

	
	SET @nQTA=0
	SET @sTmp=''
	
	
	SET @nQTA=(SELECT COUNT(ID) 
				FROM T_MovCartelleAmbulatoriali CAC WITH (NOLOCK)
					WHERE CAC.ID IN (SELECT IDCartellaAmbulatoriale
								 FROM T_MovSchede WITH (NOLOCK)
								 WHERE
									CodScheda IN (SELECT CodScheda FROM #tmpSchedeContatori) AND
									Storicizzata=0 AND
									CodStatoScheda <> 'CA'
								 )
					AND CAC.NumeroCartella=@sNumeroCartella
				)
				
	IF @nQTA=0
		BEGIN
			SET @sMessaggio=''
			SET @bEsito=0
		END	
	ELSE
		BEGIN
			SET @sMessaggio='Esiste già una cartella aperta' + CHAR(13) + CHAR(10) + + CHAR(13) + CHAR(10)			
			SET @sMessaggio=@sMessaggio + 'Numero Cartelle aperte: ' + LTRIM(@sTmp)  + CHAR(13) + CHAR(10)  + CHAR(13) + CHAR(10)						
			SET @bEsito=1
		END	
	
	
	SELECT 
		@bEsito As Esito,
		@sMessaggio As Messaggio 
END