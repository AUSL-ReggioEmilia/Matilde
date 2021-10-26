CREATE PROCEDURE [dbo].[MSP_BO_SelInfoCartella] (@CodUA VarChar(20), @NumCartella VarChar(50))
AS
BEGIN
DECLARE @uIDCartella AS UNIQUEIDENTIFIER
DECLARE @uIDEpisodio AS UNIQUEIDENTIFIER
DECLARE @uIDTrasferimento AS UNIQUEIDENTIFIER
DECLARE @nQTA AS INTEGER
DECLARE @sInfo AS VARCHAR(MAX)
DECLARE @bErrore AS BIT
DECLARE @sTmp AS VARCHAR(MAX)
DECLARE @sNumeroNosologico AS VARCHAR(50)
DECLARE @sNumeroListaAttesa AS VARCHAR(50)

		SELECT @uIDCartella=IDCartella,
		  @nQta=COUNT(*)
	FROM T_MovTrasferimenti
	WHERE IDCartella 
		IN (SELECT ID FROM T_MovCartelle
			WHERE 
			NumeroCartella=@NumCartella
			)
		AND T_MovTrasferimenti.CodUA=@CodUA
	GROUP BY IDCartella
	
	SET @nQta=ISNULL(@nQta,0)
	
	IF @nQta =0 
		BEGIN
			SET @sInfo='ERRORE : nessuna cartella trovata'
			SET @bErrore=1
		END
	ELSE
																	BEGIN
								SET @sInfo='IDCartella: ' + CONVERT(VARCHAR(255),@uIDCartella)
				
				SELECT TOP 1 
					@uIDTrasferimento=ID,
					@uIDEpisodio=IDEpisodio
				FROM T_MovTrasferimenti WITH (NOLOCK) 
				WHERE IDCartella=@uIDCartella												
				
								SET @sTmp='IDTrasferimento: ' + CONVERT(VARCHAR(50),@uIDTrasferimento) + ' IDEpisodio: ' + ISNULL(CONVERT(VARCHAR(50),@uIDEpisodio),'')
				SET @sInfo=@sInfo + CHAR(13) + CHAR(10) + @sTmp				
				
								SET @sTmp=(SELECT TOP 1 
										ISNULL(Cognome,'') + ' ' + 
										ISNULL(Nome,'')  +
										' (' + CONVERT(VARCHAR(10), DataNascita,120) + ')'
										AS Paziente
								FROM T_MovPazienti WITH (NOLOCK)
								WHERE IDEpisodio=@uIDEpisodio)
								
				SET @sInfo=@sInfo + CHAR(13) + CHAR(10) + @sTmp			
				
								SELECT TOP 1 
					@sNumeroNosologico=ISNULL(NumeroNosologico,''),
					@sNumeroListaAttesa=ISNULL(NumeroListaAttesa,'')
				FROM T_MovEpisodi WITH (NOLOCK) 
				WHERE ID=@uIDEpisodio
				
				SET @sTmp=''
				
				IF ISNULL(@sNumeroNosologico,'') <>''
					SET @sTmp='Nosologico: ' + @sNumeroNosologico 
				IF ISNULL(@sNumeroListaAttesa,'') <>''
					SET @sTmp= @sTmp + ' ListaAttesa: ' + @sNumeroListaAttesa 						
					
				SET @sInfo=@sInfo + CHAR(13) + CHAR(10) + LTRIM(@sTmp)
				
				SET @bErrore=0
			END	
			
	
		SELECT	
		ISNULL(@bErrore,0) AS Errore,
		@sInfo AS Info

END