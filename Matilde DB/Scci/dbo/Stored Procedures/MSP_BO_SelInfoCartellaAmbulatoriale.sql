CREATE PROCEDURE [dbo].[MSP_BO_SelInfoCartellaAmbulatoriale] (@CodScheda VarChar(20), @NumCartella VarChar(50))
AS
BEGIN

	
DECLARE @uIDCartella AS UNIQUEIDENTIFIER
DECLARE @uIDScheda AS UNIQUEIDENTIFIER
DECLARE @uIDPaziente AS UNIQUEIDENTIFIER
DECLARE @nQTA AS INTEGER
DECLARE @sInfo AS VARCHAR(MAX)
DECLARE @bErrore AS BIT
DECLARE @sTmp AS VARCHAR(MAX)
DECLARE @sNumeroNosologico AS VARCHAR(50)
DECLARE @sNumeroListaAttesa AS VARCHAR(50)
		

	SELECT @nQta=COUNT(*)
	FROM T_MovCartelleAmbulatoriali
	WHERE 
		NumeroCartella=@NumCartella AND 
		ID IN (SELECT IDCartellaAmbulatoriale
		       FROM T_MovSchede
			   WHERE
				CodScheda= @CodScheda AND
				CodStatoScheda NOT IN ('CA') AND
				Storicizzata=0
			   )
					

	SET @nQta=ISNULL(@nQta,0)
	
	IF @nQta =0 
		BEGIN
			SET @sInfo='ERRORE : nessuna cartella trovata'
			SET @bErrore=1
		END
	ELSE
		IF @nQta>1 
			BEGIN
				SET @sInfo='ERRORE : ho trovato più di una cartella, n°: ' +CONVERT(VARCHAR(255),@nQta)
				SET @bErrore=1				
			END
		ELSE
			BEGIN

				SELECT TOP 1
					  @uIDScheda = M.ID,
					  @uIDPaziente = M.IDPaziente,
					  @uIDCartella=M.IDCartellaAmbulatoriale
				FROM T_MovSchede M
				WHERE IDCartellaAmbulatoriale
					IN (SELECT ID FROM T_MovCartelleAmbulatoriali
						WHERE 
						NumeroCartella=@NumCartella
						)
					AND M.CodScheda=@CodScheda

								SET @sInfo='IDCartella: ' + CONVERT(VARCHAR(255),@uIDCartella)				
				
								SET @sTmp=(SELECT TOP 1 
										ISNULL(Cognome,'') + ' ' + 
										ISNULL(Nome,'')  +
										' (' + CONVERT(VARCHAR(10), DataNascita,120) + ')'
										AS Paziente
								FROM T_Pazienti WITH (NOLOCK)
								WHERE ID=@uIDPaziente)
								
				SET @sInfo=@sInfo + CHAR(13) + CHAR(10) + @sTmp											
				
				SET @bErrore=0
			END	
			
	
		SELECT	
		ISNULL(@bErrore,0) AS Errore,
		@sInfo AS Info

END